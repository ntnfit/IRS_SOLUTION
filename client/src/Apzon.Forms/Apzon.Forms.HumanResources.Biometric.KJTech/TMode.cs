using System.IO;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class TMode : DayMaskSetting
{
	public int Valid;

	public int SHour;

	public int SMin;

	public int EHour;

	public int EMin;

	public int Mode;

	public static TMode GetFromStream(Stream stream)
	{
		TMode tMode = new TMode();
		BinaryReader binaryReader = new BinaryReader(stream);
		tMode.Valid = binaryReader.ReadInt32();
		tMode.DMask = binaryReader.ReadInt32();
		tMode.SHour = binaryReader.ReadInt32();
		tMode.SMin = binaryReader.ReadInt32();
		tMode.EHour = binaryReader.ReadInt32();
		tMode.EMin = binaryReader.ReadInt32();
		tMode.Mode = binaryReader.ReadInt32();
		return tMode;
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
		binaryWriter.Write(Mode);
	}
}
