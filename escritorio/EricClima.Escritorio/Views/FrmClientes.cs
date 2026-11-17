using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    /// <summary>Listado de clientes con búsqueda, alta, edición y baja.</summary>
    public class FrmClientes : Form
    {
        private readonly ClienteController controller = new ClienteController();
        private readonly TextBox txtBuscar = new TextBox { Width = 260, PlaceholderText = "Buscar por nombre, teléfono o dirección" };
        private readonly DataGridView grilla = Ui.Grilla();
        private List<Cliente> clientes = new List<Cliente>();

        public FrmClientes()
        {
            Ui.Preparar(this, "Clientes", 820, 480);
            txtBuscar.TextChanged += (s, e) => Cargar(); // busca mientras escribís
            grilla.CellDoubleClick += (s, e) => Editar(s, e);

            Controls.Add(grilla);
            Controls.Add(Ui.Barra(DockStyle.Top, new Label { Text = "Buscar:", AutoSize = true, Margin = new Padding(4, 8, 4, 4) }, txtBuscar));
            Controls.Add(Ui.Barra(DockStyle.Bottom,
                Ui.Boton("Nuevo", Nuevo, principal: true),
                Ui.Boton("Editar", Editar),
                Ui.Boton("Dar de baja", DarDeBaja),
                Ui.Boton("Cerrar", (s, e) => Close())));
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); };
            Load += (s, e) => Cargar();
        }

        private void Cargar()
        {
            try
            {
                clientes = controller.Buscar(txtBuscar.Text);
                // Se muestran solo las columnas que le sirven al usuario
                grilla.DataSource = clientes.Select(c => new
                {
                    c.Nombre, Teléfono = c.Telefono, Dirección = c.Direccion, c.Localidad, c.Email
                }).ToList();
            }
            catch (Exception ex) { Ui.Aviso("No se pudo cargar la lista: " + ex.Message); }
        }

        /// <summary>Devuelve el cliente de la fila seleccionada (o null).</summary>
        private Cliente Seleccionado() =>
            grilla.CurrentRow == null ? null : clientes[grilla.CurrentRow.Index];

        private void Nuevo(object sender, EventArgs e)
        {
            using var f = new FrmClienteEditar(new Cliente());
            if (f.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void Editar(object sender, EventArgs e)
        {
            Cliente c = Seleccionado();
            if (c == null) { Ui.Aviso("Seleccioná un cliente."); return; }
            using var f = new FrmClienteEditar(c);
            if (f.ShowDialog(this) == DialogResult.OK) Cargar();
        }

        private void DarDeBaja(object sender, EventArgs e)
        {
            Cliente c = Seleccionado();
            if (c == null) { Ui.Aviso("Seleccioná un cliente."); return; }
            if (!Ui.Confirmar($"¿Dar de baja a {c.Nombre}? Su historial se conserva.")) return;
            Resultado r = controller.DarDeBaja(c.Id);
            Ui.Mostrar(r);
            if (r.Ok) Cargar();
        }
    }

    /// <summary>Alta y modificación de un cliente (ventana modal).</summary>
    public class FrmClienteEditar : Form
    {
        private readonly ClienteController controller = new ClienteController();
        private readonly Cliente cliente;
        private readonly TextBox txtNombre = new TextBox();
        private readonly TextBox txtTelefono = new TextBox();
        private readonly TextBox txtEmail = new TextBox();
        private readonly TextBox txtDireccion = new TextBox();
        private readonly TextBox txtLocalidad = new TextBox();
        private readonly ErrorProvider errores = new ErrorProvider();

        public FrmClienteEditar(Cliente c)
        {
            cliente = c;
            Ui.Preparar(this, c.Id == 0 ? "Nuevo cliente" : "Editar cliente", 460, 330);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;

            // Pasar los datos del objeto a los controles
            txtNombre.Text = c.Nombre;
            txtTelefono.Text = c.Telefono;
            txtEmail.Text = c.Email;
            txtDireccion.Text = c.Direccion;
            txtLocalidad.Text = c.Localidad;

            var btnGuardar = Ui.Boton("Guardar", Guardar, principal: true);
            var btnCancelar = Ui.Boton("Cancelar", (s, e) => Close());
            AcceptButton = btnGuardar;  // Enter guarda
            CancelButton = btnCancelar; // Esc cancela

            Controls.Add(Ui.Campos(
                ("Nombre *", txtNombre), ("Teléfono *", txtTelefono), ("Email", txtEmail),
                ("Dirección *", txtDireccion), ("Localidad", txtLocalidad)));
            Controls.Add(Ui.Barra(DockStyle.Bottom, btnGuardar, btnCancelar));
        }

        private void Guardar(object sender, EventArgs e)
        {
            // Pasar los controles al objeto y que el controlador decida
            cliente.Nombre = txtNombre.Text;
            cliente.Telefono = txtTelefono.Text;
            cliente.Email = txtEmail.Text;
            cliente.Direccion = txtDireccion.Text;
            cliente.Localidad = txtLocalidad.Text;

            Resultado r = controller.Guardar(cliente);
            errores.Clear();
            if (!r.Ok)
            {
                // Ícono rojo al lado del campo que falló
                Control campo = r.Mensaje.Contains("nombre") ? txtNombre
                              : r.Mensaje.Contains("teléfono") ? txtTelefono
                              : r.Mensaje.Contains("email") ? txtEmail
                              : r.Mensaje.Contains("dirección") ? txtDireccion : null;
                if (campo != null) { errores.SetError(campo, r.Mensaje); campo.Focus(); }
                Ui.Mostrar(r);
                return;
            }
            DialogResult = DialogResult.OK;
        }
    }
}
