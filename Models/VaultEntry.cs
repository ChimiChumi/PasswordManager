using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace PasswordManager.Models
{
    public class VaultEntry
    {
        [Name("user_id")]
        public string UserId { get; set; }

        [Name("username")]
        public string UserName { get; set; }

        [Name("password")]
        public string PassWord { get; set; }

        [Name("website")]
        public string Website { get; set; } = string.Empty;
    }
}