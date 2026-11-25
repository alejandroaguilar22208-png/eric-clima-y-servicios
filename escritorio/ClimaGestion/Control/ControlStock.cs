using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlStock
    {
        private ModeloMateriales modelo = new ModeloMateriales();

        public DataTable listar()
        {
            return modelo.listar();
        }

        public DataTable paraCombo()
        {
            return modelo.paraCombo();
        }

        public DataTable stockBajo()
        {
            return modelo.stockBajo();
        }

        public Material obtener(int id)
        {
            return modelo.obtener(id);
        }

        public string guardar(Material m)
        {
            if (!Sesion.EsAdministrador()) return "Solo el administrador puede modificar el stock.";
            string error = m.Validar();
            if (error != "") return error;
            try
            {
                if (m.Id == 0) modelo.insertar(m);
                else modelo.actualizar(m);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }
    }
}
