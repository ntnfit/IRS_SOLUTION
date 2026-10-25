using System;
using System.IO;
using System.IO.Compression;

namespace Apzon.Commons.Helper;

public class JsoncCompressionHelper
{
	public static byte[] Compress(byte[] buffer)
	{
		MemoryStream memoryStream = new MemoryStream();
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, leaveOpen: true))
		{
			gZipStream.Write(buffer, 0, buffer.Length);
		}
		memoryStream.Position = 0L;
		byte[] array = new byte[memoryStream.Length];
		memoryStream.Read(array, 0, array.Length);
		byte[] array2 = new byte[array.Length + 4];
		Buffer.BlockCopy(array, 0, array2, 4, array.Length);
		Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, array2, 0, 4);
		return array2;
	}

	public static byte[] Decompress(byte[] gzBuffer)
	{
		using MemoryStream memoryStream = new MemoryStream();
		int num = BitConverter.ToInt32(gzBuffer, 0);
		memoryStream.Write(gzBuffer, 4, gzBuffer.Length - 4);
		byte[] array = new byte[num];
		memoryStream.Position = 0L;
		using (GZipStream gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
		{
			gZipStream.Read(array, 0, array.Length);
		}
		return array;
	}
}
