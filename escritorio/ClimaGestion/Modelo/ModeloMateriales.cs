using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloMateriales
    {
        public DataTable listar()
        {
            return Consultas.tabla(
                "SELECT id, nombre AS Material, unidad AS Unidad, stock_actual AS Stock, stock_minimo AS Mínimo, precio AS Precio " +
                "FROM materiales ORDER BY stock_actual <= stock_minimo DESC, nombre");
        }

        public DataTable paraCombo()
        {
            return Consultas.tabla("SELECT id, CONCAT(nombre, ' (stock: ', stock_actual + 0, ' ', unidad, ')') AS nombre, precio FROM materiales ORDER BY nombre");
        }

        public DataTable stockBajo()
        {
            return Consultas.tabla("SELECT nombre, stock_actual, stock_minimo, unidad FROM materiales WHERE stock_actual <= stock_minimo ORDER BY nombre");
        }

        public Material obtener(int id)
        {
            DataTable dt = Consultas.tabla("SELECT * FROM materiales WHERE id = @id", new MySqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow fila = dt.Rows[0];
            Material m = new Material();
            m.Id = id;
            m.Nombre = fila["nombre"].ToString();
            m.Unidad = fila["unidad"].ToString();
            m.StockActual = (decimal)fila["stock_actual"];
            m.StockMinimo = (decimal)fila["stock_minimo"];
            m.Precio = (decimal)fila["precio"];
            return m;
        }

        public void insertar(Material m)
        {
            Consultas.ejecutar(
                "INSERT INTO materiales (nombre, unidad, stock_actual, stock_minimo, precio) VALUES (@nombre, @unidad, @actual, @minimo, @precio)",
                new MySqlParameter("@nombre", m.Nombre.Trim()),
                new MySqlParameter("@unidad", m.Unidad),
                new MySqlParameter("@actual", m.StockActual),
                new MySqlParameter("@minimo", m.StockMinimo),
                new MySqlParameter("@precio", m.Precio));
        }

        public void actualizar(Material m)
        {
            Consultas.ejecutar(
                "UPDATE materiales SET nombre = @nombre, unidad = @unidad, stock_actual = @actual, stock_minimo = @minimo, precio = @precio WHERE id = @id",
                new MySqlParameter("@nombre", m.Nombre.Trim()),
                new MySqlParameter("@unidad", m.Unidad),
                new MySqlParameter("@actual", m.StockActual),
                new MySqlParameter("@minimo", m.StockMinimo),
                new MySqlParameter("@precio", m.Precio),
                new MySqlParameter("@id", m.Id));
        }
    }
}
