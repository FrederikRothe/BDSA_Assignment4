using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Assignment4.Core;
using System;

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
        public State State { get; set; } // TODO: Rename to State
        public ICollection<Tag> Tags { get; set; } // TODO: Renamte to Tags
        public DateTime StateUpdated { get; set; }
        public DateTime Created { get; set; }
    }
}
