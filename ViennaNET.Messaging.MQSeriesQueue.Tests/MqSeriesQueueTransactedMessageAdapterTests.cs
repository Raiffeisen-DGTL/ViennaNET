using IBM.XMS;
using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.MQSeriesQueue.Tests.DSL;

namespace ViennaNET.Messaging.MQSeriesQueue.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(MqSeriesQueueTransactedMessageAdapter))]
  public class MqSeriesQueueTransactedMessageAdapterTests
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

      provider.ConnectionMock.Verify(x => x.CreateSession(false, It.IsAny<AcknowledgeMode>()));
    }

    [Test]
    public void CreateTransactedSessionIfTransactedTest()
    {
      var provider = Given.ConnectionFactoryProvider;
      var adapter = Given.TransactedMessageAdapter.Transacted().WithConnectionFactoryProvider(provider.Please())
        .Please();

      adapter.Connect();
      adapter.TryReceive(out var msg);

      provider.ConnectionMock.Verify(x => x.CreateSession(true, It.IsAny<AcknowledgeMode>()));
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