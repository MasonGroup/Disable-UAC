using System;
using System.Diagnostics;
using System.Security.Principal;
using Microsoft.Win32;

class Program
{
    static void Main(string[] args)
    {
        // Define the registry key path for UAC settings
        string keyPath = @"HKEY_LOCAL_MACHINE\SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System";

        try
        {
            // Check if the application is already running as administrator
            if (!IsRunningAsAdmin())
            {
                // Restart the application with administrative privileges
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Process.GetCurrentProcess().MainModule.FileName;
                startInfo.Verb = "runas"; // Run as administrator
                Process.Start(startInfo);

                // Exit the current process
                Environment.Exit(0);
            }

            // Open the registry key
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Policies\System", true);

            // Check if the key exists
            if (key != null)
            {
                // Set the EnableLUA value to 0 to disable UAC
                key.SetValue("EnableLUA", 0, RegistryValueKind.DWord);

                // Close the registry key
                key.Close();

                Console.WriteLine("UAC has been disabled successfully.");
            }
            else
            {
                Console.WriteLine("Registry key not found. UAC may already be disabled.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }
    }

    static bool IsRunningAsAdmin()
    {
        // Check if the application is running with administrative privileges
        return new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator);
    }
}
