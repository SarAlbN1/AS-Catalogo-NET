# ===== build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPIA SOLO EL .csproj (para usar cache de restore)
COPY AS-Catalogo-NET/AS-Catalogo-NET.csproj AS-Catalogo-NET/
RUN dotnet restore AS-Catalogo-NET/AS-Catalogo-NET.csproj

# COPIA TODO Y PUBLICA
COPY . .
WORKDIR /src/AS-Catalogo-NET
RUN dotnet publish AS-Catalogo-NET.csproj -c Release -o /app/publish

# ===== runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "AS-Catalogo-NET.dll"]
