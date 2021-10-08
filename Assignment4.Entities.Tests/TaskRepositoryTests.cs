using System;
using Assignment4.Entities;
using Assignment4.Core;
using Assignment4;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Assignment4.Entities.Tests
{
    //all tests should be 'in-memory' testing moving forward, not through database

    public class TaskRepositoryTests
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
            context.SaveChanges();

            _context = context;
            _repo = new TaskRepository(_context);
        }
        
        [Fact]
        public void Create_given_task_return_task_with_id()
        {
            var task = new TaskCreateDTO {
                Title = "First",
                Description = "A description"
            };

            var created = _repo.Create(task);
            
            Assert.Equal(new TaskDTO(1, "First", "A description"), created);
        }

    }
}
