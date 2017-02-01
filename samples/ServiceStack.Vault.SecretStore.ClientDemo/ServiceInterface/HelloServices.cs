namespace ServiceStack.Vault.SecretStore.ClientDemo.ServiceInterface
{
    using Authentication.IdentityServer.Providers;
    using ServiceModel;

    [Authenticate(IdentityServerAuthProvider.Name)]
    public class HelloServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = "Many bothans died to bring you this information." };
        }
    }
}
