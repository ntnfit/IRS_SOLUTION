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
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.Mrp;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.Sales;

public class frm_ScheduleForAtp : VirSchedule
{
	public frm_ScheduleForAtp()
	{
		InitComponent();
	}

	public frm_ScheduleForAtp(int psCode)
		: base(psCode)
	{
		InitComponent();
	}

	private void InitComponent()
	{
		Text = "Schedule For Atp";
		EnableDisableControl(isEnable: true);
		lbInputCode.Utext = "Atp Code";
		lbfixdate.Visible = false;
		txtFixDate.Visible = false;
		lbtofstDate.Utext = "To Date";
		field_BP.FieldName = "ItemName";
		field_BP.Caption = "Desctiption";
		field_Version.Caption = "Item Group";
		field_Version.FieldName = "ItemGroup";
		field_WorkDate.FieldName = "DueDate";
		field_Shift.Visible = false;
		chkDetailSchedule.Visible = false;
	}

	protected override int GetNewIdToAddNew()
	{
		try
		{
			HttpResult newIdScheduleForAtp = HttpClient.AtpClient.GetNewIdScheduleForAtp();
			if (string.IsNullOrEmpty(newIdScheduleForAtp.Message))
			{
				return Function.ParseInt(newIdScheduleForAtp.Content);
			}
			WinCommons.Error(Enums.MessageCode.None, newIdScheduleForAtp.Message);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	protected override async Task GetDataInput()
	{
		Enums.FormViewModeType oldView = FormMode;
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataSet dataSet = await HttpClient.AtpClient.GetDataAtpForSchedle(Function.ParseInt(txtInputCode.EditValue));
			if (dataSet == null || dataSet.Tables.Count == 0 || !dataSet.Tables[0].IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.CanNotGetDataAtp, "");
				FormMode = oldView;
				return;
			}
			if (dataSet.Tables.Count == 1)
			{
				WinCommons.Error(Enums.MessageCode.None, $"{Function.ToString(dataSet.Tables[0].Rows[0][0])} - {Function.ToString(dataSet.Tables[0].Rows[0][1])}");
				FormMode = oldView;
				return;
			}
			if (dataSet.Tables.Count < 2)
			{
				WinCommons.Error(Enums.MessageCode.CanNotGetDataAtp, "");
				FormMode = oldView;
				return;
			}
			DataTable dataTable = dataSet.Tables[1];
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (!Function.ToString(dataTable.Rows[0]["Status"]).Equals("A"))
				{
					WinCommons.Error(Enums.MessageCode.YouMustChooseApprovalAtp, "");
					FormMode = oldView;
					WinCommons._CloseProcessing();
				}
				txtFromDate.EditValue = Function.ParseDateTimes(dataTable.Rows[0]["FromDate"]);
				txtToDate.EditValue = Function.ParseDateTimes(dataTable.Rows[0]["ToDate"]);
				await Run();
				oldView = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
	}

	protected override async Task Run()
	{
		try
		{
			DataTable dataSource = JoinDataLine(await HttpClient.AtpClient.RunSchedule(Function.ToString(txtInputCode.EditValue), Function.ToString(txtType.EditValue)));
			pgcDetail.DataSource = dataSource;
			pgcDetail.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void SetDataType()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Purchase",
					Code = "P"
				}
			};
			txtType.Properties.DataSource = dataSource;
			txtType.EditValue = "P";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void InputControllButtonClick(object sender, ButtonPressedEventArgs e)
	{
		if (e.Button.Index == 0)
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListAtp, false, Function.ToString(txtInputCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				AtpType = "I",
				AtpStatus = "A",
				OutputAtpType = "S"
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtInputCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
			}
		}
		else
		{
			int num = Function.ParseInt(txtInputCode.EditValue);
			if (num != 0)
			{
				this.DrillDown(ApzObjectTypes.oAtp, ApzObjectTypes.oAtp, Function.ToString(num));
			}
		}
	}

	protected override async Task GetDataDoc(Enums.NavigateType type)
	{
		Enums.FormViewModeType oldView = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataTable dataTable = await HttpClient.AtpClient.GetDataDocSchedule(Function.ParseInt(txtCode.EditValue), type);
			if (dataTable.IsNotNull())
			{
				SetDataDoc(dataTable.Rows[0]);
				SetDataStatus(Function.ToString(txtStatus.EditValue));
				await LoadDataLine();
				oldView = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
	}

	protected override async Task LoadDataLine()
	{
		try
		{
			DataTable dataSource = JoinDataLine(await GetDataLineAtpSchedule(Function.ParseInt(txtCode.EditValue)));
			pgcDetail.DataSource = dataSource;
			pgcDetail.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataSet> GetDataLineAtpSchedule(int code)
	{
		try
		{
			return await HttpClient.AtpClient.GetDataLineSchedule(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	protected override void SetDataDoc(DataRow row)
	{
		try
		{
			_isLoad = true;
			if (row != null)
			{
				txtType.EditValue = Function.ToString(row["Type"]);
				txtCode.EditValue = Function.ParseInt(row["Code"]);
				txtName.EditValue = Function.ToString(row["Name"]);
				txtInputCode.EditValue = Function.ToString(row["AtpCode"]);
				txtStatus.EditValue = Function.ToString(row["Status"]);
				txtFromDate.EditValue = Function.ParseDateTime(row["FromDate"]);
				txtToDate.EditValue = Function.ParseDateTime(row["ToDate"]);
			}
			else
			{
				if (FormMode != Enums.FormViewModeType.Add)
				{
					txtName.EditValue = null;
				}
				txtCode.EditValue = null;
				txtInputCode.EditValue = null;
				txtFromDate.EditValue = null;
				txtToDate.EditValue = null;
				txtFixDate.EditValue = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
	}

	protected override Enums.MessageCode GetDataDoc(ref DataTable dt)
	{
		if (DocStructure == null || DocStructure.Columns.Count == 0)
		{
			return Enums.MessageCode.CanNotGetHeaderStruc2Save;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtInputCode.EditValue)))
		{
			txtInputCode.Focus();
			return Enums.MessageCode.MrpCodeNotNull;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtFromDate.EditValue)))
		{
			txtFromDate.Focus();
			return Enums.MessageCode.FromDateCanNotBeNull;
		}
		if (string.IsNullOrEmpty(Function.ToString(txtToDate.EditValue)))
		{
			txtToDate.Focus();
			return Enums.MessageCode.ToDateCanNotBeNull;
		}
		dt = DocStructure.Clone();
		DataRow dataRow = dt.NewRow();
		dataRow["Code"] = Function.ParseInt(txtCode.EditValue);
		dataRow["Name"] = txtName.EditValue;
		dataRow["AtpCode"] = txtInputCode.EditValue;
		dataRow["Status"] = txtStatus.EditValue;
		dataRow["Type"] = txtType.EditValue;
		dataRow["CreateDate"] = DateTime.Now.Date;
		dt.Rows.Add(dataRow);
		return Enums.MessageCode.Success;
	}

	protected override async Task GetDocScheduleStructure()
	{
		try
		{
			DocStructure = await HttpClient.AtpClient.GetDocScheduleStructure();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			DocStructure = new DataTable();
		}
	}

	protected override async Task Save()
	{
		Enums.FormViewModeType oldView = FormMode;
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			DataTable dt = new DataTable();
			Enums.MessageCode dataDoc = GetDataDoc(ref dt);
			if (dataDoc != Enums.MessageCode.Success)
			{
				WinCommons.Error(dataDoc, "");
				return;
			}
			DataTable dataTable = pgcDetail.DataSourceTable().Copy();
			if (!dt.IsNotNull() || !dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
			}
			dt.TableName = "Table doc";
			dataTable.TableName = "Table Line";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dt);
			dataSet.Tables.Add(dataTable);
			HttpResult httpResult = ((oldView != Enums.FormViewModeType.Add) ? (await HttpClient.AtpClient.UpdateSchedule(dataSet)) : (await HttpClient.AtpClient.SaveSchedule(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (string.IsNullOrEmpty(httpResult2.Message))
			{
				txtCode.EditValue = Function.ParseInt(httpResult2.Content);
				WinCommons.Success((oldView == Enums.FormViewModeType.Add) ? Enums.MessageCode.SaveSuccess : Enums.MessageCode.UpdateSuccess, "");
				SetDataStatus(Function.ToString(txtStatus.EditValue));
				oldView = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
			}
		}
		FormMode = oldView;
	}

	protected override DataTable JoinDataLine(DataSet line)
	{
		try
		{
			if (line == null || line.Tables.Count < 3 || !line.Tables[2].IsNotNull() || !line.Tables[0].IsNotNull() || !line.Tables[1].IsNotNull())
			{
				WinCommons.Warning(Enums.MessageCode.DataLinesIsEmpty, "");
				return null;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("DueDate", typeof(DateTime));
			DateTime dateTime = Function.ParseDateTimes(line.Tables[0].Rows[0][0]).Date;
			DateTime date = Function.ParseDateTimes(line.Tables[0].Rows[0][1]).Date;
			while (dateTime <= date)
			{
				dataTable.NewRow()[0] = dateTime;
				dataTable.Rows.Add(dateTime);
				dateTime = dateTime.AddDays(1.0);
			}
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DateDataIsNull, "");
				return null;
			}
			DataTable dtPlaning = new DataTable();
			dtPlaning.Columns.Add("DueDate", typeof(string));
			dtPlaning.Columns.Add("Planning", typeof(string));
			DataTable source = (from distinct in line.Tables[1].AsEnumerable()
				join dataRow in dataTable.AsEnumerable() on 1 equals 1 into temps
				from temp in temps.DefaultIfEmpty()
				select dtPlaning.LoadDataRow(new object[2]
				{
					temp["DueDate"],
					distinct["Planning"]
				}, fAcceptChanges: true)).CopyToDataTable();
			DataTable dtresult = new DataTable();
			dtresult.Columns.Add("ItemCode", typeof(string));
			dtresult.Columns.Add("Planning", typeof(string));
			dtresult.Columns.Add("DueDate", typeof(DateTime));
			dtresult.Columns.Add("Qty", typeof(double));
			dtresult.Columns.Add("DocEntry", typeof(int));
			dtresult.Columns.Add("ItemName", typeof(string));
			dtresult.Columns.Add("ItemGroup", typeof(string));
			return (from tem in (from distinct in line.Tables[2].DefaultView.ToTable(true, "ItemCode", "DueDate", "ItemName", "ItemGroup").AsEnumerable()
					join dataRow in source.AsEnumerable() on new
					{
						WorkDate = Function.ParseDateTimes(distinct["DueDate"]).Date
					} equals new
					{
						WorkDate = Function.ParseDateTimes(dataRow["DueDate"]).Date
					} into temps
					from temp in temps.DefaultIfEmpty()
					select dtresult.LoadDataRow(new object[7]
					{
						distinct["ItemCode"],
						temp["Planning"],
						temp["DueDate"],
						0,
						0,
						distinct["ItemName"],
						distinct["ItemGroup"]
					}, fAcceptChanges: true)).CopyToDataTable().AsEnumerable()
				join row in line.Tables[2].AsEnumerable() on new
				{
					ItemCode = Function.ToString(tem["ItemCode"]),
					TypePlanning = Function.ToString(tem["Planning"]),
					WorkDate = Function.ParseDateTimes(tem["DueDate"]).Date
				} equals new
				{
					ItemCode = Function.ToString(row["ItemCode"]),
					TypePlanning = Function.ToString(row["Planning"]),
					WorkDate = Function.ParseDateTimes(row["DueDate"]).Date
				} into join2
				from rowjoin in join2.DefaultIfEmpty()
				select dtresult.LoadDataRow(new object[7]
				{
					tem["ItemCode"],
					tem["Planning"],
					tem["DueDate"],
					(rowjoin == null) ? 0.0 : Function.ParseDouble(rowjoin["Qty"]),
					(rowjoin != null) ? Function.ParseInt(rowjoin["DocEntry"]) : 0,
					tem["ItemName"],
					tem["ItemGroup"]
				}, fAcceptChanges: true)).CopyToDataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	protected override void Find(bool isFind)
	{
		frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListScheduleAtp, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
		{
			AtpCode = (isFind ? Function.ToString(txtInputCode.EditValue) : ""),
			AtpScheduleStatus = (isFind ? Function.ToString(txtStatus.EditValue) : ""),
			AtpScheduleType = Function.ToString(txtType.EditValue)
		};
		frm_CFL2.ShowDialog(this);
		if (frm_CFL2.ChooseItem != null)
		{
			txtCode.EditValue = Function.ParseInt(frm_CFL2.ChooseItem["Code"]);
			FormMode = Enums.FormViewModeType.View;
		}
	}

	protected override void LoadDataAutoFillClick()
	{
		int columnCount = pgcDetail.Cells.ColumnCount;
		int rowCount = pgcDetail.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDetail.GetFieldValue(field_Planning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDetail.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDetail.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDetail.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDetail.CreateDrillDownDataSource(j, i + 1);
				double num = 0.0;
				double num2 = 0.0;
				if (pivotDrillDownDataSource5 != null && pivotDrillDownDataSource5.RowCount > 0)
				{
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0)
					{
						num = Function.ParseDouble(pivotDrillDownDataSource2[0][field_Quantity]);
					}
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDetail.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							num2 = Function.ParseDouble(pivotDrillDownDataSource6[0][field_Quantity]);
						}
					}
					if (Function.ParseDouble(pivotDrillDownDataSource5[0][field_Quantity]) == 0.0)
					{
						num += num2;
						pivotDrillDownDataSource5[0][field_Quantity] = num;
						if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
						{
							pivotDrillDownDataSource[0][field_Quantity] = num;
						}
					}
					else
					{
						num = Function.ParseDouble(pivotDrillDownDataSource5[0][field_Quantity]);
					}
				}
				double num3 = ((pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2[0] != null) ? Function.ParseDouble(pivotDrillDownDataSource2[0][field_Quantity]) : 0.0);
				double num4 = 0.0;
				if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
				{
					num4 = Function.ParseDouble(pivotDrillDownDataSource4[0][field_Quantity]);
				}
				if (pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3.RowCount > 0)
				{
					pivotDrillDownDataSource3[0][field_Quantity] = num3 + num4 + num2 - num;
				}
			}
			i += 4;
		}
		pgcDetail.RefreshData();
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	protected override void LoadDataAutoFillColumn()
	{
		int columnCount = pgcDetail.Cells.ColumnCount;
		int rowCount = pgcDetail.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDetail.GetFieldValue(field_Planning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDetail.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDetail.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDetail.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDetail.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDetail.CreateDrillDownDataSource(j, i + 1);
				double num = 0.0;
				double num2 = 0.0;
				if (pivotDrillDownDataSource5 != null && pivotDrillDownDataSource5.RowCount > 0)
				{
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0)
					{
						num = Function.ParseDouble(pivotDrillDownDataSource2[0][field_Quantity]);
					}
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDetail.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							num2 = Function.ParseDouble(pivotDrillDownDataSource6[0][field_Quantity]);
						}
					}
					if (j == ColumnIndexAutoFill)
					{
						num += num2;
						pivotDrillDownDataSource5[0][field_Quantity] = num;
						if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
						{
							pivotDrillDownDataSource[0][field_Quantity] = num;
						}
					}
					else
					{
						num = Function.ParseDouble(pivotDrillDownDataSource5[0][field_Quantity]);
					}
				}
				double num3 = ((pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2[0] != null) ? Function.ParseDouble(pivotDrillDownDataSource2[0][field_Quantity]) : 0.0);
				double num4 = 0.0;
				if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
				{
					num4 = Function.ParseDouble(pivotDrillDownDataSource4[0][field_Quantity]);
				}
				if (pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3.RowCount > 0)
				{
					pivotDrillDownDataSource3[0][field_Quantity] = num3 + num4 + num2 - num;
				}
			}
			i += 4;
		}
		pgcDetail.RefreshData();
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	protected override void ShowingEditorExtentions(CancelPivotCellEditEventArgs e)
	{
		if (e.ColumnField.FieldName.Equals(field_WorkDate.FieldName) && e.ColumnIndex == 0)
		{
			e.Cancel = true;
		}
	}

	private void InitializeComponent()
	{
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_ScheduleForAtp));
		((System.ComponentModel.ISupportInitialize)base.txtInputCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.chkDetailSchedule.Properties).BeginInit();
		base.SuspendLayout();
		base.txtInputCode.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtInputCode.Properties.Appearance.Options.UseBackColor = true;
		base.txtInputCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtInputCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtStatus.EditValue = "D";
		base.txtStatus.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtStatus.Properties.Appearance.Options.UseBackColor = true;
		base.txtStatus.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtStatus.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtFromDate.EditValue = null;
		base.txtFromDate.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtFromDate.Properties.Appearance.Options.UseBackColor = true;
		base.txtFromDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtFromDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtToDate.EditValue = null;
		base.txtToDate.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtToDate.Properties.Appearance.Options.UseBackColor = true;
		base.txtToDate.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtToDate.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.btn_Save.EditValue = "Find";
		base.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		base.btn_Save.Text = "Find";
		base.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		base.txtFixDate.EditValue = null;
		base.txtFixDate.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtFixDate.Properties.Appearance.Options.UseBackColor = true;
		base.txtType.EditValue = "O";
		base.txtType.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtType.Properties.Appearance.Options.UseBackColor = true;
		base.txtName.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtName.Properties.Appearance.Options.UseBackColor = true;
		base.txtName.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtName.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.txtCode.Properties.Appearance.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtCode.Properties.Appearance.Options.UseBackColor = true;
		base.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		base.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		base.btnAutoFillData.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAutoFillData.ImageOptions.Image");
		base.chkDetailSchedule.EditValue = "N";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(970, 603);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ScheduleForAtp.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ScheduleForAtp";
		((System.ComponentModel.ISupportInitialize)base.txtInputCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFixDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.chkDetailSchedule.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
