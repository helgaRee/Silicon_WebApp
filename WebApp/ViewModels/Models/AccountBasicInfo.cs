using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Models;

public class AccountBasicInfo
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



    [Display(Name = "Phone (optional)", Prompt = "Enter your phone number")]
    [DataType(DataType.PhoneNumber)]
    public string? PhoneNumber { get; set; }


    [Display(Name = "Bio (optional)", Prompt = "Enter your biography")]
    [DataType(DataType.MultilineText)]
    public string? Biography { get; set; }
}
