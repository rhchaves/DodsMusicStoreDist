using DMS.WebApp.MVC.Models;

namespace DMS.WebApp.MVC.Services;

public interface IIdentityAuthenticationService
{
    Task<UserResponseLogin> Login(UserLogin userLogin);

    Task<UserResponseLogin> Register(UserRegistration userRegistration);
}
