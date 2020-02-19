using System.Collections.Generic;
using System.Linq;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Tests.Data;
using ViennaNET.Validation.Tests.RuleValidation;
using ViennaNET.Validation.Tests.RuleValidation.When;
using ViennaNET.Validation.Validators;
using Moq;
using NUnit.Framework;

namespace ViennaNET.Validation.Tests
{
  [TestFixture(Category = "Unit")]
  public class RuleValidatorTests
  {
    [Test]
    public void AccountTypeRule_ValidAccountType_Success()
    {
      var entity = GetEntity();
      var rule = new AccountTypeRule(GetDbAccessor().Object);
      var res = rule.Validate(entity.Object, null);
      Assert.IsNull(res);
    }

    [Test]
    public void ContractAccountRule_ExistedContractAccount_Success()
    {
      var entity = GetEntity();
      var rule = new ContractAccountRule(GetDbAccessor().Object);
      var res = rule.Validate(entity.Object, null);
      Assert.IsNull(res);
    }

    [Test]
    public void Validator_LoadingContextWhen_Success()
    {
      var entity = GetEntity();
      var validator = new Validator();
      var validationContext = new LoadingWhenRuleSet(GetDbAccessor().Object);
      var res = validator.Validate(validationContext, entity.Object, null);
      Assert.IsTrue(res.IsValid);
    }

    [Test]
    public void Validator_LoadingDependsOnContext_Success()
    {
      var entity = GetEntity();

      entity.Setup(x => x.Salaries).Returns(new List<ICollectionEntity>());

      var validator = new Validator();
      var validationContext = new LoadingDependsOnRuleSet(GetDbAccessor().Object);
      var res = validator.Validate(validationContext, entity.Object, null);
      Assert.IsTrue(res.IsValid);
    }

    [Test]
    public void Validator_DependsOnWithErrorContext_DontCallErrorRule()
    {
      var entity = GetEntity();
      var validator = new Validator();
      var validationContext = new DependsOnWithErrorRuleSet(GetDbAccessor().Object);
      Assert.DoesNotThrow(() => validator.Validate(validationContext, entity.Object, null));
    }

    [Test]
    public void Validator_LoadingDependsOnContext_HasErrorInNestedContext()
    {
      var entity = GetEntity();

      var salary = new Mock<ICollectionEntity>();
      salary.Setup(x => x.AccountCba).Returns("1234567890");
      salary.Setup(x => x.TotalAmount).Returns(1.324567m);

      entity.Setup(x => x.Salaries).Returns(new List<ICollectionEntity> { salary.Object });

      var validator = new Validator();
      var validationContext = new LoadingDependsOnRuleSet(GetDbAccessor().Object);
      var res = validator.Validate(validationContext, entity.Object, null);
      Assert.IsTrue(!res.IsValid);
    }

    [Test]
    public void Validator_ManyContexts_CorrectResult()
    {
      var entity = GetEntity();

      var accountInfo = new Mock<IAccInfo>();
      accountInfo.Setup(info => info.AccountType).Returns("CurrentInThirdpartyBank");

      entity.Setup(p => p.AccountsInfo).Returns(accountInfo.Object);
      entity.Setup(x => x.Salaries).Returns(new List<ICollectionEntity>());

      var validator = new Validator();
      var validationContext = new LoadingDependsOnRuleSet(GetDbAccessor().Object);
      var validationWhenContext = new LoadingWhenRuleSet(GetDbAccessor().Object);
      var res = validator.ValidateMany(new IValidationRuleSet<IMainEntity>[] { validationContext, validationWhenContext }, entity.Object, null);
      Assert.IsTrue(res.Results.Count == 2);
    }

    [Test]
    public void Validator_FluentRule_HasManyMessages()
    {
      var entity = GetEntity();
      entity.Setup(p => p.DocType).Returns("Excel");

      var accountInfo = new Mock<IAccInfo>();
      accountInfo.Setup(info => info.AccountType).Returns("CurrentInThirdpartyBank");
      accountInfo.Setup(a => a.Account).Returns("New account");
      accountInfo.Setup(a => a.AccountCba).Returns("1234567890");

      entity.Setup(p => p.AccountsInfo).Returns(accountInfo.Object);
      var validator = new Validator();
      var rule = new FluentRule();
      var validationContext = new FluentRuleRuleSet(rule);
      var res = validator.Validate(validationContext, entity.Object, null);
      Assert.IsTrue(res.Results.Count == 1);
      Assert.IsTrue(res.Results[0].Messages.Count() == 2);
    }

    [Test]
    public void Validator_FluentRule_OverrideMessages()
    {
      var entity = GetEntity();
      entity.Setup(p => p.DocType).Returns("Excel");

      var accountInfo = new Mock<IAccInfo>();
      accountInfo.Setup(info => info.AccountType).Returns("CurrentInThirdpartyBank");
      accountInfo.Setup(a => a.Account).Returns("New account");
      accountInfo.Setup(a => a.AccountCba).Returns("1234567890");

      entity.Setup(p => p.AccountsInfo).Returns(accountInfo.Object);
      var validator = new Validator();
      var rule = new FluentRule();
      var validationContext = new FluentRuleRuleSet(rule);
      var res = validator.Validate(validationContext, entity.Object, null);
      var formatter = new FluentRuleMessageFormatter();
      res = formatter.Format(res);
      Assert.IsTrue(res.Results[0].Messages.First().Error == "A type of an account 1234567890 must be CurrentInThirdpartyBank");
      Assert.IsTrue(res.Results[0].Messages.Last().Error == "A type of an action must not be Update");
    }

    [Test]
    public void Validator_RuleDependOnContext_GenerateError()
    {
      var entity = GetEntity();
      entity.Setup(p => p.DocType).Returns("Excel");

      var validator = new Validator();
      var rule = new FluentRule();
      var validationContext = new FluentRuleRuleSet(rule);
      var res = validator.Validate(validationContext, entity.Object, new ValidationContext(12));    
      Assert.IsTrue(res.Results.Any());
    }

    [Test]
    public void Validator_RuleWithStop_StoppedAfterFirstFailure()
    {
      var entity = GetEntity();
      entity.Setup(p => p.DocType).Returns("Excel");

      var validator = new Validator();
      var validationContext = new StopOnFailureRuleSet();
      Assert.DoesNotThrow(() => validator.Validate(validationContext, entity.Object, null));
    }

    [Test]
    public void Validator_RuleWithWhen_RuleDoesntReturnsError()
    {
      var entity = GetEntity();
      entity.Setup(p => p.DocType).Returns("Excel");

      var validator = new Validator();
      var validationContext = new WhenRuleSet();

      var res = validator.Validate(validationContext, entity.Object, null);
      Assert.True(res.IsValid);
    }

    [Test]
    public void UseRuleFluentRule_WhenObjectIsValid_CorrectResult()
    {
      var entity = GetEntity();
      var validator = new Validator();

      var res = validator.Validate(new UseIRuleFluentRule(), entity.Object, null);

      Assert.That(res.IsValid, Is.True);
    }

    [Test]
    public void UseRuleFluentRule_WhenAggregatedPropertyHasNull_MustReturnsError()
    {
      var entity = new Mock<IMainEntity>();
      var accountInfo = new Mock<IAccInfo>();
      accountInfo.Setup(x => x.Account)
                 .Returns<string>(null);
      accountInfo.Setup(x => x.AccountCba)
                 .Returns("dfhbv");
      accountInfo.Setup(x => x.AccountType)
                 .Returns("dfhbv");

      entity.Setup(x => x.AccountsInfo).Returns(accountInfo.Object);
      var validator = new Validator();

      var res = validator.Validate(new UseIRuleFluentRule(), entity.Object, null);

      Assert.That(res.IsValid, Is.False);
      Assert.That(res.Results[0].Messages.Count(), Is.EqualTo(1));
      Assert.That(res.Results[0].Messages.First().Error, Does.StartWith("Account is null"));
    }

    [Test]
    public void UseRuleFluentRule_WhenAggregatedPropertyHas2Nulls_MustReturns2ErrorMessages()
    {
      var entity = new Mock<IMainEntity>();
      var accountInfo = new Mock<IAccInfo>();
      accountInfo.Setup(x => x.Account)
                 .Returns<string>(null);
      accountInfo.Setup(x => x.AccountCba)
                 .Returns("dfhbv");
      accountInfo.Setup(x => x.AccountType)
                 .Returns<string>(null);
      entity.Setup(x => x.AccountsInfo).Returns(accountInfo.Object);
      var validator = new Validator();

      var res = validator.Validate(new UseIRuleFluentRule(), entity.Object, null);

      Assert.That(res.IsValid, Is.False);
      Assert.That(res.Results[0].Messages.Count(), Is.EqualTo(2));
      Assert.That(res.Results[0].Messages.First().Error, Does.StartWith("Account is null"));
      Assert.That(res.Results[0].Messages.Last().Error, Does.StartWith("AccountType is null"));
    }

    private Mock<IDbAccessor> GetDbAccessor()
    {
      var dbAccessor = new Mock<IDbAccessor>();
      dbAccessor.Setup(
        accessor => accessor.GetContractAccountType(It.IsAny<string>()))
        .Returns("Текущий в банке");

      dbAccessor.Setup(accessor => accessor.GetContractAccount(It.IsAny<int>(), It.IsAny<string>()))
        .Returns(new Mock<IAccInfo>().Object);
      return dbAccessor;
    }

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


  }
}