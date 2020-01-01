using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GuxtEditor
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
            try
            {
                var baseDir = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                var tileTypePath = Path.Combine(baseDir, "tiletypes.png");
                Application.Run(FormMain.Create(tileTypePath, Keybinds.Default.StageEditor));
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Error");
            }            
        }
    }
}
