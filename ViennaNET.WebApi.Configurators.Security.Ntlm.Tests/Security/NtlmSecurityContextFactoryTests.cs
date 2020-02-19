using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using ViennaNET.WebApi.Abstractions;
using ViennaNET.WebApi.Net;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;

namespace ViennaNET.WebApi.Configurators.Security.Ntlm.Tests.Security
{
  [TestFixture(Category = "Unit.Win")]
  [Explicit]
  public class NtlmSecurityContextFactoryTests
  {
    [Test]
    public void Create_RequestIdentity_TakeNameFromCurrent()
    {
      // arrange
      var fakeHttpContext = new Mock<IHttpContextAccessor>();
      var fakeHttpClientFactory = new Mock<IHttpClientFactory>();
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();

      // act
      var factory = new NtlmSecurityContextFactory(fakeHttpContext.Object, 
                                                   fakeHttpClientFactory.Object, 
                                                   fakeLoopbackIpFilter.Object);
      var result = factory.Create();

      // assert
      var expectedName = WindowsIdentity.GetCurrent()
                                        .Name.Split('\\')
                                        .Last();

      Assert.That(result.UserName, Is.EqualTo(expectedName));
    }

    [Test]
    public void Create_RequestHeadersExists_TakeDataFromHeaders()
    {
      // arrange
      var fakeHttpClientFactory = new Mock<IHttpClientFactory>();
      var fakeRequest = new Mock<HttpRequest>();
      var fakeHeaders = new HeaderDictionary();
      fakeHeaders.Add(CompanyHttpHeaders.UserId, "hamster");
      fakeHeaders.Add(CompanyHttpHeaders.RequestHeaderCallerIp, "some IP");
      fakeRequest.Setup(x => x.Headers)
                 .Returns(fakeHeaders);
      var fakeContext = new Mock<HttpContext>();
      fakeContext.Setup(x => x.Request)
                 .Returns(fakeRequest.Object);
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();
      fakeLoopbackIpFilter.Setup(x => x.FilterIp(It.IsAny<string>()))
                          .Returns((string s) => s);

      var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
      fakeHttpContextAccessor.Setup(x => x.HttpContext)
                             .Returns(fakeContext.Object);

      // act
      var factory = new NtlmSecurityContextFactory(fakeHttpContextAccessor.Object, 
                                                   fakeHttpClientFactory.Object, 
                                                   fakeLoopbackIpFilter.Object);
      var result = factory.Create();

      // assert
      Assert.That(result.UserName, Is.EqualTo("hamster"));
      Assert.That(result.UserIp, Is.EqualTo("some IP"));
    }
  }
}
