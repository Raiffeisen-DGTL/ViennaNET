using System;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;
using ViennaNET.Validation.Tests.Data;

namespace ViennaNET.Validation.Tests.RuleValidation
{
  internal class CheckAmountRule : BaseRule<ICollectionEntity>
  {
    private const string InternalCode = "CSAL1";

    public CheckAmountRule()
      : base(InternalCode)
    {
    }

    public override RuleValidationResult Validate(ICollectionEntity value, ValidationContext context)
    {
      var result = new RuleValidationResult(Identity);
      if (Math.Abs(Math.Truncate(value.TotalAmount*100) - (value.TotalAmount*100)) != 0)
      {
        result.Append(new ErrorRuleMessage(new MessageIdentity("CSAL1M1"), 
          string.Format("По счету физического лица {0} сумма заработной платы содержит более двух знаков поле запятой: {1} ",
            value.AccountCba,
            value.TotalAmount)));
        return result;
      }
      return null;
    }
  }
}