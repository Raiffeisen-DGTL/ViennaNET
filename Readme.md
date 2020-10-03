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

#### Messaging
*  [**ViennaNET.Messaging**](ViennaNET.Messaging) - assembly with classes and interfaces providing a common functionality for working with queues
*  [**ViennaNET.Messaging.DefaultConfiguration**](ViennaNET.Messaging.DefaultConfiguration) - assembly integration into the project through SimpleInjector

> IBM MQ:
> *  [**ViennaNET.Messaging.MQSeriesQueue**](ViennaNET.Messaging.MQSeriesQueue) - assembly providing work with the IBM MQ Series queue
> *  [**ViennaNET.Messaging.MQSeriesQueue.DefaultConfiguration**](ViennaNET.Messaging.MQSeriesQueue.DefaultConfiguration) - assembly integration into the project through SimpleInjector

> Kafka:
> *  [**ViennaNET.Messaging.Kafka**](ViennaNET.Messaging.MQSeriesQueue) - assembly providing work with the Kafka queue
> *  [**ViennaNET.Messaging.Kafka.DefaultConfiguration**](ViennaNET.Messaging.Kafka.DefaultConfiguration) - assembly integration into the project through SimpleInjector

> RabbitMQ:
> *  [**ViennaNET.Messaging.RabbitMQ**](ViennaNET.Messaging.RabbitMQ) - assembly providing work with the RabbitMQ queue
> *  [**ViennaNET.Messaging.RabbitMQ.DefaultConfiguration**](ViennaNET.Messaging.RabbitMQ.DefaultConfiguration) - assembly integration into the project through SimpleInjector

#### ORM
*  [**ViennaNET.Orm**](ViennaNET.Orm) - DAL, wrapper over NHibernate
*  **ViennaNET.Orm.DefaultConfiguration** - build integration into the project through SimpleInjector
*  [**ViennaNET.Orm.Seedwork**](ViennaNET.Orm.Seedwork) - assembly with interfaces

> DB2:
> *  [**ViennaNET.Orm.DB2**](ViennaNET.Orm.DB2) - connection of the DB2 driver
> *  [**ViennaNET.Orm.DB2.Win**](ViennaNET.Orm.DB2.Win.DefaultConfiguration) - contains a driver for connecting to IBM DB2 under Windows
> *  [**ViennaNET.Orm.DB2.Lnx**](ViennaNET.Orm.DB2.Lnx.DefaultConfiguration) - contains a driver for connecting to IBM DB2 under Linux

> MSSql:
> *  [**ViennaNET.Orm.MSSQL**](ViennaNET.Orm.MSSQL) - connection of the MSSql driver
> *  [**ViennaNET.Orm.MSSQL.DefaultConfiguration**](ViennaNET.Orm.MSSQL.DefaultConfiguration) - assembly integration into the project through SimpleInjector

> Oracle:
> *  [**ViennaNET.Orm.Oracle**](ViennaNET.Orm.Oracle) - connecting the Oracle driver
> *  [**ViennaNET.Orm.Oracle.DefaultConfiguration**](ViennaNET.Orm.Oracle.DefaultConfiguration) - assembly integration into the project through SimpleInjector

> PostgreSQL:
> *  [**ViennaNET.Orm.PostgreSql**](ViennaNET.Orm.PostgreSql) - connecting the PostgreSql driver
> *  [**ViennaNET.Orm.PostgreSql.DefaultConfiguration**](ViennaNET.Orm.PostgreSql.DefaultConfiguration) - assembly integration into the project through SimpleInjector

> SQLite:
> *  [**ViennaNET.Orm.SQLite**](ViennaNET.Orm.SQLite) - connecting the SQLite driver
> *  [**ViennaNET.Orm.SQLite.DefaultConfiguration**](ViennaNET.Orm.SQLite.DefaultConfiguration) - assembly integration into the project through SimpleInjector

#### Redis
*  [**ViennaNET.Redis**](ViennaNET.Redis) - services for working with Redis
*  **ViennaNET.Redis.DefaultConfiguration** - assembly integration into the project through SimpleInjector

#### Security
* **ViennaNET.Security** - base security interface abstractions library
* **ViennaNET.Security.Jwt** - token factory 

#### Useful Utilities
* **ViennaNET.Utils** - contains useful extension methods and attributes
* [**ViennaNET.Validation**](ViennaNET.Validation) - implementation of validation services
* **ViennaNET.Specifications** - library for creating and using specifications
* [**ViennaNET.Sagas**](ViennaNET.Sagas) - basic implementation of the sagas mechanism

#### WebApi
*  [**ViennaNET.WebApi**](ViennaNET.WebApi) - WebApi-service builder with common AspNetCore & Swagger features enabled
*  **ViennaNET.HttpClient** - Http-client builder, integrated in standard AspNetCore DI-container


# Quickstart

### Examples
* [**Simple empty microservice example**](Examples/1-empty-service)
* [**Example of microservice with Mediator and SimpleInjector**](Examples/2-mediator)
* [**Example of microservice with validation**](Examples/3-validation)
* [**Example of microservice with database**](Examples/4-orm)
* [**Example of microservice with simple saga implementation**](Examples/5-saga)
* [**Example of microservice with messaging**](Examples/6-messaging)

### Description
* [**ViennaNET part1 (WebApi, Mediator, Validation, Redis, Specifications)**](https://habr.com/ru/company/raiffeisenbank/blog/494830/)
* [**ViennaNET part2 (Sagas, Orm, Messaging, CallContext)**](https://habr.com/ru/company/raiffeisenbank/blog/516540/)

### [Video materials](https://github.com/Raiffeisen-DGTL/ViennaNET/wiki/Video-materials)


# Let's contribute

* [**Open issues**](https://github.com/Raiffeisen-DGTL/ViennaNET/issues)
* [**Codestyle conventions**](https://github.com/Raiffeisen-DGTL/ViennaNET/wiki/Codestyle-conventions)
* [**ReleaseNotes**](ReleaseNotes.md)
* [**Contributors**](https://github.com/Raiffeisen-DGTL/ViennaNET/wiki/Contributors)
