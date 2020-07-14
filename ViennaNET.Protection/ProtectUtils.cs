using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ViennaNET.Protection
{
  /// <summary>
  ///   Обеспечивает шифрование и расшифровку строки
  /// </summary>
  public static class ProtectUtils
  {
    private const string environmentVariable = "CRYPTO_KEY";

    /// <summary>
    ///   Шифрует строку
    /// </summary>
    /// <param name="plainText">Текст для шифрования</param>
    /// <returns>Зашифрованная строка</returns>
    public static string Protect(this string plainText)
    {
      var buffer = Encoding.UTF8.GetBytes(plainText);
      var key = GetValueEnvironmentVariable(environmentVariable);

      using (var aes = Aes.Create())
      {
        if (aes == null)
        {
          throw new ArgumentException("Parameter must not be null.", nameof(aes));
        }

        aes.Key = key;

        using (var encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
        using (var resultStream = new MemoryStream())
        {
          using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
          using (var plainStream = new MemoryStream(buffer))
          {
            plainStream.CopyTo(aesStream);
          }

          var result = resultStream.ToArray();
          var combined = new byte[aes.IV.Length + result.Length];
          Array.ConstrainedCopy(aes.IV, 0, combined, 0, aes.IV.Length);
          Array.ConstrainedCopy(result, 0, combined, aes.IV.Length, result.Length);

          return Convert.ToBase64String(combined);
        }
      }
    }

    /// <summary>
    ///   Расшифровывает ранее зашифрованную строку
    /// </summary>
    /// <param name="cipherText">Зашифрованный текст</param>
    /// <returns>Расшифрованная строка</returns>
    public static string Unprotect(this string cipherText)
    {
      var buffer = Convert.FromBase64String(cipherText);

      using (var aes = Aes.Create())
      {
        if (aes == null)
        {
          throw new ArgumentException("Parameter must not be null.", nameof(aes));
        }

        var iv = new byte[aes.IV.Length];
        var key = GetValueEnvironmentVariable(environmentVariable);
        var ciphertext = new byte[buffer.Length - iv.Length];

        Array.ConstrainedCopy(buffer, 0, iv, 0, iv.Length);
        Array.ConstrainedCopy(buffer, iv.Length, ciphertext, 0, ciphertext.Length);

        aes.IV = iv;
        aes.Key = key;

        using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
        using (var resultStream = new MemoryStream())
        {
          using (var aesStream = new CryptoStream(resultStream, decryptor, CryptoStreamMode.Write))
          using (var plainStream = new MemoryStream(ciphertext))
          {
            plainStream.CopyTo(aesStream);
          }

          return Encoding.UTF8.GetString(resultStream.ToArray());
        }
      }
    }

    private static byte[] GetValueEnvironmentVariable(string dataEnvironment)
    {
      var data = Environment.GetEnvironmentVariable(dataEnvironment);

      if (data == null)
      {
        throw new ArgumentException($"Environment variable {dataEnvironment} is not defined.");
      }

      return Encoding.UTF8.GetBytes(data);
    }
  }
}