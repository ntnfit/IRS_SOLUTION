using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;

namespace Apzon.HttpClientHelper;

public interface IFileNetworkManagerClient
{
	Task<HttpResult> SaveAttachment(DataTable dt);

	Task<string> UpLoadFile(string destFolder, bool isOverride, params string[] sourceFile);

	Task<string> DownLoadFile(string sourceFile, string destFolder, bool isOverride);

	Task<string> UpLoadFolder(string sourceFolder, string destFolder, bool isOverride);

	Task<string> DownLoadFolder(string sourceFolder, string destFolder, bool isOverride);

	Task DeleteFile(string fileDelete);

	Task DeleteFolder(string folderDelete);

	Task<bool> CheckExistsFile(string file);

	Task<bool> CheckExistsFolder(string folder, bool isFullPath);

	List<Common.ListItems> LoadReportModified(string folder = "Report");

	Task<UploadOneDriveResultImpl> UploadFileOneDriveResult(string filePath, string fileName = "");

	Task<Tuple<string, UploadOneDriveFileResultImpl>> UploadFileOneDrive(UploadOneDriveInputImpl input);
}
