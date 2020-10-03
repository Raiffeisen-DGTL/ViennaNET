# ViennaNET.Orm.DB2.Lnx.DefaultConfiguration
## Assembly for registering DB2 implementation of ISessionFactoryProviderGetter under **Windows**

#### Instructions for use:

##### Development, Debugging, and Deployment for Windows
If the development is under Windows and for Windows, then **two** packages must be connected to the **infrastructure** layer of the project: **ViennaNET.Orm.DB2.Win.DefaultConfiguration** and **ViennaNET.Orm.DB2.Win**.
This is due to the fact that the ViennaNET.Orm.DB2.Win package contains a driver for connecting to DB2, and if this package is connected transitively, the driver will not get into the assembly folder and the application will not work.
When directly connecting the ViennaNET.Orm.DB2.Win assembly, the driver folder will be automatically connected to the project and copied to the assembly directory.

##### Develop, debug on Windows, but deploy on Linux
If the development is under Windows, but the application is deployed under Linux, then everything is somewhat more complicated: depending on the execution environment, you need to connect either *Lin-packages or *Win-packages
1. In the **infrastructure** layer of the project, you must first connect the following packages: **ViennaNET.Orm.DB2.Win.DefaultConfiguration** and **ViennaNET.Orm.DB2.Win**.
2. Add a section to the infrastructure *.csproj

```
	<PropertyGroup>
		<RuntimeIdentifiers>win-x64;linux-x64</RuntimeIdentifiers> -- Describing the list of runtime environments
		<RuntimeIdentifier>win-x64</RuntimeIdentifier> -- Specify the current runtime
	</PropertyGroup>
```

3. Find the section with Nuget packages in the same place
There should be something like this:

```
	<PackageReference Include="ViennaNET.Orm.DB2.Win" Version="0.1.4171.15418" />
	<PackageReference Include="ViennaNET.Orm.DB2.Win.DefaultConfiguration" Version="0.1.4171.15418" />
```

Replace with

```
	<PackageReference Include="ViennaNET.Orm.DB2.Lnx" Version="0.1.4171.15418" Condition="'$ (RuntimeIdentifier)' == 'linux-x64'" />
	<PackageReference Include="ViennaNET.Orm.DB2.Lnx.DefaultConfiguration" Version="0.1.4171.15418" Condition="'$ (RuntimeIdentifier)' == 'linux-x64'" />
	<PackageReference Include="ViennaNET.Orm.DB2.Win" Version="0.1.4171.15418" Condition="'$ (RuntimeIdentifier)' == 'win-x64'" />
	<PackageReference Include="ViennaNET.Orm.DB2.Win.DefaultConfiguration" Version="0.1.4171.15418" Condition="'$ (RuntimeIdentifier)' == 'win-x64'" />
```

> Check to make sure the versions match!

##### Build the release version on the pipeline:

In the build script, when calling dotnet build, you must specify an additional parameter with the runtime identifier (RID):

```
		dotnet build -r win-x64
```		