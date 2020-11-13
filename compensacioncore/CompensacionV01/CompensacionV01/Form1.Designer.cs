namespace CompensacionV01
{
    partial class CompAut
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CompAut));
            this.tabPage5 = new System.Windows.Forms.TabPage();
            this.lblPathArchCompContable = new System.Windows.Forms.Label();
            this.btnOpenFileCompContable = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.lblPathArchivo = new System.Windows.Forms.Label();
            this.btnOpenFileSEP = new System.Windows.Forms.Button();
            this.btnRun = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lblArchGenSEP = new System.Windows.Forms.Label();
            this.dtpCompFechActual = new System.Windows.Forms.DateTimePicker();
            this.dtpComp = new System.Windows.Forms.DateTimePicker();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnOpenVB6Exe = new System.Windows.Forms.Button();
            this.btnCountRegMatchBD = new System.Windows.Forms.Button();
            this.btnDisconnPathRemoto = new System.Windows.Forms.Button();
            this.btnPathRemoto = new System.Windows.Forms.Button();
            this.btnRunScriptSQL = new System.Windows.Forms.Button();
            this.btnImportDataBD = new System.Windows.Forms.Button();
            this.btnOpenNotepad = new System.Windows.Forms.Button();
            this.btnXlsxToTextDelimTab = new System.Windows.Forms.Button();
            this.btnCreandoArchivo = new System.Windows.Forms.Button();
            this.btnEliminarCarpeta = new System.Windows.Forms.Button();
            this.btnEnviarCorreo = new System.Windows.Forms.Button();
            this.btnCopiarCarpeta = new System.Windows.Forms.Button();
            this.btnCopiarArchivo = new System.Windows.Forms.Button();
            this.btnComprimirCarpeta = new System.Windows.Forms.Button();
            this.btnComprimirArchivo = new System.Windows.Forms.Button();
            this.btnRenombrarCarpeta = new System.Windows.Forms.Button();
            this.btnRenombrarArchivo = new System.Windows.Forms.Button();
            this.btnCrearCarpeta = new System.Windows.Forms.Button();
            this.btnEliminarArchivo = new System.Windows.Forms.Button();
            this.tabCtrlEjecComp = new System.Windows.Forms.TabControl();
            this.tabPageBorrarRegistrosCoincidentes = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.txtBDBorrarRegCoincid = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btnRevertInsertDB = new System.Windows.Forms.Button();
            this.txtTablaPadre = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtTablaHija = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtBoxLogs = new System.Windows.Forms.RichTextBox();
            this.tabPage5.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabCtrlEjecComp.SuspendLayout();
            this.tabPageBorrarRegistrosCoincidentes.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPage5
            // 
            this.tabPage5.Controls.Add(this.lblPathArchCompContable);
            this.tabPage5.Controls.Add(this.btnOpenFileCompContable);
            this.tabPage5.Controls.Add(this.label3);
            this.tabPage5.Controls.Add(this.lblPathArchivo);
            this.tabPage5.Controls.Add(this.btnOpenFileSEP);
            this.tabPage5.Controls.Add(this.btnRun);
            this.tabPage5.Controls.Add(this.label2);
            this.tabPage5.Controls.Add(this.label1);
            this.tabPage5.Controls.Add(this.lblArchGenSEP);
            this.tabPage5.Controls.Add(this.dtpCompFechActual);
            this.tabPage5.Controls.Add(this.dtpComp);
            this.tabPage5.Location = new System.Drawing.Point(4, 22);
            this.tabPage5.Name = "tabPage5";
            this.tabPage5.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage5.Size = new System.Drawing.Size(666, 262);
            this.tabPage5.TabIndex = 4;
            this.tabPage5.Text = "Comp";
            this.tabPage5.UseVisualStyleBackColor = true;
            // 
            // lblPathArchCompContable
            // 
            this.lblPathArchCompContable.AutoSize = true;
            this.lblPathArchCompContable.Location = new System.Drawing.Point(165, 187);
            this.lblPathArchCompContable.Name = "lblPathArchCompContable";
            this.lblPathArchCompContable.Size = new System.Drawing.Size(28, 13);
            this.lblPathArchCompContable.TabIndex = 10;
            this.lblPathArchCompContable.Text = "path";
            // 
            // btnOpenFileCompContable
            // 
            this.btnOpenFileCompContable.Location = new System.Drawing.Point(168, 161);
            this.btnOpenFileCompContable.Name = "btnOpenFileCompContable";
            this.btnOpenFileCompContable.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFileCompContable.TabIndex = 9;
            this.btnOpenFileCompContable.Text = "Browse";
            this.btnOpenFileCompContable.UseVisualStyleBackColor = true;
            this.btnOpenFileCompContable.Click += new System.EventHandler(this.btnOpenFileCompContable_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 166);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Arch.ComprimidoContable:";
            // 
            // lblPathArchivo
            // 
            this.lblPathArchivo.AutoSize = true;
            this.lblPathArchivo.Location = new System.Drawing.Point(165, 118);
            this.lblPathArchivo.Name = "lblPathArchivo";
            this.lblPathArchivo.Size = new System.Drawing.Size(28, 13);
            this.lblPathArchivo.TabIndex = 7;
            this.lblPathArchivo.Text = "path";
            // 
            // btnOpenFileSEP
            // 
            this.btnOpenFileSEP.Location = new System.Drawing.Point(168, 92);
            this.btnOpenFileSEP.Name = "btnOpenFileSEP";
            this.btnOpenFileSEP.Size = new System.Drawing.Size(75, 23);
            this.btnOpenFileSEP.TabIndex = 6;
            this.btnOpenFileSEP.Text = "Browse";
            this.btnOpenFileSEP.UseVisualStyleBackColor = true;
            this.btnOpenFileSEP.Click += new System.EventHandler(this.btnOpenFileSEP_Click);
            // 
            // btnRun
            // 
            this.btnRun.Location = new System.Drawing.Point(293, 233);
            this.btnRun.Name = "btnRun";
            this.btnRun.Size = new System.Drawing.Size(75, 23);
            this.btnRun.TabIndex = 5;
            this.btnRun.Text = "Run";
            this.btnRun.UseVisualStyleBackColor = true;
            this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "FchComp:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(121, 57);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Hoy:";
            // 
            // lblArchGenSEP
            // 
            this.lblArchGenSEP.AutoSize = true;
            this.lblArchGenSEP.Location = new System.Drawing.Point(59, 97);
            this.lblArchGenSEP.Name = "lblArchGenSEP";
            this.lblArchGenSEP.Size = new System.Drawing.Size(91, 13);
            this.lblArchGenSEP.TabIndex = 2;
            this.lblArchGenSEP.Text = "Archivo gen SEP:";
            // 
            // dtpCompFechActual
            // 
            this.dtpCompFechActual.Location = new System.Drawing.Point(168, 57);
            this.dtpCompFechActual.Name = "dtpCompFechActual";
            this.dtpCompFechActual.Size = new System.Drawing.Size(200, 20);
            this.dtpCompFechActual.TabIndex = 1;
            this.dtpCompFechActual.Value = new System.DateTime(2020, 1, 29, 9, 39, 4, 0);
            this.dtpCompFechActual.ValueChanged += new System.EventHandler(this.dtpCompFechActual_ValueChanged);
            // 
            // dtpComp
            // 
            this.dtpComp.Location = new System.Drawing.Point(168, 17);
            this.dtpComp.Name = "dtpComp";
            this.dtpComp.Size = new System.Drawing.Size(200, 20);
            this.dtpComp.TabIndex = 0;
            this.dtpComp.Value = new System.DateTime(2020, 1, 29, 9, 38, 48, 0);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnOpenVB6Exe);
            this.tabPage1.Controls.Add(this.btnCountRegMatchBD);
            this.tabPage1.Controls.Add(this.btnDisconnPathRemoto);
            this.tabPage1.Controls.Add(this.btnPathRemoto);
            this.tabPage1.Controls.Add(this.btnRunScriptSQL);
            this.tabPage1.Controls.Add(this.btnImportDataBD);
            this.tabPage1.Controls.Add(this.btnOpenNotepad);
            this.tabPage1.Controls.Add(this.btnXlsxToTextDelimTab);
            this.tabPage1.Controls.Add(this.btnCreandoArchivo);
            this.tabPage1.Controls.Add(this.btnEliminarCarpeta);
            this.tabPage1.Controls.Add(this.btnEnviarCorreo);
            this.tabPage1.Controls.Add(this.btnCopiarCarpeta);
            this.tabPage1.Controls.Add(this.btnCopiarArchivo);
            this.tabPage1.Controls.Add(this.btnComprimirCarpeta);
            this.tabPage1.Controls.Add(this.btnComprimirArchivo);
            this.tabPage1.Controls.Add(this.btnRenombrarCarpeta);
            this.tabPage1.Controls.Add(this.btnRenombrarArchivo);
            this.tabPage1.Controls.Add(this.btnCrearCarpeta);
            this.tabPage1.Controls.Add(this.btnEliminarArchivo);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(666, 262);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Test";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnOpenVB6Exe
            // 
            this.btnOpenVB6Exe.Location = new System.Drawing.Point(17, 172);
            this.btnOpenVB6Exe.Name = "btnOpenVB6Exe";
            this.btnOpenVB6Exe.Size = new System.Drawing.Size(132, 23);
            this.btnOpenVB6Exe.TabIndex = 19;
            this.btnOpenVB6Exe.Text = "OpenVB6Exe";
            this.btnOpenVB6Exe.UseVisualStyleBackColor = true;
            this.btnOpenVB6Exe.Click += new System.EventHandler(this.btnOpenVB6Exe_Click);
            // 
            // btnCountRegMatchBD
            // 
            this.btnCountRegMatchBD.Location = new System.Drawing.Point(388, 172);
            this.btnCountRegMatchBD.Name = "btnCountRegMatchBD";
            this.btnCountRegMatchBD.Size = new System.Drawing.Size(132, 23);
            this.btnCountRegMatchBD.TabIndex = 18;
            this.btnCountRegMatchBD.Text = "ContarRegMatchBD";
            this.btnCountRegMatchBD.UseVisualStyleBackColor = true;
            this.btnCountRegMatchBD.Click += new System.EventHandler(this.btnCountRegMatchBD_Click);
            // 
            // btnDisconnPathRemoto
            // 
            this.btnDisconnPathRemoto.Location = new System.Drawing.Point(168, 201);
            this.btnDisconnPathRemoto.Name = "btnDisconnPathRemoto";
            this.btnDisconnPathRemoto.Size = new System.Drawing.Size(132, 23);
            this.btnDisconnPathRemoto.TabIndex = 17;
            this.btnDisconnPathRemoto.Text = "DisccPathRemoto";
            this.btnDisconnPathRemoto.UseVisualStyleBackColor = true;
            this.btnDisconnPathRemoto.Click += new System.EventHandler(this.btnDisconnPathRemoto_Click);
            // 
            // btnPathRemoto
            // 
            this.btnPathRemoto.Location = new System.Drawing.Point(168, 172);
            this.btnPathRemoto.Name = "btnPathRemoto";
            this.btnPathRemoto.Size = new System.Drawing.Size(132, 23);
            this.btnPathRemoto.TabIndex = 16;
            this.btnPathRemoto.Text = "PathRemoto";
            this.btnPathRemoto.UseVisualStyleBackColor = true;
            this.btnPathRemoto.Click += new System.EventHandler(this.btnPathRemoto_Click);
            // 
            // btnRunScriptSQL
            // 
            this.btnRunScriptSQL.Location = new System.Drawing.Point(168, 123);
            this.btnRunScriptSQL.Name = "btnRunScriptSQL";
            this.btnRunScriptSQL.Size = new System.Drawing.Size(132, 23);
            this.btnRunScriptSQL.TabIndex = 15;
            this.btnRunScriptSQL.Text = "RunScriptSQL";
            this.btnRunScriptSQL.UseVisualStyleBackColor = true;
            this.btnRunScriptSQL.Click += new System.EventHandler(this.btnRunScriptSQL_Click);
            // 
            // btnImportDataBD
            // 
            this.btnImportDataBD.Location = new System.Drawing.Point(168, 93);
            this.btnImportDataBD.Name = "btnImportDataBD";
            this.btnImportDataBD.Size = new System.Drawing.Size(132, 23);
            this.btnImportDataBD.TabIndex = 14;
            this.btnImportDataBD.Text = "ImportDataBD";
            this.btnImportDataBD.UseVisualStyleBackColor = true;
            this.btnImportDataBD.Click += new System.EventHandler(this.btnImportDataBD_Click);
            // 
            // btnOpenNotepad
            // 
            this.btnOpenNotepad.Location = new System.Drawing.Point(168, 64);
            this.btnOpenNotepad.Name = "btnOpenNotepad";
            this.btnOpenNotepad.Size = new System.Drawing.Size(132, 23);
            this.btnOpenNotepad.TabIndex = 13;
            this.btnOpenNotepad.Text = "OpenNotepad";
            this.btnOpenNotepad.UseVisualStyleBackColor = true;
            this.btnOpenNotepad.Click += new System.EventHandler(this.btnOpenNotepad_Click);
            // 
            // btnXlsxToTextDelimTab
            // 
            this.btnXlsxToTextDelimTab.Location = new System.Drawing.Point(168, 34);
            this.btnXlsxToTextDelimTab.Name = "btnXlsxToTextDelimTab";
            this.btnXlsxToTextDelimTab.Size = new System.Drawing.Size(132, 23);
            this.btnXlsxToTextDelimTab.TabIndex = 12;
            this.btnXlsxToTextDelimTab.Text = "XlsxToTextDelimTab";
            this.btnXlsxToTextDelimTab.UseVisualStyleBackColor = true;
            this.btnXlsxToTextDelimTab.Click += new System.EventHandler(this.btnXlsxToTextDelimTab_Click);
            // 
            // btnCreandoArchivo
            // 
            this.btnCreandoArchivo.Location = new System.Drawing.Point(6, 6);
            this.btnCreandoArchivo.Name = "btnCreandoArchivo";
            this.btnCreandoArchivo.Size = new System.Drawing.Size(143, 23);
            this.btnCreandoArchivo.TabIndex = 4;
            this.btnCreandoArchivo.Text = "CreandoArchivo";
            this.btnCreandoArchivo.UseVisualStyleBackColor = true;
            this.btnCreandoArchivo.Click += new System.EventHandler(this.btnCreandoArchivo_Click);
            // 
            // btnEliminarCarpeta
            // 
            this.btnEliminarCarpeta.Location = new System.Drawing.Point(388, 94);
            this.btnEliminarCarpeta.Name = "btnEliminarCarpeta";
            this.btnEliminarCarpeta.Size = new System.Drawing.Size(143, 23);
            this.btnEliminarCarpeta.TabIndex = 8;
            this.btnEliminarCarpeta.Text = "EliminarCarpeta";
            this.btnEliminarCarpeta.UseVisualStyleBackColor = true;
            this.btnEliminarCarpeta.Click += new System.EventHandler(this.btnEliminarCarpeta_Click);
            // 
            // btnEnviarCorreo
            // 
            this.btnEnviarCorreo.Location = new System.Drawing.Point(168, 6);
            this.btnEnviarCorreo.Name = "btnEnviarCorreo";
            this.btnEnviarCorreo.Size = new System.Drawing.Size(132, 23);
            this.btnEnviarCorreo.TabIndex = 11;
            this.btnEnviarCorreo.Text = "EnviarCorreo";
            this.btnEnviarCorreo.UseVisualStyleBackColor = true;
            this.btnEnviarCorreo.Click += new System.EventHandler(this.btnEnviarCorreo_Click);
            // 
            // btnCopiarCarpeta
            // 
            this.btnCopiarCarpeta.Location = new System.Drawing.Point(388, 35);
            this.btnCopiarCarpeta.Name = "btnCopiarCarpeta";
            this.btnCopiarCarpeta.Size = new System.Drawing.Size(143, 23);
            this.btnCopiarCarpeta.TabIndex = 7;
            this.btnCopiarCarpeta.Text = "CopiarCarpeta";
            this.btnCopiarCarpeta.UseVisualStyleBackColor = true;
            this.btnCopiarCarpeta.Click += new System.EventHandler(this.btnCopiarCarpeta_Click);
            // 
            // btnCopiarArchivo
            // 
            this.btnCopiarArchivo.Location = new System.Drawing.Point(6, 35);
            this.btnCopiarArchivo.Name = "btnCopiarArchivo";
            this.btnCopiarArchivo.Size = new System.Drawing.Size(143, 23);
            this.btnCopiarArchivo.TabIndex = 0;
            this.btnCopiarArchivo.Text = "CopiarArchivo";
            this.btnCopiarArchivo.UseVisualStyleBackColor = true;
            this.btnCopiarArchivo.Click += new System.EventHandler(this.btnCopiarArchivo_Click);
            // 
            // btnComprimirCarpeta
            // 
            this.btnComprimirCarpeta.Location = new System.Drawing.Point(388, 123);
            this.btnComprimirCarpeta.Name = "btnComprimirCarpeta";
            this.btnComprimirCarpeta.Size = new System.Drawing.Size(143, 23);
            this.btnComprimirCarpeta.TabIndex = 5;
            this.btnComprimirCarpeta.Text = "ComprimirCarpeta";
            this.btnComprimirCarpeta.UseVisualStyleBackColor = true;
            this.btnComprimirCarpeta.Click += new System.EventHandler(this.btnComprimirCarpeta_Click);
            // 
            // btnComprimirArchivo
            // 
            this.btnComprimirArchivo.Location = new System.Drawing.Point(6, 123);
            this.btnComprimirArchivo.Name = "btnComprimirArchivo";
            this.btnComprimirArchivo.Size = new System.Drawing.Size(143, 23);
            this.btnComprimirArchivo.TabIndex = 10;
            this.btnComprimirArchivo.Text = "ComprimirArchivo";
            this.btnComprimirArchivo.UseVisualStyleBackColor = true;
            this.btnComprimirArchivo.Click += new System.EventHandler(this.btnComprimirCarpeta_Click);
            // 
            // btnRenombrarCarpeta
            // 
            this.btnRenombrarCarpeta.Location = new System.Drawing.Point(388, 64);
            this.btnRenombrarCarpeta.Name = "btnRenombrarCarpeta";
            this.btnRenombrarCarpeta.Size = new System.Drawing.Size(143, 23);
            this.btnRenombrarCarpeta.TabIndex = 2;
            this.btnRenombrarCarpeta.Text = "RenombrarCarpeta";
            this.btnRenombrarCarpeta.UseVisualStyleBackColor = true;
            this.btnRenombrarCarpeta.Click += new System.EventHandler(this.btnRenombrarCarpeta_Click);
            // 
            // btnRenombrarArchivo
            // 
            this.btnRenombrarArchivo.Location = new System.Drawing.Point(6, 64);
            this.btnRenombrarArchivo.Name = "btnRenombrarArchivo";
            this.btnRenombrarArchivo.Size = new System.Drawing.Size(143, 23);
            this.btnRenombrarArchivo.TabIndex = 3;
            this.btnRenombrarArchivo.Text = "RenombrarArchivo";
            this.btnRenombrarArchivo.UseVisualStyleBackColor = true;
            this.btnRenombrarArchivo.Click += new System.EventHandler(this.btnRenombrarArchivo_Click);
            // 
            // btnCrearCarpeta
            // 
            this.btnCrearCarpeta.Location = new System.Drawing.Point(388, 6);
            this.btnCrearCarpeta.Name = "btnCrearCarpeta";
            this.btnCrearCarpeta.Size = new System.Drawing.Size(143, 23);
            this.btnCrearCarpeta.TabIndex = 1;
            this.btnCrearCarpeta.Text = "CrearCarpeta";
            this.btnCrearCarpeta.UseVisualStyleBackColor = true;
            this.btnCrearCarpeta.Click += new System.EventHandler(this.btnCrearCarpeta_Click);
            // 
            // btnEliminarArchivo
            // 
            this.btnEliminarArchivo.Location = new System.Drawing.Point(6, 94);
            this.btnEliminarArchivo.Name = "btnEliminarArchivo";
            this.btnEliminarArchivo.Size = new System.Drawing.Size(143, 23);
            this.btnEliminarArchivo.TabIndex = 9;
            this.btnEliminarArchivo.Text = "EliminarArchivo";
            this.btnEliminarArchivo.UseVisualStyleBackColor = true;
            this.btnEliminarArchivo.Click += new System.EventHandler(this.btnEliminarArchivo_Click);
            // 
            // tabCtrlEjecComp
            // 
            this.tabCtrlEjecComp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabCtrlEjecComp.Controls.Add(this.tabPage5);
            this.tabCtrlEjecComp.Controls.Add(this.tabPage1);
            this.tabCtrlEjecComp.Controls.Add(this.tabPageBorrarRegistrosCoincidentes);
            this.tabCtrlEjecComp.Location = new System.Drawing.Point(12, 12);
            this.tabCtrlEjecComp.Name = "tabCtrlEjecComp";
            this.tabCtrlEjecComp.SelectedIndex = 0;
            this.tabCtrlEjecComp.Size = new System.Drawing.Size(674, 288);
            this.tabCtrlEjecComp.TabIndex = 13;
            this.tabCtrlEjecComp.SelectedIndexChanged += new System.EventHandler(this.tabCtrlEjecComp_SelectedIndexChanged_1);
            // 
            // tabPageBorrarRegistrosCoincidentes
            // 
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.label8);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.txtBDBorrarRegCoincid);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.label7);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.label6);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.btnRevertInsertDB);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.txtTablaPadre);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.label5);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.txtTablaHija);
            this.tabPageBorrarRegistrosCoincidentes.Controls.Add(this.label4);
            this.tabPageBorrarRegistrosCoincidentes.Location = new System.Drawing.Point(4, 22);
            this.tabPageBorrarRegistrosCoincidentes.Name = "tabPageBorrarRegistrosCoincidentes";
            this.tabPageBorrarRegistrosCoincidentes.Size = new System.Drawing.Size(666, 262);
            this.tabPageBorrarRegistrosCoincidentes.TabIndex = 5;
            this.tabPageBorrarRegistrosCoincidentes.Text = "BorrarRegistrosCoincidentes";
            this.tabPageBorrarRegistrosCoincidentes.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(364, 40);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(173, 13);
            this.label8.TabIndex = 8;
            this.label8.Text = "[ octopus_pci, octopus_pci_comp ]";
            // 
            // txtBDBorrarRegCoincid
            // 
            this.txtBDBorrarRegCoincid.Location = new System.Drawing.Point(134, 37);
            this.txtBDBorrarRegCoincid.Name = "txtBDBorrarRegCoincid";
            this.txtBDBorrarRegCoincid.Size = new System.Drawing.Size(214, 20);
            this.txtBDBorrarRegCoincid.TabIndex = 7;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(4, 37);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(110, 13);
            this.label7.TabIndex = 6;
            this.label7.Text = "BaseDatosConfigKey:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(131, 144);
            this.label6.MaximumSize = new System.Drawing.Size(400, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(384, 39);
            this.label6.TabIndex = 5;
            this.label6.Text = resources.GetString("label6.Text");
            // 
            // btnRevertInsertDB
            // 
            this.btnRevertInsertDB.Location = new System.Drawing.Point(134, 196);
            this.btnRevertInsertDB.Name = "btnRevertInsertDB";
            this.btnRevertInsertDB.Size = new System.Drawing.Size(214, 23);
            this.btnRevertInsertDB.TabIndex = 4;
            this.btnRevertInsertDB.Text = "Verificar";
            this.btnRevertInsertDB.UseVisualStyleBackColor = true;
            this.btnRevertInsertDB.Click += new System.EventHandler(this.btnRevertInsertDB_Click);
            // 
            // txtTablaPadre
            // 
            this.txtTablaPadre.Location = new System.Drawing.Point(134, 72);
            this.txtTablaPadre.Name = "txtTablaPadre";
            this.txtTablaPadre.Size = new System.Drawing.Size(214, 20);
            this.txtTablaPadre.TabIndex = 3;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(49, 75);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "TablaPadre:";
            // 
            // txtTablaHija
            // 
            this.txtTablaHija.Location = new System.Drawing.Point(134, 109);
            this.txtTablaHija.Name = "txtTablaHija";
            this.txtTablaHija.Size = new System.Drawing.Size(214, 20);
            this.txtTablaHija.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(59, 112);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "TablaHija:";
            this.label4.Click += new System.EventHandler(this.label4_Click);
            // 
            // txtBoxLogs
            // 
            this.txtBoxLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBoxLogs.Location = new System.Drawing.Point(3, 306);
            this.txtBoxLogs.Name = "txtBoxLogs";
            this.txtBoxLogs.Size = new System.Drawing.Size(693, 225);
            this.txtBoxLogs.TabIndex = 15;
            this.txtBoxLogs.Text = "";
            // 
            // CompAut
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(698, 532);
            this.Controls.Add(this.txtBoxLogs);
            this.Controls.Add(this.tabCtrlEjecComp);
            this.Name = "CompAut";
            this.Text = "CompAut";
            this.tabPage5.ResumeLayout(false);
            this.tabPage5.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabCtrlEjecComp.ResumeLayout(false);
            this.tabPageBorrarRegistrosCoincidentes.ResumeLayout(false);
            this.tabPageBorrarRegistrosCoincidentes.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TabPage tabPage5;
        private System.Windows.Forms.Label lblPathArchCompContable;
        private System.Windows.Forms.Button btnOpenFileCompContable;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label lblPathArchivo;
        private System.Windows.Forms.Button btnOpenFileSEP;
        private System.Windows.Forms.Button btnRun;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblArchGenSEP;
        private System.Windows.Forms.DateTimePicker dtpCompFechActual;
        private System.Windows.Forms.DateTimePicker dtpComp;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnCountRegMatchBD;
        private System.Windows.Forms.Button btnDisconnPathRemoto;
        private System.Windows.Forms.Button btnPathRemoto;
        private System.Windows.Forms.Button btnRunScriptSQL;
        private System.Windows.Forms.Button btnImportDataBD;
        private System.Windows.Forms.Button btnOpenNotepad;
        private System.Windows.Forms.Button btnXlsxToTextDelimTab;
        private System.Windows.Forms.Button btnCreandoArchivo;
        private System.Windows.Forms.Button btnEliminarCarpeta;
        private System.Windows.Forms.Button btnEnviarCorreo;
        private System.Windows.Forms.Button btnCopiarCarpeta;
        private System.Windows.Forms.Button btnCopiarArchivo;
        private System.Windows.Forms.Button btnComprimirCarpeta;
        private System.Windows.Forms.Button btnComprimirArchivo;
        private System.Windows.Forms.Button btnRenombrarCarpeta;
        private System.Windows.Forms.Button btnRenombrarArchivo;
        private System.Windows.Forms.Button btnCrearCarpeta;
        private System.Windows.Forms.Button btnEliminarArchivo;
        private System.Windows.Forms.TabControl tabCtrlEjecComp;
        private System.Windows.Forms.RichTextBox txtBoxLogs;
        private System.Windows.Forms.TabPage tabPageBorrarRegistrosCoincidentes;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnRevertInsertDB;
        private System.Windows.Forms.TextBox txtTablaPadre;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtTablaHija;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtBDBorrarRegCoincid;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnOpenVB6Exe;
    }
}

