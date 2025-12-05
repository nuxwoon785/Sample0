using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace YoloLabeler;

internal static class ApplicationConfiguration
{
    public static void Initialize()
    {
        Application.EnableVisualStyles();
        Application.SetHighDpiMode(HighDpiMode.SystemAware);
        Application.SetCompatibleTextRenderingDefault(false);
        ApplicationConfigurationSection();
    }

    [Conditional("NET5_0_OR_GREATER")]
    private static void ApplicationConfigurationSection()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(10))
        {
            Application.SetDefaultFont(new Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point));
        }
    }
}
