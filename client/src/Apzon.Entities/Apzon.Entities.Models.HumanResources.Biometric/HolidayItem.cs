using System.IO;

namespace Apzon.Entities.Models.HumanResources.Biometric;

public class HolidayItem
{
	public int Valid;

	public int Month;

	public int Day;

	public int Number;

	public static HolidayItem GetFromStream(Stream stream)
	{
		HolidayItem holidayItem = new HolidayItem();
		BinaryReader binaryReader = new BinaryReader(stream);
		holidayItem.Valid = binaryReader.ReadInt32();
		holidayItem.Month = binaryReader.ReadInt32();
		holidayItem.Day = binaryReader.ReadInt32();
		holidayItem.Number = binaryReader.ReadInt32();
		return holidayItem;
	}

	public void Serialize(Stream stream)
	{
		BinaryWriter binaryWriter = new BinaryWriter(stream);
		binaryWriter.Write(Valid);
		binaryWriter.Write(Month);
		binaryWriter.Write(Day);
		binaryWriter.Write(Number);
	}
}
