# Eric Clima y Servicios — Guía rápida para entender y defender el sistema

## 1. Cómo abrirlo y probarlo (5 minutos)

1. Abrí **XAMPP** y dale **Start** a **MySQL**.
2. Abrí **DBeaver** → nueva conexión MySQL/MariaDB → `localhost`, puerto `3306`, usuario `root`, sin contraseña.
   Abrí y ejecutá el archivo `database/climagestion.sql` (Alt+X). Crea la base con datos de prueba.
3. Doble clic en `escritorio/EricClima.slnx` → se abre en **Visual Studio**.
4. Apretá **F5** (o el botón ▶ verde).
5. Ingresá con el usuario de prueba:
   - Email: `admin@climagestion.test`
   - Contraseña: `Clima2026!`

> La conexión a la base está en `Data/Conexion.cs`. Si cambia la PC o la contraseña, se toca solo ahí.

---

## 2. Qué hace el sistema (lo que le decís al profe en 30 segundos)

> "Eric Clima y Servicios es un técnico de aires de Bahía Blanca que anotaba todo en un cuaderno y en WhatsApp.
> Hice un sistema de escritorio en C# con MySQL para que registre **clientes**, **trabajos** y **stock de materiales**.
> Cuando un trabajo se marca como realizado, el sistema **descuenta solo los materiales** y le pone
> **12 meses de garantía**. En la pantalla principal muestra **alertas** de stock bajo y trabajos del día."

Recorrido para mostrar en la defensa:

1. **Login** → probá primero una contraseña mala (no entra) y después la buena.
2. **Clientes** → buscá escribiendo, creá uno nuevo, dejá el nombre vacío para mostrar la validación.
3. **Trabajos** → Nuevo trabajo → Agendar (fecha y técnico) → Agregar material → Marcar realizado.
   Queda en verde y con fecha de garantía.
4. **Stock** → el material usado bajó. Las filas en **rojo** tienen stock bajo.
5. **Presupuestos** → Nuevo → elegí cliente, agregá ítems desde la lista (servicio o material) →
   Guardar → "¿Generar el PDF?" → Sí. Se abre el PDF con logo, ítems, total y vencimiento (15 días).
   Después marcalo **Aceptado** (queda verde) e intentá rechazarlo: no deja.
6. Volvé al menú → aparecen las **alertas**.

---

## 3. MVC: cómo está organizado (la pregunta segura del profe)

| Carpeta | Letra | Qué tiene | Ejemplo |
| --- | --- | --- | --- |
| `Models/` | **M** — Modelo | Las clases que representan los datos. No saben de ventanas ni de SQL. | `Cliente`, `Trabajo`, `Material` |
| `Views/` | **V** — Vista | Los formularios. Solo muestran datos y capturan clics. **Nunca escriben SQL.** | `FrmClientes`, `FrmTrabajos` |
| `Controllers/` | **C** — Controlador | Reciben lo que pide la vista, aplican las reglas y llaman a los repositorios. | `ClienteController.Guardar()` |
| `Data/` | Acceso a datos | Todo el SQL. Un repositorio por tabla. | `ClienteRepositorio.Insertar()` |

**El camino de un clic** (explicalo con el botón Guardar cliente):

```
FrmClienteEditar (Vista)        → el usuario aprieta "Guardar"
   ↓ arma un objeto Cliente con lo que escribió
ClienteController (Controlador) → cliente.Validar()  ¿falta algo? → devuelve error
   ↓ si está todo bien
ClienteRepositorio (Datos)      → INSERT INTO clientes ...
   ↓
MySQL                           → guarda la fila
   ↓ vuelve un Resultado (Ok + mensaje)
La Vista muestra el mensaje y recarga la grilla
```

---

## 4. Programación orientada a objetos: dónde está cada principio

| Principio | Dónde mostrarlo | Qué decir |
| --- | --- | --- |
| **Encapsulamiento** | `Models/Modelos.cs` | Los datos están en propiedades y cada clase tiene sus métodos (`Material.StockBajo()`). |
| **Herencia** | `Persona` → `Cliente` y `Usuario` | Nombre, teléfono y email están una sola vez en `Persona`, y los hijos los heredan. |
| **Abstracción** | `public abstract class Persona` | No existe "una persona" suelta: siempre es un cliente o un usuario. |
| **Polimorfismo** | `Validar()` es `virtual` en Persona y `override` en Cliente | Cada clase valida a su manera, pero se llama igual. |
| **Interfaz** | `IRepositorio<T>` en `Data/Conexion.cs` | Todos los repositorios prometen Listar, Insertar y Actualizar. |

---

## 5. Reglas de negocio y dónde se controlan

Están en **dos lugares**: en el controlador (C#), que avisa lindo, y en la base (triggers), que es la última barrera.

| Regla | En C# | En MySQL |
| --- | --- | --- |
| No hay trabajo sin cliente | `TrabajoController.Crear` | Clave foránea `fk_trabajos_cliente` |
| No usar más material del que hay | `TrabajoController.AgregarMaterial` | Trigger `trg_tm_validar_stock` |
| Para cerrar un trabajo hace falta fecha y técnico | `TrabajoController.MarcarRealizado` | Trigger `trg_trabajos_estado` |
| Al cerrar, se descuenta stock y garantía de 12 meses | — | Triggers `trg_trabajos_estado` y `trg_trabajos_descontar_stock` |
| Un trabajo cerrado no se modifica | `Trabajo.EstaCerrado()` | Trigger `trg_trabajos_estado` |
| Un técnico no puede tener 2 trabajos a la misma hora | — | Índice único `uq_tecnico_horario` |
| Los clientes no se borran, se dan de baja | `ClienteRepositorio.Desactivar` | Columna `activo` |
| Solo el admin da de baja y toca el stock | `Sesion.EsAdmin` | — |
| El presupuesto vence a los 15 días si no se indica fecha | `PresupuestoController.Crear` | Trigger `trg_presupuestos_vence` |
| Solo se acepta o rechaza un presupuesto pendiente | `PresupuestoController.CambiarEstado` | — |
| Presupuesto + ítems se guardan juntos o nada | `PresupuestoRepositorio.Insertar` (transacción) | — |

---

## 6. Seguridad (siempre lo preguntan)

- **Contraseñas con hash bcrypt.** En la base no está la contraseña, está `$2y$10$...`. Se compara con `BCrypt.Verify()`.
- **Consultas con parámetros** (`@nombre`, `@id`) en vez de pegar texto. Eso evita la **inyección SQL**.
- **Mismo mensaje** si falla el usuario o la contraseña, para no darle pistas a un intruso.

---

## 7. Preguntas típicas y respuesta corta

- **¿Por qué C# y MySQL?** Lo pide la materia. Son gratuitos y MySQL lo comparte con la página web.
- **¿Qué es un trigger?** Código que la base ejecuta sola cuando pasa algo (un INSERT o un UPDATE). Lo uso para que el stock se descuente siempre, aunque se cambie el dato desde otro lado.
- **¿Qué es una clave foránea?** Un campo que apunta a otra tabla. `trabajos.cliente_id` apunta a `clientes.id` y no deja crear un trabajo para un cliente que no existe.
- **¿Por qué `trabajo_material` es una tabla aparte?** Porque un trabajo usa muchos materiales y un material se usa en muchos trabajos (N a N). Eso lo pide la normalización (1FN).
- **¿Qué pasa si se cae la base?** El controlador atrapa el error (`try/catch`) y muestra un mensaje en vez de cerrarse.
- **¿Dónde está el manual?** Adentro del sistema: **F1** o el botón **Ayuda**. Es `Manual.pdf`; no es lineal: el índice y los recuadros "Ver también" son vínculos para saltar al tema. Las capturas están también en `docs/capturas/`.
- **¿Cómo hacés el PDF?** Con la librería **QuestPDF** (NuGet, licencia Community gratuita). Está en `Views/PresupuestoPdf.cs`: arma encabezado, tabla de ítems, total y pie con número de página.
- **¿Qué es una transacción?** Al guardar un presupuesto se insertan el encabezado y los ítems. Si falla uno, `Rollback()` deshace todo para que no quede un presupuesto a medias.
- **¿Cómo se conecta con la web?** Las dos usan la misma base `climagestion`. Lo que entra por el formulario web queda en `solicitudes`.

---

## 8. Archivos para tener a mano

```
escritorio/
├── EricClima.slnx                      ← abrir con Visual Studio
└── EricClima.Escritorio/
    ├── Program.cs                      ← arranca: login → menú
    ├── Models/Modelos.cs               ← M: Persona, Cliente, Usuario, Trabajo, Material, Servicio, Presupuesto
    ├── Controllers/Controladores.cs    ← C: reglas de negocio + Sesion + Resultado
    ├── Data/                           ← SQL: Conexion, IRepositorio y un repositorio por tabla
    └── Views/                          ← V: FrmLogin, FrmPrincipal, FrmClientes, FrmTrabajos, FrmMateriales, FrmPresupuestos, PresupuestoPdf, Ui
database/climagestion.sql               ← la base completa con datos de prueba
```

## 7 bis. Escritorio y web conectados

- **¿Cómo se comunican el escritorio y la web?** No se mandan nada entre ellos: los dos leen y escriben **la misma base** `climagestion`. Por eso lo que se carga en uno aparece en el otro.
- **Solicitudes web** (`FrmSolicitudes`, `SolicitudController`, `SolicitudRepositorio`): los pedidos del formulario de la página llegan solos; la ventana y el menú principal se refrescan cada 30 segundos con un `Timer`. *Convertir* crea el cliente y el trabajo dentro de una **transacción** (si algo falla, no queda nada a medias).
- **Equipos en venta** (`FrmEquiposVenta`, `EquipoVentaController`, `EquipoVentaRepositorio`, modelo `EquipoVenta` con `Validar()`): lo que se carga acá se ve al instante en la sección *Equipos en venta* de la web. Solo el administrador modifica.

## 7 ter. Cuenta, rechazos y descartes

- **¿Cómo se cambia el email o la contraseña?** Con **Mi cuenta** (en el menú del escritorio y arriba a la derecha en el panel web). Pide la contraseña actual, y la nueva tiene que tener 8 caracteres o más. Se guarda cifrada con bcrypt y sirve para los dos sistemas.
- **¿Qué pasa al rechazar un presupuesto o descartar una solicitud?** Pregunta "¿Seguro…?" con el nombre de la persona. Si se confirma, el registro se **borra** de la base (los ítems del presupuesto se van solos por `ON DELETE CASCADE`), así no se llenan las listas de rechazados.

## 8 bis. Instalarlo en otra computadora

- **¿Funciona en una notebook?** Sí. Es una aplicación de escritorio de Windows: corre igual en una notebook que en una PC de escritorio, con Windows 10 u 11 de 64 bits.
- **¿Hay que instalar .NET?** No. El instalador lleva el programa *autocontenido* (`--self-contained`): trae .NET adentro.
- **¿Cómo se arma el instalador?** Con `escritorio/instalador/crear-instalador.bat`: hace `dotnet publish` y después compila `ClimaGestion.iss` con Inno Setup. Sale `Instalar-ClimaGestion.exe`.
- **¿Qué instala?** El programa, el manual, el script de la base y accesos directos en el menú Inicio y, si se elige, en el escritorio. Se desinstala desde Configuración de Windows.
- **¿Y la base de datos?** Puede estar en la misma computadora (XAMPP) o en otra de la red. La cadena de conexión se lee del archivo `conexion.txt` que está al lado del programa (`Conexion.LeerCadena()`), así no hay que recompilar para cambiar de servidor.

## 9. Lo que falta (si te da el tiempo)

- [x] Presupuestos con PDF.
- [ ] Pantalla de proveedores.
- [ ] Bandeja de solicitudes que llegan de la web.
- [x] `Manual.pdf` (15 páginas, con capturas, índice con vínculos y "Ver también"). Se copia solo al lado del .exe y se abre con **F1** o el botón Ayuda.
