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
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_UnitOfMeasureGroups : XtraFormEx
{
	private const string FormController = "UnitOfMeasureGroups";

	private const string TableName = "OUGP";

	private bool _isCancel;

	private bool _forceClose;

	private int _maxEntry;

	private DataSet _ds = new DataSet();

	private DataTable _dtGroupsDefine = new DataTable();

	private List<DataRow> _lstGroupsDelete = new List<DataRow>();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public SimpleButtonEx btnCancel;

	private RepositoryItemButtonEdit rep_GLAcct;

	private GridViewEx gvDoc;

	private GridControlEx grdDoc;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btn_DenfineGroup;

	private GridColumnEx gridColumn1;

	private GridColumnEx gc_Group;

	private GridColumnEx gc_Name;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn10;

	private ToolStripMenuItem gotoDefinitionToolStripMenuItem;

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
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_UnitOfMeasureGroups()
	{
		InitializeComponent();
		gvDoc.KeyDown += KeyDownPaste;
		grdDoc.EditorKeyDown += KeyDownPaste;
		FormId = "oUnitOfMeasureGroups";
		gvDoc.ItemId = "OUGP";
		gvDoc.FormId = FormId;
		OnLoadAsync += frm_UnitOfMeasureGroups_Load;
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeyDownPaste(object sender, KeyEventArgs e)
	{
		PasteFromExcel(e, grdDoc, gvDoc);
	}

	private void PasteFromExcel(KeyEventArgs e, GridControl grd, GridView gv)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject == null)
			{
				return;
			}
			string text = dataObject.GetData(DataFormats.UnicodeText).ToString();
			object data = text;
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				DataTable dataFromExcel = GetDataFromExcel(text);
				if (dataFromExcel != null && (dataFromExcel.Rows.Count > 1 || dataFromExcel.Columns.Count > 1))
				{
					data = dataFromExcel.Rows[0][0];
					int num = gv.FocusedRowHandle;
					GridColumn focusedColumn = gv.FocusedColumn;
					int num2 = ((gv.RowCount > dataFromExcel.Rows.Count) ? dataFromExcel.Rows.Count : gv.RowCount);
					int num3 = ((gv.Columns.Count > dataFromExcel.Columns.Count) ? dataFromExcel.Columns.Count : gv.Columns.Count);
					gv.BeginUpdate();
					for (int i = 0; i < num2; i++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int j = 0; j < num3; j++)
						{
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							if (gridColumn != null && gridColumn.GroupIndex < 0)
							{
								gv.SetRowCellValue(num, gridColumn, dataFromExcel.Rows[i][j]);
							}
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
					gv.EndUpdate();
					Clipboard.Clear();
				}
			}
			Clipboard.SetData(DataFormats.Text, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDataFromExcel(string clipboardData)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<string> list = new List<string>(clipboardData.Split('\n'));
			bool flag = true;
			if (list.Count > 0 && string.IsNullOrWhiteSpace(list[list.Count - 1]))
			{
				list.RemoveAt(list.Count - 1);
			}
			foreach (string item in list)
			{
				string text = item;
				if (text.EndsWith("\r"))
				{
					text = text.Substring(0, text.Length - "\r".Length);
				}
				string[] array = text.Split('\r', '\t');
				DataRow dataRow = dataTable.NewRow();
				if (flag)
				{
					int num = 0;
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (string.IsNullOrWhiteSpace(text2))
						{
							dataTable.Columns.Add($"[BLANK{num}]");
						}
						else if (!dataTable.Columns.Contains(text2))
						{
							dataTable.Columns.Add(text2);
						}
						else
						{
							dataTable.Columns.Add($"Column {num}");
						}
						num++;
					}
					for (int j = 0; j < array.Length && j < dataTable.Columns.Count; j++)
					{
						dataRow[j] = array[j];
					}
					dataTable.Rows.Add(dataRow);
					flag = false;
				}
				else
				{
					for (int k = 0; k < array.Length && k < dataTable.Columns.Count; k++)
					{
						dataRow[k] = array[k];
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
			DataTable dataTable = await helper.GetAsync("LoadDoc");
			if (dataTable.IsNotNull())
			{
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				grdDoc.DataSource = new DataTable();
				grdDoc.RefreshDataSource();
			}
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckCode(DataTable dt)
	{
		int num = 0;
		bool result = false;
		try
		{
			if (dt.Select("UgpCode <> ''").Any())
			{
				EnumerableRowCollection<DataRow> source = from r in dt.AsEnumerable()
					where r.Field<string>("UgpCode").Trim() != ""
					select r;
				if (source.Any() && (from dr in source.CopyToDataTable().AsEnumerable()
					select dr.Field<string>("UgpCode").Trim() into x
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

	private async Task<string> SaveData()
	{
		_ = 2;
		try
		{
			DataTable dt = grdDoc.DataSource as DataTable;
			if (dt.IsNotNull())
			{
				DataTable dtCheck = dt.Copy();
				if (dtCheck.Rows.Count > 0 && string.IsNullOrEmpty(Function.ToString(dtCheck.Rows[dtCheck.Rows.Count - 1]["UgpCode"])))
				{
					dtCheck.Rows.RemoveAt(dtCheck.Rows.Count - 1);
					dtCheck.AcceptChanges();
				}
				if (CheckCode(dtCheck))
				{
					WinCommons.Error(Enums.MessageCode.GroupCannotBeDuplicated, "");
					return "Error!";
				}
				if (string.IsNullOrEmpty(Function.ToString(dtCheck.Rows[dtCheck.Rows.Count - 1]["UgpCode"])))
				{
					WinCommons.Error(Enums.MessageCode.NotEnoughDataGroupCode, "");
					return "Error!";
				}
				if (string.IsNullOrEmpty(Function.ToString(dtCheck.Rows[dtCheck.Rows.Count - 1]["UgpName"])))
				{
					WinCommons.Error(Enums.MessageCode.NotEnoughDataGroupDers, "");
					return "Error!";
				}
				using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups"))
				{
					for (int i = 0; i < dtCheck.Rows.Count; i++)
					{
						int ugpEntry = Function.ParseInt(dtCheck.Rows[i]["UgpEntry"]);
						DataTable dataTable = await helper.GetAsync("LoadDefinition", new { ugpEntry });
						if (!dataTable.IsNotNull() || dataTable.Rows.Count == 0)
						{
							WinCommons.Error(Enums.MessageCode.UomGroupCanOnlyDefinedIfBaseUomDefinedForItWithCode, "", Function.ToString(dtCheck.Rows[i]["UgpCode"]));
							return "Error!";
						}
					}
				}
				EnumerableRowCollection<DataRow> source = from t in dt.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["UgpCode"]))
					select t;
				if (source.Any())
				{
					dt = source.CopyToDataTable().Copy();
				}
				WinCommons._OpenProcessing();
				using HttpClientHelper<HttpResult> helper2 = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UnitOfMeasureGroups");
				HttpResult x = await helper2.PostAsync("Save", dt);
				if (string.IsNullOrEmpty(x.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, x.Message);
				}
				WinCommons._CloseProcessing();
				return x.Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	public async Task SaveSap()
	{
		_ = 3;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				if (dataSourceTable.Rows.Count > 0 && string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[dataSourceTable.Rows.Count - 1]["UgpCode"])))
				{
					dataSourceTable.Rows.RemoveAt(dataSourceTable.Rows.Count - 1);
					dataSourceTable.AcceptChanges();
				}
				if (CheckCode(dataSourceTable))
				{
					WinCommons.Error(Enums.MessageCode.GroupCannotBeDuplicated, "");
					return;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[dataSourceTable.Rows.Count - 1]["UgpCode"])))
				{
					WinCommons.Error(Enums.MessageCode.NotEnoughDataGroupCode, "");
					return;
				}
				if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[dataSourceTable.Rows.Count - 1]["UgpName"])))
				{
					WinCommons.Error(Enums.MessageCode.NotEnoughDataGroupDers, "");
					return;
				}
				new DataTable();
				new HttpResult();
				foreach (DataRow rowDoc in dataSourceTable.Rows)
				{
					int ugpEntry = Function.ParseInt(rowDoc["UgpEntry"]);
					if (ugpEntry == -1)
					{
						continue;
					}
					bool flag = _httpClient.UnitOfMeasureGroupsClient.CheckUoMGroupExists(ugpEntry);
					List<DataRow> list = (from dataRow2 in _dtGroupsDefine.AsEnumerable()
						where Function.ParseInt(dataRow2["UgpEntry"]) == ugpEntry
						select dataRow2).ToList();
					DataTable dtGroupsDefine;
					Enums.HttpAction httpAction;
					if (flag)
					{
						DataTable dataTable = ((list.Count <= 0) ? (await _httpClient.UnitOfMeasureGroupsClient.GetUoMGroupDefinition(ugpEntry)) : list.CopyToDataTable());
						dtGroupsDefine = dataTable;
						httpAction = Enums.HttpAction.Update;
					}
					else
					{
						dtGroupsDefine = list.CopyToDataTable();
						httpAction = Enums.HttpAction.AddNew;
					}
					DataRow dataRow = dtGroupsDefine.AsEnumerable().FirstOrDefault((DataRow dataRow2) => Function.ParseInt(dataRow2["UomEntry"]) != -1 && Function.ParseInt(dataRow2["AltQty"]) == 1 && Function.ParseInt(dataRow2["BaseQty"]) == 1);
					if (dataRow != null)
					{
						rowDoc["BaseUom"] = Function.ParseInt(dataRow["UomEntry"]);
					}
					HttpResult httpResult;
					if (_lstGroupsDelete.Count > 0)
					{
						foreach (DataRow row in _lstGroupsDelete.ToList())
						{
							Tuple<DataTable, DataTable, Enums.HttpAction> tupData = new Tuple<DataTable, DataTable, Enums.HttpAction>(row.ToArray().CopyToDataTable(), new DataTable(), Enums.HttpAction.Delete);
							httpResult = await _httpClient.UnitOfMeasureGroupsClient.SaveSap(tupData);
							if (!string.IsNullOrEmpty(httpResult.Message))
							{
								WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
								return;
							}
							_lstGroupsDelete.Remove(row);
						}
					}
					Tuple<DataTable, DataTable, Enums.HttpAction> tupData2 = new Tuple<DataTable, DataTable, Enums.HttpAction>(rowDoc.ToArray().CopyToDataTable(), dtGroupsDefine, httpAction);
					httpResult = await _httpClient.UnitOfMeasureGroupsClient.SaveSap(tupData2);
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						return;
					}
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void AddNew()
	{
		try
		{
			gvDoc.BeginInsertRow();
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				_maxEntry = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["UgpEntry"])) + 1;
				DataRow dataRow = dataTable.NewRow();
				dataRow["UgpEntry"] = ((_maxEntry + 1 <= 0) ? 1 : (_maxEntry + 1));
				dataRow["UgpCode"] = string.Empty;
				dataRow["UgpName"] = string.Empty;
				dataRow["BaseUom"] = 1;
				dataRow["DataSource"] = "I";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["LogInstanc"] = 0;
				dataRow["UserSign2"] = DBNull.Value;
				dataRow["UpdateDate"] = DBNull.Value;
				dataRow["CreateDate"] = DateTime.Now;
				dataRow["Locked"] = "N";
				dataTable.Rows.Add(dataRow);
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("UgpEntry", typeof(int));
				dataTable2.Columns.Add("UgpCode", typeof(string));
				dataTable2.Columns.Add("UgpName", typeof(string));
				dataTable2.Columns.Add("BaseUom", typeof(int));
				dataTable2.Columns.Add("DataSource", typeof(string));
				dataTable2.Columns.Add("UserSign", typeof(int));
				dataTable2.Columns.Add("LogInstanc", typeof(int));
				dataTable2.Columns.Add("UserSign2", typeof(int));
				dataTable2.Columns.Add("UpdateDate", typeof(DateTime));
				dataTable2.Columns.Add("CreateDate", typeof(DateTime));
				dataTable2.Columns.Add("Locked", typeof(string));
				dataTable2.Rows.Add(1, string.Empty, string.Empty, 1, "I", GlobalData.UserID, 0, DBNull.Value, DBNull.Value, DateTime.Now, "N");
				dataTable2.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable2;
				grdDoc.RefreshDataSource();
			}
			gvDoc.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gvDoc.EndInsertRow();
	}

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
			if (dataTable != null)
			{
				DataTable dataTable2 = await helper.GetAsync("GetAddNewRow", new
				{
					tableName = "OUGP",
					codeMax = dataTable.Rows[dataTable.Rows.Count - 1]["UgpEntry"]
				});
				if (dataTable2.IsNotNull())
				{
					return dataTable2;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> CheckExistUgpEntry(int itgrCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups");
			return await helper.GetAsync("CheckExistUgpEntry", new { itgrCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private string CleanUoMGroup()
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "UnitOfMeasureGroups");
			return httpClientHelper.Get("CleanUoMGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async Task frm_UnitOfMeasureGroups_Load(object sender, EventArgs e)
	{
		try
		{
			string text = CleanUoMGroup();
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_DenfineGroup_Click(object sender, EventArgs e)
	{
		try
		{
			if (grdDoc.DataSource is DataTable dataTable && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("UgpCode"))) && Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) == Function.ParseInt(dataTable.Rows[dataTable.Rows.Count - 1]["UgpEntry"]))
			{
				WinCommons.Error(Enums.MessageCode.EnterGroupCodeBeforeCreateDefinetionForNewGroup, "");
				return;
			}
			WinCommons._OpenProcessing();
			using frm_UoMGroupDefinition frm_UoMGroupDefinition2 = new frm_UoMGroupDefinition
			{
				UgpEntry = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "UgpEntry"))
			};
			if (!_dtGroupsDefine.IsNotNull())
			{
				_dtGroupsDefine = WinFuntions.GetTableStructure("UGP1");
				_dtGroupsDefine.Columns.Add("UomCode", typeof(string));
				_dtGroupsDefine.Columns.Add("BaseUoMCode", typeof(string));
				_dtGroupsDefine.AcceptChanges();
			}
			frm_UoMGroupDefinition2._dtSource = _dtGroupsDefine;
			frm_UoMGroupDefinition2.ShowDialog(this);
			if (frm_UoMGroupDefinition2._dtSource.IsNotNull())
			{
				FormMode = (frm_UoMGroupDefinition2._isUpdated ? Enums.FormViewModeType.Update : FormMode);
				_dtGroupsDefine = frm_UoMGroupDefinition2._dtSource;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (e.RowHandle == dataTable.Rows.Count - 1)
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

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		if (FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				Close();
			}
		}
		else
		{
			Close();
			_isCancel = false;
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					btn_Save.Enabled = false;
					btnCancel.Enabled = false;
					await SaveSap();
					btn_Save.Enabled = true;
					btnCancel.Enabled = true;
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

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry"));
			if (num == _maxEntry || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteGroup, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes || Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) < 0)
			{
				return;
			}
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups"))
			{
				if (dataTable != null)
				{
					DataTable dataTable2 = await helper.GetAsync("CheckGroupInItems", new
					{
						ugpEntry = num
					});
					if (dataTable2.IsNotNull() || dataTable2.Rows.Count > 0)
					{
						WinCommons.Error(Enums.MessageCode.UomGroupCannotBeDeleteBecauseItIsCurrentlyReferencedWithCode, "", Function.ToString(gvDoc.GetFocusedRowCellValue("UgpCode")));
						return;
					}
				}
			}
			if (!_lstGroupsDelete.Contains(gvDoc.GetFocusedDataRow()))
			{
				DataRow item = gvDoc.GetDataRow(gvDoc.FocusedRowHandle).ToArray().CopyToDataTable()
					.Rows[0];
				_lstGroupsDelete.Add(item);
				gvDoc.DeleteRow(gvDoc.FocusedRowHandle);
				grdDoc.DataSourceTable.AcceptChanges();
				grdDoc.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gotoDefinitionToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			btn_DenfineGroup_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) != -1)
			{
				btn_DenfineGroup_Click(null, null);
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

	private void gvDoc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			btn_DenfineGroup.Enabled = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) != -1;
			gotoDefinitionToolStripMenuItem.Enabled = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) != -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_UnitOfMeasureGroups_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					btn_Save_Click(null, null);
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvDoc.FocusedColumn == gc_Group && Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) == -1)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_UnitOfMeasureGroups));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.rep_GLAcct = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Group = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.gotoDefinitionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_DenfineGroup = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(101, 326);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.rep_GLAcct.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_GLAcct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions)
		});
		this.rep_GLAcct.Name = "rep_GLAcct";
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = "UgpEntry";
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn1, this.gc_Group, this.gc_Name, this.gridColumn4, this.gridColumn6, this.gridColumn10 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oUnitOfMeasureGroup";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = true;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "OUGP";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvDoc_FocusedRowChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gvDoc.DoubleClick += new System.EventHandler(this.gvDoc_DoubleClick);
		this.gridColumn1.Caption = "UgpEntry";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "UgpEntry";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gc_Group.Caption = "Group";
		this.gc_Group.CurrencyDateField = null;
		this.gc_Group.CurrencyField = null;
		this.gc_Group.CurrencyFixed = null;
		this.gc_Group.CurrencyFixedRate = 0.0;
		this.gc_Group.CurrencyRateField = null;
		this.gc_Group.FieldName = "UgpCode";
		this.gc_Group.Name = "gc_Group";
		this.gc_Group.PrimaryKey = false;
		this.gc_Group.UnitField = null;
		this.gc_Group.Visible = true;
		this.gc_Group.VisibleIndex = 0;
		this.gc_Group.Width = 204;
		this.gc_Name.Caption = "Group Description";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "UgpName";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 1;
		this.gc_Name.Width = 429;
		this.gridColumn4.Caption = "BaseUom";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "BaseUom";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn6.Caption = "UserSign";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "UserSign";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn10.Caption = "CreateDate";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "CreateDate";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_GLAcct });
		this.grdDoc.Size = new System.Drawing.Size(763, 317);
		this.grdDoc.TabIndex = 74;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.gotoDefinitionToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(163, 48);
		this.gotoDefinitionToolStripMenuItem.Name = "gotoDefinitionToolStripMenuItem";
		this.gotoDefinitionToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
		this.gotoDefinitionToolStripMenuItem.Text = "Group Definition";
		this.gotoDefinitionToolStripMenuItem.Click += new System.EventHandler(this.gotoDefinitionToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(162, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(0, 326);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 76;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_DenfineGroup.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_DenfineGroup.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_DenfineGroup.Appearance.Options.UseBackColor = true;
		this.btn_DenfineGroup.FieldName = null;
		this.btn_DenfineGroup.ImageOptions.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.btn_DenfineGroup.IsSearchControl = false;
		this.btn_DenfineGroup.Location = new System.Drawing.Point(577, 326);
		this.btn_DenfineGroup.Name = "btn_DenfineGroup";
		this.btn_DenfineGroup.Size = new System.Drawing.Size(173, 20);
		this.btn_DenfineGroup.TabIndex = 77;
		this.btn_DenfineGroup.Text = "Group Definition";
		this.btn_DenfineGroup.Click += new System.EventHandler(this.btn_DenfineGroup_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(762, 355);
		base.Controls.Add(this.btn_DenfineGroup);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oUnitOfMeasureGroup";
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_UnitOfMeasureGroups";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Unit Of Measure Groups";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UnitOfMeasureGroups_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
