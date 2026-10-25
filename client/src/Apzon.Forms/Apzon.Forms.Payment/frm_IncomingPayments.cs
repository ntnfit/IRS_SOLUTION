using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;

namespace Apzon.Forms.Payment;

public class frm_IncomingPayments : frm_VirPayments
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public frm_IncomingPayments()
	{
		InitializeComponent();
		FormId = "oIncomingPayments";
		Text = "Incoming Payments";
		gv_Doc.FormId = FormId;
		gv_Line.FormId = FormId;
		gv_Doc.ItemId = "APZ_ORCT";
		gv_Line.ItemId = "APZ_RCT1";
		FormObjectType = ApzObjectTypes.oIncomingPayment;
	}

	protected override async Task LoadData()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("fromDate", typeof(DateTime));
			dataTable.Columns.Add("toDate", typeof(DateTime));
			dataTable.Columns.Add("docStatus", typeof(string));
			dataTable.Columns.Add("docNum", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["fromDate"] = txtFromDate.EditValue ?? DBNull.Value;
			dataRow["toDate"] = txtToDate.EditValue ?? DBNull.Value;
			dataRow["docStatus"] = Function.ToString(cbxStatus.EditValue);
			dataRow["docNum"] = Function.ParseInt(txtNoOfTrans.EditValue);
			dataTable.Rows.Add(dataRow);
			DataTable dataTable2 = await _httpClient.IncomingPaymentsClient.Search(dataTable);
			dataTable2.SetDefaultAndMaxLen(gv_Doc.ItemId);
			grd_Doc.DataSource = dataTable2;
			grd_Doc.RefreshDataSource();
			dataTable2.AcceptChanges();
			if (!dataTable2.IsNotNull())
			{
				DataTable tableStructure = WinFuntions.GetTableStructure(gv_Line.ItemId);
				dataTable2.SetDefaultAndMaxLen(gv_Line.ItemId);
				grd_Line.DataSource = tableStructure;
				grd_Line.RefreshDataSource();
				tableStructure.AcceptChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async Task LoadLines(int docEntry)
	{
		try
		{
			_isLoading = true;
			DataTable dataTable = await _httpClient.IncomingPaymentsClient.GetLines(docEntry);
			dataTable.SetDefaultAndMaxLen(gv_Line.ItemId);
			grd_Line.DataSource = dataTable;
			grd_Line.RefreshDataSource();
			dataTable.AcceptChanges();
			AddNewRow();
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.View;
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async Task<string> Save(DataSet ds)
	{
		try
		{
			Enums.FormViewModeType currMode = FormMode;
			if (ds.IsNotNull() && ds.Tables.Count > 1 && ds.Tables[0].IsNotNull())
			{
				DataRow dataRow = ds.Tables[0].Rows[0];
				switch (FormMode)
				{
				case Enums.FormViewModeType.Add:
					dataRow["CreateDate"] = DateTime.Today;
					dataRow["CreateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
					dataRow["UserSign"] = GlobalData.UserID;
					break;
				case Enums.FormViewModeType.Update:
					dataRow["UpdateDate"] = DateTime.Today;
					dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
					dataRow["UserSign2"] = GlobalData.UserID;
					break;
				}
			}
			FormMode = Enums.FormViewModeType.Busy;
			string text = await _httpClient.IncomingPaymentsClient.Save(ds);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				if (currMode == Enums.FormViewModeType.Add)
				{
					REFRESH_DATA();
				}
				return string.Empty;
			}
			FormMode = currMode;
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "Something went wrong!";
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
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.Text = "frm_IncomingPayments";
	}
}
