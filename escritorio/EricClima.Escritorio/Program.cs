using EricClima.Escritorio.Views;

namespace EricClima.Escritorio
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            using (var login = new FrmLogin())
            {
                if (login.ShowDialog() != DialogResult.OK) return;
            }
            Application.Run(new FrmPrincipal());
        }
    }
}
