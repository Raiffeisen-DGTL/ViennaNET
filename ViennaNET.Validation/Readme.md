# An assembly containing classes for creating validation rules.

### Key Entities

* **BaseRule** - the base class of the validation rule, the unit of validation in the project. The rule implements the IRule <in T> interface, where T is the entity validated by the rule.
Each rule contains a unique identifier of type RuleIdentity.

* **BaseFluentRule** - the base class of the validation rule that supports the fluid interface. Allows you to configure checks in the designer using the extension functions.

* **RuleValidationResult** - the result of the rule. It contains a set of messages that implement the **IRuleMessage** interface.
Each message has a unique MessageIdentity.
The result is considered correct if it contains only messages of the ** WarningRuleMessage ** type. If there is a message of type ErrorRuleMessage, the result is incorrect. When designing custom rules, consider this when returning a value.

* **BaseValidationRuleSet** - the base class for defining a set of validation rules. T is a validated entity that matches rule entities within a set.
The base class contains a set of helper methods for building rule chains.
Rule sets should be used to group rules and perform validation parts by input conditions.
The validation algorithm for nested set rules is specified in the constructor. To do this, use the following functions:

SetRule (IRule <T> rule) - puts the rule in the validation chain.

SetCollectionContext (Expression <Func <T, IEnumerable <TEntity> >> expression, IValidationRuleSet <TEntity> ruleSet) - Defines a set of validation rules for a nested collection of a validated entity

SetCollectionContext (Expression <Func <T, IEnumerable <TEntity> >> expression, IEnumerable <IValidationRuleSet <TEntity>> contexts) - Defines several sets of validation rules for a nested collection of a validated entity

* **Validator** - the main class that allows you to validate rules and rule sets.

### Instructions for use

1. Create a rule class, inherit from the BaseRule class, set the type of the entity being validated. Define the InternalCode constant, which uniquely identifies the rule within the project. After that, we redefine the Validate method as we need. An example of a ready rule:

```csharp
    public class Rule1: BaseRule <Foo>
    {
      private const string InternalCode = "UniqueString";
      
	  public Rule1 (): base (InternalCode)
      {
      }

      public override RuleValidationResult Validate (Foo item, ValidationContext context)
      {
        if (item.Prop1 == 300)
        {
          return new RuleValidationResult (Identity, new ErrorRuleMessage (new MessageIdentity ("Unique Message"),
            string.Format ("Invalid property value: {0}",
             item.Prop1)));
        }
        return null;
      }
    }
```

2. Let's create a set of rules, inheriting from the BaseVlidationRuleSet class and setting a validated entity common to all the rules. In the constructor, we define the rules and the dependencies between them. An example of a ready-made set of rules:

```csharp  
    public class RuleSet1: BaseValidationRuleSet <Foo>
    {
      public RuleSet1 (Rule1 r1, Rule2 r2, Rule3 r3, Rule4 r4)
      {
        SetRule (r1) .DependsOn (r2);
        SetRule (r3) .StopOnFailure ();
        SetRule (r4);
      }
    }
```

3. To perform validation, you need to create a validator instance and call the validation function. After the validation call, we modify the execution result. Usage example:

```csharp
    public class FooValidator
    {
      private IValidator validator;
      private IValidationRuleSet ruleSet;
      private IValidationMessageFormatter formatter;
 
      public FooValidator (IValidator validator, RuleSet1 ruleSet)
      {
        this.validator = validator;
        this.ruleSet = ruleSet;
        this.formatter = new FooFormatter ();
      }
     
      public void Validate (Foo item)
      {
        item.Prop1 = 300;
        var result = validator.Validate (ruleSet, item, null);
        result formatter.Format (result);
        foreach (var res in result.Results)
        {
          foreach (var message in res.Messages)
          {
            Console.WriteLine (message.Error);
          }
        }
      }
    }
```

Instead of using the **IValidator** interface, you can use the static wrapper **RulesValidator**.

### Conditional execution of rules

To conditionally execute the rules, the WhenXXX methods of the BaseValidationRuleSet class are used:

When - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of the ErrorRuleMessage type.

```csharp
    When (Rule1, () =>
	{
      SetRule (Rule2);
      SetRule (Rule3);
    })
```

WhenNoWarnings - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of the WarningRuleMessage type.

```csharp  
    WhenNoWarnings (Rule1, () =>
    {
      SetRule (Rule2);
      SetRule (Rule3);
    })
```

WhenNoInfos - rules 2 and 3 will be executed only if
