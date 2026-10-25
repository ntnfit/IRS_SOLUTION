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
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.Banking;

public class frm_CreditPaymentMethods : XtraFormEx
{
	private const string FormController = "CreditPaymentMethods";

	private const string tableName = "OCRP";

	private bool _isCancel;

	private bool _forceClose;

	private int _maxEntry;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private RepositoryItemButtonEdit rep_GLAcct;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemLookUpEdit rep_CreditCard;

	private RepositoryItemLookUpEdit rep_PaymentCode;

	private RepositoryItemLookUpEdit rep_Payments;

	private RepositoryItemCalcEdit rep_Amount;

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

	public frm_CreditPaymentMethods()
	{
		InitializeComponent();
		FormId = "oCreditPaymentMethods";
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "OCRP";
		OnLoadAsync += frm_CreditPaymentMethods_Load;
	}

	private void LoadInitForm()
	{
		try
		{
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreditPaymentMethods"))
			{
				DataTable dataSource = httpClientHelper.Get("GetCreditCards");
				rep_CreditCard.Columns.Clear();
				rep_CreditCard.Columns.Add(new LookUpColumnInfo
				{
					Caption = "",
					FieldName = "Name"
				});
				rep_CreditCard.DisplayMember = "Name";
				rep_CreditCard.ValueMember = "Code";
				rep_CreditCard.NullText = "";
				rep_CreditCard.DataSource = dataSource;
			}
			using (HttpClientHelper<DataTable> httpClientHelper2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreditPaymentMethods"))
			{
				DataTable dataSource2 = httpClientHelper2.Get("GetPaymentCodes");
				rep_PaymentCode.Columns.Clear();
				rep_PaymentCode.Columns.Add(new LookUpColumnInfo
				{
					Caption = "",
					FieldName = "Code"
				});
				rep_PaymentCode.Columns.Add(new LookUpColumnInfo
				{
					Caption = "",
					FieldName = "Name"
				});
				rep_PaymentCode.DisplayMember = "Name";
				rep_PaymentCode.ValueMember = "Code";
				rep_PaymentCode.NullText = "";
				rep_PaymentCode.DataSource = dataSource2;
			}
			List<Common.Table_Code> dataSource3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "Y",
					Name = "Yes"
				},
				new Common.Table_Code
				{
					Code = "N",
					Name = "No"
				},
				new Common.Table_Code
				{
					Code = "C",
					Name = "Cr"
				},
				new Common.Table_Code
				{
					Code = "L",
					Name = "Rd"
				}
			};
			rep_Payments.DataSource = dataSource3;
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
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
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
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreditPaymentMethods");
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

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["CrTypeName"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				if (dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["CrTypeName"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.TypeNameCannotEmpty, "");
					return "Error";
				}
				if (CheckPaymentMethodName(dataTable2))
				{
					WinCommons.Error(Enums.MessageCode.TypeNameCannotDuplicate, "");
					return "Error!";
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["CrTypeName"]))
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable().Copy();
				}
				WinCommons._OpenProcessing();
				using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "CreditPaymentMethods");
				string x = await helper.PostAsync("Save", dataTable);
				if (string.IsNullOrEmpty(x))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, x);
				}
				WinCommons._CloseProcessing();
				return x;
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
		return string.Empty;
	}

	private void AddNew()
	{
		try
		{
			gvDoc.BeginInsertRow();
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				_maxEntry = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["CrTypeCode"]));
				DataRow dataRow = dataTable.NewRow();
				dataRow["CrTypeCode"] = _maxEntry + 1;
				dataRow["CrTypeName"] = string.Empty;
				dataRow["CreditCard"] = DBNull.Value;
				dataRow["DueTerms"] = string.Empty;
				dataRow["MinCredit"] = DBNull.Value;
				dataRow["MinToPay"] = DBNull.Value;
				dataRow["MaxValid"] = DBNull.Value;
				dataRow["InstalMent"] = "N";
				dataRow["Locked"] = "N";
				dataRow["DataSource"] = DBNull.Value;
				dataRow["UserSign"] = GlobalData.UserID;
				dataTable.Rows.Add(dataRow);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("CrTypeCode", typeof(int));
				dataTable2.Columns.Add("CrTypeName", typeof(string));
				dataTable2.Columns.Add("CreditCard", typeof(int));
				dataTable2.Columns.Add("DueTerms", typeof(string));
				dataTable2.Columns.Add("MinCredit", typeof(double));
				dataTable2.Columns.Add("MinToPay", typeof(double));
				dataTable2.Columns.Add("MaxValid", typeof(double));
				dataTable2.Columns.Add("InstalMent", typeof(string));
				dataTable2.Columns.Add("Locked", typeof(string));
				dataTable2.Columns.Add("DataSource", typeof(string));
				dataTable2.Columns.Add("UserSign", typeof(int));
				dataTable2.Rows.Add(1, string.Empty, DBNull.Value, string.Empty, DBNull.Value, DBNull.Value, DBNull.Value, "N", "N", DBNull.Value, GlobalData.UserID);
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

	private void Remove(int crTypeCode)
	{
		WinCommons._OpenProcessing();
		using (HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "CreditPaymentMethods"))
		{
			string text = httpClientHelper.Post("DeleteLine", crTypeCode);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			LoadData();
		}
		WinCommons._CloseProcessing();
	}

	private bool CheckPaymentMethodName(DataTable dt)
	{
		int num = 0;
		bool result = false;
		try
		{
			if (dt.Select("CrTypeName <> ''").Any())
			{
				EnumerableRowCollection<DataRow> source = from r in dt.AsEnumerable()
					where r.Field<string>("CrTypeName").Trim() != ""
					select r;
				if (source.Any() && (from dr in source.CopyToDataTable().AsEnumerable()
					select dr.Field<string>("CrTypeName").Trim() into x
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

	private async Task frm_CreditPaymentMethods_Load(object sender, EventArgs e)
	{
		LoadInitForm();
		await LoadData();
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

	private async void frm_CreditPaymentMethods_FormClosing(object sender, FormClosingEventArgs e)
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
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[gvDoc.FocusedRowHandle]["CrTypeCode"]);
				if (num != _maxEntry && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[gvDoc.FocusedRowHandle]["CrTypeName"])) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Remove(num);
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.Banking.frm_CreditPaymentMethods));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CreditCard = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_PaymentCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Amount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Payments = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_GLAcct = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreditCard).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PaymentCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Amount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Payments).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 365);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 76;
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
		this.btnCancel.Location = new System.Drawing.Point(105, 365);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_GLAcct, this.rep_CreditCard, this.rep_PaymentCode, this.rep_Payments, this.rep_Amount });
		this.grdDoc.Size = new System.Drawing.Size(815, 359);
		this.grdDoc.TabIndex = 74;
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
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oCreditPaymentMethod";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = true;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ODOC";
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
		this.gridColumn1.Caption = "Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "CrTypeCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "CrTypeName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 99;
		this.gridColumn3.Caption = "Card";
		this.gridColumn3.ColumnEdit = this.rep_CreditCard;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "CreditCard";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 88;
		this.rep_CreditCard.AutoHeight = false;
		this.rep_CreditCard.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CreditCard.DisplayMember = "Name";
		this.rep_CreditCard.Name = "rep_CreditCard";
		this.rep_CreditCard.NullText = "";
		this.rep_CreditCard.ValueMember = "Code";
		this.gridColumn4.Caption = "Payment Code";
		this.gridColumn4.ColumnEdit = this.rep_PaymentCode;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DueTerms";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn4.Width = 125;
		this.rep_PaymentCode.AutoHeight = false;
		this.rep_PaymentCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_PaymentCode.DisplayMember = "Name";
		this.rep_PaymentCode.Name = "rep_PaymentCode";
		this.rep_PaymentCode.NullText = "";
		this.rep_PaymentCode.ValueMember = "Code";
		this.gridColumn5.Caption = "Min. Credit Voucher";
		this.gridColumn5.ColumnEdit = this.rep_Amount;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatString = "n2";
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn5.FieldName = "MinCredit";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn5.Width = 123;
		this.rep_Amount.AutoHeight = false;
		this.rep_Amount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Amount.Name = "rep_Amount";
		this.gridColumn6.Caption = "Min. Payment";
		this.gridColumn6.ColumnEdit = this.rep_Amount;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormat.FormatString = "n2";
		this.gridColumn6.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn6.FieldName = "MinToPay";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.gridColumn6.Width = 122;
		this.gridColumn7.Caption = "Without Approval";
		this.gridColumn7.ColumnEdit = this.rep_Amount;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormat.FormatString = "n2";
		this.gridColumn7.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn7.FieldName = "MaxValid";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 5;
		this.gridColumn7.Width = 115;
		this.gridColumn8.Caption = "Payments";
		this.gridColumn8.ColumnEdit = this.rep_Payments;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "InstalMent";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 6;
		this.gridColumn8.Width = 171;
		this.rep_Payments.AutoHeight = false;
		this.rep_Payments.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Payments.DisplayMember = "Name";
		this.rep_Payments.Name = "rep_Payments";
		this.rep_Payments.NullText = "";
		this.rep_Payments.ValueMember = "Code";
		this.rep_GLAcct.AutoHeight = false;
		this.rep_GLAcct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_GLAcct.Name = "rep_GLAcct";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(816, 389);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oCreditPaymentMethod";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_CreditPaymentMethods";
		this.Text = "Credit Card Payment Methods - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CreditPaymentMethods_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CreditCard).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PaymentCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Amount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Payments).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).EndInit();
		base.ResumeLayout(false);
	}
}
