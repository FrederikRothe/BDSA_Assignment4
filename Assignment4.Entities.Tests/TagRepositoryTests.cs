using System;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Xunit;

namespace Assignment4.Entities.Tests
{
    public class TagRepositoryTests : IDisposable
    {
        private readonly IKanbanContext _context;
        private readonly ITagRepository _repo;

        public TagRepositoryTests()
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            var builder = new DbContextOptionsBuilder<KanbanContext>();
            builder.UseSqlite(connection);
            var context = new KanbanContext(builder.Options);
            context.Database.EnsureCreated();

            context.Tags.Add(new Tag { Name = "First"});
            context.SaveChanges();

            _context = context;
            _repo = new TagRepository(_context);
        }
        
        [Fact]
        public void Create_given_tag_return_tag_with_id()
        {
            var tag = new TagCreateDTO("Second");
            var created = _repo.Create(tag);
            
            Assert.Equal(Response.Created, created.Item1);
            Assert.Equal(new TagDTO(2, "Second"), created.Item2);
        }

        [Fact]
        public void Create_given_existing_tag_returns_conflict()
        {
            var tag = new TagCreateDTO("First");
            var created = _repo.Create(tag);
            
            Assert.Equal(Response.Conflict, created.Item1);
            Assert.Null(created.Item2);
        }

        [Fact]
        public void Read_given_non_existing_id_return_null() {
            var read = _repo.FindById(53);

            Assert.Equal(Response.NotFound, read.Item1);
            Assert.Null(read.Item2);            
        }

        [Fact]
        public void Read_given_existing_id_return_task() {
            var read = _repo.FindById(1);

            Assert.Equal(Response.Success, read.Item1);
            Assert.Equal(new TagDTO(1, "First"), read.Item2);
        }

        [Fact]
        public void Delete_given_id_delete_and_read_returns_null() {
            var read = _repo.FindById(1);
            Assert.NotNull(read.Item2);
            
            var response = _repo.Delete(1);
            Assert.Equal(Response.Deleted, response);

            read = _repo.FindById(1);
            Assert.Null(read.Item2);
        }

        [Fact]
        public void Delete_given_non_existing_id_returns_not_found() {
            var response = _repo.Delete(59);
            Assert.Equal(Response.NotFound, response);
        }

        [Fact]
        public void Delete_tag_in_use_returns_conflict() {
            var entity = _context
                .Tags
                .Include(t => t.Tasks)
                .Where(t => t.Id == 1)
                .First();

            entity.Tasks.Add(new Task {
                Title = "Task",
                Description = "Blocking"
            });

            _context.SaveChanges();

            var response = _repo.Delete(1);
            Assert.Equal(Response.Conflict, response);
        }


        [Fact]
        public void All_find_all_tags() {
            _context.Tags.Add(new Tag { Name = "Second" });
            _context.SaveChanges();

            var tags = _repo.All();

            Assert.Equal(Response.Success, tags.Item1);
            Assert.Collection(tags.Item2,
                c => Assert.Equal(new TagDTO(1, "First"), c),
                c => Assert.Equal(new TagDTO(2, "Second"), c)
            );
        }

        [Fact]
        public void Update_update_tag_given_new_data() {
            var tag = new TagDTO(1, "New name");

            var response = _repo.Update(tag);
            
            Assert.Equal(Response.Updated, response);
            Assert.Equal(tag, _repo.FindById(1).Item2);
        }

        [Fact]
        public void Update_given_non_existing_id_returns_not_found() {
            var response = _repo.Update(new TagDTO(59, "Not existing"));
            Assert.Equal(Response.NotFound, response);
        }


        public void Dispose() {
            _context.Dispose();
            _repo.Dispose();
        }
    }
}