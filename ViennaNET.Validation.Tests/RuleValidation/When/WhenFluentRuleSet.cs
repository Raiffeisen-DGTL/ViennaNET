using System;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Validators;
using Moq;

namespace ViennaNET.Validation.Tests.RuleValidation.When
{
  internal class WhenFluentRuleSet : BaseValidationRuleSet<Tuple<bool, bool, bool>>
  {
    public WhenFluentRuleSet(int number)
    {
      var errorRule = new Mock<IRule<Tuple<bool, bool, bool>>>();
      errorRule.Setup(x => x.Validate(It.IsAny<Tuple<bool, bool, bool>>(), It.IsAny<ValidationContext>()))
               .Throws(new WhenException("error"));
      errorRule.Setup(x => x.Identity)
               .Returns(new RuleIdentity("ERRULE"));
      switch (number)
      {
        case 0:
          When(new WhenFluentRule(), () =>
          {
            SetRule(errorRule.Object);
          });
          break;
        case 1:
          WhenNoInfos(new WhenFluentRule(), () =>
          {
            SetRule(errorRule.Object);
          });
          break;
        case 2:
          WhenNoInfosAndErrors(new WhenFluentRule(), () =>
          {
            SetRule(errorRule.Object);
          });
          break;
        case 3:
          WhenNoInfosAndWarnings(new WhenFluentRule(), () =>
          {
            SetRule(errorRule.Object);
          });
          break;
        case 4:
          WhenNoWarnings(new WhenFluentRule(), () =>
          {
            SetRule(errorRule.Object);
          });
          break;
        case 5:
          WhenNoWarningsAndErrors(new WhenFluentRule(), () =>
          {
            SetRule(errorRule.Object);
          });
          break;
        default:
          throw new ArgumentException();
      }
    }
  }
}
