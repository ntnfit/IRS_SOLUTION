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
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.TableLayout;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Tile;
using DevExpress.XtraTab;
using DevExpress.XtraTab.ViewInfo;
using ExcelDataReader;
using Microsoft.VisualBasic.FileIO;

namespace Apzon.Forms.Commons;

public class frm_ImportFiles : XtraFormEx
{
	private List<Tuple<Bitmap, string, string, string>> lstDataSource = new List<Tuple<Bitmap, string, string, string>>();

	private bool isOpenFolder;

	private IContainer components;

	private SimpleButtonEx simpleButtonEx1;

	private ButtonEditEx txt_browserFile;

	private LookUpEditEx txt_type;

	private XtraTabControlEx tabControl;

	private XtraTabPageEx xtraTabPageEx1;

	private GridControl grd_AllFile;

	private RepositoryItemPictureEdit txt_picture;

	private RepositoryItemTextEdit txt_fileName;

	private TileView gv_AllFile;

	private TileViewColumn col_Image;

	private TileViewColumn col_FileName;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem openToolStripMenuItem;

	private TileViewColumn col_Extension;

	private TileViewColumn col_FullName;

	private ToolStripMenuItem openWithoutHeaderToolStripMenuItem;

	private ToolStripMenuItem jumpToAllFilesTabToolStripMenuItem;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	public frm_ImportFiles()
	{
		OnLoadAsync += frm_ImportFiles_Load;
		InitializeComponent();
	}

	private async Task frm_ImportFiles_Load(object sender, EventArgs e)
	{
		try
		{
			string[] dataSource = new string[2] { "Files", "Folder" };
			txt_type.Properties.DataSource = dataSource;
			txt_type.EditValue = "Files";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable ReadTextFileToDataTable(string path, bool hasHeader = true, string delimiter = ",")
	{
		try
		{
			List<string[]> list = new List<string[]>();
			using (TextFieldParser textFieldParser = new TextFieldParser(path))
			{
				textFieldParser.Delimiters = new string[1] { delimiter };
				while (true)
				{
					string[] array = textFieldParser.ReadFields();
					if (array != null)
					{
						list.Add(array);
						continue;
					}
					break;
				}
			}
			int num = 1;
			DataTable dataTable = new DataTable();
			dataTable.TableName = Path.GetFileName(path);
			if (list.Count <= 0)
			{
				return new DataTable();
			}
			if (hasHeader)
			{
				num = 1;
				for (int i = 0; i < list[0].Length; i++)
				{
					dataTable.Columns.Add(list[0][i], typeof(string));
				}
			}
			else
			{
				num = 0;
				for (int j = 0; j < list[0].Length; j++)
				{
					dataTable.Columns.Add(new DataColumn());
				}
			}
			dataTable.AcceptChanges();
			for (int k = num; k < list.Count; k++)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int l = 0; l < dataTable.Columns.Count; l++)
				{
					dataRow[l] = list[k][l];
				}
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private DataSet ReadExcelToDataTable(string path, string extension, bool hasHeader = true)
	{
		try
		{
			DataSet result = new DataSet();
			FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read);
			ExcelDataSetConfiguration configuration = new ExcelDataSetConfiguration
			{
				ConfigureDataTable = (IExcelDataReader _) => new ExcelDataTableConfiguration
				{
					UseHeaderRow = hasHeader
				}
			};
			switch (extension)
			{
			case ".xlsx":
			case ".xlsm":
				result = ExcelReaderFactory.CreateOpenXmlReader(fileStream).AsDataSet(configuration);
				break;
			case ".xls":
				result = ExcelReaderFactory.CreateBinaryReader(fileStream).AsDataSet(configuration);
				break;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
		}
	}

	private void AddTabForResult(DataSet result, string shortFileName, bool selectedCurrentPage = true)
	{
		try
		{
			XtraTabPageEx xtraTabPageEx = new XtraTabPageEx();
			xtraTabPageEx.Text = shortFileName;
			tabControl.TabPages.Add(xtraTabPageEx);
			if (result.Tables.Count > 1)
			{
				XtraTabControlEx xtraTabControlEx = new XtraTabControlEx();
				xtraTabControlEx.Dock = DockStyle.Fill;
				xtraTabPageEx.Controls.Add(xtraTabControlEx);
				for (int i = 0; i < result.Tables.Count; i++)
				{
					XtraTabPageEx xtraTabPageEx2 = new XtraTabPageEx();
					xtraTabPageEx2.Text = result.Tables[i].TableName;
					GridControlEx gridControlEx = new GridControlEx();
					gridControlEx.Dock = DockStyle.Fill;
					gridControlEx.DataSource = result.Tables[i];
					xtraTabPageEx2.Controls.Add(gridControlEx);
					xtraTabControlEx.TabPages.Add(xtraTabPageEx2);
				}
			}
			else if (result.Tables.Count == 1)
			{
				GridControlEx gridControlEx2 = new GridControlEx();
				gridControlEx2.Dock = DockStyle.Fill;
				xtraTabPageEx.Controls.Add(gridControlEx2);
				gridControlEx2.DataSource = result.Tables[0];
			}
			if (selectedCurrentPage)
			{
				tabControl.SelectedTabPage = xtraTabPageEx;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OpenDataAllFiles()
	{
		try
		{
			DataSet dataSet = new DataSet();
			for (int i = 0; i < lstDataSource.Count; i++)
			{
				if (lstDataSource[i].Item4 == ".xlsx" || lstDataSource[i].Item4 == ".xlsm" || lstDataSource[i].Item4 == ".xls" || lstDataSource[i].Item4 == ".csv" || lstDataSource[i].Item4 == ".txt")
				{
					if (lstDataSource[i].Item4 == ".xlsx" || lstDataSource[i].Item4 == ".xlsm" || lstDataSource[i].Item4 == ".xls")
					{
						dataSet = new DataSet();
						dataSet = ReadExcelToDataTable(lstDataSource[i].Item2, lstDataSource[i].Item4);
					}
					else if (lstDataSource[i].Item4 == ".csv")
					{
						dataSet = new DataSet();
						DataTable table = ReadTextFileToDataTable(lstDataSource[i].Item2);
						dataSet.Tables.Add(table);
						dataSet.AcceptChanges();
					}
					else if (lstDataSource[i].Item4 == ".txt")
					{
						dataSet = new DataSet();
						DataTable table2 = ReadTextFileToDataTable(lstDataSource[i].Item2, hasHeader: true, "\t");
						dataSet.Tables.Add(table2);
						dataSet.AcceptChanges();
					}
					if (i == lstDataSource.Count - 1)
					{
						AddTabForResult(dataSet, lstDataSource[i].Item3);
					}
					else
					{
						AddTabForResult(dataSet, lstDataSource[i].Item3, selectedCurrentPage: false);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_browserFile_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (Function.ToString(txt_type.EditValue) == "Folder")
			{
				isOpenFolder = true;
				using FolderBrowserDialogEx folderBrowserDialogEx = new FolderBrowserDialogEx();
				if (folderBrowserDialogEx.ShowDialog() == DialogResult.OK)
				{
					lstDataSource.Clear();
					txt_browserFile.Text = folderBrowserDialogEx.SelectedPath;
					string[] files = Directory.GetFiles(folderBrowserDialogEx.SelectedPath);
					foreach (string text in files)
					{
						Tuple<Bitmap, string, string, string> item = Tuple.Create(Icon.ExtractAssociatedIcon(text).ToBitmap(), text, Path.GetFileName(text), Path.GetExtension(text));
						lstDataSource.Add(item);
					}
				}
				return;
			}
			isOpenFolder = false;
			using OpenFileDialog openFileDialog = new OpenFileDialog();
			openFileDialog.Multiselect = true;
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				lstDataSource.Clear();
				string text2 = "";
				for (int j = 0; j < openFileDialog.FileNames.Length; j++)
				{
					text2 = ((j == openFileDialog.FileNames.Length - 1) ? (text2 + openFileDialog.FileNames[j]) : (text2 + openFileDialog.FileNames[j] + "; "));
					lstDataSource.Add(Tuple.Create(Icon.ExtractAssociatedIcon(openFileDialog.FileNames[j]).ToBitmap(), openFileDialog.FileNames[j], Path.GetFileName(openFileDialog.FileNames[j]), Path.GetExtension(openFileDialog.FileNames[j])));
				}
				txt_browserFile.Text = text2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void simpleButtonEx1_Click(object sender, EventArgs e)
	{
		try
		{
			grd_AllFile.DataSource = lstDataSource;
			grd_AllFile.RefreshDataSource();
			if (!isOpenFolder)
			{
				WinCommons._OpenProcessing();
				OpenDataAllFiles();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tabControl_CloseButtonClick(object sender, EventArgs e)
	{
		try
		{
			XtraTabPageEx xtraTabPageEx = (e as ClosePageButtonEventArgs).Page as XtraTabPageEx;
			if (tabControl.TabPages[0] != xtraTabPageEx)
			{
				tabControl.TabPages.Remove(xtraTabPageEx);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OpenFileSelected(bool hasHeader = true)
	{
		try
		{
			string path = Function.ToString(gv_AllFile.GetFocusedRowCellValue(col_FullName));
			string shortFileName = Function.ToString(gv_AllFile.GetFocusedRowCellValue(col_FileName));
			string text = Function.ToString(gv_AllFile.GetFocusedRowCellValue(col_Extension));
			if (hasHeader)
			{
				switch (text)
				{
				case ".xlsx":
				case ".xlsm":
				case ".xls":
				{
					DataSet result = ReadExcelToDataTable(path, text);
					AddTabForResult(result, shortFileName);
					break;
				}
				case ".csv":
				{
					DataTable table2 = ReadTextFileToDataTable(path);
					DataSet dataSet2 = new DataSet();
					dataSet2.Tables.Add(table2);
					AddTabForResult(dataSet2, shortFileName);
					break;
				}
				case ".txt":
				{
					DataTable table = ReadTextFileToDataTable(path, hasHeader: true, "\t");
					DataSet dataSet = new DataSet();
					dataSet.Tables.Add(table);
					AddTabForResult(dataSet, shortFileName);
					break;
				}
				}
			}
			else
			{
				switch (text)
				{
				case ".xlsx":
				case ".xlsm":
				case ".xls":
				{
					DataSet result2 = ReadExcelToDataTable(path, text, hasHeader: false);
					AddTabForResult(result2, shortFileName);
					break;
				}
				case ".csv":
				{
					DataTable table4 = ReadTextFileToDataTable(path, hasHeader: false);
					DataSet dataSet4 = new DataSet();
					dataSet4.Tables.Add(table4);
					AddTabForResult(dataSet4, shortFileName);
					break;
				}
				case ".txt":
				{
					DataTable table3 = ReadTextFileToDataTable(path, hasHeader: false, "\t");
					DataSet dataSet3 = new DataSet();
					dataSet3.Tables.Add(table3);
					AddTabForResult(dataSet3, shortFileName);
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void openToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileSelected();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_AllFile.GetFocusedRowCellValue(col_Extension));
			if (text != ".xlsx" && text != ".xlsm" && text != ".xls" && text != ".txt" && text != ".csv")
			{
				openToolStripMenuItem.Enabled = false;
				openWithoutHeaderToolStripMenuItem.Enabled = false;
			}
			if (tabControl.SelectedTabPageIndex != 0)
			{
				openToolStripMenuItem.Enabled = false;
				openWithoutHeaderToolStripMenuItem.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void openWithoutHeaderToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileSelected(hasHeader: false);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void jumpToAllFilesTabToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			tabControl.SelectedTabPageIndex = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_AllFile_ItemDoubleClick(object sender, TileViewItemClickEventArgs e)
	{
		try
		{
			OpenFileSelected();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ImportFiles));
		DevExpress.XtraEditors.TableLayout.TableColumnDefinition obj = new DevExpress.XtraEditors.TableLayout.TableColumnDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition2 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableRowDefinition tableRowDefinition3 = new DevExpress.XtraEditors.TableLayout.TableRowDefinition();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraEditors.TableLayout.TableSpan tableSpan2 = new DevExpress.XtraEditors.TableLayout.TableSpan();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		DevExpress.XtraGrid.Views.Tile.TileViewItemElement tileViewItemElement2 = new DevExpress.XtraGrid.Views.Tile.TileViewItemElement();
		this.col_Image = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.txt_picture = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.col_FileName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.simpleButtonEx1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_browserFile = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_type = new Apzon.Controls.Editor.LookUpEditEx();
		this.tabControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openWithoutHeaderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.jumpToAllFilesTabToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.xtraTabPageEx1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_AllFile = new DevExpress.XtraGrid.GridControl();
		this.gv_AllFile = new DevExpress.XtraGrid.Views.Tile.TileView();
		this.col_Extension = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.col_FullName = new DevExpress.XtraGrid.Columns.TileViewColumn();
		this.txt_fileName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_picture).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_browserFile.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_type.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		this.tabControl.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		this.xtraTabPageEx1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_AllFile).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_AllFile).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_fileName).BeginInit();
		base.SuspendLayout();
		this.col_Image.Caption = "tileViewColumn1";
		this.col_Image.ColumnEdit = this.txt_picture;
		this.col_Image.FieldName = "Item1";
		this.col_Image.Name = "col_Image";
		this.col_Image.Visible = true;
		this.col_Image.VisibleIndex = 0;
		this.txt_picture.Name = "txt_picture";
		this.txt_picture.PictureStoreMode = DevExpress.XtraEditors.Controls.PictureStoreMode.Image;
		this.txt_picture.ZoomAccelerationFactor = 1.0;
		this.col_FileName.Caption = "tileViewColumn2";
		this.col_FileName.FieldName = "Item3";
		this.col_FileName.Name = "col_FileName";
		this.col_FileName.Visible = true;
		this.col_FileName.VisibleIndex = 1;
		this.simpleButtonEx1.FieldName = null;
		this.simpleButtonEx1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButtonEx1.ImageOptions.Image");
		this.simpleButtonEx1.IsSearchControl = false;
		this.simpleButtonEx1.Location = new System.Drawing.Point(321, 10);
		this.simpleButtonEx1.Name = "simpleButtonEx1";
		this.simpleButtonEx1.Size = new System.Drawing.Size(72, 23);
		this.simpleButtonEx1.TabIndex = 3;
		this.simpleButtonEx1.Text = "Import";
		this.simpleButtonEx1.Click += new System.EventHandler(this.simpleButtonEx1_Click);
		this.txt_browserFile.FieldName = null;
		this.txt_browserFile.IsSearchControl = false;
		this.txt_browserFile.Location = new System.Drawing.Point(95, 12);
		this.txt_browserFile.Name = "txt_browserFile";
		this.txt_browserFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txt_browserFile.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_browserFile_Properties_ButtonClick);
		this.txt_browserFile.Size = new System.Drawing.Size(220, 20);
		this.txt_browserFile.TabIndex = 5;
		this.txt_type.FieldName = null;
		this.txt_type.IsSearchControl = false;
		this.txt_type.Location = new System.Drawing.Point(12, 12);
		this.txt_type.Name = "txt_type";
		this.txt_type.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_type.Properties.NullText = "";
		this.txt_type.Size = new System.Drawing.Size(77, 20);
		this.txt_type.TabIndex = 6;
		this.tabControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControl.ClosePageButtonShowMode = DevExpress.XtraTab.ClosePageButtonShowMode.InActiveTabPageHeaderAndOnMouseHover;
		this.tabControl.ContextMenuStrip = this.contextMenuStrip1;
		this.tabControl.IsSearchControl = false;
		this.tabControl.Location = new System.Drawing.Point(2, 50);
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedTabPage = this.xtraTabPageEx1;
		this.tabControl.Size = new System.Drawing.Size(753, 389);
		this.tabControl.TabIndex = 7;
		this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPageEx1 });
		this.tabControl.CloseButtonClick += new System.EventHandler(this.tabControl_CloseButtonClick);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.openToolStripMenuItem, this.openWithoutHeaderToolStripMenuItem, this.jumpToAllFilesTabToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(187, 70);
		this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
		this.openToolStripMenuItem.Name = "openToolStripMenuItem";
		this.openToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
		this.openToolStripMenuItem.Text = "Open";
		this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
		this.openWithoutHeaderToolStripMenuItem.Name = "openWithoutHeaderToolStripMenuItem";
		this.openWithoutHeaderToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
		this.openWithoutHeaderToolStripMenuItem.Text = "Open without header";
		this.openWithoutHeaderToolStripMenuItem.Click += new System.EventHandler(this.openWithoutHeaderToolStripMenuItem_Click);
		this.jumpToAllFilesTabToolStripMenuItem.Name = "jumpToAllFilesTabToolStripMenuItem";
		this.jumpToAllFilesTabToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
		this.jumpToAllFilesTabToolStripMenuItem.Text = "Jump to All Files tab";
		this.jumpToAllFilesTabToolStripMenuItem.Click += new System.EventHandler(this.jumpToAllFilesTabToolStripMenuItem_Click);
		this.xtraTabPageEx1.Controls.Add(this.grd_AllFile);
		this.xtraTabPageEx1.FieldName = "";
		this.xtraTabPageEx1.IsSearchControl = false;
		this.xtraTabPageEx1.Name = "xtraTabPageEx1";
		this.xtraTabPageEx1.Size = new System.Drawing.Size(747, 361);
		this.xtraTabPageEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPageEx1.Text = "All Files";
		this.grd_AllFile.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_AllFile.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_AllFile.Location = new System.Drawing.Point(0, 0);
		this.grd_AllFile.MainView = this.gv_AllFile;
		this.grd_AllFile.Name = "grd_AllFile";
		this.grd_AllFile.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.txt_picture, this.txt_fileName });
		this.grd_AllFile.Size = new System.Drawing.Size(747, 361);
		this.grd_AllFile.TabIndex = 0;
		this.grd_AllFile.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_AllFile });
		this.gv_AllFile.Appearance.EmptySpace.BackColor = System.Drawing.Color.White;
		this.gv_AllFile.Appearance.EmptySpace.Options.UseBackColor = true;
		this.gv_AllFile.Appearance.ItemNormal.BorderColor = System.Drawing.Color.Transparent;
		this.gv_AllFile.Appearance.ItemNormal.Options.UseBorderColor = true;
		this.gv_AllFile.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.col_Image, this.col_FileName, this.col_Extension, this.col_FullName });
		this.gv_AllFile.GridControl = this.grd_AllFile;
		this.gv_AllFile.Name = "gv_AllFile";
		this.gv_AllFile.OptionsTiles.HorizontalContentAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.gv_AllFile.OptionsTiles.ItemSize = new System.Drawing.Size(134, 86);
		this.gv_AllFile.OptionsTiles.Orientation = System.Windows.Forms.Orientation.Vertical;
		this.gv_AllFile.OptionsTiles.RowCount = 0;
		this.gv_AllFile.TileColumns.Add(obj);
		tableRowDefinition.Length.Value = 24.0;
		tableRowDefinition2.Length.Value = 13.0;
		tableRowDefinition3.Length.Value = 15.0;
		this.gv_AllFile.TileRows.Add(tableRowDefinition);
		this.gv_AllFile.TileRows.Add(tableRowDefinition2);
		this.gv_AllFile.TileRows.Add(tableRowDefinition3);
		tableSpan.ColumnSpan = 3;
		tableSpan.RowSpan = 2;
		tableSpan2.ColumnSpan = 3;
		tableSpan2.RowIndex = 2;
		this.gv_AllFile.TileSpans.Add(tableSpan);
		this.gv_AllFile.TileSpans.Add(tableSpan2);
		tileViewItemElement.Appearance.Normal.BackColor = System.Drawing.Color.Transparent;
		tileViewItemElement.Appearance.Normal.Options.UseBackColor = true;
		tileViewItemElement.Column = this.col_Image;
		tileViewItemElement.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement.Text = "col_Image";
		tileViewItemElement.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.Column = this.col_FileName;
		tileViewItemElement2.ImageAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		tileViewItemElement2.ImageScaleMode = DevExpress.XtraEditors.TileItemImageScaleMode.ZoomInside;
		tileViewItemElement2.RowIndex = 2;
		tileViewItemElement2.Text = "col_FileName";
		tileViewItemElement2.TextAlignment = DevExpress.XtraEditors.TileItemContentAlignment.MiddleCenter;
		this.gv_AllFile.TileTemplate.Add(tileViewItemElement);
		this.gv_AllFile.TileTemplate.Add(tileViewItemElement2);
		this.gv_AllFile.ItemDoubleClick += new DevExpress.XtraGrid.Views.Tile.TileViewItemClickEventHandler(this.gv_AllFile_ItemDoubleClick);
		this.col_Extension.Caption = "Extension";
		this.col_Extension.FieldName = "Item4";
		this.col_Extension.Name = "col_Extension";
		this.col_Extension.Visible = true;
		this.col_Extension.VisibleIndex = 2;
		this.col_FullName.Caption = "Full Name";
		this.col_FullName.FieldName = "Item2";
		this.col_FullName.Name = "col_FullName";
		this.col_FullName.Visible = true;
		this.col_FullName.VisibleIndex = 3;
		this.txt_fileName.AutoHeight = false;
		this.txt_fileName.Name = "txt_fileName";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(584, 445);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 11;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(665, 445);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 12;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click_1);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(752, 480);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.tabControl);
		base.Controls.Add(this.txt_type);
		base.Controls.Add(this.txt_browserFile);
		base.Controls.Add(this.simpleButtonEx1);
		base.Name = "frm_ImportFiles";
		base.ShowIcon = false;
		this.Text = "Import Files";
		((System.ComponentModel.ISupportInitialize)this.txt_picture).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_browserFile.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_type.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		this.tabControl.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		this.xtraTabPageEx1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_AllFile).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_AllFile).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_fileName).EndInit();
		base.ResumeLayout(false);
	}
}
