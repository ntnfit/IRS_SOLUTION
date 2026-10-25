using System;

namespace Apzon.Services;

internal class RSA
{
	internal static string PrivateKey = RequiredEnvironmentVariable("IRS_RSA_PRIVATE_KEY");
	internal static string PublicKey = RequiredEnvironmentVariable("IRS_RSA_PUBLIC_KEY");

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
