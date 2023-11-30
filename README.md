# ViennaNET

ViennaNET is an open-source extensions of .NET and ASP.NET Core frameworks for quickly creating enterprise
microservices, such as RESTFull web api services, uses any databases, message brokers, etc...

[![Default ViennaNET release pipeline][49]][50]
[![Quality Gate Status][51]][52]

[![Maintainability Rating][53]][54]
[![Security Rating][55]][56]
[![Reliability Rating][57]][58]
[![Coverage][59]][60]

## Table of contents

### ArcSight

* [ViennaNET.ArcSight][1] - library with ArcSight connection logic
* [ViennaNET.ArcSight.DefaultConfiguration][2] - assembly integration into the project through SimpleInjector

### Mediator (inner bus)

* [ViennaNET.Mediator][3] - implementation of the internal message bus
* [ViennaNET.Mediator.DefaultConfiguration][4] - assembly integration into the project through SimpleInjector

### Messaging

* [ViennaNET.Messaging][5] - classes and interfaces providing a common functionality for working with queues
* [ViennaNET.Messaging.DefaultConfiguration][6] - integration through SimpleInjector

> ActiveMQ:
> * [ViennaNET.Messaging.ActiveMQQueue][7] - work with the ActiveMQ queue
> * [ViennaNET.Messaging.ActiveMQQueue.DefaultConfiguration][8] - integration through SimpleInjector

> IBM MQ:
> * [ViennaNET.Messaging.MQSeriesQueue][9] - work with the IBM MQ Series queue
> * [ViennaNET.Messaging.MQSeriesQueue.DefaultConfiguration][10] - integration through SimpleInjector

> Kafka:
> * [ViennaNET.Messaging.KafkaQueue][11] - work with the Kafka queue
> * [ViennaNET.Messaging.KafkaQueue.DefaultConfiguration][12] - integration through SimpleInjector

> RabbitMQ:
> * [ViennaNET.Messaging.RabbitMQQueue][13] - work with the RabbitMQ queue
> * [ViennaNET.Messaging.RabbitMQQueue.DefaultConfiguration][14] - integration through SimpleInjector

### ORM

* [ViennaNET.Orm][15] - DAL, wrapper over NHibernate
* [ViennaNET.Orm.DefaultConfiguration][16] - assembly integration into the project through SimpleInjector

> DB2:
> * [ViennaNET.Orm.DB2.Win][17] - driver for connecting to IBM DB2 under Windows
> * [ViennaNET.Orm.DB2.Lnx][18] - driver for connecting to IBM DB2 under Linux

> MSSql:
> * [ViennaNET.Orm.MSSQL][19] - connection of the MS SQL driver
> * [ViennaNET.Orm.MSSQL.DefaultConfiguration][20] - assembly integration into the project through SimpleInjector

> Oracle:
> * [ViennaNET.Orm.Oracle][21] - connecting the Oracle driver
> * [ViennaNET.Orm.Oracle.DefaultConfiguration][22] - assembly integration into the project through SimpleInjector

> PostgreSQL:
> * [ViennaNET.Orm.PostgreSql][23] - connecting the PostgreSql driver
> * [ViennaNET.Orm.PostgreSql.DefaultConfiguration][24] - assembly integration into the project through SimpleInjector

> SQLite:
> * [ViennaNET.Orm.SQLite][25] - connecting the SQLite driver
> * [ViennaNET.Orm.SQLite.DefaultConfiguration][26] - assembly integration into the project through SimpleInjector

### Redis

* [ViennaNET.Redis][27] - services for working with Redis
* [ViennaNET.Redis.DefaultConfiguration][28] - assembly integration into the project through SimpleInjector

### Security

* [ViennaNET.Security][29] - base security interface abstractions library
* [ViennaNET.Security.Jwt][30] - token factory

### Useful Utilities

* [ViennaNET.Utils][31] - contains useful extension methods and attributes
* [ViennaNET.Validation][32] - implementation of validation services
* [ViennaNET.Specifications][33] - library for creating and using specifications
* [ViennaNET.Sagas][34] - basic implementation of the sagas mechanism

### WebApi

* [ViennaNET.WebApi][35] - WebApi-service builder with common AspNetCore & Swagger features enabled
* [ViennaNET.HttpClient][36] - Http-client builder, integrated in standard AspNetCore DI-container

## Quickstart

## Examples

* [Simple empty microservice example][37]
* [Example of microservice with Mediator and SimpleInjector][38]
* [Example of microservice with validation][39]
* [Example of microservice with database][40]
* [Example of microservice with simple saga implementation][41]
* [Example of microservice with messaging][42]

## Description

* [ViennaNET part1 (WebApi, Mediator, Validation, Redis, Specifications)][43]
* [ViennaNET part2 (Sagas, Orm, Messaging, CallContext)][44]

## [Video materials][45]

## Let's contribute

* [Open issues][46]
* [Codestyle conventions][47]
* [Release notes][48]

[1]: <src/ViennaNET.ArcSight/> "Руководство по пакету ViennaNET.ArcSight"

[2]: <src/ViennaNET.ArcSight.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ArcSight в DI"

[3]: <src/ViennaNET.Mediator/> "Руководство по пакету ViennaNET.Mediator"

[4]: <src/ViennaNET.Mediator.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента Mediator в DI"

[5]: <src/ViennaNET.Messaging/> "Руководство по пакету ViennaNET.Messaging"

[6]: <src/ViennaNET.Messaging.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента Messaging в DI"

[7]: <src/ViennaNET.Messaging.ActiveMQQueue/> "Руководство по пакету ViennaNET.Messaging.ActiveMQQueue"

[8]: <src/ViennaNET.Messaging.ActiveMQQueue.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента Messaging.ActiveMQQueue в DI"

[9]: <src/ViennaNET.Messaging.MQSeriesQueue/> "Руководство по пакету ViennaNET.Messaging.MQSeriesQueue"

[10]: <src/ViennaNET.Messaging.MQSeriesQueue.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента Messaging.MQSeriesQueue в DI"

[11]: <src/ViennaNET.Messaging.KafkaQueue/> "Руководство по пакету ViennaNET.Messaging.KafkaQueue"

[12]: <src/ViennaNET.Messaging.KafkaQueue.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента Messaging.KafkaQueue в DI"

[13]: <src/ViennaNET.Messaging.RabbitMQQueue/> "Руководство по пакету ViennaNET.Messaging.RabbitMQQueue"

[14]: <src/ViennaNET.Messaging.RabbitMQQueue.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента Messaging.RabbitMQQueue в DI"

[15]: <src/ViennaNET.Orm/> "Руководство по пакету ViennaNET.Orm"

[16]: <src/ViennaNET.Orm.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm в DI"

[17]: <src/ViennaNET.Orm.DB2.Win.DefaultConfiguration/> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm.DB2.Win в DI"

[18]: <src/ViennaNET.Orm.DB2.Lnx.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm.DB2.Lnx в DI"

[19]: <src/ViennaNET.Orm.MSSQL/> "Руководство по пакету ViennaNET.Orm.MSSQL"

[20]: <src/ViennaNET.Orm.MSSQL.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm.MSSQL в DI"

[21]: <src/ViennaNET.Orm.Oracle/> "Руководство по пакету ViennaNET.Orm.Oracle"

[22]: <src/ViennaNET.Orm.Oracle.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm.Oracle в DI"

[23]: <src/ViennaNET.Orm.PostgreSql/> "Руководство по пакету ViennaNET.Orm.PostgreSql"

[24]: <src/ViennaNET.Orm.PostgreSql.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm.PostgreSql в DI"

[25]: <src/ViennaNET.Orm.SQLite/> "Руководство по пакету ViennaNET.Orm.SQLite"

[26]: <src/ViennaNET.Orm.SQLite.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Orm.SQLite в DI"

[27]: <src/ViennaNET.Redis/> "Руководство по пакету ViennaNET.Redis"

[28]: <src/ViennaNET.Redis.DefaultConfiguration> "Пакет SimpleInjector для регистрации клиента ViennaNET.Redis в DI"

[29]: <src/ViennaNET.Security> "Контекст безопасности"

[30]: <src/ViennaNET.Security.Jwt> "Контекст безопасности JWT"

[31]: <src/ViennaNET.Utils> "Некоторые вспомогательные программы"

[32]: <src/ViennaNET.Validation> "Компонент предоставляющий Api для создания валидаторов"

[33]: <src/ViennaNET.Specifications> "Компонент предоставляющий Api для созданя спецификаций"

[34]: <src/ViennaNET.Sagas> "Компонент предоставляющий Api для создания саги"

[35]: <src/ViennaNET.WebApi> "ViennaNET.WebApi"

[36]: <src/ViennaNET.HttpClient> "ViennaNET.HttpClient"

[37]: <src/Examples/1-empty-service> "Пример создания простой WebApi  службы на основе ViennaNET"

[38]: <src/Examples/2-mediator> "Пример использования ViennaNET.Mediator"

[39]: <src/Examples/3-validation> "Пример использования ViennaNET.Validation"

[40]: <src/Examples/4-orm> "Пример использования ViennaNET.Orm"

[41]: <src/Examples/5-saga> "Пример использования ViennaNET.Sagas"

[42]: <src/Examples/6-messaginga> "Пример использования ViennaNET.Messaging"

[43]: <https://habr.com/ru/company/raiffeisenbank/blog/494830/> "ViennaNET part1"

[44]: <https://habr.com/ru/company/raiffeisenbank/blog/516540/> "ViennaNET part2"

[45]: <https://github.com/Raiffeisen-DGTL/ViennaNET/wiki/Video-materials> "Video materials"

[46]: <https://github.com/Raiffeisen-DGTL/ViennaNET/issues> "Open issues"

[47]: <https://github.com/Raiffeisen-DGTL/ViennaNET/wiki/Codestyle-conventions> "Codestyle conventions"

[48]: <RELEASE_NOTES.md> "Release notes"

[49]: <https://github.com/Raiffeisen-DGTL/ViennaNET/actions/workflows/default.yml/badge.svg> "Pipeline status badge"

[50]: <https://github.com/Raiffeisen-DGTL/ViennaNET/actions/workflows/default.yml> "Default pipeline link"

[51]: <https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=alert_status> "Quality Gate Status badge"

[52]: <https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET> "Quality Gate Status link"

[53]: <https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=sqale_rating> "Maintainability rating badge"

[54]: <https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET> "Maintainability rating link"

[55]: <https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=security_rating> "Security Rating badge"

[56]: <https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET> "Security Rating link"

[57]: <https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=reliability_rating> "Reliability Rating badge"

[58]: <https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET> "Reliability Rating link"

[59]: <https://sonarcloud.io/api/project_badges/measure?project=Raiffeisen-DGTL_ViennaNET&metric=coverage> "Coverage badge"

[60]: <https://sonarcloud.io/dashboard?id=Raiffeisen-DGTL_ViennaNET> "Coverage link"