# Сборка, содержащая реализацию посредника для выполнения операций сервисов

### Основные сущности

Основной класс - **Mediator**. Представляет собой посредник, позволяющий связывать различные виды сообщений и их обработчики.

**EventCollector** позволяет обеспечить отложенную отправку через посредник. 

#### Инструкция по применению для обработки запросов и комманд:

1.  Добавляем в класс зависимость от **IMediator**.
2.  Создаем либо регистрируем в DI класс **Mediator**, параметризуя его коллекциями синхронных и асинхронных обработчиков сообщений. Список обработчиков должен соответствовать ожидаемым в сервисе типам сообщений. 
3.  В методах сервисов вызываем методы интерфейса SendMessage либо SendMessageAsync. Посредник автоматически направит сообщение соответствующему обработчику. 

### Пример использования

  public IHttpActionResult Get()
  {
    IEnumerable<ProductModel> products = _mediator.SendMessage<GetProductsRequest, IEnumerable<ProductModel>>(new GetProductsRequest());

    if (products is null || !products.Any())
    {
      return NotFound();
    }
    else
    {
      return Ok(products);
    }
  }

#### Инструкция по применению для обработки доменных событий:

1. Реализовать в сущности интерфейс **IEntityEventPublisher**. Стандартная реализация сохраняет экземпляр **IEventCollector** во внутреннюю переменную.

  public class Payroll : IEntityKey<int>, IEntityEventPublisher
  {
    private IEventCollector _eventCollector;

    public virtual void SetCollector(IEventCollector eventCollector);
    {
      _eventCollector = eventCollector;
    }
  }

2. При необходимости опубликовать событие с использованием **IEventCollector**. 

  _eventCollector.Enqueue(new PayrollReachesFinalStatusEvent(this));

3. В аппликационном сервисе, управляющем доменными сущностями, реализовать единицу работы для отправки доменных событий.

   using (var collector = _eventCollectorFactory.Create())
   {
     ...
	 collector.Publish();
   }

Для корректной обработки событий Handlers должны быть зарегистрированы в классе **Mediator**. 