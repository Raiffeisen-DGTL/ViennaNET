using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using ViennaNET.Extensions.Mediator.DependencyInjection;
using ViennaNET.Extensions.Mediator.Tests.Handlers;
using ViennaNET.Extensions.Mediator.Tests.Models;

namespace ViennaNET.Extensions.Mediator.Tests.DependencyInjection;

[Category("Integration")]
[TestOf(typeof(MediatorServiceCollectionExtensions))]
public class MediatorServiceCollectionExtensionsTests
{
#nullable disable
    private IServiceCollection _services;
#nullable restore
    [SetUp]
    public void Setup()
    {
        _services = new ServiceCollection();
    }

    [Test]
    [Category("Unit")]
    public void AddMediator_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => default(IServiceCollection)!.AddMediator());
    }

    [Test]
    [Category("Unit")]
    public void Generic_AddMediator_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => default(IServiceCollection)!.AddMediator<Mediator>());
    }


    [Test]
    public void AddMediator_InstanceOfType_IMediator_Resolved()
    {
        Assert.That(_services.AddMediator().BuildServiceProvider().GetRequiredService<IMediator>(), Is.Not.Null);
    }

    [Test]
    [Category("Unit")]
    public void TryAddHandler_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => default(IServiceCollection)!.TryAddHandler<MessageHandler>());
    }

    [Test]
    public void TryAddHandler_Adds_Handler_Only_Once()
    {
        Assert.That(() => _services
                .TryAddHandler<MessageHandler>()
                .TryAddHandler<MessageHandler>()
                .SingleOrDefault(descriptor =>
                    descriptor.ImplementationType == typeof(MessageHandler)),
            Is.Not.Null);
    }

    [Test]
    public void Resolve_Mediator_With_Dependencies()
    {
        var provider = _services.AddMediator()
            .TryAddHandler<MessageHandler>()
            .TryAddHandler<MessageHandlerWithResult>()
            .BuildServiceProvider(true);

        var mediator = provider.CreateScope().ServiceProvider.GetRequiredService<IMediator>();

        Assert.Multiple(() =>
        {
            Assert.That(async () => await mediator.SendAsync(new Message(1)), Throws.Nothing);
            Assert.That(async () => await mediator.SendAsync<Message, int>(new Message(1)), Throws.Nothing);
        });
    }

    [Test]
    [Category("Unit")]
    public void AddHandlers_Throw_ArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => default(IServiceCollection)!.AddHandlers());
    }

    [Test]
    [Category("Unit")]
    public void AddHandlersFromAssemblies_Throw_ArgumentNullException()
    {
        Assert.Multiple(() =>
        {
            Assert.Throws<ArgumentNullException>(() =>
                default(IServiceCollection)!.AddHandlersFromAssemblies(Array.Empty<Assembly>()));
            Assert.Throws<ArgumentNullException>(() => _services.AddHandlersFromAssemblies(null!));
        });
    }

    [Test]
    [Category("Unit")]
    public void AddHandlersFromAssemblies_Ignore_Abstract_And_Generic_Types()
    {
        Assert.That(() =>
                _services.AddHandlersFromAssemblies(new[] { Assembly.GetExecutingAssembly() })
                    .Select(descriptor => descriptor.ImplementationType),
            Is.EquivalentTo(new[]
            {
                typeof(IncrementEventHandler), typeof(MessageHandler),
                typeof(MessageHandlerWithResult), typeof(MultiplicationEventHandler)
            }).And.Not.AnyOf(typeof(AbstractTypeForTestAddHandlersFromAssemblies),
                typeof(AbstractTypeForTestAddHandlersFromAssemblies<>)));
    }

    [Test]
    public void AddHandlers_Adds_All_Handlers_From_Current_Assembly()
    {
        Assert.That(() =>
                _services.AddHandlers().Select(descriptor => descriptor.ImplementationType),
            Is.EquivalentTo(new[]
            {
                typeof(IncrementEventHandler), typeof(MessageHandler),
                typeof(MessageHandlerWithResult), typeof(MultiplicationEventHandler)
            }));
    }
}