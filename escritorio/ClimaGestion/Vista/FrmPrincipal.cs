using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using ClimaGestion.Control;

namespace ClimaGestion.Vista
{
    public partial class FrmPrincipal : Form
    {
        private FrmLogin login;
        private bool saliendo = false;

        public FrmPrincipal(FrmLogin login)
        {
            InitializeComponent();
            this.login = login;
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            lblHola.Text = "¡Hola, " + Sesion.UsuarioActual.Nombre + "!";
            lblFecha.Text = Sesion.UsuarioActual.NombreTipo + "  ·  " + DateTime.Today.ToString("dddd d 'de' MMMM 'de' yyyy");
            mnuAdministracion.Visible = Sesion.EsAdministrador();
            btnUsuarios.Visible = Sesion.EsAdministrador();
            timerActualizar.Start();
        }

        private void FrmPrincipal_Activated(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void timerActualizar_Tick(object sender, EventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            lstAlertas.Items.Clear();
            try
            {
                DataTable solicitudes = new ControlSolicitudes().pendientes();
                DataTable hoy = new ControlTrabajos().agendadosHoy();
                DataTable sinAgendar = new ControlTrabajos().pendientes();
                DataTable stockBajo = new ControlStock().stockBajo();

                lblNumSolicitudes.Text = solicitudes.Rows.Count.ToString();
                lblNumParaHoy.Text = hoy.Rows.Count.ToString();
                lblNumSinAgendar.Text = sinAgendar.Rows.Count.ToString();
                lblNumStockBajo.Text = stockBajo.Rows.Count.ToString();

                foreach (DataRow fila in solicitudes.Rows)
                    lstAlertas.Items.Add("Solicitud web: " + fila["nombre"] + " (" + fila["telefono"] + ") — " + fila["servicio"]);
                foreach (DataRow fila in hoy.Rows)
                    lstAlertas.Items.Add("Hoy " + ((DateTime)fila["fecha_agendada"]).ToString("HH:mm") + ": " + fila["servicio"] + " en " + fila["cliente"]);
                foreach (DataRow fila in sinAgendar.Rows)
                    lstAlertas.Items.Add("Sin agendar: " + fila["servicio"] + " — " + fila["cliente"]);
                foreach (DataRow fila in stockBajo.Rows)
                    lstAlertas.Items.Add("Stock bajo: " + fila["nombre"] + " — quedan " + fila["stock_actual"] + " " + fila["unidad"]);

                if (lstAlertas.Items.Count == 0) lstAlertas.Items.Add("No hay alertas. Todo en orden.");
            }
            catch (Exception ex)
            {
                lstAlertas.Items.Add(Errores.Mensaje(ex));
            }
        }

        private void lstAlertas_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            string texto = lstAlertas.Items[e.Index].ToString();
            Color punto = Estilos.Azul;
            if (texto.StartsWith("Solicitud")) punto = Color.FromArgb(124, 58, 237);
            else if (texto.StartsWith("Hoy")) punto = Color.FromArgb(22, 163, 74);
            else if (texto.StartsWith("Sin agendar")) punto = Color.FromArgb(255, 122, 26);
            else if (texto.StartsWith("Stock")) punto = Color.FromArgb(220, 38, 38);

            Color fondo = e.Index % 2 == 0 ? Color.White : Color.FromArgb(248, 251, 254);
            using (SolidBrush pincel = new SolidBrush(fondo)) e.Graphics.FillRectangle(pincel, e.Bounds);
            using (SolidBrush pincel = new SolidBrush(punto)) e.Graphics.FillEllipse(pincel, e.Bounds.X + 10, e.Bounds.Y + e.Bounds.Height / 2 - 5, 10, 10);
            Rectangle area = new Rectangle(e.Bounds.X + 30, e.Bounds.Y, e.Bounds.Width - 34, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, texto, lstAlertas.Font, area, Estilos.AzulOscuro,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        private void Abrir(Form formulario)
        {
            formulario.ShowDialog(this);
            formulario.Dispose();
            CargarDatos();
        }

        private void btnSolicitudes_Click(object sender, EventArgs e) { Abrir(new FrmSolicitudes()); }
        private void btnClientes_Click(object sender, EventArgs e) { Abrir(new FrmClientes()); }
        private void btnTrabajos_Click(object sender, EventArgs e) { Abrir(new FrmTrabajos()); }
        private void btnStock_Click(object sender, EventArgs e) { Abrir(new FrmMateriales()); }
        private void btnPresupuestos_Click(object sender, EventArgs e) { Abrir(new FrmPresupuestos()); }
        private void btnEquipos_Click(object sender, EventArgs e) { Abrir(new FrmEquiposVenta()); }
        private void btnUsuarios_Click(object sender, EventArgs e) { Abrir(new FrmUsuarios()); }
        private void btnMiCuenta_Click(object sender, EventArgs e) { Abrir(new FrmMiCuenta()); lblHola.Text = "¡Hola, " + Sesion.UsuarioActual.Nombre + "!"; }

        private void mnuSolicitudes_Click(object sender, EventArgs e) { btnSolicitudes_Click(sender, e); }
        private void mnuClientes_Click(object sender, EventArgs e) { btnClientes_Click(sender, e); }
        private void mnuTrabajos_Click(object sender, EventArgs e) { btnTrabajos_Click(sender, e); }
        private void mnuStock_Click(object sender, EventArgs e) { btnStock_Click(sender, e); }
        private void mnuPresupuestos_Click(object sender, EventArgs e) { btnPresupuestos_Click(sender, e); }
        private void mnuEquipos_Click(object sender, EventArgs e) { btnEquipos_Click(sender, e); }
        private void mnuUsuarios_Click(object sender, EventArgs e) { btnUsuarios_Click(sender, e); }
        private void mnuMiCuenta_Click(object sender, EventArgs e) { btnMiCuenta_Click(sender, e); }
        private void mnuManual_Click(object sender, EventArgs e) { btnAyuda_Click(sender, e); }
        private void mnuSalir_Click(object sender, EventArgs e) { btnSalir_Click(sender, e); }

        private void btnAyuda_Click(object sender, EventArgs e)
        {
            string ruta = Path.Combine(Application.StartupPath, "Manual.pdf");
            if (File.Exists(ruta))
                Process.Start(ruta);
            else
                MessageBox.Show("No se encontró el archivo Manual.pdf al lado del programa.", "Ayuda", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void mnuCerrarSesion_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show("Está a punto de cerrar su sesión, ¿confirma?", "ClimaGestión",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                saliendo = true;
                Sesion.UsuarioActual = null;
                this.Close();
                login.Show();
            }
        }

        private void btnSalir_Click(object sender, EventArgs e)
        {
            DialogResult resp = MessageBox.Show("Cerrar el sistema, ¿confirma?", "ClimaGestión",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                saliendo = true;
                Application.Exit();
            }
        }

        private void FrmPrincipal_FormClosing(object sender, FormClosingEventArgs e)
        {
            timerActualizar.Stop();
            if (saliendo || e.CloseReason != CloseReason.UserClosing) return;
            DialogResult resp = MessageBox.Show("Cerrar el sistema, ¿confirma?", "ClimaGestión",
                MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (resp == DialogResult.Yes)
            {
                saliendo = true;
                Application.Exit();
            }
            else
            {
                e.Cancel = true;
                timerActualizar.Start();
            }
        }
    }
}
