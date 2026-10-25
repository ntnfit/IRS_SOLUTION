using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.DMS;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.DMS;

public class frm_AdminUploadAttachment : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable dtBP = new DataTable();

	private DataTable dtAttachment = new DataTable();

	private IContainer components;

	private DataTable dataTable1;

	private DataTable dataTable2;

	private DataTable dataTable3;

	private DataTable dataTable4;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_storeCode;

	private GridColumnEx gc_storeName;

	private GridColumnEx gc_address;

	private GridColumnEx gc_attachment;

	private ButtonEditCFL cfl_Employee;

	private LabelLine labelLine1;

	private DataTable dataTable5;

	private DataTable dataTable6;

	public frm_AdminUploadAttachment()
	{
		InitializeComponent();
		OnLoadAsync += frm_AdminUploadAttachment_Load;
	}

	private async Task frm_AdminUploadAttachment_Load(object sender, EventArgs e)
	{
		try
		{
			AddColumnAttachmentButton();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cfl_Employee_EditValueChanged(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		new DataTable();
		string userIdByEmpId = _httpClient.User.GetUserIdByEmpId(Function.ParseInt(cfl_Employee.EditValue));
		if (!string.IsNullOrEmpty(userIdByEmpId))
		{
			DataTable dataTable = await _httpClient.User.GetAnUser(Function.ParseInt(userIdByEmpId));
			if (dataTable.Rows.Count > 0)
			{
				dtBP = await _httpClient.BusinessMaster.GetBpListDMS(Function.ToString(dataTable.Rows[0]["USER_CODE"]));
				grd_Doc.DataSource = dtBP;
				grd_Doc.Refresh();
				gv_Doc.BestFitColumns();
			}
		}
		WinCommons._CloseProcessing();
	}

	public void AttachmentClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string objCode = Function.ToString(((ButtonEdit)sender).GetGridViewFromEditor().GetFocusedDataRow()["CardCode"]);
			new frm_UploadAttachment(ApzObjectTypes.oDMSAdminUpload, objCode, new UploadAttachmentConfigImpl
			{
				ShowSave = true
			}).ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddColumnAttachmentButton()
	{
		try
		{
			RepositoryItemButtonEdit repositoryItemButtonEdit = new RepositoryItemButtonEdit();
			repositoryItemButtonEdit.TextEditStyle = TextEditStyles.HideTextEditor;
			repositoryItemButtonEdit.Buttons[0].Kind = ButtonPredefines.Glyph;
			Image image = ImageResourceCache.Default.GetImage("images/mail/attachment_16x16.png");
			repositoryItemButtonEdit.Buttons[0].ImageOptions.Image = image;
			repositoryItemButtonEdit.ButtonClick += AttachmentClick;
			gv_Doc.Columns["Attachment"].ColumnEdit = repositoryItemButtonEdit;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_AdminUploadAttachment));
		this.dataTable1 = new System.Data.DataTable();
		this.dataTable2 = new System.Data.DataTable();
		this.dataTable3 = new System.Data.DataTable();
		this.dataTable4 = new System.Data.DataTable();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_storeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_storeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_address = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_attachment = new Apzon.Controls.Grid.GridColumnEx();
		this.cfl_Employee = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.dataTable5 = new System.Data.DataTable();
		this.dataTable6 = new System.Data.DataTable();
		((System.ComponentModel.ISupportInitialize)base.DsContextAction).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Employee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable6).BeginInit();
		base.SuspendLayout();
		this.dataTable1.Namespace = "";
		this.dataTable1.TableName = "Table1";
		this.dataTable2.Namespace = "";
		this.dataTable2.TableName = "Table1";
		this.dataTable3.Namespace = "";
		this.dataTable3.TableName = "Table1";
		this.dataTable4.Namespace = "";
		this.dataTable4.TableName = "Table1";
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(3, 30);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(795, 417);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_storeCode, this.gc_storeName, this.gc_address, this.gc_attachment });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = 0;
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
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.ReadOnly = false;
		this.gv_Doc.SettingReadonly = false;
		this.gv_Doc.UseBatchMod = false;
		this.gc_storeCode.Caption = "Store code";
		this.gc_storeCode.CurrencyDateField = null;
		this.gc_storeCode.CurrencyField = null;
		this.gc_storeCode.CurrencyFixed = null;
		this.gc_storeCode.CurrencyFixedRate = 0.0;
		this.gc_storeCode.CurrencyRateField = null;
		this.gc_storeCode.FieldName = "CardCode";
		this.gc_storeCode.Name = "gc_storeCode";
		this.gc_storeCode.PrimaryKey = false;
		this.gc_storeCode.UnitField = null;
		this.gc_storeCode.Visible = true;
		this.gc_storeCode.VisibleIndex = 0;
		this.gc_storeName.Caption = "Store name";
		this.gc_storeName.CurrencyDateField = null;
		this.gc_storeName.CurrencyField = null;
		this.gc_storeName.CurrencyFixed = null;
		this.gc_storeName.CurrencyFixedRate = 0.0;
		this.gc_storeName.CurrencyRateField = null;
		this.gc_storeName.FieldName = "CardName";
		this.gc_storeName.Name = "gc_storeName";
		this.gc_storeName.PrimaryKey = false;
		this.gc_storeName.UnitField = null;
		this.gc_storeName.Visible = true;
		this.gc_storeName.VisibleIndex = 1;
		this.gc_address.Caption = "Address";
		this.gc_address.CurrencyDateField = null;
		this.gc_address.CurrencyField = null;
		this.gc_address.CurrencyFixed = null;
		this.gc_address.CurrencyFixedRate = 0.0;
		this.gc_address.CurrencyRateField = null;
		this.gc_address.FieldName = "ShipToAddress";
		this.gc_address.Name = "gc_address";
		this.gc_address.PrimaryKey = false;
		this.gc_address.UnitField = null;
		this.gc_address.Visible = true;
		this.gc_address.VisibleIndex = 2;
		this.gc_attachment.Caption = "Attachment";
		this.gc_attachment.CurrencyDateField = null;
		this.gc_attachment.CurrencyField = null;
		this.gc_attachment.CurrencyFixed = null;
		this.gc_attachment.CurrencyFixedRate = 0.0;
		this.gc_attachment.CurrencyRateField = null;
		this.gc_attachment.FieldName = "Attachment";
		this.gc_attachment.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
		this.gc_attachment.Name = "gc_attachment";
		this.gc_attachment.PrimaryKey = false;
		this.gc_attachment.UnitField = null;
		this.gc_attachment.Visible = true;
		this.gc_attachment.VisibleIndex = 3;
		this.cfl_Employee.EditControl = null;
		this.cfl_Employee.FieldName = null;
		this.cfl_Employee.IsSearchControl = false;
		this.cfl_Employee.Location = new System.Drawing.Point(90, 4);
		this.cfl_Employee.Name = "cfl_Employee";
		this.cfl_Employee.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_Employee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_Employee.Properties.ContainerControl = null;
		this.cfl_Employee.Properties.DisplayMember = "fullName";
		this.cfl_Employee.Properties.DisplayMember2 = "";
		this.cfl_Employee.Properties.ListMandatoryField = null;
		this.cfl_Employee.Properties.LoadDataSourceWhenEnter = true;
		this.cfl_Employee.Properties.SeparateCharacterForMulti = ';';
		this.cfl_Employee.Properties.SeparateString = ", ";
		this.cfl_Employee.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.cfl_Employee.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.cfl_Employee.Properties.UsingAutoCompleteBox = true;
		this.cfl_Employee.Properties.UsingDataSource = true;
		this.cfl_Employee.Properties.ValidateWhenNoDataSource = false;
		this.cfl_Employee.Properties.ValueMember = "empID";
		this.cfl_Employee.SettingInVisible = false;
		this.cfl_Employee.SettingReadonly = false;
		this.cfl_Employee.Size = new System.Drawing.Size(172, 20);
		this.cfl_Employee.TabIndex = 7;
		this.cfl_Employee.EditValueChanged += new System.EventHandler(this.cfl_Employee_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Nhân viên";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 7);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(77, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 6;
		this.labelLine1.Utext = "Nhân viên";
		this.dataTable5.Namespace = "";
		this.dataTable5.TableName = "Table1";
		this.dataTable6.Namespace = "";
		this.dataTable6.TableName = "Table1";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 450);
		base.Controls.Add(this.cfl_Employee);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_AdminUploadAttachment.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AdminUploadAttachment";
		this.Text = "Upload Attachment";
		((System.ComponentModel.ISupportInitialize)base.DsContextAction).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Employee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dataTable6).EndInit();
		base.ResumeLayout(false);
	}
}
