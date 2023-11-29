using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;
using ViennaNET.Redis.Implementation.Silent;
using RedisException = ViennaNET.Redis.Exceptions.RedisException;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Silent
{
  [TestFixture(Category = "Unit", TestOf = typeof(SilentRedisDatabase))]
  public class SilentRedisDatabaseTests
  {
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

    private SilentRedisDatabase _silentRedisDatabaseNull;
    private SilentRedisDatabase _silentRedisDatabaseMock;

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
    public async Task ObjectSetAsync_DictionaryOverloadRedisDatabaseNull_WithoutException()
    {
      await _silentRedisDatabaseNull.ObjectSetAsync(new Dictionary<string, object>());

      Assert.Pass();
    }

    [Test]
    public async Task ObjectSetAsync_TimespanOverloadRedisDatabaseNull_WithoutException()
    {
      await _silentRedisDatabaseNull.ObjectSetAsync("key", new object(), TimeSpan.FromSeconds(1));

      Assert.Pass();
    }

    [Test]
    public async Task ObjectSetAsync_LifetimeOverloadRedisDatabaseNull_WithoutException()
    {
      await _silentRedisDatabaseNull.ObjectSetAsync("key", new object(), "lifetime");

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

    [Test]
    public void HashObjectGet_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashObjectGet<object>("key", "field");

      Assert.Pass();
    }

    [Test]
    public async Task HashObjectGetAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashObjectGetAsync<object>("key", "field");

      Assert.Pass();
    }

    [Test]
    public void HashObjectGetList_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashObjectGet<object>("key", new[] { "field", "field1" });

      Assert.Pass();
    }

    [Test]
    public async Task HashObjectGetListAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashObjectGetAsync<object>("key", new[] { "field", "field1" });

      Assert.Pass();
    }

    [Test]
    public void HashObjectGetAll_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashObjectGetAll<object>("key");

      Assert.Pass();
    }

    [Test]
    public async Task HashObjectGetAllAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashObjectGetAllAsync<object>("key");

      Assert.Pass();
    }

    [Test]
    public void HashObjectSet_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashObjectSet("key", "field", null);

      Assert.Pass();
    }

    [Test]
    public async Task HashObjectSetAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashObjectSetAsync("key", "field", null);

      Assert.Pass();
    }

    [Test]
    public void HashObjectSetList_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashObjectSet("key",
        new Dictionary<string, object> { { "field", null }, { "field1", null } });

      Assert.Pass();
    }

    [Test]
    public async Task HashObjectSetListAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashObjectSetAsync("key",
        new Dictionary<string, object> { { "field", null }, { "field1", null } });

      Assert.Pass();
    }

    [Test]
    public void HashStringGet_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashStringGet("key", "field");

      Assert.Pass();
    }

    [Test]
    public async Task HashStringGetAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashStringGetAsync("key", "field");

      Assert.Pass();
    }

    [Test]
    public void HashStringSet_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashStringSet("key", "field", "value");

      Assert.Pass();
    }

    [Test]
    public async Task HashStringSetAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashStringSetAsync("key", "field", "value");

      Assert.Pass();
    }

    [Test]
    public void HashStringSetList_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashStringSet("key",
        new Dictionary<string, string> { { "field", "value" }, { "field1", "value" } });

      Assert.Pass();
    }

    [Test]
    public async Task HashStringSetListAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashStringSetAsync("key",
        new Dictionary<string, string> { { "field", "value" }, { "field1", "value" } });

      Assert.Pass();
    }

    [Test]
    public void HashDelete_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashDelete("key", "field");

      Assert.Pass();
    }

    [Test]
    public async Task HashDeleteAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashDeleteAsync("key", "field");

      Assert.Pass();
    }

    [Test]
    public void HashDeleteList_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashDelete("key", new[] { "field", "field1" });

      Assert.Pass();
    }

    [Test]
    public async Task HashDeleteListAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashDeleteAsync("key", new[] { "field", "field1" });

      Assert.Pass();
    }

    [Test]
    public async Task HashStringGetListAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashStringGetAsync("key", new[] { "field", "field1" });

      Assert.Pass();
    }

    [Test]
    public void HashStringGetList_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashStringGet("key", new[] { "field", "field1" });

      Assert.Pass();
    }

    [Test]
    public async Task HashStringGetAllAsync_RedisDatabaseThrowsException_WithoutException()
    {
      await _silentRedisDatabaseMock.HashStringGetAllAsync("key");

      Assert.Pass();
    }

    [Test]
    public void HashStringGetAll_RedisDatabaseThrowsException_WithoutException()
    {
      _silentRedisDatabaseMock.HashStringGetAll("key");

      Assert.Pass();
    }

    [Test]
    public async Task ObjectGetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.ObjectGetAsync<object>("key", CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.ObjectGetAsync<object>("key");

      db.Verify(x => x.ObjectGetAsync<object>("key", CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task StringGetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.StringGetAsync("key", CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.StringGetAsync("key");

      db.Verify(x => x.StringGetAsync("key", CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task HashStringGetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.HashStringGetAsync("key", "field", CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.HashStringGetAsync("key", "field");

      db.Verify(x => x.HashStringGetAsync("key", "field", CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task StringSetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.StringSetAsync("key", "value", It.IsAny<TimeSpan?>(), It.IsAny<When>(), CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.StringSetAsync("key", "value");

      db.Verify(x => x.StringSetAsync("key", "value", It.IsAny<TimeSpan?>(), It.IsAny<When>(), CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task ObjectSetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.ObjectSetAsync("key", "value", It.IsAny<TimeSpan?>(), It.IsAny<When>(), It.IsAny<CommandFlags>()))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.ObjectSetAsync("key", "value");

      db.Verify(x => x.ObjectSetAsync("key", "value", It.IsAny<TimeSpan?>(), It.IsAny<When>(), CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task HashStringSetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.HashStringSetAsync("key", "field", "value", It.IsAny<When>(), CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.HashStringSetAsync("key", "field", "value");

      db.Verify(x => x.HashStringSetAsync("key", "field", "value", It.IsAny<When>(), CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task HashObjectSetAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.HashObjectSetAsync("key", "field", "value", It.IsAny<When>(), CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.HashObjectSetAsync("key", "field", "value");

      db.Verify(x => x.HashObjectSetAsync("key", "field", "value", It.IsAny<When>(), CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task HashObjectSetAsync_DictionaryHasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.HashObjectSetAsync("key", It.IsAny<IDictionary<string, object>>(), CommandFlags.None))
        .ThrowsAsync(new ArithmeticException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.HashObjectSetAsync("key", new Dictionary<string, object>());

      db.Verify(x => x.HashObjectSetAsync("key", It.IsAny<IDictionary<string, object>>(), CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task KeyDeleteAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.KeyDeleteAsync("key", CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.KeyDeleteAsync("key");

      db.Verify(x => x.KeyDeleteAsync("key", CommandFlags.None));
      Assert.Pass();
    }

    [Test]
    public async Task HashDeleteAsync_HasDbException_ResultWithoutException()
    {
      var db = new Mock<IRedisDatabase>();
      db.Setup(x => x.HashDeleteAsync("key", "field", CommandFlags.None))
        .ThrowsAsync(new ArgumentException())
        .Verifiable();

      var silentDb = new SilentRedisDatabase(db.Object, new NullLogger<SilentRedisDatabase>());

      await silentDb.HashDeleteAsync("key", "field");

      db.Verify(x => x.HashDeleteAsync("key", "field", CommandFlags.None));
      Assert.Pass();
    }
  }
}