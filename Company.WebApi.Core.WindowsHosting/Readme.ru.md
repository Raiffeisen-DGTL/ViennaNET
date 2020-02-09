# Сборка с расширением для запуска сервисов как Windows-службы

Работает как для сервисов на основе Kestrel так и на основе HttpSys

#### Пример применения
В корневом файле Program.cs необходимо вызвать метод-расширение RunAsWindowsService для сконфигурированного IWebHost :

		DefaultKestrelRunner.Configure()
                        .BuildWebHost(args)
                        .RunAsWindowsService(args);