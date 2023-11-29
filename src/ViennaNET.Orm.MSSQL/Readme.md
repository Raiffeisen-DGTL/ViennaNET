# Build to create a connection to the MsSql database through NHibernate

### Key Entities

* **MsSqlSessionFactoryProvider**. It configures NHibernate parameters to create **SessionFactory** for the MS SQL database.
* **MsSqlSessionFactoryProviderGetter**. Factory to create **MsSqlSessionFactoryProvider**.

### Breaking changes
* Switched to **Microsoft.Data.SqlClient**, it means that you should set Encrypt parameter in ConnectionString to correct value, because now it is **true** by default (https://github.com/dotnet/SqlClient/blob/main/release-notes/4.0/4.0.0.md#breaking-changes)