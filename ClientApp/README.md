# 📱 ClientApp - Cliente de Consola para Catálogo de Productos

Aplicación de consola interactiva para consumir la API REST del BusinessTier.

## 🚀 Características

- ✅ Listar todos los productos
- 🔍 Buscar producto por ID
- 🔍 Buscar producto por nombre
- ➕ Crear nuevos productos
- ✏️ Actualizar productos existentes
- 🗑️ Eliminar productos
- 🎨 Interfaz colorida y amigable
- 📧 Notificaciones de eventos (vía Kafka → Email)

## 📋 Requisitos

- .NET 9.0 SDK
- BusinessTier ejecutándose en http://localhost:8080

## 🏃 Ejecutar la Aplicación

### Opción 1: Desarrollo Local

```bash
cd ClientApp
dotnet run
```

### Opción 2: Build y Ejecutar

```bash
cd ClientApp
dotnet build
dotnet run --no-build
```

## 🎯 Uso

Al iniciar la aplicación, se mostrará un menú interactivo:

```
╔════════════════════════════════════════════════╗
║              MENÚ PRINCIPAL                    ║
╚════════════════════════════════════════════════╝

📋 CONSULTAS:
  1️⃣  - Listar todos los productos
  2️⃣  - Buscar producto por ID
  6️⃣  - Buscar producto por nombre

✏️  OPERACIONES:
  3️⃣  - Crear nuevo producto
  4️⃣  - Actualizar producto
  5️⃣  - Eliminar producto

  0️⃣  - Salir
```

### Ejemplos de Uso

#### 1. Listar Productos

Muestra una tabla con todos los productos:

```
┌────┬─────────────────────────┬──────────────┬─────────┐
│ ID │ Nombre                  │ Precio       │ Stock   │
├────┼─────────────────────────┼──────────────┼─────────┤
│ 1  │ Laptop Dell             │ $999.99      │ 10      │
│ 2  │ Mouse Logitech          │ $29.99       │ 50      │
└────┴─────────────────────────┴──────────────┴─────────┘
```

#### 2. Crear Producto

La aplicación solicitará:
- Nombre del producto
- Descripción
- Precio
- Stock

Después de crear el producto:
- ✅ Se muestra confirmación
- 📧 Se envía un email automático (vía KafkaConsumer)

#### 3. Actualizar Producto

- Ingresa el ID del producto
- Muestra los valores actuales
- Presiona Enter para mantener valores o ingresa nuevos

#### 4. Eliminar Producto

- Ingresa el ID del producto
- Muestra confirmación antes de eliminar
- Requiere confirmación (S/N)

## ⚙️ Configuración

Al iniciar, la aplicación pregunta por la URL del API:

```
🔧 Configuración:
Ingrese la URL del API (Enter para usar http://localhost:8080/api):
```

Puedes ingresar una URL personalizada o presionar Enter para usar la predeterminada.

## 🔗 Integración con el Sistema

El ClientApp interactúa con todo el sistema:

```
┌─────────────┐
│  ClientApp  │
└──────┬──────┘
       │ HTTP REST
       ▼
┌─────────────────┐
│  BusinessTier   │ (Port 8080)
└──────┬──────────┘
       │ gRPC
       ▼
┌─────────────────┐
│   DataTier      │ (Port 5001)
└──────┬──────────┘
       │ Kafka
       ▼
┌─────────────────┐
│ KafkaConsumer   │
└──────┬──────────┘
       │ SMTP
       ▼
┌─────────────────┐
│  Gmail / Email  │
└─────────────────┘
```

## 📊 Flujo de Eventos

1. **Usuario crea producto en ClientApp**
   - ClientApp → POST /api/productos → BusinessTier
   - BusinessTier → gRPC CreateProducto → DataTier
   - DataTier → Kafka Topic "product-events"
   - KafkaConsumer → Lee evento → Envía email

2. **Notificación por Email**
   - Email con template HTML
   - Detalles del producto creado
   - Timestamp del evento

## 🎨 Características de la UI

- ✅ Colores para mejorar la experiencia
- 📋 Tablas formateadas
- ⚠️ Mensajes de error claros
- 🎉 Confirmaciones visuales
- 🔄 Navegación intuitiva

## 🛠️ Tecnologías Utilizadas

- **HttpClient**: Para requests HTTP
- **System.Net.Http.Json**: Serialización JSON
- **Console API**: Interfaz de usuario

## 📝 Estructura del Proyecto

```
ClientApp/
├── ClientApp.csproj
├── Program.cs           # Aplicación principal
├── README.md
└── DTOs/
    ├── ProductoDto.cs
    ├── ProductoCreateDto.cs
    └── ProductoUpdateDto.cs
```

## 🐛 Troubleshooting

### Error: "No se puede conectar al API"

**Solución**:
1. Verifica que BusinessTier esté ejecutándose
2. Verifica la URL (http://localhost:8080)
3. Revisa el firewall

### Error: "404 Not Found"

**Solución**:
1. Verifica que el endpoint sea `/api/productos`
2. Asegúrate de que BusinessTier tenga el controlador

### Error: "500 Internal Server Error"

**Solución**:
1. Revisa los logs de BusinessTier
2. Verifica que DataTier esté funcionando
3. Verifica la conexión a MySQL

## 📚 Documentación Relacionada

- [BusinessTier API](../BusinessTier/README.md)
- [DataTier gRPC](../DataTier/README.md)
- [KafkaConsumer](../KafkaConsumer/README.md)
- [Email Setup Guide](../docs/email-setup-guide.md)

---

**Desarrollado como parte del proyecto AS-Catalogo-NET**
