# Сборка с базовой реализацией механизма саг

### Содержит:
*  SagaBase - Базовый класс для создания саг через механизм наследования
*  SagaStep - Класс для описания шага саги

### Принцип работы:
При вызове метода Execute, объявленного в классе SagaBase<>, начнется процесс последовательного выполнения основных действий шагов, указанных с помощью WithAction.
В случае появления ЛЮБОГО необработанного исключения, прямой процесс прерывается, и начинается процесс отката, который состоит из последовательного вызова действий шагов в ОБРАТНОМ порядке, указанных с помощью WithCompensation.

### Как использовать:
1. Создать класс для описания конкретной саги, сделать наследование от SagaBase<>.
2. В конструкторе конкретного класса последовательно указать шаги саги через использование метода Step() из SagaBase<>.
3. Если на каком-то этапе необходимо запустить процедуру отката саги принудительно, то достаточно просто выбросить исключение AbortSagaExecutingException.

### Пример:

      private class TestFailedSaga : SagaBase<SomeContext>
      {
        public TestFailedSaga(SomeService someService)
        {
          Step("step 1")
            .WithAction(c => someService.Method1())
            .WithCompensation(c => someService.Method1());

          Step("step 2")
            .WithAction(c => someService.Method2())
            .WithCompensation(c => someService.Method2());

          Step("step 3")
            .WithAction(c => throw new AbortSagaExecutingException())
            .WithCompensation(c => someService.Method3());
        }
      }
