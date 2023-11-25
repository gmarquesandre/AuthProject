using System.ComponentModel.DataAnnotations;

namespace AuthProject.Models
{
    public class CreateUserDto
    {        
        [Required]
        public string Email { get; set; } = String.Empty;
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; } = String.Empty;
        [DataType(DataType.Password)]
        [Required]
        [Compare("Password")]
        public string RePassword { get; set; } = String.Empty;
    }
}
