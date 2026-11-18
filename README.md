# Eric Clima y Servicios

Sistema de gestión para un servicio técnico de instalación y reparación de aires acondicionados de Bahía Blanca.
Proyecto final de 7.º 6.ª Programación — E.E.S.T. N.º 1 — 2026.
Alumno: Alejandro Aguilar Villan.

El sistema tiene dos partes que comparten **una misma base de datos MySQL/MariaDB**:

| Carpeta | Qué es | Tecnologías | Materia |
| --- | --- | --- | --- |
| `web/` | Sitio público con formulario de presupuesto + panel de gestión + API JSON | PHP 8, MySQL, Bootstrap 5 (XAMPP) | Proyecto Final Informático (Sitios Web Dinámicos) |
| `escritorio/` | Sistema de escritorio: clientes, trabajos, stock y presupuestos en PDF | C# .NET (WinForms), MVC, MySQL | Proyecto, diseño e implementación de sistemas computacionales |
| `database/` | Script de la base con tablas, triggers, vistas y datos de prueba | MySQL / MariaDB | Las dos |

## Instalación rápida

1. Instalar **XAMPP** e iniciar **Apache** y **MySQL**.
2. Importar `database/climagestion.sql` desde phpMyAdmin o DBeaver.
3. **Web:** copiar `web/` a `C:\xampp\htdocs\ericclima` y abrir `http://localhost/ericclima/`.
4. **Escritorio:** abrir `escritorio/EricClima.slnx` con Visual Studio y ejecutar con F5.

### Instalar el programa de escritorio en otra computadora

Funciona en cualquier notebook o PC de escritorio con Windows 10 u 11 de 64 bits, sin instalar .NET ni Visual Studio.

1. En la carpeta `escritorio/instalador/`, hacer doble clic en `crear-instalador.bat` (necesita .NET SDK e Inno Setup 6).
2. Se genera `escritorio/instalador/salida/Instalar-ClimaGestion.exe`: copiarlo a la otra computadora (pendrive, Drive) e instalarlo.
3. Esa computadora necesita la base: XAMPP con `climagestion.sql` importado, o apuntar a la base de otra PC de la red desde **Inicio → ClimaGestión → Configurar conexión a la base** (archivo `conexion.txt`).

El usuario de prueba está en `escritorio/GUIA.md`. Más detalles en `web/GUIA-WEB.md` y `escritorio/GUIA.md`.

## Funcionalidades

- Login con contraseñas cifradas (bcrypt) y roles (administrador / técnico).
- Clientes y equipos instalados, con historial de trabajos.
- Solicitudes desde la web que llegan solas al escritorio y se convierten en cliente + trabajo.
- Equipos en venta: se cargan desde el escritorio o el panel y se muestran en la página con una calculadora de frigorías.
- Trabajos con estados: pendiente → agendado → realizado / cancelado.
- Materiales usados por trabajo, con descuento automático de stock y alertas de stock bajo.
- Presupuestos con ítems, vencimiento automático a 15 días y exportación a PDF.
- Garantía de 12 meses calculada automáticamente.
- Reportes de ingresos por mes y por servicio.
- API REST de solo lectura para apps móviles.

## Documentación

| Archivo | Qué es |
| --- | --- |
| `docs/Documentacion-Escritorio.pdf` | Documentación del sistema de escritorio (índice vinculado, diagramas y manual). |
| `docs/Documentacion-Web.pdf` | Documentación del sitio web (17 puntos de la consigna, diagramas y capturas). |
| `escritorio/instalador/` | Script del instalador (Inno Setup) y el LEEME que se instala con el programa. |
| `escritorio/EricClima.Escritorio/Manual.pdf` | Manual de usuario con capturas; se abre desde el programa con F1. |
| `docs/diagramas/*.drawio` | MER, modelo relacional, diagrama de clases, DFD y mapa del sitio (draw.io). Sus imágenes están en `docs/diagramas/imagenes/`. |
| `docs/capturas/` y `docs/capturas-web/` | Capturas del programa de escritorio y del sitio web. |

