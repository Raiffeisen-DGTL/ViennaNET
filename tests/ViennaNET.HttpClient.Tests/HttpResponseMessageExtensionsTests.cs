using System.Net;
using System.Text;
using Newtonsoft.Json;
using NUnit.Framework;
using ViennaNET.Utils;

namespace ViennaNET.HttpClient.Tests
{
    [TestFixture(Category = "Unit", TestOf = typeof(HttpResponseMessageExtensions))]
    public class HttpResponseMessageExtensionsTests
    {
        private class SuccessDto
        {
            public string TestField { get; set; } = null!;
        }

        private class ErrorDto
        {
            public string Message { get; set; } = null!;
        }

        private static HttpResponseMessage CreateSuccessResponse()
        {
            const string jsonContentType = "application/json";

            var dto = new SuccessDto { TestField = "Test!" };
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

            var dto = new ErrorDto { Message = "Message!" };
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
            return new HttpResponseMessage { StatusCode = HttpStatusCode.NotFound };
        }

        [Test]
        public async Task HandleAsyncIsSuccessTest()
        {
            const string field = "Test!";
            var response = CreateSuccessResponse();
            var actual = await response.HandleAsync<SuccessDto>();

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Success));
                Assert.That(actual.State, Is.EqualTo(ResultState.Success));
                Assert.That(actual.Result?.TestField, Is.EqualTo(field));
            });
        }

        [Test]
        public async Task HandleAsyncIsCorrectBadRequestTest()
        {
            const string message = "Message!";
            
            var response = CreateCorrectBadRequestResponse();
            var actual = await response.HandleAsync<SuccessDto>();

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Invalid));
                Assert.That(actual.InvalidMessage, Is.EqualTo(message));
            });
        }

        [Test]
        public async Task HandleAsyncIsIncorrectBadRequestTest()
        {
            const string message = "Message!";
            
            var response = CreateIncorrectBadRequestResponse();
            var actual = await response.HandleAsync<SuccessDto>();

            Assert.Multiple(() =>
            {
                Assert.That(actual.State, Is.EqualTo(ResultState.Invalid));
                Assert.That(actual.InvalidMessage, Is.EqualTo(message));
            });
        }

        [Test]
        public async Task HandleAsyncIsNotFoundTest()
        {
            var response = CreateNotFoundResponse();

            var actual = await response.HandleAsync<SuccessDto>();

            Assert.That(actual.State, Is.EqualTo(ResultState.Empty));
        }
    }
}