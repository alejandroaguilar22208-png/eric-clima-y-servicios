using System;
using System.Data;
using ClimaGestion.Entidades;
using ClimaGestion.Modelo;

namespace ClimaGestion.Control
{
    public class ControlUsuarios
    {
        private ModeloUsuarios modelo = new ModeloUsuarios();

        public DataTable listar()
        {
            return modelo.listar();
        }

        public DataTable tipos()
        {
            return modelo.obtenerTipos();
        }

        public DataTable tecnicos()
        {
            return modelo.tecnicos();
        }

        public string guardar(Usuario u)
        {
            if (!Sesion.EsAdministrador()) return "Solo el administrador puede gestionar usuarios.";
            if (u.Id == 0 && u.Password == "") return "La contraseña es obligatoria para un usuario nuevo.";
            string error = u.Validar();
            if (error != "") return error;
            if (u.Id == Sesion.UsuarioActual.Id && (!u.Activo || u.IdTipo != Usuario.TipoAdministrador))
                return "No podés desactivarte ni quitarte el permiso de administrador.";

            try
            {
                u.PasswordHash = u.Password == "" ? "" : BCrypt.Net.BCrypt.HashPassword(u.Password);
                if (u.Id == 0) modelo.insertar(u);
                else modelo.actualizar(u);
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }

        public string cambiarMiCuenta(string nombre, string email, string claveActual, string claveNueva, string claveConfirma)
        {
            Usuario actual = Sesion.UsuarioActual;
            if (actual == null) return "Tenés que iniciar sesión.";
            if (string.IsNullOrEmpty(claveActual) || !BCrypt.Net.BCrypt.Verify(claveActual, actual.PasswordHash))
                return "La contraseña actual no es correcta.";

            Usuario cambios = new Usuario();
            cambios.Id = actual.Id;
            cambios.Nombre = nombre;
            cambios.Email = email;
            cambios.IdTipo = actual.IdTipo;
            cambios.Activo = true;
            cambios.Password = claveNueva;
            cambios.PasswordConfirma = claveConfirma;
            string error = cambios.Validar();
            if (error != "") return error;

            try
            {
                cambios.PasswordHash = claveNueva == "" ? "" : BCrypt.Net.BCrypt.HashPassword(claveNueva);
                modelo.actualizar(cambios);
                actual.Nombre = nombre.Trim();
                actual.Email = email.Trim();
                if (cambios.PasswordHash != "") actual.PasswordHash = cambios.PasswordHash;
                return "";
            }
            catch (Exception ex)
            {
                return Errores.Mensaje(ex);
            }
        }
    }
}
