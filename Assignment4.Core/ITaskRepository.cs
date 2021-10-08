using System;
using System.Collections.Generic;


namespace Assignment4.Core
{
    public interface ITaskRepository : IDisposable
    {
        IReadOnlyCollection<TaskDTO> All();

        /// <summary>
        ///
        /// </summary>
        /// <param name="task"></param>
        /// <returns>The id of the newly created task</returns>
        TaskDTO Create(TaskCreateDTO task);

        void Delete(int taskId);

        TaskDetailsDTO FindById(int id);

        void Update(TaskDTO task);
    }
}
