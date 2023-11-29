using Microsoft.Extensions.Logging.Abstractions;
using ViennaNET.Orm.Repositories;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NHibernate;
using NHibernate.Type;
using NUnit.Framework;
using ViennaNET.Orm.Tests.Unit.DSL;

namespace ViennaNET.Orm.Tests.Unit.Repositories
{
  [TestFixture]
  [Category("Unit")]
  [TestOf(typeof(CommandExecutor<>))]
  public class CommandExecutorTests
  {
    [Test]
    public void Execute_CommandWithParameters_QueryMethodCalled()
    {
      var sqlQuery = new Mock<ISQLQuery>();
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(sqlQuery.Object);
      var commandExecutor =
        new CommandExecutor<TestCommand>(session.Object, new NullLogger<CommandExecutor<TestCommand>>());

      commandExecutor.Execute(new TestCommand());

      sqlQuery.Verify(x => x.SetParameter("param", 12L, TypeFactory.Basic(typeof(long).FullName)));
      sqlQuery.Verify(x => x.ExecuteUpdate(), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_CommandWithParameters_QueryMethodCalled()
    {
      var sqlQuery = new Mock<ISQLQuery>();
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(sqlQuery.Object);
      var commandExecutor =
        new CommandExecutor<TestCommand>(session.Object, new NullLogger<CommandExecutor<TestCommand>>());

      await commandExecutor.ExecuteAsync(new TestCommand());

      sqlQuery.Verify(x => x.SetParameter("param", 12L, TypeFactory.Basic(typeof(long).FullName)));
      sqlQuery.Verify(x => x.ExecuteUpdateAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
  }
}