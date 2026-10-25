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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_Braches : XtraFormEx
{
	private const string FormController = "Branches";

	private const string tableName = "OBPL";

	private bool check;

	private List<int> lstRemove = new List<int>();

	private IContainer components;

	public SimpleButtonEx btn_Save;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	public SimpleButtonEx btnCancel;

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

	private GridColumnEx gridColumn25;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private GridColumnEx gridColumn29;

	private GridColumnEx gridColumn30;

	private GridColumnEx gridColumn31;

	private GridColumnEx gridColumn32;

	private GridColumnEx gridColumn33;

	private GridColumnEx gridColumn34;

	private GridColumnEx gridColumn35;

	private GridColumnEx gridColumn36;

	private GridColumnEx gridColumn37;

	private GridColumnEx gridColumn38;

	private GridColumnEx gridColumn39;

	private GridColumnEx gridColumn40;

	private GridColumnEx gridColumn41;

	private GridColumnEx gridColumn42;

	private GridColumnEx gridColumn43;

	private GridColumnEx gridColumn44;

	private GridColumnEx gridColumn45;

	private GridColumnEx gridColumn46;

	private GridColumnEx gridColumn47;

	private GridColumnEx gridColumn48;

	private GridColumnEx gridColumn49;

	private GridColumnEx gridColumn50;

	private GridColumnEx gridColumn51;

	private GridColumnEx gridColumn52;

	private GridColumnEx gridColumn53;

	private GridColumnEx gridColumn54;

	private RepositoryItemCheckEdit rep_MainBranch;

	private RepositoryItemButtonEdit rep_WareHouse;

	private RepositoryItemButtonEdit rep_Country;

	private RepositoryItemButtonEdit rep_Whse;

	private RepositoryItemButtonEdit rep_Customer;

	private RepositoryItemButtonEdit rep_Vender;

	private RepositoryItemButtonEdit rep_State;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private RepositoryItemCheckEdit rep_Disabled;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_Braches()
	{
		InitializeComponent();
	}

	private async void frm_Braches_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async void AddNew()
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (dataSourceTable.Columns.Count > 0)
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["BPLId"] = 0;
				dataRow["MainBPL"] = "N";
				dataRow["Disabled"] = "N";
				dataRow["LogInstanc"] = 0;
				dataRow["CompNature"] = -1;
				dataRow["EconActT"] = -1;
				dataRow["CoopAssocT"] = -1;
				dataRow["ProfTax"] = -1;
				dataRow["CompQualif"] = -1;
				dataRow["DeclType"] = -1;
				dataRow["EnvTypeNFe"] = -1;
				dataRow["Opt4ICMS"] = "N";
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				grdDoc.DataSource = dataSourceTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetMaxCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Branches");
			return await helper.GetAsync("GetMaxCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Branches");
			DataTable dataTable = await helper.GetAsync("LoadDoc");
			if ((from r in dataTable.AsEnumerable()
				where Function.ToString(r["MainBPL"]) == "Y"
				select r).ToList().Count > 0)
			{
				check = true;
			}
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			AddNew();
			lstRemove.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataTable dataTable2 = dataTable.Copy();
			DataRow[] array = (from t in dataTable2.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["BPLName"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				dataTable2 = array.CopyToDataTable();
			}
			List<IGrouping<string, DataRow>> list = (from i in dataTable2.AsEnumerable()
				group i by Function.ToString(i.Field<string>("BPLName")).ToUpper() into gr
				where gr.Count() > 1
				select gr).ToList();
			if (list != null && list.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableBpPlace, "");
				return;
			}
			string text = CheckExistBPLInOWHS(dataTable2);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			foreach (int item in lstRemove)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["BPLId"] = -item;
				dataTable2.Rows.Add(dataRow);
			}
			dataTable2.AcceptChanges();
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Branches");
			HttpResult httpResult = helper.Post("SaveOBPL", dataTable2);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				await LoadData();
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string CheckExistBPLInOWHS(DataTable dtDoc)
	{
		try
		{
			foreach (DataRow row in dtDoc.Rows)
			{
				int num = Function.ParseInt(row["BPLId"]);
				string text = Function.ToString(row["DflWhs"]);
				string text2 = Function.ToString(row["DfltResWhs"]);
				if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(text2))
				{
					return "Select a warehouse assigned to this branch";
				}
				Tuple<string, string> pr = new Tuple<string, string>(text, text2);
				using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Branches");
				DataTable dataTable = httpClientHelper.Post("CheckExistBPLInOWHS", pr);
				if (num > 0 && dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])) && num != Function.ParseInt(dataTable.Rows[0][0]))
				{
					return "Select a warehouse not assigned to any other branches";
				}
				if (num == 0 && dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[0][0]) > 0)
				{
					return "Select a warehouse not assigned to any other branches";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
		return string.Empty;
	}

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int codeMax = 0;
			if (dataTable.IsNotNull())
			{
				codeMax = Function.ParseInt(dataTable.Rows[dataTable.Rows.Count - 1]["BPLId"]);
			}
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Branches");
			DataTable dataTable2 = await helper.GetAsync("GetAddNewRow", new
			{
				tableName = "OBPL",
				codeMax = codeMax
			});
			if (dataTable2.IsNotNull())
			{
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task REFRESH_DATA()
	{
		frm_Braches_Load(null, null);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				switch (keyData)
				{
				case Keys.Return:
					btn_Save_Click(null, null);
					break;
				case Keys.Tab:
					EventTabkey();
					break;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void EventTabkey()
	{
		try
		{
			if (gvDoc.FocusedColumn.FieldName == "DflCust" && Function.ToString(gvDoc.GetFocusedRowCellValue("Disabled")) == "N" && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("DflCust"))))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("DflCust", frm_CFL2.ChooseItem["CardCode"]);
				}
			}
			if (gvDoc.FocusedColumn.FieldName == "DflVendor" && Function.ToString(gvDoc.GetFocusedRowCellValue("Disabled")) == "N" && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("DflVendor"))))
			{
				frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.Vendor, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL3.ShowDialog(this);
				if (frm_CFL3.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("DflVendor", frm_CFL3.ChooseItem["CardCode"]);
				}
			}
			if (gvDoc.FocusedColumn.FieldName == "DflWhs" && Function.ToString(gvDoc.GetFocusedRowCellValue("Disabled")) == "N" && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("DflWhs"))))
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("DflWhs", frm_ChooseFromList2.ChooseItem["WhsCode"]);
				}
			}
			if (gvDoc.FocusedColumn.FieldName == "DfltResWhs" && Function.ToString(gvDoc.GetFocusedRowCellValue("Disabled")) == "N" && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("DfltResWhs"))))
			{
				frm_ChooseFromList frm_ChooseFromList3 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList3.ShowDialog(this);
				if (frm_ChooseFromList3.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("DfltResWhs", frm_ChooseFromList3.ChooseItem["WhsCode"]);
				}
			}
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("Country"));
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseCountryBefore, "");
				return;
			}
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.State, multiSelect: false, string.Empty);
			frm_ChooseFromList2.Coutry = text;
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("State", frm_ChooseFromList2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull() && (e.Column.FieldName == "BPLName" || e.Column.FieldName == "BPLFrName" || e.Column.FieldName == "Address" || e.Column.FieldName == "AddressFr" || e.Column.FieldName == "MainBPL" || e.Column.FieldName == "TxOffcNo" || e.Column.FieldName == "Disabled" || e.Column.FieldName == "DflCust" || e.Column.FieldName == "DflVendor" || e.Column.FieldName == "DflWhs" || e.Column.FieldName == "TaxIdNum" || e.Column.FieldName == "AddrType" || e.Column.FieldName == "Street" || e.Column.FieldName == "StreetNo" || e.Column.FieldName == "Building" || e.Column.FieldName == "ZipCode" || e.Column.FieldName == "Block" || e.Column.FieldName == "City" || e.Column.FieldName == "State" || e.Column.FieldName == "County" || e.Column.FieldName == "Country" || e.Column.FieldName == "AliasName" || e.Column.FieldName == "GlblLocNum" || e.Column.FieldName == "DfltResWhs"))
			{
				if (e.RowHandle == dataTable.Rows.Count - 1)
				{
					AddNew();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					btn_Save.Enabled = false;
					await SaveData();
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void rep_WareHouse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
				string whsSearchCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gridColumn18));
				frm_WareHouses2._whsSearchCode = whsSearchCode;
				frm_WareHouses2.MdiParent = MdiParent;
				frm_WareHouses2.Show();
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("DflWhs", frm_ChooseFromList2.ChooseItem["WhsCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Whse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
				string whsSearchCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gridColumn54));
				frm_WareHouses2._whsSearchCode = whsSearchCode;
				frm_WareHouses2.MdiParent = MdiParent;
				frm_WareHouses2.Show();
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("DfltResWhs", frm_ChooseFromList2.ChooseItem["WhsCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Country_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ListOfInstallBase, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("Country", frm_ChooseFromList2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Customer_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				string copyF_CodeCustomer = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gridColumn16));
				frm_BusinessMasterData2._copyF_CodeCustomer = copyF_CodeCustomer;
				frm_BusinessMasterData2.ShowDialog(this);
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("DflCust", frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Vender_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				string copyF_CodeCustomer = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gridColumn17));
				frm_BusinessMasterData2._copyF_CodeCustomer = copyF_CodeCustomer;
				frm_BusinessMasterData2.ShowDialog(this);
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Vendor, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("DflVendor", frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (e.Column.FieldName == "MainBPL")
			{
				if (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gridColumn10)) == "Y" || !check)
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
				if (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gridColumn10)) == "N" && check)
				{
					e.Appearance.BackColor = Color.LightGray;
				}
			}
			if (e.Column.FieldName == "Address" || e.Column.FieldName == "AddressFr")
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("MainBPL"));
			if (gvDoc.FocusedColumn.FieldName == "MainBPL" && text.Equals("N") && check)
			{
				e.Cancel = true;
			}
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("Disabled")).Equals("Y") && gvDoc.FocusedColumn.FieldName != "Disabled")
			{
				e.Cancel = true;
			}
			if (gvDoc.FocusedColumn.FieldName == "Address" || gvDoc.FocusedColumn.FieldName == "AddressFr")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ToString((sender as GridView).GetRowCellValue(e.RowHandle, gridColumn12)) == "Y")
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Disabled_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gvDoc.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_MainBranch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			CheckEdit obj = sender as CheckEdit;
			gvDoc.PostEditor();
			if (obj.Checked)
			{
				check = true;
			}
			else
			{
				check = false;
			}
			gvDoc.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "BPLName" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("BPLId")) > 0)
				{
					lstRemove.Add(Function.ParseInt(gvDoc.GetFocusedRowCellValue("BPLId")));
				}
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				dataTable.Rows.RemoveAt(focusedRowHandle);
				dataTable.AcceptChanges();
				grdDoc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Update;
			}
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Braches));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MainBranch = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Disabled = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Customer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Vender = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WareHouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn54 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Whse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn32 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn50 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn34 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn36 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn37 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn38 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn39 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn40 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn41 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn42 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn43 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Country = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn44 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn45 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn46 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn47 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn48 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn49 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn51 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn52 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn53 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_State = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MainBranch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Disabled).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Customer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Vender).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WareHouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Country).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_State).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(0, 302);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 82;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(2, 1);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_MainBranch, this.rep_WareHouse, this.rep_Country, this.rep_Whse, this.rep_Customer, this.rep_Vender, this.rep_State, this.repositoryItemButtonEdit1, this.rep_Disabled });
		this.grdDoc.Size = new System.Drawing.Size(827, 295);
		this.grdDoc.TabIndex = 80;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[54]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn21, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9,
			this.gridColumn10, this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn54,
			this.gridColumn19, this.gridColumn20, this.gridColumn22, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn29,
			this.gridColumn30, this.gridColumn31, this.gridColumn32, this.gridColumn33, this.gridColumn50, this.gridColumn34, this.gridColumn35, this.gridColumn36, this.gridColumn37, this.gridColumn38,
			this.gridColumn39, this.gridColumn40, this.gridColumn41, this.gridColumn42, this.gridColumn43, this.gridColumn44, this.gridColumn45, this.gridColumn46, this.gridColumn47, this.gridColumn48,
			this.gridColumn49, this.gridColumn51, this.gridColumn52, this.gridColumn53
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oBraches";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OBPL";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDoc_RowStyle);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gridColumn1.Caption = "BPLId";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "BPLId";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Branch Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "BPLName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 80;
		this.gridColumn3.Caption = "Branch Name (Foreign)";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "BPLFrName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 140;
		this.gridColumn21.Caption = "Branch Reg. No.";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "TaxIdNum";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 2;
		this.gridColumn21.Width = 89;
		this.gridColumn4.Caption = "gridColumn4";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "VATRegNum";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn5.Caption = "gridColumn5";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "RepName";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn6.Caption = "gridColumn6";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Industry";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn7.Caption = "gridColumn7";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Business";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "Address";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Address";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 3;
		this.gridColumn8.Width = 275;
		this.gridColumn9.Caption = "Address (Foreign)";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "AddressFr";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 4;
		this.gridColumn9.Width = 101;
		this.gridColumn10.Caption = "Main Branch";
		this.gridColumn10.ColumnEdit = this.rep_MainBranch;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "MainBPL";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 5;
		this.gridColumn10.Width = 66;
		this.rep_MainBranch.AutoHeight = false;
		this.rep_MainBranch.Name = "rep_MainBranch";
		this.rep_MainBranch.ValueChecked = "Y";
		this.rep_MainBranch.ValueUnchecked = "N";
		this.rep_MainBranch.EditValueChanged += new System.EventHandler(this.rep_MainBranch_EditValueChanged);
		this.gridColumn11.Caption = "Tax Office No.";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "TxOffcNo";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 6;
		this.gridColumn11.Width = 83;
		this.gridColumn12.Caption = "Disabled";
		this.gridColumn12.ColumnEdit = this.rep_Disabled;
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "Disabled";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 7;
		this.gridColumn12.Width = 52;
		this.rep_Disabled.AutoHeight = false;
		this.rep_Disabled.Name = "rep_Disabled";
		this.rep_Disabled.ValueChecked = "Y";
		this.rep_Disabled.ValueUnchecked = "N";
		this.rep_Disabled.EditValueChanged += new System.EventHandler(this.rep_Disabled_EditValueChanged);
		this.gridColumn13.Caption = "gridColumn13";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "LogInstanc";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "gridColumn14";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "UserSign2";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "gridColumn15";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "UpdateDate";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "Default Customer ID";
		this.gridColumn16.ColumnEdit = this.rep_Customer;
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "DflCust";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 8;
		this.gridColumn16.Width = 109;
		this.rep_Customer.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Customer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Customer.Name = "rep_Customer";
		this.rep_Customer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Customer_ButtonClick);
		this.gridColumn17.Caption = "Default Vendor ID";
		this.gridColumn17.ColumnEdit = this.rep_Vender;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "DflVendor";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 9;
		this.gridColumn17.Width = 94;
		this.rep_Vender.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Vender.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Vender.Name = "rep_Vender";
		this.rep_Vender.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Vender_ButtonClick);
		this.gridColumn18.Caption = "Default Warehouse ID";
		this.gridColumn18.ColumnEdit = this.rep_WareHouse;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "DflWhs";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 10;
		this.gridColumn18.Width = 115;
		this.rep_WareHouse.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_WareHouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_WareHouse.Name = "rep_WareHouse";
		this.rep_WareHouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WareHouse_ButtonClick);
		this.gridColumn54.Caption = "Default Resource Warehouse ID";
		this.gridColumn54.ColumnEdit = this.rep_Whse;
		this.gridColumn54.CurrencyDateField = null;
		this.gridColumn54.CurrencyField = null;
		this.gridColumn54.CurrencyFixed = null;
		this.gridColumn54.CurrencyFixedRate = 0.0;
		this.gridColumn54.CurrencyRateField = null;
		this.gridColumn54.FieldName = "DfltResWhs";
		this.gridColumn54.Name = "gridColumn54";
		this.gridColumn54.PrimaryKey = false;
		this.gridColumn54.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn54.UnitField = null;
		this.gridColumn54.Visible = true;
		this.gridColumn54.VisibleIndex = 11;
		this.gridColumn54.Width = 163;
		this.rep_Whse.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Whse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Whse.Name = "rep_Whse";
		this.rep_Whse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Whse_ButtonClick);
		this.gridColumn19.Caption = "gridColumn19";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "DflTaxCode";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn20.Caption = "gridColumn20";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "RevOffice";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn22.Caption = "gridColumn22";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "TaxIdNum2";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn23.Caption = "gridColumn23";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "TaxIdNum3";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn24.Caption = "gridColumn24";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "AddtnlId";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn25.Caption = "gridColumn25";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "CompNature";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn26.Caption = "gridColumn26";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "EconActT";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn27.Caption = "gridColumn27";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "CredCOrig";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn28.Caption = "gridColumn28";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "IPIPeriod";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn29.Caption = "gridColumn29";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "CoopAssocT";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.gridColumn30.Caption = "gridColumn30";
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.FieldName = "PrefState";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		this.gridColumn31.Caption = "gridColumn31";
		this.gridColumn31.CurrencyDateField = null;
		this.gridColumn31.CurrencyField = null;
		this.gridColumn31.CurrencyFixed = null;
		this.gridColumn31.CurrencyFixedRate = 0.0;
		this.gridColumn31.CurrencyRateField = null;
		this.gridColumn31.FieldName = "ProfTax";
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.PrimaryKey = false;
		this.gridColumn31.UnitField = null;
		this.gridColumn32.Caption = "gridColumn32";
		this.gridColumn32.CurrencyDateField = null;
		this.gridColumn32.CurrencyField = null;
		this.gridColumn32.CurrencyFixed = null;
		this.gridColumn32.CurrencyFixedRate = 0.0;
		this.gridColumn32.CurrencyRateField = null;
		this.gridColumn32.FieldName = "CompQualif";
		this.gridColumn32.Name = "gridColumn32";
		this.gridColumn32.PrimaryKey = false;
		this.gridColumn32.UnitField = null;
		this.gridColumn33.Caption = "gridColumn33";
		this.gridColumn33.CurrencyDateField = null;
		this.gridColumn33.CurrencyField = null;
		this.gridColumn33.CurrencyFixed = null;
		this.gridColumn33.CurrencyFixedRate = 0.0;
		this.gridColumn33.CurrencyRateField = null;
		this.gridColumn33.FieldName = "DeclType";
		this.gridColumn33.Name = "gridColumn33";
		this.gridColumn33.PrimaryKey = false;
		this.gridColumn33.UnitField = null;
		this.gridColumn50.Caption = "Alias Name";
		this.gridColumn50.CurrencyDateField = null;
		this.gridColumn50.CurrencyField = null;
		this.gridColumn50.CurrencyFixed = null;
		this.gridColumn50.CurrencyFixedRate = 0.0;
		this.gridColumn50.CurrencyRateField = null;
		this.gridColumn50.FieldName = "AliasName";
		this.gridColumn50.Name = "gridColumn50";
		this.gridColumn50.PrimaryKey = false;
		this.gridColumn50.UnitField = null;
		this.gridColumn50.Visible = true;
		this.gridColumn50.VisibleIndex = 12;
		this.gridColumn50.Width = 172;
		this.gridColumn34.Caption = "Address Type";
		this.gridColumn34.CurrencyDateField = null;
		this.gridColumn34.CurrencyField = null;
		this.gridColumn34.CurrencyFixed = null;
		this.gridColumn34.CurrencyFixedRate = 0.0;
		this.gridColumn34.CurrencyRateField = null;
		this.gridColumn34.FieldName = "AddrType";
		this.gridColumn34.Name = "gridColumn34";
		this.gridColumn34.PrimaryKey = false;
		this.gridColumn34.UnitField = null;
		this.gridColumn34.Visible = true;
		this.gridColumn34.VisibleIndex = 13;
		this.gridColumn34.Width = 74;
		this.gridColumn35.Caption = "Street";
		this.gridColumn35.CurrencyDateField = null;
		this.gridColumn35.CurrencyField = null;
		this.gridColumn35.CurrencyFixed = null;
		this.gridColumn35.CurrencyFixedRate = 0.0;
		this.gridColumn35.CurrencyRateField = null;
		this.gridColumn35.FieldName = "Street";
		this.gridColumn35.Name = "gridColumn35";
		this.gridColumn35.PrimaryKey = false;
		this.gridColumn35.UnitField = null;
		this.gridColumn35.Visible = true;
		this.gridColumn35.VisibleIndex = 14;
		this.gridColumn35.Width = 287;
		this.gridColumn36.Caption = "Street No.";
		this.gridColumn36.CurrencyDateField = null;
		this.gridColumn36.CurrencyField = null;
		this.gridColumn36.CurrencyFixed = null;
		this.gridColumn36.CurrencyFixedRate = 0.0;
		this.gridColumn36.CurrencyRateField = null;
		this.gridColumn36.FieldName = "StreetNo";
		this.gridColumn36.Name = "gridColumn36";
		this.gridColumn36.PrimaryKey = false;
		this.gridColumn36.UnitField = null;
		this.gridColumn36.Visible = true;
		this.gridColumn36.VisibleIndex = 15;
		this.gridColumn36.Width = 59;
		this.gridColumn37.Caption = "Building/Floor/Room";
		this.gridColumn37.CurrencyDateField = null;
		this.gridColumn37.CurrencyField = null;
		this.gridColumn37.CurrencyFixed = null;
		this.gridColumn37.CurrencyFixedRate = 0.0;
		this.gridColumn37.CurrencyRateField = null;
		this.gridColumn37.FieldName = "Building";
		this.gridColumn37.Name = "gridColumn37";
		this.gridColumn37.PrimaryKey = false;
		this.gridColumn37.UnitField = null;
		this.gridColumn37.Visible = true;
		this.gridColumn37.VisibleIndex = 16;
		this.gridColumn37.Width = 105;
		this.gridColumn38.Caption = "Zip Code";
		this.gridColumn38.CurrencyDateField = null;
		this.gridColumn38.CurrencyField = null;
		this.gridColumn38.CurrencyFixed = null;
		this.gridColumn38.CurrencyFixedRate = 0.0;
		this.gridColumn38.CurrencyRateField = null;
		this.gridColumn38.FieldName = "ZipCode";
		this.gridColumn38.Name = "gridColumn38";
		this.gridColumn38.PrimaryKey = false;
		this.gridColumn38.UnitField = null;
		this.gridColumn38.Visible = true;
		this.gridColumn38.VisibleIndex = 17;
		this.gridColumn38.Width = 50;
		this.gridColumn39.Caption = "Block";
		this.gridColumn39.CurrencyDateField = null;
		this.gridColumn39.CurrencyField = null;
		this.gridColumn39.CurrencyFixed = null;
		this.gridColumn39.CurrencyFixedRate = 0.0;
		this.gridColumn39.CurrencyRateField = null;
		this.gridColumn39.FieldName = "Block";
		this.gridColumn39.Name = "gridColumn39";
		this.gridColumn39.PrimaryKey = false;
		this.gridColumn39.UnitField = null;
		this.gridColumn39.Visible = true;
		this.gridColumn39.VisibleIndex = 18;
		this.gridColumn39.Width = 37;
		this.gridColumn40.Caption = "City";
		this.gridColumn40.CurrencyDateField = null;
		this.gridColumn40.CurrencyField = null;
		this.gridColumn40.CurrencyFixed = null;
		this.gridColumn40.CurrencyFixedRate = 0.0;
		this.gridColumn40.CurrencyRateField = null;
		this.gridColumn40.FieldName = "City";
		this.gridColumn40.Name = "gridColumn40";
		this.gridColumn40.PrimaryKey = false;
		this.gridColumn40.UnitField = null;
		this.gridColumn40.Visible = true;
		this.gridColumn40.VisibleIndex = 19;
		this.gridColumn40.Width = 31;
		this.gridColumn41.Caption = "State";
		this.gridColumn41.ColumnEdit = this.repositoryItemButtonEdit1;
		this.gridColumn41.CurrencyDateField = null;
		this.gridColumn41.CurrencyField = null;
		this.gridColumn41.CurrencyFixed = null;
		this.gridColumn41.CurrencyFixedRate = 0.0;
		this.gridColumn41.CurrencyRateField = null;
		this.gridColumn41.FieldName = "State";
		this.gridColumn41.Name = "gridColumn41";
		this.gridColumn41.PrimaryKey = false;
		this.gridColumn41.UnitField = null;
		this.gridColumn41.Visible = true;
		this.gridColumn41.VisibleIndex = 20;
		this.gridColumn41.Width = 103;
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.repositoryItemButtonEdit1.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1_ButtonClick);
		this.gridColumn42.Caption = "County";
		this.gridColumn42.CurrencyDateField = null;
		this.gridColumn42.CurrencyField = null;
		this.gridColumn42.CurrencyFixed = null;
		this.gridColumn42.CurrencyFixedRate = 0.0;
		this.gridColumn42.CurrencyRateField = null;
		this.gridColumn42.FieldName = "County";
		this.gridColumn42.Name = "gridColumn42";
		this.gridColumn42.PrimaryKey = false;
		this.gridColumn42.UnitField = null;
		this.gridColumn42.Visible = true;
		this.gridColumn42.VisibleIndex = 21;
		this.gridColumn42.Width = 45;
		this.gridColumn43.Caption = "Country";
		this.gridColumn43.ColumnEdit = this.rep_Country;
		this.gridColumn43.CurrencyDateField = null;
		this.gridColumn43.CurrencyField = null;
		this.gridColumn43.CurrencyFixed = null;
		this.gridColumn43.CurrencyFixedRate = 0.0;
		this.gridColumn43.CurrencyRateField = null;
		this.gridColumn43.FieldName = "Country";
		this.gridColumn43.Name = "gridColumn43";
		this.gridColumn43.PrimaryKey = false;
		this.gridColumn43.UnitField = null;
		this.gridColumn43.Visible = true;
		this.gridColumn43.VisibleIndex = 22;
		this.gridColumn43.Width = 52;
		this.rep_Country.AutoHeight = false;
		this.rep_Country.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Country.Name = "rep_Country";
		this.rep_Country.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Country_ButtonClick);
		this.gridColumn44.Caption = "gridColumn44";
		this.gridColumn44.CurrencyDateField = null;
		this.gridColumn44.CurrencyField = null;
		this.gridColumn44.CurrencyFixed = null;
		this.gridColumn44.CurrencyFixedRate = 0.0;
		this.gridColumn44.CurrencyRateField = null;
		this.gridColumn44.FieldName = "PmtClrAct";
		this.gridColumn44.Name = "gridColumn44";
		this.gridColumn44.PrimaryKey = false;
		this.gridColumn44.UnitField = null;
		this.gridColumn45.Caption = "gridColumn45";
		this.gridColumn45.CurrencyDateField = null;
		this.gridColumn45.CurrencyField = null;
		this.gridColumn45.CurrencyFixed = null;
		this.gridColumn45.CurrencyFixedRate = 0.0;
		this.gridColumn45.CurrencyRateField = null;
		this.gridColumn45.FieldName = "CommerReg";
		this.gridColumn45.Name = "gridColumn45";
		this.gridColumn45.PrimaryKey = false;
		this.gridColumn45.UnitField = null;
		this.gridColumn46.Caption = "gridColumn46";
		this.gridColumn46.CurrencyDateField = null;
		this.gridColumn46.CurrencyField = null;
		this.gridColumn46.CurrencyFixed = null;
		this.gridColumn46.CurrencyFixedRate = 0.0;
		this.gridColumn46.CurrencyRateField = null;
		this.gridColumn46.FieldName = "DateOfInc";
		this.gridColumn46.Name = "gridColumn46";
		this.gridColumn46.PrimaryKey = false;
		this.gridColumn46.UnitField = null;
		this.gridColumn47.Caption = "gridColumn47";
		this.gridColumn47.CurrencyDateField = null;
		this.gridColumn47.CurrencyField = null;
		this.gridColumn47.CurrencyFixed = null;
		this.gridColumn47.CurrencyFixedRate = 0.0;
		this.gridColumn47.CurrencyRateField = null;
		this.gridColumn47.FieldName = "SPEDProf";
		this.gridColumn47.Name = "gridColumn47";
		this.gridColumn47.PrimaryKey = false;
		this.gridColumn47.UnitField = null;
		this.gridColumn48.Caption = "gridColumn48";
		this.gridColumn48.CurrencyDateField = null;
		this.gridColumn48.CurrencyField = null;
		this.gridColumn48.CurrencyFixed = null;
		this.gridColumn48.CurrencyFixedRate = 0.0;
		this.gridColumn48.CurrencyRateField = null;
		this.gridColumn48.FieldName = "EnvTypeNFe";
		this.gridColumn48.Name = "gridColumn48";
		this.gridColumn48.PrimaryKey = false;
		this.gridColumn48.UnitField = null;
		this.gridColumn49.Caption = "gridColumn49";
		this.gridColumn49.CurrencyDateField = null;
		this.gridColumn49.CurrencyField = null;
		this.gridColumn49.CurrencyFixed = null;
		this.gridColumn49.CurrencyFixedRate = 0.0;
		this.gridColumn49.CurrencyRateField = null;
		this.gridColumn49.FieldName = "Opt4ICMS";
		this.gridColumn49.Name = "gridColumn49";
		this.gridColumn49.PrimaryKey = false;
		this.gridColumn49.UnitField = null;
		this.gridColumn51.Caption = "Global Location Number";
		this.gridColumn51.CurrencyDateField = null;
		this.gridColumn51.CurrencyField = null;
		this.gridColumn51.CurrencyFixed = null;
		this.gridColumn51.CurrencyFixedRate = 0.0;
		this.gridColumn51.CurrencyRateField = null;
		this.gridColumn51.FieldName = "GlblLocNum";
		this.gridColumn51.Name = "gridColumn51";
		this.gridColumn51.PrimaryKey = false;
		this.gridColumn51.UnitField = null;
		this.gridColumn51.Visible = true;
		this.gridColumn51.VisibleIndex = 23;
		this.gridColumn51.Width = 119;
		this.gridColumn52.Caption = "gridColumn52";
		this.gridColumn52.CurrencyDateField = null;
		this.gridColumn52.CurrencyField = null;
		this.gridColumn52.CurrencyFixed = null;
		this.gridColumn52.CurrencyFixedRate = 0.0;
		this.gridColumn52.CurrencyRateField = null;
		this.gridColumn52.FieldName = "TaxRptFrm";
		this.gridColumn52.Name = "gridColumn52";
		this.gridColumn52.PrimaryKey = false;
		this.gridColumn52.UnitField = null;
		this.gridColumn53.Caption = "gridColumn53";
		this.gridColumn53.CurrencyDateField = null;
		this.gridColumn53.CurrencyField = null;
		this.gridColumn53.CurrencyFixed = null;
		this.gridColumn53.CurrencyFixedRate = 0.0;
		this.gridColumn53.CurrencyRateField = null;
		this.gridColumn53.FieldName = "Suframa";
		this.gridColumn53.Name = "gridColumn53";
		this.gridColumn53.PrimaryKey = false;
		this.gridColumn53.UnitField = null;
		this.rep_State.AutoHeight = false;
		this.rep_State.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_State.Name = "rep_State";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(102, 302);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 81;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(831, 326);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oBraches";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Braches.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Braches";
		this.Text = "Branches";
		base.Load += new System.EventHandler(this.frm_Braches_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MainBranch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Disabled).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Customer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Vender).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WareHouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Country).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_State).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
