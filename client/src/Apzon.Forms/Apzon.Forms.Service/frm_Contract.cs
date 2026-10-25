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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_Contract : XtraFormEx
{
	private int _ContractID = -1;

	private int _OldContactID;

	private bool _OpenChooseCFL;

	private bool _isLoadData;

	private bool _isFocusRow;

	private bool _isUpdateFile;

	private bool _changeType;

	private bool _isLoadcoverage;

	private bool _fl;

	private bool _isCancel;

	private bool _forceClose;

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private DataView clone;

	private DataTable _dtContact = new DataTable();

	private const string FormController = "ServiceContract";

	public List<string> Params = new List<string>();

	private IContainer components;

	private SplitContainerControl spl_Content;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private XtraTabControlEx tab_Content;

	private XtraTabPageEx tab_Items;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btRefresh;

	public SimpleButtonEx btnCancel;

	private PanelControl panelControl2;

	private XtraTabPageEx tab_Coverage;

	private XtraTabPageEx tab_Attachments;

	private XtraTabPageEx tab_ServiceCalls;

	private XtraTabPageEx tab_SalesData;

	private GridControlEx grd_Items;

	private GridViewEx gv_Items;

	private GridControlEx grd_ServiceCall;

	private GridViewEx gv_ServiceCall;

	private PanelControl pnl_Coverage;

	private CheckEditEx chk_Including;

	private PanelControl panelControl4;

	private CheckEditEx chk_Travel;

	private CheckEditEx chk_Labor;

	private CheckEditEx chk_Parts;

	private LabelLine labelLine1;

	private DateEditEx date_SunE;

	private DateEditEx date_FriE;

	private DateEditEx date_SatE;

	private DateEditEx date_SunS;

	private DateEditEx date_ThuE;

	private DateEditEx date_WenE;

	private DateEditEx date_ThuS;

	private DateEditEx date_FriS;

	private DateEditEx date_SatS;

	private DateEditEx date_MonS;

	private LabelControl lbl_EndTime;

	private LabelControl lbl_StartTime;

	private CheckEditEx chk_Sun;

	private CheckEditEx chk_Sat;

	private CheckEditEx chk_Fri;

	private CheckEditEx chk_Thu;

	private CheckEditEx chk_Wed;

	private CheckEditEx chk_Tue;

	private CheckEditEx chk_Mon;

	private GridColumnEx cl_ContractNo;

	private GridColumnEx cl_CustomerCode;

	private RepositoryItemButtonEdit rp_Customer;

	private GridColumnEx cl_CustomerName;

	private GridColumnEx cl_ContactPerson;

	private RepositoryItemLookUpEdit rep_ContactPersonCode;

	private GridColumnEx cl_TelephoneNo;

	private RepositoryItemDateEdit rep_Date;

	private GridColumnEx cl_TerminationDate;

	private GridColumnEx cl_Description;

	private LabelLine lbl_StartDate;

	private LabelLine lbl_EndDate;

	public SimpleButtonEx btnSearch;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem ctm_Menu_Add;

	private DateEditEx date_WenS;

	private DateEditEx date_TueE;

	private DateEditEx date_TueS;

	private DateEditEx date_MonE;

	private GridColumnEx cl_ItemNo;

	private GridColumnEx cl_ItemDescription;

	private GridColumnEx cl_MfrSerialNo;

	private GridColumnEx cl_SerialNumber;

	private GridColumnEx cl_LineStartDate;

	private GridColumnEx cl_LineEndDate;

	private GridColumnEx cl_LineTerminationDate;

	private GridColumnEx cl_LineContractID;

	private RepositoryItemButtonEdit rep_ItemCode;

	private RepositoryItemDateEdit rep_LineDate;

	private RepositoryItemButtonEdit rep_SerialNumber;

	private PanelControl panelControl5;

	private GridColumnEx cl_CallID;

	private GridColumnEx cl_CreationDate;

	private GridColumnEx cl_Subject;

	private GridColumnEx cl_SC_ItemNo;

	private GridColumnEx cl_SN;

	private GridColumnEx cl_SC_CustomerName;

	private GridColumnEx cl_SC_Status;

	private PanelControl panelControl6;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_RecTemp;

	private GridViewEx gv_RecTemp;

	private LabelLine lbl_RecurringTemp;

	private GridControlEx grd_RecTrans;

	private GridViewEx gv_RecTrans;

	private LabelLine lbl_RecurringTransactions;

	private GridColumnEx cl_Template;

	private GridColumnEx cl_Type;

	private GridColumnEx cl_RecTran_Template;

	private GridColumnEx cl_Instance;

	private GridColumnEx cl_RecTran_Instance;

	private GridColumnEx cl_RecTran_Date;

	private GridColumnEx cl_DocTotal;

	private GridColumnEx cl_LineItem;

	private GridColumnEx cl_ItemGroup;

	private RepositoryItemLookUpEdit rlp_ItemGroup;

	private GridColumnEx cl_AtcEntry;

	private ContextMenuStrip ctm_Item;

	private ToolStripMenuItem del_ctm_Item;

	private GridColumnEx cl_SrvcType;

	private GridColumnEx cl_CntrcType;

	private GridColumnEx cl_CntrcTmplt;

	private GridColumnEx cl_ResponseTime;

	private GridColumnEx cl_ResponseUnit;

	private GridColumnEx cl_ResolutionTime;

	private GridColumnEx cl_ResolutionUnit;

	private GridColumnEx cl_Remarks;

	private GridColumnEx cl_Status;

	private GridColumnEx cl_Owner;

	private GridColumnEx cl_Renewal;

	private GridColumnEx cl_RemTime;

	private GridColumnEx cl_RemUnit;

	private GridColumnEx cl_ActiveItems;

	private GridColumnEx cl_CTRemarks;

	private RepositoryItemLookUpEdit rep_SrvcType;

	private RepositoryItemLookUpEdit rep_CntrcType;

	private RepositoryItemLookUpEdit rep_ReponseU;

	private RepositoryItemLookUpEdit rep_Status;

	private RepositoryItemLookUpEdit rep_Owner;

	private RepositoryItemCheckEdit rep_Renewal;

	private LabelLine lblServiceType;

	private LabelLine lbl_ContractType;

	private LabelLine lbl_Template;

	private LabelLine lbl_Resoulution;

	private LabelLine lbl_ResponseTime;

	private LabelLine Remarks;

	private ComboBoxEditEx cbb_SrvcType;

	private ComboBoxEditEx cbb_cntrc;

	private LabelLine lbl_Status;

	private ComboBoxEditEx cbb_Status;

	private LabelLine lbl_Owner;

	private ComboBoxEditEx cbb_Owner;

	private TextEditEx txt_Template;

	private CheckEditEx chk_Renewal;

	private LabelLine lbl_Reminder;

	private LabelLine lbl_ActiveItems;

	private LabelLine labelLine10;

	private MemoEditEx txt_Remark;

	private MemoEditEx txt_ContractTemp;

	private ComboBoxEditEx cbb_ResP;

	private ComboBoxEditEx cbb_TimeResO;

	private ComboBoxEditEx cbb_Remind;

	private TextEditEx txt_ActiveItems;

	private SpinEdit spin_Resp;

	private SpinEdit spin_ResO;

	private SpinEdit spin_Remind;

	private RepositoryItemLookUpEdit rep_RemU;

	private RepositoryItemMemoExEdit rep_Remarks2;

	private RepositoryItemMemoExEdit rep_Remarks1;

	public GridColumnEx cl_StartDate;

	public GridColumnEx cl_EndDate;

	private ucAttachment ucAttachment1;

	private RepositoryItemButtonEditCFL rep_ItemName;

	private DateEditEx date_Start;

	private DateEditEx date_EndDate;

	private RepositoryItemButtonEditCFL rep_Template;

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
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_Contract()
	{
		InitializeComponent();
		gv_Items.KeyDown += KeyDownPaste;
		grd_Items.EditorKeyDown += KeyDownPaste;
		OnLoadAsync += frm_Contract_Load;
		FormId = "oContract";
		gv_Doc.FormId = FormId;
		gv_Items.FormId = FormId;
		gv_Doc.ItemId = "OCTR";
		gv_Items.ItemId = "CTR1";
	}

	private async Task frm_Contract_Load(object sender, EventArgs e)
	{
		try
		{
			cl_StartDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			cl_EndDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			cl_TerminationDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_Date.Mask.EditMask = GlobalData.DateTimeFormat;
			cl_LineStartDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			cl_LineEndDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			cl_LineTerminationDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rep_LineDate.Mask.EditMask = GlobalData.DateTimeFormat;
			cl_CreationDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			ucAttachment1.gcDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			date_Start.Properties.Mask.EditMask = GlobalData.DateTimeFormat;
			date_Start.Properties.Mask.UseMaskAsDisplayFormat = true;
			date_EndDate.Properties.Mask.EditMask = GlobalData.DateTimeFormat;
			date_EndDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			await LoadData();
			AddHandlers();
			_OpenChooseCFL = true;
			rep_ItemName.ValueMember = "ItemName";
			rep_ItemName.Buttons[1].Visible = false;
			rep_ItemName.FindCFLBy(Enums.TypeCFL.ListItems, false, "", Enums.ItemObjectType.NoItemType, null);
			rep_ItemName.ListMandatoryField = "ItemDescription";
			rep_ItemName.HandleChooseItem = FillColumns;
			frm_Contract frm_Contract2 = this;
			GridViewEx gridViewEx = gv_Doc;
			GridViewEx gridViewEx2 = gv_ServiceCall;
			GridViewEx gridViewEx3 = gv_Items;
			string text = (ucAttachment1.gv_File.FormId = "oServiceContract");
			string text3 = (gridViewEx3.FormId = text);
			string text5 = (gridViewEx2.FormId = text3);
			string formId = (gridViewEx.FormId = text5);
			frm_Contract2.FormId = formId;
			gv_Doc.ItemId = "OCTR";
			ucAttachment1.gv_File.ItemId = "CTR2";
			gv_ServiceCall.ItemId = "OSCL";
			gv_Items.ItemId = "CTR1";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			_ContractID = 0;
			WinCommons._OpenProcessing();
			AddNew();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			DataTable dataTable = await Search();
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				_ContractID = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.TopRowIndex, "ContractID"));
				LoadAllTabs(await GetDetail(_ContractID));
			}
			else
			{
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Renewal")) == "Y")
			{
				RepositoryItemTextEdit repositoryItem = new RepositoryItemTextEdit
				{
					NullText = "",
					ReadOnly = false
				};
				if (e.Column.FieldName == "RemindVal")
				{
					e.RepositoryItem = repositoryItem;
				}
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
				{
					NullText = "",
					DisplayMember = "Name",
					ValueMember = "Code",
					ReadOnly = false
				};
				List<Common.Table_Code> dataSource = new List<Common.Table_Code>
				{
					new Common.Table_Code
					{
						Code = "D",
						Name = "Day(s)"
					},
					new Common.Table_Code
					{
						Code = "W",
						Name = "Week(s)"
					},
					new Common.Table_Code
					{
						Code = "M",
						Name = "Month(s)"
					}
				};
				if (e.Column.FieldName == "RemindUnit")
				{
					repositoryItemLookUpEdit.DataSource = dataSource;
					e.RepositoryItem = repositoryItemLookUpEdit;
				}
			}
			else
			{
				RepositoryItemTextEdit repositoryItem2 = new RepositoryItemTextEdit
				{
					NullText = "",
					ReadOnly = true
				};
				if (e.Column.FieldName == "RemindVal")
				{
					e.RepositoryItem = repositoryItem2;
				}
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
				{
					NullText = "",
					DisplayMember = "Name",
					ValueMember = "Code",
					ReadOnly = true
				};
				List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
				{
					new Common.Table_Code
					{
						Code = "D",
						Name = "Day(s)"
					},
					new Common.Table_Code
					{
						Code = "W",
						Name = "Week(s)"
					},
					new Common.Table_Code
					{
						Code = "M",
						Name = "Month(s)"
					}
				};
				if (e.Column.FieldName == "RemindUnit")
				{
					repositoryItemLookUpEdit2.DataSource = dataSource2;
					e.RepositoryItem = repositoryItemLookUpEdit2;
				}
			}
			if (e.Column.FieldName != "CntctCode")
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "CstmrCode"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit3 = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "CntctName",
				ValueMember = "CntctCode"
			};
			if (_dtContact.IsNotNull())
			{
				DataRow[] array = _dtContact.Select($"CardCode='{text}'");
				if (array.Length != 0)
				{
					repositoryItemLookUpEdit3.DataSource = array.CopyToDataTable();
				}
				repositoryItemLookUpEdit3.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
				repositoryItemLookUpEdit3.Columns.Add(new LookUpColumnInfo("CntctName", 20, "Name"));
				e.RepositoryItem = repositoryItemLookUpEdit3;
				e.RepositoryItem.EditValueChanging += rep_ContactPersonCode_EditValueChanging;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "Renewal")) == "Y")
			{
				RepositoryItemTextEdit repositoryItem = new RepositoryItemTextEdit
				{
					NullText = "",
					ReadOnly = false
				};
				if (e.Column.FieldName == "RemindVal")
				{
					e.RepositoryItem = repositoryItem;
				}
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
				{
					NullText = "",
					DisplayMember = "Name",
					ValueMember = "Code",
					ReadOnly = false
				};
				List<Common.Table_Code> dataSource = new List<Common.Table_Code>
				{
					new Common.Table_Code
					{
						Code = "D",
						Name = "Day(s)"
					},
					new Common.Table_Code
					{
						Code = "W",
						Name = "Week(s)"
					},
					new Common.Table_Code
					{
						Code = "M",
						Name = "Month(s)"
					}
				};
				if (e.Column.FieldName == "RemindUnit")
				{
					repositoryItemLookUpEdit.DataSource = dataSource;
					e.RepositoryItem = repositoryItemLookUpEdit;
				}
			}
			else
			{
				RepositoryItemTextEdit repositoryItem2 = new RepositoryItemTextEdit
				{
					NullText = "",
					ReadOnly = true
				};
				if (e.Column.FieldName == "RemindVal")
				{
					e.RepositoryItem = repositoryItem2;
				}
				RepositoryItemLookUpEdit repositoryItemLookUpEdit2 = new RepositoryItemLookUpEdit
				{
					NullText = "",
					DisplayMember = "Name",
					ValueMember = "Code",
					ReadOnly = true
				};
				List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
				{
					new Common.Table_Code
					{
						Code = "D",
						Name = "Day(s)"
					},
					new Common.Table_Code
					{
						Code = "W",
						Name = "Week(s)"
					},
					new Common.Table_Code
					{
						Code = "M",
						Name = "Month(s)"
					}
				};
				if (e.Column.FieldName == "RemindUnit")
				{
					repositoryItemLookUpEdit2.DataSource = dataSource2;
					e.RepositoryItem = repositoryItemLookUpEdit2;
				}
			}
			if (e.Column.FieldName != "CntctCode")
			{
				return;
			}
			string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "CstmrCode"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit3 = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "CntctName",
				ValueMember = "CntctCode"
			};
			if (_dtContact.IsNotNull())
			{
				DataRow[] array = _dtContact.Select($"CardCode='{text}'");
				if (array.Length != 0)
				{
					repositoryItemLookUpEdit3.DataSource = array.CopyToDataTable();
				}
				repositoryItemLookUpEdit3.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
				repositoryItemLookUpEdit3.Columns.Add(new LookUpColumnInfo("CntctName", 20, "Name"));
				e.RepositoryItem = repositoryItemLookUpEdit3;
				e.RepositoryItem.EditValueChanging += rep_ContactPersonCode_EditValueChanging;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void gv_Items_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("Status")) == "T")
			{
				if (gv_Doc.FocusedColumn == gv_Doc.Columns["CstmrCode"])
				{
					rp_Customer.ReadOnly = true;
				}
				else
				{
					e.Cancel = true;
				}
			}
			else if (gv_Doc.FocusedColumn == gv_Doc.Columns["CstmrCode"])
			{
				rp_Customer.ReadOnly = false;
			}
			if (gv_Doc.FocusedColumn.FieldName == "Status")
			{
				e.Cancel = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Items_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("Status")) == "T")
			{
				if (gv_Items.FocusedColumn == gv_Items.Columns["ItemCode"])
				{
					rep_ItemCode.ReadOnly = true;
				}
				else
				{
					e.Cancel = true;
				}
			}
			else if (gv_Items.FocusedColumn == gv_Items.Columns["ItemCode"])
			{
				rep_ItemCode.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Contract_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (!_isCancel && FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btn_Save_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_EndDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = date_Start.DateTime;
			if (date_EndDate.DateTime < dateTime)
			{
				WinCommons.writeStatus(Enums.MessageCode.EndDateGreaterStartDate, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				date_EndDate.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ContractID")) == 0 || FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			if (_isFocusRow)
			{
				_isFocusRow = false;
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Cancel)
				{
					for (int i = 0; i < gv_Doc.DataRowCount; i++)
					{
						object rowCellValue = gv_Doc.GetRowCellValue(i, "ContractID");
						if (rowCellValue != null && Function.ParseInt(rowCellValue) == _ContractID)
						{
							_isFocusRow = true;
							gv_Doc.FocusedRowHandle = i;
						}
					}
					return;
				}
				_isFocusRow = false;
				if (FormMode != Enums.FormViewModeType.Add)
				{
					FormMode = Enums.FormViewModeType.View;
				}
			}
			else
			{
				_isFocusRow = false;
				if (FormMode != Enums.FormViewModeType.Add)
				{
					FormMode = Enums.FormViewModeType.View;
				}
			}
			WinCommons._OpenProcessing();
			_ContractID = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("ContractID"));
			LoadAllTabs(await GetDetail(_ContractID));
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Status"));
			if (FormMode == Enums.FormViewModeType.View && text == "T")
			{
				pnl_Coverage.Enabled = false;
				grd_ServiceCall.Enabled = false;
				grd_RecTemp.Enabled = false;
				grd_RecTrans.Enabled = false;
				ucAttachment1.EnableButtons(isEnabled: false);
				ctm_Item.Enabled = false;
			}
			else
			{
				pnl_Coverage.Enabled = true;
				grd_ServiceCall.Enabled = true;
				grd_RecTemp.Enabled = true;
				grd_RecTrans.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				ctm_Item.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_changeType)
			{
				if (FormMode != Enums.FormViewModeType.Add && _isLoadData && _ContractID != 0)
				{
					btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				}
				_changeType = false;
				return;
			}
			if (FormMode != Enums.FormViewModeType.Add && _isLoadData && _ContractID != 0)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode != Enums.FormViewModeType.Add || e.Column.FieldName != "EndDate")
			{
				return;
			}
			DataTable dataTable = grd_Items.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["EndDate"] = e.Value;
				}
				dataTable.AcceptChanges();
				grd_Items.DataSource = dataTable;
				grd_Items.RefreshDataSource();
			}
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
			if (FormMode != Enums.FormViewModeType.Add && _isLoadData && _ContractID != 0)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Status_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!Function.ToString(e.NewValue).Equals("T"))
			{
				gv_Doc.SetFocusedRowCellValue("TermDate", DBNull.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_ContactPersonCode_EditValueChanged(object sender, EventArgs e)
	{
	}

	private async void rep_ContactPersonCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			int cntctCode = Function.ParseInt(e.NewValue);
			DataTable dataTable = await GetContactPerson();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				DataTable dataTable2 = (from r in dataTable.AsEnumerable()
					where r.Field<int>("CntctCode") == cntctCode
					select r).CopyToDataTable();
				if (dataTable2.IsNotNull())
				{
					gv_Doc.SetFocusedRowCellValue("TelephoneNo", Function.ToString(dataTable2.Rows[0]["Tel1"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rp_Customer_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && !(Function.ToString(gv_Doc.GetFocusedRowCellValue("Status")) == "T"))
			{
				int focusedRowHandle = gv_Doc.FocusedRowHandle;
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(focusedRowHandle, "ContractID"));
				if (num > 0 && !num.Equals(_ContractID))
				{
					return;
				}
				string cardcode = Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "CstmrCode"));
				ChooseFormListCardCode(cardcode);
			}
			if (e.Button.Index == 1)
			{
				int focusedRowHandle2 = gv_Doc.FocusedRowHandle;
				string text = Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle2, "CstmrCode"));
				if (!string.IsNullOrEmpty(text))
				{
					frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
					frm_BusinessMasterData2._copyF_CodeCustomer = text;
					frm_BusinessMasterData2.MdiParent = MdiParent;
					frm_BusinessMasterData2.Show();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await Search();
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				_ContractID = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.TopRowIndex, "ContractID"));
				LoadAllTabs(await GetDetail(_ContractID));
			}
			else
			{
				grd_Doc.DataSource = new DataTable();
				grd_Doc.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btRefresh_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await Search();
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				_ContractID = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.TopRowIndex, "ContractID"));
				LoadAllTabs(await GetDetail(_ContractID));
			}
			else
			{
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ctm_Menu_Add_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			_ContractID = 0;
			AddNew();
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
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				if (FormMode == Enums.FormViewModeType.View || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				DataTable valueForm_Doc = GetValueForm_Doc();
				DataTable dataSourceTable = grd_Items.DataSourceTable;
				dataSourceTable.AcceptChanges();
				if (!ucAttachment1.GetAttachmentNoSAP(out var dt))
				{
					return;
				}
				DataTable dataTable = new DataTable();
				if (dataSourceTable != null)
				{
					dataTable = dataSourceTable.Copy();
					DataRow[] array = dataTable.Select("ItemCode = '' and  ItemGroup = 0");
					if (array.Length != 0)
					{
						DataRow[] array2 = array;
						foreach (DataRow row in array2)
						{
							dataTable.Rows.Remove(row);
						}
						dataTable.AcceptChanges();
					}
				}
				DataSet data = new DataSet();
				valueForm_Doc.TableName = "Doc";
				data.Tables.Add(valueForm_Doc.Copy());
				if (dataTable != null)
				{
					dataTable.TableName = "Line";
					data.Tables.Add(dataTable.Copy());
				}
				if (dt.IsNotNull())
				{
					dt.TableName = "File";
					data.Tables.Add(dt.Copy());
				}
				await SaveDoc(data);
				data.Clear();
				data.Dispose();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Doc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!(sender is GridControl gridControl))
			{
				return;
			}
			GridView gridView = gridControl.FocusedView as GridView;
			if (e.KeyData != Keys.Tab || gridView == null || !(gridView.FocusedColumn.FieldName.ToUpper() == "CSTMRCODE") || !_OpenChooseCFL)
			{
				return;
			}
			int focusedRowHandle = gv_Doc.FocusedRowHandle;
			if (focusedRowHandle > -1)
			{
				string text = Function.ToString(gv_Doc.GetRowCellValue(focusedRowHandle, "CstmrCode"));
				if (string.IsNullOrEmpty(text))
				{
					ChooseFormListCardCode(text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			if (sender is GridView gridView && gridView.FocusedColumn.FieldName == "CntctCode" && gridView.ActiveEditor is LookUpEdit)
			{
				LookUpEdit lookUpEdit = (LookUpEdit)gridView.ActiveEditor;
				if (lookUpEdit != null)
				{
					DataTable table = lookUpEdit.Properties.DataSource as DataTable;
					clone = new DataView(table)
					{
						RowFilter = "[CardCode] = '" + Function.ToString(gv_Doc.GetFocusedRowCellValue("CstmrCode")) + "'"
					};
					lookUpEdit.Properties.DataSource = clone;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_HiddenEditor(object sender, EventArgs e)
	{
		try
		{
			if (clone != null)
			{
				clone.Dispose();
				clone = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void del_ctm_Item_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Items.RowCount > 1)
			{
				gv_Items.DeleteSelectedRows();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else if (gv_Items.RowCount == 1)
			{
				gv_Items.DeleteSelectedRows();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				AddNewRowItems();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && !(Function.ToString(gv_Doc.GetFocusedRowCellValue("Status")) == "T"))
			{
				int focusedRowHandle = gv_Items.FocusedRowHandle;
				if (focusedRowHandle > -1)
				{
					_OpenChooseCFL = false;
					if (_OpenChooseCFL)
					{
						_OpenChooseCFL = true;
						return;
					}
					string itemCode = Function.ToString(gv_Items.GetRowCellValue(focusedRowHandle, "ItemCode"));
					ChooseFormListItemCode(itemCode, focusedRowHandle);
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.YouHaveNotChooseDocument, Enums.MessageType.Warning, Msg_Time._TimeMedium, "");
				}
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_Items.GetRowCellValue(gv_Items.FocusedRowHandle, cl_ItemNo));
				if (!string.IsNullOrEmpty(text))
				{
					ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_SerialNumber_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			if (gv_Items.FocusedRowHandle > -1)
			{
				_OpenChooseCFL = false;
				if (!_OpenChooseCFL)
				{
					ChooseFormListSn();
				}
				else
				{
					_OpenChooseCFL = true;
				}
			}
			else
			{
				WinCommons.writeStatus(Enums.MessageCode.YouHaveNotChooseDocument, Enums.MessageType.Warning, Msg_Time._TimeMedium, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Items_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && _isLoadData && _ContractID != 0)
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

	private void gv_Items_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_fl)
			{
				if (FormMode != Enums.FormViewModeType.Add && _isLoadData && _ContractID != 0)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				if (e.Column.FieldName.ToUpper() == "ITEMCODE")
				{
					FillItemFromCFL(e.RowHandle);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FillColumns(DataRow dr)
	{
		if (dr != null)
		{
			_fl = true;
			gv_Items.SetFocusedRowCellValue("ItemCode", Function.ToString(dr["ItemCode"]));
			gv_Items.SetFocusedRowCellValue("ItemName", Function.ToString(dr["ItemDescription"]));
			_fl = false;
		}
	}

	private async void FillItemFromCFL(int rowHandle, bool tabWhenEmpty = false)
	{
		try
		{
			string itemCode = Function.ToString(gv_Items.GetRowCellValue(rowHandle, "ItemCode"));
			if (!string.IsNullOrEmpty(itemCode))
			{
				DataSet dataSet = await GetListItem(itemCode, Function.ToString(0), GlobalData.UserName, GlobalData.DefaultBranch.ToString());
				if (dataSet.IsNotNull() && dataSet.Tables[1].IsNotNull())
				{
					_fl = true;
					if (dataSet.Tables[1].IsExistsColumn("ItemCode"))
					{
						gv_Items.SetRowCellValue(rowHandle, "ItemCode", dataSet.Tables[1].Rows[0]["ItemCode"]);
					}
					if (dataSet.Tables[1].IsExistsColumn("ItemDescription"))
					{
						gv_Items.SetRowCellValue(rowHandle, "ItemName", dataSet.Tables[1].Rows[0]["ItemDescription"]);
					}
					if (Function.ParseInt(gv_Items.GetFocusedRowCellValue("Line")) == 0)
					{
						gv_Items.SetRowCellValue(rowHandle, "Line", gv_Items.DataRowCount + 1);
					}
					if (dataSet.Tables[1].Columns.Contains("ItmsGrpCod"))
					{
						gv_Items.SetRowCellValue(rowHandle, "itemGroup", dataSet.Tables[1].Rows[0]["ItmsGrpCod"]);
					}
					_fl = false;
					int dataRowCount = gv_Items.DataRowCount;
					string value = Function.ToString(gv_Items.GetRowCellValue(dataRowCount - 1, "ItemCode"));
					int num = Function.ParseInt(gv_Items.GetRowCellValue(dataRowCount - 1, "ItemGroup"));
					if (!string.IsNullOrEmpty(value) || num != 0)
					{
						AddNewRowItems();
					}
				}
				else
				{
					gv_Items.SetRowCellValue(rowHandle, "ItemCode", "");
					gv_Items.SetRowCellValue(rowHandle, "ItemName", "");
					gv_Items.SetRowCellValue(rowHandle, "itemGroup", 0);
					if (rowHandle == gv_Items.FocusedRowHandle)
					{
						WinCommons.Error(Enums.MessageCode.ItemCodeIsNotValid, "");
						ChooseFormListItemCode(itemCode, rowHandle);
					}
				}
			}
			else if (tabWhenEmpty)
			{
				ChooseFormListItemCode(itemCode, rowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_CntrcType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue == null)
			{
				return;
			}
			string contractType = Function.ToString(e.NewValue);
			if (gv_Items.DataRowCount > 1)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ContractConfirmChangeService, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					e.Cancel = true;
					_changeType = true;
					return;
				}
				grd_Items.DataSource = null;
				grd_Items.RefreshDataSource();
				LoadItemForm(contractType);
				AddNewRowItems();
			}
			else
			{
				LoadItemForm(contractType);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbb_cntrc_CloseUp(object sender, CloseUpEventArgs e)
	{
		try
		{
			if (e.Value == null)
			{
				return;
			}
			string text = Function.ToString(((Common.ListItems)e.Value).Value);
			if (gv_Items.DataRowCount > 1 && Function.ParseInt(((DataTable)grd_Items.DataSource).Rows[0]["ContractID"]) == _ContractID)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ContractInfoChangingService, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
				{
					e.AcceptValue = false;
					e.Value = cbb_cntrc.EditValue;
					cbb_cntrc.Text = Function.ToString(((Common.ListItems)e.Value).Text);
					return;
				}
				grd_Items.DataSource = null;
				grd_Items.RefreshDataSource();
				AddNewRowItems();
			}
			if (text == "S")
			{
				lbl_ActiveItems.Visible = true;
				txt_ActiveItems.Visible = true;
				cl_ItemGroup.Visible = false;
				cl_ItemGroup.VisibleIndex = -1;
				cl_ItemNo.Visible = true;
				cl_ItemNo.VisibleIndex = 0;
				cl_ItemDescription.Visible = true;
				cl_ItemDescription.VisibleIndex = 1;
				cl_MfrSerialNo.Visible = true;
				cl_MfrSerialNo.VisibleIndex = 2;
				cl_SerialNumber.Visible = true;
				cl_SerialNumber.VisibleIndex = 3;
			}
			if (text == "G")
			{
				lbl_ActiveItems.Visible = false;
				txt_ActiveItems.Visible = false;
				cl_ItemGroup.Visible = true;
				cl_ItemGroup.VisibleIndex = 0;
				cl_ItemNo.Visible = false;
				cl_SerialNumber.Visible = false;
				cl_MfrSerialNo.Visible = false;
				cl_ItemDescription.Visible = false;
			}
			if (text == "C")
			{
				tab_Items.PageEnabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rlp_ItemGroup_CloseUp(object sender, CloseUpEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Items.DataSource as DataTable;
			if (dataTable.IsNotNull() && dataTable.Select("ItemGroup='" + Function.ParseInt(e.Value) + "'").Length != 0)
			{
				e.AcceptValue = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Items_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!(sender is GridControl gridControl))
			{
				return;
			}
			GridView gridView = gridControl.FocusedView as GridView;
			if (e.KeyData != Keys.Tab || gridView == null)
			{
				return;
			}
			gridView.PostEditor();
			gridView.RefreshData();
			if (!_OpenChooseCFL)
			{
				return;
			}
			int focusedRowHandle = gv_Items.FocusedRowHandle;
			if (focusedRowHandle > -1)
			{
				if (gridView.FocusedColumn.FieldName.ToUpper() == "ITEMCODE")
				{
					FillItemFromCFL(focusedRowHandle, tabWhenEmpty: true);
				}
				if (gridView.FocusedColumn.FieldName.ToUpper() == "INTERNALSN" || gridView.FocusedColumn.FieldName.ToUpper() == "MANUFSN")
				{
					ChooseFormListSn();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Items_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_ServiceCall_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void rep_ItemName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private async Task LoadData()
	{
		_ = 5;
		try
		{
			date_Start.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_EndDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			cl_ActiveItems.OptionsColumn.AllowEdit = false;
			cl_ActiveItems.OptionsColumn.ReadOnly = true;
			cl_CTRemarks.OptionsColumn.AllowEdit = false;
			cl_CTRemarks.OptionsColumn.ReadOnly = true;
			_dtContact = await GetContactPerson();
			await LoadDataCombobox();
			await WinFuntions.LoadUdfColumns(gv_Doc, "OCTR");
			await WinFuntions.LoadUdfColumns(gv_Items, "CTR1");
			DataTable dataTable = await Search();
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				_ContractID = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.TopRowIndex, "ContractID"));
				LoadAllTabs(await GetDetail(_ContractID));
			}
			else
			{
				grd_Doc.DataSource = new DataTable();
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<int> GetContractId()
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceContract");
			DataTable dataTable = await helperServiceCall.GetAsync("GetContractID");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["ContractID"]) + 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 1;
	}

	private void LoadAllTabs(DataSet ds)
	{
		try
		{
			_isLoadcoverage = true;
			if (ds.IsNotNull() && ds.Tables.Count > 4)
			{
				if (ds.Tables[1].IsNotNull())
				{
					LoadData_Coverage(ds.Tables[1]);
				}
				else
				{
					LoadData_Coverage(new DataTable());
				}
				ucAttachment1.LoadAttachments(Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AtcEntry")));
				if (ds.Tables[3].IsNotNull())
				{
					LoadItemForm(Function.ToString(ds.Tables[0].Rows[0]["CntrcType"]));
					grd_Items.DataSource = ds.Tables[3];
					grd_Items.RefreshDataSource();
				}
				else
				{
					LoadItemForm(Function.ToString(ds.Tables[0].Rows[0]["CntrcType"]));
					grd_Items.DataSource = null;
					grd_Items.RefreshDataSource();
				}
				if (ds.Tables[4].IsNotNull())
				{
					grd_ServiceCall.DataSource = ds.Tables[4];
					grd_ServiceCall.RefreshDataSource();
				}
				else
				{
					grd_ServiceCall.DataSource = null;
					grd_ServiceCall.RefreshDataSource();
				}
				AddNewRowItems();
				_isLoadData = true;
				_isLoadcoverage = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeyDownPaste(object sender, KeyEventArgs e)
	{
		PasteFromExcel(e, grd_Items, gv_Items);
	}

	private void PasteFromExcel(KeyEventArgs e, GridControl grd, GridView gv)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control || Function.ToString(gv_Doc.GetFocusedRowCellValue("Status")) == "T")
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject == null)
			{
				return;
			}
			string text = dataObject.GetData(DataFormats.UnicodeText).ToString();
			object data = text;
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				DataTable dataFromExcel = GetDataFromExcel(text);
				if (dataFromExcel != null && (dataFromExcel.Rows.Count > 1 || dataFromExcel.Columns.Count > 1))
				{
					data = dataFromExcel.Rows[0][0];
					int num = gv.FocusedRowHandle;
					GridColumn focusedColumn = gv.FocusedColumn;
					int count = dataFromExcel.Rows.Count;
					int num2 = ((gv.Columns.Count > dataFromExcel.Columns.Count) ? dataFromExcel.Columns.Count : gv.Columns.Count);
					gv.BeginUpdate();
					for (int i = 0; i < count; i++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int j = 0; j < num2; j++)
						{
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							if (gridColumn != null && gridColumn.GroupIndex < 0)
							{
								gv.SetRowCellValue(num, gridColumn, dataFromExcel.Rows[i][j]);
							}
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
					gv.EndUpdate();
					Clipboard.Clear();
				}
			}
			Clipboard.SetData(DataFormats.Text, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDataFromExcel(string clipboardData)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<string> list = new List<string>(clipboardData.Split('\n'));
			bool flag = true;
			if (list.Count > 0 && string.IsNullOrWhiteSpace(list[list.Count - 1]))
			{
				list.RemoveAt(list.Count - 1);
			}
			foreach (string item in list)
			{
				string text = item;
				if (text.EndsWith("\r"))
				{
					text = text.Substring(0, text.Length - "\r".Length);
				}
				string[] array = text.Split('\r', '\t');
				DataRow dataRow = dataTable.NewRow();
				if (flag)
				{
					int num = 0;
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (string.IsNullOrWhiteSpace(text2))
						{
							dataTable.Columns.Add($"[BLANK{num}]");
						}
						else if (!dataTable.Columns.Contains(text2))
						{
							dataTable.Columns.Add(text2);
						}
						else
						{
							dataTable.Columns.Add($"Column {num}");
						}
						num++;
					}
					for (int j = 0; j < array.Length && j < dataTable.Columns.Count; j++)
					{
						dataRow[j] = array[j];
					}
					dataTable.Rows.Add(dataRow);
					flag = false;
				}
				else
				{
					for (int k = 0; k < array.Length && k < dataTable.Columns.Count; k++)
					{
						dataRow[k] = array[k];
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	private async Task LoadDataCombobox()
	{
		_ = 2;
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "H",
					Name = "Hours(s)"
				},
				new Common.Table_Code
				{
					Code = "D",
					Name = "Day(s)"
				}
			};
			rep_ReponseU.DataSource = dataSource;
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "R",
					Name = "Regular"
				},
				new Common.Table_Code
				{
					Code = "W",
					Name = "Warranty"
				}
			};
			rep_SrvcType.DataSource = dataSource2;
			List<Common.Table_Code> dataSource3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "C",
					Name = "Customer"
				},
				new Common.Table_Code
				{
					Code = "G",
					Name = "ItemGroup"
				},
				new Common.Table_Code
				{
					Code = "S",
					Name = "Serial Number"
				}
			};
			rep_CntrcType.DataSource = dataSource3;
			List<Common.Table_Code> dataSource4 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "A",
					Name = "Approved"
				},
				new Common.Table_Code
				{
					Code = "F",
					Name = "On Hold"
				},
				new Common.Table_Code
				{
					Code = "D",
					Name = "Draft"
				},
				new Common.Table_Code
				{
					Code = "T",
					Name = "Terminated"
				}
			};
			rep_Status.DataSource = dataSource4;
			List<Common.Table_Code> dataSource5 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "D",
					Name = "Day(s)"
				},
				new Common.Table_Code
				{
					Code = "W",
					Name = "Week(s)"
				},
				new Common.Table_Code
				{
					Code = "M",
					Name = "Month(s)"
				}
			};
			rep_RemU.DataSource = dataSource5;
			await LoadComboboxOwner();
			await LoadCbItemGroup();
			await LoadContactPerson();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadItemForm(string contractType)
	{
		try
		{
			if (contractType == "S")
			{
				tab_Items.PageEnabled = true;
				gv_Items.BeginUpdate();
				gv_Items.Columns.Remove(cl_ItemGroup);
				gv_Items.Columns.Add(cl_ItemDescription);
				gv_Items.Columns.Add(cl_MfrSerialNo);
				gv_Items.Columns.Add(cl_SerialNumber);
				gv_Items.Columns.Add(cl_ItemNo);
				gv_Items.EndUpdate();
				cl_ItemNo.Visible = true;
				cl_ItemNo.VisibleIndex = 0;
				cl_ItemDescription.Visible = true;
				cl_ItemDescription.VisibleIndex = 1;
				cl_MfrSerialNo.Visible = true;
				cl_MfrSerialNo.VisibleIndex = 2;
				cl_SerialNumber.Visible = true;
				cl_SerialNumber.VisibleIndex = 3;
				cl_ItemGroup.Visible = false;
				rlp_ItemGroup.ReadOnly = true;
				cl_ItemGroup.VisibleIndex = -1;
			}
			if (contractType == "G")
			{
				tab_Items.PageEnabled = true;
				gv_Items.BeginUpdate();
				gv_Items.Columns.Remove(cl_ItemNo);
				gv_Items.Columns.Remove(cl_SerialNumber);
				gv_Items.Columns.Remove(cl_MfrSerialNo);
				gv_Items.Columns.Remove(cl_ItemDescription);
				gv_Items.Columns.Add(cl_ItemGroup);
				gv_Items.EndUpdate();
				cl_ItemGroup.Visible = true;
				cl_ItemGroup.VisibleIndex = 0;
				cl_ItemNo.Visible = false;
				cl_SerialNumber.Visible = false;
				rep_SerialNumber.ReadOnly = true;
				cl_MfrSerialNo.Visible = false;
				cl_ItemDescription.Visible = false;
				rlp_ItemGroup.ReadOnly = false;
			}
			if (contractType == "C")
			{
				tab_Items.PageEnabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadData_General(DataTable dt)
	{
	}

	private void LoadData_Coverage(DataTable dt)
	{
		try
		{
			DateTime today = DateTime.Today;
			TimeSpan value = new TimeSpan(8, 0, 0);
			TimeSpan value2 = new TimeSpan(0, 0, 0);
			TimeSpan value3 = new TimeSpan(17, 0, 0);
			TimeSpan value4 = new TimeSpan(23, 59, 0);
			chk_Mon.EditValue = !dt.IsExistsColumn("MonEnabled") || Function.ToString(dt.Rows[0]["MonEnabled"]).Equals("Y");
			date_MonS.EditValue = (dt.IsExistsColumn("MonStart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["MonStart"])) : today.Add(value));
			date_MonE.EditValue = (dt.IsExistsColumn("MonEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["MonEnd"])) : today.Add(value3));
			chk_Tue.EditValue = !dt.IsExistsColumn("TueEnabled") || dt.Rows[0]["TueEnabled"] == null || Function.ToString(dt.Rows[0]["TueEnabled"]).Equals("Y");
			date_TueS.EditValue = (dt.IsExistsColumn("TueStart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["TueStart"])) : today.Add(value));
			date_TueE.EditValue = (dt.IsExistsColumn("TueEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["TueEnd"])) : today.Add(value3));
			chk_Wed.EditValue = !dt.IsExistsColumn("WedEnabled") || dt.Rows[0]["WedEnabled"] == null || Function.ToString(dt.Rows[0]["WedEnabled"]).Equals("Y");
			date_WenS.EditValue = (dt.IsExistsColumn("WedStart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["WedStart"])) : today.Add(value));
			date_WenE.EditValue = (dt.IsExistsColumn("WedEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["WedEnd"])) : today.Add(value3));
			chk_Thu.EditValue = !dt.IsExistsColumn("ThuEnabled") || dt.Rows[0]["ThuEnabled"] == null || Function.ToString(dt.Rows[0]["ThuEnabled"]).Equals("Y");
			date_ThuS.EditValue = (dt.IsExistsColumn("ThuStart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["ThuStart"])) : today.Add(value));
			date_ThuE.EditValue = (dt.IsExistsColumn("ThuEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["ThuEnd"])) : today.Add(value3));
			chk_Fri.EditValue = !dt.IsExistsColumn("FriEnabled") || dt.Rows[0]["FriEnabled"] == null || Function.ToString(dt.Rows[0]["FriEnabled"]).Equals("Y");
			date_FriS.EditValue = (dt.IsExistsColumn("FriStart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["FriStart"])) : today.Add(value));
			date_FriE.EditValue = (dt.IsExistsColumn("FriEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["FriEnd"])) : today.Add(value3));
			chk_Sat.EditValue = !dt.IsExistsColumn("SatEnabled") || dt.Rows[0]["SatEnabled"] == null || Function.ToString(dt.Rows[0]["SatEnabled"]).Equals("Y");
			date_SatS.EditValue = (dt.IsExistsColumn("SatStart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["SatStart"])) : today.Add(value2));
			date_SatE.EditValue = (dt.IsExistsColumn("SatEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["SatEnd"])) : today.Add(value4));
			chk_Sun.EditValue = !dt.IsExistsColumn("SunEnabled") || dt.Rows[0]["SunEnabled"] == null || Function.ToString(dt.Rows[0]["SunEnabled"]).Equals("Y");
			date_SunS.EditValue = (dt.IsExistsColumn("SunStrart") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["SunStrart"])) : today.Add(value2));
			date_SunE.EditValue = (dt.IsExistsColumn("SunEnd") ? Function.IntToTime(Function.ParseInt(dt.Rows[0]["SunEnd"])) : today.Add(value4));
			chk_Parts.EditValue = dt.IsExistsColumn("InclParts") && dt.Rows[0]["InclParts"] != null && Function.ToString(dt.Rows[0]["InclParts"]).Equals("Y");
			chk_Labor.EditValue = dt.IsExistsColumn("InclWork") && dt.Rows[0]["InclWork"] != null && Function.ToString(dt.Rows[0]["InclWork"]).Equals("Y");
			chk_Travel.EditValue = dt.IsExistsColumn("InclTravel") && dt.Rows[0]["InclTravel"] != null && Function.ToString(dt.Rows[0]["InclTravel"]).Equals("Y");
			chk_Including.EditValue = dt.IsExistsColumn("InclHldays") && dt.Rows[0]["InclHldays"] != null && Function.ToString(dt.Rows[0]["InclHldays"]).Equals("Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("fDate", typeof(DateTime));
			dataTable.Columns.Add("tDate", typeof(DateTime));
			DataRow dataRow = dataTable.NewRow();
			if (!string.IsNullOrEmpty(date_Start.Text))
			{
				dataRow["fDate"] = date_Start.DateTime;
			}
			if (!string.IsNullOrEmpty(date_EndDate.Text))
			{
				dataRow["tDate"] = date_EndDate.DateTime;
			}
			dataTable.Rows.Add(dataRow);
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceContract");
			return await _helper.PostAsync("Search", dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataSet> GetDetail(int contractID)
	{
		try
		{
			using HttpClientHelper<DataSet> _helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ServiceContract");
			return await _helper.GetAsync("GetDetail", new { contractID });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataSet();
		}
	}

	public void ChooseFormListCardCode(string cardcode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, cardcode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				SetDataByCardCode(chooseItem);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void SetDataByCardCode(DataRow obj)
	{
		try
		{
			WinCommons._OpenProcessing();
			gv_Doc.SetFocusedRowCellValue("CstmrCode", obj["CardCode"]);
			gv_Doc.SetFocusedRowCellValue("CstmrName", obj["CardName"]);
			DataTable dataTable = await GetContactPerson(Function.ToString(obj["CardCode"]));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				rep_ContactPersonCode.DataSource = dataTable;
				DataRow dataRow = dataTable.Rows[0];
				gv_Doc.SetFocusedRowCellValue("CntctCode", dataRow["CntctCode"]);
				gv_Doc.SetFocusedRowCellValue("TelephoneNo", dataRow["Tel1"]);
			}
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadComboboxOwner()
	{
		try
		{
			using HttpClientHelper<RequesterImpl> helper = new HttpClientHelper<RequesterImpl>(Configuration.ApiServer, "Requester");
			DataTable dataTable = await helper.GetDataTableAsync("GetUser_tableOUSR");
			if (dataTable.IsNotNull() && dataTable.ExistsColumn("USERID") && dataTable.ExistsColumn("U_NAME"))
			{
				List<Common.Table_Code> list = new List<Common.Table_Code>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Common.Table_Code item = new Common.Table_Code
					{
						Code = Function.ToString(dataTable.Rows[i]["USERID"]),
						Name = Function.ToString(dataTable.Rows[i]["U_NAME"])
					};
					list.Add(item);
				}
				rep_Owner.DataSource = list;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCbItemGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemGroups");
			DataTable dataTable = await _help.GetDataTableAsync("GetItemGroup");
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				Common.Table_Code item = new Common.Table_Code
				{
					Code = dataTable.Rows[i]["ITMSGRPCOD"].ToString(),
					Name = dataTable.Rows[i]["ITMSGRPNAM"].ToString()
				};
				list.Add(item);
			}
			rlp_ItemGroup.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadContactPerson()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await _help.GetDataTableAsync("GetAllContactPerson");
			rep_ContactPersonCode.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetContactPerson(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			return await helperServiceCall.GetAsync("GetContactPerson", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public void ChooseFormListItemCode(string itemCode, int rowHandle)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, itemCode, "O", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ListMandatoryField = "ItemDescription,ItmsGrpCod";
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				if (chooseItem.Table.IsExistsColumn("ItemCode"))
				{
					gv_Items.SetFocusedRowCellValue("ItemCode", chooseItem["ItemCode"]);
				}
				if (chooseItem.Table.IsExistsColumn("ItemDescription"))
				{
					gv_Items.SetFocusedRowCellValue("ItemName", chooseItem["ItemDescription"]);
				}
				if (Function.ParseInt(gv_Items.GetFocusedRowCellValue("Line")) == 0)
				{
					gv_Items.SetFocusedRowCellValue("Line", gv_Items.DataRowCount + 1);
				}
				if (chooseItem.Table.Columns.Contains("ItmsGrpCod"))
				{
					gv_Items.SetFocusedRowCellValue("itemGroup", chooseItem["ItmsGrpCod"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void ChooseFormListSn()
	{
		try
		{
			string itemId = Function.ToString(gv_Doc.GetFocusedRowCellValue("CstmrCode"));
			string text = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListOfInstallBase, false, "", "", Enums.ItemObjectType.NoItemType, null, itemId, text);
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				if (chooseItem.Table.IsExistsColumn("manufSN"))
				{
					gv_Items.SetFocusedRowCellValue("ManufSN", chooseItem["manufSN"]);
				}
				if (chooseItem.Table.IsExistsColumn("internalSN"))
				{
					gv_Items.SetFocusedRowCellValue("InternalSN", chooseItem["internalSN"]);
				}
				if (chooseItem.Table.IsExistsColumn("customer"))
				{
					gv_Doc.SetFocusedRowCellValue("CstmrCode", chooseItem["customer"]);
				}
				if (chooseItem.Table.IsExistsColumn("custmrName"))
				{
					gv_Doc.SetFocusedRowCellValue("CstmrName", chooseItem["custmrName"]);
				}
				if (chooseItem.Table.IsExistsColumn("itemCode"))
				{
					gv_Items.SetFocusedRowCellValue("ItemCode", chooseItem["itemCode"]);
				}
				if (chooseItem.Table.IsExistsColumn("itemName"))
				{
					gv_Items.SetFocusedRowCellValue("ItemName", chooseItem["itemName"]);
				}
			}
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
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Clone();
				DataRow dataRow = dataTable2.NewRow();
				dataRow["ContractID"] = 0;
				dataRow["CstmrCode"] = string.Empty;
				dataRow["CstmrName"] = string.Empty;
				dataRow["CntctCode"] = DBNull.Value;
				dataRow["EndDate"] = DBNull.Value;
				dataRow["StartDate"] = DateTime.Today;
				dataRow["TermDate"] = DBNull.Value;
				dataRow["Descriptio"] = string.Empty;
				dataRow["AtcEntry"] = 0;
				dataRow["ResponseV"] = DBNull.Value;
				dataRow["ResponsVal"] = DBNull.Value;
				dataRow["Owner"] = GlobalData.UserID;
				dataRow["RemindVal"] = DBNull.Value;
				dataRow["CntrcType"] = "S";
				dataRow["CntrcTmplt"] = string.Empty;
				dataRow["SrvcType"] = "R";
				dataRow["ResponseU"] = "H";
				dataRow["ResponsUnt"] = "D";
				dataRow["Status"] = "D";
				dataRow["Renewal"] = "N";
				dataRow["RemindUnit"] = "D";
				dataRow["Remarks1"] = string.Empty;
				dataRow["Remarks2"] = string.Empty;
				dataRow["CTR1Count"] = DBNull.Value;
				dataTable2.Rows.Add(dataRow);
				grd_Doc.DataSource = dataTable2;
				grd_Doc.RefreshDataSource();
			}
			else
			{
				DataTable dataTable3 = new DataTable();
				dataTable3.Columns.Add("ContractID", typeof(int));
				dataTable3.Columns.Add("CstmrCode", typeof(string));
				dataTable3.Columns.Add("CstmrName", typeof(string));
				dataTable3.Columns.Add("CntctCode", typeof(int));
				dataTable3.Columns.Add("TelephoneNo", typeof(string));
				dataTable3.Columns.Add("StartDate", typeof(DateTime));
				dataTable3.Columns.Add("EndDate", typeof(DateTime));
				dataTable3.Columns.Add("TermDate", typeof(DateTime));
				dataTable3.Columns.Add("Descriptio", typeof(string));
				dataTable3.Columns.Add("AtcEntry", typeof(int));
				dataTable3.Columns.Add("ResponseV", typeof(int));
				dataTable3.Columns.Add("ResponsVal", typeof(int));
				dataTable3.Columns.Add("Owner", typeof(int));
				dataTable3.Columns.Add("RemindVal", typeof(int));
				dataTable3.Columns.Add("CntrcType", typeof(string));
				dataTable3.Columns.Add("CntrcTmplt", typeof(string));
				dataTable3.Columns.Add("SrvcType", typeof(string));
				dataTable3.Columns.Add("ResponseU", typeof(string));
				dataTable3.Columns.Add("ResponsUnt", typeof(string));
				dataTable3.Columns.Add("Status", typeof(string));
				dataTable3.Columns.Add("Renewal", typeof(string));
				dataTable3.Columns.Add("RemindUnit", typeof(string));
				dataTable3.Columns.Add("Remarks1", typeof(string));
				dataTable3.Columns.Add("Remarks2", typeof(string));
				dataTable3.Columns.Add("CTR1Count", typeof(int));
				dataTable3.Rows.Add(0, string.Empty, string.Empty, DBNull.Value, DBNull.Value, DateTime.Today, DBNull.Value, DBNull.Value, string.Empty, 0, DBNull.Value, DBNull.Value, GlobalData.UserID, DBNull.Value, "S", string.Empty, "R", "H", "D", "D", "N", "D", string.Empty, string.Empty, DBNull.Value);
				grd_Doc.DataSource = dataTable3;
				grd_Doc.RefreshDataSource();
			}
			LoadItemForm("S");
			DataTable dataTable4 = new DataTable();
			dataTable4.Columns.Add("Line", typeof(int));
			dataTable4.Columns.Add("ManufSN", typeof(int));
			dataTable4.Columns.Add("InternalSN", typeof(int));
			dataTable4.Columns.Add("ItemGroup", typeof(int));
			dataTable4.Columns.Add("ItemCode", typeof(string));
			dataTable4.Columns.Add("ItemName", typeof(string));
			dataTable4.Columns.Add("StartDate", typeof(DateTime));
			dataTable4.Columns.Add("EndDate", typeof(DateTime));
			dataTable4.Columns.Add("TermDate", typeof(DateTime));
			dataTable4.Rows.Add(0, DBNull.Value, DBNull.Value, 0, string.Empty, string.Empty, DateTime.Today, DBNull.Value, DBNull.Value);
			grd_Items.DataSource = dataTable4;
			grd_Items.RefreshDataSource();
			LoadData_Coverage(new DataTable());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRowItems()
	{
		try
		{
			DataTable dataTable = grd_Items.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Line"] = dataTable.Rows.Count + 1;
				dataRow["ManufSN"] = DBNull.Value;
				dataRow["InternalSN"] = DBNull.Value;
				dataRow["ItemGroup"] = 0;
				dataRow["ItemCode"] = string.Empty;
				dataRow["ItemName"] = string.Empty;
				dataRow["StartDate"] = dataTable.Rows[0]["StartDate"];
				dataRow["EndDate"] = dataTable.Rows[0]["EndDate"];
				dataRow["TermDate"] = DBNull.Value;
				dataTable.Rows.Add(dataRow);
				grd_Items.DataSource = dataTable;
				grd_Items.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("Line", typeof(int));
				dataTable2.Columns.Add("ManufSN", typeof(int));
				dataTable2.Columns.Add("InternalSN", typeof(int));
				dataTable2.Columns.Add("ItemGroup", typeof(int));
				dataTable2.Columns.Add("ItemCode", typeof(string));
				dataTable2.Columns.Add("ItemName", typeof(string));
				dataTable2.Columns.Add("StartDate", typeof(DateTime));
				dataTable2.Columns.Add("EndDate", typeof(DateTime));
				dataTable2.Columns.Add("TermDate", typeof(DateTime));
				dataTable2.Rows.Add(0, DBNull.Value, DBNull.Value, 0, string.Empty, string.Empty, DateTime.Today, Function.ParseDateTimes(gv_Doc.GetFocusedRowCellValue("EndDate")), DBNull.Value);
				grd_Items.DataSource = dataTable2;
				grd_Items.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetValueForm_Doc()
	{
		try
		{
			DataTable tableStructure = WinFuntions.GetTableStructure("OCTR");
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = tableStructure.NewRow();
				foreach (DataColumn column in dataTable.Columns)
				{
					if (dataRow.IsExistsColumn(column.ColumnName))
					{
						dataRow[column.ColumnName] = gv_Doc.GetFocusedRowCellValue(column.ColumnName);
					}
				}
				dataRow["MonEnabled"] = (chk_Mon.Checked ? "Y" : "N");
				dataRow["TueEnabled"] = (chk_Tue.Checked ? "Y" : "N");
				dataRow["WedEnabled"] = (chk_Wed.Checked ? "Y" : "N");
				dataRow["ThuEnabled"] = (chk_Thu.Checked ? "Y" : "N");
				dataRow["FriEnabled"] = (chk_Fri.Checked ? "Y" : "N");
				dataRow["SatEnabled"] = (chk_Sat.Checked ? "Y" : "N");
				dataRow["SunEnabled"] = (chk_Sun.Checked ? "Y" : "N");
				dataRow["InclParts"] = (chk_Parts.Checked ? "Y" : "N");
				dataRow["InclWork"] = (chk_Labor.Checked ? "Y" : "N");
				dataRow["InclTravel"] = (chk_Travel.Checked ? "Y" : "N");
				dataRow["InclHldays"] = (chk_Including.Checked ? "Y" : "N");
				dataRow["MonStart"] = Function.ParseTimeSpanToInt(date_MonS.DateTime.TimeOfDay);
				dataRow["MonEnd"] = Function.ParseTimeSpanToInt(date_MonE.DateTime.TimeOfDay);
				dataRow["TueStart"] = Function.ParseTimeSpanToInt(date_TueS.DateTime.TimeOfDay);
				dataRow["TueEnd"] = Function.ParseTimeSpanToInt(date_TueE.DateTime.TimeOfDay);
				dataRow["WedStart"] = Function.ParseTimeSpanToInt(date_WenS.DateTime.TimeOfDay);
				dataRow["WedEnd"] = Function.ParseTimeSpanToInt(date_WenE.DateTime.TimeOfDay);
				dataRow["ThuStart"] = Function.ParseTimeSpanToInt(date_ThuS.DateTime.TimeOfDay);
				dataRow["ThuEnd"] = Function.ParseTimeSpanToInt(date_ThuE.DateTime.TimeOfDay);
				dataRow["FriStart"] = Function.ParseTimeSpanToInt(date_FriS.DateTime.TimeOfDay);
				dataRow["FriEnd"] = Function.ParseTimeSpanToInt(date_FriE.DateTime.TimeOfDay);
				dataRow["SatStart"] = Function.ParseTimeSpanToInt(date_SatS.DateTime.TimeOfDay);
				dataRow["SatEnd"] = Function.ParseTimeSpanToInt(date_SatE.DateTime.TimeOfDay);
				dataRow["SunStrart"] = Function.ParseTimeSpanToInt(date_SunS.DateTime.TimeOfDay);
				dataRow["SunEnd"] = Function.ParseTimeSpanToInt(date_SunE.DateTime.TimeOfDay);
				tableStructure.Rows.Add(dataRow);
			}
			return tableStructure;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public string TimeToTimeString(TimeSpan time)
	{
		try
		{
			return ((time.Hours < 10) ? ("0" + time.Hours) : time.Hours.ToString()) + ":" + ((time.Minutes < 10) ? ("0" + time.Minutes) : time.Minutes.ToString());
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return string.Empty;
		}
	}

	private async Task SaveDoc(DataSet data)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ServiceContract"))
			{
				string text = await _help.PostAsJsonAsync("Save", data);
				if (string.IsNullOrEmpty(text))
				{
					FormMode = Enums.FormViewModeType.View;
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					DataTable dataTable = await Search();
					if (dataTable.IsNotNull())
					{
						grd_Doc.DataSource = dataTable;
						grd_Doc.RefreshDataSource();
						_ContractID = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.TopRowIndex, "ContractID"));
						DataSet ds = await GetDetail(_ContractID);
						if (_ContractID != 0)
						{
							LoadAllTabs(ds);
						}
					}
					else
					{
						grd_Doc.DataSource = new DataTable();
						grd_Doc.RefreshDataSource();
					}
					btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
					_isFocusRow = false;
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
	}

	private void AddHandlers()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View)
			{
				return;
			}
			foreach (Control item in pnl_Coverage.Controls.OfType<Control>().ToList())
			{
				if (item.GetType().Name == "DateEdit")
				{
					((DateEdit)item).EditValueChanged += OnContentChanged;
				}
				if (item.GetType().Name == "CheckEdit")
				{
					((CheckEdit)item).CheckStateChanged += OnContentChanged;
				}
			}
			foreach (Control item2 in panelControl4.Controls.OfType<Control>().ToList())
			{
				if (item2.GetType().Name == "CheckEdit")
				{
					((CheckEdit)item2).CheckStateChanged += OnContentChanged;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && _isLoadData && _ContractID != 0 && !_isLoadcoverage)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			else
			{
				if (_OldContactID != _ContractID)
				{
					_isLoadData = false;
				}
				_OldContactID = _ContractID;
			}
			if (!Function.ParseBool(chk_Mon.EditValue))
			{
				date_MonS.ReadOnly = true;
				date_MonE.ReadOnly = true;
			}
			else
			{
				date_MonS.ReadOnly = false;
				date_MonE.ReadOnly = false;
			}
			if (!Function.ParseBool(chk_Tue.EditValue))
			{
				date_TueS.ReadOnly = true;
				date_TueE.ReadOnly = true;
			}
			else
			{
				date_TueS.ReadOnly = false;
				date_TueE.ReadOnly = false;
			}
			if (!Function.ParseBool(chk_Wed.EditValue))
			{
				date_WenS.ReadOnly = true;
				date_WenE.ReadOnly = true;
			}
			else
			{
				date_WenS.ReadOnly = false;
				date_WenE.ReadOnly = false;
			}
			if (!Function.ParseBool(chk_Thu.EditValue))
			{
				date_ThuS.ReadOnly = true;
				date_ThuE.ReadOnly = true;
			}
			else
			{
				date_ThuS.ReadOnly = false;
				date_ThuE.ReadOnly = false;
			}
			if (!Function.ParseBool(chk_Fri.EditValue))
			{
				date_FriS.ReadOnly = true;
				date_FriE.ReadOnly = true;
			}
			else
			{
				date_FriS.ReadOnly = false;
				date_FriE.ReadOnly = false;
			}
			if (!Function.ParseBool(chk_Sat.EditValue))
			{
				date_SatS.ReadOnly = true;
				date_SatE.ReadOnly = true;
			}
			else
			{
				date_SatS.ReadOnly = false;
				date_SatE.ReadOnly = false;
			}
			if (!Function.ParseBool(chk_Sun.EditValue))
			{
				date_SunS.ReadOnly = true;
				date_SunE.ReadOnly = true;
			}
			else
			{
				date_SunS.ReadOnly = false;
				date_SunE.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetContactPerson()
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			return await helperServiceCall.GetAsync("GetContactPerson");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataSet> GetListItem(string itemCode, string itemType, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			string text = string.Join(";", Params.ToArray());
			if (string.IsNullOrEmpty(text))
			{
				text = "";
			}
			return await helper.GetAsync("GetListItem", new
			{
				itemCode = itemCode,
				itemType = itemType,
				userName = userName,
				branch = branch,
				pr = text
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	protected override void NoAuthSetControlSearch()
	{
		btnSearch.Enabled = true;
		DateEditEx dateEditEx = date_Start;
		bool readOnly = (date_EndDate.ReadOnly = false);
		dateEditEx.ReadOnly = readOnly;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_Contract));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		this.spl_Content = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.ctm_Menu_Add = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ContractNo = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CustomerCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rp_Customer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_CustomerName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ContactPerson = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ContactPersonCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_TelephoneNo = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_StartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.cl_EndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_TerminationDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Description = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_AtcEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SrvcType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SrvcType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_CntrcType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CntrcType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_CntrcTmplt = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Template = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.cl_ResponseTime = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ResponseUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ReponseU = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_ResolutionTime = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ResolutionUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Remarks2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.cl_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Owner = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Owner = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Renewal = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Renewal = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.cl_RemTime = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_RemUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ActiveItems = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CTRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Remarks1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.rep_RemU = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tab_Content = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Items = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.grd_Items = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Item = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.del_ctm_Item = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Items = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ItemGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rlp_ItemGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_ItemNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_ItemDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.cl_MfrSerialNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SerialNumber = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.cl_SerialNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_LineStartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LineDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.cl_LineEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_LineTerminationDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_LineContractID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_LineItem = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_Coverage = new Apzon.Controls.Controls.XtraTabPageEx();
		this.pnl_Coverage = new DevExpress.XtraEditors.PanelControl();
		this.date_WenS = new Apzon.Controls.Editor.DateEditEx();
		this.date_TueE = new Apzon.Controls.Editor.DateEditEx();
		this.date_TueS = new Apzon.Controls.Editor.DateEditEx();
		this.date_MonE = new Apzon.Controls.Editor.DateEditEx();
		this.chk_Including = new Apzon.Controls.Editor.CheckEditEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.chk_Travel = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Labor = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Parts = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_SunE = new Apzon.Controls.Editor.DateEditEx();
		this.date_FriE = new Apzon.Controls.Editor.DateEditEx();
		this.date_SatE = new Apzon.Controls.Editor.DateEditEx();
		this.date_SunS = new Apzon.Controls.Editor.DateEditEx();
		this.date_ThuE = new Apzon.Controls.Editor.DateEditEx();
		this.date_WenE = new Apzon.Controls.Editor.DateEditEx();
		this.date_ThuS = new Apzon.Controls.Editor.DateEditEx();
		this.date_FriS = new Apzon.Controls.Editor.DateEditEx();
		this.date_SatS = new Apzon.Controls.Editor.DateEditEx();
		this.date_MonS = new Apzon.Controls.Editor.DateEditEx();
		this.lbl_EndTime = new DevExpress.XtraEditors.LabelControl();
		this.lbl_StartTime = new DevExpress.XtraEditors.LabelControl();
		this.chk_Sun = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Sat = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Fri = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Thu = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Wed = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Tue = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_Mon = new Apzon.Controls.Editor.CheckEditEx();
		this.tab_ServiceCalls = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ServiceCall = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ServiceCall = new Apzon.Controls.Grid.GridViewEx();
		this.cl_CallID = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CreationDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Subject = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SC_ItemNo = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SN = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SC_CustomerName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_SC_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.tab_SalesData = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl6 = new DevExpress.XtraEditors.PanelControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_RecTemp = new Apzon.Controls.Grid.GridControlEx();
		this.gv_RecTemp = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Template = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.lbl_RecurringTemp = new Apzon.Controls.Controls.LabelLine();
		this.grd_RecTrans = new Apzon.Controls.Grid.GridControlEx();
		this.gv_RecTrans = new Apzon.Controls.Grid.GridViewEx();
		this.cl_RecTran_Template = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Instance = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_RecTran_Instance = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_RecTran_Date = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_DocTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.lbl_RecurringTransactions = new Apzon.Controls.Controls.LabelLine();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_StartDate = new Apzon.Controls.Controls.LabelLine();
		this.lbl_EndDate = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lblServiceType = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ContractType = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Template = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Resoulution = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ResponseTime = new Apzon.Controls.Controls.LabelLine();
		this.Remarks = new Apzon.Controls.Controls.LabelLine();
		this.cbb_SrvcType = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbb_cntrc = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lbl_Status = new Apzon.Controls.Controls.LabelLine();
		this.cbb_Status = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.lbl_Owner = new Apzon.Controls.Controls.LabelLine();
		this.cbb_Owner = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_Template = new Apzon.Controls.Editor.TextEditEx();
		this.chk_Renewal = new Apzon.Controls.Editor.CheckEditEx();
		this.lbl_Reminder = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ActiveItems = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Remark = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_ContractTemp = new Apzon.Controls.Editor.MemoEditEx();
		this.cbb_ResP = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbb_TimeResO = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cbb_Remind = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_ActiveItems = new Apzon.Controls.Editor.TextEditEx();
		this.spin_Resp = new DevExpress.XtraEditors.SpinEdit();
		this.spin_ResO = new DevExpress.XtraEditors.SpinEdit();
		this.spin_Remind = new DevExpress.XtraEditors.SpinEdit();
		this.date_Start = new Apzon.Controls.Editor.DateEditEx();
		this.date_EndDate = new Apzon.Controls.Editor.DateEditEx();
		((System.ComponentModel.ISupportInitialize)this.spl_Content).BeginInit();
		this.spl_Content.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rp_Customer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ContactPersonCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SrvcType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CntrcType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Template).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReponseU).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remarks2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Owner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Renewal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remarks1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RemU).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Content).BeginInit();
		this.tab_Content.SuspendLayout();
		this.tab_Items.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Items).BeginInit();
		this.ctm_Item.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlp_ItemGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SerialNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineDate.CalendarTimeProperties).BeginInit();
		this.tab_Coverage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pnl_Coverage).BeginInit();
		this.pnl_Coverage.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_WenS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_WenS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Including.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_Travel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Labor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Parts.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_WenE.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_WenE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonS.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonS.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Sun.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Sat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Fri.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Thu.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Wed.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Tue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Mon.Properties).BeginInit();
		this.tab_ServiceCalls.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ServiceCall).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ServiceCall).BeginInit();
		this.tab_SalesData.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl6).BeginInit();
		this.panelControl6.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_RecTemp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_RecTemp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_RecTrans).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_RecTrans).BeginInit();
		this.tab_Attachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.cbb_SrvcType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_cntrc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Owner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Template.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Renewal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ContractTemp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_ResP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_TimeResO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Remind.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ActiveItems.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Resp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_ResO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Remind.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Start.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Start.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties).BeginInit();
		base.SuspendLayout();
		this.spl_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.spl_Content.Horizontal = false;
		this.spl_Content.Location = new System.Drawing.Point(5, 33);
		this.spl_Content.Name = "spl_Content";
		this.spl_Content.Panel1.Controls.Add(this.grd_Doc);
		this.spl_Content.Panel1.Text = "Panel1";
		this.spl_Content.Panel2.Controls.Add(this.tab_Content);
		this.spl_Content.Panel2.Text = "Panel2";
		this.spl_Content.Size = new System.Drawing.Size(754, 437);
		this.spl_Content.SplitterPosition = 200;
		this.spl_Content.TabIndex = 0;
		this.spl_Content.Text = "spl_Content";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.rp_Customer, this.rep_Date, this.rep_ContactPersonCode, this.rep_SrvcType, this.rep_CntrcType, this.rep_ReponseU, this.rep_Status, this.rep_Owner, this.rep_Renewal, this.rep_RemU,
			this.rep_Remarks2, this.rep_Remarks1, this.rep_Template
		});
		this.grd_Doc.Size = new System.Drawing.Size(754, 200);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_EditorKeyDown);
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.ctm_Menu_Add });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(97, 26);
		this.ctm_Menu_Add.Name = "ctm_Menu_Add";
		this.ctm_Menu_Add.Size = new System.Drawing.Size(96, 22);
		this.ctm_Menu_Add.Text = "Add";
		this.ctm_Menu_Add.Click += new System.EventHandler(this.ctm_Menu_Add_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[25]
		{
			this.cl_ContractNo, this.cl_CustomerCode, this.cl_CustomerName, this.cl_ContactPerson, this.cl_TelephoneNo, this.cl_StartDate, this.cl_EndDate, this.cl_TerminationDate, this.cl_Description, this.cl_AtcEntry,
			this.cl_SrvcType, this.cl_CntrcType, this.cl_CntrcTmplt, this.cl_ResponseTime, this.cl_ResponseUnit, this.cl_ResolutionTime, this.cl_ResolutionUnit, this.cl_Remarks, this.cl_Status, this.cl_Owner,
			this.cl_Renewal, this.cl_RemTime, this.cl_RemUnit, this.cl_ActiveItems, this.cl_CTRemarks
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oSericeContract";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OCTR";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEdit);
		this.gv_Doc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Doc_CustomRowCellEditForEditing);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.HiddenEditor += new System.EventHandler(this.gv_Doc_HiddenEditor);
		this.gv_Doc.ShownEditor += new System.EventHandler(this.gv_Doc_ShownEditor);
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.cl_ContractNo.Caption = "Contract No.";
		this.cl_ContractNo.CurrencyDateField = null;
		this.cl_ContractNo.CurrencyField = null;
		this.cl_ContractNo.CurrencyFixed = null;
		this.cl_ContractNo.CurrencyFixedRate = 0.0;
		this.cl_ContractNo.CurrencyRateField = null;
		this.cl_ContractNo.FieldName = "ContractID";
		this.cl_ContractNo.Name = "cl_ContractNo";
		this.cl_ContractNo.OptionsColumn.AllowEdit = false;
		this.cl_ContractNo.OptionsColumn.ReadOnly = true;
		this.cl_ContractNo.PrimaryKey = true;
		this.cl_ContractNo.UnitField = null;
		this.cl_ContractNo.Width = 77;
		this.cl_CustomerCode.Caption = "Customer Code";
		this.cl_CustomerCode.ColumnEdit = this.rp_Customer;
		this.cl_CustomerCode.CurrencyDateField = null;
		this.cl_CustomerCode.CurrencyField = null;
		this.cl_CustomerCode.CurrencyFixed = null;
		this.cl_CustomerCode.CurrencyFixedRate = 0.0;
		this.cl_CustomerCode.CurrencyRateField = null;
		this.cl_CustomerCode.FieldName = "CstmrCode";
		this.cl_CustomerCode.MinWidth = 100;
		this.cl_CustomerCode.Name = "cl_CustomerCode";
		this.cl_CustomerCode.PrimaryKey = false;
		this.cl_CustomerCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.cl_CustomerCode.UnitField = null;
		this.cl_CustomerCode.Visible = true;
		this.cl_CustomerCode.VisibleIndex = 0;
		this.cl_CustomerCode.Width = 100;
		this.rp_Customer.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rp_Customer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rp_Customer.Name = "rp_Customer";
		this.rp_Customer.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rp_Customer_ButtonPressed);
		this.cl_CustomerName.Caption = "Customer Name";
		this.cl_CustomerName.CurrencyDateField = null;
		this.cl_CustomerName.CurrencyField = null;
		this.cl_CustomerName.CurrencyFixed = null;
		this.cl_CustomerName.CurrencyFixedRate = 0.0;
		this.cl_CustomerName.CurrencyRateField = null;
		this.cl_CustomerName.FieldName = "CstmrName";
		this.cl_CustomerName.Name = "cl_CustomerName";
		this.cl_CustomerName.PrimaryKey = false;
		this.cl_CustomerName.UnitField = null;
		this.cl_CustomerName.Visible = true;
		this.cl_CustomerName.VisibleIndex = 1;
		this.cl_CustomerName.Width = 106;
		this.cl_ContactPerson.Caption = "Contact Person";
		this.cl_ContactPerson.ColumnEdit = this.rep_ContactPersonCode;
		this.cl_ContactPerson.CurrencyDateField = null;
		this.cl_ContactPerson.CurrencyField = null;
		this.cl_ContactPerson.CurrencyFixed = null;
		this.cl_ContactPerson.CurrencyFixedRate = 0.0;
		this.cl_ContactPerson.CurrencyRateField = null;
		this.cl_ContactPerson.FieldName = "CntctCode";
		this.cl_ContactPerson.Name = "cl_ContactPerson";
		this.cl_ContactPerson.PrimaryKey = false;
		this.cl_ContactPerson.UnitField = null;
		this.cl_ContactPerson.Visible = true;
		this.cl_ContactPerson.VisibleIndex = 2;
		this.rep_ContactPersonCode.AutoHeight = false;
		this.rep_ContactPersonCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ContactPersonCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[4]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CntctCode", "CntctCode", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name "),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CardCode", "CardCode", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Telephone", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_ContactPersonCode.DisplayMember = "Name";
		this.rep_ContactPersonCode.Name = "rep_ContactPersonCode";
		this.rep_ContactPersonCode.NullText = "";
		this.rep_ContactPersonCode.ValueMember = "CntctCode";
		this.rep_ContactPersonCode.EditValueChanged += new System.EventHandler(this.rep_ContactPersonCode_EditValueChanged);
		this.rep_ContactPersonCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_ContactPersonCode_EditValueChanging);
		this.cl_TelephoneNo.Caption = "Telephone No.";
		this.cl_TelephoneNo.CurrencyDateField = null;
		this.cl_TelephoneNo.CurrencyField = null;
		this.cl_TelephoneNo.CurrencyFixed = null;
		this.cl_TelephoneNo.CurrencyFixedRate = 0.0;
		this.cl_TelephoneNo.CurrencyRateField = null;
		this.cl_TelephoneNo.FieldName = "TelephoneNo";
		this.cl_TelephoneNo.Name = "cl_TelephoneNo";
		this.cl_TelephoneNo.OptionsColumn.AllowEdit = false;
		this.cl_TelephoneNo.OptionsColumn.ReadOnly = true;
		this.cl_TelephoneNo.PrimaryKey = false;
		this.cl_TelephoneNo.UnitField = null;
		this.cl_TelephoneNo.Visible = true;
		this.cl_TelephoneNo.VisibleIndex = 3;
		this.cl_StartDate.Caption = "Start Date";
		this.cl_StartDate.ColumnEdit = this.rep_Date;
		this.cl_StartDate.CurrencyDateField = null;
		this.cl_StartDate.CurrencyField = null;
		this.cl_StartDate.CurrencyFixed = null;
		this.cl_StartDate.CurrencyFixedRate = 0.0;
		this.cl_StartDate.CurrencyRateField = null;
		this.cl_StartDate.DisplayFormat.FormatString = "d";
		this.cl_StartDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_StartDate.FieldName = "StartDate";
		this.cl_StartDate.Name = "cl_StartDate";
		this.cl_StartDate.PrimaryKey = false;
		this.cl_StartDate.UnitField = null;
		this.cl_StartDate.Visible = true;
		this.cl_StartDate.VisibleIndex = 4;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.Name = "rep_Date";
		this.cl_EndDate.Caption = "End Date";
		this.cl_EndDate.ColumnEdit = this.rep_Date;
		this.cl_EndDate.CurrencyDateField = null;
		this.cl_EndDate.CurrencyField = null;
		this.cl_EndDate.CurrencyFixed = null;
		this.cl_EndDate.CurrencyFixedRate = 0.0;
		this.cl_EndDate.CurrencyRateField = null;
		this.cl_EndDate.DisplayFormat.FormatString = "d";
		this.cl_EndDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_EndDate.FieldName = "EndDate";
		this.cl_EndDate.Name = "cl_EndDate";
		this.cl_EndDate.PrimaryKey = false;
		this.cl_EndDate.UnitField = null;
		this.cl_EndDate.Visible = true;
		this.cl_EndDate.VisibleIndex = 5;
		this.cl_TerminationDate.Caption = "Termination Date";
		this.cl_TerminationDate.ColumnEdit = this.rep_Date;
		this.cl_TerminationDate.CurrencyDateField = null;
		this.cl_TerminationDate.CurrencyField = null;
		this.cl_TerminationDate.CurrencyFixed = null;
		this.cl_TerminationDate.CurrencyFixedRate = 0.0;
		this.cl_TerminationDate.CurrencyRateField = null;
		this.cl_TerminationDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_TerminationDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_TerminationDate.FieldName = "TermDate";
		this.cl_TerminationDate.Name = "cl_TerminationDate";
		this.cl_TerminationDate.PrimaryKey = false;
		this.cl_TerminationDate.UnitField = null;
		this.cl_TerminationDate.Visible = true;
		this.cl_TerminationDate.VisibleIndex = 6;
		this.cl_Description.Caption = "Description";
		this.cl_Description.CurrencyDateField = null;
		this.cl_Description.CurrencyField = null;
		this.cl_Description.CurrencyFixed = null;
		this.cl_Description.CurrencyFixedRate = 0.0;
		this.cl_Description.CurrencyRateField = null;
		this.cl_Description.FieldName = "Descriptio";
		this.cl_Description.Name = "cl_Description";
		this.cl_Description.PrimaryKey = false;
		this.cl_Description.UnitField = null;
		this.cl_Description.Visible = true;
		this.cl_Description.VisibleIndex = 7;
		this.cl_Description.Width = 98;
		this.cl_AtcEntry.Caption = "AtcEntry";
		this.cl_AtcEntry.CurrencyDateField = null;
		this.cl_AtcEntry.CurrencyField = null;
		this.cl_AtcEntry.CurrencyFixed = null;
		this.cl_AtcEntry.CurrencyFixedRate = 0.0;
		this.cl_AtcEntry.CurrencyRateField = null;
		this.cl_AtcEntry.FieldName = "AtcEntry";
		this.cl_AtcEntry.Name = "cl_AtcEntry";
		this.cl_AtcEntry.PrimaryKey = false;
		this.cl_AtcEntry.UnitField = null;
		this.cl_SrvcType.Caption = "Service Type";
		this.cl_SrvcType.ColumnEdit = this.rep_SrvcType;
		this.cl_SrvcType.CurrencyDateField = null;
		this.cl_SrvcType.CurrencyField = null;
		this.cl_SrvcType.CurrencyFixed = null;
		this.cl_SrvcType.CurrencyFixedRate = 0.0;
		this.cl_SrvcType.CurrencyRateField = null;
		this.cl_SrvcType.FieldName = "SrvcType";
		this.cl_SrvcType.Name = "cl_SrvcType";
		this.cl_SrvcType.PrimaryKey = false;
		this.cl_SrvcType.UnitField = null;
		this.cl_SrvcType.Visible = true;
		this.cl_SrvcType.VisibleIndex = 8;
		this.rep_SrvcType.AutoHeight = false;
		this.rep_SrvcType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SrvcType.DisplayMember = "Name";
		this.rep_SrvcType.Name = "rep_SrvcType";
		this.rep_SrvcType.NullText = "";
		this.rep_SrvcType.ValueMember = "Code";
		this.cl_CntrcType.Caption = "Contract Type";
		this.cl_CntrcType.ColumnEdit = this.rep_CntrcType;
		this.cl_CntrcType.CurrencyDateField = null;
		this.cl_CntrcType.CurrencyField = null;
		this.cl_CntrcType.CurrencyFixed = null;
		this.cl_CntrcType.CurrencyFixedRate = 0.0;
		this.cl_CntrcType.CurrencyRateField = null;
		this.cl_CntrcType.FieldName = "CntrcType";
		this.cl_CntrcType.Name = "cl_CntrcType";
		this.cl_CntrcType.PrimaryKey = false;
		this.cl_CntrcType.UnitField = null;
		this.cl_CntrcType.Visible = true;
		this.cl_CntrcType.VisibleIndex = 9;
		this.rep_CntrcType.AutoHeight = false;
		this.rep_CntrcType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CntrcType.DisplayMember = "Name";
		this.rep_CntrcType.Name = "rep_CntrcType";
		this.rep_CntrcType.NullText = "";
		this.rep_CntrcType.ValueMember = "Code";
		this.rep_CntrcType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_CntrcType_EditValueChanging);
		this.cl_CntrcTmplt.Caption = "Template";
		this.cl_CntrcTmplt.ColumnEdit = this.rep_Template;
		this.cl_CntrcTmplt.CurrencyDateField = null;
		this.cl_CntrcTmplt.CurrencyField = null;
		this.cl_CntrcTmplt.CurrencyFixed = null;
		this.cl_CntrcTmplt.CurrencyFixedRate = 0.0;
		this.cl_CntrcTmplt.CurrencyRateField = null;
		this.cl_CntrcTmplt.FieldName = "CntrcTmplt";
		this.cl_CntrcTmplt.Name = "cl_CntrcTmplt";
		this.cl_CntrcTmplt.PrimaryKey = false;
		this.cl_CntrcTmplt.UnitField = null;
		this.cl_CntrcTmplt.Visible = true;
		this.cl_CntrcTmplt.VisibleIndex = 10;
		this.rep_Template.AutoHeight = false;
		this.rep_Template.AutoLoadDataSource = true;
		this.rep_Template.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Template.ContainerControl = this;
		this.rep_Template.DisplayMember = "TmpltName";
		this.rep_Template.DisplayMember2 = "";
		this.rep_Template.LoadDataSourceWhenEnter = false;
		this.rep_Template.Name = "rep_Template";
		this.rep_Template.SeparateCharacterForMulti = ';';
		this.rep_Template.SeparateString = ", ";
		this.rep_Template.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListContractTemplate;
		this.rep_Template.UsingAutoCompleteBox = false;
		this.rep_Template.UsingDataSource = true;
		this.rep_Template.ValidateWhenNoDataSource = false;
		this.rep_Template.ValueMember = "TmpltName";
		this.cl_ResponseTime.Caption = "Response Time";
		this.cl_ResponseTime.CurrencyDateField = null;
		this.cl_ResponseTime.CurrencyField = null;
		this.cl_ResponseTime.CurrencyFixed = null;
		this.cl_ResponseTime.CurrencyFixedRate = 0.0;
		this.cl_ResponseTime.CurrencyRateField = null;
		this.cl_ResponseTime.FieldName = "ResponseV";
		this.cl_ResponseTime.Name = "cl_ResponseTime";
		this.cl_ResponseTime.PrimaryKey = false;
		this.cl_ResponseTime.UnitField = null;
		this.cl_ResponseTime.Visible = true;
		this.cl_ResponseTime.VisibleIndex = 11;
		this.cl_ResponseUnit.Caption = "Response Unit";
		this.cl_ResponseUnit.ColumnEdit = this.rep_ReponseU;
		this.cl_ResponseUnit.CurrencyDateField = null;
		this.cl_ResponseUnit.CurrencyField = null;
		this.cl_ResponseUnit.CurrencyFixed = null;
		this.cl_ResponseUnit.CurrencyFixedRate = 0.0;
		this.cl_ResponseUnit.CurrencyRateField = null;
		this.cl_ResponseUnit.FieldName = "ResponseU";
		this.cl_ResponseUnit.Name = "cl_ResponseUnit";
		this.cl_ResponseUnit.PrimaryKey = false;
		this.cl_ResponseUnit.UnitField = null;
		this.cl_ResponseUnit.Visible = true;
		this.cl_ResponseUnit.VisibleIndex = 12;
		this.rep_ReponseU.AutoHeight = false;
		this.rep_ReponseU.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ReponseU.DisplayMember = "Name";
		this.rep_ReponseU.Name = "rep_ReponseU";
		this.rep_ReponseU.NullText = "";
		this.rep_ReponseU.ValueMember = "Code";
		this.cl_ResolutionTime.Caption = "Resolution Time";
		this.cl_ResolutionTime.CurrencyDateField = null;
		this.cl_ResolutionTime.CurrencyField = null;
		this.cl_ResolutionTime.CurrencyFixed = null;
		this.cl_ResolutionTime.CurrencyFixedRate = 0.0;
		this.cl_ResolutionTime.CurrencyRateField = null;
		this.cl_ResolutionTime.FieldName = "ResponsVal";
		this.cl_ResolutionTime.Name = "cl_ResolutionTime";
		this.cl_ResolutionTime.PrimaryKey = false;
		this.cl_ResolutionTime.UnitField = null;
		this.cl_ResolutionTime.Visible = true;
		this.cl_ResolutionTime.VisibleIndex = 13;
		this.cl_ResolutionUnit.Caption = "Resolution Unit";
		this.cl_ResolutionUnit.ColumnEdit = this.rep_ReponseU;
		this.cl_ResolutionUnit.CurrencyDateField = null;
		this.cl_ResolutionUnit.CurrencyField = null;
		this.cl_ResolutionUnit.CurrencyFixed = null;
		this.cl_ResolutionUnit.CurrencyFixedRate = 0.0;
		this.cl_ResolutionUnit.CurrencyRateField = null;
		this.cl_ResolutionUnit.FieldName = "ResponsUnt";
		this.cl_ResolutionUnit.Name = "cl_ResolutionUnit";
		this.cl_ResolutionUnit.PrimaryKey = false;
		this.cl_ResolutionUnit.UnitField = null;
		this.cl_ResolutionUnit.Visible = true;
		this.cl_ResolutionUnit.VisibleIndex = 14;
		this.cl_Remarks.Caption = "Remarks";
		this.cl_Remarks.ColumnEdit = this.rep_Remarks2;
		this.cl_Remarks.CurrencyDateField = null;
		this.cl_Remarks.CurrencyField = null;
		this.cl_Remarks.CurrencyFixed = null;
		this.cl_Remarks.CurrencyFixedRate = 0.0;
		this.cl_Remarks.CurrencyRateField = null;
		this.cl_Remarks.FieldName = "Remarks2";
		this.cl_Remarks.Name = "cl_Remarks";
		this.cl_Remarks.PrimaryKey = false;
		this.cl_Remarks.UnitField = null;
		this.cl_Remarks.Visible = true;
		this.cl_Remarks.VisibleIndex = 15;
		this.rep_Remarks2.AutoHeight = false;
		this.rep_Remarks2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Remarks2.Name = "rep_Remarks2";
		this.rep_Remarks2.ShowIcon = false;
		this.cl_Status.Caption = "Status";
		this.cl_Status.ColumnEdit = this.rep_Status;
		this.cl_Status.CurrencyDateField = null;
		this.cl_Status.CurrencyField = null;
		this.cl_Status.CurrencyFixed = null;
		this.cl_Status.CurrencyFixedRate = 0.0;
		this.cl_Status.CurrencyRateField = null;
		this.cl_Status.FieldName = "Status";
		this.cl_Status.Name = "cl_Status";
		this.cl_Status.PrimaryKey = false;
		this.cl_Status.UnitField = null;
		this.cl_Status.Visible = true;
		this.cl_Status.VisibleIndex = 16;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.DisplayMember = "Name";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ValueMember = "Code";
		this.rep_Status.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_Status_EditValueChanging);
		this.cl_Owner.Caption = "Owner";
		this.cl_Owner.ColumnEdit = this.rep_Owner;
		this.cl_Owner.CurrencyDateField = null;
		this.cl_Owner.CurrencyField = null;
		this.cl_Owner.CurrencyFixed = null;
		this.cl_Owner.CurrencyFixedRate = 0.0;
		this.cl_Owner.CurrencyRateField = null;
		this.cl_Owner.FieldName = "Owner";
		this.cl_Owner.Name = "cl_Owner";
		this.cl_Owner.PrimaryKey = false;
		this.cl_Owner.UnitField = null;
		this.cl_Owner.Visible = true;
		this.cl_Owner.VisibleIndex = 17;
		this.rep_Owner.AutoHeight = false;
		this.rep_Owner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Owner.DisplayMember = "Name";
		this.rep_Owner.Name = "rep_Owner";
		this.rep_Owner.NullText = "";
		this.rep_Owner.ValueMember = "Code";
		this.cl_Renewal.Caption = "Renewal";
		this.cl_Renewal.ColumnEdit = this.rep_Renewal;
		this.cl_Renewal.CurrencyDateField = null;
		this.cl_Renewal.CurrencyField = null;
		this.cl_Renewal.CurrencyFixed = null;
		this.cl_Renewal.CurrencyFixedRate = 0.0;
		this.cl_Renewal.CurrencyRateField = null;
		this.cl_Renewal.FieldName = "Renewal";
		this.cl_Renewal.Name = "cl_Renewal";
		this.cl_Renewal.PrimaryKey = false;
		this.cl_Renewal.UnitField = null;
		this.cl_Renewal.Visible = true;
		this.cl_Renewal.VisibleIndex = 18;
		this.rep_Renewal.AutoHeight = false;
		this.rep_Renewal.Name = "rep_Renewal";
		this.rep_Renewal.ValueChecked = "Y";
		this.rep_Renewal.ValueUnchecked = "N";
		this.cl_RemTime.Caption = "Reminder Time";
		this.cl_RemTime.CurrencyDateField = null;
		this.cl_RemTime.CurrencyField = null;
		this.cl_RemTime.CurrencyFixed = null;
		this.cl_RemTime.CurrencyFixedRate = 0.0;
		this.cl_RemTime.CurrencyRateField = null;
		this.cl_RemTime.FieldName = "RemindVal";
		this.cl_RemTime.Name = "cl_RemTime";
		this.cl_RemTime.PrimaryKey = false;
		this.cl_RemTime.UnitField = null;
		this.cl_RemTime.Visible = true;
		this.cl_RemTime.VisibleIndex = 19;
		this.cl_RemUnit.Caption = "Reminder Unit";
		this.cl_RemUnit.CurrencyDateField = null;
		this.cl_RemUnit.CurrencyField = null;
		this.cl_RemUnit.CurrencyFixed = null;
		this.cl_RemUnit.CurrencyFixedRate = 0.0;
		this.cl_RemUnit.CurrencyRateField = null;
		this.cl_RemUnit.FieldName = "RemindUnit";
		this.cl_RemUnit.Name = "cl_RemUnit";
		this.cl_RemUnit.PrimaryKey = false;
		this.cl_RemUnit.UnitField = null;
		this.cl_RemUnit.Visible = true;
		this.cl_RemUnit.VisibleIndex = 20;
		this.cl_ActiveItems.Caption = "Active Items";
		this.cl_ActiveItems.CurrencyDateField = null;
		this.cl_ActiveItems.CurrencyField = null;
		this.cl_ActiveItems.CurrencyFixed = null;
		this.cl_ActiveItems.CurrencyFixedRate = 0.0;
		this.cl_ActiveItems.CurrencyRateField = null;
		this.cl_ActiveItems.FieldName = "CTR1Count";
		this.cl_ActiveItems.Name = "cl_ActiveItems";
		this.cl_ActiveItems.PrimaryKey = false;
		this.cl_ActiveItems.UnitField = null;
		this.cl_ActiveItems.Visible = true;
		this.cl_ActiveItems.VisibleIndex = 21;
		this.cl_CTRemarks.Caption = "Contract Template Remarks";
		this.cl_CTRemarks.ColumnEdit = this.rep_Remarks1;
		this.cl_CTRemarks.CurrencyDateField = null;
		this.cl_CTRemarks.CurrencyField = null;
		this.cl_CTRemarks.CurrencyFixed = null;
		this.cl_CTRemarks.CurrencyFixedRate = 0.0;
		this.cl_CTRemarks.CurrencyRateField = null;
		this.cl_CTRemarks.FieldName = "Remarks1";
		this.cl_CTRemarks.Name = "cl_CTRemarks";
		this.cl_CTRemarks.PrimaryKey = false;
		this.cl_CTRemarks.UnitField = null;
		this.cl_CTRemarks.Visible = true;
		this.cl_CTRemarks.VisibleIndex = 22;
		this.rep_Remarks1.AutoHeight = false;
		this.rep_Remarks1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Remarks1.Name = "rep_Remarks1";
		this.rep_Remarks1.ShowIcon = false;
		this.rep_RemU.AutoHeight = false;
		this.rep_RemU.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_RemU.DisplayMember = "Name";
		this.rep_RemU.Name = "rep_RemU";
		this.rep_RemU.NullText = "";
		this.rep_RemU.ValueMember = "Code";
		this.tab_Content.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tab_Content.IsSearchControl = false;
		this.tab_Content.Location = new System.Drawing.Point(0, 0);
		this.tab_Content.Name = "tab_Content";
		this.tab_Content.SelectedTabPage = this.tab_Items;
		this.tab_Content.Size = new System.Drawing.Size(754, 227);
		this.tab_Content.TabIndex = 0;
		this.tab_Content.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[5] { this.tab_Items, this.tab_Coverage, this.tab_ServiceCalls, this.tab_SalesData, this.tab_Attachments });
		this.tab_Items.Controls.Add(this.panelControl2);
		this.tab_Items.EditControl = null;
		this.tab_Items.EditValue = "Items";
		this.tab_Items.FieldName = null;
		this.tab_Items.IsSearchControl = false;
		this.tab_Items.Name = "tab_Items";
		this.tab_Items.ReadOnly = false;
		this.tab_Items.SettingInVisible = false;
		this.tab_Items.SettingReadonly = false;
		this.tab_Items.Size = new System.Drawing.Size(752, 202);
		this.tab_Items.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Items.Text = "Items";
		this.panelControl2.Controls.Add(this.grd_Items);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(752, 202);
		this.panelControl2.TabIndex = 0;
		this.grd_Items.AllowPasteExcel = true;
		this.grd_Items.ClipboardData = null;
		this.grd_Items.ContextMenuStrip = this.ctm_Item;
		this.grd_Items.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Items.Location = new System.Drawing.Point(2, 2);
		this.grd_Items.MainView = this.gv_Items;
		this.grd_Items.Name = "grd_Items";
		this.grd_Items.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_ItemCode, this.rep_LineDate, this.rep_SerialNumber, this.rlp_ItemGroup, this.rep_ItemName });
		this.grd_Items.Size = new System.Drawing.Size(748, 198);
		this.grd_Items.TabIndex = 0;
		this.grd_Items.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Items });
		this.grd_Items.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Items_EditorKeyDown);
		this.ctm_Item.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.del_ctm_Item });
		this.ctm_Item.Name = "ctm_Item";
		this.ctm_Item.Size = new System.Drawing.Size(135, 26);
		this.del_ctm_Item.Name = "del_ctm_Item";
		this.del_ctm_Item.Size = new System.Drawing.Size(134, 22);
		this.del_ctm_Item.Text = "Delete item";
		this.del_ctm_Item.Click += new System.EventHandler(this.del_ctm_Item_Click);
		this.gv_Items.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Items.ColumnKey = null;
		this.gv_Items.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.cl_ItemGroup, this.cl_ItemNo, this.cl_ItemDescription, this.cl_MfrSerialNo, this.cl_SerialNumber, this.cl_LineStartDate, this.cl_LineEndDate, this.cl_LineTerminationDate, this.cl_LineContractID, this.cl_LineItem });
		this.gv_Items.DisableFormSetting = false;
		this.gv_Items.DisplayException = false;
		this.gv_Items.EditColumn = null;
		this.gv_Items.FieldCurrency = null;
		this.gv_Items.FieldRate = null;
		this.gv_Items.FormId = "oSericeContract";
		this.gv_Items.GridControl = this.grd_Items;
		this.gv_Items.IgnoreFilter = false;
		this.gv_Items.IndexEditRow = -1;
		this.gv_Items.IndicatorWidth = 30;
		this.gv_Items.IsApzTable = false;
		this.gv_Items.IsInsertNewRow = false;
		this.gv_Items.IsLoadingData = false;
		this.gv_Items.IsMappingContextAction = false;
		this.gv_Items.ItemId = "CTR1";
		this.gv_Items.LstColumnAllowBatch = null;
		this.gv_Items.LstColumnNotAllowBatch = null;
		this.gv_Items.LstColumnNotEditable = null;
		this.gv_Items.LstColumNotSetting = null;
		this.gv_Items.Name = "gv_Items";
		this.gv_Items.OptionsView.ShowGroupPanel = false;
		this.gv_Items.UseBatchMod = false;
		this.gv_Items.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Items_CustomDrawRowIndicator);
		this.gv_Items.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Items_RowCellStyle);
		this.gv_Items.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Items_ShowingEditor);
		this.gv_Items.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Items_CellValueChanged);
		this.gv_Items.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Items_CellValueChanging);
		this.cl_ItemGroup.Caption = "Item Group";
		this.cl_ItemGroup.ColumnEdit = this.rlp_ItemGroup;
		this.cl_ItemGroup.CurrencyDateField = null;
		this.cl_ItemGroup.CurrencyField = null;
		this.cl_ItemGroup.CurrencyFixed = null;
		this.cl_ItemGroup.CurrencyFixedRate = 0.0;
		this.cl_ItemGroup.CurrencyRateField = null;
		this.cl_ItemGroup.FieldName = "ItemGroup";
		this.cl_ItemGroup.Name = "cl_ItemGroup";
		this.cl_ItemGroup.PrimaryKey = true;
		this.cl_ItemGroup.UnitField = null;
		this.cl_ItemGroup.Visible = true;
		this.cl_ItemGroup.VisibleIndex = 0;
		this.rlp_ItemGroup.AutoHeight = false;
		this.rlp_ItemGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlp_ItemGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rlp_ItemGroup.DisplayMember = "Name";
		this.rlp_ItemGroup.Name = "rlp_ItemGroup";
		this.rlp_ItemGroup.NullText = "";
		this.rlp_ItemGroup.ValueMember = "Code";
		this.rlp_ItemGroup.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(this.rlp_ItemGroup_CloseUp);
		this.cl_ItemNo.Caption = "Item No.";
		this.cl_ItemNo.ColumnEdit = this.rep_ItemCode;
		this.cl_ItemNo.CurrencyDateField = null;
		this.cl_ItemNo.CurrencyField = null;
		this.cl_ItemNo.CurrencyFixed = null;
		this.cl_ItemNo.CurrencyFixedRate = 0.0;
		this.cl_ItemNo.CurrencyRateField = null;
		this.cl_ItemNo.FieldName = "ItemCode";
		this.cl_ItemNo.Name = "cl_ItemNo";
		this.cl_ItemNo.PrimaryKey = false;
		this.cl_ItemNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.cl_ItemNo.UnitField = null;
		this.cl_ItemNo.Visible = true;
		this.cl_ItemNo.VisibleIndex = 1;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonPressed);
		this.cl_ItemDescription.Caption = "Item Descripton";
		this.cl_ItemDescription.ColumnEdit = this.rep_ItemName;
		this.cl_ItemDescription.CurrencyDateField = null;
		this.cl_ItemDescription.CurrencyField = null;
		this.cl_ItemDescription.CurrencyFixed = null;
		this.cl_ItemDescription.CurrencyFixedRate = 0.0;
		this.cl_ItemDescription.CurrencyRateField = null;
		this.cl_ItemDescription.FieldName = "ItemName";
		this.cl_ItemDescription.Name = "cl_ItemDescription";
		this.cl_ItemDescription.PrimaryKey = false;
		this.cl_ItemDescription.UnitField = null;
		this.cl_ItemDescription.Visible = true;
		this.cl_ItemDescription.VisibleIndex = 2;
		this.rep_ItemName.AutoHeight = false;
		this.rep_ItemName.AutoLoadDataSource = true;
		this.rep_ItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemName.ContainerControl = this;
		this.rep_ItemName.DisplayMember = "";
		this.rep_ItemName.DisplayMember2 = "";
		this.rep_ItemName.LoadDataSourceWhenEnter = false;
		this.rep_ItemName.Name = "rep_ItemName";
		this.rep_ItemName.SeparateCharacterForMulti = ';';
		this.rep_ItemName.SeparateString = ", ";
		this.rep_ItemName.UsingAutoCompleteBox = false;
		this.rep_ItemName.UsingDataSource = true;
		this.rep_ItemName.ValidateWhenNoDataSource = false;
		this.rep_ItemName.ValueMember = "";
		this.rep_ItemName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemName_ButtonClick);
		this.cl_MfrSerialNo.Caption = "Mfr Serial No.";
		this.cl_MfrSerialNo.ColumnEdit = this.rep_SerialNumber;
		this.cl_MfrSerialNo.CurrencyDateField = null;
		this.cl_MfrSerialNo.CurrencyField = null;
		this.cl_MfrSerialNo.CurrencyFixed = null;
		this.cl_MfrSerialNo.CurrencyFixedRate = 0.0;
		this.cl_MfrSerialNo.CurrencyRateField = null;
		this.cl_MfrSerialNo.FieldName = "ManufSN";
		this.cl_MfrSerialNo.Name = "cl_MfrSerialNo";
		this.cl_MfrSerialNo.PrimaryKey = false;
		this.cl_MfrSerialNo.UnitField = null;
		this.cl_MfrSerialNo.Visible = true;
		this.cl_MfrSerialNo.VisibleIndex = 3;
		this.rep_SerialNumber.AutoHeight = false;
		this.rep_SerialNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_SerialNumber.Name = "rep_SerialNumber";
		this.rep_SerialNumber.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_SerialNumber_ButtonPressed);
		this.cl_SerialNumber.Caption = "Serial Number";
		this.cl_SerialNumber.ColumnEdit = this.rep_SerialNumber;
		this.cl_SerialNumber.CurrencyDateField = null;
		this.cl_SerialNumber.CurrencyField = null;
		this.cl_SerialNumber.CurrencyFixed = null;
		this.cl_SerialNumber.CurrencyFixedRate = 0.0;
		this.cl_SerialNumber.CurrencyRateField = null;
		this.cl_SerialNumber.FieldName = "InternalSN";
		this.cl_SerialNumber.Name = "cl_SerialNumber";
		this.cl_SerialNumber.PrimaryKey = false;
		this.cl_SerialNumber.UnitField = null;
		this.cl_SerialNumber.Visible = true;
		this.cl_SerialNumber.VisibleIndex = 4;
		this.cl_LineStartDate.Caption = "Start Date";
		this.cl_LineStartDate.ColumnEdit = this.rep_LineDate;
		this.cl_LineStartDate.CurrencyDateField = null;
		this.cl_LineStartDate.CurrencyField = null;
		this.cl_LineStartDate.CurrencyFixed = null;
		this.cl_LineStartDate.CurrencyFixedRate = 0.0;
		this.cl_LineStartDate.CurrencyRateField = null;
		this.cl_LineStartDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_LineStartDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_LineStartDate.FieldName = "StartDate";
		this.cl_LineStartDate.Name = "cl_LineStartDate";
		this.cl_LineStartDate.PrimaryKey = false;
		this.cl_LineStartDate.UnitField = null;
		this.cl_LineStartDate.Visible = true;
		this.cl_LineStartDate.VisibleIndex = 5;
		this.rep_LineDate.AutoHeight = false;
		this.rep_LineDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LineDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LineDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rep_LineDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_LineDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rep_LineDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rep_LineDate.Mask.EditMask = "dd/MM/yyyy";
		this.rep_LineDate.Name = "rep_LineDate";
		this.cl_LineEndDate.Caption = "End Date";
		this.cl_LineEndDate.ColumnEdit = this.rep_LineDate;
		this.cl_LineEndDate.CurrencyDateField = null;
		this.cl_LineEndDate.CurrencyField = null;
		this.cl_LineEndDate.CurrencyFixed = null;
		this.cl_LineEndDate.CurrencyFixedRate = 0.0;
		this.cl_LineEndDate.CurrencyRateField = null;
		this.cl_LineEndDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_LineEndDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_LineEndDate.FieldName = "EndDate";
		this.cl_LineEndDate.Name = "cl_LineEndDate";
		this.cl_LineEndDate.PrimaryKey = false;
		this.cl_LineEndDate.UnitField = null;
		this.cl_LineEndDate.Visible = true;
		this.cl_LineEndDate.VisibleIndex = 6;
		this.cl_LineTerminationDate.Caption = "Termination Date";
		this.cl_LineTerminationDate.ColumnEdit = this.rep_LineDate;
		this.cl_LineTerminationDate.CurrencyDateField = null;
		this.cl_LineTerminationDate.CurrencyField = null;
		this.cl_LineTerminationDate.CurrencyFixed = null;
		this.cl_LineTerminationDate.CurrencyFixedRate = 0.0;
		this.cl_LineTerminationDate.CurrencyRateField = null;
		this.cl_LineTerminationDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_LineTerminationDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_LineTerminationDate.FieldName = "TermDate";
		this.cl_LineTerminationDate.Name = "cl_LineTerminationDate";
		this.cl_LineTerminationDate.PrimaryKey = false;
		this.cl_LineTerminationDate.UnitField = null;
		this.cl_LineTerminationDate.Visible = true;
		this.cl_LineTerminationDate.VisibleIndex = 7;
		this.cl_LineContractID.Caption = "ContractID";
		this.cl_LineContractID.CurrencyDateField = null;
		this.cl_LineContractID.CurrencyField = null;
		this.cl_LineContractID.CurrencyFixed = null;
		this.cl_LineContractID.CurrencyFixedRate = 0.0;
		this.cl_LineContractID.CurrencyRateField = null;
		this.cl_LineContractID.Name = "cl_LineContractID";
		this.cl_LineContractID.OptionsColumn.AllowEdit = false;
		this.cl_LineContractID.OptionsColumn.ReadOnly = true;
		this.cl_LineContractID.PrimaryKey = false;
		this.cl_LineContractID.UnitField = null;
		this.cl_LineItem.Caption = "Line";
		this.cl_LineItem.CurrencyDateField = null;
		this.cl_LineItem.CurrencyField = null;
		this.cl_LineItem.CurrencyFixed = null;
		this.cl_LineItem.CurrencyFixedRate = 0.0;
		this.cl_LineItem.CurrencyRateField = null;
		this.cl_LineItem.FieldName = "Line";
		this.cl_LineItem.Name = "cl_LineItem";
		this.cl_LineItem.PrimaryKey = false;
		this.cl_LineItem.UnitField = null;
		this.tab_Coverage.Controls.Add(this.pnl_Coverage);
		this.tab_Coverage.EditControl = null;
		this.tab_Coverage.EditValue = "Coverage";
		this.tab_Coverage.FieldName = null;
		this.tab_Coverage.IsSearchControl = false;
		this.tab_Coverage.Name = "tab_Coverage";
		this.tab_Coverage.ReadOnly = false;
		this.tab_Coverage.SettingInVisible = false;
		this.tab_Coverage.SettingReadonly = false;
		this.tab_Coverage.Size = new System.Drawing.Size(752, 202);
		this.tab_Coverage.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Coverage.Text = "Coverage";
		this.pnl_Coverage.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.pnl_Coverage.Appearance.Options.UseBackColor = true;
		this.pnl_Coverage.Controls.Add(this.date_WenS);
		this.pnl_Coverage.Controls.Add(this.date_TueE);
		this.pnl_Coverage.Controls.Add(this.date_TueS);
		this.pnl_Coverage.Controls.Add(this.date_MonE);
		this.pnl_Coverage.Controls.Add(this.chk_Including);
		this.pnl_Coverage.Controls.Add(this.panelControl4);
		this.pnl_Coverage.Controls.Add(this.date_SunE);
		this.pnl_Coverage.Controls.Add(this.date_FriE);
		this.pnl_Coverage.Controls.Add(this.date_SatE);
		this.pnl_Coverage.Controls.Add(this.date_SunS);
		this.pnl_Coverage.Controls.Add(this.date_ThuE);
		this.pnl_Coverage.Controls.Add(this.date_WenE);
		this.pnl_Coverage.Controls.Add(this.date_ThuS);
		this.pnl_Coverage.Controls.Add(this.date_FriS);
		this.pnl_Coverage.Controls.Add(this.date_SatS);
		this.pnl_Coverage.Controls.Add(this.date_MonS);
		this.pnl_Coverage.Controls.Add(this.lbl_EndTime);
		this.pnl_Coverage.Controls.Add(this.lbl_StartTime);
		this.pnl_Coverage.Controls.Add(this.chk_Sun);
		this.pnl_Coverage.Controls.Add(this.chk_Sat);
		this.pnl_Coverage.Controls.Add(this.chk_Fri);
		this.pnl_Coverage.Controls.Add(this.chk_Thu);
		this.pnl_Coverage.Controls.Add(this.chk_Wed);
		this.pnl_Coverage.Controls.Add(this.chk_Tue);
		this.pnl_Coverage.Controls.Add(this.chk_Mon);
		this.pnl_Coverage.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnl_Coverage.Location = new System.Drawing.Point(0, 0);
		this.pnl_Coverage.Name = "pnl_Coverage";
		this.pnl_Coverage.Size = new System.Drawing.Size(752, 202);
		this.pnl_Coverage.TabIndex = 0;
		this.date_WenS.EditControl = null;
		this.date_WenS.EditValue = null;
		this.date_WenS.FieldName = null;
		this.date_WenS.Location = new System.Drawing.Point(127, 72);
		this.date_WenS.Name = "date_WenS";
		this.date_WenS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_WenS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_WenS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_WenS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_WenS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_WenS.Properties.IsSearchControl = false;
		this.date_WenS.Properties.Mask.EditMask = "HH:mm";
		this.date_WenS.SettingInVisible = false;
		this.date_WenS.SettingReadonly = false;
		this.date_WenS.Size = new System.Drawing.Size(100, 20);
		this.date_WenS.TabIndex = 28;
		this.date_TueE.EditControl = null;
		this.date_TueE.EditValue = null;
		this.date_TueE.FieldName = null;
		this.date_TueE.Location = new System.Drawing.Point(273, 48);
		this.date_TueE.Name = "date_TueE";
		this.date_TueE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_TueE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_TueE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_TueE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_TueE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_TueE.Properties.IsSearchControl = false;
		this.date_TueE.Properties.Mask.EditMask = "HH:mm";
		this.date_TueE.SettingInVisible = false;
		this.date_TueE.SettingReadonly = false;
		this.date_TueE.Size = new System.Drawing.Size(100, 20);
		this.date_TueE.TabIndex = 27;
		this.date_TueS.EditControl = null;
		this.date_TueS.EditValue = null;
		this.date_TueS.FieldName = null;
		this.date_TueS.Location = new System.Drawing.Point(127, 48);
		this.date_TueS.Name = "date_TueS";
		this.date_TueS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_TueS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_TueS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_TueS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_TueS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_TueS.Properties.IsSearchControl = false;
		this.date_TueS.Properties.Mask.EditMask = "HH:mm";
		this.date_TueS.SettingInVisible = false;
		this.date_TueS.SettingReadonly = false;
		this.date_TueS.Size = new System.Drawing.Size(100, 20);
		this.date_TueS.TabIndex = 26;
		this.date_MonE.EditControl = null;
		this.date_MonE.EditValue = null;
		this.date_MonE.FieldName = null;
		this.date_MonE.Location = new System.Drawing.Point(273, 24);
		this.date_MonE.Name = "date_MonE";
		this.date_MonE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_MonE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_MonE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_MonE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_MonE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_MonE.Properties.IsSearchControl = false;
		this.date_MonE.Properties.Mask.EditMask = "HH:mm";
		this.date_MonE.SettingInVisible = false;
		this.date_MonE.SettingReadonly = false;
		this.date_MonE.Size = new System.Drawing.Size(100, 20);
		this.date_MonE.TabIndex = 25;
		this.chk_Including.EditControl = null;
		this.chk_Including.FieldName = null;
		this.chk_Including.IsSearchControl = false;
		this.chk_Including.Location = new System.Drawing.Point(396, 123);
		this.chk_Including.Name = "chk_Including";
		this.chk_Including.Properties.Caption = "Including Holidays";
		this.chk_Including.SettingInVisible = false;
		this.chk_Including.SettingReadonly = false;
		this.chk_Including.Size = new System.Drawing.Size(195, 20);
		this.chk_Including.TabIndex = 24;
		this.panelControl4.Controls.Add(this.chk_Travel);
		this.panelControl4.Controls.Add(this.chk_Labor);
		this.panelControl4.Controls.Add(this.chk_Parts);
		this.panelControl4.Controls.Add(this.labelLine1);
		this.panelControl4.Location = new System.Drawing.Point(390, 24);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(200, 96);
		this.panelControl4.TabIndex = 23;
		this.chk_Travel.EditControl = null;
		this.chk_Travel.FieldName = null;
		this.chk_Travel.IsSearchControl = false;
		this.chk_Travel.Location = new System.Drawing.Point(6, 72);
		this.chk_Travel.Name = "chk_Travel";
		this.chk_Travel.Properties.Caption = "Travel";
		this.chk_Travel.SettingInVisible = false;
		this.chk_Travel.SettingReadonly = false;
		this.chk_Travel.Size = new System.Drawing.Size(75, 20);
		this.chk_Travel.TabIndex = 3;
		this.chk_Labor.EditControl = null;
		this.chk_Labor.FieldName = null;
		this.chk_Labor.IsSearchControl = false;
		this.chk_Labor.Location = new System.Drawing.Point(6, 48);
		this.chk_Labor.Name = "chk_Labor";
		this.chk_Labor.Properties.Caption = "Labor";
		this.chk_Labor.SettingInVisible = false;
		this.chk_Labor.SettingReadonly = false;
		this.chk_Labor.Size = new System.Drawing.Size(75, 20);
		this.chk_Labor.TabIndex = 2;
		this.chk_Parts.EditControl = null;
		this.chk_Parts.FieldName = null;
		this.chk_Parts.IsSearchControl = false;
		this.chk_Parts.Location = new System.Drawing.Point(6, 25);
		this.chk_Parts.Name = "chk_Parts";
		this.chk_Parts.Properties.Caption = "Parts";
		this.chk_Parts.SettingInVisible = false;
		this.chk_Parts.SettingReadonly = false;
		this.chk_Parts.Size = new System.Drawing.Size(75, 20);
		this.chk_Parts.TabIndex = 1;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Include";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(6, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = false;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Include";
		this.date_SunE.EditControl = null;
		this.date_SunE.EditValue = null;
		this.date_SunE.FieldName = null;
		this.date_SunE.Location = new System.Drawing.Point(273, 168);
		this.date_SunE.Name = "date_SunE";
		this.date_SunE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_SunE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_SunE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SunE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_SunE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SunE.Properties.IsSearchControl = false;
		this.date_SunE.Properties.Mask.EditMask = "HH:mm";
		this.date_SunE.SettingInVisible = false;
		this.date_SunE.SettingReadonly = false;
		this.date_SunE.Size = new System.Drawing.Size(100, 20);
		this.date_SunE.TabIndex = 22;
		this.date_FriE.EditControl = null;
		this.date_FriE.EditValue = null;
		this.date_FriE.FieldName = null;
		this.date_FriE.Location = new System.Drawing.Point(273, 120);
		this.date_FriE.Name = "date_FriE";
		this.date_FriE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FriE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_FriE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FriE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_FriE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FriE.Properties.IsSearchControl = false;
		this.date_FriE.Properties.Mask.EditMask = "HH:mm";
		this.date_FriE.SettingInVisible = false;
		this.date_FriE.SettingReadonly = false;
		this.date_FriE.Size = new System.Drawing.Size(100, 20);
		this.date_FriE.TabIndex = 21;
		this.date_SatE.EditControl = null;
		this.date_SatE.EditValue = null;
		this.date_SatE.FieldName = null;
		this.date_SatE.Location = new System.Drawing.Point(273, 144);
		this.date_SatE.Name = "date_SatE";
		this.date_SatE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_SatE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_SatE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SatE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_SatE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SatE.Properties.IsSearchControl = false;
		this.date_SatE.Properties.Mask.EditMask = "HH:mm";
		this.date_SatE.SettingInVisible = false;
		this.date_SatE.SettingReadonly = false;
		this.date_SatE.Size = new System.Drawing.Size(100, 20);
		this.date_SatE.TabIndex = 20;
		this.date_SunS.EditControl = null;
		this.date_SunS.EditValue = null;
		this.date_SunS.FieldName = null;
		this.date_SunS.Location = new System.Drawing.Point(127, 168);
		this.date_SunS.Name = "date_SunS";
		this.date_SunS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_SunS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_SunS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SunS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_SunS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SunS.Properties.IsSearchControl = false;
		this.date_SunS.Properties.Mask.EditMask = "HH:mm";
		this.date_SunS.SettingInVisible = false;
		this.date_SunS.SettingReadonly = false;
		this.date_SunS.Size = new System.Drawing.Size(100, 20);
		this.date_SunS.TabIndex = 19;
		this.date_ThuE.EditControl = null;
		this.date_ThuE.EditValue = null;
		this.date_ThuE.FieldName = null;
		this.date_ThuE.Location = new System.Drawing.Point(273, 96);
		this.date_ThuE.Name = "date_ThuE";
		this.date_ThuE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ThuE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_ThuE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_ThuE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_ThuE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_ThuE.Properties.IsSearchControl = false;
		this.date_ThuE.Properties.Mask.EditMask = "HH:mm";
		this.date_ThuE.SettingInVisible = false;
		this.date_ThuE.SettingReadonly = false;
		this.date_ThuE.Size = new System.Drawing.Size(100, 20);
		this.date_ThuE.TabIndex = 18;
		this.date_WenE.EditControl = null;
		this.date_WenE.EditValue = null;
		this.date_WenE.FieldName = null;
		this.date_WenE.Location = new System.Drawing.Point(273, 72);
		this.date_WenE.Name = "date_WenE";
		this.date_WenE.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_WenE.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_WenE.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_WenE.Properties.EditFormat.FormatString = "HH:mm";
		this.date_WenE.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_WenE.Properties.IsSearchControl = false;
		this.date_WenE.Properties.Mask.EditMask = "HH:mm";
		this.date_WenE.SettingInVisible = false;
		this.date_WenE.SettingReadonly = false;
		this.date_WenE.Size = new System.Drawing.Size(100, 20);
		this.date_WenE.TabIndex = 17;
		this.date_ThuS.EditControl = null;
		this.date_ThuS.EditValue = null;
		this.date_ThuS.FieldName = null;
		this.date_ThuS.Location = new System.Drawing.Point(127, 96);
		this.date_ThuS.Name = "date_ThuS";
		this.date_ThuS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ThuS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_ThuS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_ThuS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_ThuS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_ThuS.Properties.IsSearchControl = false;
		this.date_ThuS.Properties.Mask.EditMask = "HH:mm";
		this.date_ThuS.SettingInVisible = false;
		this.date_ThuS.SettingReadonly = false;
		this.date_ThuS.Size = new System.Drawing.Size(100, 20);
		this.date_ThuS.TabIndex = 13;
		this.date_FriS.EditControl = null;
		this.date_FriS.EditValue = null;
		this.date_FriS.FieldName = null;
		this.date_FriS.Location = new System.Drawing.Point(127, 120);
		this.date_FriS.Name = "date_FriS";
		this.date_FriS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FriS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_FriS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FriS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_FriS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FriS.Properties.IsSearchControl = false;
		this.date_FriS.Properties.Mask.EditMask = "HH:mm";
		this.date_FriS.SettingInVisible = false;
		this.date_FriS.SettingReadonly = false;
		this.date_FriS.Size = new System.Drawing.Size(100, 20);
		this.date_FriS.TabIndex = 12;
		this.date_SatS.EditControl = null;
		this.date_SatS.EditValue = null;
		this.date_SatS.FieldName = null;
		this.date_SatS.Location = new System.Drawing.Point(127, 144);
		this.date_SatS.Name = "date_SatS";
		this.date_SatS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_SatS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_SatS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SatS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_SatS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_SatS.Properties.IsSearchControl = false;
		this.date_SatS.Properties.Mask.EditMask = "HH:mm";
		this.date_SatS.SettingInVisible = false;
		this.date_SatS.SettingReadonly = false;
		this.date_SatS.Size = new System.Drawing.Size(100, 20);
		this.date_SatS.TabIndex = 11;
		this.date_MonS.EditControl = null;
		this.date_MonS.EditValue = null;
		this.date_MonS.FieldName = null;
		this.date_MonS.Location = new System.Drawing.Point(127, 24);
		this.date_MonS.Name = "date_MonS";
		this.date_MonS.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_MonS.Properties.DisplayFormat.FormatString = "HH:mm";
		this.date_MonS.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_MonS.Properties.EditFormat.FormatString = "HH:mm";
		this.date_MonS.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_MonS.Properties.IsSearchControl = false;
		this.date_MonS.Properties.Mask.EditMask = "HH:mm";
		this.date_MonS.SettingInVisible = false;
		this.date_MonS.SettingReadonly = false;
		this.date_MonS.Size = new System.Drawing.Size(100, 20);
		this.date_MonS.TabIndex = 9;
		this.lbl_EndTime.Location = new System.Drawing.Point(273, 6);
		this.lbl_EndTime.Name = "lbl_EndTime";
		this.lbl_EndTime.Size = new System.Drawing.Size(43, 13);
		this.lbl_EndTime.TabIndex = 8;
		this.lbl_EndTime.Text = "End Time";
		this.lbl_StartTime.Location = new System.Drawing.Point(127, 6);
		this.lbl_StartTime.Name = "lbl_StartTime";
		this.lbl_StartTime.Size = new System.Drawing.Size(49, 13);
		this.lbl_StartTime.TabIndex = 7;
		this.lbl_StartTime.Text = "Start Time";
		this.chk_Sun.EditControl = null;
		this.chk_Sun.FieldName = null;
		this.chk_Sun.IsSearchControl = false;
		this.chk_Sun.Location = new System.Drawing.Point(6, 168);
		this.chk_Sun.Name = "chk_Sun";
		this.chk_Sun.Properties.Caption = "Sunday";
		this.chk_Sun.SettingInVisible = false;
		this.chk_Sun.SettingReadonly = false;
		this.chk_Sun.Size = new System.Drawing.Size(75, 20);
		this.chk_Sun.TabIndex = 6;
		this.chk_Sat.EditControl = null;
		this.chk_Sat.FieldName = null;
		this.chk_Sat.IsSearchControl = false;
		this.chk_Sat.Location = new System.Drawing.Point(6, 144);
		this.chk_Sat.Name = "chk_Sat";
		this.chk_Sat.Properties.Caption = "Saturday";
		this.chk_Sat.SettingInVisible = false;
		this.chk_Sat.SettingReadonly = false;
		this.chk_Sat.Size = new System.Drawing.Size(75, 20);
		this.chk_Sat.TabIndex = 5;
		this.chk_Fri.EditControl = null;
		this.chk_Fri.FieldName = null;
		this.chk_Fri.IsSearchControl = false;
		this.chk_Fri.Location = new System.Drawing.Point(6, 120);
		this.chk_Fri.Name = "chk_Fri";
		this.chk_Fri.Properties.Caption = "Friday";
		this.chk_Fri.SettingInVisible = false;
		this.chk_Fri.SettingReadonly = false;
		this.chk_Fri.Size = new System.Drawing.Size(75, 20);
		this.chk_Fri.TabIndex = 4;
		this.chk_Thu.EditControl = null;
		this.chk_Thu.FieldName = null;
		this.chk_Thu.IsSearchControl = false;
		this.chk_Thu.Location = new System.Drawing.Point(6, 96);
		this.chk_Thu.Name = "chk_Thu";
		this.chk_Thu.Properties.Caption = "Thursday";
		this.chk_Thu.SettingInVisible = false;
		this.chk_Thu.SettingReadonly = false;
		this.chk_Thu.Size = new System.Drawing.Size(75, 20);
		this.chk_Thu.TabIndex = 3;
		this.chk_Wed.EditControl = null;
		this.chk_Wed.FieldName = null;
		this.chk_Wed.IsSearchControl = false;
		this.chk_Wed.Location = new System.Drawing.Point(6, 72);
		this.chk_Wed.Name = "chk_Wed";
		this.chk_Wed.Properties.Caption = "Wednesday";
		this.chk_Wed.SettingInVisible = false;
		this.chk_Wed.SettingReadonly = false;
		this.chk_Wed.Size = new System.Drawing.Size(93, 20);
		this.chk_Wed.TabIndex = 2;
		this.chk_Tue.EditControl = null;
		this.chk_Tue.FieldName = null;
		this.chk_Tue.IsSearchControl = false;
		this.chk_Tue.Location = new System.Drawing.Point(6, 48);
		this.chk_Tue.Name = "chk_Tue";
		this.chk_Tue.Properties.Caption = "Tuesday";
		this.chk_Tue.SettingInVisible = false;
		this.chk_Tue.SettingReadonly = false;
		this.chk_Tue.Size = new System.Drawing.Size(75, 20);
		this.chk_Tue.TabIndex = 1;
		this.chk_Mon.EditControl = null;
		this.chk_Mon.FieldName = null;
		this.chk_Mon.IsSearchControl = false;
		this.chk_Mon.Location = new System.Drawing.Point(6, 24);
		this.chk_Mon.Name = "chk_Mon";
		this.chk_Mon.Properties.Caption = "Monday";
		this.chk_Mon.SettingInVisible = false;
		this.chk_Mon.SettingReadonly = false;
		this.chk_Mon.Size = new System.Drawing.Size(75, 20);
		this.chk_Mon.TabIndex = 0;
		this.tab_ServiceCalls.Controls.Add(this.grd_ServiceCall);
		this.tab_ServiceCalls.EditControl = null;
		this.tab_ServiceCalls.EditValue = "Service Calls";
		this.tab_ServiceCalls.FieldName = null;
		this.tab_ServiceCalls.IsSearchControl = false;
		this.tab_ServiceCalls.Name = "tab_ServiceCalls";
		this.tab_ServiceCalls.ReadOnly = false;
		this.tab_ServiceCalls.SettingInVisible = false;
		this.tab_ServiceCalls.SettingReadonly = false;
		this.tab_ServiceCalls.Size = new System.Drawing.Size(752, 202);
		this.tab_ServiceCalls.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_ServiceCalls.Text = "Service Calls";
		this.grd_ServiceCall.AllowPasteExcel = false;
		this.grd_ServiceCall.ClipboardData = null;
		this.grd_ServiceCall.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ServiceCall.Location = new System.Drawing.Point(0, 0);
		this.grd_ServiceCall.MainView = this.gv_ServiceCall;
		this.grd_ServiceCall.Name = "grd_ServiceCall";
		this.grd_ServiceCall.Size = new System.Drawing.Size(752, 202);
		this.grd_ServiceCall.TabIndex = 0;
		this.grd_ServiceCall.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ServiceCall });
		this.gv_ServiceCall.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ServiceCall.ColumnKey = null;
		this.gv_ServiceCall.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.cl_CallID, this.cl_CreationDate, this.cl_Subject, this.cl_SC_ItemNo, this.cl_SN, this.cl_SC_CustomerName, this.cl_SC_Status });
		this.gv_ServiceCall.DisableFormSetting = false;
		this.gv_ServiceCall.DisplayException = false;
		this.gv_ServiceCall.EditColumn = null;
		this.gv_ServiceCall.FieldCurrency = null;
		this.gv_ServiceCall.FieldRate = null;
		this.gv_ServiceCall.FormId = "oSericeContract";
		this.gv_ServiceCall.GridControl = this.grd_ServiceCall;
		this.gv_ServiceCall.IgnoreFilter = false;
		this.gv_ServiceCall.IndexEditRow = -1;
		this.gv_ServiceCall.IndicatorWidth = 30;
		this.gv_ServiceCall.IsApzTable = false;
		this.gv_ServiceCall.IsInsertNewRow = false;
		this.gv_ServiceCall.IsLoadingData = false;
		this.gv_ServiceCall.IsMappingContextAction = false;
		this.gv_ServiceCall.ItemId = "OSCL";
		this.gv_ServiceCall.LstColumnAllowBatch = null;
		this.gv_ServiceCall.LstColumnNotAllowBatch = null;
		this.gv_ServiceCall.LstColumnNotEditable = null;
		this.gv_ServiceCall.LstColumNotSetting = null;
		this.gv_ServiceCall.Name = "gv_ServiceCall";
		this.gv_ServiceCall.OptionsBehavior.Editable = false;
		this.gv_ServiceCall.OptionsView.ShowGroupPanel = false;
		this.gv_ServiceCall.UseBatchMod = false;
		this.gv_ServiceCall.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_ServiceCall_CustomDrawRowIndicator);
		this.cl_CallID.Caption = "Call ID";
		this.cl_CallID.CurrencyDateField = null;
		this.cl_CallID.CurrencyField = null;
		this.cl_CallID.CurrencyFixed = null;
		this.cl_CallID.CurrencyFixedRate = 0.0;
		this.cl_CallID.CurrencyRateField = null;
		this.cl_CallID.FieldName = "callID";
		this.cl_CallID.Name = "cl_CallID";
		this.cl_CallID.PrimaryKey = false;
		this.cl_CallID.UnitField = null;
		this.cl_CallID.Visible = true;
		this.cl_CallID.VisibleIndex = 0;
		this.cl_CreationDate.Caption = "CreationDate";
		this.cl_CreationDate.CurrencyDateField = null;
		this.cl_CreationDate.CurrencyField = null;
		this.cl_CreationDate.CurrencyFixed = null;
		this.cl_CreationDate.CurrencyFixedRate = 0.0;
		this.cl_CreationDate.CurrencyRateField = null;
		this.cl_CreationDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_CreationDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_CreationDate.FieldName = "createDate";
		this.cl_CreationDate.Name = "cl_CreationDate";
		this.cl_CreationDate.PrimaryKey = false;
		this.cl_CreationDate.UnitField = null;
		this.cl_CreationDate.Visible = true;
		this.cl_CreationDate.VisibleIndex = 1;
		this.cl_Subject.Caption = "Subject";
		this.cl_Subject.CurrencyDateField = null;
		this.cl_Subject.CurrencyField = null;
		this.cl_Subject.CurrencyFixed = null;
		this.cl_Subject.CurrencyFixedRate = 0.0;
		this.cl_Subject.CurrencyRateField = null;
		this.cl_Subject.FieldName = "subject";
		this.cl_Subject.Name = "cl_Subject";
		this.cl_Subject.PrimaryKey = false;
		this.cl_Subject.UnitField = null;
		this.cl_Subject.Visible = true;
		this.cl_Subject.VisibleIndex = 2;
		this.cl_SC_ItemNo.Caption = "Item No.";
		this.cl_SC_ItemNo.CurrencyDateField = null;
		this.cl_SC_ItemNo.CurrencyField = null;
		this.cl_SC_ItemNo.CurrencyFixed = null;
		this.cl_SC_ItemNo.CurrencyFixedRate = 0.0;
		this.cl_SC_ItemNo.CurrencyRateField = null;
		this.cl_SC_ItemNo.FieldName = "itemCode";
		this.cl_SC_ItemNo.Name = "cl_SC_ItemNo";
		this.cl_SC_ItemNo.PrimaryKey = false;
		this.cl_SC_ItemNo.UnitField = null;
		this.cl_SC_ItemNo.Visible = true;
		this.cl_SC_ItemNo.VisibleIndex = 3;
		this.cl_SN.Caption = "SN";
		this.cl_SN.CurrencyDateField = null;
		this.cl_SN.CurrencyField = null;
		this.cl_SN.CurrencyFixed = null;
		this.cl_SN.CurrencyFixedRate = 0.0;
		this.cl_SN.CurrencyRateField = null;
		this.cl_SN.FieldName = "manufSN";
		this.cl_SN.Name = "cl_SN";
		this.cl_SN.PrimaryKey = false;
		this.cl_SN.UnitField = null;
		this.cl_SN.Visible = true;
		this.cl_SN.VisibleIndex = 4;
		this.cl_SC_CustomerName.Caption = "Customer Name";
		this.cl_SC_CustomerName.CurrencyDateField = null;
		this.cl_SC_CustomerName.CurrencyField = null;
		this.cl_SC_CustomerName.CurrencyFixed = null;
		this.cl_SC_CustomerName.CurrencyFixedRate = 0.0;
		this.cl_SC_CustomerName.CurrencyRateField = null;
		this.cl_SC_CustomerName.FieldName = "custmrName";
		this.cl_SC_CustomerName.Name = "cl_SC_CustomerName";
		this.cl_SC_CustomerName.PrimaryKey = false;
		this.cl_SC_CustomerName.UnitField = null;
		this.cl_SC_CustomerName.Visible = true;
		this.cl_SC_CustomerName.VisibleIndex = 5;
		this.cl_SC_Status.Caption = "Status";
		this.cl_SC_Status.CurrencyDateField = null;
		this.cl_SC_Status.CurrencyField = null;
		this.cl_SC_Status.CurrencyFixed = null;
		this.cl_SC_Status.CurrencyFixedRate = 0.0;
		this.cl_SC_Status.CurrencyRateField = null;
		this.cl_SC_Status.FieldName = "status";
		this.cl_SC_Status.Name = "cl_SC_Status";
		this.cl_SC_Status.PrimaryKey = false;
		this.cl_SC_Status.UnitField = null;
		this.cl_SC_Status.Visible = true;
		this.cl_SC_Status.VisibleIndex = 6;
		this.tab_SalesData.Controls.Add(this.panelControl6);
		this.tab_SalesData.EditControl = null;
		this.tab_SalesData.EditValue = "Sales Data";
		this.tab_SalesData.FieldName = null;
		this.tab_SalesData.IsSearchControl = false;
		this.tab_SalesData.Name = "tab_SalesData";
		this.tab_SalesData.ReadOnly = false;
		this.tab_SalesData.SettingInVisible = false;
		this.tab_SalesData.SettingReadonly = false;
		this.tab_SalesData.Size = new System.Drawing.Size(752, 202);
		this.tab_SalesData.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_SalesData.Text = "Sales Data";
		this.panelControl6.Controls.Add(this.splitContainerControl1);
		this.panelControl6.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl6.Location = new System.Drawing.Point(0, 0);
		this.panelControl6.Name = "panelControl6";
		this.panelControl6.Size = new System.Drawing.Size(752, 202);
		this.panelControl6.TabIndex = 0;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_RecTemp);
		this.splitContainerControl1.Panel1.Controls.Add(this.lbl_RecurringTemp);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_RecTrans);
		this.splitContainerControl1.Panel2.Controls.Add(this.lbl_RecurringTransactions);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(748, 198);
		this.splitContainerControl1.SplitterPosition = 239;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_RecTemp.AllowPasteExcel = false;
		this.grd_RecTemp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_RecTemp.ClipboardData = null;
		this.grd_RecTemp.Location = new System.Drawing.Point(5, 25);
		this.grd_RecTemp.MainView = this.gv_RecTemp;
		this.grd_RecTemp.Name = "grd_RecTemp";
		this.grd_RecTemp.Size = new System.Drawing.Size(233, 172);
		this.grd_RecTemp.TabIndex = 1;
		this.grd_RecTemp.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_RecTemp });
		this.gv_RecTemp.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_RecTemp.ColumnKey = null;
		this.gv_RecTemp.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.cl_Template, this.cl_Type });
		this.gv_RecTemp.DisableFormSetting = false;
		this.gv_RecTemp.DisplayException = false;
		this.gv_RecTemp.EditColumn = null;
		this.gv_RecTemp.FieldCurrency = null;
		this.gv_RecTemp.FieldRate = null;
		this.gv_RecTemp.FormId = "oSericeContract";
		this.gv_RecTemp.GridControl = this.grd_RecTemp;
		this.gv_RecTemp.IgnoreFilter = false;
		this.gv_RecTemp.IndexEditRow = -1;
		this.gv_RecTemp.IndicatorWidth = 35;
		this.gv_RecTemp.IsApzTable = false;
		this.gv_RecTemp.IsInsertNewRow = false;
		this.gv_RecTemp.IsLoadingData = false;
		this.gv_RecTemp.IsMappingContextAction = false;
		this.gv_RecTemp.ItemId = "ORECTEMP";
		this.gv_RecTemp.LstColumnAllowBatch = null;
		this.gv_RecTemp.LstColumnNotAllowBatch = null;
		this.gv_RecTemp.LstColumnNotEditable = null;
		this.gv_RecTemp.LstColumNotSetting = null;
		this.gv_RecTemp.Name = "gv_RecTemp";
		this.gv_RecTemp.OptionsSelection.CheckBoxSelectorColumnWidth = 30;
		this.gv_RecTemp.OptionsSelection.MultiSelect = true;
		this.gv_RecTemp.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_RecTemp.OptionsView.ShowGroupPanel = false;
		this.gv_RecTemp.UseBatchMod = false;
		this.cl_Template.Caption = "Template";
		this.cl_Template.CurrencyDateField = null;
		this.cl_Template.CurrencyField = null;
		this.cl_Template.CurrencyFixed = null;
		this.cl_Template.CurrencyFixedRate = 0.0;
		this.cl_Template.CurrencyRateField = null;
		this.cl_Template.Name = "cl_Template";
		this.cl_Template.PrimaryKey = false;
		this.cl_Template.UnitField = null;
		this.cl_Template.Visible = true;
		this.cl_Template.VisibleIndex = 2;
		this.cl_Type.Caption = "Type";
		this.cl_Type.CurrencyDateField = null;
		this.cl_Type.CurrencyField = null;
		this.cl_Type.CurrencyFixed = null;
		this.cl_Type.CurrencyFixedRate = 0.0;
		this.cl_Type.CurrencyRateField = null;
		this.cl_Type.Name = "cl_Type";
		this.cl_Type.OptionsColumn.AllowEdit = false;
		this.cl_Type.OptionsColumn.ReadOnly = true;
		this.cl_Type.PrimaryKey = false;
		this.cl_Type.UnitField = null;
		this.cl_Type.Visible = true;
		this.cl_Type.VisibleIndex = 1;
		this.lbl_RecurringTemp.EditControl = null;
		this.lbl_RecurringTemp.EditValue = "Recurring Templates";
		this.lbl_RecurringTemp.FieldName = null;
		this.lbl_RecurringTemp.HideWhenEmpty = false;
		this.lbl_RecurringTemp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_RecurringTemp.Location = new System.Drawing.Point(4, 4);
		this.lbl_RecurringTemp.Name = "lbl_RecurringTemp";
		this.lbl_RecurringTemp.ReadOnly = false;
		this.lbl_RecurringTemp.SettingInVisible = false;
		this.lbl_RecurringTemp.SettingReadonly = false;
		this.lbl_RecurringTemp.ShowUnderLine = false;
		this.lbl_RecurringTemp.Size = new System.Drawing.Size(109, 20);
		this.lbl_RecurringTemp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_RecurringTemp.TabIndex = 0;
		this.lbl_RecurringTemp.Utext = "Recurring Templates";
		this.grd_RecTrans.AllowPasteExcel = false;
		this.grd_RecTrans.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_RecTrans.ClipboardData = null;
		this.grd_RecTrans.Location = new System.Drawing.Point(0, 25);
		this.grd_RecTrans.MainView = this.gv_RecTrans;
		this.grd_RecTrans.Name = "grd_RecTrans";
		this.grd_RecTrans.Size = new System.Drawing.Size(504, 172);
		this.grd_RecTrans.TabIndex = 2;
		this.grd_RecTrans.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_RecTrans });
		this.gv_RecTrans.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_RecTrans.ColumnKey = null;
		this.gv_RecTrans.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.cl_RecTran_Template, this.cl_Instance, this.cl_RecTran_Instance, this.cl_RecTran_Date, this.cl_DocTotal });
		this.gv_RecTrans.DisableFormSetting = false;
		this.gv_RecTrans.DisplayException = false;
		this.gv_RecTrans.EditColumn = null;
		this.gv_RecTrans.FieldCurrency = null;
		this.gv_RecTrans.FieldRate = null;
		this.gv_RecTrans.FormId = "oSericeContract";
		this.gv_RecTrans.GridControl = this.grd_RecTrans;
		this.gv_RecTrans.IgnoreFilter = false;
		this.gv_RecTrans.IndexEditRow = -1;
		this.gv_RecTrans.IndicatorWidth = 35;
		this.gv_RecTrans.IsApzTable = false;
		this.gv_RecTrans.IsInsertNewRow = false;
		this.gv_RecTrans.IsLoadingData = false;
		this.gv_RecTrans.IsMappingContextAction = false;
		this.gv_RecTrans.ItemId = "ORECTRAN";
		this.gv_RecTrans.LstColumnAllowBatch = null;
		this.gv_RecTrans.LstColumnNotAllowBatch = null;
		this.gv_RecTrans.LstColumnNotEditable = null;
		this.gv_RecTrans.LstColumNotSetting = null;
		this.gv_RecTrans.Name = "gv_RecTrans";
		this.gv_RecTrans.OptionsBehavior.Editable = false;
		this.gv_RecTrans.OptionsView.ShowGroupPanel = false;
		this.gv_RecTrans.UseBatchMod = false;
		this.cl_RecTran_Template.Caption = "Template";
		this.cl_RecTran_Template.CurrencyDateField = null;
		this.cl_RecTran_Template.CurrencyField = null;
		this.cl_RecTran_Template.CurrencyFixed = null;
		this.cl_RecTran_Template.CurrencyFixedRate = 0.0;
		this.cl_RecTran_Template.CurrencyRateField = null;
		this.cl_RecTran_Template.Name = "cl_RecTran_Template";
		this.cl_RecTran_Template.PrimaryKey = false;
		this.cl_RecTran_Template.UnitField = null;
		this.cl_RecTran_Template.Visible = true;
		this.cl_RecTran_Template.VisibleIndex = 0;
		this.cl_Instance.Caption = "Instance";
		this.cl_Instance.CurrencyDateField = null;
		this.cl_Instance.CurrencyField = null;
		this.cl_Instance.CurrencyFixed = null;
		this.cl_Instance.CurrencyFixedRate = 0.0;
		this.cl_Instance.CurrencyRateField = null;
		this.cl_Instance.Name = "cl_Instance";
		this.cl_Instance.PrimaryKey = false;
		this.cl_Instance.UnitField = null;
		this.cl_Instance.Visible = true;
		this.cl_Instance.VisibleIndex = 1;
		this.cl_RecTran_Instance.Caption = "Status";
		this.cl_RecTran_Instance.CurrencyDateField = null;
		this.cl_RecTran_Instance.CurrencyField = null;
		this.cl_RecTran_Instance.CurrencyFixed = null;
		this.cl_RecTran_Instance.CurrencyFixedRate = 0.0;
		this.cl_RecTran_Instance.CurrencyRateField = null;
		this.cl_RecTran_Instance.Name = "cl_RecTran_Instance";
		this.cl_RecTran_Instance.PrimaryKey = false;
		this.cl_RecTran_Instance.UnitField = null;
		this.cl_RecTran_Instance.Visible = true;
		this.cl_RecTran_Instance.VisibleIndex = 2;
		this.cl_RecTran_Date.Caption = "Date";
		this.cl_RecTran_Date.CurrencyDateField = null;
		this.cl_RecTran_Date.CurrencyField = null;
		this.cl_RecTran_Date.CurrencyFixed = null;
		this.cl_RecTran_Date.CurrencyFixedRate = 0.0;
		this.cl_RecTran_Date.CurrencyRateField = null;
		this.cl_RecTran_Date.Name = "cl_RecTran_Date";
		this.cl_RecTran_Date.PrimaryKey = false;
		this.cl_RecTran_Date.UnitField = null;
		this.cl_RecTran_Date.Visible = true;
		this.cl_RecTran_Date.VisibleIndex = 3;
		this.cl_DocTotal.Caption = "Doc. Total";
		this.cl_DocTotal.CurrencyDateField = null;
		this.cl_DocTotal.CurrencyField = null;
		this.cl_DocTotal.CurrencyFixed = null;
		this.cl_DocTotal.CurrencyFixedRate = 0.0;
		this.cl_DocTotal.CurrencyRateField = null;
		this.cl_DocTotal.Name = "cl_DocTotal";
		this.cl_DocTotal.PrimaryKey = false;
		this.cl_DocTotal.UnitField = null;
		this.cl_DocTotal.Visible = true;
		this.cl_DocTotal.VisibleIndex = 4;
		this.lbl_RecurringTransactions.EditControl = null;
		this.lbl_RecurringTransactions.EditValue = "Recurring Transactions";
		this.lbl_RecurringTransactions.FieldName = null;
		this.lbl_RecurringTransactions.HideWhenEmpty = false;
		this.lbl_RecurringTransactions.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_RecurringTransactions.Location = new System.Drawing.Point(14, 4);
		this.lbl_RecurringTransactions.Name = "lbl_RecurringTransactions";
		this.lbl_RecurringTransactions.ReadOnly = false;
		this.lbl_RecurringTransactions.SettingInVisible = false;
		this.lbl_RecurringTransactions.SettingReadonly = false;
		this.lbl_RecurringTransactions.ShowUnderLine = false;
		this.lbl_RecurringTransactions.Size = new System.Drawing.Size(118, 20);
		this.lbl_RecurringTransactions.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_RecurringTransactions.TabIndex = 1;
		this.lbl_RecurringTransactions.Utext = "Recurring Transactions";
		this.tab_Attachments.Controls.Add(this.panelControl5);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = null;
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = false;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		this.tab_Attachments.Size = new System.Drawing.Size(752, 202);
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachments.Text = "Attachments";
		this.panelControl5.Controls.Add(this.ucAttachment1);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(0, 0);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(752, 202);
		this.panelControl5.TabIndex = 0;
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oSericeContract";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(2, 2);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(748, 198);
		this.ucAttachment1.TabIndex = 0;
		this.ucAttachment1.ButtonBrowse_Click += new System.EventHandler(this.ucAttachment1_ButtonBrowse_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 476);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 56;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btRefresh.Appearance.Options.UseBackColor = true;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(172, 476);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(99, 20);
		this.btRefresh.TabIndex = 55;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Visible = false;
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(73, 476);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 54;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbl_StartDate.EditControl = null;
		this.lbl_StartDate.EditValue = "Start Date";
		this.lbl_StartDate.FieldName = null;
		this.lbl_StartDate.HideWhenEmpty = false;
		this.lbl_StartDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_StartDate.Location = new System.Drawing.Point(6, 9);
		this.lbl_StartDate.Name = "lbl_StartDate";
		this.lbl_StartDate.ReadOnly = false;
		this.lbl_StartDate.SettingInVisible = false;
		this.lbl_StartDate.SettingReadonly = false;
		this.lbl_StartDate.ShowUnderLine = true;
		this.lbl_StartDate.Size = new System.Drawing.Size(80, 20);
		this.lbl_StartDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_StartDate.TabIndex = 57;
		this.lbl_StartDate.Utext = "Start Date";
		this.lbl_EndDate.EditControl = null;
		this.lbl_EndDate.EditValue = "End Date";
		this.lbl_EndDate.FieldName = null;
		this.lbl_EndDate.HideWhenEmpty = false;
		this.lbl_EndDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_EndDate.Location = new System.Drawing.Point(223, 9);
		this.lbl_EndDate.Name = "lbl_EndDate";
		this.lbl_EndDate.ReadOnly = false;
		this.lbl_EndDate.SettingInVisible = false;
		this.lbl_EndDate.SettingReadonly = false;
		this.lbl_EndDate.ShowUnderLine = true;
		this.lbl_EndDate.Size = new System.Drawing.Size(80, 20);
		this.lbl_EndDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EndDate.TabIndex = 58;
		this.lbl_EndDate.Utext = "End Date";
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(432, 6);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 61;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.lblServiceType.EditControl = null;
		this.lblServiceType.EditValue = "";
		this.lblServiceType.FieldName = null;
		this.lblServiceType.HideWhenEmpty = false;
		this.lblServiceType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblServiceType.Location = new System.Drawing.Point(10, 6);
		this.lblServiceType.Name = "lblServiceType";
		this.lblServiceType.ReadOnly = false;
		this.lblServiceType.SettingInVisible = false;
		this.lblServiceType.SettingReadonly = false;
		this.lblServiceType.ShowUnderLine = true;
		this.lblServiceType.Size = new System.Drawing.Size(92, 20);
		this.lblServiceType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblServiceType.TabIndex = 0;
		this.lblServiceType.Utext = "";
		this.lbl_ContractType.EditControl = null;
		this.lbl_ContractType.EditValue = "";
		this.lbl_ContractType.FieldName = null;
		this.lbl_ContractType.HideWhenEmpty = false;
		this.lbl_ContractType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ContractType.Location = new System.Drawing.Point(9, 29);
		this.lbl_ContractType.Name = "lbl_ContractType";
		this.lbl_ContractType.ReadOnly = false;
		this.lbl_ContractType.SettingInVisible = false;
		this.lbl_ContractType.SettingReadonly = false;
		this.lbl_ContractType.ShowUnderLine = true;
		this.lbl_ContractType.Size = new System.Drawing.Size(92, 20);
		this.lbl_ContractType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ContractType.TabIndex = 1;
		this.lbl_ContractType.Utext = "";
		this.lbl_Template.EditControl = null;
		this.lbl_Template.EditValue = "";
		this.lbl_Template.FieldName = null;
		this.lbl_Template.HideWhenEmpty = false;
		this.lbl_Template.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Template.Location = new System.Drawing.Point(10, 52);
		this.lbl_Template.Name = "lbl_Template";
		this.lbl_Template.ReadOnly = false;
		this.lbl_Template.SettingInVisible = false;
		this.lbl_Template.SettingReadonly = false;
		this.lbl_Template.ShowUnderLine = true;
		this.lbl_Template.Size = new System.Drawing.Size(92, 20);
		this.lbl_Template.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Template.TabIndex = 2;
		this.lbl_Template.Utext = "";
		this.lbl_Resoulution.EditControl = null;
		this.lbl_Resoulution.EditValue = "";
		this.lbl_Resoulution.FieldName = null;
		this.lbl_Resoulution.HideWhenEmpty = false;
		this.lbl_Resoulution.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Resoulution.Location = new System.Drawing.Point(9, 100);
		this.lbl_Resoulution.Name = "lbl_Resoulution";
		this.lbl_Resoulution.ReadOnly = false;
		this.lbl_Resoulution.SettingInVisible = false;
		this.lbl_Resoulution.SettingReadonly = false;
		this.lbl_Resoulution.ShowUnderLine = true;
		this.lbl_Resoulution.Size = new System.Drawing.Size(92, 20);
		this.lbl_Resoulution.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Resoulution.TabIndex = 3;
		this.lbl_Resoulution.Utext = "";
		this.lbl_ResponseTime.EditControl = null;
		this.lbl_ResponseTime.EditValue = "";
		this.lbl_ResponseTime.FieldName = null;
		this.lbl_ResponseTime.HideWhenEmpty = false;
		this.lbl_ResponseTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ResponseTime.Location = new System.Drawing.Point(9, 76);
		this.lbl_ResponseTime.Name = "lbl_ResponseTime";
		this.lbl_ResponseTime.ReadOnly = false;
		this.lbl_ResponseTime.SettingInVisible = false;
		this.lbl_ResponseTime.SettingReadonly = false;
		this.lbl_ResponseTime.ShowUnderLine = true;
		this.lbl_ResponseTime.Size = new System.Drawing.Size(92, 20);
		this.lbl_ResponseTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ResponseTime.TabIndex = 4;
		this.lbl_ResponseTime.Utext = "";
		this.Remarks.EditControl = null;
		this.Remarks.EditValue = "";
		this.Remarks.FieldName = null;
		this.Remarks.HideWhenEmpty = false;
		this.Remarks.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.Remarks.Location = new System.Drawing.Point(9, 124);
		this.Remarks.Name = "Remarks";
		this.Remarks.ReadOnly = false;
		this.Remarks.SettingInVisible = false;
		this.Remarks.SettingReadonly = false;
		this.Remarks.ShowUnderLine = true;
		this.Remarks.Size = new System.Drawing.Size(92, 20);
		this.Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Remarks.TabIndex = 5;
		this.Remarks.Utext = "";
		this.cbb_SrvcType.EditControl = null;
		this.cbb_SrvcType.FieldName = null;
		this.cbb_SrvcType.IsSearchControl = false;
		this.cbb_SrvcType.Location = new System.Drawing.Point(102, 5);
		this.cbb_SrvcType.Name = "cbb_SrvcType";
		this.cbb_SrvcType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_SrvcType.SettingInVisible = false;
		this.cbb_SrvcType.SettingReadonly = false;
		this.cbb_SrvcType.Size = new System.Drawing.Size(206, 20);
		this.cbb_SrvcType.TabIndex = 6;
		this.cbb_cntrc.EditControl = null;
		this.cbb_cntrc.FieldName = null;
		this.cbb_cntrc.IsSearchControl = false;
		this.cbb_cntrc.Location = new System.Drawing.Point(102, 29);
		this.cbb_cntrc.Name = "cbb_cntrc";
		this.cbb_cntrc.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_cntrc.SettingInVisible = false;
		this.cbb_cntrc.SettingReadonly = false;
		this.cbb_cntrc.Size = new System.Drawing.Size(206, 20);
		this.cbb_cntrc.TabIndex = 7;
		this.cbb_cntrc.CloseUp += new DevExpress.XtraEditors.Controls.CloseUpEventHandler(this.cbb_cntrc_CloseUp);
		this.lbl_Status.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Status.EditControl = null;
		this.lbl_Status.EditValue = "";
		this.lbl_Status.FieldName = null;
		this.lbl_Status.HideWhenEmpty = false;
		this.lbl_Status.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Status.Location = new System.Drawing.Point(444, 6);
		this.lbl_Status.Name = "lbl_Status";
		this.lbl_Status.ReadOnly = false;
		this.lbl_Status.SettingInVisible = false;
		this.lbl_Status.SettingReadonly = false;
		this.lbl_Status.ShowUnderLine = true;
		this.lbl_Status.Size = new System.Drawing.Size(92, 20);
		this.lbl_Status.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Status.TabIndex = 8;
		this.lbl_Status.Utext = "";
		this.cbb_Status.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbb_Status.EditControl = null;
		this.cbb_Status.FieldName = null;
		this.cbb_Status.IsSearchControl = false;
		this.cbb_Status.Location = new System.Drawing.Point(536, 5);
		this.cbb_Status.Name = "cbb_Status";
		this.cbb_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_Status.SettingInVisible = false;
		this.cbb_Status.SettingReadonly = false;
		this.cbb_Status.Size = new System.Drawing.Size(206, 20);
		this.cbb_Status.TabIndex = 9;
		this.lbl_Owner.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Owner.EditControl = null;
		this.lbl_Owner.EditValue = "";
		this.lbl_Owner.FieldName = null;
		this.lbl_Owner.HideWhenEmpty = false;
		this.lbl_Owner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Owner.Location = new System.Drawing.Point(444, 29);
		this.lbl_Owner.Name = "lbl_Owner";
		this.lbl_Owner.ReadOnly = false;
		this.lbl_Owner.SettingInVisible = false;
		this.lbl_Owner.SettingReadonly = false;
		this.lbl_Owner.ShowUnderLine = true;
		this.lbl_Owner.Size = new System.Drawing.Size(92, 20);
		this.lbl_Owner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Owner.TabIndex = 10;
		this.lbl_Owner.Utext = "";
		this.cbb_Owner.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbb_Owner.EditControl = null;
		this.cbb_Owner.FieldName = null;
		this.cbb_Owner.IsSearchControl = false;
		this.cbb_Owner.Location = new System.Drawing.Point(537, 29);
		this.cbb_Owner.Name = "cbb_Owner";
		this.cbb_Owner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_Owner.SettingInVisible = false;
		this.cbb_Owner.SettingReadonly = false;
		this.cbb_Owner.Size = new System.Drawing.Size(206, 20);
		this.cbb_Owner.TabIndex = 11;
		this.txt_Template.EditControl = null;
		this.txt_Template.FieldName = null;
		this.txt_Template.IsSearchControl = false;
		this.txt_Template.Location = new System.Drawing.Point(102, 53);
		this.txt_Template.Name = "txt_Template";
		this.txt_Template.SettingInVisible = false;
		this.txt_Template.SettingReadonly = false;
		this.txt_Template.Size = new System.Drawing.Size(206, 20);
		this.txt_Template.TabIndex = 12;
		this.chk_Renewal.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Renewal.EditControl = null;
		this.chk_Renewal.FieldName = null;
		this.chk_Renewal.IsSearchControl = false;
		this.chk_Renewal.Location = new System.Drawing.Point(445, 53);
		this.chk_Renewal.Name = "chk_Renewal";
		this.chk_Renewal.Properties.Caption = "Renewal";
		this.chk_Renewal.SettingInVisible = false;
		this.chk_Renewal.SettingReadonly = false;
		this.chk_Renewal.Size = new System.Drawing.Size(75, 20);
		this.chk_Renewal.TabIndex = 13;
		this.lbl_Reminder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Reminder.EditControl = null;
		this.lbl_Reminder.EditValue = "";
		this.lbl_Reminder.FieldName = null;
		this.lbl_Reminder.HideWhenEmpty = false;
		this.lbl_Reminder.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Reminder.Location = new System.Drawing.Point(444, 76);
		this.lbl_Reminder.Name = "lbl_Reminder";
		this.lbl_Reminder.ReadOnly = false;
		this.lbl_Reminder.SettingInVisible = false;
		this.lbl_Reminder.SettingReadonly = false;
		this.lbl_Reminder.ShowUnderLine = true;
		this.lbl_Reminder.Size = new System.Drawing.Size(92, 20);
		this.lbl_Reminder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Reminder.TabIndex = 14;
		this.lbl_Reminder.Utext = "";
		this.lbl_ActiveItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ActiveItems.EditControl = null;
		this.lbl_ActiveItems.EditValue = "";
		this.lbl_ActiveItems.FieldName = null;
		this.lbl_ActiveItems.HideWhenEmpty = false;
		this.lbl_ActiveItems.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ActiveItems.Location = new System.Drawing.Point(444, 100);
		this.lbl_ActiveItems.Name = "lbl_ActiveItems";
		this.lbl_ActiveItems.ReadOnly = false;
		this.lbl_ActiveItems.SettingInVisible = false;
		this.lbl_ActiveItems.SettingReadonly = false;
		this.lbl_ActiveItems.ShowUnderLine = true;
		this.lbl_ActiveItems.Size = new System.Drawing.Size(92, 20);
		this.lbl_ActiveItems.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ActiveItems.TabIndex = 15;
		this.lbl_ActiveItems.Utext = "";
		this.labelLine10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(444, 124);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(92, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 16;
		this.labelLine10.Utext = "";
		this.txt_Remark.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txt_Remark.EditControl = null;
		this.txt_Remark.FieldName = null;
		this.txt_Remark.IsSearchControl = false;
		this.txt_Remark.Location = new System.Drawing.Point(102, 126);
		this.txt_Remark.Name = "txt_Remark";
		this.txt_Remark.SettingInVisible = false;
		this.txt_Remark.SettingReadonly = false;
		this.txt_Remark.Size = new System.Drawing.Size(206, 73);
		this.txt_Remark.TabIndex = 19;
		this.txt_ContractTemp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txt_ContractTemp.EditControl = null;
		this.txt_ContractTemp.Enabled = false;
		this.txt_ContractTemp.FieldName = null;
		this.txt_ContractTemp.IsSearchControl = false;
		this.txt_ContractTemp.Location = new System.Drawing.Point(537, 126);
		this.txt_ContractTemp.Name = "txt_ContractTemp";
		this.txt_ContractTemp.Properties.ReadOnly = true;
		this.txt_ContractTemp.SettingInVisible = false;
		this.txt_ContractTemp.SettingReadonly = false;
		this.txt_ContractTemp.Size = new System.Drawing.Size(206, 73);
		this.txt_ContractTemp.TabIndex = 20;
		this.cbb_ResP.EditControl = null;
		this.cbb_ResP.FieldName = null;
		this.cbb_ResP.IsSearchControl = false;
		this.cbb_ResP.Location = new System.Drawing.Point(158, 77);
		this.cbb_ResP.Name = "cbb_ResP";
		this.cbb_ResP.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_ResP.SettingInVisible = false;
		this.cbb_ResP.SettingReadonly = false;
		this.cbb_ResP.Size = new System.Drawing.Size(150, 20);
		this.cbb_ResP.TabIndex = 21;
		this.cbb_TimeResO.EditControl = null;
		this.cbb_TimeResO.FieldName = null;
		this.cbb_TimeResO.IsSearchControl = false;
		this.cbb_TimeResO.Location = new System.Drawing.Point(158, 101);
		this.cbb_TimeResO.Name = "cbb_TimeResO";
		this.cbb_TimeResO.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_TimeResO.SettingInVisible = false;
		this.cbb_TimeResO.SettingReadonly = false;
		this.cbb_TimeResO.Size = new System.Drawing.Size(150, 20);
		this.cbb_TimeResO.TabIndex = 22;
		this.cbb_Remind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.cbb_Remind.EditControl = null;
		this.cbb_Remind.Enabled = false;
		this.cbb_Remind.FieldName = null;
		this.cbb_Remind.IsSearchControl = false;
		this.cbb_Remind.Location = new System.Drawing.Point(593, 77);
		this.cbb_Remind.Name = "cbb_Remind";
		this.cbb_Remind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbb_Remind.SettingInVisible = false;
		this.cbb_Remind.SettingReadonly = false;
		this.cbb_Remind.Size = new System.Drawing.Size(150, 20);
		this.cbb_Remind.TabIndex = 24;
		this.txt_ActiveItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_ActiveItems.EditControl = null;
		this.txt_ActiveItems.FieldName = null;
		this.txt_ActiveItems.IsSearchControl = false;
		this.txt_ActiveItems.Location = new System.Drawing.Point(537, 101);
		this.txt_ActiveItems.Name = "txt_ActiveItems";
		this.txt_ActiveItems.Properties.ReadOnly = true;
		this.txt_ActiveItems.SettingInVisible = false;
		this.txt_ActiveItems.SettingReadonly = false;
		this.txt_ActiveItems.Size = new System.Drawing.Size(206, 20);
		this.txt_ActiveItems.TabIndex = 25;
		this.spin_Resp.EditValue = new decimal(new int[4]);
		this.spin_Resp.Location = new System.Drawing.Point(101, 76);
		this.spin_Resp.Name = "spin_Resp";
		this.spin_Resp.Size = new System.Drawing.Size(51, 20);
		this.spin_Resp.TabIndex = 26;
		this.spin_ResO.EditValue = new decimal(new int[4]);
		this.spin_ResO.Location = new System.Drawing.Point(101, 101);
		this.spin_ResO.Name = "spin_ResO";
		this.spin_ResO.Size = new System.Drawing.Size(51, 20);
		this.spin_ResO.TabIndex = 27;
		this.spin_Remind.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.spin_Remind.EditValue = new decimal(new int[4]);
		this.spin_Remind.Enabled = false;
		this.spin_Remind.Location = new System.Drawing.Point(537, 77);
		this.spin_Remind.Name = "spin_Remind";
		this.spin_Remind.Size = new System.Drawing.Size(51, 20);
		this.spin_Remind.TabIndex = 28;
		this.date_Start.Cursor = System.Windows.Forms.Cursors.Default;
		this.date_Start.EditControl = null;
		this.date_Start.EditValue = null;
		this.date_Start.FieldName = null;
		this.date_Start.Location = new System.Drawing.Point(82, 6);
		this.date_Start.Name = "date_Start";
		this.date_Start.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Start.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Start.Properties.IsSearchControl = false;
		this.date_Start.SettingInVisible = false;
		this.date_Start.SettingReadonly = false;
		this.date_Start.Size = new System.Drawing.Size(124, 20);
		this.date_Start.TabIndex = 59;
		this.date_EndDate.EditControl = null;
		this.date_EndDate.EditValue = null;
		this.date_EndDate.FieldName = null;
		this.date_EndDate.Location = new System.Drawing.Point(298, 6);
		this.date_EndDate.Name = "date_EndDate";
		this.date_EndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_EndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_EndDate.Properties.IsSearchControl = false;
		this.date_EndDate.SettingInVisible = false;
		this.date_EndDate.SettingReadonly = false;
		this.date_EndDate.Size = new System.Drawing.Size(124, 20);
		this.date_EndDate.TabIndex = 60;
		this.date_EndDate.EditValueChanged += new System.EventHandler(this.date_EndDate_EditValueChanged);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(756, 503);
		base.Controls.Add(this.date_EndDate);
		base.Controls.Add(this.date_Start);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.lbl_EndDate);
		base.Controls.Add(this.lbl_StartDate);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btRefresh);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.spl_Content);
		base.FormId = "oSericeContract";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Contract";
		base.ShowInTaskbar = false;
		this.Text = "Service Contract";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Contract_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.spl_Content).EndInit();
		this.spl_Content.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rp_Customer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ContactPersonCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SrvcType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CntrcType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Template).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ReponseU).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remarks2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Owner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Renewal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remarks1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RemU).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Content).EndInit();
		this.tab_Content.ResumeLayout(false);
		this.tab_Items.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Items).EndInit();
		this.ctm_Item.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlp_ItemGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SerialNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineDate).EndInit();
		this.tab_Coverage.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pnl_Coverage).EndInit();
		this.pnl_Coverage.ResumeLayout(false);
		this.pnl_Coverage.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.date_WenS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_WenS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TueS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Including.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_Travel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Labor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Parts.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SunS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_WenE.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_WenE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ThuS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FriS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_SatS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonS.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_MonS.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Sun.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Sat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Fri.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Thu.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Wed.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Tue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Mon.Properties).EndInit();
		this.tab_ServiceCalls.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ServiceCall).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ServiceCall).EndInit();
		this.tab_SalesData.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl6).EndInit();
		this.panelControl6.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_RecTemp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_RecTemp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_RecTrans).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_RecTrans).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.cbb_SrvcType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_cntrc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Owner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Template.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Renewal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ContractTemp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_ResP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_TimeResO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbb_Remind.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ActiveItems.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Resp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_ResO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_Remind.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Start.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Start.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
