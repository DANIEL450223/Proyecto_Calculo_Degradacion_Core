# Proyecto Cálculo de Degradación Core

Proyecto académico desarrollado con **ASP.NET Core Web API** y **Vue.js**, enfocado en calcular la degradación de equipos tecnológicos aplicando buenas prácticas de programación, principios SOLID y patrones de diseño.

## Autor

**Emil Daniel Encalada Carrion**

## Descripción del proyecto

El sistema permite registrar y consultar información relacionada con equipos tecnológicos, departamentos, reparaciones y usuarios.
El módulo principal del proyecto es el **Core de degradación**, encargado de calcular el nivel de degradación de un equipo considerando factores como:

* Tipo de equipo.
* Estado actual.
* Fecha de compra.
* Vida útil.
* Costo inicial.
* Cantidad de reparaciones.
* Costo acumulado de reparaciones.

El resultado permite conocer el porcentaje de degradación, el valor actual estimado del equipo y una recomendación sobre su uso, mantenimiento o reemplazo.

## Tecnologías utilizadas

* ASP.NET Core Web API (.NET 8)
* Vue.js
* PostgreSQL
* Entity Framework Core
* Render
* Vercel
* GitHub

## Funcionalidad principal del Core

Endpoint principal:

```text
GET /api/Core/degradacion/{id}
```

Ejemplo:

```text
https://proyecto-calculo-degradacion-core.onrender.com/api/Core/degradacion/1
```

Este endpoint devuelve información como:

* Equipo
* Tipo
* Marca
* Estado
* Antigüedad en meses
* Vida útil
* Cantidad de reparaciones
* Costo de reparaciones
* Porcentaje de degradación
* Nivel de degradación
* Valor actual estimado
* Recomendación

## Mejores prácticas aplicadas

En esta versión se aplicaron mejores prácticas al Core del proyecto, usando **2 principios SOLID** y **2 patrones de diseño**.

## Principios SOLID aplicados

### 1. SRP - Single Responsibility Principle

Se aplicó el principio de responsabilidad única separando las funciones del sistema en diferentes clases:

* `CoreController`: recibe la petición HTTP.
* `DegradacionService`: coordina el proceso de cálculo.
* `EquipoRepository`: obtiene los datos de equipos y reparaciones.
* `CalculoDegradacionPorReglasStrategy`: contiene la lógica del cálculo de degradación.

Con esto, cada clase tiene una responsabilidad clara y el código es más fácil de mantener.

### 2. DIP - Dependency Inversion Principle

Se aplicó el principio de inversión de dependencias haciendo que `DegradacionService` dependa de interfaces y no directamente de clases concretas.

Interfaces utilizadas:

* `IEquipoRepository`
* `ICalculoDegradacionStrategy`

Esto permite cambiar la fuente de datos o la forma de cálculo sin modificar directamente el servicio principal.

## Patrones de diseño aplicados

### 1. Repository Pattern

Se aplicó mediante:

* `IEquipoRepository`
* `EquipoRepository`

Este patrón separa el acceso a datos de la lógica del negocio.
Gracias a esto, el servicio no necesita saber si los datos vienen de PostgreSQL, Entity Framework o de un respaldo interno.

### 2. Strategy Pattern

Se aplicó mediante:

* `ICalculoDegradacionStrategy`
* `CalculoDegradacionPorReglasStrategy`

Este patrón permite separar la lógica del cálculo de degradación.
Si en el futuro se desea cambiar la fórmula o agregar una nueva forma de cálculo, se puede crear otra estrategia sin modificar el servicio principal.

## Respaldo del Core

También se agregó un respaldo dentro del repositorio para que, si la base de datos no responde temporalmente, el Core pueda seguir funcionando durante la demostración.

Esto permite mantener disponible el endpoint principal:

```text
/api/Core/degradacion/1
```

## Estructura principal de las mejoras

```text
Proyecto_EmilEncalada.Server
│
├── Controllers
│   └── CoreController.cs
│
├── Interfaces
│   ├── IEquipoRepository.cs
│   └── ICalculoDegradacionStrategy.cs
│
├── Repositories
│   └── EquipoRepository.cs
│
├── Services
│   └── DegradacionService.cs
│
├── Strategies
│   └── CalculoDegradacionPorReglasStrategy.cs
│
└── Program.cs
```

## Links de entrega

### Código en GitHub

```text
https://github.com/DANIEL450223/Proyecto_Calculo_Degradacion_Core.git
```

### Proyecto deployado - Frontend Vercel

```text
https://proyecto-calculo-degradacion-core.vercel.app/
```

### Backend Render

```text
https://proyecto-calculo-degradacion-core.onrender.com
```

### Swagger del backend

```text
https://proyecto-calculo-degradacion-core.onrender.com/swagger
```

### Endpoint de prueba del Core

```text
https://proyecto-calculo-degradacion-core.onrender.com/api/Core/degradacion/1
```

### Video explicativo

```text
https://drive.google.com/drive/folders/1zlXlF58e9RMKwFBjxIKxmxCdSen4EEZx?usp=drive_link
```

## Cómo ejecutar localmente

### Backend

```bash
cd Proyecto_EmilEncalada.Server
dotnet restore
dotnet run
```

### Frontend

```bash
cd proyecto_emilencalada.client
npm install
npm run dev
```

## Conclusión

Con estas mejoras, el Core de degradación queda más ordenado, mantenible y preparado para futuros cambios.
La aplicación ahora aplica principios SOLID y patrones de diseño, separando responsabilidades entre controlador, servicio, repositorio y estrategia de cálculo.
