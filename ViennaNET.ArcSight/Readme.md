# Build to connect the service to ArcSight. Contains classes that allow you to send a standard message in CEF format to the data channel.

### Key Entities

The main class is **ArcSightClient**. It allows you to serialize the incoming message in the format specified by the settings.

Settings specified in the configuration:
* Domain name of the server to which data is transmitted
* Server port
* Data serialization protocol version
* Type of transport protocol

For sending, the CefMessage class is used. The class contains all the information needed to identify an incident with ArcSight. In case of an entity validation error, an exception **CefMessageValidationException** is thrown

#### Instructions for use:
1. Add a dependency on **IArcSightClient** to the class.
2. Add the configuration file **appsettings.json**,

{
"arcSight": {
           "serverHost": "localhost",
           "serverPort": "60",
           "syslogVersion": "rfc3164",
           "protocol": "tcp" }
}

3. Create an instance of the **CefMessage** class. The message is automatically validated upon creation.
4. Calls the **Send** method of the interface **IArcSightClient**, passing the message created in step 3 there.

### Usage example

  public class ArcSightSendingService: IArcSightSendingService
  {
    private readonly IArcSightClient _arcSightClient;
    private readonly IMapperFactory _mapperFactory;

    public ArcSightSendingService (IArcSightClient arcSightClient, IMapperFactory mapperFactory)
    {
      _arcSightClient = arcSightClient.ThrowIfNull("arcSightClient");
      _mapperFactory = mapperFactory.ThrowIfNull("mapperFactory");
    }

    public void SendViewingEvent(LoggingMessage message)
    {
      var mapper = _mapperFactory.Create<LoggingMessage, ICollection <CEFMessage>>();

      var cefMessages = mapper.Map(message);

      foreach (var cefMessage in cefMessages)
      {
        _arcSightClient.Send(cefMessage);
      }
    }
  }

### Syslog Serialization Formats
* RFC 3164
* RFC 5424