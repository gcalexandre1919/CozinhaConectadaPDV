# 🔐 Implementação de Autenticação Multi-Tenant

## Objetivo
Implementar sistema de autenticação conforme arquitetura proposta: ASP.NET Core Identity + JWT para multi-tenant por restaurante.

## Entidades de Autenticação

### 1. IdentityUser Customizado
```csharp
public class ApplicationUser : IdentityUser
{
    public string NomeCompleto { get; set; } = string.Empty;
    public int RestauranteId { get; set; }
    public virtual Restaurante? Restaurante { get; set; }
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public bool Ativo { get; set; } = true;
}
```

### 2. Atualizar Entidade Restaurante
```csharp
public class Restaurante
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? CNPJ { get; set; }
    public string? Email { get; set; }
    public string? Telefone { get; set; }
    public string? Logo { get; set; } // URL da logo
    public string? UrlPersonalizada { get; set; } // URL customizada
    public DateTime DataCriacao { get; set; } = DateTime.Now;
    public bool Ativo { get; set; } = true;
    
    // Navigation properties
    public virtual ICollection<ApplicationUser> Usuarios { get; set; } = new List<ApplicationUser>();
    public virtual ICollection<Cliente> Clientes { get; set; } = new List<Cliente>();
    public virtual ICollection<Produto> Produtos { get; set; } = new List<Produto>();
    public virtual ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
```

## Configuração do Identity

### 1. NuGet Packages Necessários
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.0.0" />
<PackageReference Include="System.IdentityModel.Tokens.Jwt" Version="7.0.3" />
```

### 2. Atualizar DbContext
```csharp
public class PDVDbContext : IdentityDbContext<ApplicationUser>
{
    // ... configurações existentes
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configurar relacionamento User -> Restaurante
        modelBuilder.Entity<ApplicationUser>()
            .HasOne(u => u.Restaurante)
            .WithMany(r => r.Usuarios)
            .HasForeignKey(u => u.RestauranteId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 3. Configurar no Program.cs
```csharp
// Configurar Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<PDVDbContext>()
.AddDefaultTokenProviders();

// Configurar JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});
```

## DTOs de Autenticação

### 1. DTOs de Login/Registro
```csharp
public class LoginDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegisterDto
{
    [Required]
    public string NomeCompleto { get; set; } = string.Empty;
    
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;
    
    [Required]
    public string Password { get; set; } = string.Empty;
    
    [Required]
    public string NomeRestaurante { get; set; } = string.Empty;
    
    public string? CNPJ { get; set; }
    public string? TelefoneRestaurante { get; set; }
}

public class AuthResponseDto
{
    public bool Success { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public string NomeUsuario { get; set; } = string.Empty;
    public string NomeRestaurante { get; set; } = string.Empty;
    public string[] Errors { get; set; } = Array.Empty<string>();
}
```

## Middleware Multi-Tenant

### 1. Middleware para Identificar Restaurante
```csharp
public class TenantMiddleware
{
    private readonly RequestDelegate _next;

    public TenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Extrair RestauranteId do token JWT
        var restauranteId = context.User.Claims
            .FirstOrDefault(c => c.Type == "RestauranteId")?.Value;
            
        if (!string.IsNullOrEmpty(restauranteId))
        {
            context.Items["RestauranteId"] = int.Parse(restauranteId);
        }

        await _next(context);
    }
}
```

## Próximos Passos

1. **Implementar AuthController** para login/registro
2. **Criar AuthService** para geração de tokens
3. **Implementar filtros globais** para multi-tenancy
4. **Atualizar todos os services** para filtrar por RestauranteId
5. **Implementar páginas Blazor** de login/registro
6. **Configurar upload de logo** do restaurante
7. **Implementar URLs personalizadas** por restaurante
