using Gw2LogParser.Properties;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace Gw2LogParser
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            var thisAssembly = Assembly.GetExecutingAssembly();
            using var programHelper = new ProgramHelper(thisAssembly.GetName().Version);
            using var form = new MainForm(programHelper);
            Application.Run(form);
        }
    }
}
