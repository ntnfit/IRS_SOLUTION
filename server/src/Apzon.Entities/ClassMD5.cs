using System;
using System.Security.Cryptography;
using System.Text;

public class ClassMD5
{
	public string GetMd5Hash(string input)
	{
		byte[] array = MD5.Create().ComputeHash(Encoding.Default.GetBytes(input));
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	public bool VerifyMd5Hash(string input, string hash)
	{
		string md5Hash = GetMd5Hash(input);
		StringComparer ordinalIgnoreCase = StringComparer.OrdinalIgnoreCase;
		if (ordinalIgnoreCase.Compare(md5Hash, hash) == 0)
		{
			return true;
		}
		return false;
	}
}
