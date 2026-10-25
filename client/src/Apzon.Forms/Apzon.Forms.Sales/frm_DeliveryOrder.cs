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
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;

namespace Apzon.Forms.Sales;

public class frm_DeliveryOrder : XtraFormEx
{
	private const string FormController = "DeliveryControl";

	public DataSet DsControl = new DataSet();

	public new IHttpClient HttpClient = HttpClientHelperFactory.GetHttpClient();

	public string UdoCode = "ODLO";

	public DataTable UdoTable = new DataTable();

	private List<UserFieldsImpl> _lstUdf;

	private GridHitInfo downHitInfo;

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private TextEditEx txt_DocNum;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LookUpEdit look_Status;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LabelLine labelLine8;

	private DateEdit date_Confirm;

	private XtraTabControlEx tabControl_Delivery;

	private XtraTabPageEx tab_Schedule;

	private LookUpEdit look_Employee;

	private LookUpEdit look_Bsx;

	private ContextMenuStrip ctx_Main;

	private ToolStripMenuItem menuMainClose;

	private ToolStripMenuItem menuMainCancel;

	private PanelControl panelControl1;

	private XtraTabPageEx tab_Invoices;

	private SplitContainerControl splitContainerControl1;

	private TextEditEx txt_DocumentBarcode;

	private LabelLine lbl_Barcode;

	private GridControlEx grd_Invoice;

	private GridViewEx gv_Invoice;

	private XtraTabPageEx tab_Batch;

	private GridControlEx grd_Batch;

	private GridViewEx gv_Batch;

	private SimpleButtonEx btn_Print;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Clear;

	private TextEditEx txt_BatchNo;

	private LabelLine lbl_BatchNo;

	private SimpleButtonEx btn_SelectBatch;

	private TextEditEx txt_Remark;

	private LabelLine labelLine9;

	private DateEdit date_Del;

	private LookUpEdit look_Employee2;

	private LookUpEdit look_Employee3;

	private LabelLine labelLine10;

	private LabelLine labelLine11;

	private LookUpEdit look_Target;

	private LabelLine labelLine7;

	private TextEditEx txt_Confirm;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Schedule;

	private GridViewEx gv_Schedule;

	protected override void SetFormMode(Enums.FormViewModeType value)
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
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonFind;
			break;
		}
	}

	public frm_DeliveryOrder()
	{
		InitializeComponent();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int type = 3;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			int type = 1;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			int type = 2;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			int type = 4;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			txt_DocNum.Properties.ReadOnly = false;
			FormMode = Enums.FormViewModeType.Find;
			date_Del.EditValue = null;
			date_Confirm.EditValue = null;
			grd_Batch.DataSource = new DataTable();
			grd_Invoice.DataSource = new DataTable();
			txt_DocumentBarcode.Text = "";
			txt_DocNum.Text = "";
			txt_Remark.Text = "";
			look_Status.EditValue = null;
			look_Employee.EditValue = null;
			look_Employee2.EditValue = null;
			look_Employee3.EditValue = null;
			look_Bsx.EditValue = null;
			txt_DocNum.BackColor = Color.FromArgb(250, 221, 122);
			look_Employee.BackColor = Color.FromArgb(250, 221, 122);
			look_Bsx.BackColor = Color.FromArgb(250, 221, 122);
			look_Status.BackColor = Color.FromArgb(250, 221, 122);
			date_Del.BackColor = Color.FromArgb(250, 221, 122);
			txt_BatchNo.BackColor = Color.FromArgb(250, 221, 122);
			txt_DocumentBarcode.BackColor = Color.FromArgb(250, 221, 122);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			int type = 5;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int currentId, int type)
	{
		WinCommons._OpenProcessing();
		try
		{
			int num = ((type != 5) ? (await GetPage(currentId, type, GlobalData.UserName, GlobalData.DefaultBranch)) : currentId);
			int id = num;
			await GetById(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetPage(int currentId, int type, string username, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryControl");
			return await helper.GetIntAsync("GetPage", new { currentId, type, username, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async void txt_DocNum_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_DocNum.Text) && FormMode == Enums.FormViewModeType.Find)
			{
				int type = 5;
				int num = Function.ParseInt(txt_DocNum.EditValue);
				if (num > 0)
				{
					await Navigate(num, type);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_DeliveryOrder_Load(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			gv_Batch.DisableFormSetting = true;
			gv_Invoice.DisableFormSetting = true;
			gv_Schedule.DisableFormSetting = true;
			gv_Schedule.OptionsCustomization.AllowSort = false;
			grd_Schedule.AllowDrop = true;
			date_Del.EditValue = DateTime.Now;
			date_Del.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			date_Del.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat + " HH:mm";
			date_Del.Properties.EditFormat.FormatType = FormatType.DateTime;
			date_Del.Properties.EditFormat.FormatString = GlobalData.DateTimeFormat + " HH:mm";
			date_Del.Properties.EditMask = GlobalData.DateTimeFormat + " HH:mm";
			date_Confirm.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			date_Confirm.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat + " HH:mm";
			date_Confirm.Properties.EditFormat.FormatType = FormatType.DateTime;
			date_Confirm.Properties.EditFormat.FormatString = GlobalData.DateTimeFormat + " HH:mm";
			date_Confirm.Properties.EditMask = GlobalData.DateTimeFormat + " HH:mm";
			LoadStatus();
			await GetListShipers();
			await GetListBranchs();
			await LoadLicenseCar();
			_lstUdf = await HttpClient.Udf.GetListField("OBTN");
			UdoTable = await HttpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "ODLO"
			});
			FormMode = Enums.FormViewModeType.View;
			look_Status.EditValue = "O";
			await LoadSchedule(0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadStatus()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "O";
			dataRow["Name"] = "Open";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "L";
			dataRow["Name"] = "Closed";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "C";
			dataRow["Name"] = "Canceled";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "P";
			dataRow["Name"] = "Printed";
			dataTable.Rows.Add(dataRow);
			look_Status.Properties.DataSource = dataTable;
			look_Status.Properties.DisplayMember = "Name";
			look_Status.Properties.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLicenseCar()
	{
		try
		{
			DataTable dataSource = await HttpClient.Commons.GetLicenseCar(GlobalData.UserName, GlobalData.DefaultBranch);
			look_Bsx.Properties.DataSource = dataSource;
			look_Bsx.Properties.DisplayMember = "Code";
			look_Bsx.Properties.ValueMember = "BienSo";
			look_Bsx.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetListShipers()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryControl");
			DataTable dataTable = await helper.GetAsync("GetListShipers", new
			{
				username = GlobalData.UserName,
				branch = GlobalData.DefaultBranch
			});
			look_Employee.Properties.DataSource = dataTable;
			look_Employee.Properties.DisplayMember = "FullName";
			look_Employee.Properties.ValueMember = "empID";
			look_Employee.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			look_Employee2.Properties.DataSource = dataTable.Copy();
			look_Employee2.Properties.DisplayMember = "FullName";
			look_Employee2.Properties.ValueMember = "empID";
			look_Employee2.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			look_Employee3.Properties.DataSource = dataTable.Copy();
			look_Employee3.Properties.DisplayMember = "FullName";
			look_Employee3.Properties.ValueMember = "empID";
			look_Employee3.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetListBranchs()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryControl");
			DataTable dataSource = await helper.GetAsync("GetListBranchs", new
			{
				username = GlobalData.UserName,
				branch = GlobalData.DefaultBranch
			});
			look_Target.Properties.DataSource = dataSource;
			look_Target.Properties.DisplayMember = "Name";
			look_Target.Properties.ValueMember = "Value";
			look_Target.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			look_Target.ItemIndex = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Find)
			{
				if (!string.IsNullOrEmpty(txt_DocNum.Text))
				{
					int type = 5;
					int currentId = Function.ParseInt(txt_DocNum.EditValue);
					await Navigate(currentId, type);
				}
				else
				{
					await SearchDeliveryOrder();
				}
			}
			else
			{
				if (!grd_Batch.DataSourceTable.IsNotNull() || (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update))
				{
					return;
				}
				DataRow[] array = grd_Batch.DataSourceTable.Select("Checked='True'");
				if (array != null && array.Length == 0)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoBranchNotSelect, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				}
				else
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
					{
						return;
					}
					DataTable dataTable = array.CopyToDataTable();
					WinCommons._OpenProcessing();
					if (UdoTable == null || UdoTable.Rows.Count == 0)
					{
						WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
						return;
					}
					string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
					DataSet ds = new DataSet
					{
						Tables = { UdoTable.Copy() }
					};
					DataTable dataTable2 = new DataTable(tableName);
					dataTable2.Columns.Add("DocEntry", typeof(int));
					dataTable2.Columns.Add("Remark", typeof(string));
					dataTable2.Columns.Add("U_Deliver", typeof(string));
					dataTable2.Columns.Add("U_Deliver2", typeof(string));
					dataTable2.Columns.Add("U_Deliver3", typeof(string));
					dataTable2.Columns.Add("U_Tel", typeof(string));
					dataTable2.Columns.Add("U_CarLicense", typeof(string));
					dataTable2.Columns.Add("U_DelDate", typeof(DateTime));
					dataTable2.Columns.Add("U_StartTime", typeof(string));
					dataTable2.Columns.Add("U_Confirmed", typeof(string));
					dataTable2.Columns.Add("U_ConfirmDate", typeof(DateTime));
					dataTable2.Columns.Add("U_ConfirmTime", typeof(string));
					dataTable2.Columns.Add("U_Target", typeof(string));
					dataTable2.Columns.Add("U_UserSign2", typeof(string));
					DataRow dataRow = dataTable2.NewRow();
					dataRow["DocEntry"] = ((!string.IsNullOrEmpty(txt_DocNum.Text)) ? Function.ParseInt(txt_DocNum.Text) : 0);
					dataRow["Remark"] = Function.ToString(txt_Remark.EditValue);
					dataRow["U_Deliver"] = Function.ToString(look_Employee.EditValue);
					dataRow["U_Deliver2"] = Function.ToString(look_Employee2.EditValue);
					dataRow["U_Deliver3"] = Function.ToString(look_Employee3.EditValue);
					dataRow["U_CarLicense"] = Function.ToString(look_Bsx.EditValue);
					dataRow["U_Target"] = Function.ToString(look_Target.EditValue);
					dataRow["U_Confirmed"] = Function.ToString(txt_Confirm.EditValue);
					if (Function.ParseDateTime(date_Del.EditValue).HasValue)
					{
						dataRow["U_DelDate"] = Convert.ToDateTime(date_Del.EditValue);
						dataRow["U_StartTime"] = Convert.ToDateTime(date_Del.EditValue).ToString("HH:mm");
					}
					if (Function.ParseDateTime(date_Confirm.EditValue).HasValue)
					{
						dataRow["U_ConfirmDate"] = Convert.ToDateTime(date_Confirm.EditValue);
						dataRow["U_ConfirmTime"] = Convert.ToDateTime(date_Confirm.EditValue).ToString("HH:mm");
					}
					dataTable2.Rows.Add(dataRow);
					ds.Tables.Add(dataTable2);
					DataTable dataTable3 = dataTable.Copy();
					dataTable3.TableName = "DLO1";
					foreach (string item in (from DataColumn t in dataTable3.Columns
						where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList())
					{
						dataTable3.Columns.Remove(item);
					}
					ds.Tables.Add(dataTable3.Copy());
					DataTable dataTable4 = grd_Invoice.DataSourceTable.Copy();
					dataTable4.TableName = "DLO2";
					foreach (string item2 in (from DataColumn t in dataTable4.Columns
						where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList())
					{
						dataTable4.Columns.Remove(item2);
					}
					ds.Tables.Add(dataTable4);
					DataTable dataTable5 = grd_Schedule.DataSourceTable.Copy();
					dataTable5.TableName = "DLO3";
					foreach (string item3 in (from DataColumn t in dataTable5.Columns
						where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
						select t.ColumnName).ToList())
					{
						dataTable5.Columns.Remove(item3);
					}
					if (dataTable5.IsNotNull() && dataTable5.IsExistsColumn("U_DelOrder"))
					{
						for (int num = 0; num < dataTable5.Rows.Count; num++)
						{
							dataTable5.Rows[num]["U_DelOrder"] = num;
						}
					}
					ds.Tables.Add(dataTable5);
					HttpResult httpResult = await HttpClient.Udo.Save(ds);
					WinCommons._CloseProcessing();
					ds.Tables.Clear();
					ds.Dispose();
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
						FormMode = Enums.FormViewModeType.View;
						if (httpResult.Content != null)
						{
							txt_DocNum.EditValue = httpResult.Content;
						}
						txt_DocNum.Properties.ReadOnly = true;
						FormMode = Enums.FormViewModeType.View;
						await PRINTLAYOUT();
						await REFRESH();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_Employee_EditValueChanged(object sender, EventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetById(int id)
	{
		WinCommons._OpenProcessing();
		try
		{
			await REFRESH();
			DataTable dataTable;
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryControl"))
			{
				dataTable = await helper.GetAsync("GetDoc", new { id });
			}
			if (dataTable.IsNotNull())
			{
				txt_DocNum.EditValue = id;
				look_Status.EditValue = Function.ToString(dataTable.Rows[0]["Status"]);
				txt_Confirm.EditValue = Function.ToString(dataTable.Rows[0]["U_Confirmed"]);
				txt_Remark.EditValue = Function.ToString(dataTable.Rows[0]["Remark"]);
				look_Employee.EditValue = Function.ToString(dataTable.Rows[0]["U_Deliver"]);
				look_Employee2.EditValue = Function.ToString(dataTable.Rows[0]["U_Deliver2"]);
				look_Employee3.EditValue = Function.ToString(dataTable.Rows[0]["U_Deliver3"]);
				look_Bsx.EditValue = dataTable.Rows[0]["U_CarLicense"];
				look_Target.EditValue = dataTable.Rows[0]["U_Target"];
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["U_ConfirmDate"])))
				{
					date_Confirm.EditValue = Function.ParseDateTime(dataTable.Rows[0]["U_ConfirmDate"]);
				}
				date_Del.EditValue = SetDatetime(dataTable.Rows[0]["U_DelDate"], dataTable.Rows[0]["U_StartTime"]);
				date_Confirm.EditValue = SetDatetime(dataTable.Rows[0]["U_ConfirmDate"], dataTable.Rows[0]["U_ConfirmTime"]);
			}
			await LoadItems(id);
			await LoadInvoices(id);
			await LoadSchedule(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadItems(int id)
	{
		_ = 1;
		try
		{
			DataSet ds;
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl"))
			{
				ds = await helper.GetAsync("LoadItems", new { id });
			}
			await GridViewHelper.GenerateGrid(grd_Batch, gv_Batch, ds, _lstUdf);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInvoices(int id)
	{
		_ = 1;
		try
		{
			if (id > 0)
			{
				DataSet ds;
				using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl"))
				{
					ds = await helper.GetAsync("LoadInvoices", new { id });
				}
				await GridViewHelper.GenerateGrid(grd_Invoice, gv_Invoice, ds);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSchedule(int id)
	{
		_ = 1;
		try
		{
			DataSet ds;
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl"))
			{
				ds = await helper.GetAsync("LoadSchedule", new { id });
			}
			await GridViewHelper.GenerateGrid(grd_Schedule, gv_Schedule, ds, _lstUdf);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DateTime? SetDatetime(object date, object time)
	{
		string text = Function.ToString(time);
		double value = 0.0;
		double value2 = 0.0;
		if (!string.IsNullOrEmpty(text))
		{
			if (text.Length < 3)
			{
				value2 = Function.ParseDouble(text);
				value = 0.0;
			}
			else
			{
				value2 = Function.ParseDouble(text.Substring(text.Length - 2, 2));
				value = Function.ParseDouble((text.Length < 4) ? text.Substring(0, 1) : text.Substring(0, 2));
			}
		}
		if (!string.IsNullOrEmpty(Function.ToString(date)))
		{
			return Function.ParseDateTime(date).Value.AddHours(value).AddMinutes(value2);
		}
		return null;
	}

	public async Task PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNum.Text);
			await WinFuntions.PrintLayout("", num, UdoCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		try
		{
			int docEntry = Function.ParseInt(txt_DocNum.Text);
			await WinFuntions.Print("", docEntry, UdoCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_Del_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_Del.EditValue != null && FormMode != Enums.FormViewModeType.Find)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void look_Bsx_EditValueChanged(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void menuMainRemove_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNum.Text) || Function.ParseInt(txt_DocNum.Text) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNum.Text);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "DeleteDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void menuMainClose_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNum.Text) || Function.ParseInt(txt_DocNum.Text) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNum.Text);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "CloseDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.CloseSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void menuMainCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNum.Text) || Function.ParseInt(txt_DocNum.Text) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNum.Text);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancel, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "CancelDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.CancelSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> OperatorDoc(DataSet ds, string command)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "UDO");
			return await _help.PostAsJsonAsync(command, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private async void txt_DocEntry_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 3;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(Function.ToString(txt_DocumentBarcode.Text)))
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				await SearchDeliveryOrder();
				return;
			}
			tabControl_Delivery.SelectedTabPage = tab_Batch;
			WinCommons._OpenProcessing();
			DataSet ds = await GetBatchDelivery(txt_DocumentBarcode.Text);
			if (ds != null && ds.Tables.Count == 1)
			{
				WinCommons.Error(Enums.MessageCode.None, Function.ToString(ds.Tables[0].Rows[0][0]));
			}
			else if (ds.IsHasStructure() && ds.Tables[1].IsNotNull())
			{
				bool flag = false;
				if (ds.Tables[0].IsNotNull() && (from r in ds.Tables[0].AsEnumerable()
					where r.Field<string>("ColumnName") == "U_BatchNo"
					select r).ToList().Count > 0)
				{
					flag = true;
				}
				if (flag)
				{
					tabControl_Delivery.SelectedTabPage = tab_Batch;
					DataTable dataSourceTable = grd_Batch.DataSourceTable;
					if (!dataSourceTable.IsNotNull())
					{
						await GridViewHelper.GenerateGrid(grd_Batch, gv_Batch, ds, _lstUdf);
					}
					else
					{
						int num = Function.ParseInt(ds.Tables[1].Rows[0]["U_ID"]);
						int num2 = Function.ParseInt(ds.Tables[1].Rows[0]["U_DelNo"]);
						if (dataSourceTable.Select($"U_ID = {num} AND U_DelNo = {num2}").Length != 0)
						{
							WinCommons.Error(Enums.MessageCode.None, "Phiếu đã có trong danh sách!");
							txt_DocumentBarcode.Text = "";
							txt_DocumentBarcode.Focus();
							return;
						}
						foreach (DataRow row in ds.Tables[1].Rows)
						{
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
						}
					}
				}
				else
				{
					tabControl_Delivery.SelectedTabPage = tab_Invoices;
					DataTable dataSourceTable2 = grd_Invoice.DataSourceTable;
					if (!dataSourceTable2.IsNotNull())
					{
						await GridViewHelper.GenerateGrid(grd_Invoice, gv_Invoice, ds, _lstUdf);
					}
					else
					{
						int num3 = Function.ParseInt(ds.Tables[1].Rows[0]["U_DocType"]);
						int num4 = Function.ParseInt(ds.Tables[1].Rows[0]["U_DocEntry"]);
						if (dataSourceTable2.Select($"U_DocType = {num3} AND U_DocEntry = {num4}").Length != 0)
						{
							WinCommons.Error(Enums.MessageCode.None, "Phiếu đã có trong danh sách!");
							txt_DocumentBarcode.Text = "";
							txt_DocumentBarcode.Focus();
							return;
						}
						foreach (DataRow row2 in ds.Tables[1].Rows)
						{
							DataRow dataRow4 = dataSourceTable2.NewRow();
							dataRow4.ItemArray = row2.ItemArray;
							dataSourceTable2.Rows.Add(dataRow4);
						}
					}
				}
				FormMode = Enums.FormViewModeType.Add;
				ds.Tables.Clear();
				ds.Dispose();
			}
			else
			{
				if (ds.IsHasStructure())
				{
					ds.Tables[1].IsNotNull();
				}
				WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy dữ liệu phù hợp!");
			}
			txt_DocumentBarcode.Text = "";
			txt_DocumentBarcode.Focus();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			WinCommons._CloseProcessing();
		}
	}

	public DataTable GetBatchDetail(string batch)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DeliveryConfirm");
			return httpClientHelper.Get("GetBatchDetail", new { batch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void btn_Clear_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private void gv_Batch_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Remove Row";
			dXMenuItem.Click += docMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void docMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(sender as DXMenuItem).Caption.Equals("Remove Row") || gv_Batch.FocusedRowHandle <= -1 || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataSourceTable = grd_Batch.DataSourceTable;
			DataRow[] array = dataSourceTable.Select("U_DelNo =" + Function.ParseInt(gv_Batch.GetFocusedRowCellValue("U_DelNo")));
			if (array.Length != 0)
			{
				DataRow[] array2 = array;
				foreach (DataRow row in array2)
				{
					dataSourceTable.Rows.Remove(row);
				}
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetBatchDelivery(string docentry)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl");
			return await helper.GetAsync("GetBatchDelivery", new
			{
				docentry = docentry,
				userName = GlobalData.UserName,
				branch = GlobalData.DefaultBranch
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	public async Task REFRESH()
	{
		try
		{
			date_Del.EditValue = DateTime.Now;
			date_Confirm.EditValue = "";
			grd_Batch.DataSource = new DataTable();
			grd_Batch.RefreshDataSource();
			grd_Invoice.DataSource = new DataTable();
			grd_Invoice.RefreshDataSource();
			grd_Schedule.DataSource = new DataTable();
			grd_Schedule.RefreshDataSource();
			look_Status.EditValue = "O";
			look_Target.ItemIndex = 0;
			look_Employee.EditValue = null;
			look_Employee2.EditValue = null;
			look_Employee3.EditValue = null;
			look_Bsx.EditValue = null;
			txt_DocNum.Text = "";
			txt_Confirm.Text = "";
			lbl_BatchNo.Visible = false;
			txt_BatchNo.Visible = false;
			txt_DocNum.ResetBackColor();
			look_Employee.ResetBackColor();
			look_Bsx.ResetBackColor();
			look_Status.ResetBackColor();
			date_Del.ResetBackColor();
			txt_BatchNo.ResetBackColor();
			txt_DocumentBarcode.ResetBackColor();
			txt_DocumentBarcode.Text = "";
			txt_DocumentBarcode.Focus();
			txt_Remark.Text = "";
			tabControl_Delivery.SelectedTabPageIndex = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SearchDeliveryOrder()
	{
		_ = 1;
		try
		{
			int currentId = 0;
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = Function.ParseInt(txt_DocNum.Text);
			documentsImpl.CardCode = Function.ToString(look_Employee.EditValue);
			documentsImpl.CardName = Function.ToString(look_Bsx.EditValue);
			documentsImpl.DocStatus = Function.ToString(look_Status.EditValue);
			documentsImpl.Ref1 = txt_BatchNo.Text;
			documentsImpl.Ref2 = txt_DocumentBarcode.Text;
			documentsImpl.UserName = GlobalData.UserName;
			documentsImpl.Branch = GlobalData.DefaultBranch;
			if (date_Del.EditValue != null)
			{
				documentsImpl.CreateDate = Function.ParseDateTimes(date_Del.EditValue);
			}
			if (date_Confirm.EditValue != null)
			{
				documentsImpl.DocDate = Function.ParseDateTimes(date_Confirm.EditValue);
			}
			DataSet dataSet;
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "DeliveryControl"))
			{
				dataSet = await helper.PostAsync("SearchDeliveryOrder", documentsImpl);
			}
			if (!dataSet.IsHasStructure() || !dataSet.Tables[1].IsNotNull())
			{
				return;
			}
			if (dataSet.Tables[1].Rows.Count > 1)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedData, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.Text = "List of Delivery Orders";
				frm_CFL2.DataSource = dataSet;
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null && chooseItem.IsExistsColumn("DocEntry"))
				{
					currentId = Function.ParseInt(chooseItem["DocEntry"]);
				}
			}
			else
			{
				currentId = Function.ParseInt(dataSet.Tables[1].Rows[0]["DocEntry"]);
			}
			if (currentId > 0)
			{
				await Navigate(currentId, 5);
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SelectBatch_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Batch.Columns.Count <= 0)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grd_Batch.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				foreach (DataRow row in dataSourceTable.Rows)
				{
					row["Checked"] = false;
				}
				gv_Batch.Columns["Checked"].Visible = true;
				lbl_BatchNo.Visible = true;
				txt_BatchNo.Visible = true;
				txt_BatchNo.Focus();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_BatchNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_BatchNo.Text))
			{
				return;
			}
			WinCommons._OpenProcessing();
			tabControl_Delivery.SelectedTabPage = tab_Batch;
			if (grd_Batch.DataSource != null)
			{
				string text = txt_BatchNo.Text;
				DataTable dataTable = grd_Batch.DataSource as DataTable;
				if (dataTable.ExistsColumn("U_BatchNo") && dataTable.ExistsColumn("U_InvNo"))
				{
					DataRow[] array = dataTable.Select($"U_BatchNo='{text}' AND U_DocEntry>0 AND U_ItemCode IS NOT NULL");
					if (array.Length == 0)
					{
						string arg = ((Function.ParseInt(text) > 0) ? text : "-1");
						array = dataTable.Select($"U_InvNo='{arg}'");
						if (array.Length == 0)
						{
							WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy dữ liệu phù hợp!");
							txt_BatchNo.Text = string.Empty;
							txt_BatchNo.Focus();
							return;
						}
					}
					DataRow[] array2 = array;
					foreach (DataRow dataRow in array2)
					{
						if (!Function.ParseBool(dataRow["Checked"]))
						{
							dataRow["Checked"] = "True";
							gv_Batch.FocusedRowHandle = dataTable.Rows.IndexOf(dataRow);
						}
					}
				}
			}
			txt_BatchNo.Text = string.Empty;
			txt_BatchNo.Focus();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Invoice_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Remove Row";
			dXMenuItem.Click += invoiceMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void invoiceMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if ((sender as DXMenuItem).Caption.Equals("Remove Row") && gv_Invoice.FocusedRowHandle > -1 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				gv_Invoice.DeleteRow(gv_Invoice.FocusedRowHandle);
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void tabControl_Delivery_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			btn_Print.Visible = tabControl_Delivery.SelectedTabPage == tab_Batch;
			btn_SelectBatch.Visible = tabControl_Delivery.SelectedTabPage == tab_Batch;
			if (tabControl_Delivery.SelectedTabPage != tab_Schedule)
			{
				return;
			}
			DataTable dataTable = grd_Batch.DatasourceTable();
			DataTable dataTable2 = grd_Schedule.DatasourceTable();
			if (!dataTable.IsNotNull() || dataTable2.Columns.Count == 0)
			{
				return;
			}
			var enumerable = from t in dataTable.AsEnumerable()
				group t by new
				{
					line = Function.ToString(t["U_CardCode"])
				} into g
				select new
				{
					Key = g.Key,
					Items = g
				};
			int num = 0;
			foreach (var item in enumerable)
			{
				int num2 = 0;
				foreach (DataRow item2 in item.Items)
				{
					DataRow dataRow = dataTable2.NewRow();
					string cardcode = Function.ToString(item2["U_CardCode"]);
					if (!dataTable2.AsEnumerable().Any((DataRow m) => m.Field<string>("U_CardCode").Equals(cardcode)))
					{
						dataRow["U_CardCode"] = item2["U_CardCode"];
						dataRow["U_OldCardCode"] = item2["OldCardCode"];
						dataRow["U_ScheduledDate"] = DateTime.Today;
						dataRow["U_ShipToCode"] = item2["ShipToCode"];
						dataRow["U_Address2"] = item2["Address2"];
						dataTable2.Rows.Add(dataRow);
					}
					num2++;
				}
				num++;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Schedule_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			if (!(sender is GridControlEx gridControlEx))
			{
				return;
			}
			GridViewEx gridViewEx = gridControlEx.MainView as GridViewEx;
			GridHitInfo gridHitInfo = e.Data.GetData(typeof(GridHitInfo)) as GridHitInfo;
			if (gridViewEx != null)
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

	private void MoveRow(int sourceRow, int targetRow)
	{
		try
		{
			if (sourceRow != targetRow)
			{
				DataTable dataTable = grd_Schedule.DatasourceTable();
				DataRow dataRow = dataTable.Rows[sourceRow];
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2.ItemArray = dataRow.ItemArray;
				dataTable.Rows.Remove(dataRow);
				dataTable.Rows.InsertAt(dataRow2, targetRow);
				dataTable.AcceptChanges();
				gv_Schedule.FocusedRowHandle = targetRow;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Schedule_DragOver(object sender, DragEventArgs e)
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

	private void gv_Schedule_MouseDown(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			downHitInfo = null;
			if (gridViewEx != null)
			{
				GridHitInfo gridHitInfo = gridViewEx.CalcHitInfo(new Point(e.X, e.Y));
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

	private void gv_Schedule_MouseMove(object sender, MouseEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (e.Button == MouseButtons.Left && downHitInfo != null)
			{
				Size dragSize = SystemInformation.DragSize;
				Rectangle rectangle = new Rectangle(new Point(downHitInfo.HitPoint.X - dragSize.Width / 2, downHitInfo.HitPoint.Y - dragSize.Height / 2), dragSize);
				if (!rectangle.Contains(new Point(e.X, e.Y)))
				{
					gridViewEx?.GridControl.DoDragDrop(downHitInfo, DragDropEffects.All);
					downHitInfo = null;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_DeliveryOrder));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNum = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.date_Confirm = new DevExpress.XtraEditors.DateEdit();
		this.tabControl_Delivery = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Batch = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Invoices = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Invoice = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Invoice = new Apzon.Controls.Grid.GridViewEx();
		this.tab_Schedule = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Schedule = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Schedule = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Print = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SelectBatch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.look_Employee = new DevExpress.XtraEditors.LookUpEdit();
		this.look_Bsx = new DevExpress.XtraEditors.LookUpEdit();
		this.ctx_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuMainClose = new System.Windows.Forms.ToolStripMenuItem();
		this.menuMainCancel = new System.Windows.Forms.ToolStripMenuItem();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_Confirm = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.look_Target = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.look_Employee3 = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.look_Employee2 = new DevExpress.XtraEditors.LookUpEdit();
		this.date_Del = new DevExpress.XtraEditors.DateEdit();
		this.txt_Remark = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BatchNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_BatchNo = new Apzon.Controls.Controls.LabelLine();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_DocumentBarcode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Barcode = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Confirm.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Confirm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Delivery).BeginInit();
		this.tabControl_Delivery.SuspendLayout();
		this.tab_Batch.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		this.tab_Invoices.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Invoice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Invoice).BeginInit();
		this.tab_Schedule.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Schedule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Schedule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Employee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Bsx.Properties).BeginInit();
		this.ctx_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Confirm.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Target.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Employee3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Employee2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Del.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Del.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocumentBarcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Doc. Num";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(102, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Doc. Num";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Tình trạng";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(720, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(102, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Tình trạng";
		this.txt_DocNum.EditControl = null;
		this.txt_DocNum.FieldName = null;
		this.txt_DocNum.IsSearchControl = false;
		this.txt_DocNum.Location = new System.Drawing.Point(107, 5);
		this.txt_DocNum.Name = "txt_DocNum";
		this.txt_DocNum.Properties.ReadOnly = true;
		this.txt_DocNum.SettingInVisible = false;
		this.txt_DocNum.SettingReadonly = false;
		this.txt_DocNum.Size = new System.Drawing.Size(145, 20);
		this.txt_DocNum.TabIndex = 4;
		this.txt_DocNum.ToolTip = "Số phiếu";
		this.txt_DocNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNum_KeyDown);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Người giao hàng";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 34);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(102, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 5;
		this.labelLine3.Utext = "Người giao hàng";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Người giao hàng 2";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(8, 60);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(102, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 7;
		this.labelLine4.Utext = "Người giao hàng 2";
		this.look_Status.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.look_Status.Location = new System.Drawing.Point(820, 5);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.NullText = "";
		this.look_Status.Size = new System.Drawing.Size(145, 20);
		this.look_Status.TabIndex = 3;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Biển số xe";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(274, 7);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(82, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 9;
		this.labelLine5.Utext = "Biển số xe";
		this.labelLine6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Ngày giao";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(721, 34);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(102, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 11;
		this.labelLine6.Utext = "Ngày giao";
		this.labelLine8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Giờ confirm";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(721, 60);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(102, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 15;
		this.labelLine8.Utext = "Giờ confirm";
		this.date_Confirm.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Confirm.EditValue = null;
		this.date_Confirm.Location = new System.Drawing.Point(820, 57);
		this.date_Confirm.Name = "date_Confirm";
		this.date_Confirm.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Confirm.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Confirm.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_Confirm.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Confirm.Properties.EditFormat.FormatString = "HH:mm";
		this.date_Confirm.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Confirm.Properties.Mask.EditMask = "HH:mm";
		this.date_Confirm.Properties.ReadOnly = true;
		this.date_Confirm.Size = new System.Drawing.Size(145, 20);
		this.date_Confirm.TabIndex = 16;
		this.tabControl_Delivery.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl_Delivery.IsSearchControl = false;
		this.tabControl_Delivery.Location = new System.Drawing.Point(0, 0);
		this.tabControl_Delivery.Name = "tabControl_Delivery";
		this.tabControl_Delivery.SelectedTabPage = this.tab_Batch;
		this.tabControl_Delivery.Size = new System.Drawing.Size(973, 491);
		this.tabControl_Delivery.TabIndex = 17;
		this.tabControl_Delivery.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tab_Batch, this.tab_Invoices, this.tab_Schedule });
		this.tabControl_Delivery.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabControl_Delivery_SelectedPageChanged);
		this.tab_Batch.Controls.Add(this.grd_Batch);
		this.tab_Batch.EditControl = null;
		this.tab_Batch.EditValue = "Batch";
		this.tab_Batch.FieldName = null;
		this.tab_Batch.IsSearchControl = false;
		this.tab_Batch.Name = "tab_Batch";
		this.tab_Batch.ReadOnly = false;
		this.tab_Batch.SettingInVisible = false;
		this.tab_Batch.SettingReadonly = false;
		this.tab_Batch.Size = new System.Drawing.Size(971, 466);
		this.tab_Batch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Batch.Text = "Batch";
		this.grd_Batch.AllowPasteExcel = true;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Batch.Location = new System.Drawing.Point(0, 0);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.Size = new System.Drawing.Size(971, 466);
		this.grd_Batch.TabIndex = 0;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = null;
		this.gv_Batch.DisableFormSetting = false;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.EditColumn = null;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oDeliveryOrder";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IgnoreFilter = false;
		this.gv_Batch.IndexEditRow = -1;
		this.gv_Batch.IndicatorWidth = 35;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.IsMappingContextAction = false;
		this.gv_Batch.ItemId = "OBTN";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.UseBatchMod = false;
		this.gv_Batch.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Batch_PopupMenuShowing);
		this.tab_Invoices.Controls.Add(this.grd_Invoice);
		this.tab_Invoices.EditControl = null;
		this.tab_Invoices.EditValue = "Invoices";
		this.tab_Invoices.FieldName = null;
		this.tab_Invoices.IsSearchControl = false;
		this.tab_Invoices.Name = "tab_Invoices";
		this.tab_Invoices.ReadOnly = false;
		this.tab_Invoices.SettingInVisible = false;
		this.tab_Invoices.SettingReadonly = false;
		this.tab_Invoices.Size = new System.Drawing.Size(971, 466);
		this.tab_Invoices.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Invoices.Text = "Invoices";
		this.grd_Invoice.AllowPasteExcel = false;
		this.grd_Invoice.ClipboardData = null;
		this.grd_Invoice.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Invoice.Location = new System.Drawing.Point(0, 0);
		this.grd_Invoice.MainView = this.gv_Invoice;
		this.grd_Invoice.Name = "grd_Invoice";
		this.grd_Invoice.Size = new System.Drawing.Size(971, 466);
		this.grd_Invoice.TabIndex = 0;
		this.grd_Invoice.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Invoice });
		this.gv_Invoice.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Invoice.ColumnKey = null;
		this.gv_Invoice.DisableFormSetting = false;
		this.gv_Invoice.DisplayException = false;
		this.gv_Invoice.EditColumn = null;
		this.gv_Invoice.FieldCurrency = null;
		this.gv_Invoice.FieldRate = null;
		this.gv_Invoice.FormId = "oDeliveryOrder";
		this.gv_Invoice.GridControl = this.grd_Invoice;
		this.gv_Invoice.IgnoreFilter = false;
		this.gv_Invoice.IndexEditRow = -1;
		this.gv_Invoice.IndicatorWidth = 35;
		this.gv_Invoice.IsApzTable = false;
		this.gv_Invoice.IsInsertNewRow = false;
		this.gv_Invoice.IsLoadingData = false;
		this.gv_Invoice.IsMappingContextAction = false;
		this.gv_Invoice.ItemId = "OINV";
		this.gv_Invoice.LstColumnAllowBatch = null;
		this.gv_Invoice.LstColumnNotAllowBatch = null;
		this.gv_Invoice.LstColumnNotEditable = null;
		this.gv_Invoice.LstColumNotSetting = null;
		this.gv_Invoice.Name = "gv_Invoice";
		this.gv_Invoice.UseBatchMod = false;
		this.gv_Invoice.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Invoice_PopupMenuShowing);
		this.tab_Schedule.Controls.Add(this.grd_Schedule);
		this.tab_Schedule.EditControl = null;
		this.tab_Schedule.EditValue = "Schedule";
		this.tab_Schedule.FieldName = null;
		this.tab_Schedule.IsSearchControl = false;
		this.tab_Schedule.Name = "tab_Schedule";
		this.tab_Schedule.ReadOnly = false;
		this.tab_Schedule.SettingInVisible = false;
		this.tab_Schedule.SettingReadonly = false;
		this.tab_Schedule.Size = new System.Drawing.Size(971, 466);
		this.tab_Schedule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Schedule.Text = "Schedule";
		this.grd_Schedule.AllowPasteExcel = false;
		this.grd_Schedule.ClipboardData = null;
		this.grd_Schedule.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Schedule.Location = new System.Drawing.Point(0, 0);
		this.grd_Schedule.MainView = this.gv_Schedule;
		this.grd_Schedule.Name = "grd_Schedule";
		this.grd_Schedule.Size = new System.Drawing.Size(971, 466);
		this.grd_Schedule.TabIndex = 9;
		this.grd_Schedule.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Schedule });
		this.grd_Schedule.DragDrop += new System.Windows.Forms.DragEventHandler(this.grd_Schedule_DragDrop);
		this.grd_Schedule.DragOver += new System.Windows.Forms.DragEventHandler(this.grd_Schedule_DragOver);
		this.gv_Schedule.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Schedule.ColumnKey = null;
		this.gv_Schedule.DisableFormSetting = false;
		this.gv_Schedule.DisplayException = false;
		this.gv_Schedule.EditColumn = null;
		this.gv_Schedule.FieldCurrency = null;
		this.gv_Schedule.FieldRate = null;
		this.gv_Schedule.FormId = "oDeliveryOrder";
		this.gv_Schedule.GridControl = this.grd_Schedule;
		this.gv_Schedule.IgnoreFilter = false;
		this.gv_Schedule.IndexEditRow = -1;
		this.gv_Schedule.IndicatorWidth = 35;
		this.gv_Schedule.IsApzTable = false;
		this.gv_Schedule.IsInsertNewRow = false;
		this.gv_Schedule.IsLoadingData = false;
		this.gv_Schedule.IsMappingContextAction = false;
		this.gv_Schedule.ItemId = "OSCL";
		this.gv_Schedule.LstColumnAllowBatch = null;
		this.gv_Schedule.LstColumnNotAllowBatch = null;
		this.gv_Schedule.LstColumnNotEditable = null;
		this.gv_Schedule.LstColumNotSetting = null;
		this.gv_Schedule.Name = "gv_Schedule";
		this.gv_Schedule.UseBatchMod = false;
		this.gv_Schedule.MouseDown += new System.Windows.Forms.MouseEventHandler(this.gv_Schedule_MouseDown);
		this.gv_Schedule.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Schedule_MouseMove);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 497);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 20;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Print.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Print.EditControl = null;
		this.btn_Print.EditValue = "Print";
		this.btn_Print.FieldName = null;
		this.btn_Print.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Print.ImageOptions.Image");
		this.btn_Print.IsSearchControl = false;
		this.btn_Print.Location = new System.Drawing.Point(891, 497);
		this.btn_Print.Name = "btn_Print";
		this.btn_Print.ReadOnly = false;
		this.btn_Print.SettingInVisible = false;
		this.btn_Print.SettingReadonly = false;
		this.btn_Print.Size = new System.Drawing.Size(75, 23);
		this.btn_Print.TabIndex = 8;
		this.btn_Print.Text = "Print";
		this.btn_Print.ToolTip = "In phiếu ra cổng";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 497);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 7;
		this.btn_Save.Text = "OK";
		this.btn_Save.ToolTip = "Tạo phiếu ra cổng";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_SelectBatch.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SelectBatch.EditControl = null;
		this.btn_SelectBatch.EditValue = "Select Batch";
		this.btn_SelectBatch.FieldName = null;
		this.btn_SelectBatch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_SelectBatch.ImageOptions.Image");
		this.btn_SelectBatch.IsSearchControl = false;
		this.btn_SelectBatch.Location = new System.Drawing.Point(784, 497);
		this.btn_SelectBatch.Name = "btn_SelectBatch";
		this.btn_SelectBatch.ReadOnly = false;
		this.btn_SelectBatch.SettingInVisible = false;
		this.btn_SelectBatch.SettingReadonly = false;
		this.btn_SelectBatch.Size = new System.Drawing.Size(91, 23);
		this.btn_SelectBatch.TabIndex = 9;
		this.btn_SelectBatch.Text = "Select Batch";
		this.btn_SelectBatch.ToolTip = "Chọn bắn hàng ra cổng";
		this.btn_SelectBatch.Click += new System.EventHandler(this.btn_SelectBatch_Click);
		this.look_Employee.Location = new System.Drawing.Point(107, 31);
		this.look_Employee.Name = "look_Employee";
		this.look_Employee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Employee.Properties.NullText = "";
		this.look_Employee.Size = new System.Drawing.Size(145, 20);
		this.look_Employee.TabIndex = 18;
		this.look_Employee.ToolTip = "Chọn nhân viên giao hàng";
		this.look_Employee.EditValueChanged += new System.EventHandler(this.look_Employee_EditValueChanged);
		this.look_Bsx.Location = new System.Drawing.Point(354, 3);
		this.look_Bsx.Name = "look_Bsx";
		this.look_Bsx.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Bsx.Properties.NullText = "";
		this.look_Bsx.Size = new System.Drawing.Size(145, 20);
		this.look_Bsx.TabIndex = 19;
		this.look_Bsx.ToolTip = "Biển số xe";
		this.look_Bsx.EditValueChanged += new System.EventHandler(this.look_Bsx_EditValueChanged);
		this.ctx_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menuMainClose, this.menuMainCancel });
		this.ctx_Main.Name = "ctx_Main";
		this.ctx_Main.Size = new System.Drawing.Size(111, 48);
		this.menuMainClose.Name = "menuMainClose";
		this.menuMainClose.Size = new System.Drawing.Size(110, 22);
		this.menuMainClose.Text = "Close";
		this.menuMainClose.Click += new System.EventHandler(this.menuMainClose_Click);
		this.menuMainCancel.Name = "menuMainCancel";
		this.menuMainCancel.Size = new System.Drawing.Size(110, 22);
		this.menuMainCancel.Text = "Cancel";
		this.menuMainCancel.Click += new System.EventHandler(this.menuMainCancel_Click);
		this.panelControl1.ContextMenuStrip = this.ctx_Main;
		this.panelControl1.Controls.Add(this.txt_Confirm);
		this.panelControl1.Controls.Add(this.labelLine11);
		this.panelControl1.Controls.Add(this.look_Target);
		this.panelControl1.Controls.Add(this.labelLine7);
		this.panelControl1.Controls.Add(this.look_Employee3);
		this.panelControl1.Controls.Add(this.labelLine10);
		this.panelControl1.Controls.Add(this.look_Employee2);
		this.panelControl1.Controls.Add(this.date_Del);
		this.panelControl1.Controls.Add(this.txt_Remark);
		this.panelControl1.Controls.Add(this.labelLine9);
		this.panelControl1.Controls.Add(this.txt_BatchNo);
		this.panelControl1.Controls.Add(this.lbl_BatchNo);
		this.panelControl1.Controls.Add(this.btn_Clear);
		this.panelControl1.Controls.Add(this.txt_DocumentBarcode);
		this.panelControl1.Controls.Add(this.lbl_Barcode);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.look_Bsx);
		this.panelControl1.Controls.Add(this.look_Status);
		this.panelControl1.Controls.Add(this.look_Employee);
		this.panelControl1.Controls.Add(this.labelLine6);
		this.panelControl1.Controls.Add(this.labelLine5);
		this.panelControl1.Controls.Add(this.date_Confirm);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.labelLine8);
		this.panelControl1.Controls.Add(this.txt_DocNum);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(973, 111);
		this.panelControl1.TabIndex = 20;
		this.txt_Confirm.EditControl = null;
		this.txt_Confirm.FieldName = null;
		this.txt_Confirm.IsSearchControl = false;
		this.txt_Confirm.Location = new System.Drawing.Point(633, 3);
		this.txt_Confirm.Name = "txt_Confirm";
		this.txt_Confirm.Properties.ReadOnly = true;
		this.txt_Confirm.SettingInVisible = false;
		this.txt_Confirm.SettingReadonly = false;
		this.txt_Confirm.Size = new System.Drawing.Size(71, 20);
		this.txt_Confirm.TabIndex = 34;
		this.txt_Confirm.ToolTip = "Bắn barcode cuộn hàng cần đi giao";
		this.txt_Confirm.Visible = false;
		this.labelLine11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Confirm";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(570, 6);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(63, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 33;
		this.labelLine11.Utext = "Confirm";
		this.labelLine11.Visible = false;
		this.look_Target.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.look_Target.Location = new System.Drawing.Point(820, 83);
		this.look_Target.Name = "look_Target";
		this.look_Target.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Target.Properties.NullText = "";
		this.look_Target.Size = new System.Drawing.Size(145, 20);
		this.look_Target.TabIndex = 32;
		this.look_Target.ToolTip = "Biển số xe";
		this.labelLine7.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Địa điểm giao hàng";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(721, 85);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(101, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 31;
		this.labelLine7.Utext = "Địa điểm giao hàng";
		this.look_Employee3.Location = new System.Drawing.Point(107, 83);
		this.look_Employee3.Name = "look_Employee3";
		this.look_Employee3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Employee3.Properties.NullText = "";
		this.look_Employee3.Size = new System.Drawing.Size(145, 20);
		this.look_Employee3.TabIndex = 30;
		this.look_Employee3.ToolTip = "Chọn nhân viên giao hàng phụ";
		this.look_Employee3.EditValueChanged += new System.EventHandler(this.look_Employee_EditValueChanged);
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Người giao hàng 3";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(8, 86);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(102, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 29;
		this.labelLine10.Utext = "Người giao hàng 3";
		this.look_Employee2.Location = new System.Drawing.Point(107, 57);
		this.look_Employee2.Name = "look_Employee2";
		this.look_Employee2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Employee2.Properties.NullText = "";
		this.look_Employee2.Size = new System.Drawing.Size(145, 20);
		this.look_Employee2.TabIndex = 28;
		this.look_Employee2.ToolTip = "Chọn nhân viên giao hàng phụ";
		this.look_Employee2.EditValueChanged += new System.EventHandler(this.look_Employee_EditValueChanged);
		this.date_Del.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Del.EditValue = null;
		this.date_Del.Location = new System.Drawing.Point(820, 31);
		this.date_Del.Name = "date_Del";
		this.date_Del.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Del.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Del.Size = new System.Drawing.Size(145, 20);
		this.date_Del.TabIndex = 27;
		this.date_Del.EditValueChanged += new System.EventHandler(this.date_Del_EditValueChanged);
		this.txt_Remark.EditControl = null;
		this.txt_Remark.FieldName = null;
		this.txt_Remark.IsSearchControl = false;
		this.txt_Remark.Location = new System.Drawing.Point(354, 31);
		this.txt_Remark.Name = "txt_Remark";
		this.txt_Remark.SettingInVisible = false;
		this.txt_Remark.SettingReadonly = false;
		this.txt_Remark.Size = new System.Drawing.Size(350, 20);
		this.txt_Remark.TabIndex = 26;
		this.txt_Remark.ToolTip = "Bắn barcode cuộn hàng cần đi giao";
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Ghi chú";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(274, 34);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(83, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 25;
		this.labelLine9.Utext = "Ghi chú";
		this.txt_BatchNo.EditControl = null;
		this.txt_BatchNo.FieldName = null;
		this.txt_BatchNo.IsSearchControl = false;
		this.txt_BatchNo.Location = new System.Drawing.Point(354, 57);
		this.txt_BatchNo.Name = "txt_BatchNo";
		this.txt_BatchNo.SettingInVisible = false;
		this.txt_BatchNo.SettingReadonly = false;
		this.txt_BatchNo.Size = new System.Drawing.Size(188, 20);
		this.txt_BatchNo.TabIndex = 24;
		this.txt_BatchNo.ToolTip = "Bắn barcode cuộn hàng cần đi giao";
		this.txt_BatchNo.Visible = false;
		this.txt_BatchNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_BatchNo_KeyDown);
		this.lbl_BatchNo.EditControl = null;
		this.lbl_BatchNo.EditValue = "Batch No";
		this.lbl_BatchNo.FieldName = null;
		this.lbl_BatchNo.HideWhenEmpty = false;
		this.lbl_BatchNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BatchNo.Location = new System.Drawing.Point(274, 60);
		this.lbl_BatchNo.Name = "lbl_BatchNo";
		this.lbl_BatchNo.ReadOnly = false;
		this.lbl_BatchNo.SettingInVisible = false;
		this.lbl_BatchNo.SettingReadonly = false;
		this.lbl_BatchNo.ShowUnderLine = true;
		this.lbl_BatchNo.Size = new System.Drawing.Size(83, 20);
		this.lbl_BatchNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BatchNo.TabIndex = 23;
		this.lbl_BatchNo.Utext = "Batch No";
		this.lbl_BatchNo.Visible = false;
		this.btn_Clear.EditControl = null;
		this.btn_Clear.EditValue = "Clear";
		this.btn_Clear.FieldName = null;
		this.btn_Clear.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.ImageOptions.Image");
		this.btn_Clear.IsSearchControl = false;
		this.btn_Clear.Location = new System.Drawing.Point(548, 79);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.ReadOnly = false;
		this.btn_Clear.SettingInVisible = false;
		this.btn_Clear.SettingReadonly = false;
		this.btn_Clear.Size = new System.Drawing.Size(91, 23);
		this.btn_Clear.TabIndex = 22;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.ToolTip = "Làm mới";
		this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
		this.txt_DocumentBarcode.EditControl = null;
		this.txt_DocumentBarcode.FieldName = null;
		this.txt_DocumentBarcode.IsSearchControl = false;
		this.txt_DocumentBarcode.Location = new System.Drawing.Point(354, 82);
		this.txt_DocumentBarcode.Name = "txt_DocumentBarcode";
		this.txt_DocumentBarcode.SettingInVisible = false;
		this.txt_DocumentBarcode.SettingReadonly = false;
		this.txt_DocumentBarcode.Size = new System.Drawing.Size(188, 20);
		this.txt_DocumentBarcode.TabIndex = 21;
		this.txt_DocumentBarcode.ToolTip = "Bắn barcode phiếu hàng ra cổng";
		this.txt_DocumentBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocEntry_KeyDown);
		this.lbl_Barcode.EditControl = null;
		this.lbl_Barcode.EditValue = "Phiếu giao hàng";
		this.lbl_Barcode.FieldName = null;
		this.lbl_Barcode.HideWhenEmpty = false;
		this.lbl_Barcode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Barcode.Location = new System.Drawing.Point(274, 85);
		this.lbl_Barcode.Name = "lbl_Barcode";
		this.lbl_Barcode.ReadOnly = false;
		this.lbl_Barcode.SettingInVisible = false;
		this.lbl_Barcode.SettingReadonly = false;
		this.lbl_Barcode.ShowUnderLine = true;
		this.lbl_Barcode.Size = new System.Drawing.Size(83, 20);
		this.lbl_Barcode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Barcode.TabIndex = 20;
		this.lbl_Barcode.Utext = "Phiếu giao hàng";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Cancel);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Print);
		this.splitContainerControl1.Panel2.Controls.Add(this.tabControl_Delivery);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Save);
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_SelectBatch);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(973, 642);
		this.splitContainerControl1.SplitterPosition = 111;
		this.splitContainerControl1.TabIndex = 21;
		this.splitContainerControl1.Text = "splitContainerControl1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(973, 642);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oDeliveryOrder";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DeliveryOrder.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DeliveryOrder";
		base.ShowInTaskbar = false;
		this.Text = "Phiếu giao việc";
		base.Load += new System.EventHandler(this.frm_DeliveryOrder_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Confirm.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Confirm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl_Delivery).EndInit();
		this.tabControl_Delivery.ResumeLayout(false);
		this.tab_Batch.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		this.tab_Invoices.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Invoice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Invoice).EndInit();
		this.tab_Schedule.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Schedule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Schedule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Employee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Bsx.Properties).EndInit();
		this.ctx_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Confirm.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Target.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Employee3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Employee2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Del.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Del.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BatchNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocumentBarcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
