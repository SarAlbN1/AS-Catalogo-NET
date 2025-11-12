# 🚀 Guía de Integración - AS-Catalogo-NET

**Fecha:** 2025-11-10
**Autor:** Nico (Persona 1 - Database & Infrastructure Lead)
**Estado:** ✅ Integración completa y probada

---

## 📋 Tabla de Contenidos

1. [Resumen de la Integración](#resumen-de-la-integración)
2. [Arquitectura del Sistema](#arquitectura-del-sistema)
3. [Requisitos Previos](#requisitos-previos)
4. [Instalación](#instalación)
5. [Configuración](#configuración)
6. [Cómo Ejecutar el Sistema](#cómo-ejecutar-el-sistema)
7. [Cómo Probar la Integración](#cómo-probar-la-integración)
8. [Solución de Problemas](#solución-de-problemas)
9. [Detalles Técnicos](#detalles-técnicos)

---

## 🎯 Resumen de la Integración

Este proyecto integra exitosamente tres enfoques de desarrollo trabajados por diferentes miembros del equipo:

- **Persona 1 (Nico):** Database First + Kafka Infrastructure
- **Persona 2 (Alejandro):** DataTier con gRPC
- **Persona 3 (Sara):** BusinessTier con REST API

### ✅ Estado de la Integración

| Componente | Estado | Descripción |
|------------|--------|-------------|
| **Database First** | ✅ Funcionando | MySQL con stored procedures y 15 productos seed |
| **Kafka Infrastructure** | ✅ Funcionando | Zookeeper, Kafka, Kafka UI configurados |
| **DataTier (gRPC)** | ✅ Funcionando | Servidor gRPC .NET 9 en puerto 5003 |
| **BusinessTier (REST)** | ✅ Funcionando | API REST .NET 8 en puerto 8888 |
| **Integración End-to-End** | ✅ PROBADA | HTTP → REST → gRPC → MySQL funcionando |

---

## 🏗️ Arquitectura del Sistema

```
┌─────────────────┐
│   HTTP Client   │
│  (curl/browser) │
└────────┬────────┘
         │ HTTP
         ↓
┌─────────────────────────┐
│   BusinessTier (REST)   │
│   Puerto: 8888          │
│   Framework: .NET 8     │
└────────┬────────────────┘
         │ gRPC
         ↓
┌─────────────────────────┐
│   DataTier (gRPC)       │
│   Puerto: 5003          │
│   Framework: .NET 9     │
└────────┬────────────────┘
         │ SQL
         ↓
┌─────────────────────────┐
│   MySQL Database        │
│   Puerto: 3306          │
│   15 productos activos  │
└─────────────────────────┘

         ↓ Events
┌─────────────────────────┐
│   Kafka + Zookeeper     │
│   Topic: product-events │
└─────────────────────────┘
```

---

## 📦 Requisitos Previos

### Software Necesario

1. **.NET SDK 8.0** (para BusinessTier)
   ```bash
   brew install dotnet@8
   ```

2. **.NET SDK 9.0** (para DataTier)
   ```bash
   brew install dotnet
   ```

3. **Docker & Docker Compose**
   ```bash
   brew install docker docker-compose
   ```

4. **MySQL Client** (opcional, para debugging)
   ```bash
   brew install mysql-client
   ```

### Verificar Instalación

```bash
# Verificar .NET 8
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
dotnet --version  # Debe mostrar 8.0.x

# Verificar .NET 9
export PATH="/usr/local/share/dotnet:$PATH"
dotnet --version  # Debe mostrar 9.0.x

# Verificar Docker
docker --version
docker-compose --version
```

---

## 🔧 Instalación

### 1. Clonar el Repositorio

```bash
git clone <repository-url>
cd AS-Catalogo-NET
git checkout feature/db-infra-nicolas
```

### 2. Levantar Infraestructura con Docker

```bash
# Levantar MySQL, Kafka, Zookeeper, Kafka UI
docker-compose up -d mysql kafka zookeeper kafka-ui

# Verificar que los servicios estén corriendo
docker-compose ps

# Esperar a que MySQL esté listo (healthcheck)
docker-compose logs -f mysql  # Ctrl+C cuando veas "ready for connections"
```

### 3. Verificar Base de Datos

```bash
# Los scripts SQL se ejecutan automáticamente al iniciar MySQL
# Verificar que las tablas y datos existan:
docker exec catalogo_mysql mysql -ucatalogo_user -pcatalogo_pass catalogo_db \
  -e "SELECT COUNT(*) as total FROM Producto WHERE Activo = TRUE;"

# Debe mostrar: total = 15
```

### 4. Verificar Kafka

```bash
# Ejecutar script de validación
chmod +x test-kafka.sh
./test-kafka.sh

# O acceder a Kafka UI en el navegador:
# http://localhost:8081
```

---

## ⚙️ Configuración

### Variables de Entorno Importantes

#### DataTier (gRPC Server)

```bash
export GrpcPort=5003  # Puerto gRPC (default: 5001)
export ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;"
export Kafka__BootstrapServers="localhost:9092"
export Kafka__Topic="product-events"
```

#### BusinessTier (REST API)

```bash
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
export PreferGrpc=true  # IMPORTANTE: usar gRPC en lugar de acceso directo a BD
export GrpcSettings__DataTierUrl="http://localhost:5003"
export ASPNETCORE_URLS="http://localhost:8888"
```

---

## 🚀 Cómo Ejecutar el Sistema

### Opción 1: Ejecución Local (Recomendado para Testing)

#### Terminal 1: DataTier

```bash
export PATH="/usr/local/share/dotnet:$PATH"
cd DataTier

# Compilar
dotnet build

# Ejecutar
GrpcPort=5003 \
ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;" \
Kafka__BootstrapServers="localhost:9092" \
Kafka__Topic="product-events" \
dotnet run --no-launch-profile

# Debe mostrar: "Now listening on: http://[::]:5003"
```

#### Terminal 2: BusinessTier

```bash
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
cd BusinessTier

# Compilar
dotnet build

# Ejecutar
PreferGrpc=true \
GrpcSettings__DataTierUrl="http://localhost:5003" \
ASPNETCORE_URLS="http://localhost:8888" \
dotnet run --no-launch-profile

# Debe mostrar: "Now listening on: http://localhost:8888"
```

### Opción 2: Ejecución con Docker Compose (Producción)

⚠️ **Nota:** Docker con Apple Silicon (ARM64) tiene problemas con gRPC Tools. Use ejecución local para testing.

```bash
# Build images
docker-compose build data-tier business-tier

# Levantar servicios
docker-compose up data-tier business-tier

# Ver logs
docker-compose logs -f data-tier business-tier
```

---

## 🧪 Cómo Probar la Integración

### 1. Verificar que los Servicios Estén Corriendo

```bash
# DataTier (debe responder con error HTTP/2, es esperado para gRPC)
curl -v http://localhost:5003/
# Esperado: "HTTP/1.1 400 Bad Request" con mensaje "HTTP/1.x request was sent to an HTTP/2 only endpoint"

# BusinessTier (debe responder OK)
curl http://localhost:8888/
# Esperado: HTTP 200 o redirección
```

### 2. Probar GET All Productos (Integración Completa)

```bash
# Obtener todos los productos
curl -s http://localhost:8888/api/productos | python3 -m json.tool

# O con jq (si está instalado)
curl -s http://localhost:8888/api/productos | jq '.'
```

**Respuesta Esperada:**

```json
[
    {
        "id": 1,
        "nombre": "Laptop Dell XPS 15",
        "descripcion": "Laptop profesional con procesador Intel i7, 16GB RAM, 512GB SSD, pantalla 4K",
        "precio": 1299.99,
        "cantidadDisponible": 15,
        "fechaCreacion": "2025-11-10T18:42:43",
        "catalogoId": 1,
        "fechaActualizacion": null,
        "activo": true
    },
    ...
]
```

**✅ Verificar que la respuesta incluya:**
- Campo `fechaActualizacion` (nuevo campo de Database First)
- Campo `activo` (nuevo campo de Database First)
- 15 productos en total

### 3. Probar GET Producto por ID

```bash
curl -s http://localhost:8888/api/productos/1 | python3 -m json.tool
```

### 4. Probar CREATE Producto

```bash
curl -X POST http://localhost:8888/api/productos \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Test Product",
    "descripcion": "Producto de prueba",
    "precio": 99.99,
    "cantidadDisponible": 10,
    "catalogoId": 1
  }' | python3 -m json.tool
```

**✅ Verificar que el producto creado tenga `activo: true`**

### 5. Probar UPDATE Producto

```bash
curl -X PUT http://localhost:8888/api/productos/1 \
  -H "Content-Type: application/json" \
  -d '{
    "nombre": "Laptop Dell XPS 15 UPDATED",
    "descripcion": "Nueva descripción",
    "precio": 1399.99,
    "cantidadDisponible": 20
  }' | python3 -m json.tool
```

### 6. Probar DELETE Producto (Soft Delete)

```bash
# Eliminar producto (soft delete)
curl -X DELETE http://localhost:8888/api/productos/1

# Verificar que el producto ya no aparezca en la lista
curl -s http://localhost:8888/api/productos | python3 -m json.tool | grep '"id": 1'
# No debe aparecer (está marcado como Activo = false)

# Verificar en la BD que sigue existiendo pero inactivo
docker exec catalogo_mysql mysql -ucatalogo_user -pcatalogo_pass catalogo_db \
  -e "SELECT Id, Nombre, Activo FROM Producto WHERE Id = 1;"
# Debe mostrar: Activo = 0
```

### 7. Verificar Eventos de Kafka (Opcional)

```bash
# Acceder a Kafka UI
open http://localhost:8081

# O consumir eventos desde terminal
docker exec catalogo_kafka kafka-console-consumer \
  --bootstrap-server localhost:9092 \
  --topic product-events \
  --from-beginning \
  --max-messages 5
```

---

## 🐛 Solución de Problemas

### Problema 1: "Port 5001 already in use"

**Causa:** El puerto 5001 está ocupado o en TIME_WAIT.

**Solución 1:** Usar puerto alternativo

```bash
# Levantar DataTier en puerto diferente
GrpcPort=5003 dotnet run --no-launch-profile
```

**Solución 2:** Matar procesos en el puerto

```bash
lsof -ti:5001 | xargs kill -9
```

**Solución 3:** Esperar 60 segundos para que el SO libere el puerto

### Problema 2: "Table 'Productos' doesn't exist"

**Causa:** BusinessTier está intentando acceder directamente a la BD en lugar de usar gRPC.

**Solución:** Asegurarse de que `PreferGrpc=true`

```bash
PreferGrpc=true dotnet run
```

### Problema 3: BusinessTier no puede compilar

**Causa:** .NET 8 no está instalado o no está en el PATH.

**Solución:**

```bash
# Instalar .NET 8
brew install dotnet@8

# Configurar PATH
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"

# Verificar
dotnet --version  # Debe mostrar 8.0.x
```

### Problema 4: "The seed entity for entity type 'Catalogo' cannot be added"

**Causa:** Conflicto entre seed data de Code First y Database First.

**Solución:** Ya está resuelto en el último commit. Los seed data están comentados en `MyAppDbContext.cs`.

### Problema 5: MySQL no está listo

**Causa:** MySQL tarda en inicializar la primera vez.

**Solución:**

```bash
# Esperar a que el healthcheck pase
docker-compose ps  # Verificar que mysql muestre (healthy)

# Ver logs
docker-compose logs -f mysql  # Esperar "ready for connections"
```

### Problema 6: No puedo conectar a gRPC desde BusinessTier

**Causa:** DataTier no está corriendo o la URL es incorrecta.

**Solución:**

```bash
# Verificar que DataTier esté corriendo
curl -v http://localhost:5003/
# Debe responder (aunque sea con error HTTP/2)

# Verificar variable de entorno
echo $GrpcSettings__DataTierUrl
# Debe mostrar: http://localhost:5003

# Ver logs de BusinessTier para mensajes de error gRPC
```

---

## 📚 Detalles Técnicos

### Cambios de Integración Realizados

#### DataTier

1. **Protos/productos.proto** (líneas 27-28)
   - Agregado `string fecha_actualizacion = 8;`
   - Agregado `bool activo = 9;`

2. **ProductosGrpcService.cs**
   - Línea 31: Filtro `Where(p => p.Activo == true)`
   - Línea 88: Inicialización `Activo = true`
   - Líneas 166-167: Soft delete `Activo = false`
   - Líneas 203-204: MapToProto con nuevos campos

3. **Program.cs** (líneas 8-16)
   - Puerto configurable via `GrpcPort` variable

4. **MyAppDbContext.cs** (líneas 26-31)
   - Seed data comentado (Database First prevalece)

#### BusinessTier

1. **Protos/productos.proto** (líneas 25-26)
   - Sincronizado con DataTier

2. **ProductoDto.cs** (líneas 12-13)
   - `public DateTime? FechaActualizacion { get; set; }`
   - `public bool Activo { get; set; }`

3. **ProductosGrpcClient.cs** (líneas 152-153)
   - Map actualizado para nuevos campos

4. **Dockerfile** (nuevo archivo)
   - Creado para despliegue con .NET 8

### Estructura de la Base de Datos

```sql
-- Tabla Producto con nuevos campos
CREATE TABLE Producto (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    Nombre VARCHAR(200) NOT NULL,
    Descripcion VARCHAR(500) NOT NULL,
    Precio DECIMAL(18,2) NOT NULL,
    CantidadDisponible INT NOT NULL DEFAULT 0,
    FechaCreacion DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FechaActualizacion DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,  -- NUEVO
    CatalogoId INT NOT NULL,
    Activo BOOLEAN NOT NULL DEFAULT TRUE,  -- NUEVO
    FOREIGN KEY (CatalogoId) REFERENCES Catalogo(Id)
);
```

### Stored Procedures Disponibles

1. `sp_GetAllProductos()` - Obtiene todos los productos activos
2. `sp_GetProductoById(p_ProductoId)` - Obtiene un producto por ID
3. `sp_CreateProducto(...)` - Crea un nuevo producto
4. `sp_UpdateProducto(...)` - Actualiza un producto existente
5. `sp_DeleteProducto(p_ProductoId)` - Soft delete de un producto
6. `sp_GetProductosByCatalogo(p_CatalogoId)` - Productos por catálogo
7. `sp_BuscarProductos(p_Busqueda)` - Búsqueda parcial

### Endpoints REST Disponibles

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/productos` | Obtener todos los productos activos |
| GET | `/api/productos/{id}` | Obtener producto por ID |
| POST | `/api/productos` | Crear nuevo producto |
| PUT | `/api/productos/{id}` | Actualizar producto |
| DELETE | `/api/productos/{id}` | Eliminar producto (soft delete) |

---

## 🎓 Para Tu Profesor

### Cómo Ejecutar Todo desde Cero

```bash
# 1. Levantar infraestructura
docker-compose up -d mysql kafka zookeeper kafka-ui

# 2. Esperar 30 segundos para que MySQL inicialice

# 3. Verificar BD
docker exec catalogo_mysql mysql -ucatalogo_user -pcatalogo_pass catalogo_db \
  -e "SELECT COUNT(*) FROM Producto;"
# Debe mostrar: 15

# 4. Terminal 1: DataTier
cd DataTier
GrpcPort=5003 \
ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;" \
dotnet run --no-launch-profile

# 5. Terminal 2: BusinessTier
export DOTNET_ROOT="/opt/homebrew/opt/dotnet@8/libexec"
export PATH="/opt/homebrew/opt/dotnet@8/bin:$PATH"
cd BusinessTier
PreferGrpc=true \
GrpcSettings__DataTierUrl="http://localhost:5003" \
dotnet run --no-launch-profile

# 6. Probar integración
curl -s http://localhost:8888/api/productos | python3 -m json.tool | head -30
```

### Verificación de Integración

✅ **Si ves esto, TODO está funcionando:**

```json
{
    "id": 1,
    "nombre": "Laptop Dell XPS 15",
    ...
    "fechaActualizacion": null,  // ← Campo de Database First
    "activo": true               // ← Campo de Database First
}
```

---

## 📞 Contacto

**Persona 1 (Nico)** - Database & Infrastructure Lead
**Branch:** `feature/db-infra-nicolas`
**Última actualización:** 2025-11-10

---

## 📄 Licencia

Este proyecto es parte de un taller académico de Arquitectura de Software.
