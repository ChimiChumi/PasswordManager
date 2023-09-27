#pragma warning disable
using CsvHelper.Configuration.Attributes;

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