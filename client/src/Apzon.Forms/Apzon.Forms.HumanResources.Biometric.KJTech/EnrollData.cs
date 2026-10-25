using System.IO;

namespace Apzon.Forms.HumanResources.Biometric.KJTech;

public class EnrollData
{
	public int dwValidTempId;

	public int dwValidCard;

	public int dwValidFp1;

	public int dwValidFp2;

	public int dwTempIdNumber;

	public int dwManager;

	public int[] dwCardData = new int[2];

	public byte[] byFpData1 = new byte[Constants.FP_SIZE_COM];

	public byte[] byFpData2 = new byte[Constants.FP_SIZE_COM];

	public static EnrollData GetFromBinary(byte[] data)
	{
		if (data == null || data.Length < Constants.ENROLL_DATA_SIZE)
		{
			return null;
		}
		return GetFromStream(new MemoryStream(data));
	}

	public static EnrollData GetFromStream(MemoryStream stream)
	{
		EnrollData enrollData = new EnrollData();
		BinaryReader binaryReader = new BinaryReader(stream);
		enrollData.dwValidTempId = binaryReader.ReadInt32();
		enrollData.dwValidCard = binaryReader.ReadInt32();
		enrollData.dwValidFp1 = binaryReader.ReadInt32();
		enrollData.dwValidFp2 = binaryReader.ReadInt32();
		enrollData.dwTempIdNumber = binaryReader.ReadInt32();
		enrollData.dwManager = binaryReader.ReadInt32();
		enrollData.dwCardData[0] = binaryReader.ReadInt32();
		enrollData.dwCardData[1] = binaryReader.ReadInt32();
		enrollData.byFpData1 = binaryReader.ReadBytes(Constants.FP_SIZE_COM);
		enrollData.byFpData2 = binaryReader.ReadBytes(Constants.FP_SIZE_COM);
		return enrollData;
	}

	public byte[] Serialize()
	{
		MemoryStream memoryStream = new MemoryStream();
		BinaryWriter binaryWriter = new BinaryWriter(memoryStream);
		binaryWriter.Write(dwValidTempId);
		binaryWriter.Write(dwValidCard);
		binaryWriter.Write(dwValidFp1);
		binaryWriter.Write(dwValidFp2);
		binaryWriter.Write(dwTempIdNumber);
		binaryWriter.Write(dwManager);
		binaryWriter.Write(dwCardData[0]);
		binaryWriter.Write(dwCardData[1]);
		binaryWriter.Write(byFpData1);
		binaryWriter.Write(byFpData2);
		return memoryStream.ToArray();
	}
}
