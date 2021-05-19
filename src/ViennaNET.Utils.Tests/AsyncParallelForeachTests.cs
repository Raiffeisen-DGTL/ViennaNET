using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace ViennaNET.Utils.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(AsyncParallelForeach))]
  public class AsyncParallelForeachTests
  {
    [OneTimeSetUp]
    public void SetUp()
    {
      for (var i = 0; i <= 10000; i++)
      {
        _accounts.Add(new Account { Id = i, Cba = i.ToString() });
      }
    }

    private class Account
    {
      public int Id { get; set; }
      public string? Cba { get; set; }
    }

    private readonly List<Account> _accounts = new List<Account>(10000);

    [Test]
    public async Task ParallelForeachSelectTest()
    {
      var source = Enumerable.Range(0, 10000).Select(x => x.ToString());

      var result = await AsyncParallelForeach.SelectAsync(source, GetAccount, 4);

      var orderedResult = result.OrderBy(acc => acc?.Id).ToArray();
      for (var i = 0; i < orderedResult.Length; i++)
      {
        Assert.That(orderedResult[i]?.Id, Is.EqualTo(i));
      }
    }

    [Test]
    public void ParallelForeachTestWithoutEntity()
    {
      var source = Enumerable.Range(0, 4000).Select(x => x.ToString());

      Assert.DoesNotThrow(() => AsyncParallelForeach.ForEachAsync(source, Highload, 4));
    }

    private async Task<Account?> GetAccount(string source)
    {
      return await Task.Run(() => _accounts.FirstOrDefault(a => a.Cba == source));
    }

    private Task Highload(string source)
    {
      return Task.Run(() => Math.Pow(int.Parse(source), 2));
    }
  }
}