using System;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Orm.Configuration;

namespace ViennaNET.Orm.Tests.Unit.Configuration
{
  [TestFixture, Category("Unit"), TestOf(typeof(ConnectionInfo))]
  public class ConnectionInfoTests
  {
    private IConfigurationBuilder LoadConfiguration()
    {
      var builder = new ConfigurationBuilder();
      var stream = new MemoryStream();
      var writer = new StreamWriter(stream);

      writer.Write(DbSection);
      writer.Flush();
      _ = stream.Seek(0, SeekOrigin.Begin);

      return builder.AddJsonStream(stream);
    }

    const string DbSection = @"{
    ""db"": [
        {
            ""nick"": ""default"",
            ""dbServerType"": ""MSSQL"",
            ""ConnectionString"": ""Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;"",
            ""IsSkipHealthCheckEntity"": ""true""
        },
        {
            ""nick"": ""defaultDB2"",
            ""dbServerType"": ""DB2"",
            ""ConnectionString"": ""Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;"",
            ""IsSkipHealthCheckEntity"": ""true""
        }
    ]
}";

    [TestCase("defaultDB2", "DB2")]
    [TestCase("default", "MSSQL")]
    public void DeserializeConnectionInfoTest(string nick, string dbServerType)
    {
      var confBuilder = LoadConfiguration();

      var connectionInfo = confBuilder.Build()
        .GetSection("db")
        .Get<ConnectionInfo[]>()
        .SingleOrDefault(ci => ci.Nick == nick && ci.DbServerType == dbServerType);

      Assert.Multiple(() =>
      {
        Assert.AreEqual(nick, connectionInfo.Nick);
        Assert.AreEqual(dbServerType, connectionInfo.DbServerType);
        Assert.AreEqual(true, connectionInfo.IsSkipHealthCheckEntity);
        Assert.AreEqual("Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;", connectionInfo.ConnectionString);
      });
    }

    [Test]
    public void ToString_EncriptedPasswordDidntSet_CorrectString()
    {
      var connectionInfo = new ConnectionInfo
      {
        Nick = "mssql",
        ConnectionString = "Server=some_server,1433;Database=some_db;User ID=some_user;Password=123;",
        DbServerType = "MSSQL",
        UseCallContext = false,
        IsSkipHealthCheckEntity = false
      };

      var connection = connectionInfo.ToString();

      Assert.That(connection,
        Is.EqualTo("Nick=mssql, Type=MSSQL, " +
        "ConnectionString=Server=some_server,1433;Database=some_db;User ID=some_user;Password=123;," +
        " UseCallContext=False, IsSkipHealthCheckEntity=False"));
    }
  }
}