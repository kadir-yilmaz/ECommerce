# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project files for caching restore layer
COPY ECommerce.WebAPI/ECommerce.WebAPI.csproj ECommerce.WebAPI/
COPY ECommerce.Application/ECommerce.Application.csproj ECommerce.Application/
COPY ECommerce.Domain/ECommerce.Domain.csproj ECommerce.Domain/
COPY ECommerce.Infrastructure/ECommerce.Infrastructure.csproj ECommerce.Infrastructure/
COPY ECommerce.Persistence/ECommerce.Persistence.csproj ECommerce.Persistence/
COPY ECommerce.SignalR/ECommerce.SignalR.csproj ECommerce.SignalR/

# Restore dependencies
RUN dotnet restore ECommerce.WebAPI/ECommerce.WebAPI.csproj

# Copy all files
COPY . .

# Build and publish WebAPI
WORKDIR /src/ECommerce.WebAPI
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 8080
ENTRYPOINT ["dotnet", "ECommerce.WebAPI.dll"]
