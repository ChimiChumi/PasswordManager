using CsvHelper;
using CsvHelper.Configuration;
using PasswordManager.Models;
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
                    if (record.Email == email)
                    {
                        return true; // Email is already registered.
                    }
                }
            }

            return false; // Email is not registered.
        }

        public void FileWrite(User user)
        {
            Directory.CreateDirectory(workDir);
            string userCsvPath = Path.Combine(workDir, "user.csv");

            if (IsEmailAlreadyRegistered(user.Email))
            {
                Console.WriteLine($"Error: User with email '{user.Email}' already exists.");
                return;
            }

            using (var writer = new StreamWriter(userCsvPath, append: true))
            using (var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                if (new FileInfo(userCsvPath).Length > 0)
                {
                    writer.WriteLine();
                }
                csv.WriteRecord(user);
                Console.WriteLine($"User '{user.UserName}' registered successfully.");
            }
        }

        public void FileWrite(VaultEntry vault)
        {
            Directory.CreateDirectory(workDir);

            string vaultCsvPath = Path.Combine(workDir, "vault.csv");

            // Write the user details to the CSV file
            using (var writer = new StreamWriter(vaultCsvPath, append: true))
            using (var csv = new CsvWriter(writer, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
            {
                // Ensure that the record will be written on a new line
                if (new FileInfo(vaultCsvPath).Length > 0)
                {
                    writer.WriteLine();
                }
                csv.WriteRecord(vault);
            }
        }
    }
}
