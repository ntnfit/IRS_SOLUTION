using System.Text;

namespace Apzon.Commons.Helper;

public static class BytesHelper
{
	public static string ConvertByteToString(this byte[] source)
	{
		if (source == null)
		{
			return null;
		}
		return Encoding.UTF8.GetString(source);
	}
}
