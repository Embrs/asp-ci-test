
using System.Security.Cryptography;
using System.Text;

namespace MyApp.Utils;
public static class AesEncryption {
  private static readonly string Key = "your-32-byte-long-secret-key"; // 長度需為 32 byte
  private static readonly string IV = "your-16-byte-iv-str"; // 長度需為 16 byte

  public static string Encrypt(string plainText) {
    using var aes = Aes.Create();
    aes.Key = Encoding.UTF8.GetBytes(Key);
    aes.IV = Encoding.UTF8.GetBytes(IV);

    var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
    using var ms = new MemoryStream();
    using var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write);
    using var sw = new StreamWriter(cs);
    sw.Write(plainText);

    return Convert.ToBase64String(ms.ToArray());
  }

  public static string Decrypt(string cipherText) {
    using var aes = Aes.Create();
    aes.Key = Encoding.UTF8.GetBytes(Key);
    aes.IV = Encoding.UTF8.GetBytes(IV);

    var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    using var ms = new MemoryStream(Convert.FromBase64String(cipherText));
    using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
    using var sr = new StreamReader(cs);

    return sr.ReadToEnd();
  }
}
