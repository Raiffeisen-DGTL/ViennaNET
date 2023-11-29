using System.Globalization;
using System.Text;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;

namespace ViennaNET.ArcSight
{
  //TODO: remove when https://github.com/emertechie/SyslogNet/pull/17 will be merged
  internal class SyslogRfc3164MessageSerializer : SyslogMessageSerializerBase, ISyslogMessageSerializer
  {
    public void Serialize(SyslogMessage message, Stream stream)
    {
      var priorityValue = CalculatePriorityValue(message.Facility, message.Severity);

      string timestamp = null;
      if (message.DateTimeOffset.HasValue)
      {
        var dt = message.DateTimeOffset.Value;
        var day = dt.Day.ToString();
        if (day.Length == 1)
        {
            day = string.Concat(' ', day);
        }

        timestamp = string.Concat(dt.ToString("MMM ", CultureInfo.InvariantCulture), day, dt.ToString(" HH:mm:ss"));
      }

      var headerBuilder = new StringBuilder();
      headerBuilder.Append("<")
                   .Append(priorityValue)
                   .Append(">");
      headerBuilder.Append(timestamp)
                   .Append(" ");
      headerBuilder.Append(message.HostName)
                   .Append(" ");
      headerBuilder.Append(message.AppName.IfNotNullOrWhitespace(x => x.EnsureMaxLength(32) + ":"));
      headerBuilder.Append(message.Message ?? "");

      var asciiBytes = Encoding.UTF8.GetBytes(headerBuilder.ToString());
      stream.Write(asciiBytes, 0, asciiBytes.Length);
    }
  }
}
