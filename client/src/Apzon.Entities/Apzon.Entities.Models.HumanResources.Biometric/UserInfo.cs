using System.IO;

namespace Apzon.Entities.Models.HumanResources.Biometric;

public class UserInfo
{
	public int dwTzone1;

	public int dwTzone2;

	public int dwAccessMode;

	public int dwLimitTime;

	public static UserInfo GetFromByte(byte[] data)
	{
		if (data == null || data.Length < Constants.USER_INFO_SIZE)
		{
			return null;
		}
		return GetFromStream(new MemoryStream(data));
	}

	public static UserInfo GetFromStream(Stream stream)
	{
		UserInfo userInfo = new UserInfo();
		BinaryReader binaryReader = new BinaryReader(stream);
		userInfo.dwTzone1 = binaryReader.ReadInt32();
		userInfo.dwTzone2 = binaryReader.ReadInt32();
		userInfo.dwAccessMode = binaryReader.ReadInt32();
		userInfo.dwLimitTime = binaryReader.ReadInt32();
		return userInfo;
	}

	public byte[] Serialize()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(dwTzone1);
		binaryWriter.Write(dwTzone2);
		binaryWriter.Write(dwAccessMode);
		binaryWriter.Write(dwLimitTime);
		return memoryStream.ToArray();
	}
}
