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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.DMS;

public class frm_Store : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isDuplicate;

	private int _newClgCode;

	private string _fatherCode = string.Empty;

	private int _levelParent;

	private IContainer components;

	private ContextMenuStrip cms_Store;

	private ToolStripMenuItem menu_AddDoc;

	private ToolStripMenuItem menu_DeleteDoc;

	private ContextMenuStrip cms_Events;

	private ToolStripMenuItem menu_AddNewEvents;

	private ToolStripMenuItem menu_RemoveEvents;

	private SimpleButtonEx btn_Search;

	private LookUpEditEx lk_Status;

	private LabelLine lbl_CardCode;

	private LabelLine lbl_Status;

	private LabelLine lbl_StoreCode;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private LabelControlEx lbl_EventActivity;

	private GridControlEx grd_Events;

	private GridViewEx gv_Events;

	private GridColumnEx gc_EventCode;

	private GridColumnEx gc_EventName;

	private GridColumnEx gc_ClgCode;

	private GridControlEx grd_Store;

	private GridViewEx gv_Store;

	private GridColumnEx gc_StoreCode;

	private GridColumnEx gc_StoreName;

	private GridColumnEx gc_Region;

	private GridColumnEx gc_Status;

	private RepositoryItemLookUpEdit glk_Status;

	private GridColumnEx gc_CardCode;

	private GridColumnEx gc_Address;

	private GridColumnEx gc_Remarks;

	private RepositoryItemButtonEditCFL rep_CardCodeCFL;

	private ButtonEditCFL btn_CardCode;

	private RepositoryItemButtonEditCFL btn_Events;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL1;

	private RepositoryItemButtonEditCFL btn_EditEvents;

	private RepositoryItemButtonEdit btn_ViewEvent;

	private ToolStripMenuItem menu_StoreDuplicate;

	private GridColumnEx gc_ECardCode;

	private GridColumnEx gc_FromDate;

	private GridColumnEx gc_ToDate;

	private GridColumnEx gc_AllDay;

	private RepositoryItemCheckEdit rep_AllDay;

	private GridColumnEx gc_Invite;

	private GridColumnEx gc_ERemark;

	private RepositoryItemButtonEditCFL rep_Region;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2;

	private GridColumnEx gc_City;

	private RepositoryItemButtonEditCFL rep_City;

	private GridColumnEx gc_District;

	private RepositoryItemButtonEditCFL rep_District;

	private GridColumnEx gc_Town;

	private RepositoryItemButtonEditCFL rep_Town;

	private GridColumnEx gc_Ward;

	private RepositoryItemButtonEditCFL rep_Ward;

	private ButtonEditCFL btn_StoreCode;

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
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			btn_Search.Enabled = true;
			break;
		default:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_Store()
	{
		InitializeComponent();
		OnLoadAsync += frm_Store_Load;
	}

	public frm_Store(string storeCode)
	{
		InitializeComponent();
		OnLoadAsync += frm_Store_Load;
	}

	public bool CheckTableExists()
	{
		try
		{
			return WinFuntions.GetTableStructure("APZ_D_STORE").Columns.Count > 0 || WinFuntions.GetTableStructure("APZ_D_STREV").Columns.Count > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public async Task LoadData()
	{
		_ = 1;
		try
		{
			if (!CheckTableExists())
			{
				btn_Search.Enabled = false;
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
				return;
			}
			btn_Search.Enabled = true;
			GridControlEx gridControlEx = grd_Store;
			gridControlEx.DataSource = await _httpClient.StoreClient.GetStore();
			grd_Store.RefreshDataSource();
			gridControlEx = grd_Events;
			gridControlEx.DataSource = await _httpClient.StoreClient.GetStoreEvents(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
			grd_Events.RefreshDataSource();
			_isDuplicate = false;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async void AddNewDoc()
	{
		try
		{
			DataTable dataTable = grd_Store.DataSourceTable.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Status"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Store.DataSource = dataTable;
			grd_Store.RefreshDataSource();
			GridControlEx gridControlEx = grd_Events;
			gridControlEx.DataSource = await _httpClient.StoreClient.GetStoreEvents("");
			grd_Events.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task NewEvents(string storeCode)
	{
		try
		{
			DataTable dtEvents = grd_Events.DataSourceTable;
			frm_EventManagement frm = new frm_EventManagement();
			try
			{
				frm.ShowDialog(this);
				if (frm.ClgCode <= 0)
				{
					return;
				}
				DataTable dataTable = await _httpClient.StoreClient.GetEvents(frm.ClgCode);
				if (dataTable.IsNotNull())
				{
					dataTable.Rows[0]["Code"] = storeCode;
					dataTable.AcceptChanges();
					DataRow dataRow = dtEvents.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["ClgCode"]) == frm.ClgCode);
					if (dataRow != null)
					{
						dtEvents.Rows.Remove(dataRow);
						dtEvents.AcceptChanges();
					}
					DataRow dataRow2 = dtEvents.NewRow();
					dataRow2.ItemArray = dataTable.Rows[0].ItemArray;
					dtEvents.Rows.Add(dataRow2);
					dtEvents.AcceptChanges();
					grd_Events.RefreshDataSource();
					if (FormMode != Enums.FormViewModeType.Add)
					{
						FormMode = Enums.FormViewModeType.Update;
					}
				}
			}
			finally
			{
				if (frm != null)
				{
					((IDisposable)frm).Dispose();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadEvents(string storeCode)
	{
		try
		{
			DataTable dataSource = await _httpClient.StoreClient.GetStoreEvents(storeCode);
			grd_Events.DataSource = dataSource;
			grd_Events.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task SaveData(DataSet ds, string type)
	{
		_ = 1;
		try
		{
			if (ds == null || ds.Tables.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			Tuple<DataSet, string> tuple = new Tuple<DataSet, string>(ds, type);
			WinCommons._OpenProcessing();
			HttpResult httpResult = await _httpClient.StoreClient.Save(tuple);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<string> RemoveDoc(string storeCode)
	{
		_ = 1;
		try
		{
			Tuple<string> tuple = new Tuple<string>(storeCode);
			HttpResult httpResult = await _httpClient.StoreClient.CheckRemove(storeCode);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			HttpResult httpResult2 = await _httpClient.StoreClient.Remove(tuple);
			if (!string.IsNullOrEmpty(httpResult2.Message))
			{
				return httpResult2.Message;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public async void GetRecord(Enums.NavigateType type, string storeCode)
	{
		_ = 1;
		try
		{
			string position = string.Empty;
			switch (type)
			{
			case Enums.NavigateType.First:
				position = "F";
				break;
			case Enums.NavigateType.Last:
				position = "L";
				break;
			case Enums.NavigateType.Previous:
				position = "P";
				break;
			case Enums.NavigateType.Next:
				position = "N";
				break;
			}
			DataTable dataTable = await _httpClient.StoreClient.GetRecord(position, storeCode);
			if (dataTable.IsNotNull())
			{
				grd_Store.DataSource = dataTable;
				grd_Store.RefreshDataSource();
				await LoadEvents(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void ClearData(int rowHandle, int level)
	{
		try
		{
			switch (level)
			{
			case -1:
				gv_Store.SetRowCellValue(rowHandle, "Region", "");
				gv_Store.SetRowCellValue(rowHandle, "City", "");
				gv_Store.SetRowCellValue(rowHandle, "District", "");
				gv_Store.SetRowCellValue(rowHandle, "Ward", "");
				gv_Store.SetRowCellValue(rowHandle, "Town", "");
				break;
			case 0:
				gv_Store.SetRowCellValue(rowHandle, "City", "");
				gv_Store.SetRowCellValue(rowHandle, "District", "");
				gv_Store.SetRowCellValue(rowHandle, "Ward", "");
				gv_Store.SetRowCellValue(rowHandle, "Town", "");
				break;
			case 1:
				gv_Store.SetRowCellValue(rowHandle, "District", "");
				gv_Store.SetRowCellValue(rowHandle, "Ward", "");
				gv_Store.SetRowCellValue(rowHandle, "Town", "");
				break;
			case 2:
				gv_Store.SetRowCellValue(rowHandle, "Ward", "");
				gv_Store.SetRowCellValue(rowHandle, "Town", "");
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	public async Task LoadCfl(string parentCode, int parentLevel, int level)
	{
		try
		{
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("parentCode", parentCode));
			list.Add(new Common.ParameterValue("parentLevel", parentLevel));
			list.Add(new Common.ParameterValue("level", level));
			using frm_CFL frm = new frm_CFL(Enums.TypeCFL.StoreGeography, false, "", "", Enums.ItemObjectType.NoItemType, list, "");
			frm.ShowDialog(this);
			if (frm.ChooseItem == null)
			{
				return;
			}
			int rowHandle = gv_Store.FocusedRowHandle;
			string text = Function.ToString(frm.ChooseItem["Code"]);
			if (level == -1)
			{
				ClearData(rowHandle, -1);
				gv_Store.SetRowCellValue(rowHandle, "Region", text);
				_fatherCode = text;
				_levelParent = -1;
			}
			DataTable dataTable = await _httpClient.StoreClient.GetParentsGeography(text);
			if (dataTable.IsNotNull())
			{
				gv_Store.SetRowCellValue(rowHandle, "Region", dataTable.Rows[0]["CountryCode"]);
				switch (level)
				{
				case 0:
					ClearData(rowHandle, 0);
					gv_Store.SetRowCellValue(rowHandle, "City", dataTable.Rows[0]["Code"]);
					_fatherCode = Function.ToString(dataTable.Rows[0]["Code"]);
					_levelParent = 0;
					break;
				case 1:
					ClearData(rowHandle, 1);
					gv_Store.SetRowCellValue(rowHandle, "City", dataTable.Rows[0]["Code"]);
					gv_Store.SetRowCellValue(rowHandle, "District", dataTable.Rows[1]["Code"]);
					_fatherCode = Function.ToString(dataTable.Rows[1]["Code"]);
					_levelParent = 1;
					break;
				case 2:
					ClearData(rowHandle, 2);
					gv_Store.SetRowCellValue(rowHandle, "City", dataTable.Rows[0]["Code"]);
					gv_Store.SetRowCellValue(rowHandle, "District", dataTable.Rows[1]["Code"]);
					gv_Store.SetRowCellValue(rowHandle, "Ward", dataTable.Rows[2]["Code"]);
					_fatherCode = Function.ToString(dataTable.Rows[2]["Code"]);
					_levelParent = 2;
					break;
				case 3:
					ClearData(rowHandle, 3);
					gv_Store.SetRowCellValue(rowHandle, "City", dataTable.Rows[0]["Code"]);
					gv_Store.SetRowCellValue(rowHandle, "District", dataTable.Rows[1]["Code"]);
					gv_Store.SetRowCellValue(rowHandle, "Ward", dataTable.Rows[2]["Code"]);
					gv_Store.SetRowCellValue(rowHandle, "Town", dataTable.Rows[3]["Code"]);
					_fatherCode = Function.ToString(dataTable.Rows[3]["Code"]);
					_levelParent = 3;
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

	public void LoadGeography(string countryCode, string ggpCode, int level)
	{
		try
		{
			if ((level == -1 && string.IsNullOrEmpty(countryCode)) || (level > -1 && string.IsNullOrEmpty(ggpCode)))
			{
				return;
			}
			WinCommons._OpenProcessing();
			frm_Geography frm_Geography2 = new frm_Geography(countryCode, ggpCode);
			frm_Geography2.MdiParent = MdiParent;
			frm_Geography2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			AddNewDoc();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.First, Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Previous, Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Next, Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			WinCommons._OpenProcessing();
			GetRecord(Enums.NavigateType.Last, Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
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

	private async Task frm_Store_Load(object sender, EventArgs e)
	{
		Dictionary<string, string> dic = new Dictionary<string, string>
		{
			{ "Y", "Active" },
			{ "N", "Inactive" }
		};
		lk_Status.SetDataSource(dic);
		glk_Status.SetDataSource(dic);
		await LoadData();
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewDoc();
		FormMode = Enums.FormViewModeType.Add;
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataSet dataSet = new DataSet();
			DataTable dataTable = gv_Store.GetListDataSourceSelectedRow().CopyToDataTable();
			dataTable.TableName = "APZ_D_STORE";
			DataTable dataSourceTable = grd_Events.DataSourceTable;
			if (dataSourceTable.Columns.Contains("Name") && dataSourceTable.Columns.Contains("CardCode") && dataSourceTable.Columns.Contains("Recontact") && dataSourceTable.Columns.Contains("endDate") && dataSourceTable.Columns.Contains("AllDayEvent") && dataSourceTable.Columns.Contains("GroupUser") && dataSourceTable.Columns.Contains("Invite"))
			{
				dataSourceTable.Columns.Remove("Name");
				dataSourceTable.Columns.Remove("CardCode");
				dataSourceTable.Columns.Remove("Recontact");
				dataSourceTable.Columns.Remove("endDate");
				dataSourceTable.Columns.Remove("AllDayEvent");
				dataSourceTable.Columns.Remove("GroupUser");
				dataSourceTable.Columns.Remove("Invite");
			}
			dataSourceTable.TableName = "APZ_D_STREV";
			if (dataSet.Tables.Contains("APZ_D_STORE") && dataSet.Tables.Contains("APZ_D_STREV"))
			{
				dataSet.Tables.Clear();
			}
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataSourceTable.Copy());
			dataSet.AcceptChanges();
			if (XtraMessageBoxEx.Show((FormMode == Enums.FormViewModeType.Add) ? Enums.MessageCode.ConfirmSave : Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveData(dataSet, (FormMode == Enums.FormViewModeType.Add) ? "A" : "U");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Events_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gv_Store_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_Store.GetRowCellValue(e.RowHandle, "Code"));
			if (string.Equals(e.Column.Name, gc_StoreCode.Name) && !string.IsNullOrEmpty(value))
			{
				DataTable dataSourceTable = grd_Events.DataSourceTable;
				if (dataSourceTable.IsNotNull())
				{
					foreach (DataRow row in dataSourceTable.Rows)
					{
						row["Code"] = value;
					}
					dataSourceTable.AcceptChanges();
				}
			}
			if (FormMode != Enums.FormViewModeType.Add)
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

	private async void gv_Store_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_ = 1;
		try
		{
			await LoadEvents(Function.ToString(gv_Store.GetRowCellValue(e.FocusedRowHandle, "Code")));
			if (grd_Events.DataSourceTable.IsNotNull())
			{
				DataTable dataTable = await _httpClient.StoreClient.GetLastEvent(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
				if (dataTable.IsNotNull())
				{
					_newClgCode = Function.ParseInt(dataTable.Rows[0]["ClgCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
			}
			WinCommons._OpenProcessing();
			DataTable dataSource = await _httpClient.StoreClient.GetStore(Function.ToString(btn_StoreCode.EditValue), btn_CardCode.Text, Function.ToString(lk_Status.EditValue));
			grd_Store.DataSource = dataSource;
			grd_Store.RefreshDataSource();
			await LoadEvents(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void menu_AddNewEvents_Click(object sender, EventArgs e)
	{
		try
		{
			await NewEvents(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_Store_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
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

	private async void menu_DeleteDoc_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				string text = await RemoveDoc(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Store_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(e.Column.Name, gc_StoreCode.Name))
			{
				if (FormMode == Enums.FormViewModeType.Add)
				{
					e.Appearance.BackColor = Color.Empty;
				}
				else
				{
					e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Store_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && string.Equals(gv_Store.FocusedColumn.Name, gc_StoreCode.Name))
			{
				if (_isDuplicate)
				{
					e.Cancel = false;
				}
				else
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Events_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(e.Column.Name, gc_EventCode.Name))
			{
				e.Appearance.BackColor = Color.FromArgb(231, 231, 231);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Events_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(gv_Events.FocusedColumn.Name, gc_EventCode.Name))
			{
				e.Cancel = true;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				DataTable dataTable = await _httpClient.StoreClient.GetLastEvent(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Code")));
				if (dataTable.IsNotNull())
				{
					Function.ParseInt(dataTable.Rows[0]["ClgCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Btn_EditEvents_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			if (e.Button.Index == 1)
			{
				frm_EventManagement frm_EventManagement2 = new frm_EventManagement();
				frm_EventManagement2.MdiParent = MdiParent;
				frm_EventManagement2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void menu_RemoveEvents_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Events.DataSourceTable;
			foreach (DataRow item in gv_Events.GetListDataSourceSelectedRow())
			{
				item.Delete();
			}
			dataSourceTable.AcceptChanges();
			if (FormMode != Enums.FormViewModeType.Add)
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

	private async void menu_StoreDuplicate_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = grd_Store.DataSourceTable.Clone();
			if (dataTable.Columns.Count == 0)
			{
				return;
			}
			DataRow focusedDataRow = gv_Store.GetFocusedDataRow();
			if (focusedDataRow != null)
			{
				dataTable = dataTable.Clone();
				focusedDataRow["Code"] = string.Empty;
				focusedDataRow["Name"] = string.Empty;
				dataTable.Rows.Add(focusedDataRow.ItemArray);
			}
			dataTable.AcceptChanges();
			grd_Store.DataSource = dataTable;
			grd_Store.RefreshDataSource();
			GridControlEx gridControlEx = grd_Events;
			gridControlEx.DataSource = await _httpClient.StoreClient.GetStoreEvents("");
			grd_Events.RefreshDataSource();
			_isDuplicate = true;
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void Rep_Region_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				await LoadCfl("", -1, -1);
			}
			else
			{
				LoadGeography(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gc_Region)), "", -1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Rep_Town_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Ward"));
				if (!string.IsNullOrEmpty(text))
				{
					_fatherCode = text;
					_levelParent = 2;
				}
				await LoadCfl(_fatherCode, _levelParent, 3);
			}
			else
			{
				LoadGeography(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gc_Region)), Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gv_Store.FocusedColumn)), 3);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Rep_District_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "City"));
				if (!string.IsNullOrEmpty(text))
				{
					_fatherCode = text;
					_levelParent = 0;
				}
				await LoadCfl(_fatherCode, _levelParent, 1);
			}
			else
			{
				LoadGeography(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gc_Region)), Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gv_Store.FocusedColumn)), 1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Rep_City_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "Region"));
				if (!string.IsNullOrEmpty(text))
				{
					_fatherCode = text;
					_levelParent = -1;
				}
				await LoadCfl(_fatherCode, _levelParent, 0);
			}
			else
			{
				LoadGeography(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gc_Region)), Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gv_Store.FocusedColumn)), 0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Rep_Ward_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string text = Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, "District"));
				if (!string.IsNullOrEmpty(text))
				{
					_fatherCode = text;
					_levelParent = 1;
				}
				await LoadCfl(_fatherCode, _levelParent, 2);
			}
			else
			{
				LoadGeography(Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gc_Region)), Function.ToString(gv_Store.GetRowCellValue(gv_Store.FocusedRowHandle, gv_Store.FocusedColumn)), 2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_StoreCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			using frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Store, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				btn_StoreCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Events_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				return;
			}
			e.Appearance.BackColor = StyleColor.GridDisable;
			e.HighPriority = true;
			for (int i = 0; i < grd_Events.DataSourceTable.Rows.Count; i++)
			{
				if (Function.ParseInt(gv_Events.GetRowCellValue(i, "ClgCode")) == _newClgCode && e.RowHandle == i)
				{
					e.Appearance.BackColor = Color.Empty;
					e.HighPriority = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Store_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			DataTable dataSourceTable = grd_Store.DataSourceTable;
			if (dataSourceTable.Columns.Count > 0)
			{
				switch (FormMode)
				{
				case Enums.FormViewModeType.Add:
					menu_AddDoc.Enabled = false;
					menu_DeleteDoc.Enabled = false;
					menu_StoreDuplicate.Enabled = true;
					break;
				case Enums.FormViewModeType.Update:
					menu_AddDoc.Enabled = true;
					menu_DeleteDoc.Enabled = true;
					menu_StoreDuplicate.Enabled = true;
					break;
				case Enums.FormViewModeType.View:
					menu_AddDoc.Enabled = true;
					menu_DeleteDoc.Enabled = dataSourceTable.IsNotNull();
					menu_StoreDuplicate.Enabled = dataSourceTable.IsNotNull();
					break;
				}
			}
			else
			{
				menu_AddDoc.Enabled = false;
				menu_DeleteDoc.Enabled = false;
				menu_StoreDuplicate.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void gv_Events_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button != MouseButtons.Right)
			{
				return;
			}
			DataTable dataSourceTable = grd_Store.DataSourceTable;
			DataTable dataSourceTable2 = grd_Events.DataSourceTable;
			if (dataSourceTable2.Columns.Count > 0)
			{
				switch (FormMode)
				{
				case Enums.FormViewModeType.Add:
					menu_AddNewEvents.Enabled = true;
					menu_RemoveEvents.Enabled = dataSourceTable2.IsNotNull();
					break;
				case Enums.FormViewModeType.Update:
					menu_AddNewEvents.Enabled = true;
					menu_RemoveEvents.Enabled = dataSourceTable2.IsNotNull();
					break;
				case Enums.FormViewModeType.View:
					menu_AddNewEvents.Enabled = dataSourceTable.IsNotNull();
					menu_RemoveEvents.Enabled = dataSourceTable2.IsNotNull();
					break;
				}
			}
			else
			{
				menu_AddNewEvents.Enabled = false;
				menu_RemoveEvents.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_Store));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.cms_Store = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_DeleteDoc = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_StoreDuplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.cms_Events = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menu_AddNewEvents = new System.Windows.Forms.ToolStripMenuItem();
		this.menu_RemoveEvents = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.lbl_CardCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Status = new Apzon.Controls.Controls.LabelLine();
		this.lbl_StoreCode = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Store = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Store = new Apzon.Controls.Grid.GridViewEx();
		this.gc_StoreCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_StoreName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Region = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Region = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_City = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_City = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_District = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_District = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Ward = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Ward = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Town = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Town = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.glk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_CardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CardCodeCFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Address = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEditCFL2 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.lbl_EventActivity = new Apzon.Controls.Controls.LabelControlEx();
		this.grd_Events = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Events = new Apzon.Controls.Grid.GridViewEx();
		this.gc_EventCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EventName = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_EditEvents = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ECardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AllDay = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AllDay = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gc_Invite = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ERemark = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Events = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.repositoryItemButtonEditCFL1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.btn_ViewEvent = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CardCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_StoreCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.cms_Store.SuspendLayout();
		this.cms_Events.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Store).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Store).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Region).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_City).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_District).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ward).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Town).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.glk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCodeCFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Events).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Events).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EditEvents).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AllDay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Events).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ViewEvent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_CardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_StoreCode.Properties).BeginInit();
		base.SuspendLayout();
		this.cms_Store.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.menu_AddDoc, this.menu_DeleteDoc, this.menu_StoreDuplicate });
		this.cms_Store.Name = "cms_Store";
		this.cms_Store.Size = new System.Drawing.Size(125, 70);
		this.menu_AddDoc.Name = "menu_AddDoc";
		this.menu_AddDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_AddDoc.Text = "Add New";
		this.menu_AddDoc.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.menu_DeleteDoc.Name = "menu_DeleteDoc";
		this.menu_DeleteDoc.Size = new System.Drawing.Size(124, 22);
		this.menu_DeleteDoc.Text = "Remove";
		this.menu_DeleteDoc.Click += new System.EventHandler(this.menu_DeleteDoc_Click);
		this.menu_StoreDuplicate.Name = "menu_StoreDuplicate";
		this.menu_StoreDuplicate.Size = new System.Drawing.Size(124, 22);
		this.menu_StoreDuplicate.Text = "Duplicate";
		this.menu_StoreDuplicate.Click += new System.EventHandler(this.menu_StoreDuplicate_Click);
		this.cms_Events.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menu_AddNewEvents, this.menu_RemoveEvents });
		this.cms_Events.Name = "cms_Events";
		this.cms_Events.Size = new System.Drawing.Size(131, 48);
		this.menu_AddNewEvents.Name = "menu_AddNewEvents";
		this.menu_AddNewEvents.Size = new System.Drawing.Size(130, 22);
		this.menu_AddNewEvents.Text = "New Event";
		this.menu_AddNewEvents.Click += new System.EventHandler(this.menu_AddNewEvents_Click);
		this.menu_RemoveEvents.Name = "menu_RemoveEvents";
		this.menu_RemoveEvents.Size = new System.Drawing.Size(130, 22);
		this.menu_RemoveEvents.Text = "Remove";
		this.menu_RemoveEvents.Click += new System.EventHandler(this.menu_RemoveEvents_Click);
		this.btn_Search.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Search.Appearance.Options.UseFont = true;
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(270, 27);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(75, 20);
		this.btn_Search.TabIndex = 4;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lk_Status.FieldName = null;
		this.lk_Status.IsSearchControl = false;
		this.lk_Status.Location = new System.Drawing.Point(105, 27);
		this.lk_Status.Name = "lk_Status";
		this.lk_Status.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.Appearance.Options.UseFont = true;
		this.lk_Status.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.lk_Status.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.lk_Status.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lk_Status.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.lk_Status.Properties.AppearanceFocused.Options.UseFont = true;
		this.lk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lk_Status.Properties.NullText = "";
		this.lk_Status.Properties.ShowHeader = false;
		this.lk_Status.Size = new System.Drawing.Size(150, 20);
		this.lk_Status.TabIndex = 3;
		this.lbl_CardCode.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_CardCode.Appearance.Options.UseFont = true;
		this.lbl_CardCode.FieldName = null;
		this.lbl_CardCode.HideWhenEmpty = false;
		this.lbl_CardCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CardCode.Location = new System.Drawing.Point(270, 8);
		this.lbl_CardCode.Name = "lbl_CardCode";
		this.lbl_CardCode.ShowUnderLine = true;
		this.lbl_CardCode.Size = new System.Drawing.Size(94, 20);
		this.lbl_CardCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CardCode.TabIndex = 0;
		this.lbl_CardCode.Utext = "Card Code";
		this.lbl_Status.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_Status.Appearance.Options.UseFont = true;
		this.lbl_Status.FieldName = null;
		this.lbl_Status.HideWhenEmpty = false;
		this.lbl_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Status.Location = new System.Drawing.Point(7, 30);
		this.lbl_Status.Name = "lbl_Status";
		this.lbl_Status.ShowUnderLine = true;
		this.lbl_Status.Size = new System.Drawing.Size(99, 20);
		this.lbl_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Status.TabIndex = 0;
		this.lbl_Status.Utext = "Status";
		this.lbl_StoreCode.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_StoreCode.Appearance.Options.UseFont = true;
		this.lbl_StoreCode.FieldName = null;
		this.lbl_StoreCode.HideWhenEmpty = false;
		this.lbl_StoreCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_StoreCode.Location = new System.Drawing.Point(7, 7);
		this.lbl_StoreCode.Name = "lbl_StoreCode";
		this.lbl_StoreCode.ShowUnderLine = true;
		this.lbl_StoreCode.Size = new System.Drawing.Size(99, 20);
		this.lbl_StoreCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_StoreCode.TabIndex = 0;
		this.lbl_StoreCode.Utext = "Store Code";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(7, 52);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Store);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.lbl_EventActivity);
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Events);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(857, 392);
		this.splitContainerControl1.SplitterPosition = 183;
		this.splitContainerControl1.TabIndex = 21;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Store.AllowPasteExcel = true;
		this.grd_Store.ClipboardData = null;
		this.grd_Store.ContextMenuStrip = this.cms_Store;
		this.grd_Store.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Store.Location = new System.Drawing.Point(0, 0);
		this.grd_Store.MainView = this.gv_Store;
		this.grd_Store.Name = "grd_Store";
		this.grd_Store.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.glk_Status, this.rep_CardCodeCFL, this.repositoryItemButtonEditCFL2, this.rep_Region, this.rep_City, this.rep_District, this.rep_Town, this.rep_Ward });
		this.grd_Store.Size = new System.Drawing.Size(857, 183);
		this.grd_Store.TabIndex = 5;
		this.grd_Store.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Store });
		this.gv_Store.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Store.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Store.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Store.ColumnKey = null;
		this.gv_Store.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gc_StoreCode, this.gc_StoreName, this.gc_Region, this.gc_City, this.gc_District, this.gc_Ward, this.gc_Town, this.gc_Status, this.gc_CardCode, this.gc_Address,
			this.gc_Remarks
		});
		this.gv_Store.DisableFormSetting = false;
		this.gv_Store.DisplayException = false;
		this.gv_Store.EditColumn = null;
		this.gv_Store.FieldCurrency = null;
		this.gv_Store.FieldRate = null;
		this.gv_Store.FormId = "oStore";
		this.gv_Store.GridControl = this.grd_Store;
		this.gv_Store.IndexEditRow = -1;
		this.gv_Store.IndicatorWidth = 35;
		this.gv_Store.IsApzTable = false;
		this.gv_Store.IsInsertNewRow = false;
		this.gv_Store.IsLoadingData = false;
		this.gv_Store.ItemId = "APZ_D_STORE";
		this.gv_Store.LstColumnAllowBatch = null;
		this.gv_Store.LstColumnNotAllowBatch = null;
		this.gv_Store.LstColumNotSetting = null;
		this.gv_Store.Name = "gv_Store";
		this.gv_Store.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Store.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Store.OptionsPrint.AutoWidth = false;
		this.gv_Store.OptionsPrint.UsePrintStyles = false;
		this.gv_Store.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Store.OptionsView.ColumnAutoWidth = false;
		this.gv_Store.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Store.OptionsView.ShowGroupedColumns = true;
		this.gv_Store.OptionsView.ShowGroupPanel = false;
		this.gv_Store.UseBatchMod = false;
		this.gv_Store.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Store_RowCellStyle);
		this.gv_Store.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Store_ShowingEditor);
		this.gv_Store.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Store_FocusedRowChanged);
		this.gv_Store.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Store_CellValueChanged);
		this.gv_Store.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Store_MouseDown);
		this.gc_StoreCode.Caption = "Store Code";
		this.gc_StoreCode.CurrencyDateField = null;
		this.gc_StoreCode.CurrencyField = null;
		this.gc_StoreCode.CurrencyFixed = null;
		this.gc_StoreCode.CurrencyFixedRate = 0.0;
		this.gc_StoreCode.CurrencyRateField = null;
		this.gc_StoreCode.FieldName = "Code";
		this.gc_StoreCode.Name = "gc_StoreCode";
		this.gc_StoreCode.PrimaryKey = false;
		this.gc_StoreCode.UnitField = null;
		this.gc_StoreCode.Visible = true;
		this.gc_StoreCode.VisibleIndex = 0;
		this.gc_StoreName.Caption = "Store Name";
		this.gc_StoreName.CurrencyDateField = null;
		this.gc_StoreName.CurrencyField = null;
		this.gc_StoreName.CurrencyFixed = null;
		this.gc_StoreName.CurrencyFixedRate = 0.0;
		this.gc_StoreName.CurrencyRateField = null;
		this.gc_StoreName.FieldName = "Name";
		this.gc_StoreName.Name = "gc_StoreName";
		this.gc_StoreName.PrimaryKey = false;
		this.gc_StoreName.UnitField = null;
		this.gc_StoreName.Visible = true;
		this.gc_StoreName.VisibleIndex = 1;
		this.gc_Region.Caption = "Region";
		this.gc_Region.ColumnEdit = this.rep_Region;
		this.gc_Region.CurrencyDateField = null;
		this.gc_Region.CurrencyField = null;
		this.gc_Region.CurrencyFixed = null;
		this.gc_Region.CurrencyFixedRate = 0.0;
		this.gc_Region.CurrencyRateField = null;
		this.gc_Region.FieldName = "Region";
		this.gc_Region.Name = "gc_Region";
		this.gc_Region.PrimaryKey = false;
		this.gc_Region.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Region.UnitField = null;
		this.gc_Region.Visible = true;
		this.gc_Region.VisibleIndex = 2;
		this.rep_Region.AutoHeight = false;
		this.rep_Region.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Region.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions)
		});
		this.rep_Region.ContainerControl = this;
		this.rep_Region.DisplayMember = "Code";
		this.rep_Region.DisplayMember2 = "";
		this.rep_Region.LoadDataSourceWhenEnter = false;
		this.rep_Region.Name = "rep_Region";
		this.rep_Region.SeparateCharacterForMulti = ';';
		this.rep_Region.SeparateString = ", ";
		this.rep_Region.UsingAutoCompleteBox = false;
		this.rep_Region.UsingDataSource = true;
		this.rep_Region.ValidateWhenNoDataSource = false;
		this.rep_Region.ValueMember = "Code";
		this.rep_Region.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Rep_Region_ButtonClick);
		this.gc_City.Caption = "City";
		this.gc_City.ColumnEdit = this.rep_City;
		this.gc_City.CurrencyDateField = null;
		this.gc_City.CurrencyField = null;
		this.gc_City.CurrencyFixed = null;
		this.gc_City.CurrencyFixedRate = 0.0;
		this.gc_City.CurrencyRateField = null;
		this.gc_City.FieldName = "City";
		this.gc_City.Name = "gc_City";
		this.gc_City.PrimaryKey = false;
		this.gc_City.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_City.UnitField = null;
		this.gc_City.Visible = true;
		this.gc_City.VisibleIndex = 3;
		this.rep_City.AutoHeight = false;
		this.rep_City.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_City.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2)
		});
		this.rep_City.ContainerControl = this;
		this.rep_City.DisplayMember = "Code";
		this.rep_City.DisplayMember2 = "";
		this.rep_City.LoadDataSourceWhenEnter = false;
		this.rep_City.Name = "rep_City";
		this.rep_City.SeparateCharacterForMulti = ';';
		this.rep_City.SeparateString = ", ";
		this.rep_City.UsingAutoCompleteBox = false;
		this.rep_City.UsingDataSource = true;
		this.rep_City.ValidateWhenNoDataSource = false;
		this.rep_City.ValueMember = "Code";
		this.rep_City.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Rep_City_ButtonClick);
		this.gc_District.Caption = "District";
		this.gc_District.ColumnEdit = this.rep_District;
		this.gc_District.CurrencyDateField = null;
		this.gc_District.CurrencyField = null;
		this.gc_District.CurrencyFixed = null;
		this.gc_District.CurrencyFixedRate = 0.0;
		this.gc_District.CurrencyRateField = null;
		this.gc_District.FieldName = "District";
		this.gc_District.Name = "gc_District";
		this.gc_District.PrimaryKey = false;
		this.gc_District.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_District.UnitField = null;
		this.gc_District.Visible = true;
		this.gc_District.VisibleIndex = 4;
		this.rep_District.AutoHeight = false;
		this.rep_District.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rep_District.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3)
		});
		this.rep_District.ContainerControl = this;
		this.rep_District.DisplayMember = "Code";
		this.rep_District.DisplayMember2 = "";
		this.rep_District.LoadDataSourceWhenEnter = false;
		this.rep_District.Name = "rep_District";
		this.rep_District.SeparateCharacterForMulti = ';';
		this.rep_District.SeparateString = ", ";
		this.rep_District.UsingAutoCompleteBox = false;
		this.rep_District.UsingDataSource = true;
		this.rep_District.ValidateWhenNoDataSource = false;
		this.rep_District.ValueMember = "Code";
		this.rep_District.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Rep_District_ButtonClick);
		this.gc_Ward.Caption = "Ward";
		this.gc_Ward.ColumnEdit = this.rep_Ward;
		this.gc_Ward.CurrencyDateField = null;
		this.gc_Ward.CurrencyField = null;
		this.gc_Ward.CurrencyFixed = null;
		this.gc_Ward.CurrencyFixedRate = 0.0;
		this.gc_Ward.CurrencyRateField = null;
		this.gc_Ward.FieldName = "Ward";
		this.gc_Ward.Name = "gc_Ward";
		this.gc_Ward.PrimaryKey = false;
		this.gc_Ward.UnitField = null;
		this.gc_Ward.Visible = true;
		this.gc_Ward.VisibleIndex = 5;
		this.rep_Ward.AutoHeight = false;
		this.rep_Ward.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.rep_Ward.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4)
		});
		this.rep_Ward.ContainerControl = this;
		this.rep_Ward.DisplayMember = "Code";
		this.rep_Ward.DisplayMember2 = "";
		this.rep_Ward.LoadDataSourceWhenEnter = false;
		this.rep_Ward.Name = "rep_Ward";
		this.rep_Ward.SeparateCharacterForMulti = ';';
		this.rep_Ward.SeparateString = ", ";
		this.rep_Ward.UsingAutoCompleteBox = false;
		this.rep_Ward.UsingDataSource = true;
		this.rep_Ward.ValidateWhenNoDataSource = false;
		this.rep_Ward.ValueMember = "Code";
		this.rep_Ward.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Rep_Ward_ButtonClick);
		this.gc_Town.Caption = "Town";
		this.gc_Town.ColumnEdit = this.rep_Town;
		this.gc_Town.CurrencyDateField = null;
		this.gc_Town.CurrencyField = null;
		this.gc_Town.CurrencyFixed = null;
		this.gc_Town.CurrencyFixedRate = 0.0;
		this.gc_Town.CurrencyRateField = null;
		this.gc_Town.FieldName = "Town";
		this.gc_Town.Name = "gc_Town";
		this.gc_Town.PrimaryKey = false;
		this.gc_Town.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Town.UnitField = null;
		this.gc_Town.Visible = true;
		this.gc_Town.VisibleIndex = 6;
		this.rep_Town.AutoHeight = false;
		this.rep_Town.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.rep_Town.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5)
		});
		this.rep_Town.ContainerControl = this;
		this.rep_Town.DisplayMember = "Code";
		this.rep_Town.DisplayMember2 = "";
		this.rep_Town.LoadDataSourceWhenEnter = false;
		this.rep_Town.Name = "rep_Town";
		this.rep_Town.SeparateCharacterForMulti = ';';
		this.rep_Town.SeparateString = ", ";
		this.rep_Town.UsingAutoCompleteBox = false;
		this.rep_Town.UsingDataSource = true;
		this.rep_Town.ValidateWhenNoDataSource = false;
		this.rep_Town.ValueMember = "Code";
		this.rep_Town.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Rep_Town_ButtonClick);
		this.gc_Status.Caption = "Status";
		this.gc_Status.ColumnEdit = this.glk_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 7;
		this.glk_Status.AutoHeight = false;
		this.glk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.glk_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.glk_Status.DisplayMember = "Name";
		this.glk_Status.Name = "glk_Status";
		this.glk_Status.NullText = "";
		this.glk_Status.ShowHeader = false;
		this.glk_Status.ValueMember = "Code";
		this.gc_CardCode.Caption = "Card Code";
		this.gc_CardCode.ColumnEdit = this.rep_CardCodeCFL;
		this.gc_CardCode.CurrencyDateField = null;
		this.gc_CardCode.CurrencyField = null;
		this.gc_CardCode.CurrencyFixed = null;
		this.gc_CardCode.CurrencyFixedRate = 0.0;
		this.gc_CardCode.CurrencyRateField = null;
		this.gc_CardCode.FieldName = "CardCode";
		this.gc_CardCode.Name = "gc_CardCode";
		this.gc_CardCode.PrimaryKey = false;
		this.gc_CardCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_CardCode.UnitField = null;
		this.gc_CardCode.Visible = true;
		this.gc_CardCode.VisibleIndex = 8;
		this.rep_CardCodeCFL.AutoHeight = false;
		this.rep_CardCodeCFL.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.rep_CardCodeCFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6)
		});
		this.rep_CardCodeCFL.ContainerControl = this;
		this.rep_CardCodeCFL.DisplayMember = "CardCode";
		this.rep_CardCodeCFL.DisplayMember2 = "";
		this.rep_CardCodeCFL.LoadDataSourceWhenEnter = false;
		this.rep_CardCodeCFL.Name = "rep_CardCodeCFL";
		this.rep_CardCodeCFL.SeparateCharacterForMulti = ';';
		this.rep_CardCodeCFL.SeparateString = ", ";
		this.rep_CardCodeCFL.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.rep_CardCodeCFL.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.rep_CardCodeCFL.UsingAutoCompleteBox = false;
		this.rep_CardCodeCFL.UsingDataSource = true;
		this.rep_CardCodeCFL.ValidateWhenNoDataSource = false;
		this.rep_CardCodeCFL.ValueMember = "CardCode";
		this.gc_Address.Caption = "Address";
		this.gc_Address.CurrencyDateField = null;
		this.gc_Address.CurrencyField = null;
		this.gc_Address.CurrencyFixed = null;
		this.gc_Address.CurrencyFixedRate = 0.0;
		this.gc_Address.CurrencyRateField = null;
		this.gc_Address.FieldName = "Address";
		this.gc_Address.Name = "gc_Address";
		this.gc_Address.PrimaryKey = false;
		this.gc_Address.UnitField = null;
		this.gc_Address.Visible = true;
		this.gc_Address.VisibleIndex = 9;
		this.gc_Remarks.Caption = "Remarks";
		this.gc_Remarks.CurrencyDateField = null;
		this.gc_Remarks.CurrencyField = null;
		this.gc_Remarks.CurrencyFixed = null;
		this.gc_Remarks.CurrencyFixedRate = 0.0;
		this.gc_Remarks.CurrencyRateField = null;
		this.gc_Remarks.FieldName = "Remarks";
		this.gc_Remarks.Name = "gc_Remarks";
		this.gc_Remarks.PrimaryKey = false;
		this.gc_Remarks.UnitField = null;
		this.gc_Remarks.Visible = true;
		this.gc_Remarks.VisibleIndex = 10;
		this.repositoryItemButtonEditCFL2.AutoHeight = false;
		this.repositoryItemButtonEditCFL2.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions16.Image");
		this.repositoryItemButtonEditCFL2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7)
		});
		this.repositoryItemButtonEditCFL2.ContainerControl = this;
		this.repositoryItemButtonEditCFL2.DisplayMember = "";
		this.repositoryItemButtonEditCFL2.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL2.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL2.Name = "repositoryItemButtonEditCFL2";
		this.repositoryItemButtonEditCFL2.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL2.SeparateString = ", ";
		this.repositoryItemButtonEditCFL2.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL2.UsingDataSource = true;
		this.repositoryItemButtonEditCFL2.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL2.ValueMember = "";
		this.lbl_EventActivity.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lbl_EventActivity.Appearance.Options.UseFont = true;
		this.lbl_EventActivity.FieldName = null;
		this.lbl_EventActivity.Location = new System.Drawing.Point(1, 3);
		this.lbl_EventActivity.Name = "lbl_EventActivity";
		this.lbl_EventActivity.Size = new System.Drawing.Size(68, 13);
		this.lbl_EventActivity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EventActivity.TabIndex = 0;
		this.lbl_EventActivity.Text = "Event/Activity";
		this.grd_Events.AllowPasteExcel = true;
		this.grd_Events.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Events.ClipboardData = null;
		this.grd_Events.ContextMenuStrip = this.cms_Events;
		this.grd_Events.Location = new System.Drawing.Point(1, 23);
		this.grd_Events.MainView = this.gv_Events;
		this.grd_Events.Name = "grd_Events";
		this.grd_Events.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.btn_Events, this.repositoryItemButtonEditCFL1, this.btn_ViewEvent, this.btn_EditEvents, this.rep_AllDay });
		this.grd_Events.Size = new System.Drawing.Size(856, 181);
		this.grd_Events.TabIndex = 6;
		this.grd_Events.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Events });
		this.gv_Events.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Events.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Events.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Events.ColumnKey = null;
		this.gv_Events.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gc_EventCode, this.gc_EventName, this.gc_ClgCode, this.gc_ECardCode, this.gc_FromDate, this.gc_ToDate, this.gc_AllDay, this.gc_Invite, this.gc_ERemark });
		this.gv_Events.DisableFormSetting = false;
		this.gv_Events.DisplayException = false;
		this.gv_Events.EditColumn = null;
		this.gv_Events.FieldCurrency = null;
		this.gv_Events.FieldRate = null;
		this.gv_Events.FormId = "oStore";
		this.gv_Events.GridControl = this.grd_Events;
		this.gv_Events.IndexEditRow = -1;
		this.gv_Events.IndicatorWidth = 35;
		this.gv_Events.IsApzTable = false;
		this.gv_Events.IsInsertNewRow = false;
		this.gv_Events.IsLoadingData = false;
		this.gv_Events.ItemId = "APZ_D_STREV";
		this.gv_Events.LstColumnAllowBatch = null;
		this.gv_Events.LstColumnNotAllowBatch = null;
		this.gv_Events.LstColumNotSetting = null;
		this.gv_Events.Name = "gv_Events";
		this.gv_Events.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Events.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Events.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Events.OptionsPrint.AutoWidth = false;
		this.gv_Events.OptionsPrint.UsePrintStyles = false;
		this.gv_Events.OptionsSelection.MultiSelect = true;
		this.gv_Events.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Events.OptionsView.ColumnAutoWidth = false;
		this.gv_Events.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Events.OptionsView.ShowGroupedColumns = true;
		this.gv_Events.OptionsView.ShowGroupPanel = false;
		this.gv_Events.UseBatchMod = false;
		this.gv_Events.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Events_RowCellStyle);
		this.gv_Events.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Events_RowStyle);
		this.gv_Events.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Events_ShowingEditor);
		this.gv_Events.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Events_CellValueChanged);
		this.gv_Events.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Events_MouseDown);
		this.gc_EventCode.Caption = "Code";
		this.gc_EventCode.CurrencyDateField = null;
		this.gc_EventCode.CurrencyField = null;
		this.gc_EventCode.CurrencyFixed = null;
		this.gc_EventCode.CurrencyFixedRate = 0.0;
		this.gc_EventCode.CurrencyRateField = null;
		this.gc_EventCode.FieldName = "Code";
		this.gc_EventCode.Name = "gc_EventCode";
		this.gc_EventCode.OptionsColumn.ReadOnly = true;
		this.gc_EventCode.PrimaryKey = false;
		this.gc_EventCode.UnitField = null;
		this.gc_EventCode.Visible = true;
		this.gc_EventCode.VisibleIndex = 0;
		this.gc_EventName.Caption = "Name";
		this.gc_EventName.ColumnEdit = this.btn_EditEvents;
		this.gc_EventName.CurrencyDateField = null;
		this.gc_EventName.CurrencyField = null;
		this.gc_EventName.CurrencyFixed = null;
		this.gc_EventName.CurrencyFixedRate = 0.0;
		this.gc_EventName.CurrencyRateField = null;
		this.gc_EventName.FieldName = "Name";
		this.gc_EventName.Name = "gc_EventName";
		this.gc_EventName.OptionsColumn.ReadOnly = true;
		this.gc_EventName.PrimaryKey = false;
		this.gc_EventName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_EventName.UnitField = null;
		this.gc_EventName.Visible = true;
		this.gc_EventName.VisibleIndex = 1;
		this.btn_EditEvents.AutoHeight = false;
		this.btn_EditEvents.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions18.Image");
		this.btn_EditEvents.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8)
		});
		this.btn_EditEvents.ContainerControl = this;
		this.btn_EditEvents.DisplayMember = "Name";
		this.btn_EditEvents.DisplayMember2 = "";
		this.btn_EditEvents.LoadDataSourceWhenEnter = false;
		this.btn_EditEvents.Name = "btn_EditEvents";
		this.btn_EditEvents.SeparateCharacterForMulti = ';';
		this.btn_EditEvents.SeparateString = ", ";
		this.btn_EditEvents.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Events;
		this.btn_EditEvents.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEventManagement;
		this.btn_EditEvents.UsingAutoCompleteBox = false;
		this.btn_EditEvents.UsingDataSource = true;
		this.btn_EditEvents.ValidateWhenNoDataSource = false;
		this.btn_EditEvents.ValueMember = "ClgCode";
		this.btn_EditEvents.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Btn_EditEvents_ButtonClick);
		this.gc_ClgCode.Caption = "ClgCode";
		this.gc_ClgCode.CurrencyDateField = null;
		this.gc_ClgCode.CurrencyField = null;
		this.gc_ClgCode.CurrencyFixed = null;
		this.gc_ClgCode.CurrencyFixedRate = 0.0;
		this.gc_ClgCode.CurrencyRateField = null;
		this.gc_ClgCode.FieldName = "ClgCode";
		this.gc_ClgCode.Name = "gc_ClgCode";
		this.gc_ClgCode.OptionsColumn.ReadOnly = true;
		this.gc_ClgCode.PrimaryKey = false;
		this.gc_ClgCode.UnitField = null;
		this.gc_ECardCode.Caption = "Card Code";
		this.gc_ECardCode.CurrencyDateField = null;
		this.gc_ECardCode.CurrencyField = null;
		this.gc_ECardCode.CurrencyFixed = null;
		this.gc_ECardCode.CurrencyFixedRate = 0.0;
		this.gc_ECardCode.CurrencyRateField = null;
		this.gc_ECardCode.FieldName = "CardCode";
		this.gc_ECardCode.Name = "gc_ECardCode";
		this.gc_ECardCode.OptionsColumn.ReadOnly = true;
		this.gc_ECardCode.PrimaryKey = false;
		this.gc_ECardCode.UnitField = null;
		this.gc_ECardCode.Visible = true;
		this.gc_ECardCode.VisibleIndex = 2;
		this.gc_FromDate.Caption = "From Date";
		this.gc_FromDate.CurrencyDateField = null;
		this.gc_FromDate.CurrencyField = null;
		this.gc_FromDate.CurrencyFixed = null;
		this.gc_FromDate.CurrencyFixedRate = 0.0;
		this.gc_FromDate.CurrencyRateField = null;
		this.gc_FromDate.FieldName = "Recontact";
		this.gc_FromDate.Name = "gc_FromDate";
		this.gc_FromDate.OptionsColumn.ReadOnly = true;
		this.gc_FromDate.PrimaryKey = false;
		this.gc_FromDate.UnitField = null;
		this.gc_FromDate.Visible = true;
		this.gc_FromDate.VisibleIndex = 3;
		this.gc_ToDate.Caption = "To Date";
		this.gc_ToDate.CurrencyDateField = null;
		this.gc_ToDate.CurrencyField = null;
		this.gc_ToDate.CurrencyFixed = null;
		this.gc_ToDate.CurrencyFixedRate = 0.0;
		this.gc_ToDate.CurrencyRateField = null;
		this.gc_ToDate.FieldName = "endDate";
		this.gc_ToDate.Name = "gc_ToDate";
		this.gc_ToDate.OptionsColumn.ReadOnly = true;
		this.gc_ToDate.PrimaryKey = false;
		this.gc_ToDate.UnitField = null;
		this.gc_ToDate.Visible = true;
		this.gc_ToDate.VisibleIndex = 4;
		this.gc_AllDay.Caption = "All Day";
		this.gc_AllDay.ColumnEdit = this.rep_AllDay;
		this.gc_AllDay.CurrencyDateField = null;
		this.gc_AllDay.CurrencyField = null;
		this.gc_AllDay.CurrencyFixed = null;
		this.gc_AllDay.CurrencyFixedRate = 0.0;
		this.gc_AllDay.CurrencyRateField = null;
		this.gc_AllDay.FieldName = "AllDayEvent";
		this.gc_AllDay.Name = "gc_AllDay";
		this.gc_AllDay.OptionsColumn.ReadOnly = true;
		this.gc_AllDay.PrimaryKey = false;
		this.gc_AllDay.UnitField = null;
		this.gc_AllDay.Visible = true;
		this.gc_AllDay.VisibleIndex = 5;
		this.rep_AllDay.AutoHeight = false;
		this.rep_AllDay.Caption = "";
		this.rep_AllDay.Name = "rep_AllDay";
		this.gc_Invite.Caption = "Invite";
		this.gc_Invite.CurrencyDateField = null;
		this.gc_Invite.CurrencyField = null;
		this.gc_Invite.CurrencyFixed = null;
		this.gc_Invite.CurrencyFixedRate = 0.0;
		this.gc_Invite.CurrencyRateField = null;
		this.gc_Invite.FieldName = "Invite";
		this.gc_Invite.Name = "gc_Invite";
		this.gc_Invite.OptionsColumn.ReadOnly = true;
		this.gc_Invite.PrimaryKey = false;
		this.gc_Invite.UnitField = null;
		this.gc_Invite.Visible = true;
		this.gc_Invite.VisibleIndex = 6;
		this.gc_ERemark.Caption = "Remarks";
		this.gc_ERemark.CurrencyDateField = null;
		this.gc_ERemark.CurrencyField = null;
		this.gc_ERemark.CurrencyFixed = null;
		this.gc_ERemark.CurrencyFixedRate = 0.0;
		this.gc_ERemark.CurrencyRateField = null;
		this.gc_ERemark.FieldName = "Remarks";
		this.gc_ERemark.Name = "gc_ERemark";
		this.gc_ERemark.OptionsColumn.ReadOnly = true;
		this.gc_ERemark.PrimaryKey = false;
		this.gc_ERemark.UnitField = null;
		this.gc_ERemark.Visible = true;
		this.gc_ERemark.VisibleIndex = 7;
		this.btn_Events.AutoHeight = false;
		this.btn_Events.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions20.Image");
		this.btn_Events.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions9)
		});
		this.btn_Events.ContainerControl = this;
		this.btn_Events.DisplayMember = "Details";
		this.btn_Events.DisplayMember2 = "";
		this.btn_Events.LoadDataSourceWhenEnter = false;
		this.btn_Events.MultiSelectMode = true;
		this.btn_Events.Name = "btn_Events";
		this.btn_Events.SeparateCharacterForMulti = ';';
		this.btn_Events.SeparateString = ", ";
		this.btn_Events.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Events;
		this.btn_Events.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEventManagement;
		this.btn_Events.UsingAutoCompleteBox = false;
		this.btn_Events.UsingDataSource = true;
		this.btn_Events.ValidateWhenNoDataSource = false;
		this.btn_Events.ValueMember = "ClgCode";
		this.repositoryItemButtonEditCFL1.AutoHeight = false;
		this.repositoryItemButtonEditCFL1.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions22.Image");
		this.repositoryItemButtonEditCFL1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions10)
		});
		this.repositoryItemButtonEditCFL1.ContainerControl = this;
		this.repositoryItemButtonEditCFL1.DisplayMember = "Details";
		this.repositoryItemButtonEditCFL1.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL1.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL1.Name = "repositoryItemButtonEditCFL1";
		this.repositoryItemButtonEditCFL1.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL1.SeparateString = ", ";
		this.repositoryItemButtonEditCFL1.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL1.UsingDataSource = true;
		this.repositoryItemButtonEditCFL1.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL1.ValueMember = "";
		this.btn_ViewEvent.AutoHeight = false;
		editorButtonImageOptions11.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_ViewEvent.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions11)
		});
		this.btn_ViewEvent.Name = "btn_ViewEvent";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Cancel.Appearance.Options.UseFont = true;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(84, 448);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_OK.Appearance.Options.UseFont = true;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(7, 448);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_CardCode.FieldName = "CardCode";
		this.btn_CardCode.IsSearchControl = false;
		this.btn_CardCode.Location = new System.Drawing.Point(364, 5);
		this.btn_CardCode.Name = "btn_CardCode";
		this.btn_CardCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_CardCode.Properties.Appearance.Options.UseFont = true;
		this.btn_CardCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_CardCode.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_CardCode.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_CardCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_CardCode.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_CardCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_CardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions12)
		});
		this.btn_CardCode.Properties.ContainerControl = this;
		this.btn_CardCode.Properties.DisplayMember = "CardCode";
		this.btn_CardCode.Properties.DisplayMember2 = "";
		this.btn_CardCode.Properties.LoadDataSourceWhenEnter = true;
		this.btn_CardCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_CardCode.Properties.SeparateString = ", ";
		this.btn_CardCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.btn_CardCode.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.btn_CardCode.Properties.UsingAutoCompleteBox = false;
		this.btn_CardCode.Properties.UsingDataSource = true;
		this.btn_CardCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_CardCode.Properties.ValueMember = "CardCode";
		this.btn_CardCode.Size = new System.Drawing.Size(150, 20);
		this.btn_CardCode.TabIndex = 2;
		this.btn_StoreCode.FieldName = null;
		this.btn_StoreCode.IsSearchControl = false;
		this.btn_StoreCode.Location = new System.Drawing.Point(105, 4);
		this.btn_StoreCode.Name = "btn_StoreCode";
		this.btn_StoreCode.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_StoreCode.Properties.Appearance.Options.UseFont = true;
		this.btn_StoreCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_StoreCode.Properties.AppearanceFocused.BackColor2 = System.Drawing.Color.FromArgb(250, 249, 215);
		this.btn_StoreCode.Properties.AppearanceFocused.Font = new System.Drawing.Font("Segoe UI", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_StoreCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_StoreCode.Properties.AppearanceFocused.Options.UseFont = true;
		this.btn_StoreCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions13.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_StoreCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions13)
		});
		this.btn_StoreCode.Properties.ContainerControl = this;
		this.btn_StoreCode.Properties.DisplayMember = "Code";
		this.btn_StoreCode.Properties.DisplayMember2 = "";
		this.btn_StoreCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_StoreCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_StoreCode.Properties.SeparateString = ", ";
		this.btn_StoreCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Store;
		this.btn_StoreCode.Properties.UsingAutoCompleteBox = false;
		this.btn_StoreCode.Properties.UsingDataSource = true;
		this.btn_StoreCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_StoreCode.Properties.ValueMember = "Code";
		this.btn_StoreCode.Size = new System.Drawing.Size(150, 20);
		this.btn_StoreCode.TabIndex = 22;
		this.btn_StoreCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_StoreCode_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(868, 474);
		base.Controls.Add(this.btn_StoreCode);
		base.Controls.Add(this.btn_CardCode);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lk_Status);
		base.Controls.Add(this.lbl_CardCode);
		base.Controls.Add(this.lbl_Status);
		base.Controls.Add(this.lbl_StoreCode);
		base.FormId = "oStore";
		base.Name = "frm_Store";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Store";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Store_FormClosing);
		this.cms_Store.ResumeLayout(false);
		this.cms_Events.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Store).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Store).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Region).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_City).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_District).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ward).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Town).EndInit();
		((System.ComponentModel.ISupportInitialize)this.glk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CardCodeCFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Events).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Events).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_EditEvents).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AllDay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Events).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ViewEvent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_CardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_StoreCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
