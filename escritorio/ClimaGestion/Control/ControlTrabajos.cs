using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlTrabajos
    {
        private ModeloTrabajos modelo = new ModeloTrabajos();

        public DataTable listar(string estado)
        {
            return modelo.listar(estado);
        }

        public DataTable servicios()
        {
            return modelo.servicios();
        }

        public DataTable materialesDe(int idTrabajo)
        {
            return modelo.materialesDe(idTrabajo);
        }

        public DataTable agendadosHoy()
        {
            return modelo.agendadosHoy();
        }

        public DataTable pendientes()
        {
            return modelo.pendientes();
        }

        public string crear(Trabajo t)
        {
            if (t.IdCliente == 0) return "Elegí un cliente.";
            if (t.IdServicio == 0) return "Elegí un servicio.";
            if (t.Total < 0) return "El total no puede ser negativo.";
            try
            {
                modelo.insertar(t);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string agendar(int idTrabajo, DateTime fecha, int idTecnico)
        {
            string error = verificarAbierto(idTrabajo);
            if (error != "") return error;
            if (idTecnico == 0) return "Elegí un técnico.";
            if (fecha < DateTime.Today) return "La fecha no puede ser anterior a hoy.";
            try
            {
                modelo.agendar(idTrabajo, fecha, idTecnico);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string agregarMaterial(int idTrabajo, int idMaterial, decimal cantidad)
        {
            string error = verificarAbierto(idTrabajo);
            if (error != "") return error;
            if (idMaterial == 0) return "Elegí un material.";
            if (cantidad <= 0) return "La cantidad tiene que ser mayor a 0.";
            try
            {
                decimal yaCargado = modelo.cantidadCargada(idTrabajo, idMaterial);
                if (yaCargado + cantidad > modelo.stockDisponible(idMaterial))
                    return "Stock insuficiente para el material solicitado.";
                modelo.agregarMaterial(idTrabajo, idMaterial, cantidad);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string marcarRealizado(int idTrabajo)
        {
            string error = verificarAbierto(idTrabajo);
            if (error != "") return error;
            try
            {
                Trabajo t = modelo.obtener(idTrabajo);
                if (!t.EstaAgendado()) return "Primero agendá el trabajo con fecha y técnico.";
                foreach (DataRow fila in modelo.materialesDe(idTrabajo).Rows)
                {
                    if ((decimal)fila["cantidad"] > (decimal)fila["stock_actual"])
                        return "No alcanza el stock de " + fila["nombre"] + ".";
                }
                modelo.marcarRealizado(idTrabajo);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string cancelar(int idTrabajo)
        {
            string error = verificarAbierto(idTrabajo);
            if (error != "") return error;
            try
            {
                modelo.cancelar(idTrabajo);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        private string verificarAbierto(int idTrabajo)
        {
            Trabajo t = modelo.obtener(idTrabajo);
            if (t == null) return "Ese trabajo no existe.";
            if (t.EstaCerrado()) return "Un trabajo realizado o cancelado no se puede modificar.";
            return "";
        }
    }
}
