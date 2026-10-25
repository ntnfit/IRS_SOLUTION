using System.IO;

namespace Apzon.Entities.Models.HumanResources.Biometric;

public class NoActKey
{
	public int FKey1;

	public int FKey2;

	public int FKey3;

	public int FKey4;

	public void ReadFromBinary(byte[] data)
	{
		if (data == null || data.Length < Constants.DB_NOACTKEY_SIZE)
		{
			return;
		}
		using MemoryStream input = new MemoryStream(data);
		using BinaryReader binaryReader = new BinaryReader(input);
		FKey1 = binaryReader.ReadInt32();
		FKey2 = binaryReader.ReadInt32();
		FKey3 = binaryReader.ReadInt32();
		FKey4 = binaryReader.ReadInt32();
	}

	public byte[] Serialize()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(FKey1);
		binaryWriter.Write(FKey2);
		binaryWriter.Write(FKey3);
		binaryWriter.Write(FKey4);
		return memoryStream.ToArray();
	}
}
