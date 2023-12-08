namespace Blog.Extensions
{
    public static class ConfigurationExtensions
    {
        public static string SmtpHost(this IConfiguration configuration) => configuration[$"Smtp:Host"];

        public static string ConnectionString(this IConfiguration configuration) => configuration[$"ConnectionStrings:DefaultConnection"];

        public static string JwtKey(this IConfiguration configuration) => configuration[$"JwtKey"];

        public static string ApiKey(this IConfiguration configuration) => configuration[$"ApiKey"];

        public static string ApiKeyName(this IConfiguration configuration) => configuration[$"ApiKeyName"];

        public static string Env(this IConfiguration configuration) => configuration[$"Env"];

        public static string Versao(this IConfiguration configuration) => configuration[$"Versao"];

        public static string NomeApp(this IConfiguration configuration) => configuration[$"NomeApp"];
    }
}
