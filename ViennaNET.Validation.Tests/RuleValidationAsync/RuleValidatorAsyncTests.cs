using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Tests.RuleValidation;
using ViennaNET.Validation.Validators;
using Moq;
using NUnit.Framework;

namespace ViennaNET.Validation.Tests.RuleValidationAsync
{
  [TestFixture(Category = "Unit")]
  public class RuleValidatorAsyncTests
  {
    private Mock<IMainEntity> GetEntity()
    {
      var entity = new Mock<IMainEntity>();

      var account = new Mock<IAccInfo>();
      account.Setup(a => a.Account).Returns("New account");
      account.Setup(a => a.AccountCba).Returns("1234567890");
      account.Setup(a => a.AccountType).Returns("CurrentInTheBank");

      entity.Setup(x => x.AccountsInfo).Returns(account.Object);
      entity.Setup(x => x.ActionType).Returns("Update");
      entity.Setup(x => x.DocType).Returns("Excel");
      entity.Setup(x => x.ContractId).Returns(2334);

      return entity;
    }

    [Test]
    public void UseRuleFluentRuleAsync_WhenObjectIsValid_CorrectResult()
    {
      var entity = GetEntity();
      var validator = new ValidatorAsync();

      var res = validator.ValidateAsync(new UseIRuleFluentRule(), entity.Object, null).Result;
      var resAsync = validator.ValidateAsync(new UseIRuleFluentRuleAsync(), entity.Object, null).Result;

      Assert.That(res.IsValid, Is.True);
      Assert.That(resAsync.IsValid, Is.False);
      Assert.That(resAsync.Results[0].Messages.Count, Is.EqualTo(1));
    }
  }
}