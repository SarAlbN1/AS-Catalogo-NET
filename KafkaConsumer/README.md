# KafkaConsumer

Servicio worker (BackgroundService) encargado de escuchar eventos de productos en Kafka y enviar notificaciones por correo.

## Requisitos

- .NET SDK 9.0
- Kafka en ejecución (por defecto `kafka:29092` dentro de docker-compose)
- SMTP disponible (por defecto MailDev en `maildev:1025` para entornos locales)

## Ejecución

```bash
cd KafkaConsumer
dotnet run
```

En producción se ejecuta dentro del contenedor definido en `docker-compose.yml`.

## Configuración

La configuración principal está en `appsettings.json` y `appsettings.Development.json`. Las claves más relevantes son:

- `Kafka:BootstrapServers`: servidor(es) de Kafka.
- `Kafka:Topic`: tópico que se va a consumir (por defecto `product-events`).
- `Email:Host`, `Email:Port`, `Email:User`, `Email:Password`: parámetros SMTP.
- `Email:UseSsl`: habilita o deshabilita SSL según el proveedor.

Todas las variables anteriores pueden sobrescribirse mediante variables de entorno; el contenedor lo hace para apuntar a MailDev en desarrollo.

## Flujo de Trabajo

1. Lee mensajes del tópico `product-events` usando `Confluent.Kafka`.
2. Deserializa el payload a `ProductEvent`.
3. Construye el contenido del correo con la información del evento.
4. Envía el correo mediante `MailKit`.

## Estructura del Proyecto

```
KafkaConsumer/
├── KafkaConsumer.csproj
├── Program.cs
├── Worker.cs
├── Models/
│   └── ProductEvent.cs
├── Services/
│   ├── EmailService.cs
│   └── KafkaConsumerService.cs
└── appsettings*.json
```

## Solución de Problemas

| Problema | Posibles causas |
| --- | --- |
| No se conecta a Kafka | Verifica `Kafka:BootstrapServers` y que el broker esté accesible. |
| No llegan correos | Revisa las credenciales/host SMTP y los logs del servicio. |
| Excepciones de deserialización | El formato del evento no coincide con el contrato esperado (`ProductEvent`). |

## Referencias

- [Confluent.Kafka](https://github.com/confluentinc/confluent-kafka-dotnet)
- [MailKit](https://github.com/jstedfast/MailKit)
- [docker-compose.yml](../docker-compose.yml) para variables de entorno y puertos exposados.
