-- ============================================
-- Script: 03-stored-procedures.sql
-- Descripción: Stored Procedures para operaciones CRUD de Productos
-- Patrón: Database First con lógica de negocio en DB
-- Autor: Nico (Persona 1 - DB & Infrastructure Lead)
-- ============================================

USE catalogo_db;

-- Cambiar delimitador para crear SPs
DELIMITER //

-- ============================================
-- SP: sp_GetAllProductos
-- Descripción: Obtiene todos los productos activos con información del catálogo
-- Parámetros: Ninguno
-- Retorna: Lista de productos
-- ============================================
DROP PROCEDURE IF EXISTS sp_GetAllProductos//

CREATE PROCEDURE sp_GetAllProductos()
BEGIN
    SELECT
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.CantidadDisponible,
        p.FechaCreacion,
        p.FechaActualizacion,
        p.CatalogoId,
        c.Nombre AS CatalogoNombre,
        p.Activo
    FROM Producto p
    INNER JOIN Catalogo c ON p.CatalogoId = c.Id
    WHERE p.Activo = TRUE
    ORDER BY p.FechaCreacion DESC;
END//

-- ============================================
-- SP: sp_GetProductoById
-- Descripción: Obtiene un producto específico por su ID
-- Parámetros:
--   @p_ProductoId INT - ID del producto a buscar
-- Retorna: Producto encontrado o NULL
-- ============================================
DROP PROCEDURE IF EXISTS sp_GetProductoById//

CREATE PROCEDURE sp_GetProductoById(
    IN p_ProductoId INT
)
BEGIN
    SELECT
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.CantidadDisponible,
        p.FechaCreacion,
        p.FechaActualizacion,
        p.CatalogoId,
        c.Nombre AS CatalogoNombre,
        p.Activo
    FROM Producto p
    INNER JOIN Catalogo c ON p.CatalogoId = c.Id
    WHERE p.Id = p_ProductoId;
END//

-- ============================================
-- SP: sp_CreateProducto
-- Descripción: Crea un nuevo producto
-- Parámetros:
--   @p_Nombre VARCHAR(200)
--   @p_Descripcion VARCHAR(500)
--   @p_Precio DECIMAL(18,2)
--   @p_CantidadDisponible INT
--   @p_CatalogoId INT
-- Retorna: ID del producto creado
-- ============================================
DROP PROCEDURE IF EXISTS sp_CreateProducto//

CREATE PROCEDURE sp_CreateProducto(
    IN p_Nombre VARCHAR(200),
    IN p_Descripcion VARCHAR(500),
    IN p_Precio DECIMAL(18,2),
    IN p_CantidadDisponible INT,
    IN p_CatalogoId INT,
    OUT p_ProductoId INT
)
BEGIN
    DECLARE v_CatalogoExiste INT;

    -- Validar que el catálogo existe y está activo
    SELECT COUNT(*) INTO v_CatalogoExiste
    FROM Catalogo
    WHERE Id = p_CatalogoId AND Activo = TRUE;

    IF v_CatalogoExiste = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El catálogo especificado no existe o está inactivo';
    END IF;

    -- Validar precio positivo
    IF p_Precio < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El precio debe ser mayor o igual a 0';
    END IF;

    -- Validar cantidad positiva
    IF p_CantidadDisponible < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La cantidad disponible debe ser mayor o igual a 0';
    END IF;

    -- Insertar el producto
    INSERT INTO Producto (
        Nombre,
        Descripcion,
        Precio,
        CantidadDisponible,
        CatalogoId,
        FechaCreacion,
        Activo
    ) VALUES (
        p_Nombre,
        p_Descripcion,
        p_Precio,
        p_CantidadDisponible,
        p_CatalogoId,
        NOW(),
        TRUE
    );

    -- Retornar el ID generado
    SET p_ProductoId = LAST_INSERT_ID();

    -- Retornar el producto creado
    SELECT
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.CantidadDisponible,
        p.FechaCreacion,
        p.FechaActualizacion,
        p.CatalogoId,
        c.Nombre AS CatalogoNombre,
        p.Activo
    FROM Producto p
    INNER JOIN Catalogo c ON p.CatalogoId = c.Id
    WHERE p.Id = p_ProductoId;
END//

-- ============================================
-- SP: sp_UpdateProducto
-- Descripción: Actualiza un producto existente
-- Parámetros:
--   @p_ProductoId INT
--   @p_Nombre VARCHAR(200)
--   @p_Descripcion VARCHAR(500)
--   @p_Precio DECIMAL(18,2)
--   @p_CantidadDisponible INT
-- Retorna: Producto actualizado
-- ============================================
DROP PROCEDURE IF EXISTS sp_UpdateProducto//

CREATE PROCEDURE sp_UpdateProducto(
    IN p_ProductoId INT,
    IN p_Nombre VARCHAR(200),
    IN p_Descripcion VARCHAR(500),
    IN p_Precio DECIMAL(18,2),
    IN p_CantidadDisponible INT
)
BEGIN
    DECLARE v_ProductoExiste INT;

    -- Validar que el producto existe
    SELECT COUNT(*) INTO v_ProductoExiste
    FROM Producto
    WHERE Id = p_ProductoId AND Activo = TRUE;

    IF v_ProductoExiste = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El producto no existe o está inactivo';
    END IF;

    -- Validar precio positivo
    IF p_Precio < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El precio debe ser mayor o igual a 0';
    END IF;

    -- Validar cantidad positiva
    IF p_CantidadDisponible < 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'La cantidad disponible debe ser mayor o igual a 0';
    END IF;

    -- Actualizar el producto
    UPDATE Producto
    SET
        Nombre = p_Nombre,
        Descripcion = p_Descripcion,
        Precio = p_Precio,
        CantidadDisponible = p_CantidadDisponible,
        FechaActualizacion = NOW()
    WHERE Id = p_ProductoId;

    -- Retornar el producto actualizado
    SELECT
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.CantidadDisponible,
        p.FechaCreacion,
        p.FechaActualizacion,
        p.CatalogoId,
        c.Nombre AS CatalogoNombre,
        p.Activo
    FROM Producto p
    INNER JOIN Catalogo c ON p.CatalogoId = c.Id
    WHERE p.Id = p_ProductoId;
END//

-- ============================================
-- SP: sp_DeleteProducto (Soft Delete)
-- Descripción: Elimina lógicamente un producto (marca como inactivo)
-- Parámetros:
--   @p_ProductoId INT
-- Retorna: Número de filas afectadas
-- ============================================
DROP PROCEDURE IF EXISTS sp_DeleteProducto//

CREATE PROCEDURE sp_DeleteProducto(
    IN p_ProductoId INT
)
BEGIN
    DECLARE v_ProductoExiste INT;

    -- Validar que el producto existe
    SELECT COUNT(*) INTO v_ProductoExiste
    FROM Producto
    WHERE Id = p_ProductoId AND Activo = TRUE;

    IF v_ProductoExiste = 0 THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'El producto no existe o ya está inactivo';
    END IF;

    -- Marcar como inactivo (soft delete)
    UPDATE Producto
    SET
        Activo = FALSE,
        FechaActualizacion = NOW()
    WHERE Id = p_ProductoId;

    -- Retornar confirmación
    SELECT
        p_ProductoId AS Id,
        'Producto eliminado exitosamente' AS Mensaje,
        ROW_COUNT() AS FilasAfectadas;
END//

-- ============================================
-- SP: sp_GetProductosByCatalogo
-- Descripción: Obtiene todos los productos de un catálogo específico
-- Parámetros:
--   @p_CatalogoId INT
-- Retorna: Lista de productos del catálogo
-- ============================================
DROP PROCEDURE IF EXISTS sp_GetProductosByCatalogo//

CREATE PROCEDURE sp_GetProductosByCatalogo(
    IN p_CatalogoId INT
)
BEGIN
    SELECT
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.CantidadDisponible,
        p.FechaCreacion,
        p.FechaActualizacion,
        p.CatalogoId,
        c.Nombre AS CatalogoNombre,
        p.Activo
    FROM Producto p
    INNER JOIN Catalogo c ON p.CatalogoId = c.Id
    WHERE p.CatalogoId = p_CatalogoId
      AND p.Activo = TRUE
    ORDER BY p.Nombre ASC;
END//

-- ============================================
-- SP: sp_BuscarProductos
-- Descripción: Busca productos por nombre (búsqueda parcial)
-- Parámetros:
--   @p_Busqueda VARCHAR(200)
-- Retorna: Lista de productos que coinciden
-- ============================================
DROP PROCEDURE IF EXISTS sp_BuscarProductos//

CREATE PROCEDURE sp_BuscarProductos(
    IN p_Busqueda VARCHAR(200)
)
BEGIN
    SELECT
        p.Id,
        p.Nombre,
        p.Descripcion,
        p.Precio,
        p.CantidadDisponible,
        p.FechaCreacion,
        p.FechaActualizacion,
        p.CatalogoId,
        c.Nombre AS CatalogoNombre,
        p.Activo
    FROM Producto p
    INNER JOIN Catalogo c ON p.CatalogoId = c.Id
    WHERE p.Activo = TRUE
      AND (p.Nombre LIKE CONCAT('%', p_Busqueda, '%')
           OR p.Descripcion LIKE CONCAT('%', p_Busqueda, '%'))
    ORDER BY p.Nombre ASC;
END//

-- Restaurar delimitador
DELIMITER ;

-- ============================================
-- Verificación de Stored Procedures
-- ============================================
SELECT
    'Stored Procedures creados exitosamente' AS mensaje,
    COUNT(*) AS total_sps
FROM information_schema.routines
WHERE routine_schema = 'catalogo_db'
  AND routine_type = 'PROCEDURE'
  AND routine_name LIKE 'sp_%';

-- Listar todos los SPs creados
SELECT
    routine_name AS stored_procedure,
    created AS fecha_creacion
FROM information_schema.routines
WHERE routine_schema = 'catalogo_db'
  AND routine_type = 'PROCEDURE'
  AND routine_name LIKE 'sp_%'
ORDER BY routine_name;
