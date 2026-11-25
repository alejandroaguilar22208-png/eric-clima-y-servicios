using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlClientes
    {
        private ModeloClientes modelo = new ModeloClientes();

        public DataTable listar(string texto)
        {
            return modelo.listar(texto);
        }

        public DataTable paraCombo()
        {
            return modelo.paraCombo();
        }

        public Cliente obtener(int id)
        {
            return modelo.obtener(id);
        }

        public string guardar(Cliente c)
        {
            string error = c.Validar();
            if (error != "") return error;
            try
            {
                if (c.Id == 0) modelo.insertar(c);
                else modelo.actualizar(c);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string darDeBaja(int id)
        {
            if (!Sesion.EsAdministrador()) return "Solo el administrador puede dar de baja clientes.";
            try
            {
                modelo.darDeBaja(id);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }
    }
}
