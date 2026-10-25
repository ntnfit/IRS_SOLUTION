using System;
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
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.Banking;

public class frm_CreditCardSetup : XtraFormEx
{
	private const string FormController = "CreditCard";

	private const string tableName = "OCRC";

	private string _privilege = "";

	public new string FormId;

	private int _maxEntry;

	private bool _isCancel;

	private bool _forceClose;

	private IContainer components;

	private GridControlEx grdDoc;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridViewEx gvDoc;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private RepositoryItemButtonEdit rep_GLAcct;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
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

	public frm_CreditCardSetup()
	{
		InitializeComponent();
		OnLoadAsync += frm_CreditCardSetup_Load;
		FormId = "oCreditCard";
		gvDoc.ItemId = "OCRC";
		gvDoc.FormId = FormId;
	}

	private void LoadInItForm()
	{
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNew()
	{
		try
		{
			gvDoc.BeginInsertRow();
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				_maxEntry = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["CreditCard"]));
				DataRow dataRow = dataTable.NewRow();
				dataRow["CreditCard"] = _maxEntry + 1;
				dataRow["CardName"] = DBNull.Value;
				dataRow["AcctCode"] = DBNull.Value;
				dataRow["Phone"] = DBNull.Value;
				dataRow["CompanyId"] = DBNull.Value;
				dataRow["Locked"] = "N";
				dataRow["DataSource"] = DBNull.Value;
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["LogInstanc"] = 0;
				dataRow["UpdateDate"] = DateTime.Now;
				dataRow["IntTaxCode"] = DBNull.Value;
				dataRow["UserSign2"] = DBNull.Value;
				dataRow["Country"] = DBNull.Value;
				dataTable.Rows.Add(dataRow);
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("CreditCard", typeof(int));
				dataTable2.Columns.Add("CardName", typeof(string));
				dataTable2.Columns.Add("AcctCode", typeof(string));
				dataTable2.Columns.Add("Phone", typeof(string));
				dataTable2.Columns.Add("CompanyId", typeof(string));
				dataTable2.Columns.Add("Locked", typeof(string));
				dataTable2.Columns.Add("DataSource", typeof(string));
				dataTable2.Columns.Add("UserSign", typeof(int));
				dataTable2.Columns.Add("LogInstanc", typeof(int));
				dataTable2.Columns.Add("UpdateDate", typeof(DateTime));
				dataTable2.Columns.Add("IntTaxCode", typeof(string));
				dataTable2.Columns.Add("UserSign2", typeof(int));
				dataTable2.Columns.Add("Country", typeof(string));
				dataTable2.Rows.Add(1, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, "N", DBNull.Value, GlobalData.UserID, 0, DateTime.Now, DBNull.Value, DBNull.Value, DBNull.Value);
				dataTable2.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable2;
				grdDoc.RefreshDataSource();
			}
			gvDoc.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		gvDoc.EndInsertRow();
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
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["CardName"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["CardName"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.CardNameCannotBeEmpty, "");
					return "Error";
				}
				if (CheckCardName(dataTable2))
				{
					WinCommons.Error(Enums.MessageCode.CardNameCannotBeDuplicate, "");
					return "Error";
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["CardName"]))
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable().Copy();
				}
				WinCommons._OpenProcessing();
				using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "CreditCard");
				string text = await helper.PostAsync("Save", dataTable);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
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
		return string.Empty;
	}

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			int codeMax = 0;
			if (dataTable.IsNotNull())
			{
				codeMax = Function.ParseInt(dataTable.Rows[dataTable.Rows.Count - 1]["CreditCard"]);
			}
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreditCard");
			DataTable dataTable2 = await helper.GetAsync("GetAddNewRow", new
			{
				tableName = "OCRC",
				codeMax = codeMax
			});
			if (dataTable2.IsNotNull())
			{
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreditCard");
			DataTable dataTable = await helper.GetAsync("LoadDoc");
			dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool CheckCardName(DataTable dt)
	{
		int num = 0;
		bool result = false;
		try
		{
			if (dt.Select("CardName <> ''").Any())
			{
				EnumerableRowCollection<DataRow> source = from r in dt.AsEnumerable()
					where r.Field<string>("CardName").Trim() != ""
					select r;
				if (source.Any() && (from dr in source.CopyToDataTable().AsEnumerable()
					select dr.Field<string>("CardName").Trim() into x
					group x by x into g
					where g.Count() > 1
					select g.Key).ToList().Count > 0)
				{
					num++;
				}
			}
			if (num > 0)
			{
				result = true;
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return result;
		}
	}

	private async Task frm_CreditCardSetup_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInItForm();
			await LoadData();
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

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
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

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull() && Function.ParseInt(dataTable.Rows[gvDoc.FocusedRowHandle]["CardName"]) != _maxEntry && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[gvDoc.FocusedRowHandle]["CardName"])) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				dataTable.Rows.RemoveAt(gvDoc.FocusedRowHandle);
				dataTable.AcceptChanges();
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_GLAcct_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ChartOfAccount, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("AcctCode", chooseItem["AcctCode"]);
				grdDoc.RefreshDataSource();
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
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					btn_Save.Enabled = false;
					btnCancel.Enabled = false;
					await SaveData();
					btn_Save.Enabled = true;
					btnCancel.Enabled = true;
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
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private async void frm_CreditCardSetup_FormClosing(object sender, FormClosingEventArgs e)
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

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
		this.components = new System.ComponentModel.Container();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.Banking.frm_CreditCardSetup));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_GLAcct = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_GLAcct });
		this.grdDoc.Size = new System.Drawing.Size(715, 366);
		this.grdDoc.TabIndex = 71;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Visible = false;
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = "UomEntry";
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oCreditCardSetup";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = true;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OCCS";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gridColumn1.Caption = "CreditCard";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "CreditCard";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Credit Card Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "CardName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 179;
		this.gridColumn3.Caption = "G/L Account";
		this.gridColumn3.ColumnEdit = this.rep_GLAcct;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "AcctCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 141;
		this.rep_GLAcct.AutoHeight = false;
		this.rep_GLAcct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_GLAcct.Name = "rep_GLAcct";
		this.rep_GLAcct.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_GLAcct_ButtonClick);
		this.gridColumn4.Caption = "Telephone";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Phone";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn4.Width = 149;
		this.gridColumn5.Caption = "Company ID";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "CompanyId";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn5.Width = 191;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 372);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 73;
		this.btn_Save.Text = "Save";
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
		this.btnCancel.Location = new System.Drawing.Point(105, 372);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 72;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(715, 402);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_CreditCardSetup.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CreditCardSetup";
		base.ShowInTaskbar = false;
		this.Text = "Credit Cards Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CreditCardSetup_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).EndInit();
		base.ResumeLayout(false);
	}
}
