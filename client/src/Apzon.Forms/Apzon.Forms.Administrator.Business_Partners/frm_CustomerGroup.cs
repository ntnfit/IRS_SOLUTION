using Apzon.Forms.Interfaces.BusinessPartner;

namespace Apzon.Forms.Administrator.Business_Partners;

public class frm_CustomerGroup : VirFormCustomerVendorGroup
{
	public frm_CustomerGroup()
	{
		FormId = (gvDoc.FormId = "oCustomerGroup");
		GroupType = "C";
	}
}
