using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloUsuarios
    {
        public Usuario buscarUsuario(string email)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                string consulta = "SELECT u.id, u.nombre, u.email, u.password_hash, u.tipo_usuario_id, u.activo, t.nombre AS tipo " +
                                  "FROM usuarios u JOIN tipo_usuarios t ON t.id = u.tipo_usuario_id WHERE u.email = @email";
                MySqlCommand comando = new MySqlCommand(consulta, conectar);
                comando.Parameters.AddWithValue("@email", email.Trim());
                using (MySqlDataReader lector = comando.ExecuteReader())
                {
                    if (!lector.Read()) return null;
                    Usuario u = new Usuario();
                    u.Id = lector.GetInt32("id");
                    u.Nombre = lector.GetString("nombre");
                    u.Email = lector.GetString("email");
                    u.PasswordHash = lector.GetString("password_hash");
                    u.IdTipo = lector.GetInt32("tipo_usuario_id");
                    u.NombreTipo = lector.GetString("tipo");
                    u.Activo = lector.GetBoolean("activo");
                    return u;
                }
            }
        }

        public DataTable obtenerTipos()
        {
            return Consultas.tabla("SELECT id, nombre FROM tipo_usuarios ORDER BY id");
        }

        public DataTable listar()
        {
            return Consultas.tabla(
                "SELECT u.id, u.nombre AS Nombre, u.email AS Email, t.nombre AS Tipo, " +
                "IF(u.activo = 1, 'Activo', 'Inactivo') AS Estado, u.tipo_usuario_id, u.activo " +
                "FROM usuarios u JOIN tipo_usuarios t ON t.id = u.tipo_usuario_id ORDER BY u.activo DESC, u.nombre");
        }

        public DataTable tecnicos()
        {
            return Consultas.tabla("SELECT id, nombre FROM usuarios WHERE activo = 1 ORDER BY nombre");
        }

        public void insertar(Usuario u)
        {
            Consultas.ejecutar(
                "INSERT INTO usuarios (nombre, email, password_hash, tipo_usuario_id, activo) VALUES (@nombre, @email, @hash, @tipo, @activo)",
                new MySqlParameter("@nombre", u.Nombre.Trim()),
                new MySqlParameter("@email", u.Email.Trim()),
                new MySqlParameter("@hash", u.PasswordHash),
                new MySqlParameter("@tipo", u.IdTipo),
                new MySqlParameter("@activo", u.Activo));
        }

        public void actualizar(Usuario u)
        {
            Consultas.ejecutar(
                "UPDATE usuarios SET nombre = @nombre, email = @email, tipo_usuario_id = @tipo, activo = @activo WHERE id = @id",
                new MySqlParameter("@nombre", u.Nombre.Trim()),
                new MySqlParameter("@email", u.Email.Trim()),
                new MySqlParameter("@tipo", u.IdTipo),
                new MySqlParameter("@activo", u.Activo),
                new MySqlParameter("@id", u.Id));
            if (u.PasswordHash != "")
            {
                Consultas.ejecutar("UPDATE usuarios SET password_hash = @hash WHERE id = @id",
                    new MySqlParameter("@hash", u.PasswordHash),
                    new MySqlParameter("@id", u.Id));
            }
        }
    }
}
