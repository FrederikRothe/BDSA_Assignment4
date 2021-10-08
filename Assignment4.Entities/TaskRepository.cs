using Assignment4.Core;
using System.Collections.Generic;
using Assignment4;
using Assignment4.Entities;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using Microsoft.Data.SqlClient;
using System.Data;
using System;

namespace Assignment4.Entities

/*

OBS: class is currently untested

OBS: class should be rewritten to use kanbanContext instead of RawSql
*/


{
    public class TaskRepository : ITaskRepository
    {
       private readonly SqlConnection _connection;

        public TaskRepository(SqlConnection connection)
        {
            _connection = connection;     
        }

        
        //skipped assignedTo, didnt finish State call
        //figure out parsing from string to enum
        public IReadOnlyCollection<TaskDTO> All()
        {
            List<TaskDTO> temporaryList = new List<TaskDTO>();
            var cmdText = @"SELECT t.Id, t.Title,t.Description, t.state
                            FROM Tasks AS t
                            ORDER BY t.Id";

            using var command = new SqlCommand(cmdText, _connection);

            OpenConnection();

            using var reader = command.ExecuteReader();

            while (reader.Read())
            {
                var task = new TaskDTO
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    //check how to parse from string to enum again
                    //State = Enum.Parse(State,reader.GetString("State")),
 
                };
                temporaryList.Add(task);
            }
            CloseConnection();

            ReadOnlyCollection<TaskDTO> listOfTasksDTO = new ReadOnlyCollection<TaskDTO>(temporaryList); 

            return  listOfTasksDTO;

        }

        public void Delete(int taskId)
        {
            var cmdText = @"DELETE Tasks WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id",taskId);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }


        public void Dispose()
        {
             _connection.Dispose();
        }


        //skipped assingnedTo, for now
        //figure out parsing of string to enum
        public TaskDetailsDTO FindById(int id)
        {
            var cmdText = @"SELECT t.Id, t.Title, t.Description, t.State
                            FROM Tasks AS t
                            WHERE c.Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", id);

            OpenConnection();

            using var reader = command.ExecuteReader();

            var taskDetails = reader.Read()
                ? new TaskDetailsDTO
                {
                    Id = reader.GetInt32("Id"),
                    Title = reader.GetString("Title"),
                    Description = reader.GetString("Description"),
                    //State = Parse.Enum(reader.GetString("State")),
 
                }
                : null;

            CloseConnection();

            return taskDetails;
        }

        //skipped assignedTo, for now
        public void Update(TaskDTO task)
 {
            var cmdText = @"UPDATE Tasks SET
                            Title = @Title,
                            Description = @Description,
                            State = @State,
                            WHERE Id = @Id";

            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Id", task.Id);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@State", task.State);
            command.Parameters.AddWithValue("@Title",task.Title);

            OpenConnection();

            command.ExecuteNonQuery();

            CloseConnection();
        }

        //skipped AssignedTo, for now
        public int Create(TaskDTO task)
        {
            //why scope identity? ask grp
            var cmdText = @"INSERT Task (Title, Description, State )
                                Values (@Title, @Description, @State);
                                SELECT SCOPE_IDENTITY()";
            using var command = new SqlCommand(cmdText, _connection);

            command.Parameters.AddWithValue("@Title", task.Title);
            command.Parameters.AddWithValue("@Description", task.Description);
            command.Parameters.AddWithValue("@State", task.State);

            OpenConnection();

            var id = command.ExecuteScalar();

            CloseConnection();

            return (int)id;
                        
        }

        private void OpenConnection()
        {
            if (_connection.State == ConnectionState.Closed)
            {
                _connection.Open();
            }
        }

         private void CloseConnection()
        {
            if (_connection.State == ConnectionState.Open)
            {
                _connection.Close();
            }
        }


    }
}
