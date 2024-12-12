using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;
using LibrarianAPI.Controllers;
using LibrarianAPI.Data;
using LibrarianAPI.Models;

namespace LibrarianAPI.Tests.Controllers
{
    public class ReadersControllerTests
    {
        private readonly DbContextOptions<ApplicationDbContext> _options;

        public ReadersControllerTests()
        {
            _options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestLibraryDB")
                .Options;
        }

        [Fact]
        public async Task AddReader_ReturnsCreatedAtActionResult()//Добавление
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            var newReader = new Reader { FullName = "Alice Johnson", BirthDate = new DateTime(1985, 3, 12) };
            var result = await controller.AddReader(newReader);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var reader = Assert.IsType<Reader>(createdAtActionResult.Value);
            Assert.Equal("Alice Johnson", reader.FullName);
        }

        [Fact]
        public async Task UpdateReader_ReturnsNoContentResult()//Обновление
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            var reader = new Reader { Id = 10, FullName = "Bob Smith", BirthDate = new DateTime(1992, 7, 21) };
            context.Readers.Add(reader);
            await context.SaveChangesAsync();

            var updatedReader = new Reader { FullName = "Robert Smith", BirthDate = new DateTime(1993, 8, 15) };
            var result = await controller.UpdateReader(reader.Id, updatedReader);

            Assert.IsType<NoContentResult>(result);
            var dbReader = await context.Readers.FindAsync(reader.Id);
            Assert.Equal("Robert Smith", dbReader.FullName);
            Assert.Equal(new DateTime(1993, 8, 15), dbReader.BirthDate);
        }

        [Fact]
        public async Task DeleteReader_ReturnsNoContentResult()//Удаление
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            var reader = new Reader { Id = 12, FullName = "Clara Wilson", BirthDate = new DateTime(2000, 5, 10) };
            context.Readers.Add(reader);
            await context.SaveChangesAsync();

            var result = await controller.DeleteReader(reader.Id);

            Assert.IsType<NoContentResult>(result);
            Assert.Null(await context.Readers.FindAsync(reader.Id));
        }

        [Fact]
        public async Task GetReaderById_ReturnsReaderWithBooks()//по айди
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            var reader = new Reader { Id = 14, FullName = "Daniel Green", BirthDate = new DateTime(1988, 9, 18) };
            context.Readers.Add(reader);
            await context.SaveChangesAsync();

            var result = await controller.GetReaderById(reader.Id);
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedReader = Assert.IsType<Reader>(okResult.Value);

            Assert.Equal("Daniel Green", returnedReader.FullName);
        }

        [Fact]
        public async Task BorrowBook_AddsBookToReaderAndUpdatesCopies()//Выдача
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            var reader = new Reader { Id = 15, FullName = "Evelyn King" };
            var book = new Book { Id = 20, Title = "C# Programming Basics", Author = "Jane Doe", ISBN = "9781234567890", Year = 2022, Copies = 3 };

            context.Readers.Add(reader);
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var result = await controller.BorrowBook(reader.Id, book.Id);
            Assert.IsType<OkResult>(result);

            var updatedBook = await context.Books.FindAsync(book.Id);
            Assert.Equal(2, updatedBook.Copies);
            Assert.Contains(book, context.Readers.Include(r => r.BorrowedBooks).FirstOrDefault(r => r.Id == reader.Id)?.BorrowedBooks);
        }

        [Fact]
        public async Task ReturnBook_RemovesBookFromReaderAndIncreasesCopies()//Возврат
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            var reader = new Reader { Id = 16, FullName = "Frank Wright" };
            var book = new Book { Id = 21, Title = "Advanced Python", Author = "John Smith", ISBN = "9780987654321", Year = 2021, Copies = 0 };

            reader.BorrowedBooks.Add(book);
            context.Readers.Add(reader);
            context.Books.Add(book);
            await context.SaveChangesAsync();

            var result = await controller.ReturnBook(reader.Id, book.Id);
            Assert.IsType<OkResult>(result);

            var updatedBook = await context.Books.FindAsync(book.Id);
            Assert.Equal(1, updatedBook.Copies);
            Assert.DoesNotContain(book, context.Readers.Include(r => r.BorrowedBooks).FirstOrDefault(r => r.Id == reader.Id)?.BorrowedBooks);
        }

        [Fact]
        public async Task SearchReaders_ReturnsMatchingReaders()//Поиск
        {
            using var context = new ApplicationDbContext(_options);
            var controller = new ReadersController(context);

            context.Readers.AddRange(
                new Reader { FullName = "George Brown", BirthDate = new DateTime(1995, 2, 14) },
                new Reader { FullName = "Grace Brown", BirthDate = new DateTime(1998, 6, 30) }
            );
            await context.SaveChangesAsync();

            var result = await controller.SearchReaders("Brown");
            var okResult = Assert.IsType<OkObjectResult>(result);
            var readers = Assert.IsAssignableFrom<List<Reader>>(okResult.Value);

            Assert.Equal(2, readers.Count);
            Assert.Contains(readers, r => r.FullName == "George Brown");
            Assert.Contains(readers, r => r.FullName == "Grace Brown");
        }
    }
}
