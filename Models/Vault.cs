using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

namespace PasswordManager.Models
{
    public class Vault
    {
        [Name("user_id")]
        public string UserId { get; set; }

        [Name("username")]
        public string UserName { get; set; }

        [Name("website")]
        public string WebSite { get; set; } = string.Empty;

        [Name("password")]
        public string PassWord { get; set; }
    }
}