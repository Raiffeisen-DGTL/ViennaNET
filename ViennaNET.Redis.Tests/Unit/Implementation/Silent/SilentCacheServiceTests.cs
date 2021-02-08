using System;
using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.Redis.Implementation.Silent;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Redis.Tests.Unit.Implementation.Silent
{
  [TestFixture(Category = "Unit", TestOf = typeof(SilentCacheService))]
  public class SilentCacheServiceTests
  {
    private Mock<IRedisDatabase> _redis;
    private ISilentCacheService _cacheService;

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

      _cacheService = new SilentCacheService(redisDatabaseProvider.Object, new NullLogger<SilentCacheService>());
    }

    [Test]
    public void SetObjectLifetimeWithoutParamsIsSilent()
    {
      _cacheService.SetObject("name", "default", new TestDto());

      Assert.Pass();
    }

    [Test]
    public void SetObjectLifetimeWithoutIsOk()
    {
      var obj = new TestDto();

      _cacheService.SetObject("name", "default", obj, "string", 123, null, true, false);

      _redis.Verify(x => x.ObjectSet("name:string-123--1-0", obj, "default", default, default), Times.Once);
    }

    [Test]
    public void SetObjectExpiryWithoutParamsIsSilent()
    {
      _cacheService.SetObject("name", TimeSpan.FromMinutes(1), new TestDto());

      Assert.Pass();
    }

    [Test]
    public void SetObjectExpiryWithoutIsOk()
    {
      var obj = new TestDto();

      _cacheService.SetObject("name", TimeSpan.FromMinutes(1), obj, "string", 123, null, true, false);

      _redis.Verify(x => x.ObjectSet("name:string-123--1-0", obj, TimeSpan.FromMinutes(1), default, default), Times.Once);
    }

    [Test]
    public void TryGetObjectWithoutParamsIsSilent()
    {
      _cacheService.TryGetObject<TestDto>("name", out var obj);

      Assert.Pass();
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
