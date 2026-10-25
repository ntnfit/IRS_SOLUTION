using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.UDF;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Commons.UDT;

public class frm_UDTManager : XtraFormEx
{
	private const string FormController = "UDT";

	private const string tableName = "OUTB";

	private int flag = -1;

	private int _maxTblNumInDb;

	private bool _isCancel;

	private bool _forceClose;

	private IContainer components;

	private GridControlEx grdDoc;

	private GridColumnEx colTableName;

	private RepositoryItemTextEdit itemTxtTableName;

	private GridColumnEx colDescription;

	private RepositoryItemTextEdit itemTxtDescription;

	private GridColumnEx colTypeObject;

	private RepositoryItemComboBox itemCombobox;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btCancel;

	private GridViewEx gvDoc;

	private GridColumnEx gridColumn11;

	private RepositoryItemLookUpEdit rep_TypeObj;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumn4;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem toolStripMenuItem1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_UDTManager()
	{
		InitializeComponent();
		FormId = "oUDTManager";
		gvDoc.ItemId = "OUTB";
		gvDoc.FormId = FormId;
		OnLoadAsync += frm_UDT_Load;
	}

	private void AddNew()
	{
		try
		{
			gvDoc.BeginInsertRow();
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				int num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["TblNum"]));
				DataRow dataRow = dataTable.NewRow();
				dataRow["TableName"] = "";
				dataRow["Descr"] = "";
				dataRow["TblNum"] = num + 1;
				dataRow["ObjectType"] = 0;
				dataRow["UsedInObj"] = "";
				dataRow["LogTable"] = "";
				dataRow["Archivable"] = 'N';
				dataRow["ArchivDate"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("TableName", typeof(string));
				dataTable2.Columns.Add("Descr", typeof(string));
				dataTable2.Columns.Add("TblNum", typeof(int));
				dataTable2.Columns.Add("ObjectType", typeof(string));
				dataTable2.Columns.Add("UsedInObj", typeof(string));
				dataTable2.Columns.Add("LogTable", typeof(string));
				dataTable2.Columns.Add("Archivable", typeof(string));
				dataTable2.Columns.Add("ArchivDate", typeof(string));
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["TableName"] = "";
				dataRow2["Descr"] = "";
				dataRow2["TblNum"] = 1;
				dataRow2["ObjectType"] = 0;
				dataRow2["UsedInObj"] = "";
				dataRow2["LogTable"] = "";
				dataRow2["Archivable"] = 'N';
				dataRow2["ArchivDate"] = "";
				dataTable2.Rows.Add(dataRow2);
				dataTable2.AcceptChanges();
				dataTable2.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable2;
				grdDoc.RefreshDataSource();
				gvDoc.EndInsertRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gvDoc.EndInsertRow();
		}
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				DataTable dataTable = await helper.GetAsync("LoadDoc", new
				{
					tableName = "OUTB"
				});
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				if (dataTable.IsNotNull())
				{
					_maxTblNumInDb = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["TblNum"]));
				}
				else
				{
					_maxTblNumInDb = 0;
				}
				flag = dataTable.Rows.Count - 1;
			}
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["TableName"])) && string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["Descr"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				if ((from dataRow in dataTable2.AsEnumerable()
					group dataRow by Function.ToString(dataRow["TableName"])).Any((IGrouping<string, DataRow> source2) => source2.Skip(1).Any()))
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyInFollowingTableOutb, "");
					return "Error";
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["TableName"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.EnterValidTableName, "");
					return "Error";
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["TableName"]))
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable().Copy();
				}
				string currentCode = Function.ToString(gvDoc.GetFocusedRowCellValue("TableName"));
				using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDT");
				string x = await helper.PostAsync("UpdateUdt", dataTable);
				if (string.IsNullOrEmpty(x))
				{
					await LoadData();
					WinCommons.Success(Enums.MessageCode.Success, "");
					int num = gvDoc.LocateByValue("TableName", currentCode);
					if (num != int.MinValue)
					{
						gvDoc.FocusedRowHandle = num;
						gvDoc.MakeRowVisible(num);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, x);
				}
				return x;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return string.Empty;
	}

	private void LoadControl()
	{
		try
		{
			List<Common.Table_ID> dataSource = CommonList.UdtObjectTypes.Select((KeyValuePair<int, string> item) => new Common.Table_ID
			{
				ID = item.Key.ToString(),
				Name = item.Value
			}).ToList();
			rep_TypeObj.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_UDT_Load(object sender, EventArgs e)
	{
		LoadControl();
		await LoadData();
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					btn_Save.Enabled = false;
					btCancel.Enabled = false;
					await SaveData();
					btn_Save.Enabled = true;
					btCancel.Enabled = true;
					WinCommons._CloseProcessing();
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void gvDoc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("TblNum")) > _maxTblNumInDb)
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ObjectType"));
			string table = Function.ToString(gvDoc.GetFocusedRowCellValue("TableName"));
			if (num == 0 || num == 5)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["TableName"]) == table);
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					int rowHandle = gvDoc.GetRowHandle(dataSourceIndex);
					gvDoc.DeleteRow(rowHandle);
					dataTable.AcceptChanges();
					grdDoc.RefreshDataSource();
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.TableIsInUseInOneOfUserObject, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_UDTManager_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveData()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvDoc.FocusedColumn == colTableName)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "TblNum")) <= _maxTblNumInDb)
				{
					gvDoc.FocusedColumn.OptionsColumn.ReadOnly = true;
				}
				else
				{
					gvDoc.FocusedColumn.OptionsColumn.ReadOnly = false;
				}
			}
			if (gvDoc.FocusedColumn == gridColumn4)
			{
				if (Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Archivable")) != "Y")
				{
					gvDoc.FocusedColumn.OptionsColumn.ReadOnly = true;
				}
				else
				{
					gvDoc.FocusedColumn.OptionsColumn.ReadOnly = false;
				}
			}
			if (gvDoc.FocusedColumn == gridColumn3)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "ObjectType")) == 0 || Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "ObjectType")) == 5)
				{
					gvDoc.FocusedColumn.OptionsColumn.ReadOnly = false;
				}
				else
				{
					gvDoc.FocusedColumn.OptionsColumn.ReadOnly = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column == colTableName)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "TblNum")) <= _maxTblNumInDb)
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
			if (e.Column == gridColumn4)
			{
				if (Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "Archivable")) != "Y")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
			if (e.Column == gridColumn3)
			{
				if (Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "ObjectType")) == 0 || Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "ObjectType")) == 5)
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
				else
				{
					e.Appearance.BackColor = Color.LightGray;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_TypeObj_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("TblNum")) <= _maxTblNumInDb)
			{
				e.Cancel = true;
				WinCommons.Error(Enums.MessageCode.FieldCannotBeUpdate, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			frm_UserDefineFieldsManagement frm_UserDefineFieldsManagement2 = new frm_UserDefineFieldsManagement();
			frm_UserDefineFieldsManagement2.MdiParent = MdiParent;
			frm_UserDefineFieldsManagement2.Show();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.UDT.frm_UDTManager));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.colTableName = new Apzon.Controls.Grid.GridColumnEx();
		this.itemTxtTableName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.colDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.itemTxtDescription = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.colTypeObject = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_TypeObj = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.itemCombobox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.itemTxtTableName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.itemTxtDescription).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TypeObj).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.itemCombobox).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(-3, -1);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.itemCombobox, this.itemTxtTableName, this.itemTxtDescription, this.repositoryItemCheckEdit1, this.rep_TypeObj });
		this.grdDoc.Size = new System.Drawing.Size(782, 337);
		this.grdDoc.TabIndex = 2;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.deleteToolStripMenuItem, this.toolStripSeparator1, this.toolStripMenuItem1 });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(259, 54);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
		this.deleteToolStripMenuItem.Text = "Delete Row";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(255, 6);
		this.toolStripMenuItem1.Name = "toolStripMenuItem1";
		this.toolStripMenuItem1.Size = new System.Drawing.Size(258, 22);
		this.toolStripMenuItem1.Text = "User-Defined Fields - Management";
		this.toolStripMenuItem1.Click += new System.EventHandler(this.toolStripMenuItem1_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = "TableName";
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.colTableName, this.colDescription, this.gridColumn11, this.colTypeObject, this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oUDTManager";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = true;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "ODOC";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEdit);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.colTableName.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.colTableName.AppearanceCell.Options.UseBackColor = true;
		this.colTableName.Caption = "Table name";
		this.colTableName.ColumnEdit = this.itemTxtTableName;
		this.colTableName.CurrencyDateField = null;
		this.colTableName.CurrencyField = null;
		this.colTableName.CurrencyFixed = null;
		this.colTableName.CurrencyFixedRate = 0.0;
		this.colTableName.CurrencyRateField = null;
		this.colTableName.FieldName = "TableName";
		this.colTableName.Name = "colTableName";
		this.colTableName.PrimaryKey = false;
		this.colTableName.UnitField = null;
		this.colTableName.Visible = true;
		this.colTableName.VisibleIndex = 0;
		this.colTableName.Width = 161;
		this.itemTxtTableName.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.itemTxtTableName.Appearance.Options.UseBackColor = true;
		this.itemTxtTableName.AutoHeight = false;
		this.itemTxtTableName.Name = "itemTxtTableName";
		this.colDescription.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.colDescription.AppearanceCell.Options.UseBackColor = true;
		this.colDescription.Caption = "Description";
		this.colDescription.ColumnEdit = this.itemTxtDescription;
		this.colDescription.CurrencyDateField = null;
		this.colDescription.CurrencyField = null;
		this.colDescription.CurrencyFixed = null;
		this.colDescription.CurrencyFixedRate = 0.0;
		this.colDescription.CurrencyRateField = null;
		this.colDescription.FieldName = "Descr";
		this.colDescription.Name = "colDescription";
		this.colDescription.OptionsColumn.AllowMove = false;
		this.colDescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colDescription.PrimaryKey = false;
		this.colDescription.UnitField = null;
		this.colDescription.Visible = true;
		this.colDescription.VisibleIndex = 1;
		this.colDescription.Width = 282;
		this.itemTxtDescription.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.itemTxtDescription.Appearance.Options.UseBackColor = true;
		this.itemTxtDescription.AutoHeight = false;
		this.itemTxtDescription.Name = "itemTxtDescription";
		this.gridColumn11.Caption = "TblNum";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "TblNum";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.colTypeObject.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.colTypeObject.AppearanceCell.Options.UseBackColor = true;
		this.colTypeObject.Caption = "Type object";
		this.colTypeObject.ColumnEdit = this.rep_TypeObj;
		this.colTypeObject.CurrencyDateField = null;
		this.colTypeObject.CurrencyField = null;
		this.colTypeObject.CurrencyFixed = null;
		this.colTypeObject.CurrencyFixedRate = 0.0;
		this.colTypeObject.CurrencyRateField = null;
		this.colTypeObject.FieldName = "ObjectType";
		this.colTypeObject.Name = "colTypeObject";
		this.colTypeObject.OptionsColumn.AllowMove = false;
		this.colTypeObject.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colTypeObject.PrimaryKey = false;
		this.colTypeObject.UnitField = null;
		this.colTypeObject.Visible = true;
		this.colTypeObject.VisibleIndex = 2;
		this.colTypeObject.Width = 178;
		this.rep_TypeObj.AutoHeight = false;
		this.rep_TypeObj.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_TypeObj.DisplayMember = "Name";
		this.rep_TypeObj.Name = "rep_TypeObj";
		this.rep_TypeObj.NullText = "";
		this.rep_TypeObj.ValueMember = "ID";
		this.rep_TypeObj.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_TypeObj_EditValueChanging);
		this.gridColumn1.Caption = "UsedInObj";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "UsedInObj";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "LogTable";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "LogTable";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "Archivable";
		this.gridColumn3.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Archivable";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 3;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.gridColumn4.Caption = "Archive Date";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "ArchivDate";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 4;
		this.itemCombobox.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.itemCombobox.Appearance.Options.UseBackColor = true;
		this.itemCombobox.AutoHeight = false;
		this.itemCombobox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.itemCombobox.Name = "itemCombobox";
		this.itemCombobox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 340);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 3;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(107, 340);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 4;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(777, 365);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grdDoc);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.FormId = "oUDTManager";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_UDTManager.IconOptions.Icon");
		base.Name = "frm_UDTManager";
		this.Text = "User Define Table Manager";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UDTManager_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.itemTxtTableName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.itemTxtDescription).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_TypeObj).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.itemCombobox).EndInit();
		base.ResumeLayout(false);
	}
}
