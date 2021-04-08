# Build with stub classes for unit testing of code using ViennaNET.Orm to access the database

### Class Description

* **EntityRepositoryStub** - a class for creating an instance of the **IEntityRepository** stub, which wraps a collection of existing objects and makes it possible to use NHibernate-specific methods **Fetch**, **ThenFetch**, after calling `Query ()`, **FetchMany**, **ThenFetchMany**, etc. There is a static method `Create<T>(IEnumerable<T> items)`, which accepts a collection of objects implementing **IEntityKey<int>**, and overloads `Create<T, TKey>(IEnumerable<T> items) where T: class, IEntityKey<TKey> `, in which you can specify a key type other than int.

* **CustomQueryExecutorStub** - a class for creating an instance of the **ICustomQueryExecutor** stub, which returns passed collection on each call and saves queries passed on each call (so they can be examined and asserted after usage).

* **CommandExecutorStub** - a class for creating an instance of the **ICommandExecutor** stub, which returns passed collection on each call and saves commands passed on each call (so they can examined and aserted after usage).

* **EntityFactoryServiceStubBuilder** - class for creating an instance of the stub **IEntityFactoryService**. It is proposed to use it in the following form:
```csharp
var existingUsers = new User[] { new User("user1login"), new User("user2Login") };
var usersRepository = EntityRepositoryStub.Create<User, string>(existingUsers); // need to explicitly specify key if it is not int

var customQueryExecutor = CustomQueryExecutorStub.Create(queryResultItems); // collection of items our code will recieve after this query executed
var commandExecutor = CommandExecutorStub.Create<ExpireSessionCommand>(1);

var entityFactoryServiceStub = EntityFactoryServiceStubBuilder.Create().
    .With(existingUsers)
    .With(EntityRepositoryStub.Create(new Session [] {...}))
    .With(EntityRepositoryStub.Create(books))
    .Build();

// ..... some work

var usersAfterWork = usersRepository.Query().ToList();
Assert.That(usersAfterWork, Is.EquivalentTo(new [] {existingUsers[0]}), "user2Login should be deleted");

Assert.That(customQueryExecutor.Queries, Has.One.Items);
Assert.That(customQueryExecutor.Queries.Single().Parameters["user_id"].BaseValue, Is.EqualTo("user2Login"));

Assert.That(commandExecutor.CommandsCalled, Has.Count.EqualTo(2));
Assert.That(commandExecutor.CommandsCalled.Select(c => c.Parameters["session_id"].BaseValue), Is.EquivalentTo(new [] {123, 456}));

```
An entityFactoryService instance can be used as **IEntityFactoryService**, or as **IEntityRepositoryFactory**.

The motivation for creating these classes was:
- make a convenient interface for creating stubs;
- add support for **NHibernate** specific methods: `Fetch`,` FetchMany`, `ThenFetch`,` ThenFetchMany`, `ToListAsync` (and other asynchronous request materializers), as well as the` DeleteAsync` method.

At the moment, commit / rollback transactions using UnitOfWork is not supported, i.e. interface methods `IUnitOfWork` do not have any effect on the contents of the repository.
There is some groundwork in the code to make available information about the fact of calling the `Commit` and` Rollback` methods, but this is left as a point for the development of the library.
