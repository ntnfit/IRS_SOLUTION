using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IBusinessPartnerss : IService<BusinessPartnersImpl>
{
	DataTable Table();

	List<BusinessPartnersImpl> GetCustomer(string username = "");

	List<BusinessPartnersImpl> GetBp(string CardType, string username = "");

	List<BusinessPartnersImpl> GetVendor(string username = "");

	List<BusinessPartnersImpl> GetLead(string username = "");

	List<BusinessPartnersImpl> GetListContactPerson(string cardCode);

	List<BusinessPartnersImpl> GetTelePh_ByCotacPerson(string cardCode, string cntctCode);

	DataTable GetTel_ByContactPerson(string cardCode, string cntctCode);

	List<BusinessPartnersImpl> GetContactPerson_byCntctCode(string cardCode, string cntctCode);

	List<BusinessPartnersImpl> GetCardType_ByCardCode(string cardCode, string clgCode);

	DataTable ListBPByCardType(string CardType, string username, int BPLId);
}
