using System;
using NUnit.Framework;
using Company.WebApi.Core.Net.IpTools;

namespace Company.WebApi.Core.Tests.Unit
{
  [TestFixture(Category = "Integration")]
  public class IpProviderTests
  {
    [Test]
    public void GetCurrentIp_InNetwork_ReturnsIp()
    {
      // arrange

      // act
      var provider = new LocalIpProvider();
      var ip = provider.GetCurrentIp();

      // assert
      Console.WriteLine($"IP is {ip}");
      Assert.That(ip, Is.Not.Null.And.Not.Empty);
    }
  }
}
