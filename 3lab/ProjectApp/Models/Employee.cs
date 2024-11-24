using System.Collections.Generic;

namespace ProjectApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        
        // Связь многие-ко-многим с проектами
        public ICollection<Project> Projects { get; set; } = new List<Project>();
        
        // Связь один-ко-многим с авторскими задачами
        public ICollection<Tasks> AuthoredTasks { get; set; } = new List<Tasks>();
        
        // Связь один-ко-многим с задачами как исполнитель
        public ICollection<Tasks> AssignedTasks { get; set; } = new List<Tasks>();

        // Связь один-ко-многим с проектами, которые этот сотрудник управляет (если этот сотрудник - менеджер)
        public ICollection<Project> ManagedProjects { get; set; } = new List<Project>();

        public string FullName => $"{FirstName} {LastName}";
    }
}
