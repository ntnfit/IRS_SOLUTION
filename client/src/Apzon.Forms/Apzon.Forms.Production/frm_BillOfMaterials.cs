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
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Payments;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Forms.UserControls.Buttons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Production;

public class frm_BillOfMaterials : XtraFormEx
{
	private delegate void SetTextCallback(bool isDisable, TextEdit textBox);

	private delegate void SetTextValueCallback(string value, TextEdit textBox);

	private delegate void SetVisibleButtonCallback(bool value, ButtonEdit bteEdit);

	private bool _isLoading;

	private bool _isPasteFromExcel;

	private bool _isDuplicating;

	private int _activeDimCount;

	private DataTable _dtUdf = new DataTable();

	private string _first = "a";

	private string _last = "b";

	private string _current = "c";

	private DataTable _dtBomItem = new DataTable();

	private List<DataTable> _lstStructDt = new List<DataTable>();

	public string _searchCode = "";

	public string ColumnsUpdate = "project,comment,issuemthd,pricelist,currency";

	public bool _isAddNew;

	private bool _txtProdNoFocused;

	private bool _itemNoFocused;

	private bool _isUpdatePrice;

	private int _curRowtype;

	private static readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private GridViewExPainter painter;

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine lblWarehouse;

	private LabelLine labelLine7;

	private TextEditEx txtProductDesc;

	private LabelLine labelLine8;

	private TextEditEx txtQuantity;

	private GridControlEx grdDoc;

	private LabelLine labelLine9;

	private GridColumnEx gcCode;

	private GridColumnEx gcQuantity;

	private GridColumnEx gcWarehouse;

	private GridColumnEx gcPrice;

	private GridColumnEx gcPriceList;

	private GridColumnEx gcIssueMthd;

	private GridColumnEx gcUom;

	private GridColumnEx gcComment;

	private GridColumnEx gcType;

	private GridColumnEx gcWipActCode;

	private GridColumnEx gcFather;

	private GridColumnEx gcChildNum;

	private CheckEditEx chkHideBom;

	private GridColumnEx gcDescription;

	private RepositoryItemButtonEdit rIBItemCode;

	private RepositoryItemCalcEdit rep_calc;

	private GridColumnEx gcTotal;

	private RepositoryItemButtonEdit rIBWarehouse;

	private RepositoryItemButtonEdit rIBAccount;

	private ContextMenuStrip contextMenuStrip_Line;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ButtonEditEx cbDistrRule;

	private GridViewEx gvDoc;

	private SimpleButtonEx btAdd;

	private SimpleButtonEx btCancel;

	private ButtonEditEx txtProject;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabP_Details;

	private XtraTabPageEx tabP_UDF;

	private GridControlEx grd_UDF;

	private GridViewEx gv_UDF;

	private RepositoryItemComboBox repositoryItemComboBox3;

	private RepositoryItemButtonEdit repositoryItemButtonEdit2;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private RepositoryItemButtonEdit repositoryItemButtonEdit3;

	private RepositoryItemComboBox repositoryItemComboBox2;

	private RepositoryItemButtonEdit repositoryItemButtonEdit4;

	private RepositoryItemComboBox repositoryItemComboBox1;

	private RepositoryItemButtonEdit repositoryItemButtonEdit5;

	private RepositoryItemLookUpEditCombo rep_luk_priceList;

	private ContextMenuStrip contextMenuStrip_Doc;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemLookUpEditCombo rep_luk_Type;

	private RepositoryItemLookUpEditCombo rep_luk_issueMtd;

	private GridColumnEx gcProject;

	private ToolStripMenuItem contextMenuItemVersion;

	private GridColumnEx gridColumn1;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripMenuItem viewDiagramToolStripMenuItem;

	private ButtonEditEx txtProductNo;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private LookUpEditCombo luk_PriceList;

	private LookUpEditCombo luk_Doc_BOMType;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private TextEditExPrice txt_StandardCost;

	private LabelLine labelLine6;

	private CalcEditEx txt_PlAvgSize;

	private LabelLine labelLine10;

	private RepositoryItemButtonEdit rep_bte_Project;

	private ucButtonCustomShape uc_btnCSSetPrice;

	private Label label1;

	private TextEditExPrice calcPrice;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private ButtonEditEx bteWarehouse;

	public GridColumn LineLastColumn { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			if (value != Enums.FormViewModeType.Find && value != Enums.FormViewModeType.View)
			{
				WinCommons.Error(Enums.MessageCode.NoPermissionPerformAction, "");
				return;
			}
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btAdd.Text = Apzon.Commons.Properties.Resources.ButtonFind;
			break;
		}
	}

	public frm_BillOfMaterials()
	{
		InitializeComponent();
		OnLoadAsync += frm_BillOfMaterials_Load;
		FormId = ApzObjectTypes.oProductTrees.ToString();
		gvDoc.FormId = FormId;
		gvDoc.ItemId = "ITT1";
		if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_BillOfMaterials" && x.authoriza == "F"))
		{
			WinFuntions.DisableAllControl(Controls);
			btAdd.Enabled = true;
			btCancel.Enabled = true;
		}
	}

	public override void CHANGELOG()
	{
		try
		{
			string primaryKey = Function.ToString(txtProductNo.EditValue);
			Enums.ChangeLog changeLog = Enums.ChangeLog.clProductTrees;
			GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
			getChangeLogParams.PrimaryKey = primaryKey;
			getChangeLogParams.Object = changeLog;
			frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
			frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
			frm_ChangeLogList2.MdiParent = MdiParent;
			frm_ChangeLogList2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				FormMode = Enums.FormViewModeType.View;
				GetFirst();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				if (_current != _last && !string.IsNullOrWhiteSpace(Function.ToString(txtProductNo.Text)))
				{
					FormMode = Enums.FormViewModeType.View;
					GetNext(Function.ToString(txtProductNo.Text));
				}
				else if (_current == _last)
				{
					MOVE_FIRST();
				}
				else
				{
					MOVE_LAST();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				GetLast();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				if (_current != _first && !string.IsNullOrWhiteSpace(Function.ToString(txtProductNo.Text)))
				{
					FormMode = Enums.FormViewModeType.View;
					GetPrev(Function.ToString(txtProductNo.Text));
				}
				else if (_current == _first)
				{
					MOVE_LAST();
				}
				else
				{
					MOVE_FIRST();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				_isLoading = true;
				FormMode = Enums.FormViewModeType.Find;
				ResetControls();
				luk_Doc_BOMType.EditValue = "P";
				txtProductNo.Properties.Buttons[0].Visible = false;
				RefreshGridDataSource(null);
				RefreshUdfGridDatasource();
				SetControlsBackColor();
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				_isLoading = true;
				FormMode = Enums.FormViewModeType.Add;
				ResetControls();
				txtProductNo.Properties.Buttons[0].Visible = true;
				RefreshGridDataSource(null);
				RefreshUdfGridDatasource();
				SetControlsBackColor();
				luk_Doc_BOMType.EditValue = "P";
				if (luk_PriceList.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					luk_PriceList.EditValue = Function.ParseShort(dataTable.Rows[0]["PriceListNo"]);
				}
				bteWarehouse.EditValue = "";
				AddNewRow();
				_isLoading = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OITT");
				if (dataTable != null)
				{
					DataTable dataTable2 = dataTable.Clone();
					dataTable2.Rows.Add(dataTable2.NewRow());
					dataTable2.Rows[0]["Code"] = txtProductNo.Text;
					dataTable2.AcceptChanges();
					SearchBoM(dataTable2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			string docEntry = Function.ToString(txtProductNo.EditValue);
			await WinFuntions.PrintLayout("", docEntry, "ITT1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			string docEntry = Function.ToString(txtProductNo.EditValue);
			await WinFuntions.PreviewLayout("", docEntry, "ITT1", this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_BillOfMaterials_Load(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			_isLoading = true;
			if (painter == null)
			{
				painter = new GridViewExPainter(gvDoc);
			}
			DataTable dtDoc = GetSearchTableDoc();
			if (dtDoc != null && dtDoc.Rows.Count > 0)
			{
				for (int i = 0; i < dtDoc.Columns.Count; i++)
				{
					if (dtDoc.Columns[i].DataType == typeof(DateTime))
					{
						dtDoc.Rows[0][dtDoc.Columns[i]] = new DateTime(1900, 1, 1);
					}
				}
			}
			DataTable searchTableLine = GetSearchTableLine();
			if (searchTableLine != null && searchTableLine.Columns.Count > 0)
			{
				if (!searchTableLine.Columns.Contains("ItemName"))
				{
					searchTableLine.Columns.Add("ItemName", typeof(string));
				}
				if (!searchTableLine.Columns.Contains("Total"))
				{
					searchTableLine.Columns.Add("Total", typeof(double));
				}
				searchTableLine.AcceptChanges();
			}
			_lstStructDt = new List<DataTable> { dtDoc, searchTableLine };
			if (_lstStructDt[0] != null)
			{
				_lstStructDt[0].TableName = "OITT";
			}
			if (_lstStructDt[1] != null)
			{
				_lstStructDt[1].TableName = "ITT1";
			}
			_activeDimCount = GetActiveDimensionCount();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx>
			{
				["OITT"] = gv_UDF,
				["ITT1"] = gvDoc
			};
			DataTable tableStructure = WinFuntions.GetTableStructure("OITT");
			if (tableStructure != null && tableStructure.Columns.Count > 0 && tableStructure.Columns.Cast<DataColumn>().FirstOrDefault((DataColumn t) => t.ColumnName.StartsWith("U_")) != null)
			{
				await WinFuntions.LoadUdfColumns(gv_UDF, "OITT", tblFormular);
			}
			DataTable tableStructure2 = WinFuntions.GetTableStructure("ITT1");
			if (tableStructure2 != null && tableStructure2.Columns.Count > 0 && tableStructure2.Columns.Cast<DataColumn>().FirstOrDefault((DataColumn t) => t.ColumnName.StartsWith("U_")) != null)
			{
				await WinFuntions.LoadUdfColumns(gvDoc, "ITT1", tblFormular);
			}
			await LoadInitData();
			DataTable searchTableDoc = GetSearchTableDoc();
			bool flag = false;
			if (searchTableDoc != null && searchTableDoc.Columns.Count > 0)
			{
				for (int num = 0; num < searchTableDoc.Columns.Count; num++)
				{
					if (searchTableDoc.Columns[num].ColumnName.ToUpper().StartsWith("U_"))
					{
						flag = true;
						break;
					}
				}
			}
			if (flag)
			{
				_dtUdf = searchTableDoc.Clone();
			}
			SetControlsBackColor();
			if (_searchCode != "" && !_isAddNew)
			{
				DataTable aDoc = GetADoc(_searchCode);
				if (aDoc != null && aDoc.Rows.Count > 0)
				{
					ShowData(aDoc.Rows[0]);
				}
			}
			else if (!(_searchCode != "") || !_isAddNew)
			{
				await MODE_FIND();
			}
			else
			{
				await REFRESH();
				SetTextValue(_searchCode, txtProductNo);
				DataTable dataTable = _dtUdf.Copy();
				dataTable.AcceptChanges();
				grd_UDF.DataSource = dataTable;
				grd_UDF.RefreshDataSource();
				dtDoc.Rows.Clear();
				dtDoc.Rows.Add(dtDoc.NewRow());
				dtDoc.Rows[0]["Code"] = _searchCode;
				dtDoc.AcceptChanges();
				if (!string.IsNullOrEmpty(_searchCode))
				{
					WinCommons._OpenProcessing();
					SearchBoM(dtDoc);
					WinCommons._CloseProcessing();
				}
				_isAddNew = false;
			}
			ucButtonCustomShape ucButtonCustomShape2 = uc_btnCSSetPrice;
			ucButtonCustomShape2.CustomOnClick = (Action<object, EventArgs>)Delegate.Combine(ucButtonCustomShape2.CustomOnClick, new Action<object, EventArgs>(uc_btnCSSetPrice_Click));
			_isLoading = false;
			await WinFuntions.CreateColumnsDistRule(gvDoc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				_curRowtype = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type"));
				PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string> { "Code" }, _curRowtype);
				e.SuppressKeyPress = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void viewDiagramToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(txtProductNo.EditValue);
			if (!string.IsNullOrEmpty(text))
			{
				frm_Diagram frm_Diagram2 = new frm_Diagram();
				frm_Diagram2.ObjectType = ApzObjectTypes.oProductTrees;
				frm_Diagram2.ObjectId = text;
				frm_Diagram2.MdiParent = MdiParent;
				frm_Diagram2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if (msg.Msg == 256 || msg.Msg == 260)
		{
			switch (keyData)
			{
			case Keys.Return:
				btAdd_Click(null, null);
				break;
			case Keys.Tab:
				if (FormMode == Enums.FormViewModeType.Add && _txtProdNoFocused)
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, txtProductNo.Text, null, Enums.ItemObjectType.InventoryItem, null, "");
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItem == null)
					{
						break;
					}
					DataRow chooseItem = frm_CFL2.ChooseItem;
					DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OITT");
					if (dataTable != null)
					{
						DataTable dataTable2 = dataTable.Clone();
						DataRow row = dataTable2.NewRow();
						dataTable2.Rows.Add(row);
						dataTable2.Rows[0]["Code"] = Function.ToString(chooseItem["ItemCode"]);
						dataTable2.AcceptChanges();
						if (!string.IsNullOrEmpty(Function.ToString(chooseItem["ItemCode"])))
						{
							WinCommons._OpenProcessing();
							SearchBoM(dataTable2);
							WinCommons._CloseProcessing();
						}
					}
				}
				else if (gvDoc.IsFocusedView && gvDoc.FocusedColumn == gcCode && Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type")) != -18)
				{
					ChooseFromListItem();
				}
				break;
			}
		}
		return base.ProcessCmdKey(ref msg, keyData);
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btAdd_Click(object sender, EventArgs e)
	{
		_isLoading = true;
		btAdd.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
					{
						btAdd.Enabled = true;
						return;
					}
					if (string.IsNullOrWhiteSpace(txtProductNo.Text))
					{
						WinCommons.Error(Enums.MessageCode.NoProductGiven, "");
						btAdd.Enabled = true;
						return;
					}
					if (Function.ParseDecimal(txtQuantity.Text) <= 0m)
					{
						WinCommons.Error(Enums.MessageCode.ProductQuantityMustGreaterThan0, "");
						btAdd.Enabled = true;
						return;
					}
					if ((Function.ToString(luk_Doc_BOMType.EditValue) == "S" || Function.ToString(luk_Doc_BOMType.EditValue) == "A") && !CheckValidItemType(txtProductNo.Text))
					{
						btAdd.Enabled = true;
						return;
					}
					if (!CheckValidComponents())
					{
						btAdd.Enabled = true;
						return;
					}
					if (GlobalData.LoginSap)
					{
						BillOfMasterials_Impl_OITT billOfMasterials_Impl_OITT = new BillOfMasterials_Impl_OITT
						{
							Code = txtProductNo.Text,
							CreateDate = DateTime.Today,
							PriceList = Function.ParseShort(luk_PriceList.EditValue).Value
						};
						billOfMasterials_Impl_OITT.Qauntity = Function.ParseDecimal(txtQuantity.EditValue);
						billOfMasterials_Impl_OITT.Name = Function.ToString(txtProductDesc.EditValue);
						billOfMasterials_Impl_OITT.ToWH = Function.ToString(bteWarehouse.EditValue);
						billOfMasterials_Impl_OITT.TreeType = Function.ToString(luk_Doc_BOMType.EditValue);
						billOfMasterials_Impl_OITT.UpdateDate = DateTime.Today;
						billOfMasterials_Impl_OITT.UpdateTime = DateTime.Now.Hour;
						billOfMasterials_Impl_OITT.PlAvgSize = Function.ParseDouble(txt_PlAvgSize.EditValue);
						billOfMasterials_Impl_OITT.HideComp = (chkHideBom.Checked ? "Y" : "N");
						string text = Function.ToString(cbDistrRule.EditValue);
						if (!string.IsNullOrWhiteSpace(text))
						{
							string[] array = text.Split(';');
							for (int i = 0; i < array.Length; i++)
							{
								if (!string.IsNullOrWhiteSpace(array[i]))
								{
									switch (i)
									{
									case 0:
										billOfMasterials_Impl_OITT.OcrCode = array[i];
										break;
									case 1:
										billOfMasterials_Impl_OITT.OcrCode2 = array[i];
										break;
									case 2:
										billOfMasterials_Impl_OITT.OcrCode3 = array[i];
										break;
									case 3:
										billOfMasterials_Impl_OITT.OcrCode4 = array[i];
										break;
									case 4:
										billOfMasterials_Impl_OITT.OcrCode5 = array[i];
										break;
									}
								}
							}
						}
						billOfMasterials_Impl_OITT.Project = Function.ToString(txtProject.EditValue);
						if (grd_UDF.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
						{
							List<UserFieldsImpl> uDFByTableName = WinFuntions.GetUDFByTableName(dataTable.Rows[0], "OITT");
							if (uDFByTableName != null && uDFByTableName.Count > 0)
							{
								billOfMasterials_Impl_OITT.UserFields = uDFByTableName;
							}
						}
						billOfMasterials_Impl_OITT.ItemPrice = Function.ParseDouble(calcPrice.EditValue);
						billOfMasterials_Impl_OITT.Currency = calcPrice.Currency;
						billOfMasterials_Impl_OITT.IsUpdatePrice = _isUpdatePrice;
						if (billOfMasterials_Impl_OITT.UserFields == null)
						{
							billOfMasterials_Impl_OITT.UserFields = new List<UserFieldsImpl>();
						}
						List<BillOfMasterials_LineImpl_ITT1> list = new List<BillOfMasterials_LineImpl_ITT1>();
						if (!(grdDoc.DataSource is DataTable dataTable2) || dataTable2.Rows.Count == 0)
						{
							WinCommons.Error(Enums.MessageCode.NoItemInBom, "");
							btAdd.Enabled = true;
							return;
						}
						int num = 0;
						DataView defaultView = dataTable2.Copy().DefaultView;
						defaultView.Sort = "ChildNum asc";
						_dtBomItem = defaultView.ToTable();
						foreach (DataRow row in _dtBomItem.Rows)
						{
							if (Function.ParseInt(row["Type"]) == -18 || (Function.ParseInt(row["Type"]) != -18 && !string.IsNullOrEmpty(row["Code"].ToString().Trim())))
							{
								BillOfMasterials_LineImpl_ITT1 billOfMasterials_LineImpl_ITT = ((Function.ParseInt(row["Type"]) != -18) ? new BillOfMasterials_LineImpl_ITT1
								{
									Code = Function.ToString(row["Code"]),
									Father = billOfMasterials_Impl_OITT.Code,
									ChildNum = ((FormMode == Enums.FormViewModeType.Add) ? num : Function.ParseInt(row["ChildNum"])),
									Quantity = Function.ParseDecimal(row["Quantity"]),
									Type = Function.ParseInt(row["Type"]),
									Uom = Function.ToString(row["Uom"]),
									Warehouse = Function.ToString(row["Warehouse"]),
									WipActCode = Function.ToString(row["WipActCode"]),
									Price = Function.ParseDecimal(row["Price"]),
									PriceList = Convert.ToInt16(Function.ToString(row["PriceList"])),
									Comment = Function.ToString(row["Comment"]),
									IssueMthd = Function.ToString(row["IssueMthd"]),
									Currency = Function.ToString(row["Currency"]),
									OcrCode = Function.ToString(row["OcrCode"]),
									Project = Function.ToString(row["Project"]),
									OcrCode2 = Function.ToString(row["OcrCode2"]),
									OcrCode3 = Function.ToString(row["OcrCode3"]),
									OcrCode4 = Function.ToString(row["OcrCode4"]),
									OcrCode5 = Function.ToString(row["OcrCode5"])
								} : new BillOfMasterials_LineImpl_ITT1
								{
									Type = -18,
									LineText = Function.ToString(row["Code"]),
									Father = billOfMasterials_Impl_OITT.Code,
									ChildNum = ((FormMode == Enums.FormViewModeType.Add) ? num : Function.ParseInt(row["ChildNum"]))
								});
								List<UserFieldsImpl> uDFByTableName2 = WinFuntions.GetUDFByTableName(row, "ITT1");
								if (uDFByTableName2 != null && uDFByTableName2.Count > 0)
								{
									billOfMasterials_LineImpl_ITT.UserFields = uDFByTableName2;
								}
								list.Add(billOfMasterials_LineImpl_ITT);
								num++;
							}
						}
						billOfMasterials_Impl_OITT.Line_IIT1 = list;
						WinCommons._OpenProcessing();
						string text2 = AddData(billOfMasterials_Impl_OITT);
						if (string.IsNullOrWhiteSpace(text2))
						{
							WinCommons.Success(Enums.MessageCode.Success, "");
							if (FormMode == Enums.FormViewModeType.Add)
							{
								REFRESH();
							}
							else if (FormMode == Enums.FormViewModeType.Update)
							{
								FormMode = Enums.FormViewModeType.View;
							}
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text2);
						}
						WinCommons._CloseProcessing();
					}
					else
					{
						DataTable dataTable3 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OITT");
						if (dataTable3 != null)
						{
							DataTable dataTable4 = dataTable3.Clone();
							DataRow dataRow2 = dataTable4.NewRow();
							if (grd_UDF.DataSource is DataTable dataTable5 && dataTable5.Rows.Count > 0)
							{
								foreach (DataColumn column in dataTable5.Columns)
								{
									dataRow2[column.ColumnName] = dataTable5.Rows[0][column];
								}
							}
							dataRow2["Code"] = txtProductNo.Text;
							dataRow2["CreateDate"] = DateTime.Today;
							dataRow2["PriceList"] = Function.ParseShort(luk_PriceList.EditValue).Value;
							dataRow2["Qauntity"] = Function.ParseDecimal(txtQuantity.Text);
							dataRow2["ToWH"] = Function.ToString(bteWarehouse.EditValue);
							dataRow2["TreeType"] = Function.ToString(luk_Doc_BOMType.EditValue);
							dataRow2["UpdateDate"] = DateTime.Today;
							dataRow2["UpdateTime"] = DateTime.Now.Hour;
							dataRow2["Object"] = "66";
							dataRow2["PlAvgSize"] = Function.ParseDouble(txt_PlAvgSize.EditValue);
							dataRow2["HideComp"] = (chkHideBom.Checked ? "Y" : "N");
							string text3 = Function.ToString(cbDistrRule.EditValue);
							if (!string.IsNullOrWhiteSpace(text3))
							{
								string[] array2 = text3.Split(';');
								for (int num2 = 0; num2 < array2.Length; num2++)
								{
									if (!string.IsNullOrWhiteSpace(array2[num2]))
									{
										switch (num2)
										{
										case 0:
											dataRow2["OcrCode"] = array2[num2];
											break;
										case 1:
											dataRow2["OcrCode2"] = array2[num2];
											break;
										case 2:
											dataRow2["OcrCode3"] = array2[num2];
											break;
										case 3:
											dataRow2["OcrCode4"] = array2[num2];
											break;
										case 4:
											dataRow2["OcrCode5"] = array2[num2];
											break;
										}
									}
								}
							}
							dataRow2["Project"] = Function.ToString(txtProject.EditValue);
							dataTable4.Columns.Add(new DataColumn("ItemPrice111", typeof(double)));
							dataTable4.AcceptChanges();
							dataRow2["ItemPrice111"] = Function.ParseDouble(calcPrice.EditValue);
							dataTable4.Rows.Add(dataRow2);
							dataTable4.AcceptChanges();
							DataTable dataTable6 = (DataTable)grdDoc.DataSource;
							if (dataTable6 == null || dataTable6.Rows.Count == 0)
							{
								WinCommons.Error(Enums.MessageCode.NoItemInBom, "");
								btAdd.Enabled = true;
								return;
							}
							dataTable6.AcceptChanges();
							DataTable dataTable7 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
							if (dataTable7 == null)
							{
								WinCommons.Error(Enums.MessageCode.CannotGetLineStructure, "");
								btAdd.Enabled = true;
								return;
							}
							DataTable dataTable8 = dataTable7.Clone();
							for (int num3 = 0; num3 < dataTable6.Rows.Count; num3++)
							{
								DataRow dataRow3 = dataTable8.NewRow();
								for (int num4 = 0; num4 < dataTable6.Columns.Count; num4++)
								{
									if (dataTable8.Columns.Contains(dataTable6.Columns[num4].ColumnName))
									{
										dataRow3[dataTable6.Columns[num4].ColumnName] = dataTable6.Rows[num3][dataTable6.Columns[num4].ColumnName];
									}
								}
								dataTable8.Rows.Add(dataRow3);
							}
							dataTable8.AcceptChanges();
							DataSet dataSet = new DataSet();
							dataSet.Tables.Clear();
							dataSet.Tables.Add(dataTable4);
							dataTable8.DataSet?.Tables.Remove(dataTable8.TableName);
							dataSet.Tables.Add(dataTable8);
							WinCommons._OpenProcessing();
							string action = ((FormMode == Enums.FormViewModeType.Add) ? "A" : "U");
							string text4 = AddDataNoSAP(dataSet, action);
							dataSet.Dispose();
							if (string.IsNullOrWhiteSpace(text4))
							{
								WinCommons.Success(Enums.MessageCode.Success, "");
								if (FormMode == Enums.FormViewModeType.Add)
								{
									REFRESH();
								}
								else if (FormMode == Enums.FormViewModeType.Update)
								{
									FormMode = Enums.FormViewModeType.View;
								}
							}
							else
							{
								WinCommons.Error(Enums.MessageCode.None, text4);
							}
						}
						WinCommons._CloseProcessing();
					}
				}
				else if (FormMode == Enums.FormViewModeType.Find)
				{
					DataTable dataTable9 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OITT");
					if (dataTable9 != null)
					{
						DataTable dataTable10 = dataTable9.Clone();
						dataTable10.Rows.Add(dataTable10.NewRow());
						dataTable10.AcceptChanges();
						dataTable10.Rows[0]["Code"] = txtProductNo.Text;
						dataTable10.Rows[0]["TreeType"] = Function.ToString(luk_Doc_BOMType.EditValue);
						dataTable10.AcceptChanges();
						SearchBoM(dataTable10);
					}
				}
				else if (FormMode == Enums.FormViewModeType.View)
				{
					Close();
				}
				_first = "a";
				_current = "b";
				_last = "c";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btAdd.Enabled = true;
		_isLoading = false;
	}

	private async void txtProductNo_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.BOMCFLOfProduct, false, txtProductNo.Text, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					DataRow chooseItem = frm_CFL2.ChooseItem;
					string text = Function.ToString(chooseItem["ItemCode"]);
					if (!_isDuplicating)
					{
						DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
						if (dataTable != null)
						{
							DataTable dataTable2 = dataTable.Clone();
							dataTable2.AcceptChanges();
							grdDoc.DataSource = dataTable2;
							grdDoc.RefreshDataSource();
							AddNewRow();
						}
						DataTable dataTable3 = _dtUdf.Copy();
						dataTable3.AcceptChanges();
						grd_UDF.DataSource = dataTable3;
						grd_UDF.RefreshDataSource();
						DataTable dataTable4 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "OITT");
						if (dataTable4 != null)
						{
							DataTable dataTable5 = dataTable4.Clone();
							dataTable5.Rows.Add(dataTable5.NewRow());
							dataTable5.Rows[0]["Code"] = text;
							dataTable5.AcceptChanges();
							if (!string.IsNullOrEmpty(text))
							{
								WinCommons._OpenProcessing();
								SearchBoM(dataTable5);
								WinCommons._CloseProcessing();
							}
						}
					}
					else
					{
						_isDuplicating = false;
						if (_httpClient.BillOfMasterials.IsABOM(Function.ToString(chooseItem["ItemCode"])))
						{
							DataTable dataTable6 = grdDoc.DataSource as DataTable;
							List<DataRow> list = new List<DataRow>();
							if (dataTable6 != null && dataTable6.Rows.Count > 0)
							{
								foreach (DataRow row in dataTable6.Copy().Rows)
								{
									if (!string.IsNullOrEmpty(Function.ToString(row["Code"])))
									{
										row["Father"] = text;
										dataTable6.AcceptChanges();
										list.Add(row);
									}
								}
							}
							DataTable aDoc = GetADoc(text);
							ShowData(aDoc.Rows[0]);
							dataTable6 = grdDoc.DataSource as DataTable;
							if (dataTable6 != null && dataTable6.Rows.Count > 0)
							{
								foreach (DataRow item in list)
								{
									dataTable6.ImportRow(item);
								}
							}
							if (dataTable6 != null)
							{
								dataTable6.AcceptChanges();
								for (int num = dataTable6.Rows.Count - 1; num >= 0; num--)
								{
									DataRow dataRow2 = dataTable6.Rows[num];
									if (string.IsNullOrEmpty(Function.ToString(dataRow2["Code"])))
									{
										dataTable6.Rows.Remove(dataRow2);
									}
								}
								dataTable6.AcceptChanges();
								grdDoc.DataSource = dataTable6;
							}
							grdDoc.RefreshDataSource();
							AddNewRow();
							FormMode = Enums.FormViewModeType.Update;
						}
						else
						{
							DataRow dataRow3 = GetItemDetails("I", text).Rows[0];
							SetTextValue(text, txtProductNo);
							SetTextValue(Function.ToString(dataRow3["ItemName"]), txtProductDesc);
							FormMode = Enums.FormViewModeType.Add;
						}
					}
				}
			}
			else if (!string.IsNullOrEmpty(txtProductNo.Text))
			{
				string itemCode = txtProductNo.Text;
				frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, itemCode);
				frm_ItemMasterData2.MdiParent = MdiParent;
				frm_ItemMasterData2.Show();
			}
			LineLastColumn = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void comboBoxEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string value = Function.ToString(cbDistrRule.EditValue);
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(cbDistrRule.EditValue);
			frm_ChooseFromList2.ShowDialog(this);
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text = text + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
				if (text.Length > 1)
				{
					text = text.Remove(text.Length - 1);
				}
			}
			string value2 = text.Replace(";", "");
			if (string.IsNullOrWhiteSpace(value) || !string.IsNullOrWhiteSpace(value2))
			{
				while (text.Length > 0 && text[text.Length - 1].Equals(';'))
				{
					text = text.Remove(text.Length - 1);
				}
				cbDistrRule.EditValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_Doc_BOMType_EditvalueChanged(object sender, EventArgs e)
	{
		try
		{
			SetButtonUpdateText();
			switch (Function.ToString(luk_Doc_BOMType.EditValue))
			{
			case "A":
				lblWarehouse.Visible = false;
				bteWarehouse.Visible = false;
				chkHideBom.Visible = false;
				break;
			case "S":
				lblWarehouse.Visible = false;
				bteWarehouse.Visible = false;
				chkHideBom.Visible = true;
				break;
			case "P":
				lblWarehouse.Visible = true;
				bteWarehouse.Visible = true;
				chkHideBom.Visible = false;
				break;
			case "T":
				lblWarehouse.Visible = false;
				bteWarehouse.Visible = false;
				chkHideBom.Visible = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_PriceList_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetButtonUpdateText();
			if (gvDoc.DataRowCount == 0)
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (_isLoading || FormMode == Enums.FormViewModeType.Find || (dataTable != null && dataTable.Rows.Count > 0 && !dataTable.AsEnumerable().Any((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["Code"])))))
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdatePriceListInRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				short? num = Function.ParseShort(luk_PriceList.EditValue);
				for (int num2 = 0; num2 < gvDoc.DataRowCount; num2++)
				{
					gvDoc.SetRowCellValue(num2, "PriceList", num);
				}
			}
			if (!string.IsNullOrEmpty(txtProductNo.Text))
			{
				DataTable itemPriceByPriceList = GetItemPriceByPriceList(txtProductNo.Text, Function.ParseInt(luk_PriceList.EditValue));
				if (itemPriceByPriceList.IsNotNull())
				{
					calcPrice.EditValue = Function.ParseDouble(itemPriceByPriceList.Rows[0][0]);
					calcPrice.Currency = Function.ToString(itemPriceByPriceList.Rows[0][1]);
				}
				else
				{
					calcPrice.EditValue = 0;
					calcPrice.Currency = GlobalData.MainCurrency;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			switch (e.Column.FieldName)
			{
			case "Code":
				if (!_isPasteFromExcel)
				{
					_curRowtype = Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "Type"));
				}
				LoadItemDetails(e.RowHandle);
				AddNewRow();
				break;
			case "Price":
			case "Quantity":
			{
				decimal num = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "Price"));
				decimal num2 = Function.ParseDecimal(gvDoc.GetRowCellValue(e.RowHandle, "Quantity"));
				gvDoc.SetRowCellValue(e.RowHandle, "Total", num * num2);
				break;
			}
			case "PriceList":
			{
				string itemCode = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "Code"));
				int priceList = Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "PriceList"));
				DataTable itemPriceByPriceList = GetItemPriceByPriceList(itemCode, priceList);
				if (itemPriceByPriceList.IsNotNull())
				{
					gvDoc.SetRowCellValue(e.RowHandle, "Price", itemPriceByPriceList.Rows[0][0]);
					gvDoc.SetRowCellValue(e.RowHandle, "Currency", itemPriceByPriceList.Rows[0][1]);
				}
				else
				{
					gvDoc.SetRowCellValue(e.RowHandle, "Price", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Currency", GlobalData.MainCurrency);
				}
				break;
			}
			}
			SetButtonUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type")) != -18)
				{
					ChooseFromListItem();
				}
				return;
			}
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type")) != -18)
			{
				LoadBoMFromGridItem();
				return;
			}
			frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
			frm_RowTextDetails2.RowText = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
			if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
			{
				gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "Code", frm_RowTextDetails2.RowText);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBWarehouse_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("Warehouse"));
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, string.Empty);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
					if (Function.ToString(dataRow["WhsCode"]) != "" && Function.ToString(dataRow["WhsCode"]).ToUpper() != text.ToUpper())
					{
						SetButtonUpdateText();
						gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "Warehouse", dataRow["WhsCode"]);
					}
				}
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBAccount_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		if (e.Button.Index == 0)
		{
			gvDoc.GetFocusedRowCellValue("Code").ToString();
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ChartOfAccountPurpose, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				Payments_AccountsImpl payments_AccountsImpl = (Payments_AccountsImpl)frm_ChooseFromList2.ChooseItem;
				gvDoc.SetFocusedRowCellValue("WipActCode", payments_AccountsImpl.AcctCode);
			}
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gvDoc.GetSelectedRows();
			if (selectedRows != null && selectedRows.Length != 0)
			{
				DataTable dataTable = grdDoc.DataSource as DataTable;
				gvDoc.DeleteSelectedRows();
				SetButtonUpdateText();
				dataTable?.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			LineLastColumn = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtProductNo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetButtonUpdateText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txtProductNo_Leave(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_txtProdNoFocused = false;
			if (FormMode != Enums.FormViewModeType.Add)
			{
				return;
			}
			object txt = txtProductNo.EditValue;
			if (txt == null || string.IsNullOrWhiteSpace(txt.ToString()))
			{
				return;
			}
			if (grdDoc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrWhiteSpace(Function.ToString(t["Code"]))) == null)
				{
					if (await ExtenseionMethods.CheckExistsItemCode(txt.ToString()))
					{
						WaitLoadBoMItemByItemCode(txt.ToString());
					}
				}
				else
				{
					AddNewRow();
				}
			}
			else if (await ExtenseionMethods.CheckExistsItemCode(txt.ToString()))
			{
				WaitLoadBoMItemByItemCode(txt.ToString());
			}
			else
			{
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtProductNo_Enter(object sender, EventArgs e)
	{
		try
		{
			_txtProdNoFocused = true;
			_itemNoFocused = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Project_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Project, false, txtProject.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null && frm_CFL2.ChooseItem.IsExistsColumn("PrjCode"))
			{
				txtProject.Text = Function.ToString(frm_CFL2.ChooseItem["PrjCode"]);
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateProjectInRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes && grdDoc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						if (Function.ParseInt(dataTable.Rows[i]["Type"]) == 4)
						{
							dataTable.Rows[i]["Project"] = txtProject.Text;
						}
					}
					dataTable.AcceptChanges();
					grdDoc.DataSource = dataTable;
					grdDoc.RefreshDataSource();
				}
			}
			LineLastColumn = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Warehouse_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Warehouse, false, Function.ToString(bteWarehouse.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null && frm_CFL2.ChooseItem.IsExistsColumn("WhsCode"))
				{
					bteWarehouse.EditValue = Function.ToString(frm_CFL2.ChooseItem["WhsCode"]);
				}
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, Function.ToString(bteWarehouse.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rIBItemCode_Enter(object sender, EventArgs e)
	{
		try
		{
			_txtProdNoFocused = false;
			_itemNoFocused = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rIBItemCode_Leave(object sender, EventArgs e)
	{
		try
		{
			_itemNoFocused = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvDoc.FocusedColumn != gcType && Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type")) == 290 && gvDoc.FocusedColumn == gcPriceList)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				string text = txtProductNo.Text;
				if (!string.IsNullOrWhiteSpace(text) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					DeleteDoc(text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void cbDistrRule_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			ButtonEditEx buttonEditEx = cbDistrRule;
			if (!buttonEditEx.IsModified)
			{
				return;
			}
			string text = Function.ToString(buttonEditEx.EditValue);
			if (text == "")
			{
				return;
			}
			string text2 = (string)(buttonEditEx.EditValue = WinFuntions.ValidateDistrRule(text));
			if (text2 != text)
			{
				BeginInvoke((Action)(() =>
				{
					WinFuntions.ShowCFL(cbDistrRule);
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip_Doc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				duplicateToolStripMenuItem.Visible = false;
				removeToolStripMenuItem.Visible = false;
			}
			else if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				duplicateToolStripMenuItem.Visible = true;
				removeToolStripMenuItem.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				FormMode = Enums.FormViewModeType.Add;
				txtProductNo.Text = "";
				txtProductDesc.Text = "";
				DisableTextBoxes(isDisable: false);
				_isDuplicating = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_luk_priceList_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gvDoc.PostEditor();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuItemVersion_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(txtProductNo.EditValue);
			if (!string.IsNullOrEmpty(text))
			{
				frm_AdvanceBillOfMaterials frm_AdvanceBillOfMaterials2 = new frm_AdvanceBillOfMaterials(text);
				frm_AdvanceBillOfMaterials2.MdiParent = MdiParent;
				frm_AdvanceBillOfMaterials2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.YouMustChooseBomBeforeChooseVersion, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.ToUpper().Equals("PRICE"))
			{
				string currency = Function.ToString(gvDoc.GetListSourceRowCellValue(e.ListSourceRowIndex, "Currency"));
				int num = -1;
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
					if (currencyCodesImpl != null)
					{
						num = currencyCodesImpl.Decimals;
					}
				}
				if (Function.ParseDouble(e.Value) > 0.0)
				{
					e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currency.ToUpper());
				}
				else
				{
					e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} ", e.Value);
				}
			}
			else if (e.Column.FieldName == "Code")
			{
				e.DisplayText = Function.ToString(e.Value).Replace(Environment.NewLine, " ");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			GridColumn focusedColumn = gvDoc.FocusedColumn;
			if (!focusedColumn.FieldName.ToUpper().Equals("PRICE"))
			{
				return;
			}
			int focusedRowHandle = gvDoc.FocusedRowHandle;
			List<string> currency = Function.GetCurrency(Function.ToString(e.Value));
			if (currency.Count > 0)
			{
				decimal num = Function.ParseDecimal(currency[0]);
				string alpha = Function.ToString(currency[1]);
				if (string.IsNullOrEmpty(alpha))
				{
					alpha = GlobalData.MainCurrency;
				}
				if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha.ToUpper())))
				{
					gvDoc.SetFocusedRowCellValue("Currency", alpha);
					e.Value = num;
					gvDoc.SetRowCellValue(focusedRowHandle, focusedColumn, num);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Control_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_bte_Project_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Project, false, txtProject.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog();
			if (frm_CFL2.ChooseItem != null && frm_CFL2.ChooseItem.IsExistsColumn("PrjCode"))
			{
				gvDoc.SetFocusedRowCellValue("Project", Function.ToString(frm_CFL2.ChooseItem["PrjCode"]));
			}
			LineLastColumn = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Controls_EditValueChanged(object sender, EventArgs e)
	{
		SetButtonUpdateText();
	}

	private void gvDoc_EndSorting(object sender, EventArgs e)
	{
	}

	private async Task LoadInitData()
	{
		try
		{
			LoadPriceList();
			LoadComponentType();
			LoadBomType();
			LoadIssueType();
			ResetControls();
			luk_Doc_BOMType.EditValue = 4;
			txtQuantity.Text = "1";
			LoadFirstItemCode();
			LoadLastItemCode();
			txtProductNo.Properties.Buttons[0].Visible = false;
			txtProject.Text = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComponentType()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "Item";
			dataRow[1] = 4;
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = "Resource";
			dataRow2[1] = 290;
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3[0] = "Text";
			dataRow3[1] = -18;
			dataTable.Rows.Add(dataRow3);
			dataTable.AcceptChanges();
			rep_luk_Type.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBomType()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "Assembly";
			dataRow[1] = "A";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = "Sales";
			dataRow2[1] = "S";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3[0] = "Production";
			dataRow3[1] = "P";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4[0] = "Template";
			dataRow4[1] = "T";
			dataTable.Rows.Add(dataRow4);
			dataTable.AcceptChanges();
			luk_Doc_BOMType.Properties.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadIssueType()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "Backflush";
			dataRow[1] = "B";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = "Manual";
			dataRow2[1] = "M";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			rep_luk_issueMtd.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadWarehouse()
	{
		try
		{
			_httpClient.BillOfMasterials.Warehouse_ShowWarehouse();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadPriceList()
	{
		try
		{
			DataTable dataTable = _httpClient.BillOfMasterials.PriceList_ShowPriceList();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseShort(r["PriceListNo"]) == -1);
				if (dataRow == null)
				{
					dataRow = dataTable.NewRow();
					dataRow["PriceListNo"] = -1;
					dataRow["PriceListName"] = "Last Purchase Price";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
				DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseShort(r["PriceListNo"]) == -2);
				if (dataRow2 == null)
				{
					dataRow2 = dataTable.NewRow();
					dataRow2["PriceListNo"] = -2;
					dataRow2["PriceListName"] = "Last Evaluated Price";
					dataTable.Rows.Add(dataRow2);
					dataTable.AcceptChanges();
				}
			}
			luk_PriceList.Properties.DataSource = dataTable;
			rep_luk_priceList.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataRow GetItemName(string itemCode)
	{
		try
		{
			itemCode = Uri.EscapeDataString(itemCode);
			DataTable itemName = _httpClient.BillOfMasterials.GetItemName(itemCode);
			if (itemName.Rows.Count > 0)
			{
				return itemName.Rows[0];
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private void WaitLoadBoMItemByItemCode(string code)
	{
		_isLoading = true;
		try
		{
			LoadBoMItemByItemCode(code, isLoadItemName: true);
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void LoadBoMItemByItemCode(string code, bool isLoadItemName = false)
	{
		try
		{
			DataTable dataTable = _httpClient.BillOfMasterials.ShowITT1(code);
			DataTable dataTable2 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
			if (dataTable2 == null)
			{
				return;
			}
			DataTable dts = dataTable2.Clone();
			if (dts.Columns.Count <= 0 || dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				DataRow des = dts.NewRow();
				SetDTRValues(dataTable.Rows[num], ref des);
				dts.Rows.Add(des);
			}
			dts.AcceptChanges();
			if (isLoadItemName)
			{
				DataTable dt = ExtenseionMethods.GetItemByItemCode(code);
				if (dt != null && dt.Rows.Count > 0)
				{
					if (txtProductDesc.InvokeRequired)
					{
						BeginInvoke((MethodInvoker)(() =>
						{
							txtProductDesc.Text = Function.ToString(dt.Rows[0]["ItemName"]);
						}));
					}
					else
					{
						txtProductDesc.Text = Function.ToString(dt.Rows[0]["ItemName"]);
					}
				}
			}
			if (grdDoc.InvokeRequired)
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					grdDoc.DataSource = dts;
					grdDoc.RefreshDataSource();
				}));
			}
			else
			{
				grdDoc.DataSource = dts;
				grdDoc.RefreshDataSource();
			}
			LineLastColumn = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SearchBoM(DataTable source)
	{
		_isLoading = true;
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Clear();
			dataSet.Tables.Add(source);
			DataTable table = new DataTable();
			dataSet.Tables.Add(table);
			DataTable dataTable = _httpClient.BillOfMasterials.Search(dataSet);
			if (dataTable != null && ((FormMode != Enums.FormViewModeType.Add && dataTable.Rows.Count > 0) || (FormMode == Enums.FormViewModeType.Add && (from t in dataTable.AsEnumerable()
				where Function.ToString(t["Code"]).Equals(Function.ToString(txtProductNo.EditValue))
				select t).Any())))
			{
				if (dataTable.Rows.Count > 1)
				{
					frm_SearchDataList frm_SearchDataList2 = new frm_SearchDataList
					{
						SourceDt = dataTable,
						XFormId = "OITT_SR",
						ItemId = "GVCFL",
						Text = "Bills of Material List",
						ObjType = ApzObjectTypes.oProductTrees
					};
					frm_SearchDataList2.ShowDialog(this);
					if (frm_SearchDataList2.ChooseItem != null)
					{
						dynamic aDoc = GetADoc(Function.ToString(frm_SearchDataList2.ChooseItem["Code"]));
						ShowData(aDoc.Rows[0]);
					}
				}
				else
				{
					DataTable aDoc2 = GetADoc(Function.ToString(dataTable.Rows[0]["Code"]));
					ShowData(aDoc2.Rows[0]);
				}
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				DataRow dataRow = GetItemDetails("I", Function.ToString(source.Rows[0]["Code"])).Rows[0];
				SetTextValue(Function.ToString(source.Rows[0]["Code"]), txtProductNo);
				SetTextValue(Function.ToString(dataRow["ItemName"]), txtProductDesc);
				if (string.IsNullOrWhiteSpace(Function.ToString(bteWarehouse.EditValue)))
				{
					bteWarehouse.EditValue = Function.ToString(dataRow["DefaultWarehouse"]);
				}
				if (string.IsNullOrWhiteSpace(Function.ToString(txtQuantity.EditValue)))
				{
					SetTextValue("1", txtQuantity);
				}
				if (string.IsNullOrWhiteSpace(Function.ToString(luk_Doc_BOMType.EditValue)))
				{
					luk_Doc_BOMType.EditValue = "P";
				}
				if (string.IsNullOrWhiteSpace(Function.ToString(cbDistrRule.EditValue)))
				{
					cbDistrRule.EditValue = "";
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	public static async Task<DataTable> GetBoMByItemCode(string code)
	{
		try
		{
			return await _httpClient.BillOfMasterials.getBoMByItemCode(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private int GetActiveDimensionCount()
	{
		int result = 0;
		try
		{
			DataTable activeDimensionCount = _httpClient.BillOfMasterials.GetActiveDimensionCount();
			if (activeDimensionCount != null && activeDimensionCount.Rows.Count > 0)
			{
				result = Function.ParseInt(activeDimensionCount.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private string AddData(BillOfMasterials_Impl_OITT doc)
	{
		try
		{
			return _httpClient.BillOfMasterials.SaveDoc(doc);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddDataNoSAP(DataSet ds, string action)
	{
		try
		{
			if (action == "A")
			{
				return _httpClient.BillOfMasterials.SaveDocNoSAP(ds);
			}
			return _httpClient.BillOfMasterials.SaveDocNoSAPUpdate(ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private async Task AddNewRow(int rowHandle = -1)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable;
			if (dataTable == null)
			{
				return;
			}
			if (dataTable.Columns.Count <= 0)
			{
				DataTable dataTable2 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
				if (dataTable2 != null)
				{
					dataTable = dataTable2.Clone();
				}
			}
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow x) => Function.ParseInt(x["Type"]) != -18 && string.IsNullOrWhiteSpace(Function.ToString(x["Code"]))) != null)
			{
				if (dataTable.Rows.Count <= 1)
				{
					RefreshGridDataSource(dataTable);
				}
			}
			else
			{
				DataRow dataRow = dataTable.NewRow();
				dataTable.Rows.Add(dataRow);
				dataRow["Father"] = txtProductNo.Text;
				short num = 0;
				if (luk_PriceList.Properties.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
				{
					num = Function.ParseShort(dataTable3.Rows[0]["PriceListNo"]).GetValueOrDefault();
				}
				dataRow["PriceList"] = ((luk_PriceList.EditValue == null) ? num : Convert.ToInt16(Function.ToString(luk_PriceList.EditValue)));
				dataRow["Quantity"] = 1;
				dataRow["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
				dataRow["Type"] = 4;
				int num2 = 0;
				try
				{
					num2 = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ChildNum"]));
				}
				catch (Exception)
				{
				}
				dataRow["ChildNum"] = num2 + 1;
				dataRow["Currency"] = GlobalData.MainCurrency;
				dataRow["IssueMthd"] = "B";
				dataTable.AcceptChanges();
				RefreshGridDataSource(dataTable);
				if (gvDoc.Columns.FirstOrDefault((GridColumn col) => col.ColumnEditName.Contains("rep_line_")) == null)
				{
					await WinFuntions.CreateColumnsDistRule(gvDoc);
				}
			}
			if (rowHandle >= 0)
			{
				gvDoc.FocusedRowHandle = rowHandle;
				gvDoc.FocusedColumn = gcQuantity;
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private DataTable GetNonEmptyRow(DataTable source)
	{
		DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
		if (dataTable == null)
		{
			return new DataTable();
		}
		DataTable dataTable2 = dataTable.Clone();
		try
		{
			for (int num = 0; num < source.Rows.Count; num++)
			{
				if (Function.ParseInt(source.Rows[num]["Type"]) == -18 || (Function.ParseInt(source.Rows[num]["Type"]) != -18 && !string.IsNullOrEmpty(Function.ToString(source.Rows[num]["Code"]))))
				{
					dataTable2.ImportRow(source.Rows[num]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable2;
	}

	private void ChooseFromListItem()
	{
		try
		{
			string txtSeach = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type"));
			Enums.TypeCFL type = Enums.TypeCFL.ListItems;
			if (num == 290)
			{
				type = Enums.TypeCFL.ListMasterResource;
			}
			frm_CFL frm_CFL2 = new frm_CFL(type, true, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ItemDescription,InvntryUom";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null)
			{
				return;
			}
			int focusedRowHandle = gvDoc.FocusedRowHandle;
			switch (num)
			{
			case 290:
			{
				if (frm_CFL2.ChooseItems.Count <= 0)
				{
					return;
				}
				_dtBomItem = grdDoc.DataSource as DataTable;
				if (_dtBomItem == null)
				{
					DataTable dataTable2 = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
					if (dataTable2 == null)
					{
						return;
					}
					_dtBomItem = dataTable2.Clone();
				}
				int focusedDataSourceRowIndex2 = gvDoc.GetFocusedDataSourceRowIndex();
				if (focusedDataSourceRowIndex2 >= 0)
				{
					DataRow dataRow5 = frm_CFL2.ChooseItems[0];
					DataRow dataRow6 = _dtBomItem.Rows[focusedDataSourceRowIndex2];
					string text3 = Function.ToString(dataRow5["ResCode"]);
					if (!string.IsNullOrEmpty(text3))
					{
						dataRow6["Code"] = text3;
						dataRow6["Father"] = txtProductNo.Text;
						dataRow6["Price"] = 0;
						dataRow6["PriceList"] = Convert.ToInt16(0);
						dataRow6["Quantity"] = 1;
						DataTable itemDetails3 = GetItemDetails("R", text3);
						string value3 = Function.ToString(itemDetails3.Rows[0]["DefaultWarehouse"]);
						if (itemDetails3.IsNotNull() && !string.IsNullOrWhiteSpace(value3))
						{
							dataRow6["Warehouse"] = value3;
						}
						else
						{
							dataRow6["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
						}
						dataRow6["Total"] = 0;
						dataRow6["Currency"] = GlobalData.MainCurrency;
						dataRow6["Quantity"] = 1;
						dataRow6["Type"] = num;
						dataRow6["IssueMthd"] = Function.ToString(itemDetails3.Rows[0]["IssueMthd"]);
						dataRow6["WipActCode"] = "";
						dataRow6["ItemName"] = Function.ToString(dataRow5["ResName"]);
						_dtBomItem.AcceptChanges();
					}
				}
				for (int num4 = 1; num4 < frm_CFL2.ChooseItems.Count; num4++)
				{
					DataRow dataRow7 = frm_CFL2.ChooseItems[num4];
					string text4 = Function.ToString(dataRow7["ResCode"]);
					DataRow dataRow8 = _dtBomItem.NewRow();
					dataRow8["Code"] = text4;
					dataRow8["Father"] = txtProductNo.Text;
					dataRow8["Price"] = 0;
					int num5 = 0;
					try
					{
						num5 = _dtBomItem.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ChildNum"]));
					}
					catch (Exception)
					{
					}
					dataRow8["ChildNum"] = num5 + 1;
					dataRow8["PriceList"] = Convert.ToInt16(0);
					dataRow8["Quantity"] = 1;
					DataTable itemDetails4 = GetItemDetails("R", text4);
					string value4 = Function.ToString(itemDetails4.Rows[0]["DefaultWarehouse"]);
					if (itemDetails4.IsNotNull() && !string.IsNullOrWhiteSpace(value4))
					{
						dataRow8["Warehouse"] = value4;
					}
					else
					{
						dataRow8["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
					}
					dataRow8["Total"] = 0;
					dataRow8["Currency"] = GlobalData.MainCurrency;
					dataRow8["Quantity"] = 1;
					dataRow8["Type"] = num;
					dataRow8["IssueMthd"] = Function.ToString(itemDetails4.Rows[0]["IssueMthd"]);
					dataRow8["WipActCode"] = "";
					dataRow8["ItemName"] = Function.ToString(dataRow7["ResName"]);
					_dtBomItem.Rows.InsertAt(dataRow8, focusedDataSourceRowIndex2 + num4);
				}
				break;
			}
			case 4:
			{
				if (frm_CFL2.ChooseItems.Count <= 0)
				{
					return;
				}
				_dtBomItem = grdDoc.DataSource as DataTable;
				if (_dtBomItem == null)
				{
					DataTable dataTable = _lstStructDt.FirstOrDefault((DataTable t) => t.TableName == "ITT1");
					if (dataTable == null)
					{
						return;
					}
					_dtBomItem = dataTable.Clone();
				}
				int focusedDataSourceRowIndex = gvDoc.GetFocusedDataSourceRowIndex();
				if (focusedDataSourceRowIndex >= 0)
				{
					DataRow dataRow = frm_CFL2.ChooseItems[0];
					DataRow dataRow2 = _dtBomItem.Rows[focusedDataSourceRowIndex];
					string text = Function.ToString(dataRow["ItemCode"]);
					if (!string.IsNullOrEmpty(text))
					{
						dataRow2["Code"] = text;
						dataRow2["Father"] = txtProductNo.Text;
						dataRow2["PriceList"] = Function.ParseShort(luk_PriceList.EditValue).Value;
						DataTable itemPriceByPriceList = GetItemPriceByPriceList(text, Function.ParseInt(dataRow2["PriceList"]));
						if (itemPriceByPriceList.IsNotNull())
						{
							dataRow2["Price"] = itemPriceByPriceList.Rows[0][0];
							dataRow2["Currency"] = itemPriceByPriceList.Rows[0][1];
						}
						dataRow2["Total"] = dataRow2["Price"];
						dataRow2["Quantity"] = 1;
						DataTable itemDetails = GetItemDetails("I", text);
						string value = Function.ToString(itemDetails.Rows[0]["DefaultWarehouse"]);
						if (itemDetails.IsNotNull() && !string.IsNullOrWhiteSpace(value))
						{
							dataRow2["Warehouse"] = value;
						}
						else
						{
							dataRow2["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
						}
						dataRow2["Quantity"] = 1;
						dataRow2["Currency"] = GlobalData.MainCurrency;
						dataRow2["Type"] = num;
						dataRow2["IssueMthd"] = "B";
						dataRow2["WipActCode"] = "";
						dataRow2["ItemName"] = Function.ToString(dataRow["ItemDescription"]);
						dataRow2["IssueMthd"] = Function.ToString(itemDetails.Rows[0]["IssueMthd"]);
						if (dataRow.IsExistsColumn("InvntryUom"))
						{
							dataRow2["Uom"] = Function.ToString(dataRow["InvntryUom"]);
						}
						_dtBomItem.AcceptChanges();
					}
				}
				for (int num2 = 1; num2 < frm_CFL2.ChooseItems.Count; num2++)
				{
					DataRow dataRow3 = frm_CFL2.ChooseItems[num2];
					string text2 = Function.ToString(dataRow3["ItemCode"]);
					DataRow dataRow4 = _dtBomItem.NewRow();
					dataRow4["Code"] = text2;
					dataRow4["Father"] = txtProductNo.Text;
					int num3 = 0;
					try
					{
						num3 = _dtBomItem.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ChildNum"]));
					}
					catch (Exception)
					{
					}
					dataRow4["ChildNum"] = num3 + 1;
					dataRow4["PriceList"] = Function.ParseShort(luk_PriceList.EditValue).Value;
					DataTable itemPriceByPriceList2 = GetItemPriceByPriceList(text2, Function.ParseInt(dataRow4["PriceList"]));
					if (itemPriceByPriceList2.IsNotNull())
					{
						dataRow4["Price"] = itemPriceByPriceList2.Rows[0][0];
						dataRow4["Currency"] = itemPriceByPriceList2.Rows[0][1];
					}
					dataRow4["Total"] = dataRow4["Price"];
					dataRow4["Quantity"] = 1;
					DataTable itemDetails2 = GetItemDetails("I", text2);
					string value2 = Function.ToString(itemDetails2.Rows[0]["DefaultWarehouse"]);
					if (itemDetails2.IsNotNull() && !string.IsNullOrWhiteSpace(value2))
					{
						dataRow4["Warehouse"] = value2;
					}
					else
					{
						dataRow4["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
					}
					dataRow4["Quantity"] = 1;
					dataRow4["Currency"] = GlobalData.MainCurrency;
					dataRow4["Type"] = num;
					dataRow4["IssueMthd"] = "B";
					dataRow4["WipActCode"] = "";
					dataRow4["ItemName"] = Function.ToString(dataRow3["ItemDescription"]);
					dataRow4["IssueMthd"] = Function.ToString(itemDetails2.Rows[0]["IssueMthd"]);
					if (dataRow3.IsExistsColumn("InvntryUom"))
					{
						dataRow4["Uom"] = Function.ToString(dataRow3["InvntryUom"]);
					}
					_dtBomItem.Rows.InsertAt(dataRow4, focusedDataSourceRowIndex + num2);
				}
				break;
			}
			}
			_dtBomItem.AcceptChanges();
			DataTable nonEmptyRow = GetNonEmptyRow(_dtBomItem);
			SetButtonUpdateText();
			RefreshGridDataSource(nonEmptyRow);
			AddNewRow(focusedRowHandle);
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex3.Message);
		}
	}

	private void SetDTRValues(DataRow source, ref DataRow des)
	{
		try
		{
			for (int i = 0; i < des.Table.Columns.Count; i++)
			{
				if (!source.Table.Columns.Contains(des.Table.Columns[i].ColumnName))
				{
					continue;
				}
				switch (des.Table.Columns[i].DataType.ToString().ToLower())
				{
				case "system.string":
					des[des.Table.Columns[i].ColumnName] = Function.ToString(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.int64":
					des[des.Table.Columns[i].ColumnName] = Function.ParseLong(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.decimal":
					des[des.Table.Columns[i].ColumnName] = Function.ParseDecimal(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.double":
					des[des.Table.Columns[i].ColumnName] = Function.ParseDouble(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.int16":
					des[des.Table.Columns[i].ColumnName] = Function.ParseShort(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.int32":
					des[des.Table.Columns[i].ColumnName] = Function.ParseInt(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.boolean":
					des[des.Table.Columns[i].ColumnName] = Function.ParseBool(source[des.Table.Columns[i].ColumnName]);
					break;
				case "system.datetime":
				{
					DateTime? dateTime = Function.ParseDateTime(source[des.Table.Columns[i].ColumnName]);
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						des[des.Table.Columns[i].ColumnName] = dateTime.Value;
					}
					else
					{
						des[des.Table.Columns[i].ColumnName] = DBNull.Value;
					}
					break;
				}
				default:
					des[des.Table.Columns[i].ColumnName] = "";
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetADoc(string code)
	{
		try
		{
			return _httpClient.BillOfMasterials.SearchADoc(code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private DataTable GetSearchTableDoc()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("OITT");
			if (tableStructure.Columns.Count > 0)
			{
				tableStructure.Rows.Add(tableStructure.NewRow());
				tableStructure.AcceptChanges();
			}
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private DataTable GetSearchTableLine()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("ITT1");
			if (tableStructure.Columns.Count > 0)
			{
				tableStructure.Rows.Add(tableStructure.NewRow());
				tableStructure.AcceptChanges();
			}
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private void ProcessGetTable(DataTable source, ref DataTable ret)
	{
		try
		{
			string text = Function.ToString(source.Rows[0][0]);
			if (text.Length <= 0)
			{
				return;
			}
			text = text.Remove(text.Length - 1);
			string[] array = text.Split(',');
			for (int i = 0; i < array.Length; i++)
			{
				string text2 = array[i].Split(' ')[0];
				string text3 = array[i].Split(' ')[1];
				if (!ret.Columns.Contains(text2))
				{
					DataColumn dataColumn = new DataColumn
					{
						ColumnName = text2,
						Caption = text2
					};
					switch (text3)
					{
					case "nvarchar":
						dataColumn.DataType = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					case "char":
						dataColumn.DataType = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					case "money":
						dataColumn.DataType = typeof(decimal);
						dataColumn.DefaultValue = Function.ParseDecimal(0);
						break;
					case "numeric":
						dataColumn.DataType = typeof(decimal);
						dataColumn.DefaultValue = Function.ParseDecimal(0);
						break;
					case "float":
						dataColumn.DataType = typeof(double);
						dataColumn.DefaultValue = Function.ParseDouble(0);
						break;
					case "decimal":
						dataColumn.DataType = typeof(decimal);
						dataColumn.DefaultValue = Function.ParseDecimal(0);
						break;
					case "datetime":
						dataColumn.DataType = typeof(DateTime);
						dataColumn.DefaultValue = Function.ParseDateTime(0);
						break;
					case "date":
						dataColumn.DataType = typeof(DateTime);
						dataColumn.DefaultValue = Function.ParseDateTime(0);
						break;
					case "smallint":
						dataColumn.DataType = typeof(short);
						dataColumn.DefaultValue = Function.ParseShort(0);
						break;
					case "tinyint":
						dataColumn.DataType = typeof(byte);
						dataColumn.DefaultValue = Function.ParseByte(0);
						break;
					case "bigint":
						dataColumn.DataType = typeof(int);
						dataColumn.DefaultValue = Function.ParseLong(0);
						break;
					case "int":
						dataColumn.DataType = typeof(int);
						dataColumn.DefaultValue = Function.ParseInt(0);
						break;
					case "bit":
						dataColumn.DataType = typeof(bool);
						dataColumn.DefaultValue = true;
						break;
					default:
						dataColumn.DataType = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					}
					ret.Columns.Add(dataColumn);
				}
			}
			ret.AcceptChanges();
			ret.Rows.Clear();
			DataRow row = ret.NewRow();
			ret.Rows.Add(row);
			ret.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckValidItemType(string code)
	{
		bool result = false;
		try
		{
			DataTable dataTable = _httpClient.BillOfMasterials.CheckValidItemType(code);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (Function.ToString(dataTable.Rows[0]["PrchseItem"]).ToUpper() == "N" && Function.ToString(dataTable.Rows[0]["InvntItem"]).ToUpper() == "N" && Function.ToString(dataTable.Rows[0]["SellItem"]).ToUpper() == "Y")
				{
					result = true;
				}
				else if (Function.ToString(dataTable.Rows[0]["PrchseItem"]).ToUpper() == "Y")
				{
					WinCommons.Error(Enums.MessageCode.OnlyProductionBomOrTemplateBomCanDefinedAsPurchaseItem, "");
				}
				else
				{
					WinCommons.Error((Function.ToString(dataTable.Rows[0]["InvntItem"]).ToUpper() == "Y") ? Enums.MessageCode.OnlyProductionBomCanDefinedAnInventoryPurchaseItem : Enums.MessageCode.ASalesBomMustDefinedASalesItem, "");
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ProductNoDoesNotExists, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private bool CheckValidComponents()
	{
		bool result = true;
		try
		{
			if (grdDoc.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (Function.ToString(dataTable.Rows[i]["Code"]).ToUpper() == Function.ToString(txtProductNo.Text).ToUpper() && !string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[i]["Code"])))
					{
						WinCommons.Error(Enums.MessageCode.ItemAlreadyDefinedAtHigherLevelInProductTreeBillOfMaterialWithLine, "", Function.ToString(i + 1));
						result = false;
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			result = false;
		}
		return result;
	}

	private DataTable GetItemPriceByPriceList(string itemCode, int priceList)
	{
		try
		{
			return _httpClient.BillOfMasterials.GetItemPriceByPriceList(itemCode, priceList);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private void DeleteDoc(string code)
	{
		try
		{
			string text = _httpClient.BillOfMasterials.DeleteDoc(code);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
				REFRESH();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetControls()
	{
		SetTextValue("", txtProductNo);
		SetTextValue("1", txtQuantity);
		SetTextValue("", txtProductDesc);
		_dtBomItem = new DataTable();
		DisableTextBoxes(isDisable: false);
	}

	private void DisableTextBoxes(bool isDisable)
	{
		if (isDisable)
		{
			SetReadOnlyText(isDisable: true, txtProductNo);
			SetVisibleButton(isVisible: false, txtProductNo);
		}
		else
		{
			SetReadOnlyText(isDisable: false, txtProductNo);
			SetVisibleButton(isVisible: true, txtProductNo);
		}
	}

	private void SetReadOnlyText(bool isDisable, TextEdit textBox)
	{
		if (textBox.InvokeRequired)
		{
			SetTextCallback method = SetReadOnlyText;
			Invoke(method, isDisable, textBox);
		}
		else if (isDisable)
		{
			textBox.ReadOnly = true;
			textBox.Properties.ReadOnly = true;
			textBox.BackColor = StyleColor.GridDisable;
			textBox.Properties.AppearanceFocused.BackColor = StyleColor.GridDisable;
		}
		else
		{
			textBox.ReadOnly = false;
			textBox.Properties.ReadOnly = false;
			textBox.BackColor = Color.White;
			textBox.Properties.AppearanceFocused.BackColor = StyleColor.ControlFocused;
		}
	}

	private void SetVisibleButton(bool isVisible, ButtonEdit btEdit)
	{
		if (btEdit.InvokeRequired)
		{
			SetVisibleButtonCallback method = SetVisibleButton;
			Invoke(method, isVisible, btEdit);
		}
		else
		{
			btEdit.Properties.Buttons[0].Visible = isVisible;
		}
	}

	private void SetTextValue(string value, TextEdit textBox)
	{
		if (textBox.InvokeRequired)
		{
			SetTextValueCallback method = SetTextValue;
			Invoke(method, value, textBox);
		}
		else
		{
			textBox.Text = value;
		}
	}

	private void RefreshGridDataSource(DataTable dt)
	{
		if (dt.IsNotNull())
		{
			foreach (DataRow row in dt.Rows)
			{
				if (Function.ParseInt(row["Type"]) == -18 && string.IsNullOrEmpty(Function.ToString(row["Code"])))
				{
					row["Code"] = row["LineText"];
				}
			}
			dt.AcceptChanges();
		}
		if (grdDoc.InvokeRequired)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				grdDoc.DataSource = dt;
				grdDoc.RefreshDataSource();
			}));
		}
		else
		{
			grdDoc.DataSource = dt;
			grdDoc.RefreshDataSource();
		}
		LineLastColumn = gvDoc.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvDoc.Columns.Max((GridColumn t) => t.VisibleIndex));
	}

	private void RefreshUdfGridDatasource()
	{
		try
		{
			DataTable dataTable = grd_UDF.DataSource as DataTable;
			if (dataTable != null)
			{
				dataTable.Rows.Clear();
			}
			else
			{
				dataTable = _dtUdf.Clone();
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				dataTable.Rows.Clear();
				grd_UDF.DataSource = dataTable;
				grd_UDF.RefreshDataSource();
				return;
			}
			bool flag = false;
			for (int i = 0; i < dataTable.Columns.Count; i++)
			{
				if (dataTable.Columns[i].ColumnName.ToUpper().StartsWith("U_"))
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
				grd_UDF.DataSource = dataTable;
				grd_UDF.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetFirst()
	{
		try
		{
			DataTable record = _httpClient.BillOfMasterials.GetRecord("F", "");
			if (record.Rows.Count > 0)
			{
				_first = record.Rows[0]["Code"].ToString();
				ShowData(record.Rows[0]);
				_current = _first;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetPrev(string value)
	{
		try
		{
			DataTable record = _httpClient.BillOfMasterials.GetRecord("P", value);
			if (record.Rows.Count > 0)
			{
				_current = value;
				ShowData(record.Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetNext(string value)
	{
		try
		{
			DataTable record = _httpClient.BillOfMasterials.GetRecord("N", value);
			if (record.Rows.Count > 0)
			{
				_current = value;
				ShowData(record.Rows[0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetLast()
	{
		try
		{
			DataTable record = _httpClient.BillOfMasterials.GetRecord("L", "");
			if (record.Rows.Count > 0)
			{
				_last = record.Rows[0]["Code"].ToString();
				ShowData(record.Rows[0]);
				_current = _last;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadLastItemCode()
	{
		try
		{
			DataTable lastRecord = _httpClient.BillOfMasterials.GetLastRecord();
			if (lastRecord.Rows.Count > 0)
			{
				_last = lastRecord.Rows[0]["Code"].ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadFirstItemCode()
	{
		try
		{
			DataTable firstRecord = _httpClient.BillOfMasterials.GetFirstRecord();
			if (firstRecord.Rows.Count > 0)
			{
				_first = firstRecord.Rows[0]["Code"].ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowData(DataRow dr)
	{
		try
		{
			_isUpdatePrice = false;
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			SetControlsBackColor();
			_current = dr["Code"].ToString();
			ResetControls();
			SetTextValue(Function.ToString(dr["Code"]), txtProductNo);
			SetTextValue(Function.ToString(dr["ItemName"]), txtProductDesc);
			SetTextValue(dr["Qauntity"].ToString(), txtQuantity);
			luk_Doc_BOMType.EditValue = Function.ToString(dr["TreeType"]);
			txt_PlAvgSize.EditValue = Function.ParseDouble(dr["PlAvgSize"]);
			string text = "";
			if (_activeDimCount > 0)
			{
				text = (string.IsNullOrWhiteSpace(Function.ToString(dr["OcrCode"])) ? (text + ";") : (text + Function.ToString(dr["OcrCode"]) + ";"));
			}
			for (int i = 2; i < _activeDimCount + 1; i++)
			{
				text = (string.IsNullOrWhiteSpace(Function.ToString(dr["OcrCode" + i])) ? (text + ";") : (text + Function.ToString(dr["OcrCode" + i]) + ";"));
			}
			if (text[text.Length - 1] == ';')
			{
				text = text.Remove(text.Length - 1);
			}
			if (text.Count((char t) => t == ';') == text.Length)
			{
				text = "";
			}
			while (text.Length > 0 && text[text.Length - 1].Equals(';'))
			{
				text = text.Remove(text.Length - 1);
			}
			cbDistrRule.EditValue = text;
			luk_PriceList.EditValue = Function.ParseShort(Function.ToString(dr["PriceList"]));
			bteWarehouse.EditValue = Function.ToString(dr["ToWH"]);
			txtProject.EditValue = Function.ToString(dr["Project"]);
			LoadBoMItemByItemCode(dr["Code"].ToString());
			DataTable itemPriceByPriceList = GetItemPriceByPriceList(_current, Function.ParseInt(Function.ToString(dr["PriceList"])));
			if (itemPriceByPriceList.IsNotNull())
			{
				calcPrice.EditValue = Function.ParseDouble(itemPriceByPriceList.Rows[0][0]);
				calcPrice.Currency = Function.ToString(itemPriceByPriceList.Rows[0][1]);
			}
			else
			{
				calcPrice.EditValue = 0;
				calcPrice.Currency = GlobalData.MainCurrency;
			}
			LoadDataUdf(dr);
			DisableTextBoxes(isDisable: true);
			AddNewRow();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataUdf(DataRow dr)
	{
		try
		{
			DataTable dataTable = new DataTable();
			for (int i = 0; i < dr.Table.Columns.Count; i++)
			{
				if (dr.Table.Columns[i].ColumnName.StartsWith("U_"))
				{
					dataTable.Columns.Add(new DataColumn(dr.Table.Columns[i].ColumnName, dr.Table.Columns[i].DataType));
				}
			}
			dataTable.AcceptChanges();
			if (dataTable.Columns.Count > 0)
			{
				DataRow dataRow = dataTable.NewRow();
				for (int j = 0; j < dr.Table.Columns.Count; j++)
				{
					if (dr.Table.Columns[j].ColumnName.StartsWith("U_"))
					{
						dataRow[dr.Table.Columns[j].ColumnName] = dr[j];
					}
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			else
			{
				gv_UDF.Columns.Clear();
			}
			grd_UDF.DataSource = dataTable;
			grd_UDF.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadItemDetails(int rowHandle)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetRowCellValue(rowHandle, "Code"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			string type = ((_curRowtype == 290) ? "R" : "I");
			DataTable itemDetails = GetItemDetails(type, text);
			if (itemDetails == null || itemDetails.Rows.Count <= 0)
			{
				return;
			}
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable != null)
			{
				DataRow dataRow = dataTable.Rows[gvDoc.GetDataSourceRowIndex(rowHandle)];
				DataRow dataRow2 = itemDetails.Rows[0];
				string value = Function.ToString(itemDetails.Rows[0]["DefaultWarehouse"]);
				if (_curRowtype == 290)
				{
					dataRow["ItemName"] = Function.ToString(dataRow2["ResName"]);
					dataRow["Code"] = Function.ToString(dataRow2["ResCode"]);
					dataRow["Father"] = txtProductNo.Text;
					dataRow["Price"] = 0;
					int num = 0;
					try
					{
						num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ChildNum"]));
					}
					catch (Exception)
					{
					}
					dataRow["ChildNum"] = num + 1;
					dataRow["PriceList"] = Convert.ToInt16(0);
					if (!string.IsNullOrWhiteSpace(value))
					{
						dataRow["Warehouse"] = value;
					}
					else
					{
						dataRow["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
					}
					dataRow["Total"] = 0;
					dataRow["Currency"] = GlobalData.MainCurrency;
					dataRow["Quantity"] = 1;
					dataRow["Type"] = _curRowtype;
					dataRow["IssueMthd"] = Function.ToString(dataRow2["IssueMthd"]);
					dataRow["WipActCode"] = "";
				}
				else
				{
					dataRow["ItemName"] = Function.ToString(dataRow2["ItemName"]);
					dataRow["Father"] = txtProductNo.Text;
					int num2 = 0;
					try
					{
						num2 = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["ChildNum"]));
					}
					catch (Exception)
					{
					}
					dataRow["ChildNum"] = num2 + 1;
					dataRow["PriceList"] = Function.ParseShort(luk_PriceList.EditValue).Value;
					DataTable itemPriceByPriceList = GetItemPriceByPriceList(text, Function.ParseInt(dataRow["PriceList"]));
					if (itemPriceByPriceList.IsNotNull())
					{
						dataRow["Price"] = itemPriceByPriceList.Rows[0][0];
						dataRow["Currency"] = itemPriceByPriceList.Rows[0][1];
					}
					dataRow["Total"] = dataRow["Price"];
					dataRow["Quantity"] = 1;
					if (!string.IsNullOrWhiteSpace(value))
					{
						dataRow["Warehouse"] = value;
					}
					else
					{
						dataRow["Warehouse"] = Function.ToString(bteWarehouse.EditValue);
					}
					dataRow["Currency"] = GlobalData.MainCurrency;
					dataRow["Type"] = _curRowtype;
					dataRow["IssueMthd"] = Function.ToString(dataRow2["IssueMthd"]);
					dataRow["WipActCode"] = "";
					dataRow["Uom"] = Function.ToString(dataRow2["InvntryUom"]);
				}
			}
			dataTable.AcceptChanges();
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex3.Message);
		}
	}

	private DataTable GetItemDetails(string type, string code)
	{
		try
		{
			return _httpClient.BillOfMasterials.GetItemDetails(type, code);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private void PasteFromExcelToDataTable(GridControl grd, GridView gv, List<string> key, int type)
	{
		_isPasteFromExcel = true;
		try
		{
			if (type != 290 && type != 4)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject != null)
			{
				string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
				if (dataObject.GetDataPresent(DataFormats.Text))
				{
					DataTable dataTable = Function.PasteFromExcel(clipboardData);
					if (dataTable != null && (dataTable.Rows.Count > 0 || dataTable.Columns.Count > 0))
					{
						int num = gv.FocusedRowHandle;
						GridColumn focusedColumn = gv.FocusedColumn;
						if (key.Contains(focusedColumn.FieldName))
						{
							for (int i = 0; i < dataTable.Rows.Count; i++)
							{
								if (!IsValidCode(type, Function.ToString(dataTable.Rows[i][0])))
								{
									WinCommons.Warning(Enums.MessageCode.ItemCodeDoesNotExistsWithCode, "", Function.ToString(dataTable.Rows[i][0]));
									break;
								}
								string text = Function.ToString(grd.DataSourceTable().Rows[num]["Code"]);
								int num2 = Function.ParseInt(grd.DataSourceTable().Rows[num]["ChildNum"]);
								if (num >= grd.DataSourceTable().Rows.Count)
								{
									AddNewRow();
								}
								int colunmIndex = focusedColumn.VisibleIndex;
								for (int j = 0; j < dataTable.Columns.Count; j++)
								{
									GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
									if (gridColumn == null)
									{
										break;
									}
									gv.SetRowCellValue(num, gridColumn, dataTable.Rows[i][j]);
									colunmIndex++;
								}
								if (text.Equals(Function.ToString(grd.DatasourceTable().Rows[num]["Code"])))
								{
									gv.SetRowCellValue(num, "ChildNum", num2);
								}
								gv.SetRowCellValue(num, "Type", type);
								grd.DataSourceTable().AcceptChanges();
								gv.FocusedRowHandle = num++;
							}
						}
						else
						{
							int focusedRowHandle = gv.FocusedRowHandle;
							for (int num3 = focusedRowHandle; num3 < gv.RowCount; num3++)
							{
								int colIndex = focusedColumn.VisibleIndex;
								bool flag = true;
								for (int num4 = 0; num4 < key.Count; num4++)
								{
									if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num3, key[num4]))))
									{
										flag = false;
										break;
									}
								}
								if (!flag || num3 - focusedRowHandle >= dataTable.Rows.Count)
								{
									break;
								}
								for (int num5 = 0; num5 < dataTable.Columns.Count; num5++)
								{
									GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
									if (gridColumn2 == null)
									{
										break;
									}
									gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num3 - focusedRowHandle][num5]);
									colIndex++;
								}
								num++;
							}
							grd.RefreshDataSource();
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		_isPasteFromExcel = false;
	}

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if ((Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "Type")) == 290 && e.Column == gcPriceList) || e.Column == gcDescription || e.Column == gcUom)
		{
			e.Appearance.BackColor = StyleColor.GridDisable;
		}
	}

	private void grdDoc_Paint(object sender, PaintEventArgs e)
	{
		try
		{
			DrawMergedCells(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void DrawMergedCells(PaintEventArgs e)
	{
		DataTable dataSourceTable = grdDoc.DataSourceTable;
		for (int i = 0; i < dataSourceTable.Rows.Count; i++)
		{
			if (Function.ParseInt(dataSourceTable.Rows[i]["Type"]) == -18)
			{
				GridViewExMergeCell cell = new GridViewExMergeCell(i, gcCode, LineLastColumn);
				painter.DrawMergedCell(cell, e);
			}
		}
	}

	private void uc_btnCSSetPrice_Click(object sender, EventArgs e)
	{
		try
		{
			double num = 0.0;
			for (int i = 0; i < gvDoc.DataRowCount; i++)
			{
				num += Function.ParseDouble(gvDoc.GetRowCellValue(i, "Total"));
			}
			calcPrice.EditValue = num;
			SetButtonUpdateText();
			_isUpdatePrice = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private void rep_luk_Type_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gvDoc.CloseEditor();
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("Type")) == -18)
			{
				foreach (GridColumn column in gvDoc.Columns)
				{
					if (column != gcType)
					{
						gvDoc.SetFocusedRowCellValue(column, null);
					}
				}
				frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
				frm_RowTextDetails2.RowText = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
				if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
				{
					gvDoc.SetRowCellValue(gvDoc.FocusedRowHandle, "Code", frm_RowTextDetails2.RowText);
				}
			}
			else
			{
				gvDoc.SetFocusedRowCellValue(gcCode, null);
				gvDoc.SetFocusedRowCellValue(gcPriceList, null);
			}
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public static bool IsValidCode(int type, string code)
	{
		bool result = false;
		try
		{
			DataTable dataTable = _httpClient.BillOfMasterials.IsValidCode(type, code);
			result = dataTable != null && dataTable.Rows.Count > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private void SetControlsBackColor()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				this.SetFindMode();
			}
			else
			{
				this.ResetData();
			}
			SetReadOnlyText(isDisable: true, txt_StandardCost);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetButtonUpdateText()
	{
		if (!_isLoading && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void LoadBoMFromGridItem()
	{
		try
		{
			string code = gvDoc.GetFocusedRowCellValue("Code").ToString();
			DataTable dataTable = await GetBoMByItemCode(code);
			if (dataTable.Rows.Count > 0)
			{
				ShowData(dataTable.Rows[0]);
			}
			else if (!string.IsNullOrEmpty(code))
			{
				frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, code);
				frm_ItemMasterData2.MdiParent = MdiParent;
				frm_ItemMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_BillOfMaterials));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txtProductNo = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Doc_BOMType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txtProductDesc = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.luk_PriceList = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.cbDistrRule = new Apzon.Controls.Editor.ButtonEditEx();
		this.lblWarehouse = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.txtQuantity = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.txtProject = new Apzon.Controls.Editor.ButtonEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_Type = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_calc = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcWarehouse = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBWarehouse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_priceList = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcIssueMthd = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_issueMtd = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.gcUom = new Apzon.Controls.Grid.GridColumnEx();
		this.gcComment = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWipActCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBAccount = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Project = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcChildNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFather = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.btAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.calcPrice = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.chkHideBom = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabP_Details = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tabP_UDF = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_UDF = new Apzon.Controls.Grid.GridControlEx();
		this.gv_UDF = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemComboBox3 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemButtonEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemComboBox2 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemComboBox1 = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
		this.repositoryItemButtonEdit5 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.contextMenuStrip_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.contextMenuItemVersion = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.viewDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.txt_StandardCost = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_PlAvgSize = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.uc_btnCSSetPrice = new Apzon.Forms.UserControls.Buttons.ucButtonCustomShape();
		this.label1 = new System.Windows.Forms.Label();
		this.bteWarehouse = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_BOMType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductDesc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbDistrRule.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtQuantity.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtProject.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_calc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBWarehouse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_priceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_issueMtd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBAccount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Project).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcPrice.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkHideBom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabP_Details.SuspendLayout();
		this.tabP_UDF.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit5).BeginInit();
		this.contextMenuStrip_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_StandardCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PlAvgSize.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bteWarehouse.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.labelLine1.EditControl = this.txtProductNo;
		this.labelLine1.EditValue = "Product No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(9, 7);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(188, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Product No.";
		this.txtProductNo.EditControl = null;
		this.txtProductNo.FieldName = null;
		this.txtProductNo.ImeMode = System.Windows.Forms.ImeMode.NoControl;
		this.txtProductNo.IsSearchControl = true;
		this.txtProductNo.Location = new System.Drawing.Point(185, 4);
		this.txtProductNo.Name = "txtProductNo";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txtProductNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtProductNo.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtProductNo_Properties_ButtonPressed);
		this.txtProductNo.SettingInVisible = false;
		this.txtProductNo.SettingReadonly = false;
		this.txtProductNo.Size = new System.Drawing.Size(146, 20);
		this.txtProductNo.TabIndex = 47;
		this.txtProductNo.EditValueChanged += new System.EventHandler(this.txtProductNo_EditValueChanged);
		this.txtProductNo.Enter += new System.EventHandler(this.txtProductNo_Enter);
		this.txtProductNo.Leave += new System.EventHandler(this.txtProductNo_Leave);
		this.labelLine2.EditControl = this.luk_Doc_BOMType;
		this.labelLine2.EditValue = "BOM Type";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(8, 51);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(188, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "BOM Type";
		this.luk_Doc_BOMType.EditControl = null;
		this.luk_Doc_BOMType.FieldName = null;
		this.luk_Doc_BOMType.IsSearchControl = true;
		this.luk_Doc_BOMType.Location = new System.Drawing.Point(185, 48);
		this.luk_Doc_BOMType.Name = "luk_Doc_BOMType";
		this.luk_Doc_BOMType.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_Doc_BOMType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Doc_BOMType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Doc_BOMType.Properties.DisplayMember = "Name";
		this.luk_Doc_BOMType.Properties.IsSearchControl = false;
		this.luk_Doc_BOMType.Properties.NullText = "";
		this.luk_Doc_BOMType.Properties.ShowFooter = false;
		this.luk_Doc_BOMType.Properties.ShowHeader = false;
		this.luk_Doc_BOMType.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWarehouses;
		this.luk_Doc_BOMType.Properties.ValueMember = "Code";
		this.luk_Doc_BOMType.SettingInVisible = false;
		this.luk_Doc_BOMType.SettingReadonly = false;
		this.luk_Doc_BOMType.Size = new System.Drawing.Size(146, 20);
		this.luk_Doc_BOMType.TabIndex = 54;
		this.luk_Doc_BOMType.EditValueChanged += new System.EventHandler(this.luk_Doc_BOMType_EditvalueChanged);
		this.labelLine3.EditControl = this.txtProductDesc;
		this.labelLine3.EditValue = "Product Description";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 28);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(188, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 2;
		this.labelLine3.Utext = "Product Description";
		this.txtProductDesc.EditControl = null;
		this.txtProductDesc.FieldName = null;
		this.txtProductDesc.IsSearchControl = true;
		this.txtProductDesc.Location = new System.Drawing.Point(185, 26);
		this.txtProductDesc.Name = "txtProductDesc";
		this.txtProductDesc.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.txtProductDesc.Properties.Appearance.Options.UseBackColor = true;
		this.txtProductDesc.SettingInVisible = false;
		this.txtProductDesc.SettingReadonly = false;
		this.txtProductDesc.Size = new System.Drawing.Size(286, 20);
		this.txtProductDesc.TabIndex = 11;
		this.labelLine4.EditControl = this.luk_PriceList;
		this.labelLine4.EditValue = "Price List";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(498, 30);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(96, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 5;
		this.labelLine4.Utext = "Price List";
		this.luk_PriceList.EditControl = null;
		this.luk_PriceList.FieldName = null;
		this.luk_PriceList.IsSearchControl = true;
		this.luk_PriceList.Location = new System.Drawing.Point(590, 27);
		this.luk_PriceList.Name = "luk_PriceList";
		this.luk_PriceList.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.luk_PriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PriceListName", "PriceListName")
		});
		this.luk_PriceList.Properties.DisplayMember = "PriceListName";
		this.luk_PriceList.Properties.IsSearchControl = false;
		this.luk_PriceList.Properties.NullText = "";
		this.luk_PriceList.Properties.NumOfItemShow = 0;
		this.luk_PriceList.Properties.ShowFooter = false;
		this.luk_PriceList.Properties.ShowHeader = false;
		this.luk_PriceList.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWarehouses;
		this.luk_PriceList.Properties.ValueMember = "PriceListNo";
		this.luk_PriceList.SettingInVisible = false;
		this.luk_PriceList.SettingReadonly = false;
		this.luk_PriceList.Size = new System.Drawing.Size(134, 20);
		this.luk_PriceList.TabIndex = 53;
		this.luk_PriceList.EditValueChanged += new System.EventHandler(this.luk_PriceList_EditValueChanged);
		this.labelLine5.EditControl = this.cbDistrRule;
		this.labelLine5.EditValue = "Distr Rule";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(499, 52);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(95, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 4;
		this.labelLine5.Utext = "Distr Rule";
		this.cbDistrRule.EditControl = null;
		this.cbDistrRule.FieldName = null;
		this.cbDistrRule.IsSearchControl = true;
		this.cbDistrRule.Location = new System.Drawing.Point(590, 49);
		this.cbDistrRule.Name = "cbDistrRule";
		this.cbDistrRule.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.cbDistrRule.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.comboBoxEdit1_Properties_ButtonPressed);
		this.cbDistrRule.SettingInVisible = false;
		this.cbDistrRule.SettingReadonly = false;
		this.cbDistrRule.Size = new System.Drawing.Size(134, 20);
		this.cbDistrRule.TabIndex = 21;
		this.cbDistrRule.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.cbDistrRule.Validating += new System.ComponentModel.CancelEventHandler(this.cbDistrRule_Validating);
		this.lblWarehouse.EditControl = null;
		this.lblWarehouse.EditValue = "Warehouse";
		this.lblWarehouse.FieldName = null;
		this.lblWarehouse.HideWhenEmpty = false;
		this.lblWarehouse.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblWarehouse.Location = new System.Drawing.Point(498, 8);
		this.lblWarehouse.Name = "lblWarehouse";
		this.lblWarehouse.ReadOnly = false;
		this.lblWarehouse.SettingInVisible = false;
		this.lblWarehouse.SettingReadonly = false;
		this.lblWarehouse.ShowUnderLine = true;
		this.lblWarehouse.Size = new System.Drawing.Size(96, 20);
		this.lblWarehouse.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblWarehouse.TabIndex = 3;
		this.lblWarehouse.Utext = "Warehouse";
		this.labelLine7.EditControl = this.txtQuantity;
		this.labelLine7.EditValue = "Quantity";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(352, 6);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(66, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 7;
		this.labelLine7.Utext = "Quantity";
		this.txtQuantity.EditControl = null;
		this.txtQuantity.FieldName = null;
		this.txtQuantity.IsSearchControl = true;
		this.txtQuantity.Location = new System.Drawing.Point(400, 3);
		this.txtQuantity.Name = "txtQuantity";
		this.txtQuantity.SettingInVisible = false;
		this.txtQuantity.SettingReadonly = false;
		this.txtQuantity.Size = new System.Drawing.Size(71, 20);
		this.txtQuantity.TabIndex = 15;
		this.txtQuantity.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.labelLine8.EditControl = this.txtProject;
		this.labelLine8.EditValue = "Project";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(498, 74);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(96, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 13;
		this.labelLine8.Utext = "Project";
		this.txtProject.EditControl = null;
		this.txtProject.FieldName = null;
		this.txtProject.IsSearchControl = true;
		this.txtProject.Location = new System.Drawing.Point(590, 71);
		this.txtProject.Name = "txtProject";
		this.txtProject.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtProject.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Project_Properties_ButtonClick);
		this.txtProject.SettingInVisible = false;
		this.txtProject.SettingReadonly = false;
		this.txtProject.Size = new System.Drawing.Size(134, 20);
		this.txtProject.TabIndex = 48;
		this.txtProject.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanging);
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rIBItemCode, this.rep_calc, this.rIBWarehouse, this.rIBAccount, this.rep_luk_priceList, this.rep_luk_Type, this.rep_luk_issueMtd, this.rep_bte_Project });
		this.grdDoc.Size = new System.Drawing.Size(716, 284);
		this.grdDoc.TabIndex = 16;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.Paint += new System.Windows.Forms.PaintEventHandler(this.grdDoc_Paint);
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuStrip_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteRowToolStripMenuItem, this.addRowToolStripMenuItem });
		this.contextMenuStrip_Line.Name = "contextMenuStrip1";
		this.contextMenuStrip_Line.Size = new System.Drawing.Size(131, 48);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.addRowToolStripMenuItem.Text = "Add row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.gcType, this.gcCode, this.gcDescription, this.gcQuantity, this.gcWarehouse, this.gcPrice, this.gcPriceList, this.gcIssueMthd, this.gcUom, this.gcComment,
			this.gcWipActCode, this.gcTotal, this.gcProject, this.gcChildNum, this.gcFather, this.gridColumn1
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oProductionTree";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.HasFormSetting = false;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ITT1";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.LstOldUDF = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsFind.AllowFindPanel = false;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowFooter = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.EndSorting += new System.EventHandler(this.gvDoc_EndSorting);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvDoc_CustomColumnDisplayText);
		this.gvDoc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvDoc_ValidatingEditor);
		this.gcType.Caption = "Type";
		this.gcType.ColumnEdit = this.rep_luk_Type;
		this.gcType.CurrencyDateField = null;
		this.gcType.CurrencyField = null;
		this.gcType.CurrencyFixed = null;
		this.gcType.CurrencyFixedRate = 0.0;
		this.gcType.CurrencyRateField = null;
		this.gcType.FieldName = "Type";
		this.gcType.Name = "gcType";
		this.gcType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcType.PrimaryKey = true;
		this.gcType.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcType.UnitField = null;
		this.gcType.Visible = true;
		this.gcType.VisibleIndex = 0;
		this.rep_luk_Type.AutoHeight = false;
		this.rep_luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_Type.DisplayMember = "Name";
		this.rep_luk_Type.IsSearchControl = false;
		this.rep_luk_Type.Name = "rep_luk_Type";
		this.rep_luk_Type.NullText = "";
		this.rep_luk_Type.ShowFooter = false;
		this.rep_luk_Type.ShowHeader = false;
		this.rep_luk_Type.ValueMember = "Code";
		this.rep_luk_Type.EditValueChanged += new System.EventHandler(this.rep_luk_Type_EditValueChanged);
		this.gcCode.Caption = "No.";
		this.gcCode.ColumnEdit = this.rIBItemCode;
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "Code";
		this.gcCode.Name = "gcCode";
		this.gcCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcCode.PrimaryKey = false;
		this.gcCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcCode.UnitField = null;
		this.gcCode.Visible = true;
		this.gcCode.VisibleIndex = 1;
		this.rIBItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rIBItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBItemCode.Name = "rIBItemCode";
		this.rIBItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBItemCode_ButtonPressed);
		this.rIBItemCode.Enter += new System.EventHandler(this.rIBItemCode_Enter);
		this.rIBItemCode.Leave += new System.EventHandler(this.rIBItemCode_Leave);
		this.gcDescription.AppearanceCell.BackColor = System.Drawing.Color.Black;
		this.gcDescription.AppearanceCell.BackColor2 = System.Drawing.Color.Black;
		this.gcDescription.Caption = "Description";
		this.gcDescription.CurrencyDateField = null;
		this.gcDescription.CurrencyField = null;
		this.gcDescription.CurrencyFixed = null;
		this.gcDescription.CurrencyFixedRate = 0.0;
		this.gcDescription.CurrencyRateField = null;
		this.gcDescription.FieldName = "ItemName";
		this.gcDescription.Name = "gcDescription";
		this.gcDescription.OptionsColumn.AllowEdit = false;
		this.gcDescription.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcDescription.OptionsColumn.ReadOnly = true;
		this.gcDescription.PrimaryKey = false;
		this.gcDescription.UnitField = null;
		this.gcDescription.Visible = true;
		this.gcDescription.VisibleIndex = 2;
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.ColumnEdit = this.rep_calc;
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.FieldName = "Quantity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.UnitField = null;
		this.gcQuantity.Visible = true;
		this.gcQuantity.VisibleIndex = 3;
		this.rep_calc.AutoHeight = false;
		this.rep_calc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_calc.Name = "rep_calc";
		this.gcWarehouse.Caption = "Warehouse";
		this.gcWarehouse.ColumnEdit = this.rIBWarehouse;
		this.gcWarehouse.CurrencyDateField = null;
		this.gcWarehouse.CurrencyField = null;
		this.gcWarehouse.CurrencyFixed = null;
		this.gcWarehouse.CurrencyFixedRate = 0.0;
		this.gcWarehouse.CurrencyRateField = null;
		this.gcWarehouse.FieldName = "Warehouse";
		this.gcWarehouse.Name = "gcWarehouse";
		this.gcWarehouse.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcWarehouse.PrimaryKey = false;
		this.gcWarehouse.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWarehouse.UnitField = null;
		this.gcWarehouse.Visible = true;
		this.gcWarehouse.VisibleIndex = 4;
		this.rIBWarehouse.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rIBWarehouse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBWarehouse.Name = "rIBWarehouse";
		this.rIBWarehouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBWarehouse_ButtonPressed);
		this.gcPrice.Caption = "Unit Price";
		this.gcPrice.CurrencyDateField = null;
		this.gcPrice.CurrencyField = null;
		this.gcPrice.CurrencyFixed = null;
		this.gcPrice.CurrencyFixedRate = 0.0;
		this.gcPrice.CurrencyRateField = null;
		this.gcPrice.DisplayFormat.FormatString = "n2";
		this.gcPrice.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcPrice.FieldName = "Price";
		this.gcPrice.Name = "gcPrice";
		this.gcPrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcPrice.PrimaryKey = false;
		this.gcPrice.UnitField = null;
		this.gcPrice.Visible = true;
		this.gcPrice.VisibleIndex = 8;
		this.gcPriceList.Caption = "Price List";
		this.gcPriceList.ColumnEdit = this.rep_luk_priceList;
		this.gcPriceList.CurrencyDateField = null;
		this.gcPriceList.CurrencyField = null;
		this.gcPriceList.CurrencyFixed = null;
		this.gcPriceList.CurrencyFixedRate = 0.0;
		this.gcPriceList.CurrencyRateField = null;
		this.gcPriceList.FieldName = "PriceList";
		this.gcPriceList.Name = "gcPriceList";
		this.gcPriceList.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcPriceList.PrimaryKey = false;
		this.gcPriceList.UnitField = null;
		this.gcPriceList.Visible = true;
		this.gcPriceList.VisibleIndex = 7;
		this.rep_luk_priceList.AutoHeight = false;
		this.rep_luk_priceList.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_priceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_priceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("PriceListName", "Name")
		});
		this.rep_luk_priceList.DisplayMember = "PriceListName";
		this.rep_luk_priceList.IsSearchControl = false;
		this.rep_luk_priceList.Name = "rep_luk_priceList";
		this.rep_luk_priceList.NullText = "";
		this.rep_luk_priceList.NumOfItemShow = 0;
		this.rep_luk_priceList.ShowFooter = false;
		this.rep_luk_priceList.ShowHeader = false;
		this.rep_luk_priceList.ValueMember = "PriceListNo";
		this.rep_luk_priceList.EditValueChanged += new System.EventHandler(this.rep_luk_priceList_EditValueChanged);
		this.gcIssueMthd.Caption = "Issue Method";
		this.gcIssueMthd.ColumnEdit = this.rep_luk_issueMtd;
		this.gcIssueMthd.CurrencyDateField = null;
		this.gcIssueMthd.CurrencyField = null;
		this.gcIssueMthd.CurrencyFixed = null;
		this.gcIssueMthd.CurrencyFixedRate = 0.0;
		this.gcIssueMthd.CurrencyRateField = null;
		this.gcIssueMthd.FieldName = "IssueMthd";
		this.gcIssueMthd.Name = "gcIssueMthd";
		this.gcIssueMthd.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcIssueMthd.PrimaryKey = false;
		this.gcIssueMthd.UnitField = null;
		this.gcIssueMthd.Visible = true;
		this.gcIssueMthd.VisibleIndex = 6;
		this.rep_luk_issueMtd.AutoHeight = false;
		this.rep_luk_issueMtd.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.rep_luk_issueMtd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_issueMtd.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_issueMtd.DisplayMember = "Name";
		this.rep_luk_issueMtd.IsSearchControl = false;
		this.rep_luk_issueMtd.Name = "rep_luk_issueMtd";
		this.rep_luk_issueMtd.NullText = "";
		this.rep_luk_issueMtd.ShowFooter = false;
		this.rep_luk_issueMtd.ShowHeader = false;
		this.rep_luk_issueMtd.ValueMember = "Code";
		this.gcUom.AppearanceCell.BackColor = System.Drawing.Color.Black;
		this.gcUom.AppearanceCell.BackColor2 = System.Drawing.Color.Black;
		this.gcUom.Caption = "UoM Name";
		this.gcUom.CurrencyDateField = null;
		this.gcUom.CurrencyField = null;
		this.gcUom.CurrencyFixed = null;
		this.gcUom.CurrencyFixedRate = 0.0;
		this.gcUom.CurrencyRateField = null;
		this.gcUom.FieldName = "Uom";
		this.gcUom.Name = "gcUom";
		this.gcUom.OptionsColumn.AllowEdit = false;
		this.gcUom.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcUom.OptionsColumn.ReadOnly = true;
		this.gcUom.PrimaryKey = false;
		this.gcUom.UnitField = null;
		this.gcUom.Visible = true;
		this.gcUom.VisibleIndex = 5;
		this.gcComment.Caption = "Comment";
		this.gcComment.CurrencyDateField = null;
		this.gcComment.CurrencyField = null;
		this.gcComment.CurrencyFixed = null;
		this.gcComment.CurrencyFixedRate = 0.0;
		this.gcComment.CurrencyRateField = null;
		this.gcComment.FieldName = "Comment";
		this.gcComment.Name = "gcComment";
		this.gcComment.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcComment.PrimaryKey = false;
		this.gcComment.UnitField = null;
		this.gcComment.Visible = true;
		this.gcComment.VisibleIndex = 10;
		this.gcWipActCode.Caption = "Wip Account";
		this.gcWipActCode.ColumnEdit = this.rIBAccount;
		this.gcWipActCode.CurrencyDateField = null;
		this.gcWipActCode.CurrencyField = null;
		this.gcWipActCode.CurrencyFixed = null;
		this.gcWipActCode.CurrencyFixedRate = 0.0;
		this.gcWipActCode.CurrencyRateField = null;
		this.gcWipActCode.FieldName = "WipActCode";
		this.gcWipActCode.Name = "gcWipActCode";
		this.gcWipActCode.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcWipActCode.PrimaryKey = false;
		this.gcWipActCode.UnitField = null;
		this.gcWipActCode.Visible = true;
		this.gcWipActCode.VisibleIndex = 11;
		this.rIBAccount.AutoHeight = false;
		this.rIBAccount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBAccount.Name = "rIBAccount";
		this.rIBAccount.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBAccount_ButtonPressed);
		this.gcTotal.Caption = "Total";
		this.gcTotal.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcTotal.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcTotal.CurrencyDateField = null;
		this.gcTotal.CurrencyField = null;
		this.gcTotal.CurrencyFixed = null;
		this.gcTotal.CurrencyFixedRate = 0.0;
		this.gcTotal.CurrencyRateField = null;
		this.gcTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcTotal.FieldName = "Total";
		this.gcTotal.Name = "gcTotal";
		this.gcTotal.OptionsColumn.AllowEdit = false;
		this.gcTotal.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcTotal.OptionsColumn.ReadOnly = true;
		this.gcTotal.PrimaryKey = false;
		this.gcTotal.UnitField = null;
		this.gcTotal.Visible = true;
		this.gcTotal.VisibleIndex = 9;
		this.gcProject.Caption = "Project";
		this.gcProject.ColumnEdit = this.rep_bte_Project;
		this.gcProject.CurrencyDateField = null;
		this.gcProject.CurrencyField = null;
		this.gcProject.CurrencyFixed = null;
		this.gcProject.CurrencyFixedRate = 0.0;
		this.gcProject.CurrencyRateField = null;
		this.gcProject.FieldName = "Project";
		this.gcProject.Name = "gcProject";
		this.gcProject.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcProject.PrimaryKey = false;
		this.gcProject.UnitField = null;
		this.gcProject.Visible = true;
		this.gcProject.VisibleIndex = 12;
		this.rep_bte_Project.AutoHeight = false;
		this.rep_bte_Project.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_bte_Project.Name = "rep_bte_Project";
		this.rep_bte_Project.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_bte_Project_ButtonPressed);
		this.gcChildNum.Caption = "Child Num";
		this.gcChildNum.CurrencyDateField = null;
		this.gcChildNum.CurrencyField = null;
		this.gcChildNum.CurrencyFixed = null;
		this.gcChildNum.CurrencyFixedRate = 0.0;
		this.gcChildNum.CurrencyRateField = null;
		this.gcChildNum.FieldName = "ChildNum";
		this.gcChildNum.Name = "gcChildNum";
		this.gcChildNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcChildNum.PrimaryKey = false;
		this.gcChildNum.UnitField = null;
		this.gcFather.Caption = "Father";
		this.gcFather.CurrencyDateField = null;
		this.gcFather.CurrencyField = null;
		this.gcFather.CurrencyFixed = null;
		this.gcFather.CurrencyFixedRate = 0.0;
		this.gcFather.CurrencyRateField = null;
		this.gcFather.FieldName = "Father";
		this.gcFather.Name = "gcFather";
		this.gcFather.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gcFather.PrimaryKey = false;
		this.gcFather.UnitField = null;
		this.gridColumn1.Caption = "Currency";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Currency";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.True;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.btAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btAdd.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btAdd.Appearance.Options.UseBackColor = true;
		this.btAdd.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btAdd.EditControl = null;
		this.btAdd.EditValue = "OK";
		this.btAdd.FieldName = null;
		this.btAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btAdd.ImageOptions.Image");
		this.btAdd.IsSearchControl = true;
		this.btAdd.Location = new System.Drawing.Point(5, 458);
		this.btAdd.Name = "btAdd";
		this.btAdd.ReadOnly = false;
		this.btAdd.SettingInVisible = false;
		this.btAdd.SettingReadonly = false;
		this.btAdd.Size = new System.Drawing.Size(75, 23);
		this.btAdd.TabIndex = 17;
		this.btAdd.Text = "OK";
		this.btAdd.Click += new System.EventHandler(this.btAdd_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btCancel.Appearance.Options.UseBackColor = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(86, 458);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 18;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.labelLine9.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine9.EditControl = this.calcPrice;
		this.labelLine9.EditValue = "Product Price";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(484, 465);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(110, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 19;
		this.labelLine9.Utext = "Product Price";
		this.calcPrice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.calcPrice.Currency = null;
		this.calcPrice.EditControl = null;
		this.calcPrice.FieldName = null;
		this.calcPrice.IsSearchControl = false;
		this.calcPrice.Location = new System.Drawing.Point(596, 462);
		this.calcPrice.Name = "calcPrice";
		this.calcPrice.Properties.Appearance.Options.UseTextOptions = true;
		this.calcPrice.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcPrice.SettingInVisible = false;
		this.calcPrice.SettingReadonly = false;
		this.calcPrice.Size = new System.Drawing.Size(120, 20);
		this.calcPrice.TabIndex = 62;
		this.calcPrice.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chkHideBom.EditControl = this.chkHideBom;
		this.chkHideBom.FieldName = null;
		this.chkHideBom.IsSearchControl = true;
		this.chkHideBom.Location = new System.Drawing.Point(10, 116);
		this.chkHideBom.Name = "chkHideBom";
		this.chkHideBom.Properties.Caption = "Hide BOM Components in Printout";
		this.chkHideBom.SettingInVisible = false;
		this.chkHideBom.SettingReadonly = false;
		this.chkHideBom.Size = new System.Drawing.Size(186, 20);
		this.chkHideBom.TabIndex = 46;
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(8, 142);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabP_Details;
		this.xtraTabControl1.Size = new System.Drawing.Size(718, 309);
		this.xtraTabControl1.TabIndex = 49;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabP_Details, this.tabP_UDF });
		this.tabP_Details.Controls.Add(this.grdDoc);
		this.tabP_Details.EditControl = null;
		this.tabP_Details.EditValue = "Details";
		this.tabP_Details.FieldName = "";
		this.tabP_Details.IsSearchControl = false;
		this.tabP_Details.Name = "tabP_Details";
		this.tabP_Details.ReadOnly = false;
		this.tabP_Details.SettingInVisible = false;
		this.tabP_Details.SettingReadonly = false;
		this.tabP_Details.Size = new System.Drawing.Size(716, 284);
		this.tabP_Details.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabP_Details.Text = "Details";
		this.tabP_UDF.Controls.Add(this.grd_UDF);
		this.tabP_UDF.EditControl = null;
		this.tabP_UDF.EditValue = "User Define Fields";
		this.tabP_UDF.FieldName = "";
		this.tabP_UDF.IsSearchControl = false;
		this.tabP_UDF.Name = "tabP_UDF";
		this.tabP_UDF.ReadOnly = false;
		this.tabP_UDF.SettingInVisible = false;
		this.tabP_UDF.SettingReadonly = false;
		this.tabP_UDF.Size = new System.Drawing.Size(716, 284);
		this.tabP_UDF.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabP_UDF.Text = "User Define Fields";
		this.grd_UDF.AllowPasteExcel = false;
		this.grd_UDF.ClipboardData = null;
		this.grd_UDF.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grd_UDF.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_UDF.Location = new System.Drawing.Point(0, 0);
		this.grd_UDF.MainView = this.gv_UDF;
		this.grd_UDF.Name = "grd_UDF";
		this.grd_UDF.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.repositoryItemComboBox3, this.repositoryItemButtonEdit2, this.repositoryItemCalcEdit2, this.repositoryItemButtonEdit3, this.repositoryItemComboBox2, this.repositoryItemButtonEdit4, this.repositoryItemComboBox1, this.repositoryItemButtonEdit5 });
		this.grd_UDF.Size = new System.Drawing.Size(716, 284);
		this.grd_UDF.TabIndex = 17;
		this.grd_UDF.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_UDF });
		this.gv_UDF.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_UDF.ColumnKey = null;
		this.gv_UDF.DisableFormSetting = false;
		this.gv_UDF.DisplayException = false;
		this.gv_UDF.EditColumn = null;
		this.gv_UDF.FieldCurrency = null;
		this.gv_UDF.FieldRate = null;
		this.gv_UDF.FormId = "oProductionTree";
		this.gv_UDF.GridControl = this.grd_UDF;
		this.gv_UDF.HasFormSetting = false;
		this.gv_UDF.IgnoreFilter = false;
		this.gv_UDF.IndexEditRow = -1;
		this.gv_UDF.IndicatorWidth = 35;
		this.gv_UDF.IsApzTable = false;
		this.gv_UDF.IsInsertNewRow = false;
		this.gv_UDF.IsLoadingData = false;
		this.gv_UDF.IsMappingContextAction = false;
		this.gv_UDF.ItemId = "OUDF";
		this.gv_UDF.LstColumnAllowBatch = null;
		this.gv_UDF.LstColumnNotAllowBatch = null;
		this.gv_UDF.LstColumnNotEditable = null;
		this.gv_UDF.LstColumNotSetting = null;
		this.gv_UDF.LstOldUDF = null;
		this.gv_UDF.Name = "gv_UDF";
		this.gv_UDF.OptionsView.ColumnAutoWidth = false;
		this.gv_UDF.OptionsView.ShowDetailButtons = false;
		this.gv_UDF.OptionsView.ShowFooter = true;
		this.gv_UDF.OptionsView.ShowGroupPanel = false;
		this.gv_UDF.UseBatchMod = false;
		this.gv_UDF.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Controls_EditValueChanged);
		this.repositoryItemComboBox3.AllowNullInput = DevExpress.Utils.DefaultBoolean.False;
		this.repositoryItemComboBox3.AutoHeight = false;
		this.repositoryItemComboBox3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox3.Name = "repositoryItemComboBox3";
		this.repositoryItemButtonEdit2.AutoHeight = false;
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.repositoryItemButtonEdit3.AutoHeight = false;
		this.repositoryItemButtonEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit3.Name = "repositoryItemButtonEdit3";
		this.repositoryItemComboBox2.AutoHeight = false;
		this.repositoryItemComboBox2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox2.Name = "repositoryItemComboBox2";
		this.repositoryItemButtonEdit4.AutoHeight = false;
		this.repositoryItemButtonEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit4.Name = "repositoryItemButtonEdit4";
		this.repositoryItemComboBox1.AutoHeight = false;
		this.repositoryItemComboBox1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemComboBox1.Name = "repositoryItemComboBox1";
		this.repositoryItemButtonEdit5.AutoHeight = false;
		this.repositoryItemButtonEdit5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repositoryItemButtonEdit5.Name = "repositoryItemButtonEdit5";
		this.contextMenuStrip_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.removeToolStripMenuItem, this.duplicateToolStripMenuItem, this.contextMenuItemVersion, this.toolStripSeparator1, this.viewDiagramToolStripMenuItem });
		this.contextMenuStrip_Doc.Name = "contextMenuStrip_Doc";
		this.contextMenuStrip_Doc.Size = new System.Drawing.Size(207, 98);
		this.contextMenuStrip_Doc.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip_Doc_Opening);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.contextMenuItemVersion.Name = "contextMenuItemVersion";
		this.contextMenuItemVersion.Size = new System.Drawing.Size(206, 22);
		this.contextMenuItemVersion.Text = "Advance Bill Of Materials";
		this.contextMenuItemVersion.Click += new System.EventHandler(this.contextMenuItemVersion_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(203, 6);
		this.viewDiagramToolStripMenuItem.Name = "viewDiagramToolStripMenuItem";
		this.viewDiagramToolStripMenuItem.Size = new System.Drawing.Size(206, 22);
		this.viewDiagramToolStripMenuItem.Text = "View Diagram";
		this.viewDiagramToolStripMenuItem.Click += new System.EventHandler(this.viewDiagramToolStripMenuItem_Click);
		this.txt_StandardCost.Currency = null;
		this.txt_StandardCost.EditControl = null;
		this.txt_StandardCost.FieldName = null;
		this.txt_StandardCost.IsSearchControl = true;
		this.txt_StandardCost.Location = new System.Drawing.Point(185, 70);
		this.txt_StandardCost.Name = "txt_StandardCost";
		this.txt_StandardCost.Properties.ReadOnly = true;
		this.txt_StandardCost.SettingInVisible = false;
		this.txt_StandardCost.SettingReadonly = false;
		this.txt_StandardCost.Size = new System.Drawing.Size(146, 20);
		this.txt_StandardCost.TabIndex = 55;
		this.labelLine6.EditControl = this.txt_StandardCost;
		this.labelLine6.EditValue = "Production Std Cost";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(8, 73);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = true;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(188, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 56;
		this.labelLine6.Utext = "Production Std Cost";
		this.txt_PlAvgSize.EditControl = null;
		this.txt_PlAvgSize.FieldName = null;
		this.txt_PlAvgSize.IsSearchControl = false;
		this.txt_PlAvgSize.Location = new System.Drawing.Point(185, 91);
		this.txt_PlAvgSize.Name = "txt_PlAvgSize";
		this.txt_PlAvgSize.Properties.IsSearchControl = false;
		this.txt_PlAvgSize.SettingInVisible = false;
		this.txt_PlAvgSize.SettingReadonly = false;
		this.txt_PlAvgSize.Size = new System.Drawing.Size(146, 20);
		this.txt_PlAvgSize.TabIndex = 57;
		this.txt_PlAvgSize.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine10.EditControl = this.txt_PlAvgSize;
		this.labelLine10.EditValue = "Planned Average Production Size";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(8, 94);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(188, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 58;
		this.labelLine10.Utext = "Planned Average Production Size";
		this.uc_btnCSSetPrice.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.uc_btnCSSetPrice.IsSearchControl = false;
		this.uc_btnCSSetPrice.Location = new System.Drawing.Point(710, 450);
		this.uc_btnCSSetPrice.Name = "uc_btnCSSetPrice";
		this.uc_btnCSSetPrice.Size = new System.Drawing.Size(14, 10);
		this.uc_btnCSSetPrice.TabIndex = 59;
		this.uc_btnCSSetPrice.TColor = System.Drawing.Color.DimGray;
		this.uc_btnCSSetPrice.TDirect = Apzon.Commons.Enums.Enums.ButtonCustomShapeDirection.Down;
		this.uc_btnCSSetPrice.TShape = Apzon.Commons.Enums.Enums.ButtonCustomShapeShape.Triangle;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Segoe UI Symbol", 12f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(333, 3);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(19, 21);
		this.label1.TabIndex = 61;
		this.label1.Text = "X";
		this.bteWarehouse.EditControl = null;
		this.bteWarehouse.FieldName = null;
		this.bteWarehouse.IsSearchControl = true;
		this.bteWarehouse.Location = new System.Drawing.Point(590, 5);
		this.bteWarehouse.Name = "bteWarehouse";
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.bteWarehouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteWarehouse.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_Warehouse_Properties_ButtonClick);
		this.bteWarehouse.SettingInVisible = false;
		this.bteWarehouse.SettingReadonly = false;
		this.bteWarehouse.Size = new System.Drawing.Size(134, 20);
		this.bteWarehouse.TabIndex = 63;
		this.bteWarehouse.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btCancel;
		base.ClientSize = new System.Drawing.Size(736, 486);
		this.ContextMenuStrip = this.contextMenuStrip_Doc;
		base.Controls.Add(this.bteWarehouse);
		base.Controls.Add(this.calcPrice);
		base.Controls.Add(this.txt_PlAvgSize);
		base.Controls.Add(this.labelLine10);
		base.Controls.Add(this.txt_StandardCost);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.luk_Doc_BOMType);
		base.Controls.Add(this.luk_PriceList);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.txtProject);
		base.Controls.Add(this.cbDistrRule);
		base.Controls.Add(this.txtProductNo);
		base.Controls.Add(this.chkHideBom);
		base.Controls.Add(this.labelLine9);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btAdd);
		base.Controls.Add(this.txtQuantity);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.txtProductDesc);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.lblWarehouse);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.uc_btnCSSetPrice);
		base.Controls.Add(this.label1);
		base.FormId = "oProductionTree";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_BillOfMaterials.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BillOfMaterials";
		this.Text = "Bill of Materials";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Doc_BOMType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProductDesc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbDistrRule.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtQuantity.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtProject.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_calc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBWarehouse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_priceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_issueMtd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBAccount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Project).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcPrice.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkHideBom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabP_Details.ResumeLayout(false);
		this.tabP_UDF.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_UDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemComboBox1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit5).EndInit();
		this.contextMenuStrip_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_StandardCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PlAvgSize.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bteWarehouse.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
