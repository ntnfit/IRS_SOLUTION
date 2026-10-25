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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.DMS;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Promotions;

public class frm_PromotionProgram : XtraFormEx
{
	private DataTable _dtDoc;

	private DataTable _dtDocOriginal;

	private DataTable _dtLineBenefit;

	private DataTable _dtLineCondition;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private DataTable dtSearch;

	public string _code = "";

	private List<string> lstPivotColBenefit = new List<string>();

	private List<string> lstPivotColCondition = new List<string>();

	private RepositoryItemButtonEditCFL rep_BenefitCode;

	private RepositoryItemButtonEditCFL rep_ConditionCode;

	public DataTable dtAttachment = new DataTable();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Benefit;

	private GridViewEx gv_Benefit;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem ctm_Doc_Add;

	private ContextMenuStrip ctm_Benefit;

	private ToolStripMenuItem ctm_Benefit_Add;

	private ToolStripMenuItem ctm_Benefit_Delete;

	private SimpleButtonEx btn_Search;

	private LookUpEditEx luk_Status;

	private LabelLine labelLine4;

	private DateEditEx date_toDate;

	private DateEditEx date_fromDate;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private ToolStripMenuItem ctm_Doc_Duplicate;

	private ToolStripMenuItem ctm_Doc_Delete;

	private LabelLine labelLine1;

	private TextEditEx txt_Code;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_Condition;

	private GridViewEx gv_Condition;

	private ContextMenuStrip ctm_Condition;

	private ToolStripMenuItem ctm_Condition_Add;

	private ToolStripMenuItem ctm_Condition_Delete;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
				btn_Ok.Text = Resources.ButtonAdd;
				ctm_Doc_Add.Enabled = false;
				ctm_Doc_Duplicate.Enabled = false;
				ctm_Doc_Delete.Enabled = false;
				ctm_Benefit_Add.Enabled = true;
				ctm_Benefit_Delete.Enabled = true;
				ctm_Condition_Add.Enabled = true;
				ctm_Condition_Delete.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = Resources.ButtonUpdate;
				ctm_Doc_Add.Enabled = true;
				ctm_Doc_Duplicate.Enabled = true;
				ctm_Doc_Delete.Enabled = true;
				ctm_Benefit_Add.Enabled = true;
				ctm_Benefit_Delete.Enabled = true;
				ctm_Condition_Add.Enabled = true;
				ctm_Condition_Delete.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = Resources.ButtonOK;
				ctm_Doc_Add.Enabled = true;
				ctm_Doc_Duplicate.Enabled = true;
				ctm_Doc_Delete.Enabled = true;
				ctm_Benefit_Add.Enabled = true;
				ctm_Benefit_Delete.Enabled = true;
				ctm_Condition_Add.Enabled = true;
				ctm_Condition_Delete.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_PromotionProgram()
	{
		InitializeComponent();
		FormId = "oPromotionProgram";
		gv_Doc.FormId = "oPromotionProgram";
		gv_Doc.ItemId = "APZ_OPPR";
		gv_Benefit.FormId = "oPromotionProgram";
		gv_Benefit.ItemId = "APZ_PPR1";
		gv_Condition.FormId = "oPromotionProgram";
		gv_Condition.ItemId = "APZ_PPR2";
		gv_Doc.ChangeLogType = Enums.ChangeLog.clPromotionProgram;
		gv_Doc.IsApzTable = true;
		OnLoadAsync += frm_PromotionProgram_Load;
	}

	public void AttachmentClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string objCode = Function.ToString(((ButtonEdit)sender).GetGridViewFromEditor().GetFocusedDataRow()["Code"]);
			frm_UploadAttachment frm_UploadAttachment2 = new frm_UploadAttachment(ApzObjectTypes.oPromotionProgram, objCode);
			frm_UploadAttachment2.dtUpload = dtAttachment;
			frm_UploadAttachment2.ShowDialog(this);
			dtAttachment = frm_UploadAttachment2.dtUpload.Copy();
			if (frm_UploadAttachment2.FormMode != Enums.FormViewModeType.View && FormMode == Enums.FormViewModeType.View)
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

	private void AddColumnAttachmentButton()
	{
		try
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
			Image image = ImageResourceCache.Default.GetImage("images/mail/attachment_16x16.png");
			repositoryItemButtonEdit.Buttons[0].ImageOptions.Image = image;
			repositoryItemButtonEdit.ButtonClick += AttachmentClick;
			gv_Doc.Columns["AttachmentButton"].ColumnEdit = repositoryItemButtonEdit;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetAttachment()
	{
		try
		{
			dtAttachment = new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_PromotionProgram_Load(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			_isLoading = true;
			luk_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			rep_BenefitCode = new RepositoryItemButtonEditCFL("Code", "Name");
			rep_BenefitCode.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			rep_BenefitCode.StoredProcedure = "APZ_PromotionProgram_CFL_Benefit";
			rep_BenefitCode.Buttons[1].Visible = false;
			await rep_BenefitCode.RefreshDataSourceAsync();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_BenefitCode;
			repositoryItemButtonEditCFL.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(repositoryItemButtonEditCFL.ItemSelected, new Action<DataRow, GridView>(BenefitCode_ItemSelected));
			rep_ConditionCode = new RepositoryItemButtonEditCFL("Code", "Name");
			rep_ConditionCode.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			rep_ConditionCode.StoredProcedure = "APZ_PromotionProgram_CFL_Condition";
			rep_ConditionCode.Buttons[1].Visible = false;
			await rep_ConditionCode.RefreshDataSourceAsync();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_ConditionCode;
			repositoryItemButtonEditCFL2.ItemSelected = (Action<DataRow, GridView>)Delegate.Combine(repositoryItemButtonEditCFL2.ItemSelected, new Action<DataRow, GridView>(ConditionCode_ItemSelected));
			new DataTable();
			DateTime dateTime = DateTime.Today.AddDays(-Configuration.SearchDate);
			DateTime dateTime2 = DateTime.Today.AddDays(Configuration.SearchDate);
			date_fromDate.EditValue = dateTime;
			date_toDate.EditValue = dateTime2;
			string value = Function.ToString(luk_Status.EditValue);
			if (!string.IsNullOrEmpty(_code))
			{
				txt_Code.EditValue = _code;
			}
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["code"] = _code;
			dataRow["fromDate"] = dateTime;
			dataRow["toDate"] = dateTime2;
			dataRow["active"] = value;
			dtSearch.Rows.Add(dataRow);
			DataSet ds = await _httpClient.PromotionProgram.Search(dtSearch.Copy());
			await LoadLineById(0);
			await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds);
			await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OPPR", null, overrideOld: true);
			AddColumnAttachmentButton();
			_dtDocOriginal = grd_Doc.DataSourceTable.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void BenefitCode_ItemSelected(DataRow row, GridView arg2)
	{
		try
		{
			string value = Function.ToString(row["Code"]);
			gv_Benefit.SetFocusedRowCellValue(gv_Benefit.FocusedColumn, value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ConditionCode_ItemSelected(DataRow row, GridView arg2)
	{
		try
		{
			string value = Function.ToString(row["Code"]);
			gv_Condition.SetFocusedRowCellValue(gv_Condition.FocusedColumn, value);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("code", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("active", typeof(string)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save("A");
				break;
			case Enums.FormViewModeType.Update:
				await Save("U");
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Save(string type)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		try
		{
			if (!grd_Benefit.DataSourceTable().IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DetailIsEmpty, "");
				return;
			}
			if (lstPivotColBenefit.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.RowIsEmpty, "");
				return;
			}
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			if (type.Equals("U"))
			{
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				dataSourceTable.AcceptChanges();
				DataRow[] array = dataSourceTable.Select("DocEntry=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
				_dtDoc.Rows[0]["UserSign2"] = GlobalData.UserName;
			}
			else
			{
				_dtDoc = grd_Doc.DataSourceTable;
			}
			_dtDoc.TableName = "dtDoc";
			if (_dtDoc.IsExistsColumn("AttachmentButton"))
			{
				_dtDoc.Columns.Remove("AttachmentButton");
				_dtDoc.AcceptChanges();
			}
			dataSet.Tables.Add(_dtDoc.Copy());
			_dtLineBenefit = grd_Benefit.DataSourceTable().Copy();
			for (int i = 0; i < _dtLineBenefit.Rows.Count; i++)
			{
				DataRow dataRow = _dtLineBenefit.Rows[i];
				bool flag = true;
				foreach (string item in lstPivotColBenefit)
				{
					if (Function.ToString(dataRow[item]) != "")
					{
						flag = false;
						break;
					}
				}
				if (flag && string.IsNullOrEmpty(Function.ToString(dataRow["PakageNo"])) && string.IsNullOrEmpty(Function.ToString(dataRow["BenefitGroup"])))
				{
					_dtLineBenefit.Rows.RemoveAt(i--);
				}
			}
			DataView dataView = new DataView(_dtLineBenefit);
			List<string> list = (from DataColumn t in _dtLineBenefit.Columns
				where !lstPivotColBenefit.Contains(t.ColumnName)
				select t.ColumnName).ToList();
			DataTable dataTable = dataView.ToTable(distinct: false, list.ToArray());
			DataTable dataTable2 = dataTable.Clone();
			for (int num = 0; num < _dtLineBenefit.Rows.Count; num++)
			{
				DataRow dataRow2 = _dtLineBenefit.Rows[num];
				dataTable.Rows[num]["RowId"] = num;
				foreach (string item2 in lstPivotColBenefit)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2[item2])))
					{
						DataRow dataRow3 = dataTable2.NewRow();
						dataRow3.ItemArray = dataTable.Rows[num].ItemArray;
						dataRow3["RowId"] = num;
						dataRow3["ColId"] = item2;
						dataRow3["BenefitCode"] = dataRow2[item2];
						dataTable2.Rows.Add(dataRow3);
					}
				}
			}
			dataTable2.TableName = "dtLineBenefit";
			dataSet.Tables.Add(dataTable2.Copy());
			_dtLineCondition = grd_Condition.DataSourceTable().Copy();
			for (int num2 = 0; num2 < _dtLineCondition.Rows.Count; num2++)
			{
				DataRow dataRow4 = _dtLineCondition.Rows[num2];
				bool flag2 = true;
				foreach (string item3 in lstPivotColCondition)
				{
					if (Function.ToString(dataRow4[item3]) != "")
					{
						flag2 = false;
						break;
					}
				}
				if (flag2 && string.IsNullOrEmpty(Function.ToString(dataRow4["PakageNo"])) && string.IsNullOrEmpty(Function.ToString(dataRow4["BenefitGroup"])))
				{
					_dtLineCondition.Rows.RemoveAt(num2--);
				}
			}
			DataView dataView2 = new DataView(_dtLineCondition);
			List<string> list2 = (from DataColumn t in _dtLineCondition.Columns
				where !lstPivotColCondition.Contains(t.ColumnName)
				select t.ColumnName).ToList();
			DataTable dataTable3 = dataView2.ToTable(distinct: false, list2.ToArray());
			DataTable dataTable4 = dataTable3.Clone();
			for (int num3 = 0; num3 < _dtLineCondition.Rows.Count; num3++)
			{
				DataRow dataRow5 = _dtLineCondition.Rows[num3];
				dataTable3.Rows[num3]["RowId"] = num3;
				foreach (string item4 in lstPivotColCondition)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataRow5[item4])))
					{
						DataRow dataRow6 = dataTable4.NewRow();
						dataRow6.ItemArray = dataTable3.Rows[num3].ItemArray;
						dataRow6["RowId"] = num3;
						dataRow6["ColId"] = item4;
						dataRow6["ConditionCode"] = dataRow5[item4];
						dataTable4.Rows.Add(dataRow6);
					}
				}
			}
			dataTable4.TableName = "dtLineCondition";
			dataSet.Tables.Add(dataTable4.Copy());
			HttpResult httpResult = ((!type.Equals("U")) ? (await _httpClient.PromotionProgram.Add(dataSet)) : (await _httpClient.PromotionProgram.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				DataTable dataTable5 = dtAttachment.Copy();
				dataTable5.TableName = "dtAttachment";
				if (dataTable5.IsNotNull())
				{
					HttpResult httpResult2 = await _httpClient.FileNetworkManagerClient.SaveAttachment(dataTable5);
					if (httpResult2.MessageCode == Enums.MessageCode.Success)
					{
						ResetAttachment();
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.None, "[Attachment] - " + httpResult2.Message);
					}
				}
				FormMode = Enums.FormViewModeType.View;
				if (type.Equals("A"))
				{
					btn_Search_Click(null, null);
				}
				else
				{
					_dtDocOriginal = grd_Doc.DataSourceTable.Copy();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add || !_isLoading)
			{
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
				AddNewBlankLineBenefit();
				AddNewBlankLineCondition();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Doc_Add_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			_isLoading = true;
			grd_Doc.DataSourceTable.Rows.Clear();
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc == null || _dtDoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtDoc.NewRow();
			dataRow["DocEntry"] = 0;
			dataRow["Code"] = "";
			dataRow["StartDate"] = DateTime.Today;
			dataRow["EndDate"] = DateTime.Today;
			dataRow["IntegrateOther"] = "N";
			dataRow["PakageSplit"] = "N";
			dataRow["Active"] = "Y";
			dataRow["UserSign"] = GlobalData.UserName;
			_dtDoc.Rows.Add(dataRow);
			gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
			grd_Doc.DataSource = _dtDoc;
			grd_Benefit.DataSourceTable().Rows.Clear();
			_dtLineBenefit = grd_Benefit.DataSourceTable;
			if (lstPivotColBenefit.Count > 0)
			{
				foreach (string item in lstPivotColBenefit)
				{
					gv_Benefit.Columns.Remove(gv_Benefit.Columns[item]);
					_dtLineBenefit.Columns.Remove(item);
				}
				lstPivotColBenefit.Clear();
			}
			AddNewBlankPivotColumnBenefit("1");
			AddNewBlankLineBenefit();
			grd_Condition.DataSourceTable().Rows.Clear();
			_dtLineCondition = grd_Condition.DataSourceTable;
			if (lstPivotColCondition.Count > 0)
			{
				foreach (string item2 in lstPivotColCondition)
				{
					gv_Condition.Columns.Remove(gv_Condition.Columns[item2]);
					_dtLineCondition.Columns.Remove(item2);
				}
				lstPivotColCondition.Clear();
			}
			AddNewBlankPivotColumnCondition("1");
			AddNewBlankLineCondition();
			ResetAttachment();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void ctm_Doc_Duplicate_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle < 0 || (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes))
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			_isLoading = true;
			DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
			DataTable dataTable = grd_Doc.DatasourceTable();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = focusedDataRow.ItemArray;
			dataRow["DocEntry"] = 0;
			dataRow["Code"] = "";
			dataTable.Rows.Clear();
			dataTable.Rows.Add(dataRow);
			gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
			ResetAttachment();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async void ctm_Doc_Delete_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedRowHandle >= 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet dataSet = new DataSet();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				DataTable dataTable = grd_Doc.DatasourceTable().Select("DocEntry = '" + num + "'").CopyToDataTable();
				dataTable.TableName = "dtDoc";
				dataTable.Rows[0]["UserSign2"] = GlobalData.UserName;
				dataTable.Rows[0]["Active"] = "D";
				if (dataTable.IsExistsColumn("AttachmentButton"))
				{
					dataTable.Columns.Remove("AttachmentButton");
					dataTable.AcceptChanges();
				}
				dataSet.Tables.Add(dataTable);
				new HttpResult();
				HttpResult httpResult = await _httpClient.PromotionProgram.Cancel(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.None, Resources.Msg_Success + httpResult.Content);
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Benefit_Add_Click(object sender, EventArgs e)
	{
		AddNewBlankLineBenefit();
		DataChanged();
	}

	private void ctm_Benefit_Delete_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Benefit.DataRowCount > 0)
			{
				grd_Benefit.BeginUpdate();
				gv_Benefit.DeleteSelectedRows();
				grd_Benefit.EndUpdate();
				grd_Benefit.DatasourceTable().AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Condition_Add_Click(object sender, EventArgs e)
	{
		AddNewBlankLineCondition();
		DataChanged();
	}

	private void ctm_Condition_Delete_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Condition.DataRowCount > 0)
			{
				grd_Condition.BeginUpdate();
				gv_Condition.DeleteSelectedRows();
				grd_Condition.EndUpdate();
				grd_Condition.DatasourceTable().AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLineById(int id)
	{
		_ = 5;
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			grd_Doc.Enabled = false;
			DataSet dsBenefit = await _httpClient.PromotionProgram.GetLinesBenefit(id);
			if (lstPivotColBenefit.Count > 0)
			{
				foreach (string item in lstPivotColBenefit)
				{
					gv_Benefit.Columns.Remove(gv_Benefit.Columns[item]);
				}
				lstPivotColBenefit.Clear();
			}
			if (!dsBenefit.IsHasStructure())
			{
				return;
			}
			if (id == 0)
			{
				await GridViewHelper.GenerateGrid(grd_Benefit, gv_Benefit, dsBenefit);
				await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_PPR1", null, overrideOld: true);
				gv_Benefit.OptionsView.ColumnAutoWidth = false;
			}
			DataTable dataTable = dsBenefit.Tables[1].Copy();
			DataTable dtLineBenefit = dsBenefit.Tables[1].Clone();
			if (dtLineBenefit.Columns.Count == 0)
			{
				return;
			}
			int num = 1;
			if (dataTable.IsNotNull())
			{
				num = dataTable.AsEnumerable().Max((DataRow m) => Function.ParseInt(m["ColId"]));
			}
			for (int num2 = 1; num2 <= num; num2++)
			{
				dtLineBenefit.Columns.Add(num2.ToString(), typeof(string));
				GridColumnEx gridColumnEx = gv_Benefit.ColumnsEx.AddEx();
				gridColumnEx.Name = $"cl_{num2}";
				gridColumnEx.FieldName = num2.ToString();
				gridColumnEx.Caption = $"Benefit No. {num2}";
				gridColumnEx.Visible = true;
				gridColumnEx.Width = 150;
				gridColumnEx.OptionsFilter.AllowFilter = true;
				gridColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
				gridColumnEx.ColumnEdit = rep_BenefitCode;
				lstPivotColBenefit.Add(num2.ToString());
			}
			foreach (var item2 in from t in dataTable.AsEnumerable()
				group t by new
				{
					rowId = Function.ParseInt(t["RowId"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				})
			{
				DataRow dataRow = dtLineBenefit.NewRow();
				dataRow.ItemArray = item2.Items.FirstOrDefault().ItemArray;
				foreach (DataRow item3 in item2.Items)
				{
					dataRow[Function.ToString(item3["ColId"])] = item3["BenefitCode"];
				}
				dtLineBenefit.Rows.Add(dataRow);
			}
			grd_Benefit.DataSource = dtLineBenefit;
			grd_Benefit.RefreshDataSource();
			if (dtLineBenefit.IsNotNull())
			{
				AddNewBlankPivotColumnBenefit((num + 1).ToString());
			}
			DataSet dsCondition = await _httpClient.PromotionProgram.GetLinesCondition(id);
			if (lstPivotColCondition.Count > 0)
			{
				foreach (string item4 in lstPivotColCondition)
				{
					gv_Condition.Columns.Remove(gv_Condition.Columns[item4]);
				}
				lstPivotColCondition.Clear();
			}
			if (!dsCondition.IsHasStructure())
			{
				return;
			}
			if (id == 0)
			{
				await GridViewHelper.GenerateGrid(grd_Condition, gv_Condition, dsCondition);
				await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_PPR2", null, overrideOld: true);
				gv_Condition.OptionsView.ColumnAutoWidth = false;
			}
			DataTable dataTable2 = dsCondition.Tables[1].Copy();
			DataTable dataTable3 = dsCondition.Tables[1].Clone();
			if (dataTable3.Columns.Count == 0)
			{
				return;
			}
			int num3 = 1;
			if (dataTable2.IsNotNull())
			{
				num3 = dataTable2.AsEnumerable().Max((DataRow m) => Function.ParseInt(m["ColId"]));
			}
			for (int num4 = 1; num4 <= num3; num4++)
			{
				dataTable3.Columns.Add(num4.ToString(), typeof(string));
				GridColumnEx gridColumnEx2 = gv_Condition.ColumnsEx.AddEx();
				gridColumnEx2.Name = $"cl_{num4}";
				gridColumnEx2.FieldName = num4.ToString();
				gridColumnEx2.Caption = $"Condition No. {num4}";
				gridColumnEx2.Visible = true;
				gridColumnEx2.Width = 150;
				gridColumnEx2.OptionsFilter.AllowFilter = true;
				gridColumnEx2.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
				gridColumnEx2.ColumnEdit = rep_ConditionCode;
				lstPivotColCondition.Add(num4.ToString());
			}
			foreach (var item5 in from t in dataTable2.AsEnumerable()
				group t by new
				{
					rowId = Function.ParseInt(t["RowId"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				})
			{
				DataRow dataRow2 = dataTable3.NewRow();
				dataRow2.ItemArray = item5.Items.FirstOrDefault().ItemArray;
				foreach (DataRow item6 in item5.Items)
				{
					dataRow2[Function.ToString(item6["ColId"])] = item6["ConditionCode"];
				}
				dataTable3.Rows.Add(dataRow2);
			}
			grd_Condition.DataSource = dataTable3;
			grd_Condition.RefreshDataSource();
			if (dataTable3.IsNotNull())
			{
				AddNewBlankPivotColumnCondition((num3 + 1).ToString());
			}
			if (dtLineBenefit.IsNotNull() && dtLineBenefit.IsExistsColumn("PakageNo") && gv_Condition.IsExistColumn("PakageNo") && dtLineBenefit.IsExistsColumn("BenefitGroup") && gv_Condition.IsExistColumn("BenefitGroup"))
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
				foreach (DataRow row in dtLineBenefit.Rows)
				{
					string pakageNo = Function.ToString(row["PakageNo"]);
					if (!string.IsNullOrEmpty(pakageNo) && !list.Any((Common.ParameterValue m) => m.Value.Equals(pakageNo)))
					{
						list.Add(new Common.ParameterValue(pakageNo, pakageNo));
					}
					string benefitGroup = Function.ToString(row["BenefitGroup"]);
					if (!string.IsNullOrEmpty(benefitGroup) && !list2.Any((Common.ParameterValue m) => m.Value.Equals(benefitGroup)))
					{
						list2.Add(new Common.ParameterValue(benefitGroup, benefitGroup));
					}
				}
				RepositoryItemLookUpEdit columnEdit = new RepositoryItemLookUpEdit
				{
					DataSource = list,
					ValueMember = "Value",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				RepositoryItemLookUpEdit columnEdit2 = new RepositoryItemLookUpEdit
				{
					DataSource = list2,
					ValueMember = "Value",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				gv_Condition.Columns["PakageNo"].ColumnEdit = columnEdit;
				gv_Condition.Columns["BenefitGroup"].ColumnEdit = columnEdit2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		grd_Doc.Enabled = true;
		_isLoading = false;
		WinCommons._CloseProcessing();
	}

	private void AddNewBlankLineBenefit()
	{
		try
		{
			if (!grd_Doc.DataSourceTable.IsNotNull())
			{
				return;
			}
			_dtLineBenefit = grd_Benefit.DataSourceTable();
			if (_dtLineBenefit != null || _dtLineBenefit.Columns.Count != 0)
			{
				if (_dtLineBenefit.Rows.Count == 0 && gv_Condition.IsExistColumn("PakageNo") && gv_Condition.IsExistColumn("BenefitGroup"))
				{
					List<Common.ParameterValue> dataSource = new List<Common.ParameterValue>();
					List<Common.ParameterValue> dataSource2 = new List<Common.ParameterValue>();
					RepositoryItemLookUpEdit columnEdit = new RepositoryItemLookUpEdit
					{
						DataSource = dataSource,
						ValueMember = "Value",
						DisplayMember = "Name",
						NullText = "",
						BestFitMode = BestFitMode.BestFitResizePopup
					};
					RepositoryItemLookUpEdit columnEdit2 = new RepositoryItemLookUpEdit
					{
						DataSource = dataSource2,
						ValueMember = "Value",
						DisplayMember = "Name",
						NullText = "",
						BestFitMode = BestFitMode.BestFitResizePopup
					};
					gv_Condition.Columns["PakageNo"].ColumnEdit = columnEdit;
					gv_Condition.Columns["BenefitGroup"].ColumnEdit = columnEdit2;
				}
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				DataRow dataRow = _dtLineBenefit.NewRow();
				dataRow["DocEntry"] = num;
				_dtLineBenefit.Rows.Add(dataRow);
				_dtLineBenefit.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewBlankLineCondition()
	{
		try
		{
			if (grd_Doc.DataSourceTable.IsNotNull())
			{
				_dtLineCondition = grd_Condition.DataSourceTable();
				if (_dtLineCondition != null || _dtLineCondition.Columns.Count != 0)
				{
					int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
					DataRow dataRow = _dtLineCondition.NewRow();
					dataRow["DocEntry"] = num;
					_dtLineCondition.Rows.Add(dataRow);
					_dtLineCondition.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewBlankPivotColumnBenefit(string colName)
	{
		try
		{
			DataTable dataTable = grd_Benefit.DataSourceTable();
			dataTable.Columns.Add(colName, typeof(string));
			GridColumnEx gridColumnEx = gv_Benefit.ColumnsEx.AddEx();
			gridColumnEx.Name = $"cl_{colName}";
			gridColumnEx.FieldName = colName;
			gridColumnEx.Caption = $"Benefit No. {colName}";
			gridColumnEx.Visible = true;
			gridColumnEx.Width = 150;
			gridColumnEx.OptionsFilter.AllowFilter = true;
			gridColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
			gridColumnEx.ColumnEdit = rep_BenefitCode;
			lstPivotColBenefit.Add(colName);
			grd_Benefit.DataSource = dataTable;
			grd_Benefit.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewBlankPivotColumnCondition(string colName)
	{
		try
		{
			DataTable dataTable = grd_Condition.DataSourceTable();
			dataTable.Columns.Add(colName, typeof(string));
			GridColumnEx gridColumnEx = gv_Condition.ColumnsEx.AddEx();
			gridColumnEx.Name = $"cl_{colName}";
			gridColumnEx.FieldName = colName;
			gridColumnEx.Caption = $"Condition No. {colName}";
			gridColumnEx.Visible = true;
			gridColumnEx.Width = 150;
			gridColumnEx.OptionsFilter.AllowFilter = true;
			gridColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
			gridColumnEx.ColumnEdit = rep_ConditionCode;
			lstPivotColCondition.Add(colName);
			grd_Condition.DataSource = dataTable;
			grd_Condition.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
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

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				_isLoading = true;
				grd_Doc.DatasourceTable().Rows.Clear();
				grd_Benefit.DatasourceTable().Rows.Clear();
				grd_Condition.DatasourceTable().Rows.Clear();
				string value = Function.ToString(luk_Status.EditValue);
				DateTime dateTime = (string.IsNullOrWhiteSpace(date_fromDate.Text) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(date_fromDate.EditValue));
				DateTime dateTime2 = (string.IsNullOrWhiteSpace(date_toDate.Text) ? DateTime.Today : Convert.ToDateTime(date_toDate.EditValue));
				CreateDataSearch();
				DataRow dataRow = dtSearch.NewRow();
				dataRow["code"] = txt_Code.Text;
				dataRow["fromDate"] = dateTime;
				dataRow["toDate"] = dateTime2;
				dataRow["active"] = value;
				dtSearch.Rows.Add(dataRow);
				_isLoading = false;
				DataSet ds = await _httpClient.PromotionProgram.Search(dtSearch.Copy());
				await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds);
				_dtDocOriginal = grd_Doc.DataSourceTable.Copy();
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = true;
				AddColumnAttachmentButton();
				ResetAttachment();
				FormMode = Enums.FormViewModeType.View;
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
			if (!_isLoading && FormMode != Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
				{
					e.Allow = false;
					return;
				}
				ResetAttachment();
				grd_Doc.DataSource = _dtDocOriginal.Copy();
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			_ = gv_Doc.FocusedRowHandle;
			if (e.Column.FieldName == "DocEntry")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Benefit_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			DataTable dataTable = grd_Benefit.DataSourceTable();
			int num = lstPivotColBenefit.Max((string m) => Function.ParseInt(m));
			if (!string.IsNullOrEmpty(Function.ToString(e.Value)) && e.Column.FieldName.Equals(num.ToString()))
			{
				AddNewBlankPivotColumnBenefit((num + 1).ToString());
			}
			if ((from t in dataTable.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t[e.Column.FieldName]))
				select t).ToList().Count <= 0)
			{
				AddNewBlankLineBenefit();
			}
			if (e.Column.FieldName.Equals("PakageNo") && gv_Condition.IsExistColumn("PakageNo"))
			{
				List<Common.ParameterValue> list = new List<Common.ParameterValue>();
				foreach (DataRow row in dataTable.Rows)
				{
					string value = Function.ToString(row[e.Column.FieldName]);
					if (!string.IsNullOrEmpty(value) && !list.Any((Common.ParameterValue m) => m.Value.Equals(value)))
					{
						list.Add(new Common.ParameterValue(value, value));
					}
				}
				RepositoryItemLookUpEdit columnEdit = new RepositoryItemLookUpEdit
				{
					DataSource = list,
					ValueMember = "Value",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				gv_Condition.Columns["PakageNo"].ColumnEdit = columnEdit;
			}
			if (e.Column.FieldName.Equals("BenefitGroup") && gv_Condition.IsExistColumn("BenefitGroup"))
			{
				List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
				foreach (DataRow row2 in dataTable.Rows)
				{
					string value2 = Function.ToString(row2[e.Column.FieldName]);
					if (!string.IsNullOrEmpty(value2) && !list2.Any((Common.ParameterValue m) => m.Value.Equals(value2)))
					{
						list2.Add(new Common.ParameterValue(value2, value2));
					}
				}
				RepositoryItemLookUpEdit columnEdit2 = new RepositoryItemLookUpEdit
				{
					DataSource = list2,
					ValueMember = "Value",
					DisplayMember = "Name",
					NullText = "",
					BestFitMode = BestFitMode.BestFitResizePopup
				};
				gv_Condition.Columns["BenefitGroup"].ColumnEdit = columnEdit2;
			}
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

	private void gv_Condition_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				DataTable source = grd_Condition.DataSourceTable();
				int num = lstPivotColCondition.Max((string m) => Function.ParseInt(m));
				if (!string.IsNullOrEmpty(Function.ToString(e.Value)) && e.Column.FieldName.Equals(num.ToString()))
				{
					AddNewBlankPivotColumnCondition((num + 1).ToString());
				}
				if ((from t in source.AsEnumerable()
					where string.IsNullOrWhiteSpace(Function.ToString(t[e.Column.FieldName]))
					select t).ToList().Count <= 0)
				{
					AddNewBlankLineCondition();
				}
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode == Enums.FormViewModeType.Add && e.Column.FieldName == "Code" && dtAttachment.IsNotNull())
			{
				string value = Function.ToString(e.Value);
				for (int i = 0; i < dtAttachment.Rows.Count; i++)
				{
					dtAttachment.Rows[i]["ObjectCode"] = value;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Condition_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle >= 0 && gv_Condition.IsExistColumn("PakageNo") && gv_Condition.IsExistColumn("BenefitGroup") && gv_Benefit.IsExistColumn("PakageNo") && gv_Benefit.IsExistColumn("BenefitGroup"))
			{
				string text = Function.ToString(gv_Benefit.GetFocusedRowCellValue("PakageNo"));
				string text2 = Function.ToString(gv_Benefit.GetFocusedRowCellValue("BenefitGroup"));
				string value = Function.ToString(gv_Condition.GetRowCellValue(e.RowHandle, "PakageNo"));
				string value2 = Function.ToString(gv_Condition.GetRowCellValue(e.RowHandle, "BenefitGroup"));
				if (text.Equals(value) && text2.Equals(value2))
				{
					e.Appearance.BackColor = Color.FromName("lightblue");
					e.HighPriority = true;
				}
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Promotions.frm_PromotionProgram));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Doc_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Doc_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Doc_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Benefit = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Benefit = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Benefit_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Benefit_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Benefit = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Condition = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Condition = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Condition_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.ctm_Condition_Delete = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Condition = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_fromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_toDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Benefit).BeginInit();
		this.ctm_Benefit.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Benefit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Condition).BeginInit();
		this.ctm_Condition.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Condition).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 59);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainerControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1298, 620);
		this.splitContainerControl1.SplitterPosition = 201;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemButtonEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(1298, 201);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.ctm_Doc_Add, this.ctm_Doc_Duplicate, this.ctm_Doc_Delete });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(143, 76);
		this.ctm_Doc_Add.Name = "ctm_Doc_Add";
		this.ctm_Doc_Add.Size = new System.Drawing.Size(142, 24);
		this.ctm_Doc_Add.Text = "Add New";
		this.ctm_Doc_Add.Click += new System.EventHandler(this.ctm_Doc_Add_Click);
		this.ctm_Doc_Duplicate.Name = "ctm_Doc_Duplicate";
		this.ctm_Doc_Duplicate.Size = new System.Drawing.Size(142, 24);
		this.ctm_Doc_Duplicate.Text = "Duplicate";
		this.ctm_Doc_Duplicate.Click += new System.EventHandler(this.ctm_Doc_Duplicate_Click);
		this.ctm_Doc_Delete.Name = "ctm_Doc_Delete";
		this.ctm_Doc_Delete.Size = new System.Drawing.Size(142, 24);
		this.ctm_Doc_Delete.Text = "Delete";
		this.ctm_Doc_Delete.Click += new System.EventHandler(this.ctm_Doc_Delete_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DetailHeight = 431;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndicatorWidth = 41;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.repositoryItemButtonEdit1.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.repositoryItemButtonEdit1.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Benefit);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_Condition);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(1298, 407);
		this.splitContainerControl2.SplitterPosition = 198;
		this.splitContainerControl2.TabIndex = 1;
		this.grd_Benefit.AllowPasteExcel = true;
		this.grd_Benefit.ClipboardData = null;
		this.grd_Benefit.ContextMenuStrip = this.ctm_Benefit;
		this.grd_Benefit.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Benefit.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Benefit.Location = new System.Drawing.Point(0, 0);
		this.grd_Benefit.MainView = this.gv_Benefit;
		this.grd_Benefit.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Benefit.Name = "grd_Benefit";
		this.grd_Benefit.Size = new System.Drawing.Size(1298, 198);
		this.grd_Benefit.TabIndex = 0;
		this.grd_Benefit.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Benefit });
		this.ctm_Benefit.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Benefit.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_Benefit_Add, this.ctm_Benefit_Delete });
		this.ctm_Benefit.Name = "ctm_Line";
		this.ctm_Benefit.Size = new System.Drawing.Size(156, 52);
		this.ctm_Benefit_Add.Name = "ctm_Benefit_Add";
		this.ctm_Benefit_Add.Size = new System.Drawing.Size(155, 24);
		this.ctm_Benefit_Add.Text = "Add Row";
		this.ctm_Benefit_Add.Click += new System.EventHandler(this.ctm_Benefit_Add_Click);
		this.ctm_Benefit_Delete.Name = "ctm_Benefit_Delete";
		this.ctm_Benefit_Delete.Size = new System.Drawing.Size(155, 24);
		this.ctm_Benefit_Delete.Text = "Delete Row";
		this.ctm_Benefit_Delete.Click += new System.EventHandler(this.ctm_Benefit_Delete_Click);
		this.gv_Benefit.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Benefit.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Benefit.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Benefit.ColumnKey = null;
		this.gv_Benefit.DetailHeight = 431;
		this.gv_Benefit.DisableFormSetting = false;
		this.gv_Benefit.DisplayException = false;
		this.gv_Benefit.FieldCurrency = null;
		this.gv_Benefit.FieldRate = null;
		this.gv_Benefit.FormId = "";
		this.gv_Benefit.GridControl = this.grd_Benefit;
		this.gv_Benefit.IndicatorWidth = 41;
		this.gv_Benefit.IsApzTable = false;
		this.gv_Benefit.IsInsertNewRow = false;
		this.gv_Benefit.IsLoadingData = false;
		this.gv_Benefit.ItemId = "";
		this.gv_Benefit.LstColumnAllowBatch = null;
		this.gv_Benefit.LstColumnNotAllowBatch = null;
		this.gv_Benefit.LstColumnNotEditable = null;
		this.gv_Benefit.LstColumNotSetting = null;
		this.gv_Benefit.Name = "gv_Benefit";
		this.gv_Benefit.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Benefit.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Benefit.OptionsPrint.AutoWidth = false;
		this.gv_Benefit.OptionsPrint.UsePrintStyles = false;
		this.gv_Benefit.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Benefit.OptionsView.ColumnAutoWidth = false;
		this.gv_Benefit.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Benefit.OptionsView.ShowGroupedColumns = true;
		this.gv_Benefit.OptionsView.ShowGroupPanel = false;
		this.gv_Benefit.OptionsView.ShowViewCaption = true;
		this.gv_Benefit.UseBatchMod = false;
		this.gv_Benefit.ViewCaption = "Benefits";
		this.gv_Benefit.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Benefit_CellValueChanged);
		this.grd_Condition.AllowPasteExcel = true;
		this.grd_Condition.ClipboardData = null;
		this.grd_Condition.ContextMenuStrip = this.ctm_Condition;
		this.grd_Condition.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Condition.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Condition.Location = new System.Drawing.Point(0, 0);
		this.grd_Condition.MainView = this.gv_Condition;
		this.grd_Condition.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.grd_Condition.Name = "grd_Condition";
		this.grd_Condition.Size = new System.Drawing.Size(1298, 197);
		this.grd_Condition.TabIndex = 1;
		this.grd_Condition.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Condition });
		this.ctm_Condition.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Condition.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.ctm_Condition_Add, this.ctm_Condition_Delete });
		this.ctm_Condition.Name = "ctm_Line";
		this.ctm_Condition.Size = new System.Drawing.Size(156, 52);
		this.ctm_Condition_Add.Name = "ctm_Condition_Add";
		this.ctm_Condition_Add.Size = new System.Drawing.Size(155, 24);
		this.ctm_Condition_Add.Text = "Add Row";
		this.ctm_Condition_Add.Click += new System.EventHandler(this.ctm_Condition_Add_Click);
		this.ctm_Condition_Delete.Name = "ctm_Condition_Delete";
		this.ctm_Condition_Delete.Size = new System.Drawing.Size(155, 24);
		this.ctm_Condition_Delete.Text = "Delete Row";
		this.ctm_Condition_Delete.Click += new System.EventHandler(this.ctm_Condition_Delete_Click);
		this.gv_Condition.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Condition.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Condition.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Condition.ColumnKey = null;
		this.gv_Condition.DetailHeight = 431;
		this.gv_Condition.DisableFormSetting = false;
		this.gv_Condition.DisplayException = false;
		this.gv_Condition.FieldCurrency = null;
		this.gv_Condition.FieldRate = null;
		this.gv_Condition.FormId = "";
		this.gv_Condition.GridControl = this.grd_Condition;
		this.gv_Condition.IndicatorWidth = 41;
		this.gv_Condition.IsApzTable = false;
		this.gv_Condition.IsInsertNewRow = false;
		this.gv_Condition.IsLoadingData = false;
		this.gv_Condition.ItemId = "";
		this.gv_Condition.LstColumnAllowBatch = null;
		this.gv_Condition.LstColumnNotAllowBatch = null;
		this.gv_Condition.LstColumnNotEditable = null;
		this.gv_Condition.LstColumNotSetting = null;
		this.gv_Condition.Name = "gv_Condition";
		this.gv_Condition.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Condition.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Condition.OptionsPrint.AutoWidth = false;
		this.gv_Condition.OptionsPrint.UsePrintStyles = false;
		this.gv_Condition.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Condition.OptionsView.ColumnAutoWidth = false;
		this.gv_Condition.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Condition.OptionsView.ShowGroupedColumns = true;
		this.gv_Condition.OptionsView.ShowGroupPanel = false;
		this.gv_Condition.OptionsView.ShowViewCaption = true;
		this.gv_Condition.UseBatchMod = false;
		this.gv_Condition.ViewCaption = "Conditions";
		this.gv_Condition.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Condition_RowStyle);
		this.gv_Condition.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Condition_CellValueChanged);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(1, 687);
		this.btn_Ok.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(88, 28);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(96, 687);
		this.btn_Cancel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(88, 28);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(222, 22);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(84, 25);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(426, 22);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(84, 25);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 6;
		this.labelLine3.Utext = "To Date";
		this.date_fromDate.EditControl = null;
		this.date_fromDate.EditValue = null;
		this.date_fromDate.FieldName = null;
		this.date_fromDate.Location = new System.Drawing.Point(302, 22);
		this.date_fromDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.date_fromDate.Name = "date_fromDate";
		this.date_fromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.IsSearchControl = false;
		this.date_fromDate.SettingInVisible = false;
		this.date_fromDate.SettingReadonly = false;
		this.date_fromDate.Size = new System.Drawing.Size(117, 22);
		this.date_fromDate.TabIndex = 7;
		this.date_toDate.EditControl = null;
		this.date_toDate.EditValue = null;
		this.date_toDate.FieldName = null;
		this.date_toDate.Location = new System.Drawing.Point(498, 22);
		this.date_toDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.date_toDate.Name = "date_toDate";
		this.date_toDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_toDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_toDate.Properties.IsSearchControl = false;
		this.date_toDate.SettingInVisible = false;
		this.date_toDate.SettingReadonly = false;
		this.date_toDate.Size = new System.Drawing.Size(117, 22);
		this.date_toDate.TabIndex = 8;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Active";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(631, 22);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(64, 25);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 9;
		this.labelLine4.Utext = "Active";
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(690, 22);
		this.luk_Status.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(117, 22);
		this.luk_Status.TabIndex = 10;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(839, 20);
		this.btn_Search.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(88, 28);
		this.btn_Search.TabIndex = 11;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(8, 22);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(52, 25);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 13;
		this.labelLine1.Utext = "Code";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(61, 22);
		this.txt_Code.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(152, 22);
		this.txt_Code.TabIndex = 12;
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1302, 720);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.date_toDate);
		base.Controls.Add(this.date_fromDate);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PromotionProgram.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
		base.Name = "frm_PromotionProgram";
		this.Text = "Promotion Programs";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Benefit).EndInit();
		this.ctm_Benefit.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Benefit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Condition).EndInit();
		this.ctm_Condition.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Condition).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
