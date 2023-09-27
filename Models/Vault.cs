using CsvHelper.Configuration.Attributes;

namespace PasswordManager.Models
{
    public class Vault
    {
        [Name("user_id")]
        public string UserId { get; set; } = null!;

        [Name("username")]
        public string UserName { get; set; } = null!;

        [Name("website")]
        public string WebSite { get; set; } = string.Empty;

        [Name("password")]
        public string PassWord { get; set; } = null!;
    }
}