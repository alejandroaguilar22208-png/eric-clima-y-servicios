using System;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmEquipoVentaEditar : Form
    {
        private ControlEquiposVenta control = new ControlEquiposVenta();
        private EquipoVenta equipo;

        public FrmEquipoVentaEditar(EquipoVenta equipo)
        {
            InitializeComponent();
            this.equipo = equipo;
        }

        private void FrmEquipoVentaEditar_Load(object sender, EventArgs e)
        {
            cboTipo.Items.AddRange(new object[] { "split", "ventana", "portatil", "piso_techo" });
            lblTitulo.Text = equipo.Id == 0 ? "      Nuevo equipo en venta" : "      Editar equipo en venta";
            txtMarca.Text = equipo.Marca;
            txtModelo.Text = equipo.Modelo;
            cboTipo.Text = string.IsNullOrEmpty(equipo.Tipo) ? "split" : equipo.Tipo;
            numFrigorias.Value = equipo.Frigorias == 0 ? 3000 : equipo.Frigorias;
            numPrecio.Value = equipo.Precio;
            numStock.Value = equipo.Stock;
            chkFrioCalor.Checked = equipo.FrioCalor;
            chkInverter.Checked = equipo.Inverter;
            txtDescripcion.Text = equipo.Descripcion;
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (txtMarca.Text.Trim() == "") { errorProvider.SetError(txtMarca, "La marca es obligatoria."); txtMarca.Focus(); return; }
            if (txtModelo.Text.Trim() == "") { errorProvider.SetError(txtModelo, "El modelo es obligatorio."); txtModelo.Focus(); return; }
            equipo.Marca = txtMarca.Text;
            equipo.Modelo = txtModelo.Text;
            equipo.Tipo = cboTipo.Text;
            equipo.Frigorias = (int)numFrigorias.Value;
            equipo.Precio = numPrecio.Value;
            equipo.Stock = (int)numStock.Value;
            equipo.FrioCalor = chkFrioCalor.Checked;
            equipo.Inverter = chkInverter.Checked;
            equipo.Descripcion = txtDescripcion.Text;
            string error = control.guardar(equipo);
            if (error != "")
            {
                MessageBox.Show(error, "Equipo en venta", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
