namespace todo.Forms
{
    partial class LicenseView
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
            LicenseTextBox = new RichTextBox();
            SuspendLayout();
            // 
            // LicenseTextBox
            // 
            LicenseTextBox.BackColor = Color.LightGray;
            LicenseTextBox.BorderStyle = BorderStyle.None;
            LicenseTextBox.Dock = DockStyle.Fill;
            LicenseTextBox.Location = new Point(0, 0);
            LicenseTextBox.Name = "LicenseTextBox";
            LicenseTextBox.ReadOnly = true;
            LicenseTextBox.Size = new Size(750, 500);
            LicenseTextBox.TabIndex = 0;
            LicenseTextBox.Text = "";
            LicenseTextBox.MouseDoubleClick += LicenseTextBox_MouseDoubleClick;
            // 
            // LicenseView
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            ClientSize = new Size(750, 500);
            Controls.Add(LicenseTextBox);
            Font = new Font("メイリオ", 9F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.None;
            Name = "LicenseView";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "LicenseView";
            Load += LicenseView_Load;
            ResumeLayout(false);
        }

        #endregion

        private RichTextBox LicenseTextBox;
    }
}