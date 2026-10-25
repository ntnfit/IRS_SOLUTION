using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;

namespace Apzon.Forms.HumanResources.Salary;

public class frm_SalaryViewResult : frm_SalaryResult
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtActiveEmployee;

	private DataTable _dtSalarySlip;

	private readonly ReportDocument _cryRpt = new ReportDocument();

	private readonly string _pathFile = Application.StartupPath + "\\Report\\" + GlobalData.DatabaseName + "\\";

	private IContainer components;

	public frm_SalaryViewResult()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryCalculation"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryCalculation" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			InitializeComponent();
			FormObjectType = ApzObjectTypes.oSalCalculation;
			base.Text = "Salary Result - View";
			FormId = "oSalaryResultView";
			ViewMode = true;
			gv_Doc.FormId = FormId;
			gv_Doc.ItemId = "APZ_OSAL";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override async Task LoadData()
	{
		_ = 2;
		try
		{
			await base.LoadData();
			btn_SendSlip.Visible = true;
			progressSendMail.Visible = true;
			_dtActiveEmployee = await _httpClient.SubActivities.GetActiveEmployees();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("listEmpId", typeof(string));
			dataTable.Columns.Add("branch", typeof(int));
			dataTable.Columns.Add("period", typeof(string));
			dataTable.Rows.Add(Function.ToString(ListEmpNo), Branch, Period);
			_dtSalarySlip = await _httpClient.SalaryCalculationClient.GetSalarySlip(dataTable);
			DataTable dataTable2 = grd_Doc.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				grd_SendMailStt.DataSource = dataTable2.Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async Task<string> SendSalarySlip()
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSenSalarySlipViaEmail, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return "Error";
			}
			DataTable dt = grd_Doc.DataSource as DataTable;
			DataTable dataTable = await _httpClient.SalaryCalculationClient.GetRptFileName();
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.PleaseImportRptFileBeforeSendMail, "");
				return "Error";
			}
			string path = Function.ToString(dataTable.Rows[0]["RptFileName"]);
			string text = _pathFile + Path.GetFileName(path);
			if (!File.Exists(text))
			{
				WinCommons.Error(Enums.MessageCode.RptPathDoesNotExists, "");
				return "Error";
			}
			_cryRpt.Load(text);
			WinCommons._OpenProcessing();
			DataTable dtSendStatus = grd_SendMailStt.DataSource as DataTable;
			SmtpMailClient mailClient = new SmtpMailClient();
			if (dt.IsNotNull())
			{
				if (!progressSendMail.Properties.PercentView)
				{
					progressSendMail.Properties.DisplayFormat.Format = new CustomFormatter(progressSendMail, "", "Completed!");
				}
				List<string> list = (from t in dt.AsEnumerable()
					select Function.ToString(t["empID"])).Distinct().ToList();
				progressSendMail.Properties.Maximum = list.Count;
				progressSendMail.Properties.Minimum = 0;
				progressSendMail.Properties.Step = 1;
				progressSendMail.EditValue = 0;
				foreach (string id in list)
				{
					DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["empID"]) == id);
					if (Function.ParseInt(dataRow["empID"]) == 0 || string.IsNullOrEmpty(Function.ToString(dataRow["WorkTotal"])))
					{
						progressSendMail.PerformStep();
						continue;
					}
					int empId = Function.ParseInt(dataRow["EmpId"]);
					DataRow dataRow2 = _dtActiveEmployee.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["empID"]) == empId);
					if (dataRow2 == null)
					{
						progressSendMail.PerformStep();
						continue;
					}
					string text2 = Function.ToString(dataRow2["email"]);
					if (string.IsNullOrEmpty(text2))
					{
						DataRow dataRow3 = dtSendStatus.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["EmpId"]) == empId);
						if (dataRow3 != null)
						{
							dataRow3["MailSent"] = text2;
							dataRow3["SendMailSuccess"] = "N";
							dataRow3["FailReason"] = "This employee has no email address!";
						}
						dtSendStatus.AcceptChanges();
						progressSendMail.PerformStep();
					}
					else
					{
						await Send_OAUTH(dataRow, text2, mailClient);
						progressSendMail.PerformStep();
					}
				}
			}
			WinCommons._CloseProcessing();
			XtraMessageBoxEx.Show(Enums.MessageCode.InfoSendEmailProcessSuccess, Enums.MessageCode.SystemMessage);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	private async Task Send_OAUTH(DataRow dr, string emailTo, SmtpMailClient mailClient)
	{
		int empId = Function.ParseInt(dr["EmpId"]);
		DataTable dtSendStatus = grd_SendMailStt.DataSource as DataTable;
		try
		{
			string empName = Function.ToString(dr["empName"]);
			string text = "Employee Salary Slip";
			EnumerableRowCollection<DataRow> source = from t in _dtSalarySlip.AsEnumerable()
				where Function.ParseInt(t["IDNV"]) == empId
				select t;
			string value;
			if (!source.Any())
			{
				value = "No salary slip for employee " + empName;
			}
			else
			{
				DataTable dtSlipPerEmp = source.CopyToDataTable();
				string text2 = Function.ToString(dtSlipPerEmp.Rows[0]["TieuDe"]);
				if (!string.IsNullOrEmpty(text2))
				{
					text = text2;
				}
				string subject = text + " " + Period;
				string body = await PopulateBody(empName);
				_ = body == "";
				DiskFileDestinationOptions diskFileDestinationOptions = new DiskFileDestinationOptions();
				PdfRtfWordFormatOptions formatOptions = new PdfRtfWordFormatOptions();
				string text3 = "SalarySlip_" + empName + "_" + Period + ".pdf";
				string item = (diskFileDestinationOptions.DiskFileName = _pathFile + Path.GetFileName(text3));
				ExportOptions exportOptions = _cryRpt.ExportOptions;
				exportOptions.ExportDestinationType = ExportDestinationType.DiskFile;
				exportOptions.ExportFormatType = ExportFormatType.PortableDocFormat;
				exportOptions.DestinationOptions = diskFileDestinationOptions;
				exportOptions.FormatOptions = formatOptions;
				_cryRpt.Refresh();
				_cryRpt.SetDataSource(dtSlipPerEmp);
				Stream value2 = _cryRpt.ExportToStream(ExportFormatType.PortableDocFormat);
				List<string> listMail = new List<string> { emailTo };
				new List<string>().Add(item);
				Dictionary<string, Stream> listAttachStream = new Dictionary<string, Stream> { { text3, value2 } };
				DataTable dataTable = await _httpClient.MailSettingClient.GetDoc();
				string text5 = "";
				if (dataTable.IsNotNull())
				{
					text5 = Function.ToString(dataTable.Rows[0]["MailAddress"]);
				}
				value = ((!string.IsNullOrWhiteSpace(text5)) ? (await mailClient.SendEmail(listMail, subject, body, null, null, new List<string> { text5 }, listAttachStream)) : (await mailClient.SendEmail(listMail, subject, body, null, null, null, listAttachStream)));
			}
			if (dtSendStatus.IsNotNull())
			{
				DataRow dataRow = dtSendStatus.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["EmpId"]) == empId);
				if (dataRow != null)
				{
					dataRow["MailSent"] = emailTo;
					dataRow["SendMailSuccess"] = (string.IsNullOrEmpty(value) ? "Y" : "N");
					dataRow["FailReason"] = value;
				}
				dtSendStatus.AcceptChanges();
				grd_SendMailStt.DataSource = dtSendStatus;
				grd_SendMailStt.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			if (dtSendStatus.IsNotNull())
			{
				DataRow dataRow2 = dtSendStatus.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["EmpId"]) == empId);
				if (dataRow2 != null)
				{
					dataRow2["MailSent"] = emailTo;
					dataRow2["SendMailSuccess"] = "N";
					dataRow2["FailReason"] = ex.Message;
				}
				dtSendStatus.AcceptChanges();
				grd_SendMailStt.DataSource = dtSendStatus;
				grd_SendMailStt.RefreshDataSource();
			}
		}
	}

	private async Task<string> PopulateBody(string empName)
	{
		return "Phiếu lương nhân viên " + empName;
	}

	protected override async Task SaveSendStatus()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grd_SendMailStt.DataSource as DataTable;
			DataSet dataSet = new DataSet();
			if (dataTable != null)
			{
				dataSet.Tables.Add(dataTable.Copy());
			}
			string text = await _httpClient.SalaryCalculationClient.SaveSendMailStt(dataSet);
			if (string.IsNullOrEmpty(text))
			{
				await LoadData();
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
		this.Text = "frm_SalaryViewResult";
	}
}
