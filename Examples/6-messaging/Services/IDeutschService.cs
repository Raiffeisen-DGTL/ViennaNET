using System.Threading.Tasks;

namespace MessagingService.Services
{
  public interface IDeutschService
  {
    Task<string> Greet();

    Task<string> Farewell();
  }
}
