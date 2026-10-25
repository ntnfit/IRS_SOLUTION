using System;
using System.Collections;
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
using Apzon.Controls.Pivot;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Reports;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;
using SAPbobsCOM;

namespace Apzon.Forms.Mrps;

public class frm_JetstarPreOrder : XtraFormEx
{
	private const string UpDateString = "Update";

	private const string OkString = "OK";

	private const string FindString = "Find";

	private const string Runstring = "Run";

	private const string SaveString = "Save";

	private const string Controller = "JetstarPreOrder";

	private readonly RepositoryItemTextEdit _commentsRepositoryItem = new RepositoryItemTextEdit();

	private RepositoryItemButtonEdit _vendorSaRepositotyItem = new RepositoryItemButtonEdit();

	private RepositoryItemButtonEdit _vendorBaRepositotyItem = new RepositoryItemButtonEdit();

	private IContainer components;

	private PivotGridControlEx pvgDoc;

	private DateEditEx txtSearchDate;

	private LabelLine lbSearchDate;

	private SimpleButton btnSearch;

	private SimpleButton btnRun;

	private SimpleButton btnCancel;

	private PivotGridField pgcOrderType;

	private PivotGridField pgcFliNo;

	private PivotGridField pgcSector;

	private PivotGridField pgcDeliTime;

	private PivotGridField pgcSsrCode;

	private PivotGridField pgcQty;

	private RepositoryItemCalcEdit rQuantity;

	private RepositoryItemLookUpEdit rOrderType;

	private PivotGridField pgcFltTime;

	private LabelLine lbMrpType;

	private LookUpEditEx txtType;

	private PivotGridField pgcFltType;

	private SimpleButton btnPreview;

	private PivotGridField pgcTotalPax;

	private SimpleButton btnGenPurchase;

	private PivotGridField pgcVisOder;

	private SimpleButton btnManualSort;

	private PivotGridField gcWhs;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btnGenPurchase.Enabled = false;
				btnPreview.Enabled = false;
				btnRun.Text = "Save";
				break;
			case Enums.FormViewModeType.Update:
				btnGenPurchase.Enabled = false;
				btnPreview.Enabled = false;
				btnRun.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
			{
				DataTable dataTable = pvgDoc.DataSourceTable();
				btnGenPurchase.Enabled = dataTable.IsNotNull();
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => !Function.ToString(t["SsrName"]).StartsWith("zzz") && Function.ParseInt(t["Quantity"]) > 0 && Function.ParseInt(t["TargetDoc"]) == 0);
					btnPreview.Enabled = dataRow == null;
				}
				else
				{
					btnPreview.Enabled = pvgDoc.DataSourceTable().IsNotNull();
				}
				btnRun.Text = "OK";
				break;
			}
			case Enums.FormViewModeType.Run:
				btnGenPurchase.Enabled = false;
				btnPreview.Enabled = false;
				btnRun.Text = "Run";
				break;
			case Enums.FormViewModeType.Busy:
				btnGenPurchase.Enabled = false;
				btnPreview.Enabled = false;
				break;
			case Enums.FormViewModeType.Find:
				break;
			}
		}
		catch
		{
		}
	}

	public frm_JetstarPreOrder()
	{
		InitializeComponent();
		_commentsRepositoryItem.Enter += EditComments_Enter;
		_vendorSaRepositotyItem.Enter += EditVendorSa_Enter;
		_vendorSaRepositotyItem.ButtonClick += EditVendorSa_ButtonClick;
		_vendorBaRepositotyItem.Enter += EditVendorBa_Enter;
		_vendorBaRepositotyItem.ButtonClick += EditVendorBa_ButtonClick;
	}

	private void EditComments_Enter(object sender, EventArgs e)
	{
		try
		{
			if (sender is TextEdit textEdit)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pvgDoc.CreateDrillDownDataSource(pvgDoc.Cells.FocusedCell.X, pvgDoc.Cells.FocusedCell.Y);
				if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount != 0)
				{
					textEdit.EditValue = Function.ToString(pivotDrillDownDataSource[0]["Comments"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EditVendorSa_Enter(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is TextEdit textEdit))
			{
				return;
			}
			PivotDrillDownDataSource scheduleSource = pvgDoc.CreateDrillDownDataSource(pvgDoc.Cells.FocusedCell.X, pvgDoc.Cells.FocusedCell.Y);
			if (scheduleSource != null && scheduleSource.RowCount != 0)
			{
				DataRow dataRow = pvgDoc.DataSourceTable().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FltNo"]).Equals(Function.ToString(scheduleSource[0]["FltNo"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(scheduleSource[0]["FltTime"]) && Function.ToString(t["ItemType"]).Equals("SA"));
				textEdit.EditValue = ((dataRow == null) ? "" : Function.ToString(dataRow["ShortName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EditVendorBa_Enter(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is TextEdit textEdit))
			{
				return;
			}
			PivotDrillDownDataSource scheduleSource = pvgDoc.CreateDrillDownDataSource(pvgDoc.Cells.FocusedCell.X, pvgDoc.Cells.FocusedCell.Y);
			if (scheduleSource != null && scheduleSource.RowCount != 0)
			{
				DataRow dataRow = pvgDoc.DataSourceTable().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FltNo"]).Equals(Function.ToString(scheduleSource[0]["FltNo"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(scheduleSource[0]["FltTime"]) && Function.ToString(t["ItemType"]).Equals("B"));
				textEdit.EditValue = ((dataRow == null) ? "" : Function.ToString(dataRow["ShortName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void EditVendorSa_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			PivotDrillDownDataSource scheduleSource = pvgDoc.CreateDrillDownDataSource(pvgDoc.Cells.FocusedCell.X, pvgDoc.Cells.FocusedCell.Y);
			if (scheduleSource == null || scheduleSource.RowCount == 0 || (!Function.ToString(scheduleSource[0]["Sector"]).StartsWith("DAD") && !Function.ToString(scheduleSource[0]["Sector"]).StartsWith("PQC-HAN") && !Function.ToString(scheduleSource[0]["Sector"]).Contains("CGO")))
			{
				return;
			}
			DataRowView dataRowView = await Cfl_Vendor();
			if (dataRowView != null)
			{
				DataTable dataTable = pvgDoc.DataSourceTable();
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["FltNo"]).Equals(Function.ToString(scheduleSource[0]["FltNo"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(scheduleSource[0]["FltTime"]) && Function.ToString(t["ItemType"]).Equals("SA")
					select t).ToList();
				if (sender is ButtonEdit buttonEdit)
				{
					buttonEdit.EditValue = dataRowView["ShortName"];
				}
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["CardCode"] = dataRowView["CardCode"];
					list[num]["CardName"] = dataRowView["CardName"];
					list[num]["ShortName"] = dataRowView["ShortName"];
				}
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void EditVendorBa_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			PivotDrillDownDataSource scheduleSource = pvgDoc.CreateDrillDownDataSource(pvgDoc.Cells.FocusedCell.X, pvgDoc.Cells.FocusedCell.Y);
			if (scheduleSource == null || scheduleSource.RowCount == 0 || (!Function.ToString(scheduleSource[0]["Sector"]).StartsWith("DAD") && !Function.ToString(scheduleSource[0]["Sector"]).StartsWith("PQC-HAN") && !Function.ToString(scheduleSource[0]["Sector"]).Contains("CGO")))
			{
				return;
			}
			DataRowView dataRowView = await Cfl_Vendor();
			if (dataRowView != null)
			{
				DataTable dataTable = pvgDoc.DataSourceTable();
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["FltNo"]).Equals(Function.ToString(scheduleSource[0]["FltNo"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(scheduleSource[0]["FltTime"]) && Function.ToString(t["ItemType"]).Equals("B")
					select t).ToList();
				if (sender is ButtonEdit buttonEdit)
				{
					buttonEdit.EditValue = dataRowView["ShortName"];
				}
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["CardCode"] = dataRowView["CardCode"];
					list[num]["CardName"] = dataRowView["CardName"];
					list[num]["ShortName"] = dataRowView["ShortName"];
				}
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Find || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
		{
			Close();
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txtSearchDate.Text))
			{
				WinCommons.Warning(Enums.MessageCode.YouMustChooseSearchDate, "");
				return;
			}
			WinCommons._OpenProcessing();
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "JetstarPreOrder");
			DataTable dataTable = await http.GetAsync("Search", new
			{
				type = Function.ToString(txtType.EditValue),
				searchDate = Function.ParseDateTimes(txtSearchDate.EditValue).ToString("yyyyMMdd")
			});
			if (!dataTable.IsNotNull())
			{
				pvgDoc.DataSource = null;
				pvgDoc.Refresh();
				WinCommons.Warning(Enums.MessageCode.DataIsEmpty, "");
				WinCommons._CloseProcessing();
				return;
			}
			pvgDoc.DataSource = dataTable;
			pvgDoc.Refresh();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void frm_JetstarPreOrder_Load(object sender, EventArgs e)
	{
		try
		{
			InitFormatControl();
			LoadInitData();
			txtSearchDate.EditValue = DateTime.Now.Date.AddDays(1.0);
			txtType.EditValue = "QUOCNOI";
			FormMode = Enums.FormViewModeType.Run;
			pgcFliNo.SortBySummaryInfo.Conditions.Clear();
			pgcFliNo.SortBySummaryInfo.Field = pgcVisOder;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btnRun_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Run:
				WinCommons._OpenProcessing();
				txtSearchDate.EditValue = DateTime.Now.Date.AddDays(1.0);
				await RunPreOrder();
				break;
			case Enums.FormViewModeType.Update:
				await SaveMrp();
				break;
			case Enums.FormViewModeType.Add:
				await SaveMrp();
				break;
			default:
				Close();
				break;
			case Enums.FormViewModeType.Find:
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void pvgDoc_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField == null)
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.Appearance.BackColor = Color.OldLace;
				e.Appearance.BackColor2 = Color.OldLace;
			}
			else if (e.ColumnField.FieldName.Equals(pgcSsrCode.FieldName))
			{
				string text = Function.ToString(pvgDoc.GetFieldValue(pgcSsrCode, e.ColumnIndex));
				if (text.Equals("zzz00"))
				{
					e.Appearance.BackColor = Color.PaleGoldenrod;
				}
				else if (new string[4] { "BV01", "BV02", "BV03", "BV04" }.Contains(text))
				{
					e.Appearance.BackColor = Color.PeachPuff;
				}
				else if (new string[4] { "HM01", "HM02", "HM03", "HM04" }.Contains(text))
				{
					e.Appearance.BackColor = Color.PaleGreen;
				}
				else if (new string[5] { "HM05", "HM06", "HM07", "HM08", "CHML" }.Contains(text))
				{
					e.Appearance.BackColor = Color.MediumTurquoise;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pvgDoc_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
	{
		try
		{
			if (e == null || e.Field == null)
			{
				return;
			}
			if (e.Field.FieldName.Equals(pgcFltTime.FieldName))
			{
				int num = Function.ParseInt(e.Value);
				e.DisplayText = string.Format("{0}:{1}", (num / 100).ToString("00"), (num % 100).ToString("00"));
			}
			else if (e.Field.FieldName.Equals(pgcSsrCode.FieldName))
			{
				if (Function.ToString(e.Value).Equals("zzz00"))
				{
					e.DisplayText = "Total";
				}
				else if (Function.ToString(e.Value).Equals("zzz01"))
				{
					DataTable dataTable = pvgDoc.DataSourceTable();
					e.DisplayText = (Function.ToString(dataTable.Rows[0]["FltType"]).Equals("QUOCNOI") ? "NCC SA" : "NCC");
				}
				else if (Function.ToString(e.Value).Equals("zzz02"))
				{
					e.DisplayText = "NCC Bánh";
				}
				else if (Function.ToString(e.Value).Equals("zzz03"))
				{
					e.DisplayText = "Ghi chú";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pvgDoc_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
	{
		try
		{
			if (e == null || e.ColumnField != pgcSsrCode)
			{
				return;
			}
			if (e.ColumnIndex < 0 || e.RowIndex < 0)
			{
				return;
			}
			if ((e.RowValueType == PivotGridValueType.GrandTotal || e.RowValueType == PivotGridValueType.Total || e.RowValueType == PivotGridValueType.GrandTotal || e.RowValueType == PivotGridValueType.CustomTotal) && Function.ParseDouble(e.Value).Equals(0.0))
			{
				e.DisplayText = "";
			}
			PivotDrillDownDataSource scheduleSource = pvgDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
			if (scheduleSource == null || scheduleSource.RowCount == 0)
			{
				return;
			}
			DataTable dataTable = pvgDoc.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			string text = Function.ToString(scheduleSource[0]["SsrName"]);
			if (text.Equals("zzz01") && e.RowValueType == PivotGridValueType.Value)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FltNo"]).Equals(Function.ToString(scheduleSource[0]["FltNo"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(scheduleSource[0]["FltTime"]) && Function.ToString(t["ItemType"]).Equals("SA"));
				e.DisplayText = ((dataRow == null) ? "" : Function.ToString(dataRow["ShortName"]));
			}
			else if (text.Equals("zzz02") && e.RowValueType == PivotGridValueType.Value)
			{
				DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FltNo"]).Equals(Function.ToString(scheduleSource[0]["FltNo"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(scheduleSource[0]["FltTime"]) && Function.ToString(t["ItemType"]).Equals("B"));
				e.DisplayText = ((dataRow2 == null) ? "" : Function.ToString(dataRow2["ShortName"]));
			}
			else if (text.Equals("zzz03") && e.RowValueType == PivotGridValueType.Value)
			{
				e.DisplayText = Function.ToString(scheduleSource[0]["Comments"]);
			}
			else if (Function.ParseDouble(e.Value).Equals(0.0))
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pvgDoc_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		try
		{
			if (e != null && e.ColumnField != null && e.RowField != null && e.DataField == pgcQty)
			{
				if (e.Editor.EditValue == null)
				{
					e.Editor.EditValue = 0;
				}
				ChangeCellValue(e);
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pvgDoc_CustomFieldSort(object sender, PivotGridCustomFieldSortEventArgs e)
	{
		try
		{
			e.Handled = true;
			if (e.Field.FieldEditName.Equals(pgcSsrCode.FieldEditName) && e.Value1 != null && e.Value2 != null)
			{
				string value = Function.ToString(e.Value1);
				string value2 = Function.ToString(e.Value2);
				string[] source = new string[4] { "BV01", "BV02", "BV03", "BV04" };
				string[] source2 = new string[8] { "HM01", "HM02", "HM03", "HM04", "HM05", "HM06", "HM07", "HM08" };
				string text = "CHML";
				if (source.Contains(value) && source2.Contains(value2))
				{
					e.Result = 1;
				}
				else if (source.Contains(value2) && source2.Contains(value))
				{
					e.Result = -1;
				}
				else if (text.Equals(value2) && source2.Contains(value))
				{
					e.Result = -1;
				}
				else if (text.Equals(value) && source2.Contains(value2))
				{
					e.Result = 1;
				}
				else if (text.Equals(value2) && source.Contains(value))
				{
					e.Result = 1;
				}
				else if (text.Equals(value) && source.Contains(value2))
				{
					e.Result = -1;
				}
				else
				{
					e.Result = Comparer.Default.Compare(e.Value1, e.Value2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pvgDoc_ShowingEditor(object sender, CancelPivotCellEditEventArgs e)
	{
		try
		{
			if (e.ColumnValueType == PivotGridValueType.Total || e.ColumnValueType == PivotGridValueType.GrandTotal || e.RowValueType == PivotGridValueType.Total || e.RowValueType == PivotGridValueType.GrandTotal)
			{
				e.Cancel = true;
			}
			if (e.ColumnField != pgcSsrCode || e.RowFieldIndex < 0 || e.ColumnIndex < 0 || e.RowIndex < 0)
			{
				return;
			}
			PivotDrillDownDataSource pivotDrillDownDataSource = pvgDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
			if (pivotDrillDownDataSource == null || pivotDrillDownDataSource.RowCount == 0)
			{
				return;
			}
			string text = Function.ToString(pivotDrillDownDataSource[0][pgcSsrCode.FieldName]);
			if (text.Equals("zzz01") || text.Equals("zzz02"))
			{
				if (!Function.ToString(pivotDrillDownDataSource[0][pgcSector.FieldName]).StartsWith("DAD") && !Function.ToString(pivotDrillDownDataSource[0][pgcSector.FieldName]).Equals("PQC-HAN") && !Function.ToString(pivotDrillDownDataSource[0][pgcSector.FieldName]).Contains("CGO"))
				{
					e.Cancel = true;
				}
			}
			else if (text.Equals("zzz00"))
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

	private void pvgDoc_CustomCellEditForEditing(object sender, PivotCustomCellEditEventArgs e)
	{
		try
		{
			if (e.ColumnValueType == PivotGridValueType.Total || e.ColumnValueType == PivotGridValueType.GrandTotal || e.RowValueType == PivotGridValueType.Total || e.RowValueType == PivotGridValueType.GrandTotal || e.ColumnField != pgcSsrCode || e.RowFieldIndex < 0 || e.ColumnIndex < 0 || e.RowIndex < 0)
			{
				return;
			}
			PivotDrillDownDataSource pivotDrillDownDataSource = pvgDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
			if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount != 0)
			{
				string text = Function.ToString(pivotDrillDownDataSource[0][pgcSsrCode.FieldName]);
				if (text.Equals("zzz01"))
				{
					e.RepositoryItem = _vendorSaRepositotyItem;
				}
				else if (text.Equals("zzz02"))
				{
					e.RepositoryItem = _vendorBaRepositotyItem;
				}
				else if (text.Equals("zzz03"))
				{
					e.RepositoryItem = _commentsRepositoryItem;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnPreview_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = pvgDoc.DataSourceTable();
			if (dataTable.IsNotNull())
			{
				DataTable dataJetstarVendorPreview = (from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["CardCode"]))
					select t).AsDataView().ToTable(true, "CardCode", "ShortName");
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.JetstarPreviewBp, multiSelect: false, "");
				frm_ChooseFromList2.DataJetstarVendorPreview = dataJetstarVendorPreview;
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					List<Common.ParameterValue> list = new List<Common.ParameterValue>();
					list.Add(new Common.ParameterValue
					{
						Name = "[%0]",
						Value = Function.ParseInt(dataTable.Rows[0]["Code"])
					});
					list.Add(new Common.ParameterValue
					{
						Name = "[%1]",
						Value = Function.ToString(frm_ChooseFromList2.ChooseItem["ShortName"])
					});
					list.Add(new Common.ParameterValue
					{
						Name = "[%2]",
						Value = Function.ParseDateTimes(dataTable.Rows[0]["MrpDate"]).Date
					});
					frm_ReportForm frm_ReportForm2 = new frm_ReportForm(Function.ToString(dataTable.Rows[0]["FltType"]).Equals("QUOCNOI") ? "PreviewQN" : "PreviewQT");
					frm_ReportForm2.defaultParamValue = list;
					frm_ReportForm2.Name = (Function.ToString(dataTable.Rows[0]["FltType"]).Equals("QUOCNOI") ? "frm_PreviewQN" : "frm_PreviewQT");
					frm_ReportForm2.Text = string.Format("Preview Order - nhà cung cấp: {0} - ngày: {1}", Function.ToString(frm_ChooseFromList2.ChooseItem["ShortName"]), Function.ParseDateTimes(dataTable.Rows[0]["MrpDate"]).Date.ToString("dd/MM/yyyy"));
					frm_ReportForm2.ShowDialog();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pvgDoc_CustomColumnWidth(object sender, PivotCustomColumnWidthEventArgs e)
	{
		try
		{
			string text = Function.ToString(pvgDoc.GetFieldValue(pgcSsrCode, e.ColumnIndex));
			if (text.Equals("zzz00"))
			{
				e.ColumnWidth = 50;
			}
			else if (text.Equals("zzz01") || text.Equals("zzz02"))
			{
				e.ColumnWidth = 60;
			}
			else if (text.Equals("zzz03"))
			{
				e.ColumnWidth = 200;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btnGenPurchase_Click(object sender, EventArgs e)
	{
		try
		{
			await SaveOrder();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void btnManualSort_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = pvgDoc.DataSourceTable().AsDataView().ToTable(true, "FltNo", "Sector", "FltDate", "FltTime", "VisOrder");
			if (!dataTable.IsNotNull())
			{
				return;
			}
			DataTable jetstarDataFltSort = (from m in dataTable.AsEnumerable()
				orderby Function.ParseInt(m["VisOrder"])
				select m).CopyToDataTable();
			frm_ChooseFromList frm = new frm_ChooseFromList(Enums.TypeCFL.JetstarFltSort, multiSelect: false, "");
			frm.JetstarDataFltSort = jetstarDataFltSort;
			frm.ShowDialog(this);
			DataTable dataTable2 = pvgDoc.DataSourceTable();
			int i;
			for (i = 0; i < frm.JetstarDataFltSort.Rows.Count; i++)
			{
				List<DataRow> list = (from t in dataTable2.AsEnumerable()
					where Function.ToString(t["FltNo"]).Equals(Function.ToString(frm.JetstarDataFltSort.Rows[i]["FltNo"])) && Function.ToString(t["Sector"]).Equals(Function.ToString(frm.JetstarDataFltSort.Rows[i]["Sector"])) && Function.ParseInt(t["FltTime"]) == Function.ParseInt(frm.JetstarDataFltSort.Rows[i]["FltTime"]) && Function.ParseDateTimes(t["FltDate"]) == Function.ParseDateTimes(frm.JetstarDataFltSort.Rows[i]["FltDate"])
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["VisOrder"] = i + 1;
				}
			}
			dataTable2.AcceptChanges();
			pvgDoc.DataSource = dataTable2;
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
			txtSearchDate.EditValue = DateTime.Now.Date.AddDays(1.0);
			pvgDoc.DataSource = null;
			pvgDoc.Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Run;
	}

	private void InitFormatControl()
	{
		try
		{
			txtSearchDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			pgcDeliTime.SetFormatControl(Enums.DisplayFormatType.DateTime);
			pgcQty.SetFormatControl(Enums.DisplayFormatType.Qty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadInitData()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "P";
			dataRow[1] = "Pre - Order";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow[0] = "A";
			dataRow[1] = "Tổ tiếp viên";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow[0] = "M";
			dataRow[1] = "Đăt thêm";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			rOrderType.DataSource = dataTable;
			DataTable dataTable2 = dataTable.Clone();
			dataRow = dataTable2.NewRow();
			dataRow[0] = "QUOCNOI";
			dataRow[1] = "QUOCNOI";
			dataTable2.Rows.Add(dataRow);
			dataRow = dataTable2.NewRow();
			dataRow[0] = "QUOCTE";
			dataRow[1] = "QUOCTE";
			dataTable2.Rows.Add(dataRow);
			txtType.Properties.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RunPreOrder()
	{
		using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "JetstarPreOrder"))
		{
			DataTable dataTable = await http.GetAsync("Run", new
			{
				type = Function.ToString(txtType.EditValue)
			});
			if (!dataTable.IsNotNull())
			{
				pvgDoc.DataSource = null;
				WinCommons.Error(Enums.MessageCode.MrpDataIsEmpty, "");
				return;
			}
			DataTable dt = dataTable.AsDataView().ToTable(true, "FltNo", "VisOrder", "WhsFlt", "FltDate", "FltTime", "DelTime", "Sector");
			List<DataRow> list = (from m in dt.AsEnumerable()
				orderby Function.ToString(m["WhsFlt"]), Function.ParseDateTimes(m["FltDate"]), Function.ParseInt(m["FltTime"])
				select m).ToList();
			string value = "";
			string text = "";
			string value2 = "";
			for (int num = 0; num < list.Count; num++)
			{
				if (string.IsNullOrEmpty(Function.ToString(list[num]["WhsFlt"])) && string.IsNullOrEmpty(Function.ToString(list[num]["Sector"])))
				{
					list[num]["VisOrder"] = 999999;
					continue;
				}
				if (!Function.ToString(list[num]["WhsFlt"]).Equals(value2))
				{
					value = "";
					text = "";
				}
				value2 = Function.ToString(list[num]["WhsFlt"]);
				list[num]["VisOrder"] = num + 1;
				string text2 = Function.ToString(list[num]["Sector"]);
				if (!string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(text) && !text.Substring(4).Equals(text2.Substring(0, 3)))
				{
					value = "";
				}
				text = Function.ToString(list[num]["Sector"]);
				if (string.IsNullOrEmpty(value) && (text2.StartsWith("HAN") || text2.StartsWith("SGN")))
				{
					value = Function.ToString(list[num]["DelTime"]);
				}
				if (!string.IsNullOrEmpty(value))
				{
					list[num]["DelTime"] = value;
				}
				if (text2.EndsWith("HAN") || text2.EndsWith("SGN"))
				{
					text = "";
					value = "";
				}
			}
			dt.AcceptChanges();
			int i;
			for (i = 0; i < dt.Rows.Count; i++)
			{
				List<DataRow> list2 = (from t in dataTable.AsEnumerable()
					where Function.ToString(t["FltNo"]).Equals(Function.ToString(dt.Rows[i]["FltNo"]))
					select t).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					list2[num2]["VisOrder"] = dt.Rows[i]["VisOrder"];
					list2[num2]["DelTime"] = dt.Rows[i]["DelTime"];
				}
			}
			dataTable.AcceptChanges();
			pvgDoc.DataSource = dataTable;
		}
		FormMode = Enums.FormViewModeType.Add;
	}

	private int FlightPairing(DataRow flt, List<DataRow> pairSorce, int maxSort)
	{
		int fltNo = Function.ParseInt(flt["FltNo"]);
		DataRow nextFlt = (from m in pairSorce.AsEnumerable().Where((DataRow t) =>
			{
				if (Function.ParseInt(t["FltNo"]) != fltNo + 1)
				{
					goto IL_0109;
				}
				if (!(Function.ParseDateTime(t["FltDate"]) > Function.ParseDateTime(flt["FltDate"])))
				{
					DateTime? dateTime3 = Function.ParseDateTime(t["FltDate"]);
					DateTime? dateTime4 = Function.ParseDateTime(flt["FltDate"]);
					if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["FltTime"]) <= Function.ParseInt(flt["EndTime"]))
					{
						goto IL_0109;
					}
				}
				goto IL_0428;
				IL_031b:
				if (Function.ParseInt(t["FltNo"]) == fltNo - 1)
				{
					if (!(Function.ParseDateTime(t["FltDate"]) < Function.ParseDateTime(flt["FltDate"])))
					{
						if (Function.ParseDateTime(t["FltDate"]) == Function.ParseDateTime(flt["FltDate"]))
						{
							return Function.ParseInt(t["EndTime"]) < Function.ParseInt(flt["FltTime"]);
						}
						return false;
					}
					return true;
				}
				return false;
				IL_0109:
				if (Function.ParseInt(t["FltNo"]) != fltNo - 1)
				{
					goto IL_0212;
				}
				if (!(Function.ParseDateTime(t["FltDate"]) > Function.ParseDateTime(flt["FltDate"])))
				{
					DateTime? dateTime3 = Function.ParseDateTime(t["FltDate"]);
					DateTime? dateTime4 = Function.ParseDateTime(flt["FltDate"]);
					if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["FltTime"]) <= Function.ParseInt(flt["EndTime"]))
					{
						goto IL_0212;
					}
				}
				goto IL_0428;
				IL_0428:
				return true;
				IL_0212:
				if (Function.ParseInt(t["FltNo"]) != fltNo + 1)
				{
					goto IL_031b;
				}
				if (!(Function.ParseDateTime(t["FltDate"]) < Function.ParseDateTime(flt["FltDate"])))
				{
					DateTime? dateTime3 = Function.ParseDateTime(t["FltDate"]);
					DateTime? dateTime4 = Function.ParseDateTime(flt["FltDate"]);
					if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["EndTime"]) >= Function.ParseInt(flt["FltTime"]))
					{
						goto IL_031b;
					}
				}
				goto IL_0428;
			})
			orderby Function.ParseDateTime(m["FltDate"]), Function.ParseInt(m["FltTime"])
			select m).FirstOrDefault();
		maxSort++;
		if (nextFlt != null)
		{
			if (Function.ToString(flt["FltType"]).Equals("QUOCNOI"))
			{
				int timeMinus = 0;
				if (Function.ParseInt(nextFlt["EndTime"]) < Function.ParseInt(flt["FltTime"]))
				{
					timeMinus = Function.ParseInt(flt["FltTime"]) - Function.ParseInt(nextFlt["EndTime"]);
				}
				else
				{
					timeMinus = Function.ParseInt(nextFlt["FltTime"]) - Function.ParseInt(flt["EndTime"]);
				}
				int fitNo = Function.ParseInt(nextFlt["FltNo"]);
				if (pairSorce.AsEnumerable().FirstOrDefault((DataRow t) =>
				{
					if (Function.ToString(flt["FltNo"]).Equals(Function.ToString(t["FltNo"])) || !Function.ToString(nextFlt["Sector"]).Substring(4).Equals(Function.ToString(t["Sector"]).Substring(0, 3)))
					{
						return false;
					}
					if (Function.ParseInt(t["FltNo"]) == fitNo + 1)
					{
						if (!(Function.ParseDateTime(t["FltDate"]) > Function.ParseDateTime(nextFlt["FltDate"])))
						{
							DateTime? dateTime3 = Function.ParseDateTime(t["FltDate"]);
							DateTime? dateTime4 = Function.ParseDateTime(nextFlt["FltDate"]);
							if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["FltTime"]) <= Function.ParseInt(nextFlt["EndTime"]))
							{
								if (Function.ParseInt(t["FltNo"]) != fitNo - 1)
								{
									goto IL_02d8;
								}
								if (!(Function.ParseDateTime(t["FltDate"]) > Function.ParseDateTime(nextFlt["FltDate"])))
								{
									dateTime3 = Function.ParseDateTime(t["FltDate"]);
									dateTime4 = Function.ParseDateTime(nextFlt["FltDate"]);
									if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["FltTime"]) <= Function.ParseInt(nextFlt["EndTime"]))
									{
										goto IL_02d8;
									}
								}
							}
						}
						if (timeMinus > Function.ParseInt(t["FltTime"]) - Function.ParseInt(nextFlt["EndTime"]))
						{
							return true;
						}
					}
					goto IL_02d8;
					IL_02d8:
					if (Function.ParseInt(t["FltNo"]) == fitNo + 1)
					{
						if (!(Function.ParseDateTime(t["FltDate"]) < Function.ParseDateTime(nextFlt["FltDate"])))
						{
							DateTime? dateTime3 = Function.ParseDateTime(t["FltDate"]);
							DateTime? dateTime4 = Function.ParseDateTime(nextFlt["FltDate"]);
							if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["EndTime"]) >= Function.ParseInt(nextFlt["FltTime"]))
							{
								if (Function.ParseInt(t["FltNo"]) != fitNo - 1)
								{
									goto IL_0539;
								}
								if (!(Function.ParseDateTime(t["FltDate"]) < Function.ParseDateTime(nextFlt["FltDate"])))
								{
									dateTime3 = Function.ParseDateTime(t["FltDate"]);
									dateTime4 = Function.ParseDateTime(nextFlt["FltDate"]);
									if (dateTime3.HasValue != dateTime4.HasValue || (dateTime3.HasValue && !(dateTime3.GetValueOrDefault() == dateTime4.GetValueOrDefault())) || Function.ParseInt(t["EndTime"]) >= Function.ParseInt(nextFlt["FltTime"]))
									{
										goto IL_0539;
									}
								}
							}
						}
						return timeMinus > Function.ParseInt(nextFlt["FltTime"]) - Function.ParseInt(t["EndTime"]);
					}
					goto IL_0539;
					IL_0539:
					return false;
				}) != null)
				{
					DataRow dataRow = null;
					if (!Function.ToString(flt["Sector"]).EndsWith("HAN") && !Function.ToString(flt["Sector"]).EndsWith("SGN"))
					{
						dataRow = (from m in pairSorce.AsEnumerable()
							where !Function.ToString(nextFlt["FltNo"]).Equals(Function.ToString(m["FltNo"])) && Function.ToString(m["Sector"]).Equals(string.Format("{0}-{1}", Function.ToString(flt["Sector"]).Substring(4), Function.ToString(flt["Sector"]).Substring(0, 3))) && (Function.ParseDateTime(m["FltDate"]) > Function.ParseDateTime(flt["FltDate"]) || (Function.ParseDateTime(m["FltDate"]) == Function.ParseDateTime(flt["FltDate"]) && Function.ParseInt(m["FltTime"]) > Function.ParseInt(flt["EndTime"]) && Function.ParseInt(m["FltTime"]) - Function.ParseInt(flt["EndTime"]) < 100))
							orderby Function.ParseDateTime(m["FltDate"]), Function.ParseInt(m["FltTime"])
							select m).FirstOrDefault();
					}
					flt["VisOrder"] = maxSort;
					pairSorce.Remove(flt);
					if (dataRow != null)
					{
						maxSort++;
						dataRow["VisOrder"] = maxSort;
						pairSorce.Remove(dataRow);
					}
					return maxSort;
				}
			}
			if (!(Function.ParseDateTime(nextFlt["FltDate"]) < Function.ParseDateTime(flt["FltDate"])))
			{
				DateTime? dateTime = Function.ParseDateTime(nextFlt["FltDate"]);
				DateTime? dateTime2 = Function.ParseDateTime(flt["FltDate"]);
				if (!(dateTime.HasValue & dateTime2.HasValue) || !(dateTime.GetValueOrDefault() < dateTime2.GetValueOrDefault()) || Function.ParseInt(nextFlt["FltTime"]) >= Function.ParseInt(flt["FltTime"]))
				{
					flt["VisOrder"] = maxSort;
					maxSort++;
					nextFlt["VisOrder"] = maxSort;
					goto IL_0422;
				}
			}
			nextFlt["VisOrder"] = maxSort;
			maxSort++;
			flt["VisOrder"] = maxSort;
			goto IL_0422;
		}
		DataRow dataRow2 = null;
		if (Function.ToString(flt["Sector"]).StartsWith("HAN") && Function.ToString(flt["Sector"]).EndsWith("KIX"))
		{
			dataRow2 = pairSorce.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Sector"]).StartsWith("KIX") && Function.ToString(t["Sector"]).EndsWith("DAD"));
		}
		else if (Function.ToString(flt["Sector"]).StartsWith("DAD") && Function.ToString(flt["Sector"]).EndsWith("KIX"))
		{
			dataRow2 = pairSorce.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Sector"]).StartsWith("KIX") && Function.ToString(t["Sector"]).EndsWith("HAN"));
		}
		else if (!Function.ToString(flt["Sector"]).EndsWith("HAN") && !Function.ToString(flt["Sector"]).EndsWith("SGN"))
		{
			dataRow2 = (from m in pairSorce.AsEnumerable()
				where Function.ToString(m["Sector"]).Equals(string.Format("{0}-{1}", Function.ToString(flt["Sector"]).Substring(4), Function.ToString(flt["Sector"]).Substring(0, 3))) && (Function.ParseDateTime(m["FltDate"]) > Function.ParseDateTime(flt["FltDate"]) || (Function.ParseDateTime(m["FltDate"]) == Function.ParseDateTime(flt["FltDate"]) && Function.ParseInt(m["FltTime"]) > Function.ParseInt(flt["EndTime"]) && Function.ParseInt(m["FltTime"]) - Function.ParseInt(flt["EndTime"]) < 100))
				orderby Function.ParseDateTime(m["FltDate"]), Function.ParseInt(m["FltTime"])
				select m).FirstOrDefault();
		}
		flt["VisOrder"] = maxSort;
		pairSorce.Remove(flt);
		if (dataRow2 != null)
		{
			maxSort++;
			dataRow2["VisOrder"] = maxSort;
			pairSorce.Remove(dataRow2);
		}
		goto IL_061c;
		IL_061c:
		if (Function.ToString(flt["Sector"]).Equals("HAN-KIX"))
		{
			DataRow dataRow3 = pairSorce.FirstOrDefault((DataRow t) => Function.ToString(t["Sector"]).Equals("DAD-KIX"));
			if (dataRow3 != null)
			{
				maxSort = FlightPairing(dataRow3, pairSorce, maxSort);
			}
		}
		return maxSort;
		IL_0422:
		pairSorce.Remove(flt);
		pairSorce.Remove(nextFlt);
		goto IL_061c;
	}

	private async Task SaveMrp()
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return;
		}
		DataTable dataTable = pvgDoc.DataSourceTable();
		if (!dataTable.IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.MrpDataIsEmpty, "");
			return;
		}
		if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Math.Abs(Function.ParseDouble(t["Quantity"])) > 1E-07) == null)
		{
			WinCommons.Error(Enums.MessageCode.NoQuantityData, "");
			return;
		}
		string action = ((Function.ParseInt(dataTable.Rows[0]["Code"]) == 0) ? "SaveData" : "UpdateData");
		using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JetstarPreOrder"))
		{
			HttpResult httpResult = await http.PostAsync(action, dataTable);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
		}
		WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
		btnSearch_Click(null, null);
		FormMode = Enums.FormViewModeType.View;
	}

	private void ChangeCellValue(EditValueChangedEventArgs e)
	{
		try
		{
			PivotDrillDownDataSource requireSource = e.CreateDrillDownDataSource();
			int columnCount = pvgDoc.Cells.ColumnCount;
			if (e.ColumnIndex < columnCount - 1 && !(e.Editor is CalcEdit))
			{
				return;
			}
			for (int i = 0; i < columnCount; i++)
			{
				int k;
				for (k = 0; k < requireSource.RowCount; k++)
				{
					if (requireSource[k] == null || e.ColumnIndex != i)
					{
						continue;
					}
					if (Function.ToString(requireSource[k][pgcSsrCode]).Equals("zzz03"))
					{
						requireSource[k]["Comments"] = Function.ToString(e.Editor.EditValue);
						pvgDoc.DataSourceTable().AcceptChanges();
						pvgDoc.RefreshData();
						return;
					}
					double num = Function.ParseDouble(requireSource[k][pgcQty]);
					double num2 = Function.ParseDouble(e.Editor.EditValue);
					requireSource[k][pgcQty] = num2;
					pvgDoc.DataSourceTable().AcceptChanges();
					DataRow dataRow = pvgDoc.DataSourceTable().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t[pgcFliNo.FieldName]).Equals(Function.ToString(requireSource[k][pgcFliNo])) && Function.ParseInt(t[pgcFltTime.FieldName]) == Function.ParseInt(requireSource[k][pgcFltTime]) && Function.ToString(t[pgcOrderType.FieldName]).Equals(Function.ToString(requireSource[k][pgcOrderType])) && Function.ToString(t[pgcSsrCode.FieldName]).Equals("zzz00"));
					if (dataRow != null)
					{
						dataRow[pgcQty.FieldName] = Function.ParseDouble(dataRow[pgcQty.FieldName]) + num2 - num;
					}
					pvgDoc.DataSourceTable().AcceptChanges();
					pvgDoc.RefreshData();
					pvgDoc.ShowEditor();
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveOrder()
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCreateDocument, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return;
		}
		DataTable dataTable = pvgDoc.DataSourceTable();
		if (!dataTable.IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.MrpDataIsEmpty, "");
			return;
		}
		List<DataRow> lstSave = (from t in dataTable.AsEnumerable()
			where Math.Abs(Function.ParseDouble(t["Quantity"])) > 1E-07 && !Function.ToString(t["SsrName"]).StartsWith("zzz")
			select t).ToList();
		if (lstSave.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.NoQuantityDataToCreatePurchaseOrder, "");
			return;
		}
		WinCommons._OpenProcessing();
		if (Function.ParseInt(lstSave[0]["Code"]) == 0)
		{
			WinCommons.Error(Enums.MessageCode.CannotGetIdToGeneratePo, "");
			return;
		}
		HttpResult resp;
		using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JetstarPreOrder"))
		{
			resp = await http.PostAsync("CancelOldPurchaseOrder", Function.ParseInt(lstSave[0]["Code"]));
			if (!string.IsNullOrEmpty(resp.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, resp.Message);
				return;
			}
		}
		DataTable dtSave = new DataTable();
		dtSave.Columns.Add("CardCode", typeof(string));
		dtSave.Columns.Add("CardName", typeof(string));
		dtSave.Columns.Add("ItemCode", typeof(string));
		dtSave.Columns.Add("ItemName", typeof(string));
		dtSave.Columns.Add("WhsCode", typeof(string));
		dtSave.Columns.Add("FltNo", typeof(string));
		dtSave.Columns.Add("FltDate", typeof(DateTime));
		dtSave.Columns.Add("FltTime", typeof(int));
		dtSave.Columns.Add("DelTime", typeof(string));
		dtSave.Columns.Add("FltType", typeof(string));
		dtSave.Columns.Add("Price", typeof(double));
		dtSave.Columns.Add("Currency", typeof(string));
		dtSave.Columns.Add("Quantity", typeof(double));
		dtSave.Columns.Add("Code", typeof(int));
		List<DataRow> dtGroup = (from r in lstSave
			group r by new
			{
				CardCode = r["CardCode"],
				CardName = r["CardName"],
				ItemCode = r["ItemCode"],
				ItemName = r["ItemName"],
				WhsCode = r["WhsCode"],
				FltNo = r["FltNo"],
				FltDate = r["FltDate"],
				FltTime = r["FltTime"],
				DelTime = r["DelTime"],
				FltType = r["FltType"],
				Price = r["Price"],
				Currency = r["Currency"],
				Code = r["Code"]
			}).Select(g =>
		{
			DataRow dataRow = dtSave.NewRow();
			dataRow["CardCode"] = g.Key.CardCode;
			dataRow["CardName"] = g.Key.CardName;
			dataRow["ItemCode"] = g.Key.ItemCode;
			dataRow["ItemName"] = g.Key.ItemName;
			dataRow["WhsCode"] = g.Key.WhsCode;
			dataRow["FltNo"] = g.Key.FltNo;
			dataRow["FltDate"] = g.Key.FltDate;
			dataRow["FltTime"] = g.Key.FltTime;
			dataRow["DelTime"] = g.Key.DelTime;
			dataRow["FltType"] = g.Key.FltType;
			dataRow["Price"] = g.Key.Price;
			dataRow["Currency"] = g.Key.Currency;
			dataRow["Code"] = g.Key.Code;
			dataRow["Quantity"] = g.Sum((DataRow y) => Function.ParseDouble(y["Quantity"]));
			return dataRow;
		}).ToList();
		string mess = "";
		DocumentsImpl doc = new DocumentsImpl();
		DateTime serverDate = await HttpClientHelperFactory.GetHttpClient().Commons.GetServerDate();
		int i;
		for (i = 0; i < dtGroup.Count; i++)
		{
			string value = (doc.CardCode = Function.ToString(dtGroup[i]["CardCode"]));
			doc.CardName = Function.ToString(dtGroup[i]["CardName"]);
			doc.BPLId = -1;
			doc.DocStatus = "O";
			doc.DocDate = serverDate;
			doc.TaxDate = serverDate;
			doc.Lines = new List<Document_LinesImpl>();
			doc.DocRate = 1.0;
			doc.OriginType = "R";
			doc.UserFields = new List<UserFieldsImpl>();
			doc.UserFields.Add(new UserFieldsImpl
			{
				Name = "U_Version",
				Type = BoFieldTypes.db_Numeric,
				Value = resp.Content
			});
			doc.UserFields.Add(new UserFieldsImpl
			{
				Name = "U_MrpDate",
				Type = BoFieldTypes.db_Date,
				Value = lstSave[0]["MrpDate"]
			});
			DateTime dateTime = Function.ParseDateTimes(dtGroup[i]["FltDate"]);
			int num = 0;
			for (int num2 = i; num2 < dtGroup.Count; num2++)
			{
				if (Function.ToString(dtGroup[num2]["CardCode"]).Equals(value))
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl
					{
						ItemCode = Function.ToString(dtGroup[num2]["ItemCode"]),
						Dscription = Function.ToString(dtGroup[num2]["ItemName"]),
						Quantity = Function.ParseDouble(dtGroup[num2]["Quantity"]),
						Price = Function.ParseDouble(dtGroup[num2]["Price"]),
						Rate = 1.0,
						LineNum = num,
						Currency = Function.ToString(dtGroup[num2]["Currency"]),
						WhsCode = Function.ToString(dtGroup[num2]["WhsCode"]),
						ShipDate = Function.ParseDateTimes(dtGroup[num2]["FltDate"])
					};
					if (string.IsNullOrEmpty(document_LinesImpl.Currency))
					{
						document_LinesImpl.Currency = GlobalData.MainCurrency;
					}
					num++;
					document_LinesImpl.UserFields = new List<UserFieldsImpl>();
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_FlightCode",
						Type = BoFieldTypes.db_Alpha,
						Value = dtGroup[num2]["FltNo"]
					});
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_FightDate",
						Type = BoFieldTypes.db_Date,
						Value = dtGroup[num2]["FltDate"]
					});
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_FlightTime",
						Type = BoFieldTypes.db_Numeric,
						Value = dtGroup[num2]["FltTime"]
					});
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_DelTime",
						Type = BoFieldTypes.db_Alpha,
						Value = dtGroup[num2]["DelTime"]
					});
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_OriginType",
						Type = BoFieldTypes.db_Alpha,
						Value = "JetstarMrp"
					});
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_OriginCode",
						Type = BoFieldTypes.db_Alpha,
						Value = dtGroup[num2]["Code"]
					});
					document_LinesImpl.UserFields.Add(new UserFieldsImpl
					{
						Name = "U_FlightType",
						Type = BoFieldTypes.db_Alpha,
						Value = dtGroup[num2]["FltType"]
					});
					if (document_LinesImpl.ShipDate < dateTime)
					{
						dateTime = document_LinesImpl.ShipDate;
					}
					doc.Lines.Add(document_LinesImpl);
					dtGroup.RemoveAt(num2);
					num2--;
				}
			}
			i--;
			doc.DocDueDate = dateTime;
			if (!(await doc.GetApprovalTemplate(new HttpClientHelper<DataTable>(Configuration.ApiServer, "PurchaseOder"))))
			{
				WinCommons.Error(Enums.MessageCode.CannotGetApprovalTemplate, "");
			}
			else
			{
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PurchaseOder");
				HttpResult httpResult = await http.PostAsync("SaveDoc", doc);
				if (httpResult.MessageCode != Enums.MessageCode.Success)
				{
					mess = mess + " " + httpResult.Message;
				}
			}
		}
		if (!string.IsNullOrWhiteSpace(mess))
		{
			WinCommons.Error(Enums.MessageCode.None, mess);
		}
		else
		{
			WinCommons.Success(Enums.MessageCode.CreatePurchaseOrderSuccess, "");
		}
		btnSearch_Click(null, null);
	}

	private async Task<DataRowView> Cfl_Vendor()
	{
		DataTable dataJetstarVendorPreview;
		using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "JetstarPreOrder"))
		{
			dataJetstarVendorPreview = await http.GetAsync("GetBp");
		}
		frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.JetstarPreviewBp, multiSelect: false, "");
		frm_ChooseFromList2.DataJetstarVendorPreview = dataJetstarVendorPreview;
		frm_ChooseFromList2.ShowDialog(this);
		return frm_ChooseFromList2.ChooseItem;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_JetstarPreOrder));
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnRun = new DevExpress.XtraEditors.SimpleButton();
		this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
		this.lbSearchDate = new Apzon.Controls.Controls.LabelLine();
		this.txtSearchDate = new Apzon.Controls.Editor.DateEditEx();
		this.pvgDoc = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.pgcSector = new DevExpress.XtraPivotGrid.PivotGridField();
		this.gcWhs = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcOrderType = new DevExpress.XtraPivotGrid.PivotGridField();
		this.rOrderType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.pgcFliNo = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcFltType = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcDeliTime = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcFltTime = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcTotalPax = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcSsrCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pgcQty = new DevExpress.XtraPivotGrid.PivotGridField();
		this.rQuantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.pgcVisOder = new DevExpress.XtraPivotGrid.PivotGridField();
		this.lbMrpType = new Apzon.Controls.Controls.LabelLine();
		this.txtType = new Apzon.Controls.Editor.LookUpEditEx();
		this.btnPreview = new DevExpress.XtraEditors.SimpleButton();
		this.btnGenPurchase = new DevExpress.XtraEditors.SimpleButton();
		this.btnManualSort = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.txtSearchDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSearchDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pvgDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOrderType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(935, 455);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnRun.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnRun.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRun.ImageOptions.Image");
		this.btnRun.Location = new System.Drawing.Point(853, 455);
		this.btnRun.Name = "btnRun";
		this.btnRun.Size = new System.Drawing.Size(75, 23);
		this.btnRun.TabIndex = 3;
		this.btnRun.Text = "Run";
		this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.Location = new System.Drawing.Point(470, 7);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(70, 20);
		this.btnSearch.TabIndex = 2;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.lbSearchDate.FieldName = null;
		this.lbSearchDate.HideWhenEmpty = false;
		this.lbSearchDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSearchDate.Location = new System.Drawing.Point(231, 10);
		this.lbSearchDate.Name = "lbSearchDate";
		this.lbSearchDate.ShowUnderLine = true;
		this.lbSearchDate.Size = new System.Drawing.Size(80, 20);
		this.lbSearchDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSearchDate.TabIndex = 0;
		this.lbSearchDate.Utext = "Mrp Date";
		this.txtSearchDate.EditValue = null;
		this.txtSearchDate.FieldName = null;
		this.txtSearchDate.Location = new System.Drawing.Point(313, 7);
		this.txtSearchDate.Name = "txtSearchDate";
		this.txtSearchDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSearchDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtSearchDate.Properties.IsSearchControl = false;
		this.txtSearchDate.Size = new System.Drawing.Size(150, 20);
		this.txtSearchDate.TabIndex = 1;
		this.pvgDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pvgDoc.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[11]
		{
			this.pgcSector, this.gcWhs, this.pgcOrderType, this.pgcFliNo, this.pgcFltType, this.pgcDeliTime, this.pgcFltTime, this.pgcTotalPax, this.pgcSsrCode, this.pgcQty,
			this.pgcVisOder
		});
		this.pvgDoc.FilterFieldControl = null;
		this.pvgDoc.FilterValueControl = null;
		this.pvgDoc.FormId = "oJetstarPlan";
		this.pvgDoc.ItemId = "APZ_OJPO";
		this.pvgDoc.Location = new System.Drawing.Point(1, 33);
		this.pvgDoc.Name = "pvgDoc";
		this.pvgDoc.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.pvgDoc.OptionsData.DataProcessingEngine = DevExpress.XtraPivotGrid.PivotDataProcessingEngine.LegacyOptimized;
		this.pvgDoc.OptionsView.ShowButtonMode = DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowAlways;
		this.pvgDoc.OptionsView.ShowFilterHeaders = false;
		this.pvgDoc.OptionsView.ShowRowGrandTotals = false;
		this.pvgDoc.OptionsView.ShowTotalsForSingleValues = true;
		this.pvgDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rQuantity, this.rOrderType });
		this.pvgDoc.Size = new System.Drawing.Size(1016, 419);
		this.pvgDoc.TabIndex = 0;
		this.pvgDoc.CustomFieldSort += new DevExpress.XtraPivotGrid.PivotGridCustomFieldSortEventHandler(this.pvgDoc_CustomFieldSort);
		this.pvgDoc.FieldValueDisplayText += new DevExpress.XtraPivotGrid.PivotFieldDisplayTextEventHandler(this.pvgDoc_FieldValueDisplayText);
		this.pvgDoc.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.pvgDoc_CustomCellDisplayText);
		this.pvgDoc.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pvgDoc_CustomAppearance);
		this.pvgDoc.CustomColumnWidth += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCustomColumnWidthEventArgs>(this.pvgDoc_CustomColumnWidth);
		this.pvgDoc.EditValueChanged += new DevExpress.XtraPivotGrid.EditValueChangedEventHandler(this.pvgDoc_EditValueChanged);
		this.pvgDoc.ShowingEditor += new System.EventHandler<DevExpress.XtraPivotGrid.CancelPivotCellEditEventArgs>(this.pvgDoc_ShowingEditor);
		this.pvgDoc.CustomCellEditForEditing += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCustomCellEditEventArgs>(this.pvgDoc_CustomCellEditForEditing);
		this.pgcSector.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcSector.AreaIndex = 2;
		this.pgcSector.Caption = "Sector";
		this.pgcSector.FieldName = "Sector";
		this.pgcSector.Name = "pgcSector";
		this.pgcSector.Options.ShowCustomTotals = false;
		this.pgcSector.Options.ShowGrandTotal = false;
		this.pgcSector.Options.ShowTotals = false;
		this.pgcSector.Width = 70;
		this.gcWhs.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.gcWhs.AreaIndex = 1;
		this.gcWhs.Caption = "Tàu bay";
		this.gcWhs.FieldName = "WhsFlt";
		this.gcWhs.Name = "gcWhs";
		this.gcWhs.Options.ShowCustomTotals = false;
		this.gcWhs.Options.ShowGrandTotal = false;
		this.gcWhs.Options.ShowTotals = false;
		this.gcWhs.Width = 80;
		this.pgcOrderType.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcOrderType.AreaIndex = 5;
		this.pgcOrderType.Caption = "Order";
		this.pgcOrderType.FieldEdit = this.rOrderType;
		this.pgcOrderType.FieldName = "OrderName";
		this.pgcOrderType.Name = "pgcOrderType";
		this.pgcOrderType.Options.ShowCustomTotals = false;
		this.pgcOrderType.Options.ShowGrandTotal = false;
		this.pgcOrderType.Options.ShowTotals = false;
		this.pgcOrderType.Width = 85;
		this.rOrderType.AutoHeight = false;
		this.rOrderType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOrderType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.rOrderType.DisplayMember = "Name";
		this.rOrderType.Name = "rOrderType";
		this.rOrderType.NullText = "";
		this.rOrderType.ReadOnly = true;
		this.rOrderType.ShowHeader = false;
		this.rOrderType.ValueMember = "Code";
		this.pgcFliNo.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcFliNo.AreaIndex = 0;
		this.pgcFliNo.Caption = "Flt No.";
		this.pgcFliNo.FieldName = "FltNo";
		this.pgcFliNo.Name = "pgcFliNo";
		this.pgcFliNo.Options.ShowCustomTotals = false;
		this.pgcFliNo.Options.ShowGrandTotal = false;
		this.pgcFliNo.Width = 73;
		this.pgcFltType.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcFltType.AreaIndex = 3;
		this.pgcFltType.Caption = "Flight Type";
		this.pgcFltType.FieldName = "FltType";
		this.pgcFltType.Name = "pgcFltType";
		this.pgcFltType.Visible = false;
		this.pgcDeliTime.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcDeliTime.AreaIndex = 6;
		this.pgcDeliTime.Caption = "Lần";
		this.pgcDeliTime.FieldName = "DelTime";
		this.pgcDeliTime.Name = "pgcDeliTime";
		this.pgcDeliTime.Options.ShowCustomTotals = false;
		this.pgcDeliTime.Options.ShowGrandTotal = false;
		this.pgcDeliTime.Options.ShowTotals = false;
		this.pgcDeliTime.Width = 65;
		this.pgcFltTime.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcFltTime.AreaIndex = 3;
		this.pgcFltTime.Caption = "Time";
		this.pgcFltTime.FieldName = "FltTime";
		this.pgcFltTime.Name = "pgcFltTime";
		this.pgcFltTime.Options.ShowCustomTotals = false;
		this.pgcFltTime.Options.ShowGrandTotal = false;
		this.pgcFltTime.Options.ShowTotals = false;
		this.pgcFltTime.UnboundFieldName = "pgcFltTime";
		this.pgcFltTime.Width = 60;
		this.pgcTotalPax.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcTotalPax.AreaIndex = 4;
		this.pgcTotalPax.Caption = "T. pax";
		this.pgcTotalPax.FieldName = "TotalPax";
		this.pgcTotalPax.Name = "pgcTotalPax";
		this.pgcTotalPax.Options.ShowCustomTotals = false;
		this.pgcTotalPax.Options.ShowGrandTotal = false;
		this.pgcTotalPax.Options.ShowTotals = false;
		this.pgcTotalPax.Width = 70;
		this.pgcSsrCode.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.pgcSsrCode.AreaIndex = 0;
		this.pgcSsrCode.Caption = "SsrCode";
		this.pgcSsrCode.FieldName = "SsrName";
		this.pgcSsrCode.Name = "pgcSsrCode";
		this.pgcSsrCode.Options.ShowGrandTotal = false;
		this.pgcSsrCode.Options.ShowTotals = false;
		this.pgcSsrCode.SortMode = DevExpress.XtraPivotGrid.PivotSortMode.Custom;
		this.pgcSsrCode.Width = 38;
		this.pgcQty.Appearance.CellGrandTotal.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.pgcQty.Appearance.CellGrandTotal.Options.UseBackColor = true;
		this.pgcQty.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.pgcQty.AreaIndex = 0;
		this.pgcQty.Caption = "Quantity";
		this.pgcQty.FieldEdit = this.rQuantity;
		this.pgcQty.FieldName = "Quantity";
		this.pgcQty.Name = "pgcQty";
		this.pgcQty.Options.ShowButtonMode = DevExpress.XtraPivotGrid.PivotShowButtonModeEnum.ShowOnlyInEditor;
		this.pgcQty.Options.ShowGrandTotal = false;
		this.rQuantity.AutoHeight = false;
		this.rQuantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rQuantity.Name = "rQuantity";
		this.pgcVisOder.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pgcVisOder.AreaIndex = 6;
		this.pgcVisOder.FieldName = "VisOrder";
		this.pgcVisOder.Name = "pgcVisOder";
		this.pgcVisOder.Visible = false;
		this.lbMrpType.FieldName = null;
		this.lbMrpType.HideWhenEmpty = false;
		this.lbMrpType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbMrpType.Location = new System.Drawing.Point(15, 10);
		this.lbMrpType.Name = "lbMrpType";
		this.lbMrpType.ShowUnderLine = true;
		this.lbMrpType.Size = new System.Drawing.Size(80, 20);
		this.lbMrpType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbMrpType.TabIndex = 6;
		this.lbMrpType.Utext = "Type";
		this.txtType.FieldName = null;
		this.txtType.IsSearchControl = false;
		this.txtType.Location = new System.Drawing.Point(97, 7);
		this.txtType.Name = "txtType";
		this.txtType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.txtType.Properties.DisplayMember = "Name";
		this.txtType.Properties.NullText = "";
		this.txtType.Properties.ShowHeader = false;
		this.txtType.Properties.ValueMember = "Code";
		this.txtType.Size = new System.Drawing.Size(125, 20);
		this.txtType.TabIndex = 7;
		this.btnPreview.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnPreview.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnPreview.ImageOptions.Image");
		this.btnPreview.Location = new System.Drawing.Point(208, 455);
		this.btnPreview.Name = "btnPreview";
		this.btnPreview.Size = new System.Drawing.Size(75, 23);
		this.btnPreview.TabIndex = 8;
		this.btnPreview.Text = "Preview";
		this.btnPreview.Click += new System.EventHandler(this.btnPreview_Click);
		this.btnGenPurchase.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnGenPurchase.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnGenPurchase.ImageOptions.Image");
		this.btnGenPurchase.Location = new System.Drawing.Point(101, 455);
		this.btnGenPurchase.Name = "btnGenPurchase";
		this.btnGenPurchase.Size = new System.Drawing.Size(101, 23);
		this.btnGenPurchase.TabIndex = 9;
		this.btnGenPurchase.Text = "Purchase Order";
		this.btnGenPurchase.Click += new System.EventHandler(this.btnGenPurchase_Click);
		this.btnManualSort.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnManualSort.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnManualSort.ImageOptions.Image");
		this.btnManualSort.Location = new System.Drawing.Point(2, 455);
		this.btnManualSort.Name = "btnManualSort";
		this.btnManualSort.Size = new System.Drawing.Size(93, 23);
		this.btnManualSort.TabIndex = 10;
		this.btnManualSort.Text = "Manual Sort";
		this.btnManualSort.Click += new System.EventHandler(this.btnManualSort_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1018, 482);
		base.Controls.Add(this.btnManualSort);
		base.Controls.Add(this.btnGenPurchase);
		base.Controls.Add(this.btnPreview);
		base.Controls.Add(this.txtType);
		base.Controls.Add(this.lbMrpType);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnRun);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.lbSearchDate);
		base.Controls.Add(this.txtSearchDate);
		base.Controls.Add(this.pvgDoc);
		base.FormId = "oJetstarPlan";
		base.Name = "frm_JetstarPreOrder";
		base.ShowIcon = false;
		this.Text = "DAILY PURCHASE PLAN";
		base.Load += new System.EventHandler(this.frm_JetstarPreOrder_Load);
		((System.ComponentModel.ISupportInitialize)this.txtSearchDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSearchDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pvgDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOrderType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
