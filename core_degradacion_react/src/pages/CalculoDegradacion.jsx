import { useState } from "react";
import api from "../api/axiosConfig";

function obtenerClaseNivel(nivel) {
  const texto = (nivel || "").toLowerCase();

  if (texto.includes("critica") || texto.includes("alta")) {
    return "danger";
  }

  if (texto.includes("media")) {
    return "warning";
  }

  return "success";
}

function calcularIndiceRiesgo(resultado) {
  if (!resultado) return 0;

  const degradacion = Number(resultado.degradacionPorcentaje || 0);
  const reparaciones = Number(resultado.cantidadReparaciones || 0);
  const costoInicial = Number(resultado.costoInicial || 0);
  const costoReparaciones = Number(resultado.costoReparaciones || 0);
  const porcentajeCosto = costoInicial > 0 ? (costoReparaciones / costoInicial) * 100 : 0;

  const riesgo = degradacion * 0.7 + reparaciones * 4 + porcentajeCosto * 0.2;
  return Math.min(100, Math.max(0, riesgo)).toFixed(2);
}

function generarDecision(resultado) {
  if (!resultado) return "Sin datos";

  const degradacion = Number(resultado.degradacionPorcentaje || 0);
  const costoInicial = Number(resultado.costoInicial || 0);
  const costoReparaciones = Number(resultado.costoReparaciones || 0);
  const porcentajeCosto = costoInicial > 0 ? (costoReparaciones / costoInicial) * 100 : 0;

  if (degradacion >= 85 || porcentajeCosto >= 50) {
    return "Reemplazo recomendado";
  }

  if (degradacion >= 65 || porcentajeCosto >= 25) {
    return "Planificar mantenimiento mayor";
  }

  if (degradacion >= 40) {
    return "Seguimiento y mantenimiento preventivo";
  }

  return "Mantener en operacion";
}

function CalculoDegradacion() {
  const [equipoId, setEquipoId] = useState(1);
  const [resultado, setResultado] = useState(null);
  const [error, setError] = useState("");
  const [cargando, setCargando] = useState(false);

  const calcularDegradacion = async (e) => {
    e.preventDefault();
    setError("");
    setResultado(null);
    setCargando(true);

    try {
      const response = await api.get(`/api/Core/degradacion/${equipoId}`);
      setResultado(response.data);
    } catch {
      setError("No se pudo consultar la API. Revisa que el endpoint exista o prueba con ID 1.");
    } finally {
      setCargando(false);
    }
  };

  const indiceRiesgo = calcularIndiceRiesgo(resultado);
  const decision = generarDecision(resultado);
  const colorNivel = obtenerClaseNivel(resultado?.nivelDegradacion);

  return (
    <div>
      <h2 className="mb-4">Calculo individual de degradacion</h2>

      <div className="card shadow mb-4">
        <div className="card-body">
          <form onSubmit={calcularDegradacion}>
            <div className="row g-3 align-items-end">
              <div className="col-md-8">
                <label className="form-label">ID del equipo</label>
                <input
                  type="number"
                  className="form-control"
                  value={equipoId}
                  onChange={(e) => setEquipoId(e.target.value)}
                  min="1"
                  required
                />
              </div>
              <div className="col-md-4">
                <button className="btn btn-primary w-100" disabled={cargando}>
                  {cargando ? "Consultando API..." : "Calcular degradacion"}
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
                <span>Degradacion</span>
                <h3>{resultado.degradacionPorcentaje}%</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="metric-card shadow">
                <span>Indice de riesgo</span>
                <h3>{indiceRiesgo}%</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className="metric-card shadow">
                <span>Valor actual</span>
                <h3>${resultado.valorActualEstimado}</h3>
              </div>
            </div>
            <div className="col-md-3">
              <div className={`metric-card shadow border-${colorNivel}`}>
                <span>Nivel</span>
                <h3>{resultado.nivelDegradacion}</h3>
              </div>
            </div>
          </div>

          <div className="card shadow mb-4">
            <div className="card-body">
              <div className="d-flex justify-content-between align-items-start gap-3 flex-wrap">
                <div>
                  <h4>Resultado del analisis</h4>
                  <p className="text-muted mb-0">Datos obtenidos desde la API del Core.</p>
                </div>
                <span className={`badge text-bg-${colorNivel} fs-6`}>{decision}</span>
              </div>

              <hr />

              <div className="row mt-3">
                <div className="col-md-6">
                  <p><strong>Equipo:</strong> {resultado.equipo}</p>
                  <p><strong>Tipo:</strong> {resultado.tipo}</p>
                  <p><strong>Marca:</strong> {resultado.marca}</p>
                  <p><strong>Estado:</strong> {resultado.estado}</p>
                  <p><strong>Costo inicial:</strong> ${resultado.costoInicial}</p>
                </div>
                <div className="col-md-6">
                  <p><strong>Antiguedad:</strong> {resultado.antiguedadMeses} meses</p>
                  <p><strong>Vida util:</strong> {resultado.vidaUtilMeses} meses</p>
                  <p><strong>Reparaciones:</strong> {resultado.cantidadReparaciones}</p>
                  <p><strong>Costo reparaciones:</strong> ${resultado.costoReparaciones}</p>
                  <p><strong>Recomendacion API:</strong> {resultado.recomendacion}</p>
                </div>
              </div>

              <div className="mt-3">
                <label className="form-label fw-bold">Barra de degradacion</label>
                <div className="progress progress-lg">
                  <div
                    className={`progress-bar bg-${colorNivel}`}
                    style={{ width: `${resultado.degradacionPorcentaje}%` }}
                  >
                    {resultado.degradacionPorcentaje}%
                  </div>
                </div>
              </div>
            </div>
          </div>
        </>
      )}
    </div>
  );
}

export default CalculoDegradacion;
