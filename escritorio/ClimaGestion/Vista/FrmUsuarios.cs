using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmUsuarios : Form
    {
        private ControlUsuarios control = new ControlUsuarios();

        public FrmUsuarios()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmUsuarios_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                dgvDatos.DataSource = control.listar();
                Estilos.OcultarColumnas(dgvDatos, "id", "tipo_usuario_id", "activo");
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmUsuarioEditar editor = new FrmUsuarioEditar(new Usuario());
            if (editor.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow == null)
            {
                MessageBox.Show("Seleccioná un usuario.", "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            DataGridViewRow fila = dgvDatos.CurrentRow;
            Usuario u = new Usuario();
            u.Id = Convert.ToInt32(fila.Cells["id"].Value);
            u.Nombre = fila.Cells["Nombre"].Value.ToString();
            u.Email = fila.Cells["Email"].Value.ToString();
            u.IdTipo = Convert.ToInt32(fila.Cells["tipo_usuario_id"].Value);
            u.Activo = Convert.ToBoolean(fila.Cells["activo"].Value);
            FrmUsuarioEditar editor = new FrmUsuarioEditar(u);
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

        private void FrmUsuarios_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
