using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Apzon.Commons;

public class SecurityData
{
	public static string Encrypt_Aes(string Base64Plaintext, string Base64Key_IV, out string Base64Result)
	{
		byte[] Result_bytes = null;
		return Encrypt_Aes(Base64Plaintext, Base64Key_IV, IsOutString: false, out Base64Result, out Result_bytes);
	}

	public static string Encrypt_Aes(string Base64Plaintext, string Base64Key_IV, out byte[] Result)
	{
		string Result_Base64string = null;
		return Encrypt_Aes(Base64Plaintext, Base64Key_IV, IsOutString: false, out Result_Base64string, out Result);
	}

	public static string Encrypt_Aes(byte[] Plaintext, string Base64Key_IV, out string Base64Result)
	{
		byte[] Result_bytes = null;
		return Encrypt_Aes(Plaintext, Base64Key_IV, IsOutString: true, out Base64Result, out Result_bytes);
	}

	public static string Encrypt_Aes(byte[] Plaintext, string Base64Key_IV, out byte[] Result)
	{
		string Result_Base64string = null;
		return Encrypt_Aes(Plaintext, Base64Key_IV, IsOutString: false, out Result_Base64string, out Result);
	}

	private static string Encrypt_Aes(object Plaintext, string Base64Key_IV, bool IsOutString, out string Result_Base64string, out byte[] Result_bytes)
	{
		string result = "";
		Result_Base64string = null;
		Result_bytes = null;
		if (Plaintext == null)
		{
			return "Plaintext is null";
		}
		if (Base64Key_IV == null || Base64Key_IV.Length <= 0)
		{
			return "Base64Key_IV is null or empty";
		}
		string[] array = Base64Key_IV.Split(':');
		if (array.Length != 2)
		{
			return "Base64Key_IV is not in correct format (<Key>:<IV>)";
		}
		try
		{
			Result_bytes = Encrypt_Aes(Plaintext, Convert.FromBase64String(array[0]), Convert.FromBase64String(array[1]));
			if (IsOutString)
			{
				Result_Base64string = Convert.ToBase64String(Result_bytes);
			}
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
		return result;
	}

	private static byte[] Encrypt_Aes(object Plaintext, byte[] Key, byte[] IV)
	{
		if (Plaintext == null)
		{
			throw new ArgumentNullException("Data");
		}
		if (Key == null || Key.Length == 0)
		{
			throw new ArgumentNullException("Key");
		}
		if (IV == null || IV.Length == 0)
		{
			throw new ArgumentNullException("IV");
		}
		using Aes aes = Aes.Create();
		aes.Key = Key;
		aes.IV = IV;
		ICryptoTransform transform = aes.CreateEncryptor(aes.Key, aes.IV);
		using MemoryStream memoryStream = new MemoryStream();
		using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
		if (Plaintext is byte[])
		{
			byte[] array = Plaintext as byte[];
			cryptoStream.Write(array, 0, array.Length);
			cryptoStream.Close();
		}
		else
		{
			using StreamWriter streamWriter = new StreamWriter(cryptoStream);
			streamWriter.Write(Plaintext);
		}
		return memoryStream.ToArray();
	}

	public static string Decrypt_Aes(string Base64CipherText, string Base64Key_IV, out string result)
	{
		byte[] result_bytes = null;
		return Decrypt_Aes(Base64CipherText, Base64Key_IV, isOutString: false, out result, out result_bytes);
	}

	public static string Decrypt_Aes(string Base64CipherText, string Base64Key_IV, out byte[] result)
	{
		string result_string = null;
		return Decrypt_Aes(Base64CipherText, Base64Key_IV, isOutString: false, out result_string, out result);
	}

	public static string Decrypt_Aes(byte[] CipherText, string Base64Key_IV, out string result)
	{
		byte[] result_bytes = null;
		return Decrypt_Aes(CipherText, Base64Key_IV, isOutString: true, out result, out result_bytes);
	}

	public static string Decrypt_Aes(byte[] CipherText, string Base64Key_IV, out byte[] result)
	{
		string result_string = null;
		return Decrypt_Aes(CipherText, Base64Key_IV, isOutString: false, out result_string, out result);
	}

	private static string Decrypt_Aes(object CipherText, string Base64Key_IV, bool isOutString, out string result_string, out byte[] result_bytes)
	{
		string result = "";
		result_string = null;
		result_bytes = null;
		if (CipherText == null)
		{
			return "CipherText is null";
		}
		if (Base64Key_IV == null || Base64Key_IV.Length <= 0)
		{
			return "Base64Key_IV is null or empty";
		}
		string[] array = Base64Key_IV.Split(':');
		if (array == null || array.Length != 2)
		{
			return "Base64Key_IV is invalid";
		}
		try
		{
			byte[] array2 = CipherText as byte[];
			if (array2 == null)
			{
				if (!(CipherText is string s))
				{
					return "CipherText is invalid";
				}
				array2 = Convert.FromBase64String(s);
			}
			DecryptStringFromBytes_Aes(array2, Convert.FromBase64String(array[0]), Convert.FromBase64String(array[1]), isOutString, out result_string, out result_bytes);
			return result;
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	private static void DecryptStringFromBytes_Aes(byte[] CipherText, byte[] Key, byte[] IV, bool isOutString, out string result_string, out byte[] result_bytes)
	{
		result_string = null;
		result_bytes = null;
		if (CipherText == null || CipherText.Length == 0)
		{
			throw new ArgumentNullException("cipherText");
		}
		if (Key == null || Key.Length == 0)
		{
			throw new ArgumentNullException("Key");
		}
		if (IV == null || IV.Length == 0)
		{
			throw new ArgumentNullException("IV");
		}
		using Aes aes = Aes.Create();
		aes.Key = Key;
		aes.IV = IV;
		ICryptoTransform transform = aes.CreateDecryptor(aes.Key, aes.IV);
		if (!isOutString)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
				cryptoStream.Write(CipherText, 0, CipherText.Length);
				cryptoStream.Close();
				result_bytes = memoryStream.ToArray();
				return;
			}
		}
		using MemoryStream stream = new MemoryStream(CipherText);
		using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
		using StreamReader streamReader = new StreamReader(stream2);
		result_string = streamReader.ReadToEnd();
	}

	public static string Encrypt(object enObject, string key, out string result)
	{
		return Encrypt(enObject, key, useHashing: true, out result);
	}

	public static string Encrypt(object enObject, string key, bool useHashing, out string result)
	{
		result = null;
		try
		{
			return Encrypt(Convertor.ObjectToBytes(enObject), key, useHashing, out result);
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	public static string Encrypt(byte[] bytes, string key, bool useHashing, out string result)
	{
		result = null;
		string result2 = "";
		try
		{
			byte[] key2;
			if (useHashing)
			{
				using MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				key2 = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
				mD5CryptoServiceProvider.Clear();
			}
			else
			{
				key2 = Encoding.UTF8.GetBytes(key);
			}
			using TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			tripleDESCryptoServiceProvider.Key = key2;
			tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
			tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			using ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateEncryptor();
			byte[] inArray = cryptoTransform.TransformFinalBlock(bytes, 0, bytes.Length);
			tripleDESCryptoServiceProvider.Clear();
			result = Convert.ToBase64String(inArray);
		}
		catch (Exception ex)
		{
			result2 = ex.ToString();
		}
		return result2;
	}

	public static string Decrypt(string cipherString, string key, out object result)
	{
		return Decrypt(cipherString, key, true, out result);
	}

	public static string Decrypt(string cipherString, string key, bool useHashing, out object result)
	{
		result = null;
		string text;
		try
		{
			byte[] result2 = null;
			text = Decrypt(cipherString, key, useHashing, out result2);
			if (text.Length > 0)
			{
				return text;
			}
			result = Convertor.BytesToObject(result2);
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return text;
	}

	public static string Decrypt(string cipherString, string key, bool useHashing, out byte[] result)
	{
		string result2 = "";
		result = null;
		try
		{
			byte[] key2;
			if (useHashing)
			{
				using MD5CryptoServiceProvider mD5CryptoServiceProvider = new MD5CryptoServiceProvider();
				key2 = mD5CryptoServiceProvider.ComputeHash(Encoding.UTF8.GetBytes(key));
				mD5CryptoServiceProvider.Clear();
			}
			else
			{
				key2 = Encoding.UTF8.GetBytes(key);
			}
			using TripleDESCryptoServiceProvider tripleDESCryptoServiceProvider = new TripleDESCryptoServiceProvider();
			tripleDESCryptoServiceProvider.Key = key2;
			tripleDESCryptoServiceProvider.Mode = CipherMode.ECB;
			tripleDESCryptoServiceProvider.Padding = PaddingMode.PKCS7;
			using ICryptoTransform cryptoTransform = tripleDESCryptoServiceProvider.CreateDecryptor();
			byte[] array = Convert.FromBase64String(cipherString);
			result = cryptoTransform.TransformFinalBlock(array, 0, array.Length);
			tripleDESCryptoServiceProvider.Clear();
		}
		catch (Exception ex)
		{
			result2 = ex.ToString();
		}
		return result2;
	}
}
