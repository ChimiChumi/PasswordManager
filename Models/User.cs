using CsvHelper.Configuration.Attributes;

namespace PasswordManager.Models
{
    public class User
    {
        [Name("username")]
        public string UserName { get; set; } = null!;

        [Name("email")]
        public string Email { get; set; } = null!;

        [Name("password")]
        public string PassWord { get; set; } = null!;

        [Name("firstname")]
        public string? FirstName { get; set; } = null!;

        [Name("lastname")]
        public string? LastName { get; set; } = null!;
    }
}
