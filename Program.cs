using System.Globalization;
using NotifyPanel.Properties;

namespace NotifyPanel
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Set culture to current Windows system language
            var systemCulture = CultureInfo.InstalledUICulture;
            var supportedCultures = new[] { "de", "en" };
            var cultureCode = supportedCultures.Contains(systemCulture.TwoLetterISOLanguageName)
                ? systemCulture.TwoLetterISOLanguageName
                : ""; // neutral

            Resources.Culture = string.IsNullOrEmpty(cultureCode)
                ? CultureInfo.InvariantCulture
                : new CultureInfo(cultureCode);

            Application.Run(new Form1());
        }
    }
}