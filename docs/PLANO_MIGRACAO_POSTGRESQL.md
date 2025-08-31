# 🔄 Plano de Migração para PostgreSQL

## Objetivo
Migrar do SQLite/SQL Server atual para PostgreSQL conforme arquitetura proposta.

## Passos da Migração

### 1. Atualizar Dependências
```xml
<!-- Remover do Infrastructure.csproj -->
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />

<!-- Adicionar -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

### 2. Atualizar Connection String
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=SistemaPDV;Username=postgres;Password=senha"
  }
}
```

### 3. Atualizar DbContext
```csharp
// No Program.cs
builder.Services.AddDbContext<PDVDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### 4. Ajustar Configurações PostgreSQL
- Remover `GETDATE()` → usar `NOW()`
- Ajustar tipos de dados específicos
- Revisar índices e constraints

### 5. Google Cloud SQL Setup
- Criar instância PostgreSQL no GCP
- Configurar firewall e conexões
- Atualizar connection strings para produção
