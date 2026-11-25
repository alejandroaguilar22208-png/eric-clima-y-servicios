# ClimaGestión Escritorio — guía para probarlo y explicarlo

## 1. Cómo abrirlo (5 minutos)

1. Abrí **XAMPP** y dale **Start** a **MySQL**.
2. En **DBeaver**: nueva conexión MariaDB → `localhost`, puerto `3306`, usuario `root`, sin contraseña.
   Abrí `database/climagestion.sql` y ejecutalo entero (Alt+X). Crea la base con datos de prueba.
3. Doble clic en `escritorio/ClimaGestion.sln` → se abre en **Visual Studio**.
4. Apretá **F5**.
5. Usuario de prueba:
   - Email: `admin@climagestion.test`
   - Contraseña: `Clima2026!`

> La cadena de conexión está en `App.config` (`connectionStrings`). Si no está, `Modelo/Conexion.cs` usa los valores por defecto.

---

## 2. Qué hace (30 segundos)

> "Eric Clima es un técnico de aires de Bahía Blanca que anotaba todo en un cuaderno.
> Hice un programa en C# con MariaDB, con POO y MVC, para registrar clientes, trabajos, stock y presupuestos.
> Cuando un trabajo se marca como realizado, se descuentan los materiales y queda una garantía de 12 meses.
> Usa la misma base que la página web, así que los pedidos de la página aparecen solos en el programa."

## 3. Recorrido para la defensa

1. **Login**: primero una contraseña mala (no entra), después la buena. Mostrá que la contraseña sale con puntos y que Tab pasa de campo.
2. **Usuarios**: Nuevo → crear un técnico eligiendo el tipo en el ComboBox. Cerrar sesión (Archivo → Cerrar sesión) y entrar con el técnico: no ve Usuarios ni puede editar el stock.
3. **Clientes**: buscar escribiendo; Nuevo con el nombre vacío para mostrar el ErrorProvider.
4. **Trabajos**: Nuevo trabajo → Agendar → Agregar material (probá 50 kg de gas: lo frena) → Marcar realizado. Queda verde, con garantía.
5. **Stock**: el material bajó; las filas rojas tienen stock bajo.
6. **Presupuestos**: Nuevo → cliente → agregar ítems → Guardar → Imprimir. Rechazar uno pide confirmación y lo borra.
7. **Salir**: pregunta "Cerrar el sistema, ¿confirma?".

---

## 4. MVC: cómo está organizado

| Carpeta | Qué tiene | Ejemplo |
| --- | --- | --- |
| `Entidades/` | Las clases con los datos: atributos privados y propiedades | `Persona`, `Cliente`, `Usuario`, `Trabajo`, `Material` |
| `Vista/` | Los formularios. Muestran y juntan datos. **No tienen SQL.** | `FrmLogin`, `FrmTrabajos`, `FrmClienteEditar` |
| `Control/` | Las reglas del negocio. Devuelven `""` si está todo bien o el mensaje de error. | `ControlTrabajos.marcarRealizado(id)` |
| `Modelo/` | La conexión y las consultas SQL con parámetros. | `Conexion.getConexion()`, `ModeloTrabajos.listar(estado)` |

**El camino de un clic (Guardar cliente):**

```
FrmClienteEditar (Vista)     → arma un objeto Cliente con lo que se escribió
   ↓
ControlClientes (Control)    → cliente.Validar(); si falta algo devuelve el mensaje
   ↓
ModeloClientes (Modelo)      → INSERT INTO clientes ... con @parámetros
   ↓
MariaDB guarda la fila → la Vista muestra el resultado y recarga la grilla
```

El login sigue el ejemplo de clase: `Usuario u = new Usuario(email, password)` → `ControlLogin.usuarioValido(u)` → `ModeloUsuarios.buscarUsuario(email)`.

---

## 5. POO: dónde está cada concepto

| Concepto | Dónde | Qué decir |
| --- | --- | --- |
| Encapsulamiento | Todas las entidades | Atributos `private` y propiedades `get` / `set`. |
| Herencia | `Persona` → `Cliente` y `Usuario` | Id, nombre, teléfono y email están una sola vez. |
| Abstracción | `public abstract class Persona` | No se crea "una persona": es cliente o usuario. |
| Polimorfismo | `Validar()` `virtual` en Persona, `override` en las hijas | Cada clase valida distinto, pero se llama igual. |
| Composición | `Presupuesto` tiene `List<ItemPresupuesto>` | `Total()` suma los `Subtotal()`. |
| Sobrecarga de constructores | `Usuario()` y `Usuario(email, password)` | El segundo se usa en el login. |
| Miembros estáticos | `Sesion.UsuarioActual`, `Consultas.tabla()` | Pertenecen a la clase, no a un objeto. |

---

## 6. Base de datos

- 14 tablas. `tipo_usuarios` guarda los tipos (1 Administrador, 2 Técnico) y `usuarios.tipo_usuario_id` apunta ahí.
- **Combo con consulta SQL** (como en el apunte): `cboTipo.ValueMember = "id"; cboTipo.DisplayMember = "nombre"; cboTipo.DataSource = control.tipos();`
- **DataGridView**: `dgvDatos.DataSource = control.listar();` y se oculta la columna `id`.
- **Transacción** (`ModeloTrabajos.marcarRealizado`): descuenta cada material, cambia el estado y pone la garantía. Si algo falla, `Rollback()` y no cambia nada.
- **UNIQUE** `uq_tecnico_horario`: un técnico no puede tener dos trabajos a la misma hora.

## 7. Errores controlados

- `Program.cs` atrapa cualquier excepción no prevista (`Application.ThreadException`) y muestra un mensaje: el programa no se cierra de golpe.
- `Control/Errores.cs` traduce los errores de MySQL: email repetido, técnico ocupado, base apagada.
- Cerrar con la X, Salir o Cerrar sesión siempre piden confirmación (`MessageBoxButtons.YesNoCancel`).

## 8. Preguntas típicas

- **¿Por qué la contraseña no se ve en la base?** Se guarda el hash bcrypt (`BCrypt.HashPassword`) y al entrar se compara con `BCrypt.Verify`. Es compatible con `password_hash` de PHP, por eso sirve el mismo usuario en la web.
- **¿Qué evita la inyección SQL?** Los parámetros (`@email`, `@id`): el texto nunca se pega dentro del SQL.
- **¿Dónde está el manual?** F1, botón Ayuda o menú Ayuda. Es `Manual.pdf`, al lado del `.exe`.
- **¿Cómo se imprime el presupuesto?** Con `PrintDocument` y `PrintPreviewDialog`. Para PDF se elige la impresora "Microsoft Print to PDF".
- **¿Cómo se conecta con la web?** No se mandan nada: los dos usan la misma base `climagestion`.

## 9. Instalador

`escritorio/instalador/crear-instalador.bat` compila en Release con MSBuild y arma `Instalar-ClimaGestion.exe` con Inno Setup. Necesita .NET Framework 4.8, que ya viene con Windows 10 y 11.
