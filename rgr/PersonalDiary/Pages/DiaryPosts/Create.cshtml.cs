using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages.DiaryPosts
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AdminSettings _adminSettings;

        public CreateModel(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            IOptions<AdminSettings> adminSettings)
        {
            _context = context;
            _userManager = userManager;
            _adminSettings = adminSettings.Value;
        }

        [BindProperty]
        public DiaryPost DiaryPost { get; set; }

         public IActionResult OnGet()
        {
            var currentUserEmail = _userManager.GetUserName(User);
            if (currentUserEmail != _adminSettings.AdminEmail)
            {
                return Forbid();
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var currentUserEmail = _userManager.GetUserName(User);
            if (currentUserEmail != _adminSettings.AdminEmail)
            {
                return Forbid(); 
            }

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
