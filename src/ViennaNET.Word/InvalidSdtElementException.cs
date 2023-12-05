using System;
using DocumentFormat.OpenXml.Wordprocessing;

namespace ViennaNET.Word
{
  /// <summary>
  ///   Исключени которое возникает если дерево дочерних элементов <see cref="SdtElement" /> содержит недопустимые элементы
  ///   или отсутствует обязательный дочерний элемент.
  /// </summary>
  public class InvalidSdtElementException : Exception
  {
    /// <summary>
    ///   Создаёт и инициализирует новый экземпляр класса <see cref="InvalidSdtElementException" />.
    /// </summary>
    /// <param name="name">Значение <see cref="SdtAlias" /> представляющее имя искомого элемента.</param>
    public InvalidSdtElementException(string name)
    {
      Message = $"{nameof(SdtElement)} с {nameof(SdtAlias)}.{nameof(SdtAlias.Val)} = {name} is not valid.";
    }

    /// <summary>
    ///   Получает сообщение, которое описывает текущее исключение.
    /// </summary>
    public override string Message { get; }
  }
}