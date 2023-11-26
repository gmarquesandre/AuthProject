namespace AuthProject.Models;

public partial class AuthController
{
    public class RequestRefreshToken
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

}
