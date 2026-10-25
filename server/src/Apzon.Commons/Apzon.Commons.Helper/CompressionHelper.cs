using System.IO;
using Ionic.Zlib;

namespace Apzon.Commons.Helper;

public class CompressionHelper
{
	public static byte[] DeflateByte(byte[] str)
	{
		if (str == null)
		{
			return null;
		}
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Compress, CompressionLevel.BestSpeed))
		{
			deflateStream.Write(str, 0, str.Length);
		}
		return memoryStream.ToArray();
	}

	public static byte[] UndeflateByte(byte[] str)
	{
		if (str == null)
		{
			return null;
		}
		using MemoryStream memoryStream = new MemoryStream();
		using (DeflateStream deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress, CompressionLevel.BestSpeed))
		{
			deflateStream.Write(str, 0, str.Length);
		}
		return memoryStream.ToArray();
	}
}
