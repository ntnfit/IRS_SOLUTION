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
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
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
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.StyleFormatConditions;

namespace Apzon.Forms.Administrator;

public class frm_ExternalRouting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isChanging;

	private bool _loadingData;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_SourceData;

	private GridViewEx gv_SourceData;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPageEx2;

	private XtraTabPageEx xtraTabPageEx1;

	private LabelControl labelControl1;

	private MemoExEdit memo_TextQuery;

	private LookUpEditEx luk_DataFrom;

	private ButtonEditCFL cfl_DefinedQuery;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemButtonEditCFL rep_Controller;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gc_Uri;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemCheckEdit rep_Active;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEdit rep_DataType;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx8;

	private RepositoryItemLookUpEdit rep_ResultType;

	private TreeEx tree_ResultConfig;

	private TreeListColumnEx treeListColumnEx1;

	private TreeListColumnEx treeCol_Father;

	private TreeListColumnEx treeListColumnEx3;

	private TreeListColumnEx treeListColumnEx4;

	private TreeListColumnEx treeListColumnEx5;

	private TreeListColumnEx treeListColumnEx6;

	private TreeListColumnEx treeListColumnEx7;

	private TreeListColumnEx treeListColumnEx8;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private RepositoryItemLookUpEdit rep_JsonType;

	private RepositoryItemLookUpEdit rep_ConfigDataType;

	private RepositoryItemLookUpEdit rep_SourceValue;

	private ContextMenuStrip ctx_ResultTreeConfig;

	private ToolStripMenuItem toolStripMenuItemIndent;

	private ToolStripMenuItem toolStripMenuItemOutdent;

	private ToolStripSeparator toolStripSeparator7;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem insertRowToolStripMenuItem;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator2;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit rep_btnAddChild;

	private ToolStripMenuItem deleteDocRowToolStripMenuItem1;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemLookUpEdit rep_ActionType;

	private GridColumnEx gridColumnEx13;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_Save.Text = Resources.ButtonAdd;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_Save.Text = Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_Save.Text = Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_Save;
				bool enabled = (btnCancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ExternalRouting()
	{
		InitializeComponent();
		FormId = "oExternalRouting";
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OURI";
		gv_SourceData.FormId = FormId;
		gv_SourceData.ItemId = "APZ_URI1";
		tree_ResultConfig.FormId = FormId;
		tree_ResultConfig.ItemId = "APZ_URI2";
		OnLoadAsync += OnOnLoadAsync;
		gv_Doc.CellValueChanged += GvDocOnCellValueChanged;
		gv_Doc.CellValueChanged += Control_EditValueChanged;
		gv_SourceData.CellValueChanged += GvSourceDataOnCellValueChanged;
		gv_SourceData.CellValueChanged += Control_EditValueChanged;
		tree_ResultConfig.CellValueChanged += Control_EditValueChanged;
	}

	private void GvSourceDataOnCellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (_loadingData || !(e.Column.FieldName == "Alias"))
			{
				return;
			}
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			string text = "";
			if (grd_SourceData.DataSource is DataTable dataTable)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					text = text + Function.ToString(row["Alias"]).Replace("[", "").Replace("]", "") + "=&";
					dictionary.Add(Function.ToString(row["Alias"]).Replace("[", "").Replace("]", ""), null);
				}
			}
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("UriGenerated"));
			string text3 = Function.ToString(gv_Doc.GetFocusedRowCellValue("ActionType"));
			int num = text2.IndexOf("?", StringComparison.Ordinal);
			string text4 = text2.Substring(0, num + 1);
			if (text3 == "1")
			{
				gv_Doc.SetFocusedRowCellValue("UriGenerated", text4 + text.Substring(0, text.Length - 1));
				return;
			}
			string value = MyDictionaryToJson(dictionary);
			gv_Doc.SetFocusedRowCellValue("BodyGenerated", value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GvDocOnCellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (_isChanging || _loadingData)
			{
				return;
			}
			object rowCellValue = gv_Doc.GetRowCellValue(e.RowHandle, "Controller");
			object rowCellValue2 = gv_Doc.GetRowCellValue(e.RowHandle, "ActionName");
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("UriGenerated"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("ActionType"));
			int num = text.IndexOf("?", StringComparison.Ordinal);
			string text3 = "";
			if (!string.IsNullOrWhiteSpace(text))
			{
				text3 = text.Substring(num + 1, text.Length - (num + 1));
			}
			if (e.Column.FieldName == "Controller" || e.Column.FieldName == "ActionName")
			{
				_isChanging = true;
				if (text2 == "1")
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "UriGenerated", $"q/0/{rowCellValue}/{rowCellValue2}/?" + text3);
				}
				else
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "UriGenerated", $"q/0/{rowCellValue}/{rowCellValue2}/");
				}
				_isChanging = false;
			}
			if (e.Column.FieldName == "ResultType")
			{
				if (Function.ToString(e.Value) == "1")
				{
					xtraTabPageEx2.PageEnabled = true;
				}
				else
				{
					xtraTabPageEx2.PageEnabled = false;
				}
			}
			if (!(e.Column.FieldName == "ActionType"))
			{
				return;
			}
			_isChanging = true;
			if (Function.ToString(e.Value) == "1")
			{
				gv_Doc.SetRowCellValue(e.RowHandle, "UriGenerated", $"q/0/{rowCellValue}/{rowCellValue2}/?" + text3);
				gv_Doc.SetFocusedRowCellValue("BodyGenerated", "");
			}
			else
			{
				gv_Doc.SetRowCellValue(e.RowHandle, "UriGenerated", $"q/0/{rowCellValue}/{rowCellValue2}/");
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (grd_SourceData.DataSource is DataTable dataTable)
				{
					foreach (DataRow row in dataTable.Rows)
					{
						dictionary.Add(Function.ToString(row["Alias"]).Replace("[", "").Replace("]", ""), null);
					}
				}
				string value = MyDictionaryToJson(dictionary);
				gv_Doc.SetFocusedRowCellValue("BodyGenerated", value);
			}
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_isChanging = false;
		}
	}

	private async Task OnOnLoadAsync(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "0",
					Name = "Table"
				},
				new Common.Table_Code
				{
					Code = "1",
					Name = "Object"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "DataSet"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "Text"
				}
			};
			SetDataSourceByList(rep_ResultType, list);
			List<Common.Table_Code> list2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1",
					Name = "GET"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "POST"
				}
			};
			SetDataSourceByList(rep_ActionType, list2);
			List<Common.Table_Code> list3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "T",
					Name = "Query Text"
				},
				new Common.Table_Code
				{
					Code = "Q",
					Name = "Defined Query"
				}
			};
			luk_DataFrom.Properties.DataSource = list3;
			luk_DataFrom.Properties.DisplayMember = "Name";
			luk_DataFrom.Properties.ValueMember = "Code";
			luk_DataFrom.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_DataFrom.Properties.ForceInitialize();
			luk_DataFrom.Properties.PopulateColumns();
			luk_DataFrom.Properties.Columns["Code"].Visible = false;
			luk_DataFrom.Properties.DropDownRows = list3.Count;
			List<Common.Table_Code> list4 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "O",
					Name = "Object"
				},
				new Common.Table_Code
				{
					Code = "A",
					Name = "Array"
				},
				new Common.Table_Code
				{
					Code = "F",
					Name = "Field"
				}
			};
			SetDataSourceByList(rep_JsonType, list4);
			cfl_DefinedQuery.Properties.DisplayMember = "QName";
			cfl_DefinedQuery.Properties.ValueMember = "IntrnalKey";
			cfl_DefinedQuery.Properties.TypeCFL = Enums.TypeCFL.Queries;
			cfl_DefinedQuery.Properties.RefreshDataSource();
			RepositoryItemButtonEditCFL properties = cfl_DefinedQuery.Properties;
			properties.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(properties.ItemSelected, new Action<DataRow, GridView>(cfl_DefinedQuery_ItemSelected));
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (Enums.DisplayFormatType value2 in Enum.GetValues(typeof(Enums.DisplayFormatType)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = (int)value2;
				string value = Function.ToString(value2);
				if (value2 == Enums.DisplayFormatType.None)
				{
					value = "String";
				}
				dataRow["Name"] = Function.SpaceBeforeCapital(value);
				dataTable.Rows.Add(dataRow);
			}
			rep_ConfigDataType.DataSource = dataTable;
			rep_ConfigDataType.DisplayMember = "Name";
			rep_ConfigDataType.ValueMember = "Code";
			rep_ConfigDataType.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_ConfigDataType.ForceInitialize();
			rep_ConfigDataType.PopulateColumns();
			rep_ConfigDataType.Columns["Code"].Visible = false;
			rep_ConfigDataType.DropDownRows = dataTable.Rows.Count;
			rep_DataType.Assign(rep_ConfigDataType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void cfl_DefinedQuery_ItemSelected(DataRow row, GridView gv)
	{
		try
		{
			if (!_loadingData)
			{
				string qtxt = Function.ToString(row["QString"]);
				await LoadQueryParameters(qtxt);
				DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
				Control_EditValueChanged(null, null);
				focusedDataRow["DataString"] = Function.ToString(cfl_DefinedQuery.EditValue);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void memo_TextQuery_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_loadingData)
			{
				DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
				string text = Function.ToString(memo_TextQuery.EditValue);
				if (gv_Doc.FocusedRowHandle >= 0 && Function.ToString(luk_DataFrom.EditValue) == "T")
				{
					focusedDataRow["DataString"] = text;
				}
				if (string.IsNullOrEmpty(text))
				{
					grd_SourceData.DataSourceTable.Rows.Clear();
					grd_SourceData.DataSourceTable.AcceptChanges();
				}
				await LoadQueryParameters(text);
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			_loadingData = true;
			DataTable dataTable = await _httpClient.ExternalRoutingClient.GetAllRoutings();
			grd_Doc.DataSource = dataTable;
			dataTable.AcceptChanges();
			grd_Doc.RefreshDataSource();
			if (dataTable.Rows.Count == 0)
			{
				await REFRESH();
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
			_loadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_loadingData = false;
		}
	}

	private void SetDataSourceByList(RepositoryItemLookUpEdit luk, List<Common.Table_Code> list)
	{
		try
		{
			luk.DataSource = list;
			luk.DisplayMember = "Name";
			luk.ValueMember = "Code";
			luk.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.ForceInitialize();
			luk.PopulateColumns();
			luk.Columns["Code"].Visible = false;
			luk.DropDownRows = list.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OURI");
			WinFuntions.GetTableStructure("APZ_URI1");
			WinFuntions.GetTableStructure("APZ_URI2");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["Active"] = "Y";
			dataRow["SourceData"] = "T";
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
			luk_DataFrom.EditValue = "T";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_DataFrom_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_loadingData)
			{
				string text = Function.ToString(luk_DataFrom.EditValue);
				string value = "";
				DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
				if (text == "T")
				{
					cfl_DefinedQuery.Visible = false;
					memo_TextQuery.Visible = true;
					memo_TextQuery.Location = cfl_DefinedQuery.Location;
					value = Function.ToString(memo_TextQuery.EditValue);
				}
				else if (text == "Q")
				{
					cfl_DefinedQuery.Visible = true;
					memo_TextQuery.Visible = false;
					value = Function.ToString(cfl_DefinedQuery.EditValue);
				}
				else
				{
					cfl_DefinedQuery.Visible = false;
					memo_TextQuery.Visible = false;
				}
				focusedDataRow["DataString"] = value;
				focusedDataRow["SourceData"] = text;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_loadingData = true;
			int absEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("SourceData"));
			string editValue = Function.ToString(gv_Doc.GetFocusedRowCellValue("DataString"));
			string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("ResultType"));
			luk_DataFrom.EditValue = text;
			if (text == "T")
			{
				memo_TextQuery.EditValue = editValue;
				memo_TextQuery.Location = cfl_DefinedQuery.Location;
				cfl_DefinedQuery.Visible = false;
				memo_TextQuery.Visible = true;
			}
			else if (text == "Q")
			{
				cfl_DefinedQuery.EditValue = editValue;
				cfl_DefinedQuery.Visible = true;
				memo_TextQuery.Visible = false;
			}
			else
			{
				memo_TextQuery.EditValue = null;
				cfl_DefinedQuery.EditValue = null;
				cfl_DefinedQuery.Visible = false;
				memo_TextQuery.Visible = false;
			}
			xtraTabPageEx2.PageEnabled = text2 == "1";
			DataSet dataSet = await Task.Run(() => _httpClient.ExternalRoutingClient.GetLines(absEntry));
			if (dataSet.IsNotNull() && dataSet.Tables.Count == 2)
			{
				grd_SourceData.DataSource = dataSet.Tables[0];
				grd_SourceData.RefreshDataSource();
				DataTable dataTable = dataSet.Tables[1];
				dataTable.Columns["ConfigID"].AutoIncrement = true;
				dataTable.Columns["ConfigID"].AutoIncrementSeed = (dataTable.IsNotNull() ? (dataTable.AsEnumerable().Max((DataRow t) => Function.ParseLong(t["ConfigID"])) + 1) : 1);
				dataTable.Columns["ConfigID"].AutoIncrementStep = 1L;
				tree_ResultConfig.DataSource = dataTable;
				tree_ResultConfig.RefreshDataSource();
				tree_ResultConfig.ExpandAll();
				NewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_loadingData = false;
		}
	}

	private void toolStripMenuItemIndent_Click(object sender, EventArgs e)
	{
	}

	private void toolStripMenuItemOutdent_Click(object sender, EventArgs e)
	{
		try
		{
			TreeListNode parentNode = tree_ResultConfig.FocusedNode.ParentNode;
			if (parentNode != null)
			{
				tree_ResultConfig.FocusedNode.SetValue("FatherNum", parentNode.GetValue("FatherNum"));
				tree_ResultConfig.FocusedNode.SetValue("Wbs", parentNode.GetValue("Wbs"));
				tree_ResultConfig.DataSourceTable.AcceptChanges();
				tree_ResultConfig.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			NewLine();
			Control_EditValueChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void insertRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = tree_ResultConfig.DataSourceTable;
			if (dataSourceTable.Rows.Count > 0)
			{
				int num = Function.ParseInt(tree_ResultConfig.FocusedNode.GetValue(treeCol_Father));
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["FatherNum"] = num;
				dataRow["AbsEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				dataSourceTable.Rows.InsertAt(dataRow, tree_ResultConfig.FocusedNode.Id);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = tree_ResultConfig.DataSourceTable;
			DataRow focusedDataRow = tree_ResultConfig.GetFocusedDataRow();
			if (dataSourceTable.Rows.Count > 0 && !string.IsNullOrWhiteSpace(Function.ToString(focusedDataRow["Name"])))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				int num = Function.ParseInt(dataRow["ConfigID"]);
				dataRow.ItemArray = focusedDataRow.ItemArray;
				dataRow["ConfigID"] = num;
				if (tree_ResultConfig.FocusedNode.Id + 1 >= dataSourceTable.Rows.Count)
				{
					dataSourceTable.Rows.Add(dataRow);
				}
				else
				{
					dataSourceTable.Rows.InsertAt(dataRow, tree_ResultConfig.FocusedNode.Id + 1);
				}
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (tree_ResultConfig.FocusedNode != null)
			{
				tree_ResultConfig.DeleteSelectedNodes();
				tree_ResultConfig.DataSourceTable.AcceptChanges();
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_btnAddChild_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tree_ResultConfig.FocusedNode.GetValue("ConfigID"));
			NewLine(fatherId);
			tree_ResultConfig.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewLine(int fatherId = -1)
	{
		try
		{
			if (tree_ResultConfig.DataSource is DataTable dataTable && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"])) && Function.ParseInt(t["FatherNum"]) == fatherId) == null)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["AbsEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				dataRow["FatherNum"] = fatherId;
				dataTable.Rows.Add(dataRow);
				tree_ResultConfig.DataSource = dataTable;
				tree_ResultConfig.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
				{
					btn_Save.Enabled = false;
					btnCancel.Enabled = false;
					await SaveData();
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
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		string rs = "Error";
		try
		{
			if ((grd_Doc.DataSource as DataTable).IsNotNull())
			{
				DataSet dataSet = new DataSet();
				if (gv_Doc.GetFocusedDataRow() == null)
				{
					return rs;
				}
				int currentEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
				DataTable dataTable = gv_Doc.GetFocusedDataRow().Table.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow.ItemArray = gv_Doc.GetFocusedDataRow().ItemArray;
				dataTable.Rows.Add(dataRow);
				dataTable.TableName = "dtDoc";
				dataSet.Tables.Add(dataTable);
				DataTable dataTable2 = grd_SourceData.DataSourceTable.Copy();
				dataTable2.TableName = "dtParam";
				dataSet.Tables.Add(dataTable2);
				DataTable dataTable3 = tree_ResultConfig.DataSourceTable.Copy();
				dataTable3.TableName = "dtRsConfig";
				dataSet.Tables.Add(dataTable3);
				WinCommons._OpenProcessing();
				rs = await _httpClient.ExternalRoutingClient.SaveDoc(dataSet);
				if (string.IsNullOrEmpty(rs))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					await REFRESH_DATA();
					int num = gv_Doc.LocateByValue("AbsEntry", currentEntry);
					if (num != int.MinValue)
					{
						gv_Doc.FocusedRowHandle = num;
						gv_Doc.MakeRowVisible(num);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, rs);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return rs;
	}

	private async void deleteDocRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			if (num > 0)
			{
				string text = await _httpClient.ExternalRoutingClient.DeleteDoc(num);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tree_ResultConfig_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column != null)
			{
				if (e.Column.FieldName == "Name")
				{
					NewLine();
				}
				Control_EditValueChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_loadingData || FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomColumnDisplayText(object sender, DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "UriGenerated")
			{
				e.DisplayText = Configuration.ApiServer + e.Value;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadQueryParameters(string qtxt)
	{
		if (string.IsNullOrEmpty(qtxt))
		{
			return;
		}
		DataTable dataTable = await _httpClient.Cfl.GetQueryParams(qtxt);
		new List<QueryParamater>();
		DataTable dataTable2 = grd_SourceData.DataSourceTable();
		dataTable2.IsNotNull();
		dataTable2.Rows.Clear();
		dataTable2.AcceptChanges();
		string text = "";
		foreach (DataRow row in dataTable.Rows)
		{
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2["AbsEntry"] = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			dataRow2["ParamName"] = Function.ToString(row["Para"]);
			dataRow2["Alias"] = Function.ToString(row["ColumnField"]).Replace("[", "").Replace("]", "");
			dataRow2["Dscription"] = Function.ToString(row["ColumnName"]);
			dataRow2["DfltValue"] = WinFuntions.GetDefaultSqlValue(Function.ToString(row["DataType"]));
			if (Function.ToString(row["DataType"]).ToLower() == "datetime")
			{
				dataRow2["DataType"] = 7;
			}
			else
			{
				dataRow2["DataType"] = 0;
			}
			dataTable2.Rows.Add(dataRow2);
			text = text + Function.ToString(row["ColumnField"]).Replace("[", "").Replace("]", "") + "=&";
		}
		dataTable2.AcceptChanges();
		grd_SourceData.DataSource = dataTable2;
		grd_SourceData.RefreshDataSource();
		string text2 = Function.ToString(gv_Doc.GetFocusedRowCellValue("UriGenerated"));
		string text3 = Function.ToString(gv_Doc.GetFocusedRowCellValue("ActionType"));
		int num = text2.IndexOf("?", StringComparison.Ordinal);
		string text4 = text2.Substring(0, num + 1);
		string value = (string.IsNullOrWhiteSpace(text) ? text4 : (text4 + text.Substring(0, text.Length - 1)));
		if (text3 == "1")
		{
			gv_Doc.SetFocusedRowCellValue("UriGenerated", value);
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		for (int i = 0; i < dataTable2.Rows.Count; i++)
		{
			dictionary.Add(Function.ToString(dataTable2.Rows[i]["Alias"]), null);
		}
		string value2 = MyDictionaryToJson(dictionary);
		gv_Doc.SetFocusedRowCellValue("BodyGenerated", value2);
	}

	private void frm_ExternalRouting_FormClosing(object sender, FormClosingEventArgs e)
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtraTabControlEx1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (e.Page == xtraTabPageEx2)
		{
			tree_ResultConfig.RefreshDataSource();
		}
	}

	private string MyDictionaryToJson(Dictionary<string, object> dict)
	{
		IEnumerable<string> values = dict.Select((KeyValuePair<string, object> d) => $"\"{d.Key}\": \"{d.Value}\"");
		return "{" + string.Join(",", values) + "}";
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
		DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule treeListFormatRule = new DevExpress.XtraTreeList.StyleFormatConditions.TreeListFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_ExternalRouting));
		this.rep_JsonType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_ConfigDataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_SourceValue = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteDocRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ActionType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ResultType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Uri = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_Controller = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.ctx_ResultTreeConfig = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItemIndent = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItemOutdent = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.insertRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.cfl_DefinedQuery = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.memo_TextQuery = new DevExpress.XtraEditors.MemoExEdit();
		this.luk_DataFrom = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.grd_SourceData = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SourceData = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DataType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPageEx2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tree_ResultConfig = new Apzon.Controls.Tree.TreeEx();
		this.treeListColumnEx1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeCol_Father = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_btnAddChild = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.treeListColumnEx4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx6 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx8 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.rep_JsonType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfigDataType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceValue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActionType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ResultType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Controller).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.ctx_ResultTreeConfig.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_DefinedQuery.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_TextQuery.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DataFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SourceData).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SourceData).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).BeginInit();
		this.xtraTabPageEx2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tree_ResultConfig).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAddChild).BeginInit();
		base.SuspendLayout();
		this.rep_JsonType.AutoHeight = false;
		this.rep_JsonType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_JsonType.Name = "rep_JsonType";
		this.rep_JsonType.NullText = "";
		this.rep_ConfigDataType.AutoHeight = false;
		this.rep_ConfigDataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ConfigDataType.Name = "rep_ConfigDataType";
		this.rep_ConfigDataType.NullText = "";
		this.rep_SourceValue.AutoHeight = false;
		this.rep_SourceValue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceValue.Name = "rep_SourceValue";
		this.rep_SourceValue.NullText = "";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1046, 467);
		this.splitContainerControl1.SplitterPosition = 151;
		this.splitContainerControl1.TabIndex = 91;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_Controller, this.repositoryItemMemoExEdit1, this.rep_Active, this.rep_ResultType, this.rep_ActionType });
		this.grd_Doc.Size = new System.Drawing.Size(1046, 151);
		this.grd_Doc.TabIndex = 86;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteDocRowToolStripMenuItem1 });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(134, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteDocRowToolStripMenuItem1.Name = "deleteDocRowToolStripMenuItem1";
		this.deleteDocRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteDocRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteDocRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteDocRowToolStripMenuItem1_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx2, this.gridColumnEx4, this.gridColumnEx11, this.gridColumnEx7, this.gridColumnEx9, this.gridColumnEx10, this.gc_Uri, this.gridColumnEx13, this.gridColumnEx12 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUrlForOutgoingData";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gv_Doc.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Doc_CustomColumnDisplayText);
		this.gridColumnEx2.Caption = "Controller";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Controller";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = true;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx2.Width = 128;
		this.gridColumnEx4.Caption = "Action Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ActionName";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 1;
		this.gridColumnEx4.Width = 100;
		this.gridColumnEx11.Caption = "Action Type";
		this.gridColumnEx11.ColumnEdit = this.rep_ActionType;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ActionType";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 2;
		this.rep_ActionType.AutoHeight = false;
		this.rep_ActionType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ActionType.Name = "rep_ActionType";
		this.rep_ActionType.NullText = "";
		this.gridColumnEx7.Caption = "Description";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Dscription";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 3;
		this.gridColumnEx7.Width = 155;
		this.gridColumnEx9.Caption = "Result Type";
		this.gridColumnEx9.ColumnEdit = this.rep_ResultType;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "ResultType";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 4;
		this.gridColumnEx9.Width = 102;
		this.rep_ResultType.AutoHeight = false;
		this.rep_ResultType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ResultType.Name = "rep_ResultType";
		this.rep_ResultType.NullText = "";
		this.gridColumnEx10.Caption = "Result Name";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "ResultName";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 5;
		this.gridColumnEx10.Width = 114;
		this.gc_Uri.Caption = "Uri Generated";
		this.gc_Uri.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gc_Uri.CurrencyDateField = null;
		this.gc_Uri.CurrencyField = null;
		this.gc_Uri.CurrencyFixed = null;
		this.gc_Uri.CurrencyFixedRate = 0.0;
		this.gc_Uri.CurrencyRateField = null;
		this.gc_Uri.FieldName = "UriGenerated";
		this.gc_Uri.Name = "gc_Uri";
		this.gc_Uri.PrimaryKey = false;
		this.gc_Uri.UnitField = null;
		this.gc_Uri.Visible = true;
		this.gc_Uri.VisibleIndex = 6;
		this.gc_Uri.Width = 199;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ReadOnly = true;
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumnEx13.Caption = "Post Body";
		this.gridColumnEx13.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "BodyGenerated";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 7;
		this.gridColumnEx12.Caption = "Active";
		this.gridColumnEx12.ColumnEdit = this.rep_Active;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Active";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 8;
		this.gridColumnEx12.Width = 216;
		this.rep_Active.AutoHeight = false;
		this.rep_Active.Name = "rep_Active";
		this.rep_Active.ValueChecked = "Y";
		this.rep_Active.ValueUnchecked = "N";
		this.rep_Controller.AutoHeight = false;
		this.rep_Controller.AutoLoadDataSource = true;
		this.rep_Controller.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Controller.ContainerControl = this;
		this.rep_Controller.DisplayMember = "";
		this.rep_Controller.DisplayMember2 = "";
		this.rep_Controller.ListMandatoryField = null;
		this.rep_Controller.LoadDataSourceWhenEnter = false;
		this.rep_Controller.Name = "rep_Controller";
		this.rep_Controller.SeparateCharacterForMulti = ';';
		this.rep_Controller.SeparateString = ", ";
		this.rep_Controller.UsingAutoCompleteBox = false;
		this.rep_Controller.UsingDataSource = true;
		this.rep_Controller.ValidateWhenNoDataSource = false;
		this.rep_Controller.ValueMember = "";
		this.xtraTabControlEx1.ContextMenuStrip = this.ctx_ResultTreeConfig;
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPageEx1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(1046, 306);
		this.xtraTabControlEx1.TabIndex = 84;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPageEx1, this.xtraTabPageEx2 });
		this.xtraTabControlEx1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControlEx1_SelectedPageChanged);
		this.ctx_ResultTreeConfig.ImageScalingSize = new System.Drawing.Size(18, 18);
		this.ctx_ResultTreeConfig.Items.AddRange(new System.Windows.Forms.ToolStripItem[8] { this.toolStripMenuItemIndent, this.toolStripMenuItemOutdent, this.toolStripSeparator7, this.addRowToolStripMenuItem, this.insertRowToolStripMenuItem, this.duplicateRowToolStripMenuItem, this.toolStripSeparator2, this.deleteRowToolStripMenuItem });
		this.ctx_ResultTreeConfig.Name = "contextMenuStrip2";
		this.ctx_ResultTreeConfig.Size = new System.Drawing.Size(204, 160);
		this.toolStripMenuItemIndent.Image = (System.Drawing.Image)resources.GetObject("toolStripMenuItemIndent.Image");
		this.toolStripMenuItemIndent.Name = "toolStripMenuItemIndent";
		this.toolStripMenuItemIndent.Size = new System.Drawing.Size(203, 24);
		this.toolStripMenuItemIndent.Text = "Indent(Alt+Shift+Right)";
		this.toolStripMenuItemIndent.Visible = false;
		this.toolStripMenuItemIndent.Click += new System.EventHandler(this.toolStripMenuItemIndent_Click);
		this.toolStripMenuItemOutdent.Image = (System.Drawing.Image)resources.GetObject("toolStripMenuItemOutdent.Image");
		this.toolStripMenuItemOutdent.Name = "toolStripMenuItemOutdent";
		this.toolStripMenuItemOutdent.Size = new System.Drawing.Size(203, 24);
		this.toolStripMenuItemOutdent.Text = "Outdent(Alt+Shift+Left)";
		this.toolStripMenuItemOutdent.Visible = false;
		this.toolStripMenuItemOutdent.Click += new System.EventHandler(this.toolStripMenuItemOutdent_Click);
		this.toolStripSeparator7.Name = "toolStripSeparator7";
		this.toolStripSeparator7.Size = new System.Drawing.Size(200, 6);
		this.toolStripSeparator7.Visible = false;
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(203, 24);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.insertRowToolStripMenuItem.Name = "insertRowToolStripMenuItem";
		this.insertRowToolStripMenuItem.Size = new System.Drawing.Size(203, 24);
		this.insertRowToolStripMenuItem.Text = "Insert Row";
		this.insertRowToolStripMenuItem.Click += new System.EventHandler(this.insertRowToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(203, 24);
		this.duplicateRowToolStripMenuItem.Text = "Duplicate Row";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(203, 24);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.xtraTabPageEx1.Controls.Add(this.cfl_DefinedQuery);
		this.xtraTabPageEx1.Controls.Add(this.memo_TextQuery);
		this.xtraTabPageEx1.Controls.Add(this.luk_DataFrom);
		this.xtraTabPageEx1.Controls.Add(this.labelControl1);
		this.xtraTabPageEx1.Controls.Add(this.grd_SourceData);
		this.xtraTabPageEx1.EditControl = null;
		this.xtraTabPageEx1.EditValue = "Source Data Properties";
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.ReadOnly = false;
		this.xtraTabPageEx1.SettingInVisible = false;
		this.xtraTabPageEx1.SettingReadonly = false;
		this.xtraTabPageEx1.Size = new System.Drawing.Size(1044, 281);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "Source Data Properties";
		this.cfl_DefinedQuery.EditControl = null;
		this.cfl_DefinedQuery.FieldName = "CostCenter";
		this.cfl_DefinedQuery.IsSearchControl = false;
		this.cfl_DefinedQuery.Location = new System.Drawing.Point(177, 5);
		this.cfl_DefinedQuery.Name = "cfl_DefinedQuery";
		this.cfl_DefinedQuery.Properties.AutoLoadDataSource = true;
		this.cfl_DefinedQuery.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_DefinedQuery.Properties.ContainerControl = null;
		this.cfl_DefinedQuery.Properties.DisplayMember = "PrcCode";
		this.cfl_DefinedQuery.Properties.DisplayMember2 = "";
		this.cfl_DefinedQuery.Properties.ListMandatoryField = null;
		this.cfl_DefinedQuery.Properties.LoadDataSourceWhenEnter = false;
		this.cfl_DefinedQuery.Properties.SeparateCharacterForMulti = ';';
		this.cfl_DefinedQuery.Properties.SeparateString = ", ";
		this.cfl_DefinedQuery.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListOfCostCenter;
		this.cfl_DefinedQuery.Properties.UsingAutoCompleteBox = false;
		this.cfl_DefinedQuery.Properties.UsingDataSource = true;
		this.cfl_DefinedQuery.Properties.ValidateWhenNoDataSource = false;
		this.cfl_DefinedQuery.Properties.ValueMember = "PrcCode";
		this.cfl_DefinedQuery.SettingInVisible = false;
		this.cfl_DefinedQuery.SettingReadonly = false;
		this.cfl_DefinedQuery.Size = new System.Drawing.Size(300, 20);
		this.cfl_DefinedQuery.TabIndex = 87;
		this.cfl_DefinedQuery.Visible = false;
		this.memo_TextQuery.Location = new System.Drawing.Point(489, 5);
		this.memo_TextQuery.Name = "memo_TextQuery";
		this.memo_TextQuery.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_TextQuery.Properties.ShowIcon = false;
		this.memo_TextQuery.Size = new System.Drawing.Size(300, 20);
		this.memo_TextQuery.TabIndex = 86;
		this.memo_TextQuery.Visible = false;
		this.memo_TextQuery.EditValueChanged += new System.EventHandler(this.memo_TextQuery_EditValueChanged);
		this.luk_DataFrom.EditControl = null;
		this.luk_DataFrom.FieldName = null;
		this.luk_DataFrom.IsSearchControl = false;
		this.luk_DataFrom.Location = new System.Drawing.Point(71, 5);
		this.luk_DataFrom.Name = "luk_DataFrom";
		this.luk_DataFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DataFrom.Properties.NullText = "";
		this.luk_DataFrom.SettingInVisible = false;
		this.luk_DataFrom.SettingReadonly = false;
		this.luk_DataFrom.Size = new System.Drawing.Size(100, 20);
		this.luk_DataFrom.TabIndex = 85;
		this.luk_DataFrom.EditValueChanged += new System.EventHandler(this.luk_DataFrom_EditValueChanged);
		this.labelControl1.Location = new System.Drawing.Point(2, 8);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(50, 13);
		this.labelControl1.TabIndex = 84;
		this.labelControl1.Text = "Data From";
		this.grd_SourceData.AllowPasteExcel = true;
		this.grd_SourceData.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_SourceData.ClipboardData = null;
		this.grd_SourceData.Location = new System.Drawing.Point(0, 33);
		this.grd_SourceData.MainView = this.gv_SourceData;
		this.grd_SourceData.Name = "grd_SourceData";
		this.grd_SourceData.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_DataType });
		this.grd_SourceData.Size = new System.Drawing.Size(1037, 250);
		this.grd_SourceData.TabIndex = 83;
		this.grd_SourceData.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SourceData });
		this.gv_SourceData.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SourceData.ColumnKey = "EmpId";
		this.gv_SourceData.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx1, this.gridColumnEx3, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx8 });
		this.gv_SourceData.DisableFormSetting = false;
		this.gv_SourceData.DisplayException = false;
		this.gv_SourceData.FieldCurrency = null;
		this.gv_SourceData.FieldRate = null;
		this.gv_SourceData.FormId = "oUrlForOutgoingData";
		this.gv_SourceData.GridControl = this.grd_SourceData;
		this.gv_SourceData.IndicatorWidth = 30;
		this.gv_SourceData.IsApzTable = false;
		this.gv_SourceData.IsInsertNewRow = true;
		this.gv_SourceData.IsLoadingData = false;
		this.gv_SourceData.ItemId = "OSOURCE";
		this.gv_SourceData.LstColumnAllowBatch = null;
		this.gv_SourceData.LstColumnNotAllowBatch = null;
		this.gv_SourceData.LstColumnNotEditable = null;
		this.gv_SourceData.LstColumNotSetting = null;
		this.gv_SourceData.Name = "gv_SourceData";
		this.gv_SourceData.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_SourceData.OptionsView.RowAutoHeight = true;
		this.gv_SourceData.OptionsView.ShowGroupPanel = false;
		this.gv_SourceData.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Parameter Name";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "ParamName";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Alias ";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Alias";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx5.Caption = "Data Type";
		this.gridColumnEx5.ColumnEdit = this.rep_DataType;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "DataType";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.rep_DataType.AutoHeight = false;
		this.rep_DataType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DataType.Name = "rep_DataType";
		this.rep_DataType.NullText = "";
		this.gridColumnEx6.Caption = "Description";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Dscription";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 3;
		this.gridColumnEx8.Caption = "Default Value";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "DfltValue";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 4;
		this.xtraTabPageEx2.Controls.Add(this.tree_ResultConfig);
		this.xtraTabPageEx2.EditControl = null;
		this.xtraTabPageEx2.EditValue = "Action Result Configuration";
		this.xtraTabPageEx2.FieldName = null;
		this.xtraTabPageEx2.IsSearchControl = false;
		this.xtraTabPageEx2.Name = "xtraTabPageEx2";
		this.xtraTabPageEx2.ReadOnly = false;
		this.xtraTabPageEx2.SettingInVisible = false;
		this.xtraTabPageEx2.SettingReadonly = false;
		this.xtraTabPageEx2.Size = new System.Drawing.Size(1044, 281);
		this.xtraTabPageEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx2.Text = "Action Result Configuration";
		this.tree_ResultConfig.AllowDrop = true;
		this.tree_ResultConfig.AllowPasteExcel = true;
		this.tree_ResultConfig.CancelNextCellValueChangedScript = false;
		this.tree_ResultConfig.CancelNextFocusObjectChangedScript = false;
		this.tree_ResultConfig.CancelNextSelectedChangedScript = false;
		this.tree_ResultConfig.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tree_ResultConfig.ColumnKey = null;
		this.tree_ResultConfig.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[8] { this.treeListColumnEx1, this.treeCol_Father, this.treeListColumnEx3, this.treeListColumnEx4, this.treeListColumnEx5, this.treeListColumnEx6, this.treeListColumnEx7, this.treeListColumnEx8 });
		this.tree_ResultConfig.ContextMenuStrip = this.ctx_ResultTreeConfig;
		this.tree_ResultConfig.Cursor = System.Windows.Forms.Cursors.Default;
		this.tree_ResultConfig.DisableFormSetting = false;
		this.tree_ResultConfig.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tree_ResultConfig.EditColumn = null;
		this.tree_ResultConfig.FieldCurrency = null;
		this.tree_ResultConfig.FieldRate = null;
		treeListFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "TrafficLights4_3.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 4, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "TrafficLights4_4.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "TrafficLights4_1.png";
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		treeListFormatRule.Rule = formatConditionRuleIconSet;
		this.tree_ResultConfig.FormatRules.Add(treeListFormatRule);
		this.tree_ResultConfig.FormId = "oUrlForOutgoingData";
		this.tree_ResultConfig.HasFormSetting = false;
		this.tree_ResultConfig.IndexEditRow = 0;
		this.tree_ResultConfig.IndicatorWidth = 40;
		this.tree_ResultConfig.IsApzTable = false;
		this.tree_ResultConfig.IsLoadingData = false;
		this.tree_ResultConfig.ItemId = "OACTIONRESULT";
		this.tree_ResultConfig.KeyFieldName = "ConfigID";
		this.tree_ResultConfig.Location = new System.Drawing.Point(0, 0);
		this.tree_ResultConfig.LstColumnNotEditable = null;
		this.tree_ResultConfig.LstColumNotSetting = null;
		this.tree_ResultConfig.LstOldUDF = null;
		this.tree_ResultConfig.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tree_ResultConfig.Name = "tree_ResultConfig";
		this.tree_ResultConfig.OptionsBehavior.AutoChangeParent = false;
		this.tree_ResultConfig.OptionsBehavior.CloseEditorOnLostFocus = false;
		this.tree_ResultConfig.OptionsBehavior.PopulateServiceColumns = true;
		this.tree_ResultConfig.OptionsBehavior.ShowToolTips = false;
		this.tree_ResultConfig.OptionsBehavior.SmartMouseHover = false;
		this.tree_ResultConfig.OptionsDragAndDrop.AcceptOuterNodes = true;
		this.tree_ResultConfig.OptionsDragAndDrop.CanCloneNodesOnDrop = true;
		this.tree_ResultConfig.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Multiple;
		this.tree_ResultConfig.OptionsDragAndDrop.DropNodesMode = DevExpress.XtraTreeList.DropNodesMode.Advanced;
		this.tree_ResultConfig.OptionsSelection.KeepSelectedOnClick = false;
		this.tree_ResultConfig.OptionsSelection.SelectNodesOnRightClick = true;
		this.tree_ResultConfig.OptionsView.AutoWidth = false;
		this.tree_ResultConfig.OptionsView.EnableAppearanceEvenRow = true;
		this.tree_ResultConfig.OptionsView.EnableAppearanceOddRow = true;
		this.tree_ResultConfig.ParentFieldName = "FatherNum";
		this.tree_ResultConfig.ReadOnly = false;
		this.tree_ResultConfig.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_btnAddChild });
		this.tree_ResultConfig.SettingReadonly = false;
		this.tree_ResultConfig.Size = new System.Drawing.Size(1044, 281);
		this.tree_ResultConfig.TabIndex = 1;
		this.tree_ResultConfig.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tree_ResultConfig_CellValueChanged);
		this.treeListColumnEx1.Caption = "ID";
		this.treeListColumnEx1.CurrencyDateField = null;
		this.treeListColumnEx1.CurrencyField = null;
		this.treeListColumnEx1.CurrencyFixed = null;
		this.treeListColumnEx1.CurrencyFixedRate = 0.0;
		this.treeListColumnEx1.CurrencyRateField = null;
		this.treeListColumnEx1.FieldName = "ConfigID";
		this.treeListColumnEx1.Name = "treeListColumnEx1";
		this.treeCol_Father.Caption = "Father Num";
		this.treeCol_Father.CurrencyDateField = null;
		this.treeCol_Father.CurrencyField = null;
		this.treeCol_Father.CurrencyFixed = null;
		this.treeCol_Father.CurrencyFixedRate = 0.0;
		this.treeCol_Father.CurrencyRateField = null;
		this.treeCol_Father.FieldName = "FatherNum";
		this.treeCol_Father.Name = "treeCol_Father";
		this.treeListColumnEx3.Caption = "Name";
		this.treeListColumnEx3.ColumnEdit = this.rep_btnAddChild;
		this.treeListColumnEx3.CurrencyDateField = null;
		this.treeListColumnEx3.CurrencyField = null;
		this.treeListColumnEx3.CurrencyFixed = null;
		this.treeListColumnEx3.CurrencyFixedRate = 0.0;
		this.treeListColumnEx3.CurrencyRateField = null;
		this.treeListColumnEx3.FieldName = "Name";
		this.treeListColumnEx3.Name = "treeListColumnEx3";
		this.treeListColumnEx3.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.treeListColumnEx3.Visible = true;
		this.treeListColumnEx3.VisibleIndex = 0;
		this.treeListColumnEx3.Width = 150;
		this.rep_btnAddChild.AutoHeight = false;
		this.rep_btnAddChild.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.rep_btnAddChild.Name = "rep_btnAddChild";
		this.rep_btnAddChild.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnAddChild_ButtonClick);
		this.treeListColumnEx4.Caption = "Type";
		this.treeListColumnEx4.ColumnEdit = this.rep_JsonType;
		this.treeListColumnEx4.CurrencyDateField = null;
		this.treeListColumnEx4.CurrencyField = null;
		this.treeListColumnEx4.CurrencyFixed = null;
		this.treeListColumnEx4.CurrencyFixedRate = 0.0;
		this.treeListColumnEx4.CurrencyRateField = null;
		this.treeListColumnEx4.FieldName = "JsonType";
		this.treeListColumnEx4.Name = "treeListColumnEx4";
		this.treeListColumnEx4.Visible = true;
		this.treeListColumnEx4.VisibleIndex = 1;
		this.treeListColumnEx4.Width = 102;
		this.treeListColumnEx5.Caption = "Data Type";
		this.treeListColumnEx5.ColumnEdit = this.rep_ConfigDataType;
		this.treeListColumnEx5.CurrencyDateField = null;
		this.treeListColumnEx5.CurrencyField = null;
		this.treeListColumnEx5.CurrencyFixed = null;
		this.treeListColumnEx5.CurrencyFixedRate = 0.0;
		this.treeListColumnEx5.CurrencyRateField = null;
		this.treeListColumnEx5.FieldName = "DataType";
		this.treeListColumnEx5.Name = "treeListColumnEx5";
		this.treeListColumnEx5.Visible = true;
		this.treeListColumnEx5.VisibleIndex = 2;
		this.treeListColumnEx5.Width = 121;
		this.treeListColumnEx6.Caption = "Value";
		this.treeListColumnEx6.CurrencyDateField = null;
		this.treeListColumnEx6.CurrencyField = null;
		this.treeListColumnEx6.CurrencyFixed = null;
		this.treeListColumnEx6.CurrencyFixedRate = 0.0;
		this.treeListColumnEx6.CurrencyRateField = null;
		this.treeListColumnEx6.FieldName = "Value";
		this.treeListColumnEx6.Name = "treeListColumnEx6";
		this.treeListColumnEx6.Visible = true;
		this.treeListColumnEx6.VisibleIndex = 3;
		this.treeListColumnEx6.Width = 185;
		this.treeListColumnEx7.Caption = "Default Value";
		this.treeListColumnEx7.CurrencyDateField = null;
		this.treeListColumnEx7.CurrencyField = null;
		this.treeListColumnEx7.CurrencyFixed = null;
		this.treeListColumnEx7.CurrencyFixedRate = 0.0;
		this.treeListColumnEx7.CurrencyRateField = null;
		this.treeListColumnEx7.FieldName = "DfltValue";
		this.treeListColumnEx7.Name = "treeListColumnEx7";
		this.treeListColumnEx7.Visible = true;
		this.treeListColumnEx7.VisibleIndex = 4;
		this.treeListColumnEx7.Width = 172;
		this.treeListColumnEx8.Caption = "Description";
		this.treeListColumnEx8.CurrencyDateField = null;
		this.treeListColumnEx8.CurrencyField = null;
		this.treeListColumnEx8.CurrencyFixed = null;
		this.treeListColumnEx8.CurrencyFixedRate = 0.0;
		this.treeListColumnEx8.CurrencyRateField = null;
		this.treeListColumnEx8.FieldName = "Dscription";
		this.treeListColumnEx8.Name = "treeListColumnEx8";
		this.treeListColumnEx8.Visible = true;
		this.treeListColumnEx8.VisibleIndex = 5;
		this.treeListColumnEx8.Width = 250;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 477);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 90;
		this.btn_Save.Text = "OK";
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
		this.btnCancel.Location = new System.Drawing.Point(99, 477);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 89;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1049, 500);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oUrlForOutgoingData";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ExternalRouting.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ExternalRouting";
		this.Text = "URI for Outgoing Data";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ExternalRouting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_JsonType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ConfigDataType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceValue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActionType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ResultType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Controller).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.ctx_ResultTreeConfig.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		this.xtraTabPageEx1.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_DefinedQuery.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_TextQuery.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DataFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SourceData).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SourceData).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DataType).EndInit();
		this.xtraTabPageEx2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tree_ResultConfig).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAddChild).EndInit();
		base.ResumeLayout(false);
	}
}
