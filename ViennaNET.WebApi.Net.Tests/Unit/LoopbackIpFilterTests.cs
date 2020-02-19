using ViennaNET.WebApi.Net.IpTools;
using Moq;
using NUnit.Framework;

namespace ViennaNET.WebApi.Net.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(LoopbackIpFilter))]
  public class LoopbackIpFilterTests
  {
    private const string localIp = "1.1.1.1";
    private ILocalIpProvider _fakeLocalIpProvider;

    [OneTimeSetUp]
    public void Init()
    {
      var fakeLocalIpProvider = new Mock<ILocalIpProvider>();
      fakeLocalIpProvider.Setup(x => x.GetCurrentIp())
                         .Returns(localIp);
      _fakeLocalIpProvider = fakeLocalIpProvider.Object;
    }

    [TestCase("", localIp)]
    [TestCase("127.0.0.1", localIp)]
    [TestCase("::1", localIp)]
    [TestCase("2.2.2.2", "2.2.2.2")]
    public void FilterTest(string testIpValue, string expectedIpValue)
    {
      // arrange

      // act
      var testItem = new LoopbackIpFilter(_fakeLocalIpProvider);
      var resultIpValue = testItem.FilterIp(testIpValue);

      // assert
      Assert.AreEqual(expectedIpValue, resultIpValue);
    }
  }
}
