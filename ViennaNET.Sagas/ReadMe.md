# Assembly with the basic implementation of the sag mechanism

### Contains:
* SagaBase - The base class for creating sagas through the inheritance mechanism
* SagaStep - Class for describing the step of the saga

### Principle of operation:
When you call the Execute method declared in the SagaBase <> class, the process of sequential execution of the basic actions of the steps specified with WithAction will begin.
In the event of ANY unhandled exception, the direct process is interrupted, and the rollback process begins, which consists of sequentially invoking the actions of the steps in the REVERSE manner indicated with WithCompensation.

### How to use:
1. Create a class to describe a specific saga, make inheritance from SagaBase <>.
2. In the constructor of a specific class, sequentially specify the steps of the saga through the use of the Step () method from SagaBase <>.
3. If at some stage it is necessary to force the rollback of the saga, then simply throw an AbortSagaExecutingException.

### Example:

      private class TestFailedSaga: SagaBase <SomeContext>
      {
        public TestFailedSaga (SomeService someService)
        {
          Step("step 1")
            .WithAction(c => someService.Method1())
            .WithCompensation(c => someService.Method1());

          Step("step 2")
            .WithAction(c => someService.Method2())
            .WithCompensation(c => someService.Method2());

          Step ("step 3")
            .WithAction(c => throw new AbortSagaExecutingException())
            .WithCompensation(c => someService.Method3());
        }
      }