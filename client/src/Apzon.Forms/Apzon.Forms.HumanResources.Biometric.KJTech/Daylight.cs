using System.IO;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class Daylight
{
	public int Valid;

	public int Year;

	public int Month;

	public int Day;

	public int Hour;

	public int Min;

	public static Daylight GetFromStream(Stream stream)
	{
		Daylight daylight = new Daylight();
		BinaryReader binaryReader = new BinaryReader(stream);
		daylight.Valid = binaryReader.ReadInt32();
		daylight.Year = binaryReader.ReadInt32();
		daylight.Month = binaryReader.ReadInt32();
		daylight.Day = binaryReader.ReadInt32();
		daylight.Hour = binaryReader.ReadInt32();
		daylight.Min = binaryReader.ReadInt32();
		return daylight;
	}

	public void Serialize(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write(Valid);
		binaryWriter.Write(Year);
		binaryWriter.Write(Month);
		binaryWriter.Write(Day);
		binaryWriter.Write(Hour);
		binaryWriter.Write(Min);
	}
}
