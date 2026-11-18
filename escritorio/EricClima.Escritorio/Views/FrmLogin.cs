using EricClima.Escritorio.Controllers;

namespace EricClima.Escritorio.Views
{
    public class FrmLogin : Form
    {
        private readonly UsuarioController controller = new UsuarioController();
        private readonly TextBox txtEmail = new TextBox();
        private readonly TextBox txtClave = new TextBox { UseSystemPasswordChar = true };

        public FrmLogin()
        {
            Ui.Preparar(this, "Ingreso", 400, 340, encabezado: false);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var cabecera = new Panel { Dock = DockStyle.Top, Height = 140, BackColor = Ui.AzulOscuro };
            cabecera.Controls.Add(new Label
            {
                Text = "Panel de gestión", Dock = DockStyle.Top, Height = 26, ForeColor = Color.FromArgb(159, 179, 204),
                TextAlign = ContentAlignment.MiddleCenter
            });
            cabecera.Controls.Add(new Label
            {
                Text = "Eric Clima y Servicios", Dock = DockStyle.Top, Height = 34, ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleCenter
            });
            cabecera.Controls.Add(new Label
            {
                Text = "❄", Dock = DockStyle.Top, Height = 70, ForeColor = Color.White,
                Font = new Font("Segoe UI Symbol", 30F), TextAlign = ContentAlignment.BottomCenter
            });
            var linea = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = Ui.Naranja };

            var btnIngresar = Ui.Boton("Ingresar", Ingresar, principal: true);
            btnIngresar.AutoSize = false;
            btnIngresar.Size = new Size(368, 42);
            AcceptButton = btnIngresar;

            var campos = Ui.Campos(("Email", txtEmail), ("Contraseña", txtClave));
            campos.Padding = new Padding(16, 20, 16, 0);
            var barra = Ui.Barra(DockStyle.Bottom, btnIngresar);
            barra.BackColor = Color.White;
            barra.Padding = new Padding(12, 4, 12, 18);

            Controls.Add(campos);
            Controls.Add(barra);
            Controls.Add(linea);
            Controls.Add(cabecera);
        }

        private void Ingresar(object sender, EventArgs e)
        {
            Resultado r = controller.Login(txtEmail.Text, txtClave.Text);
            if (!r.Ok) { Ui.Mostrar(r); txtClave.Clear(); txtClave.Focus(); return; }
            DialogResult = DialogResult.OK;
        }
    }
}
