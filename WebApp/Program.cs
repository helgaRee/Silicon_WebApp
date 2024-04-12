using Infrastructure.Contexts;
using Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();


//Registrerar HttpClient
builder.Services.AddHttpClient();
builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("WebApp_Database")));
//Registrering av IDentity
builder.Services.AddIdentity<UserEntity, IdentityRole>(x =>
{
    x.SignIn.RequireConfirmedAccount = false;
    x.User.RequireUniqueEmail = true;
    x.Password.RequiredLength = 8;

}).AddEntityFrameworkStores<DataContext>();
//Configurera Cookies
builder.Services.ConfigureApplicationCookie(x =>
{
    x.LoginPath = "/signin"; //går till anvigen address
    x.Cookie.HttpOnly = true; //bara kan läsas ut av servern
    x.Cookie.SecurePolicy = CookieSecurePolicy.Always;  //alltid ska anv sig av Https
    x.ExpireTimeSpan = TimeSpan.FromHours(1); //sessionen är giltig i 1 h framåt
    x.SlidingExpiration = true; //vid aktivitet förlängs "expiretime"
});




var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Default}/{action=Home}/{id?}");

app.Run();
