using ViennaNET.Validation.Rules.FluentRule;

namespace ViennaNET.ArcSight.Validation
{
  internal class ExtensionsRule : BaseFluentRule<Extensions>
  {
    public ExtensionsRule()
    {
      ForProperty(e => e.DeviceAction)
        .Length(0, 63);
      ForProperty(e => e.ApplicationProtocol)
        .Length(0, 31);
      ForProperty(e => e.DeviceCustomIPv6Address1Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomIPv6Address2Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomIPv6Address3Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomIPv6Address4Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomFloatingPoint1Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomFloatingPoint2Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomFloatingPoint3Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomFloatingPoint4Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomNumber1Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomNumber2Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomNumber3Label)
        .Length(0, 1023);
      ForProperty(e => e.BaseEventCount)
        .GreaterThan(0);
      ForProperty(e => e.DeviceCustomString1)
        .Length(0, 4000);
      ForProperty(e => e.DeviceCustomString1Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomString2)
        .Length(0, 4000);
      ForProperty(e => e.DeviceCustomString2Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomString3)
        .Length(0, 4000);
      ForProperty(e => e.DeviceCustomString3Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomString4)
        .Length(0, 4000);
      ForProperty(e => e.DeviceCustomString4Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomString5)
        .Length(0, 4000);
      ForProperty(e => e.DeviceCustomString5Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomString6)
        .Length(0, 4000);
      ForProperty(e => e.DeviceCustomString6Label)
        .Length(0, 1023);
      ForProperty(e => e.DestinationDnsDomain)
        .Length(0, 255);
      ForProperty(e => e.DestinationServiceName)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomDate1Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceCustomDate2Label)
        .Length(0, 1023);
      ForProperty(e => e.DeviceDnsDomain)
        .Length(0, 255);
      ForProperty(e => e.DeviceExternalId)
        .Length(0, 255);
      ForProperty(e => e.DeviceFacility)
        .Length(0, 1023);
      ForProperty(e => e.DeviceInboundInterface)
        .Length(0, 128);
      ForProperty(e => e.DeviceNtDomain)
        .Length(0, 255);
      ForProperty(e => e.DeviceOutboundInterface)
        .Length(0, 128);
      ForProperty(e => e.DevicePayloadId)
        .Length(0, 128);
      ForProperty(e => e.DeviceProcessName)
        .Length(0, 1023);
      ForProperty(e => e.DestinationHostName)
        .Length(0, 1023);
      ForProperty(e => e.DestinationNtDomain)
        .Length(0, 255);
      ForProperty(e => e.DestinationProcessId)
        .GreaterThan(0);
      ForProperty(e => e.DestinationUserPrivileges)
        .Length(0, 1023);
      ForProperty(e => e.DestinationProcessName)
        .Length(0, 1023);
      ForProperty(e => e.DeviceTimeZone)
        .Length(0, 255);
      ForProperty(e => e.DestinationUserID)
        .Length(0, 1023);
      ForProperty(e => e.DestinationUserName)
        .Length(0, 1023);
      ForProperty(e => e.DeviceHostName)
        .Length(0, 100);
      ForProperty(e => e.DeviceProcessId)
        .GreaterThan(0);
      ForProperty(e => e.ExternalId)
        .Length(0, 40);
      ForProperty(e => e.FileHash)
        .Length(0, 255);
      ForProperty(e => e.FileID)
        .Length(0, 1023);
      ForProperty(e => e.FilePath)
        .Length(0, 1023);
      ForProperty(e => e.FilePermission)
        .Length(0, 1023);
      ForProperty(e => e.FileType)
        .Length(0, 1023);
      ForProperty(e => e.FlexDate1Label)
        .Length(0, 128);
      ForProperty(e => e.FlexNumber1Label)
        .Length(0, 128);
      ForProperty(e => e.FlexNumber2Label)
        .Length(0, 128);
      ForProperty(e => e.FlexString1)
        .Length(0, 1023);
      ForProperty(e => e.FlexString1Label)
        .Length(0, 128);
      ForProperty(e => e.FlexString2)
        .Length(0, 1023);
      ForProperty(e => e.FlexString2Label)
        .Length(0, 128);
      ForProperty(e => e.FileName)
        .Length(0, 1023);
      ForProperty(e => e.FileSize)
        .GreaterThan(0);
      ForProperty(e => e.BytesIn)
        .GreaterThan(0);
      ForProperty(e => e.Message)
        .Length(0, 1023);
      ForProperty(e => e.OldFileHash)
        .Length(0, 255);
      ForProperty(e => e.OldFileId)
        .Length(0, 1023);
      ForProperty(e => e.OldFileName)
        .Length(0, 1023);
      ForProperty(e => e.OldFilePath)
        .Length(0, 1023);
      ForProperty(e => e.OldFilePermission)
        .Length(0, 1023);
      ForProperty(e => e.OldFileSize)
        .GreaterThan(0);
      ForProperty(e => e.OldFileType)
        .Length(0, 1023);
      ForProperty(e => e.BytesOut)
        .GreaterThan(0);
      ForProperty(e => e.EventOutcome)
        .Length(0, 63);
      ForProperty(e => e.TransportProtocol)
        .Length(0, 31);
      ForProperty(e => e.Reason)
        .Length(0, 1023);
      ForProperty(e => e.RequestUrl)
        .Length(0, 1023);
      ForProperty(e => e.RequestClientApplication)
        .Length(0, 1023);
      ForProperty(e => e.RequestContext)
        .Length(0, 2048);
      ForProperty(e => e.RequestCookies)
        .Length(0, 1023);
      ForProperty(e => e.RequestMethod)
        .Length(0, 1023);
      ForProperty(e => e.SourceHostName)
        .Length(1, 1023);
      ForProperty(e => e.SourceNtDomain)
        .Length(0, 255);
      ForProperty(e => e.SourceDnsDomain)
        .Length(0, 255);
      ForProperty(e => e.SourceServiceName)
        .Length(0, 1023);
      ForProperty(e => e.SourceTranslatedPort)
        .GreaterThan(0);
      ForProperty(e => e.SourceProcessId)
        .GreaterThan(0);
      ForProperty(e => e.SourceUserPrivileges)
        .Length(0, 1023);
      ForProperty(e => e.SourceProcessName)
        .Length(0, 1023);
      ForProperty(e => e.SourcePort)
        .GreaterThan(0);
      ForProperty(e => e.SourceUserId)
        .Length(0, 1023);
      ForProperty(e => e.SourceUserName)
        .Length(0, 1023);
      ForProperty(e => e.StartTime)
        .Must((x, c) => x.HasValue);
    }
  }
}