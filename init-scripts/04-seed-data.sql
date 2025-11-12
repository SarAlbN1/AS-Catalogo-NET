-- ============================================
-- Script: 04-seed-data.sql
-- Descripción: Datos de prueba para Catálogos y Productos
-- Patrón: Database First
-- Autor: Nico (Persona 1 - DB & Infrastructure Lead)
-- ============================================

USE catalogo_db;

-- Deshabilitar validaciones temporalmente
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================
-- Limpiar datos existentes (solo en desarrollo)
-- ============================================
TRUNCATE TABLE Producto;
TRUNCATE TABLE Catalogo;

-- ============================================
-- Insertar Catálogos
-- ============================================
INSERT INTO Catalogo (Id, Nombre, Descripcion, FechaCreacion, Activo) VALUES
(1, 'Electrónica', 'Dispositivos y accesorios electrónicos', NOW(), TRUE),
(2, 'Hogar y Oficina', 'Productos para el hogar y la oficina', NOW(), TRUE),
(3, 'Gaming', 'Productos para gamers y entusiastas', NOW(), TRUE),
(4, 'Accesorios', 'Accesorios y periféricos', NOW(), TRUE);

-- ============================================
-- Insertar Productos - Catálogo Electrónica (ID: 1)
-- ============================================
INSERT INTO Producto (Nombre, Descripcion, Precio, CantidadDisponible, CatalogoId, FechaCreacion, Activo) VALUES
('Laptop Dell XPS 15', 'Laptop profesional con procesador Intel i7, 16GB RAM, 512GB SSD, pantalla 4K', 1299.99, 15, 1, NOW(), TRUE),
('iPhone 15 Pro Max', 'Smartphone Apple con chip A17 Pro, 256GB, cámara de 48MP', 1199.99, 25, 1, NOW(), TRUE),
('Samsung Galaxy S24 Ultra', 'Smartphone Android con S Pen, 12GB RAM, pantalla Dynamic AMOLED 2X', 1099.99, 20, 1, NOW(), TRUE),
('MacBook Pro 14" M3', 'Laptop Apple con chip M3, 16GB RAM unificada, 1TB SSD', 1999.99, 10, 1, NOW(), TRUE),
('iPad Pro 12.9"', 'Tablet profesional con chip M2, 128GB, Apple Pencil compatible', 799.99, 18, 1, NOW(), TRUE);

-- ============================================
-- Insertar Productos - Catálogo Hogar y Oficina (ID: 2)
-- ============================================
INSERT INTO Producto (Nombre, Descripcion, Precio, CantidadDisponible, CatalogoId, FechaCreacion, Activo) VALUES
('Monitor LG UltraWide 34"', 'Monitor curvo 34 pulgadas, resolución 3440x1440, 144Hz, IPS', 449.99, 30, 2, NOW(), TRUE),
('Silla Ergonómica Herman Miller', 'Silla de oficina con soporte lumbar ajustable, reposabrazos 4D', 899.99, 12, 2, NOW(), TRUE),
('Escritorio Elevable ElectricDesk', 'Escritorio de pie ajustable eléctricamente, superficie 180x80cm', 649.99, 8, 2, NOW(), TRUE);

-- ============================================
-- Insertar Productos - Catálogo Gaming (ID: 3)
-- ============================================
INSERT INTO Producto (Nombre, Descripcion, Precio, CantidadDisponible, CatalogoId, FechaCreacion, Activo) VALUES
('PlayStation 5 Pro', 'Consola de última generación con 2TB SSD, Ray Tracing mejorado', 699.99, 22, 3, NOW(), TRUE),
('NVIDIA RTX 4090', 'Tarjeta gráfica high-end con 24GB GDDR6X, perfecta para gaming 4K', 1599.99, 5, 3, NOW(), TRUE),
('Razer BlackWidow V4 Pro', 'Teclado mecánico RGB con switches Green, reposamuñecas magnético', 229.99, 40, 3, NOW(), TRUE);

-- ============================================
-- Insertar Productos - Catálogo Accesorios (ID: 4)
-- ============================================
INSERT INTO Producto (Nombre, Descripcion, Precio, CantidadDisponible, CatalogoId, FechaCreacion, Activo) VALUES
('Mouse Logitech MX Master 3S', 'Mouse inalámbrico ergonómico con sensor 8K DPI, scroll electromagnético', 99.99, 50, 4, NOW(), TRUE),
('Webcam Logitech Brio 4K', 'Webcam profesional con resolución 4K, HDR, enfoque automático', 199.99, 35, 4, NOW(), TRUE),
('AirPods Pro (2da Gen)', 'Audífonos inalámbricos con cancelación activa de ruido, audio espacial', 249.99, 60, 4, NOW(), TRUE),
('SteelSeries Arctis Nova Pro', 'Audífonos gaming con audio Hi-Res, micrófono retráctil, DAC incorporado', 349.99, 28, 4, NOW(), TRUE);

-- Rehabilitar validaciones
SET FOREIGN_KEY_CHECKS = 1;

-- ============================================
-- Verificación de datos insertados
-- ============================================
SELECT
    '✅ Seed data insertado exitosamente' AS mensaje,
    (SELECT COUNT(*) FROM Catalogo) AS total_catalogos,
    (SELECT COUNT(*) FROM Producto) AS total_productos;

-- Mostrar resumen por catálogo
SELECT
    c.Id AS CatalogoId,
    c.Nombre AS Catalogo,
    COUNT(p.Id) AS TotalProductos,
    CONCAT('$', FORMAT(AVG(p.Precio), 2)) AS PrecioPromedio,
    CONCAT('$', FORMAT(MIN(p.Precio), 2)) AS PrecioMinimo,
    CONCAT('$', FORMAT(MAX(p.Precio), 2)) AS PrecioMaximo
FROM Catalogo c
LEFT JOIN Producto p ON c.Id = p.CatalogoId
GROUP BY c.Id, c.Nombre
ORDER BY c.Id;

-- Mostrar todos los productos con su catálogo
SELECT
    p.Id,
    c.Nombre AS Catalogo,
    p.Nombre AS Producto,
    CONCAT('$', FORMAT(p.Precio, 2)) AS Precio,
    p.CantidadDisponible AS Stock,
    DATE_FORMAT(p.FechaCreacion, '%Y-%m-%d %H:%i:%s') AS Fecha
FROM Producto p
INNER JOIN Catalogo c ON p.CatalogoId = c.Id
ORDER BY c.Id, p.Nombre;
