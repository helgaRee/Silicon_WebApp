using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class AuthController : Controller
{

    private readonly UserManager<UserEntity> _userManager;
    private readonly SignInManager<UserEntity> _signInManager;
    private readonly DataContext _context;

    public AuthController(UserManager<UserEntity> userManager, SignInManager<UserEntity> signInManager, DataContext context = null)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
    }


    [Route("/signup")]
    public IActionResult SignUp()
    {
        return View();
    }


    //REGISTRERING AV AN NY ANVÄNDARE
    [Route("/signup")]
    [HttpPost]
    public async Task<IActionResult> SignUp(SignUpViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            //kontroll om anv redan finns
            if (await _context.Users.AnyAsync(x => x.Email == viewModel.Email))
            {
                ViewData["StatusMessage"] = "User with the same email already exist";
            }
            else //skapa användaren
            {
                //Mappar om min modell till en USERENTITY
                var userEntity = new UserEntity
                {
                    Email = viewModel.Email,
                    UserName = viewModel.Email,
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                };

                var result = await _userManager.CreateAsync(userEntity, viewModel.Password);
                //när registrering har lyckats, loggas användaren in direkt
                if (result.Succeeded)
                {
                    //vidare kontroll om inloggning lyckas
                    if ((await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, false, false)).Succeeded)
                        return LocalRedirect("/");
                    else
                        //Om inlogg EJ lyckas, omdirigera till SignIn
                        return LocalRedirect("/signin");
                }
                else
                {
                    ViewData["StatusMessage"] = "Something went wrong. Try again later.";
                }
            }
        }
        return View(viewModel);
    }



    [Route("/signin")]
    public IActionResult SignIn(string returnUrl)
    {

        //Anger returnUrl, skulle den vara tom, blir den en slash
        ViewData["ReturnUrl"] = returnUrl ?? "/";
        return View();
    }



    //INLOGGNING AV EN ANVÄNDARE
    [Route("/signin")]
    [HttpPost]
    public async Task<IActionResult> SignIn(SignInViewModel viewModel, string returnUrl)
    {
        //försök logga in om giltig
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(viewModel.Email);
            if (user != null)
            {
                if ((await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password, viewModel.RememberMe, false)).Succeeded)
                    return LocalRedirect(returnUrl);
            }
            else
            {
                ViewData["ErrorMessage"] = "The user with this Email doesn't exist.";
                return View(viewModel);
            }


        }

        ViewData["ReturnUrl"] = returnUrl;
        ViewData["ErrorMessage"] = "Some of the fields are incorrect, try again.";
        //returnerar modellen
        return View(viewModel);
    }



    [Route("/signout")]
    public new async Task<IActionResult> SignOut()
    {
        await _signInManager.SignOutAsync();
        //återgår till startsidan efter inloggning
        return RedirectToAction("Home", "Default");
    }
}
