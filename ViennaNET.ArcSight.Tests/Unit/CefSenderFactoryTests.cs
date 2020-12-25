using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.ArcSight.Configuration;
using ViennaNET.ArcSight.Exceptions;
using NUnit.Framework;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(CefSenderFactory))]
  public class CefSenderFactoryTests
  {
    private readonly CefSenderFactory _cefSenderFactory;

    public CefSenderFactoryTests()
    {
      _cefSenderFactory = new CefSenderFactory(new NullLogger<CefSenderFactory>());
    }

    [Test]
    public void CreateSender_SettingsWithIncorrectProtocol_ExceptionThrown()
    {
      Assert.Throws<ArcSightConfigurationException>(() => _cefSenderFactory.CreateSender(new ArcSightSection
      {
        Protocol = "protocol",
        ServerHost = "host",
        ServerPort = 60
      }));
    }
  }
}
