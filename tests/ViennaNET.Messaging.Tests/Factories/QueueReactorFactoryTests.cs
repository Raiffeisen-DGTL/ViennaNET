using Moq;
using NUnit.Framework;
using ViennaNET.Messaging.Configuration;
using ViennaNET.Messaging.Exceptions;
using ViennaNET.Messaging.KafkaQueue;
using ViennaNET.Messaging.Processing.Impl;
using ViennaNET.Messaging.Processing.Impl.Poll;
using ViennaNET.Messaging.Processing.Impl.Subscribe;
using ViennaNET.Messaging.Tests.DSL;

namespace ViennaNET.Messaging.Tests.Factories;

[TestFixture(Category = "Unit", TestOf = typeof(QueueReactorFactory))]
public class QueueReactorFactoryTests
{
    public static readonly object[] CreateQueueReactor_AdapterType_CorrectReactorReturned_Source =
    {
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.ThreadStrategy, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.ThreadStrategy)
                .Please<IMessageAdapterWithTransactions>(),
            typeof(QueueTransactedPollingReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.Subscribe, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.Subscribe)
                .Please<IMessageAdapterWithTransactions>(),
            typeof(QueueTransactedPollingReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.SubscribeAndReply, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.SubscribeAndReply)
                .Please<IMessageAdapterWithTransactions>(),
            typeof(QueueTransactedPollingReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.Subscribe, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.Subscribe)
                .Please<IMessageAdapterWithSubscribing>(),
            typeof(QueueSubscribedReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.SubscribeAndReply, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.SubscribeAndReply)
                .Please<IMessageAdapterWithSubscribing>(),
            typeof(QueueSubscribeAndReplyReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.Subscribe, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.Subscribe)
                .Please(),
            typeof(QueuePollingReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.SubscribeAndReply, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.SubscribeAndReply)
                .Please(),
            typeof(QueuePollingReactor)
        },
        new object[]
        {
            Given.MessageAdapter
                .WithQueueConfiguration(new KafkaQueueConfiguration
                {
                    ProcessingType = MessageProcessingType.ThreadStrategy, IntervalPollingQueue = 1000
                })
                .SupportsProcessingType(MessageProcessingType.ThreadStrategy)
                .Please(),
            typeof(QueuePollingReactor)
        }
    };

    [Test]
    [TestCaseSource(nameof(CreateQueueReactor_AdapterType_CorrectReactorReturned_Source))]
    public void CreateQueueReactor_AdapterType_CorrectReactorReturned(IMessageAdapter adapter, Type reactorType)
    {
        const string queueName = "ReName";
        var messageProcessor = Mock.Of<IMessageProcessor>();
        var queueReactorFactory = Given.QueueReactorFactory
            .WithMessageProcessor(messageProcessor, queueName)
            .WithMessageAdapter(adapter)
            .Please();

        var result = queueReactorFactory.CreateQueueReactor(queueName);

        Assert.That(result, Is.InstanceOf(reactorType));
    }

    [Test]
    public void CreateQueueReactor_NoProcessorForQueue_ThrowsException()
    {
        const string queueId = "ReName";
        const string messagePart = "There are no message processors registered for queue";
        var queueReactorFactory = Given.QueueReactorFactory.Please();

        Assert.That(() => queueReactorFactory.CreateQueueReactor(queueId),
            Throws.InstanceOf<MessagingException>().And.Message.Contains(messagePart));
    }

    [Test]
    public void CreateQueueReactor_ProcessorNotRegisteredInFactory_ThrowsException()
    {
        const string queueId = "ReName";
        const string messagePart = "are not registered in the queue factory";
        var messageProcessor = Mock.Of<IMessageProcessor>();
        var messageProcessorUnreg = Mock.Of<IMessageProcessorAsync>();
        var queueReactorFactory = Given
            .QueueReactorFactory
            .WithMessageProcessor(messageProcessor, queueId)
            .WithMessageProcessorAsync(messageProcessorUnreg)
            .Please();

        Assert.That(() => queueReactorFactory.CreateQueueReactor(queueId),
            Throws.InstanceOf<MessagingException>().And.Message.Contains(messagePart));
    }

    [Test]
    public void Register_QueueName_Success()
    {
        const string queueName = "ReQueueName";
        var queueReactorFactory = Given.QueueReactorFactory.Please();

        var result = queueReactorFactory.Register<object>(queueName);

        Assert.That(result, Is.Not.Null);
    }

    [Test]
    public void Register_SameReactorTwice_ThrowException()
    {
        const string queueName = "ReQueueName";
        var queueReactorFactory = Given.QueueReactorFactory.Please();
        queueReactorFactory.Register<object>(queueName);

        Assert.Throws<MessageProcessorAlreadyRegisterException>(() => queueReactorFactory.Register<object>(queueName));
    }
}