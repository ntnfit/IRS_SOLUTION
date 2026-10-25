using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.BusinessMaster;

public class frm_CreditCardType : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public delegate void CloseEvent(string str, string value);

	private string _privilege = "";

	private List<CreditCardTypesImpl> lst;

	public ShowForms frmShowChildrenForm;

	private int docEntry;

	private int newDocEntry;

	public CloseEvent reloadParentComboBox;

	public string _comboValue = "";

	private IContainer components;

	private SimpleButtonEx btCancel;

	private PanelControl panelControl1;

	private PanelControl panelControl4;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx CreditCard;

	private GridColumnEx CardName;

	private GridColumnEx Phone;

	private PanelControl panelControl3;

	private SimpleButtonEx btOK;

	private GridViewEx tblViewManufacturer;

	private GridColumnEx STT;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx CompanyId;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx IntTaxCode;

	private GridColumnEx Locked;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CreditCardType()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CreditCardType"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CreditCardType" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void frm_CreditCardType_Load(object sender, EventArgs e)
	{
		RefreshData();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Resources.OCRC_Title_frm_CreditCardType;
		addNewToolStripMenuItem.Text = Resources.ctx_Add;
		deleteToolStripMenuItem.Text = Resources.ctx_Delete;
		btCancel.Text = Resources.ButtonCancel;
		FormMode = Enums.FormViewModeType.View;
		gvDoc.Columns["CreditCard"].Caption = Resources.OCRC_CreditCard;
		gvDoc.Columns["CardName"].Caption = Resources.OCRC_CardName;
		gvDoc.Columns["CompanyId"].Caption = Resources.OCRC_CompanyId;
		gvDoc.Columns["Phone"].Caption = Resources.OCRC_Phone;
		gvDoc.Columns["IntTaxCode"].Caption = Resources.OCRC_IntTaxCode;
		gvDoc.Columns["Locked"].Caption = Resources.lb_Locked;
	}

	private void RefreshData()
	{
		GetNewDocEntry();
		LoadData();
	}

	private async void LoadData()
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "CreditCardType", "ShowCreditCardType");
		lst = new List<CreditCardTypesImpl>();
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (!response.IsSuccessStatusCode)
		{
			return;
		}
		lst = JsonConvert.DeserializeObject<List<CreditCardTypesImpl>>(await response.Content.ReadAsStringAsync()).ToList();
		if (grdDoc.InvokeRequired)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				grdDoc.DataSource = lst;
				grdDoc.RefreshDataSource();
			}));
		}
		else
		{
			grdDoc.DataSource = lst;
			grdDoc.RefreshDataSource();
		}
		AddNewRow();
	}

	private async void GetNewDocEntry()
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "CreditCard", "GetMaxCode");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			newDocEntry = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync()) + 1;
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBox.Show("Are  you sure want to delete?", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int code = Convert.ToInt32(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "CreditCard"));
				DeleteDoc(code);
				gvDoc.DeleteSelectedRows();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void DeleteDoc(int code)
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<int>(code, new JsonMediaTypeFormatter());
		httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "CreditCardType", "DeleteDoc"));
		httpRequestMessage.Method = HttpMethod.Post;
		using HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
		if (response.IsSuccessStatusCode)
		{
			string text = await response.Content.ReadAsStringAsync();
			if (text == "null")
			{
				WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Error, Msg_Time._TimeShort, "");
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Warning, Msg_Time._TimeShort, text);
			}
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
	}

	private void AddNewRow()
	{
		List<CreditCardTypesImpl> lstNew = new List<CreditCardTypesImpl>();
		if (gvDoc.DataSource != null)
		{
			lstNew = gvDoc.DataSource as List<CreditCardTypesImpl>;
			if (lstNew.Where((CreditCardTypesImpl x) => x.CardName.Trim() == "" && x.CompanyId.Trim() == "" && x.Phone.Trim() == "" && x.IntTaxCode.Trim() == "" && x.Locked.Trim() == "N").ToList().Count == 0)
			{
				CreditCardTypesImpl creditCardTypesImpl = new CreditCardTypesImpl();
				int num = ((lstNew.Count == 0) ? 1 : (lstNew.Max((CreditCardTypesImpl x) => x.CreditCard) + 1));
				newDocEntry = ((newDocEntry > num) ? newDocEntry : num);
				creditCardTypesImpl.CreditCard = newDocEntry;
				creditCardTypesImpl.CardName = string.Empty;
				creditCardTypesImpl.CompanyId = string.Empty;
				creditCardTypesImpl.Phone = string.Empty;
				creditCardTypesImpl.IntTaxCode = string.Empty;
				creditCardTypesImpl.Locked = "N";
				lstNew.Add(creditCardTypesImpl);
			}
		}
		else
		{
			CreditCardTypesImpl creditCardTypesImpl2 = new CreditCardTypesImpl();
			int num2 = ((lstNew.Count == 0) ? 1 : (lstNew.Max((CreditCardTypesImpl x) => x.CreditCard) + 1));
			newDocEntry = ((newDocEntry > num2) ? newDocEntry : num2);
			creditCardTypesImpl2.CreditCard = newDocEntry;
			creditCardTypesImpl2.CardName = string.Empty;
			creditCardTypesImpl2.CompanyId = string.Empty;
			creditCardTypesImpl2.Phone = string.Empty;
			creditCardTypesImpl2.IntTaxCode = string.Empty;
			creditCardTypesImpl2.Locked = "N";
			lstNew.Add(creditCardTypesImpl2);
		}
		if (grdDoc.InvokeRequired)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				grdDoc.DataSource = lstNew;
				grdDoc.RefreshDataSource();
			}));
		}
		else
		{
			grdDoc.DataSource = lstNew;
			grdDoc.RefreshDataSource();
		}
	}

	private void gvDoc_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == CreditCard || e.Column == CardName || e.Column == CompanyId || e.Column == Phone || e.Column == Locked)
		{
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			Add();
			FormMode = Enums.FormViewModeType.View;
		}
		else
		{
			Close();
		}
	}

	private async void Add()
	{
		try
		{
			Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, CreditCard));
			List<CreditCardTypesImpl> source = (List<CreditCardTypesImpl>)grdDoc.DataSource;
			new CreditCardTypesImpl();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			source = source.Where((CreditCardTypesImpl x) => x.CardName.Trim() != "" && x.CompanyId.Trim() != "" && x.Phone.Trim() != "" && x.IntTaxCode.Trim() != "").ToList();
			foreach (CreditCardTypesImpl item in source)
			{
				await Save(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> Save(CreditCardTypesImpl doc)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<CreditCardTypesImpl>(doc, new JsonMediaTypeFormatter());
		httpRequestMessage.Method = HttpMethod.Post;
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "CreditCardType", "SaveDoc");
		httpRequestMessage.RequestUri = new Uri(apiUri);
		await new HttpClient
		{
			DefaultRequestHeaders = { 
			{
				"SessionId",
				GlobalData.SessionId
			} }
		}.SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
		{
			if (task.Result.IsSuccessStatusCode)
			{
				Task<string> task2 = task.Result.Content.ReadAsStringAsync();
				if (task2 != null && string.IsNullOrEmpty(task2.Result))
				{
					return task2.Result;
				}
				WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				RefreshData();
			}
			return (string)null;
		});
		return null;
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	protected override void OnClosed(EventArgs e)
	{
		try
		{
			reloadParentComboBox("CreditType", _comboValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_CreditCardType));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.CreditCard = new Apzon.Controls.Grid.GridColumnEx();
		this.CardName = new Apzon.Controls.Grid.GridColumnEx();
		this.CompanyId = new Apzon.Controls.Grid.GridColumnEx();
		this.Phone = new Apzon.Controls.Grid.GridColumnEx();
		this.IntTaxCode = new Apzon.Controls.Grid.GridColumnEx();
		this.Locked = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tblViewManufacturer = new Apzon.Controls.Grid.GridViewEx();
		this.STT = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(103, 11);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 20);
		this.btCancel.TabIndex = 37;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.panelControl1.Controls.Add(this.panelControl4);
		this.panelControl1.Controls.Add(this.panelControl3);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(748, 385);
		this.panelControl1.TabIndex = 4;
		this.panelControl4.Controls.Add(this.grdDoc);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(2, 2);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(744, 346);
		this.panelControl4.TabIndex = 2;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCheckEdit1 });
		this.grdDoc.Size = new System.Drawing.Size(740, 342);
		this.grdDoc.TabIndex = 3;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(150, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.addNewToolStripMenuItem.Text = "Add New Row";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.deleteToolStripMenuItem.Text = "Delete Row";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.CreditCard, this.CardName, this.CompanyId, this.Phone, this.IntTaxCode, this.Locked });
		this.gvDoc.CustomizationFormBounds = new System.Drawing.Rectangle(433, 310, 216, 180);
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsFind.AlwaysVisible = true;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged_1);
		this.CreditCard.Caption = "Credit Card Code";
		this.CreditCard.CurrencyDateField = null;
		this.CreditCard.CurrencyField = null;
		this.CreditCard.CurrencyFixed = null;
		this.CreditCard.CurrencyFixedRate = 0.0;
		this.CreditCard.CurrencyRateField = null;
		this.CreditCard.FieldName = "CreditCard";
		this.CreditCard.Name = "CreditCard";
		this.CreditCard.OptionsColumn.AllowEdit = false;
		this.CreditCard.OptionsColumn.ReadOnly = true;
		this.CreditCard.PrimaryKey = true;
		this.CreditCard.UnitField = null;
		this.CreditCard.Visible = true;
		this.CreditCard.VisibleIndex = 0;
		this.CreditCard.Width = 110;
		this.CardName.Caption = "Credit Card Name";
		this.CardName.CurrencyDateField = null;
		this.CardName.CurrencyField = null;
		this.CardName.CurrencyFixed = null;
		this.CardName.CurrencyFixedRate = 0.0;
		this.CardName.CurrencyRateField = null;
		this.CardName.FieldName = "CardName";
		this.CardName.Name = "CardName";
		this.CardName.PrimaryKey = false;
		this.CardName.UnitField = null;
		this.CardName.Visible = true;
		this.CardName.VisibleIndex = 1;
		this.CardName.Width = 143;
		this.CompanyId.Caption = "Company ID";
		this.CompanyId.CurrencyDateField = null;
		this.CompanyId.CurrencyField = null;
		this.CompanyId.CurrencyFixed = null;
		this.CompanyId.CurrencyFixedRate = 0.0;
		this.CompanyId.CurrencyRateField = null;
		this.CompanyId.FieldName = "CompanyId";
		this.CompanyId.Name = "CompanyId";
		this.CompanyId.PrimaryKey = false;
		this.CompanyId.UnitField = null;
		this.CompanyId.Visible = true;
		this.CompanyId.VisibleIndex = 2;
		this.CompanyId.Width = 124;
		this.Phone.Caption = "Telephone";
		this.Phone.CurrencyDateField = null;
		this.Phone.CurrencyField = null;
		this.Phone.CurrencyFixed = null;
		this.Phone.CurrencyFixedRate = 0.0;
		this.Phone.CurrencyRateField = null;
		this.Phone.FieldName = "Phone";
		this.Phone.Name = "Phone";
		this.Phone.PrimaryKey = false;
		this.Phone.UnitField = null;
		this.Phone.Visible = true;
		this.Phone.VisibleIndex = 3;
		this.Phone.Width = 92;
		this.IntTaxCode.Caption = "Internal Tax Code";
		this.IntTaxCode.CurrencyDateField = null;
		this.IntTaxCode.CurrencyField = null;
		this.IntTaxCode.CurrencyFixed = null;
		this.IntTaxCode.CurrencyFixedRate = 0.0;
		this.IntTaxCode.CurrencyRateField = null;
		this.IntTaxCode.FieldName = "IntTaxCode";
		this.IntTaxCode.Name = "IntTaxCode";
		this.IntTaxCode.PrimaryKey = false;
		this.IntTaxCode.UnitField = null;
		this.IntTaxCode.Visible = true;
		this.IntTaxCode.VisibleIndex = 4;
		this.IntTaxCode.Width = 106;
		this.Locked.Caption = "Locked";
		this.Locked.ColumnEdit = this.repositoryItemCheckEdit1;
		this.Locked.CurrencyDateField = null;
		this.Locked.CurrencyField = null;
		this.Locked.CurrencyFixed = null;
		this.Locked.CurrencyFixedRate = 0.0;
		this.Locked.CurrencyRateField = null;
		this.Locked.FieldName = "Locked";
		this.Locked.Name = "Locked";
		this.Locked.PrimaryKey = false;
		this.Locked.UnitField = null;
		this.Locked.Visible = true;
		this.Locked.VisibleIndex = 5;
		this.Locked.Width = 98;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.DisplayValueChecked = "Y";
		this.repositoryItemCheckEdit1.DisplayValueUnchecked = "N";
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.panelControl3.Controls.Add(this.btOK);
		this.panelControl3.Controls.Add(this.btCancel);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(2, 348);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(744, 35);
		this.panelControl3.TabIndex = 1;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(0, 11);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(99, 20);
		this.btOK.TabIndex = 36;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.tblViewManufacturer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tblViewManufacturer.ColumnKey = null;
		this.tblViewManufacturer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.STT });
		this.tblViewManufacturer.DisableFormSetting = false;
		this.tblViewManufacturer.DisplayException = false;
		this.tblViewManufacturer.EditColumn = null;
		this.tblViewManufacturer.FieldCurrency = null;
		this.tblViewManufacturer.FieldRate = null;
		this.tblViewManufacturer.FormId = null;
		this.tblViewManufacturer.IgnoreFilter = false;
		this.tblViewManufacturer.IndexEditRow = -1;
		this.tblViewManufacturer.IndicatorWidth = 35;
		this.tblViewManufacturer.IsApzTable = false;
		this.tblViewManufacturer.IsInsertNewRow = false;
		this.tblViewManufacturer.IsLoadingData = false;
		this.tblViewManufacturer.IsMappingContextAction = false;
		this.tblViewManufacturer.ItemId = null;
		this.tblViewManufacturer.LstColumnAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotAllowBatch = null;
		this.tblViewManufacturer.LstColumnNotEditable = null;
		this.tblViewManufacturer.LstColumNotSetting = null;
		this.tblViewManufacturer.Name = "tblViewManufacturer";
		this.tblViewManufacturer.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Top;
		this.tblViewManufacturer.UseBatchMod = false;
		this.STT.Caption = "STT";
		this.STT.CurrencyDateField = null;
		this.STT.CurrencyField = null;
		this.STT.CurrencyFixed = null;
		this.STT.CurrencyFixedRate = 0.0;
		this.STT.CurrencyRateField = null;
		this.STT.Name = "STT";
		this.STT.PrimaryKey = false;
		this.STT.UnitField = null;
		this.STT.Visible = true;
		this.STT.VisibleIndex = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(748, 385);
		base.Controls.Add(this.panelControl1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.Name = "frm_CreditCardType";
		this.Text = "Credit Card - Setup";
		base.Load += new System.EventHandler(this.frm_CreditCardType_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).EndInit();
		base.ResumeLayout(false);
	}
}
