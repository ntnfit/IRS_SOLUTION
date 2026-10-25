using Apzon.Forms.Interfaces.BusinessPartner;

namespace Apzon.Forms.Administrator.Business_Partners;

public class frm_VendorGroup : VirFormCustomerVendorGroup
{
	public frm_VendorGroup()
	{
		FormId = (gvDoc.FormId = "oVendorGroup");
		GroupType = "S";
	}
}
