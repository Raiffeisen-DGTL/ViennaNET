using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using ViennaNET.Diagnostic.Data;

namespace ViennaNET.Diagnostic.Tests.Unit
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(HealthCheckingService))]
  public class HealthCheckingServiceTests
  {
    [Test]
    public void Constructor_ImplementorsIsNull_ThrowArgumentNullException()
    {
      IEnumerable<IDiagnosticImplementor> implementors = null;

      Assert.Throws<ArgumentNullException>(() => new HealthCheckingService(implementors));
    }

    [Test]
    public async Task CheckHealthAsync_HasImplementorWithDiagnosticInfo_ReturnsDiagnosticInfo()
    {
      // arrange
      var fakeResult = new DiagnosticInfo("test", "url");
      var fakeImplementor = new Mock<IDiagnosticImplementor>();
      fakeImplementor.Setup(x => x.Diagnose())
        .ReturnsAsync(new[] { fakeResult });
      IEnumerable<IDiagnosticImplementor> implementors = new[] { fakeImplementor.Object };

      // act
      var service = new HealthCheckingService(implementors);
      var result = await service.CheckHealthAsync();

      // assert
      Assert.That(result, Is.EquivalentTo(new[] { fakeResult }));
    }

    [Test]
    public async Task CheckHealthAsync_HasImplementorWithOkDiagnosticInfo_InvokeDiagnosticPassedEvent()
    {
      // arrange
      var fakeResult = new DiagnosticInfo("test", "url");
      var fakeImplementor = new Mock<IDiagnosticImplementor>();
      fakeImplementor.Setup(x => x.Diagnose())
        .ReturnsAsync(new[] { fakeResult });
      IEnumerable<IDiagnosticImplementor> implementors = new[] { fakeImplementor.Object };

      var isEventInvoked = false;

      // act
      var service = new HealthCheckingService(implementors);
      service.DiagnosticPassedEvent += () => isEventInvoked = true;
      var result = await service.CheckHealthAsync();

      // assert
      Assert.That(isEventInvoked, Is.True);
    }

    [Test]
    public async Task CheckHealthAsync_HasImplementorWithNotOkAndSkipDiagnosticInfo_InvokeDiagnosticPassedEvent()
    {
      // arrange
      var fakeResult = new DiagnosticInfo("test", "url", DiagnosticStatus.Fail, isSkipResult: true);
      var fakeImplementor = new Mock<IDiagnosticImplementor>();
      fakeImplementor.Setup(x => x.Diagnose())
        .ReturnsAsync(new[] { fakeResult });
      IEnumerable<IDiagnosticImplementor> implementors = new[] { fakeImplementor.Object };

      var isEventInvoked = false;

      // act
      var service = new HealthCheckingService(implementors);
      service.DiagnosticPassedEvent += () => isEventInvoked = true;
      var result = await service.CheckHealthAsync();

      // assert
      Assert.That(isEventInvoked, Is.True);
    }

    [Test]
    public async Task CheckHealthAsync_HasImplementorWithNotOkDiagnosticInfo_InvokeDiagnosticFailedEvent()
    {
      // arrange
      var fakeResult = new DiagnosticInfo("test", "url", DiagnosticStatus.Fail);
      var fakeImplementor = new Mock<IDiagnosticImplementor>();
      fakeImplementor.Setup(x => x.Diagnose())
        .ReturnsAsync(new[] { fakeResult });
      IEnumerable<IDiagnosticImplementor> implementors = new[] { fakeImplementor.Object };

      var isEventInvoked = false;

      // act
      var service = new HealthCheckingService(implementors);
      service.DiagnosticFailedEvent += () => isEventInvoked = true;
      var result = await service.CheckHealthAsync();

      // assert
      Assert.That(isEventInvoked, Is.True);
    }
  }
}