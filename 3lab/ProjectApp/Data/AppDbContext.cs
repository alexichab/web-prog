using Microsoft.EntityFrameworkCore;
using ProjectApp.Models;


namespace ProjectApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Tasks> Tasks { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

    // Настройка отношений между Task и Employee (Автор и Исполнитель)
    modelBuilder.Entity<Tasks>()
        .HasOne(t => t.Assignee)  // Задача имеет одного исполнителя
        .WithMany(e => e.AssignedTasks)  // Сотрудник может быть исполнителем множества задач
        .HasForeignKey(t => t.AssigneeId)  // Внешний ключ в задаче для исполнителя
        .OnDelete(DeleteBehavior.Restrict);  // Запрещаем каскадное удаление

    modelBuilder.Entity<Tasks>()
        .HasOne(t => t.Author)  // Задача имеет одного автора
        .WithMany(e => e.AuthoredTasks)  // Сотрудник может быть автором множества задач
        .HasForeignKey(t => t.AuthorId)  // Внешний ключ в задаче для автора
        .OnDelete(DeleteBehavior.Restrict);  // Запрещаем каскадное удаление

    // Настройка отношения один-ко-многим между проектом и менеджером (руководителем проекта)
    modelBuilder.Entity<Project>()
        .HasOne(p => p.ProjectManager)  // Проект имеет одного менеджера
        .WithMany(e => e.ManagedProjects)  // Один менеджер может управлять несколькими проектами
        .HasForeignKey(p => p.ProjectManagerId)  // Внешний ключ в проекте
        .OnDelete(DeleteBehavior.Restrict);  // Запрещаем каскадное удаление

    // Настройка отношения многие-ко-многим между проектами и сотрудниками
    modelBuilder.Entity<Project>()
        .HasMany(p => p.Employees)  // Проект может иметь много сотрудников
        .WithMany(e => e.Projects)  // Сотрудник может работать над несколькими проектами
        .UsingEntity<Dictionary<string, object>>(
            "EmployeeProject",  // Явное имя промежуточной таблицы
            j => j.HasOne<Employee>().WithMany().HasForeignKey("EmployeeId"),  // Внешний ключ для сотрудника
            j => j.HasOne<Project>().WithMany().HasForeignKey("ProjectId"),  // Внешний ключ для проекта
            j => j.ToTable("EmployeeProject")  // Имя промежуточной таблицы
         );
        }

    }
}
