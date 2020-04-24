using ViennaNET.Orm.Repositories;
using Moq;
using NHibernate;
using NHibernate.Type;
using NUnit.Framework;

namespace ViennaNET.Orm.Tests.Unit.Repositories
{
  [TestFixture, Category("Unit"), TestOf(typeof(CommandExecutor<>))]
  public class CommandExecutorTests
  {
    [Test]
    public void Execute_CommandWithParameters_QueryMethodCalled()
    {
      var sqlQuery = new Mock<ISQLQuery>();
      var session = new Mock<ISession>();
      session.Setup(x => x.CreateSQLQuery(It.IsAny<string>())).Returns(sqlQuery.Object);
      var commandExecutor = new CommandExecutor<TestCommand>(session.Object);

      commandExecutor.Execute(new TestCommand());

      sqlQuery.Verify(x => x.SetParameter("param", 12L, TypeFactory.Basic(typeof(long).FullName)));
      sqlQuery.Verify(x => x.ExecuteUpdate(), Times.Once);
    }
  }
}
