namespace ClimaGestion.Vista
{
    partial class FrmPresupuestoNuevo
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        private void InitializeComponent()
        {
            this.pnlEncabezado = new System.Windows.Forms.Panel();
            this.lblTitulo = new System.Windows.Forms.Label();
            this.pnlLinea = new System.Windows.Forms.Panel();
            this.pnlCabeceraDatos = new System.Windows.Forms.Panel();
            this.lblCliente = new System.Windows.Forms.Label();
            this.cboCliente = new System.Windows.Forms.ComboBox();
            this.chkVence = new System.Windows.Forms.CheckBox();
            this.dtpVence = new System.Windows.Forms.DateTimePicker();
            this.lblVence = new System.Windows.Forms.Label();
            this.lblAgregar = new System.Windows.Forms.Label();
            this.cboAgregar = new System.Windows.Forms.ComboBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCantidad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPrecio = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colSubtotal = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pnlBotones = new System.Windows.Forms.FlowLayoutPanel();
            this.btnGuardar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.lblTotal = new System.Windows.Forms.Label();
            this.pnlEncabezado.SuspendLayout();
            this.pnlLinea.SuspendLayout();
            this.pnlCabeceraDatos.SuspendLayout();
            this.pnlBotones.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.SuspendLayout();
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Controls.Add(this.pnlLinea);
            this.pnlEncabezado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(35)))), ((int)(((byte)(64)))));
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Size = new System.Drawing.Size(800, 58);
            this.pnlEncabezado.TabIndex = 90;
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Image = global::ClimaGestion.Properties.Resources.logo_chico;
            this.lblTitulo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblTitulo.Size = new System.Drawing.Size(800, 54);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "      Nuevo presupuesto";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Name = "lblTitulo";
            this.pnlLinea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.pnlLinea.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLinea.Location = new System.Drawing.Point(0, 54);
            this.pnlLinea.Size = new System.Drawing.Size(800, 4);
            this.pnlLinea.TabIndex = 1;
            this.pnlLinea.Name = "pnlLinea";
            this.pnlCabeceraDatos.Controls.Add(this.lblCliente);
            this.pnlCabeceraDatos.Controls.Add(this.cboCliente);
            this.pnlCabeceraDatos.Controls.Add(this.chkVence);
            this.pnlCabeceraDatos.Controls.Add(this.dtpVence);
            this.pnlCabeceraDatos.Controls.Add(this.lblVence);
            this.pnlCabeceraDatos.Controls.Add(this.lblAgregar);
            this.pnlCabeceraDatos.Controls.Add(this.cboAgregar);
            this.pnlCabeceraDatos.BackColor = System.Drawing.Color.White;
            this.pnlCabeceraDatos.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlCabeceraDatos.Size = new System.Drawing.Size(800, 140);
            this.pnlCabeceraDatos.TabIndex = 1;
            this.pnlCabeceraDatos.Name = "pnlCabeceraDatos";
            this.lblCliente.AutoSize = true;
            this.lblCliente.Location = new System.Drawing.Point(24, 22);
            this.lblCliente.Size = new System.Drawing.Size(72, 17);
            this.lblCliente.TabIndex = 80;
            this.lblCliente.Text = "Cliente *";
            this.lblCliente.Name = "lblCliente";
            this.cboCliente.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboCliente.FormattingEnabled = true;
            this.cboCliente.Location = new System.Drawing.Point(180, 19);
            this.cboCliente.Size = new System.Drawing.Size(580, 25);
            this.cboCliente.TabIndex = 0;
            this.cboCliente.Name = "cboCliente";
            this.chkVence.AutoSize = true;
            this.chkVence.Location = new System.Drawing.Point(24, 60);
            this.chkVence.Size = new System.Drawing.Size(88, 21);
            this.chkVence.TabIndex = 1;
            this.chkVence.Text = "Vence el";
            this.chkVence.UseVisualStyleBackColor = true;
            this.chkVence.Size = new System.Drawing.Size(90, 21);
            this.chkVence.Name = "chkVence";
            this.chkVence.CheckedChanged += new System.EventHandler(this.chkVence_CheckedChanged);
            this.dtpVence.Enabled = false;
            this.dtpVence.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpVence.Location = new System.Drawing.Point(180, 58);
            this.dtpVence.Size = new System.Drawing.Size(160, 25);
            this.dtpVence.TabIndex = 2;
            this.dtpVence.Name = "dtpVence";
            this.lblVence.AutoSize = true;
            this.lblVence.Location = new System.Drawing.Point(350, 62);
            this.lblVence.Size = new System.Drawing.Size(272, 17);
            this.lblVence.TabIndex = 80;
            this.lblVence.Text = "(si no se marca, vence en 15 días)";
            this.lblVence.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblVence.Name = "lblVence";
            this.lblAgregar.AutoSize = true;
            this.lblAgregar.Location = new System.Drawing.Point(24, 102);
            this.lblAgregar.Size = new System.Drawing.Size(112, 17);
            this.lblAgregar.TabIndex = 80;
            this.lblAgregar.Text = "Agregar rápido";
            this.lblAgregar.Name = "lblAgregar";
            this.cboAgregar.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboAgregar.FormattingEnabled = true;
            this.cboAgregar.Location = new System.Drawing.Point(180, 99);
            this.cboAgregar.Size = new System.Drawing.Size(580, 25);
            this.cboAgregar.TabIndex = 3;
            this.cboAgregar.Name = "cboAgregar";
            this.cboAgregar.SelectedIndexChanged += new System.EventHandler(this.cboAgregar_SelectedIndexChanged);
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDescripcion,
            this.colCantidad,
            this.colPrecio,
            this.colSubtotal});
            this.dgvItems.AllowUserToAddRows = true;
            this.dgvItems.AllowUserToDeleteRows = true;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvItems.BackgroundColor = System.Drawing.Color.White;
            this.dgvItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvItems.MultiSelect = false;
            this.dgvItems.TabIndex = 4;
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellEndEdit);
            this.dgvItems.RowsRemoved += new System.Windows.Forms.DataGridViewRowsRemovedEventHandler(this.dgvItems_RowsRemoved);
            this.colDescripcion.FillWeight = 50F;
            this.colDescripcion.HeaderText = "Descripción";
            this.colDescripcion.Name = "colDescripcion";
            this.colCantidad.FillWeight = 15F;
            this.colCantidad.HeaderText = "Cantidad";
            this.colCantidad.Name = "colCantidad";
            this.colPrecio.FillWeight = 20F;
            this.colPrecio.HeaderText = "Precio unitario";
            this.colPrecio.Name = "colPrecio";
            this.colSubtotal.FillWeight = 20F;
            this.colSubtotal.HeaderText = "Subtotal";
            this.colSubtotal.ReadOnly = true;
            this.colSubtotal.Name = "colSubtotal";
            this.pnlBotones.Controls.Add(this.btnGuardar);
            this.pnlBotones.Controls.Add(this.btnCancelar);
            this.pnlBotones.Controls.Add(this.lblTotal);
            this.pnlBotones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            this.pnlBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBotones.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlBotones.Size = new System.Drawing.Size(800, 62);
            this.pnlBotones.TabIndex = 70;
            this.pnlBotones.Name = "pnlBotones";
            this.btnGuardar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.btnGuardar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGuardar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGuardar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.btnGuardar.FlatAppearance.BorderSize = 0;
            this.btnGuardar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnGuardar.ForeColor = System.Drawing.Color.White;
            this.btnGuardar.Image = global::ClimaGestion.Properties.Resources.guardar_blanco;
            this.btnGuardar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnGuardar.Margin = new System.Windows.Forms.Padding(4);
            this.btnGuardar.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnGuardar.Size = new System.Drawing.Size(112, 38);
            this.btnGuardar.TabIndex = 5;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnGuardar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Click += new System.EventHandler(this.btnGuardar_Click);
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(175)))));
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(175)))));
            this.btnCancelar.Image = global::ClimaGestion.Properties.Resources.cerrar;
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnCancelar.Size = new System.Drawing.Size(116, 38);
            this.btnCancelar.TabIndex = 6;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.lblTotal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTotal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(35)))), ((int)(((byte)(64)))));
            this.lblTotal.Margin = new System.Windows.Forms.Padding(24, 10, 4, 4);
            this.lblTotal.Size = new System.Drawing.Size(300, 26);
            this.lblTotal.TabIndex = 99;
            this.lblTotal.Text = "Total: $ 0";
            this.lblTotal.Name = "lblTotal";
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.dgvItems);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.pnlCabeceraDatos);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = global::ClimaGestion.Properties.Resources.climagestion;
            this.KeyPreview = true;
            this.Name = "FrmPresupuestoNuevo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Nuevo presupuesto - Eric Clima y Servicios";
            this.Load += new System.EventHandler(this.FrmPresupuestoNuevo_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.pnlLinea.ResumeLayout(false);
            this.pnlCabeceraDatos.ResumeLayout(false);
            this.pnlCabeceraDatos.PerformLayout();
            this.pnlBotones.ResumeLayout(false);
            this.pnlBotones.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlLinea;
        private System.Windows.Forms.Panel pnlCabeceraDatos;
        private System.Windows.Forms.Label lblCliente;
        private System.Windows.Forms.ComboBox cboCliente;
        private System.Windows.Forms.CheckBox chkVence;
        private System.Windows.Forms.DateTimePicker dtpVence;
        private System.Windows.Forms.Label lblVence;
        private System.Windows.Forms.Label lblAgregar;
        private System.Windows.Forms.ComboBox cboAgregar;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCantidad;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPrecio;
        private System.Windows.Forms.DataGridViewTextBoxColumn colSubtotal;
        private System.Windows.Forms.FlowLayoutPanel pnlBotones;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Label lblTotal;
    }
}
