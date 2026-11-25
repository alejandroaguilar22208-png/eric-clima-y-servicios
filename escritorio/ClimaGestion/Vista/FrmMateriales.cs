using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmMateriales : Form
    {
        private ControlStock control = new ControlStock();

        public FrmMateriales()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmMateriales_Load(object sender, EventArgs e)
        {
            btnNuevo.Enabled = Sesion.EsAdministrador();
            btnEditar.Enabled = Sesion.EsAdministrador();
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                dgvDatos.DataSource = control.listar();
                Estilos.OcultarColumnas(dgvDatos, "id");
                Estilos.FormatoPlata(dgvDatos, "Precio");
                foreach (DataGridViewRow fila in dgvDatos.Rows)
                {
                    if (Convert.ToDecimal(fila.Cells["Stock"].Value) <= Convert.ToDecimal(fila.Cells["Mínimo"].Value))
                        fila.DefaultCellStyle.BackColor = Estilos.Rojo;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Stock", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmMaterialEditar editor = new FrmMaterialEditar(new Material());
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!Sesion.EsAdministrador()) return;
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0)
            {
                MessageBox.Show("Seleccioná un material.", "Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FrmMaterialEditar editor = new FrmMaterialEditar(control.obtener(id));
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnEditar_Click(sender, e);
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmMateriales_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
