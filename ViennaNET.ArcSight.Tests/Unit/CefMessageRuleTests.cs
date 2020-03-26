using System;
using ViennaNET.ArcSight.Validation;
using NUnit.Framework;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  public class CefMessageRuleTests
  {
    public static object[] InvalidMessageCases()
    {
      const string hostName = "host";
      var startTime = DateTimeOffset.Parse("2013-09-19 08:26:10.999");
      return new object[]
      {
        new Action(() => new CefMessage(startTime, hostName, "", "threatmanager", "1.0", 100, "worm successfullystopped",
                                        CefSeverity.Emergency)),
        new Action(() => new CefMessage(startTime, hostName, "Security", "", "1.0", 100, "worm successfullystopped",
                                        CefSeverity.Emergency)),
        new Action(() => new CefMessage(startTime, hostName, "Security", "threatmanager", "", 100, "worm successfullystopped",
                                        CefSeverity.Emergency)),
        new Action(() => new CefMessage(startTime, hostName, "Security", "threatmanager", "1.0", -100, "", CefSeverity.Emergency)),
        new Action(() => new CefMessage(default(DateTimeOffset), hostName, "Security", "threatmanager", "1.0", -100, "",
                                        CefSeverity.Emergency)),
        new Action(() => new CefMessage(startTime, null, "Security", "threatmanager", "1.0", -100, "", CefSeverity.Emergency)),
        new Action(() => new CefMessage(default(DateTimeOffset), null, "Security", "threatmanager", "1.0", 100,
                                        "worm successfullystopped", CefSeverity.Emergency)),
      };
    }

    [Test]
    [TestCaseSource(nameof(InvalidMessageCases))]
    public void Validate_ConstructedInvalidMessage_Exception(Action constructor)
    {
      Assert.That(() => constructor(), Throws.Exception);
    }

    [Test]
    public void Validate_ConstructedMessage_IsValid()
    {
      var rule = new CefMessageRule();
      var msg = new CefMessage(DateTimeOffset.UtcNow, "host", "Security", "threatmanager", "1.0", 100, "worm successfullystopped",
                               CefSeverity.Emergency);
      var result = rule.Validate(msg, null);

      Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_ConstructedWithAdditionalInfo_IsValid()
    {
      var rule = new CefMessageRule();
      var msg = new CefMessage(DateTimeOffset.UtcNow, "host", "Security", "threatmanager", "1.0", 100, "worm successfullystopped",
                               CefSeverity.Emergency);
      msg.Extensions.FileModificationTime = DateTimeOffset.UtcNow;

      var result = rule.Validate(msg, null);

      Assert.That(result.IsValid, Is.True);
    }
  }
}
