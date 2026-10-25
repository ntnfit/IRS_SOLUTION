using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Documents;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Production;

public class frm_IssueAndReceiptLackQtyForProduction : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataSet _ds = new DataSet();

	private System.Windows.Forms.Timer _imporFgtTimer = new System.Windows.Forms.Timer();

	private int _totalImportFgTime;

	private int _totals;

	private int _proccessedFg;

	private int _packQty = 1;

	private int _sleepTime = 5;

	private string _txtElapsedTime = "Elapsed Time - ";

	private string _saveErrors = "";

	private DataTable _distinctValues = new DataTable();

	private IContainer components;

	private LabelLine labelLine1;

	private DateEditEx date_Search;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButton btn_Save;

	private SimpleButton btn_Find;

	private BackgroundWorker bgw_Main;

	private Label lbl_ImportStatus;

	private System.Windows.Forms.ProgressBar prg_ProcessFg;

	private Label lbl_ImportFgElapseTime;

	private CalcEdit calc_FGInteruptTime;

	private LabelLine labelLine3;

	public frm_IssueAndReceiptLackQtyForProduction()
	{
		InitializeComponent();
	}

	private void frm_IssueAndReceiptLackQtyForProduction_Load(object sender, EventArgs e)
	{
		try
		{
			FormId = "IssueNrecQtyPrd";
			gv_Doc.FormId = FormId;
			gv_Doc.ItemId = "gvDoc";
			calc_FGInteruptTime.EditValue = 5;
			date_Search.EditValue = DateTime.Today;
			prg_ProcessFg.Minimum = 0;
			prg_ProcessFg.Step = 1;
			_imporFgtTimer.Interval = 1000;
			_imporFgtTimer.Tick += _imporFgtTimer_Tick;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _imporFgtTimer_Tick(object sender, EventArgs e)
	{
		try
		{
			_totalImportFgTime++;
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			num = Math.Floor((double)(_totalImportFgTime / 3600));
			num2 = Math.Floor(((double)_totalImportFgTime - num * 3600.0) / 60.0);
			num3 = (double)_totalImportFgTime - (num * 3600.0 + num2 * 60.0);
			lbl_ImportFgElapseTime.Text = _txtElapsedTime + num + ":" + num2 + ":" + num3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Find_Click(object sender, EventArgs e)
	{
		Search();
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		btn_Find.Enabled = false;
		btn_Save.Enabled = false;
		try
		{
			if (_ds != null && _ds.Tables.Count == 3)
			{
				DataTable table = _ds.Tables[0];
				DataTable dataTable = _ds.Tables[1];
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					_sleepTime = Function.ParseInt(calc_FGInteruptTime.EditValue);
					DataTable dataTable2 = new DataView(table).ToTable(true, "DocID");
					DataTable dataTable3 = new DataView(dataTable).ToTable(true, "DocID");
					_totals = dataTable2.Rows.Count + dataTable3.Rows.Count;
					lbl_ImportStatus.Visible = true;
					lbl_ImportFgElapseTime.Visible = true;
					prg_ProcessFg.Maximum = _totals;
					prg_ProcessFg.Value = 0;
					prg_ProcessFg.Visible = true;
					lbl_ImportStatus.Text = "0/" + _totals + " rows.";
					_totalImportFgTime = 0;
					_proccessedFg = 0;
					_imporFgtTimer.Start();
					bgw_Main.RunWorkerAsync();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
					btn_Find.Enabled = true;
					btn_Save.Enabled = true;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				btn_Find.Enabled = true;
				btn_Save.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bgw_Main_DoWork(object sender, DoWorkEventArgs e)
	{
		Save();
	}

	private void bgw_Main_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		try
		{
			int proccessedFg = ((_proccessedFg + _packQty > _totals) ? _totals : (_proccessedFg + _packQty));
			_proccessedFg = proccessedFg;
			prg_ProcessFg.Value = _proccessedFg;
			lbl_ImportStatus.Text = _proccessedFg + "/" + _totals + " rows.";
			lbl_ImportStatus.Update();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bgw_Main_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			if (!string.IsNullOrWhiteSpace(_saveErrors))
			{
				ShowErrorResultForm();
			}
			WinCommons.Success(Enums.MessageCode.IssueAndReceiptCompleted, "");
			_saveErrors = "";
			_imporFgtTimer.Stop();
			btn_Find.Enabled = true;
			btn_Save.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Save()
	{
		try
		{
			if (_ds == null || _ds.Tables.Count != 3)
			{
				return;
			}
			List<DocumentsImpl> lstDocs = new List<DocumentsImpl>();
			DateTime? dateTime = Function.ParseDateTime(date_Search.EditValue);
			DataTable dataTable = _ds.Tables[0];
			DataTable dataTable2 = new DataView(dataTable).ToTable(true, "DocID");
			for (int i = 0; i < dataTable2.Rows.Count; i++)
			{
				int docID = Function.ParseInt(dataTable2.Rows[i]["DocID"]);
				DocumentsImpl documentsImpl = new DocumentsImpl();
				documentsImpl.DocType = "I";
				documentsImpl.ObjType = 59;
				documentsImpl.DocDate = dateTime.Value;
				List<DataRow> list = (from r in dataTable.AsEnumerable()
					where Function.ParseInt(r["DocID"]) == docID
					select r).ToList();
				if (list.Count > 0)
				{
					documentsImpl.Comments = Function.ToString(list[0]["Note"]);
					documentsImpl.JrnlMemo = documentsImpl.Comments;
					documentsImpl.Lines = new List<Document_LinesImpl>();
					foreach (DataRow item in list)
					{
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
						document_LinesImpl.ItemCode = Function.ToString(item["ItemCode"]);
						document_LinesImpl.Quantity = Function.ParseDouble(item["NeedReceiptQty"]);
						int num = (document_LinesImpl.LineNum = Function.ParseInt(item["LineID"]));
						document_LinesImpl.BaseType = -1;
						document_LinesImpl.WhsCode = Function.ToString(item["Warehouse"]);
						document_LinesImpl.AcctCode = Function.ToString(item["GLAccount"]);
						bool flag = Function.ToString(item["ManBtch"]).ToLower() == "y";
						if (flag)
						{
							BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
							batchNumbersImpl.AddmisionDate = DateTime.Today;
							batchNumbersImpl.BatchNumber = Function.ToString(item["BatchNo"]);
							batchNumbersImpl.Quantity = document_LinesImpl.Quantity;
							batchNumbersImpl.BaseLineNumber = num;
							document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
							document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
						}
						int num3 = Function.ParseInt(item["DftBin"]);
						if (num3 > 0)
						{
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl.BaseLineNumber = num;
							documentLinesBinAllocationsImpl.BinAbsEntry = num3;
							documentLinesBinAllocationsImpl.Quantity = document_LinesImpl.Quantity;
							if (flag)
							{
								documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = 0;
							}
							document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
							document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
						}
						documentsImpl.Lines.Add(document_LinesImpl);
						num++;
					}
				}
				lstDocs.Add(documentsImpl);
			}
			DataTable dataTable3 = _ds.Tables[1];
			DataTable source = _ds.Tables[2];
			DataTable dataTable4 = new DataView(dataTable3).ToTable(true, "DocID");
			for (int num4 = 0; num4 < dataTable4.Rows.Count; num4++)
			{
				int docID2 = Function.ParseInt(dataTable4.Rows[num4]["DocID"]);
				DocumentsImpl documentsImpl2 = new DocumentsImpl();
				documentsImpl2.DocType = "I";
				documentsImpl2.ObjType = 60;
				documentsImpl2.DocDate = dateTime.Value;
				List<DataRow> list2 = (from r in dataTable3.AsEnumerable()
					where Function.ParseInt(r["DocID"]) == docID2
					select r).ToList();
				if (list2.Count > 0)
				{
					documentsImpl2.Comments = Function.ToString(list2[0]["Note"]);
					documentsImpl2.JrnlMemo = documentsImpl2.Comments;
					documentsImpl2.Lines = new List<Document_LinesImpl>();
					foreach (DataRow item2 in list2)
					{
						int lineNum = Function.ParseInt(item2["DocLineID"]);
						Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
						document_LinesImpl2.ItemCode = Function.ToString(item2["ItemCode"]);
						document_LinesImpl2.Quantity = Function.ParseDouble(item2["NotIssueInDay"]);
						document_LinesImpl2.LineNum = lineNum;
						document_LinesImpl2.WhsCode = Function.ToString(item2["Warehouse"]);
						document_LinesImpl2.BaseType = 202;
						document_LinesImpl2.BaseEntry = Function.ParseInt(item2["PrdID"]);
						document_LinesImpl2.BaseLine = Function.ParseInt(item2["PrdLineID"]);
						bool flag2 = Function.ToString(item2["ManBtch"]).ToLower() == "y";
						document_LinesImpl2.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
						if (flag2)
						{
							document_LinesImpl2.BatchNumbers = new List<BatchNumbersImpl>();
							List<DataRow> list3 = (from r in source.AsEnumerable()
								where Function.ParseInt(r["DocID"]) == docID2 && Function.ParseInt(r["DocLineID"]) == lineNum
								select r).ToList();
							if (list3.Count > 0)
							{
								int num5 = 0;
								foreach (DataRow item3 in list3)
								{
									BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl();
									batchNumbersImpl2.BatchNumber = Function.ToString(item3["BatchNo"]);
									batchNumbersImpl2.Quantity = Function.ParseDouble(item3["Qty"]);
									batchNumbersImpl2.BaseLineNumber = lineNum;
									document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl2);
									int num6 = Function.ParseInt(item3["BinID"]);
									if (num6 > 0)
									{
										DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
										documentLinesBinAllocationsImpl2.BaseLineNumber = lineNum;
										documentLinesBinAllocationsImpl2.BinAbsEntry = num6;
										documentLinesBinAllocationsImpl2.Quantity = document_LinesImpl2.Quantity;
										documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = num5;
										document_LinesImpl2.BinAllocations.Add(documentLinesBinAllocationsImpl2);
									}
									num5++;
								}
							}
						}
						documentsImpl2.Lines.Add(document_LinesImpl2);
						lineNum++;
					}
				}
				lstDocs.Add(documentsImpl2);
			}
			if (lstDocs.Count <= 0)
			{
				return;
			}
			int num7 = 1;
			int i2;
			for (i2 = 0; i2 < lstDocs.Count; i2++)
			{
				string result = Task.Run(async () =>
				{
					try
					{
						return await _httpClient.OutsourcingClient.SaveDoc(new List<DocumentsImpl> { lstDocs[i2] });
					}
					catch (Exception ex2)
					{
						MessageBox.Show(ex2.Message);
						return ex2.Message;
					}
				}).Result;
				if (!string.IsNullOrEmpty(result))
				{
					if (lstDocs[i2].ObjType == 60)
					{
						_saveErrors = _saveErrors + num7 + ". (" + lstDocs[i2].Lines[0].BaseEntry + ") " + result + Environment.NewLine;
					}
					else
					{
						_saveErrors = _saveErrors + num7 + ". (Receipt) " + result + Environment.NewLine;
					}
					num7++;
				}
				bgw_Main.ReportProgress(1);
				if (i2 != lstDocs.Count - 1)
				{
					Thread.Sleep(_sleepTime * 1000);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Search()
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(date_Search.EditValue);
			if (dateTime.HasValue)
			{
				_ds = _httpClient.ProductionOrderImportTool.IssNdRecQtySearch(dateTime.Value.ToString("yyyyMMdd"));
				if (_ds != null && _ds.Tables.Count == 3)
				{
					grd_Doc.DataSource = _ds.Tables[1].Copy();
					grd_Doc.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowErrorResultForm()
	{
		try
		{
			XtraForm frm = new XtraForm();
			try
			{
				frm.Text = "Issue Result";
				frm.FormBorderStyle = FormBorderStyle.FixedSingle;
				frm.MaximizeBox = false;
				frm.Width = 410;
				frm.Height = 300;
				TextBox textBox = new TextBox();
				textBox.Multiline = true;
				textBox.Location = new Point(5, 5);
				textBox.Size = new Size(390, 235);
				textBox.ScrollBars = ScrollBars.Both;
				textBox.WordWrap = false;
				textBox.Text = _saveErrors;
				textBox.ReadOnly = true;
				frm.Controls.Add(textBox);
				SimpleButton simpleButton = new SimpleButton();
				simpleButton.Location = new Point(170, 243);
				simpleButton.Size = new Size(60, 20);
				simpleButton.Text = "Close";
				simpleButton.Click += (object s, EventArgs e1) =>
				{
					frm.Close();
				};
				frm.Controls.Add(simpleButton);
				frm.ShowIcon = false;
				frm.ShowDialog();
			}
			finally
			{
				if (frm != null)
				{
					((IDisposable)frm).Dispose();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_IssueAndReceiptLackQtyForProduction));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_Search = new Apzon.Controls.Editor.DateEditEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Save = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Find = new DevExpress.XtraEditors.SimpleButton();
		this.bgw_Main = new System.ComponentModel.BackgroundWorker();
		this.lbl_ImportStatus = new System.Windows.Forms.Label();
		this.prg_ProcessFg = new System.Windows.Forms.ProgressBar();
		this.lbl_ImportFgElapseTime = new System.Windows.Forms.Label();
		this.calc_FGInteruptTime = new DevExpress.XtraEditors.CalcEdit();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.date_Search.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Search.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_FGInteruptTime.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(20, 16);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Date";
		this.date_Search.EditControl = null;
		this.date_Search.EditValue = null;
		this.date_Search.FieldName = null;
		this.date_Search.Location = new System.Drawing.Point(97, 13);
		this.date_Search.Name = "date_Search";
		this.date_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Search.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Search.Properties.IsSearchControl = false;
		this.date_Search.SettingInVisible = false;
		this.date_Search.SettingReadonly = false;
		this.date_Search.Size = new System.Drawing.Size(100, 20);
		this.date_Search.TabIndex = 1;
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(20, 43);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(917, 438);
		this.grd_Doc.TabIndex = 2;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oIssuseAndReceiptLackQuantityFP";
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
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.Location = new System.Drawing.Point(20, 498);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 3;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Find.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Find.ImageOptions.Image");
		this.btn_Find.Location = new System.Drawing.Point(217, 11);
		this.btn_Find.Name = "btn_Find";
		this.btn_Find.Size = new System.Drawing.Size(75, 23);
		this.btn_Find.TabIndex = 4;
		this.btn_Find.Text = "Search";
		this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
		this.bgw_Main.WorkerReportsProgress = true;
		this.bgw_Main.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Main_DoWork);
		this.bgw_Main.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_Main_ProgressChanged);
		this.bgw_Main.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Main_RunWorkerCompleted);
		this.lbl_ImportStatus.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ImportStatus.AutoSize = true;
		this.lbl_ImportStatus.Location = new System.Drawing.Point(511, 505);
		this.lbl_ImportStatus.Name = "lbl_ImportStatus";
		this.lbl_ImportStatus.Size = new System.Drawing.Size(0, 13);
		this.lbl_ImportStatus.TabIndex = 22;
		this.prg_ProcessFg.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.prg_ProcessFg.Location = new System.Drawing.Point(290, 496);
		this.prg_ProcessFg.Name = "prg_ProcessFg";
		this.prg_ProcessFg.Size = new System.Drawing.Size(217, 23);
		this.prg_ProcessFg.TabIndex = 21;
		this.lbl_ImportFgElapseTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ImportFgElapseTime.AutoSize = true;
		this.lbl_ImportFgElapseTime.Location = new System.Drawing.Point(141, 505);
		this.lbl_ImportFgElapseTime.Name = "lbl_ImportFgElapseTime";
		this.lbl_ImportFgElapseTime.Size = new System.Drawing.Size(69, 13);
		this.lbl_ImportFgElapseTime.TabIndex = 23;
		this.lbl_ImportFgElapseTime.Text = "Elapsed Time";
		this.calc_FGInteruptTime.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.calc_FGInteruptTime.Location = new System.Drawing.Point(877, 498);
		this.calc_FGInteruptTime.Name = "calc_FGInteruptTime";
		this.calc_FGInteruptTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_FGInteruptTime.Size = new System.Drawing.Size(60, 20);
		this.calc_FGInteruptTime.TabIndex = 25;
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Interrupt time (s)";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(707, 501);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(169, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 24;
		this.labelLine3.Utext = "Interrupt time (s)";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(952, 533);
		base.Controls.Add(this.calc_FGInteruptTime);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.lbl_ImportFgElapseTime);
		base.Controls.Add(this.lbl_ImportStatus);
		base.Controls.Add(this.prg_ProcessFg);
		base.Controls.Add(this.btn_Find);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.date_Search);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oIssuseAndReceiptLackQuantityFP";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_IssueAndReceiptLackQtyForProduction";
		this.Text = "Issue And Receipt Lack Quantity For Production";
		base.Load += new System.EventHandler(this.frm_IssueAndReceiptLackQtyForProduction_Load);
		((System.ComponentModel.ISupportInitialize)this.date_Search.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Search.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_FGInteruptTime.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
