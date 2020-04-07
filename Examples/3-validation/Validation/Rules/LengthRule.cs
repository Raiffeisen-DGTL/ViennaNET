using ViennaNET.Validation.Rules.FluentRule;

namespace ValidationService.Validation.Rules
{
  public class LengthRule: BaseFluentRule<string>
  {
    private const int MaxGreetingLength = 30;
    private const int MinGreetingLength = 2;

    public LengthRule()
    {
      ForProperty(x => x)
        .Length(MinGreetingLength, MaxGreetingLength)
        .WithErrorMessage($"Lenght of greeting can not be smaller then {MinGreetingLength} and greater then {MaxGreetingLength}");
    }
  }
}
