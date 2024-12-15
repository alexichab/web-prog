using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages.DiaryPosts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DiaryPost DiaryPost { get; set; }
        public IActionResult OnGet(int id)
        {
            DiaryPost = _context.DiaryPosts.Find(id);
            if (DiaryPost == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            
            // Получить оригинальную запись из базы данных
            var existingPost = _context.DiaryPosts.FirstOrDefault(p => p.Id == DiaryPost.Id);
            if (existingPost == null)
            {
                return NotFound();
            }

            // Сохранить оригинальное значение CreatedAt
            DiaryPost.CreatedAt = existingPost.CreatedAt;
            DiaryPost.UpdatedAt = DateTime.Now;
            _context.Entry(existingPost).CurrentValues.SetValues(DiaryPost);
            _context.SaveChanges();
            return RedirectToPage("/Index");
        }
    }
}
