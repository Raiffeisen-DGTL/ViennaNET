using System;
using ViennaNET.Validation.Rules.FluentRule;

namespace ViennaNET.Validation.Tests.RuleValidation.When
{
  class WhenFluentRule : BaseFluentRule<Tuple<bool, bool, bool>>
  {
    public WhenFluentRule()
    {
      ForProperty(a => a.Item1)
        .Equal(true)
        .WithErrorMessage("Error");
      ForProperty(a => a.Item2)
          .Equal(true)
          .WithWarningMessage("Warning");
      ForProperty(a => a.Item3)
          .Equal(true)
          .WithInfoMessage("Info");
    }
  }
}
