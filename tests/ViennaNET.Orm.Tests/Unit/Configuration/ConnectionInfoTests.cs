using Microsoft.Extensions.Configuration;
using NUnit.Framework;
using ViennaNET.Orm.Configuration;

namespace ViennaNET.Orm.Tests.Unit.Configuration
{
    [TestFixture]
    [Category("Unit")]
    [TestOf(typeof(ConnectionInfo))]
    public class ConnectionInfoTests
    {
        private IConfiguration _configuration = null!;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            var builder = new ConfigurationBuilder();

            builder.AddInMemoryCollection(new KeyValuePair<string, string>[]
            {
                new("db:0:nick", "default"), new("db:0:dbServerType", "MSSQL"),
                new("db:0:ConnectionString", "Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;"),
                new("db:0:IsSkipHealthCheckEntity", "true"), new("db:1:nick", "defaultDB2"),
                new("db:1:dbServerType", "DB2"),
                new("db:1:ConnectionString", "Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;"),
                new("db:1:IsSkipHealthCheckEntity", "true"), new("db:2:nick", "PasswordDB2"),
                new("db:2:dbServerType", "DB2"),
                new("db:2:ConnectionString", "Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;"),
                new("db:2:IsSkipHealthCheckEntity", "true"),
            });

            _configuration = builder.Build();
        }
        
        [TestCase("defaultDB2", "DB2")]
        [TestCase("default", "MSSQL")]
        public void DeserializeConnectionInfoTest(string nick, string dbServerType)
        {
            var connectionInfo = _configuration
                .GetSection("db")
                .Get<ConnectionInfo[]>()
                .SingleOrDefault(ci => ci.Nick == nick && ci.DbServerType == dbServerType);

            Assert.Multiple(() =>
            {
                Assert.That(connectionInfo?.Nick, Is.EqualTo(nick));
                Assert.That(connectionInfo?.DbServerType, Is.EqualTo(dbServerType));
                Assert.That(connectionInfo is { IsSkipHealthCheckEntity: true }, Is.EqualTo(true));
                Assert.That(connectionInfo?.ConnectionString,
                    Is.EqualTo("Database=some_db;User ID=some_user;PWD=123;Server=some_server,9999;"));
            });
        }

        [Test]
        public void ToStringTest()
        {
            var connectionInfo = new ConnectionInfo
            {
                Nick = "mssql",
                ConnectionString =
                    "Server=some_server,1433;Database=some_db;User ID=some_user;Password=some_password;",
                DbServerType = "MSSQL"
            };

            var connection = connectionInfo.ToString();

            Assert.That(connection,
                Is.EqualTo("Nick=mssql, Type=MSSQL, " +
                           "ConnectionString=Server=some_server,1433;Database=some_db;User ID=some_user;Password=some_password;, " +
                           "IsSkipHealthCheckEntity=False"));
        }
    }
}