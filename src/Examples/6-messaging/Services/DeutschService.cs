using System.Threading.Tasks;

namespace MessagingService.Services
{
  public class DeutschService : IDeutschService
  {
    public async Task<string> Greet()
    {
      return await Task.FromResult("Guten Tag");
    }

    public async Task<string> Farewell()
    {
      return await Task.FromResult("Tschüß");
    }
  }
}
