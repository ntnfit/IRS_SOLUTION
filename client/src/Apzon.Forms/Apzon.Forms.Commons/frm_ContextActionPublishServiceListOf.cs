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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class frm_ContextActionPublishServiceListOf : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string _controllerName;

	private string _actionName;

	private string _description;

	private DataTable _searchedResult;

	private int _choosedItem;

	private IContainer components;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	public frm_ContextActionPublishServiceListOf()
	{
		InitializeComponent();
	}

	private async Task frm_ContextActionPublishServiceListOf_Load(object sender, EventArgs e)
	{
		gv_Data.ShowLoadingPanel();
		try
		{
			DataTable dataTable = ((!_searchedResult.IsNotNull()) ? (await _httpClient.PublishService.Search(_controllerName, _actionName, _description)) : _searchedResult.Copy());
			if (dataTable != null && !dataTable.Columns.Contains("Uri"))
			{
				dataTable.Columns.Add("Uri", typeof(string));
			}
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Uri"] = string.Format("{0}h/0/{1}/{2}", Configuration.ApiServer, Function.ToString(dataTable.Rows[i]["ControllerName"]), Function.ToString(dataTable.Rows[i]["ActionName"]));
				}
				dataTable.AcceptChanges();
			}
			grd_Data.DataSource = dataTable;
			grd_Data.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		gv_Data.HideLoadingPanel();
	}

	public void SetSearchParams(string controllerName = "", string actionName = "", string description = "")
	{
		try
		{
			_controllerName = controllerName;
			_actionName = actionName;
			_description = description;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetSearchedResult(DataTable dt)
	{
		try
		{
			_searchedResult = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public int GetChoosedEntry()
	{
		try
		{
			return _choosedItem;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	private void gv_Data_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				btn_OK_Click(null, null);
			}
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			throw;
		}
	}

	private void gv_Data_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			btn_OK_Click(null, null);
		}
		catch (Exception value)
		{
			Console.WriteLine(value);
			throw;
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			_choosedItem = Function.ParseInt(gv_Data.GetFocusedRowCellValue("AbsEntry"));
			DialogResult = DialogResult.Yes;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			DialogResult = DialogResult.No;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ContextActionPublishServiceListOf));
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		base.SuspendLayout();
		this.grd_Data.AllowPasteExcel = true;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Location = new System.Drawing.Point(1, 2);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(744, 436);
		this.grd_Data.TabIndex = 0;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Data.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8 });
		this.gv_Data.DisableFormSetting = false;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oListOfPublishedService";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IgnoreFilter = false;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.IsMappingContextAction = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumnNotEditable = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Data.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Data.OptionsBehavior.ReadOnly = true;
		this.gv_Data.OptionsPrint.AutoWidth = false;
		this.gv_Data.OptionsPrint.UsePrintStyles = false;
		this.gv_Data.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Data.OptionsView.ShowGroupedColumns = true;
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		this.gv_Data.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_Data_KeyDown);
		this.gv_Data.DoubleClick += new System.EventHandler(this.gv_Data_DoubleClick);
		this.gridColumnEx1.Caption = "#";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Controller";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "ControllerName";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 150;
		this.gridColumnEx3.Caption = "Action";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ActionName";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 150;
		this.gridColumnEx4.Caption = "Desctiption";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Desctiption";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 150;
		this.gridColumnEx5.Caption = "Uri";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Uri";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 150;
		this.gridColumnEx6.Caption = "ActionEntry";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "ActionEntry";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Width = 150;
		this.gridColumnEx7.Caption = "ActionFormId";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ActionFormId";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Width = 150;
		this.gridColumnEx8.Caption = "ActionItemId";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "ActionItemId";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Width = 150;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "Choose";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(13, 444);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "Choose";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(94, 444);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(747, 472);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Data);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oListOfPublishedService";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ContextActionPublishServiceListOf.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ContextActionPublishServiceListOf";
		base.ShowInTaskbar = false;
		this.Text = "List Of Published Services";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_ContextActionPublishServiceListOf_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		base.ResumeLayout(false);
	}
}
