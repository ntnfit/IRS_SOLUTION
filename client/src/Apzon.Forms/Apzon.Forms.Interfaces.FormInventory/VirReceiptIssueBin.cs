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
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Interfaces.FormInventory;

public class VirReceiptIssueBin : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private BackgroundWorker backgroundWorker1;

	private SimpleButtonEx simpleButton2;

	private SimpleButtonEx btn_OK;

	private GridViewEx gv_Batch;

	private GridControlEx grd_Batch;

	private LabelLine labelLine4;

	private TextEditEx txt_Batch;

	private DateEditEx date_Doc;

	private LabelLine labelLine3;

	private TextEditEx txt_DocNo;

	private LabelLine labelLine1;

	private SimpleButtonEx simpleButton1;

	private ButtonEditEx bte_BIN;

	private LabelLine BIN;

	public DateTime GetDate { get; set; }

	public new ApzObjectTypes FormObjectType { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			simpleButton1.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public VirReceiptIssueBin()
	{
		InitializeComponent();
		OnLoadAsync += VirFormDocumentForBatch_Load;
	}

	public virtual async void txt_Batch_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 1;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(txt_Batch.Text))
			{
				return;
			}
			string text = Function.ToString(txt_Batch.Text);
			txt_Batch.Text = "";
			string bin = Function.ToString(bte_BIN.Text);
			if (grd_Batch.DataSource != null)
			{
				DataTable dataTable = grd_Batch.DataSource as DataTable;
				if (dataTable.ExistsColumn("DistNumber") && dataTable != null && dataTable.Select(string.Format("DistNumber='{0}'", "17-" + text)).Length != 0)
				{
					WinCommons.Error(Enums.MessageCode.None, "Đã tồn tại \"Batch\" trong lưới dữ liệu");
					txt_Batch.Text = "";
					txt_Batch.Focus();
					return;
				}
			}
			DataSet dataSet = await _httpClient.ReceiptIssueBin.GetData(text, bin, FormObjectType);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 2)
			{
				if (gv_Batch.Columns.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Batch, gv_Batch, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2]);
					gv_Batch.Columns[0].Visible = true;
				}
				else if (grd_Batch.DataSource != null && grd_Batch.DataSourceTable.IsNotNull() && grd_Batch.DataSourceTable.Rows.Count > 0)
				{
					DataTable dataTable2 = (DataTable)grd_Batch.DataSource;
					foreach (DataRow row in dataSet.Tables[1].Rows)
					{
						DataRow dataRow2 = dataTable2.NewRow();
						dataRow2.ItemArray = row.ItemArray;
						dataTable2.Rows.Add(dataRow2);
					}
					dataTable2.AcceptChanges();
				}
				else
				{
					grd_Batch.DataSource = dataSet.Tables[1];
					grd_Batch.RefreshDataSource();
				}
				gv_Batch.SelectAll();
				FormMode = Enums.FormViewModeType.Update;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
			txt_Batch.Text = string.Empty;
			txt_Batch.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		try
		{
			grd_Batch.DataSource = null;
			bte_BIN.EditValue = "";
			txt_Batch.Text = "";
			bte_BIN.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task VirFormDocumentForBatch_Load(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = await _httpClient.Commons.GetServerDate();
			date_Doc.EditValue = dateTime;
			GetDate = dateTime;
			ResetData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ResetData()
	{
		try
		{
			bte_BIN.Text = "";
			txt_Batch.Text = "";
			txt_Batch.Enabled = false;
			FormMode = Enums.FormViewModeType.View;
			bte_BIN.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			int[] selectedRows = gv_Batch.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				DataTable dataTable = grd_Batch.DataSourceTable.Clone();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow.ItemArray = gv_Batch.GetDataRow(selectedRows[i]).ItemArray;
					dataTable.Rows.Add(dataRow);
				}
				DocumentsImpl documentsImpl = new DocumentsImpl
				{
					BPLId = GlobalData.DefaultBranch,
					Comments = "Receipt Issue Bin",
					Lines = new List<Document_LinesImpl>()
				};
				var enumerable = from t in dataTable.AsEnumerable()
					group t by new
					{
						ItemCode = Function.ToString(t["ItemCode"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					};
				int num = 0;
				foreach (var item2 in enumerable)
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl
					{
						BatchNumbers = new List<BatchNumbersImpl>(),
						BinAllocations = new List<DocumentLinesBinAllocationsImpl>()
					};
					double num2 = 0.0;
					List<DataColumn> list = (from DataColumn t in dataTable.Columns
						where t.ColumnName.StartsWith("U_")
						select (t)).ToList();
					int num3 = 0;
					foreach (DataRow item3 in item2.Items)
					{
						document_LinesImpl.Price = Function.ParseDouble(item3["Price"]);
						document_LinesImpl.Currency = Function.ToString(item3["Currency"]);
						if (string.IsNullOrEmpty(document_LinesImpl.ItemCode))
						{
							document_LinesImpl.ItemCode = Function.ToString(item3["ItemCode"]);
						}
						if (string.IsNullOrEmpty(document_LinesImpl.WhsCode))
						{
							document_LinesImpl.WhsCode = Function.ToString(item3["WhsCode"]);
						}
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
						{
							Quantity = Function.ParseDouble(item3["Quantity"]),
							BatchNumber = Function.ToString(item3["DistNumber"]),
							InternalSerialNumber = Function.ToString(item3["LotNumber"]),
							AddmisionDate = Function.ParseDateTime(item3["InDate"]),
							BaseLineNumber = num
						};
						int num4 = Function.ParseInt(item3["BinId"]);
						if (list.Count > 0)
						{
							batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
							for (int num5 = 0; num5 < list.Count; num5++)
							{
								UserFieldsImpl userFieldsImpl = new UserFieldsImpl
								{
									Name = list[num5].ColumnName
								};
								if (list[num5].DataType == typeof(DateTime))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Date;
								}
								else if (list[num5].DataType == typeof(double) || list[num5].DataType == typeof(decimal))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
									userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
								}
								else if (list[num5].DataType == typeof(int) || list[num5].DataType == typeof(short) || list[num5].DataType == typeof(long))
								{
									userFieldsImpl.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl.Value = item3[list[num5]];
								batchNumbersImpl.UserFields.Add(userFieldsImpl);
							}
						}
						num2 += batchNumbersImpl.Quantity;
						document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
						if (num4 > 0)
						{
							DocumentLinesBinAllocationsImpl item = new DocumentLinesBinAllocationsImpl
							{
								BaseLineNumber = num,
								BinAbsEntry = num4,
								Quantity = batchNumbersImpl.Quantity,
								SerialAndBatchNumbersBaseLine = num3
							};
							document_LinesImpl.BinAllocations.Add(item);
							num3++;
						}
					}
					document_LinesImpl.Quantity = num2;
					documentsImpl.Lines.Add(document_LinesImpl);
					num++;
				}
				HttpResult httpResult = await _httpClient.ReceiptIssueBin.SaveDoc(documentsImpl, FormObjectType);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					txt_DocNo.EditValue = httpResult.Content;
					grd_Batch.DataSource = null;
					grd_Batch.RefreshDataSource();
					PRINTLAYOUT();
				}
			}
			else
			{
				XtraMessageBox.Show("Please choose item", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		bte_BIN.Text = "";
		bte_BIN.Focus();
	}

	private void bte_BIN_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				txt_Batch.Enabled = true;
				txt_Batch.Focus();
				txt_Batch.Text = string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNo.Text);
			if (num > 0)
			{
				await WinFuntions.PrintLayout("", num, (FormObjectType == ApzObjectTypes.oInventoryGenEntry) ? "IGN1" : "IGE1", ((int)FormObjectType).ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNo.EditValue);
			await WinFuntions.PreviewLayout("", num, (FormObjectType == ApzObjectTypes.oInventoryGenEntry) ? "IGN1" : "IGE2", this, ((int)FormObjectType).ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormInventory.VirReceiptIssueBin));
		this.backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Batch = new Apzon.Controls.Editor.TextEditEx();
		this.date_Doc = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bte_BIN = new Apzon.Controls.Editor.ButtonEditEx();
		this.BIN = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).BeginInit();
		base.SuspendLayout();
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Clear";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton2.ImageOptions.Image");
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(364, 35);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 20);
		this.simpleButton2.TabIndex = 41;
		this.simpleButton2.Text = "Clear";
		this.simpleButton2.Click += new System.EventHandler(this.simpleButton2_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(1, 438);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 37;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = null;
		this.gv_Batch.DisableFormSetting = false;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.EditColumn = null;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oDocumentForBatch";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IgnoreFilter = false;
		this.gv_Batch.IndexEditRow = -1;
		this.gv_Batch.IndicatorWidth = 35;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.IsMappingContextAction = false;
		this.gv_Batch.ItemId = "OBATCH";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
		this.gv_Batch.OptionsSelection.MultiSelect = true;
		this.gv_Batch.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Batch.OptionsView.ShowGroupPanel = false;
		this.gv_Batch.UseBatchMod = false;
		this.grd_Batch.AllowPasteExcel = true;
		this.grd_Batch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.Location = new System.Drawing.Point(0, 63);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.Size = new System.Drawing.Size(876, 368);
		this.grd_Batch.TabIndex = 36;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Batch";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 37);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(105, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 35;
		this.labelLine4.Utext = "Batch";
		this.txt_Batch.EditControl = null;
		this.txt_Batch.FieldName = null;
		this.txt_Batch.IsSearchControl = false;
		this.txt_Batch.Location = new System.Drawing.Point(117, 35);
		this.txt_Batch.Name = "txt_Batch";
		this.txt_Batch.SettingInVisible = false;
		this.txt_Batch.SettingReadonly = false;
		this.txt_Batch.Size = new System.Drawing.Size(241, 20);
		this.txt_Batch.TabIndex = 34;
		this.txt_Batch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Batch_KeyDown);
		this.date_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Doc.EditControl = null;
		this.date_Doc.EditValue = null;
		this.date_Doc.Enabled = false;
		this.date_Doc.FieldName = null;
		this.date_Doc.Location = new System.Drawing.Point(726, 36);
		this.date_Doc.Name = "date_Doc";
		this.date_Doc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Doc.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_Doc.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Doc.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_Doc.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_Doc.Properties.IsSearchControl = false;
		this.date_Doc.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_Doc.SettingInVisible = false;
		this.date_Doc.SettingReadonly = false;
		this.date_Doc.Size = new System.Drawing.Size(142, 20);
		this.date_Doc.TabIndex = 32;
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(647, 39);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 31;
		this.labelLine3.Utext = "Doc Date";
		this.txt_DocNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(726, 11);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(142, 20);
		this.txt_DocNo.TabIndex = 28;
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Doc No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(647, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 27;
		this.labelLine1.Utext = "Doc No.";
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButton1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Cancel";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(82, 438);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(75, 20);
		this.simpleButton1.TabIndex = 42;
		this.simpleButton1.Text = "Cancel";
		this.bte_BIN.EditControl = null;
		this.bte_BIN.FieldName = null;
		this.bte_BIN.IsSearchControl = false;
		this.bte_BIN.Location = new System.Drawing.Point(117, 8);
		this.bte_BIN.Name = "bte_BIN";
		this.bte_BIN.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_BIN.SettingInVisible = false;
		this.bte_BIN.SettingReadonly = false;
		this.bte_BIN.Size = new System.Drawing.Size(241, 20);
		this.bte_BIN.TabIndex = 44;
		this.bte_BIN.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_BIN_KeyDown);
		this.BIN.EditControl = null;
		this.BIN.EditValue = "BIN";
		this.BIN.FieldName = null;
		this.BIN.HideWhenEmpty = false;
		this.BIN.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.BIN.Location = new System.Drawing.Point(12, 9);
		this.BIN.Name = "BIN";
		this.BIN.ReadOnly = false;
		this.BIN.SettingInVisible = false;
		this.BIN.SettingReadonly = false;
		this.BIN.ShowUnderLine = true;
		this.BIN.Size = new System.Drawing.Size(105, 20);
		this.BIN.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.BIN.TabIndex = 43;
		this.BIN.Utext = "BIN";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.simpleButton1;
		base.ClientSize = new System.Drawing.Size(877, 460);
		base.Controls.Add(this.bte_BIN);
		base.Controls.Add(this.BIN);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.simpleButton2);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Batch);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_Batch);
		base.Controls.Add(this.date_Doc);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oDocumentForBatch";
		base.IconOptions.ShowIcon = false;
		base.Name = "VirReceiptIssueBin";
		base.ShowInTaskbar = false;
		this.Text = "Document For Batch";
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Doc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_BIN.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
