using System.Diagnostics;
using System.Net.Sockets;
using Renci.SshNet;
using Renci.SshNet.Common;
using Spectre.Console;

namespace Enigma_SSH_Helper
{
    class Program
    {
        private static void EnsureSshKeygenAvailable()
        {
            try
            {
                // Try to execute ssh-keygen to see if it is available
                var psi = new ProcessStartInfo
                {
                    FileName = "ssh-keygen",
                    Arguments = "-V", // This argument makes ssh-keygen print its version, which is a harmless way to check if it's available
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };
                var process = Process.Start(psi);
                process.WaitForExit();

                // If the exit code is not 0, ssh-keygen is not available
                if (process.ExitCode != 0)
                {
                    throw new Exception("ssh-keygen not found.");
                }
            }
            catch
            {
                Console.WriteLine("ssh-keygen not found. Please install the OpenSSH Client feature for Windows and try again.");
                Environment.Exit(1); // Exit the application
            }
        }
        static void Main(string[] args)
        {
            while (true)
            {
                //Check if the user has the OpenSSH Client feature installed
                //EnsureSshKeygenAvailable();
                // Use Spectre.Console to ask for server details
                var serverInput = AnsiConsole.Prompt(
                new TextPrompt<string>("Enter <username>@<hostname>:")
                    .Validate(input =>
                    {
                        if (string.IsNullOrEmpty(input) || !input.Contains('@'))
                        {
                            return ValidationResult.Error("[red]Invalid server format. Use <username>@<hostname>[/]");
                        }
                        return ValidationResult.Success();
                    }));

                var serverParts = serverInput.Split('@');
                var username = serverParts[0];
                var hostname = serverParts[1];

                // Ask for port with a default value
                var port = AnsiConsole.Prompt(
                    new TextPrompt<int>("Enter port:")
                        .DefaultValue(22)
                        .Validate(input => input > 0 && input <= 65535 ? ValidationResult.Success() : ValidationResult.Error("[red]Invalid port number.[/]")));

                // Securely ask for password
                var password = AnsiConsole.Prompt(
                    new TextPrompt<string>("Enter your password:")
                        .Secret());

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
                    catch (SshAuthenticationException)
                    {
                        Console.WriteLine("Authentication failed. Please check your username and password.");
                    }
                    catch (SocketException)
                    {
                        Console.WriteLine("Could not connect to the server. Please check the server address and port.");
                    }
                    catch (SshException e)
                    {
                        Console.WriteLine($"An error occurred: {e.Message}");
                    }
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
