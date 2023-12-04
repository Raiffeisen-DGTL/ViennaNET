namespace ViennaNET.Validation.Rules.ValidationResults.RuleMessages
{
  /// <summary>
  ///   ��������� �� ������
  /// </summary>
  public class ErrorRuleMessage : BaseRuleMessage
  {
    /// <summary>
    ///   �������������� ��������� �������� �� ���������
    /// </summary>
    /// <param name="identity">������������� ���������</param>
    /// <param name="error">����� ������</param>
    /// <param name="args">��������� ��� �������������� ������</param>
    public ErrorRuleMessage(MessageIdentity identity, string error, params object[] args) : base(null, identity, null,
      error, args)
    {
    }

    /// <summary>
    ///   �������������� ��������� �������� �� ���������
    /// </summary>
    /// <param name="state">��������� ��� ������������ ������ ��������� �� ������ ������� ������</param>
    /// <param name="identity">������������� ���������</param>
    /// <param name="error">����� ������</param>
    /// <param name="args">��������� ��� �������������� ������</param>
    public ErrorRuleMessage(object state, MessageIdentity identity, string error, params object[] args) :
      base(state, identity, null, error, args)
    {
    }

    /// <summary>
    ///   �������������� ��������� �������� �� ���������
    /// </summary>
    /// <param name="customCode">�������������� ��� ���������</param>
    /// <param name="identity">������������� ���������</param>
    /// <param name="error">����� ������</param>
    /// <param name="args">��������� ��� �������������� ������</param>
    public ErrorRuleMessage(string customCode, MessageIdentity identity, string error, params object[] args) :
      base(null, identity, customCode, error, args)
    {
    }

    /// <summary>
    ///   �������������� ��������� �������� �� ���������
    /// </summary>
    /// <param name="state">��������� ��� ������������ ������ ��������� �� ������ ������� ������</param>
    /// <param name="customCode">�������������� ��� ���������</param>
    /// <param name="identity">������������� ���������</param>
    /// <param name="error">����� ������</param>
    /// <param name="args">��������� ��� �������������� ������</param>
    public ErrorRuleMessage(object state, string customCode, MessageIdentity identity, string error,
      params object[] args) :
      base(state, identity, customCode, error, args)
    {
    }

    /// <inheritdoc />
    public override bool IsValid => false;
  }
}