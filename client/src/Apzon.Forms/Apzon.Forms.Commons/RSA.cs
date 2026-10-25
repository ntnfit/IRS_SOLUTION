using System;

namespace Apzon.Forms.Commons;

internal class RSA
{
	internal static string PrivateKey = RequiredEnvironmentVariable("IRS_RSA_PRIVATE_KEY");
	internal static string PublicKey = RequiredEnvironmentVariable("IRS_RSA_PUBLIC_KEY");
	internal static string PasswordPhrase = RequiredEnvironmentVariable("IRS_RSA_PASSWORD_PHRASE");

	private static string RequiredEnvironmentVariable(string name)
	{
		string value = Environment.GetEnvironmentVariable(name);
		if (string.IsNullOrWhiteSpace(value))
		{
			throw new InvalidOperationException(name + " is required and must be supplied outside Git.");
		}
		return value;
	}
}
