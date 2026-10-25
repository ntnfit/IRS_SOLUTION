using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Net;
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
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.DMS;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.DMS;

public class frm_UploadAttachment : XtraFormEx
{
	private enum UploadStatus
	{
		Waiting,
		Success,
		Fail
	}

	private enum SaveType
	{
		NewFile,
		Database,
		Deleted
	}

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string objectCode;

	private ApzObjectTypes objectType;

	private UploadAttachmentConfigImpl config;

	public DataTable dtSelect = new DataTable();

	public DataTable dtUpload = new DataTable();

	private PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(UploadOneDriveFileResultImpl));

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Select;

	private GridViewEx gv_Select;

	private GridColumnEx gc_FileName;

	private GridColumnEx gc_FileType;

	private GridColumnEx gc_FileSize;

	private GridColumnEx gc_FilePath;

	private GridControlEx grd_Upload;

	private GridViewEx gv_Upload;

	private SimpleButtonEx btn_Upload;

	private SimpleButtonEx simpleButtonEx2;

	private OpenFileDialog openFileDialog;

	private GridColumnEx gc_Message;

	private GridColumnEx gc_UploadStatus;

	private RepositoryItemImageComboBox combo_UploadStatus;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx gc_FileId;

	private GridColumnEx gc_UploadFileName;

	private GridColumnEx gc_FileNameDrive;

	private GridColumnEx gc_UploadSize;

	private GridColumnEx gc_UploadExtension;

	private GridColumnEx gc_MimeType;

	private GridColumnEx gc_Path;

	private GridColumnEx gc_WebUrl;

	private GridColumnEx gc_LinkDownload;

	private GridColumnEx gc_LinkThumbnails;

	private GridColumnEx gc_Domain;

	private GridColumnEx gc_PathThumbnail;

	private GridColumnEx gc_SaveType;

	private RepositoryItemImageComboBox combo_SaveType;

	private RepositoryItemPictureEdit repositoryItemPictureEdit1;

	private GridColumnEx gc_ThumbnailsPreview;

	private RepositoryItemHyperLinkEdit repositoryItemHyperLinkEdit1;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private RepositoryItemHyperLinkEdit repositoryItemHyperLinkEdit2;

	private RepositoryItemHyperLinkEdit repositoryItemHyperLinkEdit3;

	private SimpleButtonEx simpleButtonEx3;

	private ImageCollection imgCollection;

	private LookUpEditEx txt_album;

	private LabelLine labelLine2;

	private GridColumnEx gc_Album;

	private RepositoryItemLookUpEdit rtxt_album;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				return;
			}
			_formMode = value;
			switch (value)
			{
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_UploadAttachment(ApzObjectTypes objType, string objCode)
	{
		objectCode = objCode;
		objectType = objType;
		config = new UploadAttachmentConfigImpl();
		OnLoadAsync += frm_UploadAttachment_Load;
		InitializeComponent();
	}

	public frm_UploadAttachment(ApzObjectTypes objType, string objCode, UploadAttachmentConfigImpl conf)
	{
		objectCode = objCode;
		objectType = objType;
		config = conf;
		OnLoadAsync += frm_UploadAttachment_Load;
		InitializeComponent();
	}

	private Bitmap getBitmapImage(string url)
	{
		Bitmap result = null;
		try
		{
			result = new Bitmap(WebRequest.Create(url).GetResponse().GetResponseStream());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	private async Task LoadDataAlbum()
	{
		WinCommons._OpenProcessing();
		try
		{
			DataTable dataTable = await _httpClient.ImageAlbumClient.LoadData();
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("Remarks"))
			{
				dataTable.Columns.Remove("Remarks");
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataTable.Rows[i]["Id"] = Function.ToString(dataTable.Rows[i]["Id"]);
			}
			dataTable.AcceptChanges();
			txt_album.EditValue = "-1";
			txt_album.Properties.DataSource = dataTable;
			rtxt_album.DataSource = dataTable;
			rtxt_album.ValueMember = "Id";
			rtxt_album.DisplayMember = "Name";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			ImageCollection imageCollection = new ImageCollection();
			Image image = ImageResourceCache.Default.GetImage("images/arrows/moveup_16x16.png");
			Image image2 = ImageResourceCache.Default.GetImage("images/actions/apply_16x16.png");
			Image image3 = ImageResourceCache.Default.GetImage("images/actions/cancel_16x16.png");
			if (config.ShowSave)
			{
				Image image4 = imgCollection.Images["save_16x16.png"];
				btn_Upload.ImageOptions.Image = image4;
				btn_Upload.Text = "Save";
			}
			imageCollection.AddImage(image);
			imageCollection.AddImage(image2);
			imageCollection.AddImage(image3);
			combo_UploadStatus.SmallImages = imageCollection;
			combo_UploadStatus.Items.Add("Waiting", 0, 0);
			combo_UploadStatus.Items.Add("Success", 1, 1);
			combo_UploadStatus.Items.Add("Fail", 2, 2);
			ImageCollection imageCollection2 = new ImageCollection();
			Image image5 = ImageResourceCache.Default.GetImage("images/actions/addfile_16x16.png");
			Image image6 = ImageResourceCache.Default.GetImage("images/data/database_16x16.png");
			imageCollection2.AddImage(image5);
			imageCollection2.AddImage(image6);
			combo_SaveType.SmallImages = imageCollection2;
			combo_SaveType.Items.Add("New File", 0, 0);
			combo_SaveType.Items.Add("Database", 1, 1);
			await LoadDataAlbum();
			if (dtSelect.Columns.Count == 0)
			{
				dtSelect.Columns.Add("FileName", typeof(string));
				dtSelect.Columns.Add("FilePath", typeof(string));
				dtSelect.Columns.Add("FileType", typeof(string));
				dtSelect.Columns.Add("FileSize", typeof(long));
				dtSelect.Columns.Add("AlbumId", typeof(long));
				dtSelect.Columns.Add("UploadStatus", typeof(int));
				dtSelect.Columns.Add("Message", typeof(string));
				dtSelect.AcceptChanges();
			}
			if (dtUpload.Columns.Count == 0)
			{
				foreach (PropertyDescriptor prop in props)
				{
					dtUpload.Columns.Add(prop.Name, prop.PropertyType);
				}
				dtUpload.Columns.Add("Code", typeof(string));
				dtUpload.Columns.Add("UserSign", typeof(string));
				dtUpload.Columns.Add("AlbumId", typeof(string));
				dtUpload.Columns.Add("UploadDate", typeof(DateTime));
				dtUpload.Columns.Add("ObjectType", typeof(string));
				dtUpload.Columns.Add("ObjectCode", typeof(string));
				dtUpload.Columns.Add("SaveType", typeof(int));
				dtUpload.Columns.Add("ThumbnailsPreview", typeof(Bitmap));
				dtUpload.AcceptChanges();
			}
			DataTable dataTable = await _httpClient.UploadImageClient.GetFileByObjectType(Function.ToString((int)objectType), objectCode);
			if (dataTable.IsNotNull())
			{
				DataColumn dataColumn = new DataColumn("SaveType", typeof(int));
				dataColumn.DefaultValue = 1;
				dataTable.Columns.Add(dataColumn);
				DataRow[] array = dtUpload.Select("SaveType = " + Function.ToString(1));
				for (int i = 0; i < array.Length; i++)
				{
					array[i].Delete();
				}
				dtUpload.AcceptChanges();
				dataTable.Columns.Add("ThumbnailsPreview", typeof(Bitmap));
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					Bitmap bitmapImage = getBitmapImage(Function.ToString(dataTable.Rows[j]["LinkThumbnails"]));
					dataTable.Rows[j]["ThumbnailsPreview"] = bitmapImage;
				}
				for (int k = 0; k < dataTable.Rows.Count; k++)
				{
					bool flag = false;
					for (int l = 0; l < dtUpload.Rows.Count; l++)
					{
						if (Function.ToString(dataTable.Rows[k]["FileId"]) == Function.ToString(dtUpload.Rows[l]["FileId"]) && Function.ParseInt(dtUpload.Rows[l]["SaveType"]) == 2)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						DataRow dataRow = dtUpload.NewRow();
						for (int m = 0; m < dtUpload.Columns.Count; m++)
						{
							dataRow[dtUpload.Columns[m].ColumnName] = dataTable.Rows[k][dtUpload.Columns[m].ColumnName];
						}
						dtUpload.Rows.Add(dataRow);
					}
				}
				dtUpload.AcceptChanges();
			}
			else if (dtUpload.IsNotNull())
			{
				DataRow[] array2 = dtUpload.Select("SaveType = " + Function.ToString(1));
				for (int n = 0; n < array2.Length; n++)
				{
					array2[n].Delete();
				}
				dtUpload.AcceptChanges();
			}
			grd_Upload.DataSource = dtUpload;
			grd_Upload.RefreshDataSource();
			gv_Upload.BestFitColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_UploadAttachment_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
	}

	private void simpleButtonEx2_Click(object sender, EventArgs e)
	{
		try
		{
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				for (int i = 0; i < openFileDialog.FileNames.Length; i++)
				{
					FileInfo fileInfo = new FileInfo(openFileDialog.FileNames[i]);
					DataRow dataRow = dtSelect.NewRow();
					dataRow["FileName"] = Function.ToString(fileInfo.Name);
					dataRow["FilePath"] = Function.ToString(fileInfo.FullName);
					dataRow["FileType"] = Function.ToString(fileInfo.Extension);
					dataRow["FileSize"] = Function.ToString(fileInfo.Length);
					dataRow["AlbumId"] = Function.ToString(txt_album.EditValue);
					dataRow["UploadStatus"] = 0;
					dataRow["Message"] = Function.ToString("");
					dtSelect.Rows.Add(dataRow);
				}
				dtSelect.AcceptChanges();
				grd_Select.DataSource = dtSelect;
				grd_Select.RefreshDataSource();
				gv_Select.BestFitColumns();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task UploadAttachment(DataRow dr, int i)
	{
		try
		{
			new UploadOneDriveFileResultImpl();
			string fileName = Function.ToString(dr["FileName"]);
			string filePath = Function.ToString(dr["FilePath"]);
			new UploadOneDriveInputImpl
			{
				ObjectCode = objectCode,
				ObjectTypes = objectType,
				FileName = fileName,
				FilePath = filePath
			};
			UploadOneDriveResultImpl uploadOneDriveResultImpl = await _httpClient.FileNetworkManagerClient.UploadFileOneDriveResult(filePath, fileName);
			string message = uploadOneDriveResultImpl.Message;
			bool success = uploadOneDriveResultImpl.Success;
			UploadOneDriveFileResultImpl file = uploadOneDriveResultImpl.File;
			DataTable dataTable = grd_Select.DatasourceTable();
			if (success && file != null)
			{
				dataTable.Rows[i]["Message"] = "";
				dataTable.Rows[i]["UploadStatus"] = 1;
				DataRow dataRow = dtUpload.NewRow();
				foreach (PropertyDescriptor prop in props)
				{
					dataRow[prop.Name] = file.GetType().GetProperty(prop.Name).GetValue(file, null);
				}
				dataRow["Code"] = Guid.NewGuid();
				dataRow["SaveType"] = 0;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["ObjectType"] = Function.ToString((int)objectType);
				dataRow["ObjectCode"] = objectCode;
				dataRow["AlbumId"] = Function.ToString(dr["AlbumId"]);
				dataRow["UploadDate"] = DateTime.Now;
				Bitmap bitmapImage = getBitmapImage(file.LinkThumbnails);
				dataRow["ThumbnailsPreview"] = bitmapImage;
				dtUpload.Rows.Add(dataRow);
				dr.Delete();
				dtUpload.AcceptChanges();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
			{
				dataTable.Rows[i]["Message"] = message ?? "";
				dataTable.Rows[i]["UploadStatus"] = 2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleButtonEx1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataTable dataTable = grd_Select.DatasourceTable();
			if (dataTable.Rows.Count <= 0)
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.NoDataFound, Enums.MessageCode.SystemMessage);
				return;
			}
			List<Task> listTask = new List<Task>();
			WinCommons._OpenProcessing();
			Stopwatch stopwatch = new Stopwatch();
			stopwatch.Start();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (!(Function.ToString(dataTable.Rows[i]["UploadStatus"]) == Function.ToString(1)))
				{
					Task item = UploadAttachment(dataTable.Rows[i], i);
					listTask.Add(item);
				}
			}
			await Task.WhenAll(listTask);
			grd_Upload.DataSource = dtUpload;
			grd_Upload.RefreshDataSource();
			gv_Upload.BestFitColumns();
			if (config.ShowSave)
			{
				DataTable dt = dtUpload.Copy();
				HttpResult httpResult = await _httpClient.FileNetworkManagerClient.SaveAttachment(dt);
				if (httpResult.MessageCode == Enums.MessageCode.Success)
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.None, "[Attachment] - " + httpResult.Message);
				}
			}
			Logging.Write("TRACE", "Thời gian tải lên " + listTask.Count + " ảnh: " + stopwatch.Elapsed.TotalSeconds.ToString("n2") + " (s)");
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Select.DeleteSelectedRows();
			DataTable dataSource = ((DataView)gv_Select.DataSource).ToTable().Copy();
			grd_Select.DataSource = dataSource;
			grd_Select.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				dtSelect = new DataTable();
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click_1(object sender, EventArgs e)
	{
		try
		{
			dtSelect = new DataTable();
			dtUpload = grd_Upload.DatasourceTable().Copy();
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return;
		}
		int[] selectedRows = gv_Upload.GetSelectedRows();
		DataTable dataTable = grd_Upload.DatasourceTable().Copy();
		for (int i = 0; i < dataTable.Rows.Count; i++)
		{
			for (int j = 0; j < selectedRows.Length; j++)
			{
				if (i == selectedRows[j])
				{
					if (Function.ParseInt(dataTable.Rows[i]["SaveType"]) == 1)
					{
						dataTable.Rows[i]["SaveType"] = 2;
					}
					else
					{
						dataTable.Rows[i].Delete();
					}
				}
			}
		}
		dataTable.AcceptChanges();
		dtUpload = dataTable.Copy();
		grd_Upload.DataSource = dataTable;
		grd_Upload.RefreshDataSource();
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gv_Upload_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			if (Function.ParseInt((sender as GridViewEx).GetRowCellValue(e.ListSourceRow, gc_SaveType)) == 2)
			{
				e.Visible = false;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleButtonEx3_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRefreshData, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				dtSelect = new DataTable();
				dtUpload = new DataTable();
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_UploadAttachment_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			dtUpload = grd_Upload.DatasourceTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_album_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			dtSelect = grd_Select.DatasourceTable();
			for (int i = 0; i < dtSelect.Rows.Count; i++)
			{
				dtSelect.Rows[i]["AlbumId"] = Function.ToString(txt_album.EditValue);
			}
			dtSelect.AcceptChanges();
			grd_Select.DataSource = dtSelect;
			grd_Select.RefreshDataSource();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_UploadAttachment));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Select = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Select = new Apzon.Controls.Grid.GridViewEx();
		this.gc_FileName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FilePath = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FileType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FileSize = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Album = new Apzon.Controls.Grid.GridColumnEx();
		this.rtxt_album = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Message = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UploadStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.combo_UploadStatus = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.grd_Upload = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Upload = new Apzon.Controls.Grid.GridViewEx();
		this.gc_SaveType = new Apzon.Controls.Grid.GridColumnEx();
		this.combo_SaveType = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
		this.gc_FileId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UploadFileName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FileNameDrive = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UploadSize = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UploadExtension = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_MimeType = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Path = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WebUrl = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHyperLinkEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
		this.gc_LinkDownload = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHyperLinkEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
		this.gc_LinkThumbnails = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHyperLinkEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemHyperLinkEdit();
		this.gc_Domain = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_PathThumbnail = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ThumbnailsPreview = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemPictureEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.btn_Upload = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButtonEx2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
		this.simpleButtonEx3 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.imgCollection = new DevExpress.Utils.ImageCollection(this.components);
		this.txt_album = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Select).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Select).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt_album).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.combo_UploadStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Upload).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Upload).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.combo_SaveType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.imgCollection).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_album.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(12, 38);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Select);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Upload);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(970, 473);
		this.splitContainerControl1.SplitterPosition = 176;
		this.splitContainerControl1.TabIndex = 1;
		this.grd_Select.AllowPasteExcel = false;
		this.grd_Select.ClipboardData = null;
		this.grd_Select.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Select.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Select.Location = new System.Drawing.Point(0, 0);
		this.grd_Select.MainView = this.gv_Select;
		this.grd_Select.Name = "grd_Select";
		this.grd_Select.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.combo_UploadStatus, this.rtxt_album });
		this.grd_Select.Size = new System.Drawing.Size(970, 176);
		this.grd_Select.TabIndex = 0;
		this.grd_Select.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Select });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(108, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Select.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Select.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Select.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Select.ColumnKey = null;
		this.gv_Select.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gc_FileName, this.gc_FilePath, this.gc_FileType, this.gc_FileSize, this.gc_Album, this.gc_Message, this.gc_UploadStatus });
		this.gv_Select.DisableFormSetting = false;
		this.gv_Select.DisplayException = false;
		this.gv_Select.EditColumn = null;
		this.gv_Select.FieldCurrency = null;
		this.gv_Select.FieldRate = null;
		this.gv_Select.FormId = null;
		this.gv_Select.GridControl = this.grd_Select;
		this.gv_Select.HasFormSetting = false;
		this.gv_Select.IgnoreFilter = false;
		this.gv_Select.IndexEditRow = 0;
		this.gv_Select.IndicatorWidth = 35;
		this.gv_Select.IsApzTable = false;
		this.gv_Select.IsInsertNewRow = false;
		this.gv_Select.IsLoadingData = false;
		this.gv_Select.IsMappingContextAction = false;
		this.gv_Select.ItemId = null;
		this.gv_Select.LstColumnAllowBatch = null;
		this.gv_Select.LstColumnNotAllowBatch = null;
		this.gv_Select.LstColumnNotEditable = null;
		this.gv_Select.LstColumNotSetting = null;
		this.gv_Select.LstOldUDF = null;
		this.gv_Select.Name = "gv_Select";
		this.gv_Select.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Select.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Select.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Select.OptionsPrint.AutoWidth = false;
		this.gv_Select.OptionsPrint.UsePrintStyles = false;
		this.gv_Select.OptionsSelection.MultiSelect = true;
		this.gv_Select.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Select.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
		this.gv_Select.OptionsView.ColumnAutoWidth = false;
		this.gv_Select.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Select.OptionsView.ShowGroupedColumns = true;
		this.gv_Select.OptionsView.ShowGroupPanel = false;
		this.gv_Select.ReadOnly = false;
		this.gv_Select.SettingReadonly = false;
		this.gv_Select.UseBatchMod = false;
		this.gc_FileName.Caption = "File Name";
		this.gc_FileName.CurrencyDateField = null;
		this.gc_FileName.CurrencyField = null;
		this.gc_FileName.CurrencyFixed = null;
		this.gc_FileName.CurrencyFixedRate = 0.0;
		this.gc_FileName.CurrencyRateField = null;
		this.gc_FileName.FieldName = "FileName";
		this.gc_FileName.Name = "gc_FileName";
		this.gc_FileName.OptionsColumn.ReadOnly = true;
		this.gc_FileName.PrimaryKey = false;
		this.gc_FileName.UnitField = null;
		this.gc_FileName.Visible = true;
		this.gc_FileName.VisibleIndex = 0;
		this.gc_FilePath.Caption = "File Path";
		this.gc_FilePath.CurrencyDateField = null;
		this.gc_FilePath.CurrencyField = null;
		this.gc_FilePath.CurrencyFixed = null;
		this.gc_FilePath.CurrencyFixedRate = 0.0;
		this.gc_FilePath.CurrencyRateField = null;
		this.gc_FilePath.FieldName = "FilePath";
		this.gc_FilePath.Name = "gc_FilePath";
		this.gc_FilePath.PrimaryKey = false;
		this.gc_FilePath.UnitField = null;
		this.gc_FileType.Caption = "File Type";
		this.gc_FileType.CurrencyDateField = null;
		this.gc_FileType.CurrencyField = null;
		this.gc_FileType.CurrencyFixed = null;
		this.gc_FileType.CurrencyFixedRate = 0.0;
		this.gc_FileType.CurrencyRateField = null;
		this.gc_FileType.FieldName = "FileType";
		this.gc_FileType.Name = "gc_FileType";
		this.gc_FileType.OptionsColumn.ReadOnly = true;
		this.gc_FileType.PrimaryKey = false;
		this.gc_FileType.UnitField = null;
		this.gc_FileType.Visible = true;
		this.gc_FileType.VisibleIndex = 1;
		this.gc_FileSize.Caption = "File Size";
		this.gc_FileSize.CurrencyDateField = null;
		this.gc_FileSize.CurrencyField = null;
		this.gc_FileSize.CurrencyFixed = null;
		this.gc_FileSize.CurrencyFixedRate = 0.0;
		this.gc_FileSize.CurrencyRateField = null;
		this.gc_FileSize.FieldName = "FileSize";
		this.gc_FileSize.Name = "gc_FileSize";
		this.gc_FileSize.OptionsColumn.ReadOnly = true;
		this.gc_FileSize.PrimaryKey = false;
		this.gc_FileSize.UnitField = null;
		this.gc_FileSize.Visible = true;
		this.gc_FileSize.VisibleIndex = 2;
		this.gc_Album.Caption = "Album";
		this.gc_Album.ColumnEdit = this.rtxt_album;
		this.gc_Album.CurrencyDateField = null;
		this.gc_Album.CurrencyField = null;
		this.gc_Album.CurrencyFixed = null;
		this.gc_Album.CurrencyFixedRate = 0.0;
		this.gc_Album.CurrencyRateField = null;
		this.gc_Album.FieldName = "AlbumId";
		this.gc_Album.Name = "gc_Album";
		this.gc_Album.PrimaryKey = false;
		this.gc_Album.UnitField = null;
		this.gc_Album.Visible = true;
		this.gc_Album.VisibleIndex = 3;
		this.rtxt_album.AutoHeight = false;
		this.rtxt_album.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rtxt_album.Name = "rtxt_album";
		this.rtxt_album.NullText = "";
		this.gc_Message.Caption = "Upload Message";
		this.gc_Message.CurrencyDateField = null;
		this.gc_Message.CurrencyField = null;
		this.gc_Message.CurrencyFixed = null;
		this.gc_Message.CurrencyFixedRate = 0.0;
		this.gc_Message.CurrencyRateField = null;
		this.gc_Message.FieldName = "Message";
		this.gc_Message.Name = "gc_Message";
		this.gc_Message.OptionsColumn.ReadOnly = true;
		this.gc_Message.PrimaryKey = false;
		this.gc_Message.UnitField = null;
		this.gc_Message.Visible = true;
		this.gc_Message.VisibleIndex = 4;
		this.gc_UploadStatus.Caption = "Upload Status";
		this.gc_UploadStatus.ColumnEdit = this.combo_UploadStatus;
		this.gc_UploadStatus.CurrencyDateField = null;
		this.gc_UploadStatus.CurrencyField = null;
		this.gc_UploadStatus.CurrencyFixed = null;
		this.gc_UploadStatus.CurrencyFixedRate = 0.0;
		this.gc_UploadStatus.CurrencyRateField = null;
		this.gc_UploadStatus.FieldName = "UploadStatus";
		this.gc_UploadStatus.Name = "gc_UploadStatus";
		this.gc_UploadStatus.OptionsColumn.ReadOnly = true;
		this.gc_UploadStatus.PrimaryKey = false;
		this.gc_UploadStatus.UnitField = null;
		this.gc_UploadStatus.Visible = true;
		this.gc_UploadStatus.VisibleIndex = 5;
		this.combo_UploadStatus.AutoHeight = false;
		this.combo_UploadStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.combo_UploadStatus.Name = "combo_UploadStatus";
		this.grd_Upload.AllowPasteExcel = false;
		this.grd_Upload.ClipboardData = null;
		this.grd_Upload.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Upload.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Upload.Location = new System.Drawing.Point(0, 0);
		this.grd_Upload.MainView = this.gv_Upload;
		this.grd_Upload.Name = "grd_Upload";
		this.grd_Upload.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.combo_SaveType, this.repositoryItemPictureEdit1, this.repositoryItemHyperLinkEdit1, this.repositoryItemHyperLinkEdit2, this.repositoryItemHyperLinkEdit3 });
		this.grd_Upload.Size = new System.Drawing.Size(970, 287);
		this.grd_Upload.TabIndex = 0;
		this.grd_Upload.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Upload });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(108, 26);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem1.Text = "Delete";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.gv_Upload.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Upload.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Upload.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Upload.ColumnKey = null;
		this.gv_Upload.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gc_SaveType, this.gc_FileId, this.gc_UploadFileName, this.gc_FileNameDrive, this.gc_UploadSize, this.gc_UploadExtension, this.gc_MimeType, this.gc_Path, this.gc_WebUrl, this.gc_LinkDownload,
			this.gc_LinkThumbnails, this.gc_Domain, this.gc_PathThumbnail, this.gc_ThumbnailsPreview
		});
		this.gv_Upload.DisableFormSetting = false;
		this.gv_Upload.DisplayException = false;
		this.gv_Upload.EditColumn = null;
		this.gv_Upload.FieldCurrency = null;
		this.gv_Upload.FieldRate = null;
		this.gv_Upload.FormId = null;
		this.gv_Upload.GridControl = this.grd_Upload;
		this.gv_Upload.HasFormSetting = false;
		this.gv_Upload.IgnoreFilter = false;
		this.gv_Upload.IndexEditRow = 0;
		this.gv_Upload.IndicatorWidth = 35;
		this.gv_Upload.IsApzTable = false;
		this.gv_Upload.IsInsertNewRow = false;
		this.gv_Upload.IsLoadingData = false;
		this.gv_Upload.IsMappingContextAction = false;
		this.gv_Upload.ItemId = null;
		this.gv_Upload.LstColumnAllowBatch = null;
		this.gv_Upload.LstColumnNotAllowBatch = null;
		this.gv_Upload.LstColumnNotEditable = null;
		this.gv_Upload.LstColumNotSetting = null;
		this.gv_Upload.LstOldUDF = null;
		this.gv_Upload.Name = "gv_Upload";
		this.gv_Upload.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Upload.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Upload.OptionsBehavior.ReadOnly = true;
		this.gv_Upload.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Upload.OptionsPrint.AutoWidth = false;
		this.gv_Upload.OptionsPrint.UsePrintStyles = false;
		this.gv_Upload.OptionsSelection.MultiSelect = true;
		this.gv_Upload.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Upload.OptionsView.BestFitMode = DevExpress.XtraGrid.Views.Grid.GridBestFitMode.Full;
		this.gv_Upload.OptionsView.ColumnAutoWidth = false;
		this.gv_Upload.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Upload.OptionsView.ShowGroupedColumns = true;
		this.gv_Upload.OptionsView.ShowGroupPanel = false;
		this.gv_Upload.ReadOnly = true;
		this.gv_Upload.RowHeight = 100;
		this.gv_Upload.SettingReadonly = false;
		this.gv_Upload.UseBatchMod = false;
		this.gv_Upload.CustomRowFilter += new DevExpress.XtraGrid.Views.Base.RowFilterEventHandler(this.gv_Upload_CustomRowFilter);
		this.gc_SaveType.Caption = "Save Type";
		this.gc_SaveType.ColumnEdit = this.combo_SaveType;
		this.gc_SaveType.CurrencyDateField = null;
		this.gc_SaveType.CurrencyField = null;
		this.gc_SaveType.CurrencyFixed = null;
		this.gc_SaveType.CurrencyFixedRate = 0.0;
		this.gc_SaveType.CurrencyRateField = null;
		this.gc_SaveType.FieldName = "SaveType";
		this.gc_SaveType.Name = "gc_SaveType";
		this.gc_SaveType.PrimaryKey = false;
		this.gc_SaveType.UnitField = null;
		this.gc_SaveType.Visible = true;
		this.gc_SaveType.VisibleIndex = 1;
		this.combo_SaveType.AutoHeight = false;
		this.combo_SaveType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.combo_SaveType.Name = "combo_SaveType";
		this.gc_FileId.Caption = "File Id";
		this.gc_FileId.CurrencyDateField = null;
		this.gc_FileId.CurrencyField = null;
		this.gc_FileId.CurrencyFixed = null;
		this.gc_FileId.CurrencyFixedRate = 0.0;
		this.gc_FileId.CurrencyRateField = null;
		this.gc_FileId.FieldName = "FileId";
		this.gc_FileId.Name = "gc_FileId";
		this.gc_FileId.PrimaryKey = false;
		this.gc_FileId.UnitField = null;
		this.gc_UploadFileName.Caption = "FileName";
		this.gc_UploadFileName.CurrencyDateField = null;
		this.gc_UploadFileName.CurrencyField = null;
		this.gc_UploadFileName.CurrencyFixed = null;
		this.gc_UploadFileName.CurrencyFixedRate = 0.0;
		this.gc_UploadFileName.CurrencyRateField = null;
		this.gc_UploadFileName.FieldName = "FileName";
		this.gc_UploadFileName.Name = "gc_UploadFileName";
		this.gc_UploadFileName.PrimaryKey = false;
		this.gc_UploadFileName.UnitField = null;
		this.gc_UploadFileName.Visible = true;
		this.gc_UploadFileName.VisibleIndex = 2;
		this.gc_FileNameDrive.Caption = "File Name Drive";
		this.gc_FileNameDrive.CurrencyDateField = null;
		this.gc_FileNameDrive.CurrencyField = null;
		this.gc_FileNameDrive.CurrencyFixed = null;
		this.gc_FileNameDrive.CurrencyFixedRate = 0.0;
		this.gc_FileNameDrive.CurrencyRateField = null;
		this.gc_FileNameDrive.FieldName = "FileNameDrive";
		this.gc_FileNameDrive.Name = "gc_FileNameDrive";
		this.gc_FileNameDrive.PrimaryKey = false;
		this.gc_FileNameDrive.UnitField = null;
		this.gc_UploadSize.Caption = "Size";
		this.gc_UploadSize.CurrencyDateField = null;
		this.gc_UploadSize.CurrencyField = null;
		this.gc_UploadSize.CurrencyFixed = null;
		this.gc_UploadSize.CurrencyFixedRate = 0.0;
		this.gc_UploadSize.CurrencyRateField = null;
		this.gc_UploadSize.FieldName = "Size";
		this.gc_UploadSize.Name = "gc_UploadSize";
		this.gc_UploadSize.PrimaryKey = false;
		this.gc_UploadSize.UnitField = null;
		this.gc_UploadSize.Visible = true;
		this.gc_UploadSize.VisibleIndex = 3;
		this.gc_UploadExtension.Caption = "Extension";
		this.gc_UploadExtension.CurrencyDateField = null;
		this.gc_UploadExtension.CurrencyField = null;
		this.gc_UploadExtension.CurrencyFixed = null;
		this.gc_UploadExtension.CurrencyFixedRate = 0.0;
		this.gc_UploadExtension.CurrencyRateField = null;
		this.gc_UploadExtension.FieldName = "Extension";
		this.gc_UploadExtension.Name = "gc_UploadExtension";
		this.gc_UploadExtension.PrimaryKey = false;
		this.gc_UploadExtension.UnitField = null;
		this.gc_UploadExtension.Visible = true;
		this.gc_UploadExtension.VisibleIndex = 4;
		this.gc_MimeType.Caption = "Mime Type";
		this.gc_MimeType.CurrencyDateField = null;
		this.gc_MimeType.CurrencyField = null;
		this.gc_MimeType.CurrencyFixed = null;
		this.gc_MimeType.CurrencyFixedRate = 0.0;
		this.gc_MimeType.CurrencyRateField = null;
		this.gc_MimeType.FieldName = "MimeType";
		this.gc_MimeType.Name = "gc_MimeType";
		this.gc_MimeType.PrimaryKey = false;
		this.gc_MimeType.UnitField = null;
		this.gc_MimeType.Visible = true;
		this.gc_MimeType.VisibleIndex = 5;
		this.gc_Path.Caption = "Path";
		this.gc_Path.CurrencyDateField = null;
		this.gc_Path.CurrencyField = null;
		this.gc_Path.CurrencyFixed = null;
		this.gc_Path.CurrencyFixedRate = 0.0;
		this.gc_Path.CurrencyRateField = null;
		this.gc_Path.FieldName = "Path";
		this.gc_Path.Name = "gc_Path";
		this.gc_Path.PrimaryKey = false;
		this.gc_Path.UnitField = null;
		this.gc_WebUrl.Caption = "Web Url";
		this.gc_WebUrl.ColumnEdit = this.repositoryItemHyperLinkEdit2;
		this.gc_WebUrl.CurrencyDateField = null;
		this.gc_WebUrl.CurrencyField = null;
		this.gc_WebUrl.CurrencyFixed = null;
		this.gc_WebUrl.CurrencyFixedRate = 0.0;
		this.gc_WebUrl.CurrencyRateField = null;
		this.gc_WebUrl.FieldName = "WebUrl";
		this.gc_WebUrl.Name = "gc_WebUrl";
		this.gc_WebUrl.PrimaryKey = false;
		this.gc_WebUrl.UnitField = null;
		this.gc_WebUrl.Visible = true;
		this.gc_WebUrl.VisibleIndex = 6;
		this.repositoryItemHyperLinkEdit2.AutoHeight = false;
		this.repositoryItemHyperLinkEdit2.Name = "repositoryItemHyperLinkEdit2";
		this.gc_LinkDownload.Caption = "LinkDownLoad";
		this.gc_LinkDownload.ColumnEdit = this.repositoryItemHyperLinkEdit3;
		this.gc_LinkDownload.CurrencyDateField = null;
		this.gc_LinkDownload.CurrencyField = null;
		this.gc_LinkDownload.CurrencyFixed = null;
		this.gc_LinkDownload.CurrencyFixedRate = 0.0;
		this.gc_LinkDownload.CurrencyRateField = null;
		this.gc_LinkDownload.FieldName = "LinkDownload";
		this.gc_LinkDownload.Name = "gc_LinkDownload";
		this.gc_LinkDownload.PrimaryKey = false;
		this.gc_LinkDownload.UnitField = null;
		this.gc_LinkDownload.Visible = true;
		this.gc_LinkDownload.VisibleIndex = 7;
		this.repositoryItemHyperLinkEdit3.AutoHeight = false;
		this.repositoryItemHyperLinkEdit3.Name = "repositoryItemHyperLinkEdit3";
		this.gc_LinkThumbnails.Caption = "Link Thumbnails";
		this.gc_LinkThumbnails.ColumnEdit = this.repositoryItemHyperLinkEdit1;
		this.gc_LinkThumbnails.CurrencyDateField = null;
		this.gc_LinkThumbnails.CurrencyField = null;
		this.gc_LinkThumbnails.CurrencyFixed = null;
		this.gc_LinkThumbnails.CurrencyFixedRate = 0.0;
		this.gc_LinkThumbnails.CurrencyRateField = null;
		this.gc_LinkThumbnails.FieldName = "LinkThumbnails";
		this.gc_LinkThumbnails.Name = "gc_LinkThumbnails";
		this.gc_LinkThumbnails.PrimaryKey = false;
		this.gc_LinkThumbnails.UnitField = null;
		this.gc_LinkThumbnails.Visible = true;
		this.gc_LinkThumbnails.VisibleIndex = 8;
		this.repositoryItemHyperLinkEdit1.AutoHeight = false;
		this.repositoryItemHyperLinkEdit1.Name = "repositoryItemHyperLinkEdit1";
		this.gc_Domain.Caption = "Domain";
		this.gc_Domain.CurrencyDateField = null;
		this.gc_Domain.CurrencyField = null;
		this.gc_Domain.CurrencyFixed = null;
		this.gc_Domain.CurrencyFixedRate = 0.0;
		this.gc_Domain.CurrencyRateField = null;
		this.gc_Domain.FieldName = "Domain";
		this.gc_Domain.Name = "gc_Domain";
		this.gc_Domain.PrimaryKey = false;
		this.gc_Domain.UnitField = null;
		this.gc_PathThumbnail.Caption = "Path Thumbnails";
		this.gc_PathThumbnail.CurrencyDateField = null;
		this.gc_PathThumbnail.CurrencyField = null;
		this.gc_PathThumbnail.CurrencyFixed = null;
		this.gc_PathThumbnail.CurrencyFixedRate = 0.0;
		this.gc_PathThumbnail.CurrencyRateField = null;
		this.gc_PathThumbnail.FieldName = "PathThumbnail";
		this.gc_PathThumbnail.Name = "gc_PathThumbnail";
		this.gc_PathThumbnail.PrimaryKey = false;
		this.gc_PathThumbnail.UnitField = null;
		this.gc_ThumbnailsPreview.Caption = "Preview";
		this.gc_ThumbnailsPreview.ColumnEdit = this.repositoryItemPictureEdit1;
		this.gc_ThumbnailsPreview.CurrencyDateField = null;
		this.gc_ThumbnailsPreview.CurrencyField = null;
		this.gc_ThumbnailsPreview.CurrencyFixed = null;
		this.gc_ThumbnailsPreview.CurrencyFixedRate = 0.0;
		this.gc_ThumbnailsPreview.CurrencyRateField = null;
		this.gc_ThumbnailsPreview.FieldName = "ThumbnailsPreview";
		this.gc_ThumbnailsPreview.Name = "gc_ThumbnailsPreview";
		this.gc_ThumbnailsPreview.PrimaryKey = false;
		this.gc_ThumbnailsPreview.UnitField = null;
		this.gc_ThumbnailsPreview.Visible = true;
		this.gc_ThumbnailsPreview.VisibleIndex = 0;
		this.repositoryItemPictureEdit1.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemPictureEdit1.Name = "repositoryItemPictureEdit1";
		this.repositoryItemPictureEdit1.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
		this.repositoryItemPictureEdit1.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
		this.repositoryItemPictureEdit1.ZoomingOperationMode = DevExpress.XtraEditors.Repository.ZoomingOperationMode.MouseWheel;
		this.btn_Upload.EditControl = null;
		this.btn_Upload.EditValue = "Upload";
		this.btn_Upload.FieldName = null;
		this.btn_Upload.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Upload.ImageOptions.Image");
		this.btn_Upload.IsSearchControl = false;
		this.btn_Upload.Location = new System.Drawing.Point(907, 9);
		this.btn_Upload.Name = "btn_Upload";
		this.btn_Upload.ReadOnly = false;
		this.btn_Upload.SettingInVisible = false;
		this.btn_Upload.SettingReadonly = false;
		this.btn_Upload.Size = new System.Drawing.Size(75, 23);
		this.btn_Upload.TabIndex = 14;
		this.btn_Upload.Text = "Upload";
		this.btn_Upload.Click += new System.EventHandler(this.simpleButtonEx1_Click);
		this.simpleButtonEx2.EditControl = null;
		this.simpleButtonEx2.EditValue = "Select File";
		this.simpleButtonEx2.FieldName = null;
		this.simpleButtonEx2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButtonEx2.ImageOptions.Image");
		this.simpleButtonEx2.IsSearchControl = false;
		this.simpleButtonEx2.Location = new System.Drawing.Point(826, 9);
		this.simpleButtonEx2.Name = "simpleButtonEx2";
		this.simpleButtonEx2.ReadOnly = false;
		this.simpleButtonEx2.SettingInVisible = false;
		this.simpleButtonEx2.SettingReadonly = false;
		this.simpleButtonEx2.Size = new System.Drawing.Size(75, 23);
		this.simpleButtonEx2.TabIndex = 14;
		this.simpleButtonEx2.Text = "Select File";
		this.simpleButtonEx2.Click += new System.EventHandler(this.simpleButtonEx2_Click);
		this.openFileDialog.FileName = "openFileDialog";
		this.openFileDialog.Multiselect = true;
		this.simpleButtonEx3.EditControl = null;
		this.simpleButtonEx3.EditValue = "Reset";
		this.simpleButtonEx3.FieldName = null;
		this.simpleButtonEx3.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButtonEx3.ImageOptions.Image");
		this.simpleButtonEx3.IsSearchControl = false;
		this.simpleButtonEx3.Location = new System.Drawing.Point(759, 9);
		this.simpleButtonEx3.Name = "simpleButtonEx3";
		this.simpleButtonEx3.ReadOnly = false;
		this.simpleButtonEx3.SettingInVisible = false;
		this.simpleButtonEx3.SettingReadonly = false;
		this.simpleButtonEx3.Size = new System.Drawing.Size(61, 23);
		this.simpleButtonEx3.TabIndex = 14;
		this.simpleButtonEx3.Text = "Reset";
		this.simpleButtonEx3.Click += new System.EventHandler(this.simpleButtonEx3_Click);
		this.imgCollection.ImageStream = (DevExpress.Utils.ImageCollectionStreamer)resources.GetObject("imgCollection.ImageStream");
		this.imgCollection.Images.SetKeyName(0, "save_16x16.png");
		this.imgCollection.Images.SetKeyName(1, "moveup_16x16.png");
		this.imgCollection.Images.SetKeyName(2, "reset_16x16.png");
		this.imgCollection.Images.SetKeyName(3, "addfile_16x16.png");
		this.txt_album.EditControl = null;
		this.txt_album.EditValue = "-1";
		this.txt_album.FieldName = null;
		this.txt_album.IsSearchControl = false;
		this.txt_album.Location = new System.Drawing.Point(90, 6);
		this.txt_album.Name = "txt_album";
		this.txt_album.Properties.AllowMouseWheel = false;
		this.txt_album.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_album.Properties.DisplayMember = "Name";
		this.txt_album.Properties.NullText = "";
		this.txt_album.Properties.ValueMember = "Id";
		this.txt_album.SettingInVisible = false;
		this.txt_album.SettingReadonly = false;
		this.txt_album.Size = new System.Drawing.Size(139, 20);
		this.txt_album.TabIndex = 16;
		this.txt_album.EditValueChanged += new System.EventHandler(this.txt_album_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Album";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 9);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(77, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 15;
		this.labelLine2.Utext = "Album";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(994, 523);
		base.Controls.Add(this.txt_album);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.simpleButtonEx3);
		base.Controls.Add(this.simpleButtonEx2);
		base.Controls.Add(this.btn_Upload);
		base.Controls.Add(this.splitContainerControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_UploadAttachment.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UploadAttachment";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Upload Attachment";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UploadAttachment_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Select).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Select).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rtxt_album).EndInit();
		((System.ComponentModel.ISupportInitialize)this.combo_UploadStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Upload).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Upload).EndInit();
		((System.ComponentModel.ISupportInitialize)this.combo_SaveType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHyperLinkEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemPictureEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.imgCollection).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_album.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
