# ClientApp - Cliente de Consola

Aplicación de consola para consumir la API REST de productos que publica el BusinessTier. Permite consultar, crear, actualizar y eliminar productos y, al hacerlo, dispara los eventos procesados por KafkaConsumer y MailDev.

## Requisitos

- .NET SDK 9.0
- BusinessTier disponible (por defecto en `http://localhost:8080`)

## Ejecución

```bash
cd ClientApp
dotnet run
```

Si prefieres compilar antes:

```bash
cd ClientApp
dotnet build
dotnet run --no-build
```

La URL base del API puede establecerse con la variable `CLIENTAPP_BASEURL` o pasarse como primer argumento al ejecutar `dotnet run`.

## Operaciones Disponibles

Menú principal:

- (1) Listar productos
- (2) Buscar por ID
- (3) Crear producto
- (4) Actualizar producto
- (5) Eliminar producto
- (6) Buscar por nombre
- (0) Salir

Cada opción solicita los datos necesarios y muestra la respuesta en tablas formateadas.

## Flujo de Integración

1. ClientApp llama al BusinessTier mediante HTTP.
2. BusinessTier delega en DataTier por gRPC.
3. DataTier publica eventos en Kafka.
4. KafkaConsumer lee los eventos y envía correos mediante MailDev o el SMTP configurado.

## Estructura

```
ClientApp/
├── ClientApp.csproj
├── Program.cs
└── DTOs/
    ├── ProductoDto.cs
    ├── ProductoCreateDto.cs
    └── ProductoUpdateDto.cs
```

## Solución de Problemas

| Problema | Posibles causas |
| --- | --- |
| Error de conexión | BusinessTier está detenido o la URL configurada no es correcta. |
| 404 Not Found | El recurso llamado no corresponde a `/api/productos`. |
| 500 Internal Server Error | DataTier o la base de datos no responden; revisa los logs de BusinessTier. |

## Referencias

- [BusinessTier](../BusinessTier/README.md)
- [DataTier](../DataTier/README.md)
- [KafkaConsumer](../KafkaConsumer/README.md)
- [Guía de email](../docs/email-setup-guide.md)
