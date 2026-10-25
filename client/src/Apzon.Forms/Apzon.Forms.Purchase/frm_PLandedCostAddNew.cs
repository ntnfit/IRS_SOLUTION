using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Purchase;

public class frm_PLandedCostAddNew : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public delegate void CloseEvent();

	public ShowForms frmShowChildrenForm;

	private List<Document_LinesImpl> lst;

	private int newDocEntry;

	public CloseEvent reloadParent;

	private object[] arrAllocation = new object[6] { "F \t\t - \t\t Cash Value Before Custom", "C \t\t - \t\t Cash Value After Custom", "Q \t\t - \t\t Quantity", "V \t\t - \t\t Weight", "A \t\t - \t\t Volume", "W \t\t - \t\t Equal" };

	private int hotTrackRow = int.MinValue;

	private IContainer components;

	private SimpleButtonEx btCancel;

	private PanelControl panelControl1;

	private PanelControl panelControl4;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx AlcCode;

	private GridColumnEx AlcName;

	private GridColumnEx AcctCode;

	private PanelControl panelControl3;

	private SimpleButtonEx btOK;

	private GridViewEx tblViewManufacturer;

	private GridColumnEx STT;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx OhType;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx LaCAllcAcc;

	private int HotTrackRow
	{
		get
		{
			return hotTrackRow;
		}
		set
		{
			if (hotTrackRow != value)
			{
				int rowHandle = hotTrackRow;
				hotTrackRow = value;
				if (gvDoc.ActiveEditor != null)
				{
					gvDoc.PostEditor();
				}
				gvDoc.RefreshRow(rowHandle);
				gvDoc.RefreshRow(hotTrackRow);
				if (hotTrackRow >= 0)
				{
					grdDoc.Cursor = Cursors.Hand;
				}
				else
				{
					grdDoc.Cursor = Cursors.Default;
				}
			}
		}
	}

	public frm_PLandedCostAddNew()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PLandedCostAddNew"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_PLandedCostAddNew" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

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
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	private void frm_BankAccount_Load(object sender, EventArgs e)
	{
		RefreshData();
	}

	private void RefreshData()
	{
		LoadData();
		getNewDocEntry();
	}

	private async void getNewDocEntry()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LandedCost");
			await _help.GetIntAsync("GetMaxDocEntryCostCode");
			_ = newDocEntry;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadData()
	{
		try
		{
			using HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost");
			List<Document_LinesImpl> list = await _help.GetListAsync("GetLandedCostCode");
			if (list != null && list.Count > 0)
			{
				grdDoc.DataSource = list;
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string code = Convert.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "AlcCode"));
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

	private async void DeleteDoc(string code)
	{
		using HttpClient client = new HttpClient();
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<string>(code, new JsonMediaTypeFormatter());
		httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "LandedCost", "DeleteDocCode"));
		httpRequestMessage.Method = HttpMethod.Post;
		using HttpResponseMessage response = await client.SendAsync(httpRequestMessage);
		if (response.IsSuccessStatusCode)
		{
			string text = await response.Content.ReadAsStringAsync();
			if (text == "null")
			{
				WinCommons.writeStatus(Enums.MessageCode.RemoveSuccess, Enums.MessageType.Success, Msg_Time._TimeShort, "");
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
		try
		{
			List<Document_LinesImpl> lstNew = new List<Document_LinesImpl>();
			if (gvDoc.DataSource != null)
			{
				lstNew = gvDoc.DataSource as List<Document_LinesImpl>;
				if (lstNew.Where((Document_LinesImpl x) => x.AlcName.Trim() == "").ToList().Count == 0)
				{
					Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
					int num = ((lstNew.Count == 0) ? 1 : (lstNew.Max((Document_LinesImpl x) => int.Parse(x.AlcCode)) + 1));
					newDocEntry = ((newDocEntry > num) ? newDocEntry : num);
					document_LinesImpl.AlcCode = newDocEntry.ToString();
					document_LinesImpl.AlcName = string.Empty;
					document_LinesImpl.OhType = string.Empty;
					document_LinesImpl.AcctCode = string.Empty;
					document_LinesImpl.LaCAllcAcc = string.Empty;
					lstNew.Add(document_LinesImpl);
				}
			}
			else
			{
				Document_LinesImpl document_LinesImpl2 = new Document_LinesImpl();
				int num2 = ((lstNew.Count == 0) ? 1 : (lstNew.Max((Document_LinesImpl x) => int.Parse(x.AlcCode)) + 1));
				newDocEntry = ((newDocEntry > num2) ? newDocEntry : num2);
				document_LinesImpl2.AlcCode = newDocEntry.ToString();
				document_LinesImpl2.AlcName = string.Empty;
				document_LinesImpl2.OhType = string.Empty;
				document_LinesImpl2.AcctCode = string.Empty;
				document_LinesImpl2.LaCAllcAcc = string.Empty;
				lstNew.Add(document_LinesImpl2);
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
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CellValueChanged_1(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == AlcCode || e.Column == AlcName || e.Column == OhType || e.Column == AcctCode)
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
			List<Document_LinesImpl> source = (List<Document_LinesImpl>)grdDoc.DataSource;
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			source = source.Where((Document_LinesImpl x) => x.AlcName.Trim() != "").ToList();
			foreach (Document_LinesImpl item in source)
			{
				if (string.IsNullOrEmpty(item.OhType))
				{
					item.OhType = "F";
				}
				else if (item.OhType.Length > 1)
				{
					item.OhType = item.OhType.Remove(1, item.OhType.Length - 1);
				}
				await Save(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> Save(Document_LinesImpl doc)
	{
		string result = string.Empty;
		try
		{
			using (HttpClientHelper<Document_LinesImpl> _help = new HttpClientHelper<Document_LinesImpl>(Configuration.ApiServer, "LandedCost"))
			{
				result = await _help.GetStringAsync("SaveDocCode", doc);
				if (string.IsNullOrEmpty(result))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					RefreshData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, result);
				}
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return result;
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_PLandedCostAddNew_FormClosing(object sender, FormClosingEventArgs e)
	{
		reloadParent();
	}

	private void gvDoc_RowClick(object sender, RowClickEventArgs e)
	{
		ApzonCommon.SetColorRowSelected(ref gvDoc);
	}

	private void gvDoc_MouseMove(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = (sender as GridView).CalcHitInfo(new Point(e.X, e.Y));
		if (gridHitInfo.InRowCell)
		{
			HotTrackRow = gridHitInfo.RowHandle;
		}
		else
		{
			HotTrackRow = int.MinValue;
		}
	}

	private void gvDoc_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (e.RowHandle == HotTrackRow)
		{
			e.Appearance.BackColor = ApzonCommon.colorFocusRow;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Purchase.frm_PLandedCostAddNew));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.AlcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.AlcName = new Apzon.Controls.Grid.GridColumnEx();
		this.OhType = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.AcctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.LaCAllcAcc = new Apzon.Controls.Grid.GridColumnEx();
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
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(107, 9);
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
		this.panelControl1.Size = new System.Drawing.Size(601, 385);
		this.panelControl1.TabIndex = 4;
		this.panelControl4.Controls.Add(this.grdDoc);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(2, 2);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(597, 346);
		this.panelControl4.TabIndex = 2;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(2, 2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemCheckEdit1, this.repositoryItemComboBox1 });
		this.grdDoc.Size = new System.Drawing.Size(593, 342);
		this.grdDoc.TabIndex = 3;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.AlcCode, this.AlcName, this.OhType, this.AcctCode, this.LaCAllcAcc });
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
		this.gvDoc.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvDoc_RowClick);
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged_1);
		this.gvDoc.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gvDoc_MouseMove);
		this.gvDoc.MouseLeave += new System.EventHandler(this.gvDoc_MouseLeave);
		this.AlcCode.Caption = "Code";
		this.AlcCode.CurrencyDateField = null;
		this.AlcCode.CurrencyField = null;
		this.AlcCode.CurrencyFixed = null;
		this.AlcCode.CurrencyFixedRate = 0.0;
		this.AlcCode.CurrencyRateField = null;
		this.AlcCode.FieldName = "AlcCode";
		this.AlcCode.Name = "AlcCode";
		this.AlcCode.OptionsColumn.AllowEdit = false;
		this.AlcCode.OptionsColumn.ReadOnly = true;
		this.AlcCode.PrimaryKey = true;
		this.AlcCode.UnitField = null;
		this.AlcCode.Visible = true;
		this.AlcCode.VisibleIndex = 0;
		this.AlcCode.Width = 98;
		this.AlcName.Caption = "Name";
		this.AlcName.CurrencyDateField = null;
		this.AlcName.CurrencyField = null;
		this.AlcName.CurrencyFixed = null;
		this.AlcName.CurrencyFixedRate = 0.0;
		this.AlcName.CurrencyRateField = null;
		this.AlcName.FieldName = "AlcName";
		this.AlcName.Name = "AlcName";
		this.AlcName.PrimaryKey = false;
		this.AlcName.UnitField = null;
		this.AlcName.Visible = true;
		this.AlcName.VisibleIndex = 1;
		this.AlcName.Width = 222;
		this.OhType.Caption = "Allocation By";
		this.OhType.ColumnEdit = this.repositoryItemComboBox1;
		this.OhType.CurrencyDateField = null;
		this.OhType.CurrencyField = null;
		this.OhType.CurrencyFixed = null;
		this.OhType.CurrencyFixedRate = 0.0;
		this.OhType.CurrencyRateField = null;
		this.OhType.FieldName = "OhType";
		this.OhType.Name = "OhType";
		this.OhType.PrimaryKey = false;
		this.OhType.UnitField = null;
		this.OhType.Visible = true;
		this.OhType.VisibleIndex = 2;
		this.OhType.Width = 118;
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Items.AddRange(new object[6] { "F \t\t - \t\t Cash Value Before Custom", "C \t\t - \t\t Cash Value After Custom", "Q \t\t - \t\t Quantity", "V \t\t - \t\t Weight", "A \t\t - \t\t Volume", "W \t\t - \t\t Equal" });
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.AcctCode.Caption = "Account Code";
		this.AcctCode.CurrencyDateField = null;
		this.AcctCode.CurrencyField = null;
		this.AcctCode.CurrencyFixed = null;
		this.AcctCode.CurrencyFixedRate = 0.0;
		this.AcctCode.CurrencyRateField = null;
		this.AcctCode.FieldName = "AcctCode";
		this.AcctCode.Name = "AcctCode";
		this.AcctCode.PrimaryKey = false;
		this.AcctCode.UnitField = null;
		this.AcctCode.Visible = true;
		this.AcctCode.VisibleIndex = 3;
		this.AcctCode.Width = 123;
		this.LaCAllcAcc.Caption = "Landed Costs Alloc. Account";
		this.LaCAllcAcc.CurrencyDateField = null;
		this.LaCAllcAcc.CurrencyField = null;
		this.LaCAllcAcc.CurrencyFixed = null;
		this.LaCAllcAcc.CurrencyFixedRate = 0.0;
		this.LaCAllcAcc.CurrencyRateField = null;
		this.LaCAllcAcc.FieldName = "LaCAllcAcc";
		this.LaCAllcAcc.Name = "LaCAllcAcc";
		this.LaCAllcAcc.PrimaryKey = false;
		this.LaCAllcAcc.UnitField = null;
		this.LaCAllcAcc.Visible = true;
		this.LaCAllcAcc.VisibleIndex = 4;
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
		this.panelControl3.Size = new System.Drawing.Size(597, 35);
		this.panelControl3.TabIndex = 1;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(5, 9);
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
		base.ClientSize = new System.Drawing.Size(601, 385);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PLandedCostAddNew";
		this.Text = "Landed Cost - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PLandedCostAddNew_FormClosing);
		base.Load += new System.EventHandler(this.frm_BankAccount_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tblViewManufacturer).EndInit();
		base.ResumeLayout(false);
	}
}
