namespace ClimaGestion.Vista
{
    partial class FrmAgendar
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
            this.pnlDatos = new System.Windows.Forms.Panel();
            this.lblFecha = new System.Windows.Forms.Label();
            this.dtpFecha = new System.Windows.Forms.DateTimePicker();
            this.lblTecnico = new System.Windows.Forms.Label();
            this.cboTecnico = new System.Windows.Forms.ComboBox();
            this.pnlBotones = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAgendar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.pnlEncabezado.SuspendLayout();
            this.pnlLinea.SuspendLayout();
            this.pnlDatos.SuspendLayout();
            this.pnlBotones.SuspendLayout();
            this.SuspendLayout();
            this.pnlEncabezado.Controls.Add(this.lblTitulo);
            this.pnlEncabezado.Controls.Add(this.pnlLinea);
            this.pnlEncabezado.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(35)))), ((int)(((byte)(64)))));
            this.pnlEncabezado.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlEncabezado.Location = new System.Drawing.Point(0, 0);
            this.pnlEncabezado.Size = new System.Drawing.Size(460, 58);
            this.pnlEncabezado.TabIndex = 90;
            this.pnlEncabezado.Name = "pnlEncabezado";
            this.lblTitulo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblTitulo.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTitulo.ForeColor = System.Drawing.Color.White;
            this.lblTitulo.Image = global::ClimaGestion.Properties.Resources.logo_chico;
            this.lblTitulo.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Location = new System.Drawing.Point(0, 0);
            this.lblTitulo.Padding = new System.Windows.Forms.Padding(16, 0, 0, 0);
            this.lblTitulo.Size = new System.Drawing.Size(460, 54);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "      Agendar trabajo";
            this.lblTitulo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lblTitulo.Name = "lblTitulo";
            this.pnlLinea.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.pnlLinea.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlLinea.Location = new System.Drawing.Point(0, 54);
            this.pnlLinea.Size = new System.Drawing.Size(460, 4);
            this.pnlLinea.TabIndex = 1;
            this.pnlLinea.Name = "pnlLinea";
            this.pnlDatos.Controls.Add(this.lblFecha);
            this.pnlDatos.Controls.Add(this.dtpFecha);
            this.pnlDatos.Controls.Add(this.lblTecnico);
            this.pnlDatos.Controls.Add(this.cboTecnico);
            this.pnlDatos.BackColor = System.Drawing.Color.White;
            this.pnlDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDatos.Size = new System.Drawing.Size(460, 210);
            this.pnlDatos.TabIndex = 0;
            this.pnlDatos.Name = "pnlDatos";
            this.lblFecha.AutoSize = true;
            this.lblFecha.Location = new System.Drawing.Point(24, 25);
            this.lblFecha.Size = new System.Drawing.Size(112, 17);
            this.lblFecha.TabIndex = 80;
            this.lblFecha.Text = "Fecha y hora *";
            this.lblFecha.Name = "lblFecha";
            this.dtpFecha.CustomFormat = "dd/MM/yyyy HH:mm";
            this.dtpFecha.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpFecha.Location = new System.Drawing.Point(180, 22);
            this.dtpFecha.Size = new System.Drawing.Size(250, 25);
            this.dtpFecha.TabIndex = 0;
            this.dtpFecha.Name = "dtpFecha";
            this.lblTecnico.AutoSize = true;
            this.lblTecnico.Location = new System.Drawing.Point(24, 63);
            this.lblTecnico.Size = new System.Drawing.Size(72, 17);
            this.lblTecnico.TabIndex = 80;
            this.lblTecnico.Text = "Técnico *";
            this.lblTecnico.Name = "lblTecnico";
            this.cboTecnico.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboTecnico.FormattingEnabled = true;
            this.cboTecnico.Location = new System.Drawing.Point(180, 60);
            this.cboTecnico.Size = new System.Drawing.Size(250, 25);
            this.cboTecnico.TabIndex = 1;
            this.cboTecnico.Name = "cboTecnico";
            this.pnlBotones.Controls.Add(this.btnAgendar);
            this.pnlBotones.Controls.Add(this.btnCancelar);
            this.pnlBotones.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(246)))), ((int)(((byte)(251)))));
            this.pnlBotones.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlBotones.Padding = new System.Windows.Forms.Padding(10, 8, 10, 8);
            this.pnlBotones.Size = new System.Drawing.Size(460, 62);
            this.pnlBotones.TabIndex = 70;
            this.pnlBotones.Name = "pnlBotones";
            this.btnAgendar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.btnAgendar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAgendar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAgendar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.btnAgendar.FlatAppearance.BorderSize = 0;
            this.btnAgendar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAgendar.ForeColor = System.Drawing.Color.White;
            this.btnAgendar.Image = global::ClimaGestion.Properties.Resources.agendar_blanco;
            this.btnAgendar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnAgendar.Margin = new System.Windows.Forms.Padding(4);
            this.btnAgendar.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnAgendar.Size = new System.Drawing.Size(112, 38);
            this.btnAgendar.TabIndex = 2;
            this.btnAgendar.Text = "Agendar";
            this.btnAgendar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAgendar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAgendar.UseVisualStyleBackColor = false;
            this.btnAgendar.Name = "btnAgendar";
            this.btnAgendar.Click += new System.EventHandler(this.btnAgendar_Click);
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
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(460, 210);
            this.Controls.Add(this.pnlDatos);
            this.Controls.Add(this.pnlBotones);
            this.Controls.Add(this.pnlEncabezado);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = global::ClimaGestion.Properties.Resources.climagestion;
            this.KeyPreview = true;
            this.Name = "FrmAgendar";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Agendar trabajo - Eric Clima y Servicios";
            this.Load += new System.EventHandler(this.FrmAgendar_Load);
            this.pnlEncabezado.ResumeLayout(false);
            this.pnlEncabezado.PerformLayout();
            this.pnlLinea.ResumeLayout(false);
            this.pnlDatos.ResumeLayout(false);
            this.pnlDatos.PerformLayout();
            this.pnlBotones.ResumeLayout(false);
            this.pnlBotones.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlEncabezado;
        private System.Windows.Forms.Label lblTitulo;
        private System.Windows.Forms.Panel pnlLinea;
        private System.Windows.Forms.Panel pnlDatos;
        private System.Windows.Forms.Label lblFecha;
        private System.Windows.Forms.DateTimePicker dtpFecha;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.ComboBox cboTecnico;
        private System.Windows.Forms.FlowLayoutPanel pnlBotones;
        private System.Windows.Forms.Button btnAgendar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
