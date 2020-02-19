using System.Collections.Generic;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;

namespace ViennaNET.Validation.MessageFormatting
{
  /// <inheritdoc />
  public abstract class BaseValidationMessageFormatter : IValidationMessageFormatter
  {
    private readonly IList<MessageFormatRule> _formatRules = new List<MessageFormatRule>();

    /// <inheritdoc />
    public ValidationResult Format(ValidationResult result)
    {
      foreach (var rule in _formatRules)
      {
        rule.Format(result);
      }
      return result;
    }

    protected MessageFormatRule ForRule(string code)
    {
      var rule = new MessageFormatRule(new RuleIdentity(code));
      _formatRules.Add(rule);
      return rule;
    }
  }
}
