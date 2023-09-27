using CsvHelper;
using PasswordManager.Models;
using System.Globalization;

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
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("\nError: Missing or invalid path after --workdir.\n");
                                break;
                            }
                            break;

                        case "--register":
                            if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                            {
                                Console.WriteLine("\nError: Unexpected argument following --login.\n");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("\n*******************### WELCOME! ###*******************\nTo create an account, please fill out the fields below.\n");

                                Console.Write("        First Name: ");
                                firstName = Console.ReadLine();

                                if (firstName == "")
                                {
                                    Console.WriteLine("\nThis field cannot be empty!\n");
                                    break;
                                }

                                Console.Write("         Last Name: ");
                                userName = Console.ReadLine();

                                if (userName == "")
                                {
                                    Console.WriteLine("\nThis field cannot be empty!\n");
                                    break;
                                }

                                Console.Write("          Username: ");
                                userName = Console.ReadLine();

                                if (userName == "")
                                {
                                    Console.WriteLine("\nThis field cannot be empty!\n");
                                    break;
                                }

                                Console.Write("             Email: ");
                                email = Console.ReadLine()!;

                                if (email == "")
                                {
                                    Console.WriteLine("\nThis field cannot be empty!\n");
                                    break;
                                }

                                Console.Write("   Master Password: ");
                                string pwd1 = string.Empty;

                                ConsoleKey key1;
                                do
                                {
                                    var keyInfo = Console.ReadKey(intercept: true);
                                    key1 = keyInfo.Key;

                                    if (key1 == ConsoleKey.Backspace && pwd1.Length > 0)
                                    {
                                        Console.Write("\b \b");
                                        pwd1 = pwd1[0..^1];
                                    }
                                    else if (!char.IsControl(keyInfo.KeyChar))
                                    {
                                        Console.Write('*');
                                        pwd1 += keyInfo.KeyChar;
                                    }
                                } while (key1 != ConsoleKey.Enter);

                                if (pwd1 == "")
                                {
                                    Console.WriteLine("Password field cannot be empty!");
                                    break;
                                }

                                Console.Write("\n   Repeat password: ");
                                string pwd2 = string.Empty;

                                ConsoleKey key2;
                                do
                                {
                                    var keyInfo = Console.ReadKey(intercept: true);
                                    key2 = keyInfo.Key;

                                    if (key2 == ConsoleKey.Backspace && pwd2.Length > 0)
                                    {
                                        Console.Write("\b \b");
                                        pwd2 = pwd2[0..^1];
                                    }
                                    else if (!char.IsControl(keyInfo.KeyChar))
                                    {
                                        Console.Write('*');
                                        pwd2 += keyInfo.KeyChar;
                                    }
                                } while (key2 != ConsoleKey.Enter);

                                if (pwd2 == "")
                                {
                                    Console.WriteLine("\nThis field cannot be empty!\n");
                                    break;
                                }

                                if(pwd1 != pwd2)
                                {
                                    Console.WriteLine("\n\nThe entered passwords do not match!\n");
                                    break;
                                }

                                var user = new User
                                {
                                    UserName = userName,
                                    Email = email,
                                    PassWord = encryptedType.Encrypt(pwd2, userName),
                                    FirstName = firstName,
                                    LastName = lastName
                                };

                                fileHandler.FileWrite(user);
                                Console.WriteLine("\n\nRegistration Successful!\nTo log in, use '--login'\n");

                                i += 3; // Skip to the next valid argument
                            }
                            break;

                        case "--login":
                            if (i + 1 < args.Length && !args[i + 1].StartsWith("--"))
                            {
                                Console.WriteLine("Error: Unexpected argument following --login. Expected format: --login");
                                break;
                            }
                            else
                            {
                                Console.WriteLine("\nPlease enter your master credentials to access your vault!\n");
                                Console.Write("Username: ");
                                userName = Console.ReadLine();
                                string pass = string.Empty;

                                Console.Write("Password: ");

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
                                                if (record?.UserName == userName)
                                                {
                                                    pwd = record.PassWord;
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(pwd) && (encryptedType.Decrypt(pwd, userName) == pass))
                                {
                                    loggedIn = true; // Set the logged-in flag to true
                                    authUser = userName;
                                    Console.WriteLine("\n\nSuccessful Authentication!\nPossible actions: --add, --list, --delete. Use 'exit' to log out:");
                                }
                                else
                                {
                                    Console.WriteLine("\nInvalid username or password.");
                                    break;
                                }
                                i++; // Skip the next argument
                            }
                            break;

                        default:
                            Console.WriteLine($"Unknown command-line argument: {args[i]}\n");
                            break;
                    }
                }
            }

            while (loggedIn)
            {
                string input = Console.ReadLine();
                if (input!.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    loggedIn = false; // Log out and exit the loop
                    authUser = "";
                }
                else
                {
                    switch (input)
                    {
                        case "list":
                            break;

                        case "add":
                            Console.WriteLine("\nTo save a new secret, please fill out the required fields below!");
                            Console.Write("Website: ");
                            string website = Console.ReadLine()!;

                            if (website == "")
                            {
                                Console.WriteLine("\nWebsite field cannot be empty!\n");
                                break;
                            }

                            Console.Write("Username on website: ");
                            string webUserName = Console.ReadLine()!;

                            if (webUserName == "")
                            {
                                Console.WriteLine("\nUsername field cannot be empty!\n");
                                break;
                            }

                            Console.Write("Password on website: ");
                            string webPwd = Console.ReadLine()!;

                            if (webPwd == "")
                            {
                                Console.WriteLine("\nPassword field cannot be empty!\n");
                                break;
                            }
                            var vault = new Vault
                            {
                                UserId = authUser,
                                UserName = webUserName,
                                WebSite = website,
                                PassWord = encryptedType.Encrypt(webPwd, authUser)
                            };

                            fileHandler.FileWrite(vault);
                            break;

                        case "delete":
                            break;

                        default:
                            Console.WriteLine("Unknown command: " + input);
                            break;
                    }
                }
            }
        }
    }
}
