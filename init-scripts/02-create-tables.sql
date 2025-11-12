-- ============================================
-- Script: 02-create-tables.sql
-- Descripción: Creación de tablas para el catálogo de productos
-- Patrón: Database First
-- Autor: Nico (Persona 1 - DB & Infrastructure Lead)
-- ============================================

USE catalogo_db;

-- Deshabilitar checks temporalmente para creación limpia
SET FOREIGN_KEY_CHECKS = 0;

-- ============================================
-- Tabla: Catalogo
-- Descripción: Catálogo principal que agrupa productos
-- ============================================
DROP TABLE IF EXISTS `Catalogo`;

CREATE TABLE `Catalogo` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(200) NOT NULL,
    `Descripcion` VARCHAR(500) NULL,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `Activo` BOOLEAN NOT NULL DEFAULT TRUE,

    INDEX `idx_catalogo_activo` (`Activo`),
    INDEX `idx_catalogo_nombre` (`Nombre`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- ============================================
-- Tabla: Producto
-- Descripción: Productos individuales dentro de un catálogo
-- ============================================
DROP TABLE IF EXISTS `Producto`;

CREATE TABLE `Producto` (
    `Id` INT AUTO_INCREMENT PRIMARY KEY,
    `Nombre` VARCHAR(200) NOT NULL,
    `Descripcion` VARCHAR(500) NOT NULL,
    `Precio` DECIMAL(18, 2) NOT NULL,
    `CantidadDisponible` INT NOT NULL DEFAULT 0,
    `FechaCreacion` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    `FechaActualizacion` DATETIME NULL ON UPDATE CURRENT_TIMESTAMP,
    `CatalogoId` INT NOT NULL,
    `Activo` BOOLEAN NOT NULL DEFAULT TRUE,

    -- Foreign Key
    CONSTRAINT `fk_producto_catalogo`
        FOREIGN KEY (`CatalogoId`)
        REFERENCES `Catalogo`(`Id`)
        ON DELETE CASCADE
        ON UPDATE CASCADE,

    -- Constraints
    CONSTRAINT `chk_precio_positivo` CHECK (`Precio` >= 0),
    CONSTRAINT `chk_cantidad_positiva` CHECK (`CantidadDisponible` >= 0),

    -- Indexes
    INDEX `idx_producto_catalogo` (`CatalogoId`),
    INDEX `idx_producto_nombre` (`Nombre`),
    INDEX `idx_producto_precio` (`Precio`),
    INDEX `idx_producto_activo` (`Activo`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- Rehabilitar checks
SET FOREIGN_KEY_CHECKS = 1;

-- ============================================
-- Verificación
-- ============================================
SELECT
    'Tablas creadas exitosamente' AS mensaje,
    (SELECT COUNT(*) FROM information_schema.tables
     WHERE table_schema = 'catalogo_db' AND table_name = 'Catalogo') AS catalogo_existe,
    (SELECT COUNT(*) FROM information_schema.tables
     WHERE table_schema = 'catalogo_db' AND table_name = 'Producto') AS producto_existe;

-- Mostrar estructura de las tablas
SHOW CREATE TABLE Catalogo;
SHOW CREATE TABLE Producto;
