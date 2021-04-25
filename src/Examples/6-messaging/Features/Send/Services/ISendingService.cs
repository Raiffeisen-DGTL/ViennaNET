using System.Threading.Tasks;

namespace MessagingService.Features.Send.Services
{
  public interface ISendingService
  {
    Task Send(string text);
  }
}
