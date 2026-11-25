namespace ClimaGestion.Entidades
{
    public class Solicitud
    {
        private int id;
        private string nombre = "";
        private string telefono = "";
        private string direccion = "";
        private int idServicio;
        private string mensaje = "";
        private string estado = "pendiente";

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Direccion { get => direccion; set => direccion = value; }
        public int IdServicio { get => idServicio; set => idServicio = value; }
        public string Mensaje { get => mensaje; set => mensaje = value; }
        public string Estado { get => estado; set => estado = value; }

        public bool EstaConvertida()
        {
            return Estado == "convertida";
        }
    }
}
