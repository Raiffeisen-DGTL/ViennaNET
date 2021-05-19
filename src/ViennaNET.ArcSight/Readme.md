# Сборка для подключения сервиса к ArcSight. Содержит классы, позволяющие отправить стандартное сообщение в формате CEF в канал передачи данных.

### Основные сущности

Основной класс - **ArcSightClient**. Он позволяет сериализовать входящее сообщение в формат, заданный настройками.

Настройки, задаваемые в конфигурации:
*  Доменное имя сервера, на который передаются данные
*  Порт сервера
*  Версия протокола сериализации данных
*  Тип транспортного протокола

Для отправки используется класс CefMessage. Класс содержит всю информацию, необходимую для идентификации инцидента системой ArcSight. В случае ошибки валидации сущности возникает исключение **CefMessageValidationException**

#### Инструкция по применению:
1.  Добавляем в класс зависимость от **IArcSightClient**.
2.  Добавляем файл конфигурации **appsettings.json**,  

		{
		 "arcSight": {
           "serverHost": "localhost",
           "serverPort": "60",
           "syslogVersion": "rfc3164",
           "protocol": "tcp"
		}

3.  Создаем экземпляр класса **CefMessage**. Сообщение автоматически валидируется при создании.
4.  Вызывает метод **Send** интерфейса **IArcSightClient**, передав туда созданное на шаге 3 сообщение.

### Пример использования

  public class ArcSightSendingService : IArcSightSendingService
  {
    private readonly IArcSightClient _arcSightClient;
    private readonly IMapperFactory _mapperFactory;

    public ArcSightSendingService(IArcSightClient arcSightClient, IMapperFactory mapperFactory)
    {
      _arcSightClient = arcSightClient.ThrowIfNull("arcSightClient");
      _mapperFactory = mapperFactory.ThrowIfNull("mapperFactory");
    }

    public void SendViewingEvent(LoggingMessage message)
    {
      var mapper = _mapperFactory.Create<LoggingMessage, ICollection<CEFMessage>>();

      var cefMessages = mapper.Map(message);

      foreach (var cefMessage in cefMessages)
      {
        _arcSightClient.Send(cefMessage);
      }
    }
  }

### Форматы сериализации Syslog
*  RFC 3164
*  RFC 5424