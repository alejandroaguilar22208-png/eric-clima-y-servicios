using System;
using System.Data;
using System.Windows.Forms;
using ClimaGestion.Control;
using ClimaGestion.Entidades;

namespace ClimaGestion.Vista
{
    public partial class FrmPresupuestoNuevo : Form
    {
        private ControlPresupuestos control = new ControlPresupuestos();
        private bool cargando = true;
        private int idGuardado;

        public int IdGuardado { get => idGuardado; }

        public FrmPresupuestoNuevo()
        {
            InitializeComponent();
            Estilos.Grilla(dgvItems);
        }

        private void FrmPresupuestoNuevo_Load(object sender, EventArgs e)
        {
            try
            {
                cboCliente.ValueMember = "id";
                cboCliente.DisplayMember = "nombre";
                cboCliente.DataSource = new ControlClientes().paraCombo();
                DataTable opciones = control.serviciosYMateriales();
                DataRow vacia = opciones.NewRow();
                vacia["nombre"] = "(elegí para agregar un renglón)";
                vacia["precio"] = 0;
                opciones.Rows.InsertAt(vacia, 0);
                cboAgregar.DisplayMember = "nombre";
                cboAgregar.DataSource = opciones;
            }
            catch (Exception ex)
            {
                MessageBox.Show(Errores.Mensaje(ex), "Nuevo presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            dtpVence.Value = DateTime.Today.AddDays(15);
            colPrecio.DefaultCellStyle.Format = "$ #,##0";
            colSubtotal.DefaultCellStyle.Format = "$ #,##0";
            cargando = false;
        }

        private void chkVence_CheckedChanged(object sender, EventArgs e)
        {
            dtpVence.Enabled = chkVence.Checked;
            if (!chkVence.Checked) dtpVence.Value = DateTime.Today.AddDays(15);
        }

        private void cboAgregar_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cargando || cboAgregar.SelectedIndex <= 0) return;
            DataRowView opcion = (DataRowView)cboAgregar.SelectedItem;
            int fila = dgvItems.Rows.Add(opcion["nombre"].ToString(), 1, Convert.ToDecimal(opcion["precio"]), Convert.ToDecimal(opcion["precio"]));
            dgvItems.CurrentCell = dgvItems.Rows[fila].Cells["colCantidad"];
            cboAgregar.SelectedIndex = 0;
            CalcularTotal();
        }

        private decimal Numero(object valor)
        {
            decimal numero;
            if (valor == null || !decimal.TryParse(valor.ToString().Replace("$", "").Trim(), out numero)) return 0;
            return numero;
        }

        private void dgvItems_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow fila = dgvItems.Rows[e.RowIndex];
            if (fila.IsNewRow) return;
            fila.Cells["colSubtotal"].Value = Numero(fila.Cells["colCantidad"].Value) * Numero(fila.Cells["colPrecio"].Value);
            CalcularTotal();
        }

        private void dgvItems_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (!cargando) CalcularTotal();
        }

        private Presupuesto ArmarPresupuesto()
        {
            Presupuesto p = new Presupuesto();
            p.IdCliente = cboCliente.SelectedValue == null ? 0 : Convert.ToInt32(cboCliente.SelectedValue);
            p.Vence = dtpVence.Value.Date;
            foreach (DataGridViewRow fila in dgvItems.Rows)
            {
                if (fila.IsNewRow) continue;
                ItemPresupuesto item = new ItemPresupuesto();
                item.Descripcion = fila.Cells["colDescripcion"].Value == null ? "" : fila.Cells["colDescripcion"].Value.ToString();
                item.Cantidad = Numero(fila.Cells["colCantidad"].Value);
                item.PrecioUnitario = Numero(fila.Cells["colPrecio"].Value);
                p.Items.Add(item);
            }
            return p;
        }

        private void CalcularTotal()
        {
            lblTotal.Text = "Total: " + ArmarPresupuesto().Total().ToString("$ #,##0");
        }

        private void btnGuardar_Click(object sender, EventArgs e)
        {
            dgvItems.EndEdit();
            Presupuesto p = ArmarPresupuesto();
            string error = control.crear(p);
            if (error != "")
            {
                MessageBox.Show(error, "Nuevo presupuesto", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            idGuardado = p.Id;
            this.DialogResult = DialogResult.OK;
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
