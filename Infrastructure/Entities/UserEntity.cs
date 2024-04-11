using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Entities;

public class UserEntity : IdentityUser
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? ProfileImage { get; set; } = "avatar.jpg";
    public string? Biography { get; set; }
    public int? AddressId { get; set; }
    public AddressEntity? AddressEntity { get; set; }
}
