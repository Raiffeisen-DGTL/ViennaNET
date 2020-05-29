using System;
using NUnit.Framework;
using ViennaNET.CallContext;
using ViennaNET.Messaging.Context;
using ViennaNET.Messaging.Messages;

namespace ViennaNET.Messaging.Tests.Unit.CallContext
{
  [TestFixture(Category = "Unit")]
  [TestOf(typeof(MessagingContext))]
  public class MessagingContextTests
  {
    [Test]
    public void Create_AllPropsExist_Expectedresult()
    {
      // arrange
      var fakeMessage = new TextMessage()
      {
        Properties =
        {
          { MessagingContextHeaders.UserId, "user" },
          { MessagingContextHeaders.UserDomain, "domain" },
          { MessagingContextHeaders.AuthorizeInfo, "auth" },
          { MessagingContextHeaders.RequestCallerIp, "ip" },
          { MessagingContextHeaders.RequestId, "reqId" },
        }
      };

      // act
      var context = MessagingContext.Create(fakeMessage);

      // assert
      Assert.Multiple(() =>
      {
        Assert.That(context.UserId, Is.EqualTo("user"));
        Assert.That(context.UserDomain, Is.EqualTo("domain"));
        Assert.That(context.AuthorizeInfo, Is.EqualTo("auth"));
        Assert.That(context.RequestCallerIp, Is.EqualTo("ip"));
        Assert.That(context.RequestId, Is.EqualTo("reqId"));
      });
    }

    [Test]
    public void Create_RequestIdIsEmpty_Generated()
    {
      // arrange
      var fakeMessage = new TextMessage() { Properties = { { MessagingContextHeaders.RequestId, string.Empty } } };

      // act
      var context = MessagingContext.Create(fakeMessage);

      // assert
      Assert.That(context.RequestId, Is.Not.Empty);
    }

    [Test]
    public void Create_UserIsEmpty_TookFromEnvironment()
    {
      // arrange
      var fakeMessage = new TextMessage() { Properties = { { MessagingContextHeaders.UserId, string.Empty } } };

      // act
      var context = MessagingContext.Create(fakeMessage);

      // assert
      Assert.That(context.UserId, Is.EqualTo(Environment.UserName));
    }
  }
}
