using ViennaNET.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ViennaNET.HttpClient.Tests
{
  [TestFixture(Category = "Unit", TestOf = typeof(HttpResponseMessageExtensions))]
  public class HttpResponseMessageExtensionsTests
  {
    private class SuccessDto
    {
      public string TestField { get; set; }
    }

    private class ErrorDto
    {
      public string Message { get; set; }
    }

    private static HttpResponseMessage CreateSuccessResponse()
    {
      const string jsonContentType = "application/json";

      var dto = new SuccessDto
      {
        TestField = "Test!"
      };
      var stringContent = JsonConvert.SerializeObject(dto);

      return new HttpResponseMessage
      {
        Content = new StringContent(stringContent, Encoding.UTF8, jsonContentType),
        StatusCode = HttpStatusCode.OK
      };
    }

    private static HttpResponseMessage CreateCorrectBadRequestResponse()
    {
      const string jsonContentType = "application/json";

      var dto = new ErrorDto
      {
        Message = "Message!"
      };
      var stringContent = JsonConvert.SerializeObject(dto);

      return new HttpResponseMessage
      {
        Content = new StringContent(stringContent, Encoding.UTF8, jsonContentType),
        StatusCode = HttpStatusCode.BadRequest
      };
    }

    private static HttpResponseMessage CreateIncorrectBadRequestResponse()
    {
      const string jsonContentType = "application/json";

      return new HttpResponseMessage
      {
        Content = new StringContent("Message!", Encoding.UTF8, jsonContentType),
        StatusCode = HttpStatusCode.BadRequest
      };
    }

    private static HttpResponseMessage CreateNotFoundResponse()
    {
      return new HttpResponseMessage
      {
        StatusCode = HttpStatusCode.NotFound
      };
    }

    [Test]
    public async Task HandleAsyncIsSuccessTest()
    {
      var response = CreateSuccessResponse();

      var actual = await response.HandleAsync<SuccessDto>();

      Assert.AreEqual(ResultState.Success, actual.State);
      Assert.AreEqual("Test!", actual.Result.TestField);
    }

    [Test]
    public async Task HandleAsyncIsCorrectBadRequestTest()
    {
      var response = CreateCorrectBadRequestResponse();

      var actual = await response.HandleAsync<SuccessDto>();

      Assert.AreEqual(ResultState.Invalid, actual.State);
      Assert.AreEqual("Message!", actual.InvalidMessage);
    }

    [Test]
    public async Task HandleAsyncIsIncorrectBadRequestTest()
    {
      var response = CreateIncorrectBadRequestResponse();

      var actual = await response.HandleAsync<SuccessDto>();

      Assert.AreEqual(ResultState.Invalid, actual.State);
      Assert.AreEqual("Message!", actual.InvalidMessage);
    }

    [Test]
    public async Task HandleAsyncIsNotFoundTest()
    {
      var response = CreateNotFoundResponse();

      var actual = await response.HandleAsync<SuccessDto>();

      Assert.AreEqual(ResultState.Empty, actual.State);
    }
  }
}
