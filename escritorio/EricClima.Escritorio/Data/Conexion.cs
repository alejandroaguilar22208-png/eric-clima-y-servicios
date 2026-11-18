using MySqlConnector;

namespace EricClima.Escritorio.Data
{
    public static class Conexion
    {
        private const string CadenaPorDefecto =
            "Server=localhost;Port=3306;Database=climagestion;User ID=root;Password=;CharSet=utf8mb4;";

        public static string Cadena { get; } = LeerCadena();

        private static string LeerCadena()
        {
            string archivo = Path.Combine(AppContext.BaseDirectory, "conexion.txt");
            if (!File.Exists(archivo)) return CadenaPorDefecto;
            string linea = File.ReadAllLines(archivo)
                .Select(l => l.Trim())
                .FirstOrDefault(l => l != "" && !l.StartsWith("#"));
            return string.IsNullOrEmpty(linea) ? CadenaPorDefecto : linea;
        }

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
