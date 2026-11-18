using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public class SolicitudRepositorio
    {
        private const string Consulta =
            "SELECT so.id, so.cliente_id, so.nombre, so.telefono, so.direccion, so.servicio_id, so.mensaje, so.fecha, so.estado, " +
            "s.nombre AS servicio FROM solicitudes so LEFT JOIN servicios s ON s.id = so.servicio_id ";

        public List<Solicitud> Listar()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(Consulta + "ORDER BY so.estado = 'pendiente' DESC, so.fecha DESC", cn);
            var lista = new List<Solicitud>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Mapear(dr));
            return lista;
        }

        public int ContarPendientes()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand("SELECT COUNT(*) FROM solicitudes WHERE estado = 'pendiente'", cn);
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public void CambiarEstado(int id, EstadoSolicitud estado)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand("UPDATE solicitudes SET estado = @estado WHERE id = @id", cn);
            cmd.Parameters.AddWithValue("@estado", estado.ToString().ToLower());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        public int Convertir(Solicitud s)
        {
            using var cn = Conexion.Abrir();
            using var tx = cn.BeginTransaction();
            try
            {
                var cmd = new MySqlCommand(
                    "INSERT INTO clientes (nombre, telefono, direccion) VALUES (@nombre, @telefono, @direccion)", cn, tx);
                cmd.Parameters.AddWithValue("@nombre", s.Nombre);
                cmd.Parameters.AddWithValue("@telefono", s.Telefono);
                cmd.Parameters.AddWithValue("@direccion", string.IsNullOrWhiteSpace(s.Direccion) ? "A confirmar" : s.Direccion);
                cmd.ExecuteNonQuery();
                int clienteId = (int)cmd.LastInsertedId;

                cmd = new MySqlCommand(
                    "INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) " +
                    "SELECT @cliente, s.id, s.precio_base, @obs FROM servicios s " +
                    "WHERE s.id = COALESCE(@servicio, (SELECT MIN(id) FROM servicios))", cn, tx);
                cmd.Parameters.AddWithValue("@cliente", clienteId);
                cmd.Parameters.AddWithValue("@obs", string.IsNullOrWhiteSpace(s.Mensaje) ? DBNull.Value : s.Mensaje);
                cmd.Parameters.AddWithValue("@servicio", s.ServicioId.HasValue ? s.ServicioId.Value : DBNull.Value);
                cmd.ExecuteNonQuery();

                cmd = new MySqlCommand(
                    "UPDATE solicitudes SET estado = 'convertida', cliente_id = @cliente WHERE id = @id AND estado <> 'convertida'", cn, tx);
                cmd.Parameters.AddWithValue("@cliente", clienteId);
                cmd.Parameters.AddWithValue("@id", s.Id);
                if (cmd.ExecuteNonQuery() == 0) throw new InvalidOperationException("Esa solicitud ya fue convertida.");

                tx.Commit();
                return clienteId;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        private static Solicitud Mapear(MySqlDataReader dr) => new Solicitud
        {
            Id = dr.GetInt32("id"),
            ClienteId = dr.IsDBNull(dr.GetOrdinal("cliente_id")) ? null : dr.GetInt32("cliente_id"),
            Nombre = dr.GetString("nombre"),
            Telefono = dr.GetString("telefono"),
            Direccion = dr.IsDBNull(dr.GetOrdinal("direccion")) ? "" : dr.GetString("direccion"),
            ServicioId = dr.IsDBNull(dr.GetOrdinal("servicio_id")) ? null : dr.GetInt32("servicio_id"),
            ServicioNombre = dr.IsDBNull(dr.GetOrdinal("servicio")) ? "Compra de equipo / otro" : dr.GetString("servicio"),
            Mensaje = dr.IsDBNull(dr.GetOrdinal("mensaje")) ? "" : dr.GetString("mensaje"),
            Fecha = dr.GetDateTime("fecha"),
            Estado = Enum.Parse<EstadoSolicitud>(dr.GetString("estado"), true)
        };
    }
}
