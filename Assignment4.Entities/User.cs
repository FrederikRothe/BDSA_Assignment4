using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities
{
    //[Index(nameof(User.Email),IsUnique = true)]
    public class User
    {
        public int Id { get; set; }
        [StringLength(100)]
        [Required]
        public string Name { get; set; }

        [StringLength(100)]
        [Required]
        public string Email { get; set; }
        public ICollection<Task> Tasks { get; set; }

    }
}
