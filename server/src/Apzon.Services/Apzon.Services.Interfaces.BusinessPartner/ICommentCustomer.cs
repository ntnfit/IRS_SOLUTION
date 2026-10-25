using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface ICommentCustomer
{
	DataTable GetDataCommentCustomer(int userSign, string relatedEnt, string relatedTyp);

	HttpResult Save(DataTable data);

	HttpResult Delete(DataTable data);
}
