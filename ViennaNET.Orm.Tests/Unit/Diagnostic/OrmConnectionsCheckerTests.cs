using ViennaNET.Diagnostic.Data;
using ViennaNET.Orm.Diagnostic;
using ViennaNET.Orm.Factories;
using Moq;
using NHibernate;
using NHibernate.Metadata;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ViennaNET.Orm.Tests.Unit.Diagnostic
{
  [TestFixture(Category = "Unit"), TestOf(typeof(OrmConnectionsChecker))]
  public class OrmConnectionsCheckerTests
  {
    private const string dbNick = "default";
    private const string exception = "TestException";
    private const string correctEntityKey = "Test.Entities.Entity";
    private const string exceptionEntityKey = "Test.Entities.EntityException";

    private static OrmConnectionsChecker CreateOrmConnectionsChecker(string entityKey, bool hasMetadata = true, bool hasProviders = true)
    {
      var sessionFactoryProviderMock = new Mock<ISessionFactoryProvider>();
      sessionFactoryProviderMock.Setup(x => x.Nick)
                                .Returns(dbNick);
      var sessionFactoryMock = new Mock<ISessionFactory>();
      var result = hasMetadata
        ? new Dictionary<string, IClassMetadata> { { entityKey, null } }
        : new Dictionary<string, IClassMetadata>();
      sessionFactoryMock.Setup(x => x.GetAllClassMetadata())
                        .Returns(result);
      var sessionMock = new Mock<ISession>();
      sessionMock.Setup(x => x.SessionFactory)
                 .Returns(sessionFactoryMock.Object);

      sessionFactoryMock.Setup(x => x.OpenSession())
                        .Returns(sessionMock.Object);

      var sessionFactoryManagerMock = new Mock<ISessionFactoryManager>();
      sessionFactoryManagerMock.Setup(x => x.GetSessionFactory(It.IsAny<string>()))
                               .Returns(sessionFactoryMock.Object);

      var criteria = new Mock<ICriteria>();
      criteria.Setup(x => x.SetMaxResults(It.IsAny<int>()))
              .Returns(() => criteria.Object);
      criteria.Setup(x => x.SetTimeout(It.IsAny<int>()))
              .Returns(() => criteria.Object);
      sessionMock.Setup(x => x.CreateCriteria(It.Is<string>(arg => arg == correctEntityKey)))
                 .Returns(() => criteria.Object);
      sessionMock.Setup(x => x.CreateCriteria(It.Is<string>(arg => arg == exceptionEntityKey)))
                 .Throws(new ArgumentException(exception));

      var managerMock = new Mock<ISessionFactoryProvidersManager>();
      managerMock.Setup(x => x.GetSessionFactoryProviders())
                 .Returns(hasProviders
                            ? new[] { sessionFactoryProviderMock.Object }
                            : Enumerable.Empty<ISessionFactoryProvider>());

      var checker = new OrmConnectionsChecker(managerMock.Object, sessionFactoryManagerMock.Object);
      return checker;
    }

    [Test]
    public void Diagnose_ExceptionInCriteria_FailedResult()
    {
      var checker = CreateOrmConnectionsChecker(exceptionEntityKey);

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error, Contains.Substring(exception));
      Assert.That(diagnosticInfo.Name, Is.EqualTo($"DB: {dbNick}"));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.DbConnectionError));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.False);
    }

    [Test]
    public void Diagnose_HasMetadata_CorrectResult()
    {
      var checker = CreateOrmConnectionsChecker(correctEntityKey);

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.Name, Is.EqualTo($"DB: {dbNick}"));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.Ok));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.False);
    }

    [Test]
    public void Diagnose_NoMetadata_CorrectResult()
    {
      var checker = CreateOrmConnectionsChecker(correctEntityKey, false);

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.Name, Is.EqualTo($"DB: {dbNick}"));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.Ok));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.False);
    }

    [Test]
    public void Diagnose_NoProviders_CorrectResult()
    {
      var checker = CreateOrmConnectionsChecker(correctEntityKey, true, false);

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      Assert.That(!result.Any());
    }

    [Test]
    public void Key_CorrectlyFilled()
    {
      var checker = CreateOrmConnectionsChecker(correctEntityKey);

      Assert.That(checker.Key, Is.EqualTo("ormdb"));
    }
  }
}
