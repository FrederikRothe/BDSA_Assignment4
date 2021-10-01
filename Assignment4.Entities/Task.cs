using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Assignment4.Core;

namespace Assignment4.Entities
{
    public class Task
    {
        public int Id { get; set; }

        [StringLength(100)]
        [Required]
        public string Title { get; set; }
        public User? AssignedTo { get; set; }
        public string Description { get; set; }
        public State state { get; set; }
        public ICollection<Tag> tags { get; set; }
    }
}
