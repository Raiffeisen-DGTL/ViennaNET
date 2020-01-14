using System.Threading.Tasks;

namespace Company.Security
{
  /// <summary>
  /// Фабрика по получению авторизационных данных пользователя из JWT
  /// </summary>
  public interface ISecurityContextFactory
  {
    ISecurityContext Create();
    Task<ISecurityContext> CreateAsync();
  }
}
