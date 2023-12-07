namespace Blog.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string SmtpHost(this IConfiguration configuration) => configuration[$"Smtp:Host"];

        public static string ConnectionString(this IConfiguration configuration) => configuration[$"ConnectionStrings:Complete"];

        public static string JwtKey(this IConfiguration configuration) => configuration[$"JwtKey"];

        public static string ApiKey(this IConfiguration configuration) => configuration[$"ApiKey"];

        public static string ApiKeyName(this IConfiguration configuration) => configuration[$"ApiKeyName"];
    }
}
