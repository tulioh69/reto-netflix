﻿namespace Cliente
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnInvocar = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnInvocar
            // 
            this.btnInvocar.Location = new System.Drawing.Point(425, 141);
            this.btnInvocar.Name = "btnInvocar";
            this.btnInvocar.Size = new System.Drawing.Size(75, 23);
            this.btnInvocar.TabIndex = 0;
            this.btnInvocar.Text = "Invocar";
            this.btnInvocar.UseVisualStyleBackColor = true;
            this.btnInvocar.Click += new System.EventHandler(this.btnInvocar_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnInvocar);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnInvocar;
    }
}

