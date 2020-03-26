using ViennaNET.Validation.Rules.FluentRule;

namespace ViennaNET.ArcSight.Validation
{
  internal class CefMessageRule : BaseFluentRule<CefMessage>
  {
    public CefMessageRule()
    {
      ForProperty(m => m.DeviceVendor)
        .NotEmpty();
      ForProperty(m => m.DeviceProduct)
        .NotEmpty();
      ForProperty(m => m.DeviceVersion)
        .NotEmpty();
      ForProperty(m => m.DeviceEventClassId)
        .GreaterThanOrEqual(0);
      ForProperty(m => m.Name)
        .NotEmpty();
      ForProperty(e => e.HostName)
        .Length(1, 1023)
        .WithErrorMessage("HostName length should be between 1 and 1023");
      ForProperty(m => m.Extensions)
        .UseRule(new ExtensionsRule());
    }
  }
}
