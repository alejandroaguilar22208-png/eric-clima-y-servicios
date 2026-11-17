using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    /// <summary>
    /// Único lugar donde está la cadena de conexión a MySQL/MariaDB.
    /// Si la base está en otra PC, solo se cambia acá.
    /// </summary>
    public static class Conexion
    {
        private const string Cadena =
            "Server=localhost;Port=3306;Database=climagestion;User ID=root;Password=;CharSet=utf8mb4;";

        /// <summary>Devuelve una conexión nueva ya abierta.</summary>
        public static MySqlConnection Abrir()
        {
            var cn = new MySqlConnection(Cadena);
            cn.Open();
            return cn;
        }
    }

    /// <summary>
    /// INTERFAZ genérica: cualquier repositorio que la implemente
    /// promete tener estas operaciones (ABM). Es POLIMORFISMO.
    /// </summary>
    public interface IRepositorio<T>
    {
        List<T> Listar();
        T ObtenerPorId(int id);
        int Insertar(T entidad);
        void Actualizar(T entidad);
    }
}
