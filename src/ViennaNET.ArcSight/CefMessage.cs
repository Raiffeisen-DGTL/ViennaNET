using System;
using ViennaNET.ArcSight.Exceptions;
using ViennaNET.ArcSight.Validation;
using ViennaNET.Validation.Rules.ValidationResults;
using ViennaNET.Validation.Validators;

namespace ViennaNET.ArcSight
{
  /// <summary>
  ///   Сообщение в ArcSight в стандартном CEF-формате
  /// </summary>
  public class CefMessage
  {
    public CefMessage(
      DateTimeOffset startTime,
      string hostName,
      string deviceVendor,
      string deviceProduct,
      string deviceVersion,
      int deviceEventClassId,
      string name,
      CefSeverity severity)
    {
      Extensions = new Extensions { StartTime = startTime, SourceHostName = hostName };
      DeviceVendor = deviceVendor;
      DeviceProduct = deviceProduct;
      DeviceVersion = deviceVersion;
      DeviceEventClassId = deviceEventClassId;
      Name = name;
      Severity = severity;
      HostName = hostName;

      var result = RulesValidator.Validate(new CefMessageRule(), this);
      if (!result.IsValid)
      {
        throw new CefMessageValidationException(result.Results.ToErrorsString());
      }
    }

    /// <summary>
    ///   Комментарий из документации:
    ///   Version is an integer and identifies the version of the CEF format. Event consumers use this
    ///   information to determine what the following fields represent.The current CEF version is 0 (CEF:0).
    /// </summary>
    public int Version => 0;

    public string DeviceVendor { get; }

    public string DeviceProduct { get; }

    public string DeviceVersion { get; }

    /// <summary>
    ///   Комментарий из документации:
    ///   Device Event Class ID is a unique identifier per event-type. This can be a string or an integer. Device
    ///   Event Class ID identifies the type of event reported.In the intrusion detection system(IDS) world, each
    ///   signature or rule that detects certain activity has a unique Device Event Class ID assigned.This is a
    ///   requirement for other types of devices as well, and helps correlation engines process the events.Also
    ///   known as Signature ID.
    /// </summary>
    public int DeviceEventClassId { get; }

    /// <summary>
    ///   Комментарий из документации:
    ///   Name is a string representing a human-readable and understandable description of the event. The
    ///   event name should not contain information that is specifically mentioned in other fields.For example:
    ///   "Port scan from 10.0.0.1 targeting 20.1.1.1" is not a good event name.It should be: "Port scan". The other
    ///   information is redundant and can be picked up from the other fields.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///   Комментарий из документации:
    ///   Severity is a string or integer and reflects the importance of the event. The valid string values are
    ///   Unknown, Low, Medium, High, and Very-High.The valid integer values are 0-3=Low, 4-6=Medium, 7-8=High, and
    ///   9-10=Very-High.
    /// </summary>
    public CefSeverity Severity { get; }

    public Extensions Extensions { get; }

    public string HostName { get; }
  }
}