namespace ShuriStaff.api.Services
{
    public class AuthenticationResult
    {
        public bool IsSuccess { get; set; }
        public string AccessToken { get; set; } // Or your application's auth token/session ID upon successful Discord verification
        public string ErrorMessage { get; set; }

        public static AuthenticationResult Success(string accessToken)
        {
            return new AuthenticationResult { IsSuccess = true, AccessToken = accessToken };
        }

        public static AuthenticationResult Failure(string errorMessage)
        {
            return new AuthenticationResult { IsSuccess = false, ErrorMessage = errorMessage };
        }
    }
}