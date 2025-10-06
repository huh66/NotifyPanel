using Microsoft.Win32;
using System.Globalization;
using System.Windows.Forms;

namespace NotifyPanel
{
    public class SettingsDialog : Form
    {
        private Label portLabel;
        private NumericUpDown portBox;
        private CheckBox trayCheck;
        private Button logButton;
        private GroupBox groupBoxPort;
        private Button okButton;

        public int Port { get; private set; }
        public bool MinimizeToTray { get; private set; }
        public bool ViewLog { get; private set; }

        private const string RegistryPath = @"Software\NotifyPanel";

        public SettingsDialog(int currentPort, string logPath)
        {
            InitializeComponent();

            //// Set culture to current Windows system language
            //var systemCulture = CultureInfo.InstalledUICulture;
            //var supportedCultures = new[] { "de", "en" }; // available language codes
            //var cultureCode = supportedCultures.Contains(systemCulture.TwoLetterISOLanguageName)
            //    ? systemCulture.TwoLetterISOLanguageName
            //    : ""; // neutral

            //Properties.Resources.Culture = string.IsNullOrEmpty(cultureCode)
            //    ? CultureInfo.InvariantCulture
            //    : new CultureInfo(cultureCode);

            // Load settings from registry
            LoadSettings();

            portBox.Value = Math.Max(portBox.Minimum, Math.Min(currentPort, portBox.Maximum));
            logButton.Click += (s, e) => { ViewLog = true; DialogResult = DialogResult.OK; };
            okButton.Click += (s, e) =>
            {
                Port = (int)portBox.Value;
                MinimizeToTray = trayCheck.Checked;
                SaveSettings();
                DialogResult = DialogResult.OK;
            };
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsDialog));
            portBox = new NumericUpDown();
            trayCheck = new CheckBox();
            logButton = new Button();
            okButton = new Button();
            groupBoxPort = new GroupBox();
            ((System.ComponentModel.ISupportInitialize)portBox).BeginInit();
            groupBoxPort.SuspendLayout();
            SuspendLayout();
            // 
            // portBox
            // 
            portBox.Location = new Point(12, 21);
            portBox.Maximum = new decimal(new int[] { 65535, 0, 0, 0 });
            portBox.Minimum = new decimal(new int[] { 1024, 0, 0, 0 });
            portBox.Name = "portBox";
            portBox.Size = new Size(70, 23);
            portBox.TabIndex = 1;
            portBox.Value = new decimal(new int[] { 1024, 0, 0, 0 });
            // 
            // trayCheck
            // 
            trayCheck.Location = new Point(12, 32);
            trayCheck.Name = "trayCheck";
            trayCheck.Size = new Size(167, 24);
            trayCheck.TabIndex = 2;
            trayCheck.Text = Properties.Resources.SettingsDialog_MinimizeToTray;
            // 
            // logButton
            // 
            logButton.Location = new Point(170, 90);
            logButton.Name = "logButton";
            logButton.Size = new Size(82, 23);
            logButton.TabIndex = 3;
            logButton.Text = Properties.Resources.SettingsDialog_LogButton;
            // 
            // okButton
            // 
            okButton.Location = new Point(12, 90);
            okButton.Name = "okButton";
            okButton.Size = new Size(87, 23);
            okButton.TabIndex = 4;
            okButton.Text = Properties.Resources.SettingsDialog_OkButton;
            // 
            // groupBoxPort
            // 
            groupBoxPort.Controls.Add(portBox);
            groupBoxPort.Location = new Point(170, 12);
            groupBoxPort.Name = "groupBoxPort";
            groupBoxPort.Size = new Size(94, 60);
            groupBoxPort.TabIndex = 6;
            groupBoxPort.TabStop = false;
            groupBoxPort.Text = "Port";
            groupBoxPort.Enter += groupBox1_Enter;
            // 
            // SettingsDialog
            // 
            ClientSize = new Size(276, 132);
            Controls.Add(groupBoxPort);
            Controls.Add(trayCheck);
            Controls.Add(logButton);
            Controls.Add(okButton);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SettingsDialog";
            Text = "Settings";
            ((System.ComponentModel.ISupportInitialize)portBox).EndInit();
            groupBoxPort.ResumeLayout(false);
            ResumeLayout(false);
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void LoadSettings()
        {
            using var key = Registry.CurrentUser.OpenSubKey(RegistryPath);
            if (key != null)
            {
                var portValue = key.GetValue("Port");
                var trayValue = key.GetValue("MinimizeToTray");

                if (portValue is int port && port >= portBox.Minimum && port <= portBox.Maximum)
                    portBox.Value = port;
                if (trayValue is int tray)
                    trayCheck.Checked = tray != 0;
            }
        }

        private void SaveSettings()
        {
            using var key = Registry.CurrentUser.CreateSubKey(RegistryPath);
            key.SetValue("Port", (int)portBox.Value, RegistryValueKind.DWord);
            key.SetValue("MinimizeToTray", trayCheck.Checked ? 1 : 0, RegistryValueKind.DWord);
        }
    }
}