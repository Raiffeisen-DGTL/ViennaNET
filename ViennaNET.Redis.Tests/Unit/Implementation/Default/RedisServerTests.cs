using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ViennaNET.Redis.Implementation.Default;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Default
{
  [TestFixture(Category = "Unit", TestOf = typeof(RedisServer))]
  public class RedisServerTests
  {
    private IRedisServer _redisServer;
    private Mock<IServer> _serverMock;

    [OneTimeSetUp]
    public void RedisServerTestsSetUp()
    {
      _serverMock = new Mock<IServer>();
      _serverMock.Setup(x => x.ConfigGet(It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                 .Returns(new[] { new KeyValuePair<string, string>("config1", "abcdefgh") });
      _serverMock.Setup(x => x.LastSave(It.IsAny<CommandFlags>()))
                 .Returns(new DateTime(2014, 12, 3));
      _serverMock.Setup(x => x.LastSaveAsync(It.IsAny<CommandFlags>()))
                 .Returns(Task.FromResult(new DateTime(2014, 12, 3)));
      _serverMock.Setup(x => x.Time(It.IsAny<CommandFlags>()))
                 .Returns(new DateTime(2014, 12, 3, 11, 23, 12));
      _serverMock.Setup(x => x.TimeAsync(It.IsAny<CommandFlags>()))
                 .Returns(Task.FromResult(new DateTime(2014, 12, 3, 11, 23, 12)));
      _serverMock.Setup(x => x.InfoAsync(It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                 .Returns(Task.FromResult<IGrouping<string, KeyValuePair<string, string>>[]>(null));
      _serverMock.Setup(x => x.ConfigGetAsync(It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                 .Returns(Task.FromResult(new[] { new KeyValuePair<string, string>("config1", "abcdefgh") }));

      _serverMock.Setup(x => x.ConfigSetAsync(It.IsAny<RedisValue>(), It.IsAny<RedisValue>(), It.IsAny<CommandFlags>()))
                 .Returns(Task.CompletedTask);
      _serverMock.Setup(x => x.SaveAsync(It.IsAny<SaveType>(), It.IsAny<CommandFlags>()))
                 .Returns(Task.CompletedTask);
      _serverMock.Setup(x => x.EndPoint)
                 .Returns(new DnsEndPoint("localhost", 6379));
      _serverMock.Setup(x => x.IsConnected)
                 .Returns(true);
      _serverMock.Setup(x => x.ServerType)
                 .Returns(ServerType.Cluster);
      _serverMock.Setup(x => x.Version)
                 .Returns(new Version(1, 0, 2345, 21344));
      _serverMock.Setup(x => x.IsSlave)
                 .Returns(true);

      _serverMock
        .Setup(x => x.Keys(It.IsAny<int>(), It.IsAny<RedisValue>(), It.IsAny<int>(), It.IsAny<CommandFlags>()))
        .Returns(new List<RedisKey> { "somePrefixkey1", "somePrefixkey2" });

      _redisServer = new RedisServer(_serverMock.Object, "somePrefix");
    }

    [Test]
    public void ConfigGet_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.ConfigGet("pattern*");

      Assert.That(result.First()
                        .Key, Is.EqualTo("config1"));
      Assert.That(result.First()
                        .Value, Is.EqualTo("abcdefgh"));
      _serverMock.Verify(x => x.ConfigGet("pattern*", CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task ConfigGetAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _redisServer.ConfigGetAsync("pattern*");

      Assert.That(result.First()
                        .Key, Is.EqualTo("config1"));
      Assert.That(result.First()
                        .Value, Is.EqualTo("abcdefgh"));
      _serverMock.Verify(x => x.ConfigGetAsync("pattern*", CommandFlags.None), Times.Once);
    }

    [Test]
    public void ConfigSet_RedisServer_ServerMethodCalledCorrectly()
    {
      _redisServer.ConfigSet("key", "value");

      _serverMock.Verify(x => x.ConfigSet("key", "value", CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task ConfigSetAsync_RedisServer_ServerMethodCalledCorrectly()
    {
      await _redisServer.ConfigSetAsync("key", "value");

      _serverMock.Verify(x => x.ConfigSetAsync("key", "value", CommandFlags.None), Times.Once);
    }

    [Test]
    public void Info_RedisServer_ServerMethodCorrecty()
    {
      var result = _redisServer.Info("section");

      Assert.That(result, Is.Empty);
      _serverMock.Verify(x => x.Info("section", CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task InfoAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _redisServer.InfoAsync("section");

      Assert.That(result, Is.Null);
      _serverMock.Verify(x => x.InfoAsync("section", CommandFlags.None), Times.Once);
    }

    [Test]
    public void Keys_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.Keys("pattern*")
                               .ToList();

      Assert.That(result.First(), Is.EqualTo("key1"));
      Assert.That(result.Last(), Is.EqualTo("key2"));
      _serverMock.Verify(x => x.Keys(0, "somePrefixpattern*", 10, CommandFlags.None));
    }

    [Test]
    public void LastSave_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.LastSave();

      Assert.That(result, Is.EqualTo(new DateTime(2014, 12, 3)));
      _serverMock.Verify(x => x.LastSave(CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task LastSaveAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _redisServer.LastSaveAsync();

      Assert.That(result, Is.EqualTo(new DateTime(2014, 12, 3)));
      _serverMock.Verify(x => x.LastSaveAsync(CommandFlags.None), Times.Once);
    }

    [Test]
    public void Save_RedisServer_ServerMethodCalledCorrectly()
    {
      _redisServer.Save();

      _serverMock.Verify(x => x.Save(SaveType.BackgroundSave, CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task SaveAsync_RedisServer_ServerMethodCalledCorrectly()
    {
      await _redisServer.SaveAsync();

      _serverMock.Verify(x => x.SaveAsync(SaveType.BackgroundSave, CommandFlags.None), Times.Once);
    }

    [Test]
    public void Time_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.Time();

      Assert.That(result, Is.EqualTo(new DateTime(2014, 12, 3, 11, 23, 12)));
      _serverMock.Verify(x => x.LastSave(CommandFlags.None), Times.Once);
    }

    [Test]
    public async Task TimeAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _redisServer.TimeAsync();

      Assert.That(result, Is.EqualTo(new DateTime(2014, 12, 3, 11, 23, 12)));
      _serverMock.Verify(x => x.TimeAsync(CommandFlags.None), Times.Once);
    }

    [Test]
    public void EndPoint_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.EndPoint;

      Assert.That(result, Is.EqualTo(new DnsEndPoint("localhost", 6379)));
      _serverMock.Verify(x => x.EndPoint);
    }

    [Test]
    public void IsConnected_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.IsConnected;

      Assert.That(result, Is.EqualTo(true));
      _serverMock.Verify(x => x.IsConnected);
    }

    [Test]
    public void IsSlave_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.IsSlave;

      Assert.That(result, Is.EqualTo(true));
      _serverMock.Verify(x => x.IsSlave);
    }

    [Test]
    public void ServerType_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.ServerType;

      Assert.That(result, Is.EqualTo(ServerType.Cluster));
      _serverMock.Verify(x => x.ServerType);
    }

    [Test]
    public void Version_RedisServer_ServerMethodCorrectResult()
    {
      var result = _redisServer.Version;

      Assert.That(result, Is.EqualTo(new Version(1, 0, 2345, 21344)));
      _serverMock.Verify(x => x.Version);
    }
  }
}
