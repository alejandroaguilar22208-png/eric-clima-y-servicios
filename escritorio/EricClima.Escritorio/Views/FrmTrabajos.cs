using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    /// <summary>
    /// Trabajos: crear, agendar, cargar materiales y cerrar.
    /// Ciclo de estados: Pendiente → Agendado → Realizado (o Cancelado).
    /// </summary>
    public class FrmTrabajos : Form
    {
        private readonly TrabajoController controller = new TrabajoController();
        private readonly ComboBox cboEstado = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 160 };
        private readonly DataGridView grilla = Ui.Grilla();
        private List<Trabajo> trabajos = new List<Trabajo>();

        public FrmTrabajos()
        {
            Ui.Preparar(this, "Trabajos", 980, 520);
            cboEstado.Items.Add("Todos");
            foreach (EstadoTrabajo e in Enum.GetValues<EstadoTrabajo>()) cboEstado.Items.Add(e);
            cboEstado.SelectedIndex = 0;
            cboEstado.SelectedIndexChanged += (s, e) => Cargar();
            grilla.DataBindingComplete += (s, e) => Pintar();

            Controls.Add(grilla);
            Controls.Add(Ui.Barra(DockStyle.Top,
                new Label { Text = "Estado:", AutoSize = true, Margin = new Padding(4, 8, 4, 4) }, cboEstado));
            Controls.Add(Ui.Barra(DockStyle.Bottom,
                Ui.Boton("Nuevo trabajo", Nuevo, principal: true),
                Ui.Boton("Agendar", Agendar),
                Ui.Boton("Agregar material", AgregarMaterial),
                Ui.Boton("Ver materiales", VerMateriales),
                Ui.Boton("Marcar realizado", MarcarRealizado),
                Ui.Boton("Cancelar trabajo", Cancelar),
                Ui.Boton("Cerrar", (s, e) => Close())));
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                EstadoTrabajo? filtro = cboEstado.SelectedItem is EstadoTrabajo est ? est : null;
                trabajos = controller.Listar(filtro);
                grilla.DataSource = trabajos.Select(t => new
                {
                    N = t.Id,
                    Cliente = t.ClienteNombre,
                    Servicio = t.ServicioNombre,
                    Fecha = t.FechaAgendada?.ToString("dd/MM/yyyy HH:mm") ?? "-",
                    Técnico = t.TecnicoNombre,
                    Estado = t.Estado.ToString(),
                    Total = t.Total.ToString("C0"),
                    Garantía = t.GarantiaHasta?.ToString("dd/MM/yyyy") ?? "-"
                }).ToList();
            }
            catch (Exception ex) { Ui.Aviso("No se pudieron cargar los trabajos: " + ex.Message); }
        }

        /// <summary>Verde = realizado, gris = cancelado.</summary>
        private void Pintar()
        {
            for (int i = 0; i < grilla.Rows.Count; i++)
            {
                if (trabajos[i].Estado == EstadoTrabajo.Realizado) grilla.Rows[i].DefaultCellStyle.BackColor = Ui.Verde;
                if (trabajos[i].Estado == EstadoTrabajo.Cancelado) grilla.Rows[i].DefaultCellStyle.BackColor = Ui.Gris;
            }
        }

        private Trabajo Seleccionado()
        {
            if (grilla.CurrentRow == null) { Ui.Aviso("Seleccioná un trabajo."); return null; }
            return trabajos[grilla.CurrentRow.Index];
        }

        private void Nuevo(object sender, EventArgs e)
        {
            using var f = new FrmTrabajoNuevo();
            if (f.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void Agendar(object sender, EventArgs e)
        {
            Trabajo t = Seleccionado(); if (t == null) return;
            using var f = new FrmAgendar(t);
            if (f.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void AgregarMaterial(object sender, EventArgs e)
        {
            Trabajo t = Seleccionado(); if (t == null) return;
            using var f = new FrmAgregarMaterial(t);
            f.ShowDialog(this);
        }

        private void VerMateriales(object sender, EventArgs e)
        {
            Trabajo t = Seleccionado(); if (t == null) return;
            var lista = controller.MaterialesDe(t.Id);
            MessageBox.Show(lista.Count == 0 ? "Este trabajo no tiene materiales cargados." : string.Join("\n", lista),
                $"Materiales del trabajo N.º {t.Id}", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void MarcarRealizado(object sender, EventArgs e)
        {
            Trabajo t = Seleccionado(); if (t == null) return;
            if (!Ui.Confirmar("¿Marcar como realizado? Se descuenta el stock y ya no se podrá modificar.")) return;
            Resultado r = controller.MarcarRealizado(t);
            Ui.Mostrar(r);
            if (r.Ok) Cargar();
        }

        private void Cancelar(object sender, EventArgs e)
        {
            Trabajo t = Seleccionado(); if (t == null) return;
            if (!Ui.Confirmar("¿Cancelar este trabajo?")) return;
            Resultado r = controller.Cancelar(t);
            Ui.Mostrar(r);
            if (r.Ok) Cargar();
        }
    }

    public class FrmTrabajoNuevo : Form
    {
        private readonly TrabajoController controller = new TrabajoController();
        private readonly ComboBox cboCliente = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly ComboBox cboServicio = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown numTotal = Ui.Numero(0, 0);
        private readonly TextBox txtObs = new TextBox { Multiline = true, Height = 70 };

        public FrmTrabajoNuevo()
        {
            Ui.Preparar(this, "Nuevo trabajo", 480, 330);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            cboCliente.Items.AddRange(new ClienteController().Buscar("").ToArray());
            cboServicio.Items.AddRange(controller.Servicios().ToArray());
            // Al elegir el servicio, se sugiere su precio base
            cboServicio.SelectedIndexChanged += (s, e) =>
            {
                if (cboServicio.SelectedItem is Servicio sv) numTotal.Value = sv.PrecioBase;
            };

            var btnGuardar = Ui.Boton("Crear", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, e) => Close());
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            Controls.Add(Ui.Campos(("Cliente *", cboCliente), ("Servicio *", cboServicio),
                ("Total ($)", numTotal), ("Observaciones", txtObs)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            var t = new Trabajo
            {
                ClienteId = (cboCliente.SelectedItem as Cliente)?.Id ?? 0,
                ServicioId = (cboServicio.SelectedItem as Servicio)?.Id ?? 0,
                Total = numTotal.Value,
                Observaciones = txtObs.Text
            };
            Resultado r = controller.Crear(t);
            Ui.Mostrar(r);
            if (r.Ok) DialogResult = DialogResult.OK;
        }
    }

    public class FrmAgendar : Form
    {
        private readonly TrabajoController controller = new TrabajoController();
        private readonly Trabajo trabajo;
        private readonly DateTimePicker dtpFecha = new DateTimePicker
        {
            Format = DateTimePickerFormat.Custom, CustomFormat = "dd/MM/yyyy HH:mm"
        };
        private readonly ComboBox cboTecnico = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };

        public FrmAgendar(Trabajo t)
        {
            trabajo = t;
            Ui.Preparar(this, $"Agendar trabajo N.º {t.Id}", 420, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            dtpFecha.Value = t.FechaAgendada ?? DateTime.Today.AddDays(1).AddHours(9);
            cboTecnico.Items.AddRange(new UsuarioController().Tecnicos().ToArray());
            if (cboTecnico.Items.Count > 0) cboTecnico.SelectedIndex = 0;

            var btnGuardar = Ui.Boton("Agendar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, e) => Close());
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            Controls.Add(Ui.Campos(("Fecha y hora", dtpFecha), ("Técnico", cboTecnico)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            Resultado r = controller.Agendar(trabajo, dtpFecha.Value, cboTecnico.SelectedItem as Usuario);
            Ui.Mostrar(r);
            if (r.Ok) DialogResult = DialogResult.OK;
        }
    }

    public class FrmAgregarMaterial : Form
    {
        private readonly TrabajoController controller = new TrabajoController();
        private readonly Trabajo trabajo;
        private readonly ComboBox cboMaterial = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList };
        private readonly NumericUpDown numCantidad = Ui.Numero(1);

        public FrmAgregarMaterial(Trabajo t)
        {
            trabajo = t;
            Ui.Preparar(this, $"Material para trabajo N.º {t.Id}", 460, 200);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            cboMaterial.Items.AddRange(new StockController().Listar().ToArray());

            var btnGuardar = Ui.Boton("Agregar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cerrar", (s, e) => Close());
            AcceptButton = btnGuardar;
            CancelButton = btnCancelar;

            Controls.Add(Ui.Campos(("Material", cboMaterial), ("Cantidad", numCantidad)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            Resultado r = controller.AgregarMaterial(trabajo, cboMaterial.SelectedItem as Material, numCantidad.Value);
            Ui.Mostrar(r);
        }
    }
}
