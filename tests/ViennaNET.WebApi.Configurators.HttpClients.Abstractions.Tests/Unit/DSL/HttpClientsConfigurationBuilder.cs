using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Configuration;

namespace ViennaNET.WebApi.Configurators.HttpClients.Abstractions.Tests.Unit.DSL
{
  internal class HttpClientsConfigurationBuilder
  {
    private readonly List<WebapiEndpoint> _endpoints = new();

    public HttpClientsConfigurationBuilder WithWebapiEndpoint(WebapiEndpoint endpoint)
    {
      _endpoints.Add(endpoint);
      return this;
    }

    public IConfiguration Build()
    {
      var configurationBuilder = new ConfigurationBuilder();
      foreach (var source in _endpoints.Select(MapToSource))
      {
        configurationBuilder.Sources.Add(source);
      }

      return configurationBuilder.Build();
    }

    private MemoryConfigurationSource MapToSource(WebapiEndpoint endpoint, int index)
    {
      return new()
      {
        InitialData = new[]
        {
          new KeyValuePair<string, string>($"{WebapiEndpointsSection.SectionName}:{index}:name", endpoint.Name),
          new KeyValuePair<string, string>($"{WebapiEndpointsSection.SectionName}:{index}:url", endpoint.Url),
          new KeyValuePair<string, string>($"{WebapiEndpointsSection.SectionName}:{index}:isHealthCheck",
            endpoint.IsHealthCheck.ToString())
        }
      };
    }
  }
}