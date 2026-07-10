using Microsoft.EntityFrameworkCore;

namespace Proyecto_EmilEncalada.Server.Data;

public static class DatabaseBootstrapper
{
    public static async Task EnsureReadyAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await context.Database.OpenConnectionAsync();

        try
        {
            foreach (var command in Commands)
            {
                await context.Database.ExecuteSqlRawAsync(command);
            }
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }

    private static readonly string[] Commands =
    [
        """
        IF OBJECT_ID('dbo.Departamentos', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Departamentos (
                Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Departamentos PRIMARY KEY,
                Nombre NVARCHAR(MAX) NOT NULL,
                PresupuestoAsignado DECIMAL(10,2) NOT NULL
            );
        END
        """,
        """
        IF NOT EXISTS (SELECT 1 FROM dbo.Departamentos)
        BEGIN
            INSERT INTO dbo.Departamentos (Nombre, PresupuestoAsignado)
            VALUES ('General', 10000.00), ('Sistemas', 25000.00);
        END
        """,
        """
        IF OBJECT_ID('dbo.Productos', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Productos (
                Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Productos PRIMARY KEY,
                Nombre NVARCHAR(MAX) NOT NULL,
                Precio DECIMAL(10,2) NOT NULL,
                Stock INT NOT NULL
            );
        END
        """,
        """
        IF OBJECT_ID('dbo.Usuarios', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Usuarios (
                Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Usuarios PRIMARY KEY,
                Nombre NVARCHAR(MAX) NOT NULL,
                Correo NVARCHAR(MAX) NOT NULL,
                Clave NVARCHAR(MAX) NOT NULL,
                Rol NVARCHAR(MAX) NOT NULL CONSTRAINT DF_Usuarios_Rol DEFAULT 'Usuario',
                Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1,
                FechaCreacion DATE NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT CONVERT(date, GETDATE())
            );
        END
        """,
        """
        IF COL_LENGTH('dbo.Usuarios', 'Rol') IS NULL
            ALTER TABLE dbo.Usuarios ADD Rol NVARCHAR(MAX) NOT NULL CONSTRAINT DF_Usuarios_Rol DEFAULT 'Usuario';
        """,
        """
        IF COL_LENGTH('dbo.Usuarios', 'Activo') IS NULL
            ALTER TABLE dbo.Usuarios ADD Activo BIT NOT NULL CONSTRAINT DF_Usuarios_Activo DEFAULT 1;
        """,
        """
        IF COL_LENGTH('dbo.Usuarios', 'FechaCreacion') IS NULL
            ALTER TABLE dbo.Usuarios ADD FechaCreacion DATE NOT NULL CONSTRAINT DF_Usuarios_FechaCreacion DEFAULT CONVERT(date, GETDATE());
        """,
        """
        IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE Correo = 'admin@gmail.com')
        BEGIN
            INSERT INTO dbo.Usuarios (Nombre, Correo, Clave, Rol, Activo, FechaCreacion)
            VALUES ('Administrador', 'admin@gmail.com', '12345', 'Admin', 1, CONVERT(date, GETDATE()));
        END
        """,
        """
        IF OBJECT_ID('dbo.Equipos', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Equipos (
                Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Equipos PRIMARY KEY,
                Nombre NVARCHAR(100) NOT NULL,
                Tipo NVARCHAR(50) NOT NULL,
                Marca NVARCHAR(50) NOT NULL,
                FechaCompra DATE NOT NULL,
                CostoInicial DECIMAL(10,2) NOT NULL,
                VidaUtilMeses INT NOT NULL,
                Estado NVARCHAR(50) NOT NULL,
                DepartamentoId INT NOT NULL
            );
        END
        """,
        """
        IF COL_LENGTH('dbo.Equipos', 'DepartamentoId') IS NULL
            ALTER TABLE dbo.Equipos ADD DepartamentoId INT NULL;
        """,
        """
        UPDATE dbo.Equipos
        SET DepartamentoId = (SELECT TOP 1 Id FROM dbo.Departamentos ORDER BY Id)
        WHERE DepartamentoId IS NULL;
        """,
        """
        IF COL_LENGTH('dbo.Equipos', 'DepartamentoId') IS NOT NULL
           AND EXISTS (
                SELECT 1
                FROM sys.columns
                WHERE object_id = OBJECT_ID('dbo.Equipos')
                  AND name = 'DepartamentoId'
                  AND is_nullable = 1
           )
            ALTER TABLE dbo.Equipos ALTER COLUMN DepartamentoId INT NOT NULL;
        """,
        """
        IF OBJECT_ID('dbo.FK_Equipos_Departamentos_DepartamentoId', 'F') IS NULL
        BEGIN
            ALTER TABLE dbo.Equipos
            ADD CONSTRAINT FK_Equipos_Departamentos_DepartamentoId
            FOREIGN KEY (DepartamentoId) REFERENCES dbo.Departamentos(Id);
        END
        """,
        """
        IF OBJECT_ID('dbo.Reparaciones', 'U') IS NULL
        BEGIN
            CREATE TABLE dbo.Reparaciones (
                Id INT IDENTITY(1,1) NOT NULL CONSTRAINT PK_Reparaciones PRIMARY KEY,
                FechaReparacion DATE NOT NULL,
                Descripcion NVARCHAR(250) NOT NULL,
                Costo DECIMAL(10,2) NOT NULL,
                EquipoId INT NOT NULL
            );
        END
        """,
        """
        IF OBJECT_ID('dbo.FK_Reparaciones_Equipos_EquipoId', 'F') IS NULL
        BEGIN
            ALTER TABLE dbo.Reparaciones
            ADD CONSTRAINT FK_Reparaciones_Equipos_EquipoId
            FOREIGN KEY (EquipoId) REFERENCES dbo.Equipos(Id) ON DELETE CASCADE;
        END
        """,
        """
        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Equipos_DepartamentoId' AND object_id = OBJECT_ID('dbo.Equipos'))
            CREATE INDEX IX_Equipos_DepartamentoId ON dbo.Equipos(DepartamentoId);
        """,
        """
        IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Reparaciones_EquipoId' AND object_id = OBJECT_ID('dbo.Reparaciones'))
            CREATE INDEX IX_Reparaciones_EquipoId ON dbo.Reparaciones(EquipoId);
        """
    ];
}
