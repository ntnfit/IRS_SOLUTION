using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraLayout;
using DevExpress.XtraLayout.Utils;
using DevExpress.XtraTab;
using SAPbobsCOM;

namespace Apzon.Forms.ItemsMaster;

public class frm_Items : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient(ApzObjectTypes.oItems);

	public Dictionary<string, string> DicItemType = new Dictionary<string, string>
	{
		{ "I", "Items" },
		{ "L", "Labor" },
		{ "T", "Travel" }
	};

	public Dictionary<string, string> DicPriceType = new Dictionary<string, string>
	{
		{ "P", "Primary Currency" },
		{ "A1", "Addition Currency 1" },
		{ "A2", "Addition Currency 2" }
	};

	public Dictionary<string, string> DicManagerItem = new Dictionary<string, string>
	{
		{ "-", "None" },
		{ "Y", "Batch Number" },
		{ "N", "Series Number" }
	};

	public Dictionary<string, string> DicMngMethod = new Dictionary<string, string>
	{
		{ "A", "On Every Transaction" },
		{ "R", "On Release Only" }
	};

	public Dictionary<string, string> DicGlMethod = new Dictionary<string, string>
	{
		{ "W", "Warehouse" },
		{ "C", "Item Group" },
		{ "L", "Item Level" }
	};

	public Dictionary<string, string> DicPlanningSys = new Dictionary<string, string>
	{
		{ "M", "MRP" },
		{ "N", "None" }
	};

	public Dictionary<string, string> DicProcurementMethod = new Dictionary<string, string>
	{
		{ "B", "Buy" },
		{ "M", "Make" }
	};

	public Dictionary<string, string> DicIssueMethod = new Dictionary<string, string>
	{
		{ "B", "Backflush" },
		{ "M", "Manual" }
	};

	private IContainer components;

	private LayoutControl layoutControl1;

	private LayoutControlGroup layoutControlGroup1;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private LayoutControl layoutControl2;

	private LayoutControlGroup Root;

	private XtraTabPageEx xtraTabPage2;

	private XtraTabPageEx xtraTabPage3;

	private XtraTabPageEx xtraTabPage4;

	private XtraTabPageEx xtraTabPage5;

	private XtraTabPageEx xtraTabPage6;

	private XtraTabPageEx xtraTabPage7;

	private XtraTabPageEx xtraTabPage8;

	private XtraTabPageEx xtraTabPage9;

	private CheckEditEx SellItem;

	private CheckEditEx InvntItem;

	private CheckEditEx PrchseItem;

	private TextEditEx FrgnName;

	private TextEditEx ItemName;

	private EmptySpaceItem emptySpaceItem3;

	private LayoutControlItemEx layoutControlItem1;

	private LayoutControlItemEx layoutControlItem2;

	private LayoutControlItemEx layoutControlItem3;

	private LayoutControlItemEx layoutControlItem4;

	private LayoutControlItemEx layoutControlItem5;

	private LayoutControlItemEx layoutControlItem6;

	private LayoutControlItemEx layoutControlItem7;

	private LayoutControlItemEx layoutControlItem8;

	private LayoutControlItemEx layoutControlItem9;

	private LayoutControlItemEx layoutControlItem10;

	private EmptySpaceItem emptySpaceItem5;

	private LayoutControlItemEx layoutControlItem11;

	private LayoutControlItemEx layoutControlItem12;

	private EmptySpaceItem emptySpaceItem7;

	private LayoutControlItemEx layoutControlItem15;

	private LayoutControlItemEx layoutControlItem16;

	private EmptySpaceItem emptySpaceItem9;

	private EmptySpaceItem emptySpaceItem11;

	private LayoutControlItemEx layoutControlItem17;

	private RadioGroupEx rdg_Active;

	private Label label1;

	private TextEditEx SWW;

	private CheckEditEx NoDiscount;

	private LayoutControlItemEx layoutControlItem19;

	private LayoutControlItemEx layoutControlItem20;

	private LayoutControlItemEx layoutControlItem21;

	private LayoutControlItemEx layoutControlItem22;

	private LayoutControlItemEx layoutControlItem23;

	private LayoutControlItemEx layoutControlItem24;

	private LayoutControlItemEx layoutControlItem25;

	private LayoutControl layoutControl3;

	private LayoutControlGroup layoutControlGroup2;

	private TextEditEx textEdit38;

	private SimpleButtonEx simpleButton6;

	private SimpleButtonEx simpleButton5;

	private Label label3;

	private Label label2;

	private TextEditEx Rate;

	private TextEditEx TotalTax;

	private TextEditEx PurPackMsr;

	private TextEditEx SuppCatNum;

	private EmptySpaceItem emptySpaceItem22;

	private LayoutControlItemEx layoutControlItem29;

	private EmptySpaceItem emptySpaceItem24;

	private LayoutControlItemEx layoutControlItem30;

	private EmptySpaceItem emptySpaceItem25;

	private LayoutControlItemEx layoutControlItem31;

	private LayoutControlItemEx layoutControlItem32;

	private EmptySpaceItem emptySpaceItem26;

	private LayoutControlItemEx layoutControlItem33;

	private EmptySpaceItem emptySpaceItem30;

	private EmptySpaceItem emptySpaceItem31;

	private LayoutControlItemEx layoutControlItem35;

	private LayoutControlItemEx layoutControlItem36;

	private LayoutControlItemEx layoutControlItem37;

	private LayoutControlItemEx layoutControlItem38;

	private LayoutControlItemEx layoutControlItem39;

	private LayoutControlItemEx layoutControlItem34;

	private EmptySpaceItem emptySpaceItem23;

	private EmptySpaceItem emptySpaceItem28;

	private EmptySpaceItem emptySpaceItem29;

	private EmptySpaceItem emptySpaceItem32;

	private LayoutControlItemEx layoutControlItem40;

	private LayoutControlItemEx layoutControlItem41;

	private LayoutControlItemEx layoutControlItem42;

	private LayoutControlItemEx layoutControlItem43;

	private EmptySpaceItem emptySpaceItem33;

	private EmptySpaceItem emptySpaceItem34;

	private EmptySpaceItem emptySpaceItem35;

	private LayoutControlItemEx layoutControlItem45;

	private LayoutControlItemEx layoutControlItem44;

	private EmptySpaceItem emptySpaceItem36;

	private EmptySpaceItem emptySpaceItem40;

	private LayoutControlItemEx layoutControlItem46;

	private LayoutControlItemEx layoutControlItem47;

	private LayoutControlItemEx layoutControlItem48;

	private LayoutControlItemEx layoutControlItem49;

	private LayoutControlItemEx layoutControlItem50;

	private EmptySpaceItem emptySpaceItem27;

	private LayoutControlItemEx layoutControlItem51;

	private EmptySpaceItem emptySpaceItem37;

	private EmptySpaceItem emptySpaceItem38;

	private LayoutControlItemEx layoutControlItem52;

	private EmptySpaceItem emptySpaceItem39;

	private TextEditEx textEdit24;

	private LayoutControl layoutControl4;

	private LayoutControlGroup layoutControlGroup3;

	private LayoutControl layoutControl5;

	private LayoutControlGroup layoutControlGroup4;

	private LayoutControl layoutControl6;

	private LayoutControlGroup layoutControlGroup5;

	private LayoutControl layoutControl7;

	private LayoutControlGroup layoutControlGroup6;

	private LayoutControl layoutControl8;

	private LayoutControlGroup layoutControlGroup7;

	private LayoutControl layoutControl9;

	private LayoutControlGroup layoutControlGroup8;

	private LayoutControl layoutControl10;

	private LayoutControlGroup layoutControlGroup9;

	private Label label4;

	private TextEditEx Rate2;

	private TextEditEx SalPackMsr;

	private EmptySpaceItem emptySpaceItem41;

	private EmptySpaceItem emptySpaceItem42;

	private LayoutControlItemEx layoutControlItem53;

	private EmptySpaceItem emptySpaceItem43;

	private EmptySpaceItem emptySpaceItem44;

	private LayoutControlItemEx layoutControlItem54;

	private LayoutControlItemEx layoutControlItem55;

	private EmptySpaceItem emptySpaceItem45;

	private LayoutControlItemEx layoutControlItem56;

	private LayoutControlItemEx layoutControlItem57;

	private EmptySpaceItem emptySpaceItem46;

	private EmptySpaceItem emptySpaceItem48;

	private LayoutControlItemEx layoutControlItem58;

	private LayoutControlItemEx layoutControlItem59;

	private LayoutControlItemEx layoutControlItem60;

	private LayoutControlItemEx layoutControlItem61;

	private LayoutControlItemEx layoutControlItem62;

	private EmptySpaceItem emptySpaceItem49;

	private LayoutControlItemEx layoutControlItem63;

	private LayoutControlItemEx layoutControlItem64;

	private LayoutControlItemEx layoutControlItem65;

	private LayoutControlItemEx layoutControlItem66;

	private EmptySpaceItem emptySpaceItem50;

	private EmptySpaceItem emptySpaceItem51;

	private LayoutControlItemEx layoutControlItem67;

	private EmptySpaceItem emptySpaceItem52;

	private LayoutControlItemEx layoutControlItem68;

	private LayoutControlItemEx layoutControlItem69;

	private EmptySpaceItem emptySpaceItem47;

	private SimpleButtonEx btn_SetDefaultWhs;

	private GridControlEx grd_oitw;

	private GridViewEx gv_oitw;

	private Label label5;

	private CheckEditEx ByWh;

	private EmptySpaceItem emptySpaceItem53;

	private EmptySpaceItem emptySpaceItem54;

	private EmptySpaceItem emptySpaceItem55;

	private LayoutControlItemEx layoutControlItem70;

	private LayoutControlItemEx layoutControlItem71;

	private EmptySpaceItem emptySpaceItem56;

	private LayoutControlItemEx layoutControlItem72;

	private EmptySpaceItem emptySpaceItem57;

	private LayoutControlItemEx layoutControlItem73;

	private LayoutControlItemEx layoutControlItem74;

	private EmptySpaceItem emptySpaceItem58;

	private LayoutControlItemEx layoutControlItem75;

	private LayoutControlItemEx layoutControlItem76;

	private LayoutControlItemEx layoutControlItem77;

	private LayoutControlItemEx layoutControlItem78;

	private LayoutControlItemEx layoutControlItem79;

	private LayoutControlItemEx layoutControlItem80;

	private EmptySpaceItem emptySpaceItem62;

	private LayoutControlItemEx layoutControlItem81;

	private LayoutControlItemEx layoutControlItem82;

	private EmptySpaceItem emptySpaceItem64;

	private LayoutControlItemEx layoutControlItem83;

	private LayoutControlItemEx layoutControlItem84;

	private LayoutControlItemEx layoutControlItem85;

	private EmptySpaceItem emptySpaceItem65;

	private LayoutControlItemEx layoutControlItem86;

	private LayoutControlItemEx layoutControlItem87;

	private EmptySpaceItem emptySpaceItem66;

	private CheckEditEx Phantom;

	private EmptySpaceItem emptySpaceItem68;

	private EmptySpaceItem emptySpaceItem69;

	private LayoutControlItemEx layoutControlItem90;

	private LayoutControlItemEx layoutControlItem91;

	private EmptySpaceItem emptySpaceItem70;

	private LayoutControlItemEx layoutControlItem92;

	private LayoutControlItemEx layoutControlItem93;

	private LayoutControlItemEx layoutControlItem94;

	private SimpleButtonEx btn_ClearAllProperties;

	private SimpleButtonEx btn_SelectAllProperties;

	private GridControlEx grd_Properties;

	private GridViewEx gv_Properties;

	private LayoutControlItemEx layoutControlItem95;

	private EmptySpaceItem emptySpaceItem71;

	private LayoutControlItemEx layoutControlItem96;

	private LayoutControlItemEx layoutControlItem97;

	private MemoEditEx UserText;

	private LayoutControlItemEx layoutControlItem98;

	private EmptySpaceItem emptySpaceItem77;

	private SimpleButtonEx simpleButton13;

	private SimpleButtonEx simpleButton12;

	private SimpleButtonEx simpleButton11;

	private GridControlEx gridControl3;

	private GridViewEx gridView3;

	private LayoutControlItemEx layoutControlItem100;

	private LayoutControlItemEx layoutControlItem101;

	private LayoutControlItemEx layoutControlItem102;

	private LayoutControlItemEx layoutControlItem103;

	private Label label7;

	private Label label6;

	private LayoutControlItemEx layoutControlItem88;

	private LayoutControlItemEx layoutControlItem89;

	private CheckEditEx ManOutOnly;

	private LayoutControlItemEx layoutControlItem105;

	private LayoutControlItemEx layout_ManagementMethod;

	private LayoutControlGroup layoutControlGroup10;

	private LayoutControlItemEx layout_warrantyTemplate;

	private LayoutControlItemEx layoutControlItem117;

	private TextEditEx ItemCode;

	private LookUpEdit FirmCode;

	private LookUpEdit ShipType;

	private LookUpEdit ManBtchNum;

	private LookUpEdit MngMethod;

	private ButtonEditEx WarrntTmpl;

	private ButtonEditEx CardCode;

	private ButtonEditEx BuyUnitMsr;

	private CalcEditEx NumInBuy;

	private CalcEditEx PurPackUn;

	private LookUpEdit CstGrpCode;

	private LookUpEdit VatGroupPu;

	private CalcEditEx textEdit25;

	private CalcEditEx textEdit26;

	private CalcEditEx textEdit27;

	private CalcEditEx BVolume;

	private CalcEditEx textEdit29;

	private CalcEditEx PurFactor1;

	private CalcEditEx PurFactor2;

	private CalcEditEx PurFactor3;

	private CalcEditEx PurFactor4;

	private LookUpEdit VatGourpSa;

	private ButtonEditEx SalUnitMsr;

	private CalcEditEx NumInSale;

	private CalcEditEx SalPackUn;

	private CalcEditEx textEdit44;

	private CalcEditEx textEdit45;

	private CalcEditEx textEdit46;

	private CalcEditEx SVolume;

	private CalcEditEx textEdit48;

	private CalcEditEx SalFactor1;

	private CalcEditEx SalFactor2;

	private CalcEditEx SalFactor3;

	private CalcEditEx SalFactor4;

	private LookUpEdit textEdit53;

	private LookUpEdit GLMethod;

	private ButtonEditEx InvntryUom;

	private CalcEditEx textEdit57;

	private LookUpEdit EvalSystem;

	private CalcEditEx AvgPrice;

	private CalcEditEx ReorderQty;

	private CalcEditEx MinLevel;

	private CalcEditEx MaxLevel;

	private LookUpEdit PlaningSys;

	private LookUpEdit PrcrmntMtd;

	private LookUpEdit OrdrIntrvl;

	private CalcEditEx OrdrMulti;

	private CalcEditEx MinOrdrQty;

	private CalcEditEx LeadTime;

	private CalcEditEx ToleranDay;

	private LookUpEdit IssueMthd;

	private LookUpEdit TreeType;

	private CalcEditEx textEdit71;

	private CalcEditEx textEdit72;

	private LookUpEdit Series;

	private LookUpEdit ItemType;

	private LookUpEdit ItmsGrpCod;

	private LookUpEdit UgpEntry;

	private LookUpEdit ListName;

	private LookUpEdit look_PriceType;

	private LayoutControlItemEx layoutControlItem118;

	private EmptySpaceItem emptySpaceItem1;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private EmptySpaceItem emptySpaceItem10;

	private EmptySpaceItem emptySpaceItem15;

	private TextEditEx txt_ValidRemark;

	private DateEditEx date_ValidTo;

	private DateEditEx date_ValidFrom;

	private TextEditEx FrozenComm;

	private DateEditEx frozenTo;

	private DateEditEx frozenFrom;

	private TextEditEx ValidComm;

	private DateEditEx validTo;

	private DateEditEx validFrom;

	private LayoutControlItemEx layout_ActiveFrom;

	private LayoutControlItemEx layout_ActiveTo;

	private LayoutControlItemEx layout_ActiveRemark;

	private LayoutControlItemEx layout_InactiveFrom;

	private LayoutControlItemEx layout_InactiveTo;

	private LayoutControlItemEx layout_InactiveRemark;

	private EmptySpaceItem emptySpaceItem17;

	private LayoutControlItemEx layout_ActiveRangeFrom;

	private LayoutControlItemEx layout_ActiveRangeTo;

	private LayoutControlItemEx layout_ActiveRangeRemark;

	private TextEditEx txt_InvalidRemark;

	private DateEditEx date_InvalidTo;

	private DateEditEx date_InvalidFrom;

	private LayoutControlItemEx layout_InactiveRangeFrom;

	private LayoutControlItemEx layout_InactiveRangeTo;

	private LayoutControlItemEx layout_InactiveRangeRemark;

	private EmptySpaceItem emptySpaceItem2;

	private EmptySpaceItem emptySpaceItem18;

	private EmptySpaceItem emptySpaceItem19;

	private EmptySpaceItem emptySpaceItem16;

	private EmptySpaceItem emptySpaceItem20;

	private LayoutControlItemEx layoutControlItem13;

	private ButtonEditEx textEdit5;

	private ButtonEditEx CodeBars;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private RepositoryItemCheckEdit rep_Locked;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn23;

	private TextEditEx DfltWH;

	private LayoutControlItemEx layoutControlItem14;

	private XtraTabPageEx tab_UDF;

	private LayoutControl layoutControl_Udf;

	private LayoutControlGroup layout_Udf;

	private LayoutControl layoutControl12;

	private LayoutControlGroup layout_UdfRight;

	private LayoutControl layoutControl11;

	private LayoutControlGroup layout_UdfLeft;

	private LayoutControlItemEx layoutControlItem18;

	private LayoutControlItemEx layoutControlItem26;

	public string DrillDownItemCode { get; set; }

	public frm_Items()
	{
		InitializeComponent();
	}

	private async void frm_Items_Load(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			await LoadUdfControl();
			LoadItemType();
			await LoadUomGroup();
			await LoadPriceList();
			LoadPriceType();
			await LoadManufactures();
			await LoadShippingType();
			LoadManageItem();
			LoadManagementMethod();
			await LoadCustomGroup();
			await LoadVatGroupPu();
			await LoadVatGroupSa();
			LoadGlMethod();
			LoadPlanningSys();
			LoadProcurementMethod();
			LoadIssueMethod();
			await LoadProperties();
			await LoadSeries();
			if (!string.IsNullOrEmpty(DrillDownItemCode))
			{
				await LoadDataByCode(DrillDownItemCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_SelectAllProperties_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Properties.DataSourceTable;
			foreach (DataRow item in from DataRow row in dataSourceTable.Rows
				where row.IsExistsColumn("Checked")
				select row)
			{
				item["Checked"] = true;
			}
			dataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ClearAllProperties_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Properties.DataSourceTable;
			foreach (DataRow item in from DataRow row in dataSourceTable.Rows
				where row.IsExistsColumn("Checked")
				select row)
			{
				item["Checked"] = false;
			}
			dataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ManBtchNum_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(ManBtchNum.EditValue);
			LayoutControlItemEx layoutControlItemEx = layout_ManagementMethod;
			LayoutVisibility visibility = (layout_warrantyTemplate.Visibility = (text.Equals("-") ? LayoutVisibility.Never : LayoutVisibility.Always));
			layoutControlItemEx.Visibility = visibility;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MngMethod_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(MngMethod.EditValue);
			ManOutOnly.Visible = text.Equals("R");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rdg_Active_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(rdg_Active.SelectedIndex);
			LayoutControlItemEx layoutControlItemEx = layout_ActiveFrom;
			LayoutControlItemEx layoutControlItemEx2 = layout_ActiveTo;
			LayoutVisibility layoutVisibility = (layout_ActiveRemark.Visibility = ((!num.Equals(0)) ? LayoutVisibility.Never : LayoutVisibility.Always));
			LayoutVisibility visibility = (layoutControlItemEx2.Visibility = layoutVisibility);
			layoutControlItemEx.Visibility = visibility;
			LayoutControlItemEx layoutControlItemEx3 = layout_InactiveFrom;
			LayoutControlItemEx layoutControlItemEx4 = layout_InactiveTo;
			layoutVisibility = (layout_InactiveRemark.Visibility = ((!num.Equals(1)) ? LayoutVisibility.Never : LayoutVisibility.Always));
			visibility = (layoutControlItemEx4.Visibility = layoutVisibility);
			layoutControlItemEx3.Visibility = visibility;
			LayoutControlItemEx layoutControlItemEx5 = layout_ActiveRangeFrom;
			LayoutControlItemEx layoutControlItemEx6 = layout_ActiveRangeTo;
			layoutVisibility = (layout_ActiveRangeRemark.Visibility = ((!num.Equals(2)) ? LayoutVisibility.Never : LayoutVisibility.Always));
			visibility = (layoutControlItemEx6.Visibility = layoutVisibility);
			layoutControlItemEx5.Visibility = visibility;
			LayoutControlItemEx layoutControlItemEx7 = layout_InactiveRangeFrom;
			LayoutControlItemEx layoutControlItemEx8 = layout_InactiveRangeTo;
			layoutVisibility = (layout_InactiveRangeRemark.Visibility = ((!num.Equals(2)) ? LayoutVisibility.Never : LayoutVisibility.Always));
			visibility = (layoutControlItemEx8.Visibility = layoutVisibility);
			layoutControlItemEx7.Visibility = visibility;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SetDefaultWhs_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_oitw.FocusedRowHandle > 0)
			{
				string text = Function.ToString(gv_oitw.GetFocusedRowCellValue("WhsCode"));
				if (!string.IsNullOrEmpty(text))
				{
					DfltWH.EditValue = text;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_oitw_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (grd_oitw.DataSourceTable.IsNotNull())
			{
				string value = Function.ToString(DfltWH.EditValue).ToLower();
				if (!string.IsNullOrEmpty(value) && Function.ToString(gv_oitw.GetRowCellValue(e.RowHandle, "WhsCode")).ToLower().Equals(value))
				{
					e.Appearance.Font = new Font(e.Appearance.Font.FontFamily, e.Appearance.Font.Size, FontStyle.Bold);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void frm_Items_Shown(object sender, EventArgs e)
	{
		try
		{
			this.ResetData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			string text = btn_OK.Text;
			if (text.Equals(Apzon.Commons.Properties.Resources.ButtonOK))
			{
				Close();
			}
			else if (text.Equals(Apzon.Commons.Properties.Resources.ButtonFind))
			{
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				DataTable dataTable = await _httpClient.Items.GetItemStruct();
				if (dataTable.IsNotNull())
				{
					DataTable dataTable2 = dataTable.Clone();
					DataRow row = dataTable2.NewRow();
					this.GetDataSource(row);
					dataTable2.Rows.Add(row);
					DataTable dataTable3 = await _httpClient.Items.Find(dataTable2, GlobalData.UserName);
					if (dataTable3.IsNotNull())
					{
						if (dataTable3.Rows.Count > 1)
						{
							frm_FindItem frm = new frm_FindItem
							{
								MdiParent = MdiParent,
								Source = dataTable3
							};
							frm.Closing += async (object s, CancelEventArgs ev) =>
							{
								try
								{
									string text2 = Function.ToString(frm.ChoosedItem);
									if (!string.IsNullOrEmpty(text2))
									{
										await LoadDataByCode(text2);
									}
								}
								catch (Exception ex2)
								{
									Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								}
							};
							frm.Show();
						}
						else if (dataTable3.Rows.Count == 1)
						{
							string itemCode = Function.ToString(dataTable3.Rows[0]["ItemCode"]);
							await LoadDataByCode(itemCode);
						}
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.NoMatchingRecordFound, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		string currentCode = Function.ToString(ItemCode.EditValue);
		await Navigate(currentCode, 3);
	}

	public async Task MOVE_FIRST()
	{
		string currentCode = Function.ToString(ItemCode.EditValue);
		await Navigate(currentCode, 0);
	}

	public async Task MOVE_NEXT()
	{
		string currentCode = Function.ToString(ItemCode.EditValue);
		await Navigate(currentCode, 2);
	}

	public async Task MOVE_PREVIOUS()
	{
		string currentCode = Function.ToString(ItemCode.EditValue);
		await Navigate(currentCode, 1);
	}

	public async Task MODE_FIND()
	{
		try
		{
			this.SetFindMode();
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonFind;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual void REFRESH()
	{
		try
		{
			this.ResetData();
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Navigate(string currentCode, int type)
	{
		WinCommons._OpenProcessing();
		try
		{
			HttpResult httpResult = await _httpClient.Items.Navigate(type, currentCode, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			else
			{
				string itemCode = Function.ToString(httpResult.Content);
				await LoadDataByCode(itemCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadDataByCode(string itemCode)
	{
		try
		{
			DataSet dataSet = await _httpClient.Items.GetByCode(itemCode, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!dataSet.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			else if (dataSet != null && dataSet.Tables.Count > 0)
			{
				this.ResetData();
				BindingData(dataSet);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BindingData(DataSet ds)
	{
		try
		{
			if (ds == null || ds.Tables.Count <= 0)
			{
				return;
			}
			DataTable dataTable = ds.Tables[0];
			if (dataTable.IsNotNull())
			{
				BindingDataHeader(dataTable);
				DataTable dataSourceTable = grd_Properties.DataSourceTable;
				int i;
				for (i = 1; i <= 64; i++)
				{
					if (dataTable.ExistsColumn("QryGroup" + i))
					{
						bool flag = Function.ToString(dataTable.Rows[0]["QryGroup" + i]).ToUpper().Equals("Y");
						DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["ItmsTypCod"]).Equals(i));
						if (dataRow != null)
						{
							dataRow["Checked"] = flag;
						}
					}
				}
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			if (ds.Tables.Count > 1)
			{
				DataTable dataSource = ds.Tables[1];
				grd_oitw.DataSource = dataSource;
				grd_oitw.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindingDataHeader(DataTable dtItem)
	{
		DataRow dataRow = dtItem.Rows[0];
		this.SetDataSource(dataRow);
		bool flag = Function.ParseBool(dataRow["ValidFor"]);
		bool flag2 = Function.ParseBool(dataRow["FrozenFor"]);
		rdg_Active.SelectedIndex = ((!flag || flag2) ? ((!flag & flag2) ? 1 : 2) : 0);
		if (rdg_Active.SelectedIndex == 2)
		{
			date_ValidFrom.EditValue = Function.ParseDateTime(dataRow["validFrom"]);
			date_ValidTo.EditValue = Function.ParseDateTime(dataRow["validTo"]);
			txt_ValidRemark.EditValue = Function.ToString(dataRow["validComm"]);
		}
	}

	private void LoadItemType()
	{
		try
		{
			ItemType.SetDataSource(DicItemType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadItemType, "");
		}
	}

	private async Task LoadUomGroup()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadUomGroup();
			UgpEntry.SetDataSource(dt, "Code", "Entry", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadUomGroup, "");
		}
	}

	private async Task LoadPriceList()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadPriceList();
			ListName.SetDataSource(dt, "ListName", "ListNum", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadPriceList, "");
		}
	}

	private void LoadPriceType()
	{
		try
		{
			look_PriceType.SetDataSource(DicPriceType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadPriceType, "");
		}
	}

	private async Task LoadManufactures()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadManufactures();
			FirmCode.SetDataSource(dt, "FirmName", "FirmCode", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadMenufactures, "");
		}
	}

	private async Task LoadShippingType()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadShippingType();
			ShipType.SetDataSource(dt, "Name", "Code", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadShippingType, "");
		}
	}

	private void LoadManageItem()
	{
		try
		{
			ManBtchNum.SetDataSource(DicManagerItem);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadManageItemBy, "");
		}
	}

	private void LoadManagementMethod()
	{
		try
		{
			MngMethod.SetDataSource(DicMngMethod);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadManagementMethod, "");
		}
	}

	private async Task LoadCustomGroup()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadCustomGroup();
			CstGrpCode.SetDataSource(dt, "Name", "Code", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadCustomGroup, "");
		}
	}

	private async Task LoadVatGroupPu()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadVatGroup("I");
			VatGroupPu.SetDataSource(dt, "Name", "Code", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadVatGroupPurchase, "");
		}
	}

	private async Task LoadVatGroupSa()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadVatGroup("O");
			VatGourpSa.SetDataSource(dt, "Name", "Code", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadVatGroupSales, "");
		}
	}

	private void LoadGlMethod()
	{
		try
		{
			GLMethod.SetDataSource(DicGlMethod);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadGlMethod, "");
		}
	}

	private void LoadPlanningSys()
	{
		try
		{
			PlaningSys.SetDataSource(DicPlanningSys);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadPlanningMethod, "");
		}
	}

	private void LoadProcurementMethod()
	{
		try
		{
			PrcrmntMtd.SetDataSource(DicProcurementMethod);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadProcurementMethod, "");
		}
	}

	private void LoadIssueMethod()
	{
		try
		{
			IssueMthd.SetDataSource(DicIssueMethod);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadIssueMethod, "");
		}
	}

	private async Task LoadProperties()
	{
		try
		{
			DataTable dataSource = await _httpClient.Items.LoadProperties();
			grd_Properties.DataSource = dataSource;
			grd_Properties.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadPropeties, "");
		}
	}

	private async Task LoadSeries()
	{
		try
		{
			DataTable dt = await _httpClient.Items.LoadSeries(GlobalData.UserName);
			Series.SetDataSource(dt, "Name", "Code");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadPropeties, "");
		}
	}

	private async Task LoadUdfControl()
	{
		_ = 1;
		try
		{
			List<UserFieldsImpl> udfs = await _httpClient.Udf.GetListField("OITM");
			if (udfs != null && udfs.Count > 0)
			{
				tab_UDF.Visible = true;
				for (int i = 0; i < udfs.OrderBy((UserFieldsImpl t) => t.FieldID).ToList().Count; i++)
				{
					UserFieldsImpl item = udfs[i];
					LayoutControlItem layoutItem = ((i % 2 == 0) ? layout_UdfLeft.AddItem() : layout_UdfRight.AddItem());
					if (!string.IsNullOrEmpty(item.LinkedUDO))
					{
						ButtonEdit btn = new ButtonEdit();
						btn.Properties.Buttons[0].Kind = ButtonPredefines.Search;
						btn.Visible = true;
						btn.Name = "U_" + item.Name;
						btn.EditValue = item.DefaultValue;
						btn.Size = new Size(380, 20);
						btn.StyleController = layoutControl_Udf;
						layoutItem.Control = btn;
						string udoCode = item.LinkedUDO;
						btn.ButtonClick += (object s, ButtonPressedEventArgs e) =>
						{
							try
							{
								int docEntry = Function.ParseInt(btn.EditValue);
								frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry)
								{
									Text = item.Description
								};
								frm_UDO2.ShowDialog(this);
								if (frm_UDO2.ChoosedEntry != null)
								{
									btn.EditValue = frm_UDO2.ChoosedEntry;
								}
							}
							catch (Exception ex2)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
								WinCommons.ErrorLineNumber(ex2);
							}
						};
					}
					else if (item.ValidValues != null && item.ValidValues.Count > 0)
					{
						LookUpEdit lookUpEdit = new LookUpEdit
						{
							Visible = true,
							Name = "U_" + item.Name,
							EditValue = item.DefaultValue,
							Size = new Size(380, 20)
						};
						lookUpEdit.Properties.ValueMember = "Value";
						lookUpEdit.Properties.DisplayMember = "Description";
						lookUpEdit.Properties.NullText = "";
						switch (item.Type)
						{
						case BoFieldTypes.db_Alpha:
							lookUpEdit.Properties.DataSource = item.ValidValues;
							break;
						case BoFieldTypes.db_Numeric:
							lookUpEdit.Properties.DataSource = item.ValidValues.Select((ValidValuesMDImpl r) => new
							{
								Value = Function.ParseInt(r.Value),
								Description = r.Description
							});
							break;
						case BoFieldTypes.db_Float:
							lookUpEdit.Properties.DataSource = item.ValidValues.Select((ValidValuesMDImpl r) => new
							{
								Value = Function.ParseDecimal(r.Value),
								Description = r.Description
							});
							break;
						default:
							lookUpEdit.Properties.DataSource = item.ValidValues;
							break;
						}
						lookUpEdit.StyleController = layoutControl_Udf;
						layoutItem.Control = lookUpEdit;
					}
					else
					{
						switch (item.Type)
						{
						case BoFieldTypes.db_Alpha:
							if (!string.IsNullOrEmpty(item.LinkedTable))
							{
								List<Common.Table_Code> list = await _httpClient.Udf.GetLinkedTable(item.LinkedTable, GlobalData.UserName, GlobalData.DefaultBranch);
								list.Insert(0, new Common.Table_Code("", ""));
								LookUpEdit lookUpEdit2 = new LookUpEdit
								{
									Visible = true,
									Name = "U_" + item.Name,
									EditValue = item.DefaultValue,
									Size = new Size(380, 20)
								};
								lookUpEdit2.Properties.DataSource = list;
								lookUpEdit2.Properties.ValueMember = "Code";
								lookUpEdit2.Properties.DisplayMember = "Name";
								lookUpEdit2.Properties.NullText = "";
								lookUpEdit2.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
								lookUpEdit2.StyleController = layoutControl_Udf;
								layoutItem.Control = lookUpEdit2;
							}
							else
							{
								TextEdit control = new TextEdit
								{
									Visible = true,
									Name = "U_" + item.Name,
									EditValue = item.DefaultValue,
									Size = new Size(380, 20),
									StyleController = layoutControl_Udf
								};
								layoutItem.Control = control;
							}
							break;
						case BoFieldTypes.db_Date:
							if (item.SubType == BoFldSubTypes.st_Time)
							{
								TextEdit textEdit2 = new TextEdit
								{
									Visible = true,
									Name = "U_" + item.Name,
									EditValue = item.DefaultValue,
									Size = new Size(380, 20)
								};
								textEdit2.Properties.Mask.EditMask = "(0?\\\\d|1\\\\d|2[0-3])\\\\:[0-5]\\\\d";
								textEdit2.Properties.Mask.MaskType = MaskType.RegEx;
								textEdit2.StyleController = layoutControl_Udf;
								layoutItem.Control = textEdit2;
							}
							else
							{
								DateEdit dateEdit = new DateEdit
								{
									Visible = true,
									Name = "U_" + item.Name,
									EditValue = item.DefaultValue,
									Size = new Size(380, 20)
								};
								dateEdit.Properties.DisplayFormat.FormatType = FormatType.DateTime;
								dateEdit.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
								dateEdit.StyleController = layoutControl_Udf;
								layoutItem.Control = dateEdit;
								dateEdit.StyleController = layoutControl_Udf;
								layoutItem.Control = dateEdit;
							}
							break;
						case BoFieldTypes.db_Float:
						{
							CalcEdit calcEdit2 = new CalcEdit
							{
								Visible = true,
								Name = "U_" + item.Name,
								EditValue = item.DefaultValue,
								Size = new Size(380, 20)
							};
							calcEdit2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
							calcEdit2.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
							calcEdit2.StyleController = layoutControl_Udf;
							layoutItem.Control = calcEdit2;
							break;
						}
						case BoFieldTypes.db_Memo:
						{
							MemoExEdit control2 = new MemoExEdit
							{
								Visible = true,
								Name = "U_" + item.Name,
								EditValue = item.DefaultValue,
								Size = new Size(380, 20),
								StyleController = layoutControl_Udf
							};
							layoutItem.Control = control2;
							break;
						}
						case BoFieldTypes.db_Numeric:
							if (item.SubType == BoFldSubTypes.st_Time)
							{
								TextEdit textEdit = new TextEdit
								{
									Visible = true,
									Name = "U_" + item.Name,
									EditValue = item.DefaultValue,
									Size = new Size(380, 20),
									StyleController = layoutControl_Udf
								};
								textEdit.Properties.Mask.EditMask = "(0?\\\\d|1\\\\d|2[0-3])[0-5]\\\\d";
								textEdit.Properties.Mask.MaskType = MaskType.RegEx;
								layoutItem.Control = textEdit;
							}
							else
							{
								CalcEdit calcEdit = new CalcEdit
								{
									Visible = true,
									Name = "U_" + item.Name,
									EditValue = item.DefaultValue,
									Size = new Size(380, 20),
									StyleController = layoutControl_Udf
								};
								calcEdit.Properties.DisplayFormat.FormatType = FormatType.Numeric;
								calcEdit.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
								layoutItem.Control = calcEdit;
							}
							break;
						}
					}
					layoutItem.Text = item.Description;
					layoutItem.AppearanceItemCaption.Font = new Font(layoutItem.AppearanceItemCaption.Font.FontFamily, layoutItem.AppearanceItemCaption.Font.Size, FontStyle.Underline);
				}
			}
			else
			{
				tab_UDF.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_Items));
		this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
		this.ItemCode = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl2 = new DevExpress.XtraLayout.LayoutControl();
		this.txt_InvalidRemark = new Apzon.Controls.Editor.TextEditEx();
		this.date_InvalidTo = new Apzon.Controls.Editor.DateEditEx();
		this.date_InvalidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.txt_ValidRemark = new Apzon.Controls.Editor.TextEditEx();
		this.date_ValidTo = new Apzon.Controls.Editor.DateEditEx();
		this.date_ValidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.FrozenComm = new Apzon.Controls.Editor.TextEditEx();
		this.frozenTo = new Apzon.Controls.Editor.DateEditEx();
		this.frozenFrom = new Apzon.Controls.Editor.DateEditEx();
		this.ValidComm = new Apzon.Controls.Editor.TextEditEx();
		this.validTo = new Apzon.Controls.Editor.DateEditEx();
		this.validFrom = new Apzon.Controls.Editor.DateEditEx();
		this.ManOutOnly = new Apzon.Controls.Editor.CheckEditEx();
		this.rdg_Active = new Apzon.Controls.Editor.RadioGroupEx();
		this.label1 = new System.Windows.Forms.Label();
		this.SWW = new Apzon.Controls.Editor.TextEditEx();
		this.NoDiscount = new Apzon.Controls.Editor.CheckEditEx();
		this.FirmCode = new DevExpress.XtraEditors.LookUpEdit();
		this.ShipType = new DevExpress.XtraEditors.LookUpEdit();
		this.ManBtchNum = new DevExpress.XtraEditors.LookUpEdit();
		this.MngMethod = new DevExpress.XtraEditors.LookUpEdit();
		this.WarrntTmpl = new Apzon.Controls.Editor.ButtonEditEx();
		this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem19 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem20 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem21 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem22 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem23 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem24 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem25 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem105 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_ManagementMethod = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_warrantyTemplate = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem15 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layout_ActiveFrom = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_ActiveTo = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_InactiveFrom = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_InactiveTo = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_InactiveRemark = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem17 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layout_ActiveRangeFrom = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_ActiveRangeTo = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_ActiveRangeRemark = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_InactiveRangeFrom = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_InactiveRangeTo = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_InactiveRangeRemark = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layout_ActiveRemark = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem19 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem18 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem16 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem20 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl3 = new DevExpress.XtraLayout.LayoutControl();
		this.textEdit38 = new Apzon.Controls.Editor.TextEditEx();
		this.simpleButton6 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton5 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.label3 = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.Rate = new Apzon.Controls.Editor.TextEditEx();
		this.TotalTax = new Apzon.Controls.Editor.TextEditEx();
		this.PurPackMsr = new Apzon.Controls.Editor.TextEditEx();
		this.SuppCatNum = new Apzon.Controls.Editor.TextEditEx();
		this.CardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.BuyUnitMsr = new Apzon.Controls.Editor.ButtonEditEx();
		this.NumInBuy = new Apzon.Controls.Editor.CalcEditEx();
		this.PurPackUn = new Apzon.Controls.Editor.CalcEditEx();
		this.CstGrpCode = new DevExpress.XtraEditors.LookUpEdit();
		this.VatGroupPu = new DevExpress.XtraEditors.LookUpEdit();
		this.textEdit25 = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit26 = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit27 = new Apzon.Controls.Editor.CalcEditEx();
		this.BVolume = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit29 = new Apzon.Controls.Editor.CalcEditEx();
		this.PurFactor1 = new Apzon.Controls.Editor.CalcEditEx();
		this.PurFactor2 = new Apzon.Controls.Editor.CalcEditEx();
		this.PurFactor3 = new Apzon.Controls.Editor.CalcEditEx();
		this.PurFactor4 = new Apzon.Controls.Editor.CalcEditEx();
		this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem22 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem29 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem24 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem30 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem25 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem31 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem32 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem26 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem33 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem30 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem31 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem35 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem36 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem37 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem38 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem39 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem34 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem23 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem28 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem29 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem32 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem40 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem41 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem42 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem43 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem33 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem34 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem35 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem45 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem44 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem36 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem40 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem46 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem47 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem48 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem49 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem50 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem27 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem51 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem37 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem38 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem52 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem39 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl4 = new DevExpress.XtraLayout.LayoutControl();
		this.label4 = new System.Windows.Forms.Label();
		this.Rate2 = new Apzon.Controls.Editor.TextEditEx();
		this.SalPackMsr = new Apzon.Controls.Editor.TextEditEx();
		this.VatGourpSa = new DevExpress.XtraEditors.LookUpEdit();
		this.SalUnitMsr = new Apzon.Controls.Editor.ButtonEditEx();
		this.NumInSale = new Apzon.Controls.Editor.CalcEditEx();
		this.SalPackUn = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit44 = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit45 = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit46 = new Apzon.Controls.Editor.CalcEditEx();
		this.SVolume = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit48 = new Apzon.Controls.Editor.CalcEditEx();
		this.SalFactor1 = new Apzon.Controls.Editor.CalcEditEx();
		this.SalFactor2 = new Apzon.Controls.Editor.CalcEditEx();
		this.SalFactor3 = new Apzon.Controls.Editor.CalcEditEx();
		this.SalFactor4 = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit53 = new DevExpress.XtraEditors.LookUpEdit();
		this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem41 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem42 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem53 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem43 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem44 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem54 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem55 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem45 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem56 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem57 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem46 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem48 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem58 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem59 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem60 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem61 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem62 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem49 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem63 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem64 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem65 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem66 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem50 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem51 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem67 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem52 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem68 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem69 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem47 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.xtraTabPage4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl5 = new DevExpress.XtraLayout.LayoutControl();
		this.DfltWH = new Apzon.Controls.Editor.TextEditEx();
		this.btn_SetDefaultWhs = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_oitw = new Apzon.Controls.Grid.GridControlEx();
		this.gv_oitw = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Locked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.label5 = new System.Windows.Forms.Label();
		this.ByWh = new Apzon.Controls.Editor.CheckEditEx();
		this.GLMethod = new DevExpress.XtraEditors.LookUpEdit();
		this.InvntryUom = new Apzon.Controls.Editor.ButtonEditEx();
		this.textEdit57 = new Apzon.Controls.Editor.CalcEditEx();
		this.EvalSystem = new DevExpress.XtraEditors.LookUpEdit();
		this.AvgPrice = new Apzon.Controls.Editor.CalcEditEx();
		this.ReorderQty = new Apzon.Controls.Editor.CalcEditEx();
		this.MinLevel = new Apzon.Controls.Editor.CalcEditEx();
		this.MaxLevel = new Apzon.Controls.Editor.CalcEditEx();
		this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem53 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem54 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem55 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem70 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem71 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem56 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem72 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem57 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem73 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem74 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem58 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem75 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem76 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem77 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem78 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem79 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem80 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem117 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem14 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.xtraTabPage5 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl6 = new DevExpress.XtraLayout.LayoutControl();
		this.label7 = new System.Windows.Forms.Label();
		this.label6 = new System.Windows.Forms.Label();
		this.PlaningSys = new DevExpress.XtraEditors.LookUpEdit();
		this.PrcrmntMtd = new DevExpress.XtraEditors.LookUpEdit();
		this.OrdrIntrvl = new DevExpress.XtraEditors.LookUpEdit();
		this.OrdrMulti = new Apzon.Controls.Editor.CalcEditEx();
		this.MinOrdrQty = new Apzon.Controls.Editor.CalcEditEx();
		this.LeadTime = new Apzon.Controls.Editor.CalcEditEx();
		this.ToleranDay = new Apzon.Controls.Editor.CalcEditEx();
		this.layoutControlGroup5 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem62 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem81 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem82 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem64 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem83 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem84 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem85 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem65 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem86 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem87 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem66 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem88 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem89 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.xtraTabPage6 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl7 = new DevExpress.XtraLayout.LayoutControl();
		this.Phantom = new Apzon.Controls.Editor.CheckEditEx();
		this.IssueMthd = new DevExpress.XtraEditors.LookUpEdit();
		this.TreeType = new DevExpress.XtraEditors.LookUpEdit();
		this.textEdit71 = new Apzon.Controls.Editor.CalcEditEx();
		this.textEdit72 = new Apzon.Controls.Editor.CalcEditEx();
		this.layoutControlGroup6 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem68 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem69 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem90 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem91 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem70 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem92 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem93 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem94 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.xtraTabPage7 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl8 = new DevExpress.XtraLayout.LayoutControl();
		this.btn_ClearAllProperties = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SelectAllProperties = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Properties = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Properties = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.layoutControlGroup7 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem95 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem71 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem96 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem97 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem10 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.xtraTabPage8 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl9 = new DevExpress.XtraLayout.LayoutControl();
		this.UserText = new Apzon.Controls.Editor.MemoEditEx();
		this.layoutControlGroup8 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem98 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.xtraTabPage9 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl10 = new DevExpress.XtraLayout.LayoutControl();
		this.simpleButton13 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton12 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton11 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl3 = new Apzon.Controls.Grid.GridControlEx();
		this.gridView3 = new Apzon.Controls.Grid.GridViewEx();
		this.layoutControlGroup9 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem77 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem100 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem101 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem102 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem103 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.tab_UDF = new Apzon.Controls.Controls.XtraTabPageEx();
		this.layoutControl_Udf = new DevExpress.XtraLayout.LayoutControl();
		this.layoutControl12 = new DevExpress.XtraLayout.LayoutControl();
		this.layout_UdfRight = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControl11 = new DevExpress.XtraLayout.LayoutControl();
		this.layout_UdfLeft = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layout_Udf = new DevExpress.XtraLayout.LayoutControlGroup();
		this.layoutControlItem18 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem26 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.SellItem = new Apzon.Controls.Editor.CheckEditEx();
		this.InvntItem = new Apzon.Controls.Editor.CheckEditEx();
		this.PrchseItem = new Apzon.Controls.Editor.CheckEditEx();
		this.FrgnName = new Apzon.Controls.Editor.TextEditEx();
		this.ItemName = new Apzon.Controls.Editor.TextEditEx();
		this.Series = new DevExpress.XtraEditors.LookUpEdit();
		this.ItemType = new DevExpress.XtraEditors.LookUpEdit();
		this.ItmsGrpCod = new DevExpress.XtraEditors.LookUpEdit();
		this.UgpEntry = new DevExpress.XtraEditors.LookUpEdit();
		this.ListName = new DevExpress.XtraEditors.LookUpEdit();
		this.look_PriceType = new DevExpress.XtraEditors.LookUpEdit();
		this.textEdit5 = new Apzon.Controls.Editor.ButtonEditEx();
		this.CodeBars = new Apzon.Controls.Editor.ButtonEditEx();
		this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
		this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem1 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem2 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem3 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem4 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem5 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem6 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem7 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem8 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem9 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem10 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem5 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem11 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem12 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem7 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem15 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem16 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem9 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.emptySpaceItem11 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem17 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.layoutControlItem118 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
		this.layoutControlItem13 = new Apzon.Controls.Controls.LayoutControlItemEx();
		this.textEdit24 = new Apzon.Controls.Editor.TextEditEx();
		this.layoutControlGroup10 = new DevExpress.XtraLayout.LayoutControlGroup();
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).BeginInit();
		this.layoutControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).BeginInit();
		this.layoutControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_InvalidRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ValidRemark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FrozenComm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.frozenTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.frozenTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.frozenFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.frozenFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ValidComm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.validTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.validTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.validFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.validFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ManOutOnly.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SWW.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.NoDiscount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FirmCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ShipType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ManBtchNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MngMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.WarrntTmpl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Root).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem21).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem105).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ManagementMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_warrantyTemplate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRemark).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRangeFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRangeTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRangeRemark).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRangeFrom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRangeTo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRangeRemark).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRemark).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem19).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem20).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).BeginInit();
		this.layoutControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.textEdit38.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Rate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.TotalTax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PurPackMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SuppCatNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BuyUnitMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.NumInBuy.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PurPackUn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CstGrpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.VatGroupPu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit25.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit26.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit27.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BVolume.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit29.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor4.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem22).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem25).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem31).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem39).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem23).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem28).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem29).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem32).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem40).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem41).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem42).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem43).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem45).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem40).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem46).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem47).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem48).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem49).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem50).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem27).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem51).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem52).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem39).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).BeginInit();
		this.layoutControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Rate2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalPackMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.VatGourpSa.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalUnitMsr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.NumInSale.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalPackUn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit44.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit45.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit46.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SVolume.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit48.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor4.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit53.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem41).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem42).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem53).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem43).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem44).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem54).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem55).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem56).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem57).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem46).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem58).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem59).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem60).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem61).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem62).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem49).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem63).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem64).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem65).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem66).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem50).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem51).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem67).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem52).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem68).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem69).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem47).BeginInit();
		this.xtraTabPage4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl5).BeginInit();
		this.layoutControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.DfltWH.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_oitw).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_oitw).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Locked).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ByWh.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.GLMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.InvntryUom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit57.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.EvalSystem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.AvgPrice.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ReorderQty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MinLevel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MaxLevel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem53).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem54).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem55).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem70).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem71).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem56).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem72).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem73).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem74).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem58).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem75).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem76).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem77).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem78).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem79).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem80).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem117).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).BeginInit();
		this.xtraTabPage5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl6).BeginInit();
		this.layoutControl6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.PlaningSys.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PrcrmntMtd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.OrdrIntrvl.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.OrdrMulti.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.MinOrdrQty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LeadTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ToleranDay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem62).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem81).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem82).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem64).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem83).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem84).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem85).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem65).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem86).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem87).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem66).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem88).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem89).BeginInit();
		this.xtraTabPage6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl7).BeginInit();
		this.layoutControl7.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.Phantom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.IssueMthd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.TreeType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit71.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit72.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem68).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem69).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem90).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem91).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem92).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem93).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem94).BeginInit();
		this.xtraTabPage7.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl8).BeginInit();
		this.layoutControl8.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem95).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem96).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem97).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).BeginInit();
		this.xtraTabPage8.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl9).BeginInit();
		this.layoutControl9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.UserText.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem98).BeginInit();
		this.xtraTabPage9.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl10).BeginInit();
		this.layoutControl10.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridControl3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem77).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem100).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem101).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem102).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem103).BeginInit();
		this.tab_UDF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl_Udf).BeginInit();
		this.layoutControl_Udf.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.layoutControl12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_UdfRight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_UdfLeft).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.SellItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.InvntItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.PrchseItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.FrgnName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ItemName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Series.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ItemType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ItmsGrpCod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.UgpEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ListName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_PriceType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit5.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.CodeBars.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem15).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem17).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem118).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit24.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).BeginInit();
		base.SuspendLayout();
		this.layoutControl1.Controls.Add(this.ItemCode);
		this.layoutControl1.Controls.Add(this.btn_Cancel);
		this.layoutControl1.Controls.Add(this.btn_OK);
		this.layoutControl1.Controls.Add(this.xtraTabControl1);
		this.layoutControl1.Controls.Add(this.SellItem);
		this.layoutControl1.Controls.Add(this.InvntItem);
		this.layoutControl1.Controls.Add(this.PrchseItem);
		this.layoutControl1.Controls.Add(this.FrgnName);
		this.layoutControl1.Controls.Add(this.ItemName);
		this.layoutControl1.Controls.Add(this.Series);
		this.layoutControl1.Controls.Add(this.ItemType);
		this.layoutControl1.Controls.Add(this.ItmsGrpCod);
		this.layoutControl1.Controls.Add(this.UgpEntry);
		this.layoutControl1.Controls.Add(this.ListName);
		this.layoutControl1.Controls.Add(this.look_PriceType);
		this.layoutControl1.Controls.Add(this.textEdit5);
		this.layoutControl1.Controls.Add(this.CodeBars);
		this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl1.Location = new System.Drawing.Point(0, 0);
		this.layoutControl1.Name = "layoutControl1";
		this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(513, 409, 236, 350);
		this.layoutControl1.Root = this.layoutControlGroup1;
		this.layoutControl1.Size = new System.Drawing.Size(1090, 660);
		this.layoutControl1.TabIndex = 0;
		this.layoutControl1.Text = "layoutControl1";
		this.ItemCode.FieldName = null;
		this.ItemCode.IsSearchControl = false;
		this.ItemCode.Location = new System.Drawing.Point(216, 12);
		this.ItemCode.Name = "ItemCode";
		this.ItemCode.Size = new System.Drawing.Size(376, 20);
		this.ItemCode.StyleController = this.layoutControl1;
		this.ItemCode.TabIndex = 22;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(131, 626);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(109, 22);
		this.btn_Cancel.StyleController = this.layoutControl1;
		this.btn_Cancel.TabIndex = 20;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 626);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(115, 22);
		this.btn_OK.StyleController = this.layoutControl1;
		this.btn_OK.TabIndex = 19;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(12, 180);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(1066, 442);
		this.xtraTabControl1.TabIndex = 18;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[10] { this.xtraTabPage1, this.xtraTabPage2, this.xtraTabPage3, this.xtraTabPage4, this.xtraTabPage5, this.xtraTabPage6, this.xtraTabPage7, this.xtraTabPage8, this.xtraTabPage9, this.tab_UDF });
		this.xtraTabPage1.Controls.Add(this.layoutControl2);
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "General";
		this.layoutControl2.Controls.Add(this.txt_InvalidRemark);
		this.layoutControl2.Controls.Add(this.date_InvalidTo);
		this.layoutControl2.Controls.Add(this.date_InvalidFrom);
		this.layoutControl2.Controls.Add(this.txt_ValidRemark);
		this.layoutControl2.Controls.Add(this.date_ValidTo);
		this.layoutControl2.Controls.Add(this.date_ValidFrom);
		this.layoutControl2.Controls.Add(this.FrozenComm);
		this.layoutControl2.Controls.Add(this.frozenTo);
		this.layoutControl2.Controls.Add(this.frozenFrom);
		this.layoutControl2.Controls.Add(this.ValidComm);
		this.layoutControl2.Controls.Add(this.validTo);
		this.layoutControl2.Controls.Add(this.validFrom);
		this.layoutControl2.Controls.Add(this.ManOutOnly);
		this.layoutControl2.Controls.Add(this.rdg_Active);
		this.layoutControl2.Controls.Add(this.label1);
		this.layoutControl2.Controls.Add(this.SWW);
		this.layoutControl2.Controls.Add(this.NoDiscount);
		this.layoutControl2.Controls.Add(this.FirmCode);
		this.layoutControl2.Controls.Add(this.ShipType);
		this.layoutControl2.Controls.Add(this.ManBtchNum);
		this.layoutControl2.Controls.Add(this.MngMethod);
		this.layoutControl2.Controls.Add(this.WarrntTmpl);
		this.layoutControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl2.Location = new System.Drawing.Point(0, 0);
		this.layoutControl2.Name = "layoutControl2";
		this.layoutControl2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(540, 418, 250, 350);
		this.layoutControl2.Root = this.Root;
		this.layoutControl2.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl2.TabIndex = 0;
		this.layoutControl2.Text = "layoutControl2";
		this.txt_InvalidRemark.FieldName = null;
		this.txt_InvalidRemark.IsSearchControl = false;
		this.txt_InvalidRemark.Location = new System.Drawing.Point(490, 318);
		this.txt_InvalidRemark.Name = "txt_InvalidRemark";
		this.txt_InvalidRemark.Size = new System.Drawing.Size(229, 20);
		this.txt_InvalidRemark.StyleController = this.layoutControl2;
		this.txt_InvalidRemark.TabIndex = 38;
		this.date_InvalidTo.EditValue = null;
		this.date_InvalidTo.FieldName = null;
		this.date_InvalidTo.Location = new System.Drawing.Point(332, 318);
		this.date_InvalidTo.Name = "date_InvalidTo";
		this.date_InvalidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_InvalidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_InvalidTo.Properties.IsSearchControl = false;
		this.date_InvalidTo.Size = new System.Drawing.Size(99, 20);
		this.date_InvalidTo.StyleController = this.layoutControl2;
		this.date_InvalidTo.TabIndex = 37;
		this.date_InvalidFrom.EditValue = null;
		this.date_InvalidFrom.FieldName = null;
		this.date_InvalidFrom.Location = new System.Drawing.Point(170, 318);
		this.date_InvalidFrom.Name = "date_InvalidFrom";
		this.date_InvalidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_InvalidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_InvalidFrom.Properties.IsSearchControl = false;
		this.date_InvalidFrom.Size = new System.Drawing.Size(103, 20);
		this.date_InvalidFrom.StyleController = this.layoutControl2;
		this.date_InvalidFrom.TabIndex = 36;
		this.txt_ValidRemark.FieldName = null;
		this.txt_ValidRemark.IsSearchControl = false;
		this.txt_ValidRemark.Location = new System.Drawing.Point(490, 294);
		this.txt_ValidRemark.Name = "txt_ValidRemark";
		this.txt_ValidRemark.Size = new System.Drawing.Size(229, 20);
		this.txt_ValidRemark.StyleController = this.layoutControl2;
		this.txt_ValidRemark.TabIndex = 35;
		this.date_ValidTo.EditValue = null;
		this.date_ValidTo.FieldName = null;
		this.date_ValidTo.Location = new System.Drawing.Point(332, 294);
		this.date_ValidTo.Name = "date_ValidTo";
		this.date_ValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.IsSearchControl = false;
		this.date_ValidTo.Size = new System.Drawing.Size(99, 20);
		this.date_ValidTo.StyleController = this.layoutControl2;
		this.date_ValidTo.TabIndex = 34;
		this.date_ValidFrom.EditValue = null;
		this.date_ValidFrom.FieldName = null;
		this.date_ValidFrom.Location = new System.Drawing.Point(170, 294);
		this.date_ValidFrom.Name = "date_ValidFrom";
		this.date_ValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.IsSearchControl = false;
		this.date_ValidFrom.Size = new System.Drawing.Size(103, 20);
		this.date_ValidFrom.StyleController = this.layoutControl2;
		this.date_ValidFrom.TabIndex = 33;
		this.FrozenComm.FieldName = null;
		this.FrozenComm.IsSearchControl = false;
		this.FrozenComm.Location = new System.Drawing.Point(490, 246);
		this.FrozenComm.Name = "FrozenComm";
		this.FrozenComm.Size = new System.Drawing.Size(229, 20);
		this.FrozenComm.StyleController = this.layoutControl2;
		this.FrozenComm.TabIndex = 30;
		this.frozenTo.EditValue = null;
		this.frozenTo.FieldName = null;
		this.frozenTo.Location = new System.Drawing.Point(327, 246);
		this.frozenTo.Name = "frozenTo";
		this.frozenTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.frozenTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.frozenTo.Properties.IsSearchControl = false;
		this.frozenTo.Size = new System.Drawing.Size(104, 20);
		this.frozenTo.StyleController = this.layoutControl2;
		this.frozenTo.TabIndex = 29;
		this.frozenFrom.EditValue = null;
		this.frozenFrom.FieldName = null;
		this.frozenFrom.Location = new System.Drawing.Point(170, 246);
		this.frozenFrom.Name = "frozenFrom";
		this.frozenFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.frozenFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.frozenFrom.Properties.IsSearchControl = false;
		this.frozenFrom.Size = new System.Drawing.Size(98, 20);
		this.frozenFrom.StyleController = this.layoutControl2;
		this.frozenFrom.TabIndex = 28;
		this.ValidComm.FieldName = null;
		this.ValidComm.IsSearchControl = false;
		this.ValidComm.Location = new System.Drawing.Point(490, 222);
		this.ValidComm.Name = "ValidComm";
		this.ValidComm.Size = new System.Drawing.Size(229, 20);
		this.ValidComm.StyleController = this.layoutControl2;
		this.ValidComm.TabIndex = 27;
		this.validTo.EditValue = null;
		this.validTo.FieldName = null;
		this.validTo.Location = new System.Drawing.Point(327, 222);
		this.validTo.Name = "validTo";
		this.validTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.validTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.validTo.Properties.IsSearchControl = false;
		this.validTo.Size = new System.Drawing.Size(104, 20);
		this.validTo.StyleController = this.layoutControl2;
		this.validTo.TabIndex = 26;
		this.validFrom.EditValue = null;
		this.validFrom.FieldName = null;
		this.validFrom.Location = new System.Drawing.Point(170, 222);
		this.validFrom.Name = "validFrom";
		this.validFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.validFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.validFrom.Properties.IsSearchControl = false;
		this.validFrom.Size = new System.Drawing.Size(98, 20);
		this.validFrom.StyleController = this.layoutControl2;
		this.validFrom.TabIndex = 25;
		this.ManOutOnly.FieldName = null;
		this.ManOutOnly.IsSearchControl = false;
		this.ManOutOnly.Location = new System.Drawing.Point(12, 189);
		this.ManOutOnly.Name = "ManOutOnly";
		this.ManOutOnly.Properties.Caption = "Automatic Serial Number Creation on Receipt";
		this.ManOutOnly.Size = new System.Drawing.Size(452, 19);
		this.ManOutOnly.StyleController = this.layoutControl2;
		this.ManOutOnly.TabIndex = 14;
		this.ManOutOnly.Visible = false;
		this.rdg_Active.FieldName = null;
		this.rdg_Active.IsSearchControl = false;
		this.rdg_Active.Location = new System.Drawing.Point(12, 212);
		this.rdg_Active.Name = "rdg_Active";
		this.rdg_Active.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_Active.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_Active.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_Active.Properties.Columns = 1;
		this.rdg_Active.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Active"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Inactive"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Advanced")
		});
		this.rdg_Active.Size = new System.Drawing.Size(79, 77);
		this.rdg_Active.StyleController = this.layoutControl2;
		this.rdg_Active.TabIndex = 10;
		this.rdg_Active.SelectedIndexChanged += new System.EventHandler(this.rdg_Active_SelectedIndexChanged);
		this.label1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(12, 107);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(452, 20);
		this.label1.TabIndex = 8;
		this.label1.Text = "Serial and Batch Numbers";
		this.SWW.FieldName = null;
		this.SWW.IsSearchControl = false;
		this.SWW.Location = new System.Drawing.Point(116, 59);
		this.SWW.Name = "SWW";
		this.SWW.Size = new System.Drawing.Size(348, 20);
		this.SWW.StyleController = this.layoutControl2;
		this.SWW.TabIndex = 6;
		this.NoDiscount.FieldName = null;
		this.NoDiscount.IsSearchControl = false;
		this.NoDiscount.Location = new System.Drawing.Point(12, 12);
		this.NoDiscount.Name = "NoDiscount";
		this.NoDiscount.Properties.Caption = "Do Not Appl&y Discount Groups";
		this.NoDiscount.Properties.ValueChecked = "Y";
		this.NoDiscount.Properties.ValueGrayed = "N";
		this.NoDiscount.Properties.ValueUnchecked = "N";
		this.NoDiscount.Size = new System.Drawing.Size(452, 19);
		this.NoDiscount.StyleController = this.layoutControl2;
		this.NoDiscount.TabIndex = 4;
		this.FirmCode.Location = new System.Drawing.Point(116, 35);
		this.FirmCode.Name = "FirmCode";
		this.FirmCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.FirmCode.Properties.NullText = "";
		this.FirmCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.FirmCode.Size = new System.Drawing.Size(348, 20);
		this.FirmCode.StyleController = this.layoutControl2;
		this.FirmCode.TabIndex = 5;
		this.ShipType.Location = new System.Drawing.Point(116, 83);
		this.ShipType.Name = "ShipType";
		this.ShipType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ShipType.Properties.NullText = "";
		this.ShipType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.ShipType.Size = new System.Drawing.Size(348, 20);
		this.ShipType.StyleController = this.layoutControl2;
		this.ShipType.TabIndex = 7;
		this.ManBtchNum.Location = new System.Drawing.Point(116, 141);
		this.ManBtchNum.Name = "ManBtchNum";
		this.ManBtchNum.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ManBtchNum.Properties.NullText = "";
		this.ManBtchNum.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.ManBtchNum.Size = new System.Drawing.Size(348, 20);
		this.ManBtchNum.StyleController = this.layoutControl2;
		this.ManBtchNum.TabIndex = 9;
		this.ManBtchNum.EditValueChanged += new System.EventHandler(this.ManBtchNum_EditValueChanged);
		this.MngMethod.Location = new System.Drawing.Point(116, 165);
		this.MngMethod.Name = "MngMethod";
		this.MngMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.MngMethod.Properties.NullText = "";
		this.MngMethod.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.MngMethod.Size = new System.Drawing.Size(348, 20);
		this.MngMethod.StyleController = this.layoutControl2;
		this.MngMethod.TabIndex = 15;
		this.MngMethod.EditValueChanged += new System.EventHandler(this.MngMethod_EditValueChanged);
		this.WarrntTmpl.FieldName = null;
		this.WarrntTmpl.IsSearchControl = false;
		this.WarrntTmpl.Location = new System.Drawing.Point(572, 12);
		this.WarrntTmpl.Name = "WarrntTmpl";
		this.WarrntTmpl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.WarrntTmpl.Size = new System.Drawing.Size(147, 20);
		this.WarrntTmpl.StyleController = this.layoutControl2;
		this.WarrntTmpl.TabIndex = 24;
		this.Root.CustomizationFormText = "Root";
		this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.Root.GroupBordersVisible = false;
		this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[29]
		{
			this.layoutControlItem19, this.layoutControlItem20, this.layoutControlItem21, this.layoutControlItem22, this.layoutControlItem23, this.layoutControlItem24, this.layoutControlItem25, this.layoutControlItem105, this.layout_ManagementMethod, this.layout_warrantyTemplate,
			this.emptySpaceItem15, this.layout_ActiveFrom, this.layout_ActiveTo, this.layout_InactiveFrom, this.layout_InactiveTo, this.layout_InactiveRemark, this.emptySpaceItem17, this.layout_ActiveRangeFrom, this.layout_ActiveRangeTo, this.layout_ActiveRangeRemark,
			this.layout_InactiveRangeFrom, this.layout_InactiveRangeTo, this.layout_InactiveRangeRemark, this.layout_ActiveRemark, this.emptySpaceItem2, this.emptySpaceItem19, this.emptySpaceItem18, this.emptySpaceItem16, this.emptySpaceItem20
		});
		this.Root.Location = new System.Drawing.Point(0, 0);
		this.Root.Name = "Root";
		this.Root.Size = new System.Drawing.Size(1060, 414);
		this.Root.TextVisible = false;
		this.layoutControlItem19.Control = this.NoDiscount;
		this.layoutControlItem19.CustomizationFormText = "layoutControlItem19";
		this.layoutControlItem19.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem19.Name = "layoutControlItem19";
		this.layoutControlItem19.Size = new System.Drawing.Size(456, 23);
		this.layoutControlItem19.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem19.TextVisible = false;
		this.layoutControlItem20.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem20.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem20.Control = this.FirmCode;
		this.layoutControlItem20.CustomizationFormText = "Manufacturer";
		this.layoutControlItem20.Location = new System.Drawing.Point(0, 23);
		this.layoutControlItem20.MaxSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem20.MinSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem20.Name = "layoutControlItem20";
		this.layoutControlItem20.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem20.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem20.Text = "Manufacturer";
		this.layoutControlItem20.TextSize = new System.Drawing.Size(101, 13);
		this.layoutControlItem21.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem21.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem21.Control = this.SWW;
		this.layoutControlItem21.CustomizationFormText = "Additional Identifier";
		this.layoutControlItem21.Location = new System.Drawing.Point(0, 47);
		this.layoutControlItem21.MaxSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem21.MinSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem21.Name = "layoutControlItem21";
		this.layoutControlItem21.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem21.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem21.Text = "Additional Identifier";
		this.layoutControlItem21.TextSize = new System.Drawing.Size(101, 13);
		this.layoutControlItem22.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem22.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem22.Control = this.ShipType;
		this.layoutControlItem22.CustomizationFormText = "Shipping Type";
		this.layoutControlItem22.Location = new System.Drawing.Point(0, 71);
		this.layoutControlItem22.MaxSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem22.MinSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem22.Name = "layoutControlItem22";
		this.layoutControlItem22.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem22.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem22.Text = "Shipping Type";
		this.layoutControlItem22.TextSize = new System.Drawing.Size(101, 13);
		this.layoutControlItem23.Control = this.label1;
		this.layoutControlItem23.CustomizationFormText = "layoutControlItem23";
		this.layoutControlItem23.Location = new System.Drawing.Point(0, 95);
		this.layoutControlItem23.Name = "layoutControlItem23";
		this.layoutControlItem23.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem23.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem23.TextVisible = false;
		this.layoutControlItem24.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem24.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem24.Control = this.ManBtchNum;
		this.layoutControlItem24.CustomizationFormText = "Manage Item by";
		this.layoutControlItem24.Location = new System.Drawing.Point(0, 129);
		this.layoutControlItem24.MaxSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem24.MinSize = new System.Drawing.Size(456, 24);
		this.layoutControlItem24.Name = "layoutControlItem24";
		this.layoutControlItem24.Size = new System.Drawing.Size(456, 24);
		this.layoutControlItem24.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem24.Text = "Manage Item by";
		this.layoutControlItem24.TextSize = new System.Drawing.Size(101, 13);
		this.layoutControlItem25.Control = this.rdg_Active;
		this.layoutControlItem25.CustomizationFormText = "layoutControlItem25";
		this.layoutControlItem25.Location = new System.Drawing.Point(0, 200);
		this.layoutControlItem25.Name = "layoutControlItem25";
		this.layoutControlItem25.Size = new System.Drawing.Size(83, 81);
		this.layoutControlItem25.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem25.TextVisible = false;
		this.layoutControlItem105.Control = this.ManOutOnly;
		this.layoutControlItem105.CustomizationFormText = "layoutControlItem105";
		this.layoutControlItem105.Location = new System.Drawing.Point(0, 177);
		this.layoutControlItem105.Name = "layoutControlItem105";
		this.layoutControlItem105.Size = new System.Drawing.Size(456, 23);
		this.layoutControlItem105.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem105.TextVisible = false;
		this.layout_ManagementMethod.Control = this.MngMethod;
		this.layout_ManagementMethod.CustomizationFormText = "Management Method";
		this.layout_ManagementMethod.Location = new System.Drawing.Point(0, 153);
		this.layout_ManagementMethod.MaxSize = new System.Drawing.Size(456, 24);
		this.layout_ManagementMethod.MinSize = new System.Drawing.Size(456, 24);
		this.layout_ManagementMethod.Name = "layout_ManagementMethod";
		this.layout_ManagementMethod.Size = new System.Drawing.Size(456, 24);
		this.layout_ManagementMethod.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ManagementMethod.Text = "Management Method";
		this.layout_ManagementMethod.TextSize = new System.Drawing.Size(101, 13);
		this.layout_ManagementMethod.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_warrantyTemplate.Control = this.WarrntTmpl;
		this.layout_warrantyTemplate.CustomizationFormText = "Warranty Template";
		this.layout_warrantyTemplate.Location = new System.Drawing.Point(456, 0);
		this.layout_warrantyTemplate.Name = "layout_warrantyTemplate";
		this.layout_warrantyTemplate.Size = new System.Drawing.Size(255, 200);
		this.layout_warrantyTemplate.Text = " Warranty Template";
		this.layout_warrantyTemplate.TextSize = new System.Drawing.Size(101, 13);
		this.layout_warrantyTemplate.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem15.AllowHotTrack = false;
		this.emptySpaceItem15.CustomizationFormText = "emptySpaceItem15";
		this.emptySpaceItem15.Location = new System.Drawing.Point(0, 119);
		this.emptySpaceItem15.MaxSize = new System.Drawing.Size(456, 10);
		this.emptySpaceItem15.MinSize = new System.Drawing.Size(456, 10);
		this.emptySpaceItem15.Name = "emptySpaceItem15";
		this.emptySpaceItem15.Size = new System.Drawing.Size(456, 10);
		this.emptySpaceItem15.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem15.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ActiveFrom.Control = this.validFrom;
		this.layout_ActiveFrom.CustomizationFormText = "From";
		this.layout_ActiveFrom.Location = new System.Drawing.Point(83, 210);
		this.layout_ActiveFrom.MaxSize = new System.Drawing.Size(177, 24);
		this.layout_ActiveFrom.MinSize = new System.Drawing.Size(177, 24);
		this.layout_ActiveFrom.Name = "layout_ActiveFrom";
		this.layout_ActiveFrom.Size = new System.Drawing.Size(177, 24);
		this.layout_ActiveFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ActiveFrom.Text = "From";
		this.layout_ActiveFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_ActiveFrom.TextSize = new System.Drawing.Size(70, 13);
		this.layout_ActiveFrom.TextToControlDistance = 5;
		this.layout_ActiveTo.Control = this.validTo;
		this.layout_ActiveTo.CustomizationFormText = "To";
		this.layout_ActiveTo.Location = new System.Drawing.Point(260, 210);
		this.layout_ActiveTo.MaxSize = new System.Drawing.Size(163, 24);
		this.layout_ActiveTo.MinSize = new System.Drawing.Size(163, 24);
		this.layout_ActiveTo.Name = "layout_ActiveTo";
		this.layout_ActiveTo.Size = new System.Drawing.Size(163, 24);
		this.layout_ActiveTo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ActiveTo.Text = "To";
		this.layout_ActiveTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_ActiveTo.TextSize = new System.Drawing.Size(50, 13);
		this.layout_ActiveTo.TextToControlDistance = 5;
		this.layout_InactiveFrom.Control = this.frozenFrom;
		this.layout_InactiveFrom.CustomizationFormText = "From";
		this.layout_InactiveFrom.Location = new System.Drawing.Point(83, 234);
		this.layout_InactiveFrom.MaxSize = new System.Drawing.Size(177, 24);
		this.layout_InactiveFrom.MinSize = new System.Drawing.Size(177, 24);
		this.layout_InactiveFrom.Name = "layout_InactiveFrom";
		this.layout_InactiveFrom.Size = new System.Drawing.Size(177, 24);
		this.layout_InactiveFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_InactiveFrom.Text = "From";
		this.layout_InactiveFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_InactiveFrom.TextSize = new System.Drawing.Size(70, 13);
		this.layout_InactiveFrom.TextToControlDistance = 5;
		this.layout_InactiveFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_InactiveTo.Control = this.frozenTo;
		this.layout_InactiveTo.CustomizationFormText = "To";
		this.layout_InactiveTo.Location = new System.Drawing.Point(260, 234);
		this.layout_InactiveTo.MaxSize = new System.Drawing.Size(163, 24);
		this.layout_InactiveTo.MinSize = new System.Drawing.Size(163, 24);
		this.layout_InactiveTo.Name = "layout_InactiveTo";
		this.layout_InactiveTo.Size = new System.Drawing.Size(163, 24);
		this.layout_InactiveTo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_InactiveTo.Text = "To";
		this.layout_InactiveTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_InactiveTo.TextSize = new System.Drawing.Size(50, 13);
		this.layout_InactiveTo.TextToControlDistance = 5;
		this.layout_InactiveTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_InactiveRemark.Control = this.FrozenComm;
		this.layout_InactiveRemark.CustomizationFormText = "Remark";
		this.layout_InactiveRemark.Location = new System.Drawing.Point(423, 234);
		this.layout_InactiveRemark.MaxSize = new System.Drawing.Size(288, 24);
		this.layout_InactiveRemark.MinSize = new System.Drawing.Size(288, 24);
		this.layout_InactiveRemark.Name = "layout_InactiveRemark";
		this.layout_InactiveRemark.Size = new System.Drawing.Size(288, 24);
		this.layout_InactiveRemark.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_InactiveRemark.Text = "Remark";
		this.layout_InactiveRemark.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_InactiveRemark.TextSize = new System.Drawing.Size(50, 13);
		this.layout_InactiveRemark.TextToControlDistance = 5;
		this.layout_InactiveRemark.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.emptySpaceItem17.AllowHotTrack = false;
		this.emptySpaceItem17.CustomizationFormText = "emptySpaceItem17";
		this.emptySpaceItem17.Location = new System.Drawing.Point(63, 281);
		this.emptySpaceItem17.MaxSize = new System.Drawing.Size(20, 49);
		this.emptySpaceItem17.MinSize = new System.Drawing.Size(20, 49);
		this.emptySpaceItem17.Name = "emptySpaceItem17";
		this.emptySpaceItem17.Size = new System.Drawing.Size(20, 49);
		this.emptySpaceItem17.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem17.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.emptySpaceItem17.TextSize = new System.Drawing.Size(0, 0);
		this.layout_ActiveRangeFrom.Control = this.date_ValidFrom;
		this.layout_ActiveRangeFrom.CustomizationFormText = "From";
		this.layout_ActiveRangeFrom.Location = new System.Drawing.Point(83, 282);
		this.layout_ActiveRangeFrom.MaxSize = new System.Drawing.Size(182, 24);
		this.layout_ActiveRangeFrom.MinSize = new System.Drawing.Size(182, 24);
		this.layout_ActiveRangeFrom.Name = "layout_ActiveRangeFrom";
		this.layout_ActiveRangeFrom.Size = new System.Drawing.Size(182, 24);
		this.layout_ActiveRangeFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ActiveRangeFrom.Text = "Active From";
		this.layout_ActiveRangeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_ActiveRangeFrom.TextSize = new System.Drawing.Size(70, 13);
		this.layout_ActiveRangeFrom.TextToControlDistance = 5;
		this.layout_ActiveRangeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_ActiveRangeTo.Control = this.date_ValidTo;
		this.layout_ActiveRangeTo.CustomizationFormText = "To";
		this.layout_ActiveRangeTo.Location = new System.Drawing.Point(265, 282);
		this.layout_ActiveRangeTo.MaxSize = new System.Drawing.Size(158, 24);
		this.layout_ActiveRangeTo.MinSize = new System.Drawing.Size(158, 24);
		this.layout_ActiveRangeTo.Name = "layout_ActiveRangeTo";
		this.layout_ActiveRangeTo.Size = new System.Drawing.Size(158, 24);
		this.layout_ActiveRangeTo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ActiveRangeTo.Text = "To";
		this.layout_ActiveRangeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_ActiveRangeTo.TextSize = new System.Drawing.Size(50, 13);
		this.layout_ActiveRangeTo.TextToControlDistance = 5;
		this.layout_ActiveRangeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_ActiveRangeRemark.Control = this.txt_ValidRemark;
		this.layout_ActiveRangeRemark.CustomizationFormText = "Remark";
		this.layout_ActiveRangeRemark.Location = new System.Drawing.Point(423, 282);
		this.layout_ActiveRangeRemark.MaxSize = new System.Drawing.Size(288, 24);
		this.layout_ActiveRangeRemark.MinSize = new System.Drawing.Size(288, 24);
		this.layout_ActiveRangeRemark.Name = "layout_ActiveRangeRemark";
		this.layout_ActiveRangeRemark.Size = new System.Drawing.Size(288, 24);
		this.layout_ActiveRangeRemark.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ActiveRangeRemark.Text = "Remark";
		this.layout_ActiveRangeRemark.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_ActiveRangeRemark.TextSize = new System.Drawing.Size(50, 13);
		this.layout_ActiveRangeRemark.TextToControlDistance = 5;
		this.layout_ActiveRangeRemark.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_InactiveRangeFrom.Control = this.date_InvalidFrom;
		this.layout_InactiveRangeFrom.CustomizationFormText = "From";
		this.layout_InactiveRangeFrom.Location = new System.Drawing.Point(83, 306);
		this.layout_InactiveRangeFrom.MaxSize = new System.Drawing.Size(182, 24);
		this.layout_InactiveRangeFrom.MinSize = new System.Drawing.Size(182, 24);
		this.layout_InactiveRangeFrom.Name = "layout_InactiveRangeFrom";
		this.layout_InactiveRangeFrom.Size = new System.Drawing.Size(182, 24);
		this.layout_InactiveRangeFrom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_InactiveRangeFrom.Text = "Inactive From";
		this.layout_InactiveRangeFrom.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_InactiveRangeFrom.TextSize = new System.Drawing.Size(70, 13);
		this.layout_InactiveRangeFrom.TextToControlDistance = 5;
		this.layout_InactiveRangeFrom.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_InactiveRangeTo.Control = this.date_InvalidTo;
		this.layout_InactiveRangeTo.CustomizationFormText = "To";
		this.layout_InactiveRangeTo.Location = new System.Drawing.Point(265, 306);
		this.layout_InactiveRangeTo.MaxSize = new System.Drawing.Size(158, 24);
		this.layout_InactiveRangeTo.MinSize = new System.Drawing.Size(158, 24);
		this.layout_InactiveRangeTo.Name = "layout_InactiveRangeTo";
		this.layout_InactiveRangeTo.Size = new System.Drawing.Size(158, 24);
		this.layout_InactiveRangeTo.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_InactiveRangeTo.Text = "To";
		this.layout_InactiveRangeTo.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_InactiveRangeTo.TextSize = new System.Drawing.Size(50, 13);
		this.layout_InactiveRangeTo.TextToControlDistance = 5;
		this.layout_InactiveRangeTo.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_InactiveRangeRemark.Control = this.txt_InvalidRemark;
		this.layout_InactiveRangeRemark.CustomizationFormText = "Remark";
		this.layout_InactiveRangeRemark.Location = new System.Drawing.Point(423, 306);
		this.layout_InactiveRangeRemark.MaxSize = new System.Drawing.Size(288, 24);
		this.layout_InactiveRangeRemark.MinSize = new System.Drawing.Size(288, 24);
		this.layout_InactiveRangeRemark.Name = "layout_InactiveRangeRemark";
		this.layout_InactiveRangeRemark.Size = new System.Drawing.Size(288, 24);
		this.layout_InactiveRangeRemark.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_InactiveRangeRemark.Text = "Remark";
		this.layout_InactiveRangeRemark.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_InactiveRangeRemark.TextSize = new System.Drawing.Size(50, 13);
		this.layout_InactiveRangeRemark.TextToControlDistance = 5;
		this.layout_InactiveRangeRemark.Visibility = DevExpress.XtraLayout.Utils.LayoutVisibility.Never;
		this.layout_ActiveRemark.Control = this.ValidComm;
		this.layout_ActiveRemark.CustomizationFormText = "Remark";
		this.layout_ActiveRemark.Location = new System.Drawing.Point(423, 210);
		this.layout_ActiveRemark.MaxSize = new System.Drawing.Size(288, 24);
		this.layout_ActiveRemark.MinSize = new System.Drawing.Size(288, 24);
		this.layout_ActiveRemark.Name = "layout_ActiveRemark";
		this.layout_ActiveRemark.Size = new System.Drawing.Size(288, 24);
		this.layout_ActiveRemark.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layout_ActiveRemark.Text = "Remark";
		this.layout_ActiveRemark.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.layout_ActiveRemark.TextSize = new System.Drawing.Size(50, 13);
		this.layout_ActiveRemark.TextToControlDistance = 5;
		this.emptySpaceItem2.AllowHotTrack = false;
		this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
		this.emptySpaceItem2.Location = new System.Drawing.Point(0, 330);
		this.emptySpaceItem2.MaxSize = new System.Drawing.Size(711, 64);
		this.emptySpaceItem2.MinSize = new System.Drawing.Size(711, 64);
		this.emptySpaceItem2.Name = "emptySpaceItem2";
		this.emptySpaceItem2.Size = new System.Drawing.Size(1040, 64);
		this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem19.AllowHotTrack = false;
		this.emptySpaceItem19.CustomizationFormText = "emptySpaceItem19";
		this.emptySpaceItem19.Location = new System.Drawing.Point(83, 258);
		this.emptySpaceItem19.MaxSize = new System.Drawing.Size(628, 24);
		this.emptySpaceItem19.MinSize = new System.Drawing.Size(628, 24);
		this.emptySpaceItem19.Name = "emptySpaceItem19";
		this.emptySpaceItem19.Size = new System.Drawing.Size(628, 24);
		this.emptySpaceItem19.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem19.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem18.AllowHotTrack = false;
		this.emptySpaceItem18.CustomizationFormText = "emptySpaceItem18";
		this.emptySpaceItem18.Location = new System.Drawing.Point(711, 0);
		this.emptySpaceItem18.MaxSize = new System.Drawing.Size(329, 320);
		this.emptySpaceItem18.MinSize = new System.Drawing.Size(329, 320);
		this.emptySpaceItem18.Name = "emptySpaceItem18";
		this.emptySpaceItem18.Size = new System.Drawing.Size(329, 330);
		this.emptySpaceItem18.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem18.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
		this.emptySpaceItem18.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem16.AllowHotTrack = false;
		this.emptySpaceItem16.CustomizationFormText = "emptySpaceItem16";
		this.emptySpaceItem16.Location = new System.Drawing.Point(0, 281);
		this.emptySpaceItem16.MaxSize = new System.Drawing.Size(63, 49);
		this.emptySpaceItem16.MinSize = new System.Drawing.Size(63, 49);
		this.emptySpaceItem16.Name = "emptySpaceItem16";
		this.emptySpaceItem16.Size = new System.Drawing.Size(63, 49);
		this.emptySpaceItem16.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem16.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem20.AllowHotTrack = false;
		this.emptySpaceItem20.CustomizationFormText = "emptySpaceItem20";
		this.emptySpaceItem20.Location = new System.Drawing.Point(83, 200);
		this.emptySpaceItem20.MaxSize = new System.Drawing.Size(628, 10);
		this.emptySpaceItem20.MinSize = new System.Drawing.Size(628, 10);
		this.emptySpaceItem20.Name = "emptySpaceItem20";
		this.emptySpaceItem20.Size = new System.Drawing.Size(628, 10);
		this.emptySpaceItem20.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.emptySpaceItem20.TextSize = new System.Drawing.Size(0, 0);
		this.xtraTabPage2.Controls.Add(this.layoutControl3);
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "P&urchasing Data";
		this.layoutControl3.Controls.Add(this.textEdit38);
		this.layoutControl3.Controls.Add(this.simpleButton6);
		this.layoutControl3.Controls.Add(this.simpleButton5);
		this.layoutControl3.Controls.Add(this.label3);
		this.layoutControl3.Controls.Add(this.label2);
		this.layoutControl3.Controls.Add(this.Rate);
		this.layoutControl3.Controls.Add(this.TotalTax);
		this.layoutControl3.Controls.Add(this.PurPackMsr);
		this.layoutControl3.Controls.Add(this.SuppCatNum);
		this.layoutControl3.Controls.Add(this.CardCode);
		this.layoutControl3.Controls.Add(this.BuyUnitMsr);
		this.layoutControl3.Controls.Add(this.NumInBuy);
		this.layoutControl3.Controls.Add(this.PurPackUn);
		this.layoutControl3.Controls.Add(this.CstGrpCode);
		this.layoutControl3.Controls.Add(this.VatGroupPu);
		this.layoutControl3.Controls.Add(this.textEdit25);
		this.layoutControl3.Controls.Add(this.textEdit26);
		this.layoutControl3.Controls.Add(this.textEdit27);
		this.layoutControl3.Controls.Add(this.BVolume);
		this.layoutControl3.Controls.Add(this.textEdit29);
		this.layoutControl3.Controls.Add(this.PurFactor1);
		this.layoutControl3.Controls.Add(this.PurFactor2);
		this.layoutControl3.Controls.Add(this.PurFactor3);
		this.layoutControl3.Controls.Add(this.PurFactor4);
		this.layoutControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl3.Location = new System.Drawing.Point(0, 0);
		this.layoutControl3.Name = "layoutControl3";
		this.layoutControl3.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(839, 46, 250, 350);
		this.layoutControl3.Root = this.layoutControlGroup2;
		this.layoutControl3.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl3.TabIndex = 0;
		this.layoutControl3.Text = "layoutControl3";
		this.textEdit38.FieldName = null;
		this.textEdit38.IsSearchControl = false;
		this.textEdit38.Location = new System.Drawing.Point(875, 99);
		this.textEdit38.Name = "textEdit38";
		this.textEdit38.Size = new System.Drawing.Size(50, 20);
		this.textEdit38.StyleController = this.layoutControl3;
		this.textEdit38.TabIndex = 28;
		this.simpleButton6.FieldName = null;
		this.simpleButton6.IsSearchControl = false;
		this.simpleButton6.Location = new System.Drawing.Point(875, 38);
		this.simpleButton6.Name = "simpleButton6";
		this.simpleButton6.Size = new System.Drawing.Size(47, 22);
		this.simpleButton6.StyleController = this.layoutControl3;
		this.simpleButton6.TabIndex = 27;
		this.simpleButton6.Text = ">>";
		this.simpleButton5.FieldName = null;
		this.simpleButton5.IsSearchControl = false;
		this.simpleButton5.Location = new System.Drawing.Point(361, 12);
		this.simpleButton5.Name = "simpleButton5";
		this.simpleButton5.Size = new System.Drawing.Size(36, 22);
		this.simpleButton5.StyleController = this.layoutControl3;
		this.simpleButton5.TabIndex = 26;
		this.simpleButton5.Text = "...";
		this.label3.Location = new System.Drawing.Point(469, 355);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(50, 20);
		this.label3.TabIndex = 25;
		this.label3.Text = "%";
		this.label2.Location = new System.Drawing.Point(469, 331);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(50, 20);
		this.label2.TabIndex = 24;
		this.label2.Text = "%";
		this.Rate.FieldName = null;
		this.Rate.IsSearchControl = false;
		this.Rate.Location = new System.Drawing.Point(361, 355);
		this.Rate.Name = "Rate";
		this.Rate.Size = new System.Drawing.Size(104, 20);
		this.Rate.StyleController = this.layoutControl3;
		this.Rate.TabIndex = 23;
		this.TotalTax.FieldName = null;
		this.TotalTax.IsSearchControl = false;
		this.TotalTax.Location = new System.Drawing.Point(361, 331);
		this.TotalTax.Name = "TotalTax";
		this.TotalTax.Size = new System.Drawing.Size(104, 20);
		this.TotalTax.StyleController = this.layoutControl3;
		this.TotalTax.TabIndex = 22;
		this.PurPackMsr.FieldName = null;
		this.PurPackMsr.IsSearchControl = false;
		this.PurPackMsr.Location = new System.Drawing.Point(130, 157);
		this.PurPackMsr.Name = "PurPackMsr";
		this.PurPackMsr.Size = new System.Drawing.Size(227, 20);
		this.PurPackMsr.StyleController = this.layoutControl3;
		this.PurPackMsr.TabIndex = 9;
		this.SuppCatNum.FieldName = null;
		this.SuppCatNum.IsSearchControl = false;
		this.SuppCatNum.Location = new System.Drawing.Point(130, 50);
		this.SuppCatNum.Name = "SuppCatNum";
		this.SuppCatNum.Size = new System.Drawing.Size(227, 20);
		this.SuppCatNum.StyleController = this.layoutControl3;
		this.SuppCatNum.TabIndex = 5;
		this.CardCode.FieldName = null;
		this.CardCode.IsSearchControl = false;
		this.CardCode.Location = new System.Drawing.Point(130, 12);
		this.CardCode.Name = "CardCode";
		this.CardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.CardCode.Size = new System.Drawing.Size(227, 20);
		this.CardCode.StyleController = this.layoutControl3;
		this.CardCode.TabIndex = 4;
		this.BuyUnitMsr.FieldName = null;
		this.BuyUnitMsr.IsSearchControl = false;
		this.BuyUnitMsr.Location = new System.Drawing.Point(130, 91);
		this.BuyUnitMsr.Name = "BuyUnitMsr";
		this.BuyUnitMsr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.BuyUnitMsr.Size = new System.Drawing.Size(227, 20);
		this.BuyUnitMsr.StyleController = this.layoutControl3;
		this.BuyUnitMsr.TabIndex = 6;
		this.NumInBuy.FieldName = null;
		this.NumInBuy.IsSearchControl = false;
		this.NumInBuy.Location = new System.Drawing.Point(130, 115);
		this.NumInBuy.Name = "NumInBuy";
		this.NumInBuy.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.NumInBuy.Properties.IsSearchControl = false;
		this.NumInBuy.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.NumInBuy.Size = new System.Drawing.Size(227, 20);
		this.NumInBuy.StyleController = this.layoutControl3;
		this.NumInBuy.TabIndex = 7;
		this.PurPackUn.FieldName = null;
		this.PurPackUn.IsSearchControl = false;
		this.PurPackUn.Location = new System.Drawing.Point(130, 181);
		this.PurPackUn.Name = "PurPackUn";
		this.PurPackUn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PurPackUn.Properties.IsSearchControl = false;
		this.PurPackUn.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.PurPackUn.Size = new System.Drawing.Size(227, 20);
		this.PurPackUn.StyleController = this.layoutControl3;
		this.PurPackUn.TabIndex = 8;
		this.CstGrpCode.Location = new System.Drawing.Point(130, 331);
		this.CstGrpCode.Name = "CstGrpCode";
		this.CstGrpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.CstGrpCode.Properties.NullText = "";
		this.CstGrpCode.Size = new System.Drawing.Size(227, 20);
		this.CstGrpCode.StyleController = this.layoutControl3;
		this.CstGrpCode.TabIndex = 20;
		this.VatGroupPu.Location = new System.Drawing.Point(130, 355);
		this.VatGroupPu.Name = "VatGroupPu";
		this.VatGroupPu.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.VatGroupPu.Properties.NullText = "";
		this.VatGroupPu.Size = new System.Drawing.Size(227, 20);
		this.VatGroupPu.StyleController = this.layoutControl3;
		this.VatGroupPu.TabIndex = 21;
		this.textEdit25.FieldName = null;
		this.textEdit25.IsSearchControl = false;
		this.textEdit25.Location = new System.Drawing.Point(720, 38);
		this.textEdit25.Name = "textEdit25";
		this.textEdit25.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit25.Properties.IsSearchControl = false;
		this.textEdit25.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit25.Size = new System.Drawing.Size(151, 20);
		this.textEdit25.StyleController = this.layoutControl3;
		this.textEdit25.TabIndex = 10;
		this.textEdit26.FieldName = null;
		this.textEdit26.IsSearchControl = false;
		this.textEdit26.Location = new System.Drawing.Point(720, 62);
		this.textEdit26.Name = "textEdit26";
		this.textEdit26.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit26.Properties.IsSearchControl = false;
		this.textEdit26.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit26.Size = new System.Drawing.Size(151, 20);
		this.textEdit26.StyleController = this.layoutControl3;
		this.textEdit26.TabIndex = 11;
		this.textEdit27.FieldName = null;
		this.textEdit27.IsSearchControl = false;
		this.textEdit27.Location = new System.Drawing.Point(720, 86);
		this.textEdit27.Name = "textEdit27";
		this.textEdit27.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit27.Properties.IsSearchControl = false;
		this.textEdit27.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit27.Size = new System.Drawing.Size(151, 20);
		this.textEdit27.StyleController = this.layoutControl3;
		this.textEdit27.TabIndex = 12;
		this.BVolume.FieldName = null;
		this.BVolume.IsSearchControl = false;
		this.BVolume.Location = new System.Drawing.Point(720, 110);
		this.BVolume.Name = "BVolume";
		this.BVolume.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.BVolume.Properties.IsSearchControl = false;
		this.BVolume.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.BVolume.Size = new System.Drawing.Size(151, 20);
		this.BVolume.StyleController = this.layoutControl3;
		this.BVolume.TabIndex = 13;
		this.textEdit29.FieldName = null;
		this.textEdit29.IsSearchControl = false;
		this.textEdit29.Location = new System.Drawing.Point(720, 134);
		this.textEdit29.Name = "textEdit29";
		this.textEdit29.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit29.Properties.IsSearchControl = false;
		this.textEdit29.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit29.Size = new System.Drawing.Size(151, 20);
		this.textEdit29.StyleController = this.layoutControl3;
		this.textEdit29.TabIndex = 14;
		this.PurFactor1.FieldName = null;
		this.PurFactor1.IsSearchControl = false;
		this.PurFactor1.Location = new System.Drawing.Point(720, 204);
		this.PurFactor1.Name = "PurFactor1";
		this.PurFactor1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PurFactor1.Properties.IsSearchControl = false;
		this.PurFactor1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.PurFactor1.Size = new System.Drawing.Size(151, 20);
		this.PurFactor1.StyleController = this.layoutControl3;
		this.PurFactor1.TabIndex = 15;
		this.PurFactor2.FieldName = null;
		this.PurFactor2.IsSearchControl = false;
		this.PurFactor2.Location = new System.Drawing.Point(720, 228);
		this.PurFactor2.Name = "PurFactor2";
		this.PurFactor2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PurFactor2.Properties.IsSearchControl = false;
		this.PurFactor2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.PurFactor2.Size = new System.Drawing.Size(151, 20);
		this.PurFactor2.StyleController = this.layoutControl3;
		this.PurFactor2.TabIndex = 16;
		this.PurFactor3.FieldName = null;
		this.PurFactor3.IsSearchControl = false;
		this.PurFactor3.Location = new System.Drawing.Point(720, 252);
		this.PurFactor3.Name = "PurFactor3";
		this.PurFactor3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PurFactor3.Properties.IsSearchControl = false;
		this.PurFactor3.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.PurFactor3.Size = new System.Drawing.Size(151, 20);
		this.PurFactor3.StyleController = this.layoutControl3;
		this.PurFactor3.TabIndex = 17;
		this.PurFactor4.FieldName = null;
		this.PurFactor4.IsSearchControl = false;
		this.PurFactor4.Location = new System.Drawing.Point(720, 276);
		this.PurFactor4.Name = "PurFactor4";
		this.PurFactor4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PurFactor4.Properties.IsSearchControl = false;
		this.PurFactor4.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.PurFactor4.Size = new System.Drawing.Size(151, 20);
		this.PurFactor4.StyleController = this.layoutControl3;
		this.PurFactor4.TabIndex = 18;
		this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
		this.layoutControlGroup2.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup2.GroupBordersVisible = false;
		this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[43]
		{
			this.emptySpaceItem22, this.layoutControlItem29, this.emptySpaceItem24, this.layoutControlItem30, this.emptySpaceItem25, this.layoutControlItem31, this.layoutControlItem32, this.emptySpaceItem26, this.layoutControlItem33, this.emptySpaceItem30,
			this.emptySpaceItem31, this.layoutControlItem35, this.layoutControlItem36, this.layoutControlItem37, this.layoutControlItem38, this.layoutControlItem39, this.layoutControlItem34, this.emptySpaceItem23, this.emptySpaceItem28, this.emptySpaceItem29,
			this.emptySpaceItem32, this.layoutControlItem40, this.layoutControlItem41, this.layoutControlItem42, this.layoutControlItem43, this.emptySpaceItem33, this.emptySpaceItem34, this.emptySpaceItem35, this.layoutControlItem45, this.layoutControlItem44,
			this.emptySpaceItem36, this.emptySpaceItem40, this.layoutControlItem46, this.layoutControlItem47, this.layoutControlItem48, this.layoutControlItem49, this.layoutControlItem50, this.emptySpaceItem27, this.layoutControlItem51, this.emptySpaceItem37,
			this.emptySpaceItem38, this.layoutControlItem52, this.emptySpaceItem39
		});
		this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup2.Name = "Root";
		this.layoutControlGroup2.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup2.TextVisible = false;
		this.emptySpaceItem22.AllowHotTrack = false;
		this.emptySpaceItem22.CustomizationFormText = "emptySpaceItem22";
		this.emptySpaceItem22.Location = new System.Drawing.Point(0, 367);
		this.emptySpaceItem22.Name = "emptySpaceItem22";
		this.emptySpaceItem22.Size = new System.Drawing.Size(349, 27);
		this.emptySpaceItem22.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem29.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem29.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem29.Control = this.CardCode;
		this.layoutControlItem29.CustomizationFormText = "Preferred Vendor";
		this.layoutControlItem29.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem29.Name = "layoutControlItem29";
		this.layoutControlItem29.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem29.Text = "Preferred Vendor";
		this.layoutControlItem29.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem24.AllowHotTrack = false;
		this.emptySpaceItem24.CustomizationFormText = "emptySpaceItem24";
		this.emptySpaceItem24.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem24.Name = "emptySpaceItem24";
		this.emptySpaceItem24.Size = new System.Drawing.Size(349, 14);
		this.emptySpaceItem24.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem30.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem30.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem30.Control = this.SuppCatNum;
		this.layoutControlItem30.CustomizationFormText = "layoutControlItem30";
		this.layoutControlItem30.Location = new System.Drawing.Point(0, 38);
		this.layoutControlItem30.Name = "layoutControlItem30";
		this.layoutControlItem30.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem30.Text = "Mfr Catalog No.";
		this.layoutControlItem30.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem25.AllowHotTrack = false;
		this.emptySpaceItem25.CustomizationFormText = "emptySpaceItem25";
		this.emptySpaceItem25.Location = new System.Drawing.Point(0, 62);
		this.emptySpaceItem25.Name = "emptySpaceItem25";
		this.emptySpaceItem25.Size = new System.Drawing.Size(349, 17);
		this.emptySpaceItem25.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem31.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem31.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem31.Control = this.BuyUnitMsr;
		this.layoutControlItem31.CustomizationFormText = "layoutControlItem31";
		this.layoutControlItem31.Location = new System.Drawing.Point(0, 79);
		this.layoutControlItem31.Name = "layoutControlItem31";
		this.layoutControlItem31.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem31.Text = "Purchasing UoM Name";
		this.layoutControlItem31.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem32.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem32.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem32.Control = this.NumInBuy;
		this.layoutControlItem32.CustomizationFormText = "layoutControlItem32";
		this.layoutControlItem32.Location = new System.Drawing.Point(0, 103);
		this.layoutControlItem32.Name = "layoutControlItem32";
		this.layoutControlItem32.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem32.Text = "Items per Purchase Unit";
		this.layoutControlItem32.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem26.AllowHotTrack = false;
		this.emptySpaceItem26.CustomizationFormText = "emptySpaceItem26";
		this.emptySpaceItem26.Location = new System.Drawing.Point(0, 127);
		this.emptySpaceItem26.Name = "emptySpaceItem26";
		this.emptySpaceItem26.Size = new System.Drawing.Size(349, 18);
		this.emptySpaceItem26.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem33.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem33.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem33.Control = this.PurPackUn;
		this.layoutControlItem33.CustomizationFormText = "layoutControlItem33";
		this.layoutControlItem33.Location = new System.Drawing.Point(0, 169);
		this.layoutControlItem33.Name = "layoutControlItem33";
		this.layoutControlItem33.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem33.Text = "Quantity per Package";
		this.layoutControlItem33.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem30.AllowHotTrack = false;
		this.emptySpaceItem30.CustomizationFormText = "emptySpaceItem30";
		this.emptySpaceItem30.Location = new System.Drawing.Point(863, 111);
		this.emptySpaceItem30.Name = "emptySpaceItem30";
		this.emptySpaceItem30.Size = new System.Drawing.Size(177, 35);
		this.emptySpaceItem30.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem31.AllowHotTrack = false;
		this.emptySpaceItem31.CustomizationFormText = "emptySpaceItem31";
		this.emptySpaceItem31.Location = new System.Drawing.Point(0, 193);
		this.emptySpaceItem31.Name = "emptySpaceItem31";
		this.emptySpaceItem31.Size = new System.Drawing.Size(349, 126);
		this.emptySpaceItem31.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem35.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem35.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem35.Control = this.textEdit25;
		this.layoutControlItem35.CustomizationFormText = "layoutControlItem35";
		this.layoutControlItem35.Location = new System.Drawing.Point(590, 26);
		this.layoutControlItem35.Name = "layoutControlItem35";
		this.layoutControlItem35.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem35.Text = "Length";
		this.layoutControlItem35.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem36.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem36.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem36.Control = this.textEdit26;
		this.layoutControlItem36.CustomizationFormText = "layoutControlItem36";
		this.layoutControlItem36.Location = new System.Drawing.Point(590, 50);
		this.layoutControlItem36.Name = "layoutControlItem36";
		this.layoutControlItem36.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem36.Text = "Width";
		this.layoutControlItem36.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem37.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem37.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem37.Control = this.textEdit27;
		this.layoutControlItem37.CustomizationFormText = "layoutControlItem37";
		this.layoutControlItem37.Location = new System.Drawing.Point(590, 74);
		this.layoutControlItem37.Name = "layoutControlItem37";
		this.layoutControlItem37.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem37.Text = "Height";
		this.layoutControlItem37.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem38.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem38.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem38.Control = this.BVolume;
		this.layoutControlItem38.CustomizationFormText = "layoutControlItem38";
		this.layoutControlItem38.Location = new System.Drawing.Point(590, 98);
		this.layoutControlItem38.Name = "layoutControlItem38";
		this.layoutControlItem38.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem38.Text = "Volume";
		this.layoutControlItem38.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem39.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem39.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem39.Control = this.textEdit29;
		this.layoutControlItem39.CustomizationFormText = "layoutControlItem39";
		this.layoutControlItem39.Location = new System.Drawing.Point(590, 122);
		this.layoutControlItem39.Name = "layoutControlItem39";
		this.layoutControlItem39.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem39.Text = "Weight";
		this.layoutControlItem39.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem34.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem34.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem34.Control = this.PurPackMsr;
		this.layoutControlItem34.CustomizationFormText = "layoutControlItem34";
		this.layoutControlItem34.Location = new System.Drawing.Point(0, 145);
		this.layoutControlItem34.Name = "layoutControlItem34";
		this.layoutControlItem34.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem34.Text = "Packaging UoM Name";
		this.layoutControlItem34.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem23.AllowHotTrack = false;
		this.emptySpaceItem23.CustomizationFormText = "emptySpaceItem23";
		this.emptySpaceItem23.Location = new System.Drawing.Point(590, 288);
		this.emptySpaceItem23.Name = "emptySpaceItem23";
		this.emptySpaceItem23.Size = new System.Drawing.Size(273, 31);
		this.emptySpaceItem23.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem28.AllowHotTrack = false;
		this.emptySpaceItem28.CustomizationFormText = "emptySpaceItem28";
		this.emptySpaceItem28.Location = new System.Drawing.Point(590, 146);
		this.emptySpaceItem28.Name = "emptySpaceItem28";
		this.emptySpaceItem28.Size = new System.Drawing.Size(273, 46);
		this.emptySpaceItem28.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem29.AllowHotTrack = false;
		this.emptySpaceItem29.CustomizationFormText = "emptySpaceItem29";
		this.emptySpaceItem29.Location = new System.Drawing.Point(863, 146);
		this.emptySpaceItem29.Name = "emptySpaceItem29";
		this.emptySpaceItem29.Size = new System.Drawing.Size(177, 46);
		this.emptySpaceItem29.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem32.AllowHotTrack = false;
		this.emptySpaceItem32.CustomizationFormText = "emptySpaceItem32";
		this.emptySpaceItem32.Location = new System.Drawing.Point(863, 192);
		this.emptySpaceItem32.Name = "emptySpaceItem32";
		this.emptySpaceItem32.Size = new System.Drawing.Size(177, 127);
		this.emptySpaceItem32.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem40.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem40.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem40.Control = this.PurFactor1;
		this.layoutControlItem40.CustomizationFormText = "layoutControlItem40";
		this.layoutControlItem40.Location = new System.Drawing.Point(590, 192);
		this.layoutControlItem40.Name = "layoutControlItem40";
		this.layoutControlItem40.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem40.Text = "Factor 1";
		this.layoutControlItem40.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem41.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem41.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem41.Control = this.PurFactor2;
		this.layoutControlItem41.CustomizationFormText = "layoutControlItem41";
		this.layoutControlItem41.Location = new System.Drawing.Point(590, 216);
		this.layoutControlItem41.Name = "layoutControlItem41";
		this.layoutControlItem41.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem41.Text = "Factor 2";
		this.layoutControlItem41.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem42.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem42.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem42.Control = this.PurFactor3;
		this.layoutControlItem42.CustomizationFormText = "layoutControlItem42";
		this.layoutControlItem42.Location = new System.Drawing.Point(590, 240);
		this.layoutControlItem42.Name = "layoutControlItem42";
		this.layoutControlItem42.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem42.Text = "Factor 3";
		this.layoutControlItem42.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem43.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem43.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem43.Control = this.PurFactor4;
		this.layoutControlItem43.CustomizationFormText = "layoutControlItem43";
		this.layoutControlItem43.Location = new System.Drawing.Point(590, 264);
		this.layoutControlItem43.Name = "layoutControlItem43";
		this.layoutControlItem43.Size = new System.Drawing.Size(273, 24);
		this.layoutControlItem43.Text = "Factor 4";
		this.layoutControlItem43.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem33.AllowHotTrack = false;
		this.emptySpaceItem33.CustomizationFormText = "emptySpaceItem33";
		this.emptySpaceItem33.Location = new System.Drawing.Point(590, 319);
		this.emptySpaceItem33.Name = "emptySpaceItem33";
		this.emptySpaceItem33.Size = new System.Drawing.Size(450, 75);
		this.emptySpaceItem33.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem34.AllowHotTrack = false;
		this.emptySpaceItem34.CustomizationFormText = "emptySpaceItem34";
		this.emptySpaceItem34.Location = new System.Drawing.Point(349, 26);
		this.emptySpaceItem34.Name = "emptySpaceItem34";
		this.emptySpaceItem34.Size = new System.Drawing.Size(241, 293);
		this.emptySpaceItem34.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem35.AllowHotTrack = false;
		this.emptySpaceItem35.CustomizationFormText = "emptySpaceItem35";
		this.emptySpaceItem35.Location = new System.Drawing.Point(349, 367);
		this.emptySpaceItem35.Name = "emptySpaceItem35";
		this.emptySpaceItem35.Size = new System.Drawing.Size(241, 27);
		this.emptySpaceItem35.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem45.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem45.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem45.Control = this.CstGrpCode;
		this.layoutControlItem45.CustomizationFormText = "layoutControlItem45";
		this.layoutControlItem45.Location = new System.Drawing.Point(0, 319);
		this.layoutControlItem45.Name = "layoutControlItem45";
		this.layoutControlItem45.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem45.Text = "Customs Group";
		this.layoutControlItem45.TextSize = new System.Drawing.Size(115, 13);
		this.layoutControlItem44.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem44.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem44.Control = this.VatGroupPu;
		this.layoutControlItem44.CustomizationFormText = "layoutControlItem44";
		this.layoutControlItem44.Location = new System.Drawing.Point(0, 343);
		this.layoutControlItem44.Name = "layoutControlItem44";
		this.layoutControlItem44.Size = new System.Drawing.Size(349, 24);
		this.layoutControlItem44.Text = "Tax Group";
		this.layoutControlItem44.TextSize = new System.Drawing.Size(115, 13);
		this.emptySpaceItem36.AllowHotTrack = false;
		this.emptySpaceItem36.CustomizationFormText = "emptySpaceItem36";
		this.emptySpaceItem36.Location = new System.Drawing.Point(389, 0);
		this.emptySpaceItem36.Name = "emptySpaceItem36";
		this.emptySpaceItem36.Size = new System.Drawing.Size(201, 26);
		this.emptySpaceItem36.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem40.AllowHotTrack = false;
		this.emptySpaceItem40.CustomizationFormText = "emptySpaceItem40";
		this.emptySpaceItem40.Location = new System.Drawing.Point(511, 319);
		this.emptySpaceItem40.Name = "emptySpaceItem40";
		this.emptySpaceItem40.Size = new System.Drawing.Size(79, 48);
		this.emptySpaceItem40.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem46.Control = this.TotalTax;
		this.layoutControlItem46.CustomizationFormText = "layoutControlItem46";
		this.layoutControlItem46.Location = new System.Drawing.Point(349, 319);
		this.layoutControlItem46.Name = "layoutControlItem46";
		this.layoutControlItem46.Size = new System.Drawing.Size(108, 24);
		this.layoutControlItem46.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem46.TextVisible = false;
		this.layoutControlItem47.Control = this.Rate;
		this.layoutControlItem47.CustomizationFormText = "layoutControlItem47";
		this.layoutControlItem47.Location = new System.Drawing.Point(349, 343);
		this.layoutControlItem47.Name = "layoutControlItem47";
		this.layoutControlItem47.Size = new System.Drawing.Size(108, 24);
		this.layoutControlItem47.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem47.TextVisible = false;
		this.layoutControlItem48.Control = this.label2;
		this.layoutControlItem48.CustomizationFormText = "layoutControlItem48";
		this.layoutControlItem48.Location = new System.Drawing.Point(457, 319);
		this.layoutControlItem48.Name = "layoutControlItem48";
		this.layoutControlItem48.Size = new System.Drawing.Size(54, 24);
		this.layoutControlItem48.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem48.TextVisible = false;
		this.layoutControlItem49.Control = this.label3;
		this.layoutControlItem49.CustomizationFormText = "layoutControlItem49";
		this.layoutControlItem49.Location = new System.Drawing.Point(457, 343);
		this.layoutControlItem49.Name = "layoutControlItem49";
		this.layoutControlItem49.Size = new System.Drawing.Size(54, 24);
		this.layoutControlItem49.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem49.TextVisible = false;
		this.layoutControlItem50.Control = this.simpleButton5;
		this.layoutControlItem50.CustomizationFormText = "layoutControlItem50";
		this.layoutControlItem50.Location = new System.Drawing.Point(349, 0);
		this.layoutControlItem50.Name = "layoutControlItem50";
		this.layoutControlItem50.Size = new System.Drawing.Size(40, 26);
		this.layoutControlItem50.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem50.TextVisible = false;
		this.emptySpaceItem27.AllowHotTrack = false;
		this.emptySpaceItem27.CustomizationFormText = "emptySpaceItem27";
		this.emptySpaceItem27.Location = new System.Drawing.Point(590, 0);
		this.emptySpaceItem27.Name = "emptySpaceItem27";
		this.emptySpaceItem27.Size = new System.Drawing.Size(450, 26);
		this.emptySpaceItem27.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem51.Control = this.simpleButton6;
		this.layoutControlItem51.CustomizationFormText = "layoutControlItem51";
		this.layoutControlItem51.Location = new System.Drawing.Point(863, 26);
		this.layoutControlItem51.Name = "layoutControlItem51";
		this.layoutControlItem51.Size = new System.Drawing.Size(51, 26);
		this.layoutControlItem51.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem51.TextVisible = false;
		this.emptySpaceItem37.AllowHotTrack = false;
		this.emptySpaceItem37.CustomizationFormText = "emptySpaceItem37";
		this.emptySpaceItem37.Location = new System.Drawing.Point(914, 26);
		this.emptySpaceItem37.Name = "emptySpaceItem37";
		this.emptySpaceItem37.Size = new System.Drawing.Size(126, 26);
		this.emptySpaceItem37.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem38.AllowHotTrack = false;
		this.emptySpaceItem38.CustomizationFormText = "emptySpaceItem38";
		this.emptySpaceItem38.Location = new System.Drawing.Point(863, 52);
		this.emptySpaceItem38.Name = "emptySpaceItem38";
		this.emptySpaceItem38.Size = new System.Drawing.Size(177, 35);
		this.emptySpaceItem38.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem52.Control = this.textEdit38;
		this.layoutControlItem52.CustomizationFormText = "layoutControlItem52";
		this.layoutControlItem52.Location = new System.Drawing.Point(863, 87);
		this.layoutControlItem52.Name = "layoutControlItem52";
		this.layoutControlItem52.Size = new System.Drawing.Size(54, 24);
		this.layoutControlItem52.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem52.TextVisible = false;
		this.emptySpaceItem39.AllowHotTrack = false;
		this.emptySpaceItem39.CustomizationFormText = "emptySpaceItem39";
		this.emptySpaceItem39.Location = new System.Drawing.Point(917, 87);
		this.emptySpaceItem39.Name = "emptySpaceItem39";
		this.emptySpaceItem39.Size = new System.Drawing.Size(123, 24);
		this.emptySpaceItem39.TextSize = new System.Drawing.Size(0, 0);
		this.xtraTabPage3.Controls.Add(this.layoutControl4);
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "Sales Data";
		this.layoutControl4.Controls.Add(this.label4);
		this.layoutControl4.Controls.Add(this.Rate2);
		this.layoutControl4.Controls.Add(this.SalPackMsr);
		this.layoutControl4.Controls.Add(this.VatGourpSa);
		this.layoutControl4.Controls.Add(this.SalUnitMsr);
		this.layoutControl4.Controls.Add(this.NumInSale);
		this.layoutControl4.Controls.Add(this.SalPackUn);
		this.layoutControl4.Controls.Add(this.textEdit44);
		this.layoutControl4.Controls.Add(this.textEdit45);
		this.layoutControl4.Controls.Add(this.textEdit46);
		this.layoutControl4.Controls.Add(this.SVolume);
		this.layoutControl4.Controls.Add(this.textEdit48);
		this.layoutControl4.Controls.Add(this.SalFactor1);
		this.layoutControl4.Controls.Add(this.SalFactor2);
		this.layoutControl4.Controls.Add(this.SalFactor3);
		this.layoutControl4.Controls.Add(this.SalFactor4);
		this.layoutControl4.Controls.Add(this.textEdit53);
		this.layoutControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl4.Location = new System.Drawing.Point(0, 0);
		this.layoutControl4.Name = "layoutControl4";
		this.layoutControl4.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(540, 294, 250, 350);
		this.layoutControl4.Root = this.layoutControlGroup3;
		this.layoutControl4.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl4.TabIndex = 0;
		this.layoutControl4.Text = "layoutControl4";
		this.label4.Location = new System.Drawing.Point(399, 34);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(191, 20);
		this.label4.TabIndex = 20;
		this.label4.Text = "%";
		this.Rate2.FieldName = null;
		this.Rate2.IsSearchControl = false;
		this.Rate2.Location = new System.Drawing.Point(305, 34);
		this.Rate2.Name = "Rate2";
		this.Rate2.Size = new System.Drawing.Size(90, 20);
		this.Rate2.StyleController = this.layoutControl4;
		this.Rate2.TabIndex = 19;
		this.SalPackMsr.FieldName = null;
		this.SalPackMsr.IsSearchControl = false;
		this.SalPackMsr.Location = new System.Drawing.Point(119, 161);
		this.SalPackMsr.Name = "SalPackMsr";
		this.SalPackMsr.Size = new System.Drawing.Size(182, 20);
		this.SalPackMsr.StyleController = this.layoutControl4;
		this.SalPackMsr.TabIndex = 7;
		this.VatGourpSa.Location = new System.Drawing.Point(119, 34);
		this.VatGourpSa.Name = "VatGourpSa";
		this.VatGourpSa.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.VatGourpSa.Properties.NullText = "";
		this.VatGourpSa.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.VatGourpSa.Size = new System.Drawing.Size(182, 20);
		this.VatGourpSa.StyleController = this.layoutControl4;
		this.VatGourpSa.TabIndex = 4;
		this.SalUnitMsr.FieldName = null;
		this.SalUnitMsr.IsSearchControl = false;
		this.SalUnitMsr.Location = new System.Drawing.Point(119, 90);
		this.SalUnitMsr.Name = "SalUnitMsr";
		this.SalUnitMsr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.SalUnitMsr.Size = new System.Drawing.Size(182, 20);
		this.SalUnitMsr.StyleController = this.layoutControl4;
		this.SalUnitMsr.TabIndex = 5;
		this.NumInSale.FieldName = null;
		this.NumInSale.IsSearchControl = false;
		this.NumInSale.Location = new System.Drawing.Point(119, 114);
		this.NumInSale.Name = "NumInSale";
		this.NumInSale.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.NumInSale.Properties.IsSearchControl = false;
		this.NumInSale.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.NumInSale.Size = new System.Drawing.Size(182, 20);
		this.NumInSale.StyleController = this.layoutControl4;
		this.NumInSale.TabIndex = 6;
		this.SalPackUn.FieldName = null;
		this.SalPackUn.IsSearchControl = false;
		this.SalPackUn.Location = new System.Drawing.Point(119, 185);
		this.SalPackUn.Name = "SalPackUn";
		this.SalPackUn.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SalPackUn.Properties.IsSearchControl = false;
		this.SalPackUn.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.SalPackUn.Size = new System.Drawing.Size(182, 20);
		this.SalPackUn.StyleController = this.layoutControl4;
		this.SalPackUn.TabIndex = 8;
		this.textEdit44.FieldName = null;
		this.textEdit44.IsSearchControl = false;
		this.textEdit44.Location = new System.Drawing.Point(701, 43);
		this.textEdit44.Name = "textEdit44";
		this.textEdit44.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit44.Properties.IsSearchControl = false;
		this.textEdit44.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit44.Size = new System.Drawing.Size(165, 20);
		this.textEdit44.StyleController = this.layoutControl4;
		this.textEdit44.TabIndex = 9;
		this.textEdit45.FieldName = null;
		this.textEdit45.IsSearchControl = false;
		this.textEdit45.Location = new System.Drawing.Point(701, 67);
		this.textEdit45.Name = "textEdit45";
		this.textEdit45.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit45.Properties.IsSearchControl = false;
		this.textEdit45.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit45.Size = new System.Drawing.Size(165, 20);
		this.textEdit45.StyleController = this.layoutControl4;
		this.textEdit45.TabIndex = 10;
		this.textEdit46.FieldName = null;
		this.textEdit46.IsSearchControl = false;
		this.textEdit46.Location = new System.Drawing.Point(701, 91);
		this.textEdit46.Name = "textEdit46";
		this.textEdit46.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit46.Properties.IsSearchControl = false;
		this.textEdit46.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit46.Size = new System.Drawing.Size(165, 20);
		this.textEdit46.StyleController = this.layoutControl4;
		this.textEdit46.TabIndex = 11;
		this.SVolume.FieldName = null;
		this.SVolume.IsSearchControl = false;
		this.SVolume.Location = new System.Drawing.Point(701, 115);
		this.SVolume.Name = "SVolume";
		this.SVolume.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SVolume.Properties.IsSearchControl = false;
		this.SVolume.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.SVolume.Size = new System.Drawing.Size(165, 20);
		this.SVolume.StyleController = this.layoutControl4;
		this.SVolume.TabIndex = 12;
		this.textEdit48.FieldName = null;
		this.textEdit48.IsSearchControl = false;
		this.textEdit48.Location = new System.Drawing.Point(701, 139);
		this.textEdit48.Name = "textEdit48";
		this.textEdit48.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit48.Properties.IsSearchControl = false;
		this.textEdit48.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit48.Size = new System.Drawing.Size(165, 20);
		this.textEdit48.StyleController = this.layoutControl4;
		this.textEdit48.TabIndex = 13;
		this.SalFactor1.FieldName = null;
		this.SalFactor1.IsSearchControl = false;
		this.SalFactor1.Location = new System.Drawing.Point(701, 198);
		this.SalFactor1.Name = "SalFactor1";
		this.SalFactor1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SalFactor1.Properties.IsSearchControl = false;
		this.SalFactor1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.SalFactor1.Size = new System.Drawing.Size(165, 20);
		this.SalFactor1.StyleController = this.layoutControl4;
		this.SalFactor1.TabIndex = 14;
		this.SalFactor2.FieldName = null;
		this.SalFactor2.IsSearchControl = false;
		this.SalFactor2.Location = new System.Drawing.Point(701, 222);
		this.SalFactor2.Name = "SalFactor2";
		this.SalFactor2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SalFactor2.Properties.IsSearchControl = false;
		this.SalFactor2.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.SalFactor2.Size = new System.Drawing.Size(165, 20);
		this.SalFactor2.StyleController = this.layoutControl4;
		this.SalFactor2.TabIndex = 15;
		this.SalFactor3.FieldName = null;
		this.SalFactor3.IsSearchControl = false;
		this.SalFactor3.Location = new System.Drawing.Point(701, 246);
		this.SalFactor3.Name = "SalFactor3";
		this.SalFactor3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SalFactor3.Properties.IsSearchControl = false;
		this.SalFactor3.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.SalFactor3.Size = new System.Drawing.Size(165, 20);
		this.SalFactor3.StyleController = this.layoutControl4;
		this.SalFactor3.TabIndex = 16;
		this.SalFactor4.FieldName = null;
		this.SalFactor4.IsSearchControl = false;
		this.SalFactor4.Location = new System.Drawing.Point(701, 270);
		this.SalFactor4.Name = "SalFactor4";
		this.SalFactor4.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.SalFactor4.Properties.IsSearchControl = false;
		this.SalFactor4.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.SalFactor4.Size = new System.Drawing.Size(165, 20);
		this.SalFactor4.StyleController = this.layoutControl4;
		this.SalFactor4.TabIndex = 17;
		this.textEdit53.Location = new System.Drawing.Point(870, 115);
		this.textEdit53.Name = "textEdit53";
		this.textEdit53.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit53.Properties.NullText = "";
		this.textEdit53.Size = new System.Drawing.Size(50, 20);
		this.textEdit53.StyleController = this.layoutControl4;
		this.textEdit53.TabIndex = 18;
		this.layoutControlGroup3.CustomizationFormText = "layoutControlGroup3";
		this.layoutControlGroup3.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup3.GroupBordersVisible = false;
		this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[29]
		{
			this.emptySpaceItem41, this.emptySpaceItem42, this.layoutControlItem53, this.emptySpaceItem43, this.emptySpaceItem44, this.layoutControlItem54, this.layoutControlItem55, this.emptySpaceItem45, this.layoutControlItem56, this.layoutControlItem57,
			this.emptySpaceItem46, this.emptySpaceItem48, this.layoutControlItem58, this.layoutControlItem59, this.layoutControlItem60, this.layoutControlItem61, this.layoutControlItem62, this.emptySpaceItem49, this.layoutControlItem63, this.layoutControlItem64,
			this.layoutControlItem65, this.layoutControlItem66, this.emptySpaceItem50, this.emptySpaceItem51, this.layoutControlItem67, this.emptySpaceItem52, this.layoutControlItem68, this.layoutControlItem69, this.emptySpaceItem47
		});
		this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup3.Name = "Root";
		this.layoutControlGroup3.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup3.TextVisible = false;
		this.emptySpaceItem41.AllowHotTrack = false;
		this.emptySpaceItem41.CustomizationFormText = "emptySpaceItem41";
		this.emptySpaceItem41.Location = new System.Drawing.Point(0, 197);
		this.emptySpaceItem41.Name = "emptySpaceItem41";
		this.emptySpaceItem41.Size = new System.Drawing.Size(293, 197);
		this.emptySpaceItem41.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem42.AllowHotTrack = false;
		this.emptySpaceItem42.CustomizationFormText = "emptySpaceItem42";
		this.emptySpaceItem42.Location = new System.Drawing.Point(582, 282);
		this.emptySpaceItem42.Name = "emptySpaceItem42";
		this.emptySpaceItem42.Size = new System.Drawing.Size(276, 112);
		this.emptySpaceItem42.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem53.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem53.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem53.Control = this.VatGourpSa;
		this.layoutControlItem53.CustomizationFormText = "Tax Group";
		this.layoutControlItem53.Location = new System.Drawing.Point(0, 22);
		this.layoutControlItem53.Name = "layoutControlItem53";
		this.layoutControlItem53.Size = new System.Drawing.Size(293, 24);
		this.layoutControlItem53.Text = "Tax Group";
		this.layoutControlItem53.TextSize = new System.Drawing.Size(104, 13);
		this.emptySpaceItem43.AllowHotTrack = false;
		this.emptySpaceItem43.CustomizationFormText = "emptySpaceItem43";
		this.emptySpaceItem43.Location = new System.Drawing.Point(0, 0);
		this.emptySpaceItem43.Name = "emptySpaceItem43";
		this.emptySpaceItem43.Size = new System.Drawing.Size(293, 22);
		this.emptySpaceItem43.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem44.AllowHotTrack = false;
		this.emptySpaceItem44.CustomizationFormText = "emptySpaceItem44";
		this.emptySpaceItem44.Location = new System.Drawing.Point(0, 46);
		this.emptySpaceItem44.Name = "emptySpaceItem44";
		this.emptySpaceItem44.Size = new System.Drawing.Size(293, 32);
		this.emptySpaceItem44.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem54.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem54.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem54.Control = this.SalUnitMsr;
		this.layoutControlItem54.CustomizationFormText = "Sales UoM Name";
		this.layoutControlItem54.Location = new System.Drawing.Point(0, 78);
		this.layoutControlItem54.Name = "layoutControlItem54";
		this.layoutControlItem54.Size = new System.Drawing.Size(293, 24);
		this.layoutControlItem54.Text = "Sales UoM Name";
		this.layoutControlItem54.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem55.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem55.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem55.Control = this.NumInSale;
		this.layoutControlItem55.CustomizationFormText = "Items per Sales Unit";
		this.layoutControlItem55.Location = new System.Drawing.Point(0, 102);
		this.layoutControlItem55.Name = "layoutControlItem55";
		this.layoutControlItem55.Size = new System.Drawing.Size(293, 24);
		this.layoutControlItem55.Text = "Items per Sales Unit";
		this.layoutControlItem55.TextSize = new System.Drawing.Size(104, 13);
		this.emptySpaceItem45.AllowHotTrack = false;
		this.emptySpaceItem45.CustomizationFormText = "emptySpaceItem45";
		this.emptySpaceItem45.Location = new System.Drawing.Point(0, 126);
		this.emptySpaceItem45.Name = "emptySpaceItem45";
		this.emptySpaceItem45.Size = new System.Drawing.Size(293, 23);
		this.emptySpaceItem45.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem56.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem56.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem56.Control = this.SalPackMsr;
		this.layoutControlItem56.CustomizationFormText = "Packaging UoM Name";
		this.layoutControlItem56.Location = new System.Drawing.Point(0, 149);
		this.layoutControlItem56.Name = "layoutControlItem56";
		this.layoutControlItem56.Size = new System.Drawing.Size(293, 24);
		this.layoutControlItem56.Text = "Packaging UoM Name";
		this.layoutControlItem56.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem57.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem57.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem57.Control = this.SalPackUn;
		this.layoutControlItem57.CustomizationFormText = "Quantity per Package";
		this.layoutControlItem57.Location = new System.Drawing.Point(0, 173);
		this.layoutControlItem57.Name = "layoutControlItem57";
		this.layoutControlItem57.Size = new System.Drawing.Size(293, 24);
		this.layoutControlItem57.Text = "Quantity per Package";
		this.layoutControlItem57.TextSize = new System.Drawing.Size(104, 13);
		this.emptySpaceItem46.AllowHotTrack = false;
		this.emptySpaceItem46.CustomizationFormText = "emptySpaceItem46";
		this.emptySpaceItem46.Location = new System.Drawing.Point(293, 46);
		this.emptySpaceItem46.Name = "emptySpaceItem46";
		this.emptySpaceItem46.Size = new System.Drawing.Size(289, 348);
		this.emptySpaceItem46.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem48.AllowHotTrack = false;
		this.emptySpaceItem48.CustomizationFormText = "emptySpaceItem48";
		this.emptySpaceItem48.Location = new System.Drawing.Point(582, 0);
		this.emptySpaceItem48.Name = "emptySpaceItem48";
		this.emptySpaceItem48.Size = new System.Drawing.Size(276, 31);
		this.emptySpaceItem48.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem58.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem58.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem58.Control = this.textEdit44;
		this.layoutControlItem58.CustomizationFormText = "Length";
		this.layoutControlItem58.Location = new System.Drawing.Point(582, 31);
		this.layoutControlItem58.Name = "layoutControlItem58";
		this.layoutControlItem58.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem58.Text = "Length";
		this.layoutControlItem58.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem59.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem59.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem59.Control = this.textEdit45;
		this.layoutControlItem59.CustomizationFormText = "Width";
		this.layoutControlItem59.Location = new System.Drawing.Point(582, 55);
		this.layoutControlItem59.Name = "layoutControlItem59";
		this.layoutControlItem59.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem59.Text = "Width";
		this.layoutControlItem59.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem60.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem60.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem60.Control = this.textEdit46;
		this.layoutControlItem60.CustomizationFormText = "Height";
		this.layoutControlItem60.Location = new System.Drawing.Point(582, 79);
		this.layoutControlItem60.Name = "layoutControlItem60";
		this.layoutControlItem60.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem60.Text = "Height";
		this.layoutControlItem60.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem61.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem61.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem61.Control = this.SVolume;
		this.layoutControlItem61.CustomizationFormText = "Volume";
		this.layoutControlItem61.Location = new System.Drawing.Point(582, 103);
		this.layoutControlItem61.Name = "layoutControlItem61";
		this.layoutControlItem61.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem61.Text = "Volume";
		this.layoutControlItem61.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem62.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem62.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem62.Control = this.textEdit48;
		this.layoutControlItem62.CustomizationFormText = "Weight";
		this.layoutControlItem62.Location = new System.Drawing.Point(582, 127);
		this.layoutControlItem62.Name = "layoutControlItem62";
		this.layoutControlItem62.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem62.Text = "Weight";
		this.layoutControlItem62.TextSize = new System.Drawing.Size(104, 13);
		this.emptySpaceItem49.AllowHotTrack = false;
		this.emptySpaceItem49.CustomizationFormText = "emptySpaceItem49";
		this.emptySpaceItem49.Location = new System.Drawing.Point(582, 151);
		this.emptySpaceItem49.Name = "emptySpaceItem49";
		this.emptySpaceItem49.Size = new System.Drawing.Size(276, 35);
		this.emptySpaceItem49.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem63.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem63.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem63.Control = this.SalFactor1;
		this.layoutControlItem63.CustomizationFormText = "Factor 1";
		this.layoutControlItem63.Location = new System.Drawing.Point(582, 186);
		this.layoutControlItem63.Name = "layoutControlItem63";
		this.layoutControlItem63.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem63.Text = "Factor 1";
		this.layoutControlItem63.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem64.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem64.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem64.Control = this.SalFactor2;
		this.layoutControlItem64.CustomizationFormText = "Factor 2";
		this.layoutControlItem64.Location = new System.Drawing.Point(582, 210);
		this.layoutControlItem64.Name = "layoutControlItem64";
		this.layoutControlItem64.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem64.Text = "Factor 2";
		this.layoutControlItem64.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem65.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem65.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem65.Control = this.SalFactor3;
		this.layoutControlItem65.CustomizationFormText = "Factor 3";
		this.layoutControlItem65.Location = new System.Drawing.Point(582, 234);
		this.layoutControlItem65.Name = "layoutControlItem65";
		this.layoutControlItem65.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem65.Text = "Factor 3";
		this.layoutControlItem65.TextSize = new System.Drawing.Size(104, 13);
		this.layoutControlItem66.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem66.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem66.Control = this.SalFactor4;
		this.layoutControlItem66.CustomizationFormText = "Factor 4";
		this.layoutControlItem66.Location = new System.Drawing.Point(582, 258);
		this.layoutControlItem66.Name = "layoutControlItem66";
		this.layoutControlItem66.Size = new System.Drawing.Size(276, 24);
		this.layoutControlItem66.Text = "Factor 4";
		this.layoutControlItem66.TextSize = new System.Drawing.Size(104, 13);
		this.emptySpaceItem50.AllowHotTrack = false;
		this.emptySpaceItem50.CustomizationFormText = "emptySpaceItem50";
		this.emptySpaceItem50.Location = new System.Drawing.Point(912, 0);
		this.emptySpaceItem50.Name = "emptySpaceItem50";
		this.emptySpaceItem50.Size = new System.Drawing.Size(128, 394);
		this.emptySpaceItem50.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem51.AllowHotTrack = false;
		this.emptySpaceItem51.CustomizationFormText = "emptySpaceItem51";
		this.emptySpaceItem51.Location = new System.Drawing.Point(858, 0);
		this.emptySpaceItem51.Name = "emptySpaceItem51";
		this.emptySpaceItem51.Size = new System.Drawing.Size(54, 103);
		this.emptySpaceItem51.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem67.Control = this.textEdit53;
		this.layoutControlItem67.CustomizationFormText = "layoutControlItem67";
		this.layoutControlItem67.Location = new System.Drawing.Point(858, 103);
		this.layoutControlItem67.Name = "layoutControlItem67";
		this.layoutControlItem67.Size = new System.Drawing.Size(54, 24);
		this.layoutControlItem67.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem67.TextVisible = false;
		this.emptySpaceItem52.AllowHotTrack = false;
		this.emptySpaceItem52.CustomizationFormText = "emptySpaceItem52";
		this.emptySpaceItem52.Location = new System.Drawing.Point(293, 0);
		this.emptySpaceItem52.Name = "emptySpaceItem52";
		this.emptySpaceItem52.Size = new System.Drawing.Size(289, 22);
		this.emptySpaceItem52.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem68.Control = this.Rate2;
		this.layoutControlItem68.CustomizationFormText = "layoutControlItem68";
		this.layoutControlItem68.Location = new System.Drawing.Point(293, 22);
		this.layoutControlItem68.Name = "layoutControlItem68";
		this.layoutControlItem68.Size = new System.Drawing.Size(94, 24);
		this.layoutControlItem68.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem68.TextVisible = false;
		this.layoutControlItem69.Control = this.label4;
		this.layoutControlItem69.CustomizationFormText = "layoutControlItem69";
		this.layoutControlItem69.Location = new System.Drawing.Point(387, 22);
		this.layoutControlItem69.Name = "layoutControlItem69";
		this.layoutControlItem69.Size = new System.Drawing.Size(195, 24);
		this.layoutControlItem69.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem69.TextVisible = false;
		this.emptySpaceItem47.AllowHotTrack = false;
		this.emptySpaceItem47.CustomizationFormText = "emptySpaceItem47";
		this.emptySpaceItem47.Location = new System.Drawing.Point(858, 127);
		this.emptySpaceItem47.Name = "emptySpaceItem47";
		this.emptySpaceItem47.Size = new System.Drawing.Size(54, 267);
		this.emptySpaceItem47.TextSize = new System.Drawing.Size(0, 0);
		this.xtraTabPage4.Controls.Add(this.layoutControl5);
		this.xtraTabPage4.FieldName = null;
		this.xtraTabPage4.IsSearchControl = false;
		this.xtraTabPage4.Name = "xtraTabPage4";
		this.xtraTabPage4.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage4.Text = "I&nventory Data";
		this.layoutControl5.Controls.Add(this.DfltWH);
		this.layoutControl5.Controls.Add(this.btn_SetDefaultWhs);
		this.layoutControl5.Controls.Add(this.grd_oitw);
		this.layoutControl5.Controls.Add(this.label5);
		this.layoutControl5.Controls.Add(this.ByWh);
		this.layoutControl5.Controls.Add(this.GLMethod);
		this.layoutControl5.Controls.Add(this.InvntryUom);
		this.layoutControl5.Controls.Add(this.textEdit57);
		this.layoutControl5.Controls.Add(this.EvalSystem);
		this.layoutControl5.Controls.Add(this.AvgPrice);
		this.layoutControl5.Controls.Add(this.ReorderQty);
		this.layoutControl5.Controls.Add(this.MinLevel);
		this.layoutControl5.Controls.Add(this.MaxLevel);
		this.layoutControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl5.Location = new System.Drawing.Point(0, 0);
		this.layoutControl5.Name = "layoutControl5";
		this.layoutControl5.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(905, 437, 247, 350);
		this.layoutControl5.Root = this.layoutControlGroup4;
		this.layoutControl5.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl5.TabIndex = 0;
		this.layoutControl5.Text = "layoutControl5";
		this.DfltWH.FieldName = null;
		this.DfltWH.IsSearchControl = false;
		this.DfltWH.Location = new System.Drawing.Point(675, 134);
		this.DfltWH.Name = "DfltWH";
		this.DfltWH.Properties.ReadOnly = true;
		this.DfltWH.Size = new System.Drawing.Size(373, 20);
		this.DfltWH.StyleController = this.layoutControl5;
		this.DfltWH.TabIndex = 16;
		this.btn_SetDefaultWhs.FieldName = null;
		this.btn_SetDefaultWhs.IsSearchControl = false;
		this.btn_SetDefaultWhs.Location = new System.Drawing.Point(901, 380);
		this.btn_SetDefaultWhs.Name = "btn_SetDefaultWhs";
		this.btn_SetDefaultWhs.Size = new System.Drawing.Size(147, 22);
		this.btn_SetDefaultWhs.StyleController = this.layoutControl5;
		this.btn_SetDefaultWhs.TabIndex = 14;
		this.btn_SetDefaultWhs.Text = "Set Default Whse";
		this.btn_SetDefaultWhs.Click += new System.EventHandler(this.btn_SetDefaultWhs_Click);
		this.grd_oitw.AllowPasteExcel = false;
		this.grd_oitw.ClipboardData = null;
		this.grd_oitw.Location = new System.Drawing.Point(12, 180);
		this.grd_oitw.MainView = this.gv_oitw;
		this.grd_oitw.Name = "grd_oitw";
		this.grd_oitw.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Locked });
		this.grd_oitw.Size = new System.Drawing.Size(1036, 196);
		this.grd_oitw.TabIndex = 13;
		this.grd_oitw.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_oitw });
		this.gv_oitw.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_oitw.ColumnKey = null;
		this.gv_oitw.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[20]
		{
			this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10, this.gridColumn11, this.gridColumn12, this.gridColumn13,
			this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20, this.gridColumn21, this.gridColumn22, this.gridColumn23
		});
		this.gv_oitw.DisableFormSetting = false;
		this.gv_oitw.DisplayException = false;
		this.gv_oitw.EditColumn = null;
		this.gv_oitw.FieldCurrency = null;
		this.gv_oitw.FieldRate = null;
		this.gv_oitw.FormId = "oItems";
		this.gv_oitw.GridControl = this.grd_oitw;
		this.gv_oitw.IndexEditRow = -1;
		this.gv_oitw.IndicatorWidth = 35;
		this.gv_oitw.IsApzTable = false;
		this.gv_oitw.IsInsertNewRow = false;
		this.gv_oitw.IsLoadingData = false;
		this.gv_oitw.ItemId = "OITW";
		this.gv_oitw.LstColumnAllowBatch = null;
		this.gv_oitw.LstColumnNotAllowBatch = null;
		this.gv_oitw.LstColumNotSetting = null;
		this.gv_oitw.Name = "gv_oitw";
		this.gv_oitw.OptionsView.ShowFooter = true;
		this.gv_oitw.UseBatchMod = false;
		this.gv_oitw.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_oitw_RowStyle);
		this.gridColumn4.Caption = "ItemCode";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "ItemCode";
		this.gridColumn4.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn5.Caption = "Whse Code";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "WhsCode";
		this.gridColumn5.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 0;
		this.gridColumn6.Caption = "Whse Name";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "WhsName";
		this.gridColumn6.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 1;
		this.gridColumn7.Caption = "Locked";
		this.gridColumn7.ColumnEdit = this.rep_Locked;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Locked";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 2;
		this.rep_Locked.AutoHeight = false;
		this.rep_Locked.Name = "rep_Locked";
		this.rep_Locked.ValueChecked = "Y";
		this.rep_Locked.ValueGrayed = "N";
		this.rep_Locked.ValueUnchecked = "N";
		this.gridColumn8.Caption = "InStock";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "OnHand";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 3;
		this.gridColumn9.Caption = "Committed";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "IsCommited";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 4;
		this.gridColumn10.Caption = "Ordered";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "OnOrder";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 5;
		this.gridColumn11.Caption = "Avaiable";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Avaiable";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 6;
		this.gridColumn12.Caption = "Min. Inventory";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "MinStock";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn13.Caption = "Max. Inventory";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "MaxStock";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "Req. Inv. Level";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "MinOrder";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "Item Cost";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "AvgPrice";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "Consignation";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "Consig";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn17.Caption = "Cycle Code";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "CycleCode";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn18.Caption = "Next Count Date";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "NextDate";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn19.Caption = "Time";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "Time";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn20.Caption = "User";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "DestUser";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn21.Caption = "Alert";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Alert";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn22.Caption = "Drop-Ship";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "DropShip";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn23.Caption = "Frozen In";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "FrozenIn";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.label5.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Location = new System.Drawing.Point(542, 35);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(506, 23);
		this.label5.TabIndex = 9;
		this.label5.Text = "Inventory Level";
		this.ByWh.FieldName = null;
		this.ByWh.IsSearchControl = false;
		this.ByWh.Location = new System.Drawing.Point(542, 12);
		this.ByWh.Name = "ByWh";
		this.ByWh.Properties.Caption = "Manage Invent&ory by Warehouse";
		this.ByWh.Properties.ValueChecked = "Y";
		this.ByWh.Properties.ValueGrayed = "N";
		this.ByWh.Properties.ValueUnchecked = "N";
		this.ByWh.Size = new System.Drawing.Size(506, 19);
		this.ByWh.StyleController = this.layoutControl5;
		this.ByWh.TabIndex = 8;
		this.GLMethod.Location = new System.Drawing.Point(145, 12);
		this.GLMethod.Name = "GLMethod";
		this.GLMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.GLMethod.Properties.NullText = "";
		this.GLMethod.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.GLMethod.Size = new System.Drawing.Size(338, 20);
		this.GLMethod.StyleController = this.layoutControl5;
		this.GLMethod.TabIndex = 4;
		this.InvntryUom.FieldName = null;
		this.InvntryUom.IsSearchControl = false;
		this.InvntryUom.Location = new System.Drawing.Point(145, 50);
		this.InvntryUom.Name = "InvntryUom";
		this.InvntryUom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.InvntryUom.Size = new System.Drawing.Size(338, 20);
		this.InvntryUom.StyleController = this.layoutControl5;
		this.InvntryUom.TabIndex = 5;
		this.textEdit57.FieldName = null;
		this.textEdit57.IsSearchControl = false;
		this.textEdit57.Location = new System.Drawing.Point(145, 74);
		this.textEdit57.Name = "textEdit57";
		this.textEdit57.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit57.Properties.IsSearchControl = false;
		this.textEdit57.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit57.Size = new System.Drawing.Size(338, 20);
		this.textEdit57.StyleController = this.layoutControl5;
		this.textEdit57.TabIndex = 6;
		this.EvalSystem.Location = new System.Drawing.Point(145, 110);
		this.EvalSystem.Name = "EvalSystem";
		this.EvalSystem.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.EvalSystem.Properties.NullText = "";
		this.EvalSystem.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.EvalSystem.Size = new System.Drawing.Size(338, 20);
		this.EvalSystem.StyleController = this.layoutControl5;
		this.EvalSystem.TabIndex = 7;
		this.AvgPrice.FieldName = null;
		this.AvgPrice.IsSearchControl = false;
		this.AvgPrice.Location = new System.Drawing.Point(145, 134);
		this.AvgPrice.Name = "AvgPrice";
		this.AvgPrice.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.AvgPrice.Properties.IsSearchControl = false;
		this.AvgPrice.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.AvgPrice.Size = new System.Drawing.Size(338, 20);
		this.AvgPrice.StyleController = this.layoutControl5;
		this.AvgPrice.TabIndex = 15;
		this.ReorderQty.FieldName = null;
		this.ReorderQty.IsSearchControl = false;
		this.ReorderQty.Location = new System.Drawing.Point(675, 62);
		this.ReorderQty.Name = "ReorderQty";
		this.ReorderQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ReorderQty.Properties.IsSearchControl = false;
		this.ReorderQty.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.ReorderQty.Size = new System.Drawing.Size(373, 20);
		this.ReorderQty.StyleController = this.layoutControl5;
		this.ReorderQty.TabIndex = 10;
		this.MinLevel.FieldName = null;
		this.MinLevel.IsSearchControl = false;
		this.MinLevel.Location = new System.Drawing.Point(675, 86);
		this.MinLevel.Name = "MinLevel";
		this.MinLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.MinLevel.Properties.IsSearchControl = false;
		this.MinLevel.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.MinLevel.Size = new System.Drawing.Size(373, 20);
		this.MinLevel.StyleController = this.layoutControl5;
		this.MinLevel.TabIndex = 11;
		this.MaxLevel.FieldName = null;
		this.MaxLevel.IsSearchControl = false;
		this.MaxLevel.Location = new System.Drawing.Point(675, 110);
		this.MaxLevel.Name = "MaxLevel";
		this.MaxLevel.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.MaxLevel.Properties.IsSearchControl = false;
		this.MaxLevel.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.MaxLevel.Size = new System.Drawing.Size(373, 20);
		this.MaxLevel.StyleController = this.layoutControl5;
		this.MaxLevel.TabIndex = 12;
		this.layoutControlGroup4.CustomizationFormText = "layoutControlGroup4";
		this.layoutControlGroup4.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup4.GroupBordersVisible = false;
		this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[19]
		{
			this.emptySpaceItem53, this.emptySpaceItem54, this.emptySpaceItem55, this.layoutControlItem70, this.layoutControlItem71, this.emptySpaceItem56, this.layoutControlItem72, this.emptySpaceItem57, this.layoutControlItem73, this.layoutControlItem74,
			this.emptySpaceItem58, this.layoutControlItem75, this.layoutControlItem76, this.layoutControlItem77, this.layoutControlItem78, this.layoutControlItem79, this.layoutControlItem80, this.layoutControlItem117, this.layoutControlItem14
		});
		this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup4.Name = "Root";
		this.layoutControlGroup4.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup4.TextVisible = false;
		this.emptySpaceItem53.AllowHotTrack = false;
		this.emptySpaceItem53.CustomizationFormText = "emptySpaceItem53";
		this.emptySpaceItem53.Location = new System.Drawing.Point(0, 368);
		this.emptySpaceItem53.Name = "emptySpaceItem53";
		this.emptySpaceItem53.Size = new System.Drawing.Size(889, 26);
		this.emptySpaceItem53.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem54.AllowHotTrack = false;
		this.emptySpaceItem54.CustomizationFormText = "emptySpaceItem54";
		this.emptySpaceItem54.Location = new System.Drawing.Point(0, 146);
		this.emptySpaceItem54.Name = "emptySpaceItem54";
		this.emptySpaceItem54.Size = new System.Drawing.Size(475, 22);
		this.emptySpaceItem54.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem55.AllowHotTrack = false;
		this.emptySpaceItem55.CustomizationFormText = "emptySpaceItem55";
		this.emptySpaceItem55.Location = new System.Drawing.Point(530, 146);
		this.emptySpaceItem55.Name = "emptySpaceItem55";
		this.emptySpaceItem55.Size = new System.Drawing.Size(510, 22);
		this.emptySpaceItem55.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem70.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem70.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem70.Control = this.GLMethod;
		this.layoutControlItem70.CustomizationFormText = "Set G/L Accounts By";
		this.layoutControlItem70.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem70.Name = "layoutControlItem70";
		this.layoutControlItem70.Size = new System.Drawing.Size(475, 24);
		this.layoutControlItem70.Text = "Set G/L Accounts By";
		this.layoutControlItem70.TextSize = new System.Drawing.Size(130, 13);
		this.layoutControlItem71.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem71.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem71.Control = this.InvntryUom;
		this.layoutControlItem71.CustomizationFormText = "UoM Name";
		this.layoutControlItem71.Location = new System.Drawing.Point(0, 38);
		this.layoutControlItem71.Name = "layoutControlItem71";
		this.layoutControlItem71.Size = new System.Drawing.Size(475, 24);
		this.layoutControlItem71.Text = "UoM Name";
		this.layoutControlItem71.TextSize = new System.Drawing.Size(130, 13);
		this.emptySpaceItem56.AllowHotTrack = false;
		this.emptySpaceItem56.CustomizationFormText = "emptySpaceItem56";
		this.emptySpaceItem56.Location = new System.Drawing.Point(0, 24);
		this.emptySpaceItem56.Name = "emptySpaceItem56";
		this.emptySpaceItem56.Size = new System.Drawing.Size(475, 14);
		this.emptySpaceItem56.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem72.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem72.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem72.Control = this.textEdit57;
		this.layoutControlItem72.CustomizationFormText = "Weight";
		this.layoutControlItem72.Location = new System.Drawing.Point(0, 62);
		this.layoutControlItem72.Name = "layoutControlItem72";
		this.layoutControlItem72.Size = new System.Drawing.Size(475, 24);
		this.layoutControlItem72.Text = "Weight";
		this.layoutControlItem72.TextSize = new System.Drawing.Size(130, 13);
		this.emptySpaceItem57.AllowHotTrack = false;
		this.emptySpaceItem57.CustomizationFormText = "emptySpaceItem57";
		this.emptySpaceItem57.Location = new System.Drawing.Point(0, 86);
		this.emptySpaceItem57.Name = "emptySpaceItem57";
		this.emptySpaceItem57.Size = new System.Drawing.Size(475, 12);
		this.emptySpaceItem57.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem73.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem73.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem73.Control = this.EvalSystem;
		this.layoutControlItem73.CustomizationFormText = "Valuation Method";
		this.layoutControlItem73.Location = new System.Drawing.Point(0, 98);
		this.layoutControlItem73.Name = "layoutControlItem73";
		this.layoutControlItem73.Size = new System.Drawing.Size(475, 24);
		this.layoutControlItem73.Text = "Valuation Method";
		this.layoutControlItem73.TextSize = new System.Drawing.Size(130, 13);
		this.layoutControlItem74.Control = this.ByWh;
		this.layoutControlItem74.CustomizationFormText = "layoutControlItem74";
		this.layoutControlItem74.Location = new System.Drawing.Point(530, 0);
		this.layoutControlItem74.Name = "layoutControlItem74";
		this.layoutControlItem74.Size = new System.Drawing.Size(510, 23);
		this.layoutControlItem74.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem74.TextVisible = false;
		this.emptySpaceItem58.AllowHotTrack = false;
		this.emptySpaceItem58.CustomizationFormText = "emptySpaceItem58";
		this.emptySpaceItem58.Location = new System.Drawing.Point(475, 0);
		this.emptySpaceItem58.Name = "emptySpaceItem58";
		this.emptySpaceItem58.Size = new System.Drawing.Size(55, 168);
		this.emptySpaceItem58.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem75.Control = this.label5;
		this.layoutControlItem75.CustomizationFormText = "layoutControlItem75";
		this.layoutControlItem75.Location = new System.Drawing.Point(530, 23);
		this.layoutControlItem75.Name = "layoutControlItem75";
		this.layoutControlItem75.Size = new System.Drawing.Size(510, 27);
		this.layoutControlItem75.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem75.TextVisible = false;
		this.layoutControlItem76.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem76.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem76.Control = this.ReorderQty;
		this.layoutControlItem76.CustomizationFormText = "Required (Purchasing UoM)";
		this.layoutControlItem76.Location = new System.Drawing.Point(530, 50);
		this.layoutControlItem76.Name = "layoutControlItem76";
		this.layoutControlItem76.Size = new System.Drawing.Size(510, 24);
		this.layoutControlItem76.Text = "Required (Purchasing UoM)";
		this.layoutControlItem76.TextSize = new System.Drawing.Size(130, 13);
		this.layoutControlItem77.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem77.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem77.Control = this.MinLevel;
		this.layoutControlItem77.CustomizationFormText = "Minimum";
		this.layoutControlItem77.Location = new System.Drawing.Point(530, 74);
		this.layoutControlItem77.Name = "layoutControlItem77";
		this.layoutControlItem77.Size = new System.Drawing.Size(510, 24);
		this.layoutControlItem77.Text = "Minimum";
		this.layoutControlItem77.TextSize = new System.Drawing.Size(130, 13);
		this.layoutControlItem78.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem78.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem78.Control = this.MaxLevel;
		this.layoutControlItem78.CustomizationFormText = "Maximum";
		this.layoutControlItem78.Location = new System.Drawing.Point(530, 98);
		this.layoutControlItem78.Name = "layoutControlItem78";
		this.layoutControlItem78.Size = new System.Drawing.Size(510, 24);
		this.layoutControlItem78.Text = "Maximum";
		this.layoutControlItem78.TextSize = new System.Drawing.Size(130, 13);
		this.layoutControlItem79.Control = this.grd_oitw;
		this.layoutControlItem79.CustomizationFormText = "layoutControlItem79";
		this.layoutControlItem79.Location = new System.Drawing.Point(0, 168);
		this.layoutControlItem79.Name = "layoutControlItem79";
		this.layoutControlItem79.Size = new System.Drawing.Size(1040, 200);
		this.layoutControlItem79.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem79.TextVisible = false;
		this.layoutControlItem80.Control = this.btn_SetDefaultWhs;
		this.layoutControlItem80.CustomizationFormText = "layoutControlItem80";
		this.layoutControlItem80.Location = new System.Drawing.Point(889, 368);
		this.layoutControlItem80.Name = "layoutControlItem80";
		this.layoutControlItem80.Size = new System.Drawing.Size(151, 26);
		this.layoutControlItem80.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem80.TextVisible = false;
		this.layoutControlItem117.Control = this.AvgPrice;
		this.layoutControlItem117.CustomizationFormText = "Item Cost";
		this.layoutControlItem117.Location = new System.Drawing.Point(0, 122);
		this.layoutControlItem117.Name = "layoutControlItem117";
		this.layoutControlItem117.Size = new System.Drawing.Size(475, 24);
		this.layoutControlItem117.Text = "Item Cost";
		this.layoutControlItem117.TextSize = new System.Drawing.Size(130, 13);
		this.layoutControlItem14.Control = this.DfltWH;
		this.layoutControlItem14.CustomizationFormText = "Default warehouse";
		this.layoutControlItem14.Location = new System.Drawing.Point(530, 122);
		this.layoutControlItem14.Name = "layoutControlItem14";
		this.layoutControlItem14.Size = new System.Drawing.Size(510, 24);
		this.layoutControlItem14.Text = "Default warehouse";
		this.layoutControlItem14.TextSize = new System.Drawing.Size(130, 13);
		this.xtraTabPage5.Controls.Add(this.layoutControl6);
		this.xtraTabPage5.FieldName = null;
		this.xtraTabPage5.IsSearchControl = false;
		this.xtraTabPage5.Name = "xtraTabPage5";
		this.xtraTabPage5.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage5.Text = "P&lanning Data";
		this.layoutControl6.Controls.Add(this.label7);
		this.layoutControl6.Controls.Add(this.label6);
		this.layoutControl6.Controls.Add(this.PlaningSys);
		this.layoutControl6.Controls.Add(this.PrcrmntMtd);
		this.layoutControl6.Controls.Add(this.OrdrIntrvl);
		this.layoutControl6.Controls.Add(this.OrdrMulti);
		this.layoutControl6.Controls.Add(this.MinOrdrQty);
		this.layoutControl6.Controls.Add(this.LeadTime);
		this.layoutControl6.Controls.Add(this.ToleranDay);
		this.layoutControl6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl6.Location = new System.Drawing.Point(0, 0);
		this.layoutControl6.Name = "layoutControl6";
		this.layoutControl6.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(780, 418, 250, 350);
		this.layoutControl6.Root = this.layoutControlGroup5;
		this.layoutControl6.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl6.TabIndex = 0;
		this.layoutControl6.Text = "layoutControl6";
		this.label7.Location = new System.Drawing.Point(446, 133);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(62, 20);
		this.label7.TabIndex = 12;
		this.label7.Text = "Days";
		this.label6.Location = new System.Drawing.Point(446, 109);
		this.label6.Name = "label6";
		this.label6.Size = new System.Drawing.Size(62, 20);
		this.label6.TabIndex = 11;
		this.label6.Text = "Days";
		this.PlaningSys.Location = new System.Drawing.Point(115, 12);
		this.PlaningSys.Name = "PlaningSys";
		this.PlaningSys.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PlaningSys.Properties.NullText = "";
		this.PlaningSys.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.PlaningSys.Size = new System.Drawing.Size(393, 20);
		this.PlaningSys.StyleController = this.layoutControl6;
		this.PlaningSys.TabIndex = 4;
		this.PrcrmntMtd.Location = new System.Drawing.Point(115, 36);
		this.PrcrmntMtd.Name = "PrcrmntMtd";
		this.PrcrmntMtd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.PrcrmntMtd.Properties.NullText = "";
		this.PrcrmntMtd.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.PrcrmntMtd.Size = new System.Drawing.Size(393, 20);
		this.PrcrmntMtd.StyleController = this.layoutControl6;
		this.PrcrmntMtd.TabIndex = 5;
		this.OrdrIntrvl.Location = new System.Drawing.Point(115, 85);
		this.OrdrIntrvl.Name = "OrdrIntrvl";
		this.OrdrIntrvl.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.OrdrIntrvl.Properties.NullText = "";
		this.OrdrIntrvl.Size = new System.Drawing.Size(393, 20);
		this.OrdrIntrvl.StyleController = this.layoutControl6;
		this.OrdrIntrvl.TabIndex = 6;
		this.OrdrMulti.FieldName = null;
		this.OrdrMulti.IsSearchControl = false;
		this.OrdrMulti.Location = new System.Drawing.Point(115, 109);
		this.OrdrMulti.Name = "OrdrMulti";
		this.OrdrMulti.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.OrdrMulti.Properties.IsSearchControl = false;
		this.OrdrMulti.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.OrdrMulti.Size = new System.Drawing.Size(327, 20);
		this.OrdrMulti.StyleController = this.layoutControl6;
		this.OrdrMulti.TabIndex = 7;
		this.MinOrdrQty.FieldName = null;
		this.MinOrdrQty.IsSearchControl = false;
		this.MinOrdrQty.Location = new System.Drawing.Point(115, 133);
		this.MinOrdrQty.Name = "MinOrdrQty";
		this.MinOrdrQty.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.MinOrdrQty.Properties.IsSearchControl = false;
		this.MinOrdrQty.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.MinOrdrQty.Size = new System.Drawing.Size(327, 20);
		this.MinOrdrQty.StyleController = this.layoutControl6;
		this.MinOrdrQty.TabIndex = 8;
		this.LeadTime.FieldName = null;
		this.LeadTime.IsSearchControl = false;
		this.LeadTime.Location = new System.Drawing.Point(115, 212);
		this.LeadTime.Name = "LeadTime";
		this.LeadTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.LeadTime.Properties.IsSearchControl = false;
		this.LeadTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.LeadTime.Size = new System.Drawing.Size(393, 20);
		this.LeadTime.StyleController = this.layoutControl6;
		this.LeadTime.TabIndex = 9;
		this.ToleranDay.FieldName = null;
		this.ToleranDay.IsSearchControl = false;
		this.ToleranDay.Location = new System.Drawing.Point(115, 236);
		this.ToleranDay.Name = "ToleranDay";
		this.ToleranDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ToleranDay.Properties.IsSearchControl = false;
		this.ToleranDay.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.ToleranDay.Size = new System.Drawing.Size(393, 20);
		this.ToleranDay.StyleController = this.layoutControl6;
		this.ToleranDay.TabIndex = 10;
		this.layoutControlGroup5.CustomizationFormText = "layoutControlGroup5";
		this.layoutControlGroup5.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup5.GroupBordersVisible = false;
		this.layoutControlGroup5.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[13]
		{
			this.emptySpaceItem62, this.layoutControlItem81, this.layoutControlItem82, this.emptySpaceItem64, this.layoutControlItem83, this.layoutControlItem84, this.layoutControlItem85, this.emptySpaceItem65, this.layoutControlItem86, this.layoutControlItem87,
			this.emptySpaceItem66, this.layoutControlItem88, this.layoutControlItem89
		});
		this.layoutControlGroup5.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup5.Name = "Root";
		this.layoutControlGroup5.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup5.TextVisible = false;
		this.emptySpaceItem62.AllowHotTrack = false;
		this.emptySpaceItem62.CustomizationFormText = "emptySpaceItem62";
		this.emptySpaceItem62.Location = new System.Drawing.Point(0, 248);
		this.emptySpaceItem62.Name = "emptySpaceItem62";
		this.emptySpaceItem62.Size = new System.Drawing.Size(500, 146);
		this.emptySpaceItem62.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem81.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem81.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem81.Control = this.PlaningSys;
		this.layoutControlItem81.CustomizationFormText = "Planning Method";
		this.layoutControlItem81.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem81.Name = "layoutControlItem81";
		this.layoutControlItem81.Size = new System.Drawing.Size(500, 24);
		this.layoutControlItem81.Text = "Planning Method";
		this.layoutControlItem81.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem82.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem82.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem82.Control = this.PrcrmntMtd;
		this.layoutControlItem82.CustomizationFormText = "Procurement Method";
		this.layoutControlItem82.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem82.Name = "layoutControlItem82";
		this.layoutControlItem82.Size = new System.Drawing.Size(500, 24);
		this.layoutControlItem82.Text = "Procurement Method";
		this.layoutControlItem82.TextSize = new System.Drawing.Size(100, 13);
		this.emptySpaceItem64.AllowHotTrack = false;
		this.emptySpaceItem64.CustomizationFormText = "emptySpaceItem64";
		this.emptySpaceItem64.Location = new System.Drawing.Point(0, 48);
		this.emptySpaceItem64.Name = "emptySpaceItem64";
		this.emptySpaceItem64.Size = new System.Drawing.Size(500, 25);
		this.emptySpaceItem64.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem83.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem83.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem83.Control = this.OrdrIntrvl;
		this.layoutControlItem83.CustomizationFormText = "Order Interval";
		this.layoutControlItem83.Location = new System.Drawing.Point(0, 73);
		this.layoutControlItem83.Name = "layoutControlItem83";
		this.layoutControlItem83.Size = new System.Drawing.Size(500, 24);
		this.layoutControlItem83.Text = "Order Interval";
		this.layoutControlItem83.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem84.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem84.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem84.Control = this.OrdrMulti;
		this.layoutControlItem84.CustomizationFormText = "Order Multiple";
		this.layoutControlItem84.Location = new System.Drawing.Point(0, 97);
		this.layoutControlItem84.Name = "layoutControlItem84";
		this.layoutControlItem84.Size = new System.Drawing.Size(434, 24);
		this.layoutControlItem84.Text = "Order Multiple";
		this.layoutControlItem84.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem85.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem85.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem85.Control = this.MinOrdrQty;
		this.layoutControlItem85.CustomizationFormText = "Minimum Order Qty";
		this.layoutControlItem85.Location = new System.Drawing.Point(0, 121);
		this.layoutControlItem85.Name = "layoutControlItem85";
		this.layoutControlItem85.Size = new System.Drawing.Size(434, 24);
		this.layoutControlItem85.Text = "Minimum Order Qty";
		this.layoutControlItem85.TextSize = new System.Drawing.Size(100, 13);
		this.emptySpaceItem65.AllowHotTrack = false;
		this.emptySpaceItem65.CustomizationFormText = "emptySpaceItem65";
		this.emptySpaceItem65.Location = new System.Drawing.Point(0, 145);
		this.emptySpaceItem65.Name = "emptySpaceItem65";
		this.emptySpaceItem65.Size = new System.Drawing.Size(500, 55);
		this.emptySpaceItem65.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem86.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem86.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem86.Control = this.LeadTime;
		this.layoutControlItem86.CustomizationFormText = "Lead Time";
		this.layoutControlItem86.Location = new System.Drawing.Point(0, 200);
		this.layoutControlItem86.Name = "layoutControlItem86";
		this.layoutControlItem86.Size = new System.Drawing.Size(500, 24);
		this.layoutControlItem86.Text = "Lead Time";
		this.layoutControlItem86.TextSize = new System.Drawing.Size(100, 13);
		this.layoutControlItem87.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem87.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem87.Control = this.ToleranDay;
		this.layoutControlItem87.CustomizationFormText = "Tolerance Days";
		this.layoutControlItem87.Location = new System.Drawing.Point(0, 224);
		this.layoutControlItem87.Name = "layoutControlItem87";
		this.layoutControlItem87.Size = new System.Drawing.Size(500, 24);
		this.layoutControlItem87.Text = "Tolerance Days";
		this.layoutControlItem87.TextSize = new System.Drawing.Size(100, 13);
		this.emptySpaceItem66.AllowHotTrack = false;
		this.emptySpaceItem66.CustomizationFormText = "emptySpaceItem66";
		this.emptySpaceItem66.Location = new System.Drawing.Point(500, 0);
		this.emptySpaceItem66.Name = "emptySpaceItem66";
		this.emptySpaceItem66.Size = new System.Drawing.Size(540, 394);
		this.emptySpaceItem66.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem88.Control = this.label6;
		this.layoutControlItem88.CustomizationFormText = "layoutControlItem88";
		this.layoutControlItem88.Location = new System.Drawing.Point(434, 97);
		this.layoutControlItem88.Name = "layoutControlItem88";
		this.layoutControlItem88.Size = new System.Drawing.Size(66, 24);
		this.layoutControlItem88.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem88.TextVisible = false;
		this.layoutControlItem89.Control = this.label7;
		this.layoutControlItem89.CustomizationFormText = "layoutControlItem89";
		this.layoutControlItem89.Location = new System.Drawing.Point(434, 121);
		this.layoutControlItem89.Name = "layoutControlItem89";
		this.layoutControlItem89.Size = new System.Drawing.Size(66, 24);
		this.layoutControlItem89.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem89.TextVisible = false;
		this.xtraTabPage6.Controls.Add(this.layoutControl7);
		this.xtraTabPage6.FieldName = null;
		this.xtraTabPage6.IsSearchControl = false;
		this.xtraTabPage6.Name = "xtraTabPage6";
		this.xtraTabPage6.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage6.Text = "Pr&oduction Data";
		this.layoutControl7.Controls.Add(this.Phantom);
		this.layoutControl7.Controls.Add(this.IssueMthd);
		this.layoutControl7.Controls.Add(this.TreeType);
		this.layoutControl7.Controls.Add(this.textEdit71);
		this.layoutControl7.Controls.Add(this.textEdit72);
		this.layoutControl7.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl7.Location = new System.Drawing.Point(0, 0);
		this.layoutControl7.Name = "layoutControl7";
		this.layoutControl7.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(862, 416, 250, 350);
		this.layoutControl7.Root = this.layoutControlGroup6;
		this.layoutControl7.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl7.TabIndex = 0;
		this.layoutControl7.Text = "layoutControl7";
		this.Phantom.FieldName = null;
		this.Phantom.IsSearchControl = false;
		this.Phantom.Location = new System.Drawing.Point(12, 12);
		this.Phantom.Name = "Phantom";
		this.Phantom.Properties.Caption = "Phantom Item";
		this.Phantom.Properties.ValueChecked = "Y";
		this.Phantom.Properties.ValueGrayed = "N";
		this.Phantom.Properties.ValueUnchecked = "N";
		this.Phantom.Size = new System.Drawing.Size(470, 19);
		this.Phantom.StyleController = this.layoutControl7;
		this.Phantom.TabIndex = 4;
		this.IssueMthd.Location = new System.Drawing.Point(133, 35);
		this.IssueMthd.Name = "IssueMthd";
		this.IssueMthd.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.IssueMthd.Properties.NullText = "";
		this.IssueMthd.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.IssueMthd.Size = new System.Drawing.Size(349, 20);
		this.IssueMthd.StyleController = this.layoutControl7;
		this.IssueMthd.TabIndex = 5;
		this.TreeType.Location = new System.Drawing.Point(133, 142);
		this.TreeType.Name = "TreeType";
		this.TreeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.TreeType.Properties.NullText = "";
		this.TreeType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.TreeType.Size = new System.Drawing.Size(349, 20);
		this.TreeType.StyleController = this.layoutControl7;
		this.TreeType.TabIndex = 6;
		this.textEdit71.FieldName = null;
		this.textEdit71.IsSearchControl = false;
		this.textEdit71.Location = new System.Drawing.Point(133, 166);
		this.textEdit71.Name = "textEdit71";
		this.textEdit71.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit71.Properties.IsSearchControl = false;
		this.textEdit71.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit71.Size = new System.Drawing.Size(349, 20);
		this.textEdit71.StyleController = this.layoutControl7;
		this.textEdit71.TabIndex = 7;
		this.textEdit72.FieldName = null;
		this.textEdit72.IsSearchControl = false;
		this.textEdit72.Location = new System.Drawing.Point(133, 190);
		this.textEdit72.Name = "textEdit72";
		this.textEdit72.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit72.Properties.IsSearchControl = false;
		this.textEdit72.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.textEdit72.Size = new System.Drawing.Size(349, 20);
		this.textEdit72.StyleController = this.layoutControl7;
		this.textEdit72.TabIndex = 8;
		this.layoutControlGroup6.CustomizationFormText = "layoutControlGroup6";
		this.layoutControlGroup6.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup6.GroupBordersVisible = false;
		this.layoutControlGroup6.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[8] { this.emptySpaceItem68, this.emptySpaceItem69, this.layoutControlItem90, this.layoutControlItem91, this.emptySpaceItem70, this.layoutControlItem92, this.layoutControlItem93, this.layoutControlItem94 });
		this.layoutControlGroup6.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup6.Name = "Root";
		this.layoutControlGroup6.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup6.TextVisible = false;
		this.emptySpaceItem68.AllowHotTrack = false;
		this.emptySpaceItem68.CustomizationFormText = "emptySpaceItem68";
		this.emptySpaceItem68.Location = new System.Drawing.Point(0, 202);
		this.emptySpaceItem68.Name = "emptySpaceItem68";
		this.emptySpaceItem68.Size = new System.Drawing.Size(474, 192);
		this.emptySpaceItem68.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem69.AllowHotTrack = false;
		this.emptySpaceItem69.CustomizationFormText = "emptySpaceItem69";
		this.emptySpaceItem69.Location = new System.Drawing.Point(474, 0);
		this.emptySpaceItem69.Name = "emptySpaceItem69";
		this.emptySpaceItem69.Size = new System.Drawing.Size(566, 394);
		this.emptySpaceItem69.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem90.Control = this.Phantom;
		this.layoutControlItem90.CustomizationFormText = "layoutControlItem90";
		this.layoutControlItem90.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem90.Name = "layoutControlItem90";
		this.layoutControlItem90.Size = new System.Drawing.Size(474, 23);
		this.layoutControlItem90.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem90.TextVisible = false;
		this.layoutControlItem91.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem91.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem91.Control = this.IssueMthd;
		this.layoutControlItem91.CustomizationFormText = "Issue Method";
		this.layoutControlItem91.Location = new System.Drawing.Point(0, 23);
		this.layoutControlItem91.Name = "layoutControlItem91";
		this.layoutControlItem91.Size = new System.Drawing.Size(474, 24);
		this.layoutControlItem91.Text = "Issue Method";
		this.layoutControlItem91.TextSize = new System.Drawing.Size(118, 13);
		this.emptySpaceItem70.AllowHotTrack = false;
		this.emptySpaceItem70.CustomizationFormText = "emptySpaceItem70";
		this.emptySpaceItem70.Location = new System.Drawing.Point(0, 47);
		this.emptySpaceItem70.Name = "emptySpaceItem70";
		this.emptySpaceItem70.Size = new System.Drawing.Size(474, 83);
		this.emptySpaceItem70.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem92.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem92.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem92.Control = this.TreeType;
		this.layoutControlItem92.CustomizationFormText = "BOM Type";
		this.layoutControlItem92.Location = new System.Drawing.Point(0, 130);
		this.layoutControlItem92.Name = "layoutControlItem92";
		this.layoutControlItem92.Size = new System.Drawing.Size(474, 24);
		this.layoutControlItem92.Text = "BOM Type";
		this.layoutControlItem92.TextSize = new System.Drawing.Size(118, 13);
		this.layoutControlItem93.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem93.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem93.Control = this.textEdit71;
		this.layoutControlItem93.CustomizationFormText = "No. of Item Components";
		this.layoutControlItem93.Location = new System.Drawing.Point(0, 154);
		this.layoutControlItem93.Name = "layoutControlItem93";
		this.layoutControlItem93.Size = new System.Drawing.Size(474, 24);
		this.layoutControlItem93.Text = "No. of Item Components";
		this.layoutControlItem93.TextSize = new System.Drawing.Size(118, 13);
		this.layoutControlItem94.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem94.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem94.Control = this.textEdit72;
		this.layoutControlItem94.CustomizationFormText = "No. of Item Components";
		this.layoutControlItem94.Location = new System.Drawing.Point(0, 178);
		this.layoutControlItem94.Name = "layoutControlItem94";
		this.layoutControlItem94.Size = new System.Drawing.Size(474, 24);
		this.layoutControlItem94.Text = "No. of Item Components";
		this.layoutControlItem94.TextSize = new System.Drawing.Size(118, 13);
		this.xtraTabPage7.Controls.Add(this.layoutControl8);
		this.xtraTabPage7.FieldName = null;
		this.xtraTabPage7.IsSearchControl = false;
		this.xtraTabPage7.Name = "xtraTabPage7";
		this.xtraTabPage7.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage7.Text = "P&roperties";
		this.layoutControl8.Controls.Add(this.btn_ClearAllProperties);
		this.layoutControl8.Controls.Add(this.btn_SelectAllProperties);
		this.layoutControl8.Controls.Add(this.grd_Properties);
		this.layoutControl8.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl8.Location = new System.Drawing.Point(0, 0);
		this.layoutControl8.Name = "layoutControl8";
		this.layoutControl8.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1319, 340, 250, 350);
		this.layoutControl8.Root = this.layoutControlGroup7;
		this.layoutControl8.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl8.TabIndex = 0;
		this.layoutControl8.Text = "layoutControl8";
		this.btn_ClearAllProperties.FieldName = null;
		this.btn_ClearAllProperties.IsSearchControl = false;
		this.btn_ClearAllProperties.Location = new System.Drawing.Point(473, 38);
		this.btn_ClearAllProperties.Name = "btn_ClearAllProperties";
		this.btn_ClearAllProperties.Size = new System.Drawing.Size(97, 22);
		this.btn_ClearAllProperties.StyleController = this.layoutControl8;
		this.btn_ClearAllProperties.TabIndex = 6;
		this.btn_ClearAllProperties.Text = "Clear Selection";
		this.btn_ClearAllProperties.Click += new System.EventHandler(this.btn_ClearAllProperties_Click);
		this.btn_SelectAllProperties.FieldName = null;
		this.btn_SelectAllProperties.IsSearchControl = false;
		this.btn_SelectAllProperties.Location = new System.Drawing.Point(473, 12);
		this.btn_SelectAllProperties.Name = "btn_SelectAllProperties";
		this.btn_SelectAllProperties.Size = new System.Drawing.Size(97, 22);
		this.btn_SelectAllProperties.StyleController = this.layoutControl8;
		this.btn_SelectAllProperties.TabIndex = 5;
		this.btn_SelectAllProperties.Text = "Select All";
		this.btn_SelectAllProperties.Click += new System.EventHandler(this.btn_SelectAllProperties_Click);
		this.grd_Properties.AllowPasteExcel = false;
		this.grd_Properties.ClipboardData = null;
		this.grd_Properties.Location = new System.Drawing.Point(12, 12);
		this.grd_Properties.MainView = this.gv_Properties;
		this.grd_Properties.Name = "grd_Properties";
		this.grd_Properties.Size = new System.Drawing.Size(457, 390);
		this.grd_Properties.TabIndex = 4;
		this.grd_Properties.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Properties });
		this.gv_Properties.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Properties.ColumnKey = null;
		this.gv_Properties.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn2, this.gridColumn3 });
		this.gv_Properties.DisableFormSetting = false;
		this.gv_Properties.DisplayException = false;
		this.gv_Properties.EditColumn = null;
		this.gv_Properties.FieldCurrency = null;
		this.gv_Properties.FieldRate = null;
		this.gv_Properties.FormId = "oItems";
		this.gv_Properties.GridControl = this.grd_Properties;
		this.gv_Properties.IndexEditRow = -1;
		this.gv_Properties.IndicatorWidth = 35;
		this.gv_Properties.IsApzTable = false;
		this.gv_Properties.IsInsertNewRow = false;
		this.gv_Properties.IsLoadingData = false;
		this.gv_Properties.ItemId = "OPROPETIES";
		this.gv_Properties.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Properties.LstColumnAllowBatch = null;
		this.gv_Properties.LstColumnNotAllowBatch = null;
		this.gv_Properties.LstColumNotSetting = null;
		this.gv_Properties.Name = "gv_Properties";
		this.gv_Properties.OptionsView.ColumnAutoWidth = false;
		this.gv_Properties.UseBatchMod = false;
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItmsTypCod";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Property Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItmsGrpNam";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 240;
		this.gridColumn3.Caption = " ";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Checked";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 80;
		this.layoutControlGroup7.CustomizationFormText = "layoutControlGroup7";
		this.layoutControlGroup7.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup7.GroupBordersVisible = false;
		this.layoutControlGroup7.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.layoutControlItem95, this.emptySpaceItem71, this.layoutControlItem96, this.layoutControlItem97, this.emptySpaceItem10 });
		this.layoutControlGroup7.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup7.Name = "Root";
		this.layoutControlGroup7.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup7.TextVisible = false;
		this.layoutControlItem95.Control = this.grd_Properties;
		this.layoutControlItem95.CustomizationFormText = "layoutControlItem95";
		this.layoutControlItem95.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem95.Name = "layoutControlItem95";
		this.layoutControlItem95.Size = new System.Drawing.Size(461, 394);
		this.layoutControlItem95.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem95.TextVisible = false;
		this.emptySpaceItem71.AllowHotTrack = false;
		this.emptySpaceItem71.CustomizationFormText = "emptySpaceItem71";
		this.emptySpaceItem71.Location = new System.Drawing.Point(461, 52);
		this.emptySpaceItem71.Name = "emptySpaceItem71";
		this.emptySpaceItem71.Size = new System.Drawing.Size(101, 342);
		this.emptySpaceItem71.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem96.Control = this.btn_SelectAllProperties;
		this.layoutControlItem96.CustomizationFormText = "layoutControlItem96";
		this.layoutControlItem96.Location = new System.Drawing.Point(461, 0);
		this.layoutControlItem96.Name = "layoutControlItem96";
		this.layoutControlItem96.Size = new System.Drawing.Size(101, 26);
		this.layoutControlItem96.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem96.TextVisible = false;
		this.layoutControlItem97.Control = this.btn_ClearAllProperties;
		this.layoutControlItem97.CustomizationFormText = "layoutControlItem97";
		this.layoutControlItem97.Location = new System.Drawing.Point(461, 26);
		this.layoutControlItem97.Name = "layoutControlItem97";
		this.layoutControlItem97.Size = new System.Drawing.Size(101, 26);
		this.layoutControlItem97.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem97.TextVisible = false;
		this.emptySpaceItem10.AllowHotTrack = false;
		this.emptySpaceItem10.CustomizationFormText = "emptySpaceItem10";
		this.emptySpaceItem10.Location = new System.Drawing.Point(562, 0);
		this.emptySpaceItem10.Name = "emptySpaceItem10";
		this.emptySpaceItem10.Size = new System.Drawing.Size(478, 394);
		this.emptySpaceItem10.TextSize = new System.Drawing.Size(0, 0);
		this.xtraTabPage8.Controls.Add(this.layoutControl9);
		this.xtraTabPage8.FieldName = null;
		this.xtraTabPage8.IsSearchControl = false;
		this.xtraTabPage8.Name = "xtraTabPage8";
		this.xtraTabPage8.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage8.Text = "Remar&ks";
		this.layoutControl9.Controls.Add(this.UserText);
		this.layoutControl9.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl9.Location = new System.Drawing.Point(0, 0);
		this.layoutControl9.Name = "layoutControl9";
		this.layoutControl9.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(299, 410, 251, 350);
		this.layoutControl9.Root = this.layoutControlGroup8;
		this.layoutControl9.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl9.TabIndex = 0;
		this.layoutControl9.Text = "layoutControl9";
		this.UserText.FieldName = null;
		this.UserText.IsSearchControl = false;
		this.UserText.Location = new System.Drawing.Point(12, 12);
		this.UserText.Name = "UserText";
		this.UserText.Size = new System.Drawing.Size(1036, 390);
		this.UserText.StyleController = this.layoutControl9;
		this.UserText.TabIndex = 4;
		this.layoutControlGroup8.CustomizationFormText = "layoutControlGroup8";
		this.layoutControlGroup8.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup8.GroupBordersVisible = false;
		this.layoutControlGroup8.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[1] { this.layoutControlItem98 });
		this.layoutControlGroup8.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup8.Name = "Root";
		this.layoutControlGroup8.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup8.TextVisible = false;
		this.layoutControlItem98.Control = this.UserText;
		this.layoutControlItem98.CustomizationFormText = "layoutControlItem98";
		this.layoutControlItem98.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem98.Name = "layoutControlItem98";
		this.layoutControlItem98.Size = new System.Drawing.Size(1040, 394);
		this.layoutControlItem98.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem98.TextVisible = false;
		this.xtraTabPage9.Controls.Add(this.layoutControl10);
		this.xtraTabPage9.FieldName = null;
		this.xtraTabPage9.IsSearchControl = false;
		this.xtraTabPage9.Name = "xtraTabPage9";
		this.xtraTabPage9.Size = new System.Drawing.Size(1060, 414);
		this.xtraTabPage9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage9.Text = "Attachments";
		this.layoutControl10.Controls.Add(this.simpleButton13);
		this.layoutControl10.Controls.Add(this.simpleButton12);
		this.layoutControl10.Controls.Add(this.simpleButton11);
		this.layoutControl10.Controls.Add(this.gridControl3);
		this.layoutControl10.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl10.Location = new System.Drawing.Point(0, 0);
		this.layoutControl10.Name = "layoutControl10";
		this.layoutControl10.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(540, 327, 250, 350);
		this.layoutControl10.Root = this.layoutControlGroup9;
		this.layoutControl10.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl10.TabIndex = 0;
		this.layoutControl10.Text = "layoutControl10";
		this.simpleButton13.FieldName = null;
		this.simpleButton13.IsSearchControl = false;
		this.simpleButton13.Location = new System.Drawing.Point(929, 64);
		this.simpleButton13.Name = "simpleButton13";
		this.simpleButton13.Size = new System.Drawing.Size(119, 22);
		this.simpleButton13.StyleController = this.layoutControl10;
		this.simpleButton13.TabIndex = 7;
		this.simpleButton13.Text = "Delete";
		this.simpleButton12.FieldName = null;
		this.simpleButton12.IsSearchControl = false;
		this.simpleButton12.Location = new System.Drawing.Point(929, 38);
		this.simpleButton12.Name = "simpleButton12";
		this.simpleButton12.Size = new System.Drawing.Size(119, 22);
		this.simpleButton12.StyleController = this.layoutControl10;
		this.simpleButton12.TabIndex = 6;
		this.simpleButton12.Text = "Displa&y";
		this.simpleButton11.FieldName = null;
		this.simpleButton11.IsSearchControl = false;
		this.simpleButton11.Location = new System.Drawing.Point(929, 12);
		this.simpleButton11.Name = "simpleButton11";
		this.simpleButton11.Size = new System.Drawing.Size(119, 22);
		this.simpleButton11.StyleController = this.layoutControl10;
		this.simpleButton11.TabIndex = 5;
		this.simpleButton11.Text = "&Browse";
		this.gridControl3.AllowPasteExcel = false;
		this.gridControl3.ClipboardData = null;
		this.gridControl3.Location = new System.Drawing.Point(12, 12);
		this.gridControl3.MainView = this.gridView3;
		this.gridControl3.Name = "gridControl3";
		this.gridControl3.Size = new System.Drawing.Size(913, 390);
		this.gridControl3.TabIndex = 4;
		this.gridControl3.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView3 });
		this.gridView3.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView3.ColumnKey = null;
		this.gridView3.DisableFormSetting = false;
		this.gridView3.DisplayException = false;
		this.gridView3.EditColumn = null;
		this.gridView3.FieldCurrency = null;
		this.gridView3.FieldRate = null;
		this.gridView3.FormId = "oItems";
		this.gridView3.GridControl = this.gridControl3;
		this.gridView3.IndexEditRow = -1;
		this.gridView3.IndicatorWidth = 35;
		this.gridView3.IsApzTable = false;
		this.gridView3.IsInsertNewRow = false;
		this.gridView3.IsLoadingData = false;
		this.gridView3.ItemId = "OATTM";
		this.gridView3.LstColumnAllowBatch = null;
		this.gridView3.LstColumnNotAllowBatch = null;
		this.gridView3.LstColumNotSetting = null;
		this.gridView3.Name = "gridView3";
		this.gridView3.UseBatchMod = false;
		this.layoutControlGroup9.CustomizationFormText = "layoutControlGroup9";
		this.layoutControlGroup9.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup9.GroupBordersVisible = false;
		this.layoutControlGroup9.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[5] { this.emptySpaceItem77, this.layoutControlItem100, this.layoutControlItem101, this.layoutControlItem102, this.layoutControlItem103 });
		this.layoutControlGroup9.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup9.Name = "Root";
		this.layoutControlGroup9.Size = new System.Drawing.Size(1060, 414);
		this.layoutControlGroup9.TextVisible = false;
		this.emptySpaceItem77.AllowHotTrack = false;
		this.emptySpaceItem77.CustomizationFormText = "emptySpaceItem77";
		this.emptySpaceItem77.Location = new System.Drawing.Point(917, 78);
		this.emptySpaceItem77.Name = "emptySpaceItem77";
		this.emptySpaceItem77.Size = new System.Drawing.Size(123, 316);
		this.emptySpaceItem77.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem100.Control = this.gridControl3;
		this.layoutControlItem100.CustomizationFormText = "layoutControlItem100";
		this.layoutControlItem100.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem100.Name = "layoutControlItem100";
		this.layoutControlItem100.Size = new System.Drawing.Size(917, 394);
		this.layoutControlItem100.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem100.TextVisible = false;
		this.layoutControlItem101.Control = this.simpleButton11;
		this.layoutControlItem101.CustomizationFormText = "layoutControlItem101";
		this.layoutControlItem101.Location = new System.Drawing.Point(917, 0);
		this.layoutControlItem101.Name = "layoutControlItem101";
		this.layoutControlItem101.Size = new System.Drawing.Size(123, 26);
		this.layoutControlItem101.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem101.TextVisible = false;
		this.layoutControlItem102.Control = this.simpleButton12;
		this.layoutControlItem102.CustomizationFormText = "layoutControlItem102";
		this.layoutControlItem102.Location = new System.Drawing.Point(917, 26);
		this.layoutControlItem102.Name = "layoutControlItem102";
		this.layoutControlItem102.Size = new System.Drawing.Size(123, 26);
		this.layoutControlItem102.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem102.TextVisible = false;
		this.layoutControlItem103.Control = this.simpleButton13;
		this.layoutControlItem103.CustomizationFormText = "layoutControlItem103";
		this.layoutControlItem103.Location = new System.Drawing.Point(917, 52);
		this.layoutControlItem103.Name = "layoutControlItem103";
		this.layoutControlItem103.Size = new System.Drawing.Size(123, 26);
		this.layoutControlItem103.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem103.TextVisible = false;
		this.tab_UDF.Controls.Add(this.layoutControl_Udf);
		this.tab_UDF.FieldName = null;
		this.tab_UDF.IsSearchControl = false;
		this.tab_UDF.Name = "tab_UDF";
		this.tab_UDF.Size = new System.Drawing.Size(1060, 414);
		this.tab_UDF.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_UDF.Text = "User-Defined Fields";
		this.layoutControl_Udf.Controls.Add(this.layoutControl12);
		this.layoutControl_Udf.Controls.Add(this.layoutControl11);
		this.layoutControl_Udf.Dock = System.Windows.Forms.DockStyle.Fill;
		this.layoutControl_Udf.Location = new System.Drawing.Point(0, 0);
		this.layoutControl_Udf.Name = "layoutControl_Udf";
		this.layoutControl_Udf.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(860, 449, 250, 350);
		this.layoutControl_Udf.Root = this.layout_Udf;
		this.layoutControl_Udf.Size = new System.Drawing.Size(1060, 414);
		this.layoutControl_Udf.TabIndex = 0;
		this.layoutControl_Udf.Text = "layoutControl_Udf";
		this.layoutControl12.Location = new System.Drawing.Point(513, 12);
		this.layoutControl12.Name = "layoutControl12";
		this.layoutControl12.Root = this.layout_UdfRight;
		this.layoutControl12.Size = new System.Drawing.Size(535, 390);
		this.layoutControl12.TabIndex = 5;
		this.layoutControl12.Text = "layoutControl12";
		this.layout_UdfRight.CustomizationFormText = "layout_UdfRight";
		this.layout_UdfRight.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layout_UdfRight.GroupBordersVisible = false;
		this.layout_UdfRight.Location = new System.Drawing.Point(0, 0);
		this.layout_UdfRight.Name = "layout_UdfRight";
		this.layout_UdfRight.Size = new System.Drawing.Size(535, 390);
		this.layout_UdfRight.TextVisible = false;
		this.layoutControl11.Location = new System.Drawing.Point(12, 12);
		this.layoutControl11.Name = "layoutControl11";
		this.layoutControl11.Root = this.layout_UdfLeft;
		this.layoutControl11.Size = new System.Drawing.Size(497, 390);
		this.layoutControl11.TabIndex = 4;
		this.layoutControl11.Text = "layoutControl11";
		this.layout_UdfLeft.CustomizationFormText = "layout_UdfLeft";
		this.layout_UdfLeft.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layout_UdfLeft.GroupBordersVisible = false;
		this.layout_UdfLeft.Location = new System.Drawing.Point(0, 0);
		this.layout_UdfLeft.Name = "layout_UdfLeft";
		this.layout_UdfLeft.Size = new System.Drawing.Size(497, 390);
		this.layout_UdfLeft.TextVisible = false;
		this.layout_Udf.CustomizationFormText = "layout_Udf_Left";
		this.layout_Udf.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layout_Udf.GroupBordersVisible = false;
		this.layout_Udf.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[2] { this.layoutControlItem18, this.layoutControlItem26 });
		this.layout_Udf.Location = new System.Drawing.Point(0, 0);
		this.layout_Udf.Name = "layout_Udf";
		this.layout_Udf.Size = new System.Drawing.Size(1060, 414);
		this.layout_Udf.TextVisible = false;
		this.layoutControlItem18.Control = this.layoutControl11;
		this.layoutControlItem18.CustomizationFormText = "layoutControlItem18";
		this.layoutControlItem18.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem18.Name = "layoutControlItem18";
		this.layoutControlItem18.Size = new System.Drawing.Size(501, 394);
		this.layoutControlItem18.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem18.TextVisible = false;
		this.layoutControlItem26.Control = this.layoutControl12;
		this.layoutControlItem26.CustomizationFormText = "layoutControlItem26";
		this.layoutControlItem26.Location = new System.Drawing.Point(501, 0);
		this.layoutControlItem26.Name = "layoutControlItem26";
		this.layoutControlItem26.Size = new System.Drawing.Size(539, 394);
		this.layoutControlItem26.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem26.TextVisible = false;
		this.SellItem.FieldName = null;
		this.SellItem.IsSearchControl = false;
		this.SellItem.Location = new System.Drawing.Point(716, 35);
		this.SellItem.Name = "SellItem";
		this.SellItem.Properties.Caption = "&Sales Item";
		this.SellItem.Properties.ValueChecked = "Y";
		this.SellItem.Properties.ValueGrayed = "N";
		this.SellItem.Properties.ValueUnchecked = "N";
		this.SellItem.Size = new System.Drawing.Size(95, 19);
		this.SellItem.StyleController = this.layoutControl1;
		this.SellItem.TabIndex = 13;
		this.InvntItem.FieldName = null;
		this.InvntItem.IsSearchControl = false;
		this.InvntItem.Location = new System.Drawing.Point(716, 12);
		this.InvntItem.Name = "InvntItem";
		this.InvntItem.Properties.Caption = "&Inventory Item";
		this.InvntItem.Properties.ValueChecked = "Y";
		this.InvntItem.Properties.ValueGrayed = "N";
		this.InvntItem.Properties.ValueUnchecked = "N";
		this.InvntItem.Size = new System.Drawing.Size(95, 19);
		this.InvntItem.StyleController = this.layoutControl1;
		this.InvntItem.TabIndex = 12;
		this.PrchseItem.FieldName = null;
		this.PrchseItem.IsSearchControl = false;
		this.PrchseItem.Location = new System.Drawing.Point(716, 58);
		this.PrchseItem.Name = "PrchseItem";
		this.PrchseItem.Properties.Caption = "&Purchase Item";
		this.PrchseItem.Properties.ValueChecked = "Y";
		this.PrchseItem.Properties.ValueGrayed = "N";
		this.PrchseItem.Properties.ValueUnchecked = "N";
		this.PrchseItem.Size = new System.Drawing.Size(95, 19);
		this.PrchseItem.StyleController = this.layoutControl1;
		this.PrchseItem.TabIndex = 11;
		this.FrgnName.FieldName = null;
		this.FrgnName.IsSearchControl = false;
		this.FrgnName.Location = new System.Drawing.Point(81, 60);
		this.FrgnName.Name = "FrgnName";
		this.FrgnName.Size = new System.Drawing.Size(511, 20);
		this.FrgnName.StyleController = this.layoutControl1;
		this.FrgnName.TabIndex = 6;
		this.ItemName.FieldName = null;
		this.ItemName.IsSearchControl = false;
		this.ItemName.Location = new System.Drawing.Point(81, 36);
		this.ItemName.Name = "ItemName";
		this.ItemName.Size = new System.Drawing.Size(511, 20);
		this.ItemName.StyleController = this.layoutControl1;
		this.ItemName.TabIndex = 5;
		this.Series.Location = new System.Drawing.Point(81, 12);
		this.Series.Name = "Series";
		this.Series.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.Series.Properties.NullText = "";
		this.Series.Size = new System.Drawing.Size(131, 20);
		this.Series.StyleController = this.layoutControl1;
		this.Series.TabIndex = 4;
		this.ItemType.Location = new System.Drawing.Point(81, 84);
		this.ItemType.Name = "ItemType";
		this.ItemType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ItemType.Properties.NullText = "";
		this.ItemType.Size = new System.Drawing.Size(199, 20);
		this.ItemType.StyleController = this.layoutControl1;
		this.ItemType.TabIndex = 7;
		this.ItmsGrpCod.Location = new System.Drawing.Point(81, 108);
		this.ItmsGrpCod.Name = "ItmsGrpCod";
		this.ItmsGrpCod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ItmsGrpCod.Properties.NullText = "";
		this.ItmsGrpCod.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.ItmsGrpCod.Size = new System.Drawing.Size(199, 20);
		this.ItmsGrpCod.StyleController = this.layoutControl1;
		this.ItmsGrpCod.TabIndex = 8;
		this.UgpEntry.Location = new System.Drawing.Point(81, 132);
		this.UgpEntry.Name = "UgpEntry";
		this.UgpEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.UgpEntry.Properties.NullText = "";
		this.UgpEntry.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.UgpEntry.Size = new System.Drawing.Size(199, 20);
		this.UgpEntry.StyleController = this.layoutControl1;
		this.UgpEntry.TabIndex = 9;
		this.ListName.Location = new System.Drawing.Point(81, 156);
		this.ListName.Name = "ListName";
		this.ListName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.ListName.Properties.NullText = "";
		this.ListName.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.ListName.Size = new System.Drawing.Size(199, 20);
		this.ListName.StyleController = this.layoutControl1;
		this.ListName.TabIndex = 10;
		this.look_PriceType.Location = new System.Drawing.Point(353, 156);
		this.look_PriceType.Name = "look_PriceType";
		this.look_PriceType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_PriceType.Properties.NullText = "";
		this.look_PriceType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.look_PriceType.Size = new System.Drawing.Size(83, 20);
		this.look_PriceType.StyleController = this.layoutControl1;
		this.look_PriceType.TabIndex = 15;
		this.textEdit5.FieldName = null;
		this.textEdit5.IsSearchControl = false;
		this.textEdit5.Location = new System.Drawing.Point(440, 156);
		this.textEdit5.Name = "textEdit5";
		this.textEdit5.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.textEdit5.Size = new System.Drawing.Size(152, 20);
		this.textEdit5.StyleController = this.layoutControl1;
		this.textEdit5.TabIndex = 23;
		this.CodeBars.FieldName = null;
		this.CodeBars.IsSearchControl = false;
		this.CodeBars.Location = new System.Drawing.Point(353, 132);
		this.CodeBars.Name = "CodeBars";
		this.CodeBars.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.CodeBars.Size = new System.Drawing.Size(239, 20);
		this.CodeBars.StyleController = this.layoutControl1;
		this.CodeBars.TabIndex = 14;
		this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
		this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
		this.layoutControlGroup1.GroupBordersVisible = false;
		this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[23]
		{
			this.emptySpaceItem3, this.layoutControlItem1, this.layoutControlItem2, this.layoutControlItem3, this.layoutControlItem4, this.layoutControlItem5, this.layoutControlItem6, this.layoutControlItem7, this.layoutControlItem8, this.layoutControlItem9,
			this.layoutControlItem10, this.emptySpaceItem5, this.layoutControlItem11, this.layoutControlItem12, this.emptySpaceItem7, this.layoutControlItem15, this.layoutControlItem16, this.emptySpaceItem9, this.emptySpaceItem11, this.layoutControlItem17,
			this.layoutControlItem118, this.emptySpaceItem1, this.layoutControlItem13
		});
		this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlGroup1.Name = "Root";
		this.layoutControlGroup1.Size = new System.Drawing.Size(1090, 660);
		this.layoutControlGroup1.TextVisible = false;
		this.emptySpaceItem3.AllowHotTrack = false;
		this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
		this.emptySpaceItem3.Location = new System.Drawing.Point(803, 0);
		this.emptySpaceItem3.Name = "emptySpaceItem3";
		this.emptySpaceItem3.Size = new System.Drawing.Size(267, 168);
		this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem1.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem1.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem1.Control = this.Series;
		this.layoutControlItem1.CustomizationFormText = "Item No.";
		this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
		this.layoutControlItem1.Name = "layoutControlItem1";
		this.layoutControlItem1.Size = new System.Drawing.Size(204, 24);
		this.layoutControlItem1.Text = "Item No.";
		this.layoutControlItem1.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem2.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem2.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem2.Control = this.ItemName;
		this.layoutControlItem2.CustomizationFormText = "Description";
		this.layoutControlItem2.Location = new System.Drawing.Point(0, 24);
		this.layoutControlItem2.Name = "layoutControlItem2";
		this.layoutControlItem2.Size = new System.Drawing.Size(584, 24);
		this.layoutControlItem2.Text = "Description";
		this.layoutControlItem2.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem3.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem3.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem3.Control = this.FrgnName;
		this.layoutControlItem3.CustomizationFormText = "Foreign Name";
		this.layoutControlItem3.Location = new System.Drawing.Point(0, 48);
		this.layoutControlItem3.Name = "layoutControlItem3";
		this.layoutControlItem3.Size = new System.Drawing.Size(584, 24);
		this.layoutControlItem3.Text = "Foreign Name";
		this.layoutControlItem3.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem4.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem4.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem4.Control = this.ItemType;
		this.layoutControlItem4.CustomizationFormText = "Item Type";
		this.layoutControlItem4.Location = new System.Drawing.Point(0, 72);
		this.layoutControlItem4.Name = "layoutControlItem4";
		this.layoutControlItem4.Size = new System.Drawing.Size(272, 24);
		this.layoutControlItem4.Text = "Item Type";
		this.layoutControlItem4.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem5.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem5.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem5.Control = this.ItmsGrpCod;
		this.layoutControlItem5.CustomizationFormText = "Item Group";
		this.layoutControlItem5.Location = new System.Drawing.Point(0, 96);
		this.layoutControlItem5.Name = "layoutControlItem5";
		this.layoutControlItem5.Size = new System.Drawing.Size(272, 24);
		this.layoutControlItem5.Text = "Item Group";
		this.layoutControlItem5.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem6.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem6.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem6.Control = this.UgpEntry;
		this.layoutControlItem6.CustomizationFormText = "UoM Group";
		this.layoutControlItem6.Location = new System.Drawing.Point(0, 120);
		this.layoutControlItem6.Name = "layoutControlItem6";
		this.layoutControlItem6.Size = new System.Drawing.Size(272, 24);
		this.layoutControlItem6.Text = "UoM Group";
		this.layoutControlItem6.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem7.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem7.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem7.Control = this.ListName;
		this.layoutControlItem7.CustomizationFormText = "Price List";
		this.layoutControlItem7.Location = new System.Drawing.Point(0, 144);
		this.layoutControlItem7.Name = "layoutControlItem7";
		this.layoutControlItem7.Size = new System.Drawing.Size(272, 24);
		this.layoutControlItem7.Text = "Price List";
		this.layoutControlItem7.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem8.Control = this.PrchseItem;
		this.layoutControlItem8.CustomizationFormText = "layoutControlItem8";
		this.layoutControlItem8.Location = new System.Drawing.Point(704, 46);
		this.layoutControlItem8.Name = "layoutControlItem8";
		this.layoutControlItem8.Size = new System.Drawing.Size(99, 23);
		this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem8.TextVisible = false;
		this.layoutControlItem9.Control = this.InvntItem;
		this.layoutControlItem9.CustomizationFormText = "layoutControlItem9";
		this.layoutControlItem9.Location = new System.Drawing.Point(704, 0);
		this.layoutControlItem9.Name = "layoutControlItem9";
		this.layoutControlItem9.Size = new System.Drawing.Size(99, 23);
		this.layoutControlItem9.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem9.TextVisible = false;
		this.layoutControlItem10.Control = this.SellItem;
		this.layoutControlItem10.CustomizationFormText = "layoutControlItem10";
		this.layoutControlItem10.Location = new System.Drawing.Point(704, 23);
		this.layoutControlItem10.Name = "layoutControlItem10";
		this.layoutControlItem10.Size = new System.Drawing.Size(99, 23);
		this.layoutControlItem10.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem10.TextVisible = false;
		this.emptySpaceItem5.AllowHotTrack = false;
		this.emptySpaceItem5.CustomizationFormText = "emptySpaceItem5";
		this.emptySpaceItem5.Location = new System.Drawing.Point(272, 72);
		this.emptySpaceItem5.Name = "emptySpaceItem5";
		this.emptySpaceItem5.Size = new System.Drawing.Size(312, 48);
		this.emptySpaceItem5.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem11.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem11.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem11.Control = this.CodeBars;
		this.layoutControlItem11.CustomizationFormText = "Bar Code";
		this.layoutControlItem11.Location = new System.Drawing.Point(272, 120);
		this.layoutControlItem11.Name = "layoutControlItem11";
		this.layoutControlItem11.Size = new System.Drawing.Size(312, 24);
		this.layoutControlItem11.Text = "Bar Code";
		this.layoutControlItem11.TextSize = new System.Drawing.Size(66, 13);
		this.layoutControlItem12.AppearanceItemCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.layoutControlItem12.AppearanceItemCaption.Options.UseFont = true;
		this.layoutControlItem12.Control = this.look_PriceType;
		this.layoutControlItem12.CustomizationFormText = "Unit Price";
		this.layoutControlItem12.Location = new System.Drawing.Point(272, 144);
		this.layoutControlItem12.Name = "layoutControlItem12";
		this.layoutControlItem12.Size = new System.Drawing.Size(156, 24);
		this.layoutControlItem12.Text = "Unit Price";
		this.layoutControlItem12.TextSize = new System.Drawing.Size(66, 13);
		this.emptySpaceItem7.AllowHotTrack = false;
		this.emptySpaceItem7.CustomizationFormText = "emptySpaceItem7";
		this.emptySpaceItem7.Location = new System.Drawing.Point(704, 69);
		this.emptySpaceItem7.Name = "emptySpaceItem7";
		this.emptySpaceItem7.Size = new System.Drawing.Size(99, 99);
		this.emptySpaceItem7.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem15.Control = this.xtraTabControl1;
		this.layoutControlItem15.CustomizationFormText = "layoutControlItem15";
		this.layoutControlItem15.Location = new System.Drawing.Point(0, 168);
		this.layoutControlItem15.Name = "layoutControlItem15";
		this.layoutControlItem15.Size = new System.Drawing.Size(1070, 446);
		this.layoutControlItem15.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem15.TextVisible = false;
		this.layoutControlItem16.Control = this.btn_OK;
		this.layoutControlItem16.CustomizationFormText = "layoutControlItem16";
		this.layoutControlItem16.Location = new System.Drawing.Point(0, 614);
		this.layoutControlItem16.MaxSize = new System.Drawing.Size(119, 26);
		this.layoutControlItem16.MinSize = new System.Drawing.Size(119, 26);
		this.layoutControlItem16.Name = "layoutControlItem16";
		this.layoutControlItem16.Size = new System.Drawing.Size(119, 26);
		this.layoutControlItem16.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem16.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem16.TextVisible = false;
		this.emptySpaceItem9.AllowHotTrack = false;
		this.emptySpaceItem9.CustomizationFormText = "emptySpaceItem9";
		this.emptySpaceItem9.Location = new System.Drawing.Point(829, 614);
		this.emptySpaceItem9.Name = "emptySpaceItem9";
		this.emptySpaceItem9.Size = new System.Drawing.Size(241, 26);
		this.emptySpaceItem9.TextSize = new System.Drawing.Size(0, 0);
		this.emptySpaceItem11.AllowHotTrack = false;
		this.emptySpaceItem11.CustomizationFormText = "emptySpaceItem11";
		this.emptySpaceItem11.Location = new System.Drawing.Point(232, 614);
		this.emptySpaceItem11.Name = "emptySpaceItem11";
		this.emptySpaceItem11.Size = new System.Drawing.Size(597, 26);
		this.emptySpaceItem11.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem17.Control = this.btn_Cancel;
		this.layoutControlItem17.CustomizationFormText = "layoutControlItem17";
		this.layoutControlItem17.Location = new System.Drawing.Point(119, 614);
		this.layoutControlItem17.MaxSize = new System.Drawing.Size(113, 26);
		this.layoutControlItem17.MinSize = new System.Drawing.Size(113, 26);
		this.layoutControlItem17.Name = "layoutControlItem17";
		this.layoutControlItem17.Size = new System.Drawing.Size(113, 26);
		this.layoutControlItem17.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
		this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem17.TextVisible = false;
		this.layoutControlItem118.Control = this.ItemCode;
		this.layoutControlItem118.CustomizationFormText = "layoutControlItem118";
		this.layoutControlItem118.Location = new System.Drawing.Point(204, 0);
		this.layoutControlItem118.Name = "layoutControlItem118";
		this.layoutControlItem118.Size = new System.Drawing.Size(380, 24);
		this.layoutControlItem118.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem118.TextVisible = false;
		this.emptySpaceItem1.AllowHotTrack = false;
		this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
		this.emptySpaceItem1.Location = new System.Drawing.Point(584, 0);
		this.emptySpaceItem1.Name = "emptySpaceItem1";
		this.emptySpaceItem1.Size = new System.Drawing.Size(120, 168);
		this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem13.Control = this.textEdit5;
		this.layoutControlItem13.CustomizationFormText = "layoutControlItem13";
		this.layoutControlItem13.Location = new System.Drawing.Point(428, 144);
		this.layoutControlItem13.Name = "layoutControlItem13";
		this.layoutControlItem13.Size = new System.Drawing.Size(156, 24);
		this.layoutControlItem13.TextSize = new System.Drawing.Size(0, 0);
		this.layoutControlItem13.TextVisible = false;
		this.textEdit24.EditValue = "textEdit24";
		this.textEdit24.FieldName = null;
		this.textEdit24.IsSearchControl = false;
		this.textEdit24.Location = new System.Drawing.Point(114, 298);
		this.textEdit24.Name = "textEdit24";
		this.textEdit24.Size = new System.Drawing.Size(899, 20);
		this.textEdit24.StyleController = this.layoutControl3;
		this.textEdit24.TabIndex = 10;
		this.layoutControlGroup10.CustomizationFormText = "LayoutRootGroupForRestore";
		this.layoutControlGroup10.GroupBordersVisible = false;
		this.layoutControlGroup10.Location = new System.Drawing.Point(0, 358);
		this.layoutControlGroup10.Name = "LayoutRootGroupForRestore";
		this.layoutControlGroup10.Size = new System.Drawing.Size(1040, 30);
		this.layoutControlGroup10.Tag = "LayoutRootGroupForRestore";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1090, 660);
		base.Controls.Add(this.layoutControl1);
		base.FormId = "oItems";
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_Items";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Items Master Data";
		base.Load += new System.EventHandler(this.frm_Items_Load);
		base.Shown += new System.EventHandler(this.frm_Items_Shown);
		((System.ComponentModel.ISupportInitialize)this.layoutControl1).EndInit();
		this.layoutControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl2).EndInit();
		this.layoutControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_InvalidRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_InvalidFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ValidRemark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FrozenComm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.frozenTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.frozenTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.frozenFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.frozenFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ValidComm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.validTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.validTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.validFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.validFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ManOutOnly.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SWW.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.NoDiscount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FirmCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ShipType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ManBtchNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MngMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.WarrntTmpl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Root).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem20).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem21).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem105).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ManagementMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_warrantyTemplate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRemark).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRangeFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRangeTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRangeRemark).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRangeFrom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRangeTo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_InactiveRangeRemark).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_ActiveRemark).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem19).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem20).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl3).EndInit();
		this.layoutControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.textEdit38.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Rate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.TotalTax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PurPackMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SuppCatNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BuyUnitMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.NumInBuy.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PurPackUn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CstGrpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.VatGroupPu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit25.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit26.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit27.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BVolume.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit29.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PurFactor4.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem22).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem24).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem25).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem30).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem31).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem39).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem23).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem28).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem29).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem32).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem40).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem41).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem42).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem43).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem33).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem34).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem35).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem45).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem36).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem40).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem46).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem47).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem48).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem49).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem50).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem27).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem51).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem37).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem38).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem52).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem39).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl4).EndInit();
		this.layoutControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Rate2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalPackMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.VatGourpSa.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalUnitMsr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.NumInSale.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalPackUn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit44.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit45.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit46.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SVolume.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit48.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SalFactor4.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit53.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem41).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem42).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem53).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem43).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem44).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem54).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem55).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem45).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem56).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem57).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem46).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem48).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem58).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem59).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem60).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem61).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem62).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem49).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem63).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem64).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem65).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem66).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem50).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem51).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem67).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem52).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem68).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem69).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem47).EndInit();
		this.xtraTabPage4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl5).EndInit();
		this.layoutControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.DfltWH.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_oitw).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_oitw).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Locked).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ByWh.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.GLMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.InvntryUom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit57.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.EvalSystem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.AvgPrice.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ReorderQty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MinLevel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MaxLevel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem53).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem54).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem55).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem70).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem71).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem56).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem72).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem57).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem73).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem74).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem58).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem75).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem76).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem77).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem78).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem79).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem80).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem117).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem14).EndInit();
		this.xtraTabPage5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl6).EndInit();
		this.layoutControl6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.PlaningSys.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PrcrmntMtd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.OrdrIntrvl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.OrdrMulti.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.MinOrdrQty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LeadTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ToleranDay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem62).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem81).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem82).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem64).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem83).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem84).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem85).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem65).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem86).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem87).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem66).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem88).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem89).EndInit();
		this.xtraTabPage6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl7).EndInit();
		this.layoutControl7.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.Phantom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.IssueMthd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.TreeType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit71.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit72.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem68).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem69).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem90).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem91).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem70).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem92).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem93).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem94).EndInit();
		this.xtraTabPage7.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl8).EndInit();
		this.layoutControl8.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem95).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem71).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem96).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem97).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem10).EndInit();
		this.xtraTabPage8.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl9).EndInit();
		this.layoutControl9.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.UserText.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem98).EndInit();
		this.xtraTabPage9.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl10).EndInit();
		this.layoutControl10.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridControl3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem77).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem100).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem101).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem102).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem103).EndInit();
		this.tab_UDF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl_Udf).EndInit();
		this.layoutControl_Udf.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.layoutControl12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_UdfRight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControl11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_UdfLeft).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layout_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem18).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem26).EndInit();
		((System.ComponentModel.ISupportInitialize)this.SellItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.InvntItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.PrchseItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.FrgnName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ItemName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Series.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ItemType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ItmsGrpCod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.UgpEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ListName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_PriceType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit5.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.CodeBars.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem15).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem16).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem17).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem118).EndInit();
		((System.ComponentModel.ISupportInitialize)this.emptySpaceItem1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlItem13).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit24.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.layoutControlGroup10).EndInit();
		base.ResumeLayout(false);
	}
}
