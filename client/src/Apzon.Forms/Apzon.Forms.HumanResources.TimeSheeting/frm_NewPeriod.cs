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
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_NewPeriod : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private const string FindString = "Find";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public DataTable _dtClone = new DataTable();

	public int BranchFromTimeSheeting;

	private bool isLoad;

	public ReloadWorkPeriod SendDataBackToMain;

	private IContainer components;

	private LabelLine labelLine1;

	private TextEditEx txt_Year;

	private LabelLine labelLine2;

	private TextEditEx txt_Code;

	private LabelLine labelLine3;

	private TextEditEx txt_Num;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine8;

	private TextEditEx txt_Name;

	private LabelLine labelLine9;

	private LookUpEdit look_Sub;

	private LookUpEdit look_Status;

	private LookUpEdit look_Branch;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private DateEditEx date_FD;

	private DateEditEx date_TD;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private CalcEditEx calc_WorkQuotation;

	private LabelLine lbl_hourperday;

	private CalcEditEx calc_HourPerDay;

	private LabelLine lbl_workQuotation;

	private CalcEditEx calc_ILtotal;

	private CalcEditEx calc_FPLtotal;

	private LabelLine labelLine10;

	private LabelLine labelLine11;

	private LabelLine labelLine12;

	private CheckEditEx check_IsLastPeriod;

	private XtraTabPage xTabUDF;

	private XtraTabControl xtraTabControl1;

	private GridControlEx grd_UDF;

	private GridViewEx gv_UDF;

	public frm_NewPeriod()
	{
		InitializeComponent();
		OnLoadAsync += frm_NewPeriod_Load;
		FormMode = Enums.FormViewModeType.Add;
	}

	public frm_NewPeriod(DataTable dtClone)
	{
		InitializeComponent();
		OnLoadAsync += frm_NewPeriod_Load;
		_dtClone = dtClone;
		FormMode = Enums.FormViewModeType.View;
	}

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
				btn_Ok.Text = "Add";
				look_Status.EditValue = "U";
				txt_Code.ReadOnly = false;
				date_TD.ReadOnly = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				look_Sub.ReadOnly = true;
				txt_Num.ReadOnly = true;
				txt_Year.ReadOnly = true;
				look_Status.ReadOnly = false;
				txt_Code.ReadOnly = false;
				date_TD.ReadOnly = false;
				break;
			case Enums.FormViewModeType.Find:
				look_Sub.ReadOnly = true;
				txt_Num.ReadOnly = true;
				txt_Year.ReadOnly = true;
				btn_Ok.Text = "Find";
				look_Status.ReadOnly = false;
				txt_Code.ReadOnly = true;
				date_TD.ReadOnly = true;
				break;
			case Enums.FormViewModeType.View:
				look_Sub.ReadOnly = true;
				txt_Num.ReadOnly = true;
				txt_Year.ReadOnly = true;
				btn_Ok.Text = "OK";
				look_Status.ReadOnly = false;
				txt_Code.ReadOnly = false;
				date_TD.ReadOnly = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_NewPeriod_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			isLoad = true;
			date_FD.SetFormatControl(Enums.DisplayFormatType.DateTime);
			date_TD.SetFormatControl(Enums.DisplayFormatType.DateTime);
			SetDataSub();
			SetDataStaus();
			await LoadLookupBranches();
			SetDataPedTime();
			await LoadUDF();
			if (_dtClone != null && _dtClone.Rows.Count > 0)
			{
				SetDataDoc(_dtClone.Rows[0]);
			}
			if (string.IsNullOrEmpty(Function.ToString(txt_Code.EditValue)))
			{
				date_FD.EditValue = DateTime.Now.Date;
				date_TD.EditValue = DateTime.Now.Date;
				check_IsLastPeriod.EditValue = 'N';
			}
			isLoad = false;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadUDF()
	{
		try
		{
			await WinFuntions.LoadUdfColumns(gv_UDF, "APZ_OHWP");
			DataTable dataSourceTable = grd_UDF.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Rows.Count == 0)
			{
				WinFuntions.GetNewRow_UDF(grd_UDF, "APZ_OHWP");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Saves(1);
				break;
			case Enums.FormViewModeType.Update:
				await Saves(2);
				break;
			case Enums.FormViewModeType.Find:
				await Find(isFind: true);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void Control_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (isLoad)
			{
				return;
			}
			if (((dynamic)sender).Name.Equals(txt_Num.Name))
			{
				string subPeriod = Function.ToString(look_Sub.EditValue);
				int numOfPeriod = Function.ParseInt(e.NewValue);
				DateTime fromDate = Function.ParseDateTimes(date_FD.EditValue);
				SetValueToDate(subPeriod, numOfPeriod, fromDate);
			}
			if (((dynamic)sender).Name.Equals(date_FD.Name))
			{
				string subPeriod2 = Function.ToString(look_Sub.EditValue);
				int numOfPeriod2 = Function.ParseInt(txt_Num.EditValue);
				DateTime fromDate2 = Function.ParseDateTimes(e.NewValue);
				if (FormMode == Enums.FormViewModeType.Add)
				{
					SetValueToDate(subPeriod2, numOfPeriod2, fromDate2);
				}
			}
			if (((dynamic)sender).Name.Equals(look_Status.Name))
			{
				if (!string.Equals(Function.ToString(e.NewValue), "U"))
				{
					calc_FPLtotal.ReadOnly = true;
					calc_ILtotal.ReadOnly = true;
				}
				else
				{
					calc_FPLtotal.ReadOnly = false;
					calc_ILtotal.ReadOnly = false;
				}
			}
			InitChangeDataEvent();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(txt_Code.EditValue);
			int num = Function.ParseInt(look_Branch.EditValue);
			if ((!string.IsNullOrEmpty(text) || num >= 0) && MessageBox.Show("Do you want to remove this item ?", "System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Remove(text, num);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		ResetFormControl();
		FormMode = Enums.FormViewModeType.Find;
		txt_Code.Focus();
	}

	private void SetDataSub()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Year",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Quarters",
			Code = "Q"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Month",
			Code = "M"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Day",
			Code = "D"
		});
		look_Sub.Properties.DataSource = list;
	}

	private void SetDataStaus()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Unlock",
			Code = "U"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Closing",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Closed",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Archived",
			Code = "A"
		});
		look_Status.Properties.DataSource = list;
	}

	private void SetDataPedTime()
	{
		new List<Common.Table_Code>
		{
			new Common.Table_Code
			{
				Name = "Default",
				Code = "D"
			},
			new Common.Table_Code
			{
				Name = "Reality",
				Code = "R"
			}
		};
	}

	private async Task LoadLookupBranches()
	{
		try
		{
			DataTable dataSource = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			look_Branch.Properties.DataSource = dataSource;
			look_Branch.Properties.DisplayMember = "Name";
			look_Branch.Properties.ValueMember = "Code";
			if (BranchFromTimeSheeting != 0)
			{
				look_Branch.EditValue = Function.ParseShort(BranchFromTimeSheeting);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataDoc(DataRow row)
	{
		try
		{
			if (row == null)
			{
				return;
			}
			txt_Year.EditValue = Function.ParseInt(row["PYear"]);
			txt_Code.EditValue = Function.ToString(row["PeriodCode"]);
			txt_Name.EditValue = Function.ToString(row["PeriodName"]);
			look_Sub.EditValue = Function.ToString(row["SubPeriod"]);
			txt_Num.EditValue = Function.ParseInt(row["NumOfPeriod"]);
			look_Status.EditValue = Function.ToString(row["PeriodStatus"]);
			date_FD.EditValue = Function.ParseDateTime(row["FromDate"]);
			date_TD.EditValue = Function.ParseDateTime(row["ToDate"]);
			look_Branch.EditValue = Convert.ToInt16(row["Branch"]);
			calc_HourPerDay.EditValue = Function.ParseDouble(row["HourPerDay"]);
			calc_WorkQuotation.EditValue = Function.ParseDouble(row["WorkQuota"]);
			calc_FPLtotal.EditValue = Function.ParseDouble(row["FPLTotal"]);
			calc_ILtotal.EditValue = Function.ParseDouble(row["IncreaseLeaveTotal"]);
			check_IsLastPeriod.EditValue = Function.ToString(row["IsLastPeriod"]);
			if (string.Equals(Function.ToString(look_Status.EditValue), "U"))
			{
				calc_FPLtotal.ReadOnly = false;
				calc_ILtotal.ReadOnly = false;
			}
			else
			{
				calc_FPLtotal.ReadOnly = true;
				calc_ILtotal.ReadOnly = true;
			}
			if (!grd_UDF.DataSourceTable.IsNotNull())
			{
				return;
			}
			foreach (DataColumn item in (from DataColumn c in _dtClone.Columns
				where c.ColumnName.ToUpper().StartsWith("U_")
				select c).ToList())
			{
				gv_UDF.SetRowCellValue(0, Function.ToString(item.ColumnName), row[item.ColumnName]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetFormControl()
	{
		txt_Year.EditValue = null;
		txt_Code.EditValue = null;
		txt_Name.EditValue = null;
		look_Sub.EditValue = null;
		txt_Num.EditValue = null;
		look_Status.EditValue = null;
		look_Branch.EditValue = null;
		date_FD.EditValue = null;
		date_TD.EditValue = null;
		calc_HourPerDay.EditValue = null;
		calc_WorkQuotation.EditValue = null;
		check_IsLastPeriod.EditValue = null;
	}

	private void GetDataDoc(ref DataTable dt)
	{
		try
		{
			dt = WinFuntions.GetTableStructure("APZ_OHWP");
			if (FormMode == Enums.FormViewModeType.Add)
			{
				int num = Function.ParseInt(txt_Num.EditValue);
				DateTime date = Function.ParseDateTimes(date_FD.EditValue).Date;
				_ = Function.ParseDateTimes(date_TD.EditValue).Date;
				string text = Function.ToString(look_Sub.EditValue);
				_ = date.Month;
				_ = date.Year;
				ArrayList arrayList = new ArrayList();
				string text2 = num.ToString();
				for (int i = 0; i < text2.Length - 1; i++)
				{
					arrayList.Add(0);
				}
				string text3 = string.Join("", (from o in arrayList.ToArray()
					select o.ToString()).ToArray());
				for (int num2 = 0; num2 < num; num2++)
				{
					DataRow dataRow = dt.NewRow();
					int num3 = num2 + 1;
					dataRow["PYear"] = Function.ParseInt(txt_Year.EditValue);
					if (text2.Length >= 2)
					{
						if (num2 <= 8)
						{
							dataRow["PeriodCode"] = Function.ToString(txt_Code.EditValue) + "-" + text3 + num3;
							dataRow["PeriodName"] = Function.ToString(txt_Name.EditValue) + "-" + text3 + num3;
						}
						else if (num2 >= 9)
						{
							dataRow["PeriodCode"] = Function.ToString(txt_Code.EditValue) + "-" + num3;
							dataRow["PeriodName"] = Function.ToString(txt_Name.EditValue) + "-" + num3;
						}
					}
					else if (text2.Length == 1)
					{
						dataRow["PeriodCode"] = Function.ToString(txt_Code.EditValue) + "-" + num3;
						dataRow["PeriodName"] = Function.ToString(txt_Name.EditValue) + "-" + num3;
					}
					dataRow["SubPeriod"] = Function.ToString(look_Sub.EditValue);
					dataRow["NumOfPeriod"] = Function.ToString(txt_Num.EditValue);
					dataRow["PeriodStatus"] = Function.ToString(look_Status.EditValue);
					switch (text)
					{
					case "Y":
						dataRow["FromDate"] = date.AddYears(num2);
						dataRow["ToDate"] = date.AddYears(num2 + 1).AddDays(-1.0);
						break;
					case "Q":
						dataRow["FromDate"] = date.AddMonths(num2 * 3);
						dataRow["ToDate"] = date.AddMonths(num3 * 3).AddDays(-1.0);
						break;
					case "M":
						dataRow["FromDate"] = date.AddMonths(num2);
						dataRow["ToDate"] = date.AddMonths(num2 + 1).AddDays(-1.0);
						break;
					case "D":
						dataRow["FromDate"] = date.AddDays(num2);
						dataRow["ToDate"] = date.AddDays(num2);
						break;
					}
					dataRow["Branch"] = Function.ParseInt(look_Branch.EditValue);
					dataRow["WorkQuota"] = Function.ParseDouble(calc_WorkQuotation.EditValue);
					dataRow["HourPerDay"] = Function.ParseDouble(calc_HourPerDay.EditValue);
					dataRow["FPLTotal"] = 0;
					dataRow["IncreaseLeaveTotal"] = Function.ParseDouble(calc_ILtotal.EditValue);
					dataRow["IsLastPeriod"] = Function.ToString(check_IsLastPeriod.EditValue);
					dataRow["LogInstanc"] = 0;
					dataRow["ID"] = 0;
					DataTable dataSourceTable = grd_UDF.DataSourceTable;
					if (dataSourceTable.IsNotNull())
					{
						foreach (DataColumn item in from DataColumn col in dt.Columns
							where col.ColumnName.ToUpper().StartsWith("U_")
							select col)
						{
							dataRow[item.ColumnName] = dataSourceTable.Rows[0][item.ColumnName];
						}
					}
					dt.Rows.Add(dataRow);
				}
				return;
			}
			DataRow dataRow2 = dt.NewRow();
			dataRow2["PYear"] = Function.ParseInt(txt_Year.EditValue);
			dataRow2["PeriodCode"] = Function.ToString(txt_Code.EditValue);
			dataRow2["PeriodName"] = Function.ToString(txt_Name.EditValue);
			dataRow2["SubPeriod"] = Function.ToString(look_Sub.EditValue);
			dataRow2["NumOfPeriod"] = Function.ToString(txt_Num.EditValue);
			dataRow2["PeriodStatus"] = Function.ToString(look_Status.EditValue);
			dataRow2["FromDate"] = Function.ParseDateTimes(date_FD.EditValue).Date;
			dataRow2["ToDate"] = Function.ParseDateTimes(date_TD.EditValue).Date;
			dataRow2["Branch"] = Function.ParseInt(look_Branch.EditValue);
			dataRow2["WorkQuota"] = Function.ParseDouble(calc_WorkQuotation.EditValue);
			dataRow2["HourPerDay"] = Function.ParseDouble(calc_HourPerDay.EditValue);
			dataRow2["FPLTotal"] = 0;
			dataRow2["IncreaseLeaveTotal"] = Function.ParseDouble(calc_ILtotal.EditValue);
			dataRow2["IsLastPeriod"] = Function.ToString(check_IsLastPeriod.EditValue);
			dataRow2["LogInstanc"] = 0;
			dataRow2["ID"] = _dtClone.Rows[0]["ID"];
			DataTable dataSourceTable2 = grd_UDF.DataSourceTable;
			if (dataSourceTable2.IsNotNull())
			{
				foreach (DataColumn item2 in from DataColumn col in dt.Columns
					where col.ColumnName.ToUpper().StartsWith("U_")
					select col)
				{
					dataRow2[item2.ColumnName] = dataSourceTable2.Rows[0][item2.ColumnName];
				}
			}
			dt.Rows.Add(dataRow2);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Saves(int type)
	{
		_ = 1;
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(date_FD.EditValue)))
			{
				date_FD.Focus();
				WinCommons.Warning(Enums.MessageCode.FromDateCanNotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(date_TD.EditValue)))
			{
				date_TD.Focus();
				WinCommons.Warning(Enums.MessageCode.ToDateCanNotBeNull, "");
				return 1;
			}
			if (!string.IsNullOrEmpty(Function.ToString(date_TD.EditValue)))
			{
				date_TD.Focus();
				if (Function.ParseDateTimes(date_TD.EditValue) < Function.ParseDateTimes(date_FD.EditValue))
				{
					WinCommons.Warning(Enums.MessageCode.ToDateMustGreaterFromDate, "");
					return 1;
				}
				if (Function.ToString(look_Sub.EditValue) != "D" && Function.ParseDateTimes(date_TD.EditValue) == Function.ParseDateTimes(date_FD.EditValue))
				{
					WinCommons.Warning(Enums.MessageCode.ToDateMustGreaterFromDate, "");
					return 1;
				}
				if (Function.ToString(look_Sub.EditValue) == "D" && Function.ParseInt(txt_Num.EditValue) == 0 && Function.ParseDateTimes(date_TD.EditValue) != Function.ParseDateTimes(date_FD.EditValue))
				{
					WinCommons.Warning(Enums.MessageCode.InvalidDate, "");
					return 1;
				}
			}
			if (string.IsNullOrEmpty(Function.ToString(txt_Code.EditValue)))
			{
				txt_Code.Focus();
				WinCommons.Warning(Enums.MessageCode.PeriodCodeMustNotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(txt_Name.EditValue)))
			{
				txt_Name.Focus();
				WinCommons.Warning(Enums.MessageCode.PeriodNameMustNotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(txt_Num.EditValue)))
			{
				txt_Num.Focus();
				WinCommons.Warning(Enums.MessageCode.NumberOfPeriodMustNotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(txt_Year.EditValue)))
			{
				txt_Year.Focus();
				WinCommons.Warning(Enums.MessageCode.YearMustNotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(look_Branch.EditValue)))
			{
				look_Branch.Focus();
				WinCommons.Warning(Enums.MessageCode.BranchCannotBeEmpty, "");
				return 1;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dtdoc = new DataTable();
				GetDataDoc(ref dtdoc);
				if (dtdoc.IsNotNull())
				{
					if (Function.ParseDouble(calc_FPLtotal.EditValue) > 0.0)
					{
						dtdoc.Rows[0]["FPLTotal"] = Function.ParseDouble(calc_FPLtotal.EditValue);
					}
					for (int i = 0; i < dtdoc.Rows.Count; i++)
					{
						dtdoc.Rows[i]["UserSign"] = GlobalData.UserID;
						dtdoc.Rows[i]["UserSign2"] = GlobalData.UserID;
						dtdoc.Rows[i]["UpdateDate"] = DateTime.Today;
					}
				}
				HttpResult httpResult = null;
				WinCommons._OpenProcessing();
				if (type == 1)
				{
					httpResult = await _httpClient.WorkPeriod.Add(dtdoc);
				}
				if (type == 2)
				{
					httpResult = await _httpClient.WorkPeriod.Update(dtdoc);
				}
				WinCommons._CloseProcessing();
				if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
				{
					if (SendDataBackToMain != null)
					{
						SendDataBackToMain();
					}
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					Close();
					return 0;
				}
				if (httpResult != null)
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	private async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		try
		{
			DataTable dataTable = await _httpClient.WorkPeriod.FindDataNextLast(Function.ToString(txt_Code.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				SetDataDoc(dataTable.Rows[0]);
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void CheckChangedData()
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			CheckChangedData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetChangedEvent(Control control)
	{
		try
		{
			if (control is BaseEdit)
			{
				((BaseEdit)control).EditValueChanged += Control_EditValueChanged;
				return;
			}
			if (control is GridControl)
			{
				BaseView defaultView = ((GridControl)control).DefaultView;
				if (defaultView is GridView)
				{
					((GridView)defaultView).CellValueChanged += Control_EditValueChanged;
				}
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				SetChangedEvent(control2);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitChangeDataEvent()
	{
		try
		{
			foreach (Control control in Controls)
			{
				SetChangedEvent(control);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetValueToDate(string subPeriod, int numOfPeriod, DateTime fromDate)
	{
		try
		{
			DateTime dateTime = default;
			if (!string.IsNullOrEmpty(subPeriod) && numOfPeriod > 0)
			{
				switch (subPeriod)
				{
				case "Y":
					dateTime = fromDate.AddYears(numOfPeriod).AddDays(-1.0);
					break;
				case "Q":
					dateTime = fromDate.AddMonths(numOfPeriod * 3).AddDays(-1.0);
					break;
				case "M":
					dateTime = fromDate.AddMonths(numOfPeriod).AddDays(-1.0);
					break;
				case "D":
					dateTime = fromDate.AddDays(numOfPeriod);
					break;
				}
				date_TD.EditValue = dateTime;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Find(bool isFind)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.WorkPeriod, false, Function.ToString(txt_Code.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.StatusPeriod = (isFind ? Function.ToString(look_Status.EditValue) : "");
			frm_CFL2.CodePeriod = (isFind ? Function.ToString(txt_Code.EditValue) : "");
			frm_CFL2.NamePeriod = (isFind ? Function.ToString(txt_Name.EditValue) : "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				SetDataDoc(frm_CFL2.ChooseItem);
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Remove(string code, int branch)
	{
		try
		{
			string text = await _httpClient.WorkPeriod.Remove(code, branch);
			if (string.IsNullOrWhiteSpace(text))
			{
				if (SendDataBackToMain != null)
				{
					SendDataBackToMain();
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				ResetFormControl();
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetList()
	{
		try
		{
			return await _httpClient.WorkPeriod.GetListWorkPeriod();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task<int> CheckDate(DateTime fromDate, DateTime toDate, string code)
	{
		try
		{
			DataTable dataTable = await _httpClient.WorkPeriod.GetByDate(fromDate, toDate, code);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return 1;
			}
			return 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void gv_UDF_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!isLoad && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_NewPeriod));
		this.txt_Year = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Num = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.look_Sub = new DevExpress.XtraEditors.LookUpEdit();
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.look_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_FD = new Apzon.Controls.Editor.DateEditEx();
		this.date_TD = new Apzon.Controls.Editor.DateEditEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.calc_WorkQuotation = new Apzon.Controls.Editor.CalcEditEx();
		this.lbl_hourperday = new Apzon.Controls.Controls.LabelLine();
		this.calc_HourPerDay = new Apzon.Controls.Editor.CalcEditEx();
		this.lbl_workQuotation = new Apzon.Controls.Controls.LabelLine();
		this.calc_ILtotal = new Apzon.Controls.Editor.CalcEditEx();
		this.calc_FPLtotal = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.check_IsLastPeriod = new Apzon.Controls.Editor.CheckEditEx();
		this.xTabUDF = new DevExpress.XtraTab.XtraTabPage();
		this.grd_UDF = new Apzon.Controls.Grid.GridControlEx();
		this.gv_UDF = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl1 = new DevExpress.XtraTab.XtraTabControl();
		((System.ComponentModel.ISupportInitialize)this.txt_Year.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Num.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Sub.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TD.Properties).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calc_WorkQuotation.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_HourPerDay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ILtotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_FPLtotal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_IsLastPeriod.Properties).BeginInit();
		this.xTabUDF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		base.SuspendLayout();
		this.txt_Year.EditControl = null;
		this.txt_Year.FieldName = null;
		this.txt_Year.IsSearchControl = false;
		this.txt_Year.Location = new System.Drawing.Point(86, 38);
		this.txt_Year.Name = "txt_Year";
		this.txt_Year.SettingInVisible = false;
		this.txt_Year.SettingReadonly = false;
		this.txt_Year.Size = new System.Drawing.Size(100, 20);
		this.txt_Year.TabIndex = 2;
		this.txt_Year.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(86, 64);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.Properties.ReadOnly = true;
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 3;
		this.txt_Code.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txt_Num.EditControl = null;
		this.txt_Num.FieldName = null;
		this.txt_Num.IsSearchControl = false;
		this.txt_Num.Location = new System.Drawing.Point(86, 162);
		this.txt_Num.Name = "txt_Num";
		this.txt_Num.SettingInVisible = false;
		this.txt_Num.SettingReadonly = false;
		this.txt_Num.Size = new System.Drawing.Size(100, 20);
		this.txt_Num.TabIndex = 6;
		this.txt_Num.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(86, 90);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(100, 20);
		this.txt_Name.TabIndex = 4;
		this.txt_Name.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.look_Sub.Location = new System.Drawing.Point(86, 136);
		this.look_Sub.Name = "look_Sub";
		this.look_Sub.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Sub.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Sub.Properties.DisplayMember = "Name";
		this.look_Sub.Properties.NullText = "";
		this.look_Sub.Properties.ValueMember = "Code";
		this.look_Sub.Size = new System.Drawing.Size(100, 20);
		this.look_Sub.TabIndex = 5;
		this.look_Sub.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.look_Status.Location = new System.Drawing.Point(86, 202);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Status.Properties.DisplayMember = "Name";
		this.look_Status.Properties.NullText = "";
		this.look_Status.Properties.ReadOnly = true;
		this.look_Status.Properties.ValueMember = "Code";
		this.look_Status.Size = new System.Drawing.Size(100, 20);
		this.look_Status.TabIndex = 7;
		this.look_Status.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.look_Branch.Location = new System.Drawing.Point(86, 12);
		this.look_Branch.Name = "look_Branch";
		this.look_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Branch.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Branch.Properties.DisplayMember = "Name";
		this.look_Branch.Properties.NullText = "";
		this.look_Branch.Properties.ValueMember = "Code";
		this.look_Branch.Size = new System.Drawing.Size(100, 20);
		this.look_Branch.TabIndex = 1;
		this.look_Branch.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(9, 517);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 12;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 517);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 13;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Period name";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(9, 93);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(80, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 0;
		this.labelLine9.TabStop = false;
		this.labelLine9.Utext = "Period name";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Branch";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(9, 15);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(80, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 0;
		this.labelLine8.TabStop = false;
		this.labelLine8.Utext = "Branch";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "To Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(9, 273);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(80, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 0;
		this.labelLine7.TabStop = false;
		this.labelLine7.Utext = "To Date";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "From Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(9, 247);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(80, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 0;
		this.labelLine6.TabStop = false;
		this.labelLine6.Utext = "From Date";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(9, 205);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 0;
		this.labelLine5.TabStop = false;
		this.labelLine5.Utext = "Status";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Num. of period";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(9, 165);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 0;
		this.labelLine4.TabStop = false;
		this.labelLine4.Utext = "Num. of period";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Period Code";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(9, 67);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 0;
		this.labelLine3.TabStop = false;
		this.labelLine3.Utext = "Period Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Type";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(9, 139);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.TabStop = false;
		this.labelLine2.Utext = "Type";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Year";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 41);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.TabStop = false;
		this.labelLine1.Utext = "Year";
		this.date_FD.EditControl = null;
		this.date_FD.EditValue = null;
		this.date_FD.FieldName = null;
		this.date_FD.Location = new System.Drawing.Point(86, 244);
		this.date_FD.Name = "date_FD";
		this.date_FD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FD.Properties.IsSearchControl = false;
		this.date_FD.SettingInVisible = false;
		this.date_FD.SettingReadonly = false;
		this.date_FD.Size = new System.Drawing.Size(100, 20);
		this.date_FD.TabIndex = 8;
		this.date_FD.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.date_TD.EditControl = null;
		this.date_TD.EditValue = null;
		this.date_TD.FieldName = null;
		this.date_TD.Location = new System.Drawing.Point(86, 270);
		this.date_TD.Name = "date_TD";
		this.date_TD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_TD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_TD.Properties.IsSearchControl = false;
		this.date_TD.Properties.ReadOnly = true;
		this.date_TD.SettingInVisible = false;
		this.date_TD.SettingReadonly = false;
		this.date_TD.Size = new System.Drawing.Size(100, 20);
		this.date_TD.TabIndex = 9;
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.calc_WorkQuotation.EditControl = null;
		this.calc_WorkQuotation.FieldName = null;
		this.calc_WorkQuotation.IsSearchControl = false;
		this.calc_WorkQuotation.Location = new System.Drawing.Point(393, 9);
		this.calc_WorkQuotation.Name = "calc_WorkQuotation";
		this.calc_WorkQuotation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_WorkQuotation.Properties.IsSearchControl = false;
		this.calc_WorkQuotation.SettingInVisible = false;
		this.calc_WorkQuotation.SettingReadonly = false;
		this.calc_WorkQuotation.Size = new System.Drawing.Size(161, 20);
		this.calc_WorkQuotation.TabIndex = 10;
		this.calc_WorkQuotation.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.lbl_hourperday.EditControl = null;
		this.lbl_hourperday.EditValue = "Hour Per Day";
		this.lbl_hourperday.FieldName = null;
		this.lbl_hourperday.HideWhenEmpty = false;
		this.lbl_hourperday.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_hourperday.Location = new System.Drawing.Point(272, 38);
		this.lbl_hourperday.Name = "lbl_hourperday";
		this.lbl_hourperday.ReadOnly = false;
		this.lbl_hourperday.SettingInVisible = false;
		this.lbl_hourperday.SettingReadonly = false;
		this.lbl_hourperday.ShowUnderLine = true;
		this.lbl_hourperday.Size = new System.Drawing.Size(126, 20);
		this.lbl_hourperday.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_hourperday.TabIndex = 0;
		this.lbl_hourperday.TabStop = false;
		this.lbl_hourperday.Utext = "Hour Per Day";
		this.calc_HourPerDay.EditControl = null;
		this.calc_HourPerDay.FieldName = null;
		this.calc_HourPerDay.IsSearchControl = false;
		this.calc_HourPerDay.Location = new System.Drawing.Point(393, 35);
		this.calc_HourPerDay.Name = "calc_HourPerDay";
		this.calc_HourPerDay.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_HourPerDay.Properties.IsSearchControl = false;
		this.calc_HourPerDay.SettingInVisible = false;
		this.calc_HourPerDay.SettingReadonly = false;
		this.calc_HourPerDay.Size = new System.Drawing.Size(161, 20);
		this.calc_HourPerDay.TabIndex = 11;
		this.calc_HourPerDay.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.lbl_workQuotation.EditControl = null;
		this.lbl_workQuotation.EditValue = "Work Quota";
		this.lbl_workQuotation.FieldName = null;
		this.lbl_workQuotation.HideWhenEmpty = false;
		this.lbl_workQuotation.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_workQuotation.Location = new System.Drawing.Point(272, 12);
		this.lbl_workQuotation.Name = "lbl_workQuotation";
		this.lbl_workQuotation.ReadOnly = false;
		this.lbl_workQuotation.SettingInVisible = false;
		this.lbl_workQuotation.SettingReadonly = false;
		this.lbl_workQuotation.ShowUnderLine = true;
		this.lbl_workQuotation.Size = new System.Drawing.Size(126, 20);
		this.lbl_workQuotation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_workQuotation.TabIndex = 0;
		this.lbl_workQuotation.TabStop = false;
		this.lbl_workQuotation.Utext = "Work Quota";
		this.calc_ILtotal.EditControl = null;
		this.calc_ILtotal.FieldName = null;
		this.calc_ILtotal.IsSearchControl = false;
		this.calc_ILtotal.Location = new System.Drawing.Point(393, 90);
		this.calc_ILtotal.Name = "calc_ILtotal";
		this.calc_ILtotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_ILtotal.Properties.IsSearchControl = false;
		this.calc_ILtotal.SettingInVisible = false;
		this.calc_ILtotal.SettingReadonly = false;
		this.calc_ILtotal.Size = new System.Drawing.Size(161, 20);
		this.calc_ILtotal.TabIndex = 17;
		this.calc_ILtotal.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.calc_FPLtotal.EditControl = null;
		this.calc_FPLtotal.FieldName = null;
		this.calc_FPLtotal.IsSearchControl = false;
		this.calc_FPLtotal.Location = new System.Drawing.Point(393, 64);
		this.calc_FPLtotal.Name = "calc_FPLtotal";
		this.calc_FPLtotal.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_FPLtotal.Properties.IsSearchControl = false;
		this.calc_FPLtotal.SettingInVisible = false;
		this.calc_FPLtotal.SettingReadonly = false;
		this.calc_FPLtotal.Size = new System.Drawing.Size(161, 20);
		this.calc_FPLtotal.TabIndex = 16;
		this.calc_FPLtotal.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Increase Leave Total";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(272, 93);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(126, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 14;
		this.labelLine10.TabStop = false;
		this.labelLine10.Utext = "Increase Leave Total";
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Reset Leave Total";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(272, 67);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(126, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 15;
		this.labelLine11.TabStop = false;
		this.labelLine11.Utext = "Reset Leave Total";
		this.labelLine12.EditControl = null;
		this.labelLine12.EditValue = "Is Last Period";
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(272, 119);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ReadOnly = false;
		this.labelLine12.SettingInVisible = false;
		this.labelLine12.SettingReadonly = false;
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(126, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 18;
		this.labelLine12.TabStop = false;
		this.labelLine12.Utext = "Is Last Period";
		this.check_IsLastPeriod.EditControl = null;
		this.check_IsLastPeriod.FieldName = null;
		this.check_IsLastPeriod.IsSearchControl = false;
		this.check_IsLastPeriod.Location = new System.Drawing.Point(393, 120);
		this.check_IsLastPeriod.Name = "check_IsLastPeriod";
		this.check_IsLastPeriod.Properties.Caption = "";
		this.check_IsLastPeriod.Properties.ValueChecked = "Y";
		this.check_IsLastPeriod.Properties.ValueGrayed = "N";
		this.check_IsLastPeriod.Properties.ValueUnchecked = "N";
		this.check_IsLastPeriod.SettingInVisible = false;
		this.check_IsLastPeriod.SettingReadonly = false;
		this.check_IsLastPeriod.Size = new System.Drawing.Size(75, 20);
		this.check_IsLastPeriod.TabIndex = 19;
		this.check_IsLastPeriod.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.xTabUDF.Controls.Add(this.grd_UDF);
		this.xTabUDF.Name = "xTabUDF";
		this.xTabUDF.Size = new System.Drawing.Size(580, 177);
		this.xTabUDF.Text = "User Defined Fields";
		this.grd_UDF.AllowPasteExcel = false;
		this.grd_UDF.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_UDF.ClipboardData = null;
		this.grd_UDF.Location = new System.Drawing.Point(-1, 0);
		this.grd_UDF.MainView = this.gv_UDF;
		this.grd_UDF.Name = "grd_UDF";
		this.grd_UDF.Size = new System.Drawing.Size(582, 178);
		this.grd_UDF.TabIndex = 0;
		this.grd_UDF.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_UDF });
		this.gv_UDF.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_UDF.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_UDF.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_UDF.ColumnKey = null;
		this.gv_UDF.DisableFormSetting = false;
		this.gv_UDF.DisplayException = false;
		this.gv_UDF.EditColumn = null;
		this.gv_UDF.FieldCurrency = null;
		this.gv_UDF.FieldRate = null;
		this.gv_UDF.FormId = null;
		this.gv_UDF.GridControl = this.grd_UDF;
		this.gv_UDF.HasFormSetting = false;
		this.gv_UDF.HeaderExcelAlign = Apzon.Commons.Enums.Enums.HeaderExcelAlign.Left;
		this.gv_UDF.HeaderExcelExport = null;
		this.gv_UDF.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_UDF.IgnoreFilter = false;
		this.gv_UDF.IndexEditRow = 0;
		this.gv_UDF.IndicatorWidth = 35;
		this.gv_UDF.IsApzTable = false;
		this.gv_UDF.IsInsertNewRow = false;
		this.gv_UDF.IsLoadingData = false;
		this.gv_UDF.IsMappingContextAction = false;
		this.gv_UDF.ItemId = null;
		this.gv_UDF.ItemId2 = null;
		this.gv_UDF.LstColumnAllowBatch = null;
		this.gv_UDF.LstColumnNotAllowBatch = null;
		this.gv_UDF.LstColumnNotEditable = null;
		this.gv_UDF.LstColumNotSetting = null;
		this.gv_UDF.LstOldUDF = null;
		this.gv_UDF.Name = "gv_UDF";
		this.gv_UDF.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_UDF.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_UDF.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_UDF.OptionsPrint.AutoWidth = false;
		this.gv_UDF.OptionsPrint.UsePrintStyles = false;
		this.gv_UDF.OptionsView.BestFitMaxRowCount = 100;
		this.gv_UDF.OptionsView.ColumnAutoWidth = false;
		this.gv_UDF.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_UDF.OptionsView.ShowGroupedColumns = true;
		this.gv_UDF.OptionsView.ShowGroupPanel = false;
		this.gv_UDF.ReadOnly = false;
		this.gv_UDF.SettingReadonly = false;
		this.gv_UDF.UseBatchMod = false;
		this.gv_UDF.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_UDF_CellValueChanged);
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.Location = new System.Drawing.Point(9, 309);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xTabUDF;
		this.xtraTabControl1.Size = new System.Drawing.Size(582, 202);
		this.xtraTabControl1.TabIndex = 20;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xTabUDF });
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(603, 547);
		this.ContextMenuStrip = this.contextMenuStrip1;
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.check_IsLastPeriod);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.calc_ILtotal);
		base.Controls.Add(this.calc_FPLtotal);
		base.Controls.Add(this.labelLine10);
		base.Controls.Add(this.labelLine11);
		base.Controls.Add(this.calc_HourPerDay);
		base.Controls.Add(this.calc_WorkQuotation);
		base.Controls.Add(this.lbl_hourperday);
		base.Controls.Add(this.lbl_workQuotation);
		base.Controls.Add(this.date_TD);
		base.Controls.Add(this.date_FD);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.look_Branch);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.look_Sub);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.labelLine9);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_Num);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.txt_Year);
		base.Controls.Add(this.labelLine1);
		base.FormId = "700022";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_NewPeriod.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_NewPeriod";
		this.Text = "Work Period";
		((System.ComponentModel.ISupportInitialize)this.txt_Year.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Num.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Sub.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TD.Properties).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calc_WorkQuotation.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_HourPerDay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ILtotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_FPLtotal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_IsLastPeriod.Properties).EndInit();
		this.xTabUDF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
