using System;
using Assignment4.Core;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
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
            
            Assert.Equal(new TagDTO(2, "Second"), created);
        }

        [Fact]
        public void Read_given_non_existing_id_return_null() {
            var read = _repo.FindById(53);

            Assert.Null(read);            
        }

        [Fact]
        public void Read_given_existing_id_return_task() {
            var read = _repo.FindById(1);

            Assert.Equal(new TagDTO(1, "First"), read);
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
        public void All_find_all_tags() {
            _context.Tags.Add(new Tag { Name = "Second" });
            _context.SaveChanges();

            var tags = _repo.All();

            Assert.Collection(tags,
                c => Assert.Equal(new TagDTO(1, "First"), c),
                c => Assert.Equal(new TagDTO(2, "Second"), c)
            );
        }

        [Fact]
        public void Update_update_tag_given_new_data() {
            var tag = new TagDTO(1, "New name");

            _repo.Update(tag);
            
            Assert.Equal(tag, _repo.FindById(1));
        }

        public void Dispose() {
            _context.Dispose();
            _repo.Dispose();
        }
    }
}

