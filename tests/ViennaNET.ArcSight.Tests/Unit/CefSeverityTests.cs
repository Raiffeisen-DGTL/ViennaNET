using NUnit.Framework;
using SyslogNet.Client;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture]
  [Category("Unit")]
  public class CefSeverityTests
  {
    [Test]
    [TestCase(Severity.Emergency, CefSeverity.Emergency)]
    [TestCase(Severity.Alert, CefSeverity.Alert)]
    [TestCase(Severity.Critical, CefSeverity.Critical)]
    [TestCase(Severity.Error, CefSeverity.Error)]
    [TestCase(Severity.Warning, CefSeverity.Warning)]
    [TestCase(Severity.Notice, CefSeverity.Notice)]
    [TestCase(Severity.Informational, CefSeverity.Informational)]
    [TestCase(Severity.Debug, CefSeverity.Debug)]
    public void ToSyslogSeverity_Severity_CorrectCefSeverity(Severity syslogSeverity, CefSeverity cefSeverity)
    {
      var result = cefSeverity.ToSyslogSeverity();

      Assert.That(result, Is.EqualTo(syslogSeverity));
    }
  }
}