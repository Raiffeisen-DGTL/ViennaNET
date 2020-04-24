using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using Moq;
using NUnit.Framework;
using ViennaNET.WebApi.Abstractions;

namespace ViennaNET.WebApi.Configurators.CallContext.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(HttpCallContext))]
  public class HttpCallContextTests
  {
    [Test]
    public void Create_RequestWithoutHeaders_ContextWithDefaultValues()
    {
      // arrange
      var fakeIdentity = new Mock<IIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(false);
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);

      var fakeRequest = new Mock<HttpRequest>();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary());

      var fakeHttpContext = new Mock<HttpContext>();
      fakeHttpContext.Setup(x => x.Request)
                     .Returns(fakeRequest.Object);
      fakeHttpContext.Setup(x => x.User)
                     .Returns(fakePrincipal);

      // act
      var context = HttpCallContext.Create(fakeHttpContext.Object);

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(context.UserId, Is.EqualTo(string.Empty));
        Assert.That(context.UserDomain, Is.EqualTo(string.Empty));
        Assert.That(context.RequestCallerIp, Is.EqualTo(string.Empty));
        Assert.That(context.AuthorizeInfo, Is.EqualTo(string.Empty));
        Assert.That(context.RequestId, Is.Not.Null);
      });
    }

    [Test]
    public void Create_AllFieldsInHeaders_CopiedToContext()
    {
      // arrange
      var fakeIdentity = new Mock<IIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(false);
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);

      var fakeRequest = new Mock<HttpRequest>();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary()
                 {
                   { CompanyHttpHeaders.RequestId, "123" },
                   { HeaderNames.Authorization, "token" },
                   { CompanyHttpHeaders.RequestHeaderCallerIp, "ip" },
                   { CompanyHttpHeaders.UserId, "user" },
                   { CompanyHttpHeaders.UserDomain, "domain" },
                 });

      var fakeHttpContext = new Mock<HttpContext>();
      fakeHttpContext.Setup(x => x.Request)
                     .Returns(fakeRequest.Object);
      fakeHttpContext.Setup(x => x.User)
                     .Returns(fakePrincipal);

      // act
      var context = HttpCallContext.Create(fakeHttpContext.Object);

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(context.RequestId, Is.EqualTo("123"));
        Assert.That(context.AuthorizeInfo, Is.EqualTo("token"));
        Assert.That(context.RequestCallerIp, Is.EqualTo("ip"));
        Assert.That(context.UserId, Is.EqualTo("user"));
        Assert.That(context.UserDomain, Is.EqualTo("domain"));
      });
    }

    [TestCase("ggg\\fff", "ggg")]
    [TestCase("ggg", "")]
    public void GetUserDomain_VariousCases_Correct(string username, string expectedDomain)
    {
      // arrange
      var fakeIdentity = new Mock<IIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(false);
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);

      var fakeRequest = new Mock<HttpRequest>();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary() { { CompanyHttpHeaders.UserId, username } });

      var fakeHttpContext = new Mock<HttpContext>();
      fakeHttpContext.Setup(x => x.Request)
                     .Returns(fakeRequest.Object);
      fakeHttpContext.Setup(x => x.User)
                     .Returns(fakePrincipal);

      // act
      var context = HttpCallContext.Create(fakeHttpContext.Object);

      // assert
      Assert.That(context.UserDomain, Is.EqualTo(expectedDomain));
    }

    [TestCase("ggg\\fff", "fff")]
    [TestCase("ggg", "ggg")]
    public void GetUserName_VariousCases_Correct(string username, string expectedName)
    {
      // arrange
      var fakeIdentity = new Mock<IIdentity>();
      fakeIdentity.Setup(x => x.IsAuthenticated)
                  .Returns(false);
      var fakePrincipal = new ClaimsPrincipal(fakeIdentity.Object);

      var fakeRequest = new Mock<HttpRequest>();
      fakeRequest.Setup(x => x.Headers)
                 .Returns(new HeaderDictionary() { { CompanyHttpHeaders.UserId, username } });

      var fakeHttpContext = new Mock<HttpContext>();
      fakeHttpContext.Setup(x => x.Request)
                     .Returns(fakeRequest.Object);
      fakeHttpContext.Setup(x => x.User)
                     .Returns(fakePrincipal);

      // act
      var context = HttpCallContext.Create(fakeHttpContext.Object);

      // assert
      Assert.That(context.UserId, Is.EqualTo(expectedName));
    }
  }
}
