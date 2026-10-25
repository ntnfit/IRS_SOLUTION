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
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Mrps;

public class frm_MrpPlan : XtraFormEx
{
	private enum SearchType
	{
		First = 1,
		Preveous,
		Next,
		Last,
		Find
	}

	private const string FormController = "MrpPlan";

	private const string TableHeader = "APZ_MRP_M2H";

	private const string TableDetail = "APZ_MRP_M2D";

	private IContainer components;

	private PanelControl panelControl1;

	private PanelControl panelControl3;

	private PanelControl panelControl2;

	private LabelLine labelLine1;

	private TextEditEx txt_DocEntry;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private DateEditEx date_CreateDate;

	private ButtonEditEx bte_BPCode;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private SimpleButtonEx btn_Run;

	private LookUpEdit lue_TimeType;

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

	public frm_MrpPlan()
	{
		InitializeComponent();
	}

	private void frm_MrpPlan_Load(object sender, EventArgs e)
	{
		try
		{
			Text = "MRP - Ước tính hàng tồn kho";
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Display", typeof(string));
			dataTable.Columns.Add("Value", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Display"] = "Monthly";
			dataRow["Value"] = "M";
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Display"] = "Weekly";
			dataRow2["Value"] = "W";
			dataTable.Rows.Add(dataRow);
			dataTable.Rows.Add(dataRow2);
			lue_TimeType.Properties.DataSource = dataTable;
			lue_TimeType.Properties.DisplayMember = "Display";
			lue_TimeType.Properties.ValueMember = "Value";
			lue_TimeType.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			lue_TimeType.EditValue = "M";
			date_CreateDate.EditValue = DateTime.Today;
			date_CreateDate.ReadOnly = true;
			txt_DocEntry.ReadOnly = true;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_BPCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Vendor, true, bte_BPCode.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			string text = "";
			if (chooseItems != null && chooseItems.Count > 0)
			{
				foreach (DataRow item in chooseItems)
				{
					text = text + item[0]?.ToString() + ",";
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				text = text.Substring(0, text.Length - 1);
				bte_BPCode.Text = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_DocEntry_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_DocEntry.Text) && FormMode == Enums.FormViewModeType.Find)
			{
				int num = Function.ParseInt(txt_DocEntry.EditValue);
				if (num > 0)
				{
					await Navigate(num, 5);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Detail_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				if (!string.IsNullOrEmpty(txt_DocEntry.Text))
				{
					int currentId = Function.ParseInt(txt_DocEntry.EditValue);
					await Navigate(currentId, 5);
				}
			}
			else if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				DataTable dataSourceTable = grd_Detail.DataSourceTable;
				if (!dataSourceTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				btn_Save.Enabled = false;
				DataTable dataTable = new DataTable("APZ_MRP_M2H");
				dataTable.Columns.Add("DocEntry", typeof(int));
				dataTable.Columns.Add("CreateDate", typeof(DateTime));
				dataTable.Columns.Add("CreateUser", typeof(string));
				dataTable.Columns.Add("Employee", typeof(string));
				dataTable.Columns.Add("BPCode", typeof(string));
				dataTable.Columns.Add("TimeType", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = ((FormMode != Enums.FormViewModeType.Add) ? Function.ParseInt(txt_DocEntry.Text) : 0);
				dataRow["CreateDate"] = Function.ParseDateTime(date_CreateDate.EditValue);
				dataRow["CreateUser"] = GlobalData.UserName;
				dataRow["Employee"] = "";
				dataRow["BPCode"] = Function.ToString(bte_BPCode.Text);
				dataRow["TimeType"] = Function.ToString(lue_TimeType.EditValue);
				dataTable.Rows.Add(dataRow);
				DataTable dataTable2 = dataSourceTable.Copy();
				dataTable2.TableName = "APZ_MRP_M2D";
				foreach (string item in (from DataColumn t in dataTable2.Columns
					where t.ColumnName.StartsWith("U_")
					select t.ColumnName).ToList())
				{
					dataTable2.Columns.Remove(item);
				}
				DataSet ds = new DataSet
				{
					Tables = 
					{
						dataTable,
						dataTable2.Copy()
					}
				};
				HttpResult httpResult = await Save(ds);
				WinCommons._CloseProcessing();
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					FormMode = Enums.FormViewModeType.View;
					if (httpResult.Content != null)
					{
						txt_DocEntry.EditValue = httpResult.Content;
					}
					txt_DocEntry.Properties.ReadOnly = true;
					FormMode = Enums.FormViewModeType.View;
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
		btn_Save.Enabled = false;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Run_Click(object sender, EventArgs e)
	{
		await Run();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int currentId = Function.ParseInt(txt_DocEntry.EditValue);
			await Navigate(currentId, 3);
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
			int currentId = Function.ParseInt(txt_DocEntry.EditValue);
			await Navigate(currentId, 1);
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
			int currentId = Function.ParseInt(txt_DocEntry.EditValue);
			await Navigate(currentId, 2);
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
			int currentId = Function.ParseInt(txt_DocEntry.EditValue);
			await Navigate(currentId, 4);
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
			txt_DocEntry.Properties.ReadOnly = false;
			bte_BPCode.Text = "";
			bte_BPCode.Enabled = false;
			lue_TimeType.Enabled = false;
			lue_TimeType.EditValue = "";
			FormMode = Enums.FormViewModeType.Find;
			date_CreateDate.EditValue = null;
			txt_DocEntry.BackColor = Color.FromArgb(250, 221, 122);
			date_CreateDate.BackColor = Color.FromArgb(250, 221, 122);
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
			REFRESH();
			await GetById(id);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			date_CreateDate.EditValue = DateTime.Today;
			txt_DocEntry.Properties.ReadOnly = true;
			bte_BPCode.Enabled = true;
			lue_TimeType.Enabled = true;
			FormMode = Enums.FormViewModeType.Find;
			txt_DocEntry.ResetBackColor();
			date_CreateDate.ResetBackColor();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetById(int id)
	{
		DataTable dataTable = await GetHeader(id);
		if (dataTable.IsNotNull())
		{
			txt_DocEntry.Text = Function.ToString(dataTable.Rows[0]["DocEntry"]);
			bte_BPCode.Text = Function.ToString(dataTable.Rows[0]["BPCode"]);
			lue_TimeType.EditValue = Function.ToString(dataTable.Rows[0]["TimeType"]);
			date_CreateDate.EditValue = Function.ParseDateTime(dataTable.Rows[0]["CreateDate"]);
		}
		dataTable.Dispose();
		DataSet dsDetail = await GetDetail(id);
		if (dsDetail.IsNotNull())
		{
			await GridViewHelper.GenerateGrid(grd_Detail, gv_Detail, dsDetail);
		}
		dsDetail.Tables.Clear();
		dsDetail.Dispose();
	}

	public async Task<int> GetPage(int current, int type, string username, int branch)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "MrpPlan");
			return await helper.GetIntAsync("GetPage", new { current, type, username, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public async Task<DataTable> GetHeader(int id)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MrpPlan");
			return await helper.GetAsync("GetHeader", new { id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task<DataSet> GetDetail(int id)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "MrpPlan");
			return await helper.GetAsync("GetDetail", new { id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	public async Task<HttpResult> Save(DataSet ds)
	{
		try
		{
			using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "MrpPlan");
			return await _help.PostAsync("Save", ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Run()
	{
		_ = 1;
		try
		{
			btn_Run.Enabled = false;
			btn_Save.Enabled = false;
			WinCommons._OpenProcessing();
			string timeType = Function.ToString(lue_TimeType.EditValue);
			string bpCode = Function.ToString(bte_BPCode.Text);
			new DataSet();
			DataSet ds = await RunMrpPlan(bpCode, timeType);
			if (ds.IsNotNull())
			{
				GridViewHelper.dfAutoHeightHeader = true;
				await GridViewHelper.GenerateGrid(grd_Detail, gv_Detail, ds, null, expand: true);
				FormMode = Enums.FormViewModeType.Add;
			}
			ds.Tables.Clear();
			ds.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Run.Enabled = true;
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async Task<DataSet> RunMrpPlan(string bpCode, string timeType)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "MrpPlan");
			return await helper.GetAsync("RunMrpPlan", new { bpCode, timeType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_MrpPlan));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.lue_TimeType = new DevExpress.XtraEditors.LookUpEdit();
		this.btn_Run = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_BPCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.date_CreateDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocEntry = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lue_TimeType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Controls.Add(this.lue_TimeType);
		this.panelControl1.Controls.Add(this.btn_Run);
		this.panelControl1.Controls.Add(this.bte_BPCode);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.date_CreateDate);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.txt_DocEntry);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(759, 76);
		this.panelControl1.TabIndex = 0;
		this.lue_TimeType.Location = new System.Drawing.Point(91, 12);
		this.lue_TimeType.Name = "lue_TimeType";
		this.lue_TimeType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lue_TimeType.Properties.NullText = "";
		this.lue_TimeType.Size = new System.Drawing.Size(172, 20);
		this.lue_TimeType.TabIndex = 67;
		this.btn_Run.EditControl = null;
		this.btn_Run.EditValue = "Run";
		this.btn_Run.FieldName = null;
		this.btn_Run.IsSearchControl = false;
		this.btn_Run.Location = new System.Drawing.Point(480, 12);
		this.btn_Run.Name = "btn_Run";
		this.btn_Run.ReadOnly = false;
		this.btn_Run.SettingInVisible = false;
		this.btn_Run.SettingReadonly = false;
		this.btn_Run.Size = new System.Drawing.Size(75, 23);
		this.btn_Run.TabIndex = 66;
		this.btn_Run.Text = "Run";
		this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
		this.bte_BPCode.EditControl = null;
		this.bte_BPCode.FieldName = null;
		this.bte_BPCode.IsSearchControl = false;
		this.bte_BPCode.Location = new System.Drawing.Point(91, 40);
		this.bte_BPCode.Name = "bte_BPCode";
		this.bte_BPCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.bte_BPCode.Properties.Appearance.Options.UseFont = true;
		this.bte_BPCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BPCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_BPCode_Properties_ButtonClick);
		this.bte_BPCode.SettingInVisible = false;
		this.bte_BPCode.SettingReadonly = false;
		this.bte_BPCode.Size = new System.Drawing.Size(172, 18);
		this.bte_BPCode.TabIndex = 64;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(5, 38);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 63;
		this.labelLine2.Utext = "BP Code";
		this.date_CreateDate.EditControl = null;
		this.date_CreateDate.EditValue = null;
		this.date_CreateDate.FieldName = null;
		this.date_CreateDate.Location = new System.Drawing.Point(344, 38);
		this.date_CreateDate.Name = "date_CreateDate";
		this.date_CreateDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CreateDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CreateDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_CreateDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_CreateDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.IsSearchControl = false;
		this.date_CreateDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_CreateDate.SettingInVisible = false;
		this.date_CreateDate.SettingReadonly = false;
		this.date_CreateDate.Size = new System.Drawing.Size(105, 20);
		this.date_CreateDate.TabIndex = 62;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Create Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(270, 37);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(70, 21);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 16;
		this.labelLine4.Utext = "Create Date";
		this.txt_DocEntry.EditControl = null;
		this.txt_DocEntry.FieldName = null;
		this.txt_DocEntry.IsSearchControl = false;
		this.txt_DocEntry.Location = new System.Drawing.Point(344, 12);
		this.txt_DocEntry.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_DocEntry.Name = "txt_DocEntry";
		this.txt_DocEntry.Properties.ReadOnly = true;
		this.txt_DocEntry.SettingInVisible = false;
		this.txt_DocEntry.SettingReadonly = false;
		this.txt_DocEntry.Size = new System.Drawing.Size(105, 20);
		this.txt_DocEntry.TabIndex = 15;
		this.txt_DocEntry.ToolTip = "Số phiếu tính giấy";
		this.txt_DocEntry.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocEntry_KeyDown);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc No";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(270, 11);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(70, 21);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 14;
		this.labelLine3.Utext = "Doc No";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Time Type";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(5, 15);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(82, 21);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 10;
		this.labelLine1.Utext = "Time Type";
		this.panelControl3.Controls.Add(this.btn_Cancel);
		this.panelControl3.Controls.Add(this.btn_Save);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(0, 440);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(759, 39);
		this.panelControl3.TabIndex = 2;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(84, 11);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 20;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.ToolTip = "Thoát";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(5, 11);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 8;
		this.btn_Save.Text = "OK";
		this.btn_Save.ToolTip = "Tạo phiếu ra cổng";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.panelControl2.Controls.Add(this.grd_Detail);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 76);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(759, 364);
		this.panelControl2.TabIndex = 3;
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(2, 2);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.Size = new System.Drawing.Size(755, 360);
		this.grd_Detail.TabIndex = 1;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.EditColumn = null;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "oMrpPlan";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IgnoreFilter = false;
		this.gv_Detail.IndexEditRow = -1;
		this.gv_Detail.IndicatorWidth = 35;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.IsMappingContextAction = false;
		this.gv_Detail.ItemId = "ODATA";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(759, 479);
		base.Controls.Add(this.panelControl2);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oMrpPlan";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MrpPlan";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "MRP";
		base.Load += new System.EventHandler(this.frm_MrpPlan_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lue_TimeType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		base.ResumeLayout(false);
	}
}
