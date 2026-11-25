using System;
using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloPresupuestos
    {
        public void marcarVencidos()
        {
            Consultas.ejecutar("UPDATE presupuestos SET estado = 'vencido' WHERE estado = 'pendiente' AND vence < CURDATE()");
        }

        public DataTable listar()
        {
            return Consultas.tabla(
                "SELECT p.id, LPAD(p.id, 5, '0') AS Número, c.nombre AS Cliente, DATE_FORMAT(p.fecha, '%d/%m/%Y') AS Fecha, " +
                "DATE_FORMAT(p.vence, '%d/%m/%Y') AS Vence, p.total AS Total, p.estado AS Estado " +
                "FROM presupuestos p JOIN clientes c ON c.id = p.cliente_id ORDER BY p.fecha DESC, p.id DESC");
        }

        public DataTable serviciosYMateriales()
        {
            return Consultas.tabla(
                "SELECT nombre, precio_base AS precio FROM servicios WHERE activo = 1 " +
                "UNION ALL SELECT CONCAT(nombre, ' (', unidad, ')'), precio FROM materiales ORDER BY 1");
        }

        public int insertar(Presupuesto p)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlTransaction transaccion = conectar.BeginTransaction();
                try
                {
                    MySqlCommand cabecera = new MySqlCommand(
                        "INSERT INTO presupuestos (cliente_id, fecha, vence, total) VALUES (@cliente, @fecha, @vence, @total)",
                        conectar, transaccion);
                    cabecera.Parameters.AddWithValue("@cliente", p.IdCliente);
                    cabecera.Parameters.AddWithValue("@fecha", p.Fecha);
                    cabecera.Parameters.AddWithValue("@vence", p.Vence);
                    cabecera.Parameters.AddWithValue("@total", p.Total());
                    cabecera.ExecuteNonQuery();
                    int id = (int)cabecera.LastInsertedId;

                    foreach (ItemPresupuesto item in p.Items)
                    {
                        MySqlCommand renglon = new MySqlCommand(
                            "INSERT INTO presupuesto_items (presupuesto_id, descripcion, cantidad, precio_unitario) VALUES (@p, @d, @c, @pu)",
                            conectar, transaccion);
                        renglon.Parameters.AddWithValue("@p", id);
                        renglon.Parameters.AddWithValue("@d", item.Descripcion.Trim());
                        renglon.Parameters.AddWithValue("@c", item.Cantidad);
                        renglon.Parameters.AddWithValue("@pu", item.PrecioUnitario);
                        renglon.ExecuteNonQuery();
                    }

                    transaccion.Commit();
                    return id;
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        public Presupuesto obtener(int id)
        {
            DataTable dt = Consultas.tabla(
                "SELECT p.*, c.nombre, c.direccion, c.telefono FROM presupuestos p JOIN clientes c ON c.id = p.cliente_id WHERE p.id = @id",
                new MySqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow fila = dt.Rows[0];
            Presupuesto p = new Presupuesto();
            p.Id = id;
            p.IdCliente = Convert.ToInt32(fila["cliente_id"]);
            p.NombreCliente = fila["nombre"].ToString();
            p.DireccionCliente = fila["direccion"].ToString();
            p.TelefonoCliente = fila["telefono"].ToString();
            p.Fecha = (DateTime)fila["fecha"];
            p.Vence = (DateTime)fila["vence"];
            p.Estado = fila["estado"].ToString();

            DataTable items = Consultas.tabla(
                "SELECT descripcion, cantidad, precio_unitario FROM presupuesto_items WHERE presupuesto_id = @id ORDER BY id",
                new MySqlParameter("@id", id));
            foreach (DataRow renglon in items.Rows)
            {
                ItemPresupuesto item = new ItemPresupuesto();
                item.Descripcion = renglon["descripcion"].ToString();
                item.Cantidad = (decimal)renglon["cantidad"];
                item.PrecioUnitario = (decimal)renglon["precio_unitario"];
                p.Items.Add(item);
            }
            return p;
        }

        public int aceptar(int id)
        {
            return Consultas.ejecutar("UPDATE presupuestos SET estado = 'aceptado' WHERE id = @id AND estado = 'pendiente'",
                new MySqlParameter("@id", id));
        }

        public int eliminarPendiente(int id)
        {
            return Consultas.ejecutar("DELETE FROM presupuestos WHERE id = @id AND estado = 'pendiente'",
                new MySqlParameter("@id", id));
        }
    }
}
