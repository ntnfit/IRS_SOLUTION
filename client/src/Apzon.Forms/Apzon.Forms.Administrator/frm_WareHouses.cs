using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_WareHouses : XtraFormEx
{
	private bool _isNettableChange;

	private bool _isChangFocus;

	private bool _isLoading;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string _whsSearchCode = "";

	private string _formController = "Warehouse";

	private DataTable dtObj = new DataTable();

	private DataTable _dtOriginal = new DataTable();

	private List<string> lstChangedWarehouse = new List<string>();

	private List<StateRepControls> lstStateControls = new List<StateRepControls>();

	private DataView clone;

	private List<RepositoryItemLookUpEdit> lst = new List<RepositoryItemLookUpEdit>();

	private string _privilege = "";

	private List<UserFieldsImpl> _lstUdfFields;

	private Dictionary<int, string> _dicAutoIssueMtd = new Dictionary<int, string>
	{
		{ 0, "Single Choice" },
		{ 1, "Bin Location Code Order" },
		{ 2, "Alternative Sort Code Order" },
		{ 3, "Descending Quantity" },
		{ 4, "Ascending Quantity" },
		{ 5, "FIFO" },
		{ 6, "LIFO" }
	};

	private Dictionary<int, string> _dicAutoReceipt = new Dictionary<int, string>
	{
		{ 0, "Default Bin Location" },
		{ 1, "Last Bin Location That Received Item" },
		{ 2, "Item's Current Bin Locations" },
		{ 3, "Item's Current and Historical Bin Locations" }
	};

	private Dictionary<int, string> _dicRecItemsBy = new Dictionary<int, string>
	{
		{ 0, "Bin Location Code Order" },
		{ 1, "Alternative Sort Code Order" }
	};

	private Dictionary<string, string> _dicRecvUpTo = new Dictionary<string, string>
	{
		{ "0", "Maximum Qty" },
		{ "1", "Maximum Weight" },
		{ "2", "Max. Qty and Weight" }
	};

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private TextEditEx txt_WarehouseCode;

	private TextEditEx txt_WarehouseName;

	private LabelLine lb_WareHouseCode;

	private LabelLine lb_WarehouseName;

	private SimpleButtonEx bt_Search;

	private LabelLine lb_Branch;

	private LookUpEdit lookUE_Branch;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private GridColumnEx gcWarehouseCode;

	private GridColumnEx gcWarehouseName;

	private GridColumnEx gcLocation;

	private GridColumnEx gcBuildingFloorRoom;

	private GridColumnEx gcStreet;

	private GridColumnEx gcStreetNo;

	private GridColumnEx gcBlock;

	private GridColumnEx gcZipCode;

	private GridColumnEx gcCity;

	private GridColumnEx gcFederalTaxID;

	private GridColumnEx gcObjTypeHistory;

	private GridColumnEx gcLocked;

	private RepositoryItemCheckEdit rep_chk_Inactive;

	private GridColumnEx gcDropShip;

	private GridColumnEx gcNettables;

	private GridColumnEx gcBPLid;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gcAddressType;

	private GridColumnEx gcCounty;

	private GridColumnEx gcCountry;

	private RepositoryItemLookUpEdit rep_Country;

	private GridColumnEx gcState;

	private RepositoryItemLookUpEdit rep_State;

	private GridColumnEx gcGlblLocNum;

	private GridColumnEx gcBinActivat;

	private RepositoryItemLookUpEdit rep_Location;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabPage_BinLocations;

	private GridControlEx grd_BinLocation;

	private GridViewEx gv_BinLocation;

	private GridColumnEx gcBinSeptor;

	private GridColumnEx gcRecBinEnab;

	private GridColumnEx gcNoOfBinLocations;

	private GridColumnEx gcDefaultBinLocation;

	private GridColumnEx gcDftBinEnfd;

	private GridColumnEx gcAutoIssMtd;

	private GridColumnEx gcAutoRecvMd;

	private GridColumnEx gcRecvMaxQty;

	private GridColumnEx gcRecvMaxWT;

	private XtraTabPageEx tabPage_Accounting;

	private GridControlEx grd_Account;

	private GridViewEx gv_Account;

	private RepositoryItemCalcEdit rep_calc_NoOfBinLoc;

	private RepositoryItemCheckEdit rep_bin_DftBinEnfd;

	private RepositoryItemLookUpEdit rep_bin_AutoIssMtd;

	private RepositoryItemLookUpEdit rep_bin_AutoRecvMd;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

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

	private GridColumnEx gridColumn24;

	private RepositoryItemCheckEdit rep_chk_Nettable;

	private RepositoryItemCheckEdit rep_chk_BinActivat;

	private GridColumnEx gridColumn25;

	private RepositoryItemButtonEdit rep_bte_Account;

	private RepositoryItemCheckEdit rep_bin_RecBinEnab;

	private GridColumnEx gridColumn26;

	private RepositoryItemLookUpEdit rep_bin_RecItemsBy;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private RepositoryItemLookUpEdit rep_bin_RecvUpTo;

	private RepositoryItemButtonEdit rep_bte_DftBinAbs;

	private GridColumnEx gridColumn29;

	private RepositoryItemCheckEdit rep_chk_DropShip;

	private GridColumnEx gridColumn30;

	private RepositoryItemCheckEdit rep_chk_ManageSnB;

	public bool _isRowObjectChanging { get; set; }

	public bool _isFocusedRowChanging { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			bt_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_WareHouses()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_WareHouses"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_WareHouses" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				bt_Cancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			FormId = "oWarehouse";
			gv_Doc.ItemId = "OWHS";
			gv_Doc.FormId = FormId;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_WareHouses_Load(object sender, EventArgs e)
	{
		try
		{
			WinFuntions.LoadUdfColumns(gv_Doc, "OWHS");
			RefreshData();
			tabPage_Accounting.PageVisible = GlobalData.LoginSap;
			xtraTabControl1.SelectedTabPageIndex = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_chk_BinActivat_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			BaseEdit obj = sender as BaseEdit;
			gv_Doc.PostEditor();
			if (Function.ToString(obj.EditValue) == "Y")
			{
				tabPage_BinLocations.PageVisible = true;
				if (!(grd_BinLocation.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
				{
					DataTable tableStructure = WinFuntions.GetTableStructure("OWHS");
					DataRow row = tableStructure.NewRow();
					tableStructure.Rows.Add(row);
					tableStructure.AcceptChanges();
					grd_BinLocation.DataSource = tableStructure;
					grd_BinLocation.RefreshDataSource();
				}
			}
			else
			{
				tabPage_BinLocations.PageVisible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bin_RecBinEnab_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			BaseEdit obj = sender as BaseEdit;
			gv_BinLocation.PostEditor();
			if (Function.ToString(obj.EditValue) == "Y")
			{
				gv_BinLocation.Columns["RecItemsBy"].Visible = true;
				gv_BinLocation.Columns["RecvEmpBin"].Visible = true;
				gv_BinLocation.Columns["RecvUpTo"].Visible = true;
			}
			else
			{
				gv_BinLocation.Columns["RecItemsBy"].Visible = false;
				gv_BinLocation.Columns["RecvEmpBin"].Visible = false;
				gv_BinLocation.Columns["RecvUpTo"].Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_General_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (gv_Doc.FocusedColumn.FieldName == "WarehouseCode")
				{
					e.Cancel = true;
				}
				else
				{
					e.Cancel = false;
				}
			}
			if (gv_Doc.FocusedColumn.FieldName == "Nettable" && Function.ToString(gv_Doc.GetFocusedRowCellValue("DropShip")) == "Y")
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "DropShip" && Function.ToString(gv_Doc.GetFocusedRowCellValue("Nettable")) == "Y")
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "ManageSnB" && Function.ToString(gv_Doc.GetFocusedRowCellValue("DropShip")) == "N")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string whsCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"));
			if (e.Column.FieldName == "WhsCode" && FormMode == Enums.FormViewModeType.Add)
			{
				DataTable dataTable = await ExtenseionMethods.GetWareHouseByCode(whsCode);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.WarehouseCodeAlreadyExistsEnterAnotherWhsCode, "");
					gv_Doc.FocusedColumn = e.Column;
					gv_Doc.FocusedRowHandle = e.RowHandle;
					return;
				}
			}
			if (e.Column.FieldName == "Nettable")
			{
				if (!_isNettableChange)
				{
					_isNettableChange = true;
				}
				if (Function.ToString(e.Value) == "Y")
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "DropShip", "N");
				}
				gv_Doc.SetRowCellValue(e.RowHandle, "ManageSnB", "N");
				_isNettableChange = false;
			}
			if (e.Column.FieldName == "DropShip")
			{
				if (!_isNettableChange)
				{
					_isNettableChange = true;
				}
				if (Function.ToString(e.Value) == "Y")
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "Nettable", "N");
				}
				gv_Doc.SetRowCellValue(e.RowHandle, "ManageSnB", "N");
				_isNettableChange = false;
			}
			SetUpdateTextButtonOK();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_BinLocation_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetUpdateTextButtonOK();
	}

	private void gv_General_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "State"))
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Country"));
			for (int i = 0; i < lst.Count; i++)
			{
				if (lst[i].Name == text)
				{
					e.RepositoryItem = lst[i];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_General_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName == "State"))
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Country"));
			for (int i = 0; i < lst.Count; i++)
			{
				if (lst[i].Name == text)
				{
					e.RepositoryItem = lst[i];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_General_ShownEditor(object sender, EventArgs e)
	{
	}

	private void gv_General_HiddenEditor(object sender, EventArgs e)
	{
		try
		{
			if (clone != null)
			{
				clone.Dispose();
				clone = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					grd_Doc.DataSource = _dtOriginal.Copy();
					grd_Doc.RefreshDataSource();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_isFocusedRowChanging = true;
		try
		{
			if (_isRowObjectChanging)
			{
				_isFocusedRowChanging = false;
				return;
			}
			LoadDetails();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isFocusedRowChanging = false;
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_isRowObjectChanging = true;
		try
		{
			if (_isFocusedRowChanging)
			{
				_isRowObjectChanging = false;
				return;
			}
			LoadDetails();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isRowObjectChanging = false;
	}

	private void rILUE_State_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_State frm_State2 = new frm_State();
				frm_State2._CodeLoadFrom = Function.ToString(gv_Doc.GetFocusedRowCellValue("Country"));
				frm_State2.FormClosing += (object s, FormClosingEventArgs e2) =>
				{
				};
				frm_State2.MdiParent = MdiParent;
				frm_State2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		bt_OK.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
				{
					if (!GlobalData.LoginSap)
					{
						await AddNoSAP();
					}
					else
					{
						await Add();
					}
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		bt_OK.Enabled = true;
	}

	private void bt_Search_Click(object sender, EventArgs e)
	{
		try
		{
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"));
			if (!string.IsNullOrWhiteSpace(text) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveWhs, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				string text2 = await _httpClient.Warehouse.DeleteWarehouse(text);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
					bt_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_bte_Account_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			BaseEdit baseEdit = sender as BaseEdit;
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ChartOfAccount, false, Function.ToString(baseEdit.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.Text = "List of Accounts";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Account.SetFocusedValue(Function.ToString(frm_CFL2.ChooseItem["AcctCode"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_DftBinAbs_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"))) && (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListOfBinLocationByWareHouse, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.DataString1 = Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"));
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_BinLocation.SetFocusedRowCellValue("DftBinAbs", Function.ToString(frm_CFL2.ChooseItem["AbsEntry"]));
					gv_BinLocation.SetFocusedRowCellValue("DftBinAbsCode", Function.ToString(frm_CFL2.ChooseItem["BinCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Add()
	{
		_ = 1;
		try
		{
			string whsCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"));
			if (string.IsNullOrEmpty(whsCode))
			{
				WinCommons.Error(Enums.MessageCode.InvalidWhsCode, "");
				return;
			}
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable == null && dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("OWHS");
			int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
			if (focusedDataSourceRowIndex <= -1)
			{
				return;
			}
			DataRow row = dataTable.Rows[focusedDataSourceRowIndex];
			if (grd_BinLocation.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
			{
				row["BinSeptor"] = Function.ToString(dataTable2.Rows[0]["BinSeptor"]);
				row["RecBinEnab"] = Function.ToString(dataTable2.Rows[0]["RecBinEnab"]);
				row["DftBinEnfd"] = Function.ToString(dataTable2.Rows[0]["DftBinEnfd"]);
				row["AutoIssMtd"] = Function.ParseInt(dataTable2.Rows[0]["AutoIssMtd"]);
				row["AutoRecvMd"] = Function.ParseInt(dataTable2.Rows[0]["AutoRecvMd"]);
				row["RecvMaxQty"] = Function.ToString(dataTable2.Rows[0]["RecvMaxQty"]);
				row["RecvMaxWT"] = Function.ToString(dataTable2.Rows[0]["RecvMaxWT"]);
			}
			if (grd_Account.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
			{
				row["IncresGlAc"] = Function.ToString(dataTable3.Rows[0]["IncresGlAc"]);
				row["WipAcct"] = Function.ToString(dataTable3.Rows[0]["WipAcct"]);
				row["WipVarAcct"] = Function.ToString(dataTable3.Rows[0]["WipVarAcct"]);
				row["WipOffset"] = Function.ToString(dataTable3.Rows[0]["WipOffset"]);
				row["StockOffst"] = Function.ToString(dataTable3.Rows[0]["StockOffst"]);
				row["ExpClrAct"] = Function.ToString(dataTable3.Rows[0]["ExpClrAct"]);
				row["StkInTnAct"] = Function.ToString(dataTable3.Rows[0]["StkInTnAct"]);
				row["ShpdGdsAct"] = Function.ToString(dataTable3.Rows[0]["ShpdGdsAct"]);
				row["ARCMAct"] = Function.ToString(dataTable3.Rows[0]["ARCMAct"]);
				row["APCMAct"] = Function.ToString(dataTable3.Rows[0]["APCMAct"]);
				row["NegStckAct"] = Function.ToString(dataTable3.Rows[0]["NegStckAct"]);
				row["DecreasAc"] = Function.ToString(dataTable3.Rows[0]["DecreasAc"]);
				row["IncreasAc"] = Function.ToString(dataTable3.Rows[0]["IncreasAc"]);
				row["ReturnAc"] = Function.ToString(dataTable3.Rows[0]["ReturnAc"]);
				row["ExchangeAc"] = Function.ToString(dataTable3.Rows[0]["ExchangeAc"]);
				row["BalanceAcc"] = Function.ToString(dataTable3.Rows[0]["BalanceAcc"]);
				row["DecresGlAc"] = Function.ToString(dataTable3.Rows[0]["DecresGlAc"]);
				row["ExpensesAc"] = Function.ToString(dataTable3.Rows[0]["ExpensesAc"]);
				row["RevenuesAc"] = Function.ToString(dataTable3.Rows[0]["RevenuesAc"]);
				row["BalInvntAc"] = Function.ToString(dataTable3.Rows[0]["BalInvntAc"]);
				row["SaleCostAc"] = Function.ToString(dataTable3.Rows[0]["SaleCostAc"]);
				row["TransferAc"] = Function.ToString(dataTable3.Rows[0]["TransferAc"]);
				row["VarianceAc"] = Function.ToString(dataTable3.Rows[0]["VarianceAc"]);
				row["PriceDifAc"] = Function.ToString(dataTable3.Rows[0]["PriceDifAc"]);
			}
			tableStructure.ImportRow(row);
			tableStructure.AcceptChanges();
			WarehouseImpl obj = (WarehouseImpl)TableToEntity("OWHS", tableStructure);
			_lstUdfFields = await WinFuntions.GetUdfFields("OWHS");
			if (_lstUdfFields != null && _lstUdfFields.Count > 0 && grd_Doc.DataSource is DataTable dataTable4 && dataTable4.Rows.Count > 0)
			{
				obj.UserFields = new List<UserFieldsImpl>();
				for (int i = 0; i < _lstUdfFields.Count; i++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = _lstUdfFields[i].Name;
					userFieldsImpl.Value = row[_lstUdfFields[i].Name];
					userFieldsImpl.Type = _lstUdfFields[i].Type;
					obj.UserFields.Add(userFieldsImpl);
				}
			}
			string text = await Save(obj);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons._OpenProcessing();
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			FormMode = Enums.FormViewModeType.View;
			RefreshData();
			int focusedRowHandle = gv_Doc.LocateByValue("WhsCode", whsCode);
			gv_Doc.FocusedRowHandle = focusedRowHandle;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowHideBinLocationColumn()
	{
		try
		{
			if (Function.ToString(gv_BinLocation.GetFocusedRowCellValue("RecBinEnab")) == "Y")
			{
				gv_BinLocation.Columns["RecItemsBy"].Visible = true;
				gv_BinLocation.Columns["RecvEmpBin"].Visible = true;
				gv_BinLocation.Columns["RecvUpTo"].Visible = true;
			}
			else
			{
				gv_BinLocation.Columns["RecItemsBy"].Visible = false;
				gv_BinLocation.Columns["RecvEmpBin"].Visible = false;
				gv_BinLocation.Columns["RecvUpTo"].Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNoSAP()
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode"))))
			{
				WinCommons.Error(Enums.MessageCode.InvalidWhsCode, "");
				return;
			}
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable == null && dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			DataTable tableStructure = WinFuntions.GetTableStructure("OWHS");
			int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
			if (focusedDataSourceRowIndex > -1)
			{
				DataRow dataRow = dataTable.Rows[focusedDataSourceRowIndex];
				if (grd_BinLocation.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
				{
					dataRow["BinSeptor"] = Function.ToString(dataTable2.Rows[0]["BinSeptor"]);
					dataRow["RecBinEnab"] = Function.ToString(dataTable2.Rows[0]["RecBinEnab"]);
					dataRow["DftBinEnfd"] = Function.ToString(dataTable2.Rows[0]["DftBinEnfd"]);
					dataRow["AutoIssMtd"] = Function.ParseInt(dataTable2.Rows[0]["AutoIssMtd"]);
					dataRow["AutoRecvMd"] = Function.ParseInt(dataTable2.Rows[0]["AutoRecvMd"]);
					dataRow["RecvMaxQty"] = Function.ToString(dataTable2.Rows[0]["RecvMaxQty"]);
					dataRow["RecvMaxWT"] = Function.ToString(dataTable2.Rows[0]["RecvMaxWT"]);
					dataRow["DftBinAbs"] = Function.ParseInt(dataTable2.Rows[0]["DftBinAbs"]);
					dataRow["RecItemsBy"] = Function.ParseInt(dataTable2.Rows[0]["RecItemsBy"]);
					dataRow["RecvEmpBin"] = Function.ToString(dataTable2.Rows[0]["RecvEmpBin"]);
					dataRow["RecvUpTo"] = Function.ToString(dataTable2.Rows[0]["RecvUpTo"]);
				}
				if (grd_Account.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
				{
					dataRow["IncresGlAc"] = Function.ToString(dataTable3.Rows[0]["IncresGlAc"]);
					dataRow["WipAcct"] = Function.ToString(dataTable3.Rows[0]["WipAcct"]);
					dataRow["WipVarAcct"] = Function.ToString(dataTable3.Rows[0]["WipVarAcct"]);
					dataRow["WipOffset"] = Function.ToString(dataTable3.Rows[0]["WipOffset"]);
					dataRow["StockOffst"] = Function.ToString(dataTable3.Rows[0]["StockOffst"]);
					dataRow["ExpClrAct"] = Function.ToString(dataTable3.Rows[0]["ExpClrAct"]);
					dataRow["StkInTnAct"] = Function.ToString(dataTable3.Rows[0]["StkInTnAct"]);
					dataRow["ShpdGdsAct"] = Function.ToString(dataTable3.Rows[0]["ShpdGdsAct"]);
					dataRow["ARCMAct"] = Function.ToString(dataTable3.Rows[0]["ARCMAct"]);
					dataRow["APCMAct"] = Function.ToString(dataTable3.Rows[0]["APCMAct"]);
					dataRow["NegStckAct"] = Function.ToString(dataTable3.Rows[0]["NegStckAct"]);
					dataRow["DecreasAc"] = Function.ToString(dataTable3.Rows[0]["DecreasAc"]);
					dataRow["IncreasAc"] = Function.ToString(dataTable3.Rows[0]["IncreasAc"]);
					dataRow["ReturnAc"] = Function.ToString(dataTable3.Rows[0]["ReturnAc"]);
					dataRow["ExchangeAc"] = Function.ToString(dataTable3.Rows[0]["ExchangeAc"]);
					dataRow["BalanceAcc"] = Function.ToString(dataTable3.Rows[0]["BalanceAcc"]);
					dataRow["DecresGlAc"] = Function.ToString(dataTable3.Rows[0]["DecresGlAc"]);
					dataRow["ExpensesAc"] = Function.ToString(dataTable3.Rows[0]["ExpensesAc"]);
					dataRow["RevenuesAc"] = Function.ToString(dataTable3.Rows[0]["RevenuesAc"]);
					dataRow["BalInvntAc"] = Function.ToString(dataTable3.Rows[0]["BalInvntAc"]);
					dataRow["SaleCostAc"] = Function.ToString(dataTable3.Rows[0]["SaleCostAc"]);
					dataRow["TransferAc"] = Function.ToString(dataTable3.Rows[0]["TransferAc"]);
					dataRow["VarianceAc"] = Function.ToString(dataTable3.Rows[0]["VarianceAc"]);
					dataRow["PriceDifAc"] = Function.ToString(dataTable3.Rows[0]["PriceDifAc"]);
				}
				tableStructure.ImportRow(dataRow);
				if (tableStructure.Columns.Contains("DftBinAbsCode"))
				{
					tableStructure.Columns.Remove("DftBinAbsCode");
				}
				tableStructure.AcceptChanges();
				string text = await SaveNoSAP(tableStructure);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons._OpenProcessing();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				RefreshData();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> Save(WarehouseImpl obj)
	{
		string result = string.Empty;
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			WinCommons._OpenProcessing();
			result = await hp.PostAsync((FormMode == Enums.FormViewModeType.Add) ? "Save" : "Update", obj);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private async Task<string> SaveNoSAP(DataTable dt)
	{
		string result = string.Empty;
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			WinCommons._OpenProcessing();
			result = await hp.PostAsync("SaveDataTable", dt);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private async Task<DataTable> GetListAllState()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "State");
			return await hp.GetAsync("GetListAllState");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void DeleteRowEmpty()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[i]["WarehouseCode"]).ToString()))
				{
					list.Add(dataTable.Rows[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				dataTable.Rows.Remove(list[j]);
			}
			dtObj = dataTable;
			dataTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Search()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			new WarehouseImpl();
			string whsCode = txt_WarehouseCode.Text;
			string whsName = txt_WarehouseName.Text;
			int bPLid = Function.ParseInt(lookUE_Branch.EditValue);
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = httpClientHelper.Get("Search", new { whsCode, whsName, bPLid });
			if (dataTable != null)
			{
				_dtOriginal = dataTable.Copy();
			}
			else
			{
				_dtOriginal = new DataTable();
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				gv_BinLocation.Columns["DftBinAbs"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["WhsCode"].OptionsColumn.AllowEdit = false;
				LoadDetails();
			}
			else
			{
				dataTable = new DataTable();
				grd_Doc.DataSource = dataTable;
				_dtOriginal = dataTable;
				WinCommons.Warning(Enums.MessageCode.NoDataFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshData()
	{
		try
		{
			lstChangedWarehouse.Clear();
			LoadComBo();
			if (!string.IsNullOrEmpty(_whsSearchCode))
			{
				txt_WarehouseCode.Text = _whsSearchCode;
			}
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComBo()
	{
		try
		{
			GetLocation();
			GetBranch();
			GetCountry();
			GetLookUpEditSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetLookUpEditSource()
	{
		try
		{
			List<KeyValuePair<int, string>> list = _dicAutoIssueMtd.ToList();
			rep_bin_AutoIssMtd.Columns.Clear();
			rep_bin_AutoIssMtd.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			rep_bin_AutoIssMtd.DisplayMember = "Name";
			rep_bin_AutoIssMtd.ValueMember = "Code";
			rep_bin_AutoIssMtd.NullText = "";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(int));
			for (int i = 0; i < list.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = list[i].Key;
				dataRow["Name"] = list[i].Value;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			rep_bin_AutoIssMtd.DataSource = dataTable;
			List<KeyValuePair<int, string>> list2 = _dicAutoReceipt.ToList();
			rep_bin_AutoIssMtd.Columns.Clear();
			rep_bin_AutoIssMtd.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			rep_bin_AutoIssMtd.DisplayMember = "Name";
			rep_bin_AutoIssMtd.ValueMember = "Code";
			rep_bin_AutoIssMtd.NullText = "";
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Name", typeof(string));
			dataTable2.Columns.Add("Code", typeof(int));
			for (int j = 0; j < list2.Count; j++)
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["Code"] = list2[j].Key;
				dataRow2["Name"] = list2[j].Value;
				dataTable2.Rows.Add(dataRow2);
			}
			dataTable2.AcceptChanges();
			rep_bin_AutoRecvMd.DataSource = dataTable2;
			List<KeyValuePair<int, string>> list3 = _dicRecItemsBy.ToList();
			rep_bin_RecItemsBy.Columns.Clear();
			rep_bin_RecItemsBy.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			rep_bin_RecItemsBy.DisplayMember = "Name";
			rep_bin_RecItemsBy.ValueMember = "Code";
			rep_bin_RecItemsBy.NullText = "";
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("Name", typeof(string));
			dataTable3.Columns.Add("Code", typeof(int));
			for (int k = 0; k < list3.Count; k++)
			{
				DataRow dataRow3 = dataTable3.NewRow();
				dataRow3["Code"] = list3[k].Key;
				dataRow3["Name"] = list3[k].Value;
				dataTable3.Rows.Add(dataRow3);
			}
			dataTable3.AcceptChanges();
			rep_bin_RecItemsBy.DataSource = dataTable3;
			List<KeyValuePair<string, string>> list4 = _dicRecvUpTo.ToList();
			rep_bin_RecvUpTo.Columns.Clear();
			rep_bin_RecvUpTo.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "Name"
			});
			rep_bin_RecvUpTo.DisplayMember = "Name";
			rep_bin_RecvUpTo.ValueMember = "Code";
			rep_bin_RecvUpTo.NullText = "";
			DataTable dataTable4 = new DataTable();
			dataTable4.Columns.Add("Name", typeof(string));
			dataTable4.Columns.Add("Code", typeof(string));
			for (int l = 0; l < list4.Count; l++)
			{
				DataRow dataRow4 = dataTable4.NewRow();
				dataRow4["Code"] = list4[l].Key;
				dataRow4["Name"] = list4[l].Value;
				dataTable4.Rows.Add(dataRow4);
			}
			dataTable4.AcceptChanges();
			rep_bin_RecvUpTo.DataSource = dataTable4;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetLocation()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Location");
			DataTable dataSource = httpClientHelper.Get("GetLocation");
			rep_Location.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetCountry()
	{
		int userID = GlobalData.UserID;
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = httpClientHelper.Get("GetCountry", new
			{
				user = userID
			});
			rep_Country.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetBranch()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = httpClientHelper.Get("GetBranchList", new
			{
				userCode = GlobalData.UserCodeLogin
			});
			lookUE_Branch.Properties.DataSource = dataSource;
			rep_Branch.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private object TableToEntity(string table, DataTable dt)
	{
		object result = new object();
		try
		{
			new List<PropertyInfo>();
			if (table == "OWHS")
			{
				result = (WarehouseImpl)WinFuntions.DataRowToEntity(new WarehouseImpl(), dt.Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void LoadDetails()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				int focusedDataSourceRowIndex = gv_Doc.GetFocusedDataSourceRowIndex();
				if (focusedDataSourceRowIndex > -1)
				{
					DataTable dataTable2 = dataTable.Clone();
					dataTable2.ImportRow(dataTable.Rows[focusedDataSourceRowIndex]);
					dataTable2.AcceptChanges();
					grd_Account.DataSource = dataTable2;
					grd_Account.RefreshDataSource();
					if (Function.ToString(gv_Doc.GetFocusedRowCellValue("BinActivat")) == "Y")
					{
						grd_BinLocation.DataSource = dataTable2;
						grd_BinLocation.RefreshDataSource();
						ShowHideBinLocationColumn();
						tabPage_BinLocations.PageVisible = true;
					}
					else
					{
						tabPage_BinLocations.PageVisible = false;
					}
					splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
				}
			}
			else
			{
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetUpdateTextButtonOK()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			await GetRecord("F");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			await GetRecord("L");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			await GetRecord("N", Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			await GetRecord("P", Function.ToString(gv_Doc.GetFocusedRowCellValue("WhsCode")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		bool flag = true;
		if (FormMode == Enums.FormViewModeType.Update)
		{
			flag = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
		}
		if (flag)
		{
			_isLoading = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("OWHS");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["Nettable"] = "Y";
			dataRow["DropShip"] = "N";
			dataRow["ManageSnB"] = "N";
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			if (tableStructure != null && tableStructure.Columns.Count > 0)
			{
				grd_Doc.DataSource = tableStructure;
				grd_Doc.RefreshDataSource();
				DataTable dataSource = tableStructure.Copy();
				grd_Account.DataSource = dataSource;
				grd_Account.RefreshDataSource();
				DataTable dataSource2 = tableStructure.Copy();
				grd_BinLocation.DataSource = dataSource2;
				grd_BinLocation.RefreshDataSource();
				ShowHideBinLocationColumn();
				splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			}
			FormMode = Enums.FormViewModeType.Add;
			gv_BinLocation.Columns["DftBinAbs"].OptionsColumn.AllowEdit = false;
			gv_Doc.Columns["WhsCode"].OptionsColumn.AllowEdit = true;
			_isLoading = false;
		}
	}

	private async Task GetRecord(string position, string whsCode = "")
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await _httpClient.Warehouse.GetRecord(position, whsCode);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				gv_BinLocation.Columns["DftBinAbs"].OptionsColumn.AllowEdit = true;
				gv_Doc.Columns["WhsCode"].OptionsColumn.AllowEdit = false;
				LoadDetails();
			}
			else
			{
				dataTable = new DataTable();
				grd_Doc.DataSource = dataTable;
				WinCommons.Warning(Enums.MessageCode.NoDataFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void gv_Account_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetUpdateTextButtonOK();
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_WareHouses));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcWarehouseCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWarehouseName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_Inactive = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcBuildingFloorRoom = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_DropShip = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcStreet = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Location = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcStreetNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_Nettable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcBlock = new Apzon.Controls.Grid.GridColumnEx();
		this.gcZipCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_BinActivat = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcCity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFederalTaxID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcObjTypeHistory = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDropShip = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNettables = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBPLid = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Country = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcAddressType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_State = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCounty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCountry = new Apzon.Controls.Grid.GridColumnEx();
		this.gcState = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGlblLocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBinActivat = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_ManageSnB = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPage_BinLocations = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_BinLocation = new Apzon.Controls.Grid.GridControlEx();
		this.gv_BinLocation = new Apzon.Controls.Grid.GridViewEx();
		this.gcBinSeptor = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecBinEnab = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bin_RecBinEnab = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcNoOfBinLocations = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_calc_NoOfBinLoc = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcDefaultBinLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_DftBinAbs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDftBinEnfd = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bin_DftBinEnfd = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcAutoIssMtd = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bin_AutoIssMtd = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcAutoRecvMd = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bin_AutoRecvMd = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRecvMaxQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecvMaxWT = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bin_RecItemsBy = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bin_RecvUpTo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabPage_Accounting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Account = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Account = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Account = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
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
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_WarehouseCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_WarehouseName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_WareHouseCode = new Apzon.Controls.Controls.LabelLine();
		this.lb_WarehouseName = new Apzon.Controls.Controls.LabelLine();
		this.bt_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_Branch = new Apzon.Controls.Controls.LabelLine();
		this.lookUE_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Inactive).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_DropShip).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Location).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Nettable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_BinActivat).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Country).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_State).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_ManageSnB).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabPage_BinLocations.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_BinLocation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BinLocation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_RecBinEnab).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_calc_NoOfBinLoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_DftBinAbs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_DftBinEnfd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_AutoIssMtd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_AutoRecvMd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_RecItemsBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_RecvUpTo).BeginInit();
		this.tabPage_Accounting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Account).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Account).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Account).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WarehouseCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WarehouseName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_Branch.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 34);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(790, 412);
		this.splitContainerControl1.SplitterPosition = 228;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_chk_Inactive, this.rep_Branch, this.rep_Country, this.rep_State, this.rep_Location, this.rep_chk_Nettable, this.rep_chk_BinActivat, this.rep_chk_DropShip, this.rep_chk_ManageSnB });
		this.grd_Doc.Size = new System.Drawing.Size(790, 228);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[23]
		{
			this.gcWarehouseCode, this.gcWarehouseName, this.gcLocation, this.gcBuildingFloorRoom, this.gcStreet, this.gcStreetNo, this.gcBlock, this.gcZipCode, this.gcCity, this.gcFederalTaxID,
			this.gcObjTypeHistory, this.gcLocked, this.gcDropShip, this.gcNettables, this.gcBPLid, this.gcAddressType, this.gcCounty, this.gcCountry, this.gcState, this.gcGlblLocNum,
			this.gcBinActivat, this.gridColumn25, this.gridColumn30
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oWarehouse";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OWHS";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_General_CustomRowCellEdit);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_General_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_General_ShowingEditor);
		this.gv_Doc.HiddenEditor += new System.EventHandler(this.gv_General_HiddenEditor);
		this.gv_Doc.ShownEditor += new System.EventHandler(this.gv_General_ShownEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gcWarehouseCode.Caption = "Warehouse Code";
		this.gcWarehouseCode.CurrencyDateField = null;
		this.gcWarehouseCode.CurrencyField = null;
		this.gcWarehouseCode.CurrencyFixed = null;
		this.gcWarehouseCode.CurrencyFixedRate = 0.0;
		this.gcWarehouseCode.CurrencyRateField = null;
		this.gcWarehouseCode.FieldName = "WhsCode";
		this.gcWarehouseCode.Name = "gcWarehouseCode";
		this.gcWarehouseCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcWarehouseCode.PrimaryKey = true;
		this.gcWarehouseCode.UnitField = null;
		this.gcWarehouseCode.Visible = true;
		this.gcWarehouseCode.VisibleIndex = 0;
		this.gcWarehouseCode.Width = 96;
		this.gcWarehouseName.Caption = "Warehouse Name";
		this.gcWarehouseName.CurrencyDateField = null;
		this.gcWarehouseName.CurrencyField = null;
		this.gcWarehouseName.CurrencyFixed = null;
		this.gcWarehouseName.CurrencyFixedRate = 0.0;
		this.gcWarehouseName.CurrencyRateField = null;
		this.gcWarehouseName.FieldName = "WhsName";
		this.gcWarehouseName.Name = "gcWarehouseName";
		this.gcWarehouseName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcWarehouseName.PrimaryKey = false;
		this.gcWarehouseName.UnitField = null;
		this.gcWarehouseName.Visible = true;
		this.gcWarehouseName.VisibleIndex = 1;
		this.gcWarehouseName.Width = 173;
		this.gcLocation.Caption = "Inactive";
		this.gcLocation.ColumnEdit = this.rep_chk_Inactive;
		this.gcLocation.CurrencyDateField = null;
		this.gcLocation.CurrencyField = null;
		this.gcLocation.CurrencyFixed = null;
		this.gcLocation.CurrencyFixedRate = 0.0;
		this.gcLocation.CurrencyRateField = null;
		this.gcLocation.FieldName = "Inactive";
		this.gcLocation.Name = "gcLocation";
		this.gcLocation.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLocation.PrimaryKey = false;
		this.gcLocation.UnitField = null;
		this.gcLocation.Visible = true;
		this.gcLocation.VisibleIndex = 2;
		this.rep_chk_Inactive.AutoHeight = false;
		this.rep_chk_Inactive.Name = "rep_chk_Inactive";
		this.rep_chk_Inactive.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_chk_Inactive.ValueChecked = "Y";
		this.rep_chk_Inactive.ValueGrayed = "N";
		this.rep_chk_Inactive.ValueUnchecked = "N";
		this.gcBuildingFloorRoom.Caption = "Drop-Ship";
		this.gcBuildingFloorRoom.ColumnEdit = this.rep_chk_DropShip;
		this.gcBuildingFloorRoom.CurrencyDateField = null;
		this.gcBuildingFloorRoom.CurrencyField = null;
		this.gcBuildingFloorRoom.CurrencyFixed = null;
		this.gcBuildingFloorRoom.CurrencyFixedRate = 0.0;
		this.gcBuildingFloorRoom.CurrencyRateField = null;
		this.gcBuildingFloorRoom.FieldName = "DropShip";
		this.gcBuildingFloorRoom.Name = "gcBuildingFloorRoom";
		this.gcBuildingFloorRoom.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBuildingFloorRoom.PrimaryKey = false;
		this.gcBuildingFloorRoom.UnitField = null;
		this.gcBuildingFloorRoom.Visible = true;
		this.gcBuildingFloorRoom.VisibleIndex = 20;
		this.rep_chk_DropShip.AutoHeight = false;
		this.rep_chk_DropShip.DisplayValueChecked = "Y";
		this.rep_chk_DropShip.DisplayValueGrayed = "N";
		this.rep_chk_DropShip.DisplayValueUnchecked = "N";
		this.rep_chk_DropShip.Name = "rep_chk_DropShip";
		this.rep_chk_DropShip.ValueChecked = "Y";
		this.rep_chk_DropShip.ValueGrayed = "N";
		this.rep_chk_DropShip.ValueUnchecked = "N";
		this.gcStreet.Caption = "Location";
		this.gcStreet.ColumnEdit = this.rep_Location;
		this.gcStreet.CurrencyDateField = null;
		this.gcStreet.CurrencyField = null;
		this.gcStreet.CurrencyFixed = null;
		this.gcStreet.CurrencyFixedRate = 0.0;
		this.gcStreet.CurrencyRateField = null;
		this.gcStreet.FieldName = "Location";
		this.gcStreet.Name = "gcStreet";
		this.gcStreet.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcStreet.PrimaryKey = false;
		this.gcStreet.UnitField = null;
		this.gcStreet.Visible = true;
		this.gcStreet.VisibleIndex = 3;
		this.rep_Location.AutoHeight = false;
		this.rep_Location.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Location.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Location", "Location")
		});
		this.rep_Location.DisplayMember = "Location";
		this.rep_Location.Name = "rep_Location";
		this.rep_Location.NullText = "";
		this.rep_Location.ValueMember = "Code";
		this.gcStreetNo.Caption = "Nettable";
		this.gcStreetNo.ColumnEdit = this.rep_chk_Nettable;
		this.gcStreetNo.CurrencyDateField = null;
		this.gcStreetNo.CurrencyField = null;
		this.gcStreetNo.CurrencyFixed = null;
		this.gcStreetNo.CurrencyFixedRate = 0.0;
		this.gcStreetNo.CurrencyRateField = null;
		this.gcStreetNo.FieldName = "Nettable";
		this.gcStreetNo.Name = "gcStreetNo";
		this.gcStreetNo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcStreetNo.PrimaryKey = false;
		this.gcStreetNo.UnitField = null;
		this.gcStreetNo.Visible = true;
		this.gcStreetNo.VisibleIndex = 13;
		this.rep_chk_Nettable.AutoHeight = false;
		this.rep_chk_Nettable.Name = "rep_chk_Nettable";
		this.rep_chk_Nettable.ValueChecked = "Y";
		this.rep_chk_Nettable.ValueGrayed = "N";
		this.rep_chk_Nettable.ValueUnchecked = "N";
		this.gcBlock.Caption = "Street/PO Box";
		this.gcBlock.CurrencyDateField = null;
		this.gcBlock.CurrencyField = null;
		this.gcBlock.CurrencyFixed = null;
		this.gcBlock.CurrencyFixedRate = 0.0;
		this.gcBlock.CurrencyRateField = null;
		this.gcBlock.FieldName = "Street";
		this.gcBlock.Name = "gcBlock";
		this.gcBlock.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBlock.PrimaryKey = false;
		this.gcBlock.UnitField = null;
		this.gcBlock.Visible = true;
		this.gcBlock.VisibleIndex = 4;
		this.gcBlock.Width = 79;
		this.gcZipCode.Caption = "Enab&le Bin Locations";
		this.gcZipCode.ColumnEdit = this.rep_chk_BinActivat;
		this.gcZipCode.CurrencyDateField = null;
		this.gcZipCode.CurrencyField = null;
		this.gcZipCode.CurrencyFixed = null;
		this.gcZipCode.CurrencyFixedRate = 0.0;
		this.gcZipCode.CurrencyRateField = null;
		this.gcZipCode.FieldName = "BinActivat";
		this.gcZipCode.Name = "gcZipCode";
		this.gcZipCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcZipCode.PrimaryKey = false;
		this.gcZipCode.UnitField = null;
		this.gcZipCode.Visible = true;
		this.gcZipCode.VisibleIndex = 5;
		this.gcZipCode.Width = 114;
		this.rep_chk_BinActivat.AutoHeight = false;
		this.rep_chk_BinActivat.Name = "rep_chk_BinActivat";
		this.rep_chk_BinActivat.ValueChecked = "Y";
		this.rep_chk_BinActivat.ValueGrayed = "N";
		this.rep_chk_BinActivat.ValueUnchecked = "N";
		this.rep_chk_BinActivat.EditValueChanged += new System.EventHandler(this.rep_chk_BinActivat_EditValueChanged);
		this.gcCity.Caption = "Street No.";
		this.gcCity.CurrencyDateField = null;
		this.gcCity.CurrencyField = null;
		this.gcCity.CurrencyFixed = null;
		this.gcCity.CurrencyFixedRate = 0.0;
		this.gcCity.CurrencyRateField = null;
		this.gcCity.FieldName = "StreetNo";
		this.gcCity.Name = "gcCity";
		this.gcCity.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCity.PrimaryKey = false;
		this.gcCity.UnitField = null;
		this.gcCity.Visible = true;
		this.gcCity.VisibleIndex = 6;
		this.gcFederalTaxID.Caption = "Block";
		this.gcFederalTaxID.CurrencyDateField = null;
		this.gcFederalTaxID.CurrencyField = null;
		this.gcFederalTaxID.CurrencyFixed = null;
		this.gcFederalTaxID.CurrencyFixedRate = 0.0;
		this.gcFederalTaxID.CurrencyRateField = null;
		this.gcFederalTaxID.FieldName = "Block";
		this.gcFederalTaxID.Name = "gcFederalTaxID";
		this.gcFederalTaxID.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFederalTaxID.PrimaryKey = false;
		this.gcFederalTaxID.UnitField = null;
		this.gcFederalTaxID.Visible = true;
		this.gcFederalTaxID.VisibleIndex = 8;
		this.gcObjTypeHistory.Caption = "Building/Floor/Room";
		this.gcObjTypeHistory.CurrencyDateField = null;
		this.gcObjTypeHistory.CurrencyField = null;
		this.gcObjTypeHistory.CurrencyFixed = null;
		this.gcObjTypeHistory.CurrencyFixedRate = 0.0;
		this.gcObjTypeHistory.CurrencyRateField = null;
		this.gcObjTypeHistory.FieldName = "Building";
		this.gcObjTypeHistory.Name = "gcObjTypeHistory";
		this.gcObjTypeHistory.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcObjTypeHistory.PrimaryKey = false;
		this.gcObjTypeHistory.UnitField = null;
		this.gcObjTypeHistory.Visible = true;
		this.gcObjTypeHistory.VisibleIndex = 18;
		this.gcObjTypeHistory.Width = 105;
		this.gcLocked.Caption = "Zip Code";
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "ZipCode";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcLocked.Visible = true;
		this.gcLocked.VisibleIndex = 19;
		this.gcDropShip.Caption = "City";
		this.gcDropShip.CurrencyDateField = null;
		this.gcDropShip.CurrencyField = null;
		this.gcDropShip.CurrencyFixed = null;
		this.gcDropShip.CurrencyFixedRate = 0.0;
		this.gcDropShip.CurrencyRateField = null;
		this.gcDropShip.FieldName = "City";
		this.gcDropShip.Name = "gcDropShip";
		this.gcDropShip.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDropShip.PrimaryKey = false;
		this.gcDropShip.UnitField = null;
		this.gcDropShip.Visible = true;
		this.gcDropShip.VisibleIndex = 9;
		this.gcNettables.Caption = "County";
		this.gcNettables.CurrencyDateField = null;
		this.gcNettables.CurrencyField = null;
		this.gcNettables.CurrencyFixed = null;
		this.gcNettables.CurrencyFixedRate = 0.0;
		this.gcNettables.CurrencyRateField = null;
		this.gcNettables.FieldName = "County";
		this.gcNettables.Name = "gcNettables";
		this.gcNettables.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcNettables.PrimaryKey = false;
		this.gcNettables.UnitField = null;
		this.gcNettables.Visible = true;
		this.gcNettables.VisibleIndex = 10;
		this.gcBPLid.Caption = "Country";
		this.gcBPLid.ColumnEdit = this.rep_Country;
		this.gcBPLid.CurrencyDateField = null;
		this.gcBPLid.CurrencyField = null;
		this.gcBPLid.CurrencyFixed = null;
		this.gcBPLid.CurrencyFixedRate = 0.0;
		this.gcBPLid.CurrencyRateField = null;
		this.gcBPLid.FieldName = "Country";
		this.gcBPLid.Name = "gcBPLid";
		this.gcBPLid.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBPLid.PrimaryKey = false;
		this.gcBPLid.UnitField = null;
		this.gcBPLid.Visible = true;
		this.gcBPLid.VisibleIndex = 11;
		this.rep_Country.AutoHeight = false;
		this.rep_Country.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Country.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Country.DisplayMember = "Name";
		this.rep_Country.Name = "rep_Country";
		this.rep_Country.NullText = "";
		this.rep_Country.ValueMember = "Code";
		this.gcAddressType.Caption = "State";
		this.gcAddressType.ColumnEdit = this.rep_State;
		this.gcAddressType.CurrencyDateField = null;
		this.gcAddressType.CurrencyField = null;
		this.gcAddressType.CurrencyFixed = null;
		this.gcAddressType.CurrencyFixedRate = 0.0;
		this.gcAddressType.CurrencyRateField = null;
		this.gcAddressType.FieldName = "State";
		this.gcAddressType.Name = "gcAddressType";
		this.gcAddressType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcAddressType.PrimaryKey = false;
		this.gcAddressType.UnitField = null;
		this.gcAddressType.Visible = true;
		this.gcAddressType.VisibleIndex = 12;
		this.rep_State.AutoHeight = false;
		this.rep_State.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_State.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Country", "Country", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_State.DisplayMember = "Name";
		this.rep_State.Name = "rep_State";
		this.rep_State.NullText = "";
		this.rep_State.ValueMember = "Code";
		this.rep_State.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rILUE_State_ButtonClick);
		this.gcCounty.Caption = "Federal Tax ID";
		this.gcCounty.CurrencyDateField = null;
		this.gcCounty.CurrencyField = null;
		this.gcCounty.CurrencyFixed = null;
		this.gcCounty.CurrencyFixedRate = 0.0;
		this.gcCounty.CurrencyRateField = null;
		this.gcCounty.FieldName = "FedTaxID";
		this.gcCounty.Name = "gcCounty";
		this.gcCounty.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCounty.PrimaryKey = false;
		this.gcCounty.UnitField = null;
		this.gcCounty.Visible = true;
		this.gcCounty.VisibleIndex = 7;
		this.gcCounty.Width = 81;
		this.gcCountry.Caption = "GLN";
		this.gcCountry.CurrencyDateField = null;
		this.gcCountry.CurrencyField = null;
		this.gcCountry.CurrencyFixed = null;
		this.gcCountry.CurrencyFixedRate = 0.0;
		this.gcCountry.CurrencyRateField = null;
		this.gcCountry.FieldName = "GlblLocNum";
		this.gcCountry.Name = "gcCountry";
		this.gcCountry.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcCountry.PrimaryKey = false;
		this.gcCountry.UnitField = null;
		this.gcCountry.Visible = true;
		this.gcCountry.VisibleIndex = 14;
		this.gcState.Caption = "Tax Office";
		this.gcState.CurrencyDateField = null;
		this.gcState.CurrencyField = null;
		this.gcState.CurrencyFixed = null;
		this.gcState.CurrencyFixedRate = 0.0;
		this.gcState.CurrencyRateField = null;
		this.gcState.FieldName = "TaxOffice";
		this.gcState.Name = "gcState";
		this.gcState.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcState.PrimaryKey = false;
		this.gcState.UnitField = null;
		this.gcState.Visible = true;
		this.gcState.VisibleIndex = 15;
		this.gcGlblLocNum.Caption = "Address Name 2";
		this.gcGlblLocNum.CurrencyDateField = null;
		this.gcGlblLocNum.CurrencyField = null;
		this.gcGlblLocNum.CurrencyFixed = null;
		this.gcGlblLocNum.CurrencyFixedRate = 0.0;
		this.gcGlblLocNum.CurrencyRateField = null;
		this.gcGlblLocNum.FieldName = "Address2";
		this.gcGlblLocNum.Name = "gcGlblLocNum";
		this.gcGlblLocNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcGlblLocNum.PrimaryKey = false;
		this.gcGlblLocNum.UnitField = null;
		this.gcGlblLocNum.Visible = true;
		this.gcGlblLocNum.VisibleIndex = 16;
		this.gcGlblLocNum.Width = 88;
		this.gcBinActivat.Caption = "Address Name 3";
		this.gcBinActivat.ColumnEdit = this.rep_Branch;
		this.gcBinActivat.CurrencyDateField = null;
		this.gcBinActivat.CurrencyField = null;
		this.gcBinActivat.CurrencyFixed = null;
		this.gcBinActivat.CurrencyFixedRate = 0.0;
		this.gcBinActivat.CurrencyRateField = null;
		this.gcBinActivat.FieldName = "Address";
		this.gcBinActivat.Name = "gcBinActivat";
		this.gcBinActivat.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcBinActivat.PrimaryKey = false;
		this.gcBinActivat.UnitField = null;
		this.gcBinActivat.Visible = true;
		this.gcBinActivat.VisibleIndex = 17;
		this.gcBinActivat.Width = 88;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLid", "BPLid", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "BPLName")
		});
		this.rep_Branch.DisplayMember = "BPLName";
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.rep_Branch.ValueMember = "BPLId";
		this.gridColumn25.Caption = "Branch";
		this.gridColumn25.ColumnEdit = this.rep_Branch;
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "BPLid";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn25.Visible = true;
		this.gridColumn25.VisibleIndex = 21;
		this.gridColumn30.Caption = "Manage &Serial Numbers and Batches";
		this.gridColumn30.ColumnEdit = this.rep_chk_ManageSnB;
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.FieldName = "ManageSnB";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		this.gridColumn30.Visible = true;
		this.gridColumn30.VisibleIndex = 22;
		this.rep_chk_ManageSnB.AutoHeight = false;
		this.rep_chk_ManageSnB.DisplayValueChecked = "Y";
		this.rep_chk_ManageSnB.DisplayValueGrayed = "N";
		this.rep_chk_ManageSnB.DisplayValueUnchecked = "N";
		this.rep_chk_ManageSnB.Name = "rep_chk_ManageSnB";
		this.rep_chk_ManageSnB.ValueChecked = "Y";
		this.rep_chk_ManageSnB.ValueGrayed = "N";
		this.rep_chk_ManageSnB.ValueUnchecked = "N";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabPage_BinLocations;
		this.xtraTabControl1.Size = new System.Drawing.Size(790, 174);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabPage_Accounting, this.tabPage_BinLocations });
		this.tabPage_BinLocations.Controls.Add(this.grd_BinLocation);
		this.tabPage_BinLocations.EditControl = null;
		this.tabPage_BinLocations.EditValue = "Bin Locations";
		this.tabPage_BinLocations.FieldName = null;
		this.tabPage_BinLocations.IsSearchControl = false;
		this.tabPage_BinLocations.Name = "tabPage_BinLocations";
		this.tabPage_BinLocations.PageVisible = false;
		this.tabPage_BinLocations.ReadOnly = false;
		this.tabPage_BinLocations.SettingInVisible = false;
		this.tabPage_BinLocations.SettingReadonly = false;
		this.tabPage_BinLocations.Size = new System.Drawing.Size(788, 149);
		this.tabPage_BinLocations.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_BinLocations.Text = "Bin Locations";
		this.grd_BinLocation.AllowPasteExcel = false;
		this.grd_BinLocation.ClipboardData = null;
		this.grd_BinLocation.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_BinLocation.Location = new System.Drawing.Point(0, 0);
		this.grd_BinLocation.MainView = this.gv_BinLocation;
		this.grd_BinLocation.Name = "grd_BinLocation";
		this.grd_BinLocation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_calc_NoOfBinLoc, this.rep_bin_DftBinEnfd, this.rep_bin_AutoIssMtd, this.rep_bin_AutoRecvMd, this.rep_bin_RecBinEnab, this.rep_bin_RecItemsBy, this.rep_bin_RecvUpTo, this.rep_bte_DftBinAbs });
		this.grd_BinLocation.Size = new System.Drawing.Size(788, 149);
		this.grd_BinLocation.TabIndex = 0;
		this.grd_BinLocation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_BinLocation });
		this.gv_BinLocation.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_BinLocation.ColumnKey = null;
		this.gv_BinLocation.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gcBinSeptor, this.gcRecBinEnab, this.gcNoOfBinLocations, this.gcDefaultBinLocation, this.gcDftBinEnfd, this.gcAutoIssMtd, this.gcAutoRecvMd, this.gcRecvMaxQty, this.gcRecvMaxWT, this.gridColumn26,
			this.gridColumn27, this.gridColumn28, this.gridColumn29
		});
		this.gv_BinLocation.DisableFormSetting = false;
		this.gv_BinLocation.DisplayException = false;
		this.gv_BinLocation.EditColumn = null;
		this.gv_BinLocation.FieldCurrency = null;
		this.gv_BinLocation.FieldRate = null;
		this.gv_BinLocation.FormId = "oWarehouse";
		this.gv_BinLocation.GridControl = this.grd_BinLocation;
		this.gv_BinLocation.HasFormSetting = false;
		this.gv_BinLocation.IgnoreFilter = false;
		this.gv_BinLocation.IndexEditRow = -1;
		this.gv_BinLocation.IndicatorWidth = 35;
		this.gv_BinLocation.IsApzTable = false;
		this.gv_BinLocation.IsInsertNewRow = false;
		this.gv_BinLocation.IsLoadingData = false;
		this.gv_BinLocation.IsMappingContextAction = false;
		this.gv_BinLocation.ItemId = "OBIN";
		this.gv_BinLocation.LstColumnAllowBatch = null;
		this.gv_BinLocation.LstColumnNotAllowBatch = null;
		this.gv_BinLocation.LstColumnNotEditable = null;
		this.gv_BinLocation.LstColumNotSetting = null;
		this.gv_BinLocation.LstOldUDF = null;
		this.gv_BinLocation.Name = "gv_BinLocation";
		this.gv_BinLocation.OptionsView.ColumnAutoWidth = false;
		this.gv_BinLocation.OptionsView.ShowGroupPanel = false;
		this.gv_BinLocation.UseBatchMod = false;
		this.gv_BinLocation.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_BinLocation_CellValueChanged);
		this.gcBinSeptor.Caption = "Bin Loc. Code Separator";
		this.gcBinSeptor.CurrencyDateField = null;
		this.gcBinSeptor.CurrencyField = null;
		this.gcBinSeptor.CurrencyFixed = null;
		this.gcBinSeptor.CurrencyFixedRate = 0.0;
		this.gcBinSeptor.CurrencyRateField = null;
		this.gcBinSeptor.FieldName = "BinSeptor";
		this.gcBinSeptor.Name = "gcBinSeptor";
		this.gcBinSeptor.PrimaryKey = false;
		this.gcBinSeptor.UnitField = null;
		this.gcBinSeptor.Visible = true;
		this.gcBinSeptor.VisibleIndex = 0;
		this.gcBinSeptor.Width = 126;
		this.gcRecBinEnab.Caption = "Enable Receiving Bin Locations";
		this.gcRecBinEnab.ColumnEdit = this.rep_bin_RecBinEnab;
		this.gcRecBinEnab.CurrencyDateField = null;
		this.gcRecBinEnab.CurrencyField = null;
		this.gcRecBinEnab.CurrencyFixed = null;
		this.gcRecBinEnab.CurrencyFixedRate = 0.0;
		this.gcRecBinEnab.CurrencyRateField = null;
		this.gcRecBinEnab.FieldName = "RecBinEnab";
		this.gcRecBinEnab.Name = "gcRecBinEnab";
		this.gcRecBinEnab.PrimaryKey = false;
		this.gcRecBinEnab.UnitField = null;
		this.gcRecBinEnab.Visible = true;
		this.gcRecBinEnab.VisibleIndex = 1;
		this.gcRecBinEnab.Width = 158;
		this.rep_bin_RecBinEnab.AutoHeight = false;
		this.rep_bin_RecBinEnab.DisplayValueChecked = "Y";
		this.rep_bin_RecBinEnab.DisplayValueGrayed = "N";
		this.rep_bin_RecBinEnab.DisplayValueUnchecked = "N";
		this.rep_bin_RecBinEnab.Name = "rep_bin_RecBinEnab";
		this.rep_bin_RecBinEnab.ValueChecked = "Y";
		this.rep_bin_RecBinEnab.ValueGrayed = "N";
		this.rep_bin_RecBinEnab.ValueUnchecked = "N";
		this.rep_bin_RecBinEnab.EditValueChanged += new System.EventHandler(this.rep_bin_RecBinEnab_EditValueChanged);
		this.gcNoOfBinLocations.Caption = "No. Of Bin Locations";
		this.gcNoOfBinLocations.ColumnEdit = this.rep_calc_NoOfBinLoc;
		this.gcNoOfBinLocations.CurrencyDateField = null;
		this.gcNoOfBinLocations.CurrencyField = null;
		this.gcNoOfBinLocations.CurrencyFixed = null;
		this.gcNoOfBinLocations.CurrencyFixedRate = 0.0;
		this.gcNoOfBinLocations.CurrencyRateField = null;
		this.gcNoOfBinLocations.DisplayFormat.FormatString = "n0";
		this.gcNoOfBinLocations.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcNoOfBinLocations.FieldName = "NoOfBinLocs";
		this.gcNoOfBinLocations.Name = "gcNoOfBinLocations";
		this.gcNoOfBinLocations.OptionsColumn.AllowEdit = false;
		this.gcNoOfBinLocations.OptionsColumn.ReadOnly = true;
		this.gcNoOfBinLocations.PrimaryKey = false;
		this.gcNoOfBinLocations.UnitField = null;
		this.gcNoOfBinLocations.Visible = true;
		this.gcNoOfBinLocations.VisibleIndex = 2;
		this.gcNoOfBinLocations.Width = 107;
		this.rep_calc_NoOfBinLoc.AutoHeight = false;
		this.rep_calc_NoOfBinLoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_calc_NoOfBinLoc.Name = "rep_calc_NoOfBinLoc";
		this.gcDefaultBinLocation.Caption = "Default Bin Location";
		this.gcDefaultBinLocation.ColumnEdit = this.rep_bte_DftBinAbs;
		this.gcDefaultBinLocation.CurrencyDateField = null;
		this.gcDefaultBinLocation.CurrencyField = null;
		this.gcDefaultBinLocation.CurrencyFixed = null;
		this.gcDefaultBinLocation.CurrencyFixedRate = 0.0;
		this.gcDefaultBinLocation.CurrencyRateField = null;
		this.gcDefaultBinLocation.FieldName = "DftBinAbsCode";
		this.gcDefaultBinLocation.Name = "gcDefaultBinLocation";
		this.gcDefaultBinLocation.OptionsColumn.ReadOnly = true;
		this.gcDefaultBinLocation.PrimaryKey = false;
		this.gcDefaultBinLocation.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gcDefaultBinLocation.UnitField = null;
		this.gcDefaultBinLocation.Visible = true;
		this.gcDefaultBinLocation.VisibleIndex = 3;
		this.gcDefaultBinLocation.Width = 107;
		this.rep_bte_DftBinAbs.AutoHeight = false;
		this.rep_bte_DftBinAbs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_DftBinAbs.Name = "rep_bte_DftBinAbs";
		this.rep_bte_DftBinAbs.ReadOnly = true;
		this.rep_bte_DftBinAbs.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_DftBinAbs_ButtonPressed);
		this.gcDftBinEnfd.Caption = "Enforce Default Bin Loc.";
		this.gcDftBinEnfd.ColumnEdit = this.rep_bin_DftBinEnfd;
		this.gcDftBinEnfd.CurrencyDateField = null;
		this.gcDftBinEnfd.CurrencyField = null;
		this.gcDftBinEnfd.CurrencyFixed = null;
		this.gcDftBinEnfd.CurrencyFixedRate = 0.0;
		this.gcDftBinEnfd.CurrencyRateField = null;
		this.gcDftBinEnfd.FieldName = "DftBinEnfd";
		this.gcDftBinEnfd.Name = "gcDftBinEnfd";
		this.gcDftBinEnfd.PrimaryKey = false;
		this.gcDftBinEnfd.UnitField = null;
		this.gcDftBinEnfd.Visible = true;
		this.gcDftBinEnfd.VisibleIndex = 4;
		this.gcDftBinEnfd.Width = 125;
		this.rep_bin_DftBinEnfd.AutoHeight = false;
		this.rep_bin_DftBinEnfd.DisplayValueChecked = "Y";
		this.rep_bin_DftBinEnfd.DisplayValueGrayed = "N";
		this.rep_bin_DftBinEnfd.DisplayValueUnchecked = "N";
		this.rep_bin_DftBinEnfd.Name = "rep_bin_DftBinEnfd";
		this.rep_bin_DftBinEnfd.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rep_bin_DftBinEnfd.ValueChecked = "Y";
		this.rep_bin_DftBinEnfd.ValueGrayed = "N";
		this.rep_bin_DftBinEnfd.ValueUnchecked = "N";
		this.gcAutoIssMtd.Caption = "Auto Alloc. on Issue";
		this.gcAutoIssMtd.ColumnEdit = this.rep_bin_AutoIssMtd;
		this.gcAutoIssMtd.CurrencyDateField = null;
		this.gcAutoIssMtd.CurrencyField = null;
		this.gcAutoIssMtd.CurrencyFixed = null;
		this.gcAutoIssMtd.CurrencyFixedRate = 0.0;
		this.gcAutoIssMtd.CurrencyRateField = null;
		this.gcAutoIssMtd.FieldName = "AutoIssMtd";
		this.gcAutoIssMtd.Name = "gcAutoIssMtd";
		this.gcAutoIssMtd.PrimaryKey = false;
		this.gcAutoIssMtd.UnitField = null;
		this.gcAutoIssMtd.Visible = true;
		this.gcAutoIssMtd.VisibleIndex = 5;
		this.gcAutoIssMtd.Width = 108;
		this.rep_bin_AutoIssMtd.AutoHeight = false;
		this.rep_bin_AutoIssMtd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_bin_AutoIssMtd.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_bin_AutoIssMtd.DisplayMember = "Name";
		this.rep_bin_AutoIssMtd.Name = "rep_bin_AutoIssMtd";
		this.rep_bin_AutoIssMtd.NullText = "";
		this.rep_bin_AutoIssMtd.ValueMember = "Code";
		this.gcAutoRecvMd.Caption = "Auto Alloc. on Receipt";
		this.gcAutoRecvMd.ColumnEdit = this.rep_bin_AutoRecvMd;
		this.gcAutoRecvMd.CurrencyDateField = null;
		this.gcAutoRecvMd.CurrencyField = null;
		this.gcAutoRecvMd.CurrencyFixed = null;
		this.gcAutoRecvMd.CurrencyFixedRate = 0.0;
		this.gcAutoRecvMd.CurrencyRateField = null;
		this.gcAutoRecvMd.FieldName = "AutoRecvMd";
		this.gcAutoRecvMd.Name = "gcAutoRecvMd";
		this.gcAutoRecvMd.PrimaryKey = false;
		this.gcAutoRecvMd.UnitField = null;
		this.gcAutoRecvMd.Visible = true;
		this.gcAutoRecvMd.VisibleIndex = 6;
		this.gcAutoRecvMd.Width = 119;
		this.rep_bin_AutoRecvMd.AutoHeight = false;
		this.rep_bin_AutoRecvMd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_bin_AutoRecvMd.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_bin_AutoRecvMd.DisplayMember = "Name";
		this.rep_bin_AutoRecvMd.Name = "rep_bin_AutoRecvMd";
		this.rep_bin_AutoRecvMd.NullText = "";
		this.rep_bin_AutoRecvMd.ValueMember = "Code";
		this.gcRecvMaxQty.Caption = "Receive Up to Max. Qty";
		this.gcRecvMaxQty.ColumnEdit = this.rep_bin_DftBinEnfd;
		this.gcRecvMaxQty.CurrencyDateField = null;
		this.gcRecvMaxQty.CurrencyField = null;
		this.gcRecvMaxQty.CurrencyFixed = null;
		this.gcRecvMaxQty.CurrencyFixedRate = 0.0;
		this.gcRecvMaxQty.CurrencyRateField = null;
		this.gcRecvMaxQty.FieldName = "RecvMaxQty";
		this.gcRecvMaxQty.Name = "gcRecvMaxQty";
		this.gcRecvMaxQty.PrimaryKey = false;
		this.gcRecvMaxQty.UnitField = null;
		this.gcRecvMaxQty.Visible = true;
		this.gcRecvMaxQty.VisibleIndex = 7;
		this.gcRecvMaxQty.Width = 125;
		this.gcRecvMaxWT.Caption = "Receive Up to Max. Weight";
		this.gcRecvMaxWT.ColumnEdit = this.rep_bin_DftBinEnfd;
		this.gcRecvMaxWT.CurrencyDateField = null;
		this.gcRecvMaxWT.CurrencyField = null;
		this.gcRecvMaxWT.CurrencyFixed = null;
		this.gcRecvMaxWT.CurrencyFixedRate = 0.0;
		this.gcRecvMaxWT.CurrencyRateField = null;
		this.gcRecvMaxWT.FieldName = "RecvMaxWT";
		this.gcRecvMaxWT.Name = "gcRecvMaxWT";
		this.gcRecvMaxWT.PrimaryKey = false;
		this.gcRecvMaxWT.UnitField = null;
		this.gcRecvMaxWT.Visible = true;
		this.gcRecvMaxWT.VisibleIndex = 8;
		this.gcRecvMaxWT.Width = 141;
		this.gridColumn26.Caption = "Receive Items By";
		this.gridColumn26.ColumnEdit = this.rep_bin_RecItemsBy;
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "RecItemsBy";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn26.Visible = true;
		this.gridColumn26.VisibleIndex = 9;
		this.gridColumn26.Width = 93;
		this.rep_bin_RecItemsBy.AutoHeight = false;
		this.rep_bin_RecItemsBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_bin_RecItemsBy.Name = "rep_bin_RecItemsBy";
		this.gridColumn27.Caption = "Re&strict Receipts to Empty Bin Locations";
		this.gridColumn27.ColumnEdit = this.rep_bin_DftBinEnfd;
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "RecvEmpBin";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn27.Visible = true;
		this.gridColumn27.VisibleIndex = 10;
		this.gridColumn27.Width = 209;
		this.gridColumn28.Caption = "Receive up to";
		this.gridColumn28.ColumnEdit = this.rep_bin_RecvUpTo;
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "RecvUpTo";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn28.Visible = true;
		this.gridColumn28.VisibleIndex = 11;
		this.gridColumn28.Width = 76;
		this.rep_bin_RecvUpTo.AutoHeight = false;
		this.rep_bin_RecvUpTo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_bin_RecvUpTo.Name = "rep_bin_RecvUpTo";
		this.gridColumn29.Caption = "Default Bin Location";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "DftBinAbs";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.tabPage_Accounting.Controls.Add(this.grd_Account);
		this.tabPage_Accounting.EditControl = null;
		this.tabPage_Accounting.EditValue = "Accounting";
		this.tabPage_Accounting.FieldName = null;
		this.tabPage_Accounting.IsSearchControl = false;
		this.tabPage_Accounting.Name = "tabPage_Accounting";
		this.tabPage_Accounting.ReadOnly = false;
		this.tabPage_Accounting.SettingInVisible = false;
		this.tabPage_Accounting.SettingReadonly = false;
		this.tabPage_Accounting.Size = new System.Drawing.Size(788, 149);
		this.tabPage_Accounting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPage_Accounting.Text = "Accounting";
		this.grd_Account.AllowPasteExcel = false;
		this.grd_Account.ClipboardData = null;
		this.grd_Account.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Account.Location = new System.Drawing.Point(0, 0);
		this.grd_Account.MainView = this.gv_Account;
		this.grd_Account.Name = "grd_Account";
		this.grd_Account.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_bte_Account });
		this.grd_Account.Size = new System.Drawing.Size(788, 149);
		this.grd_Account.TabIndex = 1;
		this.grd_Account.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Account });
		this.gv_Account.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Account.ColumnKey = null;
		this.gv_Account.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[24]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20,
			this.gridColumn21, this.gridColumn22, this.gridColumn23, this.gridColumn24
		});
		this.gv_Account.DisableFormSetting = false;
		this.gv_Account.DisplayException = false;
		this.gv_Account.EditColumn = null;
		this.gv_Account.FieldCurrency = null;
		this.gv_Account.FieldRate = null;
		this.gv_Account.FormId = "oWarehouse";
		this.gv_Account.GridControl = this.grd_Account;
		this.gv_Account.HasFormSetting = false;
		this.gv_Account.IgnoreFilter = false;
		this.gv_Account.IndexEditRow = -1;
		this.gv_Account.IndicatorWidth = 25;
		this.gv_Account.IsApzTable = false;
		this.gv_Account.IsInsertNewRow = false;
		this.gv_Account.IsLoadingData = false;
		this.gv_Account.IsMappingContextAction = false;
		this.gv_Account.ItemId = "OACCT";
		this.gv_Account.LstColumnAllowBatch = null;
		this.gv_Account.LstColumnNotAllowBatch = null;
		this.gv_Account.LstColumnNotEditable = null;
		this.gv_Account.LstColumNotSetting = null;
		this.gv_Account.LstOldUDF = null;
		this.gv_Account.Name = "gv_Account";
		this.gv_Account.OptionsView.ColumnAutoWidth = false;
		this.gv_Account.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Account.OptionsView.ShowGroupPanel = false;
		this.gv_Account.UseBatchMod = false;
		this.gv_Account.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Account_CellValueChanged);
		this.gridColumn1.Caption = "Expense Account";
		this.gridColumn1.ColumnEdit = this.rep_bte_Account;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ExpensesAc";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 93;
		this.rep_bte_Account.AutoHeight = false;
		this.rep_bte_Account.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_bte_Account.Name = "rep_bte_Account";
		this.rep_bte_Account.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_Account_ButtonPressed);
		this.gridColumn2.Caption = "Revenue Account";
		this.gridColumn2.ColumnEdit = this.rep_bte_Account;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "RevenuesAc";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 95;
		this.gridColumn3.Caption = "Inventory Account";
		this.gridColumn3.ColumnEdit = this.rep_bte_Account;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "BalInvntAc";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 100;
		this.gridColumn4.Caption = "Cost of Goods Sold Account";
		this.gridColumn4.ColumnEdit = this.rep_bte_Account;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "SaleCostAc";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 143;
		this.gridColumn5.Caption = "Allocation Account";
		this.gridColumn5.ColumnEdit = this.rep_bte_Account;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "TransferAc";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn5.Width = 98;
		this.gridColumn6.Caption = "Variance Account";
		this.gridColumn6.ColumnEdit = this.rep_bte_Account;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "VarianceAc";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn6.Width = 93;
		this.gridColumn7.Caption = "Price Difference Account";
		this.gridColumn7.ColumnEdit = this.rep_bte_Account;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "PriceDifAc";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn7.Width = 128;
		this.gridColumn8.Caption = "Negative Inventory Adjustment Acct";
		this.gridColumn8.ColumnEdit = this.rep_bte_Account;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "NegStckAct";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.gridColumn8.Width = 186;
		this.gridColumn9.Caption = "Inventory Offset - Decrease Account";
		this.gridColumn9.ColumnEdit = this.rep_bte_Account;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "DecreasAc";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 8;
		this.gridColumn9.Width = 189;
		this.gridColumn10.Caption = "Inventory Offset - Increase Account";
		this.gridColumn10.ColumnEdit = this.rep_bte_Account;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "IncreasAc";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 9;
		this.gridColumn10.Width = 186;
		this.gridColumn11.Caption = "Sales Returns Account";
		this.gridColumn11.ColumnEdit = this.rep_bte_Account;
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "ReturnAc";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 10;
		this.gridColumn11.Width = 118;
		this.gridColumn12.Caption = "Exchange Rate Differences Account";
		this.gridColumn12.ColumnEdit = this.rep_bte_Account;
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "ExchangeAc";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 11;
		this.gridColumn12.Width = 183;
		this.gridColumn13.Caption = "Goods Clearing Account";
		this.gridColumn13.ColumnEdit = this.rep_bte_Account;
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "BalanceAcc";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 12;
		this.gridColumn13.Width = 124;
		this.gridColumn14.Caption = "G/L Decrease Account";
		this.gridColumn14.ColumnEdit = this.rep_bte_Account;
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "DecresGlAc";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 13;
		this.gridColumn14.Width = 116;
		this.gridColumn15.Caption = "G/L Increase Account";
		this.gridColumn15.ColumnEdit = this.rep_bte_Account;
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "IncresGlAc";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 14;
		this.gridColumn15.Width = 113;
		this.gridColumn16.Caption = "WIP Inventory Account";
		this.gridColumn16.ColumnEdit = this.rep_bte_Account;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "WipAcct";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 15;
		this.gridColumn16.Width = 123;
		this.gridColumn17.Caption = "WIP Inventory Variance Account";
		this.gridColumn17.ColumnEdit = this.rep_bte_Account;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "WipVarAcct";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 16;
		this.gridColumn17.Width = 167;
		this.gridColumn18.Caption = "WIP Offset P&L Account";
		this.gridColumn18.ColumnEdit = this.rep_bte_Account;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "WipOffset";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 17;
		this.gridColumn18.Width = 127;
		this.gridColumn19.Caption = "Inventory Offset P&L Account";
		this.gridColumn19.ColumnEdit = this.rep_bte_Account;
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "StockOffst";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 18;
		this.gridColumn19.Width = 155;
		this.gridColumn20.Caption = "Expense Clearing Account";
		this.gridColumn20.ColumnEdit = this.rep_bte_Account;
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "ExpClrAct";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 19;
		this.gridColumn20.Width = 135;
		this.gridColumn21.Caption = "Stock In Transit Account";
		this.gridColumn21.ColumnEdit = this.rep_bte_Account;
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "StkInTnAct";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 20;
		this.gridColumn21.Width = 127;
		this.gridColumn22.Caption = "Shipped Goods Account";
		this.gridColumn22.ColumnEdit = this.rep_bte_Account;
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "ShpdGdsAct";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn22.Visible = true;
		this.gridColumn22.VisibleIndex = 21;
		this.gridColumn22.Width = 123;
		this.gridColumn23.Caption = "Sales Credit Account";
		this.gridColumn23.ColumnEdit = this.rep_bte_Account;
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "ARCMAct";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn23.Visible = true;
		this.gridColumn23.VisibleIndex = 22;
		this.gridColumn23.Width = 109;
		this.gridColumn24.Caption = "Purchase Credit Account";
		this.gridColumn24.ColumnEdit = this.rep_bte_Account;
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "APCMAct";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn24.Visible = true;
		this.gridColumn24.VisibleIndex = 23;
		this.gridColumn24.Width = 128;
		this.txt_WarehouseCode.EditControl = null;
		this.txt_WarehouseCode.FieldName = null;
		this.txt_WarehouseCode.IsSearchControl = false;
		this.txt_WarehouseCode.Location = new System.Drawing.Point(104, 5);
		this.txt_WarehouseCode.Name = "txt_WarehouseCode";
		this.txt_WarehouseCode.SettingInVisible = false;
		this.txt_WarehouseCode.SettingReadonly = false;
		this.txt_WarehouseCode.Size = new System.Drawing.Size(100, 20);
		this.txt_WarehouseCode.TabIndex = 1;
		this.txt_WarehouseName.EditControl = null;
		this.txt_WarehouseName.FieldName = null;
		this.txt_WarehouseName.IsSearchControl = false;
		this.txt_WarehouseName.Location = new System.Drawing.Point(312, 5);
		this.txt_WarehouseName.Name = "txt_WarehouseName";
		this.txt_WarehouseName.SettingInVisible = false;
		this.txt_WarehouseName.SettingReadonly = false;
		this.txt_WarehouseName.Size = new System.Drawing.Size(161, 20);
		this.txt_WarehouseName.TabIndex = 1;
		this.lb_WareHouseCode.EditControl = null;
		this.lb_WareHouseCode.EditValue = "Warehouse Code";
		this.lb_WareHouseCode.FieldName = null;
		this.lb_WareHouseCode.HideWhenEmpty = false;
		this.lb_WareHouseCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_WareHouseCode.Location = new System.Drawing.Point(8, 8);
		this.lb_WareHouseCode.Name = "lb_WareHouseCode";
		this.lb_WareHouseCode.ReadOnly = false;
		this.lb_WareHouseCode.SettingInVisible = false;
		this.lb_WareHouseCode.SettingReadonly = false;
		this.lb_WareHouseCode.ShowUnderLine = true;
		this.lb_WareHouseCode.Size = new System.Drawing.Size(99, 20);
		this.lb_WareHouseCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_WareHouseCode.TabIndex = 2;
		this.lb_WareHouseCode.Utext = "Warehouse Code";
		this.lb_WarehouseName.EditControl = null;
		this.lb_WarehouseName.EditValue = "Warehouse Name";
		this.lb_WarehouseName.FieldName = null;
		this.lb_WarehouseName.HideWhenEmpty = false;
		this.lb_WarehouseName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_WarehouseName.Location = new System.Drawing.Point(217, 8);
		this.lb_WarehouseName.Name = "lb_WarehouseName";
		this.lb_WarehouseName.ReadOnly = false;
		this.lb_WarehouseName.SettingInVisible = false;
		this.lb_WarehouseName.SettingReadonly = false;
		this.lb_WarehouseName.ShowUnderLine = true;
		this.lb_WarehouseName.Size = new System.Drawing.Size(99, 20);
		this.lb_WarehouseName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_WarehouseName.TabIndex = 2;
		this.lb_WarehouseName.Utext = "Warehouse Name";
		this.bt_Search.EditControl = null;
		this.bt_Search.EditValue = "Search";
		this.bt_Search.FieldName = null;
		this.bt_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Search.ImageOptions.Image");
		this.bt_Search.IsSearchControl = false;
		this.bt_Search.Location = new System.Drawing.Point(671, 3);
		this.bt_Search.Name = "bt_Search";
		this.bt_Search.ReadOnly = false;
		this.bt_Search.SettingInVisible = false;
		this.bt_Search.SettingReadonly = false;
		this.bt_Search.Size = new System.Drawing.Size(75, 23);
		this.bt_Search.TabIndex = 3;
		this.bt_Search.Text = "Search";
		this.bt_Search.Click += new System.EventHandler(this.bt_Search_Click);
		this.lb_Branch.EditControl = null;
		this.lb_Branch.EditValue = "Branch";
		this.lb_Branch.FieldName = null;
		this.lb_Branch.HideWhenEmpty = false;
		this.lb_Branch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Branch.Location = new System.Drawing.Point(485, 8);
		this.lb_Branch.Name = "lb_Branch";
		this.lb_Branch.ReadOnly = false;
		this.lb_Branch.SettingInVisible = false;
		this.lb_Branch.SettingReadonly = false;
		this.lb_Branch.ShowUnderLine = true;
		this.lb_Branch.Size = new System.Drawing.Size(86, 20);
		this.lb_Branch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Branch.TabIndex = 5;
		this.lb_Branch.Utext = "Branch";
		this.lookUE_Branch.Location = new System.Drawing.Point(566, 5);
		this.lookUE_Branch.Name = "lookUE_Branch";
		this.lookUE_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUE_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLId", "BPLId", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("BPLName", "BPLName")
		});
		this.lookUE_Branch.Properties.DisplayMember = "BPLName";
		this.lookUE_Branch.Properties.NullText = "";
		this.lookUE_Branch.Properties.ValueMember = "BPLId";
		this.lookUE_Branch.Size = new System.Drawing.Size(99, 20);
		this.lookUE_Branch.TabIndex = 6;
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(1, 451);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 7;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(79, 451);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 7;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(790, 478);
		base.Controls.Add(this.txt_WarehouseName);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.lookUE_Branch);
		base.Controls.Add(this.lb_Branch);
		base.Controls.Add(this.bt_Search);
		base.Controls.Add(this.lb_WarehouseName);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.txt_WarehouseCode);
		base.Controls.Add(this.lb_WareHouseCode);
		base.FormId = "oWarehouse";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WareHouses";
		this.Text = "WareHouse";
		base.Load += new System.EventHandler(this.frm_WareHouses_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Inactive).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_DropShip).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Location).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Nettable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_BinActivat).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Country).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_State).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_ManageSnB).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabPage_BinLocations.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_BinLocation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_BinLocation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_RecBinEnab).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_calc_NoOfBinLoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_DftBinAbs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_DftBinEnfd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_AutoIssMtd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_AutoRecvMd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_RecItemsBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bin_RecvUpTo).EndInit();
		this.tabPage_Accounting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Account).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Account).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Account).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WarehouseCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WarehouseName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUE_Branch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
