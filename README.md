# Eric Clima y Servicios

Sistema de gestión para un servicio técnico de instalación y reparación de aires acondicionados de Bahía Blanca.
Proyecto final de 7.º 6.ª Programación — E.E.S.T. N.º 1 — 2026.

El sistema tiene dos partes que comparten **una misma base de datos MySQL/MariaDB**:

| Carpeta | Qué es | Tecnologías | Materia |
| --- | --- | --- | --- |
| `web/` | Sitio público con formulario de presupuesto + panel de gestión + API JSON | PHP 8, MySQL, Bootstrap 5 (XAMPP) | Proyecto Final Informático (Sitios Web Dinámicos) |
| `escritorio/` | Sistema de escritorio: clientes, trabajos y stock | C# .NET (WinForms), MVC, MySQL | Proyecto, diseño e implementación de sistemas computacionales |
| `database/` | Script de la base con tablas, triggers, vistas y datos de prueba | MySQL / MariaDB | Las dos |

## Instalación rápida

1. Instalar **XAMPP** e iniciar **Apache** y **MySQL**.
2. Importar `database/climagestion.sql` desde phpMyAdmin o DBeaver.
3. **Web:** copiar `web/` a `C:\xampp\htdocs\ericclima` y abrir `http://localhost/ericclima/`.
4. **Escritorio:** abrir `escritorio/EricClima.slnx` con Visual Studio y ejecutar con F5.

El usuario de prueba está en `escritorio/GUIA.md`. Más detalles en `web/GUIA-WEB.md` y `escritorio/GUIA.md`.

## Funcionalidades

- Login con contraseñas cifradas (bcrypt) y roles (administrador / técnico).
- Clientes y equipos instalados, con historial de trabajos.
- Solicitudes desde la web que se convierten en cliente + trabajo.
- Trabajos con estados: pendiente → agendado → realizado / cancelado.
- Materiales usados por trabajo, con descuento automático de stock y alertas de stock bajo.
- Garantía de 12 meses calculada automáticamente.
- Reportes de ingresos por mes y por servicio.
- API REST de solo lectura para apps móviles.
