using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
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
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.integrated;

public class frm_Integrate_BusinessPartner : XtraFormEx
{
	private string _privilege = "";

	private string _formController = "IntegratedBP";

	public string _isIns = "N";

	public ApzObjectTypes _formObjType = ApzObjectTypes.oBusinessPartners;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridControlEx grd_Contacts;

	private SimpleButtonEx btn_Search;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private System.Windows.Forms.ProgressBar progressBar1;

	private Label lbl_Progress;

	private GridViewEx gv_Doc;

	private GridViewEx gv_Contacts;

	private ComboBoxEditEx cbType;

	private LabelLine lblType;

	private LabelLine labelLine3;

	private TextEditEx txt_CardName;

	private RadioGroupEx radioGroupStatus;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Addresses;

	private GridViewEx gv_Addresses;

	private TextEditEx txt_CardCode;

	private LabelLine labelLine1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		default:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Integrate_BusinessPartner()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Integrate_BusinessPartner"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Integrate_BusinessPartner" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			FormMode = Enums.FormViewModeType.Add;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_Integrate_BusinessPartner_Load(object sender, EventArgs e)
	{
		try
		{
			lbl_Progress.Text = "";
			lbl_Progress.Visible = false;
			progressBar1.Visible = false;
			cbType.SelectedText = "All";
			radioGroupStatus.SelectedIndex = 0;
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		EnableDisableButton();
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				await Save();
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.BpAlreadyImported, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = ((radioGroupStatus.SelectedIndex != 0) ? Enums.FormViewModeType.Find : Enums.FormViewModeType.Add);
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && keyData == Keys.Return && (txt_CardCode.ContainsFocus || txt_CardName.ContainsFocus || radioGroupStatus.ContainsFocus || cbType.CanFocus))
		{
			btn_Search_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private async Task<bool> CheckExists(DataTable dt)
	{
		bool ret = false;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = await hp.PostAsync("CheckExists", dt);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret = Function.ParseInt(dataTable.Rows[0][0]) > 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return ret;
	}

	private async Task Search()
	{
		EnableDisableButton();
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("CardCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardType", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsImported", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["IsImported"] = ((radioGroupStatus.SelectedIndex == 0) ? "O" : "C");
			switch (((cbType.SelectedText == null || string.IsNullOrWhiteSpace(cbType.SelectedText)) ? "ALL" : Function.ToString(cbType.SelectedText).ToUpper()).ToUpper())
			{
			case "ALL":
				dataRow["CardType"] = "";
				break;
			case "CUSTOMER":
				dataRow["CardType"] = "C";
				break;
			case "VENDOR":
				dataRow["CardType"] = "S";
				break;
			default:
				dataRow["CardType"] = "S";
				break;
			}
			dataRow["CardCode"] = Function.ToString(txt_CardCode.Text).Trim();
			dataRow["CardName"] = Function.ToString(txt_CardName.Text).Trim();
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet ds = await hp.PostAsync("Search", dataTable);
			if (ds != null && ds.Tables.Count == 2 && ds.Tables[0] != null && ds.Tables[1] != null)
			{
				await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds.Tables[0], ds.Tables[1]);
				if (ds.Tables[1].Rows.Count == 0)
				{
					grd_Addresses.DataSource = null;
					grd_Addresses.RefreshDataSource();
					grd_Contacts.DataSource = null;
					grd_Contacts.RefreshDataSource();
				}
				else if (ds.Tables[1].Rows.Count == 1)
				{
					gv_Doc_FocusedRowObjectChanged(null, null);
				}
				FormMode = Enums.FormViewModeType.Add;
			}
			else
			{
				grd_Addresses.DataSource = null;
				grd_Addresses.RefreshDataSource();
				grd_Contacts.DataSource = null;
				grd_Contacts.RefreshDataSource();
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				gv_Doc.PopulateColumns();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			DataTable dtCheck = new DataTable
			{
				Columns = { 
				{
					"CardCode",
					typeof(string)
				} }
			};
			EnableDisableButton();
			lbl_Progress.Text = "";
			lbl_Progress.Visible = true;
			progressBar1.Visible = true;
			DataTable dtsDoc = grd_Doc.DataSource as DataTable;
			int[] arrSel = gv_Doc.GetSelectedRows();
			int totalLine = 0;
			int success = 0;
			if (arrSel != null)
			{
				totalLine = arrSel.Length;
				progressBar1.Maximum = totalLine + 1;
				progressBar1.Minimum = 1;
				progressBar1.Step = 1;
				progressBar1.Value = 1;
			}
			for (int i = 0; i < arrSel.Length; i++)
			{
				DataRow dataRow = dtsDoc.Rows[gv_Doc.GetDataSourceRowIndex(arrSel[i])];
				if (dataRow == null)
				{
					continue;
				}
				BPImpls doc = DataRowToBPImpls(dataRow);
				if (grd_Contacts.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					List<ContactPersonImpl> list = new List<ContactPersonImpl>();
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						ContactPersonImpl contactPersonImpl = new ContactPersonImpl();
						contactPersonImpl.Name = Function.ToString(dataTable.Rows[j]["Name"]);
						contactPersonImpl.Address = Function.ToString(dataTable.Rows[j]["Address"]);
						contactPersonImpl.Tel1 = Function.ToString(dataTable.Rows[j]["Tel1"]);
						contactPersonImpl.Cellolar = Function.ToString(dataTable.Rows[j]["Cellolar"]);
						contactPersonImpl.E_MailL = Function.ToString(dataTable.Rows[j]["E_MailL"]);
						list.Add(contactPersonImpl);
					}
					doc.ListContactPerson = list;
				}
				if (grd_Addresses.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
				{
					List<AddressesImpl> list2 = new List<AddressesImpl>();
					for (int k = 0; k < dataTable2.Rows.Count; k++)
					{
						AddressesImpl addressesImpl = new AddressesImpl();
						addressesImpl.Address = Function.ToString(dataTable2.Rows[k]["Address"]);
						addressesImpl.AdresType = Function.ToString(dataTable2.Rows[k]["AdresType"]);
						list2.Add(addressesImpl);
					}
					doc.Addresses = list2;
				}
				dtCheck.Rows.Clear();
				dtCheck.AcceptChanges();
				DataRow dataRow2 = dtCheck.NewRow();
				dataRow2["CardCode"] = doc.CardCode;
				dtCheck.Rows.Add(dataRow2);
				dtCheck.AcceptChanges();
				if (await CheckExists(dtCheck))
				{
					gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "This documents exists!");
					continue;
				}
				string text = await SaveADoc(doc);
				if (string.IsNullOrWhiteSpace(text))
				{
					success++;
					WriteLogFile("[SUCCESS] " + _formObjType);
					gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Success");
				}
				else
				{
					WriteLogFile("[ERROR] " + _formObjType.ToString() + ": " + text);
					gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Error: " + text.ToString());
				}
				progressBar1.PerformStep();
				lbl_Progress.Text = i + 1 + " of " + totalLine;
			}
			lbl_Progress.Text = lbl_Progress.Text + ", " + success + " success.";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveADoc(BPImpls doc)
	{
		string result = "";
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			result = await hp.PostAsync("SaveADoc", doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private async Task<DataSet> GetContactEmps(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("GetContactEmps", new { cardCode });
			if (dataSet != null)
			{
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private async Task<DataSet> GetAddresses(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("GetAddresses", new { cardCode });
			if (dataSet != null)
			{
				return dataSet;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataSet();
	}

	private void WriteLogFile(string errorMsg)
	{
		try
		{
			DateTime now = DateTime.Now;
			string text = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log\\" + now.ToString("yyyy") + "\\" + now.ToString("MM"));
			DirectoryInfo directoryInfo = new DirectoryInfo(text);
			if (!directoryInfo.Exists)
			{
				directoryInfo.Create();
			}
			using StreamWriter streamWriter = new StreamWriter(text + "\\Intergrated-log-" + now.ToString("yyyy_MM_dd") + ".txt", append: true);
			streamWriter.WriteLine(errorMsg);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableButton(bool enable = false)
	{
		try
		{
			if (enable)
			{
				radioGroupStatus.Enabled = true;
				btn_Search.Enabled = true;
				btn_OK.Enabled = true;
				btn_Cancel.Enabled = true;
			}
			else
			{
				radioGroupStatus.Enabled = false;
				btn_Search.Enabled = false;
				btn_OK.Enabled = false;
				btn_Cancel.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private BPImpls DataRowToBPImpls(DataRow dr)
	{
		BPImpls bPImpls = new BPImpls();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(BPImpls).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (list.Contains(propertyInfo.Name))
				{
					try
					{
						PropertyInfo property = bPImpls.GetType().GetProperty(propertyInfo.Name);
						propertyInfo.SetValue(bPImpls, (dr[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(dr[propertyInfo.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			WinCommons.ErrorLineNumber(ex2);
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return bPImpls;
	}

	private ItemsImpl DataRowToItemsImpl(DataRow dr)
	{
		ItemsImpl itemsImpl = new ItemsImpl();
		try
		{
			List<string> list = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(ItemsImpl).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (list.Contains(propertyInfo.Name))
				{
					try
					{
						PropertyInfo property = itemsImpl.GetType().GetProperty(propertyInfo.Name);
						propertyInfo.SetValue(itemsImpl, (dr[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(dr[propertyInfo.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
		}
		catch (Exception ex2)
		{
			WinCommons.ErrorLineNumber(ex2);
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
		}
		return itemsImpl;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.integrated.frm_Integrate_BusinessPartner));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Contacts = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Contacts = new Apzon.Controls.Grid.GridViewEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Addresses = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Addresses = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.lbl_Progress = new System.Windows.Forms.Label();
		this.cbType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lblType = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_CardName = new Apzon.Controls.Editor.TextEditEx();
		this.radioGroupStatus = new Apzon.Controls.Editor.RadioGroupEx();
		this.txt_CardCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Contacts).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contacts).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Addresses).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Addresses).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 36);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(882, 465);
		this.splitContainerControl1.SplitterPosition = 191;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(882, 191);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oItegratedBp";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OCRD";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(882, 264);
		this.xtraTabControl1.TabIndex = 3;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.grd_Contacts);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Contact Employees";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(880, 239);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Contact Employees";
		this.grd_Contacts.AllowPasteExcel = true;
		this.grd_Contacts.ClipboardData = null;
		this.grd_Contacts.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Contacts.Location = new System.Drawing.Point(0, 0);
		this.grd_Contacts.MainView = this.gv_Contacts;
		this.grd_Contacts.Name = "grd_Contacts";
		this.grd_Contacts.Size = new System.Drawing.Size(880, 239);
		this.grd_Contacts.TabIndex = 1;
		this.grd_Contacts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Contacts });
		this.gv_Contacts.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Contacts.ColumnKey = null;
		this.gv_Contacts.DisableFormSetting = false;
		this.gv_Contacts.DisplayException = false;
		this.gv_Contacts.EditColumn = null;
		this.gv_Contacts.FieldCurrency = null;
		this.gv_Contacts.FieldRate = null;
		this.gv_Contacts.FormId = "oItegratedBp";
		this.gv_Contacts.GridControl = this.grd_Contacts;
		this.gv_Contacts.IgnoreFilter = false;
		this.gv_Contacts.IndexEditRow = -1;
		this.gv_Contacts.IndicatorWidth = 35;
		this.gv_Contacts.IsApzTable = false;
		this.gv_Contacts.IsInsertNewRow = false;
		this.gv_Contacts.IsLoadingData = false;
		this.gv_Contacts.IsMappingContextAction = false;
		this.gv_Contacts.ItemId = "OCTR";
		this.gv_Contacts.LstColumnAllowBatch = null;
		this.gv_Contacts.LstColumnNotAllowBatch = null;
		this.gv_Contacts.LstColumnNotEditable = null;
		this.gv_Contacts.LstColumNotSetting = null;
		this.gv_Contacts.Name = "gv_Contacts";
		this.gv_Contacts.UseBatchMod = false;
		this.xtraTabPage2.Controls.Add(this.grd_Addresses);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Addresses";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(880, 239);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Addresses";
		this.grd_Addresses.AllowPasteExcel = false;
		this.grd_Addresses.ClipboardData = null;
		this.grd_Addresses.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Addresses.Location = new System.Drawing.Point(0, 0);
		this.grd_Addresses.MainView = this.gv_Addresses;
		this.grd_Addresses.Name = "grd_Addresses";
		this.grd_Addresses.Size = new System.Drawing.Size(880, 239);
		this.grd_Addresses.TabIndex = 2;
		this.grd_Addresses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Addresses });
		this.gv_Addresses.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Addresses.ColumnKey = null;
		this.gv_Addresses.DisableFormSetting = false;
		this.gv_Addresses.DisplayException = false;
		this.gv_Addresses.EditColumn = null;
		this.gv_Addresses.FieldCurrency = null;
		this.gv_Addresses.FieldRate = null;
		this.gv_Addresses.FormId = "oItegratedBp";
		this.gv_Addresses.GridControl = this.grd_Addresses;
		this.gv_Addresses.IgnoreFilter = false;
		this.gv_Addresses.IndexEditRow = -1;
		this.gv_Addresses.IndicatorWidth = 35;
		this.gv_Addresses.IsApzTable = false;
		this.gv_Addresses.IsInsertNewRow = false;
		this.gv_Addresses.IsLoadingData = false;
		this.gv_Addresses.IsMappingContextAction = false;
		this.gv_Addresses.ItemId = "OADR";
		this.gv_Addresses.LstColumnAllowBatch = null;
		this.gv_Addresses.LstColumnNotAllowBatch = null;
		this.gv_Addresses.LstColumnNotEditable = null;
		this.gv_Addresses.LstColumNotSetting = null;
		this.gv_Addresses.Name = "gv_Addresses";
		this.gv_Addresses.UseBatchMod = false;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(800, 5);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(68, 20);
		this.btn_Search.TabIndex = 5;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 510);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 6;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 510);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 7;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressBar1.Location = new System.Drawing.Point(480, 510);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(213, 20);
		this.progressBar1.TabIndex = 8;
		this.lbl_Progress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Progress.AutoSize = true;
		this.lbl_Progress.Location = new System.Drawing.Point(700, 513);
		this.lbl_Progress.Name = "lbl_Progress";
		this.lbl_Progress.Size = new System.Drawing.Size(35, 13);
		this.lbl_Progress.TabIndex = 9;
		this.lbl_Progress.Text = "label1";
		this.cbType.EditControl = null;
		this.cbType.FieldName = null;
		this.cbType.IsSearchControl = false;
		this.cbType.Location = new System.Drawing.Point(554, 4);
		this.cbType.Name = "cbType";
		this.cbType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbType.Properties.Items.AddRange(new object[3] { "All", "Customer", "Vendor" });
		this.cbType.SettingInVisible = false;
		this.cbType.SettingReadonly = false;
		this.cbType.Size = new System.Drawing.Size(100, 20);
		this.cbType.TabIndex = 10;
		this.lblType.EditControl = null;
		this.lblType.EditValue = "Type";
		this.lblType.FieldName = null;
		this.lblType.HideWhenEmpty = false;
		this.lblType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblType.Location = new System.Drawing.Point(478, 7);
		this.lblType.Name = "lblType";
		this.lblType.ReadOnly = false;
		this.lblType.SettingInVisible = false;
		this.lblType.SettingReadonly = false;
		this.lblType.ShowUnderLine = true;
		this.lblType.Size = new System.Drawing.Size(80, 20);
		this.lblType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblType.TabIndex = 11;
		this.lblType.Utext = "Type";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "BP Name";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(182, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(67, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 13;
		this.labelLine3.Utext = "BP Name";
		this.txt_CardName.EditControl = null;
		this.txt_CardName.FieldName = null;
		this.txt_CardName.IsSearchControl = false;
		this.txt_CardName.Location = new System.Drawing.Point(248, 5);
		this.txt_CardName.Name = "txt_CardName";
		this.txt_CardName.SettingInVisible = false;
		this.txt_CardName.SettingReadonly = false;
		this.txt_CardName.Size = new System.Drawing.Size(224, 20);
		this.txt_CardName.TabIndex = 14;
		this.radioGroupStatus.EditControl = null;
		this.radioGroupStatus.FieldName = null;
		this.radioGroupStatus.IsSearchControl = false;
		this.radioGroupStatus.Location = new System.Drawing.Point(673, 2);
		this.radioGroupStatus.Name = "radioGroupStatus";
		this.radioGroupStatus.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroupStatus.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroupStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroupStatus.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "Open"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("C", "Close")
		});
		this.radioGroupStatus.SettingInVisible = false;
		this.radioGroupStatus.SettingReadonly = false;
		this.radioGroupStatus.Size = new System.Drawing.Size(121, 26);
		this.radioGroupStatus.TabIndex = 15;
		this.txt_CardCode.EditControl = null;
		this.txt_CardCode.FieldName = null;
		this.txt_CardCode.IsSearchControl = false;
		this.txt_CardCode.Location = new System.Drawing.Point(80, 5);
		this.txt_CardCode.Name = "txt_CardCode";
		this.txt_CardCode.SettingInVisible = false;
		this.txt_CardCode.SettingReadonly = false;
		this.txt_CardCode.Size = new System.Drawing.Size(95, 20);
		this.txt_CardCode.TabIndex = 17;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(17, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(67, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 16;
		this.labelLine1.Utext = "BP Code";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(885, 536);
		base.Controls.Add(this.txt_CardCode);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.radioGroupStatus);
		base.Controls.Add(this.txt_CardName);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.cbType);
		base.Controls.Add(this.lblType);
		base.Controls.Add(this.lbl_Progress);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oItegratedBp";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Integrate_BusinessPartner.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Integrate_BusinessPartner";
		this.Text = "Document";
		base.Load += new System.EventHandler(this.frm_Integrate_BusinessPartner_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Contacts).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Contacts).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Addresses).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Addresses).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CardCode.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
