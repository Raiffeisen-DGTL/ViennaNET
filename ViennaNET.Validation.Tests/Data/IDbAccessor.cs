namespace ViennaNET.Validation.Tests.Data
{
  public interface IDbAccessor
  {
    string GetContractAccountType(string accountType);

    IAccInfo GetContractAccount(int contractId, string cba);
  }
}
