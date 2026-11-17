using EricClima.Escritorio.Controllers;

namespace EricClima.Escritorio.Views
{
    /// <summary>VISTA de ingreso. Solo junta los datos y le pregunta al controlador.</summary>
    public class FrmLogin : Form
    {
        private readonly UsuarioController controller = new UsuarioController();
        private readonly TextBox txtEmail = new TextBox();
        private readonly TextBox txtClave = new TextBox { UseSystemPasswordChar = true };

        public FrmLogin()
        {
            Ui.Preparar(this, "Ingreso", 380, 300);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            var titulo = new Label
            {
                Text = "Eric Clima y Servicios",
                Dock = DockStyle.Top,
                Height = 70,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Ui.Azul
            };
            var btnIngresar = Ui.Boton("Ingresar", Ingresar, principal: true);
            AcceptButton = btnIngresar; // Enter = ingresar

            Controls.Add(Ui.Campos(("Email", txtEmail), ("Contraseña", txtClave)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnIngresar));
            Controls.Add(titulo);
        }

        private void Ingresar(object sender, EventArgs e)
        {
            Resultado r = controller.Login(txtEmail.Text, txtClave.Text);
            if (!r.Ok) { Ui.Mostrar(r); txtClave.Clear(); txtClave.Focus(); return; }
            DialogResult = DialogResult.OK; // cierra el login y Program.cs abre el menú
        }
    }
}
