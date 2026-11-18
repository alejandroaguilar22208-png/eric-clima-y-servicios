using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    public class FrmMateriales : Form
    {
        private readonly StockController controller = new StockController();
        private readonly DataGridView grilla = Ui.Grilla();
        private List<Material> materiales = new List<Material>();

        public FrmMateriales()
        {
            Ui.Preparar(this, "Stock de materiales", 760, 440);
            grilla.CellDoubleClick += (s, e) => Editar(s, e);
            grilla.DataBindingComplete += (s, e) => Pintar();

            Controls.Add(grilla);
            Controls.Add(Ui.Barra(DockStyle.Bottom,
                Ui.Boton("Nuevo", Nuevo, principal: true),
                Ui.Boton("Editar", Editar),
                Ui.Boton("Cerrar", (s, e) => Close()),
                new Label { Text = "Filas en rojo = stock bajo", AutoSize = true, Margin = new Padding(16, 10, 4, 4) }));
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                materiales = controller.Listar();
                grilla.DataSource = materiales.Select(m => new
                {
                    m.Nombre, m.Unidad, Stock = m.StockActual, Mínimo = m.StockMinimo, Precio = m.Precio.ToString("C0")
                }).ToList();
            }
            catch (Exception ex) { Ui.Aviso("No se pudo cargar el stock: " + ex.Message); }
        }

        private void Pintar()
        {
            for (int i = 0; i < grilla.Rows.Count; i++)
                if (materiales[i].StockBajo())
                    grilla.Rows[i].DefaultCellStyle.BackColor = Ui.Rojo;
        }

        private void Nuevo(object sender, EventArgs e) => AbrirEditor(new Material());

        private void Editar(object sender, EventArgs e)
        {
            if (grilla.CurrentRow == null) { Ui.Aviso("Seleccioná un material."); return; }
            AbrirEditor(materiales[grilla.CurrentRow.Index]);
        }

        private void AbrirEditor(Material m)
        {
            using var f = new FrmMaterialEditar(m);
            if (f.ShowDialog(this) == DialogResult.OK) Cargar();
        }
    }

    public class FrmMaterialEditar : Form
    {
        private readonly StockController controller = new StockController();
        private readonly Material material;
        private readonly TextBox txtNombre = new TextBox();
        private readonly ComboBox cboUnidad = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown numStock;
        private readonly NumericUpDown numMinimo;
        private readonly NumericUpDown numPrecio;

        public FrmMaterialEditar(Material m)
        {
            material = m;
            Ui.Preparar(this, m.Id == 0 ? "Nuevo material" : "Editar material", 440, 320);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            txtNombre.Text = m.Nombre;
            cboUnidad.Items.AddRange(new object[] { "unidad", "m", "kg" });
            cboUnidad.SelectedItem = cboUnidad.Items.Contains(m.Unidad) ? m.Unidad : "unidad";
            numStock = Ui.Numero(m.StockActual);
            numMinimo = Ui.Numero(m.StockMinimo);
            numPrecio = Ui.Numero(m.Precio);

            var btnGuardar = Ui.Boton("Guardar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, e) => Close());
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            Controls.Add(Ui.Campos(("Nombre *", txtNombre), ("Unidad", cboUnidad),
                ("Stock actual", numStock), ("Stock mínimo", numMinimo), ("Precio ($)", numPrecio)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            material.Nombre = txtNombre.Text;
            material.Unidad = cboUnidad.SelectedItem.ToString();
            material.StockActual = numStock.Value;
            material.StockMinimo = numMinimo.Value;
            material.Precio = numPrecio.Value;

            Resultado r = controller.Guardar(material);
            if (!r.Ok) { Ui.Mostrar(r); return; }
            DialogResult = DialogResult.OK;
        }
    }
}
