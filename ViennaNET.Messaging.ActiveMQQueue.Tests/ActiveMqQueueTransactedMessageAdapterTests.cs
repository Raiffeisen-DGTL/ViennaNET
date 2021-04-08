using Apache.NMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.ActiveMQQueue.Tests.DSL;

namespace ViennaNET.Messaging.ActiveMQQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(ActiveMqQueueTransactedMessageAdapter))]
  public class ActiveMqQueueTransactedMessageAdapterTests
  {
    [Test]
    public void CommitIfTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.Transacted().WithConnectionFactoryProvider(provider.Please())
        .Please();

      adapter.Connect();
      adapter.CommitIfTransacted(null);

      provider.SessionMock.Verify(x => x.Commit());
    }

    [Test]
    public void CreateNonTransactedSessionIfNotTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.ConnectionMock.Verify(x => x.CreateSession(It.IsAny<AcknowledgementMode>()));
    }

    [Test]
    public void CreateTransactedSessionIfTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.Transacted().WithConnectionFactoryProvider(provider.Please())
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.ConnectionMock.Verify(x => x.CreateSession(It.IsAny<AcknowledgementMode>()));
    }

    [Test]
    public void NotCommitIfNotTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

      adapter.Connect();
      adapter.CommitIfTransacted(null);

      provider.SessionMock.Verify(x => x.Commit(), Times.Never);
    }

    [Test]
    public void NotRollbackIfNotTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.WithConnectionFactoryProvider(provider.Please()).Please();

      adapter.Connect();
      adapter.RollbackIfTransacted();

      provider.SessionMock.Verify(x => x.Rollback(), Times.Never);
    }

    [Test]
    public void RollbackIfTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.Transacted().WithConnectionFactoryProvider(provider.Please())
        .Please();

      adapter.Connect();
      adapter.RollbackIfTransacted();

      provider.SessionMock.Verify(x => x.Rollback());
    }
  }
}