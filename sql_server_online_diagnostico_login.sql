-- Ejecuta esto conectado a tu base db59263.
-- Sirve para confirmar si el backend puede encontrar la tabla dbo.Usuarios.

SELECT DB_NAME() AS BaseActual;

SELECT
    TABLE_SCHEMA,
    TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_NAME IN ('Usuarios', 'Departamentos', 'Equipos', 'Reparaciones', 'Productos')
ORDER BY TABLE_SCHEMA, TABLE_NAME;

SELECT
    Id,
    Nombre,
    Correo,
    Clave,
    Rol,
    Activo,
    FechaCreacion
FROM dbo.Usuarios;

SELECT
    Id,
    Nombre,
    Correo,
    Rol,
    Activo,
    FechaCreacion
FROM dbo.Usuarios
WHERE Correo = 'admin@gmail.com'
  AND Clave = '12345'
  AND Activo = 1;
