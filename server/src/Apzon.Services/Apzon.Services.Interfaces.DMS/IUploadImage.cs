using System.Data;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;

namespace Apzon.Services.Interfaces.DMS;

public interface IUploadImage
{
	DataTable GetAvatar(string cardCode);

	HttpResult UploadImage(DataTable dt);

	DataSet GetImageByDate(string cardCode, string date);

	DataTable GetServerUpload(string ServerAddress, string DomainName, string DBName);

	DataTable GetAllAvatarStore(string userName);

	DataTable GetListAlbum();

	DataTable GetAvatarUser(string userName);

	DataTable GetFileByOjectType(string objectType, string objectCode);

	HttpResult UploadFiles(DataTable dt);

	DataTable GetImageList(DataTable dt);

	DataTable GetItemImageList(DataTable dt);

	ResponseResult GetStoreAlbumImage(string fromDate, string toDate, int userSign, string listCardCode, int pageIndex, string listAlbumId, string filterTypeAttach, string lstObjectType, int empId);
}
