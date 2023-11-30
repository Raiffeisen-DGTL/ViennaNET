namespace ViennaNET.Extensions.Http.Authentication;

/// <summary>
///     Представляет параметры аутентификации по схеме Negotiate.
///     С использованием протокола Kerberos или NTLM (Протокол выбирается механизмом SPNEGO).
/// </summary>
public class NegotiateOptions
{
    /// <summary>
    ///     Определяет, слудет ли использовать учётные данные текущего пользователя, при выполненни запроса.
    /// </summary>
    /// <remarks>Если указано, <see cref="UserName" /> и <see cref="Password" /> игнорируются.</remarks>
    public bool UseDefaultCredentials { get; set; } = true;

    /// <summary>
    ///     Имя пользователя, от лица которого необходимо выполнить запрос.
    /// </summary>
    /// <remarks>Чтобы применить, укажите <see cref="UseDefaultCredentials" /> = <see langword="false" />.</remarks>
    public string? UserName { get; set; }

    /// <summary>
    ///     Пароль от учётной записи <see cref="UserName" />.
    /// </summary>
    /// <remarks>Чтобы применить, укажите <see cref="UseDefaultCredentials" /> = <see langword="false" />.</remarks>
    public string? Password { get; set; }
}