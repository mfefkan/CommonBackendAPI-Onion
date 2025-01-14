# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 443

# Build image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["Presentation/Presentation.csproj", "Presentation/"]
COPY ["Application/Application.csproj", "Application/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Domain/Domain.csproj", "Domain/"]

RUN dotnet restore "Presentation/Presentation.csproj"

# Copy the entire solution and build
COPY . .
WORKDIR "/src/Presentation"
RUN dotnet build "Presentation.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "Presentation.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Presentation.dll"]
