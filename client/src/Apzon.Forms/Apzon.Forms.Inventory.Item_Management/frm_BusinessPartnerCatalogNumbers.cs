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
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Inventory.Item_Management;

public class frm_BusinessPartnerCatalogNumbers : XtraFormEx
{
	private bool _isLeave = true;

	private const string Controller = "BusinessPartnerCatalogNumbers";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private XtraTabControlEx xtraTabBPCN;

	private XtraTabPageEx TabBP;

	private XtraTabPageEx TabItems;

	private CheckEditEx BPPrintBP;

	private GridControlEx grdBP;

	private LabelLine lb_BPName;

	private LabelLine lb_BPCode;

	private CheckEditEx ItemsPrintBP;

	private GridControlEx grdItems;

	private LabelLine lb_ItemsName;

	private LabelLine lb_ItemsCode;

	private ButtonEditEx btnItemCode;

	private GridColumnEx ItemCode;

	private GridColumnEx ItemName;

	private GridColumnEx Substitute;

	private RepositoryItemButtonEdit rep_ItemCode;

	private RepositoryItemButtonEdit rep_ItemName;

	private GridColumnEx CardCode;

	private GridColumnEx CardName;

	private GridColumnEx Substitute2;

	private RepositoryItemButtonEdit rep_CardCode;

	private RepositoryItemButtonEdit rep_CardName;

	private ButtonEditEx btnBPCode;

	private GridViewEx gvBP;

	private GridViewEx gvItems;

	private ContextMenuStrip ContexMenuDoc;

	private ToolStripMenuItem RemoveToolStripMenuItem;

	private ContextMenuStrip contextMenu;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private SimpleButtonEx btnAdd;

	private SimpleButtonEx btnCancel;

	private TextEditEx txt_BpName;

	private TextEditEx txt_ItemName;

	private RepositoryItemTextEdit txt_SubTitile;

	private ToolStripMenuItem removeRow_ToolStripMenuItem;

	public frm_BusinessPartnerCatalogNumbers()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BusinessPartnerCatalogNumbers"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BusinessPartnerCatalogNumbers" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			FormId = "TABBP";
			gvBP.FormId = FormId;
			gvBP.ItemId = "OSCN";
			gvItems.FormId = FormId;
			gvItems.ItemId = "OSCNITM";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx5 = btnAdd;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx5.Enabled = enabled;
				btnAdd.Text = Resources.ButtonAdd;
				ButtonEditEx buttonEditEx3 = btnBPCode;
				enabled = (btnItemCode.Enabled = true);
				buttonEditEx3.Enabled = enabled;
				GridViewEx gridViewEx4 = gvBP;
				enabled = (gvItems.ReadOnly = false);
				gridViewEx4.ReadOnly = enabled;
				removeRow_ToolStripMenuItem.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx4 = btnAdd;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btnAdd.Text = Resources.ButtonUpdate;
				removeRow_ToolStripMenuItem.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				GridViewEx gridViewEx3 = gvBP;
				bool enabled = (gvItems.ReadOnly = false);
				gridViewEx3.ReadOnly = enabled;
				SimpleButtonEx simpleButtonEx3 = btnAdd;
				enabled = (btnCancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				removeRow_ToolStripMenuItem.Enabled = true;
				btnAdd.Text = Resources.ButtonOK;
				ButtonEditEx buttonEditEx2 = btnBPCode;
				enabled = (btnItemCode.Enabled = false);
				buttonEditEx2.Enabled = enabled;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx2 = btnAdd;
				bool enabled = (btnCancel.Enabled = false);
				simpleButtonEx2.Enabled = enabled;
				removeRow_ToolStripMenuItem.Enabled = false;
				GridViewEx gridViewEx2 = gvBP;
				enabled = (gvItems.ReadOnly = true);
				gridViewEx2.ReadOnly = enabled;
				break;
			}
			case Enums.FormViewModeType.Find:
			{
				SimpleButtonEx simpleButtonEx = btnAdd;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx.Enabled = enabled;
				btnAdd.Text = Resources.ButtonFind;
				ButtonEditEx buttonEditEx = btnBPCode;
				enabled = (btnItemCode.Enabled = true);
				buttonEditEx.Enabled = enabled;
				GridViewEx gridViewEx = gvBP;
				enabled = (gvItems.ReadOnly = true);
				gridViewEx.ReadOnly = enabled;
				removeRow_ToolStripMenuItem.Enabled = false;
				ClearControl(xtraTabBPCN.SelectedTabPage.Name);
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		EnableDisableControl(enable: false);
		try
		{
			GetData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	public async Task MOVE_NEXT()
	{
		EnableDisableControl(enable: false);
		try
		{
			GetData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	public async Task MOVE_FIRST()
	{
		EnableDisableControl(enable: false);
		try
		{
			GetData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	public async Task MOVE_LAST()
	{
		EnableDisableControl(enable: false);
		try
		{
			GetData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	public async Task MODE_FIND()
	{
		try
		{
			EnableDisableControlFind(enable: false);
			FormMode = Enums.FormViewModeType.Find;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "MODE_FIND  ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No))
			{
				ClearControl(xtraTabBPCN.SelectedTabPage.Name);
				AddNewRow(xtraTabBPCN.SelectedTabPage.Name);
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "REFRESH ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ClearControl(string name)
	{
		try
		{
			if (!(name == "TabBP"))
			{
				if (name == "TabItems")
				{
					btnItemCode.EditValue = "";
					btnItemCode.Focus();
					txt_ItemName.Text = "";
					grdItems.DataSourceTable.Rows.Clear();
					grdItems.RefreshDataSource();
				}
				else
				{
					btnBPCode.EditValue = "";
					txt_BpName.Text = "";
					grdBP.DataSourceTable.Rows.Clear();
					grdBP.RefreshDataSource();
					btnItemCode.EditValue = "";
					txt_ItemName.Text = "";
					grdItems.DataSourceTable.Rows.Clear();
					grdItems.RefreshDataSource();
				}
			}
			else
			{
				btnBPCode.EditValue = "";
				btnBPCode.Focus();
				txt_BpName.Text = "";
				grdBP.DataSourceTable.Rows.Clear();
				grdBP.RefreshDataSource();
			}
			BPPrintBP.Checked = false;
			ItemsPrintBP.Checked = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void EnableDisableControlFind(bool enable)
	{
		try
		{
			txt_BpName.Enabled = enable;
			txt_ItemName.Enabled = enable;
			btnBPCode.Properties.Buttons[0].Visible = enable;
			btnItemCode.Properties.Buttons[0].Visible = enable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void AddNewRow(string tabName)
	{
		try
		{
			if (!(tabName == "TabBP"))
			{
				if (tabName == "TabItems")
				{
					if (grdItems.DataSourceTable != null && grdItems.DataSourceTable.Columns.Count > 0)
					{
						DataTable dataSourceTable = grdItems.DataSourceTable;
						DataRow row = dataSourceTable.NewRow();
						dataSourceTable.Rows.Add(row);
						grdItems.DataSource = dataSourceTable;
						grdItems.RefreshDataSource();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
					}
				}
			}
			else if (grdBP.DataSourceTable != null && grdBP.DataSourceTable.Columns.Count > 0)
			{
				DataTable dataSourceTable2 = grdBP.DataSourceTable;
				DataRow row2 = dataSourceTable2.NewRow();
				dataSourceTable2.Rows.Add(row2);
				grdBP.DataSource = dataSourceTable2;
				grdBP.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task GetItemByCardCode(string cardcode)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BusinessPartnerCatalogNumbers");
			DataTable dataTable = await http.GetAsync("GetItemByCardCode", new { cardcode });
			grdBP.DataSource = dataTable;
			grdBP.RefreshDataSource();
			if (dataTable == null || dataTable.Count() == 0)
			{
				BPPrintBP.Checked = false;
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
				BPPrintBP.Checked = Function.ToString(dataTable.Rows[0]["ShowSCN"]) == "Y";
			}
			AddNewRow(TabBP.Name);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "REFRESH ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetBpByItemCode(string itemcode)
	{
		DataTable bpByItemCode = _httpClient.BusinessPartnerCatalogNumbersClient.GetBpByItemCode(itemcode);
		grdItems.DataSource = bpByItemCode;
		grdItems.RefreshDataSource();
		if (bpByItemCode == null || bpByItemCode.Count() == 0)
		{
			ItemsPrintBP.Checked = false;
			FormMode = Enums.FormViewModeType.Add;
		}
		else
		{
			FormMode = Enums.FormViewModeType.View;
			ItemsPrintBP.Checked = Function.ToString(bpByItemCode.Rows[0]["ShowSCN"]) == "Y";
		}
		AddNewRow(TabItems.Name);
	}

	private void Remove(string paramateCode, string subtitle, string item, string type)
	{
		try
		{
			string text = _httpClient.BusinessPartnerCatalogNumbersClient.Remove(new Tuple<string, string, string, string>(paramateCode, subtitle, item, type));
			if (string.IsNullOrEmpty(text))
			{
				ClearControl(xtraTabBPCN.SelectedTabPage.Name);
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (xtraTabBPCN.SelectedTabPage.Name == TabBP.Name)
				{
					gvBP.DeleteDataSourceRowSelected();
					grdBP.DataSourceTable.AcceptChanges();
				}
				else
				{
					gvItems.DeleteDataSourceRowSelected();
					grdItems.DataSourceTable.AcceptChanges();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void EnableDisableControl(bool enable)
	{
		try
		{
			btnAdd.Enabled = enable;
			btnCancel.Enabled = enable;
			ContexMenuDoc.Enabled = enable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void GetData(Enums.NavigateType type)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			if (xtraTabBPCN.SelectedTabPage.Name == TabBP.Name)
			{
				DataTable data = _httpClient.BusinessPartnerCatalogNumbersClient.GetData(type, Enums.BusinessPartnerCatalogNumbersTableType.TabBP, btnBPCode.Text);
				if (data != null && data.Rows.Count > 0)
				{
					btnBPCode.Text = Function.ToString(data.Rows[0]["CardCode"]);
					txt_BpName.Text = Function.ToString(data.Rows[0]["CardName"]);
					grdBP.DataSource = data;
					grdBP.RefreshDataSource();
					AddNewRow(TabBP.Name);
				}
			}
			else
			{
				DataTable data = _httpClient.BusinessPartnerCatalogNumbersClient.GetData(type, Enums.BusinessPartnerCatalogNumbersTableType.TabItem, btnItemCode.Text);
				if (data != null && data.Rows.Count > 0)
				{
					btnItemCode.Text = Function.ToString(data.Rows[0]["ItemCode"]);
					txt_ItemName.Text = Function.ToString(data.Rows[0]["ItemName"]);
					grdItems.DataSource = data;
					grdItems.RefreshDataSource();
					AddNewRow(TabItems.Name);
				}
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string GetNameKey(string code, Enums.BusinessPartnerCatalogNumbersTableType type)
	{
		try
		{
			return _httpClient.BusinessPartnerCatalogNumbersClient.GetNameKey(code, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	private async Task<string> GetCardNameByCode(string cardCode)
	{
		return await _httpClient.BusinessPartnerCatalogNumbersClient.GetCardNameByCode(cardCode);
	}

	private async Task<string> GetItemNameByCode(string itemCode)
	{
		return await _httpClient.BusinessPartnerCatalogNumbersClient.GetItemNameByCode(itemCode);
	}

	private void Find()
	{
		string text = ((xtraTabBPCN.SelectedTabPage == TabBP) ? Function.ToString(btnBPCode.EditValue) : Function.ToString(btnItemCode.EditValue));
		GridControlEx gridControlEx = ((xtraTabBPCN.SelectedTabPage == TabBP) ? grdBP : grdItems);
		DataTable dataTable = ((xtraTabBPCN.SelectedTabPage == TabBP) ? _httpClient.BusinessPartnerCatalogNumbersClient.GetItemByCardCode(text) : _httpClient.BusinessPartnerCatalogNumbersClient.GetBpByItemCode(text));
		if (dataTable.IsNotNull())
		{
			gridControlEx.DataSource = dataTable;
			gridControlEx.RefreshDataSource();
			AddNewRow(xtraTabBPCN.SelectedTabPage.Name);
			FormMode = Enums.FormViewModeType.View;
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
		}
	}

	private new void Validate()
	{
	}

	private new async Task Update()
	{
		DataTable dt = ((xtraTabBPCN.SelectedTabPage == TabBP) ? grdBP : grdItems).DataSourceTable.Copy();
		HttpResult httpResult = await _httpClient.BusinessPartnerCatalogNumbersClient.SaveBp(dt);
		if (string.IsNullOrEmpty(httpResult.Message))
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
			FormMode = Enums.FormViewModeType.View;
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
	}

	private async void btnAdd_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
			return;
		}
		EnableDisableControl(enable: false);
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Find:
				Find();
				break;
			case Enums.FormViewModeType.Update:
				await Update();
				break;
			case Enums.FormViewModeType.Add:
				await Update();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void gvBP_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvItems_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) == DialogResult.OK)
		{
			Close();
		}
	}

	private async Task frm_BusinessPartnerCatalogNumbers_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			Task task = WinFuntions.LoadUdfColumns(gvBP, "OSCN");
			Task T5 = WinFuntions.LoadUdfColumns(gvItems, "OSCN");
			await task;
			await T5;
			grdBP.DataSource = _httpClient.BusinessPartnerCatalogNumbersClient.GetItemByCardCode("");
			grdItems.DataSource = _httpClient.BusinessPartnerCatalogNumbersClient.GetBpByItemCode("");
			AddNewRow(xtraTabBPCN.SelectedTabPage.Name);
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void btnItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		WinCommons._OpenProcessing();
		EnableDisableControl(enable: false);
		try
		{
			ShowChooseFromListItems();
			AddNewRow(TabItems.Name);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
		WinCommons._CloseProcessing();
	}

	private void ShowChooseFromListItems()
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, Function.ToString(btnItemCode.Text), "", Enums.ItemObjectType.NoItemType, null, "");
		frm_CFL2.ListMandatoryField = "ItemName";
		frm_CFL2.ShowDialog(this);
		if (frm_CFL2.ChooseItem != null)
		{
			btnItemCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["ItemCode"]);
			txt_ItemName.Text = Function.ToString(frm_CFL2.ChooseItem["ItemName"]);
			DataTable bpByItemCode = _httpClient.BusinessPartnerCatalogNumbersClient.GetBpByItemCode(Function.ToString(frm_CFL2.ChooseItem["ItemCode"]));
			grdItems.DataSource = bpByItemCode;
			grdItems.RefreshDataSource();
			if (bpByItemCode.IsNotNull())
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
	}

	private async void btnItemCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void rep_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ChooseItems();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseItems()
	{
		EnableDisableControl(enable: false);
		try
		{
			if (string.IsNullOrEmpty(btnBPCode.Text))
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoEnterBpBeforeEnterItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
				EnableDisableControl(enable: true);
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, "", null, Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ItemName";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
			{
				int focusedRowHandle = gvBP.FocusedRowHandle;
				if (!string.IsNullOrEmpty(Function.ToString(gvBP.GetFocusedRowCellValue("ItemCode"))))
				{
					gvBP.DeleteRow(focusedRowHandle);
					grdBP.DataSourceTable.AcceptChanges();
				}
				DataTable dataTable = grdBP.DataSourceTable.Copy();
				for (int num = frm_CFL2.ChooseItems.Count - 1; num >= 0; num--)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["CardCode"] = btnBPCode.Text;
					dataRow["ItemCode"] = frm_CFL2.ChooseItems[num]["ItemCode"];
					dataRow["ItemName"] = frm_CFL2.ChooseItems[num]["ItemName"];
					dataRow["Substitute"] = "";
					dataRow["ShowSCN"] = (BPPrintBP.Checked ? "Y" : "N");
					dataTable.Rows.InsertAt(dataRow, focusedRowHandle);
				}
				grdBP.DataSource = dataTable;
				grdBP.RefreshDataSource();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void rep_ItemName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		ChooseItems();
	}

	private void rep_CardCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			ShowChooseFromListBp(isControl: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void rep_CardName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			ShowChooseFromListBp(isControl: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void btn_BPCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ShowChooseFromListBp(isControl: true);
			AddNewRow(TabBP.Name);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowChooseFromListBp(bool isControl)
	{
		WinCommons._OpenProcessing();
		EnableDisableControl(enable: false);
		try
		{
			if (!isControl && string.IsNullOrEmpty(btnItemCode.Text))
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.InfoEnterItemBeforeEnterBp, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
				EnableDisableControl(enable: true);
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, !isControl, isControl ? Function.ToString(btnBPCode.Text) : string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (!isControl)
			{
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					int focusedRowHandle = gvItems.FocusedRowHandle;
					if (!string.IsNullOrEmpty(Function.ToString(gvItems.GetFocusedRowCellValue("CardCode"))))
					{
						gvItems.DeleteRow(focusedRowHandle);
						grdItems.DataSourceTable.AcceptChanges();
					}
					DataTable dataTable = grdItems.DataSourceTable.Copy();
					for (int num = frm_CFL2.ChooseItems.Count - 1; num >= 0; num--)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["ItemCode"] = btnItemCode.Text;
						dataRow["CardCode"] = frm_CFL2.ChooseItems[num]["CardCode"];
						dataRow["CardName"] = frm_CFL2.ChooseItems[num]["CardName"];
						dataRow["Substitute"] = "";
						dataRow["ShowSCN"] = (ItemsPrintBP.Checked ? "Y" : "N");
						dataTable.Rows.InsertAt(dataRow, focusedRowHandle);
					}
					grdItems.DataSource = dataTable;
					grdItems.RefreshDataSource();
					if (FormMode == Enums.FormViewModeType.View)
					{
						FormMode = Enums.FormViewModeType.Update;
					}
				}
			}
			else
			{
				if (frm_CFL2.ChooseItem != null)
				{
					btnBPCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
					txt_BpName.Text = Function.ToString(frm_CFL2.ChooseItem["CardName"]);
					DataTable itemByCardCode = _httpClient.BusinessPartnerCatalogNumbersClient.GetItemByCardCode(Function.ToString(frm_CFL2.ChooseItem["CardCode"]));
					grdBP.DataSource = itemByCardCode;
					grdBP.RefreshDataSource();
					if (itemByCardCode.IsNotNull())
					{
						FormMode = Enums.FormViewModeType.View;
					}
				}
				_isLeave = false;
				_isLeave = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			WinCommons._CloseProcessing();
		}
		EnableDisableControl(enable: true);
		WinCommons._CloseProcessing();
	}

	private async void btnBPCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void xtraTabBPCN_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != Enums.FormViewModeType.Find)
			{
				GridControlEx gridControlEx = ((xtraTabBPCN.SelectedTabPage == TabBP) ? grdBP : grdItems);
				DataTable dataTable = gridControlEx.DataSource as DataTable;
				bool flag = false;
				if (dataTable.IsNotNull())
				{
					if (gridControlEx == grdBP)
					{
						foreach (DataRow row in dataTable.Rows)
						{
							if (!string.IsNullOrWhiteSpace(Function.ToString(row["ItemCode"])) || !string.IsNullOrWhiteSpace(Function.ToString(row["ItemName"])) || !string.IsNullOrWhiteSpace(Function.ToString(row["Substitute"])))
							{
								flag = true;
							}
						}
					}
					else
					{
						foreach (DataRow row2 in dataTable.Rows)
						{
							if (!string.IsNullOrWhiteSpace(Function.ToString(row2["CardCode"])) || !string.IsNullOrWhiteSpace(Function.ToString(row2["CardName"])) || !string.IsNullOrWhiteSpace(Function.ToString(row2["Substitute"])))
							{
								flag = true;
							}
						}
					}
					if (flag && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
					{
						e.Cancel = true;
						EnableDisableControl(enable: true);
						return;
					}
				}
			}
			ClearControl(e.Page.Name);
			AddNewRow(e.Page.Name);
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "xtraTabBPCN_Selected  ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void gvBP_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (e.Column.FieldName == ItemCode.FieldName)
			{
				gvBP.SetFocusedRowCellValue("ItemName", GetNameKey(Function.ToString(e.Value), Enums.BusinessPartnerCatalogNumbersTableType.TabBP));
				gvBP.GetFocusedDataRow()["CardCode"] = btnBPCode.Text;
				gvBP.GetFocusedDataRow()["CardName"] = txt_BpName.Text;
				gvBP.GetFocusedDataRow()["ShowSCN"] = (BPPrintBP.Checked ? "Y" : "N");
				grdBP.DataSourceTable.AcceptChanges();
			}
			if (e.Column.FieldName == "Substitute" && string.IsNullOrEmpty(Function.ToString(gvBP.GetFocusedRowCellValue("ItemCode"))))
			{
				EnableDisableControl(enable: true);
				return;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "gvBP_CellValueChanged  ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void gvItems_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (e.Column.FieldName == CardCode.FieldName)
			{
				gvItems.SetFocusedRowCellValue("CardName", GetNameKey(Function.ToString(e.Value), Enums.BusinessPartnerCatalogNumbersTableType.TabItem));
				gvItems.GetFocusedDataRow()["ItemCode"] = btnItemCode.Text;
				gvItems.GetFocusedDataRow()["ItemName"] = txt_ItemName.Text;
				gvItems.GetFocusedDataRow()["ShowSCN"] = (ItemsPrintBP.Checked ? "Y" : "N");
				grdItems.DataSourceTable.AcceptChanges();
			}
			if (e.Column.FieldName.ToLower().Equals("Substitute".ToLower()) && string.IsNullOrEmpty(Function.ToString(gvItems.GetFocusedRowCellValue("CardCode"))))
			{
				EnableDisableControl(enable: true);
				return;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "gvBP_CellValueChanged  ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void BPPrintBP_CheckedChanged(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			DataTable dataTable = grdBP.DataSourceTable.Copy();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ItemCode"])))
				{
					dataTable.Rows[i]["ShowSCN"] = (BPPrintBP.Checked ? "Y" : "N");
				}
			}
			grdBP.DataSource = dataTable;
			grdBP.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(enable: true);
	}

	private void ItemsPrintBP_CheckedChanged(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			DataTable dataTable = grdItems.DataSourceTable.Copy();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["CardCode"])))
				{
					dataTable.Rows[i]["ShowSCN"] = (BPPrintBP.Checked ? "Y" : "N");
				}
			}
			grdItems.DataSource = dataTable;
			grdItems.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(enable: true);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if (keyData == Keys.Tab)
		{
			if (ActiveControl.Parent == btnBPCode)
			{
				ShowChooseFromListBp(isControl: true);
			}
			else if (ActiveControl.Parent == btnItemCode)
			{
				ShowChooseFromListItems();
			}
		}
		return base.ProcessDialogKey(keyData);
	}

	private void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (xtraTabBPCN.SelectedTabPage.Name == TabBP.Name)
			{
				if (string.IsNullOrEmpty(btnBPCode.Text))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseBpCodeToRemove, "");
					EnableDisableControl(enable: true);
				}
				else
				{
					EnableDisableControl(enable: true);
				}
				return;
			}
			if (xtraTabBPCN.SelectedTabPage.Name == TabItems.Name && string.IsNullOrEmpty(btnItemCode.Text))
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseItemToRemove, "");
				EnableDisableControl(enable: true);
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(enable: true);
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (xtraTabBPCN.SelectedTabPage.Name == TabBP.Name)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvBP.GetFocusedRowCellValue("ItemCode"))))
				{
					EnableDisableControl(enable: true);
					return;
				}
				DataTable dataSourceTable = grdBP.DataSourceTable;
				dataSourceTable.Rows.RemoveAt(gvBP.FocusedRowHandle);
				dataSourceTable.AcceptChanges();
				grdBP.DataSource = dataSourceTable;
				grdBP.RefreshDataSource();
				if (dataSourceTable.AsEnumerable().All((DataRow r) => !string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])) || !string.IsNullOrWhiteSpace(Function.ToString(r["ItemName"])) || !string.IsNullOrWhiteSpace(Function.ToString(r["Substitute"]))))
				{
					AddNewRow(TabBP.Name);
				}
			}
			else if (xtraTabBPCN.SelectedTabPage.Name == TabItems.Name)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvItems.GetFocusedRowCellValue("CardCode"))))
				{
					EnableDisableControl(enable: true);
					return;
				}
				DataTable dataSourceTable2 = grdItems.DataSourceTable;
				dataSourceTable2.Rows.RemoveAt(gvItems.FocusedRowHandle);
				dataSourceTable2.AcceptChanges();
				grdItems.DataSource = dataSourceTable2;
				grdItems.RefreshDataSource();
				if (dataSourceTable2.AsEnumerable().All((DataRow r) => !string.IsNullOrWhiteSpace(Function.ToString(r["CardCode"])) || !string.IsNullOrWhiteSpace(Function.ToString(r["CardName"])) || !string.IsNullOrWhiteSpace(Function.ToString(r["Substitute"]))))
				{
					AddNewRow(TabItems.Name);
				}
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (xtraTabBPCN.SelectedTabPage.Name == TabBP.Name)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvBP.GetFocusedRowCellValue("ItemCode"))))
				{
					EnableDisableControl(enable: true);
					return;
				}
				DataRow dataRow = grdBP.DataSourceTable.NewRow();
				dataRow["ItemCode"] = gvBP.GetFocusedRowCellValue("ItemCode");
				dataRow["ItemName"] = gvBP.GetFocusedRowCellValue("ItemName");
				dataRow["CardCode"] = gvBP.GetFocusedRowCellValue("CardCode");
				dataRow["CardName"] = gvBP.GetFocusedRowCellValue("CardName");
				dataRow["Substitute"] = gvBP.GetFocusedRowCellValue("Substitute");
				dataRow["ShowSCN"] = gvBP.GetFocusedRowCellValue("ShowSCN");
				grdBP.DataSourceTable.Rows.InsertAt(dataRow, grdBP.DataSourceTable.Rows.Count - 1);
				grdBP.DataSourceTable.AcceptChanges();
				grdBP.RefreshDataSource();
			}
			else if (xtraTabBPCN.SelectedTabPage.Name == TabItems.Name)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvItems.GetFocusedRowCellValue("CardCode"))))
				{
					EnableDisableControl(enable: true);
					return;
				}
				DataRow dataRow2 = grdItems.DataSourceTable.NewRow();
				dataRow2["ItemCode"] = gvItems.GetFocusedRowCellValue("ItemCode");
				dataRow2["ItemName"] = gvItems.GetFocusedRowCellValue("ItemName");
				dataRow2["CardCode"] = gvItems.GetFocusedRowCellValue("CardCode");
				dataRow2["CardName"] = gvItems.GetFocusedRowCellValue("CardName");
				dataRow2["Substitute"] = gvItems.GetFocusedRowCellValue("Substitute");
				dataRow2["ShowSCN"] = gvItems.GetFocusedRowCellValue("ShowSCN");
				grdItems.DataSourceTable.Rows.InsertAt(dataRow2, grdItems.DataSourceTable.Rows.Count - 1);
				grdItems.DataSourceTable.AcceptChanges();
				grdItems.RefreshDataSource();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(enable: true);
	}

	private void grdBP_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(btnBPCode.Text) && grdBP.DataSourceTable != null && grdBP.DataSourceTable.Rows.Count > 0 && e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				EnableDisableControl(enable: false);
				List<string> key = new List<string> { "ItemCode" };
				ExtenseionMethods.PasteFromExcelToDataTable(grdBP, gvBP, key);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(enable: true);
	}

	private void grdItems_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(btnItemCode.Text) && grdItems.DataSourceTable != null && grdItems.DataSourceTable.Rows.Count > 0 && e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				EnableDisableControl(enable: false);
				List<string> key = new List<string> { "CardCode" };
				ExtenseionMethods.PasteFromExcelToDataTable(grdItems, gvItems, key);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(enable: true);
	}

	private void removeRow_ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		EnableDisableControl(enable: false);
		try
		{
			if (xtraTabBPCN.SelectedTabPage.Name == TabBP.Name)
			{
				string subtitle = Function.ToString(gvBP.GetFocusedRowCellValue("Substitute"));
				string item = Function.ToString(gvBP.GetFocusedRowCellValue("ItemCode"));
				if (string.IsNullOrEmpty(btnBPCode.Text))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseBpCodeToRemove, "");
					EnableDisableControl(enable: true);
				}
				else
				{
					Remove(btnBPCode.Text, subtitle, item, TabBP.Name);
					EnableDisableControl(enable: true);
				}
				return;
			}
			if (xtraTabBPCN.SelectedTabPage.Name == TabItems.Name)
			{
				string subtitle2 = Function.ToString(gvItems.GetFocusedRowCellValue("Substitute"));
				string paramateCode = Function.ToString(gvItems.GetFocusedRowCellValue("CardCode"));
				if (string.IsNullOrEmpty(btnItemCode.Text))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseItemToRemove, "");
					EnableDisableControl(enable: true);
					return;
				}
				Remove(paramateCode, subtitle2, btnItemCode.Text, TabItems.Name);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(enable: true);
	}

	private void gvBP_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void gvBP_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string code = Function.ToString(gvBP.GetFocusedRowCellValue("CardCode"));
			string subtitle = Function.ToString(gvBP.GetFocusedRowCellValue("Substitute"));
			string item = Function.ToString(gvBP.GetFocusedRowCellValue("ItemCode"));
			if (string.Equals(_httpClient.BusinessPartnerCatalogNumbersClient.CheckExist(code, subtitle, item, ApzObjectTypes.oBusinessPartners), "Y"))
			{
				removeRow_ToolStripMenuItem.Visible = true;
			}
			else
			{
				removeRow_ToolStripMenuItem.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void gvItems_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string code = Function.ToString(gvItems.GetFocusedRowCellValue("CardCode"));
			string subtitle = Function.ToString(gvItems.GetFocusedRowCellValue("Substitute"));
			string item = Function.ToString(gvItems.GetFocusedRowCellValue("ItemCode"));
			if (string.Equals(_httpClient.BusinessPartnerCatalogNumbersClient.CheckExist(code, subtitle, item, ApzObjectTypes.oItems), "Y"))
			{
				removeRow_ToolStripMenuItem.Enabled = true;
			}
			else
			{
				removeRow_ToolStripMenuItem.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.frm_BusinessPartnerCatalogNumbers));
		this.xtraTabBPCN = new Apzon.Controls.Controls.XtraTabControlEx();
		this.ContexMenuDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.RemoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.TabBP = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_BpName = new Apzon.Controls.Editor.TextEditEx();
		this.btnBPCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.BPPrintBP = new Apzon.Controls.Editor.CheckEditEx();
		this.grdBP = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRow_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvBP = new Apzon.Controls.Grid.GridViewEx();
		this.ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemName = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Substitute = new Apzon.Controls.Grid.GridColumnEx();
		this.lb_BPName = new Apzon.Controls.Controls.LabelLine();
		this.lb_BPCode = new Apzon.Controls.Controls.LabelLine();
		this.TabItems = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_ItemName = new Apzon.Controls.Editor.TextEditEx();
		this.ItemsPrintBP = new Apzon.Controls.Editor.CheckEditEx();
		this.grdItems = new Apzon.Controls.Grid.GridControlEx();
		this.gvItems = new Apzon.Controls.Grid.GridViewEx();
		this.CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardName = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Substitute2 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_SubTitile = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.lb_ItemsName = new Apzon.Controls.Controls.LabelLine();
		this.lb_ItemsCode = new Apzon.Controls.Controls.LabelLine();
		this.btnItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTabBPCN).BeginInit();
		this.xtraTabBPCN.SuspendLayout();
		this.ContexMenuDoc.SuspendLayout();
		this.TabBP.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_BpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnBPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.BPPrintBP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdBP).BeginInit();
		this.contextMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvBP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemName).BeginInit();
		this.TabItems.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ItemsPrintBP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdItems).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvItems).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SubTitile).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnItemCode.Properties).BeginInit();
		base.SuspendLayout();
		this.xtraTabBPCN.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabBPCN.ContextMenuStrip = this.ContexMenuDoc;
		this.xtraTabBPCN.IsSearchControl = false;
		this.xtraTabBPCN.Location = new System.Drawing.Point(2, 3);
		this.xtraTabBPCN.Name = "xtraTabBPCN";
		this.xtraTabBPCN.SelectedTabPage = this.TabBP;
		this.xtraTabBPCN.Size = new System.Drawing.Size(664, 475);
		this.xtraTabBPCN.TabIndex = 0;
		this.xtraTabBPCN.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.TabBP, this.TabItems });
		this.xtraTabBPCN.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.xtraTabBPCN_SelectedPageChanging);
		this.ContexMenuDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.RemoveToolStripMenuItem });
		this.ContexMenuDoc.Name = "ContexMenuDoc";
		this.ContexMenuDoc.Size = new System.Drawing.Size(118, 26);
		this.RemoveToolStripMenuItem.Name = "RemoveToolStripMenuItem";
		this.RemoveToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.RemoveToolStripMenuItem.Text = "Remove";
		this.RemoveToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
		this.TabBP.Appearance.Header.Options.UseTextOptions = true;
		this.TabBP.Appearance.Header.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.TabBP.Controls.Add(this.txt_BpName);
		this.TabBP.Controls.Add(this.btnBPCode);
		this.TabBP.Controls.Add(this.BPPrintBP);
		this.TabBP.Controls.Add(this.grdBP);
		this.TabBP.Controls.Add(this.lb_BPName);
		this.TabBP.Controls.Add(this.lb_BPCode);
		this.TabBP.EditControl = null;
		this.TabBP.EditValue = "BP";
		this.TabBP.FieldName = "TabBP";
		this.TabBP.IsSearchControl = false;
		this.TabBP.Name = "TabBP";
		this.TabBP.ReadOnly = false;
		this.TabBP.SettingInVisible = false;
		this.TabBP.SettingReadonly = false;
		this.TabBP.Size = new System.Drawing.Size(662, 450);
		this.TabBP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.TabBP.TabPageWidth = 75;
		this.TabBP.Text = "BP";
		this.txt_BpName.EditControl = null;
		this.txt_BpName.FieldName = null;
		this.txt_BpName.IsSearchControl = false;
		this.txt_BpName.Location = new System.Drawing.Point(307, 5);
		this.txt_BpName.Name = "txt_BpName";
		this.txt_BpName.Properties.ReadOnly = true;
		this.txt_BpName.SettingInVisible = false;
		this.txt_BpName.SettingReadonly = false;
		this.txt_BpName.Size = new System.Drawing.Size(150, 20);
		this.txt_BpName.TabIndex = 7;
		this.btnBPCode.EditControl = null;
		this.btnBPCode.FieldName = null;
		this.btnBPCode.IsSearchControl = false;
		this.btnBPCode.Location = new System.Drawing.Point(76, 5);
		this.btnBPCode.Name = "btnBPCode";
		this.btnBPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnBPCode.SettingInVisible = false;
		this.btnBPCode.SettingReadonly = false;
		this.btnBPCode.Size = new System.Drawing.Size(150, 20);
		this.btnBPCode.TabIndex = 1;
		this.btnBPCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_BPCode_ButtonClick);
		this.btnBPCode.EditValueChanged += new System.EventHandler(this.btnBPCode_EditValueChanged);
		this.BPPrintBP.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.BPPrintBP.EditControl = null;
		this.BPPrintBP.FieldName = null;
		this.BPPrintBP.IsSearchControl = false;
		this.BPPrintBP.Location = new System.Drawing.Point(7, 424);
		this.BPPrintBP.Name = "BPPrintBP";
		this.BPPrintBP.Properties.Caption = "Print BP Catalog Number in Documents";
		this.BPPrintBP.SettingInVisible = false;
		this.BPPrintBP.SettingReadonly = false;
		this.BPPrintBP.Size = new System.Drawing.Size(214, 20);
		this.BPPrintBP.TabIndex = 4;
		this.BPPrintBP.CheckedChanged += new System.EventHandler(this.BPPrintBP_CheckedChanged);
		this.grdBP.AllowPasteExcel = true;
		this.grdBP.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdBP.ClipboardData = null;
		this.grdBP.ContextMenuStrip = this.contextMenu;
		this.grdBP.Location = new System.Drawing.Point(3, 34);
		this.grdBP.MainView = this.gvBP;
		this.grdBP.Name = "grdBP";
		this.grdBP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_ItemCode, this.rep_ItemName });
		this.grdBP.Size = new System.Drawing.Size(655, 384);
		this.grdBP.TabIndex = 6;
		this.grdBP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvBP });
		this.grdBP.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdBP_EditorKeyDown);
		this.contextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.deleteRowToolStripMenuItem, this.duplicateRowToolStripMenuItem, this.removeRow_ToolStripMenuItem });
		this.contextMenu.Name = "contextMenu";
		this.contextMenu.Size = new System.Drawing.Size(151, 70);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.removeRow_ToolStripMenuItem.Name = "removeRow_ToolStripMenuItem";
		this.removeRow_ToolStripMenuItem.Size = new System.Drawing.Size(150, 22);
		this.removeRow_ToolStripMenuItem.Text = "Remove";
		this.removeRow_ToolStripMenuItem.Click += new System.EventHandler(this.removeRow_ToolStripMenuItem_Click);
		this.gvBP.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBP.ColumnKey = null;
		this.gvBP.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.ItemCode, this.ItemName, this.Substitute });
		this.gvBP.DisableFormSetting = false;
		this.gvBP.DisplayException = false;
		this.gvBP.EditColumn = null;
		this.gvBP.FieldCurrency = null;
		this.gvBP.FieldRate = null;
		this.gvBP.FormId = "oBpCatalog";
		this.gvBP.GridControl = this.grdBP;
		this.gvBP.IgnoreFilter = false;
		this.gvBP.IndexEditRow = -1;
		this.gvBP.IndicatorWidth = 40;
		this.gvBP.IsApzTable = false;
		this.gvBP.IsInsertNewRow = false;
		this.gvBP.IsLoadingData = false;
		this.gvBP.IsMappingContextAction = false;
		this.gvBP.ItemId = "OSCN";
		this.gvBP.LstColumnAllowBatch = null;
		this.gvBP.LstColumnNotAllowBatch = null;
		this.gvBP.LstColumnNotEditable = null;
		this.gvBP.LstColumNotSetting = null;
		this.gvBP.Name = "gvBP";
		this.gvBP.OptionsView.ColumnAutoWidth = false;
		this.gvBP.OptionsView.ShowGroupPanel = false;
		this.gvBP.UseBatchMod = false;
		this.gvBP.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvBP_CustomDrawRowIndicator);
		this.gvBP.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvBP_FocusedRowChanged);
		this.gvBP.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvBP_FocusedRowObjectChanged);
		this.gvBP.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBP_CellValueChanged);
		this.ItemCode.Caption = "Item No.";
		this.ItemCode.ColumnEdit = this.rep_ItemCode;
		this.ItemCode.CurrencyDateField = null;
		this.ItemCode.CurrencyField = null;
		this.ItemCode.CurrencyFixed = null;
		this.ItemCode.CurrencyFixedRate = 0.0;
		this.ItemCode.CurrencyRateField = null;
		this.ItemCode.FieldName = "ItemCode";
		this.ItemCode.Name = "ItemCode";
		this.ItemCode.PrimaryKey = true;
		this.ItemCode.UnitField = null;
		this.ItemCode.Visible = true;
		this.ItemCode.VisibleIndex = 0;
		this.ItemCode.Width = 102;
		this.rep_ItemCode.AutoHeight = false;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonClick);
		this.ItemName.Caption = "Item Description";
		this.ItemName.ColumnEdit = this.rep_ItemName;
		this.ItemName.CurrencyDateField = null;
		this.ItemName.CurrencyField = null;
		this.ItemName.CurrencyFixed = null;
		this.ItemName.CurrencyFixedRate = 0.0;
		this.ItemName.CurrencyRateField = null;
		this.ItemName.FieldName = "ItemName";
		this.ItemName.Name = "ItemName";
		this.ItemName.PrimaryKey = false;
		this.ItemName.UnitField = null;
		this.ItemName.Visible = true;
		this.ItemName.VisibleIndex = 1;
		this.ItemName.Width = 151;
		this.rep_ItemName.AutoHeight = false;
		this.rep_ItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_ItemName.Name = "rep_ItemName";
		this.rep_ItemName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemName_ButtonClick);
		this.Substitute.Caption = "BP Catalog No.";
		this.Substitute.CurrencyDateField = null;
		this.Substitute.CurrencyField = null;
		this.Substitute.CurrencyFixed = null;
		this.Substitute.CurrencyFixedRate = 0.0;
		this.Substitute.CurrencyRateField = null;
		this.Substitute.FieldName = "Substitute";
		this.Substitute.Name = "Substitute";
		this.Substitute.PrimaryKey = false;
		this.Substitute.UnitField = null;
		this.Substitute.Visible = true;
		this.Substitute.VisibleIndex = 2;
		this.Substitute.Width = 187;
		this.lb_BPName.EditControl = null;
		this.lb_BPName.EditValue = "Name";
		this.lb_BPName.FieldName = null;
		this.lb_BPName.HideWhenEmpty = false;
		this.lb_BPName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BPName.Location = new System.Drawing.Point(245, 8);
		this.lb_BPName.Name = "lb_BPName";
		this.lb_BPName.ReadOnly = false;
		this.lb_BPName.SettingInVisible = false;
		this.lb_BPName.SettingReadonly = false;
		this.lb_BPName.ShowUnderLine = true;
		this.lb_BPName.Size = new System.Drawing.Size(70, 20);
		this.lb_BPName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BPName.TabIndex = 0;
		this.lb_BPName.Utext = "Name";
		this.lb_BPCode.EditControl = null;
		this.lb_BPCode.EditValue = "Code";
		this.lb_BPCode.FieldName = null;
		this.lb_BPCode.HideWhenEmpty = false;
		this.lb_BPCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BPCode.Location = new System.Drawing.Point(7, 8);
		this.lb_BPCode.Name = "lb_BPCode";
		this.lb_BPCode.ReadOnly = false;
		this.lb_BPCode.SettingInVisible = false;
		this.lb_BPCode.SettingReadonly = false;
		this.lb_BPCode.ShowUnderLine = true;
		this.lb_BPCode.Size = new System.Drawing.Size(70, 20);
		this.lb_BPCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BPCode.TabIndex = 0;
		this.lb_BPCode.Utext = "Code";
		this.TabItems.Appearance.Header.Options.UseTextOptions = true;
		this.TabItems.Appearance.Header.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.TabItems.Controls.Add(this.txt_ItemName);
		this.TabItems.Controls.Add(this.ItemsPrintBP);
		this.TabItems.Controls.Add(this.grdItems);
		this.TabItems.Controls.Add(this.lb_ItemsName);
		this.TabItems.Controls.Add(this.lb_ItemsCode);
		this.TabItems.Controls.Add(this.btnItemCode);
		this.TabItems.EditControl = null;
		this.TabItems.EditValue = "Items";
		this.TabItems.FieldName = "TabItems";
		this.TabItems.IsSearchControl = false;
		this.TabItems.Name = "TabItems";
		this.TabItems.ReadOnly = false;
		this.TabItems.SettingInVisible = false;
		this.TabItems.SettingReadonly = false;
		this.TabItems.Size = new System.Drawing.Size(662, 450);
		this.TabItems.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.TabItems.TabPageWidth = 75;
		this.TabItems.Text = "Items";
		this.txt_ItemName.EditControl = null;
		this.txt_ItemName.FieldName = null;
		this.txt_ItemName.IsSearchControl = false;
		this.txt_ItemName.Location = new System.Drawing.Point(310, 5);
		this.txt_ItemName.Name = "txt_ItemName";
		this.txt_ItemName.Properties.ReadOnly = true;
		this.txt_ItemName.SettingInVisible = false;
		this.txt_ItemName.SettingReadonly = false;
		this.txt_ItemName.Size = new System.Drawing.Size(150, 20);
		this.txt_ItemName.TabIndex = 18;
		this.ItemsPrintBP.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.ItemsPrintBP.EditControl = null;
		this.ItemsPrintBP.FieldName = null;
		this.ItemsPrintBP.IsSearchControl = false;
		this.ItemsPrintBP.Location = new System.Drawing.Point(4, 424);
		this.ItemsPrintBP.Name = "ItemsPrintBP";
		this.ItemsPrintBP.Properties.Caption = "Print BP Catalog Number in Documents";
		this.ItemsPrintBP.SettingInVisible = false;
		this.ItemsPrintBP.SettingReadonly = false;
		this.ItemsPrintBP.Size = new System.Drawing.Size(214, 20);
		this.ItemsPrintBP.TabIndex = 8;
		this.ItemsPrintBP.CheckedChanged += new System.EventHandler(this.ItemsPrintBP_CheckedChanged);
		this.grdItems.AllowPasteExcel = false;
		this.grdItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdItems.ClipboardData = null;
		this.grdItems.ContextMenuStrip = this.contextMenu;
		this.grdItems.Location = new System.Drawing.Point(3, 35);
		this.grdItems.MainView = this.gvItems;
		this.grdItems.Name = "grdItems";
		this.grdItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_CardCode, this.rep_CardName, this.txt_SubTitile });
		this.grdItems.Size = new System.Drawing.Size(655, 384);
		this.grdItems.TabIndex = 17;
		this.grdItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvItems });
		this.grdItems.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdItems_EditorKeyDown);
		this.gvItems.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvItems.ColumnKey = null;
		this.gvItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.CardCode, this.CardName, this.Substitute2 });
		this.gvItems.DisableFormSetting = false;
		this.gvItems.DisplayException = false;
		this.gvItems.EditColumn = null;
		this.gvItems.FieldCurrency = null;
		this.gvItems.FieldRate = null;
		this.gvItems.FormId = "oBpCatalog";
		this.gvItems.GridControl = this.grdItems;
		this.gvItems.IgnoreFilter = false;
		this.gvItems.IndexEditRow = -1;
		this.gvItems.IndicatorWidth = 40;
		this.gvItems.IsApzTable = false;
		this.gvItems.IsInsertNewRow = false;
		this.gvItems.IsLoadingData = false;
		this.gvItems.IsMappingContextAction = false;
		this.gvItems.ItemId = "OSCNITM";
		this.gvItems.LstColumnAllowBatch = null;
		this.gvItems.LstColumnNotAllowBatch = null;
		this.gvItems.LstColumnNotEditable = null;
		this.gvItems.LstColumNotSetting = null;
		this.gvItems.Name = "gvItems";
		this.gvItems.OptionsView.ColumnAutoWidth = false;
		this.gvItems.OptionsView.ShowGroupPanel = false;
		this.gvItems.UseBatchMod = false;
		this.gvItems.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvItems_CustomDrawRowIndicator);
		this.gvItems.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvItems_FocusedRowObjectChanged);
		this.gvItems.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvItems_CellValueChanged);
		this.CardCode.Caption = "BP Code";
		this.CardCode.ColumnEdit = this.rep_CardCode;
		this.CardCode.CurrencyDateField = null;
		this.CardCode.CurrencyField = null;
		this.CardCode.CurrencyFixed = null;
		this.CardCode.CurrencyFixedRate = 0.0;
		this.CardCode.CurrencyRateField = null;
		this.CardCode.FieldName = "CardCode";
		this.CardCode.Name = "CardCode";
		this.CardCode.PrimaryKey = false;
		this.CardCode.UnitField = null;
		this.CardCode.Visible = true;
		this.CardCode.VisibleIndex = 0;
		this.CardCode.Width = 125;
		this.rep_CardCode.AutoHeight = false;
		this.rep_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_CardCode.Name = "rep_CardCode";
		this.rep_CardCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CardCode_ButtonClick);
		this.CardName.Caption = "BP Name";
		this.CardName.ColumnEdit = this.rep_CardName;
		this.CardName.CurrencyDateField = null;
		this.CardName.CurrencyField = null;
		this.CardName.CurrencyFixed = null;
		this.CardName.CurrencyFixedRate = 0.0;
		this.CardName.CurrencyRateField = null;
		this.CardName.FieldName = "CardName";
		this.CardName.Name = "CardName";
		this.CardName.PrimaryKey = false;
		this.CardName.UnitField = null;
		this.CardName.Visible = true;
		this.CardName.VisibleIndex = 1;
		this.CardName.Width = 120;
		this.rep_CardName.AutoHeight = false;
		this.rep_CardName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_CardName.Name = "rep_CardName";
		this.rep_CardName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_CardName_ButtonClick);
		this.Substitute2.Caption = "Bp Catalog No.";
		this.Substitute2.ColumnEdit = this.txt_SubTitile;
		this.Substitute2.CurrencyDateField = null;
		this.Substitute2.CurrencyField = null;
		this.Substitute2.CurrencyFixed = null;
		this.Substitute2.CurrencyFixedRate = 0.0;
		this.Substitute2.CurrencyRateField = null;
		this.Substitute2.FieldName = "Substitute";
		this.Substitute2.Name = "Substitute2";
		this.Substitute2.PrimaryKey = false;
		this.Substitute2.UnitField = null;
		this.Substitute2.Visible = true;
		this.Substitute2.VisibleIndex = 2;
		this.Substitute2.Width = 147;
		this.txt_SubTitile.AutoHeight = false;
		this.txt_SubTitile.Name = "txt_SubTitile";
		this.lb_ItemsName.EditControl = null;
		this.lb_ItemsName.EditValue = "Name";
		this.lb_ItemsName.FieldName = null;
		this.lb_ItemsName.HideWhenEmpty = false;
		this.lb_ItemsName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemsName.Location = new System.Drawing.Point(243, 8);
		this.lb_ItemsName.Name = "lb_ItemsName";
		this.lb_ItemsName.ReadOnly = false;
		this.lb_ItemsName.SettingInVisible = false;
		this.lb_ItemsName.SettingReadonly = false;
		this.lb_ItemsName.ShowUnderLine = true;
		this.lb_ItemsName.Size = new System.Drawing.Size(70, 20);
		this.lb_ItemsName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemsName.TabIndex = 15;
		this.lb_ItemsName.Utext = "Name";
		this.lb_ItemsCode.EditControl = null;
		this.lb_ItemsCode.EditValue = "Code";
		this.lb_ItemsCode.FieldName = null;
		this.lb_ItemsCode.HideWhenEmpty = false;
		this.lb_ItemsCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemsCode.Location = new System.Drawing.Point(7, 8);
		this.lb_ItemsCode.Name = "lb_ItemsCode";
		this.lb_ItemsCode.ReadOnly = false;
		this.lb_ItemsCode.SettingInVisible = false;
		this.lb_ItemsCode.SettingReadonly = false;
		this.lb_ItemsCode.ShowUnderLine = true;
		this.lb_ItemsCode.Size = new System.Drawing.Size(70, 20);
		this.lb_ItemsCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemsCode.TabIndex = 8;
		this.lb_ItemsCode.Utext = "Code";
		this.btnItemCode.EditControl = null;
		this.btnItemCode.FieldName = null;
		this.btnItemCode.IsSearchControl = false;
		this.btnItemCode.Location = new System.Drawing.Point(77, 5);
		this.btnItemCode.Name = "btnItemCode";
		this.btnItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btnItemCode.SettingInVisible = false;
		this.btnItemCode.SettingReadonly = false;
		this.btnItemCode.Size = new System.Drawing.Size(150, 20);
		this.btnItemCode.TabIndex = 5;
		this.btnItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnItemCode_ButtonClick);
		this.btnItemCode.EditValueChanged += new System.EventHandler(this.btnItemCode_EditValueChanged);
		this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnAdd.EditControl = null;
		this.btnAdd.EditValue = "OK";
		this.btnAdd.FieldName = null;
		this.btnAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAdd.ImageOptions.Image");
		this.btnAdd.IsSearchControl = false;
		this.btnAdd.Location = new System.Drawing.Point(9, 484);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.ReadOnly = false;
		this.btnAdd.SettingInVisible = false;
		this.btnAdd.SettingReadonly = false;
		this.btnAdd.Size = new System.Drawing.Size(70, 20);
		this.btnAdd.TabIndex = 11;
		this.btnAdd.Text = "OK";
		this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(83, 484);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(70, 20);
		this.btnCancel.TabIndex = 12;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btnAdd;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(669, 510);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnAdd);
		base.Controls.Add(this.xtraTabBPCN);
		base.FormId = "oBpCatalog";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BusinessPartnerCatalogNumbers";
		this.Text = "Business Partner Catalog Numbers";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_BusinessPartnerCatalogNumbers_Load);
		((System.ComponentModel.ISupportInitialize)this.xtraTabBPCN).EndInit();
		this.xtraTabBPCN.ResumeLayout(false);
		this.ContexMenuDoc.ResumeLayout(false);
		this.TabBP.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_BpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnBPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.BPPrintBP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdBP).EndInit();
		this.contextMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvBP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemName).EndInit();
		this.TabItems.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_ItemName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ItemsPrintBP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdItems).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvItems).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SubTitile).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnItemCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
