using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public class TrabajoRepositorio
    {
        private const string Select =
            "SELECT t.id, t.cliente_id, c.nombre AS cliente, t.servicio_id, s.nombre AS servicio, " +
            "t.tecnico_id, u.nombre AS tecnico, t.fecha_agendada, t.estado, t.total, " +
            "t.garantia_hasta, t.observaciones " +
            "FROM trabajos t " +
            "JOIN clientes c ON c.id = t.cliente_id " +
            "JOIN servicios s ON s.id = t.servicio_id " +
            "LEFT JOIN usuarios u ON u.id = t.tecnico_id ";

        public List<Trabajo> Listar(EstadoTrabajo? estado)
        {
            using var cn = Conexion.Abrir();
            string sql = Select + (estado == null ? "" : "WHERE t.estado = @estado ") +
                         "ORDER BY t.fecha_agendada IS NULL, t.fecha_agendada, t.id DESC";
            using var cmd = new MySqlCommand(sql, cn);
            if (estado != null) cmd.Parameters.AddWithValue("@estado", estado.ToString().ToLower());

            var lista = new List<Trabajo>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Mapear(dr));
            return lista;
        }

        public int Insertar(Trabajo t)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) " +
                "VALUES (@cliente, @servicio, @total, @obs)", cn);
            cmd.Parameters.AddWithValue("@cliente", t.ClienteId);
            cmd.Parameters.AddWithValue("@servicio", t.ServicioId);
            cmd.Parameters.AddWithValue("@total", t.Total);
            cmd.Parameters.AddWithValue("@obs", t.Observaciones);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void Agendar(int trabajoId, DateTime fecha, int tecnicoId)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "UPDATE trabajos SET fecha_agendada=@f, tecnico_id=@tec, estado='agendado' WHERE id=@id", cn);
            cmd.Parameters.AddWithValue("@f", fecha);
            cmd.Parameters.AddWithValue("@tec", tecnicoId);
            cmd.Parameters.AddWithValue("@id", trabajoId);
            cmd.ExecuteNonQuery();
        }

        public void CambiarEstado(int trabajoId, EstadoTrabajo estado)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand("UPDATE trabajos SET estado=@e WHERE id=@id", cn);
            cmd.Parameters.AddWithValue("@e", estado.ToString().ToLower());
            cmd.Parameters.AddWithValue("@id", trabajoId);
            cmd.ExecuteNonQuery();
        }

        public void AgregarMaterial(int trabajoId, int materialId, decimal cantidad)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "INSERT INTO trabajo_material (trabajo_id, material_id, cantidad) VALUES (@t, @m, @c) " +
                "ON DUPLICATE KEY UPDATE cantidad = cantidad + @c", cn);
            cmd.Parameters.AddWithValue("@t", trabajoId);
            cmd.Parameters.AddWithValue("@m", materialId);
            cmd.Parameters.AddWithValue("@c", cantidad);
            cmd.ExecuteNonQuery();
        }

        public List<string> MaterialesDe(int trabajoId)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "SELECT m.nombre, tm.cantidad, m.unidad FROM trabajo_material tm " +
                "JOIN materiales m ON m.id = tm.material_id WHERE tm.trabajo_id = @t", cn);
            cmd.Parameters.AddWithValue("@t", trabajoId);
            var lista = new List<string>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add($"{dr.GetString(0)}: {dr.GetDecimal(1)} {dr.GetString(2)}");
            return lista;
        }

        public List<Servicio> ListarServicios()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "SELECT id, nombre, precio_base FROM servicios WHERE activo = 1 ORDER BY nombre", cn);
            var lista = new List<Servicio>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(new Servicio { Id = dr.GetInt32(0), Nombre = dr.GetString(1), PrecioBase = dr.GetDecimal(2) });
            return lista;
        }

        private static Trabajo Mapear(MySqlDataReader dr) => new Trabajo
        {
            Id = dr.GetInt32("id"),
            ClienteId = dr.GetInt32("cliente_id"),
            ClienteNombre = dr.GetString("cliente"),
            ServicioId = dr.GetInt32("servicio_id"),
            ServicioNombre = dr.GetString("servicio"),
            TecnicoId = dr.IsDBNull(dr.GetOrdinal("tecnico_id")) ? null : dr.GetInt32("tecnico_id"),
            TecnicoNombre = dr.IsDBNull(dr.GetOrdinal("tecnico")) ? "" : dr.GetString("tecnico"),
            FechaAgendada = dr.IsDBNull(dr.GetOrdinal("fecha_agendada")) ? null : dr.GetDateTime("fecha_agendada"),
            Estado = Enum.Parse<EstadoTrabajo>(dr.GetString("estado"), ignoreCase: true),
            Total = dr.GetDecimal("total"),
            GarantiaHasta = dr.IsDBNull(dr.GetOrdinal("garantia_hasta")) ? null : dr.GetDateTime("garantia_hasta"),
            Observaciones = dr.IsDBNull(dr.GetOrdinal("observaciones")) ? "" : dr.GetString("observaciones")
        };
    }
}
