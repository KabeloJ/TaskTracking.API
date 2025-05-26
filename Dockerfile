# Use .NET SDK for building the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . ./
RUN dotnet restore

# Build the application
RUN dotnet publish -c Release -o /app/publish

# Use a runtime image for deployment
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "TaskTracking.UI.dll"]