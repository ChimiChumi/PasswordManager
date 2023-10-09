using CsvHelper;
using CsvHelper.Configuration;
using PasswordManager.Models;
using System.Globalization;

namespace PasswordManager.Classes
{
    public class FileHandler
    {
        private string workDir;

        public FileHandler(string workDir) => this.workDir = workDir;

        public void SetWorkDir(string workDir, string configFilePath)
        {
            this.workDir = workDir;
            Directory.CreateDirectory(workDir);
            File.WriteAllText(configFilePath, workDir);
            Console.WriteLine($"\nWorking directory set to: {workDir}\n");
        }

        public bool emailPresent(string email)
        {
            string userCsvPath = Path.Combine(workDir, "user.csv");

            if (!File.Exists(userCsvPath))
            {
                return false; // CSV doesn't exist, so email can't be registered.
            }

            using (var reader = new StreamReader(userCsvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                while (csv.Read())
                {
                    var record = csv.GetRecord<User>();
                    if (record?.Email == email)
                    {
                        Console.WriteLine($"\nError: User with email '{record.Email}' already exists!\n\n");
                        return true;
                    }
                }
            }

            return false;
        }

        public bool websitePresent(string website)
        {
            string userCsvPath = Path.Combine(workDir, "vault.csv");

            if (!File.Exists(userCsvPath))
            {
                return false; // CSV doesn't exist, so email can't be registered.
            }

            using (var reader = new StreamReader(userCsvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                while (csv.Read())
                {
                    var record = csv.GetRecord<Vault>();
                    if (record?.WebSite == website)
                    {
                        Console.WriteLine($"\nError: Secret with website '{record.WebSite}' already exists!\n\nTry adding a new one:\n");
                        return true;
                    }
                }
            }

            return false;
        }

        public void FileWrite(User user)
        {
            Directory.CreateDirectory(workDir);
            string userCsvPath = Path.Combine(workDir, "user.csv");

            if (emailPresent(user.Email))
            {
                Console.WriteLine($"\n\nError: User with email '{user.Email}' already exists!\n");
                return;
            }

            using StreamWriter writer = new(userCsvPath, append: true);
            bool writeHeader = writer.BaseStream.Length == 0;

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = writeHeader
            };

            using CsvWriter csv = new(writer, config);
            csv.WriteRecords(new User[] { user });
            Console.WriteLine("\n\n  ~ ~ ~ Registration Successful! ~ ~ ~\n     (To log in, use '--login')\n");
        }

        public void FileWrite(Vault vault)
        {
            Directory.CreateDirectory(workDir);
            string userCsvPath = Path.Combine(workDir, "vault.csv");

            using StreamWriter writer = new(userCsvPath, append: true);
            bool writeHeader = writer.BaseStream.Length == 0;

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = writeHeader
            };

            using CsvWriter csv = new(writer, config);
            csv.WriteRecords(new Vault[] { vault });
            Console.WriteLine("\n\nSecret added to your personal vault!\n");
        }

        public List<string> GetWebsitesForUser(string userId)
        {
            string vaultCsvPath = Path.Combine(workDir, "vault.csv");
            List<string> websites = new List<string>();

            if (!File.Exists(vaultCsvPath))
            {
                return websites; // Return an empty list if the file doesn't exist
            }

            using (var reader = new StreamReader(vaultCsvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                while (csv.Read())
                {
                    var record = csv.GetRecord<Vault>();
                    if (record?.UserId == userId)
                    {
                        websites.Add(record.WebSite);
                    }
                }
            }

            return websites;
        }

        public List<Vault> GetSecretsForUser(string userId)
        {
            string vaultCsvPath = Path.Combine(workDir, "vault.csv");
            List<Vault> secrets = new List<Vault>();

            if (!File.Exists(vaultCsvPath))
            {
                return secrets; // Return an empty list if the file doesn't exist
            }

            using (var reader = new StreamReader(vaultCsvPath))
            using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                while (csv.Read())
                {
                    var record = csv.GetRecord<Vault>();
                    if (record?.UserId == userId)
                    {
                        secrets.Add(record);
                    }
                }
            }

            return secrets;
        }

        public void DeleteSecret(string userId)
        {
            string vaultCsvPath = Path.Combine(workDir, "vault.csv");
            List<Vault> secrets = new List<Vault>();

            if (!File.Exists(vaultCsvPath))
            {
                Console.WriteLine("No secrets found!");
                return;
            }
            while (true) // to stay in delete mode
            {
                using (var reader = new StreamReader(vaultCsvPath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    secrets = csv.GetRecords<Vault>().ToList();
                }

                var userSecrets = secrets.Where(s => s.UserId == userId).ToList();

                if (userSecrets.Count == 0)
                {
                    Console.WriteLine("\nYou have no secrets stored!");
                    return;
                }

                Console.WriteLine("\nAvailable websites:");
                Console.WriteLine("0. [RETURN]");

                for (int i = 0; i < userSecrets.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {userSecrets[i].WebSite}");
                }

                Console.Write("\nEnter the number of the secret to delete: ");
                if (int.TryParse(Console.ReadLine(), out int selectedIndex) && selectedIndex >= 0 && selectedIndex <= userSecrets.Count)
                {
                    if (selectedIndex == 0)
                    {
                        Console.WriteLine("\nReturned to the menu");
                        break;
                    }
                    string selectedWebsite = userSecrets[selectedIndex - 1].WebSite;
                    secrets.RemoveAll(s => s.UserId == userId && s.WebSite == selectedWebsite);

                    using StreamWriter writer = new(vaultCsvPath, append: false);
                    CsvConfiguration config = new(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };

                    using CsvWriter csv = new(writer, config);
                    csv.WriteRecords(secrets);

                    Console.WriteLine("\n~ ~ ~ Secret deleted successfully! ~ ~ ~");
                }
                else
                {
                    Console.WriteLine("Invalid selection!");
                }
            }
        }
    }
}
