using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Forms.Sales;

namespace Apzon.Forms.Purchase;

public class frm_PurchaseBlanketAgreement : frm_SalesBlanketAgreement
{
	public frm_PurchaseBlanketAgreement()
	{
		InitComponents();
	}

	public void InitComponents()
	{
		FormObjectType = ApzObjectTypes.oBlanketAgreement;
		FormId = "oPurchaseBlanketAgreement";
		base.Text = "Purchase Blanket Agreement";
		_bpType = Enums.TypeCFL.Vendor;
	}
}
