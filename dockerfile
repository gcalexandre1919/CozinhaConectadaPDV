# Dockerfile para Oracle Cloud Always Free
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar arquivos de projeto
COPY ["API/SistemaPDV.API/SistemaPDV.API.csproj", "API/SistemaPDV.API/"]
COPY ["Web/SistemaPDV.Web/SistemaPDV.Web.csproj", "Web/SistemaPDV.Web/"]
COPY ["Core/SistemaPDV.Core/SistemaPDV.Core.csproj", "Core/SistemaPDV.Core/"]
COPY ["Infrastructure/SistemaPDV.Infrastructure/SistemaPDV.Infrastructure.csproj", "Infrastructure/SistemaPDV.Infrastructure/"]

# Restaurar dependências
RUN dotnet restore "Web/SistemaPDV.Web/SistemaPDV.Web.csproj"

# Copiar código fonte
COPY . .

# Build da aplicação
WORKDIR "/src/Web/SistemaPDV.Web"
RUN dotnet build "SistemaPDV.Web.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "SistemaPDV.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Configurar para rodar na porta 8080 (Oracle Cloud)
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "SistemaPDV.Web.dll"]
