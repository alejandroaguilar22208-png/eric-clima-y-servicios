using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmEquiposVenta : Form
    {
        private ControlEquiposVenta control = new ControlEquiposVenta();

        public FrmEquiposVenta()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmEquiposVenta_Load(object sender, EventArgs e)
        {
            btnNuevo.Enabled = Sesion.EsAdministrador();
            btnEditar.Enabled = Sesion.EsAdministrador();
            btnVisible.Enabled = Sesion.EsAdministrador();
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
                    if (fila.Cells["Web"].Value.ToString() == "Oculto")
                        fila.DefaultCellStyle.BackColor = Estilos.Gris;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Equipos en venta", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmEquipoVentaEditar editor = new FrmEquipoVentaEditar(new EquipoVenta());
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (!Sesion.EsAdministrador()) return;
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0)
            {
                MessageBox.Show("Seleccioná un equipo.", "Equipos en venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FrmEquipoVentaEditar editor = new FrmEquipoVentaEditar(control.obtener(id));
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnEditar_Click(sender, e);
        }

        private void btnVisible_Click(object sender, EventArgs e)
        {
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0)
            {
                MessageBox.Show("Seleccioná un equipo.", "Equipos en venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string error = control.cambiarVisible(id);
            if (error != "")
                MessageBox.Show(error, "Equipos en venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmEquiposVenta_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
