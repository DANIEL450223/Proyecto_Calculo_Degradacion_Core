<template>
  <div style="padding:20px;font-family:Arial;background:white;min-height:100vh">
    <div v-if="!u" style="max-width:340px;border:1px solid #000;padding:20px;margin:auto;margin-top:60px">
      <div v-if="vista === 'login'">
        <h3>Login</h3>

        <div style="margin-bottom:10px">
          <label style="display:block;margin-bottom:4px">Correo</label>
          <input v-model="login.correo" placeholder="Ingrese su correo" style="width:100%;padding:8px" />
        </div>

        <div style="margin-bottom:10px">
          <label style="display:block;margin-bottom:4px">Contraseña</label>
          <input v-model="login.clave" type="password" placeholder="Ingrese su contraseña" style="width:100%;padding:8px" />
        </div>

        <button @click="iniciarSesion" style="width:100%;padding:8px;margin-bottom:10px">Ingresar</button>
        <button @click="vista = 'registro'; mensaje = ''" style="width:100%;padding:8px">Ir a registro</button>
      </div>

      <div v-else>
        <h3>Registro</h3>

        <div style="margin-bottom:10px">
          <label style="display:block;margin-bottom:4px">Nombre</label>
          <input v-model="registro.nombre" placeholder="Ingrese su nombre" style="width:100%;padding:8px" />
        </div>

        <div style="margin-bottom:10px">
          <label style="display:block;margin-bottom:4px">Correo</label>
          <input v-model="registro.correo" placeholder="Ingrese su correo" style="width:100%;padding:8px" />
        </div>

        <div style="margin-bottom:10px">
          <label style="display:block;margin-bottom:4px">Contraseña</label>
          <input v-model="registro.clave" type="password" placeholder="Ingrese su contraseña" style="width:100%;padding:8px" />
        </div>

        <button @click="registrarUsuario" style="width:100%;padding:8px;margin-bottom:10px">Registrar</button>
        <button @click="vista = 'login'; mensaje = ''" style="width:100%;padding:8px">Volver al login</button>
      </div>

      <p v-if="mensaje" style="margin-top:15px">{{ mensaje }}</p>
    </div>

    <div v-else style="max-width:1150px;margin:auto">
      <div style="border:1px solid #000;padding:15px;margin-bottom:20px">
        <b>Bienvenido {{ u.nombre }}</b>
        <button @click="cerrarSesion" style="float:right">Salir</button>
      </div>

      <div style="margin-bottom:15px">
        <button @click="cambiarModulo('productos')">Productos</button>
        <button @click="cambiarModulo('departamentos')" style="margin-left:5px">Departamentos</button>
        <button @click="cambiarModulo('equipos')" style="margin-left:5px">Equipos</button>
        <button @click="cambiarModulo('reparaciones')" style="margin-left:5px">Reparaciones</button>
      </div>

      <!-- PRODUCTOS -->
      <div v-if="modulo === 'productos'" style="border:1px solid #000;padding:15px;margin-bottom:20px">
        <h3>Productos</h3>

        <div style="display:flex;flex-wrap:wrap;gap:10px;align-items:end;margin-bottom:15px">
          <div>
            <label style="display:block;margin-bottom:4px">Nombre del producto</label>
            <input v-model="producto.nombre" placeholder="Ingrese el nombre del producto" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Precio</label>
            <input v-model.number="producto.precio" type="number" placeholder="Ingrese el precio" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Stock</label>
            <input v-model.number="producto.stock" type="number" placeholder="Ingrese el stock" style="padding:8px;width:220px" />
          </div>

          <div>
            <button @click="guardarProducto">{{ editandoProducto ? 'Actualizar' : 'Guardar' }}</button>
            <button @click="limpiarProducto" style="margin-left:5px">Limpiar</button>
          </div>
        </div>

        <table border="1" width="100%" style="border-collapse:collapse;text-align:center">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Precio</th>
              <th>Stock</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="x in productos" :key="x.id">
              <td>{{ x.id }}</td>
              <td>{{ x.nombre }}</td>
              <td>{{ x.precio }}</td>
              <td>{{ x.stock }}</td>
              <td>
                <button @click="editarProducto(x)">Editar</button>
                <button @click="eliminarProducto(x.id)">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- DEPARTAMENTOS -->
      <div v-if="modulo === 'departamentos'" style="border:1px solid #000;padding:15px;margin-bottom:20px">
        <h3>Departamentos</h3>

        <div style="display:flex;flex-wrap:wrap;gap:10px;align-items:end;margin-bottom:15px">
          <div>
            <label style="display:block;margin-bottom:4px">Nombre del departamento</label>
            <input v-model="departamento.nombre" placeholder="Ingrese el nombre del departamento" style="padding:8px;width:240px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Presupuesto asignado</label>
            <input v-model.number="departamento.presupuestoAsignado" type="number" placeholder="Ingrese el presupuesto" style="padding:8px;width:240px" />
          </div>

          <div>
            <button @click="guardarDepartamento">{{ editandoDepartamento ? 'Actualizar' : 'Guardar' }}</button>
            <button @click="limpiarDepartamento" style="margin-left:5px">Limpiar</button>
          </div>
        </div>

        <table border="1" width="100%" style="border-collapse:collapse;text-align:center">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Presupuesto</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="x in departamentos" :key="x.id">
              <td>{{ x.id }}</td>
              <td>{{ x.nombre }}</td>
              <td>{{ x.presupuestoAsignado }}</td>
              <td>
                <button @click="editarDepartamento(x)">Editar</button>
                <button @click="eliminarDepartamento(x.id)">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- EQUIPOS -->
      <div v-if="modulo === 'equipos'" style="border:1px solid #000;padding:15px;margin-bottom:20px">
        <h3>Equipos</h3>

        <div style="display:flex;flex-wrap:wrap;gap:10px;align-items:end;margin-bottom:15px">
          <div>
            <label style="display:block;margin-bottom:4px">Nombre del equipo</label>
            <input v-model="equipo.nombre" placeholder="Ingrese el nombre del equipo" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Tipo de equipo</label>
            <input v-model="equipo.tipo" placeholder="Ingrese el tipo de equipo" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Marca del equipo</label>
            <input v-model="equipo.marca" placeholder="Ingrese la marca del equipo" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Fecha de compra</label>
            <input v-model="equipo.fechaCompra" type="date" style="padding:8px;width:170px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Costo inicial</label>
            <input v-model.number="equipo.costoInicial" type="number" placeholder="Ingrese el costo inicial" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Vida útil en meses</label>
            <input v-model.number="equipo.vidaUtilMeses" type="number" placeholder="Ingrese la vida útil" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Estado del equipo</label>
            <input v-model="equipo.estado" placeholder="Ejemplo: Activo" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Departamento</label>
            <select v-model="equipo.departamentoId" style="padding:8px;width:220px">
              <option disabled value="">Seleccione departamento</option>
              <option v-for="d in departamentos" :key="d.id" :value="d.id">{{ d.nombre }}</option>
            </select>
          </div>

          <div>
            <button @click="guardarEquipo">{{ editandoEquipo ? 'Actualizar' : 'Guardar' }}</button>
            <button @click="limpiarEquipo" style="margin-left:5px">Limpiar</button>
          </div>
        </div>

        <table border="1" width="100%" style="border-collapse:collapse;text-align:center">
          <thead>
            <tr>
              <th>ID</th>
              <th>Nombre</th>
              <th>Tipo</th>
              <th>Marca</th>
              <th>Departamento</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="x in equipos" :key="x.id">
              <td>{{ x.id }}</td>
              <td>{{ x.nombre }}</td>
              <td>{{ x.tipo }}</td>
              <td>{{ x.marca }}</td>
              <td>{{ x.departamento?.nombre }}</td>
              <td>
                <button @click="editarEquipo(x)">Editar</button>
                <button @click="eliminarEquipo(x.id)">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <!-- REPARACIONES -->
      <div v-if="modulo === 'reparaciones'" style="border:1px solid #000;padding:15px;margin-bottom:20px">
        <h3>Reparaciones</h3>

        <div style="display:flex;flex-wrap:wrap;gap:10px;align-items:end;margin-bottom:15px">
          <div>
            <label style="display:block;margin-bottom:4px">Fecha de reparación</label>
            <input v-model="reparacion.fechaReparacion" type="date" style="padding:8px;width:170px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Descripción</label>
            <input v-model="reparacion.descripcion" placeholder="Ingrese la descripción de la reparación" style="padding:8px;width:260px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Costo de reparación</label>
            <input v-model.number="reparacion.costo" type="number" placeholder="Ingrese el costo" style="padding:8px;width:220px" />
          </div>

          <div>
            <label style="display:block;margin-bottom:4px">Equipo</label>
            <select v-model="reparacion.equipoId" style="padding:8px;width:220px">
              <option disabled value="">Seleccione equipo</option>
              <option v-for="e in equipos" :key="e.id" :value="e.id">{{ e.nombre }}</option>
            </select>
          </div>

          <div>
            <button @click="guardarReparacion">{{ editandoReparacion ? 'Actualizar' : 'Guardar' }}</button>
            <button @click="limpiarReparacion" style="margin-left:5px">Limpiar</button>
          </div>
        </div>

        <table border="1" width="100%" style="border-collapse:collapse;text-align:center">
          <thead>
            <tr>
              <th>ID</th>
              <th>Fecha</th>
              <th>Descripción</th>
              <th>Costo</th>
              <th>Equipo</th>
              <th>Acciones</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="x in reparaciones" :key="x.id">
              <td>{{ x.id }}</td>
              <td>{{ x.fechaReparacion }}</td>
              <td>{{ x.descripcion }}</td>
              <td>{{ x.costo }}</td>
              <td>{{ x.equipo?.nombre }}</td>
              <td>
                <button @click="editarReparacion(x)">Editar</button>
                <button @click="eliminarReparacion(x.id)">Eliminar</button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <p v-if="mensaje" style="margin-top:15px">{{ mensaje }}</p>
    </div>
  </div>
</template>

<script>
  import axios from 'axios'

  export default {
    data() {
      return {
        vista: 'login',
        modulo: 'productos',
        mensaje: '',
        u: JSON.parse(localStorage.getItem('usuario')),

        login: { correo: '', clave: '' },
        registro: { nombre: '', correo: '', clave: '' },

        productos: [],
        producto: { id: 0, nombre: '', precio: '', stock: '' },
        editandoProducto: false,

        departamentos: [],
        departamento: { id: 0, nombre: '', presupuestoAsignado: '' },
        editandoDepartamento: false,

        equipos: [],
        equipo: {
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
        editandoEquipo: false,

        reparaciones: [],
        reparacion: {
          id: 0,
          fechaReparacion: '',
          descripcion: '',
          costo: '',
          equipoId: ''
        },
        editandoReparacion: false,

        apiA: '/api/Auth',
        apiP: '/api/Productos',
        apiD: '/api/Departamentos',
        apiE: '/api/Equipos',
        apiR: '/api/Reparaciones'
      }
    },

    methods: {
      cambiarModulo(nombre) {
        this.modulo = nombre
        this.mensaje = ''
      },

      async registrarUsuario() {
        try {
          await axios.post(`${this.apiA}/registrar`, this.registro)
          this.mensaje = 'Usuario registrado correctamente'
          this.registro = { nombre: '', correo: '', clave: '' }
          this.vista = 'login'
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al registrar'
        }
      },

      async iniciarSesion() {
        try {
          let r = await axios.post(`${this.apiA}/login`, this.login)
          this.u = r.data.usuario
          localStorage.setItem('usuario', JSON.stringify(this.u))
          this.mensaje = ''
          await this.cargarTodo()
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al iniciar sesión'
        }
      },

      cerrarSesion() {
        this.u = null
        localStorage.removeItem('usuario')
        this.mensaje = ''
        this.vista = 'login'
      },

      async cargarTodo() {
        await this.obtenerProductos()
        await this.obtenerDepartamentos()
        await this.obtenerEquipos()
        await this.obtenerReparaciones()
      },

      async obtenerProductos() {
        const r = await axios.get(this.apiP)
        this.productos = r.data
      },

      async guardarProducto() {
        if (this.editandoProducto) await axios.put(`${this.apiP}/${this.producto.id}`, this.producto)
        else await axios.post(this.apiP, this.producto)
        this.limpiarProducto()
        await this.obtenerProductos()
      },

      editarProducto(x) {
        this.producto = { ...x }
        this.editandoProducto = true
      },

      async eliminarProducto(id) {
        await axios.delete(`${this.apiP}/${id}`)
        await this.obtenerProductos()
      },

      limpiarProducto() {
        this.producto = { id: 0, nombre: '', precio: '', stock: '' }
        this.editandoProducto = false
      },

      async obtenerDepartamentos() {
        const r = await axios.get(this.apiD)
        this.departamentos = r.data
      },

      async guardarDepartamento() {
        try {
          console.log('ENVIANDO DEPARTAMENTO:', this.departamento)

          if (!this.departamento.nombre || this.departamento.nombre.trim() === '') {
            this.mensaje = 'Debe ingresar el nombre del departamento'
            return
          }

          if (this.departamento.presupuestoAsignado === '' || this.departamento.presupuestoAsignado <= 0) {
            this.mensaje = 'El presupuesto debe ser mayor a cero'
            return
          }

          if (this.editandoDepartamento) {
            await axios.put(`${this.apiD}/${this.departamento.id}`, this.departamento)
            this.mensaje = 'Departamento actualizado correctamente'
          } else {
            await axios.post(`${this.apiD}`, this.departamento)
            this.mensaje = 'Departamento guardado correctamente'
          }

          this.limpiarDepartamento()
          await this.obtenerDepartamentos()
        } catch (e) {
          console.log('ERROR DEPARTAMENTO COMPLETO:', e)
          console.log('ERROR DEPARTAMENTO RESPONSE:', e.response?.data)
          this.mensaje = e.response?.data?.mensaje || 'Error al guardar departamento'
        }
      },

      editarDepartamento(x) {
        this.departamento = { ...x }
        this.editandoDepartamento = true
      },

      async eliminarDepartamento(id) {
        await axios.delete(`${this.apiD}/${id}`)
        await this.obtenerDepartamentos()
      },

      limpiarDepartamento() {
        this.departamento = { id: 0, nombre: '', presupuestoAsignado: '' }
        this.editandoDepartamento = false
      },

      async obtenerEquipos() {
        const r = await axios.get(this.apiE)
        this.equipos = r.data
      },

      async guardarEquipo() {
        try {
          if (this.editandoEquipo) await axios.put(`${this.apiE}/${this.equipo.id}`, this.equipo)
          else await axios.post(this.apiE, this.equipo)
          this.limpiarEquipo()
          await this.obtenerEquipos()
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al guardar equipo'
        }
      },

      editarEquipo(x) {
        this.equipo = {
          id: x.id,
          nombre: x.nombre,
          tipo: x.tipo,
          marca: x.marca,
          fechaCompra: x.fechaCompra?.split('T')[0] || '',
          costoInicial: x.costoInicial,
          vidaUtilMeses: x.vidaUtilMeses,
          estado: x.estado,
          departamentoId: x.departamentoId
        }
        this.editandoEquipo = true
      },

      async eliminarEquipo(id) {
        await axios.delete(`${this.apiE}/${id}`)
        await this.obtenerEquipos()
      },

      limpiarEquipo() {
        this.equipo = {
          id: 0,
          nombre: '',
          tipo: '',
          marca: '',
          fechaCompra: '',
          costoInicial: '',
          vidaUtilMeses: '',
          estado: '',
          departamentoId: ''
        }
        this.editandoEquipo = false
      },

      async obtenerReparaciones() {
        const r = await axios.get(this.apiR)
        this.reparaciones = r.data
      },

      async guardarReparacion() {
        try {
          if (this.editandoReparacion) await axios.put(`${this.apiR}/${this.reparacion.id}`, this.reparacion)
          else await axios.post(this.apiR, this.reparacion)
          this.limpiarReparacion()
          await this.obtenerReparaciones()
        } catch (e) {
          this.mensaje = e.response?.data?.mensaje || 'Error al guardar reparación'
        }
      },

      editarReparacion(x) {
        this.reparacion = {
          id: x.id,
          fechaReparacion: x.fechaReparacion?.split('T')[0] || '',
          descripcion: x.descripcion,
          costo: x.costo,
          equipoId: x.equipoId
        }
        this.editandoReparacion = true
      },

      async eliminarReparacion(id) {
        await axios.delete(`${this.apiR}/${id}`)
        await this.obtenerReparaciones()
      },

      limpiarReparacion() {
        this.reparacion = {
          id: 0,
          fechaReparacion: '',
          descripcion: '',
          costo: '',
          equipoId: ''
        }
        this.editandoReparacion = false
      }
    },

    mounted() {
      if (this.u) this.cargarTodo()
    }
  }
</script>
