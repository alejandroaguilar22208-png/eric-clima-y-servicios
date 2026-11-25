using System;
using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloSolicitudes
    {
        public DataTable listar()
        {
            return Consultas.tabla(
                "SELECT so.id, DATE_FORMAT(so.fecha, '%d/%m/%Y %H:%i') AS Fecha, so.nombre AS Nombre, so.telefono AS Teléfono, " +
                "so.direccion AS Dirección, IFNULL(s.nombre, 'Compra de equipo / otro') AS Servicio, so.estado AS Estado, so.mensaje " +
                "FROM solicitudes so LEFT JOIN servicios s ON s.id = so.servicio_id " +
                "ORDER BY so.estado = 'pendiente' DESC, so.fecha DESC");
        }

        public DataTable pendientes()
        {
            return Consultas.tabla(
                "SELECT so.nombre, so.telefono, IFNULL(s.nombre, 'Compra de equipo / otro') AS servicio " +
                "FROM solicitudes so LEFT JOIN servicios s ON s.id = so.servicio_id WHERE so.estado = 'pendiente' ORDER BY so.fecha DESC");
        }

        public Solicitud obtener(int id)
        {
            DataTable dt = Consultas.tabla("SELECT * FROM solicitudes WHERE id = @id", new MySqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow fila = dt.Rows[0];
            Solicitud s = new Solicitud();
            s.Id = id;
            s.Nombre = fila["nombre"].ToString();
            s.Telefono = fila["telefono"].ToString();
            s.Direccion = fila["direccion"].ToString();
            s.IdServicio = fila["servicio_id"] == DBNull.Value ? 0 : Convert.ToInt32(fila["servicio_id"]);
            s.Mensaje = fila["mensaje"].ToString();
            s.Estado = fila["estado"].ToString();
            return s;
        }

        public void marcarContactado(int id)
        {
            Consultas.ejecutar("UPDATE solicitudes SET estado = 'contactado' WHERE id = @id AND estado = 'pendiente'",
                new MySqlParameter("@id", id));
        }

        public int eliminar(int id)
        {
            return Consultas.ejecutar("DELETE FROM solicitudes WHERE id = @id AND estado <> 'convertida'",
                new MySqlParameter("@id", id));
        }

        public void convertir(Solicitud s)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlTransaction transaccion = conectar.BeginTransaction();
                try
                {
                    MySqlCommand cliente = new MySqlCommand(
                        "INSERT INTO clientes (nombre, telefono, direccion) VALUES (@nombre, @telefono, @direccion)", conectar, transaccion);
                    cliente.Parameters.AddWithValue("@nombre", s.Nombre);
                    cliente.Parameters.AddWithValue("@telefono", s.Telefono);
                    cliente.Parameters.AddWithValue("@direccion", s.Direccion == "" ? "A confirmar" : s.Direccion);
                    cliente.ExecuteNonQuery();
                    long idCliente = cliente.LastInsertedId;

                    MySqlCommand trabajo = new MySqlCommand(
                        "INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) " +
                        "SELECT @cliente, id, precio_base, @obs FROM servicios WHERE id = IFNULL(@servicio, (SELECT MIN(id) FROM servicios))",
                        conectar, transaccion);
                    trabajo.Parameters.AddWithValue("@cliente", idCliente);
                    trabajo.Parameters.AddWithValue("@obs", s.Mensaje == "" ? null : s.Mensaje);
                    trabajo.Parameters.AddWithValue("@servicio", s.IdServicio == 0 ? (object)null : s.IdServicio);
                    trabajo.ExecuteNonQuery();

                    MySqlCommand marcar = new MySqlCommand(
                        "UPDATE solicitudes SET estado = 'convertida', cliente_id = @cliente WHERE id = @id", conectar, transaccion);
                    marcar.Parameters.AddWithValue("@cliente", idCliente);
                    marcar.Parameters.AddWithValue("@id", s.Id);
                    marcar.ExecuteNonQuery();

                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }
    }
}
