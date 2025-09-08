using SistemaPDV.Web.Components;
using SistemaPDV.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddHubOptions(options =>
    {
        options.MaximumReceiveMessageSize = 10 * 1024 * 1024; // 10MB
    });

// Registrar o handler de autenticação
builder.Services.AddScoped<AuthenticationHandler>();

// HTTP Client para API com autenticação automática
builder.Services.AddHttpClient<ClienteApiService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/"); // URL da API padrão
})
.AddHttpMessageHandler<AuthenticationHandler>();

// HTTP Client para autenticação (caso necessário em páginas de login/registro)
builder.Services.AddHttpClient<IAuthWebService, AuthWebService>(client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
});

// Serviço de contexto do usuário
builder.Services.AddScoped<IUserContextService, UserContextService>();

// HTTP Client para pedidos com autenticação automática
builder.Services.AddHttpClient("Default", client =>
{
    client.BaseAddress = new Uri("http://localhost:5001/");
})
.AddHttpMessageHandler<AuthenticationHandler>();

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
    .AddInteractiveServerRenderMode()
    .AllowAnonymous();

app.Run();