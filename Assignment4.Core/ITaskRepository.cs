using System;
using System.Collections.Generic;


namespace Assignment4.Core
{
    public interface ITaskRepository : IDisposable
    {
        (Response, IReadOnlyCollection<TaskDTO>) All();
        
        (Response, IReadOnlyCollection<TaskDTO>) AllRemoved();
        
        (Response, IReadOnlyCollection<TaskDTO>) AllByTag(string tag);
        
        (Response, IReadOnlyCollection<TaskDTO>) AllByUser(int userId);
        
        (Response, IReadOnlyCollection<TaskDTO>) AllByState(State state);

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <returns>The id of the newly created task</returns>
        (Response, TaskDTO) Create(TaskCreateDTO task);

        Response Delete(int taskId);

        (Response, TaskDetailsDTO) FindById(int taskId);

        Response Update(TaskUpdateDTO task);
    }
}
