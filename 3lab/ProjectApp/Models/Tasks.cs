using System;

namespace ProjectApp.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int AuthorId { get; set; }
        public Employee Author { get; set; }
        public int? AssigneeId { get; set; }
        public Employee Assignee { get; set; }
        public TaskStatus Status { get; set; }
        public string Comment { get; set; }
        public int Priority { get; set; }
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }

    public enum TaskStatus
    {
        ToDo,
        InProgress,
        Done
    }
}
