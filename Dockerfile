# Base image for running the application
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the solution and project files into the container
COPY ["WebNewsAPIs/WebNewsAPIs.sln", "WebNewsAPIs/"]
COPY ["WebNewsAPIs/WebNewsAPIs.csproj", "WebNewsAPIs/"]
COPY ["AccessDatas/AccessDatas.csproj", "AccessDatas/"]
COPY ["BusinessObjects/BusinessObjects.csproj", "BusinessObjects/"]
COPY ["Repositories/Repositories.csproj", "Repositories/"]
COPY ["WebNewsClients/WebNewsClients.csproj", "WebNewsClients/"]

# Ensure all other files are copied into the container
COPY . .

# Restore dependencies for the solution (WebNewsAPIs.sln)
RUN dotnet restore "WebNewsAPIs/WebNewsAPIs.sln"

# Build the solution
RUN dotnet build "WebNewsAPIs/WebNewsAPIs.sln" -c "$BUILD_CONFIGURATION" -o "/app/build"

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebNewsAPIs/WebNewsAPIs.sln" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false || true

# Final image for API
FROM base AS final-api
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebNewsAPIs.dll"]

# Build and Publish for Web MVC (WebNewsClients)

# Build stage for Web News Client (Web MVC)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-web
WORKDIR /src

# Copy WebNewsClients files into the container
COPY ["WebNewsClients/WebNewsClients.csproj", "WebNewsClients/"]

# Ensure all other files are copied into the container
COPY . .

# Restore dependencies for WebNewsClients
RUN dotnet restore "WebNewsClients/WebNewsClients.csproj"

# Build the Web News Client project
WORKDIR "/src/WebNewsClients"
RUN dotnet build "WebNewsClients.csproj" -c "$BUILD_CONFIGURATION" -o "/app/build"

# Publish stage for Web News Client
FROM build-web AS publish-web
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WebNewsClients.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false || true

# Final image for Web MVC
FROM base AS final-web
WORKDIR /app
COPY --from=publish-web /app/publish .
ENTRYPOINT ["dotnet", "WebNewsClients.dll"]
