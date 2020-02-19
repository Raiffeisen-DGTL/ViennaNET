using System.Collections.Generic;
using ViennaNET.Validation.Rules;
using ViennaNET.Validation.ValidationChains;

namespace ViennaNET.Validation.Validators
{
  /// <summary>
  /// ���������, ����������� ��������� ������ ������
  /// </summary>
  /// <typeparam name="T">��� ������� ��� ���������</typeparam>
  public interface IValidationRuleSet<T>
  {
    /// <summary>
    /// ���������� ��������� ���� �� ������ ������� ���������
    /// </summary>
    /// <param name="rule">������� ���������</param>
    /// <returns>�������� ���� ���������</returns>
    IValidationChainMember<T> AddMemberToChain(IRuleBase<T> rule);

    /// <summary>
    /// ��������� ���� ���������
    /// </summary>
    /// <returns>���� ���������</returns>
    IList<IValidationChainMember<T>> GetValidationChain();
  }
}
