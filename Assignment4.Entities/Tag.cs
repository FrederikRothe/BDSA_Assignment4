using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Assignment4.Entities
{
    //[Index(nameof(Tag.Name),IsUnique = true)]
    public class Tag
    {
        public int Id { get; set; }
        [StringLength(50)]
        [Required]
        public string Name { get; set; }
        public ICollection<Task> Tasks { get; set; }
    }
}
