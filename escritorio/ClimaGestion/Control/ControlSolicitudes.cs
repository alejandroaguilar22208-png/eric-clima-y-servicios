using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlSolicitudes
    {
        private ModeloSolicitudes modelo = new ModeloSolicitudes();

        public DataTable listar()
        {
            return modelo.listar();
        }

        public DataTable pendientes()
        {
            return modelo.pendientes();
        }

        public string marcarContactado(int id)
        {
            try
            {
                Solicitud s = modelo.obtener(id);
                if (s == null) return "Esa solicitud no existe.";
                if (s.Estado != "pendiente") return "Solo se puede marcar como contactada una solicitud pendiente.";
                modelo.marcarContactado(id);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string descartar(int id)
        {
            try
            {
                return modelo.eliminar(id) == 1 ? "" : "Esa solicitud ya fue convertida y no se puede descartar.";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string convertir(int id)
        {
            try
            {
                Solicitud s = modelo.obtener(id);
                if (s == null) return "Esa solicitud no existe.";
                if (s.EstaConvertida()) return "Esa solicitud ya fue convertida.";
                modelo.convertir(s);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }
    }
}
