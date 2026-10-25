using System.IO;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class AutoDoor : DayMaskSetting
{
	public int Valid;

	public int SHour;

	public int SMin;

	public int EHour;

	public int EMin;

	public static AutoDoor GetFromStream(Stream stream)
	{
		AutoDoor autoDoor = new AutoDoor();
		BinaryReader binaryReader = new BinaryReader(stream);
		autoDoor.Valid = binaryReader.ReadInt32();
		autoDoor.DMask = binaryReader.ReadInt32();
		autoDoor.SHour = binaryReader.ReadInt32();
		autoDoor.SMin = binaryReader.ReadInt32();
		autoDoor.EHour = binaryReader.ReadInt32();
		autoDoor.EMin = binaryReader.ReadInt32();
		return autoDoor;
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
