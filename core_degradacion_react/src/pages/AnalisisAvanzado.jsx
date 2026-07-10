import { useState } from "react";
import api from "../api/axiosConfig";

function proyectarDegradacion(base, meses, ajusteMantenimiento) {
  const degradacionBase = Number(base?.degradacionPorcentaje || 0);
  const reparaciones = Number(base?.cantidadReparaciones || 0);
  const crecimientoMensual = 1.15 + reparaciones * 0.18;
  const degradacion = degradacionBase + meses * crecimientoMensual - ajusteMantenimiento;

  return Math.min(100, Math.max(0, degradacion));
}

function calcularValor(base, degradacion) {
  const costoInicial = Number(base?.costoInicial || 0);
  return Math.max(0, costoInicial * (1 - degradacion / 100));
}

function AnalisisAvanzado() {
  const [equipoId, setEquipoId] = useState(1);
  const [resultado, setResultado] = useState(null);
  const [escenarios, setEscenarios] = useState([]);
  const [error, setError] = useState("");
  const [cargando, setCargando] = useState(false);

  const analizar = async (e) => {
    e.preventDefault();
    setError("");
    setResultado(null);
    setEscenarios([]);
    setCargando(true);

    try {
      const response = await api.get(`/api/Core/degradacion/${equipoId}`);
      const data = response.data;

      const nuevosEscenarios = [
        {
          nombre: "Mantener sin accion",
          detalle: "Se mantiene el equipo sin intervencion adicional.",
          meses: 12,
          ajuste: 0,
          costo: 0,
        },
        {
          nombre: "Mantenimiento preventivo",
          detalle: "Se planifica mantenimiento para reducir el riesgo operativo.",
          meses: 12,
          ajuste: 12,
          costo: Number(data.costoInicial || 0) * 0.08,
        },
        {
          nombre: "Reemplazo planificado",
          detalle: "Se recomienda renovar el equipo para evitar fallos criticos.",
          meses: 12,
          ajuste: 35,
          costo: Number(data.costoInicial || 0),
        },
      ].map((escenario) => {
        const degradacionProyectada = proyectarDegradacion(data, escenario.meses, escenario.ajuste);
        const valorProyectado = calcularValor(data, degradacionProyectada);
        const costoTotal = escenario.costo + Number(data.costoReparaciones || 0);
        const prioridad = degradacionProyectada >= 80 ? "Alta" : degradacionProyectada >= 55 ? "Media" : "Baja";

        return {
          ...escenario,
          degradacionProyectada: degradacionProyectada.toFixed(2),
          valorProyectado: valorProyectado.toFixed(2),
          costoTotal: costoTotal.toFixed(2),
          prioridad,
        };
      });

      setResultado(data);
      setEscenarios(nuevosEscenarios);
    } catch {
      setError("No se pudo consultar la API. Prueba con ID 1 o revisa la conexion.");
    } finally {
      setCargando(false);
    }
  };

  const mejorEscenario = escenarios.length > 0
    ? [...escenarios].sort((a, b) => Number(a.degradacionProyectada) - Number(b.degradacionProyectada))[0]
    : null;

  return (
    <div>
      <h2 className="mb-4">Analisis avanzado del Core</h2>

      <div className="card shadow mb-4">
        <div className="card-body">
          <p>
            Esta vista consume el endpoint real del Core y genera una matriz de
            escenarios para apoyar la toma de decisiones. No es solo una lista:
            compara riesgos, costos y valor proyectado.
          </p>

          <form onSubmit={analizar}>
            <div className="row g-3 align-items-end">
              <div className="col-md-8">
                <label className="form-label">ID del equipo a analizar</label>
                <input
                  type="number"
                  className="form-control"
                  value={equipoId}
                  min="1"
                  onChange={(e) => setEquipoId(e.target.value)}
                />
              </div>
              <div className="col-md-4">
                <button className="btn btn-primary w-100" disabled={cargando}>
                  {cargando ? "Analizando..." : "Generar analisis"}
                </button>
              </div>
            </div>
          </form>
        </div>
      </div>

      {error && <div className="alert alert-danger">{error}</div>}

      {resultado && (
        <>
          <div className="row g-3 mb-4">
            <div className="col-md-3">
              <div className="metric-card shadow">
                <span>Equipo base</span>
                <h4>{resultado.equipo}</h4>
              </div>
            </div>
            <div className="col-md-3">
              <div className="metric-card shadow">
                <span>Degradacion actual</span>
                <h3>{resultado.degradacionPorcentaje}%</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="metric-card shadow">
                <span>Costo reparaciones</span>
                <h3>${resultado.costoReparaciones}</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="metric-card dark-card shadow">
                <span>Mejor escenario</span>
                <h5>{mejorEscenario?.nombre}</h5>
              </div>
            </div>
          </div>

          <div className="card shadow mb-4">
            <div className="card-body">
              <h4>Matriz de escenarios a 12 meses</h4>
              <div className="table-responsive">
                <table className="table table-hover align-middle">
                  <thead>
                    <tr>
                      <th>Escenario</th>
                      <th>Detalle</th>
                      <th>Degradacion proyectada</th>
                      <th>Valor proyectado</th>
                      <th>Costo total estimado</th>
                      <th>Prioridad</th>
                    </tr>
                  </thead>
                  <tbody>
                    {escenarios.map((escenario) => (
                      <tr key={escenario.nombre}>
                        <td className="fw-bold">{escenario.nombre}</td>
                        <td>{escenario.detalle}</td>
                        <td style={{ minWidth: "190px" }}>
                          <div className="progress progress-lg">
                            <div
                              className="progress-bar"
                              style={{ width: `${escenario.degradacionProyectada}%` }}
                            >
                              {escenario.degradacionProyectada}%
                            </div>
                          </div>
                        </td>
                        <td>${escenario.valorProyectado}</td>
                        <td>${escenario.costoTotal}</td>
                        <td>
                          <span className="badge text-bg-secondary">{escenario.prioridad}</span>
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
          </div>

          <div className="card shadow">
            <div className="card-body">
              <h4>Conclusion tecnica</h4>
              <p>
                Segun los datos recibidos desde la API, el sistema recomienda
                evaluar el escenario <strong>{mejorEscenario?.nombre}</strong>,
                porque presenta la menor degradacion proyectada para el equipo
                seleccionado.
              </p>
              <p className="mb-0">
                Esta conclusion se genera en React a partir de datos reales
                entregados por el Core del backend.
              </p>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

export default AnalisisAvanzado;
