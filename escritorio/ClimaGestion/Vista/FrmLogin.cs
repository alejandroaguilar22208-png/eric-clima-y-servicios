using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario u = new Usuario(txtEmail.Text, txtPassword.Text);
                ControlLogin control = new ControlLogin();
                bool validacion = control.usuarioValido(u);
                if (validacion)
                {
                    MessageBox.Show("¡Bienvenido, " + Sesion.UsuarioActual.Nombre + "!", "Inicio de sesión",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    FrmPrincipal principal = new FrmPrincipal(this);
                    txtPassword.Text = "";
                    this.Visible = false;
                    principal.Show();
                }
                else
                {
                    MessageBox.Show("Usuario o contraseña inválidos. Revisá los datos ingresados.", "Inicio de sesión",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (txtEmail.Text == "")
                        txtEmail.Focus();
                    else
                        txtPassword.Focus();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Inicio de sesión", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            txtEmail.Text = "";
            txtPassword.Text = "";
            txtEmail.Focus();
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show("Cerrar el sistema, ¿confirma?", "ClimaGestión",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
