using System;
using System.Linq;
using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.Diagnostic.Data;
using ViennaNET.Redis;
using ViennaNET.Redis.Diagnostic;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace ViennaNET.Cache.Redis.Tests.Unit.Diagnostic
{
  [TestFixture(Category = "Unit"), TestOf(typeof(RedisConnectionChecker))]
  public class RedisConnectionCheckerTests
  {
    private const string redisKey = "redis";
    private const string localhost = "localhost";

    [Test]
    public void Diagnose_CorrectConnection_CorrectResult()
    {
      var checker = CreateRedisConnectionChecker();

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.Name, Is.EqualTo(redisKey));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.Ok));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(localhost));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.True);
    }

    [Test]
    public void Diagnose_ErrorWhileConnection_FailedResult()
    {
      const string error = "redis error";
      var provider = new Mock<IRedisDatabaseProvider>();
      provider.Setup(x => x.GetDatabase(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<object>()))
              .Throws(new InvalidOperationException(error));
      var checker = new RedisConnectionChecker(provider.Object, new NullLogger<RedisConnectionChecker>());

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error.Contains(error));
      Assert.That(diagnosticInfo.Name, Is.EqualTo(redisKey));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.DbConnectionError));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(localhost));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.True);
    }

    [Test]
    public void Diagnose_FalseWhileSetString_FailedResult()
    {
      const string error = "Cannot save value to redis database";
      var checker = CreateRedisConnectionChecker(false);

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error.Contains(error));
      Assert.That(diagnosticInfo.Name, Is.EqualTo(redisKey));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.DbConnectionError));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(localhost));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.True);
    }

    [Test]
    public void Diagnose_FalseWhileDeleteString_FailedResult()
    {
      const string error = "Cannot delete value from redis database";
      var checker = CreateRedisConnectionChecker(true, false);

      var result = checker.Diagnose()
                          .GetAwaiter()
                          .GetResult();

      var diagnosticInfo = result.First();
      Assert.That(diagnosticInfo.Error.Contains(error));
      Assert.That(diagnosticInfo.Name, Is.EqualTo(redisKey));
      Assert.That(diagnosticInfo.Status, Is.EqualTo(DiagnosticStatus.DbConnectionError));
      Assert.That(diagnosticInfo.Url, Is.EqualTo(localhost));
      Assert.That(diagnosticInfo.Version, Is.EqualTo(string.Empty));
      Assert.That(diagnosticInfo.IsSkipResult, Is.True);
    }

    private static RedisConnectionChecker CreateRedisConnectionChecker(bool setResult = true, bool deleteResult = true)
    {
      var database = new Mock<IRedisDatabase>();
      database.Setup(x => x.StringSet(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<TimeSpan?>(), It.IsAny<When>(),
                                      It.IsAny<CommandFlags>(), It.IsAny<bool>()))
              .Returns(setResult);
      database.Setup(x => x.KeyDelete(It.IsAny<string>(), It.IsAny<CommandFlags>(),It.IsAny<bool>()))
              .Returns(deleteResult);
      var provider = new Mock<IRedisDatabaseProvider>();
      provider.Setup(x => x.GetDatabase(It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<object>()))
              .Returns(database.Object);
      var checker = new RedisConnectionChecker(provider.Object, new NullLogger<RedisConnectionChecker>());
      return checker;
    }

    [Test]
    public void Key_CorrectlyFilled()
    {
      var provider = new Mock<IRedisDatabaseProvider>();
      var checker = new RedisConnectionChecker(provider.Object, new NullLogger<RedisConnectionChecker>());

      Assert.That(checker.Key, Is.EqualTo(redisKey));
    }
  }
}
