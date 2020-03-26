using System;
using ViennaNET.Excel.Contracts;
using NPOI.SS.UserModel;

namespace ViennaNET.Excel.Impl
{
  internal class NPoiStyle : IStyle
  {
    private readonly IWorkbook _workbook;

    public NPoiStyle(ICellStyle style, IWorkbook workbook)
    {
      Style = style ?? throw new ArgumentNullException(nameof(style));
      _workbook = workbook ?? throw new ArgumentNullException(nameof(workbook));
    }

    internal ICellStyle Style { get; set; }

    public void SetFormat(string format)
    {
      Style.DataFormat = _workbook.CreateDataFormat()
                                  .GetFormat(format);
    }
  }
}
