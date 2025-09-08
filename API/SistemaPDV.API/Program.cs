using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SistemaPDV.Core.Interfaces;
using SistemaPDV.Infrastructure.Data;
using SistemaPDV.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sistema PDV API",
        Version = "v1",
        Description = "API para Sistema de Ponto de Venda - Cozinha Conectada"
    });
    
    // Configuração JWT para Swagger
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header usando Bearer scheme. Exemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    
    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Entity Framework
builder.Services.AddDbContext<PDVDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ChaveSecretaMuitoSeguraParaJWT123456789";
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// Services
builder.Services.AddScoped<IPedidoService, PedidoService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IClienteService, ClienteService>();
builder.Services.AddScoped<IRestauranteService, RestauranteService>();
builder.Services.AddScoped<IImpressaoService, ImpressaoService>();
builder.Services.AddScoped<IRelatorioService, RelatorioService>();

// 🚀 Novos serviços de autenticação modernos
builder.Services.AddScoped<IAuthService, SistemaPDV.Infrastructure.Services.AuthService>();
builder.Services.AddScoped<IPasswordService, PasswordService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// HttpClient para PasswordService (verificação de senhas comprometidas)
builder.Services.AddHttpClient<IPasswordService, PasswordService>();

builder.Services.AddScoped<SistemaPDV.Core.Interfaces.ICurrentUserService, SistemaPDV.API.Services.CurrentUserService>();

// HttpContextAccessor para acessar o contexto HTTP
builder.Services.AddHttpContextAccessor();

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
// Habilitando Swagger em todos os ambientes
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sistema PDV API v1");
    c.RoutePrefix = "swagger";
});

// Comentando o redirecionamento HTTPS para teste
// app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Ensure database is created and migrations are applied
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<PDVDbContext>();
    
    // Aplicar migrações pendentes
    context.Database.Migrate();
    
    // Criar um restaurante padrão se não existir
    if (!context.Restaurantes.Any())
    {
        var restaurantePadrao = new SistemaPDV.Core.Entities.Restaurante
        {
            Nome = "Restaurante Padrão",
            IsActive = true,
            CriadoEm = DateTime.UtcNow
        };
        context.Restaurantes.Add(restaurantePadrao);
        context.SaveChanges();
        
        // Criar usuário admin padrão se não existir
        if (!context.ApplicationUsers.Any())
        {
            var passwordService = scope.ServiceProvider.GetRequiredService<IPasswordService>();
            var passwordHash = passwordService.HashPassword("123456");
            
            var adminUser = new SistemaPDV.Core.Entities.ApplicationUser
            {
                Email = "admin@sistema.com",
                PasswordHash = passwordHash,
                Nome = "Administrador",
                RestauranteId = restaurantePadrao.Id,
                IsActive = true,
                IsAdmin = true,
                CriadoEm = DateTime.UtcNow
            };
            
            context.ApplicationUsers.Add(adminUser);
            context.SaveChanges();
            
            Console.WriteLine("✅ Usuário padrão criado: admin@sistema.com / 123456");
        }
    }
}

app.Run();