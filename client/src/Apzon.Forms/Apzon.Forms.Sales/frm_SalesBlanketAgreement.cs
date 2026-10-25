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
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.PM;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Sales;

public class frm_SalesBlanketAgreement : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string _currentRowStatus = "";

	private string _bpCurrency = "##";

	private bool isLoadDetail;

	private Enums.FormViewModeType _openFormViewMode = Enums.FormViewModeType.View;

	private bool _isMapping;

	public int AbsID;

	public int detail = -1;

	private readonly string _auth = "F";

	private DataTable dtContact = new DataTable();

	private DataTable dtDetail;

	public DataTable dtClgCode = new DataTable();

	private readonly List<CntPRepControls> lstContactControls = new List<CntPRepControls>();

	public static Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	public string FormDocumentTable = "OOAT";

	public string FormLineTable = "OAT1";

	public string FormLineTableDetail = "OAT2";

	public string FormController = "SalesBlanketAgreement";

	public string FormLayoutCode = "OAT1";

	public Enums.TypeCFL _bpType = Enums.TypeCFL.Customer;

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private IContainer components;

	private GridControlEx grdDoc;

	private GridColumnEx gcBPCode;

	private GridColumnEx gcBPName;

	private GridColumnEx gcContactPerson;

	private GridColumnEx gcSeries;

	private GridColumnEx gcNumber;

	private GridColumnEx gcAgreementMethod;

	private GridColumnEx gcStartDate;

	private GridColumnEx gcEndDate;

	private GridColumnEx gcTerminationDate;

	private GridColumnEx gcSigningDate;

	private GridColumnEx gcDescription;

	private GridColumnEx gcAgreementType;

	private GridColumnEx gcStatus;

	private GridColumnEx gcUserDiscnt;

	private GridColumnEx gcOwner;

	private GridColumnEx gcRenewal;

	private GridColumnEx gcSettlementProbability;

	private GridColumnEx gcRemindVal;

	private GridColumnEx gcRemindUnit;

	private GridColumnEx gcRemarks;

	private RepositoryItemButtonEditCFL rBpCode;

	private RepositoryItemButtonEditCFL rBpName;

	private RepositoryItemLookUpEdit rSeries;

	private RepositoryItemLookUpEdit rMethod;

	private RepositoryItemDateEdit rDate;

	private RepositoryItemLookUpEdit rType;

	private RepositoryItemLookUpEdit rStatusD;

	private RepositoryItemCheckEdit rUseDiscnt;

	private RepositoryItemLookUpEdit rOwner;

	private RepositoryItemCheckEdit rRenewal;

	private RepositoryItemCalcEdit rSettleProb;

	private RepositoryItemLookUpEdit rRemindUnit;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private XtraTabControlEx tabControl;

	private XtraTabPageEx tabPageDetails;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdDetails;

	private GridColumnEx gcTelephoneNo;

	private GridColumnEx gcEmail;

	private GridColumnEx gcItemCode;

	private RepositoryItemButtonEditCFL rItemCode;

	private GridColumnEx gcItemName;

	private RepositoryItemButtonEditCFL rItemName;

	private GridColumnEx gcItemGroup;

	private GridColumnEx gcPlanQty;

	private RepositoryItemCalcEdit rPlanQty;

	private GridColumnEx gcUnitPrice;

	private GridColumnEx gcCumQty;

	private GridColumnEx gcCumulativeAmountLC;

	private GridColumnEx gcCumulativeAmountFC;

	private GridColumnEx gcOpenQuantity;

	private GridColumnEx gcOpenAmountLC;

	private GridColumnEx gcOpenAmountFC;

	private GridColumnEx gcFreeText;

	private GridColumnEx gcUomCode;

	private GridColumnEx gcUoMName;

	private GridColumnEx gcNumPerMsr;

	private GridColumnEx gcUoMGroup;

	private GridColumnEx gcRetPortion;

	private GridColumnEx gcWrrtyEnd;

	private GridColumnEx gcLineStatus;

	private RepositoryItemCalcEdit rRetPortion;

	private RepositoryItemDateEdit rWrrtyEnd;

	private RepositoryItemLookUpEdit rCntctCode;

	private GridColumnEx gcAbsID;

	private RepositoryItemLookUpEdit rItemGroup;

	private ContextMenuStrip contextDocMenu;

	private ToolStripMenuItem iNSERTToolStripMenuItem;

	private RepositoryItemLookUpEdit rlineStatus;

	private GridColumnEx gvAgrNo;

	private GridColumnEx gcPriceList;

	private RepositoryItemLookUpEdit rListNum;

	private GridColumnEx gcDiscount;

	private GridColumnEx gcPlanAmtLC;

	private GridColumnEx gcPlanAmtFC;

	private GridColumnEx gcAgrLineNum;

	private RepositoryItemLookUpEdit rUgpEntry;

	private ContextMenuStrip contextLineMenu;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem listItemToolStripMenuItem;

	private ToolStripMenuItem listBussissToolStripMenuItem;

	private RepositoryItemLookUpEdit rStatusA;

	private RepositoryItemLookUpEdit rStatusDF;

	private GridViewEx gvDoc;

	private GridViewEx gvDetails;

	private GridColumnEx gcCurrency;

	private RepositoryItemLookUpEdit rep_Currency;

	private RepositoryItemCalcEdit rPlanAmount;

	private RepositoryItemCalcEdit rDiscount;

	private RepositoryItemButtonEditCFL rUomCode;

	public DropDownButtonEx drlCopyTo;

	public ButtonEditCFL txtBusinessPartner;

	public LabelLine lb_DocStatus;

	public LabelLine lb_BusinessPartner;

	public LabelLine lb_DocNumber;

	public LabelLine lb_ToDate;

	public LabelLine lb_FromDate;

	public SimpleButtonEx btn_Search;

	public TextEditEx txtNoOfTrans;

	public DateEditEx txtToDate;

	public DateEditEx txtFromDate;

	private XtraTabPageEx tabAttachments;

	private ucAttachment ucAttachment1;

	private LookUpEditEx cbxStatus;

	private ToolStripMenuItem ctxMenu_Cancel;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx8;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx9;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx10;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx11;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx12;

	private GridColumnEx gcProject;

	private RepositoryItemButtonEditCFL rProject;

	private GridColumnEx gcLine_Project;

	private GridColumnEx gcBpCurr;

	private RepositoryItemLookUpEdit rBpCurrency;

	private ToolStripMenuItem ctxMenu_Duplicate;

	private GridColumnEx gcBpType;

	private ToolStripMenuItem relatedActivitiesToolStripMenuItem;

	private GridColumnEx gcTaxCode;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private RepositoryItemLookUpEdit rTaxCode;

	private GridColumnEx gcPaymentTerm;

	private RepositoryItemLookUpEdit luk_PaymentTerm;

	private GridColumnEx gcPayMethod;

	private RepositoryItemLookUpEdit luk_PayMethod;

	private DataTable OpenFormDocument { get; set; }

	private DataTable OpenFormLines { get; set; }

	private DataTable OpenFormDetails { get; set; }

	private ContextActionMappingImpl[] MappingDocument { get; set; }

	private ContextActionMappingImpl[] MappingLines { get; set; }

	private ContextActionMappingImpl[] MappingDetails { get; set; }

	public List<ApzObjectTypes> PopCopyTo { get; set; }

	public frm_SalesBlanketAgreement()
	{
		FormId = "oSalesBlanketAgreement";
		InitializeComponent();
		OnLoadAsync += grdSalesBlanketAgreement_Load;
	}

	public frm_SalesBlanketAgreement(int absID)
	{
		AbsID = absID;
	}

	private void VisibleColumnByAgreeMethod(string method)
	{
		try
		{
			if (method == "M")
			{
				gvDetails.Columns["ItemCode"].Visible = false;
				gvDetails.Columns["ItemName"].Visible = false;
				gvDetails.Columns["ItemGroup"].Visible = false;
				gvDetails.Columns["PlanQty"].Visible = false;
				gvDetails.Columns["UnitPrice"].Visible = false;
				gvDetails.Columns["CumQty"].Visible = false;
				gvDetails.Columns["CumAmntFC"].Visible = false;
				gvDetails.Columns["PlanAmtFC"].Visible = false;
				gvDetails.Columns["OpenAmntFC"].Visible = false;
				gvDetails.Columns["UomCode"].Visible = false;
				gvDetails.Columns["InvntryUom"].Visible = false;
				gvDetails.Columns["NumPerMsr"].Visible = false;
				gvDetails.Columns["UgpEntry"].Visible = false;
				gvDetails.Columns["LineStatus"].Visible = false;
				gvDetails.Columns["LineStatus"].Visible = false;
				gvDetails.Columns["PlanAmtLC"].Visible = true;
				gvDetails.Columns["PlanAmtFC"].Visible = true;
				gvDetails.Columns["Discount"].Visible = true;
				gvDetails.Columns["CumAmntLC"].Visible = true;
				gvDetails.Columns["OpenAmntLC"].Visible = true;
				gvDetails.Columns["FreeTxt"].Visible = true;
				gvDetails.Columns["RetPortion"].Visible = true;
				gvDetails.Columns["WrrtyEnd"].Visible = true;
			}
			else
			{
				gvDetails.Columns["ItemCode"].Visible = true;
				gvDetails.Columns["ItemName"].Visible = true;
				gvDetails.Columns["ItemGroup"].Visible = true;
				gvDetails.Columns["PlanQty"].Visible = true;
				gvDetails.Columns["UnitPrice"].Visible = true;
				gvDetails.Columns["CumQty"].Visible = true;
				gvDetails.Columns["CumAmntFC"].Visible = true;
				gvDetails.Columns["PlanAmtFC"].Visible = true;
				gvDetails.Columns["UomCode"].Visible = true;
				gvDetails.Columns["InvntryUom"].Visible = true;
				gvDetails.Columns["NumPerMsr"].Visible = true;
				gvDetails.Columns["UgpEntry"].Visible = true;
				gvDetails.Columns["LineStatus"].Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (e.Column.FieldName == "TermDate" && Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) != "T" && !string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("TermDate"))))
			{
				gvDoc.SetFocusedRowCellValue("Status", "T");
			}
			if (e.Column.FieldName.Equals(gcProject.FieldName) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdateBpProject2Line, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DataTable dataSourceTable = grdDetails.DataSourceTable;
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					dataSourceTable.Rows[i][gcLine_Project.FieldName] = e.Value;
				}
				dataSourceTable.AcceptChanges();
			}
			if (e.Column.FieldName.Equals(gcBPCode.FieldName))
			{
				DataTable dataTable = await HttpClient.BusinessMaster.GetADoc(Function.ToString(e.Value));
				if (!dataTable.IsNotNull())
				{
					return;
				}
				_bpCurrency = Function.ToString(dataTable.Rows[0]["Currency"]);
				if (_bpCurrency.Equals("##"))
				{
					gcBpCurr.OptionsColumn.ReadOnly = false;
					gvDoc.SetRowCellValue(e.RowHandle, gcBpCurr, GlobalData.MainCurrency);
				}
				else
				{
					gcBpCurr.OptionsColumn.ReadOnly = true;
					gvDoc.SetRowCellValue(e.RowHandle, gcBpCurr, _bpCurrency);
				}
			}
			if (e.Column.FieldName.Equals(gcBpCurr.FieldName))
			{
				string text = Function.ToString(e.Value);
				gvDetails.Columns["PlanAmtLC"].OptionsColumn.ReadOnly = !text.Equals(GlobalData.MainCurrency);
				gvDetails.Columns["PlanAmtFC"].OptionsColumn.ReadOnly = text.Equals(GlobalData.MainCurrency);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetMapping(DataTable doc, DataTable lines, ContextActionMappingImpl[] mappingDoc, ContextActionMappingImpl[] mappingLines, DataTable details, ContextActionMappingImpl[] mappingDetails, Enums.FormViewModeType viewmode = Enums.FormViewModeType.Add)
	{
		OpenFormDocument = doc;
		OpenFormLines = lines;
		MappingDocument = mappingDoc;
		MappingLines = mappingLines;
		OpenFormDetails = details;
		MappingDetails = mappingDetails;
		_openFormViewMode = viewmode;
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx7 = btn_OK;
				SimpleButtonEx simpleButtonEx8 = btn_Cancel;
				bool flag = (btn_Search.Enabled = true);
				bool enabled = (simpleButtonEx8.Enabled = flag);
				simpleButtonEx7.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				drlCopyTo.Enabled = true;
				drlCopyTo.Enabled = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx5 = btn_OK;
				SimpleButtonEx simpleButtonEx6 = btn_Cancel;
				bool flag = (btn_Search.Enabled = true);
				bool enabled = (simpleButtonEx6.Enabled = flag);
				simpleButtonEx5.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				drlCopyTo.Enabled = false;
				drlCopyTo.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				SimpleButtonEx simpleButtonEx4 = btn_Cancel;
				bool flag = (btn_Search.Enabled = true);
				bool enabled = (simpleButtonEx4.Enabled = flag);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				drlCopyTo.Enabled = false;
				drlCopyTo.Enabled = true;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				SimpleButtonEx simpleButtonEx2 = btn_Cancel;
				bool flag = (btn_Search.Enabled = false);
				bool enabled = (simpleButtonEx2.Enabled = flag);
				simpleButtonEx.Enabled = enabled;
				drlCopyTo.Enabled = false;
				drlCopyTo.Enabled = false;
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

	public async void PRINTLAYOUT()
	{
		try
		{
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"));
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
			await WinFuntions.PrintLayout(cardCode, num, FormLayoutCode);
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
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"));
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
			await WinFuntions.PreviewLayout(cardCode, num, FormLayoutCode, this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_SalesBlanketAgreement_Load(object sender, EventArgs e)
	{
		try
		{
			LoadPaymentLookupEdit();
			switch (_openFormViewMode)
			{
			case Enums.FormViewModeType.Add:
				_isMapping = true;
				grdDoc.DataSourceTable.Rows.Clear();
				grdDetails.DataSourceTable.Rows.Clear();
				iNSERTToolStripMenuItem_Click(null, null);
				if (OpenFormDocument.IsNotNull() || OpenFormLines.IsNotNull())
				{
					gvDoc.MappingSource(OpenFormDocument, MappingDocument, null, null, null, multirow: false);
					if (Function.ToString(gvDoc.GetFocusedRowCellValue("BpType")) == "S")
					{
						FormId = "oPurchaseBlanketAgreement";
						Text = "Purchase Blanket Agreement";
						_bpType = Enums.TypeCFL.Vendor;
					}
					gvDoc.SetFocusedRowCellValue("Number", GetNumber());
					gvDoc.SetFocusedRowCellValue("Series", FormId.Equals("oSalesBlanketAgreement") ? 47 : 48);
					GridControlEx gridControlEx = grdDetails;
					ContextActionMappingImpl[] array = MappingLines.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
					DataTable dataTable = OpenFormLines.Clone();
					if (array.Length != 0)
					{
						ContextActionMappingImpl[] groupcol = MappingLines.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
						foreach (IGrouping<NTuple<object>, DataRow> item2 in from r in OpenFormLines.AsEnumerable()
							group r by new NTuple<object>(from map in groupcol
								where !string.IsNullOrEmpty(map.SourceField)
								select new
								{
									Name = map.SourceField,
									Value = r[map.SourceField]
								}))
						{
							DataRow dataRow = dataTable.NewRow();
							List<Common.ParameterValue> list = new List<Common.ParameterValue>();
							object[] values = item2.Key.Values;
							for (int num = 0; num < values.Length; num++)
							{
								if (ContextActionMappingSource.ToType(values[num], typeof(Common.ParameterValue)) is Common.ParameterValue item)
								{
									list.Add(item);
								}
							}
							ContextActionMappingImpl[] array2 = groupcol;
							foreach (ContextActionMappingImpl col in array2)
							{
								if (!string.IsNullOrEmpty(col.SourceField))
								{
									Common.ParameterValue parameterValue = list.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
									if (parameterValue != null)
									{
										dataRow[col.SourceField] = parameterValue.Value;
									}
								}
							}
							array2 = array;
							foreach (ContextActionMappingImpl col2 in array2)
							{
								dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
							}
							dataTable.Rows.Add(dataRow);
						}
					}
					if (dataTable.IsNotNull())
					{
						OpenFormLines = dataTable;
					}
					int count = OpenFormLines.Rows.Count;
					if (gridControlEx != null)
					{
						List<string> list2 = new List<string>();
						ContextActionMappingImpl[] array3 = MappingLines.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
						for (int num2 = 0; num2 < array3.Length; num2++)
						{
							if (!string.IsNullOrEmpty(array3[num2].SourceField))
							{
								list2.Add("ISNULL(" + array3[num2].SourceField + ",'')='{" + num2 + "}'");
							}
						}
						try
						{
							GridColumn[] array4 = gvDetails.Columns.Where((GridColumn t) => MappingLines.Any((ContextActionMappingImpl contextActionMappingImpl) => contextActionMappingImpl.TargetField == t.FieldName)).ToArray();
							List<ContextActionMappingImpl> list3 = new List<ContextActionMappingImpl>();
							GridColumn[] array5 = array4;
							foreach (GridColumn x in array5)
							{
								list3.Add(MappingLines.FirstOrDefault((ContextActionMappingImpl t) => t.TargetField == x.FieldName));
							}
							for (int num3 = 0; num3 < count; num3++)
							{
								int num4 = gvDetails.RowCount - 1;
								gvDetails.FocusedRowHandle = num4;
								if (gvDetails.FocusedRowHandle != num4)
								{
									return;
								}
								gvDetails.BeginInsertRow();
								ContextActionMappingSource.MappingValue(OpenFormLines, list3.ToArray(), num3, gvDetails);
								if (MappingDetails != null && MappingDetails.Length != 0 && OpenFormDetails.IsNotNull())
								{
									ContextActionMappingImpl[] grpBatch = array3.Where((ContextActionMappingImpl contextActionMappingImpl) => Function.ToString(contextActionMappingImpl.SumType) == "").ToArray();
									int i1 = num3;
									DataRow[] array6 = (from r in OpenFormDetails.AsEnumerable()
										where new NTuple<object>(from map in grpBatch
											where !string.IsNullOrEmpty(map.SourceField)
											select new
											{
												Name = map.SourceField,
												Value = OpenFormLines.Rows[i1][map.SourceField]
											}).Equals(new NTuple<object>(from map in grpBatch
											where !string.IsNullOrEmpty(map.SourceField)
											select new
											{
												Name = map.SourceField,
												Value = r[map.SourceField]
											}))
										select (r)).ToArray();
									if (array6.Length != 0)
									{
										try
										{
											for (int num5 = 0; num5 < array6.Length; num5++)
											{
												DataRow dataRow2 = dtDetail.NewRow();
												for (int num6 = 0; num6 < MappingDetails.Length; num6++)
												{
													if (!string.IsNullOrEmpty(MappingDetails[num6].SourceField))
													{
														dataRow2[MappingDetails[num6].TargetField] = array6[num5][MappingDetails[num6].SourceField];
													}
													if (MappingDetails[num6].DfltValue != null && dataRow2[MappingDetails[num6].TargetField] == DBNull.Value)
													{
														dataRow2[MappingDetails[num6].TargetField] = MappingDetails[num6].DfltValue;
													}
												}
												dataRow2["AgrNo"] = Function.ParseInt(gvDetails.GetRowCellValue(num3, "AgrNo"));
												dataRow2["AgrLnNum"] = Function.ParseInt(gvDetails.GetRowCellValue(num3, "AgrLineNum"));
												dataRow2["AgrEfctNum"] = num5;
												dtDetail.Rows.Add(dataRow2);
											}
											dtDetail.AcceptChanges();
										}
										catch (Exception ex)
										{
											Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
											WinCommons.ErrorLineNumber(ex);
										}
									}
								}
								gvDetails.EndInsertRow();
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
							WinCommons.ErrorLineNumber(ex2);
						}
					}
				}
				_isMapping = false;
				break;
			case Enums.FormViewModeType.View:
				if (AbsID > 0)
				{
					await GetDoc();
				}
				break;
			}
			if (FormId.Equals("oSalesBlanketAgreement"))
			{
				Text = "Sales Blanket Agreement";
				PopCopyTo = new List<ApzObjectTypes>
				{
					ApzObjectTypes.oQuotations,
					ApzObjectTypes.oOrders,
					ApzObjectTypes.oDeliveryNotes,
					ApzObjectTypes.oInvoices
				};
			}
			else
			{
				gvDoc.FormId = "oPurchaseBlanketAgreement";
				gvDetails.FormId = "oPurchaseBlanketAgreement";
				PopCopyTo = new List<ApzObjectTypes>
				{
					ApzObjectTypes.oPurchaseQuotations,
					ApzObjectTypes.oPurchaseOrders,
					ApzObjectTypes.oPurchaseDeliveryNotes
				};
			}
			txtBusinessPartner.Properties.TypeCFL = _bpType;
			rOwner.BestFitMode = BestFitMode.BestFitResizePopup;
			rOwner.BestFitRowCount = 100;
			LoadCopyMenu();
		}
		catch (Exception ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex3.Message);
		}
	}

	public async Task grdSalesBlanketAgreement_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			WinFuntions.SetFormatControlValue(this);
			await InitCustomize();
			await LoadSalesBlanketAgreement();
			await LoadDetail(detail);
			await GetListItemGroup();
			LoadCurrency();
			rBpCurrency.DataSource = GlobalData.Currencies;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task InitCustomize()
	{
		_ = 3;
		try
		{
			cbxStatus.SetDataSource(new Dictionary<string, string>
			{
				{ "D", "Draft" },
				{ "F", "On Hold" },
				{ "A", "Approved" },
				{ "T", "Terminated" },
				{ "C", "Canceled" }
			});
			gvDoc.DisplayFormatDocument();
			TblFormular[FormDocumentTable] = gvDoc;
			TblFormular[FormLineTable] = gvDetails;
			DataTable formulaSearchByTable = _httpClient.Commons.GetFormulaSearchByTable(FormDocumentTable);
			if (formulaSearchByTable.IsNotNull())
			{
				await gvDoc.InitChooseFromList(formulaSearchByTable, TblFormular);
			}
			DataTable formulaSearchByTable2 = _httpClient.Commons.GetFormulaSearchByTable(FormLineTable);
			if (formulaSearchByTable2.IsNotNull())
			{
				await gvDetails.InitChooseFromList(formulaSearchByTable2, TblFormular);
			}
			gvDoc.InitPasteFromExcel();
			await WinFuntions.LoadUdfColumns(gvDoc, FormDocumentTable, TblFormular);
			await WinFuntions.LoadUdfColumns(gvDetails, FormLineTable, TblFormular);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadPaymentLookupEdit()
	{
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Commons");
			string condition = "";
			DataSet dataSet = await hp.GetAsync("DocumentGetComBoTabAccounting", new { condition });
			if (dataSet != null)
			{
				if (dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Columns.Count > 0)
				{
					DataRow dataRow = dataSet.Tables[0].NewRow();
					dataRow["Code"] = -1;
					dataRow["Name"] = "";
					dataSet.Tables[0].Rows.InsertAt(dataRow, 0);
					dataSet.Tables[0].AcceptChanges();
					luk_PaymentTerm.DataSource = dataSet.Tables[0];
				}
				if (dataSet.Tables.Count > 1 && dataSet.Tables[1] != null && dataSet.Tables[1].Columns.Count > 0)
				{
					DataRow dataRow2 = dataSet.Tables[1].NewRow();
					dataRow2["Code"] = "";
					dataRow2["Name"] = "";
					dataSet.Tables[1].Rows.InsertAt(dataRow2, 0);
					dataSet.Tables[1].AcceptChanges();
					luk_PayMethod.DataSource = dataSet.Tables[1];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public virtual void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView))
			{
				return;
			}
			string text = Function.ToString(gridView.GetRowCellValue(e.RowHandle, "Status"));
			if (text.Equals("C") || text.Equals("T"))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
			if (e.Column.FieldName == "ListNum")
			{
				if (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gcAgreementMethod)) == "I")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = SystemColors.Window;
				}
			}
			if (new string[4] { "BpName", "Tel1", "E_MailL", "Method" }.Contains(e.Column.FieldName))
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

	public virtual void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (_auth == "F")
			{
				int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Series"));
				if (gvDoc.FocusedColumn.FieldName == "Number" && (num == 47 || num == 48))
				{
					e.Cancel = true;
				}
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("Renewal"));
				if ((gvDoc.FocusedColumn.FieldName == "RemindVal" || gvDoc.FocusedColumn.FieldName == "RemindUnit") && text == "N")
				{
					e.Cancel = true;
				}
				if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"))) && gvDoc.FocusedColumn.FieldName != "BpCode" && gvDoc.FocusedColumn.FieldName != "BpName")
				{
					e.Cancel = true;
				}
				if (gvDoc.FocusedColumn.FieldName == "TermDate" && Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "D")
				{
					e.Cancel = true;
				}
				if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate && (Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "A" || _currentRowStatus.Equals("A")) && !new string[5] { "Status", "CntctCode", "TermDate", "Descript", "NumAtCard" }.Contains(gvDoc.FocusedColumn.FieldName))
				{
					e.Cancel = true;
				}
				if (Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "C" || Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "T")
				{
					e.Cancel = true;
				}
				if (gvDoc.FocusedColumn.FieldName == "TermDate" && Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "T" && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("TermDate"))))
				{
					e.Cancel = false;
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

	private void rType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.EditingValue);
			gvDoc.SetFocusedRowCellValue("UseDiscnt", (text == "S") ? "N" : "Y");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async void rSeries_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gvDoc.EditingValue);
			if (num == 47 || num == 48)
			{
				gvDoc.SetFocusedRowCellValue("Number", GetNumber());
			}
			else
			{
				gvDoc.SetFocusedRowCellValue("Number", 0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void rBpCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Busy)
				{
					return;
				}
				string txtSeach = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBPCode));
				frm_CFL frm_CFL2 = new frm_CFL(_bpType, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("BpCode", chooseItem["CardCode"]);
					gvDoc.SetFocusedRowCellValue("BpName", chooseItem["CardName"]);
					DataTable contactPerson = GetContactPerson(Function.ToString(chooseItem["CardCode"]));
					int ownerByBPCode = GetOwnerByBPCode(Function.ToString(chooseItem["CardCode"]));
					if (contactPerson != null && contactPerson.Rows.Count > 0)
					{
						chooseItem = contactPerson.Rows[0];
						rCntctCode.DataSource = contactPerson;
						gvDoc.SetFocusedRowCellValue("CntctCode", chooseItem["CntctCode"]);
						gvDoc.SetFocusedRowCellValue("Tel1", chooseItem["Tel1"]);
						gvDoc.SetFocusedRowCellValue("E_MailL", chooseItem["E_MailL"]);
					}
					else
					{
						gvDoc.SetFocusedRowCellValue("CntctCode", 0);
						gvDoc.SetFocusedRowCellValue("Tel1", "");
						gvDoc.SetFocusedRowCellValue("E_MailL", "");
					}
					gvDoc.SetFocusedRowCellValue("Owner", ownerByBPCode);
				}
				grdDoc.RefreshDataSource();
				gvDoc.UpdateCurrentRow();
			}
			else
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				string copyF_CodeCustomer = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBPCode));
				frm_BusinessMasterData2._copyF_CodeCustomer = copyF_CodeCustomer;
				frm_BusinessMasterData2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void rBpName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Busy)
				{
					return;
				}
				string txtSeach = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBPCode));
				frm_CFL frm_CFL2 = new frm_CFL(_bpType, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				DataRow chooseItem = frm_CFL2.ChooseItem;
				if (chooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("BpCode", chooseItem["CardCode"]);
					gvDoc.SetFocusedRowCellValue("BpName", chooseItem["CardName"]);
					DataTable contactPerson = GetContactPerson(Function.ToString(chooseItem["CardCode"]));
					int ownerByBPCode = GetOwnerByBPCode(Function.ToString(chooseItem["CardCode"]));
					if (contactPerson != null && contactPerson.Rows.Count > 0)
					{
						chooseItem = contactPerson.Rows[0];
						rCntctCode.DataSource = contactPerson;
						gvDoc.SetFocusedRowCellValue("CntctCode", chooseItem["CntctCode"]);
						gvDoc.SetFocusedRowCellValue("Tel1", chooseItem["Tel1"]);
						gvDoc.SetFocusedRowCellValue("E_MailL", chooseItem["E_MailL"]);
					}
					else
					{
						gvDoc.SetFocusedRowCellValue("CntctCode", 0);
						gvDoc.SetFocusedRowCellValue("Tel1", "");
						gvDoc.SetFocusedRowCellValue("E_MailL", "");
					}
					gvDoc.SetFocusedRowCellValue("Owner", ownerByBPCode);
				}
				grdDoc.RefreshDataSource();
				gvDoc.UpdateCurrentRow();
			}
			else
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				string copyF_CodeCustomer = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBPCode));
				frm_BusinessMasterData2._copyF_CodeCustomer = copyF_CodeCustomer;
				frm_BusinessMasterData2.ShowDialog(this);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void rBpCode_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Busy)
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

	public virtual void gvDoc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Status" && (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && sender is GridView gridView)
			{
				switch (Function.ToString(gridView.GetRowCellValue(e.RowHandle, gcStatus)))
				{
				case "A":
				case "F":
				case "T":
					e.RepositoryItem = rStatusA;
					break;
				case "D":
					e.RepositoryItem = rStatusD;
					break;
				}
			}
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				gvDoc.Columns["Owner"].OptionsColumn.AllowEdit = false;
			}
			else
			{
				gvDoc.Columns["Owner"].OptionsColumn.AllowEdit = true;
			}
			if (!(e.Column.FieldName == "CntctCode"))
			{
				return;
			}
			string text = Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "BpCode"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit
			{
				NullText = "",
				DisplayMember = "Name",
				ValueMember = "CntctCode"
			};
			if (dtContact.IsNotNull())
			{
				DataRow[] array = dtContact.Select($"CardCode='{text}'");
				if (array.Length != 0)
				{
					repositoryItemLookUpEdit.DataSource = array.CopyToDataTable();
				}
				repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Name", 10, "Name"));
				repositoryItemLookUpEdit.ShowHeader = false;
				repositoryItemLookUpEdit.ShowFooter = false;
				repositoryItemLookUpEdit.PopupWidth = 10;
				e.RepositoryItem = repositoryItemLookUpEdit;
			}
			repositoryItemLookUpEdit.EditValueChanged += (object s, EventArgs v) =>
			{
				try
				{
					if (s is LookUpEdit lookUpEdit)
					{
						int num = Function.ParseInt(lookUpEdit.EditValue);
						DataRow[] array2 = dtContact.Select($"CntctCode='{num}'");
						if (array2.Length != 0)
						{
							gvDoc.SetFocusedRowCellValue("Tel1", array2[0]["Tel1"]);
							gvDoc.SetFocusedRowCellValue("E_MailL", array2[0]["E_MailL"]);
						}
					}
					grdDoc.RefreshDataSource();
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.ErrorLineNumber(ex2);
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void gvDoc_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.RowCount > 0)
			{
				int absId = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.TopRowIndex, "AbsID"));
				await LoadDetail(absId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void iNSERTToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Cancel.Enabled = true;
			AddNewRowDoc();
			AddNewLine();
			ChangeAllowEdit(allowEdit: true);
			rMethod.Enabled = true;
			rMethod.ShowDropDown = ShowDropDown.SingleClick;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void gvDoc_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "CntctCode")
			{
				return;
			}
			string card = ((gvDoc.GetRowCellValue(e.RowHandle, "BpCode") == DBNull.Value || gvDoc.GetRowCellValue(e.RowHandle, "BpCode") == null) ? "" : gvDoc.GetRowCellValue(e.RowHandle, "BpCode").ToString());
			if (!string.IsNullOrWhiteSpace(card) && lstContactControls.FirstOrDefault((CntPRepControls x) => x.CardCode == card).RepControl != null)
			{
				e.RepositoryItem = lstContactControls.FirstOrDefault((CntPRepControls x) => x.CardCode == card).RepControl;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void rItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"))))
				{
					return;
				}
				DataTable dataTable = grdDetails.DataSource as DataTable;
				string txtSeach = Function.ToString(gvDetails.GetRowCellValue(gvDetails.FocusedRowHandle, gcItemCode));
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, txtSeach, "O", Enums.ItemObjectType.SellItem, null, "");
				frm_CFL2.ShowDialog(this);
				List<DataRow> chooseItems = frm_CFL2.ChooseItems;
				if (chooseItems != null && chooseItems.Count > 0)
				{
					int num = gvDetails.FocusedRowHandle;
					if (dataTable != null)
					{
						dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
						foreach (DataRow item in chooseItems)
						{
							ChooseFromListItemCode(Function.ToString(item["ItemCode"]), dataTable, num);
							num++;
						}
						if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetRowCellValue(gvDetails.RowCount - 1, "ItemCode"))) && !Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
						{
							DataRow dataRow = dataTable.NewRow();
							dataRow["LineStatus"] = "O";
							dataTable.Rows.InsertAt(dataRow, num);
						}
						if (FormMode == Enums.FormViewModeType.View)
						{
							FormMode = Enums.FormViewModeType.Update;
						}
					}
				}
				grdDetails.DataSource = dataTable;
				grdDetails.RefreshDataSource();
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gvDetails.GetFocusedRowCellValue("ItemCode"));
				if (!string.IsNullOrEmpty(text))
				{
					ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void rMethod_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDetails.DataSource as DataTable;
			if (Function.ToString(gvDoc.EditingValue) == "M")
			{
				if (dataTable != null && dataTable.Rows.Count > 1 && XtraMessageBoxEx.Show(Enums.MessageCode.SalesBlanketConfirmChangeMethod, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					gvDoc.SetFocusedRowCellValue("Method", "I");
					return;
				}
				VisibleColumnByAgreeMethod("M");
				if (dataTable != null)
				{
					DataTable dataTable2 = dataTable.Clone();
					DataRow row = dataTable2.NewRow();
					dataTable2.Rows.Add(row);
					grdDetails.DataSource = dataTable2;
				}
				grdDetails.RefreshDataSource();
				using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement"))
				{
					RepositoryItemLookUpEdit repositoryItemLookUpEdit = rListNum;
					repositoryItemLookUpEdit.DataSource = await helper.GetAsync("GetPriceList");
				}
				gvDoc.SetFocusedRowCellValue("ListNum", 1);
				gvDoc.SetFocusedRowCellValue("Method", "M");
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetFocusedRowCellValue("PlanAmtLC"))) && XtraMessageBoxEx.Show(Enums.MessageCode.SalesBlanketConfirmChangeMethod, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				gvDoc.SetFocusedRowCellValue("Method", "M");
			}
			else
			{
				VisibleColumnByAgreeMethod("I");
				gvDoc.SetFocusedRowCellValue("ListNum", null);
				grdDetails.DataSource = dataTable.Clone();
				grdDetails.RefreshDataSource();
				AddNewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void gvDetails_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
			{
				if (e.Column.FieldName == "CumAmntLC" || e.Column.FieldName == "OpenAmntLC")
				{
					e.Appearance.BackColor = Color.LightGray;
				}
			}
			else if (gvDetails.Columns.Where((GridColumn t) => t.OptionsColumn.ReadOnly).Contains(e.Column))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void gvDetails_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (_auth == "F")
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("Method"));
				if (gvDetails.FocusedColumn.FieldName == "PlanAmtLC" || gvDetails.FocusedColumn.FieldName == "Discount")
				{
					if (text.Equals("I"))
					{
						e.Cancel = true;
						return;
					}
				}
				else if ((gvDetails.FocusedColumn.FieldName == "ItemCode" || gvDetails.FocusedColumn.FieldName == "ItemName" || gvDetails.FocusedColumn.FieldName == "PlanQty" || gvDetails.FocusedColumn.FieldName == "UnitPrice" || gvDetails.FocusedColumn.FieldName == "CumAmntLC") && text.Equals("M"))
				{
					e.Cancel = true;
					return;
				}
				if (text.Equals("I") && string.IsNullOrEmpty(Function.ToString(gvDetails.GetFocusedRowCellValue("ItemCode"))) && gvDetails.FocusedColumn.FieldName != "ItemCode" && gvDetails.FocusedColumn.FieldName != "ItemName")
				{
					e.Cancel = true;
				}
				else if (btn_OK.Text == Apzon.Commons.Properties.Resources.ButtonUpdate && (Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "A" || _currentRowStatus.Equals("A")))
				{
					e.Cancel = true;
				}
				else if (new string[2] { "T", "C" }.Contains(Function.ToString(gvDoc.GetFocusedRowCellValue("Status"))))
				{
					e.Cancel = true;
				}
				else if (gvDetails.FocusedColumn.FieldName.Equals(gcUomCode.FieldName) && Function.ParseInt(gvDetails.GetFocusedRowCellValue(gcUoMGroup)) == -1)
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

	private void gvDetails_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (sender is GridView { FocusedColumn: var focusedColumn } gridView)
			{
				if (focusedColumn.FieldName.ToUpper().Equals("UNITPRICE"))
				{
					int focusedRowHandle = gridView.FocusedRowHandle;
					List<string> currency = Function.GetCurrency(Function.ToString(e.Value));
					int lineNum = Function.ParseInt(gridView.GetRowCellValue(focusedRowHandle, "AgrLineNum"));
					int docNum = Function.ParseInt(gvDoc.GetFocusedRowCellValue("Number"));
					if (((DataTable)grdDoc.DataSource).AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["Number"]) == docNum) == null)
					{
						WinCommons.Error(Enums.MessageCode.SalesBlanketIsNull, "");
						return;
					}
					DataRow dataRow = ((DataTable)gridView.GridControl.DataSource).AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["AgrLineNum"]) == lineNum);
					if (currency.Count > 0 && dataRow != null)
					{
						decimal num = Function.ParseDecimal(currency[0]);
						string alpha = Function.ToString(currency[1]);
						if (string.IsNullOrEmpty(alpha))
						{
							alpha = Function.ToString(dataRow["Currency"]);
						}
						if (GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha.ToUpper())))
						{
							dataRow["Currency"] = alpha;
							e.Value = num;
							gridView.SetRowCellValue(focusedRowHandle, focusedColumn, num);
						}
					}
				}
				if (focusedColumn.FieldName.ToUpper().Equals("PLANAMTLC"))
				{
					int focusedRowHandle2 = gridView.FocusedRowHandle;
					List<string> currency2 = Function.GetCurrency(Function.ToString(e.Value));
					int lineNum2 = Function.ParseInt(gridView.GetRowCellValue(focusedRowHandle2, "AgrLineNum"));
					DataRow dataRow2 = ((DataTable)gridView.GridControl.DataSource).AsEnumerable().FirstOrDefault((DataRow s) => Function.ParseInt(s["AgrLineNum"]) == lineNum2);
					if (currency2.Count > 0)
					{
						decimal num2 = Function.ParseDecimal(currency2[0]);
						string alpha2 = Function.ToString(currency2[1]);
						if (string.IsNullOrEmpty(alpha2))
						{
							alpha2 = GlobalData.MainCurrency;
						}
						if (!GlobalData.Currencies.Exists((Common.Table_Code t) => t.Code.ToUpper().Equals(alpha2.ToUpper())))
						{
							WinCommons.Error(Enums.MessageCode.InvalidCurrency, "");
							return;
						}
						if (dataRow2 != null)
						{
							dataRow2["Currency"] = alpha2;
						}
						e.Value = num2;
						gridView.SetRowCellValue(focusedRowHandle2, focusedColumn, num2);
					}
				}
				if (focusedColumn.FieldName.ToUpper().Equals("PLANAMTFC"))
				{
					e.Value = Function.ToString(e.Value) + " " + Function.ToString(gvDoc.GetFocusedRowCellValue(gcBpCurr));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public void gvDetails_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			string currency = Function.ToString(gvDetails.GetListSourceRowCellValue(e.ListSourceRowIndex, "Currency"));
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			if (e.Column.FieldName.ToUpper().Equals("UNITPRICE") && Function.ParseDouble(e.Value) > 0.0)
			{
				e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currency.ToUpper());
			}
			if (e.Column.FieldName.ToUpper().Equals("PLANAMTLC") && Function.ParseDouble(e.Value) > 0.0)
			{
				e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currency.ToUpper());
			}
			if (e.Column.FieldName.ToUpper().Equals("OPENAMNTLC") && Function.ParseDouble(e.Value) > 0.0)
			{
				e.DisplayText = string.Format("{0:n" + GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits + "} {1}", e.Value, GlobalData.MainCurrency.ToUpper());
			}
			string text = $"{GlobalData.MainCurrency.ToUpper()}";
			gcOpenAmountLC.SummaryItem.DisplayFormat = "{0:n" + GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits + "} " + text;
			gcOpenQuantity.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.CurrencyDecimalDigits + "}";
			gcPlanQty.SummaryItem.DisplayFormat = "{0:n" + GlobalData.QuantitiesFormat.CurrencyDecimalDigits + "}";
			gcPlanAmtLC.SummaryItem.DisplayFormat = "{0:n" + GlobalData.TotalsAccuracyFormat.CurrencyDecimalDigits + "} " + text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void grdDetails_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grdDetails, gvDetails, new List<string> { "ItemCode" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void gvDetails_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetDataRow(gvDetails.RowCount - 1)["ItemCode"])) && e.KeyCode == Keys.Tab)
			{
				AddNewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void gvDetails_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDetails.DataSourceTable;
			int focusedDataSourceRowIndex = gvDetails.GetFocusedDataSourceRowIndex();
			if (dataSourceTable.IsNotNull())
			{
				if (e.Column.FieldName == "PlanQty" && !Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
				{
					gvDetails.SetFocusedRowCellValue("OpenQty", Function.ParseDecimal(gvDetails.GetFocusedRowCellValue("PlanQty")));
					if (InitLineCellValueChanged(gvDetails.GetFocusedDataRow()) > 0.0)
					{
						gvDetails.SetFocusedRowCellValue("OpenAmntLC", InitLineCellValueChanged(gvDetails.GetFocusedDataRow()));
					}
					else
					{
						gvDetails.SetFocusedRowCellValue("OpenAmntLC", DBNull.Value);
					}
				}
				if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetRowCellValue(gvDetails.RowCount - 1, "ItemCode"))))
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["LineStatus"] = "O";
					dataSourceTable.Rows.Add(dataRow);
				}
				dataSourceTable.AcceptChanges();
			}
			if (e.Column.FieldName.Equals(gcItemCode.FieldName))
			{
				DataTable dataSourceTable2 = grdDetails.DataSourceTable;
				ChooseFromListItemCode(Function.ToString(e.Value), dataSourceTable2, gvDetails.FocusedRowHandle);
				if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetRowCellValue(gvDetails.RowCount - 1, "ItemCode"))) && !Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
				{
					DataRow dataRow2 = dataSourceTable2.NewRow();
					dataRow2["LineStatus"] = "O";
					dataSourceTable2.Rows.InsertAt(dataRow2, gvDetails.FocusedRowHandle);
				}
				grdDetails.DataSource = dataSourceTable2;
				grdDetails.DataSourceTable.AcceptChanges();
			}
			else if (e.Column.FieldName == "PlanAmtLC" && Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
			{
				gvDetails.SetFocusedRowCellValue("OpenAmntLC", Function.ParseDecimal(gvDetails.GetFocusedRowCellValue("PlanAmtLC")));
			}
			else if (e.Column.FieldName == gcUomCode.FieldName)
			{
				DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gvDetails.GetRowCellValue(e.RowHandle, gcUoMGroup)), Function.ToString(e.Value));
				if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
				{
					dataSourceTable.Rows[focusedDataSourceRowIndex]["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
					dataSourceTable.Rows[focusedDataSourceRowIndex][gcUoMName.FieldName] = dataUomByCode.Rows[0]["UomName"];
					dataSourceTable.Rows[focusedDataSourceRowIndex][gcNumPerMsr.FieldName] = Function.ParseDouble(gvDetails.GetRowCellValue(e.RowHandle, "NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
					grdDetails.DataSourceTable.AcceptChanges();
				}
			}
			else if (e.Column.FieldName.Equals(gcCurrency.FieldName))
			{
				dataSourceTable.Rows[focusedDataSourceRowIndex]["Rate"] = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(Function.ToString(e.Value), DateTime.Now.Date);
			}
			if (new string[3] { "PlanQty", "UnitPrice", "Currency" }.Contains(e.Column.FieldName) && dataSourceTable.IsExistsColumn("OpenAmntLC"))
			{
				dataSourceTable.Rows[focusedDataSourceRowIndex]["OpenAmntFC"] = Function.ParseDouble(gvDetails.GetRowCellValue(focusedDataSourceRowIndex, "PlanQty")) * Function.ParseDouble(gvDetails.GetRowCellValue(focusedDataSourceRowIndex, "UnitPrice"));
				dataSourceTable.Rows[focusedDataSourceRowIndex]["OpenAmntLC"] = Function.ParseDouble(gvDetails.GetRowCellValue(focusedDataSourceRowIndex, "PlanQty")) * Function.ParseDouble(gvDetails.GetRowCellValue(focusedDataSourceRowIndex, "UnitPrice")) * Function.ParseDouble(gvDetails.GetRowCellValue(focusedDataSourceRowIndex, "Rate"));
				dataSourceTable.AcceptChanges();
			}
			grdDetails.DataSource = dataSourceTable;
			grdDetails.RefreshDataSource();
			if (FormMode == Enums.FormViewModeType.View)
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

	private void gvDetails_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (!gvDetails.CalcHitInfo(grdDetails.PointToClient(Control.MousePosition)).InRow || string.IsNullOrEmpty(Function.ToString(gvDetails.GetFocusedRowCellValue("ItemCode"))))
			{
				return;
			}
			string cardCode = Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"));
			DateTime value = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("StartDate")).Value;
			DateTime endDate = ((!Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("EndDate")).HasValue) ? DateTime.MaxValue : Function.ParseDateTime(gvDoc.GetFocusedRowCellValue("EndDate")).Value);
			DataTable dataTable = ((DataTable)grdDetails.DataSource).Clone();
			DataRow focusedDataRow = gvDetails.GetFocusedDataRow();
			DataRow dataRow = dataTable.NewRow();
			dataRow["AgrNo"] = focusedDataRow["AgrNo"];
			dataRow["AgrLineNum"] = focusedDataRow["AgrLineNum"];
			dataRow["ItemCode"] = focusedDataRow["ItemCode"];
			dataRow["ItemName"] = focusedDataRow["ItemName"];
			dataRow["ItemGroup"] = focusedDataRow["ItemGroup"];
			dataRow["PlanQty"] = focusedDataRow["PlanQty"];
			dataRow["UnitPrice"] = focusedDataRow["UnitPrice"];
			dataRow["CumQty"] = focusedDataRow["CumQty"];
			dataRow["CumAmntLC"] = focusedDataRow["CumAmntLC"];
			dataRow["CumAmntFC"] = focusedDataRow["CumAmntFC"];
			dataRow["PlanAmtLC"] = focusedDataRow["PlanAmtLC"];
			dataRow["PlanAmtFC"] = focusedDataRow["PlanAmtFC"];
			dataRow["Discount"] = focusedDataRow["Discount"];
			dataRow["FreeTxt"] = focusedDataRow["FreeTxt"];
			dataRow["UomCode"] = focusedDataRow["UomCode"];
			dataRow["InvntryUom"] = focusedDataRow["InvntryUom"];
			dataRow["NumPerMsr"] = focusedDataRow["NumPerMsr"];
			dataRow["RetPortion"] = focusedDataRow["RetPortion"];
			dataRow["WrrtyEnd"] = focusedDataRow["WrrtyEnd"];
			dataRow["UgpEntry"] = focusedDataRow["UgpEntry"];
			dataRow["LineStatus"] = focusedDataRow["LineStatus"];
			dataRow["Currency"] = focusedDataRow["Currency"];
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			frm_SalesBlanketAgreement_RowDetails frmDetail = new frm_SalesBlanketAgreement_RowDetails();
			frmDetail.MdiParent = MdiParent;
			frmDetail.CardCode = cardCode;
			frmDetail.StartDate = value;
			frmDetail.EndDate = endDate;
			frmDetail.dtLine = dataTable;
			frmDetail.dtDetail = dtDetail;
			frmDetail.dtClgCode = dtClgCode;
			frmDetail.FormClosed += (object obj, FormClosedEventArgs s) =>
			{
				try
				{
					if (frmDetail.dtDetail != null && frmDetail.dtDetail.Rows.Count > 0)
					{
						MergeDataDetails(frmDetail.dtDetail.Copy());
						dtClgCode = frmDetail.dtClgCode.Copy();
						if ((dtDetail.IsNotNull() || dtClgCode.IsNotNull()) && FormMode == Enums.FormViewModeType.View)
						{
							FormMode = Enums.FormViewModeType.Update;
						}
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
				}
			};
			frmDetail.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual async void btnSave_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("EndDate"))))
			{
				WinCommons.Error(Enums.MessageCode.SpecifyEndDateBlanket, "");
				gvDoc.FocusedColumn = gvDoc.Columns["EndDate"];
				gvDoc.ShowEditor();
				return;
			}
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("Status")) == "T" && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("TermDate"))))
			{
				WinCommons.Error(Enums.MessageCode.TerminationDateBlanket, "");
				gvDoc.FocusedColumn = gvDoc.Columns["TermDate"];
				gvDoc.ShowEditor();
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("SignDate"))))
			{
				WinCommons.Error(Enums.MessageCode.SpecifySignDateBlanket, "");
				gvDoc.FocusedColumn = gvDoc.Columns["SignDate"];
				gvDoc.ShowEditor();
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("StartDate"))))
			{
				WinCommons.Error(Enums.MessageCode.SpecifyStartDateBlanket, "");
				gvDoc.FocusedColumn = gvDoc.Columns["StartDate"];
				gvDoc.ShowEditor();
				return;
			}
			if (Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("StartDate")) > Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("EndDate")))
			{
				WinCommons.Error(Enums.MessageCode.SpecifyEndDateLaterOrEqualStartDateBlanket, "");
				gvDoc.FocusedColumn = gvDoc.Columns["EndDate"];
				gvDoc.ShowEditor();
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"))))
			{
				WinCommons.Error(Enums.MessageCode.InvalidBpCodeBlanket, "");
				gvDoc.FocusedColumn = gvDoc.Columns["BpCode"];
				gvDoc.ShowEditor();
				return;
			}
			DataTable dataTable = grdDoc.DataSourceTable.Clone();
			dataTable.ImportRow(gvDoc.GetFocusedDataRow());
			DataTable dataTable2 = grdDetails.DataSourceTable.Clone();
			DataRow[] array;
			if (Function.ToString(dataTable.Rows[0]["Method"]).Equals("I"))
			{
				array = (from t in grdDetails.DataSourceTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))
					select t).ToArray();
			}
			else
			{
				array = (Function.ToString(dataTable.Rows[0]["BPCurr"]).Equals(GlobalData.MainCurrency) ? (from t in grdDetails.DataSourceTable.AsEnumerable()
					where Math.Abs(Function.ParseDouble(t["PlanAmtLC"])) > 1E-07
					select t).ToArray() : (from t in grdDetails.DataSourceTable.AsEnumerable()
					where Math.Abs(Function.ParseDouble(t["PlanAmtFC"])) > 1E-07
					select t).ToArray());
			}
			if (array.Length != 0)
			{
				dataTable2 = array.CopyToDataTable();
			}
			dataTable2.AcceptChanges();
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("Method")) == "I")
			{
				for (int num = 0; num < dataTable2.Rows.Count; num++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[num]["PlanQty"])))
					{
						WinCommons.Error(Enums.MessageCode.SpecifyQuantityGreater0BlanketRow, "");
						return;
					}
				}
			}
			else if (dataTable.Rows.Count > 0 && dataTable2.Rows.Count > 0)
			{
				if (Function.ToString(dataTable.Rows[0]["BPCurr"]).Equals(GlobalData.MainCurrency) && string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0]["PlanAmtLC"])))
				{
					WinCommons.Error(Enums.MessageCode.SpecifyPlanAmountLcGreaterThan0, "");
					return;
				}
				if (!Function.ToString(dataTable.Rows[0]["BPCurr"]).Equals(GlobalData.MainCurrency) && string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[0]["PlanAmtFC"])))
				{
					WinCommons.Error(Enums.MessageCode.SpecifyPlanAmountFcGreaterThan0, "");
					return;
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				btn_Search.Enabled = false;
				btn_OK.Enabled = false;
				if (!dtDetail.IsNotNull())
				{
					dtDetail = new DataTable();
				}
				string text = await Save(dataTable, dataTable2, dtDetail, dtClgCode);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await GetDoc();
					detail = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
					await LoadDetail(detail);
					ChangeAllowEdit(allowEdit: false);
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			btn_Search.Enabled = true;
			btn_OK.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	public virtual void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnBusinessPartner_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(_bpType, false, Function.ToString(txtBusinessPartner.Text), "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtBusinessPartner.Text = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual async void btnSearch_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			btn_Search.Enabled = false;
			await GetDoc();
			await LoadDetail(detail);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			btn_Search.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	public virtual void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDetails.FocusedRowHandle != gvDetails.RowCount - 1)
			{
				DataRow focusedDataRow = gvDetails.GetFocusedDataRow();
				grdDetails.DataSourceTable().Rows.Remove(focusedDataRow);
				grdDetails.RefreshDataSource();
			}
			if (FormMode == Enums.FormViewModeType.View)
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

	private void listItemToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"))) || Function.ToString(gvDoc.GetFocusedRowCellValue("Method")) == "M")
			{
				return;
			}
			DataTable dataTable = grdDetails.DataSource as DataTable;
			string txtSeach = Function.ToString(gvDetails.GetRowCellValue(gvDoc.FocusedRowHandle, gcItemCode));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, true, txtSeach, "O", Enums.ItemObjectType.SellItem, null, "");
			frm_CFL2.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			if (chooseItems != null && chooseItems.Count > 0)
			{
				int num = gvDetails.FocusedRowHandle;
				if (dataTable != null)
				{
					dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
					foreach (DataRow item in chooseItems)
					{
						ChooseFromListItemCode(Function.ToString(item["ItemCode"]), dataTable, num);
						num++;
					}
					if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetRowCellValue(gvDetails.RowCount - 1, "ItemCode"))) && !Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["LineStatus"] = "O";
						dataTable.Rows.InsertAt(dataRow, num);
					}
				}
			}
			grdDetails.DataSource = dataTable;
			grdDetails.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual void listBussissToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update)
			{
				return;
			}
			string txtSeach = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBPCode));
			frm_CFL frm_CFL2 = new frm_CFL(_bpType, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("BpCode", chooseItem["CardCode"]);
				gvDoc.SetFocusedRowCellValue("BpName", chooseItem["CardName"]);
				DataTable contactPerson = GetContactPerson(Function.ToString(chooseItem["CardCode"]));
				if (contactPerson != null && contactPerson.Rows.Count > 0)
				{
					chooseItem = contactPerson.Rows[0];
					rCntctCode.DataSource = contactPerson;
					gvDoc.SetFocusedRowCellValue("CntctCode", chooseItem["CntctCode"]);
					gvDoc.SetFocusedRowCellValue("Name", chooseItem["Name"]);
					gvDoc.SetFocusedRowCellValue("Tel1", chooseItem["Tel1"]);
					gvDoc.SetFocusedRowCellValue("E_MailL", chooseItem["E_MailL"]);
				}
				else
				{
					gvDoc.SetFocusedRowCellValue("CntctCode", 0);
					gvDoc.SetFocusedRowCellValue("Tel1", "");
					gvDoc.SetFocusedRowCellValue("E_MailL", "");
				}
			}
			grdDoc.RefreshDataSource();
			gvDoc.UpdateCurrentRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rUomCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUomCode, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
			{
				UomGroup = Function.ParseInt(gvDetails.GetFocusedRowCellValue(gcUoMGroup))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDetails.SetFocusedRowCellValue(gcUomCode, frm_CFL2.ChooseItem["UomCode"]);
				grdDetails.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual async Task LoadSalesBlanketAgreement()
	{
		List<Common.ListItems> list = new List<Common.ListItems>();
		list.Add(new Common.ListItems
		{
			Value = (FormId.Equals("oSalesBlanketAgreement") ? "47" : "48"),
			Text = "Primary"
		});
		list.Add(new Common.ListItems
		{
			Value = "-1",
			Text = "Manual"
		});
		rSeries.DataSource = list;
		List<Common.Table_ID> list2 = new List<Common.Table_ID>();
		list2.Add(new Common.Table_ID
		{
			ID = "I",
			Name = "Items Method"
		});
		list2.Add(new Common.Table_ID
		{
			ID = "M",
			Name = "Monetary Method"
		});
		rMethod.DataSource = list2;
		List<Common.Table_ID> list3 = new List<Common.Table_ID>();
		list3.Add(new Common.Table_ID
		{
			ID = "G",
			Name = "General"
		});
		list3.Add(new Common.Table_ID
		{
			ID = "S",
			Name = "Specific"
		});
		rType.DataSource = list3;
		List<Common.Table_ID> list4 = new List<Common.Table_ID>();
		list4.Add(new Common.Table_ID
		{
			ID = "A",
			Name = "Approved"
		});
		list4.Add(new Common.Table_ID
		{
			ID = "F",
			Name = "On Hold"
		});
		list4.Add(new Common.Table_ID
		{
			ID = "D",
			Name = "Draft"
		});
		list4.Add(new Common.Table_ID
		{
			ID = "C",
			Name = "Canceled"
		});
		rStatusDF.DataSource = list4;
		List<Common.Table_ID> list5 = new List<Common.Table_ID>();
		list5.Add(new Common.Table_ID
		{
			ID = "A",
			Name = "Approved"
		});
		list5.Add(new Common.Table_ID
		{
			ID = "D",
			Name = "Draft"
		});
		rStatusD.DataSource = list5;
		List<Common.Table_ID> list6 = new List<Common.Table_ID>();
		list6.Add(new Common.Table_ID
		{
			ID = "A",
			Name = "Approved"
		});
		list6.Add(new Common.Table_ID
		{
			ID = "F",
			Name = "On Hold"
		});
		list6.Add(new Common.Table_ID
		{
			ID = "T",
			Name = "Terminated"
		});
		rStatusA.DataSource = list6;
		List<Common.Table_ID> list7 = new List<Common.Table_ID>();
		list7.Add(new Common.Table_ID
		{
			ID = "D",
			Name = "Day(s)"
		});
		list7.Add(new Common.Table_ID
		{
			ID = "W",
			Name = "Week(s)"
		});
		list7.Add(new Common.Table_ID
		{
			ID = "M",
			Name = "Month(s)"
		});
		rRemindUnit.DataSource = list7;
		List<Common.Table_ID> list8 = new List<Common.Table_ID>();
		list8.Add(new Common.Table_ID
		{
			ID = "O",
			Name = "Open"
		});
		list8.Add(new Common.Table_ID
		{
			ID = "C",
			Name = "Closed"
		});
		rlineStatus.DataSource = list8;
		txtFromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
		txtToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
		await GetDoc();
		rSettleProb.Mask.MaskType = MaskType.Numeric;
		rSettleProb.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		rSettleProb.Mask.UseMaskAsDisplayFormat = true;
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			dtContact = await helper.GetAsync("GetNameCntctCode");
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rListNum;
			repositoryItemLookUpEdit.DataSource = await helper.GetAsync("GetPriceList");
			repositoryItemLookUpEdit = rUgpEntry;
			repositoryItemLookUpEdit.DataSource = await HttpClientHelperFactory.GetHttpClient().Commons.GetDataUomGroup();
			repositoryItemLookUpEdit = rOwner;
			repositoryItemLookUpEdit.DataSource = await helper.GetAsync("GetOwner");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		ChangeAllowEdit(allowEdit: false);
		rMethod.Enabled = false;
		rMethod.ShowDropDown = ShowDropDown.Never;
	}

	public virtual async Task GetDoc()
	{
		try
		{
			DocumentSBAImpl documentSBAImpl = new DocumentSBAImpl();
			if (AbsID != 0)
			{
				documentSBAImpl.AbsID = AbsID;
				AbsID = 0;
			}
			else
			{
				if (Function.ParseInt(txtNoOfTrans.EditValue) > 0)
				{
					documentSBAImpl.Number = Function.ParseInt(txtNoOfTrans.EditValue);
				}
				else
				{
					documentSBAImpl.StartDate = txtFromDate.DateTime;
					documentSBAImpl.EndDate = txtToDate.DateTime;
				}
				documentSBAImpl.Status = Function.ToString(cbxStatus.EditValue);
				documentSBAImpl.BpCode = Function.ToString(txtBusinessPartner.EditValue);
				documentSBAImpl.BpType = (FormId.Equals("oSalesBlanketAgreement") ? "C" : "S");
			}
			DataTable dataTable = await _httpClient.BlanketAgreement.GetDoc(documentSBAImpl);
			if (OpenFormDocument != null || OpenFormLines != null)
			{
				dataTable = dataTable.Clone();
			}
			grdDoc.DataSource = dataTable;
			grdDoc.RefreshDataSource();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				detail = Function.ParseInt(dataTable.Rows[0]["AbsID"]);
			}
			else
			{
				detail = -1;
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual async Task LoadDetail(int absId)
	{
		try
		{
			if (_isMapping)
			{
				return;
			}
			DataTable dataTable = await _httpClient.BlanketAgreement.GetLine(absId);
			grdDetails.DataSource = dataTable;
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("Method")) == "I")
			{
				VisibleColumnByAgreeMethod("I");
			}
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("Method")) == "M")
			{
				VisibleColumnByAgreeMethod("M");
			}
			dtDetail = _httpClient.BlanketAgreement.GetBlanketAgreementDetails(new Tuple<int, int>(absId, 0));
			if (dtDetail.IsNotNull() && !dtDetail.IsExistsColumn("ItemCode"))
			{
				dtDetail.Columns.Add("ItemCode", typeof(string));
				dtDetail.Columns.Add("ItemName", typeof(string));
				dtDetail.AcceptChanges();
			}
			if (dataTable.IsNotNull())
			{
				foreach (DataRow row in dataTable.Rows)
				{
					double num = Function.ParseDouble(row["PlanQty"]);
					double num2 = Function.ParseDouble(row["UnitPrice"]);
					double exchangeRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(Function.ToString(row["Currency"]), DateTime.Now.Date);
					row["Rate"] = exchangeRate;
					row["OpenAmntFC"] = num * num2;
					row["OpenAmntLC"] = num * num2 * exchangeRate;
				}
				dataTable.AcceptChanges();
			}
			AddNewLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual async Task GetListItemGroup()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rItemGroup;
			repositoryItemLookUpEdit.DataSource = await helper.GetAsync("GetItemGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public DataTable GetContactPerson(string code)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return httpClientHelper.Get("GetContactPerson", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public virtual int GetNumber()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			DataTable dataTable = httpClientHelper.Get("GetNumerMax", new
			{
				series = (FormId.Equals("oSalesBlanketAgreement") ? 47 : 48)
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]) + 1;
			}
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	public virtual async Task<string> Save(DataTable doc, DataTable dtLines, DataTable dtDetails, DataTable dtClgCode)
	{
		try
		{
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(doc.Copy());
			dataSet.Tables.Add(dtLines.Copy());
			dataSet.Tables.Add(dtDetails.Copy());
			dataSet.Tables.Add(dtClgCode.Copy());
			using HttpClientHelper<string> helprer = new HttpClientHelper<string>(Configuration.ApiServer, "SalesBlanketAgreement");
			return await helprer.PostParamsAsJsonAsync("Save", dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	public virtual int GetOwnerByBPCode(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SalesBlanketAgreement");
			return httpClientHelper.GetInt("GetOwnerByBPCode", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public virtual void ChangeAllowEdit(bool allowEdit)
	{
		gcBPName.OptionsColumn.AllowEdit = allowEdit;
		gcSeries.OptionsColumn.AllowEdit = allowEdit;
		gcNumber.OptionsColumn.AllowEdit = allowEdit;
	}

	private double GetExchangeRate(string curr, DateTime date)
	{
		try
		{
			return HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(curr, date);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1.0;
		}
	}

	public virtual void LoadCurrency()
	{
		try
		{
			rep_Currency.DataSource = GlobalData.Currencies;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual void AddNewRowDoc()
	{
		DataTable dataTable = grdDoc.DataSourceTable.Clone();
		DataRow dataRow = dataTable.NewRow();
		dataRow["AbsID"] = 0;
		dataRow["CntctCode"] = DBNull.Value;
		if (OpenFormDocument == null && OpenFormLines == null)
		{
			dataRow["Series"] = (FormId.Equals("oSalesBlanketAgreement") ? 47 : 48);
			dataRow["Number"] = GetNumber();
		}
		dataRow["Method"] = 'I';
		dataRow["StartDate"] = DateTime.Today;
		dataRow["SignDate"] = DateTime.Today;
		dataRow["Type"] = 'G';
		dataRow["UseDiscnt"] = 'Y';
		dataRow["Status"] = 'D';
		dataRow["Owner"] = 57;
		dataRow["Renewal"] = 'N';
		dataRow["SettleProb"] = 0;
		dataRow["RemindUnit"] = "D";
		dataTable.Rows.Add(dataRow);
		grdDoc.DataSource = dataTable;
		grdDoc.RefreshDataSource();
		FormMode = Enums.FormViewModeType.Add;
	}

	public virtual void AddNewLine()
	{
		DataTable dataSourceTable = grdDoc.DataSourceTable;
		if (dataSourceTable == null || dataSourceTable.Rows.Count <= 0)
		{
			return;
		}
		DataTable dataSourceTable2 = grdDetails.DataSourceTable;
		if (dataSourceTable2.Select("isnull(ItemCode,'') = ''").Length != 0)
		{
			return;
		}
		DataRow dataRow = dataSourceTable2.NewRow();
		if (dataRow.Table.Columns.Contains("AgrNo"))
		{
			dataRow["AgrNo"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
		}
		if (dataRow.Table.Columns.Contains("AgrLineNum"))
		{
			dataRow["AgrLineNum"] = ((dataSourceTable2.Rows.Count == 0) ? 1 : (dataSourceTable2.AsEnumerable().Max((DataRow r) => Function.ParseInt(r["AgrLineNum"])) + 1));
		}
		if (dataRow.Table.Columns.Contains("LineStatus"))
		{
			dataRow["LineStatus"] = "O";
		}
		dataSourceTable2.Rows.Add(dataRow);
		grdDetails.DataSource = dataSourceTable2;
		grdDetails.DataSourceTable.AcceptChanges();
		grdDetails.RefreshDataSource();
	}

	public virtual bool CheckItemCode(string itemCode, DataTable dt, int sourcerowindex)
	{
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (i != sourcerowindex && Function.ToString(dt.Rows[i]["ItemCode"]) == itemCode)
				{
					WinCommons.Error(Enums.MessageCode.YouCannotSelectSameItemTwice, "");
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (msg.Msg == 256 || msg.Msg == 260)
			{
				switch (keyData)
				{
				case Keys.Return:
					btnSave_Click(null, null);
					break;
				case Keys.Tab:
					EventTabkey();
					break;
				}
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	public async Task REFRESH()
	{
		try
		{
			btn_Cancel.Enabled = true;
			AddNewRowDoc();
			AddNewLine();
			ChangeAllowEdit(allowEdit: true);
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
			await GetDoc();
			await LoadDetail(detail);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public virtual double InitLineCellValueChanged(DataRow drDetail)
	{
		try
		{
			double rate = 0.0;
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gcBpCurr));
			if (text.ToUpper() == GlobalData.MainCurrency.ToUpper())
			{
				rate = 1.0;
			}
			else
			{
				if (string.IsNullOrEmpty(text))
				{
					text = GlobalData.MainCurrency;
				}
				DateTime date = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("SignDate"));
				rate = GetExchangeRate(text, date);
				if (rate == 0.0)
				{
					frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
					frm.Curr = text;
					frm._date = date;
					frm.FormClosing += (object s, FormClosingEventArgs e1) =>
					{
						rate = Function.ParseDouble(frm.rate);
					};
					frm.ShowDialog(this);
				}
			}
			return Function.ParseDouble(drDetail["UnitPrice"]) * Function.ParseDouble(drDetail["PlanQty"]) * rate;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0.0;
	}

	private void MergeDataDetails(DataTable dtDetails)
	{
		if (dtDetail != null && dtDetail.Rows.Count > 0)
		{
			foreach (DataRow row in dtDetails.Rows)
			{
				int agrNo = Function.ParseInt(row["AgrNo"]);
				int agrLnNum = Function.ParseInt(row["AgrLnNum"]);
				int agrEfctNum = Function.ParseInt(row["AgrEfctNum"]);
				List<DataRow> list = (from s in dtDetail.AsEnumerable()
					where Function.ParseInt(s["AgrNo"]) == agrNo && Function.ParseInt(s["AgrLnNum"]) == agrLnNum && Function.ParseInt(s["AgrEfctNum"]) == agrEfctNum
					select s).ToList();
				if (list.Count > 0)
				{
					foreach (DataRow item in list)
					{
						dtDetail.Rows.Remove(item);
					}
					dtDetail.AcceptChanges();
				}
				DataRow dataRow2 = dtDetail.NewRow();
				dataRow2.ItemArray = row.ItemArray;
				dtDetail.Rows.Add(dataRow2);
			}
			return;
		}
		dtDetail = dtDetails.Copy();
	}

	private void EventTabkey()
	{
		try
		{
			if (txtBusinessPartner.Focused)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, Function.ToString(txtBusinessPartner.Text), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					txtBusinessPartner.Text = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
				}
			}
			if (gvDoc.FocusedColumn.FieldName == "BpCode" && gvDoc.IsEditorFocused)
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					return;
				}
				string txtSeach = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBPCode));
				frm_CFL frm_CFL3 = new frm_CFL(_bpType, false, txtSeach, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL3.ShowDialog(this);
				DataRow chooseItem = frm_CFL3.ChooseItem;
				if (chooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("BpCode", chooseItem["CardCode"]);
					gvDoc.SetFocusedRowCellValue("BpName", chooseItem["CardName"]);
					DataTable contactPerson = GetContactPerson(Function.ToString(chooseItem["CardCode"]));
					int ownerByBPCode = GetOwnerByBPCode(Function.ToString(chooseItem["CardCode"]));
					if (contactPerson != null && contactPerson.Rows.Count > 0)
					{
						chooseItem = contactPerson.Rows[0];
						rCntctCode.DataSource = contactPerson;
						gvDoc.SetFocusedRowCellValue("CntctCode", chooseItem["CntctCode"]);
						gvDoc.SetFocusedRowCellValue("Tel1", chooseItem["Tel1"]);
						gvDoc.SetFocusedRowCellValue("E_MailL", chooseItem["E_MailL"]);
					}
					else
					{
						gvDoc.SetFocusedRowCellValue("CntctCode", 0);
						gvDoc.SetFocusedRowCellValue("Tel1", "");
						gvDoc.SetFocusedRowCellValue("E_MailL", "");
					}
					gvDoc.SetFocusedRowCellValue("Owner", ownerByBPCode);
				}
				grdDoc.RefreshDataSource();
				gvDoc.UpdateCurrentRow();
			}
			if (!(gvDetails.FocusedColumn.FieldName == "ItemCode") || !gvDetails.IsEditorFocused || string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("BpCode"))))
			{
				return;
			}
			DataTable dataTable = grdDetails.DataSource as DataTable;
			string txtSeach2 = Function.ToString(gvDetails.GetRowCellValue(gvDoc.FocusedRowHandle, gcItemCode));
			frm_CFL frm_CFL4 = new frm_CFL(Enums.TypeCFL.ListItems, true, txtSeach2, "O", Enums.ItemObjectType.SellItem, null, "");
			frm_CFL4.ShowDialog(this);
			List<DataRow> chooseItems = frm_CFL4.ChooseItems;
			if (chooseItems != null && chooseItems.Count > 0)
			{
				int num = gvDetails.FocusedRowHandle;
				if (dataTable != null)
				{
					dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
					foreach (DataRow item in chooseItems)
					{
						ChooseFromListItemCode(Function.ToString(item["ItemCode"]), dataTable, num);
						num++;
					}
					if (!string.IsNullOrEmpty(Function.ToString(gvDetails.GetRowCellValue(gvDetails.RowCount - 1, "ItemCode"))) && !Function.ToString(gvDoc.GetFocusedRowCellValue("Method")).Equals("M"))
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["LineStatus"] = "O";
						dataTable.Rows.InsertAt(dataRow, num);
					}
				}
			}
			grdDetails.DataSource = dataTable;
			grdDetails.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChooseFromListItemCode(string itemCode, DataTable dtLine, int index)
	{
		DataRow dataRow = dtLine.NewRow();
		DataTable dataItemCode = HttpClientHelperFactory.GetHttpClient().Commons.GetDataItemCode(itemCode, -1, GlobalData.UserName, GlobalData.DefaultBranch);
		if (dataItemCode != null && dataItemCode.Rows.Count > 0)
		{
			int dataSourceRowIndex = gvDetails.GetDataSourceRowIndex(index);
			if (!CheckItemCode(Function.ToString(dataItemCode.Rows[0]["ItemCode"]), dtLine, dataSourceRowIndex))
			{
				if (index == gvDetails.FocusedRowHandle)
				{
					gvDetails.DeleteRow(index);
					grdDetails.DataSourceTable.AcceptChanges();
				}
				dataRow["AgrNo"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
				dataRow["AgrLineNum"] = index + 1;
				dataRow["ItemCode"] = Function.ToString(dataItemCode.Rows[0]["ItemCode"]);
				dataRow["ItemName"] = Function.ToString(dataItemCode.Rows[0]["ItemName"]);
				dataRow["ItemGroup"] = Function.ParseInt(dataItemCode.Rows[0]["ItmsGrpCod"]);
				dataRow["UgpEntry"] = Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]);
				dataRow["InvntryUom"] = Function.ToString(dataItemCode.Rows[0]["InvntryUom"]);
				dataRow["NumPerMsr"] = Function.ParseDecimal(dataItemCode.Rows[0]["NumInInven"]);
				dataRow["SalUnitMsr"] = dataItemCode.Rows[0]["SalUnitMsr"];
				dataRow["NumInSale"] = dataItemCode.Rows[0]["NumInSale"];
				dataRow["BuyUnitMsr"] = Function.ToString(dataItemCode.Rows[0]["BuyUnitMsr"]);
				dataRow["NumInBuy"] = Function.ParseDouble(dataItemCode.Rows[0]["NumInBuy"]);
				dataRow["NumPerMsr"] = 1;
				dataRow["UgpCode"] = dataItemCode.Rows[0]["UgpEntry"];
				dataRow["InvUnitMsr"] = dataItemCode.Rows[0]["InvntryUom"];
				dataRow["NumInInven"] = dataItemCode.Rows[0]["NumInInven"];
				dataRow["Currency"] = Function.ToString(gvDoc.GetFocusedRowCellValue(gcBpCurr));
				dataRow["Rate"] = 1;
				dataRow["UnitPrice"] = 0;
				if (Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]) == -1)
				{
					dataRow["UomCode"] = "Manual";
				}
				else
				{
					dataRow["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]), Function.ToString(dataItemCode.Rows[0]["InvntryUom"]));
				}
				dataRow["LineStatus"] = "O";
				dtLine.Rows.InsertAt(dataRow, index);
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
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

	private void txtNoOfTrans_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(txtNoOfTrans.EditValue) > 0)
			{
				txtFromDate.EditValue = string.Empty;
				txtToDate.EditValue = string.Empty;
				txtBusinessPartner.EditValue = string.Empty;
				cbxStatus.EditValue = string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (isLoadDetail)
		{
			return;
		}
		try
		{
			WinCommons._OpenProcessing();
			int absId = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
			await LoadDetail(absId);
			_currentRowStatus = Function.ToString(gvDoc.GetFocusedRowCellValue("Status"));
			DateTime dateTime = Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("EndDate"));
			if (_currentRowStatus.Equals("A") && dateTime > DateTime.Now.Date)
			{
				drlCopyTo.Enabled = true;
			}
			else
			{
				drlCopyTo.Enabled = false;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void LoadCopyMenu()
	{
		try
		{
			if (PopCopyTo == null || PopCopyTo.Count <= 0)
			{
				return;
			}
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			foreach (ApzObjectTypes item in PopCopyTo)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				switch (item)
				{
				case ApzObjectTypes.oQuotations:
					dXMenuItem.Caption = MenuCaption.SalesQuotation;
					break;
				case ApzObjectTypes.oOrders:
					dXMenuItem.Caption = MenuCaption.SalesOrder;
					break;
				case ApzObjectTypes.oDeliveryNotes:
					dXMenuItem.Caption = MenuCaption.Delivery;
					break;
				case ApzObjectTypes.oInvoices:
					dXMenuItem.Caption = MenuCaption.ARInvoices;
					break;
				case ApzObjectTypes.oPurchaseQuotations:
					dXMenuItem.Caption = MenuCaption.PurchaseQuotation;
					break;
				case ApzObjectTypes.oPurchaseOrders:
					dXMenuItem.Caption = MenuCaption.PurchaseOder;
					break;
				case ApzObjectTypes.oPurchaseDeliveryNotes:
					dXMenuItem.Caption = MenuCaption.GoodReceiptPO;
					break;
				}
				ApzObjectTypes objType = item;
				dXMenuItem.Click += (object s, EventArgs e) =>
				{
					try
					{
						WinFuntions.CopyBlanketToDocument(_bpType, objType, this, gvDoc);
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				dXPopupMenu.Items.Add(dXMenuItem);
			}
			drlCopyTo.DropDownControl = dXPopupMenu;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ctxMenu_Cancel_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			int docEntry = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
			if (!Function.ToString(gvDoc.GetFocusedRowCellValue("Status")).Equals("C") && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancel, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				string text = await _httpClient.BlanketAgreement.Cancel(docEntry);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await GetDoc();
					detail = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID"));
					await LoadDetail(detail);
					ChangeAllowEdit(allowEdit: false);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvDoc_RowUpdated(object sender, RowObjectEventArgs e)
	{
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void ctxMenu_Duplicate_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.FocusedRowHandle > -1)
			{
				isLoadDetail = true;
				DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
				focusedDataRow["AbsID"] = 0;
				focusedDataRow["Number"] = 0;
				focusedDataRow["Status"] = "D";
				object[] itemArray = focusedDataRow.ItemArray;
				DataTable dataTable = grdDoc.DataSourceTable();
				dataTable.Rows.Clear();
				dataTable.Rows.Add(itemArray);
				DataTable dataTable2 = grdDetails.DatasourceTable();
				if (dataTable2.IsNotNull())
				{
					for (int i = 0; i < dataTable2.Rows.Count; i++)
					{
						if (dataTable2.Columns.Contains("AgrNo"))
						{
							dataTable2.Rows[i]["AgrNo"] = 0;
						}
						if (dataTable2.Columns.Contains("AgrLineNum"))
						{
							dataTable2.Rows[i]["AgrLineNum"] = i + 1;
						}
						if (dataTable2.Columns.Contains("LineStatus"))
						{
							dataTable2.Rows[i]["LineStatus"] = "O";
						}
					}
				}
				btn_Cancel.Enabled = true;
				FormMode = Enums.FormViewModeType.Add;
				ChangeAllowEdit(allowEdit: true);
				rMethod.Enabled = true;
				rMethod.ShowDropDown = ShowDropDown.SingleClick;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isLoadDetail = false;
	}

	private void relatedActivitiesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsID")) <= 0)
			{
				WinCommons.Warning(Enums.MessageCode.DataIsNotAvailable, "");
				return;
			}
			frm_RelatedActivites frm_RelatedActivites2 = new frm_RelatedActivites(ApzObjectTypes.oBlanketAgreement, Function.ToString(gvDoc.GetFocusedRowCellValue("AbsID")));
			frm_RelatedActivites2.DocNum = Function.ToString(gvDoc.GetFocusedRowCellValue("Number"));
			frm_RelatedActivites2.MdiParent = MdiParent;
			frm_RelatedActivites2.Show();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_SalesBlanketAgreement));
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx8 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx9 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx10 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx11 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx12 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextDocMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.iNSERTToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ctxMenu_Duplicate = new System.Windows.Forms.ToolStripMenuItem();
		this.ctxMenu_Cancel = new System.Windows.Forms.ToolStripMenuItem();
		this.listBussissToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.relatedActivitiesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcSeries = new Apzon.Controls.Grid.GridColumnEx();
		this.rSeries = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBPCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rBpCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcBPName = new Apzon.Controls.Grid.GridColumnEx();
		this.rBpName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcContactPerson = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTelephoneNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEmail = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgreementMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.rMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcStartDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcEndDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTerminationDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSigningDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAgreementType = new Apzon.Controls.Grid.GridColumnEx();
		this.rType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcUserDiscnt = new Apzon.Controls.Grid.GridColumnEx();
		this.rUseDiscnt = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rStatusDF = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rListNum = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcOwner = new Apzon.Controls.Grid.GridColumnEx();
		this.rOwner = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRenewal = new Apzon.Controls.Grid.GridColumnEx();
		this.rRenewal = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcSettlementProbability = new Apzon.Controls.Grid.GridColumnEx();
		this.rSettleProb = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcRemindVal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemindUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.rRemindUnit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAbsID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rProject = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcBpCurr = new Apzon.Controls.Grid.GridColumnEx();
		this.rBpCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcBpType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPaymentTerm = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_PaymentTerm = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPayMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_PayMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rStatusD = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rCntctCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rStatusA = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tabControl = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabPageDetails = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdDetails = new Apzon.Controls.Grid.GridControlEx();
		this.contextLineMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.listItemToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDetails = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcAgrLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcItemGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPlanQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUnitPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCumQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCumulativeAmountLC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCumulativeAmountFC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPlanAmtLC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPlanAmtFC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpenQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpenAmountLC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcOpenAmountFC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFreeText = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rUomCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcUoMName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNumPerMsr = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUoMGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rUgpEntry = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRetPortion = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWrrtyEnd = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rlineStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gvAgrNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCurrency = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLine_Project = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTaxCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rTaxCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rPlanQty = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rRetPortion = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rWrrtyEnd = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rPlanAmount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rDiscount = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tabAttachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.drlCopyTo = new Apzon.Controls.Controls.DropDownButtonEx();
		this.txtBusinessPartner = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lb_DocStatus = new Apzon.Controls.Controls.LabelLine();
		this.lb_BusinessPartner = new Apzon.Controls.Controls.LabelLine();
		this.lb_DocNumber = new Apzon.Controls.Controls.LabelLine();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtNoOfTrans = new Apzon.Controls.Editor.TextEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.cbxStatus = new Apzon.Controls.Editor.LookUpEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx11).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx12).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextDocMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSeries).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBpName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUseDiscnt).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusDF).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rListNum).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOwner).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRenewal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSettleProb).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRemindUnit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentTerm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PayMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusD).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCntctCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusA).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).BeginInit();
		this.tabControl.SuspendLayout();
		this.tabPageDetails.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDetails).BeginInit();
		this.contextLineMenu.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDetails).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUgpEntry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlineStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPlanQty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRetPortion).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWrrtyEnd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWrrtyEnd.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPlanAmount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDiscount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		this.tabAttachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtBusinessPartner.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbxStatus.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		this.repositoryItemCalcEditEx6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx6.IsSearchControl = false;
		this.repositoryItemCalcEditEx6.Name = "repositoryItemCalcEditEx6";
		this.repositoryItemCalcEditEx7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx7.IsSearchControl = false;
		this.repositoryItemCalcEditEx7.Name = "repositoryItemCalcEditEx7";
		this.repositoryItemCalcEditEx8.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx8.IsSearchControl = false;
		this.repositoryItemCalcEditEx8.Name = "repositoryItemCalcEditEx8";
		this.repositoryItemCalcEditEx9.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx9.IsSearchControl = false;
		this.repositoryItemCalcEditEx9.Name = "repositoryItemCalcEditEx9";
		this.repositoryItemCalcEditEx10.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx10.IsSearchControl = false;
		this.repositoryItemCalcEditEx10.Name = "repositoryItemCalcEditEx10";
		this.repositoryItemCalcEditEx11.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx11.IsSearchControl = false;
		this.repositoryItemCalcEditEx11.Name = "repositoryItemCalcEditEx11";
		this.repositoryItemCalcEditEx12.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx12.IsSearchControl = false;
		this.repositoryItemCalcEditEx12.Name = "repositoryItemCalcEditEx12";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextDocMenu;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[20]
		{
			this.rBpCode, this.rBpName, this.rSeries, this.rMethod, this.rDate, this.rType, this.rStatusD, this.rUseDiscnt, this.rOwner, this.rRenewal,
			this.rSettleProb, this.rRemindUnit, this.rCntctCode, this.rListNum, this.rStatusA, this.rStatusDF, this.rProject, this.rBpCurrency, this.luk_PaymentTerm, this.luk_PayMethod
		});
		this.grdDoc.Size = new System.Drawing.Size(1316, 242);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextDocMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextDocMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.iNSERTToolStripMenuItem, this.ctxMenu_Duplicate, this.ctxMenu_Cancel, this.listBussissToolStripMenuItem, this.relatedActivitiesToolStripMenuItem });
		this.contextDocMenu.Name = "contextMenuStrip1";
		this.contextDocMenu.Size = new System.Drawing.Size(234, 124);
		this.iNSERTToolStripMenuItem.Name = "iNSERTToolStripMenuItem";
		this.iNSERTToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
		this.iNSERTToolStripMenuItem.Text = "Add";
		this.iNSERTToolStripMenuItem.Click += new System.EventHandler(this.iNSERTToolStripMenuItem_Click);
		this.ctxMenu_Duplicate.Name = "ctxMenu_Duplicate";
		this.ctxMenu_Duplicate.Size = new System.Drawing.Size(233, 24);
		this.ctxMenu_Duplicate.Text = "Duplicate";
		this.ctxMenu_Duplicate.Click += new System.EventHandler(this.ctxMenu_Duplicate_Click);
		this.ctxMenu_Cancel.Name = "ctxMenu_Cancel";
		this.ctxMenu_Cancel.Size = new System.Drawing.Size(233, 24);
		this.ctxMenu_Cancel.Text = "Cancel";
		this.ctxMenu_Cancel.Click += new System.EventHandler(this.ctxMenu_Cancel_Click);
		this.listBussissToolStripMenuItem.Name = "listBussissToolStripMenuItem";
		this.listBussissToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
		this.listBussissToolStripMenuItem.Text = "List of Business Partners";
		this.listBussissToolStripMenuItem.Click += new System.EventHandler(this.listBussissToolStripMenuItem_Click);
		this.relatedActivitiesToolStripMenuItem.Name = "relatedActivitiesToolStripMenuItem";
		this.relatedActivitiesToolStripMenuItem.Size = new System.Drawing.Size(233, 24);
		this.relatedActivitiesToolStripMenuItem.Text = "Related Activities";
		this.relatedActivitiesToolStripMenuItem.Click += new System.EventHandler(this.relatedActivitiesToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[29]
		{
			this.gcSeries, this.gcNumber, this.gcBPCode, this.gcBPName, this.gcContactPerson, this.gcTelephoneNo, this.gcEmail, this.gcAgreementMethod, this.gcStartDate, this.gcEndDate,
			this.gcTerminationDate, this.gcSigningDate, this.gcDescription, this.gcAgreementType, this.gcUserDiscnt, this.gcStatus, this.gcPriceList, this.gcOwner, this.gcRenewal, this.gcSettlementProbability,
			this.gcRemindVal, this.gcRemindUnit, this.gcRemarks, this.gcAbsID, this.gcProject, this.gcBpCurr, this.gcBpType, this.gcPaymentTerm, this.gcPayMethod
		});
		this.gvDoc.DetailHeight = 431;
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oSalesBlanketAgreement";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 47;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OOAT";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsCustomization.AllowSort = false;
		this.gvDoc.OptionsDetail.EnableMasterViewMode = false;
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEdit);
		this.gvDoc.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvDoc_CustomRowCellEditForEditing);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.RowUpdated += new DevExpress.XtraGrid.Views.Base.RowObjectEventHandler(this.gvDoc_RowUpdated);
		this.gvDoc.ColumnFilterChanged += new System.EventHandler(this.gvDoc_ColumnFilterChanged);
		this.gcSeries.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcSeries.AppearanceCell.Options.UseBackColor = true;
		this.gcSeries.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcSeries.Caption = "Series";
		this.gcSeries.ColumnEdit = this.rSeries;
		this.gcSeries.CurrencyDateField = null;
		this.gcSeries.CurrencyField = null;
		this.gcSeries.CurrencyFixed = null;
		this.gcSeries.CurrencyFixedRate = 0.0;
		this.gcSeries.CurrencyRateField = null;
		this.gcSeries.FieldName = "Series";
		this.gcSeries.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcSeries.MinWidth = 23;
		this.gcSeries.Name = "gcSeries";
		this.gcSeries.OptionsColumn.ReadOnly = true;
		this.gcSeries.PrimaryKey = true;
		this.gcSeries.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcSeries.UnitField = null;
		this.gcSeries.Visible = true;
		this.gcSeries.VisibleIndex = 0;
		this.gcSeries.Width = 89;
		this.rSeries.AutoHeight = false;
		this.rSeries.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
		});
		this.rSeries.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "", 20, DevExpress.Utils.FormatType.Numeric, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rSeries.DisplayMember = "Text";
		this.rSeries.Name = "rSeries";
		this.rSeries.NullText = "";
		this.rSeries.ShowFooter = false;
		this.rSeries.ShowHeader = false;
		this.rSeries.ValueMember = "Value";
		this.rSeries.EditValueChanged += new System.EventHandler(this.rSeries_EditValueChanged);
		this.gcNumber.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcNumber.AppearanceCell.Options.UseBackColor = true;
		this.gcNumber.AppearanceHeader.Options.UseTextOptions = true;
		this.gcNumber.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcNumber.Caption = "Number";
		this.gcNumber.CurrencyDateField = null;
		this.gcNumber.CurrencyField = null;
		this.gcNumber.CurrencyFixed = null;
		this.gcNumber.CurrencyFixedRate = 0.0;
		this.gcNumber.CurrencyRateField = null;
		this.gcNumber.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcNumber.FieldName = "Number";
		this.gcNumber.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcNumber.MinWidth = 23;
		this.gcNumber.Name = "gcNumber";
		this.gcNumber.OptionsColumn.ReadOnly = true;
		this.gcNumber.PrimaryKey = false;
		this.gcNumber.UnitField = null;
		this.gcNumber.Visible = true;
		this.gcNumber.VisibleIndex = 1;
		this.gcNumber.Width = 84;
		this.gcBPCode.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcBPCode.AppearanceCell.Options.UseBackColor = true;
		this.gcBPCode.AppearanceHeader.Options.UseTextOptions = true;
		this.gcBPCode.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcBPCode.Caption = "BP Code";
		this.gcBPCode.ColumnEdit = this.rBpCode;
		this.gcBPCode.CurrencyDateField = null;
		this.gcBPCode.CurrencyField = null;
		this.gcBPCode.CurrencyFixed = null;
		this.gcBPCode.CurrencyFixedRate = 0.0;
		this.gcBPCode.CurrencyRateField = null;
		this.gcBPCode.FieldName = "BpCode";
		this.gcBPCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcBPCode.MinWidth = 23;
		this.gcBPCode.Name = "gcBPCode";
		this.gcBPCode.PrimaryKey = false;
		this.gcBPCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcBPCode.UnitField = null;
		this.gcBPCode.Visible = true;
		this.gcBPCode.VisibleIndex = 2;
		this.gcBPCode.Width = 117;
		this.rBpCode.AutoHeight = false;
		this.rBpCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rBpCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBpCode.ContainerControl = this;
		this.rBpCode.DisplayMember = "";
		this.rBpCode.DisplayMember2 = "";
		this.rBpCode.LoadDataSourceWhenEnter = false;
		this.rBpCode.Name = "rBpCode";
		this.rBpCode.SeparateCharacterForMulti = ';';
		this.rBpCode.SeparateString = ", ";
		this.rBpCode.UsingAutoCompleteBox = false;
		this.rBpCode.UsingDataSource = true;
		this.rBpCode.ValidateWhenNoDataSource = false;
		this.rBpCode.ValueMember = "";
		this.rBpCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBpCode_ButtonClick);
		this.rBpCode.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rBpCode_EditValueChanging);
		this.gcBPName.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcBPName.AppearanceCell.Options.UseBackColor = true;
		this.gcBPName.AppearanceHeader.Options.UseTextOptions = true;
		this.gcBPName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcBPName.Caption = "BP Name";
		this.gcBPName.ColumnEdit = this.rBpName;
		this.gcBPName.CurrencyDateField = null;
		this.gcBPName.CurrencyField = null;
		this.gcBPName.CurrencyFixed = null;
		this.gcBPName.CurrencyFixedRate = 0.0;
		this.gcBPName.CurrencyRateField = null;
		this.gcBPName.FieldName = "BpName";
		this.gcBPName.MinWidth = 23;
		this.gcBPName.Name = "gcBPName";
		this.gcBPName.OptionsColumn.ReadOnly = true;
		this.gcBPName.PrimaryKey = false;
		this.gcBPName.UnitField = null;
		this.gcBPName.Visible = true;
		this.gcBPName.VisibleIndex = 3;
		this.gcBPName.Width = 223;
		this.rBpName.AutoHeight = false;
		this.rBpName.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rBpName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBpName.ContainerControl = this;
		this.rBpName.DisplayMember = "";
		this.rBpName.DisplayMember2 = "";
		this.rBpName.LoadDataSourceWhenEnter = false;
		this.rBpName.Name = "rBpName";
		this.rBpName.ReadOnly = true;
		this.rBpName.SeparateCharacterForMulti = ';';
		this.rBpName.SeparateString = ", ";
		this.rBpName.UsingAutoCompleteBox = false;
		this.rBpName.UsingDataSource = true;
		this.rBpName.ValidateWhenNoDataSource = false;
		this.rBpName.ValueMember = "";
		this.rBpName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBpName_ButtonClick);
		this.gcContactPerson.AppearanceHeader.Options.UseTextOptions = true;
		this.gcContactPerson.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcContactPerson.Caption = "Contact Person";
		this.gcContactPerson.CurrencyDateField = null;
		this.gcContactPerson.CurrencyField = null;
		this.gcContactPerson.CurrencyFixed = null;
		this.gcContactPerson.CurrencyFixedRate = 0.0;
		this.gcContactPerson.CurrencyRateField = null;
		this.gcContactPerson.FieldName = "CntctCode";
		this.gcContactPerson.MinWidth = 23;
		this.gcContactPerson.Name = "gcContactPerson";
		this.gcContactPerson.PrimaryKey = false;
		this.gcContactPerson.UnitField = null;
		this.gcContactPerson.Visible = true;
		this.gcContactPerson.VisibleIndex = 4;
		this.gcContactPerson.Width = 138;
		this.gcTelephoneNo.AllowChSet = false;
		this.gcTelephoneNo.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcTelephoneNo.AppearanceCell.Options.UseBackColor = true;
		this.gcTelephoneNo.AppearanceHeader.Options.UseTextOptions = true;
		this.gcTelephoneNo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcTelephoneNo.Caption = "Telephone No.";
		this.gcTelephoneNo.CurrencyDateField = null;
		this.gcTelephoneNo.CurrencyField = null;
		this.gcTelephoneNo.CurrencyFixed = null;
		this.gcTelephoneNo.CurrencyFixedRate = 0.0;
		this.gcTelephoneNo.CurrencyRateField = null;
		this.gcTelephoneNo.FieldName = "Tel1";
		this.gcTelephoneNo.MinWidth = 23;
		this.gcTelephoneNo.Name = "gcTelephoneNo";
		this.gcTelephoneNo.OptionsColumn.ReadOnly = true;
		this.gcTelephoneNo.PrimaryKey = false;
		this.gcTelephoneNo.UnitField = null;
		this.gcTelephoneNo.Visible = true;
		this.gcTelephoneNo.VisibleIndex = 5;
		this.gcTelephoneNo.Width = 103;
		this.gcEmail.AllowChSet = false;
		this.gcEmail.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcEmail.AppearanceCell.Options.UseBackColor = true;
		this.gcEmail.AppearanceHeader.Options.UseTextOptions = true;
		this.gcEmail.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcEmail.Caption = "E-mail";
		this.gcEmail.CurrencyDateField = null;
		this.gcEmail.CurrencyField = null;
		this.gcEmail.CurrencyFixed = null;
		this.gcEmail.CurrencyFixedRate = 0.0;
		this.gcEmail.CurrencyRateField = null;
		this.gcEmail.FieldName = "E_MailL";
		this.gcEmail.MinWidth = 23;
		this.gcEmail.Name = "gcEmail";
		this.gcEmail.OptionsColumn.ReadOnly = true;
		this.gcEmail.PrimaryKey = false;
		this.gcEmail.UnitField = null;
		this.gcEmail.Visible = true;
		this.gcEmail.VisibleIndex = 6;
		this.gcEmail.Width = 176;
		this.gcAgreementMethod.AppearanceHeader.Options.UseTextOptions = true;
		this.gcAgreementMethod.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcAgreementMethod.Caption = "Agreement Method";
		this.gcAgreementMethod.ColumnEdit = this.rMethod;
		this.gcAgreementMethod.CurrencyDateField = null;
		this.gcAgreementMethod.CurrencyField = null;
		this.gcAgreementMethod.CurrencyFixed = null;
		this.gcAgreementMethod.CurrencyFixedRate = 0.0;
		this.gcAgreementMethod.CurrencyRateField = null;
		this.gcAgreementMethod.FieldName = "Method";
		this.gcAgreementMethod.MinWidth = 23;
		this.gcAgreementMethod.Name = "gcAgreementMethod";
		this.gcAgreementMethod.PrimaryKey = false;
		this.gcAgreementMethod.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcAgreementMethod.UnitField = null;
		this.gcAgreementMethod.Visible = true;
		this.gcAgreementMethod.VisibleIndex = 7;
		this.gcAgreementMethod.Width = 128;
		this.rMethod.AutoHeight = false;
		this.rMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
		});
		this.rMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rMethod.DisplayMember = "Name";
		this.rMethod.Name = "rMethod";
		this.rMethod.NullText = "";
		this.rMethod.ShowFooter = false;
		this.rMethod.ShowHeader = false;
		this.rMethod.ValueMember = "ID";
		this.rMethod.EditValueChanged += new System.EventHandler(this.rMethod_EditValueChanged);
		this.gcStartDate.AppearanceHeader.Options.UseTextOptions = true;
		this.gcStartDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcStartDate.Caption = "Start Date";
		this.gcStartDate.ColumnEdit = this.rDate;
		this.gcStartDate.CurrencyDateField = null;
		this.gcStartDate.CurrencyField = null;
		this.gcStartDate.CurrencyFixed = null;
		this.gcStartDate.CurrencyFixedRate = 0.0;
		this.gcStartDate.CurrencyRateField = null;
		this.gcStartDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcStartDate.FieldName = "StartDate";
		this.gcStartDate.MinWidth = 23;
		this.gcStartDate.Name = "gcStartDate";
		this.gcStartDate.PrimaryKey = false;
		this.gcStartDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcStartDate.UnitField = null;
		this.gcStartDate.Visible = true;
		this.gcStartDate.VisibleIndex = 8;
		this.gcStartDate.Width = 108;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.Name = "rDate";
		this.gcEndDate.AppearanceHeader.Options.UseTextOptions = true;
		this.gcEndDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcEndDate.Caption = "End Date";
		this.gcEndDate.ColumnEdit = this.rDate;
		this.gcEndDate.CurrencyDateField = null;
		this.gcEndDate.CurrencyField = null;
		this.gcEndDate.CurrencyFixed = null;
		this.gcEndDate.CurrencyFixedRate = 0.0;
		this.gcEndDate.CurrencyRateField = null;
		this.gcEndDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcEndDate.FieldName = "EndDate";
		this.gcEndDate.MinWidth = 23;
		this.gcEndDate.Name = "gcEndDate";
		this.gcEndDate.PrimaryKey = false;
		this.gcEndDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcEndDate.UnitField = null;
		this.gcEndDate.Visible = true;
		this.gcEndDate.VisibleIndex = 9;
		this.gcEndDate.Width = 111;
		this.gcTerminationDate.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcTerminationDate.AppearanceCell.Options.UseBackColor = true;
		this.gcTerminationDate.AppearanceHeader.Options.UseTextOptions = true;
		this.gcTerminationDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcTerminationDate.Caption = "Termination Date";
		this.gcTerminationDate.ColumnEdit = this.rDate;
		this.gcTerminationDate.CurrencyDateField = null;
		this.gcTerminationDate.CurrencyField = null;
		this.gcTerminationDate.CurrencyFixed = null;
		this.gcTerminationDate.CurrencyFixedRate = 0.0;
		this.gcTerminationDate.CurrencyRateField = null;
		this.gcTerminationDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcTerminationDate.FieldName = "TermDate";
		this.gcTerminationDate.MinWidth = 23;
		this.gcTerminationDate.Name = "gcTerminationDate";
		this.gcTerminationDate.PrimaryKey = false;
		this.gcTerminationDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcTerminationDate.UnitField = null;
		this.gcTerminationDate.Visible = true;
		this.gcTerminationDate.VisibleIndex = 10;
		this.gcTerminationDate.Width = 117;
		this.gcSigningDate.AppearanceHeader.Options.UseTextOptions = true;
		this.gcSigningDate.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcSigningDate.Caption = "Signing Date";
		this.gcSigningDate.ColumnEdit = this.rDate;
		this.gcSigningDate.CurrencyDateField = null;
		this.gcSigningDate.CurrencyField = null;
		this.gcSigningDate.CurrencyFixed = null;
		this.gcSigningDate.CurrencyFixedRate = 0.0;
		this.gcSigningDate.CurrencyRateField = null;
		this.gcSigningDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcSigningDate.FieldName = "SignDate";
		this.gcSigningDate.MinWidth = 23;
		this.gcSigningDate.Name = "gcSigningDate";
		this.gcSigningDate.PrimaryKey = false;
		this.gcSigningDate.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcSigningDate.UnitField = null;
		this.gcSigningDate.Visible = true;
		this.gcSigningDate.VisibleIndex = 11;
		this.gcSigningDate.Width = 85;
		this.gcDescription.AppearanceHeader.Options.UseTextOptions = true;
		this.gcDescription.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcDescription.Caption = "Description";
		this.gcDescription.CurrencyDateField = null;
		this.gcDescription.CurrencyField = null;
		this.gcDescription.CurrencyFixed = null;
		this.gcDescription.CurrencyFixedRate = 0.0;
		this.gcDescription.CurrencyRateField = null;
		this.gcDescription.FieldName = "Descript";
		this.gcDescription.MinWidth = 23;
		this.gcDescription.Name = "gcDescription";
		this.gcDescription.PrimaryKey = false;
		this.gcDescription.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDescription.UnitField = null;
		this.gcDescription.Visible = true;
		this.gcDescription.VisibleIndex = 12;
		this.gcDescription.Width = 89;
		this.gcAgreementType.AppearanceHeader.Options.UseTextOptions = true;
		this.gcAgreementType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcAgreementType.Caption = "Agreement Type";
		this.gcAgreementType.ColumnEdit = this.rType;
		this.gcAgreementType.CurrencyDateField = null;
		this.gcAgreementType.CurrencyField = null;
		this.gcAgreementType.CurrencyFixed = null;
		this.gcAgreementType.CurrencyFixedRate = 0.0;
		this.gcAgreementType.CurrencyRateField = null;
		this.gcAgreementType.FieldName = "Type";
		this.gcAgreementType.MinWidth = 23;
		this.gcAgreementType.Name = "gcAgreementType";
		this.gcAgreementType.PrimaryKey = false;
		this.gcAgreementType.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcAgreementType.UnitField = null;
		this.gcAgreementType.Visible = true;
		this.gcAgreementType.VisibleIndex = 13;
		this.gcAgreementType.Width = 112;
		this.rType.AutoHeight = false;
		this.rType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
		});
		this.rType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rType.DisplayMember = "Name";
		this.rType.Name = "rType";
		this.rType.NullText = "";
		this.rType.ShowFooter = false;
		this.rType.ShowHeader = false;
		this.rType.ValueMember = "ID";
		this.rType.EditValueChanged += new System.EventHandler(this.rType_EditValueChanged);
		this.gcUserDiscnt.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcUserDiscnt.AppearanceCell.Options.UseBackColor = true;
		this.gcUserDiscnt.AppearanceHeader.Options.UseTextOptions = true;
		this.gcUserDiscnt.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcUserDiscnt.Caption = "Ignore Discount/Prices";
		this.gcUserDiscnt.ColumnEdit = this.rUseDiscnt;
		this.gcUserDiscnt.CurrencyDateField = null;
		this.gcUserDiscnt.CurrencyField = null;
		this.gcUserDiscnt.CurrencyFixed = null;
		this.gcUserDiscnt.CurrencyFixedRate = 0.0;
		this.gcUserDiscnt.CurrencyRateField = null;
		this.gcUserDiscnt.FieldName = "UseDiscnt";
		this.gcUserDiscnt.MinWidth = 23;
		this.gcUserDiscnt.Name = "gcUserDiscnt";
		this.gcUserDiscnt.OptionsColumn.ReadOnly = true;
		this.gcUserDiscnt.PrimaryKey = false;
		this.gcUserDiscnt.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcUserDiscnt.UnitField = null;
		this.gcUserDiscnt.Visible = true;
		this.gcUserDiscnt.VisibleIndex = 14;
		this.gcUserDiscnt.Width = 86;
		this.rUseDiscnt.AutoHeight = false;
		this.rUseDiscnt.Name = "rUseDiscnt";
		this.rUseDiscnt.ValueChecked = "Y";
		this.rUseDiscnt.ValueUnchecked = "N";
		this.gcStatus.AppearanceHeader.Options.UseTextOptions = true;
		this.gcStatus.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcStatus.Caption = "Status";
		this.gcStatus.ColumnEdit = this.rStatusDF;
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "Status";
		this.gcStatus.MinWidth = 23;
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 15;
		this.gcStatus.Width = 101;
		this.rStatusDF.AutoHeight = false;
		this.rStatusDF.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatusDF.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rStatusDF.DisplayMember = "Name";
		this.rStatusDF.Name = "rStatusDF";
		this.rStatusDF.NullText = "";
		this.rStatusDF.ShowHeader = false;
		this.rStatusDF.ShowLines = false;
		this.rStatusDF.ValueMember = "ID";
		this.gcPriceList.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcPriceList.AppearanceCell.Options.UseBackColor = true;
		this.gcPriceList.Caption = "Price list";
		this.gcPriceList.ColumnEdit = this.rListNum;
		this.gcPriceList.CurrencyDateField = null;
		this.gcPriceList.CurrencyField = null;
		this.gcPriceList.CurrencyFixed = null;
		this.gcPriceList.CurrencyFixedRate = 0.0;
		this.gcPriceList.CurrencyRateField = null;
		this.gcPriceList.FieldName = "ListNum";
		this.gcPriceList.MinWidth = 23;
		this.gcPriceList.Name = "gcPriceList";
		this.gcPriceList.PrimaryKey = false;
		this.gcPriceList.UnitField = null;
		this.gcPriceList.Visible = true;
		this.gcPriceList.VisibleIndex = 16;
		this.gcPriceList.Width = 87;
		this.rListNum.AutoHeight = false;
		this.rListNum.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rListNum.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListName", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ListNum", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rListNum.DisplayMember = "ListName";
		this.rListNum.Name = "rListNum";
		this.rListNum.NullText = "";
		this.rListNum.ValueMember = "ListNum";
		this.gcOwner.AppearanceHeader.Options.UseTextOptions = true;
		this.gcOwner.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcOwner.Caption = "Owner";
		this.gcOwner.ColumnEdit = this.rOwner;
		this.gcOwner.CurrencyDateField = null;
		this.gcOwner.CurrencyField = null;
		this.gcOwner.CurrencyFixed = null;
		this.gcOwner.CurrencyFixedRate = 0.0;
		this.gcOwner.CurrencyRateField = null;
		this.gcOwner.FieldName = "Owner";
		this.gcOwner.MinWidth = 23;
		this.gcOwner.Name = "gcOwner";
		this.gcOwner.PrimaryKey = false;
		this.gcOwner.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOwner.UnitField = null;
		this.gcOwner.Visible = true;
		this.gcOwner.VisibleIndex = 17;
		this.gcOwner.Width = 153;
		this.rOwner.AutoHeight = false;
		this.rOwner.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOwner.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rOwner.DisplayMember = "Name";
		this.rOwner.Name = "rOwner";
		this.rOwner.NullText = " ";
		this.rOwner.ShowFooter = false;
		this.rOwner.ShowHeader = false;
		this.rOwner.ValueMember = "ID";
		this.gcRenewal.AppearanceHeader.Options.UseTextOptions = true;
		this.gcRenewal.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcRenewal.Caption = "Renewal";
		this.gcRenewal.ColumnEdit = this.rRenewal;
		this.gcRenewal.CurrencyDateField = null;
		this.gcRenewal.CurrencyField = null;
		this.gcRenewal.CurrencyFixed = null;
		this.gcRenewal.CurrencyFixedRate = 0.0;
		this.gcRenewal.CurrencyRateField = null;
		this.gcRenewal.FieldName = "Renewal";
		this.gcRenewal.MinWidth = 23;
		this.gcRenewal.Name = "gcRenewal";
		this.gcRenewal.PrimaryKey = false;
		this.gcRenewal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRenewal.UnitField = null;
		this.gcRenewal.Visible = true;
		this.gcRenewal.VisibleIndex = 18;
		this.gcRenewal.Width = 96;
		this.rRenewal.AutoHeight = false;
		this.rRenewal.Name = "rRenewal";
		this.rRenewal.ValueChecked = "Y";
		this.rRenewal.ValueUnchecked = "N";
		this.gcSettlementProbability.AppearanceHeader.Options.UseTextOptions = true;
		this.gcSettlementProbability.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcSettlementProbability.Caption = "Settlement Probability %";
		this.gcSettlementProbability.ColumnEdit = this.rSettleProb;
		this.gcSettlementProbability.CurrencyDateField = null;
		this.gcSettlementProbability.CurrencyField = null;
		this.gcSettlementProbability.CurrencyFixed = null;
		this.gcSettlementProbability.CurrencyFixedRate = 0.0;
		this.gcSettlementProbability.CurrencyRateField = null;
		this.gcSettlementProbability.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcSettlementProbability.FieldName = "SettleProb";
		this.gcSettlementProbability.MinWidth = 23;
		this.gcSettlementProbability.Name = "gcSettlementProbability";
		this.gcSettlementProbability.PrimaryKey = false;
		this.gcSettlementProbability.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcSettlementProbability.UnitField = null;
		this.gcSettlementProbability.Visible = true;
		this.gcSettlementProbability.VisibleIndex = 21;
		this.gcSettlementProbability.Width = 154;
		this.rSettleProb.AutoHeight = false;
		this.rSettleProb.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rSettleProb.DisplayFormat.FormatString = "n2";
		this.rSettleProb.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rSettleProb.EditFormat.FormatString = "n2";
		this.rSettleProb.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rSettleProb.Mask.EditMask = "n2";
		this.rSettleProb.Name = "rSettleProb";
		this.gcRemindVal.AppearanceHeader.Options.UseTextOptions = true;
		this.gcRemindVal.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcRemindVal.Caption = "RemindVal";
		this.gcRemindVal.CurrencyDateField = null;
		this.gcRemindVal.CurrencyField = null;
		this.gcRemindVal.CurrencyFixed = null;
		this.gcRemindVal.CurrencyFixedRate = 0.0;
		this.gcRemindVal.CurrencyRateField = null;
		this.gcRemindVal.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcRemindVal.FieldName = "RemindVal";
		this.gcRemindVal.MinWidth = 23;
		this.gcRemindVal.Name = "gcRemindVal";
		this.gcRemindVal.PrimaryKey = false;
		this.gcRemindVal.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRemindVal.UnitField = null;
		this.gcRemindVal.Visible = true;
		this.gcRemindVal.VisibleIndex = 19;
		this.gcRemindVal.Width = 73;
		this.gcRemindUnit.AppearanceHeader.Options.UseTextOptions = true;
		this.gcRemindUnit.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcRemindUnit.Caption = "Remind Unit";
		this.gcRemindUnit.ColumnEdit = this.rRemindUnit;
		this.gcRemindUnit.CurrencyDateField = null;
		this.gcRemindUnit.CurrencyField = null;
		this.gcRemindUnit.CurrencyFixed = null;
		this.gcRemindUnit.CurrencyFixedRate = 0.0;
		this.gcRemindUnit.CurrencyRateField = null;
		this.gcRemindUnit.FieldName = "RemindUnit";
		this.gcRemindUnit.MinWidth = 23;
		this.gcRemindUnit.Name = "gcRemindUnit";
		this.gcRemindUnit.PrimaryKey = false;
		this.gcRemindUnit.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRemindUnit.UnitField = null;
		this.gcRemindUnit.Visible = true;
		this.gcRemindUnit.VisibleIndex = 20;
		this.gcRemindUnit.Width = 90;
		this.rRemindUnit.AutoHeight = false;
		this.rRemindUnit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rRemindUnit.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rRemindUnit.DisplayMember = "Name";
		this.rRemindUnit.Name = "rRemindUnit";
		this.rRemindUnit.NullText = "";
		this.rRemindUnit.ShowFooter = false;
		this.rRemindUnit.ShowHeader = false;
		this.rRemindUnit.ValueMember = "ID";
		this.gcRemarks.AppearanceHeader.Options.UseTextOptions = true;
		this.gcRemarks.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gcRemarks.Caption = "Remarks";
		this.gcRemarks.CurrencyDateField = null;
		this.gcRemarks.CurrencyField = null;
		this.gcRemarks.CurrencyFixed = null;
		this.gcRemarks.CurrencyFixedRate = 0.0;
		this.gcRemarks.CurrencyRateField = null;
		this.gcRemarks.FieldName = "Remarks";
		this.gcRemarks.MinWidth = 23;
		this.gcRemarks.Name = "gcRemarks";
		this.gcRemarks.PrimaryKey = false;
		this.gcRemarks.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRemarks.UnitField = null;
		this.gcRemarks.Visible = true;
		this.gcRemarks.VisibleIndex = 22;
		this.gcRemarks.Width = 64;
		this.gcAbsID.Caption = "AbsId";
		this.gcAbsID.CurrencyDateField = null;
		this.gcAbsID.CurrencyField = null;
		this.gcAbsID.CurrencyFixed = null;
		this.gcAbsID.CurrencyFixedRate = 0.0;
		this.gcAbsID.CurrencyRateField = null;
		this.gcAbsID.FieldName = "AbsID";
		this.gcAbsID.MinWidth = 23;
		this.gcAbsID.Name = "gcAbsID";
		this.gcAbsID.PrimaryKey = false;
		this.gcAbsID.UnitField = null;
		this.gcAbsID.Width = 87;
		this.gcProject.Caption = "Project";
		this.gcProject.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.gcProject.ColumnEdit = this.rProject;
		this.gcProject.CurrencyDateField = null;
		this.gcProject.CurrencyField = null;
		this.gcProject.CurrencyFixed = null;
		this.gcProject.CurrencyFixedRate = 0.0;
		this.gcProject.CurrencyRateField = null;
		this.gcProject.FieldName = "Project";
		this.gcProject.MinWidth = 23;
		this.gcProject.Name = "gcProject";
		this.gcProject.PrimaryKey = false;
		this.gcProject.UnitField = null;
		this.gcProject.Visible = true;
		this.gcProject.VisibleIndex = 23;
		this.gcProject.Width = 87;
		this.rProject.AutoHeight = false;
		this.rProject.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rProject.ContainerControl = this;
		this.rProject.DisplayMember = "PrjCode";
		this.rProject.DisplayMember2 = "";
		this.rProject.LoadDataSourceWhenEnter = false;
		this.rProject.Name = "rProject";
		this.rProject.SeparateCharacterForMulti = ';';
		this.rProject.SeparateString = ", ";
		this.rProject.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.rProject.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProjectCodes;
		this.rProject.UsingAutoCompleteBox = false;
		this.rProject.UsingDataSource = true;
		this.rProject.ValidateWhenNoDataSource = false;
		this.rProject.ValueMember = "PrjCode";
		this.gcBpCurr.Caption = "Currency";
		this.gcBpCurr.ColumnEdit = this.rBpCurrency;
		this.gcBpCurr.CurrencyDateField = null;
		this.gcBpCurr.CurrencyField = null;
		this.gcBpCurr.CurrencyFixed = null;
		this.gcBpCurr.CurrencyFixedRate = 0.0;
		this.gcBpCurr.CurrencyRateField = null;
		this.gcBpCurr.FieldName = "BPCurr";
		this.gcBpCurr.MinWidth = 23;
		this.gcBpCurr.Name = "gcBpCurr";
		this.gcBpCurr.PrimaryKey = false;
		this.gcBpCurr.UnitField = null;
		this.gcBpCurr.Visible = true;
		this.gcBpCurr.VisibleIndex = 24;
		this.gcBpCurr.Width = 87;
		this.rBpCurrency.AutoHeight = false;
		this.rBpCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBpCurrency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.rBpCurrency.DisplayMember = "Code";
		this.rBpCurrency.Name = "rBpCurrency";
		this.rBpCurrency.NullText = "";
		this.rBpCurrency.ValueMember = "Code";
		this.gcBpType.Caption = "BP Type";
		this.gcBpType.CurrencyDateField = null;
		this.gcBpType.CurrencyField = null;
		this.gcBpType.CurrencyFixed = null;
		this.gcBpType.CurrencyFixedRate = 0.0;
		this.gcBpType.CurrencyRateField = null;
		this.gcBpType.FieldName = "BpType";
		this.gcBpType.MinWidth = 23;
		this.gcBpType.Name = "gcBpType";
		this.gcBpType.OptionsColumn.ReadOnly = true;
		this.gcBpType.PrimaryKey = false;
		this.gcBpType.UnitField = null;
		this.gcBpType.Visible = true;
		this.gcBpType.VisibleIndex = 25;
		this.gcBpType.Width = 87;
		this.gcPaymentTerm.Caption = "Payment Term";
		this.gcPaymentTerm.ColumnEdit = this.luk_PaymentTerm;
		this.gcPaymentTerm.CurrencyDateField = null;
		this.gcPaymentTerm.CurrencyField = null;
		this.gcPaymentTerm.CurrencyFixed = null;
		this.gcPaymentTerm.CurrencyFixedRate = 0.0;
		this.gcPaymentTerm.CurrencyRateField = null;
		this.gcPaymentTerm.FieldName = "GroupNum";
		this.gcPaymentTerm.MinWidth = 23;
		this.gcPaymentTerm.Name = "gcPaymentTerm";
		this.gcPaymentTerm.PrimaryKey = false;
		this.gcPaymentTerm.UnitField = null;
		this.luk_PaymentTerm.AutoHeight = false;
		this.luk_PaymentTerm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PaymentTerm.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_PaymentTerm.DisplayMember = "Name";
		this.luk_PaymentTerm.Name = "luk_PaymentTerm";
		this.luk_PaymentTerm.NullText = "";
		this.luk_PaymentTerm.ShowHeader = false;
		this.luk_PaymentTerm.ValueMember = "Code";
		this.gcPayMethod.Caption = "Pay Method";
		this.gcPayMethod.ColumnEdit = this.luk_PayMethod;
		this.gcPayMethod.CurrencyDateField = null;
		this.gcPayMethod.CurrencyField = null;
		this.gcPayMethod.CurrencyFixed = null;
		this.gcPayMethod.CurrencyFixedRate = 0.0;
		this.gcPayMethod.CurrencyRateField = null;
		this.gcPayMethod.FieldName = "PayMethod";
		this.gcPayMethod.MinWidth = 23;
		this.gcPayMethod.Name = "gcPayMethod";
		this.gcPayMethod.PrimaryKey = false;
		this.gcPayMethod.UnitField = null;
		this.luk_PayMethod.AutoHeight = false;
		this.luk_PayMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PayMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_PayMethod.DisplayMember = "Name";
		this.luk_PayMethod.Name = "luk_PayMethod";
		this.luk_PayMethod.NullText = "";
		this.luk_PayMethod.ValueMember = "Code";
		this.rStatusD.AutoHeight = false;
		this.rStatusD.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.DropDown)
		});
		this.rStatusD.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rStatusD.DisplayMember = "Name";
		this.rStatusD.Name = "rStatusD";
		this.rStatusD.ShowFooter = false;
		this.rStatusD.ShowHeader = false;
		this.rStatusD.ValueMember = "ID";
		this.rCntctCode.AutoHeight = false;
		this.rCntctCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCntctCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CntctCode", "", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rCntctCode.DisplayMember = "Name";
		this.rCntctCode.DropDownRows = 1;
		this.rCntctCode.Name = "rCntctCode";
		this.rCntctCode.NullText = "";
		this.rCntctCode.ShowFooter = false;
		this.rCntctCode.ShowHeader = false;
		this.rCntctCode.ValueMember = "CntctCode";
		this.rStatusA.AutoHeight = false;
		this.rStatusA.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatusA.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rStatusA.DisplayMember = "Name";
		this.rStatusA.Name = "rStatusA";
		this.rStatusA.ShowFooter = false;
		this.rStatusA.ShowHeader = false;
		this.rStatusA.ValueMember = "ID";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.OK;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(3, 747);
		this.btn_OK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(115, 25);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btnSave_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(125, 747);
		this.btn_Cancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(115, 25);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabControl.IsSearchControl = false;
		this.tabControl.Location = new System.Drawing.Point(0, 0);
		this.tabControl.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tabControl.Name = "tabControl";
		this.tabControl.SelectedTabPage = this.tabPageDetails;
		this.tabControl.Size = new System.Drawing.Size(1316, 425);
		this.tabControl.TabIndex = 0;
		this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabPageDetails, this.tabAttachments });
		this.tabPageDetails.Controls.Add(this.grdDetails);
		this.tabPageDetails.EditControl = null;
		this.tabPageDetails.EditValue = "Details";
		this.tabPageDetails.FieldName = "tabPageDetails";
		this.tabPageDetails.IsSearchControl = false;
		this.tabPageDetails.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tabPageDetails.Name = "tabPageDetails";
		this.tabPageDetails.ReadOnly = false;
		this.tabPageDetails.SettingInVisible = false;
		this.tabPageDetails.SettingReadonly = false;
		this.tabPageDetails.Size = new System.Drawing.Size(1314, 395);
		this.tabPageDetails.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabPageDetails.Text = "Details";
		this.grdDetails.AllowPasteExcel = true;
		this.grdDetails.ClipboardData = null;
		this.grdDetails.ContextMenuStrip = this.contextLineMenu;
		this.grdDetails.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDetails.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdDetails.Location = new System.Drawing.Point(0, 0);
		this.grdDetails.MainView = this.gvDetails;
		this.grdDetails.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grdDetails.Name = "grdDetails";
		this.grdDetails.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[14]
		{
			this.rItemCode, this.rItemName, this.rPlanQty, this.rRetPortion, this.rWrrtyEnd, this.rItemGroup, this.rlineStatus, this.rUgpEntry, this.rep_Currency, this.rPlanAmount,
			this.rDiscount, this.rUomCode, this.repositoryItemLookUpEdit1, this.rTaxCode
		});
		this.grdDetails.Size = new System.Drawing.Size(1643, 494);
		this.grdDetails.TabIndex = 0;
		this.grdDetails.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDetails });
		this.grdDetails.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDetails_EditorKeyDown);
		this.contextLineMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextLineMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteRowToolStripMenuItem, this.listItemToolStripMenuItem });
		this.contextLineMenu.Name = "contextLineMenu";
		this.contextLineMenu.Size = new System.Drawing.Size(156, 52);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(155, 24);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.listItemToolStripMenuItem.Name = "listItemToolStripMenuItem";
		this.listItemToolStripMenuItem.Size = new System.Drawing.Size(155, 24);
		this.listItemToolStripMenuItem.Text = "List Item";
		this.listItemToolStripMenuItem.Click += new System.EventHandler(this.listItemToolStripMenuItem_Click);
		this.gvDetails.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDetails.ColumnKey = null;
		this.gvDetails.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[27]
		{
			this.gcItemCode, this.gcAgrLineNum, this.gcItemName, this.gcItemGroup, this.gcPlanQty, this.gcUnitPrice, this.gcCumQty, this.gcCumulativeAmountLC, this.gcCumulativeAmountFC, this.gcPlanAmtLC,
			this.gcPlanAmtFC, this.gcDiscount, this.gcOpenQuantity, this.gcOpenAmountLC, this.gcOpenAmountFC, this.gcFreeText, this.gcUomCode, this.gcUoMName, this.gcNumPerMsr, this.gcUoMGroup,
			this.gcRetPortion, this.gcWrrtyEnd, this.gcLineStatus, this.gvAgrNo, this.gcCurrency, this.gcLine_Project, this.gcTaxCode
		});
		this.gvDetails.DetailHeight = 431;
		this.gvDetails.DisableFormSetting = false;
		this.gvDetails.DisplayException = false;
		this.gvDetails.EditColumn = null;
		this.gvDetails.FieldCurrency = null;
		this.gvDetails.FieldRate = null;
		this.gvDetails.FormId = "oSalesBlanketAgreement";
		this.gvDetails.GridControl = this.grdDetails;
		this.gvDetails.IgnoreFilter = false;
		this.gvDetails.IndexEditRow = -1;
		this.gvDetails.IndicatorWidth = 47;
		this.gvDetails.IsApzTable = false;
		this.gvDetails.IsInsertNewRow = false;
		this.gvDetails.IsLoadingData = false;
		this.gvDetails.IsMappingContextAction = false;
		this.gvDetails.ItemId = "OAT1";
		this.gvDetails.LstColumnAllowBatch = null;
		this.gvDetails.LstColumnNotAllowBatch = null;
		this.gvDetails.LstColumnNotEditable = null;
		this.gvDetails.LstColumNotSetting = null;
		this.gvDetails.Name = "gvDetails";
		this.gvDetails.OptionsCustomization.AllowSort = false;
		this.gvDetails.OptionsDetail.EnableMasterViewMode = false;
		this.gvDetails.OptionsSelection.MultiSelect = true;
		this.gvDetails.OptionsView.ColumnAutoWidth = false;
		this.gvDetails.OptionsView.ShowFooter = true;
		this.gvDetails.OptionsView.ShowGroupPanel = false;
		this.gvDetails.UseBatchMod = false;
		this.gvDetails.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDetails_RowCellStyle);
		this.gvDetails.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDetails_ShowingEditor);
		this.gvDetails.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDetails_CellValueChanged);
		this.gvDetails.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gvDetails_CustomColumnDisplayText);
		this.gvDetails.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gvDetails_KeyDown);
		this.gvDetails.DoubleClick += new System.EventHandler(this.gvDetails_DoubleClick);
		this.gvDetails.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvDetails_ValidatingEditor);
		this.gcItemCode.Caption = "Item No.";
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemCode.MinWidth = 23;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 128;
		this.rItemCode.AutoHeight = false;
		this.rItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.ContainerControl = this;
		this.rItemCode.DisplayMember = "";
		this.rItemCode.DisplayMember2 = "";
		this.rItemCode.LoadDataSourceWhenEnter = false;
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.SeparateCharacterForMulti = ';';
		this.rItemCode.SeparateString = ", ";
		this.rItemCode.UsingAutoCompleteBox = false;
		this.rItemCode.UsingDataSource = true;
		this.rItemCode.ValidateWhenNoDataSource = false;
		this.rItemCode.ValueMember = "";
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.gcAgrLineNum.Caption = "AgrLineNum";
		this.gcAgrLineNum.CurrencyDateField = null;
		this.gcAgrLineNum.CurrencyField = null;
		this.gcAgrLineNum.CurrencyFixed = null;
		this.gcAgrLineNum.CurrencyFixedRate = 0.0;
		this.gcAgrLineNum.CurrencyRateField = null;
		this.gcAgrLineNum.FieldName = "AgrLineNum";
		this.gcAgrLineNum.MinWidth = 23;
		this.gcAgrLineNum.Name = "gcAgrLineNum";
		this.gcAgrLineNum.PrimaryKey = false;
		this.gcAgrLineNum.UnitField = null;
		this.gcAgrLineNum.Width = 87;
		this.gcItemName.Caption = "Item Description";
		this.gcItemName.ColumnEdit = this.rItemName;
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemName.MinWidth = 23;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcItemName.Width = 136;
		this.rItemName.AutoHeight = false;
		this.rItemName.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemName.ContainerControl = this;
		this.rItemName.DisplayMember = "";
		this.rItemName.DisplayMember2 = "";
		this.rItemName.LoadDataSourceWhenEnter = false;
		this.rItemName.Name = "rItemName";
		this.rItemName.SeparateCharacterForMulti = ';';
		this.rItemName.SeparateString = ", ";
		this.rItemName.UsingAutoCompleteBox = false;
		this.rItemName.UsingDataSource = true;
		this.rItemName.ValidateWhenNoDataSource = false;
		this.rItemName.ValueMember = "";
		this.gcItemGroup.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcItemGroup.AppearanceCell.Options.UseBackColor = true;
		this.gcItemGroup.Caption = "Item Group";
		this.gcItemGroup.ColumnEdit = this.rItemGroup;
		this.gcItemGroup.CurrencyDateField = null;
		this.gcItemGroup.CurrencyField = null;
		this.gcItemGroup.CurrencyFixed = null;
		this.gcItemGroup.CurrencyFixedRate = 0.0;
		this.gcItemGroup.CurrencyRateField = null;
		this.gcItemGroup.FieldName = "ItemGroup";
		this.gcItemGroup.MinWidth = 23;
		this.gcItemGroup.Name = "gcItemGroup";
		this.gcItemGroup.OptionsColumn.ReadOnly = true;
		this.gcItemGroup.PrimaryKey = false;
		this.gcItemGroup.UnitField = null;
		this.gcItemGroup.Visible = true;
		this.gcItemGroup.VisibleIndex = 2;
		this.gcItemGroup.Width = 104;
		this.rItemGroup.AutoHeight = false;
		this.rItemGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rItemGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpNam", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ItmsGrpCod", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rItemGroup.DisplayMember = "ItmsGrpNam";
		this.rItemGroup.Name = "rItemGroup";
		this.rItemGroup.NullText = "";
		this.rItemGroup.ValueMember = "ItmsGrpCod";
		this.gcPlanQty.Caption = "Planned Quantity";
		this.gcPlanQty.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcPlanQty.CurrencyDateField = null;
		this.gcPlanQty.CurrencyField = null;
		this.gcPlanQty.CurrencyFixed = null;
		this.gcPlanQty.CurrencyFixedRate = 0.0;
		this.gcPlanQty.CurrencyRateField = null;
		this.gcPlanQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcPlanQty.FieldName = "PlanQty";
		this.gcPlanQty.MinWidth = 23;
		this.gcPlanQty.Name = "gcPlanQty";
		this.gcPlanQty.PrimaryKey = false;
		this.gcPlanQty.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcPlanQty.UnitField = null;
		this.gcPlanQty.Visible = true;
		this.gcPlanQty.VisibleIndex = 3;
		this.gcPlanQty.Width = 132;
		this.gcUnitPrice.Caption = "Unit Price";
		this.gcUnitPrice.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcUnitPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcUnitPrice.CurrencyDateField = null;
		this.gcUnitPrice.CurrencyField = "Currency";
		this.gcUnitPrice.CurrencyFixed = null;
		this.gcUnitPrice.CurrencyFixedRate = 0.0;
		this.gcUnitPrice.CurrencyRateField = null;
		this.gcUnitPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcUnitPrice.FieldName = "UnitPrice";
		this.gcUnitPrice.MinWidth = 23;
		this.gcUnitPrice.Name = "gcUnitPrice";
		this.gcUnitPrice.PrimaryKey = false;
		this.gcUnitPrice.UnitField = null;
		this.gcUnitPrice.Visible = true;
		this.gcUnitPrice.VisibleIndex = 4;
		this.gcUnitPrice.Width = 107;
		this.gcCumQty.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcCumQty.AppearanceCell.Options.UseBackColor = true;
		this.gcCumQty.Caption = "Cumulative Quantity";
		this.gcCumQty.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gcCumQty.CurrencyDateField = null;
		this.gcCumQty.CurrencyField = null;
		this.gcCumQty.CurrencyFixed = null;
		this.gcCumQty.CurrencyFixedRate = 0.0;
		this.gcCumQty.CurrencyRateField = null;
		this.gcCumQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcCumQty.FieldName = "CumQty";
		this.gcCumQty.MinWidth = 23;
		this.gcCumQty.Name = "gcCumQty";
		this.gcCumQty.OptionsColumn.ReadOnly = true;
		this.gcCumQty.PrimaryKey = false;
		this.gcCumQty.UnitField = null;
		this.gcCumQty.Visible = true;
		this.gcCumQty.VisibleIndex = 5;
		this.gcCumQty.Width = 140;
		this.gcCumulativeAmountLC.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcCumulativeAmountLC.AppearanceCell.Options.UseBackColor = true;
		this.gcCumulativeAmountLC.Caption = "Cumulative Amount (LC)";
		this.gcCumulativeAmountLC.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gcCumulativeAmountLC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcCumulativeAmountLC.CurrencyDateField = null;
		this.gcCumulativeAmountLC.CurrencyField = null;
		this.gcCumulativeAmountLC.CurrencyFixed = null;
		this.gcCumulativeAmountLC.CurrencyFixedRate = 0.0;
		this.gcCumulativeAmountLC.CurrencyRateField = null;
		this.gcCumulativeAmountLC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcCumulativeAmountLC.FieldName = "CumAmntLC";
		this.gcCumulativeAmountLC.MinWidth = 23;
		this.gcCumulativeAmountLC.Name = "gcCumulativeAmountLC";
		this.gcCumulativeAmountLC.OptionsColumn.ReadOnly = true;
		this.gcCumulativeAmountLC.PrimaryKey = false;
		this.gcCumulativeAmountLC.UnitField = null;
		this.gcCumulativeAmountLC.Visible = true;
		this.gcCumulativeAmountLC.VisibleIndex = 6;
		this.gcCumulativeAmountLC.Width = 160;
		this.gcCumulativeAmountFC.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcCumulativeAmountFC.AppearanceCell.Options.UseBackColor = true;
		this.gcCumulativeAmountFC.Caption = "Cumulative Amount (FC)";
		this.gcCumulativeAmountFC.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gcCumulativeAmountFC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcCumulativeAmountFC.CurrencyDateField = null;
		this.gcCumulativeAmountFC.CurrencyField = "Currency";
		this.gcCumulativeAmountFC.CurrencyFixed = null;
		this.gcCumulativeAmountFC.CurrencyFixedRate = 0.0;
		this.gcCumulativeAmountFC.CurrencyRateField = null;
		this.gcCumulativeAmountFC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcCumulativeAmountFC.FieldName = "CumAmntFC";
		this.gcCumulativeAmountFC.MinWidth = 23;
		this.gcCumulativeAmountFC.Name = "gcCumulativeAmountFC";
		this.gcCumulativeAmountFC.OptionsColumn.ReadOnly = true;
		this.gcCumulativeAmountFC.PrimaryKey = false;
		this.gcCumulativeAmountFC.UnitField = null;
		this.gcCumulativeAmountFC.Visible = true;
		this.gcCumulativeAmountFC.VisibleIndex = 7;
		this.gcCumulativeAmountFC.Width = 155;
		this.gcPlanAmtLC.Caption = "Planned Amount (LC)";
		this.gcPlanAmtLC.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gcPlanAmtLC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcPlanAmtLC.CurrencyDateField = null;
		this.gcPlanAmtLC.CurrencyField = null;
		this.gcPlanAmtLC.CurrencyFixed = null;
		this.gcPlanAmtLC.CurrencyFixedRate = 0.0;
		this.gcPlanAmtLC.CurrencyRateField = null;
		this.gcPlanAmtLC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcPlanAmtLC.FieldName = "PlanAmtLC";
		this.gcPlanAmtLC.MinWidth = 23;
		this.gcPlanAmtLC.Name = "gcPlanAmtLC";
		this.gcPlanAmtLC.PrimaryKey = false;
		this.gcPlanAmtLC.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcPlanAmtLC.UnitField = null;
		this.gcPlanAmtLC.Visible = true;
		this.gcPlanAmtLC.VisibleIndex = 8;
		this.gcPlanAmtLC.Width = 140;
		this.gcPlanAmtFC.Caption = "Planed Amount (FC)";
		this.gcPlanAmtFC.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gcPlanAmtFC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcPlanAmtFC.CurrencyDateField = null;
		this.gcPlanAmtFC.CurrencyField = "Currency";
		this.gcPlanAmtFC.CurrencyFixed = null;
		this.gcPlanAmtFC.CurrencyFixedRate = 0.0;
		this.gcPlanAmtFC.CurrencyRateField = null;
		this.gcPlanAmtFC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcPlanAmtFC.FieldName = "PlanAmtFC";
		this.gcPlanAmtFC.MinWidth = 23;
		this.gcPlanAmtFC.Name = "gcPlanAmtFC";
		this.gcPlanAmtFC.OptionsColumn.ReadOnly = true;
		this.gcPlanAmtFC.PrimaryKey = false;
		this.gcPlanAmtFC.UnitField = null;
		this.gcPlanAmtFC.Visible = true;
		this.gcPlanAmtFC.VisibleIndex = 9;
		this.gcPlanAmtFC.Width = 134;
		this.gcDiscount.Caption = "Line Discount";
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDiscount.FieldName = "Discount";
		this.gcDiscount.MinWidth = 23;
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 10;
		this.gcDiscount.Width = 87;
		this.gcOpenQuantity.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcOpenQuantity.AppearanceCell.Options.UseBackColor = true;
		this.gcOpenQuantity.Caption = "Open Quantity";
		this.gcOpenQuantity.ColumnEdit = this.repositoryItemCalcEditEx8;
		this.gcOpenQuantity.CurrencyDateField = null;
		this.gcOpenQuantity.CurrencyField = null;
		this.gcOpenQuantity.CurrencyFixed = null;
		this.gcOpenQuantity.CurrencyFixedRate = 0.0;
		this.gcOpenQuantity.CurrencyRateField = null;
		this.gcOpenQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcOpenQuantity.FieldName = "gcOpenQuantity";
		this.gcOpenQuantity.MinWidth = 23;
		this.gcOpenQuantity.Name = "gcOpenQuantity";
		this.gcOpenQuantity.OptionsColumn.ReadOnly = true;
		this.gcOpenQuantity.PrimaryKey = false;
		this.gcOpenQuantity.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcOpenQuantity.UnboundExpression = "[PlanQty] - IsNull([CumQty], 0)";
		this.gcOpenQuantity.UnboundType = DevExpress.Data.UnboundColumnType.Decimal;
		this.gcOpenQuantity.UnitField = null;
		this.gcOpenQuantity.Visible = true;
		this.gcOpenQuantity.VisibleIndex = 11;
		this.gcOpenQuantity.Width = 110;
		this.gcOpenAmountLC.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcOpenAmountLC.AppearanceCell.Options.UseBackColor = true;
		this.gcOpenAmountLC.Caption = "Open Amount (LC)";
		this.gcOpenAmountLC.ColumnEdit = this.repositoryItemCalcEditEx9;
		this.gcOpenAmountLC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcOpenAmountLC.CurrencyDateField = null;
		this.gcOpenAmountLC.CurrencyField = null;
		this.gcOpenAmountLC.CurrencyFixed = null;
		this.gcOpenAmountLC.CurrencyFixedRate = 0.0;
		this.gcOpenAmountLC.CurrencyRateField = null;
		this.gcOpenAmountLC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcOpenAmountLC.FieldName = "OpenAmntLC";
		this.gcOpenAmountLC.MinWidth = 23;
		this.gcOpenAmountLC.Name = "gcOpenAmountLC";
		this.gcOpenAmountLC.OptionsColumn.ReadOnly = true;
		this.gcOpenAmountLC.PrimaryKey = false;
		this.gcOpenAmountLC.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum)
		});
		this.gcOpenAmountLC.UnitField = null;
		this.gcOpenAmountLC.Visible = true;
		this.gcOpenAmountLC.VisibleIndex = 12;
		this.gcOpenAmountLC.Width = 124;
		this.gcOpenAmountFC.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcOpenAmountFC.AppearanceCell.Options.UseBackColor = true;
		this.gcOpenAmountFC.Caption = "Open Amount (FC)";
		this.gcOpenAmountFC.ColumnEdit = this.repositoryItemCalcEditEx10;
		this.gcOpenAmountFC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcOpenAmountFC.CurrencyDateField = null;
		this.gcOpenAmountFC.CurrencyField = "Currency";
		this.gcOpenAmountFC.CurrencyFixed = null;
		this.gcOpenAmountFC.CurrencyFixedRate = 0.0;
		this.gcOpenAmountFC.CurrencyRateField = null;
		this.gcOpenAmountFC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gcOpenAmountFC.FieldName = "OpenAmntFC";
		this.gcOpenAmountFC.MinWidth = 23;
		this.gcOpenAmountFC.Name = "gcOpenAmountFC";
		this.gcOpenAmountFC.OptionsColumn.AllowEdit = false;
		this.gcOpenAmountFC.OptionsColumn.ReadOnly = true;
		this.gcOpenAmountFC.PrimaryKey = false;
		this.gcOpenAmountFC.UnitField = null;
		this.gcOpenAmountFC.Width = 119;
		this.gcFreeText.Caption = "Free Text";
		this.gcFreeText.CurrencyDateField = null;
		this.gcFreeText.CurrencyField = null;
		this.gcFreeText.CurrencyFixed = null;
		this.gcFreeText.CurrencyFixedRate = 0.0;
		this.gcFreeText.CurrencyRateField = null;
		this.gcFreeText.FieldName = "FreeTxt";
		this.gcFreeText.MinWidth = 23;
		this.gcFreeText.Name = "gcFreeText";
		this.gcFreeText.PrimaryKey = false;
		this.gcFreeText.UnitField = null;
		this.gcFreeText.Visible = true;
		this.gcFreeText.VisibleIndex = 13;
		this.gcFreeText.Width = 80;
		this.gcUomCode.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcUomCode.AppearanceCell.Options.UseBackColor = true;
		this.gcUomCode.Caption = "UoM Code";
		this.gcUomCode.ColumnEdit = this.rUomCode;
		this.gcUomCode.CurrencyDateField = null;
		this.gcUomCode.CurrencyField = null;
		this.gcUomCode.CurrencyFixed = null;
		this.gcUomCode.CurrencyFixedRate = 0.0;
		this.gcUomCode.CurrencyRateField = null;
		this.gcUomCode.FieldName = "UomCode";
		this.gcUomCode.MinWidth = 23;
		this.gcUomCode.Name = "gcUomCode";
		this.gcUomCode.PrimaryKey = false;
		this.gcUomCode.UnitField = null;
		this.gcUomCode.Visible = true;
		this.gcUomCode.VisibleIndex = 14;
		this.rUomCode.AutoHeight = false;
		this.rUomCode.AutoLoadDataSource = true;
		this.rUomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rUomCode.ContainerControl = this;
		this.rUomCode.DisplayMember = "";
		this.rUomCode.DisplayMember2 = "";
		this.rUomCode.LoadDataSourceWhenEnter = false;
		this.rUomCode.Name = "rUomCode";
		this.rUomCode.SeparateCharacterForMulti = ';';
		this.rUomCode.SeparateString = ", ";
		this.rUomCode.UsingAutoCompleteBox = false;
		this.rUomCode.UsingDataSource = true;
		this.rUomCode.ValidateWhenNoDataSource = false;
		this.rUomCode.ValueMember = "";
		this.rUomCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rUomCode_ButtonClick);
		this.gcUoMName.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcUoMName.AppearanceCell.Options.UseBackColor = true;
		this.gcUoMName.Caption = "UoM Name";
		this.gcUoMName.CurrencyDateField = null;
		this.gcUoMName.CurrencyField = null;
		this.gcUoMName.CurrencyFixed = null;
		this.gcUoMName.CurrencyFixedRate = 0.0;
		this.gcUoMName.CurrencyRateField = null;
		this.gcUoMName.FieldName = "InvntryUom";
		this.gcUoMName.MinWidth = 23;
		this.gcUoMName.Name = "gcUoMName";
		this.gcUoMName.OptionsColumn.ReadOnly = true;
		this.gcUoMName.PrimaryKey = false;
		this.gcUoMName.UnitField = null;
		this.gcUoMName.Visible = true;
		this.gcUoMName.VisibleIndex = 15;
		this.gcUoMName.Width = 77;
		this.gcNumPerMsr.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcNumPerMsr.AppearanceCell.Options.UseBackColor = true;
		this.gcNumPerMsr.Caption = "Items per Unit";
		this.gcNumPerMsr.ColumnEdit = this.repositoryItemCalcEditEx11;
		this.gcNumPerMsr.CurrencyDateField = null;
		this.gcNumPerMsr.CurrencyField = null;
		this.gcNumPerMsr.CurrencyFixed = null;
		this.gcNumPerMsr.CurrencyFixedRate = 0.0;
		this.gcNumPerMsr.CurrencyRateField = null;
		this.gcNumPerMsr.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcNumPerMsr.FieldName = "NumPerMsr";
		this.gcNumPerMsr.MinWidth = 23;
		this.gcNumPerMsr.Name = "gcNumPerMsr";
		this.gcNumPerMsr.OptionsColumn.ReadOnly = true;
		this.gcNumPerMsr.PrimaryKey = false;
		this.gcNumPerMsr.UnitField = null;
		this.gcNumPerMsr.Visible = true;
		this.gcNumPerMsr.VisibleIndex = 16;
		this.gcNumPerMsr.Width = 104;
		this.gcUoMGroup.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcUoMGroup.AppearanceCell.Options.UseBackColor = true;
		this.gcUoMGroup.Caption = "UoM Group";
		this.gcUoMGroup.ColumnEdit = this.rUgpEntry;
		this.gcUoMGroup.CurrencyDateField = null;
		this.gcUoMGroup.CurrencyField = null;
		this.gcUoMGroup.CurrencyFixed = null;
		this.gcUoMGroup.CurrencyFixedRate = 0.0;
		this.gcUoMGroup.CurrencyRateField = null;
		this.gcUoMGroup.FieldName = "UgpEntry";
		this.gcUoMGroup.MinWidth = 23;
		this.gcUoMGroup.Name = "gcUoMGroup";
		this.gcUoMGroup.OptionsColumn.ReadOnly = true;
		this.gcUoMGroup.PrimaryKey = false;
		this.gcUoMGroup.UnitField = null;
		this.gcUoMGroup.Visible = true;
		this.gcUoMGroup.VisibleIndex = 17;
		this.gcUoMGroup.Width = 85;
		this.rUgpEntry.AutoHeight = false;
		this.rUgpEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUgpEntry.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2")
		});
		this.rUgpEntry.DisplayMember = "Name";
		this.rUgpEntry.Name = "rUgpEntry";
		this.rUgpEntry.NullText = "";
		this.rUgpEntry.ShowHeader = false;
		this.rUgpEntry.ValueMember = "Code";
		this.gcRetPortion.Caption = "Portion Of Return %";
		this.gcRetPortion.ColumnEdit = this.repositoryItemCalcEditEx12;
		this.gcRetPortion.CurrencyDateField = null;
		this.gcRetPortion.CurrencyField = null;
		this.gcRetPortion.CurrencyFixed = null;
		this.gcRetPortion.CurrencyFixedRate = 0.0;
		this.gcRetPortion.CurrencyRateField = null;
		this.gcRetPortion.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcRetPortion.FieldName = "RetPortion";
		this.gcRetPortion.MinWidth = 23;
		this.gcRetPortion.Name = "gcRetPortion";
		this.gcRetPortion.PrimaryKey = false;
		this.gcRetPortion.UnitField = null;
		this.gcRetPortion.Visible = true;
		this.gcRetPortion.VisibleIndex = 18;
		this.gcRetPortion.Width = 128;
		this.gcWrrtyEnd.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcWrrtyEnd.AppearanceCell.Options.UseBackColor = true;
		this.gcWrrtyEnd.Caption = "End Of Warranty";
		this.gcWrrtyEnd.CurrencyDateField = null;
		this.gcWrrtyEnd.CurrencyField = null;
		this.gcWrrtyEnd.CurrencyFixed = null;
		this.gcWrrtyEnd.CurrencyFixedRate = 0.0;
		this.gcWrrtyEnd.CurrencyRateField = null;
		this.gcWrrtyEnd.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcWrrtyEnd.FieldName = "WrrtyEnd";
		this.gcWrrtyEnd.MinWidth = 23;
		this.gcWrrtyEnd.Name = "gcWrrtyEnd";
		this.gcWrrtyEnd.PrimaryKey = false;
		this.gcWrrtyEnd.UnitField = null;
		this.gcWrrtyEnd.Visible = true;
		this.gcWrrtyEnd.VisibleIndex = 19;
		this.gcWrrtyEnd.Width = 108;
		this.gcLineStatus.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gcLineStatus.AppearanceCell.Options.UseBackColor = true;
		this.gcLineStatus.Caption = "Item Row Status";
		this.gcLineStatus.ColumnEdit = this.rlineStatus;
		this.gcLineStatus.CurrencyDateField = null;
		this.gcLineStatus.CurrencyField = null;
		this.gcLineStatus.CurrencyFixed = null;
		this.gcLineStatus.CurrencyFixedRate = 0.0;
		this.gcLineStatus.CurrencyRateField = null;
		this.gcLineStatus.FieldName = "LineStatus";
		this.gcLineStatus.MinWidth = 23;
		this.gcLineStatus.Name = "gcLineStatus";
		this.gcLineStatus.OptionsColumn.ReadOnly = true;
		this.gcLineStatus.PrimaryKey = false;
		this.gcLineStatus.UnitField = null;
		this.gcLineStatus.Visible = true;
		this.gcLineStatus.VisibleIndex = 20;
		this.gcLineStatus.Width = 107;
		this.rlineStatus.AutoHeight = false;
		this.rlineStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlineStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rlineStatus.DisplayMember = "Name";
		this.rlineStatus.Name = "rlineStatus";
		this.rlineStatus.NullText = "";
		this.rlineStatus.ShowHeader = false;
		this.rlineStatus.ValueMember = "ID";
		this.gvAgrNo.Caption = "AgrNo";
		this.gvAgrNo.CurrencyDateField = null;
		this.gvAgrNo.CurrencyField = null;
		this.gvAgrNo.CurrencyFixed = null;
		this.gvAgrNo.CurrencyFixedRate = 0.0;
		this.gvAgrNo.CurrencyRateField = null;
		this.gvAgrNo.FieldName = "AgrNo";
		this.gvAgrNo.MinWidth = 23;
		this.gvAgrNo.Name = "gvAgrNo";
		this.gvAgrNo.OptionsColumn.ReadOnly = true;
		this.gvAgrNo.PrimaryKey = false;
		this.gvAgrNo.UnitField = null;
		this.gvAgrNo.Width = 87;
		this.gcCurrency.Caption = "Currency";
		this.gcCurrency.ColumnEdit = this.rep_Currency;
		this.gcCurrency.CurrencyDateField = null;
		this.gcCurrency.CurrencyField = null;
		this.gcCurrency.CurrencyFixed = null;
		this.gcCurrency.CurrencyFixedRate = 0.0;
		this.gcCurrency.CurrencyRateField = null;
		this.gcCurrency.FieldName = "Currency";
		this.gcCurrency.MinWidth = 23;
		this.gcCurrency.Name = "gcCurrency";
		this.gcCurrency.OptionsColumn.ReadOnly = true;
		this.gcCurrency.PrimaryKey = false;
		this.gcCurrency.UnitField = null;
		this.gcCurrency.Visible = true;
		this.gcCurrency.VisibleIndex = 21;
		this.gcCurrency.Width = 87;
		this.rep_Currency.AutoHeight = false;
		this.rep_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Currency.DisplayMember = "Code";
		this.rep_Currency.Name = "rep_Currency";
		this.rep_Currency.NullText = "";
		this.rep_Currency.ValueMember = "Code";
		this.gcLine_Project.Caption = "Project";
		this.gcLine_Project.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Project;
		this.gcLine_Project.ColumnEdit = this.rProject;
		this.gcLine_Project.CurrencyDateField = null;
		this.gcLine_Project.CurrencyField = null;
		this.gcLine_Project.CurrencyFixed = null;
		this.gcLine_Project.CurrencyFixedRate = 0.0;
		this.gcLine_Project.CurrencyRateField = null;
		this.gcLine_Project.FieldName = "Project";
		this.gcLine_Project.MinWidth = 23;
		this.gcLine_Project.Name = "gcLine_Project";
		this.gcLine_Project.PrimaryKey = false;
		this.gcLine_Project.UnitField = null;
		this.gcLine_Project.Visible = true;
		this.gcLine_Project.VisibleIndex = 22;
		this.gcLine_Project.Width = 87;
		this.gcTaxCode.Caption = "Tax Code";
		this.gcTaxCode.ColumnEdit = this.rTaxCode;
		this.gcTaxCode.CurrencyDateField = null;
		this.gcTaxCode.CurrencyField = null;
		this.gcTaxCode.CurrencyFixed = null;
		this.gcTaxCode.CurrencyFixedRate = 0.0;
		this.gcTaxCode.CurrencyRateField = null;
		this.gcTaxCode.FieldName = "TaxCode";
		this.gcTaxCode.MinWidth = 24;
		this.gcTaxCode.Name = "gcTaxCode";
		this.gcTaxCode.PrimaryKey = false;
		this.gcTaxCode.UnitField = null;
		this.gcTaxCode.Width = 87;
		this.rTaxCode.AutoHeight = false;
		this.rTaxCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rTaxCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Rate", "Rate")
		});
		this.rTaxCode.DisplayMember = "Code";
		this.rTaxCode.Name = "rTaxCode";
		this.rTaxCode.NullText = "";
		this.rTaxCode.ValueMember = "Code";
		this.rPlanQty.AutoHeight = false;
		this.rPlanQty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rPlanQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rPlanQty.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rPlanQty.Name = "rPlanQty";
		this.rRetPortion.AutoHeight = false;
		this.rRetPortion.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.rRetPortion.Name = "rRetPortion";
		this.rWrrtyEnd.AutoHeight = false;
		this.rWrrtyEnd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.rWrrtyEnd.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rWrrtyEnd.Name = "rWrrtyEnd";
		this.rPlanAmount.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.rPlanAmount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rPlanAmount.Name = "rPlanAmount";
		this.rDiscount.AutoHeight = false;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.rDiscount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDiscount.Name = "rDiscount";
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.tabAttachments.Controls.Add(this.ucAttachment1);
		this.tabAttachments.EditControl = null;
		this.tabAttachments.EditValue = "Attachments";
		this.tabAttachments.FieldName = "tabAttachments";
		this.tabAttachments.IsSearchControl = false;
		this.tabAttachments.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.tabAttachments.Name = "tabAttachments";
		this.tabAttachments.ReadOnly = false;
		this.tabAttachments.SettingInVisible = false;
		this.tabAttachments.SettingReadonly = false;
		this.tabAttachments.Size = new System.Drawing.Size(1314, 392);
		this.tabAttachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabAttachments.Text = "Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oSalesBlanketAgreement";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(1643, 490);
		this.ucAttachment1.TabIndex = 1;
		this.ucAttachment1.ButtonBrowse_Click += new System.EventHandler(this.ucAttachment1_ButtonBrowse_Click);
		this.ucAttachment1.ButtonDelete_Click += new System.EventHandler(this.ucAttachment1_ButtonBrowse_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 62);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tabControl);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1316, 679);
		this.splitContainerControl1.SplitterPosition = 242;
		this.splitContainerControl1.TabIndex = 19;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.drlCopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drlCopyTo.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.drlCopyTo.Appearance.Options.UseBackColor = true;
		this.drlCopyTo.EditControl = null;
		this.drlCopyTo.EditValue = "Copy to";
		this.drlCopyTo.Enabled = false;
		this.drlCopyTo.FieldName = null;
		this.drlCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drlCopyTo.ImageOptions.Image");
		this.drlCopyTo.IsSearchControl = false;
		this.drlCopyTo.ItemId = null;
		this.drlCopyTo.Location = new System.Drawing.Point(1184, 746);
		this.drlCopyTo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.drlCopyTo.Name = "drlCopyTo";
		this.drlCopyTo.ReadOnly = false;
		this.drlCopyTo.SettingInVisible = false;
		this.drlCopyTo.SettingReadonly = false;
		this.drlCopyTo.Size = new System.Drawing.Size(134, 25);
		this.drlCopyTo.TabIndex = 26;
		this.drlCopyTo.Text = "Copy to";
		this.txtBusinessPartner.EditControl = null;
		this.txtBusinessPartner.FieldName = null;
		this.txtBusinessPartner.IsSearchControl = false;
		this.txtBusinessPartner.Location = new System.Drawing.Point(370, 4);
		this.txtBusinessPartner.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtBusinessPartner.Name = "txtBusinessPartner";
		this.txtBusinessPartner.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtBusinessPartner.Properties.Appearance.Options.UseFont = true;
		this.txtBusinessPartner.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = Apzon.Forms.Properties.Resources.right;
		this.txtBusinessPartner.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBusinessPartner.Properties.ContainerControl = null;
		this.txtBusinessPartner.Properties.DisplayMember = "";
		this.txtBusinessPartner.Properties.DisplayMember2 = "";
		this.txtBusinessPartner.Properties.LoadDataSourceWhenEnter = false;
		this.txtBusinessPartner.Properties.SeparateCharacterForMulti = ';';
		this.txtBusinessPartner.Properties.SeparateString = ", ";
		this.txtBusinessPartner.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Customer;
		this.txtBusinessPartner.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.txtBusinessPartner.Properties.UsingAutoCompleteBox = false;
		this.txtBusinessPartner.Properties.UsingDataSource = true;
		this.txtBusinessPartner.Properties.ValidateWhenNoDataSource = false;
		this.txtBusinessPartner.Properties.ValueMember = "CardCode";
		this.txtBusinessPartner.SettingInVisible = false;
		this.txtBusinessPartner.SettingReadonly = false;
		this.txtBusinessPartner.Size = new System.Drawing.Size(163, 22);
		this.txtBusinessPartner.TabIndex = 68;
		this.txtBusinessPartner.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnBusinessPartner_ButtonClick);
		this.lb_DocStatus.EditControl = null;
		this.lb_DocStatus.EditValue = "Document Status";
		this.lb_DocStatus.FieldName = null;
		this.lb_DocStatus.HideWhenEmpty = false;
		this.lb_DocStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocStatus.Location = new System.Drawing.Point(540, 33);
		this.lb_DocStatus.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_DocStatus.Name = "lb_DocStatus";
		this.lb_DocStatus.ReadOnly = false;
		this.lb_DocStatus.SettingInVisible = false;
		this.lb_DocStatus.SettingReadonly = false;
		this.lb_DocStatus.ShowUnderLine = true;
		this.lb_DocStatus.Size = new System.Drawing.Size(114, 25);
		this.lb_DocStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocStatus.TabIndex = 76;
		this.lb_DocStatus.Utext = "Document Status";
		this.lb_BusinessPartner.EditControl = null;
		this.lb_BusinessPartner.EditValue = "Business Partner";
		this.lb_BusinessPartner.FieldName = null;
		this.lb_BusinessPartner.HideWhenEmpty = false;
		this.lb_BusinessPartner.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BusinessPartner.Location = new System.Drawing.Point(267, 4);
		this.lb_BusinessPartner.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_BusinessPartner.Name = "lb_BusinessPartner";
		this.lb_BusinessPartner.ReadOnly = false;
		this.lb_BusinessPartner.SettingInVisible = false;
		this.lb_BusinessPartner.SettingReadonly = false;
		this.lb_BusinessPartner.ShowUnderLine = true;
		this.lb_BusinessPartner.Size = new System.Drawing.Size(103, 25);
		this.lb_BusinessPartner.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BusinessPartner.TabIndex = 75;
		this.lb_BusinessPartner.Utext = "Business Partner";
		this.lb_DocNumber.EditControl = null;
		this.lb_DocNumber.EditValue = "Doc. Number";
		this.lb_DocNumber.FieldName = null;
		this.lb_DocNumber.HideWhenEmpty = false;
		this.lb_DocNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocNumber.Location = new System.Drawing.Point(267, 34);
		this.lb_DocNumber.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_DocNumber.Name = "lb_DocNumber";
		this.lb_DocNumber.ReadOnly = false;
		this.lb_DocNumber.SettingInVisible = false;
		this.lb_DocNumber.SettingReadonly = false;
		this.lb_DocNumber.ShowUnderLine = true;
		this.lb_DocNumber.Size = new System.Drawing.Size(103, 25);
		this.lb_DocNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocNumber.TabIndex = 74;
		this.lb_DocNumber.Utext = "Doc. Number";
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(3, 34);
		this.lb_ToDate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(93, 25);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 73;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(3, 4);
		this.lb_FromDate.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(93, 25);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 72;
		this.lb_FromDate.Utext = "From Date";
		this.btn_Search.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Search.Appearance.Options.UseBackColor = true;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(782, 30);
		this.btn_Search.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(115, 25);
		this.btn_Search.TabIndex = 71;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btnSearch_Click);
		this.txtNoOfTrans.EditControl = null;
		this.txtNoOfTrans.FieldName = null;
		this.txtNoOfTrans.IsSearchControl = false;
		this.txtNoOfTrans.Location = new System.Drawing.Point(370, 33);
		this.txtNoOfTrans.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtNoOfTrans.Name = "txtNoOfTrans";
		this.txtNoOfTrans.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtNoOfTrans.Properties.Appearance.Options.UseFont = true;
		this.txtNoOfTrans.SettingInVisible = false;
		this.txtNoOfTrans.SettingReadonly = false;
		this.txtNoOfTrans.Size = new System.Drawing.Size(163, 22);
		this.txtNoOfTrans.TabIndex = 69;
		this.txtNoOfTrans.EditValueChanged += new System.EventHandler(this.txtNoOfTrans_EditValueChanged);
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(97, 33);
		this.txtToDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtToDate.Properties.Appearance.Options.UseFont = true;
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
		this.txtToDate.Size = new System.Drawing.Size(163, 22);
		this.txtToDate.TabIndex = 67;
		this.txtToDate.EditValueChanged += new System.EventHandler(this.txtToDate_EditValueChanged);
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(97, 4);
		this.txtFromDate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtFromDate.Properties.Appearance.Options.UseFont = true;
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
		this.txtFromDate.Size = new System.Drawing.Size(163, 22);
		this.txtFromDate.TabIndex = 66;
		this.cbxStatus.EditControl = null;
		this.cbxStatus.FieldName = null;
		this.cbxStatus.IsSearchControl = false;
		this.cbxStatus.Location = new System.Drawing.Point(653, 30);
		this.cbxStatus.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.cbxStatus.Name = "cbxStatus";
		this.cbxStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbxStatus.Properties.NullText = "";
		this.cbxStatus.SettingInVisible = false;
		this.cbxStatus.SettingReadonly = false;
		this.cbxStatus.Size = new System.Drawing.Size(121, 22);
		this.cbxStatus.TabIndex = 77;
		base.AcceptButton = this.btn_Search;
		base.Appearance.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		base.ClientSize = new System.Drawing.Size(1325, 782);
		base.Controls.Add(this.cbxStatus);
		base.Controls.Add(this.txtBusinessPartner);
		base.Controls.Add(this.lb_DocStatus);
		base.Controls.Add(this.lb_BusinessPartner);
		base.Controls.Add(this.lb_DocNumber);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.txtNoOfTrans);
		base.Controls.Add(this.txtToDate);
		base.Controls.Add(this.txtFromDate);
		base.Controls.Add(this.drlCopyTo);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oSalesBlanketAgreement";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_SalesBlanketAgreement.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "frm_SalesBlanketAgreement";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Blanket Agreement";
		base.Load += new System.EventHandler(this.frm_SalesBlanketAgreement_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx8).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx9).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx10).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx11).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx12).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextDocMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSeries).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBpName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUseDiscnt).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusDF).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rListNum).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOwner).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRenewal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSettleProb).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRemindUnit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBpCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PaymentTerm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PayMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusD).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCntctCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatusA).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabControl).EndInit();
		this.tabControl.ResumeLayout(false);
		this.tabPageDetails.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDetails).EndInit();
		this.contextLineMenu.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDetails).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUgpEntry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlineStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPlanQty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRetPortion).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWrrtyEnd.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWrrtyEnd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPlanAmount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDiscount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		this.tabAttachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtBusinessPartner.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtNoOfTrans.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbxStatus.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
