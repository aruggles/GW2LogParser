namespace Gw2LogParser
{
    partial class MainForm
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
            components = new System.ComponentModel.Container();
            dataGridView = new DataGridView();
            gridBindingSource = new BindingSource(components);
            btnParse = new Button();
            btnCancel = new Button();
            btnClear = new Button();
            label1 = new Label();
            btnRefreshAPI = new Button();
            InputFile = new DataGridViewTextBoxColumn();
            statusDataGridViewTextBoxColumn = new DataGridViewTextBoxColumn();
            ButtonText = new DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)dataGridView).BeginInit();
            ((System.ComponentModel.ISupportInitialize)gridBindingSource).BeginInit();
            SuspendLayout();
            // 
            // dataGridView
            // 
            dataGridView.AllowDrop = true;
            dataGridView.AllowUserToAddRows = false;
            dataGridView.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.BackgroundColor = SystemColors.Control;
            dataGridView.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView.Columns.AddRange(new DataGridViewColumn[] { InputFile, statusDataGridViewTextBoxColumn, ButtonText });
            dataGridView.DataSource = gridBindingSource;
            dataGridView.Location = new Point(16, 98);
            dataGridView.Margin = new Padding(4, 5, 4, 5);
            dataGridView.MultiSelect = false;
            dataGridView.Name = "dataGridView";
            dataGridView.ReadOnly = true;
            dataGridView.RowHeadersWidth = 51;
            dataGridView.Size = new Size(1035, 465);
            dataGridView.TabIndex = 0;
            dataGridView.DragDrop += DataGridView_DragDrop;
            dataGridView.DragEnter += DataGridView_DragEnter;
            // 
            // gridBindingSource
            // 
            gridBindingSource.AllowNew = false;
            gridBindingSource.DataSource = typeof(EvtcParserExtensions.FormOperationController);
            // 
            // btnParse
            // 
            btnParse.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnParse.Location = new Point(841, 572);
            btnParse.Margin = new Padding(4, 5, 4, 5);
            btnParse.Name = "btnParse";
            btnParse.Size = new Size(209, 35);
            btnParse.TabIndex = 1;
            btnParse.Text = "Parse";
            btnParse.UseVisualStyleBackColor = true;
            btnParse.Click += BtnParse_Click;
            // 
            // btnCancel
            // 
            btnCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancel.Location = new Point(841, 618);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(100, 35);
            btnCancel.TabIndex = 2;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += BtnCancel_Click;
            // 
            // btnClear
            // 
            btnClear.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnClear.Location = new Point(951, 618);
            btnClear.Margin = new Padding(4, 5, 4, 5);
            btnClear.Name = "btnClear";
            btnClear.Size = new Size(100, 35);
            btnClear.TabIndex = 3;
            btnClear.Text = "Clear";
            btnClear.UseVisualStyleBackColor = true;
            btnClear.Click += BtnClear_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 69);
            label1.Margin = new Padding(4, 0, 4, 0);
            label1.Name = "label1";
            label1.Size = new Size(214, 20);
            label1.TabIndex = 4;
            label1.Text = "Drag and Drop Log files below";
            // 
            // btnRefreshAPI
            // 
            btnRefreshAPI.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRefreshAPI.Location = new Point(693, 572);
            btnRefreshAPI.Margin = new Padding(4, 5, 4, 5);
            btnRefreshAPI.Name = "btnRefreshAPI";
            btnRefreshAPI.Size = new Size(140, 35);
            btnRefreshAPI.TabIndex = 4;
            btnRefreshAPI.Text = "Reresh API Cache";
            btnRefreshAPI.UseVisualStyleBackColor = true;
            btnRefreshAPI.Click += BtnRefreshAPI_Click;
            // 
            // InputFile
            // 
            InputFile.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            InputFile.DataPropertyName = "InputFile";
            InputFile.FillWeight = 60F;
            InputFile.Frozen = true;
            InputFile.HeaderText = "InputFile";
            InputFile.MinimumWidth = 6;
            InputFile.Name = "InputFile";
            InputFile.ReadOnly = true;
            InputFile.Width = 395;
            // 
            // statusDataGridViewTextBoxColumn
            // 
            statusDataGridViewTextBoxColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            statusDataGridViewTextBoxColumn.DataPropertyName = "Status";
            statusDataGridViewTextBoxColumn.FillWeight = 30F;
            statusDataGridViewTextBoxColumn.HeaderText = "Status";
            statusDataGridViewTextBoxColumn.MinimumWidth = 6;
            statusDataGridViewTextBoxColumn.Name = "statusDataGridViewTextBoxColumn";
            statusDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ButtonText
            // 
            ButtonText.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ButtonText.DataPropertyName = "ButtonText";
            ButtonText.FillWeight = 10F;
            ButtonText.HeaderText = "Action";
            ButtonText.MinimumWidth = 6;
            ButtonText.Name = "ButtonText";
            ButtonText.ReadOnly = true;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1067, 692);
            Controls.Add(btnRefreshAPI);
            Controls.Add(label1);
            Controls.Add(btnClear);
            Controls.Add(btnCancel);
            Controls.Add(btnParse);
            Controls.Add(dataGridView);
            Margin = new Padding(4, 5, 4, 5);
            Name = "MainForm";
            Text = "Guild Wars 2 Log Parser";
            ((System.ComponentModel.ISupportInitialize)dataGridView).EndInit();
            ((System.ComponentModel.ISupportInitialize)gridBindingSource).EndInit();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.BindingSource gridBindingSource;
        private System.Windows.Forms.Button btnParse;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridViewTextBoxColumn locationDataGridViewTextBoxColumn;
        private System.Windows.Forms.Button btnRefreshAPI;
        private DataGridViewTextBoxColumn InputFile;
        private DataGridViewTextBoxColumn statusDataGridViewTextBoxColumn;
        private DataGridViewTextBoxColumn ButtonText;
    }
}

