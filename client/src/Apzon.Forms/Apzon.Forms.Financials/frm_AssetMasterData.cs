using Apzon.Forms.ItemsMaster;

namespace Apzon.Forms.Financials;

internal class frm_AssetMasterData : frm_ItemMasterData
{
	public frm_AssetMasterData()
	{
		Text = "Asset Master Data";
		IsAssetMasterData = true;
		FormId = "oAssetMasetData";
		gv_WareHouse.FormId = FormId;
		gv_WareHouse.ItemId = "OITW";
		gv_Udf.FormId = FormId;
		gv_Udf.ItemId = "OITW_Udf";
		gv_Properties.FormId = FormId;
		gv_Properties.ItemId = "OITW_Prop";
		ucAttachment1.FormId = FormId;
		ucAttachment1.ItemId = "OACT";
	}
}
