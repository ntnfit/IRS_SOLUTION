using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.integrated;

public class frm_Integrate_InventoryTransfer : XtraFormEx
{
	private string _privilege = "";

	private string _formController = "IntegratedInventoryTransfer";

	public string _isIns = "N";

	public ApzObjectTypes _formObjType;

	private IContainer components;

	private LabelLine labelLine1;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private LabelLine labelLine2;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridControlEx grd_Line;

	private SimpleButtonEx btn_Search;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private System.Windows.Forms.ProgressBar progressBar1;

	private Label lbl_Progress;

	private GridViewEx gv_Doc;

	private GridViewEx gv_Line;

	private ComboBoxEditEx cbType;

	private LabelLine lblType;

	private RadioGroupEx radioGroupStatus;

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
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Resources.ButtonFind;
			break;
		default:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Integrate_InventoryTransfer()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Integrate_InventoryTransfer"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Integrate_InventoryTransfer" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			FormMode = Enums.FormViewModeType.Add;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_Integrate_InventoryTransfer_Load(object sender, EventArgs e)
	{
		try
		{
			lbl_Progress.Text = "";
			lbl_Progress.Visible = false;
			progressBar1.Visible = false;
			date_From.DateTime = DateTime.Today;
			date_To.DateTime = DateTime.Today.AddDays(1.0);
			cbType.SelectedText = "All";
			radioGroupStatus.SelectedIndex = 0;
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		EnableDisableButton();
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (_formObjType == ApzObjectTypes.oStockTransfer)
			{
				if (FormMode == Enums.FormViewModeType.Add)
				{
					await SaveTransfer();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.ItemAlreadyImported, "");
				}
			}
			else if (_formObjType == ApzObjectTypes.oInventoryTransferRequest)
			{
				await SaveRequest();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = ((radioGroupStatus.SelectedIndex != 0) ? Enums.FormViewModeType.Find : Enums.FormViewModeType.Add);
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("U_AugessId"));
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("SearchLine", new { id });
			if (dataSet != null && dataSet.Tables.Count == 2 && dataSet.Tables[0] != null && dataSet.Tables[1] != null)
			{
				await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dataSet.Tables[0], dataSet.Tables[1]);
				FormMode = Enums.FormViewModeType.Add;
				return;
			}
			grd_Line.DataSource = null;
			grd_Line.RefreshDataSource();
			gv_Line.PopulateColumns();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Return && (date_From.ContainsFocus || date_To.ContainsFocus || radioGroupStatus.ContainsFocus || cbType.CanFocus))
		{
			btn_Search_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private async Task Search()
	{
		EnableDisableButton();
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("FromDate", typeof(DateTime)));
			dataTable.Columns.Add(new DataColumn("ToDate", typeof(DateTime)));
			dataTable.Columns.Add(new DataColumn("InvType", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsImported", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["FromDate"] = ((!Function.ParseDateTime(date_From.EditValue).HasValue) ? new DateTime(1990, 1, 1) : Function.ParseDateTime(date_From.EditValue).Value);
			dataRow["ToDate"] = ((!Function.ParseDateTime(date_To.EditValue).HasValue) ? new DateTime(1990, 1, 1) : Function.ParseDateTime(date_To.EditValue).Value);
			dataRow["InvType"] = ((cbType.SelectedText == null || string.IsNullOrWhiteSpace(cbType.SelectedText)) ? "ALL" : Function.ToString(cbType.SelectedText).ToUpper());
			dataRow["IsImported"] = ((radioGroupStatus.SelectedIndex == 0) ? "O" : "C");
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet ds = await hp.PostAsync("Search", dataTable);
			if (ds != null && ds.Tables.Count == 2 && ds.Tables[0] != null && ds.Tables[1] != null)
			{
				await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds.Tables[0], ds.Tables[1]);
				if (ds.Tables[1].Rows.Count == 0)
				{
					grd_Line.DataSource = null;
					grd_Line.RefreshDataSource();
				}
				else if (ds.Tables[1].Rows.Count == 1)
				{
					gv_Doc_FocusedRowObjectChanged(null, null);
				}
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				gv_Doc.PopulateColumns();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
	}

	private async Task SaveTransfer()
	{
		_ = 2;
		try
		{
			DataTable dtCheck = new DataTable
			{
				Columns = { 
				{
					"ID",
					typeof(string)
				} }
			};
			EnableDisableButton();
			lbl_Progress.Text = "";
			lbl_Progress.Visible = true;
			progressBar1.Visible = true;
			DataTable dtsDoc = grd_Doc.DataSource as DataTable;
			int[] arrSel = gv_Doc.GetSelectedRows();
			int totalLine = 0;
			int success = 0;
			if (arrSel != null)
			{
				totalLine = arrSel.Length;
				progressBar1.Maximum = totalLine + 1;
				progressBar1.Minimum = 1;
				progressBar1.Step = 1;
				progressBar1.Value = 1;
			}
			for (int i = 0; i < arrSel.Length; i++)
			{
				DataRow dataRow = dtsDoc.Rows[gv_Doc.GetDataSourceRowIndex(arrSel[i])];
				DocumentsImpl doc = dataRow.ToDocument();
				string id = Function.ToString(dataRow["U_AugessId"]);
				List<Document_LinesImpl> list = (await GetLines(id)).ToDocumentLine();
				if (list == null && list.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.NoDataToImport, "");
					return;
				}
				for (int j = 0; j < list.Count; j++)
				{
					list[j].BaseEntry = 0;
					list[j].BaseType = -1;
					list[j].BaseLine = 0;
					list[j].TargetType = -1;
				}
				doc.Lines = list;
				doc.isIns = _isIns;
				doc.DocEntry = 0;
				dtCheck.Rows.Clear();
				dtCheck.AcceptChanges();
				DataRow dataRow2 = dtCheck.NewRow();
				dataRow2["ID"] = id;
				dtCheck.Rows.Add(dataRow2);
				dtCheck.AcceptChanges();
				if (await CheckExists(dtCheck))
				{
					gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "This documents exists!");
				}
				else
				{
					string text = await SaveADocTransfer(doc);
					if (string.IsNullOrWhiteSpace(text))
					{
						success++;
						WriteLogFile("[SUCCESS] : " + _formObjType);
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Success");
					}
					else
					{
						WriteLogFile("[ERROR] : " + _formObjType.ToString() + ": " + text);
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Error: " + text.ToString());
					}
				}
				progressBar1.PerformStep();
				lbl_Progress.Text = i + 1 + " of " + totalLine;
			}
			lbl_Progress.Text = lbl_Progress.Text + ", " + success + " success.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveRequest()
	{
	}

	private async Task<string> SaveADocTransfer(DocumentsImpl doc)
	{
		string result = "";
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			result = await hp.PostAsync("SaveADocTransfer", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private async Task<DocumentLineTable> GetLines(string id)
	{
		try
		{
			using HttpClientHelper<DocumentLineTable> hp = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, _formController);
			DocumentLineTable documentLineTable = await hp.GetAsync("SearchLineForSave", new { id });
			if (documentLineTable != null && documentLineTable.Rows.Count > 0)
			{
				return documentLineTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DocumentLineTable();
	}

	private void WriteLogFile(string errorMsg)
	{
		try
		{
			DateTime now = DateTime.Now;
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log\\" + now.ToString("yyyy") + "\\" + now.ToString("MM"));
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			using StreamWriter streamWriter = new StreamWriter(text + "\\Intergrated-log-" + now.ToString("yyyy_MM_dd") + ".txt", append: true);
			streamWriter.WriteLine(errorMsg);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableButton(bool enable = false)
	{
		try
		{
			if (enable)
			{
				radioGroupStatus.Enabled = true;
				btn_Search.Enabled = true;
				btn_OK.Enabled = true;
				btn_Cancel.Enabled = true;
			}
			else
			{
				radioGroupStatus.Enabled = false;
				btn_Search.Enabled = false;
				btn_OK.Enabled = false;
				btn_Cancel.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<bool> CheckExists(DataTable dt)
	{
		bool ret = false;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = await hp.PostAsync("CheckExists", dt);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret = Function.ParseInt(dataTable.Rows[0][0]) > 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return ret;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.integrated.frm_Integrate_InventoryTransfer));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.lbl_Progress = new System.Windows.Forms.Label();
		this.cbType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lblType = new Apzon.Controls.Controls.LabelLine();
		this.radioGroupStatus = new Apzon.Controls.Editor.RadioGroupEx();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 10);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "From Date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(88, 7);
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
		this.date_From.Size = new System.Drawing.Size(100, 20);
		this.date_From.TabIndex = 1;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(269, 7);
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
		this.date_To.Size = new System.Drawing.Size(100, 20);
		this.date_To.TabIndex = 3;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(194, 10);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "To Date";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 39);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(882, 462);
		this.splitContainerControl1.SplitterPosition = 191;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(882, 191);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oItegratedTransfer";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OWTR";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(882, 261);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oItegratedTransfer";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "WTR1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.UseBatchMod = false;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(703, 7);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(100, 20);
		this.btn_Search.TabIndex = 5;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 507);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 6;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 507);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 7;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressBar1.Location = new System.Drawing.Point(480, 510);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(213, 20);
		this.progressBar1.TabIndex = 8;
		this.lbl_Progress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Progress.AutoSize = true;
		this.lbl_Progress.Location = new System.Drawing.Point(700, 513);
		this.lbl_Progress.Name = "lbl_Progress";
		this.lbl_Progress.Size = new System.Drawing.Size(35, 13);
		this.lbl_Progress.TabIndex = 9;
		this.lbl_Progress.Text = "label1";
		this.cbType.EditControl = null;
		this.cbType.FieldName = null;
		this.cbType.IsSearchControl = false;
		this.cbType.Location = new System.Drawing.Point(455, 7);
		this.cbType.Name = "cbType";
		this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbType.Properties.Items.AddRange(new object[3] { "All", "Out", "In" });
		this.cbType.SettingInVisible = false;
		this.cbType.SettingReadonly = false;
		this.cbType.Size = new System.Drawing.Size(100, 20);
		this.cbType.TabIndex = 10;
		this.cbType.Visible = false;
		this.lblType.EditControl = null;
		this.lblType.EditValue = "Type";
		this.lblType.FieldName = null;
		this.lblType.HideWhenEmpty = false;
		this.lblType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblType.Location = new System.Drawing.Point(379, 10);
		this.lblType.Name = "lblType";
		this.lblType.ReadOnly = false;
		this.lblType.SettingInVisible = false;
		this.lblType.SettingReadonly = false;
		this.lblType.ShowUnderLine = true;
		this.lblType.Size = new System.Drawing.Size(80, 20);
		this.lblType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblType.TabIndex = 11;
		this.lblType.Utext = "Type";
		this.lblType.Visible = false;
		this.radioGroupStatus.EditControl = null;
		this.radioGroupStatus.FieldName = null;
		this.radioGroupStatus.IsSearchControl = false;
		this.radioGroupStatus.Location = new System.Drawing.Point(561, 4);
		this.radioGroupStatus.Name = "radioGroupStatus";
		this.radioGroupStatus.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroupStatus.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroupStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroupStatus.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "Open"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("C", "Close")
		});
		this.radioGroupStatus.SettingInVisible = false;
		this.radioGroupStatus.SettingReadonly = false;
		this.radioGroupStatus.Size = new System.Drawing.Size(121, 26);
		this.radioGroupStatus.TabIndex = 12;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(885, 536);
		base.Controls.Add(this.radioGroupStatus);
		base.Controls.Add(this.cbType);
		base.Controls.Add(this.lblType);
		base.Controls.Add(this.lbl_Progress);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oItegratedTransfer";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Integrate_InventoryTransfer.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Integrate_InventoryTransfer";
		this.Text = "Document";
		base.Load += new System.EventHandler(this.frm_Integrate_InventoryTransfer_Load);
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
