using System;
using MySql.Data.MySqlClient;

namespace ClimaGestion.Control
{
    public static class Errores
    {
        public static string Mensaje(Exception ex)
        {
            MySqlException error = ex as MySqlException;
            if (error != null)
            {
                if (error.Number == 1062 && error.Message.Contains("uq_tecnico_horario")) return "Ese técnico ya tiene un trabajo agendado en ese horario.";
                if (error.Number == 1062 && error.Message.Contains("email")) return "Ese email ya está registrado.";
                if (error.Number == 1062) return "Ya existe un registro con esos datos.";
                if (error.Number == 1451) return "No se puede borrar: tiene datos relacionados.";
                if (error.Number == 1042 || error.Number == 0) return "No se pudo conectar a la base de datos. Revisá que MySQL esté encendido en XAMPP.";
                return "Error de base de datos: " + error.Message;
            }
            return "Ocurrió un error: " + ex.Message;
        }
    }
}
