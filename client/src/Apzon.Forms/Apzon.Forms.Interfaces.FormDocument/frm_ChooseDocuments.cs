using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_ChooseDocuments : XtraFormEx
{
	private IContainer components;

	private GridControlEx gridControlEx1;

	private GridViewEx gridViewEx1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private LabelLine labelLine1;

	private DateEditEx date_From;

	private DateEditEx date_To;

	private LabelLine labelLine2;

	private SimpleButtonEx btn_Choose;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Search;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	public ApzObjectTypes DocType { get; set; }

	public DateTime? FromDate { get; set; }

	public DateTime? ToDate { get; set; }

	public List<int> IgnoreEntries { get; set; }

	public List<Tuple<int, int>> ChooseItems { get; set; }

	private IHttpClient httpClient { get; set; }

	public frm_ChooseDocuments()
	{
		InitializeComponent();
	}

	private void frm_ChooseDocuments_Load(object sender, EventArgs e)
	{
		try
		{
			FormId = "ChooseDoc" + (int)DocType;
			gridViewEx1.FormId = FormId;
			gridViewEx1.ItemId = ((int)DocType).ToString();
			httpClient = HttpClientHelperFactory.GetHttpClient(DocType);
			date_From.EditValue = FromDate ?? DateTime.Today;
			date_To.EditValue = ToDate ?? DateTime.Today;
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ParseDateTime(date_From.EditValue)?.ToString("yyyy-MM-dd");
			string to = Function.ParseDateTime(date_To.EditValue)?.ToString("yyyy-MM-dd");
			DataTable dataTable = await httpClient.Documents.ListChooseDocuments(text, to);
			if (!dataTable.IsNotNull())
			{
				return;
			}
			if (IgnoreEntries.Count == 0)
			{
				gridControlEx1.DataSource = dataTable;
				gridControlEx1.RefreshDataSource();
				return;
			}
			DataRow[] array = (from t in dataTable.AsEnumerable()
				where !IgnoreEntries.Contains(Function.ParseInt(t["DocEntry"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				gridControlEx1.DataSource = array.CopyToDataTable();
				gridControlEx1.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void btn_Choose_Click(object sender, EventArgs e)
	{
		try
		{
			ChooseItems = new List<Tuple<int, int>>();
			int[] selectedRows = gridViewEx1.GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				Tuple<int, int> item = new Tuple<int, int>(Function.ParseInt(gridViewEx1.GetRowCellValue(selectedRows[i], "DocEntry")), Function.ParseInt(gridViewEx1.GetRowCellValue(selectedRows[i], "DocNum")));
				ChooseItems.Add(item);
			}
			DialogResult = DialogResult.OK;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
	}

	private void gridViewEx1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_ChooseDocuments));
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gridControlEx1 = new Apzon.Controls.Grid.GridControlEx();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Choose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.gridControlEx1.AllowPasteExcel = true;
		this.gridControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControlEx1.ClipboardData = null;
		this.gridControlEx1.Location = new System.Drawing.Point(-1, 39);
		this.gridControlEx1.MainView = this.gridViewEx1;
		this.gridControlEx1.Name = "gridControlEx1";
		this.gridControlEx1.Size = new System.Drawing.Size(800, 379);
		this.gridControlEx1.TabIndex = 0;
		this.gridControlEx1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridViewEx1 });
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx6, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5 });
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.GridControl = this.gridControlEx1;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsSelection.MultiSelect = true;
		this.gridViewEx1.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.gridViewEx1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridViewEx1_CellValueChanged);
		this.gridColumnEx6.Caption = "Doc Num";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "DocNum";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = true;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 1;
		this.gridColumnEx6.Width = 91;
		this.gridColumnEx1.Caption = "Date";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx1.FieldName = "DocDate";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.gridColumnEx1.Width = 121;
		this.gridColumnEx2.Caption = "Customer/Vendor";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "CardName";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 3;
		this.gridColumnEx2.Width = 182;
		this.gridColumnEx3.Caption = "Remarks";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Comments";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 4;
		this.gridColumnEx3.Width = 148;
		this.gridColumnEx4.Caption = "Due Date";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "DocDueDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 5;
		this.gridColumnEx4.Width = 113;
		this.gridColumnEx5.Caption = "Doc Total";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx5.FieldName = "Total";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 6;
		this.gridColumnEx5.Width = 121;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "From date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(94, 10);
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
		this.date_From.Size = new System.Drawing.Size(111, 20);
		this.date_From.TabIndex = 2;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(319, 10);
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
		this.date_To.Size = new System.Drawing.Size(111, 20);
		this.date_To.TabIndex = 4;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(238, 13);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 3;
		this.labelLine2.Utext = "To date";
		this.btn_Choose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Choose.EditControl = null;
		this.btn_Choose.EditValue = "Choose";
		this.btn_Choose.FieldName = null;
		this.btn_Choose.IsSearchControl = false;
		this.btn_Choose.Location = new System.Drawing.Point(10, 421);
		this.btn_Choose.Name = "btn_Choose";
		this.btn_Choose.ReadOnly = false;
		this.btn_Choose.SettingInVisible = false;
		this.btn_Choose.SettingReadonly = false;
		this.btn_Choose.Size = new System.Drawing.Size(75, 23);
		this.btn_Choose.TabIndex = 5;
		this.btn_Choose.Text = "Choose";
		this.btn_Choose.Click += new System.EventHandler(this.btn_Choose_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(91, 421);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 6;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(445, 9);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 7;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		base.AcceptButton = this.btn_Search;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Choose);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.gridControlEx1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ChooseDocuments.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ChooseDocuments";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "List of documents";
		base.Load += new System.EventHandler(this.frm_ChooseDocuments_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControlEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
