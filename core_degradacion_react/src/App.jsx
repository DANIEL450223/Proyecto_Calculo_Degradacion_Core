import { Routes, Route, Navigate } from "react-router-dom";
import Navbar from "./components/Navbar";
import Inicio from "./pages/Inicio";
import CalculoDegradacion from "./pages/CalculoDegradacion";
import AnalisisAvanzado from "./pages/AnalisisAvanzado";
import GestionDatos from "./pages/GestionDatos";

function App() {
  return (
    <>
      <Navbar />
      <main className="container py-4">
        <Routes>
          <Route path="/" element={<Inicio />} />
          <Route path="/degradacion" element={<CalculoDegradacion />} />
          <Route path="/analisis" element={<AnalisisAvanzado />} />
          <Route path="/gestion" element={<GestionDatos />} />
          <Route path="*" element={<Navigate to="/" />} />
        </Routes>
      </main>
    </>
  );
}

export default App;
