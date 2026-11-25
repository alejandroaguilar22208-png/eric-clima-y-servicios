using System;
using System.Data;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Modelo
{
    public static class Consultas
    {
        public static DataTable tabla(string consulta, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlCommand comando = new MySqlCommand(consulta, conectar);
                comando.Parameters.AddRange(parametros);
                MySqlDataAdapter adaptador = new MySqlDataAdapter(comando);
                DataTable dt = new DataTable();
                adaptador.Fill(dt);
                return dt;
            }
        }

        public static int ejecutar(string consulta, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlCommand comando = new MySqlCommand(consulta, conectar);
                comando.Parameters.AddRange(parametros);
                return comando.ExecuteNonQuery();
            }
        }

        public static int insertar(string consulta, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlCommand comando = new MySqlCommand(consulta, conectar);
                comando.Parameters.AddRange(parametros);
                comando.ExecuteNonQuery();
                return (int)comando.LastInsertedId;
            }
        }

        public static object valor(string consulta, params MySqlParameter[] parametros)
        {
            using (MySqlConnection conectar = new Conexion().getConexion())
            {
                conectar.Open();
                MySqlCommand comando = new MySqlCommand(consulta, conectar);
                comando.Parameters.AddRange(parametros);
                object resultado = comando.ExecuteScalar();
                return resultado == DBNull.Value ? null : resultado;
            }
        }
    }
}
