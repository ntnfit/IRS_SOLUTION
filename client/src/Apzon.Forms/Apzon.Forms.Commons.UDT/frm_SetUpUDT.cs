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
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Commons.UDT;

public class frm_SetUpUDT : XtraFormEx
{
	public readonly string _tableName;

	private const string FormController = "UDT";

	private bool _isCancel;

	private bool _forceClose;

	private readonly bool _defineNew;

	private string _newCode;

	public Action<string> RefreshLookupEdit;

	private IContainer components;

	public SimpleButtonEx btnCancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private RepositoryItemButtonEdit rep_GLAcct;

	private RepositoryItemLookUpEdit rep_UOM;

	public SimpleButtonEx btn_Save;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumnEx1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_SetUpUDT(string tableName, bool defineNew = false)
	{
		string authCode = (tableName.StartsWith("UDT_") ? tableName : ("UDT_" + tableName));
		AuthsImpl authsImpl = GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl f) => f.menucode == authCode && f.authoriza != "N");
		if (authsImpl != null)
		{
			InitializeComponent();
			gvDoc.LstColumNotSetting = new List<string> { "IsNewRow" };
			_tableName = tableName;
			_defineNew = defineNew;
			if (defineNew)
			{
				Text = authsImpl.menuName;
			}
			FormId = "UDT";
			gvDoc.FormId = "UDT";
			gvDoc.ItemId = tableName;
			OnLoadAsync += frm_SetUpUDT_Load;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == authCode && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async Task frm_SetUpUDT_Load(object sender, EventArgs e)
	{
		Task task = WinFuntions.LoadUdfColumns(gvDoc, "@" + _tableName, new Dictionary<string, GridViewEx> { 
		{
			"@" + _tableName,
			gvDoc
		} });
		Task task2 = LoadData();
		await Task.WhenAll(task, task2);
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			gvDoc.ShowLoadingPanel();
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDT");
			DataTable dataTable = await helper.GetAsync("LoadDoc", new
			{
				tableName = _tableName
			});
			dataTable.SetDefaultAndMaxLen("@" + _tableName);
			dataTable.Columns.Add("IsNewRow", typeof(string));
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gvDoc.HideLoadingPanel();
		}
	}

	private async void AddNew()
	{
		try
		{
			gvDoc.BeginInsertRow();
			DataTable dt = grdDoc.DataSource as DataTable;
			DataTable dataTable = await GetDtAddNew();
			if (dt.IsNotNull())
			{
				if (dt.ExistsColumn("Code") && dt.Columns["Code"].DataType == typeof(int))
				{
					gridColumn1.OptionsColumn.ReadOnly = true;
					DataRow dataRow = dt.NewRow();
					for (int i = 0; i < dt.Columns.Count; i++)
					{
						string columnName = dt.Columns[i].ColumnName;
						if (columnName != "Code")
						{
							if (columnName == "IsNewRow")
							{
								dataRow[columnName] = "Y";
							}
							else
							{
								dataRow[columnName] = dataTable.Rows[0][columnName];
							}
						}
						else
						{
							dataRow[columnName] = DBNull.Value;
						}
					}
					dt.Rows.Add(dataRow);
					dt.AcceptChanges();
					dt.SetDefaultAndMaxLen("@" + _tableName);
					grdDoc.DataSource = dt;
					grdDoc.RefreshDataSource();
					gvDoc.FocusedRowHandle = gvDoc.GetRowHandle(dt.Rows.IndexOf(dataRow));
					gvDoc.FocusedColumn = gridColumn2;
					gvDoc.ShowEditor();
				}
				else
				{
					DataRow dataRow2 = dt.NewRow();
					for (int j = 0; j < dt.Columns.Count; j++)
					{
						string columnName2 = dt.Columns[j].ColumnName;
						dataRow2[columnName2] = dataTable.Rows[0][columnName2];
					}
					dataRow2["IsNewRow"] = "Y";
					dt.Rows.Add(dataRow2);
					dt.AcceptChanges();
					dt.SetDefaultAndMaxLen("@" + _tableName);
					grdDoc.DataSource = dt;
					grdDoc.RefreshDataSource();
					gvDoc.FocusedRowHandle = gvDoc.GetRowHandle(dt.Rows.IndexOf(dataRow2));
					gvDoc.FocusedColumn = gridColumn1;
					gvDoc.ShowEditor();
				}
			}
			else
			{
				dataTable.SetDefaultAndMaxLen("@" + _tableName);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				if (dataTable.ExistsColumn("Code") && dataTable.Columns["Code"].DataType == typeof(int))
				{
					gridColumn1.OptionsColumn.ReadOnly = true;
				}
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

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDT");
			DataTable dataTable = await helper.GetAsync("AddNEW", new
			{
				tableName = _tableName
			});
			if (dataTable.IsNotNull())
			{
				dataTable.Columns.Add("IsNewRow", typeof(string));
				dataTable.Rows[0]["IsNewRow"] = "Y";
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			if (dataTable.IsNotNull())
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["Code"])) && string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["Name"])))
				{
					dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
					dataTable.AcceptChanges();
				}
				if ((from dataRow in dataTable.AsEnumerable()
					group dataRow by Function.ToString(dataRow["Code"])).Any((IGrouping<string, DataRow> source) => source.Skip(1).Any()))
				{
					WinCommons.Error(Enums.MessageCode.CodeCannotBeDuplicate, "");
					return "Error";
				}
				if (dataTable.ExistsColumn("Code") && dataTable.Columns["Code"].DataType != typeof(int) && dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Code"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.EnterValidCode, "");
					return "Error";
				}
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["Name"])))
					{
						WinCommons.Error(Enums.MessageCode.CannotInsertNullValueIntoColumName, "", _tableName);
						return "Error";
					}
				}
				string currentEmp = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
				Tuple<DataTable, string> pr = new Tuple<DataTable, string>(dataTable, _tableName);
				using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "UDT");
				string x = await helper.PostAsync("Save", pr);
				if (string.IsNullOrEmpty(x))
				{
					if (_defineNew && RefreshLookupEdit != null)
					{
						string newCode = _newCode;
						RefreshLookupEdit(newCode);
						_newCode = string.Empty;
					}
					await LoadData();
					WinCommons.Success(Enums.MessageCode.Success, "");
					int num2 = gvDoc.LocateByValue("Code", currentEmp);
					if (num2 != int.MinValue)
					{
						gvDoc.FocusedRowHandle = num2;
						gvDoc.MakeRowVisible(num2);
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

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("IsNewRow")) == "Y")
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Update;
			string code = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
			if (string.IsNullOrEmpty(code))
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Code"]) == code && Function.ToString(t["IsNewRow"]) != "Y");
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					int rowHandle = gvDoc.GetRowHandle(dataSourceIndex);
					gvDoc.DeleteRow(rowHandle);
					dataTable.AcceptChanges();
				}
			}
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_defineNew && e.Column.FieldName == "Code")
			{
				_newCode = Function.ToString(e.Value);
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
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
					btnCancel.Enabled = false;
					await SaveData();
					btn_Save.Enabled = true;
					btnCancel.Enabled = true;
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

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!(sender is GridViewEx gridViewEx))
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull() && e.Column.FieldName == "Code")
			{
				if (dataTable.ExistsColumn("Code") && dataTable.Columns["Code"].DataType == typeof(int))
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else if (Function.ToString(gridViewEx.GetRowCellValue(e.RowHandle, "IsNewRow")) != "Y")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
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
			if (!(sender is GridViewEx gridViewEx))
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull() && gridViewEx.FocusedColumn.FieldName == "Code")
			{
				if (dataTable.ExistsColumn("Code") && dataTable.Columns["Code"].DataType == typeof(int))
				{
					e.Cancel = true;
				}
				else if (Function.ToString(gridViewEx.GetFocusedRowCellValue("IsNewRow")) != "Y")
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_SetUpUDT_FormClosing(object sender, FormClosingEventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.UDT.frm_SetUpUDT));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_GLAcct = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_UOM = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UOM).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(108, 336);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(96, 20);
		this.btnCancel.TabIndex = 81;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(-1, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_GLAcct, this.rep_UOM });
		this.grdDoc.Size = new System.Drawing.Size(622, 330);
		this.grdDoc.TabIndex = 80;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = "Code";
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn2, this.gridColumnEx1 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oSetupUdt";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = true;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "ODOC";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Code";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 125;
		this.gridColumn2.Caption = "Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Name";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 117;
		this.gridColumnEx1.Caption = "Is New Row";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "IsNewRow";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.rep_GLAcct.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_GLAcct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_GLAcct.Name = "rep_GLAcct";
		this.rep_UOM.AutoHeight = false;
		this.rep_UOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UOM.Name = "rep_UOM";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(7, 336);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(96, 20);
		this.btn_Save.TabIndex = 82;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(620, 365);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btn_Save);
		base.FormId = "oSetupUdt";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_SetUpUDT.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SetUpUDT";
		this.Text = "-Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_SetUpUDT_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UOM).EndInit();
		base.ResumeLayout(false);
	}
}
