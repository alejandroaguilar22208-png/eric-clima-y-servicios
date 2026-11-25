# Eric Clima y Servicios

Sistema de gestión para un servicio técnico de instalación y reparación de aires acondicionados de Bahía Blanca.
Proyecto final de 7.º 6.ª Programación — E.E.S.T. N.º 1 — 2026.
Alumno: Alejandro Aguilar Villan.

El sistema tiene dos partes que comparten **una misma base de datos MariaDB / MySQL**:

| Carpeta | Qué es | Tecnologías | Materia |
| --- | --- | --- | --- |
| `escritorio/` | Programa de escritorio: usuarios, clientes, trabajos, stock, presupuestos, solicitudes y equipos en venta | C# Windows Forms (.NET Framework 4.8), POO y MVC | Proyecto, diseño e implementación de sistemas computacionales (Prof. Gallardo) |
| `web/` | Sitio público con formulario de pedido + panel de gestión + API JSON | PHP 8 con PDO, Bootstrap 5 (XAMPP) | Proyecto de implementación de sitios web dinámicos (Prof. Munafo) |
| `database/` | `climagestion.sql`: crea las 14 tablas con datos de prueba | MariaDB / MySQL | Las dos |
| `docs/` | Documentación en PDF, diagramas de draw.io y capturas | — | Las dos |

## Cómo probarlo

1. Instalar **XAMPP** e iniciar **Apache** y **MySQL**.
2. Importar `database/climagestion.sql` desde DBeaver o phpMyAdmin.
3. **Escritorio:** abrir `escritorio/ClimaGestion.sln` con Visual Studio y ejecutar con F5.
4. **Web:** copiar `web/` a `C:\xampp\htdocs\ericclima` y abrir `http://localhost/ericclima/` (panel en `/admin/`).
5. Usuario de prueba: está en `escritorio/GUIA.md`.

Para instalar el programa en otra PC: `escritorio/instalador/crear-instalador.bat` genera `Instalar-ClimaGestion.exe`.

## Funcionalidades

- Login con contraseña cifrada (bcrypt) y dos tipos de usuario (tabla `tipo_usuarios`): Administrador y Técnico.
- Alta de usuarios por el administrador.
- Clientes con búsqueda y baja lógica.
- Solicitudes que llegan desde la web y se convierten en cliente + trabajo.
- Trabajos: pendiente → agendado → realizado / cancelado, con materiales usados.
- Al marcar realizado: descuento de stock y garantía de 12 meses, en una transacción.
- Stock con alerta de mínimo.
- Presupuestos con ítems, vencimiento a 15 días e impresión / PDF.
- Equipos en venta que se muestran en la web con calculadora de frigorías.
- Reportes de ingresos (web) y API JSON de trabajos.

## Documentación

| Archivo | Qué es |
| --- | --- |
| `docs/Documentacion-Escritorio.pdf` | Documento único para Gallardo: factibilidad, requerimientos, MER, relacional, clases, manual, DFD, pruebas e instalación. |
| `docs/Documentacion-Web.pdf` | Documento para Munafo: los 17 puntos con requisitos F# / NF#, reglas, pruebas y diagramas. |
| `escritorio/ClimaGestion/Manual.pdf` | Manual de usuario; se abre desde el programa con F1. |
| `docs/diagramas/*.drawio` | MER, modelo relacional, clases, arquitectura MVC, DFD y mapa del sitio. Imágenes en `docs/diagramas/imagenes/`. |
| `escritorio/GUIA.md` y `web/GUIA-WEB.md` | Guías para probar y explicar cada parte. |
