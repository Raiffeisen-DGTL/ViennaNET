namespace ViennaNET.Word.Barcode
{
  /// <summary>
  ///   Определяет количество модулей в знаке символа Code128.
  /// </summary>
  internal struct ModulesCount
  {
    /// <summary>
    ///   Количество модулей в штрихе.
    /// </summary>
    public int Bar { get; set; }

    /// <summary>
    ///   Количество модулей в пробеле.
    /// </summary>
    public int Space { get; set; }

    /// <summary>
    ///   Суммарное количество модулей пары: штрих - пробел.
    /// </summary>
    public int Total => Bar + Space;
  }
}