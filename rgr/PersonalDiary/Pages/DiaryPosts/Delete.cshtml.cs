using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages.DiaryPosts
{
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AdminSettings _adminSettings;

        public DeleteModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IOptions<AdminSettings> adminSettings)
        {
            _context = context;
            _userManager = userManager;
            _adminSettings = adminSettings.Value;
        }

        [BindProperty]
        public DiaryPost DiaryPost { get; set; }

        public IActionResult OnGet(int id)
        {
             var currentUserEmail = _userManager.GetUserName(User);
            if (currentUserEmail != _adminSettings.AdminEmail)
            {
                return Forbid(); 
            }

            DiaryPost = _context.DiaryPosts.Find(id);
            if (DiaryPost == null)
            {
                return NotFound();
            }
            return Page();
        }

        public IActionResult OnPost(int id)
        {
            var currentUserEmail = _userManager.GetUserName(User);
            if (currentUserEmail != _adminSettings.AdminEmail)
            {
                return Forbid(); 
            }

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
