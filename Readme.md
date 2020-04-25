# Repo, containing .NET Standard based infrastructure libs

![Build&Test](https://github.com/Raiffeisen-DGTL/ViennaNET/workflows/Build&Test/badge.svg?branch=master)

### Table of contents

#### ArcSight
*  [**ViennaNET.ArcSight**](ViennaNET.ArcSight) - library with ArcSight connection logic
*  **ViennaNET.ArcSight.DefaultConfiguration** - assembly integration into the project through SimpleInjector

#### Mediator (inner bus)
*  [**ViennaNET.Mediator**](ViennaNET.Mediator) - implementation of the internal message bus
*  **ViennaNET.Mediator.DefaultConfiguration** - assembly integration into the project through SimpleInjector
*  [**ViennaNET.Mediator.Seedwork**](ViennaNET.Mediator.Seedwork) - assembly with interfaces

#### DAL
*  [**ViennaNET.Orm**](ViennaNET.Orm) - DAL, wrapper over NHibernate
*  **ViennaNET.Orm.DefaultConfiguration** - build integration into the project through SimpleInjector
*  [**ViennaNET.Orm.Seedwork**](ViennaNET.Orm.Seedwork) - assembly with interfaces

> MSSql:
> *  [**ViennaNET.Orm.MSSQL**](ViennaNET.Orm.MSSQL) - connection of the MSSql driver
> *  **ViennaNET.Orm.MSSQL.DefaultConfiguration** - build integration into the project through SimpleInjector

> Oracle:
> *  [**ViennaNET.Orm.Oracle**](ViennaNET.Orm.Oracle) - connecting the Oracle driver
> *  **ViennaNET.Orm.Oracle.DefaultConfiguration** - build integration into the project through SimpleInjector

> SQLite:
> *  [**ViennaNET.Orm.SQLite**](ViennaNET.Orm.SQLite) - connecting the SQLite driver
> *  **ViennaNET.Orm.SQLite.DefaultConfiguration** - build integration into the project through SimpleInjector


#### Redis
*  [**ViennaNET.Redis**](ViennaNET.Redis) - services for working with Redis
*  **ViennaNET.Redis.DefaultConfiguration** - assembly integration into the project through SimpleInjector

#### WebApi
*  [**ViennaNET.WebApi**](ViennaNET.WebApi) - WebApi-service builder with common AspNetCore & Swagger features enabled
*  **ViennaNET.HttpClient** - Http-client builder, integrated in standard AspNetCore DI-container

#### Security
* **ViennaNET.Security** - base security interface abstractions library
* **ViennaNET.Security.Jwt** - token factory 

#### Logging
* **ViennaNET.Logging** - a logging library based on log4net


#### Useful Utilities
* **ViennaNET.Utils** - contains useful extension methods and attributes
* [**ViennaNET.Validation**](ViennaNET.Validation) - implementation of validation services
* **ViennaNET.Specifications** - library for creating and using specifications
* [**ViennaNET.Sagas**](ViennaNET.Sagas) - basic implementation of the sagas mechanism
