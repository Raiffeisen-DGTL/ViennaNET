﻿using ViennaNET.Validation.Rules.FluentRule;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class UseIRuleFluentRule : BaseFluentRule<IMainEntity>
  {
    public UseIRuleFluentRule()
    {
      ForProperty(e => e.AccountsInfo)
        .UseRule(new AccountTypeFluentRule());
    }
  }
}