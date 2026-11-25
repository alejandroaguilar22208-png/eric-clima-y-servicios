# ClimaGestión Web — guía para probarla y explicarla

## 1. Cómo levantarla

1. Copiá la carpeta `web` a `C:\xampp\htdocs\` y renombrala `ericclima`.
2. En XAMPP dale Start a **Apache** y **MySQL**.
3. Importá `database/climagestion.sql` en phpMyAdmin (`http://localhost/phpmyadmin` → Importar).
4. Sitio público: `http://localhost/ericclima/`
5. Panel: `http://localhost/ericclima/admin/` (usuario de prueba en `escritorio/GUIA.md`).

## 2. Qué decir en 30 segundos

> "Es el sitio de Eric Clima y Servicios. La parte pública muestra servicios y equipos en venta que salen de la base,
> con una calculadora de frigorías y un formulario de pedido. El pedido entra al panel privado, donde se convierte
> en cliente y trabajo, se agenda, se cargan materiales y se cierra. Al cerrarlo se descuenta el stock y queda
> una garantía de 12 meses. Usa la misma base que el programa de escritorio en C#."

## 3. Recorrido para la defensa

1. **Sitio público**: servicios, equipos y calculadora. Achicá la ventana: se adapta al celular.
2. **Formulario**: vacío (marca los campos) y después completo.
3. **Login** con clave mala y después con la buena.
4. **Solicitudes** → Convertir en trabajo.
5. **Trabajos** → abrir → Marcar realizado sin agendar (lo frena) → agendar → cargar material de más (lo frena) → cargar 3 m de caño → realizado.
6. **Presupuestos** → Nuevo → Guardar → Imprimir / Guardar PDF.
7. **Usuarios** (solo admin) → crear un técnico. Entrar con él: no ve Reportes ni Usuarios.
8. **Salir** y abrir `admin/clientes.php` sin sesión: vuelve al login.

## 4. Cómo está organizada

| Carpeta / archivo | Qué hace |
| --- | --- |
| `config/db.php` | Datos de conexión y creación de `$pdo` (PDO) dentro de un `try/catch`. Datos del negocio. |
| `includes/funciones.php` | Funciones comunes: `h()`, `requerirLogin()`, `requerirAdmin()`, `iniciarSesion()`, `campoCsrf()`, `verificarCsrf()`, `flash()`… |
| `includes/header.php` y `footer.php` | Menú y pie del panel, para no repetirlos en cada página. |
| `index.php` | Página pública (lee servicios y equipos de la base). |
| `solicitud.php` | Recibe el formulario, valida y guarda en `solicitudes`. |
| `admin/*.php` | Una página por módulo: solicitudes, clientes, trabajos, materiales, presupuestos, equipos, reportes, usuarios, cuenta. |
| `admin/presupuesto.php` | Hoja del presupuesto para imprimir o guardar como PDF (`@media print`). |
| `api/trabajos.php` | API JSON de trabajos (pide la clave en `X-API-Key`). |

**Cada página del panel sigue el mismo esquema:**
1. `require '../includes/funciones.php';` y `requerirLogin();`
2. Si llegó un `POST`: `verificarCsrf()`, validar, `INSERT` / `UPDATE` con `$pdo->prepare()` y `redirigir()` (así al recargar no se duplica).
3. Si es `GET`: consultar y mostrar el HTML con `include '../includes/header.php';`.

## 5. Tipos de usuario

- Tabla `tipo_usuarios` (1 Administrador, 2 Técnico). `iniciarSesion()` hace un `JOIN` y guarda el tipo en la sesión.
- `esAdmin()` mira `tipo_usuario_id == 1`. `requerirAdmin()` protege `reportes.php` y `usuarios.php`.
- Un administrador no puede quitarse a sí mismo el tipo ni desactivarse.

## 6. Reglas de negocio y transacciones

Las reglas están en PHP (no en triggers). Las que tocan varias tablas usan transacción:

```php
$pdo->beginTransaction();
// descontar stock de cada material, poner realizado y garantía +12 meses
$pdo->commit();      // si algo falla: $pdo->rollBack();
```

Se usa en: marcar trabajo realizado (`trabajos.php`), convertir solicitud (`solicitudes.php`) y guardar presupuesto con ítems (`presupuestos.php`).

## 7. Seguridad

| Riesgo | Cómo se evita |
| --- | --- |
| Inyección SQL | `$pdo->prepare()` con `?` en todas las consultas |
| XSS | Todo lo que se muestra pasa por `h()` = `htmlspecialchars` |
| Contraseñas | `password_hash()` y `password_verify()` |
| CSRF | Token en cada formulario (`campoCsrf()` / `verificarCsrf()`) |
| Robo de sesión | `session_regenerate_id()` al ingresar |
| Spam | Campo trampa invisible `sitio_web` en el formulario |

## 8. Preguntas típicas

- **¿Por qué es dinámico?** Porque el contenido sale de la base: si cambia un precio en la base, cambia la página.
- **¿Qué es PDO?** La clase de PHP para hablar con la base con consultas preparadas.
- **¿Validan en el navegador o en el servidor?** En los dos: JavaScript avisa rápido, pero PHP vuelve a validar.
- **¿Cómo se conecta con el escritorio?** Usan la misma base `climagestion`; el mismo usuario sirve en los dos.
- **Datos del negocio**: `TELEFONO`, `WHATSAPP` y `EMAIL` están en `config/db.php`.
