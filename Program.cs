using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using CsvHelper;
using PasswordManager.Models;

namespace PasswordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string configFilePath = "config.txt";

            string workDir = File.Exists(configFilePath) ? File.ReadAllText(configFilePath) : "..\\..\\PasswordManager\\database\\";

            // Initialize FileHandler
            FileHandler fileHandler = new FileHandler(workDir);
            EncryptedType encryptedType = new EncryptedType();

            string userName;
            string email;
            string masterPass;
            string? firstName = null;
            string? lastName = null;

            bool loggedIn = false; // Track login status
            string authUser = ""; // Track who is logged in

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
                                fileHandler.SetWorkDir(workDir, configFilePath);
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
                                //Console.WriteLine(masterPass);

                                if (i + 5 < args.Length && !args[i + 4].StartsWith("--") && !args[i + 5].StartsWith("--"))
                                {
                                    firstName = args[i + 4];
                                    lastName = args[i + 5];
                                    i += 2;
                                }
                                else if (i + 4 < args.Length && !args[i + 4].StartsWith("--"))
                                {
                                    firstName = args[i + 4];
                                    i++;
                                }

                                // Create a User object
                                var user = new User
                                {
                                    UserName = userName,
                                    Email = email,
                                    PassWord = encryptedType.Encrypt(masterPass, userName),
                                    FirstName = firstName,
                                    LastName = lastName
                                };

                                string encryptedPassword = encryptedType.Encrypt(masterPass, userName);
                                Console.WriteLine("Encrypted Password: " + encryptedPassword);
                                string decryptedPassword = encryptedType.Decrypt(encryptedPassword, userName);
                                Console.WriteLine("Decrypted Password: " + decryptedPassword);

                                // Use FileHandler to write the user details to the CSV file
                                fileHandler.FileWrite(user);

                                i += 3; // Skip to the next valid argument
                            }
                            else
                            {
                                Console.WriteLine("Argument expects the following: --register <username> <email> <master-password> [firstname] [lastname]");
                            }
                            break;

                        case "--login":
                            if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                            {
                                Console.WriteLine("Error: Unexpected argument following --login. Expected format: --login");
                            }
                            else
                            {
                                Console.Write("Please enter username: ");
                                userName = Console.ReadLine();
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
                                string pwd = "";
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
                                                    pwd = record.PassWord;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(pwd) && (encryptedType.Decrypt(pwd, userName) == pass) )
                                {
                                    loggedIn = true; // Set the logged-in flag to true
                                    authUser = userName;
                                    Console.WriteLine("Jelszo: " + encryptedType.Decrypt(pwd, authUser));
                                    Console.WriteLine("\n\nSuccessful Authentication!\nPossible actions: --add, --list, --delete. Use 'exit' to log out:");
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid username or password.");
                                }
                                i++; // Skip the next argument
                            }
                            break;

                        case "--list":
                            if (loggedIn) // Check if the user is logged in
                            {
                                // Handle listing functionality here
                                Console.WriteLine("List command executed.");
                            }
                            else
                            {
                                Console.WriteLine("Error: You need to log in first.");
                            }
                            break;

                        case "--add":
                            if (loggedIn) // Check if the user is logged in
                            {
                                if (i + 3 < args.Length && !args[i + 1].StartsWith("--") && !args[i + 2].StartsWith("--") && !args[i + 3].StartsWith("--"))
                                {
                                    var vault = new VaultEntry
                                    {
                                        UserId = authUser,
                                        UserName = args[i + 1],
                                        WebSite = args[i + 2],
                                        PassWord = encryptedType.Encrypt(args[i + 3], authUser)
                                    };

                                    // Use FileHandler to write the user details to the CSV file
                                    fileHandler.FileWrite(vault);

                                    i += 3; // Skip to the next valid argument
                                }
                                else
                                {
                                    Console.WriteLine("Expected argument looks the following: --add <website-username> <website> <secret>");
                                }
                                break;
                            }
                            else
                            {
                                Console.WriteLine("Error: You need to log in first.");
                            }
                            break;

                        case "--delete":
                            if (loggedIn) // Check if the user is logged in
                            {
                                // Handle adding functionality here
                                Console.WriteLine("Delete command executed.");
                            }
                            else
                            {
                                Console.WriteLine("Error: You need to log in first.");
                            }
                            break;

                        default:
                            Console.WriteLine($"Unknown command-line argument: {args[i]}");
                            break;
                    }
                }
            }

            while (loggedIn)
            {
                string input = Console.ReadLine();
                if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    loggedIn = false; // Log out and exit the loop
                    authUser = "";
                }
                else
                {
                    // Handle additional commands within the session
                    Console.WriteLine("You entered: " + input);
                }
            }
        }
    }
}
