using Microsoft.EntityFrameworkCore;
using ProjectApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавление поддержки контроллеров и представлений (MVC)
builder.Services.AddControllersWithViews();

// Регистрация AppDbContext
builder.Services.AddDbContext<AppDbContext>(options =>
     options.UseMySql(
         builder.Configuration.GetConnectionString("DefaultConnection"),
         new MySqlServerVersion(new Version(8, 0, 40)) // Укажите правильную версию вашего MySQL сервера
     ));
var app = builder.Build();

// Обработчик завершения работы
app.Lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Приложение завершает работу...");
});

// Конфигурация HTTP-пайплайна
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// Определение маршрутов для контроллеров
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
