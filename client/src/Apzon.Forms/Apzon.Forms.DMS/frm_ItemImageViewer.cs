using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.DMS;

public class frm_ItemImageViewer : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private bool _isChanging;

	private List<GalleryItem> _listImageItem = new List<GalleryItem>();

	private DataTable _dtAlbum = new DataTable();

	private DataTable _dtOutlet = new DataTable();

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private SplitContainerControl splitContainerControl1;

	private ZoomTrackBarControl zoomTrackBarControl1;

	private LabelLine labelLine1;

	private ButtonEditCFL cfl_ItemCodeTo;

	private SimpleButtonEx btn_Search;

	private ZoomTrackBarControl zoomTrackControl;

	private LabelLine labelLine2;

	private CheckedComboBoxEditEx cb_ItemGroup;

	private ButtonEditCFL cfl_ItemCodeFrom;

	private LabelLine labelLine3;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_ItemCode;

	private RepositoryItemButtonEditCFL rep_CFL;

	private GridColumnEx gc_ItemName;

	private GridColumnEx gc_Image;

	private RepositoryItemPictureEdit rep_Image;

	private GridViewEx gridViewEx1;

	private GridColumnEx gc_ItmsGrpCod;

	private RepositoryItemLookUpEdit cbItemGroup;

	public frm_ItemImageViewer()
	{
		InitializeComponent();
		OnLoadAsync += frm_ItemImageViewer_Load;
	}

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
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpload;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	private void Gallery_ItemClick(object sender, GalleryItemClickEventArgs e)
	{
		try
		{
			string text = Path.GetTempPath() + "\\a.jpg";
			if (e.Item.Image == null)
			{
				WinCommons.Warning(Enums.MessageCode.None, "Không có ảnh để hiển thị!");
				return;
			}
			e.Item.Image.Save(text);
			if (File.Exists(text))
			{
				Process.Start(text);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.AttachmentChangedOrDelete, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ItemImageViewer_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadInit();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInit()
	{
		try
		{
			_isLoading = true;
			DataTable itemGroup = _httpClient.Items.GetItemGroup();
			SetDataSource(cb_ItemGroup, itemGroup, hideCodeColumn: true, "ITMSGRPCOD", "ITMSGRPNAM");
			cbItemGroup.Columns.Clear();
			cbItemGroup.Columns.Add(new LookUpColumnInfo
			{
				Caption = "Name",
				FieldName = "ITMSGRPNAM"
			});
			cbItemGroup.DisplayMember = "ITMSGRPNAM";
			cbItemGroup.ValueMember = "ITMSGRPCOD";
			cbItemGroup.NullText = "";
			cbItemGroup.DataSource = itemGroup;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await frm_ItemImageViewer_Load(null, null);
			}
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
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void galleryControl1_Gallery_GetThumbnailImage(object sender, GalleryThumbnailImageEventArgs e)
	{
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			await LoadImages();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadImages()
	{
		try
		{
			WinCommons._OpenProcessing();
			List<Task> listTask = new List<Task>();
			if ((Function.ToString(cfl_ItemCodeFrom.EditValue) == "" || Function.ToString(cfl_ItemCodeTo.EditValue) == "") && Function.ToString(cb_ItemGroup.EditValue) == "")
			{
				WinCommons.Warning(Enums.MessageCode.None, "Cho\u0323n khoa\u0309ng ma\u0303 ha\u0300ng hoă\u0323c cho\u0323n nho\u0301m sa\u0309n phâ\u0309m đê\u0300 tiê\u0301p tu\u0323c!");
				return;
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FromCode", typeof(string));
			dataTable.Columns.Add("ToCode", typeof(string));
			dataTable.Columns.Add("ItemGroups", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["FromCode"] = Function.ToString(cfl_ItemCodeFrom.EditValue);
			dataRow["ToCode"] = Function.ToString(cfl_ItemCodeTo.EditValue);
			if (CheckedComboBoxEdit.IsSelectAllItem(cb_ItemGroup.InternalListBox.Items[0]) && cb_ItemGroup.InternalListBox.Items[0].CheckState == CheckState.Checked)
			{
				dataRow["ItemGroups"] = "A";
			}
			else
			{
				dataRow["ItemGroups"] = Function.ToString(cb_ItemGroup.EditValue);
			}
			dataTable.Rows.Add(dataRow);
			_listImageItem.Clear();
			DataTable dataTable2 = await _httpClient.UploadImageClient.GetItemImageList(dataTable);
			if (dataTable2.IsNotNull())
			{
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					DataRow dataRow2 = dataTable2.Rows[i];
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["LinkDownload"])))
					{
						Task item = DownloadImage(Function.ToString(dataRow2["ItemCode"]), Function.ToString(dataRow2["LinkDownload"]));
						listTask.Add(item);
					}
				}
			}
			dataTable2.Columns.Add("Image", typeof(Image));
			grd_Doc.DataSource = dataTable2;
			grd_Doc.RefreshDataSource();
			Task.WhenAll(listTask);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	public void SetDataSource(CheckedComboBoxEdit look, DataTable dt, bool hideCodeColumn = false, string valueMember = "Code", string displayMember = "Name")
	{
		try
		{
			look.Properties.DataSource = dt;
			look.Properties.DisplayMember = displayMember;
			look.Properties.ValueMember = valueMember;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
		}
	}

	private async Task DownloadImage(string itemCode, string url)
	{
		if (string.IsNullOrEmpty(Function.ToString(url)))
		{
			return;
		}
		using WebClient client = new WebClient();
		Image value = Image.FromStream(await client.OpenReadTaskAsync(url));
		DataTable dataTable = grd_Doc.DataSource as DataTable;
		DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow item) => Function.ToString(item["ItemCode"]) == itemCode);
		gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataTable.Rows.IndexOf(row)), "Image", value);
	}

	private void zoomTrackBarControl2_Properties_ValueChanged(object sender, EventArgs e)
	{
		ZoomTrackBarControl zoomTrackBarControl = (ZoomTrackBarControl)sender;
		gv_Doc.RowHeight = zoomTrackBarControl.Value;
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		MouseEventArgs e2 = e as MouseEventArgs;
		GridHitInfo gridHitInfo = (sender as GridView).CalcHitInfo(e2.Location);
		if ((!gridHitInfo.InRow && !gridHitInfo.InRowCell) || gridHitInfo.Column.FieldName != "Image")
		{
			return;
		}
		string text = Path.GetTempPath() + "\\a.jpg";
		if (gv_Doc.GetRowCellValue(gridHitInfo.RowHandle, "Image") is Image image)
		{
			image.Save(text);
			if (File.Exists(text))
			{
				Process.Start(text);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.AttachmentChangedOrDelete, "");
			}
		}
	}

	private void gridLookUp_Bp_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				e.Cancel = true;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_ItemImageViewer));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.cfl_ItemCodeFrom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.cb_ItemGroup = new Apzon.Controls.Editor.CheckedComboBoxEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cfl_ItemCodeTo = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ItmsGrpCod = new Apzon.Controls.Grid.GridColumnEx();
		this.cbItemGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Image = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Image = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.zoomTrackBarControl1 = new DevExpress.XtraEditors.ZoomTrackBarControl();
		this.zoomTrackControl = new DevExpress.XtraEditors.ZoomTrackBarControl();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cfl_ItemCodeFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ItemGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ItemCodeTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbItemGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Image).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 482);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Visible = false;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 482);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, -5);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.cfl_ItemCodeFrom);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel1.Controls.Add(this.cb_ItemGroup);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_Search);
		this.splitContainerControl1.Panel1.Controls.Add(this.cfl_ItemCodeTo);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1131, 481);
		this.splitContainerControl1.SplitterPosition = 262;
		this.splitContainerControl1.TabIndex = 17;
		this.cfl_ItemCodeFrom.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.cfl_ItemCodeFrom.FieldName = null;
		this.cfl_ItemCodeFrom.IsSearchControl = false;
		this.cfl_ItemCodeFrom.Location = new System.Drawing.Point(89, 34);
		this.cfl_ItemCodeFrom.Name = "cfl_ItemCodeFrom";
		this.cfl_ItemCodeFrom.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_ItemCodeFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ItemCodeFrom.Properties.ContainerControl = null;
		this.cfl_ItemCodeFrom.Properties.DisplayMember = "ItemCode";
		this.cfl_ItemCodeFrom.Properties.DisplayMember2 = "";
		this.cfl_ItemCodeFrom.Properties.LoadDataSourceWhenEnter = true;
		this.cfl_ItemCodeFrom.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ItemCodeFrom.Properties.SeparateString = ", ";
		this.cfl_ItemCodeFrom.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.cfl_ItemCodeFrom.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.cfl_ItemCodeFrom.Properties.UsingAutoCompleteBox = true;
		this.cfl_ItemCodeFrom.Properties.UsingDataSource = true;
		this.cfl_ItemCodeFrom.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ItemCodeFrom.Properties.ValueMember = "ItemCode";
		this.cfl_ItemCodeFrom.Size = new System.Drawing.Size(169, 20);
		this.cfl_ItemCodeFrom.TabIndex = 51;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(3, 37);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(88, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 50;
		this.labelLine3.Utext = "Tư\u0300 ma\u0303 ha\u0300ng";
		this.cb_ItemGroup.FieldName = null;
		this.cb_ItemGroup.IsSearchControl = false;
		this.cb_ItemGroup.Location = new System.Drawing.Point(89, 8);
		this.cb_ItemGroup.Name = "cb_ItemGroup";
		this.cb_ItemGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_ItemGroup.Properties.SeparatorChar = ';';
		this.cb_ItemGroup.Size = new System.Drawing.Size(169, 20);
		this.cb_ItemGroup.TabIndex = 49;
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(88, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 48;
		this.labelLine2.Utext = "Nho\u0301m sa\u0309n phâ\u0309m";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(183, 86);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 43;
		this.btn_Search.Text = "Tra cứu";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.cfl_ItemCodeTo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.cfl_ItemCodeTo.FieldName = null;
		this.cfl_ItemCodeTo.IsSearchControl = false;
		this.cfl_ItemCodeTo.Location = new System.Drawing.Point(89, 60);
		this.cfl_ItemCodeTo.Name = "cfl_ItemCodeTo";
		this.cfl_ItemCodeTo.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.cfl_ItemCodeTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.cfl_ItemCodeTo.Properties.ContainerControl = null;
		this.cfl_ItemCodeTo.Properties.DisplayMember = "ItemCode";
		this.cfl_ItemCodeTo.Properties.DisplayMember2 = "";
		this.cfl_ItemCodeTo.Properties.LoadDataSourceWhenEnter = true;
		this.cfl_ItemCodeTo.Properties.SeparateCharacterForMulti = ';';
		this.cfl_ItemCodeTo.Properties.SeparateString = ", ";
		this.cfl_ItemCodeTo.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.cfl_ItemCodeTo.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.cfl_ItemCodeTo.Properties.UsingAutoCompleteBox = true;
		this.cfl_ItemCodeTo.Properties.UsingDataSource = true;
		this.cfl_ItemCodeTo.Properties.ValidateWhenNoDataSource = false;
		this.cfl_ItemCodeTo.Properties.ValueMember = "ItemCode";
		this.cfl_ItemCodeTo.Size = new System.Drawing.Size(169, 20);
		this.cfl_ItemCodeTo.TabIndex = 5;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 63);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(88, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Đê\u0301n ma\u0303 ha\u0300ng";
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_CFL, this.rep_Image, this.cbItemGroup });
		this.grd_Doc.Size = new System.Drawing.Size(859, 481);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Doc, this.gridViewEx1 });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gc_ItemCode, this.gc_ItemName, this.gc_ItmsGrpCod, this.gc_Image });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUploadItemImage";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "APZ_D_UUPIM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.Click;
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsCustomization.AllowRowSizing = true;
		this.gv_Doc.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.RowHeight = 100;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.DoubleClick += new System.EventHandler(this.gv_Doc_DoubleClick);
		this.gc_ItemCode.Caption = "Mã sản phẩm";
		this.gc_ItemCode.ColumnEdit = this.rep_CFL;
		this.gc_ItemCode.CurrencyDateField = null;
		this.gc_ItemCode.CurrencyField = null;
		this.gc_ItemCode.CurrencyFixed = null;
		this.gc_ItemCode.CurrencyFixedRate = 0.0;
		this.gc_ItemCode.CurrencyRateField = null;
		this.gc_ItemCode.FieldName = "ItemCode";
		this.gc_ItemCode.Name = "gc_ItemCode";
		this.gc_ItemCode.PrimaryKey = false;
		this.gc_ItemCode.UnitField = null;
		this.gc_ItemCode.Visible = true;
		this.gc_ItemCode.VisibleIndex = 0;
		this.gc_ItemCode.Width = 145;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.ContainerControl = this;
		this.rep_CFL.DisplayMember = "";
		this.rep_CFL.DisplayMember2 = "";
		this.rep_CFL.LoadDataSourceWhenEnter = false;
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.SeparateCharacterForMulti = ';';
		this.rep_CFL.SeparateString = ", ";
		this.rep_CFL.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.rep_CFL.UsingAutoCompleteBox = false;
		this.rep_CFL.UsingDataSource = true;
		this.rep_CFL.ValidateWhenNoDataSource = false;
		this.rep_CFL.ValueMember = "";
		this.gc_ItemName.Caption = "Tên sản phẩm";
		this.gc_ItemName.CurrencyDateField = null;
		this.gc_ItemName.CurrencyField = null;
		this.gc_ItemName.CurrencyFixed = null;
		this.gc_ItemName.CurrencyFixedRate = 0.0;
		this.gc_ItemName.CurrencyRateField = null;
		this.gc_ItemName.FieldName = "ItemName";
		this.gc_ItemName.Name = "gc_ItemName";
		this.gc_ItemName.OptionsColumn.ReadOnly = true;
		this.gc_ItemName.PrimaryKey = false;
		this.gc_ItemName.UnitField = null;
		this.gc_ItemName.Visible = true;
		this.gc_ItemName.VisibleIndex = 1;
		this.gc_ItemName.Width = 150;
		this.gc_ItmsGrpCod.Caption = "Nho\u0301m";
		this.gc_ItmsGrpCod.ColumnEdit = this.cbItemGroup;
		this.gc_ItmsGrpCod.CurrencyDateField = null;
		this.gc_ItmsGrpCod.CurrencyField = null;
		this.gc_ItmsGrpCod.CurrencyFixed = null;
		this.gc_ItmsGrpCod.CurrencyFixedRate = 0.0;
		this.gc_ItmsGrpCod.CurrencyRateField = null;
		this.gc_ItmsGrpCod.FieldName = "ItmsGrpCod";
		this.gc_ItmsGrpCod.Name = "gc_ItmsGrpCod";
		this.gc_ItmsGrpCod.PrimaryKey = false;
		this.gc_ItmsGrpCod.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_ItmsGrpCod.UnitField = null;
		this.gc_ItmsGrpCod.Visible = true;
		this.gc_ItmsGrpCod.VisibleIndex = 2;
		this.gc_ItmsGrpCod.Width = 131;
		this.cbItemGroup.AutoHeight = false;
		this.cbItemGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbItemGroup.Name = "cbItemGroup";
		this.cbItemGroup.NullText = "";
		this.gc_Image.Caption = "Ảnh";
		this.gc_Image.ColumnEdit = this.rep_Image;
		this.gc_Image.CurrencyDateField = null;
		this.gc_Image.CurrencyField = null;
		this.gc_Image.CurrencyFixed = null;
		this.gc_Image.CurrencyFixedRate = 0.0;
		this.gc_Image.CurrencyRateField = null;
		this.gc_Image.FieldName = "Image";
		this.gc_Image.Name = "gc_Image";
		this.gc_Image.OptionsColumn.ReadOnly = true;
		this.gc_Image.PrimaryKey = false;
		this.gc_Image.UnitField = null;
		this.gc_Image.Visible = true;
		this.gc_Image.VisibleIndex = 3;
		this.gc_Image.Width = 297;
		this.rep_Image.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.True;
		this.rep_Image.Name = "rep_Image";
		this.rep_Image.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
		this.rep_Image.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
		this.rep_Image.ZoomingOperationMode = DevExpress.XtraEditors.Repository.ZoomingOperationMode.MouseWheel;
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.GridControl = this.grd_Doc;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.zoomTrackBarControl1.EditValue = null;
		this.zoomTrackBarControl1.Location = new System.Drawing.Point(908, 672);
		this.zoomTrackBarControl1.Name = "zoomTrackBarControl1";
		this.zoomTrackBarControl1.Size = new System.Drawing.Size(104, 16);
		this.zoomTrackBarControl1.TabIndex = 2;
		this.zoomTrackControl.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.zoomTrackControl.EditValue = 50;
		this.zoomTrackControl.Location = new System.Drawing.Point(896, 487);
		this.zoomTrackControl.Name = "zoomTrackControl";
		this.zoomTrackControl.Properties.AutoSize = false;
		this.zoomTrackControl.Properties.Maximum = 400;
		this.zoomTrackControl.Properties.Middle = 225;
		this.zoomTrackControl.Properties.Minimum = 50;
		this.zoomTrackControl.Properties.ValueChanged += new System.EventHandler(this.zoomTrackBarControl2_Properties_ValueChanged);
		this.zoomTrackControl.Size = new System.Drawing.Size(237, 18);
		this.zoomTrackControl.TabIndex = 18;
		this.zoomTrackControl.Value = 50;
		base.AcceptButton = this.btn_Search;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1135, 510);
		base.Controls.Add(this.zoomTrackControl);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oUploadItemImage";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsSetting;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemImageViewer";
		this.Text = "Danh sa\u0301ch sa\u0309n phâ\u0309m";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cfl_ItemCodeFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_ItemGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_ItemCodeTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbItemGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Image).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
