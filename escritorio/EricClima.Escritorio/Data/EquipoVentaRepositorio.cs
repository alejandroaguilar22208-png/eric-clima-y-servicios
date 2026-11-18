using EricClima.Escritorio.Models;
using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public class EquipoVentaRepositorio : IRepositorio<EquipoVenta>
    {
        private const string Columnas =
            "id, marca, modelo, tipo, frigorias, frio_calor, inverter, precio, stock, descripcion, activo";

        private static readonly Dictionary<string, TipoEquipoVenta> Tipos = new Dictionary<string, TipoEquipoVenta>
        {
            ["split"] = TipoEquipoVenta.Split,
            ["ventana"] = TipoEquipoVenta.Ventana,
            ["portatil"] = TipoEquipoVenta.Portatil,
            ["piso_techo"] = TipoEquipoVenta.PisoTecho
        };

        public List<EquipoVenta> Listar()
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand($"SELECT {Columnas} FROM equipos_venta ORDER BY activo DESC, marca, frigorias", cn);
            var lista = new List<EquipoVenta>();
            using var dr = cmd.ExecuteReader();
            while (dr.Read()) lista.Add(Mapear(dr));
            return lista;
        }

        public EquipoVenta ObtenerPorId(int id)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand($"SELECT {Columnas} FROM equipos_venta WHERE id = @id", cn);
            cmd.Parameters.AddWithValue("@id", id);
            using var dr = cmd.ExecuteReader();
            return dr.Read() ? Mapear(dr) : null;
        }

        public int Insertar(EquipoVenta e)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "INSERT INTO equipos_venta (marca, modelo, tipo, frigorias, frio_calor, inverter, precio, stock, descripcion, activo) " +
                "VALUES (@marca, @modelo, @tipo, @frigorias, @frioCalor, @inverter, @precio, @stock, @descripcion, @activo)", cn);
            CargarParametros(cmd, e);
            cmd.ExecuteNonQuery();
            return (int)cmd.LastInsertedId;
        }

        public void Actualizar(EquipoVenta e)
        {
            using var cn = Conexion.Abrir();
            using var cmd = new MySqlCommand(
                "UPDATE equipos_venta SET marca=@marca, modelo=@modelo, tipo=@tipo, frigorias=@frigorias, frio_calor=@frioCalor, " +
                "inverter=@inverter, precio=@precio, stock=@stock, descripcion=@descripcion, activo=@activo WHERE id=@id", cn);
            CargarParametros(cmd, e);
            cmd.Parameters.AddWithValue("@id", e.Id);
            cmd.ExecuteNonQuery();
        }

        private static void CargarParametros(MySqlCommand cmd, EquipoVenta e)
        {
            cmd.Parameters.AddWithValue("@marca", e.Marca.Trim());
            cmd.Parameters.AddWithValue("@modelo", e.Modelo.Trim());
            cmd.Parameters.AddWithValue("@tipo", Tipos.First(t => t.Value == e.Tipo).Key);
            cmd.Parameters.AddWithValue("@frigorias", e.Frigorias);
            cmd.Parameters.AddWithValue("@frioCalor", e.FrioCalor);
            cmd.Parameters.AddWithValue("@inverter", e.Inverter);
            cmd.Parameters.AddWithValue("@precio", e.Precio);
            cmd.Parameters.AddWithValue("@stock", e.Stock);
            cmd.Parameters.AddWithValue("@descripcion", string.IsNullOrWhiteSpace(e.Descripcion) ? DBNull.Value : e.Descripcion.Trim());
            cmd.Parameters.AddWithValue("@activo", e.Activo);
        }

        private static EquipoVenta Mapear(MySqlDataReader dr) => new EquipoVenta
        {
            Id = dr.GetInt32("id"),
            Marca = dr.GetString("marca"),
            Modelo = dr.GetString("modelo"),
            Tipo = Tipos.TryGetValue(dr.GetString("tipo"), out var t) ? t : TipoEquipoVenta.Split,
            Frigorias = dr.GetInt32("frigorias"),
            FrioCalor = dr.GetBoolean("frio_calor"),
            Inverter = dr.GetBoolean("inverter"),
            Precio = dr.GetDecimal("precio"),
            Stock = dr.GetInt32("stock"),
            Descripcion = dr.IsDBNull(dr.GetOrdinal("descripcion")) ? "" : dr.GetString("descripcion"),
            Activo = dr.GetBoolean("activo")
        };
    }
}
