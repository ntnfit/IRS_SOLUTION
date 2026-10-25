using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IImageAlbum
{
	DataTable LoadData();

	HttpResult Saves(DataTable dt);

	string DeleteDocs(string id);
}
