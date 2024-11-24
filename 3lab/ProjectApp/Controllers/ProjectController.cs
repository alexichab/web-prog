using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectApp.Data;
using ProjectApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectApp.Controllers
{
    public class ProjectsController : Controller
    {
        private readonly AppDbContext _context;

        public ProjectsController(AppDbContext context)
        {
            _context = context;
        }

        // Отображение списка проектов
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? priority, string sortOrder)
        {
            var projects = _context.Projects.Include(p => p.Tasks).Include(p => p.ProjectManager).AsQueryable();

            // Фильтрация по дате
            if (startDate.HasValue && endDate.HasValue)
            {
                projects = projects.Where(p => p.StartDate >= startDate && p.StartDate <= endDate);
            }

            // Фильтрация по приоритету
            if (priority.HasValue)
            {
                projects = projects.Where(p => p.Priority == priority);
            }

            // Сортировка
            projects = sortOrder switch
            {
                "name_desc" => projects.OrderByDescending(p => p.Name),
                "date_asc" => projects.OrderBy(p => p.StartDate),
                "date_desc" => projects.OrderByDescending(p => p.StartDate),
                "priority_asc" => projects.OrderBy(p => p.Priority),
                "priority_desc" => projects.OrderByDescending(p => p.Priority),
                _ => projects.OrderBy(p => p.Name),
            };

            return View(await projects.ToListAsync());
        }

        // Просмотр деталей проекта
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Employees)
                .Include(p => p.Tasks)
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            return View(project);
        }

        // Создание проекта (GET)
        public async Task<IActionResult> Create()
        {
            ViewData["ProjectManagerId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FirstName");
            return View();
        }

        // Создание проекта (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,CustomerCompany,PerformerCompany,StartDate,EndDate,Priority,ProjectManagerId")] Project project)
        {
            project.ProjectManager = await _context.Employees.FindAsync(project.ProjectManagerId);

            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage);
            }
            _context.Add(project);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Редактирование проекта (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects.FindAsync(id);
            if (project == null) return NotFound();

            ViewData["ProjectManagerId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FirstName", project.ProjectManagerId);
            return View(project);
        }

        // Редактирование проекта (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,CustomerCompany,PerformerCompany,StartDate,EndDate,Priority,ProjectManagerId")] Project project)
        {
            if (id != project.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    project.ProjectManager = await _context.Employees.FindAsync(project.ProjectManagerId);
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Projects.Any(p => p.Id == project.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["ProjectManagerId"] = new SelectList(await _context.Employees.ToListAsync(), "Id", "FirstName", project.ProjectManagerId);
            return View(project);
        }

        // Удаление проекта (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.ProjectManager)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            return View(project);
        }

        // Удаление проекта (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Добавление сотрудника к проекту (GET)
        public async Task<IActionResult> AddEmployee(int? id)
        {
            if (id == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            var availableEmployees = await _context.Employees
                .Where(e => !project.Employees.Contains(e))
                .ToListAsync();

            ViewBag.Employees = new SelectList(availableEmployees, "Id", "FullName");
            return View(project);
        }

        // Добавление сотрудника к проекту (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEmployee(int id, int employeeId)
        {
            var project = await _context.Projects
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (project == null) return NotFound();

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return NotFound();

            if (!project.Employees.Contains(employee))
            {
                project.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = project.Id });
        }

        // Удаление сотрудника с проекта (GET)
        public async Task<IActionResult> RemoveEmployee(int? projectId, int? employeeId)
        {
            if (projectId == null || employeeId == null) return NotFound();

            var project = await _context.Projects
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.Id == projectId);

            if (project == null) return NotFound();

            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null) return NotFound();

            if (project.Employees.Contains(employee))
            {
                project.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Details), new { id = project.Id });
        }

        
    }
}
