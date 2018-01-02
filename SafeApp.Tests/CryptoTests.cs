﻿using System;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SafeApp.Misc;
using SafeApp.Utilities;

namespace SafeApp.Tests {
  [TestFixture]
  internal class CryptoTests {

    [Test]
    public async Task GetPublicEncryptKey() {
      var session = await MockAuthBindings.MockSession.CreateTestApp();
      var encKeyPairTuple = await session.Crypto.EncGenerateKeyPairAsync();
      using (encKeyPairTuple.Item1)
      using (encKeyPairTuple.Item2)
      {
        var rawKey = await session.Crypto.EncPubKeyGetAsync(encKeyPairTuple.Item1);
        Assert.AreEqual(rawKey.Count, AppConstants.AsymSecretKeyLen);
        var handle = await session.Crypto.EncPubKeyNewAsync(rawKey);
        Assert.NotNull(handle);
        rawKey = await session.Crypto.EncSecretKeyGetAsync(encKeyPairTuple.Item2);
        Assert.AreEqual(rawKey.Count, AppConstants.AsymSecretKeyLen);
        handle = await session.Crypto.EncSecretKeyNewAsync(rawKey);
        Assert.NotNull(handle);
        session.FreeApp();
      }
    }

    [Test]
    public async Task SealedBoxEncryption() {
      var session = await MockAuthBindings.MockSession.CreateTestApp();
      var encKeyPairTuple = await session.Crypto.EncGenerateKeyPairAsync();
      using (encKeyPairTuple.Item1)
      using (encKeyPairTuple.Item1)
      {
        var plainBytes = new byte[1024];
        new Random().NextBytes(plainBytes);
        var cipherBytes = await session.Crypto.EncryptSealedBoxAsync(plainBytes.ToList(), encKeyPairTuple.Item1);
        var decryptedBytes = await session.Crypto.DecryptSealedBoxAsync(cipherBytes, encKeyPairTuple.Item1, encKeyPairTuple.Item2);
        Assert.AreEqual(plainBytes, decryptedBytes);
      }
      session.FreeApp();
    }

    [Test]
    public async Task SymmetricEncryption() {
      var session = await MockAuthBindings.MockSession.CreateTestApp();
      var encKeyPairTuple = await session.Crypto.EncGenerateKeyPairAsync();
      using (encKeyPairTuple.Item1)
      using (encKeyPairTuple.Item1)
      {
        var plainBytes = new byte[1024];
        new Random().NextBytes(plainBytes);
        var cipherBytes = await session.Crypto.EncryptAsync(plainBytes.ToList(), encKeyPairTuple.Item1, encKeyPairTuple.Item2);
        var decryptedBytes = await session.Crypto.DecryptAsync(cipherBytes, encKeyPairTuple.Item1, encKeyPairTuple.Item2);
        Assert.AreEqual(plainBytes, decryptedBytes.ToArray());
      }
      session.FreeApp();
    }

    [Test]
    public async Task SignAndVerify()
    {
      var session = await MockAuthBindings.MockSession.CreateTestApp();
      var signKeyPairTuple = await session.Crypto.SignGenerateKeyPairAsync();
      using (signKeyPairTuple.Item1)
      using (signKeyPairTuple.Item1)
      {
        var plainBytes = new byte[1024];
        new Random().NextBytes(plainBytes);
        var signedData = await session.Crypto.SignAsync(plainBytes.ToList(), signKeyPairTuple.Item2);
        var verifiedData = await session.Crypto.VerifyAsync(signedData, signKeyPairTuple.Item1);
        Assert.AreEqual(plainBytes, verifiedData.ToArray());
      }
      session.FreeApp();
    }
  }
}
