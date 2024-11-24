using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectApp.Data;
using ProjectApp.Models; // Пространство имен для ваших моделей
using System.Threading.Tasks;

namespace ProjectApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Получаем данные из базы
            var projects = await _context.Projects.ToListAsync();
            var employees = await _context.Employees.ToListAsync();
            var tasks = await _context.Tasks
                .Include(t => t.Author)     // Подключаем автора задачи
                .Include(t => t.Assignee)  // Подключаем исполнителя задачи
                .Include(t => t.Project)   // Подключаем проект задачи
                .ToListAsync();

            // Создаем объект ViewModel
            var viewModel = new HomeViewModel
            {
                Projects = projects,
                Employees = employees,
                Tasks = tasks
            };

            // Возвращаем данные в представление
            return View(viewModel);
        }
    }
}
