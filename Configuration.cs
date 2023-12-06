using System.Security.Cryptography;

namespace Blog;

public static class Configuration
{
    //Token - JWT - Json Web Token
    public static string JwtKey { get; set; } = "MWUxN2NhMDEtNjJiMC00ZTk5LWExMzQtMTBiMjcwNWI1ZDZi";
    public static string ApiKeyName = "api_key";
    public static string ApiKey = "blogAPI_bd4b1c89407b26ccfca96c22d923f478";
    public static SmtpConfiguration Smtp = new ();


    public class SmtpConfiguration
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}