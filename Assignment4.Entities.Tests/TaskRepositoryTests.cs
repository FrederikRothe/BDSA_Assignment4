using System;

namespace Assignment4.Entities.Tests
{
    //all tests should be 'in-memory' testing moving forward, not through database

    public class TaskRepositoryTests
    {

        [Fact]
        public void CreatedFirstTask()
        {
        using var connection = new SqlConnection(connectionString);
        var repo = new TaskRepository(connection);

        TaskDTO task = new TaskDTO{Title = "Working", Description = "coding", state = State.New};

        repo.Create(task);  

        // NOT FINISHED, connection test failed  
        }

    }
}
