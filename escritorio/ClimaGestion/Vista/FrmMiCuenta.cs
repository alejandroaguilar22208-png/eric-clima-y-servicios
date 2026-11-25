using System;
using System.Windows.Forms;
using ClimaGestion.Control;

namespace ClimaGestion.Vista
{
    public partial class FrmMiCuenta : Form
    {
        private ControlUsuarios control = new ControlUsuarios();

        public FrmMiCuenta()
        {
            InitializeComponent();
        }

        private void FrmMiCuenta_Load(object sender, EventArgs e)
        {
            txtNombre.Text = Sesion.UsuarioActual.Nombre;
            txtEmail.Text = Sesion.UsuarioActual.Email;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (txtNombre.Text.Trim() == "") { errorProvider.SetError(txtNombre, "El nombre es obligatorio."); txtNombre.Focus(); return; }
            if (txtEmail.Text.Trim() == "") { errorProvider.SetError(txtEmail, "El email es obligatorio."); txtEmail.Focus(); return; }
            if (txtActual.Text == "") { errorProvider.SetError(txtActual, "Para guardar escribí tu contraseña actual."); txtActual.Focus(); return; }

            string error = control.cambiarMiCuenta(txtNombre.Text, txtEmail.Text, txtActual.Text, txtNueva.Text, txtConfirma.Text);
            if (error != "")
            {
                MessageBox.Show(error, "Mi cuenta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtActual.Text = "";
                return;
            }
            MessageBox.Show("Tus datos se guardaron. La próxima vez ingresá con el email y la contraseña nuevos.", "Mi cuenta",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
