# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/AdminService.Api/AdminService.Api.csproj ./src/AdminService.Api/
COPY src/AdminService.Application/AdminService.Application.csproj ./src/AdminService.Application/
COPY src/AdminService.Infrastructure/AdminService.Infrastructure.csproj ./src/AdminService.Infrastructure/

RUN dotnet restore src/AdminService.Api/AdminService.Api.csproj

COPY . .

WORKDIR /app/src/AdminService.Api
RUN dotnet publish -c Release -o /app/release

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/release .
ENTRYPOINT ["dotnet", "AdminService.Api.dll"]