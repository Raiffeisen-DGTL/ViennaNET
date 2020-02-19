# Build to connect SimpleInjector as DI / IoC to the application

Contains:
* SimpleInjectorConfigurator - Configurator for integrating SimpleInjector into the service. Configuring integration with built-in DI, searching and initializing packages (installers), container validation
* SimpleInjectorConfiguration - configuration section for setting up SimpleInjector injection into the service

The loadPackagesDynamically flag is responsible for enabling automatic loading of packages from all assemblies that are at the root of the application folder, so that all dependencies get to the application folder, you need to add the parameter to the service project file:

    <PropertyGroup>
      <CopyLocalLockFileAssemblies> true </CopyLocalLockFileAssemblies>
    </PropertyGroup>

By default, the flag is considered set to false, which means that all dependencies must be connected explicitly, manually, using the extension method for the container from the ViennaNET.SimpleInjector.Extensions assembly in the root service package, for example:

    container.AddPackage (new DiagnosingPackage ())
             .AddPackage (new RabbitMqPackage ())
             .AddPackage (new MessagingPackage ())