using System;
using System.Collections.Generic;

namespace ProjectApp.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CustomerCompany { get; set; }
        public string PerformerCompany { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int Priority { get; set; }

        // Внешний ключ для менеджера проекта
        public int ProjectManagerId { get; set; }
        public Employee ProjectManager { get; set; }

        // Связь многие-ко-многим с сотрудниками
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

        // Связь один-ко-многим с задачами
        public ICollection<Tasks> Tasks { get; set; } = new List<Tasks>();
    }
}
