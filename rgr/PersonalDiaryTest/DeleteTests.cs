using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Moq;
using PersonalDiary.Data;
using PersonalDiary.Models;
using PersonalDiary.Pages.DiaryPosts;
using System.Security.Claims;
using Xunit;

namespace PersonalDiary.Tests
{
    public class DeleteModelTests
    {
        private ApplicationDbContext GetTestDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
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
        public void OnGet_NonAdminUser_ReturnsForbid()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("user@example.com");

            var deleteModel = new DeleteModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = deleteModel.OnGet(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void OnGet_PostDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var deleteModel = new DeleteModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = deleteModel.OnGet(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void OnGet_ValidAdminUser_ReturnsPage()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            var diaryPost = new DiaryPost { Id = 1, Title = "Test Post", Content = "Test Content", Mood = "Happy", Category = "Personal" };
            dbContext.DiaryPosts.Add(diaryPost);
            dbContext.SaveChanges();

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var deleteModel = new DeleteModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = deleteModel.OnGet(1);

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

            var deleteModel = new DeleteModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = deleteModel.OnPost(1);

            // Assert
            Assert.IsType<ForbidResult>(result);
        }

        [Fact]
        public void OnPost_PostDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var deleteModel = new DeleteModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = deleteModel.OnPost(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void OnPost_ValidAdminUser_DeletesPostAndRedirects()
        {
            // Arrange
            var dbContext = GetTestDbContext();
            var userManager = GetMockUserManager();
            var adminSettings = Options.Create(new AdminSettings { AdminEmail = "admin@example.com" });

            var diaryPost = new DiaryPost { Id = 1, Title = "Test Post", Content = "Test Content", Mood = "Happy", Category = "Personal" };
            dbContext.DiaryPosts.Add(diaryPost);
            dbContext.SaveChanges();

            userManager.Setup(um => um.GetUserName(It.IsAny<ClaimsPrincipal>())).Returns("admin@example.com");

            var deleteModel = new DeleteModel(dbContext, userManager.Object, adminSettings);

            // Act
            var result = deleteModel.OnPost(1);

            // Assert
            Assert.IsType<RedirectToPageResult>(result);

            var deletedPost = dbContext.DiaryPosts.Find(1);
            Assert.Null(deletedPost); // Пост должен быть удален
        }
    }
}
