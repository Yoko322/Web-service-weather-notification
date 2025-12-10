using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Добавляем поддержку CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Используем CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseStaticFiles();
app.UseDefaultFiles();

app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run("http://localhost:7200");