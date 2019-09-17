using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Principal;
using System.Diagnostics;
using System.ComponentModel;

namespace PortForward
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (!IsUserAdministrator())
            {
                runAsAdmin();
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        static bool IsUserAdministrator()
        {
            bool isAdmin;
            try
            {
                WindowsIdentity user = WindowsIdentity.GetCurrent();
                WindowsPrincipal principal = new WindowsPrincipal(user);
                isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch (UnauthorizedAccessException ex)
            {
                isAdmin = false;
            }
            catch (Exception ex)
            {
                isAdmin = false;
            }
            return isAdmin;
        }

        static void runAsAdmin()
        {
            try
            {
                ProcessStartInfo info = new ProcessStartInfo(Application.ExecutablePath); // my own .exe
                info.UseShellExecute = true;
                info.Verb = "runas";   // invoke UAC prompt
                Process.Start(info);
            }
            catch (Win32Exception ex)
            {
                if (ex.NativeErrorCode == 1223) //The operation was canceled by the user.
                {
                    // MessageBox.Show("Why did you not selected Yes?");
                    Application.Exit();
                }
                else
                    throw new Exception("Something went wrong :-(");
            }
            Application.Exit();
        }
    }
}
