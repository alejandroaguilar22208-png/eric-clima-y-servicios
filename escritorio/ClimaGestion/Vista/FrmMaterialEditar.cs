using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmMaterialEditar : Form
    {
        private ControlStock control = new ControlStock();
        private Material material;

        public FrmMaterialEditar(Material material)
        {
            InitializeComponent();
            this.material = material;
        }

        private void FrmMaterialEditar_Load(object sender, EventArgs e)
        {
            cboUnidad.Items.AddRange(new object[] { "unidad", "m", "kg", "litro", "rollo", "caja" });
            lblTitulo.Text = material.Id == 0 ? "      Nuevo material" : "      Editar material";
            txtNombre.Text = material.Nombre;
            cboUnidad.Text = string.IsNullOrEmpty(material.Unidad) ? "unidad" : material.Unidad;
            numStock.Value = material.StockActual;
            numMinimo.Value = material.StockMinimo;
            numPrecio.Value = material.Precio;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (txtNombre.Text.Trim() == "") { errorProvider.SetError(txtNombre, "El nombre es obligatorio."); txtNombre.Focus(); return; }
            material.Nombre = txtNombre.Text;
            material.Unidad = cboUnidad.Text;
            material.StockActual = numStock.Value;
            material.StockMinimo = numMinimo.Value;
            material.Precio = numPrecio.Value;
            string error = control.guardar(material);
            if (error != "")
            {
                MessageBox.Show(error, "Material", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
