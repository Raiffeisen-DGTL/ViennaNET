# Repo, containing .NET Standard based infrastructure libs

[![Build&Test](https://github.com/Raiffeisen-DGTL/ViennaNET/workflows/Build&Test/badge.svg?branch=master)](https://github.com/Raiffeisen-DGTL/ViennaNET/actions?query=workflow%3ABuild%26Test)

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=alert_status)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=security_rating)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Reliability Rating](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=reliability_rating)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)

[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=coverage)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Bugs](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=bugs)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Code Smells](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=code_smells)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Duplicated Lines (%)](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=duplicated_lines_density)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Lines of Code](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=ncloc)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)     [![Technical Debt](https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=sqale_index)](https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET)


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
