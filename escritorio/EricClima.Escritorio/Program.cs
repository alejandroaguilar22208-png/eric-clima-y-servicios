using EricClima.Escritorio.Views;

namespace EricClima.Escritorio
{
    internal static class Program
    {
        /// <summary>Punto de entrada: primero el login, si sale bien abre el menú.</summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return; // cerró sin ingresar
            }
            Application.Run(new FrmPrincipal());
        }
    }
}
