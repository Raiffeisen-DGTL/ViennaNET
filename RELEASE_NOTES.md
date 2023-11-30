# v0016 (03.03.2022)
1. Add missed host lifetime hook: ApplicationStopping
2. Rename AddOnStopAction to AddOnStoppedAction for ApplicationStopped lifetime hook

# v0015 (25.01.2022)
1. Remove using of log4net from all Orm libraries (see ViennaNET.Orm docs for details)
2. Add ViennaNET.Orm.DB2.Osx for macOs users
3. Breaking change: ViennaNET.Orm.MSSQL updated to Microsoft.Data.SqlClient from System.Data.SqlClient, it means that you should set Encrypt parameter in ConnectionString to correct value, because now it is **true** by default (https://github.com/dotnet/SqlClient/blob/main/release-notes/4.0/4.0.0.md#breaking-changes)
4. Breaking change: fix namespace ViennaNET.Orm.DB2.Win.DefaultConfiguration to ViennaNET.Orm.DB2.DefaultConfiguration for ViennaNET DB2 Windows libs

# v0014 (08.04.2021)
1. Add ViennaNET.Messaging.ActiveMQ, split ViennaNET.Messaging.Tests
2. Extension ViennaNET.Redis, ViennaNET.Utils.Orm

# v0013 (01.03.2021)
1. Minor changes in ViennaNET.Messaging, ViennaNET.Messaging.RabbitMQQueue, ViennaNET.Orm

# v0012 (04.02.2021)
1. Minor changes in ViennaNET.Messaging, ViennaNET.Messaging.MQSeriesQueue, ViennaNET.Orm, ViennaNET.Redis, ViennaNET.WebApi
2. Refactor logging

# v0011 (27.11.2020)

### Release Summary
1. Various small bugfixes in Excel, Messaging and WebApi


# v0010 (29.10.2020)

### Release Summary
1. Change Messaging


# v0009 (4.10.2020)

### Release Summary
1. Added ORM DB2 integration
2. Change Messaging pooling process
3. Added LoggingAdapterProvider - wrapper over ViennaNET.Logging.Logger to switch to standard Microsoft.Extensions.Logging.ILogger<T>. Now ILogger<T> can be used, but internally it will work like ViennaNET.Logging.Logger.
After the complete translation of the libraries to ILogger<T>, the ViennaNET.Logging library will be abandoned

### Breaking Changes
In order for the current logging option to continue working as it is, you need to add the section to the service configuration file:

```
    "Logging": {
        "UseLegacyLogger": true,
        "LogLevel": {
          "Default": "Trace",
          "Microsoft": "Warning",
          "Microsoft.Hosting.Lifetime": "Information"
        }
    }
```	

# v0007 (30.07.2020)

### Release Summary
1. For Rabbit queues, added the ability to create additional bindings through the routings configuration. 
Bindgin is the link between the exchange point and the queue. For exchageType direct and topic, 
you can create multiple peer-to-queue connections, and when sending a message with different rountigKey values ​​to the peer, 
messages to one or more queues. The direct type requires a complete match of rountigKey with the name of the link between 
the exchange point and the queue. Whereas topic allows you to use a pattern such as \*.green.\* In the name of the connection.
This revision allows you to create additional links of the exchange point with the queue to the default link, 
which has the name of the queue identifier from the configuration. This is necessary in order to be able to control where 
the message will go when it is sent to the exchange point in one or more queues, with the same configuration of routes from 
the exchange point to the queues.
2. For HttpClients added Basic-authentication
3. Added optional loading of an additional configuration file, which is selected by the value of the environment variable:
   https://docs.microsoft.com/en-us/aspnet/core/fundamentals/environments?view=aspnetcore-3.1

# v0006 (30.06.2020)

### Release Summary
1. MQSeriesQueue - selectors improved
2. Protection - added the ability to encrypt passwords for connecting to databases
3. HttpClients.NoAuthentication - added the ability to anonymously connect to other services
4. WebApi.CompanyHostBuilder - add enironment variables to configuration

# v0005 (29.05.2020)

### Breaking Changes
Changed CallContextHeaders headers. In case of their use, it is necessary to take into account the new formats.


# v0004 (25.04.2020)

### Release Summary
We have added ICallContext. This abstraction receive and hold some context data such as request id, user name, domain and so on to pass in microservices by different channels: Http-requests and queue messaging.
SecurityContextFactories now uses this context to build authentication data.

### Breaking Changes
If you used manual installation of SimpleInjector packages, you will find that DiagnosticPackage is deleted: since this release it register automatically. Just delete DiagnosticPackage registration in your code.


# v0003 (26.03.2020)

### Release Summary
Add more libraries


# v0002 (19.02.2020)

### Release Summary
Rebranding Company.Framework to ViennaNET.
ViennaNET.WebApi.\* Was made based on AspNetCore 3, and .NET Core 3.1 is recommended for its use.
Assemblies with configurators were renamed to ViennaNET.WebApi.Configurators.\*, And some of them were merged or split.
In the new version, the use of composite configurators has become easier, for example, if earlier it was necessary to call several extension methods, now only one public connection method remains in each configurator.

### Breaking Changes
1. AspNetCore 3 is not compatible with the classic .NET Framework, only with .NET Core 2+.
2. In the new version of the ViennaNET.WebApi. * Libraries, some incompatible changes have been made. Assemblies with configurators were renamed ViennaNET.WebApi.Configurators. *, As well as some of them were merged or divided. To migrate existing services to new versions of these libraries, you must perform the migration.
3. In SecurityKeysContainer, the default values ​​for the publisher and the audience have changed, which means that you need to check the compatibility of these parameters with existing authorization services. It is better in the configuration to explicitly override these fields as in all services.


### Migration of AspNetCore applications from ViennaNET.WebApi 1.\*.\* (NET Core 2.2) to 2.\*.\* (NET Core 3.1)

#### Case I. Used by DefaultKestrelRunner or DefaultHttpSysRunner
1. In all assemblies using <TargetFramework>netcoreapp2.2</TargetFramework>, change to <TargetFramework>net6.0</TargetFramework>.
2. In the main assembly with the service, delete the link to the ViennaNET.WebApi.DefaultConfiguration package for the service on Kestrel or ViennaNET.WebApi.DefaultHttpSysRunner for the service on HttpSys.
3. Install the latest version of the package (2. \*.\*) ViennaNET.WebApi.Runners.BaseKestrel or ViennaNET.WebApi.Runners.BaseHttpSys, respectively.
4. We also update versions of Company libraries to (2. \*.\*) In all assemblies. Version conflicts with third-party libraries (for example, SimpleInjector) may occur, so they also need to be updated.
5. Redoing Program.cs:
  * Replace DefaultKestrelRunner with BaseKestrelRunner (for DefaultHttpSysRunner - BaseHttpSysRunner)
  * If you used AddOnStartAction<T> / AddOnStopAction<T> extension methods, then you need to connect the ViennaNET.WebApi.Runners.Extensions package
6. We redo the configuration file:
  * Remove unnecessary parameters from the webApiConfiguration section: logRequests and swaggerSubmit
  * If you want to use the dynamic search for SimpleInjector packages, as is happening now, you need to add a section:

```
       "simpleInjector": {
         "loadPackagesDynamically": true
       },
```

   The loadPackagesDynamically flag is responsible for enabling automatic loading of packages from all assemblies that are at the root of the application folder, so that all dependencies get to the application folder, you need to add the parameter to the service project file:

```
       <PropertyGroup>
          <CopyLocalLockFileAssemblies> true </CopyLocalLockFileAssemblies>
       </PropertyGroup>
```

   By default, the flag is considered set to false, which means that all dependencies must be connected explicitly manually, using the extension method for the container from the ViennaNET.SimpleInjector.Extensions assembly in the root service package, for example:

```csharp
       container.AddPackage(new DiagnosingPackage())
                .AddPackage(new RabbitMqPackage())
                .AddPackage(new MessagingPackage())
```

   In this case, there is no need to use the CopyLocalLockFileAssemblies parameter
7. The swagger enable flag has been moved from the webApiConfiguration section to the swagger section with the renaming swaggerSubmit -> useSwagger

```
       "swagger": {
         "useSwagger": true,
         ...
       },
```

8. If you used HttpClients through the ViennaNET.WebApi.Configurators.HttpClients.Jwt/Ntlm assemblies, you need to add a parameter with an authentication type to the section of each webApiEndpoints:

```
       "webApiEndpoints": [
         ...
         {
           ...
           "authType": "jwt"
         },
         {
           ...
           "authType": "ntlm"
         },
         ...
       ],
```

#### Case II. The service was built manually (without using DefaultKestrelRunner or DefaultHttpSysRunner)
1) In all assemblies using <TargetFramework>netcoreapp2.2</TargetFramework>, change to <TargetFramework>net6.0</TargetFramework>.
2) Assemblies with configurators were renamed to ViennaNET.WebApi.Configurators.\*, As well as some of them were merged or divided, so you need to familiarize yourself with their description and select the ones you need.

   Example:

```csharp
       return CompanyHostBuilder.Create()
                                .UseKestrel()
                                .UseCommonModule()
                                .UseSimpleInjector()
                                .UseSwaggerWithJwtAuth()
                                .UseDiagnosing()
                                .UseJwtHttpClients()                                
                                .UseJwtAuth();
```                                

3) We update versions of Company libraries to (2. \*.\*) In all assemblies. Version conflicts with third-party libraries (for example, SimpleInjector) may occur, so they need to be synchronized.


# v0001 (14.01.2020)

### Release Summary
MVP Company.Framework on NET Core 2.2
