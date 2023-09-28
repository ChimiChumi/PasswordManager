#pragma warning disable
using CsvHelper;
using PasswordManager.Models;
using System.Globalization;

namespace PasswordManager.Classes
{
    internal class Utils
    {
        EncryptedType encryptType = new EncryptedType();
        public void badArgument(string command)
        {
            Console.WriteLine($"\nError: Unexpected argument following '{command}'\n");
        }

        public string GetUserInput(string fieldName)
        {
            string input;
            while (true) // Loop until valid input is received
            {
                Console.Write($"        {fieldName}: ");
                input = Console.ReadLine();

                if (!string.IsNullOrEmpty(input))
                    break;

                int currentLineCursor = Console.CursorTop - 1; // Get the previous line number
                Console.SetCursorPosition(fieldName.Length + 10, currentLineCursor); // Move cursor to the end of the input
                Console.Write(" <-- This field cannot be empty!\n\n     ####### Try again! #######");
                Console.WriteLine();
            }

            return input;
        }


        public string GetPasswordInput(string fieldName)
        {
            string pwd = string.Empty;
            while (true) // Loop until valid input is received
            {
                Console.Write($"    {fieldName}: ");

                // mask typed password
                ConsoleKey key;
                do
                {
                    var keyInfo = Console.ReadKey(intercept: true);
                    key = keyInfo.Key;

                    if (key == ConsoleKey.Backspace && pwd.Length > 0)
                    {
                        Console.Write("\b \b");
                        pwd = pwd[0..^1];
                    }
                    else if (!char.IsControl(keyInfo.KeyChar))
                    {
                        Console.Write('*');
                        pwd += keyInfo.KeyChar;
                    }
                } while (key != ConsoleKey.Enter);

                Console.WriteLine();

                if (!string.IsNullOrEmpty(pwd))
                    break;

                int currentLineCursor = Console.CursorTop - 1;
                Console.SetCursorPosition(fieldName.Length + 10, currentLineCursor); // Move cursor to the end of the input
                Console.Write(" <-- This field cannot be empty!\n\n   ~~~~~~~~~ Try again! ~~~~~~~~~");
                Console.WriteLine();
                pwd = string.Empty; // Clear the password string for the next input
            }
            return pwd;
        }

        public string GetStoredPassword(string userName, string filePath)
        {
            if (File.Exists(filePath))
            {
                using (var reader = new StreamReader(filePath))
                {
                    using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)))
                    {
                        while (csv.Read())
                        {
                            var record = csv.GetRecord<User>();
                            if (record?.UserName == userName)
                            {
                                return record.PassWord;
                            }
                        }
                    }
                }
            }

            return string.Empty;
        }

        public bool ValidateUser(string storedPassword, string inputPassword, string userName)
        {
            return !string.IsNullOrEmpty(storedPassword) && encryptType.Decrypt(storedPassword, userName) == inputPassword;
        }
    }
}
