using System.Threading.Tasks;

namespace EmptyService.Features.Send.Services
{
  public interface ISendingService
  {
    Task Send(string text);
  }
}
