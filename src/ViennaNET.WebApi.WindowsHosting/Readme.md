# Build with extension to run services as a Windows service

#### Application Example
In the main Program.cs file, you must call the RunAsWindowsService extension method for the configured IWebHost:

	BaseHttpSysRunner.Configure()
        .UseWindowsService(args)
        .BuildWebHost(args)
        .Run();