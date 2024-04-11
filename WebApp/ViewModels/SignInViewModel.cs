using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;

public class SignInViewModel
{
    [Required]
    [Display(Name = "Email Addres", Prompt = "Enter your email address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;



    [Required]
    [Display(Name = "Password", Prompt = "Enter your password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;




    [Display(Name = "Remember me", Prompt = "Remember me")]
    public bool RememberMe { get; set; }
}
