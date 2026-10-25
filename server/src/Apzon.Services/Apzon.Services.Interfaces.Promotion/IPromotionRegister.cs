using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Promotion;

public interface IPromotionRegister
{
	HttpResult Save(DataSet model, string usercode);

	DataTable GetAttachment(DataTable dt, string usercode);
}
