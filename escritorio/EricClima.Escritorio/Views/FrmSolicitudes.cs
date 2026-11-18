using EricClima.Escritorio.Controllers;
using EricClima.Escritorio.Models;

namespace EricClima.Escritorio.Views
{
    public class FrmSolicitudes : Form
    {
        private readonly SolicitudController controller = new SolicitudController();
        private readonly DataGridView grilla = Ui.Grilla();
        private readonly TextBox txtMensaje = new TextBox
        {
            Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical,
            BackColor = Color.White, BorderStyle = BorderStyle.None
        };
        private readonly System.Windows.Forms.Timer actualizar = new System.Windows.Forms.Timer { Interval = 30000 };
        private List<Solicitud> solicitudes = new List<Solicitud>();

        public FrmSolicitudes()
        {
            Ui.Preparar(this, "Solicitudes desde la web", 960, 500);
            grilla.SelectionChanged += (s, e) => MostrarMensaje();
            grilla.DataBindingComplete += (s, e) => Pintar();

            var panelMensaje = new Panel { Dock = DockStyle.Bottom, Height = 90, Padding = new Padding(12, 8, 12, 8), BackColor = Color.White };
            panelMensaje.Controls.Add(txtMensaje);
            panelMensaje.Controls.Add(new Label
            {
                Text = "Mensaje del cliente", Dock = DockStyle.Top, Height = 22,
                Font = Ui.FuenteNegrita, ForeColor = Ui.AzulOscuro
            });

            Controls.Add(grilla);
            Controls.Add(panelMensaje);
            Controls.Add(Ui.Barra(DockStyle.Top, new Label
            {
                Text = "Los pedidos que la gente manda desde el formulario de la página web llegan acá solos (se actualiza cada 30 segundos).",
                AutoSize = true, Margin = new Padding(4, 6, 4, 4)
            }));
            Controls.Add(Ui.Barra(DockStyle.Bottom,
                Ui.Boton("Convertir en cliente + trabajo", Convertir, principal: true),
                Ui.Boton("Contactado", (s, e) => Aplicar(controller.MarcarContactado)),
                Ui.Boton("Descartar", Descartar),
                Ui.Boton("Actualizar", (s, e) => Cargar()),
                Ui.Boton("Cerrar", (s, e) => Close())));

            actualizar.Tick += (s, e) => Cargar();
            KeyDown += (s, e) => { if (e.KeyCode == Keys.Escape) Close(); if (e.KeyCode == Keys.F5) Cargar(); };
            Load += (s, e) => { Cargar(); actualizar.Start(); };
            FormClosed += (s, e) => actualizar.Dispose();
        }

        private void Cargar()
        {
            int? seleccionado = Seleccionada()?.Id;
            try
            {
                solicitudes = controller.Listar();
                grilla.DataSource = solicitudes.Select(s => new
                {
                    Fecha = s.Fecha.ToString("dd/MM/yyyy HH:mm"),
                    s.Nombre,
                    Teléfono = s.Telefono,
                    Dirección = s.Direccion,
                    Servicio = s.ServicioNombre,
                    Estado = s.Estado.ToString()
                }).ToList();
                int i = solicitudes.FindIndex(s => s.Id == seleccionado);
                if (i >= 0) grilla.CurrentCell = grilla.Rows[i].Cells[0];
                MostrarMensaje();
            }
            catch (Exception ex)
            {
                actualizar.Stop();
                Ui.Aviso("No se pudieron cargar las solicitudes: " + ex.Message);
            }
        }

        private void Pintar()
        {
            for (int i = 0; i < grilla.Rows.Count; i++)
            {
                var estilo = grilla.Rows[i].DefaultCellStyle;
                switch (solicitudes[i].Estado)
                {
                    case EstadoSolicitud.Pendiente: estilo.Font = Ui.FuenteNegrita; break;
                    case EstadoSolicitud.Convertida: estilo.BackColor = Ui.Verde; break;
                    case EstadoSolicitud.Descartada: estilo.BackColor = Ui.Gris; estilo.ForeColor = Ui.TextoGris; break;
                }
            }
        }

        private Solicitud Seleccionada() =>
            grilla.CurrentRow == null || grilla.CurrentRow.Index >= solicitudes.Count ? null : solicitudes[grilla.CurrentRow.Index];

        private void MostrarMensaje()
        {
            Solicitud s = Seleccionada();
            txtMensaje.Text = s == null ? "" : (s.Mensaje == "" ? "(sin mensaje)" : s.Mensaje);
        }

        private void Aplicar(Func<Solicitud, Resultado> accion)
        {
            Solicitud s = Seleccionada();
            if (s == null) { Ui.Aviso("Seleccioná una solicitud."); return; }
            Resultado r = accion(s);
            Ui.Mostrar(r);
            if (r.Ok) Cargar();
        }

        private void Descartar(object sender, EventArgs e)
        {
            Solicitud s = Seleccionada();
            if (s == null) { Ui.Aviso("Seleccioná una solicitud."); return; }
            if (s.EstaCerrada()) { Ui.Aviso("Esa solicitud ya está cerrada."); return; }
            if (!Ui.Confirmar($"¿Seguro que querés descartar la solicitud de {s.Nombre}?\n\nSe va a borrar y no va a aparecer más, ni acá ni en la web.")) return;
            Aplicar(controller.Descartar);
        }

        private void Convertir(object sender, EventArgs e)
        {
            Solicitud s = Seleccionada();
            if (s == null) { Ui.Aviso("Seleccioná una solicitud."); return; }
            if (s.Estado != EstadoSolicitud.Convertida &&
                !Ui.Confirmar($"¿Crear el cliente \"{s.Nombre}\" y un trabajo pendiente de {s.ServicioNombre}?")) return;
            Aplicar(controller.Convertir);
        }
    }
}
