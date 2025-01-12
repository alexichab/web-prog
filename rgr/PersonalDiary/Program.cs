using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using PersonalDiary.Data;
using PersonalDiary.Models;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.Configure<AdminSettings>(builder.Configuration.GetSection("Admin"));
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
     options.UseMySql(
         connectionString,
         new MySqlServerVersion(new Version(8, 0, 40)) 
     ));

builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 1;
}).AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();

app.Run();
