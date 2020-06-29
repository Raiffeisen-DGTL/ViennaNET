using System;
using NUnit.Framework;

namespace ViennaNET.Protection.Tests.Unit
{
  [TestFixture, Category("Unit"), TestOf(typeof(ProtectUtils))]
  public class ProtectUtilsTest
  {
    private const string PlainText = "any_password";

    [TestCase("0123456789ABCDEFFEDCBA9876543210")]
    [TestCase("00000000000000000000000000000000")]
    [TestCase("____any_secret_32_symbol_key____")]
    public void Protect_Unprotect_Correct(string dataKey)
    {
      Environment.SetEnvironmentVariable("CRYPTO_KEY", dataKey);

      var cipherText = PlainText.Protect();

      Assert.AreEqual(PlainText, cipherText.Unprotect());
    }
  }
}
