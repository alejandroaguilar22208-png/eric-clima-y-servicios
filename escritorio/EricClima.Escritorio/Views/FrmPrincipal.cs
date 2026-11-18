using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    public class FrmPrincipal : Form
    {
        private readonly StockController stock = new StockController();
        private readonly TrabajoController trabajos = new TrabajoController();
        private readonly SolicitudController solicitudes = new SolicitudController();
        private readonly System.Windows.Forms.Timer actualizar = new System.Windows.Forms.Timer { Interval = 30000 };
        private readonly ListBox lstAlertas = new ListBox
        {
            Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, DrawMode = DrawMode.OwnerDrawFixed,
            ItemHeight = 38, IntegralHeight = false
        };
        private readonly Label lblSolicitudes = Numero();
        private Label lblHola;
        private readonly Label lblHoy = Numero();
        private readonly Label lblSinAgendar = Numero();
        private readonly Label lblStock = Numero();

        private static readonly Color Verde = Color.FromArgb(22, 163, 74);
        private static readonly Color RojoFuerte = Color.FromArgb(220, 38, 38);

        public FrmPrincipal()
        {
            Ui.Preparar(this, "Menú principal", 1000, 620, encabezado: false);
            StartPosition = FormStartPosition.CenterScreen;
            BackColor = Ui.Fondo;
            MinimumSize = new Size(900, 600);

            var menu = new FlowLayoutPanel
            {
                Dock = DockStyle.Left, Width = 220, BackColor = Ui.AzulOscuro,
                FlowDirection = FlowDirection.TopDown, WrapContents = false, Padding = new Padding(0, 12, 0, 12)
            };
            menu.Controls.Add(Logo());
            menu.Controls.Add(ItemMenu("", "Solicitudes web", () => Abrir(new FrmSolicitudes())));
            menu.Controls.Add(ItemMenu("", "Clientes", () => Abrir(new FrmClientes())));
            menu.Controls.Add(ItemMenu("", "Trabajos", () => Abrir(new FrmTrabajos())));
            menu.Controls.Add(ItemMenu("", "Stock", () => Abrir(new FrmMateriales())));
            menu.Controls.Add(ItemMenu("", "Presupuestos", () => Abrir(new FrmPresupuestos())));
            menu.Controls.Add(ItemMenu("", "Equipos en venta", () => Abrir(new FrmEquiposVenta())));
            menu.Controls.Add(new Panel { Width = 196, Height = 1, BackColor = Color.FromArgb(40, 70, 105), Margin = new Padding(12, 8, 12, 8) });
            menu.Controls.Add(ItemMenu("", "Mi cuenta", () => { Abrir(new FrmMiCuenta()); ActualizarSaludo(); }));
            menu.Controls.Add(ItemMenu("", "Ayuda (F1)", AbrirAyuda));
            menu.Controls.Add(ItemMenu("", "Salir", Close));

            var cabecera = new Panel { Dock = DockStyle.Top, Height = 84, BackColor = Color.White, Padding = new Padding(24, 14, 24, 0) };
            cabecera.Controls.Add(new Label
            {
                Text = $"{Sesion.UsuarioActual.Rol}  ·  " + DateTime.Today.ToString("dddd d 'de' MMMM 'de' yyyy"),
                Dock = DockStyle.Top, Height = 24, ForeColor = Ui.TextoGris
            });
            cabecera.Controls.Add(lblHola = new Label
            {
                Text = $"¡Hola, {Sesion.UsuarioActual.Nombre}!", Dock = DockStyle.Top, Height = 38,
                Font = new Font("Segoe UI", 17F, FontStyle.Bold), ForeColor = Ui.AzulOscuro
            });
            var lineaCabecera = new Panel { Dock = DockStyle.Top, Height = 4, BackColor = Ui.Naranja };

            var tarjetas = new TableLayoutPanel { Dock = DockStyle.Top, Height = 130, ColumnCount = 4, Padding = new Padding(18, 18, 18, 6) };
            for (int i = 0; i < 4; i++) tarjetas.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));
            tarjetas.Controls.Add(Tarjeta("", "Solicitudes", lblSolicitudes, Ui.Azul), 0, 0);
            tarjetas.Controls.Add(Tarjeta("", "Para hoy", lblHoy, Verde), 1, 0);
            tarjetas.Controls.Add(Tarjeta("", "Sin agendar", lblSinAgendar, Ui.Naranja), 2, 0);
            tarjetas.Controls.Add(Tarjeta("", "Stock bajo", lblStock, RojoFuerte), 3, 0);

            var cajaAlertas = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 12, 24, 24) };
            var tarjetaAlertas = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16, 8, 16, 12) };
            tarjetaAlertas.Controls.Add(lstAlertas);
            tarjetaAlertas.Controls.Add(new Label
            {
                Text = "Alertas del día", Dock = DockStyle.Top, Height = 40, ForeColor = Ui.AzulOscuro,
                Font = new Font("Segoe UI Semibold", 13F), TextAlign = ContentAlignment.MiddleLeft
            });
            cajaAlertas.Controls.Add(tarjetaAlertas);
            lstAlertas.DrawItem += DibujarAlerta;

            Controls.Add(cajaAlertas);
            Controls.Add(tarjetas);
            Controls.Add(lineaCabecera);
            Controls.Add(cabecera);
            Controls.Add(menu);

            Activated += (s, e) => CargarDatos();
            actualizar.Tick += (s, e) => CargarDatos();
            Load += (s, e) => actualizar.Start();
            FormClosed += (s, e) => actualizar.Dispose();
            KeyDown += (s, e) => { if (e.KeyCode == Keys.F1) AbrirAyuda(); };
        }

        private static Label Numero() => new Label
        {
            Text = "-", AutoSize = true, Font = new Font("Segoe UI", 24F, FontStyle.Bold), Margin = new Padding(0)
        };

        private Control Logo()
        {
            var panel = new Panel { Width = 220, Height = 86, Margin = new Padding(0, 0, 0, 16) };
            panel.Controls.Add(new Label
            {
                Text = "❄", Font = new Font("Segoe UI Symbol", 22F), ForeColor = Color.White, BackColor = Ui.Azul,
                Size = new Size(52, 52), Location = new Point(18, 16), TextAlign = ContentAlignment.MiddleCenter
            });
            panel.Controls.Add(new Label
            {
                Text = "Eric Clima", Font = new Font("Segoe UI", 15F, FontStyle.Bold), ForeColor = Color.White,
                AutoSize = true, Location = new Point(78, 16)
            });
            panel.Controls.Add(new Label
            {
                Text = "y Servicios", ForeColor = Color.FromArgb(159, 179, 204), AutoSize = true, Location = new Point(80, 46)
            });
            return panel;
        }

        private Control ItemMenu(string icono, string texto, Action accion)
        {
            var item = new Panel { Width = 220, Height = 44, Margin = new Padding(0, 2, 0, 2), Cursor = Cursors.Hand };
            var marca = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = Ui.AzulOscuro };
            var ico = new Label
            {
                Text = icono, Font = new Font("Segoe MDL2 Assets", 14F), ForeColor = Color.White,
                Location = new Point(24, 0), Size = new Size(32, 44), TextAlign = ContentAlignment.MiddleCenter
            };
            var lbl = new Label
            {
                Text = texto, Font = new Font("Segoe UI Semibold", 11F), ForeColor = Color.FromArgb(220, 230, 242),
                Location = new Point(62, 0), Size = new Size(150, 44), TextAlign = ContentAlignment.MiddleLeft
            };
            item.Controls.Add(ico);
            item.Controls.Add(lbl);
            item.Controls.Add(marca);

            void Resaltar(bool si)
            {
                item.BackColor = si ? Color.FromArgb(22, 55, 94) : Ui.AzulOscuro;
                marca.BackColor = si ? Ui.Naranja : Ui.AzulOscuro;
                lbl.ForeColor = si ? Color.White : Color.FromArgb(220, 230, 242);
            }
            foreach (Control c in new Control[] { item, ico, lbl, marca })
            {
                c.MouseEnter += (s, e) => Resaltar(true);
                c.MouseLeave += (s, e) => Resaltar(item.ClientRectangle.Contains(item.PointToClient(Cursor.Position)));
                c.Click += (s, e) => accion();
                c.Cursor = Cursors.Hand;
            }
            return item;
        }

        private static Control Tarjeta(string icono, string titulo, Label numero, Color color)
        {
            var tarjeta = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Margin = new Padding(6) };
            var borde = new Panel { Dock = DockStyle.Left, Width = 5, BackColor = color };
            var ico = new Label
            {
                Text = icono, Font = new Font("Segoe MDL2 Assets", 18F), ForeColor = color,
                BackColor = Color.FromArgb(30, color), Size = new Size(54, 54), Location = new Point(20, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            numero.ForeColor = color;
            numero.Location = new Point(84, 14);
            var lbl = new Label { Text = titulo, AutoSize = true, ForeColor = Ui.TextoGris, Location = new Point(88, 62) };
            tarjeta.Controls.Add(lbl);
            tarjeta.Controls.Add(numero);
            tarjeta.Controls.Add(ico);
            tarjeta.Controls.Add(borde);
            return tarjeta;
        }

        private void DibujarAlerta(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0) return;
            string texto = lstAlertas.Items[e.Index].ToString();
            Color color = texto.StartsWith("Stock") ? RojoFuerte
                        : texto.StartsWith("Hoy") ? Verde
                        : texto.StartsWith("Sin agendar") ? Ui.Naranja
                        : texto.StartsWith("Solicitud") ? Color.FromArgb(124, 58, 237)
                        : Ui.Azul;
            var fondo = e.Index % 2 == 0 ? Color.White : Color.FromArgb(248, 251, 254);
            using (var b = new SolidBrush(fondo)) e.Graphics.FillRectangle(b, e.Bounds);
            using (var b = new SolidBrush(color))
                e.Graphics.FillEllipse(b, e.Bounds.X + 6, e.Bounds.Y + e.Bounds.Height / 2 - 5, 10, 10);
            var area = new Rectangle(e.Bounds.X + 26, e.Bounds.Y, e.Bounds.Width - 30, e.Bounds.Height);
            TextRenderer.DrawText(e.Graphics, texto, Ui.Fuente, area, Ui.AzulOscuro,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.EndEllipsis);
        }

        private void ActualizarSaludo() => lblHola.Text = $"¡Hola, {Sesion.UsuarioActual.Nombre}!";

        private void Abrir(Form f)
        {
            using (f) f.ShowDialog(this);
            CargarDatos();
        }

        private void CargarDatos()
        {
            lstAlertas.Items.Clear();
            try
            {
                List<Material> bajos = stock.StockBajo();
                List<Trabajo> agendados = trabajos.Listar(EstadoTrabajo.Agendado);
                List<Trabajo> pendientes = trabajos.Listar(EstadoTrabajo.Pendiente);
                List<Trabajo> hoy = agendados.Where(t => t.FechaAgendada?.Date == DateTime.Today).ToList();

                List<Solicitud> nuevas = solicitudes.Listar().Where(x => x.Estado == EstadoSolicitud.Pendiente).ToList();
                lblSolicitudes.Text = nuevas.Count.ToString();
                lblHoy.Text = hoy.Count.ToString();
                lblSinAgendar.Text = pendientes.Count.ToString();
                lblStock.Text = bajos.Count.ToString();

                foreach (Solicitud x in nuevas)
                    lstAlertas.Items.Add($"Solicitud web: {x.Nombre} ({x.Telefono}) — {x.ServicioNombre}");

                foreach (Material m in bajos)
                    lstAlertas.Items.Add($"Stock bajo: {m.Nombre} — quedan {m.StockActual} {m.Unidad} (mínimo {m.StockMinimo})");

                foreach (Trabajo t in hoy)
                    lstAlertas.Items.Add($"Hoy {t.FechaAgendada:HH:mm}: {t.ServicioNombre} en {t.ClienteNombre}");

                foreach (Trabajo t in pendientes)
                    lstAlertas.Items.Add($"Sin agendar: {t.ServicioNombre} — {t.ClienteNombre}");

                if (lstAlertas.Items.Count == 0) lstAlertas.Items.Add("No hay alertas. Todo en orden.");
            }
            catch (Exception ex) { lstAlertas.Items.Add("No se pudo conectar a la base: " + ex.Message); }
        }

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
