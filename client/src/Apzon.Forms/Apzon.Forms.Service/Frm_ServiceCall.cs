using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
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

public class Frm_ServiceCall : XtraFormEx
{
	public int clgCode;

	public DateTime ActivityCntctDate;

	private string _okString = Apzon.Commons.Properties.Resources.ButtonOK;

	private string _saveString = Apzon.Commons.Properties.Resources.ButtonSave;

	private string _updateString = Apzon.Commons.Properties.Resources.ButtonUpdate;

	private Enums.FormAuthorities authoritioAuthorities;

	private bool _OpenChooseCFL = true;

	private bool _isAddNew;

	private bool _isCancel;

	private bool _forceClose;

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private DataTable _dtContact = new DataTable();

	private readonly List<CntPRepControls> _lstContactControls = new List<CntPRepControls>();

	private IContainer components;

	private LabelLine lbFromDate;

	private LabelLine lbPriority;

	private LabelLine lbToDate;

	private LabelLine lbCustomer;

	private LabelLine lbCallStatus;

	private ButtonEditEx txtCustomer;

	private SimpleButtonEx btnSearch;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnCancel;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private XtraTabControlEx tabLine;

	private XtraTabPageEx tabActivities;

	private GridControlEx grdActivities;

	private GridViewEx gvActivities;

	private XtraTabPageEx tabSolutions;

	private GridControlEx grdSolutions;

	private GridViewEx gvSolutions;

	private XtraTabPageEx tabExpenses;

	private XtraTabPageEx tabHistory;

	private XtraTabPageEx tabScheduling;

	private XtraTabPageEx tabAttachments;

	private GridColumnEx gcCallID;

	private GridColumnEx gcCustomer;

	private GridColumnEx gvContactPerson;

	private GridColumnEx gcName;

	private GridColumnEx gcTelephoneNo;

	private GridColumnEx gcCustomerRefNo;

	private GridColumnEx gcMfrSerialNo;

	private GridColumnEx gcSerialNumber;

	private GridColumnEx gcItem;

	private GridColumnEx gcDescription;

	private GridColumnEx gcItemGroup;

	private GridColumnEx gcNo;

	private GridColumnEx gcDocNum;

	private GridColumnEx gcCallStatus;

	private GridColumnEx gcPriority;

	private GridColumnEx gcCreatedDate;

	private GridColumnEx gcCreateTime;

	private GridColumnEx gcClosedDate;

	private GridColumnEx gcClosedTime;

	private GridColumnEx gcContractNo;

	private GridColumnEx gcEndDate;

	private GridColumnEx gcSubject;

	private GridColumnEx gcDate;

	private GridColumnEx gcTime;

	private GridColumnEx gcHandledbyInActivities;

	private GridColumnEx gcActivityDate;

	private GridColumnEx gcRecurrence;

	private GridColumnEx gcContent;

	private GridColumnEx gcMAttachT;

	private GridColumnEx gcDocType;

	private XtraTabPageEx tabGeneral;

	private GridControlEx grdGeneral;

	private GridViewEx gvGeneral;

	private GridColumnEx gcOrigin;

	private GridColumnEx gcProblemType;

	private GridColumnEx gcProblemSubtype;

	private GridColumnEx gcCallType;

	private GridColumnEx gcTechnicianName;

	private GridColumnEx gcIsQueue;

	private GridColumnEx gcHandledByInGeneral;

	private GridColumnEx gcQueue;

	private GridColumnEx gcResponseByDate;

	private GridColumnEx gcResponseByTime;

	private GridColumnEx gcResponseOnDate;

	private GridColumnEx gcResponseOnTime;

	private GridColumnEx gcResolutionByDate;

	private GridColumnEx gcResolutionByTime;

	private GridColumnEx gcResolutionOnDate;

	private GridColumnEx gcResolutionOnTime;

	private XtraTabPageEx tabRemarks;

	private TextBoxEx txtRemarks;

	private XtraTabPageEx tabResolution;

	private GridColumnEx gcSltCode;

	private GridColumnEx gcSolution;

	private GridColumnEx gcSolutionCreatedOn;

	private GridColumnEx gcOwner;

	private GridColumnEx gcStatus;

	private GridColumnEx gcHandledbyInSolutions;

	private GridControlEx grdExpenses;

	private GridViewEx gvExpenses;

	private GridColumnEx gcDocumentType;

	private GridColumnEx gcDocumentNo;

	private GridColumnEx gcDateInExpenses;

	private GridColumnEx gcItemNo;

	private GridColumnEx gcItemDescription;

	private GridColumnEx gcQuantity;

	private GridColumnEx gcFromWhse;

	private GridColumnEx gcToWhse;

	private TextBoxEx txtResolution;

	private GridControlEx grdHistory;

	private GridViewEx gvHistory;

	private GridColumnEx gcDateofUpdate;

	private GridColumnEx gcUpdateTime;

	private GridColumnEx gcUpdatedby;

	private GridColumnEx gcDescriptionInHistory;

	private GridColumnEx gcPreviousValue;

	private GridColumnEx gcNewValue;

	private GridControlEx grdScheduling;

	private GridViewEx gvScheduling;

	private GridColumnEx gcStartDate;

	private GridColumnEx gcStartTime;

	private GridColumnEx gcEndDateInScheduling;

	private GridColumnEx gcEndTime;

	private GridColumnEx gcDuration;

	private GridColumnEx gcReminder;

	private GridColumnEx gcDisplayinCalendar;

	private GridColumnEx gcRemQty;

	private GridColumnEx gcMeetingLocation;

	private GridColumnEx gcStreet;

	private GridColumnEx gcCity;

	private GridColumnEx gcRoom;

	private GridColumnEx gcState;

	private GridColumnEx gcCountry;

	private RepositoryItemButtonEdit rCustomer;

	private RepositoryItemLookUpEdit rOrigin;

	private RepositoryItemLookUpEdit rProblemType;

	private RepositoryItemLookUpEdit rProblemSubtype;

	private RepositoryItemButtonEdit rName;

	private RepositoryItemLookUpEdit rContactPerson;

	private RepositoryItemButtonEdit rMfrSerialNo;

	private RepositoryItemButtonEdit rSerialNumber;

	private RepositoryItemButtonEdit rItem;

	private RepositoryItemButtonEdit rDescription;

	private RepositoryItemLookUpEdit rNo;

	private RepositoryItemLookUpEdit rCallStatus;

	private RepositoryItemLookUpEdit rPriority;

	private RepositoryItemDateEdit rCreatedDate;

	private RepositoryItemLookUpEdit rCallType;

	private RepositoryItemButtonEdit rTechnician;

	private RepositoryItemCheckEdit rIsQueue;

	private RepositoryItemLookUpEdit rHandledByInGeneral;

	private RepositoryItemDateEdit rDate;

	private RepositoryItemCheckEdit rReminder;

	private RepositoryItemCheckEdit rDisplayinCalendar;

	private RepositoryItemLookUpEdit rMeetingLocation;

	private RepositoryItemLookUpEdit rState;

	private RepositoryItemLookUpEdit rCountry;

	private GridColumnEx gcAddressID;

	private ContextMenuStrip contextMenuDoc;

	private ToolStripMenuItem addToolStripMenuItem;

	private RepositoryItemLookUpEdit rItemGroup;

	private GridColumnEx gcDurType;

	private RepositoryItemLookUpEdit rDurType;

	private GridColumnEx gcRemType;

	private RepositoryItemLookUpEdit rRemType;

	private SimpleButtonEx btnActivities;

	private SimpleButtonEx btnRecommend;

	private CheckBox cbDisplay;

	private RepositoryItemLookUpEdit rContractNo;

	private RepositoryItemTimeEdit rCreateTime;

	private RepositoryItemDateEdit rDateInGerenal;

	private RepositoryItemTimeEdit rTimeInGeneral;

	private RepositoryItemTimeEdit rStartTime;

	private RepositoryItemTextEdit rDuration;

	private RepositoryItemTextEdit rRemQty;

	private LookUpEdit txtCallStatus;

	private LookUpEdit txtPriority;

	private RepositoryItemLookUpEdit rHandledby;

	private RepositoryItemLookUpEdit rOwner;

	private RepositoryItemLookUpEdit rUpdatedbyInHistory;

	private RepositoryItemTimeEdit rEndtime;

	private ContextMenuStrip contextMenuActivities;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuSolution;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private RepositoryItemLookUpEdit rAddressID;

	private GridColumnEx AddrType;

	private XtraTabPageEx tabBP;

	private GridControlEx grdBusinessPartner;

	private GridViewEx gvBusinessPartner;

	private GridColumnEx gcShipCode;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private GridColumnEx gcShipAddress;

	private GridColumnEx gcBillCode;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit2;

	private GridColumnEx gcBillAddress;

	private GridColumnEx gcTerritory;

	private GridColumnEx gcContactPerson;

	private GridColumnEx gcPhone1;

	private GridColumnEx gcPhone2;

	private GridColumnEx gcMobilePhone;

	private GridColumnEx gcFax;

	private GridColumnEx gcEMail;

	private GridColumnEx gcBPProject;

	private RepositoryItemButtonEdit rBPProjCode;

	private GridColumnEx gcTechnician;

	private RepositoryItemButtonEdit rQueue;

	private GridColumnEx gcCode;

	private GridColumnEx gcStatusName;

	private GridColumnEx gridColumn2;

	public ucAttachment ucAttachment1;

	private DateEditEx txtFromDate;

	private DateEditEx txtToDate;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (authoritioAuthorities == Enums.FormAuthorities.Read || authoritioAuthorities == Enums.FormAuthorities.NoAuthorities)
			{
				_formMode = FormMode;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				btnSave.Text = _saveString;
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Find:
				EnableDisableControl(isEnable: true);
				grdDoc.DataSource = null;
				grdDoc.RefreshDataSource();
				grdActivities.DataSource = null;
				grdActivities.RefreshDataSource();
				ucAttachment1.DataSource = null;
				ucAttachment1.grd_File.RefreshDataSource();
				grdExpenses.DataSource = null;
				grdExpenses.RefreshDataSource();
				grdGeneral.DataSource = null;
				grdGeneral.RefreshDataSource();
				grdHistory.DataSource = null;
				grdHistory.RefreshDataSource();
				grdScheduling.DataSource = null;
				grdScheduling.RefreshDataSource();
				grdSolutions.DataSource = null;
				grdSolutions.RefreshDataSource();
				btnSave.Enabled = false;
				ucAttachment1.EnableButtons(isEnabled: false);
				btnRecommend.Enabled = false;
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				btnSave.Text = _updateString;
				break;
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				btnSave.Text = _okString;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public Frm_ServiceCall()
	{
		InitializeComponent();
		OnLoadAsync += Frm_ServiceCall_Load;
	}

	private async Task Frm_ServiceCall_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			btnActivities.Enabled = false;
			ucAttachment1.EnableButtons(isEnabled: false);
			btnRecommend.Enabled = false;
			btnSave.Enabled = false;
			contextMenuActivities.Enabled = false;
			contextMenuDoc.Enabled = false;
			contextMenuSolution.Enabled = false;
			gcCreatedDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcEndDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcClosedDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rCreatedDate.Mask.EditMask = GlobalData.DateTimeFormat;
			gcResponseOnDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcResponseByDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcResolutionOnDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcResolutionByDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rTimeInGeneral.Mask.EditMask = GlobalData.DateTimeFormat;
			gcStartDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcEndDateInScheduling.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			rDate.Mask.EditMask = GlobalData.DateTimeFormat;
			gcActivityDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcSolutionCreatedOn.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcDateInExpenses.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			gcDateofUpdate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			ucAttachment1.gcDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			txtFromDate.Properties.Mask.EditMask = GlobalData.DateTimeFormat;
			txtFromDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			txtToDate.Properties.Mask.EditMask = GlobalData.DateTimeFormat;
			txtToDate.Properties.Mask.UseMaskAsDisplayFormat = true;
			txtFromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			txtToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			await FormLoad();
			await LoadDoc();
			LoadExpenses();
			Frm_ServiceCall frm_ServiceCall = this;
			GridViewEx gridViewEx = gvDoc;
			GridViewEx gridViewEx2 = gvGeneral;
			GridViewEx gridViewEx3 = gvActivities;
			GridViewEx gridViewEx4 = gvSolutions;
			string text = (gvExpenses.FormId = "oServiceCall");
			string text3 = (gridViewEx4.FormId = text);
			string text5 = (gridViewEx3.FormId = text3);
			string text7 = (gridViewEx2.FormId = text5);
			string formId = (gridViewEx.FormId = text7);
			frm_ServiceCall.FormId = formId;
			gvDoc.ItemId = "OSCL";
			gvGeneral.ItemId = "OSCL1";
			gvActivities.ItemId = "OCLG";
			gvSolutions.ItemId = "OSLT";
			gvExpenses.ItemId = "SCL4";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gvDoc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_ = 3;
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			int callId = Function.ParseInt(gvDoc.GetFocusedRowCellValue("callID"));
			if (callId >= 0)
			{
				if (callId == 0)
				{
					return;
				}
				WinCommons._OpenProcessing();
				await LoadLine(callId);
				await LoadActivities(callId, 0);
				await LoadSolutions(callId, 0);
				int absEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry"));
				await ucAttachment1.LoadAttachments(absEntry);
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("status")) == -1)
				{
					grdActivities.Enabled = false;
					grdSolutions.Enabled = false;
					grdScheduling.Enabled = false;
					grdHistory.Enabled = false;
					grdBusinessPartner.Enabled = false;
					grdExpenses.Enabled = false;
					txtRemarks.Enabled = false;
					txtResolution.Enabled = false;
					btnActivities.Enabled = false;
					btnRecommend.Enabled = false;
					ucAttachment1.EnableButtons(isEnabled: false);
				}
				else
				{
					grdActivities.Enabled = true;
					grdSolutions.Enabled = true;
					grdScheduling.Enabled = true;
					grdHistory.Enabled = true;
					grdBusinessPartner.Enabled = true;
					grdExpenses.Enabled = true;
					txtRemarks.Enabled = true;
					txtResolution.Enabled = true;
					btnActivities.Enabled = true;
					btnRecommend.Enabled = true;
					ucAttachment1.EnableButtons(isEnabled: true);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private async void rNo_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gvDoc.EditingValue);
			if (num == -1)
			{
				gvDoc.SetFocusedRowCellValue("DocNum", 0);
				return;
			}
			GridViewEx gridViewEx = gvDoc;
			gridViewEx.SetFocusedRowCellValue("DocNum", await GetDocNum(num));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rAddressID_Closed(object sender, ClosedEventArgs e)
	{
		try
		{
			string adressId = Function.ToString(gvScheduling.EditingValue);
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("customer"));
			LoadStreet(cardCode, adressId);
			GridViewEx gridViewEx = gvScheduling;
			gridViewEx.SetFocusedRowCellValue("AddrType", await GetAddrType(adressId));
			gvScheduling.SetFocusedRowCellValue("AddrName", adressId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rCountry_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string country = Function.ToString(gvScheduling.EditingValue);
			await GetDataStateInScheduling(country);
			gvScheduling.SetFocusedRowCellValue("Country", country);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (authoritioAuthorities == Enums.FormAuthorities.Full)
			{
				int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Series"));
				if (gvDoc.FocusedColumn.FieldName == "DocNum" && num != -1)
				{
					e.Cancel = true;
				}
				if (btnSave.Text == _updateString && (gvDoc.FocusedColumn.FieldName == "Series" || gvDoc.FocusedColumn.FieldName == "DocNum"))
				{
					e.Cancel = true;
				}
				if (btnSave.Text == _saveString && (gvDoc.FocusedColumn.FieldName == "closeDate" || gvDoc.FocusedColumn.FieldName == "closeTime"))
				{
					e.Cancel = true;
				}
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("status")) == -1)
				{
					if (gvDoc.FocusedColumn.FieldName != "status" && gvDoc.FocusedColumn.FieldName != "closeDate" && gvDoc.FocusedColumn.FieldName != "closeTime")
					{
						e.Cancel = true;
					}
				}
				else if (gvDoc.FocusedColumn.FieldName == "closeTime" || gvDoc.FocusedColumn.FieldName == "closeDate")
				{
					e.Cancel = true;
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rCustomer_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string cardCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCustomer));
				ChooseFormListCardCode(cardCode);
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCustomer));
			if (!string.IsNullOrEmpty(text))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = text;
				frm_BusinessMasterData2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string cardCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCustomer));
				ChooseFormListCardCode(cardCode);
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcCustomer));
			if (!string.IsNullOrEmpty(text))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = text;
				frm_BusinessMasterData2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rCallStatus_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvDoc.EditingValue) == -1 && btnSave.Text == _saveString)
			{
				gvDoc.SetFocusedRowCellValue("status", -3);
				WinCommons.Error(Enums.MessageCode.CanNotClose, "");
			}
			if (Function.ParseInt(gvDoc.EditingValue) == -1)
			{
				txtRemarks.Enabled = false;
				txtResolution.Enabled = false;
				ucAttachment1.EnableButtons(isEnabled: false);
				btnRecommend.Enabled = false;
				cbDisplay.Enabled = false;
			}
			else
			{
				txtResolution.Enabled = true;
				txtRemarks.Enabled = true;
				ucAttachment1.EnableButtons(isEnabled: true);
				btnRecommend.Enabled = true;
				cbDisplay.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvScheduling_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void rDuration_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			switch (Function.ToString(gvScheduling.GetFocusedRowCellValue("DurType")))
			{
			case "M":
			{
				int num7 = Function.ParseInt(gvScheduling.EditingValue);
				int num2 = num7 / 518400;
				int num3 = (num7 - num2 * 518400) / 43200;
				int num4 = (num7 - num3 * 43200 - num2 * 518400) / 1440;
				int num6 = (num7 - num3 * 43200 - num2 * 518400 - num4 * 1440) / 60;
				int num8 = num7 - num2 * 518400 - num3 * 43200 - num4 * 1440 - num6 * 60;
				DateTime dateTime2 = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartTime")));
				DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartDate")));
				num2 += dateTime.Year;
				num3 += dateTime.Month;
				num4 += dateTime.Day;
				num6 += dateTime2.Hour;
				num8 += dateTime2.Minute;
				while (num8 > 60)
				{
					num8 -= 60;
					num6++;
				}
				while (num6 > 24)
				{
					num6 -= 24;
					num4++;
				}
				while (num4 > 30)
				{
					num4 -= 30;
					num3++;
				}
				while (num3 > 12)
				{
					num3 -= 12;
					num2++;
				}
				if (num2 > 10000)
				{
					num2 = DateTime.MaxValue.Year;
				}
				gvScheduling.SetFocusedRowCellValue("EndDate", num4 + "/" + num3 + "/" + num2);
				if (num8 < 10)
				{
					if (num6 < 10)
					{
						gvScheduling.SetFocusedRowCellValue("EndTime", "0" + num6 + ":0" + dateTime2.Minute);
					}
					gvScheduling.SetFocusedRowCellValue("EndTime", num6 + ":0" + num8);
				}
				else
				{
					if (num6 < 10)
					{
						gvScheduling.SetFocusedRowCellValue("EndTime", "0" + num6 + ":" + dateTime2.Minute);
					}
					gvScheduling.SetFocusedRowCellValue("EndTime", num6 + ":" + num8);
				}
				break;
			}
			case "H":
			{
				int num5 = Function.ParseInt(gvScheduling.EditingValue);
				int num2 = num5 / 8640;
				int num3 = (num5 - num2 * 8640) / 720;
				int num4 = (num5 - num2 * 8640 - num3 * 720) / 24;
				int num6 = num5 - num2 * 8640 - num3 * 720 - num4 * 24;
				DateTime dateTime2 = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartTime")));
				DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartDate")));
				num2 += dateTime.Year;
				num3 += dateTime.Month;
				num4 += dateTime.Day;
				num6 += dateTime2.Hour;
				while (num6 > 24)
				{
					num6 -= 24;
					num4++;
				}
				while (num4 > 30)
				{
					num4 -= 30;
					num3++;
				}
				while (num3 > 12)
				{
					num3 -= 12;
					num2++;
				}
				if (num2 > 10000)
				{
					num2 = DateTime.MaxValue.Year;
				}
				gvScheduling.SetFocusedRowCellValue("EndDate", num4 + "/" + num3 + "/" + num2);
				if (dateTime2.Minute < 10)
				{
					if (num6 < 10)
					{
						gvScheduling.SetFocusedRowCellValue("EndTime", "0" + num6 + ":0" + dateTime2.Minute);
					}
					gvScheduling.SetFocusedRowCellValue("EndTime", num6 + ":0" + dateTime2.Minute);
				}
				else
				{
					if (num6 < 10)
					{
						gvScheduling.SetFocusedRowCellValue("EndTime", "0" + num6 + ":" + dateTime2.Minute);
					}
					gvScheduling.SetFocusedRowCellValue("EndTime", num6 + ":" + dateTime2.Minute);
				}
				break;
			}
			case "D":
			{
				int num = Function.ParseInt(gvScheduling.EditingValue);
				int num2 = num / 360;
				int num3 = (num - num2 * 360) / 30;
				int num4 = num - num2 * 360 - num3 * 30;
				DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartDate")));
				num2 += dateTime.Year;
				num3 += dateTime.Month;
				num4 += dateTime.Day;
				while (num4 > 30)
				{
					num4 -= 30;
					num3++;
				}
				while (num3 > 12)
				{
					num3 -= 12;
					num2++;
				}
				if (num2 > 10000)
				{
					num2 = DateTime.MaxValue.Year;
				}
				gvScheduling.SetFocusedRowCellValue("EndDate", num4 + "/" + num3 + "/" + num2);
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

	private void rDurType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			switch (Function.ToString(gvScheduling.EditingValue))
			{
			case "M":
			{
				int num7 = Function.ParseInt(gvScheduling.GetFocusedRowCellValue("Duration"));
				int num2 = num7 / 518400;
				int num3 = (num7 - num2 * 518400) / 43200;
				int num4 = (num7 - num3 * 43200 - num2 * 518400) / 1440;
				int num6 = (num7 - num3 * 43200 - num2 * 518400 - num4 * 1440) / 60;
				int num8 = num7 - num2 * 518400 - num3 * 43200 - num4 * 1440 - num6 * 60;
				DateTime dateTime2 = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartTime")));
				DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartDate")));
				num2 += dateTime.Year;
				num3 += dateTime.Month;
				num4 += dateTime.Day;
				num6 += dateTime2.Hour;
				num8 += dateTime2.Minute;
				while (num8 > 60)
				{
					num8 -= 60;
					num6++;
				}
				while (num6 > 24)
				{
					num6 -= 24;
					num4++;
				}
				while (num4 > 30)
				{
					num4 -= 30;
					num3++;
				}
				while (num3 > 12)
				{
					num3 -= 12;
					num2++;
				}
				if (num2 > 10000)
				{
					num2 = DateTime.MaxValue.Year;
				}
				gvScheduling.SetFocusedRowCellValue("EndDate", num4 + "/" + num3 + "/" + num2);
				gvScheduling.SetFocusedRowCellValue("EndTime", num6 + ":" + num8);
				break;
			}
			case "H":
			{
				int num5 = Function.ParseInt(gvScheduling.GetFocusedRowCellValue("Duration"));
				int num2 = num5 / 8640;
				int num3 = (num5 - num2 * 8640) / 720;
				int num4 = (num5 - num2 * 8640 - num3 * 720) / 24;
				int num6 = num5 - num2 * 8640 - num3 * 720 - num4 * 24;
				DateTime dateTime2 = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartTime")));
				DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartDate")));
				num2 += dateTime.Year;
				num3 += dateTime.Month;
				num4 += dateTime.Day;
				num6 += dateTime2.Hour;
				while (num6 > 24)
				{
					num6 -= 24;
					num4++;
				}
				while (num4 > 30)
				{
					num4 -= 30;
					num3++;
				}
				while (num3 > 12)
				{
					num3 -= 12;
					num2++;
				}
				if (num2 > 10000)
				{
					num2 = DateTime.MaxValue.Year;
				}
				gvScheduling.SetFocusedRowCellValue("EndDate", num4 + "/" + num3 + "/" + num2);
				gvScheduling.SetFocusedRowCellValue("EndTime", num6 + ":" + dateTime2.Minute);
				break;
			}
			case "D":
			{
				int num = Function.ParseInt(gvScheduling.GetFocusedRowCellValue("Duration"));
				int num2 = num / 360;
				int num3 = (num - num2 * 360) / 30;
				int num4 = num - num2 * 360 - num3 * 30;
				DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartDate")));
				num2 += dateTime.Year;
				num3 += dateTime.Month;
				num4 += dateTime.Day;
				while (num4 > 30)
				{
					num4 -= 30;
					num3++;
				}
				while (num3 > 12)
				{
					num3 -= 12;
					num2++;
				}
				if (num2 > 10000)
				{
					num2 = DateTime.MaxValue.Year;
				}
				gvScheduling.SetFocusedRowCellValue("EndDate", num4 + "/" + num3 + "/" + num2);
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

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await LoadDoc();
			LoadExpenses();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void txtCustomer_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, txtCustomer.Text, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				txtCustomer.Text = Function.ToString(chooseItem["CardCode"]);
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
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private void btnActivities_Click(object sender, EventArgs e)
	{
		try
		{
			if (btnActivities.Text == Apzon.Commons.Properties.Resources.ButtonActivities)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("customer"))))
				{
					try
					{
						int linkDocNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
						frm_ActivitiesForOpportunity frm_ActivitiesForOpportunity2 = new frm_ActivitiesForOpportunity();
						frm_ActivitiesForOpportunity2._linkeDocType = 191;
						frm_ActivitiesForOpportunity2._linkDocNum = linkDocNum;
						frm_ActivitiesForOpportunity2._linkCardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("customer"));
						frm_ActivitiesForOpportunity2.frmSCall = this;
						frm_ActivitiesForOpportunity2.MdiParent = MdiParent;
						frm_ActivitiesForOpportunity2.Show();
						return;
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
						WinCommons.ErrorLineNumber(ex);
						return;
					}
				}
			}
			else if (btnActivities.Text == "New" && !string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("customer"))))
			{
				frm_Solution frm_Solution2 = new frm_Solution();
				frm_Solution2.frmSCall = this;
				frm_Solution2.newLine = true;
				frm_Solution2.MdiParent = MdiParent;
				frm_Solution2.Show();
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void rItem_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string itemCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItem));
				ChooseFormListItemCode(itemCode);
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItem));
			if (!string.IsNullOrEmpty(text))
			{
				ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rDescription_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string itemCode = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcItem));
				ChooseFormListItemCode(itemCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rTechnician_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseFromTechnicianList();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (authoritioAuthorities == Enums.FormAuthorities.Full)
			{
				string text = Function.ToString(gvGeneral.GetFocusedRowCellValue("isQueue"));
				if (gvGeneral.FocusedColumn.FieldName == "assignee" && text == "Y")
				{
					gvGeneral.SetFocusedRowCellValue("Queue", "");
					e.Cancel = true;
				}
				if (gvGeneral.FocusedColumn.FieldName == "Queue" && text == "N")
				{
					gvGeneral.SetFocusedRowCellValue("Queue", "");
					e.Cancel = true;
				}
				if (gvDoc.GetFocusedRowCellDisplayText("status") == "Closed")
				{
					e.Cancel = true;
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvScheduling_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (authoritioAuthorities == Enums.FormAuthorities.Full)
			{
				int num = Function.ParseInt(gvScheduling.GetFocusedRowCellValue("Location"));
				if (gvScheduling.FocusedColumn.FieldName == "AddrName" && num == -1)
				{
					e.Cancel = true;
				}
				if (gvDoc.GetFocusedRowCellDisplayText("status") == "Closed")
				{
					e.Cancel = true;
				}
			}
			else
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tabLine_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			switch (e.Page.Text)
			{
			case "Activities":
				btnActivities.Visible = true;
				btnActivities.Text = Apzon.Commons.Properties.Resources.ButtonActivities;
				btnRecommend.Visible = false;
				cbDisplay.Visible = false;
				break;
			case "Solutions":
				btnActivities.Visible = true;
				btnActivities.Text = "New";
				btnRecommend.Visible = true;
				cbDisplay.Visible = false;
				btnRecommend.Text = "Recommend";
				break;
			case "Expenses":
				btnActivities.Visible = true;
				btnActivities.Text = "Ex&penses Details";
				btnRecommend.Visible = false;
				cbDisplay.Visible = true;
				break;
			case "History":
				btnActivities.Visible = true;
				btnActivities.Text = "Ex&pand All";
				btnRecommend.Visible = true;
				btnRecommend.Text = "Co&llapse All";
				cbDisplay.Visible = false;
				break;
			default:
				btnActivities.Visible = false;
				btnRecommend.Visible = false;
				cbDisplay.Visible = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rMfrSerialNo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void rSerialNumber_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			Function.ToString(gvDoc.GetFocusedRowCellValue("customer"));
			if (gvDoc.FocusedRowHandle > -1)
			{
				if (_OpenChooseCFL)
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

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			await AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private void rCreateTime_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = Convert.ToDateTime(Function.ToString(gvDoc.EditingValue));
			if (gvDoc.FocusedColumn.FieldName == "createTime")
			{
				if (dateTime.Minute < 10)
				{
					if (dateTime.Hour < 10)
					{
						gvDoc.SetFocusedRowCellValue("createTime", "0" + dateTime.Hour + ":0" + dateTime.Minute);
					}
					else
					{
						gvDoc.SetFocusedRowCellValue("createTime", dateTime.Hour + ":0" + dateTime.Minute);
					}
				}
				else if (dateTime.Hour < 10)
				{
					gvDoc.SetFocusedRowCellValue("createTime", "0" + dateTime.Hour + ":" + dateTime.Minute);
				}
				else
				{
					gvDoc.SetFocusedRowCellValue("createTime", dateTime.Hour + ":" + dateTime.Minute);
				}
			}
			else if (dateTime.Minute < 10)
			{
				if (dateTime.Hour < 10)
				{
					gvDoc.SetFocusedRowCellValue("closeTime", "0" + dateTime.Hour + ":0" + dateTime.Minute);
				}
				else
				{
					gvDoc.SetFocusedRowCellValue("closeTime", dateTime.Hour + ":0" + dateTime.Minute);
				}
			}
			else if (dateTime.Hour < 10)
			{
				gvDoc.SetFocusedRowCellValue("closeTime", "0" + dateTime.Hour + ":" + dateTime.Minute);
			}
			else
			{
				gvDoc.SetFocusedRowCellValue("closeTime", dateTime.Hour + ":" + dateTime.Minute);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rTimeInScheduling_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.EditingValue));
			if (dateTime.Minute < 10)
			{
				if (dateTime.Hour < 10)
				{
					gvScheduling.SetFocusedRowCellValue("StartTime", "0" + dateTime.Hour + ":0" + dateTime.Minute);
				}
				else
				{
					gvScheduling.SetFocusedRowCellValue("StartTime", "0" + dateTime.Hour + ":0" + dateTime.Minute);
				}
			}
			else if (dateTime.Hour < 10)
			{
				gvScheduling.SetFocusedRowCellValue("StartTime", "0" + dateTime.Hour + ":" + dateTime.Minute);
			}
			else
			{
				gvScheduling.SetFocusedRowCellValue("StartTime", "0" + dateTime.Hour + ":" + dateTime.Minute);
			}
			DateTime dateTime2 = Convert.ToDateTime(Function.ToString(gvScheduling.EditingValue));
			DateTime dateTime3 = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("EndTime")));
			if (dateTime3 < dateTime2)
			{
				gvScheduling.SetFocusedRowCellValue("EndTime", Function.ToString(gvScheduling.EditingValue));
				gvScheduling.SetFocusedRowCellValue("Duration", 0);
			}
			else
			{
				gvScheduling.SetFocusedRowCellValue("Duration", (dateTime3.Hour - dateTime2.Hour) * 60 + (dateTime3.Minute - dateTime2.Minute));
				gvScheduling.SetFocusedRowCellValue("DurType", "M");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rEndtime_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = Convert.ToDateTime(Function.ToString(gvScheduling.EditingValue));
			if (dateTime.Minute < 10)
			{
				if (dateTime.Hour < 10)
				{
					gvScheduling.SetFocusedRowCellValue("EndTime", "0" + dateTime.Hour + ":0" + dateTime.Minute);
				}
				else
				{
					gvScheduling.SetFocusedRowCellValue("EndTime", dateTime.Hour + ":0" + dateTime.Minute);
				}
			}
			else if (dateTime.Hour < 10)
			{
				gvScheduling.SetFocusedRowCellValue("EndTime", "0" + dateTime.Hour + ":" + dateTime.Minute);
			}
			else
			{
				gvScheduling.SetFocusedRowCellValue("EndTime", dateTime.Hour + ":" + dateTime.Minute);
			}
			DateTime dateTime2 = Convert.ToDateTime(Function.ToString(gvScheduling.GetFocusedRowCellValue("StartTime")));
			DateTime dateTime3 = Convert.ToDateTime(Function.ToString(gvScheduling.EditingValue));
			if (dateTime3 < dateTime2)
			{
				gvScheduling.SetFocusedRowCellValue("StartTime", Function.ToString(gvScheduling.EditingValue));
				gvScheduling.SetFocusedRowCellValue("Duration", 0);
			}
			else
			{
				gvScheduling.SetFocusedRowCellValue("Duration", (dateTime3.Hour - dateTime2.Hour) * 60 + (dateTime3.Minute - dateTime2.Minute));
				gvScheduling.SetFocusedRowCellValue("DurType", "M");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grdActivities.DataSourceTable != null && grdActivities.DataSourceTable.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				gvActivities.DeleteSelectedRows();
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (grdSolutions.DataSourceTable != null && grdSolutions.DataSourceTable.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				gvSolutions.DeleteSelectedRows();
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnRecommend_Click(object sender, EventArgs e)
	{
		try
		{
			if (btnRecommend.Text == "Recommend" && !string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("customer"))))
			{
				try
				{
					frm_Solution frm_Solution2 = new frm_Solution();
					frm_Solution2.frmSCall = this;
					frm_Solution2.MdiParent = MdiParent;
					frm_Solution2.Show();
					return;
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
					WinCommons.ErrorLineNumber(ex);
					return;
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (btnSave.Text == _okString)
			{
				Close();
				return;
			}
			DataTable doc = grdDoc.DataSourceTable.Copy();
			DataTable dataTable = doc.Clone();
			DataRow dataRow = dataTable.NewRow();
			DataRow dataRow2 = gvDoc.GetDataRow(gvDoc.FocusedRowHandle);
			if (dataRow2 == null)
			{
				return;
			}
			dataRow.ItemArray = dataRow2.ItemArray;
			if (string.IsNullOrEmpty(Function.ToString(dataRow["subject"])) || string.IsNullOrEmpty(Function.ToString(dataRow["customer"])))
			{
				WinCommons.Error(Enums.MessageCode.SubjectCustomerIsNull, "");
				return;
			}
			DataTable dataSourceTable = grdGeneral.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
			{
				dataSourceTable.Rows[0]["descrption"] = txtRemarks.Text;
				dataSourceTable.Rows[0]["resolution"] = txtResolution.Text;
			}
			DataSet dataSet = new DataSet();
			dataTable.Rows.Add(dataRow);
			dataSet.Tables.Add(dataTable.Copy());
			if (dataSourceTable != null)
			{
				dataSet.Tables.Add(dataSourceTable.Copy());
			}
			dataSet.Tables.Add((grdActivities.DataSourceTable != null) ? grdActivities.DataSourceTable.Copy() : new DataTable().Copy());
			dataSet.Tables.Add((grdExpenses.DataSourceTable != null) ? grdExpenses.DataSourceTable.Copy() : new DataTable().Copy());
			dataSet.Tables.Add((grdSolutions.DataSource != null) ? grdSolutions.DataSourceTable.Copy() : new DataTable().Copy());
			List<AttachmentFile_Impl> list = ucAttachment1.UploadFile();
			if (ucAttachment1.DataSource.IsNotNull() && list.Count == 0)
			{
				return;
			}
			Tuple<DataSet, List<AttachmentFile_Impl>> data = new Tuple<DataSet, List<AttachmentFile_Impl>>(dataSet, list);
			string text = await Save(data);
			if (string.IsNullOrEmpty(text))
			{
				if (btnSave.Text == _saveString)
				{
					string userName = GlobalData.UserName;
					string message = "*Company Name: " + GlobalData.CompanyName + "*.\n```Object Name:  " + doc.Rows[0]["subject"]?.ToString() + "```.\n```User Name:  " + GlobalData.UserName + "```.\n```Customer Name: " + doc.Rows[0]["custmrName"]?.ToString() + "```.\n```Problem type: " + gvGeneral.GetFocusedRowCellDisplayText("problemTyp") + "```.\n```created: " + DateTime.Now.ToString("dd/MM/yyyy hh:mm") + "```.";
					SendMessage(userName, message);
				}
				await LoadDoc();
				LoadExpenses();
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				btnSave.Text = _okString;
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

	private async void rContactPerson_EditValueChanging(object sender, ChangingEventArgs e)
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
					gvDoc.SetFocusedRowCellValue("TelephoneNo", Function.ToString(dataTable2.Rows[0]["Tel1"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rBPProjCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseFromBpList();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rQueue_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseFromQueueList();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grdActivities_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int linkDocNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("DocNum"));
			frm_ActivitiesForOpportunity frm_ActivitiesForOpportunity2 = new frm_ActivitiesForOpportunity();
			frm_ActivitiesForOpportunity2._clgCode_FromOppotunity = Function.ParseInt(gvActivities.GetFocusedRowCellValue("ClgCode"));
			frm_ActivitiesForOpportunity2._linkeDocType = 191;
			frm_ActivitiesForOpportunity2._linkDocNum = linkDocNum;
			frm_ActivitiesForOpportunity2._linkCardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("customer"));
			frm_ActivitiesForOpportunity2.frmSCall = this;
			frm_ActivitiesForOpportunity2.MdiParent = MdiParent;
			frm_ActivitiesForOpportunity2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvSolutions_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			frm_Solution frm_Solution2 = new frm_Solution();
			frm_Solution2.frmSCall = this;
			frm_Solution2._sltCode = Function.ParseInt(gvSolutions.GetFocusedRowCellValue("SltCode"));
			frm_Solution2.newLine = false;
			frm_Solution2.MdiParent = MdiParent;
			frm_Solution2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvGeneral_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvGeneral_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvBusinessPartner_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvBusinessPartner_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void txtRemarks_TextChanged(object sender, EventArgs e)
	{
		SetViewUpdate();
	}

	private void gvActivities_RowUpdated(object sender, RowObjectEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvSolutions_RowUpdated(object sender, RowObjectEventArgs e)
	{
		SetViewUpdate();
	}

	private void txtResolution_TextChanged(object sender, EventArgs e)
	{
		SetViewUpdate();
	}

	private void gvScheduling_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		SetViewUpdate();
	}

	private void gvDoc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "contctCode")
			{
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "customer"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
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
					repositoryItemLookUpEdit.DataSource = array.CopyToDataTable();
				}
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctName", 20, "Name"));
				e.RepositoryItem = repositoryItemLookUpEdit;
				e.RepositoryItem.EditValueChanging += rContactPerson_EditValueChanging;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "contctCode")
			{
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "customer"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
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
					repositoryItemLookUpEdit.DataSource = array.CopyToDataTable();
				}
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctCode", 20, "Code"));
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("CntctName", 20, "Name"));
				e.RepositoryItem = repositoryItemLookUpEdit;
				e.RepositoryItem.EditValueChanging += rContactPerson_EditValueChanging;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvActivities_RowCountChanged(object sender, EventArgs e)
	{
		SetViewUpdate();
	}

	private void gvSolutions_RowCountChanged(object sender, EventArgs e)
	{
		SetViewUpdate();
	}

	private void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!(sender is GridControl gridControl))
			{
				return;
			}
			GridView gridView = gridControl.FocusedView as GridView;
			if (e.KeyData != Keys.Tab)
			{
				return;
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "ITEMCODE")
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "customer"))) && _OpenChooseCFL && focusedRowHandle > -1)
				{
					string text = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "itemCode"));
					if (string.IsNullOrEmpty(text))
					{
						ChooseFormListItemCode(text);
					}
				}
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "ITEMNAME")
			{
				int focusedRowHandle2 = gvDoc.FocusedRowHandle;
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle2, "customer"))) && _OpenChooseCFL && focusedRowHandle2 > -1)
				{
					string text2 = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle2, "itemCode"));
					if (string.IsNullOrEmpty(text2))
					{
						ChooseFormListItemCode(text2);
					}
				}
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "CUSTOMER")
			{
				if (_OpenChooseCFL)
				{
					int focusedRowHandle3 = gvDoc.FocusedRowHandle;
					if (focusedRowHandle3 > -1)
					{
						string text3 = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle3, "customer"));
						if (string.IsNullOrEmpty(text3))
						{
							ChooseFormListCardCode(text3);
						}
					}
				}
			}
			else if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "OWNERNAME")
			{
				WinFuntions.LoadOwnerCode(gvDoc);
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "CUSTMRNAME")
			{
				if (_OpenChooseCFL)
				{
					int focusedRowHandle4 = gvDoc.FocusedRowHandle;
					if (focusedRowHandle4 > -1)
					{
						string text4 = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle4, "customer"));
						if (string.IsNullOrEmpty(text4))
						{
							ChooseFormListCardCode(text4);
						}
					}
				}
			}
			else if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "OWNERNAME")
			{
				WinFuntions.LoadOwnerCode(gvDoc);
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "MANUFSN")
			{
				int focusedRowHandle5 = gvDoc.FocusedRowHandle;
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle5, "customer"))) && _OpenChooseCFL && focusedRowHandle5 > -1 && string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle5, "manufSN"))))
				{
					ChooseFormListSn();
				}
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "INTERNALSN")
			{
				int focusedRowHandle6 = gvDoc.FocusedRowHandle;
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle6, "customer"))) && _OpenChooseCFL && focusedRowHandle6 > -1 && string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle6, "internalSN"))))
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

	private void grdBusinessPartner_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!(sender is GridControl gridControl))
			{
				return;
			}
			GridView gridView = gridControl.FocusedView as GridView;
			if (e.KeyData != Keys.Tab)
			{
				return;
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "BPPROJCODE")
			{
				if (_OpenChooseCFL && gvDoc.FocusedRowHandle > -1)
				{
					ChooseFromBpList();
				}
			}
			else if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "OWNERNAME")
			{
				WinFuntions.LoadOwnerCode(gvDoc);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grdGeneral_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!(sender is GridControl gridControl))
			{
				return;
			}
			GridView gridView = gridControl.FocusedView as GridView;
			if (e.KeyData != Keys.Tab)
			{
				return;
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "TECHNICIANNAME")
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (_OpenChooseCFL && focusedRowHandle > -1)
				{
					ChooseFromTechnicianList();
				}
			}
			if (gridView != null && gridView.FocusedColumn.FieldName.ToUpper() == "QUEUE" && _OpenChooseCFL && gvDoc.FocusedRowHandle > -1)
			{
				ChooseFromQueueList();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DateTime dateTime = txtFromDate.DateTime;
			if (txtToDate.DateTime < dateTime)
			{
				WinCommons.writeStatus(Enums.MessageCode.ToDateMustGreaterFromDate, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
				txtToDate.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_ServiceCall_FormClosing(object sender, FormClosingEventArgs e)
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
					btnSave_Click(null, null);
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

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public void AddActivityToGrid(DataTable dt)
	{
		try
		{
			if (grdActivities.DataSourceTable != null && grdActivities.DataSourceTable.Rows.Count > 0)
			{
				DataTable dataSourceTable = grdActivities.DataSourceTable;
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					if (Function.ParseInt(dt.Rows[0]["ClgCode"]) == Function.ParseInt(dataSourceTable.Rows[i]["ClgCode"]))
					{
						return;
					}
				}
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["ClgCode"] = Function.ParseInt(dt.Rows[0]["ClgCode"]);
				dataRow["CntctDate"] = Function.ParseDateTimes(dt.Rows[0]["CntctDate"]);
				dataRow["CntctTime"] = Function.ToString(dt.Rows[0]["CntctTime"]);
				dataRow["handledby"] = Function.ParseInt(dt.Rows[0]["handledby"]);
				dataRow["ActivityDate"] = Function.ParseDateTimes(dt.Rows[0]["ActivityDate"]);
				dataRow["RecurPat"] = Function.ToString(dt.Rows[0]["RecurPat"]);
				dataRow["Notes"] = Function.ToString(dt.Rows[0]["Notes"]);
				dataRow["Attachment"] = Function.ToString(dt.Rows[0]["Attachment"]);
				dataRow["DocType"] = Function.ToString(dt.Rows[0]["DocType"]);
				dataSourceTable.Rows.Add(dataRow);
				grdActivities.DataSource = dataSourceTable;
				grdActivities.RefreshDataSource();
			}
			else
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("ClgCode", typeof(int));
				dataTable.Columns.Add("CntctDate", typeof(DateTime));
				dataTable.Columns.Add("CntctTime", typeof(string));
				dataTable.Columns.Add("handledby", typeof(int));
				dataTable.Columns.Add("ActivityDate", typeof(DateTime));
				dataTable.Columns.Add("RecurPat", typeof(string));
				dataTable.Columns.Add("Notes", typeof(string));
				dataTable.Columns.Add("Attachment", typeof(string));
				dataTable.Columns.Add("DocType", typeof(string));
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["ClgCode"] = Function.ParseInt(dt.Rows[0]["ClgCode"]);
				dataRow2["CntctDate"] = Function.ParseDateTimes(dt.Rows[0]["CntctDate"]);
				dataRow2["CntctTime"] = Function.ToString(dt.Rows[0]["CntctTime"]);
				dataRow2["handledby"] = Function.ParseInt(dt.Rows[0]["handledby"]);
				dataRow2["ActivityDate"] = Function.ParseDateTimes(dt.Rows[0]["ActivityDate"]);
				dataRow2["RecurPat"] = Function.ToString(dt.Rows[0]["RecurPat"]);
				dataRow2["Notes"] = Function.ToString(dt.Rows[0]["Notes"]);
				dataRow2["Attachment"] = Function.ToString(dt.Rows[0]["Attachment"]);
				dataRow2["DocType"] = Function.ToString(dt.Rows[0]["DocType"]);
				dataTable.Rows.Add(dataRow2);
				grdActivities.DataSource = dataTable;
				grdActivities.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void AddSolutionToGrid(DataTable dt)
	{
		try
		{
			DataTable dataTable;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution"))
			{
				dataTable = httpClientHelper.Get("GetSolutionStatuses");
			}
			if (grdSolutions.DataSourceTable != null && grdSolutions.DataSourceTable.Rows.Count > 0)
			{
				DataTable dataSourceTable = grdSolutions.DataSourceTable;
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					if (Function.ParseInt(dt.Rows[0]["SltCode"]) == Function.ParseInt(dataSourceTable.Rows[i]["SltCode"]))
					{
						return;
					}
				}
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["SltCode"] = Function.ParseInt(dt.Rows[0]["SltCode"]);
				dataRow["Subject"] = Function.ToString(dt.Rows[0]["Subject"]);
				dataRow["DateCreate"] = Function.ParseDateTimes(dt.Rows[0]["DateCreate"]);
				dataRow["Owner"] = Function.ParseInt(dt.Rows[0]["Owner"]);
				dataRow["StatusNum"] = Function.ParseInt(dt.Rows[0]["StatusNum"]);
				DataRow[] array = dataTable.Select("Number = " + Function.ParseInt(dt.Rows[0]["StatusNum"]));
				dataRow["StatusName"] = Function.ToString(array[0]["Name"]);
				dataRow["userSign2"] = Function.ParseInt(dt.Rows[0]["userSign2"]);
				dataSourceTable.Rows.Add(dataRow);
				grdSolutions.DataSource = dataSourceTable;
				grdSolutions.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("SltCode", typeof(int));
				dataTable2.Columns.Add("Subject", typeof(string));
				dataTable2.Columns.Add("DateCreate", typeof(DateTime));
				dataTable2.Columns.Add("Owner", typeof(int));
				dataTable2.Columns.Add("StatusNum", typeof(int));
				dataTable2.Columns.Add("StatusName", typeof(string));
				dataTable2.Columns.Add("userSign2", typeof(int));
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["SltCode"] = Function.ParseInt(dt.Rows[0]["SltCode"]);
				dataRow2["Subject"] = Function.ToString(dt.Rows[0]["Subject"]);
				dataRow2["DateCreate"] = Function.ParseDateTimes(dt.Rows[0]["DateCreate"]);
				dataRow2["Owner"] = Function.ParseInt(dt.Rows[0]["Owner"]);
				dataRow2["StatusNum"] = Function.ParseInt(dt.Rows[0]["StatusNum"]);
				DataRow[] array2 = dataTable.Select("Number = " + Function.ParseInt(dt.Rows[0]["StatusNum"]));
				dataRow2["StatusName"] = Function.ToString(array2[0]["Name"]);
				dataRow2["userSign2"] = Function.ParseInt(dt.Rows[0]["userSign2"]);
				dataTable2.Rows.Add(dataRow2);
				grdSolutions.DataSource = dataTable2;
				grdSolutions.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Busy;
		WinCommons._OpenProcessing();
		try
		{
			await AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (btnSave.Text == Apzon.Commons.Properties.Resources.ButtonAdd || btnSave.Text == Apzon.Commons.Properties.Resources.ButtonUpdate)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					btnSave_Click(null, null);
				}
				else
				{
					FormMode = Enums.FormViewModeType.View;
					txtFromDate.EditValue = null;
					txtToDate.EditValue = null;
					txtPriority.EditValue = null;
					txtCustomer.EditValue = null;
					txtCallStatus.EditValue = null;
					LoadDoc();
				}
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
				txtFromDate.EditValue = null;
				txtToDate.EditValue = null;
				txtPriority.EditValue = null;
				txtCustomer.EditValue = null;
				txtCallStatus.EditValue = null;
				LoadDoc();
			}
			btnSave.Text = Apzon.Commons.Properties.Resources.ButtonOK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDoc()
	{
		_ = 5;
		try
		{
			_dtContact = await GetContactPerson();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("fromDate", typeof(DateTime));
			dataTable.Columns.Add("toDate", typeof(DateTime));
			dataTable.Columns.Add("customer", typeof(string));
			dataTable.Columns.Add("status", typeof(string));
			dataTable.Columns.Add("priority", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			if (!string.IsNullOrEmpty(Function.ToString(txtFromDate.Text)))
			{
				dataRow["fromDate"] = txtFromDate.EditValue;
			}
			if (!string.IsNullOrEmpty(Function.ToString(txtToDate.Text)))
			{
				dataRow["toDate"] = txtToDate.DateTime;
			}
			dataRow["customer"] = txtCustomer.EditValue;
			dataRow["status"] = txtCallStatus.EditValue;
			dataRow["priority"] = txtPriority.EditValue;
			dataTable.Rows.Add(dataRow);
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataSource = await helperServiceCall.PostAsync("GetDoc", dataTable);
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
			if (gvDoc.RowCount > 0)
			{
				await LoadLine(Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.TopRowIndex, "callID")));
				await LoadActivities(Function.ParseInt(Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.TopRowIndex, "callID"))), 0);
				await LoadSolutions(Function.ParseInt(Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.TopRowIndex, "callID"))), 0);
				await ucAttachment1.LoadAttachments(Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.TopRowIndex, "AtcEntry")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLine(int callID)
	{
		_ = 3;
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			await LoadAdressId(Function.ToString(gvDoc.GetFocusedRowCellValue("customer")));
			DataTable dataTable = await helperServiceCall.GetAsync("GetLineGeneral", new { callID });
			grdGeneral.DataSource = dataTable;
			grdScheduling.DataSource = dataTable;
			grdBusinessPartner.DataSource = dataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txtRemarks.Text = Function.ToString(dataTable.Rows[0]["descrption"]);
				txtResolution.Text = Function.ToString(dataTable.Rows[0]["resolution"]);
			}
			grdGeneral.RefreshDataSource();
			grdScheduling.RefreshDataSource();
			grdSolutions.RefreshDataSource();
			grdBusinessPartner.RefreshDataSource();
			dataTable = await helperServiceCall.GetAsync("GetHistory", new { callID });
			grdHistory.DataSource = dataTable;
			grdHistory.RefreshDataSource();
			int absEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AtcEntry"));
			await ucAttachment1.LoadAttachments(absEntry);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadActivities(int callID, int docNum)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = await helperServiceCall.GetAsync("GetActivities", new { callID, docNum });
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataTable.Rows[i]["RecurPat"]);
				string value = "None";
				switch (text)
				{
				case "N":
					value = "None";
					break;
				case "D":
					value = "Daily";
					break;
				case "W":
					value = "Weekly";
					break;
				case "A":
					value = "Annually";
					break;
				}
				dataTable.Rows[i]["RecurPat"] = value;
			}
			grdActivities.DataSource = dataTable;
			grdActivities.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadSolutions(int callID, int sltCode)
	{
		try
		{
			DataTable lstStatus;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Solution"))
			{
				lstStatus = httpClientHelper.Get("GetSolutionStatuses");
			}
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = await helperServiceCall.GetAsync("GetSolutions", new { callID, sltCode });
			if (sltCode != 0 && dataTable != null && dataTable.Rows.Count > 0)
			{
				if (grdSolutions.DataSourceTable != null && grdSolutions.DataSourceTable.Rows.Count > 0)
				{
					DataTable dataSourceTable = grdSolutions.DataSourceTable;
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["SltCode"] = dataTable.Rows[0]["SltCode"];
					dataRow["Subject"] = dataTable.Rows[0]["Subject"];
					dataRow["DateCreate"] = dataTable.Rows[0]["DateCreate"];
					dataRow["Owner"] = dataTable.Rows[0]["Owner"];
					dataRow["StatusNum"] = dataTable.Rows[0]["StatusNum"];
					DataRow[] array = lstStatus.Select("Number = " + Function.ParseInt(dataTable.Rows[0]["StatusNum"]));
					dataRow["StatusName"] = Function.ToString(array[0]["Name"]);
					dataRow["userSign2"] = Function.ParseInt(dataTable.Rows[0]["userSign2"]);
					dataSourceTable.Rows.Add(dataRow);
					grdSolutions.DataSource = dataSourceTable;
					grdSolutions.RefreshDataSource();
				}
				else
				{
					DataTable dataTable2 = new DataTable();
					dataTable2.Columns.Add("SltCode", typeof(int));
					dataTable2.Columns.Add("Subject", typeof(string));
					dataTable2.Columns.Add("DateCreate", typeof(DateTime));
					dataTable2.Columns.Add("Owner", typeof(int));
					dataTable2.Columns.Add("StatusNum", typeof(string));
					dataTable2.Columns.Add("userSign2", typeof(int));
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["SltCode"] = dataTable.Rows[0]["SltCode"];
					dataRow2["Subject"] = dataTable.Rows[0]["Subject"];
					dataRow2["DateCreate"] = dataTable.Rows[0]["DateCreate"];
					dataRow2["Owner"] = dataTable.Rows[0]["Owner"];
					dataRow2["StatusNum"] = dataTable.Rows[0]["StatusNum"];
					DataRow[] array2 = lstStatus.Select("Number = " + Function.ParseInt(dataTable.Rows[0]["StatusNum"]));
					dataRow2["StatusName"] = Function.ToString(array2[0]["Name"]);
					dataRow2["userSign2"] = Function.ParseInt(dataTable.Rows[0]["userSign2"]);
					dataTable2.Rows.Add(dataRow2);
					grdSolutions.DataSource = dataTable2;
					grdSolutions.RefreshDataSource();
				}
			}
			else
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow[] array3 = lstStatus.Select("Number = " + Function.ParseInt(dataTable.Rows[i]["StatusNum"]));
					dataTable.Rows[i]["StatusName"] = Function.ToString(array3[0]["Name"]);
				}
				grdSolutions.DataSource = dataTable;
				grdSolutions.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task FormLoad()
	{
		_ = 13;
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = await helperServiceCall.GetAsync("GetSeries");
			DataRow dataRow = dataTable.NewRow();
			if (dataTable.Rows.Count > 0 && dataRow.Table.Columns.Contains("ID") && dataRow.Table.Columns.Contains("Name"))
			{
				dataRow["ID"] = -1;
				dataRow["Name"] = "Manual";
				dataTable.Rows.Add(dataRow);
			}
			else
			{
				dataTable.Columns.Add("Name", typeof(string));
				dataTable.Columns.Add("ID", typeof(int));
				dataRow = dataTable.NewRow();
				dataRow["ID"] = -1;
				dataRow["Name"] = "Manual";
				dataTable.Rows.Add(dataRow);
			}
			rNo.DataSource = dataTable;
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rCallStatus;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetCallStatus");
			List<Common.Table_ID> dataSource = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					ID = "L",
					Name = "Low"
				},
				new Common.Table_ID
				{
					ID = "M",
					Name = "Medium"
				},
				new Common.Table_ID
				{
					ID = "H",
					Name = "High"
				}
			};
			rPriority.DataSource = dataSource;
			txtPriority.Properties.DataSource = dataSource;
			repositoryItemLookUpEdit = rOrigin;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetOrigin");
			repositoryItemLookUpEdit = rProblemType;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetProblemType");
			repositoryItemLookUpEdit = rProblemSubtype;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetProblemSubType");
			repositoryItemLookUpEdit = rCallType;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetCallType");
			repositoryItemLookUpEdit = rHandledByInGeneral;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetAssignee");
			repositoryItemLookUpEdit = rMeetingLocation;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetLocation");
			repositoryItemLookUpEdit = rCountry;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetCountries");
			repositoryItemLookUpEdit = rItemGroup;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetListItemGroup");
			List<Common.Table_ID> dataSource2 = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					ID = "M",
					Name = "Minutes"
				},
				new Common.Table_ID
				{
					ID = "H",
					Name = "Hours"
				},
				new Common.Table_ID
				{
					ID = "D",
					Name = "Day"
				}
			};
			rDurType.DataSource = dataSource2;
			List<Common.Table_ID> dataSource3 = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					ID = "M",
					Name = "Minutes"
				},
				new Common.Table_ID
				{
					ID = "H",
					Name = "Hours"
				},
				new Common.Table_ID
				{
					ID = "D",
					Name = "Day"
				}
			};
			rRemType.DataSource = dataSource3;
			List<Common.Table_ID> dataSource4 = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					ID = "0",
					Name = "No Contract"
				}
			};
			rContractNo.DataSource = dataSource4;
			repositoryItemLookUpEdit = txtCallStatus.Properties;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetCallStatus");
			repositoryItemLookUpEdit = rHandledby;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetAssignee");
			repositoryItemLookUpEdit = rOwner;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetAssignee");
			repositoryItemLookUpEdit = rUpdatedbyInHistory;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetAssignee");
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

	private async Task GetDataStateInScheduling(string countryCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rState;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetState", new { countryCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetDocNum(int series)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = await helperServiceCall.GetAsync("GetDocNum", new { series });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["DocNum"]) + 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 1;
	}

	public int GetCallId()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = httpClientHelper.Get("GetCallID");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["CallID"]) + 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 1;
	}

	private async Task AddNew()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			DataTable docNew = new DataTable
			{
				Columns = 
				{
					{
						"callID",
						typeof(int)
					},
					{
						"customer",
						typeof(string)
					},
					{
						"custmrName",
						typeof(string)
					},
					{
						"contctCode",
						typeof(int)
					},
					{
						"TelephoneNo",
						typeof(string)
					},
					{
						"NumAtCard",
						typeof(string)
					},
					{
						"manufSN",
						typeof(string)
					},
					{
						"internalSN",
						typeof(string)
					},
					{
						"itemCode",
						typeof(string)
					},
					{
						"itemName",
						typeof(string)
					},
					{
						"itemGroup",
						typeof(string)
					},
					{
						"Series",
						typeof(int)
					},
					{
						"DocNum",
						typeof(int)
					},
					{
						"status",
						typeof(string)
					},
					{
						"priority",
						typeof(string)
					},
					{
						"createDate",
						typeof(DateTime)
					},
					{
						"createTime",
						typeof(string)
					},
					{
						"closeDate",
						typeof(DateTime)
					},
					{
						"closeTime",
						typeof(string)
					},
					{
						"contractID",
						typeof(string)
					},
					{
						"cntrctDate",
						typeof(DateTime)
					},
					{
						"subject",
						typeof(string)
					},
					{
						"AtcEntry",
						typeof(int)
					}
				}
			};
			DataRow rowDoc = docNew.NewRow();
			rowDoc["callID"] = GetCallId();
			if (rNo.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				rowDoc["Series"] = dataTable.Rows[0]["ID"];
			}
			DataRow dataRow = rowDoc;
			dataRow["DocNum"] = await GetDocNum(Function.ParseInt(rowDoc["Series"]));
			if (rCallStatus.DataSource is DataTable dataTable2 && dataTable2.Rows.Count > 0)
			{
				rowDoc["status"] = -3;
			}
			rowDoc["contractID"] = "0";
			rowDoc["priority"] = "L";
			rowDoc["createDate"] = DateTime.Now.Date;
			if (DateTime.Now.Minute < 10)
			{
				rowDoc["createTime"] = DateTime.Now.Hour + ":0" + DateTime.Now.Minute;
			}
			else
			{
				rowDoc["createTime"] = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
			}
			if (DateTime.Now.Hour < 10)
			{
				rowDoc["createTime"] = "0" + rowDoc["createTime"];
			}
			rowDoc["AtcEntry"] = Function.ParseInt(GetNextFileEntry());
			docNew.Rows.Add(rowDoc);
			grdDoc.DataSource = docNew;
			grdDoc.RefreshDataSource();
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("origin", typeof(string));
			dataTable3.Columns.Add("problemTyp", typeof(string));
			dataTable3.Columns.Add("ProSubType", typeof(string));
			dataTable3.Columns.Add("callType", typeof(string));
			dataTable3.Columns.Add("isQueue", typeof(string));
			dataTable3.Columns.Add("assignee", typeof(string));
			dataTable3.Columns.Add("Queue", typeof(string));
			dataTable3.Columns.Add("respByDate", typeof(DateTime));
			dataTable3.Columns.Add("respByTime", typeof(string));
			dataTable3.Columns.Add("respOnDate", typeof(DateTime));
			dataTable3.Columns.Add("respOnTime", typeof(string));
			dataTable3.Columns.Add("resolDate", typeof(DateTime));
			dataTable3.Columns.Add("resolTime", typeof(string));
			dataTable3.Columns.Add("resolOnDat", typeof(DateTime));
			dataTable3.Columns.Add("resolOnTim", typeof(string));
			dataTable3.Columns.Add("descrption", typeof(string));
			dataTable3.Columns.Add("resolution", typeof(string));
			dataTable3.Columns.Add("StartDate", typeof(DateTime));
			dataTable3.Columns.Add("StartTime", typeof(string));
			dataTable3.Columns.Add("EndDate", typeof(DateTime));
			dataTable3.Columns.Add("EndTime", typeof(string));
			dataTable3.Columns.Add("Duration", typeof(string));
			dataTable3.Columns.Add("technician", typeof(int));
			dataTable3.Columns.Add("technicianName", typeof(string));
			dataTable3.Columns.Add("DurType", typeof(string));
			dataTable3.Columns.Add("Reminder", typeof(string));
			dataTable3.Columns.Add("DisplInCal", typeof(string));
			dataTable3.Columns.Add("RemQty", typeof(string));
			dataTable3.Columns.Add("RemType", typeof(string));
			dataTable3.Columns.Add("Location", typeof(string));
			dataTable3.Columns.Add("AddrName", typeof(string));
			dataTable3.Columns.Add("AddrType", typeof(string));
			dataTable3.Columns.Add("Street", typeof(string));
			dataTable3.Columns.Add("City", typeof(string));
			dataTable3.Columns.Add("Room", typeof(string));
			dataTable3.Columns.Add("State", typeof(string));
			dataTable3.Columns.Add("Country", typeof(string));
			dataTable3.Columns.Add("BPShipCode", typeof(string));
			dataTable3.Columns.Add("BPShipAddr", typeof(string));
			dataTable3.Columns.Add("BPBillCode", typeof(string));
			dataTable3.Columns.Add("BPBillAddr", typeof(string));
			dataTable3.Columns.Add("BPTerrit", typeof(int));
			dataTable3.Columns.Add("BPContact", typeof(string));
			dataTable3.Columns.Add("BPPhone1", typeof(string));
			dataTable3.Columns.Add("BPPhone2", typeof(string));
			dataTable3.Columns.Add("BPCellular", typeof(string));
			dataTable3.Columns.Add("BPFax", typeof(string));
			dataTable3.Columns.Add("BPE_Mail", typeof(string));
			dataTable3.Columns.Add("BPProjCode", typeof(string));
			DataRow dataRow2 = dataTable3.NewRow();
			if (dataRow2.Table.Columns.Contains("isQueue"))
			{
				dataRow2["isQueue"] = "N";
			}
			DataTable dataTable4 = rHandledByInGeneral.DataSource as DataTable;
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("assignee"))
			{
				dataRow2["assignee"] = dataTable4.Rows[0]["ID"];
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("Duration"))
			{
				dataRow2["Duration"] = 15;
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("DurType"))
			{
				dataRow2["DurType"] = "M";
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("StartDate"))
			{
				dataRow2["StartDate"] = DateTime.Now.Date;
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("StartTime"))
			{
				if (DateTime.Now.Minute < 10)
				{
					dataRow2["StartTime"] = DateTime.Now.Hour + ":0" + DateTime.Now.Minute;
				}
				else
				{
					dataRow2["StartTime"] = DateTime.Now.Hour + ":" + DateTime.Now.Minute;
				}
				if (DateTime.Now.Hour < 10)
				{
					dataRow2["StartTime"] = "0" + dataRow2["StartTime"];
				}
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("EndTime"))
			{
				DateTime dateTime = Convert.ToDateTime(Function.ToString(dataRow2["StartTime"]));
				DateTime now = DateTime.Now;
				int num = dateTime.Minute + 15;
				int num2 = dateTime.Hour;
				int num3 = now.Day;
				int num4 = now.Month;
				int num5 = now.Year;
				if (num > 59)
				{
					num -= 60;
					num2++;
				}
				if (num2 > 23)
				{
					num2 -= 24;
					num3++;
				}
				if (num3 > 30)
				{
					num3 -= 30;
					num4++;
				}
				if (num4 > 12)
				{
					num4 -= 12;
					num5++;
				}
				if (num < 10)
				{
					dataRow2["EndTime"] = num2 + ":0" + num;
				}
				else
				{
					dataRow2["EndTime"] = num2 + ":" + num;
				}
				if (num2 < 10)
				{
					dataRow2["EndTime"] = "0" + dataRow2["EndTime"];
				}
				if (num4 == 2 && num3 == 30)
				{
					num4++;
					num3 = 1;
				}
				dataRow2["EndDate"] = new DateTime(num5, num4, num3);
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("RemQty"))
			{
				dataRow2["RemQty"] = 15;
			}
			if (dataTable4 != null && dataTable4.Rows.Count > 0 && dataRow2.Table.Columns.Contains("RemType"))
			{
				dataRow2["RemType"] = "M";
			}
			dataTable3.Rows.Add(dataRow2);
			grdGeneral.DataSource = dataTable3;
			grdScheduling.DataSource = dataTable3;
			grdBusinessPartner.DataSource = dataTable3;
			grdGeneral.RefreshDataSource();
			grdScheduling.RefreshDataSource();
			grdBusinessPartner.RefreshDataSource();
			txtRemarks.Text = "";
			txtResolution.Text = "";
			if (grdActivities.DataSourceTable != null)
			{
				grdActivities.DataSourceTable.Clear();
			}
			if (grdExpenses.DataSourceTable != null)
			{
				grdExpenses.DataSourceTable.Clear();
			}
			if (grdHistory.DataSourceTable != null)
			{
				grdHistory.DataSourceTable.Clear();
			}
			if (grdSolutions.DataSourceTable != null)
			{
				grdSolutions.DataSourceTable.Clear();
			}
			if (ucAttachment1.grd_File.DataSourceTable != null)
			{
				ucAttachment1.grd_File.DataSourceTable.Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> Save(Tuple<DataSet, List<AttachmentFile_Impl>> data)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			return await helperServiceCall.PostParamsAsJsonAsync("Save", data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private void LoadExpenses()
	{
		grdExpenses.DataSource = null;
		grdExpenses.RefreshDataSource();
	}

	private async void SendMessage(string user, string message)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Slack");
			await helperServiceCall.GetStringAsync("SendMessage", new { user, message });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadAdressId(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rAddressID;
			repositoryItemLookUpEdit.DataSource = await helperServiceCall.GetAsync("GetAdressID", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadStreet(string cardCode, string adressId)
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = await helperServiceCall.GetAsync("GetStreet", new
			{
				cardCode = cardCode,
				adressID = adressId
			});
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			gvScheduling.SetFocusedRowCellValue("Street", Function.ToString(dataTable.Rows[0]["Street"]));
			gvScheduling.SetFocusedRowCellValue("City", Function.ToString(dataTable.Rows[0]["City"]));
			gvScheduling.SetFocusedRowCellValue("Country", Function.ToString(dataTable.Rows[0]["Country"]));
			await GetDataStateInScheduling(Function.ToString(dataTable.Rows[0]["Country"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> GetAddrType(string addressId)
	{
		try
		{
			using HttpClientHelper<DataTable> helperServiceCall = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ServiceCall");
			DataTable dataTable = await helperServiceCall.GetAsync("GetAddrType", new
			{
				addressID = addressId
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0]["type"]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btnCancel.Enabled = isEnable;
			btnActivities.Enabled = isEnable;
			ucAttachment1.EnableButtons(isEnable);
			btnSave.Enabled = isEnable;
			btnRecommend.Enabled = isEnable;
			btnSearch.Enabled = isEnable;
			txtCallStatus.ReadOnly = !isEnable;
			txtCustomer.ReadOnly = !isEnable;
			txtFromDate.ReadOnly = !isEnable;
			txtPriority.ReadOnly = !isEnable;
			txtRemarks.ReadOnly = !isEnable;
			txtResolution.ReadOnly = !isEnable;
			txtToDate.ReadOnly = !isEnable;
			gvDoc.ReadOnly = !isEnable;
			gvActivities.ReadOnly = !isEnable;
			ucAttachment1.gv_File.ReadOnly = !isEnable;
			gvExpenses.ReadOnly = !isEnable;
			gvGeneral.ReadOnly = !isEnable;
			gvHistory.ReadOnly = !isEnable;
			gvScheduling.ReadOnly = !isEnable;
			gvSolutions.ReadOnly = !isEnable;
			contextMenuActivities.Enabled = isEnable;
			contextMenuDoc.Enabled = isEnable;
			contextMenuSolution.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetNextFileEntry()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IssueForProduction");
			return httpClientHelper.GetInt("GetMaxFileEntry") + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return -1;
		}
	}

	private async void ChooseFormListCardCode(string cardCode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Customer, false, cardCode, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("customer", chooseItem["CardCode"]);
				gvDoc.SetFocusedRowCellValue("custmrName", chooseItem["CardName"]);
				DataTable dataTable = await GetContactPerson(Function.ToString(chooseItem["CardCode"]));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					rContactPerson.DataSource = dataTable;
					chooseItem = dataTable.Rows[0];
					gvDoc.SetFocusedRowCellValue("contctCode", chooseItem["CntctCode"]);
					gvDoc.SetFocusedRowCellValue("TelephoneNo", chooseItem["Tel1"]);
					gvDoc.SetFocusedRowCellValue("E_MaiL", chooseItem["E_MaiL"]);
				}
			}
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFormListItemCode(string itemCode)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, itemCode, "O", Enums.ItemObjectType.SellItem, null, "");
			frm_CFL2.ListMandatoryField = "ItemName,ItmsGrpCod";
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				if (chooseItem.Table.Columns.Contains("ItemCode"))
				{
					gvDoc.SetFocusedRowCellValue("itemCode", Function.ToString(chooseItem["ItemCode"]));
				}
				if (chooseItem.Table.Columns.Contains("ItemName"))
				{
					gvDoc.SetFocusedRowCellValue("itemName", Function.ToString(chooseItem["ItemName"]));
				}
				if (chooseItem.Table.Columns.Contains("ItmsGrpCod"))
				{
					gvDoc.SetFocusedRowCellValue("itemGroup", Function.ParseInt(chooseItem["ItmsGrpCod"]));
				}
			}
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFromTechnicianList()
	{
		try
		{
			Function.ToString(gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, gcTechnician));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Employees, false, string.Empty, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "technician");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvGeneral.SetFocusedRowCellValue("technician", Function.ParseInt(chooseItem["empID"]));
				gvGeneral.SetFocusedRowCellValue("technicianName", Function.ToString(chooseItem["lastName"]) + " , " + Function.ToString(chooseItem["firstName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFromQueueList()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Employees, false, string.Empty, "Apz_Cfl_GetQueue", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvGeneral.SetFocusedRowCellValue("Queue", Function.ToString(chooseItem["queueID"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFromBpList()
	{
		try
		{
			BusinessPartnersImpl businessPartnersImpl = new BusinessPartnersImpl();
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bp, multiSelect: false, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItem;
			}
			if (businessPartnersImpl != null && !string.IsNullOrEmpty(businessPartnersImpl.CardCode))
			{
				gvBusinessPartner.SetFocusedRowCellValue("BPProjCode", businessPartnersImpl.CardCode);
			}
			frm_ChooseFromList2.Close();
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
			string itemId = Function.ToString(gvDoc.GetFocusedRowCellValue("customer"));
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("itemCode"));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListOfInstallBase, false, "", "", Enums.ItemObjectType.NoItemType, null, itemId, text);
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				if (chooseItem.Table.IsExistsColumn("manufSN"))
				{
					gvDoc.SetFocusedRowCellValue("manufSN", chooseItem["manufSN"]);
				}
				if (chooseItem.Table.IsExistsColumn("internalSN"))
				{
					gvDoc.SetFocusedRowCellValue("internalSN", chooseItem["internalSN"]);
				}
				if (chooseItem.Table.IsExistsColumn("customer"))
				{
					gvDoc.SetFocusedRowCellValue("customer", chooseItem["customer"]);
				}
				if (chooseItem.Table.IsExistsColumn("custmrName"))
				{
					gvDoc.SetFocusedRowCellValue("custmrName", chooseItem["custmrName"]);
				}
				if (chooseItem.Table.IsExistsColumn("itemCode"))
				{
					gvDoc.SetFocusedRowCellValue("itemCode", chooseItem["itemCode"]);
				}
				if (chooseItem.Table.IsExistsColumn("itemName"))
				{
					gvDoc.SetFocusedRowCellValue("itemName", chooseItem["itemName"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetViewUpdate()
	{
		try
		{
			btnSave.Text = _updateString;
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btnSearch.Enabled = true;
		DateEditEx dateEditEx = txtFromDate;
		DateEditEx dateEditEx2 = txtToDate;
		LookUpEdit lookUpEdit = txtPriority;
		ButtonEditEx buttonEditEx = txtCustomer;
		bool flag = (txtCallStatus.ReadOnly = false);
		bool flag3 = (buttonEditEx.ReadOnly = flag);
		bool flag5 = (lookUpEdit.ReadOnly = flag3);
		bool readOnly = (dateEditEx2.ReadOnly = flag5);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.Frm_ServiceCall));
		DevExpress.XtraGrid.GridLevelNode gridLevelNode = new DevExpress.XtraGrid.GridLevelNode();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbCallStatus = new Apzon.Controls.Controls.LabelLine();
		this.lbCustomer = new Apzon.Controls.Controls.LabelLine();
		this.lbToDate = new Apzon.Controls.Controls.LabelLine();
		this.lbPriority = new Apzon.Controls.Controls.LabelLine();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.txtCustomer = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.tabLine = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabGeneral = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdGeneral = new Apzon.Controls.Grid.GridControlEx();
		this.gvGeneral = new Apzon.Controls.Grid.GridViewEx();
		this.gcOrigin = new Apzon.Controls.Grid.GridColumnEx();
		this.rOrigin = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcProblemType = new Apzon.Controls.Grid.GridColumnEx();
		this.rProblemType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcProblemSubtype = new Apzon.Controls.Grid.GridColumnEx();
		this.rProblemSubtype = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCallType = new Apzon.Controls.Grid.GridColumnEx();
		this.rCallType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcTechnician = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTechnicianName = new Apzon.Controls.Grid.GridColumnEx();
		this.rTechnician = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcIsQueue = new Apzon.Controls.Grid.GridColumnEx();
		this.rIsQueue = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcHandledByInGeneral = new Apzon.Controls.Grid.GridColumnEx();
		this.rHandledByInGeneral = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcQueue = new Apzon.Controls.Grid.GridColumnEx();
		this.rQueue = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcResponseByDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDateInGerenal = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcResponseByTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rTimeInGeneral = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.gcResponseOnDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResponseOnTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResolutionByDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResolutionByTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResolutionOnDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResolutionOnTime = new Apzon.Controls.Grid.GridColumnEx();
		this.tabBP = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdBusinessPartner = new Apzon.Controls.Grid.GridControlEx();
		this.gvBusinessPartner = new Apzon.Controls.Grid.GridViewEx();
		this.gcShipCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcShipAddress = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBillCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcBillAddress = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTerritory = new Apzon.Controls.Grid.GridColumnEx();
		this.gcContactPerson = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPhone1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPhone2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMobilePhone = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFax = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEMail = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBPProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rBPProjCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tabRemarks = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtRemarks = new Apzon.Controls.Editor.TextBoxEx();
		this.tabActivities = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdActivities = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuActivities = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvActivities = new Apzon.Controls.Grid.GridViewEx();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcHandledbyInActivities = new Apzon.Controls.Grid.GridColumnEx();
		this.rHandledby = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcActivityDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecurrence = new Apzon.Controls.Grid.GridColumnEx();
		this.gcContent = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMAttachT = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocType = new Apzon.Controls.Grid.GridColumnEx();
		this.tabSolutions = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdSolutions = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuSolution = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gvSolutions = new Apzon.Controls.Grid.GridViewEx();
		this.gcSltCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSolution = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSolutionCreatedOn = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOwner = new Apzon.Controls.Grid.GridColumnEx();
		this.rOwner = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcStatusName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.gcHandledbyInSolutions = new Apzon.Controls.Grid.GridColumnEx();
		this.tabExpenses = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdExpenses = new Apzon.Controls.Grid.GridControlEx();
		this.gvExpenses = new Apzon.Controls.Grid.GridViewEx();
		this.gcDocumentType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocumentNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDateInExpenses = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFromWhse = new Apzon.Controls.Grid.GridColumnEx();
		this.gcToWhse = new Apzon.Controls.Grid.GridColumnEx();
		this.tabResolution = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtResolution = new Apzon.Controls.Editor.TextBoxEx();
		this.tabHistory = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdHistory = new Apzon.Controls.Grid.GridControlEx();
		this.gvHistory = new Apzon.Controls.Grid.GridViewEx();
		this.gcDateofUpdate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUpdateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUpdatedby = new Apzon.Controls.Grid.GridColumnEx();
		this.rUpdatedbyInHistory = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDescriptionInHistory = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPreviousValue = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNewValue = new Apzon.Controls.Grid.GridColumnEx();
		this.tabScheduling = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdScheduling = new Apzon.Controls.Grid.GridControlEx();
		this.gvScheduling = new Apzon.Controls.Grid.GridViewEx();
		this.gcStartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcStartTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rStartTime = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.gcEndDateInScheduling = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEndTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rEndtime = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.gcDuration = new Apzon.Controls.Grid.GridColumnEx();
		this.rDuration = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcDurType = new Apzon.Controls.Grid.GridColumnEx();
		this.rDurType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcReminder = new Apzon.Controls.Grid.GridColumnEx();
		this.rReminder = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcDisplayinCalendar = new Apzon.Controls.Grid.GridColumnEx();
		this.rDisplayinCalendar = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcRemQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rRemQty = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcRemType = new Apzon.Controls.Grid.GridColumnEx();
		this.rRemType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcMeetingLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.rMeetingLocation = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcAddressID = new Apzon.Controls.Grid.GridColumnEx();
		this.rAddressID = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.AddrType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStreet = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRoom = new Apzon.Controls.Grid.GridColumnEx();
		this.gcState = new Apzon.Controls.Grid.GridColumnEx();
		this.rState = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCountry = new Apzon.Controls.Grid.GridColumnEx();
		this.rCountry = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tabAttachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCallID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSubject = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCustomer = new Apzon.Controls.Grid.GridColumnEx();
		this.rCustomer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.rName = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gvContactPerson = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTelephoneNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCustomerRefNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMfrSerialNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rSerialNumber = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcSerialNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItem = new Apzon.Controls.Grid.GridColumnEx();
		this.rItem = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.rDescription = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItemGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rNo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCallStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rCallStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPriority = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriority = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCreatedDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rCreatedDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcCreateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rCreateTime = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
		this.gcClosedDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcClosedTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcContractNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rContractNo = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rContactPerson = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rMfrSerialNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btnActivities = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnRecommend = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbDisplay = new System.Windows.Forms.CheckBox();
		this.txtCallStatus = new DevExpress.XtraEditors.LookUpEdit();
		this.txtPriority = new DevExpress.XtraEditors.LookUpEdit();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtCustomer.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tabLine).BeginInit();
		this.tabLine.SuspendLayout();
		this.tabGeneral.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdGeneral).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvGeneral).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOrigin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProblemType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProblemSubtype).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCallType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTechnician).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIsQueue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rHandledByInGeneral).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rQueue).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDateInGerenal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDateInGerenal.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTimeInGeneral).BeginInit();
		this.tabBP.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdBusinessPartner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvBusinessPartner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBPProjCode).BeginInit();
		this.tabRemarks.SuspendLayout();
		this.tabActivities.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdActivities).BeginInit();
		this.contextMenuActivities.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvActivities).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rHandledby).BeginInit();
		this.tabSolutions.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdSolutions).BeginInit();
		this.contextMenuSolution.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvSolutions).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOwner).BeginInit();
		this.tabExpenses.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdExpenses).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvExpenses).BeginInit();
		this.tabResolution.SuspendLayout();
		this.tabHistory.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdHistory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvHistory).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUpdatedbyInHistory).BeginInit();
		this.tabScheduling.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdScheduling).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvScheduling).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStartTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEndtime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDuration).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDurType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rReminder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDisplayinCalendar).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRemQty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRemType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rMeetingLocation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rAddressID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rState).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCountry).BeginInit();
		this.tabAttachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCustomer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSerialNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItem).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDescription).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCallStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriority).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCreatedDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCreatedDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCreateTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rContractNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rContactPerson).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rMfrSerialNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCallStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPriority.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		base.SuspendLayout();
		this.lbCallStatus.EditControl = null;
		this.lbCallStatus.EditValue = "Call status";
		this.lbCallStatus.FieldName = null;
		this.lbCallStatus.HideWhenEmpty = false;
		this.lbCallStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCallStatus.Location = new System.Drawing.Point(447, 12);
		this.lbCallStatus.Name = "lbCallStatus";
		this.lbCallStatus.ReadOnly = false;
		this.lbCallStatus.SettingInVisible = false;
		this.lbCallStatus.SettingReadonly = false;
		this.lbCallStatus.ShowUnderLine = true;
		this.lbCallStatus.Size = new System.Drawing.Size(90, 20);
		this.lbCallStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCallStatus.TabIndex = 4;
		this.lbCallStatus.Utext = "Call status";
		this.lbCustomer.EditControl = null;
		this.lbCustomer.EditValue = "Customer";
		this.lbCustomer.FieldName = null;
		this.lbCustomer.HideWhenEmpty = false;
		this.lbCustomer.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCustomer.Location = new System.Drawing.Point(235, 36);
		this.lbCustomer.Name = "lbCustomer";
		this.lbCustomer.ReadOnly = false;
		this.lbCustomer.SettingInVisible = false;
		this.lbCustomer.SettingReadonly = false;
		this.lbCustomer.ShowUnderLine = true;
		this.lbCustomer.Size = new System.Drawing.Size(90, 20);
		this.lbCustomer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCustomer.TabIndex = 3;
		this.lbCustomer.Utext = "Customer";
		this.lbToDate.EditControl = null;
		this.lbToDate.EditValue = "To Date";
		this.lbToDate.FieldName = null;
		this.lbToDate.HideWhenEmpty = false;
		this.lbToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToDate.Location = new System.Drawing.Point(27, 36);
		this.lbToDate.Name = "lbToDate";
		this.lbToDate.ReadOnly = false;
		this.lbToDate.SettingInVisible = false;
		this.lbToDate.SettingReadonly = false;
		this.lbToDate.ShowUnderLine = true;
		this.lbToDate.Size = new System.Drawing.Size(80, 20);
		this.lbToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToDate.TabIndex = 2;
		this.lbToDate.Utext = "To Date";
		this.lbPriority.EditControl = null;
		this.lbPriority.EditValue = "Priority";
		this.lbPriority.FieldName = null;
		this.lbPriority.HideWhenEmpty = false;
		this.lbPriority.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbPriority.Location = new System.Drawing.Point(235, 12);
		this.lbPriority.Name = "lbPriority";
		this.lbPriority.ReadOnly = false;
		this.lbPriority.SettingInVisible = false;
		this.lbPriority.SettingReadonly = false;
		this.lbPriority.ShowUnderLine = true;
		this.lbPriority.Size = new System.Drawing.Size(90, 20);
		this.lbPriority.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbPriority.TabIndex = 1;
		this.lbPriority.Utext = "Priority";
		this.lbFromDate.EditControl = null;
		this.lbFromDate.EditValue = "From Date";
		this.lbFromDate.FieldName = null;
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(27, 12);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ReadOnly = false;
		this.lbFromDate.SettingInVisible = false;
		this.lbFromDate.SettingReadonly = false;
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(90, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 0;
		this.lbFromDate.Utext = "From Date";
		this.txtCustomer.EditControl = null;
		this.txtCustomer.FieldName = null;
		this.txtCustomer.IsSearchControl = false;
		this.txtCustomer.Location = new System.Drawing.Point(305, 36);
		this.txtCustomer.Name = "txtCustomer";
		this.txtCustomer.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtCustomer.SettingInVisible = false;
		this.txtCustomer.SettingReadonly = false;
		this.txtCustomer.Size = new System.Drawing.Size(120, 20);
		this.txtCustomer.TabIndex = 8;
		this.txtCustomer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCustomer_ButtonClick);
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnSearch.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.Appearance.Options.UseForeColor = true;
		this.btnSearch.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(447, 34);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(100, 25);
		this.btnSearch.TabIndex = 10;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnSave.Appearance.Options.UseBackColor = true;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Update;
		this.btnSave.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Update";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(12, 562);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(68, 23);
		this.btnSave.TabIndex = 11;
		this.btnSave.Text = "Update";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(87, 562);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 12;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 62);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tabLine);
		this.splitContainerControl1.Panel2.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(920, 493);
		this.splitContainerControl1.SplitterPosition = 0;
		this.splitContainerControl1.TabIndex = 13;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.tabLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabLine.IsSearchControl = false;
		this.tabLine.Location = new System.Drawing.Point(-5, 256);
		this.tabLine.Name = "tabLine";
		this.tabLine.SelectedTabPage = this.tabGeneral;
		this.tabLine.Size = new System.Drawing.Size(920, 237);
		this.tabLine.TabIndex = 1;
		this.tabLine.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[10] { this.tabGeneral, this.tabBP, this.tabRemarks, this.tabActivities, this.tabSolutions, this.tabExpenses, this.tabResolution, this.tabHistory, this.tabScheduling, this.tabAttachments });
		this.tabLine.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tabLine_SelectedPageChanged);
		this.tabGeneral.Controls.Add(this.grdGeneral);
		this.tabGeneral.EditControl = null;
		this.tabGeneral.EditValue = "General";
		this.tabGeneral.FieldName = null;
		this.tabGeneral.IsSearchControl = false;
		this.tabGeneral.Name = "tabGeneral";
		this.tabGeneral.ReadOnly = false;
		this.tabGeneral.SettingInVisible = false;
		this.tabGeneral.SettingReadonly = false;
		this.tabGeneral.Size = new System.Drawing.Size(918, 212);
		this.tabGeneral.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabGeneral.Text = "General";
		this.grdGeneral.AllowPasteExcel = true;
		this.grdGeneral.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdGeneral.ClipboardData = null;
		gridLevelNode.RelationName = "Level1";
		this.grdGeneral.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.grdGeneral.Location = new System.Drawing.Point(2, 0);
		this.grdGeneral.MainView = this.gvGeneral;
		this.grdGeneral.Name = "grdGeneral";
		this.grdGeneral.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[10] { this.rOrigin, this.rProblemType, this.rProblemSubtype, this.rCallType, this.rTechnician, this.rIsQueue, this.rHandledByInGeneral, this.rDateInGerenal, this.rTimeInGeneral, this.rQueue });
		this.grdGeneral.Size = new System.Drawing.Size(914, 206);
		this.grdGeneral.TabIndex = 0;
		this.grdGeneral.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvGeneral });
		this.grdGeneral.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdGeneral_EditorKeyDown);
		this.gvGeneral.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvGeneral.ColumnKey = null;
		this.gvGeneral.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.gcOrigin, this.gcProblemType, this.gcProblemSubtype, this.gcCallType, this.gcTechnician, this.gcTechnicianName, this.gcIsQueue, this.gcHandledByInGeneral, this.gcQueue, this.gcResponseByDate,
			this.gcResponseByTime, this.gcResponseOnDate, this.gcResponseOnTime, this.gcResolutionByDate, this.gcResolutionByTime, this.gcResolutionOnDate, this.gcResolutionOnTime
		});
		this.gvGeneral.DisableFormSetting = false;
		this.gvGeneral.DisplayException = false;
		this.gvGeneral.EditColumn = null;
		this.gvGeneral.FieldCurrency = null;
		this.gvGeneral.FieldRate = null;
		this.gvGeneral.FormId = "oServiceCall";
		this.gvGeneral.GridControl = this.grdGeneral;
		this.gvGeneral.IgnoreFilter = false;
		this.gvGeneral.IndexEditRow = -1;
		this.gvGeneral.IndicatorWidth = 40;
		this.gvGeneral.IsApzTable = false;
		this.gvGeneral.IsInsertNewRow = false;
		this.gvGeneral.IsLoadingData = false;
		this.gvGeneral.IsMappingContextAction = false;
		this.gvGeneral.ItemId = "SCL1";
		this.gvGeneral.LstColumnAllowBatch = null;
		this.gvGeneral.LstColumnNotAllowBatch = null;
		this.gvGeneral.LstColumnNotEditable = null;
		this.gvGeneral.LstColumNotSetting = null;
		this.gvGeneral.Name = "gvGeneral";
		this.gvGeneral.OptionsCustomization.AllowSort = false;
		this.gvGeneral.OptionsView.ColumnAutoWidth = false;
		this.gvGeneral.OptionsView.ShowGroupPanel = false;
		this.gvGeneral.UseBatchMod = false;
		this.gvGeneral.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvGeneral_ShowingEditor);
		this.gvGeneral.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvGeneral_CellValueChanged);
		this.gvGeneral.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvGeneral_CellValueChanging);
		this.gcOrigin.Caption = "Origin";
		this.gcOrigin.ColumnEdit = this.rOrigin;
		this.gcOrigin.CurrencyDateField = null;
		this.gcOrigin.CurrencyField = null;
		this.gcOrigin.CurrencyFixed = null;
		this.gcOrigin.CurrencyFixedRate = 0.0;
		this.gcOrigin.CurrencyRateField = null;
		this.gcOrigin.FieldName = "origin";
		this.gcOrigin.Name = "gcOrigin";
		this.gcOrigin.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcOrigin.PrimaryKey = true;
		this.gcOrigin.UnitField = null;
		this.gcOrigin.Visible = true;
		this.gcOrigin.VisibleIndex = 0;
		this.rOrigin.AutoHeight = false;
		this.rOrigin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOrigin.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name11"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name12", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rOrigin.DisplayMember = "Name";
		this.rOrigin.Name = "rOrigin";
		this.rOrigin.NullText = "";
		this.rOrigin.ShowFooter = false;
		this.rOrigin.ShowHeader = false;
		this.rOrigin.ValueMember = "ID";
		this.gcProblemType.Caption = "Problem Type";
		this.gcProblemType.ColumnEdit = this.rProblemType;
		this.gcProblemType.CurrencyDateField = null;
		this.gcProblemType.CurrencyField = null;
		this.gcProblemType.CurrencyFixed = null;
		this.gcProblemType.CurrencyFixedRate = 0.0;
		this.gcProblemType.CurrencyRateField = null;
		this.gcProblemType.FieldName = "problemTyp";
		this.gcProblemType.Name = "gcProblemType";
		this.gcProblemType.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcProblemType.PrimaryKey = false;
		this.gcProblemType.UnitField = null;
		this.gcProblemType.Visible = true;
		this.gcProblemType.VisibleIndex = 1;
		this.rProblemType.AutoHeight = false;
		this.rProblemType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rProblemType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name13"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name14", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rProblemType.DisplayMember = "Name";
		this.rProblemType.Name = "rProblemType";
		this.rProblemType.NullText = "";
		this.rProblemType.ShowFooter = false;
		this.rProblemType.ShowHeader = false;
		this.rProblemType.ValueMember = "ID";
		this.gcProblemSubtype.Caption = "Problem Subtype";
		this.gcProblemSubtype.ColumnEdit = this.rProblemSubtype;
		this.gcProblemSubtype.CurrencyDateField = null;
		this.gcProblemSubtype.CurrencyField = null;
		this.gcProblemSubtype.CurrencyFixed = null;
		this.gcProblemSubtype.CurrencyFixedRate = 0.0;
		this.gcProblemSubtype.CurrencyRateField = null;
		this.gcProblemSubtype.FieldName = "ProSubType";
		this.gcProblemSubtype.Name = "gcProblemSubtype";
		this.gcProblemSubtype.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcProblemSubtype.PrimaryKey = false;
		this.gcProblemSubtype.UnitField = null;
		this.gcProblemSubtype.Visible = true;
		this.gcProblemSubtype.VisibleIndex = 2;
		this.gcProblemSubtype.Width = 98;
		this.rProblemSubtype.AutoHeight = false;
		this.rProblemSubtype.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rProblemSubtype.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name15"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name16", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rProblemSubtype.DisplayMember = "Name";
		this.rProblemSubtype.Name = "rProblemSubtype";
		this.rProblemSubtype.NullText = "";
		this.rProblemSubtype.ShowFooter = false;
		this.rProblemSubtype.ShowHeader = false;
		this.rProblemSubtype.ValueMember = "ID";
		this.gcCallType.Caption = "Call Type";
		this.gcCallType.ColumnEdit = this.rCallType;
		this.gcCallType.CurrencyDateField = null;
		this.gcCallType.CurrencyField = null;
		this.gcCallType.CurrencyFixed = null;
		this.gcCallType.CurrencyFixedRate = 0.0;
		this.gcCallType.CurrencyRateField = null;
		this.gcCallType.FieldName = "callType";
		this.gcCallType.Name = "gcCallType";
		this.gcCallType.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcCallType.PrimaryKey = false;
		this.gcCallType.UnitField = null;
		this.gcCallType.Visible = true;
		this.gcCallType.VisibleIndex = 3;
		this.gcCallType.Width = 64;
		this.rCallType.AutoHeight = false;
		this.rCallType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCallType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rCallType.DisplayMember = "Name";
		this.rCallType.Name = "rCallType";
		this.rCallType.NullText = "";
		this.rCallType.ShowFooter = false;
		this.rCallType.ShowHeader = false;
		this.rCallType.ValueMember = "ID";
		this.gcTechnician.Caption = "gridColumn1";
		this.gcTechnician.CurrencyDateField = null;
		this.gcTechnician.CurrencyField = null;
		this.gcTechnician.CurrencyFixed = null;
		this.gcTechnician.CurrencyFixedRate = 0.0;
		this.gcTechnician.CurrencyRateField = null;
		this.gcTechnician.FieldName = "technician";
		this.gcTechnician.Name = "gcTechnician";
		this.gcTechnician.OptionsColumn.ReadOnly = true;
		this.gcTechnician.PrimaryKey = false;
		this.gcTechnician.UnitField = null;
		this.gcTechnicianName.Caption = "Technician";
		this.gcTechnicianName.ColumnEdit = this.rTechnician;
		this.gcTechnicianName.CurrencyDateField = null;
		this.gcTechnicianName.CurrencyField = null;
		this.gcTechnicianName.CurrencyFixed = null;
		this.gcTechnicianName.CurrencyFixedRate = 0.0;
		this.gcTechnicianName.CurrencyRateField = null;
		this.gcTechnicianName.FieldName = "technicianName";
		this.gcTechnicianName.Name = "gcTechnicianName";
		this.gcTechnicianName.OptionsColumn.ReadOnly = true;
		this.gcTechnicianName.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcTechnicianName.PrimaryKey = false;
		this.gcTechnicianName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcTechnicianName.UnitField = null;
		this.gcTechnicianName.Visible = true;
		this.gcTechnicianName.VisibleIndex = 4;
		this.rTechnician.AutoHeight = false;
		this.rTechnician.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rTechnician.Name = "rTechnician";
		this.rTechnician.ReadOnly = true;
		this.rTechnician.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rTechnician_ButtonClick);
		this.gcIsQueue.Caption = "Is Queue";
		this.gcIsQueue.ColumnEdit = this.rIsQueue;
		this.gcIsQueue.CurrencyDateField = null;
		this.gcIsQueue.CurrencyField = null;
		this.gcIsQueue.CurrencyFixed = null;
		this.gcIsQueue.CurrencyFixedRate = 0.0;
		this.gcIsQueue.CurrencyRateField = null;
		this.gcIsQueue.FieldName = "isQueue";
		this.gcIsQueue.Name = "gcIsQueue";
		this.gcIsQueue.OptionsFilter.AllowAutoFilter = false;
		this.gcIsQueue.OptionsFilter.AllowFilter = false;
		this.gcIsQueue.PrimaryKey = false;
		this.gcIsQueue.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcIsQueue.UnitField = null;
		this.gcIsQueue.Visible = true;
		this.gcIsQueue.VisibleIndex = 5;
		this.rIsQueue.AutoHeight = false;
		this.rIsQueue.Name = "rIsQueue";
		this.rIsQueue.ValueChecked = "Y";
		this.rIsQueue.ValueUnchecked = "N";
		this.gcHandledByInGeneral.Caption = "Hand&led By";
		this.gcHandledByInGeneral.ColumnEdit = this.rHandledByInGeneral;
		this.gcHandledByInGeneral.CurrencyDateField = null;
		this.gcHandledByInGeneral.CurrencyField = null;
		this.gcHandledByInGeneral.CurrencyFixed = null;
		this.gcHandledByInGeneral.CurrencyFixedRate = 0.0;
		this.gcHandledByInGeneral.CurrencyRateField = null;
		this.gcHandledByInGeneral.FieldName = "assignee";
		this.gcHandledByInGeneral.Name = "gcHandledByInGeneral";
		this.gcHandledByInGeneral.OptionsFilter.AllowAutoFilter = false;
		this.gcHandledByInGeneral.OptionsFilter.AllowFilter = false;
		this.gcHandledByInGeneral.PrimaryKey = false;
		this.gcHandledByInGeneral.UnitField = null;
		this.gcHandledByInGeneral.Visible = true;
		this.gcHandledByInGeneral.VisibleIndex = 6;
		this.rHandledByInGeneral.AutoHeight = false;
		this.rHandledByInGeneral.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rHandledByInGeneral.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rHandledByInGeneral.DisplayMember = "Name";
		this.rHandledByInGeneral.Name = "rHandledByInGeneral";
		this.rHandledByInGeneral.NullText = "";
		this.rHandledByInGeneral.ShowFooter = false;
		this.rHandledByInGeneral.ShowHeader = false;
		this.rHandledByInGeneral.ValueMember = "ID";
		this.gcQueue.Caption = "Queue";
		this.gcQueue.ColumnEdit = this.rQueue;
		this.gcQueue.CurrencyDateField = null;
		this.gcQueue.CurrencyField = null;
		this.gcQueue.CurrencyFixed = null;
		this.gcQueue.CurrencyFixedRate = 0.0;
		this.gcQueue.CurrencyRateField = null;
		this.gcQueue.FieldName = "Queue";
		this.gcQueue.Name = "gcQueue";
		this.gcQueue.OptionsFilter.AllowAutoFilter = false;
		this.gcQueue.OptionsFilter.AllowFilter = false;
		this.gcQueue.PrimaryKey = false;
		this.gcQueue.UnitField = null;
		this.gcQueue.Visible = true;
		this.gcQueue.VisibleIndex = 7;
		this.gcQueue.Width = 49;
		this.rQueue.AutoHeight = false;
		this.rQueue.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rQueue.Name = "rQueue";
		this.rQueue.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rQueue_ButtonClick);
		this.gcResponseByDate.Caption = "Response By Date";
		this.gcResponseByDate.ColumnEdit = this.rDateInGerenal;
		this.gcResponseByDate.CurrencyDateField = null;
		this.gcResponseByDate.CurrencyField = null;
		this.gcResponseByDate.CurrencyFixed = null;
		this.gcResponseByDate.CurrencyFixedRate = 0.0;
		this.gcResponseByDate.CurrencyRateField = null;
		this.gcResponseByDate.DisplayFormat.FormatString = "d";
		this.gcResponseByDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcResponseByDate.FieldName = "respByDate";
		this.gcResponseByDate.Name = "gcResponseByDate";
		this.gcResponseByDate.OptionsColumn.AllowEdit = false;
		this.gcResponseByDate.OptionsFilter.AllowAutoFilter = false;
		this.gcResponseByDate.OptionsFilter.AllowFilter = false;
		this.gcResponseByDate.PrimaryKey = false;
		this.gcResponseByDate.UnitField = null;
		this.gcResponseByDate.Visible = true;
		this.gcResponseByDate.VisibleIndex = 8;
		this.gcResponseByDate.Width = 102;
		this.rDateInGerenal.AutoHeight = false;
		this.rDateInGerenal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDateInGerenal.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDateInGerenal.Name = "rDateInGerenal";
		this.gcResponseByTime.Caption = "Response By Time";
		this.gcResponseByTime.ColumnEdit = this.rTimeInGeneral;
		this.gcResponseByTime.CurrencyDateField = null;
		this.gcResponseByTime.CurrencyField = null;
		this.gcResponseByTime.CurrencyFixed = null;
		this.gcResponseByTime.CurrencyFixedRate = 0.0;
		this.gcResponseByTime.CurrencyRateField = null;
		this.gcResponseByTime.DisplayFormat.FormatString = "g";
		this.gcResponseByTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcResponseByTime.FieldName = "respByTime";
		this.gcResponseByTime.Name = "gcResponseByTime";
		this.gcResponseByTime.OptionsColumn.AllowEdit = false;
		this.gcResponseByTime.OptionsFilter.AllowAutoFilter = false;
		this.gcResponseByTime.OptionsFilter.AllowFilter = false;
		this.gcResponseByTime.PrimaryKey = false;
		this.gcResponseByTime.UnitField = null;
		this.gcResponseByTime.Visible = true;
		this.gcResponseByTime.VisibleIndex = 9;
		this.gcResponseByTime.Width = 100;
		this.rTimeInGeneral.AutoHeight = false;
		this.rTimeInGeneral.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rTimeInGeneral.Name = "rTimeInGeneral";
		this.gcResponseOnDate.Caption = "Response On Date";
		this.gcResponseOnDate.ColumnEdit = this.rDateInGerenal;
		this.gcResponseOnDate.CurrencyDateField = null;
		this.gcResponseOnDate.CurrencyField = null;
		this.gcResponseOnDate.CurrencyFixed = null;
		this.gcResponseOnDate.CurrencyFixedRate = 0.0;
		this.gcResponseOnDate.CurrencyRateField = null;
		this.gcResponseOnDate.DisplayFormat.FormatString = "d";
		this.gcResponseOnDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcResponseOnDate.FieldName = "respOnDate";
		this.gcResponseOnDate.Name = "gcResponseOnDate";
		this.gcResponseOnDate.OptionsColumn.AllowEdit = false;
		this.gcResponseOnDate.OptionsFilter.AllowAutoFilter = false;
		this.gcResponseOnDate.OptionsFilter.AllowFilter = false;
		this.gcResponseOnDate.PrimaryKey = false;
		this.gcResponseOnDate.UnitField = null;
		this.gcResponseOnDate.Visible = true;
		this.gcResponseOnDate.VisibleIndex = 10;
		this.gcResponseOnDate.Width = 104;
		this.gcResponseOnTime.Caption = "ResponseOnTime";
		this.gcResponseOnTime.ColumnEdit = this.rTimeInGeneral;
		this.gcResponseOnTime.CurrencyDateField = null;
		this.gcResponseOnTime.CurrencyField = null;
		this.gcResponseOnTime.CurrencyFixed = null;
		this.gcResponseOnTime.CurrencyFixedRate = 0.0;
		this.gcResponseOnTime.CurrencyRateField = null;
		this.gcResponseOnTime.DisplayFormat.FormatString = "g";
		this.gcResponseOnTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcResponseOnTime.FieldName = "respOnTime";
		this.gcResponseOnTime.Name = "gcResponseOnTime";
		this.gcResponseOnTime.OptionsColumn.AllowEdit = false;
		this.gcResponseOnTime.OptionsFilter.AllowAutoFilter = false;
		this.gcResponseOnTime.OptionsFilter.AllowFilter = false;
		this.gcResponseOnTime.PrimaryKey = false;
		this.gcResponseOnTime.UnitField = null;
		this.gcResponseOnTime.Visible = true;
		this.gcResponseOnTime.VisibleIndex = 11;
		this.gcResponseOnTime.Width = 96;
		this.gcResolutionByDate.Caption = "Resolution By Date";
		this.gcResolutionByDate.ColumnEdit = this.rDateInGerenal;
		this.gcResolutionByDate.CurrencyDateField = null;
		this.gcResolutionByDate.CurrencyField = null;
		this.gcResolutionByDate.CurrencyFixed = null;
		this.gcResolutionByDate.CurrencyFixedRate = 0.0;
		this.gcResolutionByDate.CurrencyRateField = null;
		this.gcResolutionByDate.DisplayFormat.FormatString = "d";
		this.gcResolutionByDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcResolutionByDate.FieldName = "resolDate";
		this.gcResolutionByDate.Name = "gcResolutionByDate";
		this.gcResolutionByDate.OptionsColumn.AllowEdit = false;
		this.gcResolutionByDate.OptionsFilter.AllowAutoFilter = false;
		this.gcResolutionByDate.OptionsFilter.AllowFilter = false;
		this.gcResolutionByDate.PrimaryKey = false;
		this.gcResolutionByDate.UnitField = null;
		this.gcResolutionByDate.Visible = true;
		this.gcResolutionByDate.VisibleIndex = 12;
		this.gcResolutionByDate.Width = 99;
		this.gcResolutionByTime.Caption = "Resolution By Time";
		this.gcResolutionByTime.ColumnEdit = this.rTimeInGeneral;
		this.gcResolutionByTime.CurrencyDateField = null;
		this.gcResolutionByTime.CurrencyField = null;
		this.gcResolutionByTime.CurrencyFixed = null;
		this.gcResolutionByTime.CurrencyFixedRate = 0.0;
		this.gcResolutionByTime.CurrencyRateField = null;
		this.gcResolutionByTime.FieldName = "resolTime";
		this.gcResolutionByTime.Name = "gcResolutionByTime";
		this.gcResolutionByTime.OptionsColumn.AllowEdit = false;
		this.gcResolutionByTime.OptionsFilter.AllowAutoFilter = false;
		this.gcResolutionByTime.OptionsFilter.AllowFilter = false;
		this.gcResolutionByTime.PrimaryKey = false;
		this.gcResolutionByTime.UnitField = null;
		this.gcResolutionByTime.Visible = true;
		this.gcResolutionByTime.VisibleIndex = 13;
		this.gcResolutionByTime.Width = 107;
		this.gcResolutionOnDate.Caption = "Resolution On Date";
		this.gcResolutionOnDate.ColumnEdit = this.rDateInGerenal;
		this.gcResolutionOnDate.CurrencyDateField = null;
		this.gcResolutionOnDate.CurrencyField = null;
		this.gcResolutionOnDate.CurrencyFixed = null;
		this.gcResolutionOnDate.CurrencyFixedRate = 0.0;
		this.gcResolutionOnDate.CurrencyRateField = null;
		this.gcResolutionOnDate.DisplayFormat.FormatString = "d";
		this.gcResolutionOnDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcResolutionOnDate.FieldName = "resolOnDat";
		this.gcResolutionOnDate.Name = "gcResolutionOnDate";
		this.gcResolutionOnDate.OptionsColumn.AllowEdit = false;
		this.gcResolutionOnDate.OptionsFilter.AllowAutoFilter = false;
		this.gcResolutionOnDate.OptionsFilter.AllowFilter = false;
		this.gcResolutionOnDate.PrimaryKey = false;
		this.gcResolutionOnDate.UnitField = null;
		this.gcResolutionOnDate.Visible = true;
		this.gcResolutionOnDate.VisibleIndex = 14;
		this.gcResolutionOnDate.Width = 114;
		this.gcResolutionOnTime.Caption = "Resolution On Time";
		this.gcResolutionOnTime.ColumnEdit = this.rTimeInGeneral;
		this.gcResolutionOnTime.CurrencyDateField = null;
		this.gcResolutionOnTime.CurrencyField = null;
		this.gcResolutionOnTime.CurrencyFixed = null;
		this.gcResolutionOnTime.CurrencyFixedRate = 0.0;
		this.gcResolutionOnTime.CurrencyRateField = null;
		this.gcResolutionOnTime.FieldName = "resolOnTim";
		this.gcResolutionOnTime.Name = "gcResolutionOnTime";
		this.gcResolutionOnTime.OptionsColumn.AllowEdit = false;
		this.gcResolutionOnTime.OptionsFilter.AllowAutoFilter = false;
		this.gcResolutionOnTime.OptionsFilter.AllowFilter = false;
		this.gcResolutionOnTime.PrimaryKey = false;
		this.gcResolutionOnTime.UnitField = null;
		this.gcResolutionOnTime.Visible = true;
		this.gcResolutionOnTime.VisibleIndex = 15;
		this.gcResolutionOnTime.Width = 90;
		this.tabBP.Controls.Add(this.grdBusinessPartner);
		this.tabBP.EditControl = null;
		this.tabBP.EditValue = "Business Partner";
		this.tabBP.FieldName = null;
		this.tabBP.IsSearchControl = false;
		this.tabBP.Name = "tabBP";
		this.tabBP.ReadOnly = false;
		this.tabBP.SettingInVisible = false;
		this.tabBP.SettingReadonly = false;
		this.tabBP.Size = new System.Drawing.Size(918, 212);
		this.tabBP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabBP.Text = "Business Partner";
		this.grdBusinessPartner.AllowPasteExcel = false;
		this.grdBusinessPartner.ClipboardData = null;
		this.grdBusinessPartner.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdBusinessPartner.Location = new System.Drawing.Point(0, 0);
		this.grdBusinessPartner.MainView = this.gvBusinessPartner;
		this.grdBusinessPartner.Name = "grdBusinessPartner";
		this.grdBusinessPartner.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemLookUpEdit1, this.repositoryItemLookUpEdit2, this.rBPProjCode });
		this.grdBusinessPartner.Size = new System.Drawing.Size(918, 212);
		this.grdBusinessPartner.TabIndex = 0;
		this.grdBusinessPartner.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvBusinessPartner });
		this.grdBusinessPartner.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdBusinessPartner_EditorKeyDown);
		this.gvBusinessPartner.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvBusinessPartner.ColumnKey = null;
		this.gvBusinessPartner.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gcShipCode, this.gcShipAddress, this.gcBillCode, this.gcBillAddress, this.gcTerritory, this.gcContactPerson, this.gcPhone1, this.gcPhone2, this.gcMobilePhone, this.gcFax,
			this.gcEMail, this.gcBPProject
		});
		this.gvBusinessPartner.DisableFormSetting = false;
		this.gvBusinessPartner.DisplayException = false;
		this.gvBusinessPartner.EditColumn = null;
		this.gvBusinessPartner.FieldCurrency = null;
		this.gvBusinessPartner.FieldRate = null;
		this.gvBusinessPartner.FormId = "oServiceCall";
		this.gvBusinessPartner.GridControl = this.grdBusinessPartner;
		this.gvBusinessPartner.IgnoreFilter = false;
		this.gvBusinessPartner.IndexEditRow = -1;
		this.gvBusinessPartner.IndicatorWidth = 40;
		this.gvBusinessPartner.IsApzTable = false;
		this.gvBusinessPartner.IsInsertNewRow = false;
		this.gvBusinessPartner.IsLoadingData = false;
		this.gvBusinessPartner.IsMappingContextAction = false;
		this.gvBusinessPartner.ItemId = "OCRD";
		this.gvBusinessPartner.LstColumnAllowBatch = null;
		this.gvBusinessPartner.LstColumnNotAllowBatch = null;
		this.gvBusinessPartner.LstColumnNotEditable = null;
		this.gvBusinessPartner.LstColumNotSetting = null;
		this.gvBusinessPartner.Name = "gvBusinessPartner";
		this.gvBusinessPartner.OptionsView.ShowGroupPanel = false;
		this.gvBusinessPartner.UseBatchMod = false;
		this.gvBusinessPartner.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBusinessPartner_CellValueChanged);
		this.gvBusinessPartner.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvBusinessPartner_CellValueChanging);
		this.gcShipCode.Caption = "Ship To";
		this.gcShipCode.ColumnEdit = this.repositoryItemLookUpEdit1;
		this.gcShipCode.CurrencyDateField = null;
		this.gcShipCode.CurrencyField = null;
		this.gcShipCode.CurrencyFixed = null;
		this.gcShipCode.CurrencyFixedRate = 0.0;
		this.gcShipCode.CurrencyRateField = null;
		this.gcShipCode.FieldName = "BPShipCode";
		this.gcShipCode.Name = "gcShipCode";
		this.gcShipCode.PrimaryKey = false;
		this.gcShipCode.UnitField = null;
		this.gcShipCode.Visible = true;
		this.gcShipCode.VisibleIndex = 0;
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.repositoryItemLookUpEdit1.NullText = "";
		this.gcShipAddress.Caption = "Ship Address";
		this.gcShipAddress.CurrencyDateField = null;
		this.gcShipAddress.CurrencyField = null;
		this.gcShipAddress.CurrencyFixed = null;
		this.gcShipAddress.CurrencyFixedRate = 0.0;
		this.gcShipAddress.CurrencyRateField = null;
		this.gcShipAddress.FieldName = "BPShipAddr";
		this.gcShipAddress.Name = "gcShipAddress";
		this.gcShipAddress.PrimaryKey = false;
		this.gcShipAddress.UnitField = null;
		this.gcShipAddress.Visible = true;
		this.gcShipAddress.VisibleIndex = 1;
		this.gcBillCode.Caption = "Bill To";
		this.gcBillCode.ColumnEdit = this.repositoryItemLookUpEdit2;
		this.gcBillCode.CurrencyDateField = null;
		this.gcBillCode.CurrencyField = null;
		this.gcBillCode.CurrencyFixed = null;
		this.gcBillCode.CurrencyFixedRate = 0.0;
		this.gcBillCode.CurrencyRateField = null;
		this.gcBillCode.FieldName = "BPBillCode";
		this.gcBillCode.Name = "gcBillCode";
		this.gcBillCode.PrimaryKey = false;
		this.gcBillCode.UnitField = null;
		this.gcBillCode.Visible = true;
		this.gcBillCode.VisibleIndex = 2;
		this.repositoryItemLookUpEdit2.AutoHeight = false;
		this.repositoryItemLookUpEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit2.Name = "repositoryItemLookUpEdit2";
		this.repositoryItemLookUpEdit2.NullText = "";
		this.gcBillAddress.Caption = "Bill Address";
		this.gcBillAddress.CurrencyDateField = null;
		this.gcBillAddress.CurrencyField = null;
		this.gcBillAddress.CurrencyFixed = null;
		this.gcBillAddress.CurrencyFixedRate = 0.0;
		this.gcBillAddress.CurrencyRateField = null;
		this.gcBillAddress.FieldName = "BPBillAddr";
		this.gcBillAddress.Name = "gcBillAddress";
		this.gcBillAddress.PrimaryKey = false;
		this.gcBillAddress.UnitField = null;
		this.gcBillAddress.Visible = true;
		this.gcBillAddress.VisibleIndex = 3;
		this.gcTerritory.Caption = "Territory";
		this.gcTerritory.CurrencyDateField = null;
		this.gcTerritory.CurrencyField = null;
		this.gcTerritory.CurrencyFixed = null;
		this.gcTerritory.CurrencyFixedRate = 0.0;
		this.gcTerritory.CurrencyRateField = null;
		this.gcTerritory.FieldName = "BPTerrit";
		this.gcTerritory.Name = "gcTerritory";
		this.gcTerritory.PrimaryKey = false;
		this.gcTerritory.UnitField = null;
		this.gcTerritory.Visible = true;
		this.gcTerritory.VisibleIndex = 4;
		this.gcContactPerson.Caption = "Contact Person";
		this.gcContactPerson.CurrencyDateField = null;
		this.gcContactPerson.CurrencyField = null;
		this.gcContactPerson.CurrencyFixed = null;
		this.gcContactPerson.CurrencyFixedRate = 0.0;
		this.gcContactPerson.CurrencyRateField = null;
		this.gcContactPerson.FieldName = "BPContact";
		this.gcContactPerson.Name = "gcContactPerson";
		this.gcContactPerson.PrimaryKey = false;
		this.gcContactPerson.UnitField = null;
		this.gcContactPerson.Visible = true;
		this.gcContactPerson.VisibleIndex = 5;
		this.gcPhone1.Caption = "Tel 1";
		this.gcPhone1.CurrencyDateField = null;
		this.gcPhone1.CurrencyField = null;
		this.gcPhone1.CurrencyFixed = null;
		this.gcPhone1.CurrencyFixedRate = 0.0;
		this.gcPhone1.CurrencyRateField = null;
		this.gcPhone1.FieldName = "BPPhone1";
		this.gcPhone1.Name = "gcPhone1";
		this.gcPhone1.PrimaryKey = false;
		this.gcPhone1.UnitField = null;
		this.gcPhone1.Visible = true;
		this.gcPhone1.VisibleIndex = 6;
		this.gcPhone2.Caption = "Tel 2";
		this.gcPhone2.CurrencyDateField = null;
		this.gcPhone2.CurrencyField = null;
		this.gcPhone2.CurrencyFixed = null;
		this.gcPhone2.CurrencyFixedRate = 0.0;
		this.gcPhone2.CurrencyRateField = null;
		this.gcPhone2.FieldName = "BPPhone2";
		this.gcPhone2.Name = "gcPhone2";
		this.gcPhone2.PrimaryKey = false;
		this.gcPhone2.UnitField = null;
		this.gcPhone2.Visible = true;
		this.gcPhone2.VisibleIndex = 7;
		this.gcMobilePhone.Caption = "Mobile Phone";
		this.gcMobilePhone.CurrencyDateField = null;
		this.gcMobilePhone.CurrencyField = null;
		this.gcMobilePhone.CurrencyFixed = null;
		this.gcMobilePhone.CurrencyFixedRate = 0.0;
		this.gcMobilePhone.CurrencyRateField = null;
		this.gcMobilePhone.FieldName = "BPCellular";
		this.gcMobilePhone.Name = "gcMobilePhone";
		this.gcMobilePhone.PrimaryKey = false;
		this.gcMobilePhone.UnitField = null;
		this.gcMobilePhone.Visible = true;
		this.gcMobilePhone.VisibleIndex = 8;
		this.gcFax.Caption = "Fax.";
		this.gcFax.CurrencyDateField = null;
		this.gcFax.CurrencyField = null;
		this.gcFax.CurrencyFixed = null;
		this.gcFax.CurrencyFixedRate = 0.0;
		this.gcFax.CurrencyRateField = null;
		this.gcFax.FieldName = "BPFax";
		this.gcFax.Name = "gcFax";
		this.gcFax.PrimaryKey = false;
		this.gcFax.UnitField = null;
		this.gcFax.Visible = true;
		this.gcFax.VisibleIndex = 9;
		this.gcEMail.Caption = "E-Mail";
		this.gcEMail.CurrencyDateField = null;
		this.gcEMail.CurrencyField = null;
		this.gcEMail.CurrencyFixed = null;
		this.gcEMail.CurrencyFixedRate = 0.0;
		this.gcEMail.CurrencyRateField = null;
		this.gcEMail.FieldName = "BPE_Mail";
		this.gcEMail.Name = "gcEMail";
		this.gcEMail.PrimaryKey = false;
		this.gcEMail.UnitField = null;
		this.gcEMail.Visible = true;
		this.gcEMail.VisibleIndex = 10;
		this.gcBPProject.Caption = "BP Project";
		this.gcBPProject.ColumnEdit = this.rBPProjCode;
		this.gcBPProject.CurrencyDateField = null;
		this.gcBPProject.CurrencyField = null;
		this.gcBPProject.CurrencyFixed = null;
		this.gcBPProject.CurrencyFixedRate = 0.0;
		this.gcBPProject.CurrencyRateField = null;
		this.gcBPProject.FieldName = "BPProjCode";
		this.gcBPProject.Name = "gcBPProject";
		this.gcBPProject.PrimaryKey = false;
		this.gcBPProject.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBPProject.UnitField = null;
		this.gcBPProject.Visible = true;
		this.gcBPProject.VisibleIndex = 11;
		this.rBPProjCode.AutoHeight = false;
		this.rBPProjCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rBPProjCode.Name = "rBPProjCode";
		this.rBPProjCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBPProjCode_ButtonClick);
		this.tabRemarks.Controls.Add(this.txtRemarks);
		this.tabRemarks.EditControl = null;
		this.tabRemarks.EditValue = "Remarks";
		this.tabRemarks.FieldName = null;
		this.tabRemarks.IsSearchControl = false;
		this.tabRemarks.Name = "tabRemarks";
		this.tabRemarks.ReadOnly = false;
		this.tabRemarks.SettingInVisible = false;
		this.tabRemarks.SettingReadonly = false;
		this.tabRemarks.Size = new System.Drawing.Size(918, 212);
		this.tabRemarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabRemarks.Text = "Remarks";
		this.txtRemarks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtRemarks.EditControl = null;
		this.txtRemarks.EditValue = "";
		this.txtRemarks.FieldName = null;
		this.txtRemarks.IsSearchControl = false;
		this.txtRemarks.Location = new System.Drawing.Point(3, 0);
		this.txtRemarks.Multiline = true;
		this.txtRemarks.Name = "txtRemarks";
		this.txtRemarks.SettingInVisible = false;
		this.txtRemarks.SettingReadonly = false;
		this.txtRemarks.Size = new System.Drawing.Size(910, 205);
		this.txtRemarks.TabIndex = 0;
		this.txtRemarks.TextChanged += new System.EventHandler(this.txtRemarks_TextChanged);
		this.tabActivities.Controls.Add(this.grdActivities);
		this.tabActivities.EditControl = null;
		this.tabActivities.EditValue = "Activities";
		this.tabActivities.FieldName = null;
		this.tabActivities.IsSearchControl = false;
		this.tabActivities.Name = "tabActivities";
		this.tabActivities.ReadOnly = false;
		this.tabActivities.SettingInVisible = false;
		this.tabActivities.SettingReadonly = false;
		this.tabActivities.Size = new System.Drawing.Size(918, 212);
		this.tabActivities.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabActivities.Text = "Activities";
		this.grdActivities.AllowPasteExcel = false;
		this.grdActivities.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdActivities.ClipboardData = null;
		this.grdActivities.ContextMenuStrip = this.contextMenuActivities;
		this.grdActivities.Location = new System.Drawing.Point(2, -1);
		this.grdActivities.MainView = this.gvActivities;
		this.grdActivities.Name = "grdActivities";
		this.grdActivities.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rHandledby });
		this.grdActivities.Size = new System.Drawing.Size(917, 200);
		this.grdActivities.TabIndex = 0;
		this.grdActivities.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvActivities });
		this.grdActivities.DoubleClick += new System.EventHandler(this.grdActivities_DoubleClick);
		this.contextMenuActivities.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuActivities.Name = "contextMenuActivities";
		this.contextMenuActivities.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvActivities.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvActivities.ColumnKey = null;
		this.gvActivities.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gcCode, this.gcDate, this.gcTime, this.gcHandledbyInActivities, this.gcActivityDate, this.gcRecurrence, this.gcContent, this.gcMAttachT, this.gcDocType });
		this.gvActivities.DisableFormSetting = false;
		this.gvActivities.DisplayException = false;
		this.gvActivities.EditColumn = null;
		this.gvActivities.FieldCurrency = null;
		this.gvActivities.FieldRate = null;
		this.gvActivities.FormId = "oServiceCall";
		this.gvActivities.GridControl = this.grdActivities;
		this.gvActivities.IgnoreFilter = false;
		this.gvActivities.IndexEditRow = -1;
		this.gvActivities.IndicatorWidth = 40;
		this.gvActivities.IsApzTable = false;
		this.gvActivities.IsInsertNewRow = false;
		this.gvActivities.IsLoadingData = false;
		this.gvActivities.IsMappingContextAction = false;
		this.gvActivities.ItemId = "OATV";
		this.gvActivities.LstColumnAllowBatch = null;
		this.gvActivities.LstColumnNotAllowBatch = null;
		this.gvActivities.LstColumnNotEditable = null;
		this.gvActivities.LstColumNotSetting = null;
		this.gvActivities.Name = "gvActivities";
		this.gvActivities.OptionsCustomization.AllowSort = false;
		this.gvActivities.OptionsView.ColumnAutoWidth = false;
		this.gvActivities.OptionsView.ShowGroupPanel = false;
		this.gvActivities.UseBatchMod = false;
		this.gvActivities.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gvActivities_RowUpdated);
		this.gvActivities.RowCountChanged += new System.EventHandler(this.gvActivities_RowCountChanged);
		this.gcCode.Caption = "ClgCode";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "ClgCode";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = false;
		this.gcCode.UnitField = null;
		this.gcDate.Caption = "Date";
		this.gcDate.CurrencyDateField = null;
		this.gcDate.CurrencyField = null;
		this.gcDate.CurrencyFixed = null;
		this.gcDate.CurrencyFixedRate = 0.0;
		this.gcDate.CurrencyRateField = null;
		this.gcDate.FieldName = "CntctDate";
		this.gcDate.Name = "gcDate";
		this.gcDate.OptionsColumn.AllowEdit = false;
		this.gcDate.OptionsFilter.AllowAutoFilter = false;
		this.gcDate.OptionsFilter.AllowFilter = false;
		this.gcDate.PrimaryKey = false;
		this.gcDate.UnitField = null;
		this.gcDate.Visible = true;
		this.gcDate.VisibleIndex = 0;
		this.gcTime.Caption = "Time";
		this.gcTime.CurrencyDateField = null;
		this.gcTime.CurrencyField = null;
		this.gcTime.CurrencyFixed = null;
		this.gcTime.CurrencyFixedRate = 0.0;
		this.gcTime.CurrencyRateField = null;
		this.gcTime.FieldName = "CntctTime";
		this.gcTime.Name = "gcTime";
		this.gcTime.OptionsColumn.AllowEdit = false;
		this.gcTime.OptionsFilter.AllowAutoFilter = false;
		this.gcTime.OptionsFilter.AllowFilter = false;
		this.gcTime.PrimaryKey = false;
		this.gcTime.UnitField = null;
		this.gcTime.Visible = true;
		this.gcTime.VisibleIndex = 1;
		this.gcHandledbyInActivities.Caption = "Handled by";
		this.gcHandledbyInActivities.ColumnEdit = this.rHandledby;
		this.gcHandledbyInActivities.CurrencyDateField = null;
		this.gcHandledbyInActivities.CurrencyField = null;
		this.gcHandledbyInActivities.CurrencyFixed = null;
		this.gcHandledbyInActivities.CurrencyFixedRate = 0.0;
		this.gcHandledbyInActivities.CurrencyRateField = null;
		this.gcHandledbyInActivities.FieldName = "handledby";
		this.gcHandledbyInActivities.Name = "gcHandledbyInActivities";
		this.gcHandledbyInActivities.PrimaryKey = false;
		this.gcHandledbyInActivities.UnitField = null;
		this.gcHandledbyInActivities.Visible = true;
		this.gcHandledbyInActivities.VisibleIndex = 2;
		this.rHandledby.AutoHeight = false;
		this.rHandledby.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rHandledby.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rHandledby.DisplayMember = "Name";
		this.rHandledby.Name = "rHandledby";
		this.rHandledby.NullText = "";
		this.rHandledby.ShowFooter = false;
		this.rHandledby.ShowHeader = false;
		this.rHandledby.ValueMember = "ID";
		this.gcActivityDate.Caption = "Activity Date";
		this.gcActivityDate.CurrencyDateField = null;
		this.gcActivityDate.CurrencyField = null;
		this.gcActivityDate.CurrencyFixed = null;
		this.gcActivityDate.CurrencyFixedRate = 0.0;
		this.gcActivityDate.CurrencyRateField = null;
		this.gcActivityDate.DisplayFormat.FormatString = "d";
		this.gcActivityDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcActivityDate.FieldName = "ActivityDate";
		this.gcActivityDate.Name = "gcActivityDate";
		this.gcActivityDate.OptionsColumn.AllowEdit = false;
		this.gcActivityDate.OptionsFilter.AllowAutoFilter = false;
		this.gcActivityDate.OptionsFilter.AllowFilter = false;
		this.gcActivityDate.PrimaryKey = false;
		this.gcActivityDate.UnitField = null;
		this.gcActivityDate.Visible = true;
		this.gcActivityDate.VisibleIndex = 3;
		this.gcRecurrence.Caption = "Recurrence";
		this.gcRecurrence.CurrencyDateField = null;
		this.gcRecurrence.CurrencyField = null;
		this.gcRecurrence.CurrencyFixed = null;
		this.gcRecurrence.CurrencyFixedRate = 0.0;
		this.gcRecurrence.CurrencyRateField = null;
		this.gcRecurrence.FieldName = "RecurPat";
		this.gcRecurrence.Name = "gcRecurrence";
		this.gcRecurrence.OptionsColumn.AllowEdit = false;
		this.gcRecurrence.OptionsFilter.AllowAutoFilter = false;
		this.gcRecurrence.OptionsFilter.AllowFilter = false;
		this.gcRecurrence.PrimaryKey = false;
		this.gcRecurrence.UnitField = null;
		this.gcRecurrence.Visible = true;
		this.gcRecurrence.VisibleIndex = 4;
		this.gcContent.Caption = "Content";
		this.gcContent.CurrencyDateField = null;
		this.gcContent.CurrencyField = null;
		this.gcContent.CurrencyFixed = null;
		this.gcContent.CurrencyFixedRate = 0.0;
		this.gcContent.CurrencyRateField = null;
		this.gcContent.FieldName = "Notes";
		this.gcContent.Name = "gcContent";
		this.gcContent.OptionsColumn.AllowEdit = false;
		this.gcContent.OptionsFilter.AllowAutoFilter = false;
		this.gcContent.OptionsFilter.AllowFilter = false;
		this.gcContent.PrimaryKey = false;
		this.gcContent.UnitField = null;
		this.gcContent.Visible = true;
		this.gcContent.VisibleIndex = 5;
		this.gcMAttachT.Caption = "MAttachT";
		this.gcMAttachT.CurrencyDateField = null;
		this.gcMAttachT.CurrencyField = null;
		this.gcMAttachT.CurrencyFixed = null;
		this.gcMAttachT.CurrencyFixedRate = 0.0;
		this.gcMAttachT.CurrencyRateField = null;
		this.gcMAttachT.FieldName = "Attachment";
		this.gcMAttachT.Name = "gcMAttachT";
		this.gcMAttachT.OptionsColumn.AllowEdit = false;
		this.gcMAttachT.OptionsFilter.AllowAutoFilter = false;
		this.gcMAttachT.OptionsFilter.AllowFilter = false;
		this.gcMAttachT.PrimaryKey = false;
		this.gcMAttachT.UnitField = null;
		this.gcMAttachT.Visible = true;
		this.gcMAttachT.VisibleIndex = 6;
		this.gcDocType.Caption = "Doc Type";
		this.gcDocType.CurrencyDateField = null;
		this.gcDocType.CurrencyField = null;
		this.gcDocType.CurrencyFixed = null;
		this.gcDocType.CurrencyFixedRate = 0.0;
		this.gcDocType.CurrencyRateField = null;
		this.gcDocType.FieldName = "DocType";
		this.gcDocType.Name = "gcDocType";
		this.gcDocType.OptionsColumn.AllowEdit = false;
		this.gcDocType.OptionsFilter.AllowAutoFilter = false;
		this.gcDocType.OptionsFilter.AllowFilter = false;
		this.gcDocType.PrimaryKey = false;
		this.gcDocType.UnitField = null;
		this.gcDocType.Visible = true;
		this.gcDocType.VisibleIndex = 7;
		this.tabSolutions.Controls.Add(this.grdSolutions);
		this.tabSolutions.EditControl = null;
		this.tabSolutions.EditValue = "Solutions";
		this.tabSolutions.FieldName = null;
		this.tabSolutions.IsSearchControl = false;
		this.tabSolutions.Name = "tabSolutions";
		this.tabSolutions.ReadOnly = false;
		this.tabSolutions.SettingInVisible = false;
		this.tabSolutions.SettingReadonly = false;
		this.tabSolutions.Size = new System.Drawing.Size(918, 212);
		this.tabSolutions.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabSolutions.Text = "Solutions";
		this.grdSolutions.AllowPasteExcel = false;
		this.grdSolutions.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdSolutions.ClipboardData = null;
		this.grdSolutions.ContextMenuStrip = this.contextMenuSolution;
		this.grdSolutions.Location = new System.Drawing.Point(3, -1);
		this.grdSolutions.MainView = this.gvSolutions;
		this.grdSolutions.Name = "grdSolutions";
		this.grdSolutions.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rOwner });
		this.grdSolutions.Size = new System.Drawing.Size(913, 200);
		this.grdSolutions.TabIndex = 0;
		this.grdSolutions.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvSolutions });
		this.contextMenuSolution.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem1 });
		this.contextMenuSolution.Name = "contextMenuSolution";
		this.contextMenuSolution.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gvSolutions.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvSolutions.ColumnKey = null;
		this.gvSolutions.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gcSltCode, this.gcSolution, this.gcSolutionCreatedOn, this.gcOwner, this.gcStatusName, this.gcStatus, this.gcHandledbyInSolutions });
		this.gvSolutions.DisableFormSetting = false;
		this.gvSolutions.DisplayException = false;
		this.gvSolutions.EditColumn = null;
		this.gvSolutions.FieldCurrency = null;
		this.gvSolutions.FieldRate = null;
		this.gvSolutions.FormId = "oServiceCall";
		this.gvSolutions.GridControl = this.grdSolutions;
		this.gvSolutions.IgnoreFilter = false;
		this.gvSolutions.IndexEditRow = -1;
		this.gvSolutions.IndicatorWidth = 40;
		this.gvSolutions.IsApzTable = false;
		this.gvSolutions.IsInsertNewRow = false;
		this.gvSolutions.IsLoadingData = false;
		this.gvSolutions.IsMappingContextAction = false;
		this.gvSolutions.ItemId = "OTAS";
		this.gvSolutions.LstColumnAllowBatch = null;
		this.gvSolutions.LstColumnNotAllowBatch = null;
		this.gvSolutions.LstColumnNotEditable = null;
		this.gvSolutions.LstColumNotSetting = null;
		this.gvSolutions.Name = "gvSolutions";
		this.gvSolutions.OptionsCustomization.AllowSort = false;
		this.gvSolutions.OptionsView.ColumnAutoWidth = false;
		this.gvSolutions.OptionsView.ShowGroupPanel = false;
		this.gvSolutions.UseBatchMod = false;
		this.gvSolutions.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gvSolutions_RowUpdated);
		this.gvSolutions.DoubleClick += new System.EventHandler(this.gvSolutions_DoubleClick);
		this.gvSolutions.RowCountChanged += new System.EventHandler(this.gvSolutions_RowCountChanged);
		this.gcSltCode.Caption = "ID";
		this.gcSltCode.CurrencyDateField = null;
		this.gcSltCode.CurrencyField = null;
		this.gcSltCode.CurrencyFixed = null;
		this.gcSltCode.CurrencyFixedRate = 0.0;
		this.gcSltCode.CurrencyRateField = null;
		this.gcSltCode.FieldName = "SltCode";
		this.gcSltCode.Name = "gcSltCode";
		this.gcSltCode.OptionsColumn.AllowEdit = false;
		this.gcSltCode.OptionsFilter.AllowAutoFilter = false;
		this.gcSltCode.OptionsFilter.AllowFilter = false;
		this.gcSltCode.PrimaryKey = false;
		this.gcSltCode.UnitField = null;
		this.gcSltCode.Visible = true;
		this.gcSltCode.VisibleIndex = 0;
		this.gcSolution.Caption = "Solution";
		this.gcSolution.CurrencyDateField = null;
		this.gcSolution.CurrencyField = null;
		this.gcSolution.CurrencyFixed = null;
		this.gcSolution.CurrencyFixedRate = 0.0;
		this.gcSolution.CurrencyRateField = null;
		this.gcSolution.FieldName = "Subject";
		this.gcSolution.Name = "gcSolution";
		this.gcSolution.OptionsColumn.AllowEdit = false;
		this.gcSolution.OptionsFilter.AllowAutoFilter = false;
		this.gcSolution.OptionsFilter.AllowFilter = false;
		this.gcSolution.PrimaryKey = false;
		this.gcSolution.UnitField = null;
		this.gcSolution.Visible = true;
		this.gcSolution.VisibleIndex = 1;
		this.gcSolutionCreatedOn.Caption = "Created on";
		this.gcSolutionCreatedOn.CurrencyDateField = null;
		this.gcSolutionCreatedOn.CurrencyField = null;
		this.gcSolutionCreatedOn.CurrencyFixed = null;
		this.gcSolutionCreatedOn.CurrencyFixedRate = 0.0;
		this.gcSolutionCreatedOn.CurrencyRateField = null;
		this.gcSolutionCreatedOn.DisplayFormat.FormatString = "d";
		this.gcSolutionCreatedOn.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcSolutionCreatedOn.FieldName = "DateCreate";
		this.gcSolutionCreatedOn.Name = "gcSolutionCreatedOn";
		this.gcSolutionCreatedOn.OptionsColumn.AllowEdit = false;
		this.gcSolutionCreatedOn.OptionsFilter.AllowAutoFilter = false;
		this.gcSolutionCreatedOn.OptionsFilter.AllowFilter = false;
		this.gcSolutionCreatedOn.PrimaryKey = false;
		this.gcSolutionCreatedOn.UnitField = null;
		this.gcSolutionCreatedOn.Visible = true;
		this.gcSolutionCreatedOn.VisibleIndex = 2;
		this.gcOwner.Caption = "Owner";
		this.gcOwner.ColumnEdit = this.rOwner;
		this.gcOwner.CurrencyDateField = null;
		this.gcOwner.CurrencyField = null;
		this.gcOwner.CurrencyFixed = null;
		this.gcOwner.CurrencyFixedRate = 0.0;
		this.gcOwner.CurrencyRateField = null;
		this.gcOwner.FieldName = "Owner";
		this.gcOwner.Name = "gcOwner";
		this.gcOwner.OptionsColumn.AllowEdit = false;
		this.gcOwner.OptionsFilter.AllowAutoFilter = false;
		this.gcOwner.OptionsFilter.AllowFilter = false;
		this.gcOwner.PrimaryKey = false;
		this.gcOwner.UnitField = null;
		this.gcOwner.Visible = true;
		this.gcOwner.VisibleIndex = 3;
		this.rOwner.AutoHeight = false;
		this.rOwner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOwner.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rOwner.DisplayMember = "Name";
		this.rOwner.Name = "rOwner";
		this.rOwner.NullText = "";
		this.rOwner.ShowFooter = false;
		this.rOwner.ShowHeader = false;
		this.rOwner.ValueMember = "ID";
		this.gcStatusName.Caption = "Status";
		this.gcStatusName.CurrencyDateField = null;
		this.gcStatusName.CurrencyField = null;
		this.gcStatusName.CurrencyFixed = null;
		this.gcStatusName.CurrencyFixedRate = 0.0;
		this.gcStatusName.CurrencyRateField = null;
		this.gcStatusName.FieldName = "StatusName";
		this.gcStatusName.Name = "gcStatusName";
		this.gcStatusName.OptionsColumn.AllowEdit = false;
		this.gcStatusName.OptionsColumn.ReadOnly = true;
		this.gcStatusName.PrimaryKey = false;
		this.gcStatusName.UnitField = null;
		this.gcStatusName.Visible = true;
		this.gcStatusName.VisibleIndex = 4;
		this.gcStatus.Caption = "Status Num";
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "StatusNum";
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.OptionsColumn.AllowEdit = false;
		this.gcStatus.OptionsFilter.AllowAutoFilter = false;
		this.gcStatus.OptionsFilter.AllowFilter = false;
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcHandledbyInSolutions.Caption = "Handled by";
		this.gcHandledbyInSolutions.ColumnEdit = this.rOwner;
		this.gcHandledbyInSolutions.CurrencyDateField = null;
		this.gcHandledbyInSolutions.CurrencyField = null;
		this.gcHandledbyInSolutions.CurrencyFixed = null;
		this.gcHandledbyInSolutions.CurrencyFixedRate = 0.0;
		this.gcHandledbyInSolutions.CurrencyRateField = null;
		this.gcHandledbyInSolutions.FieldName = "userSign2";
		this.gcHandledbyInSolutions.Name = "gcHandledbyInSolutions";
		this.gcHandledbyInSolutions.OptionsColumn.AllowEdit = false;
		this.gcHandledbyInSolutions.OptionsFilter.AllowAutoFilter = false;
		this.gcHandledbyInSolutions.OptionsFilter.AllowFilter = false;
		this.gcHandledbyInSolutions.PrimaryKey = false;
		this.gcHandledbyInSolutions.UnitField = null;
		this.gcHandledbyInSolutions.Visible = true;
		this.gcHandledbyInSolutions.VisibleIndex = 5;
		this.tabExpenses.Controls.Add(this.grdExpenses);
		this.tabExpenses.EditControl = null;
		this.tabExpenses.EditValue = "Expenses";
		this.tabExpenses.FieldName = null;
		this.tabExpenses.IsSearchControl = false;
		this.tabExpenses.Name = "tabExpenses";
		this.tabExpenses.ReadOnly = false;
		this.tabExpenses.SettingInVisible = false;
		this.tabExpenses.SettingReadonly = false;
		this.tabExpenses.Size = new System.Drawing.Size(918, 212);
		this.tabExpenses.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabExpenses.Text = "Expenses";
		this.grdExpenses.AllowPasteExcel = false;
		this.grdExpenses.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdExpenses.ClipboardData = null;
		this.grdExpenses.Location = new System.Drawing.Point(-1, 0);
		this.grdExpenses.MainView = this.gvExpenses;
		this.grdExpenses.Name = "grdExpenses";
		this.grdExpenses.Size = new System.Drawing.Size(915, 209);
		this.grdExpenses.TabIndex = 0;
		this.grdExpenses.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvExpenses });
		this.gvExpenses.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvExpenses.ColumnKey = null;
		this.gvExpenses.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcDocumentType, this.gcDocumentNo, this.gcDateInExpenses, this.gcItemNo, this.gcItemDescription, this.gcQuantity, this.gcFromWhse, this.gcToWhse });
		this.gvExpenses.DisableFormSetting = false;
		this.gvExpenses.DisplayException = false;
		this.gvExpenses.EditColumn = null;
		this.gvExpenses.FieldCurrency = null;
		this.gvExpenses.FieldRate = null;
		this.gvExpenses.FormId = "oServiceCall";
		this.gvExpenses.GridControl = this.grdExpenses;
		this.gvExpenses.IgnoreFilter = false;
		this.gvExpenses.IndexEditRow = -1;
		this.gvExpenses.IndicatorWidth = 35;
		this.gvExpenses.IsApzTable = false;
		this.gvExpenses.IsInsertNewRow = false;
		this.gvExpenses.IsLoadingData = false;
		this.gvExpenses.IsMappingContextAction = false;
		this.gvExpenses.ItemId = "OEXP";
		this.gvExpenses.LstColumnAllowBatch = null;
		this.gvExpenses.LstColumnNotAllowBatch = null;
		this.gvExpenses.LstColumnNotEditable = null;
		this.gvExpenses.LstColumNotSetting = null;
		this.gvExpenses.Name = "gvExpenses";
		this.gvExpenses.OptionsBehavior.Editable = false;
		this.gvExpenses.OptionsBehavior.ReadOnly = true;
		this.gvExpenses.OptionsCustomization.AllowSort = false;
		this.gvExpenses.OptionsView.ColumnAutoWidth = false;
		this.gvExpenses.OptionsView.ShowGroupPanel = false;
		this.gvExpenses.UseBatchMod = false;
		this.gcDocumentType.Caption = "Document Type";
		this.gcDocumentType.CurrencyDateField = null;
		this.gcDocumentType.CurrencyField = null;
		this.gcDocumentType.CurrencyFixed = null;
		this.gcDocumentType.CurrencyFixedRate = 0.0;
		this.gcDocumentType.CurrencyRateField = null;
		this.gcDocumentType.FieldName = "Object";
		this.gcDocumentType.Name = "gcDocumentType";
		this.gcDocumentType.OptionsColumn.AllowEdit = false;
		this.gcDocumentType.OptionsFilter.AllowAutoFilter = false;
		this.gcDocumentType.OptionsFilter.AllowFilter = false;
		this.gcDocumentType.PrimaryKey = false;
		this.gcDocumentType.UnitField = null;
		this.gcDocumentType.Visible = true;
		this.gcDocumentType.VisibleIndex = 0;
		this.gcDocumentNo.Caption = "Document No.";
		this.gcDocumentNo.CurrencyDateField = null;
		this.gcDocumentNo.CurrencyField = null;
		this.gcDocumentNo.CurrencyFixed = null;
		this.gcDocumentNo.CurrencyFixedRate = 0.0;
		this.gcDocumentNo.CurrencyRateField = null;
		this.gcDocumentNo.FieldName = "documentNo";
		this.gcDocumentNo.Name = "gcDocumentNo";
		this.gcDocumentNo.OptionsColumn.AllowEdit = false;
		this.gcDocumentNo.OptionsFilter.AllowAutoFilter = false;
		this.gcDocumentNo.OptionsFilter.AllowFilter = false;
		this.gcDocumentNo.PrimaryKey = false;
		this.gcDocumentNo.UnitField = null;
		this.gcDocumentNo.Visible = true;
		this.gcDocumentNo.VisibleIndex = 1;
		this.gcDateInExpenses.Caption = "Date";
		this.gcDateInExpenses.CurrencyDateField = null;
		this.gcDateInExpenses.CurrencyField = null;
		this.gcDateInExpenses.CurrencyFixed = null;
		this.gcDateInExpenses.CurrencyFixedRate = 0.0;
		this.gcDateInExpenses.CurrencyRateField = null;
		this.gcDateInExpenses.DisplayFormat.FormatString = "d";
		this.gcDateInExpenses.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDateInExpenses.FieldName = "Date";
		this.gcDateInExpenses.Name = "gcDateInExpenses";
		this.gcDateInExpenses.OptionsColumn.AllowEdit = false;
		this.gcDateInExpenses.OptionsFilter.AllowAutoFilter = false;
		this.gcDateInExpenses.OptionsFilter.AllowFilter = false;
		this.gcDateInExpenses.PrimaryKey = false;
		this.gcDateInExpenses.UnitField = null;
		this.gcDateInExpenses.Visible = true;
		this.gcDateInExpenses.VisibleIndex = 2;
		this.gcItemNo.Caption = "Item No.";
		this.gcItemNo.CurrencyDateField = null;
		this.gcItemNo.CurrencyField = null;
		this.gcItemNo.CurrencyFixed = null;
		this.gcItemNo.CurrencyFixedRate = 0.0;
		this.gcItemNo.CurrencyRateField = null;
		this.gcItemNo.FieldName = "itemNo";
		this.gcItemNo.Name = "gcItemNo";
		this.gcItemNo.OptionsColumn.AllowEdit = false;
		this.gcItemNo.OptionsFilter.AllowAutoFilter = false;
		this.gcItemNo.OptionsFilter.AllowFilter = false;
		this.gcItemNo.PrimaryKey = false;
		this.gcItemNo.UnitField = null;
		this.gcItemNo.Visible = true;
		this.gcItemNo.VisibleIndex = 3;
		this.gcItemDescription.Caption = "Item Description";
		this.gcItemDescription.CurrencyDateField = null;
		this.gcItemDescription.CurrencyField = null;
		this.gcItemDescription.CurrencyFixed = null;
		this.gcItemDescription.CurrencyFixedRate = 0.0;
		this.gcItemDescription.CurrencyRateField = null;
		this.gcItemDescription.FieldName = "itemDescription";
		this.gcItemDescription.Name = "gcItemDescription";
		this.gcItemDescription.OptionsColumn.AllowEdit = false;
		this.gcItemDescription.OptionsFilter.AllowAutoFilter = false;
		this.gcItemDescription.PrimaryKey = false;
		this.gcItemDescription.UnitField = null;
		this.gcItemDescription.Visible = true;
		this.gcItemDescription.VisibleIndex = 4;
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.FieldName = "Quantity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.OptionsColumn.AllowEdit = false;
		this.gcQuantity.OptionsFilter.AllowAutoFilter = false;
		this.gcQuantity.OptionsFilter.AllowFilter = false;
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.UnitField = null;
		this.gcQuantity.Visible = true;
		this.gcQuantity.VisibleIndex = 5;
		this.gcFromWhse.Caption = "From Whse";
		this.gcFromWhse.CurrencyDateField = null;
		this.gcFromWhse.CurrencyField = null;
		this.gcFromWhse.CurrencyFixed = null;
		this.gcFromWhse.CurrencyFixedRate = 0.0;
		this.gcFromWhse.CurrencyRateField = null;
		this.gcFromWhse.FieldName = "fromWhse";
		this.gcFromWhse.Name = "gcFromWhse";
		this.gcFromWhse.OptionsColumn.AllowEdit = false;
		this.gcFromWhse.OptionsFilter.AllowAutoFilter = false;
		this.gcFromWhse.OptionsFilter.AllowFilter = false;
		this.gcFromWhse.PrimaryKey = false;
		this.gcFromWhse.UnitField = null;
		this.gcFromWhse.Visible = true;
		this.gcFromWhse.VisibleIndex = 6;
		this.gcToWhse.Caption = "To Whse";
		this.gcToWhse.CurrencyDateField = null;
		this.gcToWhse.CurrencyField = null;
		this.gcToWhse.CurrencyFixed = null;
		this.gcToWhse.CurrencyFixedRate = 0.0;
		this.gcToWhse.CurrencyRateField = null;
		this.gcToWhse.FieldName = "toWhse";
		this.gcToWhse.Name = "gcToWhse";
		this.gcToWhse.OptionsColumn.AllowEdit = false;
		this.gcToWhse.OptionsFilter.AllowAutoFilter = false;
		this.gcToWhse.OptionsFilter.AllowFilter = false;
		this.gcToWhse.PrimaryKey = false;
		this.gcToWhse.UnitField = null;
		this.gcToWhse.Visible = true;
		this.gcToWhse.VisibleIndex = 7;
		this.tabResolution.Controls.Add(this.txtResolution);
		this.tabResolution.EditControl = null;
		this.tabResolution.EditValue = "Resolution";
		this.tabResolution.FieldName = null;
		this.tabResolution.IsSearchControl = false;
		this.tabResolution.Name = "tabResolution";
		this.tabResolution.ReadOnly = false;
		this.tabResolution.SettingInVisible = false;
		this.tabResolution.SettingReadonly = false;
		this.tabResolution.Size = new System.Drawing.Size(918, 212);
		this.tabResolution.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabResolution.Text = "Resolution";
		this.txtResolution.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtResolution.EditControl = null;
		this.txtResolution.EditValue = "";
		this.txtResolution.FieldName = null;
		this.txtResolution.IsSearchControl = false;
		this.txtResolution.Location = new System.Drawing.Point(2, 0);
		this.txtResolution.Multiline = true;
		this.txtResolution.Name = "txtResolution";
		this.txtResolution.SettingInVisible = false;
		this.txtResolution.SettingReadonly = false;
		this.txtResolution.Size = new System.Drawing.Size(914, 206);
		this.txtResolution.TabIndex = 0;
		this.txtResolution.TextChanged += new System.EventHandler(this.txtResolution_TextChanged);
		this.tabHistory.Controls.Add(this.grdHistory);
		this.tabHistory.EditControl = null;
		this.tabHistory.EditValue = "History";
		this.tabHistory.FieldName = null;
		this.tabHistory.IsSearchControl = false;
		this.tabHistory.Name = "tabHistory";
		this.tabHistory.ReadOnly = false;
		this.tabHistory.SettingInVisible = false;
		this.tabHistory.SettingReadonly = false;
		this.tabHistory.Size = new System.Drawing.Size(918, 212);
		this.tabHistory.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabHistory.Text = "History";
		this.grdHistory.AllowPasteExcel = false;
		this.grdHistory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdHistory.ClipboardData = null;
		this.grdHistory.Location = new System.Drawing.Point(-1, 0);
		this.grdHistory.MainView = this.gvHistory;
		this.grdHistory.Name = "grdHistory";
		this.grdHistory.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rUpdatedbyInHistory });
		this.grdHistory.Size = new System.Drawing.Size(920, 209);
		this.grdHistory.TabIndex = 0;
		this.grdHistory.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvHistory });
		this.gvHistory.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvHistory.ColumnKey = null;
		this.gvHistory.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gcDateofUpdate, this.gcUpdateTime, this.gcUpdatedby, this.gcDescriptionInHistory, this.gcPreviousValue, this.gcNewValue });
		this.gvHistory.DisableFormSetting = false;
		this.gvHistory.DisplayException = false;
		this.gvHistory.EditColumn = null;
		this.gvHistory.FieldCurrency = null;
		this.gvHistory.FieldRate = null;
		this.gvHistory.FormId = "oServiceCall";
		this.gvHistory.GridControl = this.grdHistory;
		this.gvHistory.IgnoreFilter = false;
		this.gvHistory.IndexEditRow = -1;
		this.gvHistory.IndicatorWidth = 40;
		this.gvHistory.IsApzTable = false;
		this.gvHistory.IsInsertNewRow = false;
		this.gvHistory.IsLoadingData = false;
		this.gvHistory.IsMappingContextAction = false;
		this.gvHistory.ItemId = "OHTR";
		this.gvHistory.LstColumnAllowBatch = null;
		this.gvHistory.LstColumnNotAllowBatch = null;
		this.gvHistory.LstColumnNotEditable = null;
		this.gvHistory.LstColumNotSetting = null;
		this.gvHistory.Name = "gvHistory";
		this.gvHistory.OptionsBehavior.Editable = false;
		this.gvHistory.OptionsBehavior.ReadOnly = true;
		this.gvHistory.OptionsCustomization.AllowSort = false;
		this.gvHistory.OptionsView.ColumnAutoWidth = false;
		this.gvHistory.OptionsView.ShowGroupPanel = false;
		this.gvHistory.UseBatchMod = false;
		this.gcDateofUpdate.Caption = "Date of Update";
		this.gcDateofUpdate.CurrencyDateField = null;
		this.gcDateofUpdate.CurrencyField = null;
		this.gcDateofUpdate.CurrencyFixed = null;
		this.gcDateofUpdate.CurrencyFixedRate = 0.0;
		this.gcDateofUpdate.CurrencyRateField = null;
		this.gcDateofUpdate.DisplayFormat.FormatString = "d";
		this.gcDateofUpdate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcDateofUpdate.FieldName = "updateDate";
		this.gcDateofUpdate.Name = "gcDateofUpdate";
		this.gcDateofUpdate.OptionsColumn.AllowEdit = false;
		this.gcDateofUpdate.OptionsFilter.AllowAutoFilter = false;
		this.gcDateofUpdate.OptionsFilter.AllowFilter = false;
		this.gcDateofUpdate.PrimaryKey = false;
		this.gcDateofUpdate.UnitField = null;
		this.gcDateofUpdate.Visible = true;
		this.gcDateofUpdate.VisibleIndex = 0;
		this.gcUpdateTime.Caption = "Update Time";
		this.gcUpdateTime.CurrencyDateField = null;
		this.gcUpdateTime.CurrencyField = null;
		this.gcUpdateTime.CurrencyFixed = null;
		this.gcUpdateTime.CurrencyFixedRate = 0.0;
		this.gcUpdateTime.CurrencyRateField = null;
		this.gcUpdateTime.FieldName = "UpdateTime";
		this.gcUpdateTime.Name = "gcUpdateTime";
		this.gcUpdateTime.OptionsColumn.AllowEdit = false;
		this.gcUpdateTime.OptionsFilter.AllowAutoFilter = false;
		this.gcUpdateTime.OptionsFilter.AllowFilter = false;
		this.gcUpdateTime.PrimaryKey = false;
		this.gcUpdateTime.UnitField = null;
		this.gcUpdateTime.Visible = true;
		this.gcUpdateTime.VisibleIndex = 1;
		this.gcUpdatedby.Caption = "Updated by";
		this.gcUpdatedby.ColumnEdit = this.rUpdatedbyInHistory;
		this.gcUpdatedby.CurrencyDateField = null;
		this.gcUpdatedby.CurrencyField = null;
		this.gcUpdatedby.CurrencyFixed = null;
		this.gcUpdatedby.CurrencyFixedRate = 0.0;
		this.gcUpdatedby.CurrencyRateField = null;
		this.gcUpdatedby.FieldName = "assignee";
		this.gcUpdatedby.Name = "gcUpdatedby";
		this.gcUpdatedby.OptionsColumn.AllowEdit = false;
		this.gcUpdatedby.OptionsFilter.AllowAutoFilter = false;
		this.gcUpdatedby.OptionsFilter.AllowFilter = false;
		this.gcUpdatedby.PrimaryKey = false;
		this.gcUpdatedby.UnitField = null;
		this.gcUpdatedby.Visible = true;
		this.gcUpdatedby.VisibleIndex = 2;
		this.rUpdatedbyInHistory.AutoHeight = false;
		this.rUpdatedbyInHistory.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUpdatedbyInHistory.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rUpdatedbyInHistory.DisplayMember = "Name";
		this.rUpdatedbyInHistory.Name = "rUpdatedbyInHistory";
		this.rUpdatedbyInHistory.NullText = "";
		this.rUpdatedbyInHistory.ShowFooter = false;
		this.rUpdatedbyInHistory.ShowHeader = false;
		this.rUpdatedbyInHistory.ValueMember = "ID";
		this.gcDescriptionInHistory.Caption = "Description";
		this.gcDescriptionInHistory.CurrencyDateField = null;
		this.gcDescriptionInHistory.CurrencyField = null;
		this.gcDescriptionInHistory.CurrencyFixed = null;
		this.gcDescriptionInHistory.CurrencyFixedRate = 0.0;
		this.gcDescriptionInHistory.CurrencyRateField = null;
		this.gcDescriptionInHistory.FieldName = "descrption";
		this.gcDescriptionInHistory.Name = "gcDescriptionInHistory";
		this.gcDescriptionInHistory.OptionsColumn.AllowEdit = false;
		this.gcDescriptionInHistory.OptionsFilter.AllowAutoFilter = false;
		this.gcDescriptionInHistory.OptionsFilter.AllowFilter = false;
		this.gcDescriptionInHistory.PrimaryKey = false;
		this.gcDescriptionInHistory.UnitField = null;
		this.gcDescriptionInHistory.Visible = true;
		this.gcDescriptionInHistory.VisibleIndex = 3;
		this.gcPreviousValue.Caption = "Previous Value";
		this.gcPreviousValue.CurrencyDateField = null;
		this.gcPreviousValue.CurrencyField = null;
		this.gcPreviousValue.CurrencyFixed = null;
		this.gcPreviousValue.CurrencyFixedRate = 0.0;
		this.gcPreviousValue.CurrencyRateField = null;
		this.gcPreviousValue.FieldName = "previousValue";
		this.gcPreviousValue.Name = "gcPreviousValue";
		this.gcPreviousValue.OptionsColumn.AllowEdit = false;
		this.gcPreviousValue.OptionsFilter.AllowAutoFilter = false;
		this.gcPreviousValue.OptionsFilter.AllowFilter = false;
		this.gcPreviousValue.PrimaryKey = false;
		this.gcPreviousValue.UnitField = null;
		this.gcPreviousValue.Visible = true;
		this.gcPreviousValue.VisibleIndex = 4;
		this.gcNewValue.Caption = "New Value";
		this.gcNewValue.CurrencyDateField = null;
		this.gcNewValue.CurrencyField = null;
		this.gcNewValue.CurrencyFixed = null;
		this.gcNewValue.CurrencyFixedRate = 0.0;
		this.gcNewValue.CurrencyRateField = null;
		this.gcNewValue.FieldName = "newValue";
		this.gcNewValue.Name = "gcNewValue";
		this.gcNewValue.OptionsColumn.AllowEdit = false;
		this.gcNewValue.OptionsFilter.AllowAutoFilter = false;
		this.gcNewValue.OptionsFilter.AllowFilter = false;
		this.gcNewValue.PrimaryKey = false;
		this.gcNewValue.UnitField = null;
		this.gcNewValue.Visible = true;
		this.gcNewValue.VisibleIndex = 5;
		this.tabScheduling.Controls.Add(this.grdScheduling);
		this.tabScheduling.EditControl = null;
		this.tabScheduling.EditValue = "Scheduling";
		this.tabScheduling.FieldName = null;
		this.tabScheduling.IsSearchControl = false;
		this.tabScheduling.Name = "tabScheduling";
		this.tabScheduling.ReadOnly = false;
		this.tabScheduling.SettingInVisible = false;
		this.tabScheduling.SettingReadonly = false;
		this.tabScheduling.Size = new System.Drawing.Size(918, 212);
		this.tabScheduling.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabScheduling.Text = "Scheduling";
		this.grdScheduling.AllowPasteExcel = false;
		this.grdScheduling.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdScheduling.ClipboardData = null;
		this.grdScheduling.Location = new System.Drawing.Point(-1, 0);
		this.grdScheduling.MainView = this.gvScheduling;
		this.grdScheduling.Name = "grdScheduling";
		this.grdScheduling.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.rDate, this.rReminder, this.rDisplayinCalendar, this.rMeetingLocation, this.rCountry, this.rState, this.rDurType, this.rRemType, this.rStartTime, this.rDuration,
			this.rRemQty, this.rEndtime, this.rAddressID
		});
		this.grdScheduling.Size = new System.Drawing.Size(915, 214);
		this.grdScheduling.TabIndex = 0;
		this.grdScheduling.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvScheduling });
		this.gvScheduling.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvScheduling.ColumnKey = null;
		this.gvScheduling.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gcStartDate, this.gcStartTime, this.gcEndDateInScheduling, this.gcEndTime, this.gcDuration, this.gcDurType, this.gcReminder, this.gcDisplayinCalendar, this.gcRemQty, this.gcRemType,
			this.gcMeetingLocation, this.gcAddressID, this.AddrType, this.gcStreet, this.gcCity, this.gcRoom, this.gcState, this.gcCountry
		});
		this.gvScheduling.DisableFormSetting = false;
		this.gvScheduling.DisplayException = false;
		this.gvScheduling.EditColumn = null;
		this.gvScheduling.FieldCurrency = null;
		this.gvScheduling.FieldRate = null;
		this.gvScheduling.FormId = "oServiceCall";
		this.gvScheduling.GridControl = this.grdScheduling;
		this.gvScheduling.IgnoreFilter = false;
		this.gvScheduling.IndexEditRow = -1;
		this.gvScheduling.IndicatorWidth = 35;
		this.gvScheduling.IsApzTable = false;
		this.gvScheduling.IsInsertNewRow = false;
		this.gvScheduling.IsLoadingData = false;
		this.gvScheduling.IsMappingContextAction = false;
		this.gvScheduling.ItemId = "OSCD";
		this.gvScheduling.LstColumnAllowBatch = null;
		this.gvScheduling.LstColumnNotAllowBatch = null;
		this.gvScheduling.LstColumnNotEditable = null;
		this.gvScheduling.LstColumNotSetting = null;
		this.gvScheduling.Name = "gvScheduling";
		this.gvScheduling.OptionsCustomization.AllowSort = false;
		this.gvScheduling.OptionsView.ColumnAutoWidth = false;
		this.gvScheduling.OptionsView.ShowGroupPanel = false;
		this.gvScheduling.UseBatchMod = false;
		this.gvScheduling.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvScheduling_ShowingEditor);
		this.gvScheduling.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvScheduling_CellValueChanged);
		this.gvScheduling.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvScheduling_CellValueChanging);
		this.gcStartDate.Caption = "Start date";
		this.gcStartDate.ColumnEdit = this.rDate;
		this.gcStartDate.CurrencyDateField = null;
		this.gcStartDate.CurrencyField = null;
		this.gcStartDate.CurrencyFixed = null;
		this.gcStartDate.CurrencyFixedRate = 0.0;
		this.gcStartDate.CurrencyRateField = null;
		this.gcStartDate.DisplayFormat.FormatString = "d";
		this.gcStartDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcStartDate.FieldName = "StartDate";
		this.gcStartDate.Name = "gcStartDate";
		this.gcStartDate.OptionsFilter.AllowAutoFilter = false;
		this.gcStartDate.OptionsFilter.AllowFilter = false;
		this.gcStartDate.PrimaryKey = false;
		this.gcStartDate.UnitField = null;
		this.gcStartDate.Visible = true;
		this.gcStartDate.VisibleIndex = 0;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.Name = "rDate";
		this.gcStartTime.Caption = "Start time";
		this.gcStartTime.ColumnEdit = this.rStartTime;
		this.gcStartTime.CurrencyDateField = null;
		this.gcStartTime.CurrencyField = null;
		this.gcStartTime.CurrencyFixed = null;
		this.gcStartTime.CurrencyFixedRate = 0.0;
		this.gcStartTime.CurrencyRateField = null;
		this.gcStartTime.FieldName = "StartTime";
		this.gcStartTime.Name = "gcStartTime";
		this.gcStartTime.OptionsFilter.AllowAutoFilter = false;
		this.gcStartTime.OptionsFilter.AllowFilter = false;
		this.gcStartTime.PrimaryKey = false;
		this.gcStartTime.UnitField = null;
		this.gcStartTime.Visible = true;
		this.gcStartTime.VisibleIndex = 1;
		this.rStartTime.AutoHeight = false;
		this.rStartTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStartTime.Mask.EditMask = "HH:mm";
		this.rStartTime.Name = "rStartTime";
		this.rStartTime.EditValueChanged += new System.EventHandler(this.rTimeInScheduling_EditValueChanged);
		this.gcEndDateInScheduling.Caption = "End date";
		this.gcEndDateInScheduling.ColumnEdit = this.rDate;
		this.gcEndDateInScheduling.CurrencyDateField = null;
		this.gcEndDateInScheduling.CurrencyField = null;
		this.gcEndDateInScheduling.CurrencyFixed = null;
		this.gcEndDateInScheduling.CurrencyFixedRate = 0.0;
		this.gcEndDateInScheduling.CurrencyRateField = null;
		this.gcEndDateInScheduling.DisplayFormat.FormatString = "d";
		this.gcEndDateInScheduling.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcEndDateInScheduling.FieldName = "EndDate";
		this.gcEndDateInScheduling.Name = "gcEndDateInScheduling";
		this.gcEndDateInScheduling.OptionsFilter.AllowAutoFilter = false;
		this.gcEndDateInScheduling.OptionsFilter.AllowFilter = false;
		this.gcEndDateInScheduling.PrimaryKey = false;
		this.gcEndDateInScheduling.UnitField = null;
		this.gcEndDateInScheduling.Visible = true;
		this.gcEndDateInScheduling.VisibleIndex = 2;
		this.gcEndTime.Caption = "End time";
		this.gcEndTime.ColumnEdit = this.rEndtime;
		this.gcEndTime.CurrencyDateField = null;
		this.gcEndTime.CurrencyField = null;
		this.gcEndTime.CurrencyFixed = null;
		this.gcEndTime.CurrencyFixedRate = 0.0;
		this.gcEndTime.CurrencyRateField = null;
		this.gcEndTime.FieldName = "EndTime";
		this.gcEndTime.Name = "gcEndTime";
		this.gcEndTime.OptionsFilter.AllowAutoFilter = false;
		this.gcEndTime.OptionsFilter.AllowFilter = false;
		this.gcEndTime.PrimaryKey = false;
		this.gcEndTime.UnitField = null;
		this.gcEndTime.Visible = true;
		this.gcEndTime.VisibleIndex = 3;
		this.rEndtime.AutoHeight = false;
		this.rEndtime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rEndtime.Mask.EditMask = "HH:mm";
		this.rEndtime.Name = "rEndtime";
		this.rEndtime.EditValueChanged += new System.EventHandler(this.rEndtime_EditValueChanged);
		this.gcDuration.Caption = "Duration";
		this.gcDuration.ColumnEdit = this.rDuration;
		this.gcDuration.CurrencyDateField = null;
		this.gcDuration.CurrencyField = null;
		this.gcDuration.CurrencyFixed = null;
		this.gcDuration.CurrencyFixedRate = 0.0;
		this.gcDuration.CurrencyRateField = null;
		this.gcDuration.FieldName = "Duration";
		this.gcDuration.Name = "gcDuration";
		this.gcDuration.OptionsFilter.AllowAutoFilter = false;
		this.gcDuration.OptionsFilter.AllowFilter = false;
		this.gcDuration.PrimaryKey = false;
		this.gcDuration.UnitField = null;
		this.gcDuration.Visible = true;
		this.gcDuration.VisibleIndex = 4;
		this.rDuration.AutoHeight = false;
		this.rDuration.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rDuration.Name = "rDuration";
		this.rDuration.EditValueChanged += new System.EventHandler(this.rDuration_EditValueChanged);
		this.gcDurType.Caption = "Duration Type";
		this.gcDurType.ColumnEdit = this.rDurType;
		this.gcDurType.CurrencyDateField = null;
		this.gcDurType.CurrencyField = null;
		this.gcDurType.CurrencyFixed = null;
		this.gcDurType.CurrencyFixedRate = 0.0;
		this.gcDurType.CurrencyRateField = null;
		this.gcDurType.FieldName = "DurType";
		this.gcDurType.Name = "gcDurType";
		this.gcDurType.PrimaryKey = false;
		this.gcDurType.UnitField = null;
		this.gcDurType.Visible = true;
		this.gcDurType.VisibleIndex = 5;
		this.rDurType.AutoHeight = false;
		this.rDurType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDurType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDurType.DisplayMember = "Name";
		this.rDurType.Name = "rDurType";
		this.rDurType.NullText = "";
		this.rDurType.ShowFooter = false;
		this.rDurType.ShowHeader = false;
		this.rDurType.ValueMember = "ID";
		this.rDurType.EditValueChanged += new System.EventHandler(this.rDurType_EditValueChanged);
		this.gcReminder.Caption = "Reminder";
		this.gcReminder.ColumnEdit = this.rReminder;
		this.gcReminder.CurrencyDateField = null;
		this.gcReminder.CurrencyField = null;
		this.gcReminder.CurrencyFixed = null;
		this.gcReminder.CurrencyFixedRate = 0.0;
		this.gcReminder.CurrencyRateField = null;
		this.gcReminder.FieldName = "Reminder";
		this.gcReminder.Name = "gcReminder";
		this.gcReminder.OptionsFilter.AllowAutoFilter = false;
		this.gcReminder.OptionsFilter.AllowFilter = false;
		this.gcReminder.PrimaryKey = false;
		this.gcReminder.UnitField = null;
		this.gcReminder.Visible = true;
		this.gcReminder.VisibleIndex = 6;
		this.rReminder.AutoHeight = false;
		this.rReminder.Name = "rReminder";
		this.rReminder.ValueChecked = "Y";
		this.rReminder.ValueUnchecked = "N";
		this.gcDisplayinCalendar.Caption = "Display in Calendar";
		this.gcDisplayinCalendar.ColumnEdit = this.rDisplayinCalendar;
		this.gcDisplayinCalendar.CurrencyDateField = null;
		this.gcDisplayinCalendar.CurrencyField = null;
		this.gcDisplayinCalendar.CurrencyFixed = null;
		this.gcDisplayinCalendar.CurrencyFixedRate = 0.0;
		this.gcDisplayinCalendar.CurrencyRateField = null;
		this.gcDisplayinCalendar.FieldName = "DisplInCal";
		this.gcDisplayinCalendar.Name = "gcDisplayinCalendar";
		this.gcDisplayinCalendar.OptionsFilter.AllowAutoFilter = false;
		this.gcDisplayinCalendar.OptionsFilter.AllowFilter = false;
		this.gcDisplayinCalendar.PrimaryKey = false;
		this.gcDisplayinCalendar.UnitField = null;
		this.gcDisplayinCalendar.Visible = true;
		this.gcDisplayinCalendar.VisibleIndex = 7;
		this.rDisplayinCalendar.AutoHeight = false;
		this.rDisplayinCalendar.Name = "rDisplayinCalendar";
		this.rDisplayinCalendar.ValueChecked = "Y";
		this.rDisplayinCalendar.ValueUnchecked = "N";
		this.gcRemQty.Caption = "RemQty";
		this.gcRemQty.ColumnEdit = this.rRemQty;
		this.gcRemQty.CurrencyDateField = null;
		this.gcRemQty.CurrencyField = null;
		this.gcRemQty.CurrencyFixed = null;
		this.gcRemQty.CurrencyFixedRate = 0.0;
		this.gcRemQty.CurrencyRateField = null;
		this.gcRemQty.FieldName = "RemQty";
		this.gcRemQty.Name = "gcRemQty";
		this.gcRemQty.OptionsFilter.AllowAutoFilter = false;
		this.gcRemQty.OptionsFilter.AllowFilter = false;
		this.gcRemQty.PrimaryKey = false;
		this.gcRemQty.UnitField = null;
		this.gcRemQty.Visible = true;
		this.gcRemQty.VisibleIndex = 8;
		this.rRemQty.AutoHeight = false;
		this.rRemQty.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rRemQty.Name = "rRemQty";
		this.gcRemType.Caption = "RemQty Type";
		this.gcRemType.ColumnEdit = this.rRemType;
		this.gcRemType.CurrencyDateField = null;
		this.gcRemType.CurrencyField = null;
		this.gcRemType.CurrencyFixed = null;
		this.gcRemType.CurrencyFixedRate = 0.0;
		this.gcRemType.CurrencyRateField = null;
		this.gcRemType.FieldName = "RemType";
		this.gcRemType.Name = "gcRemType";
		this.gcRemType.PrimaryKey = false;
		this.gcRemType.UnitField = null;
		this.gcRemType.Visible = true;
		this.gcRemType.VisibleIndex = 9;
		this.rRemType.AutoHeight = false;
		this.rRemType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rRemType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rRemType.DisplayMember = "Name";
		this.rRemType.Name = "rRemType";
		this.rRemType.NullText = "";
		this.rRemType.ShowFooter = false;
		this.rRemType.ShowHeader = false;
		this.rRemType.ValueMember = "ID";
		this.gcMeetingLocation.Caption = "Meeting Location";
		this.gcMeetingLocation.ColumnEdit = this.rMeetingLocation;
		this.gcMeetingLocation.CurrencyDateField = null;
		this.gcMeetingLocation.CurrencyField = null;
		this.gcMeetingLocation.CurrencyFixed = null;
		this.gcMeetingLocation.CurrencyFixedRate = 0.0;
		this.gcMeetingLocation.CurrencyRateField = null;
		this.gcMeetingLocation.FieldName = "Location";
		this.gcMeetingLocation.Name = "gcMeetingLocation";
		this.gcMeetingLocation.OptionsFilter.AllowAutoFilter = false;
		this.gcMeetingLocation.OptionsFilter.AllowFilter = false;
		this.gcMeetingLocation.PrimaryKey = false;
		this.gcMeetingLocation.UnitField = null;
		this.gcMeetingLocation.Visible = true;
		this.gcMeetingLocation.VisibleIndex = 10;
		this.rMeetingLocation.AutoHeight = false;
		this.rMeetingLocation.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rMeetingLocation.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name17"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name18", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rMeetingLocation.DisplayMember = "Name";
		this.rMeetingLocation.Name = "rMeetingLocation";
		this.rMeetingLocation.NullText = "";
		this.rMeetingLocation.ShowFooter = false;
		this.rMeetingLocation.ShowHeader = false;
		this.rMeetingLocation.ValueMember = "ID";
		this.gcAddressID.Caption = "Address ID";
		this.gcAddressID.ColumnEdit = this.rAddressID;
		this.gcAddressID.CurrencyDateField = null;
		this.gcAddressID.CurrencyField = null;
		this.gcAddressID.CurrencyFixed = null;
		this.gcAddressID.CurrencyFixedRate = 0.0;
		this.gcAddressID.CurrencyRateField = null;
		this.gcAddressID.FieldName = "AddrName";
		this.gcAddressID.Name = "gcAddressID";
		this.gcAddressID.OptionsFilter.AllowAutoFilter = false;
		this.gcAddressID.OptionsFilter.AllowFilter = false;
		this.gcAddressID.PrimaryKey = false;
		this.gcAddressID.UnitField = null;
		this.rAddressID.AutoHeight = false;
		this.rAddressID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rAddressID.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name3", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rAddressID.DisplayMember = "Name";
		this.rAddressID.Name = "rAddressID";
		this.rAddressID.NullText = "";
		this.rAddressID.ShowFooter = false;
		this.rAddressID.ShowHeader = false;
		this.rAddressID.ValueMember = "ID";
		this.rAddressID.Closed += new DevExpress.XtraEditors.Controls.ClosedEventHandler(this.rAddressID_Closed);
		this.AddrType.Caption = "AddrType";
		this.AddrType.CurrencyDateField = null;
		this.AddrType.CurrencyField = null;
		this.AddrType.CurrencyFixed = null;
		this.AddrType.CurrencyFixedRate = 0.0;
		this.AddrType.CurrencyRateField = null;
		this.AddrType.FieldName = "AddrType";
		this.AddrType.Name = "AddrType";
		this.AddrType.PrimaryKey = false;
		this.AddrType.UnitField = null;
		this.gcStreet.Caption = "Street";
		this.gcStreet.CurrencyDateField = null;
		this.gcStreet.CurrencyField = null;
		this.gcStreet.CurrencyFixed = null;
		this.gcStreet.CurrencyFixedRate = 0.0;
		this.gcStreet.CurrencyRateField = null;
		this.gcStreet.FieldName = "Street";
		this.gcStreet.Name = "gcStreet";
		this.gcStreet.OptionsFilter.AllowAutoFilter = false;
		this.gcStreet.OptionsFilter.AllowFilter = false;
		this.gcStreet.PrimaryKey = false;
		this.gcStreet.UnitField = null;
		this.gcStreet.Visible = true;
		this.gcStreet.VisibleIndex = 11;
		this.gcCity.Caption = "City";
		this.gcCity.CurrencyDateField = null;
		this.gcCity.CurrencyField = null;
		this.gcCity.CurrencyFixed = null;
		this.gcCity.CurrencyFixedRate = 0.0;
		this.gcCity.CurrencyRateField = null;
		this.gcCity.FieldName = "City";
		this.gcCity.Name = "gcCity";
		this.gcCity.OptionsFilter.AllowAutoFilter = false;
		this.gcCity.OptionsFilter.AllowFilter = false;
		this.gcCity.PrimaryKey = false;
		this.gcCity.UnitField = null;
		this.gcCity.Visible = true;
		this.gcCity.VisibleIndex = 12;
		this.gcRoom.Caption = "Room";
		this.gcRoom.CurrencyDateField = null;
		this.gcRoom.CurrencyField = null;
		this.gcRoom.CurrencyFixed = null;
		this.gcRoom.CurrencyFixedRate = 0.0;
		this.gcRoom.CurrencyRateField = null;
		this.gcRoom.FieldName = "Room";
		this.gcRoom.Name = "gcRoom";
		this.gcRoom.OptionsFilter.AllowAutoFilter = false;
		this.gcRoom.OptionsFilter.AllowFilter = false;
		this.gcRoom.PrimaryKey = false;
		this.gcRoom.UnitField = null;
		this.gcRoom.Visible = true;
		this.gcRoom.VisibleIndex = 13;
		this.gcState.Caption = "State";
		this.gcState.ColumnEdit = this.rState;
		this.gcState.CurrencyDateField = null;
		this.gcState.CurrencyField = null;
		this.gcState.CurrencyFixed = null;
		this.gcState.CurrencyFixedRate = 0.0;
		this.gcState.CurrencyRateField = null;
		this.gcState.FieldName = "State";
		this.gcState.Name = "gcState";
		this.gcState.OptionsFilter.AllowAutoFilter = false;
		this.gcState.OptionsFilter.AllowFilter = false;
		this.gcState.PrimaryKey = false;
		this.gcState.UnitField = null;
		this.gcState.Visible = true;
		this.gcState.VisibleIndex = 14;
		this.rState.AutoHeight = false;
		this.rState.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rState.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rState.DisplayMember = "Name";
		this.rState.Name = "rState";
		this.rState.NullText = "";
		this.rState.ShowFooter = false;
		this.rState.ShowHeader = false;
		this.rState.ValueMember = "ID";
		this.gcCountry.Caption = "Country";
		this.gcCountry.ColumnEdit = this.rCountry;
		this.gcCountry.CurrencyDateField = null;
		this.gcCountry.CurrencyField = null;
		this.gcCountry.CurrencyFixed = null;
		this.gcCountry.CurrencyFixedRate = 0.0;
		this.gcCountry.CurrencyRateField = null;
		this.gcCountry.FieldName = "Country";
		this.gcCountry.Name = "gcCountry";
		this.gcCountry.OptionsFilter.AllowAutoFilter = false;
		this.gcCountry.OptionsFilter.AllowFilter = false;
		this.gcCountry.PrimaryKey = false;
		this.gcCountry.UnitField = null;
		this.gcCountry.Visible = true;
		this.gcCountry.VisibleIndex = 15;
		this.rCountry.AutoHeight = false;
		this.rCountry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCountry.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rCountry.DisplayMember = "Name";
		this.rCountry.Name = "rCountry";
		this.rCountry.NullText = "";
		this.rCountry.ShowFooter = false;
		this.rCountry.ShowHeader = false;
		this.rCountry.ValueMember = "ID";
		this.rCountry.EditValueChanged += new System.EventHandler(this.rCountry_EditValueChanged);
		this.tabAttachments.Controls.Add(this.ucAttachment1);
		this.tabAttachments.EditControl = null;
		this.tabAttachments.EditValue = "Attachments";
		this.tabAttachments.FieldName = null;
		this.tabAttachments.IsSearchControl = false;
		this.tabAttachments.Name = "tabAttachments";
		this.tabAttachments.ReadOnly = false;
		this.tabAttachments.SettingInVisible = false;
		this.tabAttachments.SettingReadonly = false;
		this.tabAttachments.Size = new System.Drawing.Size(918, 212);
		this.tabAttachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabAttachments.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oServiceCall";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(918, 212);
		this.ucAttachment1.TabIndex = 0;
		this.ucAttachment1.ButtonBrowse_Click += new System.EventHandler(this.ucAttachment1_ButtonBrowse_Click);
		this.ucAttachment1.ButtonDelete_Click += new System.EventHandler(this.ucAttachment1_ButtonBrowse_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuDoc;
		this.grdDoc.Location = new System.Drawing.Point(-5, -1);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[14]
		{
			this.rCustomer, this.rName, this.rContactPerson, this.rMfrSerialNo, this.rSerialNumber, this.rItem, this.rDescription, this.rNo, this.rCallStatus, this.rPriority,
			this.rCreatedDate, this.rItemGroup, this.rContractNo, this.rCreateTime
		});
		this.grdDoc.Size = new System.Drawing.Size(920, 247);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addToolStripMenuItem });
		this.contextMenuDoc.Name = "contextMenuDoc";
		this.contextMenuDoc.Size = new System.Drawing.Size(97, 26);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedCell.BackColor = System.Drawing.Color.WhiteSmoke;
		this.gvDoc.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.NavajoWhite;
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[23]
		{
			this.gcCallID, this.gcSubject, this.gcCustomer, this.gcName, this.gvContactPerson, this.gcTelephoneNo, this.gcCustomerRefNo, this.gcMfrSerialNo, this.gcSerialNumber, this.gcItem,
			this.gcDescription, this.gcItemGroup, this.gcNo, this.gcDocNum, this.gcCallStatus, this.gcPriority, this.gcCreatedDate, this.gcCreateTime, this.gcClosedDate, this.gcClosedTime,
			this.gcContractNo, this.gcEndDate, this.gridColumn2
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oServiceCall";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OSCL";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsCustomization.AllowSort = false;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEdit);
		this.gvDoc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEditForEditing);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvDoc_FocusedRowChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gcCallID.AppearanceCell.Options.UseTextOptions = true;
		this.gcCallID.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.gcCallID.Caption = "Call ID";
		this.gcCallID.CurrencyDateField = null;
		this.gcCallID.CurrencyField = null;
		this.gcCallID.CurrencyFixed = null;
		this.gcCallID.CurrencyFixedRate = 0.0;
		this.gcCallID.CurrencyRateField = null;
		this.gcCallID.FieldName = "callID";
		this.gcCallID.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcCallID.Name = "gcCallID";
		this.gcCallID.OptionsColumn.AllowEdit = false;
		this.gcCallID.OptionsColumn.ReadOnly = true;
		this.gcCallID.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcCallID.PrimaryKey = true;
		this.gcCallID.UnitField = null;
		this.gcCallID.Width = 46;
		this.gcSubject.Caption = "Subject";
		this.gcSubject.CurrencyDateField = null;
		this.gcSubject.CurrencyField = null;
		this.gcSubject.CurrencyFixed = null;
		this.gcSubject.CurrencyFixedRate = 0.0;
		this.gcSubject.CurrencyRateField = null;
		this.gcSubject.FieldName = "subject";
		this.gcSubject.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcSubject.Name = "gcSubject";
		this.gcSubject.OptionsFilter.AllowAutoFilter = false;
		this.gcSubject.OptionsFilter.AllowFilter = false;
		this.gcSubject.PrimaryKey = false;
		this.gcSubject.UnitField = null;
		this.gcSubject.Visible = true;
		this.gcSubject.VisibleIndex = 0;
		this.gcCustomer.Caption = "Customer";
		this.gcCustomer.ColumnEdit = this.rCustomer;
		this.gcCustomer.CurrencyDateField = null;
		this.gcCustomer.CurrencyField = null;
		this.gcCustomer.CurrencyFixed = null;
		this.gcCustomer.CurrencyFixedRate = 0.0;
		this.gcCustomer.CurrencyRateField = null;
		this.gcCustomer.FieldName = "customer";
		this.gcCustomer.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcCustomer.Name = "gcCustomer";
		this.gcCustomer.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcCustomer.PrimaryKey = false;
		this.gcCustomer.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcCustomer.UnitField = null;
		this.gcCustomer.Visible = true;
		this.gcCustomer.VisibleIndex = 1;
		this.rCustomer.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rCustomer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rCustomer.Name = "rCustomer";
		this.rCustomer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rCustomer_ButtonClick);
		this.gcName.Caption = "Name";
		this.gcName.ColumnEdit = this.rName;
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "custmrName";
		this.gcName.Name = "gcName";
		this.gcName.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcName.PrimaryKey = false;
		this.gcName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedCell;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 2;
		this.rName.AutoHeight = false;
		this.rName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rName.Name = "rName";
		this.rName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rName_ButtonClick);
		this.gvContactPerson.Caption = "Contact Person";
		this.gvContactPerson.CurrencyDateField = null;
		this.gvContactPerson.CurrencyField = null;
		this.gvContactPerson.CurrencyFixed = null;
		this.gvContactPerson.CurrencyFixedRate = 0.0;
		this.gvContactPerson.CurrencyRateField = null;
		this.gvContactPerson.FieldName = "contctCode";
		this.gvContactPerson.Name = "gvContactPerson";
		this.gvContactPerson.OptionsFilter.AllowAutoFilter = false;
		this.gvContactPerson.OptionsFilter.AllowFilter = false;
		this.gvContactPerson.PrimaryKey = false;
		this.gvContactPerson.UnitField = null;
		this.gvContactPerson.Visible = true;
		this.gvContactPerson.VisibleIndex = 3;
		this.gvContactPerson.Width = 96;
		this.gcTelephoneNo.Caption = "Telephone No.";
		this.gcTelephoneNo.CurrencyDateField = null;
		this.gcTelephoneNo.CurrencyField = null;
		this.gcTelephoneNo.CurrencyFixed = null;
		this.gcTelephoneNo.CurrencyFixedRate = 0.0;
		this.gcTelephoneNo.CurrencyRateField = null;
		this.gcTelephoneNo.FieldName = "TelephoneNo";
		this.gcTelephoneNo.Name = "gcTelephoneNo";
		this.gcTelephoneNo.OptionsFilter.AllowAutoFilter = false;
		this.gcTelephoneNo.OptionsFilter.AllowFilter = false;
		this.gcTelephoneNo.PrimaryKey = false;
		this.gcTelephoneNo.UnitField = null;
		this.gcTelephoneNo.Visible = true;
		this.gcTelephoneNo.VisibleIndex = 4;
		this.gcTelephoneNo.Width = 88;
		this.gcCustomerRefNo.Caption = "Customer Ref. No.";
		this.gcCustomerRefNo.CurrencyDateField = null;
		this.gcCustomerRefNo.CurrencyField = null;
		this.gcCustomerRefNo.CurrencyFixed = null;
		this.gcCustomerRefNo.CurrencyFixedRate = 0.0;
		this.gcCustomerRefNo.CurrencyRateField = null;
		this.gcCustomerRefNo.FieldName = "NumAtCard";
		this.gcCustomerRefNo.Name = "gcCustomerRefNo";
		this.gcCustomerRefNo.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcCustomerRefNo.PrimaryKey = false;
		this.gcCustomerRefNo.UnitField = null;
		this.gcCustomerRefNo.Visible = true;
		this.gcCustomerRefNo.VisibleIndex = 5;
		this.gcCustomerRefNo.Width = 98;
		this.gcMfrSerialNo.Caption = "Mfr Serial No.";
		this.gcMfrSerialNo.ColumnEdit = this.rSerialNumber;
		this.gcMfrSerialNo.CurrencyDateField = null;
		this.gcMfrSerialNo.CurrencyField = null;
		this.gcMfrSerialNo.CurrencyFixed = null;
		this.gcMfrSerialNo.CurrencyFixedRate = 0.0;
		this.gcMfrSerialNo.CurrencyRateField = null;
		this.gcMfrSerialNo.FieldName = "manufSN";
		this.gcMfrSerialNo.Name = "gcMfrSerialNo";
		this.gcMfrSerialNo.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcMfrSerialNo.PrimaryKey = false;
		this.gcMfrSerialNo.UnitField = null;
		this.gcMfrSerialNo.Visible = true;
		this.gcMfrSerialNo.VisibleIndex = 6;
		this.rSerialNumber.AutoHeight = false;
		this.rSerialNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rSerialNumber.Name = "rSerialNumber";
		this.rSerialNumber.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rSerialNumber_ButtonClick);
		this.gcSerialNumber.Caption = "Serial Number";
		this.gcSerialNumber.ColumnEdit = this.rSerialNumber;
		this.gcSerialNumber.CurrencyDateField = null;
		this.gcSerialNumber.CurrencyField = null;
		this.gcSerialNumber.CurrencyFixed = null;
		this.gcSerialNumber.CurrencyFixedRate = 0.0;
		this.gcSerialNumber.CurrencyRateField = null;
		this.gcSerialNumber.FieldName = "internalSN";
		this.gcSerialNumber.Name = "gcSerialNumber";
		this.gcSerialNumber.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcSerialNumber.PrimaryKey = false;
		this.gcSerialNumber.UnitField = null;
		this.gcSerialNumber.Visible = true;
		this.gcSerialNumber.VisibleIndex = 7;
		this.gcItem.Caption = "Item";
		this.gcItem.ColumnEdit = this.rItem;
		this.gcItem.CurrencyDateField = null;
		this.gcItem.CurrencyField = null;
		this.gcItem.CurrencyFixed = null;
		this.gcItem.CurrencyFixedRate = 0.0;
		this.gcItem.CurrencyRateField = null;
		this.gcItem.FieldName = "itemCode";
		this.gcItem.Name = "gcItem";
		this.gcItem.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcItem.PrimaryKey = false;
		this.gcItem.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItem.UnitField = null;
		this.gcItem.Visible = true;
		this.gcItem.VisibleIndex = 8;
		this.gcItem.Width = 82;
		this.rItem.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rItem.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItem.Name = "rItem";
		this.rItem.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItem_ButtonClick);
		this.gcDescription.Caption = "Description";
		this.gcDescription.ColumnEdit = this.rDescription;
		this.gcDescription.CurrencyDateField = null;
		this.gcDescription.CurrencyField = null;
		this.gcDescription.CurrencyFixed = null;
		this.gcDescription.CurrencyFixedRate = 0.0;
		this.gcDescription.CurrencyRateField = null;
		this.gcDescription.FieldName = "itemName";
		this.gcDescription.Name = "gcDescription";
		this.gcDescription.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcDescription.PrimaryKey = false;
		this.gcDescription.UnitField = null;
		this.gcDescription.Visible = true;
		this.gcDescription.VisibleIndex = 9;
		this.rDescription.AutoHeight = false;
		this.rDescription.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rDescription.Name = "rDescription";
		this.rDescription.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rDescription_ButtonClick);
		this.gcItemGroup.Caption = "Item Group";
		this.gcItemGroup.ColumnEdit = this.rItemGroup;
		this.gcItemGroup.CurrencyDateField = null;
		this.gcItemGroup.CurrencyField = null;
		this.gcItemGroup.CurrencyFixed = null;
		this.gcItemGroup.CurrencyFixedRate = 0.0;
		this.gcItemGroup.CurrencyRateField = null;
		this.gcItemGroup.FieldName = "itemGroup";
		this.gcItemGroup.Name = "gcItemGroup";
		this.gcItemGroup.OptionsColumn.AllowEdit = false;
		this.gcItemGroup.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcItemGroup.PrimaryKey = false;
		this.gcItemGroup.UnitField = null;
		this.gcItemGroup.Visible = true;
		this.gcItemGroup.VisibleIndex = 10;
		this.rItemGroup.AutoHeight = false;
		this.rItemGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rItemGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rItemGroup.DisplayMember = "Name";
		this.rItemGroup.Name = "rItemGroup";
		this.rItemGroup.NullText = "";
		this.rItemGroup.ShowFooter = false;
		this.rItemGroup.ShowHeader = false;
		this.rItemGroup.ValueMember = "ID";
		this.gcNo.Caption = "No.";
		this.gcNo.ColumnEdit = this.rNo;
		this.gcNo.CurrencyDateField = null;
		this.gcNo.CurrencyField = null;
		this.gcNo.CurrencyFixed = null;
		this.gcNo.CurrencyFixedRate = 0.0;
		this.gcNo.CurrencyRateField = null;
		this.gcNo.FieldName = "Series";
		this.gcNo.Name = "gcNo";
		this.gcNo.OptionsFilter.AllowAutoFilter = false;
		this.gcNo.OptionsFilter.AllowFilter = false;
		this.gcNo.PrimaryKey = false;
		this.gcNo.UnitField = null;
		this.gcNo.Visible = true;
		this.gcNo.VisibleIndex = 11;
		this.gcNo.Width = 58;
		this.rNo.AutoHeight = false;
		this.rNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rNo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rNo.DisplayMember = "Name";
		this.rNo.Name = "rNo";
		this.rNo.NullText = "";
		this.rNo.ShowFooter = false;
		this.rNo.ShowHeader = false;
		this.rNo.ValueMember = "ID";
		this.rNo.EditValueChanged += new System.EventHandler(this.rNo_EditValueChanged);
		this.gcDocNum.Caption = "DocNum";
		this.gcDocNum.CurrencyDateField = null;
		this.gcDocNum.CurrencyField = null;
		this.gcDocNum.CurrencyFixed = null;
		this.gcDocNum.CurrencyFixedRate = 0.0;
		this.gcDocNum.CurrencyRateField = null;
		this.gcDocNum.FieldName = "DocNum";
		this.gcDocNum.Name = "gcDocNum";
		this.gcDocNum.OptionsFilter.AllowAutoFilter = false;
		this.gcDocNum.OptionsFilter.AllowFilter = false;
		this.gcDocNum.PrimaryKey = false;
		this.gcDocNum.UnitField = null;
		this.gcDocNum.Visible = true;
		this.gcDocNum.VisibleIndex = 12;
		this.gcCallStatus.Caption = "Call Status";
		this.gcCallStatus.ColumnEdit = this.rCallStatus;
		this.gcCallStatus.CurrencyDateField = null;
		this.gcCallStatus.CurrencyField = null;
		this.gcCallStatus.CurrencyFixed = null;
		this.gcCallStatus.CurrencyFixedRate = 0.0;
		this.gcCallStatus.CurrencyRateField = null;
		this.gcCallStatus.FieldName = "status";
		this.gcCallStatus.Name = "gcCallStatus";
		this.gcCallStatus.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcCallStatus.PrimaryKey = false;
		this.gcCallStatus.UnitField = null;
		this.gcCallStatus.Visible = true;
		this.gcCallStatus.VisibleIndex = 13;
		this.rCallStatus.AutoHeight = false;
		this.rCallStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCallStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name8", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rCallStatus.DisplayMember = "Name";
		this.rCallStatus.Name = "rCallStatus";
		this.rCallStatus.ShowFooter = false;
		this.rCallStatus.ShowHeader = false;
		this.rCallStatus.ValueMember = "ID";
		this.rCallStatus.EditValueChanged += new System.EventHandler(this.rCallStatus_EditValueChanged);
		this.gcPriority.Caption = "Priority";
		this.gcPriority.ColumnEdit = this.rPriority;
		this.gcPriority.CurrencyDateField = null;
		this.gcPriority.CurrencyField = null;
		this.gcPriority.CurrencyFixed = null;
		this.gcPriority.CurrencyFixedRate = 0.0;
		this.gcPriority.CurrencyRateField = null;
		this.gcPriority.FieldName = "priority";
		this.gcPriority.Name = "gcPriority";
		this.gcPriority.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcPriority.PrimaryKey = false;
		this.gcPriority.UnitField = null;
		this.gcPriority.Visible = true;
		this.gcPriority.VisibleIndex = 14;
		this.rPriority.AutoHeight = false;
		this.rPriority.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rPriority.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name9"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name10", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rPriority.DisplayMember = "Name";
		this.rPriority.Name = "rPriority";
		this.rPriority.ShowFooter = false;
		this.rPriority.ShowHeader = false;
		this.rPriority.ValueMember = "ID";
		this.gcCreatedDate.Caption = "Created Date";
		this.gcCreatedDate.ColumnEdit = this.rCreatedDate;
		this.gcCreatedDate.CurrencyDateField = null;
		this.gcCreatedDate.CurrencyField = null;
		this.gcCreatedDate.CurrencyFixed = null;
		this.gcCreatedDate.CurrencyFixedRate = 0.0;
		this.gcCreatedDate.CurrencyRateField = null;
		this.gcCreatedDate.DisplayFormat.FormatString = "d";
		this.gcCreatedDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcCreatedDate.FieldName = "createDate";
		this.gcCreatedDate.Name = "gcCreatedDate";
		this.gcCreatedDate.OptionsFilter.AllowAutoFilter = false;
		this.gcCreatedDate.OptionsFilter.AllowFilter = false;
		this.gcCreatedDate.PrimaryKey = false;
		this.gcCreatedDate.UnitField = null;
		this.gcCreatedDate.Visible = true;
		this.gcCreatedDate.VisibleIndex = 15;
		this.rCreatedDate.AutoHeight = false;
		this.rCreatedDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCreatedDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCreatedDate.Name = "rCreatedDate";
		this.gcCreateTime.Caption = "Create Time";
		this.gcCreateTime.ColumnEdit = this.rCreateTime;
		this.gcCreateTime.CurrencyDateField = null;
		this.gcCreateTime.CurrencyField = null;
		this.gcCreateTime.CurrencyFixed = null;
		this.gcCreateTime.CurrencyFixedRate = 0.0;
		this.gcCreateTime.CurrencyRateField = null;
		this.gcCreateTime.FieldName = "createTime";
		this.gcCreateTime.Name = "gcCreateTime";
		this.gcCreateTime.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcCreateTime.PrimaryKey = false;
		this.gcCreateTime.UnitField = null;
		this.gcCreateTime.Visible = true;
		this.gcCreateTime.VisibleIndex = 16;
		this.rCreateTime.AutoHeight = false;
		this.rCreateTime.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCreateTime.DisplayFormat.FormatString = "hh:mm";
		this.rCreateTime.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rCreateTime.Mask.EditMask = "HH:mm";
		this.rCreateTime.Name = "rCreateTime";
		this.rCreateTime.EditValueChanged += new System.EventHandler(this.rCreateTime_EditValueChanged);
		this.gcClosedDate.Caption = "Closed Date";
		this.gcClosedDate.ColumnEdit = this.rCreatedDate;
		this.gcClosedDate.CurrencyDateField = null;
		this.gcClosedDate.CurrencyField = null;
		this.gcClosedDate.CurrencyFixed = null;
		this.gcClosedDate.CurrencyFixedRate = 0.0;
		this.gcClosedDate.CurrencyRateField = null;
		this.gcClosedDate.DisplayFormat.FormatString = "d";
		this.gcClosedDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcClosedDate.FieldName = "closeDate";
		this.gcClosedDate.Name = "gcClosedDate";
		this.gcClosedDate.OptionsColumn.AllowEdit = false;
		this.gcClosedDate.OptionsColumn.ReadOnly = true;
		this.gcClosedDate.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcClosedDate.PrimaryKey = false;
		this.gcClosedDate.UnitField = null;
		this.gcClosedDate.Visible = true;
		this.gcClosedDate.VisibleIndex = 17;
		this.gcClosedTime.Caption = "Closed Time";
		this.gcClosedTime.ColumnEdit = this.rCreateTime;
		this.gcClosedTime.CurrencyDateField = null;
		this.gcClosedTime.CurrencyField = null;
		this.gcClosedTime.CurrencyFixed = null;
		this.gcClosedTime.CurrencyFixedRate = 0.0;
		this.gcClosedTime.CurrencyRateField = null;
		this.gcClosedTime.FieldName = "closeTime";
		this.gcClosedTime.Name = "gcClosedTime";
		this.gcClosedTime.OptionsColumn.AllowEdit = false;
		this.gcClosedTime.OptionsColumn.ReadOnly = true;
		this.gcClosedTime.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcClosedTime.PrimaryKey = false;
		this.gcClosedTime.UnitField = null;
		this.gcClosedTime.Visible = true;
		this.gcClosedTime.VisibleIndex = 18;
		this.gcContractNo.Caption = "Contract No.";
		this.gcContractNo.ColumnEdit = this.rContractNo;
		this.gcContractNo.CurrencyDateField = null;
		this.gcContractNo.CurrencyField = null;
		this.gcContractNo.CurrencyFixed = null;
		this.gcContractNo.CurrencyFixedRate = 0.0;
		this.gcContractNo.CurrencyRateField = null;
		this.gcContractNo.FieldName = "contractID";
		this.gcContractNo.Name = "gcContractNo";
		this.gcContractNo.OptionsFilter.AllowAutoFilter = false;
		this.gcContractNo.OptionsFilter.AllowFilter = false;
		this.gcContractNo.PrimaryKey = false;
		this.gcContractNo.UnitField = null;
		this.gcContractNo.Visible = true;
		this.gcContractNo.VisibleIndex = 19;
		this.rContractNo.AutoHeight = false;
		this.rContractNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rContractNo.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rContractNo.DisplayMember = "Name";
		this.rContractNo.Name = "rContractNo";
		this.rContractNo.NullText = "";
		this.rContractNo.ShowFooter = false;
		this.rContractNo.ShowHeader = false;
		this.rContractNo.ValueMember = "ID";
		this.gcEndDate.Caption = "Contract End Date";
		this.gcEndDate.ColumnEdit = this.rCreatedDate;
		this.gcEndDate.CurrencyDateField = null;
		this.gcEndDate.CurrencyField = null;
		this.gcEndDate.CurrencyFixed = null;
		this.gcEndDate.CurrencyFixedRate = 0.0;
		this.gcEndDate.CurrencyRateField = null;
		this.gcEndDate.DisplayFormat.FormatString = "d";
		this.gcEndDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcEndDate.FieldName = "cntrctDate";
		this.gcEndDate.Name = "gcEndDate";
		this.gcEndDate.OptionsColumn.AllowEdit = false;
		this.gcEndDate.OptionsColumn.ReadOnly = true;
		this.gcEndDate.OptionsFilter.AllowAutoFilter = false;
		this.gcEndDate.OptionsFilter.AllowFilter = false;
		this.gcEndDate.PrimaryKey = false;
		this.gcEndDate.UnitField = null;
		this.gcEndDate.Visible = true;
		this.gcEndDate.VisibleIndex = 20;
		this.gridColumn2.Caption = "AtcEntry";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "AtcEntry";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.rContactPerson.AutoHeight = false;
		this.rContactPerson.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rContactPerson.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CntctName", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CntctCode", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rContactPerson.DisplayMember = "CntctName";
		this.rContactPerson.Name = "rContactPerson";
		this.rContactPerson.NullText = "";
		this.rContactPerson.ShowFooter = false;
		this.rContactPerson.ShowHeader = false;
		this.rContactPerson.ValueMember = "CntctCode";
		this.rContactPerson.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rContactPerson_EditValueChanging);
		this.rMfrSerialNo.AutoHeight = false;
		this.rMfrSerialNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rMfrSerialNo.Name = "rMfrSerialNo";
		this.rMfrSerialNo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rMfrSerialNo_ButtonClick);
		this.btnActivities.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnActivities.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnActivities.Appearance.Options.UseBackColor = true;
		this.btnActivities.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnActivities.EditControl = null;
		this.btnActivities.EditValue = "Activities";
		this.btnActivities.FieldName = null;
		this.btnActivities.IsSearchControl = false;
		this.btnActivities.Location = new System.Drawing.Point(841, 562);
		this.btnActivities.Name = "btnActivities";
		this.btnActivities.ReadOnly = false;
		this.btnActivities.SettingInVisible = false;
		this.btnActivities.SettingReadonly = false;
		this.btnActivities.Size = new System.Drawing.Size(75, 23);
		this.btnActivities.TabIndex = 14;
		this.btnActivities.Text = "Activities";
		this.btnActivities.Visible = false;
		this.btnActivities.Click += new System.EventHandler(this.btnActivities_Click);
		this.btnRecommend.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnRecommend.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnRecommend.Appearance.Options.UseBackColor = true;
		this.btnRecommend.ButtonStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.btnRecommend.EditControl = null;
		this.btnRecommend.EditValue = "Recommned";
		this.btnRecommend.FieldName = null;
		this.btnRecommend.IsSearchControl = false;
		this.btnRecommend.Location = new System.Drawing.Point(758, 562);
		this.btnRecommend.Name = "btnRecommend";
		this.btnRecommend.ReadOnly = false;
		this.btnRecommend.SettingInVisible = false;
		this.btnRecommend.SettingReadonly = false;
		this.btnRecommend.Size = new System.Drawing.Size(75, 23);
		this.btnRecommend.TabIndex = 15;
		this.btnRecommend.Text = "Recommned";
		this.btnRecommend.Visible = false;
		this.btnRecommend.Click += new System.EventHandler(this.btnRecommend_Click);
		this.cbDisplay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.cbDisplay.AutoSize = true;
		this.cbDisplay.Location = new System.Drawing.Point(618, 568);
		this.cbDisplay.Name = "cbDisplay";
		this.cbDisplay.Size = new System.Drawing.Size(130, 17);
		this.cbDisplay.TabIndex = 16;
		this.cbDisplay.Text = "Displa&y All Documents";
		this.cbDisplay.UseVisualStyleBackColor = true;
		this.cbDisplay.Visible = false;
		this.txtCallStatus.Location = new System.Drawing.Point(535, 9);
		this.txtCallStatus.Name = "txtCallStatus";
		this.txtCallStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCallStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtCallStatus.Properties.DisplayMember = "Name";
		this.txtCallStatus.Properties.NullText = "";
		this.txtCallStatus.Properties.PopupSizeable = false;
		this.txtCallStatus.Properties.ShowFooter = false;
		this.txtCallStatus.Properties.ShowHeader = false;
		this.txtCallStatus.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.txtCallStatus.Properties.ValueMember = "ID";
		this.txtCallStatus.Size = new System.Drawing.Size(120, 20);
		this.txtCallStatus.TabIndex = 9;
		this.txtPriority.Location = new System.Drawing.Point(305, 9);
		this.txtPriority.Name = "txtPriority";
		this.txtPriority.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtPriority.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name7"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name8", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtPriority.Properties.DisplayMember = "Name";
		this.txtPriority.Properties.NullText = "";
		this.txtPriority.Properties.ShowFooter = false;
		this.txtPriority.Properties.ShowHeader = false;
		this.txtPriority.Properties.ValueMember = "ID";
		this.txtPriority.Size = new System.Drawing.Size(120, 20);
		this.txtPriority.TabIndex = 17;
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(100, 9);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(120, 20);
		this.txtFromDate.TabIndex = 5;
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(100, 36);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(120, 20);
		this.txtToDate.TabIndex = 6;
		this.txtToDate.EditValueChanged += new System.EventHandler(this.txtToDate_EditValueChanged);
		base.AcceptButton = this.btnSave;
		base.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(928, 597);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.txtPriority);
		base.Controls.Add(this.cbDisplay);
		base.Controls.Add(this.btnRecommend);
		base.Controls.Add(this.btnActivities);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txtCustomer);
		base.Controls.Add(this.lbCallStatus);
		base.Controls.Add(this.lbCustomer);
		base.Controls.Add(this.lbToDate);
		base.Controls.Add(this.lbPriority);
		base.Controls.Add(this.lbFromDate);
		base.Controls.Add(this.txtCallStatus);
		base.FormId = "oServiceCall";
		base.IconOptions.ShowIcon = false;
		base.Name = "Frm_ServiceCall";
		base.ShowInTaskbar = false;
		this.Text = "Service Call";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ServiceCall_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.txtCustomer.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tabLine).EndInit();
		this.tabLine.ResumeLayout(false);
		this.tabGeneral.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdGeneral).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvGeneral).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOrigin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProblemType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProblemSubtype).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCallType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTechnician).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIsQueue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rHandledByInGeneral).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rQueue).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDateInGerenal.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDateInGerenal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTimeInGeneral).EndInit();
		this.tabBP.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdBusinessPartner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvBusinessPartner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBPProjCode).EndInit();
		this.tabRemarks.ResumeLayout(false);
		this.tabRemarks.PerformLayout();
		this.tabActivities.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdActivities).EndInit();
		this.contextMenuActivities.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvActivities).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rHandledby).EndInit();
		this.tabSolutions.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdSolutions).EndInit();
		this.contextMenuSolution.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvSolutions).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOwner).EndInit();
		this.tabExpenses.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdExpenses).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvExpenses).EndInit();
		this.tabResolution.ResumeLayout(false);
		this.tabResolution.PerformLayout();
		this.tabHistory.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdHistory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvHistory).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUpdatedbyInHistory).EndInit();
		this.tabScheduling.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdScheduling).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvScheduling).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStartTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEndtime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDuration).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDurType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rReminder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDisplayinCalendar).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRemQty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRemType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rMeetingLocation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rAddressID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rState).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCountry).EndInit();
		this.tabAttachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCustomer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSerialNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItem).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDescription).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCallStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriority).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCreatedDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCreatedDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCreateTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rContractNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rContactPerson).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rMfrSerialNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCallStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPriority.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
