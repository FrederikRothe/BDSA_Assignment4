using System.Collections.Generic;

namespace Assignment4.Core
{
    // TODO: Assigned to
    public record TaskDTO(int Id, string Title, string Description);

    public record TaskDetailsDTO
    {
        public int Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public int? AssignedToId { get; init; }
        public string AssignedToName { get; init; }
        public string AssignedToEmail { get; init; }
        public IEnumerable<string> Tags { get; init; }
        public State State { get; init; }
    }

    public record TaskCreateDTO
    {
        public string Title { get; init; }
        public string Description { get; init; }
        public int? AssignedToId { get; init; }
        public IReadOnlyCollection<string> Tags { get; init; }
        public State State { get; init; }
    }
}