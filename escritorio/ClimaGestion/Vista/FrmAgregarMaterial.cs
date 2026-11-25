using System;
using System.Windows.Forms;
using ClimaGestion.Control;

namespace ClimaGestion.Vista
{
    public partial class FrmAgregarMaterial : Form
    {
        private ControlTrabajos control = new ControlTrabajos();
        private int idTrabajo;

        public FrmAgregarMaterial(int idTrabajo)
        {
            InitializeComponent();
            this.idTrabajo = idTrabajo;
        }

        private void FrmAgregarMaterial_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = "      Material para el trabajo N° " + idTrabajo;
            try
            {
                cboMaterial.ValueMember = "id";
                cboMaterial.DisplayMember = "nombre";
                cboMaterial.DataSource = new ControlStock().paraCombo();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Agregar material", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            int idMaterial = cboMaterial.SelectedValue == null ? 0 : Convert.ToInt32(cboMaterial.SelectedValue);
            string error = control.agregarMaterial(idTrabajo, idMaterial, numCantidad.Value);
            if (error != "")
            {
                MessageBox.Show(error, "Agregar material", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MessageBox.Show("Material agregado.", "Agregar material", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
