using Infrastrucutre.WebApp.Models.Components;

namespace Presentation.ViewModels;

public class ShowcaseViewModel
{
    public string? Id { get; set; }
    //public ImageViewModel Showcase-Image { get; set; } = "showcase-image.svg";
    public ImageViewModel ShowcaseImage { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string Text { get; set; } = null!;

    //skapar en instans av LinkViewModel så den inte är tom
    public LinkViewModel Link { get; set; } = new LinkViewModel();

    public string? BrandsText { get; set; }
    public List<ImageViewModel>? Brands { get; set; }

}
