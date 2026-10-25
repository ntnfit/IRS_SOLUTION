using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.XtraBars.Navigation;
using DevExpress.XtraBars.Ribbon;
using DevExpress.XtraBars.Ribbon.Gallery;
using DevExpress.XtraBars.Ribbon.ViewInfo;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.DMS;

public class frm_ImageViewer : XtraFormEx
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

	private GalleryControl galleryControl1;

	private GalleryControlClient galleryControlClient1;

	private SplitContainerControl splitContainerControl1;

	private ZoomTrackBarControl zoomTrackBarControl1;

	private AccordionControl acco_Album;

	private AccordionControlElement accoElement_Album;

	private LabelLine labelLine1;

	private ButtonEditCFL cfl_Employee;

	private DateEditEx date_searchFD;

	private LabelLine labelLine25;

	private DateEditEx date_searchTD;

	private LabelLine labelLine26;

	private SimpleButtonEx btn_Search;

	private ZoomTrackBarControl zoomTrackControl;

	private LabelLine labelLine2;

	private GridLookUpEdit gridLookUp_Bp;

	private GridView gridLookUp_BpView;

	private GridColumn gc_CardCode;

	private GridColumn gc_CardName;

	public frm_ImageViewer()
	{
		InitializeComponent();
		OnLoadAsync += frm_ImageViewer_Load;
		galleryControl1.Gallery.ItemDoubleClick += Gallery_ItemClick;
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

	private async Task frm_ImageViewer_Load(object sender, EventArgs e)
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
			date_searchFD.EditValue = DateTime.Now;
			date_searchTD.EditValue = DateTime.Now;
			galleryControl1.Gallery.AllowHoverImages = false;
			galleryControl1.Gallery.OptionsImageLoad.AsyncLoad = true;
			galleryControl1.Gallery.ItemImageLayout = DevExpress.Utils.Drawing.ImageLayoutMode.Squeeze;
			galleryControl1.Gallery.ImageSize = new Size(225, 225);
			galleryControl1.Gallery.FilterCaption = "Lọc theo nhóm";
			galleryControl1.Gallery.OptionsImageLoad.AnimationType = ImageContentAnimationType.SegmentedFade;
			galleryControl1.Gallery.ShowItemText = true;
			_dtAlbum = await _httpClient.ImageAlbumClient.LoadData();
			foreach (DataRow row in _dtAlbum.Rows)
			{
				AccordionControlElement accordionControlElement = new AccordionControlElement();
				accordionControlElement.Text = Function.ToString(row["Name"]);
				accordionControlElement.Tag = row;
				accordionControlElement.Name = "element_" + Function.ParseInt(row["Id"]);
				accordionControlElement.Style = ElementStyle.Item;
				accordionControlElement.Appearance.Normal.Font = new Font("Verdana", 8.25f);
				accordionControlElement.Appearance.Normal.ForeColor = Color.FromArgb(47, 46, 68);
				accoElement_Album.Elements.Add(accordionControlElement);
			}
			if (_dtAlbum.IsNotNull())
			{
				acco_Album.SelectedElement = accoElement_Album.Elements[0];
			}
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
				await frm_ImageViewer_Load(null, null);
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
			if (Function.ParseInt(cfl_Employee.EditValue) == 0)
			{
				WinCommons.Warning(Enums.MessageCode.None, "Chưa chọn nhân viên!");
				return;
			}
			if (Function.ToString(date_searchFD.EditValue) == "")
			{
				WinCommons.Warning(Enums.MessageCode.None, "Chưa chọn từ ngày!");
				return;
			}
			if (Function.ToString(date_searchTD.EditValue) == "")
			{
				WinCommons.Warning(Enums.MessageCode.None, "Chưa chọn đến ngày!");
				return;
			}
			galleryControl1.Gallery.Groups.Clear();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FromDate", typeof(DateTime));
			dataTable.Columns.Add("ToDate", typeof(DateTime));
			dataTable.Columns.Add("empID", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow["FromDate"] = date_searchFD.EditValue ?? DBNull.Value;
			dataRow["ToDate"] = date_searchTD.EditValue ?? DBNull.Value;
			dataRow["empID"] = Function.ParseInt(cfl_Employee.EditValue);
			dataTable.Rows.Add(dataRow);
			for (int i = 0; i < _dtAlbum.Rows.Count; i++)
			{
				DataRow dataRow2 = _dtAlbum.Rows[i];
				acco_Album.Elements[0].Elements["element_" + Function.ParseInt(dataRow2["Id"])].Text = Function.ToString(dataRow2["Name"]);
			}
			_listImageItem.Clear();
			DataTable dataTable2 = await _httpClient.UploadImageClient.GetImageList(dataTable);
			string[] lstCardCode = Function.ToString(gridLookUp_Bp.EditValue).Split(',');
			EnumerableRowCollection<DataRow> source = from dataRow4 in dataTable2.AsEnumerable()
				where lstCardCode.Any((string a) => Function.ToString(dataRow4["CardCode"]) == a.Trim())
				select dataRow4;
			dataTable2 = ((!source.Any()) ? dataTable2.Clone() : source.CopyToDataTable());
			if (dataTable2.IsNotNull())
			{
				for (int num = 0; num < _dtAlbum.Rows.Count; num++)
				{
					DataRow albumRow = _dtAlbum.Rows[num];
					int count = (from t in dataTable2.AsEnumerable()
						where Function.ParseInt(t["AlbumId"]) == Function.ParseInt(albumRow["Id"])
						select t).ToList().Count;
					acco_Album.Elements[0].Elements["element_" + Function.ParseInt(albumRow["Id"])].Text = Function.ToString(albumRow["Name"]) + $" ({count})";
				}
				int num2 = Function.ParseInt(((DataRow)acco_Album.SelectedElement.Tag)["Id"]);
				List<object> list = (from t in dataTable2.AsEnumerable()
					select t["UploadOnlyDate"]).Distinct().ToList();
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					GalleryItemGroup galleryItemGroup = new GalleryItemGroup();
					for (int num4 = 0; num4 < dataTable2.Rows.Count; num4++)
					{
						DataRow dataRow3 = dataTable2.Rows[num4];
						if (!(Function.ParseDateTimes(dataRow3["UploadOnlyDate"]) != Function.ParseDateTimes(list[num3])))
						{
							string caption = ((TimeSpan)dataRow3["UploadTime"]).Format(GlobalData.TimeFormat);
							GalleryItem galleryItem = new GalleryItem(null, caption, Function.ToString(dataRow3["CardName"]));
							galleryItem.Tag = dataRow3;
							_listImageItem.Add(galleryItem);
							if (num2 == Function.ParseInt(dataRow3["AlbumId"]))
							{
								galleryItemGroup.Items.Add(galleryItem);
							}
							Task item = DownloadImage(galleryItemGroup, Function.ToString(dataRow3["LinkDownload"]));
							listTask.Add(item);
						}
					}
					if (galleryItemGroup.Items.Count > 0)
					{
						galleryItemGroup.Caption = Function.ParseDateTimes(list[num3]).ToString(GlobalData.DateTimeFormat);
						galleryControl1.Gallery.Groups.Add(galleryItemGroup);
					}
				}
			}
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

	private void FilterImagesByAlbum()
	{
		try
		{
			galleryControl1.Gallery.Groups.Clear();
			int albumId = Function.ParseInt(((DataRow)acco_Album.SelectedElement.Tag)["Id"]);
			List<GalleryItem> list = _listImageItem.Where((GalleryItem t) => Function.ParseInt(((DataRow)t.Tag)["AlbumId"]) == albumId).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			List<DateTime> list2 = (from t in list.AsEnumerable()
				select Function.ParseDateTimes(((DataRow)t.Tag)["UploadOnlyDate"])).Distinct().ToList();
			for (int num = 0; num < list2.Count; num++)
			{
				GalleryItemGroup galleryItemGroup = new GalleryItemGroup();
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					GalleryItem galleryItem = list[num2];
					if (!(Function.ParseDateTimes(((DataRow)galleryItem.Tag)["UploadOnlyDate"]) != Function.ParseDateTimes(list2[num])))
					{
						galleryItemGroup.Items.Add(galleryItem);
					}
				}
				if (galleryItemGroup.Items.Count > 0)
				{
					galleryItemGroup.Caption = Function.ParseDateTimes(list2[num]).ToString(GlobalData.DateTimeFormat);
					galleryControl1.Gallery.Groups.Add(galleryItemGroup);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task DownloadImage(GalleryItemGroup group, string url)
	{
		if (string.IsNullOrEmpty(Function.ToString(url)))
		{
			return;
		}
		using WebClient client = new WebClient();
		Image image = Image.FromStream(await client.OpenReadTaskAsync(url));
		GalleryItem galleryItem = _listImageItem.Where((GalleryItem t) => Function.ToString(((DataRow)t.Tag)["LinkDownload"]) == url).FirstOrDefault();
		GalleryItem galleryItem2 = group.Items.Where((GalleryItem t) => Function.ToString(((DataRow)t.Tag)["LinkDownload"]) == url).FirstOrDefault();
		if (galleryItem != null)
		{
			galleryItem.Image = image;
		}
		if (galleryItem2 != null)
		{
			galleryItem2.Image = image;
		}
	}

	private void zoomTrackBarControl2_Properties_ValueChanged(object sender, EventArgs e)
	{
		ZoomTrackBarControl zoomTrackBarControl = (ZoomTrackBarControl)sender;
		galleryControl1.Gallery.ImageSize = new Size(zoomTrackBarControl.Value, zoomTrackBarControl.Value);
		galleryControl1.Gallery.HoverImageSize = new Size(zoomTrackBarControl.Value + 30, zoomTrackBarControl.Value + 30);
	}

	private void acco_Album_SelectedElementChanged(object sender, SelectedElementChangedEventArgs e)
	{
		try
		{
			if (e.Element.OwnerElement != null && !_isLoading)
			{
				FilterImagesByAlbum();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void cfl_Employee_EditValueChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			new DataTable();
			string userIdByEmpId = _httpClient.User.GetUserIdByEmpId(Function.ParseInt(cfl_Employee.EditValue));
			if (!string.IsNullOrEmpty(userIdByEmpId))
			{
				DataTable dataTable = await _httpClient.User.GetAnUser(Function.ParseInt(userIdByEmpId));
				if (dataTable.Rows.Count > 0)
				{
					_dtOutlet = await _httpClient.BusinessMaster.GetBpListDMS(Function.ToString(dataTable.Rows[0]["USER_CODE"]));
					gridLookUp_Bp.Properties.DataSource = _dtOutlet;
				}
				else
				{
					gridLookUp_Bp.Properties.DataSource = new DataTable();
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridLookUpEdit1View_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			_isChanging = true;
			StringBuilder stringBuilder = new StringBuilder();
			GridView gridView = sender as GridView;
			int[] selectedRows = gridView.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				if (stringBuilder.ToString().Length > 0)
				{
					stringBuilder.Append(", ");
				}
				DataRow dataRow = gridView.GetDataRow(rowHandle);
				stringBuilder.Append(Function.ToString(dataRow["CardCode"]));
			}
			gridLookUp_Bp.EditValue = stringBuilder.ToString();
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isChanging = false;
		}
	}

	private void gridLookUp_Bp_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			GridView gridView = gridLookUp_BpView;
			int[] selectedRows = gridView.GetSelectedRows();
			foreach (int rowHandle in selectedRows)
			{
				if (stringBuilder.ToString().Length > 0)
				{
					stringBuilder.Append(", ");
				}
				DataRow dataRow = gridView.GetDataRow(rowHandle);
				stringBuilder.Append(Function.ToString(dataRow["CardName"]));
			}
			e.DisplayText = stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void galleryControl1_Gallery_ContextButtonClick(object sender, ContextItemClickEventArgs e)
	{
		object dataItem = e.DataItem;
		if (dataItem != null)
		{
			GalleryItem galleryItem = (GalleryItem)dataItem;
			SaveFileDialog saveFileDialog = new SaveFileDialog();
			saveFileDialog.OverwritePrompt = true;
			saveFileDialog.Filter = "All files(*.*) | *.*";
			string text = "";
			Image image = galleryItem.Image;
			if (image.RawFormat.Equals(ImageFormat.Bmp))
			{
				text = ".bmp";
			}
			else if (image.RawFormat.Equals(ImageFormat.Gif))
			{
				text = ".gif";
			}
			else if (image.RawFormat.Equals(ImageFormat.Icon))
			{
				text = ".ico";
			}
			else if (image.RawFormat.Equals(ImageFormat.Jpeg))
			{
				text = ".jpg";
			}
			else if (image.RawFormat.Equals(ImageFormat.Png))
			{
				text = ".png";
			}
			else if (image.RawFormat.Equals(ImageFormat.Tiff))
			{
				text = ".tiff";
			}
			else if (image.RawFormat.Equals(ImageFormat.Wmf))
			{
				text = ".wmf";
			}
			saveFileDialog.FileName = galleryItem.Description + "-" + Function.ToString(DateTime.Now.Ticks) + text;
			if (saveFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = saveFileDialog.FileName;
				image.Save(fileName, image.RawFormat);
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
		DevExpress.Utils.ContextButton contextButton = new DevExpress.Utils.ContextButton();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_ImageViewer));
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
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.galleryControl1 = new DevExpress.XtraBars.Ribbon.GalleryControl();
		this.galleryControlClient1 = new DevExpress.XtraBars.Ribbon.GalleryControlClient();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridLookUp_Bp = new DevExpress.XtraEditors.GridLookUpEdit();
		this.gridLookUp_BpView = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gc_CardCode = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gc_CardName = new DevExpress.XtraGrid.Columns.GridColumn();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_searchTD = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine26 = new Apzon.Controls.Controls.LabelLine();
		this.date_searchFD = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine25 = new Apzon.Controls.Controls.LabelLine();
		this.cfl_Employee = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.acco_Album = new DevExpress.XtraBars.Navigation.AccordionControl();
		this.accoElement_Album = new DevExpress.XtraBars.Navigation.AccordionControlElement();
		this.zoomTrackBarControl1 = new DevExpress.XtraEditors.ZoomTrackBarControl();
		this.zoomTrackControl = new DevExpress.XtraEditors.ZoomTrackBarControl();
		((System.ComponentModel.ISupportInitialize)this.galleryControl1).BeginInit();
		this.galleryControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridLookUp_Bp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUp_BpView).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Employee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.acco_Album).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 482);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Visible = false;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 482);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.galleryControl1.Controls.Add(this.galleryControlClient1);
		this.galleryControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.galleryControl1.Gallery.Appearance.ItemDescriptionAppearance.Normal.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.galleryControl1.Gallery.Appearance.ItemDescriptionAppearance.Normal.Options.UseFont = true;
		this.galleryControl1.Gallery.ContextButtonOptions.DisplayArea = DevExpress.XtraBars.Ribbon.ViewInfo.ContextItemDisplayArea.Item;
		contextButton.Id = new System.Guid("dd57be72-1550-41b3-b820-eb00be76e7bd");
		contextButton.ImageOptionsCollection.ItemNormal.Image = (System.Drawing.Image)resources.GetObject("resource.Image");
		contextButton.Name = "contextButton1";
		this.galleryControl1.Gallery.ContextButtons.Add(contextButton);
		this.galleryControl1.Gallery.ContextButtonClick += new DevExpress.Utils.ContextItemClickEventHandler(this.galleryControl1_Gallery_ContextButtonClick);
		this.galleryControl1.Gallery.GetThumbnailImage += new DevExpress.XtraBars.Ribbon.Gallery.GalleryThumbnailImageEventHandler(this.galleryControl1_Gallery_GetThumbnailImage);
		this.galleryControl1.Location = new System.Drawing.Point(0, 0);
		this.galleryControl1.Name = "galleryControl1";
		this.galleryControl1.Size = new System.Drawing.Size(864, 481);
		this.galleryControl1.TabIndex = 16;
		this.galleryControl1.Text = "galleryControl1";
		this.galleryControlClient1.GalleryControl = this.galleryControl1;
		this.galleryControlClient1.Location = new System.Drawing.Point(2, 2);
		this.galleryControlClient1.Size = new System.Drawing.Size(843, 477);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, -5);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridLookUp_Bp);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_Search);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_searchTD);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine26);
		this.splitContainerControl1.Panel1.Controls.Add(this.date_searchFD);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine25);
		this.splitContainerControl1.Panel1.Controls.Add(this.cfl_Employee);
		this.splitContainerControl1.Panel1.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel1.Controls.Add(this.acco_Album);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.galleryControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1131, 481);
		this.splitContainerControl1.SplitterPosition = 257;
		this.splitContainerControl1.TabIndex = 17;
		this.gridLookUp_Bp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridLookUp_Bp.Location = new System.Drawing.Point(81, 93);
		this.gridLookUp_Bp.Name = "gridLookUp_Bp";
		this.gridLookUp_Bp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.gridLookUp_Bp.Properties.NullText = "";
		this.gridLookUp_Bp.Properties.PopupView = this.gridLookUp_BpView;
		this.gridLookUp_Bp.Properties.SearchMode = DevExpress.XtraEditors.Repository.GridLookUpSearchMode.AutoSearch;
		this.gridLookUp_Bp.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.gridLookUp_Bp.Properties.ViewType = DevExpress.XtraEditors.Repository.GridLookUpViewType.GridView;
		this.gridLookUp_Bp.Size = new System.Drawing.Size(172, 20);
		this.gridLookUp_Bp.TabIndex = 46;
		this.gridLookUp_Bp.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.gridLookUp_Bp_EditValueChanging);
		this.gridLookUp_Bp.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.gridLookUp_Bp_CustomDisplayText);
		this.gridLookUp_BpView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gc_CardCode, this.gc_CardName });
		this.gridLookUp_BpView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridLookUp_BpView.Name = "gridLookUp_BpView";
		this.gridLookUp_BpView.OptionsFind.AlwaysVisible = true;
		this.gridLookUp_BpView.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridLookUp_BpView.OptionsSelection.MultiSelect = true;
		this.gridLookUp_BpView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gridLookUp_BpView.OptionsView.ShowGroupPanel = false;
		this.gridLookUp_BpView.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gridLookUpEdit1View_SelectionChanged);
		this.gc_CardCode.Caption = "Ma\u0303";
		this.gc_CardCode.FieldName = "CardCode";
		this.gc_CardCode.Name = "gc_CardCode";
		this.gc_CardCode.Visible = true;
		this.gc_CardCode.VisibleIndex = 1;
		this.gc_CardName.Caption = "Tên cư\u0309a ha\u0300ng";
		this.gc_CardName.FieldName = "CardName";
		this.gc_CardName.Name = "gc_CardName";
		this.gc_CardName.Visible = true;
		this.gc_CardName.VisibleIndex = 2;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Cư\u0309a ha\u0300ng";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 93);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(77, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 44;
		this.labelLine2.Utext = "Cư\u0309a ha\u0300ng";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Tra cứu";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(178, 124);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 43;
		this.btn_Search.Text = "Tra cứu";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.date_searchTD.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.date_searchTD.EditControl = null;
		this.date_searchTD.EditValue = null;
		this.date_searchTD.FieldName = null;
		this.date_searchTD.Location = new System.Drawing.Point(81, 35);
		this.date_searchTD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_searchTD.Name = "date_searchTD";
		this.date_searchTD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchTD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchTD.Properties.IsSearchControl = true;
		this.date_searchTD.SettingInVisible = false;
		this.date_searchTD.SettingReadonly = false;
		this.date_searchTD.Size = new System.Drawing.Size(172, 20);
		this.date_searchTD.TabIndex = 42;
		this.labelLine26.EditControl = null;
		this.labelLine26.EditValue = "Đến ngày";
		this.labelLine26.FieldName = null;
		this.labelLine26.HideWhenEmpty = false;
		this.labelLine26.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine26.Location = new System.Drawing.Point(3, 37);
		this.labelLine26.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine26.Name = "labelLine26";
		this.labelLine26.ReadOnly = false;
		this.labelLine26.SettingInVisible = false;
		this.labelLine26.SettingReadonly = false;
		this.labelLine26.ShowUnderLine = true;
		this.labelLine26.Size = new System.Drawing.Size(80, 20);
		this.labelLine26.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine26.TabIndex = 41;
		this.labelLine26.Utext = "Đến ngày";
		this.date_searchFD.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.date_searchFD.EditControl = null;
		this.date_searchFD.EditValue = null;
		this.date_searchFD.FieldName = null;
		this.date_searchFD.Location = new System.Drawing.Point(81, 7);
		this.date_searchFD.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_searchFD.Name = "date_searchFD";
		this.date_searchFD.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchFD.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_searchFD.Properties.IsSearchControl = true;
		this.date_searchFD.SettingInVisible = false;
		this.date_searchFD.SettingReadonly = false;
		this.date_searchFD.Size = new System.Drawing.Size(172, 20);
		this.date_searchFD.TabIndex = 40;
		this.labelLine25.EditControl = null;
		this.labelLine25.EditValue = "Từ ngày";
		this.labelLine25.FieldName = null;
		this.labelLine25.HideWhenEmpty = false;
		this.labelLine25.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine25.Location = new System.Drawing.Point(3, 9);
		this.labelLine25.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine25.Name = "labelLine25";
		this.labelLine25.ReadOnly = false;
		this.labelLine25.SettingInVisible = false;
		this.labelLine25.SettingReadonly = false;
		this.labelLine25.ShowUnderLine = true;
		this.labelLine25.Size = new System.Drawing.Size(80, 20);
		this.labelLine25.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine25.TabIndex = 39;
		this.labelLine25.Utext = "Từ ngày";
		this.cfl_Employee.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.cfl_Employee.EditControl = null;
		this.cfl_Employee.FieldName = null;
		this.cfl_Employee.IsSearchControl = false;
		this.cfl_Employee.Location = new System.Drawing.Point(81, 62);
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
		this.cfl_Employee.TabIndex = 5;
		this.cfl_Employee.EditValueChanged += new System.EventHandler(this.cfl_Employee_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Nhân viên";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 65);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(77, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Nhân viên";
		this.acco_Album.AllowItemSelection = true;
		this.acco_Album.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.acco_Album.Elements.AddRange(new DevExpress.XtraBars.Navigation.AccordionControlElement[1] { this.accoElement_Album });
		this.acco_Album.Location = new System.Drawing.Point(3, 153);
		this.acco_Album.Name = "acco_Album";
		this.acco_Album.ScrollBarMode = DevExpress.XtraBars.Navigation.ScrollBarMode.Auto;
		this.acco_Album.ShowFilterControl = DevExpress.XtraBars.Navigation.ShowFilterControl.Auto;
		this.acco_Album.ShowGroupExpandButtons = false;
		this.acco_Album.Size = new System.Drawing.Size(259, 328);
		this.acco_Album.TabIndex = 3;
		this.acco_Album.Text = "accordionControl1";
		this.acco_Album.SelectedElementChanged += new DevExpress.XtraBars.Navigation.SelectedElementChangedEventHandler(this.acco_Album_SelectedElementChanged);
		this.accoElement_Album.Expanded = true;
		this.accoElement_Album.Name = "accoElement_Album";
		this.accoElement_Album.Text = "Album";
		this.zoomTrackBarControl1.EditValue = null;
		this.zoomTrackBarControl1.Location = new System.Drawing.Point(908, 672);
		this.zoomTrackBarControl1.Name = "zoomTrackBarControl1";
		this.zoomTrackBarControl1.Size = new System.Drawing.Size(104, 16);
		this.zoomTrackBarControl1.TabIndex = 2;
		this.zoomTrackControl.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.zoomTrackControl.EditValue = 225;
		this.zoomTrackControl.Location = new System.Drawing.Point(896, 487);
		this.zoomTrackControl.Name = "zoomTrackControl";
		this.zoomTrackControl.Properties.AutoSize = false;
		this.zoomTrackControl.Properties.Maximum = 400;
		this.zoomTrackControl.Properties.Middle = 225;
		this.zoomTrackControl.Properties.Minimum = 50;
		this.zoomTrackControl.Properties.ValueChanged += new System.EventHandler(this.zoomTrackBarControl2_Properties_ValueChanged);
		this.zoomTrackControl.Size = new System.Drawing.Size(237, 18);
		this.zoomTrackControl.TabIndex = 18;
		this.zoomTrackControl.Value = 225;
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
		base.Name = "frm_ImageViewer";
		this.Text = "Xem ảnh báo cáo";
		((System.ComponentModel.ISupportInitialize)this.galleryControl1).EndInit();
		this.galleryControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridLookUp_Bp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridLookUp_BpView).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchTD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_searchFD.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cfl_Employee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.acco_Album).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackBarControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.zoomTrackControl).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
