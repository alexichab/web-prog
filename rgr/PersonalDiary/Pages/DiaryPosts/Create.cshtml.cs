using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages.DiaryPosts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DiaryPost DiaryPost { get; set; }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DiaryPosts.Add(DiaryPost);
            _context.SaveChanges();
            return RedirectToPage("/Index");
        }
    }
}
