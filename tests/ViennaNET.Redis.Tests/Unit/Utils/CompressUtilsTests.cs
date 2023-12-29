﻿using NUnit.Framework;
using ViennaNET.Redis.Exceptions;
using ViennaNET.Redis.Utils;

namespace ViennaNET.Redis.Tests.Unit.Utils
{
    [TestFixture(Category = "Unit", TestOf = typeof(CompressUtils))]
    public class CompressUtilsTests
    {
        [Test]
        public void CompressString_Exception()
        {
            Assert.Catch<RedisException>(() => CompressUtils.CompressString(null!));
        }

        [Test]
        public void DecompressString_Null()
        {
            var actual = CompressUtils.DecompressString(null!);

            Assert.That(actual, Is.Null);
        }

        [Test]
        public void DecompressString_Exception()
        {
            Assert.Catch<RedisException>(() => CompressUtils.DecompressString("==??"));
        }
    }
}