-- ======================================================
-- RESET COMPLETO PARA SQL SERVER ONLINE: db59263
-- Compatible con el backend Proyecto_EmilEncalada.Server
--
-- IMPORTANTE:
-- Este script elimina y vuelve a crear las tablas del proyecto.
-- Ejecutalo en la base db59263, no en master.
-- ======================================================

USE [db59263];
GO

-- ======================================================
-- LIMPIEZA
-- ======================================================

IF OBJECT_ID('dbo.Reparaciones', 'U') IS NOT NULL DROP TABLE dbo.Reparaciones;
IF OBJECT_ID('dbo.Equipos', 'U') IS NOT NULL DROP TABLE dbo.Equipos;
IF OBJECT_ID('dbo.Departamentos', 'U') IS NOT NULL DROP TABLE dbo.Departamentos;
IF OBJECT_ID('dbo.Productos', 'U') IS NOT NULL DROP TABLE dbo.Productos;
IF OBJECT_ID('dbo.Usuarios', 'U') IS NOT NULL DROP TABLE dbo.Usuarios;
GO

-- ======================================================
-- TABLAS QUE USA ENTITY FRAMEWORK
-- ======================================================

CREATE TABLE dbo.Departamentos (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Departamentos PRIMARY KEY,
    Nombre NVARCHAR(MAX) NOT NULL,
    PresupuestoAsignado DECIMAL(10,2) NOT NULL
);
GO

CREATE TABLE dbo.Productos (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Productos PRIMARY KEY,
    Nombre NVARCHAR(MAX) NOT NULL,
    Precio DECIMAL(10,2) NOT NULL,
    Stock INT NOT NULL
);
GO

CREATE TABLE dbo.Usuarios (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
    Nombre NVARCHAR(MAX) NOT NULL,
    Correo NVARCHAR(MAX) NOT NULL,
    Clave NVARCHAR(MAX) NOT NULL,
    Rol NVARCHAR(MAX) NOT NULL CONSTRAINT DF_Usuarios_Rol DEFAULT 'Usuario',
    Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1,
    FechaCreacion DATE NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT CONVERT(date, GETDATE())
);
GO

CREATE TABLE dbo.Equipos (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Equipos PRIMARY KEY,
    Nombre NVARCHAR(100) NOT NULL,
    Tipo NVARCHAR(50) NOT NULL,
    Marca NVARCHAR(50) NOT NULL,
    FechaCompra DATE NOT NULL,
    CostoInicial DECIMAL(10,2) NOT NULL,
    VidaUtilMeses INT NOT NULL,
    Estado NVARCHAR(50) NOT NULL,
    DepartamentoId INT NOT NULL,
    CONSTRAINT FK_Equipos_Departamentos_DepartamentoId
        FOREIGN KEY (DepartamentoId)
        REFERENCES dbo.Departamentos(Id)
        ON DELETE NO ACTION
);
GO

CREATE TABLE dbo.Reparaciones (
    Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Reparaciones PRIMARY KEY,
    FechaReparacion DATE NOT NULL,
    Descripcion NVARCHAR(250) NOT NULL,
    Costo DECIMAL(10,2) NOT NULL,
    EquipoId INT NOT NULL,
    CONSTRAINT FK_Reparaciones_Equipos_EquipoId
        FOREIGN KEY (EquipoId)
        REFERENCES dbo.Equipos(Id)
        ON DELETE CASCADE
);
GO

CREATE INDEX IX_Equipos_DepartamentoId ON dbo.Equipos(DepartamentoId);
GO

CREATE INDEX IX_Reparaciones_EquipoId ON dbo.Reparaciones(EquipoId);
GO

-- ======================================================
-- USUARIOS PARA LOGIN
-- ======================================================

INSERT INTO dbo.Usuarios (Nombre, Correo, Clave, Rol, Activo, FechaCreacion)
VALUES
('Administrador', 'admin@gmail.com', '12345', 'Admin', 1, CONVERT(date, GETDATE())),
('Usuario Prueba', 'usuario@gmail.com', '12345', 'Usuario', 1, CONVERT(date, GETDATE()));
GO

-- ======================================================
-- DATOS DE PRUEBA
-- ======================================================

INSERT INTO dbo.Departamentos (Nombre, PresupuestoAsignado)
VALUES
('Administracion', 10000.00),
('Sistemas', 25000.00),
('Contabilidad', 12000.00),
('Aulas', 18000.00);
GO

INSERT INTO dbo.Equipos
(Nombre, Tipo, Marca, FechaCompra, CostoInicial, VidaUtilMeses, Estado, DepartamentoId)
VALUES
('Laptop Administrativa', 'Laptop', 'Lenovo', '2022-01-15', 850.00, 60, 'Regular', 1),
('Servidor Principal', 'Servidor', 'Dell', '2020-06-10', 3500.00, 84, 'Activo', 2),
('Impresora Oficina', 'Impresora', 'HP', '2021-03-20', 420.00, 48, 'Deteriorado', 1),
('PC Contabilidad', 'Escritorio', 'HP', '2019-11-05', 950.00, 72, 'Regular', 3),
('Televisor Sala Espera', 'Televisor', 'Samsung', '2023-07-01', 600.00, 72, 'Activo', 1),
('Proyector Aula 1', 'Electronico', 'Epson', '2021-09-10', 750.00, 60, 'Regular', 4),
('Router Principal', 'Electronico', 'Cisco', '2022-05-22', 300.00, 72, 'Activo', 2);
GO

INSERT INTO dbo.Reparaciones
(EquipoId, FechaReparacion, Descripcion, Costo)
VALUES
(1, '2023-04-12', 'Cambio de teclado', 45.00),
(1, '2024-02-18', 'Mantenimiento general', 35.00),
(2, '2022-09-10', 'Cambio de fuente de poder', 180.00),
(2, '2024-03-14', 'Mantenimiento preventivo', 120.00),
(3, '2022-08-05', 'Cambio de rodillos', 60.00),
(3, '2023-01-15', 'Reparacion de bandeja', 40.00),
(3, '2024-05-22', 'Limpieza interna', 30.00),
(4, '2021-10-11', 'Cambio de disco duro', 75.00),
(4, '2023-06-19', 'Cambio de memoria RAM', 65.00),
(5, '2024-01-20', 'Mantenimiento de pantalla', 25.00),
(6, '2023-11-12', 'Cambio de lampara', 95.00),
(7, '2024-04-01', 'Actualizacion de firmware', 20.00);
GO

INSERT INTO dbo.Productos (Nombre, Precio, Stock)
VALUES
('Mouse', 12.50, 20),
('Teclado', 18.00, 15),
('Cable HDMI', 7.50, 30);
GO

-- ======================================================
-- VERIFICACION DEL LOGIN
-- Si este SELECT devuelve 1 fila, el login debe funcionar.
-- ======================================================

SELECT Id, Nombre, Correo, Rol, Activo, FechaCreacion
FROM dbo.Usuarios
WHERE Correo = 'admin@gmail.com'
  AND Clave = '12345'
  AND Activo = 1;
GO
