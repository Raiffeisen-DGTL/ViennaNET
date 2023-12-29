using System.Diagnostics.CodeAnalysis;
using ViennaNET.Mediator.Seedwork;
using ViennaNET.Validation.Exceptions;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;

namespace ViennaNET.Mediator.Pipelines.Behaviors;

[ExcludeFromCodeCoverage(Justification = "Будет удалён в ближайшем рефакторинге")]
public class MessageValidationBehavior : IBroadcastPreProcessor
{
    private readonly Dictionary<Type, IMessageValidation> _rules = new();

    public MessageValidationBehavior(IEnumerable<IMessageValidation> rules)
    {
        foreach (var rule in rules)
        {
            var baseType = rule.GetType().BaseType;
            if (baseType == null || !baseType.IsGenericType)
            {
                throw new InvalidOperationException($"Base type of rule '{nameof(rule)}' is null or not generic type.");
            }

            _rules.Add(baseType.GetGenericArguments()[0], rule);
        }
    }

    public Task ProcessAsync(IMessage message, CancellationToken cancellationToken)
    {
        Process(message);
        return Task.CompletedTask;
    }

    public void Process(IMessage message)
    {
        var messageType = message.GetType();
        if (!_rules.TryGetValue(messageType, out var rule))
        {
            return;
        }

        var result = rule.Validate(message);
        if (!result.IsValid)
        {
            throw new MessageValidationException(result.Results.ToErrorsString());
        }
    }
}