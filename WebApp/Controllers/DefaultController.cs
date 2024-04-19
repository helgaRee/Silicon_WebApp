using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using WebApp.ViewModels;

namespace WebApp.Controllers;

public class DefaultController : Controller
{
    private readonly HttpClient _httpClient;

    public DefaultController(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public IActionResult Home()
    {

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Subscribe(SubscribeViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            //Paketera ihop och skicka iväg till APIet. Apiet behöver nedan info för att hämta in:
            //Packa ihop och konvertera till JSON-formaterad Data och serializera viewmodellen - encoda den - sätt mediaType
            var content = new StringContent(JsonConvert.SerializeObject(viewModel), Encoding.UTF8, "application/json");
            //response - postar - Request URL från swagger
            var response = await _httpClient.PostAsync("https://localhost:7077/api/Subscribe", content);
            //kontroll om responsen lyckades
            if (response.IsSuccessStatusCode)
            {
                TempData["StatusMessage"] = "You are subscribed!";
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
            {
                TempData["StatusMessage"] = "You are already subscribed.";
            }

        }
        else
        {
            TempData["StatusMessage"] = "Something went wrong, is the email address correct?";
        }

        //omdirigeras till sidan
        return RedirectToAction("Home", "Default", "subscribe"); //action, controller, Id. För att kommer till ett fragment, en viss sektion på sidan
    }


    [Route("/error")]
    public IActionResult Error404(int? statusCode)
    {
        if (statusCode.HasValue && statusCode.Value == 404)
        {
            // Anpassa din felhanteringssida för 404-fel
            return View("Error404");
        }
        // För andra felstatuskoder, returnera standardfelhanteringssidan
        return View("Error");
    }


}
