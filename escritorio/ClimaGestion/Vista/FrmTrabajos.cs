using System;
using System.Data;
using System.Windows.Forms;
using ClimaGestion.Control;

namespace ClimaGestion.Vista
{
    public partial class FrmTrabajos : Form
    {
        private ControlTrabajos control = new ControlTrabajos();

        public FrmTrabajos()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmTrabajos_Load(object sender, EventArgs e)
        {
            cboEstado.Items.AddRange(new object[] { "Todos", "pendiente", "agendado", "realizado", "cancelado" });
            cboEstado.SelectedIndex = 0;
        }

        private void Cargar()
        {
            try
            {
                string estado = cboEstado.SelectedIndex <= 0 ? "" : cboEstado.Text;
                dgvDatos.DataSource = control.listar(estado);
                Estilos.FormatoPlata(dgvDatos, "Total");
                dgvDatos.Columns["N"].FillWeight = 30;
                foreach (DataGridViewRow fila in dgvDatos.Rows)
                {
                    string estadoFila = fila.Cells["Estado"].Value.ToString();
                    if (estadoFila == "realizado") fila.DefaultCellStyle.BackColor = Estilos.Verde;
                    else if (estadoFila == "cancelado") fila.DefaultCellStyle.BackColor = Estilos.Gris;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Trabajos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cboEstado_SelectedIndexChanged(object sender, EventArgs e)
        {
            Cargar();
        }

        private int TrabajoSeleccionado()
        {
            int id = Estilos.IdSeleccionado(dgvDatos, "N");
            if (id == 0) MessageBox.Show("Seleccioná un trabajo.", "Trabajos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return id;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmTrabajoNuevo nuevo = new FrmTrabajoNuevo();
            if (nuevo.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void btnAgendar_Click(object sender, EventArgs e)
        {
            int id = TrabajoSeleccionado();
            if (id == 0) return;
            FrmAgendar agendar = new FrmAgendar(id);
            if (agendar.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void btnMaterial_Click(object sender, EventArgs e)
        {
            int id = TrabajoSeleccionado();
            if (id == 0) return;
            FrmAgregarMaterial agregar = new FrmAgregarMaterial(id);
            agregar.ShowDialog(this);
        }

        private void btnVerMateriales_Click(object sender, EventArgs e)
        {
            int id = TrabajoSeleccionado();
            if (id == 0) return;
            try
            {
                DataTable materiales = control.materialesDe(id);
                string texto = "";
                foreach (DataRow fila in materiales.Rows)
                    texto += "• " + fila["nombre"] + ": " + Convert.ToDecimal(fila["cantidad"]).ToString("0.##") + " " + fila["unidad"] + "\n";
                if (texto == "") texto = "Este trabajo todavía no tiene materiales cargados.";
                MessageBox.Show(texto, "Materiales del trabajo N° " + id, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Trabajos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRealizado_Click(object sender, EventArgs e)
        {
            int id = TrabajoSeleccionado();
            if (id == 0) return;
            DialogResult resp = MessageBox.Show("¿Marcar el trabajo N° " + id + " como realizado? Se descuenta el stock y empieza la garantía.",
                "Trabajos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;
            string error = control.marcarRealizado(id);
            if (error != "")
                MessageBox.Show(error, "Trabajos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                MessageBox.Show("Trabajo realizado. Garantía de 12 meses.", "Trabajos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cargar();
            }
        }

        private void btnCancelarTrabajo_Click(object sender, EventArgs e)
        {
            int id = TrabajoSeleccionado();
            if (id == 0) return;
            DialogResult resp = MessageBox.Show("¿Cancelar el trabajo N° " + id + "?", "Trabajos", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;
            string error = control.cancelar(id);
            if (error != "")
                MessageBox.Show(error, "Trabajos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmTrabajos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
