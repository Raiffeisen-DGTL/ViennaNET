using System;

namespace ViennaNET.Redis
{
  /// <summary>
  ///   Описывает возможности, которые предоставлены для работы с сервисом кэширования
  /// </summary>
  public interface ICacheService
  {
    /// <summary>
    ///   Возвращет объект в случае существования ключа
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="name">Префикс имени ключа</param>
    /// <param name="obj">Полученный объект</param>
    /// <param name="keyIdentifier">Набор параметров, определяющих уникальный ключ</param>
    /// <exception cref="ArgumentException">Если отсутствует набор параметров</exception>
    /// <returns>True в случае успеха, иначе False.</returns>
    bool TryGetObject<T>(string name, out T obj, params object[] keyIdentifier) where T : class;

    /// <summary>
    ///   Сохраняет объект
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="name">Префикс имени ключа</param>
    /// <param name="lifetime">Ключ словаря времени жизни из конфигурации</param>
    /// <param name="obj">Объект для сохранения</param>
    /// <param name="keyIdentifier">Набор параметров, определяющих уникальный ключ</param>
    /// <exception cref="ArgumentException">Если отсутствует набор параметров</exception>
    void SetObject<T>(string name, string lifetime, T obj, params object[] keyIdentifier) where T : class;

    /// <summary>
    ///   Сохраняет объект
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    /// <param name="name">Префикс имени ключа</param>
    /// <param name="expiry">Время жизни ключа</param>
    /// <param name="obj">Объект для сохранения</param>
    /// <param name="keyIdentifier">Набор параметров, определяющих уникальный ключ</param>
    /// <exception cref="ArgumentException">Если отсутствует набор параметров</exception>
    void SetObject<T>(string name, TimeSpan? expiry, T obj, params object[] keyIdentifier) where T : class;
  }
}