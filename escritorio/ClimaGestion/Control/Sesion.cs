using ClimaGestion.Entidades;

namespace ClimaGestion.Control
{
    public static class Sesion
    {
        private static Usuario usuarioActual;

        public static Usuario UsuarioActual { get => usuarioActual; set => usuarioActual = value; }

        public static bool EsAdministrador()
        {
            return UsuarioActual != null && UsuarioActual.EsAdministrador();
        }
    }
}
