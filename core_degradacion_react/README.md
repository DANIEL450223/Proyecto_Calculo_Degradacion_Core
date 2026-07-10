# Core Degradacion TI - React + API

Proyecto frontend desarrollado en React para consumir el Core principal de degradacion de equipos TI.

## Objetivo

La aplicacion demuestra el consumo de una API REST desde React. La logica principal del calculo no esta en React, sino en el backend ASP.NET Core desplegado en Render.

## API consumida

Backend Render:

```text
https://proyecto-calculo-degradacion-core.onrender.com
```

Endpoint principal:

```text
/api/Core/degradacion/{id}
```

Ejemplo:

```text
https://proyecto-calculo-degradacion-core.onrender.com/api/Core/degradacion/1
```

## Funciones implementadas

- Vista de inicio explicando la arquitectura.
- Calculo individual de degradacion por ID de equipo.
- Consumo de API con Axios.
- Visualizacion de porcentaje de degradacion.
- Indice de riesgo calculado en React a partir de datos de la API.
- Matriz de escenarios a 12 meses.
- Comparacion entre mantener, mantenimiento preventivo y reemplazo planificado.
- Conclusion tecnica para apoyar la toma de decisiones.

## Tecnologias

- React
- Vite
- Axios
- Bootstrap
- ASP.NET Core API externa
- Render

## Como ejecutar

```bash
npm install
npm run dev
```

Abrir:

```text
http://localhost:5173
```

## Configuracion de API

El archivo `.env.local` contiene:

```env
VITE_API_URL=https://proyecto-calculo-degradacion-core.onrender.com
```

No se debe agregar `/swagger` en esta variable.

## Que mostrar en el video

1. Abrir Swagger del backend.
2. Mostrar el endpoint `/api/Core/degradacion/1`.
3. Abrir React.
4. Mostrar calculo individual.
5. Mostrar analisis avanzado.
6. Explicar que React consume la API con Axios y que el Core esta en el backend.

## Explicacion corta

Este proyecto usa una arquitectura desacoplada. El backend contiene el Core de degradacion y la API REST. El frontend React consume esa API mediante Axios, muestra los datos y genera analisis adicional para apoyar la toma de decisiones.
