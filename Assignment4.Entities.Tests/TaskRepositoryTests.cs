using System;
using Assignment4.Entities;
using Assignment4.Core;
using Assignment4;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Collections.Generic;

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

            context.Tasks.Add(new Task {
                Title = "First",
                Description = "A description"
            });
            context.SaveChanges();

            _context = context;
            _repo = new TaskRepository(_context);
        }
        
        [Fact]
        public void Create_given_task_return_task_with_id()
        {
            var task = new TaskCreateDTO {
                Title = "Second",
                Description = "A description"
            };

            var created = _repo.Create(task);
            
            Assert.Equal(new TaskDTO(2, "Second", "A description"), created);
        }

        [Fact]
        public void Read_given_non_existing_id_return_null() {
            var read = _repo.FindById(53);

            Assert.Null(read);            
        }

        [Fact]
        public void Read_given_existing_id_return_task() {
            var read = _repo.FindById(1);

            Assert.Equal(new TaskDetailsDTO {
                Id = 1,
                Title = "First",
                Description = "A description",
                State = State.New
            }, read);
        }

        [Fact]
        public void Delete_given_id_delete_and_read_returns_null() {
            var read = _repo.FindById(1);
            _repo.Delete(1);
            Assert.NotNull(read);

            read = _repo.FindById(1);
            Assert.Null(read);
        }

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

        [Fact]
        public void Update_update_task_given_new_data() {
            var task = new TaskDTO(1, "New title", "Newer");

            _repo.Update(task);
            
            Assert.Equal(new TaskDetailsDTO {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description
            }, _repo.FindById(1));
        }

        public void Dispose() {
            _context.Dispose();
            _repo.Dispose();
        }
    }
}
