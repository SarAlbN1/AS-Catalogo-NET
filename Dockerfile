# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo del proyecto y restaurar dependencias
COPY ["AS-Catalogo-NET/AS-Catalogo-NET.csproj", "AS-Catalogo-NET/"]
RUN dotnet restore "AS-Catalogo-NET/AS-Catalogo-NET.csproj"

# Copiar el resto del código y compilar
COPY . .
WORKDIR "/src/AS-Catalogo-NET"
RUN dotnet build "AS-Catalogo-NET.csproj" -c Release -o /app/build

# Etapa de publicación
FROM build AS publish
RUN dotnet publish "AS-Catalogo-NET.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
EXPOSE 8080

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "AS-Catalogo-NET.dll"]
