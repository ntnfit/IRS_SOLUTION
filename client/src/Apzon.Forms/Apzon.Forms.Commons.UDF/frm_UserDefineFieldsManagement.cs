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
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
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
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Commons.UDF;

public class frm_UserDefineFieldsManagement : XtraFormEx
{
	private const string FormController = "UDF";

	private readonly RepositoryItemLookUpEdit _typeEditTypesAlphaNumeric = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeEditTypesDateTime = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeEditTypesUnitandTotal = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeEditTypesGeneral = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeEditTypesNumeric = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _defaultValueEdit = new RepositoryItemLookUpEdit();

	private List<RepositoryItemLookUpEdit> _listLuk = new List<RepositoryItemLookUpEdit>();

	private string[] arrGreyColsWhenUpdate = new string[3] { "AliasID", "TypeID1", "EditType1" };

	private RepositoryItemTextEdit _timeEdit = new RepositoryItemTextEdit();

	private DataTable _dtUdf1 = new DataTable();

	private DataTable _dtKeyFields = new DataTable();

	private DataTable _dtValidValues = new DataTable();

	private bool _isNodeChanging;

	private int _codeMax;

	private bool _isLoading;

	private bool _isCancel;

	private bool _forceClose;

	private readonly Dictionary<string, string> _lstValid = new Dictionary<string, string>
	{
		{ "N", "None" },
		{ "LS", "Link to System Object" },
		{ "LT", "Set Linked Table" },
		{ "LU", "Link to UDO" },
		{ "V", "Valid Values" }
	};

	private bool _isFocusedRowChanging;

	private bool _isFocusedRowObjectChanging;

	private IContainer components;

	private TreeList tv_UDF;

	public SimpleButtonEx btn_Remove;

	public SimpleButtonEx btn_Keys;

	public SimpleButtonEx btn_Update;

	private TreeListColumnEx TableCode;

	private TreeListColumnEx Descr;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private RepositoryItemLookUpEdit rep_Type;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private RepositoryItemCheckEdit rep_Mandatory;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private SplitContainerControl splitContainerControl2;

	private XtraTabControlEx xtraTab_Keys;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private RepositoryItemLookUpEdit rep_Validation;

	private RepositoryItemLookUpEdit rep_LinkToTable;

	private RepositoryItemLookUpEdit rep_LinkUdo;

	private GridColumnEx gridColumn24;

	private GridColumnEx gridColumn23;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private ContextMenuStrip ctx_Valid;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Keys;

	private GridViewEx gv_Keys;

	private GridColumnEx gridColumn25;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private ContextMenuStrip ctx_Key;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip ctx_TreeList;

	private ToolStripMenuItem expandAllToolStripMenuItem;

	private ToolStripMenuItem collapseAllToolStripMenuItem;

	private GridColumnEx gridColumn32;

	private RepositoryItemLookUpEdit rep_LinkSysObj;

	private GridColumnEx gridColumn30;

	private GridColumnEx gridColumn31;

	private GridColumnEx gridColumn29;

	private RepositoryItemCheckEdit rep_Index;

	private CheckEditEx chk_Unique;

	private TextEditEx txt_KeyName;

	private LabelLine labelLine1;

	private LookUpEdit look_Key;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Delete;

	private SimpleButtonEx btn_New;

	private RepositoryItemLookUpEdit rep_ColAlias;

	private ContextMenuStrip ctx_Line;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gridColumn33;

	private GridColumnEx gridColumn34;

	public SimpleButtonEx btn_Cancel;

	private ToolStripMenuItem addToolStripMenuItem;

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
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_UserDefineFieldsManagement()
	{
		InitializeComponent();
		FormId = "oUserFields";
		gv_Doc.ItemId = "CUFD";
		gv_Doc.FormId = FormId;
		OnLoadAsync += frm_UserDefineFieldsManagement_Load;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			gv_Doc.PostEditor();
			gv_Doc.RefreshData();
			bool flag = true;
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				FormMode = Enums.FormViewModeType.View;
				gv_Doc.ClearColumnsFilter();
				gv_Doc.ClearGrouping();
				gv_Doc.ClearSorting();
				await GetFocusNode();
			}
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
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			string text = Function.ToString(tv_UDF.FocusedNode.GetValue("TableName"));
			if (!string.IsNullOrWhiteSpace(text) && flag)
			{
				FormMode = Enums.FormViewModeType.Add;
				await AddNew(text);
				grd_Keys.DataSource = new DataTable();
				grd_Keys.RefreshDataSource();
				_dtValidValues = WinFuntions.GetTableStructure("UFD1");
				grd_Line.DataSource = _dtValidValues.Clone();
				grd_Line.RefreshDataSource();
				_dtUdf1 = _dtUdf1.Clone();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadChildUdfData()
	{
		try
		{
			int fieldId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("FieldID"));
			string tableCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("TypeID"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("EditType"));
			LoadSysObj(tableCode.StartsWith("APZ_"));
			if (text.Equals("N") && text2.Equals("T"))
			{
				gv_Line.Columns["FldValue"].DisplayFormat.FormatType = FormatType.Numeric;
			}
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("Validation")).Equals("V"))
			{
				await GetUdf1(tableCode, fieldId);
			}
			else
			{
				grd_Line.DataSource = WinFuntions.GetTableStructure("UFD1");
				grd_Line.RefreshDataSource();
			}
			AddNewGridValue(tableCode, fieldId);
			if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("AliasID"))))
			{
				btn_Update.Enabled = true;
				btn_Remove.Enabled = false;
			}
			else
			{
				btn_Update.Enabled = true;
				btn_Remove.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetButtonOKTextToUpdate()
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void AddNewGridValue(string tableId, int fieldId)
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = _listLuk.Find((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(fieldId));
			DataTable dataTable = grd_Line.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["FldValue"])))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["FieldID"] = fieldId;
					dataRow["IndexID"] = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["IndexID"])) + 1;
					dataRow["TableID"] = tableId;
					dataRow["FldDate"] = DBNull.Value;
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_Line.DataSource = dataTable;
					grd_Line.RefreshDataSource();
					_dtValidValues = dataTable.Copy();
					_dtValidValues.AcceptChanges();
				}
				if (repositoryItemLookUpEdit != null)
				{
					dataTable.AcceptChanges();
					repositoryItemLookUpEdit.DataSource = dataTable.Copy();
				}
				return;
			}
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("TableID", typeof(string));
			dataTable2.Columns.Add("FieldID", typeof(int));
			dataTable2.Columns.Add("IndexID", typeof(int));
			dataTable2.Columns.Add("FldValue", typeof(string));
			dataTable2.Columns.Add("Descr", typeof(string));
			dataTable2.Columns.Add("FldDate", typeof(DateTime));
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2["FieldID"] = fieldId;
			dataRow2["IndexID"] = 0;
			dataRow2["TableID"] = tableId;
			dataRow2["FldDate"] = DBNull.Value;
			dataTable2.Rows.Add(dataRow2);
			dataTable2.AcceptChanges();
			grd_Line.DataSource = dataTable2;
			grd_Line.RefreshDataSource();
			_dtValidValues = dataTable2.Copy();
			_dtValidValues.AcceptChanges();
			if (repositoryItemLookUpEdit != null)
			{
				repositoryItemLookUpEdit.DataSource = dataTable2.Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadInItForm()
	{
		_ = 1;
		try
		{
			using (HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF"))
			{
				DataTable dataSource = await help.GetAsync("GetTableLinked");
				rep_LinkToTable.DataSource = dataSource;
			}
			using (HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF"))
			{
				DataTable dataSource2 = await help.GetAsync("GetObject");
				rep_LinkUdo.DataSource = dataSource2;
			}
			List<Common.Table_Code> dataSource3 = _lstValid.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rep_Validation.DataSource = dataSource3;
			List<Common.Table_Code> dataSource4 = CommonList.Types.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rep_Type.DataSource = dataSource4;
			List<Common.Table_Code> dataSource5 = CommonList.EditTypesAlphaNumeric.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeEditTypesAlphaNumeric.DataSource = dataSource5;
			_typeEditTypesAlphaNumeric.DisplayMember = "Name";
			_typeEditTypesAlphaNumeric.ValueMember = "Code";
			_typeEditTypesAlphaNumeric.NullText = string.Empty;
			_typeEditTypesAlphaNumeric.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource6 = CommonList.EditTypesDateTime.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeEditTypesDateTime.DataSource = dataSource6;
			_typeEditTypesDateTime.DisplayMember = "Name";
			_typeEditTypesDateTime.ValueMember = "Code";
			_typeEditTypesDateTime.NullText = string.Empty;
			_typeEditTypesDateTime.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource7 = CommonList.EditTypesUnitandTotal.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeEditTypesUnitandTotal.DataSource = dataSource7;
			_typeEditTypesUnitandTotal.DisplayMember = "Name";
			_typeEditTypesUnitandTotal.ValueMember = "Code";
			_typeEditTypesUnitandTotal.NullText = string.Empty;
			_typeEditTypesUnitandTotal.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> dataSource8 = CommonList.EditTypesGeneral.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeEditTypesGeneral.DataSource = dataSource8;
			_typeEditTypesGeneral.DisplayMember = "Name";
			_typeEditTypesGeneral.ValueMember = "Code";
			_typeEditTypesGeneral.NullText = string.Empty;
			_typeEditTypesGeneral.BestFitMode = BestFitMode.BestFitResizePopup;
			_typeEditTypesNumeric.ReadOnly = true;
			_defaultValueEdit.NullText = "";
			_defaultValueEdit.DisplayMember = "FldValue";
			_defaultValueEdit.ValueMember = "FldValue";
			_defaultValueEdit.Columns.Add(new LookUpColumnInfo("FldValue", 20, ""));
			_defaultValueEdit.Columns.Add(new LookUpColumnInfo("Descr", 20, ""));
			_defaultValueEdit.NullText = string.Empty;
			_defaultValueEdit.BestFitMode = BestFitMode.BestFitResizePopup;
			_timeEdit.CustomDisplayText += edit_CustomDisplayText;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddNew(string table)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dataTable = await helper.GetAsync("GetAddNewRow", new
			{
				tableName = table
			});
			if (dataTable != null)
			{
				dataTable.Columns.Add("Validation", typeof(string));
				if (dataTable.Rows.Count == 0)
				{
					dataTable.Rows.Add(dataTable.NewRow());
				}
				dataTable.Rows[0]["Validation"] = "N";
				dataTable.Rows[0]["EditType"] = " ";
				dataTable.AcceptChanges();
			}
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			LoadInTreeList(await helper.GetAsync("LoadDataOUTB"));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetListUdf(string tblName)
	{
		_ = 1;
		try
		{
			_listLuk.Clear();
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dtUdf = await helper.GetAsync("GetListCUFD", new
			{
				tableName = tblName
			});
			if (dtUdf.IsNotNull())
			{
				dtUdf.Columns.Add("Validation", typeof(string));
				dtUdf.AcceptChanges();
				foreach (DataRow dr in dtUdf.Rows)
				{
					string tableCode = Function.ToString(dr["TableID"]);
					int fieldID = Function.ParseInt(dr["FieldID"]);
					if ((await helper.GetAsync("GetUfd1", new { tableCode, fieldID })).IsNotNull())
					{
						dr["Validation"] = "V";
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dr["RTable"])))
					{
						dr["Validation"] = "LT";
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dr["RelUDO"])))
					{
						dr["Validation"] = "LU";
					}
					else if (!string.IsNullOrEmpty(Function.ToString(dr["RelSO"])))
					{
						dr["Validation"] = "LS";
					}
					else
					{
						dr["Validation"] = "N";
					}
				}
				_dtValidValues = WinFuntions.GetTableStructure("UFD1");
				foreach (DataRow row2 in dtUdf.Rows)
				{
					List<DataRow> list = helper.Get("GetUfd1", new
					{
						tableCode = row2["TableID"],
						fieldID = row2["FieldID"]
					}).AsEnumerable().ToList();
					DataTable dataTable = _dtValidValues.Clone();
					foreach (DataRow item in list)
					{
						_dtValidValues.ImportRow(item);
						dataTable.ImportRow(item);
					}
					if (list.Count > 0)
					{
						RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
						repositoryItemLookUpEdit.Assign(_defaultValueEdit);
						DataRow row = dataTable.NewRow();
						dataTable.Rows.InsertAt(row, 0);
						dataTable.AcceptChanges();
						repositoryItemLookUpEdit.DataSource = dataTable.Copy();
						repositoryItemLookUpEdit.Name = Function.ToString(row2["FieldID"]);
						_listLuk.Add(repositoryItemLookUpEdit);
					}
				}
				btn_Update.Enabled = true;
				btn_Remove.Enabled = false;
				if (Function.ToString(gv_Doc.GetFocusedRowCellValue("TypeID")) == "D")
				{
					if (gv_Line.Columns["FldValue"] != null)
					{
						gv_Line.Columns["FldValue"].ColumnEdit = new RepositoryItemDateEditEx();
					}
					DataTable dataTable2 = grd_Line.DataSource as DataTable;
					if (dataTable2.IsNotNull())
					{
						DataTable dataTable3 = dataTable2.Copy();
						dataTable3.Columns.Remove("FldValue");
						dataTable3.AcceptChanges();
						dataTable3.Columns.Add(new DataColumn("FldValue", typeof(DateTime)));
						dataTable3.AcceptChanges();
						for (int i = 0; i < dataTable2.Rows.Count; i++)
						{
							string text = Function.ToString(dataTable2.Rows[i]["FldValue"]);
							if (text.Length == 8)
							{
								dataTable3.Rows[i]["FldValue"] = new DateTime(Function.ParseInt(text.Substring(0, 4)), Function.ParseInt(text.Substring(4, 2)), Function.ParseInt(text.Substring(6, 2)));
							}
							else
							{
								dataTable3.Rows[i]["FldValue"] = DBNull.Value;
							}
						}
						dataTable3.AcceptChanges();
						grd_Line.DataSource = dataTable3;
						grd_Line.RefreshDataSource();
					}
				}
			}
			else
			{
				_dtValidValues = WinFuntions.GetTableStructure("UFD1");
			}
			grd_Doc.DataSource = dtUdf;
			dtUdf.AcceptChanges();
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetFocusNode()
	{
		_ = 1;
		try
		{
			string table = Function.ToString(tv_UDF.FocusedNode.GetValue("TableName"));
			if (string.IsNullOrWhiteSpace(table))
			{
				grd_Doc.DataSource = WinFuntions.GetTableStructure("CUFD");
				grd_Doc.RefreshDataSource();
				grd_Line.DataSource = WinFuntions.GetTableStructure("UFD1");
				grd_Line.RefreshDataSource();
				return;
			}
			WinCommons._OpenProcessing();
			_isLoading = true;
			txt_KeyName.EditValue = "";
			if (look_Key.Properties.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
			}
			DataTable dataSourceTable = grd_Keys.DataSourceTable;
			dataSourceTable.Rows.Clear();
			dataSourceTable.AcceptChanges();
			if (Function.ParseInt(tv_UDF.FocusedNode.GetValue("ParentID")) > 0)
			{
				await GetListUdf(table);
				DataTable dataTable2 = grd_Doc.DataSource as DataTable;
				if (dataTable2.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0]["AliasID"])))
				{
					btn_Keys.Enabled = true;
					_codeMax = grd_Doc.DataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["FieldID"]));
				}
				else
				{
					_codeMax = -1;
					btn_Keys.Enabled = false;
				}
			}
			await GetStructureUfd1();
			xtraTabPage2.PageVisible = false;
			if (table.StartsWith("@"))
			{
				List<Common.Table_Code> dataSource = new Dictionary<string, string>
				{
					{ "N", "None" },
					{ "LT", "Set Linked Table" },
					{ "V", "Valid Values" }
				}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
				{
					Code = item.Key,
					Name = item.Value
				}).ToList();
				rep_Validation.DataSource = dataSource;
			}
			else
			{
				List<Common.Table_Code> dataSource2 = _lstValid.Select((KeyValuePair<string, string> item) => new Common.Table_Code
				{
					Code = item.Key,
					Name = item.Value
				}).ToList();
				rep_Validation.DataSource = dataSource2;
			}
			FormMode = Enums.FormViewModeType.View;
			_isLoading = false;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task GetUdf1(string tableCode, int fieldId)
	{
		try
		{
			if (_dtUdf1.IsNotNull())
			{
				List<DataRow> list = (from t in _dtUdf1.AsEnumerable()
					where Function.ToString(t["TableID"]) == tableCode && Function.ParseInt(t["FieldID"]) == fieldId
					select t).ToList();
				if (list.Count > 0)
				{
					DataTable dataSource = list.CopyToDataTable();
					grd_Line.DataSource = dataSource;
					grd_Line.RefreshDataSource();
				}
				else
				{
					using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
					DataTable dataSource2 = await helper.GetAsync("GetUfd1", new
					{
						tableCode = tableCode,
						fieldID = fieldId
					});
					grd_Line.DataSource = dataSource2;
					grd_Line.RefreshDataSource();
				}
			}
			else
			{
				using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
				DataTable dataSource3 = await helper.GetAsync("GetUfd1", new
				{
					tableCode = tableCode,
					fieldID = fieldId
				});
				grd_Line.DataSource = dataSource3;
				grd_Line.RefreshDataSource();
			}
			if (!(Function.ToString(gv_Doc.GetFocusedRowCellValue("TypeID")) == "D"))
			{
				return;
			}
			if (gv_Line.Columns["FldValue"] != null)
			{
				gv_Line.Columns["FldValue"].ColumnEdit = new RepositoryItemDateEditEx();
			}
			DataTable dataTable = grd_Line.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataTable dataTable2 = dataTable.Copy();
			dataTable2.Columns.Remove("FldValue");
			dataTable2.AcceptChanges();
			dataTable2.Columns.Add(new DataColumn("FldValue", typeof(DateTime)));
			dataTable2.AcceptChanges();
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				string text = Function.ToString(dataTable.Rows[num]["FldValue"]);
				if (text.Length == 8)
				{
					dataTable2.Rows[num]["FldValue"] = new DateTime(Function.ParseInt(text.Substring(0, 4)), Function.ParseInt(text.Substring(4, 2)), Function.ParseInt(text.Substring(6, 2)));
				}
				else
				{
					dataTable2.Rows[num]["FldValue"] = DBNull.Value;
				}
			}
			dataTable2.AcceptChanges();
			grd_Line.DataSource = dataTable2;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetUkd1(string tblName, int keyId)
	{
		try
		{
			if (_dtKeyFields.IsNotNull())
			{
				List<DataRow> list = (from t in _dtKeyFields.AsEnumerable()
					where Function.ToString(t["TableName"]) == tblName && Function.ParseInt(t["KeyId"]) == keyId
					select t).ToList();
				if (list.Count > 0)
				{
					DataTable dataSource = list.CopyToDataTable();
					grd_Keys.DataSource = dataSource;
					grd_Keys.RefreshDataSource();
					AddNewKeys();
					return;
				}
				using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
				DataTable dataSource2 = await helper.GetAsync("GetUkd1", new
				{
					tableName = tblName,
					keyId = keyId
				});
				grd_Keys.DataSource = dataSource2;
				grd_Keys.RefreshDataSource();
				AddNewKeys();
			}
			else
			{
				using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
				DataTable dataSource3 = await helper.GetAsync("GetUkd1", new
				{
					tableName = tblName,
					keyId = keyId
				});
				grd_Keys.DataSource = dataSource3;
				grd_Keys.RefreshDataSource();
				AddNewKeys();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetOukd(string tblName)
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dtUdf = await helper.GetAsync("GetOUKD", new
			{
				tableName = tblName
			});
			look_Key.Properties.DataSource = dtUdf;
			if (!dtUdf.IsNotNull())
			{
				btn_Delete.Enabled = false;
				return;
			}
			_isLoading = true;
			look_Key.EditValue = dtUdf.Rows[0]["KeyId"];
			string tblName2 = Function.ToString(dtUdf.Rows[0]["TableName"]);
			int keyId = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
			await GetUkd1(tblName2, keyId);
			txt_KeyName.Text = Function.ToString(dtUdf.Rows[0]["KeyName"]);
			chk_Unique.EditValue = Function.ToString(dtUdf.Rows[0]["UniqueKey"]);
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetStructureUkd1()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			_dtKeyFields = await helper.GetAsync("GetStructureUKD1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetStructureUfd1()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			_dtUdf1 = await helper.GetAsync("GetStructureUFD1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetListKeyAlias(string tblName)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dataTable = await helper.GetAsync("GetListKeyAlias", new
			{
				tableName = tblName
			});
			DataRow dataRow = dataTable.NewRow();
			dataRow["AliasID"] = "";
			dataRow["Descr"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			rep_ColAlias.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewKeys()
	{
		try
		{
			DataTable dataTable = grd_Keys.DataSource as DataTable;
			DataTable dataTable2 = rep_ColAlias.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["ColAlias"])))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["KeyId"] = Function.ParseInt(dataTable.Rows[dataTable.Rows.Count - 1]["KeyId"]);
					dataRow["TableName"] = Function.ToString(look_Key.GetColumnValue("TableName"));
					dataRow["SubKeyId"] = Function.ParseInt(dataTable.Rows[dataTable.Rows.Count - 1]["SubKeyId"]) + 1;
					dataRow["ColAlias"] = "";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_Keys.DataSource = dataTable;
					grd_Keys.RefreshDataSource();
				}
				return;
			}
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("TableName", typeof(string));
			dataTable3.Columns.Add("KeyId", typeof(int));
			dataTable3.Columns.Add("SubKeyId", typeof(int));
			dataTable3.Columns.Add("ColAlias", typeof(string));
			DataRow dataRow2 = dataTable3.NewRow();
			dataRow2["KeyId"] = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
			dataRow2["TableName"] = Function.ToString(look_Key.GetColumnValue("TableName"));
			dataRow2["SubKeyId"] = 0;
			if (dataTable != null && dataTable2 != null && dataTable.Rows.Count < 1)
			{
				dataRow2["ColAlias"] = dataTable2.Rows[1]["AliasID"];
			}
			else
			{
				dataRow2["ColAlias"] = "";
			}
			DataRow dataRow3 = dataTable3.NewRow();
			dataRow3["KeyId"] = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
			dataRow3["TableName"] = Function.ToString(look_Key.GetColumnValue("TableName"));
			dataRow3["SubKeyId"] = 1;
			dataRow3["ColAlias"] = "";
			dataTable3.Rows.Add(dataRow2);
			dataTable3.Rows.Add(dataRow3);
			dataTable3.AcceptChanges();
			grd_Keys.DataSource = dataTable3;
			grd_Keys.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> CheckExistFieldInKeys(string tblName, string alias)
	{
		string empty;
		try
		{
			string[] array = tblName.Split(',');
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dataTable = await helper.GetAsync("GetUKD1ByTable", new
			{
				tableName = array[0]
			});
			if (dataTable.IsNotNull())
			{
				if ((from t in dataTable.AsEnumerable()
					where Function.ToString(t["ColAlias"]) == alias
					select t).ToList().Count > 0)
				{
					return "There is a user-defined key referring to this field.";
				}
				empty = string.Empty;
			}
			else
			{
				empty = string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
		return empty;
	}

	private async Task<string> SaveUdf()
	{
		try
		{
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Clone();
				dataTable2.ImportRow(dataTable.Rows[gv_Doc.GetFocusedDataSourceRowIndex()]);
				dataTable2.AcceptChanges();
				if (!dataTable.IsNotNull())
				{
					return "No data to save";
				}
				DataRow[] array = (from t in dataTable2.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["AliasID"]))
					select t).ToArray();
				if (array.Length == 0)
				{
					return "Field AliasID cannot be null";
				}
				if (array.Length != 0)
				{
					dataTable2 = array.CopyToDataTable();
				}
				if ((from i in dataTable2.AsEnumerable()
					group i by Function.ToString(i.Field<string>("AliasID")).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList().Count > 0)
				{
					return "This entry already exists in the following tables  'User Fields - Description'";
				}
				if ((from j in dataTable2.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(j["EditSize"])) && Function.ToString(j["TypeID"]) != "M" && (Function.ParseInt(j["EditSize"]) > 254 || Function.ParseInt(j["EditSize"]) < 1)
					select j).ToList().Count > 0)
				{
					return "Field size deviates from legal range [1..254]. Field: Length.";
				}
				using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF"))
				{
					string tableName = Function.ToString(dataTable2.Rows[0]["TableID"]);
					DataTable dataTable3 = httpClientHelper.Get("GetListCUFD", new { tableName });
					if (dataTable3.IsNotNull())
					{
						foreach (DataRow row2 in dataTable2.Rows)
						{
							int field = Function.ParseInt(row2["FieldID"]);
							DataRow row1 = row2;
							if ((from t in dataTable3.AsEnumerable()
								where Function.ParseInt(t["FieldID"]) == field && Function.ParseInt(t["EditSize"]) > Function.ParseInt(row1["EditSize"])
								select t).ToList().Count > 0)
							{
								return "Field length cannot be decreased. Field: Length.";
							}
						}
					}
				}
				string value = Function.ToString(dataTable2.Rows[0]["EditType"]);
				string text = Function.ToString(dataTable2.Rows[0]["TypeID"]);
				string text2 = Function.ToString(dataTable2.Rows[0]["Dflt"]);
				int editSize = Function.ParseInt(dataTable2.Rows[0]["EditSize"]);
				double result2;
				switch (text)
				{
				case "A":
				case "M":
					if (!string.IsNullOrWhiteSpace(text2) && text.Equals("M"))
					{
						return "Cannot set Default Value for Field type 'General'";
					}
					if (double.TryParse(dataTable2.Rows[0]["EditSize"].ToString(), out result2) && text2.Length > editSize)
					{
						return "In 'Set Default Value for Field', enter number of characters less than or equal to value in 'Length' field";
					}
					if (grd_Line.DataSource is DataTable dataTable4 && dataTable4.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FldValue"]).Length > editSize) != null)
					{
						return "In 'Value' column, enter number of characters less than or equal to value in 'Length' field";
					}
					break;
				case "N":
				case "B":
					if (!string.IsNullOrWhiteSpace(text2) && !double.TryParse(text2, out result2))
					{
						return "Default values doesn't match 'Type' field.";
					}
					if (!string.IsNullOrEmpty(value) && text == "N")
					{
						return "Can't enter value for 'Structure' while choose 'Type' Numeric";
					}
					if (double.TryParse(dataTable2.Rows[0]["EditSize"].ToString(), out result2) && text2.Length > editSize)
					{
						return "In 'Set Default Value for Field', enter number of characters less than or equal to value in 'Length' field";
					}
					if (grd_Line.DataSource is DataTable dataTable5 && dataTable5.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FldValue"]).Length > editSize) != null)
					{
						return "In 'Value' column, enter number of characters less than or equal to value in 'Length' field";
					}
					break;
				case "D":
					if (!string.IsNullOrWhiteSpace(text2))
					{
						if (!DateTime.TryParse(text2, out var _))
						{
							return "Default values doesn't match 'Type' field.";
						}
						if (DateTime.Parse(text2).Year == 1899)
						{
							return "Default values doesn't match 'Type' field.";
						}
						if (DateTime.Parse(text2).Year == 1900)
						{
							return "Default values doesn't match 'Type' field.";
						}
						if (Convert.ToDateTime(text2) == DateTime.MinValue)
						{
							return "Default values doesn't match 'Type' field.";
						}
						dataTable2.Rows[0]["Dflt"] = DateTime.Parse(text2).ToString("yyyy-MM-dd HH:mm:ss");
					}
					break;
				}
				if (text == "N" && string.IsNullOrEmpty(value))
				{
					if (editSize <= 6)
					{
						dataTable2.Rows[0]["SizeID"] = 6;
					}
					else
					{
						dataTable2.Rows[0]["SizeID"] = 11;
					}
				}
				DataTable dataTable6 = look_Key.Properties.DataSource as DataTable;
				if (grd_Line.DataSource is DataTable dataTable7)
				{
					DataTable dataTable8 = dataTable7.Copy();
					if (!dataTable8.IsNotNull())
					{
						dataTable8 = WinFuntions.GetTableStructure("UFD1");
					}
					if (Function.ToString(dataTable2.Rows[0]["TypeID"]) == "D")
					{
						dataTable8.Columns.Remove("FldValue");
						dataTable8.AcceptChanges();
						dataTable8.Columns.Add(new DataColumn("FldValue", typeof(string)));
						dataTable8.AcceptChanges();
						for (int num = 0; num < dataTable7.Rows.Count; num++)
						{
							DateTime? dateTime = Function.ParseDateTime(dataTable7.Rows[num]["FldValue"]);
							dataTable8.Rows[num]["FldValue"] = ((!dateTime.HasValue) ? "19000101" : dateTime.Value.ToString("yyyyMMdd"));
						}
						dataTable8.AcceptChanges();
					}
					for (int num2 = dataTable8.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow dataRow2 = dataTable8.Rows[num2];
						if (string.IsNullOrWhiteSpace(Function.ToString(dataRow2["FldValue"])) || Function.ToString(dataRow2["FldValue"]) == "19000101")
						{
							dataTable8.Rows.Remove(dataRow2);
							dataTable8.AcceptChanges();
						}
					}
					dataTable8.AcceptChanges();
					if (!dataTable6.IsNotNull())
					{
						dataTable6 = new DataTable();
					}
					DataTable dataTable9 = new DataTable();
					dataTable9.Columns.Add(new DataColumn("Action", typeof(string)));
					DataRow dataRow3 = dataTable9.NewRow();
					if (FormMode == Enums.FormViewModeType.Add)
					{
						dataRow3[0] = "A";
					}
					else
					{
						dataRow3[0] = "U";
					}
					dataTable9.Rows.Add(dataRow3);
					dataTable9.AcceptChanges();
					dataTable2.TableName = "doc";
					dataTable2.AcceptChanges();
					dataTable8.TableName = "valid";
					dataTable8.AcceptChanges();
					dataTable6.TableName = "key";
					dataTable6.AcceptChanges();
					_dtKeyFields.TableName = "key1";
					_dtKeyFields.AcceptChanges();
					dataTable9.TableName = "action";
					dataTable9.AcceptChanges();
					DataTable[] array2 = new DataTable[5] { dataTable2, dataTable8, dataTable6, _dtKeyFields, dataTable9 };
					for (int num3 = 0; num3 < array2.Length; num3++)
					{
						DataSet dataSet = array2[num3].DataSet;
						if (dataSet != null)
						{
							dataSet.Tables.Remove(array2[num3].TableName);
							dataSet.AcceptChanges();
						}
					}
					DataSet dataSet2 = new DataSet();
					dataSet2.Tables.AddRange(array2);
					using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UDF");
					return (await helper.PostAsync("Save", dataSet2)).Message;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	private void LoadInTreeList(DataTable dtOutb)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ID", typeof(int));
			dataTable.Columns.Add("ParentID", typeof(int));
			dataTable.Columns.Add("Descr", typeof(string));
			dataTable.Columns.Add("TableName", typeof(string));
			dataTable.AcceptChanges();
			dataTable.Rows.Add(1, -1, "Master Data", null);
			dataTable.Rows.Add(2, 1, "Activities", "OCLG");
			dataTable.Rows.Add(4, 1, "Bin Location", "OBIN");
			dataTable.Rows.Add(5, 1, "Blanket Agreement", null);
			dataTable.Rows.Add(6, 5, "Blanket Agreement", "AOAT");
			dataTable.Rows.Add(7, 5, "Blanket Agreement - Details", "AOA2");
			dataTable.Rows.Add(8, 5, "Blanket Agreement - Rows", "AOA1");
			dataTable.Rows.Add(9, 1, "Business Partner", null);
			dataTable.Rows.Add(10, 9, "Business Partner", "ACRD");
			dataTable.Rows.Add(11, 9, "Business Partners - Addresses", "ACR1");
			dataTable.Rows.Add(12, 9, "Contact Persons", "ACPR");
			dataTable.Rows.Add(20, 1, "Employees", null);
			dataTable.Rows.Add(21, 20, "Employees", "AHEM");
			dataTable.Rows.Add(22, 20, "Absence Information", "AHE1");
			dataTable.Rows.Add(23, 20, "Education", "AHE2");
			dataTable.Rows.Add(24, 20, "Employee Data Ownership Authorization", "HEM5");
			dataTable.Rows.Add(25, 20, "Employee Reviews", "AHE3");
			dataTable.Rows.Add(26, 20, "Employee Roles", "AHE6");
			dataTable.Rows.Add(27, 20, "Previous Employment", "AHE4");
			dataTable.Rows.Add(28, 20, "Savings Payments", "AHE7");
			dataTable.Rows.Add(29, 20, "Employee Relatives", "APZ_OHER");
			dataTable.Rows.Add(262, 20, "Health Status", "APZ_OHEH");
			dataTable.Rows.Add(263, 20, "Employee Contract", "APZ_OHEC");
			dataTable.Rows.Add(30, 1, "G/L Accounts", "AACT");
			dataTable.Rows.Add(31, 1, "Item Groups", "AITB");
			dataTable.Rows.Add(32, 1, "Items", null);
			dataTable.Rows.Add(33, 32, "Items", "AITM");
			dataTable.Rows.Add(34, 32, "Items - Warehouse", "AITW");
			dataTable.Rows.Add(35, 1, "Price Lists", "APLN");
			dataTable.Rows.Add(36, 1, "Project Codes", "APRJ");
			dataTable.Rows.Add(37, 1, "Project Plan", "APJT");
			dataTable.Rows.Add(42, 1, "Sales Employee", "OSLP");
			dataTable.Rows.Add(43, 1, "Users", "AUSR");
			dataTable.Rows.Add(44, 1, "Warehouses", "AWHS");
			dataTable.Rows.Add(48, -1, "Marketing Documents", null);
			dataTable.Rows.Add(49, 48, "Title", "ADOC");
			dataTable.Rows.Add(50, 48, "Rows", "ADO1");
			dataTable.Rows.Add(51, 48, "Document Freight Charges", "ADO3");
			dataTable.Rows.Add(53, 48, "Installments", "ADO6");
			dataTable.Rows.Add(54, 48, "Package Content", "ADO7");
			dataTable.Rows.Add(55, 48, "Assembly Rows", "ADO14");
			dataTable.Rows.Add(73, -1, "Production", null);
			dataTable.Rows.Add(74, 73, "BoM - Title", "AITT");
			dataTable.Rows.Add(75, 73, "BoM - Rows", "ATT1");
			dataTable.Rows.Add(76, 73, "Production Order - Title", "AWOR");
			dataTable.Rows.Add(77, 73, "Production Order - Rows", "AWO1");
			dataTable.Rows.Add(80, 73, "Sales Forecast - Title", "OFCT");
			dataTable.Rows.Add(81, 73, "Sales Forecast - Rows", "FCT1");
			dataTable.Rows.Add(83, 73, "Production Order - Closing", "AWO3");
			dataTable.Rows.Add(96, -1, "Opportunities", null);
			dataTable.Rows.Add(97, 96, "Title", "AOPR");
			dataTable.Rows.Add(98, 96, "Stages", "OPR1");
			dataTable.Rows.Add(99, 96, "Partners", "OPR2");
			dataTable.Rows.Add(100, 96, "Competitors", "OPR3");
			dataTable.Rows.Add(101, 96, "Interests", "OPR4");
			dataTable.Rows.Add(102, 96, "Reasons", "OPR5");
			dataTable.Rows.Add(103, -1, "Service", null);
			dataTable.Rows.Add(109, 103, "Contract - Title", "OCTR");
			dataTable.Rows.Add(110, 103, "Contract - Rows", "CTR1");
			dataTable.Rows.Add(111, 103, "Service Contract Template", "OCTT");
			dataTable.Rows.Add(112, 103, "Service Call Solutions", "OSLT");
			dataTable.Rows.Add(113, 103, "Customer Equipment Card", "AINS");
			dataTable.Rows.Add(114, 103, "Service Call Scheduling", "ASC6");
			dataTable.Rows.Add(115, 103, "Service Call BP Address", "ASC7");
			dataTable.Rows.Add(1115, 103, "Activities Management", "APZ_OCLG");
			dataTable.Rows.Add(211, 103, "Service Call", "APZ_OSCL");
			dataTable.Rows.Add(116, -1, "Inventory", null);
			dataTable.Rows.Add(119, 116, "Customer / Vendor Cat. No.", "OSCN");
			dataTable.Rows.Add(124, 116, "Inventory Opening Balance", "AIQI");
			dataTable.Rows.Add(125, 116, "Inventory Opening Balance - Row", "AQI1");
			dataTable.Rows.Add(126, 116, "Inventory Posting", "AINC");
			dataTable.Rows.Add(127, 116, "Inventory Posting - Row", "AIN1");
			dataTable.Rows.Add(128, 116, "Inventory Counting Transactions - UoM", "AIN2");
			dataTable.Rows.Add(129, 116, "Inventory Counting", null);
			dataTable.Rows.Add(130, 129, "Inventory Counting - Title", "APZ_OINC");
			dataTable.Rows.Add(184, 129, "Inventory Counting - Row", "APZ_INC1");
			dataTable.Rows.Add(185, 129, "Inventory Counting - Batch", "APZ_INC2");
			dataTable.Rows.Add(131, 116, "Special Prices for BP/Period and Volume Discounts", "ASPP");
			dataTable.Rows.Add(132, 116, "Special Prices for BP/Per. and Vol. Discounts - Period Discounts", "ASP1");
			dataTable.Rows.Add(133, 116, "Special Prices for BP/Per. and Vol. Discounts - Volume Discounts", "ASP2");
			dataTable.Rows.Add(134, 116, "Serial/Batch Number Object", "ABTN");
			dataTable.Rows.Add(154, 116, "Picking document", null);
			dataTable.Rows.Add(155, 154, "Title", "APZ_OPKL");
			dataTable.Rows.Add(156, 154, "Rows", "APZ_PKL1");
			dataTable.Rows.Add(157, 154, "Batch/Serial", "APZ_PKL2");
			dataTable.Rows.Add(158, 116, "Packing", null);
			dataTable.Rows.Add(159, 158, "Title", "APZ_OPAK");
			dataTable.Rows.Add(160, 158, "Rows", "APZ_PAK1");
			dataTable.Rows.Add(161, 116, "Shipping", null);
			dataTable.Rows.Add(162, 161, "Title", "APZ_SHIP");
			dataTable.Rows.Add(163, 161, "Rows", "APZ_SHIP1");
			dataTable.Rows.Add(170, -1, "MRP", null);
			dataTable.Rows.Add(171, 170, "Advance Forecast", null);
			dataTable.Rows.Add(172, 171, "Title", "APZ_OFCT");
			dataTable.Rows.Add(173, 171, "Rows", "APZ_FCT1");
			dataTable.Rows.Add(164, -1, "Project Management", null);
			dataTable.Rows.Add(165, 164, "Project Management", null);
			dataTable.Rows.Add(166, 165, "Title", "APZ_OPRG");
			dataTable.Rows.Add(167, 165, "Work breakdown Structure", "APZ_PRG1");
			dataTable.Rows.Add(168, 165, "Item Budget", "APZ_PRG4");
			dataTable.Rows.Add(183, 165, "Payment", "APZ_PRG6");
			dataTable.Rows.Add(169, 164, "Tracking Issue", "APZ_TGIE");
			dataTable.Rows.Add(177, 164, "Report Completion", null);
			dataTable.Rows.Add(178, 177, "Title", "APZ_ORTC");
			dataTable.Rows.Add(179, 177, "Rows", "APZ_RTC1");
			dataTable.Rows.Add(180, 164, "Confirm", null);
			dataTable.Rows.Add(181, 180, "Title", "APZ_OPJC");
			dataTable.Rows.Add(182, 180, "Rows", "APZ_PJC1");
			dataTable.Rows.Add(174, -1, "HumanResources");
			dataTable.Rows.Add(175, 174, "Employee Penalty/Bonus", "APZ_HPT1");
			dataTable.Rows.Add(216, 174, "TimeSheeting", "APZ_OTSL");
			dataTable.Rows.Add(258, 174, "TimeSheeting In/Out", null);
			dataTable.Rows.Add(259, 258, "Title", "APZ_OIOD");
			dataTable.Rows.Add(260, 258, "Time Sheeting", "APZ_IOD1");
			dataTable.Rows.Add(261, 258, "Time Sheeting OT", "APZ_IOD2");
			dataTable.Rows.Add(176, 174, "Payroll", "APZ_OSAL");
			dataTable.Rows.Add(210, 174, "Applicants", "APZ_OHAP");
			dataTable.Rows.Add(217, 174, "Salary Information", "APZ_OHSI");
			dataTable.Rows.Add(230, 174, "Work Type", "APZ_OHWT");
			dataTable.Rows.Add(231, 174, "Work Code", "APZ_OHWC");
			dataTable.Rows.Add(232, 174, "Work Period", "APZ_OHWP");
			dataTable.Rows.Add(233, 174, "Overtime Type", "APZ_OHOT");
			dataTable.Rows.Add(234, 174, "Holiday Setup - Title", "APZ_OHLD");
			dataTable.Rows.Add(235, 174, "Holiday Setup - Rows", "APZ_HLD1");
			dataTable.Rows.Add(236, 174, "In/Out Pick Up Method", "APZ_OIOM");
			dataTable.Rows.Add(237, 174, "Work Schedule Setup - Title", "APZ_OWSC");
			dataTable.Rows.Add(238, 174, "Work Schedule Setup - Week", "APZ_WSC1");
			dataTable.Rows.Add(239, 174, "Work Schedule Setup - Month", "APZ_WSC2");
			dataTable.Rows.Add(240, 174, "Work Schedule Setup - Year", "APZ_WSC3");
			dataTable.Rows.Add(241, 174, "Work Schedule Register", "APZ_OWSR");
			dataTable.Rows.Add(242, 174, "Allowance Type", "APZ_OHAT");
			dataTable.Rows.Add(243, 174, "Deduction Type - Title", "APZ_OHDT");
			dataTable.Rows.Add(244, 174, "Deduction Type - Detail", "APZ_HDT1");
			dataTable.Rows.Add(245, 174, "Personal Income Tax - Title", "APZ_OPIT");
			dataTable.Rows.Add(246, 174, "Personal Income Tax - Detail", "APZ_PIT1");
			dataTable.Rows.Add(247, 174, "Salary Parameters", "APZ_OHSP");
			dataTable.Rows.Add(248, 174, "Leave Parameter", "APZ_OSLP");
			dataTable.Rows.Add(249, 174, "Penalty/Bonus Type", "APZ_OHPT");
			dataTable.Rows.Add(186, 116, "Inventory Counting Request", "APZ_OCRQ");
			dataTable.Rows.Add(187, 103, "Warranty Card - Title", "APZ_OINS");
			dataTable.Rows.Add(188, 103, "Warranty Card - Row", "APZ_INS1");
			dataTable.Rows.Add(189, 103, "Service Level Agreement", null);
			dataTable.Rows.Add(190, 189, "Title", "APZ_OSLA");
			dataTable.Rows.Add(191, 189, "Coverage", "APZ_SLA1");
			dataTable.Rows.Add(192, 189, "Resolutions", "APZ_SLA2");
			dataTable.Rows.Add(193, 103, "Marketing Campaign", null);
			dataTable.Rows.Add(194, 193, "Marketing Campaign - Title", "APZ_OSMC");
			dataTable.Rows.Add(195, 193, "Target Bp", "APZ_SMC1");
			dataTable.Rows.Add(196, 193, "Items", "APZ_SMC2");
			dataTable.Rows.Add(197, 193, "Partner", "APZ_SMC3");
			dataTable.Rows.Add(198, 193, "Member", "APZ_SMC4");
			dataTable.Rows.Add(199, 193, "Cost", "APZ_SMC5");
			dataTable.Rows.Add(200, 174, "Job Vacancy", null);
			dataTable.Rows.Add(201, 200, "Title", "APZ_OHJV");
			dataTable.Rows.Add(202, 200, "Qualifications", "APZ_HJV7");
			dataTable.Rows.Add(203, 200, "Hiring Team", "APZ_HJV1");
			dataTable.Rows.Add(204, 200, "Advertisement", "APZ_HJV2");
			dataTable.Rows.Add(205, 200, "Applicants", "APZ_HJV3");
			dataTable.Rows.Add(206, 200, "Interview", "APZ_HJV4");
			dataTable.Rows.Add(207, 200, "Applicants Interview", "APZ_HJV8");
			dataTable.Rows.Add(208, 200, "Offer", "APZ_HJV5");
			dataTable.Rows.Add(209, 200, "Hired", "APZ_HJV6");
			dataTable.Rows.Add(212, 164, "WBS Template", null);
			dataTable.Rows.Add(213, 212, "Title", "APZ_OWBS");
			dataTable.Rows.Add(214, 212, "Wbs", "APZ_WBS1");
			dataTable.Rows.Add(215, 212, "Task", "APZ_TASK");
			dataTable.Rows.Add(218, -1, "KPI", null);
			dataTable.Rows.Add(219, 218, "KPI Plans - Title", "APZ_OKPN");
			dataTable.Rows.Add(254, 218, "KPI Plans - Row", "APZ_KPN1");
			dataTable.Rows.Add(220, 218, "KPI Scopes - Title", "APZ_OKSE");
			dataTable.Rows.Add(250, 218, "KPI Scopes - Rows", "APZ_KSE1");
			dataTable.Rows.Add(221, 218, "KPI Indicators - Title", "APZ_OKIR");
			dataTable.Rows.Add(255, 218, "KPI Indicators - Rows", "APZ_KIR1");
			dataTable.Rows.Add(222, 218, "KPI Groups - Title", "APZ_OKGP");
			dataTable.Rows.Add(251, 218, "KPI Groups - Employee Rows", "APZ_KGP1");
			dataTable.Rows.Add(252, 218, "KPI Groups - Indicator Rows", "APZ_KGP2");
			dataTable.Rows.Add(223, 218, "KPI Setups - Title", "APZ_OKSP");
			dataTable.Rows.Add(256, 218, "KPI Setups - Rows", "APZ_KSP1");
			dataTable.Rows.Add(224, 218, "KPI Scores - Title", "APZ_OKSC");
			dataTable.Rows.Add(257, 218, "KPI Scores - Rows", "APZ_KSC1");
			dataTable.Rows.Add(225, -1, "Promotions", null);
			dataTable.Rows.Add(226, 225, "Promotion Conditions - Title", "APZ_OPCN");
			dataTable.Rows.Add(227, 225, "Promotion Conditions - Row", "APZ_PCN1");
			dataTable.Rows.Add(228, 225, "Promotion Benefits - Title", "APZ_OPBF");
			dataTable.Rows.Add(229, 225, "Promotion Benefit - Row", "APZ_PBF1");
			dataTable.Rows.Add(253, 225, "Promotion Programs - Title", "APZ_OPPR");
			int num = 10000;
			dataTable.Rows.Add(num, -1, "User Tables", null);
			int num2 = num + 1;
			foreach (DataRow row in dtOutb.Rows)
			{
				string text = "@" + Function.ToString(row["TableName"]);
				dataTable.Rows.Add(num2, num, row["Descr"], text);
				num2++;
			}
			dataTable.AcceptChanges();
			tv_UDF.DataSource = dataTable;
			tv_UDF.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadSysObj(bool irsObject)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Type", typeof(string));
			dataTable.Columns.Add("Tbl", typeof(string));
			dataTable.Columns.Add("Descr", typeof(string));
			dataTable.AcceptChanges();
			dataTable.Rows.Add("1", "OACT", "Accounts");
			dataTable.Rows.Add("2", "OCRD", "Business Partners");
			dataTable.Rows.Add("3", "ODSC", "Bank Codes");
			dataTable.Rows.Add("4", "OITM", "Items");
			dataTable.Rows.Add("12", "OUSR", "Users");
			dataTable.Rows.Add("13", "OINV", "A/R Invoices");
			dataTable.Rows.Add("14", "ORIN", "A/R Credit Memos");
			dataTable.Rows.Add("15", "ODLN", "Deliveries");
			dataTable.Rows.Add("16", "ORDN", "Returns");
			dataTable.Rows.Add("17", "ORDR", "Sales Orders");
			dataTable.Rows.Add("18", "OPCH", "A/P Invoices");
			dataTable.Rows.Add("19", "ORPC", "A/P Credit Memos");
			dataTable.Rows.Add("20", "OPDN", "Goods Receipt PO");
			dataTable.Rows.Add("21", "ORPD", "Goods Return");
			dataTable.Rows.Add("22", "OPOR", "Purchase Orders");
			dataTable.Rows.Add("23", "OQUT", "Sales Quotations");
			dataTable.Rows.Add("24", "ORCT", "Incoming Payments");
			dataTable.Rows.Add("25", "ODPS", "Deposit");
			dataTable.Rows.Add("30", "OJDT", "Journal Entries");
			dataTable.Rows.Add("33", "OCLG", "Activities");
			dataTable.Rows.Add("46", "OVPM", "Outgoing Payments");
			dataTable.Rows.Add("57", "OCHO", "Checks for Payment");
			dataTable.Rows.Add("59", "OIGN", "Goods Receipt");
			dataTable.Rows.Add("60", "OIGE", "Goods Issue");
			dataTable.Rows.Add("64", "OWHS", "Warehouses");
			dataTable.Rows.Add("66", "OITT", "Product Tree");
			dataTable.Rows.Add("67", "OWTR", "Inventory Transfer");
			dataTable.Rows.Add("97", "OOPR", "Opportunity");
			dataTable.Rows.Add("112", "ODRF", "Drafts");
			dataTable.Rows.Add("162", "OMRV", "Inventory Revaluation");
			dataTable.Rows.Add("171", "OHEM", "Employees");
			dataTable.Rows.Add("176", "OINS", "Customer Equipment Card");
			dataTable.Rows.Add("190", "OCTR", "Service Contracts");
			dataTable.Rows.Add("191", "OSCL", "Service Calls");
			dataTable.Rows.Add("202", "OWOR", "Production Order");
			dataTable.Rows.Add("1250000001", "OWTQ", "Inventory Transfer Request");
			dataTable.Rows.Add("1250000025", "OOAT", "Blanket Agreement");
			dataTable.Rows.Add("234000021", "OPMG", "Project Management");
			dataTable.Rows.Add("234000031", "ORRR", "Return Request");
			dataTable.Rows.Add("234000032", "OPRR", "Goods Return Request");
			if (irsObject)
			{
				dataTable.Rows.Add("1470000128", "APZ_OCLG", "Task Management");
				dataTable.Rows.Add("1470000129", "APZ_OCLG", "Event Management");
				dataTable.Rows.Add("19000102", "APZ_OCLG", "Note Management");
				dataTable.Rows.Add("1470000130", "APZ_TGIE", "Tracking Issue");
				dataTable.Rows.Add("1470000132", "APZ_OPRG", "IRS Project Management");
			}
			dataTable.AcceptChanges();
			rep_LinkSysObj.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_UserDefineFieldsManagement_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInItForm();
			await LoadData();
			btn_Keys.Enabled = false;
			btn_Update.Enabled = false;
			btn_Remove.Enabled = false;
			xtraTabPage1.PageVisible = true;
			xtraTabPage2.PageVisible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (!(gv_Line.FocusedColumn.FieldName == "FldValue"))
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("TypeID"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("EditType"));
			if (text == "N" && text2 == "T")
			{
				string text3 = string.Join("", from c in e.Value.ToString()
					where char.IsDigit(c)
					select c);
				if (text3.Length > 4)
				{
					text3 = text3.Remove(0, text3.Length - 4);
				}
				text3 = Function.ParseInt(text3).ToString("0000");
				int num = Function.ParseInt(text3.Substring(0, 2));
				int num2 = Function.ParseInt(text3.Substring(2, 2));
				if (num < 0 || num > 23 || num2 < 0 || num2 > 59)
				{
					WinCommons.Error(Enums.MessageCode.InvalidTimeString, "");
					e.Value = "";
				}
				else if (text3 != "0000")
				{
					e.Value = text3;
				}
				else
				{
					e.Value = "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
					grd_Doc.DataSourceTable.RejectChanges();
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

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_isFocusedRowObjectChanging = true;
		try
		{
			if (_isFocusedRowChanging)
			{
				_isFocusedRowObjectChanging = false;
				return;
			}
			await LoadChildUdfData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isFocusedRowObjectChanging = false;
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_isFocusedRowChanging = true;
		try
		{
			if (_isFocusedRowObjectChanging)
			{
				_isFocusedRowChanging = false;
				return;
			}
			await LoadChildUdfData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isFocusedRowChanging = false;
	}

	private void tv_UDF_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				_isNodeChanging = true;
				e.CanFocus = false;
				_isNodeChanging = false;
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
	}

	private async void tv_UDF_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			if (!_isNodeChanging)
			{
				await GetFocusNode();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.FocusedRow;
			}
			else
			{
				if (FormMode == Enums.FormViewModeType.Add)
				{
					return;
				}
				GridView gridView = sender as GridView;
				if ((e.Column.FieldName == "AliasID" || e.Column.FieldName == "TypeID" || e.Column.FieldName == "EditType") && gridView != null && Function.ParseInt(gridView.GetRowCellValue(e.RowHandle, "FieldID")) <= _codeMax)
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				if (gridView != null)
				{
					string text = Function.ToString(gridView.GetRowCellValue(e.RowHandle, gridColumn32));
					string text2 = Function.ToString(gridView.GetRowCellValue(e.RowHandle, gridColumn5));
					if ((e.Column.FieldName == "TypeID1" || e.Column.FieldName == "EditType1" || e.Column.FieldName == "Dflt" || e.Column.FieldName == "NotNull") && text != "N" && text != "V")
					{
						e.Appearance.BackColor = Color.LightGray;
					}
					if (e.Column.FieldName == "EditSize")
					{
						if (text2 == "A" && Function.ToString(gridView.GetRowCellValue(e.RowHandle, gridColumn6)) != "")
						{
							e.Appearance.BackColor = Color.LightGray;
						}
						else if (text2 != "N" && text2 != "A")
						{
							e.Appearance.BackColor = Color.LightGray;
						}
						else if (text == "LS" || text == "LU")
						{
							e.Appearance.BackColor = Color.LightGray;
						}
					}
					if (e.Column.FieldName == "EditType" && text2 == "N")
					{
						e.Appearance.BackColor = Color.LightGray;
					}
					if (e.Column.FieldName == "RTable" && text != "LT")
					{
						e.Appearance.BackColor = Color.LightGray;
					}
					if (e.Column.FieldName == "RelUDO" && text != "LU")
					{
						e.Appearance.BackColor = Color.LightGray;
					}
					if (e.Column.FieldName == "RelSO" && text != "LS")
					{
						e.Appearance.BackColor = Color.LightGray;
					}
				}
				string text3 = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "TypeID"));
				string text4 = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "EditType"));
				if ((FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && (arrGreyColsWhenUpdate.Contains(e.Column.FieldName) || (((!(text3 == "N") && !(text3 == "A")) || !(text4 == "")) && e.Column.FieldName == "EditSize")))
				{
					e.Appearance.BackColor = Color.LightGray;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (!(grd_Doc.DataSource as DataTable).IsNotNull())
			{
				return;
			}
			if (e.Column.FieldName == "EditType1")
			{
				string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "TypeID1"));
				if (string.IsNullOrWhiteSpace(text))
				{
					return;
				}
				switch (text)
				{
				case "A":
					e.RepositoryItem = _typeEditTypesAlphaNumeric;
					break;
				case "D":
					e.RepositoryItem = _typeEditTypesDateTime;
					break;
				case "B":
					e.RepositoryItem = _typeEditTypesUnitandTotal;
					break;
				case "M":
					e.RepositoryItem = _typeEditTypesGeneral;
					break;
				}
			}
			if (!(e.Column.FieldName == "Dflt"))
			{
				return;
			}
			string text2 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Validation"));
			Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "TableID"));
			int fieldId = Function.ParseInt(gv_Doc.GetRowCellValue(e.RowHandle, "FieldID"));
			if (text2.Equals("V"))
			{
				e.RepositoryItem = _listLuk.Find((RepositoryItemLookUpEdit item) => item.Name == Function.ToString(fieldId));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void gv_Valid_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "FldValue")
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("TypeID"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("EditType"));
			RepositoryItemDateEditEx repositoryItem = new RepositoryItemDateEditEx();
			RepositoryItemCalcEdit repositoryItem2 = new RepositoryItemCalcEdit();
			RepositoryItemTextEdit repositoryItem3 = new RepositoryItemTextEdit();
			if ((grd_Doc.DataSource as DataTable).IsNotNull())
			{
				if (text == "D")
				{
					e.RepositoryItem = repositoryItem;
				}
				else if (text == "N" && text2 == "T")
				{
					e.RepositoryItem = _timeEdit;
				}
				else if (text == "N" || text == "B")
				{
					e.RepositoryItem = repositoryItem2;
				}
				else
				{
					e.RepositoryItem = repositoryItem3;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Valid_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			if ((gv_Doc.FocusedColumn.FieldName == "AliasID" || gv_Doc.FocusedColumn.FieldName == "TypeID1" || gv_Doc.FocusedColumn.FieldName == "EditType1") && Function.ParseInt(gv_Doc.GetFocusedRowCellValue("FieldID")) <= _codeMax)
			{
				e.Cancel = true;
			}
			if ((gv_Doc.FocusedColumn.FieldName == "TypeID1" || gv_Doc.FocusedColumn.FieldName == "EditType1" || gv_Doc.FocusedColumn.FieldName == "Dflt" || gv_Doc.FocusedColumn.FieldName == "NotNull") && !gv_Doc.GetFocusedRowCellValue("Validation").Equals("N") && !gv_Doc.GetFocusedRowCellValue("Validation").Equals("V"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "EditSize")
			{
				if (gv_Doc.GetFocusedRowCellValue("TypeID1").Equals("A") && !Function.ToString(gv_Doc.GetFocusedRowCellValue("EditType1")).Equals(""))
				{
					e.Cancel = true;
				}
				else if (!gv_Doc.GetFocusedRowCellValue("TypeID1").Equals("N") && !gv_Doc.GetFocusedRowCellValue("TypeID1").Equals("A"))
				{
					e.Cancel = true;
				}
				if (gv_Doc.GetFocusedRowCellValue("Validation").Equals("LS") || gv_Doc.GetFocusedRowCellValue("Validation").Equals("LU"))
				{
					e.Cancel = true;
				}
			}
			if (gv_Doc.FocusedColumn.FieldName == "EditType1" && gv_Doc.GetFocusedRowCellValue("TypeID1").Equals("N"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "RTable" && !gv_Doc.GetFocusedRowCellValue("Validation").Equals("LT"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "RelUDO" && !gv_Doc.GetFocusedRowCellValue("Validation").Equals("LU"))
			{
				e.Cancel = true;
			}
			if (gv_Doc.FocusedColumn.FieldName == "RelSO" && !gv_Doc.GetFocusedRowCellValue("Validation").Equals("LS"))
			{
				e.Cancel = true;
			}
			if ((gv_Doc.FocusedColumn.FieldName == "RelUDO" || gv_Doc.FocusedColumn.FieldName == "RelSO") && Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID")).StartsWith("@"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Valid_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if ((gv_Line.FocusedColumn.FieldName == "FldValue" || gv_Line.FocusedColumn.FieldName == "Descr") && !gv_Doc.GetFocusedRowCellValue("Validation").Equals("V"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "TypeID1")
			{
				switch (Function.ToString(e.Value))
				{
				case "A":
					gv_Doc.SetRowCellValue(e.RowHandle, "EditType1", "");
					break;
				case "N":
					gv_Doc.SetRowCellValue(e.RowHandle, "EditType1", "");
					break;
				case "D":
					gv_Doc.SetRowCellValue(e.RowHandle, "EditType1", "");
					break;
				case "B":
					gv_Doc.SetRowCellValue(e.RowHandle, "EditType1", "R");
					break;
				case "M":
					gv_Doc.SetRowCellValue(e.RowHandle, "EditType1", "B");
					break;
				}
			}
			if (e.Column.FieldName == "EditType1")
			{
				string text = Function.ToString(e.Value);
				string text2 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "TypeID1"));
				gv_Doc.SetRowCellValue(e.RowHandle, "EditType", text);
				switch (text2)
				{
				case "A":
					if (text == "B")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "TypeID", "M");
						gv_Doc.SetRowCellValue(e.RowHandle, "EditType", "");
					}
					else
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "TypeID", "A");
					}
					break;
				case "N":
					gv_Doc.SetRowCellValue(e.RowHandle, "TypeID", "N");
					break;
				case "D":
					gv_Doc.SetRowCellValue(e.RowHandle, "TypeID", (text == "") ? "D" : "N");
					break;
				case "B":
					gv_Doc.SetRowCellValue(e.RowHandle, "TypeID", "B");
					break;
				case "M":
					gv_Doc.SetRowCellValue(e.RowHandle, "TypeID", "M");
					break;
				}
			}
			if (e.Column.FieldName == "TypeID")
			{
				string text3 = Function.ToString(e.Value);
				string text4 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "EditType"));
				gv_Doc.SetRowCellValue(e.RowHandle, "Dflt", DBNull.Value);
				switch (text3.ToUpper())
				{
				case "B":
					if (string.IsNullOrEmpty(text4) || text4.ToUpper() == "R")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 10);
					}
					else if (text4.ToUpper() == "P" || text4.ToUpper() == "Q" || text4.ToUpper() == "%" || text4.ToUpper() == "M")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 16);
					}
					else if (text4.ToUpper() == "S")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 16);
						gv_Doc.SetRowCellValue(e.RowHandle, "Dflt", 0.0);
					}
					break;
				case "N":
					if (string.IsNullOrEmpty(text4))
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", 10);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 10);
					}
					else if (text4.ToUpper() == "T")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 6);
					}
					break;
				case "A":
					if (string.IsNullOrEmpty(text4))
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", 10);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 10);
					}
					else if (text4.ToUpper() == "I")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 200);
					}
					else if (text4.ToUpper() == "?")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 254);
					}
					else if (text4.ToUpper() == "#")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 20);
					}
					break;
				case "M":
					if (string.IsNullOrEmpty(text4))
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 4);
					}
					else if (text4.ToUpper() == "B")
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
						gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 4);
					}
					break;
				case "D":
					gv_Doc.SetRowCellValue(e.RowHandle, "EditSize", DBNull.Value);
					gv_Doc.SetRowCellValue(e.RowHandle, "SizeID", 8);
					break;
				}
			}
			if (e.Column.FieldName == "Validation")
			{
				if (gv_Doc.GetRowCellValue(e.RowHandle, "Validation").Equals("N") || gv_Doc.GetRowCellValue(e.RowHandle, "Validation").Equals("V"))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "RTable", null);
					gv_Doc.SetRowCellValue(e.RowHandle, "RelUDO", null);
					gv_Doc.SetRowCellValue(e.RowHandle, "RelSO", null);
				}
				if (gv_Doc.GetRowCellValue(e.RowHandle, "Validation").Equals("LS"))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "RTable", null);
					gv_Doc.SetRowCellValue(e.RowHandle, "RelUDO", null);
					if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "TableID")).StartsWith("@"))
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "Validation", "N");
					}
				}
				if (gv_Doc.GetRowCellValue(e.RowHandle, "Validation").Equals("LT"))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "RelUDO", null);
					gv_Doc.SetRowCellValue(e.RowHandle, "RelSO", null);
				}
				if (gv_Doc.GetRowCellValue(e.RowHandle, "Validation").Equals("LU"))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "RTable", null);
					gv_Doc.SetRowCellValue(e.RowHandle, "RelSO", null);
					if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "TableID")).StartsWith("@"))
					{
						gv_Doc.SetRowCellValue(e.RowHandle, "Validation", "N");
					}
				}
			}
			if (e.Column.FieldName == "AliasID" || e.Column.FieldName == "Descr" || e.Column.FieldName == "TypeID" || e.Column.FieldName == "EditType" || e.Column.FieldName == "EditSize" || e.Column.FieldName == "Dflt" || e.Column.FieldName == "NotNull" || e.Column.FieldName == "IndexID" || e.Column.FieldName == "RTable" || e.Column.FieldName == "RelUDO" || e.Column.FieldName == "ValidRule")
			{
				SetButtonOKTextToUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void edit_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			string text = Function.ParseInt(e.Value).ToString("0000");
			if (text == "0000")
			{
				e.DisplayText = "";
				return;
			}
			text = text.Insert(2, ":");
			e.DisplayText = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Keys_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (grd_Keys.DataSource is DataTable dataTable)
			{
				if (Function.ToString(e.Value).Equals(""))
				{
					gv_Keys.BeginUpdate();
					gv_Keys.DeleteSelectedRows();
					gv_Keys.EndUpdate();
					grd_Keys.DataSourceTable.AcceptChanges();
					grd_Keys.RefreshDataSource();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["ColAlias"])).Distinct().ToList().Count < dataTable.Rows.Count)
				{
					gv_Keys.BeginUpdate();
					gv_Keys.DeleteSelectedRows();
					gv_Keys.EndUpdate();
					grd_Keys.DataSourceTable.AcceptChanges();
					grd_Keys.RefreshDataSource();
					WinCommons.Error(Enums.MessageCode.InFieldNameColumnYouCannotEnterDuplicateValue, "");
				}
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ColAlias"]))
					select t).ToList();
				if (_dtKeyFields.IsNotNull() && list.Count > 0)
				{
					string tblName = Function.ToString(list[0]["TableName"]);
					int keyId = Function.ParseInt(list[0]["KeyId"]);
					List<DataRow> list2 = (from r in _dtKeyFields.AsEnumerable()
						where Function.ToString(r["TableName"]) == tblName && Function.ParseInt(r["KeyId"]) == keyId
						select r).ToList();
					if (list2.Count > 0)
					{
						foreach (DataRow item in list2)
						{
							_dtKeyFields.Rows.Remove(item);
						}
					}
					_dtKeyFields.AcceptChanges();
				}
				foreach (DataRow item2 in list)
				{
					DataRow dataRow = _dtKeyFields.NewRow();
					dataRow["TableName"] = item2["TableName"];
					dataRow["KeyId"] = item2["KeyId"];
					dataRow["SubKeyId"] = item2["SubKeyId"];
					dataRow["ColAlias"] = item2["ColAlias"];
					_dtKeyFields.Rows.Add(dataRow);
				}
			}
			AddNewKeys();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Valid_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Line.DataSource as DataTable;
			string tableId = Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID"));
			int fieldId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("FieldID"));
			if (dataTable != null)
			{
				List<IGrouping<string, DataRow>> list = (from i in dataTable.AsEnumerable()
					group i by Function.ToString(i["FldValue"]).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList();
				if (list.Count > 0 && !string.IsNullOrWhiteSpace(Function.ToString(list[0].FirstOrDefault()["FldValue"])))
				{
					WinCommons.Error(Enums.MessageCode.InValueColumnYouCannotEnterDuplicateValues, "");
					return;
				}
			}
			AddNewGridValue(tableId, fieldId);
			SetButtonOKTextToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void look_Key_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				DataTable dataTable = look_Key.Properties.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					string tblName = Function.ToString(dataTable.Rows[0]["TableName"]);
					int keyId = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
					await GetUkd1(tblName, keyId);
					txt_KeyName.Text = Function.ToString(look_Key.GetColumnValue("KeyName"));
					chk_Unique.EditValue = Function.ToString(look_Key.GetColumnValue("UniqueKey"));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_KeyName_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (look_Key.Properties.DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Clone();
				if (dataTable.IsNotNull())
				{
					int num = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
					foreach (DataRow row in dataTable.Rows)
					{
						if (Function.ParseInt(row["KeyId"]) == num)
						{
							DataRow dataRow2 = dataTable2.NewRow();
							dataRow2["TableName"] = row["TableName"];
							dataRow2["KeyId"] = row["KeyId"];
							dataRow2["UniqueKey"] = row["UniqueKey"];
							dataRow2["Action"] = row["Action"];
							dataRow2["KeyName"] = txt_KeyName.Text;
							dataTable2.Rows.Add(dataRow2);
						}
						else
						{
							DataRow dataRow3 = dataTable2.NewRow();
							dataRow3["TableName"] = row["TableName"];
							dataRow3["KeyId"] = row["KeyId"];
							dataRow3["UniqueKey"] = row["UniqueKey"];
							dataRow3["Action"] = row["Action"];
							dataRow3["KeyName"] = row["KeyName"];
							dataTable2.Rows.Add(dataRow3);
						}
					}
				}
				look_Key.Properties.DataSource = dataTable2;
			}
			int num2 = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
			look_Key.EditValue = num2;
			SetButtonOKTextToUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_Unique_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (look_Key.Properties.DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Clone();
				if (dataTable.IsNotNull())
				{
					int num = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
					foreach (DataRow row in dataTable.Rows)
					{
						if (Function.ParseInt(row["KeyId"]) == num)
						{
							DataRow dataRow2 = dataTable2.NewRow();
							dataRow2["TableName"] = row["TableName"];
							dataRow2["KeyId"] = row["KeyId"];
							dataRow2["UniqueKey"] = chk_Unique.EditValue;
							dataRow2["Action"] = row["Action"];
							dataRow2["KeyName"] = row["KeyName"];
							dataTable2.Rows.Add(dataRow2);
						}
						else
						{
							DataRow dataRow3 = dataTable2.NewRow();
							dataRow3["TableName"] = row["TableName"];
							dataRow3["KeyId"] = row["KeyId"];
							dataRow3["UniqueKey"] = row["UniqueKey"];
							dataRow3["Action"] = row["Action"];
							dataRow3["KeyName"] = row["KeyName"];
							dataTable2.Rows.Add(dataRow3);
						}
					}
				}
				look_Key.Properties.DataSource = dataTable2;
			}
			int num2 = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
			look_Key.EditValue = num2;
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Type_EditValueChanged(object sender, EventArgs e)
	{
		gv_Doc.PostEditor();
	}

	private void rep_Validation_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.PostEditor();
			gv_Doc.SetFocusedRowCellValue("Dflt", "");
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("Validation")) == "LT")
			{
				gv_Doc.SetFocusedRowCellValue("TypeID", "A");
				gv_Doc.SetFocusedRowCellValue("EditType", " ");
				gv_Doc.SetFocusedRowCellValue("EditSize", 50);
			}
			string tableId = Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID"));
			int fieldId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("FieldID"));
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("Validation")).Equals("V"))
			{
				grd_Line.DataSource = WinFuntions.GetTableStructure("UFD1");
				grd_Line.RefreshDataSource();
			}
			else
			{
				grd_Line.DataSource = WinFuntions.GetTableStructure("UFD1");
				grd_Line.RefreshDataSource();
			}
			AddNewGridValue(tableId, fieldId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_LinkToTable_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rep_LinkUdo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.PostEditor();
			string tblCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("RelUDO"));
			if (!(rep_LinkUdo.DataSource is DataTable source))
			{
				return;
			}
			string tblName = (from t in source.AsEnumerable()
				where Function.ToString(t["Code"]) == tblCode
				select Function.ToString(t["TableName"])).FirstOrDefault();
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF");
			DataTable dataTable = await helper.GetAsync("GetKeyFieldOUDO", new { tblName });
			if (dataTable.IsNotNull())
			{
				gv_Doc.SetFocusedRowCellValue("TypeID", Function.ToString(dataTable.Rows[0]["Type"]));
				gv_Doc.SetFocusedRowCellValue("EditType", " ");
				gv_Doc.SetFocusedRowCellValue("EditSize", Function.ToString(dataTable.Rows[0]["MaxLength"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_LinkSysObj_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Doc.PostEditor();
			if (gv_Doc.GetFocusedRowCellValue("RelSO").Equals("1") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("2"))
			{
				gv_Doc.SetFocusedRowCellValue("TypeID", "A");
				gv_Doc.SetFocusedRowCellValue("EditType", " ");
				gv_Doc.SetFocusedRowCellValue("EditSize", 15);
			}
			if (gv_Doc.GetFocusedRowCellValue("RelSO").Equals("4"))
			{
				gv_Doc.SetFocusedRowCellValue("TypeID", "A");
				gv_Doc.SetFocusedRowCellValue("EditType", " ");
				gv_Doc.SetFocusedRowCellValue("EditSize", 50);
			}
			if (gv_Doc.GetFocusedRowCellValue("RelSO").Equals("13") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("14") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("15") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("16") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("17") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("18") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("19") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("20") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("21") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("22") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("23") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("24") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("30") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("46") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("59") || gv_Doc.GetFocusedRowCellValue("RelSO").Equals("60"))
			{
				gv_Doc.SetFocusedRowCellValue("TypeID", "N");
				gv_Doc.SetFocusedRowCellValue("EditType", " ");
				gv_Doc.SetFocusedRowCellValue("EditSize", 11);
			}
			if (gv_Doc.GetFocusedRowCellValue("RelSO").Equals("64"))
			{
				gv_Doc.SetFocusedRowCellValue("TypeID", "A");
				gv_Doc.SetFocusedRowCellValue("EditType", " ");
				gv_Doc.SetFocusedRowCellValue("EditSize", 8);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Remove_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_New_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Delete.Enabled = true;
			if (!(look_Key.Properties.DataSource is DataTable dataTable))
			{
				return;
			}
			List<DataRow> list = (from t in dataTable.AsEnumerable()
				where string.IsNullOrEmpty(Function.ToString(t["KeyName"]))
				select t).ToList();
			if (list.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.KeyNameIsNotEmpty, "");
				look_Key.EditValue = list[0]["KeyId"];
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			int num = 0;
			if (dataTable.IsNotNull())
			{
				num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["KeyId"])) + 1;
			}
			dataRow["TableName"] = Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID"));
			dataRow["KeyId"] = num;
			dataRow["UniqueKey"] = "N";
			dataRow["Action"] = "N";
			dataRow["KeyName"] = "IX_" + Function.ToString(dataTable.Rows.Count);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			look_Key.Properties.DataSource = dataTable;
			look_Key.EditValue = Function.ParseShort(num);
			if (num == 0)
			{
				string tblName = Function.ToString(dataTable.Rows[0]["TableName"]);
				int keyId = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
				await GetUkd1(tblName, keyId);
				txt_KeyName.Text = Function.ToString(look_Key.GetColumnValue("KeyName"));
				chk_Unique.EditValue = Function.ToString(look_Key.GetColumnValue("UniqueKey"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Delete_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = look_Key.Properties.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				int keyId = Function.ParseInt(look_Key.GetColumnValue("KeyId"));
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["KeyId"]) == keyId);
				if (dataRow != null)
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
			}
			look_Key.Properties.DataSource = dataTable;
			if (dataTable.IsNotNull())
			{
				look_Key.EditValue = dataTable.Rows[0]["KeyId"];
			}
			else
			{
				txt_KeyName.EditValue = "";
				grd_Keys.DataSourceTable.Rows.Clear();
				grd_Keys.DataSourceTable.AcceptChanges();
				chk_Unique.EditValue = "N";
				btn_Delete.Enabled = false;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Keys_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (!xtraTabPage2.PageVisible)
			{
				string tableCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID"));
				await GetListKeyAlias(tableCode);
				await GetStructureUkd1();
				await GetOukd(tableCode);
				xtraTabPage2.PageVisible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			tv_UDF.ExpandAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			tv_UDF.CollapseAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				_isCancel = true;
				Close();
				_isCancel = false;
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmAddUdf, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				FormCollection openForms = Application.OpenForms;
				for (int i = 0; i < openForms.Count; i++)
				{
					Form openForm = openForms[i];
					if (openForm != this && !openForm.IsMdiContainer)
					{
						Invoke((MethodInvoker)(() =>
						{
							openForm.Close();
						}));
					}
				}
				WinCommons._OpenProcessing();
				btn_Update.Enabled = false;
				btn_Cancel.Enabled = false;
				string text = await SaveUdf();
				if (string.IsNullOrEmpty(text))
				{
					FormMode = Enums.FormViewModeType.View;
					await GetFocusNode();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				btn_Update.Enabled = true;
				btn_Cancel.Enabled = true;
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gv_Line.GetFocusedRowCellValue("FldValue"))))
			{
				return;
			}
			string tableId = Function.ToString(gv_Line.GetFocusedRowCellValue("TableID"));
			int fieldId = Function.ParseInt(gv_Line.GetFocusedRowCellValue("FieldID"));
			int indexId = Function.ParseInt(gv_Line.GetFocusedRowCellValue("IndexID"));
			Tuple<string, int, int> tup = new Tuple<string, int, int>(tableId, fieldId, indexId);
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDF");
			if (!string.IsNullOrEmpty(await helper.PostAsync("CheckExistUFD1", tup)))
			{
				gv_Line.BeginUpdate();
				gv_Line.DeleteSelectedRows();
				gv_Line.EndUpdate();
				DataTable dataTable = (DataTable)grd_Line.DataSource;
				dataTable.AcceptChanges();
				grd_Line.DataSource = dataTable;
				grd_Line.RefreshDataSource();
				SetButtonOKTextToUpdate();
				if (_dtValidValues.IsNotNull())
				{
					DataRow dataRow = _dtValidValues.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["TableID"]) == tableId && Function.ParseInt(r["FieldID"]) == fieldId && Function.ParseInt(r["IndexID"]) == indexId);
					if (dataRow != null)
					{
						_dtValidValues.Rows.Remove(dataRow);
						_dtValidValues.AcceptChanges();
					}
				}
				if (_dtUdf1.IsNotNull())
				{
					DataRow dataRow2 = _dtUdf1.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["TableID"]) == tableId && Function.ParseInt(r["FieldID"]) == fieldId && Function.ParseInt(r["IndexID"]) == indexId);
					if (dataRow2 != null)
					{
						_dtUdf1.Rows.Remove(dataRow2);
						_dtUdf1.AcceptChanges();
					}
				}
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmAddUdf, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormCollection openForms = Application.OpenForms;
				for (int num = 0; num < openForms.Count; num++)
				{
					Form openForm = openForms[num];
					if (openForm != this && !openForm.IsMdiContainer)
					{
						Invoke((MethodInvoker)(() =>
						{
							openForm.Close();
						}));
					}
				}
				WinCommons._OpenProcessing();
				string text = await helper.PostAsync("RemoveUFD1", tup);
				if (string.IsNullOrEmpty(text))
				{
					await GetUdf1(tableId, fieldId);
					AddNewGridValue(tableId, fieldId);
					if (_dtValidValues.IsNotNull())
					{
						DataRow dataRow3 = _dtValidValues.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["TableID"]) == tableId && Function.ParseInt(r["FieldID"]) == fieldId && Function.ParseInt(r["IndexID"]) == indexId);
						if (dataRow3 != null)
						{
							_dtValidValues.Rows.Remove(dataRow3);
							_dtValidValues.AcceptChanges();
						}
					}
					if (_dtUdf1.IsNotNull())
					{
						DataRow dataRow4 = _dtUdf1.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["TableID"]) == tableId && Function.ParseInt(r["FieldID"]) == fieldId && Function.ParseInt(r["IndexID"]) == indexId);
						if (dataRow4 != null)
						{
							_dtUdf1.Rows.Remove(dataRow4);
							_dtUdf1.AcceptChanges();
						}
					}
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async void btn_Remove_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			string tblName = Function.ToString(gv_Doc.GetFocusedRowCellValue("TableID"));
			int fieldId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("FieldID"));
			string alias = Function.ToString(gv_Doc.GetFocusedRowCellValue("AliasID"));
			if (string.IsNullOrEmpty(alias))
			{
				return;
			}
			DataTable dataTable;
			using (HttpClientHelper<DataTable> help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDF"))
			{
				dataTable = await help.GetAsync("GetListCUFD", new
				{
					tableName = tblName
				});
			}
			if (dataTable.IsNotNull())
			{
				if ((from t in dataTable.AsEnumerable()
					where Function.ParseInt(t["FieldID"]) == fieldId
					select t).ToList().Count > 0)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmAddUdf, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveUdf, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						return;
					}
					string text = await CheckExistFieldInKeys(tblName, alias);
					if (!string.IsNullOrEmpty(text))
					{
						WinCommons.Error(Enums.MessageCode.None, text);
						return;
					}
					FormCollection openForms = Application.OpenForms;
					for (int num = 0; num < openForms.Count; num++)
					{
						Form openForm = openForms[num];
						if (openForm != this && !openForm.IsMdiContainer)
						{
							Invoke((MethodInvoker)(() =>
							{
								openForm.Close();
							}));
						}
					}
					WinCommons._OpenProcessing();
					Tuple<string, int, string> pr = new Tuple<string, int, string>(tblName, fieldId, alias);
					using (HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDF"))
					{
						string text2 = await helper.PostAsync("RemoveUdf", pr);
						if (string.IsNullOrEmpty(text2))
						{
							gv_Doc.BeginUpdate();
							gv_Doc.DeleteSelectedRows();
							gv_Doc.EndUpdate();
							grd_Doc.DataSourceTable.AcceptChanges();
							grd_Doc.RefreshDataSource();
							WinCommons.Success(Enums.MessageCode.Success, "");
							FormMode = Enums.FormViewModeType.View;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text2);
						}
					}
					WinCommons._CloseProcessing();
				}
				else
				{
					gv_Doc.BeginUpdate();
					gv_Doc.DeleteSelectedRows();
					gv_Doc.EndUpdate();
					DataTable dataTable2 = (DataTable)grd_Doc.DataSource;
					dataTable2.AcceptChanges();
					grd_Doc.DataSource = dataTable2;
					grd_Doc.RefreshDataSource();
				}
			}
			else
			{
				gv_Doc.BeginUpdate();
				gv_Doc.DeleteSelectedRows();
				gv_Doc.EndUpdate();
				DataTable dataTable3 = (DataTable)grd_Doc.DataSource;
				dataTable3.AcceptChanges();
				grd_Doc.DataSource = dataTable3;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Keys.RowCount <= 2)
			{
				WinCommons.Error(Enums.MessageCode.KeyMustHaveAtLeast1Field, "");
				return;
			}
			gv_Keys.BeginUpdate();
			gv_Keys.DeleteSelectedRows();
			gv_Keys.EndUpdate();
			DataTable dataSourceTable = grd_Keys.DataSourceTable;
			dataSourceTable.AcceptChanges();
			grd_Keys.RefreshDataSource();
			List<DataRow> list = (from t in dataSourceTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ColAlias"]))
				select t).ToList();
			if (_dtKeyFields.IsNotNull() && list.Count > 0)
			{
				string tblName = Function.ToString(list[0]["TableName"]);
				int keyId = Function.ParseInt(list[0]["KeyId"]);
				List<DataRow> list2 = (from r in _dtKeyFields.AsEnumerable()
					where Function.ToString(r["TableName"]) == tblName && Function.ParseInt(r["KeyId"]) == keyId
					select r).ToList();
				if (list2.Count > 0)
				{
					foreach (DataRow item in list2)
					{
						_dtKeyFields.Rows.Remove(item);
					}
				}
				_dtKeyFields.AcceptChanges();
			}
			foreach (DataRow item2 in list)
			{
				DataRow dataRow = _dtKeyFields.NewRow();
				dataRow["TableName"] = item2["TableName"];
				dataRow["KeyId"] = item2["KeyId"];
				dataRow["SubKeyId"] = item2["SubKeyId"];
				dataRow["ColAlias"] = item2["ColAlias"];
				_dtKeyFields.Rows.Add(dataRow);
			}
			AddNewKeys();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_UserDefineFieldsManagement_FormClosing(object sender, FormClosingEventArgs e)
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
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveUdf()))
					{
						_forceClose = true;
						Close();
					}
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.UDF.frm_UserDefineFieldsManagement));
		this.tv_UDF = new DevExpress.XtraTreeList.TreeList();
		this.TableCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.Descr = new Apzon.Controls.Tree.TreeListColumnEx();
		this.ctx_TreeList = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Remove = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Keys = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Mandatory = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn32 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Validation = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Index = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LinkSysObj = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LinkToTable = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LinkUdo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn30 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn31 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn34 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTab_Keys = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Valid = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.look_Key = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Delete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_New = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chk_Unique = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_KeyName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Keys = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Key = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Keys = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ColAlias = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.tv_UDF).BeginInit();
		this.ctx_TreeList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Mandatory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Validation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Index).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LinkSysObj).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LinkToTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LinkUdo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Keys).BeginInit();
		this.xtraTab_Keys.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctx_Valid.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Key.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Unique.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_KeyName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Keys).BeginInit();
		this.ctx_Key.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Keys).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ColAlias).BeginInit();
		base.SuspendLayout();
		this.tv_UDF.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[2] { this.TableCode, this.Descr });
		this.tv_UDF.ContextMenuStrip = this.ctx_TreeList;
		this.tv_UDF.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_UDF.Location = new System.Drawing.Point(0, 0);
		this.tv_UDF.Name = "tv_UDF";
		this.tv_UDF.Size = new System.Drawing.Size(203, 478);
		this.tv_UDF.TabIndex = 0;
		this.tv_UDF.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(this.tv_UDF_BeforeFocusNode);
		this.tv_UDF.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_UDF_FocusedNodeChanged);
		this.TableCode.Caption = "TableName";
		this.TableCode.CurrencyDateField = null;
		this.TableCode.CurrencyField = null;
		this.TableCode.CurrencyFixed = null;
		this.TableCode.CurrencyFixedRate = 0.0;
		this.TableCode.CurrencyRateField = null;
		this.TableCode.FieldName = "TableName";
		this.TableCode.Name = "TableCode";
		this.TableCode.OptionsColumn.AllowEdit = false;
		this.Descr.Caption = "Category";
		this.Descr.CurrencyDateField = null;
		this.Descr.CurrencyField = null;
		this.Descr.CurrencyFixed = null;
		this.Descr.CurrencyFixedRate = 0.0;
		this.Descr.CurrencyRateField = null;
		this.Descr.FieldName = "Descr";
		this.Descr.Name = "Descr";
		this.Descr.OptionsColumn.AllowEdit = false;
		this.Descr.Visible = true;
		this.Descr.VisibleIndex = 0;
		this.ctx_TreeList.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.expandAllToolStripMenuItem, this.collapseAllToolStripMenuItem });
		this.ctx_TreeList.Name = "ctx_TreeList";
		this.ctx_TreeList.Size = new System.Drawing.Size(137, 48);
		this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
		this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
		this.expandAllToolStripMenuItem.Text = "Expand All";
		this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
		this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
		this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
		this.collapseAllToolStripMenuItem.Text = "Collapse All";
		this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
		this.btn_Remove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Remove.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Remove.Appearance.Options.UseBackColor = true;
		this.btn_Remove.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Remove.EditControl = null;
		this.btn_Remove.EditValue = "Remove";
		this.btn_Remove.FieldName = null;
		this.btn_Remove.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Remove.ImageOptions.Image");
		this.btn_Remove.IsSearchControl = false;
		this.btn_Remove.Location = new System.Drawing.Point(977, 485);
		this.btn_Remove.Name = "btn_Remove";
		this.btn_Remove.ReadOnly = false;
		this.btn_Remove.SettingInVisible = false;
		this.btn_Remove.SettingReadonly = false;
		this.btn_Remove.Size = new System.Drawing.Size(99, 20);
		this.btn_Remove.TabIndex = 83;
		this.btn_Remove.Text = "Remove";
		this.btn_Remove.Click += new System.EventHandler(this.btn_Remove_Click);
		this.btn_Keys.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Keys.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Keys.Appearance.Options.UseBackColor = true;
		this.btn_Keys.EditControl = null;
		this.btn_Keys.EditValue = "Keys";
		this.btn_Keys.FieldName = null;
		this.btn_Keys.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Keys.ImageOptions.Image");
		this.btn_Keys.IsSearchControl = false;
		this.btn_Keys.Location = new System.Drawing.Point(872, 485);
		this.btn_Keys.Name = "btn_Keys";
		this.btn_Keys.ReadOnly = false;
		this.btn_Keys.SettingInVisible = false;
		this.btn_Keys.SettingReadonly = false;
		this.btn_Keys.Size = new System.Drawing.Size(99, 20);
		this.btn_Keys.TabIndex = 86;
		this.btn_Keys.Text = "Keys";
		this.btn_Keys.Click += new System.EventHandler(this.btn_Keys_Click);
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Update.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Update.Appearance.Options.UseBackColor = true;
		this.btn_Update.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "OK";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Update.ImageOptions.Image");
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(213, 485);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(99, 20);
		this.btn_Update.TabIndex = 85;
		this.btn_Update.Text = "OK";
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(4, 1);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.tv_UDF);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1072, 478);
		this.splitContainerControl1.SplitterPosition = 203;
		this.splitContainerControl1.TabIndex = 87;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.xtraTab_Keys);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(859, 478);
		this.splitContainerControl2.SplitterPosition = 318;
		this.splitContainerControl2.TabIndex = 2;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Line;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_Type, this.rep_Mandatory, this.rep_Validation, this.rep_LinkToTable, this.rep_LinkUdo, this.repositoryItemCalcEdit1, this.rep_LinkSysObj, this.rep_Index });
		this.grd_Doc.Size = new System.Drawing.Size(859, 318);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctx_Line.Name = "ctx_Line";
		this.ctx_Line.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[24]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn32, this.gridColumn11, this.gridColumn29, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn30,
			this.gridColumn31, this.gridColumn18, this.gridColumn33, this.gridColumn34
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUdfManagement";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OFIELD";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsFind.AllowFindPanel = false;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Line_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumn1.Caption = "Entities.Models.Common.Common.Table_ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "TableID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "FieldID";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "FieldID";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "Title";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "AliasID";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.gridColumn3.Width = 102;
		this.gridColumn4.Caption = "Description";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Descr";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.gridColumn4.Width = 158;
		this.gridColumn5.Caption = "Type";
		this.gridColumn5.ColumnEdit = this.rep_Type;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "TypeID1";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.gridColumn5.Width = 79;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Type.DisplayMember = "Name";
		this.rep_Type.Name = "rep_Type";
		this.rep_Type.NullText = "";
		this.rep_Type.ShowFooter = false;
		this.rep_Type.ShowHeader = false;
		this.rep_Type.ValueMember = "Code";
		this.rep_Type.EditValueChanged += new System.EventHandler(this.rep_Type_EditValueChanged);
		this.gridColumn6.Caption = "Structure";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "EditType1";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.gridColumn7.Caption = "Length";
		this.gridColumn7.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n0";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "EditSize";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 4;
		this.gridColumn7.Width = 79;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gridColumn8.Caption = "gridColumn8";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "SizeID";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn9.Caption = "Set Default Value for Field";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Dflt";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 5;
		this.gridColumn9.Width = 149;
		this.gridColumn10.Caption = "Mandatory Field";
		this.gridColumn10.ColumnEdit = this.rep_Mandatory;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "NotNull";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 6;
		this.gridColumn10.Width = 87;
		this.rep_Mandatory.AutoHeight = false;
		this.rep_Mandatory.Name = "rep_Mandatory";
		this.rep_Mandatory.ValueChecked = "Y";
		this.rep_Mandatory.ValueUnchecked = "N";
		this.gridColumn32.Caption = "Validation";
		this.gridColumn32.ColumnEdit = this.rep_Validation;
		this.gridColumn32.CurrencyDateField = null;
		this.gridColumn32.CurrencyField = null;
		this.gridColumn32.CurrencyFixed = null;
		this.gridColumn32.CurrencyFixedRate = 0.0;
		this.gridColumn32.CurrencyRateField = null;
		this.gridColumn32.FieldName = "Validation";
		this.gridColumn32.Name = "gridColumn32";
		this.gridColumn32.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn32.PrimaryKey = false;
		this.gridColumn32.UnitField = null;
		this.gridColumn32.Visible = true;
		this.gridColumn32.VisibleIndex = 7;
		this.gridColumn32.Width = 80;
		this.rep_Validation.AutoHeight = false;
		this.rep_Validation.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Validation.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Validation.DisplayMember = "Name";
		this.rep_Validation.Name = "rep_Validation";
		this.rep_Validation.NullText = "";
		this.rep_Validation.ShowFooter = false;
		this.rep_Validation.ShowHeader = false;
		this.rep_Validation.ValueMember = "Code";
		this.rep_Validation.EditValueChanged += new System.EventHandler(this.rep_Validation_EditValueChanged);
		this.gridColumn11.Caption = "Index";
		this.gridColumn11.ColumnEdit = this.rep_Index;
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "IndexID";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Width = 57;
		this.rep_Index.AutoHeight = false;
		this.rep_Index.Name = "rep_Index";
		this.gridColumn29.Caption = "Link to System Object";
		this.gridColumn29.ColumnEdit = this.rep_LinkSysObj;
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "RelSO";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.gridColumn29.Visible = true;
		this.gridColumn29.VisibleIndex = 8;
		this.gridColumn29.Width = 120;
		this.rep_LinkSysObj.AutoHeight = false;
		this.rep_LinkSysObj.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LinkSysObj.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Type", "Type", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Tbl", "Tbl", 10, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descr", "Descr")
		});
		this.rep_LinkSysObj.DisplayMember = "Descr";
		this.rep_LinkSysObj.Name = "rep_LinkSysObj";
		this.rep_LinkSysObj.NullText = "";
		this.rep_LinkSysObj.ShowFooter = false;
		this.rep_LinkSysObj.ShowHeader = false;
		this.rep_LinkSysObj.ShowLines = false;
		this.rep_LinkSysObj.ValueMember = "Type";
		this.rep_LinkSysObj.EditValueChanged += new System.EventHandler(this.rep_LinkSysObj_EditValueChanged);
		this.gridColumn12.Caption = "Set Linked Table";
		this.gridColumn12.ColumnEdit = this.rep_LinkToTable;
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "RTable";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 9;
		this.gridColumn12.Width = 94;
		this.rep_LinkToTable.AutoHeight = false;
		this.rep_LinkToTable.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LinkToTable.DisplayMember = "Name";
		this.rep_LinkToTable.Name = "rep_LinkToTable";
		this.rep_LinkToTable.NullText = "";
		this.rep_LinkToTable.ShowFooter = false;
		this.rep_LinkToTable.ShowHeader = false;
		this.rep_LinkToTable.ShowLines = false;
		this.rep_LinkToTable.ValueMember = "Code";
		this.rep_LinkToTable.EditValueChanged += new System.EventHandler(this.rep_LinkToTable_EditValueChanged);
		this.gridColumn13.Caption = "gridColumn13";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "RField";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "gridColumn14";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Action";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "gridColumn15";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "Sys";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "gridColumn16";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "DfltDate";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn17.Caption = "Link to UDO";
		this.gridColumn17.ColumnEdit = this.rep_LinkUdo;
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "RelUDO";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 10;
		this.gridColumn17.Width = 90;
		this.rep_LinkUdo.AutoHeight = false;
		this.rep_LinkUdo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LinkUdo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TableName", "TableName", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_LinkUdo.DisplayMember = "Name";
		this.rep_LinkUdo.Name = "rep_LinkUdo";
		this.rep_LinkUdo.NullText = "";
		this.rep_LinkUdo.ShowFooter = false;
		this.rep_LinkUdo.ShowHeader = false;
		this.rep_LinkUdo.ShowLines = false;
		this.rep_LinkUdo.ValueMember = "Code";
		this.rep_LinkUdo.EditValueChanged += new System.EventHandler(this.rep_LinkUdo_EditValueChanged);
		this.gridColumn30.Caption = "RThrdPTab";
		this.gridColumn30.CurrencyDateField = null;
		this.gridColumn30.CurrencyField = null;
		this.gridColumn30.CurrencyFixed = null;
		this.gridColumn30.CurrencyFixedRate = 0.0;
		this.gridColumn30.CurrencyRateField = null;
		this.gridColumn30.FieldName = "RThrdPTab";
		this.gridColumn30.Name = "gridColumn30";
		this.gridColumn30.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn30.PrimaryKey = false;
		this.gridColumn30.UnitField = null;
		this.gridColumn31.Caption = "RThrdPFld";
		this.gridColumn31.CurrencyDateField = null;
		this.gridColumn31.CurrencyField = null;
		this.gridColumn31.CurrencyFixed = null;
		this.gridColumn31.CurrencyFixedRate = 0.0;
		this.gridColumn31.CurrencyRateField = null;
		this.gridColumn31.FieldName = "RThrdPFld";
		this.gridColumn31.Name = "gridColumn31";
		this.gridColumn31.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn31.PrimaryKey = false;
		this.gridColumn31.UnitField = null;
		this.gridColumn18.Caption = "Value";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "ValidRule";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn33.Caption = "TypeID";
		this.gridColumn33.CurrencyDateField = null;
		this.gridColumn33.CurrencyField = null;
		this.gridColumn33.CurrencyFixed = null;
		this.gridColumn33.CurrencyFixedRate = 0.0;
		this.gridColumn33.CurrencyRateField = null;
		this.gridColumn33.FieldName = "TypeID";
		this.gridColumn33.Name = "gridColumn33";
		this.gridColumn33.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn33.PrimaryKey = false;
		this.gridColumn33.UnitField = null;
		this.gridColumn34.Caption = "Structure";
		this.gridColumn34.CurrencyDateField = null;
		this.gridColumn34.CurrencyField = null;
		this.gridColumn34.CurrencyFixed = null;
		this.gridColumn34.CurrencyFixedRate = 0.0;
		this.gridColumn34.CurrencyRateField = null;
		this.gridColumn34.FieldName = "EditType";
		this.gridColumn34.Name = "gridColumn34";
		this.gridColumn34.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn34.PrimaryKey = false;
		this.gridColumn34.UnitField = null;
		this.xtraTab_Keys.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTab_Keys.IsSearchControl = false;
		this.xtraTab_Keys.Location = new System.Drawing.Point(0, 0);
		this.xtraTab_Keys.Name = "xtraTab_Keys";
		this.xtraTab_Keys.SelectedTabPage = this.xtraTabPage1;
		this.xtraTab_Keys.Size = new System.Drawing.Size(859, 150);
		this.xtraTab_Keys.TabIndex = 0;
		this.xtraTab_Keys.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.grd_Line);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Valid Values";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(857, 125);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Valid Values";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctx_Valid;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(857, 125);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctx_Valid.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.ctx_Valid.Name = "ctx_Valid";
		this.ctx_Valid.Size = new System.Drawing.Size(118, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.deleteToolStripMenuItem.Text = "Remove";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn24, this.gridColumn23, this.gridColumn22, this.gridColumn21, this.gridColumn19, this.gridColumn20 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oUdfManagement";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "OVALIDVALUE";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Valid_CustomRowCellEdit);
		this.gv_Line.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Valid_CustomRowCellEditForEditing);
		this.gv_Line.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Valid_ShowingEditor);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Valid_CellValueChanged);
		this.gv_Line.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Line_ValidatingEditor);
		this.gridColumn24.Caption = "TableID";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "TableID";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn23.Caption = "FieldID";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "FieldID";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn22.Caption = "IndexID";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "IndexID";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn21.Caption = "FldDate";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "FldDate";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn19.Caption = "Value";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn19.FieldName = "FldValue";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = true;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 0;
		this.gridColumn19.Width = 305;
		this.gridColumn20.Caption = "Description";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "Descr";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 1;
		this.gridColumn20.Width = 757;
		this.xtraTabPage2.Controls.Add(this.look_Key);
		this.xtraTabPage2.Controls.Add(this.labelLine2);
		this.xtraTabPage2.Controls.Add(this.btn_Delete);
		this.xtraTabPage2.Controls.Add(this.btn_New);
		this.xtraTabPage2.Controls.Add(this.chk_Unique);
		this.xtraTabPage2.Controls.Add(this.txt_KeyName);
		this.xtraTabPage2.Controls.Add(this.labelLine1);
		this.xtraTabPage2.Controls.Add(this.grd_Keys);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Keys";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(857, 125);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Keys";
		this.look_Key.Location = new System.Drawing.Point(87, 26);
		this.look_Key.Name = "look_Key";
		this.look_Key.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Key.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[5]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TableName", "TableName", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("KeyId", "KeyId", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("KeyName", "KeyName"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("UniqueKey", "UniqueKey", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Action", "Action", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Key.Properties.DisplayMember = "KeyName";
		this.look_Key.Properties.NullText = "";
		this.look_Key.Properties.ShowFooter = false;
		this.look_Key.Properties.ShowHeader = false;
		this.look_Key.Properties.ShowLines = false;
		this.look_Key.Properties.ValueMember = "KeyId";
		this.look_Key.Size = new System.Drawing.Size(147, 20);
		this.look_Key.TabIndex = 6;
		this.look_Key.EditValueChanged += new System.EventHandler(this.look_Key_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Selected Key";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(16, 29);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "Selected Key";
		this.btn_Delete.EditControl = null;
		this.btn_Delete.EditValue = "Delete";
		this.btn_Delete.FieldName = null;
		this.btn_Delete.IsSearchControl = false;
		this.btn_Delete.Location = new System.Drawing.Point(253, 58);
		this.btn_Delete.Name = "btn_Delete";
		this.btn_Delete.ReadOnly = false;
		this.btn_Delete.SettingInVisible = false;
		this.btn_Delete.SettingReadonly = false;
		this.btn_Delete.Size = new System.Drawing.Size(75, 23);
		this.btn_Delete.TabIndex = 4;
		this.btn_Delete.Text = "Delete";
		this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
		this.btn_New.EditControl = null;
		this.btn_New.EditValue = "New";
		this.btn_New.FieldName = null;
		this.btn_New.IsSearchControl = false;
		this.btn_New.Location = new System.Drawing.Point(253, 29);
		this.btn_New.Name = "btn_New";
		this.btn_New.ReadOnly = false;
		this.btn_New.SettingInVisible = false;
		this.btn_New.SettingReadonly = false;
		this.btn_New.Size = new System.Drawing.Size(75, 20);
		this.btn_New.TabIndex = 4;
		this.btn_New.Text = "New";
		this.btn_New.Click += new System.EventHandler(this.btn_New_Click);
		this.chk_Unique.EditControl = null;
		this.chk_Unique.FieldName = null;
		this.chk_Unique.IsSearchControl = false;
		this.chk_Unique.Location = new System.Drawing.Point(87, 84);
		this.chk_Unique.Name = "chk_Unique";
		this.chk_Unique.Properties.Caption = "  Unique";
		this.chk_Unique.Properties.ValueChecked = "Y";
		this.chk_Unique.Properties.ValueUnchecked = "N";
		this.chk_Unique.SettingInVisible = false;
		this.chk_Unique.SettingReadonly = false;
		this.chk_Unique.Size = new System.Drawing.Size(75, 20);
		this.chk_Unique.TabIndex = 3;
		this.chk_Unique.CheckedChanged += new System.EventHandler(this.chk_Unique_CheckedChanged);
		this.txt_KeyName.EditControl = null;
		this.txt_KeyName.FieldName = null;
		this.txt_KeyName.IsSearchControl = false;
		this.txt_KeyName.Location = new System.Drawing.Point(87, 59);
		this.txt_KeyName.Name = "txt_KeyName";
		this.txt_KeyName.SettingInVisible = false;
		this.txt_KeyName.SettingReadonly = false;
		this.txt_KeyName.Size = new System.Drawing.Size(147, 20);
		this.txt_KeyName.TabIndex = 2;
		this.txt_KeyName.EditValueChanged += new System.EventHandler(this.txt_KeyName_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Key Name";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(16, 62);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Key Name";
		this.grd_Keys.AllowPasteExcel = false;
		this.grd_Keys.ClipboardData = null;
		this.grd_Keys.ContextMenuStrip = this.ctx_Key;
		this.grd_Keys.Dock = System.Windows.Forms.DockStyle.Right;
		this.grd_Keys.Location = new System.Drawing.Point(349, 0);
		this.grd_Keys.MainView = this.gv_Keys;
		this.grd_Keys.Name = "grd_Keys";
		this.grd_Keys.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_ColAlias });
		this.grd_Keys.Size = new System.Drawing.Size(508, 125);
		this.grd_Keys.TabIndex = 0;
		this.grd_Keys.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Keys });
		this.ctx_Key.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.ctx_Key.Name = "ctx_Key";
		this.ctx_Key.Size = new System.Drawing.Size(118, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.deleteRowToolStripMenuItem.Text = "Remove";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Keys.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Keys.ColumnKey = null;
		this.gv_Keys.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28 });
		this.gv_Keys.DisableFormSetting = false;
		this.gv_Keys.DisplayException = false;
		this.gv_Keys.EditColumn = null;
		this.gv_Keys.FieldCurrency = null;
		this.gv_Keys.FieldRate = null;
		this.gv_Keys.FormId = "oUdfManagement";
		this.gv_Keys.GridControl = this.grd_Keys;
		this.gv_Keys.IgnoreFilter = false;
		this.gv_Keys.IndexEditRow = -1;
		this.gv_Keys.IndicatorWidth = 35;
		this.gv_Keys.IsApzTable = false;
		this.gv_Keys.IsInsertNewRow = false;
		this.gv_Keys.IsLoadingData = false;
		this.gv_Keys.IsMappingContextAction = false;
		this.gv_Keys.ItemId = "OKEY";
		this.gv_Keys.LstColumnAllowBatch = null;
		this.gv_Keys.LstColumnNotAllowBatch = null;
		this.gv_Keys.LstColumnNotEditable = null;
		this.gv_Keys.LstColumNotSetting = null;
		this.gv_Keys.Name = "gv_Keys";
		this.gv_Keys.OptionsView.ShowGroupPanel = false;
		this.gv_Keys.UseBatchMod = false;
		this.gv_Keys.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Keys_CellValueChanged);
		this.gridColumn25.Caption = "TableName";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "TableName";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn25.Width = 195;
		this.gridColumn26.Caption = "KeyId";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "KeyId";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn26.Width = 111;
		this.gridColumn27.Caption = "SubKeyId";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "SubKeyId";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn27.Width = 195;
		this.gridColumn28.Caption = "Field Name";
		this.gridColumn28.ColumnEdit = this.rep_ColAlias;
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "ColAlias";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn28.Visible = true;
		this.gridColumn28.VisibleIndex = 0;
		this.gridColumn28.Width = 386;
		this.rep_ColAlias.AutoHeight = false;
		this.rep_ColAlias.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ColAlias.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("AliasID", "AliasID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Descr", "Descr", 30, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_ColAlias.DisplayMember = "AliasID";
		this.rep_ColAlias.Name = "rep_ColAlias";
		this.rep_ColAlias.NullText = "";
		this.rep_ColAlias.ShowFooter = false;
		this.rep_ColAlias.ShowHeader = false;
		this.rep_ColAlias.ShowLines = false;
		this.rep_ColAlias.ValueMember = "AliasID";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(318, 485);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(99, 20);
		this.btn_Cancel.TabIndex = 88;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Update;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1079, 512);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Keys);
		base.Controls.Add(this.btn_Update);
		base.Controls.Add(this.btn_Remove);
		base.FormId = "oUdfManagement";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_UserDefineFieldsManagement.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UserDefineFieldsManagement";
		this.Text = "User-Defined Fields-Management";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UserDefineFieldsManagement_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.tv_UDF).EndInit();
		this.ctx_TreeList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Mandatory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Validation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Index).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LinkSysObj).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LinkToTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LinkUdo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTab_Keys).EndInit();
		this.xtraTab_Keys.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctx_Valid.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Key.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Unique.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_KeyName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Keys).EndInit();
		this.ctx_Key.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Keys).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ColAlias).EndInit();
		base.ResumeLayout(false);
	}
}
