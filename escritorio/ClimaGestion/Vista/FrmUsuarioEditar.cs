using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmUsuarioEditar : Form
    {
        private ControlUsuarios control = new ControlUsuarios();
        private Usuario usuario;

        public FrmUsuarioEditar(Usuario usuario)
        {
            InitializeComponent();
            this.usuario = usuario;
        }

        private void FrmUsuarioEditar_Load(object sender, EventArgs e)
        {
            cargarTiposEnComboBox();
            lblTitulo.Text = usuario.Id == 0 ? "      Nuevo usuario" : "      Editar usuario";
            txtNombre.Text = usuario.Nombre;
            txtEmail.Text = usuario.Email;
            cboTipo.SelectedValue = usuario.IdTipo;
            chkActivo.Checked = usuario.Activo;
        }

        private void cargarTiposEnComboBox()
        {
            try
            {
                cboTipo.ValueMember = "id";
                cboTipo.DisplayMember = "nombre";
                cboTipo.DataSource = control.tipos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Usuario", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (txtNombre.Text.Trim() == "") { errorProvider.SetError(txtNombre, "El nombre es obligatorio."); txtNombre.Focus(); return; }
            if (txtEmail.Text.Trim() == "") { errorProvider.SetError(txtEmail, "El email es obligatorio."); txtEmail.Focus(); return; }

            usuario.Nombre = txtNombre.Text;
            usuario.Email = txtEmail.Text;
            usuario.IdTipo = Convert.ToInt32(cboTipo.SelectedValue);
            usuario.Password = txtPassword.Text;
            usuario.PasswordConfirma = txtConfirma.Text;
            usuario.Activo = chkActivo.Checked;

            string error = control.guardar(usuario);
            if (error != "")
            {
                MessageBox.Show(error, "Usuario", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Usuario guardado.", "Usuario", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
