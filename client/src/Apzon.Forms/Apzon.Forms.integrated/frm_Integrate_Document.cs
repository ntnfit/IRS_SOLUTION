using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.integrated;

public class frm_Integrate_Document : XtraFormEx
{
	private string _formController = "Integrated";

	public string _isIns = "N";

	public ApzObjectTypes _formObjType;

	public string _IsReceiptFromProduct = "";

	private List<GridColumn> _lstEditableLineColumns = new List<GridColumn>();

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

	private RadioGroupEx radioGroupStatus;

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
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonSave;
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

	public frm_Integrate_Document()
	{
		InitializeComponent();
		FormMode = Enums.FormViewModeType.Add;
	}

	private async Task frm_Integrate_Document_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			lbl_Progress.Text = "";
			lbl_Progress.Visible = false;
			progressBar1.Visible = false;
			date_From.EditValue = DateTime.Today.AddDays(-30.0);
			date_To.EditValue = DateTime.Today;
			radioGroupStatus.SelectedIndex = 0;
			await LoadGridStructure();
			await Search();
			WindowState = FormWindowState.Maximized;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		EnableDisableButton();
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await Save();
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemAlreadyImported, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		EnableDisableButton();
		try
		{
			FormMode = ((radioGroupStatus.SelectedIndex != 0) ? Enums.FormViewModeType.Find : Enums.FormViewModeType.Add);
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
		WinCommons._CloseProcessing();
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string isReceiptFromProduct = _IsReceiptFromProduct;
			string isIns = _isIns;
			string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("U_CustomID"));
			int formObjType = (int)_formObjType;
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = await hp.GetAsync("SearchLine", new
			{
				objType = formObjType,
				id = id,
				isIns = isIns,
				isRecFromProd = isReceiptFromProduct
			});
			if (dataTable != null)
			{
				grd_Line.DataSource = dataTable;
				grd_Line.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Add;
				if (gv_Line.Columns["OcrCode"] != null)
				{
					gv_Line.Columns["OcrCode"].OptionsColumn.AllowEdit = true;
					gv_Line.Columns["OcrCode"].OptionsColumn.ReadOnly = false;
				}
				if (gv_Line.Columns["OcrCode2"] != null)
				{
					gv_Line.Columns["OcrCode2"].OptionsColumn.AllowEdit = true;
					gv_Line.Columns["OcrCode2"].OptionsColumn.ReadOnly = false;
				}
				if (gv_Line.Columns["OcrCode3"] != null)
				{
					gv_Line.Columns["OcrCode3"].OptionsColumn.AllowEdit = true;
					gv_Line.Columns["OcrCode3"].OptionsColumn.ReadOnly = false;
				}
				if (gv_Line.Columns["OcrCode4"] != null)
				{
					gv_Line.Columns["OcrCode4"].OptionsColumn.AllowEdit = true;
					gv_Line.Columns["OcrCode4"].OptionsColumn.ReadOnly = false;
				}
				if (gv_Line.Columns["OcrCode5"] != null)
				{
					gv_Line.Columns["OcrCode5"].OptionsColumn.AllowEdit = true;
					gv_Line.Columns["OcrCode5"].OptionsColumn.ReadOnly = false;
				}
			}
			else
			{
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				gv_Line.PopulateColumns();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Return && (date_From.ContainsFocus || date_To.ContainsFocus || radioGroupStatus.ContainsFocus))
		{
			btn_Search_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void gv_Doc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			GridView gridView = (GridView)sender;
			if (e.RowHandle == gridView.FocusedRowHandle)
			{
				e.Appearance.BackColor = gridView.Appearance.FocusedRow.BackColor;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void splitContainerControl1_Resize(object sender, EventArgs e)
	{
		splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.FocusedRow;
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				if (e.Column.FieldName == "SaveStatus")
				{
					string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, e.Column));
					if (!string.IsNullOrEmpty(text))
					{
						if (text.ToLower() == "success")
						{
							e.Appearance.ForeColor = Color.Green;
						}
						else
						{
							e.Appearance.ForeColor = Color.Red;
						}
					}
				}
				else if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly)
				{
					e.Appearance.BackColor = StyleColor.GridDisable1;
				}
			}
			else if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = StyleColor.GridDisable1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Line.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.FocusedRow;
			}
			else if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly || FormMode != Enums.FormViewModeType.Add)
			{
				e.Appearance.BackColor = StyleColor.GridDisable1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			if (e.Action == CollectionChangeAction.Add && (Function.ToString(gv_Doc.GetRowCellValue(e.ControllerRow, "SaveStatus")).ToLower() == "success" || FormMode == Enums.FormViewModeType.Find))
			{
				gv_Doc.UnselectRow(e.ControllerRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search()
	{
		EnableDisableButton();
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("ObjectType", typeof(int)));
			dataTable.Columns.Add(new DataColumn("FromDate", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ToDate", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsIns", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsReceiptFromProduct", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsImported", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["ObjectType"] = (int)_formObjType;
			DateTime? dateTime = Function.ParseDateTime(date_From.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(date_To.EditValue);
			dataRow["FromDate"] = ((!dateTime.HasValue) ? "19000101" : dateTime.Value.ToString("yyyyMMdd"));
			dataRow["ToDate"] = ((!dateTime2.HasValue) ? "19000101" : dateTime2.Value.ToString("yyyyMMdd"));
			dataRow["IsIns"] = _isIns;
			dataRow["IsReceiptFromProduct"] = _IsReceiptFromProduct;
			dataRow["IsImported"] = ((radioGroupStatus.SelectedIndex == 0) ? "O" : "C");
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable2 = await hp.PostAsync("Search", dataTable);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				grd_Doc.DataSource = dataTable2;
				grd_Doc.RefreshDataSource();
				if (dataTable2.Rows.Count == 0)
				{
					grd_Line.DataSource = null;
					grd_Line.RefreshDataSource();
				}
				else if (dataTable2.Rows.Count == 1)
				{
					gv_Doc_FocusedRowObjectChanged(null, null);
				}
				FormMode = Enums.FormViewModeType.Add;
				if (FormMode == Enums.FormViewModeType.Add)
				{
					for (int i = 0; i < _lstEditableLineColumns.Count; i++)
					{
						_lstEditableLineColumns[i].OptionsColumn.AllowEdit = true;
						_lstEditableLineColumns[i].OptionsColumn.ReadOnly = false;
					}
				}
				else
				{
					for (int j = 0; j < _lstEditableLineColumns.Count; j++)
					{
						_lstEditableLineColumns[j].OptionsColumn.AllowEdit = false;
						_lstEditableLineColumns[j].OptionsColumn.ReadOnly = true;
					}
				}
			}
			else
			{
				grd_Line.DataSource = null;
				grd_Line.RefreshDataSource();
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
	}

	private async Task Save()
	{
		_ = 2;
		try
		{
			DataTable dtCheck = new DataTable
			{
				Columns = 
				{
					{
						"ObjectType",
						typeof(int)
					},
					{
						"IsReceiptFromProduct",
						typeof(string)
					},
					{
						"IsIns",
						typeof(string)
					},
					{
						"ID",
						typeof(string)
					}
				}
			};
			EnableDisableButton();
			lbl_Progress.Text = "";
			lbl_Progress.Visible = true;
			progressBar1.Visible = true;
			DataTable dtsDoc = grd_Doc.DataSource as DataTable;
			int[] selectedRows = gv_Doc.GetSelectedRows();
			int success = 0;
			if (selectedRows.Length != 0)
			{
				IEnumerable<int> source = selectedRows.Where((int t) => Function.ToString(gv_Doc.GetRowCellValue(t, "SaveStatus")).ToLower() != "success");
				if (!source.Any())
				{
					return;
				}
				int[] arrSel = source.ToArray();
				int totalLine = arrSel.Length;
				progressBar1.Maximum = totalLine + 1;
				progressBar1.Minimum = 1;
				progressBar1.Step = 1;
				progressBar1.Value = 1;
				for (int i = 0; i < arrSel.Length; i++)
				{
					DataRow docRow = dtsDoc.Rows[gv_Doc.GetDataSourceRowIndex(arrSel[i])];
					int formObjType = (int)_formObjType;
					string id = Function.ToString(docRow["U_CustomID"]);
					List<Document_LinesImpl> list = (await GetLines(formObjType, id)).ToDocumentLine().ToList();
					if (list.Count == 0)
					{
						WinCommons.Error(Enums.MessageCode.NoDataToImport, "");
						break;
					}
					for (int num = 0; num < list.Count; num++)
					{
						list[num].BaseEntry = 0;
						list[num].BaseType = -1;
						list[num].BaseLine = 0;
						list[num].TargetDoc = 0;
						list[num].TargetType = -1;
						list[num].LineNum = num + 1;
					}
					DataTable dataTable = dtsDoc.Clone();
					dataTable.ImportRow(docRow);
					dataTable.Columns.Remove("U_CustomID");
					dataTable.AcceptChanges();
					DocumentsImpl doc = dataTable.Rows[0].ToDocument();
					doc.Lines = list;
					doc.isIns = _isIns;
					dtCheck.Rows.Clear();
					dtCheck.AcceptChanges();
					DataRow dataRow = dtCheck.NewRow();
					dataRow["ObjectType"] = (int)_formObjType;
					dataRow["IsReceiptFromProduct"] = _IsReceiptFromProduct;
					dataRow["IsIns"] = _isIns;
					dataRow["ID"] = id;
					dtCheck.Rows.Add(dataRow);
					dtCheck.AcceptChanges();
					if (await CheckExists(dtCheck))
					{
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "This documents exists!");
					}
					else
					{
						string text = await SaveADoc(doc);
						if (string.IsNullOrWhiteSpace(text))
						{
							success++;
							WriteLogFile("[SUCCESS] : " + _formObjType);
							gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Success");
							gv_Doc.UnselectRow(arrSel[i]);
						}
						else
						{
							WriteLogFile("[ERROR] : " + _formObjType.ToString() + ": " + text);
							gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Error: " + text);
						}
					}
					progressBar1.PerformStep();
					lbl_Progress.Text = i + 1 + " of " + totalLine + ", " + success + " success.";
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableButton(bool enable = false)
	{
		try
		{
			radioGroupStatus.Enabled = enable;
			btn_Search.Enabled = enable;
			btn_OK.Enabled = enable;
			btn_Cancel.Enabled = enable;
			date_From.Enabled = enable;
			date_To.Enabled = enable;
			Enums.FormViewModeType formMode;
			if (gv_Doc.DataRowCount <= 0)
			{
				Enums.FormViewModeType formViewModeType = (FormMode = Enums.FormViewModeType.View);
				formMode = formViewModeType;
			}
			else
			{
				Enums.FormViewModeType formViewModeType = (FormMode = Enums.FormViewModeType.Add);
				formMode = formViewModeType;
			}
			FormMode = formMode;
			if (enable && FormMode == Enums.FormViewModeType.Find)
			{
				btn_OK.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveADoc(DocumentsImpl doc)
	{
		string result;
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			result = await hp.PostAsync("SaveADoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			result = ex.Message;
		}
		return result;
	}

	private async Task<DocumentLineTable> GetLines(int objType, string id)
	{
		try
		{
			string isIns = _isIns;
			string isReceiptFromProduct = _IsReceiptFromProduct;
			using HttpClientHelper<DocumentLineTable> hp = new HttpClientHelper<DocumentLineTable>(Configuration.ApiServer, _formController);
			DocumentLineTable documentLineTable = await hp.GetAsync("SearchLine", new
			{
				objType = objType,
				id = id,
				isIns = isIns,
				isRecFromProd = isReceiptFromProduct
			});
			if (documentLineTable != null && documentLineTable.Rows.Count > 0)
			{
				if (documentLineTable.Columns.Contains("U_OldOrderRev"))
				{
					documentLineTable.Columns.Remove("U_OldOrderRev");
					documentLineTable.AcceptChanges();
				}
				return documentLineTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
			Logging.Write("ERROR", ex);
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
			Logging.Write("ERROR", ex);
		}
		return ret;
	}

	private async Task LoadGridStructure()
	{
		_ = 5;
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("GetDocGridStruct");
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
			{
				if (dataSet.Tables.Count != 2 || dataSet.Tables[1] == null)
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet.Tables[0], null);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet.Tables[0], null, dataSet.Tables[1]);
				}
			}
			gv_Doc.OptionsView.EnableAppearanceEvenRow = false;
			DataSet dataSet2 = await hp.GetAsync("GetDocLineGridStruct");
			if (dataSet2 != null && dataSet2.Tables.Count > 0 && dataSet2.Tables[0] != null && dataSet2.Tables[0].Rows.Count > 0)
			{
				if (dataSet2.Tables.Count != 2 || dataSet2.Tables[1] == null)
				{
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dataSet2.Tables[0], null);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dataSet2.Tables[0], null, dataSet2.Tables[1]);
				}
			}
			gv_Line.OptionsView.EnableAppearanceEvenRow = false;
			for (int i = 0; i < gv_Line.Columns.Count; i++)
			{
				GridColumn gridColumn = gv_Line.Columns[i];
				if (gridColumn.OptionsColumn.AllowEdit && !gridColumn.OptionsColumn.ReadOnly)
				{
					_lstEditableLineColumns.Add(gridColumn);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.integrated.frm_Integrate_Document));
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
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 11);
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
		this.date_From.Location = new System.Drawing.Point(88, 8);
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
		this.splitContainerControl1.SplitterPosition = 183;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl1.Resize += new System.EventHandler(this.splitContainerControl1_Resize);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(882, 183);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oItegratedDocument";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 50;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ORDR";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Doc_RowStyle);
		this.gv_Doc.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Doc_SelectionChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(882, 269);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oItegratedDocument";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 50;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "RDR1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Line_RowCellStyle);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(490, 8);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(107, 20);
		this.btn_Search.TabIndex = 5;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.btn_OK.Appearance.Options.UseForeColor = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 508);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(76, 20);
		this.btn_OK.TabIndex = 6;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 508);
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
		this.radioGroupStatus.EditControl = null;
		this.radioGroupStatus.FieldName = null;
		this.radioGroupStatus.IsSearchControl = false;
		this.radioGroupStatus.Location = new System.Drawing.Point(376, 4);
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
		this.radioGroupStatus.TabIndex = 10;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(885, 536);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.radioGroupStatus);
		base.Controls.Add(this.lbl_Progress);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oItegratedDocument";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Integrate_Document.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Integrate_Document";
		this.Text = "Document";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_Integrate_Document_Load);
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
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
