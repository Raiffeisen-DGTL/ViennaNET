using System.Collections.Generic;

namespace ViennaNET.Word.Tests.Documents
{
  internal class TableTestDocument : Document
  {
    public TableTestDocument(string name, IEnumerable<object> value)
    {
      TableState = (name, value);
    }

    public (string name, IEnumerable<object> value) TableState { get; }
    public override string FileName => nameof(TableTestDocument);

    protected override void Fill()
    {
      FillSdtElement(TableState.name, TableState.value, true);
    }

    internal class TestTableRow 
    {
      public TestTableRow(string name, int id, string firstName, bool gender)
      {
        Name = name;
        Id = id;
        FirstName = firstName;
        Gender = gender;
      }

      public string Name { get; }
      public int Id { get; }
      public string FirstName { get; }
      public bool Gender { get; }
    }
  }
}