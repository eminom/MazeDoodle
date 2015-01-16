using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Diagnostics;
using DoodleControls;

namespace MazeDoodle
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String []args)
        {
            if (args.Length > 0)
            {
                RunInConsole(args);
                return;
            }

            DefaultProcess();
        }

        private static void RunInConsole(String []args)
        {
            //IntPtr ptr = GetForegroundWindow();
            //int u;
            //GetWindowThreadProcessId(ptr, out u);
            //Process process = Process.GetProcessById(u);
            //if (0 == String.CompareOrdinal(process.ProcessName.ToLower(), "cmd.exe"))
            //{
            //    AttachConsole(process.Id);
            //}
            //else
            //{
            //    AllocConsole();
            //}

            try
            {
                AllocConsole();
                Console.WriteLine("In console mode.");
                String target_file = args[0];
                if (target_file.EndsWith(".json") && File.Exists(target_file))
                {
                    Console.WriteLine("Loading \"{0}\"...", target_file);
                    DoodleCake dc = new DoodleCake(target_file);
                    File.Delete(target_file);
                    dc.WriteToJson(target_file);
                    Console.WriteLine("Flush done.\n");
                    //MessageBox.Show(target_file + "\r\nFlush done!");
                }
                Console.WriteLine("Press enter to exit.");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                FreeConsole();
            }
        }

        private static void DefaultProcess()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new EditorForm());
        }

        [DllImport("kernel32", SetLastError = true)]
        static extern bool AllocConsole();

        [DllImport("kernel32", SetLastError = true)]
        static extern bool FreeConsole();

        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);

        [DllImport("user32")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out int lpdwProcessId);
    }
}