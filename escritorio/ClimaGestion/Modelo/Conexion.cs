using System.Configuration;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public class Conexion
    {
        private const string servidor = "server=localhost";
        private const string puerto = "port=3306";
        private const string usuario = "user=root";
        private const string password = "password=";
        private const string baseDeDatos = "database=climagestion";
        private const string juegoDeCaracteres = "charset=utf8mb4";

        private string cadenaConexion;

        public Conexion()
        {
            ConnectionStringSettings configurada = ConfigurationManager.ConnectionStrings["climagestion"];
            if (configurada != null && configurada.ConnectionString != "")
                cadenaConexion = configurada.ConnectionString;
            else
                cadenaConexion = servidor + ";" + puerto + ";" + usuario + ";" + password + ";" + baseDeDatos + ";" + juegoDeCaracteres;
        }

        public MySqlConnection getConexion()
        {
            return new MySqlConnection(cadenaConexion);
        }
    }
}
