# AS – Catálogo .NET

Proyecto base para una API RESTful de gestión de catálogo de productos desarrollada con .NET 9 y MySQL.

## 📝 Descripción del Proyecto

Este proyecto es una aplicación web ASP.NET Core configurada con Entity Framework Core y MySQL. Actualmente cuenta con:

✅ **Implementado:**
- Configuración completa de Entity Framework Core con MySQL
- Modelo de datos (Catálogo y Productos) con relaciones
- Migraciones automáticas al iniciar la aplicación
- Datos de prueba iniciales (8 productos)
- Dockerización completa (MySQL + Aplicación .NET)
- Persistencia de datos con volúmenes Docker

🚧 **Pendiente de implementar:**
- Controladores API REST para CRUD de productos
- Endpoints de la API
- Validaciones y manejo de errores
- Documentación Swagger/OpenAPI

La aplicación utiliza Entity Framework Core como ORM para interactuar con una base de datos MySQL, y está completamente dockerizada para facilitar el despliegue y desarrollo.

## 🚀 Tecnologías Utilizadas

### Backend
- **.NET 9.0** - Framework principal
- **ASP.NET Core** - Framework web para APIs REST
- **Entity Framework Core 9.0** - ORM para acceso a datos
- **C# 12** - Lenguaje de programación

### Base de Datos
- **MySQL 8.0** - Sistema de gestión de base de datos
- **Pomelo.EntityFrameworkCore.MySql 9.0** - Provider de MySQL para EF Core

### DevOps y Contenedores
- **Docker** - Containerización de la aplicación
- **Docker Compose** - Orquestación de contenedores
- **Multi-stage Dockerfile** - Optimización de imágenes

### Herramientas de Desarrollo
- **Entity Framework Tools** - Migraciones de base de datos
- **OpenAPI/Swagger** - Documentación de la API

## 📊 Modelo de Datos

### Catálogo
- `Id` (int, PK): Identificador único del catálogo
- `Productos` (Collection): Colección de productos

### Producto
- `Id` (int, PK): Identificador único del producto
- `Nombre` (string, required): Nombre del producto
- `Descripcion` (string, required): Descripción detallada
- `Precio` (decimal, required): Precio del producto
- `CantidadDisponible` (int, required): Stock disponible
- `FechaCreacion` (DateTime): Fecha de creación del registro
- `CatalogoId` (int, FK): Referencia al catálogo

**Relación:** Un catálogo contiene múltiples productos (relación 1:N con eliminación en cascada).

## 🐳 Despliegue con Docker

### Prerequisitos
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) instalado y ejecutándose
- 8 GB de RAM disponible (mínimo)
- Puertos 3306 y 8080 libres

### Opción 1: Despliegue Rápido con Docker Compose

```powershell
# Clonar el repositorio
git clone https://github.com/SarAlbN1/AS-Catalogo-NET.git
cd AS-Catalogo-NET

# Iniciar todos los servicios
docker-compose up -d

# Verificar que los contenedores estén corriendo
docker-compose ps

# Ver logs de la aplicación
docker-compose logs -f app
```

La aplicación estará disponible en:
- **Aplicación Web**: http://localhost:8080
- **Base de Datos MySQL**: localhost:3306

> **Nota:** Los endpoints de la API REST aún no están implementados. Actualmente la aplicación solo inicia y configura la base de datos con datos de prueba.

### Arquitectura de Contenedores

**Servicios desplegados:**

1. **mysql** (Puerto 3306)
   - Imagen: `mysql:8.0`
   - Base de datos: `catalogo_db`
   - Usuario: `catalogo_user`
   - Contraseña: `catalogo_pass`
   - Volumen persistente: `mysql_data`
   - Healthcheck: Verifica disponibilidad cada 10s

2. **app** (Puerto 8080)
   - Imagen: Construida desde Dockerfile
   - Framework: .NET 9 ASP.NET Core
   - Migraciones automáticas al iniciar
   - Dependencia: Espera a que MySQL esté saludable

### Datos de Prueba

Al iniciar por primera vez, se crean automáticamente:
- 1 catálogo principal
- 8 productos de ejemplo con diferentes precios y cantidades:
  - Laptop Dell XPS 15 (10 unidades, $1,299.99)
  - Mouse Logitech MX Master 3 (50 unidades, $99.99)
  - Teclado Mecánico Corsair K95 (25 unidades, $189.99)
  - Monitor Samsung 27" 4K (15 unidades, $449.99)
  - Auriculares Sony WH-1000XM5 (30 unidades, $349.99)
  - Webcam Logitech C920 (40 unidades, $79.99)
  - SSD Samsung 1TB (60 unidades, $129.99)
  - Hub USB-C Anker (35 unidades, $59.99)

### Persistencia de Datos

Los datos de MySQL se almacenan en un volumen Docker (`mysql_data`) que persiste entre reinicios. Para eliminar completamente los datos:

```powershell
docker-compose down -v
```

## 💻 Desarrollo Local (Sin Docker)

### Prerequisitos
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [MySQL 8.0](https://dev.mysql.com/downloads/mysql/)
- [Entity Framework Tools](https://docs.microsoft.com/en-us/ef/core/cli/dotnet)

### Configuración

1. **Instalar dependencias:**
```powershell
cd AS-Catalogo-NET
dotnet restore
```

2. **Configurar la cadena de conexión:**

Edita `appsettings.json` con tus credenciales de MySQL local:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Port=3306;Database=catalogo_db;User=root;Password=tu_contraseña;"
  }
}
```

3. **Crear la base de datos:**
```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Ejecutar la aplicación:**
```powershell
dotnet run
```

La aplicación estará disponible en: https://localhost:5001 o http://localhost:5000

> **Nota:** Actualmente solo se configura la base de datos. Los endpoints REST se implementarán próximamente.

## 📋 Estado Actual del Proyecto

### ✅ Completado
- [x] Configuración de Entity Framework Core con MySQL
- [x] Modelos de datos (Catalogo y Producto)
- [x] Relaciones entre entidades (1:N con cascada)
- [x] Migraciones de base de datos
- [x] Aplicación automática de migraciones al iniciar
- [x] Datos de prueba (seed data)
- [x] Configuración de Docker y Docker Compose
- [x] Volúmenes persistentes para MySQL
- [x] Scripts de gestión (PowerShell)

### 🚧 Pendiente
- [ ] Implementar ProductosController
- [ ] Endpoints CRUD para productos
- [ ] DTOs (Data Transfer Objects)
- [ ] Validaciones de datos
- [ ] Manejo de errores y excepciones
- [ ] Configuración de Swagger/OpenAPI
- [ ] Tests unitarios
- [ ] Tests de integración

## 🔧 Comandos Útiles

### Entity Framework Migrations

```powershell
# Crear nueva migración
dotnet ef migrations add NombreDeLaMigracion

# Aplicar migraciones pendientes
dotnet ef database update

# Revertir a una migración específica
dotnet ef database update NombreMigracionAnterior

# Eliminar última migración (si no se aplicó)
dotnet ef migrations remove

# Listar migraciones
dotnet ef migrations list
```

### Docker

```powershell
# Ver logs de MySQL
docker-compose logs -f mysql

# Ver logs de la aplicación
docker-compose logs -f app

# Acceder al contenedor de MySQL
docker-compose exec mysql mysql -u catalogo_user -p
# Password: catalogo_pass

# Acceder al contenedor de la aplicación
docker-compose exec app bash

# Reconstruir solo la aplicación
docker-compose up -d --build app

# Ver estado de los contenedores
docker-compose ps

# Detener sin eliminar volúmenes
docker-compose down

# Detener y eliminar todo
docker-compose down -v
```

### Gestión de Base de Datos

```powershell
# Conectarse a MySQL desde línea de comandos
docker-compose exec mysql mysql -u catalogo_user -p catalogo_db

# Exportar backup de la base de datos
docker-compose exec mysql mysqldump -u catalogo_user -p catalogo_db > backup.sql

# Importar backup
docker-compose exec -T mysql mysql -u catalogo_user -p catalogo_db < backup.sql
```

## 🧪 Testing

```powershell
# Ejecutar tests (cuando estén implementados)
dotnet test

# Ejecutar con cobertura
dotnet test /p:CollectCoverage=true
```

## 📚 Documentación Adicional

- [DOCKER-README.md](DOCKER-README.md) - Guía detallada de Docker
- [README-DOCKER.md](README-DOCKER.md) - Instrucciones completas de despliegue

## 🔍 Solución de Problemas

### La aplicación no se conecta a MySQL

```powershell
# Verificar que MySQL esté corriendo
docker-compose ps

# Revisar logs de MySQL
docker-compose logs mysql

# Esperar a que MySQL termine de inicializar (puede tomar 30-60 segundos)
docker-compose logs -f mysql
```

### Error "Puerto ya en uso"

```powershell
# Cambiar puertos en docker-compose.yml
# Para MySQL: "3307:3306" en lugar de "3306:3306"
# Para App: "8081:8080" en lugar de "8080:8080"
```

### Reiniciar desde cero

```powershell
# Eliminar contenedores, redes y volúmenes
docker-compose down -v

# Limpiar imágenes antiguas
docker system prune -a

# Volver a construir
docker-compose up -d --build
```

### Migraciones no se aplican

```powershell
# Aplicar manualmente desde el contenedor
docker-compose exec app dotnet ef database update

# O reiniciar la aplicación
docker-compose restart app
```

## 👥 Contribución

Alejandro Caicedo (INDI260)
Sara Albarracin (SarAlbN1)

## 📄 Licencia

Este proyecto es parte de un taller académico.

## 🔗 Enlaces Útiles

- [Documentación de .NET](https://docs.microsoft.com/en-us/dotnet/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Docker Documentation](https://docs.docker.com/)
- [MySQL Documentation](https://dev.mysql.com/doc/)
- [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/)
