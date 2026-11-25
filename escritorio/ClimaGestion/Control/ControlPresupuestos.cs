using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlPresupuestos
    {
        private ModeloPresupuestos modelo = new ModeloPresupuestos();

        public DataTable listar()
        {
            modelo.marcarVencidos();
            return modelo.listar();
        }

        public DataTable serviciosYMateriales()
        {
            return modelo.serviciosYMateriales();
        }

        public Presupuesto obtener(int id)
        {
            return modelo.obtener(id);
        }

        public string crear(Presupuesto p)
        {
            string error = p.Validar();
            if (error != "") return error;
            try
            {
                p.Id = modelo.insertar(p);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string aceptar(int id)
        {
            try
            {
                return modelo.aceptar(id) == 1 ? "" : "Solo se puede aceptar un presupuesto pendiente.";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string rechazar(int id)
        {
            try
            {
                return modelo.eliminarPendiente(id) == 1 ? "" : "Solo se puede rechazar un presupuesto pendiente.";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }
    }
}
