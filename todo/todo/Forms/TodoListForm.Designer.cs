namespace todo
{
    partial class TodoListForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TodoListForm));
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            splitContainer = new SplitContainer();
            ShowLicense = new Button();
            ButtonExit = new Button();
            LabelTitle = new Label();
            TodoList = new DataGridView();
            ListContextMenuStrip = new ContextMenuStrip(components);
            UpToolStripMenuItem = new ToolStripMenuItem();
            DownToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            CreateItemCToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            DeleteItemDToolStripMenuItem = new ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)splitContainer).BeginInit();
            splitContainer.Panel1.SuspendLayout();
            splitContainer.Panel2.SuspendLayout();
            splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)TodoList).BeginInit();
            ListContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // splitContainer
            // 
            splitContainer.Dock = DockStyle.Fill;
            splitContainer.IsSplitterFixed = true;
            splitContainer.Location = new Point(0, 0);
            splitContainer.Margin = new Padding(1);
            splitContainer.Name = "splitContainer";
            splitContainer.Orientation = Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            splitContainer.Panel1.BackColor = Color.DimGray;
            splitContainer.Panel1.Controls.Add(ShowLicense);
            splitContainer.Panel1.Controls.Add(ButtonExit);
            splitContainer.Panel1.Controls.Add(LabelTitle);
            splitContainer.Panel1.MouseDown += SplitContainer1_Panel1_MouseDown;
            splitContainer.Panel1.MouseMove += SplitContainer1_Panel1_MouseMove;
            // 
            // splitContainer.Panel2
            // 
            splitContainer.Panel2.BackColor = Color.LightGray;
            splitContainer.Panel2.Controls.Add(TodoList);
            splitContainer.Size = new Size(900, 500);
            splitContainer.SplitterDistance = 45;
            splitContainer.SplitterWidth = 1;
            splitContainer.TabIndex = 0;
            // 
            // ShowLicense
            // 
            ShowLicense.BackColor = Color.Silver;
            ShowLicense.FlatStyle = FlatStyle.Flat;
            ShowLicense.ForeColor = Color.DimGray;
            ShowLicense.Image = (Image)resources.GetObject("ShowLicense.Image");
            ShowLicense.Location = new Point(731, 3);
            ShowLicense.Name = "ShowLicense";
            ShowLicense.Size = new Size(75, 37);
            ShowLicense.TabIndex = 2;
            ShowLicense.UseVisualStyleBackColor = false;
            ShowLicense.Click += ShowLicense_Click;
            // 
            // ButtonExit
            // 
            ButtonExit.BackColor = Color.Silver;
            ButtonExit.FlatStyle = FlatStyle.Flat;
            ButtonExit.ForeColor = Color.LightBlue;
            ButtonExit.Image = (Image)resources.GetObject("ButtonExit.Image");
            ButtonExit.Location = new Point(812, 3);
            ButtonExit.Name = "ButtonExit";
            ButtonExit.Size = new Size(75, 37);
            ButtonExit.TabIndex = 1;
            ButtonExit.UseVisualStyleBackColor = false;
            ButtonExit.Click += ButtonExit_ClickAsync;
            // 
            // LabelTitle
            // 
            LabelTitle.AutoSize = true;
            LabelTitle.Font = new Font("メイリオ", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            LabelTitle.ForeColor = Color.LightGray;
            LabelTitle.Location = new Point(11, 8);
            LabelTitle.Name = "LabelTitle";
            LabelTitle.Size = new Size(60, 28);
            LabelTitle.TabIndex = 0;
            LabelTitle.Text = "Todo";
            LabelTitle.MouseDown += SplitContainer1_Panel1_MouseDown;
            LabelTitle.MouseMove += SplitContainer1_Panel1_MouseMove;
            // 
            // TodoList
            // 
            TodoList.AllowUserToOrderColumns = true;
            TodoList.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            TodoList.BorderStyle = BorderStyle.None;
            TodoList.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.ControlDark;
            dataGridViewCellStyle1.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.True;
            TodoList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            TodoList.ContextMenuStrip = ListContextMenuStrip;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = SystemColors.Control;
            dataGridViewCellStyle2.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.False;
            TodoList.DefaultCellStyle = dataGridViewCellStyle2;
            TodoList.Dock = DockStyle.Fill;
            TodoList.EnableHeadersVisualStyles = false;
            TodoList.GridColor = Color.Black;
            TodoList.Location = new Point(0, 0);
            TodoList.MultiSelect = false;
            TodoList.Name = "TodoList";
            TodoList.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.Transparent;
            dataGridViewCellStyle3.Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = Color.Gray;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.True;
            TodoList.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            TodoList.RowHeadersVisible = false;
            TodoList.RowHeadersWidth = 25;
            dataGridViewCellStyle4.BackColor = Color.LightGray;
            dataGridViewCellStyle4.SelectionBackColor = Color.DimGray;
            TodoList.RowsDefaultCellStyle = dataGridViewCellStyle4;
            TodoList.RowTemplate.Height = 25;
            TodoList.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            TodoList.Size = new Size(900, 454);
            TodoList.TabIndex = 0;
            TodoList.DataError += TodoList_DataError;
            // 
            // ListContextMenuStrip
            // 
            ListContextMenuStrip.Items.AddRange(new ToolStripItem[] { UpToolStripMenuItem, DownToolStripMenuItem, toolStripSeparator1, CreateItemCToolStripMenuItem, toolStripSeparator2, DeleteItemDToolStripMenuItem });
            ListContextMenuStrip.Name = "ContextMenuStrip";
            ListContextMenuStrip.Size = new Size(150, 104);
            ListContextMenuStrip.Opened += ContextMenuStrip_Opened;
            // 
            // UpToolStripMenuItem
            // 
            UpToolStripMenuItem.Name = "UpToolStripMenuItem";
            UpToolStripMenuItem.Size = new Size(149, 22);
            UpToolStripMenuItem.Text = "↑(&U)";
            UpToolStripMenuItem.Click += UpToolStripMenuItem_Click;
            // 
            // DownToolStripMenuItem
            // 
            DownToolStripMenuItem.Name = "DownToolStripMenuItem";
            DownToolStripMenuItem.Size = new Size(149, 22);
            DownToolStripMenuItem.Text = "↓(&D)";
            DownToolStripMenuItem.Click += DownToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(146, 6);
            // 
            // CreateItemCToolStripMenuItem
            // 
            CreateItemCToolStripMenuItem.Name = "CreateItemCToolStripMenuItem";
            CreateItemCToolStripMenuItem.Size = new Size(149, 22);
            CreateItemCToolStripMenuItem.Text = "Create Item(&C)";
            CreateItemCToolStripMenuItem.Click += CreateItemToolStripMenuItem_ClickAsync;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(146, 6);
            // 
            // DeleteItemDToolStripMenuItem
            // 
            DeleteItemDToolStripMenuItem.Name = "DeleteItemDToolStripMenuItem";
            DeleteItemDToolStripMenuItem.Size = new Size(149, 22);
            DeleteItemDToolStripMenuItem.Text = "Delete Item(&D)";
            DeleteItemDToolStripMenuItem.Click += DeleteItemToolStripMenuItem_ClickAsync;
            // 
            // TodoListForm
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(900, 500);
            Controls.Add(splitContainer);
            Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Name = "TodoListForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Form1";
            Load += TodoListForm_LoadAsync;
            splitContainer.Panel1.ResumeLayout(false);
            splitContainer.Panel1.PerformLayout();
            splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer).EndInit();
            splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)TodoList).EndInit();
            ListContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private SplitContainer splitContainer;
        private Label LabelTitle;
        private Button ButtonExit;
        private DataGridView TodoList;
        private ContextMenuStrip ListContextMenuStrip;
        private ToolStripMenuItem UpToolStripMenuItem;
        private ToolStripMenuItem DownToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem CreateItemCToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem DeleteItemDToolStripMenuItem;
        private Button ShowLicense;
    }
}