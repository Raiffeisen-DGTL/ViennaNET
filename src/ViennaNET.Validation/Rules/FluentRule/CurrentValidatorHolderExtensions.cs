using System;
using System.Linq;
using ViennaNET.Utils;
using ViennaNET.Validation.Rules.ValidationResults.RuleMessages;

namespace ViennaNET.Validation.Rules.FluentRule
{
  /// <summary>
  /// Методы расширения для контейнера со ссылкой на последний валидатор в цепи
  /// </summary>
  public static class CurrentValidatorHolderExtensions
  {
    /// <summary>
    /// Задает обещание, что правило вернет предупреждение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор правила</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithWarningMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetMessageSource(() => new WarningRuleMessage(new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет предупреждение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithWarningMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetMessageSource(() => new WarningRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                                 .ToString()), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет предупреждение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор правила</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithWarningMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, Func<T, object> customState, string message,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new WarningRuleMessage(new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет предупреждение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithWarningMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, Func<T, object> customState, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new WarningRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                                 .ToString()), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет предупреждение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="customCode">Дополнительный код сообщения</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithWarningMessageCustomCode<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, string customCode, Func<T, object> customState, string message,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new WarningRuleMessage(customCode, new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет информационное сообщение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="customCode">Дополнительный код сообщения</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithInfoMessageCustomCode<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, string customCode, Func<T, object> customState, string message,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new InfoRuleMessage(customCode, new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет информационное сообщение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithInfoMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetMessageSource(() => new InfoRuleMessage(new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет информационное сообщение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithInfoMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetMessageSource(() => new InfoRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                              .ToString()), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет информационное сообщение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithInfoMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, Func<T, object> customState, string message,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new InfoRuleMessage(new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет информационное сообщение с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithInfoMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, Func<T, object> customState, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new InfoRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                              .ToString()), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет сообщение об ошибке с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithErrorMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetMessageSource(() => new ErrorRuleMessage(new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет сообщение об ошибке с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithErrorMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetMessageSource(() => new ErrorRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                               .ToString()), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет сообщение об ошибке с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithErrorMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, Func<T, object> customState, string message,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new ErrorRuleMessage(new MessageIdentity(code), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет сообщение об ошибке с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="code">Идентификатор сообщения</param>
    /// <param name="customCode">Дополнительный код сообщения</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithErrorMessageCustomCode<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, string code, string customCode, Func<T, object> customState, string message,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new ErrorRuleMessage(new MessageIdentity(code), customCode, message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет сообщение об ошибке с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="message">Текст сообщения</param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithErrorMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, Func<T, object> customState, string message, params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(() => new ErrorRuleMessage(new MessageIdentity(Guid.NewGuid()
                                                                                               .ToString()), message));
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }

    /// <summary>
    /// Задает обещание, что правило вернет сообщение об ошибке с указанными аргументами
    /// </summary>
    /// <typeparam name="T">Тип объекта валидации</typeparam>
    /// <typeparam name="TProperty">Тип свойства</typeparam>
    /// <param name="obj">Контейнер со ссылкой на последний валидатор в цепи</param>
    /// <param name="customState">Ссылка на функцию, возвращающую дополнительное состояние</param>
    /// <param name="customRuleMessage">Ссылка на функцию, возвращающую сообщение </param>
    /// <param name="funcs">Функции, вычисляющие параметры форматирования текста сообщения</param>
    /// <returns>Строитель валидаторов правила с текучим интерфейсом</returns>
    [StringFormatMethod("message")]
    public static RuleValidationMemberBuilder<T, TProperty> WithCustomMessage<T, TProperty>(
      this CurrentValidatorHolder<T, TProperty> obj, Func<T, object> customState, Func<IRuleMessage> customRuleMessage,
      params Func<T, object>[] funcs)
    {
      obj.CurrentValidator.AddArguments(funcs.Select(func => (Func<object, object>)(x => func((T)x))));
      obj.CurrentValidator.SetState(x => customState((T)x));
      obj.CurrentValidator.SetMessageSource(customRuleMessage);
      return (RuleValidationMemberBuilder<T, TProperty>)obj;
    }
  }
}
