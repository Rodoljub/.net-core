namespace Quantum.ResourceServer.Infrastructure.Configurations
{
    public class CorsConfiguration
    {
        public CorsConfiguration()
        {

        }

        public CorsConfiguration(bool loadDafault = false)
        {
            if (loadDafault)
            {
                this.AllowedOrigins = new[] { "*" };

                this.AllowedMethods = new[] { "GET", "POST" /*, "PUT", "PATCH", "DELETE" */ };
            }
        }

        public string[] AllowedOrigins { get; set; }

        public string[] AllowedMethods { get; set; }
    }
}
