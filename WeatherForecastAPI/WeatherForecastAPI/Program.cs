using WeatherForecastAPI.Services;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Текущая директория: " + Directory.GetCurrentDirectory());
Console.WriteLine("Файл конфигурации: " +
    Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
Console.WriteLine("Файл существует: " +
    File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")));

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowWebInterface",
        policy =>
        {
            policy.WithOrigins("http://localhost:7200")  // Порт вашего веб-интерфейса
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IWeatherService, WeatherService>();

var app = builder.Build();

// Включаем статические файлы
app.UseDefaultFiles();
app.UseStaticFiles();

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather Forecast API V1");
    c.RoutePrefix = "swagger";
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowWebInterface");
app.Run();