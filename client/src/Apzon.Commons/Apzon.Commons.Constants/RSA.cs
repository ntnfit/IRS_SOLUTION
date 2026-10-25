using System;

namespace Apzon.Commons.Constants;

internal class RSA
{
	internal static int KEY_SIZE = 2048;

	internal static bool fOAEP = false;

	internal static string PrivateKey = RequiredEnvironmentVariable("IRS_RSA_PRIVATE_KEY");

	internal static string PublicKey = "<RSAKeyValue><Modulus>vLtugeMPaxyc+bmo2bmzimv+cp79GCM6dVBCyPzChQG67+cJuIs6Mg0yEUaJUHvsmpvufAnKhTLtbY690PQ1XWwl07ehp7xB9yryd9VF2f5fh8yE3xhTj0VkxFbPPnN8+biljlYptq6fCkH2PzwQ9AzbszXs5c9OksQR6DMOtN/zbmjg3rkDt8DN43BhYtVPuKdv2nfeuwaTtttJ6IAJXjs3rDRinhx3ZKv/OFYozT4DiAeic5bv1XKj8rf58yXFYNE4soHQVWMlvs56+ePdHQWU9UJXHX5WwoC+ACHpw+zsJKYfh6dgtBTHhqjwls/lV/wEE1E4l44deA7OcQTvJw==</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";

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
