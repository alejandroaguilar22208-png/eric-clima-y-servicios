using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public class MaterialRepositorio : IRepositorio<Material>
    {
        private const string Columnas = "id, nombre, unidad, stock_actual, stock_minimo, precio";

        public List<Material> Listar()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand($"SELECT {Columnas} FROM materiales ORDER BY nombre", cn);
            var lista = new List<Material>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Mapear(dr));
            return lista;
        }

        public Material ObtenerPorId(int id)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand($"SELECT {Columnas} FROM materiales WHERE id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Mapear(dr) : null;
        }

        public int Insertar(Material m)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "INSERT INTO materiales (nombre, unidad, stock_actual, stock_minimo, precio) " +
                "VALUES (@nombre, @unidad, @actual, @minimo, @precio)", cn);
            CargarParametros(cmd, m);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void Actualizar(Material m)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "UPDATE materiales SET nombre=@nombre, unidad=@unidad, stock_actual=@actual, " +
                "stock_minimo=@minimo, precio=@precio WHERE id=@id", cn);
            CargarParametros(cmd, m);
            cmd.Parameters.AddWithValue("@id", m.Id);
            cmd.ExecuteNonQuery();
        }

        private static void CargarParametros(MySqlCommand cmd, Material m)
        {
            cmd.Parameters.AddWithValue("@nombre", m.Nombre.Trim());
            cmd.Parameters.AddWithValue("@unidad", m.Unidad.Trim());
            cmd.Parameters.AddWithValue("@actual", m.StockActual);
            cmd.Parameters.AddWithValue("@minimo", m.StockMinimo);
            cmd.Parameters.AddWithValue("@precio", m.Precio);
        }

        private static Material Mapear(MySqlDataReader dr) => new Material
        {
            Id = dr.GetInt32("id"),
            Nombre = dr.GetString("nombre"),
            Unidad = dr.GetString("unidad"),
            StockActual = dr.GetDecimal("stock_actual"),
            StockMinimo = dr.GetDecimal("stock_minimo"),
            Precio = dr.GetDecimal("precio")
        };
    }
}
