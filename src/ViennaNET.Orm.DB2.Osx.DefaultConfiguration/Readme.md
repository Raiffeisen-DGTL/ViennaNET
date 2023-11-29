# ViennaNET.Orm.DB2.Osx.DefaultConfiguration
## Assembly to register DB2 implementation of ISessionFactoryProviderGetter under **MacOS X**

#### Instructions for use:

##### Development, Debugging and Working under MacOS X
If development is under MacOS X and for Linux, then **two** packages must be connected to the **infrastructure** layer of the project: **ViennaNET.Orm.DB2.Osx.DefaultConfiguration** and **ViennaNET.Orm.DB2.Osx**.
This is due to the fact that the ViennaNET.Orm.DB2.Osx package contains a driver for connecting to DB2, and if this package is connected transitively, the driver will not get into the assembly folder and the application will not work.
When directly connecting the ViennaNET.Orm.DB2.Osx assembly, the driver folder will be automatically connected to the project and copied to the assembly directory.

##### Develop, debug on MacOS X, but deploy on Linux
If the development is under MacOS X, but the application is deployed under Linux, then everything is somewhat more complicated: depending on the execution environment, you need to connect either *Lin-packages or *Osx-packages
1. In the **infrastructure** layer of the project, you must first connect the folloosxg packages: **ViennaNET.Orm.DB2.Osx.DefaultConfiguration** and **ViennaNET.Orm.DB2.Osx**.
2. Add a section to the infrastructure *.csproj

```
<PropertyGroup>
	<RuntimeIdentifiers>osx-x64;linux-x64</RuntimeIdentifiers> -- Describing the list of runtime environments
	<RuntimeIdentifier>osx-x64</RuntimeIdentifier> -- Specify the current runtime
</PropertyGroup>
```

3. Find the section with Nuget packages in the same place
There should be something like this:

```
<PackageReference Include="ViennaNET.Orm.DB2.Osx" Version="0.1.4171.15418" />
<PackageReference Include="ViennaNET.Orm.DB2.Osx.DefaultConfiguration" Version="0.1.4171.15418" />
```

Replace with

```
<PackageReference Include="ViennaNET.Orm.DB2.Osx" Version="0.1.4171.12770" Condition="'$ (RuntimeIdentifier)' == 'linux-x64'" />
<PackageReference Include="ViennaNET.Orm.DB2.Osx.DefaultConfiguration" Version="0.1.4171.12770" 
                  Condition="'$ (RuntimeIdentifier)' == 'linux-x64'" />
<PackageReference Include="ViennaNET.Orm.DB2.Osx" Version="0.1.4171.15418" Condition="'$ (RuntimeIdentifier)' == 'osx-x64'" />
<PackageReference Include="ViennaNET.Orm.DB2.Osx.DefaultConfiguration" Version="0.1.4171.15418" 
                  Condition="'$ (RuntimeIdentifier)' == 'osx-x64'" />
```

> Check to make sure the versions match!

##### Build the release version on the pipeline:

In the build script, when calling dotnet build, you must specify an additional parameter with the runtime identifier (RID):

```
dotnet build -r linux-x64
```		
