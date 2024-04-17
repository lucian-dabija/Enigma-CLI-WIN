using System.Diagnostics;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace EnigmaCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var username = string.Empty;
            var hostname = string.Empty;
            var port = 22;
            if (args.Length < 1)
            {
                Console.WriteLine("Usage: enigma-cli.exe <username>@<hostname> [-p <port>]");
                return;
            }
            var server = args[0];
            if (!server.Contains('@'))
            {
                Console.WriteLine("Invalid server format. Use <username>@<hostname>");
                return;
            }
            var serverParts = server.Split('@');
            username = serverParts[0];
            hostname = serverParts[1];
            if (args.Length == 3 && args[1] == "-p")
            {
                if (!int.TryParse(args[2], out port))
                {
                    Console.WriteLine("Invalid port number.");
                    return;
                }
            }
            Console.Write("Enter your password: ");
            var password = ReadPassword();

            var privateKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_rsa");
            var publicKeyPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_rsa.pub");
            if (!File.Exists(publicKeyPath))
            {
                Console.WriteLine("Public key not found. Do you want to generate a new key pair? (y/n)");
                var response = Console.ReadLine();
                if (response.ToLower() == "y")
                {
                    GenerateKeyPair(privateKeyPath, publicKeyPath);
                }
                else
                {
                    return;
                }
            }
            var connectionInfo = new PasswordConnectionInfo(hostname, port, username, password);
            using (var client = new SshClient(connectionInfo))
            {
                try
                {
                    client.Connect();
                    var publicKeyText = File.ReadAllText(publicKeyPath);
                    client.RunCommand($"echo \"{publicKeyText}\" >> ~/.ssh/authorized_keys");
                    client.Disconnect();
                    Console.WriteLine("Public key has been copied to the remote server.");
                }
                catch (SshException e)
                {
                    Console.WriteLine($"An error occurred: {e.Message}");
                }
            }
        }

        private static void GenerateKeyPair(string privateKeyPath, string publicKeyPath)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "ssh-keygen",
                Arguments = $"-t rsa -f {privateKeyPath} -q -N \"\"",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };
            var process = Process.Start(psi);
            process.WaitForExit();
        }

        private static string ReadPassword()
        {
            string password = string.Empty;
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                {
                    break;
                }
                password += key.KeyChar;
                Console.Write("*");
            }
            Console.WriteLine();
            return password;
        }
    }
}
