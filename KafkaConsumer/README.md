# 📧 KafkaConsumer - Worker Service con Email Notifications

Worker Service que consume eventos de Kafka y envía notificaciones por email cuando ocurren cambios en el catálogo de productos.

## 🎯 Características

- ✅ Consume eventos del topic `product-events` de Kafka
- 📧 Envía emails HTML con MailKit/MimeKit
- 🔄 Procesa tres tipos de eventos:
  - `ProductCreated` - Producto creado
  - `ProductUpdated` - Producto actualizado
  - `ProductDeleted` - Producto eliminado
- 📝 Logs estructurados y detallados
- 🔁 Manejo de errores y reintentos automáticos
- 🐳 Soporte para Docker

## 📋 Requisitos

- .NET 9.0 SDK
- Kafka corriendo en kafka:29092 (Docker) o localhost:9092 (Local)
- Gmail App Password configurado (ver [email-setup-guide.md](../docs/email-setup-guide.md))

## 📦 Paquetes NuGet

```xml
<PackageReference Include="Confluent.Kafka" Version="2.12.0" />
<PackageReference Include="MailKit" Version="4.14.1" />
<PackageReference Include="MimeKit" Version="4.14.0" />
<PackageReference Include="Microsoft.Extensions.Hosting" Version="9.0.10" />
```

## 🏗️ Estructura del Proyecto

```
KafkaConsumer/
├── KafkaConsumer.csproj
├── Program.cs                  # Configuración del host
├── Worker.cs                   # Background Service principal
├── appsettings.json
├── appsettings.Development.json
├── Dockerfile
├── Models/
│   └── ProductEvent.cs         # Modelo del evento
└── Services/
    └── EmailService.cs         # Servicio de envío de emails
```

## ⚙️ Configuración

### appsettings.json

```json
{
  "Kafka": {
    "BootstrapServers": "kafka:29092",
    "GroupId": "product-consumer-group",
    "Topic": "product-events"
  },
  "Email": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": "587",
    "Username": "tu-email@gmail.com",
    "Password": "xxxx-xxxx-xxxx-xxxx",
    "FromAddress": "tu-email@gmail.com",
    "ToAddress": "destinatario@gmail.com"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

### Variables de Entorno (Docker)

```bash
Kafka__BootstrapServers=kafka:29092
Kafka__GroupId=product-consumer-group
Kafka__Topic=product-events
Email__SmtpServer=smtp.gmail.com
Email__SmtpPort=587
Email__Username=tu-email@gmail.com
Email__Password=xxxx-xxxx-xxxx-xxxx
Email__FromAddress=tu-email@gmail.com
Email__ToAddress=destinatario@gmail.com
```

## 🚀 Ejecutar

### Desarrollo Local

```bash
cd KafkaConsumer
dotnet run
```

### Docker Compose

```bash
# Desde la raíz del proyecto
docker-compose up kafkaconsumer
```

## 📨 Formato de Eventos

### ProductEvent

```json
{
  "EventType": "ProductCreated",
  "ProductId": 1,
  "ProductName": "Laptop Dell",
  "Price": 999.99,
  "Timestamp": "2025-11-09T10:30:00Z"
}
```

### Tipos de Eventos

| EventType        | Descripción           | Color Email |
|------------------|-----------------------|-------------|
| ProductCreated   | Producto creado       | Verde       |
| ProductUpdated   | Producto actualizado  | Azul        |
| ProductDeleted   | Producto eliminado    | Rojo        |

## 📧 Templates de Email

### Producto Creado

```
Asunto: ✅ Nuevo Producto Creado: Laptop Dell

🎉 Nuevo Producto en el Catálogo

ID: 1
Nombre: Laptop Dell
Precio: $999.99
Fecha: 09/11/2025 10:30:00
```

### Producto Actualizado

```
Asunto: 🔄 Producto Actualizado: Laptop Dell

🔄 Producto Actualizado en el Catálogo

ID: 1
Nombre: Laptop Dell
Precio: $899.99
Fecha: 09/11/2025 11:00:00
```

### Producto Eliminado

```
Asunto: 🗑️ Producto Eliminado: Laptop Dell

🗑️ Producto Eliminado del Catálogo

ID: 1
Nombre: Laptop Dell
Precio: $899.99
Fecha: 09/11/2025 11:30:00
```

## 📊 Flujo de Procesamiento

```
┌─────────────┐
│  DataTier   │
└──────┬──────┘
       │ Produce event
       ▼
┌─────────────────┐
│  Kafka Topic    │ product-events
└──────┬──────────┘
       │ Subscribe
       ▼
┌─────────────────┐
│ KafkaConsumer   │
└──────┬──────────┘
       │ Process event
       ▼
┌─────────────────┐
│ EmailService    │
└──────┬──────────┘
       │ SMTP
       ▼
┌─────────────────┐
│  Gmail / Email  │
└─────────────────┘
```

## 🔍 Logs del Sistema

### Logs Exitosos

```
🚀 Kafka Consumer Worker iniciado...
📡 Conectado a: kafka:29092
📬 Suscrito al topic: product-events
👥 Group ID: product-consumer-group
📨 Mensaje recibido - Key: product-1
📄 Contenido: {"EventType":"ProductCreated",...}
⚙️ Procesando evento: ProductCreated para producto: Laptop Dell
✉️ Email de creación enviado para: Laptop Dell
✅ Email enviado exitosamente para producto creado: Laptop Dell
✅ Mensaje procesado y commiteado
```

### Logs de Error

```
❌ Error consumiendo mensaje: Connection refused
❌ Error general: Unable to send email
❌ Error enviando email: Authentication failed
```

## 🛠️ Troubleshooting

### Consumer no recibe mensajes

**Problema**: No se procesan eventos de Kafka

**Solución**:
1. Verifica que Kafka esté corriendo: `docker-compose ps kafka`
2. Verifica el topic existe: `docker exec -it catalogo_kafka kafka-topics --list --bootstrap-server localhost:9092`
3. Revisa la configuración de `BootstrapServers`
4. Verifica los logs del consumer

### Emails no se envían

**Problema**: Los eventos se procesan pero no llegan emails

**Solución**:
1. Verifica las credenciales de Gmail
2. Asegúrate de usar App Password, no contraseña normal
3. Verifica que el puerto 587 no esté bloqueado
4. Revisa la carpeta de SPAM
5. Consulta [email-setup-guide.md](../docs/email-setup-guide.md)

### Error de autenticación SMTP

**Problema**: `Authentication failed`

**Solución**:
1. Regenera el App Password en Gmail
2. Verifica que no haya espacios en la contraseña
3. Asegúrate de tener verificación en dos pasos activada

### Consumer se cae constantemente

**Problema**: El worker se detiene o reinicia

**Solución**:
1. Revisa los logs para ver el error específico
2. Verifica la conexión a Kafka
3. Asegúrate de que el email service no lance excepciones no manejadas
4. Aumenta el delay de reintento en caso de errores

## 🔒 Seguridad

### Mejores Prácticas

1. **No subir credenciales a Git**
   - Usa variables de entorno
   - Archivo `.env` en `.gitignore`

2. **App Passwords de Gmail**
   - Usa App Password específico
   - Rota las credenciales periódicamente

3. **Consumer Groups**
   - Usa group IDs únicos por ambiente
   - `product-consumer-group` (producción)
   - `product-consumer-group-dev` (desarrollo)

## 📈 Monitoreo

### Health Checks

El worker registra su estado en los logs:

```bash
# Ver logs en tiempo real
docker-compose logs -f kafkaconsumer

# Ver últimas 100 líneas
docker-compose logs --tail=100 kafkaconsumer
```

### Métricas Importantes

- ✅ Mensajes procesados correctamente
- ❌ Mensajes con error
- ⏱️ Tiempo de procesamiento
- 📧 Emails enviados exitosamente
- 🔁 Reintentos por errores

## 🐳 Docker

### Build

```bash
docker build -f KafkaConsumer/Dockerfile -t kafkaconsumer:latest .
```

### Run

```bash
docker run -d \
  --name kafkaconsumer \
  --network catalogo_network \
  -e Kafka__BootstrapServers=kafka:29092 \
  -e Kafka__Topic=product-events \
  -e Email__Username=tu-email@gmail.com \
  -e Email__Password=xxxx-xxxx-xxxx-xxxx \
  kafkaconsumer:latest
```

## 📚 Documentación Relacionada

- [Email Setup Guide](../docs/email-setup-guide.md) - Configuración de Gmail
- [DataTier](../DataTier/README.md) - Productor de eventos
- [BusinessTier](../BusinessTier/README.md) - API REST
- [ClientApp](../ClientApp/README.md) - Cliente de consola

## 🧪 Testing

### Prueba Manual

1. **Iniciar el sistema completo**:
   ```bash
   docker-compose up
   ```

2. **Crear un producto usando ClientApp**:
   ```bash
   cd ClientApp
   dotnet run
   # Seleccionar opción 3 - Crear producto
   ```

3. **Verificar logs del consumer**:
   ```bash
   docker-compose logs kafkaconsumer
   ```

4. **Verificar email recibido**:
   - Revisa la bandeja de entrada del email configurado
   - También revisa SPAM por si acaso

## ✅ Criterios de Éxito

- [x] Consumer recibe mensajes de Kafka
- [x] Emails se envían correctamente con templates HTML
- [x] Logs muestran procesamiento de eventos
- [x] Manejo de errores implementado con reintentos
- [x] Soporte para los tres tipos de eventos
- [x] Configuración via environment variables
- [x] Dockerfile optimizado
- [x] Documentación completa

---

**Desarrollado como parte del proyecto AS-Catalogo-NET**  
**Tarea 4: Kafka Consumer + Email Service (Bonus)**
