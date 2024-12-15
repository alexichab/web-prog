using System.ComponentModel.DataAnnotations;

namespace PersonalDiary.Models
{
    public class DiaryPost
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; }

        [Required]
        public string Mood { get; set; }

        [Required]
        [StringLength(50)]
        public string Category { get; set; }

    }
}
