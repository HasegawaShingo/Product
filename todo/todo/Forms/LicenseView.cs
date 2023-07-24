namespace todo.Forms
{
    public partial class LicenseView : Form
    {
        public LicenseView()
        {
            InitializeComponent();
        }

        private void LicenseView_Load(object sender, EventArgs e)
        {
            var licenseDirectory = $@"{Environment.CurrentDirectory}\License";
            if (!Directory.Exists(licenseDirectory))
            {
                LicenseTextBox.Text = "License folder was not found.";
                return;
            }

            var licenseFiles = Directory.GetFiles(licenseDirectory);
            if (!licenseFiles.Any())
            {
                LicenseTextBox.Text = "License file was not found.";
                return;
            }

            string message = string.Empty;
            foreach (var file in licenseFiles)
            {
                message += File.ReadAllText(file);
                message += Environment.NewLine;
                message += Environment.NewLine;
            }

            LicenseTextBox.Text = message;
        }

        private void LicenseTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Close();
        }
    }
}
