using System.ComponentModel.DataAnnotations;

namespace WebApp.Validators;

public class CheckBoxRequired : ValidationAttribute
{
    //Använder ValidationAttributes metod 'IsValid' som kontrollerar om det inskickade värdet är true.
    public override bool IsValid(object? value) => value is bool b && b;

}
