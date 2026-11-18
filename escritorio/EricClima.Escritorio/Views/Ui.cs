using EricClima.Escritorio.Controllers;

namespace EricClima.Escritorio.Views
{
    public static class Ui
    {
        public static readonly Color Azul = Color.FromArgb(0, 92, 175);
        public static readonly Color AzulClaro = Color.FromArgb(232, 241, 251);
        public static readonly Color Rojo = Color.FromArgb(253, 226, 226);
        public static readonly Color Verde = Color.FromArgb(226, 245, 230);
        public static readonly Color Gris = Color.FromArgb(235, 235, 235);
        public static readonly Font Fuente = new Font("Segoe UI", 10F);

        public static void Preparar(Form f, string titulo, int ancho, int alto)
        {
            f.Text = $"{titulo} - Eric Clima y Servicios";
            f.Font = Fuente;
            f.StartPosition = FormStartPosition.CenterParent;
            f.ClientSize = new Size(ancho, alto);
            f.BackColor = Color.White;
            f.KeyPreview = true;
        }

        public static Button Boton(string texto, EventHandler click, bool principal = false)
        {
            var b = new Button
            {
                Text = texto,
                AutoSize = true,
                Padding = new Padding(10, 4, 10, 4),
                Margin = new Padding(4),
                FlatStyle = FlatStyle.Flat,
                BackColor = principal ? Azul : Color.White,
                ForeColor = principal ? Color.White : Azul,
                Cursor = Cursors.Hand
            };
            b.FlatAppearance.BorderColor = Azul;
            b.Click += click;
            return b;
        }

        public static DataGridView Grilla() => new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            RowHeadersVisible = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None
        };

        public static FlowLayoutPanel Barra(DockStyle dock, params Control[] controles)
        {
            var p = new FlowLayoutPanel { Dock = dock, AutoSize = true, Padding = new Padding(8), BackColor = AzulClaro };
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
