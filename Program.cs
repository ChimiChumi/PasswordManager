using System;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using CsvHelper;
using PasswordManager.Models;

namespace Mobiles
{
    class Program
    {
        static void Main(string[] args)
        {

            // Define the path to the configuration file
            string configFilePath = "config.txt";

            // Read the working directory from the configuration file
            string workDir = File.Exists(configFilePath) ? File.ReadAllText(configFilePath) : "..\\..\\PasswordManager\\database\\";

            string userName = "";
            string email = "";
            string masterPass = "";
            string? firstName = null;
            string? lastName = null;

            if (args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    switch (args[i])
                    {
                        case "--workdir":
                            if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                            {
                                workDir = args[i + 1];
                                // Update the configuration file with the new working directory
                                File.WriteAllText(configFilePath, workDir);
                                Console.WriteLine($"Working directory set to: {workDir}");
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("Error: Missing or invalid path after --workdir.");
                            }
                            break;

                        case "--register":
                            if (i + 3 < args.Length && !args[i + 1].StartsWith("--") && !args[i + 2].StartsWith("--") && !args[i + 3].StartsWith("--"))
                            {
                                userName = args[i + 1];
                                email = args[i + 2];
                                masterPass = args[i + 3];

                                // Check if last name is provided
                                if (i + 5 < args.Length && !args[i + 5].StartsWith("--"))
                                {
                                    firstName = args[i + 4];
                                    lastName = args[i + 5];
                                    i += 2;
                                }

                                // Check if first name is provided
                                if (i + 4 < args.Length && !args[i + 4].StartsWith("--"))
                                {
                                    firstName = args[i + 4];
                                    i++;
                                }

                                // Perform user registration and write to CSV
                                RegisterUser(userName, email, masterPass, firstName, lastName, workDir);

                                Console.WriteLine($"User '{userName}' registered successfully.");
                                i += 3; // Skip to the next valid argument
                            }
                            else
                            {
                                Console.WriteLine("Argument expects the following: --register <username> <email> <master-password> [firstname] [lastname]");
                            }
                            break;

                        case "--login":
                            if (i + 1 < args.Length && !string.IsNullOrEmpty(args[i + 1]) && !args[i + 1].StartsWith("--"))
                            {
                                userName = args[i + 1];
                                string pass = string.Empty;

                                // The password user enters during login
                                Console.Write("Please enter password: ");

                                ConsoleKey key;
                                do
                                {
                                    var keyInfo = Console.ReadKey(intercept: true);
                                    key = keyInfo.Key;

                                    if (key == ConsoleKey.Backspace && pass.Length > 0)
                                    {
                                        Console.Write("\b \b");
                                        pass = pass[0..^1];
                                    }
                                    else if (!char.IsControl(keyInfo.KeyChar))
                                    {
                                        Console.Write('*');
                                        pass += keyInfo.KeyChar;
                                    }
                                } while (key != ConsoleKey.Enter);

                                // Retrieve the hashed password from the CSV file for the user
                                string userCsvPath = Path.Combine(workDir, "user.csv");
                                string storedHash = "";
                                if (File.Exists(userCsvPath))
                                {
                                    using (var reader = new StreamReader(userCsvPath))
                                    {
                                        using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
                                        {
                                            while (csv.Read())
                                            {
                                                var record = csv.GetRecord<User>();
                                                if (record.UserName == userName)
                                                {
                                                    storedHash = record.PassWord;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(storedHash) && VerifyPassword(pass, storedHash))
                                {
                                    Console.WriteLine("\nLogin successful.");
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid username or password.");
                                }
                                i++; // Skip the next argument
                            }
                            else
                            {
                                Console.WriteLine("Error: Expected format: --login <username>");
                            }
                            break;

                        case "--list":
                            Console.WriteLine("list");
                            break;

                        default:
                            Console.WriteLine($"Unknown command-line argument: {args[i]}");
                            break;
                    }
                }
            }
        }

        static void RegisterUser(string userName, string email, string masterPass, string? firstName, string? lastName, string workDir)
        {
            // Hashing the password before saving it
            string hashedPassword = HashPassword(masterPass);

            // Create a User object
            var user = new User
            {
                UserName = userName,
                PassWord = hashedPassword,
                Email = email,
                FirstName = firstName, // Please note there seems to be a typo here, it should probably be FirstName
                LastName = lastName
            };

            // Ensure the directory for the CSV file exists
            Directory.CreateDirectory(workDir);

            // Define the path to the user CSV file
            string userCsvPath = Path.Combine(workDir, "user.csv");

            // Write the user details to the CSV file
            using (var writer = new StreamWriter(userCsvPath, append: true))
            using (var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Ensure that the record will be written on a new line
                if (new FileInfo(userCsvPath).Length > 0)
                {
                    writer.WriteLine();
                }
                csv.WriteRecord(user);
            }
        }

        static string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                // ComputeHash - returns byte array
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));

                // Convert byte array to a string
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        static bool VerifyPassword(string inputPassword, string storedHash)
        {
            string hashedInputPassword = HashPassword(inputPassword);
            return hashedInputPassword == storedHash;
        }
    }
}
