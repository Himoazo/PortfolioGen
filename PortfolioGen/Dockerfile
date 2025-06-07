# Use the official ASP.NET 9.0 SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file first for better caching
COPY ["PortfolioGen/PortfolioGen.csproj", "PortfolioGen/"]

# Restore dependencies
RUN dotnet restore "PortfolioGen/PortfolioGen.csproj"

# Copy everything else
COPY . .

# Build the application
WORKDIR "/src/PortfolioGen"
RUN dotnet build "PortfolioGen.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "PortfolioGen.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Build the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Use PORT environment variable for Railway compatibility
ENV ASPNETCORE_URLS=http://+:${PORT:-8080}

# Set non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Start the app
ENTRYPOINT ["dotnet", "PortfolioGen.dll"]