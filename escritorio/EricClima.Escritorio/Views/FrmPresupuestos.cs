using System.Globalization;
using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    public class FrmPresupuestos : Form
    {
        private readonly PresupuestoController controller = new PresupuestoController();
        private readonly DataGridView grilla = Ui.Grilla();
        private List<Presupuesto> presupuestos = new List<Presupuesto>();

        public FrmPresupuestos()
        {
            Ui.Preparar(this, "Presupuestos", 860, 460);
            grilla.CellDoubleClick += (s, e) => Pdf(s, e);
            grilla.DataBindingComplete += (s, e) => Pintar();

            Controls.Add(grilla);
            Controls.Add(Ui.Barra(DockStyle.Bottom,
                Ui.Boton("Nuevo", Nuevo, principal: true),
                Ui.Boton("Ver PDF", Pdf),
                Ui.Boton("Aceptado", (s, e) => Cambiar(EstadoPresupuesto.Aceptado)),
                Ui.Boton("Rechazado", (s, e) => Cambiar(EstadoPresupuesto.Rechazado)),
                Ui.Boton("Cerrar", (s, e) => Close())));
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                presupuestos = controller.Listar();
                grilla.DataSource = presupuestos.Select(p => new
                {
                    Número = p.Id,
                    Cliente = p.ClienteNombre,
                    Fecha = p.Fecha.ToString("dd/MM/yyyy"),
                    Vence = p.Vence?.ToString("dd/MM/yyyy"),
                    Total = p.Total().ToString("C0", new CultureInfo("es-AR")),
                    Estado = p.Estado.ToString()
                }).ToList();
            }
            catch (Exception ex) { Ui.Aviso("No se pudieron cargar los presupuestos: " + ex.Message); }
        }

        private void Pintar()
        {
            for (int i = 0; i < grilla.Rows.Count; i++)
            {
                var estado = presupuestos[i].Estado;
                if (estado == EstadoPresupuesto.Aceptado) grilla.Rows[i].DefaultCellStyle.BackColor = Ui.Verde;
                else if (estado != EstadoPresupuesto.Pendiente) grilla.Rows[i].DefaultCellStyle.BackColor = Ui.Gris;
            }
        }

        private Presupuesto Seleccionado()
        {
            if (grilla.CurrentRow == null) { Ui.Aviso("Seleccioná un presupuesto."); return null; }
            return presupuestos[grilla.CurrentRow.Index];
        }

        private void Nuevo(object sender, EventArgs e)
        {
            using var f = new FrmPresupuestoNuevo();
            if (f.ShowDialog(this) == DialogResult.OK)
            {
                Cargar();
                if (Ui.Confirmar("¿Querés generar el PDF ahora?")) GuardarPdf(f.Creado);
            }
        }

        private void Pdf(object sender, EventArgs e)
        {
            var p = Seleccionado();
            if (p != null) GuardarPdf(p);
        }

        private void GuardarPdf(Presupuesto p)
        {
            using var dlg = new SaveFileDialog
            {
                Filter = "PDF (*.pdf)|*.pdf",
                FileName = $"Presupuesto {p.Id:D5} - {p.ClienteNombre}.pdf"
            };
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            Resultado r = controller.ExportarPdf(p, dlg.FileName);
            if (!r.Ok) { Ui.Mostrar(r); return; }
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo(dlg.FileName) { UseShellExecute = true });
        }

        private void Cambiar(EstadoPresupuesto estado)
        {
            var p = Seleccionado();
            if (p == null) return;
            Ui.Mostrar(controller.CambiarEstado(p, estado));
            Cargar();
        }
    }

    public class FrmPresupuestoNuevo : Form
    {
        private readonly PresupuestoController controller = new PresupuestoController();
        private readonly ComboBox cboCliente = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cboAgregar = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 330 };
        private readonly DateTimePicker dtpVence = new DateTimePicker { Format = DateTimePickerFormat.Short, ShowCheckBox = true, Checked = false };
        private readonly DataGridView items = new DataGridView
        {
            Dock = DockStyle.Fill, AllowUserToAddRows = true, RowHeadersVisible = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.White
        };
        private readonly Label lblTotal = new Label { AutoSize = true, Font = new Font("Segoe UI", 12F, FontStyle.Bold), Margin = new Padding(16, 8, 4, 4) };

        public Presupuesto Creado { get; private set; }

        public FrmPresupuestoNuevo()
        {
            Ui.Preparar(this, "Nuevo presupuesto", 760, 520);

            cboCliente.Items.AddRange(new ClienteController().Buscar("").ToArray());

            cboAgregar.Items.Add("— Agregar servicio o material —");
            foreach (Servicio s in new TrabajoController().Servicios()) cboAgregar.Items.Add(s);
            foreach (Material m in new StockController().Listar()) cboAgregar.Items.Add(m);
            cboAgregar.SelectedIndex = 0;
            cboAgregar.SelectedIndexChanged += (s, e) => AgregarDesdeLista();

            items.Columns.Add("descripcion", "Descripción");
            items.Columns.Add("cantidad", "Cantidad");
            items.Columns.Add("precio", "Precio unitario");
            items.Columns.Add("subtotal", "Subtotal");
            items.Columns["descripcion"].FillWeight = 250;
            items.Columns["subtotal"].ReadOnly = true;
            items.Columns["subtotal"].DefaultCellStyle.BackColor = Ui.Gris;
            items.CellEndEdit += (s, e) => Recalcular();
            items.RowsRemoved += (s, e) => Recalcular();

            var btnGuardar = Ui.Boton("Guardar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, e) => Close());
            CancelButton = btnCancelar;

            var datos = Ui.Campos(("Cliente *", cboCliente), ("Vence (opcional)", dtpVence));
            datos.Dock = DockStyle.Top;
            datos.Height = 110;

            var ayuda = new Label
            {
                Text = "Escribí los ítems en la tabla o elegilos de la lista. Si no marcás \"Vence\", vale 15 días.",
                AutoSize = true, ForeColor = Color.DimGray, Margin = new Padding(12, 8, 4, 4)
            };

            Controls.Add(items);
            Controls.Add(Ui.Barra(DockStyle.Top, cboAgregar, ayuda));
            Controls.Add(datos);
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar, lblTotal));
            Recalcular();
        }

        private void AgregarDesdeLista()
        {
            object elegido = cboAgregar.SelectedItem;
            if (elegido is Servicio s) items.Rows.Add(s.Nombre, "1", s.PrecioBase.ToString("0.##"), "");
            else if (elegido is Material m) items.Rows.Add($"{m.Nombre} ({m.Unidad})", "1", m.Precio.ToString("0.##"), "");
            else return;
            cboAgregar.SelectedIndex = 0;
            Recalcular();
        }

        private List<PresupuestoItem> LeerItems(out string error)
        {
            error = "";
            var lista = new List<PresupuestoItem>();
            foreach (DataGridViewRow fila in items.Rows)
            {
                if (fila.IsNewRow) continue;
                string desc = fila.Cells["descripcion"].Value?.ToString() ?? "";
                string cant = fila.Cells["cantidad"].Value?.ToString() ?? "";
                string precio = fila.Cells["precio"].Value?.ToString() ?? "";
                if (desc == "" && cant == "" && precio == "") continue;

                if (!decimal.TryParse(cant, out decimal c) || !decimal.TryParse(precio, out decimal pr))
                {
                    error = $"Revisá la cantidad y el precio de \"{desc}\": tienen que ser números.";
                    fila.DefaultCellStyle.BackColor = Ui.Rojo;
                    continue;
                }
                fila.DefaultCellStyle.BackColor = Color.White;
                var item = new PresupuestoItem { Descripcion = desc, Cantidad = c, PrecioUnitario = pr };
                fila.Cells["subtotal"].Value = item.Subtotal().ToString("N2");
                lista.Add(item);
            }
            return lista;
        }

        private void Recalcular()
        {
            var lista = LeerItems(out _);
            lblTotal.Text = "Total: " + lista.Sum(i => i.Subtotal()).ToString("C2", new CultureInfo("es-AR"));
        }

        private void Guardar(object sender, EventArgs e)
        {
            items.EndEdit();
            var lista = LeerItems(out string error);
            if (error != "") { Ui.Aviso(error); return; }

            var p = new Presupuesto
            {
                ClienteId = (cboCliente.SelectedItem as Cliente)?.Id ?? 0,
                Vence = dtpVence.Checked ? dtpVence.Value.Date : null,
                Items = lista
            };
            if (cboCliente.SelectedItem is Cliente cli)
            {
                p.ClienteNombre = cli.Nombre;
                p.ClienteDireccion = cli.Direccion;
                p.ClienteTelefono = cli.Telefono;
            }

            Resultado r = controller.Crear(p);
            Ui.Mostrar(r);
            if (!r.Ok) return;
            Creado = p;
            DialogResult = DialogResult.OK;
        }
    }
}
