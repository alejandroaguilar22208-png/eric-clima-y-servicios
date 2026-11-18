using EricClima.Escritorio.Controllers;

namespace EricClima.Escritorio.Views
{
    public class FrmMiCuenta : Form
    {
        private readonly UsuarioController controller = new UsuarioController();
        private readonly TextBox txtNombre = new TextBox { MaxLength = 100 };
        private readonly TextBox txtEmail = new TextBox { MaxLength = 120 };
        private readonly TextBox txtNueva = new TextBox { UseSystemPasswordChar = true };
        private readonly TextBox txtRepetir = new TextBox { UseSystemPasswordChar = true };
        private readonly TextBox txtActual = new TextBox { UseSystemPasswordChar = true };

        public FrmMiCuenta()
        {
            Ui.Preparar(this, "Mi cuenta", 500, 380);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            txtNombre.Text = Sesion.UsuarioActual.Nombre;
            txtEmail.Text = Sesion.UsuarioActual.Email;

            var ayuda = new Label
            {
                Text = "Con este email y contraseña entrás al programa y al panel web. " +
                       "Dejá la contraseña nueva vacía si no la querés cambiar (mínimo 8 caracteres). " +
                       "Para guardar, escribí tu contraseña actual.",
                Dock = DockStyle.Top, Height = 64, Padding = new Padding(16, 10, 16, 0), ForeColor = Ui.TextoGris
            };

            var btnGuardar = Ui.Boton("Guardar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, e) => Close());
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            Controls.Add(Ui.Campos(("Nombre", txtNombre), ("Email", txtEmail),
                ("Contraseña nueva", txtNueva), ("Repetir nueva", txtRepetir), ("Contraseña actual *", txtActual)));
            Controls.Add(ayuda);
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            Resultado r = controller.CambiarCuenta(txtNombre.Text, txtEmail.Text, txtActual.Text, txtNueva.Text, txtRepetir.Text);
            Ui.Mostrar(r);
            if (!r.Ok) { txtActual.Clear(); txtActual.Focus(); return; }
            DialogResult = DialogResult.OK;
        }
    }
}
