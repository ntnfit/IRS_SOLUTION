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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.BusinessMaster;

public class frm_CashDiscountEdit : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public delegate void CloseEvent();

	private string _privilege = "";

	public string Code;

	private int newDocEntry;

	public ShowForms frmShowChildrenForm;

	public CloseEvent reloadPaymentTermComboBox;

	private IContainer components;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btCancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private LabelLine lb_Code;

	private LabelLine lb_Name;

	private TextEditEx txtCode;

	private TextEditEx txtDesc;

	private GridColumnEx LineId;

	private GridColumnEx NumOfDays;

	private GridColumnEx Discount;

	private SimpleButtonEx btDelete;

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
		case Enums.FormViewModeType.Add:
			btOK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_CashDiscountEdit()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CashDiscountEdit"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_CashDiscountEdit" && x.authoriza == "F"))
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

	private void frm_CashDiscountAddNew_Load(object sender, EventArgs e)
	{
		RefreshData();
		SettingLanguage();
	}

	private void SettingLanguage()
	{
		Text = Resources.OCDC_Title_frm_CashDiscountEdit;
		btCancel.Text = Resources.ButtonCancel;
		FormMode = Enums.FormViewModeType.View;
		btDelete.Text = Resources.ButtonDelete;
		lb_Code.Utext = Resources.lb_Code;
		lb_Name.Utext = Resources.lb_Name;
		gvDoc.Columns["LineId"].Caption = Resources.CDC1_LineId;
		gvDoc.Columns["NumOfDays"].Caption = Resources.CDC1_NumOfDays;
		gvDoc.Columns["Discount"].Caption = Resources.CDC1_Discount;
	}

	private void RefreshData()
	{
		GetNewDocEntry();
		if (Code != "")
		{
			txtCode.Text = Code;
			txtCode.ReadOnly = true;
			LoadData(Code);
			LoadDataO(Code);
		}
		else
		{
			txtCode.ReadOnly = false;
			AddNewRow();
		}
	}

	private async void GetNewDocEntry()
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "CashDiscount", "GetMaxLineId");
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			newDocEntry = JsonConvert.DeserializeObject<int>(await response.Content.ReadAsStringAsync()) + 1;
		}
	}

	private async void LoadData(string iCode)
	{
		try
		{
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "CashDiscount", "GetCashDiscount", new { iCode });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (!response.IsSuccessStatusCode)
			{
				return;
			}
			List<CashDiscountsImpl> lst = JsonConvert.DeserializeObject<CashDiscountsImpl[]>(await response.Content.ReadAsStringAsync()).ToList();
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void LoadDataO(string iCode)
	{
		_ = 1;
		try
		{
			using HttpClient client = new HttpClient();
			client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "OCashDiscount", "GetOCashDiscount", new { iCode });
			using HttpResponseMessage response = await client.GetAsync(apiUri);
			if (response.IsSuccessStatusCode)
			{
				OCashDiscountsImpl[] array = JsonConvert.DeserializeObject<OCashDiscountsImpl[]>(await response.Content.ReadAsStringAsync()).ToArray();
				txtDesc.Text = array[0].TableDesc;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void AddNewRow()
	{
		List<CashDiscountsImpl> lstNew = new List<CashDiscountsImpl>();
		if (gvDoc.DataSource != null)
		{
			lstNew = gvDoc.DataSource as List<CashDiscountsImpl>;
			if (lstNew.Where((CashDiscountsImpl x) => x.NumOfDays == 0 && x.Discount == 0m).ToList().Count == 0)
			{
				CashDiscountsImpl cashDiscountsImpl = new CashDiscountsImpl();
				int num = ((lstNew.Count == 0) ? 1 : (lstNew.Max((CashDiscountsImpl x) => x.LineId) + 1));
				newDocEntry = ((newDocEntry > num) ? newDocEntry : num);
				cashDiscountsImpl.LineId = newDocEntry;
				cashDiscountsImpl.NumOfDays = 0;
				cashDiscountsImpl.Discount = 0m;
				lstNew.Add(cashDiscountsImpl);
			}
		}
		else
		{
			CashDiscountsImpl cashDiscountsImpl2 = new CashDiscountsImpl();
			int num2 = ((lstNew.Count == 0) ? 1 : (lstNew.Max((CashDiscountsImpl x) => x.LineId) + 1));
			newDocEntry = ((newDocEntry > num2) ? newDocEntry : num2);
			cashDiscountsImpl2.LineId = newDocEntry;
			cashDiscountsImpl2.NumOfDays = 0;
			cashDiscountsImpl2.Discount = 0m;
			lstNew.Add(cashDiscountsImpl2);
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

	private async void Add()
	{
		try
		{
			Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, LineId));
			List<CashDiscountsImpl> source = (List<CashDiscountsImpl>)grdDoc.DataSource;
			source = source.Where((CashDiscountsImpl x) => x.LineId != 0).ToList();
			foreach (CashDiscountsImpl item in source)
			{
				item.CdcCode = txtCode.Text.Trim();
				await Save(item);
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async void AddCode()
	{
		try
		{
			OCashDiscountsImpl oCashDiscountsImpl = new OCashDiscountsImpl();
			oCashDiscountsImpl.Code = txtCode.Text.Trim();
			oCashDiscountsImpl.TableDesc = txtDesc.Text.Trim();
			await SaveCode(oCashDiscountsImpl);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> Save(CashDiscountsImpl doc)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<CashDiscountsImpl>(doc, new JsonMediaTypeFormatter());
		httpRequestMessage.Method = HttpMethod.Post;
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "CashDiscount", "SaveDoc");
		httpRequestMessage.RequestUri = new Uri(apiUri);
		new List<CashDiscountsImpl>();
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

	private async Task<string> SaveCode(OCashDiscountsImpl doc)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<OCashDiscountsImpl>(doc, new JsonMediaTypeFormatter());
		httpRequestMessage.Method = HttpMethod.Post;
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "OCashDiscount", "SaveDoc");
		httpRequestMessage.RequestUri = new Uri(apiUri);
		new List<OCashDiscountsImpl>();
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

	private void btCancel_Click(object sender, EventArgs e)
	{
		reloadPaymentTermComboBox();
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && txtCode.Text.Trim() != "" && txtDesc.Text.Trim() != "")
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				AddCode();
				Add();
			}
		}
		else if (FormMode == Enums.FormViewModeType.View)
		{
			reloadPaymentTermComboBox();
			Close();
		}
	}

	private void txtCode_TextChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Add;
	}

	private void txtDesc_TextChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		AddNewRow();
		FormMode = Enums.FormViewModeType.Update;
	}

	private void txtDesc_EditValueChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		DeleteData(Code);
	}

	private async void DeleteData(string code)
	{
		await DeleteCDC(code);
		await DeleteOCDC(code);
		reloadPaymentTermComboBox();
		Close();
	}

	private async Task DeleteCDC(string code)
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<string>(code, new JsonMediaTypeFormatter());
		httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "CashDiscount", "DeleteDoc"));
		httpRequestMessage.Method = HttpMethod.Post;
		using HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
		if (response.IsSuccessStatusCode)
		{
			string text = await response.Content.ReadAsStringAsync();
			if (text == "null")
			{
				WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeShort, "");
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Warning, Msg_Time._TimeShort, text);
			}
		}
	}

	private async Task DeleteOCDC(string code)
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<string>(code, new JsonMediaTypeFormatter());
		httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "OCashDiscount", "DeleteDoc"));
		httpRequestMessage.Method = HttpMethod.Post;
		using HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
		if (response.IsSuccessStatusCode)
		{
			string text = await response.Content.ReadAsStringAsync();
			if (text == "null")
			{
				WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeShort, "");
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Warning, Msg_Time._TimeShort, text);
			}
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_CashDiscountEdit));
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.LineId = new Apzon.Controls.Grid.GridColumnEx();
		this.NumOfDays = new Apzon.Controls.Grid.GridColumnEx();
		this.Discount = new Apzon.Controls.Grid.GridColumnEx();
		this.lb_Code = new Apzon.Controls.Controls.LabelLine();
		this.lb_Name = new Apzon.Controls.Controls.LabelLine();
		this.txtCode = new Apzon.Controls.Editor.TextEditEx();
		this.txtDesc = new Apzon.Controls.Editor.TextEditEx();
		this.btDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDesc.Properties).BeginInit();
		base.SuspendLayout();
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(6, 326);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(90, 21);
		this.btOK.TabIndex = 0;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(197, 326);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(96, 21);
		this.btCancel.TabIndex = 2;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(6, 67);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(518, 253);
		this.grdDoc.TabIndex = 3;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.LineId, this.NumOfDays, this.Discount });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
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
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.LineId.Caption = "Row no";
		this.LineId.CurrencyDateField = null;
		this.LineId.CurrencyField = null;
		this.LineId.CurrencyFixed = null;
		this.LineId.CurrencyFixedRate = 0.0;
		this.LineId.CurrencyRateField = null;
		this.LineId.FieldName = "LineId";
		this.LineId.Name = "LineId";
		this.LineId.OptionsColumn.AllowEdit = false;
		this.LineId.PrimaryKey = true;
		this.LineId.UnitField = null;
		this.LineId.Visible = true;
		this.LineId.VisibleIndex = 0;
		this.NumOfDays.Caption = "Days After Posting date";
		this.NumOfDays.CurrencyDateField = null;
		this.NumOfDays.CurrencyField = null;
		this.NumOfDays.CurrencyFixed = null;
		this.NumOfDays.CurrencyFixedRate = 0.0;
		this.NumOfDays.CurrencyRateField = null;
		this.NumOfDays.FieldName = "NumOfDays";
		this.NumOfDays.Name = "NumOfDays";
		this.NumOfDays.PrimaryKey = false;
		this.NumOfDays.UnitField = null;
		this.NumOfDays.Visible = true;
		this.NumOfDays.VisibleIndex = 1;
		this.Discount.Caption = "Discount %";
		this.Discount.CurrencyDateField = null;
		this.Discount.CurrencyField = null;
		this.Discount.CurrencyFixed = null;
		this.Discount.CurrencyFixedRate = 0.0;
		this.Discount.CurrencyRateField = null;
		this.Discount.FieldName = "Discount";
		this.Discount.Name = "Discount";
		this.Discount.PrimaryKey = false;
		this.Discount.UnitField = null;
		this.Discount.Visible = true;
		this.Discount.VisibleIndex = 2;
		this.lb_Code.EditControl = null;
		this.lb_Code.EditValue = "Code";
		this.lb_Code.FieldName = null;
		this.lb_Code.HideWhenEmpty = false;
		this.lb_Code.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Code.Location = new System.Drawing.Point(6, 9);
		this.lb_Code.Name = "lb_Code";
		this.lb_Code.ReadOnly = false;
		this.lb_Code.SettingInVisible = false;
		this.lb_Code.SettingReadonly = false;
		this.lb_Code.ShowUnderLine = true;
		this.lb_Code.Size = new System.Drawing.Size(58, 20);
		this.lb_Code.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Code.TabIndex = 4;
		this.lb_Code.Utext = "Code";
		this.lb_Name.EditControl = null;
		this.lb_Name.EditValue = "Name";
		this.lb_Name.FieldName = null;
		this.lb_Name.HideWhenEmpty = false;
		this.lb_Name.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Name.Location = new System.Drawing.Point(6, 35);
		this.lb_Name.Name = "lb_Name";
		this.lb_Name.ReadOnly = false;
		this.lb_Name.SettingInVisible = false;
		this.lb_Name.SettingReadonly = false;
		this.lb_Name.ShowUnderLine = true;
		this.lb_Name.Size = new System.Drawing.Size(58, 20);
		this.lb_Name.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Name.TabIndex = 5;
		this.lb_Name.Utext = "Name";
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(68, 6);
		this.txtCode.Name = "txtCode";
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(244, 20);
		this.txtCode.TabIndex = 6;
		this.txtCode.TextChanged += new System.EventHandler(this.txtCode_TextChanged);
		this.txtDesc.AllowDrop = true;
		this.txtDesc.EditControl = null;
		this.txtDesc.FieldName = null;
		this.txtDesc.IsSearchControl = false;
		this.txtDesc.Location = new System.Drawing.Point(68, 36);
		this.txtDesc.Name = "txtDesc";
		this.txtDesc.SettingInVisible = false;
		this.txtDesc.SettingReadonly = false;
		this.txtDesc.Size = new System.Drawing.Size(244, 20);
		this.txtDesc.TabIndex = 7;
		this.txtDesc.EditValueChanged += new System.EventHandler(this.txtDesc_EditValueChanged);
		this.txtDesc.TextChanged += new System.EventHandler(this.txtDesc_TextChanged);
		this.btDelete.EditControl = null;
		this.btDelete.EditValue = "Delete";
		this.btDelete.FieldName = null;
		this.btDelete.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btDelete.ImageOptions.Image");
		this.btDelete.IsSearchControl = false;
		this.btDelete.Location = new System.Drawing.Point(98, 326);
		this.btDelete.Name = "btDelete";
		this.btDelete.ReadOnly = false;
		this.btDelete.SettingInVisible = false;
		this.btDelete.SettingReadonly = false;
		this.btDelete.Size = new System.Drawing.Size(96, 21);
		this.btDelete.TabIndex = 8;
		this.btDelete.Text = "Delete";
		this.btDelete.Click += new System.EventHandler(this.simpleButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(530, 349);
		base.Controls.Add(this.btDelete);
		base.Controls.Add(this.txtDesc);
		base.Controls.Add(this.txtCode);
		base.Controls.Add(this.lb_Name);
		base.Controls.Add(this.lb_Code);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.Name = "frm_CashDiscountEdit";
		this.Text = "Cash Discount - Edit";
		base.Load += new System.EventHandler(this.frm_CashDiscountAddNew_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDesc.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
