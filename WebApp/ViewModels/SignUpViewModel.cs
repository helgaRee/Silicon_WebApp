using System.ComponentModel.DataAnnotations;
using WebApp.Validators;

namespace WebApp.ViewModels;

public class SignUpViewModel
{
    //är Automatiskt Text som Datatyp
    [Required]
    [Display(Name = "First Name", Prompt = "Enter your first name")]
    public string FirstName { get; set; } = null!;


    //är Automatiskt Text som Datatyp
    [Required]
    [Display(Name = "Last name", Prompt = "Enter your last name")]
    public string LastName { get; set; } = null!;



    [Required]
    [Display(Name = "Email Addres", Prompt = "Enter your email address")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; } = null!;



    [Required]
    [Display(Name = "Password", Prompt = "Enter your password")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = null!;



    [Required]
    [Display(Name = "Confirm Password", Prompt = "Confirm your password")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Passwords doesn't match")]
    public string ConfirmPassword { get; set; } = null!;



    [CheckBoxRequired(ErrorMessage = "You have to confirm the Terms and Conditions")]
    [Display(Name = "Terms and conditions", Prompt = "Terms and conditions")]
    public bool TermsAndConditions { get; set; }
}
