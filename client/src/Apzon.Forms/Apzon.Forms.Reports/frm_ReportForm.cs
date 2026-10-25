using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Interfaces;
using Apzon.Controls.Pivot;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Spreadsheet;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Reports;

public class frm_ReportForm : XtraFormEx
{
	public IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private const string FormController = "Commons";

	private TreeEx _tree;

	private PivotGridControlEx _pivot;

	private int structureId;

	private DataTable defaultStructure = new DataTable("Structure");

	private DataTable defaultValid = new DataTable("Valid");

	private DataTable paramValid;

	public bool isExecuteWhenLoad;

	public List<Common.ParameterValue> defaultParamValue;

	public string _storeName = string.Empty;

	public string _fileName = string.Empty;

	public string ItemId = string.Empty;

	public DataRow dataRow;

	public string mappingParams;

	public string _tableType = "1";

	private string _queryType = "S";

	private string _qString = "";

	private bool isNotChangeSetting;

	private bool isLoadForm;

	private List<UserFieldsImpl> lstUdf = new List<UserFieldsImpl>();

	private List<string> lstFormOpen = new List<string>();

	private List<Common.ParameterValue> _lstParamValue = new List<Common.ParameterValue>();

	private IContainer components;

	private GridControlEx grd_Data;

	private SimpleButtonEx btn_Execute;

	private SimpleButtonEx btn_Cancel;

	private SplitContainerControl splitContainerControl1;

	private GridViewEx gv_Data;

	private GroupControl grp_Control;

	private XtraScrollableControl xtr_Control;

	private CheckBox chk_SetDefault;

	private LabelLine lbl_FormSetting;

	private LookUpEdit lue_FormSetting;

	private SimpleButtonEx btn_SaveFormSetting;

	private SimpleButtonEx btn_getImg;

	public frm_ReportForm()
	{
		InitializeComponent();
		gv_Data.FormId = ApzObjectTypes.oUserDefinedReports.ToString();
	}

	public frm_ReportForm(string docCode, List<Common.ParameterValue> lstParamValue = null)
	{
		InitializeComponent();
		FormId = ApzObjectTypes.oUserDefinedReports.ToString();
		ItemId = docCode;
		gv_Data.FormId = ApzObjectTypes.oUserDefinedReports.ToString();
		gv_Data.ItemId = docCode;
		gv_Data.DisableFormSetting = true;
		if (lstParamValue != null && lstParamValue.Count > 0)
		{
			defaultParamValue = lstParamValue;
		}
	}

	public async Task REFRESH_DATA()
	{
		await Execute();
	}

	private async Task frm_ReportForm_OnLoadAsync(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			if (!string.IsNullOrEmpty(Name) && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == Name))
			{
				if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == Name && x.authoriza == "F"))
				{
					WinFuntions.DisableAllControl(Controls);
				}
				if (string.IsNullOrEmpty(ItemId))
				{
					return;
				}
				btn_Execute.Enabled = false;
				btn_Cancel.Enabled = false;
				gv_Data.OptionsBehavior.EditorShowMode = EditorShowMode.MouseDown;
				gv_Data.DisableFormSetting = true;
				DataTable dt = await GetStoreName(ItemId);
				if (!dt.IsNotNull())
				{
					return;
				}
				if (dt.Columns.Contains("EnbExlHeader") && Function.ToString(dt.Rows[0]["EnbExlHeader"]) != "N")
				{
					gv_Data.HeaderExcelExport = ((Function.ToString(dt.Rows[0]["ExcelHeader"]) != "") ? dt.Rows[0]["ExcelHeader"].ToString() : "");
					string text = Function.ToString(dt.Rows[0]["EnbExlHeader"]);
					gv_Data.HeaderExcelType = ((text == "S") ? Enums.HeaderExcelType.Split : ((text == "M") ? Enums.HeaderExcelType.Merge : Enums.HeaderExcelType.Disabled));
					if (dt.Columns.Contains("ExlHeaderAlign"))
					{
						string text2 = Function.ToString(dt.Rows[0]["ExlHeaderAlign"]);
						gv_Data.HeaderExcelAlign = ((text2 == "C") ? Enums.HeaderExcelAlign.Center : ((text2 == "R") ? Enums.HeaderExcelAlign.Right : Enums.HeaderExcelAlign.Left));
					}
				}
				_storeName = Function.ToString(dt.Rows[0]["StoreName"]);
				_fileName = Function.ToString(dt.Rows[0]["ReportFile"]);
				_tableType = Function.ToString(dt.Rows[0]["TYPE"]);
				Text = Function.ToString(dt.Rows[0]["MenuCapt"]);
				if (dt.IsExistsColumn("QueryType"))
				{
					_queryType = Function.ToString(dt.Rows[0]["QueryType"]);
				}
				string text3 = Function.ToString(dt.Rows[0]["TableName"]);
				if (string.IsNullOrEmpty(text3))
				{
					lstUdf = await _httpClient.Udf.GetListField(text3);
				}
				await GetListFormSetting();
				if (_queryType.Equals("U"))
				{
					DataTable dataTable = await _httpClient.Queries.GetQueryById(Function.ParseInt(_storeName));
					if (dataTable.IsNotNull())
					{
						_qString = Function.ToString(dataTable.Rows[0]["QString"]);
					}
				}
				if (Function.ToString(dt.Rows[0]["AllRowsInGrp"]) == "Y")
				{
					gv_Data.SelectionChanged += Gv_Data_SelectionChanged;
				}
				bool showCheckboxSelect = !dt.Columns.Contains("ShChkBSelect") || Function.ToString(dt.Rows[0]["ShChkBSelect"]) != "N";
				await LoadData(showCheckboxSelect);
				btn_Execute.Enabled = true;
				btn_Cancel.Enabled = true;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.FormAccessDenied, "");
				WinFuntions.DisableAllControl(Controls);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Gv_Data_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			int num = ((!gv_Data.CalcHitInfo(grd_Data.PointToClient(Control.MousePosition)).InRowCell) ? gv_Data.FocusedRowHandle : e.ControllerRow);
			if (num < 0)
			{
				return;
			}
			int parentRowHandle = gv_Data.GetParentRowHandle(num);
			int childRowCount = gv_Data.GetChildRowCount(parentRowHandle);
			gv_Data.BeginSelection();
			if (gv_Data.IsRowSelected(num))
			{
				for (int i = 0; i < childRowCount; i++)
				{
					gv_Data.SelectRow(gv_Data.GetChildRowHandle(parentRowHandle, i));
				}
				gv_Data.SelectRow(parentRowHandle);
			}
			else
			{
				for (int j = 0; j < childRowCount; j++)
				{
					gv_Data.UnselectRow(gv_Data.GetChildRowHandle(parentRowHandle, j));
				}
				gv_Data.UnselectRow(parentRowHandle);
			}
			gv_Data.EndSelection();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void Excel_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dt = new DataTable();
			if (_tableType.Equals("2") && _tree != null)
			{
				dt = _tree.DataSourceTable();
			}
			else if (_tableType.Equals("3") && _pivot != null)
			{
				dt = _pivot.DataSourceTable();
			}
			else if (grd_Data != null)
			{
				dt = grd_Data.DataSourceTable();
			}
			new frm_Speadsheet(dt, defaultStructure, Text).ShowDialog();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Execute()
	{
		_ = 10;
		try
		{
			WinCommons._OpenProcessing();
			isNotChangeSetting = true;
			btn_Execute.Enabled = false;
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			foreach (Control control in xtr_Control.Controls)
			{
				Type type = control.GetType();
				if (!type.Name.Equals("LabelControl"))
				{
					switch (type.Name)
					{
					case "CalcEdit":
					{
						CalcEdit calcEdit = (CalcEdit)control;
						Common.ParameterValue parameterValue12 = new Common.ParameterValue();
						parameterValue12.Value = calcEdit.EditValue;
						parameterValue12.Name = calcEdit.Name;
						list.Add(parameterValue12);
						break;
					}
					case "CalcEditEx":
					{
						CalcEditEx calcEditEx = (CalcEditEx)control;
						Common.ParameterValue parameterValue11 = new Common.ParameterValue();
						parameterValue11.Value = calcEditEx.EditValue;
						parameterValue11.Name = calcEditEx.Name;
						list.Add(parameterValue11);
						break;
					}
					case "CheckEdit":
					{
						CheckEdit checkEdit = (CheckEdit)control;
						Common.ParameterValue parameterValue10 = new Common.ParameterValue();
						parameterValue10.Value = Function.ParseBool(checkEdit.Checked);
						parameterValue10.Name = checkEdit.Name;
						list.Add(parameterValue10);
						break;
					}
					case "CheckEditEx":
					{
						CheckEditEx checkEditEx = (CheckEditEx)control;
						Common.ParameterValue parameterValue9 = new Common.ParameterValue();
						parameterValue9.Value = Function.ParseBool(checkEditEx.Checked);
						parameterValue9.Name = checkEditEx.Name;
						list.Add(parameterValue9);
						break;
					}
					case "DateEditEx":
					{
						DateEditEx dateEditEx = (DateEditEx)control;
						Common.ParameterValue parameterValue8 = new Common.ParameterValue();
						parameterValue8.Value = Function.ParseDateTime(dateEditEx.EditValue);
						parameterValue8.Name = dateEditEx.Name;
						list.Add(parameterValue8);
						break;
					}
					case "TimeEditEx":
					{
						TimeEditEx timeEditEx = (TimeEditEx)control;
						Common.ParameterValue parameterValue7 = new Common.ParameterValue();
						parameterValue7.Value = Function.ParseDateTime(timeEditEx.EditValue);
						parameterValue7.Name = timeEditEx.Name;
						list.Add(parameterValue7);
						break;
					}
					case "LookUpEdit":
					{
						LookUpEdit lookUpEdit = (LookUpEdit)control;
						Common.ParameterValue parameterValue6 = new Common.ParameterValue();
						parameterValue6.Value = Function.ToString(lookUpEdit.EditValue);
						parameterValue6.Name = lookUpEdit.Name;
						list.Add(parameterValue6);
						break;
					}
					case "LookUpEditEx":
					{
						LookUpEditEx lookUpEditEx = (LookUpEditEx)control;
						Common.ParameterValue parameterValue5 = new Common.ParameterValue();
						parameterValue5.Value = Function.ToString(lookUpEditEx.EditValue);
						parameterValue5.Name = lookUpEditEx.Name;
						list.Add(parameterValue5);
						break;
					}
					case "ButtonEdit":
					{
						ButtonEdit buttonEdit = (ButtonEdit)control;
						Common.ParameterValue parameterValue4 = new Common.ParameterValue();
						parameterValue4.Value = Function.ToString(buttonEdit.EditValue);
						parameterValue4.Name = buttonEdit.Name;
						list.Add(parameterValue4);
						break;
					}
					case "ButtonEditEx":
					{
						ButtonEditEx buttonEditEx = (ButtonEditEx)control;
						Common.ParameterValue parameterValue3 = new Common.ParameterValue();
						parameterValue3.Value = Function.ToString(buttonEditEx.EditValue);
						parameterValue3.Name = buttonEditEx.Name;
						list.Add(parameterValue3);
						break;
					}
					case "TextEdit":
					{
						TextEdit textEdit = (TextEdit)control;
						Common.ParameterValue parameterValue2 = new Common.ParameterValue();
						parameterValue2.Value = Function.ToString(textEdit.EditValue);
						parameterValue2.Name = textEdit.Name;
						list.Add(parameterValue2);
						break;
					}
					case "TextEditEx":
					{
						TextEditEx textEditEx = (TextEditEx)control;
						Common.ParameterValue parameterValue = new Common.ParameterValue();
						parameterValue.Value = Function.ToString(textEditEx.EditValue);
						parameterValue.Name = textEditEx.Name;
						list.Add(parameterValue);
						break;
					}
					}
				}
			}
			new DataSet();
			DataSet ds;
			if (_queryType.Equals("U"))
			{
				List<QueryParamater> list2 = new List<QueryParamater>();
				foreach (Common.ParameterValue item2 in list)
				{
					list2.Add(new QueryParamater
					{
						ParaName = item2.Name,
						Value = item2.Value
					});
				}
				string qString = _qString;
				qString = Regex.Replace(qString, "@CURRENTUSER", "N'" + GlobalData.UserName + "'", RegexOptions.IgnoreCase);
				qString = Regex.Replace(qString, "\\$\\[USER\\]", $"N'{GlobalData.UserID}'", RegexOptions.IgnoreCase);
				qString = Regex.Replace(qString, "@CURRENTBRANCH", $"N'{GlobalData.DefaultBranch}'", RegexOptions.IgnoreCase);
				ds = _httpClient.Commons.ExecuteSql(qString, list2);
				if (ds != null && ds.Tables.Count > 1)
				{
					DataTable dataTable = ds.Tables["Message"];
					if (dataTable.IsNotNull())
					{
						string text = Function.ToString(dataTable.Rows[0]["MsgType"]);
						string msg = Function.ToString(dataTable.Rows[0]["Message"]);
						if (!(text == "200"))
						{
							if (text == "5xx")
							{
								WinCommons.Error(Enums.MessageCode.None, msg);
								ds.DisposeDataSet();
								return;
							}
						}
						else
						{
							WinCommons.Success(Enums.MessageCode.None, msg);
							ds.Tables.RemoveAt(0);
						}
					}
				}
			}
			else
			{
				foreach (Common.ParameterValue item in list)
				{
					_lstParamValue.FirstOrDefault((Common.ParameterValue m) => m.Name == item.Name).Value = item.Value;
				}
				DataSetResult dataSetResult = await _httpClient.Commons.ExecuteStore(_storeName, _lstParamValue);
				if (!string.IsNullOrEmpty(dataSetResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				}
				ds = dataSetResult.Data;
				if (ds.IsHasStructure())
				{
					defaultStructure = ds.Tables[0].Copy();
				}
				if (ds.IsHasValidValue())
				{
					defaultValid = ds.Tables[2].Copy();
				}
			}
			if (lue_FormSetting.EditValue != null && Function.ParseInt(lue_FormSetting.EditValue) > 0)
			{
				DataSet dsNew = new DataSet();
				DataTable dataTable2 = new DataTable("Data");
				if (ds.IsNotNull() && ds.Tables.Count == 1)
				{
					dataTable2 = ds.Tables[0].Copy();
				}
				else if (ds.IsNotNull() && ds.Tables.Count > 1)
				{
					dataTable2 = ds.Tables[1].Copy();
				}
				dsNew.Tables.Add(defaultStructure.Copy());
				dsNew.Tables.Add(dataTable2.Copy());
				dsNew.Tables.Add(defaultValid.Copy());
				if (_tableType.Equals("1"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dsNew, lstUdf, expand: true);
				}
				else if (_tableType.Equals("2"))
				{
					await TreeListHelper.GenerateTreeList(_tree, dsNew, lstUdf, expand: true);
				}
				else if (_tableType.Equals("3"))
				{
					await PivotGridHelper.GeneratePivotGrid(_pivot, dsNew, lstUdf, expand: true);
				}
				dsNew.DisposeDataSet();
				return;
			}
			DataSet dsStruc = await _httpClient.FormSettingClient.GetStructure(0, ApzObjectTypes.oUserDefinedReports.ToString(), ItemId, GlobalData.UserName, GlobalData.DefaultBranch, "0");
			if (dsStruc.IsNotNull() && dsStruc.Tables[0].IsNotNull())
			{
				structureId = Function.ParseInt(dsStruc.Tables[0].Rows[0]["ID"]);
				dsStruc.Tables[0].TableName = "Structure";
				defaultStructure = dsStruc.Tables[0].Copy();
				if (dsStruc.IsHasStructure())
				{
					dsStruc.Tables[1].TableName = "Valid";
					defaultValid = dsStruc.Tables[1].Copy();
					dsStruc.Tables.RemoveAt(1);
				}
				for (int num = 0; num < ds.Tables.Count; num++)
				{
					dsStruc.Tables.Add(ds.Tables[num].Copy());
				}
				dsStruc.Tables.Add(defaultValid.Copy());
				if (_tableType.Equals("2"))
				{
					await TreeListHelper.GenerateTreeList(_tree, dsStruc, lstUdf, expand: true);
				}
				else if (!_tableType.Equals("3"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dsStruc, lstUdf, expand: true);
				}
				else
				{
					await PivotGridHelper.GeneratePivotGrid(_pivot, dsStruc, lstUdf, expand: true);
				}
			}
			else
			{
				if (ds.IsHasStructure() && ds.Tables[0].IsExistsColumn("ID"))
				{
					structureId = Function.ParseInt(ds.Tables[0].Rows[0]["ID"]);
					int num2 = Function.ParseInt(ds.Tables[0].Rows[1]["ID"]);
					if (structureId != num2)
					{
						structureId = 0;
					}
				}
				if (_tableType.Equals("2"))
				{
					await TreeListHelper.GenerateTreeList(_tree, ds, lstUdf, expand: true);
				}
				else if (!_tableType.Equals("3"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, ds, lstUdf, expand: true);
				}
				else
				{
					await PivotGridHelper.GeneratePivotGrid(_pivot, ds, lstUdf, expand: true);
				}
			}
			ds.DisposeDataSet();
			dsStruc.DisposeDataSet();
			lue_FormSetting.EditValue = structureId;
			if (structureId == 0)
			{
				chk_SetDefault.Visible = false;
				btn_SaveFormSetting.Visible = false;
				lbl_FormSetting.Visible = false;
				lue_FormSetting.Visible = false;
			}
			else if (lue_FormSetting.Properties.GetDataSourceRowByKeyValue(lue_FormSetting.EditValue) is DataRowView dataRowView)
			{
				chk_SetDefault.Checked = Function.ParseBool(dataRowView["SetDefault"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			btn_Execute.Enabled = true;
			isNotChangeSetting = false;
			WinCommons._CloseProcessing();
		}
	}

	private async void bt_Execute_Click(object sender, EventArgs e)
	{
		await Execute();
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_SaveFormSetting_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveFormSetting();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void lue_FormSetting_EditValueChanged(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			if (isNotChangeSetting)
			{
				return;
			}
			WinCommons._OpenProcessing();
			isLoadForm = true;
			structureId = Function.ParseInt(lue_FormSetting.EditValue);
			DataSet ds = await _httpClient.FormSettingClient.GetStructure(structureId, ApzObjectTypes.oUserDefinedReports.ToString(), ItemId, GlobalData.UserName, GlobalData.DefaultBranch, "1");
			if (ds.IsHasStructure())
			{
				defaultStructure = ds.Tables[1].Copy();
				defaultValid = await _httpClient.FormSettingClient.GetValidData(ApzObjectTypes.oUserDefinedReports.ToString(), ItemId, GlobalData.UserName, structureId);
				DataSet dsNew = new DataSet();
				DataTable dataTable = new DataTable("Data");
				if (_tableType.Equals("1"))
				{
					dataTable = grd_Data.DatasourceTable();
					gv_Data.GroupSummary.Clear();
					gv_Data.ColumnsEx.Clear();
				}
				else if (_tableType.Equals("2"))
				{
					dataTable = _tree.DataSourceTable();
					_tree.Columns.Clear();
				}
				else if (_tableType.Equals("3"))
				{
					dataTable = _pivot.DataSourceTable();
					_pivot.Fields.Clear();
				}
				dsNew.Tables.Add(defaultStructure.Copy());
				dsNew.Tables.Add(dataTable.Copy());
				dsNew.Tables.Add(defaultValid.Copy());
				if (_tableType.Equals("1"))
				{
					await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dsNew, lstUdf, expand: true);
				}
				else if (_tableType.Equals("2"))
				{
					await TreeListHelper.GenerateTreeList(_tree, dsNew, lstUdf, expand: true);
				}
				else if (_tableType.Equals("3"))
				{
					await PivotGridHelper.GeneratePivotGrid(_pivot, dsNew, lstUdf, expand: true);
				}
				dsNew.DisposeDataSet();
				if (lue_FormSetting.Properties.GetDataSourceRowByKeyValue(lue_FormSetting.EditValue) is DataRowView dataRowView)
				{
					chk_SetDefault.Checked = Function.ParseBool(dataRowView["SetDefault"]);
				}
				ds.DisposeDataSet();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isLoadForm = false;
			WinCommons._CloseProcessing();
		}
	}

	private void chk_SetDefault_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!isLoadForm && Function.ParseInt(lue_FormSetting.EditValue) != 0)
			{
				DataRow dataRow = ((DataTable)lue_FormSetting.Properties.DataSource).Select("ID = " + lue_FormSetting.EditValue).FirstOrDefault();
				if (dataRow != null)
				{
					dataRow["SetDefault"] = chk_SetDefault.Checked;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ReportForm_Activated(object sender, EventArgs e)
	{
		try
		{
			if (lstFormOpen.Count <= 0)
			{
				return;
			}
			foreach (Form openForm in Application.OpenForms)
			{
				if (!openForm.IsDisposed && !openForm.Disposing && openForm.GetType() == typeof(frm_ReportFormSettings) && openForm.Text.Equals(lstFormOpen[0]))
				{
					openForm.Activate();
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

	private void frm_ReportForm_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (lstFormOpen.Count <= 0)
			{
				return;
			}
			foreach (Form openForm in Application.OpenForms)
			{
				if (openForm.GetType() == typeof(frm_ReportFormSettings) && openForm.Text.Equals(lstFormOpen[0]))
				{
					openForm.Close();
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

	private async Task LoadData(bool showCheckboxSelect = true)
	{
		int tabOrder = 0;
		int labelX = 20;
		int labelY = 10;
		int controlX = 150;
		int controlY = 10;
		int panelHeight = controlY;
		ToolStripMenuItem toolStripMenuItem = new ToolStripMenuItem
		{
			Name = "CTM_Excel_Design",
			Text = "Excel Merge Data"
		};
		toolStripMenuItem.Click += Excel_Click;
		if (_tableType.Equals("2"))
		{
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem
			{
				Name = "TreeList_Expand_All",
				Text = "Expand All"
			};
			toolStripMenuItem2.Click += expandCollapseTree_Click;
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem
			{
				Name = "TreeList_Collapse_All",
				Text = "Collapse All"
			};
			toolStripMenuItem3.Click += expandCollapseTree_Click;
			splitContainerControl1.Panel2.Controls.Clear();
			_tree = new TreeEx();
			_tree.FormId = ApzObjectTypes.oUserDefinedReports.ToString();
			_tree.ItemId = gv_Data.ItemId;
			if (showCheckboxSelect)
			{
				_tree.OptionsSelection.MultiSelectMode = TreeListMultiSelectMode.RowSelect;
				_tree.OptionsSelection.MultiSelect = true;
				_tree.OptionsView.CheckBoxStyle = DefaultNodeCheckBoxStyle.Check;
			}
			splitContainerControl1.Panel2.Controls.Add(_tree);
			_tree.Dock = DockStyle.Fill;
			_tree.DisableFormSetting = true;
			_tree.AllowPasteExcel = true;
			ContextMenuStrip contextMenuStrip = _tree.ContextMenuStrip;
			if (contextMenuStrip == null)
			{
				contextMenuStrip = new ContextMenuStrip();
				_tree.ContextMenuStrip = contextMenuStrip;
			}
			else
			{
				ToolStripSeparator value = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value);
			}
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			contextMenuStrip.Items.Add(toolStripMenuItem);
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			contextMenuStrip.Items.Add(toolStripMenuItem3);
		}
		else if (_tableType.Equals("3"))
		{
			splitContainerControl1.Panel2.Controls.Clear();
			_pivot = new PivotGridControlEx();
			_pivot.FormId = ApzObjectTypes.oUserDefinedReports.ToString();
			_pivot.ItemId = gv_Data.ItemId;
			splitContainerControl1.Panel2.Controls.Add(_pivot);
			_pivot.Dock = DockStyle.Fill;
			_pivot.AllowPasteExcel = true;
			ContextMenuStrip contextMenuStrip2 = _pivot.ContextMenuStrip;
			if (contextMenuStrip2 == null)
			{
				contextMenuStrip2 = new ContextMenuStrip();
				_pivot.ContextMenuStrip = contextMenuStrip2;
			}
			else
			{
				ToolStripSeparator value2 = new ToolStripSeparator();
				contextMenuStrip2.Items.Add(value2);
			}
			contextMenuStrip2.Items.Add(new ToolStripSeparator());
			contextMenuStrip2.Items.Add(toolStripMenuItem);
		}
		else
		{
			if (showCheckboxSelect)
			{
				gv_Data.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
				gv_Data.OptionsSelection.ShowCheckBoxSelectorInGroupRow = DefaultBoolean.True;
			}
			ContextMenuStrip contextMenuStrip3 = grd_Data.ContextMenuStrip;
			if (contextMenuStrip3 == null)
			{
				contextMenuStrip3 = new ContextMenuStrip();
				grd_Data.ContextMenuStrip = contextMenuStrip3;
			}
			else
			{
				ToolStripSeparator value3 = new ToolStripSeparator();
				contextMenuStrip3.Items.Add(value3);
			}
			contextMenuStrip3.Items.Add(new ToolStripSeparator());
			contextMenuStrip3.Items.Add(toolStripMenuItem);
		}
		DataTable dt = await GetParamStore(_storeName);
		if (!dt.IsNotNull())
		{
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel2;
			await Execute();
			return;
		}
		int visibleIndex = 0;
		bool isFullDfValue = true;
		Dictionary<IEditControlEx, object> lstDfltValue = new Dictionary<IEditControlEx, object>();
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			string paraName = Function.ToString(dt.Rows[i]["ParamName"]);
			string text = Function.ToString(dt.Rows[i]["DataType"]).ToLower();
			string subType = Function.ToString(dt.Rows[i]["SubType"]);
			string text2 = Function.ToString(dt.Rows[i]["FormatString"]);
			object defaultValue = getDfValue(paraName, dt.Rows[i]["DefaultValue"]);
			string text3 = Function.ToString(dt.Rows[i]["Label"]);
			string toolTip = Function.ToString(dt.Rows[i]["ToolTip"]);
			bool allowMultiple = Function.ParseBool(dt.Rows[i]["AllowMultiple"]);
			bool visible = Function.ParseBool(dt.Rows[i]["Visible"]);
			bool flag = Function.ParseBool(dt.Rows[i]["AllowEdit"]);
			string text4 = Function.ToString(dt.Rows[i]["Type"]).ToLower();
			if (defaultParamValue != null && defaultParamValue.Count > 0)
			{
				Common.ParameterValue parameterValue = defaultParamValue.FirstOrDefault((Common.ParameterValue m) => m.Name.Equals(paraName));
				if (parameterValue != null)
				{
					defaultValue = parameterValue.Value;
				}
			}
			if (!text4.Equals("datetime") && string.IsNullOrEmpty(Function.ToString(defaultValue)))
			{
				isFullDfValue = false;
			}
			LabelControl labelControl = new LabelControl();
			labelControl.Name = "lbl_" + text3;
			labelControl.Text = text3;
			labelControl.ToolTip = toolTip;
			if (!visible)
			{
				labelControl.Visible = false;
			}
			labelControl.Location = new Point(labelX, labelY);
			labelControl.Anchor = AnchorStyles.Top | AnchorStyles.Left;
			xtr_Control.Controls.Add(labelControl);
			if (string.IsNullOrEmpty(text) || text4.Equals(text) || _queryType.Equals("U"))
			{
				switch (text4)
				{
				case "int":
				case "smallint":
				case "bigint":
				{
					CalcEditEx calcEditEx = new CalcEditEx();
					calcEditEx.Location = new Point(controlX, controlY);
					calcEditEx.Name = paraName;
					calcEditEx.Tag = subType;
					calcEditEx.ToolTip = toolTip;
					calcEditEx.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					calcEditEx.TabIndex = tabOrder;
					tabOrder++;
					calcEditEx.Properties.DisplayFormat.FormatString = (string.IsNullOrEmpty(text2) ? "n0" : text2);
					calcEditEx.Properties.DisplayFormat.FormatType = FormatType.Numeric;
					calcEditEx.Properties.EditFormat.FormatString = (string.IsNullOrEmpty(text2) ? "n0" : text2);
					calcEditEx.Properties.EditFormat.FormatType = FormatType.Numeric;
					calcEditEx.Properties.EditMask = (string.IsNullOrEmpty(text2) ? "n0" : text2);
					calcEditEx.Properties.AllowMouseWheel = false;
					calcEditEx.Properties.Buttons.Clear();
					if (!string.IsNullOrEmpty(Function.ToString(defaultValue)))
					{
						lstDfltValue.Add(calcEditEx, Function.ParseInt(defaultValue));
					}
					if (!visible)
					{
						calcEditEx.Visible = false;
					}
					if (!flag)
					{
						calcEditEx.Enabled = false;
					}
					xtr_Control.Controls.Add(calcEditEx);
					calcEditEx.FieldName = paraName.Substring(1);
					break;
				}
				case "numeric":
				case "double":
				case "float":
				case "decimal":
				case "smallmoney":
				case "money":
				{
					CalcEditEx calcEditEx2 = new CalcEditEx();
					calcEditEx2.Location = new Point(controlX, controlY);
					calcEditEx2.Name = paraName;
					calcEditEx2.Tag = subType;
					calcEditEx2.ToolTip = toolTip;
					calcEditEx2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					calcEditEx2.TabIndex = tabOrder;
					tabOrder++;
					calcEditEx2.Properties.DisplayFormat.FormatString = (string.IsNullOrEmpty(text2) ? string.Empty : text2);
					calcEditEx2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
					calcEditEx2.Properties.EditFormat.FormatString = (string.IsNullOrEmpty(text2) ? string.Empty : text2);
					calcEditEx2.Properties.EditFormat.FormatType = FormatType.Numeric;
					calcEditEx2.Properties.EditMask = (string.IsNullOrEmpty(text2) ? string.Empty : text2);
					calcEditEx2.Properties.AllowMouseWheel = false;
					calcEditEx2.Properties.Buttons.Clear();
					if (!string.IsNullOrEmpty(Function.ToString(defaultValue)))
					{
						lstDfltValue.Add(calcEditEx2, Function.ParseDouble(defaultValue));
					}
					if (!visible)
					{
						calcEditEx2.Visible = false;
					}
					if (!flag)
					{
						calcEditEx2.Enabled = false;
					}
					xtr_Control.Controls.Add(calcEditEx2);
					calcEditEx2.FieldName = paraName.Substring(1);
					break;
				}
				case "datetime":
				{
					DateEditEx dateEditEx = new DateEditEx();
					dateEditEx.Location = new Point(controlX, controlY);
					dateEditEx.Name = paraName;
					dateEditEx.Tag = subType;
					dateEditEx.ToolTip = toolTip;
					dateEditEx.TabIndex = tabOrder;
					tabOrder++;
					lstDfltValue.Add(dateEditEx, string.IsNullOrEmpty(Function.ToString(defaultValue)) ? DateTime.Today : Function.ParseDateTimes(defaultValue));
					if (!visible)
					{
						dateEditEx.Visible = false;
					}
					if (!flag)
					{
						dateEditEx.Enabled = false;
					}
					xtr_Control.Controls.Add(dateEditEx);
					dateEditEx.FieldName = paraName.Substring(1);
					break;
				}
				case "bit":
				{
					CheckEditEx checkEditEx = new CheckEditEx();
					checkEditEx.Location = new Point(controlX, controlY);
					checkEditEx.Name = paraName;
					checkEditEx.ToolTip = toolTip;
					checkEditEx.Tag = subType;
					checkEditEx.Text = string.Empty;
					lstDfltValue.Add(checkEditEx, !string.IsNullOrEmpty(Function.ToString(defaultValue)) && Function.ParseBool(defaultValue));
					if (!visible)
					{
						checkEditEx.Visible = false;
					}
					if (!flag)
					{
						checkEditEx.Enabled = false;
					}
					checkEditEx.TabIndex = tabOrder;
					tabOrder++;
					xtr_Control.Controls.Add(checkEditEx);
					checkEditEx.FieldName = paraName.Substring(1);
					break;
				}
				default:
				{
					TextEditEx textEditEx = new TextEditEx();
					textEditEx.Location = new Point(controlX, controlY);
					textEditEx.Size = new Size(180, 20);
					textEditEx.Name = paraName;
					textEditEx.Tag = subType;
					textEditEx.ToolTip = toolTip;
					textEditEx.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					textEditEx.Text = string.Empty;
					textEditEx.TabIndex = tabOrder;
					tabOrder++;
					lstDfltValue.Add(textEditEx, string.IsNullOrEmpty(Function.ToString(defaultValue)) ? string.Empty : Function.ToString(defaultValue));
					if (!visible)
					{
						textEditEx.Visible = false;
					}
					if (!flag)
					{
						textEditEx.ReadOnly = true;
					}
					xtr_Control.Controls.Add(textEditEx);
					textEditEx.FieldName = paraName.Substring(1);
					break;
				}
				}
				if (_queryType.Equals("U") || text.Equals("queryparam"))
				{
					int count = xtr_Control.Controls.Count;
					SimpleButton simpleButton = new SimpleButton();
					simpleButton.Location = new Point(controlX + ((dynamic)xtr_Control.Controls[count - 1]).Size.Width + 5, controlY);
					simpleButton.Name = "btn_" + paraName;
					simpleButton.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					simpleButton.Size = new Size(25, 21);
					SimpleButton simpleButton2 = simpleButton;
					simpleButton2.Click += async (object s, EventArgs e) =>
					{
						try
						{
							string text6 = "";
							string[] array2 = subType.Split('/');
							string text7 = array2[0];
							if (array2.Length > 1)
							{
								text6 = array2[1];
							}
							string sql = "select distinct " + text7 + " as Value from " + text6;
							DataTable dt2 = await GetData(sql);
							if (dt2.IsNotNull())
							{
								CFLParamaterSql cFLParamaterSql = new CFLParamaterSql
								{
									Dt = dt2,
									MutilSelect = allowMultiple
								};
								cFLParamaterSql.ShowDialog(this);
								if (!allowMultiple && cFLParamaterSql.Value != null)
								{
									((dynamic)xtr_Control.Controls[count - 1]).EditValue = cFLParamaterSql.Value["Value"];
								}
								if (allowMultiple && cFLParamaterSql.ChooseItems != null)
								{
									string text8 = "";
									for (int j = 0; j < cFLParamaterSql.ChooseItems.Count; j++)
									{
										text8 = text8 + Function.ToString(cFLParamaterSql.ChooseItems[i]["Value"]) + ";";
									}
									if (text8.Length > 1)
									{
										text8 = text8.Substring(0, text8.Length - 1);
									}
									((dynamic)xtr_Control.Controls[count - 1]).EditValue = text8;
								}
							}
						}
						catch (Exception ex)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(3, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
							WinCommons.ErrorLineNumber(ex);
						}
					};
					simpleButton2.ImageOptions.Image = (Image)new ComponentResourceManager(typeof(frm_ReportForm)).GetObject("btn_getImg.ImageOptions.Image");
					xtr_Control.Controls.Add(simpleButton2);
				}
			}
			else
			{
				switch (text)
				{
				case "int":
				{
					CalcEditEx calcEditEx4 = new CalcEditEx();
					calcEditEx4.Location = new Point(controlX, controlY);
					calcEditEx4.Name = paraName;
					calcEditEx4.Tag = subType;
					calcEditEx4.ToolTip = toolTip;
					calcEditEx4.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					calcEditEx4.TabIndex = tabOrder;
					tabOrder++;
					calcEditEx4.Properties.DisplayFormat.FormatString = (string.IsNullOrEmpty(text2) ? "n0" : text2);
					calcEditEx4.Properties.DisplayFormat.FormatType = FormatType.Numeric;
					calcEditEx4.Properties.EditFormat.FormatString = (string.IsNullOrEmpty(text2) ? "n0" : text2);
					calcEditEx4.Properties.EditFormat.FormatType = FormatType.Numeric;
					calcEditEx4.Properties.EditMask = (string.IsNullOrEmpty(text2) ? "n0" : text2);
					calcEditEx4.Properties.AllowMouseWheel = false;
					calcEditEx4.Properties.Buttons.Clear();
					if (!string.IsNullOrEmpty(Function.ToString(defaultValue)))
					{
						lstDfltValue.Add(calcEditEx4, Function.ParseInt(defaultValue));
					}
					if (!visible)
					{
						calcEditEx4.Visible = false;
					}
					if (!flag)
					{
						calcEditEx4.Enabled = false;
					}
					xtr_Control.Controls.Add(calcEditEx4);
					calcEditEx4.FieldName = paraName.Substring(1);
					break;
				}
				case "numeric":
				{
					CalcEditEx calcEditEx3 = new CalcEditEx();
					calcEditEx3.Location = new Point(controlX, controlY);
					calcEditEx3.Name = paraName;
					calcEditEx3.Tag = subType;
					calcEditEx3.ToolTip = toolTip;
					calcEditEx3.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					calcEditEx3.TabIndex = tabOrder;
					tabOrder++;
					calcEditEx3.Properties.DisplayFormat.FormatString = (string.IsNullOrEmpty(text2) ? string.Empty : text2);
					calcEditEx3.Properties.DisplayFormat.FormatType = FormatType.Numeric;
					calcEditEx3.Properties.EditFormat.FormatString = (string.IsNullOrEmpty(text2) ? string.Empty : text2);
					calcEditEx3.Properties.EditFormat.FormatType = FormatType.Numeric;
					calcEditEx3.Properties.EditMask = (string.IsNullOrEmpty(text2) ? string.Empty : text2);
					calcEditEx3.Properties.AllowMouseWheel = false;
					calcEditEx3.Properties.Buttons.Clear();
					if (!string.IsNullOrEmpty(Function.ToString(defaultValue)))
					{
						lstDfltValue.Add(calcEditEx3, Function.ParseDouble(defaultValue));
					}
					if (!visible)
					{
						calcEditEx3.Visible = false;
					}
					if (!flag)
					{
						calcEditEx3.Enabled = false;
					}
					xtr_Control.Controls.Add(calcEditEx3);
					calcEditEx3.FieldName = paraName.Substring(1);
					break;
				}
				case "datetime":
				{
					DateEditEx dateEditEx2 = new DateEditEx();
					dateEditEx2.Location = new Point(controlX, controlY);
					dateEditEx2.Name = paraName;
					dateEditEx2.Tag = subType;
					dateEditEx2.ToolTip = toolTip;
					dateEditEx2.TabIndex = tabOrder;
					tabOrder++;
					lstDfltValue.Add(dateEditEx2, string.IsNullOrEmpty(Function.ToString(defaultValue)) ? DateTime.Today : Function.ParseDateTimes(defaultValue));
					if (!visible)
					{
						dateEditEx2.Visible = false;
					}
					if (!flag)
					{
						dateEditEx2.Enabled = false;
					}
					xtr_Control.Controls.Add(dateEditEx2);
					dateEditEx2.FieldName = paraName.Substring(1);
					break;
				}
				case "time":
				{
					TimeEditEx timeEditEx = new TimeEditEx();
					timeEditEx.Location = new Point(controlX, controlY);
					timeEditEx.Name = paraName;
					timeEditEx.Tag = subType;
					timeEditEx.ToolTip = toolTip;
					timeEditEx.TabIndex = tabOrder;
					tabOrder++;
					lstDfltValue.Add(timeEditEx, string.IsNullOrEmpty(Function.ToString(defaultValue)) ? DateTime.Now : Function.ParseDateTimes(defaultValue));
					if (!visible)
					{
						timeEditEx.Visible = false;
					}
					if (!flag)
					{
						timeEditEx.Enabled = false;
					}
					xtr_Control.Controls.Add(timeEditEx);
					timeEditEx.FieldName = paraName.Substring(1);
					break;
				}
				case "bit":
				{
					CheckEditEx checkEditEx2 = new CheckEditEx();
					checkEditEx2.Location = new Point(controlX, controlY);
					checkEditEx2.Name = paraName;
					checkEditEx2.ToolTip = toolTip;
					checkEditEx2.Tag = subType;
					checkEditEx2.Text = string.Empty;
					lstDfltValue.Add(checkEditEx2, !string.IsNullOrEmpty(Function.ToString(defaultValue)) && Function.ParseBool(defaultValue));
					if (!visible)
					{
						checkEditEx2.Visible = false;
					}
					if (!flag)
					{
						checkEditEx2.Enabled = false;
					}
					checkEditEx2.TabIndex = tabOrder;
					tabOrder++;
					xtr_Control.Controls.Add(checkEditEx2);
					checkEditEx2.FieldName = paraName.Substring(1);
					break;
				}
				case "cfl":
				{
					ButtonEditEx bte = new ButtonEditEx();
					bte.Location = new Point(controlX, controlY);
					bte.Size = new Size(180, 20);
					bte.Name = paraName;
					bte.ToolTip = toolTip;
					bte.Tag = subType;
					if (!visible)
					{
						bte.Visible = false;
					}
					if (!flag)
					{
						bte.Properties.ReadOnly = true;
					}
					bte.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					bte.TabIndex = tabOrder;
					tabOrder++;
					bte.Properties.Buttons.Clear();
					bte.Properties.Buttons.AddRange(new EditorButton[1]
					{
						new EditorButton(ButtonPredefines.Search)
					});
					bte.Properties.ButtonClick += (object s, ButtonPressedEventArgs v) =>
					{
						try
						{
							List<Common.ParameterValue> lstpr4 = new List<Common.ParameterValue>();
							string userProcedure4 = GetUserProcedure(subType, ref lstpr4);
							frm_CFL frm = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, allowMultiple, "", userProcedure4, Enums.ItemObjectType.NoItemType, lstpr4, "");
							if (allowMultiple)
							{
								frm.ButtonEditMultiSelect = allowMultiple;
								frm_CFL frm_CFL2 = frm;
								frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
								{
									frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
									string[] array2 = bte.Text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
									GridColumn column = new GridColumn();
									foreach (GridColumn column2 in frm.gv_CFL.Columns)
									{
										if (column2.FieldName == frm.grd_CFL.DataSourceTable.Columns[0].ColumnName)
										{
											column = column2;
											break;
										}
									}
									frm.gv_CFL.ClearSelection();
									string[] array3 = array2;
									foreach (string text7 in array3)
									{
										int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text7.Trim());
										frm.gv_CFL.SelectRow(rowHandle);
									}
								}));
							}
							frm.ShowDialog(this);
							string text6 = string.Empty;
							if (allowMultiple && frm.ChooseItems != null && frm.ChooseItems.Count > 0)
							{
								foreach (DataRow chooseItem in frm.ChooseItems)
								{
									text6 = text6 + chooseItem[0]?.ToString() + ",";
								}
							}
							else if (!allowMultiple && frm.ChooseItem != null)
							{
								text6 = text6 + frm.ChooseItem[0]?.ToString() + ",";
							}
							if (!string.IsNullOrEmpty(text6))
							{
								text6 = text6.Substring(0, text6.Length - 1);
								bte.Text = text6;
								bte.DoValidate();
							}
						}
						catch (Exception ex)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
							WinCommons.ErrorLineNumber(ex);
						}
					};
					bte.Properties.KeyDown += (object s, KeyEventArgs k) =>
					{
						if (k.KeyCode == Keys.Tab)
						{
							try
							{
								List<Common.ParameterValue> lstpr4 = new List<Common.ParameterValue>();
								string userProcedure4 = GetUserProcedure(subType, ref lstpr4);
								frm_CFL frm = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, allowMultiple, "", userProcedure4, Enums.ItemObjectType.NoItemType, lstpr4, "");
								if (allowMultiple)
								{
									frm.ButtonEditMultiSelect = allowMultiple;
									frm_CFL frm_CFL2 = frm;
									frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
									{
										string[] array2 = bte.Text.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
										GridColumn column = new GridColumn();
										foreach (GridColumn column3 in frm.gv_CFL.Columns)
										{
											if (column3.FieldName == frm.grd_CFL.DataSourceTable.Columns[0].ColumnName)
											{
												column = column3;
												break;
											}
										}
										frm.gv_CFL.ClearSelection();
										string[] array3 = array2;
										foreach (string text7 in array3)
										{
											int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text7.Trim());
											frm.gv_CFL.SelectRow(rowHandle);
										}
									}));
								}
								frm.ShowDialog(this);
								string text6 = string.Empty;
								if (allowMultiple && frm.ChooseItems != null && frm.ChooseItems.Count > 0)
								{
									foreach (DataRow chooseItem2 in frm.ChooseItems)
									{
										text6 = text6 + chooseItem2[0]?.ToString() + ",";
									}
								}
								else if (!allowMultiple && frm.ChooseItem != null)
								{
									text6 = text6 + frm.ChooseItem[0]?.ToString() + ",";
								}
								if (!string.IsNullOrEmpty(text6))
								{
									text6 = text6.Substring(0, text6.Length - 1);
									bte.Text = text6;
									bte.DoValidate();
								}
							}
							catch (Exception ex)
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
								WinCommons.ErrorLineNumber(ex);
							}
						}
					};
					bte.Properties.EditValueChanged += async (object s, EventArgs c) =>
					{
						Control.ControlCollection controls = xtr_Control.Controls;
						foreach (Control item in controls)
						{
							if (item.GetType().Name == "LookUpEdit")
							{
								LookUpEdit lke2 = (LookUpEdit)item;
								string text6 = lke2.Tag.ToString();
								if (text6.Contains(bte.Name))
								{
									List<Common.ParameterValue> lstpr4 = new List<Common.ParameterValue>();
									string userProcedure4 = GetUserProcedure(text6, ref lstpr4);
									DataSet data4 = (await _httpClient.Commons.ExecuteStore(userProcedure4, lstpr4)).Data;
									if (data4.IsNotNull() && data4.Tables[0].IsNotNull())
									{
										lke2.Properties.DataSource = data4.Tables[0];
										lke2.Properties.ValueMember = data4.Tables[0].Columns[0].ColumnName;
										lke2.Properties.DisplayMember = data4.Tables[0].Columns[0].ColumnName;
										if (data4.Tables[0].Columns.Count > 1)
										{
											lke2.Properties.DisplayMember = data4.Tables[0].Columns[1].ColumnName;
										}
									}
								}
							}
						}
					};
					bte.Location = new Point(controlX, controlY);
					xtr_Control.Controls.Add(bte);
					bte.FieldName = paraName.Substring(1);
					string df = Function.ToString(defaultValue);
					if (!string.IsNullOrEmpty(df) && df.StartsWith("@"))
					{
						List<Common.ParameterValue> lstpr3 = new List<Common.ParameterValue>();
						string userProcedure3 = GetUserProcedure(subType, ref lstpr3);
						DataSet data3 = (await _httpClient.Commons.ExecuteStore(userProcedure3, lstpr3)).Data;
						if (data3.IsHasStructure())
						{
							data3.Tables.RemoveAt(0);
						}
						if (!data3.IsNotNull() || !data3.Tables[0].IsNotNull())
						{
							break;
						}
						string text5 = df.ToLower();
						if (!(text5 == "@first"))
						{
							if (text5 == "@last")
							{
								bte.EditValue = data3.Tables[0].Rows[data3.Tables[0].Rows.Count - 1][0];
							}
						}
						else
						{
							bte.EditValue = data3.Tables[0].Rows[0][0];
						}
					}
					else
					{
						lstDfltValue.Add(bte, string.IsNullOrEmpty(df) ? string.Empty : df);
					}
					break;
				}
				case "validvalue":
				{
					LookUpEditEx lke = new LookUpEditEx();
					lke.Location = new Point(controlX, controlY);
					lke.Size = new Size(180, 20);
					lke.Properties.NullText = string.Empty;
					lke.Name = paraName;
					lke.ToolTip = toolTip;
					lke.Tag = subType;
					if (!visible)
					{
						lke.Visible = false;
					}
					if (!flag)
					{
						lke.Enabled = false;
					}
					lke.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					lke.TabIndex = tabOrder;
					tabOrder++;
					lke.Properties.Buttons.Clear();
					lke.Properties.Buttons.AddRange(new EditorButton[1]
					{
						new EditorButton(ButtonPredefines.Combo)
					});
					lke.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
					if (string.IsNullOrEmpty(subType) && paramValid.IsNotNull() && paramValid.IsExistsColumn("ColumnName"))
					{
						DataRow[] array = paramValid.Select($"ColumnName = '{paraName}'");
						if (array.IsNotNull())
						{
							lke.Properties.DataSource = array.CopyToDataTable();
							lke.Properties.ValueMember = "Value";
							lke.Properties.DisplayMember = "Description";
							lke.Properties.Columns.Add(new LookUpColumnInfo("Value", "Value"));
							lke.Properties.Columns.Add(new LookUpColumnInfo("Description", "Description"));
						}
					}
					else
					{
						List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>();
						string userProcedure = GetUserProcedure(subType, ref lstpr);
						DataSet data = (await _httpClient.Commons.ExecuteStore(userProcedure, lstpr)).Data;
						if (data.IsNotNull() && data.Tables[0].IsNotNull())
						{
							lke.Properties.DataSource = data.Tables[0];
							lke.Properties.ValueMember = data.Tables[0].Columns[0].ColumnName;
							lke.Properties.DisplayMember = data.Tables[0].Columns[0].ColumnName;
							if (data.Tables[0].Columns.Count > 1)
							{
								lke.Properties.DisplayMember = data.Tables[0].Columns[1].ColumnName;
							}
						}
					}
					lke.Properties.EditValueChanged += async (object s, EventArgs v) =>
					{
						Control.ControlCollection controls = xtr_Control.Controls;
						foreach (Control item2 in controls)
						{
							switch (item2.GetType().Name)
							{
							case "ButtonEdit":
							{
								ButtonEdit buttonEdit = (ButtonEdit)item2;
								if (buttonEdit.Tag.ToString().Contains(lke.Name))
								{
									buttonEdit.Tag = buttonEdit.Tag.ToString().Replace(lke.Name, Function.ToString(lke.EditValue));
								}
								break;
							}
							case "ButtonEditEx":
							{
								ButtonEditEx buttonEditEx = (ButtonEditEx)item2;
								if (buttonEditEx.Tag.ToString().Contains(lke.Name))
								{
									buttonEditEx.Tag = buttonEditEx.Tag.ToString().Replace(lke.Name, Function.ToString(lke.EditValue));
								}
								break;
							}
							case "LookUpEdit":
							{
								LookUpEdit lke2 = (LookUpEdit)item2;
								string text7 = lke2.Tag.ToString();
								if (text7.Contains(lke.Name))
								{
									lke2.Tag = text7.Replace(lke.Name, Function.ToString(lke.EditValue));
									List<Common.ParameterValue> lstpr5 = new List<Common.ParameterValue>();
									string userProcedure5 = GetUserProcedure(lke2.Tag.ToString(), ref lstpr5);
									DataSet data5 = (await _httpClient.Commons.ExecuteStore(userProcedure5, lstpr5)).Data;
									if (data5.IsNotNull())
									{
										lke2.Properties.DataSource = data5;
										lke2.Properties.ValueMember = data5.Tables[0].Columns[0].ColumnName;
										lke2.Properties.DisplayMember = data5.Tables[0].Columns[0].ColumnName;
										if (data5.Tables[0].Columns.Count > 1)
										{
											lke2.Properties.DisplayMember = data5.Tables[0].Columns[1].ColumnName;
										}
									}
									data5.DisposeDataSet();
								}
								break;
							}
							case "LookUpEditEx":
							{
								LookUpEditEx lkeEx = (LookUpEditEx)item2;
								string text6 = lkeEx.Tag.ToString();
								if (text6.Contains(lke.Name))
								{
									lkeEx.Tag = text6.Replace(lke.Name, Function.ToString(lke.EditValue));
									List<Common.ParameterValue> lstpr4 = new List<Common.ParameterValue>();
									string userProcedure4 = GetUserProcedure(lkeEx.Tag.ToString(), ref lstpr4);
									DataSet data4 = (await _httpClient.Commons.ExecuteStore(userProcedure4, lstpr4)).Data;
									if (data4.IsNotNull())
									{
										lkeEx.Properties.DataSource = data4;
										lkeEx.Properties.ValueMember = data4.Tables[0].Columns[0].ColumnName;
										lkeEx.Properties.DisplayMember = data4.Tables[0].Columns[0].ColumnName;
										if (data4.Tables[0].Columns.Count > 1)
										{
											lkeEx.Properties.DisplayMember = data4.Tables[0].Columns[1].ColumnName;
										}
									}
									data4.DisposeDataSet();
								}
								break;
							}
							}
						}
					};
					lke.Location = new Point(controlX, controlY);
					xtr_Control.Controls.Add(lke);
					lke.FieldName = paraName.Substring(1);
					string dfVal = Function.ToString(defaultValue);
					if (!string.IsNullOrEmpty(dfVal) && dfVal.StartsWith("@"))
					{
						List<Common.ParameterValue> lstpr2 = new List<Common.ParameterValue>();
						string userProcedure2 = GetUserProcedure(subType, ref lstpr2);
						DataSet data2 = (await _httpClient.Commons.ExecuteStore(userProcedure2, lstpr2)).Data;
						if (data2.IsHasStructure())
						{
							data2.Tables.RemoveAt(0);
						}
						if (!data2.IsNotNull() || !data2.Tables[0].IsNotNull())
						{
							break;
						}
						string text5 = dfVal.ToLower();
						if (!(text5 == "@first"))
						{
							if (text5 == "@last")
							{
								lke.EditValue = data2.Tables[0].Rows[data2.Tables[0].Rows.Count - 1][0];
							}
						}
						else
						{
							lke.EditValue = data2.Tables[0].Rows[0][0];
						}
					}
					else
					{
						lstDfltValue.Add(lke, string.IsNullOrEmpty(dfVal) ? string.Empty : dfVal);
					}
					break;
				}
				case "barcode":
				{
					TextEditEx txt_BarCode = new TextEditEx();
					txt_BarCode.Location = new Point(controlX, controlY);
					txt_BarCode.Size = new Size(180, 20);
					txt_BarCode.Name = paraName;
					txt_BarCode.Tag = subType;
					txt_BarCode.ToolTip = toolTip;
					txt_BarCode.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					txt_BarCode.Text = string.Empty;
					txt_BarCode.TabIndex = tabOrder;
					tabOrder++;
					lstDfltValue.Add(txt_BarCode, string.IsNullOrEmpty(Function.ToString(defaultValue)) ? string.Empty : Function.ToString(defaultValue));
					if (!visible)
					{
						txt_BarCode.Visible = false;
					}
					if (!flag)
					{
						txt_BarCode.Enabled = false;
					}
					txt_BarCode.KeyDown += async (object s, KeyEventArgs e) =>
					{
						if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_BarCode.Text))
						{
							txt_BarCode.Enabled = false;
							await Execute();
							txt_BarCode.Enabled = true;
							txt_BarCode.Text = string.Empty;
							txt_BarCode.Focus();
						}
					};
					xtr_Control.Controls.Add(txt_BarCode);
					txt_BarCode.FieldName = paraName.Substring(1);
					AcceptButton = null;
					break;
				}
				default:
				{
					TextEditEx textEditEx2 = new TextEditEx();
					textEditEx2.Location = new Point(controlX, controlY);
					textEditEx2.Size = new Size(180, 20);
					textEditEx2.Name = paraName;
					textEditEx2.Tag = subType;
					textEditEx2.ToolTip = toolTip;
					textEditEx2.Anchor = AnchorStyles.Top | AnchorStyles.Left;
					textEditEx2.Text = string.Empty;
					textEditEx2.TabIndex = tabOrder;
					tabOrder++;
					lstDfltValue.Add(textEditEx2, string.IsNullOrEmpty(Function.ToString(defaultValue)) ? string.Empty : Function.ToString(defaultValue));
					if (!visible)
					{
						textEditEx2.Visible = false;
					}
					if (!flag)
					{
						textEditEx2.Enabled = false;
					}
					xtr_Control.Controls.Add(textEditEx2);
					textEditEx2.FieldName = paraName.Substring(1);
					break;
				}
				}
			}
			if (visible)
			{
				if (visibleIndex % 2 == 0)
				{
					labelX += 400;
					controlX += 420;
					panelHeight += 25;
				}
				else if (visibleIndex % 2 > 0 && visibleIndex > 0)
				{
					labelY += 25;
					controlY += 25;
					labelX -= 400;
					controlX -= 420;
					panelHeight = controlY;
				}
				visibleIndex++;
			}
		}
		foreach (IEditControlEx key in lstDfltValue.Keys)
		{
			key.EditValue = lstDfltValue[key];
		}
		splitContainerControl1.SplitterPosition = panelHeight + 10;
		if (!dt.AsEnumerable().Any((DataRow m) => Function.ParseBool(m["Visible"])))
		{
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel2;
		}
		if ((isExecuteWhenLoad || defaultParamValue != null) | isFullDfValue)
		{
			await Execute();
		}
	}

	private async Task SaveFormSetting()
	{
		WinCommons._OpenProcessing();
		try
		{
			btn_Execute.Enabled = false;
			btn_SaveFormSetting.Enabled = false;
			DataSet ds = new DataSet();
			DataTable dataTable = ((DataTable)lue_FormSetting.Properties.DataSource).Select("ID = " + lue_FormSetting.EditValue).CopyToDataTable();
			new DataTable();
			DataTable dataTable2 = defaultValid.Copy();
			if (defaultStructure.IsNotNull())
			{
				if (_tableType.Equals("1"))
				{
					int num = gv_Data.Columns.Max((GridColumn m) => m.VisibleIndex) + 1;
					foreach (GridColumnEx column in gv_Data.Columns)
					{
						string colName = column.FieldName;
						DataRow dataRow = (from c in defaultStructure.AsEnumerable()
							where c.Field<string>("ColumnName").Equals(colName)
							select c).FirstOrDefault();
						if (dataRow != null)
						{
							dataRow["Caption"] = column.Caption;
							dataRow["Width"] = column.Width;
							dataRow["GroupIndex"] = column.GroupIndex;
							dataRow["Visible"] = column.Visible;
							dataRow["VisIndex"] = column.VisibleIndex;
							dataRow["ReadOnly"] = column.OptionsColumn.ReadOnly;
							if (!column.Visible)
							{
								dataRow["VisIndex"] = num;
								num++;
							}
						}
					}
					defaultStructure.Rows[0]["FilterString"] = gv_Data.ActiveFilterString;
					DataView defaultView = defaultStructure.DefaultView;
					defaultStructure.DefaultView.Sort = "VisIndex ASC";
					defaultStructure = defaultView.ToTable("Structure");
				}
				else if (_tableType.Equals("2"))
				{
					int num2 = _tree.Columns.Max((TreeListColumn m) => m.VisibleIndex) + 1;
					foreach (TreeListColumn column2 in _tree.Columns)
					{
						string colName2 = column2.FieldName;
						DataRow dataRow2 = (from c in defaultStructure.AsEnumerable()
							where c.Field<string>("ColumnName").Equals(colName2)
							select c).FirstOrDefault();
						if (dataRow2 != null)
						{
							dataRow2["Caption"] = column2.Caption;
							dataRow2["Width"] = column2.Width;
							dataRow2["Visible"] = column2.Visible;
							dataRow2["VisIndex"] = column2.VisibleIndex;
							if (!column2.Visible)
							{
								dataRow2["VisIndex"] = num2;
								num2++;
							}
						}
					}
					defaultStructure.Rows[0]["FilterString"] = gv_Data.ActiveFilterString;
					DataView defaultView2 = defaultStructure.DefaultView;
					defaultStructure.DefaultView.Sort = "VisIndex ASC";
					defaultStructure = defaultView2.ToTable("Structure");
				}
				DataTable dataTable3 = defaultStructure.Copy();
				dataTable.Rows[0]["Usersign2"] = GlobalData.UserName;
				dataTable.TableName = "OFST";
				dataTable3.TableName = "FST1";
				dataTable2.TableName = "FST2";
				ds.Tables.Add(dataTable);
				ds.Tables.Add(dataTable3);
				ds.Tables.Add(dataTable2);
				new HttpResult();
				HttpResult httpResult = await _httpClient.FormSettingClient.Save(ds);
				ds.DisposeDataSet();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotLoadStructure, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_Execute.Enabled = true;
			btn_SaveFormSetting.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	public string GetColumnType(Type dataType)
	{
		if (dataType == typeof(string))
		{
			return "nvarchar";
		}
		if (dataType == typeof(int))
		{
			return "int";
		}
		if (dataType == typeof(decimal))
		{
			return "numeric";
		}
		if (dataType == typeof(DateTime))
		{
			return "datetime";
		}
		if (dataType == typeof(bool))
		{
			return "bit";
		}
		return "nvarchar";
	}

	private async Task<DataTable> GetParamStore(string storeName)
	{
		DataTable dt = new DataTable();
		try
		{
			DataSetResult dataSetResult = await _httpClient.Udr.GetParamByStore(storeName, ItemId, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				return dt;
			}
			DataSet data = dataSetResult.Data;
			if (data.IsHasStructure())
			{
				dt = data.Tables[1].Copy();
				if (this.dataRow != null && !string.IsNullOrWhiteSpace(mappingParams))
				{
					string[] array = mappingParams.Split(',');
					defaultParamValue = new List<Common.ParameterValue>();
					for (int i = 0; i < dt.Rows.Count && i != array.Length; i++)
					{
						if (array[i].StartsWith("@"))
						{
							string text = array[i].Substring(1);
							if (this.dataRow.IsExistsColumn(text))
							{
								object value = this.dataRow[text];
								defaultParamValue.Add(new Common.ParameterValue(Function.ToString(dt.Rows[i]["ParamName"]), value));
							}
						}
						else
						{
							object value = ((!array[i].StartsWith("'") || !array[i].EndsWith("'")) ? array[i] : array[i].Substring(1, array[i].Length - 2));
							if (Function.ToString(value) != "")
							{
								defaultParamValue.Add(new Common.ParameterValue(Function.ToString(dt.Rows[i]["ParamName"]), value));
							}
						}
					}
				}
				if (dt.IsNotNull() && dt.Rows.Count > 0)
				{
					dt.DefaultView.Sort = "LineNum ASC";
					dt = dt.DefaultView.ToTable();
				}
				_lstParamValue = new List<Common.ParameterValue>();
				foreach (DataRow row in data.Tables[1].Rows)
				{
					_lstParamValue.Add(new Common.ParameterValue(Function.ToString(row["ParamName"])));
				}
				if (data.Tables.Count > 3)
				{
					paramValid = data.Tables[3].Copy();
				}
			}
			data.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private async Task<DataTable> GetStoreName(string code)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			dt = await _helper.GetDataTableAsync("GetStoreName", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private async Task GetListFormSetting()
	{
		try
		{
			isLoadForm = true;
			DataTable dataTable = await _httpClient.FormSettingClient.GetListFormSetting(FormId, ItemId, GlobalData.UserName);
			if (dataTable.IsNotNull())
			{
				lue_FormSetting.Properties.Columns.Clear();
				lue_FormSetting.Properties.DataSource = null;
				lue_FormSetting.Properties.DataSource = dataTable;
				lue_FormSetting.Properties.DisplayMember = "Name";
				lue_FormSetting.Properties.ValueMember = "ID";
				lue_FormSetting.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				lue_FormSetting.Properties.PopulateColumns();
				for (int i = 0; i < lue_FormSetting.Properties.Columns.Count; i++)
				{
					if (!lue_FormSetting.Properties.Columns[i].FieldName.Equals("Name"))
					{
						lue_FormSetting.Properties.Columns[i].Visible = false;
					}
				}
				chk_SetDefault.Visible = true;
				btn_SaveFormSetting.Visible = true;
				lbl_FormSetting.Visible = true;
				lue_FormSetting.Visible = true;
			}
			else
			{
				chk_SetDefault.Visible = false;
				btn_SaveFormSetting.Visible = false;
				lbl_FormSetting.Visible = false;
				lue_FormSetting.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isLoadForm = false;
	}

	private object getDfValue(string paraname, object value)
	{
		switch (Function.ToString(value).ToLower())
		{
		case "@userid":
			value = GlobalData.UserID;
			break;
		case "@usercode":
			value = GlobalData.UserName;
			break;
		case "@branch":
			value = GlobalData.DefaultBranch.ToString();
			break;
		case "@firstdaymonth":
			value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd");
			break;
		case "@lastdaymonth":
			value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1.0).ToString("yyyy-MM-dd");
			break;
		}
		return value;
	}

	private string GetUserProcedure(string subType, ref List<Common.ParameterValue> lstpr)
	{
		string text = subType;
		string[] array = text.Split(' ');
		if (array.Length > 1 && array[1].StartsWith("@"))
		{
			text = array[0];
			string[] array2 = array[1].Split(',');
			for (int i = 0; i < array2.Length; i++)
			{
				string text2 = array2[i];
				Control.ControlCollection controls = xtr_Control.Controls;
				if (array2[i].Contains("="))
				{
					int num = array2[i].IndexOf('=');
					lstpr.Add(new Common.ParameterValue(array2[i].Substring(0, num), array2[i].Substring(num + 1).Replace("'", "")));
				}
				else if (text2.ToLower().Contains("username") || text2.ToLower().Contains("usercode"))
				{
					lstpr.Add(new Common.ParameterValue(array2[i], GlobalData.UserName));
				}
				else if (text2.ToLower().Contains("branch"))
				{
					lstpr.Add(new Common.ParameterValue(array2[i], GlobalData.DefaultBranch));
				}
				else if (controls.ContainsKey(text2))
				{
					int index = controls.IndexOfKey(text2);
					dynamic val = ((dynamic)controls[index]).EditValue;
					lstpr.Add(new Common.ParameterValue(array2[i], val));
				}
				else
				{
					lstpr.Add(new Common.ParameterValue(array2[i], array2[i]));
				}
			}
		}
		return text;
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			if (string.IsNullOrEmpty(_fileName))
			{
				return;
			}
			DataTable mailMergeDataSource = new DataTable();
			if (_tableType.Equals("2") && _tree != null)
			{
				mailMergeDataSource = _tree.DataSourceTable();
			}
			else if (_tableType.Equals("3") && _pivot != null)
			{
				mailMergeDataSource = _pivot.DataSourceTable();
			}
			else if (grd_Data != null)
			{
				mailMergeDataSource = grd_Data.DataSourceTable();
			}
			string path = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + _fileName;
			if (Path.GetExtension(path).ToLower().Contains("xls") && File.Exists(path))
			{
				WinCommons._OpenProcessing();
				Workbook workbook = new Workbook();
				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
				}
				workbook.MailMergeDataSource = mailMergeDataSource;
				foreach (IWorkbook item in workbook.GenerateMailMergeDocuments())
				{
					foreach (Worksheet worksheet in item.Worksheets)
					{
						if (!item.Worksheets.Contains(worksheet.Name))
						{
							item.Worksheets.Add(worksheet.Name);
							item.Worksheets[worksheet.Name].CopyFrom(worksheet);
						}
					}
					PrintableComponentLink printableComponentLink = new PrintableComponentLink(new PrintingSystem());
					printableComponentLink.Component = item;
					printableComponentLink.CreateDocument();
					printableComponentLink.ShowPreview();
				}
				WinCommons._CloseProcessing();
				return;
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			foreach (Control control in xtr_Control.Controls)
			{
				Type type = control.GetType();
				if (!type.Name.Equals("LabelControl"))
				{
					switch (type.Name)
					{
					case "CalcEdit":
					{
						CalcEdit calcEdit = (CalcEdit)control;
						Common.ParameterValue parameterValue7 = new Common.ParameterValue();
						parameterValue7.Value = Function.ToString(calcEdit.EditValue);
						parameterValue7.Name = (calcEdit.Name.StartsWith("@") ? calcEdit.Name.Replace("@", string.Empty) : calcEdit.Name);
						list.Add(parameterValue7);
						break;
					}
					case "CheckEdit":
					{
						CheckEdit checkEdit = (CheckEdit)control;
						Common.ParameterValue parameterValue2 = new Common.ParameterValue();
						parameterValue2.Value = Function.ParseBool(checkEdit.Checked);
						parameterValue2.Name = (checkEdit.Name.StartsWith("@") ? checkEdit.Name.Replace("@", string.Empty) : checkEdit.Name);
						list.Add(parameterValue2);
						break;
					}
					case "DateEditEx":
					{
						DateEditEx dateEditEx2 = (DateEditEx)control;
						Common.ParameterValue parameterValue8 = new Common.ParameterValue();
						parameterValue8.Value = Function.ParseDateTime(dateEditEx2.EditValue);
						parameterValue8.Name = (dateEditEx2.Name.StartsWith("@") ? dateEditEx2.Name.Replace("@", string.Empty) : dateEditEx2.Name);
						list.Add(parameterValue8);
						break;
					}
					case "TimeEditEx":
					{
						TimeEditEx timeEditEx = (TimeEditEx)control;
						Common.ParameterValue parameterValue3 = new Common.ParameterValue();
						parameterValue3.Value = Function.ParseDateTime(timeEditEx.EditValue);
						parameterValue3.Name = (timeEditEx.Name.StartsWith("@") ? timeEditEx.Name.Replace("@", string.Empty) : timeEditEx.Name);
						list.Add(parameterValue3);
						break;
					}
					case "DateTimePicker":
					{
						DateEditEx dateEditEx = (DateEditEx)control;
						Common.ParameterValue parameterValue6 = new Common.ParameterValue();
						parameterValue6.Value = Function.ParseDateTime(dateEditEx.EditValue);
						parameterValue6.Name = (dateEditEx.Name.StartsWith("@") ? dateEditEx.Name.Replace("@", string.Empty) : dateEditEx.Name);
						list.Add(parameterValue6);
						break;
					}
					case "LookUpEdit":
					{
						LookUpEdit lookUpEdit = (LookUpEdit)control;
						Common.ParameterValue parameterValue4 = new Common.ParameterValue();
						parameterValue4.Value = Function.ToString(lookUpEdit.EditValue);
						parameterValue4.Name = (lookUpEdit.Name.StartsWith("@") ? lookUpEdit.Name.Replace("@", string.Empty) : lookUpEdit.Name);
						list.Add(parameterValue4);
						break;
					}
					case "ButtonEdit":
					{
						ButtonEdit buttonEdit = (ButtonEdit)control;
						Common.ParameterValue parameterValue5 = new Common.ParameterValue();
						parameterValue5.Value = Function.ToString(buttonEdit.EditValue);
						parameterValue5.Name = (buttonEdit.Name.StartsWith("@") ? buttonEdit.Name.Replace("@", string.Empty) : buttonEdit.Name);
						list.Add(parameterValue5);
						break;
					}
					case "TextEdit":
					{
						TextEdit textEdit = (TextEdit)control;
						Common.ParameterValue parameterValue = new Common.ParameterValue();
						parameterValue.Value = Function.ToString(textEdit.EditValue);
						parameterValue.Name = (textEdit.Name.StartsWith("@") ? textEdit.Name.Replace("@", string.Empty) : textEdit.Name);
						list.Add(parameterValue);
						break;
					}
					}
				}
			}
			if (list.Count > 0)
			{
				string text = await WinFuntions.ReportViewerListParam(Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + _fileName, list);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			if (string.IsNullOrEmpty(_fileName))
			{
				return;
			}
			DataTable mailMergeDataSource = new DataTable();
			if (_tableType.Equals("2") && _tree != null)
			{
				mailMergeDataSource = _tree.DataSourceTable();
			}
			else if (_tableType.Equals("3") && _pivot != null)
			{
				mailMergeDataSource = _pivot.DataSourceTable();
			}
			else if (grd_Data != null)
			{
				mailMergeDataSource = grd_Data.DataSourceTable();
			}
			string path = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + _fileName;
			if (Path.GetExtension(path).ToLower().Contains("xls") && File.Exists(path))
			{
				WinCommons._OpenProcessing();
				Workbook workbook = new Workbook();
				using (FileStream stream = new FileStream(path, FileMode.Open))
				{
					workbook.LoadDocument(stream, DevExpress.Spreadsheet.DocumentFormat.Xlsx);
				}
				workbook.MailMergeDataSource = mailMergeDataSource;
				foreach (IWorkbook item in workbook.GenerateMailMergeDocuments())
				{
					foreach (Worksheet worksheet in item.Worksheets)
					{
						if (!item.Worksheets.Contains(worksheet.Name))
						{
							item.Worksheets.Add(worksheet.Name);
							item.Worksheets[worksheet.Name].CopyFrom(worksheet);
						}
					}
					PrintableComponentLink printableComponentLink = new PrintableComponentLink(new PrintingSystem());
					printableComponentLink.Component = item;
					printableComponentLink.CreateDocument();
					printableComponentLink.ShowPreview();
				}
				WinCommons._CloseProcessing();
				return;
			}
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			foreach (Control control in xtr_Control.Controls)
			{
				Type type = control.GetType();
				if (!type.Name.Equals("LabelControl"))
				{
					switch (type.Name)
					{
					case "CalcEdit":
					{
						CalcEdit calcEdit = (CalcEdit)control;
						Common.ParameterValue parameterValue7 = new Common.ParameterValue();
						parameterValue7.Value = Function.ToString(calcEdit.EditValue);
						parameterValue7.Name = (calcEdit.Name.StartsWith("@") ? calcEdit.Name.Replace("@", string.Empty) : calcEdit.Name);
						list.Add(parameterValue7);
						break;
					}
					case "CheckEdit":
					{
						CheckEdit checkEdit = (CheckEdit)control;
						Common.ParameterValue parameterValue2 = new Common.ParameterValue();
						parameterValue2.Value = Function.ParseBool(checkEdit.Checked);
						parameterValue2.Name = (checkEdit.Name.StartsWith("@") ? checkEdit.Name.Replace("@", string.Empty) : checkEdit.Name);
						list.Add(parameterValue2);
						break;
					}
					case "DateEditEx":
					{
						DateEditEx dateEditEx2 = (DateEditEx)control;
						Common.ParameterValue parameterValue8 = new Common.ParameterValue();
						parameterValue8.Value = Function.ParseDateTime(dateEditEx2.EditValue);
						parameterValue8.Name = (dateEditEx2.Name.StartsWith("@") ? dateEditEx2.Name.Replace("@", string.Empty) : dateEditEx2.Name);
						list.Add(parameterValue8);
						break;
					}
					case "TimeEditEx":
					{
						TimeEditEx timeEditEx = (TimeEditEx)control;
						Common.ParameterValue parameterValue3 = new Common.ParameterValue();
						parameterValue3.Value = Function.ParseDateTime(timeEditEx.EditValue);
						parameterValue3.Name = (timeEditEx.Name.StartsWith("@") ? timeEditEx.Name.Replace("@", string.Empty) : timeEditEx.Name);
						list.Add(parameterValue3);
						break;
					}
					case "DateTimePicker":
					{
						DateEditEx dateEditEx = (DateEditEx)control;
						Common.ParameterValue parameterValue6 = new Common.ParameterValue();
						parameterValue6.Value = Function.ParseDateTime(dateEditEx.EditValue);
						parameterValue6.Name = (dateEditEx.Name.StartsWith("@") ? dateEditEx.Name.Replace("@", string.Empty) : dateEditEx.Name);
						list.Add(parameterValue6);
						break;
					}
					case "LookUpEdit":
					{
						LookUpEdit lookUpEdit = (LookUpEdit)control;
						Common.ParameterValue parameterValue4 = new Common.ParameterValue();
						parameterValue4.Value = Function.ToString(lookUpEdit.EditValue);
						parameterValue4.Name = (lookUpEdit.Name.StartsWith("@") ? lookUpEdit.Name.Replace("@", string.Empty) : lookUpEdit.Name);
						list.Add(parameterValue4);
						break;
					}
					case "ButtonEdit":
					{
						ButtonEdit buttonEdit = (ButtonEdit)control;
						Common.ParameterValue parameterValue5 = new Common.ParameterValue();
						parameterValue5.Value = Function.ToString(buttonEdit.EditValue);
						parameterValue5.Name = (buttonEdit.Name.StartsWith("@") ? buttonEdit.Name.Replace("@", string.Empty) : buttonEdit.Name);
						list.Add(parameterValue5);
						break;
					}
					case "TextEdit":
					{
						TextEdit textEdit = (TextEdit)control;
						Common.ParameterValue parameterValue = new Common.ParameterValue();
						parameterValue.Value = Function.ToString(textEdit.EditValue);
						parameterValue.Name = (textEdit.Name.StartsWith("@") ? textEdit.Name.Replace("@", string.Empty) : textEdit.Name);
						list.Add(parameterValue);
						break;
					}
					}
				}
			}
			if (list.Count > 0)
			{
				string text = await WinFuntions.ReportViewerListParam(Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\" + _fileName, list);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void FORMSETTING()
	{
		try
		{
			foreach (Form openForm in Application.OpenForms)
			{
				if (!openForm.IsDisposed && !openForm.Disposing && openForm.GetType() == typeof(frm_ReportFormSettings) && openForm.Text.Equals(string.Format("{0} - {1}", "Form Settings", Text)))
				{
					openForm.Activate();
					return;
				}
			}
			if ((_tableType.Equals("1") && grd_Data != null && grd_Data.DatasourceTable().Columns.Count > 0) || (_tableType.Equals("2") && _tree != null && _tree.DataSourceTable().Columns.Count > 0) || (_tableType.Equals("3") && _pivot != null && _pivot.DataSourceTable().Columns.Count > 0))
			{
				WinCommons._OpenProcessing();
				frm_ReportFormSettings frm = new frm_ReportFormSettings(_tableType, grd_Data, _tree, ItemId, ApzObjectTypes.oUserDefinedReports.ToString(), structureId, defaultStructure, defaultValid);
				frm.pivot = _pivot;
				frm.Text = string.Format("{0} - {1}", "Form Settings", Text);
				frm.MdiParent = MdiParent;
				frm.WindowState = FormWindowState.Normal;
				frm.StartPosition = FormStartPosition.CenterScreen;
				frm.FormClosed += async (object s, FormClosedEventArgs v) =>
				{
					if (frm.isSave)
					{
						isNotChangeSetting = true;
						defaultStructure = frm.dtStructure;
						structureId = frm.id;
						defaultValid = frm.dtValidValue;
						await GetListFormSetting();
						lue_FormSetting.EditValue = structureId;
						isNotChangeSetting = false;
					}
				};
				frm.FormClosed += (object s, FormClosedEventArgs e) =>
				{
					lstFormOpen.Remove(frm.Text);
				};
				frm.Show();
				lstFormOpen.Add(frm.Text);
				WinCommons._CloseProcessing();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.DataIsEmpty, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetData(string sql)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await http.PostAsync("GetDataParamaterSql", sql);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private void expandCollapseTree_Click(object sender, EventArgs e)
	{
		try
		{
			string name = (sender as ToolStripMenuItem).Name;
			if (_tree.DataSource == null)
			{
				return;
			}
			if (!(name == "TreeList_Expand_All"))
			{
				if (name == "TreeList_Collapse_All")
				{
					_tree.CollapseAll();
				}
			}
			else
			{
				_tree.ExpandAll();
			}
		}
		catch (Exception)
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ReportForm));
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Execute = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grp_Control = new DevExpress.XtraEditors.GroupControl();
		this.xtr_Control = new DevExpress.XtraEditors.XtraScrollableControl();
		this.chk_SetDefault = new System.Windows.Forms.CheckBox();
		this.lbl_FormSetting = new Apzon.Controls.Controls.LabelLine();
		this.lue_FormSetting = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_SaveFormSetting = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_getImg = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grp_Control).BeginInit();
		this.grp_Control.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lue_FormSetting.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Location = new System.Drawing.Point(0, 0);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(882, 323);
		this.grd_Data.TabIndex = 1;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oUserDefinedReports";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsSelection.MultiSelect = true;
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		this.btn_Execute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Execute.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Execute.Appearance.Options.UseBackColor = true;
		this.btn_Execute.FieldName = null;
		this.btn_Execute.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Execute.ImageOptions.Image");
		this.btn_Execute.IsSearchControl = false;
		this.btn_Execute.Location = new System.Drawing.Point(5, 487);
		this.btn_Execute.Name = "btn_Execute";
		this.btn_Execute.Size = new System.Drawing.Size(75, 20);
		this.btn_Execute.TabIndex = 2;
		this.btn_Execute.Text = "Execute";
		this.btn_Execute.Click += new System.EventHandler(this.bt_Execute_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 487);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grp_Control);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Data);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(882, 481);
		this.splitContainerControl1.SplitterPosition = 153;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grp_Control.Controls.Add(this.xtr_Control);
		this.grp_Control.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grp_Control.Location = new System.Drawing.Point(0, 0);
		this.grp_Control.Name = "grp_Control";
		this.grp_Control.ShowCaption = false;
		this.grp_Control.Size = new System.Drawing.Size(882, 153);
		this.grp_Control.TabIndex = 0;
		this.grp_Control.Text = "Search";
		this.xtr_Control.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtr_Control.Location = new System.Drawing.Point(2, 2);
		this.xtr_Control.Name = "xtr_Control";
		this.xtr_Control.Size = new System.Drawing.Size(878, 149);
		this.xtr_Control.TabIndex = 0;
		this.chk_SetDefault.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.chk_SetDefault.AutoSize = true;
		this.chk_SetDefault.Location = new System.Drawing.Point(642, 490);
		this.chk_SetDefault.Name = "chk_SetDefault";
		this.chk_SetDefault.Size = new System.Drawing.Size(94, 17);
		this.chk_SetDefault.TabIndex = 38;
		this.chk_SetDefault.Text = "Set as Default";
		this.chk_SetDefault.UseVisualStyleBackColor = true;
		this.chk_SetDefault.CheckedChanged += new System.EventHandler(this.chk_SetDefault_CheckedChanged);
		this.lbl_FormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_FormSetting.FieldName = null;
		this.lbl_FormSetting.HideWhenEmpty = false;
		this.lbl_FormSetting.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FormSetting.Location = new System.Drawing.Point(358, 490);
		this.lbl_FormSetting.Name = "lbl_FormSetting";
		this.lbl_FormSetting.ShowUnderLine = true;
		this.lbl_FormSetting.Size = new System.Drawing.Size(77, 20);
		this.lbl_FormSetting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FormSetting.TabIndex = 36;
		this.lbl_FormSetting.Utext = "Form Setting";
		this.lue_FormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lue_FormSetting.Location = new System.Drawing.Point(431, 488);
		this.lue_FormSetting.Name = "lue_FormSetting";
		this.lue_FormSetting.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_FormSetting.Properties.NullText = "";
		this.lue_FormSetting.Size = new System.Drawing.Size(172, 20);
		this.lue_FormSetting.TabIndex = 37;
		this.lue_FormSetting.EditValueChanged += new System.EventHandler(this.lue_FormSetting_EditValueChanged);
		this.btn_SaveFormSetting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SaveFormSetting.FieldName = null;
		this.btn_SaveFormSetting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SaveFormSetting.ImageOptions.Image");
		this.btn_SaveFormSetting.IsSearchControl = false;
		this.btn_SaveFormSetting.Location = new System.Drawing.Point(609, 486);
		this.btn_SaveFormSetting.Name = "btn_SaveFormSetting";
		this.btn_SaveFormSetting.Size = new System.Drawing.Size(27, 23);
		this.btn_SaveFormSetting.TabIndex = 39;
		this.btn_SaveFormSetting.ToolTip = "Save Form Setting";
		this.btn_SaveFormSetting.Click += new System.EventHandler(this.btn_SaveFormSetting_Click);
		this.btn_getImg.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_getImg.FieldName = null;
		this.btn_getImg.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_getImg.ImageOptions.Image");
		this.btn_getImg.IsSearchControl = false;
		this.btn_getImg.Location = new System.Drawing.Point(164, 487);
		this.btn_getImg.Name = "btn_getImg";
		this.btn_getImg.Size = new System.Drawing.Size(27, 23);
		this.btn_getImg.TabIndex = 40;
		this.btn_getImg.ToolTip = "Save Form Setting";
		this.btn_getImg.Visible = false;
		base.AcceptButton = this.btn_Execute;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(884, 512);
		base.Controls.Add(this.btn_getImg);
		base.Controls.Add(this.btn_SaveFormSetting);
		base.Controls.Add(this.chk_SetDefault);
		base.Controls.Add(this.lue_FormSetting);
		base.Controls.Add(this.lbl_FormSetting);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Execute);
		base.FormId = "oUserDefinedReports";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReportForm";
		base.ShowInTaskbar = false;
		this.ShowMdiChildCaptionInParentTitle = true;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Report Form";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_ReportForm_OnLoadAsync);
		base.Activated += new System.EventHandler(this.frm_ReportForm_Activated);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ReportForm_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grp_Control).EndInit();
		this.grp_Control.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lue_FormSetting.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
