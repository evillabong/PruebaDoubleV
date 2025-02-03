namespace WebApi.Extensions
{
    public static class  ConfigurationExtension
    {
        public static string GetPublicKey(this IConfiguration configuration) => configuration.GetValue<string>("SecurityOptions:RsaPublicKey") ?? string.Empty;
        public static string GetPrivateKey(this IConfiguration configuration) => configuration.GetValue<string>("SecurityOptions:RsaPrivateKey") ?? string.Empty;

    }
}
