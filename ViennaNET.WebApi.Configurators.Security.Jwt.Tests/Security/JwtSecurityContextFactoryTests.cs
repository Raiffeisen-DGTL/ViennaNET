using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;
using ViennaNET.CallContext;
using ViennaNET.Security.Jwt;
using ViennaNET.WebApi.Net;

namespace ViennaNET.WebApi.Configurators.Security.Jwt.Tests.Security
{
  [TestFixture, Category("Unit"), TestOf(typeof(JwtSecurityContextFactory))]
  public class JwtSecurityContextFactoryTests
  {
    [Test]
    public void Create_NoAnyIdentity_CreatesFromEmptyContext()
    {
      // arrange
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();
      fakeLoopbackIpFilter.Setup(x => x.FilterIp(It.IsAny<string>()))
                          .Returns("123");

      var fakeCallContextFactory = new Mock<ICallContextFactory>();
      var fakeCallContext = new EmptyCallContext();
      fakeCallContextFactory.Setup(x => x.Create())
                            .Returns(fakeCallContext);

      var fakeJwtReadingService = new Mock<IJwtTokenReader>();
      fakeJwtReadingService.Setup(x => x.Read(It.IsAny<string>()))
                           .Returns((ClaimsPrincipal)null);

      // act
      var factory = new JwtSecurityContextFactory(fakeCallContextFactory.Object, fakeLoopbackIpFilter.Object,
                                                  fakeJwtReadingService.Object);
      var result = factory.Create();

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(result.UserName, Is.EqualTo(Environment.UserName));
        Assert.That(result.UserIp, Is.EqualTo("123"));
        Assert.That(result.GetUserPermissionsAsync()
                          .Result.Any(), Is.False);
      });
    }

    [Test]
    public async Task Create_AuthenticatedIdentity_CorrectlyPutHeadersDataToContext()
    {
      // arrange
      var fakeIdentity = new Mock<ClaimsIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(true);
      fakeIdentity.Setup(x => x.Claims)
                  .Returns(new List<Claim>() { new Claim(ClaimTypes.Role, "some Role") });

      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();
      fakeLoopbackIpFilter.Setup(x => x.FilterIp(It.IsAny<string>()))
                          .Returns((string s) => s);

      var fakeCallContext = new Mock<ICallContext>();
      fakeCallContext.Setup(x => x.AuthorizeInfo)
                     .Returns("token");
      fakeCallContext.Setup(x => x.UserId)
                     .Returns("Hamster");
      fakeCallContext.Setup(x => x.RequestCallerIp)
                     .Returns("123");
      var fakeCallContextFactory = new Mock<ICallContextFactory>();
      fakeCallContextFactory.Setup(x => x.Create())
                            .Returns(fakeCallContext.Object);
      var fakeJwtReadingService = new Mock<IJwtTokenReader>();
      fakeJwtReadingService.Setup(x => x.Read(It.IsAny<string>()))
                           .Returns(new ClaimsPrincipal(fakeIdentity.Object));

      // act
      var factory = new JwtSecurityContextFactory(fakeCallContextFactory.Object, fakeLoopbackIpFilter.Object,
                                                  fakeJwtReadingService.Object);
      var result = factory.Create();

      // assert
      Assert.That(result.UserName, Is.EqualTo("Hamster"));
      Assert.That(result.UserIp, Is.EqualTo("123"));
      Assert.That(await result.GetUserPermissionsAsync(), Is.EquivalentTo(new List<string>() { "some Role" }));
    }
  }
}
