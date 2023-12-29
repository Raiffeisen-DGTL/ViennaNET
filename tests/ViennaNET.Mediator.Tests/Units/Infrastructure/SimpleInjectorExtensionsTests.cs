using NUnit.Framework;
using NUnit.Framework.Legacy;
using SimpleInjector;
using ViennaNET.Mediator.DefaultConfiguration;
using ViennaNET.Mediator.Tests.Fake;
using ViennaNET.Mediator.Tests.Fake.Handlers;

namespace ViennaNET.Mediator.Tests.Units.Infrastructure
{
    [TestFixture(Category = "Unit", TestOf = typeof(SimpleInjectorExtensions))]
    internal class SimpleInjectorExtensionsTests
    {
        [Test]
        public void RegisterHandler_ReturnValue_ShouldRegister()
        {
            var container = new Container();

            container.RegisterHandler<Request, int, RequestHandler>();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler<Request, int>>());
            });
        }

        [Test]
        public void RegisterHandler_ShouldRegister()
        {
            var container = new Container();

            container.RegisterHandler<Event, EventListener>();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler<Event>>());
            });
        }

        [Test]
        public void RegisterAsyncHandler_ReturnValue_ShouldRegister()
        {
            var container = new Container();

            container.RegisterAsyncHandler<Request, int, AsyncRequestHandler>();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync<Request, int>>());
            });
        }

        [Test]
        public void RegisterAsyncHandler_ShouldRegister()
        {
            var container = new Container();

            container.RegisterAsyncHandler<Event, AsyncEventListener>();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync<Event>>());
            });
        }

        [Test]
        public void RegisterFullHandler_ReturnValue_ShouldRegister()
        {
            var container = new Container();

            container.RegisterFullHandler<Request, int, FullRequestHandler>();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler<Request, int>>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync<Request, int>>());
            });
        }

        [Test]
        public void RegisterFullHandler_ShouldRegister()
        {
            var container = new Container();

            container.RegisterFullHandler<Event, FullEventListener>();

            Assert.Multiple(() =>
            {
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandler<Event>>());
                CollectionAssert.IsNotEmpty(container.GetAllInstances<IMessageHandlerAsync<Event>>());
            });
        }
    }
}