using WebApp.ViewModels.Models;

namespace WebApp.ViewModels;

public class AccountDetailsViewModel
{
    public AccountBasicInfo? BasicInfo { get; set; }
    public AccountAddressInfo? AddressInfo { get; set; }
}
