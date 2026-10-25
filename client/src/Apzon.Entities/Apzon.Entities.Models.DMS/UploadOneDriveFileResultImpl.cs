using System;

namespace Apzon.Entities.Models.DMS;

public class UploadOneDriveFileResultImpl
{
	public string FileId { get; set; }

	public string FileName { get; set; }

	public string FileNameDrive { get; set; }

	public long Size { get; set; }

	public string Extension { get; set; }

	public string MimeType { get; set; }

	public string Path { get; set; }

	public string WebUrl { get; set; }

	public string LinkDownload { get; set; }

	public string LinkThumbnails { get; set; }

	public DateTime DateCreated { get; set; }

	public long TimestampCreated { get; set; }

	public string Domain { get; set; }

	public string PathThumbnail { get; set; }
}
