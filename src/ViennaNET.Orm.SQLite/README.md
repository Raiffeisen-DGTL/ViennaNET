# Build to create a connection to self-contained SQLite database via NHibernate

https://www.sqlite.org/

The database can be created both in a file and in memory.

### Key Entities

* **SQLiteSessionFactoryProvider**. Configures NHibernate parameters to create **SessionFactory** for the SQLite database.
* **SQLiteSessionFactoryProviderGetter**. Factory to create **SQLiteSessionFactoryProvider**.


### Database Tools

* https://sqlitebrowser.org/
