using System;
using System.Diagnostics;
using System.IO;
using ViennaNET.FileUtils.Interfaces;

namespace ViennaNET.FileUtils.Implementations
{
  public class FileUtilsImpl : IFileUtils
  {
    private Stream GetStreamFromBytes(byte[] bytes)
    {
      var stream = new MemoryStream();
      stream.Write(bytes, 0, bytes.Length);
      stream.Seek(0, SeekOrigin.Begin);
      return stream;
    }

    public Stream GetStreamFromFile(string fileName)
    {
      var bytes = ReadAllBytesFromFile(fileName);
      return GetStreamFromBytes(bytes);
    }

    private byte[] ReadAllBytesFromFile(string fileName)
    {
      using (var file = GetFileStream(fileName))
      {
        var length = file.Length;
        if (length > int.MaxValue)
        {
          throw new InvalidOperationException("File size is too big");
        }
        var count = (int)length;
        var buffer = new byte[count];
        var offset = 0;
        while (count > 0)
        {
          var num = file.Read(buffer, offset, count);
          if (num == 0)
          {
            throw new InvalidOperationException("Unexpected end of file");
          }
          offset += num;
          count -= num;
        }
        return buffer;
      }
    }

    private static Stream GetFileStream(string fileName)
    {
      var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      return stream;
    }
  }
}
