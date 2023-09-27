using CsvHelper;
using CsvHelper.Configuration;
using PasswordManager.Models;
using System.Diagnostics;
using System.Globalization;

namespace PasswordManager
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

        public bool IsEmailAlreadyRegistered(string email)
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
                        return true; // Email is already registered.
                    }
                }
            }

            return false;
        }

        public void FileWrite(User user)
        {
            Directory.CreateDirectory(workDir);
            string userCsvPath = Path.Combine(workDir, "user.csv");

            if (IsEmailAlreadyRegistered(user.Email))
            {
                Console.WriteLine($"\n\nError: User with email '{user.Email}' already exists.");
                return;
            }

            using (StreamWriter writer = new(userCsvPath, append: true))
            {
                if (writer.BaseStream.Length == 0)
                {
                    CsvConfiguration config = new(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };
                    using CsvWriter csv = new(writer, config);
                    csv.WriteRecords(new User[] { user });
                }
                else
                {
                    CsvConfiguration config = new(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false
                    };
                    using CsvWriter csv = new(writer, config);
                    csv.WriteRecords(new User[] { user });
                } 
            }
        }

        public void FileWrite(Vault vault)
        {
            Directory.CreateDirectory(workDir);
            string userCsvPath = Path.Combine(workDir, "vault.csv");

            if (IsEmailAlreadyRegistered(vault.WebSite))
            {
                Console.WriteLine($"Error: Secret with this website already exsits: '{vault.WebSite}'.");
                return;
            }

            using (StreamWriter writer = new(userCsvPath, append: true))
            {
                if (writer.BaseStream.Length == 0)
                {
                    CsvConfiguration config = new(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = true
                    };
                    using CsvWriter csv = new(writer, config);
                    csv.WriteRecords(new Vault[] { vault });
                }
                else
                {
                    CsvConfiguration config = new(CultureInfo.InvariantCulture)
                    {
                        HasHeaderRecord = false
                    };
                    using CsvWriter csv = new(writer, config);
                    csv.WriteRecords(new Vault[] { vault });
                }
            }

            Console.WriteLine("\nSecret added to your personal vault!");
        }
    }
}
