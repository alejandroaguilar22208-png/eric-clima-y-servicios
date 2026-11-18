using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public class UsuarioRepositorio
    {
        public Usuario ObtenerPorEmail(string email)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "SELECT id, nombre, email, password_hash, rol, activo FROM usuarios WHERE email = @e", cn);
            cmd.Parameters.AddWithValue("@e", email.Trim());
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Mapear(dr) : null;
        }

        public List<Usuario> ListarActivos()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "SELECT id, nombre, email, password_hash, rol, activo FROM usuarios WHERE activo = 1 ORDER BY nombre", cn);
            var lista = new List<Usuario>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Mapear(dr));
            return lista;
        }

        private static Usuario Mapear(MySqlDataReader dr) => new Usuario
        {
            Id = dr.GetInt32("id"),
            Nombre = dr.GetString("nombre"),
            Email = dr.GetString("email"),
            PasswordHash = dr.GetString("password_hash"),
            Rol = dr.GetString("rol") == "admin" ? Rol.Admin : Rol.Tecnico,
            Activo = dr.GetBoolean("activo")
        };
    }
}
