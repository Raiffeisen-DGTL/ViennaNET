using System.Collections.Generic;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;
using SyslogNet.Client.Transport;

namespace ViennaNET.ArcSight
{
  internal class CefSender : ISyslogMessageSender, ICefSender
  {
    private readonly ISyslogMessageSender _syslogMessageSender;

    public CefSender(ISyslogMessageSender syslogMessageSender)
    {
      _syslogMessageSender = syslogMessageSender;
    }

    public void Send(CefMessage message, CefMessageSerializer serializer)
    {
      Send(serializer.Serialize(message), serializer);
    }

    public void Send(IEnumerable<CefMessage> messages, CefMessageSerializer serializer)
    {
      foreach (var message in messages)
      {
        Send(message, serializer);
      }
    }

    public void Dispose()
    {
      _syslogMessageSender.Dispose();
    }

    public void Reconnect()
    {
      _syslogMessageSender.Reconnect();
    }

    public void Send(SyslogMessage message, ISyslogMessageSerializer serializer)
    {
      _syslogMessageSender.Send(message, serializer);
    }

    public void Send(IEnumerable<SyslogMessage> messages, ISyslogMessageSerializer serializer)
    {
      _syslogMessageSender.Send(messages, serializer);
    }
  }
}