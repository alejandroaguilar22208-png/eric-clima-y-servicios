using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    /// <summary>Acceso a las tablas presupuestos y presupuesto_items.</summary>
    public class PresupuestoRepositorio
    {
        private const string Consulta =
            "SELECT p.id, p.cliente_id, p.fecha, p.vence, p.estado, " +
            "       c.nombre, c.direccion, c.telefono " +
            "FROM presupuestos p JOIN clientes c ON c.id = p.cliente_id ";

        public List<Presupuesto> Listar()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(Consulta + "ORDER BY p.fecha DESC, p.id DESC", cn);
            var lista = new List<Presupuesto>();
            using (var dr = cmd.ExecuteReader())
                while (dr.Read()) lista.Add(Mapear(dr));
            foreach (var p in lista) p.Items = Items(cn, p.Id);
            return lista;
        }

        public Presupuesto ObtenerPorId(int id)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(Consulta + "WHERE p.id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            Presupuesto p;
            using (var dr = cmd.ExecuteReader())
            {
                if (!dr.Read()) return null;
                p = Mapear(dr);
            }
            p.Items = Items(cn, id);
            return p;
        }

        /// <summary>
        /// Guarda el encabezado y todos los ítems en una TRANSACCIÓN:
        /// si falla un ítem, no queda un presupuesto a medias.
        /// </summary>
        public int Insertar(Presupuesto p)
        {
            using var cn = Conexion.Abrir();
            using var tx = cn.BeginTransaction();
            try
            {
                using var cab = new MySqlCommand(
                    "INSERT INTO presupuestos (cliente_id, fecha, vence, total) VALUES (@cli, @fecha, @vence, @total)", cn, tx);
                cab.Parameters.AddWithValue("@cli", p.ClienteId);
                cab.Parameters.AddWithValue("@fecha", p.Fecha.Date);
                cab.Parameters.AddWithValue("@vence", (object)p.Vence?.Date ?? DBNull.Value); // NULL → el trigger pone +15 días
                cab.Parameters.AddWithValue("@total", p.Total());
                cab.ExecuteNonQuery();
                int id = (int)cab.LastInsertedId;

                foreach (var i in p.Items)
                {
                    using var it = new MySqlCommand(
                        "INSERT INTO presupuesto_items (presupuesto_id, descripcion, cantidad, precio_unitario) " +
                        "VALUES (@id, @desc, @cant, @precio)", cn, tx);
                    it.Parameters.AddWithValue("@id", id);
                    it.Parameters.AddWithValue("@desc", i.Descripcion.Trim());
                    it.Parameters.AddWithValue("@cant", i.Cantidad);
                    it.Parameters.AddWithValue("@precio", i.PrecioUnitario);
                    it.ExecuteNonQuery();
                }
                tx.Commit();
                return id;
            }
            catch
            {
                tx.Rollback();
                throw;
            }
        }

        public void CambiarEstado(int id, EstadoPresupuesto estado)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand("UPDATE presupuestos SET estado = @e WHERE id = @id", cn);
            cmd.Parameters.AddWithValue("@e", estado.ToString().ToLower());
            cmd.Parameters.AddWithValue("@id", id);
            cmd.ExecuteNonQuery();
        }

        /// <summary>Marca como vencidos los pendientes cuya fecha de vencimiento ya pasó.</summary>
        public void MarcarVencidos()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "UPDATE presupuestos SET estado = 'vencido' WHERE estado = 'pendiente' AND vence < CURDATE()", cn);
            cmd.ExecuteNonQuery();
        }

        private static List<PresupuestoItem> Items(MySqlConnection cn, int presupuestoId)
        {
            using var cmd = new MySqlCommand(
                "SELECT descripcion, cantidad, precio_unitario FROM presupuesto_items WHERE presupuesto_id = @id ORDER BY id", cn);
            cmd.Parameters.AddWithValue("@id", presupuestoId);
            var lista = new List<PresupuestoItem>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read())
                lista.Add(new PresupuestoItem
                {
                    Descripcion = dr.GetString("descripcion"),
                    Cantidad = dr.GetDecimal("cantidad"),
                    PrecioUnitario = dr.GetDecimal("precio_unitario")
                });
            return lista;
        }

        private static Presupuesto Mapear(MySqlDataReader dr) => new Presupuesto
        {
            Id = dr.GetInt32("id"),
            ClienteId = dr.GetInt32("cliente_id"),
            Fecha = dr.GetDateTime("fecha"),
            Vence = dr.IsDBNull(dr.GetOrdinal("vence")) ? null : dr.GetDateTime("vence"),
            Estado = Enum.Parse<EstadoPresupuesto>(dr.GetString("estado"), ignoreCase: true),
            ClienteNombre = dr.GetString("nombre"),
            ClienteDireccion = dr.GetString("direccion"),
            ClienteTelefono = dr.GetString("telefono")
        };
    }
}
