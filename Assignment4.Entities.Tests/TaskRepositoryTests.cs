using System;
using System.Collections.Generic;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq;
using Xunit.Abstractions;

namespace Assignment4.Entities.Tests
{
    //all tests should be 'in-memory' testing moving forward, not through database

    public class TaskRepositoryTests : IDisposable
    {
        private readonly IKanbanContext _context;
        private readonly ITaskRepository _repo;

        public TaskRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            _context = context;
            _repo = new TaskRepository(_context);

            seed();
        }



        public void seed() {
            var js = new Tag { Name = "Javascript"};
            var ts = new Tag { Name = "Typescript"};
            var inferior = new Tag { Name = "C#"};
            var superior = new Tag { Name = "PHP"};
            var go = new Tag { Name = "Go"};

            _context.Tags.AddRange(new List<Tag>(){
                js,
                ts,
                inferior,
                superior,
                go
            });

            var goose = new User { Name = "Goose", Email = "goose@goose.dk" };
            var noMoreKings = new User { Name = "NoMoreKings", Email = "no@more.kings" };
            var fin = new User { Name = "fin", Email = "f@i.n" };
            var ac = new User { Name = "Aphrodite's child", Email = "aphrodite@s.child" };

            _context.Users.Add(goose);
            _context.SaveChanges();
            _context.Users.Add(noMoreKings);
            _context.SaveChanges();
            _context.Users.Add(fin);
            _context.SaveChanges();

            _context.Tasks.Add(new Task {
                Title = "End Of The World",
                AssignedTo = ac,
                Description = "Doomsday music",
                state = State.New,
                tags = new List<Tag>() {
                    js,
                    superior,
                    go
                }
            });
            _context.SaveChanges();
            _context.Tasks.Add(new Task {
                Title = "Critical Hit",
                AssignedTo = noMoreKings,
                Description = "Funky",
                state = State.Active,
                tags = new List<Tag>() {
                    js,
                    superior
                }
            });
            _context.SaveChanges();
            _context.Tasks.Add(new Task {
                Title = "Obey the groove",
                AssignedTo = noMoreKings,
                Description = "Funky",
                state = State.Removed,
                tags = new List<Tag>() {
                    js,
                    go
                }
            });
            _context.SaveChanges();
            _context.Tasks.Add(new Task {
                Title = "Ship in a Bottle",
                AssignedTo = fin,
                Description = "To be heard, not to be described (aka i have no clue)",
                state = State.Closed,
                tags = new List<Tag>() {
                    ts,
                    inferior
                }
            });
            _context.SaveChanges();
            _context.Tasks.Add(new Task {
                Title = "Anyone say over engineered?",
                AssignedTo = goose,
                Description = "How many technologies can this bad boy fit?",
                state = State.Resolved,
                tags = new List<Tag>() {
                    js,
                    ts,
                    inferior,
                    superior,
                    go
                }
            });
            _context.SaveChanges();
        }

        [Fact]
        public void Create_given_task_return_task_with_id()
        {
            var task = new TaskCreateDTO {
                Title = "Second",
                Description = "A description",
                Tags = new List<string>(){"Javascript", "Go"}
            };

            var created = _repo.Create(task);

            // Date.now is hard to test, and therefore ignored for now
            Assert.Equal(Response.Created, created.Item1);
            Assert.Equal(6, created.Item2.Id);
            Assert.Equal("Second", created.Item2.Title);
            Assert.Equal("", created.Item2.AssignedToName);
            Assert.True(created.Item2.Tags.SequenceEqual(new[] { "Javascript", "Go" }));
            Assert.Equal(State.New, created.Item2.State);
        }

        [Fact]
        public void Create_with_nonexisting_user_returns_BadRequest() {
            var task = new TaskCreateDTO {
                AssignedToId = 154,
            };

            var response = _repo.Create(task);
            Assert.Equal(Response.BadRequest, response.Item1);
        }

        [Fact]
        public void Update_update_task_given_new_state() {
            var task = new TaskUpdateDTO {
                Id = 1,
                Title = "Updated",
                Description = "Testing",
                State = State.Resolved,
                Tags = new List<string>() { "C#" },
                AssignedToId = 2,
            };

            var response = _repo.Update(task);

            var retrievedTask = _context.Tasks.Find(1);

            Assert.Equal(Response.Updated, response);
            Assert.Equal(State.Resolved, retrievedTask.state);
            Assert.Equal("Updated", retrievedTask.Title);
            Assert.Equal("Testing", retrievedTask.Description);
            Assert.Equal("NoMoreKings", retrievedTask.AssignedTo.Name);
            Assert.True(retrievedTask.tags.Select(t => t.Name).ToList().SequenceEqual(new[] { "C#" }));
        }

        [Fact]
        public void Update_update_with_nonexisting_user_returns_BadRequest() {
            var task = new TaskUpdateDTO {
                Id = 1,
                AssignedToId = 154,
            };

            var response = _repo.Update(task);
            Assert.Equal(Response.BadRequest, response);
        }

        [Fact]
        public void Update_update_with_nonexisting_id_returns_not_found() {
            var task = new TaskUpdateDTO {
                Id = 5112,
                AssignedToId = 154,
            };

            var response = _repo.Update(task);
            Assert.Equal(Response.NotFound, response);
        }

        [Fact]
        public void Delete_new_task_deletes_it() {
            var read = _context.Tasks.Find(1);
            Assert.NotNull(read);
            
            var response = _repo.Delete(1);

            read = _context.Tasks.Find(1);
            Assert.Equal(Response.Deleted, response);
            Assert.Null(read);
        }

       [Fact]
        public void Delete_active_task_sets_removed() {
            var response = _repo.Delete(2);

            Assert.Equal(Response.Deleted, response);
            Assert.Equal(State.Removed, _context.Tasks.Find(2).state);
        }

        [Fact]
        public void Delete_resolved_closed_removed_task_returns_conflict() {
            Assert.Equal(Response.Conflict, _repo.Delete(3)); // Removed
            Assert.Equal(Response.Conflict, _repo.Delete(4)); // Closed
            Assert.Equal(Response.Conflict, _repo.Delete(5)); // Resolved
        }

        [Fact]
        public void Delete_not_existing_task_returns_not_found() {
            Assert.Equal(Response.NotFound, _repo.Delete(512));
        }

        [Fact]
        public void Read_given_non_existing_id_returns_not_found() {
            var read = _repo.FindById(53);

            Assert.Equal(Response.NotFound, read.Item1);            
            Assert.Null(read.Item2);            
        }

        [Fact]
        public void Read_given_existing_id_return_task() {
            var read = _repo.FindById(1);

            Assert.Equal(Response.Success, read.Item1);
            Assert.Equal(1, read.Item2.Id);
            Assert.Equal("End Of The World", read.Item2.Title);
            Assert.Equal("Doomsday music", read.Item2.Description);
            Assert.Equal("Aphrodite's child", read.Item2.AssignedToName);
            Assert.True(read.Item2.Tags.SequenceEqual(new[] { "Javascript", "PHP", "Go" }));
            Assert.Equal(State.New, read.Item2.State);
        }

        /*
        [Fact]
        public void All_find_all_tasks() {
            _context.Tasks.Add(new Task {
                Title = "Second",
                Description = "Other task"
            });
            _context.SaveChanges();

            var tasks = _repo.All();

            Assert.Collection(tasks,
                c => Assert.Equal(new TaskDTO(1, "First", "A description"), c),
                c => Assert.Equal(new TaskDTO(2, "Second", "Other task"), c)
            );
        } 
*/
        public void Dispose() {
            _context.Dispose();
            _repo.Dispose();
        }
    }
}
