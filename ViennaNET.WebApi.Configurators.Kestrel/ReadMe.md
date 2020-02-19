# Build with Kestrel North settings

Contains:
* KestrelConfigurator - Configurator for registering Kestrel as a server. For hosting both under Windows and under Linux
* WebApiConfiguration - section in the configuration file

```
    "webApiConfiguration": {
      "portNumber": 80,
      "httpsPort": 8080,
      "useStrictHttps": false,
      "certificatePath": "RootCA.crt"
    },
```

To enable https, you need to add the port for https **httpsPort** in the **webApiConfiguration** configuration section.
There is also an option **useStrictHttps** - when setting the value **true**, all requests via http will be forcibly redirected to https.
The **useHsts** option set to false allows you to disable sending the HSTS header, which activates forced redirects to HTTPS from the browser to the current host.
The **certificatePath** parameter is responsible for the path to the certificate for enabling https under Linux.
