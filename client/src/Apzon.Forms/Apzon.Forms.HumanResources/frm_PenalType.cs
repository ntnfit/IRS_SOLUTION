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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources;

public class frm_PenalType : XtraFormEx
{
	public RefreshLookupType refreshType;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public bool _isChanging;

	public bool _isCancel;

	public bool _forceClose;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx cl_Code;

	private GridColumnEx cl_name;

	private GridColumnEx cl_Remark;

	private GridColumnEx cl_Amount;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridColumnEx cl_Type;

	private RepositoryItemLookUpEdit rep_Type;

	private GridColumnEx gridColumnEx1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
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

	public frm_PenalType()
	{
		InitializeComponent();
		FormId = "oPenalType";
		gv_Doc.ItemId = "APZ_OHPT";
		gv_Doc.FormId = FormId;
		gv_Doc.ChangeLogType = Enums.ChangeLog.clPenalType;
		gv_Doc.IsApzTable = true;
		gv_Doc.ShowingEditor += GvDocOnShowingEditor;
		OnLoadAsync += frm_PenalType_Load;
	}

	private void GvDocOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_Doc.FocusedColumn.FieldName == "Code")
		{
			e.Cancel = true;
		}
	}

	private void LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "P",
					Name = "Penalty"
				},
				new Common.Table_Code
				{
					Code = "B",
					Name = "Bonus"
				}
			};
			rep_Type.DataSource = list;
			rep_Type.DisplayMember = "Name";
			rep_Type.ValueMember = "Code";
			rep_Type.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Type.PopulateColumns();
			rep_Type.Columns["Code"].Visible = false;
			rep_Type.DropDownRows = list.Count;
			rep_Type.PopupWidth = cl_Type.Width;
			rep_Type.ShowHeader = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = await _httpClient.PenalType.GetData();
			if (dataTable.IsNotNull())
			{
				dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Name"])))
				{
					AddNewRow();
				}
			}
			else
			{
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				AddNewRow();
			}
			FormMode = Enums.FormViewModeType.View;
			gv_Doc.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			gv_Doc.BeginInsertRow();
			DataTable dataTable = grd_Doc.DataSourceTable();
			if (!dataTable.HasColumn())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHPT");
				dataTable.Columns.Add("TypeGroup", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			if (dataTable.Rows.Count > 0)
			{
				dataRow["Code"] = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Code"])) + 1;
			}
			else
			{
				dataRow["Code"] = 1;
			}
			dataRow["Name"] = "";
			dataRow["Amount"] = 0;
			dataRow["Type"] = "P";
			dataRow["TypeGroup"] = "Penalty";
			dataRow["Remarks"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			gv_Doc.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Doc.EndInsertRow();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["Name"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
					return "Error";
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Amount"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.AmountCannotBeEmpty, "");
					return "Error";
				}
				if ((from x in dataTable2.AsEnumerable()
					group x by Function.ToString(x["Name"])).Any((IGrouping<string, DataRow> x) => x.Skip(1).Any()))
				{
					WinCommons.Error(Enums.MessageCode.NameCannotBeDuplicate, "");
					return "Error";
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["Name"])) && !string.IsNullOrEmpty(Function.ToString(t["Amount"]))
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable().Copy();
				}
				if (dataTable.IsNotNull())
				{
					WinCommons._OpenProcessing();
					for (int num = 0; num < dataTable.Rows.Count; num++)
					{
						DataRow dataRow = dataTable.Rows[num];
						dataRow["UserSign2"] = GlobalData.UserID;
						dataRow["UpdateDate"] = DateTime.Today;
					}
					string text = await _httpClient.PenalType.Save(dataTable);
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						REFRESH_DATA();
						if (refreshType != null && !((XtraForm)refreshType.Target).IsDisposed)
						{
							refreshType();
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		return string.Empty;
	}

	private async Task frm_PenalType_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			LoadInitForm();
			await WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OHPT", new Dictionary<string, GridViewEx> { { "APZ_OHPT", gv_Doc } });
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Type")) == "P")
				{
					_isChanging = true;
					gv_Doc.BeginUpdate();
					gv_Doc.SetRowCellValue(e.RowHandle, "TypeGroup", "Penalty");
					gv_Doc.EndUpdate();
					_isChanging = false;
				}
				else
				{
					_isChanging = true;
					gv_Doc.BeginUpdate();
					gv_Doc.SetRowCellValue(e.RowHandle, "TypeGroup", "Bonus");
					gv_Doc.EndUpdate();
					_isChanging = false;
				}
				gv_Doc.PostEditor();
				gv_Doc.RefreshData();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
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

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			_isCancel = FormMode != Enums.FormViewModeType.Update;
			Close();
			_isCancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_PenalType_FormClosing(object sender, FormClosingEventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = grd_Doc.DataSourceTable;
			if (!dt.IsNotNull())
			{
				return;
			}
			int num = Function.ParseInt(dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["Code"])));
			if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Code")) == num || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			string text = await _httpClient.PenalType.Remove(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("Code")));
			if (string.IsNullOrEmpty(text))
			{
				gv_Doc.BeginUpdate();
				gv_Doc.DeleteSelectedRows();
				gv_Doc.EndUpdate();
				dt.AcceptChanges();
				grd_Doc.RefreshDataSource();
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				if (refreshType != null && !((XtraForm)refreshType.Target).IsDisposed)
				{
					refreshType();
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Type_EditValueChanged(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_PenalType));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Code = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Amount = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.cl_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(1, 306);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 85;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(68, 306);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 84;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(1, 1);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCalcEdit1, this.repositoryItemMemoExEdit1, this.rep_Type });
		this.grd_Doc.Size = new System.Drawing.Size(588, 299);
		this.grd_Doc.TabIndex = 83;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "Code";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumnEx1, this.cl_Code, this.cl_name, this.cl_Amount, this.cl_Type, this.cl_Remark });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oPenalType";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = true;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OHPT";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx1, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gridColumnEx1.Caption = "Type Group";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "TypeGroup";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowMove = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.cl_Code.Caption = "Code";
		this.cl_Code.CurrencyDateField = null;
		this.cl_Code.CurrencyField = null;
		this.cl_Code.CurrencyFixed = null;
		this.cl_Code.CurrencyFixedRate = 0.0;
		this.cl_Code.CurrencyRateField = null;
		this.cl_Code.FieldName = "Code";
		this.cl_Code.Name = "cl_Code";
		this.cl_Code.OptionsColumn.ReadOnly = true;
		this.cl_Code.PrimaryKey = false;
		this.cl_Code.UnitField = null;
		this.cl_name.Caption = "Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "Name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = false;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_name.Width = 291;
		this.cl_Amount.Caption = "Fixed Amount";
		this.cl_Amount.ColumnEdit = this.repositoryItemCalcEdit1;
		this.cl_Amount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.cl_Amount.CurrencyDateField = null;
		this.cl_Amount.CurrencyField = null;
		this.cl_Amount.CurrencyFixed = null;
		this.cl_Amount.CurrencyFixedRate = 0.0;
		this.cl_Amount.CurrencyRateField = null;
		this.cl_Amount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.cl_Amount.FieldName = "Amount";
		this.cl_Amount.Name = "cl_Amount";
		this.cl_Amount.PrimaryKey = false;
		this.cl_Amount.UnitField = null;
		this.cl_Amount.Visible = true;
		this.cl_Amount.VisibleIndex = 1;
		this.cl_Amount.Width = 238;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.cl_Type.Caption = "Type";
		this.cl_Type.ColumnEdit = this.rep_Type;
		this.cl_Type.CurrencyDateField = null;
		this.cl_Type.CurrencyField = null;
		this.cl_Type.CurrencyFixed = null;
		this.cl_Type.CurrencyFixedRate = 0.0;
		this.cl_Type.CurrencyRateField = null;
		this.cl_Type.FieldName = "Type";
		this.cl_Type.Name = "cl_Type";
		this.cl_Type.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
		this.cl_Type.PrimaryKey = false;
		this.cl_Type.UnitField = null;
		this.cl_Type.Visible = true;
		this.cl_Type.VisibleIndex = 2;
		this.cl_Type.Width = 228;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Type.Name = "rep_Type";
		this.rep_Type.NullText = "";
		this.rep_Type.EditValueChanged += new System.EventHandler(this.rep_Type_EditValueChanged);
		this.cl_Remark.Caption = "Description";
		this.cl_Remark.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.cl_Remark.CurrencyDateField = null;
		this.cl_Remark.CurrencyField = null;
		this.cl_Remark.CurrencyFixed = null;
		this.cl_Remark.CurrencyFixedRate = 0.0;
		this.cl_Remark.CurrencyRateField = null;
		this.cl_Remark.FieldName = "Remarks";
		this.cl_Remark.Name = "cl_Remark";
		this.cl_Remark.PrimaryKey = false;
		this.cl_Remark.UnitField = null;
		this.cl_Remark.Visible = true;
		this.cl_Remark.VisibleIndex = 3;
		this.cl_Remark.Width = 426;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(589, 330);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oPenalType";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PenalType";
		this.Text = "Penalty/Bonus Type";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PenalType_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
