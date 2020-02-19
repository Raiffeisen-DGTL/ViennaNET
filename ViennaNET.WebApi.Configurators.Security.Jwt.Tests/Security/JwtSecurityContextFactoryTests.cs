using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using ViennaNET.WebApi.Abstractions;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;
using Moq;
using ViennaNET.WebApi.Net;

namespace ViennaNET.WebApi.Configurators.Security.Jwt.Tests.Security
{
  [TestFixture, Category("Unit"), TestOf(typeof(JwtSecurityContextFactory))]
  public class JwtSecurityContextFactoryTests
  {
    [Test]
    public void Create_NoAnyIdentity_ReturnsHostContext()
    {
      // arrange
      var fakeContext = new Mock<HttpContext>();
      var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
      fakeHttpContextAccessor.Setup(x => x.HttpContext)
                             .Returns(fakeContext.Object);
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();

      // act
      var factory = new JwtSecurityContextFactory(fakeHttpContextAccessor.Object, 
                                                  fakeLoopbackIpFilter.Object);
      var result = factory.Create();

      // assert
      Assert.That(result.UserName, Is.EqualTo(Environment.UserName));
    }

    [Test]
    public void Create_UnauthenticatedIdentity_ReturnsHostContext()
    {
      // arrange
      var fakeIdentity = new Mock<IIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(false);
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);
      var fakeContext = new Mock<HttpContext>();
      fakeContext.Setup(x => x.User)
                 .Returns(fakePrincipal);
      var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
      fakeHttpContextAccessor.Setup(x => x.HttpContext)
                             .Returns(fakeContext.Object);
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();

      // act
      var factory = new JwtSecurityContextFactory(fakeHttpContextAccessor.Object, 
                                                  fakeLoopbackIpFilter.Object);
      var result = factory.Create();

      // assert
      Assert.That(result.UserName, Is.EqualTo(Environment.UserName));
    }

    [Test]
    public async Task Create_AuthenticatedIdentity_CorrectlyPutHeadersDataToContext()
    {
      // arrange
      var fakeIdentity = new Mock<ClaimsIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(true);
      fakeIdentity.Setup(x => x.Claims)
                  .Returns(new List<Claim>()
                  {
                    new Claim(ClaimTypes.NameIdentifier, "Hamster"), new Claim(ClaimTypes.Role, "some Role")
                  });
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);
      var fakeContext = new Mock<HttpContext>();
      fakeContext.Setup(x => x.User)
                 .Returns(fakePrincipal);
      var fakeRequest = new Mock<HttpRequest>();
      var fakeHeaderDictionary = new HeaderDictionary();
      fakeHeaderDictionary.Add(CompanyHttpHeaders.RequestHeaderCallerIp, "123");
      fakeRequest.Setup(x => x.Headers)
                 .Returns(fakeHeaderDictionary);
      fakeContext.Setup(x => x.Request)
                 .Returns(fakeRequest.Object);
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();
      fakeLoopbackIpFilter.Setup(x => x.FilterIp(It.IsAny<string>()))
                    .Returns((string s) => s);

      var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
      fakeHttpContextAccessor.Setup(x => x.HttpContext)
                             .Returns(fakeContext.Object);
      // act
      var factory = new JwtSecurityContextFactory(fakeHttpContextAccessor.Object, 
                                                  fakeLoopbackIpFilter.Object);
      var result = factory.Create();

      // assert
      Assert.That(result.UserName, Is.EqualTo("Hamster"));
      Assert.That(result.UserIp, Is.EqualTo("123"));
      Assert.That(await result.GetUserPermissionsAsync(), Is.EquivalentTo(new List<string>() { "some Role" }));
    }

    [Test]
    public void Create_NoIpInIncomingHeader_TakeLocalIp()
    {
      // arrange
      var fakeIdentity = new Mock<ClaimsIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(true);
      fakeIdentity.Setup(x => x.Claims)
                  .Returns(new List<Claim>()
                  {
                    new Claim(ClaimTypes.NameIdentifier, "Hamster"), new Claim(ClaimTypes.Role, "some Role")
                  });
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);
      var fakeContext = new Mock<HttpContext>();
      fakeContext.Setup(x => x.User)
                 .Returns(fakePrincipal);
      var fakeRequest = new Mock<HttpRequest>();
      var fakeHeaderDictionary = new HeaderDictionary();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(fakeHeaderDictionary);
      fakeContext.Setup(x => x.Request)
                 .Returns(fakeRequest.Object);
      var fakeLoopbackIpFilter = new Mock<ILoopbackIpFilter>();
      fakeLoopbackIpFilter.Setup(x => x.FilterIp(It.IsAny<string>()))
                    .Returns("10.10.10.10");

      var fakeHttpContextAccessor = new Mock<IHttpContextAccessor>();
      fakeHttpContextAccessor.Setup(x => x.HttpContext)
                             .Returns(fakeContext.Object);
      // act
      var factory = new JwtSecurityContextFactory(fakeHttpContextAccessor.Object, 
                                                  fakeLoopbackIpFilter.Object);
      var result = factory.Create();

      // assert
      var expectedIp = "10.10.10.10";
      Assert.That(result.UserIp, Is.EqualTo(expectedIp));
    }
  }
}
