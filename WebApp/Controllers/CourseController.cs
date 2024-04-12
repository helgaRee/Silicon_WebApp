using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using WebApp.ViewModels;

namespace WebApp.Controllers;

//säkrar upp denna controller - måste vara inloggad för att visas
[Authorize]
public class CourseController(HttpClient httpClient) : Controller
{
    private readonly HttpClient _httpClient = httpClient;


    // den här Index()-metoden i CourseController en asynkron förfrågan till en API-endpunkt för att hämta en lista med kurser.
    // Om förfrågan lyckas, deserialiserar den svaret till en lista med kursvyer och skickar den tillbaka till vyn för att
    // visas för användaren.

    [Route("/courses")]
    public async Task<IActionResult> Index()
    {

        //hämtar en vy-modell och instansierar den
        var viewModel = new CourseIndexViewModel();

        //Använder HttpClient för att göra en asynkron GET-förfrågan till en API-endpoint (URLen) som ska returnera en lista med kurser
        var response = await _httpClient.GetAsync("https://localhost:7077/api/courses");

        //om förfrågan lyckas läses innehåller från svaret och deserialiserar det till en IEnumerable av typen CourseViewModel med JsnonConvert...
        if (response.IsSuccessStatusCode)
        {
            //returnera kurser - Deserializa det som en kurs - för detta krävs en modell
            var courses = JsonConvert.DeserializeObject<IEnumerable<CourseViewModel>>(await response.Content.ReadAsStringAsync());


            //Om den deserialiserade listan innehåller kurser, tilldesas den till Courses-egenskapen i vår vy-modell
            if (courses != null && courses.Any())
            {
                viewModel.Courses = courses;
            }

        }
        return View(viewModel);
    }
}
