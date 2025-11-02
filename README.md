# AS – Catálogo .NET

API RESTful para gestión de productos, desarrollada con **ASP.NET Core 8** y **Entity Framework Core 8** usando **SQLite** como base de datos embebida. Incluye **CRUD completo**, **Swagger/OpenAPI**, y datos de ejemplo (seed). Probado con **Postman**.

---

## 📝 Descripción

Este proyecto expone endpoints REST para administrar un catálogo de productos. La persistencia se implementa con **EF Core** (ORM) sobre **SQLite** para desarrollo rápido sin dependencias externas. Se versiona la BD (`Data/catalogo.db`) para que el equipo comparta el mismo estado en dev.

### ✅ Implementado
- Modelo `Producto` (Id, Nombre, Precio, Stock, Activo, CreadoEn).
- DTOs: `ProductoCreateDto`, `ProductoUpdateDto`.
- Capa de servicios: `IProductosService`, `ProductosService`.
- Controlador: `ProductosController` (CRUD + búsqueda + paginación).
- DbContext + configuración: `CatalogoDb` (EF Core + SQLite).
- Seed de datos inicial.
- Swagger/OpenAPI habilitado en Development.
- **Postman**: endpoints probados y verificados.

### 🚧 Pendiente / Siguientes pasos
- Validaciones más estrictas y manejo de errores detallado.


---

## 🚀 Tecnologías

- **.NET 8** (ASP.NET Core Web API)
- **EF Core 8** (ORM)
- **SQLite** (dev / embebido)
- **Swashbuckle.AspNetCore** (Swagger)

---

## 🗂️ Estructura mínima

```
Catalogo.Api/
├── Controllers/
│   └── ProductosController.cs
├── Data/
│   ├── CatalogoDb.cs
│   ├── Seed.cs
│   └── data/catalogo.db   ← BD versionada (dev)
├── DTOs/
│   ├── ProductoCreateDto.cs
│   └── ProductoUpdateDto.cs
├── Models/
│   └── Producto.cs
├── Services/
│   ├── IProductosService.cs
│   └── ProductosService.cs
├── Program.cs
└── appsettings.json
```

---

## 🔌 Ejecución local

1) Restaurar, compilar y correr (puerto recomendado **5280**):
```bash
dotnet run --project Catalogo.Api/Catalogo.Api.csproj --urls http://localhost:5280
```

2) Entrar a Swagger:
- **http://localhost:5280/swagger**

> Si aparece “address already in use”, libera el puerto:
```bash
lsof -nP -iTCP:5280 | grep LISTEN
kill -9 <PID>
```

---

## 🔗 Endpoints principales

Base URL: `http://localhost:5280`

- `GET /api/Productos` — lista (query: `q`, `page`, `pageSize`)
- `GET /api/Productos/{id}`
- `POST /api/Productos`
- `PUT /api/Productos/{id}`
- `DELETE /api/Productos/{id}`

### Ejemplos rápidos (body JSON)

**POST /api/Productos**
```json
{ "nombre": "Cargador USB-C", "precio": 89900, "stock": 25 }
```

**PUT /api/Productos/1**
```json
{ "nombre": "Cargador USB-C 65W", "precio": 119900, "stock": 20, "activo": true }
```

---

## 🗄️ Base de datos

- **Dev:** `SQLite` embebido (`Data/catalogo.db`) **versionado** en el repo para compartir estado.
- **Seed:** `Seed.CargarAsync(...)` crea datos demo en el primer arranque.

> **Nota de equipo:** Antes de ejecutar, hacer `git pull` con la API apagada para evitar conflictos del archivo `.db`.

---

## 🔧 EF Core (comandos útiles)

```bash
# Instalar herramienta (una vez)
dotnet tool install --global dotnet-ef

# Crear migración (si se cambia el modelo y migramos a RDBMS)
dotnet ef migrations add InitialCreate --project Catalogo.Api --startup-project Catalogo.Api

# Aplicar migraciones
dotnet ef database update --project Catalogo.Api --startup-project Catalogo.Api
```

---

## 👥 Autores

- Alejandro Caicedo (INDI260) – DB/ORM
- Sara Albarracín (SarAlbN1) – CRUD/Servicios/Controlador + pruebas Postman

---
