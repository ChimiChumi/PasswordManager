using CsvHelper.Configuration.Attributes;

namespace PasswordManager.Models
{
    public class User
    {
        [Name("username")]
        public string UserName { get; set; }

        [Name("email")]
        public string Email { get; set; }

        [Name("password")]
        public string PassWord { get; set; }

        [Name("firstname")]
        public string? FirstName { get; set; } = string.Empty;

        [Name("lastname")]
        public string? LastName { get; set; } = string.Empty;
    }
}
