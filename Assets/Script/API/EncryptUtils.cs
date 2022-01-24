using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class EncryptUtils
{
    public static byte[] key = Convert.FromBase64String("AQIDBAUGBWGJCGSMDQ0PAA==");

    public static string Encrypt(string decodedStr)
    {
        // SDLogger.Log("Encrypt: " + decodedStr);
        byte[] data = Encoding.UTF8.GetBytes(decodedStr);
        using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
        {
            csp.KeySize = 256;
            csp.BlockSize = 128;
            csp.Key = key;
            csp.Padding = PaddingMode.PKCS7;
            csp.Mode = CipherMode.ECB;

            using (ICryptoTransform encrypter = csp.CreateEncryptor())
            {
                var arr = encrypter.TransformFinalBlock(data, 0, data.Length);
                return Convert.ToBase64String(arr);
            }
        }
    }

    public static string Decrypt(string encodedStr)
    {
        // SDLogger.Log("Decrypt: " + encodedStr);
        byte[] data = Convert.FromBase64String(encodedStr.Replace("_", "/"));
        using (AesCryptoServiceProvider csp = new AesCryptoServiceProvider())
        {
            csp.KeySize = 256;
            csp.BlockSize = 128;
            csp.Key = key;
            csp.Padding = PaddingMode.PKCS7;
            csp.Mode = CipherMode.ECB;

            using (ICryptoTransform decrypter = csp.CreateDecryptor())
            {
                var arr = decrypter.TransformFinalBlock(data, 0, data.Length);
                return Encoding.UTF8.GetString(arr);
            }
        }
    }

    public static string DecryptBase64(string base64)
    {
        var mBytes = Convert.FromBase64String(base64);
        var domain = ASCIIEncoding.ASCII.GetString(mBytes);
        return domain;
    }
}
