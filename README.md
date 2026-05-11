# CRUD + Login + Administración MVC

Proyecto fusionado con:
- Login y registro de usuarios
- CRUD de productos
- CRUD de departamentos
- CRUD de equipos
- CRUD de reparaciones
- Validaciones backend
- Dropdowns para relaciones

## Pasos
1. Abrir `Proyecto_EmilEncalada.sln`
2. Verificar `appsettings.json`
3. Ejecutar `npm install` en `proyecto_emilencalada.client`
4. Ejecutar migraciones:
   - Add-Migration AdminCoreFinal
   - Update-Database
5. Correr backend y frontend

## Validaciones backend
- Presupuesto asignado mayor a cero
- Fecha de compra no futura
- Costo inicial mayor a cero
- Fecha de reparación no futura
- Costo de reparación mayor a cero
