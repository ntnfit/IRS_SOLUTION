namespace Apzon.Entities.Models.DMS;

public class UploadOneDriveResultImpl
{
	public bool Success { get; set; }

	public string Message { get; set; }

	public UploadOneDriveFileResultImpl File { get; set; }
}
