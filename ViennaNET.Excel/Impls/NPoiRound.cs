using System;
using NPOI.SS.Formula.Functions;

namespace ViennaNET.Excel.Impl
{
  public class NPoiRound : TwoArg
  {
    public override double Evaluate(double d0, double d1)
    {
      return (double)Math.Round((decimal)d0, (int)d1, MidpointRounding.AwayFromZero);
    }
  }
}
