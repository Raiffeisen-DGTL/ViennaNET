using System;
using ViennaNET.Redis.Implementation.Default;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Default
{
  [TestFixture(Category = "Unit", TestOf = typeof(CacheService))]
  public class CacheServiceTests
  {
    private Mock<IRedisDatabase> _redis;
    private ICacheService _cacheService;

    private class TestDto
    {
      public string Field { get; set; }
    }

    [OneTimeSetUp]
    public void RedisDatabaseTestsSetUp()
    {
      _redis = new Mock<IRedisDatabase>();

      var redisDatabaseProvider = new Mock<IRedisDatabaseProvider>();
      redisDatabaseProvider.Setup(x => x.GetDatabase(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<object>()))
                           .Returns(_redis.Object);

      _cacheService = new CacheService(redisDatabaseProvider.Object);
    }

    [Test]
    public void SetObjectLifetimeWithoutParamsIsException()
    {
      Assert.That(() =>
      {
        _cacheService.SetObject("name", "default", new TestDto());
      }, Throws.ArgumentException);
    }

    [Test]
    public void SetObjectLifetimeWithoutIsOk()
    {
      var obj = new TestDto();

      _cacheService.SetObject("name", "default", obj, "string", 123, null, true, false);

      _redis.Verify(x => x.ObjectSet("name:string-123--1-0", obj, "default", default, default), Times.Once);
    }

    [Test]
    public void SetObjectExpiryWithoutParamsIsException()
    {
      Assert.That(() =>
      {
        _cacheService.SetObject("name", TimeSpan.FromMinutes(1), new TestDto());
      }, Throws.ArgumentException);
    }

    [Test]
    public void SetObjectExpiryWithoutIsOk()
    {
      var obj = new TestDto();

      _cacheService.SetObject("name", TimeSpan.FromMinutes(1), obj, "string", 123, null, true, false);

      _redis.Verify(x => x.ObjectSet("name:string-123--1-0", obj, TimeSpan.FromMinutes(1), default, default), Times.Once);
    }

    [Test]
    public void TryGetObjectWithoutParamsIsException()
    {
      Assert.That(() =>
      {
        _cacheService.TryGetObject<TestDto>("name", out var obj);
      }, Throws.ArgumentException);
    }

    [Test]
    public void TryGetObjectIsOk()
    {
      _redis.Reset();
      _redis.Setup(x => x.ObjectGet<TestDto>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .Returns(new TestDto { Field = "Text" });

      var actual = _cacheService.TryGetObject<TestDto>("name", out var obj, "string", 123, null, true, false);

      Assert.IsTrue(actual);
      Assert.AreEqual("Text", obj.Field);
      _redis.Verify(x => x.ObjectGet<TestDto>("name:string-123--1-0", default), Times.Once);
    }

    [Test]
    public void TryGetObjectIsNotFound()
    {
      _redis.Reset();
      _redis.Setup(x => x.ObjectGet<TestDto>(It.IsAny<string>(), It.IsAny<CommandFlags>()))
            .Returns((TestDto)null);

      var actual = _cacheService.TryGetObject<TestDto>("name", out var obj, "string", 123, null, true, false);

      Assert.IsFalse(actual);
      Assert.AreEqual(default, obj);
      _redis.Verify(x => x.ObjectGet<TestDto>("name:string-123--1-0", default), Times.Once);
    }
  }
}
