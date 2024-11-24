using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectApp.Data;
using ProjectApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectApp.Controllers
{
    public class TasksController : Controller
    {
        private readonly AppDbContext _context;

        public TasksController(AppDbContext context)
        {
            _context = context;
        }

        // Просмотр списка задач
        public async Task<IActionResult> Index(string statusFilter, string sortOrder)
        {
            var tasks = _context.Tasks
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .Include(t => t.Project)
                .AsQueryable();

            // Фильтрация по статусу
            if (!string.IsNullOrEmpty(statusFilter) &&
                Enum.TryParse<ProjectApp.Models.TaskStatus>(statusFilter, out var status))
            {
                tasks = tasks.Where(t => t.Status == status);
            }

            // Сортировка
            tasks = sortOrder switch
            {
                "name_desc" => tasks.OrderByDescending(t => t.Name),
                "author" => tasks.OrderBy(t => t.Author.FirstName),
                "author_desc" => tasks.OrderByDescending(t => t.Author.FirstName),
                "assignee" => tasks.OrderBy(t => t.Assignee.FirstName),
                "assignee_desc" => tasks.OrderByDescending(t => t.Assignee.FirstName),
                "priority" => tasks.OrderBy(t => t.Priority),
                "priority_desc" => tasks.OrderByDescending(t => t.Priority),
                "project" => tasks.OrderBy(t => t.Project.Name),
                "project_desc" => tasks.OrderByDescending(t => t.Project.Name),
                _ => tasks.OrderBy(t => t.Name), // Сортировка по умолчанию
            };

            // Подготовка ViewBag для фильтрации и сортировки
            ViewBag.CurrentSort = sortOrder;
            ViewBag.StatusFilter = statusFilter;

            return View(await tasks.ToListAsync());
        }

        // Просмотр деталей задачи
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null) return NotFound();

            return View(task);
        }

        // Отображение формы создания задачи (GET)
        public IActionResult Create()
        {
            ViewData["AuthorId"] = new SelectList(_context.Employees, "Id", "FirstName");
            ViewData["AssigneeId"] = new SelectList(_context.Employees, "Id", "FirstName");
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name");
            return View(new Tasks());
        }

        // Создание задачи (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,AuthorId,AssigneeId,ProjectId,Status,Comment,Priority")] Tasks task)
        {
            task.Author= await _context.Employees.FindAsync(task.AuthorId);
            task.Assignee= await _context.Employees.FindAsync(task.AssigneeId);
            task.Project= await _context.Projects.FindAsync(task.ProjectId);


            ViewData["AuthorId"] = new SelectList(_context.Employees, "Id", "FirstName", task.AuthorId);
            ViewData["AssigneeId"] = new SelectList(_context.Employees, "Id", "FirstName", task.AssigneeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", task.ProjectId);
            _context.Add(task);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            return View(task);
        }

        // Редактирование задачи (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks.FindAsync(id);
            if (task == null) return NotFound();

            ViewData["AuthorId"] = new SelectList(_context.Employees, "Id", "FirstName", task.AuthorId);
            ViewData["AssigneeId"] = new SelectList(_context.Employees, "Id", "FirstName", task.AssigneeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", task.ProjectId);
            return View(task);
        }

        // Редактирование задачи (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,AuthorId,AssigneeId,ProjectId,Status,Comment,Priority")] Tasks task)
        {
            if (id != task.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(task);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Tasks.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["AuthorId"] = new SelectList(_context.Employees, "Id", "FirstName", task.AuthorId);
            ViewData["AssigneeId"] = new SelectList(_context.Employees, "Id", "FirstName", task.AssigneeId);
            ViewData["ProjectId"] = new SelectList(_context.Projects, "Id", "Name", task.ProjectId);
            return View(task);
        }

        // Удаление задачи (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var task = await _context.Tasks
                .Include(t => t.Author)
                .Include(t => t.Assignee)
                .Include(t => t.Project)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (task == null) return NotFound();

            return View(task);
        }

        // Удаление задачи (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var task = await _context.Tasks.FindAsync(id);
            if (task != null)
            {
                _context.Tasks.Remove(task);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
