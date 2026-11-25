using System;
using System.Data;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmTrabajoNuevo : Form
    {
        private ControlTrabajos control = new ControlTrabajos();

        public FrmTrabajoNuevo()
        {
            InitializeComponent();
        }

        private void FrmTrabajoNuevo_Load(object sender, EventArgs e)
        {
            try
            {
                cboCliente.ValueMember = "id";
                cboCliente.DisplayMember = "nombre";
                cboCliente.DataSource = new ControlClientes().paraCombo();
                cboServicio.ValueMember = "id";
                cboServicio.DisplayMember = "nombre";
                cboServicio.DataSource = control.servicios();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Nuevo trabajo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboServicio_SelectedIndexChanged(object sender, EventArgs e)
        {
            DataRowView fila = cboServicio.SelectedItem as DataRowView;
            if (fila != null) numTotal.Value = Convert.ToDecimal(fila["precio_base"]);
        }

        private void btnCrear_Click(object sender, EventArgs e)
        {
            if (cboCliente.SelectedValue == null)
            {
                MessageBox.Show("Primero cargá un cliente.", "Nuevo trabajo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Trabajo t = new Trabajo();
            t.IdCliente = Convert.ToInt32(cboCliente.SelectedValue);
            t.IdServicio = Convert.ToInt32(cboServicio.SelectedValue);
            t.Total = numTotal.Value;
            t.Observaciones = txtObservaciones.Text;
            string error = control.crear(t);
            if (error != "")
            {
                MessageBox.Show(error, "Nuevo trabajo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
