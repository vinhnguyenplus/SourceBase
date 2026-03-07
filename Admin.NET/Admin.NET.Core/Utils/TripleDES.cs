// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using System.Security.Cryptography;

namespace Admin.NET.Core;

/// <summary>
/// 3DES file encryption and decryption
/// </summary>
public static class TripleDES
{
    /// <summary>
    /// Encrypt files
    /// </summary>
    /// <param name="inputFile">Path of file to be encrypted</param>
    /// <param name="outputFile">Encrypted file path</param>
    /// <param name="password">Password (24-bit length)</param>
    [Obsolete]
    public static void EncryptFile(string inputFile, string outputFile, string password)
    {
        using var ties = new TripleDESCryptoServiceProvider();
        ties.Mode = CipherMode.ECB;
        ties.Padding = PaddingMode.PKCS7;
        ties.Key = Encoding.UTF8.GetBytes(password);
        using var inputFileStream = new FileStream(inputFile, FileMode.Open);
        using var encryptedFileStream = new FileStream(outputFile, FileMode.Create);
        using var cryptoStream = new CryptoStream(encryptedFileStream, ties.CreateEncryptor(), CryptoStreamMode.Write);
        inputFileStream.CopyTo(cryptoStream);
    }

    /// <summary>
    /// Encrypt files
    /// </summary>
    /// <param name="inputFile">Encrypted file path</param>
    /// <param name="outputFile">Decrypted file path</param>
    /// <param name="password">Password (24-bit length)</param>
    [Obsolete]
    public static void DecryptFile(string inputFile, string outputFile, string password)
    {
        using var ties = new TripleDESCryptoServiceProvider();
        ties.Mode = CipherMode.ECB;
        ties.Padding = PaddingMode.PKCS7;
        ties.Key = Encoding.UTF8.GetBytes(password);
        using var encryptedFileStream = new FileStream(inputFile, FileMode.Open);
        using var decryptedFileStream = new FileStream(outputFile, FileMode.Create);
        using var cryptoStream = new CryptoStream(encryptedFileStream, ties.CreateDecryptor(), CryptoStreamMode.Read);
        cryptoStream.CopyTo(decryptedFileStream);
    }
}