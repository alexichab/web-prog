using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<DiaryPost> DiaryPosts { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedCategory { get; set; }
        public void OnGet()
        {
            DiaryPosts = string.IsNullOrEmpty(SelectedCategory)
                ? _context.DiaryPosts.OrderByDescending(p => p.CreatedAt).ToList()
                : _context.DiaryPosts
                    .Where(p => p.Category == SelectedCategory)
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();
        }
    }
}
