# Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

COPY src/EmailService.Worker/EmailService.Worker.csproj ./src/EmailService.Worker/

RUN dotnet restore src/EmailService.Worker/EmailService.Worker.csproj

COPY . .

WORKDIR /app/src/EmailService.Worker
RUN dotnet publish -c Release -o /app/release

# Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/release .
ENTRYPOINT ["dotnet", "EmailService.Worker.dll"]