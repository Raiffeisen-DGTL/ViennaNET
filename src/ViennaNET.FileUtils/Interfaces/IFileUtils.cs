using System.IO;

namespace ViennaNET.FileUtils.Interfaces
{
  public interface IFileUtils
  {
    Stream GetStreamFromFile(string fileName);
  }
}