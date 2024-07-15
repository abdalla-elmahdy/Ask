using System.Security.Claims;
using System.Text.Json;
using Ask.Frontend.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace Ask.Frontend.Identity;

public class CookieAuthenticationStateProvider(IHttpClientFactory httpClient) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = httpClient.CreateClient("Auth");
    private bool _authenticated;
    private readonly ClaimsPrincipal _unauthenticated = new ClaimsPrincipal(new ClaimsIdentity());
    private readonly JsonSerializerOptions _jsonSerializerOptions = new () {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };


    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        _authenticated = false;
        
        var user = _unauthenticated;

        try
        {
            var userResponse = await _httpClient.GetAsync("/manage/info");
            userResponse.EnsureSuccessStatusCode();
            var userJson = await userResponse.Content.ReadAsStringAsync();
            var userInfo = JsonSerializer.Deserialize<UserInfo>(userJson, _jsonSerializerOptions);

            if (userInfo is not null)
            {
                _authenticated = true;
                var claims = new List<Claim>
                {
                    new (ClaimTypes.Name, userInfo.Email),
                    new (ClaimTypes.Email, userInfo.Email)
                };

                var claimsIdentity = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
                user = new ClaimsPrincipal(claimsIdentity);
            }
        }
        catch
        {
            //TODO: Add logging for this case instead of crashing
        }

        return new AuthenticationState(user);
    }
}
