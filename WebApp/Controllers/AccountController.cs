using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using WebApp.ViewModels;
using WebApp.ViewModels.Models;

namespace WebApp.Controllers;

[Authorize]
//tar emot en UserManager<UserEntity> och en DataContext genom dependency injection.
public class AccountController(UserManager<UserEntity> userManager, DataContext context, HttpClient httpClient) : Controller
{
    //Initialiserar privata fält för UserManager och DataContext.
    public readonly UserManager<UserEntity> _userManager = userManager;
    private readonly DataContext _context = context;
    private readonly HttpClient _httpClient = httpClient;


    //En GET-metod som returnerar en vy för att visa användardetaljer.
    //Den skapar en ny AccountDetailsViewModel och skickar den till vyn.
    //populera information för användaren

    public async Task<IActionResult> Details()
    {

        //Hämtar användarens ID från claims (identitetsinformation).
        var nameIdentifier = User.FindFirst(ClaimTypes.NameIdentifier)!.Value;

        //Hämtar användaren från databasen inklusive dess adressinformation.
        var user = await _context.Users.Include(i => i.AddressEntity).FirstOrDefaultAsync(x => x.Id == nameIdentifier);

        //Skapa viewModeln för att kunna Hämta informationen och populera informationer till den.



        var viewModel = new AccountDetailsViewModel
        {
            BasicInfo = new AccountBasicInfo
            {
                FirstName = user?.FirstName ?? string.Empty,
                LastName = user?.LastName ?? string.Empty,
                Email = user?.Email ?? string.Empty,
                PhoneNumber = user?.PhoneNumber ?? string.Empty,
                Biography = user?.Biography ?? string.Empty,
            },

            AddressInfo = new AccountAddressInfo
            {
                AddressLine_1 = user?.AddressEntity?.AddressLine_1 ?? string.Empty,
                AddressLine_2 = user?.AddressEntity?.AddressLine_2 ?? string.Empty,
                PostalCode = user?.AddressEntity?.PostalCode ?? string.Empty,
                City = user?.AddressEntity?.City ?? string.Empty,
            },
        };



        return View(viewModel);

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
                user.FirstName = model.BasicInfo!.FirstName;
                user.LastName = model.BasicInfo!.LastName;
                user.Email = model.BasicInfo!.Email;
                user.PhoneNumber = model.BasicInfo!.PhoneNumber;
                user.UserName = model.BasicInfo!.Email;
                user.Biography = model.BasicInfo!.Biography;

                //Uppdaterar användarinformationen asynkront med UserManager.
                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
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
                catch
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

    [HttpPost]
    public async Task<IActionResult> UploadProfileImage(IFormFile file)
    {
        //Hämtar usern
        var user = await _userManager.GetUserAsync(User);

        //kontroll om de är tomma
        if (user != null && file != null && file.Length != 0)
        {
            //skapa unikt filnamn - hämtar Id för user - unikt id för bilden - ändrar filändelsen
            var fileName = $"p_{user.Id}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            //skapa filvägen - spara in i katalogen ...
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/uploads/profiles", fileName);

            // ? - skapar en ny fil
            using var fs = new FileStream(filePath, FileMode.Create);
            //uppladdning av bild - "kopierar filen"
            await file.CopyToAsync(fs);

            //sätter det nya filnamnet till userna profilimage
            user.ProfileImage = fileName;
            //Uppdaterar
            await _userManager.UpdateAsync(user);
        }
        else
        {
            TempData["StatusMessage"] = "Unable to upload profile image.";
        }



        //Ska inte returnera en egen vy, utan ska dirigera tillbaka till details när den är klar. "En omladdning av sidan krävs"
        return RedirectToAction("Details", "Account");
    }


    [HttpPost]
    public async Task<IActionResult> UnSubscribe(SubscribeViewModel viewModel)
    {

        //hitta emailen
        var subscribedEmail = await _context.Users.FirstOrDefaultAsync(x => x.Email == viewModel.Email);
        if (subscribedEmail != null)
        {
            if (ModelState.IsValid)
            {
                // Skapa en ny modell enbart med e-postadressen
                var emailModel = new SubscribeViewModel { Email = viewModel.Email };


                var content = new StringContent(JsonConvert.SerializeObject(emailModel), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("https://localhost:7077/api/Subscribe", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["StatusMessage"] = "You have unsubscribed successfully!";
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    TempData["StatusMessage"] = "Email address not found.";
                }
            }
            else
            {
                TempData["StatusMessage"] = "Something went wrong, please try again.";
            }

        }


        return RedirectToAction("Home", "Default");

    }

}