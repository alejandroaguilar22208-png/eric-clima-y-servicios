using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    /// <summary>Ventana principal: menú lateral y panel de alertas.</summary>
    public class FrmPrincipal : Form
    {
        private readonly StockController stock = new StockController();
        private readonly TrabajoController trabajos = new TrabajoController();
        private readonly ListBox lstAlertas = new ListBox { Dock = DockStyle.Fill, BorderStyle = BorderStyle.None };

        public FrmPrincipal()
        {
            Ui.Preparar(this, "Menú principal", 900, 560);
            StartPosition = FormStartPosition.CenterScreen;

            // ---- Menú lateral ----
            var menu = new FlowLayoutPanel
            {
                Dock = DockStyle.Left, Width = 200, BackColor = Ui.Azul,
                FlowDirection = FlowDirection.TopDown, Padding = new Padding(12)
            };
            menu.Controls.Add(new Label
            {
                Text = "Eric Clima\ny Servicios", ForeColor = Color.White, AutoSize = true,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold), Margin = new Padding(4, 4, 4, 24)
            });
            menu.Controls.Add(BotonMenu("Clientes", (s, e) => Abrir(new FrmClientes())));
            menu.Controls.Add(BotonMenu("Trabajos", (s, e) => Abrir(new FrmTrabajos())));
            menu.Controls.Add(BotonMenu("Stock", (s, e) => Abrir(new FrmMateriales())));
            menu.Controls.Add(BotonMenu("Ayuda (F1)", (s, e) => AbrirAyuda()));
            menu.Controls.Add(BotonMenu("Salir", (s, e) => Close()));

            // ---- Panel central ----
            var bienvenida = new Label
            {
                Text = $"Hola, {Sesion.UsuarioActual.Nombre} ({Sesion.UsuarioActual.Rol})",
                Dock = DockStyle.Top, Height = 50, Padding = new Padding(16, 14, 0, 0),
                Font = new Font("Segoe UI", 13F, FontStyle.Bold)
            };
            var tituloAlertas = new Label
            {
                Text = "Alertas", Dock = DockStyle.Top, Height = 30, Padding = new Padding(16, 6, 0, 0),
                Font = new Font("Segoe UI", 11F, FontStyle.Bold), ForeColor = Ui.Azul
            };
            var contenedor = new Panel { Dock = DockStyle.Fill, Padding = new Padding(16) };
            contenedor.Controls.Add(lstAlertas);

            Controls.Add(contenedor);
            Controls.Add(tituloAlertas);
            Controls.Add(bienvenida);
            Controls.Add(menu);

            Activated += (s, e) => CargarAlertas(); // se actualiza cada vez que volvés al menú
            KeyDown += (s, e) => { if (e.KeyCode == Keys.F1) AbrirAyuda(); };
        }

        private Button BotonMenu(string texto, EventHandler click)
        {
            var b = new Button
            {
                Text = texto, Width = 170, Height = 42, FlatStyle = FlatStyle.Flat,
                ForeColor = Color.White, TextAlign = ContentAlignment.MiddleLeft, Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderSize = 0;
            b.FlatAppearance.MouseOverBackColor = Color.FromArgb(0, 70, 140);
            b.Click += click;
            return b;
        }

        private void Abrir(Form f)
        {
            using (f) f.ShowDialog(this);
            CargarAlertas();
        }

        private void CargarAlertas()
        {
            lstAlertas.Items.Clear();
            try
            {
                foreach (Material m in stock.StockBajo())
                    lstAlertas.Items.Add($"Stock bajo: {m.Nombre} — quedan {m.StockActual} {m.Unidad} (mínimo {m.StockMinimo})");

                foreach (Trabajo t in trabajos.Listar(EstadoTrabajo.Agendado))
                    if (t.FechaAgendada?.Date == DateTime.Today)
                        lstAlertas.Items.Add($"Hoy {t.FechaAgendada:HH:mm}: {t.ServicioNombre} en {t.ClienteNombre}");

                foreach (Trabajo t in trabajos.Listar(EstadoTrabajo.Pendiente))
                    lstAlertas.Items.Add($"Sin agendar: {t.ServicioNombre} — {t.ClienteNombre}");

                if (lstAlertas.Items.Count == 0) lstAlertas.Items.Add("No hay alertas. Todo en orden.");
            }
            catch (Exception ex) { lstAlertas.Items.Add("No se pudo conectar a la base: " + ex.Message); }
        }

        /// <summary>El manual se abre desde el sistema (lo pide la consigna).</summary>
        private void AbrirAyuda()
        {
            string ruta = Path.Combine(AppContext.BaseDirectory, "Manual.pdf");
            if (File.Exists(ruta))
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(ruta) { UseShellExecute = true });
            else
                Ui.Aviso("Todavía no está el Manual.pdf al lado del programa.");
        }
    }
}
