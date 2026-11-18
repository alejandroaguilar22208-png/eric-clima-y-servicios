using EricClima.Escritorio.Data;
using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Controllers
{
    public class Resultado
    {
        public bool Ok { get; }
        public string Mensaje { get; }
        private Resultado(bool ok, string mensaje) { Ok = ok; Mensaje = mensaje; }
        public static Resultado Bien(string m = "Operación realizada.") => new Resultado(true, m);
        public static Resultado Error(string m) => new Resultado(false, m);

        public static Resultado DesdeExcepcion(Exception ex)
        {
            if (ex is MySqlException my)
            {
                if (my.Number == 1644) return Error(my.Message);
                if (my.Number == 1062) return Error("Ya existe un registro con esos datos (por ejemplo, el técnico ya tiene un trabajo en ese horario).");
                if (my.Number == 1451) return Error("No se puede borrar: tiene datos relacionados.");
                return Error("Error de base de datos: " + my.Message);
            }
            return Error("Error inesperado: " + ex.Message);
        }
    }

    public static class Sesion
    {
        public static Usuario UsuarioActual { get; set; }
        public static bool EsAdmin => UsuarioActual?.Rol == Rol.Admin;
    }

    public class UsuarioController
    {
        private readonly UsuarioRepositorio repo = new UsuarioRepositorio();

        public Resultado Login(string email, string clave)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(clave))
                return Resultado.Error("Completá email y contraseña.");
            try
            {
                Usuario u = repo.ObtenerPorEmail(email);
                if (u == null || !u.Activo || !u.VerificarClave(clave))
                    return Resultado.Error("Usuario o contraseña incorrectos.");
                Sesion.UsuarioActual = u;
                return Resultado.Bien($"Bienvenido, {u.Nombre}.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public List<Usuario> Tecnicos() => repo.ListarActivos();

        public Resultado CambiarCuenta(string nombre, string email, string claveActual, string claveNueva, string claveRepetir)
        {
            Usuario u = Sesion.UsuarioActual;
            if (u == null) return Resultado.Error("Tenés que ingresar al sistema.");
            nombre = (nombre ?? "").Trim();
            email = (email ?? "").Trim();
            claveNueva ??= "";
            if (nombre.Length < 3) return Resultado.Error("Escribí tu nombre.");
            if (!email.Contains('@') || !email.Contains('.') || email.Contains(' ')) return Resultado.Error("El email no es válido.");
            if (string.IsNullOrEmpty(claveActual) || !u.VerificarClave(claveActual)) return Resultado.Error("La contraseña actual no es correcta.");
            if (claveNueva != "" && claveNueva.Length < 8) return Resultado.Error("La contraseña nueva tiene que tener al menos 8 caracteres.");
            if (claveNueva != (claveRepetir ?? "")) return Resultado.Error("La contraseña nueva y la repetición no coinciden.");
            try
            {
                string hash = claveNueva == "" ? null : BCrypt.Net.BCrypt.HashPassword(claveNueva);
                repo.ActualizarCuenta(u.Id, nombre, email, hash);
                u.Nombre = nombre;
                u.Email = email;
                if (hash != null) u.PasswordHash = hash;
                return Resultado.Bien(hash != null
                    ? "Datos y contraseña actualizados. La próxima vez entrá con los datos nuevos (sirven también para la web)."
                    : "Datos actualizados.");
            }
            catch (MySqlException ex) when (ex.Number == 1062) { return Resultado.Error("Ese email ya lo usa otro usuario."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }
    }

    public class ClienteController
    {
        private readonly ClienteRepositorio repo = new ClienteRepositorio();

        public List<Cliente> Buscar(string texto) => repo.Buscar(texto ?? "");

        public Resultado Guardar(Cliente c)
        {
            string error = c.Validar();
            if (error != "") return Resultado.Error(error);
            try
            {
                if (c.Id == 0) { c.Id = repo.Insertar(c); return Resultado.Bien("Cliente agregado."); }
                repo.Actualizar(c);
                return Resultado.Bien("Cliente modificado.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado DarDeBaja(int id)
        {
            if (!Sesion.EsAdmin) return Resultado.Error("Solo el administrador puede dar de baja clientes.");
            try { repo.Desactivar(id); return Resultado.Bien("Cliente dado de baja."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }
    }

    public class TrabajoController
    {
        private readonly TrabajoRepositorio repo = new TrabajoRepositorio();

        public List<Trabajo> Listar(EstadoTrabajo? estado) => repo.Listar(estado);
        public List<Servicio> Servicios() => repo.ListarServicios();
        public List<string> MaterialesDe(int trabajoId) => repo.MaterialesDe(trabajoId);

        public Resultado Crear(Trabajo t)
        {
            if (t.ClienteId <= 0) return Resultado.Error("Elegí un cliente.");
            if (t.ServicioId <= 0) return Resultado.Error("Elegí un servicio.");
            if (t.Total < 0) return Resultado.Error("El total no puede ser negativo.");
            try { repo.Insertar(t); return Resultado.Bien("Trabajo creado."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado Agendar(Trabajo t, DateTime fecha, Usuario tecnico)
        {
            if (t.EstaCerrado()) return Resultado.Error("Ese trabajo ya está cerrado.");
            if (tecnico == null) return Resultado.Error("Elegí un técnico.");
            if (fecha < DateTime.Today) return Resultado.Error("La fecha no puede ser anterior a hoy.");
            try { repo.Agendar(t.Id, fecha, tecnico.Id); return Resultado.Bien("Trabajo agendado."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado AgregarMaterial(Trabajo t, Material m, decimal cantidad)
        {
            if (t.EstaCerrado()) return Resultado.Error("Ese trabajo ya está cerrado.");
            if (m == null) return Resultado.Error("Elegí un material.");
            if (cantidad <= 0) return Resultado.Error("La cantidad debe ser mayor a 0.");
            if (cantidad > m.StockActual) return Resultado.Error($"Stock insuficiente: hay {m.StockActual} {m.Unidad}.");
            try { repo.AgregarMaterial(t.Id, m.Id, cantidad); return Resultado.Bien("Material agregado al trabajo."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado MarcarRealizado(Trabajo t)
        {
            if (t.EstaCerrado()) return Resultado.Error("Ese trabajo ya está cerrado.");
            if (t.FechaAgendada == null || t.TecnicoId == null)
                return Resultado.Error("Primero agendá el trabajo con fecha y técnico.");
            try { repo.CambiarEstado(t.Id, EstadoTrabajo.Realizado); return Resultado.Bien("Trabajo realizado. Se descontó el stock y la garantía corre 12 meses."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado Cancelar(Trabajo t)
        {
            if (t.EstaCerrado()) return Resultado.Error("Ese trabajo ya está cerrado.");
            try { repo.CambiarEstado(t.Id, EstadoTrabajo.Cancelado); return Resultado.Bien("Trabajo cancelado."); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }
    }

    public class StockController
    {
        private readonly MaterialRepositorio repo = new MaterialRepositorio();

        public List<Material> Listar() => repo.Listar();
        public List<Material> StockBajo() => repo.Listar().Where(m => m.StockBajo()).ToList();

        public Resultado Guardar(Material m)
        {
            if (!Sesion.EsAdmin) return Resultado.Error("Solo el administrador puede modificar el stock.");
            string error = m.Validar();
            if (error != "") return Resultado.Error(error);
            try
            {
                if (m.Id == 0) { repo.Insertar(m); return Resultado.Bien("Material agregado."); }
                repo.Actualizar(m);
                return Resultado.Bien("Material modificado.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }
    }

    public class PresupuestoController
    {
        private readonly PresupuestoRepositorio repo = new PresupuestoRepositorio();

        public List<Presupuesto> Listar()
        {
            repo.MarcarVencidos();
            return repo.Listar();
        }

        public Resultado Crear(Presupuesto p)
        {
            string error = p.Validar();
            if (error != "") return Resultado.Error(error);
            try
            {
                p.Id = repo.Insertar(p);
                Presupuesto guardado = repo.ObtenerPorId(p.Id);
                p.Vence = guardado.Vence;
                return Resultado.Bien($"Presupuesto N.º {p.Id} guardado. Vence el {p.Vence:dd/MM/yyyy}.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado Rechazar(Presupuesto p)
        {
            if (p.Estado != EstadoPresupuesto.Pendiente)
                return Resultado.Error("Solo se puede rechazar un presupuesto pendiente.");
            try
            {
                if (!repo.EliminarPendiente(p.Id)) return Resultado.Error("Ese presupuesto ya no está pendiente.");
                return Resultado.Bien($"Presupuesto de {p.ClienteNombre} rechazado y borrado.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado CambiarEstado(Presupuesto p, EstadoPresupuesto nuevo)
        {
            if (p.Estado != EstadoPresupuesto.Pendiente)
                return Resultado.Error($"El presupuesto ya está {p.Estado.ToString().ToLower()}; no se puede cambiar.");
            try
            {
                repo.CambiarEstado(p.Id, nuevo);
                p.Estado = nuevo;
                return Resultado.Bien($"Presupuesto marcado como {nuevo.ToString().ToLower()}.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado ExportarPdf(Presupuesto p, string ruta)
        {
            try
            {
                Views.PresupuestoPdf.Generar(p, ruta);
                return Resultado.Bien("PDF generado.");
            }
            catch (Exception ex) { return Resultado.Error("No se pudo crear el PDF: " + ex.Message); }
        }
    }

    public class EquipoVentaController
    {
        private readonly EquipoVentaRepositorio repo = new EquipoVentaRepositorio();

        public List<EquipoVenta> Listar() => repo.Listar();

        public Resultado Guardar(EquipoVenta e)
        {
            if (!Sesion.EsAdmin) return Resultado.Error("Solo el administrador puede modificar los equipos en venta.");
            string error = e.Validar();
            if (error != "") return Resultado.Error(error);
            try
            {
                if (e.Id == 0) { repo.Insertar(e); return Resultado.Bien("Equipo agregado. Ya se ve en la página web."); }
                repo.Actualizar(e);
                return Resultado.Bien("Equipo modificado. La página web ya muestra los cambios.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado CambiarVisibilidad(EquipoVenta e)
        {
            if (!Sesion.EsAdmin) return Resultado.Error("Solo el administrador puede modificar los equipos en venta.");
            try
            {
                e.Activo = !e.Activo;
                repo.Actualizar(e);
                return Resultado.Bien(e.Activo ? "El equipo se volvió a mostrar en la web." : "El equipo quedó oculto en la web.");
            }
            catch (Exception ex) { e.Activo = !e.Activo; return Resultado.DesdeExcepcion(ex); }
        }
    }

    public class SolicitudController
    {
        private readonly SolicitudRepositorio repo = new SolicitudRepositorio();

        public List<Solicitud> Listar() => repo.Listar();
        public int Pendientes() => repo.ContarPendientes();

        public Resultado MarcarContactado(Solicitud s) => Cambiar(s, EstadoSolicitud.Contactado, "Solicitud marcada como contactada.");
        public Resultado Descartar(Solicitud s)
        {
            if (s.EstaCerrada()) return Resultado.Error("Esa solicitud ya está cerrada.");
            try
            {
                if (!repo.Eliminar(s.Id)) return Resultado.Error("Esa solicitud ya no se puede descartar.");
                return Resultado.Bien($"Solicitud de {s.Nombre} descartada y borrada.");
            }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        private Resultado Cambiar(Solicitud s, EstadoSolicitud estado, string mensaje)
        {
            if (s.EstaCerrada()) return Resultado.Error("Esa solicitud ya está cerrada.");
            try { repo.CambiarEstado(s.Id, estado); return Resultado.Bien(mensaje); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }

        public Resultado Convertir(Solicitud s)
        {
            if (s.Estado == EstadoSolicitud.Convertida) return Resultado.Error("Esa solicitud ya fue convertida.");
            try
            {
                repo.Convertir(s);
                return Resultado.Bien("Se creó el cliente y un trabajo pendiente. Ahora agendalo en Trabajos.");
            }
            catch (InvalidOperationException ex) { return Resultado.Error(ex.Message); }
            catch (Exception ex) { return Resultado.DesdeExcepcion(ex); }
        }
    }
}
