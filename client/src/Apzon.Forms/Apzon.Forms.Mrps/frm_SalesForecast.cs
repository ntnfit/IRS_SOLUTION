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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Mrps;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Mrps;

public class frm_SalesForecast : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	private List<Common.Table_Code> lstSaleEmp = new List<Common.Table_Code>();

	private DataTable dtDocs = new DataTable();

	private DataTable dtDoc = new DataTable();

	private DataTable dtEmployee = new DataTable();

	private DataTable dtTable = new DataTable();

	private DataSet ds = new DataSet();

	private int maxAbsID;

	private DataTable dt = new DataTable();

	private DataTable dtFather = new DataTable();

	private List<Document_LinesImpl> linesEdit = new List<Document_LinesImpl>();

	public ShowForms frmShowChildrenForm;

	private int currentId = -1;

	private bool statusSearch;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_General;

	private GridControlEx grd_Salescast;

	private LabelLine labelLine1;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine2;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private TextEditEx txtBPName;

	private ButtonEditEx btnBPCode;

	private ComboBoxEditEx cbType;

	private ButtonEditEx btnWareHouse;

	private ComboBoxEditEx cbStatus;

	private ButtonEditEx btnDivision;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnQuickAnalysis;

	private SimpleButtonEx btnClosed;

	private SimpleButtonEx simpleButton1;

	private ContextMenuStrip contextMenuStripdoc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private RepositoryItemComboBox cbForeCastType;

	private RepositoryItemComboBox Repository_ForeCastSource;

	private SimpleButtonEx btnSearch;

	private RepositoryItemButtonEdit rep_itemcode;

	private RepositoryItemButtonEdit rep_itemname;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip_SalesForCast;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemComboBox rep_status;

	private RepositoryItemButtonEdit rep_BPCode;

	private RepositoryItemLookUpEdit rILSalesEmp;

	private RepositoryItemLookUpEdit rILStaSalesEmpl;

	private RepositoryItemButtonEdit rep_BPName;

	private RepositoryItemLookUpEdit rep_Employee;

	private RepositoryItemComboBox rep_Year;

	private RepositoryItemButtonEdit rep_Whse;

	private RepositoryItemDateEdit rep_date;

	private SearchLookUpEdit cbEmployee;

	private ComboBoxEditEx dtYear;

	private RepositoryItemComboBox rep_ComboPeriod;

	private RepositoryItemComboBox repFormView;

	private GridViewEx gv_General;

	private GridViewEx gv_Salescast;

	private GridViewEx searchLookUpEdit1View;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx View;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	public frm_SalesForecast()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalesForecast"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalesForecast" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnClosed.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				if (keyData == (Keys.A | Keys.Control))
				{
					AddNewLine();
				}
				if (keyData == (Keys.F | Keys.Control))
				{
					Search();
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception)
		{
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	private void btnWareHouse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, Function.ToString(btnWareHouse.Text));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				btnWareHouse.Text = Function.ToString(frm_ChooseFromList2.ChooseItem["WhsCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gv_General_KeyPress(object sender, KeyPressEventArgs e)
	{
	}

	private void gv_General_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void gv_General_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void addNewToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		AddNewLine();
	}

	private void txtBPName_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void gv_General_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "U_PeriodsToForecast")
			{
				GetData();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_General_RowCellClick(object sender, RowCellClickEventArgs e)
	{
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		WinCommons.Error(Enums.MessageCode.NotCompleteBecauseNotEnoughInfomation, "");
	}

	private void gv_General_RowClick(object sender, RowClickEventArgs e)
	{
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewLine();
	}

	private void gv_General_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void rILStaSalesEmpl_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void rep_Whse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string empty = string.Empty;
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, "");
				frm_ChooseFromList2.ShowDialog(this);
				DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
				if (dataRow == null)
				{
					return;
				}
				empty = Function.ToString(dataRow["WhsCode"]);
				if (!string.IsNullOrEmpty(empty))
				{
					gv_General.SetFocusedRowCellValue("Whse", empty);
				}
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_General.GetFocusedRowCellValue("U_WarehouseCode"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void frm_SalesForecast_KeyPress(object sender, KeyPressEventArgs e)
	{
		if (e.KeyChar == Convert.ToChar(Keys.A | Keys.Control))
		{
			AddNewLine();
		}
		if (e.KeyChar == Convert.ToChar(Keys.F | Keys.Control))
		{
			btnSearch_Click(sender, e);
		}
	}

	private void btnWareHouse_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void btnClosed_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_BPCode_Click(object sender, EventArgs e)
	{
	}

	private async void deleteToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_General.GetSelectedRows() == null || gv_General.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				int[] selectedRows = gv_General.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gv_General.GetRowCellValue(rowHandle, "AbsID").ToString() + ",";
				}
				if (text.Length <= 0)
				{
					return;
				}
				text = text.Remove(text.Length - 1);
				using HttpClientHelper<SalesForeCastImpl> _helper = new HttpClientHelper<SalesForeCastImpl>(Configuration.ApiServer, "SalesForeCast");
				string text2 = await _helper.PostParamsAsJsonAsync("DeleteDocs", text);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					await LoadAllDoc();
					grd_Salescast.DataSource = null;
					gv_Salescast.Columns.Clear();
					gv_Salescast.RefreshData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int ID = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "AbsID"));
			DataTable source = (DataTable)grd_General.DataSource;
			DataRow[] dr = (from t in source.AsEnumerable()
				where Function.ParseInt(t["AbsId"]) == ID
				select t).ToArray();
			await GetMaxAbsID();
			dr[0]["AbsID"] = maxAbsID;
			if (dr != null && dr.Length != 0)
			{
				DataTable dataSource = dr.CopyToDataTable();
				grd_General.DataSource = null;
				grd_General.DataSource = dataSource;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_BPCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				DataRow dataRow = dtDocs.Rows[gv_General.GetDataSourceRowIndex(gv_General.FocusedRowHandle)];
				dataRow["BPCode"] = businessPartnersImpl.CardCode;
				dataRow["BPName"] = businessPartnersImpl.CardName;
				dtDocs.AcceptChanges();
				buttonEdit.EditValue = businessPartnersImpl.CardCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void rep_BPName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
				DataRow dataRow = dtDocs.Rows[gv_General.GetDataSourceRowIndex(gv_General.FocusedRowHandle)];
				dataRow["BPName"] = businessPartnersImpl.CardName;
				dataRow["BPCode"] = businessPartnersImpl.CardCode;
				dtDocs.AcceptChanges();
				buttonEdit.EditValue = businessPartnersImpl.CardName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void cbEmployee_SelectedIndexChanged(object sender, EventArgs e)
	{
	}

	private async void reEmployee_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			await LoadComboEmployee();
			ComboBoxEdit comboBoxEdit = sender as ComboBoxEdit;
			dtDocs.Rows[gv_General.GetDataSourceRowIndex(gv_General.FocusedRowHandle)]["Employee"] = comboBoxEdit.EditValue;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_General_Click(object sender, EventArgs e)
	{
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Salescast.FocusedRowHandle;
			dtDoc.Rows.RemoveAt(focusedRowHandle);
			grd_Salescast.DataSource = dtDoc;
			grd_Salescast.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cboTT_Click(object sender, EventArgs e)
	{
	}

	private void textEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btnQuickAnalysis_Click(object sender, EventArgs e)
	{
		frm_SalesForeCastQuickAnalys frm_SalesForeCastQuickAnalys2 = new frm_SalesForeCastQuickAnalys();
		frm_SalesForeCastQuickAnalys2.MdiParent = MdiParent;
		frm_SalesForeCastQuickAnalys2.Show();
	}

	private void btnBPCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void btnBPCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: false, Function.ToString(btnBPCode.Text));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				btnBPCode.Text = frm_ChooseFromList2.ChooseItem.CardCode;
				txtBPName.Text = frm_ChooseFromList2.ChooseItem.CardName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void btnSave_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			SaveData();
			WinCommons._CloseProcessing();
		}
	}

	private void rep_itemcode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit obj = sender as ButtonEdit;
			ItemsImpl itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItems[0];
			DataRow dataRow = dtDoc.Rows[gv_Salescast.GetDataSourceRowIndex(gv_Salescast.FocusedRowHandle)];
			dataRow["ItemCode"] = itemsImpl.ItemCode;
			dataRow["ItemName"] = itemsImpl.ItemName;
			dtDoc.AcceptChanges();
			obj.EditValue = itemsImpl.ItemCode;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				for (int i = 1; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					DataRow dataRow2 = dtDoc.NewRow();
					ItemsImpl itemsImpl2 = (ItemsImpl)frm_ChooseFromList2.ChooseItems[i];
					dataRow2["ItemCode"] = itemsImpl2.ItemCode;
					dataRow2["ItemName"] = itemsImpl2.ItemName;
					dtDoc.Rows.Add(dataRow2);
					dtDoc.AcceptChanges();
				}
			}
			DataRow dataRow3 = dtDoc.NewRow();
			dataRow3["ItemCode"] = "";
			dataRow3["ItemName"] = "";
			dataRow3["LineID"] = dtDoc.Rows.Count - 1;
			for (int j = 3; j < dtDoc.Columns.Count - 1; j++)
			{
				dataRow3[j] = 0;
			}
			dtDoc.Rows.Add(dataRow3);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_General_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private void rep_itemname_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit obj = sender as ButtonEdit;
			ItemsImpl itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItems[0];
			DataRow dataRow = dtDoc.Rows[gv_Salescast.GetDataSourceRowIndex(gv_Salescast.FocusedRowHandle)];
			dataRow["ItemCode"] = itemsImpl.ItemCode;
			dataRow["ItemName"] = itemsImpl.ItemName;
			dtDoc.AcceptChanges();
			obj.EditValue = itemsImpl.ItemName;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				for (int i = 1; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					DataRow dataRow2 = dtDoc.NewRow();
					ItemsImpl itemsImpl2 = (ItemsImpl)frm_ChooseFromList2.ChooseItems[i];
					dataRow2["ItemCode"] = itemsImpl2.ItemCode;
					dataRow2["ItemName"] = itemsImpl2.ItemName;
					dtDoc.Rows.Add(dataRow2);
					dtDoc.AcceptChanges();
				}
			}
			DataRow dataRow3 = dtDoc.NewRow();
			dataRow3["ItemCode"] = "";
			dataRow3["ItemName"] = "";
			dataRow3["LineID"] = dtDoc.Rows.Count - 1;
			for (int j = 3; j < dtDoc.Columns.Count - 1; j++)
			{
				dataRow3[j] = 0;
			}
			dtDoc.Rows.Add(dataRow3);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnWareHouse_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, Function.ToString(btnWareHouse.Text));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				btnWareHouse.Text = Function.ToString(frm_ChooseFromList2.ChooseItem["WhsCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void frm_SalesForecast_Load(object sender, EventArgs e)
	{
		LoadData();
		gv_Salescast.InitPasteFromExcel();
	}

	private async void LoadData()
	{
		await LoadComboEmployee();
		await LoadAllDoc();
	}

	private async Task LoadComboEmployee()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee");
			dtEmployee = await _helper.GetDataTableAsync("ShowSaleEmployee");
			if (dtEmployee == null || dtEmployee.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dtEmployee.Columns.Count; i++)
			{
				if (!dtEmployee.Columns[i].ColumnName.Equals("SlpCode") && !dtEmployee.Columns[i].ColumnName.Equals("SlpName"))
				{
					dtEmployee.Columns.RemoveAt(i);
					i--;
				}
			}
			cbEmployee.Properties.DataSource = dtEmployee;
			rILStaSalesEmpl.DataSource = dtEmployee;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadAllDoc()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast");
			dtDocs = await _helper.GetDataTableAsync("GetDocs");
			if (dtDocs != null)
			{
				if (grd_General.DataSource != null)
				{
					grd_General.DataSource = null;
					grd_General.RefreshDataSource();
				}
				grd_General.DataSource = dtDocs;
				grd_General.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadDetail()
	{
		try
		{
			Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "AbsID"));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadSalesForeCast()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast");
			dtDocs = await _helper.GetDataTableAsync("GetInfor", new { });
			if (dtDocs != null && dtDocs.Rows.Count > 0)
			{
				dtDocs.PrimaryKey = new DataColumn[1] { dtDocs.Columns["AbsID"] };
				if (dtDocs.Rows.Count == 1 && dtDocs.Rows[0]["AbsID"] != null && Function.ParseInt(dtDocs.Rows[0]["AbsID"]) == 0)
				{
					dtDocs.Rows.Clear();
				}
				grd_General.DataSource = dtDocs;
				grd_General.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Search()
	{
		if (!statusSearch)
		{
			dt.Columns.Add("U_BPCode");
			dt.Columns.Add("U_ForecastType");
			dt.Columns.Add("U_Year");
			dt.Columns.Add("U_BPName");
			dt.Columns.Add("U_Employee");
			dt.Columns.Add("U_Whse");
			dt.Columns.Add("U_Division");
			dt.Columns.Add("U_Status");
			statusSearch = true;
		}
		dt.Rows.Clear();
		DataRow dataRow = dt.NewRow();
		dataRow["U_BPCode"] = ((btnBPCode.EditValue == null) ? "" : btnBPCode.EditValue);
		dataRow["U_ForecastType"] = ((cbType.EditValue == null) ? "" : cbType.EditValue.ToString());
		dataRow["U_Year"] = ((dtYear.EditValue == null) ? "" : dtYear.EditValue);
		dataRow["U_BPName"] = ((txtBPName.EditValue == null) ? "" : txtBPName.EditValue.ToString());
		dataRow["U_Employee"] = ((cbEmployee.EditValue == null) ? "" : cbEmployee.EditValue.ToString());
		dataRow["U_Whse"] = ((btnWareHouse.EditValue == null) ? "" : btnWareHouse.EditValue.ToString());
		dataRow["U_Division"] = ((btnDivision.EditValue == null) ? "" : btnDivision.EditValue.ToString());
		dataRow["U_Status"] = ((cbStatus.EditValue == null) ? "" : cbStatus.EditValue.ToString());
		dt.Rows.Add(dataRow);
		statusSearch = true;
		await Search(dt);
	}

	private async Task GetDataByID(int ID)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast");
			dtDoc = await _helper.GetDataTableAsync("GetInfor", new { ID });
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> WaitSaveData(DataSet dtFather)
	{
		try
		{
			using HttpClientHelper<SalesForeCastImpl> _helper = new HttpClientHelper<SalesForeCastImpl>(Configuration.ApiServer, "SalesForeCast");
			return await _helper.PostParamsAsJsonAsync("SaveDocs", dtFather);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private async void SaveData()
	{
		_ = 1;
		try
		{
			dtFather = (DataTable)grd_General.DataSource;
			dtTable = CreatTableToSaveChild();
			ds.Tables.AddRange(new DataTable[2] { dtFather, dtTable });
			if (await WaitSaveData(ds) == "")
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SaveError, "");
			}
			await LoadAllDoc();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveWaitDataChild(DataTable childTable)
	{
		try
		{
			using HttpClientHelper<SalesForeCastImpl> _helper = new HttpClientHelper<SalesForeCastImpl>(Configuration.ApiServer, "SalesForeCast");
			return await _helper.PostParamsAsJsonAsync("SaveChild", childTable);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private void CreatDataGrid()
	{
		gv_Salescast.Columns.Clear();
		GridColumn gridColumn = new GridColumn();
		gridColumn.Name = "LineID";
		gridColumn.FieldName = "LineID";
		gridColumn.Caption = "Line ID";
		gridColumn.OptionsColumn.AllowEdit = false;
		gridColumn.Visible = false;
		gv_Salescast.Columns.Add(gridColumn);
		GridColumn gridColumn2 = new GridColumn();
		gridColumn2.Name = "ItemCode";
		gridColumn2.FieldName = "ItemCode";
		gridColumn2.Caption = "Item Code";
		gridColumn2.ColumnEdit = rep_itemcode;
		gridColumn2.OptionsColumn.AllowEdit = true;
		gridColumn2.Visible = true;
		gridColumn2.Width = 150;
		gv_Salescast.Columns.Add(gridColumn2);
		GridColumn gridColumn3 = new GridColumn();
		gridColumn3.Name = "ItemName";
		gridColumn3.FieldName = "ItemName";
		gridColumn3.Caption = "Item Name";
		gridColumn3.Width = 150;
		gridColumn3.Visible = true;
		gridColumn3.OptionsColumn.AllowEdit = true;
		gridColumn3.ColumnEdit = rep_itemname;
		gv_Salescast.Columns.Add(gridColumn3);
		int num = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "U_Year"));
		int num2 = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "U_PeriodsToForecast"));
		if (num != 0)
		{
			for (int i = 1; i <= num2; i++)
			{
				GridColumn gridColumn4 = new GridColumn();
				gridColumn4.Caption = new DateTime(num, i, 1).ToString("MMM-yyyy");
				gridColumn4.FieldName = i.ToString() ?? "";
				gridColumn4.Name = i.ToString() ?? "";
				gridColumn4.Visible = true;
				gridColumn4.OptionsColumn.AllowEdit = true;
				gv_Salescast.Columns.Add(gridColumn4);
			}
			for (int j = 1; j <= num2; j++)
			{
				GridColumn gridColumn5 = new GridColumn();
				gridColumn5.Caption = new DateTime(num - 1, j, 1).ToString("MMM-yyyy");
				gridColumn5.FieldName = j + "1";
				gridColumn5.Name = j.ToString() ?? "";
				gridColumn5.Visible = true;
				gridColumn5.OptionsColumn.AllowEdit = false;
				gv_Salescast.Columns.Add(gridColumn5);
			}
		}
	}

	private DataTable CreatTableToSaveChild()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AbsID", typeof(int));
			dataTable.Columns.Add("LineID", typeof(int));
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("Date", typeof(DateTime));
			dataTable.Columns.Add("Quantity", typeof(decimal));
			for (int i = 0; i < dtDoc.Rows.Count; i++)
			{
				for (int j = 3; j < Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "U_PeriodsToForecast")) + 3; j++)
				{
					if (dtDoc.Rows[i][dtDoc.Columns[j]] != null && Function.ParseInt(dtDoc.Rows[i][dtDoc.Columns[j]]) != 0)
					{
						int num = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "Year"));
						int num2 = Function.ParseInt(dtDoc.Rows[i][dtDoc.Columns["LineID"]]);
						DataRow dataRow = dataTable.NewRow();
						dataRow["ItemCode"] = dtDoc.Rows[i]["ItemCode"];
						dataRow["AbsID"] = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "AbsID"));
						dataRow["Quantity"] = Function.ParseInt(dtDoc.Rows[i][dtDoc.Columns[j]]);
						string caption = dtDoc.Columns[j].Caption;
						if (caption == "1")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-01-01");
						}
						if (caption == "2")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-02-01");
						}
						if (caption == "3")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-03-01");
						}
						if (caption == "4")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-04-01");
						}
						if (caption == "5")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-05-01");
						}
						if (caption == "6")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-06-01");
						}
						if (caption == "7")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-07-01");
						}
						if (caption == "8")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-08-01");
						}
						if (caption == "9")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-09-01");
						}
						if (caption == "10")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-10-01");
						}
						if (caption == "11")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-11-01");
						}
						if (caption == "12")
						{
							dataRow["Date"] = Function.ParseDateTimes(num + "-12-01");
						}
						dataRow["LineID"] = num2++;
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
					}
				}
			}
			List<DataRow> list = new List<DataRow>();
			for (int k = 0; k < dataTable.Rows.Count; k++)
			{
				if (dataTable.Rows[k]["ItemCode"] == null || dataTable.Rows[k]["ItemCode"] == DBNull.Value || string.IsNullOrEmpty(dataTable.Rows[k]["ItemCode"].ToString()))
				{
					list.Add(dataTable.Rows[k]);
				}
			}
			foreach (DataRow item in list)
			{
				dataTable.Rows.Remove(item);
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<int> GetMaxAbsID()
	{
		try
		{
			using HttpClientHelper<SalesForeCastImpl> _helper = new HttpClientHelper<SalesForeCastImpl>(Configuration.ApiServer, "SalesForeCast");
			return await _helper.GetIntAsync("GetMaxAbsID") + 1;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async void AddNewLine()
	{
		try
		{
			maxAbsID = await GetMaxAbsID();
			dtDocs.Rows.Clear();
			grd_General.DataSource = null;
			grd_General.RefreshDataSource();
			DataRow dataRow = dtDocs.NewRow();
			dataRow["AbsID"] = maxAbsID;
			object value = (dataRow["Name"] = "");
			dataRow["Code"] = value;
			dataRow["UserSign"] = 0;
			value = (dataRow["EndDate"] = DateTime.Today);
			dataRow["StartDate"] = value;
			dataRow["FormView"] = "";
			value = (dataRow["U_ForecastSource"] = "");
			dataRow["U_ForecastType"] = value;
			dataRow["U_PeriodsToForecast"] = 12;
			object obj4 = (dataRow["U_SeasonRate"] = 0);
			value = (dataRow["U_Seasons"] = obj4);
			dataRow["U_PBWToAvg"] = value;
			dataRow["U_Year"] = 2015;
			dataRow["U_Employee"] = 0;
			object obj7 = (dataRow["U_ApprovedRemark"] = "");
			object obj9 = (dataRow["U_Remark"] = obj7);
			object obj11 = (dataRow["U_Status"] = obj9);
			object obj13 = (dataRow["U_Division"] = obj11);
			obj4 = (dataRow["U_Whse"] = obj13);
			value = (dataRow["U_BPName"] = obj4);
			dataRow["U_BPCode"] = value;
			dtDocs.Rows.Add(dataRow);
			grd_General.DataSource = dtDocs;
			grd_General.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> getDetail(Tuple<int, DateTime, DateTime> tup)
	{
		try
		{
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast"))
			{
				dtDoc = await _helper.PostAsync("GetDetail", tup);
			}
			return dtDoc;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void GetData()
	{
		try
		{
			int num = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "AbsID"));
			if (currentId == -1 || num != currentId)
			{
				currentId = num;
				gv_Salescast.Columns.Clear();
				grd_Salescast.DataSource = null;
				CreatDataGrid();
				int item = Function.ParseInt(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "AbsID"));
				DateTime item2 = Function.ParseDateTimes(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "StartDate"));
				DateTime item3 = Function.ParseDateTimes(gv_General.GetRowCellValue(gv_General.FocusedRowHandle, "EndDate"));
				Tuple<int, DateTime, DateTime> tup = new Tuple<int, DateTime, DateTime>(item, item2, item3);
				DataRow row = (await getDetail(tup)).NewRow();
				dtDoc.Rows.Add(row);
				dtDoc.AcceptChanges();
				grd_Salescast.DataSource = dtDoc;
				grd_Salescast.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable joinTable(DataTable table1, DataTable table2)
	{
		return new DataTable();
	}

	private void gv_General_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Salescast_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			grd_General.DataSource = null;
			grd_General.RefreshDataSource();
			Search();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Search(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesForeCast"))
			{
				DataTable dataSource = await _helper.PostAsync("Search", dt);
				grd_General.DataSource = dataSource;
				grd_General.RefreshDataSource();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_General_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			GetData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_SalesForecast));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_General = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripdoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_General = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.View = new Apzon.Controls.Grid.GridColumnEx();
		this.cbForeCastType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.Repository_ForeCastSource = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.rep_status = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.rep_BPCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rILSalesEmp = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rILStaSalesEmpl = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_BPName = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_Employee = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_Year = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.rep_Whse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_ComboPeriod = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repFormView = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.grd_Salescast = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_SalesForCast = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Salescast = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_itemcode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_itemname = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txtBPName = new Apzon.Controls.Editor.TextEditEx();
		this.btnBPCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btnWareHouse = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbStatus = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.btnDivision = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnQuickAnalysis = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnClosed = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbEmployee = new DevExpress.XtraEditors.SearchLookUpEdit();
		this.searchLookUpEdit1View = new Apzon.Controls.Grid.GridViewEx();
		this.dtYear = new Apzon.Controls.Controls.ComboBoxEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_General).BeginInit();
		this.contextMenuStripdoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_General).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbForeCastType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Repository_ForeCastSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rILStaSalesEmpl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Employee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Year).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ComboPeriod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repFormView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Salescast).BeginInit();
		this.contextMenuStrip_SalesForCast.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Salescast).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_itemcode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_itemname).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnBPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnWareHouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnDivision.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbEmployee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtYear.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 68);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_General);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Salescast);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(960, 384);
		this.splitContainerControl1.SplitterPosition = 153;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_General.AllowPasteExcel = true;
		this.grd_General.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_General.ClipboardData = null;
		this.grd_General.ContextMenuStrip = this.contextMenuStripdoc;
		this.grd_General.Location = new System.Drawing.Point(-3, 0);
		this.grd_General.MainView = this.gv_General;
		this.grd_General.Name = "grd_General";
		this.grd_General.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.cbForeCastType, this.Repository_ForeCastSource, this.rep_status, this.rep_BPCode, this.rILSalesEmp, this.rILStaSalesEmpl, this.rep_BPName, this.rep_Employee, this.rep_Year, this.rep_Whse,
			this.rep_date, this.rep_ComboPeriod, this.repFormView
		});
		this.grd_General.Size = new System.Drawing.Size(960, 153);
		this.grd_General.TabIndex = 0;
		this.grd_General.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_General });
		this.contextMenuStripdoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem, this.duplicateToolStripMenuItem });
		this.contextMenuStripdoc.Name = "contextMenuStripdoc";
		this.contextMenuStripdoc.Size = new System.Drawing.Size(134, 70);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click_1);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteToolStripMenuItem.Text = "Delete Row";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click_1);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gv_General.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_General.ColumnKey = null;
		this.gv_General.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.View });
		this.gv_General.DisableFormSetting = false;
		this.gv_General.DisplayException = false;
		this.gv_General.EditColumn = null;
		this.gv_General.FieldCurrency = null;
		this.gv_General.FieldRate = null;
		this.gv_General.FormId = "oSalesForecast";
		this.gv_General.GridControl = this.grd_General;
		this.gv_General.IgnoreFilter = false;
		this.gv_General.IndexEditRow = -1;
		this.gv_General.IndicatorWidth = 30;
		this.gv_General.IsApzTable = false;
		this.gv_General.IsInsertNewRow = false;
		this.gv_General.IsLoadingData = false;
		this.gv_General.IsMappingContextAction = false;
		this.gv_General.ItemId = "OGENERAL";
		this.gv_General.LevelIndent = 0;
		this.gv_General.LstColumnAllowBatch = null;
		this.gv_General.LstColumnNotAllowBatch = null;
		this.gv_General.LstColumnNotEditable = null;
		this.gv_General.LstColumNotSetting = null;
		this.gv_General.Name = "gv_General";
		this.gv_General.OptionsFind.AllowFindPanel = false;
		this.gv_General.OptionsSelection.MultiSelect = true;
		this.gv_General.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_General.OptionsView.ColumnAutoWidth = false;
		this.gv_General.OptionsView.ShowGroupPanel = false;
		this.gv_General.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_General.UseBatchMod = false;
		this.gv_General.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_General_RowClick);
		this.gv_General.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_General_CustomDrawRowIndicator);
		this.gv_General.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_General_FocusedRowObjectChanged);
		this.gridColumn1.Caption = "Abs ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "AbsID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 1;
		this.gridColumn2.Caption = "Forecast Code";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Code";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn2.Width = 143;
		this.gridColumn3.Caption = "Forecast Name";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Name";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 3;
		this.gridColumn3.Width = 129;
		this.gridColumn4.Caption = "UserSign";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 4;
		this.gridColumn4.Width = 117;
		this.gridColumn5.Caption = "Start Date";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "StartDate";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 5;
		this.gridColumn5.Width = 139;
		this.gridColumn6.Caption = "End Date";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "EndDate";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 6;
		this.gridColumn6.Width = 162;
		this.View.Caption = "View";
		this.View.CurrencyDateField = null;
		this.View.CurrencyField = null;
		this.View.CurrencyFixed = null;
		this.View.CurrencyFixedRate = 0.0;
		this.View.CurrencyRateField = null;
		this.View.FieldName = "FormView";
		this.View.Name = "View";
		this.View.PrimaryKey = false;
		this.View.UnitField = null;
		this.View.Visible = true;
		this.View.VisibleIndex = 7;
		this.View.Width = 94;
		this.cbForeCastType.AutoHeight = false;
		this.cbForeCastType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbForeCastType.Items.AddRange(new object[2] { "Sales", "Production" });
		this.cbForeCastType.Name = "cbForeCastType";
		this.Repository_ForeCastSource.AutoHeight = false;
		this.Repository_ForeCastSource.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.Repository_ForeCastSource.Items.AddRange(new object[3] { "Orders", "Delivery", "Invoices" });
		this.Repository_ForeCastSource.Name = "Repository_ForeCastSource";
		this.rep_status.AutoHeight = false;
		this.rep_status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_status.Items.AddRange(new object[3] { "Request", "Approved", "Draft" });
		this.rep_status.Name = "rep_status";
		this.rep_BPCode.AutoHeight = false;
		this.rep_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BPCode.Name = "rep_BPCode";
		this.rep_BPCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BPCode_ButtonClick);
		this.rep_BPCode.Click += new System.EventHandler(this.rep_BPCode_Click);
		this.rILSalesEmp.AutoHeight = false;
		this.rILSalesEmp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILSalesEmp.DisplayMember = "SlpName";
		this.rILSalesEmp.Name = "rILSalesEmp";
		this.rILSalesEmp.ValueMember = "SlpCode";
		this.rILStaSalesEmpl.AutoHeight = false;
		this.rILStaSalesEmpl.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rILStaSalesEmpl.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name")
		});
		this.rILStaSalesEmpl.DisplayMember = "SlpName";
		this.rILStaSalesEmpl.Name = "rILStaSalesEmpl";
		this.rILStaSalesEmpl.NullText = "";
		this.rILStaSalesEmpl.ValueMember = "SlpCode";
		this.rILStaSalesEmpl.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rILStaSalesEmpl_ButtonClick);
		this.rep_BPName.AutoHeight = false;
		this.rep_BPName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BPName.Name = "rep_BPName";
		this.rep_BPName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BPName_ButtonClick);
		this.rep_Employee.AutoHeight = false;
		this.rep_Employee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Employee.Name = "rep_Employee";
		this.rep_Year.AutoHeight = false;
		this.rep_Year.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Year.Items.AddRange(new object[36]
		{
			"1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999",
			"2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009",
			"2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019",
			"2020", "2021", "2022", "2023", "2024", "2025"
		});
		this.rep_Year.Name = "rep_Year";
		this.rep_Whse.AutoHeight = false;
		this.rep_Whse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinLeft)
		});
		this.rep_Whse.Name = "rep_Whse";
		this.rep_Whse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Whse_ButtonClick);
		this.rep_date.AutoHeight = false;
		this.rep_date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date.Name = "rep_date";
		this.rep_ComboPeriod.AutoHeight = false;
		this.rep_ComboPeriod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ComboPeriod.Items.AddRange(new object[2] { "6", "12" });
		this.rep_ComboPeriod.Name = "rep_ComboPeriod";
		this.rep_ComboPeriod.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.repFormView.AutoHeight = false;
		this.repFormView.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repFormView.Items.AddRange(new object[3] { "Daily", "Weekly", "Monthly" });
		this.repFormView.Name = "repFormView";
		this.grd_Salescast.AllowPasteExcel = true;
		this.grd_Salescast.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Salescast.ClipboardData = null;
		this.grd_Salescast.ContextMenuStrip = this.contextMenuStrip_SalesForCast;
		this.grd_Salescast.Location = new System.Drawing.Point(0, 0);
		this.grd_Salescast.MainView = this.gv_Salescast;
		this.grd_Salescast.Name = "grd_Salescast";
		this.grd_Salescast.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_itemcode, this.rep_itemname });
		this.grd_Salescast.Size = new System.Drawing.Size(960, 226);
		this.grd_Salescast.TabIndex = 1;
		this.grd_Salescast.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Salescast });
		this.contextMenuStrip_SalesForCast.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip_SalesForCast.Name = "contextMenuStrip1";
		this.contextMenuStrip_SalesForCast.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Salescast.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Salescast.ColumnKey = null;
		this.gv_Salescast.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10, this.gridColumn11 });
		this.gv_Salescast.DisableFormSetting = false;
		this.gv_Salescast.DisplayException = false;
		this.gv_Salescast.EditColumn = null;
		this.gv_Salescast.FieldCurrency = null;
		this.gv_Salescast.FieldRate = null;
		this.gv_Salescast.FormId = "oSalesForecast";
		this.gv_Salescast.GridControl = this.grd_Salescast;
		this.gv_Salescast.IgnoreFilter = false;
		this.gv_Salescast.IndexEditRow = -1;
		this.gv_Salescast.IndicatorWidth = 30;
		this.gv_Salescast.IsApzTable = false;
		this.gv_Salescast.IsInsertNewRow = false;
		this.gv_Salescast.IsLoadingData = false;
		this.gv_Salescast.IsMappingContextAction = false;
		this.gv_Salescast.ItemId = "OSFC";
		this.gv_Salescast.LstColumnAllowBatch = null;
		this.gv_Salescast.LstColumnNotAllowBatch = null;
		this.gv_Salescast.LstColumnNotEditable = null;
		this.gv_Salescast.LstColumNotSetting = null;
		this.gv_Salescast.Name = "gv_Salescast";
		this.gv_Salescast.OptionsView.ColumnAutoWidth = false;
		this.gv_Salescast.OptionsView.ShowGroupPanel = false;
		this.gv_Salescast.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Salescast.UseBatchMod = false;
		this.gv_Salescast.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Salescast_CustomDrawRowIndicator);
		this.gridColumn7.Caption = "AbsID";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "AbsID";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = true;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 0;
		this.gridColumn7.Width = 85;
		this.gridColumn8.Caption = "Line ID";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "LineID";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 1;
		this.gridColumn8.Width = 112;
		this.gridColumn9.Caption = "ItemCode";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "ItemCode";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 2;
		this.gridColumn9.Width = 156;
		this.gridColumn10.Caption = "Date";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Date";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 3;
		this.gridColumn10.Width = 122;
		this.gridColumn11.Caption = "Quantity";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Quantity";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 4;
		this.gridColumn11.Width = 191;
		this.rep_itemcode.AutoHeight = false;
		this.rep_itemcode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_itemcode.Name = "rep_itemcode";
		this.rep_itemcode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_itemcode_ButtonClick);
		this.rep_itemname.AutoHeight = false;
		this.rep_itemname.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_itemname.Name = "rep_itemname";
		this.rep_itemname.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_itemname_ButtonClick);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(6, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(58, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 10;
		this.labelLine1.Utext = "BP Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Year";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(582, 15);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(57, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 12;
		this.labelLine3.Utext = "Year";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "WareHouse";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(377, 15);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(65, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 13;
		this.labelLine4.Utext = "WareHouse";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Type";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(179, 14);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(72, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 14;
		this.labelLine5.Utext = "Type";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Employee";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(179, 40);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(72, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 22;
		this.labelLine2.Utext = "Employee";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Division";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(377, 40);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(65, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 21;
		this.labelLine6.Utext = "Division";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Status";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(582, 40);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(57, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 20;
		this.labelLine7.Utext = "Status";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "BP Name";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(6, 39);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(58, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 19;
		this.labelLine8.Utext = "BP Name";
		this.txtBPName.EditControl = null;
		this.txtBPName.FieldName = null;
		this.txtBPName.IsSearchControl = false;
		this.txtBPName.Location = new System.Drawing.Point(63, 38);
		this.txtBPName.Name = "txtBPName";
		this.txtBPName.SettingInVisible = false;
		this.txtBPName.SettingReadonly = false;
		this.txtBPName.Size = new System.Drawing.Size(110, 20);
		this.txtBPName.TabIndex = 15;
		this.btnBPCode.EditControl = null;
		this.btnBPCode.FieldName = null;
		this.btnBPCode.IsSearchControl = false;
		this.btnBPCode.Location = new System.Drawing.Point(63, 14);
		this.btnBPCode.Name = "btnBPCode";
		this.btnBPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnBPCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.textEdit1_Properties_ButtonPressed);
		this.btnBPCode.SettingInVisible = false;
		this.btnBPCode.SettingReadonly = false;
		this.btnBPCode.Size = new System.Drawing.Size(110, 20);
		this.btnBPCode.TabIndex = 2;
		this.btnBPCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnBPCode_ButtonClick);
		this.btnBPCode.EditValueChanged += new System.EventHandler(this.btnBPCode_EditValueChanged);
		this.cbType.EditControl = null;
		this.cbType.FieldName = null;
		this.cbType.IsSearchControl = false;
		this.cbType.Location = new System.Drawing.Point(251, 14);
		this.cbType.Name = "cbType";
		this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbType.Properties.ImmediatePopup = true;
		this.cbType.Properties.Items.AddRange(new object[3] { "Production", "Sales", "     " });
		this.cbType.SettingInVisible = false;
		this.cbType.SettingReadonly = false;
		this.cbType.Size = new System.Drawing.Size(111, 20);
		this.cbType.TabIndex = 8;
		this.btnWareHouse.EditControl = null;
		this.btnWareHouse.FieldName = null;
		this.btnWareHouse.IsSearchControl = false;
		this.btnWareHouse.Location = new System.Drawing.Point(442, 14);
		this.btnWareHouse.Name = "btnWareHouse";
		this.btnWareHouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnWareHouse.SettingInVisible = false;
		this.btnWareHouse.SettingReadonly = false;
		this.btnWareHouse.Size = new System.Drawing.Size(128, 20);
		this.btnWareHouse.TabIndex = 9;
		this.btnWareHouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnWareHouse_ButtonClick);
		this.btnWareHouse.EditValueChanged += new System.EventHandler(this.btnWareHouse_EditValueChanged);
		this.btnWareHouse.Click += new System.EventHandler(this.btnWareHouse_Click);
		this.cbStatus.EditControl = null;
		this.cbStatus.FieldName = null;
		this.cbStatus.IsSearchControl = false;
		this.cbStatus.Location = new System.Drawing.Point(640, 38);
		this.cbStatus.Name = "cbStatus";
		this.cbStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStatus.Properties.Items.AddRange(new object[3] { "Draft", "Request", "Approved" });
		this.cbStatus.SettingInVisible = false;
		this.cbStatus.SettingReadonly = false;
		this.cbStatus.Size = new System.Drawing.Size(121, 20);
		this.cbStatus.TabIndex = 16;
		this.btnDivision.EditControl = null;
		this.btnDivision.FieldName = null;
		this.btnDivision.IsSearchControl = false;
		this.btnDivision.Location = new System.Drawing.Point(442, 38);
		this.btnDivision.Name = "btnDivision";
		this.btnDivision.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnDivision.SettingInVisible = false;
		this.btnDivision.SettingReadonly = false;
		this.btnDivision.Size = new System.Drawing.Size(128, 20);
		this.btnDivision.TabIndex = 18;
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.Appearance.BackColor = System.Drawing.Color.White;
		this.btnSave.Appearance.Options.UseBackColor = true;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Save";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(3, 458);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 23;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnQuickAnalysis.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnQuickAnalysis.EditControl = null;
		this.btnQuickAnalysis.EditValue = "Quick Analysis";
		this.btnQuickAnalysis.FieldName = null;
		this.btnQuickAnalysis.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnQuickAnalysis.ImageOptions.Image");
		this.btnQuickAnalysis.IsSearchControl = false;
		this.btnQuickAnalysis.Location = new System.Drawing.Point(159, 458);
		this.btnQuickAnalysis.Name = "btnQuickAnalysis";
		this.btnQuickAnalysis.ReadOnly = false;
		this.btnQuickAnalysis.SettingInVisible = false;
		this.btnQuickAnalysis.SettingReadonly = false;
		this.btnQuickAnalysis.Size = new System.Drawing.Size(105, 23);
		this.btnQuickAnalysis.TabIndex = 24;
		this.btnQuickAnalysis.Text = "Quick Analysis";
		this.btnQuickAnalysis.Click += new System.EventHandler(this.btnQuickAnalysis_Click);
		this.btnClosed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnClosed.EditControl = null;
		this.btnClosed.EditValue = "Closed";
		this.btnClosed.FieldName = null;
		this.btnClosed.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnClosed.ImageOptions.Image");
		this.btnClosed.IsSearchControl = false;
		this.btnClosed.Location = new System.Drawing.Point(81, 458);
		this.btnClosed.Name = "btnClosed";
		this.btnClosed.ReadOnly = false;
		this.btnClosed.SettingInVisible = false;
		this.btnClosed.SettingReadonly = false;
		this.btnClosed.Size = new System.Drawing.Size(75, 23);
		this.btnClosed.TabIndex = 25;
		this.btnClosed.Text = "Closed";
		this.btnClosed.Click += new System.EventHandler(this.btnClosed_Click);
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Generating by Average Method";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(784, 458);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(176, 23);
		this.simpleButton1.TabIndex = 26;
		this.simpleButton1.Text = "Generating by Average Method";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(770, 14);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(72, 20);
		this.btnSearch.TabIndex = 27;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.cbEmployee.Location = new System.Drawing.Point(251, 38);
		this.cbEmployee.Name = "cbEmployee";
		this.cbEmployee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbEmployee.Properties.DisplayMember = "SlpName";
		this.cbEmployee.Properties.NullText = "";
		this.cbEmployee.Properties.PopupSizeable = false;
		this.cbEmployee.Properties.PopupView = this.searchLookUpEdit1View;
		this.cbEmployee.Properties.ValueMember = "SlpCode";
		this.cbEmployee.Size = new System.Drawing.Size(111, 20);
		this.cbEmployee.TabIndex = 17;
		this.searchLookUpEdit1View.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.searchLookUpEdit1View.ColumnKey = null;
		this.searchLookUpEdit1View.DisableFormSetting = false;
		this.searchLookUpEdit1View.DisplayException = false;
		this.searchLookUpEdit1View.EditColumn = null;
		this.searchLookUpEdit1View.FieldCurrency = null;
		this.searchLookUpEdit1View.FieldRate = null;
		this.searchLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.searchLookUpEdit1View.FormId = "oSalesForecast";
		this.searchLookUpEdit1View.IgnoreFilter = false;
		this.searchLookUpEdit1View.IndexEditRow = -1;
		this.searchLookUpEdit1View.IndicatorWidth = 35;
		this.searchLookUpEdit1View.IsApzTable = false;
		this.searchLookUpEdit1View.IsInsertNewRow = false;
		this.searchLookUpEdit1View.IsLoadingData = false;
		this.searchLookUpEdit1View.IsMappingContextAction = false;
		this.searchLookUpEdit1View.ItemId = "OSEARCHDATA";
		this.searchLookUpEdit1View.LstColumnAllowBatch = null;
		this.searchLookUpEdit1View.LstColumnNotAllowBatch = null;
		this.searchLookUpEdit1View.LstColumnNotEditable = null;
		this.searchLookUpEdit1View.LstColumNotSetting = null;
		this.searchLookUpEdit1View.Name = "searchLookUpEdit1View";
		this.searchLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.searchLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.searchLookUpEdit1View.UseBatchMod = false;
		this.dtYear.EditControl = null;
		this.dtYear.FieldName = null;
		this.dtYear.IsSearchControl = false;
		this.dtYear.Location = new System.Drawing.Point(640, 14);
		this.dtYear.Name = "dtYear";
		this.dtYear.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtYear.Properties.Items.AddRange(new object[36]
		{
			"1990", "1991", "1992", "1993", "1994", "1995", "1996", "1997", "1998", "1999",
			"2000", "2001", "2002", "2003", "2004", "2005", "2006", "2007", "2008", "2009",
			"2010", "2011", "2012", "2013", "2014", "2015", "2016", "2017", "2018", "2019",
			"2020", "2021", "2022", "2023", "2024", "2025"
		});
		this.dtYear.SettingInVisible = false;
		this.dtYear.SettingReadonly = false;
		this.dtYear.Size = new System.Drawing.Size(121, 20);
		this.dtYear.TabIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(966, 482);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.btnClosed);
		base.Controls.Add(this.btnQuickAnalysis);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.txtBPName);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btnBPCode);
		base.Controls.Add(this.cbType);
		base.Controls.Add(this.btnWareHouse);
		base.Controls.Add(this.cbStatus);
		base.Controls.Add(this.btnDivision);
		base.Controls.Add(this.cbEmployee);
		base.Controls.Add(this.dtYear);
		base.FormId = "oSalesForecast";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SalesForecast";
		base.ShowInTaskbar = false;
		this.Text = "Sales Forecast";
		base.Load += new System.EventHandler(this.frm_SalesForecast_Load);
		base.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.frm_SalesForecast_KeyPress);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_General).EndInit();
		this.contextMenuStripdoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_General).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbForeCastType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Repository_ForeCastSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILSalesEmp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rILStaSalesEmpl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Employee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Year).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Whse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ComboPeriod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repFormView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Salescast).EndInit();
		this.contextMenuStrip_SalesForCast.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Salescast).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_itemcode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_itemname).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnBPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnWareHouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnDivision.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbEmployee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.searchLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtYear.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
