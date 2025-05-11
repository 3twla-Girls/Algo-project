using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MapRoutingProject
{
    internal static class Program
    {
        [DllImport ("kernel32.dll")]
        public static extern bool AllocConsole ( );

        [STAThread]
        static void Main ( )
        {
            // Open the console
            AllocConsole ();

            // Start the WinForms app
            ApplicationConfiguration.Initialize ();
            Application.Run (new Form1 ());
        }
    }
}
