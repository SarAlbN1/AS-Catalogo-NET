# ===== build =====
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# COPIA SOLO EL .csproj (para usar cache de restore)
COPY BusinessTier/BusinessTier.csproj BusinessTier/
RUN dotnet restore BusinessTier/BusinessTier.csproj

# COPIA TODO Y PUBLICA
COPY . .
WORKDIR /src/BusinessTier
RUN dotnet publish BusinessTier.csproj -c Release -o /app/publish

# ===== runtime =====
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "BusinessTier.dll"]
