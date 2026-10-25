using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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

public class frm_AdvanceOutPayment : frm_AdvancePayment
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public frm_AdvanceOutPayment()
	{
		InitializeComponent();
		FormId = "oAdvanceOutPayment";
		Text = "Advanced Outgoing Payment";
		gv_Doc.FormId = FormId;
		gv_Line.FormId = FormId;
		gv_Doc.ItemId = "APZ_OAPM";
		gv_Line.ItemId = "APZ_APM1";
		FormObjectType = ApzObjectTypes.oAdvancedOutPayment;
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
			DataTable dataTable2 = await _httpClient.OutPaymentClient.Search(dataTable);
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
			DataTable dataTable = await _httpClient.OutPaymentClient.GetLines(docEntry);
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
			string text = await _httpClient.OutPaymentClient.Save(ds);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_AdvanceOutPayment));
		((System.ComponentModel.ISupportInitialize)base.splitContainerControl1).BeginInit();
		base.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)base.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.cbxStatus.Properties).BeginInit();
		base.SuspendLayout();
		base.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btn_Save.Appearance.Options.UseBackColor = true;
		base.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		base.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btnCancel.Appearance.Options.UseBackColor = true;
		base.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		base.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		base.btnSearch.Appearance.Options.UseBackColor = true;
		base.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		base.txtNoOfTrans.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtNoOfTrans.Properties.Appearance.Options.UseFont = true;
		base.txtToDate.EditValue = new System.DateTime(2018, 4, 16, 0, 0, 0, 0);
		base.txtToDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtToDate.Properties.Appearance.Options.UseFont = true;
		base.txtFromDate.EditValue = new System.DateTime(2018, 4, 16, 0, 0, 0, 0);
		base.txtFromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		base.txtFromDate.Properties.Appearance.Options.UseFont = true;
		base.cbxStatus.EditValue = "O";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(971, 490);
		base.Name = "frm_OutgoingPayment";
		this.Text = "Advance Outgoing Payment";
		((System.ComponentModel.ISupportInitialize)base.splitContainerControl1).EndInit();
		base.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)base.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.cbxStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
