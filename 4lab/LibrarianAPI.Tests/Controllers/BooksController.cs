using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using LibrarianAPI.Controllers;
using LibrarianAPI.Data;
using LibrarianAPI.Models;

namespace LibrarianAPI.Tests.Controllers
{
    public class BooksControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public BooksControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDB")
                .Options;
        }

        [Fact]
        public async Task AddBook_ReturnsCreatedAtActionResult()//Добавление книги
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new BooksController(context);

            var newBook = new Book { Title = "The Great Gatsby", Author = "F. Scott Fitzgerald", ISBN = "9780743273565", Year = 1925, Copies = 3 };
            var result = await controller.AddBook(newBook);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var book = Assert.IsType<Book>(createdAtActionResult.Value);
            Assert.Equal("The Great Gatsby", book.Title);
        }

        [Fact]
        public async Task UpdateBook_ReturnsNoContentResult()//Обновление
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new BooksController(context);

            var book = new Book { Id = 1, Title = "Old Title", Author = "Old Author", ISBN = "1111111111", Year = 2000, Copies = 10 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var updatedBook = new Book { Title = "New Title", Author = "New Author", ISBN = "1111111111", Year = 2022, Copies = 5 };
            var result = await controller.UpdateBook(book.Id, updatedBook);

            Assert.IsType<NoContentResult>(result);
            var dbBook = await context.Books.FindAsync(book.Id);
            Assert.Equal("New Title", dbBook.Title);
            Assert.Equal("New Author", dbBook.Author);
        }

        [Fact]
        public async Task DeleteBook_ReturnsNoContentResult()//Удаление
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new BooksController(context);

            var book = new Book { Id = 2, Title = "Book to Delete", Author = "Unknown Author", ISBN = "2222222222", Year = 2010, Copies = 2 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var result = await controller.DeleteBook(book.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Books.FindAsync(book.Id));
        }

        [Fact]
        public async Task GetBookById_ReturnsBook()//по ID
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new BooksController(context);

            var book = new Book { Id = 3, Title = "1984", Author = "George Orwell", ISBN = "9780451524935", Year = 1949, Copies = 4 };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var result = await controller.GetBookById(book.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedBook = Assert.IsType<Book>(okResult.Value);
            Assert.Equal("1984", returnedBook.Title);
        }

        [Fact]
        public async Task GetAvailableBooks_ReturnsAvailableBooks()//Получение доступных книг 
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new BooksController(context);
            context.Books.RemoveRange(context.Books);

            context.Books.AddRange(
                new Book { Title = "Moby Dick", Author = "Herman Melville", ISBN = "9781503280786", Year = 1851, Copies = 3 },
                new Book { Title = "Out of Stock", Author = "Author Unknown", ISBN = "0000000000", Year = 2023, Copies = 0 }
            );
            await context.SaveChangesAsync();

            var result = await controller.GetAvailableBooks();
            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsAssignableFrom<List<Book>>(okResult.Value);

            Assert.Single(books);
            Assert.Equal("Moby Dick", books[0].Title);
        }

        [Fact]
        public async Task SearchBooks_ReturnsMatchingBooks()//Поиск
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new BooksController(context);

            context.Books.AddRange(
                new Book { Title = "C# Programming", Author = "John Doe", ISBN = "3333333333", Year = 2020, Copies = 5 },
                new Book { Title = "Learning ASP.NET", Author = "Jane Smith", ISBN = "4444444444", Year = 2021, Copies = 7 }
            );
            await context.SaveChangesAsync();

            var result = await controller.SearchBooks("C#");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var books = Assert.IsAssignableFrom<List<Book>>(okResult.Value);

            Assert.Single(books);
            Assert.Equal("C# Programming", books[0].Title);
        }
    }
}
