namespace Quantum.Core.Models.Auth
{
    public class ResetPasswordTokenModel
    {
        public string UserId { get; set; }

        public string Token { get; set; }
    }
}
