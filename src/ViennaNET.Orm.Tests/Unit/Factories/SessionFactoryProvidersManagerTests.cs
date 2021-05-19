using System.Linq;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;
using ViennaNET.Orm.Configuration;
using ViennaNET.Orm.Exceptions;
using ViennaNET.Orm.Factories;

namespace ViennaNET.Orm.Tests.Unit.Factories
{
  [TestFixture, Category("Unit"), TestOf(typeof(SessionFactoryProvidersManager))]
  public class SessionFactoryProvidersManagerTests
  {
    private const string db2Nick = "db2";
    private const string mssqlNick = "mssql";
    private const string oracleNick = "oracle";
    private const string postgreSqlNick = "postgresql";

    private static Mock<ISessionFactoryProviderGetter> GetSessionFactoryProvider(string nick, string type)
    {
      var sessionFactoryProvider = new Mock<ISessionFactoryProvider>();
      sessionFactoryProvider.Setup(x => x.Nick)
                            .Returns(nick);
      var getter = new Mock<ISessionFactoryProviderGetter>();
      getter.Setup(x => x.Type)
            .Returns(type);
      getter.Setup(x => x.GetSessionFactoryProvider(It.IsAny<ConnectionInfo>()))
            .Returns(sessionFactoryProvider.Object);

      return getter;
    }

    [Test]
    public void GetSessionFactoryProvider_CorrectlyConfigured_ProvidersGot()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/orm.json", optional: true)
                                                    .Build();

      var db2Getter = GetSessionFactoryProvider(db2Nick, "DB2");

      var msSqlGetter = GetSessionFactoryProvider(mssqlNick, "MSSQL");

      var oracleGetter = GetSessionFactoryProvider(oracleNick, "Oracle");

      var postgreSqlGetter = GetSessionFactoryProvider(postgreSqlNick, "PostgreSql");

      var sessionFactoryProvidersManager =
        new SessionFactoryProvidersManager(configuration, new[] { db2Getter.Object, msSqlGetter.Object, oracleGetter.Object, postgreSqlGetter.Object });

      var db2Provider = sessionFactoryProvidersManager.GetSessionFactoryProvider(db2Nick);
      var msProvider = sessionFactoryProvidersManager.GetSessionFactoryProvider(mssqlNick);
      var oracleProvider = sessionFactoryProvidersManager.GetSessionFactoryProvider(oracleNick);
      var postgreSqlProvider = sessionFactoryProvidersManager.GetSessionFactoryProvider(postgreSqlNick);

      Assert.That(db2Provider.Nick == db2Nick);
      Assert.That(msProvider.Nick == mssqlNick);
      Assert.That(oracleProvider.Nick == oracleNick);
      Assert.That(postgreSqlProvider.Nick == postgreSqlNick);
    }

    [Test]
    public void GetSessionFactoryProvider_NoConfiguration_ExceptionThrown()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/ormNoDBSection.json", optional: true)
                                                    .Build();

      var sessionFactoryProvidersManager = new SessionFactoryProvidersManager(configuration, new ISessionFactoryProviderGetter[] { });

      Assert.Throws<SessionFactoryProviderException>(() => sessionFactoryProvidersManager.GetSessionFactoryProvider(It.IsAny<string>()),
                                                     "There is no db section in the JSON configuration. Make sure you config is valid");
    }

    [Test]
    public void GetSessionFactoryProvider_NoProviderRegistered_ExceptionThrown()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/orm.json", optional: true)
                                                    .Build();

      var db2Getter = GetSessionFactoryProvider(db2Nick, "DB2");

      var sessionFactoryProvidersManager =
        new SessionFactoryProvidersManager(configuration, new[] { db2Getter.Object });

      Assert.Throws<SessionFactoryProviderException>(() => sessionFactoryProvidersManager.GetSessionFactoryProvider(It.IsAny<string>()),
                                                     "Session factory provider getter with nick = mssql is not found");
    }

    [Test]
    public void GetSessionFactoryProviders_CorrectlyConfigured_ProvidersGot()
    {
      var configuration = new ConfigurationBuilder().AddJsonFile("TestConfiguration/orm.json", optional: true)
                                                    .Build();

      var db2Getter = GetSessionFactoryProvider(db2Nick, "DB2");

      var msSqlGetter = GetSessionFactoryProvider(mssqlNick, "MSSQL");

      var oracleGetter = GetSessionFactoryProvider(oracleNick, "Oracle");

      var postgreSqlGetter = GetSessionFactoryProvider(postgreSqlNick, "PostgreSql");

      var sessionFactoryProvidersManager =
        new SessionFactoryProvidersManager(configuration, new[] { db2Getter.Object, msSqlGetter.Object, oracleGetter.Object, postgreSqlGetter.Object });

      var providers = sessionFactoryProvidersManager.GetSessionFactoryProviders()
                                                    .ToList();

      Assert.That(providers.Count == 4);
    }
  }
}
