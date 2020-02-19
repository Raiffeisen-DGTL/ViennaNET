using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Company.Utils.Tests
{
  [TestFixture(Category = "Unit", TestOf = (typeof(AsyncParallelForeach)))]
  public class AsyncParallelForeachTests
  {
    private class Account
    {
      public int id { get; set; }
      public string cba { get; set; }
    }

    private List<Account> accounts;

    [OneTimeSetUp]
    public void SetUp()
    {
      accounts = new List<Account>(10000);
      for (int i = 0; i <= 10000; i++)
      {
        accounts.Add(new Account { id = i, cba = i.ToString() });
      }
    }

    [Test]
    public async Task ParallelForeachSelectTest()
    {
      var source = Enumerable.Range(0, 10000).Select(x => x.ToString());

      var result = await AsyncParallelForeach.SelectAsync(source, GetAccount, 4);

      var orderedResult = result.OrderBy(acc => acc.id).ToArray();
      for (int i = 0; i < orderedResult.Length; i++)
      {
        Assert.That(orderedResult[i].id, Is.EqualTo(i));
      }
    }

    [Test]
    public void ParallelForeachTestWithoutEntity()
    {
      var source = Enumerable.Range(0, 4000).Select(x => x.ToString());

      Assert.DoesNotThrow(() => AsyncParallelForeach.ForEachAsync(source, Highload, 4));
    }

    private async Task<Account> GetAccount(string source)
    {
      return await Task.Run(() => accounts.Where(a => a.cba == source).FirstOrDefault());
    }

    private async Task Highload(string source)
    {
      await Task.Run(() => Math.Pow(int.Parse(source),2));
    }
  }
}
