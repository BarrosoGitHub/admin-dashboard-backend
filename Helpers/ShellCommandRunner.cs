using System.Diagnostics;

namespace OPTConfigurator.Helpers;

public class ShellCommandRunner
{
    public void RunShellCommand(string command)
    {
        ProcessStartInfo psi = new ProcessStartInfo
        {
            FileName = "/bin/bash",
            RedirectStandardInput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            Arguments = $"-c \"{command}\""
        };

        using (Process process = Process.Start(psi)!)
        {
            using (StreamWriter sw = process.StandardInput)
            {
                if (sw.BaseStream.CanWrite)
                {
                    sw.WriteLine(command);
                }
            }

            process.WaitForExit();
        }
    }
}