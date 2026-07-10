import { Link, NavLink } from "react-router-dom";

function Navbar() {
  const linkClass = ({ isActive }) =>
    isActive ? "nav-link active fw-bold" : "nav-link";

  return (
    <nav className="navbar navbar-expand-lg navbar-dark bg-dark shadow-sm">
      <div className="container">
        <Link className="navbar-brand fw-bold" to="/">
          Core Degradacion TI
        </Link>

        <button
          className="navbar-toggler"
          type="button"
          data-bs-toggle="collapse"
          data-bs-target="#menuPrincipal"
        >
          <span className="navbar-toggler-icon"></span>
        </button>

        <div className="collapse navbar-collapse" id="menuPrincipal">
          <div className="navbar-nav me-auto">
            <NavLink className={linkClass} to="/">
              Inicio
            </NavLink>
            <NavLink className={linkClass} to="/degradacion">
              Calculo individual
            </NavLink>
            <NavLink className={linkClass} to="/analisis">
              Analisis avanzado
            </NavLink>
            <NavLink className={linkClass} to="/gestion">
              Gestion de datos
            </NavLink>
          </div>

          <a
            className="btn btn-outline-light btn-sm"
            href="https://localhost:7007/swagger"
            target="_blank"
            rel="noreferrer"
          >
            Swagger API
          </a>
        </div>
      </div>
    </nav>
  );
}

export default Navbar;
