# Extension library for running your app as Windows service 

Works for Kestrel-hosted and HttpSys-hosted services

#### Example
In main Program.cs you need to call RunAsWindowsService extension method for the configured IWebHost :

		DefaultKestrelRunner.Configure()
                        .BuildWebHost(args)
                        .RunAsWindowsService(args);