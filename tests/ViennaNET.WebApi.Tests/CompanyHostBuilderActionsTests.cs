using Microsoft.Extensions.Configuration;
using Moq;

namespace ViennaNET.WebApi.Tests;

public class CompanyHostBuilderActionsTests
{
    [Test]
    public void RegisterConfigurationBuilderAction_Action_InvokeAction()
    {
        // Arrange
        var builder = CompanyHostBuilder.Create();
        var actionMock = new Mock<Action<IConfigurationBuilder>>();
        builder.RegisterConfigurationBuilderAction(actionMock.Object);

        // Act
        builder.BuildWebHost(Array.Empty<string>());

        // Assert
        actionMock.Verify(x => x.Invoke(It.IsAny<IConfigurationBuilder>()), Times.Once);
    }
}