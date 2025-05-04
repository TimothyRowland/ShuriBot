using Microsoft.AspNetCore.Mvc;
using ShuriStaff.api.Services; // Replace with your actual namespace
using System.Threading.Tasks;

namespace ShuriStaff.api.Controllers
{
    [ApiController]
    [Route("api/auth")] // Or a different base route if you prefer
    public class AuthController : ControllerBase
    {
        private readonly DiscordAuthenticationService _discordAuthService;

        // Inject the DiscordAuthenticationService via constructor injection
        public AuthController(DiscordAuthenticationService discordAuthService)
        {
            _discordAuthService = discordAuthService;
        }

        [HttpGet("discord/callback")]
        public async Task<IActionResult> DiscordCallback([FromQuery] string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                // Handle the case where the code is missing (e.g., user denied access)
                return Redirect("/unauthorized"); // Or your frontend's unauthorized route
            }

            // Call the service to handle the Discord authentication logic
            var authenticationResult = await _discordAuthService.AuthenticateDiscordUserAsync(code);

            if (authenticationResult.IsSuccess)
            {
                // Authentication successful, redirect to dashboard and set auth cookie/token
                // Example of setting a cookie (you might use a different method)
                Response.Cookies.Append("YourAuthCookieName", authenticationResult.AccessToken, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = true, // Important for security
                    Secure = Request.IsHttps // Only send over HTTPS in production
                    // You might need to set other cookie options like expiration
                });

                return Redirect("/dashboard"); // Your frontend's dashboard route
            }
            else
            {
                // Authentication failed, redirect to unauthorized page
                return Redirect("/unauthorized"); // Your frontend's unauthorized route
            }
        }
    }

    // You might want to define a result class in your Services/Models folder
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; } // Or your application's auth token/session ID
        public string ErrorMessage { get; set; }
    }
}