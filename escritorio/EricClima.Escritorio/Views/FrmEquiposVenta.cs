using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    public class FrmEquiposVenta : Form
    {
        private readonly EquipoVentaController controller = new EquipoVentaController();
        private readonly DataGridView grilla = Ui.Grilla();
        private List<EquipoVenta> equipos = new List<EquipoVenta>();

        public FrmEquiposVenta()
        {
            Ui.Preparar(this, "Equipos en venta", 900, 460);
            grilla.CellDoubleClick += (s, e) => Editar(s, e);
            grilla.DataBindingComplete += (s, e) => Pintar();

            Controls.Add(grilla);
            Controls.Add(Ui.Barra(DockStyle.Top, new Label
            {
                Text = "Estos equipos son los que se muestran en la página web. Lo que cambies acá se ve al instante en la web.",
                AutoSize = true, Margin = new Padding(4, 6, 4, 4)
            }));
            Controls.Add(Ui.Barra(DockStyle.Bottom,
                Ui.Boton("Nuevo", Nuevo, principal: true),
                Ui.Boton("Editar", Editar),
                Ui.Boton("Ocultar / mostrar", CambiarVisibilidad),
                Ui.Boton("Cerrar", (s, e) => Close()),
                new Label { Text = "Filas grises = ocultos en la web", AutoSize = true, Margin = new Padding(16, 10, 4, 4) }));
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                equipos = controller.Listar();
                grilla.DataSource = equipos.Select(e => new
                {
                    e.Marca,
                    e.Modelo,
                    Tipo = EquipoVenta.TipoTexto(e.Tipo),
                    Frigorías = e.Frigorias.ToString("N0"),
                    Características = (e.FrioCalor ? "Frío/calor" : "Solo frío") + (e.Inverter ? " · Inverter" : ""),
                    Precio = e.Precio.ToString("C0"),
                    Stock = e.Disponible() ? e.Stock.ToString() : "A pedido",
                    Web = e.Activo ? "Visible" : "Oculto"
                }).ToList();
            }
            catch (Exception ex) { Ui.Aviso("No se pudieron cargar los equipos: " + ex.Message); }
        }

        private void Pintar()
        {
            for (int i = 0; i < grilla.Rows.Count; i++)
                if (!equipos[i].Activo)
                {
                    grilla.Rows[i].DefaultCellStyle.BackColor = Ui.Gris;
                    grilla.Rows[i].DefaultCellStyle.ForeColor = Ui.TextoGris;
                }
        }

        private EquipoVenta Seleccionado() =>
            grilla.CurrentRow == null ? null : equipos[grilla.CurrentRow.Index];

        private void Nuevo(object sender, EventArgs e) => AbrirEditor(new EquipoVenta());

        private void Editar(object sender, EventArgs e)
        {
            EquipoVenta eq = Seleccionado();
            if (eq == null) { Ui.Aviso("Seleccioná un equipo."); return; }
            AbrirEditor(eq);
        }

        private void CambiarVisibilidad(object sender, EventArgs e)
        {
            EquipoVenta eq = Seleccionado();
            if (eq == null) { Ui.Aviso("Seleccioná un equipo."); return; }
            Resultado r = controller.CambiarVisibilidad(eq);
            Ui.Mostrar(r);
            Cargar();
        }

        private void AbrirEditor(EquipoVenta eq)
        {
            using var f = new FrmEquipoVentaEditar(eq);
            if (f.ShowDialog(this) == DialogResult.OK) Cargar();
        }
    }

    public class FrmEquipoVentaEditar : Form
    {
        private readonly EquipoVentaController controller = new EquipoVentaController();
        private readonly EquipoVenta equipo;
        private readonly TextBox txtMarca = new TextBox { MaxLength = 50 };
        private readonly TextBox txtModelo = new TextBox { MaxLength = 80 };
        private readonly ComboBox cboTipo = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown numFrigorias;
        private readonly NumericUpDown numPrecio;
        private readonly NumericUpDown numStock;
        private readonly CheckBox chkFrioCalor = new CheckBox { Text = "Frío / calor", AutoSize = true };
        private readonly CheckBox chkInverter = new CheckBox { Text = "Inverter", AutoSize = true };
        private readonly TextBox txtDescripcion = new TextBox { MaxLength = 255 };
        private readonly ErrorProvider errores = new ErrorProvider { BlinkStyle = ErrorBlinkStyle.NeverBlink };

        public FrmEquipoVentaEditar(EquipoVenta e)
        {
            equipo = e;
            Ui.Preparar(this, e.Id == 0 ? "Nuevo equipo en venta" : "Editar equipo en venta", 480, 420);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            txtMarca.Text = e.Marca;
            txtModelo.Text = e.Modelo;
            foreach (TipoEquipoVenta t in Enum.GetValues<TipoEquipoVenta>()) cboTipo.Items.Add(EquipoVenta.TipoTexto(t));
            cboTipo.SelectedIndex = (int)e.Tipo;
            numFrigorias = Ui.Numero(e.Frigorias, 0);
            numFrigorias.Increment = 50;
            numPrecio = Ui.Numero(e.Precio);
            numStock = Ui.Numero(e.Stock, 0);
            chkFrioCalor.Checked = e.FrioCalor;
            chkInverter.Checked = e.Inverter;
            txtDescripcion.Text = e.Descripcion;

            var opciones = new FlowLayoutPanel { AutoSize = true, Margin = new Padding(0) };
            opciones.Controls.Add(chkFrioCalor);
            opciones.Controls.Add(chkInverter);

            var btnGuardar = Ui.Boton("Guardar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, ev) => Close());
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            Controls.Add(Ui.Campos(("Marca *", txtMarca), ("Modelo *", txtModelo), ("Tipo", cboTipo),
                ("Frigorías *", numFrigorias), ("Precio ($) *", numPrecio), ("Stock", numStock),
                ("Características", opciones), ("Descripción", txtDescripcion)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            equipo.Marca = txtMarca.Text;
            equipo.Modelo = txtModelo.Text;
            equipo.Tipo = (TipoEquipoVenta)cboTipo.SelectedIndex;
            equipo.Frigorias = (int)numFrigorias.Value;
            equipo.Precio = numPrecio.Value;
            equipo.Stock = (int)numStock.Value;
            equipo.FrioCalor = chkFrioCalor.Checked;
            equipo.Inverter = chkInverter.Checked;
            equipo.Descripcion = txtDescripcion.Text;

            errores.Clear();
            string error = equipo.Validar();
            if (error.Contains("marca")) errores.SetError(txtMarca, error);
            else if (error.Contains("modelo")) errores.SetError(txtModelo, error);
            else if (error.Contains("frigorías")) errores.SetError(numFrigorias, error);
            else if (error.Contains("precio")) errores.SetError(numPrecio, error);

            Resultado r = controller.Guardar(equipo);
            if (!r.Ok) { Ui.Mostrar(r); return; }
            DialogResult = DialogResult.OK;
        }
    }
}
