# An assembly containing classes for creating validation rules.

### Key Entities

* **BaseRule** - the base class of the validation rule, the unit of validation in the project. The rule implements the IRule<in T> interface, where T is the entity validated by the rule.
Each rule contains a unique identifier of type RuleIdentity.

* **BaseFluentRule** - the base class of the validation rule that supports the fluid interface. Allows you to configure checks in the designer using the extension functions.

* **RuleValidationResult** - the result of the rule. It contains a set of messages that implement the **IRuleMessage** interface.
Each message has a unique MessageIdentity.
The result is considered correct if it contains only messages of type **WarningRuleMessage**. If there is a message of type ErrorRuleMessage, the result is incorrect. When designing custom rules, consider this when returning a value.

* **BaseValidationRuleSet** - the base class for defining a set of validation rules. T is a validated entity that matches rule entities within a set.
The base class contains a set of helper methods for constructing chains of rules.
Rule sets should be used to group rules and perform validation parts by input conditions.
The validation algorithm for nested set rules is specified in the constructor. To do this, use the following functions:

`SetRule(IRule<T> rule)` - puts the rule in the validation chain.

`SetCollectionContext(Expression<Func<T, IEnumerable<TEntity>>> expression, IValidationRuleSet<TEntity> ruleSet)` - Defines a set of validation rules for a nested collection of a validated entity

`SetCollectionContext(Expression<Func<T, IEnumerable<TEntity>>> expression, IEnumerable<IValidationRuleSet<TEntity>> contexts)` - Defines several sets of validation rules for a nested collection of a validated entity

* **Validator** - the main class that allows you to validate rules and rule sets.

### Instructions for use

1. Create a rule class, inherit from the BaseRule class, set the type of the entity being validated. Define the InternalCode constant, which uniquely identifies the rule within the project. After that, we redefine the Validate method as we need. An example of a ready rule:

```csharp
    public class Rule1: BaseRule<Foo>
    {
      private const string InternalCode = "UniqueString";
      
	  public Rule1(): base(InternalCode)
      {
      }

      public override RuleValidationResult Validate(Foo item, ValidationContext context)
      {
        if (item.Prop1 == 300)
        {
          return new RuleValidationResult(Identity, new ErrorRuleMessage(new MessageIdentity ("Unique Message"),
            string.Format("Invalid property value: {0}",
             item.Prop1)));
        }
        return null;
      }
    }
```

2. Let's create a set of rules, having inherited from the BaseVlidationRuleSet class and setting a validated entity common to all the rules. In the constructor, we define the rules and the dependencies between them. An example of a ready-made set of rules:

```csharp
    public class RuleSet1: BaseValidationRuleSet<Foo>
    {
      public RuleSet1(Rule1 r1, Rule2 r2, Rule3 r3, Rule4 r4)
      {
        SetRule(r1).DependsOn(r2);
        SetRule(r3).StopOnFailure();
        SetRule(r4);
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
 
      public FooValidator(IValidator validator, RuleSet1 ruleSet)
      {
        this.validator = validator;
        this.ruleSet = ruleSet;
        this.formatter = new FooFormatter();
      }
     
      public void Validate(Foo item)
      {
        item.Prop1 = 300;
        var result = validator.Validate(ruleSet, item, null);
        result formatter.Format(result);
        foreach(var res in result.Results)
        {
          foreach(var message in res.Messages)
          {
            Console.WriteLine(message.Error);
          }
        }
      }
    }
```

Instead of using the **IValidator** interface, you can use the static wrapper **RulesValidator**.

### Conditional execution of rules

To conditionally execute the rules, the WhenXXX methods of the BaseValidationRuleSet class are used:

When - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of type ErrorRuleMessage.

```csharp
    When(Rule1, () =>
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })
```

WhenNoWarnings - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of the WarningRuleMessage type.

```csharp
    WhenNoWarnings(Rule1, () =>
    {
      SetRule(Rule2);
      SetRule(Rule3);
    })
```

WhenNoInfos - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of type InfoRuleMessage(Information).

```csharp
    WhenNoWarningsAndErrors(Rule1, () =>
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })
```

WhenNoInfosAndWarnings - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of type WarningRuleMessage and messages of type InfoRuleMessage(Information).

```csharp
    WhenNoInfosAndWarnings(Rule1, () =>
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })
```

WhenNoInfosAndErrors - rules 2 and 3 will be executed only if rule 1 returns a result that does not contain messages of type InfoRuleMessage (Information) and messages of type ErrorRuleMessage.

```csharp
    WhenNoInfosAndErrors(Rule1, () =>
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })
```

### Chains of dependent rules

To construct chains of dependent rules within a rule set, the DependsOn method is used. Usage example:

```csharp
    SetRule(Rule1()).DependsOn(Rule2()).DependsOn(Rule3());
```

In this case, the implementation of the first rule will depend on the implementation of 2 and 3. The rules are executed from right to left. If rule 3 returns an error, then rule 2 and 1 will not be executed. The result of each rule will be recorded in the validation result.

### Stop validation on error

To immediately stop the validation process after an error in a rule within a rule set, you must use the StopOnFailure function when defining a rule in the set. Usage example:

```csharp
    SetRule(Rule1).StopOnFailure();
```

If Rule1 returns an invalid result, then all the rules specified after it will not be executed.

### Creating a Fluent-style Rule

If the rule is a simple set of checks that do not require complex logic, you can use it when creating it with the BaseFluentRule base class. Its Validate method does not involve redefinition, and all checks must be written in the designer in the form of chains of a certain format. The following is an example of setting such a rule:

```csharp
    public class FluentRule: BaseFluentRule<Foo>
    {
      private const string InternalCode = "UniqueString";
      public FluentRule(): base(InternalCode)
      {
        ForProperty(x => x.ActionType)
	  .NotNull()
	  .WithWarningMessage("UniqueMessage1", "Action Type Not Set")
          .Must((x, с) => x! = ActionType.Update)
	  .WithErrorMessage("UniqueMessage2", "The type of action should not be updated");
      }
    }
```

The ForProperty function sets a lambda that returns the property of the entity being checked. Further along the chain, various checks can be applied to it. Each check can go with a message bound to it, specified by the WithWarningMessage and WithErrorMessage methods.
The Must function is a special case of the rule validator; it can be used if the library does not have a predefined validator for your case. The input accepts not only the value of the validated property, but also the execution context.

If you need to impose a condition on the rule, you can use the When function. It takes a lambda that returns a boolean value. If True, then the rule will be executed, otherwise the rule will be skipped.

```csharp
    public class FluentRule: BaseFluentRule<Foo>
    {
      public FluentRule()
      {
        ForProperty(x => x.ActionType)
	  .When((x, c) => x.ActionTypeEnabled)
	  .NotNull()
	  .WithWarningMessage("UniqueMessage1", "Action type not set");
      }
    }
```

In addition to the property value, the When function takes an execution context.

### Nested Fluent Rules

Rule sets are used to group rules. However, there are cases when the validation checks themselves are very simple, interaction between them is not required. In this case, you can create one validation rule for the entire object.
In such a situation, it may be necessary to validate properties, which themselves are composite objects, and for them there are already created validation rules.
In this case, you can use the validator that takes the rule as an input.

```csharp
    internal class UseIRuleFluentRule: BaseFluentRule<IMainEntity>
    {
      public UseIRuleFluentRule()
      {
        ForProperty(e => e.AccountsInfo)
          .UseRule(new AccountsInfoFluentRule());
      }
    }
```

### Validator

For validation, the **Validator** class is used that implements the **IValidator** interface. It contains the following methods:

* `ValidationResult Validate<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context)` - Validation of one set of rules for an instance entity with an arbitrary context.

* `ValidationResult ValidateMany<T>(IEnumerable<IValidationRuleSet<T>> context, T instance, ValidationContext context)` - Validation of several rule sets for an instance entity with an arbitrary context.

Additionally, there are asynchronous versions of methods.

The validation result contains a set of rule results. The result is correct if there are no messages of type ErrorRuleMessage in any result of the rule.

The ValidationContext class is used to pass general state and state exchange between set rules. If necessary, the user must create it and pass it to the validator. Each set rule can use it in its work.

Sometimes a set of rules is degenerate, it contains just a few consecutive rules.

```csharp
    public class DummyRuleSet: BaseValidationRuleSet<SomeClass>
    {
      public DummyRuleSet(Rule1 rule1, Rule2 rule2)
      {
        SetRule(rule1);
        SetRule(rule2);
      }
    }
```

In this case, you can use functions that accept not rulesets, but the rules themselves:

* `ValidationResult Validate<T>(IEnumerable<IRule<T>> rules, T instance, ValidationContext context)` - Validation of the list of rules for an instance entity with an arbitrary context.

* `ValidationResult ValidateMany<T>(IRule<T> rule, T instance)` - Validation of one rule for instance entity with arbitrary context.

### Message Formatting

To change the text of messages received during validation, you must create your own formatting class that implements the **IValidationMessageFormatter** interface and inherits from the **BaseValidationMessageFormatter** class.

Formatting rules are specified in the class constructor. To do this, you need to build a chain of the form:

```csharp
    ForRule(Rule1.InternalCode).ForMessage("Message1").OverrideMessage("Replaced String");
```

To identify a message, 2 unique identifiers are used - the rule identifier and the message identifier in the rule.
