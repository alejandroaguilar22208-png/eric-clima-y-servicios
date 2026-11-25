using System;
using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloTrabajos
    {
        public DataTable listar(string estado)
        {
            string consulta =
                "SELECT t.id AS N, c.nombre AS Cliente, s.nombre AS Servicio, " +
                "DATE_FORMAT(t.fecha_agendada, '%d/%m/%Y %H:%i') AS Fecha, u.nombre AS Técnico, t.estado AS Estado, " +
                "t.total AS Total, DATE_FORMAT(t.garantia_hasta, '%d/%m/%Y') AS Garantía " +
                "FROM trabajos t JOIN clientes c ON c.id = t.cliente_id JOIN servicios s ON s.id = t.servicio_id " +
                "LEFT JOIN usuarios u ON u.id = t.tecnico_id ";
            if (estado != "") consulta += "WHERE t.estado = @estado ";
            consulta += "ORDER BY t.fecha_agendada IS NULL, t.fecha_agendada, t.id DESC";
            return Consultas.tabla(consulta, new MySqlParameter("@estado", estado));
        }

        public DataTable servicios()
        {
            return Consultas.tabla("SELECT id, nombre, precio_base FROM servicios WHERE activo = 1 ORDER BY nombre");
        }

        public Trabajo obtener(int id)
        {
            DataTable dt = Consultas.tabla("SELECT * FROM trabajos WHERE id = @id", new MySqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow fila = dt.Rows[0];
            Trabajo t = new Trabajo();
            t.Id = id;
            t.IdCliente = Convert.ToInt32(fila["cliente_id"]);
            t.IdServicio = Convert.ToInt32(fila["servicio_id"]);
            t.IdTecnico = fila["tecnico_id"] == DBNull.Value ? 0 : Convert.ToInt32(fila["tecnico_id"]);
            t.FechaAgendada = fila["fecha_agendada"] == DBNull.Value ? (DateTime?)null : (DateTime)fila["fecha_agendada"];
            t.Estado = fila["estado"].ToString();
            t.Total = (decimal)fila["total"];
            t.Observaciones = fila["observaciones"].ToString();
            return t;
        }

        public void insertar(Trabajo t)
        {
            Consultas.ejecutar(
                "INSERT INTO trabajos (cliente_id, servicio_id, total, observaciones) VALUES (@cliente, @servicio, @total, @obs)",
                new MySqlParameter("@cliente", t.IdCliente),
                new MySqlParameter("@servicio", t.IdServicio),
                new MySqlParameter("@total", t.Total),
                new MySqlParameter("@obs", string.IsNullOrWhiteSpace(t.Observaciones) ? null : t.Observaciones.Trim()));
        }

        public void agendar(int id, DateTime fecha, int idTecnico)
        {
            Consultas.ejecutar(
                "UPDATE trabajos SET fecha_agendada = @fecha, tecnico_id = @tecnico, estado = 'agendado' WHERE id = @id",
                new MySqlParameter("@fecha", fecha),
                new MySqlParameter("@tecnico", idTecnico),
                new MySqlParameter("@id", id));
        }

        public decimal stockDisponible(int idMaterial)
        {
            object stock = Consultas.valor("SELECT stock_actual FROM materiales WHERE id = @id", new MySqlParameter("@id", idMaterial));
            return stock == null ? 0 : Convert.ToDecimal(stock);
        }

        public decimal cantidadCargada(int idTrabajo, int idMaterial)
        {
            object cantidad = Consultas.valor(
                "SELECT cantidad FROM trabajo_material WHERE trabajo_id = @t AND material_id = @m",
                new MySqlParameter("@t", idTrabajo),
                new MySqlParameter("@m", idMaterial));
            return cantidad == null ? 0 : Convert.ToDecimal(cantidad);
        }

        public void agregarMaterial(int idTrabajo, int idMaterial, decimal cantidad)
        {
            Consultas.ejecutar(
                "INSERT INTO trabajo_material (trabajo_id, material_id, cantidad) VALUES (@t, @m, @c) " +
                "ON DUPLICATE KEY UPDATE cantidad = cantidad + VALUES(cantidad)",
                new MySqlParameter("@t", idTrabajo),
                new MySqlParameter("@m", idMaterial),
                new MySqlParameter("@c", cantidad));
        }

        public DataTable materialesDe(int idTrabajo)
        {
            return Consultas.tabla(
                "SELECT m.nombre, tm.cantidad, m.unidad, m.stock_actual FROM trabajo_material tm " +
                "JOIN materiales m ON m.id = tm.material_id WHERE tm.trabajo_id = @t ORDER BY m.nombre",
                new MySqlParameter("@t", idTrabajo));
        }

        public void marcarRealizado(int idTrabajo)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlTransaction transaccion = conectar.BeginTransaction();
                try
                {
                    MySqlCommand descontar = new MySqlCommand(
                        "UPDATE materiales m JOIN trabajo_material tm ON tm.material_id = m.id " +
                        "SET m.stock_actual = m.stock_actual - tm.cantidad WHERE tm.trabajo_id = @t", conectar, transaccion);
                    descontar.Parameters.AddWithValue("@t", idTrabajo);
                    descontar.ExecuteNonQuery();

                    MySqlCommand cerrar = new MySqlCommand(
                        "UPDATE trabajos SET estado = 'realizado', fecha_realizado = CURDATE(), " +
                        "garantia_hasta = CURDATE() + INTERVAL 12 MONTH WHERE id = @t", conectar, transaccion);
                    cerrar.Parameters.AddWithValue("@t", idTrabajo);
                    cerrar.ExecuteNonQuery();

                    transaccion.Commit();
                }
                catch
                {
                    transaccion.Rollback();
                    throw;
                }
            }
        }

        public void cancelar(int idTrabajo)
        {
            Consultas.ejecutar("UPDATE trabajos SET estado = 'cancelado' WHERE id = @id", new MySqlParameter("@id", idTrabajo));
        }

        public DataTable agendadosHoy()
        {
            return Consultas.tabla(
                "SELECT t.fecha_agendada, s.nombre AS servicio, c.nombre AS cliente FROM trabajos t " +
                "JOIN clientes c ON c.id = t.cliente_id JOIN servicios s ON s.id = t.servicio_id " +
                "WHERE t.estado = 'agendado' AND DATE(t.fecha_agendada) = CURDATE() ORDER BY t.fecha_agendada");
        }

        public DataTable pendientes()
        {
            return Consultas.tabla(
                "SELECT s.nombre AS servicio, c.nombre AS cliente FROM trabajos t " +
                "JOIN clientes c ON c.id = t.cliente_id JOIN servicios s ON s.id = t.servicio_id " +
                "WHERE t.estado = 'pendiente' ORDER BY t.id");
        }
    }
}
