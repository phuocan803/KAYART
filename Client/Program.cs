using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Client
{
    internal static class Program
    {
        private static int currentProcessId;
        
        [STAThread]
        static void Main()
        {
            currentProcessId = Process.GetCurrentProcess().Id;
            
            Application.ApplicationExit += OnApplicationExit;
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            Application.ThreadException += OnThreadException;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            try
            {
                Application.Run(new Form_Loading());
            }
            catch (Exception ex)
            {
            }
            finally
            {
                CleanupAllProcesses();
            }
        }
        
        private static void CleanupAllProcesses()
        {
            try
            {
                
                string processName = Process.GetCurrentProcess().ProcessName;
                
                var relatedProcesses = Process.GetProcessesByName(processName)
                    .Where(p => p.Id != currentProcessId)
                    .ToList();
                
                foreach (var process in relatedProcesses)
                {
                    try
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                            process.WaitForExit(1000); 
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                }
                
                try
                {
                    var serverProcesses = Process.GetProcessesByName("Server");
                    foreach (var server in serverProcesses)
                    {
                        if (!server.HasExited)
                        {
                            server.Kill();
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                
            }
            catch (Exception ex)
            {
            }
        }
        
        private static void OnApplicationExit(object sender, EventArgs e)
        {
            CleanupAllProcesses();
        }
        private static void OnProcessExit(object sender, EventArgs e)
        {
            CleanupAllProcesses();
        }
        
        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            CleanupAllProcesses();
        }
        
        private static void OnThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            CleanupAllProcesses();
        }
    }
}
