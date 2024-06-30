using DMS.WebApp.MVC.Extensions;
using DMS.WebApp.MVC.Models;
using Microsoft.Extensions.Options;
using System.Text;
using System.Text.Json;
using static DMS.WebApp.MVC.Models.ErrorViewModel;

namespace DMS.WebApp.MVC.Services;

public class IdentityAuthenticationService : Service, IIdentityAuthenticationService
{
    private readonly HttpClient _httpClient;

    public IdentityAuthenticationService(HttpClient httpClient, IOptions<AppSettings> settings)
    {
        httpClient.BaseAddress = new Uri(settings.Value.AuthenticationUrl);
        _httpClient = httpClient;
    }

    public async Task<UserResponseLogin> Login(UserLogin userLogin)
    {
        var loginContent = new StringContent(
            JsonSerializer.Serialize(userLogin),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/identidade/autenticar", loginContent);

        if (!HandleErrorsResponse(response))
        {
            return new UserResponseLogin
            {
                ResponseResult = await DeserializeObjectResponse<ResponseResult>(response)
            };
        }
        return await DeserializeObjectResponse<UserResponseLogin>(response);
    }

    public async Task<UserResponseLogin> Register(UserRegistration userRegistration)
    {
        var registerContent = new StringContent(
            JsonSerializer.Serialize(userRegistration),
            Encoding.UTF8,
            "application/json");

        var response = await _httpClient.PostAsync("/api/identidade/nova-conta", registerContent);

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
        };

        if (!HandleErrorsResponse(response))
        {
            return new UserResponseLogin
            {
                ResponseResult = JsonSerializer.Deserialize<ResponseResult>(await response.Content.ReadAsStringAsync(), options)
            };
        }
        return JsonSerializer.Deserialize<UserResponseLogin>(await response.Content.ReadAsStringAsync());
    }
}