namespace ClimaGestion.Vista
{
    partial class FrmLogin
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
            this.pnlMarca = new System.Windows.Forms.Panel();
            this.picLogo = new System.Windows.Forms.PictureBox();
            this.lblEmpresa = new System.Windows.Forms.Label();
            this.lblSistema = new System.Windows.Forms.Label();
            this.pnlLineaLogin = new System.Windows.Forms.Panel();
            this.pnlDatos = new System.Windows.Forms.Panel();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.lblPassword = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.btnIngresar = new System.Windows.Forms.Button();
            this.btnCancelar = new System.Windows.Forms.Button();
            this.btnSalir = new System.Windows.Forms.Button();
            this.pnlMarca.SuspendLayout();
            this.pnlLineaLogin.SuspendLayout();
            this.pnlDatos.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            this.pnlMarca.Controls.Add(this.picLogo);
            this.pnlMarca.Controls.Add(this.lblEmpresa);
            this.pnlMarca.Controls.Add(this.lblSistema);
            this.pnlMarca.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(35)))), ((int)(((byte)(64)))));
            this.pnlMarca.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMarca.Size = new System.Drawing.Size(420, 150);
            this.pnlMarca.TabIndex = 90;
            this.pnlMarca.Name = "pnlMarca";
            this.picLogo.Image = global::ClimaGestion.Properties.Resources.logo;
            this.picLogo.Location = new System.Drawing.Point(174, 16);
            this.picLogo.Size = new System.Drawing.Size(72, 72);
            this.picLogo.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.picLogo.TabStop = false;
            this.picLogo.Name = "picLogo";
            this.lblEmpresa.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblEmpresa.ForeColor = System.Drawing.Color.White;
            this.lblEmpresa.Location = new System.Drawing.Point(0, 92);
            this.lblEmpresa.Size = new System.Drawing.Size(420, 30);
            this.lblEmpresa.TabIndex = 0;
            this.lblEmpresa.Text = "Eric Clima y Servicios";
            this.lblEmpresa.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblEmpresa.Name = "lblEmpresa";
            this.lblSistema.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(179)))), ((int)(((byte)(204)))));
            this.lblSistema.Location = new System.Drawing.Point(0, 122);
            this.lblSistema.Size = new System.Drawing.Size(420, 20);
            this.lblSistema.TabIndex = 1;
            this.lblSistema.Text = "ClimaGestión - Inicio de sesión";
            this.lblSistema.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblSistema.Name = "lblSistema";
            this.pnlLineaLogin.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.pnlLineaLogin.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLineaLogin.Size = new System.Drawing.Size(420, 4);
            this.pnlLineaLogin.TabIndex = 91;
            this.pnlLineaLogin.Name = "pnlLineaLogin";
            this.pnlDatos.Controls.Add(this.lblEmail);
            this.pnlDatos.Controls.Add(this.txtEmail);
            this.pnlDatos.Controls.Add(this.lblPassword);
            this.pnlDatos.Controls.Add(this.txtPassword);
            this.pnlDatos.Controls.Add(this.btnIngresar);
            this.pnlDatos.Controls.Add(this.btnCancelar);
            this.pnlDatos.Controls.Add(this.btnSalir);
            this.pnlDatos.BackColor = System.Drawing.Color.White;
            this.pnlDatos.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlDatos.Size = new System.Drawing.Size(420, 380);
            this.pnlDatos.TabIndex = 0;
            this.pnlDatos.Name = "pnlDatos";
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(36, 30);
            this.lblEmail.Size = new System.Drawing.Size(40, 17);
            this.lblEmail.TabIndex = 80;
            this.lblEmail.Text = "Email";
            this.lblEmail.Name = "lblEmail";
            this.txtEmail.Location = new System.Drawing.Point(140, 27);
            this.txtEmail.Size = new System.Drawing.Size(244, 25);
            this.txtEmail.TabIndex = 0;
            this.txtEmail.Name = "txtEmail";
            this.lblPassword.AutoSize = true;
            this.lblPassword.Location = new System.Drawing.Point(36, 70);
            this.lblPassword.Size = new System.Drawing.Size(80, 17);
            this.lblPassword.TabIndex = 80;
            this.lblPassword.Text = "Contraseña";
            this.lblPassword.Name = "lblPassword";
            this.txtPassword.Location = new System.Drawing.Point(140, 67);
            this.txtPassword.Size = new System.Drawing.Size(244, 25);
            this.txtPassword.TabIndex = 1;
            this.txtPassword.PasswordChar = '●';
            this.txtPassword.Name = "txtPassword";
            this.btnIngresar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.btnIngresar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnIngresar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnIngresar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(122)))), ((int)(((byte)(26)))));
            this.btnIngresar.FlatAppearance.BorderSize = 0;
            this.btnIngresar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnIngresar.ForeColor = System.Drawing.Color.White;
            this.btnIngresar.Image = global::ClimaGestion.Properties.Resources.ingresar_blanco;
            this.btnIngresar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnIngresar.Margin = new System.Windows.Forms.Padding(4);
            this.btnIngresar.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnIngresar.Size = new System.Drawing.Size(120, 38);
            this.btnIngresar.TabIndex = 2;
            this.btnIngresar.Text = "Ingresar";
            this.btnIngresar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnIngresar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnIngresar.UseVisualStyleBackColor = false;
            this.btnIngresar.Location = new System.Drawing.Point(36, 120);
            this.btnIngresar.Name = "btnIngresar";
            this.btnIngresar.Click += new System.EventHandler(this.btnIngresar_Click);
            this.btnCancelar.BackColor = System.Drawing.Color.White;
            this.btnCancelar.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCancelar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancelar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(175)))));
            this.btnCancelar.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancelar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(175)))));
            this.btnCancelar.Image = global::ClimaGestion.Properties.Resources.limpiar;
            this.btnCancelar.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancelar.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelar.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnCancelar.Size = new System.Drawing.Size(110, 38);
            this.btnCancelar.TabIndex = 3;
            this.btnCancelar.Text = "Limpiar";
            this.btnCancelar.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCancelar.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Location = new System.Drawing.Point(162, 120);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Click += new System.EventHandler(this.btnCancelar_Click);
            this.btnSalir.BackColor = System.Drawing.Color.White;
            this.btnSalir.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSalir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSalir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(175)))));
            this.btnSalir.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSalir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(92)))), ((int)(((byte)(175)))));
            this.btnSalir.Image = global::ClimaGestion.Properties.Resources.salir;
            this.btnSalir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSalir.Margin = new System.Windows.Forms.Padding(4);
            this.btnSalir.Padding = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.btnSalir.Size = new System.Drawing.Size(100, 38);
            this.btnSalir.TabIndex = 4;
            this.btnSalir.Text = "Salir";
            this.btnSalir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSalir.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSalir.UseVisualStyleBackColor = false;
            this.btnSalir.Location = new System.Drawing.Point(278, 120);
            this.btnSalir.Name = "btnSalir";
            this.btnSalir.Click += new System.EventHandler(this.btnSalir_Click);
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(420, 380);
            this.Controls.Add(this.pnlDatos);
            this.Controls.Add(this.pnlLineaLogin);
            this.Controls.Add(this.pnlMarca);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = global::ClimaGestion.Properties.Resources.climagestion;
            this.KeyPreview = true;
            this.Name = "FrmLogin";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ingreso - Eric Clima y Servicios";
            this.AcceptButton = this.btnIngresar;
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.pnlMarca.ResumeLayout(false);
            this.pnlMarca.PerformLayout();
            this.pnlLineaLogin.ResumeLayout(false);
            this.pnlDatos.ResumeLayout(false);
            this.pnlDatos.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Panel pnlMarca;
        private System.Windows.Forms.PictureBox picLogo;
        private System.Windows.Forms.Label lblEmpresa;
        private System.Windows.Forms.Label lblSistema;
        private System.Windows.Forms.Panel pnlLineaLogin;
        private System.Windows.Forms.Panel pnlDatos;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblPassword;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnIngresar;
        private System.Windows.Forms.Button btnCancelar;
        private System.Windows.Forms.Button btnSalir;
    }
}
