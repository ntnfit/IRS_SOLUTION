using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Grid;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.UserControls.ContextAction;

public class UcContextActionMappingGridView : XtraUserControl
{
	public Action CellValueChanged;

	private bool _hasSummryType;

	private IContainer components;

	private GridControlEx grd_Header;

	private GridViewEx gv_Header;

	private GridColumnEx gridColumnEx33;

	private GridColumnEx gridColumnEx34;

	private RepositoryItemLookUpEdit rep_HeaderSourceField;

	private GridColumnEx gridColumnEx35;

	private GridColumnEx gridColumnEx36;

	private RepositoryItemLookUpEdit rep_TargetHeader;

	private GridColumnEx gridColumnEx37;

	private GridColumnEx col_SumType;

	private RepositoryItemLookUpEdit rep_HeaderSumType;

	public string MappingType { get; set; }

	public string TargetItem { get; set; }

	public bool HasSumaryType
	{
		get
		{
			return _hasSummryType;
		}
		set
		{
			_hasSummryType = value;
			col_SumType.Visible = value;
		}
	}

	public string FormId
	{
		get
		{
			return gv_Header.FormId;
		}
		set
		{
			gv_Header.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return gv_Header.ItemId;
		}
		set
		{
			gv_Header.ItemId = value;
		}
	}

	public DataTable DataSource
	{
		get
		{
			return grd_Header.DataSourceTable;
		}
		set
		{
			grd_Header.DataSource = value;
			grd_Header.RefreshDataSource();
		}
	}

	public UcContextActionMappingGridView()
	{
		InitializeComponent();
		Dictionary<string, string> dic = new Dictionary<string, string>
		{
			{ "", "" },
			{ "S", "SUM" },
			{ "A", "AVG" },
			{ "C", "COUNT" }
		};
		rep_HeaderSumType.SetDataSource(dic);
	}

	public void SetTargetField(DataTable dt)
	{
		try
		{
			rep_TargetHeader.SetDataSource(dt, "Name", "Code");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ChangeSourceField(DataTable source)
	{
		try
		{
			rep_HeaderSourceField.SetDataSource(source, "Name", "Code", emptyRow: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Header_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			CellValueChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		this.grd_Header = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Header = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx33 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx34 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_HeaderSourceField = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx35 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx36 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TargetHeader = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx37 = new Apzon.Controls.Grid.GridColumnEx();
		this.col_SumType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_HeaderSumType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.grd_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_HeaderSourceField).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetHeader).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_HeaderSumType).BeginInit();
		base.SuspendLayout();
		this.grd_Header.AllowPasteExcel = false;
		this.grd_Header.ClipboardData = null;
		this.grd_Header.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Header.Location = new System.Drawing.Point(0, 0);
		this.grd_Header.MainView = this.gv_Header;
		this.grd_Header.Name = "grd_Header";
		this.grd_Header.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_TargetHeader, this.rep_HeaderSourceField, this.rep_HeaderSumType });
		this.grd_Header.Size = new System.Drawing.Size(734, 510);
		this.grd_Header.TabIndex = 1;
		this.grd_Header.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Header });
		this.gv_Header.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Header.ColumnKey = null;
		this.gv_Header.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx33, this.gridColumnEx34, this.gridColumnEx35, this.gridColumnEx36, this.gridColumnEx37, this.col_SumType });
		this.gv_Header.DisableFormSetting = true;
		this.gv_Header.DisplayException = false;
		this.gv_Header.EditColumn = null;
		this.gv_Header.FieldCurrency = null;
		this.gv_Header.FieldRate = null;
		this.gv_Header.FormId = null;
		this.gv_Header.GridControl = this.grd_Header;
		this.gv_Header.IndexEditRow = -1;
		this.gv_Header.IndicatorWidth = 35;
		this.gv_Header.IsApzTable = false;
		this.gv_Header.IsInsertNewRow = false;
		this.gv_Header.IsLoadingData = false;
		this.gv_Header.ItemId = "OHEADER";
		this.gv_Header.LstColumnAllowBatch = null;
		this.gv_Header.LstColumnNotAllowBatch = null;
		this.gv_Header.LstColumNotSetting = null;
		this.gv_Header.Name = "gv_Header";
		this.gv_Header.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Header.OptionsView.ColumnAutoWidth = false;
		this.gv_Header.OptionsView.ShowGroupPanel = false;
		this.gv_Header.UseBatchMod = false;
		this.gv_Header.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Header_CellValueChanged);
		this.gridColumnEx33.Caption = "AbsEntry";
		this.gridColumnEx33.CurrencyDateField = null;
		this.gridColumnEx33.CurrencyField = null;
		this.gridColumnEx33.CurrencyFixed = null;
		this.gridColumnEx33.CurrencyFixedRate = 0.0;
		this.gridColumnEx33.CurrencyRateField = null;
		this.gridColumnEx33.FieldName = "AbsEntry";
		this.gridColumnEx33.Name = "gridColumnEx33";
		this.gridColumnEx33.PrimaryKey = false;
		this.gridColumnEx33.UnitField = null;
		this.gridColumnEx34.Caption = "Source Field Name";
		this.gridColumnEx34.ColumnEdit = this.rep_HeaderSourceField;
		this.gridColumnEx34.CurrencyDateField = null;
		this.gridColumnEx34.CurrencyField = null;
		this.gridColumnEx34.CurrencyFixed = null;
		this.gridColumnEx34.CurrencyFixedRate = 0.0;
		this.gridColumnEx34.CurrencyRateField = null;
		this.gridColumnEx34.FieldName = "SourceField";
		this.gridColumnEx34.Name = "gridColumnEx34";
		this.gridColumnEx34.PrimaryKey = false;
		this.gridColumnEx34.UnitField = null;
		this.gridColumnEx34.Visible = true;
		this.gridColumnEx34.VisibleIndex = 1;
		this.gridColumnEx34.Width = 121;
		this.rep_HeaderSourceField.AutoHeight = false;
		this.rep_HeaderSourceField.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_HeaderSourceField.Name = "rep_HeaderSourceField";
		this.rep_HeaderSourceField.NullText = "";
		this.gridColumnEx35.Caption = "TargetItem";
		this.gridColumnEx35.CurrencyDateField = null;
		this.gridColumnEx35.CurrencyField = null;
		this.gridColumnEx35.CurrencyFixed = null;
		this.gridColumnEx35.CurrencyFixedRate = 0.0;
		this.gridColumnEx35.CurrencyRateField = null;
		this.gridColumnEx35.FieldName = "TargetItem";
		this.gridColumnEx35.Name = "gridColumnEx35";
		this.gridColumnEx35.PrimaryKey = false;
		this.gridColumnEx35.UnitField = null;
		this.gridColumnEx36.Caption = "Target Field Name";
		this.gridColumnEx36.ColumnEdit = this.rep_TargetHeader;
		this.gridColumnEx36.CurrencyDateField = null;
		this.gridColumnEx36.CurrencyField = null;
		this.gridColumnEx36.CurrencyFixed = null;
		this.gridColumnEx36.CurrencyFixedRate = 0.0;
		this.gridColumnEx36.CurrencyRateField = null;
		this.gridColumnEx36.FieldName = "TargetField";
		this.gridColumnEx36.Name = "gridColumnEx36";
		this.gridColumnEx36.OptionsColumn.AllowEdit = false;
		this.gridColumnEx36.PrimaryKey = false;
		this.gridColumnEx36.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx36.UnitField = null;
		this.gridColumnEx36.Visible = true;
		this.gridColumnEx36.VisibleIndex = 0;
		this.gridColumnEx36.Width = 114;
		this.rep_TargetHeader.AutoHeight = false;
		this.rep_TargetHeader.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TargetHeader.Name = "rep_TargetHeader";
		this.rep_TargetHeader.NullText = "";
		this.gridColumnEx37.Caption = "Default Value";
		this.gridColumnEx37.CurrencyDateField = null;
		this.gridColumnEx37.CurrencyField = null;
		this.gridColumnEx37.CurrencyFixed = null;
		this.gridColumnEx37.CurrencyFixedRate = 0.0;
		this.gridColumnEx37.CurrencyRateField = null;
		this.gridColumnEx37.FieldName = "DfltValue";
		this.gridColumnEx37.Name = "gridColumnEx37";
		this.gridColumnEx37.PrimaryKey = false;
		this.gridColumnEx37.UnitField = null;
		this.gridColumnEx37.Visible = true;
		this.gridColumnEx37.VisibleIndex = 2;
		this.gridColumnEx37.Width = 175;
		this.col_SumType.Caption = "Summary Type";
		this.col_SumType.ColumnEdit = this.rep_HeaderSumType;
		this.col_SumType.CurrencyDateField = null;
		this.col_SumType.CurrencyField = null;
		this.col_SumType.CurrencyFixed = null;
		this.col_SumType.CurrencyFixedRate = 0.0;
		this.col_SumType.CurrencyRateField = null;
		this.col_SumType.FieldName = "SumType";
		this.col_SumType.Name = "col_SumType";
		this.col_SumType.PrimaryKey = false;
		this.col_SumType.UnitField = null;
		this.col_SumType.Visible = true;
		this.col_SumType.VisibleIndex = 3;
		this.col_SumType.Width = 100;
		this.rep_HeaderSumType.AutoHeight = false;
		this.rep_HeaderSumType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_HeaderSumType.Name = "rep_HeaderSumType";
		this.rep_HeaderSumType.NullText = "";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.grd_Header);
		base.Name = "UcContextActionMappingGridView";
		base.Size = new System.Drawing.Size(734, 510);
		((System.ComponentModel.ISupportInitialize)this.grd_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Header).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_HeaderSourceField).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TargetHeader).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_HeaderSumType).EndInit();
		base.ResumeLayout(false);
	}
}
