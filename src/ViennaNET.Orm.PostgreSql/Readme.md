# Build to create a connection to the PostgreSql database via NHibernate

### Key Entities

* **PostgreSqlSessionFactoryProvider**. It configures NHibernate parameters to create **SessionFactory** for the PostgreSql database.
* **PostgreSqlSessionFactoryProviderGetter**. Factory to create **PostgreSqlSessionFactoryProvider**.

**Npgsql 6.*** no longer supports date/time representations which cannot be fully round-tripped to the database. If it can't be fully stored as-is, you can't write it.
A compatibility switch enables opting out of the new behavior, to maintain backwards compatibility.