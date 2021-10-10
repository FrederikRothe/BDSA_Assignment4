using Assignment4.Core;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Data;
using System;
using System.Linq;

namespace Assignment4.Entities
{
    public class TaskRepository : ITaskRepository
    {
        private readonly IKanbanContext _context;

        public TaskRepository(IKanbanContext context)
        {
            _context = context;
        }

        public (Response, IReadOnlyCollection<TaskDTO>) All()
        {
            var tasks = from t in GetSource()
                        select TaskToTaskDTO(t);

            return (Response.Success, tasks.ToList());
        }

        public (Response, IReadOnlyCollection<TaskDTO>) AllRemoved()
        {
            var tasks = from t in GetSource()
                        where t.state == State.Removed
                        select TaskToTaskDTO(t);

            return (Response.Success, tasks.ToList());
        }

        public (Response, IReadOnlyCollection<TaskDTO>) AllByTag(string tag)
        {
            var tasks = from t in GetSource()
                        where t.tags.Where(t => t.Name == tag).Any()
                        select TaskToTaskDTO(t);

            return (Response.Success, tasks.ToList());
        }

        public (Response, IReadOnlyCollection<TaskDTO>) AllByUser(int userId)
        {
            var tasks = from t in GetSource()
                        where t.AssignedTo.Id == userId
                        select TaskToTaskDTO(t);

            return (Response.Success, tasks.ToList());
        }

        public (Response, IReadOnlyCollection<TaskDTO>) AllByState(State state)
        {
            var tasks = from t in GetSource()
                        where t.state == state
                        select TaskToTaskDTO(t);

            return (Response.Success, tasks.ToList());
        }

        public Response Delete(int taskId)
        {
            var entity = _context.Tasks.Find(taskId);

            if (entity == null) {
                return Response.NotFound;
            }

            switch (entity.state)
            {
                case State.New:
                    _context.Tasks.Remove(entity);
                    break;
                case State.Active:
                    entity.state = State.Removed;
                    break;
                case State.Resolved:
                case State.Closed:
                case State.Removed:
                default:
                    return Response.Conflict;
            }

            _context.SaveChanges();
            return Response.Deleted;
        }

        public (Response, TaskDTO) Create(TaskCreateDTO task)
        {
            var user = GetUser(task.AssignedToId);

            if (user == null && task.AssignedToId != null) {
                return (Response.BadRequest, null);
            }

            var tags = GetTags(task.Tags);

            var created = new Task
            {
                Title = task.Title,
                Description = task.Description,
                AssignedTo = user,
                tags = tags.ToList(),
                state = State.New,
                Created = DateTime.UtcNow,
                StateUpdated = DateTime.UtcNow
            };

            _context.Tasks.Add(created);
            _context.SaveChanges();

            return (Response.Created, TaskToTaskDTO(created));
        }

        public (Response, TaskDetailsDTO) FindById(int id)
        {
            var tasks = from t in (_context
                .Tasks
                .Include(t => t.tags)
                .Include(t => t.AssignedTo)
            )
                        where t.Id == id
                        select new TaskDetailsDTO(
                            t.Id,
                            t.Title,
                            t.Description,
                            t.Created,
                            t.AssignedTo == null ? "" : t.AssignedTo.Name,
                            t.tags.Select(tag => tag.Name).ToList(),
                            t.state,
                            t.StateUpdated
                        );

            var entity = tasks.FirstOrDefault();

            if (entity == null)
            {
                return (Response.NotFound, null);
            }

            return (Response.Success, tasks.FirstOrDefault());
        }

        public Response Update(TaskUpdateDTO task)
        {
            var entity = _context.Tasks.Find(task.Id);

            if (entity == null) {
                return Response.NotFound;
            }

            if (task.Tags != null) {
                entity.tags = GetTags(task.Tags).ToList(); 
            }

            var assignedTo = GetUser(task.AssignedToId);

            if (assignedTo == null && task.AssignedToId != null)
            {
                return Response.BadRequest;
            }

            entity.AssignedTo = assignedTo;
            entity.Title = task.Title;
            entity.Description = task.Description;

            if (task.State != entity.state) {
                entity.state = task.State;
                entity.StateUpdated = DateTime.UtcNow;
            }
            
            _context.SaveChanges();

            return Response.Updated;
        }

        private IIncludableQueryable<Task, User> GetSource() {
            return _context
                .Tasks
                .Include(t => t.tags)
                .Include(t => t.AssignedTo);
        }

        private User GetUser(int? userId)
        {
            if (userId == null) {
                return null;
            }

            return _context.Users.Find(userId);
        }

        private IEnumerable<Tag> GetTags(IEnumerable<string> tags) {
            // FIXME: There is no guarantee that all the tags specified are actually here
            var entities = _context.Tags.Where(t => tags.Contains(t.Name)).ToDictionary(t => t.Name);

            foreach (var tag in tags)
            {
                yield return entities.TryGetValue(tag, out var t) ? t : new Tag { Name = tag };
            } 
        }

        private TaskDTO TaskToTaskDTO(Task task) {
            return new TaskDTO(
                task.Id,
                task.Title,
                task.AssignedTo == null ? "" : task.AssignedTo.Name,
                task.tags.Select(t => t.Name).ToList(),
                task.state
            );
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
