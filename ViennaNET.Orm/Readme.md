# An assembly containing an intermediary implementation for performing service operations

### Key Entities

* **SessionFactoryProvider** - performs the following functions:
1. Configures the basic parameters of NHibernate.
1. Registers entity mappings. Supports Fluent syntax.
1. Based on 1 and 2 points creates **SessionFactory**.

* **ScopedSessionManager** - provides the life cycle of NHibernate sessions within a web request. It is an analog of the Nhibernate session context. Created because none of the contexts is fully suitable for solving the problem.

* **EntityFactoryService** - creates various wrappers for working with the repository. The created interfaces are abstracted from the type of storage.

* **EntityRepository** - an abstract repository for working with the repository as a collection of objects. Contains additional infrastructure methods.

* **UnitOfWork** - is an implementation of the "Unit of Work" template.

#### Instructions for use:

1. Add a dependency on **IEntityFactoryService** to the class.
2. Add the configuration file **appsettings.json**,

```javascript
    "db": [
      {
        "nick": "default",
        "dbServerType": "DB2",
        "ConnectionString": 
            "Database = DBNAME; UserID = user_name; Server = servername: 50000; CurrentSchema = SCHEME;",
        "useCallContext": true
      },
      {
        "nick": "mssql",
        "dbServerType": "MSSQL",
        "ConnectionString": "Server = servername, 1433; Database = DBNAME; User ID = user_name;"
      }
    ]
```

For each connection to the database, a separate list item is created containing the following parameters:
* nick - the name of the connection to the database. Unique identificator.
* dbServerType - type of database to which the connection is made.
* connectionString - connection string to the database
* encPassword - encrypted password. Substituted in the connection string after decryption. If the connection string already has a clear password, it will be replaced with the decrypted one. If you do not intend to encrypt the password, the field should be blank.
* useCallContext - use an additional task of the execution context if the provider of a particular database supports this option.

3a. If you just need to read the entity from the database, then just create a repository and call the Get method, passing the Id into it.

```csharp
return _entityFactoryService.Create<Entity>()
                                .Get(message.Id);
```

3b. If you want to read database entities with filtering, you can use the LINQ query.

```csharp
    return _entityFactoryService.Create<Entity>()
           .Query().Where(x => x.Status == EntityStatus.Processed && x.UpdateDate < DateTime.Today);
```

4a. In the event of an entity change, a unit of work must be created. Changes in the fields of an entity must occur in it.

```csharp
     using (var unitOfWork = _entityFactoryService.Create())
     {
       var entity = _entityFactoryService.Create<Entity>().Get(message.Id);

       entity.DoStaff(message);

       unitOfWork.Commit();
     }
```

4b. If an entity is created, use the Add method to add it.

```csharp
     using (var unitOfWork = _entityFactoryService.Create())
     {
       var entityRepository = _entityFactoryService.Create<Entity>();

       var entity = Entity.Create(message);

       entityRepository.Add(entity);

       unitOfWork.Commit();
     }
```

4c. The Add method, if there is an entity with the same identifier in the collection, will try to execute the Update request. If this behavior should be prohibited, you can use the Insert method, which in the same case will throw an exception.

5. You can delete an entity using the Delete method.

```csharp
     using (var unitOfWork = _entityFactoryService.Create())
     {
       var entityRepository = _entityFactoryService.Create<Entity>();

       var entity = entityRepository.Get(message.Id);

       entityRepository.Delete(entity);

       unitOfWork.Commit();
     }
```

6. When processing in manually created tasks, it is additionally necessary to create scope sessions. Without this, exceptions will occur during operation.

```csharp
    return await Task.Run(() =>
    {
      using (_entityFactoryService.GetScopedSession())
      {
        return _entityFactoryService.Create<Entity>()
                                    .Get(message.Id);
      }
    });
```

7. For the correct operation of the entity must be registered in a limited context of the service. To do this, create a class in the assembly

```csharp
    internal sealed class TestLogicsContext: ApplicationContext
    {
      public TestLogicsContext()
      {
        AddEntity<Card>();
        AddEntity<Product>("mssql");
      }
    }
```

and register it in the DI with the IBoundedContext interface.

```csharp
    container.Collection.Append<IBoundedContext, TestLogicsContext>(Lifestyle.Singleton);
```

#### Executing SQL queries:

To create a SQL query to the database, you need to create a query class.

```csharp
    internal class EntityDetailsQuery : BaseQuery<EntityDetailsItem>
    {
      private const string query = @"SELECT FIELD, ANOTHER_FIELD
              FROM SCHEME.ENTITY SE
              WHERE SE.WHERE_FIELD = :data";

      public EntityDetailsQuery(string data)
      {
        Parameters = new Dictionary<string, object>();
        Parameters["data"] = data;

        sql = query;
      }

      protected override EntityDetailsItem TransformTuple(object[] tuple, string[] aliases)
      {
        return new EntityDetailsItem(tuple[0].ToStringSafe(), (DateTime?)tuple[1]);
      }
    }
```

Next, you need to create a query executor and call it with the created instance of the command.

```csharp
    var query = new EntityDetailsQuery("keyData");

    var customQueryExecutor = _entityFactoryService.CreateCommandExecutor<EntityDetails>();

    IEnumerable <EntityDetails> entityDetails = customQueryExecutor.CustomQuery(query);
```

#### Executing SQL commands:

To create an SQL command to the database, you need to create a command class.

```csharp
    public class EntityCommand: BaseCommand
    {
      private EntityCommand()
      {
      }

      public static EntityCommand Create(int cnum)
      {
        return new EntityCommand
        {
          Parameters = new Dictionary<string, TypeWrapper>
          {
            {"P_CNUM", new TypeWrapper(cnum, typeof(int))}
          },

          Sql = "CALL SCHEME.ENTITY_COMMAND(:P_CNUM)"
        };
      }
    }
```

Next, you need to create an executor of commands and call it with the created instance of the command.

```csharp
    var command = new EntityCommand(123456);

    var commandExecutor = _entityFactoryService.CreateCommandExecutor<EntityCommand>();

    commandExecutor.Execute(command);
```
