using Microsoft.Extensions.Logging.Abstractions;
using NUnit.Framework;
using ViennaNET.Redis.Implementation.Silent;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Silent
{
  [TestFixture(Category = "Unit", TestOf = typeof(SilentRedisDatabaseProvider))]
  public class SilentRedisDatabaseProviderTests
  {
    [OneTimeSetUp]
    public void SilentRedisDatabaseProviderTestsSetUp()
    {
      _silentRedisDatabaseProviderMock =
        new SilentRedisDatabaseProvider(null, null, new NullLogger<SilentRedisDatabaseProvider>());
    }

    private SilentRedisDatabaseProvider _silentRedisDatabaseProviderMock;

    [Test]
    public void GetDatabase_RedisDatabaseProviderNull_DatabaseCreated()
    {
      var database = _silentRedisDatabaseProviderMock.GetDatabase();

      Assert.That(database, Is.Not.Null);
    }

    [Test]
    public void GetServer_RedisDatabaseProviderNull_ServerCreated()
    {
      var server = _silentRedisDatabaseProviderMock.GetServer(null);

      Assert.That(server, Is.Not.Null);
    }

    [Test]
    public void GetAllServers_RedisDatabaseProviderNull_EmptyServersCollection()
    {
      var servers = _silentRedisDatabaseProviderMock.GetAllServers();

      Assert.That(servers, Is.Empty);
    }
  }
}