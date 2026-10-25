using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.UserControls;

public class ucAttachment : XtraUserControl
{
	private ApzObjectTypes formObjectType;

	public string FormController = "";

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Action<List<DataRow>> BrowsedFile;

	public Action DeletedFile;

	public Action<int> BeforeDelete;

	private IContainer components;

	public SimpleButtonEx btDelete;

	public SimpleButtonEx btDisplay;

	public SimpleButtonEx btBrowse;

	public GridControlEx grd_File;

	public GridViewEx gv_File;

	public GridColumnEx gcTrgtPath;

	public GridColumnEx gcFileName;

	public GridColumnEx gcSrcPath;

	public GridColumnEx gcDate;

	public GridColumnEx gcUsrID;

	public GridColumnEx gcCopied;

	public GridColumnEx gcOverride;

	public GridColumnEx gcSubPath;

	public GridColumnEx gcFileExt;

	private GridColumnEx CheckExist;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private Panel panel1;

	public string FormId
	{
		get
		{
			return gv_File.FormId;
		}
		set
		{
			gv_File.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return gv_File.ItemId;
		}
		set
		{
			gv_File.ItemId = value;
		}
	}

	public DataTable DataSource
	{
		get
		{
			return grd_File.DataSource as DataTable;
		}
		set
		{
			grd_File.DataSource = value;
		}
	}

	public bool SetEnable
	{
		set
		{
			btBrowse.Enabled = value;
			btDelete.Enabled = value;
			btDisplay.Enabled = value;
		}
	}

	public event EventHandler ButtonBrowse_Click;

	public event EventHandler ButtonDelete_Click;

	public event EventHandler ButtonDisplay_Click;

	public void EnableButtons(bool isEnabled)
	{
		btBrowse.Enabled = isEnabled;
		SimpleButtonEx simpleButtonEx = btDelete;
		bool enabled = (btDisplay.Enabled = isEnabled && gv_File.GetFocusedRow() != null);
		simpleButtonEx.Enabled = enabled;
	}

	public async Task LoadAttachmentsAsync(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IssueForProduction");
			DataTable dataTable = await hp.GetAsync("LoadAttachFiles", new { absEntry });
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			dataTable.AcceptChanges();
			dataTable.AcceptChanges();
			grd_File.DataSource = dataTable;
			grd_File.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadAttachments(int absEntry)
	{
		gv_File.ShowLoadingPanel();
		try
		{
			DataTable dataTable;
			if (absEntry > 0)
			{
				using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IssueForProduction");
				dataTable = await hp.GetAsync("LoadAttachFiles", new { absEntry });
				if (dataTable == null || dataTable.Columns.Count == 0)
				{
					dataTable = WinFuntions.GetTableStructure("ATC1");
				}
			}
			else
			{
				dataTable = OpportunityTableHelper.Attachment.Clone();
			}
			grd_File.DataSource = dataTable;
			grd_File.RefreshDataSource();
			if (dataTable.Rows.Count == 0)
			{
				if (btDisplay.Enabled)
				{
					btDisplay.Enabled = false;
				}
				if (btDelete.Enabled)
				{
					btDelete.Enabled = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_File.HideLoadingPanel();
		}
	}

	public async void LoadAttachmentsFile(int absEntry)
	{
		try
		{
			DataTable dataTable = await _httpClient.TrackingIssue.GetFileByOjectType("1470000130", Function.ToString(absEntry));
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
			}
			dataTable.AcceptChanges();
			grd_File.DataSource = dataTable;
			grd_File.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool ValidAttachmentEntry(int atcEntry)
	{
		try
		{
			DataTable dataTable = new CommonsClient().ValidAttachmentEntry(atcEntry);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	public List<AttachmentFile_Impl> UploadFile(string Fromentry = "", int AtcEntry = 0)
	{
		List<AttachmentFile_Impl> result = new List<AttachmentFile_Impl>();
		try
		{
			new Dictionary<string, string>();
			if (!(grd_File.DataSource is DataTable dataTable))
			{
				return result;
			}
			bool flag = false;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<AttachmentFile_Impl> list = dataTable.ToAttachmentFile();
				if (list != null)
				{
					foreach (AttachmentFile_Impl item in list)
					{
						if (Function.ToString(item.Copied).Equals("Y"))
						{
							continue;
						}
						if (Function.ToString(item.Override).Equals("Y"))
						{
							item.Copied = "Y";
							continue;
						}
						string path = (string.IsNullOrWhiteSpace(item.srcPath) ? "" : item.srcPath);
						string sName = (string.IsNullOrWhiteSpace(item.FileName) ? "" : item.FileName);
						string text = (string.IsNullOrWhiteSpace(item.FileExt) ? "" : item.FileExt);
						string text2 = Function.ToString(item.trgtPath);
						string text3 = Path.Combine(path, sName) + "." + text;
						if (string.IsNullOrEmpty(text3) || !File.Exists(text3) || (!string.IsNullOrEmpty(Fromentry) && ValidAttachmentEntry(AtcEntry)))
						{
							continue;
						}
						string fullDestPath = Path.Combine(text2, sName) + "." + text;
						if (!text3.Equals(fullDestPath))
						{
							string retUpload = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, text3, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, fullDestPath);
							if (retUpload != fullDestPath)
							{
								if (!retUpload.StartsWith("1000002 -"))
								{
									WinCommons._CloseProcessing();
									WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", retUpload);
									return result;
								}
								if (flag)
								{
									if (AutoRename(item, text3, ref sName, text, text2, ref fullDestPath, ref retUpload))
									{
										continue;
									}
									return result;
								}
								if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRenameDestFileName, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, fullDestPath) != DialogResult.Yes)
								{
									WinCommons._CloseProcessing();
									WinCommons.Error(Enums.MessageCode.None, retUpload);
									return result;
								}
								flag = true;
								if (!AutoRename(item, text3, ref sName, text, text2, ref fullDestPath, ref retUpload))
								{
									return result;
								}
							}
							else
							{
								item.Copied = "Y";
							}
						}
						else
						{
							item.Override = "Y";
							item.Copied = "Y";
						}
					}
					result = list;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return result;
	}

	public bool GetAttachment(out List<AttachmentFile_Impl> list, string Fromentry = "", int AtcEntry = 0)
	{
		try
		{
			list = UploadFile(Fromentry, AtcEntry);
			if (list.Count == 0 && grd_File.DataSourceTable.IsNotNull())
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		list = new List<AttachmentFile_Impl>();
		return false;
	}

	private string GetObjectPath()
	{
		if (FindForm() is XtraFormEx xtraFormEx)
		{
			if (xtraFormEx.FormObjectType != ApzObjectTypes.oNone)
			{
				formObjectType = xtraFormEx.FormObjectType;
			}
			else if (!string.IsNullOrEmpty(xtraFormEx.FormId))
			{
				foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
				{
					if (Function.ToString(value) == xtraFormEx.FormId)
					{
						formObjectType = value;
						break;
					}
				}
			}
		}
		string result = "";
		if (formObjectType != ApzObjectTypes.oNone)
		{
			int key = (int)formObjectType;
			if (CopyFileSharingCfg.ObjectPath.ContainsKey(key))
			{
				result = CopyFileSharingCfg.ObjectPath[key];
			}
		}
		return result;
	}

	public DataTable UploadFileNoSAP(string Fromentry = "", int AtcEntry = 0)
	{
		try
		{
			DataTable dataTable = grd_File.DataSource as DataTable;
			if (dataTable == null)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
				grd_File.DataSource = dataTable.Copy();
			}
			DataTable dataTable2 = new DataTable();
			if (dataTable != null)
			{
				dataTable2 = dataTable.Copy();
				bool flag = false;
				foreach (DataRow row in dataTable2.Rows)
				{
					if (Function.ToString(row["Copied"]).Equals("Y"))
					{
						continue;
					}
					if (Function.ToString(row["Override"]).Equals("Y"))
					{
						row["Copied"] = "Y";
						continue;
					}
					string path = (string.IsNullOrWhiteSpace(Function.ToString(row["srcPath"])) ? "" : Function.ToString(row["srcPath"]));
					string sName = (string.IsNullOrWhiteSpace(Function.ToString(row["FileName"])) ? "" : Function.ToString(row["FileName"]));
					string text = (string.IsNullOrWhiteSpace(Function.ToString(row["FileExt"])) ? "" : Function.ToString(row["FileExt"]));
					string text2 = Function.ToString(row["trgtPath"]);
					string text3 = Path.Combine(path, sName) + "." + text;
					if (string.IsNullOrEmpty(text3) || !File.Exists(text3) || (!string.IsNullOrEmpty(Fromentry) && ValidAttachmentEntry(AtcEntry)))
					{
						continue;
					}
					string fullDestPath = Path.Combine(text2, sName) + "." + text;
					if (!text3.Equals(fullDestPath))
					{
						string retUpload = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, text3, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, fullDestPath);
						if (retUpload != fullDestPath)
						{
							if (!retUpload.StartsWith("1000002 -"))
							{
								WinCommons._CloseProcessing();
								WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", retUpload);
								return new DataTable();
							}
							if (flag)
							{
								if (AutoRename(row, text3, ref sName, text, text2, ref fullDestPath, ref retUpload))
								{
									continue;
								}
								return new DataTable();
							}
							if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRenameDestFileName, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, fullDestPath) != DialogResult.Yes)
							{
								WinCommons._CloseProcessing();
								WinCommons.Error(Enums.MessageCode.None, retUpload);
								return new DataTable();
							}
							flag = true;
							if (!AutoRename(row, text3, ref sName, text, text2, ref fullDestPath, ref retUpload))
							{
								return new DataTable();
							}
						}
						else
						{
							row["Copied"] = "Y";
						}
					}
					else
					{
						row["Override"] = "Y";
						row["Copied"] = "Y";
					}
				}
				dataTable2.AcceptChanges();
				grd_File.DataSource = dataTable2.Copy();
			}
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public bool GetAttachmentNoSAP(out DataTable dt, string Fromentry = "", int AtcEntry = 0)
	{
		try
		{
			dt = UploadFileNoSAP(Fromentry, AtcEntry);
			if ((dt == null || dt.Rows.Count < 0) && grd_File.DatasourceTable() != null)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		dt = new DataTable();
		return false;
	}

	private bool AutoRename(DataRow r, string sPath, ref string sName, string sExt, string upLoadPath, ref string fullDestPath, ref string retUpload)
	{
		int num = 1;
		string text = sName;
		while (File.Exists(fullDestPath))
		{
			text = $"{sName}({num++})";
			fullDestPath = Path.Combine(upLoadPath, text) + "." + sExt;
		}
		r["FileName"] = text;
		r.Table.AcceptChanges();
		retUpload = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, sPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, fullDestPath);
		if (retUpload != fullDestPath)
		{
			WinCommons._CloseProcessing();
			if (retUpload.StartsWith("1000002 -"))
			{
				WinCommons.Error(Enums.MessageCode.None, retUpload);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", retUpload);
			}
			return false;
		}
		r["Copied"] = "Y";
		return true;
	}

	private bool AutoRename(AttachmentFile_Impl item, string sPath, ref string sName, string sExt, string upLoadPath, ref string fullDestPath, ref string retUpload)
	{
		int num = 1;
		string text = sName;
		while (File.Exists(fullDestPath))
		{
			text = $"{sName}({num++})";
			fullDestPath = Path.Combine(upLoadPath, text) + "." + sExt;
		}
		item.FileName = text;
		retUpload = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, sPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, fullDestPath);
		if (retUpload != fullDestPath)
		{
			WinCommons._CloseProcessing();
			if (retUpload.StartsWith("1000002 -"))
			{
				WinCommons.Error(Enums.MessageCode.None, retUpload);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.UploadAttachmentFailed, "", retUpload);
			}
			return false;
		}
		item.Copied = "Y";
		return true;
	}

	public ucAttachment()
	{
		InitializeComponent();
		WinFuntions.LoadUdfColumns(gv_File, "ATC1");
		btBrowse.Click += HandleButtonBrowseClick;
		btDelete.Click += HandleButtonDeleteClick;
		btDisplay.Click += HandleButtonDisplayClick;
		removeToolStripMenuItem.Click += HandleButtonDeleteClick;
	}

	protected void HandleButtonBrowseClick(object sender, EventArgs e)
	{
		OnButtonBrowse_Click(EventArgs.Empty);
	}

	protected void OnButtonBrowse_Click(EventArgs e)
	{
		ButtonBrowse_Click?.Invoke(this, e);
	}

	protected void HandleButtonDeleteClick(object sender, EventArgs e)
	{
		OnButtonDelete_Click(EventArgs.Empty);
	}

	protected void OnButtonDelete_Click(EventArgs e)
	{
		ButtonDelete_Click?.Invoke(this, e);
	}

	protected void HandleButtonDisplayClick(object sender, EventArgs e)
	{
		OnButtonDisplay_Click(EventArgs.Empty);
	}

	protected void OnButtonDisplay_Click(EventArgs e)
	{
		ButtonDisplay_Click?.Invoke(this, e);
	}

	private void btDelete_Click(object sender, EventArgs e)
	{
		try
		{
			DeleteRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteRow()
	{
		if (BeforeDelete != null)
		{
			BeforeDelete(gv_File.FocusedRowHandle);
		}
		gv_File.BeginUpdate();
		gv_File.DeleteSelectedRows();
		gv_File.EndUpdate();
		(grd_File.DataSource as DataTable).AcceptChanges();
		grd_File.RefreshDataSource();
		if (DeletedFile != null)
		{
			DeletedFile();
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DeleteRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btDisplay_Click(object sender, EventArgs e)
	{
		try
		{
			DisplayAttachment();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DisplayAttachment()
	{
		try
		{
			WinCommons._OpenProcessing();
			if (FormId == "oTrackingIssue")
			{
				if (string.Equals(Function.ToString(gv_File.GetFocusedRowCellValue("IsDMS")), "Y"))
				{
					Process.Start(Function.ToString(gv_File.GetFocusedRowCellValue("WebUrl")));
				}
				else
				{
					GeneDisplayAttachment();
				}
			}
			else
			{
				GeneDisplayAttachment();
			}
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

	private void GeneDisplayAttachment()
	{
		try
		{
			string path = Function.ToString(gv_File.GetFocusedRowCellValue("FileName"));
			string text = Function.ToString(gv_File.GetFocusedRowCellValue("FileExt"));
			string text2 = "";
			text2 = ((!Function.ToString(gv_File.GetFocusedRowCellValue("Copied")).Equals("Y")) ? (Path.Combine(Function.ToString(gv_File.GetFocusedRowCellValue("srcPath")), path) + "." + text) : (Path.Combine(Function.ToString(gv_File.GetFocusedRowCellValue("trgtPath")), path) + "." + text));
			if (File.Exists(text2))
			{
				Process.Start(text2);
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
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void btBrowse_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog();
			string objectPath = GetObjectPath();
			string text = ((!string.IsNullOrEmpty(objectPath)) ? objectPath : CopyFileSharingCfg.AttachPath);
			if (!Directory.Exists(text))
			{
				WinCommons.Error(Enums.MessageCode.AttachmentPathNotAccess, "", text);
				return;
			}
			openFileDialog.InitialDirectory = text;
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			string[] fileNames = openFileDialog.FileNames;
			if (fileNames.Length == 0)
			{
				return;
			}
			DataTable dataTable = new DataTable();
			if (grd_File.DataSource != null)
			{
				dataTable = grd_File.DataSource as DataTable;
			}
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ATC1");
				dataTable.AcceptChanges();
			}
			List<DataRow> list = new List<DataRow>();
			if (dataTable != null)
			{
				for (int i = 0; i < fileNames.Length; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["AbsEntry"] = 0;
					dataRow["Date"] = DateTime.Now.Date;
					dataRow["srcPath"] = Path.GetDirectoryName(fileNames[i]);
					string text2 = Path.GetExtension(fileNames[i]);
					if (text2.Length > 1)
					{
						text2 = text2.Remove(0, 1);
					}
					dataRow["FileExt"] = text2;
					string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileNames[i]);
					dataRow["FileName"] = fileNameWithoutExtension;
					string text3 = ((!string.IsNullOrEmpty(objectPath)) ? objectPath : CopyFileShareHelper.GetUpLoadPath(text2));
					if (!CopyFileSharingCfg.DisableSplitFolder)
					{
						DateTime now = DateTime.Now;
						if (new DirectoryInfo(text3).Exists)
						{
							text3 = Path.Combine(text3, now.ToString("dd") + "-" + now.ToString("MM") + "-" + now.ToString("yyyy"));
							DirectoryInfo directoryInfo = new DirectoryInfo(text3);
							if (!directoryInfo.Exists)
							{
								directoryInfo.Create();
							}
						}
					}
					dataRow["trgtPath"] = text3;
					dataRow["Copied"] = "N";
					dataRow["Override"] = "N";
					string fullPath = Path.GetFullPath(fileNames[i]);
					foreach (DataRow item in dataTable.AsEnumerable())
					{
						string path = Function.ToString(item["srcPath"]);
						string path2 = Function.ToString(item["FileName"]);
						string text4 = Function.ToString(item["FileExt"]);
						string value = Path.Combine(path, path2) + "." + text4;
						if (fullPath.Equals(value) && !string.IsNullOrEmpty(fullPath))
						{
							dataRow["Override"] = "Y";
							break;
						}
					}
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					list.Add(dataRow);
				}
				for (int j = 0; j < dataTable.Rows.Count; j++)
				{
					dataTable.Rows[j]["Line"] = j + 1;
				}
				grd_File.DataSource = dataTable;
				grd_File.RefreshDataSource();
				if (BrowsedFile != null)
				{
					BrowsedFile(list);
				}
			}
			else
			{
				grd_File.DataSource = dataTable;
				grd_File.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucAttachment_Load(object sender, EventArgs e)
	{
		gcSrcPath.Caption = Resources.Line_gvFile_gcTrgtPath;
		gcFileName.Caption = Resources.Line_gvFile_gcFileName;
		gcDate.Caption = Resources.Line_gvFile_gcDate;
		btBrowse.Text = Resources.ButtonBrowse;
		btDisplay.Text = Resources.ButtonDisplay;
		btDelete.Text = Resources.ButtonDelete;
	}

	private void grd_File_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Delete)
			{
				DeleteRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_File_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (sender is GridView gridView)
			{
				Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
				GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
				if (gridHitInfo.InDataRow)
				{
					gridView.FocusedRowHandle = gridHitInfo.RowHandle;
					DisplayAttachment();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_File_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (!gv_File.FocusedColumn.FieldName.StartsWith("U_") && gv_File.FocusedColumn.FieldName != "FreeText")
		{
			e.Cancel = true;
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		removeToolStripMenuItem.Enabled = gv_File.GetFocusedRow() != null;
	}

	private void gv_File_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (btDisplay.SettingReadonly)
		{
			btDisplay.SettingReadonly = false;
		}
		SimpleButtonEx simpleButtonEx = btDelete;
		bool enabled = (btDisplay.Enabled = gv_File.GetFocusedRow() != null);
		simpleButtonEx.Enabled = enabled;
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
		DevExpress.XtraGrid.GridLevelNode gridLevelNode = new DevExpress.XtraGrid.GridLevelNode();
		this.btDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btDisplay = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btBrowse = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_File = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_File = new Apzon.Controls.Grid.GridViewEx();
		this.gcTrgtPath = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFileName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSrcPath = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUsrID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCopied = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOverride = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSubPath = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFileExt = new Apzon.Controls.Grid.GridColumnEx();
		this.CheckExist = new Apzon.Controls.Grid.GridColumnEx();
		this.panel1 = new System.Windows.Forms.Panel();
		((System.ComponentModel.ISupportInitialize)this.grd_File).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_File).BeginInit();
		this.panel1.SuspendLayout();
		base.SuspendLayout();
		this.btDelete.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btDelete.EditControl = null;
		this.btDelete.EditValue = "Delete";
		this.btDelete.FieldName = null;
		this.btDelete.IsSearchControl = false;
		this.btDelete.Location = new System.Drawing.Point(6, 101);
		this.btDelete.Name = "btDelete";
		this.btDelete.ReadOnly = false;
		this.btDelete.SettingInVisible = false;
		this.btDelete.SettingReadonly = false;
		this.btDelete.Size = new System.Drawing.Size(75, 23);
		this.btDelete.TabIndex = 62;
		this.btDelete.Text = "Delete";
		this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
		this.btDisplay.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btDisplay.EditControl = null;
		this.btDisplay.EditValue = "Display";
		this.btDisplay.FieldName = null;
		this.btDisplay.IsSearchControl = false;
		this.btDisplay.Location = new System.Drawing.Point(6, 32);
		this.btDisplay.Name = "btDisplay";
		this.btDisplay.ReadOnly = false;
		this.btDisplay.SettingInVisible = false;
		this.btDisplay.SettingReadonly = false;
		this.btDisplay.Size = new System.Drawing.Size(75, 23);
		this.btDisplay.TabIndex = 61;
		this.btDisplay.Tag = "DisplayAttachment";
		this.btDisplay.Text = "Display";
		this.btDisplay.Click += new System.EventHandler(this.btDisplay_Click);
		this.btBrowse.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.btBrowse.EditControl = null;
		this.btBrowse.EditValue = "Browse";
		this.btBrowse.FieldName = null;
		this.btBrowse.IsSearchControl = false;
		this.btBrowse.Location = new System.Drawing.Point(6, 3);
		this.btBrowse.Name = "btBrowse";
		this.btBrowse.ReadOnly = false;
		this.btBrowse.SettingInVisible = false;
		this.btBrowse.SettingReadonly = false;
		this.btBrowse.Size = new System.Drawing.Size(75, 23);
		this.btBrowse.TabIndex = 60;
		this.btBrowse.Text = "Browse";
		this.btBrowse.Click += new System.EventHandler(this.btBrowse_Click);
		this.grd_File.AllowPasteExcel = false;
		this.grd_File.ClipboardData = null;
		this.grd_File.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_File.Dock = System.Windows.Forms.DockStyle.Fill;
		gridLevelNode.RelationName = "Level1";
		this.grd_File.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.grd_File.Location = new System.Drawing.Point(0, 0);
		this.grd_File.MainView = this.gv_File;
		this.grd_File.Name = "grd_File";
		this.grd_File.Size = new System.Drawing.Size(741, 127);
		this.grd_File.TabIndex = 63;
		this.grd_File.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_File });
		this.grd_File.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_File_KeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_File.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_File.ColumnKey = null;
		this.gv_File.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gcTrgtPath, this.gcFileName, this.gcSrcPath, this.gcDate, this.gcUsrID, this.gcCopied, this.gcOverride, this.gcSubPath, this.gcFileExt, this.CheckExist });
		this.gv_File.DisableFormSetting = false;
		this.gv_File.DisplayException = false;
		this.gv_File.EditColumn = null;
		this.gv_File.FieldCurrency = null;
		this.gv_File.FieldRate = null;
		this.gv_File.FormId = "";
		this.gv_File.GridControl = this.grd_File;
		this.gv_File.HasFormSetting = false;
		this.gv_File.IgnoreFilter = false;
		this.gv_File.IndexEditRow = -1;
		this.gv_File.IndicatorWidth = 35;
		this.gv_File.IsApzTable = false;
		this.gv_File.IsInsertNewRow = false;
		this.gv_File.IsLoadingData = false;
		this.gv_File.IsMappingContextAction = false;
		this.gv_File.ItemId = "ATC1";
		this.gv_File.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_File.LstColumnAllowBatch = null;
		this.gv_File.LstColumnNotAllowBatch = null;
		this.gv_File.LstColumnNotEditable = null;
		this.gv_File.LstColumNotSetting = new System.Collections.Generic.List<string> { "CheckExist" };
		this.gv_File.LstOldUDF = null;
		this.gv_File.Name = "gv_File";
		this.gv_File.OptionsView.ShowDetailButtons = false;
		this.gv_File.OptionsView.ShowGroupPanel = false;
		this.gv_File.ReadOnly = false;
		this.gv_File.SettingReadonly = false;
		this.gv_File.UseBatchMod = false;
		this.gv_File.ViewCaption = "Attachments";
		this.gv_File.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_File_ShowingEditor);
		this.gv_File.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_File_FocusedRowObjectChanged);
		this.gv_File.DoubleClick += new System.EventHandler(this.gv_File_DoubleClick);
		this.gcTrgtPath.Caption = "Target Path";
		this.gcTrgtPath.CurrencyDateField = null;
		this.gcTrgtPath.CurrencyField = null;
		this.gcTrgtPath.CurrencyFixed = null;
		this.gcTrgtPath.CurrencyFixedRate = 0.0;
		this.gcTrgtPath.CurrencyRateField = null;
		this.gcTrgtPath.FieldName = "trgtPath";
		this.gcTrgtPath.Name = "gcTrgtPath";
		this.gcTrgtPath.OptionsColumn.AllowEdit = false;
		this.gcTrgtPath.OptionsColumn.ReadOnly = true;
		this.gcTrgtPath.PrimaryKey = false;
		this.gcTrgtPath.UnitField = null;
		this.gcTrgtPath.Visible = true;
		this.gcTrgtPath.VisibleIndex = 0;
		this.gcTrgtPath.Width = 342;
		this.gcFileName.Caption = "File Name";
		this.gcFileName.CurrencyDateField = null;
		this.gcFileName.CurrencyField = null;
		this.gcFileName.CurrencyFixed = null;
		this.gcFileName.CurrencyFixedRate = 0.0;
		this.gcFileName.CurrencyRateField = null;
		this.gcFileName.FieldName = "FileName";
		this.gcFileName.Name = "gcFileName";
		this.gcFileName.PrimaryKey = false;
		this.gcFileName.UnitField = null;
		this.gcFileName.Visible = true;
		this.gcFileName.VisibleIndex = 1;
		this.gcFileName.Width = 210;
		this.gcSrcPath.Caption = "srcPath";
		this.gcSrcPath.CurrencyDateField = null;
		this.gcSrcPath.CurrencyField = null;
		this.gcSrcPath.CurrencyFixed = null;
		this.gcSrcPath.CurrencyFixedRate = 0.0;
		this.gcSrcPath.CurrencyRateField = null;
		this.gcSrcPath.FieldName = "srcPath";
		this.gcSrcPath.Name = "gcSrcPath";
		this.gcSrcPath.PrimaryKey = false;
		this.gcSrcPath.UnitField = null;
		this.gcDate.Caption = "Date";
		this.gcDate.CurrencyDateField = null;
		this.gcDate.CurrencyField = null;
		this.gcDate.CurrencyFixed = null;
		this.gcDate.CurrencyFixedRate = 0.0;
		this.gcDate.CurrencyRateField = null;
		this.gcDate.FieldName = "Date";
		this.gcDate.Name = "gcDate";
		this.gcDate.PrimaryKey = false;
		this.gcDate.UnitField = null;
		this.gcDate.Visible = true;
		this.gcDate.VisibleIndex = 2;
		this.gcDate.Width = 89;
		this.gcUsrID.Caption = "UsrID";
		this.gcUsrID.CurrencyDateField = null;
		this.gcUsrID.CurrencyField = null;
		this.gcUsrID.CurrencyFixed = null;
		this.gcUsrID.CurrencyFixedRate = 0.0;
		this.gcUsrID.CurrencyRateField = null;
		this.gcUsrID.FieldName = "UsrID";
		this.gcUsrID.Name = "gcUsrID";
		this.gcUsrID.PrimaryKey = false;
		this.gcUsrID.UnitField = null;
		this.gcCopied.Caption = "Copied";
		this.gcCopied.CurrencyDateField = null;
		this.gcCopied.CurrencyField = null;
		this.gcCopied.CurrencyFixed = null;
		this.gcCopied.CurrencyFixedRate = 0.0;
		this.gcCopied.CurrencyRateField = null;
		this.gcCopied.FieldName = "Copied";
		this.gcCopied.Name = "gcCopied";
		this.gcCopied.PrimaryKey = false;
		this.gcCopied.UnitField = null;
		this.gcOverride.Caption = "Override";
		this.gcOverride.CurrencyDateField = null;
		this.gcOverride.CurrencyField = null;
		this.gcOverride.CurrencyFixed = null;
		this.gcOverride.CurrencyFixedRate = 0.0;
		this.gcOverride.CurrencyRateField = null;
		this.gcOverride.FieldName = "Override";
		this.gcOverride.Name = "gcOverride";
		this.gcOverride.PrimaryKey = false;
		this.gcOverride.UnitField = null;
		this.gcSubPath.Caption = "subPath";
		this.gcSubPath.CurrencyDateField = null;
		this.gcSubPath.CurrencyField = null;
		this.gcSubPath.CurrencyFixed = null;
		this.gcSubPath.CurrencyFixedRate = 0.0;
		this.gcSubPath.CurrencyRateField = null;
		this.gcSubPath.FieldName = "subPath";
		this.gcSubPath.Name = "gcSubPath";
		this.gcSubPath.PrimaryKey = false;
		this.gcSubPath.UnitField = null;
		this.gcFileExt.Caption = "FileExt";
		this.gcFileExt.CurrencyDateField = null;
		this.gcFileExt.CurrencyField = null;
		this.gcFileExt.CurrencyFixed = null;
		this.gcFileExt.CurrencyFixedRate = 0.0;
		this.gcFileExt.CurrencyRateField = null;
		this.gcFileExt.FieldName = "FileExt";
		this.gcFileExt.Name = "gcFileExt";
		this.gcFileExt.PrimaryKey = false;
		this.gcFileExt.UnitField = null;
		this.CheckExist.Caption = "gridColumn1";
		this.CheckExist.CurrencyDateField = null;
		this.CheckExist.CurrencyField = null;
		this.CheckExist.CurrencyFixed = null;
		this.CheckExist.CurrencyFixedRate = 0.0;
		this.CheckExist.CurrencyRateField = null;
		this.CheckExist.FieldName = "CheckExist";
		this.CheckExist.Name = "CheckExist";
		this.CheckExist.PrimaryKey = false;
		this.CheckExist.UnitField = null;
		this.panel1.Controls.Add(this.btBrowse);
		this.panel1.Controls.Add(this.btDisplay);
		this.panel1.Controls.Add(this.btDelete);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
		this.panel1.Location = new System.Drawing.Point(741, 0);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(84, 127);
		this.panel1.TabIndex = 64;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.grd_File);
		base.Controls.Add(this.panel1);
		base.Name = "ucAttachment";
		base.Size = new System.Drawing.Size(825, 127);
		((System.ComponentModel.ISupportInitialize)this.grd_File).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_File).EndInit();
		this.panel1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
