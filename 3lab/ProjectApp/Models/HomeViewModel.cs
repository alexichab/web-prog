using ProjectApp.Models;    
using System.Collections.Generic;

namespace ProjectApp.Models
{
    public class HomeViewModel
    {

        public List<Project> Projects { get; set; }
        public List<Employee> Employees { get; set; }
        public List<Tasks> Tasks { get; set; }
    }
}