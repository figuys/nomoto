using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using lenovo.mbg.service.common.log;
using lenovo.mbg.service.common.webservices.WebApiModel;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.X509;

namespace lenovo.mbg.service.common.webservices;

public class RsaHelper
{
	public static bool RSAPrivateKeyJava2DotNet(string privateKey, out string key)
	{
		bool result = true;
		try
		{
			RsaPrivateCrtKeyParameters rsaPrivateCrtKeyParameters = (RsaPrivateCrtKeyParameters)PrivateKeyFactory.CreateKey(Convert.FromBase64String(privateKey));
			key = $"<RSAKeyValue><Modulus>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.Modulus.ToByteArrayUnsigned())}</Modulus><Exponent>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.PublicExponent.ToByteArrayUnsigned())}</Exponent><P>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.P.ToByteArrayUnsigned())}</P><Q>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.Q.ToByteArrayUnsigned())}</Q><DP>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.DP.ToByteArrayUnsigned())}</DP><DQ>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.DQ.ToByteArrayUnsigned())}</DQ><InverseQ>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.QInv.ToByteArrayUnsigned())}</InverseQ><D>{Convert.ToBase64String(rsaPrivateCrtKeyParameters.Exponent.ToByteArrayUnsigned())}</D></RSAKeyValue>";
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("RsaHelper.RSAPrivateKeyJava2DotNet(string privateKey) occur an excception", exception);
			key = string.Empty;
		}
		return result;
	}

	public static bool RSAPrivateKeyDotNet2Java(string privateKey, out string key)
	{
		bool result = true;
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(privateKey);
			BigInteger modulus = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
			BigInteger publicExponent = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
			BigInteger privateExponent = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("D")[0].InnerText));
			BigInteger p = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("P")[0].InnerText));
			BigInteger q = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Q")[0].InnerText));
			BigInteger dP = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("DP")[0].InnerText));
			BigInteger dQ = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("DQ")[0].InnerText));
			BigInteger qInv = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("InverseQ")[0].InnerText));
			byte[] encoded = PrivateKeyInfoFactory.CreatePrivateKeyInfo(new RsaPrivateCrtKeyParameters(modulus, publicExponent, privateExponent, p, q, dP, dQ, qInv)).ToAsn1Object().GetEncoded();
			key = Convert.ToBase64String(encoded);
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("RsaHelper.RSAPrivateKeyDotNet2Java(string privateKey) occur an excception", exception);
			key = string.Empty;
		}
		return result;
	}

	public static bool RSAPublicKeyJava2DotNet(string publicKey, out string key)
	{
		bool result = true;
		try
		{
			RsaKeyParameters rsaKeyParameters = (RsaKeyParameters)PublicKeyFactory.CreateKey(Convert.FromBase64String(publicKey));
			key = $"<RSAKeyValue><Modulus>{Convert.ToBase64String(rsaKeyParameters.Modulus.ToByteArrayUnsigned())}</Modulus><Exponent>{Convert.ToBase64String(rsaKeyParameters.Exponent.ToByteArrayUnsigned())}</Exponent></RSAKeyValue>";
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("RsaHelper.RSAPublicKeyJava2DotNet(string publicKey) occur an excception", exception);
			key = string.Empty;
		}
		return result;
	}

	public static bool RSAPublicKeyDotNet2Java(string publicKey, out string key)
	{
		bool result = true;
		try
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(publicKey);
			BigInteger modulus = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Modulus")[0].InnerText));
			BigInteger exponent = new BigInteger(1, Convert.FromBase64String(xmlDocument.DocumentElement.GetElementsByTagName("Exponent")[0].InnerText));
			byte[] derEncoded = SubjectPublicKeyInfoFactory.CreateSubjectPublicKeyInfo(new RsaKeyParameters(isPrivate: false, modulus, exponent)).ToAsn1Object().GetDerEncoded();
			key = Convert.ToBase64String(derEncoded);
		}
		catch (Exception exception)
		{
			LogHelper.LogInstance.Error("RsaHelper.RSAPublicKeyDotNet2Java(string publicKey) occur an excception", exception);
			key = string.Empty;
		}
		return result;
	}

	public static RSAKey GenerateRsaKey()
	{
		RSAKey rSAKey = new RSAKey();
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSAKey.PrivateKey = rSACryptoServiceProvider.ToXmlString(includePrivateParameters: true);
		rSAKey.PublicKey = rSACryptoServiceProvider.ToXmlString(includePrivateParameters: false);
		return rSAKey;
	}

	public static string RSAEncrypt(string xmlPublicKey, string encryptString)
	{
		if (string.IsNullOrEmpty(encryptString))
		{
			return string.Empty;
		}
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(xmlPublicKey);
		byte[] bytes = Encoding.UTF8.GetBytes(encryptString);
		return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(bytes, fOAEP: false));
	}

	public static string RSAEncrypt(string xmlPublicKey, byte[] EncryptString)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(xmlPublicKey);
		return Convert.ToBase64String(rSACryptoServiceProvider.Encrypt(EncryptString, fOAEP: false));
	}

	public static string RSADecrypt(string xmlPrivateKey, string encryptString)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(xmlPrivateKey);
		byte[] rgb = Convert.FromBase64String(encryptString);
		byte[] bytes = rSACryptoServiceProvider.Decrypt(rgb, fOAEP: false);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string RSADecrypt(string xmlPrivateKey, byte[] encryptString)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(xmlPrivateKey);
		byte[] bytes = rSACryptoServiceProvider.Decrypt(encryptString, fOAEP: false);
		return Encoding.UTF8.GetString(bytes);
	}

	public static string RSADecryptByPublicKey(string xmlPublicKey, string encryptString)
	{
		if (string.IsNullOrEmpty(encryptString))
		{
			return string.Empty;
		}
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(xmlPublicKey);
		byte[] input = Convert.FromBase64String(encryptString);
		RsaKeyParameters rsaPublicKey = DotNetUtilities.GetRsaPublicKey(rSACryptoServiceProvider);
		IBufferedCipher cipher = CipherUtilities.GetCipher("RSA");
		cipher.Init(forEncryption: false, rsaPublicKey);
		byte[] bytes = cipher.DoFinal(input);
		return Encoding.UTF8.GetString(bytes);
	}

	public static bool SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref byte[] EncryptedSignatureData)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
		RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
		EncryptedSignatureData = rSAPKCS1SignatureFormatter.CreateSignature(HashbyteSignature);
		return true;
	}

	public static bool SignatureFormatter(string p_strKeyPrivate, byte[] HashbyteSignature, ref string m_strEncryptedSignatureData)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
		RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
		byte[] inArray = rSAPKCS1SignatureFormatter.CreateSignature(HashbyteSignature);
		m_strEncryptedSignatureData = Convert.ToBase64String(inArray);
		return true;
	}

	public static bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref byte[] EncryptedSignatureData)
	{
		byte[] rgbHash = Convert.FromBase64String(m_strHashbyteSignature);
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
		RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
		EncryptedSignatureData = rSAPKCS1SignatureFormatter.CreateSignature(rgbHash);
		return true;
	}

	public static bool SignatureFormatter(string p_strKeyPrivate, string m_strHashbyteSignature, ref string m_strEncryptedSignatureData)
	{
		byte[] rgbHash = Convert.FromBase64String(m_strHashbyteSignature);
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPrivate);
		RSAPKCS1SignatureFormatter rSAPKCS1SignatureFormatter = new RSAPKCS1SignatureFormatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureFormatter.SetHashAlgorithm("MD5");
		byte[] inArray = rSAPKCS1SignatureFormatter.CreateSignature(rgbHash);
		m_strEncryptedSignatureData = Convert.ToBase64String(inArray);
		return true;
	}

	public static bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, byte[] DeformatterData)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
		RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
		if (rSAPKCS1SignatureDeformatter.VerifySignature(HashbyteDeformatter, DeformatterData))
		{
			return true;
		}
		return false;
	}

	public static bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, byte[] DeformatterData)
	{
		byte[] rgbHash = Convert.FromBase64String(p_strHashbyteDeformatter);
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
		RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
		if (rSAPKCS1SignatureDeformatter.VerifySignature(rgbHash, DeformatterData))
		{
			return true;
		}
		return false;
	}

	public static bool SignatureDeformatter(string p_strKeyPublic, byte[] HashbyteDeformatter, string p_strDeformatterData)
	{
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
		RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
		byte[] rgbSignature = Convert.FromBase64String(p_strDeformatterData);
		if (rSAPKCS1SignatureDeformatter.VerifySignature(HashbyteDeformatter, rgbSignature))
		{
			return true;
		}
		return false;
	}

	public static bool SignatureDeformatter(string p_strKeyPublic, string p_strHashbyteDeformatter, string p_strDeformatterData)
	{
		byte[] rgbHash = Convert.FromBase64String(p_strHashbyteDeformatter);
		RSACryptoServiceProvider rSACryptoServiceProvider = new RSACryptoServiceProvider();
		rSACryptoServiceProvider.FromXmlString(p_strKeyPublic);
		RSAPKCS1SignatureDeformatter rSAPKCS1SignatureDeformatter = new RSAPKCS1SignatureDeformatter(rSACryptoServiceProvider);
		rSAPKCS1SignatureDeformatter.SetHashAlgorithm("MD5");
		byte[] rgbSignature = Convert.FromBase64String(p_strDeformatterData);
		if (rSAPKCS1SignatureDeformatter.VerifySignature(rgbHash, rgbSignature))
		{
			return true;
		}
		return false;
	}
}
