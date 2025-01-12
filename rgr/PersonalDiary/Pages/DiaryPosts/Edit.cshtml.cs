using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using PersonalDiary.Data;
using PersonalDiary.Models;

namespace PersonalDiary.Pages.DiaryPosts
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly AdminSettings _adminSettings;

        public EditModel( ApplicationDbContext context,
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
