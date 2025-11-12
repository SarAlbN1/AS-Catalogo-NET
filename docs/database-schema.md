# 📊 DATABASE SCHEMA DOCUMENTATION

**Proyecto:** AS-Catalogo-NET
**Patrón:** Database First
**Motor:** MySQL 8.0
**Autor:** Nico (Persona 1 - DB & Infrastructure Lead)
**Fecha:** 2025-11-10

---

## 🎯 Resumen Ejecutivo

Esta documentación describe el schema de base de datos para el sistema de catálogo de productos, implementado usando el patrón **Database First** con MySQL 8.0 y Entity Framework Core.

---

## 📐 Diagrama Entidad-Relación

```
┌──────────────────────────────┐
│         CATALOGO             │
├──────────────────────────────┤
│ • Id (PK) INT                │
│ • Nombre VARCHAR(200)        │
│ • Descripcion VARCHAR(500)   │
│ • FechaCreacion DATETIME     │
│ • Activo BOOLEAN             │
└──────────────┬───────────────┘
               │ 1
               │
               │ N
┌──────────────┴───────────────┐
│         PRODUCTO             │
├──────────────────────────────┤
│ • Id (PK) INT                │
│ • Nombre VARCHAR(200)        │
│ • Descripcion VARCHAR(500)   │
│ • Precio DECIMAL(18,2)       │
│ • CantidadDisponible INT     │
│ • FechaCreacion DATETIME     │
│ • FechaActualizacion DATETIME│
│ • CatalogoId (FK) INT        │
│ • Activo BOOLEAN             │
└──────────────────────────────┘
```

---

## 📋 Tablas

### 1. **Catalogo**

Tabla principal que agrupa productos relacionados.

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| `Id` | INT | PRIMARY KEY, AUTO_INCREMENT | Identificador único |
| `Nombre` | VARCHAR(200) | NOT NULL, INDEX | Nombre del catálogo |
| `Descripcion` | VARCHAR(500) | NULL | Descripción opcional |
| `FechaCreacion` | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Fecha de creación |
| `Activo` | BOOLEAN | NOT NULL, DEFAULT TRUE, INDEX | Estado activo/inactivo |

**Índices:**
- `PRIMARY KEY (Id)`
- `INDEX idx_catalogo_activo (Activo)`
- `INDEX idx_catalogo_nombre (Nombre)`

**Collation:** `utf8mb4_unicode_ci`

---

### 2. **Producto**

Tabla que contiene los productos individuales de cada catálogo.

| Columna | Tipo | Restricciones | Descripción |
|---------|------|---------------|-------------|
| `Id` | INT | PRIMARY KEY, AUTO_INCREMENT | Identificador único |
| `Nombre` | VARCHAR(200) | NOT NULL, INDEX | Nombre del producto |
| `Descripcion` | VARCHAR(500) | NOT NULL | Descripción del producto |
| `Precio` | DECIMAL(18,2) | NOT NULL, CHECK >= 0, INDEX | Precio del producto |
| `CantidadDisponible` | INT | NOT NULL, DEFAULT 0, CHECK >= 0 | Stock disponible |
| `FechaCreacion` | DATETIME | NOT NULL, DEFAULT CURRENT_TIMESTAMP | Fecha de creación |
| `FechaActualizacion` | DATETIME | NULL, ON UPDATE CURRENT_TIMESTAMP | Última modificación |
| `CatalogoId` | INT | FOREIGN KEY, INDEX | Referencia a Catalogo |
| `Activo` | BOOLEAN | NOT NULL, DEFAULT TRUE, INDEX | Estado activo/inactivo |

**Índices:**
- `PRIMARY KEY (Id)`
- `INDEX idx_producto_catalogo (CatalogoId)`
- `INDEX idx_producto_nombre (Nombre)`
- `INDEX idx_producto_precio (Precio)`
- `INDEX idx_producto_activo (Activo)`

**Foreign Keys:**
- `CONSTRAINT fk_producto_catalogo`
  - `FOREIGN KEY (CatalogoId) REFERENCES Catalogo(Id)`
  - `ON DELETE CASCADE`
  - `ON UPDATE CASCADE`

**Constraints:**
- `CHECK (Precio >= 0)` - Precio no puede ser negativo
- `CHECK (CantidadDisponible >= 0)` - Cantidad no puede ser negativa

**Collation:** `utf8mb4_unicode_ci`

---

## 🔐 Relaciones

### Catalogo → Producto (1:N)

- Un **Catalogo** puede contener múltiples **Productos**
- Un **Producto** pertenece a un único **Catalogo**
- **Eliminación en cascada**: Al eliminar un Catalogo, se eliminan todos sus Productos
- **Actualización en cascada**: Al actualizar el Id de un Catalogo, se actualizan las referencias

---

## 📦 Stored Procedures

### 1. `sp_GetAllProductos()`

Obtiene todos los productos activos con información del catálogo.

**Parámetros:** Ninguno

**Retorna:** Lista de productos con nombre del catálogo

**Uso:**
```sql
CALL sp_GetAllProductos();
```

---

### 2. `sp_GetProductoById(p_ProductoId)`

Obtiene un producto específico por su ID.

**Parámetros:**
- `p_ProductoId` (INT) - ID del producto

**Retorna:** Producto encontrado o vacío

**Uso:**
```sql
CALL sp_GetProductoById(1);
```

---

### 3. `sp_CreateProducto(...)`

Crea un nuevo producto con validaciones.

**Parámetros:**
- `p_Nombre` (VARCHAR 200)
- `p_Descripcion` (VARCHAR 500)
- `p_Precio` (DECIMAL 18,2)
- `p_CantidadDisponible` (INT)
- `p_CatalogoId` (INT)
- `p_ProductoId` (OUT INT) - ID del producto creado

**Validaciones:**
- Catálogo existe y está activo
- Precio >= 0
- Cantidad >= 0

**Uso:**
```sql
SET @new_id = 0;
CALL sp_CreateProducto('iPhone 16', 'Smartphone Apple', 1299.99, 50, 1, @new_id);
SELECT @new_id;
```

---

### 4. `sp_UpdateProducto(...)`

Actualiza un producto existente.

**Parámetros:**
- `p_ProductoId` (INT)
- `p_Nombre` (VARCHAR 200)
- `p_Descripcion` (VARCHAR 500)
- `p_Precio` (DECIMAL 18,2)
- `p_CantidadDisponible` (INT)

**Validaciones:**
- Producto existe y está activo
- Precio >= 0
- Cantidad >= 0

**Uso:**
```sql
CALL sp_UpdateProducto(1, 'iPhone 16 Pro', 'Nueva descripción', 1499.99, 100);
```

---

### 5. `sp_DeleteProducto(p_ProductoId)`

Elimina lógicamente un producto (soft delete).

**Parámetros:**
- `p_ProductoId` (INT)

**Comportamiento:** Marca `Activo = FALSE` sin eliminar físicamente

**Uso:**
```sql
CALL sp_DeleteProducto(1);
```

---

### 6. `sp_GetProductosByCatalogo(p_CatalogoId)`

Obtiene todos los productos de un catálogo específico.

**Parámetros:**
- `p_CatalogoId` (INT)

**Uso:**
```sql
CALL sp_GetProductosByCatalogo(1);
```

---

### 7. `sp_BuscarProductos(p_Busqueda)`

Busca productos por nombre o descripción (búsqueda parcial).

**Parámetros:**
- `p_Busqueda` (VARCHAR 200)

**Uso:**
```sql
CALL sp_BuscarProductos('iPhone');
```

---

## 📊 Datos de Prueba (Seed Data)

### Catálogos (4)

1. **Electrónica** - Dispositivos y accesorios electrónicos
2. **Hogar y Oficina** - Productos para el hogar y la oficina
3. **Gaming** - Productos para gamers y entusiastas
4. **Accesorios** - Accesorios y periféricos

### Productos (15)

| ID | Nombre | Catálogo | Precio | Stock |
|----|--------|----------|--------|-------|
| 1 | Laptop Dell XPS 15 | Electrónica | $1,299.99 | 15 |
| 2 | iPhone 15 Pro Max | Electrónica | $1,199.99 | 25 |
| 3 | Samsung Galaxy S24 Ultra | Electrónica | $1,099.99 | 20 |
| 4 | MacBook Pro 14" M3 | Electrónica | $1,999.99 | 10 |
| 5 | iPad Pro 12.9" | Electrónica | $799.99 | 18 |
| 6 | Monitor LG UltraWide 34" | Hogar y Oficina | $449.99 | 30 |
| 7 | Silla Ergonómica Herman Miller | Hogar y Oficina | $899.99 | 12 |
| 8 | Escritorio Elevable ElectricDesk | Hogar y Oficina | $649.99 | 8 |
| 9 | PlayStation 5 Pro | Gaming | $699.99 | 22 |
| 10 | NVIDIA RTX 4090 | Gaming | $1,599.99 | 5 |
| 11 | Razer BlackWidow V4 Pro | Gaming | $229.99 | 40 |
| 12 | Mouse Logitech MX Master 3S | Accesorios | $99.99 | 50 |
| 13 | Webcam Logitech Brio 4K | Accesorios | $199.99 | 35 |
| 14 | AirPods Pro (2da Gen) | Accesorios | $249.99 | 60 |
| 15 | SteelSeries Arctis Nova Pro | Accesorios | $349.99 | 28 |

---

## 🔧 Configuración de Conexión

**Connection String (Docker):**
```
Server=mysql;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;
```

**Connection String (Local):**
```
Server=localhost;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;
```

---

## 🚀 Comandos Útiles

### Verificar Estructura
```sql
SHOW TABLES;
DESCRIBE Catalogo;
DESCRIBE Producto;
```

### Ver Stored Procedures
```sql
SHOW PROCEDURE STATUS WHERE Db = 'catalogo_db';
```

### Ejecutar Scaffold (Entity Framework)
```bash
dotnet ef dbcontext scaffold \
  "Server=localhost;Port=3306;Database=catalogo_db;User=catalogo_user;Password=catalogo_pass;" \
  Pomelo.EntityFrameworkCore.MySql \
  --output-dir Models \
  --force \
  --data-annotations
```

---

## 📝 Notas de Implementación

### Patrón Database First

1. **Tablas creadas primero** en MySQL mediante scripts SQL
2. **Stored Procedures** implementados para lógica de negocio
3. **Modelos de C#** generados automáticamente desde la BD
4. **DbContext** creado por scaffolding

### Ventajas

- ✅ Control total sobre el schema SQL
- ✅ Optimización de índices desde el diseño
- ✅ Stored Procedures para lógica compleja
- ✅ Migración facilitada a otros ORMs

### Soft Delete

Todos los deletes son **lógicos** (soft delete):
- No se eliminan físicamente registros
- Se marca `Activo = FALSE`
- Los SPs filtran automáticamente por `Activo = TRUE`

---

## 🔍 Consultas de Ejemplo

### Productos más caros
```sql
SELECT * FROM Producto
WHERE Activo = TRUE
ORDER BY Precio DESC
LIMIT 10;
```

### Productos con stock bajo
```sql
SELECT * FROM Producto
WHERE Activo = TRUE
  AND CantidadDisponible < 10;
```

### Resumen por catálogo
```sql
SELECT
    c.Nombre AS Catalogo,
    COUNT(p.Id) AS TotalProductos,
    AVG(p.Precio) AS PrecioPromedio,
    SUM(p.CantidadDisponible) AS StockTotal
FROM Catalogo c
LEFT JOIN Producto p ON c.Id = p.CatalogoId AND p.Activo = TRUE
WHERE c.Activo = TRUE
GROUP BY c.Id, c.Nombre;
```

---

## 📚 Referencias

- [MySQL 8.0 Documentation](https://dev.mysql.com/doc/refman/8.0/en/)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Pomelo MySQL Provider](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)
- [Database First Approach](https://docs.microsoft.com/en-us/ef/core/managing-schemas/scaffolding)

---

**Última actualización:** 2025-11-10
**Versión del Schema:** 1.0
**Autor:** Nico (Database & Infrastructure Lead)
