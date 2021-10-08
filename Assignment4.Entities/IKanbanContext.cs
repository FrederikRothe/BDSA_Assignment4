using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Assignment4.Entities
{
    public interface IKanbanContext : IDisposable {
        DbSet<Tag> Tags { get; set; }
        DbSet<User> Users { get; set; }
        DbSet<Task> Tasks { get; set; }    

        int SaveChanges();
    }
}