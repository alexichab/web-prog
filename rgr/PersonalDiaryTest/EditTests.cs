using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Moq;
using PersonalDiary.Data;
using PersonalDiary.Models;
using PersonalDiary.Pages.DiaryPosts;
using System.Security.Claims;
using Xunit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PersonalDiary.Tests
{
    public class EditModelTests
    {
        private ApplicationDbContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase("TestDatabase_Edit")
                .Options;
            return new ApplicationDbContext(options);
        }

        private Mock<UserManager<IdentityUser>> GetMockUserManager()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            return new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }

        [Fact]
        public void OnGet_AdminUser_ReturnsPage()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            // Добавляем тестовую запись
            var testDiaryPost = new DiaryPost
            {
                Id = 1,
                Title = "Test Title",
                Content = "Test Content",
                Mood = "Хорошо",
                Category = "Личное",
                CreatedAt = DateTime.Now
            };
            dbContext.DiaryPosts.Add(testDiaryPost);
            dbContext.SaveChanges();

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var editModel = new EditModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = editModel.OnGet(1);

            // Assert
            Assert.IsType<PageResult>(result);
            Assert.Equal(testDiaryPost.Title, editModel.DiaryPost.Title); // Проверяем загрузку записи
        }

        [Fact]
        public void OnGet_NonAdminUser_ReturnsForbid()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("user@example.com");

            var editModel = new EditModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = editModel.OnGet(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void OnGet_PostNotFound_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var editModel = new EditModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = editModel.OnGet(99); // Невалидный ID

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void OnPost_ValidAdminUser_UpdatesPostAndRedirects()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            // Добавляем тестовую запись
            var testDiaryPost = new DiaryPost
            {
                Id = 2,
                Title = "Old Title",
                Content = "Old Content",
                Mood = "Нормально",
                Category = "Работа",
                CreatedAt = DateTime.Now
            };
            dbContext.DiaryPosts.Add(testDiaryPost);
            dbContext.SaveChanges();

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var editModel = new EditModel(dbContext, userManager.Object, adminSettings)
            {
                DiaryPost = dbContext.DiaryPosts.Find(2) // Получаем отслеживаемый объект из контекста
            };

            // Обновляем данные записи
            editModel.DiaryPost.Title = "New Title";
            editModel.DiaryPost.Content = "New Content";

            // Act
            var result = editModel.OnPost();

            // Assert
            Assert.IsType<RedirectToPageResult>(result);

            var updatedDiaryPost = dbContext.DiaryPosts.Find(2);
            Assert.Equal("New Title", updatedDiaryPost.Title); // Проверяем обновление
            Assert.Equal("New Content", updatedDiaryPost.Content);
        }

        [Fact]
        public void OnPost_InvalidModel_ReturnsPage()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var editModel = new EditModel(dbContext, userManager.Object, adminSettings)
            {
                DiaryPost = new DiaryPost
                {
                    Id = 1,
                    Title = "", // Пустой заголовок
                    Content = "New Content",
                    Mood = "Хорошо",
                    Category = "Личное"
                }
            };
            editModel.ModelState.AddModelError("Title", "The Title field is required."); // Добавляем ошибку модели

            // Act
            var result = editModel.OnPost();

            // Assert
            Assert.IsType<PageResult>(result);
        }

        [Fact]
        public void OnPost_NonAdminUser_ReturnsForbid()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("user@example.com");

            var editModel = new EditModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = editModel.OnPost();

            // Assert
            Assert.IsType<ForbidResult>(result);
        }
    }
}
