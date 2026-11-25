using System.Data;
using ClimaGestion.Entidades;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class ModeloClientes
    {
        public DataTable listar(string texto)
        {
            return Consultas.tabla(
                "SELECT id, nombre AS Nombre, telefono AS Teléfono, direccion AS Dirección, localidad AS Localidad, email AS Email " +
                "FROM clientes WHERE activo = 1 AND (nombre LIKE @t OR telefono LIKE @t OR direccion LIKE @t OR localidad LIKE @t) " +
                "ORDER BY nombre",
                new MySqlParameter("@t", "%" + texto.Trim() + "%"));
        }

        public DataTable paraCombo()
        {
            return Consultas.tabla("SELECT id, CONCAT(nombre, ' - ', direccion) AS nombre FROM clientes WHERE activo = 1 ORDER BY nombre");
        }

        public Cliente obtener(int id)
        {
            DataTable dt = Consultas.tabla("SELECT * FROM clientes WHERE id = @id", new MySqlParameter("@id", id));
            if (dt.Rows.Count == 0) return null;
            DataRow fila = dt.Rows[0];
            Cliente c = new Cliente();
            c.Id = id;
            c.Nombre = fila["nombre"].ToString();
            c.Telefono = fila["telefono"].ToString();
            c.Email = fila["email"].ToString();
            c.Direccion = fila["direccion"].ToString();
            c.Localidad = fila["localidad"].ToString();
            c.Activo = (bool)fila["activo"];
            return c;
        }

        public void insertar(Cliente c)
        {
            Consultas.ejecutar(
                "INSERT INTO clientes (nombre, telefono, email, direccion, localidad) VALUES (@nombre, @telefono, @email, @direccion, @localidad)",
                parametros(c));
        }

        public void actualizar(Cliente c)
        {
            MySqlParameter[] p = parametros(c);
            MySqlParameter[] conId = new MySqlParameter[p.Length + 1];
            p.CopyTo(conId, 0);
            conId[p.Length] = new MySqlParameter("@id", c.Id);
            Consultas.ejecutar(
                "UPDATE clientes SET nombre = @nombre, telefono = @telefono, email = @email, direccion = @direccion, localidad = @localidad WHERE id = @id",
                conId);
        }

        public void darDeBaja(int id)
        {
            Consultas.ejecutar("UPDATE clientes SET activo = 0 WHERE id = @id", new MySqlParameter("@id", id));
        }

        public int cantidadActivos()
        {
            return System.Convert.ToInt32(Consultas.valor("SELECT COUNT(*) FROM clientes WHERE activo = 1"));
        }

        private MySqlParameter[] parametros(Cliente c)
        {
            return new MySqlParameter[]
            {
                new MySqlParameter("@nombre", c.Nombre.Trim()),
                new MySqlParameter("@telefono", c.Telefono.Trim()),
                new MySqlParameter("@email", string.IsNullOrWhiteSpace(c.Email) ? null : c.Email.Trim()),
                new MySqlParameter("@direccion", c.Direccion.Trim()),
                new MySqlParameter("@localidad", c.Localidad.Trim())
            };
        }
    }
}
