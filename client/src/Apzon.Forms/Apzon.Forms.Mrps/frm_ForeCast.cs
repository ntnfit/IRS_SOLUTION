using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Mrps;

public class frm_ForeCast : XtraFormEx
{
	public DataTable DtDocNew = new DataTable();

	public DataTable DtDocLineNew = new DataTable();

	public string FormController = "Forecast";

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_Code;

	private TextEditEx txt_Name;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private LabelLine labelLine4;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridControlEx grd_Line;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private RepositoryItemLookUpEdit rep_View;

	private GridColumnEx gridColumn6;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Caccel;

	private GridViewEx gv_Doc;

	private GridViewEx gv_Line;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private RepositoryItemButtonEdit rep_ItemCode;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	public int OpportId { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ForeCast()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ForeCast"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ForeCast" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Caccel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_ForeCast_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			LoadCbx();
			if (!OpportId.Equals(0))
			{
				DataTable dataSource = await GetForeCastFromOpportunity(OpportId);
				grd_Doc.DataSource = dataSource;
				grd_Doc.RefreshDataSource();
			}
			else
			{
				DataTable dataSource2 = await GetData("", "", null, null);
				grd_Doc.DataSource = dataSource2;
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetForeCastFromOpportunity(int id)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetForeCastFromOpportunity", new { id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void LoadCbx()
	{
		try
		{
			List<Common.Table_ID> dataSource = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					Name = "Monthly",
					ID = "M"
				},
				new Common.Table_ID
				{
					Name = "Weekly",
					ID = "W"
				},
				new Common.Table_ID
				{
					Name = "Daily",
					ID = "D"
				}
			};
			rep_View.DisplayMember = "Name";
			rep_View.ValueMember = "ID";
			rep_View.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			foreach (GridColumn column in gv_Doc.Columns)
			{
				if (column.FieldName.Equals("StartDate") || column.FieldName.Equals("EndDate") || column.FieldName.Equals("FormView"))
				{
					column.OptionsColumn.AllowEdit = false;
				}
			}
			FormMode = Enums.FormViewModeType.View;
			string code = Function.ToString(txt_Code.Text);
			string name = Function.ToString(txt_Name.Text);
			DateTime? fromdate = Function.ParseDateTime(date_From.EditValue);
			DateTime? todate = Function.ParseDateTime(date_To.EditValue);
			DataTable dataSource = await GetData(code, name, fromdate, todate);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<DataTable> GetData(string code, string name, DateTime? fromdate, DateTime? todate)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetData", new { code, name, fromdate, todate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await LoadDataToGridLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataToGridLine()
	{
		try
		{
			WinCommons._OpenProcessing();
			int num = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "AbsID"));
			if (num != 0)
			{
				DataTable dataSource = await GetDataRow(num);
				grd_Line.DataSource = dataSource;
				grd_Line.RefreshDataSource();
				gv_Line.PopulateColumns();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRowDoc()
	{
		try
		{
			gv_Doc.Columns["StartDate"].OptionsColumn.AllowEdit = true;
			gv_Doc.Columns["EndDate"].OptionsColumn.AllowEdit = true;
			gv_Doc.Columns["FormView"].OptionsColumn.AllowEdit = true;
			DateTime today = DateTime.Today;
			DateTime dateTime = new DateTime(DateTime.Today.Year, 12, 31);
			if (!DtDocNew.IsNotNull())
			{
				DtDocNew = new DataTable();
				DtDocNew.Columns.Add("AbsID", typeof(int));
				DtDocNew.Columns.Add("Code", typeof(string));
				DtDocNew.Columns.Add("Name", typeof(string));
				DtDocNew.Columns.Add("UserSign", typeof(int));
				DtDocNew.Columns.Add("StartDate", typeof(DateTime));
				DtDocNew.Columns.Add("EndDate", typeof(DateTime));
				DtDocNew.Columns.Add("FormView", typeof(string));
				DataRow dataRow = DtDocNew.NewRow();
				dataRow["StartDate"] = today;
				dataRow["EndDate"] = dateTime;
				dataRow["AbsID"] = 0;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["FormView"] = ForeCastFormView.M.ToString();
				DtDocNew.Rows.Add(dataRow);
				DtDocNew.AcceptChanges();
				grd_Doc.DataSource = DtDocNew;
				grd_Doc.RefreshDataSource();
			}
			grd_Doc.DataSource = DtDocNew;
			grd_Doc.RefreshDataSource();
			ResetLines(today, dateTime);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetLines(DateTime startDate, DateTime endDate, string formView = "M")
	{
		WinCommons._OpenProcessing();
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			DtDocLineNew = new DataTable();
			gv_Line.Columns.Clear();
			DtDocLineNew.Columns.Add("ItemCode", typeof(string));
			DtDocLineNew.Columns.Add("ItemName", typeof(string));
			DtDocLineNew.Columns.Add("AbsId", typeof(int));
			DtDocLineNew.Columns.Add("LineID", typeof(int));
			GridColumn column = new GridColumn
			{
				FieldName = "ItemCode",
				Visible = true,
				ColumnEdit = rep_ItemCode
			};
			gv_Line.Columns.Add(column);
			column = new GridColumn
			{
				FieldName = "ItemName",
				Visible = true
			};
			gv_Line.Columns.Add(column);
			RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit.DisplayFormat.Format = GlobalData.QuantitiesFormat;
			switch (formView)
			{
			case "M":
			{
				startDate = ((startDate.Day > 1) ? new DateTime(startDate.Year, DateTime.Today.Month, 1).AddMonths(1) : startDate);
				grd_Doc.DataSourceTable.Rows[0]["StartDate"] = startDate;
				grd_Doc.DataSourceTable.AcceptChanges();
				for (int j = 0; j < 12 - startDate.Month; j++)
				{
					DateTime dateTime2 = startDate.AddMonths(j);
					string text2 = dateTime2.ToString("[MM-dd-yyyy]");
					string caption2 = dateTime2.ToString("M").Substring(0, 3) + ".";
					DataColumn column4 = new DataColumn
					{
						ColumnName = text2,
						DataType = typeof(double),
						DefaultValue = 0
					};
					DtDocLineNew.Columns.Add(column4);
					GridColumn column5 = new GridColumn
					{
						FieldName = text2,
						Caption = caption2,
						ColumnEdit = repositoryItemCalcEdit,
						Visible = true
					};
					gv_Line.Columns.Add(column5);
				}
				break;
			}
			case "D":
			{
				DateTime[] array = (from offset in Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
					select startDate.AddDays(offset)).ToArray();
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					string text3 = array[num2].ToString("[MM-dd-yyyy]");
					string caption3 = array[num2].ToString(GlobalData.DateTimeFormat);
					DataColumn column6 = new DataColumn
					{
						ColumnName = text3,
						DataType = typeof(double),
						DefaultValue = 0
					};
					DtDocLineNew.Columns.Add(column6);
					GridColumn column7 = new GridColumn
					{
						FieldName = text3,
						Caption = caption3,
						ColumnEdit = repositoryItemCalcEdit,
						Visible = true
					};
					gv_Line.Columns.Add(column7);
				}
				break;
			}
			case "W":
			{
				DateTime dateTime = startDate.AddDays(0 - startDate.DayOfWeek).AddDays(1.0);
				grd_Doc.DataSourceTable.Rows[0]["StartDate"] = dateTime;
				grd_Doc.DataSourceTable.AcceptChanges();
				int num = (int)((endDate - dateTime).TotalDays / 7.0);
				for (int i = 0; i < num; i++)
				{
					DateTime time = dateTime.AddDays(7 * i);
					string text = time.ToString("[MM-dd-yyyy]");
					string caption = Function.GetIso8601WeekOfYear(time, CalendarWeekRule.FirstDay).ToString();
					DataColumn column2 = new DataColumn
					{
						ColumnName = text,
						DataType = typeof(double),
						DefaultValue = 0
					};
					DtDocLineNew.Columns.Add(column2);
					GridColumn column3 = new GridColumn
					{
						FieldName = text,
						Caption = caption,
						ColumnEdit = repositoryItemCalcEdit,
						Visible = true
					};
					gv_Line.Columns.Add(column3);
				}
				break;
			}
			}
			if (Function.ParseInt(dataSourceTable.Rows[0]["AbsId"]) == 0)
			{
				for (int num3 = 0; num3 < dataSourceTable.Rows.Count; num3++)
				{
					string value = Function.ToString(dataSourceTable.Rows[num3]["ItemCode"]);
					if (!string.IsNullOrEmpty(value))
					{
						DataRow dataRow = DtDocLineNew.NewRow();
						dataRow["ItemCode"] = value;
						dataRow["ItemName"] = dataSourceTable.Rows[num3]["ItemName"];
						dataRow["AbsId"] = 0;
						dataRow["LineID"] = num3;
						DtDocLineNew.Rows.Add(dataRow);
					}
				}
			}
			DtDocLineNew.Rows.Add(DtDocLineNew.NewRow());
			grd_Line.DataSource = DtDocLineNew;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void AddNewRowLine()
	{
		try
		{
			DataTable dataTable = grd_Line.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["ItemCode"])))
				{
					DataRow dataRow = dataTable.NewRow();
					int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsID"));
					dataRow["AbsID"] = num;
					dataRow["LineID"] = Function.ParseInt(dataTable.Rows[dataTable.Rows.Count - 1]["LineID"]) + 1;
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_Line.DataSource = dataTable;
					grd_Line.RefreshDataSource();
				}
				return;
			}
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("AbsID", typeof(int));
			dataTable2.Columns.Add("LineID", typeof(int));
			dataTable2.Columns.Add("ItemCode", typeof(string));
			dataTable2.Columns.Add("ItemName", typeof(string));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("FormView"));
			DateTime dateTime = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			DateTime dateTime2 = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("EndDate"));
			switch (text)
			{
			case "W":
			{
				DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
				Calendar calendar = currentInfo.Calendar;
				int weekOfYear = calendar.GetWeekOfYear(dateTime, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
				int weekOfYear2 = calendar.GetWeekOfYear(dateTime, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
				for (int i = weekOfYear; i <= weekOfYear2; i++)
				{
					dataTable2.Columns.Add(Function.ToString(i), typeof(double));
				}
				break;
			}
			case "M":
			{
				DateTime dateTime4 = dateTime;
				while (dateTime4 <= dateTime2)
				{
					dataTable2.Columns.Add(Function.ToString(dateTime4), typeof(double));
					dateTime4 = dateTime4.AddMonths(1);
				}
				break;
			}
			case "D":
			{
				DateTime dateTime3 = dateTime;
				while (dateTime3 <= dateTime2)
				{
					dataTable2.Columns.Add(Function.ToString(dateTime3), typeof(double));
					dateTime3 = dateTime3.AddDays(1.0);
				}
				break;
			}
			}
			DataRow dataRow2 = dataTable2.NewRow();
			int num2 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsID"));
			dataRow2["AbsID"] = num2;
			dataRow2["LineID"] = 1;
			dataTable2.Rows.Add(dataRow2);
			dataTable2.AcceptChanges();
			grd_Line.DataSource = dataTable2;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreadColumnGridByTime()
	{
		try
		{
			grd_Line.DataSource = null;
			int num;
			for (num = 4; num < gv_Line.Columns.Count; num++)
			{
				GridColumn column = gv_Line.Columns[num];
				gv_Line.Columns.Remove(column);
				num--;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("FormView"));
			DateTime dateTime = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			DateTime dateTime2 = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("EndDate"));
			DateTimeFormatInfo currentInfo = DateTimeFormatInfo.CurrentInfo;
			Calendar calendar = currentInfo.Calendar;
			int weekOfYear = calendar.GetWeekOfYear(dateTime, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
			int weekOfYear2 = calendar.GetWeekOfYear(dateTime2, currentInfo.CalendarWeekRule, currentInfo.FirstDayOfWeek);
			switch (text)
			{
			case "W":
			{
				for (int i = weekOfYear; i <= weekOfYear2; i++)
				{
					GridColumn gridColumn3 = new GridColumn();
					gridColumn3.Caption = Function.ToString(i);
					gridColumn3.FieldName = dateTime.ToString("MM-dd-yyyy");
					gridColumn3.Name = "col" + Function.ToString(dateTime);
					gridColumn3.Visible = true;
					gridColumn3.OptionsColumn.AllowEdit = true;
					gv_Line.Columns.Add(gridColumn3);
					dateTime = dateTime.AddDays(7.0);
				}
				break;
			}
			case "M":
				while (dateTime <= dateTime2)
				{
					GridColumn gridColumn2 = new GridColumn();
					gridColumn2.Caption = dateTime.ToString("MMM") + ".";
					gridColumn2.FieldName = dateTime.ToString("MM-dd-yyyy");
					gridColumn2.Name = "col" + Function.ToString(dateTime);
					gridColumn2.Visible = true;
					gridColumn2.OptionsColumn.AllowEdit = true;
					gv_Line.Columns.Add(gridColumn2);
					dateTime = dateTime.AddMonths(1);
				}
				break;
			case "D":
				while (dateTime <= dateTime2)
				{
					GridColumn gridColumn = new GridColumn();
					gridColumn.Caption = dateTime.ToString("MM-dd");
					gridColumn.FieldName = dateTime.ToString("MM-dd-yyyy");
					gridColumn.Name = "col" + Function.ToString(dateTime);
					gridColumn.Visible = true;
					gridColumn.OptionsColumn.AllowEdit = true;
					gv_Line.Columns.Add(gridColumn);
					dateTime = dateTime.AddDays(1.0);
				}
				break;
			}
			foreach (GridColumn column2 in gv_Line.Columns)
			{
				column2.Width = 100;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable CreadColumnTableByTime()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add("AbsID", typeof(int));
			dataTable.Columns.Add("LineID", typeof(int));
			dataTable.Columns.Add("ItemCode", typeof(string));
			dataTable.Columns.Add("ItemName", typeof(string));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("FormView"));
			DateTime dateTime = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("StartDate"));
			DateTime dateTime2 = Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("EndDate"));
			switch (text)
			{
			case "W":
				while (dateTime <= dateTime2)
				{
					dataTable.Columns.Add(dateTime.ToString("MM-dd-yyyy"), typeof(double));
					dateTime = dateTime.AddDays(7.0);
				}
				break;
			case "M":
				while (dateTime <= dateTime2)
				{
					dataTable.Columns.Add(dateTime.ToString("MM-dd-yyyy"), typeof(double));
					dateTime = dateTime.AddMonths(1);
				}
				break;
			case "D":
			{
				DateTime dateTime3 = dateTime;
				while (dateTime3 <= dateTime2)
				{
					dataTable.Columns.Add(dateTime.ToString("MM-dd-yyyy"), typeof(double));
					dateTime = dateTime.AddDays(1.0);
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return dataTable;
	}

	private async Task<DataTable> GetMaxAbsId()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetMaxAbsId");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetMaxLineIdByAbsId(int absID)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetMaxLineIdByAbsId", absID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetDataRow(int absId)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetDataRow", new { absId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void btn_Caccel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		WinCommons._OpenProcessing();
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataSet dataSet = new DataSet();
				DataRow[] array = dataTable.Select("AbsId=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsId")));
				if (array.Length != 0)
				{
					DataTable dataTable2 = array.CopyToDataTable();
					dataSet.Tables.Add(dataTable2.Copy());
					DataTable dataSourceTable = grd_Line.DataSourceTable;
					if ((from t in dataSourceTable.AsEnumerable()
						where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
						select t).ToList().Count > 0)
					{
						DataTable dataTable3 = dataSourceTable.Copy();
						(from DataRow t in dataTable3.Rows
							where string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
							select t).ToList().ForEach((DataRow r) =>
						{
							r.Delete();
						});
						dataSet.Tables.Add(dataTable3);
						HttpResult httpResult;
						using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController))
						{
							httpResult = await helper.PostAsync("SaveData", dataSet);
						}
						if (!string.IsNullOrEmpty(httpResult.Message))
						{
							WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.DetailIsEmpty, "");
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.TitleDataIsEmpty, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void rep_View_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				DateTime startDate = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("StartDate")) ?? DateTime.Today;
				DateTime endDate = Function.ParseDateTime(gv_Doc.GetFocusedRowCellValue("EndDate")) ?? DateTime.Today;
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ForecastConfirmChanging, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					string formView = Function.ToString(e.NewValue);
					ResetLines(startDate, endDate, formView);
				}
				else
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowDoc();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ItemDescription";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[i];
				string itemCode = Function.ToString(dataRow["ItemCode"]);
				string value = Function.ToString(dataRow["ItemDescription"]);
				if (!string.IsNullOrEmpty(itemCode) && !itemCode.Equals(Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"))) && (from t in dataSourceTable.AsEnumerable()
					where Function.ToString(t["ItemCode"]).Equals(itemCode)
					select t).ToList().Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ItemCannotAppearTwiceInForecast, "");
					return;
				}
				if (i != 0)
				{
					dataSourceTable.Rows.InsertAt(dataSourceTable.NewRow(), i);
				}
				gv_Line.SetRowCellValue(i, "ItemCode", itemCode);
				gv_Line.SetRowCellValue(i, "ItemName", value);
			}
			if ((from t in dataSourceTable.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["ItemCode"]))
				select t).ToList().Count <= 0)
			{
				dataSourceTable.Rows.Add(dataSourceTable.NewRow());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int item = Function.ParseInt(gv_Line.GetFocusedRowCellValue("AbsID"));
			string item2 = Function.ToString(gv_Line.GetFocusedRowCellValue("ItemCode"));
			DateTime item3 = Function.ParseDateTimes(gv_Line.GetFocusedRowCellValue("ItemCode"));
			Tuple<int, string, DateTime> tup = new Tuple<int, string, DateTime>(item, item2, item3);
			string text = await DeleteDataLine(tup);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadDataToGridLine();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> DeleteDataLine(Tuple<int, string, DateTime> tup)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("DeleteDataLine", tup);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int abs = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsID"));
				WinCommons._OpenProcessing();
				string text = await DeleteData(abs);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					DataTable dataSource = await GetData("", "", null, null);
					grd_Doc.DataSource = dataSource;
					grd_Doc.RefreshDataSource();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> DeleteData(int abs)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("DeleteData", abs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_ForeCast));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_View = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Caccel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Forecast Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Forecast Code";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(92, 10);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(156, 20);
		this.txt_Code.TabIndex = 1;
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(92, 36);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(156, 20);
		this.txt_Name.TabIndex = 3;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Forecast Name";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(13, 39);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Forecast Name";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "From Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(254, 13);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "From Date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(333, 10);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(156, 20);
		this.date_From.TabIndex = 5;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(333, 36);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.IsSearchControl = false;
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(156, 20);
		this.date_To.TabIndex = 7;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(254, 39);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "To Date";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(496, 35);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(91, 20);
		this.btn_Search.TabIndex = 8;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 62);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(781, 408);
		this.splitContainerControl1.SplitterPosition = 192;
		this.splitContainerControl1.TabIndex = 9;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_View });
		this.grd_Doc.Size = new System.Drawing.Size(781, 192);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem, this.duplicateToolStripMenuItem });
		this.ctx_Doc.Name = "contextMenuStrip1";
		this.ctx_Doc.Size = new System.Drawing.Size(125, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addRowToolStripMenuItem.Text = "Add";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.deleteRowToolStripMenuItem.Text = "Remove";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oForecast";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gridColumn1.Caption = "Forecast Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Code";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Forecast Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Name";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn3.Caption = "Start Date";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.DisplayFormat.FormatString = "MM/dd/yyyy";
		this.gridColumn3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gridColumn3.FieldName = "StartDate";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn4.Caption = "End Date";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormat.FormatString = "MM/dd/yyyy";
		this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gridColumn4.FieldName = "EndDate";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn5.Caption = "View";
		this.gridColumn5.ColumnEdit = this.rep_View;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "FormView";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.rep_View.AutoHeight = false;
		this.rep_View.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_View.Name = "rep_View";
		this.rep_View.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_View_EditValueChanging);
		this.gridColumn6.Caption = "AbsID";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "AbsID";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_ItemCode });
		this.grd_Line.Size = new System.Drawing.Size(781, 206);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10 });
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oForecast";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "OLINE";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gridColumn7.Caption = "AbsID";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "AbsID";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = true;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "LineID";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "LineID";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn9.Caption = "Item No.";
		this.gridColumn9.ColumnEdit = this.rep_ItemCode;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "ItemCode";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 0;
		this.gridColumn9.Width = 148;
		this.rep_ItemCode.AutoHeight = false;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonClick);
		this.gridColumn10.Caption = "Item Description";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "ItemName";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 1;
		this.gridColumn10.Width = 1123;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 476);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(91, 20);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Caccel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Caccel.EditControl = null;
		this.btn_Caccel.EditValue = "Cancel";
		this.btn_Caccel.FieldName = null;
		this.btn_Caccel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Caccel.ImageOptions.Image");
		this.btn_Caccel.IsSearchControl = false;
		this.btn_Caccel.Location = new System.Drawing.Point(95, 476);
		this.btn_Caccel.Name = "btn_Caccel";
		this.btn_Caccel.ReadOnly = false;
		this.btn_Caccel.SettingInVisible = false;
		this.btn_Caccel.SettingReadonly = false;
		this.btn_Caccel.Size = new System.Drawing.Size(91, 20);
		this.btn_Caccel.TabIndex = 11;
		this.btn_Caccel.Text = "Cancel";
		this.btn_Caccel.Click += new System.EventHandler(this.btn_Caccel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(780, 500);
		base.Controls.Add(this.btn_Caccel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oForecast";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ForeCast";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Forecast";
		base.Load += new System.EventHandler(this.frm_ForeCast_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		base.ResumeLayout(false);
	}
}
