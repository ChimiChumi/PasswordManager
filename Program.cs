#pragma warning disable
using CsvHelper.Configuration;
using CsvHelper;
using PasswordManager.Classes;
using PasswordManager.Models;
using System.Globalization;
using System.Threading;

namespace PasswordManager
{
    class Program
    {
        static void Main(string[] args)
        {
            string userName;
            string email;
            string firstName;
            string lastName;

            bool loggedIn = false;
            string authUser = "";

            string configFile = "config.txt";
            string workDir = File.Exists(configFile) ? File.ReadAllText(configFile) : "..\\..\\PasswordManager\\database\\";

            FileHandler fileHandler = new FileHandler(workDir);
            EncryptedType encryptedType = new EncryptedType();
            Utils utils = new Utils();

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
                                fileHandler.SetWorkDir(workDir, configFile);
                                i++;
                            }
                            else
                            {
                                Console.WriteLine("\nError: Missing or invalid path after --workdir.\n");
                                return;
                            }
                            break;

                        case "--register":
                            if (i + 1 < args.Length)
                            {
                                utils.badArgument("--register");
                                return;
                            }
                            else
                            {
                                Console.WriteLine("\n*******************### HELLO USER ! ###*******************\nTo create an account, please fill out the fields below.\n");

                                firstName = utils.GetUserInput(" First Name");
                                lastName = utils.GetUserInput("  Last Name");
                                userName = utils.GetUserInput("   Username");
                                email = utils.GetUserInput("      Email");
                                if (fileHandler.emailPresent(email) == true) break;
                                string pwd = utils.GetPasswordInput("Master Password");
                                string confirmPassword = utils.GetPasswordInput("Repeat Password");

                                if (pwd != confirmPassword)
                                {
                                    Console.WriteLine("\n\nThe entered passwords do not match!\n");
                                    break;
                                }

                                var user = new User
                                {
                                    UserName = userName,
                                    Email = email,
                                    PassWord = encryptedType.Encrypt(pwd, userName),
                                    FirstName = firstName,
                                    LastName = lastName
                                };

                                fileHandler.FileWrite(user);
                                break;
                            }

                        case "--login":

                            if (i + 1 < args.Length)
                            {
                                utils.badArgument("--login");
                                return;
                            }

                            Console.WriteLine("\nPlease enter your master credentials to access your vault:\n");

                            while (true)
                            {
                                userName = utils.GetUserInput("Username");
                                string password = utils.GetPasswordInput("    Password");
                                string userCsvPath = Path.Combine(workDir, "user.csv");
                                string storedPassword = utils.GetStoredPassword(userName, userCsvPath);

                                if (utils.ValidateUser(storedPassword, password, userName))
                                {
                                    loggedIn = true; // user logged in
                                    authUser = userName;
                                    Console.WriteLine("\n  ~ ~ ~ ~ ~ ~ Successful Authentication! ~ ~ ~ ~ ~ ~\n");

                                    Thread.Sleep(500);
                                    Console.WriteLine("          ╔═══════════════════════════════╗");
                                    Thread.Sleep(100);
                                    Console.WriteLine("          ║         WELCOME BACK !        ║");
                                    Thread.Sleep(100);
                                    Console.WriteLine("          ╚═══════════════════════════════╝\n\nPossible commands: 'add', 'list', 'delete'. Use 'exit' to log out:");

                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("\n\nInvalid username or password! Please try again:\n");
                                    // Allow the user to try again; continue to the next iteration of the loop
                                }
                            }
                            break;

                        default:
                            Console.WriteLine($"\nUnknown command-line argument: '{args[i]}'\n");
                            break;
                    }
                }
            }
            else
            {
                Console.WriteLine("\n   No arguments given!\n");
            }

            while (loggedIn)
            {
                Console.Write(">> ");
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
                            var websites = fileHandler.GetWebsitesForUser(authUser);

                            if (websites.Count == 0)
                            {
                                Console.WriteLine("\nYou have no secrets stored!");
                                break;
                            }

                            Console.WriteLine("\nStored Websites and Passwords:\n" +
                                $"----------------------------------------------------------------------");
                            foreach (var website in websites)
                            {
                                // Find the specific record for the website
                                string vaultCsvPath = Path.Combine(workDir, "vault.csv");
                                using (var reader = new StreamReader(vaultCsvPath))
                                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                                {
                                    while (csv.Read())
                                    {
                                        var record = csv.GetRecord<Vault>();
                                        if (record?.UserId == authUser && record.WebSite == website)
                                        {
                                            // Decrypt the password and print the website, username, and decrypted password
                                            string decryptedPassword = encryptedType.Decrypt(record.PassWord, authUser);
                                            Console.WriteLine($"| Website: '{record.WebSite}' | Username: '{record.UserName}' | Password: '{decryptedPassword}'\n" +
                                                $"----------------------------------------------------------------------");
                                            break;
                                        }
                                    }
                                }
                            }
                            break;

                        case "add":
                            Console.WriteLine("\nTo save a new secret, please fill out the required fields below!");
                            while (true)
                            {
                                string website = utils.GetUserInput(" Website Address");
                                if (fileHandler.websitePresent(website) == true) continue;
                                string webUserName = utils.GetUserInput("Website Username");
                                string webPwd = utils.GetPasswordInput("    Website Password");

                                var vault = new Vault
                                {
                                    UserId = authUser,
                                    UserName = webUserName,
                                    WebSite = website,
                                    PassWord = encryptedType.Encrypt(webPwd, authUser)
                                };

                                fileHandler.FileWrite(vault);
                                break;
                            }
                            break;

                        case "delete":
                            fileHandler.DeleteSecret(authUser);
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
