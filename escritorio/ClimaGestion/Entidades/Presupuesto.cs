using System;
using System.Collections.Generic;

namespace ClimaGestion.Entidades
{
    public class Presupuesto
    {
        private int id;
        private int idCliente;
        private string nombreCliente = "";
        private string direccionCliente = "";
        private string telefonoCliente = "";
        private DateTime fecha = DateTime.Today;
        private DateTime vence = DateTime.Today.AddDays(15);
        private string estado = "pendiente";
        private List<ItemPresupuesto> items = new List<ItemPresupuesto>();

        public int Id { get => id; set => id = value; }
        public int IdCliente { get => idCliente; set => idCliente = value; }
        public string NombreCliente { get => nombreCliente; set => nombreCliente = value; }
        public string DireccionCliente { get => direccionCliente; set => direccionCliente = value; }
        public string TelefonoCliente { get => telefonoCliente; set => telefonoCliente = value; }
        public DateTime Fecha { get => fecha; set => fecha = value; }
        public DateTime Vence { get => vence; set => vence = value; }
        public string Estado { get => estado; set => estado = value; }
        public List<ItemPresupuesto> Items { get => items; set => items = value; }

        public decimal Total()
        {
            decimal total = 0;
            foreach (ItemPresupuesto item in Items)
            {
                total += item.Subtotal();
            }
            return total;
        }

        public string Validar()
        {
            if (IdCliente == 0) return "Elegí un cliente.";
            if (Items.Count == 0) return "Agregá al menos un ítem.";
            foreach (ItemPresupuesto item in Items)
            {
                if (string.IsNullOrWhiteSpace(item.Descripcion) || item.Cantidad <= 0 || item.PrecioUnitario <= 0)
                    return "Cada ítem necesita descripción, cantidad y precio mayores a 0.";
            }
            if (Vence < Fecha) return "La fecha de vencimiento no puede ser anterior a hoy.";
            return "";
        }
    }
}
