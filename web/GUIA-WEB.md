# Eric Clima y Servicios — Web: guía para usar y defender

## 1. Cómo levantarla

**Opción A — XAMPP (la que vas a mostrar):**

1. Copiá la carpeta `web` a `C:\xampp\htdocs\` y renombrala `ericclima`.
2. En XAMPP dale Start a **Apache** y **MySQL**.
3. Si todavía no está la base: abrí `http://localhost/phpmyadmin` → Importar → `database/climagestion.sql`.
4. Sitio público: `http://localhost/ericclima/public/`
5. Panel: `http://localhost/ericclima/admin/` (mismo usuario de prueba que escritorio, está en `escritorio/GUIA.md`).

**Opción B — sin copiar nada (para programar):**

```bash
cd "C:\tareas chila\proyecto\web"
C:\xampp\php\php.exe -S localhost:8080 -t .
```
y abrís `http://localhost:8080/`.

---

## 2. Qué decir en 30 segundos

> "Es el sitio de Eric Clima y Servicios. Tiene una **parte pública** donde el cliente ve los servicios
> (que salen de la base de datos), los **equipos en venta** con una calculadora de frigorías, cómo trabajamos,
> quiénes somos y un **formulario para pedir presupuesto**.
> Ese pedido entra al **panel privado**, donde el técnico lo convierte en cliente y trabajo, lo agenda,
> carga los materiales y lo cierra. Al cerrarlo, la base **descuenta el stock y pone 12 meses de garantía**.
> Usa la **misma base de datos** que el sistema de escritorio en C#."

## 3. Recorrido para la defensa (5 minutos)

1. **Sitio público** → mostrá servicios, preguntas y el botón de WhatsApp. Achicá la ventana: se adapta al celular.
2. **Formulario** → mandalo vacío (marca en rojo) y después completo (mensaje de gracias).
3. **Panel → Login** con una clave mala y después con la buena.
4. **Inicio** → la solicitud nueva aparece contada; alertas de stock bajo.
5. **Solicitudes** → "Convertir en trabajo".
6. **Trabajos** → abrir el trabajo → intentar "Marcar realizado" sin agendar (lo frena) → agendar →
   cargar 50 kg de gas (lo frena por stock) → cargar 3 m de caño → marcar realizado.
7. **Presupuestos** → Nuevo → cliente + "Agregar rápido" (el total se calcula solo) → Guardar →
   se abre la hoja del presupuesto → **Imprimir / Guardar PDF** → "Guardar como PDF".
8. **Stock** → el caño bajó. **Clientes** → ficha con equipos e historial. **Reportes** → ingresos.
9. **Salir** y probar entrar a `admin/clientes.php` sin sesión: te manda al login.

---

## 4. Cómo está organizada

| Carpeta / archivo | Qué hace |
| --- | --- |
| `app/config.php` | Conexión PDO, datos del negocio, funciones comunes (escapar HTML, CSRF, mensajes). |
| `app/auth.php` | Login, sesión y roles (`requerir_login`, `requerir_admin`). |
| `app/layout.php` | Encabezado con menú y pie del panel, para no repetirlos en cada página. |
| `public/index.php` | Sitio público (lee servicios y equipos en venta de la base). |
| `admin/equipos.php` | Alta, edición y ocultar/mostrar los equipos en venta (solo admin modifica). |
| `public/solicitud.php` | Recibe el formulario, valida y guarda en `solicitudes`. |
| `admin/*.php` | Una página por módulo: inicio, solicitudes, clientes, trabajos, stock, reportes. |
| `admin/presupuestos.php` | Lista, alta con ítems (en una transacción) y aceptar/rechazar. |
| `admin/presupuesto.php` | La hoja del presupuesto lista para imprimir o guardar como PDF (`@media print`). |
| `api/trabajos.php` | API REST en JSON para las apps (pide la clave `X-API-Key`). |
| `assets/css/estilos.css` | Colores de la marca. |

**Cada página del panel sigue el mismo esquema:**
1. `requerir_login()` → si no hay sesión, al login.
2. Si llegó un `POST` → verifica el token CSRF, valida, hace el `INSERT`/`UPDATE` y **redirige** (así al recargar no se duplica).
3. Si es `GET` → consulta la base y muestra el HTML.

---

## 5. Seguridad (la pregunta segura)

| Riesgo | Cómo se evita | Dónde |
| --- | --- | --- |
| Inyección SQL | Consultas preparadas con `?` (PDO) | todas las consultas |
| XSS (meter código en un formulario) | Todo lo que se muestra pasa por `e()` = `htmlspecialchars` | `app/config.php` |
| Contraseñas robadas | Se guardan con `password_hash` (bcrypt); se comparan con `password_verify` | `app/auth.php` |
| Formularios falsos de otro sitio (CSRF) | Token secreto en cada formulario | `csrf_campo()` / `csrf_verificar()` |
| Robo de sesión | `session_regenerate_id()` al ingresar | `iniciar_sesion()` |
| Spam en el formulario | Campo trampa invisible (`sitio_web`) | `public/solicitud.php` |
| Entrar sin permiso | `requerir_login()` y `requerir_admin()` | cada página del panel |

---

## 6. Preguntas típicas

- **¿Por qué es "dinámico"?** Porque el contenido sale de la base: si cambian los servicios o precios en MySQL, la página cambia sola.
- **¿Qué es PDO?** La forma de PHP de hablar con la base usando consultas preparadas.
- **¿Qué es una transacción?** Al convertir una solicitud se crean cliente + trabajo + se actualiza la solicitud. Si algo falla, se deshace todo (`rollBack`).
- **¿Cómo sacan el PDF en la web?** La página `presupuesto.php` tiene un CSS especial para imprimir (`@media print`) que oculta los botones. El navegador lo guarda como PDF con "Imprimir → Guardar como PDF".
- **¿Para qué la API?** Para que la app móvil o la de escritorio puedan leer los trabajos en JSON. Ejemplo: `api/trabajos.php?estado=agendado`.
- **¿Validan en el navegador o en el servidor?** En los dos: JavaScript avisa rápido, pero el servidor vuelve a validar porque el navegador se puede saltear.

## 7. Datos a cambiar por los reales

En `app/config.php`: `TELEFONO`, `WHATSAPP`, `EMAIL` (ahora tienen datos de prueba). Los equipos en venta se cargan desde el panel, en **Equipos en venta** (tabla `equipos_venta`).

## 8. Equipos en venta

- **¿De dónde salen los equipos de la página?** De la tabla `equipos_venta`. Se cargan desde el panel; si un equipo se oculta, deja de verse en la web pero no se borra.
- **¿Cómo funciona la calculadora de frigorías?** Con JavaScript: metros² × 2,6 m de alto × 50 frigorías (20 % más si le da el sol). Marca como *Recomendado* los equipos que alcanzan esa potencia.
- **¿Qué pasa al tocar "Consultar"?** Completa el mensaje del formulario con la marca y el modelo, así el pedido llega al panel como una solicitud más.

## 9. Cuenta y borrado de rechazados

- **Mi cuenta** (`admin/cuenta.php`): cambia nombre, email y contraseña. Pide la contraseña actual y guarda la nueva con `password_hash()`. Sirve también para entrar al programa de escritorio.
- **Rechazar un presupuesto o descartar una solicitud** pide confirmación con el nombre de la persona y después hace un `DELETE`, así desaparece de la web y del escritorio.

