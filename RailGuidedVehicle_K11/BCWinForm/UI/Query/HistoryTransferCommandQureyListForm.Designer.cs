﻿namespace com.mirle.ibg3k0.bc.winform.UI
{
    partial class HistoryTransferCommandQureyListForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.m_exportBtn = new CCWin.SkinControl.SkinButton();
            this.btnlSearch = new CCWin.SkinControl.SkinButton();
            this.skinGroupBox4 = new CCWin.SkinControl.SkinGroupBox();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.m_EndDTCbx = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.m_StartDTCbx = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dgv_TransferCommand = new System.Windows.Forms.DataGridView();
            this.iDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.vHIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cARRIERIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lOTIDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tRANSFERSTATEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pRIORITYDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hOSTSOURCEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hOSTDESTINATIONDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMDINSERTIMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMDSTARTTIMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cMDFINISHTIMEDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hCMDMCSObjToShowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.skinGroupBox2 = new CCWin.SkinControl.SkinGroupBox();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.m_EqptIDCbx = new System.Windows.Forms.ComboBox();
            this.lbl_commandCountName = new System.Windows.Forms.Label();
            this.lbl_commandCountValue = new System.Windows.Forms.Label();
            this.sectionObjToShowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cMDMCSObjToShowBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.skinGroupBox4.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_TransferCommand)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCMDMCSObjToShowBindingSource)).BeginInit();
            this.skinGroupBox2.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.sectionObjToShowBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cMDMCSObjToShowBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this.skinGroupBox4, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.dgv_TransferCommand, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.skinGroupBox2, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lbl_commandCountName, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.lbl_commandCountValue, 3, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 130F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1602, 716);
            this.tableLayoutPanel1.TabIndex = 7;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.m_exportBtn);
            this.panel2.Controls.Add(this.btnlSearch);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(1455, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(144, 124);
            this.panel2.TabIndex = 79;
            // 
            // m_exportBtn
            // 
            this.m_exportBtn.BackColor = System.Drawing.Color.Transparent;
            this.m_exportBtn.BaseColor = System.Drawing.Color.LightGray;
            this.m_exportBtn.BorderColor = System.Drawing.Color.Black;
            this.m_exportBtn.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.m_exportBtn.DownBack = null;
            this.m_exportBtn.DownBaseColor = System.Drawing.Color.RoyalBlue;
            this.m_exportBtn.Font = new System.Drawing.Font("Arial", 14.25F);
            this.m_exportBtn.Image = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.export;
            this.m_exportBtn.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.m_exportBtn.ImageSize = new System.Drawing.Size(24, 24);
            this.m_exportBtn.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.m_exportBtn.Location = new System.Drawing.Point(8, 68);
            this.m_exportBtn.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.m_exportBtn.MouseBack = null;
            this.m_exportBtn.Name = "m_exportBtn";
            this.m_exportBtn.NormlBack = null;
            this.m_exportBtn.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.m_exportBtn.Size = new System.Drawing.Size(127, 33);
            this.m_exportBtn.TabIndex = 85;
            this.m_exportBtn.Text = "   Export";
            this.m_exportBtn.UseVisualStyleBackColor = false;
            this.m_exportBtn.Visible = false;
            // 
            // btnlSearch
            // 
            this.btnlSearch.BackColor = System.Drawing.Color.Transparent;
            this.btnlSearch.BaseColor = System.Drawing.Color.LightGray;
            this.btnlSearch.BorderColor = System.Drawing.Color.Black;
            this.btnlSearch.ControlState = CCWin.SkinClass.ControlState.Normal;
            this.btnlSearch.DownBack = null;
            this.btnlSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnlSearch.Font = new System.Drawing.Font("Arial", 14F);
            this.btnlSearch.ForeColor = System.Drawing.Color.Black;
            this.btnlSearch.Image = global::com.mirle.ibg3k0.bc.winform.Properties.Resources.se;
            this.btnlSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnlSearch.ImageSize = new System.Drawing.Size(24, 24);
            this.btnlSearch.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.btnlSearch.Location = new System.Drawing.Point(8, 28);
            this.btnlSearch.MouseBack = null;
            this.btnlSearch.Name = "btnlSearch";
            this.btnlSearch.NormlBack = null;
            this.btnlSearch.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.btnlSearch.Size = new System.Drawing.Size(127, 32);
            this.btnlSearch.TabIndex = 83;
            this.btnlSearch.Text = "Search";
            this.btnlSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnlSearch.UseVisualStyleBackColor = false;
            this.btnlSearch.Click += new System.EventHandler(this.btnlSearch_Click);
            // 
            // skinGroupBox4
            // 
            this.skinGroupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.skinGroupBox4.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox4.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox4.Controls.Add(this.tableLayoutPanel3);
            this.skinGroupBox4.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox4.Font = new System.Drawing.Font("Arial", 14F);
            this.skinGroupBox4.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox4.Location = new System.Drawing.Point(45, 3);
            this.skinGroupBox4.Name = "skinGroupBox4";
            this.skinGroupBox4.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox4.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox4.Size = new System.Drawing.Size(393, 124);
            this.skinGroupBox4.TabIndex = 78;
            this.skinGroupBox4.TabStop = false;
            this.skinGroupBox4.Text = "    Time    ";
            this.skinGroupBox4.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox4.TitleRectBackColor = System.Drawing.Color.LightSteelBlue;
            this.skinGroupBox4.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 2;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.12903F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.87097F));
            this.tableLayoutPanel3.Controls.Add(this.m_EndDTCbx, 1, 1);
            this.tableLayoutPanel3.Controls.Add(this.label3, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.m_StartDTCbx, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.label1, 0, 1);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(3, 25);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 2;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(387, 96);
            this.tableLayoutPanel3.TabIndex = 79;
            // 
            // m_EndDTCbx
            // 
            this.m_EndDTCbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_EndDTCbx.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.m_EndDTCbx.Font = new System.Drawing.Font("Arial", 14F);
            this.m_EndDTCbx.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.m_EndDTCbx.Location = new System.Drawing.Point(142, 57);
            this.m_EndDTCbx.Name = "m_EndDTCbx";
            this.m_EndDTCbx.Size = new System.Drawing.Size(242, 29);
            this.m_EndDTCbx.TabIndex = 66;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial", 14F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(3, 13);
            this.label3.Margin = new System.Windows.Forms.Padding(3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 22);
            this.label3.TabIndex = 57;
            this.label3.Text = "From Time";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // m_StartDTCbx
            // 
            this.m_StartDTCbx.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.m_StartDTCbx.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.m_StartDTCbx.Font = new System.Drawing.Font("Arial", 14F);
            this.m_StartDTCbx.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.m_StartDTCbx.Location = new System.Drawing.Point(142, 9);
            this.m_StartDTCbx.Name = "m_StartDTCbx";
            this.m_StartDTCbx.Size = new System.Drawing.Size(242, 29);
            this.m_StartDTCbx.TabIndex = 65;
            // 
            // label1
            // 
            this.label1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial", 14F);
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 61);
            this.label1.Margin = new System.Windows.Forms.Padding(3);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 22);
            this.label1.TabIndex = 61;
            this.label1.Text = "End Time ";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // dgv_TransferCommand
            // 
            this.dgv_TransferCommand.AllowUserToAddRows = false;
            this.dgv_TransferCommand.AutoGenerateColumns = false;
            this.dgv_TransferCommand.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_TransferCommand.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv_TransferCommand.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_TransferCommand.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.iDDataGridViewTextBoxColumn,
            this.vHIDDataGridViewTextBoxColumn,
            this.cARRIERIDDataGridViewTextBoxColumn,
            this.lOTIDDataGridViewTextBoxColumn,
            this.tRANSFERSTATEDataGridViewTextBoxColumn,
            this.pRIORITYDataGridViewTextBoxColumn,
            this.hOSTSOURCEDataGridViewTextBoxColumn,
            this.hOSTDESTINATIONDataGridViewTextBoxColumn,
            this.cMDINSERTIMEDataGridViewTextBoxColumn,
            this.cMDSTARTTIMEDataGridViewTextBoxColumn,
            this.cMDFINISHTIMEDataGridViewTextBoxColumn});
            this.tableLayoutPanel1.SetColumnSpan(this.dgv_TransferCommand, 4);
            this.dgv_TransferCommand.DataSource = this.hCMDMCSObjToShowBindingSource;
            this.dgv_TransferCommand.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgv_TransferCommand.GridColor = System.Drawing.SystemColors.ControlDarkDark;
            this.dgv_TransferCommand.Location = new System.Drawing.Point(3, 133);
            this.dgv_TransferCommand.MultiSelect = false;
            this.dgv_TransferCommand.Name = "dgv_TransferCommand";
            this.dgv_TransferCommand.ReadOnly = true;
            this.dgv_TransferCommand.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dgv_TransferCommand.RowTemplate.Height = 24;
            this.dgv_TransferCommand.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgv_TransferCommand.Size = new System.Drawing.Size(1596, 540);
            this.dgv_TransferCommand.TabIndex = 8;
            // 
            // iDDataGridViewTextBoxColumn
            // 
            this.iDDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.iDDataGridViewTextBoxColumn.HeaderText = "ID";
            this.iDDataGridViewTextBoxColumn.Name = "iDDataGridViewTextBoxColumn";
            this.iDDataGridViewTextBoxColumn.ReadOnly = true;
            this.iDDataGridViewTextBoxColumn.Width = 55;
            // 
            // vHIDDataGridViewTextBoxColumn
            // 
            this.vHIDDataGridViewTextBoxColumn.DataPropertyName = "VH_ID";
            this.vHIDDataGridViewTextBoxColumn.HeaderText = "VhID";
            this.vHIDDataGridViewTextBoxColumn.Name = "vHIDDataGridViewTextBoxColumn";
            this.vHIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.vHIDDataGridViewTextBoxColumn.Width = 75;
            // 
            // cARRIERIDDataGridViewTextBoxColumn
            // 
            this.cARRIERIDDataGridViewTextBoxColumn.DataPropertyName = "CARRIER_ID";
            this.cARRIERIDDataGridViewTextBoxColumn.HeaderText = "CST ID";
            this.cARRIERIDDataGridViewTextBoxColumn.Name = "cARRIERIDDataGridViewTextBoxColumn";
            this.cARRIERIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.cARRIERIDDataGridViewTextBoxColumn.Width = 95;
            // 
            // lOTIDDataGridViewTextBoxColumn
            // 
            this.lOTIDDataGridViewTextBoxColumn.DataPropertyName = "LOT_ID";
            this.lOTIDDataGridViewTextBoxColumn.HeaderText = "Lot ID";
            this.lOTIDDataGridViewTextBoxColumn.Name = "lOTIDDataGridViewTextBoxColumn";
            this.lOTIDDataGridViewTextBoxColumn.ReadOnly = true;
            this.lOTIDDataGridViewTextBoxColumn.Width = 95;
            // 
            // tRANSFERSTATEDataGridViewTextBoxColumn
            // 
            this.tRANSFERSTATEDataGridViewTextBoxColumn.DataPropertyName = "TRANSFERSTATE";
            this.tRANSFERSTATEDataGridViewTextBoxColumn.HeaderText = "State";
            this.tRANSFERSTATEDataGridViewTextBoxColumn.Name = "tRANSFERSTATEDataGridViewTextBoxColumn";
            this.tRANSFERSTATEDataGridViewTextBoxColumn.ReadOnly = true;
            this.tRANSFERSTATEDataGridViewTextBoxColumn.Width = 85;
            // 
            // pRIORITYDataGridViewTextBoxColumn
            // 
            this.pRIORITYDataGridViewTextBoxColumn.DataPropertyName = "PRIORITY";
            this.pRIORITYDataGridViewTextBoxColumn.HeaderText = "Priority";
            this.pRIORITYDataGridViewTextBoxColumn.Name = "pRIORITYDataGridViewTextBoxColumn";
            this.pRIORITYDataGridViewTextBoxColumn.ReadOnly = true;
            this.pRIORITYDataGridViewTextBoxColumn.Width = 115;
            // 
            // hOSTSOURCEDataGridViewTextBoxColumn
            // 
            this.hOSTSOURCEDataGridViewTextBoxColumn.DataPropertyName = "HOSTSOURCE";
            this.hOSTSOURCEDataGridViewTextBoxColumn.HeaderText = "Source";
            this.hOSTSOURCEDataGridViewTextBoxColumn.Name = "hOSTSOURCEDataGridViewTextBoxColumn";
            this.hOSTSOURCEDataGridViewTextBoxColumn.ReadOnly = true;
            this.hOSTSOURCEDataGridViewTextBoxColumn.Width = 95;
            // 
            // hOSTDESTINATIONDataGridViewTextBoxColumn
            // 
            this.hOSTDESTINATIONDataGridViewTextBoxColumn.DataPropertyName = "HOSTDESTINATION";
            this.hOSTDESTINATIONDataGridViewTextBoxColumn.HeaderText = "Dest";
            this.hOSTDESTINATIONDataGridViewTextBoxColumn.Name = "hOSTDESTINATIONDataGridViewTextBoxColumn";
            this.hOSTDESTINATIONDataGridViewTextBoxColumn.ReadOnly = true;
            this.hOSTDESTINATIONDataGridViewTextBoxColumn.Width = 75;
            // 
            // cMDINSERTIMEDataGridViewTextBoxColumn
            // 
            this.cMDINSERTIMEDataGridViewTextBoxColumn.DataPropertyName = "CMD_INSER_TIME";
            this.cMDINSERTIMEDataGridViewTextBoxColumn.HeaderText = "Inser time";
            this.cMDINSERTIMEDataGridViewTextBoxColumn.Name = "cMDINSERTIMEDataGridViewTextBoxColumn";
            this.cMDINSERTIMEDataGridViewTextBoxColumn.ReadOnly = true;
            this.cMDINSERTIMEDataGridViewTextBoxColumn.Width = 135;
            // 
            // cMDSTARTTIMEDataGridViewTextBoxColumn
            // 
            this.cMDSTARTTIMEDataGridViewTextBoxColumn.DataPropertyName = "CMD_START_TIME";
            this.cMDSTARTTIMEDataGridViewTextBoxColumn.HeaderText = "Start time";
            this.cMDSTARTTIMEDataGridViewTextBoxColumn.Name = "cMDSTARTTIMEDataGridViewTextBoxColumn";
            this.cMDSTARTTIMEDataGridViewTextBoxColumn.ReadOnly = true;
            this.cMDSTARTTIMEDataGridViewTextBoxColumn.Width = 135;
            // 
            // cMDFINISHTIMEDataGridViewTextBoxColumn
            // 
            this.cMDFINISHTIMEDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.cMDFINISHTIMEDataGridViewTextBoxColumn.DataPropertyName = "CMD_FINISH_TIME";
            this.cMDFINISHTIMEDataGridViewTextBoxColumn.HeaderText = "Finish Time";
            this.cMDFINISHTIMEDataGridViewTextBoxColumn.Name = "cMDFINISHTIMEDataGridViewTextBoxColumn";
            this.cMDFINISHTIMEDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // hCMDMCSObjToShowBindingSource
            // 
            this.hCMDMCSObjToShowBindingSource.DataSource = typeof(com.mirle.ibg3k0.sc.ObjectRelay.HCMD_MCSObjToShow);
            // 
            // skinGroupBox2
            // 
            this.skinGroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.skinGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.skinGroupBox2.BorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Controls.Add(this.tableLayoutPanel4);
            this.skinGroupBox2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.skinGroupBox2.Font = new System.Drawing.Font("Arial", 15F);
            this.skinGroupBox2.ForeColor = System.Drawing.Color.Black;
            this.skinGroupBox2.Location = new System.Drawing.Point(529, 3);
            this.skinGroupBox2.Name = "skinGroupBox2";
            this.skinGroupBox2.RectBackColor = System.Drawing.SystemColors.Control;
            this.skinGroupBox2.RoundStyle = CCWin.SkinClass.RoundStyle.All;
            this.skinGroupBox2.Size = new System.Drawing.Size(393, 124);
            this.skinGroupBox2.TabIndex = 76;
            this.skinGroupBox2.TabStop = false;
            this.skinGroupBox2.Text = "Vehicle ID ";
            this.skinGroupBox2.TitleBorderColor = System.Drawing.Color.Black;
            this.skinGroupBox2.TitleRectBackColor = System.Drawing.Color.LightSteelBlue;
            this.skinGroupBox2.TitleRoundStyle = CCWin.SkinClass.RoundStyle.All;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Controls.Add(this.m_EqptIDCbx, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(3, 26);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 1;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(387, 95);
            this.tableLayoutPanel4.TabIndex = 84;
            // 
            // m_EqptIDCbx
            // 
            this.m_EqptIDCbx.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.m_EqptIDCbx.Font = new System.Drawing.Font("Arial", 14F);
            this.m_EqptIDCbx.FormattingEnabled = true;
            this.m_EqptIDCbx.Location = new System.Drawing.Point(80, 32);
            this.m_EqptIDCbx.Name = "m_EqptIDCbx";
            this.m_EqptIDCbx.Size = new System.Drawing.Size(227, 30);
            this.m_EqptIDCbx.TabIndex = 53;
            // 
            // lbl_commandCountName
            // 
            this.lbl_commandCountName.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lbl_commandCountName.AutoSize = true;
            this.lbl_commandCountName.Location = new System.Drawing.Point(1379, 685);
            this.lbl_commandCountName.Name = "lbl_commandCountName";
            this.lbl_commandCountName.Size = new System.Drawing.Size(70, 22);
            this.lbl_commandCountName.TabIndex = 80;
            this.lbl_commandCountName.Text = "Count:";
            // 
            // lbl_commandCountValue
            // 
            this.lbl_commandCountValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lbl_commandCountValue.AutoSize = true;
            this.lbl_commandCountValue.Location = new System.Drawing.Point(1455, 676);
            this.lbl_commandCountValue.Name = "lbl_commandCountValue";
            this.lbl_commandCountValue.Size = new System.Drawing.Size(140, 40);
            this.lbl_commandCountValue.TabIndex = 81;
            this.lbl_commandCountValue.Text = "                    ";
            // 
            // sectionObjToShowBindingSource
            // 
            this.sectionObjToShowBindingSource.DataSource = typeof(com.mirle.ibg3k0.bc.winform.ObjectRelay.SectionObjToShow);
            // 
            // cMDMCSObjToShowBindingSource
            // 
            this.cMDMCSObjToShowBindingSource.DataSource = typeof(com.mirle.ibg3k0.sc.ObjectRelay.TRANSFERObjToShow);
            // 
            // HistoryTransferCommandQureyListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1602, 716);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Consolas", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "HistoryTransferCommandQureyListForm";
            this.Text = "TransferCommandQureyListForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.TransferCommandQureyListForm_FormClosed);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.skinGroupBox4.ResumeLayout(false);
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_TransferCommand)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.hCMDMCSObjToShowBindingSource)).EndInit();
            this.skinGroupBox2.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.sectionObjToShowBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cMDMCSObjToShowBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.BindingSource cMDMCSObjToShowBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.DataGridView dgv_TransferCommand;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMDIDDataGridViewTextBoxColumn;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox4;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.DateTimePicker m_EndDTCbx;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker m_StartDTCbx;
        private System.Windows.Forms.Label label1;
        private CCWin.SkinControl.SkinGroupBox skinGroupBox2;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private System.Windows.Forms.ComboBox m_EqptIDCbx;
        private System.Windows.Forms.Panel panel2;
        private CCWin.SkinControl.SkinButton m_exportBtn;
        private CCWin.SkinControl.SkinButton btnlSearch;
        private System.Windows.Forms.Label lbl_commandCountName;
        private System.Windows.Forms.Label lbl_commandCountValue;
        private System.Windows.Forms.BindingSource sectionObjToShowBindingSource;
        private System.Windows.Forms.BindingSource hCMDMCSObjToShowBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn iDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn vHIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cARRIERIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lOTIDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn tRANSFERSTATEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn pRIORITYDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hOSTSOURCEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hOSTDESTINATIONDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMDINSERTIMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMDSTARTTIMEDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn cMDFINISHTIMEDataGridViewTextBoxColumn;
    }
}