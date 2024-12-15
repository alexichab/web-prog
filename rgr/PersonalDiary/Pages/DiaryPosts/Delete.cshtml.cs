using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages.DiaryPosts
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public DeleteModel(ApplicationDbContext context)
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

        public IActionResult OnPost(int id)
        {
            var diaryPost = _context.DiaryPosts.Find(id);
            if (diaryPost == null)
            {
                return NotFound();
            }

            _context.DiaryPosts.Remove(diaryPost);
            _context.SaveChanges();
            return RedirectToPage("/Index");
        }
    }
}
