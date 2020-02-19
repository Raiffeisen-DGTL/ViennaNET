using ViennaNET.Validation.MessageFormatting;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class FluentRuleMessageFormatter : BaseValidationMessageFormatter
  {
    public FluentRuleMessageFormatter()
    {
      ForRule(FluentRule.InternalCode).ForMessage(FluentRule.MessageCode1).OverrideMessage("There is no info about an account");
      ForRule(FluentRule.InternalCode).ForMessage(FluentRule.MessageCode2).OverrideMessage("There is no account");
      ForRule(FluentRule.InternalCode).ForMessage(FluentRule.MessageCode3).OverrideMessage("A type of an action didnt setted");
      ForRule(FluentRule.InternalCode).ForMessage(FluentRule.MessageCode4).OverrideMessage("A type of an action must not be Update");
      ForRule(FluentRule.InternalCode).ForMessage(FluentRule.MessageCode5).OverrideMessage("A type of an account {0} must be CurrentInThirdpartyBank");
    }
  }
}