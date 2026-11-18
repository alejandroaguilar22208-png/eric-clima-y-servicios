using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public static class Conexion
    {
        private const string Cadena =
            "Server=localhost;Port=3306;Database=climagestion;User ID=root;Password=;CharSet=utf8mb4;";

        public static MySqlConnection Abrir()
        {
            var cn = new MySqlConnection(Cadena);
            cn.Open();
            return cn;
        }
    }

    public interface IRepositorio<T>
    {
        List<T> Listar();
        T ObtenerPorId(int id);
        int Insertar(T entidad);
        void Actualizar(T entidad);
    }
}
