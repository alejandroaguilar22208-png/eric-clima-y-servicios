using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmPresupuestos : Form
    {
        private ControlPresupuestos control = new ControlPresupuestos();
        private Presupuesto aImprimir;

        public FrmPresupuestos()
        {
            InitializeComponent();
            Estilos.Grilla(dgvDatos);
        }

        private void FrmPresupuestos_Load(object sender, EventArgs e)
        {
            Cargar();
        }

        private void Cargar()
        {
            try
            {
                dgvDatos.DataSource = control.listar();
                Estilos.OcultarColumnas(dgvDatos, "id");
                Estilos.FormatoPlata(dgvDatos, "Total");
                foreach (DataGridViewRow fila in dgvDatos.Rows)
                {
                    string estado = fila.Cells["Estado"].Value.ToString();
                    if (estado == "aceptado") fila.DefaultCellStyle.BackColor = Estilos.Verde;
                    else if (estado == "vencido") fila.DefaultCellStyle.BackColor = Estilos.Gris;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Presupuestos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private int PresupuestoSeleccionado()
        {
            int id = Estilos.IdSeleccionado(dgvDatos, "id");
            if (id == 0) MessageBox.Show("Seleccioná un presupuesto.", "Presupuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return id;
        }

        private void btnNuevo_Click(object sender, EventArgs e)
        {
            FrmPresupuestoNuevo nuevo = new FrmPresupuestoNuevo();
            if (nuevo.ShowDialog(this) == DialogResult.OK)
            {
                Cargar();
                DialogResult resp = MessageBox.Show("Presupuesto guardado. ¿Querés imprimirlo ahora?", "Presupuestos",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (resp == DialogResult.Yes) Imprimir(nuevo.IdGuardado);
            }
        }

        private void btnImprimir_Click(object sender, EventArgs e)
        {
            int id = PresupuestoSeleccionado();
            if (id != 0) Imprimir(id);
        }

        private void dgvDatos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) btnImprimir_Click(sender, e);
        }

        private void Imprimir(int id)
        {
            try
            {
                aImprimir = control.obtener(id);
                PrintDocument documento = new PrintDocument();
                documento.DocumentName = "Presupuesto " + id.ToString("00000");
                documento.PrintPage += documento_PrintPage;
                PrintPreviewDialog vista = new PrintPreviewDialog();
                vista.Document = documento;
                vista.Text = "Vista previa - Presupuesto N° " + id.ToString("00000");
                vista.WindowState = FormWindowState.Maximized;
                vista.ShowIcon = false;
                vista.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Presupuestos", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void documento_PrintPage(object sender, PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font titulo = new Font("Segoe UI", 18, FontStyle.Bold);
            Font negrita = new Font("Segoe UI", 10, FontStyle.Bold);
            Font normal = new Font("Segoe UI", 10);
            Brush azul = new SolidBrush(Estilos.Azul);
            int x = e.MarginBounds.Left;
            int ancho = e.MarginBounds.Width;
            int y = e.MarginBounds.Top;

            g.DrawString("Eric Clima y Servicios", titulo, azul, x, y);
            g.DrawString("Presupuesto N° " + aImprimir.Id.ToString("00000"), negrita, Brushes.Black, x + ancho - 200, y + 8);
            y += 40;
            g.DrawString("Instalación y mantenimiento de aire acondicionado - Bahía Blanca", normal, Brushes.Gray, x, y);
            y += 30;
            g.FillRectangle(azul, x, y, ancho, 3);
            y += 15;

            g.DrawString("Cliente: " + aImprimir.NombreCliente, negrita, Brushes.Black, x, y);
            g.DrawString("Fecha: " + aImprimir.Fecha.ToString("dd/MM/yyyy"), normal, Brushes.Black, x + ancho - 200, y);
            y += 22;
            g.DrawString("Dirección: " + aImprimir.DireccionCliente, normal, Brushes.Black, x, y);
            g.DrawString("Válido hasta: " + aImprimir.Vence.ToString("dd/MM/yyyy"), normal, Brushes.Black, x + ancho - 200, y);
            y += 22;
            g.DrawString("Teléfono: " + aImprimir.TelefonoCliente, normal, Brushes.Black, x, y);
            y += 35;

            g.FillRectangle(azul, x, y, ancho, 26);
            g.DrawString("Descripción", negrita, Brushes.White, x + 6, y + 4);
            g.DrawString("Cant.", negrita, Brushes.White, x + ancho - 300, y + 4);
            g.DrawString("Precio", negrita, Brushes.White, x + ancho - 220, y + 4);
            g.DrawString("Subtotal", negrita, Brushes.White, x + ancho - 110, y + 4);
            y += 30;

            foreach (ItemPresupuesto item in aImprimir.Items)
            {
                g.DrawString(item.Descripcion, normal, Brushes.Black, new RectangleF(x + 6, y, ancho - 320, 22));
                g.DrawString(item.Cantidad.ToString("0.##"), normal, Brushes.Black, x + ancho - 300, y);
                g.DrawString(item.PrecioUnitario.ToString("$ #,##0"), normal, Brushes.Black, x + ancho - 220, y);
                g.DrawString(item.Subtotal().ToString("$ #,##0"), normal, Brushes.Black, x + ancho - 110, y);
                y += 24;
                g.DrawLine(Pens.LightGray, x, y - 2, x + ancho, y - 2);
            }

            y += 10;
            g.DrawString("TOTAL: " + aImprimir.Total().ToString("$ #,##0"), titulo, azul, x + ancho - 260, y);
            y += 60;
            g.DrawString("Los precios pueden cambiar después de la fecha de vencimiento.", normal, Brushes.Gray, x, y);

            titulo.Dispose();
            negrita.Dispose();
            normal.Dispose();
            azul.Dispose();
        }

        private void btnAceptado_Click(object sender, EventArgs e)
        {
            int id = PresupuestoSeleccionado();
            if (id == 0) return;
            string error = control.aceptar(id);
            if (error != "")
                MessageBox.Show(error, "Presupuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnRechazado_Click(object sender, EventArgs e)
        {
            int id = PresupuestoSeleccionado();
            if (id == 0) return;
            string cliente = Estilos.TextoSeleccionado(dgvDatos, "Cliente");
            DialogResult resp = MessageBox.Show("¿Está seguro de rechazar el presupuesto de " + cliente + "? Se borra definitivamente.",
                "Presupuestos", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (resp != DialogResult.Yes) return;
            string error = control.rechazar(id);
            if (error != "")
                MessageBox.Show(error, "Presupuestos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                Cargar();
        }

        private void btnCerrar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmPresupuestos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape) this.Close();
        }
    }
}
