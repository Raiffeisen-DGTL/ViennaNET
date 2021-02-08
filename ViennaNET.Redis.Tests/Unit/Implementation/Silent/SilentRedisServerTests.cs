using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.Redis.Implementation.Silent;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using RedisException = ViennaNET.Redis.Exceptions.RedisException;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Silent
{
  [TestFixture(Category = "Unit", TestOf = typeof(SilentRedisServer))]
  public class SilentRedisServerTests
  {
    private ISilentRedisServer _silentRedisServerNull;
    private ISilentRedisServer _silentRedisServerMock;

    [OneTimeSetUp]
    public void SilentRedisServerTestsSetUp()
    {
      _silentRedisServerNull = new SilentRedisServer(null, null);

      var exceptionMock = new Mock<IRedisServer>();
      exceptionMock.Setup(x => x.ConfigGet(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.LastSave(It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.LastSaveAsync(It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.Time(It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.TimeAsync(It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.Info(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.InfoAsync(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.ConfigGetAsync(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.ConfigSet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.ConfigSetAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.Save(It.IsAny<SaveType>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.SaveAsync(It.IsAny<SaveType>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.EndPoint)
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.IsConnected)
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.ServerType)
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.Version)
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.IsSlave)
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.EndPoint)
                   .Throws(new RedisException());

      _silentRedisServerMock = new SilentRedisServer(exceptionMock.Object, new NullLogger<SilentRedisServer>());
    }

    [Test]
    public void ConfigGet_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.ConfigGet("pattern*");

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConfigGet_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.ConfigGet("pattern*");

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ConfigGetAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerMock.ConfigGetAsync("pattern*");

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task ConfigGetAsync_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerNull.ConfigGetAsync("pattern*");

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ConfigSet_RedisServer_ServerMethodCalledCorrectly()
    {
      _silentRedisServerMock.ConfigSet("key", "value");

      Assert.Pass();
    }

    [Test]
    public void ConfigSet_RedisServerNull_ServerMethodCalledCorrectly()
    {
      _silentRedisServerNull.ConfigSet("key", "value");

      Assert.Pass();
    }

    [Test]
    public async Task ConfigSetAsync_RedisServer_ServerMethodCalledCorrectly()
    {
      await _silentRedisServerMock.ConfigSetAsync("key", "value");

      Assert.Pass();
    }

    [Test]
    public async Task ConfigSetAsync_RedisServerNull_ServerMethodCalledCorrectly()
    {
      await _silentRedisServerNull.ConfigSetAsync("key", "value");

      Assert.Pass();
    }

    [Test]
    public void EndPoint_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.EndPoint;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void EndPoint_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.EndPoint;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Info_RedisServer_ServerMethodCorrecty()
    {
      var result = _silentRedisServerMock.Info("section");

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Info_RedisServerNull_ServerMethodCorrecty()
    {
      var result = _silentRedisServerNull.Info("section");

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task InfoAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerMock.InfoAsync("section");

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task InfoAsync_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerNull.InfoAsync("section");

      Assert.That(result, Is.Null);
    }

    [Test]
    public void IsConnected_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.IsConnected;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void IsConnected_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.IsConnected;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void IsSlave_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.IsSlave;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void IsSlave_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.IsSlave;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Keys_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.Keys("pattern*");

      Assert.That(result, Is.Empty);
    }

    [Test]
    public void Keys_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.Keys("pattern*");

      Assert.That(result, Is.Null);
    }

    [Test]
    public void LastSave_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.LastSave();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void LastSave_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.LastSave();

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task LastSaveAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerMock.LastSaveAsync();

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task LastSaveAsync_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerNull.LastSaveAsync();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Save_RedisServer_ServerMethodCalledCorrectly()
    {
      _silentRedisServerMock.Save();

      Assert.Pass();
    }

    [Test]
    public void Save_RedisServerNull_ServerMethodCalledCorrectly()
    {
      _silentRedisServerNull.Save();

      Assert.Pass();
    }

    [Test]
    public async Task SaveAsync_RedisServer_ServerMethodCalledCorrectly()
    {
      await _silentRedisServerMock.SaveAsync();

      Assert.Pass();
    }

    [Test]
    public async Task SaveAsync_RedisServerNull_ServerMethodCalledCorrectly()
    {
      await _silentRedisServerNull.SaveAsync();

      Assert.Pass();
    }

    [Test]
    public void ServerType_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.ServerType;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void ServerType_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.ServerType;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Time_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.Time();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Time_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.Time();

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task TimeAsync_RedisServer_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerMock.TimeAsync();

      Assert.That(result, Is.Null);
    }

    [Test]
    public async Task TimeAsync_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = await _silentRedisServerNull.TimeAsync();

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Version_RedisServer_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerMock.Version;

      Assert.That(result, Is.Null);
    }

    [Test]
    public void Version_RedisServerNull_ServerMethodCorrectResult()
    {
      var result = _silentRedisServerNull.Version;

      Assert.That(result, Is.Null);
    }
  }
}
