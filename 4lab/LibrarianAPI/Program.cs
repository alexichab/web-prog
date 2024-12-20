using Microsoft.EntityFrameworkCore;
using LibrarianAPI.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();

// connect db
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// document API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger configuration
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library API");
        c.RoutePrefix = string.Empty;  // Swagger UI page
    });
}

app.UseAuthorization();
app.MapControllers();

app.Run();