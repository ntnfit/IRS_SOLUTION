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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraWizard;

namespace Apzon.Forms.Commons.UDO;

public class frm_ORW : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private Enums.ImportDOREnum _currentAction;

	private Dictionary<string, Control> _dicControls;

	private string rs;

	private readonly string[] _colNotEdit = new string[19]
	{
		"Code", "Name", "DocEntry", "DocNum", "LineId", "Instance", "Canceled", "Transfered", "Status", "LogInst",
		"Object", "CreateDate", "CreateTime", "UpdateDate", "UserSign", "RequestStatus", "Creator", "DataSource", "UpdateTime"
	};

	private readonly string[] _colForFind = new string[5] { "Name", "Code", "DocNum", "CreateDate", "UpdateDate" };

	public Action ReloadUdo;

	private IContainer components;

	private WizardControlEx wiz_ORM;

	private WelcomeWizardPage welcomeWizardPage1;

	private DevExpress.XtraWizard.WizardPage wzp_SelectRegisMode;

	private CompletionWizardPage wzp_Finish;

	private Panel panel2;

	private Label label5;

	private RadioGroupEx rdg_OperationType;

	private DevExpress.XtraWizard.WizardPage wzp_LinkChildUser;

	private DevExpress.XtraWizard.WizardPage wzp_ModifyDefaultForm;

	private DevExpress.XtraWizard.WizardPage wzp_UISetting;

	private DevExpress.XtraWizard.WizardPage wzp_SetSer;

	private DevExpress.XtraWizard.WizardPage wzp_DefBasSet;

	private Panel panel1;

	private Label label1;

	private TextEditEx txt_Name;

	private TextEditEx txt_UniqueID;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private TextEditEx txt_TabNameLog;

	private LabelLine lb_TabLog;

	private CheckEditEx ck_YearTranfer;

	private CheckEditEx ck_Log;

	private CheckEditEx ck_Cancel;

	private CheckEditEx ck_Delete;

	private CheckEditEx ck_Find;

	private CheckEditEx ck_Update;

	private CheckEditEx ck_Add;

	private Panel panel3;

	private Label label2;

	private Panel panel5;

	private Label lblModifyDefault;

	private TextEditEx txt_MenuCaption;

	private LabelLine labelLine6;

	private Panel panel4;

	private Label label3;

	private GridControlEx grd_NameTable;

	private GridViewEx gv_NameTable;

	private GridColumnEx col_ObjectName;

	private GridColumnEx col_TableName;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private GridColumnEx col_TableDescription;

	private Panel panel6;

	private Label lblNameTable;

	private GridControlEx grd_Column;

	private GridViewEx gv_Column;

	private RepositoryItemLookUpEdit rep_luk_TargetField;

	private DevExpress.XtraWizard.WizardPage wzp_SetFieldChildTable;

	private GridControlEx grd_ChildNameTable;

	private GridViewEx gv_ChildNameTable;

	private GridColumnEx col_ColIsUsed;

	private GridColumnEx col_ColEdit;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

	private GridColumnEx ColAlias;

	private GridColumnEx gridColumn8;

	private Panel panel7;

	private Label label7;

	private LookUpEdit luk_TableNameChild;

	private GridControlEx grd_Table;

	private GridViewEx gv_Table;

	private GridColumnEx col_code;

	private GridColumnEx col_name;

	private GridColumnEx col_table;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit3;

	private TextEditEx txtTempApp;

	private LabelLine lbTempApp;

	private CheckEditEx ck_Approve;

	private CheckEditEx ck_DataArchi;

	private CheckEditEx ck_ManagerSeries;

	private GridColumnEx col_type;

	private RepositoryItemLookUpEdit lk_Type;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

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

	private DevExpress.XtraWizard.WizardPage wzp_ModifyFieldsFind;

	private GridControlEx grd_FieldsFind;

	private GridViewEx gv_FieldsFind;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit4;

	private Panel panel8;

	private Label lblModify;

	private RepositoryItemCheckEdit ck_Visible;

	private RepositoryItemCheckEdit ck_EditTable;

	private RepositoryItemCheckEdit ck_VisibledeForm;

	private RepositoryItemCheckEdit ck_EditabledeForm;

	private CheckEditEx ck_Close;

	private LookUpEdit Luk_Type;

	private RepositoryItemCheckedComboBoxEdit repositoryItemCheckedComboBoxEdit1;

	private ButtonEditEx btn_TableName;

	private RepositoryItemCheckEdit ck_FielsFind;

	private GridColumn col_Check;

	private RepositoryItemCheckEdit ck_CheckTabName;

	private CheckEditEx ck_menuItem;

	private CheckEditEx ck_CheckAll;

	private CheckEditEx ck_CheckAllNameTable;

	private CheckEditEx ck_CheckAllColumn;

	private CheckEditEx ck_CheckAllChildName;

	public frm_ORW()
	{
		InitializeComponent();
	}

	public void CheckField(bool check)
	{
		ck_Find.Checked = check;
		ck_Delete.Checked = check;
		ck_Cancel.Checked = check;
		ck_Close.Checked = check;
		ck_Log.Checked = check;
		ck_ManagerSeries.Checked = check;
		ck_YearTranfer.Checked = check;
		ck_DataArchi.Checked = check;
		ck_Approve.Checked = check;
	}

	private async void wiz_ORM_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		_ = 9;
		try
		{
			switch (e.Page.Name)
			{
			case "wzp_SelectRegisMode":
				switch (rdg_OperationType.SelectedIndex)
				{
				case 0:
					_currentAction = Enums.ImportDOREnum.Add;
					txt_UniqueID.Text = string.Empty;
					txt_Name.Text = string.Empty;
					btn_TableName.Text = string.Empty;
					txt_MenuCaption.Text = string.Empty;
					Luk_Type.EditValue = 1;
					CheckField(check: false);
					txt_UniqueID.Enabled = true;
					btn_TableName.ReadOnly = true;
					btn_TableName.Enabled = true;
					Luk_Type.Properties.ReadOnly = false;
					break;
				case 1:
					_currentAction = Enums.ImportDOREnum.Update;
					txt_UniqueID.Text = Function.ToString(gv_Table.GetFocusedRowCellValue("Code"));
					txt_Name.Text = Function.ToString(gv_Table.GetFocusedRowCellValue("Name"));
					btn_TableName.Text = Function.ToString(gv_Table.GetFocusedRowCellValue("TableName"));
					Luk_Type.EditValue = Function.ParseInt(gv_Table.GetFocusedRowCellValue("TYPE"));
					txt_UniqueID.Enabled = false;
					btn_TableName.Enabled = false;
					Luk_Type.Properties.ReadOnly = true;
					break;
				case 2:
					_currentAction = Enums.ImportDOREnum.Unregister;
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUnregisterObject, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						string text4 = Function.ToString(gv_Table.GetFocusedRowCellValue("Code"));
						if (string.IsNullOrEmpty(text4))
						{
							WinCommons.Error(Enums.MessageCode.ObjectCodeIsEmpty, "");
							break;
						}
						HttpResult httpResult2 = await _httpClient.ORW.Unregister(text4);
						if (!string.IsNullOrEmpty(httpResult2.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
							break;
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (ReloadUdo != null)
						{
							ReloadUdo();
						}
						e.Handled = true;
						Close();
					}
					else
					{
						e.Handled = true;
					}
					break;
				case 3:
					_currentAction = Enums.ImportDOREnum.Delete;
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteObjectAndTable, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						string text3 = Function.ToString(gv_Table.GetFocusedRowCellValue("Code"));
						if (string.IsNullOrEmpty(text3))
						{
							WinCommons.Error(Enums.MessageCode.ObjectCodeIsEmpty, "");
							break;
						}
						HttpResult httpResult = await _httpClient.ORW.Delete(text3);
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
							break;
						}
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (ReloadUdo != null)
						{
							ReloadUdo();
						}
						e.Handled = true;
						Close();
					}
					else
					{
						e.Handled = true;
					}
					break;
				}
				break;
			case "wzp_DefBasSet":
				if (_currentAction == Enums.ImportDOREnum.Add)
				{
					if (btn_TableName.Text == "" || txt_Name.Text == "" || txt_UniqueID.Text == "" || Luk_Type.EditValue == null)
					{
						WinCommons.Error(Enums.MessageCode.YouMustToFillInfomation, "");
						e.Handled = true;
					}
					if (Luk_Type.Text == "Master Data")
					{
						ck_Close.Visible = false;
						ck_ManagerSeries.Visible = false;
						ck_DataArchi.Visible = false;
						ck_Approve.Visible = false;
					}
					txt_TabNameLog.Text = "A" + btn_TableName.Text;
				}
				if (_currentAction == Enums.ImportDOREnum.Update)
				{
					if (Luk_Type.Text == "Master Data")
					{
						ck_Close.Visible = false;
						ck_ManagerSeries.Visible = false;
						ck_DataArchi.Visible = false;
						ck_Approve.Visible = false;
					}
					if (btn_TableName.Text == "" || txt_Name.Text == "" || txt_UniqueID.Text == "" || Luk_Type.EditValue == null)
					{
						WinCommons.Error(Enums.MessageCode.YouMustToFillInfomation, "");
						e.Handled = true;
					}
					char? c = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanDelete"));
					char? c2 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanClose"));
					char? c3 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanCancel"));
					char? c4 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanFind"));
					char? c5 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanYrTrnsf"));
					char? c6 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanLog"));
					char? c7 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanArchive"));
					char? c8 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanApprove"));
					Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanNewForm"));
					char? c9 = Function.ParseChar(gv_Table.GetFocusedRowCellValue("CanDefForm"));
					string text = Function.ToString(gv_Table.GetFocusedRowCellValue("LogTable"));
					txt_TabNameLog.Text = text;
					ck_ManagerSeries.Enabled = false;
					ck_Add.Enabled = false;
					ck_Update.Enabled = false;
					ck_YearTranfer.Checked = c5 == 'Y';
					ck_Log.Checked = c6 == 'Y';
					ck_Delete.Checked = c9 == 'Y';
					ck_Approve.Checked = c8 == 'Y';
					ck_DataArchi.Checked = c7 == 'Y';
					ck_Find.Checked = c4 == 'Y';
					ck_Delete.Checked = c == 'Y';
					ck_Close.Checked = c2 == 'Y';
					ck_Cancel.Checked = c3 == 'Y';
				}
				break;
			case "wzp_SetSer":
				if (_currentAction == Enums.ImportDOREnum.Update)
				{
					string text2 = Function.ToString(gv_Table.GetFocusedRowCellValue("MenuCapt"));
					txt_MenuCaption.Text = text2;
				}
				else
				{
					txt_MenuCaption.Text = "";
				}
				break;
			case "wzp_UISetting":
				if (txt_MenuCaption.Text == "")
				{
					WinCommons.Error(Enums.MessageCode.YouMustToFillInfomation, "");
					e.Handled = true;
					break;
				}
				wzp_ModifyFieldsFind.Visible = ck_Find.Checked;
				if (_currentAction == Enums.ImportDOREnum.Add)
				{
					lblModify.Text = "Setting the fields for Find";
					lblModifyDefault.Text = "Setting the fields for the default form";
					await GetFieldsForFind(btn_TableName.Text, "A");
					await GetFieldsForForm(btn_TableName.Text, "A");
				}
				if (_currentAction == Enums.ImportDOREnum.Update)
				{
					lblModify.Text = "Modifying the fields for Find";
					lblModifyDefault.Text = "Modifying fields for the default form";
					await GetFieldsForFind(btn_TableName.Text, "U");
					await GetFieldsForForm(btn_TableName.Text, "U");
				}
				break;
			case "wzp_ModifyDefaultForm":
			{
				int objtype = Function.ParseInt(Luk_Type.EditValue);
				string colUDO = txt_UniqueID.Text;
				await GetcolEditTableUDO1(objtype, colUDO);
				if (_currentAction == Enums.ImportDOREnum.Add)
				{
					lblNameTable.Text = "Linking child user tables";
				}
				if (_currentAction == Enums.ImportDOREnum.Update)
				{
					lblNameTable.Text = "Linking additional child user tables";
				}
				break;
			}
			case "wzp_LinkChildUser":
			{
				WinCommons._OpenProcessing();
				DataTable dataTable = grd_NameTable.DataSourceTable();
				DataRow dataRow = (from n in dataTable.AsEnumerable()
					where n.Field<string>("IsCheck") == "Y"
					select n).FirstOrDefault();
				grd_ChildNameTable.DataSource = new DataTable();
				if (dataRow != null)
				{
					DataTable dtchild = (from p in dataTable.AsEnumerable()
						where Function.ToString(p["IsCheck"]).Equals("Y")
						select p).CopyToDataTable();
					DataColumn column = new DataColumn("SonNum", typeof(int));
					dtchild.Columns.Add(column);
					for (int num = 0; num < dtchild.Rows.Count; num++)
					{
						dtchild.Rows[num]["SonNum"] = num + 1;
					}
					luk_TableNameChild.Properties.DataSource = dtchild;
					luk_TableNameChild.Properties.ValueMember = "TableName";
					luk_TableNameChild.Properties.DisplayMember = "TableName";
					foreach (DataRow row2 in dtchild.Rows)
					{
						string colchild = Function.ToString(row2["TableName"]);
						int sonNum = Function.ParseInt(row2["SonNum"]);
						await GetTableColumChild(colchild, sonNum);
					}
					luk_TableNameChild.EditValue = dtchild.Rows[0]["TableName"];
				}
				else
				{
					DataTable dataTable2 = dataTable.Clone();
					DataRow row = dataTable2.NewRow();
					dataTable2.Rows.Add(row);
					luk_TableNameChild.Properties.DataSource = dataTable2;
					luk_TableNameChild.Properties.ValueMember = "TableName";
					luk_TableNameChild.Properties.DisplayMember = "TableName";
					DataSet ds = await _httpClient.ORW.GetFieldsChild("");
					await GridViewHelper.GenerateGrid(grd_ChildNameTable, gv_ChildNameTable, ds);
					ds.DisposeDataSet();
				}
				WinCommons._CloseProcessing();
				break;
			}
			case "wzp_ModifyFieldsFind":
				break;
			case "wzp_SetFieldChildTable":
				break;
			case "wzp_Finish":
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_ORW_Load(object sender, EventArgs e)
	{
		try
		{
			ck_Find.Checked = false;
			ck_Delete.Checked = false;
			ck_Cancel.Checked = false;
			ck_Close.Checked = false;
			ck_Log.Checked = false;
			ck_ManagerSeries.Checked = false;
			ck_YearTranfer.Checked = false;
			ck_DataArchi.Checked = false;
			ck_Approve.Checked = false;
			SetDictionaryControls();
			SetDataType();
			await RefreshTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rdg_OperationType_SelectedIndexChanged(object sender, EventArgs e)
	{
		switch (rdg_OperationType.SelectedIndex)
		{
		case 0:
			_currentAction = Enums.ImportDOREnum.Add;
			wiz_ORM.NextText = "Next";
			grd_Table.Visible = false;
			break;
		case 1:
			_currentAction = Enums.ImportDOREnum.Update;
			wiz_ORM.NextText = "Next";
			grd_Table.Visible = true;
			await RefreshTable();
			break;
		case 2:
			_currentAction = Enums.ImportDOREnum.Unregister;
			wiz_ORM.NextText = "Finish";
			grd_Table.Visible = true;
			await RefreshTable();
			break;
		case 3:
			_currentAction = Enums.ImportDOREnum.Delete;
			wiz_ORM.NextText = "Finish";
			grd_Table.Visible = true;
			await RefreshTable();
			break;
		}
	}

	private async Task RefreshTable()
	{
		try
		{
			Task<DataTable> tableNameOUDO = _httpClient.ORW.GetTableNameOUDO();
			GridControlEx gridControlEx = grd_Table;
			gridControlEx.DataSource = await tableNameOUDO;
			grd_Table.DataSourceTable().AcceptChanges();
			grd_Table.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetcolEditTableUDO1(int objtype, string colUDO1)
	{
		Task<DataTable> task = _httpClient.ORW.GetcolEditTableUDO1(objtype, colUDO1);
		GridControlEx gridControlEx = grd_NameTable;
		gridControlEx.DataSource = await task;
		grd_NameTable.DataSourceTable().AcceptChanges();
		grd_NameTable.RefreshDataSource();
	}

	private async Task GetTableColumChild(string colchild, int sonNum = 0)
	{
		_ = 1;
		try
		{
			DataTable dt = grd_ChildNameTable.DatasourceTable();
			DataSet ds = await _httpClient.ORW.GetFieldsChild(colchild);
			if (!ds.IsHasStructure() || !ds.Tables[1].IsNotNull())
			{
				return;
			}
			foreach (DataRow row in ds.Tables[1].Rows)
			{
				row["SonNum"] = sonNum;
			}
			if (!dt.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_ChildNameTable, gv_ChildNameTable, ds);
			}
			else
			{
				dt.Merge(ds.Tables[1], preserveChanges: false);
			}
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_TableNameChild_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataRowView dataRowView = luk_TableNameChild.Properties.GetDataSourceRowByKeyValue(luk_TableNameChild.EditValue) as DataRowView;
			int num = 0;
			if (dataRowView != null)
			{
				num = Function.ParseInt(dataRowView["SonNum"]);
				gv_ChildNameTable.ActiveFilterString = $"SonNum={num}";
				gv_ChildNameTable.OptionsView.ShowFilterPanelMode = ShowFilterPanelMode.Never;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ck_Log_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (ck_Log.Checked)
			{
				lb_TabLog.Visible = true;
				txt_TabNameLog.Visible = true;
			}
			else
			{
				lb_TabLog.Visible = false;
				txt_TabNameLog.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ck_Approve_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (ck_Approve.Checked)
			{
				lbTempApp.Visible = true;
				txtTempApp.Visible = true;
			}
			else
			{
				lbTempApp.Visible = false;
				txtTempApp.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetDataType()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Master Data",
				ID = 1
			});
			list.Add(new Common.Table_ID
			{
				Name = "Document",
				ID = 3
			});
			Luk_Type.Properties.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Table_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_NameTable_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ChildNameTable_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetControlsValueORW()
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("OUDO");
		DataRow dataRow = tableStructure.NewRow();
		try
		{
			dataRow["Code"] = txt_UniqueID.Text;
			dataRow["Name"] = txt_Name.Text;
			dataRow["TableName"] = btn_TableName.Text;
			dataRow["LogTable"] = txt_TabNameLog.Text;
			dataRow["TYPE"] = Luk_Type.EditValue;
			dataRow["MngSeries"] = ck_ManagerSeries.EditValue;
			dataRow["CanDelete"] = ck_Delete.EditValue;
			dataRow["CanClose"] = ck_Close.EditValue;
			dataRow["CanCancel"] = ck_Cancel.EditValue;
			dataRow["ExtName"] = DBNull.Value;
			dataRow["CanFind"] = ck_Find.EditValue;
			dataRow["CanYrTrnsf"] = ck_YearTranfer.EditValue;
			dataRow["CanDefForm"] = 'Y';
			dataRow["CanLog"] = ck_Log.EditValue;
			dataRow["OvrWrtDll"] = 'Y';
			dataRow["UIDFormat"] = 'Y';
			dataRow["CanArchive"] = ck_DataArchi.EditValue;
			dataRow["MenuItem"] = ck_menuItem.EditValue;
			dataRow["MenuCapt"] = txt_MenuCaption.Text;
			dataRow["FatherMenu"] = DBNull.Value;
			dataRow["Position"] = DBNull.Value;
			dataRow["CanNewForm"] = 'Y';
			dataRow["IsRebuild"] = DBNull.Value;
			dataRow["NewFormSrf"] = DBNull.Value;
			dataRow["MenuUid"] = txt_UniqueID.Text;
			dataRow["LstUpdDate"] = DBNull.Value;
			dataRow["LstUpdTime"] = DBNull.Value;
			dataRow["CanApprove"] = ck_Approve.EditValue;
			dataRow["TemplateID"] = DBNull.Value;
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return tableStructure;
	}

	private void SaveData()
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = GetControlsValueORW().Copy();
			DataTable dataTable2 = new DataTable();
			DataTable dataTable3 = new DataTable();
			DataTable dataTable4 = new DataTable();
			DataTable dataTable5 = new DataTable();
			if (luk_TableNameChild.Properties.DataSource != null)
			{
				dataTable2 = ((DataTable)luk_TableNameChild.Properties.DataSource).Copy();
			}
			if (grd_FieldsFind.DataSource != null)
			{
				dataTable3 = grd_FieldsFind.DataSourceTable().Copy();
			}
			if (grd_Column.DataSource != null)
			{
				dataTable4 = grd_Column.DataSourceTable().Copy();
			}
			if (grd_ChildNameTable.DataSource != null)
			{
				dataTable5 = grd_ChildNameTable.DataSourceTable().Copy();
			}
			dataTable.TableName = "dtOUDO";
			dataTable2.TableName = "dtODU1";
			dataTable3.TableName = "dtODU2";
			dataTable4.TableName = "dtODU3";
			dataTable5.TableName = "dtODU4";
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			dataSet.Tables.Add(dataTable3);
			dataSet.Tables.Add(dataTable4);
			dataSet.Tables.Add(dataTable5);
			switch (_currentAction)
			{
			case Enums.ImportDOREnum.Add:
				rs = _httpClient.ORW.SaveORW(dataSet);
				break;
			case Enums.ImportDOREnum.Update:
				rs = _httpClient.ORW.SaveORWUpdate(dataSet);
				break;
			}
			if (string.IsNullOrEmpty(rs))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (ReloadUdo != null)
				{
					ReloadUdo();
				}
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, rs);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDictionaryControls()
	{
		try
		{
			_dicControls = new Dictionary<string, Control>();
			_dicControls.Add("OUDO.Code", txt_UniqueID);
			_dicControls.Add("OUDO.Name", txt_Name);
			_dicControls.Add("OUDO.TYPE", Luk_Type);
			_dicControls.Add("OUDO.TableName", btn_TableName);
			_dicControls.Add("OUDO.CanFind", ck_Find);
			_dicControls.Add("OUDO.CanDelete", ck_Delete);
			_dicControls.Add("OUDO.CanCancel", ck_Cancel);
			_dicControls.Add("OUDO.CanClose", ck_Close);
			_dicControls.Add("OUDO.CanLog", ck_Log);
			_dicControls.Add("OUDO.MngSeries", ck_ManagerSeries);
			_dicControls.Add("OUDO.MenuItem", ck_menuItem);
			_dicControls.Add("OUDO.CanYrTrnsf", ck_YearTranfer);
			_dicControls.Add("OUDO.CanArchive", ck_DataArchi);
			_dicControls.Add("OUDO.CanApprove", ck_Approve);
			_dicControls.Add("OUDO.MenuCapt", txt_MenuCaption);
			_dicControls.Add("OUDO.MenuUid", txt_UniqueID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveORW()
	{
		_ = 1;
		try
		{
			DataTable dt = grd_Table.DataSourceTable();
			string code = Function.ToString(gv_Table.GetFocusedRowCellValue("Code"));
			string text = await _httpClient.ORW.RemoveObjectRegistration(code);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Table.BeginUpdate();
				gv_Table.DeleteSelectedRows();
				gv_Table.EndUpdate();
				dt.AcceptChanges();
				grd_Table.RefreshDataSource();
				await RefreshTable();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void DeleteDT()
	{
		try
		{
			DataTable dt = grd_Table.DataSourceTable();
			string tableName = Function.ToString(gv_Table.GetFocusedRowCellValue("LogTable"));
			string text = await _httpClient.ORW.DeleteDT(tableName);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_Table.BeginUpdate();
				gv_Table.EndUpdate();
				dt.AcceptChanges();
				grd_Table.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void wiz_ORM_FinishClick(object sender, CancelEventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				SaveData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void wiz_ORM_CancelClick(object sender, CancelEventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_FieldsFind_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_FieldsFind.GetFocusedRowCellValue("ColAlias"));
			if (gv_FieldsFind.FocusedColumn.FieldName.Equals("IsCheck") && _colForFind.Contains(value))
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

	private void gv_FieldsFind_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_FieldsFind.GetRowCellValue(e.RowHandle, "ColAlias"));
			if (e.Column.FieldName == "IsCheck" && _colForFind.Contains(value))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ck_CheckAll_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_FieldsFind.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (!_colForFind.Contains(Function.ToString(item["ColAlias"])))
				{
					item["IsCheck"] = ck_CheckAll.Checked;
				}
			}
			ck_CheckAll.Text = (ck_CheckAll.Checked ? "Unselect" : "Select") + " All";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Column_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Column.GetRowCellValue(e.RowHandle, "ColAlias"));
			if (e.Column.FieldName == "IsVisible" && (text.Equals("Code") || text.Equals("DocEntry")))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
			if (e.Column.FieldName == "IsEdit" && _colNotEdit.Contains(text))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Column_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Column.GetFocusedRowCellValue("ColAlias"));
			if (text.Equals("Code") || text.Equals("DocEntry"))
			{
				if (gv_Column.FocusedColumn.FieldName == "IsVisible")
				{
					e.Cancel = true;
				}
				if (gv_Column.FocusedColumn.FieldName == "IsEdit")
				{
					e.Cancel = true;
				}
			}
			if (_colNotEdit.Contains(text) && gv_Column.FocusedColumn.FieldName == "IsEdit")
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

	private void gv_Column_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("IsEdit"))
			{
				string value = Function.ToString(gv_Column.GetRowCellValue(e.RowHandle, "ColAlias"));
				if (!(bool)gv_Column.GetRowCellValue(e.RowHandle, "IsVisible") || _colNotEdit.Contains(value))
				{
					gv_Column.SetRowCellValue(e.RowHandle, "IsEdit", false);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Column_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("IsVisible"))
			{
				string text = Function.ToString(gv_Column.GetRowCellValue(e.RowHandle, "ColAlias"));
				bool flag = (bool)gv_Column.GetRowCellValue(e.RowHandle, "IsVisible");
				if (!flag && (text.Equals("Code") || text.Equals("DocEntry")))
				{
					gv_Column.SetRowCellValue(e.RowHandle, "IsVisible", true);
				}
				else if (!_colNotEdit.Contains(text))
				{
					gv_Column.SetRowCellValue(e.RowHandle, "IsEdit", flag);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ck_CheckAllColumn_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Column.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (!Function.ToString(item["ColAlias"]).Contains("Docentry"))
				{
					if (!_colNotEdit.Contains(Function.ToString(item["ColAlias"])))
					{
						item["IsVisible"] = ck_CheckAllColumn.Checked;
						item["IsEdit"] = ck_CheckAllColumn.Checked;
					}
					else
					{
						item["IsVisible"] = ck_CheckAllColumn.Checked;
					}
				}
			}
			ck_CheckAllColumn.Text = (ck_CheckAllColumn.Checked ? "Unselect" : "Select") + " All";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ChildNameTable_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_ChildNameTable.GetRowCellValue(e.RowHandle, "ColAlias"));
			if (e.Column.FieldName == "ColEdit" && _colNotEdit.Contains(value))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_ChildNameTable_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_ChildNameTable.GetFocusedRowCellValue("ColAlias"));
			if (_colNotEdit.Contains(value) && gv_ChildNameTable.FocusedColumn.FieldName == "ColEdit")
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

	private void gv_ChildNameTable_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("ColIsUsed"))
			{
				string value = Function.ToString(gv_ChildNameTable.GetRowCellValue(e.RowHandle, "ColAlias"));
				bool flag = Function.ParseBool(gv_ChildNameTable.GetRowCellValue(e.RowHandle, "ColIsUsed"));
				if (!_colNotEdit.Contains(value))
				{
					gv_ChildNameTable.SetRowCellValue(e.RowHandle, "ColEdit", flag);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ck_CheckAllChildName_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_ChildNameTable.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataRowView dataRowView = luk_TableNameChild.Properties.GetDataSourceRowByKeyValue(luk_TableNameChild.EditValue) as DataRowView;
			int num = 0;
			if (dataRowView == null)
			{
				return;
			}
			num = Function.ParseInt(dataRowView["SonNum"]);
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (Function.ParseInt(item["SonNum"]) == num)
				{
					if (!_colNotEdit.Contains(Function.ToString(item["ColAlias"])))
					{
						item["ColIsUsed"] = ck_CheckAllChildName.Checked;
						item["ColEdit"] = ck_CheckAllChildName.Checked;
					}
					else
					{
						item["ColIsUsed"] = ck_CheckAllChildName.Checked;
					}
				}
			}
			ck_CheckAllChildName.Text = (ck_CheckAllChildName.Checked ? "Unselect" : "Select") + " All";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ck_CheckAllNameTable_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_NameTable.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			if (ck_CheckAllNameTable.Checked)
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					item["IsCheck"] = 'Y';
				}
				ck_CheckAllNameTable.Text = "Unselect All";
				return;
			}
			foreach (DataRow item2 in dataTable.AsEnumerable())
			{
				item2["IsCheck"] = 'N';
			}
			ck_CheckAllNameTable.Text = "Select All";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_TableName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetTableNameUDO, false, Function.ToString(btn_TableName.EditValue), "", Enums.ItemObjectType.NoItemType, null, "", Function.ToString(Luk_Type.EditValue));
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					btn_TableName.EditValue = frm_CFL2.ChooseItem["TableName"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void btn_TableName_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Tab)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetTableNameUDO, false, Function.ToString(btn_TableName.EditValue), "", Enums.ItemObjectType.NoItemType, null, "", Function.ToString(Luk_Type.EditValue));
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					btn_TableName.EditValue = frm_CFL2.ChooseItem["TableName"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetFieldsForFind(string tableName, string type)
	{
		_ = 1;
		try
		{
			DataSet ds = await _httpClient.ORW.GetFieldsForFind(tableName, type);
			if (ds.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_FieldsFind, gv_FieldsFind, ds);
			}
			ds.DisposeDataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetFieldsForForm(string tableName, string type)
	{
		_ = 1;
		try
		{
			DataSet ds = await _httpClient.ORW.GetFieldsForForm(tableName, type);
			if (ds.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_Column, gv_Column, ds);
			}
			ds.DisposeDataSet();
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
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		this.wiz_ORM = new Apzon.Controls.Controls.WizardControlEx();
		this.welcomeWizardPage1 = new DevExpress.XtraWizard.WelcomeWizardPage();
		this.wzp_SelectRegisMode = new DevExpress.XtraWizard.WizardPage();
		this.grd_Table = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Table = new Apzon.Controls.Grid.GridViewEx();
		this.col_code = new Apzon.Controls.Grid.GridColumnEx();
		this.col_name = new Apzon.Controls.Grid.GridColumnEx();
		this.col_table = new Apzon.Controls.Grid.GridColumnEx();
		this.col_type = new Apzon.Controls.Grid.GridColumnEx();
		this.lk_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.panel2 = new System.Windows.Forms.Panel();
		this.label5 = new System.Windows.Forms.Label();
		this.rdg_OperationType = new Apzon.Controls.Editor.RadioGroupEx();
		this.wzp_Finish = new DevExpress.XtraWizard.CompletionWizardPage();
		this.wzp_LinkChildUser = new DevExpress.XtraWizard.WizardPage();
		this.ck_CheckAllNameTable = new Apzon.Controls.Editor.CheckEditEx();
		this.grd_NameTable = new Apzon.Controls.Grid.GridControlEx();
		this.gv_NameTable = new Apzon.Controls.Grid.GridViewEx();
		this.col_Check = new DevExpress.XtraGrid.Columns.GridColumn();
		this.ck_CheckTabName = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.col_ObjectName = new Apzon.Controls.Grid.GridColumnEx();
		this.col_TableName = new Apzon.Controls.Grid.GridColumnEx();
		this.col_TableDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.panel6 = new System.Windows.Forms.Panel();
		this.lblNameTable = new System.Windows.Forms.Label();
		this.wzp_ModifyDefaultForm = new DevExpress.XtraWizard.WizardPage();
		this.ck_CheckAllColumn = new Apzon.Controls.Editor.CheckEditEx();
		this.grd_Column = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Column = new Apzon.Controls.Grid.GridViewEx();
		this.rep_luk_TargetField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.ck_Visible = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.ck_EditTable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.panel5 = new System.Windows.Forms.Panel();
		this.lblModifyDefault = new System.Windows.Forms.Label();
		this.wzp_UISetting = new DevExpress.XtraWizard.WizardPage();
		this.ck_menuItem = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_MenuCaption = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.panel4 = new System.Windows.Forms.Panel();
		this.label3 = new System.Windows.Forms.Label();
		this.wzp_SetSer = new DevExpress.XtraWizard.WizardPage();
		this.ck_Close = new Apzon.Controls.Editor.CheckEditEx();
		this.txtTempApp = new Apzon.Controls.Editor.TextEditEx();
		this.lbTempApp = new Apzon.Controls.Controls.LabelLine();
		this.ck_Approve = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_DataArchi = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_ManagerSeries = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_TabNameLog = new Apzon.Controls.Editor.TextEditEx();
		this.lb_TabLog = new Apzon.Controls.Controls.LabelLine();
		this.ck_YearTranfer = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Log = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Cancel = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Delete = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Find = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Update = new Apzon.Controls.Editor.CheckEditEx();
		this.ck_Add = new Apzon.Controls.Editor.CheckEditEx();
		this.panel3 = new System.Windows.Forms.Panel();
		this.label2 = new System.Windows.Forms.Label();
		this.wzp_DefBasSet = new DevExpress.XtraWizard.WizardPage();
		this.btn_TableName = new Apzon.Controls.Editor.ButtonEditEx();
		this.Luk_Type = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.txt_UniqueID = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.panel1 = new System.Windows.Forms.Panel();
		this.label1 = new System.Windows.Forms.Label();
		this.wzp_SetFieldChildTable = new DevExpress.XtraWizard.WizardPage();
		this.ck_CheckAllChildName = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_TableNameChild = new DevExpress.XtraEditors.LookUpEdit();
		this.grd_ChildNameTable = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ChildNameTable = new Apzon.Controls.Grid.GridViewEx();
		this.col_ColIsUsed = new Apzon.Controls.Grid.GridColumnEx();
		this.ck_VisibledeForm = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.col_ColEdit = new Apzon.Controls.Grid.GridColumnEx();
		this.ck_EditabledeForm = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.ColAlias = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repositoryItemCheckedComboBoxEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.panel7 = new System.Windows.Forms.Panel();
		this.label7 = new System.Windows.Forms.Label();
		this.wzp_ModifyFieldsFind = new DevExpress.XtraWizard.WizardPage();
		this.ck_CheckAll = new Apzon.Controls.Editor.CheckEditEx();
		this.grd_FieldsFind = new Apzon.Controls.Grid.GridControlEx();
		this.gv_FieldsFind = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemLookUpEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.ck_FielsFind = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.panel8 = new System.Windows.Forms.Panel();
		this.lblModify = new System.Windows.Forms.Label();
		((System.ComponentModel.ISupportInitialize)this.wiz_ORM).BeginInit();
		this.wiz_ORM.SuspendLayout();
		this.wzp_SelectRegisMode.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Table).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Table).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit3).BeginInit();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rdg_OperationType.Properties).BeginInit();
		this.wzp_LinkChildUser.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAllNameTable.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_NameTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_NameTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_CheckTabName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		this.panel6.SuspendLayout();
		this.wzp_ModifyDefaultForm.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAllColumn.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Column).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Column).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_TargetField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Visible).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_EditTable).BeginInit();
		this.panel5.SuspendLayout();
		this.wzp_UISetting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_menuItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MenuCaption.Properties).BeginInit();
		this.panel4.SuspendLayout();
		this.wzp_SetSer.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_Close.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTempApp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Approve.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_DataArchi.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_ManagerSeries.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TabNameLog.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_YearTranfer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Log.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Cancel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Delete.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Find.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Update.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Add.Properties).BeginInit();
		this.panel3.SuspendLayout();
		this.wzp_DefBasSet.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_TableName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Luk_Type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UniqueID.Properties).BeginInit();
		this.panel1.SuspendLayout();
		this.wzp_SetFieldChildTable.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAllChildName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TableNameChild.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ChildNameTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ChildNameTable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_VisibledeForm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_EditabledeForm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckedComboBoxEdit1).BeginInit();
		this.panel7.SuspendLayout();
		this.wzp_ModifyFieldsFind.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAll.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_FieldsFind).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_FieldsFind).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.ck_FielsFind).BeginInit();
		this.panel8.SuspendLayout();
		base.SuspendLayout();
		this.wiz_ORM.AllowPagePadding = false;
		this.wiz_ORM.Controls.Add(this.welcomeWizardPage1);
		this.wiz_ORM.Controls.Add(this.wzp_SelectRegisMode);
		this.wiz_ORM.Controls.Add(this.wzp_Finish);
		this.wiz_ORM.Controls.Add(this.wzp_LinkChildUser);
		this.wiz_ORM.Controls.Add(this.wzp_ModifyDefaultForm);
		this.wiz_ORM.Controls.Add(this.wzp_UISetting);
		this.wiz_ORM.Controls.Add(this.wzp_SetSer);
		this.wiz_ORM.Controls.Add(this.wzp_DefBasSet);
		this.wiz_ORM.Controls.Add(this.wzp_SetFieldChildTable);
		this.wiz_ORM.Controls.Add(this.wzp_ModifyFieldsFind);
		this.wiz_ORM.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wiz_ORM.Name = "wiz_ORM";
		this.wiz_ORM.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[10] { this.welcomeWizardPage1, this.wzp_SelectRegisMode, this.wzp_DefBasSet, this.wzp_SetSer, this.wzp_UISetting, this.wzp_ModifyFieldsFind, this.wzp_ModifyDefaultForm, this.wzp_LinkChildUser, this.wzp_SetFieldChildTable, this.wzp_Finish });
		this.wiz_ORM.Size = new System.Drawing.Size(595, 385);
		this.wiz_ORM.TitleHeight = 0;
		this.wiz_ORM.CancelClick += new System.ComponentModel.CancelEventHandler(this.wiz_ORM_CancelClick);
		this.wiz_ORM.FinishClick += new System.ComponentModel.CancelEventHandler(this.wiz_ORM_FinishClick);
		this.wiz_ORM.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_ORM_NextClick);
		this.welcomeWizardPage1.IntroductionText = "";
		this.welcomeWizardPage1.Name = "welcomeWizardPage1";
		this.welcomeWizardPage1.ProceedText = "";
		this.welcomeWizardPage1.Size = new System.Drawing.Size(410, 269);
		this.welcomeWizardPage1.Text = "User - Defined Object Registration";
		this.wzp_SelectRegisMode.Controls.Add(this.grd_Table);
		this.wzp_SelectRegisMode.Controls.Add(this.panel2);
		this.wzp_SelectRegisMode.Controls.Add(this.rdg_OperationType);
		this.wzp_SelectRegisMode.Name = "wzp_SelectRegisMode";
		this.wzp_SelectRegisMode.Size = new System.Drawing.Size(595, 339);
		this.grd_Table.AllowPasteExcel = false;
		this.grd_Table.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Table.ClipboardData = null;
		this.grd_Table.Location = new System.Drawing.Point(12, 199);
		this.grd_Table.MainView = this.gv_Table;
		this.grd_Table.Name = "grd_Table";
		this.grd_Table.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemLookUpEdit3, this.lk_Type });
		this.grd_Table.Size = new System.Drawing.Size(573, 137);
		this.grd_Table.TabIndex = 12;
		this.grd_Table.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Table });
		this.grd_Table.Visible = false;
		this.gv_Table.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Table.ColumnKey = null;
		this.gv_Table.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[29]
		{
			this.col_code, this.col_name, this.col_table, this.col_type, this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20,
			this.gridColumn21, this.gridColumn22, this.gridColumn23, this.gridColumn24, this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28, this.gridColumn29
		});
		this.gv_Table.DisableFormSetting = false;
		this.gv_Table.DisplayException = false;
		this.gv_Table.EditColumn = null;
		this.gv_Table.FieldCurrency = null;
		this.gv_Table.FieldRate = null;
		this.gv_Table.FormId = "oUserDefinedObject";
		this.gv_Table.GridControl = this.grd_Table;
		this.gv_Table.IgnoreFilter = false;
		this.gv_Table.IndexEditRow = -1;
		this.gv_Table.IndicatorWidth = 40;
		this.gv_Table.IsApzTable = false;
		this.gv_Table.IsInsertNewRow = false;
		this.gv_Table.IsLoadingData = false;
		this.gv_Table.IsMappingContextAction = false;
		this.gv_Table.ItemId = "OTABLE";
		this.gv_Table.LstColumnAllowBatch = null;
		this.gv_Table.LstColumnNotAllowBatch = null;
		this.gv_Table.LstColumnNotEditable = null;
		this.gv_Table.LstColumNotSetting = null;
		this.gv_Table.Name = "gv_Table";
		this.gv_Table.OptionsView.ShowGroupPanel = false;
		this.gv_Table.UseBatchMod = false;
		this.gv_Table.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Table_CustomDrawRowIndicator);
		this.col_code.Caption = "Code";
		this.col_code.CurrencyDateField = null;
		this.col_code.CurrencyField = null;
		this.col_code.CurrencyFixed = null;
		this.col_code.CurrencyFixedRate = 0.0;
		this.col_code.CurrencyRateField = null;
		this.col_code.FieldName = "Code";
		this.col_code.Name = "col_code";
		this.col_code.OptionsColumn.ReadOnly = true;
		this.col_code.PrimaryKey = false;
		this.col_code.UnitField = null;
		this.col_code.Visible = true;
		this.col_code.VisibleIndex = 0;
		this.col_code.Width = 157;
		this.col_name.Caption = "Name";
		this.col_name.CurrencyDateField = null;
		this.col_name.CurrencyField = null;
		this.col_name.CurrencyFixed = null;
		this.col_name.CurrencyFixedRate = 0.0;
		this.col_name.CurrencyRateField = null;
		this.col_name.FieldName = "Name";
		this.col_name.Name = "col_name";
		this.col_name.OptionsColumn.ReadOnly = true;
		this.col_name.PrimaryKey = false;
		this.col_name.UnitField = null;
		this.col_name.Visible = true;
		this.col_name.VisibleIndex = 1;
		this.col_name.Width = 258;
		this.col_table.Caption = "Table";
		this.col_table.CurrencyDateField = null;
		this.col_table.CurrencyField = null;
		this.col_table.CurrencyFixed = null;
		this.col_table.CurrencyFixedRate = 0.0;
		this.col_table.CurrencyRateField = null;
		this.col_table.FieldName = "TableName";
		this.col_table.Name = "col_table";
		this.col_table.OptionsColumn.ReadOnly = true;
		this.col_table.PrimaryKey = false;
		this.col_table.UnitField = null;
		this.col_table.Visible = true;
		this.col_table.VisibleIndex = 2;
		this.col_table.Width = 619;
		this.col_type.Caption = "Type";
		this.col_type.ColumnEdit = this.lk_Type;
		this.col_type.CurrencyDateField = null;
		this.col_type.CurrencyField = null;
		this.col_type.CurrencyFixed = null;
		this.col_type.CurrencyFixedRate = 0.0;
		this.col_type.CurrencyRateField = null;
		this.col_type.FieldName = "TYPE";
		this.col_type.Name = "col_type";
		this.col_type.OptionsColumn.ReadOnly = true;
		this.col_type.PrimaryKey = false;
		this.col_type.UnitField = null;
		this.lk_Type.AutoHeight = false;
		this.lk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lk_Type.DisplayMember = "ID";
		this.lk_Type.Name = "lk_Type";
		this.lk_Type.ValueMember = "Name";
		this.gridColumn1.Caption = "LogTable";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "LogTable";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "MngSeries";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "MngSeries";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "CanDelete";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "CanDelete";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn4.Caption = "CanClose";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "CanClose";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn9.Caption = "CanCancel";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "CanCancel";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn10.Caption = "ExtName";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "ExtName";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn11.Caption = "CanFind";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "CanFind";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn12.Caption = "CanYrTrnsf";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "CanYrTrnsf";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn13.Caption = "CanDelForm";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "CanDelForm";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "CanLog";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "CanLog";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "OvrWrtDll";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "OvrWrtDll";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.OptionsColumn.ReadOnly = true;
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "UIDFormat";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "UIDFormat";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn17.Caption = "CanArchive";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "CanArchive";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn18.Caption = "MenuItem";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "MenuItem";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn19.Caption = "MenuCapt";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "MenuCapt";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn20.Caption = "FatherMenu";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "FatherMenu";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsColumn.ReadOnly = true;
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn21.Caption = "Position";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Position";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsColumn.ReadOnly = true;
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn22.Caption = "CanNewForm";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "CanNewForm";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.OptionsColumn.ReadOnly = true;
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn23.Caption = "IsRebuild";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "IsRebuild";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.OptionsColumn.ReadOnly = true;
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn24.Caption = "NewFormSrf";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "NewFormSrf";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.OptionsColumn.ReadOnly = true;
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn25.Caption = "MenuUid";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "MenuUid";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.OptionsColumn.ReadOnly = true;
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn26.Caption = "LstUpdDate";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "LstUpdDate";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.OptionsColumn.ReadOnly = true;
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn27.Caption = "LstUpdTime";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "LstUpdTime";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.OptionsColumn.ReadOnly = true;
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn28.Caption = "CanApprove";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "CanApprove";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.OptionsColumn.ReadOnly = true;
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn29.Caption = "TemplateID";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "TemplateID";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.OptionsColumn.ReadOnly = true;
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.repositoryItemLookUpEdit3.AutoHeight = false;
		this.repositoryItemLookUpEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit3.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "Caption")
		});
		this.repositoryItemLookUpEdit3.DisplayMember = "Caption";
		this.repositoryItemLookUpEdit3.Name = "repositoryItemLookUpEdit3";
		this.repositoryItemLookUpEdit3.NullText = "";
		this.repositoryItemLookUpEdit3.ValueMember = "Name";
		this.panel2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel2.BackColor = System.Drawing.Color.White;
		this.panel2.Controls.Add(this.label5);
		this.panel2.Location = new System.Drawing.Point(3, 3);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(589, 85);
		this.panel2.TabIndex = 6;
		this.label5.AutoSize = true;
		this.label5.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label5.Location = new System.Drawing.Point(138, 4);
		this.label5.Name = "label5";
		this.label5.Size = new System.Drawing.Size(226, 19);
		this.label5.TabIndex = 1;
		this.label5.Text = "Selecting the Registration Mode";
		this.rdg_OperationType.EditControl = null;
		this.rdg_OperationType.FieldName = null;
		this.rdg_OperationType.IsSearchControl = false;
		this.rdg_OperationType.Location = new System.Drawing.Point(12, 94);
		this.rdg_OperationType.Name = "rdg_OperationType";
		this.rdg_OperationType.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.rdg_OperationType.Properties.Appearance.Options.UseBackColor = true;
		this.rdg_OperationType.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[4]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Add a New Object"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Update an existing object"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Unregister an existing object"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(null, "Delete an existing object")
		});
		this.rdg_OperationType.SettingInVisible = false;
		this.rdg_OperationType.SettingReadonly = false;
		this.rdg_OperationType.Size = new System.Drawing.Size(571, 97);
		this.rdg_OperationType.TabIndex = 4;
		this.rdg_OperationType.SelectedIndexChanged += new System.EventHandler(this.rdg_OperationType_SelectedIndexChanged);
		this.wzp_Finish.Name = "wzp_Finish";
		this.wzp_Finish.ProceedText = "";
		this.wzp_Finish.Size = new System.Drawing.Size(410, 246);
		this.wzp_Finish.Text = "Linking business logic implementation";
		this.wzp_LinkChildUser.Controls.Add(this.ck_CheckAllNameTable);
		this.wzp_LinkChildUser.Controls.Add(this.grd_NameTable);
		this.wzp_LinkChildUser.Controls.Add(this.panel6);
		this.wzp_LinkChildUser.Name = "wzp_LinkChildUser";
		this.wzp_LinkChildUser.Size = new System.Drawing.Size(595, 339);
		this.ck_CheckAllNameTable.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ck_CheckAllNameTable.EditControl = null;
		this.ck_CheckAllNameTable.EditValue = "N";
		this.ck_CheckAllNameTable.FieldName = null;
		this.ck_CheckAllNameTable.IsSearchControl = false;
		this.ck_CheckAllNameTable.Location = new System.Drawing.Point(8, 317);
		this.ck_CheckAllNameTable.Name = "ck_CheckAllNameTable";
		this.ck_CheckAllNameTable.Properties.Caption = "&Check All";
		this.ck_CheckAllNameTable.Properties.DisplayValueChecked = "Y";
		this.ck_CheckAllNameTable.Properties.DisplayValueGrayed = "N";
		this.ck_CheckAllNameTable.Properties.DisplayValueUnchecked = "N";
		this.ck_CheckAllNameTable.Properties.ValueChecked = "Y";
		this.ck_CheckAllNameTable.Properties.ValueGrayed = "N";
		this.ck_CheckAllNameTable.Properties.ValueUnchecked = "N";
		this.ck_CheckAllNameTable.SettingInVisible = false;
		this.ck_CheckAllNameTable.SettingReadonly = false;
		this.ck_CheckAllNameTable.Size = new System.Drawing.Size(94, 20);
		this.ck_CheckAllNameTable.TabIndex = 25;
		this.ck_CheckAllNameTable.CheckedChanged += new System.EventHandler(this.ck_CheckAllNameTable_CheckedChanged);
		this.grd_NameTable.AllowPasteExcel = false;
		this.grd_NameTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_NameTable.ClipboardData = null;
		this.grd_NameTable.Location = new System.Drawing.Point(8, 81);
		this.grd_NameTable.MainView = this.gv_NameTable;
		this.grd_NameTable.Name = "grd_NameTable";
		this.grd_NameTable.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemLookUpEdit1, this.ck_CheckTabName });
		this.grd_NameTable.Size = new System.Drawing.Size(584, 232);
		this.grd_NameTable.TabIndex = 13;
		this.grd_NameTable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_NameTable });
		this.gv_NameTable.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_NameTable.ColumnKey = null;
		this.gv_NameTable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.col_Check, this.col_ObjectName, this.col_TableName, this.col_TableDescription });
		this.gv_NameTable.DisableFormSetting = false;
		this.gv_NameTable.DisplayException = false;
		this.gv_NameTable.EditColumn = null;
		this.gv_NameTable.FieldCurrency = null;
		this.gv_NameTable.FieldRate = null;
		this.gv_NameTable.FormId = "oUserDefinedObject";
		this.gv_NameTable.GridControl = this.grd_NameTable;
		this.gv_NameTable.IgnoreFilter = false;
		this.gv_NameTable.IndexEditRow = -1;
		this.gv_NameTable.IndicatorWidth = 40;
		this.gv_NameTable.IsApzTable = false;
		this.gv_NameTable.IsInsertNewRow = false;
		this.gv_NameTable.IsLoadingData = false;
		this.gv_NameTable.IsMappingContextAction = false;
		this.gv_NameTable.ItemId = "ONAMETABLE";
		this.gv_NameTable.LstColumnAllowBatch = null;
		this.gv_NameTable.LstColumnNotAllowBatch = null;
		this.gv_NameTable.LstColumnNotEditable = null;
		this.gv_NameTable.LstColumNotSetting = null;
		this.gv_NameTable.Name = "gv_NameTable";
		this.gv_NameTable.OptionsView.ShowGroupPanel = false;
		this.gv_NameTable.UseBatchMod = false;
		this.gv_NameTable.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_NameTable_CustomDrawRowIndicator);
		this.col_Check.ColumnEdit = this.ck_CheckTabName;
		this.col_Check.FieldName = "IsCheck";
		this.col_Check.Name = "col_Check";
		this.col_Check.OptionsColumn.ShowCaption = false;
		this.col_Check.Visible = true;
		this.col_Check.VisibleIndex = 0;
		this.ck_CheckTabName.AutoHeight = false;
		this.ck_CheckTabName.DisplayValueChecked = "Y";
		this.ck_CheckTabName.DisplayValueGrayed = "N";
		this.ck_CheckTabName.DisplayValueUnchecked = "N";
		this.ck_CheckTabName.Name = "ck_CheckTabName";
		this.ck_CheckTabName.ValueChecked = "Y";
		this.ck_CheckTabName.ValueGrayed = "N";
		this.ck_CheckTabName.ValueUnchecked = "N";
		this.col_ObjectName.Caption = "Object Name";
		this.col_ObjectName.CurrencyDateField = null;
		this.col_ObjectName.CurrencyField = null;
		this.col_ObjectName.CurrencyFixed = null;
		this.col_ObjectName.CurrencyFixedRate = 0.0;
		this.col_ObjectName.CurrencyRateField = null;
		this.col_ObjectName.FieldName = "TableName";
		this.col_ObjectName.Name = "col_ObjectName";
		this.col_ObjectName.OptionsColumn.AllowEdit = false;
		this.col_ObjectName.OptionsColumn.ReadOnly = true;
		this.col_ObjectName.PrimaryKey = false;
		this.col_ObjectName.UnitField = null;
		this.col_ObjectName.Visible = true;
		this.col_ObjectName.VisibleIndex = 1;
		this.col_ObjectName.Width = 149;
		this.col_TableName.Caption = "Table Name";
		this.col_TableName.CurrencyDateField = null;
		this.col_TableName.CurrencyField = null;
		this.col_TableName.CurrencyFixed = null;
		this.col_TableName.CurrencyFixedRate = 0.0;
		this.col_TableName.CurrencyRateField = null;
		this.col_TableName.FieldName = "TableName";
		this.col_TableName.Name = "col_TableName";
		this.col_TableName.OptionsColumn.AllowEdit = false;
		this.col_TableName.OptionsColumn.ReadOnly = true;
		this.col_TableName.PrimaryKey = false;
		this.col_TableName.UnitField = null;
		this.col_TableName.Visible = true;
		this.col_TableName.VisibleIndex = 2;
		this.col_TableName.Width = 170;
		this.col_TableDescription.Caption = "Table Description";
		this.col_TableDescription.CurrencyDateField = null;
		this.col_TableDescription.CurrencyField = null;
		this.col_TableDescription.CurrencyFixed = null;
		this.col_TableDescription.CurrencyFixedRate = 0.0;
		this.col_TableDescription.CurrencyRateField = null;
		this.col_TableDescription.FieldName = "Descr";
		this.col_TableDescription.Name = "col_TableDescription";
		this.col_TableDescription.OptionsColumn.AllowEdit = false;
		this.col_TableDescription.OptionsColumn.ReadOnly = true;
		this.col_TableDescription.PrimaryKey = false;
		this.col_TableDescription.UnitField = null;
		this.col_TableDescription.Visible = true;
		this.col_TableDescription.VisibleIndex = 3;
		this.col_TableDescription.Width = 236;
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "Caption")
		});
		this.repositoryItemLookUpEdit1.DisplayMember = "Caption";
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit1.NullText = "";
		this.repositoryItemLookUpEdit1.ValueMember = "Name";
		this.panel6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel6.BackColor = System.Drawing.Color.White;
		this.panel6.Controls.Add(this.lblNameTable);
		this.panel6.Location = new System.Drawing.Point(3, 3);
		this.panel6.Name = "panel6";
		this.panel6.Size = new System.Drawing.Size(589, 72);
		this.panel6.TabIndex = 12;
		this.lblNameTable.AutoSize = true;
		this.lblNameTable.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblNameTable.Location = new System.Drawing.Point(171, 4);
		this.lblNameTable.Name = "lblNameTable";
		this.lblNameTable.Size = new System.Drawing.Size(0, 19);
		this.lblNameTable.TabIndex = 1;
		this.wzp_ModifyDefaultForm.Controls.Add(this.ck_CheckAllColumn);
		this.wzp_ModifyDefaultForm.Controls.Add(this.grd_Column);
		this.wzp_ModifyDefaultForm.Controls.Add(this.panel5);
		this.wzp_ModifyDefaultForm.Name = "wzp_ModifyDefaultForm";
		this.wzp_ModifyDefaultForm.Size = new System.Drawing.Size(595, 339);
		this.ck_CheckAllColumn.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ck_CheckAllColumn.EditControl = null;
		this.ck_CheckAllColumn.EditValue = "N";
		this.ck_CheckAllColumn.FieldName = null;
		this.ck_CheckAllColumn.IsSearchControl = false;
		this.ck_CheckAllColumn.Location = new System.Drawing.Point(8, 317);
		this.ck_CheckAllColumn.Name = "ck_CheckAllColumn";
		this.ck_CheckAllColumn.Properties.Caption = "&Check All";
		this.ck_CheckAllColumn.Properties.DisplayValueChecked = "Y";
		this.ck_CheckAllColumn.Properties.DisplayValueGrayed = "N";
		this.ck_CheckAllColumn.Properties.DisplayValueUnchecked = "N";
		this.ck_CheckAllColumn.Properties.ValueChecked = "Y";
		this.ck_CheckAllColumn.Properties.ValueGrayed = "N";
		this.ck_CheckAllColumn.Properties.ValueUnchecked = "N";
		this.ck_CheckAllColumn.SettingInVisible = false;
		this.ck_CheckAllColumn.SettingReadonly = false;
		this.ck_CheckAllColumn.Size = new System.Drawing.Size(92, 20);
		this.ck_CheckAllColumn.TabIndex = 25;
		this.ck_CheckAllColumn.CheckedChanged += new System.EventHandler(this.ck_CheckAllColumn_CheckedChanged);
		this.grd_Column.AllowPasteExcel = false;
		this.grd_Column.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Column.ClipboardData = null;
		this.grd_Column.Location = new System.Drawing.Point(8, 81);
		this.grd_Column.MainView = this.gv_Column;
		this.grd_Column.Name = "grd_Column";
		this.grd_Column.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_luk_TargetField, this.ck_Visible, this.ck_EditTable });
		this.grd_Column.Size = new System.Drawing.Size(584, 232);
		this.grd_Column.TabIndex = 11;
		this.grd_Column.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Column });
		this.gv_Column.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Column.ColumnKey = null;
		this.gv_Column.DisableFormSetting = false;
		this.gv_Column.DisplayException = false;
		this.gv_Column.EditColumn = null;
		this.gv_Column.FieldCurrency = null;
		this.gv_Column.FieldRate = null;
		this.gv_Column.FormId = "oUserDefinedObject";
		this.gv_Column.GridControl = this.grd_Column;
		this.gv_Column.IgnoreFilter = false;
		this.gv_Column.IndexEditRow = -1;
		this.gv_Column.IndicatorWidth = 40;
		this.gv_Column.IsApzTable = false;
		this.gv_Column.IsInsertNewRow = false;
		this.gv_Column.IsLoadingData = false;
		this.gv_Column.IsMappingContextAction = false;
		this.gv_Column.ItemId = "OFIELDCOLUMN";
		this.gv_Column.LstColumnAllowBatch = null;
		this.gv_Column.LstColumnNotAllowBatch = null;
		this.gv_Column.LstColumnNotEditable = null;
		this.gv_Column.LstColumNotSetting = null;
		this.gv_Column.Name = "gv_Column";
		this.gv_Column.OptionsView.ShowGroupPanel = false;
		this.gv_Column.UseBatchMod = false;
		this.gv_Column.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Column_RowCellStyle);
		this.gv_Column.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Column_ShowingEditor);
		this.gv_Column.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Column_CellValueChanged);
		this.gv_Column.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Column_CellValueChanging);
		this.rep_luk_TargetField.AutoHeight = false;
		this.rep_luk_TargetField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_TargetField.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "Caption")
		});
		this.rep_luk_TargetField.DisplayMember = "Caption";
		this.rep_luk_TargetField.Name = "rep_luk_TargetField";
		this.rep_luk_TargetField.NullText = "";
		this.rep_luk_TargetField.ValueMember = "Name";
		this.ck_Visible.AutoHeight = false;
		this.ck_Visible.DisplayValueChecked = "Y";
		this.ck_Visible.DisplayValueGrayed = "N";
		this.ck_Visible.DisplayValueUnchecked = "N";
		this.ck_Visible.Name = "ck_Visible";
		this.ck_Visible.ValueChecked = "Y";
		this.ck_Visible.ValueGrayed = "N";
		this.ck_Visible.ValueUnchecked = "N";
		this.ck_EditTable.AutoHeight = false;
		this.ck_EditTable.DisplayValueChecked = "Y";
		this.ck_EditTable.DisplayValueGrayed = "N";
		this.ck_EditTable.DisplayValueUnchecked = "N";
		this.ck_EditTable.Name = "ck_EditTable";
		this.ck_EditTable.ValueChecked = "Y";
		this.ck_EditTable.ValueGrayed = "N";
		this.ck_EditTable.ValueUnchecked = "N";
		this.panel5.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel5.BackColor = System.Drawing.Color.White;
		this.panel5.Controls.Add(this.lblModifyDefault);
		this.panel5.Location = new System.Drawing.Point(3, 3);
		this.panel5.Name = "panel5";
		this.panel5.Size = new System.Drawing.Size(589, 72);
		this.panel5.TabIndex = 10;
		this.lblModifyDefault.AutoSize = true;
		this.lblModifyDefault.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblModifyDefault.Location = new System.Drawing.Point(122, 16);
		this.lblModifyDefault.Name = "lblModifyDefault";
		this.lblModifyDefault.Size = new System.Drawing.Size(0, 19);
		this.lblModifyDefault.TabIndex = 1;
		this.wzp_UISetting.Controls.Add(this.ck_menuItem);
		this.wzp_UISetting.Controls.Add(this.txt_MenuCaption);
		this.wzp_UISetting.Controls.Add(this.labelLine6);
		this.wzp_UISetting.Controls.Add(this.panel4);
		this.wzp_UISetting.Name = "wzp_UISetting";
		this.wzp_UISetting.Size = new System.Drawing.Size(595, 339);
		this.ck_menuItem.EditControl = null;
		this.ck_menuItem.EditValue = 'Y';
		this.ck_menuItem.Enabled = false;
		this.ck_menuItem.FieldName = null;
		this.ck_menuItem.IsSearchControl = false;
		this.ck_menuItem.Location = new System.Drawing.Point(38, 123);
		this.ck_menuItem.Name = "ck_menuItem";
		this.ck_menuItem.Properties.Caption = "Menu &Item";
		this.ck_menuItem.Properties.ValueChecked = 'Y';
		this.ck_menuItem.Properties.ValueUnchecked = 'N';
		this.ck_menuItem.SettingInVisible = false;
		this.ck_menuItem.SettingReadonly = false;
		this.ck_menuItem.Size = new System.Drawing.Size(75, 20);
		this.ck_menuItem.TabIndex = 19;
		this.ck_menuItem.Visible = false;
		this.txt_MenuCaption.EditControl = null;
		this.txt_MenuCaption.FieldName = null;
		this.txt_MenuCaption.IsSearchControl = false;
		this.txt_MenuCaption.Location = new System.Drawing.Point(196, 155);
		this.txt_MenuCaption.Name = "txt_MenuCaption";
		this.txt_MenuCaption.SettingInVisible = false;
		this.txt_MenuCaption.SettingReadonly = false;
		this.txt_MenuCaption.Size = new System.Drawing.Size(213, 20);
		this.txt_MenuCaption.TabIndex = 16;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Menu Caption";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(82, 159);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(117, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 15;
		this.labelLine6.Utext = "Menu Caption";
		this.panel4.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel4.BackColor = System.Drawing.Color.White;
		this.panel4.Controls.Add(this.label3);
		this.panel4.Location = new System.Drawing.Point(3, 3);
		this.panel4.Name = "panel4";
		this.panel4.Size = new System.Drawing.Size(589, 77);
		this.panel4.TabIndex = 9;
		this.label3.AutoSize = true;
		this.label3.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label3.Location = new System.Drawing.Point(212, 4);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(77, 19);
		this.label3.TabIndex = 1;
		this.label3.Text = "UI Setting";
		this.wzp_SetSer.Controls.Add(this.ck_Close);
		this.wzp_SetSer.Controls.Add(this.txtTempApp);
		this.wzp_SetSer.Controls.Add(this.lbTempApp);
		this.wzp_SetSer.Controls.Add(this.ck_Approve);
		this.wzp_SetSer.Controls.Add(this.ck_DataArchi);
		this.wzp_SetSer.Controls.Add(this.ck_ManagerSeries);
		this.wzp_SetSer.Controls.Add(this.txt_TabNameLog);
		this.wzp_SetSer.Controls.Add(this.lb_TabLog);
		this.wzp_SetSer.Controls.Add(this.ck_YearTranfer);
		this.wzp_SetSer.Controls.Add(this.ck_Log);
		this.wzp_SetSer.Controls.Add(this.ck_Cancel);
		this.wzp_SetSer.Controls.Add(this.ck_Delete);
		this.wzp_SetSer.Controls.Add(this.ck_Find);
		this.wzp_SetSer.Controls.Add(this.ck_Update);
		this.wzp_SetSer.Controls.Add(this.ck_Add);
		this.wzp_SetSer.Controls.Add(this.panel3);
		this.wzp_SetSer.Name = "wzp_SetSer";
		this.wzp_SetSer.Size = new System.Drawing.Size(595, 339);
		this.ck_Close.EditControl = null;
		this.ck_Close.FieldName = null;
		this.ck_Close.IsSearchControl = false;
		this.ck_Close.Location = new System.Drawing.Point(50, 214);
		this.ck_Close.Name = "ck_Close";
		this.ck_Close.Properties.Caption = "&Close";
		this.ck_Close.Properties.DisplayValueChecked = "Y";
		this.ck_Close.Properties.DisplayValueGrayed = "N";
		this.ck_Close.Properties.DisplayValueUnchecked = "N";
		this.ck_Close.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Close.Properties.ValueChecked = "Y";
		this.ck_Close.Properties.ValueGrayed = "N";
		this.ck_Close.Properties.ValueUnchecked = "N";
		this.ck_Close.SettingInVisible = false;
		this.ck_Close.SettingReadonly = false;
		this.ck_Close.Size = new System.Drawing.Size(75, 20);
		this.ck_Close.TabIndex = 23;
		this.txtTempApp.EditControl = null;
		this.txtTempApp.FieldName = null;
		this.txtTempApp.IsSearchControl = false;
		this.txtTempApp.Location = new System.Drawing.Point(456, 159);
		this.txtTempApp.Name = "txtTempApp";
		this.txtTempApp.SettingInVisible = false;
		this.txtTempApp.SettingReadonly = false;
		this.txtTempApp.Size = new System.Drawing.Size(117, 20);
		this.txtTempApp.TabIndex = 22;
		this.txtTempApp.Visible = false;
		this.lbTempApp.EditControl = null;
		this.lbTempApp.EditValue = "Template ID";
		this.lbTempApp.FieldName = null;
		this.lbTempApp.HideWhenEmpty = false;
		this.lbTempApp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbTempApp.Location = new System.Drawing.Point(387, 163);
		this.lbTempApp.Name = "lbTempApp";
		this.lbTempApp.ReadOnly = false;
		this.lbTempApp.SettingInVisible = false;
		this.lbTempApp.SettingReadonly = false;
		this.lbTempApp.ShowUnderLine = true;
		this.lbTempApp.Size = new System.Drawing.Size(75, 20);
		this.lbTempApp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbTempApp.TabIndex = 21;
		this.lbTempApp.Utext = "Template ID";
		this.lbTempApp.Visible = false;
		this.ck_Approve.EditControl = null;
		this.ck_Approve.FieldName = null;
		this.ck_Approve.IsSearchControl = false;
		this.ck_Approve.Location = new System.Drawing.Point(274, 164);
		this.ck_Approve.Name = "ck_Approve";
		this.ck_Approve.Properties.Caption = "A&pprove";
		this.ck_Approve.Properties.DisplayValueChecked = "Y";
		this.ck_Approve.Properties.DisplayValueGrayed = "N";
		this.ck_Approve.Properties.DisplayValueUnchecked = "N";
		this.ck_Approve.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Approve.Properties.ValueChecked = "Y";
		this.ck_Approve.Properties.ValueGrayed = "N";
		this.ck_Approve.Properties.ValueUnchecked = "N";
		this.ck_Approve.SettingInVisible = false;
		this.ck_Approve.SettingReadonly = false;
		this.ck_Approve.Size = new System.Drawing.Size(97, 20);
		this.ck_Approve.TabIndex = 20;
		this.ck_Approve.Visible = false;
		this.ck_Approve.CheckedChanged += new System.EventHandler(this.ck_Approve_CheckedChanged);
		this.ck_DataArchi.EditControl = null;
		this.ck_DataArchi.FieldName = null;
		this.ck_DataArchi.IsSearchControl = false;
		this.ck_DataArchi.Location = new System.Drawing.Point(274, 139);
		this.ck_DataArchi.Name = "ck_DataArchi";
		this.ck_DataArchi.Properties.Caption = "D&ata Archive (Requires Implementation DLL)";
		this.ck_DataArchi.Properties.DisplayValueChecked = "Y";
		this.ck_DataArchi.Properties.DisplayValueGrayed = "N";
		this.ck_DataArchi.Properties.DisplayValueUnchecked = "N";
		this.ck_DataArchi.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_DataArchi.Properties.ValueChecked = "Y";
		this.ck_DataArchi.Properties.ValueGrayed = "N";
		this.ck_DataArchi.Properties.ValueUnchecked = "N";
		this.ck_DataArchi.SettingInVisible = false;
		this.ck_DataArchi.SettingReadonly = false;
		this.ck_DataArchi.Size = new System.Drawing.Size(318, 20);
		this.ck_DataArchi.TabIndex = 19;
		this.ck_DataArchi.Visible = false;
		this.ck_ManagerSeries.EditControl = null;
		this.ck_ManagerSeries.FieldName = null;
		this.ck_ManagerSeries.IsSearchControl = false;
		this.ck_ManagerSeries.Location = new System.Drawing.Point(274, 94);
		this.ck_ManagerSeries.Name = "ck_ManagerSeries";
		this.ck_ManagerSeries.Properties.Caption = "Manager &Series";
		this.ck_ManagerSeries.Properties.DisplayValueChecked = "Y";
		this.ck_ManagerSeries.Properties.DisplayValueGrayed = "N";
		this.ck_ManagerSeries.Properties.DisplayValueUnchecked = "N";
		this.ck_ManagerSeries.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_ManagerSeries.Properties.ValueChecked = "Y";
		this.ck_ManagerSeries.Properties.ValueGrayed = "N";
		this.ck_ManagerSeries.Properties.ValueUnchecked = "N";
		this.ck_ManagerSeries.SettingInVisible = false;
		this.ck_ManagerSeries.SettingReadonly = false;
		this.ck_ManagerSeries.Size = new System.Drawing.Size(97, 20);
		this.ck_ManagerSeries.TabIndex = 18;
		this.ck_ManagerSeries.Visible = false;
		this.txt_TabNameLog.EditControl = null;
		this.txt_TabNameLog.Enabled = false;
		this.txt_TabNameLog.FieldName = null;
		this.txt_TabNameLog.IsSearchControl = false;
		this.txt_TabNameLog.Location = new System.Drawing.Point(187, 276);
		this.txt_TabNameLog.Name = "txt_TabNameLog";
		this.txt_TabNameLog.SettingInVisible = false;
		this.txt_TabNameLog.SettingReadonly = false;
		this.txt_TabNameLog.Size = new System.Drawing.Size(213, 20);
		this.txt_TabNameLog.TabIndex = 17;
		this.txt_TabNameLog.Visible = false;
		this.lb_TabLog.EditControl = null;
		this.lb_TabLog.EditValue = "Table Name";
		this.lb_TabLog.FieldName = null;
		this.lb_TabLog.HideWhenEmpty = false;
		this.lb_TabLog.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_TabLog.Location = new System.Drawing.Point(73, 279);
		this.lb_TabLog.Name = "lb_TabLog";
		this.lb_TabLog.ReadOnly = false;
		this.lb_TabLog.SettingInVisible = false;
		this.lb_TabLog.SettingReadonly = false;
		this.lb_TabLog.ShowUnderLine = true;
		this.lb_TabLog.Size = new System.Drawing.Size(117, 20);
		this.lb_TabLog.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_TabLog.TabIndex = 16;
		this.lb_TabLog.Utext = "Table Name";
		this.lb_TabLog.Visible = false;
		this.ck_YearTranfer.EditControl = null;
		this.ck_YearTranfer.FieldName = null;
		this.ck_YearTranfer.IsSearchControl = false;
		this.ck_YearTranfer.Location = new System.Drawing.Point(274, 114);
		this.ck_YearTranfer.Name = "ck_YearTranfer";
		this.ck_YearTranfer.Properties.Caption = "&Year Transfer";
		this.ck_YearTranfer.Properties.DisplayValueChecked = "Y";
		this.ck_YearTranfer.Properties.DisplayValueGrayed = "N";
		this.ck_YearTranfer.Properties.DisplayValueUnchecked = "N";
		this.ck_YearTranfer.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_YearTranfer.Properties.ValueChecked = "Y";
		this.ck_YearTranfer.Properties.ValueGrayed = "N";
		this.ck_YearTranfer.Properties.ValueUnchecked = "N";
		this.ck_YearTranfer.SettingInVisible = false;
		this.ck_YearTranfer.SettingReadonly = false;
		this.ck_YearTranfer.Size = new System.Drawing.Size(97, 20);
		this.ck_YearTranfer.TabIndex = 15;
		this.ck_YearTranfer.Visible = false;
		this.ck_Log.EditControl = null;
		this.ck_Log.FieldName = null;
		this.ck_Log.IsSearchControl = false;
		this.ck_Log.Location = new System.Drawing.Point(50, 245);
		this.ck_Log.Name = "ck_Log";
		this.ck_Log.Properties.Caption = "L&og";
		this.ck_Log.Properties.DisplayValueChecked = "Y";
		this.ck_Log.Properties.DisplayValueGrayed = "N";
		this.ck_Log.Properties.DisplayValueUnchecked = "N";
		this.ck_Log.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Log.Properties.ValueChecked = "Y";
		this.ck_Log.Properties.ValueGrayed = "N";
		this.ck_Log.Properties.ValueUnchecked = "N";
		this.ck_Log.SettingInVisible = false;
		this.ck_Log.SettingReadonly = false;
		this.ck_Log.Size = new System.Drawing.Size(75, 20);
		this.ck_Log.TabIndex = 14;
		this.ck_Log.Visible = false;
		this.ck_Log.CheckedChanged += new System.EventHandler(this.ck_Log_CheckedChanged);
		this.ck_Cancel.EditControl = null;
		this.ck_Cancel.FieldName = null;
		this.ck_Cancel.IsSearchControl = false;
		this.ck_Cancel.Location = new System.Drawing.Point(50, 189);
		this.ck_Cancel.Name = "ck_Cancel";
		this.ck_Cancel.Properties.Caption = "Cance&l";
		this.ck_Cancel.Properties.DisplayValueChecked = "Y";
		this.ck_Cancel.Properties.DisplayValueGrayed = "N";
		this.ck_Cancel.Properties.DisplayValueUnchecked = "N";
		this.ck_Cancel.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Cancel.Properties.ValueChecked = "Y";
		this.ck_Cancel.Properties.ValueGrayed = "N";
		this.ck_Cancel.Properties.ValueUnchecked = "N";
		this.ck_Cancel.SettingInVisible = false;
		this.ck_Cancel.SettingReadonly = false;
		this.ck_Cancel.Size = new System.Drawing.Size(75, 20);
		this.ck_Cancel.TabIndex = 13;
		this.ck_Delete.EditControl = null;
		this.ck_Delete.FieldName = null;
		this.ck_Delete.IsSearchControl = false;
		this.ck_Delete.Location = new System.Drawing.Point(50, 164);
		this.ck_Delete.Name = "ck_Delete";
		this.ck_Delete.Properties.Caption = "Delete";
		this.ck_Delete.Properties.DisplayValueChecked = "Y";
		this.ck_Delete.Properties.DisplayValueGrayed = "N";
		this.ck_Delete.Properties.DisplayValueUnchecked = "N";
		this.ck_Delete.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Delete.Properties.ValueChecked = "Y";
		this.ck_Delete.Properties.ValueGrayed = "N";
		this.ck_Delete.Properties.ValueUnchecked = "N";
		this.ck_Delete.SettingInVisible = false;
		this.ck_Delete.SettingReadonly = false;
		this.ck_Delete.Size = new System.Drawing.Size(75, 20);
		this.ck_Delete.TabIndex = 12;
		this.ck_Find.EditControl = null;
		this.ck_Find.FieldName = null;
		this.ck_Find.IsSearchControl = false;
		this.ck_Find.Location = new System.Drawing.Point(50, 139);
		this.ck_Find.Name = "ck_Find";
		this.ck_Find.Properties.Caption = "F&ind";
		this.ck_Find.Properties.DisplayValueChecked = "Y";
		this.ck_Find.Properties.DisplayValueGrayed = "N";
		this.ck_Find.Properties.DisplayValueUnchecked = "N";
		this.ck_Find.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Find.Properties.ValueChecked = "Y";
		this.ck_Find.Properties.ValueGrayed = "N";
		this.ck_Find.Properties.ValueUnchecked = "N";
		this.ck_Find.SettingInVisible = false;
		this.ck_Find.SettingReadonly = false;
		this.ck_Find.Size = new System.Drawing.Size(75, 20);
		this.ck_Find.TabIndex = 11;
		this.ck_Update.EditControl = null;
		this.ck_Update.EditValue = "Y";
		this.ck_Update.Enabled = false;
		this.ck_Update.FieldName = null;
		this.ck_Update.IsSearchControl = false;
		this.ck_Update.Location = new System.Drawing.Point(50, 114);
		this.ck_Update.Name = "ck_Update";
		this.ck_Update.Properties.Caption = "&Update";
		this.ck_Update.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Update.Properties.ValueChecked = "Y";
		this.ck_Update.Properties.ValueUnchecked = "N";
		this.ck_Update.SettingInVisible = false;
		this.ck_Update.SettingReadonly = false;
		this.ck_Update.Size = new System.Drawing.Size(75, 20);
		this.ck_Update.TabIndex = 10;
		this.ck_Add.EditControl = null;
		this.ck_Add.EditValue = "Y";
		this.ck_Add.Enabled = false;
		this.ck_Add.FieldName = null;
		this.ck_Add.IsSearchControl = false;
		this.ck_Add.Location = new System.Drawing.Point(50, 89);
		this.ck_Add.Name = "ck_Add";
		this.ck_Add.Properties.Caption = "Add";
		this.ck_Add.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.ck_Add.Properties.ValueChecked = "Y";
		this.ck_Add.Properties.ValueUnchecked = "N";
		this.ck_Add.SettingInVisible = false;
		this.ck_Add.SettingReadonly = false;
		this.ck_Add.Size = new System.Drawing.Size(75, 20);
		this.ck_Add.TabIndex = 9;
		this.panel3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel3.BackColor = System.Drawing.Color.White;
		this.panel3.Controls.Add(this.label2);
		this.panel3.Location = new System.Drawing.Point(3, 3);
		this.panel3.Name = "panel3";
		this.panel3.Size = new System.Drawing.Size(589, 80);
		this.panel3.TabIndex = 8;
		this.label2.AutoSize = true;
		this.label2.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label2.Location = new System.Drawing.Point(205, 15);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(117, 19);
		this.label2.TabIndex = 1;
		this.label2.Text = "Setting Services";
		this.wzp_DefBasSet.Controls.Add(this.btn_TableName);
		this.wzp_DefBasSet.Controls.Add(this.Luk_Type);
		this.wzp_DefBasSet.Controls.Add(this.txt_Name);
		this.wzp_DefBasSet.Controls.Add(this.txt_UniqueID);
		this.wzp_DefBasSet.Controls.Add(this.labelLine4);
		this.wzp_DefBasSet.Controls.Add(this.labelLine3);
		this.wzp_DefBasSet.Controls.Add(this.labelLine2);
		this.wzp_DefBasSet.Controls.Add(this.labelLine1);
		this.wzp_DefBasSet.Controls.Add(this.panel1);
		this.wzp_DefBasSet.Name = "wzp_DefBasSet";
		this.wzp_DefBasSet.Size = new System.Drawing.Size(595, 339);
		this.btn_TableName.EditControl = null;
		this.btn_TableName.FieldName = null;
		this.btn_TableName.IsSearchControl = false;
		this.btn_TableName.Location = new System.Drawing.Point(230, 199);
		this.btn_TableName.Name = "btn_TableName";
		this.btn_TableName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.btn_TableName.Properties.AppearanceFocused.Options.UseBackColor = true;
		this.btn_TableName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.btn_TableName.SettingInVisible = false;
		this.btn_TableName.SettingReadonly = false;
		this.btn_TableName.Size = new System.Drawing.Size(213, 20);
		this.btn_TableName.TabIndex = 18;
		this.btn_TableName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_TableName_ButtonClick);
		this.btn_TableName.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.btn_TableName_PreviewKeyDown);
		this.Luk_Type.Location = new System.Drawing.Point(230, 173);
		this.Luk_Type.Name = "Luk_Type";
		this.Luk_Type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.Luk_Type.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.Luk_Type.Properties.DisplayMember = "Name";
		this.Luk_Type.Properties.NullText = "";
		this.Luk_Type.Properties.ValueMember = "ID";
		this.Luk_Type.Size = new System.Drawing.Size(213, 20);
		this.Luk_Type.TabIndex = 17;
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(230, 147);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(213, 20);
		this.txt_Name.TabIndex = 14;
		this.txt_UniqueID.EditControl = null;
		this.txt_UniqueID.FieldName = null;
		this.txt_UniqueID.IsSearchControl = false;
		this.txt_UniqueID.Location = new System.Drawing.Point(230, 119);
		this.txt_UniqueID.Name = "txt_UniqueID";
		this.txt_UniqueID.SettingInVisible = false;
		this.txt_UniqueID.SettingReadonly = false;
		this.txt_UniqueID.Size = new System.Drawing.Size(213, 20);
		this.txt_UniqueID.TabIndex = 13;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Table Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(116, 202);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(117, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 12;
		this.labelLine4.Utext = "Table Name";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Type";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(116, 176);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 11;
		this.labelLine3.Utext = "Type";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Unique ID";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(116, 122);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(117, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 10;
		this.labelLine2.Utext = "Unique ID";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Name";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(116, 150);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(117, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 8;
		this.labelLine1.Utext = "Name";
		this.panel1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel1.BackColor = System.Drawing.Color.White;
		this.panel1.Controls.Add(this.label1);
		this.panel1.Location = new System.Drawing.Point(3, 3);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(589, 88);
		this.panel1.TabIndex = 7;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(171, 4);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(165, 19);
		this.label1.TabIndex = 1;
		this.label1.Text = "Defining Basic Settings";
		this.wzp_SetFieldChildTable.Controls.Add(this.ck_CheckAllChildName);
		this.wzp_SetFieldChildTable.Controls.Add(this.luk_TableNameChild);
		this.wzp_SetFieldChildTable.Controls.Add(this.grd_ChildNameTable);
		this.wzp_SetFieldChildTable.Controls.Add(this.panel7);
		this.wzp_SetFieldChildTable.Name = "wzp_SetFieldChildTable";
		this.wzp_SetFieldChildTable.Size = new System.Drawing.Size(595, 339);
		this.ck_CheckAllChildName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ck_CheckAllChildName.EditControl = null;
		this.ck_CheckAllChildName.EditValue = "N";
		this.ck_CheckAllChildName.FieldName = null;
		this.ck_CheckAllChildName.IsSearchControl = false;
		this.ck_CheckAllChildName.Location = new System.Drawing.Point(3, 316);
		this.ck_CheckAllChildName.Name = "ck_CheckAllChildName";
		this.ck_CheckAllChildName.Properties.Caption = "&Check All";
		this.ck_CheckAllChildName.Properties.DisplayValueChecked = "Y";
		this.ck_CheckAllChildName.Properties.DisplayValueGrayed = "N";
		this.ck_CheckAllChildName.Properties.DisplayValueUnchecked = "N";
		this.ck_CheckAllChildName.Properties.ValueChecked = "Y";
		this.ck_CheckAllChildName.Properties.ValueGrayed = "N";
		this.ck_CheckAllChildName.Properties.ValueUnchecked = "N";
		this.ck_CheckAllChildName.SettingInVisible = false;
		this.ck_CheckAllChildName.SettingReadonly = false;
		this.ck_CheckAllChildName.Size = new System.Drawing.Size(89, 20);
		this.ck_CheckAllChildName.TabIndex = 25;
		this.ck_CheckAllChildName.CheckedChanged += new System.EventHandler(this.ck_CheckAllChildName_CheckedChanged);
		this.luk_TableNameChild.Location = new System.Drawing.Point(12, 83);
		this.luk_TableNameChild.Name = "luk_TableNameChild";
		this.luk_TableNameChild.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TableNameChild.Properties.NullText = "";
		this.luk_TableNameChild.Size = new System.Drawing.Size(220, 20);
		this.luk_TableNameChild.TabIndex = 14;
		this.luk_TableNameChild.EditValueChanged += new System.EventHandler(this.luk_TableNameChild_EditValueChanged);
		this.grd_ChildNameTable.AllowPasteExcel = false;
		this.grd_ChildNameTable.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ChildNameTable.ClipboardData = null;
		this.grd_ChildNameTable.Location = new System.Drawing.Point(3, 109);
		this.grd_ChildNameTable.MainView = this.gv_ChildNameTable;
		this.grd_ChildNameTable.Name = "grd_ChildNameTable";
		this.grd_ChildNameTable.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repositoryItemLookUpEdit2, this.ck_VisibledeForm, this.ck_EditabledeForm, this.repositoryItemCheckedComboBoxEdit1 });
		this.grd_ChildNameTable.Size = new System.Drawing.Size(589, 201);
		this.grd_ChildNameTable.TabIndex = 13;
		this.grd_ChildNameTable.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ChildNameTable });
		this.gv_ChildNameTable.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ChildNameTable.ColumnKey = null;
		this.gv_ChildNameTable.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.col_ColIsUsed, this.col_ColEdit, this.ColAlias, this.gridColumn8 });
		this.gv_ChildNameTable.DisableFormSetting = false;
		this.gv_ChildNameTable.DisplayException = false;
		this.gv_ChildNameTable.EditColumn = null;
		this.gv_ChildNameTable.FieldCurrency = null;
		this.gv_ChildNameTable.FieldRate = null;
		this.gv_ChildNameTable.FormId = "oUserDefinedObject";
		this.gv_ChildNameTable.GridControl = this.grd_ChildNameTable;
		this.gv_ChildNameTable.IgnoreFilter = false;
		this.gv_ChildNameTable.IndexEditRow = -1;
		this.gv_ChildNameTable.IndicatorWidth = 40;
		this.gv_ChildNameTable.IsApzTable = false;
		this.gv_ChildNameTable.IsInsertNewRow = false;
		this.gv_ChildNameTable.IsLoadingData = false;
		this.gv_ChildNameTable.IsMappingContextAction = false;
		this.gv_ChildNameTable.ItemId = "OCHILDTABLENAME";
		this.gv_ChildNameTable.LstColumnAllowBatch = null;
		this.gv_ChildNameTable.LstColumnNotAllowBatch = null;
		this.gv_ChildNameTable.LstColumnNotEditable = null;
		this.gv_ChildNameTable.LstColumNotSetting = null;
		this.gv_ChildNameTable.Name = "gv_ChildNameTable";
		this.gv_ChildNameTable.OptionsSelection.MultiSelect = true;
		this.gv_ChildNameTable.OptionsView.ShowGroupPanel = false;
		this.gv_ChildNameTable.UseBatchMod = false;
		this.gv_ChildNameTable.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_ChildNameTable_CustomDrawRowIndicator);
		this.gv_ChildNameTable.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_ChildNameTable_RowCellStyle);
		this.gv_ChildNameTable.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ChildNameTable_ShowingEditor);
		this.gv_ChildNameTable.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_ChildNameTable_CellValueChanged);
		this.col_ColIsUsed.Caption = "Visible";
		this.col_ColIsUsed.ColumnEdit = this.ck_VisibledeForm;
		this.col_ColIsUsed.CurrencyDateField = null;
		this.col_ColIsUsed.CurrencyField = null;
		this.col_ColIsUsed.CurrencyFixed = null;
		this.col_ColIsUsed.CurrencyFixedRate = 0.0;
		this.col_ColIsUsed.CurrencyRateField = null;
		this.col_ColIsUsed.FieldName = "ColIsUsed";
		this.col_ColIsUsed.Name = "col_ColIsUsed";
		this.col_ColIsUsed.PrimaryKey = false;
		this.col_ColIsUsed.UnitField = null;
		this.col_ColIsUsed.Visible = true;
		this.col_ColIsUsed.VisibleIndex = 0;
		this.col_ColIsUsed.Width = 99;
		this.ck_VisibledeForm.AutoHeight = false;
		this.ck_VisibledeForm.DisplayValueChecked = "Y";
		this.ck_VisibledeForm.DisplayValueGrayed = "N";
		this.ck_VisibledeForm.DisplayValueUnchecked = "N";
		this.ck_VisibledeForm.Name = "ck_VisibledeForm";
		this.ck_VisibledeForm.ValueChecked = "Y";
		this.ck_VisibledeForm.ValueGrayed = "N";
		this.ck_VisibledeForm.ValueUnchecked = "N";
		this.col_ColEdit.Caption = "Editable";
		this.col_ColEdit.ColumnEdit = this.ck_EditabledeForm;
		this.col_ColEdit.CurrencyDateField = null;
		this.col_ColEdit.CurrencyField = null;
		this.col_ColEdit.CurrencyFixed = null;
		this.col_ColEdit.CurrencyFixedRate = 0.0;
		this.col_ColEdit.CurrencyRateField = null;
		this.col_ColEdit.FieldName = "ColEdit";
		this.col_ColEdit.Name = "col_ColEdit";
		this.col_ColEdit.PrimaryKey = false;
		this.col_ColEdit.UnitField = null;
		this.col_ColEdit.Visible = true;
		this.col_ColEdit.VisibleIndex = 1;
		this.col_ColEdit.Width = 114;
		this.ck_EditabledeForm.AutoHeight = false;
		this.ck_EditabledeForm.DisplayValueChecked = "Y";
		this.ck_EditabledeForm.DisplayValueGrayed = "N";
		this.ck_EditabledeForm.DisplayValueUnchecked = "N";
		this.ck_EditabledeForm.Name = "ck_EditabledeForm";
		this.ck_EditabledeForm.ValueChecked = "Y";
		this.ck_EditabledeForm.ValueGrayed = "N";
		this.ck_EditabledeForm.ValueUnchecked = "N";
		this.ColAlias.Caption = "Column Alias";
		this.ColAlias.CurrencyDateField = null;
		this.ColAlias.CurrencyField = null;
		this.ColAlias.CurrencyFixed = null;
		this.ColAlias.CurrencyFixedRate = 0.0;
		this.ColAlias.CurrencyRateField = null;
		this.ColAlias.FieldName = "COLUMN_NAME";
		this.ColAlias.Name = "ColAlias";
		this.ColAlias.OptionsColumn.AllowEdit = false;
		this.ColAlias.OptionsColumn.ReadOnly = true;
		this.ColAlias.PrimaryKey = false;
		this.ColAlias.UnitField = null;
		this.ColAlias.Visible = true;
		this.ColAlias.VisibleIndex = 2;
		this.ColAlias.Width = 212;
		this.gridColumn8.Caption = "Column Description";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "COLUMN_NAME";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 3;
		this.gridColumn8.Width = 613;
		this.repositoryItemLookUpEdit2.AutoHeight = false;
		this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "Caption")
		});
		this.repositoryItemLookUpEdit2.DisplayMember = "Caption";
		this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
		this.repositoryItemLookUpEdit2.NullText = "";
		this.repositoryItemLookUpEdit2.ValueMember = "Name";
		this.repositoryItemCheckedComboBoxEdit1.AutoHeight = false;
		this.repositoryItemCheckedComboBoxEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCheckedComboBoxEdit1.Name = "repositoryItemCheckedComboBoxEdit1";
		this.panel7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel7.BackColor = System.Drawing.Color.White;
		this.panel7.Controls.Add(this.label7);
		this.panel7.Location = new System.Drawing.Point(3, 3);
		this.panel7.Name = "panel7";
		this.panel7.Size = new System.Drawing.Size(589, 74);
		this.panel7.TabIndex = 12;
		this.label7.AutoSize = true;
		this.label7.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.label7.Location = new System.Drawing.Point(125, 13);
		this.label7.Name = "label7";
		this.label7.Size = new System.Drawing.Size(288, 19);
		this.label7.TabIndex = 1;
		this.label7.Text = "Setting fields for a child table default form";
		this.wzp_ModifyFieldsFind.Controls.Add(this.ck_CheckAll);
		this.wzp_ModifyFieldsFind.Controls.Add(this.grd_FieldsFind);
		this.wzp_ModifyFieldsFind.Controls.Add(this.panel8);
		this.wzp_ModifyFieldsFind.Name = "wzp_ModifyFieldsFind";
		this.wzp_ModifyFieldsFind.Size = new System.Drawing.Size(595, 339);
		this.ck_CheckAll.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ck_CheckAll.EditControl = null;
		this.ck_CheckAll.EditValue = "N";
		this.ck_CheckAll.FieldName = null;
		this.ck_CheckAll.IsSearchControl = false;
		this.ck_CheckAll.Location = new System.Drawing.Point(8, 317);
		this.ck_CheckAll.Name = "ck_CheckAll";
		this.ck_CheckAll.Properties.Caption = "&Check All";
		this.ck_CheckAll.Properties.DisplayValueChecked = "Y";
		this.ck_CheckAll.Properties.DisplayValueGrayed = "N";
		this.ck_CheckAll.Properties.DisplayValueUnchecked = "N";
		this.ck_CheckAll.Properties.ValueChecked = "Y";
		this.ck_CheckAll.Properties.ValueGrayed = "N";
		this.ck_CheckAll.Properties.ValueUnchecked = "N";
		this.ck_CheckAll.SettingInVisible = false;
		this.ck_CheckAll.SettingReadonly = false;
		this.ck_CheckAll.Size = new System.Drawing.Size(103, 20);
		this.ck_CheckAll.TabIndex = 24;
		this.ck_CheckAll.CheckedChanged += new System.EventHandler(this.ck_CheckAll_CheckedChanged);
		this.grd_FieldsFind.AllowPasteExcel = false;
		this.grd_FieldsFind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_FieldsFind.ClipboardData = null;
		this.grd_FieldsFind.Location = new System.Drawing.Point(8, 81);
		this.grd_FieldsFind.MainView = this.gv_FieldsFind;
		this.grd_FieldsFind.Name = "grd_FieldsFind";
		this.grd_FieldsFind.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemLookUpEdit4, this.ck_FielsFind });
		this.grd_FieldsFind.Size = new System.Drawing.Size(584, 230);
		this.grd_FieldsFind.TabIndex = 13;
		this.grd_FieldsFind.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_FieldsFind });
		this.gv_FieldsFind.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_FieldsFind.ColumnKey = null;
		this.gv_FieldsFind.DisableFormSetting = false;
		this.gv_FieldsFind.DisplayException = false;
		this.gv_FieldsFind.EditColumn = null;
		this.gv_FieldsFind.FieldCurrency = null;
		this.gv_FieldsFind.FieldRate = null;
		gridFormatRule.Name = "Format0";
		gridFormatRule.Rule = null;
		this.gv_FieldsFind.FormatRules.Add(gridFormatRule);
		this.gv_FieldsFind.FormId = "oUserDefinedObject";
		this.gv_FieldsFind.GridControl = this.grd_FieldsFind;
		this.gv_FieldsFind.IgnoreFilter = false;
		this.gv_FieldsFind.IndexEditRow = -1;
		this.gv_FieldsFind.IndicatorWidth = 40;
		this.gv_FieldsFind.IsApzTable = false;
		this.gv_FieldsFind.IsInsertNewRow = false;
		this.gv_FieldsFind.IsLoadingData = false;
		this.gv_FieldsFind.IsMappingContextAction = false;
		this.gv_FieldsFind.ItemId = "OFIELDFIND";
		this.gv_FieldsFind.LstColumnAllowBatch = null;
		this.gv_FieldsFind.LstColumnNotAllowBatch = null;
		this.gv_FieldsFind.LstColumnNotEditable = null;
		this.gv_FieldsFind.LstColumNotSetting = null;
		this.gv_FieldsFind.Name = "gv_FieldsFind";
		this.gv_FieldsFind.OptionsSelection.MultiSelect = true;
		this.gv_FieldsFind.OptionsView.ShowGroupPanel = false;
		this.gv_FieldsFind.UseBatchMod = false;
		this.gv_FieldsFind.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_FieldsFind_RowCellStyle);
		this.gv_FieldsFind.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_FieldsFind_ShowingEditor);
		this.repositoryItemLookUpEdit4.AutoHeight = false;
		this.repositoryItemLookUpEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit4.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Caption", "Caption")
		});
		this.repositoryItemLookUpEdit4.DisplayMember = "Caption";
		this.repositoryItemLookUpEdit4.Name = "repositoryItemLookUpEdit4";
		this.repositoryItemLookUpEdit4.NullText = "";
		this.repositoryItemLookUpEdit4.ValueMember = "Name";
		this.ck_FielsFind.AutoHeight = false;
		this.ck_FielsFind.DisplayValueChecked = "Y";
		this.ck_FielsFind.DisplayValueGrayed = "N";
		this.ck_FielsFind.DisplayValueUnchecked = "N";
		this.ck_FielsFind.Name = "ck_FielsFind";
		this.ck_FielsFind.ValueChecked = "Y";
		this.ck_FielsFind.ValueGrayed = "N";
		this.ck_FielsFind.ValueUnchecked = "N";
		this.panel8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panel8.BackColor = System.Drawing.Color.White;
		this.panel8.Controls.Add(this.lblModify);
		this.panel8.Location = new System.Drawing.Point(3, 3);
		this.panel8.Name = "panel8";
		this.panel8.Size = new System.Drawing.Size(589, 72);
		this.panel8.TabIndex = 12;
		this.lblModify.AutoSize = true;
		this.lblModify.Font = new System.Drawing.Font("Times New Roman", 12f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.lblModify.Location = new System.Drawing.Point(122, 16);
		this.lblModify.Name = "lblModify";
		this.lblModify.Size = new System.Drawing.Size(0, 19);
		this.lblModify.TabIndex = 1;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(595, 385);
		base.Controls.Add(this.wiz_ORM);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oUserDefinedObject";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ORW";
		this.Text = "User - Defined Object Registration";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_ORW_Load);
		((System.ComponentModel.ISupportInitialize)this.wiz_ORM).EndInit();
		this.wiz_ORM.ResumeLayout(false);
		this.wzp_SelectRegisMode.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Table).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Table).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit3).EndInit();
		this.panel2.ResumeLayout(false);
		this.panel2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.rdg_OperationType.Properties).EndInit();
		this.wzp_LinkChildUser.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAllNameTable.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_NameTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_NameTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_CheckTabName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		this.panel6.ResumeLayout(false);
		this.panel6.PerformLayout();
		this.wzp_ModifyDefaultForm.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAllColumn.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Column).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Column).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_TargetField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Visible).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_EditTable).EndInit();
		this.panel5.ResumeLayout(false);
		this.panel5.PerformLayout();
		this.wzp_UISetting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_menuItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MenuCaption.Properties).EndInit();
		this.panel4.ResumeLayout(false);
		this.panel4.PerformLayout();
		this.wzp_SetSer.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_Close.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTempApp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Approve.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_DataArchi.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_ManagerSeries.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_TabNameLog.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_YearTranfer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Log.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Cancel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Delete.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Find.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Update.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_Add.Properties).EndInit();
		this.panel3.ResumeLayout(false);
		this.panel3.PerformLayout();
		this.wzp_DefBasSet.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_TableName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Luk_Type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UniqueID.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel1.PerformLayout();
		this.wzp_SetFieldChildTable.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAllChildName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TableNameChild.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_ChildNameTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ChildNameTable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_VisibledeForm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_EditabledeForm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckedComboBoxEdit1).EndInit();
		this.panel7.ResumeLayout(false);
		this.panel7.PerformLayout();
		this.wzp_ModifyFieldsFind.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.ck_CheckAll.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_FieldsFind).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_FieldsFind).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.ck_FielsFind).EndInit();
		this.panel8.ResumeLayout(false);
		this.panel8.PerformLayout();
		base.ResumeLayout(false);
	}
}
