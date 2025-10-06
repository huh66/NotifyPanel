using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;
using System.Threading;

namespace NotifyPanel
{
    public partial class Form1 : Form
    {
        private TcpListener _listener;
        private int _port = 1526;
        private CancellationTokenSource _cts;
        private string _logPath;
        private ListBox _listBox;
        private ContextMenuStrip _contextMenu;
        private ToolStripMenuItem _settingsMenu;
        private ToolStripMenuItem _exitMenu; // Addition to the fields
        private NotifyIcon _trayIcon;
        

        private int LoadPortFromRegistry()
        {
            const int defaultPort = 1526;
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\NotifyPanel"))
            {
                if (key != null)
                {
                    var portValue = key.GetValue("Port");
                    if (portValue is int port && port >= 1024 && port <= 65535)
                        return port;
                }
            }
            return defaultPort;
        }

        public Form1()
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

            _port = LoadPortFromRegistry(); // Load port from registry
            InitializeCustomComponents();
            Load += Form1_Load;
            StartServer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("de");

            // Load settings from registry
            var minimizeToTray = true;
            using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\NotifyPanel"))
            {
                if (key != null)
                {
                    var trayValue = key.GetValue("MinimizeToTray");
                    if (trayValue is int tray)
                        minimizeToTray = tray != 0;
                }
            }

            if (minimizeToTray)
            {
                //this.Hide();
                WindowState = FormWindowState.Minimized;
                ShowInTaskbar = false;    
                _trayIcon.Visible = true;
            }
        }

        private void InitializeCustomComponents()
        {
            // ListBox for messages
            _listBox = new ListBox { Dock = DockStyle.Fill };
            Controls.Add(_listBox);

            // Context menuü
            _contextMenu = new ContextMenuStrip();
            _settingsMenu = new ToolStripMenuItem(Properties.Resources.MenuSettings_Text); // Use resources
            _settingsMenu.Click += SettingsMenu_Click;
            _contextMenu.Items.Add(_settingsMenu);

            // Add "Exit" menu item
            _exitMenu = new ToolStripMenuItem(Properties.Resources.MenuExit_Text); // Use resources
            _exitMenu.Click += ExitMenu_Click;
            _contextMenu.Items.Add(_exitMenu);

            ContextMenuStrip = _contextMenu;

            // Tray Icon
            _trayIcon = new NotifyIcon
            {
                Icon = this.Icon,
                Visible = false,
                Text = Properties.Resources.TrayIcon_Text // Use resources
            };
            _trayIcon.DoubleClick += (s, e) => { Show(); WindowState = FormWindowState.Normal; };
            _trayIcon.MouseClick += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    Show();
                    WindowState = FormWindowState.Normal;
                }
            };

            // Log path
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var dir = Path.Combine(appData, "NotifyPanel");
            Directory.CreateDirectory(dir);
            _logPath = Path.Combine(dir, "message_history.log");
        }

        private void StartServer()
        {
            _cts = new CancellationTokenSource();
            _listener = new TcpListener(IPAddress.Any, _port);
            _listener.Start();
            ListenForClients(_cts.Token);
        }

        private void StopServer()
        {
            _cts?.Cancel();
            _listener?.Stop();
            _listener = null;
        }

        private async void ListenForClients(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    var client = await _listener.AcceptTcpClientAsync();
                    _ = HandleClientAsync(client, token);
                }
                catch { }
            }
        }

        private async Task HandleClientAsync(TcpClient client, CancellationToken token)
        {
            using (client)
            using (var stream = client.GetStream())
            {
                var buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);
                var json = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                try
                {
                    var msg = JsonSerializer.Deserialize<Message>(json);
                    if (msg != null)
                    {
                        Invoke(new Action(() =>
                        {
                            // LEVEL is optional, default value "INFORMATION"
                            msg.LEVEL = string.IsNullOrWhiteSpace(msg.LEVEL) ? "INFORMATION" : msg.LEVEL;
                            ShowMessageForm(msg);
                            AddToList(msg);
                        }));
                    }
                }
                catch
                {
                    // Ignore invalid messages
                }
            }
        }

        private void ShowMessage(Message msg)
        {
            var text = $"HEADER: {msg.HEADER}\nSUBJECT: {msg.SUBJECT}\nREFERENZ: {msg.REFERENZ}\nMESSAGE: {msg.MESSAGE}";
            //var result = MessageBox.Show(text, "Neue Nachricht", MessageBoxButtons.OK, MessageBoxIcon.Information);
            var result = MessageBox.Show(text, msg.HEADER, MessageBoxButtons.OK, MessageBoxIcon.Information);
            if (result == DialogResult.OK)
            {
                LogMessage(msg);
            }
        }

        private void AddToList(Message msg)
        {
            _listBox.Items.Add($"{DateTime.Now:G} | {msg.HEADER} | {msg.LEVEL} | {msg.SUBJECT} | {msg.REFERENZ} | {msg.MESSAGE}");
        }

        private void LogMessage(Message msg)
        {
            var entry = $"{DateTime.Now:G};{msg.HEADER};{msg.SUBJECT};{msg.REFERENZ};{msg.MESSAGE}";
            File.AppendAllText(_logPath, entry + Environment.NewLine);
        }

        private void SettingsMenu_Click(object sender, EventArgs e)
        {
            StopServer(); // Stop server before dialog

            var dlg = new SettingsDialog(_port, _logPath);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (dlg.MinimizeToTray)
                {
                    Hide();
                    _trayIcon.Visible = true;
                }
                else
                {
                    Show();
                    _trayIcon.Visible = false;
                }

                if (dlg.Port != _port)
                {
                    _port = dlg.Port;
                }

                if (dlg.ViewLog)
                {
                    var log = File.Exists(_logPath) ? File.ReadAllText(_logPath) : "Keine Einträge.";
                    MessageBox.Show(
                        $"Log file: {_logPath}\n\n{log}",
                        "Logdatei - Einträge",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }

            StartServer(); // Restart server after dialog
        }

        private void ExitMenu_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var result = MessageBox.Show(
                Properties.Resources.ExitSureQuestion,
                Properties.Resources.MenuExit_Text,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );

            if (result != DialogResult.Yes)
            {
                e.Cancel = true;
                return;
            }

            _cts?.Cancel();
            _listener?.Stop();
            _trayIcon?.Dispose();
            base.OnFormClosing(e);
        }

        public class Message
        {
            public string HEADER { get; set; }
            public string SUBJECT { get; set; }
            public int REFERENZ { get; set; }
            public string MESSAGE { get; set; }
            public string LEVEL { get; set; } // NEW: LEVEL is optional
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
    $"Simple Message\n\nReceives messages from Firebird SQL Server and displays them.\n\nLog file: {_logPath}\n\n(c) HUH, Stark Gummiwalzen GmbH",
    "Information",
    MessageBoxButtons.OK,
    MessageBoxIcon.Information
);
        }

        private void beendenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExitMenu_Click(sender, e);
        }

        private void einstellungenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingsMenu_Click(sender, e);  
        }

        private void ShowMessageForm(Message msg)
        {
            using var form = new MessageForm(msg.HEADER, msg.SUBJECT, msg.REFERENZ, msg.MESSAGE, msg.LEVEL);
            form.ShowDialog(this);
            LogMessage(msg);
        }
    }
}