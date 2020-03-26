namespace ViennaNET.Word.Tests.Documents
{
  internal class CheckboxTestDocument : Document
  {
    public CheckboxTestDocument(string name, bool value)
    {
      CheckboxState = (name, value);
    }

    public (string name, bool value) CheckboxState { get; }
    public override string FileName => nameof(CheckboxTestDocument);

    protected override void Fill()
    {
      FillSdtElement(CheckboxState.name, CheckboxState.value);
      if (CheckboxState.value)
      {
        RemoveSdtElement(string.Empty);
      }
    }
  }
}