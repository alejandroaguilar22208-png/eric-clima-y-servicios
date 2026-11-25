namespace ClimaGestion.Entidades
{
    public abstract class Persona
    {
        private int id;
        private string nombre = "";
        private string telefono = "";
        private string email = "";

        public int Id { get => id; set => id = value; }
        public string Nombre { get => nombre; set => nombre = value; }
        public string Telefono { get => telefono; set => telefono = value; }
        public string Email { get => email; set => email = value; }

        public virtual string Validar()
        {
            if (string.IsNullOrWhiteSpace(Nombre)) return "El nombre es obligatorio.";
            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains("@")) return "El email no es válido.";
            return "";
        }
    }
}
