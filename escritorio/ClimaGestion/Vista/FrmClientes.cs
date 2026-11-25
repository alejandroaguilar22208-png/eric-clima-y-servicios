using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmClientes : Form
    {
        private ControlClientes control = new ControlClientes();

        public FrmClientes()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmClientes_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                dgvDatos.DataSource = control.listar(txtBuscar.Text);
                Estilos.OcultarColumnas(dgvDatos, "id");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void txtBuscar_TextChanged(object sender, EventArgs e)
        {
            Cargar();
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmClienteEditar editor = new FrmClienteEditar(new Cliente());
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0)
            {
                MessageBox.Show("Seleccioná un cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            FrmClienteEditar editor = new FrmClienteEditar(control.obtener(id));
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnEditar_Click(sender, e);
        }

        private void btnBaja_Click(object sender, EventArgs e)
        {
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0)
            {
                MessageBox.Show("Seleccioná un cliente.", "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            string nombre = Estilos.TextoSeleccionado(dgvDatos, "Nombre");
            DialogResult resp = MessageBox.Show("¿Dar de baja a " + nombre + "? Su historial se conserva.", "Clientes",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;
            string error = control.darDeBaja(id);
            if (error != "")
                MessageBox.Show(error, "Clientes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmClientes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
