using System.ComponentModel.DataAnnotations;

namespace LibrarianAPI.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Author { get; set; }

        [Required]
        public string ISBN { get; set; }  // Уникальный артикул

        public int Year { get; set; }
        public int Copies { get; set; } // Количество экземпляров

        public bool IsAvailable => Copies > 0;  // Доступность книги
    }
}