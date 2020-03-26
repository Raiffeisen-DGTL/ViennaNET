using System;
using NPOI.SS.Formula.Eval;
using NPOI.SS.Formula.Function;
using NPOI.SS.Formula.Functions;

namespace ViennaNET.Excel.Impl
{
  public abstract class NPoiFunctionEval : FunctionEval
  {
    public static void ReplaceFunction(string name, Function function)
    {
      var func = FunctionMetadataRegistry.GetFunctionByName(name.ToUpper());

      if (func == null)
      {
        throw new InvalidOperationException($"Functin {name} not found in NPoi.");
      }

      functions[func.Index] = function;
    }
  }
}
