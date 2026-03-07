// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Utilities.Encoders;

namespace Admin.NET.Core;

/// <summary>
/// GM tools
/// </summary>
public class GMUtil
{
    /// <summary>
    /// SM2 encryption
    /// </summary>
    /// <param name="publicKeyHex"></param>
    /// <param name="data_string"></param>
    /// <returns></returns>
    public static string SM2Encrypt(string publicKeyHex, string data_string)
    {
        // If it is a 130-bit public key, if it is used by .NET, cut off the 04 at the beginning.
        if (publicKeyHex.Length == 130)
        {
            publicKeyHex = publicKeyHex.Substring(2, 128);
        }
        // Public key X, first 64 bits
        string x = publicKeyHex.Substring(0, 64);
        // Public key Y, last 64 bits
        string y = publicKeyHex.Substring(64);
        // Get the public key object
        AsymmetricKeyParameter publicKey1 = GM.GetPublickeyFromXY(new BigInteger(x, 16), new BigInteger(y, 16));
        // Sm2Encrypt: C1C3C2
        // Sm2EncryptOld: C1C2C3
        byte[] digestByte = GM.Sm2Encrypt(Encoding.UTF8.GetBytes(data_string), publicKey1);
        string strSM2 = Hex.ToHexString(digestByte);
        return strSM2;
    }

    /// <summary>
    /// SM2 decryption
    /// </summary>
    /// <param name="privateKey_string"></param>
    /// <param name="encryptedData_string"></param>
    /// <returns></returns>
    public static string SM2Decrypt(string privateKey_string, string encryptedData_string)
    {
        if (!encryptedData_string.StartsWith("04"))
            encryptedData_string = "04" + encryptedData_string;
        BigInteger d = new(privateKey_string, 16);
        // First get the private key object, use ECPrivateKeyParameters or AsymmetricKeyParameter.
        // ECPrivateKeyParameters bcecPrivateKey = GmUtil.GetPrivatekeyFromD(d);
        AsymmetricKeyParameter bcecPrivateKey = GM.GetPrivatekeyFromD(d);
        byte[] byToDecrypt = Hex.Decode(encryptedData_string);
        byte[] byDecrypted = GM.Sm2Decrypt(byToDecrypt, bcecPrivateKey);
        string strDecrypted = Encoding.UTF8.GetString(byDecrypted);
        return strDecrypted;
    }

    /// <summary>
    /// SM4 encryption (ECB)
    /// </summary>
    /// <param name="key_string"></param>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string SM4EncryptECB(string key_string, string plainText)
    {
        byte[] key = Hex.Decode(key_string);
        byte[] bs = GM.Sm4EncryptECB(key, Encoding.UTF8.GetBytes(plainText), GM.SM4_ECB_PKCS7PADDING);// In the case of NoPadding, the verification data length needs to be a multiple of 16. Use HandleSm4Padding to process
        return Hex.ToHexString(bs);
    }

    /// <summary>
    /// SM4 Decryption (ECB)
    /// </summary>
    /// <param name="key_string"></param>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string SM4DecryptECB(string key_string, string cipherText)
    {
        byte[] key = Hex.Decode(key_string);
        byte[] bs = GM.Sm4DecryptECB(key, Hex.Decode(cipherText), GM.SM4_ECB_PKCS7PADDING);
        return Encoding.UTF8.GetString(bs);
    }

    /// <summary>
    /// SM4 encryption (CBC)
    /// </summary>
    /// <param name="key_string"></param>
    /// <param name="iv_string"></param>
    /// <param name="plainText"></param>
    /// <returns></returns>
    public static string SM4EncryptCBC(string key_string, string iv_string, string plainText)
    {
        byte[] key = Hex.Decode(key_string);
        byte[] iv = Hex.Decode(iv_string);
        byte[] bs = GM.Sm4EncryptCBC(key, Encoding.UTF8.GetBytes(plainText), iv, GM.SM4_CBC_PKCS7PADDING);
        return Hex.ToHexString(bs);
    }

    /// <summary>
    /// SM4 Decryption (CBC)
    /// </summary>
    /// <param name="key_string"></param>
    /// <param name="iv_string"></param>
    /// <param name="cipherText"></param>
    /// <returns></returns>
    public static string SM4DecryptCBC(string key_string, string iv_string, string cipherText)
    {
        byte[] key = Hex.Decode(key_string);
        byte[] iv = Hex.Decode(iv_string);
        byte[] bs = GM.Sm4DecryptCBC(key, Hex.Decode(cipherText), iv, GM.SM4_CBC_PKCS7PADDING);
        return Encoding.UTF8.GetString(bs);
    }

    /// <summary>
    /// Complement the 0 characters of the hexadecimal string and return the hexadecimal string without 0x
    /// </summary>
    /// <param name="input"></param>
    /// <param name="mode">1 means encryption, 0 means decryption</param>
    /// <returns></returns>
    private static byte[] HandleSm4Padding(byte[] input, int mode)
    {
        if (input == null)
        {
            return null;
        }
        byte[] ret = (byte[])null;
        if (mode == 1)
        {
            int p = 16 - input.Length % 16;
            ret = new byte[input.Length + p];
            Array.Copy(input, 0, ret, 0, input.Length);
            for (int i = 0; i < p; i++)
            {
                ret[input.Length + i] = (byte)p;
            }
        }
        else
        {
            int p = input[input.Length - 1];
            ret = new byte[input.Length - p];
            Array.Copy(input, 0, ret, 0, input.Length - p);
        }
        return ret;
    }
}