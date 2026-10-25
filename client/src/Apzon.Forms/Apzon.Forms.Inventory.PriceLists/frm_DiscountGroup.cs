using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Inventory.PriceLists;

public class frm_DiscountGroup : XtraFormEx
{
	private string _privilege = "N";

	private string _formController = "DiscountGroup";

	private RepositoryItemLookUpEdit luk_Customer;

	private RepositoryItemLookUpEdit luk_Vendor;

	private static readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btn_Add;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabp_ItemGroup;

	private XtraTabPageEx tabp_Properties;

	private XtraTabPageEx tabp_Manufacturers;

	private XtraTabPageEx tabp_Items;

	private LabelLine lb_Find;

	private LabelLine lbl_BPCode;

	private LabelLine lbl_BPName;

	private ComboBoxEditEx cb_Type;

	private TextEditEx txt_BPName;

	private SimpleButtonEx btn_Search;

	private LabelLine lbl_GroupNum;

	private LookUpEdit lk_Group;

	private GridControlEx grd_ItemGroups;

	private GridViewEx gv_ItemGroups;

	private SimpleButtonEx btn_ItemGroupsReset;

	private SimpleButtonEx btn_ItemGroupsNew;

	private SimpleButtonEx btn_PropertiesReset;

	private GridControlEx grd_Properties;

	private GridViewEx gv_Properties;

	private SimpleButtonEx btn_ManufacturersReset;

	private SimpleButtonEx btn_ManufacturersNew;

	private GridControlEx grd_Manufacturers;

	private GridViewEx gv_Manufacturers;

	private SimpleButtonEx btn_ItemsReset;

	private SimpleButtonEx btn_ItemsAdd;

	private GridControlEx grd_Items;

	private GridViewEx gv_Items;

	private SimpleButtonEx btn_ItemsClearTable;

	private ButtonEditEx bte_BPCode;

	private GridColumnEx gc_AbsEntry;

	private GridColumnEx gc_Type;

	private GridColumnEx gc_ObjType;

	private GridColumnEx gc_ObjCode;

	private GridColumnEx gc_DiscRel;

	private GridColumnEx gc_ValidFor;

	private GridColumnEx gc_ValidForm;

	private GridColumnEx gc_ValidTo;

	private GridColumnEx gc_DataSource;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_LogInstanc;

	private GridColumnEx gc_UserSign2;

	private GridColumnEx gc_CreateDate;

	private GridColumnEx gc_UpdateDate;

	private RepositoryItemLookUpEdit rep_luk_grd_DocType;

	private GridColumnEx gc_GroupCode;

	private GridColumnEx gc_CardCode;

	private GridColumnEx gc_CardName;

	private RepositoryItemButtonEdit rep_BP;

	private GridColumnEx gc_ItmsGrpCod;

	private GridColumnEx gc_ItmsGrpNam;

	private GridColumnEx gc_ItemsGroup_Discount;

	private GridColumnEx gc_ItmsTypCod;

	private GridColumnEx gc_Properties_ItmsGrpNam;

	private GridColumnEx gc_Properties_Discount;

	private GridColumnEx gc_FirmCode;

	private GridColumnEx gc_FirmName;

	private GridColumnEx gc_Manufacturer_Discount;

	private GridColumnEx gc_Items_ItemCode;

	private GridColumnEx gc_ItemName;

	private GridColumnEx gc_Items_Discount;

	private RepositoryItemLookUpEdit rep_luk_DiscRel;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_Add.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Add.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Add.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_DiscountGroup()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DiscountGroup"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_DiscountGroup" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async void frm_DiscountGroup_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			cb_Type.SelectedIndex = 0;
			SetFormatControls();
			await CreateLookUpEdit();
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			int entry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			if (entry > 0)
			{
				await LoadTabpagesData();
				await SearchLine(entry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cb_Type_SelectedIndexChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (cb_Type.SelectedIndex)
			{
			case 0:
				lbl_BPCode.Visible = false;
				lbl_BPName.Visible = false;
				lbl_GroupNum.Visible = false;
				lk_Group.Visible = false;
				bte_BPCode.Visible = false;
				txt_BPName.Visible = false;
				break;
			case 1:
				lbl_BPCode.Visible = false;
				lbl_BPName.Visible = false;
				lbl_GroupNum.Visible = true;
				lbl_GroupNum.Top = 14;
				lk_Group.Visible = true;
				lk_Group.Top = 12;
				bte_BPCode.Visible = false;
				txt_BPName.Visible = false;
				await LoadComboGroup();
				break;
			case 2:
				lbl_BPCode.Visible = false;
				lbl_BPName.Visible = false;
				lbl_GroupNum.Visible = true;
				lbl_GroupNum.Top = 14;
				lk_Group.Visible = true;
				lk_Group.Top = 12;
				bte_BPCode.Visible = false;
				txt_BPName.Visible = false;
				await LoadComboGroup();
				break;
			case 3:
				lbl_BPCode.Visible = true;
				lbl_BPName.Visible = true;
				lbl_GroupNum.Visible = false;
				lk_Group.Visible = false;
				bte_BPCode.Visible = true;
				txt_BPName.Visible = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_luk_DiscRel_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_BP_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_luk_grd_DocType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_ItemGroups_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Properties_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Manufacturers_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Items_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			ClearSearch();
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ItemGroupsNew_Click(object sender, EventArgs e)
	{
	}

	private void btn_ItemGroupsReset_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_ItemGroups.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Discount"] = 0;
				}
				dataTable.AcceptChanges();
				grd_ItemGroups.DataSource = dataTable;
				grd_ItemGroups.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Add_Click(object sender, EventArgs e)
	{
		DisableControls();
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update)
			{
				EnableControls();
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				EnableControls();
				return;
			}
			string type = Function.ToString(gv_Doc.GetFocusedRowCellValue("Type"));
			if (type == "S" && string.IsNullOrWhiteSpace(Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"))))
			{
				WinCommons.Error(Enums.MessageCode.NoBussinessSpecify, "");
				EnableControls();
				return;
			}
			if ((type == "C" || type == "V") && Function.ParseInt(gv_Doc.GetFocusedRowCellValue("GroupCode")) < 1)
			{
				WinCommons.Error(Enums.MessageCode.NoBpGroupSpecify, "");
				EnableControls();
				return;
			}
			DataSet ds = new DataSet();
			ds.Tables.Clear();
			if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentToSave, "");
				EnableControls();
				return;
			}
			DataRow docS = dataTable.Rows[gv_Doc.GetFocusedDataSourceRowIndex()];
			DataTable dtDoc = await GenDocTable();
			dtDoc.Rows.Clear();
			DataRow doc = dtDoc.NewRow();
			switch (type)
			{
			case "A":
				doc["Type"] = "A";
				doc["ObjType"] = "-1";
				doc["ObjCode"] = "0";
				break;
			case "C":
				doc["Type"] = "C";
				doc["ObjType"] = "10";
				doc["ObjCode"] = Function.ToString(docS["GroupCode"]);
				break;
			case "V":
				doc["Type"] = "V";
				doc["ObjType"] = "10";
				doc["ObjCode"] = Function.ToString(docS["GroupCode"]);
				break;
			case "S":
				doc["Type"] = "S";
				doc["ObjType"] = "2";
				doc["ObjCode"] = Function.ToString(docS["CardCode"]);
				break;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				if (await GetAllBPEntry() > 0 && type == "A")
				{
					WinCommons.Error(Enums.MessageCode.AllBpRecordExistsCannotInsertAgain, "");
					EnableControls();
					return;
				}
				doc["AbsEntry"] = 0;
			}
			else
			{
				doc["AbsEntry"] = Function.ParseInt(docS["AbsEntry"]);
			}
			doc["DiscRel"] = Function.ToString(docS["DiscRel"]);
			doc["ValidFor"] = "Y";
			doc["CreateDate"] = DateTime.Now.ToString("yyyy-MM-dd");
			dtDoc.Rows.Add(doc);
			dtDoc.AcceptChanges();
			ds.Tables.Add(dtDoc);
			DataTable dataTable2 = await GenLinesTable();
			dataTable2.Rows.Clear();
			if (grd_ItemGroups.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable3.Rows.Count; i++)
				{
					if (Function.ParseInt(dataTable3.Rows[i]["Discount"]) > 0)
					{
						DataRow dataRow = dataTable2.NewRow();
						dataRow["ObjType"] = "52";
						dataRow["ObjKey"] = Function.ToString(dataTable3.Rows[i]["ItmsGrpCod"]);
						dataRow["Discount"] = Function.ParseDecimal(dataTable3.Rows[i]["Discount"]);
						dataTable2.Rows.Add(dataRow);
					}
				}
				dataTable2.AcceptChanges();
			}
			if (grd_Properties.DataSource is DataTable dataTable4 && dataTable4.Rows.Count > 0)
			{
				for (int j = 0; j < dataTable4.Rows.Count; j++)
				{
					if (Function.ParseInt(dataTable4.Rows[j]["Discount"]) > 0)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2["ObjType"] = "8";
						dataRow2["ObjKey"] = Function.ToString(dataTable4.Rows[j]["ItmsTypCod"]);
						dataRow2["Discount"] = Function.ParseDecimal(dataTable4.Rows[j]["Discount"]);
						dataTable2.Rows.Add(dataRow2);
					}
				}
				dataTable2.AcceptChanges();
			}
			if (grd_Manufacturers.DataSource is DataTable dataTable5 && dataTable5.Rows.Count > 0)
			{
				for (int k = 0; k < dataTable5.Rows.Count; k++)
				{
					if (Function.ParseInt(dataTable5.Rows[k]["Discount"]) > 0)
					{
						DataRow dataRow3 = dataTable2.NewRow();
						dataRow3["ObjType"] = "43";
						dataRow3["ObjKey"] = Function.ToString(dataTable5.Rows[k]["FirmCode"]);
						dataRow3["Discount"] = Function.ParseDecimal(dataTable5.Rows[k]["Discount"]);
						dataTable2.Rows.Add(dataRow3);
					}
				}
				dataTable2.AcceptChanges();
			}
			if (grd_Items.DataSource is DataTable dataTable6 && dataTable6.Rows.Count > 0)
			{
				for (int l = 0; l < dataTable6.Rows.Count; l++)
				{
					if (Function.ParseInt(dataTable6.Rows[l]["Discount"]) > 0)
					{
						DataRow dataRow4 = dataTable2.NewRow();
						dataRow4["ObjType"] = "4";
						dataRow4["ObjKey"] = Function.ToString(dataTable6.Rows[l]["ItemCode"]);
						dataRow4["Discount"] = Function.ParseDecimal(dataTable6.Rows[l]["Discount"]);
						dataTable2.Rows.Add(dataRow4);
					}
				}
				dataTable2.AcceptChanges();
			}
			ds.Tables.Add(dataTable2);
			string text = ((!GlobalData.LoginSap) ? (await SaveDoc(ds)) : (await SaveSap(ds)));
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				btn_Search_Click(null, null);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableControls();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_ItemsClearTable_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Items.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
				grd_Items.DataSource = dataTable;
				grd_Items.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ItemsAdd_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDocumentToSave, "");
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ItemDescription";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			DataTable dataTable2 = grd_Items.DataSource as DataTable;
			if (dataTable2 == null || dataTable2.Columns.Count == 0)
			{
				dataTable2 = GetTabPageItemColumns();
			}
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[i];
				if (dataTable2.Select("[ItemCode]='" + Function.ToString(dataRow["ItemCode"]).Trim() + "'") != null)
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["ItemCode"] = Function.ToString(dataRow["ItemCode"]);
					dataRow2["ItemName"] = Function.ToString(dataRow["ItemDescription"]);
					dataRow2["Discount"] = 0;
					dataTable2.Rows.Add(dataRow2);
				}
			}
			dataTable2.AcceptChanges();
			grd_Items.DataSource = dataTable2;
			grd_Items.RefreshDataSource();
			if (FormMode == Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Add && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ItemsReset_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Items.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Discount"] = 0;
				}
				dataTable.AcceptChanges();
				grd_Items.DataSource = dataTable;
				grd_Items.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_PropertiesReset_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Discount"] = 0;
				}
				dataTable.AcceptChanges();
				grd_Properties.DataSource = dataTable;
				grd_Properties.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_ManufacturersNew_Click(object sender, EventArgs e)
	{
	}

	private void btn_ManufacturersReset_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_Manufacturers.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Discount"] = 0;
				}
				dataTable.AcceptChanges();
				grd_Manufacturers.DataSource = dataTable;
				grd_Manufacturers.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "GroupCode")
			{
				return;
			}
			string text = ((gv_Doc.GetRowCellValue(e.RowHandle, "Type") == DBNull.Value || gv_Doc.GetRowCellValue(e.RowHandle, "Type") == null) ? "" : gv_Doc.GetRowCellValue(e.RowHandle, "Type").ToString());
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			string text2 = text.ToUpper();
			if (!(text2 == "C"))
			{
				if (text2 == "V")
				{
					e.RepositoryItem = luk_Vendor;
				}
			}
			else
			{
				e.RepositoryItem = luk_Customer;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "GroupCode")
			{
				return;
			}
			string text = ((gv_Doc.GetRowCellValue(e.RowHandle, "Type") == DBNull.Value || gv_Doc.GetRowCellValue(e.RowHandle, "Type") == null) ? "" : gv_Doc.GetRowCellValue(e.RowHandle, "Type").ToString());
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			string text2 = text.ToUpper();
			if (!(text2 == "C"))
			{
				if (text2 == "V")
				{
					e.RepositoryItem = luk_Vendor;
				}
			}
			else
			{
				e.RepositoryItem = luk_Customer;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_BP_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null)
				{
					gv_Doc.SetFocusedRowCellValue("CardCode", Function.ToString(chooseItem["CardCode"]));
					gv_Doc.SetFocusedRowCellValue("CardName", Function.ToString(chooseItem["CardName"]));
				}
			}
			else if (!string.IsNullOrWhiteSpace(text))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = Function.ToString(bte_BPCode.EditValue);
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void buttonEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, Function.ToString(bte_BPCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null)
				{
					bte_BPCode.EditValue = Function.ToString(chooseItem["CardCode"]);
					txt_BPName.EditValue = Function.ToString(chooseItem["CardName"]);
				}
			}
			else if (!string.IsNullOrWhiteSpace(Function.ToString(bte_BPCode.EditValue)))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = Function.ToString(bte_BPCode.EditValue);
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && (gv_Doc.FocusedColumn.FieldName.ToUpper() == "TYPE" || gv_Doc.FocusedColumn.FieldName.ToUpper() == "GROUPCODE" || gv_Doc.FocusedColumn.FieldName.ToUpper() == "CARDCODE"))
			{
				e.Cancel = true;
			}
			switch (Function.ToString(gv_Doc.GetFocusedRowCellValue("Type")))
			{
			case "A":
				gv_Doc.Columns["ObjCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardName"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardCode"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["GroupCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["ObjCode"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["CardName"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["GroupCode"].OptionsColumn.ReadOnly = true;
				break;
			case "C":
				gv_Doc.Columns["ObjCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardName"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardCode"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["GroupCode"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["ObjCode"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["CardName"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["GroupCode"].OptionsColumn.ReadOnly = false;
				break;
			case "V":
				gv_Doc.Columns["ObjCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardName"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["CardCode"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["GroupCode"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["ObjCode"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["CardName"].OptionsColumn.ReadOnly = true;
				gv_Doc.Columns["GroupCode"].OptionsColumn.ReadOnly = false;
				break;
			case "S":
				gv_Doc.Columns["ObjCode"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["CardName"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["CardCode"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["CardCode"].OptionsColumn.ReadOnly = false;
				gv_Doc.Columns["GroupCode"].OptionsColumn.AllowEdit = false;
				gv_Doc.Columns["ObjCode"].OptionsColumn.ReadOnly = false;
				gv_Doc.Columns["CardName"].OptionsColumn.ReadOnly = false;
				gv_Doc.Columns["GroupCode"].OptionsColumn.ReadOnly = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GenDocTable()
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			dt = await hp.GetAsync("GenDocTable");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	private async Task<DataTable> GenLinesTable()
	{
		return await GetTableColumns("EDG1");
	}

	private async Task<DataTable> GetTableColumns(string table)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			dt = await hp.GetAsync("GetTableColumns", new { table });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	private DataTable GetTabPageItemColumns()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("ItemName", typeof(string));
			dataTable.Columns.Add("Discount", typeof(decimal));
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
	}

	private void EnableControls()
	{
		try
		{
			btn_Add.Enabled = true;
			btn_Cancel.Enabled = true;
			btn_Search.Enabled = true;
			btn_ItemGroupsNew.Enabled = true;
			btn_ItemGroupsReset.Enabled = true;
			btn_PropertiesReset.Enabled = true;
			btn_ManufacturersNew.Enabled = true;
			btn_ManufacturersReset.Enabled = true;
			btn_ItemsAdd.Enabled = true;
			btn_ItemsClearTable.Enabled = true;
			btn_ItemsReset.Enabled = true;
			bte_BPCode.Enabled = true;
			lk_Group.Enabled = true;
			cb_Type.Enabled = true;
			grd_Doc.Enabled = true;
			grd_ItemGroups.Enabled = true;
			grd_Properties.Enabled = true;
			grd_Items.Enabled = true;
			grd_Manufacturers.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DisableControls()
	{
		try
		{
			btn_Add.Enabled = false;
			btn_Cancel.Enabled = false;
			btn_Search.Enabled = false;
			btn_ItemGroupsNew.Enabled = false;
			btn_ItemGroupsReset.Enabled = false;
			btn_PropertiesReset.Enabled = false;
			btn_ManufacturersNew.Enabled = false;
			btn_ManufacturersReset.Enabled = false;
			btn_ItemsAdd.Enabled = false;
			btn_ItemsClearTable.Enabled = false;
			btn_ItemsReset.Enabled = false;
			bte_BPCode.Enabled = false;
			lk_Group.Enabled = false;
			cb_Type.Enabled = false;
			grd_Doc.Enabled = false;
			grd_ItemGroups.Enabled = false;
			grd_Properties.Enabled = false;
			grd_Items.Enabled = false;
			grd_Manufacturers.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Add;
				ResetControls();
				AddNewDoc();
				await LoadTabpagesData();
				_ = grd_Items.DataSource is DataTable;
				DataTable tabPageItemColumns = GetTabPageItemColumns();
				grd_Items.DataSource = tabPageItemColumns;
				grd_Items.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetControls()
	{
		try
		{
			cb_Type.SelectedIndex = 0;
			bte_BPCode.EditValue = "";
			txt_BPName.Text = "";
			lbl_BPCode.Visible = false;
			lbl_BPName.Visible = false;
			lbl_GroupNum.Visible = false;
			lk_Group.Visible = false;
			bte_BPCode.Visible = false;
			txt_BPName.Visible = false;
			btn_ItemGroupsReset_Click(null, null);
			btn_ItemsClearTable_Click(null, null);
			btn_PropertiesReset_Click(null, null);
			btn_ManufacturersNew_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadComboGroup()
	{
		try
		{
			string text = "";
			switch (cb_Type.SelectedIndex)
			{
			case 1:
				text = "C";
				break;
			case 2:
				text = "S";
				break;
			}
			if (!string.IsNullOrWhiteSpace(text))
			{
				RepositoryItemLookUpEdit properties = lk_Group.Properties;
				properties.DataSource = await LoadComboGroup(text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> LoadComboGroup(string groupType)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessMasterData");
			dt = await hp.GetAsync("ShowCardGroup", new { groupType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dt;
	}

	private async Task CreateLookUpEdit()
	{
		_ = 1;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Name"] = "All BPs";
			dataRow["Code"] = "A";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Name"] = "Customer Group";
			dataRow2["Code"] = "C";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["Name"] = "Vendor Group";
			dataRow3["Code"] = "V";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4["Name"] = "Specific BP";
			dataRow4["Code"] = "S";
			dataTable.Rows.Add(dataRow4);
			dataTable.AcceptChanges();
			rep_luk_grd_DocType.DataSource = dataTable;
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Name", typeof(string));
			dataTable2.Columns.Add("Code", typeof(string));
			DataRow dataRow5 = dataTable2.NewRow();
			dataRow5["Name"] = "Lowest Discount";
			dataRow5["Code"] = "L";
			dataTable2.Rows.Add(dataRow5);
			DataRow dataRow6 = dataTable2.NewRow();
			dataRow6["Name"] = "Highest Discount";
			dataRow6["Code"] = "H";
			dataTable2.Rows.Add(dataRow6);
			DataRow dataRow7 = dataTable2.NewRow();
			dataRow7["Name"] = "Average";
			dataRow7["Code"] = "A";
			dataTable2.Rows.Add(dataRow7);
			DataRow dataRow8 = dataTable2.NewRow();
			dataRow8["Name"] = "Total";
			dataRow8["Code"] = "S";
			dataTable2.Rows.Add(dataRow8);
			DataRow dataRow9 = dataTable2.NewRow();
			dataRow9["Name"] = "Discount Multiples";
			dataRow9["Code"] = "M";
			dataTable2.Rows.Add(dataRow9);
			dataTable2.AcceptChanges();
			rep_luk_DiscRel.DataSource = dataTable2;
			luk_Customer = new RepositoryItemLookUpEdit();
			LookUpColumnInfo lookUpColumnInfo = new LookUpColumnInfo();
			lookUpColumnInfo.FieldName = "GroupName";
			lookUpColumnInfo.Caption = "Group Name";
			luk_Customer.Columns.Add(lookUpColumnInfo);
			luk_Customer.NullText = "";
			luk_Customer.DisplayMember = "GroupName";
			luk_Customer.ValueMember = "GroupCode";
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = luk_Customer;
			repositoryItemLookUpEdit.DataSource = await LoadComboGroup("C");
			luk_Vendor = new RepositoryItemLookUpEdit();
			LookUpColumnInfo lookUpColumnInfo2 = new LookUpColumnInfo();
			lookUpColumnInfo2.FieldName = "GroupName";
			lookUpColumnInfo2.Caption = "Group Name";
			luk_Vendor.Columns.Add(lookUpColumnInfo2);
			luk_Vendor.NullText = "";
			luk_Vendor.DisplayMember = "GroupName";
			luk_Vendor.ValueMember = "GroupCode";
			repositoryItemLookUpEdit = luk_Vendor;
			repositoryItemLookUpEdit.DataSource = await LoadComboGroup("S");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SearchLine(int entry)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dt = await hp.GetAsync("SearchLine", new { entry });
			DataTable dataTable = grd_ItemGroups.DataSource as DataTable;
			DataTable dataTable2 = grd_Properties.DataSource as DataTable;
			DataTable dataTable3 = grd_Manufacturers.DataSource as DataTable;
			DataTable tabPageItemColumns = GetTabPageItemColumns();
			if (dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			int i;
			for (i = 0; i < dt.Rows.Count; i++)
			{
				switch (Function.ToString(dt.Rows[i]["ObjType"]))
				{
				case "52":
					dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItmsGrpCod"]) == Function.ToString(dt.Rows[i]["ObjKey"]))["Discount"] = Function.ParseDecimal(dt.Rows[i]["Discount"]);
					dataTable.AcceptChanges();
					break;
				case "8":
					dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItmsTypCod"]) == Function.ToString(dt.Rows[i]["ObjKey"]))["Discount"] = Function.ParseDecimal(dt.Rows[i]["Discount"]);
					dataTable2.AcceptChanges();
					break;
				case "43":
					dataTable3.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FirmCode"]) == Function.ToString(dt.Rows[i]["ObjKey"]))["Discount"] = Function.ParseDecimal(dt.Rows[i]["Discount"]);
					dataTable3.AcceptChanges();
					break;
				case "4":
				{
					DataRow dataRow = tabPageItemColumns.NewRow();
					dataRow["ItemCode"] = Function.ToString(dt.Rows[i]["ObjKey"]);
					DataTable itemByItemCode = ExtenseionMethods.GetItemByItemCode(Function.ToString(dataRow["ItemCode"]));
					if (itemByItemCode != null && itemByItemCode.Rows.Count > 0)
					{
						dataRow["ItemName"] = Function.ToString(itemByItemCode.Rows[0]["ItemName"]);
					}
					dataRow["Discount"] = Function.ParseDecimal(dt.Rows[i]["Discount"]);
					tabPageItemColumns.Rows.Add(dataRow);
					tabPageItemColumns.AcceptChanges();
					break;
				}
				}
			}
			grd_ItemGroups.DataSource = dataTable;
			grd_ItemGroups.RefreshDataSource();
			grd_Properties.DataSource = dataTable2;
			grd_Properties.RefreshDataSource();
			grd_Manufacturers.DataSource = dataTable3;
			grd_Manufacturers.RefreshDataSource();
			grd_Items.DataSource = tabPageItemColumns;
			grd_Items.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search()
	{
		_isLoading = true;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("ObjCode", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			switch (cb_Type.SelectedIndex)
			{
			case 0:
				dataRow["Type"] = "A";
				break;
			case 1:
				dataRow["Type"] = "C";
				dataRow["ObjCode"] = Function.ToString(lk_Group.EditValue);
				break;
			case 2:
				dataRow["Type"] = "V";
				dataRow["ObjCode"] = Function.ToString(lk_Group.EditValue);
				break;
			case 3:
				dataRow["Type"] = "S";
				dataRow["ObjCode"] = Function.ToString(bte_BPCode.EditValue);
				break;
			default:
				dataRow["Type"] = "A";
				break;
			}
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable2 = await hp.PostAsync("Search", dataTable);
			grd_Doc.DataSource = dataTable2;
			grd_Doc.RefreshDataSource();
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				int entry = Function.ParseInt(dataTable2.Rows[0]["AbsEntry"]);
				await LoadTabpagesData();
				await SearchLine(entry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private async void AddNewDoc()
	{
		try
		{
			DataTable dataSource = await GenDocTable();
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveDoc(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, _formController);
			return httpClientHelper.Post("SaveDocNoSap", ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task<string> SaveSap(DataSet ds)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			return httpClientHelper.Post("SaveDoc", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task LoadTabpagesData()
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("LoadTabPagesData");
			if (dataSet.Tables[0] != null)
			{
				grd_ItemGroups.DataSource = dataSet.Tables[0];
				grd_ItemGroups.RefreshDataSource();
			}
			if (dataSet.Tables[1] != null)
			{
				grd_Properties.DataSource = dataSet.Tables[1];
				grd_Properties.RefreshDataSource();
			}
			if (dataSet.Tables[2] != null)
			{
				grd_Manufacturers.DataSource = dataSet.Tables[2];
				grd_Manufacturers.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<int> GetAllBPEntry()
	{
		int ret = 0;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = await hp.GetAsync("GetAllBPEntry");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret = Function.ParseInt(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return ret;
	}

	private void ClearSearch()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
			if (grd_ItemGroups.DataSource is DataTable dataTable2)
			{
				dataTable2.Rows.Clear();
				dataTable2.AcceptChanges();
				grd_ItemGroups.DataSource = dataTable2;
				grd_ItemGroups.RefreshDataSource();
			}
			if (grd_Properties.DataSource is DataTable dataTable3)
			{
				dataTable3.Rows.Clear();
				dataTable3.AcceptChanges();
				grd_Properties.DataSource = dataTable3;
				grd_Properties.RefreshDataSource();
			}
			if (grd_Manufacturers.DataSource is DataTable dataTable4)
			{
				dataTable4.Rows.Clear();
				dataTable4.AcceptChanges();
				grd_Manufacturers.DataSource = dataTable4;
				grd_Manufacturers.RefreshDataSource();
			}
			if (grd_Items.DataSource is DataTable dataTable5)
			{
				dataTable5.Rows.Clear();
				dataTable5.AcceptChanges();
				grd_Items.DataSource = dataTable5;
				grd_Items.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControls()
	{
		try
		{
			gc_Items_Discount.DisplayFormat.FormatType = FormatType.Numeric;
			gc_Items_Discount.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			gc_ItemsGroup_Discount.DisplayFormat.FormatType = FormatType.Numeric;
			gc_ItemsGroup_Discount.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			gc_Properties_Discount.DisplayFormat.FormatType = FormatType.Numeric;
			gc_Properties_Discount.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
			gc_Manufacturer_Discount.DisplayFormat.FormatType = FormatType.Numeric;
			gc_Manufacturer_Discount.DisplayFormat.FormatString = "n" + GlobalData.TotalsAccuracyFormat.NumberDecimalDigits;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.PriceLists.frm_DiscountGroup));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_AbsEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_grd_DocType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ObjType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ObjCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DiscRel = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_DiscRel = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_ValidFor = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ValidForm = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ValidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_LogInstanc = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_GroupCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BP = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabp_ItemGroup = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ItemGroupsReset = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ItemGroupsNew = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_ItemGroups = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ItemGroups = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ItmsGrpCod = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ItmsGrpNam = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ItemsGroup_Discount = new Apzon.Controls.Grid.GridColumnEx();
		this.tabp_Properties = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_PropertiesReset = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Properties = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Properties = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ItmsTypCod = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Properties_ItmsGrpNam = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Properties_Discount = new Apzon.Controls.Grid.GridColumnEx();
		this.tabp_Manufacturers = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ManufacturersReset = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ManufacturersNew = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Manufacturers = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Manufacturers = new Apzon.Controls.Grid.GridViewEx();
		this.gc_FirmCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FirmName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Manufacturer_Discount = new Apzon.Controls.Grid.GridColumnEx();
		this.tabp_Items = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ItemsClearTable = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ItemsReset = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ItemsAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Items = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Items = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Items_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Items_Discount = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.lbl_BPCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_BPName = new Apzon.Controls.Controls.LabelLine();
		this.cb_Type = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_BPName = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_GroupNum = new Apzon.Controls.Controls.LabelLine();
		this.lk_Group = new DevExpress.XtraEditors.LookUpEdit();
		this.bte_BPCode = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_grd_DocType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_DiscRel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabp_ItemGroup.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ItemGroups).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ItemGroups).BeginInit();
		this.tabp_Properties.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).BeginInit();
		this.tabp_Manufacturers.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Manufacturers).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Manufacturers).BeginInit();
		this.tabp_Items.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Group.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(12, 38);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(706, 384);
		this.splitContainerControl1.SplitterPosition = 179;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_luk_grd_DocType, this.rep_BP, this.rep_luk_DiscRel });
		this.grd_Doc.Size = new System.Drawing.Size(706, 179);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.gc_AbsEntry, this.gc_Type, this.gc_ObjType, this.gc_ObjCode, this.gc_DiscRel, this.gc_ValidFor, this.gc_ValidForm, this.gc_ValidTo, this.gc_DataSource, this.gc_UserSign,
			this.gc_LogInstanc, this.gc_UserSign2, this.gc_CreateDate, this.gc_UpdateDate, this.gc_GroupCode, this.gc_CardCode, this.gc_CardName
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oDiscountGroup";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OEDG";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gc_AbsEntry.Caption = "Unique Entry";
		this.gc_AbsEntry.CurrencyDateField = null;
		this.gc_AbsEntry.CurrencyField = null;
		this.gc_AbsEntry.CurrencyFixed = null;
		this.gc_AbsEntry.CurrencyFixedRate = 0.0;
		this.gc_AbsEntry.CurrencyRateField = null;
		this.gc_AbsEntry.FieldName = "AbsEntry";
		this.gc_AbsEntry.Name = "gc_AbsEntry";
		this.gc_AbsEntry.PrimaryKey = false;
		this.gc_AbsEntry.UnitField = null;
		this.gc_Type.Caption = "Type";
		this.gc_Type.ColumnEdit = this.rep_luk_grd_DocType;
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "Type";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.PrimaryKey = false;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 0;
		this.rep_luk_grd_DocType.AutoHeight = false;
		this.rep_luk_grd_DocType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_grd_DocType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_grd_DocType.DisplayMember = "Name";
		this.rep_luk_grd_DocType.Name = "rep_luk_grd_DocType";
		this.rep_luk_grd_DocType.NullText = "";
		this.rep_luk_grd_DocType.ValueMember = "Code";
		this.rep_luk_grd_DocType.EditValueChanged += new System.EventHandler(this.rep_luk_grd_DocType_EditValueChanged);
		this.gc_ObjType.Caption = "Object Type";
		this.gc_ObjType.CurrencyDateField = null;
		this.gc_ObjType.CurrencyField = null;
		this.gc_ObjType.CurrencyFixed = null;
		this.gc_ObjType.CurrencyFixedRate = 0.0;
		this.gc_ObjType.CurrencyRateField = null;
		this.gc_ObjType.FieldName = "ObjType";
		this.gc_ObjType.Name = "gc_ObjType";
		this.gc_ObjType.PrimaryKey = false;
		this.gc_ObjType.UnitField = null;
		this.gc_ObjCode.Caption = "Object Code";
		this.gc_ObjCode.CurrencyDateField = null;
		this.gc_ObjCode.CurrencyField = null;
		this.gc_ObjCode.CurrencyFixed = null;
		this.gc_ObjCode.CurrencyFixedRate = 0.0;
		this.gc_ObjCode.CurrencyRateField = null;
		this.gc_ObjCode.FieldName = "ObjCode";
		this.gc_ObjCode.Name = "gc_ObjCode";
		this.gc_ObjCode.PrimaryKey = false;
		this.gc_ObjCode.UnitField = null;
		this.gc_DiscRel.Caption = "Disc. Relations";
		this.gc_DiscRel.ColumnEdit = this.rep_luk_DiscRel;
		this.gc_DiscRel.CurrencyDateField = null;
		this.gc_DiscRel.CurrencyField = null;
		this.gc_DiscRel.CurrencyFixed = null;
		this.gc_DiscRel.CurrencyFixedRate = 0.0;
		this.gc_DiscRel.CurrencyRateField = null;
		this.gc_DiscRel.FieldName = "DiscRel";
		this.gc_DiscRel.Name = "gc_DiscRel";
		this.gc_DiscRel.PrimaryKey = false;
		this.gc_DiscRel.UnitField = null;
		this.gc_DiscRel.Visible = true;
		this.gc_DiscRel.VisibleIndex = 1;
		this.gc_DiscRel.Width = 131;
		this.rep_luk_DiscRel.AutoHeight = false;
		this.rep_luk_DiscRel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_DiscRel.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_DiscRel.DisplayMember = "Name";
		this.rep_luk_DiscRel.Name = "rep_luk_DiscRel";
		this.rep_luk_DiscRel.NullText = "";
		this.rep_luk_DiscRel.ValueMember = "Code";
		this.rep_luk_DiscRel.EditValueChanged += new System.EventHandler(this.rep_luk_DiscRel_EditValueChanged);
		this.gc_ValidFor.Caption = "Active";
		this.gc_ValidFor.CurrencyDateField = null;
		this.gc_ValidFor.CurrencyField = null;
		this.gc_ValidFor.CurrencyFixed = null;
		this.gc_ValidFor.CurrencyFixedRate = 0.0;
		this.gc_ValidFor.CurrencyRateField = null;
		this.gc_ValidFor.FieldName = "ValidFor";
		this.gc_ValidFor.Name = "gc_ValidFor";
		this.gc_ValidFor.PrimaryKey = false;
		this.gc_ValidFor.UnitField = null;
		this.gc_ValidForm.Caption = "Active From";
		this.gc_ValidForm.CurrencyDateField = null;
		this.gc_ValidForm.CurrencyField = null;
		this.gc_ValidForm.CurrencyFixed = null;
		this.gc_ValidForm.CurrencyFixedRate = 0.0;
		this.gc_ValidForm.CurrencyRateField = null;
		this.gc_ValidForm.FieldName = "ValidForm";
		this.gc_ValidForm.Name = "gc_ValidForm";
		this.gc_ValidForm.PrimaryKey = false;
		this.gc_ValidForm.UnitField = null;
		this.gc_ValidTo.Caption = "Active To";
		this.gc_ValidTo.CurrencyDateField = null;
		this.gc_ValidTo.CurrencyField = null;
		this.gc_ValidTo.CurrencyFixed = null;
		this.gc_ValidTo.CurrencyFixedRate = 0.0;
		this.gc_ValidTo.CurrencyRateField = null;
		this.gc_ValidTo.FieldName = "ValidTo";
		this.gc_ValidTo.Name = "gc_ValidTo";
		this.gc_ValidTo.PrimaryKey = false;
		this.gc_ValidTo.UnitField = null;
		this.gc_DataSource.Caption = "DataSource";
		this.gc_DataSource.CurrencyDateField = null;
		this.gc_DataSource.CurrencyField = null;
		this.gc_DataSource.CurrencyFixed = null;
		this.gc_DataSource.CurrencyFixedRate = 0.0;
		this.gc_DataSource.CurrencyRateField = null;
		this.gc_DataSource.FieldName = "DataSource";
		this.gc_DataSource.Name = "gc_DataSource";
		this.gc_DataSource.PrimaryKey = false;
		this.gc_DataSource.UnitField = null;
		this.gc_UserSign.Caption = "User Sign";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.gc_LogInstanc.Caption = "Log Instance";
		this.gc_LogInstanc.CurrencyDateField = null;
		this.gc_LogInstanc.CurrencyField = null;
		this.gc_LogInstanc.CurrencyFixed = null;
		this.gc_LogInstanc.CurrencyFixedRate = 0.0;
		this.gc_LogInstanc.CurrencyRateField = null;
		this.gc_LogInstanc.FieldName = "LogInstanc";
		this.gc_LogInstanc.Name = "gc_LogInstanc";
		this.gc_LogInstanc.PrimaryKey = false;
		this.gc_LogInstanc.UnitField = null;
		this.gc_UserSign2.Caption = "UserSign2";
		this.gc_UserSign2.CurrencyDateField = null;
		this.gc_UserSign2.CurrencyField = null;
		this.gc_UserSign2.CurrencyFixed = null;
		this.gc_UserSign2.CurrencyFixedRate = 0.0;
		this.gc_UserSign2.CurrencyRateField = null;
		this.gc_UserSign2.FieldName = "UserSign2";
		this.gc_UserSign2.Name = "gc_UserSign2";
		this.gc_UserSign2.PrimaryKey = false;
		this.gc_UserSign2.UnitField = null;
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.FieldName = "CreateDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.gc_UpdateDate.Caption = "Update Date";
		this.gc_UpdateDate.CurrencyDateField = null;
		this.gc_UpdateDate.CurrencyField = null;
		this.gc_UpdateDate.CurrencyFixed = null;
		this.gc_UpdateDate.CurrencyFixedRate = 0.0;
		this.gc_UpdateDate.CurrencyRateField = null;
		this.gc_UpdateDate.FieldName = "UpdateDate";
		this.gc_UpdateDate.Name = "gc_UpdateDate";
		this.gc_UpdateDate.PrimaryKey = false;
		this.gc_UpdateDate.UnitField = null;
		this.gc_GroupCode.Caption = "Group Name";
		this.gc_GroupCode.CurrencyDateField = null;
		this.gc_GroupCode.CurrencyField = null;
		this.gc_GroupCode.CurrencyFixed = null;
		this.gc_GroupCode.CurrencyFixedRate = 0.0;
		this.gc_GroupCode.CurrencyRateField = null;
		this.gc_GroupCode.FieldName = "GroupCode";
		this.gc_GroupCode.Name = "gc_GroupCode";
		this.gc_GroupCode.PrimaryKey = false;
		this.gc_GroupCode.UnitField = null;
		this.gc_GroupCode.Visible = true;
		this.gc_GroupCode.VisibleIndex = 2;
		this.gc_CardCode.Caption = "BP Code";
		this.gc_CardCode.ColumnEdit = this.rep_BP;
		this.gc_CardCode.CurrencyDateField = null;
		this.gc_CardCode.CurrencyField = null;
		this.gc_CardCode.CurrencyFixed = null;
		this.gc_CardCode.CurrencyFixedRate = 0.0;
		this.gc_CardCode.CurrencyRateField = null;
		this.gc_CardCode.FieldName = "CardCode";
		this.gc_CardCode.Name = "gc_CardCode";
		this.gc_CardCode.PrimaryKey = false;
		this.gc_CardCode.UnitField = null;
		this.gc_CardCode.Visible = true;
		this.gc_CardCode.VisibleIndex = 3;
		this.rep_BP.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BP.Name = "rep_BP";
		this.rep_BP.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BP_ButtonPressed);
		this.rep_BP.EditValueChanged += new System.EventHandler(this.rep_BP_EditValueChanged);
		this.gc_CardName.Caption = "BP Name";
		this.gc_CardName.CurrencyDateField = null;
		this.gc_CardName.CurrencyField = null;
		this.gc_CardName.CurrencyFixed = null;
		this.gc_CardName.CurrencyFixedRate = 0.0;
		this.gc_CardName.CurrencyRateField = null;
		this.gc_CardName.FieldName = "CardName";
		this.gc_CardName.Name = "gc_CardName";
		this.gc_CardName.PrimaryKey = false;
		this.gc_CardName.UnitField = null;
		this.gc_CardName.Visible = true;
		this.gc_CardName.VisibleIndex = 4;
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabp_ItemGroup;
		this.xtraTabControl1.Size = new System.Drawing.Size(706, 200);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[4] { this.tabp_ItemGroup, this.tabp_Properties, this.tabp_Manufacturers, this.tabp_Items });
		this.tabp_ItemGroup.Controls.Add(this.btn_ItemGroupsReset);
		this.tabp_ItemGroup.Controls.Add(this.btn_ItemGroupsNew);
		this.tabp_ItemGroup.Controls.Add(this.grd_ItemGroups);
		this.tabp_ItemGroup.EditControl = null;
		this.tabp_ItemGroup.EditValue = "Item Groups";
		this.tabp_ItemGroup.FieldName = null;
		this.tabp_ItemGroup.IsSearchControl = false;
		this.tabp_ItemGroup.Name = "tabp_ItemGroup";
		this.tabp_ItemGroup.ReadOnly = false;
		this.tabp_ItemGroup.SettingInVisible = false;
		this.tabp_ItemGroup.SettingReadonly = false;
		this.tabp_ItemGroup.Size = new System.Drawing.Size(704, 175);
		this.tabp_ItemGroup.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabp_ItemGroup.Text = "Item Groups";
		this.btn_ItemGroupsReset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ItemGroupsReset.EditControl = null;
		this.btn_ItemGroupsReset.EditValue = "Reset";
		this.btn_ItemGroupsReset.FieldName = null;
		this.btn_ItemGroupsReset.IsSearchControl = false;
		this.btn_ItemGroupsReset.Location = new System.Drawing.Point(579, 124);
		this.btn_ItemGroupsReset.Name = "btn_ItemGroupsReset";
		this.btn_ItemGroupsReset.ReadOnly = false;
		this.btn_ItemGroupsReset.SettingInVisible = false;
		this.btn_ItemGroupsReset.SettingReadonly = false;
		this.btn_ItemGroupsReset.Size = new System.Drawing.Size(104, 23);
		this.btn_ItemGroupsReset.TabIndex = 16;
		this.btn_ItemGroupsReset.Text = "Reset";
		this.btn_ItemGroupsReset.Click += new System.EventHandler(this.btn_ItemGroupsReset_Click);
		this.btn_ItemGroupsNew.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ItemGroupsNew.EditControl = null;
		this.btn_ItemGroupsNew.EditValue = "New Group";
		this.btn_ItemGroupsNew.FieldName = null;
		this.btn_ItemGroupsNew.IsSearchControl = false;
		this.btn_ItemGroupsNew.Location = new System.Drawing.Point(579, 95);
		this.btn_ItemGroupsNew.Name = "btn_ItemGroupsNew";
		this.btn_ItemGroupsNew.ReadOnly = false;
		this.btn_ItemGroupsNew.SettingInVisible = false;
		this.btn_ItemGroupsNew.SettingReadonly = false;
		this.btn_ItemGroupsNew.Size = new System.Drawing.Size(104, 23);
		this.btn_ItemGroupsNew.TabIndex = 15;
		this.btn_ItemGroupsNew.Text = "New Group";
		this.btn_ItemGroupsNew.Click += new System.EventHandler(this.btn_ItemGroupsNew_Click);
		this.grd_ItemGroups.AllowPasteExcel = true;
		this.grd_ItemGroups.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ItemGroups.ClipboardData = null;
		this.grd_ItemGroups.Location = new System.Drawing.Point(0, 0);
		this.grd_ItemGroups.MainView = this.gv_ItemGroups;
		this.grd_ItemGroups.Name = "grd_ItemGroups";
		this.grd_ItemGroups.Size = new System.Drawing.Size(554, 172);
		this.grd_ItemGroups.TabIndex = 1;
		this.grd_ItemGroups.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ItemGroups });
		this.gv_ItemGroups.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ItemGroups.ColumnKey = null;
		this.gv_ItemGroups.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_ItmsGrpCod, this.gc_ItmsGrpNam, this.gc_ItemsGroup_Discount });
		this.gv_ItemGroups.DisableFormSetting = false;
		this.gv_ItemGroups.DisplayException = false;
		this.gv_ItemGroups.EditColumn = null;
		this.gv_ItemGroups.FieldCurrency = null;
		this.gv_ItemGroups.FieldRate = null;
		this.gv_ItemGroups.FormId = "oDiscountGroup";
		this.gv_ItemGroups.GridControl = this.grd_ItemGroups;
		this.gv_ItemGroups.IgnoreFilter = false;
		this.gv_ItemGroups.IndexEditRow = -1;
		this.gv_ItemGroups.IndicatorWidth = 35;
		this.gv_ItemGroups.IsApzTable = false;
		this.gv_ItemGroups.IsInsertNewRow = false;
		this.gv_ItemGroups.IsLoadingData = false;
		this.gv_ItemGroups.IsMappingContextAction = false;
		this.gv_ItemGroups.ItemId = "OITB";
		this.gv_ItemGroups.LstColumnAllowBatch = null;
		this.gv_ItemGroups.LstColumnNotAllowBatch = null;
		this.gv_ItemGroups.LstColumnNotEditable = null;
		this.gv_ItemGroups.LstColumNotSetting = null;
		this.gv_ItemGroups.Name = "gv_ItemGroups";
		this.gv_ItemGroups.OptionsView.ShowGroupPanel = false;
		this.gv_ItemGroups.UseBatchMod = false;
		this.gv_ItemGroups.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ItemGroups_CellValueChanged);
		this.gc_ItmsGrpCod.Caption = "Items Group Code";
		this.gc_ItmsGrpCod.CurrencyDateField = null;
		this.gc_ItmsGrpCod.CurrencyField = null;
		this.gc_ItmsGrpCod.CurrencyFixed = null;
		this.gc_ItmsGrpCod.CurrencyFixedRate = 0.0;
		this.gc_ItmsGrpCod.CurrencyRateField = null;
		this.gc_ItmsGrpCod.FieldName = "ItmsGrpCod";
		this.gc_ItmsGrpCod.Name = "gc_ItmsGrpCod";
		this.gc_ItmsGrpCod.PrimaryKey = false;
		this.gc_ItmsGrpCod.UnitField = null;
		this.gc_ItmsGrpNam.Caption = "Group Name";
		this.gc_ItmsGrpNam.CurrencyDateField = null;
		this.gc_ItmsGrpNam.CurrencyField = null;
		this.gc_ItmsGrpNam.CurrencyFixed = null;
		this.gc_ItmsGrpNam.CurrencyFixedRate = 0.0;
		this.gc_ItmsGrpNam.CurrencyRateField = null;
		this.gc_ItmsGrpNam.FieldName = "ItmsGrpNam";
		this.gc_ItmsGrpNam.Name = "gc_ItmsGrpNam";
		this.gc_ItmsGrpNam.OptionsColumn.AllowEdit = false;
		this.gc_ItmsGrpNam.OptionsColumn.ReadOnly = true;
		this.gc_ItmsGrpNam.PrimaryKey = false;
		this.gc_ItmsGrpNam.UnitField = null;
		this.gc_ItmsGrpNam.Visible = true;
		this.gc_ItmsGrpNam.VisibleIndex = 0;
		this.gc_ItemsGroup_Discount.Caption = "Discount";
		this.gc_ItemsGroup_Discount.CurrencyDateField = null;
		this.gc_ItemsGroup_Discount.CurrencyField = null;
		this.gc_ItemsGroup_Discount.CurrencyFixed = null;
		this.gc_ItemsGroup_Discount.CurrencyFixedRate = 0.0;
		this.gc_ItemsGroup_Discount.CurrencyRateField = null;
		this.gc_ItemsGroup_Discount.DisplayFormat.FormatString = "n3";
		this.gc_ItemsGroup_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_ItemsGroup_Discount.FieldName = "Discount";
		this.gc_ItemsGroup_Discount.Name = "gc_ItemsGroup_Discount";
		this.gc_ItemsGroup_Discount.PrimaryKey = false;
		this.gc_ItemsGroup_Discount.UnitField = null;
		this.gc_ItemsGroup_Discount.Visible = true;
		this.gc_ItemsGroup_Discount.VisibleIndex = 1;
		this.tabp_Properties.Controls.Add(this.btn_PropertiesReset);
		this.tabp_Properties.Controls.Add(this.grd_Properties);
		this.tabp_Properties.EditControl = null;
		this.tabp_Properties.EditValue = "Properties";
		this.tabp_Properties.FieldName = null;
		this.tabp_Properties.IsSearchControl = false;
		this.tabp_Properties.Name = "tabp_Properties";
		this.tabp_Properties.ReadOnly = false;
		this.tabp_Properties.SettingInVisible = false;
		this.tabp_Properties.SettingReadonly = false;
		this.tabp_Properties.Size = new System.Drawing.Size(704, 175);
		this.tabp_Properties.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabp_Properties.Text = "Properties";
		this.btn_PropertiesReset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PropertiesReset.EditControl = null;
		this.btn_PropertiesReset.EditValue = "Reset";
		this.btn_PropertiesReset.FieldName = null;
		this.btn_PropertiesReset.IsSearchControl = false;
		this.btn_PropertiesReset.Location = new System.Drawing.Point(582, 124);
		this.btn_PropertiesReset.Name = "btn_PropertiesReset";
		this.btn_PropertiesReset.ReadOnly = false;
		this.btn_PropertiesReset.SettingInVisible = false;
		this.btn_PropertiesReset.SettingReadonly = false;
		this.btn_PropertiesReset.Size = new System.Drawing.Size(104, 23);
		this.btn_PropertiesReset.TabIndex = 19;
		this.btn_PropertiesReset.Text = "Reset";
		this.btn_PropertiesReset.Click += new System.EventHandler(this.btn_PropertiesReset_Click);
		this.grd_Properties.AllowPasteExcel = false;
		this.grd_Properties.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Properties.ClipboardData = null;
		this.grd_Properties.Location = new System.Drawing.Point(2, 0);
		this.grd_Properties.MainView = this.gv_Properties;
		this.grd_Properties.Name = "grd_Properties";
		this.grd_Properties.Size = new System.Drawing.Size(554, 172);
		this.grd_Properties.TabIndex = 17;
		this.grd_Properties.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Properties });
		this.gv_Properties.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Properties.ColumnKey = null;
		this.gv_Properties.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_ItmsTypCod, this.gc_Properties_ItmsGrpNam, this.gc_Properties_Discount });
		this.gv_Properties.DisableFormSetting = false;
		this.gv_Properties.DisplayException = false;
		this.gv_Properties.EditColumn = null;
		this.gv_Properties.FieldCurrency = null;
		this.gv_Properties.FieldRate = null;
		this.gv_Properties.FormId = "oDiscountGroup";
		this.gv_Properties.GridControl = this.grd_Properties;
		this.gv_Properties.IgnoreFilter = false;
		this.gv_Properties.IndexEditRow = -1;
		this.gv_Properties.IndicatorWidth = 35;
		this.gv_Properties.IsApzTable = false;
		this.gv_Properties.IsInsertNewRow = false;
		this.gv_Properties.IsLoadingData = false;
		this.gv_Properties.IsMappingContextAction = false;
		this.gv_Properties.ItemId = "OPET";
		this.gv_Properties.LstColumnAllowBatch = null;
		this.gv_Properties.LstColumnNotAllowBatch = null;
		this.gv_Properties.LstColumnNotEditable = null;
		this.gv_Properties.LstColumNotSetting = null;
		this.gv_Properties.Name = "gv_Properties";
		this.gv_Properties.OptionsView.ShowGroupPanel = false;
		this.gv_Properties.UseBatchMod = false;
		this.gv_Properties.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Properties_CellValueChanged);
		this.gc_ItmsTypCod.Caption = "Items Properties Code";
		this.gc_ItmsTypCod.CurrencyDateField = null;
		this.gc_ItmsTypCod.CurrencyField = null;
		this.gc_ItmsTypCod.CurrencyFixed = null;
		this.gc_ItmsTypCod.CurrencyFixedRate = 0.0;
		this.gc_ItmsTypCod.CurrencyRateField = null;
		this.gc_ItmsTypCod.FieldName = "ItmsTypCod";
		this.gc_ItmsTypCod.Name = "gc_ItmsTypCod";
		this.gc_ItmsTypCod.PrimaryKey = false;
		this.gc_ItmsTypCod.UnitField = null;
		this.gc_Properties_ItmsGrpNam.Caption = "Group Name";
		this.gc_Properties_ItmsGrpNam.CurrencyDateField = null;
		this.gc_Properties_ItmsGrpNam.CurrencyField = null;
		this.gc_Properties_ItmsGrpNam.CurrencyFixed = null;
		this.gc_Properties_ItmsGrpNam.CurrencyFixedRate = 0.0;
		this.gc_Properties_ItmsGrpNam.CurrencyRateField = null;
		this.gc_Properties_ItmsGrpNam.FieldName = "ItmsGrpNam";
		this.gc_Properties_ItmsGrpNam.Name = "gc_Properties_ItmsGrpNam";
		this.gc_Properties_ItmsGrpNam.OptionsColumn.AllowEdit = false;
		this.gc_Properties_ItmsGrpNam.OptionsColumn.ReadOnly = true;
		this.gc_Properties_ItmsGrpNam.PrimaryKey = false;
		this.gc_Properties_ItmsGrpNam.UnitField = null;
		this.gc_Properties_ItmsGrpNam.Visible = true;
		this.gc_Properties_ItmsGrpNam.VisibleIndex = 0;
		this.gc_Properties_Discount.Caption = "Discount";
		this.gc_Properties_Discount.CurrencyDateField = null;
		this.gc_Properties_Discount.CurrencyField = null;
		this.gc_Properties_Discount.CurrencyFixed = null;
		this.gc_Properties_Discount.CurrencyFixedRate = 0.0;
		this.gc_Properties_Discount.CurrencyRateField = null;
		this.gc_Properties_Discount.DisplayFormat.FormatString = "n3";
		this.gc_Properties_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_Properties_Discount.FieldName = "Discount";
		this.gc_Properties_Discount.Name = "gc_Properties_Discount";
		this.gc_Properties_Discount.PrimaryKey = false;
		this.gc_Properties_Discount.UnitField = null;
		this.gc_Properties_Discount.Visible = true;
		this.gc_Properties_Discount.VisibleIndex = 1;
		this.tabp_Manufacturers.Controls.Add(this.btn_ManufacturersReset);
		this.tabp_Manufacturers.Controls.Add(this.btn_ManufacturersNew);
		this.tabp_Manufacturers.Controls.Add(this.grd_Manufacturers);
		this.tabp_Manufacturers.EditControl = null;
		this.tabp_Manufacturers.EditValue = "Manufacturers";
		this.tabp_Manufacturers.FieldName = null;
		this.tabp_Manufacturers.IsSearchControl = false;
		this.tabp_Manufacturers.Name = "tabp_Manufacturers";
		this.tabp_Manufacturers.ReadOnly = false;
		this.tabp_Manufacturers.SettingInVisible = false;
		this.tabp_Manufacturers.SettingReadonly = false;
		this.tabp_Manufacturers.Size = new System.Drawing.Size(704, 175);
		this.tabp_Manufacturers.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabp_Manufacturers.Text = "Manufacturers";
		this.btn_ManufacturersReset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ManufacturersReset.EditControl = null;
		this.btn_ManufacturersReset.EditValue = "Reset";
		this.btn_ManufacturersReset.FieldName = null;
		this.btn_ManufacturersReset.IsSearchControl = false;
		this.btn_ManufacturersReset.Location = new System.Drawing.Point(571, 124);
		this.btn_ManufacturersReset.Name = "btn_ManufacturersReset";
		this.btn_ManufacturersReset.ReadOnly = false;
		this.btn_ManufacturersReset.SettingInVisible = false;
		this.btn_ManufacturersReset.SettingReadonly = false;
		this.btn_ManufacturersReset.Size = new System.Drawing.Size(119, 23);
		this.btn_ManufacturersReset.TabIndex = 19;
		this.btn_ManufacturersReset.Text = "Reset";
		this.btn_ManufacturersReset.Click += new System.EventHandler(this.btn_ManufacturersReset_Click);
		this.btn_ManufacturersNew.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ManufacturersNew.EditControl = null;
		this.btn_ManufacturersNew.EditValue = "New Manufacturer";
		this.btn_ManufacturersNew.FieldName = null;
		this.btn_ManufacturersNew.IsSearchControl = false;
		this.btn_ManufacturersNew.Location = new System.Drawing.Point(571, 95);
		this.btn_ManufacturersNew.Name = "btn_ManufacturersNew";
		this.btn_ManufacturersNew.ReadOnly = false;
		this.btn_ManufacturersNew.SettingInVisible = false;
		this.btn_ManufacturersNew.SettingReadonly = false;
		this.btn_ManufacturersNew.Size = new System.Drawing.Size(119, 23);
		this.btn_ManufacturersNew.TabIndex = 18;
		this.btn_ManufacturersNew.Text = "New Manufacturer";
		this.btn_ManufacturersNew.Click += new System.EventHandler(this.btn_ManufacturersNew_Click);
		this.grd_Manufacturers.AllowPasteExcel = false;
		this.grd_Manufacturers.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Manufacturers.ClipboardData = null;
		this.grd_Manufacturers.Location = new System.Drawing.Point(2, 0);
		this.grd_Manufacturers.MainView = this.gv_Manufacturers;
		this.grd_Manufacturers.Name = "grd_Manufacturers";
		this.grd_Manufacturers.Size = new System.Drawing.Size(554, 172);
		this.grd_Manufacturers.TabIndex = 17;
		this.grd_Manufacturers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Manufacturers });
		this.gv_Manufacturers.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Manufacturers.ColumnKey = null;
		this.gv_Manufacturers.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_FirmCode, this.gc_FirmName, this.gc_Manufacturer_Discount });
		this.gv_Manufacturers.DisableFormSetting = false;
		this.gv_Manufacturers.DisplayException = false;
		this.gv_Manufacturers.EditColumn = null;
		this.gv_Manufacturers.FieldCurrency = null;
		this.gv_Manufacturers.FieldRate = null;
		this.gv_Manufacturers.FormId = "oDiscountGroup";
		this.gv_Manufacturers.GridControl = this.grd_Manufacturers;
		this.gv_Manufacturers.IgnoreFilter = false;
		this.gv_Manufacturers.IndexEditRow = -1;
		this.gv_Manufacturers.IndicatorWidth = 35;
		this.gv_Manufacturers.IsApzTable = false;
		this.gv_Manufacturers.IsInsertNewRow = false;
		this.gv_Manufacturers.IsLoadingData = false;
		this.gv_Manufacturers.IsMappingContextAction = false;
		this.gv_Manufacturers.ItemId = "OMNF";
		this.gv_Manufacturers.LstColumnAllowBatch = null;
		this.gv_Manufacturers.LstColumnNotAllowBatch = null;
		this.gv_Manufacturers.LstColumnNotEditable = null;
		this.gv_Manufacturers.LstColumNotSetting = null;
		this.gv_Manufacturers.Name = "gv_Manufacturers";
		this.gv_Manufacturers.OptionsView.ShowGroupPanel = false;
		this.gv_Manufacturers.UseBatchMod = false;
		this.gv_Manufacturers.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Manufacturers_CellValueChanged);
		this.gc_FirmCode.Caption = "Firm Code";
		this.gc_FirmCode.CurrencyDateField = null;
		this.gc_FirmCode.CurrencyField = null;
		this.gc_FirmCode.CurrencyFixed = null;
		this.gc_FirmCode.CurrencyFixedRate = 0.0;
		this.gc_FirmCode.CurrencyRateField = null;
		this.gc_FirmCode.FieldName = "FirmCode";
		this.gc_FirmCode.Name = "gc_FirmCode";
		this.gc_FirmCode.PrimaryKey = false;
		this.gc_FirmCode.UnitField = null;
		this.gc_FirmName.Caption = "Name";
		this.gc_FirmName.CurrencyDateField = null;
		this.gc_FirmName.CurrencyField = null;
		this.gc_FirmName.CurrencyFixed = null;
		this.gc_FirmName.CurrencyFixedRate = 0.0;
		this.gc_FirmName.CurrencyRateField = null;
		this.gc_FirmName.FieldName = "FirmName";
		this.gc_FirmName.Name = "gc_FirmName";
		this.gc_FirmName.OptionsColumn.AllowEdit = false;
		this.gc_FirmName.OptionsColumn.ReadOnly = true;
		this.gc_FirmName.PrimaryKey = false;
		this.gc_FirmName.UnitField = null;
		this.gc_FirmName.Visible = true;
		this.gc_FirmName.VisibleIndex = 0;
		this.gc_Manufacturer_Discount.Caption = "Discount";
		this.gc_Manufacturer_Discount.CurrencyDateField = null;
		this.gc_Manufacturer_Discount.CurrencyField = null;
		this.gc_Manufacturer_Discount.CurrencyFixed = null;
		this.gc_Manufacturer_Discount.CurrencyFixedRate = 0.0;
		this.gc_Manufacturer_Discount.CurrencyRateField = null;
		this.gc_Manufacturer_Discount.DisplayFormat.FormatString = "n3";
		this.gc_Manufacturer_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_Manufacturer_Discount.FieldName = "Discount";
		this.gc_Manufacturer_Discount.Name = "gc_Manufacturer_Discount";
		this.gc_Manufacturer_Discount.PrimaryKey = false;
		this.gc_Manufacturer_Discount.UnitField = null;
		this.gc_Manufacturer_Discount.Visible = true;
		this.gc_Manufacturer_Discount.VisibleIndex = 1;
		this.tabp_Items.Controls.Add(this.btn_ItemsClearTable);
		this.tabp_Items.Controls.Add(this.btn_ItemsReset);
		this.tabp_Items.Controls.Add(this.btn_ItemsAdd);
		this.tabp_Items.Controls.Add(this.grd_Items);
		this.tabp_Items.EditControl = null;
		this.tabp_Items.EditValue = "Items";
		this.tabp_Items.FieldName = null;
		this.tabp_Items.IsSearchControl = false;
		this.tabp_Items.Name = "tabp_Items";
		this.tabp_Items.ReadOnly = false;
		this.tabp_Items.SettingInVisible = false;
		this.tabp_Items.SettingReadonly = false;
		this.tabp_Items.Size = new System.Drawing.Size(704, 175);
		this.tabp_Items.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabp_Items.Text = "Items";
		this.btn_ItemsClearTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ItemsClearTable.EditControl = null;
		this.btn_ItemsClearTable.EditValue = "Clear Table";
		this.btn_ItemsClearTable.FieldName = null;
		this.btn_ItemsClearTable.IsSearchControl = false;
		this.btn_ItemsClearTable.Location = new System.Drawing.Point(581, 65);
		this.btn_ItemsClearTable.Name = "btn_ItemsClearTable";
		this.btn_ItemsClearTable.ReadOnly = false;
		this.btn_ItemsClearTable.SettingInVisible = false;
		this.btn_ItemsClearTable.SettingReadonly = false;
		this.btn_ItemsClearTable.Size = new System.Drawing.Size(104, 23);
		this.btn_ItemsClearTable.TabIndex = 20;
		this.btn_ItemsClearTable.Text = "Clear Table";
		this.btn_ItemsClearTable.Click += new System.EventHandler(this.btn_ItemsClearTable_Click);
		this.btn_ItemsReset.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ItemsReset.EditControl = null;
		this.btn_ItemsReset.EditValue = "Reset";
		this.btn_ItemsReset.FieldName = null;
		this.btn_ItemsReset.IsSearchControl = false;
		this.btn_ItemsReset.Location = new System.Drawing.Point(581, 123);
		this.btn_ItemsReset.Name = "btn_ItemsReset";
		this.btn_ItemsReset.ReadOnly = false;
		this.btn_ItemsReset.SettingInVisible = false;
		this.btn_ItemsReset.SettingReadonly = false;
		this.btn_ItemsReset.Size = new System.Drawing.Size(104, 23);
		this.btn_ItemsReset.TabIndex = 19;
		this.btn_ItemsReset.Text = "Reset";
		this.btn_ItemsReset.Click += new System.EventHandler(this.btn_ItemsReset_Click);
		this.btn_ItemsAdd.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ItemsAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_ItemsAdd.EditControl = null;
		this.btn_ItemsAdd.EditValue = "Add";
		this.btn_ItemsAdd.FieldName = null;
		this.btn_ItemsAdd.IsSearchControl = false;
		this.btn_ItemsAdd.Location = new System.Drawing.Point(581, 94);
		this.btn_ItemsAdd.Name = "btn_ItemsAdd";
		this.btn_ItemsAdd.ReadOnly = false;
		this.btn_ItemsAdd.SettingInVisible = false;
		this.btn_ItemsAdd.SettingReadonly = false;
		this.btn_ItemsAdd.Size = new System.Drawing.Size(104, 23);
		this.btn_ItemsAdd.TabIndex = 18;
		this.btn_ItemsAdd.Text = "Add";
		this.btn_ItemsAdd.Click += new System.EventHandler(this.btn_ItemsAdd_Click);
		this.grd_Items.AllowPasteExcel = false;
		this.grd_Items.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Items.ClipboardData = null;
		this.grd_Items.Location = new System.Drawing.Point(2, 0);
		this.grd_Items.MainView = this.gv_Items;
		this.grd_Items.Name = "grd_Items";
		this.grd_Items.Size = new System.Drawing.Size(554, 172);
		this.grd_Items.TabIndex = 17;
		this.grd_Items.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Items });
		this.gv_Items.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Items.ColumnKey = null;
		this.gv_Items.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_Items_ItemCode, this.gc_ItemName, this.gc_Items_Discount });
		this.gv_Items.DisableFormSetting = false;
		this.gv_Items.DisplayException = false;
		this.gv_Items.EditColumn = null;
		this.gv_Items.FieldCurrency = null;
		this.gv_Items.FieldRate = null;
		this.gv_Items.FormId = "oDiscountGroup";
		this.gv_Items.GridControl = this.grd_Items;
		this.gv_Items.IgnoreFilter = false;
		this.gv_Items.IndexEditRow = -1;
		this.gv_Items.IndicatorWidth = 35;
		this.gv_Items.IsApzTable = false;
		this.gv_Items.IsInsertNewRow = false;
		this.gv_Items.IsLoadingData = false;
		this.gv_Items.IsMappingContextAction = false;
		this.gv_Items.ItemId = "OITM";
		this.gv_Items.LstColumnAllowBatch = null;
		this.gv_Items.LstColumnNotAllowBatch = null;
		this.gv_Items.LstColumnNotEditable = null;
		this.gv_Items.LstColumNotSetting = null;
		this.gv_Items.Name = "gv_Items";
		this.gv_Items.OptionsView.ShowGroupPanel = false;
		this.gv_Items.UseBatchMod = false;
		this.gv_Items.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Items_CellValueChanged);
		this.gc_Items_ItemCode.Caption = "Item Code";
		this.gc_Items_ItemCode.CurrencyDateField = null;
		this.gc_Items_ItemCode.CurrencyField = null;
		this.gc_Items_ItemCode.CurrencyFixed = null;
		this.gc_Items_ItemCode.CurrencyFixedRate = 0.0;
		this.gc_Items_ItemCode.CurrencyRateField = null;
		this.gc_Items_ItemCode.FieldName = "ItemCode";
		this.gc_Items_ItemCode.Name = "gc_Items_ItemCode";
		this.gc_Items_ItemCode.OptionsColumn.AllowEdit = false;
		this.gc_Items_ItemCode.OptionsColumn.ReadOnly = true;
		this.gc_Items_ItemCode.PrimaryKey = false;
		this.gc_Items_ItemCode.UnitField = null;
		this.gc_Items_ItemCode.Visible = true;
		this.gc_Items_ItemCode.VisibleIndex = 0;
		this.gc_ItemName.Caption = "Item Name";
		this.gc_ItemName.CurrencyDateField = null;
		this.gc_ItemName.CurrencyField = null;
		this.gc_ItemName.CurrencyFixed = null;
		this.gc_ItemName.CurrencyFixedRate = 0.0;
		this.gc_ItemName.CurrencyRateField = null;
		this.gc_ItemName.FieldName = "ItemName";
		this.gc_ItemName.Name = "gc_ItemName";
		this.gc_ItemName.OptionsColumn.AllowEdit = false;
		this.gc_ItemName.OptionsColumn.ReadOnly = true;
		this.gc_ItemName.PrimaryKey = false;
		this.gc_ItemName.UnitField = null;
		this.gc_ItemName.Visible = true;
		this.gc_ItemName.VisibleIndex = 1;
		this.gc_Items_Discount.Caption = "Discount";
		this.gc_Items_Discount.CurrencyDateField = null;
		this.gc_Items_Discount.CurrencyField = null;
		this.gc_Items_Discount.CurrencyFixed = null;
		this.gc_Items_Discount.CurrencyFixedRate = 0.0;
		this.gc_Items_Discount.CurrencyRateField = null;
		this.gc_Items_Discount.DisplayFormat.FormatString = "n3";
		this.gc_Items_Discount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_Items_Discount.FieldName = "Discount";
		this.gc_Items_Discount.Name = "gc_Items_Discount";
		this.gc_Items_Discount.PrimaryKey = false;
		this.gc_Items_Discount.UnitField = null;
		this.gc_Items_Discount.Visible = true;
		this.gc_Items_Discount.VisibleIndex = 2;
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "OK";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(12, 432);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(75, 24);
		this.btn_Add.TabIndex = 1;
		this.btn_Add.Text = "OK";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(91, 432);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 24);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Type";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(12, 15);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(41, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 8;
		this.lb_Find.Utext = "Type";
		this.lbl_BPCode.EditControl = null;
		this.lbl_BPCode.EditValue = "BP Code";
		this.lbl_BPCode.FieldName = null;
		this.lbl_BPCode.HideWhenEmpty = false;
		this.lbl_BPCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BPCode.Location = new System.Drawing.Point(159, 14);
		this.lbl_BPCode.Name = "lbl_BPCode";
		this.lbl_BPCode.ReadOnly = false;
		this.lbl_BPCode.SettingInVisible = false;
		this.lbl_BPCode.SettingReadonly = false;
		this.lbl_BPCode.ShowUnderLine = true;
		this.lbl_BPCode.Size = new System.Drawing.Size(66, 20);
		this.lbl_BPCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BPCode.TabIndex = 9;
		this.lbl_BPCode.Utext = "BP Code";
		this.lbl_BPName.EditControl = null;
		this.lbl_BPName.EditValue = "BP Name";
		this.lbl_BPName.FieldName = null;
		this.lbl_BPName.HideWhenEmpty = false;
		this.lbl_BPName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BPName.Location = new System.Drawing.Point(337, 14);
		this.lbl_BPName.Name = "lbl_BPName";
		this.lbl_BPName.ReadOnly = false;
		this.lbl_BPName.SettingInVisible = false;
		this.lbl_BPName.SettingReadonly = false;
		this.lbl_BPName.ShowUnderLine = true;
		this.lbl_BPName.Size = new System.Drawing.Size(62, 20);
		this.lbl_BPName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BPName.TabIndex = 10;
		this.lbl_BPName.Utext = "BP Name";
		this.cb_Type.EditControl = null;
		this.cb_Type.FieldName = null;
		this.cb_Type.IsSearchControl = false;
		this.cb_Type.Location = new System.Drawing.Point(47, 13);
		this.cb_Type.Name = "cb_Type";
		this.cb_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Type.Properties.Items.AddRange(new object[4] { "All BPs", "Customer Group", "Vendor Group", "Specific BP" });
		this.cb_Type.SettingInVisible = false;
		this.cb_Type.SettingReadonly = false;
		this.cb_Type.Size = new System.Drawing.Size(106, 20);
		this.cb_Type.TabIndex = 11;
		this.cb_Type.SelectedIndexChanged += new System.EventHandler(this.cb_Type_SelectedIndexChanged);
		this.txt_BPName.EditControl = null;
		this.txt_BPName.FieldName = null;
		this.txt_BPName.IsSearchControl = false;
		this.txt_BPName.Location = new System.Drawing.Point(397, 12);
		this.txt_BPName.Name = "txt_BPName";
		this.txt_BPName.SettingInVisible = false;
		this.txt_BPName.SettingReadonly = false;
		this.txt_BPName.Size = new System.Drawing.Size(240, 20);
		this.txt_BPName.TabIndex = 13;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(643, 12);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 14;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lbl_GroupNum.EditControl = null;
		this.lbl_GroupNum.EditValue = "Group Name";
		this.lbl_GroupNum.FieldName = null;
		this.lbl_GroupNum.HideWhenEmpty = false;
		this.lbl_GroupNum.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_GroupNum.Location = new System.Drawing.Point(159, 39);
		this.lbl_GroupNum.Name = "lbl_GroupNum";
		this.lbl_GroupNum.ReadOnly = false;
		this.lbl_GroupNum.SettingInVisible = false;
		this.lbl_GroupNum.SettingReadonly = false;
		this.lbl_GroupNum.ShowUnderLine = true;
		this.lbl_GroupNum.Size = new System.Drawing.Size(66, 20);
		this.lbl_GroupNum.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_GroupNum.TabIndex = 15;
		this.lbl_GroupNum.Utext = "Group Name";
		this.lk_Group.Location = new System.Drawing.Point(220, 36);
		this.lk_Group.Name = "lk_Group";
		this.lk_Group.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_Group.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupName", "Name")
		});
		this.lk_Group.Properties.DisplayMember = "GroupName";
		this.lk_Group.Properties.NullText = "";
		this.lk_Group.Properties.ValueMember = "GroupCode";
		this.lk_Group.Size = new System.Drawing.Size(110, 20);
		this.lk_Group.TabIndex = 16;
		this.bte_BPCode.EditControl = null;
		this.bte_BPCode.FieldName = null;
		this.bte_BPCode.IsSearchControl = false;
		this.bte_BPCode.Location = new System.Drawing.Point(220, 12);
		this.bte_BPCode.Name = "bte_BPCode";
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.bte_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_BPCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit1_Properties_ButtonPressed);
		this.bte_BPCode.SettingInVisible = false;
		this.bte_BPCode.SettingReadonly = false;
		this.bte_BPCode.Size = new System.Drawing.Size(110, 20);
		this.bte_BPCode.TabIndex = 17;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(730, 463);
		base.Controls.Add(this.bte_BPCode);
		base.Controls.Add(this.lk_Group);
		base.Controls.Add(this.lbl_GroupNum);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.txt_BPName);
		base.Controls.Add(this.cb_Type);
		base.Controls.Add(this.lbl_BPName);
		base.Controls.Add(this.lbl_BPCode);
		base.Controls.Add(this.lb_Find);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oDiscountGroup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DiscountGroup";
		this.Text = "Discount Group";
		base.Load += new System.EventHandler(this.frm_DiscountGroup_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_grd_DocType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_DiscRel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabp_ItemGroup.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ItemGroups).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ItemGroups).EndInit();
		this.tabp_Properties.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Properties).EndInit();
		this.tabp_Manufacturers.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Manufacturers).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Manufacturers).EndInit();
		this.tabp_Items.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Group.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
