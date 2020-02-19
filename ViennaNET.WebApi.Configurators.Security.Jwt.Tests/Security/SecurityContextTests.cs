using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.WebApi.Configurators.Security.Jwt.Tests.Security
{
  [TestFixture, Category("Unit"), TestOf(typeof(SecurityContext))]
  public class SecurityContextTests
  {
    [TestCase("CanSold", false)]
    [TestCase("CanBuy", true)]
    public async Task HasPermissions_VariousCases_CorrectResult(string permission, bool expectedresult)
    {
      // arrange
      var userName = "Hamster";
      var ip = "1.1.1.1";
      var permissions = new[] { "CanBuy" };

      // act
      var context = new SecurityContext(userName, ip, permissions);
      var result = await context.HasPermissionsAsync(permission);

      // assert
      Assert.That(result, Is.EqualTo(expectedresult));
    }

    [Test]
    public void UserName_UserNameWithDomain_ReturnsClearedUserName()
    {
      // arrange
      var userName = "raiffeisen\\hamster";
      var ip = "1.1.1.1";
      var permissions = new[] { "CanBuy" };

      // act
      var context = new SecurityContext(userName, ip, permissions);
      var result = context.UserName;

      // assert
      Assert.That(result, Is.EqualTo("hamster"));
    }
  }
}
