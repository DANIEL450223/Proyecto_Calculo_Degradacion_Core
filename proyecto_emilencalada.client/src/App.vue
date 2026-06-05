<template>
  <main class="app">
    <section v-if="!u" class="auth-page">
      <article class="auth-card">
        <h2>{{ vista === 'login' ? 'Iniciar sesión' : 'Registro' }}</h2>

        <form v-if="vista === 'login'" class="form" @submit.prevent="iniciarSesion">
          <label>
            Correo
            <input v-model="login.correo" type="email" placeholder="Ingrese su correo" />
          </label>

          <label>
            Contraseña
            <input v-model="login.clave" type="password" placeholder="Ingrese su contraseña" />
          </label>

          <button type="submit">Ingresar</button>
          <button type="button" class="secondary" @click="cambiarVista('registro')">Ir a registro</button>
        </form>

        <form v-else class="form" @submit.prevent="registrarUsuario">
          <label>
            Nombre
            <input v-model="registro.nombre" type="text" placeholder="Ingrese su nombre" />
          </label>

          <label>
            Correo
            <input v-model="registro.correo" type="email" placeholder="Ingrese su correo" />
          </label>

          <label>
            Contraseña
            <input v-model="registro.clave" type="password" placeholder="Ingrese su contraseña" />
          </label>

          <button type="submit">Registrar</button>
          <button type="button" class="secondary" @click="cambiarVista('login')">Volver al login</button>
        </form>

        <p v-if="mensaje" class="message">{{ mensaje }}</p>
      </article>
    </section>

    <section v-else class="dashboard">
      <header class="topbar">
        <section>
          <h2>Sistema de Gestión de Equipos TI</h2>
          <p>{{ u.nombre }} | {{ u.rol }}</p>
        </section>

        <button @click="cerrarSesion">Salir</button>
      </header>

      <nav class="menu">
        <button v-for="item in menuVisible"
                :key="item.key"
                :class="{ active: modulo === item.key }"
                @click="cambiarModulo(item.key)">
          {{ item.label }}
        </button>
      </nav>

      <section v-if="esModuloCrud(modulo)" class="card">
        <header class="section-title">
          <h3>{{ tituloModulo }}</h3>
          <p>{{ descripcionModulo }}</p>
        </header>

        <form class="grid-form" @submit.prevent="guardarModulo(modulo)">
          <label v-for="campo in camposModulo" :key="campo.key">
            {{ campo.label }}

            <select v-if="campo.tipo === 'select'" v-model="formularios[modulo][campo.key]">
              <option disabled value="">Seleccione una opción</option>
              <option v-for="opcion in opcionesCampo(campo)"
                      :key="opcion.valor"
                      :value="opcion.valor">
                {{ opcion.texto }}
              </option>
            </select>

            <input v-else
                   v-model="formularios[modulo][campo.key]"
                   :type="campo.tipo"
                   :placeholder="campo.placeholder" />
          </label>

          <section class="actions">
            <button type="submit">
              {{ editando[modulo] ? 'Actualizar' : 'Guardar' }}
            </button>

            <button type="button" class="secondary" @click="limpiarModulo(modulo)">Limpiar</button>
          </section>
        </form>

        <section class="table-wrap">
          <table>
            <thead>
              <tr>
                <th v-for="col in columnasModulo" :key="col.key">{{ col.label }}</th>
                <th v-if="mostrarAcciones(modulo)">Acciones</th>
              </tr>
            </thead>

            <tbody>
              <tr v-for="item in listas[modulo]" :key="item.id">
                <td v-for="col in columnasModulo" :key="col.key">
                  {{ valorTabla(item, col) }}
                </td>

                <td v-if="mostrarAcciones(modulo)" class="table-actions">
                  <button v-if="puedeEditar(modulo)" @click="editarModulo(modulo, item)">Editar</button>
                  <button v-if="puedeEliminar(modulo)" @click="eliminarModulo(modulo, item.id)">Eliminar</button>

                  <button v-if="modulo === 'usuarios'" @click="cambiarEstadoUsuario(item.id)">
                    {{ item.activo ? 'Deshabilitar' : 'Habilitar' }}
                  </button>

                  <button v-if="modulo === 'usuarios'" @click="cambiarRolUsuario(item)">
                    Hacer {{ item.rol === 'Admin' ? 'Usuario' : 'Admin' }}
                  </button>
                </td>
              </tr>
            </tbody>
          </table>
        </section>
      </section>

      <section v-if="modulo === 'core'" class="card">
        <header class="section-title">
          <h3>Core de análisis de degradación</h3>
          <p>Seleccione un equipo para calcular su devaluación, vida restante y recomendación.</p>
        </header>

        <form class="grid-form" @submit.prevent="analizarCore">
          <label>
            Equipo a analizar
            <select v-model="equipoCoreId">
              <option disabled value="">Seleccione un equipo</option>
              <option v-for="e in listas.equipos" :key="e.id" :value="e.id">
                {{ e.nombre }} - {{ e.tipo }}
              </option>
            </select>
          </label>

          <section class="actions">
            <button type="submit">Analizar equipo</button>
            <button type="button" class="secondary" @click="limpiarCore">Limpiar</button>
          </section>
        </form>

        <article v-if="analisis" class="analysis">
          <h3>{{ analisis.equipo }}</h3>

          <section class="stats">
            <article>
              <span>Degradación</span>
              <strong>{{ analisis.degradacionPorcentaje }}%</strong>
            </article>

            <article>
              <span>Nivel</span>
              <strong>{{ analisis.nivelDegradacion }}</strong>
            </article>

            <article>
              <span>Valor actual</span>
              <strong>$ {{ analisis.valorActualEstimado }}</strong>
            </article>

            <article>
              <span>Recomendación</span>
              <strong>{{ analisis.recomendacion }}</strong>
            </article>
          </section>

          <ul class="details">
            <li><b>Tipo:</b> {{ analisis.tipo }}</li>
            <li><b>Marca:</b> {{ analisis.marca }}</li>
            <li><b>Estado:</b> {{ analisis.estado }}</li>
            <li><b>Antigüedad:</b> {{ analisis.antiguedadMeses }} meses</li>
            <li><b>Vida útil:</b> {{ analisis.vidaUtilMeses }} meses</li>
            <li><b>Vida restante:</b> {{ analisis.vidaRestanteMeses }} meses</li>
            <li><b>Costo inicial:</b> $ {{ analisis.costoInicial }}</li>
            <li><b>Reparaciones:</b> {{ analisis.cantidadReparaciones || 0 }}</li>
            <li><b>Costo reparaciones:</b> $ {{ analisis.costoReparaciones }}</li>
            <li><b>Porcentaje reparación:</b> {{ analisis.porcentajeCostoReparacion || 0 }}%</li>
            <li><b>Meses de degradacion por reparacion:</b> {{ analisis.restaReparacion }} meses</li>
          </ul>
        </article>
      </section>

      <p v-if="mensaje" class="message">{{ mensaje }}</p>
    </section>
  </main>
</template>

<script>
  import axios from 'axios'

  axios.interceptors.request.use(config => {
    const usuario = JSON.parse(sessionStorage.getItem('usuario') || 'null')

    if (usuario && usuario.rol) {
      config.headers['X-Rol'] = usuario.rol
    }

    return config
  })

  export default {
    data() {
      return {
        vista: 'login',
        modulo: 'departamentos',
        mensaje: '',
        u: null,

        login: { correo: '', clave: '' },
        registro: { nombre: '', correo: '', clave: '' },

        equipoCoreId: '',
        analisis: null,

        api: {
          departamentos: `${import.meta.env.VITE_API_URL}/api/Departamentos`,
          equipos: `${import.meta.env.VITE_API_URL}/api/Equipos`,
          reparaciones: `${import.meta.env.VITE_API_URL}/api/Reparaciones`,
          usuarios: `${import.meta.env.VITE_API_URL}/api/Usuarios`,
          core: `${import.meta.env.VITE_API_URL}/api/Core`,
          auth: `${import.meta.env.VITE_API_URL}/api/Auth`
        },

        listas: {
          departamentos: [],
          equipos: [],
          reparaciones: [],
          usuarios: []
        },

        formularios: {
          departamentos: { id: 0, nombre: '', presupuestoAsignado: '' },
          equipos: {
            id: 0,
            nombre: '',
            tipo: '',
            marca: '',
            fechaCompra: '',
            costoInicial: '',
            vidaUtilMeses: '',
            estado: '',
            departamentoId: ''
          },
          reparaciones: {
            id: 0,
            fechaReparacion: '',
            descripcion: '',
            costo: '',
            equipoId: ''
          },
          usuarios: {
            id: 0,
            nombre: '',
            correo: '',
            clave: '',
            rol: 'Usuario',
            activo: true,
            fechaCreacion: ''
          }
        },

        editando: {
          departamentos: false,
          equipos: false,
          reparaciones: false,
          usuarios: false
        },

        modulos: {
          departamentos: {
            titulo: 'Departamentos',
            descripcion: 'Administración de áreas y presupuestos.',
            admin: true,
            usuario: false,
            campos: [
              { key: 'nombre', label: 'Nombre', tipo: 'text', placeholder: 'Nombre del departamento' },
              { key: 'presupuestoAsignado', label: 'Presupuesto', tipo: 'number', placeholder: 'Presupuesto asignado' }
            ],
            columnas: [
              { key: 'id', label: 'ID' },
              { key: 'nombre', label: 'Nombre' },
              { key: 'presupuestoAsignado', label: 'Presupuesto' }
            ]
          },

          equipos: {
            titulo: 'Equipos',
            descripcion: 'Registro de equipos tecnológicos por departamento.',
            admin: true,
            usuario: true,
            campos: [
              { key: 'nombre', label: 'Nombre', tipo: 'text', placeholder: 'Nombre del equipo' },
              { key: 'tipo', label: 'Tipo', tipo: 'select', fuente: 'tiposEquipo' },
              { key: 'marca', label: 'Marca', tipo: 'text', placeholder: 'Marca del equipo' },
              { key: 'fechaCompra', label: 'Fecha de compra', tipo: 'date', placeholder: '' },
              { key: 'costoInicial', label: 'Costo inicial', tipo: 'number', placeholder: 'Costo inicial' },
              { key: 'vidaUtilMeses', label: 'Vida útil en meses', tipo: 'number', placeholder: 'Vida útil' },
              { key: 'estado', label: 'Estado', tipo: 'select', fuente: 'estadosEquipo' },
              { key: 'departamentoId', label: 'Departamento', tipo: 'select', fuente: 'departamentos' }
            ],
            columnas: [
              { key: 'id', label: 'ID' },
              { key: 'nombre', label: 'Nombre' },
              { key: 'tipo', label: 'Tipo' },
              { key: 'marca', label: 'Marca' },
              { key: 'departamento.nombre', label: 'Departamento' }
            ]
          },

          reparaciones: {
            titulo: 'Reparaciones',
            descripcion: 'Registro de mantenimientos y costos por equipo.',
            admin: true,
            usuario: true,
            campos: [
              { key: 'fechaReparacion', label: 'Fecha', tipo: 'date', placeholder: '' },
              { key: 'descripcion', label: 'Descripción', tipo: 'text', placeholder: 'Descripción de la reparación' },
              { key: 'costo', label: 'Costo', tipo: 'number', placeholder: 'Costo de reparación' },
              { key: 'equipoId', label: 'Equipo', tipo: 'select', fuente: 'equipos' }
            ],
            columnas: [
              { key: 'id', label: 'ID' },
              { key: 'fechaReparacion', label: 'Fecha', formato: 'fecha' },
              { key: 'descripcion', label: 'Descripción' },
              { key: 'costo', label: 'Costo' },
              { key: 'equipo.nombre', label: 'Equipo' }
            ]
          },

          usuarios: {
            titulo: 'Usuarios',
            descripcion: 'Administración de cuentas, roles y permisos.',
            admin: true,
            usuario: false,
            campos: [
              { key: 'nombre', label: 'Nombre', tipo: 'text', placeholder: 'Nombre del usuario' },
              { key: 'correo', label: 'Correo', tipo: 'email', placeholder: 'Correo del usuario' },
              { key: 'clave', label: 'Contraseña', tipo: 'text', placeholder: 'Contraseña' },
              { key: 'rol', label: 'Rol', tipo: 'select', fuente: 'roles' }
            ],
            columnas: [
              { key: 'id', label: 'ID' },
              { key: 'nombre', label: 'Nombre' },
              { key: 'correo', label: 'Correo' },
              { key: 'rol', label: 'Rol' },
              { key: 'activo', label: 'Estado', formato: 'estado' },
              { key: 'fechaCreacion', label: 'Fecha creación', formato: 'fecha' }
            ]
          }
        }
      }
    },

    computed: {
      menuVisible() {
        const base = [
          { key: 'departamentos', label: 'Departamentos' },
          { key: 'equipos', label: 'Equipos' },
          { key: 'reparaciones', label: 'Reparaciones' },
          { key: 'core', label: 'Core' },
          { key: 'usuarios', label: 'Usuarios' }
        ]

        return base.filter(x => {
          if (x.key === 'core') return true

          const config = this.modulos[x.key]

          if (this.esAdmin()) return config?.admin
          return config?.usuario
        })
      },

      tituloModulo() {
        return this.modulos[this.modulo]?.titulo || ''
      },

      descripcionModulo() {
        return this.modulos[this.modulo]?.descripcion || ''
      },

      camposModulo() {
        return this.modulos[this.modulo]?.campos || []
      },

      columnasModulo() {
        return this.modulos[this.modulo]?.columnas || []
      }
    },

    methods: {
      formVacio(tipo) {
        const modelos = {
          departamentos: { id: 0, nombre: '', presupuestoAsignado: '' },
          equipos: {
            id: 0,
            nombre: '',
            tipo: '',
            marca: '',
            fechaCompra: '',
            costoInicial: '',
            vidaUtilMeses: '',
            estado: '',
            departamentoId: ''
          },
          reparaciones: {
            id: 0,
            fechaReparacion: '',
            descripcion: '',
            costo: '',
            equipoId: ''
          },
          usuarios: {
            id: 0,
            nombre: '',
            correo: '',
            clave: '',
            rol: 'Usuario',
            activo: true,
            fechaCreacion: ''
          }
        }

        return { ...modelos[tipo] }
      },

      esAdmin() {
        return this.u?.rol === 'Admin'
      },

      esUsuario() {
        return this.u?.rol === 'Usuario'
      },

      esModuloCrud(nombre) {
        return ['departamentos', 'equipos', 'reparaciones', 'usuarios'].includes(nombre)
      },

      cambiarVista(nombre) {
        this.vista = nombre
        this.mensaje = ''
      },

      cambiarModulo(nombre) {
        if (!this.puedeVer(nombre)) {
          this.mensaje = 'No tiene permisos para acceder a este módulo'
          this.modulo = this.esAdmin() ? 'departamentos' : 'equipos'
          return
        }

        this.modulo = nombre
        this.mensaje = ''
        this.analisis = null
        this.equipoCoreId = ''
      },

      puedeVer(nombre) {
        if (nombre === 'core') return true

        const config = this.modulos[nombre]

        if (!config) return false

        return this.esAdmin() ? config.admin : config.usuario
      },

      puedeEditar() {
        return this.esAdmin()
      },

      puedeEliminar() {
        return this.esAdmin()
      },

      mostrarAcciones(nombre) {
        return this.esAdmin() && ['departamentos', 'equipos', 'reparaciones', 'usuarios'].includes(nombre)
      },

      opcionesCampo(campo) {
        if (campo.fuente === 'departamentos') {
          return this.listas.departamentos.map(x => ({ valor: x.id, texto: x.nombre }))
        }

        if (campo.fuente === 'equipos') {
          return this.listas.equipos.map(x => ({ valor: x.id, texto: x.nombre }))
        }

        if (campo.fuente === 'roles') {
          return [
            { valor: 'Admin', texto: 'Admin' },
            { valor: 'Usuario', texto: 'Usuario' }
          ]
        }

        if (campo.fuente === 'tiposEquipo') {
          return [
            { valor: 'Laptop', texto: 'Laptop' },
            { valor: 'Portatil', texto: 'Portátil' },
            { valor: 'Escritorio', texto: 'Escritorio' },
            { valor: 'PC', texto: 'PC' },
            { valor: 'Impresora', texto: 'Impresora' },
            { valor: 'Servidor', texto: 'Servidor' },
            { valor: 'Televisor', texto: 'Televisor' },
            { valor: 'Electronico', texto: 'Electrónico' }
          ]
        }

        if (campo.fuente === 'estadosEquipo') {
          return [
            { valor: 'Activo', texto: 'Activo' },
            { valor: 'Regular', texto: 'Regular' },
            { valor: 'Deteriorado', texto: 'Deteriorado' },
            { valor: 'Crítico', texto: 'Crítico' },
            { valor: 'Danado', texto: 'Dañado' }
          ]
        }

        return []
      },

      valorTabla(item, col) {
        const valor = col.key.split('.').reduce((obj, key) => obj?.[key], item)

        if (col.formato === 'fecha') return valor ? String(valor).split('T')[0] : ''
        if (col.formato === 'estado') return valor ? 'Habilitado' : 'Deshabilitado'

        return valor ?? ''
      },

      async registrarUsuario() {
        try {
          await axios.post(`${this.api.auth}/registrar`, this.registro)

          this.mensaje = 'Usuario registrado correctamente'
          this.registro = { nombre: '', correo: '', clave: '' }
          this.vista = 'login'
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al registrar'
        }
      },

      async iniciarSesion() {
        try {
          const r = await axios.post(`${this.api.auth}/login`, this.login)

          this.u = r.data.usuario
          sessionStorage.setItem('usuario', JSON.stringify(this.u))

          this.modulo = this.esAdmin() ? 'departamentos' : 'equipos'
          this.mensaje = ''

          await this.cargarTodo()
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al iniciar sesión'
        }
      },

      cerrarSesion() {
        this.u = null
        sessionStorage.removeItem('usuario')
        this.vista = 'login'
        this.modulo = 'departamentos'
        this.mensaje = ''
        this.analisis = null
        this.equipoCoreId = ''
      },

      async cargarTodo() {
        await this.obtenerModulo('departamentos')
        await this.obtenerModulo('equipos')
        await this.obtenerModulo('reparaciones')

        if (this.esAdmin()) {
          await this.obtenerModulo('usuarios')
        }
      },

      async obtenerModulo(nombre) {
        try {
          if (!this.api[nombre]) return

          const r = await axios.get(this.api[nombre])
          this.listas[nombre] = r.data
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || `Error al cargar ${nombre}`
        }
      },

      validarFormulario(nombre, data) {
        const reglas = {
          departamentos: [
            [!data.nombre, 'Debe ingresar el nombre del departamento'],
            [Number(data.presupuestoAsignado) <= 0, 'El presupuesto debe ser mayor a cero']
          ],
          equipos: [
            [!data.nombre, 'Debe ingresar el nombre del equipo'],
            [!data.tipo, 'Debe seleccionar el tipo de equipo'],
            [!data.marca, 'Debe ingresar la marca del equipo'],
            [!data.fechaCompra, 'Debe seleccionar la fecha de compra'],
            [Number(data.costoInicial) <= 0, 'El costo inicial debe ser mayor a cero'],
            [Number(data.vidaUtilMeses) <= 0, 'La vida útil debe ser mayor a cero'],
            [!data.estado, 'Debe seleccionar el estado del equipo'],
            [!data.departamentoId, 'Debe seleccionar un departamento']
          ],
          reparaciones: [
            [!data.fechaReparacion, 'Debe seleccionar la fecha de reparación'],
            [!data.descripcion, 'Debe ingresar la descripción'],
            [Number(data.costo) <= 0, 'El costo debe ser mayor a cero'],
            [!data.equipoId, 'Debe seleccionar un equipo']
          ],
          usuarios: [
            [!data.nombre, 'Debe ingresar el nombre del usuario'],
            [!data.correo, 'Debe ingresar el correo'],
            [!data.clave, 'Debe ingresar la contraseña'],
            [!data.rol, 'Debe seleccionar un rol']
          ]
        }

        for (const regla of reglas[nombre] || []) {
          if (regla[0]) {
            this.mensaje = regla[1]
            return false
          }
        }

        return true
      },

      prepararPayload(nombre) {
        const data = { ...this.formularios[nombre] }

        if (nombre === 'departamentos') {
          data.presupuestoAsignado = Number(data.presupuestoAsignado)
        }

        if (nombre === 'equipos') {
          data.costoInicial = Number(data.costoInicial)
          data.vidaUtilMeses = Number(data.vidaUtilMeses)
          data.departamentoId = Number(data.departamentoId)
        }

        if (nombre === 'reparaciones') {
          data.costo = Number(data.costo)
          data.equipoId = Number(data.equipoId)
        }

        return data
      },

      async guardarModulo(nombre) {
        try {
          const data = this.prepararPayload(nombre)

          if (!this.validarFormulario(nombre, data)) return

          if (this.editando[nombre]) {
            await axios.put(`${this.api[nombre]}/${data.id}`, data)
            this.mensaje = 'Registro actualizado correctamente'
          } else {
            await axios.post(this.api[nombre], data)
            this.mensaje = 'Registro guardado correctamente'
          }

          this.limpiarModulo(nombre)
          await this.obtenerModulo(nombre)

          if (nombre === 'departamentos') await this.obtenerModulo('equipos')
          if (nombre === 'equipos') await this.obtenerModulo('reparaciones')
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al guardar'
        }
      },

      editarModulo(nombre, item) {
        const data = { ...item }

        if (nombre === 'equipos') data.fechaCompra = data.fechaCompra?.split('T')[0] || ''
        if (nombre === 'reparaciones') data.fechaReparacion = data.fechaReparacion?.split('T')[0] || ''
        if (nombre === 'usuarios') data.fechaCreacion = data.fechaCreacion?.split('T')[0] || ''

        this.formularios[nombre] = data
        this.editando[nombre] = true
        this.mensaje = ''
      },

      async eliminarModulo(nombre, id) {
        try {
          await axios.delete(`${this.api[nombre]}/${id}`)

          this.mensaje = 'Registro eliminado correctamente'

          await this.obtenerModulo(nombre)
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al eliminar'
        }
      },

      limpiarModulo(nombre) {
        this.formularios[nombre] = this.formVacio(nombre)
        this.editando[nombre] = false
        this.mensaje = ''
      },

      async cambiarEstadoUsuario(id) {
        try {
          await axios.put(`${this.api.usuarios}/estado/${id}`)

          await this.obtenerModulo('usuarios')
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al cambiar estado del usuario'
        }
      },

      async cambiarRolUsuario(usuario) {
        try {
          const nuevoRol = usuario.rol === 'Admin' ? 'Usuario' : 'Admin'

          await axios.put(`${this.api.usuarios}/cambiar-rol/${usuario.id}`, JSON.stringify(nuevoRol), {
            headers: { 'Content-Type': 'application/json' }
          })

          await this.obtenerModulo('usuarios')
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al cambiar rol del usuario'
        }
      },

      async analizarCore() {
        try {
          if (!this.equipoCoreId) {
            this.mensaje = 'Debe seleccionar un equipo'
            return
          }

          const r = await axios.get(`${this.api.core}/degradacion/${this.equipoCoreId}`)

          this.analisis = r.data
          this.mensaje = ''
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al ejecutar el análisis del core'
        }
      },

      limpiarCore() {
        this.analisis = null
        this.equipoCoreId = ''
        this.mensaje = ''
      }
    },

    mounted() {
      this.u = null
      this.vista = 'login'
      this.modulo = 'departamentos'
    }
  }
</script>

<style scoped>
  * {
    box-sizing: border-box;
  }

  .app {
    min-height: 100vh;
    padding: 24px;
    font-family: Arial, sans-serif;
    background: #eef3f8;
    color: #1d2939;
  }

  .auth-page {
    min-height: 90vh;
    display: grid;
    place-items: center;
  }

  .auth-card,
  .card,
  .topbar {
    background: white;
    border-radius: 16px;
    box-shadow: 0 10px 25px rgba(15, 23, 42, 0.08);
  }

  .auth-card {
    width: 100%;
    max-width: 380px;
    padding: 28px;
  }

  .dashboard {
    max-width: 1200px;
    margin: auto;
  }

  .topbar {
    padding: 20px;
    margin-bottom: 18px;
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

    .topbar h2,
    .topbar p {
      margin: 0;
    }

  .menu {
    display: flex;
    flex-wrap: wrap;
    gap: 10px;
    margin-bottom: 18px;
  }

  button {
    border: none;
    border-radius: 10px;
    padding: 10px 14px;
    cursor: pointer;
    background: #2563eb;
    color: white;
    font-weight: 600;
  }

    button:hover {
      opacity: 0.9;
    }

    button.secondary {
      background: #64748b;
    }

    button.active {
      background: #0f172a;
    }

  .card {
    padding: 22px;
    margin-bottom: 18px;
  }

  .section-title h3 {
    margin: 0 0 4px;
  }

  .section-title p {
    margin: 0 0 18px;
    color: #667085;
  }

  .form,
  .grid-form {
    display: grid;
    gap: 14px;
  }

  .grid-form {
    grid-template-columns: repeat(auto-fit, minmax(210px, 1fr));
    align-items: end;
    margin-bottom: 20px;
  }

  label {
    display: grid;
    gap: 6px;
    font-size: 14px;
    font-weight: 600;
  }

  input,
  select {
    width: 100%;
    padding: 11px;
    border: 1px solid #cbd5e1;
    border-radius: 10px;
    background: white;
  }

  .actions,
  .table-actions {
    display: flex;
    flex-wrap: wrap;
    gap: 8px;
  }

  .table-wrap {
    overflow-x: auto;
  }

  table {
    width: 100%;
    border-collapse: collapse;
    background: white;
  }

  th,
  td {
    padding: 12px;
    border-bottom: 1px solid #e2e8f0;
    text-align: left;
  }

  th {
    background: #f8fafc;
  }

  .message {
    padding: 12px;
    background: #fff7ed;
    border: 1px solid #fed7aa;
    border-radius: 10px;
    color: #9a3412;
  }

  .analysis {
    margin-top: 20px;
    padding: 18px;
    border-radius: 16px;
    background: #f8fafc;
  }

  .stats {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(160px, 1fr));
    gap: 12px;
    margin: 16px 0;
  }

    .stats article {
      background: white;
      border-radius: 14px;
      padding: 16px;
      border: 1px solid #e2e8f0;
    }

    .stats span {
      display: block;
      color: #667085;
      margin-bottom: 6px;
    }

    .stats strong {
      font-size: 20px;
    }

  .details {
    display: grid;
    grid-template-columns: repeat(auto-fit, minmax(220px, 1fr));
    gap: 8px;
    padding-left: 18px;
  }
</style>
