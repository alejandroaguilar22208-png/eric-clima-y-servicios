using System;
using System.Windows.Forms;
using ClimaGestion.Control;

namespace ClimaGestion.Vista
{
    public partial class FrmAgendar : Form
    {
        private ControlTrabajos control = new ControlTrabajos();
        private int idTrabajo;

        public FrmAgendar(int idTrabajo)
        {
            InitializeComponent();
            this.idTrabajo = idTrabajo;
        }

        private void FrmAgendar_Load(object sender, EventArgs e)
        {
            lblTitulo.Text = "      Agendar trabajo N° " + idTrabajo;
            dtpFecha.Value = DateTime.Today.AddDays(1).AddHours(9);
            try
            {
                cboTecnico.ValueMember = "id";
                cboTecnico.DisplayMember = "nombre";
                cboTecnico.DataSource = new ControlUsuarios().tecnicos();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Agendar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            int idTecnico = cboTecnico.SelectedValue == null ? 0 : Convert.ToInt32(cboTecnico.SelectedValue);
            string error = control.agendar(idTrabajo, dtpFecha.Value, idTecnico);
            if (error != "")
            {
                MessageBox.Show(error, "Agendar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
