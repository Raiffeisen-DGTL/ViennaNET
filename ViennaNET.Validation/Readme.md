# Сборка, содержащая классы для создания валидационных правил.

### Основные сущности

* **BaseRule** - базовый класс правила валидации, единица валидации в проекте. Правило реализует интерфейс IRule<in T>, где T - это валидируемая правилом сущность.
Каждое правило содержит уникальный идентификатор типа RuleIdentity. 

* **BaseFluentRule** - базовый класс правила валидации, поддерживающего текучий интерфейс. Позволяет конфигурировать проверки в конструкторе с помощью функций расширения. 

* **RuleValidationResult** - результат выполнения правила. Содержит в себе набор сообщений, реализующих интерфейс **IRuleMessage**.
Каждое сообщение имеет уникальный идентификатор MessageIdentity.
Результат считается корректным, если в нем содержатся только сообщения типа **WarningRuleMessage**. При наличии сообщения типа ErrorRuleMessage результат некорректный. При проектировании пользовательских правил следует учитывать это при возвращении значения.

* **BaseValidationRuleSet** - базовый класс для определения набора валидационных правил. Т - валидируемая сущность, совпадающая с сущностями правил внутри набора. 
Базовый класс содержит набор вспомогательных методов для построения цепочек правил. 
Наборы правил следует использовать для группировки правил и выполнения частей валидации по входным условиям.
Алгоритм валидации вложенных правил набора задается в конструкторе. Для этого используются следующие функции:

SetRule(IRule<T> rule) - помещает правило в цепочку валидации.

SetCollectionContext(Expression<Func<T, IEnumerable<TEntity>>> expression, IValidationRuleSet<TEntity> ruleSet) - Для вложенной коллекции валидируемой сущности задает набор правил валидации

SetCollectionContext(Expression<Func<T, IEnumerable<TEntity>>> expression, IEnumerable<IValidationRuleSet<TEntity>> contexts) - Для вложенной коллекции валидируемой сущности задает несколько наборов правил валидации

* **Validator** - основной класс, позволяющий валидировать правила и наборы правил. 

### Инструкция по применению

1. Создадим класс правила, отнаследуемся от класса BaseRule, зададим тип валидируемой сущности. Зададим константу InternalCode, которая однозначно идентифицирует правило в пределах проекта. После этого переопределим метод Validate так, как нам нужно. Пример готового правила:

    public class Rule1 : BaseRule<Foo>
    {
      private const string InternalCode = "УникальнаяСтрока";
      
	  public Rule1(): base(InternalCode)
      {
      }

      public override RuleValidationResult Validate(Foo item, ValidationContext context)
      {
        if (item.Prop1 == 300)
        {
          return new RuleValidationResult(Identity, new ErrorRuleMessage(new MessageIdentity("УникальныйМесседж"),
            string.Format("Неверное значение свойства: {0}",
             item.Prop1)));
        }
        return null;
      }
    }

2. Создадим набор правил, отнаследовавшись от класса BaseVlidationRuleSet и задав общую для всех правил валидируемую сущность. В конструкторе зададим правила и зависимости между ними. Пример готового набора правил:

    public class RuleSet1 : BaseValidationRuleSet<Foo>
    {
      public RuleSet1(Rule1 r1, Rule2 r2, Rule3 r3, Rule4 r4)
      {
        SetRule(r1).DependsOn(r2);
        SetRule(r3).StopOnFailure();
        SetRule(r4);
      }
    }

3. Для выполнения валидации требуется создать экземпляр валидатора и вызвать функцию валидации. После вызова валидации модифицируем результат выполнения. Пример использования:

    public class FooValidator
    {
      private IValidator validator;
      private IValidationRuleSet ruleSet;
      private IValidationMessageFormatter formatter;
 
      public FooValidator (IValidator validator, RuleSet1 ruleSet)
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

Вместо использования интерфейса **IValidator** можно воспользоваться статической оберткой **RulesValidator**.

### Условное выполнение правил

Для условного выполнения правил используется методы WhenXXX класса BaseValidationRuleSet:

When - правила 2 и 3 будут выполнены только в том случае, если правило 1 вернуло результат, не содержащий сообщений типа ErrorRuleMessage (Ошибка).

    When(Rule1, () => 
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })

WhenNoWarnings - правила 2 и 3 будут выполнены только в том случае, если правило 1 вернуло результат, не содержащий сообщений типа WarningRuleMessage (Предупреждение).

    WhenNoWarnings(Rule1, () => 
    {
      SetRule(Rule2);
      SetRule(Rule3);
    })

WhenNoInfos - правила 2 и 3 будут выполнены только в том случае, если правило 1 вернуло результат, не содержащий сообщений типа InfoRuleMessage (Информация).

    WhenNoWarningsAndErrors(Rule1, () => 
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })

WhenNoInfosAndWarnings - правила 2 и 3 будут выполнены только в том случае, если правило 1 вернуло результат, не содержащий сообщений типа WarningRuleMessage (Предупреждение) И сообщений типа InfoRuleMessage (Информация).

    WhenNoInfosAndWarnings(Rule1, () => 
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })

WhenNoInfosAndErrors - правила 2 и 3 будут выполнены только в том случае, если правило 1 вернуло результат, не содержащий сообщений типа InfoRuleMessage (Информация) И сообщений типа ErrorRuleMessage.

    WhenNoInfosAndErrors(Rule1, () => 
	{
      SetRule(Rule2);
      SetRule(Rule3);
    })

### Цепочки зависимых правил

Для построения цепочек зависимых правил внутри набора правил используется метод DependsOn. Пример использования:

    SetRule(Rule1()).DependsOn(Rule2()).DependsOn(Rule3());

В данном случае выполнение первого правила будет зависеть от выполнения 2 и 3. Правила выполняются справа налево. Если правило 3 вернуло ошибку, то правило 2 и 1 не будет выполнены. Результат каждого правила будет записан в результат валидации.

### Остановка валидации при ошибке

Для немедленной остановки процесса валидации после ошибки в правиле внутри набора правил требуется использовать функцию StopOnFailure при задании правила в наборе. Пример использования:

    SetRule(Rule1).StopOnFailure();

Если Rule1 вернет невалидный результат, то все правила, заданные после него, не будут выполняться.

### Создание правила в стиле Fluent

В случае если правило представляет собой простой набор проверок, не требующий сложной логики, можно воспользоваться при создании его базовым классом BaseFluentRule. Его метод Validate не предполагает переопределения, а все проверки необходимо прописать в конструкторе в виде цепочек определенного формата. Ниже приведен пример задания такого правила:

    public class FluentRule : BaseFluentRule<Foo>
    {
      private const string InternalCode = "УникальнаяСтрока";
      public FluentRule() : base(InternalCode)
      {
        ForProperty(x => x.ActionType).NotNull().WithWarningMessage("УникальныйМесседж1", "Тип действия не задан")
         .Must((x, с) => x != ActionType.Update).WithErrorMessage("УникальныйМесседж2", "Типом действия не должно быть обновления");
      }
    }

В функции ForProperty задается лямбда, возвращающая свойство проверяемой сущности. Далее по цепочке к ней могут быть применены различные проверки. Каждая проверка может идти с привязанным к ней сообщением, задаваемым методами WithWarningMessage и WithErrorMessage.
Функция Must является частным случаем валидатора правила, ее можно использовать, если в библиотеке нет предопределенного валидатора для вашего случая. На вход принимает не только значение валидируемого свойства, но и контекст выполнения.

В случае если требуется наложить условие на выполнение правила, можно использовать функцию When. Она принимает лямбду, которая возвращает булево значение. Если значение True, то правило будет выполняться, в обратном случае правило пропускается.

    public class FluentRule : BaseFluentRule<Foo>
    {
      public FluentRule()
      {
        ForProperty(x => x.ActionType).When((x, c) => x.ActionTypeEnabled).NotNull().WithWarningMessage("УникальныйМесседж1", "Тип действия не задан");
      }
    }

Кроме значения свойства функция When принимает контекст выполнения.

### Вложенные Fluent-правила

Для группировки правил служат наборы правил. Однако бывают случаи, когда сами валидационные проверки очень просты, между ними не требуется взаимодействия. В таком случае можно создать одно валидационное правило на весь объект. 
В такой ситуации может потребоваться валидация свойств, которые сами по себе являются составными объектами, и для них существуют уже созданные правила валидации. 
В этом случае можно воспользоваться валидатором, принимающим на вход правило. 

    internal class UseIRuleFluentRule : BaseFluentRule<IMainEntity>
    {
      public UseIRuleFluentRule()
      {
        ForProperty(e => e.AccountsInfo)
          .UseRule(new AccountsInfoFluentRule());
      }
    }

### Валидатор

Для валидации используется класс **Validator**, реализующий интерфейс **IValidator**. Он содержит следующие методы:

* ValidationResult Validate<T>(IValidationRuleSet<T> ruleSet, T instance, ValidationContext context) - Валидация одного набора правил для сущности instance с произвольным контекстом.

* ValidationResult ValidateMany<T>(IEnumerable<IValidationRuleSet<T>> context, T instance, ValidationContext context) - Валидация нескольких наборов правил для сущности instance с произвольным контекстом.

Дополнительно существуют асинхронные версии методов.

Результат валидации содержит набор результатов правил. Результат корректен, если ни в одном результате правила нет сообщений типа ErrorRuleMessage.

Для передачи общего состояния и обмена состоянием между правилами набора используется класс ValidationContext. При необходимости пользователь должен создать его и передать в валидатор. Каждое правило набора может использовать его в своей работе.

Иногда набор правил является вырожденным, содержит просто несколько последовательных правил.

    public class DummyRuleSet : BaseValidationRuleSet<SomeClass>
    {
      public DummyRuleSet(Rule1 rule1, Rule2 rule2)
      {
        SetRule(rule1);
        SetRule(rule2);
      }
    }

В этом случае можно воспользоваться функциями, принимающими не наборы правил, а непосредственно правила:

* ValidationResult Validate<T>(IEnumerable<IRule<T>> rules, T instance, ValidationContext context) - Валидация списка правил для сущности instance с произвольным контекстом.

* ValidationResult ValidateMany<T>(IRule<T> rule, T instance) - Валидация одного правила для сущности instance с произвольным контекстом.

### Форматирование сообщений

Для изменения текста сообщений, полученных при валидации, необходимо создать собственный класс форматирования, реализующий интерфейс **IValidationMessageFormatter** и отнаследованный от класса **BaseValidationMessageFormatter**.

Правила форматирования задаются в конструкторе класса. Для этого требуется построить цепочку вида:

    ForRule(Rule1.InternalCode).ForMessage("Message1").OverrideMessage("Замененная строка");

Для определения сообщения используется 2 уникальных идентификатора - идентификатор правила и идентификатор сообщения в правиле.