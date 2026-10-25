using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.DMS;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.DMS;

public class frm_UploadItemImage : XtraFormEx
{
	private enum UploadStatus
	{
		Waiting,
		Success,
		Fail
	}

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem ctm_DeleteEmp;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_ItemCode;

	private RepositoryItemButtonEditCFL rep_CFL;

	private GridColumnEx gc_ItemName;

	private GridColumnEx gc_FileName;

	private GridColumnEx gc_Process;

	private GridColumnEx gc_UploadStatus;

	private GridColumnEx gc_UploadUrl;

	private GridViewEx gridViewEx1;

	private SimpleButtonEx btn_Browse;

	private TextEditEx txt_ImageFolder;

	private LabelLine lbl_UploadFrom;

	private RepositoryItemButtonEdit rep_Browse;

	private GridColumnEx gc_Image;

	private RepositoryItemPictureEdit rep_Image;

	private RepositoryItemProgressBar rep_Progress;

	private GridColumnEx gc_FilePath;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gc_UploadContent;

	private CheckEditEx chk_Zoom;

	private GridColumnEx gc_ViewImage;

	private RepositoryItemButtonEdit btn_ViewImage;

	public int AbsEntry { get; set; }

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

	public frm_UploadItemImage()
	{
		InitializeComponent();
		OnLoadAsync += frm_UploadItemImage_Load;
	}

	private async Task frm_UploadItemImage_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			gv_Doc.RowHeight = -1;
			LoadInit();
			FormMode = Enums.FormViewModeType.View;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_D_UUPIM");
			tableStructure.Columns.Add("ItemCode", typeof(string));
			tableStructure.Columns.Add("ItemName", typeof(string));
			tableStructure.Columns.Add("UploadStatus", typeof(string));
			tableStructure.Columns.Add("UploadContent", typeof(string));
			tableStructure.Columns.Add("UploadUrl", typeof(string));
			tableStructure.Columns.Add("Process", typeof(string));
			tableStructure.Columns.Add("FilePath", typeof(string));
			tableStructure.Columns.Add("Image", typeof(Image));
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
			AddNewLine();
			txt_ImageFolder.EditValue = null;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInit()
	{
		try
		{
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "0", "Chờ tải lên" },
				{ "1", "Thành công" },
				{ "2", "Lỗi" }
			};
			rep_Status.SetDataSource(dic, hideCodeColumn: true);
			rep_CFL.ValueMember = "ItemCode";
			rep_CFL.ListMandatoryField = "ItemCode,ItemName,LinkThumbnails";
			rep_CFL.FindCFLWith(Enums.TypeCFL.ListItems, true, "", Enums.ItemObjectType.NoItemType, null);
			rep_CFL.TypeDrillDown = ApzObjectTypes.oItems;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_CFL;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, new Action<DataRow>(FillData));
			rep_CFL.HandleChooseItemMulti = HandleChooseItemMulti;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_CFL;
			repositoryItemButtonEditCFL2.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.AfterShowCFL, new Action<frm_CFL>(AfterShow));
			rep_CFL.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AfterShow(frm_CFL frm)
	{
		try
		{
			EnumerableRowCollection<DataRow> source = from t in grd_Doc.DatasourceTable().AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
				select t;
			IEnumerable<object> selectedEmps = source.Select((DataRow t) => t["ItemCode"]).Distinct();
			DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
			if (dataTable.Rows.Count > 0 && dataTable != null)
			{
				DataRow[] source2 = (from s in dataTable.AsEnumerable()
					where !selectedEmps.Contains(Function.ToString(s["ItemCode"]))
					select s).ToArray();
				if (source2.Count() > 0)
				{
					frm.gv_CFL.GridControl.DataSource = source2.CopyToDataTable();
					frm.gv_CFL.ExpandAllGroups();
				}
				else
				{
					dataTable.Clear();
					dataTable.AcceptChanges();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewLine()
	{
		try
		{
			DataTable dataTable = grd_Doc.DatasourceTable();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Process"] = 0;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HandleChooseItemMulti(List<DataRow> rows)
	{
		try
		{
			if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			gv_Doc.SetFocusedRowCellValue("ItemCode", Function.ToString(rows[0]["ItemCode"]));
			gv_Doc.SetFocusedRowCellValue("ItemName", Function.ToString(rows[0]["ItemName"]));
			gv_Doc.SetFocusedRowCellValue("UploadUrl", Function.ToString(rows[0]["LinkThumbnails"]));
			gv_Doc.SetFocusedRowCellValue("UploadStatus", 0);
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])));
				int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
				if (dataRow == null)
				{
					AddNewLine();
					DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])));
					dataSourceIndex = dataTable.Rows.IndexOf(row);
				}
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "ItemCode", Function.ToString(rows[i]["ItemCode"]));
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "ItemName", Function.ToString(rows[i]["ItemName"]));
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "UploadUrl", Function.ToString(rows[i]["LinkThumbnails"]));
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "UploadStatus", 0);
			}
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"]))) == null)
			{
				AddNewLine();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void FillData(DataRow dataRow)
	{
		try
		{
			gv_Doc.SetFocusedRowCellValue("ItemCode", Function.ToString(dataRow["ItemCode"]));
			gv_Doc.SetFocusedRowCellValue("ItemName", Function.ToString(dataRow["ItemName"]));
			gv_Doc.SetFocusedRowCellValue("UploadUrl", Function.ToString(dataRow["LinkThumbnails"]));
			gv_Doc.SetFocusedRowCellValue("UploadStatus", 0);
			gv_Doc.PostEditor();
			gv_Doc.RefreshData();
			if ((grd_Doc.DataSource as DataTable).AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"]))) == null)
			{
				AddNewLine();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
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
				await frm_UploadItemImage_Load(null, null);
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
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				DataTable dtUpload = grd_Doc.DatasourceTable();
				List<Task> listTask = new List<Task>();
				if (FormMode == Enums.FormViewModeType.Update)
				{
					WinCommons._OpenProcessing();
					Stopwatch stopwatch = new Stopwatch();
					stopwatch.Start();
					for (int i = 0; i < dtUpload.Rows.Count; i++)
					{
						string value = Function.ToString(dtUpload.Rows[i]["ItemCode"]);
						string text = Function.ToString(dtUpload.Rows[i]["UploadStatus"]);
						if (!string.IsNullOrEmpty(value) && !(text == Function.ToString(1)))
						{
							await UploadImage(dtUpload.Rows[i], i);
						}
					}
					Logging.Write("TRACE", "Thời gian tải lên " + listTask.Count + " ảnh: " + stopwatch.Elapsed.TotalSeconds.ToString("n2") + " (s)");
					WinCommons._CloseProcessing();
				}
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task UploadImage(DataRow dr, int i)
	{
		try
		{
			new UploadOneDriveFileResultImpl();
			string objectCode = Function.ToString(dr["ItemCode"]);
			string text = Function.ToString(dr["FileName"]);
			string text2 = Function.ToString(dr["FilePath"]);
			UploadOneDriveInputImpl uploadOneDriveInputImpl = new UploadOneDriveInputImpl();
			uploadOneDriveInputImpl.ObjectCode = objectCode;
			uploadOneDriveInputImpl.ObjectTypes = ApzObjectTypes.oUploadItemImage;
			uploadOneDriveInputImpl.FileName = text;
			uploadOneDriveInputImpl.FilePath = text2 + "\\" + text;
			Tuple<string, UploadOneDriveFileResultImpl> tuple = await _httpClient.FileNetworkManagerClient.UploadFileOneDrive(uploadOneDriveInputImpl);
			string item = tuple.Item1;
			UploadOneDriveFileResultImpl item2 = tuple.Item2;
			DataTable dataTable = grd_Doc.DatasourceTable();
			if (string.IsNullOrEmpty(item) && item2 != null)
			{
				dataTable.Rows[i]["UploadContent"] = "";
				dataTable.Rows[i]["UploadStatus"] = 1;
				dataTable.Rows[i]["UploadUrl"] = item2.LinkThumbnails;
				dataTable.Rows[i]["Process"] = 100;
			}
			else
			{
				dataTable.Rows[i]["UploadContent"] = item;
				dataTable.Rows[i]["UploadStatus"] = 2;
				dataTable.Rows[i]["UploadUrl"] = "";
				dataTable.Rows[i]["Process"] = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_UploadItemImage_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
		{
			e.Cancel = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_DeleteEmp_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_Doc.GetSelectedRows();
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			gv_Doc.BeginUpdate();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataSourceTable.Rows[gv_Doc.GetDataSourceRowIndex(selectedRows[i])]["ItemCode"])))
				{
					gv_Doc.UnselectRow(selectedRows[i]);
				}
			}
			selectedRows = gv_Doc.GetSelectedRows();
			bool flag = false;
			if (selectedRows.Length != 0)
			{
				flag = true;
			}
			gv_Doc.DeleteSelectedRows();
			gv_Doc.EndUpdate();
			dataSourceTable.AcceptChanges();
			if ((FormMode == Enums.FormViewModeType.View) & flag)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Browse_Click(object sender, EventArgs e)
	{
		try
		{
			FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
			folderBrowserDialogEx.ShowEditBox = true;
			folderBrowserDialogEx.ShowFullPathInEditBox = true;
			folderBrowserDialogEx.RootFolder = Environment.SpecialFolder.MyComputer;
			if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
			{
				if (!string.IsNullOrEmpty(folderBrowserDialogEx.SelectedPath))
				{
					txt_ImageFolder.Text = folderBrowserDialogEx.SelectedPath + "\\";
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Browse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.ShowReadOnly = true;
			openFileDialog.Multiselect = false;
			openFileDialog.Filter = "Files Pictures (*.BMP,*.GIF,*.JPG;*.JPEG,*.PNG)|*.BMP;*.GIF;*.JPG;*.JPEG;*.PNG";
			if (openFileDialog.ShowDialog() != DialogResult.OK || openFileDialog.FileNames.Length == 0)
			{
				return;
			}
			if (!string.IsNullOrEmpty(openFileDialog.FileNames[0]))
			{
				Bitmap value = new Bitmap(openFileDialog.FileNames[0]);
				gv_Doc.SetFocusedRowCellValue("Image", value);
				gv_Doc.SetFocusedRowCellValue("FileName", openFileDialog.SafeFileNames[0]);
				gv_Doc.SetFocusedRowCellValue("FilePath", Path.GetDirectoryName(openFileDialog.FileNames[0]));
				gv_Doc.SetFocusedRowCellValue("UploadStatus", 0);
				gv_Doc.SetFocusedRowCellValue("Process", 0);
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InvalidFolderPath, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_ImageFolder_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			FileInfo[] array = (from s in new DirectoryInfo(Function.ToString(txt_ImageFolder.EditValue)).GetFiles().AsEnumerable()
				where s.Name.ToUpper().EndsWith(".BMP") || s.Name.ToUpper().EndsWith(".JPG") || s.Name.ToUpper().EndsWith(".GIF") || s.Name.ToUpper().EndsWith(".PNG") || s.Name.ToUpper().EndsWith(".JPEG")
				select s).ToArray();
			DataTable dataTable = grd_Doc.DataSourceTable();
			dataTable.Rows.Clear();
			WinCommons._OpenProcessing();
			bool flag = array.Length <= 100;
			if (!flag)
			{
				gc_Image.Visible = false;
				chk_Zoom.Visible = false;
			}
			else
			{
				gc_Image.Visible = true;
				chk_Zoom.Visible = true;
			}
			FileInfo[] array2 = array;
			foreach (FileInfo fileInfo in array2)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int num2 = 0; num2 < rep_CFL.DataSource.Rows.Count; num2++)
				{
					DataRow dataRow2 = rep_CFL.DataSource.Rows[num2];
					if (fileInfo.Name.Split('.')[0].ToUpper().Equals(Function.ToString(dataRow2["ItemCode"]).ToUpper()))
					{
						dataRow["ItemCode"] = dataRow2["ItemCode"];
						dataRow["ItemName"] = dataRow2["ItemName"];
						dataRow["UploadUrl"] = dataRow2["LinkThumbnails"];
						break;
					}
				}
				dataRow["FileName"] = fileInfo.Name;
				dataRow["FilePath"] = fileInfo.Directory;
				dataRow["UploadStatus"] = 0;
				if (flag)
				{
					Image image = Image.FromStream(new FileStream(fileInfo.FullName, FileMode.Open));
					Bitmap value = Function.ResizeBitmap(image, 100, image.Height * 100 / image.Width);
					dataRow["Image"] = value;
				}
				dataTable.Rows.Add(dataRow);
			}
			WinCommons._CloseProcessing();
			if (dataTable.Rows.Count > 0 && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ItemCode"])) && string.IsNullOrWhiteSpace(Function.ToString(r["FileName"]))) == null)
			{
				AddNewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Zoom_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseBool(chk_Zoom.Checked))
			{
				gv_Doc.RowHeight = 100;
			}
			else
			{
				gv_Doc.RowHeight = -1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ViewImage_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataRow focusedDataRow = ((ButtonEdit)sender).GetGridViewFromEditor().GetFocusedDataRow();
			Process.Start(Path.Combine(Function.ToString(focusedDataRow["FilePath"]), Function.ToString(focusedDataRow["FileName"])));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_UploadItemImage));
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_DeleteEmp = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FileName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Browse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_FilePath = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Image = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Image = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.gc_Process = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Progress = new DevExpress.XtraEditors.Repository.RepositoryItemProgressBar();
		this.gc_UploadStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_UploadContent = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UploadUrl = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ViewImage = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ViewImage = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Browse = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_ImageFolder = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_UploadFrom = new Apzon.Controls.Controls.LabelLine();
		this.chk_Zoom = new Apzon.Controls.Editor.CheckEditEx();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Browse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Image).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Progress).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ViewImage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ImageFolder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Zoom.Properties).BeginInit();
		base.SuspendLayout();
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.ctm_DeleteEmp });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(108, 26);
		this.ctm_DeleteEmp.Name = "ctm_DeleteEmp";
		this.ctm_DeleteEmp.Size = new System.Drawing.Size(107, 22);
		this.ctm_DeleteEmp.Text = "Delete";
		this.ctm_DeleteEmp.Click += new System.EventHandler(this.ctm_DeleteEmp_Click);
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
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "Upload";
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
		this.btn_OK.Text = "Upload";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctx_Doc;
		this.grd_Doc.Location = new System.Drawing.Point(2, 38);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_CFL, this.rep_Browse, this.rep_Image, this.rep_Progress, this.rep_Status, this.btn_ViewImage });
		this.grd_Doc.Size = new System.Drawing.Size(1129, 438);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Doc, this.gridViewEx1 });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gc_ItemCode, this.gc_ItemName, this.gc_FileName, this.gc_FilePath, this.gc_Image, this.gc_Process, this.gc_UploadStatus, this.gc_UploadContent, this.gc_UploadUrl, this.gc_ViewImage });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUploadItemImage";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_UUPIM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
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
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
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
		this.gc_ItemCode.Width = 150;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.gc_FileName.Caption = "Tên file";
		this.gc_FileName.ColumnEdit = this.rep_Browse;
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
		this.gc_FileName.VisibleIndex = 2;
		this.gc_FileName.Width = 150;
		this.rep_Browse.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.rep_Browse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Browse.Name = "rep_Browse";
		this.rep_Browse.ReadOnly = true;
		this.rep_Browse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Browse_ButtonClick);
		this.gc_FilePath.Caption = "Đường dẫn file";
		this.gc_FilePath.CurrencyDateField = null;
		this.gc_FilePath.CurrencyField = null;
		this.gc_FilePath.CurrencyFixed = null;
		this.gc_FilePath.CurrencyFixedRate = 0.0;
		this.gc_FilePath.CurrencyRateField = null;
		this.gc_FilePath.FieldName = "FilePath";
		this.gc_FilePath.Name = "gc_FilePath";
		this.gc_FilePath.OptionsColumn.ReadOnly = true;
		this.gc_FilePath.PrimaryKey = false;
		this.gc_FilePath.UnitField = null;
		this.gc_FilePath.Visible = true;
		this.gc_FilePath.VisibleIndex = 3;
		this.gc_FilePath.Width = 150;
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
		this.gc_Image.VisibleIndex = 4;
		this.gc_Image.Width = 150;
		this.rep_Image.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.True;
		this.rep_Image.Name = "rep_Image";
		this.rep_Image.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
		this.rep_Image.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
		this.rep_Image.ZoomingOperationMode = DevExpress.XtraEditors.Repository.ZoomingOperationMode.MouseWheel;
		this.gc_Process.Caption = "Tiến trình";
		this.gc_Process.ColumnEdit = this.rep_Progress;
		this.gc_Process.CurrencyDateField = null;
		this.gc_Process.CurrencyField = null;
		this.gc_Process.CurrencyFixed = null;
		this.gc_Process.CurrencyFixedRate = 0.0;
		this.gc_Process.CurrencyRateField = null;
		this.gc_Process.FieldName = "Process";
		this.gc_Process.Name = "gc_Process";
		this.gc_Process.OptionsColumn.ReadOnly = true;
		this.gc_Process.PrimaryKey = false;
		this.gc_Process.UnitField = null;
		this.gc_Process.Visible = true;
		this.gc_Process.VisibleIndex = 6;
		this.gc_Process.Width = 100;
		this.rep_Progress.Name = "rep_Progress";
		this.gc_UploadStatus.Caption = "Trạng thái";
		this.gc_UploadStatus.ColumnEdit = this.rep_Status;
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
		this.gc_UploadStatus.VisibleIndex = 7;
		this.gc_UploadStatus.Width = 100;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gc_UploadContent.Caption = "Ghi chú";
		this.gc_UploadContent.CurrencyDateField = null;
		this.gc_UploadContent.CurrencyField = null;
		this.gc_UploadContent.CurrencyFixed = null;
		this.gc_UploadContent.CurrencyFixedRate = 0.0;
		this.gc_UploadContent.CurrencyRateField = null;
		this.gc_UploadContent.FieldName = "UploadContent";
		this.gc_UploadContent.Name = "gc_UploadContent";
		this.gc_UploadContent.OptionsColumn.ReadOnly = true;
		this.gc_UploadContent.PrimaryKey = false;
		this.gc_UploadContent.UnitField = null;
		this.gc_UploadContent.Visible = true;
		this.gc_UploadContent.VisibleIndex = 8;
		this.gc_UploadContent.Width = 150;
		this.gc_UploadUrl.Caption = "URL tải lên";
		this.gc_UploadUrl.CurrencyDateField = null;
		this.gc_UploadUrl.CurrencyField = null;
		this.gc_UploadUrl.CurrencyFixed = null;
		this.gc_UploadUrl.CurrencyFixedRate = 0.0;
		this.gc_UploadUrl.CurrencyRateField = null;
		this.gc_UploadUrl.FieldName = "UploadUrl";
		this.gc_UploadUrl.Name = "gc_UploadUrl";
		this.gc_UploadUrl.OptionsColumn.ReadOnly = true;
		this.gc_UploadUrl.PrimaryKey = false;
		this.gc_UploadUrl.UnitField = null;
		this.gc_UploadUrl.Visible = true;
		this.gc_UploadUrl.VisibleIndex = 9;
		this.gc_UploadUrl.Width = 150;
		this.gc_ViewImage.Caption = "Xem ảnh";
		this.gc_ViewImage.ColumnEdit = this.btn_ViewImage;
		this.gc_ViewImage.CurrencyDateField = null;
		this.gc_ViewImage.CurrencyField = null;
		this.gc_ViewImage.CurrencyFixed = null;
		this.gc_ViewImage.CurrencyFixedRate = 0.0;
		this.gc_ViewImage.CurrencyRateField = null;
		this.gc_ViewImage.Name = "gc_ViewImage";
		this.gc_ViewImage.PrimaryKey = false;
		this.gc_ViewImage.UnitField = null;
		this.gc_ViewImage.Visible = true;
		this.gc_ViewImage.VisibleIndex = 5;
		this.btn_ViewImage.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_ViewImage.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ViewImage.Name = "btn_ViewImage";
		this.btn_ViewImage.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.btn_ViewImage.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ViewImage_ButtonClick);
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
		this.gridViewEx1.HasFormSetting = false;
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
		this.gridViewEx1.LstOldUDF = null;
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
		this.btn_Browse.EditControl = null;
		this.btn_Browse.EditValue = "";
		this.btn_Browse.FieldName = null;
		this.btn_Browse.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Browse.ImageOptions.Image");
		this.btn_Browse.ImageOptions.ImageToTextAlignment = DevExpress.XtraEditors.ImageAlignToText.TopCenter;
		this.btn_Browse.IsSearchControl = false;
		this.btn_Browse.Location = new System.Drawing.Point(410, 8);
		this.btn_Browse.Name = "btn_Browse";
		this.btn_Browse.ReadOnly = false;
		this.btn_Browse.SettingInVisible = false;
		this.btn_Browse.SettingReadonly = false;
		this.btn_Browse.Size = new System.Drawing.Size(43, 22);
		this.btn_Browse.TabIndex = 13;
		this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
		this.txt_ImageFolder.EditControl = null;
		this.txt_ImageFolder.FieldName = null;
		this.txt_ImageFolder.IsSearchControl = false;
		this.txt_ImageFolder.Location = new System.Drawing.Point(115, 9);
		this.txt_ImageFolder.Name = "txt_ImageFolder";
		this.txt_ImageFolder.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.txt_ImageFolder.Properties.Appearance.Options.UseBackColor = true;
		this.txt_ImageFolder.Properties.ReadOnly = true;
		this.txt_ImageFolder.SettingInVisible = false;
		this.txt_ImageFolder.SettingReadonly = false;
		this.txt_ImageFolder.Size = new System.Drawing.Size(289, 20);
		this.txt_ImageFolder.TabIndex = 12;
		this.txt_ImageFolder.EditValueChanged += new System.EventHandler(this.txt_ImageFolder_EditValueChanged);
		this.lbl_UploadFrom.EditControl = null;
		this.lbl_UploadFrom.EditValue = "Tải ảnh từ thư mục";
		this.lbl_UploadFrom.FieldName = null;
		this.lbl_UploadFrom.HideWhenEmpty = false;
		this.lbl_UploadFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_UploadFrom.Location = new System.Drawing.Point(2, 12);
		this.lbl_UploadFrom.Name = "lbl_UploadFrom";
		this.lbl_UploadFrom.ReadOnly = false;
		this.lbl_UploadFrom.SettingInVisible = false;
		this.lbl_UploadFrom.SettingReadonly = false;
		this.lbl_UploadFrom.ShowUnderLine = true;
		this.lbl_UploadFrom.Size = new System.Drawing.Size(107, 20);
		this.lbl_UploadFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_UploadFrom.TabIndex = 14;
		this.lbl_UploadFrom.Utext = "Tải ảnh từ thư mục";
		this.chk_Zoom.EditControl = null;
		this.chk_Zoom.FieldName = null;
		this.chk_Zoom.IsSearchControl = false;
		this.chk_Zoom.Location = new System.Drawing.Point(459, 9);
		this.chk_Zoom.Name = "chk_Zoom";
		this.chk_Zoom.Properties.Caption = "Phóng to ảnh";
		this.chk_Zoom.SettingInVisible = false;
		this.chk_Zoom.SettingReadonly = false;
		this.chk_Zoom.Size = new System.Drawing.Size(88, 20);
		this.chk_Zoom.TabIndex = 15;
		this.chk_Zoom.CheckedChanged += new System.EventHandler(this.chk_Zoom_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1135, 510);
		base.Controls.Add(this.chk_Zoom);
		base.Controls.Add(this.txt_ImageFolder);
		base.Controls.Add(this.lbl_UploadFrom);
		base.Controls.Add(this.btn_Browse);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oUploadItemImage";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsSetting;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UploadItemImage";
		this.Text = "Tải ảnh sản phẩm";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UploadItemImage_FormClosing);
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Browse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Image).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Progress).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ViewImage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ImageFolder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Zoom.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
