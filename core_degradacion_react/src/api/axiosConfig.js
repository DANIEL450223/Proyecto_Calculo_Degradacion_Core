import axios from "axios";

const api = axios.create({
    baseURL:
        import.meta.env.VITE_API_URL ||
        "http://localhost:5086",
    timeout: 90000,
});

api.interceptors.request.use((config) => {
    config.headers["X-Rol"] = "Admin";
    return config;
});

export default api;
