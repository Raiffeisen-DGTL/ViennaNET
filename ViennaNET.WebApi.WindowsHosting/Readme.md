# Build with extension to run services as a Windows service

Kestrel and based on HttpSys

#### Application Example
In the main Program.cs file, you must call the RunAsWindowsService extension method for the configured IWebHost:

	DefaultKestrelRunner.Configure ()
		.BuildWebHost (arg)
		.RunAsWindowsService (arg);