using System;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Validators;
using Moq;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class DependsOnWithErrorRuleSet : BaseValidationRuleSet<IMainEntity>
  {
    public DependsOnWithErrorRuleSet(IDbAccessor accessor)
    {
      var errorRule = new Mock<IRule<IMainEntity>>();
      errorRule.Setup(x => x.Validate(It.IsAny<IMainEntity>(), It.IsAny<ValidationContext>()))
               .Throws(new Exception("error"));
      errorRule.Setup(x => x.Identity)
               .Returns(new RuleIdentity("ERRULE"));
      var falseRule = new Mock<IRule<IMainEntity>>();
      falseRule.Setup(x => x.Validate(It.IsAny<IMainEntity>(), It.IsAny<ValidationContext>()))
               .Returns(new RuleValidationResult(errorRule.Object.Identity,
                                                 new ErrorRuleMessage(new MessageIdentity("ERRULEM1"), "error")));
      falseRule.Setup(x => x.Identity)
               .Returns(new RuleIdentity("FALSERULE"));

      SetRule(errorRule.Object)
        .DependsOn(falseRule.Object)
        .DependsOn(new ContractAccountRule(accessor));
    }
  }
}
