using NUnit.Framework;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqConnectionFactory))]
  public class ActiveMqConnectionFactoryTests
  {
    [Test]
    public void CannectionFactoryTest()
    {
      var provider = new ActiveMqConnectionFactory();

      var factory = provider.GetConnectionFactory("localhost", default);

      Assert.IsNotNull(factory);
    }
  }
}