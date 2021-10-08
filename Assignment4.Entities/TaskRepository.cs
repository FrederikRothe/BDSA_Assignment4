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
using System.Linq;

namespace Assignment4.Entities

/*

OBS: class is currently untested

OBS: class should be rewritten to use kanbanContext instead of RawSql
*/


{
    public class TaskRepository : ITaskRepository
    {
       private readonly IKanbanContext _context;

        public TaskRepository(IKanbanContext context)
        {
            _context = context;     
        }

        
        //skipped assignedTo, didnt finish State call
        //figure out parsing from string to enum
        public IReadOnlyCollection<TaskDTO> Read()
        {
            var tasks = from t in _context.Tasks
                select new TaskDTO(
                    t.Id,
                    t.Title,
                    t.Description
                    // State = t.state
                );

           return new ReadOnlyCollection<TaskDTO>(tasks.ToList());
        }

        public void Delete(int taskId)
        {
            throw new NotImplementedException();
            // _context.Remove(_context.Tasks.Single(t => t.Id == taskId));
        }


        public void Dispose()
        {
             _context.Dispose();
        }

        public IReadOnlyCollection<TaskDTO> All()
        {
            throw new NotImplementedException();
        }

        public TaskDTO Create(TaskCreateDTO task)
        {
            var created = new Task {
                Title = task.Title,
                Description = task.Description
            };

            _context.Tasks.Add(created);
            _context.SaveChanges();

            return new TaskDTO(created.Id, created.Title, created.Description);            
        }

        public TaskDetailsDTO FindById(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(TaskDTO task)
        {
            throw new NotImplementedException();
        }
    }
}
