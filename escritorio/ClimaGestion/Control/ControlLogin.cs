using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlLogin
    {
        public bool usuarioValido(Usuario u)
        {
            if (string.IsNullOrWhiteSpace(u.Email) || string.IsNullOrEmpty(u.Password)) return false;

            ModeloUsuarios m = new ModeloUsuarios();
            Usuario usuarioEncontrado = m.buscarUsuario(u.Email);
            if (usuarioEncontrado == null || !usuarioEncontrado.Activo) return false;
            if (!BCrypt.Net.BCrypt.Verify(u.Password, usuarioEncontrado.PasswordHash)) return false;

            Sesion.UsuarioActual = usuarioEncontrado;
            return true;
        }
    }
}
