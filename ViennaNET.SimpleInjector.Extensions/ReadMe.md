# Build to connect SimpleInjector as DI/IoC to the application

For the configurator to work correctly in the .csproj assembly with the service, you must add the line

```
      <CopyLocalLockFileAssemblies>true</CopyLocalLockFileAssemblies>
```

It requires during assembly to copy assemblies from downloaded nuget packages to the output folder so that SimpleInjector can see them and register services in the container.
