# Etapa de build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar el archivo de proyecto y restaurar dependencias
COPY ["KafkaConsumer/KafkaConsumer.csproj", "KafkaConsumer/"]
RUN dotnet restore "KafkaConsumer/KafkaConsumer.csproj"

# Copiar el resto del código fuente
COPY KafkaConsumer/ KafkaConsumer/

# Build de la aplicación
WORKDIR "/src/KafkaConsumer"
RUN dotnet build "KafkaConsumer.csproj" -c Release -o /app/build

# Publicar la aplicación
FROM build AS publish
RUN dotnet publish "KafkaConsumer.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Etapa final
FROM mcr.microsoft.com/dotnet/runtime:9.0 AS final
WORKDIR /app

# Instalar certificados de CA para SSL/TLS
RUN apt-get update && apt-get install -y ca-certificates && rm -rf /var/lib/apt/lists/*

# Copiar los binarios publicados
COPY --from=publish /app/publish .

# Variables de entorno por defecto (pueden sobrescribirse en docker-compose)
ENV KAFKA__BOOTSTRAPSERVERS="kafka:29092"
ENV KAFKA__GROUPID="product-consumer-group"
ENV KAFKA__TOPIC="product-events"
ENV EMAIL__SMTPSERVER="maildev"
ENV EMAIL__SMTPPORT="1025"

# Punto de entrada
ENTRYPOINT ["dotnet", "KafkaConsumer.dll"]
