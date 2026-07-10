# Documentacion tecnica: Core, API y React

## Resumen

El sistema queda separado en dos partes principales:

- **API/Core backend**: ASP.NET Core, ubicado en `Proyecto_EmilEncalada.Server`.
- **Frontend React**: Vite + React, ubicado en `core_degradacion_react`.

El archivo ZIP `core_degradacion_react_clean.zip` corresponde al frontend React. No contiene la API del Core. La API del Core esta implementada en el backend .NET.

## Donde esta implementada la API del Core

La API principal del Core esta en:

- `Proyecto_EmilEncalada.Server/Controllers/CoreController.cs`

Endpoint principal:

```http
GET /api/Core/degradacion/{id}
```

En local se ejecuta en:

```text
http://localhost:5086/api/Core/degradacion/1
```

Swagger local:

```text
http://localhost:5086/swagger
```

## Flujo del Core

1. React o Swagger llama a `GET /api/Core/degradacion/{id}`.
2. `CoreController` recibe la peticion HTTP.
3. `CoreController` delega en `DegradacionService`.
4. `DegradacionService` pide el equipo y sus reparaciones al repositorio.
5. `EquipoRepository` consulta la base de datos mediante Entity Framework.
6. `DegradacionService` envia los datos a la estrategia de calculo.
7. `CalculoDegradacionPorReglasStrategy` calcula degradacion, valor actual, nivel y recomendacion.
8. El resultado vuelve al cliente como JSON.

## Patrones y principios aplicados

### Strategy

Implementado en:

- `Proyecto_EmilEncalada.Server/Interfaces/ICalculoDegradacionStrategy.cs`
- `Proyecto_EmilEncalada.Server/Strategies/CalculoDegradacionPorReglasStrategy.cs`

Permite cambiar la formula de degradacion sin modificar el controlador ni el servicio.

### Repository

Implementado en:

- `Proyecto_EmilEncalada.Server/Interfaces/IEquipoRepository.cs`
- `Proyecto_EmilEncalada.Server/Repositories/EquipoRepository.cs`

Encapsula el acceso a datos de equipos y reparaciones.

### Service Layer

Implementado en:

- `Proyecto_EmilEncalada.Server/Services/DegradacionService.cs`

Coordina el repositorio y la estrategia.

### DTO

Implementado en:

- `Proyecto_EmilEncalada.Server/Dtos/ResultadoDegradacionDto.cs`

Define la respuesta JSON del Core.

### SOLID

- **SRP**: cada clase tiene una responsabilidad concreta.
- **OCP**: se pueden agregar nuevas estrategias sin cambiar el controlador.
- **DIP**: `DegradacionService` depende de interfaces, no de clases concretas.

## URLs locales

Backend/API:

```text
http://localhost:5086
```

Swagger:

```text
http://localhost:5086/swagger
```

React:

```text
http://localhost:5174
```

Gestion de datos en React:

```text
http://localhost:5174/gestion
```

## Endpoints principales

```http
GET    /api/Departamentos
POST   /api/Departamentos
GET    /api/Equipos
POST   /api/Equipos
GET    /api/Reparaciones
POST   /api/Reparaciones
GET    /api/Core/degradacion/{id}
```

Para crear, actualizar o eliminar datos, el frontend envia:

```http
X-Rol: Admin
```

## React integrado

El frontend React esta en:

```text
core_degradacion_react
```

Consume la API mediante:

```text
core_degradacion_react/src/api/axiosConfig.js
```

URL local del API:

```text
VITE_API_URL=http://localhost:5086
```

Pantallas:

- `/`: inicio.
- `/degradacion`: calculo individual por ID.
- `/analisis`: escenarios avanzados.
- `/gestion`: agregar, listar y evaluar datos.

## Base de datos

El backend esta configurado para SQL Server online en:

```text
Proyecto_EmilEncalada.Server/appsettings.json
```

Al iniciar, el backend ejecuta:

```text
Proyecto_EmilEncalada.Server/Data/DatabaseBootstrapper.cs
```

Esto crea o completa las tablas necesarias:

- `dbo.Usuarios`
- `dbo.Departamentos`
- `dbo.Equipos`
- `dbo.Reparaciones`
- `dbo.Productos`

## Despliegue

Para desplegar correctamente hay dos piezas:

1. **Backend/API**: debe desplegarse como ASP.NET Core en un hosting compatible con .NET.
2. **Frontend React**: debe desplegarse como sitio estatico de Vite.

En produccion, React no debe usar `localhost`. Debe usar la URL publica del backend:

```text
VITE_API_URL=https://URL-PUBLICA-DE-TU-API
```

Si solo se despliega React sin desplegar la API, la pantalla cargara pero no podra listar, agregar ni evaluar datos.

## Comandos locales

Backend:

```powershell
dotnet run --project Proyecto_EmilEncalada.Server\Proyecto_EmilEncalada.Server.csproj --launch-profile https
```

React:

```powershell
cd core_degradacion_react
npm run dev -- --host 0.0.0.0
```

Build React:

```powershell
cd core_degradacion_react
npm run build
```
