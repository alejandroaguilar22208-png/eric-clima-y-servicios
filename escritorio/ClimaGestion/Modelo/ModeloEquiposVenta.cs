using System;
using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloEquiposVenta
    {
        public DataTable listar()
        {
            return Consultas.tabla(
                "SELECT id, marca AS Marca, modelo AS Modelo, tipo AS Tipo, frigorias AS Frigorías, " +
                "IF(frio_calor = 1, 'Frío/calor', 'Solo frío') AS Clima, IF(inverter = 1, 'Sí', 'No') AS Inverter, " +
                "precio AS Precio, stock AS Stock, IF(activo = 1, 'Visible', 'Oculto') AS Web " +
                "FROM equipos_venta ORDER BY activo DESC, marca, frigorias");
        }

        public EquipoVenta obtener(int id)
        {
            DataTable dt = Consultas.tabla("SELECT * FROM equipos_venta WHERE id = @id", new MySqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow fila = dt.Rows[0];
            EquipoVenta e = new EquipoVenta();
            e.Id = id;
            e.Marca = fila["marca"].ToString();
            e.Modelo = fila["modelo"].ToString();
            e.Tipo = fila["tipo"].ToString();
            e.Frigorias = Convert.ToInt32(fila["frigorias"]);
            e.FrioCalor = Convert.ToBoolean(fila["frio_calor"]);
            e.Inverter = Convert.ToBoolean(fila["inverter"]);
            e.Precio = (decimal)fila["precio"];
            e.Stock = Convert.ToInt32(fila["stock"]);
            e.Descripcion = fila["descripcion"].ToString();
            e.Activo = Convert.ToBoolean(fila["activo"]);
            return e;
        }

        public void insertar(EquipoVenta e)
        {
            Consultas.ejecutar(
                "INSERT INTO equipos_venta (marca, modelo, tipo, frigorias, frio_calor, inverter, precio, stock, descripcion, activo) " +
                "VALUES (@marca, @modelo, @tipo, @frigorias, @frioCalor, @inverter, @precio, @stock, @descripcion, @activo)",
                parametros(e));
        }

        public void actualizar(EquipoVenta e)
        {
            MySqlParameter[] p = parametros(e);
            MySqlParameter[] conId = new MySqlParameter[p.Length + 1];
            p.CopyTo(conId, 0);
            conId[p.Length] = new MySqlParameter("@id", e.Id);
            Consultas.ejecutar(
                "UPDATE equipos_venta SET marca = @marca, modelo = @modelo, tipo = @tipo, frigorias = @frigorias, frio_calor = @frioCalor, " +
                "inverter = @inverter, precio = @precio, stock = @stock, descripcion = @descripcion, activo = @activo WHERE id = @id",
                conId);
        }

        private MySqlParameter[] parametros(EquipoVenta e)
        {
            return new MySqlParameter[]
            {
                new MySqlParameter("@marca", e.Marca.Trim()),
                new MySqlParameter("@modelo", e.Modelo.Trim()),
                new MySqlParameter("@tipo", e.Tipo),
                new MySqlParameter("@frigorias", e.Frigorias),
                new MySqlParameter("@frioCalor", e.FrioCalor),
                new MySqlParameter("@inverter", e.Inverter),
                new MySqlParameter("@precio", e.Precio),
                new MySqlParameter("@stock", e.Stock),
                new MySqlParameter("@descripcion", string.IsNullOrWhiteSpace(e.Descripcion) ? null : e.Descripcion.Trim()),
                new MySqlParameter("@activo", e.Activo)
            };
        }
    }
}
