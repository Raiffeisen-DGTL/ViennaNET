using System.Threading.Tasks;

namespace SagaService.Services
{
  public interface IDeutschService
  {
    Task<string> Greet();

    Task<string> Farewell();
  }
}
