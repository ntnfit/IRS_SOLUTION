using System.IO;

namespace Apzon.Entities.Models.HumanResources.Biometric;

public class BellTime : DayMaskSetting
{
	public int Valid;

	public int SHour;

	public int SMin;

	public int Sec;

	public static BellTime GetFromStream(Stream stream)
	{
		BellTime bellTime = new BellTime();
		BinaryReader binaryReader = new BinaryReader(stream);
		bellTime.Valid = binaryReader.ReadInt32();
		bellTime.DMask = binaryReader.ReadInt32();
		bellTime.SHour = binaryReader.ReadInt32();
		bellTime.SMin = binaryReader.ReadInt32();
		bellTime.Sec = binaryReader.ReadInt32();
		return bellTime;
	}

	public void Serialize(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write(Valid);
		binaryWriter.Write(DMask);
		binaryWriter.Write(SHour);
		binaryWriter.Write(SMin);
		binaryWriter.Write(Sec);
	}
}
