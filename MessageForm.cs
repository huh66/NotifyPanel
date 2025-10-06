using System;
using System.Drawing;
using System.Windows.Forms;

namespace NotifyPanel
{
    public partial class MessageForm : Form
    {
        public string Level { get; private set; }

        public MessageForm(string header, string subject, int referenz, string message, string level)
        {
            InitializeComponent();

            Level = level?.ToUpperInvariant() ?? "INFORMATION";

            // Colors for background based on level
            Color backColor = Color.White;
            switch (Level)
            {
                case "ERROR":
                    backColor = Color.FromArgb(255, 220, 220); // Pastel red
                    break;
                case "WARN":
                    backColor = Color.FromArgb(255, 255, 220); // Pastel yellow
                    break;
                case "INFO":
                    backColor = Color.White;
                    break;
            }
            BackColor = backColor;

            lblHeader.Text = header;
            lblSubject.Text = subject;
            lblReferenz.Text = $"Referenz: {referenz}";
            lblMessage.Text = message;

            // Title line with date and time
            Text = $"Message - {DateTime.Now:dd.MM.yyyy HH:mm:ss}";
        }
    }
}