namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.Unit.DSL
{
  internal static class Given
  {
    public static HttpClientsConfigurationBuilder Configuration => new();
    public static HttpClientFactoryBuilder HttpClientFactory => new();
  }
}