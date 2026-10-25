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
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.SystemInitialization;

public class frm_DocumentSettings : XtraFormEx
{
	private DataTable dtOADM;

	private DataTable dtCINF;

	private DataTable dtADP1;

	private DataTable dtOADP;

	private bool isLoading;

	private bool isCancel;

	private IContainer components;

	private CheckEditEx chk_BlockZeroQ;

	private XtraTabControlEx tabControlDocumentSettings;

	private XtraTabPageEx tabGeneral;

	private XtraTabPageEx tabPerDocument;

	private SimpleButtonEx btnOK;

	private LabelLine labelLine1;

	private RadioGroupEx rdg_LevelWarn;

	private LookUpEdit luk_NegStoLv;

	private CheckEditEx chk_AllowFuPos;

	private CheckEditEx chk_OrderBlock;

	private LookUpEdit cbObjList;

	private XtraTabControlEx tabControlPerDoc;

	private XtraTabPageEx xtraPageSalesQuotation;

	private XtraTabPageEx xtraPageSalesOrder;

	private CheckEditEx chk_ChkQunty;

	private CheckEditEx chk_ChangeRdr;

	private XtraTabPageEx xtraPagePurchaseRequest;

	private XtraTabPageEx xtraPagePurchaseQuotation;

	private XtraTabPageEx xtraPagePurchaseOrder;

	private CheckEditEx chk_ForceReOrdSale;

	private CheckEditEx chk_ReopOrderSale;

	private CheckEditEx chk_CpyPRPrice;

	private CheckEditEx chk_ClosePQ;

	private CheckEditEx chk_ReopOrderPurchase;

	private XtraTabPageEx xtraPageInventoryCounting;

	private LabelLine labelLine6;

	private XtraTabPageEx xtraPageInventoryPosting;

	private LabelLine labelLine9;

	private Label label3;

	private LabelLine labelLine8;

	private Label label2;

	private LabelLine labelLine7;

	private CheckEditEx chk_ClsNoConfi;

	private CheckEditEx chk_ClsZoDiffR;

	private Label label4;

	private LabelLine labelLine10;

	private CheckEditEx chk_AllowPostZ;

	private CheckEditEx chk_ForceReOrdPurchase;

	private CheckEditEx chk_RevisionPo;

	private CheckEditEx chk_AllowFuPosSale;

	private CheckEditEx chk_ClosedQuot;

	private CalcEditEx txt_PostDiffR;

	private CalcEditEx txt_ICDifExPe1;

	private CalcEditEx txt_ICDifExPe3;

	private SimpleButtonEx btnCancel;

	private LabelLine lbDocument;

	private XtraTabPageEx xtraPageReturn;

	private LookUpEditEx txtReturnPriceList;

	private CheckEditEx txtReturnPriceListCheck;

	private CheckEditEx txtReturnEnableSettingCost;

	private CheckEditEx txtReturnAllowSettingItem;

	private CheckEditEx txtReturnBlockDocument;

	private CheckEditEx txtReturnAllowCopyCus;

	private RadioGroupEx txtReturnDuplicateCus;

	private LabelLine lbReturnTitle;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_DocumentSettings()
	{
		InitializeComponent();
		tabControlPerDoc.ShowTabHeader = DefaultBoolean.False;
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadData()
	{
		isLoading = true;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataSet dataSet = await HttpClient.DocumentSettingClient.GetDataForm();
			if (dataSet.Tables.Count > 0)
			{
				DataTable dataTable = dataSet.Tables[0];
				dtOADM = dataTable.Clone();
				DataTable dataTable2 = dataSet.Tables[1];
				dtCINF = dataTable2.Clone();
				DataTable dataTable3 = dataSet.Tables[2];
				dtADP1 = dataTable3.Clone();
				DataTable dataTable4 = dataSet.Tables[3];
				dtOADP = dataTable4.Clone();
				isLoading = true;
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.Rows[0];
					rdg_LevelWarn.EditValue = dataRow["LevelWarn"];
					chk_OrderBlock.EditValue = dataRow["OrderBlock"];
					chk_AllowFuPos.EditValue = dataRow["AllowFuPos"];
					chk_ClosedQuot.EditValue = dataRow["ClosedQuot"];
					chk_ChangeRdr.EditValue = dataRow["ChangeRdr"];
					chk_RevisionPo.EditValue = dataRow["RevisionPo"];
					txt_ICDifExPe1.EditValue = dataRow["ICDifExPe1"];
					txt_ICDifExPe3.EditValue = dataRow["ICDifExPe3"];
					chk_ClsZoDiffR.EditValue = dataRow["ClsZoDiffR"];
					chk_ClsNoConfi.EditValue = dataRow["ClsNoConfi"];
					chk_AllowPostZ.EditValue = dataRow["AllowPostZ"];
					txt_PostDiffR.EditValue = dataRow["PostDiffR"];
				}
				if (dataTable2.IsNotNull())
				{
					DataRow dataRow2 = dataTable2.Rows[0];
					chk_BlockZeroQ.EditValue = dataRow2["BlockZeroQ"];
					luk_NegStoLv.EditValue = dataRow2["NegStoLv"];
					chk_ChkQunty.EditValue = dataRow2["ChkQunty"];
				}
				if (dataTable3.IsNotNull())
				{
					DataRow dataRow3 = dataTable3.AsEnumerable().FirstOrDefault((DataRow p) => p.Field<string>("ObjType") == "23");
					chk_AllowFuPosSale.EditValue = dataRow3["AllowFuPos"];
					dataRow3 = dataTable3.AsEnumerable().FirstOrDefault((DataRow p) => p.Field<string>("ObjType") == "17");
					chk_ReopOrderSale.EditValue = dataRow3["ReopOrder"];
					chk_ForceReOrdSale.EditValue = dataRow3["ForceReOrd"];
					dataRow3 = dataTable3.AsEnumerable().FirstOrDefault((DataRow p) => p.Field<string>("ObjType") == "1470000113");
					chk_CpyPRPrice.EditValue = dataRow3["CpyPRPrice"];
					dataRow3 = dataTable3.AsEnumerable().FirstOrDefault((DataRow p) => p.Field<string>("ObjType") == "540000006");
					chk_ClosePQ.EditValue = dataRow3["ClosePQ"];
					dataRow3 = dataTable3.AsEnumerable().FirstOrDefault((DataRow p) => p.Field<string>("ObjType") == "22");
					chk_ReopOrderPurchase.EditValue = dataRow3["ReopOrder"];
					chk_ForceReOrdPurchase.EditValue = dataRow3["ForceReOrd"];
					DataRow dataRow4 = dataTable3.AsEnumerable().FirstOrDefault((DataRow p) => Function.ToString(p["ObjType"]).Equals("16"));
					if (dataRow4 != null)
					{
						txtReturnDuplicateCus.EditValue = Function.ToString(dataRow4["ChkDupRef"]);
						txtReturnAllowCopyCus.EditValue = Function.ToString(dataRow4["CpyCVRef"]);
						txtReturnBlockDocument.EditValue = Function.ToString(dataRow4["orderblock"]);
						txtReturnAllowSettingItem.EditValue = Function.ToString(dataRow4["Enitemcost"]);
						txtReturnEnableSettingCost.EditValue = Function.ToString(dataRow4["EnSetCost"]);
						txtReturnPriceListCheck.EditValue = Function.ToString(dataRow4["DftPLChk"]);
						txtReturnPriceList.EditValue = Function.ToString(dataRow4["DfltPLSel"]);
					}
				}
				if (dataTable4.IsNotNull())
				{
					DataRow dataRow5 = dataTable4.Rows[0];
					cbObjList.EditValue = dataRow5["ObjList"];
				}
			}
			isLoading = false;
			SetupCombo();
			SetupControlLogic();
			RepositoryItemLookUpEdit properties = txtReturnPriceList.Properties;
			properties.DataSource = await HttpClient.Commons.GetPriceList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetupControlLogic()
	{
		if (!chk_BlockZeroQ.Checked)
		{
			luk_NegStoLv.Visible = false;
		}
		if (chk_BlockZeroQ.Checked)
		{
			luk_NegStoLv.Visible = true;
		}
		WithoutUserCheck(chk_ReopOrderSale, chk_ForceReOrdSale);
		WithoutUserCheck(chk_ReopOrderPurchase, chk_ForceReOrdPurchase);
		WithoutUserCheck(chk_ClsZoDiffR, chk_ClsNoConfi);
	}

	private void SetupCombo()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		dataTable.AcceptChanges();
		DataRow dataRow = dataTable.NewRow();
		dataRow["Code"] = "C";
		dataRow["Name"] = "Company";
		dataTable.Rows.Add(dataRow);
		DataRow dataRow2 = dataTable.NewRow();
		dataRow2["Code"] = "W";
		dataRow2["Name"] = "Warehouse";
		dataTable.Rows.Add(dataRow2);
		DataRow dataRow3 = dataTable.NewRow();
		dataRow3["Code"] = "I";
		dataRow3["Name"] = "Item Setting";
		dataTable.Rows.Add(dataRow3);
		dataTable.AcceptChanges();
		luk_NegStoLv.Properties.DataSource = dataTable;
		DataTable dataTable2 = new DataTable();
		dataTable2.Columns.Add("Code", typeof(int));
		dataTable2.Columns.Add("Name", typeof(string));
		dataTable2.AcceptChanges();
		DataRow dataRow4 = dataTable2.NewRow();
		dataRow4["Code"] = 23;
		dataRow4["Name"] = "Sales Quotation";
		dataTable2.Rows.Add(dataRow4);
		DataRow dataRow5 = dataTable2.NewRow();
		dataRow5["Code"] = 17;
		dataRow5["Name"] = "Sales Order";
		dataTable2.Rows.Add(dataRow5);
		DataRow dataRow6 = dataTable2.NewRow();
		dataRow6["Code"] = 16;
		dataRow6["Name"] = "Returns";
		dataTable2.Rows.Add(dataRow6);
		DataRow dataRow7 = dataTable2.NewRow();
		dataRow7["Code"] = 1470000113;
		dataRow7["Name"] = "Purchase Request";
		dataTable2.Rows.Add(dataRow7);
		DataRow dataRow8 = dataTable2.NewRow();
		dataRow8["Code"] = 540000006;
		dataRow8["Name"] = "Purchase Quotation";
		dataTable2.Rows.Add(dataRow8);
		DataRow dataRow9 = dataTable2.NewRow();
		dataRow9["Code"] = 22;
		dataRow9["Name"] = "Purchase Order";
		dataTable2.Rows.Add(dataRow9);
		DataRow dataRow10 = dataTable2.NewRow();
		dataRow10["Code"] = 1470000065;
		dataRow10["Name"] = "Inventory Counting";
		dataTable2.Rows.Add(dataRow10);
		DataRow dataRow11 = dataTable2.NewRow();
		dataRow11["Code"] = 10000071;
		dataRow11["Name"] = "Inventory Posting";
		dataTable2.Rows.Add(dataRow11);
		dataTable2.AcceptChanges();
		cbObjList.Properties.DataSource = dataTable2;
	}

	private void WithoutUserCheck(CheckEdit main, CheckEdit sub)
	{
		if (main.EditValue.ToString() == "N")
		{
			sub.EditValue = "N";
			sub.Enabled = false;
		}
		if (main.EditValue.ToString() == "Y")
		{
			sub.Enabled = true;
		}
	}

	private void CheckSaveState()
	{
		if (!isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private async void frm_DocumentSettings_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dataTable = dtOADM.Clone();
			DataRow dataRow = dataTable.NewRow();
			dataRow["LevelWarn"] = Function.ToString(rdg_LevelWarn.EditValue);
			dataRow["OrderBlock"] = Function.ToString(chk_OrderBlock.EditValue);
			dataRow["AllowFuPos"] = Function.ToString(chk_AllowFuPos.EditValue);
			dataRow["ClosedQuot"] = Function.ToString(chk_ClosedQuot.EditValue);
			dataRow["ChangeRdr"] = Function.ToString(chk_ChangeRdr.EditValue);
			dataRow["RevisionPo"] = Function.ToString(chk_RevisionPo.EditValue);
			dataRow["ICDifExPe1"] = Function.ParseDouble(txt_ICDifExPe1.EditValue);
			dataRow["ICDifExPe3"] = Function.ParseDouble(txt_ICDifExPe3.EditValue);
			dataRow["ClsZoDiffR"] = Function.ToString(chk_ClsZoDiffR.EditValue);
			dataRow["ClsNoConfi"] = Function.ToString(chk_ClsNoConfi.EditValue);
			dataRow["AllowPostZ"] = Function.ToString(chk_AllowPostZ.EditValue);
			dataRow["PostDiffR"] = Function.ParseDouble(txt_PostDiffR.EditValue);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataTable dataTable2 = dtCINF.Clone();
			DataRow dataRow2 = dataTable2.NewRow();
			dataRow2["BlockZeroQ"] = Function.ToString(chk_BlockZeroQ.EditValue);
			dataRow2["NegStoLv"] = Function.ToString(luk_NegStoLv.EditValue);
			dataRow2["ChkQunty"] = Function.ToString(chk_ChkQunty.EditValue);
			dataTable2.Rows.Add(dataRow2);
			dataTable2.AcceptChanges();
			DataTable dataTable3 = dtADP1.Clone();
			DataRow dataRow3 = dataTable3.NewRow();
			dataRow3["AllowFuPos"] = Function.ToString(chk_AllowFuPosSale.EditValue);
			dataRow3["ObjType"] = "23";
			dataTable3.Rows.Add(dataRow3);
			dataRow3 = dataTable3.NewRow();
			dataRow3["ReopOrder"] = Function.ToString(chk_ReopOrderSale.EditValue);
			dataRow3["ForceReOrd"] = Function.ToString(chk_ForceReOrdSale.EditValue);
			dataRow3["ObjType"] = "17";
			dataTable3.Rows.Add(dataRow3);
			dataRow3 = dataTable3.NewRow();
			dataRow3["CpyPRPrice"] = Function.ToString(chk_CpyPRPrice.EditValue);
			dataRow3["ObjType"] = "1470000113";
			dataTable3.Rows.Add(dataRow3);
			dataRow3 = dataTable3.NewRow();
			dataRow3["ClosePQ"] = Function.ToString(chk_ClosePQ.EditValue);
			dataRow3["ObjType"] = "540000006";
			dataTable3.Rows.Add(dataRow3);
			dataRow3 = dataTable3.NewRow();
			dataRow3["ReopOrder"] = Function.ToString(chk_ReopOrderPurchase.EditValue);
			dataRow3["ForceReOrd"] = Function.ToString(chk_ForceReOrdPurchase.EditValue);
			dataRow3["ObjType"] = "22";
			dataTable3.Rows.Add(dataRow3);
			dataTable3.AcceptChanges();
			DataTable dataTable4 = dtOADP.Clone();
			DataRow dataRow4 = dataTable4.NewRow();
			dataRow4["ObjList"] = Function.ParseInt(cbObjList.EditValue);
			dataTable4.Rows.Add(dataRow4);
			dataTable4.AcceptChanges();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			dataSet.Tables.Add(dataTable3);
			dataSet.Tables.Add(dataTable4);
			string text = await HttpClient.DocumentSettingClient.Save(dataSet);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
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

	private void cbObjList_EditValueChanged(object sender, EventArgs e)
	{
		switch (Function.ParseInt(cbObjList.EditValue))
		{
		case 23:
			tabControlPerDoc.SelectedTabPageIndex = 0;
			break;
		case 17:
			tabControlPerDoc.SelectedTabPageIndex = 1;
			break;
		case 1470000113:
			tabControlPerDoc.SelectedTabPageIndex = 2;
			break;
		case 540000006:
			tabControlPerDoc.SelectedTabPageIndex = 3;
			break;
		case 22:
			tabControlPerDoc.SelectedTabPageIndex = 4;
			break;
		case 1470000065:
			tabControlPerDoc.SelectedTabPageIndex = 5;
			break;
		case 10000071:
			tabControlPerDoc.SelectedTabPageIndex = 6;
			break;
		case 16:
			tabControlPerDoc.SelectedTabPage = xtraPageReturn;
			break;
		}
		CheckSaveState();
	}

	private void chk_BlockZeroQ_CheckedChanged(object sender, EventArgs e)
	{
		if (isLoading)
		{
			return;
		}
		if (!chk_BlockZeroQ.Checked)
		{
			switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmEnableNegativeInventoryUsage, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
			{
			case DialogResult.OK:
				FormMode = Enums.FormViewModeType.Update;
				luk_NegStoLv.Visible = false;
				return;
			case DialogResult.Cancel:
				chk_BlockZeroQ.Checked = true;
				break;
			}
		}
		if (chk_BlockZeroQ.Checked)
		{
			FormMode = Enums.FormViewModeType.Update;
			luk_NegStoLv.Visible = true;
		}
	}

	private void chk_ReopOrderSale_CheckedChanged(object sender, EventArgs e)
	{
		WithoutUserCheck(chk_ReopOrderSale, chk_ForceReOrdSale);
		CheckSaveState();
	}

	private void ControlChecked_Changed(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void rdg_LevelWarn_SelectedIndexChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void chk_ReopOrderPurchase_CheckedChanged(object sender, EventArgs e)
	{
		WithoutUserCheck(chk_ReopOrderPurchase, chk_ForceReOrdPurchase);
		CheckSaveState();
	}

	private void chk_ClsZoDiffR_CheckedChanged(object sender, EventArgs e)
	{
		WithoutUserCheck(chk_ClsZoDiffR, chk_ClsNoConfi);
		CheckSaveState();
	}

	private void txt_PostDiffR_EditValueChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void luk_NegStoLv_EditValueChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_DocumentSettings_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
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

	private void txtReturnAllowSettingItem_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			CheckEditEx checkEditEx = txtReturnEnableSettingCost;
			bool visible = (txtReturnPriceListCheck.Visible = Function.ToString(txtReturnAllowSettingItem.EditValue).Equals("Y"));
			checkEditEx.Visible = visible;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtReturnPriceListCheck_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txtReturnPriceList.Visible = Function.ToString(txtReturnPriceListCheck.EditValue).Equals("Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtReturnDuplicateCus_EditValueChanged(object sender, EventArgs e)
	{
		CheckSaveState();
	}

	private void txtReturnPriceList_EditValueChanged(object sender, EventArgs e)
	{
		CheckSaveState();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SystemInitialization.frm_DocumentSettings));
		this.chk_BlockZeroQ = new Apzon.Controls.Editor.CheckEditEx();
		this.tabControlDocumentSettings = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabGeneral = new Apzon.Controls.Controls.XtraTabPageEx();
		this.luk_NegStoLv = new DevExpress.XtraEditors.LookUpEdit();
		this.chk_AllowFuPos = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_OrderBlock = new Apzon.Controls.Editor.CheckEditEx();
		this.rdg_LevelWarn = new Apzon.Controls.Editor.RadioGroupEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.tabPerDocument = new Apzon.Controls.Controls.XtraTabPageEx();
		this.lbDocument = new Apzon.Controls.Controls.LabelLine();
		this.tabControlPerDoc = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraPageSalesQuotation = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_AllowFuPosSale = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ClosedQuot = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraPageSalesOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_ForceReOrdSale = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ChkQunty = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ReopOrderSale = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ChangeRdr = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraPageReturn = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txtReturnPriceList = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtReturnPriceListCheck = new Apzon.Controls.Editor.CheckEditEx();
		this.txtReturnEnableSettingCost = new Apzon.Controls.Editor.CheckEditEx();
		this.txtReturnAllowSettingItem = new Apzon.Controls.Editor.CheckEditEx();
		this.txtReturnBlockDocument = new Apzon.Controls.Editor.CheckEditEx();
		this.txtReturnAllowCopyCus = new Apzon.Controls.Editor.CheckEditEx();
		this.txtReturnDuplicateCus = new Apzon.Controls.Editor.RadioGroupEx();
		this.lbReturnTitle = new Apzon.Controls.Controls.LabelLine();
		this.xtraPagePurchaseRequest = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_CpyPRPrice = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraPagePurchaseQuotation = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_ClosePQ = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraPagePurchaseOrder = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_ForceReOrdPurchase = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_RevisionPo = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ReopOrderPurchase = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraPageInventoryCounting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_ICDifExPe3 = new Apzon.Controls.Editor.CalcEditEx();
		this.txt_ICDifExPe1 = new Apzon.Controls.Editor.CalcEditEx();
		this.label3 = new System.Windows.Forms.Label();
		this.chk_ClsNoConfi = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ClsZoDiffR = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.label2 = new System.Windows.Forms.Label();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.xtraPageInventoryPosting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_PostDiffR = new Apzon.Controls.Editor.CalcEditEx();
		this.label4 = new System.Windows.Forms.Label();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.chk_AllowPostZ = new Apzon.Controls.Editor.CheckEditEx();
		this.cbObjList = new DevExpress.XtraEditors.LookUpEdit();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.chk_BlockZeroQ.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControlDocumentSettings).BeginInit();
		this.tabControlDocumentSettings.SuspendLayout();
		this.tabGeneral.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_NegStoLv.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowFuPos.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_OrderBlock.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_LevelWarn.Properties).BeginInit();
		this.tabPerDocument.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tabControlPerDoc).BeginInit();
		this.tabControlPerDoc.SuspendLayout();
		this.xtraPageSalesQuotation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowFuPosSale.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ClosedQuot.Properties).BeginInit();
		this.xtraPageSalesOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_ForceReOrdSale.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ChkQunty.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ReopOrderSale.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ChangeRdr.Properties).BeginInit();
		this.xtraPageReturn.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtReturnPriceList.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnPriceListCheck.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnEnableSettingCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnAllowSettingItem.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnBlockDocument.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnAllowCopyCus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnDuplicateCus.Properties).BeginInit();
		this.xtraPagePurchaseRequest.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_CpyPRPrice.Properties).BeginInit();
		this.xtraPagePurchaseQuotation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_ClosePQ.Properties).BeginInit();
		this.xtraPagePurchaseOrder.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_ForceReOrdPurchase.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_RevisionPo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ReopOrderPurchase.Properties).BeginInit();
		this.xtraPageInventoryCounting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_ICDifExPe3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ICDifExPe1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ClsNoConfi.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ClsZoDiffR.Properties).BeginInit();
		this.xtraPageInventoryPosting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_PostDiffR.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowPostZ.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbObjList.Properties).BeginInit();
		base.SuspendLayout();
		this.chk_BlockZeroQ.EditValue = "Y";
		this.chk_BlockZeroQ.FieldName = null;
		this.chk_BlockZeroQ.IsSearchControl = false;
		this.chk_BlockZeroQ.Location = new System.Drawing.Point(11, 118);
		this.chk_BlockZeroQ.Name = "chk_BlockZeroQ";
		this.chk_BlockZeroQ.Properties.Caption = "Block Negative Inventory By";
		this.chk_BlockZeroQ.Properties.ValueChecked = "Y";
		this.chk_BlockZeroQ.Properties.ValueUnchecked = "N";
		this.chk_BlockZeroQ.Size = new System.Drawing.Size(299, 20);
		this.chk_BlockZeroQ.TabIndex = 0;
		this.chk_BlockZeroQ.CheckedChanged += new System.EventHandler(this.chk_BlockZeroQ_CheckedChanged);
		this.tabControlDocumentSettings.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControlDocumentSettings.IsSearchControl = false;
		this.tabControlDocumentSettings.Location = new System.Drawing.Point(0, 0);
		this.tabControlDocumentSettings.Name = "tabControlDocumentSettings";
		this.tabControlDocumentSettings.SelectedTabPage = this.tabGeneral;
		this.tabControlDocumentSettings.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
		this.tabControlDocumentSettings.Size = new System.Drawing.Size(551, 493);
		this.tabControlDocumentSettings.TabIndex = 1;
		this.tabControlDocumentSettings.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabGeneral, this.tabPerDocument });
		this.tabGeneral.Controls.Add(this.luk_NegStoLv);
		this.tabGeneral.Controls.Add(this.chk_BlockZeroQ);
		this.tabGeneral.Controls.Add(this.chk_AllowFuPos);
		this.tabGeneral.Controls.Add(this.chk_OrderBlock);
		this.tabGeneral.Controls.Add(this.rdg_LevelWarn);
		this.tabGeneral.Controls.Add(this.labelLine1);
		this.tabGeneral.FieldName = "";
		this.tabGeneral.IsSearchControl = false;
		this.tabGeneral.Name = "tabGeneral";
		this.tabGeneral.Size = new System.Drawing.Size(549, 464);
		this.tabGeneral.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabGeneral.Text = "General";
		this.luk_NegStoLv.EditValue = "I";
		this.luk_NegStoLv.Location = new System.Drawing.Point(382, 118);
		this.luk_NegStoLv.Name = "luk_NegStoLv";
		this.luk_NegStoLv.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_NegStoLv.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_NegStoLv.Properties.DisplayMember = "Name";
		this.luk_NegStoLv.Properties.DropDownRows = 3;
		this.luk_NegStoLv.Properties.NullText = "";
		this.luk_NegStoLv.Properties.ValueMember = "Code";
		this.luk_NegStoLv.Size = new System.Drawing.Size(111, 20);
		this.luk_NegStoLv.TabIndex = 4;
		this.luk_NegStoLv.Visible = false;
		this.luk_NegStoLv.EditValueChanged += new System.EventHandler(this.luk_NegStoLv_EditValueChanged);
		this.chk_AllowFuPos.EditValue = "N";
		this.chk_AllowFuPos.FieldName = null;
		this.chk_AllowFuPos.IsSearchControl = false;
		this.chk_AllowFuPos.Location = new System.Drawing.Point(12, 168);
		this.chk_AllowFuPos.Name = "chk_AllowFuPos";
		this.chk_AllowFuPos.Properties.Caption = "Allow Future Posting Date";
		this.chk_AllowFuPos.Properties.ValueChecked = "Y";
		this.chk_AllowFuPos.Properties.ValueUnchecked = "N";
		this.chk_AllowFuPos.Size = new System.Drawing.Size(325, 20);
		this.chk_AllowFuPos.TabIndex = 6;
		this.chk_AllowFuPos.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_OrderBlock.EditValue = "N";
		this.chk_OrderBlock.FieldName = null;
		this.chk_OrderBlock.IsSearchControl = false;
		this.chk_OrderBlock.Location = new System.Drawing.Point(11, 143);
		this.chk_OrderBlock.Name = "chk_OrderBlock";
		this.chk_OrderBlock.Properties.Caption = "Block documents with earlier Posting Date";
		this.chk_OrderBlock.Properties.ValueChecked = "Y";
		this.chk_OrderBlock.Properties.ValueUnchecked = "N";
		this.chk_OrderBlock.Size = new System.Drawing.Size(326, 20);
		this.chk_OrderBlock.TabIndex = 5;
		this.chk_OrderBlock.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.rdg_LevelWarn.EditValue = "W";
		this.rdg_LevelWarn.FieldName = null;
		this.rdg_LevelWarn.IsSearchControl = false;
		this.rdg_LevelWarn.Location = new System.Drawing.Point(21, 40);
		this.rdg_LevelWarn.Name = "rdg_LevelWarn";
		this.rdg_LevelWarn.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.rdg_LevelWarn.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Without Warning"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("W", "Warning Only"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("B", "Block Release / Receipt")
		});
		this.rdg_LevelWarn.Size = new System.Drawing.Size(158, 72);
		this.rdg_LevelWarn.TabIndex = 3;
		this.rdg_LevelWarn.SelectedIndexChanged += new System.EventHandler(this.rdg_LevelWarn_SelectedIndexChanged);
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(11, 14);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(326, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.UseWaitCursor = true;
		this.labelLine1.Utext = "Response to Release / Receipt of Inventory Outside Defined Range";
		this.tabPerDocument.Controls.Add(this.lbDocument);
		this.tabPerDocument.Controls.Add(this.tabControlPerDoc);
		this.tabPerDocument.Controls.Add(this.cbObjList);
		this.tabPerDocument.FieldName = "";
		this.tabPerDocument.IsSearchControl = false;
		this.tabPerDocument.Name = "tabPerDocument";
		this.tabPerDocument.Size = new System.Drawing.Size(549, 464);
		this.tabPerDocument.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPerDocument.Text = "Per Document";
		this.lbDocument.FieldName = null;
		this.lbDocument.HideWhenEmpty = false;
		this.lbDocument.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDocument.Location = new System.Drawing.Point(27, 18);
		this.lbDocument.Name = "lbDocument";
		this.lbDocument.ShowUnderLine = true;
		this.lbDocument.Size = new System.Drawing.Size(80, 20);
		this.lbDocument.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDocument.TabIndex = 0;
		this.lbDocument.Utext = "Document";
		this.tabControlPerDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tabControlPerDoc.IsSearchControl = false;
		this.tabControlPerDoc.Location = new System.Drawing.Point(-1, 38);
		this.tabControlPerDoc.Name = "tabControlPerDoc";
		this.tabControlPerDoc.SelectedTabPage = this.xtraPageSalesQuotation;
		this.tabControlPerDoc.ShowTabHeader = DevExpress.Utils.DefaultBoolean.True;
		this.tabControlPerDoc.Size = new System.Drawing.Size(551, 463);
		this.tabControlPerDoc.TabIndex = 7;
		this.tabControlPerDoc.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[8] { this.xtraPageSalesQuotation, this.xtraPageSalesOrder, this.xtraPageReturn, this.xtraPagePurchaseRequest, this.xtraPagePurchaseQuotation, this.xtraPagePurchaseOrder, this.xtraPageInventoryCounting, this.xtraPageInventoryPosting });
		this.xtraPageSalesQuotation.Controls.Add(this.chk_AllowFuPosSale);
		this.xtraPageSalesQuotation.Controls.Add(this.chk_ClosedQuot);
		this.xtraPageSalesQuotation.FieldName = "";
		this.xtraPageSalesQuotation.IsSearchControl = false;
		this.xtraPageSalesQuotation.Name = "xtraPageSalesQuotation";
		this.xtraPageSalesQuotation.Size = new System.Drawing.Size(549, 434);
		this.xtraPageSalesQuotation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageSalesQuotation.Text = "Sale Quotation";
		this.chk_AllowFuPosSale.EditValue = "N";
		this.chk_AllowFuPosSale.FieldName = null;
		this.chk_AllowFuPosSale.IsSearchControl = false;
		this.chk_AllowFuPosSale.Location = new System.Drawing.Point(11, 37);
		this.chk_AllowFuPosSale.Name = "chk_AllowFuPosSale";
		this.chk_AllowFuPosSale.Properties.Caption = "Allow Future Posting Date";
		this.chk_AllowFuPosSale.Properties.ValueChecked = "Y";
		this.chk_AllowFuPosSale.Properties.ValueUnchecked = "N";
		this.chk_AllowFuPosSale.Size = new System.Drawing.Size(484, 20);
		this.chk_AllowFuPosSale.TabIndex = 7;
		this.chk_AllowFuPosSale.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_ClosedQuot.EditValue = "N";
		this.chk_ClosedQuot.FieldName = null;
		this.chk_ClosedQuot.IsSearchControl = false;
		this.chk_ClosedQuot.Location = new System.Drawing.Point(11, 12);
		this.chk_ClosedQuot.Name = "chk_ClosedQuot";
		this.chk_ClosedQuot.Properties.Caption = "Allow Copying Closed Quotations to Target Doc.";
		this.chk_ClosedQuot.Properties.ValueChecked = "Y";
		this.chk_ClosedQuot.Properties.ValueUnchecked = "N";
		this.chk_ClosedQuot.Size = new System.Drawing.Size(484, 20);
		this.chk_ClosedQuot.TabIndex = 7;
		this.chk_ClosedQuot.CheckStateChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.xtraPageSalesOrder.Controls.Add(this.chk_ForceReOrdSale);
		this.xtraPageSalesOrder.Controls.Add(this.chk_ChkQunty);
		this.xtraPageSalesOrder.Controls.Add(this.chk_ReopOrderSale);
		this.xtraPageSalesOrder.Controls.Add(this.chk_ChangeRdr);
		this.xtraPageSalesOrder.FieldName = "";
		this.xtraPageSalesOrder.IsSearchControl = false;
		this.xtraPageSalesOrder.Name = "xtraPageSalesOrder";
		this.xtraPageSalesOrder.Size = new System.Drawing.Size(549, 434);
		this.xtraPageSalesOrder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageSalesOrder.Text = "Sale Order";
		this.chk_ForceReOrdSale.EditValue = "Y";
		this.chk_ForceReOrdSale.FieldName = null;
		this.chk_ForceReOrdSale.IsSearchControl = false;
		this.chk_ForceReOrdSale.Location = new System.Drawing.Point(36, 87);
		this.chk_ForceReOrdSale.Name = "chk_ForceReOrdSale";
		this.chk_ForceReOrdSale.Properties.Caption = "Without User Confirmation";
		this.chk_ForceReOrdSale.Properties.ValueChecked = "Y";
		this.chk_ForceReOrdSale.Properties.ValueUnchecked = "N";
		this.chk_ForceReOrdSale.Size = new System.Drawing.Size(418, 20);
		this.chk_ForceReOrdSale.TabIndex = 11;
		this.chk_ForceReOrdSale.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_ChkQunty.EditValue = "Y";
		this.chk_ChkQunty.FieldName = null;
		this.chk_ChkQunty.IsSearchControl = false;
		this.chk_ChkQunty.Location = new System.Drawing.Point(11, 37);
		this.chk_ChkQunty.Name = "chk_ChkQunty";
		this.chk_ChkQunty.Properties.Caption = "Active Automatic Availablity Check";
		this.chk_ChkQunty.Properties.ValueChecked = "Y";
		this.chk_ChkQunty.Properties.ValueUnchecked = "N";
		this.chk_ChkQunty.Size = new System.Drawing.Size(526, 20);
		this.chk_ChkQunty.TabIndex = 7;
		this.chk_ChkQunty.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_ReopOrderSale.EditValue = "Y";
		this.chk_ReopOrderSale.FieldName = null;
		this.chk_ReopOrderSale.IsSearchControl = false;
		this.chk_ReopOrderSale.Location = new System.Drawing.Point(11, 62);
		this.chk_ReopOrderSale.Name = "chk_ReopOrderSale";
		this.chk_ReopOrderSale.Properties.Caption = "Reopen Doc. by Creating Returns/Goods Returns/Credit Memos Based on Doc";
		this.chk_ReopOrderSale.Properties.ValueChecked = "Y";
		this.chk_ReopOrderSale.Properties.ValueUnchecked = "N";
		this.chk_ReopOrderSale.Size = new System.Drawing.Size(526, 20);
		this.chk_ReopOrderSale.TabIndex = 10;
		this.chk_ReopOrderSale.CheckedChanged += new System.EventHandler(this.chk_ReopOrderSale_CheckedChanged);
		this.chk_ChangeRdr.EditValue = "Y";
		this.chk_ChangeRdr.FieldName = null;
		this.chk_ChangeRdr.IsSearchControl = false;
		this.chk_ChangeRdr.Location = new System.Drawing.Point(11, 12);
		this.chk_ChangeRdr.Name = "chk_ChangeRdr";
		this.chk_ChangeRdr.Properties.Caption = "Allows Change to Existing Orders";
		this.chk_ChangeRdr.Properties.ValueChecked = "Y";
		this.chk_ChangeRdr.Properties.ValueUnchecked = "N";
		this.chk_ChangeRdr.Size = new System.Drawing.Size(508, 20);
		this.chk_ChangeRdr.TabIndex = 6;
		this.chk_ChangeRdr.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.xtraPageReturn.Controls.Add(this.txtReturnPriceList);
		this.xtraPageReturn.Controls.Add(this.txtReturnPriceListCheck);
		this.xtraPageReturn.Controls.Add(this.txtReturnEnableSettingCost);
		this.xtraPageReturn.Controls.Add(this.txtReturnAllowSettingItem);
		this.xtraPageReturn.Controls.Add(this.txtReturnBlockDocument);
		this.xtraPageReturn.Controls.Add(this.txtReturnAllowCopyCus);
		this.xtraPageReturn.Controls.Add(this.txtReturnDuplicateCus);
		this.xtraPageReturn.Controls.Add(this.lbReturnTitle);
		this.xtraPageReturn.FieldName = "";
		this.xtraPageReturn.IsSearchControl = false;
		this.xtraPageReturn.Name = "xtraPageReturn";
		this.xtraPageReturn.Size = new System.Drawing.Size(549, 434);
		this.xtraPageReturn.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageReturn.Text = "Returns";
		this.txtReturnPriceList.FieldName = null;
		this.txtReturnPriceList.IsSearchControl = false;
		this.txtReturnPriceList.Location = new System.Drawing.Point(343, 331);
		this.txtReturnPriceList.Name = "txtReturnPriceList";
		this.txtReturnPriceList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtReturnPriceList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txtReturnPriceList.Properties.DisplayMember = "Name";
		this.txtReturnPriceList.Properties.NullText = "";
		this.txtReturnPriceList.Properties.ShowHeader = false;
		this.txtReturnPriceList.Properties.ValueMember = "Code";
		this.txtReturnPriceList.Size = new System.Drawing.Size(160, 20);
		this.txtReturnPriceList.TabIndex = 7;
		this.txtReturnPriceList.Visible = false;
		this.txtReturnPriceList.EditValueChanged += new System.EventHandler(this.txtReturnPriceList_EditValueChanged);
		this.txtReturnPriceListCheck.FieldName = null;
		this.txtReturnPriceListCheck.IsSearchControl = false;
		this.txtReturnPriceListCheck.Location = new System.Drawing.Point(60, 332);
		this.txtReturnPriceListCheck.Name = "txtReturnPriceListCheck";
		this.txtReturnPriceListCheck.Properties.Caption = "Price List of Return Cost";
		this.txtReturnPriceListCheck.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.txtReturnPriceListCheck.Properties.ValueChecked = "Y";
		this.txtReturnPriceListCheck.Properties.ValueUnchecked = "N";
		this.txtReturnPriceListCheck.Size = new System.Drawing.Size(168, 20);
		this.txtReturnPriceListCheck.TabIndex = 6;
		this.txtReturnPriceListCheck.Visible = false;
		this.txtReturnPriceListCheck.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.txtReturnPriceListCheck.EditValueChanged += new System.EventHandler(this.txtReturnPriceListCheck_EditValueChanged);
		this.txtReturnEnableSettingCost.FieldName = null;
		this.txtReturnEnableSettingCost.IsSearchControl = false;
		this.txtReturnEnableSettingCost.Location = new System.Drawing.Point(60, 304);
		this.txtReturnEnableSettingCost.Name = "txtReturnEnableSettingCost";
		this.txtReturnEnableSettingCost.Properties.Caption = "Enable Setting Cost on All Document Rows by Default";
		this.txtReturnEnableSettingCost.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.txtReturnEnableSettingCost.Properties.ValueChecked = "Y";
		this.txtReturnEnableSettingCost.Properties.ValueUnchecked = "N";
		this.txtReturnEnableSettingCost.Size = new System.Drawing.Size(296, 20);
		this.txtReturnEnableSettingCost.TabIndex = 5;
		this.txtReturnEnableSettingCost.Visible = false;
		this.txtReturnEnableSettingCost.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.txtReturnAllowSettingItem.FieldName = null;
		this.txtReturnAllowSettingItem.IsSearchControl = false;
		this.txtReturnAllowSettingItem.Location = new System.Drawing.Point(27, 276);
		this.txtReturnAllowSettingItem.Name = "txtReturnAllowSettingItem";
		this.txtReturnAllowSettingItem.Properties.Caption = "Allow Setting Item Cost When Document is Not Based";
		this.txtReturnAllowSettingItem.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.txtReturnAllowSettingItem.Properties.ValueChecked = "Y";
		this.txtReturnAllowSettingItem.Properties.ValueUnchecked = "N";
		this.txtReturnAllowSettingItem.Size = new System.Drawing.Size(302, 20);
		this.txtReturnAllowSettingItem.TabIndex = 4;
		this.txtReturnAllowSettingItem.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.txtReturnAllowSettingItem.EditValueChanged += new System.EventHandler(this.txtReturnAllowSettingItem_EditValueChanged);
		this.txtReturnBlockDocument.Enabled = false;
		this.txtReturnBlockDocument.FieldName = null;
		this.txtReturnBlockDocument.IsSearchControl = false;
		this.txtReturnBlockDocument.Location = new System.Drawing.Point(27, 232);
		this.txtReturnBlockDocument.Name = "txtReturnBlockDocument";
		this.txtReturnBlockDocument.Properties.Caption = "Block documents with earlier Posting Date";
		this.txtReturnBlockDocument.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.txtReturnBlockDocument.Properties.ValueChecked = "Y";
		this.txtReturnBlockDocument.Properties.ValueUnchecked = "N";
		this.txtReturnBlockDocument.Size = new System.Drawing.Size(249, 20);
		this.txtReturnBlockDocument.TabIndex = 3;
		this.txtReturnBlockDocument.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.txtReturnAllowCopyCus.FieldName = null;
		this.txtReturnAllowCopyCus.IsSearchControl = false;
		this.txtReturnAllowCopyCus.Location = new System.Drawing.Point(27, 188);
		this.txtReturnAllowCopyCus.Name = "txtReturnAllowCopyCus";
		this.txtReturnAllowCopyCus.Properties.Caption = "Allow Copying Customer Reference No. to Target Doc.";
		this.txtReturnAllowCopyCus.Properties.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.txtReturnAllowCopyCus.Properties.ValueChecked = "Y";
		this.txtReturnAllowCopyCus.Properties.ValueUnchecked = "N";
		this.txtReturnAllowCopyCus.Size = new System.Drawing.Size(302, 20);
		this.txtReturnAllowCopyCus.TabIndex = 2;
		this.txtReturnAllowCopyCus.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.txtReturnDuplicateCus.FieldName = null;
		this.txtReturnDuplicateCus.IsSearchControl = false;
		this.txtReturnDuplicateCus.Location = new System.Drawing.Point(79, 89);
		this.txtReturnDuplicateCus.Name = "txtReturnDuplicateCus";
		this.txtReturnDuplicateCus.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("N", "Without Warning"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("W", "Warning Only"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("B", "Block Release / Receipt")
		});
		this.txtReturnDuplicateCus.Size = new System.Drawing.Size(195, 90);
		this.txtReturnDuplicateCus.TabIndex = 1;
		this.txtReturnDuplicateCus.EditValueChanged += new System.EventHandler(this.txtReturnDuplicateCus_EditValueChanged);
		this.lbReturnTitle.FieldName = null;
		this.lbReturnTitle.HideWhenEmpty = false;
		this.lbReturnTitle.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbReturnTitle.Location = new System.Drawing.Point(27, 63);
		this.lbReturnTitle.Name = "lbReturnTitle";
		this.lbReturnTitle.ShowUnderLine = true;
		this.lbReturnTitle.Size = new System.Drawing.Size(317, 20);
		this.lbReturnTitle.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbReturnTitle.TabIndex = 0;
		this.lbReturnTitle.Utext = "When Duplicated Customer Reference No. Occurs";
		this.xtraPagePurchaseRequest.Controls.Add(this.chk_CpyPRPrice);
		this.xtraPagePurchaseRequest.FieldName = "";
		this.xtraPagePurchaseRequest.IsSearchControl = false;
		this.xtraPagePurchaseRequest.Name = "xtraPagePurchaseRequest";
		this.xtraPagePurchaseRequest.Size = new System.Drawing.Size(549, 434);
		this.xtraPagePurchaseRequest.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPagePurchaseRequest.Text = "Purchase Request";
		this.chk_CpyPRPrice.EditValue = "N";
		this.chk_CpyPRPrice.FieldName = null;
		this.chk_CpyPRPrice.IsSearchControl = false;
		this.chk_CpyPRPrice.Location = new System.Drawing.Point(11, 12);
		this.chk_CpyPRPrice.Name = "chk_CpyPRPrice";
		this.chk_CpyPRPrice.Properties.Caption = "Copy Prices from Purchase Requests  to  Purchase Orders";
		this.chk_CpyPRPrice.Properties.ValueChecked = "Y";
		this.chk_CpyPRPrice.Properties.ValueUnchecked = "N";
		this.chk_CpyPRPrice.Size = new System.Drawing.Size(526, 20);
		this.chk_CpyPRPrice.TabIndex = 11;
		this.chk_CpyPRPrice.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.xtraPagePurchaseQuotation.Controls.Add(this.chk_ClosePQ);
		this.xtraPagePurchaseQuotation.FieldName = "";
		this.xtraPagePurchaseQuotation.IsSearchControl = false;
		this.xtraPagePurchaseQuotation.Name = "xtraPagePurchaseQuotation";
		this.xtraPagePurchaseQuotation.Size = new System.Drawing.Size(549, 434);
		this.xtraPagePurchaseQuotation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPagePurchaseQuotation.Text = "Purchase Quotation";
		this.chk_ClosePQ.EditValue = "N";
		this.chk_ClosePQ.FieldName = null;
		this.chk_ClosePQ.IsSearchControl = false;
		this.chk_ClosePQ.Location = new System.Drawing.Point(11, 12);
		this.chk_ClosePQ.Name = "chk_ClosePQ";
		this.chk_ClosePQ.Properties.Caption = "Closed Document When Its Base Purchase Request is Closed";
		this.chk_ClosePQ.Properties.ValueChecked = "Y";
		this.chk_ClosePQ.Properties.ValueUnchecked = "N";
		this.chk_ClosePQ.Size = new System.Drawing.Size(526, 20);
		this.chk_ClosePQ.TabIndex = 11;
		this.chk_ClosePQ.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.xtraPagePurchaseOrder.Controls.Add(this.chk_ForceReOrdPurchase);
		this.xtraPagePurchaseOrder.Controls.Add(this.chk_RevisionPo);
		this.xtraPagePurchaseOrder.Controls.Add(this.chk_ReopOrderPurchase);
		this.xtraPagePurchaseOrder.FieldName = "";
		this.xtraPagePurchaseOrder.IsSearchControl = false;
		this.xtraPagePurchaseOrder.Name = "xtraPagePurchaseOrder";
		this.xtraPagePurchaseOrder.Size = new System.Drawing.Size(549, 434);
		this.xtraPagePurchaseOrder.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPagePurchaseOrder.Text = "Purchase Order";
		this.chk_ForceReOrdPurchase.EditValue = "Y";
		this.chk_ForceReOrdPurchase.FieldName = null;
		this.chk_ForceReOrdPurchase.IsSearchControl = false;
		this.chk_ForceReOrdPurchase.Location = new System.Drawing.Point(31, 79);
		this.chk_ForceReOrdPurchase.Name = "chk_ForceReOrdPurchase";
		this.chk_ForceReOrdPurchase.Properties.Caption = "Without User Confirmation";
		this.chk_ForceReOrdPurchase.Properties.ValueChecked = "Y";
		this.chk_ForceReOrdPurchase.Properties.ValueUnchecked = "N";
		this.chk_ForceReOrdPurchase.Size = new System.Drawing.Size(397, 20);
		this.chk_ForceReOrdPurchase.TabIndex = 25;
		this.chk_ForceReOrdPurchase.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_RevisionPo.EditValue = "N";
		this.chk_RevisionPo.FieldName = null;
		this.chk_RevisionPo.IsSearchControl = false;
		this.chk_RevisionPo.Location = new System.Drawing.Point(11, 12);
		this.chk_RevisionPo.Name = "chk_RevisionPo";
		this.chk_RevisionPo.Properties.Caption = "Split Purchase Order";
		this.chk_RevisionPo.Properties.ValueChecked = "Y";
		this.chk_RevisionPo.Properties.ValueUnchecked = "N";
		this.chk_RevisionPo.Size = new System.Drawing.Size(487, 20);
		this.chk_RevisionPo.TabIndex = 6;
		this.chk_RevisionPo.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_ReopOrderPurchase.EditValue = "Y";
		this.chk_ReopOrderPurchase.FieldName = null;
		this.chk_ReopOrderPurchase.IsSearchControl = false;
		this.chk_ReopOrderPurchase.Location = new System.Drawing.Point(11, 54);
		this.chk_ReopOrderPurchase.Name = "chk_ReopOrderPurchase";
		this.chk_ReopOrderPurchase.Properties.Caption = "Reopen Doc. by Creating Returns/Goods Returns/Credit Memos Based on It";
		this.chk_ReopOrderPurchase.Properties.ValueChecked = "Y";
		this.chk_ReopOrderPurchase.Properties.ValueUnchecked = "N";
		this.chk_ReopOrderPurchase.Size = new System.Drawing.Size(526, 20);
		this.chk_ReopOrderPurchase.TabIndex = 8;
		this.chk_ReopOrderPurchase.CheckedChanged += new System.EventHandler(this.chk_ReopOrderPurchase_CheckedChanged);
		this.xtraPageInventoryCounting.Controls.Add(this.txt_ICDifExPe3);
		this.xtraPageInventoryCounting.Controls.Add(this.txt_ICDifExPe1);
		this.xtraPageInventoryCounting.Controls.Add(this.label3);
		this.xtraPageInventoryCounting.Controls.Add(this.chk_ClsNoConfi);
		this.xtraPageInventoryCounting.Controls.Add(this.chk_ClsZoDiffR);
		this.xtraPageInventoryCounting.Controls.Add(this.labelLine8);
		this.xtraPageInventoryCounting.Controls.Add(this.labelLine9);
		this.xtraPageInventoryCounting.Controls.Add(this.label2);
		this.xtraPageInventoryCounting.Controls.Add(this.labelLine6);
		this.xtraPageInventoryCounting.Controls.Add(this.labelLine7);
		this.xtraPageInventoryCounting.FieldName = "";
		this.xtraPageInventoryCounting.IsSearchControl = false;
		this.xtraPageInventoryCounting.Name = "xtraPageInventoryCounting";
		this.xtraPageInventoryCounting.Size = new System.Drawing.Size(549, 434);
		this.xtraPageInventoryCounting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageInventoryCounting.Text = " Inventory Counting";
		this.txt_ICDifExPe3.EditValue = new decimal(new int[4]);
		this.txt_ICDifExPe3.FieldName = null;
		this.txt_ICDifExPe3.IsSearchControl = false;
		this.txt_ICDifExPe3.Location = new System.Drawing.Point(338, 118);
		this.txt_ICDifExPe3.Name = "txt_ICDifExPe3";
		this.txt_ICDifExPe3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ICDifExPe3.Properties.DisplayFormat.FormatString = "n2";
		this.txt_ICDifExPe3.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_ICDifExPe3.Properties.IsSearchControl = false;
		this.txt_ICDifExPe3.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_ICDifExPe3.Size = new System.Drawing.Size(63, 20);
		this.txt_ICDifExPe3.TabIndex = 23;
		this.txt_ICDifExPe3.EditValueChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.txt_ICDifExPe1.EditValue = new decimal(new int[4] { 5, 0, 0, 0 });
		this.txt_ICDifExPe1.FieldName = null;
		this.txt_ICDifExPe1.IsSearchControl = false;
		this.txt_ICDifExPe1.Location = new System.Drawing.Point(338, 40);
		this.txt_ICDifExPe1.Name = "txt_ICDifExPe1";
		this.txt_ICDifExPe1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ICDifExPe1.Properties.DisplayFormat.FormatString = "n2";
		this.txt_ICDifExPe1.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_ICDifExPe1.Properties.IsSearchControl = false;
		this.txt_ICDifExPe1.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_ICDifExPe1.Size = new System.Drawing.Size(63, 20);
		this.txt_ICDifExPe1.TabIndex = 21;
		this.txt_ICDifExPe1.EditValueChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.label3.AutoSize = true;
		this.label3.Location = new System.Drawing.Point(407, 121);
		this.label3.Name = "label3";
		this.label3.Size = new System.Drawing.Size(18, 13);
		this.label3.TabIndex = 24;
		this.label3.Text = "%";
		this.chk_ClsNoConfi.EditValue = "N";
		this.chk_ClsNoConfi.FieldName = null;
		this.chk_ClsNoConfi.IsSearchControl = false;
		this.chk_ClsNoConfi.Location = new System.Drawing.Point(32, 200);
		this.chk_ClsNoConfi.Name = "chk_ClsNoConfi";
		this.chk_ClsNoConfi.Properties.Caption = "Without User Confirmation";
		this.chk_ClsNoConfi.Properties.ValueChecked = "Y";
		this.chk_ClsNoConfi.Properties.ValueUnchecked = "N";
		this.chk_ClsNoConfi.Size = new System.Drawing.Size(501, 20);
		this.chk_ClsNoConfi.TabIndex = 24;
		this.chk_ClsNoConfi.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.chk_ClsZoDiffR.EditValue = "Y";
		this.chk_ClsZoDiffR.FieldName = null;
		this.chk_ClsZoDiffR.IsSearchControl = false;
		this.chk_ClsZoDiffR.Location = new System.Drawing.Point(11, 175);
		this.chk_ClsZoDiffR.Name = "chk_ClsZoDiffR";
		this.chk_ClsZoDiffR.Properties.Caption = "When Copying Inventory Counting to Inventory Posting, Close Counted Rows with Variance of Zero";
		this.chk_ClsZoDiffR.Properties.ValueChecked = "Y";
		this.chk_ClsZoDiffR.Properties.ValueUnchecked = "N";
		this.chk_ClsZoDiffR.Size = new System.Drawing.Size(522, 20);
		this.chk_ClsZoDiffR.TabIndex = 23;
		this.chk_ClsZoDiffR.CheckedChanged += new System.EventHandler(this.chk_ClsZoDiffR_CheckedChanged);
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(11, 121);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(331, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 22;
		this.labelLine8.Utext = "Highlight Rows Where Counters Difference Is Greater Than:";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(11, 90);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(138, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 21;
		this.labelLine9.Utext = "For Two Inventory Counters";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(407, 43);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(18, 13);
		this.label2.TabIndex = 22;
		this.label2.Text = "%";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(11, 12);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(138, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 19;
		this.labelLine6.Utext = "For One Inventory Counter";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(11, 43);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(329, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 20;
		this.labelLine7.Utext = "Highlight Rows Where the Variance Is Greater Than:";
		this.xtraPageInventoryPosting.Controls.Add(this.txt_PostDiffR);
		this.xtraPageInventoryPosting.Controls.Add(this.label4);
		this.xtraPageInventoryPosting.Controls.Add(this.labelLine10);
		this.xtraPageInventoryPosting.Controls.Add(this.chk_AllowPostZ);
		this.xtraPageInventoryPosting.FieldName = "";
		this.xtraPageInventoryPosting.IsSearchControl = false;
		this.xtraPageInventoryPosting.Name = "xtraPageInventoryPosting";
		this.xtraPageInventoryPosting.Size = new System.Drawing.Size(549, 434);
		this.xtraPageInventoryPosting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraPageInventoryPosting.Text = "Inventory Posting";
		this.txt_PostDiffR.EditValue = new decimal(new int[4] { 5, 0, 0, 0 });
		this.txt_PostDiffR.FieldName = null;
		this.txt_PostDiffR.IsSearchControl = false;
		this.txt_PostDiffR.Location = new System.Drawing.Point(338, 40);
		this.txt_PostDiffR.Name = "txt_PostDiffR";
		this.txt_PostDiffR.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_PostDiffR.Properties.DisplayFormat.FormatString = "n2";
		this.txt_PostDiffR.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_PostDiffR.Properties.IsSearchControl = false;
		this.txt_PostDiffR.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txt_PostDiffR.Size = new System.Drawing.Size(63, 20);
		this.txt_PostDiffR.TabIndex = 26;
		this.txt_PostDiffR.EditValueChanged += new System.EventHandler(this.txt_PostDiffR_EditValueChanged);
		this.label4.AutoSize = true;
		this.label4.Location = new System.Drawing.Point(407, 43);
		this.label4.Name = "label4";
		this.label4.Size = new System.Drawing.Size(18, 13);
		this.label4.TabIndex = 27;
		this.label4.Text = "%";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(11, 43);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(339, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 25;
		this.labelLine10.Utext = "Highlight Rows Where the Variance Is Greater Than:";
		this.chk_AllowPostZ.EditValue = "Y";
		this.chk_AllowPostZ.FieldName = null;
		this.chk_AllowPostZ.IsSearchControl = false;
		this.chk_AllowPostZ.Location = new System.Drawing.Point(11, 12);
		this.chk_AllowPostZ.Name = "chk_AllowPostZ";
		this.chk_AllowPostZ.Properties.Caption = "Allow Inbound Posting with Zero Price";
		this.chk_AllowPostZ.Properties.ValueChecked = "Y";
		this.chk_AllowPostZ.Properties.ValueUnchecked = "N";
		this.chk_AllowPostZ.Size = new System.Drawing.Size(505, 20);
		this.chk_AllowPostZ.TabIndex = 24;
		this.chk_AllowPostZ.CheckedChanged += new System.EventHandler(this.ControlChecked_Changed);
		this.cbObjList.EditValue = "0";
		this.cbObjList.Location = new System.Drawing.Point(108, 15);
		this.cbObjList.Name = "cbObjList";
		this.cbObjList.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbObjList.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.cbObjList.Properties.DisplayMember = "Name";
		this.cbObjList.Properties.NullText = "";
		this.cbObjList.Properties.ValueMember = "Code";
		this.cbObjList.Size = new System.Drawing.Size(137, 20);
		this.cbObjList.TabIndex = 5;
		this.cbObjList.EditValueChanged += new System.EventHandler(this.cbObjList_EditValueChanged);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(4, 499);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(75, 20);
		this.btnOK.TabIndex = 2;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(81, 499);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(551, 523);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.tabControlDocumentSettings);
		base.FormId = "oDocumentSetting";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DocumentSettings.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DocumentSettings";
		base.ShowInTaskbar = false;
		this.Text = "Document Settings";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_DocumentSettings_FormClosing);
		base.Load += new System.EventHandler(this.frm_DocumentSettings_Load);
		((System.ComponentModel.ISupportInitialize)this.chk_BlockZeroQ.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControlDocumentSettings).EndInit();
		this.tabControlDocumentSettings.ResumeLayout(false);
		this.tabGeneral.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_NegStoLv.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowFuPos.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_OrderBlock.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rdg_LevelWarn.Properties).EndInit();
		this.tabPerDocument.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tabControlPerDoc).EndInit();
		this.tabControlPerDoc.ResumeLayout(false);
		this.xtraPageSalesQuotation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_AllowFuPosSale.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ClosedQuot.Properties).EndInit();
		this.xtraPageSalesOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_ForceReOrdSale.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ChkQunty.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ReopOrderSale.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ChangeRdr.Properties).EndInit();
		this.xtraPageReturn.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtReturnPriceList.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnPriceListCheck.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnEnableSettingCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnAllowSettingItem.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnBlockDocument.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnAllowCopyCus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtReturnDuplicateCus.Properties).EndInit();
		this.xtraPagePurchaseRequest.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_CpyPRPrice.Properties).EndInit();
		this.xtraPagePurchaseQuotation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_ClosePQ.Properties).EndInit();
		this.xtraPagePurchaseOrder.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_ForceReOrdPurchase.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_RevisionPo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ReopOrderPurchase.Properties).EndInit();
		this.xtraPageInventoryCounting.ResumeLayout(false);
		this.xtraPageInventoryCounting.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_ICDifExPe3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ICDifExPe1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ClsNoConfi.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ClsZoDiffR.Properties).EndInit();
		this.xtraPageInventoryPosting.ResumeLayout(false);
		this.xtraPageInventoryPosting.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_PostDiffR.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllowPostZ.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbObjList.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
