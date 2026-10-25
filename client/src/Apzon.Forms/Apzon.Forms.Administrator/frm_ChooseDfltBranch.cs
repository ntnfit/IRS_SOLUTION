using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Administrator;

public class frm_ChooseDfltBranch : XtraForm
{
	public DataTable _dt = new DataTable();

	public string branchName = "";

	private int hotTrackRow = int.MinValue;

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit_Type;

	private RepositoryItemTextEdit rtxt;

	private GridColumnEx gcBPLName;

	private GridColumnEx gcAliasName;

	private GridColumnEx gcState;

	private GridColumnEx gcCounty;

	private GridColumnEx gcCountry;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	private int HotTrackRow
	{
		get
		{
			return hotTrackRow;
		}
		set
		{
			if (hotTrackRow != value)
			{
				int rowHandle = hotTrackRow;
				hotTrackRow = value;
				if (gv_Doc.ActiveEditor != null)
				{
					gv_Doc.PostEditor();
				}
				gv_Doc.RefreshRow(rowHandle);
				gv_Doc.RefreshRow(hotTrackRow);
			}
		}
	}

	public frm_ChooseDfltBranch()
	{
		InitializeComponent();
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmChooseDfltBranch;
		gcBPLName.Caption = Resources.OBPL_BPLName;
		gcAliasName.Caption = Resources.OBPL_AliasName;
		gcState.Caption = Resources.OBPL_State;
		gcCounty.Caption = Resources.OBPL_County;
		gcCountry.Caption = Resources.OBPL_Country;
		btnChoose.Text = Resources.ButtonSetAsDefault;
		btnCancel.Text = Resources.ButtonCancel;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		try
		{
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("BPLId");
			if (focusedRowCellValue != DBNull.Value && focusedRowCellValue != null && !string.IsNullOrWhiteSpace(focusedRowCellValue.ToString()))
			{
				GlobalData.DefaultBranch = Function.ParseInt(focusedRowCellValue);
				branchName = Function.ToString(gv_Doc.GetFocusedRowCellValue("BPLName"));
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_ChooseDfltBranch_Load(object sender, EventArgs e)
	{
		try
		{
			btnChoose.Enabled = false;
			LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadData()
	{
		try
		{
			grd_Doc.DataSource = _dt;
			int num = gv_Doc.LocateByValue("BPLId", GlobalData.DefaultBranch);
			if (num != int.MinValue)
			{
				gv_Doc.FocusedRowHandle = num;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			ApzonCommon.SetColorRowSelected(ref gv_Doc);
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("BPLId");
			if (focusedRowCellValue != DBNull.Value && focusedRowCellValue != null && !string.IsNullOrWhiteSpace(focusedRowCellValue.ToString()))
			{
				btnChoose.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		if (gv_Doc.CalcHitInfo(grd_Doc.PointToClient(Control.MousePosition)).InRow)
		{
			btnChoose_Click(null, null);
		}
	}

	private void gv_Doc_MouseMove(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = (sender as GridView).CalcHitInfo(new Point(e.X, e.Y));
		if (gridHitInfo.InRowCell)
		{
			HotTrackRow = gridHitInfo.RowHandle;
		}
		else
		{
			HotTrackRow = int.MinValue;
		}
	}

	private void gv_Doc_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.RowHandle == HotTrackRow)
		{
			e.Appearance.BackColor = ApzonCommon.colorFocusRow;
		}
		else if (e.RowHandle == gv_Doc.FocusedRowHandle)
		{
			e.Appearance.BackColor = StyleColor.GridEditting;
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Return)
		{
			btnChoose_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void gv_Doc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == HotTrackRow)
			{
				e.Appearance.BackColor = ApzonCommon.colorFocusRow;
			}
			else if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gcBPLName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAliasName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcState = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCounty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCountry = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rtxt = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Doc.Location = new System.Drawing.Point(1, 2);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemLookUpEdit_Type, this.rtxt });
		this.grd_Doc.Size = new System.Drawing.Size(689, 333);
		this.grd_Doc.TabIndex = 2;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcBPLName, this.gcAliasName, this.gcState, this.gcCounty, this.gcCountry });
		this.gv_Doc.DetailHeight = 431;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
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
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.Editable = false;
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsDetail.EnableMasterViewMode = false;
		this.gv_Doc.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gv_Doc.OptionsSelection.EnableAppearanceFocusedRow = false;
		this.gv_Doc.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.ReadOnly = true;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Doc_RowStyle);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gv_Doc_MouseMove);
		this.gv_Doc.MouseLeave += new System.EventHandler(this.gv_Doc_MouseLeave);
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gcBPLName.Caption = "Branch Name";
		this.gcBPLName.CurrencyDateField = null;
		this.gcBPLName.CurrencyField = null;
		this.gcBPLName.CurrencyFixed = null;
		this.gcBPLName.CurrencyFixedRate = 0.0;
		this.gcBPLName.CurrencyRateField = null;
		this.gcBPLName.FieldName = "BPLName";
		this.gcBPLName.MinWidth = 23;
		this.gcBPLName.Name = "gcBPLName";
		this.gcBPLName.PrimaryKey = true;
		this.gcBPLName.UnitField = null;
		this.gcBPLName.Visible = true;
		this.gcBPLName.VisibleIndex = 0;
		this.gcBPLName.Width = 87;
		this.gcAliasName.Caption = "Alias Name";
		this.gcAliasName.CurrencyDateField = null;
		this.gcAliasName.CurrencyField = null;
		this.gcAliasName.CurrencyFixed = null;
		this.gcAliasName.CurrencyFixedRate = 0.0;
		this.gcAliasName.CurrencyRateField = null;
		this.gcAliasName.FieldName = "AliasName";
		this.gcAliasName.MinWidth = 23;
		this.gcAliasName.Name = "gcAliasName";
		this.gcAliasName.PrimaryKey = false;
		this.gcAliasName.UnitField = null;
		this.gcAliasName.Visible = true;
		this.gcAliasName.VisibleIndex = 1;
		this.gcAliasName.Width = 269;
		this.gcState.Caption = "State";
		this.gcState.CurrencyDateField = null;
		this.gcState.CurrencyField = null;
		this.gcState.CurrencyFixed = null;
		this.gcState.CurrencyFixedRate = 0.0;
		this.gcState.CurrencyRateField = null;
		this.gcState.FieldName = "State";
		this.gcState.MinWidth = 23;
		this.gcState.Name = "gcState";
		this.gcState.PrimaryKey = false;
		this.gcState.UnitField = null;
		this.gcState.Visible = true;
		this.gcState.VisibleIndex = 2;
		this.gcState.Width = 87;
		this.gcCounty.Caption = "County";
		this.gcCounty.CurrencyDateField = null;
		this.gcCounty.CurrencyField = null;
		this.gcCounty.CurrencyFixed = null;
		this.gcCounty.CurrencyFixedRate = 0.0;
		this.gcCounty.CurrencyRateField = null;
		this.gcCounty.FieldName = "County";
		this.gcCounty.MinWidth = 23;
		this.gcCounty.Name = "gcCounty";
		this.gcCounty.PrimaryKey = false;
		this.gcCounty.UnitField = null;
		this.gcCounty.Visible = true;
		this.gcCounty.VisibleIndex = 3;
		this.gcCounty.Width = 87;
		this.gcCountry.Caption = "Country";
		this.gcCountry.CurrencyDateField = null;
		this.gcCountry.CurrencyField = null;
		this.gcCountry.CurrencyFixed = null;
		this.gcCountry.CurrencyFixedRate = 0.0;
		this.gcCountry.CurrencyRateField = null;
		this.gcCountry.FieldName = "Country";
		this.gcCountry.MinWidth = 23;
		this.gcCountry.Name = "gcCountry";
		this.gcCountry.PrimaryKey = false;
		this.gcCountry.UnitField = null;
		this.gcCountry.Visible = true;
		this.gcCountry.VisibleIndex = 4;
		this.gcCountry.Width = 87;
		this.repositoryItemLookUpEdit_Type.AutoHeight = false;
		this.repositoryItemLookUpEdit_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", " ")
		});
		this.repositoryItemLookUpEdit_Type.Name = "repositoryItemLookUpEdit_Type";
		this.repositoryItemLookUpEdit_Type.NullText = "";
		this.rtxt.AutoHeight = false;
		this.rtxt.Name = "rtxt";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(126, 345);
		this.btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(87, 28);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnChoose.Appearance.Options.UseBackColor = true;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Set as Default";
		this.btnChoose.FieldName = null;
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(8, 345);
		this.btnChoose.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(112, 28);
		this.btnChoose.TabIndex = 4;
		this.btnChoose.Text = "Set as Default";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(692, 384);
		base.ControlBox = false;
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_ChooseDfltBranch";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Select Default Branch";
		base.Load += new System.EventHandler(this.frm_ChooseDfltBranch_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt).EndInit();
		base.ResumeLayout(false);
	}
}
