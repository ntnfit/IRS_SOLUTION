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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_Solution : XtraFormEx
{
	private bool _isLoading;

	public bool OkToClose;

	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	public int _sltCode;

	public Frm_ServiceCall frmSCall;

	public bool newLine;

	public bool _preAdd;

	private bool _isCancel;

	private bool _forceClose;

	private string Controller = "Solution";

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution");

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SimpleButtonEx btn_Add;

	private SimpleButtonEx btn_Cancel;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdDoc;

	public GridViewEx gvDoc;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gcUpdateOn;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private SimpleButtonEx btn_Search;

	private LabelLine labelLine1;

	private ButtonEditEx btn_ItemCode;

	private LabelLine labelLine2;

	private LookUpEditEx txt_Status;

	private LabelLine labelLine3;

	private TextEditEx txt_sltCode;

	private GridColumnEx gridColumn14;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ContextMenuStrip ctx_Line;

	private ToolStripMenuItem addNewLineToolStripMenuItem;

	private RepositoryItemButtonEditCFL rep_Item;

	private RepositoryItemLookUpEdit rep_Updateby;

	private RepositoryItemDateEdit rep_date;

	private RepositoryItemLookUpEdit rep_Status;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private XtraTabControlEx xtraTabControl1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gridColumn16;

	private XtraTabPageEx xtraTabPage1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private ucAttachment ucAttachment1;

	public int sltCode { get; set; }

	public string ItemCode { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Add.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				btn_Add.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btn_Add.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_Solution()
	{
		InitializeComponent();
		OnLoadAsync += frm_Solution_Load;
		FormId = "oSolutions";
		gvDoc.ItemId = "OSLT";
		gvDoc.FormId = FormId;
	}

	private async Task frm_Solution_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			gcUpdateOn.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_date.Mask.EditMask = GlobalData.DateTimeFormat;
			ucAttachment1.gcDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			await LoadInItForm();
			frm_Solution frm_Solution2 = this;
			GridViewEx gridViewEx = gvDoc;
			string text = (ucAttachment1.gv_File.FormId = "oSolution");
			string formId = (gridViewEx.FormId = text);
			frm_Solution2.FormId = formId;
			gvDoc.ItemId = "OSLT";
			ucAttachment1.gv_File.ItemId = "SLT1";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void btn_Add_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					btn_Add.Enabled = false;
					btn_Cancel.Enabled = false;
					AddSolution();
					btn_Add.Enabled = true;
					btn_Cancel.Enabled = true;
				}
				break;
			case Enums.FormViewModeType.Update:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					btn_Add.Enabled = false;
					btn_Cancel.Enabled = false;
					UpdateSolution();
					btn_Add.Enabled = true;
					btn_Cancel.Enabled = true;
				}
				break;
			case Enums.FormViewModeType.View:
				OkToClose = true;
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ItemCodeFrom_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_CFL2.ChooseItems.Count > 0)
			{
				DataRow dataRow = frm_CFL2.ChooseItems[0];
				if (buttonEdit != null)
				{
					buttonEdit.EditValue = dataRow["ItemCode"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int absEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry"));
			_sltCode = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SltCode"));
			ucAttachment1.LoadAttachments(absEntry);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_preAdd && FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					e.Allow = false;
				}
				else
				{
					grdDoc.DataSourceTable.RejectChanges();
				}
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
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Item_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				if (sltCode > 0)
				{
					txt_sltCode.EditValue = sltCode;
				}
				DataTable dataTable = await Search(Function.ParseInt(txt_sltCode.EditValue), Function.ToString(btn_ItemCode.EditValue), Function.ParseInt(txt_Status.EditValue));
				sltCode = 0;
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.GetSelectedRows() == null || gvDoc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SltCode"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByNo, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, Function.ToString(num)) == DialogResult.Yes && num > 0)
			{
				string text = await _httpClient.Solution.DeleteDocs(num);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
					btn_Search_Click(null, null);
					ucAttachment1.DataSource = null;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
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
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
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
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_Solution_FormClosing(object sender, FormClosingEventArgs e)
	{
		_ = 1;
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
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (FormMode == Enums.FormViewModeType.Update)
					{
						if (string.IsNullOrEmpty(await UpdateSolution()))
						{
							_forceClose = true;
							Close();
						}
					}
					else if (FormMode == Enums.FormViewModeType.Add && string.IsNullOrEmpty(await AddSolution()))
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

	private void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Owner" || e.Column.FieldName == "UpdateBy" || e.Column.FieldName == "DateUpdate")
			{
				e.Appearance.BackColor = Color.LightGray;
			}
			if (FormMode != Enums.FormViewModeType.Add && e.Column.FieldName == "SltCode")
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_RowStyle(object sender, RowStyleEventArgs e)
	{
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_preAdd = true;
				ucAttachment1.grd_File.DataSourceTable.Rows.Clear();
				DataTable dtDoc = grdDoc.DataSourceTable.Clone();
				if (!dtDoc.IsNotNull())
				{
					DataTable tableStructure = WinFuntions.GetTableStructure("OSLT");
					dtDoc = tableStructure.Clone();
				}
				DataRow row = dtDoc.NewRow();
				if (rep_Status.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					row["StatusNum"] = dataTable.Rows[0]["Number"];
				}
				row["SltCode"] = await _httpClient.Solution.GetMaxSltCode();
				row["CreatedBy"] = GlobalData.UserID;
				row["DateCreate"] = DateTime.Now.Date;
				row["UpdateBy"] = GlobalData.UserID;
				row["DateUpdate"] = DateTime.Now.Date;
				row["Transfered"] = "N";
				row["Instance"] = 0;
				if (!string.IsNullOrEmpty(ItemCode))
				{
					row["ItemCode"] = ItemCode;
					ItemCode = "";
				}
				dtDoc.Rows.Add(row);
				dtDoc.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dtDoc;
				grdDoc.DataSourceTable.AcceptChanges();
				DataTable dataSourceTable = ucAttachment1.grd_File.DataSourceTable;
				if (dataSourceTable == null && dataSourceTable.Columns.Count < 0)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoCannotLoadData, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK);
				}
				ucAttachment1.DataSource = dataSourceTable;
				FormMode = Enums.FormViewModeType.Add;
				_preAdd = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadInItForm()
	{
		try
		{
			btn_Add.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			FormMode = Enums.FormViewModeType.View;
			LoadDataIntoComponents();
			if (newLine)
			{
				addNewToolStripMenuItem_Click(null, null);
			}
			else if (_sltCode != 0)
			{
				FormMode = Enums.FormViewModeType.View;
				btn_ItemCode.Text = "";
				txt_sltCode.Text = Function.ToString(_sltCode);
				txt_Status.EditValue = null;
				btn_Search_Click(null, null);
			}
			else
			{
				if (sltCode > 0)
				{
					txt_sltCode.EditValue = sltCode;
				}
				DataTable dataTable = await Search(sltCode, "", 0);
				sltCode = 0;
				grdDoc.DataSource = dataTable;
				if (!dataTable.IsNotNull())
				{
					gvDoc_FocusedRowObjectChanged(null, null);
				}
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				grdDoc.DataSourceTable.AcceptChanges();
			}
			rep_Item.ValueMember = "ItemCode";
			rep_Item.FindCFLBy(Enums.TypeCFL.ListItems, false, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_Item;
			repositoryItemButtonEditCFL.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethod, (Action<string>)((string input) =>
			{
				this.DrillDown(ApzObjectTypes.oItems, input);
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> Search(int sltCode, string itemCode, int status)
	{
		try
		{
			return await _httpClient.Solution.Search(sltCode, itemCode, status);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.Solution.FindDataNextLast(Function.ParseInt(gvDoc.GetFocusedRowCellValue("SltCode")), (int)type);
			if (dataTable.IsNotNull())
			{
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.DataSourceTable.AcceptChanges();
				int absEntry = Function.ParseInt(dataTable.Rows[0]["AtcEntry"]);
				await ucAttachment1.LoadAttachments(absEntry);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadDataIntoComponents()
	{
		_ = 2;
		try
		{
			await WinFuntions.LoadUdfColumns(gvDoc, gvDoc.ItemId);
			DataTable lstStatus = await _helper.GetAsync("GetSolutionStatuses");
			rep_Status.DataSource = lstStatus;
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rep_Updateby;
			repositoryItemLookUpEdit.DataSource = await _helper.GetAsync("GetOwner");
			txt_Status.Properties.DataSource = lstStatus;
			txt_Status.Properties.DisplayMember = "Name";
			txt_Status.Properties.ValueMember = "Number";
			txt_Status.Properties.Columns.Add(new LookUpColumnInfo(txt_Status.Properties.DisplayMember));
			rep_Status.Columns.Add(new LookUpColumnInfo(rep_Status.DisplayMember));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> AddSolution()
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				if (Function.ToString(dataSourceTable.Rows[i]["SltCode"]) == "")
				{
					dataSourceTable.Rows.RemoveAt(i--);
				}
				dataSourceTable.Rows[i]["DateCreate"] = DateTime.Now.Date;
				dataSourceTable.Rows[i]["DateUpdate"] = DateTime.Now.Date;
				dataSourceTable.Rows[i]["Transfered"] = "N";
				dataSourceTable.Rows[i]["Instance"] = 0;
				dataSourceTable.Rows[i]["CreatedBy"] = GlobalData.UserID;
				dataSourceTable.Rows[i]["UpdateBy"] = GlobalData.UserID;
				dataSourceTable.Rows[i]["Owner"] = GlobalData.UserID;
			}
			dataSourceTable.AcceptChanges();
			DataTable dataTable = new DataTable();
			if (ucAttachment1.DataSource != null)
			{
				dataTable = ucAttachment1.UploadFileNoSAP();
				if (!dataTable.IsNotNull() && ucAttachment1.DataSource.IsNotNull())
				{
					return "Error";
				}
			}
			dataSourceTable.TableName = "Solution";
			dataTable.TableName = "Attactment";
			dataSet.Tables.Add(dataSourceTable.Copy());
			dataSet.Tables.Add(dataTable.Copy());
			WinCommons._OpenProcessing();
			string text = await _httpClient.Solution.Add(dataSet);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				if (newLine)
				{
					OkToClose = true;
					Close();
				}
				btn_Search_Click(null, null);
				return string.Empty;
			}
			WinCommons._CloseProcessing();
			WinCommons.Error(Enums.MessageCode.None, text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private async Task<string> UpdateSolution()
	{
		try
		{
			DataSet dataSet = new DataSet();
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SltCode"));
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			DataRow[] array = dataTable.Select("SltCode=" + num);
			if (array.Length != 0)
			{
				dataTable = array.CopyToDataTable();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (Function.ToString(dataTable.Rows[i]["SltCode"]) == "")
					{
						dataTable.Rows.RemoveAt(i--);
					}
				}
				dataTable.AcceptChanges();
			}
			for (int j = 0; j < dataTable.Rows.Count; j++)
			{
				dataTable.Rows[j]["UpdateBy"] = GlobalData.UserID;
				dataTable.Rows[j]["DateUpdate"] = DateTime.Now.Date;
				dataTable.AcceptChanges();
			}
			DataTable dataTable2 = new DataTable();
			if (ucAttachment1.DataSource != null)
			{
				dataTable2 = ucAttachment1.UploadFileNoSAP();
				if (!dataTable2.IsNotNull() && ucAttachment1.DataSource.IsNotNull())
				{
					return "Error";
				}
			}
			dataTable.TableName = "Solution";
			dataTable2.TableName = "Attactment";
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			WinCommons._OpenProcessing();
			string text = await _httpClient.Solution.Update(dataSet);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				btn_Search_Click(null, null);
				WinCommons._CloseProcessing();
				return string.Empty;
			}
			WinCommons._CloseProcessing();
			WinCommons.Error(Enums.MessageCode.None, text);
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btn_Search.Enabled = true;
		TextEditEx textEditEx = txt_sltCode;
		ButtonEditEx buttonEditEx = btn_ItemCode;
		bool flag = (txt_Status.ReadOnly = false);
		bool readOnly = (buttonEditEx.ReadOnly = flag);
		textEditEx.ReadOnly = readOnly;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_Solution));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Item = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Updateby = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcUpdateOn = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_sltCode = new Apzon.Controls.Editor.TextEditEx();
		this.ctx_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.ctx_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Updateby).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_sltCode.Properties).BeginInit();
		this.ctx_Line.SuspendLayout();
		base.SuspendLayout();
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Add.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Add.Appearance.Options.UseBackColor = true;
		this.btn_Add.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "Add";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(5, 484);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(99, 20);
		this.btn_Add.TabIndex = 7;
		this.btn_Add.Text = "Add";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(113, 484);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(99, 20);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(-3, 38);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(858, 440);
		this.splitContainerControl1.SplitterPosition = 197;
		this.splitContainerControl1.TabIndex = 46;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.ctx_Doc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_Item, this.rep_Updateby, this.rep_date, this.rep_Status, this.repositoryItemMemoExEdit1 });
		this.grdDoc.Size = new System.Drawing.Size(858, 197);
		this.grdDoc.TabIndex = 5;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.gridColumn1, this.gridColumn2, this.gcUpdateOn, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn14, this.gridColumn16, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oSolutionKnowlegeBase";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OSLT";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupedColumns = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDoc_RowStyle);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gridColumn1.Caption = "ItemCode";
		this.gridColumn1.ColumnEdit = this.rep_Item;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItemCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 1;
		this.gridColumn1.Width = 140;
		this.rep_Item.AutoHeight = false;
		this.rep_Item.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Item.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Item.ContainerControl = this;
		this.rep_Item.DisplayMember = "";
		this.rep_Item.DisplayMember2 = "";
		this.rep_Item.LoadDataSourceWhenEnter = false;
		this.rep_Item.Name = "rep_Item";
		this.rep_Item.SeparateCharacterForMulti = ';';
		this.rep_Item.SeparateString = ", ";
		this.rep_Item.UsingAutoCompleteBox = false;
		this.rep_Item.UsingDataSource = true;
		this.rep_Item.ValidateWhenNoDataSource = false;
		this.rep_Item.ValueMember = "";
		this.rep_Item.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Item_ButtonClick);
		this.gridColumn2.Caption = "Update by";
		this.gridColumn2.ColumnEdit = this.rep_Updateby;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "UpdateBy";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn2.Width = 172;
		this.rep_Updateby.AutoHeight = false;
		this.rep_Updateby.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_Updateby.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Updateby.DisplayMember = "USER_CODE";
		this.rep_Updateby.Name = "rep_Updateby";
		this.rep_Updateby.NullText = "";
		this.rep_Updateby.ValueMember = "USERID";
		this.gcUpdateOn.Caption = "Update on";
		this.gcUpdateOn.ColumnEdit = this.rep_date;
		this.gcUpdateOn.CurrencyDateField = null;
		this.gcUpdateOn.CurrencyField = null;
		this.gcUpdateOn.CurrencyFixed = null;
		this.gcUpdateOn.CurrencyFixedRate = 0.0;
		this.gcUpdateOn.CurrencyRateField = null;
		this.gcUpdateOn.DisplayFormat.FormatString = "dd-MM-yyyy";
		this.gcUpdateOn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcUpdateOn.FieldName = "DateUpdate";
		this.gcUpdateOn.Name = "gcUpdateOn";
		this.gcUpdateOn.OptionsColumn.ReadOnly = true;
		this.gcUpdateOn.PrimaryKey = false;
		this.gcUpdateOn.UnitField = null;
		this.gcUpdateOn.Visible = true;
		this.gcUpdateOn.VisibleIndex = 3;
		this.gcUpdateOn.Width = 116;
		this.rep_date.AutoHeight = false;
		this.rep_date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date.Name = "rep_date";
		this.gridColumn4.Caption = "Status";
		this.gridColumn4.ColumnEdit = this.rep_Status;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "StatusNum";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 4;
		this.gridColumn4.Width = 79;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.DisplayMember = "Name";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ValueMember = "Number";
		this.gridColumn5.Caption = "No";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "SltCode";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 0;
		this.gridColumn5.Width = 76;
		this.gridColumn6.Caption = "Owner";
		this.gridColumn6.ColumnEdit = this.rep_Updateby;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Owner";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn6.Width = 98;
		this.gridColumn7.Caption = "Solution";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Subject";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn7.Width = 227;
		this.gridColumn8.Caption = "Symptom";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Symptom";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.gridColumn8.Width = 152;
		this.gridColumn9.Caption = "Cause";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Cause";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 8;
		this.gridColumn9.Width = 173;
		this.gridColumn10.Caption = "Remarks";
		this.gridColumn10.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Descriptio";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 9;
		this.gridColumn10.Width = 423;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumn14.Caption = "AtcEntry";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "AtcEntry";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn16.Caption = "Date Create";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "DateCreate";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumnEx1.Caption = "gridColumnEx1";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "CreatedBy";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "gridColumnEx2";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Instance";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "gridColumnEx3";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Transfered";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "gridColumnEx4";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Attachment";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(858, 233);
		this.xtraTabControl1.TabIndex = 6;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.ucAttachment1);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Attachments";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(856, 208);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oSolutionKnowlegeBase";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(856, 208);
		this.ucAttachment1.TabIndex = 0;
		this.ucAttachment1.ButtonBrowse_Click += new System.EventHandler(this.ucAttachment1_ButtonBrowse_Click);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(629, 12);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(96, 20);
		this.btn_Search.TabIndex = 4;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Item";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(210, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(63, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 47;
		this.labelLine1.Utext = "Item";
		this.btn_ItemCode.EditControl = null;
		this.btn_ItemCode.FieldName = null;
		this.btn_ItemCode.IsSearchControl = true;
		this.btn_ItemCode.Location = new System.Drawing.Point(274, 12);
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_ItemCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ItemCodeFrom_Properties_ButtonClick);
		this.btn_ItemCode.SettingInVisible = false;
		this.btn_ItemCode.SettingReadonly = false;
		this.btn_ItemCode.Size = new System.Drawing.Size(128, 20);
		this.btn_ItemCode.TabIndex = 2;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Status";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(414, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(63, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 52;
		this.labelLine2.Utext = "Status";
		this.txt_Status.EditControl = null;
		this.txt_Status.FieldName = null;
		this.txt_Status.IsSearchControl = true;
		this.txt_Status.Location = new System.Drawing.Point(477, 12);
		this.txt_Status.Name = "txt_Status";
		this.txt_Status.Properties.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.txt_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Status.Properties.NullText = "";
		this.txt_Status.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.txt_Status.SettingInVisible = false;
		this.txt_Status.SettingReadonly = false;
		this.txt_Status.Size = new System.Drawing.Size(128, 20);
		this.txt_Status.TabIndex = 3;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "No.";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(12, 12);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(63, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 54;
		this.labelLine3.Utext = "No.";
		this.txt_sltCode.EditControl = null;
		this.txt_sltCode.FieldName = null;
		this.txt_sltCode.IsSearchControl = true;
		this.txt_sltCode.Location = new System.Drawing.Point(75, 12);
		this.txt_sltCode.Name = "txt_sltCode";
		this.txt_sltCode.SettingInVisible = false;
		this.txt_sltCode.SettingReadonly = false;
		this.txt_sltCode.Size = new System.Drawing.Size(128, 20);
		this.txt_sltCode.TabIndex = 1;
		this.ctx_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addNewLineToolStripMenuItem });
		this.ctx_Line.Name = "ctx_Line";
		this.ctx_Line.Size = new System.Drawing.Size(149, 26);
		this.addNewLineToolStripMenuItem.Name = "addNewLineToolStripMenuItem";
		this.addNewLineToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.addNewLineToolStripMenuItem.Text = "Add New Line";
		base.AcceptButton = this.btn_Add;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(856, 510);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_ItemCode);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Add);
		base.Controls.Add(this.txt_Status);
		base.Controls.Add(this.txt_sltCode);
		base.FormId = "oSolutionKnowlegeBase";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Solution.IconOptions.Icon");
		base.Name = "frm_Solution";
		this.Text = "Solutions Knowledge Base";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Solution_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Updateby).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_sltCode.Properties).EndInit();
		this.ctx_Line.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
