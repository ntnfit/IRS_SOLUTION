using System.IO;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class AutoKey : DayMaskSetting
{
	public int Valid;

	public int SHour;

	public int SMin;

	public int EHour;

	public int EMin;

	public int FKey;

	public static AutoKey GetFromStream(Stream stream)
	{
		AutoKey autoKey = new AutoKey();
		BinaryReader binaryReader = new BinaryReader(stream);
		autoKey.Valid = binaryReader.ReadInt32();
		autoKey.DMask = binaryReader.ReadInt32();
		autoKey.SHour = binaryReader.ReadInt32();
		autoKey.SMin = binaryReader.ReadInt32();
		autoKey.EHour = binaryReader.ReadInt32();
		autoKey.EMin = binaryReader.ReadInt32();
		autoKey.FKey = binaryReader.ReadInt32();
		return autoKey;
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
		binaryWriter.Write(FKey);
	}
}
