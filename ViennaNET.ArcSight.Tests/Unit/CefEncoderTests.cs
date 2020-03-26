using System;
using NUnit.Framework;

namespace ViennaNET.ArcSight.Tests.Unit
{
  [TestFixture]
  [Category("Unit"), TestOf(typeof(CefEncoder))]
  public class CefEncoderTests
  {
    [Test]
    [TestCaseSource(nameof(MessageCases))]
    public void Escape(CefMessage orig, CefMessage escaped)
    {
      var origDeviceVendor = CefEncoder.EncodeHeader(orig.DeviceVendor);
      var origDeviceProduct = CefEncoder.EncodeHeader(orig.DeviceProduct);
      var origDeviceVersion = CefEncoder.EncodeHeader(orig.DeviceVersion);
      var origName = CefEncoder.EncodeHeader(orig.Name);
      var extensionsSourceHostName = CefEncoder.EncodeExtension(orig.Extensions.SourceHostName);

      Assert.That(origDeviceVendor, Is.EqualTo(escaped.DeviceVendor));
      Assert.That(origDeviceProduct, Is.EqualTo(escaped.DeviceProduct));
      Assert.That(origDeviceVersion, Is.EqualTo(escaped.DeviceVersion));
      Assert.That(origName, Is.EqualTo(escaped.Name));
      Assert.That(extensionsSourceHostName, Is.EqualTo(escaped.Extensions.SourceHostName));
    }

    public static object[] MessageCases()
    {
      var startTime = DateTimeOffset.Parse("2013-09-19 08:26:10.999");
      return new object[]
      {
        new object[]
        {
          new CefMessage(startTime, "host", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency),
          new CefMessage(startTime, "host", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency)
        },
        new object[]
        {
          new CefMessage(startTime, "   host   ", "   Security   ", "   threatmanager   ", "   1.0   ", 100,
                         "   worm successfullystopped   ", CefSeverity.Emergency),
          new CefMessage(startTime, "host", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency)
        },
        new object[]
        {
          new CefMessage(startTime, "host", "Secu|rity", "threa|tmanager", "1.|0", 100, "worm suc|cessfullystopped", CefSeverity.Emergency),
          new CefMessage(startTime, "host", "Secu\\|rity", "threa\\|tmanager", "1.\\|0", 100, "worm suc\\|cessfullystopped",
                         CefSeverity.Emergency)
        },
        new object[]
        {
          new CefMessage(startTime, "ho\\st", "Secu\\rity", "threat\\manager", "1.\\0", 100, "worm successful\\lystopped",
                         CefSeverity.Emergency),
          new CefMessage(startTime, "ho\\\\st", "Secu\\\\rity", "threat\\\\manager", "1.\\\\0", 100, "worm successful\\\\lystopped",
                         CefSeverity.Emergency)
        },
        new object[]
        {
          new CefMessage(startTime, "ho=st", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency),
          new CefMessage(startTime, "ho\\=st", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency)
        },
        new object[]
        {
          new CefMessage(startTime, "ho\rst", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency),
          new CefMessage(startTime, "ho\\rst", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency)
        },
        new object[]
        {
          new CefMessage(startTime, "ho\nst", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency),
          new CefMessage(startTime, "ho\\nst", "Security", "threatmanager", "1.0", 100, "worm successfullystopped", CefSeverity.Emergency)
        },
      };
    }
  }
}
