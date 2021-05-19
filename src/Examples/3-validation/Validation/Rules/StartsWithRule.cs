using System.Linq;
using ValidationService.Validation.Contexts;
using ViennaNET.Validation.Rules.FluentRule;

namespace ValidationService.Validation.Rules
{
  public class StartsWithRule : BaseFluentRule<string>
  {
    public StartsWithRule()
    {
      ForProperty(x => x)
        .Must((x, c) => ((GreetingValidationContext)c).AllowedPhrases.Any(p => x.StartsWith(p)))
        .WithErrorMessage($"Greeting can begun only with allowed phrases");
    }
  }
}
