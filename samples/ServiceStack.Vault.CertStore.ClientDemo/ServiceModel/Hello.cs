namespace ServiceStack.Vault.CertStore.ClientDemo.ServiceModel
{
    [Route("/hello")]
    public class Hello : IReturn<HelloResponse>
    {
    }

    public class HelloResponse
    {
        public string Result { get; set; }
    }
}
