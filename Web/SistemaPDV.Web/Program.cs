using SistemaPDV.Web.Components;
using SistemaPDV.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// HTTP Client para API
builder.Services.AddHttpClient<ClienteApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/"); // URL da API (porta corrigida)
});

// HTTP Client para pedidos
builder.Services.AddHttpClient("Default", client =>
{
    client.BaseAddress = new Uri("http://localhost:5000/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

// Comentando HTTPS redirect para evitar problemas
// app.UseHttpsRedirection();

// Configurar arquivos estáticos
app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();