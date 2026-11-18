namespace EricClima.Escritorio.Models
{
    public enum Rol { Admin, Tecnico }

    public enum EstadoTrabajo { Pendiente, Agendado, Realizado, Cancelado }

    public abstract class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";
        public string Email { get; set; } = "";

        public virtual string Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre)) return "El nombre es obligatorio.";
            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains('@')) return "El email no es válido.";
            return "";
        }
    }

    public class Cliente : Persona
    {
        public string Direccion { get; set; } = "";
        public string Localidad { get; set; } = "Bahía Blanca";
        public bool Activo { get; set; } = true;

        public override string Validar()
        {
            string error = base.Validar();
            if (error != "") return error;
            if (string.IsNullOrWhiteSpace(Telefono)) return "El teléfono es obligatorio.";
            if (string.IsNullOrWhiteSpace(Direccion)) return "La dirección es obligatoria.";
            return "";
        }

        public override string ToString() => $"{Nombre} - {Direccion}";
    }

    public class Usuario : Persona
    {
        public Rol Rol { get; set; }
        public string PasswordHash { get; set; } = "";
        public bool Activo { get; set; } = true;

        public bool VerificarClave(string clave) => BCrypt.Net.BCrypt.Verify(clave, PasswordHash);

        public override string ToString() => Nombre;
    }

    public class Servicio
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public decimal PrecioBase { get; set; }

        public override string ToString() => Nombre;
    }

    public class Material
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Unidad { get; set; } = "unidad";
        public decimal StockActual { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal Precio { get; set; }

        public bool StockBajo() => StockActual <= StockMinimo;

        public string Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre)) return "El nombre es obligatorio.";
            if (StockActual < 0 || StockMinimo < 0) return "El stock no puede ser negativo.";
            if (Precio < 0) return "El precio no puede ser negativo.";
            return "";
        }

        public override string ToString() => $"{Nombre} (stock: {StockActual} {Unidad})";
    }

    public class Trabajo
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public int ServicioId { get; set; }
        public string ServicioNombre { get; set; } = "";
        public int? TecnicoId { get; set; }
        public string TecnicoNombre { get; set; } = "";
        public DateTime? FechaAgendada { get; set; }
        public EstadoTrabajo Estado { get; set; } = EstadoTrabajo.Pendiente;
        public decimal Total { get; set; }
        public DateTime? GarantiaHasta { get; set; }
        public string Observaciones { get; set; } = "";

        public bool EstaCerrado() => Estado == EstadoTrabajo.Realizado || Estado == EstadoTrabajo.Cancelado;
    }

    public enum EstadoPresupuesto { Pendiente, Aceptado, Rechazado, Vencido }

    public class PresupuestoItem
    {
        public string Descripcion { get; set; } = "";
        public decimal Cantidad { get; set; } = 1;
        public decimal PrecioUnitario { get; set; }

        public decimal Subtotal() => Cantidad * PrecioUnitario;
    }

    public class Presupuesto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public string ClienteNombre { get; set; } = "";
        public string ClienteDireccion { get; set; } = "";
        public string ClienteTelefono { get; set; } = "";
        public DateTime Fecha { get; set; } = DateTime.Today;
        public DateTime? Vence { get; set; }
        public EstadoPresupuesto Estado { get; set; } = EstadoPresupuesto.Pendiente;
        public List<PresupuestoItem> Items { get; set; } = new List<PresupuestoItem>();

        public decimal Total() => Items.Sum(i => i.Subtotal());

        public string Validar()
        {
            if (ClienteId == 0) return "Elegí un cliente.";
            if (Items.Count == 0) return "Agregá al menos un ítem.";
            foreach (var i in Items)
            {
                if (string.IsNullOrWhiteSpace(i.Descripcion)) return "Hay un ítem sin descripción.";
                if (i.Cantidad <= 0) return $"La cantidad de \"{i.Descripcion}\" tiene que ser mayor a 0.";
                if (i.PrecioUnitario <= 0) return $"El precio de \"{i.Descripcion}\" tiene que ser mayor a 0.";
            }
            return "";
        }
    }
}
