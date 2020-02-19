using System.Collections.Generic;

namespace ViennaNET.Validation.Tests.Data
{
  public interface IMainEntity
  {
    IAccInfo AccountsInfo { get; }
    string ActionType { get; }
    string DocType { get; }
    int ContractId { get; }
    IEnumerable<ICollectionEntity> Salaries { get; }
  }
}
