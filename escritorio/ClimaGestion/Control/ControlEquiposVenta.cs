using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlEquiposVenta
    {
        private ModeloEquiposVenta modelo = new ModeloEquiposVenta();

        public DataTable listar()
        {
            return modelo.listar();
        }

        public EquipoVenta obtener(int id)
        {
            return modelo.obtener(id);
        }

        public string guardar(EquipoVenta e)
        {
            if (!Sesion.EsAdministrador()) return "Solo el administrador puede modificar los equipos en venta.";
            string error = e.Validar();
            if (error != "") return error;
            try
            {
                if (e.Id == 0) modelo.insertar(e);
                else modelo.actualizar(e);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string cambiarVisible(int id)
        {
            if (!Sesion.EsAdministrador()) return "Solo el administrador puede modificar los equipos en venta.";
            try
            {
                EquipoVenta e = modelo.obtener(id);
                if (e == null) return "Ese equipo no existe.";
                e.Activo = !e.Activo;
                modelo.actualizar(e);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }
    }
}
