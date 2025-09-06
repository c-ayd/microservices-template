# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/AuthService.Api/AuthService.Api.csproj ./src/AuthService.Api/
COPY src/AuthService.Application/AuthService.Application.csproj ./src/AuthService.Application/
COPY src/AuthService.Domain/AuthService.Domain.csproj ./src/AuthService.Domain/
COPY src/AuthService.Infrastructure/AuthService.Infrastructure.csproj ./src/AuthService.Infrastructure/
COPY src/AuthService.Persistence/AuthService.Persistence.csproj ./src/AuthService.Persistence/

RUN dotnet restore src/AuthService.Api/AuthService.Api.csproj

COPY . .

WORKDIR /app/src/AuthService.Api
RUN dotnet publish -c Release -o /app/release

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/release .
ENTRYPOINT ["dotnet", "AuthService.Api.dll"]