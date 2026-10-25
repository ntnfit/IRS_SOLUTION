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
using Apzon.Entities.Models.MasterData;
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

namespace Apzon.Forms.Retail;

public class frm_RetailStore : XtraFormEx
{
	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtBranchAssignment = new DataTable();

	private bool _preAdd;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private RepositoryItemButtonEdit rep_WHouse;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private RepositoryItemButtonEdit rep_Customer;

	private RepositoryItemButtonEdit rep_SalePerson;

	private RepositoryItemLookUpEdit rep_PriceList;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private GridColumnEx gridColumn9;

	private RepositoryItemCheckEdit rep_Keyboard;

	private GridColumnEx col_Branch;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumnEx2;

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
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_RetailStore()
	{
		InitializeComponent();
		OnLoadAsync += frm_RetailStore_Load;
		FormId = (gvDoc.FormId = "oRetailStore");
		gvDoc.ItemId = "APZ_STORE";
	}

	private async Task LoadInitForm()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			using (HttpClientHelper<PriceListImpl> httpClientHelper = new HttpClientHelper<PriceListImpl>(Configuration.ApiServer, "PriceList"))
			{
				foreach (PriceListImpl item in httpClientHelper.GetList("Cfl"))
				{
					dataTable.Rows.Add(item.PriceListNo, item.PriceListName);
				}
				rep_PriceList.Columns.Clear();
				rep_PriceList.Columns.Add(new LookUpColumnInfo
				{
					Caption = "",
					FieldName = "Name"
				});
				rep_PriceList.DisplayMember = "Name";
				rep_PriceList.ValueMember = "Code";
				rep_PriceList.NullText = "";
				rep_PriceList.DataSource = dataTable;
			}
			rep_Branch.Columns.Clear();
			rep_Branch.Columns.Add(new LookUpColumnInfo("BPLId", "Code"));
			rep_Branch.Columns.Add(new LookUpColumnInfo("BPLName", "Name"));
			rep_Branch.DisplayMember = "BPLName";
			rep_Branch.ValueMember = "BPLId";
			rep_Branch.NullText = "";
			_dtBranchAssignment = await _httpClient.User.GetUserBranchAssignment("");
			rep_Branch.DataSource = _dtBranchAssignment;
			if (GlobalData.IsMultiBranch)
			{
				col_Branch.OptionsColumn.ReadOnly = false;
			}
			else
			{
				col_Branch.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					AddNew();
				}
			}
			else
			{
				AddNew();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await _httpClient.RetailStore.LoadAllDoc();
			grdDoc.DataSource = dataTable;
			dataTable.AcceptChanges();
			grdDoc.RefreshDataSource();
			gridColumn1.OptionsColumn.AllowEdit = false;
			gridColumn1.OptionsColumn.ReadOnly = true;
			if (!dataTable.IsNotNull())
			{
				AddNew();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNew()
	{
		try
		{
			_preAdd = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_STORE");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["StoreID"] = 0;
			dataRow["Locked"] = "N";
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			grdDoc.DataSource = tableStructure;
			grdDoc.RefreshDataSource();
			gridColumn1.OptionsColumn.AllowEdit = true;
			gridColumn1.OptionsColumn.ReadOnly = false;
			_preAdd = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		_ = 2;
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int id = Function.ParseInt(gvDoc.GetFocusedRowCellValue("StoreID"));
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRow drow = dataTable.AsEnumerable().SingleOrDefault((DataRow r) => r.Field<int>("StoreID") == id);
			DataTable dtSave = dataTable.Clone();
			dtSave.ImportRow(drow);
			dtSave.AcceptChanges();
			DataTable dataTable2 = await _httpClient.RetailStore.LoadAllDoc();
			if (dataTable2.AsEnumerable().SingleOrDefault((DataRow r) => r.Field<int>("StoreID") == id) == null)
			{
				dataTable2.ImportRow(drow);
				dtSave.Rows[0]["CreateDate"] = DateTime.Now;
				dtSave.Rows[0]["UserSign"] = GlobalData.UserID;
				if (CheckCode(dataTable2))
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInDatabase, "");
					return;
				}
			}
			dtSave.Rows[0]["UpdateDate"] = DateTime.Now;
			dtSave.Rows[0]["UserSign2"] = GlobalData.UserID;
			dtSave.AcceptChanges();
			string text = await _httpClient.RetailStore.Save(dtSave);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				int num = gvDoc.LocateByValue("StoreID", id);
				if (num != int.MinValue)
				{
					gvDoc.FocusedRowHandle = num;
					gvDoc.MakeRowVisible(num);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckCode(DataTable dt)
	{
		int num = 0;
		bool result = false;
		try
		{
			if (dt.Select("Code <> ''").Any())
			{
				EnumerableRowCollection<DataRow> source = from r in dt.AsEnumerable()
					where r.Field<string>("Code").Trim() != ""
					select r;
				if (source.Any() && (from dr in source.CopyToDataTable().AsEnumerable()
					select dr.Field<string>("Code").Trim() into x
					group x by x into g
					where g.Count() > 1
					select g.Key).ToList().Count > 0)
				{
					num++;
				}
			}
			if (num > 0)
			{
				result = true;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return result;
		}
	}

	public void ChooseFormListCardCode(string cardCode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, cardCode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("CardCode", chooseItem["CardCode"]);
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ChooseFormListWareHouse(string whseCode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, whseCode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("WhsCode", chooseItem["WhsCode"]);
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ChooseFormListSalePerson()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.SaleEmployee, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("SlpCode", chooseItem["SlpCode"]);
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_RetailStore_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadData();
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
					await SaveData();
					WinCommons._CloseProcessing();
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
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_RetailStore_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btn_Save_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Customer_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode"));
			if (e.Button.Index == 0)
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "Code"))))
				{
					return;
				}
				ChooseFormListCardCode(text);
			}
			if (e.Button.Index == 1 && !string.IsNullOrEmpty(text))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = text;
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_WHouse_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("WhsCode"));
			if (e.Button.Index == 0)
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "Code"))))
				{
					return;
				}
				ChooseFormListWareHouse(text);
			}
			if (e.Button.Index == 1 && !string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_SalePerson_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "Code"))))
				{
					return;
				}
				ChooseFormListSalePerson();
			}
			if (e.Button.Index == 1)
			{
				_ = gvDoc.FocusedRowHandle;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			string text = Function.ToString(dataTable.Rows[gvDoc.FocusedRowHandle]["Code"]);
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = await _httpClient.RetailStore.RemoveStore(text);
				if (text2 == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
			FormMode = Enums.FormViewModeType.View;
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
			if (!_preAdd && FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
					return;
				}
				(grdDoc.DataSource as DataTable)?.RejectChanges();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Retail.frm_RetailStore));
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
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.col_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WHouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Customer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SalePerson = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Keyboard = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WHouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Customer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalePerson).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Keyboard).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 395);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 76;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(105, 395);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(0, 5);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_WHouse, this.rep_PriceList, this.rep_Customer, this.rep_SalePerson, this.rep_Keyboard, this.rep_Branch, this.repositoryItemCheckEdit1 });
		this.grdDoc.Size = new System.Drawing.Size(975, 384);
		this.grdDoc.TabIndex = 74;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx2, this.gridColumn1, this.col_Branch, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8,
			this.gridColumnEx1, this.gridColumn9
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oRetailStore";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ODATA";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx2.Caption = "ID";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "StoreID";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Code";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 69;
		this.col_Branch.Caption = "Branch";
		this.col_Branch.ColumnEdit = this.rep_Branch;
		this.col_Branch.CurrencyDateField = null;
		this.col_Branch.CurrencyField = null;
		this.col_Branch.CurrencyFixed = null;
		this.col_Branch.CurrencyFixedRate = 0.0;
		this.col_Branch.CurrencyRateField = null;
		this.col_Branch.FieldName = "BPLId";
		this.col_Branch.Name = "col_Branch";
		this.col_Branch.PrimaryKey = false;
		this.col_Branch.UnitField = null;
		this.col_Branch.Visible = true;
		this.col_Branch.VisibleIndex = 1;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumn2.Caption = "Description";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Dscription";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn2.Width = 85;
		this.gridColumn3.Caption = "Selling Warehouse";
		this.gridColumn3.ColumnEdit = this.rep_WHouse;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "WhsCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 3;
		this.gridColumn3.Width = 108;
		this.rep_WHouse.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_WHouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WHouse.Name = "rep_WHouse";
		this.rep_WHouse.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WHouse_ButtonPressed);
		this.gridColumn4.Caption = "Address";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Address";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 4;
		this.gridColumn4.Width = 81;
		this.gridColumn5.Caption = "Email";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Email";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 5;
		this.gridColumn5.Width = 67;
		this.gridColumn6.Caption = "Customer";
		this.gridColumn6.ColumnEdit = this.rep_Customer;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "CardCode";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 6;
		this.gridColumn6.Width = 87;
		this.rep_Customer.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Customer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Customer.Name = "rep_Customer";
		this.rep_Customer.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Customer_ButtonPressed);
		this.gridColumn7.Caption = "Sale Person";
		this.gridColumn7.ColumnEdit = this.rep_SalePerson;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "SlpCode";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 7;
		this.gridColumn7.Width = 114;
		this.rep_SalePerson.AutoHeight = false;
		editorButtonImageOptions3.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_SalePerson.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_SalePerson.Name = "rep_SalePerson";
		this.rep_SalePerson.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_SalePerson_ButtonPressed);
		this.gridColumn8.Caption = "Price List";
		this.gridColumn8.ColumnEdit = this.rep_PriceList;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "ListNum";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 8;
		this.gridColumn8.Width = 94;
		this.rep_PriceList.AutoHeight = false;
		this.rep_PriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PriceList.DisplayMember = "Name";
		this.rep_PriceList.Name = "rep_PriceList";
		this.rep_PriceList.NullText = "";
		this.rep_PriceList.ValueMember = "Code";
		this.gridColumnEx1.Caption = "Locked";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Locked";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 9;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gridColumn9.Caption = "Enable OS Keyboard";
		this.gridColumn9.ColumnEdit = this.rep_Keyboard;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "OSKeyBoardEnable";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Width = 198;
		this.rep_Keyboard.AutoHeight = false;
		this.rep_Keyboard.Name = "rep_Keyboard";
		this.rep_Keyboard.ValueChecked = "Y";
		this.rep_Keyboard.ValueUnchecked = "N";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(975, 420);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oRetailStore";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_RetailStore";
		this.Text = "Store Configuration";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_RetailStore_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WHouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Customer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalePerson).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Keyboard).EndInit();
		base.ResumeLayout(false);
	}
}
