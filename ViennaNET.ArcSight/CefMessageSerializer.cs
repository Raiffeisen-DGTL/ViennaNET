using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using ViennaNET.Utils;
using SyslogNet.Client;
using SyslogNet.Client.Serialization;

namespace ViennaNET.ArcSight
{
  /// <summary>
  /// Сериализатор для преобразования <see cref="CefMessage"/> в <see cref="SyslogMessage"/>/>
  /// </summary>
  public class CefMessageSerializer : ISyslogMessageSerializer
  {
    private static readonly Dictionary<string, string> cefKeys;

    static CefMessageSerializer()
    {
      Extensions exp;
      cefKeys = new Dictionary<string, string>
      {
        { nameof(exp.DeviceAction), "act" },
        { nameof(exp.ApplicationProtocol), "app" },
        { nameof(exp.DeviceCustomIPv6Address1), "c6a1" },
        { nameof(exp.DeviceCustomIPv6Address1Label), "c6a1Label" },
        { nameof(exp.DeviceCustomIPv6Address2), "c6a2" },
        { nameof(exp.DeviceCustomIPv6Address2Label), "c6a2Label" },
        { nameof(exp.DeviceCustomIPv6Address3), "c6a3" },
        { nameof(exp.DeviceCustomIPv6Address3Label), "c6a3Label" },
        { nameof(exp.DeviceCustomIPv6Address4), "c6a4" },
        { nameof(exp.DeviceCustomIPv6Address4Label), "c6a4Label" },
        { nameof(exp.DeviceCustomFloatingPoint1), "cfp1" },
        { nameof(exp.DeviceCustomFloatingPoint1Label), "cfp1Label" },
        { nameof(exp.DeviceCustomFloatingPoint2), "cfp2" },
        { nameof(exp.DeviceCustomFloatingPoint2Label), "cfp2Label" },
        { nameof(exp.DeviceCustomFloatingPoint3), "cfp3" },
        { nameof(exp.DeviceCustomFloatingPoint3Label), "cfp3Label" },
        { nameof(exp.DeviceCustomFloatingPoint4), "cfp4" },
        { nameof(exp.DeviceCustomFloatingPoint4Label), "cfp4Label" },
        { nameof(exp.DeviceCustomNumber1), "cn1" },
        { nameof(exp.DeviceCustomNumber1Label), "cn1Label" },
        { nameof(exp.DeviceCustomNumber2), "cn2" },
        { nameof(exp.DeviceCustomNumber2Label), "cn2Label" },
        { nameof(exp.DeviceCustomNumber3), "cn3" },
        { nameof(exp.DeviceCustomNumber3Label), "cn3Label" },
        { nameof(exp.BaseEventCount), "cnt" },
        { nameof(exp.DeviceCustomString1), "cs1" },
        { nameof(exp.DeviceCustomString1Label), "cs1Label" },
        { nameof(exp.DeviceCustomString2), "cs2" },
        { nameof(exp.DeviceCustomString2Label), "cs2Label" },
        { nameof(exp.DeviceCustomString3), "cs3" },
        { nameof(exp.DeviceCustomString3Label), "cs3Label" },
        { nameof(exp.DeviceCustomString4), "cs4" },
        { nameof(exp.DeviceCustomString4Label), "cs4Label" },
        { nameof(exp.DeviceCustomString5), "cs5" },
        { nameof(exp.DeviceCustomString5Label), "cs5Label" },
        { nameof(exp.DeviceCustomString6), "cs6" },
        { nameof(exp.DeviceCustomString6Label), "cs6Label" },
        { nameof(exp.DestinationDnsDomain), "destinationDnsDomain" },
        { nameof(exp.DestinationServiceName), "destinationServiceName" },
        { nameof(exp.DestinationTranslatedAddress), "destinationTranslatedAddress" },
        { nameof(exp.DestinationTranslatedPort), "destinationTranslatedPort" },
        { nameof(exp.DeviceCustomDate1), "deviceCustomDate1" },
        { nameof(exp.DeviceCustomDate1Label), "deviceCustomDate1Label" },
        { nameof(exp.DeviceCustomDate2), "deviceCustomDate2" },
        { nameof(exp.DeviceCustomDate2Label), "deviceCustomDate2Label" },
        { nameof(exp.DeviceDirection), "deviceDirection" },
        { nameof(exp.DeviceDnsDomain), "deviceDnsDomain" },
        { nameof(exp.DeviceExternalId), "deviceExternalId" },
        { nameof(exp.DeviceFacility), "deviceFacility" },
        { nameof(exp.DeviceInboundInterface), "deviceInboundInterface" },
        { nameof(exp.DeviceNtDomain), "deviceNtDomain" },
        { nameof(exp.DeviceOutboundInterface), "deviceOutboundInterface" },
        { nameof(exp.DevicePayloadId), "devicePayloadId" },
        { nameof(exp.DeviceProcessName), "deviceProcessName" },
        { nameof(exp.DeviceTranslatedAddress), "deviceTranslatedAddress" },
        { nameof(exp.DestinationHostName), "destinationHostName" },
        { nameof(exp.DestinationMacAddress), "dmac" },
        { nameof(exp.DestinationNtDomain), "dntdom" },
        { nameof(exp.DestinationProcessId), "dpid" },
        { nameof(exp.DestinationUserPrivileges), "dpriv" },
        { nameof(exp.DestinationProcessName), "dproc" },
        { nameof(exp.DestinationPort), "dpt" },
        { nameof(exp.DestinationAddress), "dst" },
        { nameof(exp.DestinationUserID), "duid" },
        { nameof(exp.DestinationUserName), "duser" },
        { nameof(exp.DeviceAddress), "dvc" },
        { nameof(exp.DeviceHostName), "dvchost" },
        { nameof(exp.DeviceMacAddress), "dvcmac" },
        { nameof(exp.DeviceProcessId), "dvcpid" },
        { nameof(exp.EndTime), "end" },
        { nameof(exp.ExternalId), "externalId" },
        { nameof(exp.FileCreateTime), "fileCreateTime" },
        { nameof(exp.FileHash), "fileHash" },
        { nameof(exp.FileID), "fileId" },
        { nameof(exp.FileModificationTime), "fileModificationTime" },
        { nameof(exp.FilePath), "filePath" },
        { nameof(exp.FilePermission), "filePermission" },
        { nameof(exp.FileType), "fileType" },
        { nameof(exp.FlexDate1), "flexDate1" },
        { nameof(exp.FlexDate1Label), "flexDate1Label" },
        { nameof(exp.FlexNumber1), "flexNumber1" },
        { nameof(exp.FlexNumber1Label), "flexNumber1Label" },
        { nameof(exp.FlexNumber2), "flexNumber2" },
        { nameof(exp.FlexNumber2Label), "flexNumber2Label" },
        { nameof(exp.FlexString1), "flexString1" },
        { nameof(exp.FlexString1Label), "flexString1Label" },
        { nameof(exp.FlexString2), "flexString2" },
        { nameof(exp.FlexString2Label), "flexString2Label" },
        { nameof(exp.FileName), "fname" },
        { nameof(exp.FileSize), "fsize" },
        { nameof(exp.BytesIn), "in" },
        { nameof(exp.Message), "msg" },
        { nameof(exp.OldFileCreateTime), "oldFileCreateTime" },
        { nameof(exp.OldFileHash), "oldFileHash" },
        { nameof(exp.OldFileId), "oldFileId" },
        { nameof(exp.OldFileModificationTime), "oldFileModificationTime" },
        { nameof(exp.OldFileName), "oldFileName" },
        { nameof(exp.OldFilePath), "oldFilePath" },
        { nameof(exp.OldFilePermission), "oldFilePermission" },
        { nameof(exp.OldFileSize), "oldFileSize" },
        { nameof(exp.OldFileType), "oldFileType" },
        { nameof(exp.BytesOut), "out" },
        { nameof(exp.EventOutcome), "outcome" },
        { nameof(exp.TransportProtocol), "proto" },
        { nameof(exp.Reason), "reason" },
        { nameof(exp.RequestUrl), "request" },
        { nameof(exp.RequestClientApplication), "requestClientApplication" },
        { nameof(exp.RequestContext), "requestContext" },
        { nameof(exp.RequestCookies), "requestCookies" },
        { nameof(exp.RequestMethod), "requestMethod" },
        { nameof(exp.DeviceReceiptTime), "rt" },
        { nameof(exp.SourceHostName), "shost" },
        { nameof(exp.SourceMacAddress), "smac" },
        { nameof(exp.SourceNtDomain), "sntdom" },
        { nameof(exp.SourceDnsDomain), "sourceDnsDomain" },
        { nameof(exp.SourceServiceName), "sourceServiceName" },
        { nameof(exp.SourceTranslatedAddress), "sourceTranslatedAddress" },
        { nameof(exp.SourceTranslatedPort), "sourceTranslatedPort" },
        { nameof(exp.SourceProcessId), "spid" },
        { nameof(exp.SourceUserPrivileges), "spriv" },
        { nameof(exp.SourceProcessName), "sproc" },
        { nameof(exp.SourcePort), "spt" },
        { nameof(exp.SourceAddress), "src" },
        { nameof(exp.StartTime), "start" },
        { nameof(exp.SourceUserId), "suid" },
        { nameof(exp.SourceUserName), "suser" },
        { nameof(exp.Type), "type" }
      };
    }

    private readonly ISyslogMessageSerializer _syslogMessageSerializer;

    public CefMessageSerializer(ISyslogMessageSerializer syslogMessageSerializer)
    {
      _syslogMessageSerializer = syslogMessageSerializer;
    }

    /// <summary>
    /// Сериализует сообщение из <see cref="CefMessage"/> в <see cref="SyslogMessage"/> 
    /// </summary>
    /// <param name="message">Сообщение для сериализации</param>
    /// <returns>Сообщение в формате библиотеки Syslog</returns>
    public SyslogMessage Serialize(CefMessage message)
    {
      var msg = string.Join("|",
                            message.Version,
                            CefEncoder.EncodeHeader(message.DeviceVendor),
                            CefEncoder.EncodeHeader(message.DeviceProduct),
                            CefEncoder.EncodeHeader(message.DeviceVersion),
                            message.DeviceEventClassId,
                            CefEncoder.EncodeHeader(message.Name),
                            (int)message.Severity,
                            Serialize(message.Extensions));

      return new SyslogMessage(
                               message.Extensions.StartTime,
                               Facility.UserLevelMessages,
                               message.Severity.ToSyslogSeverity(),
                               message.HostName,
                               "CEF",
                               msg);
    }

    private static string Serialize(Extensions extensions)
    {
      var result = new StringBuilder();

      result.Append(Serialize(nameof(extensions.DeviceAction), extensions.DeviceAction));
      result.Append(Serialize(nameof(extensions.ApplicationProtocol), extensions.ApplicationProtocol));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address1), extensions.DeviceCustomIPv6Address1));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address1Label), extensions.DeviceCustomIPv6Address1Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address2), extensions.DeviceCustomIPv6Address2));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address2Label), extensions.DeviceCustomIPv6Address2Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address3), extensions.DeviceCustomIPv6Address3));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address3Label), extensions.DeviceCustomIPv6Address3Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address4), extensions.DeviceCustomIPv6Address4));
      result.Append(Serialize(nameof(extensions.DeviceCustomIPv6Address4Label), extensions.DeviceCustomIPv6Address4Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint1), extensions.DeviceCustomFloatingPoint1));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint1Label), extensions.DeviceCustomFloatingPoint1Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint2), extensions.DeviceCustomFloatingPoint2));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint2Label), extensions.DeviceCustomFloatingPoint2Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint3), extensions.DeviceCustomFloatingPoint3));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint3Label), extensions.DeviceCustomFloatingPoint3Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint4), extensions.DeviceCustomFloatingPoint4));
      result.Append(Serialize(nameof(extensions.DeviceCustomFloatingPoint4Label), extensions.DeviceCustomFloatingPoint4Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomNumber1), extensions.DeviceCustomNumber1));
      result.Append(Serialize(nameof(extensions.DeviceCustomNumber1Label), extensions.DeviceCustomNumber1Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomNumber2), extensions.DeviceCustomNumber2));
      result.Append(Serialize(nameof(extensions.DeviceCustomNumber2Label), extensions.DeviceCustomNumber2Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomNumber3), extensions.DeviceCustomNumber3));
      result.Append(Serialize(nameof(extensions.DeviceCustomNumber3Label), extensions.DeviceCustomNumber3Label));
      result.Append(Serialize(nameof(extensions.BaseEventCount), extensions.BaseEventCount));
      result.Append(Serialize(nameof(extensions.DeviceCustomString1), extensions.DeviceCustomString1));
      result.Append(Serialize(nameof(extensions.DeviceCustomString1Label), extensions.DeviceCustomString1Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomString2), extensions.DeviceCustomString2));
      result.Append(Serialize(nameof(extensions.DeviceCustomString2Label), extensions.DeviceCustomString2Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomString3), extensions.DeviceCustomString3));
      result.Append(Serialize(nameof(extensions.DeviceCustomString3Label), extensions.DeviceCustomString3Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomString4), extensions.DeviceCustomString4));
      result.Append(Serialize(nameof(extensions.DeviceCustomString4Label), extensions.DeviceCustomString4Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomString5), extensions.DeviceCustomString5));
      result.Append(Serialize(nameof(extensions.DeviceCustomString5Label), extensions.DeviceCustomString5Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomString6), extensions.DeviceCustomString6));
      result.Append(Serialize(nameof(extensions.DeviceCustomString6Label), extensions.DeviceCustomString6Label));
      result.Append(Serialize(nameof(extensions.DestinationDnsDomain), extensions.DestinationDnsDomain));
      result.Append(Serialize(nameof(extensions.DestinationServiceName), extensions.DestinationServiceName));
      result.Append(Serialize(nameof(extensions.DestinationTranslatedAddress), extensions.DestinationTranslatedAddress));
      result.Append(Serialize(nameof(extensions.DestinationTranslatedPort), extensions.DestinationTranslatedPort));
      result.Append(Serialize(nameof(extensions.DeviceCustomDate1), extensions.DeviceCustomDate1));
      result.Append(Serialize(nameof(extensions.DeviceCustomDate1Label), extensions.DeviceCustomDate1Label));
      result.Append(Serialize(nameof(extensions.DeviceCustomDate2), extensions.DeviceCustomDate2));
      result.Append(Serialize(nameof(extensions.DeviceCustomDate2Label), extensions.DeviceCustomDate2Label));
      result.Append(Serialize(nameof(extensions.DeviceDirection), extensions.DeviceDirection));
      result.Append(Serialize(nameof(extensions.DeviceDnsDomain), extensions.DeviceDnsDomain));
      result.Append(Serialize(nameof(extensions.DeviceExternalId), extensions.DeviceExternalId));
      result.Append(Serialize(nameof(extensions.DeviceFacility), extensions.DeviceFacility));
      result.Append(Serialize(nameof(extensions.DeviceInboundInterface), extensions.DeviceInboundInterface));
      result.Append(Serialize(nameof(extensions.DeviceNtDomain), extensions.DeviceNtDomain));
      result.Append(Serialize(nameof(extensions.DeviceOutboundInterface), extensions.DeviceOutboundInterface));
      result.Append(Serialize(nameof(extensions.DevicePayloadId), extensions.DevicePayloadId));
      result.Append(Serialize(nameof(extensions.DeviceProcessName), extensions.DeviceProcessName));
      result.Append(Serialize(nameof(extensions.DestinationTranslatedAddress), extensions.DestinationTranslatedAddress));
      result.Append(Serialize(nameof(extensions.DestinationHostName), extensions.DestinationHostName));
      result.Append(Serialize(nameof(extensions.DestinationMacAddress), extensions.DestinationMacAddress));
      result.Append(Serialize(nameof(extensions.DestinationNtDomain), extensions.DestinationNtDomain));
      result.Append(Serialize(nameof(extensions.DestinationProcessId), extensions.DestinationProcessId));
      result.Append(Serialize(nameof(extensions.DestinationUserPrivileges), extensions.DestinationUserPrivileges));
      result.Append(Serialize(nameof(extensions.DestinationProcessName), extensions.DestinationProcessName));
      result.Append(Serialize(nameof(extensions.DestinationPort), extensions.DestinationPort));
      result.Append(Serialize(nameof(extensions.DestinationAddress), extensions.DestinationAddress));
      result.Append(Serialize(nameof(extensions.DestinationUserID), extensions.DestinationUserID));
      result.Append(Serialize(nameof(extensions.DestinationUserName), extensions.DestinationUserName));
      result.Append(Serialize(nameof(extensions.DeviceAddress), extensions.DeviceAddress));
      result.Append(Serialize(nameof(extensions.DeviceHostName), extensions.DeviceHostName));
      result.Append(Serialize(nameof(extensions.DeviceMacAddress), extensions.DeviceMacAddress));
      result.Append(Serialize(nameof(extensions.DeviceProcessId), extensions.DeviceProcessId));
      result.Append(Serialize(nameof(extensions.EndTime), extensions.EndTime));
      result.Append(Serialize(nameof(extensions.ExternalId), extensions.ExternalId));
      result.Append(Serialize(nameof(extensions.FileCreateTime), extensions.FileCreateTime));
      result.Append(Serialize(nameof(extensions.FileHash), extensions.FileHash));
      result.Append(Serialize(nameof(extensions.FileID), extensions.FileID));
      result.Append(Serialize(nameof(extensions.FileModificationTime), extensions.FileModificationTime));
      result.Append(Serialize(nameof(extensions.FilePath), extensions.FilePath));
      result.Append(Serialize(nameof(extensions.FilePermission), extensions.FilePermission));
      result.Append(Serialize(nameof(extensions.FileType), extensions.FileType));
      result.Append(Serialize(nameof(extensions.FlexDate1), extensions.FlexDate1));
      result.Append(Serialize(nameof(extensions.FlexDate1Label), extensions.FlexDate1Label));
      result.Append(Serialize(nameof(extensions.FlexNumber1), extensions.FlexNumber1));
      result.Append(Serialize(nameof(extensions.FlexNumber1Label), extensions.FlexNumber1Label));
      result.Append(Serialize(nameof(extensions.FlexNumber2), extensions.FlexNumber2));
      result.Append(Serialize(nameof(extensions.FlexNumber2Label), extensions.FlexNumber2Label));
      result.Append(Serialize(nameof(extensions.FlexString1), extensions.FlexString1));
      result.Append(Serialize(nameof(extensions.FlexString1Label), extensions.FlexString1Label));
      result.Append(Serialize(nameof(extensions.FlexString2), extensions.FlexString2));
      result.Append(Serialize(nameof(extensions.FlexString2Label), extensions.FlexString2Label));
      result.Append(Serialize(nameof(extensions.FileName), extensions.FileName));
      result.Append(Serialize(nameof(extensions.FileSize), extensions.FileSize));
      result.Append(Serialize(nameof(extensions.BytesIn), extensions.BytesIn));
      result.Append(Serialize(nameof(extensions.Message), extensions.Message));
      result.Append(Serialize(nameof(extensions.OldFileCreateTime), extensions.OldFileCreateTime));
      result.Append(Serialize(nameof(extensions.OldFileHash), extensions.OldFileHash));
      result.Append(Serialize(nameof(extensions.OldFileId), extensions.OldFileId));
      result.Append(Serialize(nameof(extensions.OldFileModificationTime), extensions.OldFileModificationTime));
      result.Append(Serialize(nameof(extensions.OldFileName), extensions.OldFileName));
      result.Append(Serialize(nameof(extensions.OldFilePath), extensions.OldFilePath));
      result.Append(Serialize(nameof(extensions.OldFilePermission), extensions.OldFilePermission));
      result.Append(Serialize(nameof(extensions.OldFileSize), extensions.OldFileSize));
      result.Append(Serialize(nameof(extensions.OldFileType), extensions.OldFileType));
      result.Append(Serialize(nameof(extensions.BytesOut), extensions.BytesOut));
      result.Append(Serialize(nameof(extensions.EventOutcome), extensions.EventOutcome));
      result.Append(Serialize(nameof(extensions.TransportProtocol), extensions.TransportProtocol));
      result.Append(Serialize(nameof(extensions.Reason), extensions.Reason));
      result.Append(Serialize(nameof(extensions.RequestUrl), extensions.RequestUrl));
      result.Append(Serialize(nameof(extensions.RequestClientApplication), extensions.RequestClientApplication));
      result.Append(Serialize(nameof(extensions.RequestContext), extensions.RequestContext));
      result.Append(Serialize(nameof(extensions.RequestCookies), extensions.RequestCookies));
      result.Append(Serialize(nameof(extensions.RequestMethod), extensions.RequestMethod));
      result.Append(Serialize(nameof(extensions.DeviceReceiptTime), extensions.DeviceReceiptTime));
      result.Append(Serialize(nameof(extensions.SourceHostName), extensions.SourceHostName));
      result.Append(Serialize(nameof(extensions.SourceMacAddress), extensions.SourceMacAddress));
      result.Append(Serialize(nameof(extensions.SourceNtDomain), extensions.SourceNtDomain));
      result.Append(Serialize(nameof(extensions.SourceDnsDomain), extensions.SourceDnsDomain));
      result.Append(Serialize(nameof(extensions.SourceServiceName), extensions.SourceServiceName));
      result.Append(Serialize(nameof(extensions.SourceTranslatedAddress), extensions.SourceTranslatedAddress));
      result.Append(Serialize(nameof(extensions.SourceTranslatedPort), extensions.SourceTranslatedPort));
      result.Append(Serialize(nameof(extensions.SourceProcessId), extensions.SourceProcessId));
      result.Append(Serialize(nameof(extensions.SourceUserPrivileges), extensions.SourceUserPrivileges));
      result.Append(Serialize(nameof(extensions.SourceProcessName), extensions.SourceProcessName));
      result.Append(Serialize(nameof(extensions.SourcePort), extensions.SourcePort));
      result.Append(Serialize(nameof(extensions.SourceAddress), extensions.SourceAddress));
      result.Append(Serialize(nameof(extensions.StartTime), extensions.StartTime));
      result.Append(Serialize(nameof(extensions.SourceUserId), extensions.SourceUserId));
      result.Append(Serialize(nameof(extensions.SourceUserName), extensions.SourceUserName));
      result.Append(Serialize(nameof(extensions.Type), extensions.Type));

      return result.ToString()
                   .Trim();
    }

    [CanBeNull]
    private static string Serialize<T>(string fullName, T value)
    {
      if (value == null || !cefKeys.ContainsKey(fullName))
      {
        return null;
      }

      string valueAsString;

      if (value is Enum)
      {
        valueAsString = Convert.ToInt32(value)
                               .ToString();
      }
      else if (value is DateTimeOffset)
      {
        valueAsString = string.Format(CultureInfo.InvariantCulture, "{0:MMM dd yyyy HH:mm:ss}", value);
      }
      else
      {
        valueAsString = CefEncoder.EncodeExtension(value.ToString());
      }

      return $"{cefKeys[fullName]}={valueAsString} ";
    }

    /// <summary>
    /// Сериализует сообщение в формате Syslog в поток
    /// </summary>
    /// <param name="message">Сообщение в формате Syslog</param>
    /// <param name="stream">Поток для помещения сериализованного сообщения</param>
    public void Serialize(SyslogMessage message, Stream stream)
    {
      _syslogMessageSerializer.Serialize(message, stream);
    }
  }
}
