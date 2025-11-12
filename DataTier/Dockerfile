# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["DataTier/DataTier.csproj", "./"]
RUN dotnet restore "DataTier.csproj"

# Copy everything else and build
COPY DataTier/. ./
RUN dotnet build "DataTier.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "DataTier.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

# Expose gRPC port
EXPOSE 5001

# Copy published files
COPY --from=publish /app/publish .

# Set default environment
ENV ASPNETCORE_ENVIRONMENT=Production

# Run the application
ENTRYPOINT ["dotnet", "DataTier.dll"]
