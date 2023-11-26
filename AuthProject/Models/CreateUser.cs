using System.ComponentModel.DataAnnotations;

namespace AuthProject.Models
{
    public class CreateUser
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "Invalid format of field {0}")]
        public string Email { get; set; } = String.Empty;
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} should be between {2} and {1} chars", MinimumLength = 8)]
        public string Password { get; set; } = String.Empty;
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The field {0} is required")]
        [Compare("Password", ErrorMessage = "The password must match.")]
        public string RePassword { get; set; } = String.Empty;
    }
    public class UserLogin
    {
        [Required(ErrorMessage = "The field {0} is required")]
        [EmailAddress(ErrorMessage = "Invalid format of field {0}")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field {0} should be between {2} and {1} chars", MinimumLength = 6)]
        public string Password { get; set; }
    }
}
