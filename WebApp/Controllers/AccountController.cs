using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApp.ViewModels;
using WebApp.ViewModels.Models;

namespace WebApp.Controllers;

[Authorize]
//tar emot en UserManager<UserEntity> och en DataContext genom dependency injection.
public class AccountController(UserManager<UserEntity> userManager, DataContext context) : Controller
{
    //Initialiserar privata fält för UserManager och DataContext.
    public readonly UserManager<UserEntity> _userManager = userManager;
    private readonly DataContext _context = context;










    //En GET-metod som returnerar en vy för att visa användardetaljer.
    //Den skapar en ny AccountDetailsViewModel och skickar den till vyn.
    [HttpGet]
    public async Task<IActionResult> Details()
    {
        try
        {
            //Hämtar användarens ID från claims (identitetsinformation).
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            //Hämtar användaren från databasen inklusive dess adressinformation.
            var user = await _context.Users.Include(i => i.AddressEntity).FirstOrDefaultAsync(x => x.Id == nameIdentifier);

            if (user != null && user.AddressEntity != null)
            {

                //Skapa viewModeln för att kunna Hämta informationen och populera informationer till den.
                var viewModel = new AccountDetailsViewModel
                {
                    BasicInfo = new AccountBasicInfo
                    {
                        FirstName = user!.FirstName,
                        LastName = user.LastName,
                        Email = user.Email!,
                        PhoneNumber = user.PhoneNumber,
                        Biography = user.Biography,
                    },

                    AddressInfo = new AccountAddressInfo
                    {
                        AddressLine_1 = user.AddressEntity!.AddressLine_1,
                        AddressLine_2 = user.AddressEntity.AddressLine_2,
                        PostalCode = user.AddressEntity.PostalCode,
                        City = user.AddressEntity.City,
                    },
                };
                return View(viewModel);
            }
            return RedirectToAction("Details", "Account");
        }
        catch (Exception ex)
        {
            TempData["StatusMessage"] = "The request failed.";
        }
        return RedirectToAction("Details", "Account");

    }


    //En POST-metod för att uppdatera grundläggande användarinformation.
    [HttpPost]
    public async Task<IActionResult> UpdateBasicInfo(AccountDetailsViewModel model)
    {
        //Kontrollerar om den mottagna modellen model.BasicInfo är giltig.
        if (TryValidateModel(model.BasicInfo!))
        {
            //Hämtar aktuell inloggad användare med hjälp av UserManager.
            var user = await _userManager.GetUserAsync(User);

            //Uppdaterar användarens grundläggande information med samma värden som tidigare (ingen faktisk uppdatering).
            if (user != null)
            {


                user.FirstName = user.FirstName;
                user.LastName = user.LastName;
                user.Email = user.Email;
                user.PhoneNumber = user.PhoneNumber;
                user.UserName = user.Email;
                user.Biography = user.Biography;

                //Uppdaterar användarinformationen asynkront med UserManager.
                var result = _userManager.UpdateAsync(user);


                //Sätter ett statusmeddelande beroende på om uppdateringen lyckades eller misslyckades.
                if (result.IsCompletedSuccessfully)
                {
                    TempData["StatusMessage"] = "The User was updated.";
                }
                else
                {
                    TempData["StatusMessage"] = "Update failed.";
                }
            }
        }
        else
        {
            // Sätter ett statusmeddelande i TempData om modellen inte är giltig.
            TempData["StatusMessage"] = "Unable to save Basic information.";
        }
        //Oavsett om uppdateringen var framgångsrik eller inte,
        //omdirigeras användaren tillbaka till kontodetaljerna med ett statusmeddelande.
        return RedirectToAction("Details", "Account");
    }





    [HttpPost]
    public async Task<IActionResult> UpdateAddressInfo(AccountDetailsViewModel model)
    {
        if (TryValidateModel(model.AddressInfo!))
        {
            //Hämtar användarens ID från claims (identitetsinformation).
            var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

            //Hämtar användaren från databasen inklusive dess adressinformation.
            var user = await _context.Users.Include(i => i.AddressEntity).FirstOrDefaultAsync(x => x.Id == nameIdentifier);


            //Om användaren finns:
            if (user != null)
            {
                try
                {
                    //1.Kontrollerar om användaren redan har en adress(user.AddressEntity).
                    if (user.AddressEntity != null)
                    {
                        //2.Uppdaterar adressinformationen om det finns en befintlig adress.
                        user.AddressEntity.AddressLine_1 = model.AddressInfo!.AddressLine_1;
                        user.AddressEntity.AddressLine_2 = model.AddressInfo!.AddressLine_2;
                        user.AddressEntity.PostalCode = model.AddressInfo!.PostalCode;
                        user.AddressEntity.City = model.AddressInfo!.City;

                    }
                    else
                    {
                        //3.Skapar en ny adress om ingen adress finns.
                        user.AddressEntity = new AddressEntity
                        {
                            AddressLine_1 = model.AddressInfo!.AddressLine_1,
                            AddressLine_2 = model.AddressInfo!.AddressLine_2,
                            PostalCode = model.AddressInfo!.PostalCode,
                            City = model.AddressInfo.City,
                        };
                    }

                    //Uppdaterar användaruppgifterna i databasen.
                    //Spara ändringarna asynkront.
                    _context.Update(user);
                    await _context.SaveChangesAsync();

                    TempData["StatusMessage"] = "Updated Address information succesfully.";

                }
                catch (Exception ex)
                {
                    TempData["StatusMessage"] = "Unable to save address information.";
                }
            }
        }
        else
        {
            TempData["StatusMessage"] = "Unable to save Address information.";
        }
        return RedirectToAction("Details", "Account");
    }


}
