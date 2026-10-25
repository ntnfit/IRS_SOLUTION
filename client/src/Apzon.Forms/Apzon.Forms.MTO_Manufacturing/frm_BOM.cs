using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.ItemsMaster;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_BOM : XtraFormEx
{
	private bool _edittingLine;

	private int _currentRow = -1;

	private IContainer components;

	private LabelLine lb_ProductCode;

	private LabelLine lb_ProductName;

	private TextEditEx txt_ItemName;

	private LabelLine lb_CostCenter;

	private LabelLine lb_Status;

	private ComboBoxEditEx cbx_Status;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btn_Search;

	private ButtonEditEx txt_ItemCode;

	private GroupControl groupControl1;

	private GridControlEx grd_General;

	private GridViewEx gv_General;

	private GridColumnEx cl_ItemCode;

	private GridColumnEx cl_ItemName;

	private GridColumnEx cl_Quantity;

	private GridColumnEx cl_Status;

	private GridColumnEx cl_ValidFrom;

	private GridColumnEx cl_ValidTo;

	private GridColumnEx cl_CardCode;

	private GridColumnEx cl_CardName;

	private GridColumnEx cl_DistRule;

	private GridColumnEx cl_ToWH;

	private GridColumnEx cl_Uom;

	private GridColumnEx cl_Remark;

	private GroupControl groupControl2;

	private GridControlEx grd_BOMDetail;

	private GridViewEx gv_BOMDetail;

	private ButtonEditEx bte_CostCenter;

	private GridColumnEx cl_Line_Type;

	private GridColumnEx cl_Line_ItemCode;

	private GridColumnEx cl_Line_ItemName;

	private GridColumnEx cl_Line_Quantity;

	private GridColumnEx cl_Line_IssueMthd;

	private GridColumnEx cl_Line_PriceList;

	private GridColumnEx cl_Line_UnitPrice;

	private GridColumnEx cl_Line_Whse;

	private GridColumnEx cl_Line_Uom;

	private GridColumnEx cl_Line_WipAccount;

	private GridColumnEx cl_Line_Project;

	private Panel panel1;

	private SimpleButtonEx btn_Refresh;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private GridColumnEx cl_Line_Comment;

	private GridColumnEx cl_Line_Father;

	private GridColumnEx cl_Line_ChildNum;

	private RepositoryItemButtonEdit rp_CardCode;

	private RepositoryItemButtonEdit rp_ItemCode;

	private GridColumnEx cl_TreeType;

	private GridColumnEx cl_Scrap;

	private RepositoryItemButtonEdit rp_Line_ItemCode;

	private RepositoryItemSpinEdit sp_NumbericN2;

	private RepositoryItemButtonEdit rpb_Warehouse;

	private RepositoryItemButtonEdit rpb_Line_Warehouse;

	private RepositoryItemLookUpEdit rpl_Status;

	private RepositoryItemMemoExEdit rpm_Remark;

	private RepositoryItemMemoExEdit rpm_Line_Comment;

	private RepositoryItemLookUpEdit rpl_Line_IssueMth;

	private RepositoryItemLookUpEdit rpl_Line_PriceList;

	private RepositoryItemLookUpEdit rpl_TreeType;

	private RepositoryItemLookUpEdit rpl_Line_Type;

	private RepositoryItemDateEdit rpd_Valid;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_BOM()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BOM"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BOM" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmBOM;
		btn_Cancel.Text = Resources.ButtonCancel;
		btn_Save.Text = Resources.ButtonSave;
		btn_Refresh.Text = Resources.ButtonRefresh;
		btn_Search.Text = Resources.ButtonSearch;
		lb_ProductCode.Utext = Resources.lb_ProductCode;
		lb_ProductName.Utext = Resources.lb_ProductName;
		lb_Status.Utext = Resources.lb_Status;
		lb_CostCenter.Utext = Resources.lb_CostCenter;
		cl_ItemCode.Caption = Resources.lb_ProductCode;
		cl_ItemName.Caption = Resources.lb_ProductName;
		cl_TreeType.Caption = Resources.lb_TreeType;
		cl_Quantity.Caption = Resources.lb_Quantity;
		cl_ValidFrom.Caption = Resources.lb_ActiveFrom;
		cl_ValidTo.Caption = Resources.lb_ActiveTo;
		cl_CardCode.Caption = Resources.lb_BPCode;
		cl_CardName.Caption = Resources.lb_BPName;
		cl_DistRule.Caption = Resources.lb_DistRule;
		cl_ToWH.Caption = Resources.lb_Warehouse;
		cl_Uom.Caption = Resources.lb_UoM;
		cl_Remark.Caption = Resources.lb_Remark;
		cl_Line_Type.Caption = Resources.lb_Type;
		cl_Line_ItemCode.Caption = Resources.lb_Code;
		cl_Line_ItemName.Caption = Resources.Line_gvLine_gcItemName;
		cl_Line_Quantity.Caption = Resources.lb_Quantity;
		cl_Line_IssueMthd.Caption = Resources.lb_IssuedMethod;
		cl_Line_PriceList.Caption = Resources.lb_PriceList;
		cl_Line_UnitPrice.Caption = Resources.Line_gvLine_gcPrice;
		cl_Line_Whse.Caption = Resources.lb_Warehouse;
		cl_Line_Uom.Caption = Resources.lb_UoM;
		cl_Line_WipAccount.Caption = Resources.lb_WipAccount;
		cl_Line_Project.Caption = Resources.lb_Project;
		cl_Line_Comment.Caption = Resources.lb_Comment;
	}

	private void txt_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.BOM, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				ApzBillOfMaterials apzBillOfMaterials = (ApzBillOfMaterials)frm_ChooseFromList2.ChooseItems[0];
				txt_ItemCode.Text = apzBillOfMaterials.Code;
				txt_ItemName.Text = apzBillOfMaterials.ItemName;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_BOM_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			LoadComboboxStatus();
			LoadRepoGrid();
			await LoadPriceList();
			ApzBillOfMaterials bom = new ApzBillOfMaterials();
			DataTable dataSource = await SearchBom(bom);
			grd_General.DataSource = dataSource;
			grd_General.RefreshDataSource();
			List<ApzBillOfMaterialLines> list = new List<ApzBillOfMaterialLines>();
			DataTable dataSource2 = CreateDataTable(list);
			grd_BOMDetail.DataSource = dataSource2;
			grd_BOMDetail.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_General_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void rp_CardCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "CardCode"));
			if (e.Button.Index == 0)
			{
				ChooseFromListBP(text);
				return;
			}
			frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
			frm_BusinessMasterData2._copyF_CodeCustomer = text;
			frm_BusinessMasterData2.MdiParent = MdiParent;
			frm_BusinessMasterData2.Show();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rp_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_General.FocusedRowHandle;
			string itemCode = Function.ToString(gv_General.GetRowCellValue(focusedRowHandle, cl_ItemCode));
			if (e.Button.Index == 0)
			{
				ChooseFormListItemCode(itemCode);
			}
			else if (e.Button.Index == 1)
			{
				frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: true, flagSearchFrm: false, itemCode);
				frm_ItemMasterData2.MdiParent = MdiParent;
				frm_ItemMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gv_General_InitNewRow(object sender, InitNewRowEventArgs e)
	{
		GridView gridView = sender as GridView;
		gridView.SetRowCellValue(e.RowHandle, gridView.Columns["Status"], "Y");
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			ApzBillOfMaterials apzBillOfMaterials = new ApzBillOfMaterials();
			List<ApzBillOfMaterialLines> list = new List<ApzBillOfMaterialLines>();
			DataTable dataSource = CreateDataTable(list);
			grd_BOMDetail.DataSource = dataSource;
			grd_BOMDetail.RefreshDataSource();
			apzBillOfMaterials.Code = txt_ItemCode.Text.Trim();
			apzBillOfMaterials.ItemName = txt_ItemName.Text.Trim();
			apzBillOfMaterials.Status = "";
			apzBillOfMaterials.OcrCode = bte_CostCenter.Text.Trim();
			DataTable dataSource2 = await SearchBom(apzBillOfMaterials);
			grd_General.DataSource = dataSource2;
			grd_General.RefreshDataSource();
			gv_General.FocusedRowHandle = 0;
			_edittingLine = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Refresh_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			ApzBillOfMaterials bom = new ApzBillOfMaterials();
			DataTable dataSource = await SearchBom(bom);
			grd_General.DataSource = dataSource;
			grd_General.RefreshDataSource();
			List<ApzBillOfMaterialLines> list = new List<ApzBillOfMaterialLines>();
			DataTable dataSource2 = CreateDataTable(list);
			grd_BOMDetail.DataSource = dataSource2;
			grd_BOMDetail.RefreshDataSource();
			_edittingLine = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet ds = new DataSet();
				DataTable changes = ((DataTable)grd_General.DataSource).GetChanges();
				if (changes == null || changes.Rows.Count == 0)
				{
					return;
				}
				DataTable dataTable = (DataTable)grd_BOMDetail.DataSource;
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					return;
				}
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (dataTable.Rows[i]["Code"] == null || string.IsNullOrEmpty(dataTable.Rows[i]["Code"].ToString()))
					{
						dataTable.Rows.RemoveAt(i);
					}
				}
				ds.Tables.AddRange(new DataTable[2] { changes, dataTable });
				string text = await SaveBom(ds);
				ds.Tables.Clear();
				ds.Dispose();
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					_edittingLine = false;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void gv_General_RowClick(object sender, RowClickEventArgs e)
	{
	}

	private async void gv_General_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (e.FocusedRowHandle != _currentRow && _edittingLine && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				gv_General.FocusedRowHandle = _currentRow;
				return;
			}
			if (e.FocusedRowHandle == _currentRow && _edittingLine)
			{
				return;
			}
			ApzBillOfMaterials apzBillOfMaterials = new ApzBillOfMaterials();
			if (gv_General.FocusedRowHandle != -2147483647)
			{
				string code = ((gv_General.GetRowCellValue(e.FocusedRowHandle, cl_ItemCode) == null) ? "" : gv_General.GetRowCellValue(e.FocusedRowHandle, cl_ItemCode).ToString());
				string cardCode = ((gv_General.GetRowCellValue(e.FocusedRowHandle, cl_CardCode) == null) ? "" : gv_General.GetRowCellValue(e.FocusedRowHandle, cl_CardCode).ToString());
				apzBillOfMaterials.Code = code;
				apzBillOfMaterials.CardCode = cardCode;
			}
			DataTable dataSource = await SearchLines(apzBillOfMaterials);
			grd_BOMDetail.DataSource = dataSource;
			grd_BOMDetail.RefreshDataSource();
			_currentRow = e.FocusedRowHandle;
			_edittingLine = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_General_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		GridViewMenu gridViewMenu = new GridViewMenu(gv_General);
		string[] array = new string[1] { "Xóa" };
		foreach (string caption in array)
		{
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = caption;
			dXMenuItem.Click += itemMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
		}
		if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
		{
			e.Allow = true;
			gridViewMenu.Show(e.Point);
		}
	}

	private async void itemMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(sender as DXMenuItem).Caption.Equals("Xóa") || gv_General.FocusedRowHandle <= -1)
			{
				return;
			}
			string text = gv_General.GetFocusedRowCellValue(cl_ItemCode).ToString();
			string text2 = gv_General.GetFocusedRowCellValue(cl_CardCode).ToString();
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					ApzBillOfMaterials apzBillOfMaterials = new ApzBillOfMaterials();
					apzBillOfMaterials.Code = text;
					apzBillOfMaterials.CardCode = text2;
					string text3 = await DeleteBom(apzBillOfMaterials);
					if (string.IsNullOrEmpty(text3))
					{
						gv_General.DeleteRow(gv_General.FocusedRowHandle);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text3);
					}
				}
			}
			else
			{
				gv_General.DeleteRow(gv_General.FocusedRowHandle);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rpb_Warehouse_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, cl_ToWH));
			if (!string.IsNullOrEmpty(Function.ToString(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, cl_ItemCode))))
			{
				if (e.Button.Index == 0)
				{
					ChooseFromListWarehouse(text);
				}
				else
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_BOMDetail_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_edittingLine = true;
	}

	private async Task<DataTable> SearchBom(ApzBillOfMaterials bom)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApzBillOfMaterials");
			dt = await _helper.PostAsync("SearchBom", bom);
			if (dt != null && dt.Rows.Count > 0)
			{
				return dt;
			}
			List<ApzBillOfMaterials> list = new List<ApzBillOfMaterials>();
			dt = CreateDataTable(list);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private void ChooseFromListBP(string cardCode)
	{
		frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, cardCode);
		frm_ChooseFromList2.ShowDialog(this);
		if (frm_ChooseFromList2.ChooseItem != null)
		{
			BusinessPartnersImpl bPFromCFL = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
			SetBPFromCFL(bPFromCFL);
		}
	}

	private void SetBPFromCFL(BusinessPartnersImpl buss)
	{
		try
		{
			if (gv_General.IsNewItemRow(gv_General.FocusedRowHandle))
			{
				DataTable dataTable = (DataTable)grd_General.DataSource;
				DataRow dataRow = dataTable.NewRow();
				dataRow["CardCode"] = buss.CardCode;
				dataRow["CardName"] = buss.CardName;
				dataRow["Status"] = "Y";
				dataRow["Quantity"] = 1;
				dataTable.Rows.Add(dataRow);
				grd_General.DataSource = dataTable;
				grd_General.RefreshDataSource();
			}
			else
			{
				gv_General.SetFocusedRowCellValue(cl_CardCode, buss.CardCode);
				gv_General.SetFocusedRowCellValue(cl_CardName, buss.CardName);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFormListItemCode(string ItemCode)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ItemsInventory, multiSelect: true, ItemCode);
			frm_ChooseFromList2.ShowDialog(this);
			SetItemsToGridGeneral(frm_ChooseFromList2.ChooseItems);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetItemsToGridGeneral(List<dynamic> items)
	{
		try
		{
			if (items != null && items.Count != 0)
			{
				if (gv_General.IsNewItemRow(gv_General.FocusedRowHandle))
				{
					DataTable dataTable = (DataTable)grd_General.DataSource;
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = (object)items[0].ItemCode;
					dataRow["ItemName"] = (object)items[0].ItemName;
					dataRow["Status"] = "Y";
					dataRow["Quantity"] = 1;
					dataTable.Rows.Add(dataRow);
					grd_General.DataSource = dataTable;
					grd_General.RefreshDataSource();
					gv_General.FocusedRowHandle = dataTable.Rows.Count - 1;
				}
				else
				{
					gv_General.SetFocusedRowCellValue(cl_ItemCode, items[0].ItemCode);
					gv_General.SetFocusedRowCellValue(cl_ItemName, items[0].ItemName);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveBom(DataSet ds)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ApzBillOfMaterials");
			string text = await _help.PostAsJsonAsync("SaveBom", ds);
			if (text != null && !(text == ""))
			{
				return text;
			}
			WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
		return null;
	}

	private async Task<string> DeleteBom(ApzBillOfMaterials bom)
	{
		WinCommons._OpenProcessing();
		try
		{
			using HttpClientHelper<ApzBillOfMaterials> _help = new HttpClientHelper<ApzBillOfMaterials>(Configuration.ApiServer, "ApzBillOfMaterials");
			string text = await _help.PostAsJsonAsync("DeleteBom", bom);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
		WinCommons._OpenProcessing();
		return null;
	}

	private void ChooseFromListWarehouse(string whse)
	{
		frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, whse);
		frm_ChooseFromList2.ShowDialog(this);
		if (frm_ChooseFromList2.ChooseItem != null)
		{
			DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
			SetWarehouseFromCFL(Function.ToString(dataRow["WhsCode"]));
		}
	}

	private void SetWarehouseFromCFL(string whsCode)
	{
		try
		{
			gv_General.SetFocusedRowCellValue(cl_ToWH, whsCode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_BOMDetail_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task<DataTable> SearchLines(ApzBillOfMaterials bom)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ApzBillOfMaterials");
			dt = await _helper.PostAsync("SearchLines", bom);
			if (dt != null && dt.Rows.Count > 0)
			{
				return dt;
			}
			List<ApzBillOfMaterialLines> list = new List<ApzBillOfMaterialLines>();
			dt = CreateDataTable(list);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private void rp_Line_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_General.FocusedRowHandle;
			string text = Function.ToString(gv_General.GetRowCellValue(focusedRowHandle, cl_ItemCode));
			string text2 = Function.ToString(gv_General.GetRowCellValue(focusedRowHandle, cl_CardCode));
			if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(text2))
			{
				string itemCode = Function.ToString(gv_BOMDetail.GetRowCellValue(gv_BOMDetail.FocusedRowHandle, cl_Line_ItemCode));
				if (e.Button.Index == 0)
				{
					ChooseFormListItemCodeLine(itemCode, text, text2);
					_edittingLine = true;
				}
				else if (e.Button.Index == 1)
				{
					frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: true, flagSearchFrm: false, itemCode);
					frm_ItemMasterData2.MdiParent = MdiParent;
					frm_ItemMasterData2.Show();
				}
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.ChooseItemCodeAndCardCodeInGeneral, Enums.MessageType.Warning, Msg_Time._TimeMedium, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void ChooseFormListItemCodeLine(string ItemCode, string fatherCode, string cardCode)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ItemsInventory, multiSelect: true, ItemCode);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count != 0)
			{
				if (gv_BOMDetail.IsNewItemRow(gv_BOMDetail.FocusedRowHandle))
				{
					DataTable dataTable = (DataTable)grd_BOMDetail.DataSource;
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = (object)frm_ChooseFromList2.ChooseItems[0].ItemCode;
					dataRow["ItemName"] = (object)frm_ChooseFromList2.ChooseItems[0].ItemName;
					dataRow["Quantity"] = 1;
					dataRow["Type"] = "I";
					dataRow["IssueMthd"] = "M";
					dataRow["FatherCode"] = fatherCode;
					dataRow["CardCode"] = cardCode;
					dataRow["ChildNum"] = dataTable.Rows.Count + 1;
					dataTable.Rows.Add(dataRow);
					grd_BOMDetail.DataSource = dataTable;
					grd_BOMDetail.RefreshDataSource();
				}
				else
				{
					gv_BOMDetail.SetFocusedRowCellValue(cl_Line_ItemCode, frm_ChooseFromList2.ChooseItems[0].ItemCode);
					gv_BOMDetail.SetFocusedRowCellValue(cl_Line_ItemName, frm_ChooseFromList2.ChooseItems[0].ItemName);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rpb_Line_Warehouse_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_BOMDetail.GetRowCellValue(gv_BOMDetail.FocusedRowHandle, cl_Line_Whse));
			if (!string.IsNullOrEmpty(Function.ToString(gv_BOMDetail.GetRowCellValue(gv_BOMDetail.FocusedRowHandle, cl_Line_ItemCode))))
			{
				if (e.Button.Index == 0)
				{
					ChooseFromListWarehouse_Line(text);
					_edittingLine = true;
				}
				else
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFromListWarehouse_Line(string whse)
	{
		frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, whse);
		frm_ChooseFromList2.ShowDialog(this);
		if (frm_ChooseFromList2.ChooseItem != null)
		{
			DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
			gv_BOMDetail.SetFocusedRowCellValue(cl_Line_Whse, Function.ToString(dataRow["WhsCode"]));
		}
	}

	public DataTable CreateDataTable<T>(IEnumerable<T> list)
	{
		PropertyInfo[] properties = typeof(T).GetProperties();
		DataTable dataTable = new DataTable();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			dataTable.Columns.Add(new DataColumn(propertyInfo.Name, Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType));
		}
		foreach (T item in list)
		{
			object[] array2 = new object[properties.Length];
			for (int j = 0; j < properties.Length; j++)
			{
				array2[j] = properties[j].GetValue(item);
			}
			dataTable.Rows.Add(array2);
		}
		return dataTable;
	}

	private void LoadComboboxStatus()
	{
		try
		{
			cbx_Status.Properties.Items.Clear();
			cbx_Status.Properties.Items.Add(new Common.ListItems("", ""));
			cbx_Status.Properties.Items.Add(new Common.ListItems("Active", "Y"));
			cbx_Status.Properties.Items.Add(new Common.ListItems("Inactive", "N"));
			cbx_Status.SelectedIndex = 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadRepoGrid()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Active",
				ID = "Y"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Inactive",
				ID = "N"
			});
			rpl_Status.DataSource = list;
			rpl_Status.DisplayMember = "Name";
			rpl_Status.ValueMember = "ID";
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			list2.Add(new Common.Table_ID
			{
				Name = "Backflush",
				ID = "B"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Manual",
				ID = "M"
			});
			rpl_Line_IssueMth.DataSource = list2;
			rpl_Line_IssueMth.DisplayMember = "Name";
			rpl_Line_IssueMth.ValueMember = "ID";
			List<Common.Table_ID> list3 = new List<Common.Table_ID>();
			list3.Add(new Common.Table_ID
			{
				Name = "Production",
				ID = "P"
			});
			list3.Add(new Common.Table_ID
			{
				Name = "Assembly",
				ID = "A"
			});
			list3.Add(new Common.Table_ID
			{
				Name = "Sales",
				ID = "S"
			});
			list3.Add(new Common.Table_ID
			{
				Name = "Template",
				ID = "T"
			});
			rpl_TreeType.DataSource = list3;
			rpl_TreeType.DisplayMember = "Name";
			rpl_TreeType.ValueMember = "ID";
			List<Common.Table_ID> list4 = new List<Common.Table_ID>();
			list4.Add(new Common.Table_ID
			{
				Name = "Item",
				ID = "I"
			});
			list4.Add(new Common.Table_ID
			{
				Name = "Resource",
				ID = "R"
			});
			list4.Add(new Common.Table_ID
			{
				Name = "Text",
				ID = "T"
			});
			rpl_Line_Type.DataSource = list4;
			rpl_Line_Type.DisplayMember = "Name";
			rpl_Line_Type.ValueMember = "ID";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PriceList");
			DataTable dataTable = await _helper.GetDataTableAsync("ShowPriceList");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rpl_Line_PriceList.DataSource = dataTable;
				rpl_Line_PriceList.DisplayMember = "PriceListName";
				rpl_Line_PriceList.ValueMember = "PriceListNo";
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_General_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_General.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_BOMDetail_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_BOMDetail.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_BOM));
		this.lb_ProductCode = new Apzon.Controls.Controls.LabelLine();
		this.lb_ProductName = new Apzon.Controls.Controls.LabelLine();
		this.txt_ItemName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_CostCenter = new Apzon.Controls.Controls.LabelLine();
		this.lb_Status = new Apzon.Controls.Controls.LabelLine();
		this.cbx_Status = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.grd_General = new Apzon.Controls.Grid.GridControlEx();
		this.gv_General = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rp_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_TreeType = new Apzon.Controls.Grid.GridColumnEx();
		this.rpl_TreeType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Quantity = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Scrap = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rpl_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_ValidFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.rpd_Valid = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.cl_ValidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rp_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_DistRule = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ToWH = new Apzon.Controls.Grid.GridColumnEx();
		this.rpb_Warehouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_Uom = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.rpm_Remark = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.sp_NumbericN2 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grd_BOMDetail = new Apzon.Controls.Grid.GridControlEx();
		this.gv_BOMDetail = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Line_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.rpl_Line_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Line_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rp_Line_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_Line_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_Quantity = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_IssueMthd = new Apzon.Controls.Grid.GridColumnEx();
		this.rpl_Line_IssueMth = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Line_PriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rpl_Line_PriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Line_UnitPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_Whse = new Apzon.Controls.Grid.GridColumnEx();
		this.rpb_Line_Warehouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_Line_Uom = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_WipAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_Project = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_Comment = new Apzon.Controls.Grid.GridColumnEx();
		this.rpm_Line_Comment = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.cl_Line_Father = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Line_ChildNum = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.bte_CostCenter = new Apzon.Controls.Editor.ButtonEditEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_General).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_General).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rp_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_TreeType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpd_Valid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpd_Valid.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rp_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpb_Warehouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpm_Remark).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.sp_NumbericN2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_BOMDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BOMDetail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Line_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rp_Line_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Line_IssueMth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Line_PriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpb_Line_Warehouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rpm_Line_Comment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_CostCenter.Properties).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.lb_ProductCode.EditControl = null;
		this.lb_ProductCode.EditValue = "Product Code";
		this.lb_ProductCode.FieldName = null;
		this.lb_ProductCode.HideWhenEmpty = false;
		this.lb_ProductCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ProductCode.Location = new System.Drawing.Point(7, 11);
		this.lb_ProductCode.Name = "lb_ProductCode";
		this.lb_ProductCode.ReadOnly = false;
		this.lb_ProductCode.SettingInVisible = false;
		this.lb_ProductCode.SettingReadonly = false;
		this.lb_ProductCode.ShowUnderLine = true;
		this.lb_ProductCode.Size = new System.Drawing.Size(117, 20);
		this.lb_ProductCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ProductCode.TabIndex = 0;
		this.lb_ProductCode.Utext = "Product Code";
		this.lb_ProductName.EditControl = null;
		this.lb_ProductName.EditValue = "Product Name";
		this.lb_ProductName.FieldName = null;
		this.lb_ProductName.HideWhenEmpty = false;
		this.lb_ProductName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ProductName.Location = new System.Drawing.Point(7, 35);
		this.lb_ProductName.Name = "lb_ProductName";
		this.lb_ProductName.ReadOnly = false;
		this.lb_ProductName.SettingInVisible = false;
		this.lb_ProductName.SettingReadonly = false;
		this.lb_ProductName.ShowUnderLine = true;
		this.lb_ProductName.Size = new System.Drawing.Size(117, 20);
		this.lb_ProductName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ProductName.TabIndex = 2;
		this.lb_ProductName.Utext = "Product Name";
		this.txt_ItemName.EditControl = null;
		this.txt_ItemName.FieldName = null;
		this.txt_ItemName.IsSearchControl = false;
		this.txt_ItemName.Location = new System.Drawing.Point(122, 31);
		this.txt_ItemName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_ItemName.Name = "txt_ItemName";
		this.txt_ItemName.SettingInVisible = false;
		this.txt_ItemName.SettingReadonly = false;
		this.txt_ItemName.Size = new System.Drawing.Size(147, 20);
		this.txt_ItemName.TabIndex = 3;
		this.lb_CostCenter.EditControl = null;
		this.lb_CostCenter.EditValue = "Cost Center";
		this.lb_CostCenter.FieldName = null;
		this.lb_CostCenter.HideWhenEmpty = false;
		this.lb_CostCenter.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CostCenter.Location = new System.Drawing.Point(324, 35);
		this.lb_CostCenter.Name = "lb_CostCenter";
		this.lb_CostCenter.ReadOnly = false;
		this.lb_CostCenter.SettingInVisible = false;
		this.lb_CostCenter.SettingReadonly = false;
		this.lb_CostCenter.ShowUnderLine = true;
		this.lb_CostCenter.Size = new System.Drawing.Size(114, 20);
		this.lb_CostCenter.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CostCenter.TabIndex = 6;
		this.lb_CostCenter.Utext = "Cost Center";
		this.lb_Status.EditControl = null;
		this.lb_Status.EditValue = "Status";
		this.lb_Status.FieldName = null;
		this.lb_Status.HideWhenEmpty = false;
		this.lb_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Status.Location = new System.Drawing.Point(324, 11);
		this.lb_Status.Name = "lb_Status";
		this.lb_Status.ReadOnly = false;
		this.lb_Status.SettingInVisible = false;
		this.lb_Status.SettingReadonly = false;
		this.lb_Status.ShowUnderLine = true;
		this.lb_Status.Size = new System.Drawing.Size(114, 20);
		this.lb_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Status.TabIndex = 4;
		this.lb_Status.Utext = "Status";
		this.cbx_Status.EditControl = null;
		this.cbx_Status.FieldName = null;
		this.cbx_Status.IsSearchControl = false;
		this.cbx_Status.Location = new System.Drawing.Point(435, 6);
		this.cbx_Status.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.cbx_Status.Name = "cbx_Status";
		this.cbx_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_Status.SettingInVisible = false;
		this.cbx_Status.SettingReadonly = false;
		this.cbx_Status.Size = new System.Drawing.Size(147, 20);
		this.cbx_Status.TabIndex = 8;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 61);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(726, 366);
		this.splitContainerControl1.SplitterPosition = 182;
		this.splitContainerControl1.TabIndex = 9;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl1.Controls.Add(this.grd_General);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(726, 182);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "General";
		this.grd_General.AllowPasteExcel = true;
		this.grd_General.ClipboardData = null;
		this.grd_General.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_General.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_General.Location = new System.Drawing.Point(2, 23);
		this.grd_General.MainView = this.gv_General;
		this.grd_General.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_General.Name = "grd_General";
		this.grd_General.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rp_CardCode, this.rp_ItemCode, this.sp_NumbericN2, this.rpb_Warehouse, this.rpl_Status, this.rpm_Remark, this.rpl_TreeType, this.rpd_Valid });
		this.grd_General.Size = new System.Drawing.Size(722, 157);
		this.grd_General.TabIndex = 0;
		this.grd_General.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_General });
		this.gv_General.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_General.ColumnKey = null;
		this.gv_General.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.cl_ItemCode, this.cl_ItemName, this.cl_TreeType, this.cl_Quantity, this.cl_Scrap, this.cl_Status, this.cl_ValidFrom, this.cl_ValidTo, this.cl_CardCode, this.cl_CardName,
			this.cl_DistRule, this.cl_ToWH, this.cl_Uom, this.cl_Remark
		});
		this.gv_General.DisableFormSetting = false;
		this.gv_General.DisplayException = false;
		this.gv_General.EditColumn = null;
		this.gv_General.FieldCurrency = null;
		this.gv_General.FieldRate = null;
		this.gv_General.FormId = null;
		this.gv_General.GridControl = this.grd_General;
		this.gv_General.IgnoreFilter = false;
		this.gv_General.IndexEditRow = -1;
		this.gv_General.IndicatorWidth = 35;
		this.gv_General.IsApzTable = false;
		this.gv_General.IsInsertNewRow = false;
		this.gv_General.IsLoadingData = false;
		this.gv_General.IsMappingContextAction = false;
		this.gv_General.ItemId = null;
		this.gv_General.LstColumnAllowBatch = null;
		this.gv_General.LstColumnNotAllowBatch = null;
		this.gv_General.LstColumnNotEditable = null;
		this.gv_General.LstColumNotSetting = null;
		this.gv_General.Name = "gv_General";
		this.gv_General.OptionsView.ColumnAutoWidth = false;
		this.gv_General.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_General.OptionsView.ShowGroupPanel = false;
		this.gv_General.UseBatchMod = false;
		this.gv_General.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_General_RowClick);
		this.gv_General.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_General_CustomDrawRowIndicator);
		this.gv_General.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_General_RowStyle);
		this.gv_General.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_General_PopupMenuShowing);
		this.gv_General.InitNewRow += new DevExpress.XtraGrid.Views.Grid.InitNewRowEventHandler(this.gv_General_InitNewRow);
		this.gv_General.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_General_FocusedRowChanged);
		this.cl_ItemCode.Caption = "Product Code";
		this.cl_ItemCode.ColumnEdit = this.rp_ItemCode;
		this.cl_ItemCode.CurrencyDateField = null;
		this.cl_ItemCode.CurrencyField = null;
		this.cl_ItemCode.CurrencyFixed = null;
		this.cl_ItemCode.CurrencyFixedRate = 0.0;
		this.cl_ItemCode.CurrencyRateField = null;
		this.cl_ItemCode.FieldName = "Code";
		this.cl_ItemCode.Name = "cl_ItemCode";
		this.cl_ItemCode.OptionsColumn.ReadOnly = true;
		this.cl_ItemCode.PrimaryKey = true;
		this.cl_ItemCode.UnitField = null;
		this.cl_ItemCode.Visible = true;
		this.cl_ItemCode.VisibleIndex = 0;
		this.cl_ItemCode.Width = 105;
		this.rp_ItemCode.AutoHeight = false;
		this.rp_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rp_ItemCode.Name = "rp_ItemCode";
		this.rp_ItemCode.ReadOnly = true;
		this.rp_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rp_ItemCode_ButtonPressed);
		this.cl_ItemName.Caption = "Product Name";
		this.cl_ItemName.CurrencyDateField = null;
		this.cl_ItemName.CurrencyField = null;
		this.cl_ItemName.CurrencyFixed = null;
		this.cl_ItemName.CurrencyFixedRate = 0.0;
		this.cl_ItemName.CurrencyRateField = null;
		this.cl_ItemName.FieldName = "ItemName";
		this.cl_ItemName.Name = "cl_ItemName";
		this.cl_ItemName.OptionsColumn.AllowEdit = false;
		this.cl_ItemName.PrimaryKey = false;
		this.cl_ItemName.UnitField = null;
		this.cl_ItemName.Visible = true;
		this.cl_ItemName.VisibleIndex = 1;
		this.cl_ItemName.Width = 149;
		this.cl_TreeType.Caption = "Tree Type";
		this.cl_TreeType.ColumnEdit = this.rpl_TreeType;
		this.cl_TreeType.CurrencyDateField = null;
		this.cl_TreeType.CurrencyField = null;
		this.cl_TreeType.CurrencyFixed = null;
		this.cl_TreeType.CurrencyFixedRate = 0.0;
		this.cl_TreeType.CurrencyRateField = null;
		this.cl_TreeType.FieldName = "TreeType";
		this.cl_TreeType.Name = "cl_TreeType";
		this.cl_TreeType.PrimaryKey = false;
		this.cl_TreeType.UnitField = null;
		this.cl_TreeType.Visible = true;
		this.cl_TreeType.VisibleIndex = 2;
		this.rpl_TreeType.AutoHeight = false;
		this.rpl_TreeType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpl_TreeType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "TreeType")
		});
		this.rpl_TreeType.Name = "rpl_TreeType";
		this.rpl_TreeType.NullText = "";
		this.cl_Quantity.Caption = "Quantity";
		this.cl_Quantity.CurrencyDateField = null;
		this.cl_Quantity.CurrencyField = null;
		this.cl_Quantity.CurrencyFixed = null;
		this.cl_Quantity.CurrencyFixedRate = 0.0;
		this.cl_Quantity.CurrencyRateField = null;
		this.cl_Quantity.DisplayFormat.FormatString = "n2";
		this.cl_Quantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cl_Quantity.FieldName = "Quantity";
		this.cl_Quantity.Name = "cl_Quantity";
		this.cl_Quantity.PrimaryKey = false;
		this.cl_Quantity.UnitField = null;
		this.cl_Quantity.Visible = true;
		this.cl_Quantity.VisibleIndex = 3;
		this.cl_Scrap.Caption = "Scrap";
		this.cl_Scrap.CurrencyDateField = null;
		this.cl_Scrap.CurrencyField = null;
		this.cl_Scrap.CurrencyFixed = null;
		this.cl_Scrap.CurrencyFixedRate = 0.0;
		this.cl_Scrap.CurrencyRateField = null;
		this.cl_Scrap.DisplayFormat.FormatString = "n2";
		this.cl_Scrap.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cl_Scrap.FieldName = "Scrap";
		this.cl_Scrap.Name = "cl_Scrap";
		this.cl_Scrap.PrimaryKey = false;
		this.cl_Scrap.UnitField = null;
		this.cl_Scrap.Visible = true;
		this.cl_Scrap.VisibleIndex = 10;
		this.cl_Status.Caption = "Status";
		this.cl_Status.ColumnEdit = this.rpl_Status;
		this.cl_Status.CurrencyDateField = null;
		this.cl_Status.CurrencyField = null;
		this.cl_Status.CurrencyFixed = null;
		this.cl_Status.CurrencyFixedRate = 0.0;
		this.cl_Status.CurrencyRateField = null;
		this.cl_Status.FieldName = "Status";
		this.cl_Status.Name = "cl_Status";
		this.cl_Status.PrimaryKey = false;
		this.cl_Status.UnitField = null;
		this.cl_Status.Visible = true;
		this.cl_Status.VisibleIndex = 9;
		this.cl_Status.Width = 69;
		this.rpl_Status.AutoHeight = false;
		this.rpl_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpl_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rpl_Status.Name = "rpl_Status";
		this.rpl_Status.NullText = "";
		this.cl_ValidFrom.Caption = "Active From";
		this.cl_ValidFrom.ColumnEdit = this.rpd_Valid;
		this.cl_ValidFrom.CurrencyDateField = null;
		this.cl_ValidFrom.CurrencyField = null;
		this.cl_ValidFrom.CurrencyFixed = null;
		this.cl_ValidFrom.CurrencyFixedRate = 0.0;
		this.cl_ValidFrom.CurrencyRateField = null;
		this.cl_ValidFrom.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_ValidFrom.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_ValidFrom.FieldName = "ValidFrom";
		this.cl_ValidFrom.Name = "cl_ValidFrom";
		this.cl_ValidFrom.PrimaryKey = false;
		this.cl_ValidFrom.UnitField = null;
		this.cl_ValidFrom.Visible = true;
		this.cl_ValidFrom.VisibleIndex = 4;
		this.cl_ValidFrom.Width = 89;
		this.rpd_Valid.AutoHeight = false;
		this.rpd_Valid.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpd_Valid.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpd_Valid.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rpd_Valid.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rpd_Valid.EditFormat.FormatString = "dd/MM/yyyy";
		this.rpd_Valid.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rpd_Valid.Mask.EditMask = "dd/MM/yyyy";
		this.rpd_Valid.Name = "rpd_Valid";
		this.cl_ValidTo.Caption = "Active To";
		this.cl_ValidTo.ColumnEdit = this.rpd_Valid;
		this.cl_ValidTo.CurrencyDateField = null;
		this.cl_ValidTo.CurrencyField = null;
		this.cl_ValidTo.CurrencyFixed = null;
		this.cl_ValidTo.CurrencyFixedRate = 0.0;
		this.cl_ValidTo.CurrencyRateField = null;
		this.cl_ValidTo.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_ValidTo.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_ValidTo.FieldName = "ValidTo";
		this.cl_ValidTo.Name = "cl_ValidTo";
		this.cl_ValidTo.PrimaryKey = false;
		this.cl_ValidTo.UnitField = null;
		this.cl_ValidTo.Visible = true;
		this.cl_ValidTo.VisibleIndex = 5;
		this.cl_ValidTo.Width = 90;
		this.cl_CardCode.Caption = "BP Code";
		this.cl_CardCode.ColumnEdit = this.rp_CardCode;
		this.cl_CardCode.CurrencyDateField = null;
		this.cl_CardCode.CurrencyField = null;
		this.cl_CardCode.CurrencyFixed = null;
		this.cl_CardCode.CurrencyFixedRate = 0.0;
		this.cl_CardCode.CurrencyRateField = null;
		this.cl_CardCode.FieldName = "CardCode";
		this.cl_CardCode.Name = "cl_CardCode";
		this.cl_CardCode.PrimaryKey = false;
		this.cl_CardCode.UnitField = null;
		this.cl_CardCode.Visible = true;
		this.cl_CardCode.VisibleIndex = 6;
		this.cl_CardCode.Width = 104;
		this.rp_CardCode.AutoHeight = false;
		this.rp_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rp_CardCode.Name = "rp_CardCode";
		this.rp_CardCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rp_CardCode_ButtonPressed);
		this.cl_CardName.Caption = "BP Name";
		this.cl_CardName.CurrencyDateField = null;
		this.cl_CardName.CurrencyField = null;
		this.cl_CardName.CurrencyFixed = null;
		this.cl_CardName.CurrencyFixedRate = 0.0;
		this.cl_CardName.CurrencyRateField = null;
		this.cl_CardName.FieldName = "CardName";
		this.cl_CardName.Name = "cl_CardName";
		this.cl_CardName.OptionsColumn.AllowEdit = false;
		this.cl_CardName.PrimaryKey = false;
		this.cl_CardName.UnitField = null;
		this.cl_CardName.Visible = true;
		this.cl_CardName.VisibleIndex = 7;
		this.cl_DistRule.Caption = "Dist. Rule";
		this.cl_DistRule.CurrencyDateField = null;
		this.cl_DistRule.CurrencyField = null;
		this.cl_DistRule.CurrencyFixed = null;
		this.cl_DistRule.CurrencyFixedRate = 0.0;
		this.cl_DistRule.CurrencyRateField = null;
		this.cl_DistRule.FieldName = "OcrCode";
		this.cl_DistRule.Name = "cl_DistRule";
		this.cl_DistRule.PrimaryKey = false;
		this.cl_DistRule.UnitField = null;
		this.cl_DistRule.Visible = true;
		this.cl_DistRule.VisibleIndex = 12;
		this.cl_ToWH.Caption = "Whse";
		this.cl_ToWH.ColumnEdit = this.rpb_Warehouse;
		this.cl_ToWH.CurrencyDateField = null;
		this.cl_ToWH.CurrencyField = null;
		this.cl_ToWH.CurrencyFixed = null;
		this.cl_ToWH.CurrencyFixedRate = 0.0;
		this.cl_ToWH.CurrencyRateField = null;
		this.cl_ToWH.FieldName = "ToWH";
		this.cl_ToWH.Name = "cl_ToWH";
		this.cl_ToWH.PrimaryKey = false;
		this.cl_ToWH.UnitField = null;
		this.cl_ToWH.Visible = true;
		this.cl_ToWH.VisibleIndex = 8;
		this.rpb_Warehouse.AutoHeight = false;
		this.rpb_Warehouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rpb_Warehouse.Name = "rpb_Warehouse";
		this.rpb_Warehouse.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rpb_Warehouse_ButtonPressed);
		this.cl_Uom.Caption = "UoM";
		this.cl_Uom.CurrencyDateField = null;
		this.cl_Uom.CurrencyField = null;
		this.cl_Uom.CurrencyFixed = null;
		this.cl_Uom.CurrencyFixedRate = 0.0;
		this.cl_Uom.CurrencyRateField = null;
		this.cl_Uom.FieldName = "Uom";
		this.cl_Uom.Name = "cl_Uom";
		this.cl_Uom.PrimaryKey = false;
		this.cl_Uom.UnitField = null;
		this.cl_Uom.Visible = true;
		this.cl_Uom.VisibleIndex = 11;
		this.cl_Remark.Caption = "Remark";
		this.cl_Remark.ColumnEdit = this.rpm_Remark;
		this.cl_Remark.CurrencyDateField = null;
		this.cl_Remark.CurrencyField = null;
		this.cl_Remark.CurrencyFixed = null;
		this.cl_Remark.CurrencyFixedRate = 0.0;
		this.cl_Remark.CurrencyRateField = null;
		this.cl_Remark.FieldName = "Remark";
		this.cl_Remark.Name = "cl_Remark";
		this.cl_Remark.PrimaryKey = false;
		this.cl_Remark.UnitField = null;
		this.cl_Remark.Visible = true;
		this.cl_Remark.VisibleIndex = 13;
		this.rpm_Remark.AutoHeight = false;
		this.rpm_Remark.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpm_Remark.Name = "rpm_Remark";
		this.rpm_Remark.ShowIcon = false;
		this.sp_NumbericN2.AutoHeight = false;
		this.sp_NumbericN2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.sp_NumbericN2.DisplayFormat.FormatString = "n2";
		this.sp_NumbericN2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.sp_NumbericN2.EditFormat.FormatString = "n2";
		this.sp_NumbericN2.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.sp_NumbericN2.Name = "sp_NumbericN2";
		this.groupControl2.Controls.Add(this.grd_BOMDetail);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(726, 174);
		this.groupControl2.TabIndex = 0;
		this.groupControl2.Text = "BOM Detail";
		this.grd_BOMDetail.AllowPasteExcel = true;
		this.grd_BOMDetail.ClipboardData = null;
		this.grd_BOMDetail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_BOMDetail.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_BOMDetail.Location = new System.Drawing.Point(2, 23);
		this.grd_BOMDetail.MainView = this.gv_BOMDetail;
		this.grd_BOMDetail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_BOMDetail.Name = "grd_BOMDetail";
		this.grd_BOMDetail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rp_Line_ItemCode, this.rpb_Line_Warehouse, this.rpm_Line_Comment, this.rpl_Line_IssueMth, this.rpl_Line_PriceList, this.rpl_Line_Type });
		this.grd_BOMDetail.Size = new System.Drawing.Size(722, 149);
		this.grd_BOMDetail.TabIndex = 1;
		this.grd_BOMDetail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BOMDetail });
		this.gv_BOMDetail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BOMDetail.ColumnKey = null;
		this.gv_BOMDetail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.cl_Line_Type, this.cl_Line_ItemCode, this.cl_Line_ItemName, this.cl_Line_Quantity, this.cl_Line_IssueMthd, this.cl_Line_PriceList, this.cl_Line_UnitPrice, this.cl_Line_Whse, this.cl_Line_Uom, this.cl_Line_WipAccount,
			this.cl_Line_Project, this.cl_Line_Comment, this.cl_Line_Father, this.cl_Line_ChildNum
		});
		this.gv_BOMDetail.DisableFormSetting = false;
		this.gv_BOMDetail.DisplayException = false;
		this.gv_BOMDetail.EditColumn = null;
		this.gv_BOMDetail.FieldCurrency = null;
		this.gv_BOMDetail.FieldRate = null;
		this.gv_BOMDetail.FormId = null;
		this.gv_BOMDetail.GridControl = this.grd_BOMDetail;
		this.gv_BOMDetail.IgnoreFilter = false;
		this.gv_BOMDetail.IndexEditRow = -1;
		this.gv_BOMDetail.IndicatorWidth = 30;
		this.gv_BOMDetail.IsApzTable = false;
		this.gv_BOMDetail.IsInsertNewRow = false;
		this.gv_BOMDetail.IsLoadingData = false;
		this.gv_BOMDetail.IsMappingContextAction = false;
		this.gv_BOMDetail.ItemId = null;
		this.gv_BOMDetail.LstColumnAllowBatch = null;
		this.gv_BOMDetail.LstColumnNotAllowBatch = null;
		this.gv_BOMDetail.LstColumnNotEditable = null;
		this.gv_BOMDetail.LstColumNotSetting = null;
		this.gv_BOMDetail.Name = "gv_BOMDetail";
		this.gv_BOMDetail.OptionsView.ColumnAutoWidth = false;
		this.gv_BOMDetail.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_BOMDetail.OptionsView.ShowGroupPanel = false;
		this.gv_BOMDetail.UseBatchMod = false;
		this.gv_BOMDetail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_BOMDetail_CustomDrawRowIndicator);
		this.gv_BOMDetail.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_BOMDetail_RowStyle);
		this.gv_BOMDetail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_BOMDetail_CellValueChanged);
		this.cl_Line_Type.Caption = "Type";
		this.cl_Line_Type.ColumnEdit = this.rpl_Line_Type;
		this.cl_Line_Type.CurrencyDateField = null;
		this.cl_Line_Type.CurrencyField = null;
		this.cl_Line_Type.CurrencyFixed = null;
		this.cl_Line_Type.CurrencyFixedRate = 0.0;
		this.cl_Line_Type.CurrencyRateField = null;
		this.cl_Line_Type.FieldName = "Type";
		this.cl_Line_Type.Name = "cl_Line_Type";
		this.cl_Line_Type.PrimaryKey = false;
		this.cl_Line_Type.UnitField = null;
		this.cl_Line_Type.Visible = true;
		this.cl_Line_Type.VisibleIndex = 2;
		this.cl_Line_Type.Width = 60;
		this.rpl_Line_Type.AutoHeight = false;
		this.rpl_Line_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpl_Line_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type")
		});
		this.rpl_Line_Type.Name = "rpl_Line_Type";
		this.rpl_Line_Type.NullText = "";
		this.cl_Line_ItemCode.Caption = "Code";
		this.cl_Line_ItemCode.ColumnEdit = this.rp_Line_ItemCode;
		this.cl_Line_ItemCode.CurrencyDateField = null;
		this.cl_Line_ItemCode.CurrencyField = null;
		this.cl_Line_ItemCode.CurrencyFixed = null;
		this.cl_Line_ItemCode.CurrencyFixedRate = 0.0;
		this.cl_Line_ItemCode.CurrencyRateField = null;
		this.cl_Line_ItemCode.FieldName = "Code";
		this.cl_Line_ItemCode.Name = "cl_Line_ItemCode";
		this.cl_Line_ItemCode.PrimaryKey = true;
		this.cl_Line_ItemCode.UnitField = null;
		this.cl_Line_ItemCode.Visible = true;
		this.cl_Line_ItemCode.VisibleIndex = 0;
		this.cl_Line_ItemCode.Width = 128;
		this.rp_Line_ItemCode.AutoHeight = false;
		this.rp_Line_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rp_Line_ItemCode.Name = "rp_Line_ItemCode";
		this.rp_Line_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rp_Line_ItemCode_ButtonPressed);
		this.cl_Line_ItemName.Caption = "Item Name";
		this.cl_Line_ItemName.CurrencyDateField = null;
		this.cl_Line_ItemName.CurrencyField = null;
		this.cl_Line_ItemName.CurrencyFixed = null;
		this.cl_Line_ItemName.CurrencyFixedRate = 0.0;
		this.cl_Line_ItemName.CurrencyRateField = null;
		this.cl_Line_ItemName.FieldName = "ItemName";
		this.cl_Line_ItemName.Name = "cl_Line_ItemName";
		this.cl_Line_ItemName.PrimaryKey = false;
		this.cl_Line_ItemName.UnitField = null;
		this.cl_Line_ItemName.Visible = true;
		this.cl_Line_ItemName.VisibleIndex = 1;
		this.cl_Line_ItemName.Width = 149;
		this.cl_Line_Quantity.Caption = "Quantity";
		this.cl_Line_Quantity.CurrencyDateField = null;
		this.cl_Line_Quantity.CurrencyField = null;
		this.cl_Line_Quantity.CurrencyFixed = null;
		this.cl_Line_Quantity.CurrencyFixedRate = 0.0;
		this.cl_Line_Quantity.CurrencyRateField = null;
		this.cl_Line_Quantity.DisplayFormat.FormatString = "n2";
		this.cl_Line_Quantity.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cl_Line_Quantity.FieldName = "Quantity";
		this.cl_Line_Quantity.Name = "cl_Line_Quantity";
		this.cl_Line_Quantity.PrimaryKey = false;
		this.cl_Line_Quantity.UnitField = null;
		this.cl_Line_Quantity.Visible = true;
		this.cl_Line_Quantity.VisibleIndex = 3;
		this.cl_Line_Quantity.Width = 79;
		this.cl_Line_IssueMthd.Caption = "Issued Method";
		this.cl_Line_IssueMthd.ColumnEdit = this.rpl_Line_IssueMth;
		this.cl_Line_IssueMthd.CurrencyDateField = null;
		this.cl_Line_IssueMthd.CurrencyField = null;
		this.cl_Line_IssueMthd.CurrencyFixed = null;
		this.cl_Line_IssueMthd.CurrencyFixedRate = 0.0;
		this.cl_Line_IssueMthd.CurrencyRateField = null;
		this.cl_Line_IssueMthd.FieldName = "IssueMthd";
		this.cl_Line_IssueMthd.Name = "cl_Line_IssueMthd";
		this.cl_Line_IssueMthd.PrimaryKey = false;
		this.cl_Line_IssueMthd.UnitField = null;
		this.cl_Line_IssueMthd.Visible = true;
		this.cl_Line_IssueMthd.VisibleIndex = 4;
		this.cl_Line_IssueMthd.Width = 102;
		this.rpl_Line_IssueMth.AutoHeight = false;
		this.rpl_Line_IssueMth.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpl_Line_IssueMth.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Issue method"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rpl_Line_IssueMth.Name = "rpl_Line_IssueMth";
		this.rpl_Line_IssueMth.NullText = "";
		this.cl_Line_PriceList.Caption = "Price List";
		this.cl_Line_PriceList.ColumnEdit = this.rpl_Line_PriceList;
		this.cl_Line_PriceList.CurrencyDateField = null;
		this.cl_Line_PriceList.CurrencyField = null;
		this.cl_Line_PriceList.CurrencyFixed = null;
		this.cl_Line_PriceList.CurrencyFixedRate = 0.0;
		this.cl_Line_PriceList.CurrencyRateField = null;
		this.cl_Line_PriceList.FieldName = "PriceList";
		this.cl_Line_PriceList.Name = "cl_Line_PriceList";
		this.cl_Line_PriceList.PrimaryKey = false;
		this.cl_Line_PriceList.UnitField = null;
		this.cl_Line_PriceList.Visible = true;
		this.cl_Line_PriceList.VisibleIndex = 5;
		this.cl_Line_PriceList.Width = 101;
		this.rpl_Line_PriceList.AutoHeight = false;
		this.rpl_Line_PriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpl_Line_PriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PriceListName", "PriceList"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PriceListNo", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rpl_Line_PriceList.Name = "rpl_Line_PriceList";
		this.rpl_Line_PriceList.NullText = "";
		this.cl_Line_UnitPrice.Caption = "Unit Price";
		this.cl_Line_UnitPrice.CurrencyDateField = null;
		this.cl_Line_UnitPrice.CurrencyField = null;
		this.cl_Line_UnitPrice.CurrencyFixed = null;
		this.cl_Line_UnitPrice.CurrencyFixedRate = 0.0;
		this.cl_Line_UnitPrice.CurrencyRateField = null;
		this.cl_Line_UnitPrice.FieldName = "Price";
		this.cl_Line_UnitPrice.Name = "cl_Line_UnitPrice";
		this.cl_Line_UnitPrice.PrimaryKey = false;
		this.cl_Line_UnitPrice.UnitField = null;
		this.cl_Line_UnitPrice.Visible = true;
		this.cl_Line_UnitPrice.VisibleIndex = 6;
		this.cl_Line_UnitPrice.Width = 93;
		this.cl_Line_Whse.Caption = "Whse";
		this.cl_Line_Whse.ColumnEdit = this.rpb_Line_Warehouse;
		this.cl_Line_Whse.CurrencyDateField = null;
		this.cl_Line_Whse.CurrencyField = null;
		this.cl_Line_Whse.CurrencyFixed = null;
		this.cl_Line_Whse.CurrencyFixedRate = 0.0;
		this.cl_Line_Whse.CurrencyRateField = null;
		this.cl_Line_Whse.FieldName = "Warehouse";
		this.cl_Line_Whse.Name = "cl_Line_Whse";
		this.cl_Line_Whse.PrimaryKey = false;
		this.cl_Line_Whse.UnitField = null;
		this.cl_Line_Whse.Visible = true;
		this.cl_Line_Whse.VisibleIndex = 7;
		this.rpb_Line_Warehouse.AutoHeight = false;
		this.rpb_Line_Warehouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rpb_Line_Warehouse.Name = "rpb_Line_Warehouse";
		this.rpb_Line_Warehouse.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rpb_Line_Warehouse_ButtonPressed);
		this.cl_Line_Uom.Caption = "UoM";
		this.cl_Line_Uom.CurrencyDateField = null;
		this.cl_Line_Uom.CurrencyField = null;
		this.cl_Line_Uom.CurrencyFixed = null;
		this.cl_Line_Uom.CurrencyFixedRate = 0.0;
		this.cl_Line_Uom.CurrencyRateField = null;
		this.cl_Line_Uom.FieldName = "Uom";
		this.cl_Line_Uom.Name = "cl_Line_Uom";
		this.cl_Line_Uom.PrimaryKey = false;
		this.cl_Line_Uom.UnitField = null;
		this.cl_Line_Uom.Visible = true;
		this.cl_Line_Uom.VisibleIndex = 8;
		this.cl_Line_WipAccount.Caption = "Wip Account";
		this.cl_Line_WipAccount.CurrencyDateField = null;
		this.cl_Line_WipAccount.CurrencyField = null;
		this.cl_Line_WipAccount.CurrencyFixed = null;
		this.cl_Line_WipAccount.CurrencyFixedRate = 0.0;
		this.cl_Line_WipAccount.CurrencyRateField = null;
		this.cl_Line_WipAccount.FieldName = "WipActCode";
		this.cl_Line_WipAccount.Name = "cl_Line_WipAccount";
		this.cl_Line_WipAccount.PrimaryKey = false;
		this.cl_Line_WipAccount.UnitField = null;
		this.cl_Line_WipAccount.Visible = true;
		this.cl_Line_WipAccount.VisibleIndex = 9;
		this.cl_Line_Project.Caption = "Project";
		this.cl_Line_Project.CurrencyDateField = null;
		this.cl_Line_Project.CurrencyField = null;
		this.cl_Line_Project.CurrencyFixed = null;
		this.cl_Line_Project.CurrencyFixedRate = 0.0;
		this.cl_Line_Project.CurrencyRateField = null;
		this.cl_Line_Project.FieldName = "Project";
		this.cl_Line_Project.Name = "cl_Line_Project";
		this.cl_Line_Project.PrimaryKey = false;
		this.cl_Line_Project.UnitField = null;
		this.cl_Line_Project.Visible = true;
		this.cl_Line_Project.VisibleIndex = 10;
		this.cl_Line_Comment.Caption = "Comment";
		this.cl_Line_Comment.ColumnEdit = this.rpm_Line_Comment;
		this.cl_Line_Comment.CurrencyDateField = null;
		this.cl_Line_Comment.CurrencyField = null;
		this.cl_Line_Comment.CurrencyFixed = null;
		this.cl_Line_Comment.CurrencyFixedRate = 0.0;
		this.cl_Line_Comment.CurrencyRateField = null;
		this.cl_Line_Comment.FieldName = "Comments";
		this.cl_Line_Comment.Name = "cl_Line_Comment";
		this.cl_Line_Comment.PrimaryKey = false;
		this.cl_Line_Comment.UnitField = null;
		this.cl_Line_Comment.Visible = true;
		this.cl_Line_Comment.VisibleIndex = 11;
		this.rpm_Line_Comment.AutoHeight = false;
		this.rpm_Line_Comment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rpm_Line_Comment.Name = "rpm_Line_Comment";
		this.rpm_Line_Comment.ShowIcon = false;
		this.cl_Line_Father.Caption = "Father";
		this.cl_Line_Father.CurrencyDateField = null;
		this.cl_Line_Father.CurrencyField = null;
		this.cl_Line_Father.CurrencyFixed = null;
		this.cl_Line_Father.CurrencyFixedRate = 0.0;
		this.cl_Line_Father.CurrencyRateField = null;
		this.cl_Line_Father.FieldName = "FatherCode";
		this.cl_Line_Father.Name = "cl_Line_Father";
		this.cl_Line_Father.PrimaryKey = false;
		this.cl_Line_Father.UnitField = null;
		this.cl_Line_ChildNum.Caption = "ChildNum";
		this.cl_Line_ChildNum.CurrencyDateField = null;
		this.cl_Line_ChildNum.CurrencyField = null;
		this.cl_Line_ChildNum.CurrencyFixed = null;
		this.cl_Line_ChildNum.CurrencyFixedRate = 0.0;
		this.cl_Line_ChildNum.CurrencyRateField = null;
		this.cl_Line_ChildNum.FieldName = "ChildNum";
		this.cl_Line_ChildNum.Name = "cl_Line_ChildNum";
		this.cl_Line_ChildNum.PrimaryKey = false;
		this.cl_Line_ChildNum.UnitField = null;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(587, 30);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 21;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.txt_ItemCode.EditControl = null;
		this.txt_ItemCode.FieldName = null;
		this.txt_ItemCode.IsSearchControl = false;
		this.txt_ItemCode.Location = new System.Drawing.Point(122, 8);
		this.txt_ItemCode.Name = "txt_ItemCode";
		this.txt_ItemCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txt_ItemCode.Properties.Appearance.Options.UseFont = true;
		this.txt_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_ItemCode.SettingInVisible = false;
		this.txt_ItemCode.SettingReadonly = false;
		this.txt_ItemCode.Size = new System.Drawing.Size(147, 18);
		this.txt_ItemCode.TabIndex = 48;
		this.txt_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ItemCode_ButtonClick);
		this.bte_CostCenter.EditControl = null;
		this.bte_CostCenter.FieldName = null;
		this.bte_CostCenter.IsSearchControl = false;
		this.bte_CostCenter.Location = new System.Drawing.Point(435, 32);
		this.bte_CostCenter.Name = "bte_CostCenter";
		this.bte_CostCenter.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.bte_CostCenter.Properties.Appearance.Options.UseFont = true;
		this.bte_CostCenter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_CostCenter.SettingInVisible = false;
		this.bte_CostCenter.SettingReadonly = false;
		this.bte_CostCenter.Size = new System.Drawing.Size(147, 18);
		this.bte_CostCenter.TabIndex = 49;
		this.panel1.Controls.Add(this.btn_Refresh);
		this.panel1.Controls.Add(this.btn_Cancel);
		this.panel1.Controls.Add(this.btn_Save);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 429);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(728, 33);
		this.panel1.TabIndex = 50;
		this.panel1.Tag = "ss";
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(183, 7);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(85, 23);
		this.btn_Refresh.TabIndex = 44;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(94, 7);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(85, 23);
		this.btn_Cancel.TabIndex = 14;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 7);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(85, 23);
		this.btn_Save.TabIndex = 13;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(728, 462);
		base.Controls.Add(this.panel1);
		base.Controls.Add(this.bte_CostCenter);
		base.Controls.Add(this.txt_ItemCode);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.cbx_Status);
		base.Controls.Add(this.lb_CostCenter);
		base.Controls.Add(this.lb_Status);
		base.Controls.Add(this.txt_ItemName);
		base.Controls.Add(this.lb_ProductName);
		base.Controls.Add(this.lb_ProductCode);
		base.IconOptions.ShowIcon = false;
		base.KeyPreview = true;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_BOM";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Bill Of Materials";
		base.Load += new System.EventHandler(this.frm_BOM_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_ItemName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_General).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_General).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rp_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_TreeType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpd_Valid.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpd_Valid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rp_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpb_Warehouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpm_Remark).EndInit();
		((System.ComponentModel.ISupportInitialize)this.sp_NumbericN2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_BOMDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BOMDetail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Line_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rp_Line_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Line_IssueMth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpl_Line_PriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpb_Line_Warehouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rpm_Line_Comment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_CostCenter.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
