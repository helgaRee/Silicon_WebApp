using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels.Models;

public class AccountAddressInfo
{
    [Required]
    [Display(Name = "Address line 1", Prompt = "Enter your first address line")]
    public string AddressLine_1 { get; set; } = null!;


    [Display(Name = "Address line 2 (optional)", Prompt = "Enter your second address line")]
    public string? AddressLine_2 { get; set; }


    [Required]
    [Display(Name = "Postal Code", Prompt = "Enter your postal code")]
    public string PostalCode { get; set; } = null!;


    [Required]
    [Display(Name = "City", Prompt = "City")]
    public string City { get; set; } = null!;
}
