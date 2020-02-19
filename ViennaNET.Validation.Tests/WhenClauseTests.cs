using System;
using System.Collections.Generic;
using ViennaNET.Validation.Tests.RuleValidation.When;
using ViennaNET.Validation.Validators;
using NUnit.Framework;

namespace ViennaNET.Validation.Tests
{
  [TestFixture(Category = "Unit")]
  public class WhenClauseTests
  {
     private class WhenRuleProvider
    {
      public static IEnumerable<TestCaseData> WhenRuleParameters
      {
        get
        {
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, false), 0);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, true), 0);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, false), 0);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, true), 0);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, false), 1);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, false), 1);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, false), 1);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, false), 1);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, true), 2);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, true), 2);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, false), 2);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, false), 2);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, false), 2);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, false), 2);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, true), 3);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, true), 3);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, false), 3);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, false), 3);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, false), 3);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, false), 3);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, true), 4);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, true), 4);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, false), 4);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, false), 4);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, true), 5);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, true), 5);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, true), 5);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, false), 5);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, false), 5);
          yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, false), 5);
        }
      }
    }

     private class WhenRuleThrowsProvider
     {
       public static IEnumerable<TestCaseData> WhenRuleParameters
       {
         get
         {
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, false), 0);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, true), 0);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, false), 0);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, true), 0);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(false, false, true), 1);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, true), 1);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, true), 1);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, true), 1);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, false, true), 2);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, true), 2);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, true), 3);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, true), 3);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, true), 4);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, true), 4);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, false), 4);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(false, true, false), 4);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, true), 5);
           yield return new TestCaseData(new Tuple<bool, bool, bool>(true, true, false), 5);
         }
       }
     }

    [Test, TestCaseSource(typeof(WhenRuleProvider), nameof(WhenRuleProvider.WhenRuleParameters))]
    public void WhenRuleSet_DifferentData_CorrectResults(Tuple<bool, bool, bool> param, int number)
    {
      var rule = new WhenFluentRuleSet(number);
      var validator = new Validator();
      validator.Validate(rule, param, null);
    }

    [Test, TestCaseSource(typeof(WhenRuleThrowsProvider), nameof(WhenRuleThrowsProvider.WhenRuleParameters))]
    public void WhenRuleSet_DifferentData_Throws(Tuple<bool, bool, bool> param, int number)
    {
      var rule = new WhenFluentRuleSet(number);
      var validator = new Validator();

      Assert.That(() => validator.Validate(rule, param, null), Throws.TypeOf<WhenException>());
    }
  }
}
