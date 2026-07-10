import { useEffect, useMemo, useState } from "react";
import api from "../api/axiosConfig";

const hoy = new Date().toISOString().slice(0, 10);

function GestionDatos() {
  const [departamentos, setDepartamentos] = useState([]);
  const [equipos, setEquipos] = useState([]);
  const [reparaciones, setReparaciones] = useState([]);
  const [mensaje, setMensaje] = useState("");
  const [error, setError] = useState("");
  const [resultado, setResultado] = useState(null);
  const [equipoEvaluarId, setEquipoEvaluarId] = useState("");

  const [departamento, setDepartamento] = useState({
    nombre: "",
    presupuestoAsignado: "",
  });

  const [equipo, setEquipo] = useState({
    nombre: "",
    tipo: "Laptop",
    marca: "",
    fechaCompra: hoy,
    costoInicial: "",
    vidaUtilMeses: 60,
    estado: "Activo",
    departamentoId: "",
  });

  const [reparacion, setReparacion] = useState({
    equipoId: "",
    fechaReparacion: hoy,
    descripcion: "",
    costo: "",
  });

  const primerDepartamentoId = useMemo(
    () => departamentos[0]?.id || "",
    [departamentos],
  );

  const primerEquipoId = useMemo(
    () => equipos[0]?.id || "",
    [equipos],
  );

  const manejarError = (e, texto) => {
    setMensaje("");
    setError(e.response?.data?.mensaje || texto);
  };

  const cargarTodo = async () => {
    setError("");
    try {
      const [departamentosRes, equiposRes, reparacionesRes] = await Promise.all([
        api.get("/api/Departamentos"),
        api.get("/api/Equipos"),
        api.get("/api/Reparaciones"),
      ]);

      setDepartamentos(departamentosRes.data);
      setEquipos(equiposRes.data);
      setReparaciones(reparacionesRes.data);
    } catch (e) {
      manejarError(e, "No se pudo cargar la informacion. Revisa backend y base de datos.");
    }
  };

  useEffect(() => {
    cargarTodo();
  }, []);

  useEffect(() => {
    if (!equipo.departamentoId && primerDepartamentoId) {
      setEquipo((actual) => ({ ...actual, departamentoId: primerDepartamentoId }));
    }
  }, [equipo.departamentoId, primerDepartamentoId]);

  useEffect(() => {
    if (!reparacion.equipoId && primerEquipoId) {
      setReparacion((actual) => ({ ...actual, equipoId: primerEquipoId }));
    }

    if (!equipoEvaluarId && primerEquipoId) {
      setEquipoEvaluarId(primerEquipoId);
    }
  }, [equipoEvaluarId, primerEquipoId, reparacion.equipoId]);

  const guardarDepartamento = async (e) => {
    e.preventDefault();
    try {
      await api.post("/api/Departamentos", {
        nombre: departamento.nombre,
        presupuestoAsignado: Number(departamento.presupuestoAsignado),
      });
      setDepartamento({ nombre: "", presupuestoAsignado: "" });
      setMensaje("Departamento guardado correctamente.");
      setError("");
      await cargarTodo();
    } catch (ex) {
      manejarError(ex, "No se pudo guardar el departamento.");
    }
  };

  const guardarEquipo = async (e) => {
    e.preventDefault();
    try {
      await api.post("/api/Equipos", {
        ...equipo,
        costoInicial: Number(equipo.costoInicial),
        vidaUtilMeses: Number(equipo.vidaUtilMeses),
        departamentoId: Number(equipo.departamentoId),
      });
      setEquipo({
        nombre: "",
        tipo: "Laptop",
        marca: "",
        fechaCompra: hoy,
        costoInicial: "",
        vidaUtilMeses: 60,
        estado: "Activo",
        departamentoId: primerDepartamentoId,
      });
      setMensaje("Equipo guardado correctamente.");
      setError("");
      await cargarTodo();
    } catch (ex) {
      manejarError(ex, "No se pudo guardar el equipo.");
    }
  };

  const guardarReparacion = async (e) => {
    e.preventDefault();
    try {
      await api.post("/api/Reparaciones", {
        ...reparacion,
        equipoId: Number(reparacion.equipoId),
        costo: Number(reparacion.costo),
      });
      setReparacion({
        equipoId: primerEquipoId,
        fechaReparacion: hoy,
        descripcion: "",
        costo: "",
      });
      setMensaje("Reparacion guardada correctamente.");
      setError("");
      await cargarTodo();
    } catch (ex) {
      manejarError(ex, "No se pudo guardar la reparacion.");
    }
  };

  const evaluarEquipo = async (e) => {
    e.preventDefault();
    setResultado(null);

    try {
      const response = await api.get(`/api/Core/degradacion/${equipoEvaluarId}`);
      setResultado(response.data);
      setMensaje("Evaluacion generada correctamente.");
      setError("");
    } catch (ex) {
      manejarError(ex, "No se pudo evaluar el equipo.");
    }
  };

  return (
    <div>
      <div className="d-flex justify-content-between align-items-start gap-3 flex-wrap mb-4">
        <div>
          <h2 className="mb-1">Gestion completa del Core</h2>
          <p className="text-muted mb-0">
            Agrega datos, lista registros y evalua equipos usando el mismo backend local.
          </p>
        </div>
        <button className="btn btn-outline-primary" onClick={cargarTodo}>
          Recargar
        </button>
      </div>

      {mensaje && <div className="alert alert-success">{mensaje}</div>}
      {error && <div className="alert alert-danger">{error}</div>}

      <div className="row g-4 mb-4">
        <div className="col-lg-4">
          <div className="card shadow h-100">
            <div className="card-body">
              <h5>Nuevo departamento</h5>
              <form onSubmit={guardarDepartamento} className="vstack gap-3">
                <input
                  className="form-control"
                  placeholder="Nombre"
                  value={departamento.nombre}
                  onChange={(e) => setDepartamento({ ...departamento, nombre: e.target.value })}
                  required
                />
                <input
                  className="form-control"
                  type="number"
                  min="1"
                  placeholder="Presupuesto"
                  value={departamento.presupuestoAsignado}
                  onChange={(e) => setDepartamento({ ...departamento, presupuestoAsignado: e.target.value })}
                  required
                />
                <button className="btn btn-primary">Guardar departamento</button>
              </form>
            </div>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card shadow h-100">
            <div className="card-body">
              <h5>Nuevo equipo</h5>
              <form onSubmit={guardarEquipo} className="vstack gap-3">
                <input className="form-control" placeholder="Nombre" value={equipo.nombre} onChange={(e) => setEquipo({ ...equipo, nombre: e.target.value })} required />
                <select className="form-select" value={equipo.tipo} onChange={(e) => setEquipo({ ...equipo, tipo: e.target.value })}>
                  <option>Laptop</option>
                  <option>Escritorio</option>
                  <option>Servidor</option>
                  <option>Impresora</option>
                  <option>Televisor</option>
                  <option>Electronico</option>
                </select>
                <input className="form-control" placeholder="Marca" value={equipo.marca} onChange={(e) => setEquipo({ ...equipo, marca: e.target.value })} required />
                <input className="form-control" type="date" value={equipo.fechaCompra} onChange={(e) => setEquipo({ ...equipo, fechaCompra: e.target.value })} required />
                <input className="form-control" type="number" min="1" placeholder="Costo inicial" value={equipo.costoInicial} onChange={(e) => setEquipo({ ...equipo, costoInicial: e.target.value })} required />
                <input className="form-control" type="number" min="1" placeholder="Vida util meses" value={equipo.vidaUtilMeses} onChange={(e) => setEquipo({ ...equipo, vidaUtilMeses: e.target.value })} required />
                <select className="form-select" value={equipo.estado} onChange={(e) => setEquipo({ ...equipo, estado: e.target.value })}>
                  <option>Activo</option>
                  <option>Regular</option>
                  <option>Deteriorado</option>
                  <option>Critico</option>
                  <option>Danado</option>
                </select>
                <select className="form-select" value={equipo.departamentoId} onChange={(e) => setEquipo({ ...equipo, departamentoId: e.target.value })} required>
                  <option value="">Seleccione departamento</option>
                  {departamentos.map((item) => (
                    <option key={item.id} value={item.id}>{item.nombre}</option>
                  ))}
                </select>
                <button className="btn btn-primary">Guardar equipo</button>
              </form>
            </div>
          </div>
        </div>

        <div className="col-lg-4">
          <div className="card shadow h-100">
            <div className="card-body">
              <h5>Nueva reparacion</h5>
              <form onSubmit={guardarReparacion} className="vstack gap-3">
                <select className="form-select" value={reparacion.equipoId} onChange={(e) => setReparacion({ ...reparacion, equipoId: e.target.value })} required>
                  <option value="">Seleccione equipo</option>
                  {equipos.map((item) => (
                    <option key={item.id} value={item.id}>{item.nombre}</option>
                  ))}
                </select>
                <input className="form-control" type="date" value={reparacion.fechaReparacion} onChange={(e) => setReparacion({ ...reparacion, fechaReparacion: e.target.value })} required />
                <input className="form-control" placeholder="Descripcion" value={reparacion.descripcion} onChange={(e) => setReparacion({ ...reparacion, descripcion: e.target.value })} required />
                <input className="form-control" type="number" min="1" placeholder="Costo" value={reparacion.costo} onChange={(e) => setReparacion({ ...reparacion, costo: e.target.value })} required />
                <button className="btn btn-primary">Guardar reparacion</button>
              </form>
            </div>
          </div>
        </div>
      </div>

      <div className="card shadow mb-4">
        <div className="card-body">
          <h5>Evaluar equipo</h5>
          <form onSubmit={evaluarEquipo} className="row g-3 align-items-end">
            <div className="col-md-8">
              <select className="form-select" value={equipoEvaluarId} onChange={(e) => setEquipoEvaluarId(e.target.value)} required>
                <option value="">Seleccione equipo</option>
                {equipos.map((item) => (
                  <option key={item.id} value={item.id}>{item.nombre}</option>
                ))}
              </select>
            </div>
            <div className="col-md-4">
              <button className="btn btn-dark w-100">Evaluar degradacion</button>
            </div>
          </form>

          {resultado && (
            <div className="row g-3 mt-3">
              <div className="col-md-3"><div className="metric-card shadow"><span>Equipo</span><h5>{resultado.equipo}</h5></div></div>
              <div className="col-md-3"><div className="metric-card shadow"><span>Degradacion</span><h3>{resultado.degradacionPorcentaje}%</h3></div></div>
              <div className="col-md-3"><div className="metric-card shadow"><span>Valor actual</span><h3>${resultado.valorActualEstimado}</h3></div></div>
              <div className="col-md-3"><div className="metric-card dark-card shadow"><span>Recomendacion</span><h5>{resultado.recomendacion}</h5></div></div>
            </div>
          )}
        </div>
      </div>

      <div className="row g-4">
        <div className="col-xl-4">
          <Tabla titulo="Departamentos" columnas={["id", "nombre", "presupuestoAsignado"]} datos={departamentos} />
        </div>
        <div className="col-xl-4">
          <Tabla titulo="Equipos" columnas={["id", "nombre", "tipo", "marca", "estado"]} datos={equipos} />
        </div>
        <div className="col-xl-4">
          <Tabla titulo="Reparaciones" columnas={["id", "fechaReparacion", "descripcion", "costo"]} datos={reparaciones} />
        </div>
      </div>
    </div>
  );
}

function Tabla({ titulo, columnas, datos }) {
  return (
    <div className="card shadow h-100">
      <div className="card-body">
        <h5>{titulo}</h5>
        <div className="table-responsive">
          <table className="table table-sm table-hover align-middle">
            <thead>
              <tr>{columnas.map((columna) => <th key={columna}>{columna}</th>)}</tr>
            </thead>
            <tbody>
              {datos.map((item) => (
                <tr key={item.id}>
                  {columnas.map((columna) => (
                    <td key={columna}>{String(item[columna] ?? "").split("T")[0]}</td>
                  ))}
                </tr>
              ))}
              {datos.length === 0 && (
                <tr>
                  <td colSpan={columnas.length} className="text-muted">Sin datos</td>
                </tr>
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default GestionDatos;
