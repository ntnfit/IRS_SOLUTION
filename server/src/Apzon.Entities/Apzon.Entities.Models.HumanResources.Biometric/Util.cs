using System.Net;

namespace Apzon.Entities.Models.HumanResources.Biometric;

public static class Util
{
	public static int pubIPAddrToLong(string txtIP)
	{
		byte[] addressBytes = IPAddress.Parse(txtIP).GetAddressBytes();
		return (addressBytes[0] << 24) | (addressBytes[1] << 16) | (addressBytes[2] << 8) | addressBytes[3];
	}

	public static string pubLongToIPAddr(long vValue)
	{
		byte[] array = new byte[4]
		{
			(byte)(vValue >> 24),
			(byte)(vValue >> 16),
			(byte)(vValue >> 8),
			(byte)vValue
		};
		return $"{array[0]:D}.{array[1]:D}.{array[2]:D}.{array[3]:D}";
	}
}
