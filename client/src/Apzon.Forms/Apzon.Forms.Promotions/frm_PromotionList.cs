using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Promotions;

public class frm_PromotionList : XtraFormEx
{
	private DataTable _activeFilterSource;

	private readonly IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_List;

	private GridViewEx gv_List;

	private LabelLine lb_Search;

	private ButtonEditEx btn_Search;

	private SimpleButtonEx btn_OK;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private SimpleButtonEx btn_Add;

	private RepositoryItemButtonEdit rep_Code;

	public frm_PromotionList()
	{
		InitializeComponent();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			DataTable dataTable = await httpClient.Promotion.GetListPromotions(3, "", "", "");
			grd_List.DataSource = dataTable;
			grd_List.RefreshDataSource();
			_activeFilterSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Add_Click(object sender, EventArgs e)
	{
		try
		{
			frm_PromotionDefinition frm_PromotionDefinition2 = new frm_PromotionDefinition();
			frm_PromotionDefinition2.ReloadAfterSave = (Action)Delegate.Combine(frm_PromotionDefinition2.ReloadAfterSave, (Action)(async () =>
			{
				await REFRESH_DATA();
			}));
			frm_PromotionDefinition2.MdiParent = MdiParent;
			frm_PromotionDefinition2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_PromotionList_Load(object sender, EventArgs e)
	{
		try
		{
			await REFRESH_DATA();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Search_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = "";
			foreach (GridColumn column in gv_List.Columns)
			{
				if (column.Visible && _activeFilterSource.ExistsColumn(column.FieldName))
				{
					text += $"OR {column.FieldName} LIKE '%{btn_Search.Text}%' ";
				}
			}
			text = ((text.Length > 2) ? text.Substring(2) : "");
			gv_List.ActiveFilterString = text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Code_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			ShowEdit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ShowEdit()
	{
		frm_PromotionDefinition frm_PromotionDefinition2 = new frm_PromotionDefinition();
		frm_PromotionDefinition2.ReloadAfterSave = (Action)Delegate.Combine(frm_PromotionDefinition2.ReloadAfterSave, (Action)(async () =>
		{
			await REFRESH_DATA();
		}));
		frm_PromotionDefinition2.MdiParent = MdiParent;
		frm_PromotionDefinition2.FromCode = Function.ToString(gv_List.GetFocusedRowCellValue("Code"));
		frm_PromotionDefinition2.Show();
	}

	private void grd_List_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (gv_List.FocusedRowHandle >= 0)
			{
				ShowEdit();
			}
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Promotions.frm_PromotionList));
		this.grd_List = new Apzon.Controls.Grid.GridControlEx();
		this.gv_List = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Code = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.lb_Search = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_List).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_List).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Code).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Search.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_List.AllowPasteExcel = true;
		this.grd_List.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_List.ClipboardData = null;
		this.grd_List.Location = new System.Drawing.Point(2, 40);
		this.grd_List.MainView = this.gv_List;
		this.grd_List.Name = "grd_List";
		this.grd_List.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Code });
		this.grd_List.Size = new System.Drawing.Size(1043, 568);
		this.grd_List.TabIndex = 1;
		this.grd_List.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_List });
		this.grd_List.DoubleClick += new System.EventHandler(this.grd_List_DoubleClick);
		this.gv_List.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_List.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_List.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_List.ColumnKey = null;
		this.gv_List.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8 });
		this.gv_List.DisableFormSetting = false;
		this.gv_List.DisplayException = false;
		this.gv_List.EditColumn = null;
		this.gv_List.FieldCurrency = null;
		this.gv_List.FieldRate = null;
		this.gv_List.FormId = null;
		this.gv_List.GridControl = this.grd_List;
		this.gv_List.IgnoreFilter = false;
		this.gv_List.IndexEditRow = -1;
		this.gv_List.IndicatorWidth = 35;
		this.gv_List.IsApzTable = false;
		this.gv_List.IsInsertNewRow = false;
		this.gv_List.IsLoadingData = false;
		this.gv_List.IsMappingContextAction = false;
		this.gv_List.ItemId = null;
		this.gv_List.LstColumnAllowBatch = null;
		this.gv_List.LstColumnNotAllowBatch = null;
		this.gv_List.LstColumnNotEditable = null;
		this.gv_List.LstColumNotSetting = null;
		this.gv_List.Name = "gv_List";
		this.gv_List.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_List.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_List.OptionsPrint.AutoWidth = false;
		this.gv_List.OptionsPrint.UsePrintStyles = false;
		this.gv_List.OptionsView.BestFitMaxRowCount = 100;
		this.gv_List.OptionsView.ColumnAutoWidth = false;
		this.gv_List.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_List.OptionsView.ShowGroupedColumns = true;
		this.gv_List.OptionsView.ShowGroupPanel = false;
		this.gv_List.UseBatchMod = false;
		this.gridColumnEx1.Caption = "Mã chương trình";
		this.gridColumnEx1.ColumnEdit = this.rep_Code;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Code";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx1.Width = 150;
		this.rep_Code.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Code.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Code.Name = "rep_Code";
		this.rep_Code.ReadOnly = true;
		this.rep_Code.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Code_ButtonClick);
		this.gridColumnEx2.Caption = "Tên chương trình";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 150;
		this.gridColumnEx3.Caption = "Ghi chú";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Remark";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 150;
		this.gridColumnEx4.Caption = "Ngày hiệu lực";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "FromDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 150;
		this.gridColumnEx5.Caption = "Ngày hết hiệu lực";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ToDate";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 150;
		this.gridColumnEx6.Caption = "Trạng thái";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Status";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.gridColumnEx6.Width = 150;
		this.gridColumnEx7.Caption = "Ngày tạo ";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "CreateDate";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 6;
		this.gridColumnEx7.Width = 150;
		this.gridColumnEx8.Caption = "Người tạo";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "Owner";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 7;
		this.gridColumnEx8.Width = 150;
		this.lb_Search.EditControl = null;
		this.lb_Search.EditValue = "Tìm kiếm";
		this.lb_Search.FieldName = null;
		this.lb_Search.HideWhenEmpty = false;
		this.lb_Search.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Search.Location = new System.Drawing.Point(2, 14);
		this.lb_Search.Name = "lb_Search";
		this.lb_Search.ReadOnly = false;
		this.lb_Search.SettingInVisible = false;
		this.lb_Search.SettingReadonly = false;
		this.lb_Search.ShowUnderLine = true;
		this.lb_Search.Size = new System.Drawing.Size(105, 20);
		this.lb_Search.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Search.TabIndex = 2;
		this.lb_Search.Utext = "Tìm kiếm";
		this.btn_Search.EditControl = null;
		this.btn_Search.FieldName = null;
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(107, 11);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(756, 20);
		this.btn_Search.TabIndex = 3;
		this.btn_Search.EditValueChanged += new System.EventHandler(this.btn_Search_EditValueChanged);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 615);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 4;
		this.btn_OK.Text = "OK";
		this.btn_Add.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "Tạo mới chương trình";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(884, 9);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(161, 23);
		this.btn_Add.TabIndex = 5;
		this.btn_Add.Text = "Tạo mới chương trình";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1047, 642);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lb_Search);
		base.Controls.Add(this.grd_List);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PromotionList.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PromotionList";
		base.ShowInTaskbar = false;
		this.Text = "Danh sách chương trình khuyến mại";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_PromotionList_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_List).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_List).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Code).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Search.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
