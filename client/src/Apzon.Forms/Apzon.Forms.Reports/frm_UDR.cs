using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraWizard;

namespace Apzon.Forms.Reports;

public class frm_UDR : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Enums.ImportDOREnum _currentAction;

	private GridHitInfo downHitInfo;

	private DataTable dtValidValue = new DataTable();

	public RefreshMain RefreshMain;

	private readonly string _nextText;

	private bool _actionSuccess;

	private IContainer components;

	private WizardControl wiz_UDR;

	private WelcomeWizardPage wp_Welcome;

	private CompletionWizardPage wp_Complete;

	private GridControlEx grd_ListReport;

	private GridViewEx gv_ListReport;

	private RadioGroupEx rdg_OperationType;

	private DevExpress.XtraWizard.WizardPage wp_Setting;

	private TextEditEx txt_TableName;

	private TextEditEx txt_Name;

	private TextEditEx txt_Code;

	private DevExpress.XtraWizard.WizardPage wp_StoreParam;

	private CheckEditEx chk_PlaceBottom;

	private TextEditEx txt_Position;

	private TextEditEx txt_MenuUID;

	private TextEditEx txt_Caption;

	private CheckEditEx chk_MenuItem;

	private DevExpress.XtraWizard.WizardPage wp_File;

	private PanelControl panelControl1;

	private CheckEditEx chk_Delete;

	private CheckEditEx chk_Cancel;

	private CheckEditEx chk_Find;

	private CheckEditEx chk_Update;

	private CheckEditEx chk_Add;

	private CheckEditEx chk_Log;

	private TextEditEx txt_LogTable;

	private CheckEditEx chk_Close;

	private LookUpEdit cbx_Type;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine labelLine8;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine5;

	private LabelLine labelLine10;

	private ButtonEditEx txt_FileName;

	private TextEditEx txt_FilePath;

	private LabelLine labelLine11;

	private DevExpress.XtraWizard.WizardPage wp_Operation;

	private ButtonEditCFL bte_ParentMenuId;

	private SimpleButtonEx btn_Import;

	private SimpleButtonEx btn_Export;

	private CheckEditEx check_allRows;

	private CheckEditEx check_ShowChk;

	private SplitContainerControl splitContainerControl1;

	private LabelLine labelLine12;

	private LookUpEdit cbx_QueryType;

	private SimpleButtonEx btn_EditValidValue;

	private LabelLine labelLine9;

	private ButtonEditEx bte_Store;

	private GridControlEx grd_ListParam;

	private GridViewEx gv_ListParam;

	private RichEditControl txt_HeaderExcel;

	private System.Windows.Forms.ComboBox cbb_HeaderExcel;

	private LabelLine labelLine13;

	private System.Windows.Forms.ComboBox cbb_Align;

	private LabelLine labelLine14;

	public frm_UDR()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UDR"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UDR" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
			_nextText = wiz_UDR.NextText;
			cbb_HeaderExcel.MouseWheel += Cbb_HeaderExcel_MouseWheel;
			cbb_Align.MouseWheel += Cbb_HeaderExcel_MouseWheel;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void Cbb_HeaderExcel_MouseWheel(object sender, MouseEventArgs e)
	{
		((HandledMouseEventArgs)e).Handled = true;
	}

	private async void frm_UDR_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			gv_ListParam.DisableFormSetting = true;
			bte_ParentMenuId.Properties.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			bte_ParentMenuId.Properties.StoredProcedure = "Apz_Cfl_GetFatherMenu";
			bte_ParentMenuId.Properties.RefreshDataSource();
			bte_ParentMenuId.Properties.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL properties = bte_ParentMenuId.Properties;
			properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm._tableType = "2";
			}));
			bte_Store.ReadOnly = true;
			chk_PlaceBottom.Checked = true;
			chk_PlaceBottom.ReadOnly = true;
			chk_MenuItem.Checked = true;
			bte_ParentMenuId.ReadOnly = true;
			SetDataType();
			rdg_OperationType.Properties.AllowMouseWheel = false;
			DataSetResult dataSetResult = await _httpClient.Udr.GetListReport(GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				return;
			}
			await GridViewHelper.GenerateGrid(grd_ListReport, gv_ListReport, dataSetResult.Data, null, expand: true);
			rdg_OperationType.SelectedIndex = 0;
			gv_ListParam.OptionsCustomization.AllowSort = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rdg_OperationType_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (rdg_OperationType.SelectedIndex)
		{
		case 0:
			_currentAction = Enums.ImportDOREnum.Add;
			grd_ListReport.Visible = false;
			btn_Export.Enabled = false;
			wiz_UDR.NextText = _nextText;
			break;
		case 1:
			_currentAction = Enums.ImportDOREnum.Update;
			grd_ListReport.Visible = true;
			gv_ListReport.Focus();
			btn_Export.Enabled = true;
			wiz_UDR.NextText = _nextText;
			break;
		case 2:
			_currentAction = Enums.ImportDOREnum.Delete;
			grd_ListReport.Visible = true;
			gv_ListReport.Focus();
			btn_Export.Enabled = true;
			wiz_UDR.NextText = "Finish";
			break;
		}
	}

	private void cbx_QueryType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			bte_Store.Text = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_Store_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("@type", cbx_QueryType.EditValue));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, bte_Store.Text, "Apz_Cfl_GetUserProcedure", Enums.ItemObjectType.NoItemType, list, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("SPECIFIC_NAME"))
			{
				bte_Store.Text = Function.ToString(chooseItem["SPECIFIC_NAME"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bte_Store_EditValueChanged(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (string.IsNullOrEmpty(bte_Store.Text))
			{
				grd_ListParam.DataSourceTable().Rows.Clear();
				grd_ListParam.RefreshDataSource();
				dtValidValue = new DataTable();
				return;
			}
			string code = "";
			if (_currentAction == Enums.ImportDOREnum.Update)
			{
				code = txt_Code.Text;
			}
			DataSetResult dataSetResult = await _httpClient.Udr.GetParamByStore(bte_Store.Text, code, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				return;
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsNotNull() && ds.Tables.Count > 3)
			{
				dtValidValue = ds.Tables[3].Copy();
				ds.Tables.RemoveAt(3);
			}
			if (ds.IsHasValidValue() && ds.Tables[1].Rows.Count > 0)
			{
				DataTable dataTable = ds.Tables[1].Copy();
				DataTable table = ds.Tables[2].Copy();
				dataTable.DefaultView.Sort = "LineNum ASC";
				dataTable = dataTable.DefaultView.ToTable();
				ds.Tables.RemoveAt(1);
				ds.Tables.RemoveAt(1);
				ds.Tables.Add(dataTable);
				ds.Tables.Add(table);
			}
			await GridViewHelper.GenerateGrid(grd_ListParam, gv_ListParam, ds, null, expand: true);
			ds.DisposeDataSet();
			gv_ListParam.Columns["ParamName"].OptionsColumn.ReadOnly = true;
			gv_ListParam.Columns["ParamName"].OptionsColumn.AllowEdit = false;
			gv_ListParam.OptionsCustomization.AllowSort = false;
			DataTable dtParam = grd_ListParam.DataSourceTable();
			if (!Function.ToString(cbx_QueryType.EditValue).Equals("U") || dtParam.IsNotNull())
			{
				return;
			}
			string sql = "";
			DataTable dataTable2 = await _httpClient.Queries.GetQueryById(Function.ParseInt(bte_Store.EditValue));
			if (dataTable2.IsNotNull())
			{
				sql = Function.ToString(dataTable2.Rows[0]["QString"]);
			}
			List<DatabaseParamater> list = Function.ParseSql(Regex.Replace(Regex.Replace(Regex.Replace(sql, "@CURRENTUSER", $"N'{GlobalData.UserName}'", RegexOptions.IgnoreCase), "$[USER]", $"N'{GlobalData.UserID}'", RegexOptions.IgnoreCase), "@CURRENTBRANCH", $"N'{GlobalData.DefaultBranch}'", RegexOptions.IgnoreCase));
			DataTable dataTable3 = await GetInfoColum(list);
			if (dataTable3.IsNotNull())
			{
				for (int i = 0; i < dataTable3.Rows.Count; i++)
				{
					DataRow dataRow = dtParam.NewRow();
					dataRow["Code"] = txt_Code.EditValue;
					dataRow["LineNum"] = i;
					dataRow["ParamName"] = dataTable3.Rows[i]["Para"];
					dataRow["Label"] = dataTable3.Rows[i]["ColumnField"];
					dataRow["Type"] = dataTable3.Rows[i]["DataType"];
					dataRow["DataType"] = "queryparam";
					dataRow["FormatString"] = "";
					dataRow["SubType"] = dataTable3.Rows[i]["ColumnField"]?.ToString() + "/" + dataTable3.Rows[i]["TableName"];
					dataRow["DefaultValue"] = "";
					dataRow["ToolTip"] = "";
					dataRow["AllowMultiple"] = false;
					dataRow["Visible"] = true;
					dataRow["AllowEdit"] = true;
					dtParam.Rows.Add(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListReport_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			bte_Store.Text = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void wiz_UDR_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		try
		{
			switch (e.Page.Name)
			{
			case "wp_Operation":
				switch (_currentAction)
				{
				case Enums.ImportDOREnum.Add:
				{
					txt_Code.Text = string.Empty;
					txt_Name.Text = string.Empty;
					txt_TableName.Text = string.Empty;
					txt_Caption.Text = string.Empty;
					bte_ParentMenuId.EditValue = string.Empty;
					cbx_Type.EditValue = 1;
					check_ShowChk.Checked = true;
					check_allRows.Checked = false;
					txt_Code.ReadOnly = false;
					txt_LogTable.ReadOnly = false;
					System.Windows.Forms.ComboBox comboBox = cbb_HeaderExcel;
					int selectedIndex = (cbb_Align.SelectedIndex = 0);
					comboBox.SelectedIndex = selectedIndex;
					txt_HeaderExcel.Text = string.Empty;
					RichEditControl richEditControl = txt_HeaderExcel;
					LabelLine labelLine = labelLine14;
					bool flag = (cbb_Align.Enabled = false);
					bool enabled = (labelLine.Enabled = flag);
					richEditControl.Enabled = enabled;
					break;
				}
				case Enums.ImportDOREnum.Update:
				{
					if (gv_ListReport.FocusedRowHandle < 0)
					{
						e.Handled = true;
					}
					txt_Code.ReadOnly = true;
					txt_LogTable.ReadOnly = true;
					txt_Code.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("Code"));
					txt_Name.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("Name"));
					cbx_Type.EditValue = Function.ParseInt(gv_ListReport.GetFocusedRowCellValue("TYPE"));
					txt_TableName.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("TableName"));
					txt_FileName.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("ReportFile"));
					chk_MenuItem.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("MenuItem")).Equals("Y");
					txt_Caption.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("MenuCapt"));
					bte_ParentMenuId.EditValue = Function.ToString(gv_ListReport.GetFocusedRowCellValue("FatherMenu"));
					txt_Position.EditValue = Function.ParseInt(gv_ListReport.GetFocusedRowCellValue("Position"));
					txt_MenuUID.EditValue = Function.ToString(gv_ListReport.GetFocusedRowCellValue("Position"));
					chk_Find.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("CanFind")).Equals("Y");
					chk_Delete.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("CanDelete")).Equals("Y");
					chk_Close.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("CanClose")).Equals("Y");
					chk_Cancel.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("CanCancel")).Equals("Y");
					cbx_QueryType.EditValue = Function.ToString(gv_ListReport.GetFocusedRowCellValue("QueryType"));
					check_allRows.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("AllRowsInGrp")).Equals("Y");
					check_ShowChk.Checked = Function.ToString(gv_ListReport.GetFocusedRowCellValue("ShChkBSelect")) != "N";
					string text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("EnbExlHeader"));
					if (!(text == "S"))
					{
						if (text == "M")
						{
							cbb_HeaderExcel.SelectedIndex = 2;
						}
						else
						{
							cbb_HeaderExcel.SelectedIndex = 0;
						}
					}
					else
					{
						cbb_HeaderExcel.SelectedIndex = 1;
					}
					string text2 = Function.ToString(gv_ListReport.GetFocusedRowCellValue("ExlHeaderAlign"));
					if (!(text2 == "C"))
					{
						if (text2 == "R")
						{
							cbb_Align.SelectedIndex = 2;
						}
						else
						{
							cbb_Align.SelectedIndex = 0;
						}
					}
					else
					{
						cbb_Align.SelectedIndex = 1;
					}
					txt_HeaderExcel.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("ExcelHeader"));
					RichEditControl richEditControl2 = txt_HeaderExcel;
					LabelLine labelLine2 = labelLine14;
					bool flag = (cbb_Align.Enabled = cbb_HeaderExcel.SelectedIndex != 0);
					bool enabled = (labelLine2.Enabled = flag);
					richEditControl2.Enabled = enabled;
					bte_Store.Text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("StoreName"));
					break;
				}
				case Enums.ImportDOREnum.Delete:
					e.Handled = true;
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						await DeleteData();
					}
					break;
				case Enums.ImportDOREnum.Unregister:
					break;
				}
				break;
			case "wp_Setting":
				switch (_currentAction)
				{
				case Enums.ImportDOREnum.Add:
					e.Handled = !ValidateAdd();
					bte_Store.Text = "";
					grd_ListParam.DataSource = null;
					break;
				case Enums.ImportDOREnum.Unregister:
					e.Handled = true;
					break;
				case Enums.ImportDOREnum.Delete:
					e.Handled = true;
					break;
				case Enums.ImportDOREnum.Update:
					break;
				}
				break;
			case "wp_StoreParam":
				if (string.IsNullOrEmpty(bte_Store.Text))
				{
					WinCommons.Warning(Enums.MessageCode.StoreIsNotEmpty, "");
					e.Handled = true;
				}
				break;
			case "wp_File":
				break;
			case "wp_Complete":
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void wiz_UDR_FinishClick(object sender, CancelEventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveData();
		}
	}

	private void wiz_UDR_CancelClick(object sender, CancelEventArgs e)
	{
		Close();
	}

	private void chk_MenuItem_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			txt_Caption.ReadOnly = !chk_MenuItem.Checked;
			bte_ParentMenuId.Enabled = chk_MenuItem.Checked;
			txt_MenuUID.ReadOnly = !chk_MenuItem.Checked;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Log_EditValueChanged(object sender, EventArgs e)
	{
		txt_LogTable.Visible = chk_Log.Checked;
	}

	public void SetDataType()
	{
		List<Common.Table_ID> list = new List<Common.Table_ID>();
		list.Add(new Common.Table_ID
		{
			Name = "GridView Report",
			ID = 1
		});
		list.Add(new Common.Table_ID
		{
			Name = "TreeView Report",
			ID = 2
		});
		list.Add(new Common.Table_ID
		{
			Name = "PivotGrid Report",
			ID = 3
		});
		cbx_Type.Properties.DataSource = list;
		cbx_Type.EditValue = 1;
		List<Common.Table_ID> list2 = new List<Common.Table_ID>();
		list2.Add(new Common.Table_ID
		{
			Name = "Store Procedure",
			ID = "S"
		});
		list2.Add(new Common.Table_ID
		{
			Name = "User Queries",
			ID = "U"
		});
		cbx_QueryType.Properties.DataSource = list2;
		cbx_QueryType.EditValue = "S";
	}

	private async Task SaveData()
	{
		_ = 2;
		try
		{
			wp_Complete.AllowFinish = false;
			wp_Complete.AllowBack = false;
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			DataTable dataTable = GetControlsValue().Copy();
			if (dataTable.Columns.Contains("EnbExlHeader") && Function.ToString(dataTable.Rows[0]["EnbExlHeader"]) != "N" && Function.ToString(dataTable.Rows[0]["ExcelHeader"]) == "")
			{
				WinCommons.Error(Enums.MessageCode.None, "Excel header cannot be empty if enable.");
				return;
			}
			DataTable dataTable2 = new DataTable();
			DataTable dataTable3 = new DataTable();
			DataTable dataTable4 = grd_ListParam.DataSourceTable();
			if (dataTable4.IsNotNull())
			{
				for (int i = 0; i < dataTable4.Rows.Count; i++)
				{
					dataTable4.Rows[i]["LineNum"] = i;
				}
				dataTable2 = dataTable4.Copy();
			}
			if (dtValidValue != null)
			{
				dataTable3 = dtValidValue.Copy();
			}
			dataTable.TableName = "dtOUDR";
			dataTable2.TableName = "dtUDR1";
			dataTable3.TableName = "dtUDR2";
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			dataSet.Tables.Add(dataTable3);
			HttpResult rs = new HttpResult();
			switch (_currentAction)
			{
			case Enums.ImportDOREnum.Add:
				rs = await _httpClient.Udr.Add(dataSet);
				break;
			case Enums.ImportDOREnum.Update:
				rs = await _httpClient.Udr.Update(dataSet);
				break;
			}
			if (string.IsNullOrEmpty(rs.Message))
			{
				if (!string.IsNullOrEmpty(txt_FilePath.Text))
				{
					string text = await _httpClient.FileNetworkManagerClient.UpLoadFile("\\Report\\", true, txt_FilePath.Text);
					if (!string.IsNullOrEmpty(text))
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text);
					}
				}
				WinCommons.Success(Enums.MessageCode.None, Resources.Msg_Success + rs.Content);
				_actionSuccess = true;
				wp_Complete.AllowBack = true;
				wp_Complete.AllowNext = false;
				wiz_UDR.CancelText = "Close";
				if (RefreshMain != null)
				{
					RefreshMain();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, rs.Message);
				wp_Complete.AllowFinish = true;
				wp_Complete.AllowBack = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task DeleteData()
	{
		try
		{
			WinCommons._OpenProcessing();
			wp_Operation.AllowBack = false;
			wp_Operation.AllowNext = false;
			DataSet dataSet = new DataSet();
			string text = Function.ToString(gv_ListReport.GetFocusedRowCellValue("Code"));
			DataTable dataTable = grd_ListReport.DatasourceTable().Select("Code = '" + text + "'").CopyToDataTable();
			dataTable.TableName = "dtOUDR";
			dataSet.Tables.Add(dataTable);
			new HttpResult();
			HttpResult httpResult = await _httpClient.Udr.Delete(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.None, Resources.Msg_Success + httpResult.Content);
				_actionSuccess = true;
				wiz_UDR.SelectedPage = wp_Complete;
				wp_Complete.AllowNext = false;
				wp_Complete.AllowFinish = false;
				wiz_UDR.CancelText = "Close";
				if (RefreshMain != null)
				{
					RefreshMain();
				}
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
		finally
		{
			wp_Operation.AllowBack = true;
			wp_Operation.AllowNext = true;
		}
	}

	private DataTable GetControlsValue()
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OUDR");
		DataRow dataRow = tableStructure.NewRow();
		try
		{
			tableStructure.Rows.Add(dataRow);
			dataRow["Code"] = txt_Code.Text.Trim();
			dataRow["Name"] = txt_Name.Text.Trim();
			dataRow["TableName"] = txt_TableName.Text.Trim();
			dataRow["StoreName"] = bte_Store.Text;
			dataRow["ReportFile"] = txt_FileName.Text.Trim();
			dataRow["LogTable"] = txt_LogTable.Text.Trim();
			dataRow["TYPE"] = cbx_Type.EditValue;
			dataRow["CanDelete"] = (chk_Delete.Checked ? "Y" : "N");
			dataRow["CanClose"] = (chk_Close.Checked ? "Y" : "N");
			dataRow["CanCancel"] = (chk_Cancel.Checked ? "Y" : "N");
			dataRow["CanFind"] = (chk_Find.Checked ? "Y" : "N");
			dataRow["CanLog"] = (chk_Log.Checked ? "Y" : "N");
			dataRow["MenuItem"] = (chk_MenuItem.Checked ? "Y" : "N");
			dataRow["MenuCapt"] = txt_Caption.Text;
			dataRow["FatherMenu"] = bte_ParentMenuId.EditValue;
			dataRow["Position"] = Function.ParseInt(txt_Position.EditValue);
			dataRow["MenuUid"] = txt_MenuUID.EditValue;
			dataRow["QueryType"] = cbx_QueryType.EditValue;
			dataRow["AllRowsInGrp"] = (check_allRows.Checked ? "Y" : "N");
			if (tableStructure.Columns.Contains("ShChkBSelect"))
			{
				dataRow["ShChkBSelect"] = (check_ShowChk.Checked ? "Y" : "N");
			}
			if (tableStructure.Columns.Contains("EnbExlHeader"))
			{
				DataRow dataRow2 = dataRow;
				string value;
				if (cbb_HeaderExcel.SelectedIndex != 0)
				{
					value = ((cbb_HeaderExcel.SelectedIndex == 1) ? "S" : "M");
				}
				else
				{
					value = "N";
				}
				dataRow2["EnbExlHeader"] = value;
				dataRow["ExcelHeader"] = txt_HeaderExcel.Text;
			}
			if (tableStructure.Columns.Contains("ExlHeaderAlign"))
			{
				DataRow dataRow3 = dataRow;
				string value2;
				if (cbb_Align.SelectedIndex != 0)
				{
					value2 = ((cbb_Align.SelectedIndex == 1) ? "C" : "R");
				}
				else
				{
					value2 = "L";
				}
				dataRow3["ExlHeaderAlign"] = value2;
			}
			tableStructure.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return tableStructure;
	}

	private bool ValidateAdd()
	{
		if (string.IsNullOrEmpty(txt_Code.Text))
		{
			WinCommons.Warning(Enums.MessageCode.CodeCannotBeEmpty, "");
			return false;
		}
		if (string.IsNullOrEmpty(txt_Caption.Text))
		{
			WinCommons.Warning(Enums.MessageCode.CaptionIsNotEmpty, "");
			return false;
		}
		return true;
	}

	private void buttonEdit1_Properties_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Filter = "Report file (.rpt)|*.rpt|Excel file (.xlsx)|*.xlsx";
			openFileDialog.Multiselect = false;
			if (openFileDialog.ShowDialog(this) == DialogResult.OK)
			{
				string fileName = Path.GetFileName(openFileDialog.FileName);
				string fileName2 = openFileDialog.FileName;
				txt_FileName.Text = fileName;
				txt_FilePath.Text = fileName2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_PlaceBottom_CheckedChanged(object sender, EventArgs e)
	{
		txt_Position.ReadOnly = chk_PlaceBottom.Checked;
	}

	private void gv_ListParam_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			downHitInfo = null;
			if (sender is GridControlEx gridControlEx)
			{
				_ = gridControlEx.MainView;
				DXMouseEventArgs e2 = e as DXMouseEventArgs;
				GridHitInfo gridHitInfo = gv_ListParam.CalcHitInfo(e2.Location);
				if (Control.ModifierKeys == Keys.None && e.Button == MouseButtons.Left && gridHitInfo.InRow && gridHitInfo.RowHandle != -2147483647)
				{
					downHitInfo = gridHitInfo;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListParam_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			if (e.Button != MouseButtons.Left || downHitInfo == null)
			{
				return;
			}
			Size dragSize = SystemInformation.DragSize;
			Rectangle rectangle = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2, downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
			if (!rectangle.Contains(new Point(e.X, e.Y)))
			{
				if (sender is GridControlEx gridControlEx)
				{
					gridControlEx.DoDragDrop(downHitInfo, DragDropEffects.All);
				}
				downHitInfo = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void MoveRow(int sourceRow, int targetRow)
	{
		try
		{
			if (sourceRow != targetRow)
			{
				DataTable dataTable = grd_ListParam.DatasourceTable();
				DataRow dataRow = dataTable.Rows[sourceRow];
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				dataTable.Rows.Remove(dataRow);
				dataTable.Rows.InsertAt(dataRow2, targetRow);
				dataTable.AcceptChanges();
				gv_ListParam.FocusedRowHandle = targetRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_EditValidValue_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow[] array = grd_ListParam.DatasourceTable().Select("DataType = 'validvalue'");
			if (array != null && array.Length != 0)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("ColumnName", typeof(string));
				DataRow[] array2 = array;
				foreach (DataRow dataRow in array2)
				{
					DataRow dataRow2 = dataTable.NewRow();
					dataRow2["ColumnName"] = dataRow["ParamName"];
					dataTable.Rows.Add(dataRow2);
				}
				frm_ValidValue frm_ValidValue2 = new frm_ValidValue(dataTable, dtValidValue);
				if (Function.ToString(gv_ListParam.GetFocusedRowCellValue("DataType")).Equals("validvalue"))
				{
					frm_ValidValue2.dfColumn = Function.ToString(gv_ListParam.GetFocusedRowCellValue("ParamName"));
				}
				frm_ValidValue2._code = txt_Code.Text;
				frm_ValidValue2._type = "2";
				frm_ValidValue2.ShowDialog(this);
				if (frm_ValidValue2.isSave)
				{
					dtValidValue = frm_ValidValue2._dtData.Copy();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_ListParam_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			if (e.Data.GetDataPresent(typeof(GridHitInfo)) && e.Data.GetData(typeof(GridHitInfo)) is GridHitInfo gridHitInfo && sender is GridControlEx { MainView: GridViewEx mainView } gridControlEx)
			{
				GridHitInfo gridHitInfo2 = mainView.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo2.InRow && gridHitInfo2.RowHandle != gridHitInfo.RowHandle && gridHitInfo2.RowHandle != -2147483647)
				{
					e.Effect = DragDropEffects.Move;
				}
				else
				{
					e.Effect = DragDropEffects.None;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_ListParam_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridControlEx.MainView is GridViewEx gridViewEx)
			{
				GridHitInfo gridHitInfo2 = gridViewEx.CalcHitInfo(gridControlEx.PointToClient(new Point(e.X, e.Y)));
				if (gridHitInfo != null)
				{
					int rowHandle = gridHitInfo.RowHandle;
					int rowHandle2 = gridHitInfo2.RowHandle;
					MoveRow(rowHandle, rowHandle2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetInfoColum(List<DatabaseParamater> List)
	{
		try
		{
			if (List == null || List.Count == 0)
			{
				return new DataTable();
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Para", typeof(string));
			dataTable.Columns.Add("Column", typeof(string));
			dataTable.Columns.Add("Table", typeof(string));
			dataTable.BeginLoadData();
			for (int i = 0; i < List.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Para"] = List[i].ParaName;
				string text = (string)(dataRow["Column"] = List[i].ColumnName);
				if (text.LastIndexOf(".", StringComparison.Ordinal) >= 0)
				{
					dataRow["Column"] = text.Substring(text.LastIndexOf(".", StringComparison.Ordinal) + 1);
				}
				dataRow["Table"] = List[i].TableName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			dataTable.EndLoadData();
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			return await http.PostAsync("GetInfoColum", new Tuple<DataTable, string>(dataTable, GlobalData.UserCodeLogin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private async void btn_Export_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (gv_ListReport.SelectedRowsCount > 1)
			{
				ExportMultiReport();
				return;
			}
			WinCommons._OpenProcessing();
			string code = Function.ToString(gv_ListReport.GetFocusedRowCellValue("Code"));
			string storeName = Function.ToString(gv_ListReport.GetFocusedRowCellValue("StoreName"));
			DataTable dtOUDO = grd_ListReport.DatasourceTable().Select("Code = '" + code + "'").CopyToDataTable();
			DataTable dtODU1 = new DataTable();
			DataTable dtODU2 = new DataTable();
			DataSetResult dataSetResult = await _httpClient.Udr.GetParamByStore(storeName, code, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				return;
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsHasStructure())
			{
				dtODU1 = ds.Tables[1].Copy();
			}
			if (ds.IsNotNull() && ds.Tables.Count > 3 && ds.Tables[3].IsNotNull())
			{
				dtODU2 = ds.Tables[3].Copy();
			}
			dtOUDO.TableName = "dtOUDR";
			dtODU1.TableName = "dtUDR1";
			dtODU2.TableName = "dtUDR2";
			DataSet dsSave = new DataSet
			{
				DataSetName = "DataSetReport",
				Tables = { dtOUDO, dtODU1, dtODU2 }
			};
			DataTable dt = await _httpClient.FormSettingClient.GetListFormSetting(ApzObjectTypes.oUserDefinedReports.ToString(), code, GlobalData.UserName);
			if (dt.IsNotNull())
			{
				int structureId = Function.ParseInt(dt.Rows[0]["ID"]);
				DataSet dataSet = await _httpClient.FormSettingClient.GetStructure(structureId, ApzObjectTypes.oUserDefinedReports.ToString(), code, GlobalData.UserName, GlobalData.DefaultBranch, "0");
				if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
				{
					DataTable dataTable = dt.Select("ID = " + structureId).CopyToDataTable();
					new DataTable();
					DataTable dataTable2 = new DataTable();
					DataTable dataTable3 = dataSet.Tables[0].Copy();
					dataTable.Rows[0]["ID"] = 0;
					foreach (DataRow row in dataTable3.Rows)
					{
						row["ID"] = 0;
					}
					if (dataSet.IsHasStructure() && dataSet.Tables[1].IsNotNull())
					{
						dataTable2 = dataSet.Tables[1].Copy();
						foreach (DataRow row2 in dataTable2.Rows)
						{
							row2["ID"] = 0;
						}
					}
					dataTable.TableName = "OFST";
					dataTable3.TableName = "FST1";
					dataTable2.TableName = "FST2";
					dsSave.Tables.Add(dataTable);
					dsSave.Tables.Add(dataTable3);
					dsSave.Tables.Add(dataTable2);
				}
			}
			WinCommons._CloseProcessing();
			string text = ExtensionMethods.PathSaveFile("XML", Enums.ExportExt.Xml);
			if (!string.IsNullOrEmpty(text))
			{
				dsSave.WriteXml(text, XmlWriteMode.WriteSchema);
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			dsSave.DisposeDataSet();
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ExportMultiReport()
	{
		string folderPath = WinFuntions.PathSaveFolder();
		if (string.IsNullOrEmpty(folderPath))
		{
			return;
		}
		WinCommons._OpenProcessing();
		List<DataRow> listDataSourceSelectedRow = gv_ListReport.GetListDataSourceSelectedRow();
		foreach (DataRow item in listDataSourceSelectedRow)
		{
			string code = Function.ToString(item["Code"]);
			string storeName = Function.ToString(item["StoreName"]);
			DataTable dtOUDO = grd_ListReport.DatasourceTable().Select("Code = '" + code + "'").CopyToDataTable();
			DataTable dtODU1 = new DataTable();
			DataTable dtODU2 = new DataTable();
			DataSetResult dataSetResult = await _httpClient.Udr.GetParamByStore(storeName, code, GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				return;
			}
			DataSet ds = dataSetResult.Data;
			if (ds.IsHasStructure())
			{
				dtODU1 = ds.Tables[1].Copy();
			}
			if (ds.IsNotNull() && ds.Tables.Count > 3 && ds.Tables[3].IsNotNull())
			{
				dtODU2 = ds.Tables[3].Copy();
			}
			dtOUDO.TableName = "dtOUDR";
			dtODU1.TableName = "dtUDR1";
			dtODU2.TableName = "dtUDR2";
			DataSet dsSave = new DataSet
			{
				DataSetName = "DataSetReport",
				Tables = { dtOUDO, dtODU1, dtODU2 }
			};
			DataTable dt = await _httpClient.FormSettingClient.GetListFormSetting(ApzObjectTypes.oUserDefinedReports.ToString(), code, GlobalData.UserName);
			if (dt.IsNotNull())
			{
				int structureId = Function.ParseInt(dt.Rows[0]["ID"]);
				DataSet dataSet = await _httpClient.FormSettingClient.GetStructure(structureId, ApzObjectTypes.oUserDefinedReports.ToString(), code, GlobalData.UserName, GlobalData.DefaultBranch, "0");
				if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
				{
					DataTable dataTable = dt.Select("ID = " + structureId).CopyToDataTable();
					new DataTable();
					DataTable dataTable2 = new DataTable();
					DataTable dataTable3 = dataSet.Tables[0].Copy();
					dataTable.Rows[0]["ID"] = 0;
					foreach (DataRow row in dataTable3.Rows)
					{
						row["ID"] = 0;
					}
					if (dataSet.IsHasStructure() && dataSet.Tables[1].IsNotNull())
					{
						dataTable2 = dataSet.Tables[1].Copy();
						foreach (DataRow row2 in dataTable2.Rows)
						{
							row2["ID"] = 0;
						}
					}
					dataTable.TableName = "OFST";
					dataTable3.TableName = "FST1";
					dataTable2.TableName = "FST2";
					dsSave.Tables.Add(dataTable);
					dsSave.Tables.Add(dataTable3);
					dsSave.Tables.Add(dataTable2);
				}
			}
			string fileName = folderPath + "\\" + code + ".xml";
			dsSave.WriteXml(fileName, XmlWriteMode.WriteSchema);
			dsSave.DisposeDataSet();
			ds.DisposeDataSet();
			WinCommons.Success(Enums.MessageCode.None, Resources.Msg_Success + " " + code);
		}
		Process.Start(folderPath);
		WinCommons._CloseProcessing();
	}

	private async void btn_Import_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			string[] array = WinFuntions.PathOpenMultiFile("XML", "xml");
			if (array == null || array.Length == 0)
			{
				return;
			}
			WinCommons._OpenProcessing();
			string[] array2 = array;
			foreach (string pathFile in array2)
			{
				DataSet ds = new DataSet();
				ds.ReadXml(pathFile, XmlReadMode.ReadSchema);
				if (!ds.IsHasStructure())
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), Path.GetFileName(pathFile), "DataSet Table count:" + ds.Tables.Count);
					continue;
				}
				if (!ds.Tables.Contains("dtOUDR"))
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), Path.GetFileName(pathFile), "DataSet Table dtOUDR not found");
					continue;
				}
				new HttpResult();
				HttpResult rs = await _httpClient.Udr.Add(ds);
				if (string.IsNullOrEmpty(rs.Message))
				{
					if (ds.Tables.Contains("OFST") && ds.Tables["OFST"].IsNotNull())
					{
						foreach (DataRow row in ds.Tables["OFST"].Rows)
						{
							row["UserSign"] = GlobalData.UserName;
						}
						HttpResult httpResult = await _httpClient.FormSettingClient.Save(ds);
						if (string.IsNullOrEmpty(httpResult.Message))
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), Path.GetFileName(pathFile), httpResult.Message);
						}
					}
					WinCommons.Success(Enums.MessageCode.None, Path.GetFileName(pathFile) + "\\t\\" + Resources.Msg_Success + rs.Content);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, Path.GetFileName(pathFile) + "\\t\\" + Resources.Msg_SaveError + ": " + rs.Message);
				}
				ds.DisposeDataSet();
			}
			DataSetResult dataSetResult = await _httpClient.Udr.GetListReport(GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				return;
			}
			DataSet dsLoad = dataSetResult.Data;
			await GridViewHelper.GenerateGrid(grd_ListReport, gv_ListReport, dsLoad, null, expand: true);
			dsLoad.DisposeDataSet();
			if (RefreshMain != null)
			{
				RefreshMain();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ListParam_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (txt_HeaderExcel.Enabled)
			{
				DXMouseEventArgs e2 = e as DXMouseEventArgs;
				if (gv_ListParam.CalcHitInfo(e2.Location).InRow)
				{
					string text = Function.ToString(gv_ListParam.GetFocusedRowCellValue("ParamName"));
					DocumentPosition caretPosition = txt_HeaderExcel.Document.CaretPosition;
					SubDocument subDocument = caretPosition.BeginUpdateDocument();
					subDocument.InsertText(caretPosition, "[" + text + "]");
					caretPosition.EndUpdateDocument(subDocument);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			RichEditControl richEditControl = txt_HeaderExcel;
			LabelLine labelLine = labelLine14;
			bool flag = (cbb_Align.Enabled = cbb_HeaderExcel.SelectedIndex != 0);
			bool enabled = (labelLine.Enabled = flag);
			richEditControl.Enabled = enabled;
			if (txt_HeaderExcel.Enabled && string.IsNullOrWhiteSpace(txt_HeaderExcel.Text))
			{
				txt_HeaderExcel.Text = ((_currentAction == Enums.ImportDOREnum.Update) ? Function.ToString(gv_ListReport.GetFocusedRowCellValue("MenuCapt")) : txt_Caption.Text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void wiz_UDR_PrevClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		if (!_actionSuccess)
		{
			return;
		}
		try
		{
			grd_ListReport.DataSourceTable.Clear();
			gv_ListReport.ShowLoadingPanel();
			wiz_UDR.SelectedPage = wp_Operation;
			_actionSuccess = false;
			wp_Complete.AllowFinish = true;
			wp_Complete.AllowNext = true;
			wiz_UDR.CancelText = "Cancel";
			e.Handled = true;
			DataSetResult dataSetResult = await _httpClient.Udr.GetListReport(GlobalData.UserName, GlobalData.DefaultBranch);
			if (!string.IsNullOrEmpty(dataSetResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, dataSetResult.Message);
				e.Handled = true;
			}
			else
			{
				grd_ListReport.DataSource = dataSetResult.Data.Tables[1];
				grd_ListReport.RefreshDataSource();
				gv_ListReport.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
		finally
		{
			gv_ListReport.HideLoadingPanel();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_UDR));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.wiz_UDR = new DevExpress.XtraWizard.WizardControl();
		this.wp_Welcome = new DevExpress.XtraWizard.WelcomeWizardPage();
		this.wp_Complete = new DevExpress.XtraWizard.CompletionWizardPage();
		this.wp_Operation = new DevExpress.XtraWizard.WizardPage();
		this.btn_Import = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_ListReport = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListReport = new Apzon.Controls.Grid.GridViewEx();
		this.rdg_OperationType = new Apzon.Controls.Editor.RadioGroupEx();
		this.wp_Setting = new DevExpress.XtraWizard.WizardPage();
		this.check_ShowChk = new Apzon.Controls.Editor.CheckEditEx();
		this.check_allRows = new Apzon.Controls.Editor.CheckEditEx();
		this.bte_ParentMenuId = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.cbx_Type = new DevExpress.XtraEditors.LookUpEdit();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.chk_Close = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_LogTable = new Apzon.Controls.Editor.TextEditEx();
		this.chk_Log = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Delete = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Cancel = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Find = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Update = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Add = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_PlaceBottom = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_Position = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MenuUID = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Caption = new Apzon.Controls.Editor.TextEditEx();
		this.chk_MenuItem = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_TableName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.wp_StoreParam = new DevExpress.XtraWizard.WizardPage();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.cbx_QueryType = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_EditValidValue = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.bte_Store = new Apzon.Controls.Editor.ButtonEditEx();
		this.grd_ListParam = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListParam = new Apzon.Controls.Grid.GridViewEx();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.cbb_HeaderExcel = new System.Windows.Forms.ComboBox();
		this.txt_HeaderExcel = new DevExpress.XtraRichEdit.RichEditControl();
		this.wp_File = new DevExpress.XtraWizard.WizardPage();
		this.txt_FilePath = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.txt_FileName = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.cbb_Align = new System.Windows.Forms.ComboBox();
		((System.ComponentModel.ISupportInitialize)this.wiz_UDR).BeginInit();
		this.wiz_UDR.SuspendLayout();
		this.wp_Operation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ListReport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListReport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_OperationType.Properties).BeginInit();
		this.wp_Setting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.check_ShowChk.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_allRows.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ParentMenuId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Close.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LogTable.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Log.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Delete.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Cancel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Find.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Update.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Add.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PlaceBottom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Position.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MenuUID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MenuItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TableName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		this.wp_StoreParam.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbx_QueryType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Store.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ListParam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListParam).BeginInit();
		this.wp_File.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_FilePath.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FileName.Properties).BeginInit();
		base.SuspendLayout();
		this.wiz_UDR.Controls.Add(this.wp_Welcome);
		this.wiz_UDR.Controls.Add(this.wp_Complete);
		this.wiz_UDR.Controls.Add(this.wp_Operation);
		this.wiz_UDR.Controls.Add(this.wp_Setting);
		this.wiz_UDR.Controls.Add(this.wp_StoreParam);
		this.wiz_UDR.Controls.Add(this.wp_File);
		this.wiz_UDR.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wiz_UDR.ImageWidth = 216;
		this.wiz_UDR.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_UDR.MinimumSize = new System.Drawing.Size(117, 123);
		this.wiz_UDR.Name = "wiz_UDR";
		this.wiz_UDR.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[6] { this.wp_Welcome, this.wp_Operation, this.wp_Setting, this.wp_StoreParam, this.wp_File, this.wp_Complete });
		this.wiz_UDR.Size = new System.Drawing.Size(742, 624);
		this.wiz_UDR.Text = "Modifying Basic Settings";
		this.wiz_UDR.CancelClick += new System.ComponentModel.CancelEventHandler(this.wiz_UDR_CancelClick);
		this.wiz_UDR.FinishClick += new System.ComponentModel.CancelEventHandler(this.wiz_UDR_FinishClick);
		this.wiz_UDR.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_UDR_NextClick);
		this.wiz_UDR.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_UDR_PrevClick);
		this.wp_Welcome.IntroductionText = "Use this wizard to add a new user-defined report to IRS or to update, delete an exist user-defined report.";
		this.wp_Welcome.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wp_Welcome.Name = "wp_Welcome";
		this.wp_Welcome.Size = new System.Drawing.Size(486, 460);
		this.wp_Welcome.Text = "User-Defined Report Registration";
		this.wp_Complete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wp_Complete.FinishText = "";
		this.wp_Complete.Name = "wp_Complete";
		this.wp_Complete.Size = new System.Drawing.Size(486, 460);
		this.wp_Complete.ProceedText = "";
		this.wp_Operation.Controls.Add(this.btn_Import);
		this.wp_Operation.Controls.Add(this.btn_Export);
		this.wp_Operation.Controls.Add(this.grd_ListReport);
		this.wp_Operation.Controls.Add(this.rdg_OperationType);
		this.wp_Operation.DescriptionText = "";
		this.wp_Operation.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wp_Operation.Name = "wp_Operation";
		this.wp_Operation.Size = new System.Drawing.Size(702, 446);
		this.wp_Operation.Text = "Selecting the Registration Mode";
		this.btn_Import.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Import.EditControl = null;
		this.btn_Import.EditValue = "Import";
		this.btn_Import.FieldName = null;
		this.btn_Import.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Import.ImageOptions.Image");
		this.btn_Import.IsSearchControl = false;
		this.btn_Import.Location = new System.Drawing.Point(599, 43);
		this.btn_Import.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_Import.Name = "btn_Import";
		this.btn_Import.ReadOnly = false;
		this.btn_Import.SettingInVisible = false;
		this.btn_Import.SettingReadonly = false;
		this.btn_Import.Size = new System.Drawing.Size(87, 28);
		this.btn_Import.TabIndex = 12;
		this.btn_Import.Text = "Import";
		this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Export.EditControl = null;
		this.btn_Export.EditValue = "Export";
		this.btn_Export.FieldName = null;
		this.btn_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Export.ImageOptions.Image");
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(504, 43);
		this.btn_Export.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.ReadOnly = false;
		this.btn_Export.SettingInVisible = false;
		this.btn_Export.SettingReadonly = false;
		this.btn_Export.Size = new System.Drawing.Size(87, 28);
		this.btn_Export.TabIndex = 13;
		this.btn_Export.Text = "Export";
		this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
		this.grd_ListReport.AllowPasteExcel = false;
		this.grd_ListReport.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ListReport.ClipboardData = null;
		this.grd_ListReport.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_ListReport.Location = new System.Drawing.Point(0, 90);
		this.grd_ListReport.MainView = this.gv_ListReport;
		this.grd_ListReport.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_ListReport.Name = "grd_ListReport";
		this.grd_ListReport.Size = new System.Drawing.Size(702, 356);
		this.grd_ListReport.TabIndex = 6;
		this.grd_ListReport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListReport });
		this.grd_ListReport.Visible = false;
		this.gv_ListReport.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListReport.ColumnKey = null;
		this.gv_ListReport.DetailHeight = 431;
		this.gv_ListReport.DisableFormSetting = false;
		this.gv_ListReport.DisplayException = false;
		this.gv_ListReport.EditColumn = null;
		this.gv_ListReport.FieldCurrency = null;
		this.gv_ListReport.FieldRate = null;
		this.gv_ListReport.FormId = "oUserDefinedReport";
		this.gv_ListReport.GridControl = this.grd_ListReport;
		this.gv_ListReport.HasFormSetting = true;
		this.gv_ListReport.HeaderExcelExport = null;
		this.gv_ListReport.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_ListReport.IgnoreFilter = false;
		this.gv_ListReport.IndexEditRow = -1;
		this.gv_ListReport.IndicatorWidth = 41;
		this.gv_ListReport.IsApzTable = false;
		this.gv_ListReport.IsInsertNewRow = false;
		this.gv_ListReport.IsLoadingData = false;
		this.gv_ListReport.IsMappingContextAction = false;
		this.gv_ListReport.ItemId = "OLOR";
		this.gv_ListReport.LstColumnAllowBatch = null;
		this.gv_ListReport.LstColumnNotAllowBatch = null;
		this.gv_ListReport.LstColumnNotEditable = null;
		this.gv_ListReport.LstColumNotSetting = null;
		this.gv_ListReport.LstOldUDF = null;
		this.gv_ListReport.Name = "gv_ListReport";
		this.gv_ListReport.OptionsSelection.MultiSelect = true;
		this.gv_ListReport.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ListReport.OptionsView.ColumnAutoWidth = false;
		this.gv_ListReport.OptionsView.ShowGroupPanel = false;
		this.gv_ListReport.ReadOnly = false;
		this.gv_ListReport.SettingReadonly = false;
		this.gv_ListReport.UseBatchMod = false;
		this.gv_ListReport.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_ListReport_FocusedRowChanged);
		this.rdg_OperationType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rdg_OperationType.EditControl = null;
		this.rdg_OperationType.FieldName = null;
		this.rdg_OperationType.IsSearchControl = false;
		this.rdg_OperationType.Location = new System.Drawing.Point(0, 0);
		this.rdg_OperationType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.rdg_OperationType.Name = "rdg_OperationType";
		this.rdg_OperationType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_OperationType.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_OperationType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("add", "Add a New Report"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("update", "Update an existing report"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("Delete", "Delete an existing report")
		});
		this.rdg_OperationType.SettingInVisible = false;
		this.rdg_OperationType.SettingReadonly = false;
		this.rdg_OperationType.Size = new System.Drawing.Size(702, 82);
		this.rdg_OperationType.TabIndex = 5;
		this.rdg_OperationType.SelectedIndexChanged += new System.EventHandler(this.rdg_OperationType_SelectedIndexChanged);
		this.wp_Setting.Controls.Add(this.check_ShowChk);
		this.wp_Setting.Controls.Add(this.check_allRows);
		this.wp_Setting.Controls.Add(this.bte_ParentMenuId);
		this.wp_Setting.Controls.Add(this.labelLine5);
		this.wp_Setting.Controls.Add(this.labelLine6);
		this.wp_Setting.Controls.Add(this.labelLine7);
		this.wp_Setting.Controls.Add(this.labelLine8);
		this.wp_Setting.Controls.Add(this.labelLine4);
		this.wp_Setting.Controls.Add(this.labelLine3);
		this.wp_Setting.Controls.Add(this.labelLine2);
		this.wp_Setting.Controls.Add(this.labelLine1);
		this.wp_Setting.Controls.Add(this.cbx_Type);
		this.wp_Setting.Controls.Add(this.panelControl1);
		this.wp_Setting.Controls.Add(this.chk_PlaceBottom);
		this.wp_Setting.Controls.Add(this.txt_Position);
		this.wp_Setting.Controls.Add(this.txt_MenuUID);
		this.wp_Setting.Controls.Add(this.txt_Caption);
		this.wp_Setting.Controls.Add(this.chk_MenuItem);
		this.wp_Setting.Controls.Add(this.txt_TableName);
		this.wp_Setting.Controls.Add(this.txt_Name);
		this.wp_Setting.Controls.Add(this.txt_Code);
		this.wp_Setting.DescriptionText = "Modifying Basic Settings";
		this.wp_Setting.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wp_Setting.Name = "wp_Setting";
		this.wp_Setting.Size = new System.Drawing.Size(702, 446);
		this.wp_Setting.Text = "Modifying Basic Settings";
		this.check_ShowChk.EditControl = null;
		this.check_ShowChk.FieldName = null;
		this.check_ShowChk.IsSearchControl = false;
		this.check_ShowChk.Location = new System.Drawing.Point(17, 185);
		this.check_ShowChk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.check_ShowChk.Name = "check_ShowChk";
		this.check_ShowChk.Properties.Caption = "Show Check Box Select";
		this.check_ShowChk.SettingInVisible = false;
		this.check_ShowChk.SettingReadonly = false;
		this.check_ShowChk.Size = new System.Drawing.Size(302, 24);
		this.check_ShowChk.TabIndex = 38;
		this.check_allRows.EditControl = null;
		this.check_allRows.FieldName = null;
		this.check_allRows.IsSearchControl = false;
		this.check_allRows.Location = new System.Drawing.Point(17, 154);
		this.check_allRows.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.check_allRows.Name = "check_allRows";
		this.check_allRows.Properties.Caption = "Select/Unselect All Rows In Group";
		this.check_allRows.SettingInVisible = false;
		this.check_allRows.SettingReadonly = false;
		this.check_allRows.Size = new System.Drawing.Size(302, 24);
		this.check_allRows.TabIndex = 37;
		this.bte_ParentMenuId.EditControl = null;
		this.bte_ParentMenuId.FieldName = null;
		this.bte_ParentMenuId.IsSearchControl = false;
		this.bte_ParentMenuId.Location = new System.Drawing.Point(503, 68);
		this.bte_ParentMenuId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.bte_ParentMenuId.Name = "bte_ParentMenuId";
		this.bte_ParentMenuId.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_ParentMenuId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_ParentMenuId.Properties.ContainerControl = null;
		this.bte_ParentMenuId.Properties.DisplayMember = "Name";
		this.bte_ParentMenuId.Properties.DisplayMember2 = "";
		this.bte_ParentMenuId.Properties.ListMandatoryField = null;
		this.bte_ParentMenuId.Properties.LoadDataSourceWhenEnter = false;
		this.bte_ParentMenuId.Properties.SeparateCharacterForMulti = ';';
		this.bte_ParentMenuId.Properties.SeparateString = ", ";
		this.bte_ParentMenuId.Properties.UsingAutoCompleteBox = false;
		this.bte_ParentMenuId.Properties.UsingDataSource = true;
		this.bte_ParentMenuId.Properties.ValidateWhenNoDataSource = false;
		this.bte_ParentMenuId.Properties.ValueMember = "Code";
		this.bte_ParentMenuId.SettingInVisible = false;
		this.bte_ParentMenuId.SettingReadonly = false;
		this.bte_ParentMenuId.Size = new System.Drawing.Size(175, 22);
		this.bte_ParentMenuId.TabIndex = 27;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Menu UID";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(386, 135);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(117, 25);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 36;
		this.labelLine5.Utext = "Menu UID";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Position";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(386, 102);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(117, 25);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 35;
		this.labelLine6.Utext = "Position";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Parent Menu ID";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(386, 74);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(117, 25);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 34;
		this.labelLine7.Utext = "Parent Menu ID";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Menu Caption";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(386, 42);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(117, 25);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 33;
		this.labelLine8.Utext = "Menu Caption";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Table Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 118);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(117, 25);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 32;
		this.labelLine4.Utext = "Table Name";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Type";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(12, 85);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 25);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 31;
		this.labelLine3.Utext = "Type";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Name";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 50);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(117, 25);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 30;
		this.labelLine2.Utext = "Name";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Unique ID";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 16);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(117, 25);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 29;
		this.labelLine1.Utext = "Unique ID";
		this.cbx_Type.Location = new System.Drawing.Point(135, 82);
		this.cbx_Type.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbx_Type.Name = "cbx_Type";
		this.cbx_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_Type.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.cbx_Type.Properties.DisplayMember = "Name";
		this.cbx_Type.Properties.KeyMember = "ID";
		this.cbx_Type.Properties.NullText = "";
		this.cbx_Type.Properties.ValueMember = "ID";
		this.cbx_Type.Size = new System.Drawing.Size(192, 22);
		this.cbx_Type.TabIndex = 2;
		this.panelControl1.Controls.Add(this.chk_Close);
		this.panelControl1.Controls.Add(this.txt_LogTable);
		this.panelControl1.Controls.Add(this.chk_Log);
		this.panelControl1.Controls.Add(this.chk_Delete);
		this.panelControl1.Controls.Add(this.chk_Cancel);
		this.panelControl1.Controls.Add(this.chk_Find);
		this.panelControl1.Controls.Add(this.chk_Update);
		this.panelControl1.Controls.Add(this.chk_Add);
		this.panelControl1.Location = new System.Drawing.Point(3, 226);
		this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(701, 215);
		this.panelControl1.TabIndex = 28;
		this.panelControl1.Visible = false;
		this.chk_Close.EditControl = null;
		this.chk_Close.FieldName = null;
		this.chk_Close.IsSearchControl = false;
		this.chk_Close.Location = new System.Drawing.Point(122, 98);
		this.chk_Close.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Close.Name = "chk_Close";
		this.chk_Close.Properties.Caption = "Close";
		this.chk_Close.SettingInVisible = false;
		this.chk_Close.SettingReadonly = false;
		this.chk_Close.Size = new System.Drawing.Size(111, 24);
		this.chk_Close.TabIndex = 26;
		this.txt_LogTable.EditControl = null;
		this.txt_LogTable.FieldName = null;
		this.txt_LogTable.IsSearchControl = false;
		this.txt_LogTable.Location = new System.Drawing.Point(192, 228);
		this.txt_LogTable.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_LogTable.Name = "txt_LogTable";
		this.txt_LogTable.SettingInVisible = false;
		this.txt_LogTable.SettingReadonly = false;
		this.txt_LogTable.Size = new System.Drawing.Size(175, 22);
		this.txt_LogTable.TabIndex = 25;
		this.txt_LogTable.Visible = false;
		this.chk_Log.EditControl = null;
		this.chk_Log.FieldName = null;
		this.chk_Log.IsSearchControl = false;
		this.chk_Log.Location = new System.Drawing.Point(125, 229);
		this.chk_Log.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Log.Name = "chk_Log";
		this.chk_Log.Properties.Caption = "Log";
		this.chk_Log.SettingInVisible = false;
		this.chk_Log.SettingReadonly = false;
		this.chk_Log.Size = new System.Drawing.Size(61, 24);
		this.chk_Log.TabIndex = 24;
		this.chk_Log.Visible = false;
		this.chk_Log.EditValueChanged += new System.EventHandler(this.chk_Log_EditValueChanged);
		this.chk_Delete.EditControl = null;
		this.chk_Delete.FieldName = null;
		this.chk_Delete.IsSearchControl = false;
		this.chk_Delete.Location = new System.Drawing.Point(383, 6);
		this.chk_Delete.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Delete.Name = "chk_Delete";
		this.chk_Delete.Properties.Caption = "Delete";
		this.chk_Delete.SettingInVisible = false;
		this.chk_Delete.SettingReadonly = false;
		this.chk_Delete.Size = new System.Drawing.Size(111, 24);
		this.chk_Delete.TabIndex = 23;
		this.chk_Cancel.EditControl = null;
		this.chk_Cancel.FieldName = null;
		this.chk_Cancel.IsSearchControl = false;
		this.chk_Cancel.Location = new System.Drawing.Point(383, 37);
		this.chk_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Cancel.Name = "chk_Cancel";
		this.chk_Cancel.Properties.Caption = "Cancel";
		this.chk_Cancel.SettingInVisible = false;
		this.chk_Cancel.SettingReadonly = false;
		this.chk_Cancel.Size = new System.Drawing.Size(111, 24);
		this.chk_Cancel.TabIndex = 22;
		this.chk_Find.EditControl = null;
		this.chk_Find.FieldName = null;
		this.chk_Find.IsSearchControl = false;
		this.chk_Find.Location = new System.Drawing.Point(122, 68);
		this.chk_Find.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Find.Name = "chk_Find";
		this.chk_Find.Properties.Caption = "Find";
		this.chk_Find.SettingInVisible = false;
		this.chk_Find.SettingReadonly = false;
		this.chk_Find.Size = new System.Drawing.Size(111, 24);
		this.chk_Find.TabIndex = 21;
		this.chk_Update.EditControl = null;
		this.chk_Update.EditValue = true;
		this.chk_Update.Enabled = false;
		this.chk_Update.FieldName = null;
		this.chk_Update.IsSearchControl = false;
		this.chk_Update.Location = new System.Drawing.Point(122, 37);
		this.chk_Update.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Update.Name = "chk_Update";
		this.chk_Update.Properties.Caption = "Update";
		this.chk_Update.SettingInVisible = false;
		this.chk_Update.SettingReadonly = false;
		this.chk_Update.Size = new System.Drawing.Size(111, 24);
		this.chk_Update.TabIndex = 20;
		this.chk_Add.EditControl = null;
		this.chk_Add.EditValue = true;
		this.chk_Add.Enabled = false;
		this.chk_Add.FieldName = null;
		this.chk_Add.IsSearchControl = false;
		this.chk_Add.Location = new System.Drawing.Point(122, 6);
		this.chk_Add.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_Add.Name = "chk_Add";
		this.chk_Add.Properties.Caption = "Add";
		this.chk_Add.SettingInVisible = false;
		this.chk_Add.SettingReadonly = false;
		this.chk_Add.Size = new System.Drawing.Size(111, 24);
		this.chk_Add.TabIndex = 19;
		this.chk_PlaceBottom.EditControl = null;
		this.chk_PlaceBottom.FieldName = null;
		this.chk_PlaceBottom.IsSearchControl = false;
		this.chk_PlaceBottom.Location = new System.Drawing.Point(570, 100);
		this.chk_PlaceBottom.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_PlaceBottom.Name = "chk_PlaceBottom";
		this.chk_PlaceBottom.Properties.Caption = "&Place at Bottom";
		this.chk_PlaceBottom.SettingInVisible = false;
		this.chk_PlaceBottom.SettingReadonly = false;
		this.chk_PlaceBottom.Size = new System.Drawing.Size(124, 24);
		this.chk_PlaceBottom.TabIndex = 8;
		this.chk_PlaceBottom.CheckedChanged += new System.EventHandler(this.chk_PlaceBottom_CheckedChanged);
		this.txt_Position.EditControl = null;
		this.txt_Position.FieldName = null;
		this.txt_Position.IsSearchControl = false;
		this.txt_Position.Location = new System.Drawing.Point(503, 100);
		this.txt_Position.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_Position.Name = "txt_Position";
		this.txt_Position.SettingInVisible = false;
		this.txt_Position.SettingReadonly = false;
		this.txt_Position.Size = new System.Drawing.Size(61, 22);
		this.txt_Position.TabIndex = 7;
		this.txt_MenuUID.EditControl = null;
		this.txt_MenuUID.FieldName = null;
		this.txt_MenuUID.IsSearchControl = false;
		this.txt_MenuUID.Location = new System.Drawing.Point(503, 130);
		this.txt_MenuUID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_MenuUID.Name = "txt_MenuUID";
		this.txt_MenuUID.SettingInVisible = false;
		this.txt_MenuUID.SettingReadonly = false;
		this.txt_MenuUID.Size = new System.Drawing.Size(175, 22);
		this.txt_MenuUID.TabIndex = 9;
		this.txt_Caption.EditControl = null;
		this.txt_Caption.FieldName = null;
		this.txt_Caption.IsSearchControl = false;
		this.txt_Caption.Location = new System.Drawing.Point(503, 38);
		this.txt_Caption.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_Caption.Name = "txt_Caption";
		this.txt_Caption.SettingInVisible = false;
		this.txt_Caption.SettingReadonly = false;
		this.txt_Caption.Size = new System.Drawing.Size(175, 22);
		this.txt_Caption.TabIndex = 5;
		this.chk_MenuItem.EditControl = null;
		this.chk_MenuItem.FieldName = null;
		this.chk_MenuItem.IsSearchControl = false;
		this.chk_MenuItem.Location = new System.Drawing.Point(386, 14);
		this.chk_MenuItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.chk_MenuItem.Name = "chk_MenuItem";
		this.chk_MenuItem.Properties.Caption = "Menu Item";
		this.chk_MenuItem.SettingInVisible = false;
		this.chk_MenuItem.SettingReadonly = false;
		this.chk_MenuItem.Size = new System.Drawing.Size(111, 24);
		this.chk_MenuItem.TabIndex = 4;
		this.chk_MenuItem.CheckedChanged += new System.EventHandler(this.chk_MenuItem_CheckedChanged);
		this.txt_TableName.EditControl = null;
		this.txt_TableName.FieldName = null;
		this.txt_TableName.IsSearchControl = false;
		this.txt_TableName.Location = new System.Drawing.Point(135, 115);
		this.txt_TableName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_TableName.Name = "txt_TableName";
		this.txt_TableName.SettingInVisible = false;
		this.txt_TableName.SettingReadonly = false;
		this.txt_TableName.Size = new System.Drawing.Size(192, 22);
		this.txt_TableName.TabIndex = 3;
		this.txt_TableName.ToolTip = "Table Sap Object";
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(135, 47);
		this.txt_Name.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(192, 22);
		this.txt_Name.TabIndex = 1;
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(135, 15);
		this.txt_Code.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(192, 22);
		this.txt_Code.TabIndex = 0;
		this.wp_StoreParam.Controls.Add(this.splitContainerControl1);
		this.wp_StoreParam.DescriptionText = "Modifying Services";
		this.wp_StoreParam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wp_StoreParam.Name = "wp_StoreParam";
		this.wp_StoreParam.Size = new System.Drawing.Size(702, 446);
		this.wp_StoreParam.Text = "UI Settings";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.FixedPanel = DevExpress.XtraEditors.SplitFixedPanel.None;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine12);
		this.splitContainerControl1.Panel1.Controls.Add(this.cbx_QueryType);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_EditValidValue);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine9);
		this.splitContainerControl1.Panel1.Controls.Add(this.bte_Store);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_ListParam);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.cbb_Align);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine14);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine13);
		this.splitContainerControl1.Panel2.Controls.Add(this.cbb_HeaderExcel);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_HeaderExcel);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(702, 446);
		this.splitContainerControl1.SplitterPosition = 331;
		this.splitContainerControl1.TabIndex = 0;
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Type";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(16, 11);
		this.labelLine12.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(149, 25);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 52;
		this.labelLine12.Utext = "Type";
		this.cbx_QueryType.Location = new System.Drawing.Point(173, 7);
		this.cbx_QueryType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbx_QueryType.Name = "cbx_QueryType";
		this.cbx_QueryType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_QueryType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.cbx_QueryType.Properties.DisplayMember = "Name";
		this.cbx_QueryType.Properties.KeyMember = "ID";
		this.cbx_QueryType.Properties.NullText = "";
		this.cbx_QueryType.Properties.ValueMember = "ID";
		this.cbx_QueryType.Size = new System.Drawing.Size(206, 22);
		this.cbx_QueryType.TabIndex = 51;
		this.cbx_QueryType.EditValueChanged += new System.EventHandler(this.cbx_QueryType_EditValueChanged);
		this.btn_EditValidValue.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditValidValue.EditControl = null;
		this.btn_EditValidValue.EditValue = "Edit Valid Value";
		this.btn_EditValidValue.FieldName = null;
		this.btn_EditValidValue.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditValidValue.ImageOptions.Image");
		this.btn_EditValidValue.IsSearchControl = false;
		this.btn_EditValidValue.Location = new System.Drawing.Point(583, 2);
		this.btn_EditValidValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_EditValidValue.Name = "btn_EditValidValue";
		this.btn_EditValidValue.ReadOnly = false;
		this.btn_EditValidValue.SettingInVisible = false;
		this.btn_EditValidValue.SettingReadonly = false;
		this.btn_EditValidValue.Size = new System.Drawing.Size(118, 28);
		this.btn_EditValidValue.TabIndex = 50;
		this.btn_EditValidValue.Text = "Edit Valid Value";
		this.btn_EditValidValue.ToolTip = "Preview";
		this.btn_EditValidValue.Click += new System.EventHandler(this.btn_EditValidValue_Click);
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Store Procedure / Query";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(16, 39);
		this.labelLine9.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(149, 25);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 49;
		this.labelLine9.Utext = "Store Procedure / Query";
		this.bte_Store.EditControl = null;
		this.bte_Store.FieldName = null;
		this.bte_Store.IsSearchControl = false;
		this.bte_Store.Location = new System.Drawing.Point(173, 39);
		this.bte_Store.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.bte_Store.Name = "bte_Store";
		this.bte_Store.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.bte_Store.Properties.Appearance.Options.UseFont = true;
		this.bte_Store.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_Store.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_Store_Properties_ButtonClick);
		this.bte_Store.SettingInVisible = false;
		this.bte_Store.SettingReadonly = false;
		this.bte_Store.Size = new System.Drawing.Size(384, 22);
		this.bte_Store.TabIndex = 48;
		this.bte_Store.EditValueChanged += new System.EventHandler(this.bte_Store_EditValueChanged);
		this.grd_ListParam.AllowDrop = true;
		this.grd_ListParam.AllowPasteExcel = false;
		this.grd_ListParam.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ListParam.ClipboardData = null;
		this.grd_ListParam.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_ListParam.Location = new System.Drawing.Point(0, 69);
		this.grd_ListParam.MainView = this.gv_ListParam;
		this.grd_ListParam.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_ListParam.Name = "grd_ListParam";
		this.grd_ListParam.Size = new System.Drawing.Size(702, 255);
		this.grd_ListParam.TabIndex = 47;
		this.grd_ListParam.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListParam });
		this.grd_ListParam.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_ListParam_DragDrop);
		this.grd_ListParam.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_ListParam_DragOver);
		this.grd_ListParam.DoubleClick += new System.EventHandler(this.gv_ListParam_DoubleClick);
		this.grd_ListParam.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_ListParam_MouseDown);
		this.grd_ListParam.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_ListParam_MouseMove);
		this.gv_ListParam.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListParam.ColumnKey = null;
		this.gv_ListParam.DetailHeight = 431;
		this.gv_ListParam.DisableFormSetting = false;
		this.gv_ListParam.DisplayException = false;
		this.gv_ListParam.EditColumn = null;
		this.gv_ListParam.FieldCurrency = null;
		this.gv_ListParam.FieldRate = null;
		this.gv_ListParam.FormId = "oUserDefinedReport";
		this.gv_ListParam.GridControl = this.grd_ListParam;
		this.gv_ListParam.HasFormSetting = true;
		this.gv_ListParam.HeaderExcelExport = null;
		this.gv_ListParam.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_ListParam.IgnoreFilter = false;
		this.gv_ListParam.IndexEditRow = -1;
		this.gv_ListParam.IndicatorWidth = 41;
		this.gv_ListParam.IsApzTable = false;
		this.gv_ListParam.IsInsertNewRow = false;
		this.gv_ListParam.IsLoadingData = false;
		this.gv_ListParam.IsMappingContextAction = false;
		this.gv_ListParam.ItemId = "OLOP";
		this.gv_ListParam.LstColumnAllowBatch = null;
		this.gv_ListParam.LstColumnNotAllowBatch = null;
		this.gv_ListParam.LstColumnNotEditable = null;
		this.gv_ListParam.LstColumNotSetting = null;
		this.gv_ListParam.LstOldUDF = null;
		this.gv_ListParam.Name = "gv_ListParam";
		this.gv_ListParam.OptionsCustomization.AllowSort = false;
		this.gv_ListParam.OptionsView.BestFitMaxRowCount = 100;
		this.gv_ListParam.OptionsView.ColumnAutoWidth = false;
		this.gv_ListParam.OptionsView.ShowGroupPanel = false;
		this.gv_ListParam.ReadOnly = false;
		this.gv_ListParam.SettingReadonly = false;
		this.gv_ListParam.UseBatchMod = false;
		this.labelLine13.EditControl = null;
		this.labelLine13.EditValue = "Header Export Excel";
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(16, 9);
		this.labelLine13.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ReadOnly = false;
		this.labelLine13.SettingInVisible = false;
		this.labelLine13.SettingReadonly = false;
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(149, 25);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 3;
		this.labelLine13.Utext = "Header Export Excel";
		this.cbb_HeaderExcel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbb_HeaderExcel.FormattingEnabled = true;
		this.cbb_HeaderExcel.Items.AddRange(new object[3] { "None", "Split", "Merge" });
		this.cbb_HeaderExcel.Location = new System.Drawing.Point(173, 4);
		this.cbb_HeaderExcel.Name = "cbb_HeaderExcel";
		this.cbb_HeaderExcel.Size = new System.Drawing.Size(121, 24);
		this.cbb_HeaderExcel.TabIndex = 2;
		this.cbb_HeaderExcel.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
		this.txt_HeaderExcel.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Simple;
		this.txt_HeaderExcel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_HeaderExcel.Enabled = false;
		this.txt_HeaderExcel.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.txt_HeaderExcel.Location = new System.Drawing.Point(5, 41);
		this.txt_HeaderExcel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_HeaderExcel.Name = "txt_HeaderExcel";
		this.txt_HeaderExcel.Size = new System.Drawing.Size(696, 61);
		this.txt_HeaderExcel.TabIndex = 1;
		this.wp_File.Controls.Add(this.txt_FilePath);
		this.wp_File.Controls.Add(this.labelLine11);
		this.wp_File.Controls.Add(this.txt_FileName);
		this.wp_File.Controls.Add(this.labelLine10);
		this.wp_File.DescriptionText = "";
		this.wp_File.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wp_File.Name = "wp_File";
		this.wp_File.Size = new System.Drawing.Size(702, 446);
		this.wp_File.Text = "Link to report";
		this.txt_FilePath.EditControl = null;
		this.txt_FilePath.FieldName = null;
		this.txt_FilePath.IsSearchControl = false;
		this.txt_FilePath.Location = new System.Drawing.Point(105, 258);
		this.txt_FilePath.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_FilePath.Name = "txt_FilePath";
		this.txt_FilePath.SettingInVisible = false;
		this.txt_FilePath.SettingReadonly = false;
		this.txt_FilePath.Size = new System.Drawing.Size(530, 22);
		this.txt_FilePath.TabIndex = 40;
		this.txt_FilePath.Visible = false;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Report File";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(3, 258);
		this.labelLine11.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(107, 25);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 39;
		this.labelLine11.Utext = "Report File";
		this.labelLine11.Visible = false;
		this.txt_FileName.EditControl = null;
		this.txt_FileName.FieldName = null;
		this.txt_FileName.IsSearchControl = false;
		this.txt_FileName.Location = new System.Drawing.Point(105, 209);
		this.txt_FileName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txt_FileName.Name = "txt_FileName";
		this.txt_FileName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_FileName.Properties.Click += new System.EventHandler(this.buttonEdit1_Properties_Click);
		this.txt_FileName.SettingInVisible = false;
		this.txt_FileName.SettingReadonly = false;
		this.txt_FileName.Size = new System.Drawing.Size(530, 22);
		this.txt_FileName.TabIndex = 38;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Report File";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(3, 213);
		this.labelLine10.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(107, 25);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 37;
		this.labelLine10.Utext = "Report File";
		this.labelLine14.EditControl = null;
		this.labelLine14.EditValue = "Align";
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(322, 9);
		this.labelLine14.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ReadOnly = false;
		this.labelLine14.SettingInVisible = false;
		this.labelLine14.SettingReadonly = false;
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(71, 25);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 4;
		this.labelLine14.Utext = "Align";
		this.cbb_Align.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cbb_Align.FormattingEnabled = true;
		this.cbb_Align.Items.AddRange(new object[3] { "Left", "Center", "Right" });
		this.cbb_Align.Location = new System.Drawing.Point(399, 4);
		this.cbb_Align.Name = "cbb_Align";
		this.cbb_Align.Size = new System.Drawing.Size(121, 24);
		this.cbb_Align.TabIndex = 5;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(742, 624);
		base.Controls.Add(this.wiz_UDR);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oUserDefinedReport";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "frm_UDR";
		base.ShowInTaskbar = false;
		this.Text = "User-Defined Report Registration";
		base.Load += new System.EventHandler(this.frm_UDR_Load);
		((System.ComponentModel.ISupportInitialize)this.wiz_UDR).EndInit();
		this.wiz_UDR.ResumeLayout(false);
		this.wp_Operation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ListReport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListReport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_OperationType.Properties).EndInit();
		this.wp_Setting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.check_ShowChk.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_allRows.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ParentMenuId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_Close.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LogTable.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Log.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Delete.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Cancel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Find.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Update.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Add.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_PlaceBottom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Position.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MenuUID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Caption.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MenuItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TableName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		this.wp_StoreParam.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbx_QueryType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Store.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ListParam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListParam).EndInit();
		this.wp_File.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_FilePath.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FileName.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
