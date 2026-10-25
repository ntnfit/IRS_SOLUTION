using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using SAPbobsCOM;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryCountingReport : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine lb_Period;

	private LookUpEdit look_Period;

	private LabelLine labelLine1;

	private ButtonEditEx bte_Bin;

	private SimpleButtonEx btn_Load;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Posting;

	public DateTime GetDate { get; set; }

	public frm_InventoryCountingReport()
	{
		InitializeComponent();
	}

	private async void frm_InventoryCountingReport_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataTable dataSource = await _httpClient.InventoryCountingBin.GetPeriods();
			look_Period.Properties.DataSource = dataSource;
			look_Period.Properties.DisplayMember = "PeriodName";
			look_Period.Properties.ValueMember = "PeriodCode";
			GetDate = await _httpClient.Commons.GetServerDate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_Bin_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bin, multiSelect: false, Function.ToString(bte_Bin.EditValue));
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				string editValue = Function.ToString(((DataRowView)frm_ChooseFromList2.ChooseItem)["BinCode"]);
				bte_Bin.EditValue = editValue;
				SendKeys.Send("{Enter}");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Load_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			string period = Function.ToString(look_Period.EditValue);
			string bin = Function.ToString(bte_Bin.EditValue);
			DataSet dataSet = await _httpClient.InventoryCountingBin.GetCountingReport(period, bin);
			if (dataSet.IsHasValidValue())
			{
				if (grd_Doc.DataSourceTable.Rows.Count <= 0)
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2], null, expand: true);
				}
				else
				{
					grd_Doc.DataSource = dataSet.Tables[1];
					grd_Doc.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async void btn_Posting_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_Doc.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DocumentsImpl documentsImpl = new DocumentsImpl();
				documentsImpl.DocDate = GetDate;
				documentsImpl.TaxDate = documentsImpl.DocDate;
				documentsImpl.DocDueDate = documentsImpl.DocDate;
				documentsImpl.Lines = new List<Document_LinesImpl>();
				documentsImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				int num = 0;
				DocumentsImpl documentsImpl2 = new DocumentsImpl();
				documentsImpl2.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
				documentsImpl2.Lines = new List<Document_LinesImpl>();
				int num2 = 0;
				DocumentsImpl documentsImpl3 = new DocumentsImpl();
				documentsImpl3.BPLId = Function.ParseInt(GlobalData.DefaultBranch);
				documentsImpl3.Lines = new List<Document_LinesImpl>();
				int num3 = 0;
				for (int i = 0; i < selectedRows.Length; i++)
				{
					switch (Function.ToString(gv_Doc.GetRowCellValue(i, "U_Status")))
					{
					case "SB":
					{
						int binAbsEntry = Function.ParseInt(gv_Doc.GetRowCellValue(i, "U_LogicBinId"));
						int num4 = Function.ParseInt(gv_Doc.GetRowCellValue(i, "U_ActualBinId"));
						if (!binAbsEntry.Equals(num4))
						{
							Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
							document_LinesImpl2.UserFields = new List<UserFieldsImpl>();
							UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
							userFieldsImpl2.Name = "U_IncDocEntry";
							userFieldsImpl2.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "DocEntry"));
							document_LinesImpl2.UserFields.Add(userFieldsImpl2);
							userFieldsImpl2 = new UserFieldsImpl();
							userFieldsImpl2.Name = "U_IncLineId";
							userFieldsImpl2.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "LineId"));
							document_LinesImpl2.UserFields.Add(userFieldsImpl2);
							document_LinesImpl2.ItemCode = Function.ToString(gv_Doc.GetRowCellValue(i, "U_ItemCode"));
							document_LinesImpl2.FromWhsCod = Function.ToString(gv_Doc.GetRowCellValue(i, "U_LogicWhs"));
							if (string.IsNullOrEmpty(documentsImpl.Filler))
							{
								documentsImpl.Filler = document_LinesImpl2.FromWhsCod;
							}
							document_LinesImpl2.WhsCode = Function.ToString(gv_Doc.GetRowCellValue(i, "U_ActualWhs"));
							if (string.IsNullOrEmpty(documentsImpl.ToWhsCode))
							{
								documentsImpl.ToWhsCode = document_LinesImpl2.WhsCode;
							}
							document_LinesImpl2.Quantity = Function.ParseDouble(gv_Doc.GetRowCellValue(i, "U_Qty"));
							document_LinesImpl2.BatchNumbers = new List<BatchNumbersImpl>();
							BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl();
							batchNumbersImpl2.BatchNumber = Function.ToString(gv_Doc.GetRowCellValue(i, "U_DistNumber"));
							batchNumbersImpl2.Quantity = document_LinesImpl2.Quantity;
							batchNumbersImpl2.BaseLineNumber = num;
							document_LinesImpl2.BatchNumbers.Add(batchNumbersImpl2);
							document_LinesImpl2.LineNum = num;
							documentsImpl.Lines.Add(document_LinesImpl2);
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl2 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl2.BinAbsEntry = binAbsEntry;
							documentLinesBinAllocationsImpl2.BaseLineNumber = num;
							documentLinesBinAllocationsImpl2.BinActionType = BinActionTypeEnum.batFromWarehouse;
							documentLinesBinAllocationsImpl2.Quantity = document_LinesImpl2.Quantity;
							documentLinesBinAllocationsImpl2.SerialAndBatchNumbersBaseLine = 0;
							if (documentLinesBinAllocationsImpl2.BinAbsEntry > 0)
							{
								documentsImpl.BinAllocations.Add(documentLinesBinAllocationsImpl2);
							}
							DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl3 = new DocumentLinesBinAllocationsImpl();
							documentLinesBinAllocationsImpl3.BinAbsEntry = num4;
							documentLinesBinAllocationsImpl3.BaseLineNumber = num;
							documentLinesBinAllocationsImpl3.BinActionType = BinActionTypeEnum.batToWarehouse;
							documentLinesBinAllocationsImpl3.Quantity = document_LinesImpl2.Quantity;
							documentLinesBinAllocationsImpl3.SerialAndBatchNumbersBaseLine = 0;
							if (documentLinesBinAllocationsImpl3.BinAbsEntry > 0)
							{
								documentsImpl.BinAllocations.Add(documentLinesBinAllocationsImpl3);
							}
							num++;
						}
						break;
					}
					case "T1":
					{
						Document_LinesImpl document_LinesImpl3 = new Document_LinesImpl();
						document_LinesImpl3.UserFields = new List<UserFieldsImpl>();
						UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl();
						userFieldsImpl3.Name = "U_IncDocEntry";
						userFieldsImpl3.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "DocEntry"));
						document_LinesImpl3.UserFields.Add(userFieldsImpl3);
						userFieldsImpl3 = new UserFieldsImpl();
						userFieldsImpl3.Name = "U_IncLineId";
						userFieldsImpl3.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "LineId"));
						document_LinesImpl3.UserFields.Add(userFieldsImpl3);
						document_LinesImpl3.Quantity = Function.ParseDouble(gv_Doc.GetRowCellValue(i, "U_Qty"));
						document_LinesImpl3.ItemCode = Function.ToString(gv_Doc.GetRowCellValue(i, "U_ItemCode"));
						document_LinesImpl3.WhsCode = Function.ToString(gv_Doc.GetRowCellValue(i, "U_ActualWhs"));
						document_LinesImpl3.BaseType = -1;
						document_LinesImpl3.BatchNumbers = new List<BatchNumbersImpl>();
						BatchNumbersImpl batchNumbersImpl3 = new BatchNumbersImpl();
						batchNumbersImpl3.Quantity = document_LinesImpl3.Quantity;
						batchNumbersImpl3.BatchNumber = Function.ToString(gv_Doc.GetRowCellValue(i, "U_DistNumber"));
						batchNumbersImpl3.BaseLineNumber = num3;
						batchNumbersImpl3.UserFields = new List<UserFieldsImpl>();
						UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl();
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Name = "U_QtyUoM";
						userFieldsImpl4.Value = Function.ParseDouble(gv_Doc.GetRowCellValue(i, "U_QtyUoM"));
						batchNumbersImpl3.UserFields.Add(userFieldsImpl4);
						userFieldsImpl4 = new UserFieldsImpl();
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Name = "U_Width";
						userFieldsImpl4.Value = Function.ParseDouble(gv_Doc.GetRowCellValue(i, "U_Width"));
						batchNumbersImpl3.UserFields.Add(userFieldsImpl4);
						userFieldsImpl4 = new UserFieldsImpl();
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Name = "U_Length";
						userFieldsImpl4.Value = Function.ParseDouble(gv_Doc.GetRowCellValue(i, "U_Length"));
						batchNumbersImpl3.UserFields.Add(userFieldsImpl4);
						userFieldsImpl4 = new UserFieldsImpl();
						userFieldsImpl4.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl4.Name = "U_UnitUoM";
						userFieldsImpl4.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "U_UnitUoM"));
						batchNumbersImpl3.UserFields.Add(userFieldsImpl4);
						document_LinesImpl3.BatchNumbers.Add(batchNumbersImpl3);
						document_LinesImpl3.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl4 = new DocumentLinesBinAllocationsImpl();
						documentLinesBinAllocationsImpl4.BinAbsEntry = Function.ParseInt(gv_Doc.GetRowCellValue(i, "U_ActualBinId"));
						documentLinesBinAllocationsImpl4.BaseLineNumber = num3;
						documentLinesBinAllocationsImpl4.Quantity = document_LinesImpl3.Quantity;
						documentLinesBinAllocationsImpl4.SerialAndBatchNumbersBaseLine = 0;
						if (documentLinesBinAllocationsImpl4.BinAbsEntry > 0)
						{
							document_LinesImpl3.BinAllocations.Add(documentLinesBinAllocationsImpl4);
						}
						documentsImpl3.Lines.Add(document_LinesImpl3);
						num3++;
						break;
					}
					case "T2":
					{
						Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
						document_LinesImpl.UserFields = new List<UserFieldsImpl>();
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = "U_IncDocEntry";
						userFieldsImpl.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "DocEntry"));
						document_LinesImpl.UserFields.Add(userFieldsImpl);
						userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = "U_IncLineId";
						userFieldsImpl.Value = Function.ToString(gv_Doc.GetRowCellValue(i, "LineId"));
						document_LinesImpl.UserFields.Add(userFieldsImpl);
						document_LinesImpl.Quantity = Function.ParseDouble(gv_Doc.GetRowCellValue(i, "U_Qty"));
						document_LinesImpl.ItemCode = Function.ToString(gv_Doc.GetRowCellValue(i, "U_ItemCode"));
						document_LinesImpl.WhsCode = Function.ToString(gv_Doc.GetRowCellValue(i, "U_ActualWhs"));
						document_LinesImpl.BaseType = -1;
						document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
						BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl();
						batchNumbersImpl.Quantity = document_LinesImpl.Quantity;
						batchNumbersImpl.BatchNumber = Function.ToString(gv_Doc.GetRowCellValue(i, "U_DistNumber"));
						batchNumbersImpl.BaseLineNumber = num2;
						document_LinesImpl.BatchNumbers.Add(batchNumbersImpl);
						document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
						DocumentLinesBinAllocationsImpl documentLinesBinAllocationsImpl = new DocumentLinesBinAllocationsImpl();
						documentLinesBinAllocationsImpl.BinAbsEntry = Function.ParseInt(gv_Doc.GetRowCellValue(i, "U_ActualBinId"));
						documentLinesBinAllocationsImpl.BaseLineNumber = num2;
						documentLinesBinAllocationsImpl.Quantity = document_LinesImpl.Quantity;
						documentLinesBinAllocationsImpl.SerialAndBatchNumbersBaseLine = 0;
						if (documentLinesBinAllocationsImpl.BinAbsEntry > 0)
						{
							document_LinesImpl.BinAllocations.Add(documentLinesBinAllocationsImpl);
						}
						documentsImpl2.Lines.Add(document_LinesImpl);
						num2++;
						break;
					}
					}
				}
				HttpResult httpResult = await _httpClient.InventoryCountingBin.Posting(documentsImpl, documentsImpl3, documentsImpl2);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					XtraMessageBox.Show(httpResult.Message, "Error");
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Load_Click(null, null);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, "Chọn dòng để thực hiện thao tác.");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCountingReport));
		this.lb_Period = new Apzon.Controls.Controls.LabelLine();
		this.look_Period = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.bte_Bin = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Load = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Posting = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.look_Period.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Bin.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		base.SuspendLayout();
		this.lb_Period.EditControl = null;
		this.lb_Period.EditValue = "Period";
		this.lb_Period.FieldName = null;
		this.lb_Period.HideWhenEmpty = false;
		this.lb_Period.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Period.Location = new System.Drawing.Point(3, 13);
		this.lb_Period.Name = "lb_Period";
		this.lb_Period.ReadOnly = false;
		this.lb_Period.SettingInVisible = false;
		this.lb_Period.SettingReadonly = false;
		this.lb_Period.ShowUnderLine = true;
		this.lb_Period.Size = new System.Drawing.Size(80, 20);
		this.lb_Period.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Period.TabIndex = 0;
		this.lb_Period.Utext = "Period";
		this.look_Period.Location = new System.Drawing.Point(80, 10);
		this.look_Period.Name = "look_Period";
		this.look_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Period.Properties.NullText = "";
		this.look_Period.Size = new System.Drawing.Size(120, 20);
		this.look_Period.TabIndex = 1;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Bin Location";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(206, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.Utext = "Bin Location";
		this.bte_Bin.EditControl = null;
		this.bte_Bin.FieldName = null;
		this.bte_Bin.IsSearchControl = false;
		this.bte_Bin.Location = new System.Drawing.Point(282, 10);
		this.bte_Bin.Name = "bte_Bin";
		this.bte_Bin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_Bin.SettingInVisible = false;
		this.bte_Bin.SettingReadonly = false;
		this.bte_Bin.Size = new System.Drawing.Size(120, 20);
		this.bte_Bin.TabIndex = 3;
		this.bte_Bin.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_Bin_ButtonClick);
		this.btn_Load.EditControl = null;
		this.btn_Load.EditValue = "Load";
		this.btn_Load.FieldName = null;
		this.btn_Load.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Load.ImageOptions.Image");
		this.btn_Load.IsSearchControl = false;
		this.btn_Load.Location = new System.Drawing.Point(409, 10);
		this.btn_Load.Name = "btn_Load";
		this.btn_Load.ReadOnly = false;
		this.btn_Load.SettingInVisible = false;
		this.btn_Load.SettingReadonly = false;
		this.btn_Load.Size = new System.Drawing.Size(87, 20);
		this.btn_Load.TabIndex = 4;
		this.btn_Load.Text = "Load";
		this.btn_Load.Click += new System.EventHandler(this.btn_Load_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(1, 40);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(745, 372);
		this.grd_Doc.TabIndex = 5;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Doc.UseBatchMod = false;
		this.btn_Posting.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Posting.EditControl = null;
		this.btn_Posting.EditValue = "Posting";
		this.btn_Posting.FieldName = null;
		this.btn_Posting.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Posting.ImageOptions.Image");
		this.btn_Posting.IsSearchControl = false;
		this.btn_Posting.Location = new System.Drawing.Point(3, 418);
		this.btn_Posting.Name = "btn_Posting";
		this.btn_Posting.ReadOnly = false;
		this.btn_Posting.SettingInVisible = false;
		this.btn_Posting.SettingReadonly = false;
		this.btn_Posting.Size = new System.Drawing.Size(87, 20);
		this.btn_Posting.TabIndex = 6;
		this.btn_Posting.Text = "Posting";
		this.btn_Posting.Click += new System.EventHandler(this.btn_Posting_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(748, 442);
		base.Controls.Add(this.btn_Posting);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Load);
		base.Controls.Add(this.bte_Bin);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.look_Period);
		base.Controls.Add(this.lb_Period);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_InventoryCountingReport.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryCountingReport";
		base.ShowInTaskbar = false;
		this.Text = "Inventory Counting Report (Batch)";
		base.Load += new System.EventHandler(this.frm_InventoryCountingReport_Load);
		((System.ComponentModel.ISupportInitialize)this.look_Period.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_Bin.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		base.ResumeLayout(false);
	}
}
