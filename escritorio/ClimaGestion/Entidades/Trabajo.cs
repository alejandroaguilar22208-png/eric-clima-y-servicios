using System;

namespace ClimaGestion.Entidades
{
    public class Trabajo
    {
        private int id;
        private int idCliente;
        private int idServicio;
        private int idTecnico;
        private DateTime? fechaAgendada;
        private string estado = "pendiente";
        private decimal total;
        private string observaciones = "";

        public int Id { get => id; set => id = value; }
        public int IdCliente { get => idCliente; set => idCliente = value; }
        public int IdServicio { get => idServicio; set => idServicio = value; }
        public int IdTecnico { get => idTecnico; set => idTecnico = value; }
        public DateTime? FechaAgendada { get => fechaAgendada; set => fechaAgendada = value; }
        public string Estado { get => estado; set => estado = value; }
        public decimal Total { get => total; set => total = value; }
        public string Observaciones { get => observaciones; set => observaciones = value; }

        public bool EstaCerrado()
        {
            return Estado == "realizado" || Estado == "cancelado";
        }

        public bool EstaAgendado()
        {
            return FechaAgendada != null && IdTecnico != 0;
        }
    }
}
