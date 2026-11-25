using System;
using System.Threading;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Vista;

namespace ClimaGestion
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += ErrorNoControlado;
            Application.Run(new FrmLogin());
        }

        private static void ErrorNoControlado(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(Errores.Mensaje(e.Exception), "ClimaGestión", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
