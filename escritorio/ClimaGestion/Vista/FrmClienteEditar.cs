using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmClienteEditar : Form
    {
        private ControlClientes control = new ControlClientes();
        private Cliente cliente;

        public FrmClienteEditar(Cliente cliente)
        {
            InitializeComponent();
            this.cliente = cliente;
        }

        private void FrmClienteEditar_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = cliente.Id == 0 ? "      Nuevo cliente" : "      Editar cliente";
            txtNombre.Text = cliente.Nombre;
            txtTelefono.Text = cliente.Telefono;
            txtEmail.Text = cliente.Email;
            txtDireccion.Text = cliente.Direccion;
            txtLocalidad.Text = cliente.Localidad;
        }

        private bool CamposCompletos()
        {
            errorProvider.Clear();
            if (txtNombre.Text.Trim() == "") { errorProvider.SetError(txtNombre, "El nombre es obligatorio."); txtNombre.Focus(); return false; }
            if (txtTelefono.Text.Trim() == "") { errorProvider.SetError(txtTelefono, "El teléfono es obligatorio."); txtTelefono.Focus(); return false; }
            if (txtDireccion.Text.Trim() == "") { errorProvider.SetError(txtDireccion, "La dirección es obligatoria."); txtDireccion.Focus(); return false; }
            return true;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            if (!CamposCompletos()) return;
            cliente.Nombre = txtNombre.Text;
            cliente.Telefono = txtTelefono.Text;
            cliente.Email = txtEmail.Text;
            cliente.Direccion = txtDireccion.Text;
            cliente.Localidad = txtLocalidad.Text == "" ? "Bahía Blanca" : txtLocalidad.Text;

            string error = control.guardar(cliente);
            if (error != "")
            {
                MessageBox.Show(error, "Cliente", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
