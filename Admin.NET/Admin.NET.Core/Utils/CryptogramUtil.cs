// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

namespace Admin.NET.Core;

public class CryptogramUtil
{
    public static readonly bool StrongPassword = App.GetConfig<bool>("Cryptogram:StrongPassword"); // Whether to enable password strength verification
    public static readonly string PasswordStrengthValidation = App.GetConfig<string>("Cryptogram:PasswordStrengthValidation"); // Password strength verification regular expression
    public static readonly string PasswordStrengthValidationMsg = App.GetConfig<string>("Cryptogram:PasswordStrengthValidationMsg"); // Password strength verification prompts
    public static readonly string CryptoType = App.GetConfig<string>("Cryptogram:CryptoType"); // Encryption type
    public static readonly string PublicKey = App.GetConfig<string>("Cryptogram:PublicKey"); // public key
    public static readonly string PrivateKey = App.GetConfig<string>("Cryptogram:PrivateKey"); // private key

    public static readonly string SM4_key = "0123456789abcdeffedcba9876543210";
    public static readonly string SM4_iv = "595298c7c6fd271f0402f804c33d3f66";

    /// <summary>
    /// encryption
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string Encrypt(string plainText)
    {
        if (CryptoType == CryptogramEnum.MD5.ToString())
        {
            return MD5Encryption.Encrypt(plainText);
        }
        else if (CryptoType == CryptogramEnum.SM2.ToString())
        {
            return SM2Encrypt(plainText);
        }
        else if (CryptoType == CryptogramEnum.SM4.ToString())
        {
            return SM4EncryptECB(plainText);
        }
        return plainText;
    }

    /// <summary>
    /// Decrypt
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string Decrypt(string cipherText)
    {
        if (CryptoType == CryptogramEnum.SM2.ToString())
        {
            return SM2Decrypt(cipherText);
        }
        else if (CryptoType == CryptogramEnum.SM4.ToString())
        {
            return SM4DecryptECB(cipherText);
        }
        return cipherText;
    }

    /// <summary>
    /// SM2 encryption
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string SM2Encrypt(string plainText)
    {
        return GMUtil.SM2Encrypt(PublicKey, plainText);
    }

    /// <summary>
    /// SM2 decryption
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string SM2Decrypt(string cipherText)
    {
        return GMUtil.SM2Decrypt(PrivateKey, cipherText);
    }

    /// <summary>
    /// SM4 encryption (ECB)
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string SM4EncryptECB(string plainText)
    {
        return GMUtil.SM4EncryptECB(SM4_key, plainText);
    }

    /// <summary>
    /// SM4 Decryption (ECB)
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string SM4DecryptECB(string cipherText)
    {
        return GMUtil.SM4DecryptECB(SM4_key, cipherText);
    }

    /// <summary>
    /// SM4 encryption (CBC)
    /// </summary>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string SM4EncryptCBC(string plainText)
    {
        return GMUtil.SM4EncryptCBC(SM4_key, SM4_iv, plainText);
    }

    /// <summary>
    /// SM4 Decryption (CBC)
    /// </summary>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string SM4DecryptCBC(string cipherText)
    {
        return GMUtil.SM4DecryptCBC(SM4_key, SM4_iv, cipherText);
    }
}