using Microsoft.EntityFrameworkCore;
using LibrarianAPI.Models;

namespace LibrarianAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Book> Books { get; set; }
        public DbSet<Reader> Readers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Reader>()
                .HasMany(r => r.BorrowedBooks)
                .WithMany(); // связь многие-ко-многим для книг и читателей
        }
    }
}