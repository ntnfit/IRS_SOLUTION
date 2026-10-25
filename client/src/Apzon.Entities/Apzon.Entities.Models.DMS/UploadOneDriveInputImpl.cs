using Apzon.Commons;

namespace Apzon.Entities.Models.DMS;

public class UploadOneDriveInputImpl
{
	public string FilePath { get; set; }

	public string FileName { get; set; }

	public ApzObjectTypes ObjectTypes { get; set; }

	public string ObjectCode { get; set; }
}
