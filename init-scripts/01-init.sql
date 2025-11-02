-- Script de inicialización de MySQL
-- Este archivo se ejecuta automáticamente la primera vez que se crea el contenedor

USE catalogo_db;

-- Configurar zona horaria a hora de Colombia (UTC-5)
SET GLOBAL time_zone = '-05:00';

-- Configurar formato de fecha
SET GLOBAL sql_mode = 'TRADITIONAL';

-- Mensaje de confirmación
SELECT 
    'Base de datos MySQL inicializada correctamente' AS mensaje,
    'Entity Framework creará las tablas y datos de prueba automáticamente' AS nota,
    DATABASE() AS base_datos_actual,
    VERSION() AS version_mysql;
