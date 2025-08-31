using Microsoft.EntityFrameworkCore;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using SistemaPDV.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Entity Framework
builder.Services.AddDbContext<PDVDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<IImpressaoService, ImpressaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Comentando o redirecionamento HTTPS para teste
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PDVDbContext>();
    context.Database.EnsureCreated();
    
    // Criar um restaurante padrão se não existir
    if (!context.Restaurantes.Any())
    {
        var restaurantePadrao = new SistemaPDV.Core.Entities.Restaurante
        {
            Nome = "Restaurante Padrão",
            Ativo = true
        };
        context.Restaurantes.Add(restaurantePadrao);
        context.SaveChanges();
    }
}

app.Run();