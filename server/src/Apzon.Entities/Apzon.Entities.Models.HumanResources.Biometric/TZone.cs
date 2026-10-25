using System.IO;

namespace Apzon.Entities.Models.HumanResources.Biometric;

public class TZone : DayMaskSetting
{
	public int Valid;

	public int SHour;

	public int SMin;

	public int EHour;

	public int EMin;

	public static TZone GetFromStream(Stream stream)
	{
		BinaryReader binaryReader = new BinaryReader(stream);
		return new TZone
		{
			Valid = binaryReader.ReadInt32(),
			DMask = binaryReader.ReadInt32(),
			SHour = binaryReader.ReadInt32(),
			SMin = binaryReader.ReadInt32(),
			EHour = binaryReader.ReadInt32(),
			EMin = binaryReader.ReadInt32()
		};
	}

	public void Serialize(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write(Valid);
		binaryWriter.Write(DMask);
		binaryWriter.Write(SHour);
		binaryWriter.Write(SMin);
		binaryWriter.Write(EHour);
		binaryWriter.Write(EMin);
	}
}
