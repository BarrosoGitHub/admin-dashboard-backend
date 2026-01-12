using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OPTConfigurator.Services;

public static class SystemUtils
{
    public static bool IsLinux
    {
        get
        {
            int p = (int)Environment.OSVersion.Platform;
            return (p == 4) || (p == 6) || (p == 128);
        }
    }
    public static void Reboot()
    {
        Console.WriteLine("Rebooting system...");

        if (IsLinux)
        {
            try
            {
                if (File.Exists("/proc/sysrq-trigger"))
                {
                    Console.WriteLine("Attempting reboot via SysRq trigger");
                    File.WriteAllText("/proc/sysrq-trigger", "b");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SysRq reboot failed: {0}", ex.Message);
            }

            try
            {
                Console.WriteLine("Attempting reboot via systemctl");
                ExecuteCommandSync("/bin/systemctl reboot");
            }
            catch (Exception ex)
            {
                Console.WriteLine("systemctl reboot failed: {0}", ex.Message);
            }

            try
            {
                Console.WriteLine("Attempting reboot via /sbin/reboot");
                ExecuteCommandSync("/sbin/reboot");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Direct reboot failed: {0}", ex.Message);
            }

            Console.WriteLine("All reboot methods failed, exiting application");
            Environment.Exit(0);
        }
        else
        {
            ExecuteCommandSync("shutdown -r -f -t 00");
        }
    }

    private static string ExecuteCommandSync(string command)
    {
        string result = string.Empty;

        try
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo =
                new System.Diagnostics.ProcessStartInfo(IsLinux ? "/bin/sh" : "cmd",
                    IsLinux ? "-c \"" + command + "\"" : "/c " + command)
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            // Do not create the black window.
            // Now we create a process, assign its ProcessStartInfo and start it
            using (System.Diagnostics.Process proc = new System.Diagnostics.Process())
            {
                proc.StartInfo = procStartInfo;
                Console.WriteLine("Executing process [{0}] [{1}]", procStartInfo.FileName, procStartInfo.Arguments);
                proc.Start();
                // Get the output into a string
                result = proc.StandardOutput.ReadToEnd();
                // Display the command output.
                Console.WriteLine(result);
            }
        }
        catch (Exception objException)
        {
            Console.WriteLine(objException);
        }

        return result;
    }
}
