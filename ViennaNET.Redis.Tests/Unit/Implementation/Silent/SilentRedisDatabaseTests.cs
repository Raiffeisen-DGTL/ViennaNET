using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.Redis.Implementation.Silent;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using RedisException = ViennaNET.Redis.Exceptions.RedisException;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Silent
{
  [TestFixture(Category = "Unit", TestOf = typeof(SilentRedisDatabase))]
  public class SilentRedisDatabaseTests
  {
    private SilentRedisDatabase _silentRedisDatabaseNull;
    private SilentRedisDatabase _silentRedisDatabaseMock;

    [OneTimeSetUp]
    public void SilentRedisDatabaseTestsSetUp()
    {
      _silentRedisDatabaseNull = new SilentRedisDatabase(null, null);

      var exceptionMock = new Mock<IRedisDatabase>();
      exceptionMock.Setup(x => x.StringGet(It.IsAny<string>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());
      exceptionMock.Setup(x => x.StringSet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(),
                                           It.IsAny<When>(), It.IsAny<CommandFlags>()))
                   .Throws(new RedisException());

      _silentRedisDatabaseMock = new SilentRedisDatabase(exceptionMock.Object, new NullLogger<SilentRedisDatabase>());
    }

    [Test]
    public void StringGet_RedisDatabaseNull_WithoutException()
    {
      _silentRedisDatabaseNull.StringGet("key");

      Assert.Pass();
    }

    [Test]
    public void StringGet_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.StringGet("key");

      Assert.Pass();
    }

    [Test]
    public void StringSet_RedisDatabaseNull_WithoutException()
    {
      _silentRedisDatabaseNull.StringSet("key", "value", "test");

      Assert.Pass();
    }

    [Test]
    public void StringSet_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.StringSet("key", "value", "test");

      Assert.Pass();
    }

    [Test]
    public void KeyDelete_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.KeyDelete("key");

      Assert.Pass();
    }

    [Test]
    public void KeyDeleteList_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.KeyDelete(new List<string> { "key", "key1" });

      Assert.Pass();
    }

    [Test]
    public async Task KeyDeleteAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.KeyDeleteAsync("key");

      Assert.Pass();
    }

    [Test]
    public async Task KeyDeleteAsyncList_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.KeyDeleteAsync(new List<string> { "key", "key1" });

      Assert.Pass();
    }
  }
}
