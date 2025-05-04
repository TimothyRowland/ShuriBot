using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ShuriStaff.api.Services
{
    public class DiscordAuthenticationService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public DiscordAuthenticationService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<AuthenticationResult> AuthenticateDiscordUserAsync(string code)
        {
            try
            {
                // 1. Exchange the authorization code for an access token
                var tokenResponse = await ExchangeCodeForTokenAsync(code);
                if (!tokenResponse.IsSuccess)
                {
                    return AuthenticationResult.Failure(tokenResponse.Error);
                }

                var accessToken = tokenResponse.AccessToken;

                // 2. Fetch user information from Discord
                var discordUser = await GetDiscordUserAsync(accessToken);
                if (discordUser == null)
                {
                    return AuthenticationResult.Failure("Failed to retrieve Discord user information.");
                }

                // 3. Fetch the user's guilds from Discord
                var guilds = await GetUserGuildsAsync(accessToken);
                if (guilds == null)
                {
                    return AuthenticationResult.Failure("Failed to retrieve Discord user guilds.");
                }

                // 4. Check if the user is in the required server and has the required roles
                var requiredGuildId = _configuration["Discord:RequiredGuildId"]; // Configure in appsettings.json
                var requiredRoleIds = _configuration.GetSection("Discord:RequiredRoleIds").Get<string[]>(); // Configure as an array

                if (!await IsUserInRequiredServerAndHasRoleAsync(discordUser, guilds, requiredGuildId, requiredRoleIds, accessToken))
                {
                    return AuthenticationResult.Failure("User is not authorized based on server and roles.");
                }

                // 5. Authentication successful
                // In a real application, you might generate your own authentication token here
                return AuthenticationResult.Success(accessToken); // For now, we'll just return the Discord access token as a sign of success
            }
            catch (Exception ex)
            {
                return AuthenticationResult.Failure($"An error occurred during Discord authentication: {ex.Message}");
            }
        }

        private async Task<TokenResponse> ExchangeCodeForTokenAsync(string code)
        {
            var clientId = _configuration["Discord:ClientId"];
            var clientSecret = _configuration["Discord:ClientSecret"];
            var redirectUri = _configuration["Discord:RedirectUri"];
            var tokenEndpoint = "https://discord.com/api/oauth2/token";

            using var httpClient = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint);
            request.Headers.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

            var content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", redirectUri),
            });

            request.Content = content;

            var response = await httpClient.SendAsync(request);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    var tokenData = JsonSerializer.Deserialize<TokenResponse>(responseContent);
                    return new TokenResponse { AccessToken = tokenData?.access_token };
                }
                catch (JsonException)
                {
                    return new TokenResponse { Error = "Failed to parse token response." };
                }
            }
            else
            {
                return new TokenResponse { Error = $"Failed to exchange code for token: {responseContent}" };
            }
        }

        private async Task<JsonElement?> GetDiscordUserAsync(string accessToken)
        {
            var userEndpoint = "https://discord.com/api/v10/users/@me";
            return await GetDiscordApiAsync(userEndpoint, accessToken);
        }

        private async Task<JsonElement?> GetUserGuildsAsync(string accessToken)
        {
            var guildsEndpoint = "https://discord.com/api/v10/users/@me/guilds";
            return await GetDiscordApiAsync(guildsEndpoint, accessToken);
        }

        private async Task<JsonElement?> GetGuildMemberAsync(string accessToken, string guildId, string userId)
        {
            var memberEndpoint = $"https://discord.com/api/v10/guilds/{guildId}/members/{userId}";
            return await GetDiscordApiAsync(memberEndpoint, accessToken);
        }

        private async Task<JsonElement?> GetDiscordApiAsync(string endpoint, string accessToken)
        {
            using var httpClient = _httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync(endpoint);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                try
                {
                    return JsonSerializer.Deserialize<JsonElement>(responseContent);
                }
                catch (JsonException)
                {
                    // Log error
                    return null;
                }
            }
            else
            {
                // Log error with responseContent
                return null;
            }
        }

        private async Task<bool> IsUserInRequiredServerAndHasRoleAsync(JsonElement? user, JsonElement? guilds, string requiredGuildId, string[] requiredRoleIds, string accessToken)
        {
            if (!user.HasValue || !guilds.HasValue || string.IsNullOrEmpty(requiredGuildId) || requiredRoleIds == null || requiredRoleIds.Length == 0)
            {
                return false;
            }

            if (!user.Value.TryGetProperty("id", out var userIdElement) || !userIdElement.TryGetString(out var userId))
            {
                return false;
            }

            bool isInRequiredServer = false;
            if (guilds.Value.ValueKind == JsonValueKind.Array)
            {
                foreach (var guild in guilds.Value.EnumerateArray())
                {
                    if (guild.TryGetProperty("id", out var guildIdElement) && guildIdElement.GetString() == requiredGuildId)
                    {
                        isInRequiredServer = true;
                        break;
                    }
                }
            }

            if (!isInRequiredServer)
            {
                return false;
            }

            // Now we need to fetch the member information for the specific guild to check roles
            var accessToken = await ExchangeCodeForTokenAsync("dummy").Result.AccessToken; // We need a valid access token here, consider how to pass it efficiently
            if (string.IsNullOrEmpty(accessToken)) return false;

            var memberInfo = await GetGuildMemberAsync(accessToken, requiredGuildId, userId);
            if (memberInfo.HasValue && memberInfo.Value.TryGetProperty("roles", out var rolesElement) && rolesElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var roleIdElement in rolesElement.EnumerateArray())
                {
                    if (roleIdElement.TryGetString(out var roleId) && requiredRoleIds.Contains(roleId))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

    // Helper class for the token response from Discord
    public class TokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public string scope { get; set; }
        public string Error { get; set; }
    }
}