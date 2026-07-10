import { Link } from "react-router-dom";

function Inicio() {
  return (
    <div>
      <section className="hero card shadow mb-4">
        <div className="card-body p-4 p-md-5">
          <div className="row align-items-center g-4">
            <div className="col-md-8">
              <span className="badge bg-primary mb-3">React + API REST</span>
              <h1 className="display-6 fw-bold">
                Sistema de analisis de degradacion de equipos TI
              </h1>
              <p className="lead mt-3">
                Esta pagina consume el Core principal desplegado en Render. El
                calculo se ejecuta en la API y React muestra resultados,
                indicadores, escenarios y recomendaciones.
              </p>
              <div className="d-flex gap-2 flex-wrap mt-4">
                <Link className="btn btn-primary" to="/degradacion">
                  Calcular equipo
                </Link>
                <Link className="btn btn-outline-primary" to="/analisis">
                  Ver analisis avanzado
                </Link>
              </div>
            </div>
            <div className="col-md-4">
              <div className="metric-card dark-card text-center">
                <h6>Endpoint principal</h6>
                <p className="mb-1">/api/Core/degradacion/id</p>
                <small>Datos procesados desde la API</small>
              </div>
            </div>
          </div>
        </div>
      </section>

      <div className="row g-3">
        <div className="col-md-4">
          <div className="card shadow h-100">
            <div className="card-body">
              <h5>API propia</h5>
              <p>
                React consume una API ASP.NET Core conectada al Core de
                degradacion del proyecto principal.
              </p>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card shadow h-100">
            <div className="card-body">
              <h5>No es solo listado</h5>
              <p>
                Incluye calculo individual, indice de riesgo, proyecciones y
                comparacion de escenarios.
              </p>
            </div>
          </div>
        </div>
        <div className="col-md-4">
          <div className="card shadow h-100">
            <div className="card-body">
              <h5>Defensa tecnica</h5>
              <p>
                La logica principal queda en backend. React solo consume,
                analiza y presenta los datos obtenidos.
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default Inicio;
