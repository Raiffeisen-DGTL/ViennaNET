# Build with stub classes for unit testing of code using ViennaNET.Orm to access the database

### Class Description

* **EntityRepositoryStub** - a class for creating an instance of the stub **IEntityRepository**, which wraps a collection of existing objects and makes it possible to use NHibernate-specific methods **Fetch**, **ThenFetch**, after calling `Query ()`, **FetchMany**, **ThenFetchMany**, etc. There is a static method `Create<T>(IEnumerable<T> items)`, which accepts a collection of objects implementing **IEntityKey<int>**, and overloads `Create<T, TKey>(IEnumerable<T> items) where T: class, IEntityKey<TKey> `, in which you can specify a key type other than int.

* **EntityFactoryServiceStubBuilder** - class for creating an instance of the stub **IEntityFactoryService**. It is proposed to use it in the following form:
```csharp
var entityFactoryServiceStub = EntityFactoryServiceStubBuilder.Create().
    .With(EntityRepositoryStub.Create(new Session [] {...}))
    .With(EntityRepositoryStub.Create<User, string>(new List<User> {...}))
    .With(EntityRepositoryStub.Create(books))
    .Build();
```
An entityFactoryService instance can be used as **IEntityFactoryService**, or as **IEntityRepositoryFactory**.

The motivation for creating these classes was:
- make a convenient interface for creating stubs;
- add support for **NHibernate** specific methods: `Fetch`,` FetchMany`, `ThenFetch`,` ThenFetchMany`, `ToListAsync` (and other asynchronous request materializers), as well as the` DeleteAsync` method.

At the moment, commit / rollback transactions using UnitOfWork is not supported, i.e. interface methods `IUnitOfWork` do not have any effect on the contents of the repository.
There is some groundwork in the code to make available information about the fact of calling the `Commit` and` Rollback` methods, but this is left as a point for the development of the library.