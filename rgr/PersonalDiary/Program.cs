using Microsoft.EntityFrameworkCore;
using PersonalDiary.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseMySql(
         connectionString,
         new MySqlServerVersion(new Version(8, 0, 40)) 
     ));
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
