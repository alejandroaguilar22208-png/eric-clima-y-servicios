using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    /// <summary>Todo el SQL de la tabla clientes está acá y en ningún otro lado.</summary>
    public class ClienteRepositorio : IRepositorio<Cliente>
    {
        private const string Columnas = "id, nombre, telefono, email, direccion, localidad, activo";

        public List<Cliente> Listar() => Buscar("");

        /// <summary>Busca clientes activos por nombre, teléfono o dirección.</summary>
        public List<Cliente> Buscar(string texto)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                $"SELECT {Columnas} FROM clientes " +
                "WHERE activo = 1 AND (nombre LIKE @t OR telefono LIKE @t OR direccion LIKE @t) " +
                "ORDER BY nombre", cn);
            // Parámetro (@t) en vez de concatenar texto: evita inyección SQL
            cmd.Parameters.AddWithValue("@t", "%" + texto + "%");

            var lista = new List<Cliente>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Mapear(dr));
            return lista;
        }

        public Cliente ObtenerPorId(int id)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand($"SELECT {Columnas} FROM clientes WHERE id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Mapear(dr) : null;
        }

        public int Insertar(Cliente c)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "INSERT INTO clientes (nombre, telefono, email, direccion, localidad) " +
                "VALUES (@nombre, @telefono, @email, @direccion, @localidad)", cn);
            CargarParametros(cmd, c);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void Actualizar(Cliente c)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "UPDATE clientes SET nombre=@nombre, telefono=@telefono, email=@email, " +
                "direccion=@direccion, localidad=@localidad WHERE id=@id", cn);
            CargarParametros(cmd, c);
            cmd.Parameters.AddWithValue("@id", c.Id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>RN-08: los clientes no se borran, se desactivan.</summary>
        public void Desactivar(int id)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand("UPDATE clientes SET activo = 0 WHERE id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        private static void CargarParametros(MySqlCommand cmd, Cliente c)
        {
            cmd.Parameters.AddWithValue("@nombre", c.Nombre.Trim());
            cmd.Parameters.AddWithValue("@telefono", c.Telefono.Trim());
            cmd.Parameters.AddWithValue("@email", string.IsNullOrWhiteSpace(c.Email) ? null : c.Email.Trim());
            cmd.Parameters.AddWithValue("@direccion", c.Direccion.Trim());
            cmd.Parameters.AddWithValue("@localidad", c.Localidad.Trim());
        }

        /// <summary>Convierte una fila de la base en un objeto Cliente.</summary>
        private static Cliente Mapear(MySqlDataReader dr) => new Cliente
        {
            Id = dr.GetInt32("id"),
            Nombre = dr.GetString("nombre"),
            Telefono = dr.GetString("telefono"),
            Email = dr.IsDBNull(dr.GetOrdinal("email")) ? "" : dr.GetString("email"),
            Direccion = dr.GetString("direccion"),
            Localidad = dr.GetString("localidad"),
            Activo = dr.GetBoolean("activo")
        };
    }
}
