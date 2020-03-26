namespace ViennaNET.Word.Tests.Documents
{
  internal class ImageTestDocument : Document
  {
    public ImageTestDocument(string name, string value)
    {
      ImageState = (name, value);
    }

    public (string name, string value) ImageState { get; }
    public override string FileName => nameof(ImageTestDocument);

    protected override void Fill()
    {
      FillBarcode(ImageState.name, ImageState.value);
    }
  }
}