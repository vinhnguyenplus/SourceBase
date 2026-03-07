// The copyright, trademark, patent and other related rights of the Admin.NET project are protected by corresponding laws and regulations. Use of this project shall comply with relevant laws, regulations and license requirements.
//
// This project is distributed and used primarily under the MIT License and the Apache License (version 2.0). The license is located in the LICENSE-MIT and LICENSE-APACHE files in the root of the source tree.
//
// This project may not be used to engage in activities that endanger national security, disrupt social order, infringe on the legitimate rights and interests of others, and other activities prohibited by laws and regulations! We do not assume any responsibility for any legal disputes and liabilities arising from the secondary development of this project!

using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.GM;
using Org.BouncyCastle.Asn1.X9;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Digests;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Utilities;
using Org.BouncyCastle.Utilities.Encoders;
using Org.BouncyCastle.X509;

namespace Admin.NET.Core;

/**
 *
 * useBCPoints to note：
 * This version ofBCCorrectSM3withSM2The result isasn1Format'srands，If direct concatenation is neededr||sNeed selfselfConvert。BelowrsAsn1ToPlainByteArray、rsPlainByteArrayToAsn1Just doing this。
 * This version ofBCCorrectSM2The result isC1||C2||C3，It is said to be the old standard，The new standard isC1||C3||C2，Need to use the new standardselfConvert。Below（Commented out）changeC1C2C3ToC1C3C2、changeC1C3C2ToC1C2C3Just doing this。javaThe higher version of the edition has been addedC1C3C2，csharpThe edition might be added in the future too，But not yet，javaThe current version is availableBeginningInitializationtime“ SM2Engine sm2Engine = new SM2Engine(SM2Engine.Mode.C1C3C2);”。
 *
 */

public class GM
{
    private static X9ECParameters x9ECParameters = GMNamedCurves.GetByName("sm2p256v1");
    private static ECDomainParameters ecDomainParameters = new(x9ECParameters.Curve, x9ECParameters.G, x9ECParameters.N);

    /**
     *
     * @param msg
     * @param userId
     * @param privateKey
     * @return r||s，Direct concatenationbytenumbergroupofrs
     */

    public static byte[] SignSm3WithSm2(byte[] msg, byte[] userId, AsymmetricKeyParameter privateKey)
    {
        return RsAsn1ToPlainByteArray(SignSm3WithSm2Asn1Rs(msg, userId, privateKey));
    }

    /**
      * @param msg
      * @param userId
      * @param privateKey
      * @return rs in <b>asn1 format</b>
      */

    public static byte[] SignSm3WithSm2Asn1Rs(byte[] msg, byte[] userId, AsymmetricKeyParameter privateKey)
    {
        ISigner signer = SignerUtilities.GetSigner("SM3withSM2");
        signer.Init(true, new ParametersWithID(privateKey, userId));
        signer.BlockUpdate(msg, 0, msg.Length);
        byte[] sig = signer.GenerateSignature();
        return sig;
    }

    /**
    *
    * @param msg
    * @param userId
    * @param rs r||s，Direct concatenationbytenumbergroupofrs
    * @param publicKey
    * @return
    */

    public static bool VerifySm3WithSm2(byte[] msg, byte[] userId, byte[] rs, AsymmetricKeyParameter publicKey)
    {
        if (rs == null || msg == null || userId == null) return false;
        if (rs.Length != RS_LEN * 2) return false;
        return VerifySm3WithSm2Asn1Rs(msg, userId, RsPlainByteArrayToAsn1(rs), publicKey);
    }

    /**
     *
     * @param msg
     * @param userId
     * @param rs in <b>asn1 format</b>
     * @param publicKey
     * @return
     */

    public static bool VerifySm3WithSm2Asn1Rs(byte[] msg, byte[] userId, byte[] sign, AsymmetricKeyParameter publicKey)
    {
        ISigner signer = SignerUtilities.GetSigner("SM3withSM2");
        signer.Init(false, new ParametersWithID(publicKey, userId));
        signer.BlockUpdate(msg, 0, msg.Length);
        return signer.VerifySignature(sign);
    }

    /**
     * bcEncryption and decryption use the old tagc1||c2||c3，This method is called after encryption，Convert the result toc1||c3||c2
     * @param c1c2c3
     * @return
     */

    private static byte[] ChangeC1C2C3ToC1C3C2(byte[] c1c2c3)
    {
        int c1Len = (x9ECParameters.Curve.FieldSize + 7) / 8 * 2 + 1; // This is fixed to 65 for sm2p256v1. You can see the GMNamedCurves and ECCurve codes.
        const int c3Len = 32; //new SM3Digest().getDigestSize();
        byte[] result = new byte[c1c2c3.Length];
        Buffer.BlockCopy(c1c2c3, 0, result, 0, c1Len); //c1
        Buffer.BlockCopy(c1c2c3, c1c2c3.Length - c3Len, result, c1Len, c3Len); //c3
        Buffer.BlockCopy(c1c2c3, c1Len, result, c1Len + c3Len, c1c2c3.Length - c1Len - c3Len); //c2
        return result;
    }

    /**
     * bcEncryption and decryption use the old tagc1||c3||c2，This method is called before decryption，Convert ciphertext toc1||c2||c3Decrypt again
     * @param c1c3c2
     * @return
     */

    private static byte[] ChangeC1C3C2ToC1C2C3(byte[] c1c3c2)
    {
        int c1Len = (x9ECParameters.Curve.FieldSize + 7) / 8 * 2 + 1; // This is fixed to 65 for sm2p256v1. You can see the GMNamedCurves and ECCurve codes.
        const int c3Len = 32; //new SM3Digest().GetDigestSize();
        byte[] result = new byte[c1c3c2.Length];
        Buffer.BlockCopy(c1c3c2, 0, result, 0, c1Len); //c1: 0->65
        Buffer.BlockCopy(c1c3c2, c1Len + c3Len, result, c1Len, c1c3c2.Length - c1Len - c3Len); //c2
        Buffer.BlockCopy(c1c3c2, c1Len, result, c1c3c2.Length - c3Len, c3Len); //c3
        return result;
    }

    /**
     * c1||c3||c2
     * @param data
     * @param key
     * @return
     */

    public static byte[] Sm2Decrypt(byte[] data, AsymmetricKeyParameter key)
    {
        return Sm2DecryptOld(ChangeC1C3C2ToC1C2C3(data), key);
    }

    /**
     * c1||c3||c2
     * @param data
     * @param key
     * @return
     */

    public static byte[] Sm2Encrypt(byte[] data, AsymmetricKeyParameter key)
    {
        return ChangeC1C2C3ToC1C3C2(Sm2EncryptOld(data, key));
    }

    /**
     * c1||c2||c3
     * @param data
     * @param key
     * @return
     */

    public static byte[] Sm2EncryptOld(byte[] data, AsymmetricKeyParameter pubkey)
    {
        SM2Engine sm2Engine = new SM2Engine();
        sm2Engine.Init(true, new ParametersWithRandom(pubkey, new SecureRandom()));
        return sm2Engine.ProcessBlock(data, 0, data.Length);
    }

    /**
     * c1||c2||c3
     * @param data
     * @param key
     * @return
     */

    public static byte[] Sm2DecryptOld(byte[] data, AsymmetricKeyParameter key)
    {
        SM2Engine sm2Engine = new SM2Engine();
        sm2Engine.Init(false, key);
        return sm2Engine.ProcessBlock(data, 0, data.Length);
    }

    /**
     * @param bytes
     * @return
     */

    public static byte[] Sm3(byte[] bytes)
    {
        SM3Digest digest = new();
        digest.BlockUpdate(bytes, 0, bytes.Length);
        byte[] result = DigestUtilities.DoFinal(digest);
        return result;
    }

    private const int RS_LEN = 32;

    private static byte[] BigIntToFixexLengthBytes(BigInteger rOrS)
    {
        // for sm2p256v1, n is 00fffffffeffffffffffffffffffffffff7203df6b21c6052b53bbf40939d54123,
        // r and s are the result of mod n, so they should be less than n and have length<=32
        byte[] rs = rOrS.ToByteArray();
        if (rs.Length == RS_LEN) return rs;
        else if (rs.Length == RS_LEN + 1 && rs[0] == 0) return Arrays.CopyOfRange(rs, 1, RS_LEN + 1);
        else if (rs.Length < RS_LEN)
        {
            byte[] result = new byte[RS_LEN];
            Arrays.Fill(result, (byte)0);
            Buffer.BlockCopy(rs, 0, result, RS_LEN - rs.Length, rs.Length);
            return result;
        }
        else
        {
            throw new ArgumentException("err rs: " + Hex.ToHexString(rs));
        }
    }

    /**
     * BCofSM3withSM2Signatureof the obtained resultrsYesasn1Format's，This method is converted into direct concatenationr||s
     * @param rsDer rs in asn1 format
     * @return sign result in plain byte array
     */

    private static byte[] RsAsn1ToPlainByteArray(byte[] rsDer)
    {
        Asn1Sequence seq = Asn1Sequence.GetInstance(rsDer);
        byte[] r = BigIntToFixexLengthBytes(DerInteger.GetInstance(seq[0]).Value);
        byte[] s = BigIntToFixexLengthBytes(DerInteger.GetInstance(seq[1]).Value);
        byte[] result = new byte[RS_LEN * 2];
        Buffer.BlockCopy(r, 0, result, 0, r.Length);
        Buffer.BlockCopy(s, 0, result, RS_LEN, s.Length);
        return result;
    }

    /**
     * BCofSM3withSM2Required for signature verificationrsYesasn1Format's，This method will concatenate directlyr||snumber of bytesgroupconvert intoasn1Format
     * @param sign in plain byte array
     * @return rs result in asn1 format
     */

    private static byte[] RsPlainByteArrayToAsn1(byte[] sign)
    {
        if (sign.Length != RS_LEN * 2) throw new ArgumentException("err rs. ");
        BigInteger r = new BigInteger(1, Arrays.CopyOfRange(sign, 0, RS_LEN));
        BigInteger s = new BigInteger(1, Arrays.CopyOfRange(sign, RS_LEN, RS_LEN * 2));
        Asn1EncodableVector v = new Asn1EncodableVector
        {
            new DerInteger(r),
            new DerInteger(s)
        };

        return new DerSequence(v).GetEncoded("DER");
    }

    // Generate public and private key pairs
    public static AsymmetricCipherKeyPair GenerateKeyPair()
    {
        ECKeyPairGenerator kpGen = new();
        kpGen.Init(new ECKeyGenerationParameters(ecDomainParameters, new SecureRandom()));
        return kpGen.GenerateKeyPair();
    }

    public static ECPrivateKeyParameters GetPrivatekeyFromD(BigInteger d)
    {
        return new ECPrivateKeyParameters(d, ecDomainParameters);
    }

    public static ECPublicKeyParameters GetPublickeyFromXY(BigInteger x, BigInteger y)
    {
        return new ECPublicKeyParameters(x9ECParameters.Curve.CreatePoint(x, y), ecDomainParameters);
    }

    public static AsymmetricKeyParameter GetPublickeyFromX509File(FileInfo file)
    {
        FileStream fileStream = null;
        try
        {
            //file.DirectoryName + "\\" + file.Name
            fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read);
            X509Certificate certificate = new X509CertificateParser().ReadCertificate(fileStream);
            return certificate.GetPublicKey();
        }
        catch (Exception)
        {
            //log.Error(file.Name + "Reading failed, exception: " + e);
        }
        finally
        {
            if (fileStream != null)
                fileStream.Close();
        }
        return null;
    }

    public class Sm2Cert
    {
        public AsymmetricKeyParameter privateKey;
        public AsymmetricKeyParameter publicKey;
        public string certId;
    }

    private static byte[] ToByteArray(int i)
    {
        byte[] byteArray = new byte[4];
        byteArray[0] = (byte)(i >> 24);
        byteArray[1] = (byte)((i & 0xFFFFFF) >> 16);
        byteArray[2] = (byte)((i & 0xFFFF) >> 8);
        byteArray[3] = (byte)(i & 0xFF);
        return byteArray;
    }

    /**
     * Number of bytesgroupsplicing
     *
     * @param params
     * @return
     */

    private static byte[] Join(params byte[][] byteArrays)
    {
        List<byte> byteSource = new();
        for (int i = 0; i < byteArrays.Length; i++)
        {
            byteSource.AddRange(byteArrays[i]);
        }
        byte[] data = byteSource.ToArray();
        return data;
    }

    /**
     * keyDerived function
     *
     * @param Z
     * @param klen
     * GenerateklenNumber of byteslengthofkey
     * @return
     */

    private static byte[] KDF(byte[] Z, int klen)
    {
        int ct = 1;
        int end = (int)Math.Ceiling(klen * 1.0 / 32);
        List<byte> byteSource = new();

        for (int i = 1; i < end; i++)
        {
            byteSource.AddRange(Sm3(Join(Z, ToByteArray(ct))));
            ct++;
        }
        byte[] last = Sm3(Join(Z, ToByteArray(ct)));
        if (klen % 32 == 0)
        {
            byteSource.AddRange(last);
        }
        else
            byteSource.AddRange(Arrays.CopyOfRange(last, 0, klen % 32));
        return byteSource.ToArray();
    }

    public static byte[] Sm4DecryptCBC(byte[] keyBytes, byte[] cipher, byte[] iv, string algo)
    {
        if (keyBytes.Length != 16) throw new ArgumentException("err key length");
        if (cipher.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

        KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
        IBufferedCipher c = CipherUtilities.GetCipher(algo);
        if (iv == null) iv = ZeroIv(algo);
        c.Init(false, new ParametersWithIV(key, iv));
        return c.DoFinal(cipher);
    }

    public static byte[] Sm4EncryptCBC(byte[] keyBytes, byte[] plain, byte[] iv, string algo)
    {
        if (keyBytes.Length != 16) throw new ArgumentException("err key length");
        if (plain.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

        KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
        IBufferedCipher c = CipherUtilities.GetCipher(algo);
        if (iv == null) iv = ZeroIv(algo);
        c.Init(true, new ParametersWithIV(key, iv));
        return c.DoFinal(plain);
    }

    public static byte[] Sm4EncryptECB(byte[] keyBytes, byte[] plain, string algo)
    {
        if (keyBytes.Length != 16) throw new ArgumentException("err key length");
        //In the case of NoPadding, the verification data length needs to be a multiple of 16.
        if (plain.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

        KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
        IBufferedCipher c = CipherUtilities.GetCipher(algo);
        c.Init(true, key);
        return c.DoFinal(plain);
    }

    public static byte[] Sm4DecryptECB(byte[] keyBytes, byte[] cipher, string algo)
    {
        if (keyBytes.Length != 16) throw new ArgumentException("err key length");
        if (cipher.Length % 16 != 0 && algo.Contains("NoPadding")) throw new ArgumentException("err data length");

        KeyParameter key = ParameterUtilities.CreateKeyParameter("SM4", keyBytes);
        IBufferedCipher c = CipherUtilities.GetCipher(algo);
        c.Init(false, key);
        return c.DoFinal(cipher);
    }

    public const string SM4_ECB_NOPADDING = "SM4/ECB/NoPadding";
    public const string SM4_CBC_NOPADDING = "SM4/CBC/NoPadding";
    public const string SM4_ECB_PKCS7PADDING = "SM4/ECB/PKCS7Padding";
    public const string SM4_CBC_PKCS7PADDING = "SM4/CBC/PKCS7Padding";

    /**
     * cfcaOfficial websiteCSPSandboxExportofsm2Document
     * @param pem TwoOriginal numeral system text
     * @param pwd password
     * @return
     */

    public static Sm2Cert ReadSm2File(byte[] pem, string pwd)
    {
        Sm2Cert sm2Cert = new();

        Asn1Sequence asn1Sequence = (Asn1Sequence)Asn1Object.FromByteArray(pem);
        //            ASN1Integer asn1Integer = (ASN1Integer) asn1Sequence.getObjectAt(0); //version=1
        Asn1Sequence priSeq = (Asn1Sequence)asn1Sequence[1];//private key
        Asn1Sequence pubSeq = (Asn1Sequence)asn1Sequence[2];//public key and x509 cert

        //            ASN1ObjectIdentifier sm2DataOid = (ASN1ObjectIdentifier) priSeq.getObjectAt(0);
        //            ASN1ObjectIdentifier sm4AlgOid = (ASN1ObjectIdentifier) priSeq.getObjectAt(1);
        Asn1OctetString priKeyAsn1 = (Asn1OctetString)priSeq[2];
        byte[] key = KDF(System.Text.Encoding.UTF8.GetBytes(pwd), 32);
        byte[] priKeyD = Sm4DecryptCBC(Arrays.CopyOfRange(key, 16, 32),
                priKeyAsn1.GetOctets(),
                Arrays.CopyOfRange(key, 0, 16), SM4_CBC_PKCS7PADDING);
        sm2Cert.privateKey = GetPrivatekeyFromD(new BigInteger(1, priKeyD));
        //            log.Info(Hex.toHexString(priKeyD));

        //            ASN1ObjectIdentifier sm2DataOidPub = (ASN1ObjectIdentifier) pubSeq.getObjectAt(0);
        Asn1OctetString pubKeyX509 = (Asn1OctetString)pubSeq[1];
        X509Certificate x509 = new X509CertificateParser().ReadCertificate(pubKeyX509.GetOctets());
        sm2Cert.publicKey = x509.GetPublicKey();
        sm2Cert.certId = x509.SerialNumber.ToString(10); // This is converted to decimal. If you have any other base requirements, you can change it yourself.
        return sm2Cert;
    }

    /**
     *
     * @param cert
     * @return
     */

    public static Sm2Cert ReadSm2X509Cert(byte[] cert)
    {
        Sm2Cert sm2Cert = new();

        X509Certificate x509 = new X509CertificateParser().ReadCertificate(cert);
        sm2Cert.publicKey = x509.GetPublicKey();
        sm2Cert.certId = x509.SerialNumber.ToString(10); // This is converted to decimal. If you have any other base requirements, you can change it yourself.
        return sm2Cert;
    }

    public static byte[] ZeroIv(string algo)
    {
        IBufferedCipher cipher = CipherUtilities.GetCipher(algo);
        int blockSize = cipher.GetBlockSize();
        byte[] iv = new byte[blockSize];
        Arrays.Fill(iv, (byte)0);
        return iv;
    }
}