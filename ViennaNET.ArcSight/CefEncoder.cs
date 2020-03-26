using System.Linq;

namespace ViennaNET.ArcSight
{
  internal static class CefEncoder
  {
    public static string EncodeHeader(string value)
    {
      return new string(value.Trim().SelectMany(EncodeHeader).ToArray());
    }

    private static string EncodeHeader(char c)
    {
      switch (c)
      {
        case '|':
          return "\\|";
        case '\\':
          return "\\\\";
        default:
          return c.ToString();
      }
    }

    public static string EncodeExtension(string value)
    {
      return new string(value.Trim().SelectMany(EncodeExtension).ToArray());
    }

    private static string EncodeExtension(char c)
    {
      switch (c)
      {
        case '\\':
          return "\\\\";
        case '=':
          return "\\=";
        case '\r':
          return "\\r";
        case '\n':
          return "\\n";
        default:
          return c.ToString();
      }
    }
  }
}
