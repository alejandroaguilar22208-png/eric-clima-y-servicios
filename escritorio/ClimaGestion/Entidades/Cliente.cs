namespace ClimaGestion.Entidades
{
    public class Cliente : Persona
    {
        private string direccion = "";
        private string localidad = "Bahía Blanca";
        private bool activo = true;

        public string Direccion { get => direccion; set => direccion = value; }
        public string Localidad { get => localidad; set => localidad = value; }
        public bool Activo { get => activo; set => activo = value; }

        public override string Validar()
        {
            string error = base.Validar();
            if (error != "") return error;
            if (string.IsNullOrWhiteSpace(Telefono)) return "El teléfono es obligatorio.";
            if (string.IsNullOrWhiteSpace(Direccion)) return "La dirección es obligatoria.";
            return "";
        }
    }
}
