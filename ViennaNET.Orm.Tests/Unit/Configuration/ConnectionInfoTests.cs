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
      Environment.SetEnvironmentVariable("CRYPTO_KEY", "____any_secret_32_symbol_key____");

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
        },
        {
            ""nick"": ""encPasswordDB2"",
            ""dbServerType"": ""DB2"",
            ""EncPassword"": ""Mt73QjHmJHZHgeU61dux8/KEwXH7m+5Cimh7UE4QK1o="",
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
        Assert.AreEqual(default, connectionInfo.EncPassword);
        Assert.AreEqual(true, connectionInfo.IsSkipHealthCheckEntity);
        Assert.AreEqual("Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;", connectionInfo.ConnectionString);
      });
    }

    [TestCase("defaultDB2", "DB2")]
    [TestCase("encPasswordDB2", "DB2")]
    public void DeserializeEncPasswordTest(string nick, string dbServerType)
    {
      var confBuilder = LoadConfiguration();

      var connectionInfo = confBuilder.Build()
        .GetSection("db")
        .Get<ConnectionInfo[]>()
        .SingleOrDefault(ci => ci.Nick == nick && ci.DbServerType == dbServerType);

      Assert.That(connectionInfo.EncPassword, Is.EqualTo(default).Or.EqualTo("123"));
    }

    [Test]
    public void ToString_EncriptedPasswordDidntSet_CorrectString()
    {
      var connectionInfo = new ConnectionInfo
      {
        Nick = "mssql",
        ConnectionString = "Server=some_server,1433;Database=some_db;User ID=some_user;Password=some_password;",
        DbServerType = "MSSQL",
        EncPassword = "",
        UseCallContext = false,
        IsSkipHealthCheckEntity = false
      };

      var connection = connectionInfo.ToString();

      Assert.That(connection,
        Is.EqualTo("Nick=mssql, Type=MSSQL, " +
        "ConnectionString=Server=some_server,1433;Database=some_db;User ID=some_user;Password=some_password;," +
        " UseCallContext=False, IsSkipHealthCheckEntity=False"));
    }
  }
}