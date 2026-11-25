namespace ClimaGestion.Entidades
{
    public class Usuario : Persona
    {
        public const int TipoAdministrador = 1;
        public const int TipoTecnico = 2;

        private string password = "";
        private string passwordConfirma = "";
        private string passwordHash = "";
        private int idTipo = TipoTecnico;
        private string nombreTipo = "";
        private bool activo = true;

        public Usuario()
        {
        }

        public Usuario(string email, string password)
        {
            Email = email;
            Password = password;
        }

        public string Password { get => password; set => password = value; }
        public string PasswordConfirma { get => passwordConfirma; set => passwordConfirma = value; }
        public string PasswordHash { get => passwordHash; set => passwordHash = value; }
        public int IdTipo { get => idTipo; set => idTipo = value; }
        public string NombreTipo { get => nombreTipo; set => nombreTipo = value; }
        public bool Activo { get => activo; set => activo = value; }

        public bool EsAdministrador()
        {
            return IdTipo == TipoAdministrador;
        }

        public override string Validar()
        {
            string error = base.Validar();
            if (error != "") return error;
            if (string.IsNullOrWhiteSpace(Email)) return "El email es obligatorio.";
            if (Password != "" && Password.Length < 8) return "La contraseña tiene que tener al menos 8 caracteres.";
            if (Password != PasswordConfirma) return "La contraseña y la confirmación no coinciden.";
            return "";
        }
    }
}
