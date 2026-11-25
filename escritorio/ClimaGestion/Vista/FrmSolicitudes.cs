using System;
using System.Windows.Forms;
using ClimaGestion.Control;

namespace ClimaGestion.Vista
{
    public partial class FrmSolicitudes : Form
    {
        private ControlSolicitudes control = new ControlSolicitudes();

        public FrmSolicitudes()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmSolicitudes_Load(object sender, EventArgs e)
        {
            Cargar();
            timerActualizar.Start();
        }

        private void Cargar()
        {
            try
            {
                int seleccion = Estilos.IdSeleccionado(dgvDatos, "id");
                dgvDatos.DataSource = control.listar();
                Estilos.OcultarColumnas(dgvDatos, "id", "mensaje");
                foreach (DataGridViewRow fila in dgvDatos.Rows)
                {
                    string estado = fila.Cells["Estado"].Value.ToString();
                    if (estado == "pendiente") fila.DefaultCellStyle.BackColor = Estilos.AzulClaro;
                    else if (estado == "convertida") fila.DefaultCellStyle.BackColor = Estilos.Verde;
                    if (Convert.ToInt32(fila.Cells["id"].Value) == seleccion)
                        dgvDatos.CurrentCell = fila.Cells["Nombre"];
                }
            }
            catch (Exception ex)
            {
                timerActualizar.Stop();
                MessageBox.Show(Errores.Mensaje(ex), "Solicitudes", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dgvDatos_SelectionChanged(object sender, EventArgs e)
        {
            if (dgvDatos.CurrentRow == null || !dgvDatos.Columns.Contains("mensaje")) { txtMensaje.Text = ""; return; }
            object mensaje = dgvDatos.CurrentRow.Cells["mensaje"].Value;
            txtMensaje.Text = mensaje == DBNull.Value || mensaje.ToString() == "" ? "(sin mensaje)" : mensaje.ToString();
        }

        private int SolicitudSeleccionada()
        {
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0) MessageBox.Show("Seleccioná una solicitud.", "Solicitudes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return id;
        }

        private void btnConvertir_Click(object sender, EventArgs e)
        {
            int id = SolicitudSeleccionada();
            if (id == 0) return;
            string nombre = Estilos.TextoSeleccionado(dgvDatos, "Nombre");
            DialogResult resp = MessageBox.Show("¿Convertir la solicitud de " + nombre + " en cliente y trabajo pendiente?", "Solicitudes",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (resp != DialogResult.Yes) return;
            string error = control.convertir(id);
            if (error != "")
                MessageBox.Show(error, "Solicitudes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
            {
                MessageBox.Show("Listo. Ahora el trabajo aparece en Trabajos para agendarlo.", "Solicitudes", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Cargar();
            }
        }

        private void btnContactado_Click(object sender, EventArgs e)
        {
            int id = SolicitudSeleccionada();
            if (id == 0) return;
            string error = control.marcarContactado(id);
            if (error != "")
                MessageBox.Show(error, "Solicitudes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnDescartar_Click(object sender, EventArgs e)
        {
            int id = SolicitudSeleccionada();
            if (id == 0) return;
            string nombre = Estilos.TextoSeleccionado(dgvDatos, "Nombre");
            DialogResult resp = MessageBox.Show("¿Está seguro de descartar la solicitud de " + nombre + "? Se borra definitivamente.", "Solicitudes",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp != DialogResult.Yes) return;
            string error = control.descartar(id);
            if (error != "")
                MessageBox.Show(error, "Solicitudes", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnActualizar_Click(object sender, EventArgs e)
        {
            Cargar();
        }

        private void timerActualizar_Tick(object sender, EventArgs e)
        {
            Cargar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmSolicitudes_FormClosed(object sender, FormClosedEventArgs e)
        {
            timerActualizar.Stop();
        }

        private void FrmSolicitudes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
