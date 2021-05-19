using System.Linq;
using System.Net.Http;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Moq;
using NUnit.Framework;
using ViennaNET.CallContext;
using ViennaNET.WebApi.Net;

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
      var fakeCallContextFactory = new Mock<ICallContextFactory>();
      var callContext = new Mock<ICallContext>();
      fakeCallContextFactory.Setup(f => f.Create()).Returns(callContext.Object);

      // act
      var factory = new NtlmSecurityContextFactory(fakeHttpContext.Object,
                                                   fakeHttpClientFactory.Object,
                                                   fakeLoopbackIpFilter.Object,
                                                   fakeCallContextFactory.Object);
      var result = factory.Create();

      // assert
      var expectedName = WindowsIdentity.GetCurrent()
                                        .Name.Split('\\')
                                        .Last();

      Assert.That(result.UserName, Is.EqualTo(expectedName));
    }

    [Test]
    public void Create_CallContextExists_TakeDataFromCallContext()
    {
      // arrange
      var fakeHttpClientFactory = new Mock<IHttpClientFactory>();
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();
      fakeLoopbackIpFilter.Setup(x => x.FilterIp(It.IsAny<string>()))
                          .Returns((string s) => s);

      var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
      var fakeCallContextFactory = new Mock<ICallContextFactory>();
      var callContext = new Mock<ICallContext>();
      callContext.Setup(c => c.UserId).Returns("hamster");
      callContext.Setup(c => c.RequestCallerIp).Returns("some IP");
      fakeCallContextFactory.Setup(f => f.Create()).Returns(callContext.Object);

      // act
      var factory = new NtlmSecurityContextFactory(fakeHttpContextAccessor.Object,
                                                   fakeHttpClientFactory.Object,
                                                   fakeLoopbackIpFilter.Object,
                                                   fakeCallContextFactory.Object);
      var result = factory.Create();

      // assert
      Assert.That(result.UserName, Is.EqualTo("hamster"));
      Assert.That(result.UserIp, Is.EqualTo("some IP"));
    }
  }
}
