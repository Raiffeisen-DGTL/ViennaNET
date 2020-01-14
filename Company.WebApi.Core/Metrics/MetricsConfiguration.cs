namespace Company.WebApi.Core.Metrics
{
  internal sealed class MetricsConfiguration
  {
    public MetricsConfiguration()
    {
      Enabled = false;
      Reporter = MetricsReportersTypes.Default;
    }

    public bool Enabled { get; set; }
    public string Reporter { get; set; }
  }
}
