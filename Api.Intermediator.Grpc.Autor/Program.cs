var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders(); // Limpia los proveedores de logging existentes
    logging.AddConsole();     // Añade el proveedor de logging para consola
    logging.AddDebug();       // Añade el proveedor de logging para debugging
});


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAnyOrigin", builder =>
    {
        builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseRouting();

// Middleware de CORS
app.UseCors("AllowAnyOrigin");

// Middleware para manejar solicitudes HTTP
app.UseEndpoints(endpoints =>
{
    endpoints.MapGet("/", async context =>
    {
        await context.Response.WriteAsync("Hello World!");
    });
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();