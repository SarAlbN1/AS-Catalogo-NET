# AS-Catalogo-NET

Plataforma de catálogo de productos construida con .NET 8/9, MySQL y Kafka. El sistema incluye una API REST (BusinessTier), un servicio gRPC (DataTier), un worker de eventos (KafkaConsumer) y un cliente de consola (ClientApp). Los servicios se orquestan mediante Docker Compose y se integran con MailDev para observar las notificaciones por correo.

## Componentes

| Servicio | Descripción | Framework | Puerto |
| --- | --- | --- | --- |
| **BusinessTier** | API REST que expone CRUD de productos y coordina con DataTier vía gRPC o EF Core. | ASP.NET Core 8 | 8080 |
| **DataTier** | Servicio gRPC que opera la base MySQL y publica eventos en Kafka. | ASP.NET Core gRPC 9 | 5001 |
| **KafkaConsumer** | Worker que consume `product-events` y envía correos SMTP. | .NET Worker 9 | N/A |
| **ClientApp** | Aplicación de consola para operar el catálogo manualmente. | .NET Console 9 | N/A |
| **MailDev** | Servidor SMTP y visor web para correos de prueba. | Node.js | 1025 / 1080 |
| **Kafka / Zookeeper** | Broker y coordinador para mensajería. | Confluent 7.4 | 9092 / 2181 |
| **MySQL** | Persistencia de catálogo. | MySQL 8.0 | 3306 |

## Funcionalidad Principal

- CRUD completo de productos (`/api/productos`) con DTOs de entrada/salida.
- Selección dinámica entre acceso directo por EF Core o gRPC según la bandera `PreferGrpc`.
- Emisión de eventos `ProductCreated`, `ProductUpdated`, `ProductDeleted` al tópico `product-events` de Kafka.
- KafkaConsumer procesa los eventos y envía correos con MailKit (MailDev por defecto).
- ClientApp ejecuta todas las operaciones del catálogo desde la consola y se integra con la API.
- Docker Compose inicializa MySQL, Kafka, MailDev y los servicios .NET dejándolos listos para pruebas end-to-end.

## Requisitos

- Docker Desktop con soporte para Linux containers.
- PowerShell 5.1 o superior para los comandos sugeridos (en Windows).
- .NET SDK 9.0 si deseas ejecutar o depurar los proyectos fuera de contenedores.
- Puertos libres: 8080, 5001, 3306, 9092, 2181, 1025, 1080.

## Inicio Rápido con Docker Compose

```powershell
git clone https://github.com/SarAlbN1/AS-Catalogo-NET.git
cd AS-Catalogo-NET
docker-compose up -d --build
docker-compose ps
```

Servicios expuestos:
- API REST: `http://localhost:8080/api/productos`
- MailDev (UI): `http://localhost:1080`
- MailDev (SMTP): `localhost:1025`

Para detener:

```powershell
docker-compose down          # conserva volúmenes
docker-compose down -v       # elimina volúmenes y datos
```

## Operaciones de la API

```http
GET    /api/productos
GET    /api/productos/{id}
POST   /api/productos
PUT    /api/productos/{id}
DELETE /api/productos/{id}
```

Ejemplos en PowerShell:

```powershell
Invoke-RestMethod -Uri http://localhost:8080/api/productos

$body = @{ nombre = "Mouse Vertical"; descripcion = "USB-C"; precio = 49.9; cantidadDisponible = 20; catalogoId = 1 } | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:8080/api/productos -Method Post -Body $body -ContentType 'application/json'

$update = @{ nombre = "Mouse Vertical"; descripcion = "Bluetooth"; precio = 59.9; cantidadDisponible = 25 } | ConvertTo-Json
Invoke-RestMethod -Uri http://localhost:8080/api/productos/1 -Method Put -Body $update -ContentType 'application/json'

Invoke-RestMethod -Uri http://localhost:8080/api/productos/1 -Method Delete
```

Cada operación genera un evento en Kafka; MailDev mostrará el correo correspondiente en su interfaz web.

## ClientApp

Aplicación de consola incluida en `ClientApp/Program.cs`.

```powershell
cd ClientApp
$env:CLIENTAPP_BASEURL = 'http://localhost:8080/api'

```

El menú permite listar, buscar, crear, actualizar y eliminar productos. La URL puede configurarse con la variable `CLIENTAPP_BASEURL` o como primer argumento de línea de comandos.

## KafkaConsumer

- Usa `Confluent.Kafka` para consumir el tópico `product-events`.
- Envía correos mediante `MailKit` y las credenciales configuradas (`Email:Host`, `Email:Port`, `Email:User`, etc.).
- En docker-compose apunta automáticamente a MailDev. Para SMTP externo utiliza las variables `EMAIL_HOST`, `EMAIL_PORT`, `EMAIL_USER`, `EMAIL_PASSWORD`, `EMAIL_USESSL`.
- Los logs muestran cada evento procesado y el resultado del envío.

## DataTier

- Servicio gRPC que maneja los productos en MySQL usando `MyAppDbContext`.
- Ejecuta `EnsureCreated()` en el arranque para garantizar la existencia del esquema.
- Publica eventos a Kafka mediante `ProductEventProducer`.
- Escucha en `http://0.0.0.0:5001` (HTTP/2) cuando se ejecuta bajo docker-compose.

## BusinessTier

- Controlador `ProductosController` expone los endpoints REST.
- `ProductosService` decide si usa gRPC o EF Core según configuración.
- Incluye AutoMapper manual para DTOs y validaciones básicas.
- Registra y documenta los correos gatillados mediante logs.

## Configuración Clave

- `appsettings*.json` en cada proyecto contienen valores por defecto para desarrollo.
- Docker compose inyecta variables de entorno para conexiones y puertos.
- Scripts SQL en `init-scripts/` inicializan la base si se requiere un estado mínimo.

## Monitoreo y Logs

```powershell
docker-compose logs -f businesstier
docker-compose logs -f datatier
docker-compose logs -f kafkaconsumer
docker-compose logs -f kafka
```

MailDev muestra los correos enviados en `http://localhost:1080`. KafkaConsumer imprime el evento procesado, entidad afectada y resultado del correo.

## Desarrollo Local sin Docker

1. Levanta MySQL, Kafka y MailDev (puedes usar docker-compose limitando los servicios).
2. Ajusta las cadenas de conexión/hosts en `appsettings.Development.json` de cada proyecto o usa variables de entorno.
3. Ejecuta los proyectos desde Visual Studio o con:
   ```powershell
   dotnet run --project DataTier/DataTier.csproj
   dotnet run --project BusinessTier/BusinessTier.csproj
   dotnet run --project KafkaConsumer/KafkaConsumer.csproj
   dotnet run --project ClientApp/ClientApp.csproj
   ```

## Estructura del Repositorio

```
AS-Catalogo-NET/
├── BusinessTier/
├── DataTier/
├── KafkaConsumer/
├── ClientApp/
├── DataTier/Protos/
├── init-scripts/
├── docs/
└── docker-compose.yml
```

## Solución de Problemas

| Incidencia | Acciones recomendadas |
| --- | --- |
| API responde 500 | Revisa `docker-compose logs businesstier` y confirma que MySQL esté saludable (`docker-compose ps`). |
| KafkaConsumer no consume | Verifica que Kafka esté activo (`docker-compose logs kafka`) y que la variable `KAFKA__BOOTSTRAPSERVERS` apunte al host correcto. |
| Correos no llegan | Comprueba `docker-compose logs kafkaconsumer` y la UI de MailDev; ajusta las credenciales SMTP si usas otro servidor. |
| Puerto en uso | Modifica el mapeo de puertos en `docker-compose.yml` (ej. `8081:8080`). |

## Créditos

- Alejandro Caicedo (INDI260)
- Sara Albarracín (SarAlbN1)
- Alejandro Pinzón (alejandro09pf)
- Claude (claude)

Proyecto académico para prácticas de Arquitectura de Software.