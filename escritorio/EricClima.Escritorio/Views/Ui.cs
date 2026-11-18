using EricClima.Escritorio.Controllers;

namespace EricClima.Escritorio.Views
{
    public static class Ui
    {
        public static readonly Color Azul = Color.FromArgb(0, 92, 175);
        public static readonly Color AzulOscuro = Color.FromArgb(11, 35, 64);
        public static readonly Color AzulHover = Color.FromArgb(0, 63, 125);
        public static readonly Color AzulClaro = Color.FromArgb(234, 243, 252);
        public static readonly Color Naranja = Color.FromArgb(255, 122, 26);
        public static readonly Color NaranjaHover = Color.FromArgb(233, 101, 10);
        public static readonly Color Fondo = Color.FromArgb(242, 246, 251);
        public static readonly Color TextoGris = Color.FromArgb(100, 116, 139);
        public static readonly Color Rojo = Color.FromArgb(253, 226, 226);
        public static readonly Color Verde = Color.FromArgb(226, 245, 230);
        public static readonly Color Gris = Color.FromArgb(235, 235, 235);
        public static readonly Font Fuente = new Font("Segoe UI", 10F);
        private static readonly Icon Icono = CargarIcono();

        private static Icon CargarIcono()
        {
            try { return Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { return null; }
        }
        public static readonly Font FuenteNegrita = new Font("Segoe UI Semibold", 10F);

        public static void Preparar(Form f, string titulo, int ancho, int alto, bool encabezado = true)
        {
            f.Text = $"{titulo} - Eric Clima y Servicios";
            f.Font = Fuente;
            f.StartPosition = FormStartPosition.CenterParent;
            f.ClientSize = new Size(ancho, alto + (encabezado ? 58 : 0));
            f.BackColor = Color.White;
            f.KeyPreview = true;
            if (Icono != null) f.Icon = Icono;
            if (encabezado)
            {
                var cabecera = Encabezado(titulo);
                f.Load += (s, e) => { f.Controls.Add(cabecera); cabecera.SendToBack(); };
            }
        }

        public static Panel Encabezado(string titulo)
        {
            var panel = new Panel { Dock = DockStyle.Top, Height = 58, BackColor = AzulOscuro };
            var texto = new Label
            {
                Text = "❄  " + titulo, Dock = DockStyle.Fill, ForeColor = Color.White,
                Font = new Font("Segoe UI Semibold", 14F), TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(16, 0, 0, 0)
            };
            var linea = new Panel { Dock = DockStyle.Bottom, Height = 4, BackColor = Naranja };
            panel.Controls.Add(texto);
            panel.Controls.Add(linea);
            return panel;
        }

        public static Button Boton(string texto, EventHandler click, bool principal = false)
        {
            var b = new Button
            {
                Text = texto,
                AutoSize = true,
                MinimumSize = new Size(96, 36),
                Padding = new Padding(12, 4, 12, 4),
                Margin = new Padding(4),
                FlatStyle = FlatStyle.Flat,
                Font = FuenteNegrita,
                BackColor = principal ? Naranja : Color.White,
                ForeColor = principal ? Color.White : Azul,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = principal ? Naranja : Azul;
            b.FlatAppearance.BorderSize = principal ? 0 : 1;
            b.FlatAppearance.MouseOverBackColor = principal ? NaranjaHover : AzulClaro;
            b.FlatAppearance.MouseDownBackColor = principal ? NaranjaHover : Color.FromArgb(207, 224, 243);
            b.Click += click;
            return b;
        }

        public static DataGridView Grilla()
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                RowHeadersVisible = false
            };
            return Estilo(g);
        }

        public static DataGridView Estilo(DataGridView g)
        {
            g.BorderStyle = BorderStyle.None;
            g.BackgroundColor = Color.White;
            g.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            g.GridColor = Color.FromArgb(226, 232, 240);
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            g.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            g.ColumnHeadersHeight = 38;
            g.RowHeadersDefaultCellStyle.BackColor = Fondo;
            g.RowHeadersDefaultCellStyle.SelectionBackColor = AzulClaro;
            g.ColumnHeadersDefaultCellStyle.BackColor = Azul;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            g.ColumnHeadersDefaultCellStyle.Font = FuenteNegrita;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Azul;
            g.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            g.DefaultCellStyle.Padding = new Padding(6, 0, 0, 0);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(207, 224, 243);
            g.DefaultCellStyle.SelectionForeColor = AzulOscuro;
            g.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 251, 254);
            g.RowTemplate.Height = 32;
            return g;
        }

        public static FlowLayoutPanel Barra(DockStyle dock, params Control[] controles)
        {
            var p = new FlowLayoutPanel { Dock = dock, AutoSize = true, Padding = new Padding(10, 8, 10, 8), BackColor = dock == DockStyle.Bottom ? Fondo : AzulClaro };
            p.Controls.AddRange(controles);
            return p;
        }

        public static TableLayoutPanel Campos(params (string etiqueta, Control campo)[] filas)
        {
            var t = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, Padding = new Padding(16), AutoScroll = true };
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 140));
            t.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (var (etiqueta, campo) in filas)
            {
                campo.Dock = DockStyle.Fill;
                campo.Margin = new Padding(4, 6, 4, 6);
                t.Controls.Add(new Label { Text = etiqueta, AutoSize = true, Margin = new Padding(4, 10, 4, 4) });
                t.Controls.Add(campo);
            }
            return t;
        }

        public static NumericUpDown Numero(decimal valor = 0, int decimales = 2) => new NumericUpDown
        {
            Maximum = 100_000_000,
            DecimalPlaces = decimales,
            ThousandsSeparator = true,
            Value = valor
        };

        public static void Mostrar(Resultado r) =>
            MessageBox.Show(r.Mensaje, r.Ok ? "Listo" : "Atención", MessageBoxButtons.OK,
                r.Ok ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

        public static bool Confirmar(string pregunta) =>
            MessageBox.Show(pregunta, "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

        public static void Aviso(string texto) =>
            MessageBox.Show(texto, "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }
}
