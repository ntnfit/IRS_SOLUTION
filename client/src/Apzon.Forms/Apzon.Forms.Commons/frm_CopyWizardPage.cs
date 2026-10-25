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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraWizard;

namespace Apzon.Forms.Commons;

public class frm_CopyWizardPage : XtraFormEx
{
	public delegate Task PasteDrawAll(ApzObjectTypes type, List<DocumentsImpl> lst, int setRateType);

	public delegate Task PasteCustomize(DataTable dt, DataTable dtFreight, List<DocumentsImpl> lst, int setRateType, string Fromentry, ApzObjectTypes FormObjectType, ApzObjectTypes Fromtype);

	private readonly HttpClientHelper<DocumentLineTable> _docLineTableHelper;

	private readonly string _cardCode;

	private readonly List<DocumentsImpl> _lstItems = new List<DocumentsImpl>();

	private readonly ApzObjectTypes _type;

	private readonly ApzObjectTypes _toType;

	public ApzObjectTypes FromObjType;

	public string TableFreight;

	private readonly DataTable _freightAmt = new DataTable();

	private DataTable _dtFreight;

	private decimal _qtySum;

	private decimal _totalSum;

	private DateTime? _postingDate;

	public DataTable DtPaste;

	public PasteDrawAll FillDrawAll;

	public PasteCustomize FillCustomize;

	public string LineTable = "LineTable";

	private IContainer components;

	private DevExpress.XtraWizard.WizardPage wiz_customerList;

	private GridControlEx grd_ListOfDocument;

	private GridColumnEx grd_ListCustomer;

	private GridColumnEx gcPostingDate;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private GridColumnEx gcCardCode;

	private GridColumnEx gcCardName;

	private GridColumnEx gcRemark;

	private GridColumnEx gcDocDueDate;

	private GridColumnEx gcRequesterType;

	private GridColumnEx gcReqName;

	private GridColumnEx gcReqTypeName;

	private DevExpress.XtraWizard.WizardPage wiz_PageSelectItem;

	private GridControlEx grd_Select;

	private DevExpress.XtraWizard.WizardPage wiz_PageCustomize;

	private RadioGroupEx radioGroup_OptionSelect;

	private RadioGroupEx radioGroup_ExRatePrice;

	private Label label1;

	private WizardControlEx wiz_Main;

	private GridViewEx gv_ListOfDocument;

	private GridViewEx gridView1;

	private GridViewEx gv_Select;

	private GridViewEx gridView2;

	private RepositoryItemLookUpEdit rRequesterType;

	private GridColumnEx gcSalsEmployee;

	private RepositoryItemLookUpEdit rSalesEmployee;

	private DevExpress.XtraWizard.WizardPage wiz_PageSelectFreight;

	private GridControlEx grd_Freight;

	private GridViewEx gv_Freight;

	private GridColumnEx gcBaseRef;

	private GridColumnEx gcExpnsCode;

	private GridColumnEx gcLineTotal;

	private GridColumnEx gcBaseMethod;

	private GridColumnEx gcOrgExpSum;

	private RepositoryItemLookUpEdit rep_BaseMethod;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private GridColumnEx gcUnitPrice;

	private GridColumnEx gcTotalLC;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private GridColumnEx gcTotalFC;

	private GridColumnEx gcDiscount;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice4;

	public frm_CopyWizardPage()
	{
		InitializeComponent();
	}

	public frm_CopyWizardPage(ApzObjectTypes type, ApzObjectTypes toType, string cardCode, DateTime? postingDate, HttpClientHelper<DocumentLineTable> docLineTableHelper)
	{
		InitializeComponent();
		_docLineTableHelper = docLineTableHelper;
		_type = type;
		_toType = toType;
		_cardCode = cardCode;
		_postingDate = postingDate;
		FormObjectType = ApzObjectTypes.oCopyWizardPage;
		if (type == ApzObjectTypes.oBlanketAgreement || type == ApzObjectTypes.oPurchaseBlanketAgreement)
		{
			gv_ListOfDocument.ItemId = "OOAT";
			radioGroup_ExRatePrice.Visible = false;
			label1.Visible = false;
		}
		else
		{
			gv_ListOfDocument.ItemId = "ORDR";
		}
	}

	private async void frm_wizardpage_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "12",
					Name = "User"
				},
				new Common.Table_Code
				{
					Code = "171",
					Name = "Employee"
				}
			};
			Dictionary<string, string> source = new Dictionary<string, string>
			{
				{ "N", "None" },
				{ "Q", "Quantity" },
				{ "T", "Total" },
				{ "A", "All" }
			};
			rep_BaseMethod.DataSource = source.Select((KeyValuePair<string, string> r) => new Common.Table_Code
			{
				Code = r.Key,
				Name = r.Value
			});
			rRequesterType.DataSource = dataSource;
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "SaleEmployee"))
			{
				DataTable dataTable = await helper.GetDataTableAsync("ShowSaleEmployee");
				if (dataTable.IsNotNull())
				{
					rSalesEmployee.DataSource = dataTable;
				}
			}
			if (_type != ApzObjectTypes.oPurchaseRequest)
			{
				gcReqName.Visible = false;
				gcRequesterType.Visible = false;
				gcReqTypeName.Visible = false;
			}
			else
			{
				gcCardCode.Visible = false;
				gcCardName.Visible = false;
			}
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_ListOfDocument, (_type == ApzObjectTypes.oBlanketAgreement || _type == ApzObjectTypes.oPurchaseBlanketAgreement) ? "OOAT" : "ODRF"), LoadData());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DocumentTable> helper = new HttpClientHelper<DocumentTable>(Configuration.ApiServer, "Commons"))
			{
				WinCommons._OpenProcessing();
				DocumentTable dataSource = await helper.GetAsync("GetListDocumentCopy", new
				{
					objType = _type,
					toType = _toType,
					cardCode = _cardCode,
					postingDate = (_postingDate?.ToString("yyyyMMdd") ?? "")
				});
				grd_ListOfDocument.DataSource = dataSource;
				grd_ListOfDocument.RefreshDataSource();
				WinCommons._CloseProcessing();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void wiz_Main_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		_ = 3;
		try
		{
			ApzObjectTypes apzObjectTypes = _type;
			if (_type == ApzObjectTypes.oPurchaseBlanketAgreement)
			{
				apzObjectTypes = ApzObjectTypes.oBlanketAgreement;
			}
			if (e.Page == wiz_customerList)
			{
				_lstItems.Clear();
				List<Tuple<string, string>> docUDFNameAndType = WinFuntions.GetDocUDFNameAndType();
				if (gv_ListOfDocument.GetSelectedRows() != null && gv_ListOfDocument.GetSelectedRows().Length != 0)
				{
					string text = "";
					int[] selectedRows = gv_ListOfDocument.GetSelectedRows();
					foreach (int rowHandle in selectedRows)
					{
						DocumentRow documentRow = (DocumentRow)gv_ListOfDocument.GetDataRow(rowHandle);
						text = text + documentRow.DocEntry + ",";
						DocumentsImpl item = documentRow.ToDocument(docUDFNameAndType);
						_lstItems.Add(item);
					}
					IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
					_dtFreight = httpClient.Commons.GetFreight(TableFreight, text);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
					e.Handled = true;
				}
				radioGroup_OptionSelect.SelectedIndex = 1;
				radioGroup_ExRatePrice.SelectedIndex = 2;
			}
			else if (e.Page == wiz_PageCustomize)
			{
				if (!_dtFreight.IsNotNull())
				{
					wiz_Main.NextText = "Finish";
				}
				switch (radioGroup_OptionSelect.SelectedIndex)
				{
				case 0:
					e.Handled = true;
					Hide();
					switch (radioGroup_ExRatePrice.SelectedIndex)
					{
					case 0:
						await FillDrawAll(apzObjectTypes, _lstItems, 0);
						Close();
						break;
					case 1:
						await FillDrawAll(apzObjectTypes, _lstItems, 1);
						Close();
						break;
					case 2:
						await FillDrawAll(apzObjectTypes, _lstItems, 2);
						Close();
						break;
					}
					break;
				case 1:
				{
					int selectedIndex = radioGroup_ExRatePrice.SelectedIndex;
					if ((uint)selectedIndex <= 2u)
					{
						await LoadData(_lstItems, apzObjectTypes);
					}
					break;
				}
				}
			}
			else
			{
				if (e.Page != wiz_PageSelectItem)
				{
					return;
				}
				grd_Freight.DataSourceTable.Rows.Clear();
				if (!_dtFreight.IsNotNull())
				{
					wiz_Main_FinishClick(null, null);
					e.Handled = true;
				}
				else
				{
					if (!grd_Select.DataSourceTable.IsNotNull())
					{
						return;
					}
					List<DataRow> listDataSourceSelectedRow = gv_Select.GetListDataSourceSelectedRow();
					if (!listDataSourceSelectedRow.Any())
					{
						return;
					}
					if (_freightAmt.Columns.Count == 0)
					{
						_freightAmt.Columns.Add("ExpnsCode", typeof(int));
						_freightAmt.Columns.Add("Quantity", typeof(int));
						_freightAmt.Columns.Add("Total", typeof(int));
						_freightAmt.Columns.Add("All", typeof(int));
					}
					_freightAmt.Rows.Clear();
					DataTable dataTable = _dtFreight.Copy();
					dataTable.Columns.Add("BaseMethod2");
					foreach (DataRow row in dataTable.Rows)
					{
						decimal num = 0m;
						decimal num2 = 0m;
						if (_qtySum != 0m)
						{
							num = Math.Round(listDataSourceSelectedRow.Sum((DataRow r) => Function.ParseDecimal(r["Quantity"])) * Function.ParseDecimal(row["LineTotal"]) / _qtySum);
						}
						if (_totalSum != 0m)
						{
							num2 = Math.Round(listDataSourceSelectedRow.Sum((DataRow r) => Function.ParseDecimal(r["LineTotal"])) * Function.ParseDecimal(row["LineTotal"]) / _totalSum);
						}
						DataRow dataRow2 = _freightAmt.NewRow();
						dataRow2["ExpnsCode"] = Function.ParseInt(row["ExpnsCode"]);
						dataRow2["Quantity"] = num;
						dataRow2["Total"] = num2;
						dataRow2["All"] = Function.ParseInt(row["LineTotal"]);
						_freightAmt.Rows.Add(dataRow2);
						row["LineTotal"] = num2;
						row["BaseRef"] = row["DocNum"];
						row["BaseMethod2"] = "T";
					}
					_freightAmt.AcceptChanges();
					dataTable.AcceptChanges();
					grd_Freight.DataSource = dataTable;
					grd_Freight.RefreshDataSource();
					gv_Freight.SelectAll();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData(List<DocumentsImpl> lst, ApzObjectTypes objtype)
	{
		gv_Select.ShowLoadingPanel();
		try
		{
			string entry = string.Join(",", lst.Select((DocumentsImpl t) => t.DocEntry.ToString()).ToArray());
			HttpClientHelper<DocumentLineTable> docLineTableHelper = _docLineTableHelper;
			int num = (int)objtype;
			DocumentLineTable documentLineTable = await docLineTableHelper.GetAsync("CopyFromLine", new
			{
				type = num.ToString(),
				entry = entry,
				totype = ((int)_toType).ToString()
			});
			if (documentLineTable.IsNotNull())
			{
				_qtySum = documentLineTable.AsEnumerable().Sum((DataRow r) => Function.ParseDecimal(r["OpenCreQty"]));
				_totalSum = documentLineTable.AsEnumerable().Sum((DataRow r) => Function.ParseDecimal(r["LineTotal"]));
			}
			grd_Select.DataSource = documentLineTable;
			grd_Select.RefreshDataSource();
			gv_Select.SelectAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Select.HideLoadingPanel();
	}

	private async void wiz_Main_FinishClick(object sender, CancelEventArgs e)
	{
		try
		{
			ApzObjectTypes fromtype = _type;
			if (_type == ApzObjectTypes.oPurchaseBlanketAgreement)
			{
				fromtype = ApzObjectTypes.oBlanketAgreement;
			}
			DataTable dataSourceTable = grd_Select.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				return;
			}
			int[] selectedRows = gv_Select.GetSelectedRows();
			if (selectedRows.Count() > 0)
			{
				DataTable dataTable = dataSourceTable.Clone();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					DataRow dataRow = gv_Select.GetDataRow(selectedRows[i]);
					dataTable.ImportRow(dataRow);
					dataTable.AcceptChanges();
				}
				int selectedIndex = radioGroup_ExRatePrice.SelectedIndex;
				List<DataRow> listDataSourceSelectedRow = gv_Freight.GetListDataSourceSelectedRow();
				DataTable dtFreight = new DataTable();
				if (listDataSourceSelectedRow.Any())
				{
					dtFreight = listDataSourceSelectedRow.CopyToDataTable();
				}
				Hide();
				await FillCustomize(dataTable, dtFreight, _lstItems, selectedIndex, string.Join(",", _lstItems.Select((DocumentsImpl t) => t.DocEntry).Distinct()), FromObjType, fromtype);
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_BaseMethod_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Freight.CloseEditor();
			int expnsCode = Function.ParseInt(gv_Freight.GetFocusedRowCellValue("ExpnsCode"));
			if (expnsCode == 0 || !grd_Freight.DataSourceTable.IsNotNull())
			{
				return;
			}
			DataRow dataRow = _freightAmt.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["ExpnsCode"]) == expnsCode);
			if (dataRow != null)
			{
				switch (Function.ToString(gv_Freight.GetFocusedRowCellValue("BaseMethod2")))
				{
				case "N":
					gv_Freight.SetFocusedRowCellValue("LineTotal", 0);
					break;
				case "Q":
					gv_Freight.SetFocusedRowCellValue("LineTotal", Function.ParseInt(dataRow["Quantity"]));
					break;
				case "T":
					gv_Freight.SetFocusedRowCellValue("LineTotal", Function.ParseInt(dataRow["Total"]));
					break;
				case "A":
					gv_Freight.SetFocusedRowCellValue("LineTotal", Function.ParseInt(dataRow["All"]));
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void radioGroup_OptionSelect_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (radioGroup_OptionSelect.SelectedIndex == 0)
		{
			wiz_Main.NextText = "Finish";
		}
		else
		{
			wiz_Main.NextText = "&Next >";
		}
	}

	private void gv_Select_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (new string[3] { "Quantity", "PriceBefDi", "DiscPrcnt" }.Contains(e.Column.FieldName))
			{
				int num = Function.ParseInt(gv_Select.GetRowCellValue(e.RowHandle, "Quantity"));
				double num2 = Function.ParseDouble(gv_Select.GetRowCellValue(e.RowHandle, "PriceBefDi"));
				double num3 = Function.ParseDouble(gv_Select.GetRowCellValue(e.RowHandle, "Rate"));
				string text = Function.ToString(gv_Select.GetRowCellValue(e.RowHandle, "Currency"));
				double num4 = Function.ParseDouble(gv_Select.GetRowCellValue(e.RowHandle, "DiscPrcnt"));
				double num5 = Function.ParseDouble(gv_Select.GetRowCellValue(e.RowHandle, "VatPrcnt"));
				double num6 = (double)num * num2 * (1.0 - num4 / 100.0);
				double num7 = num6 * (1.0 + num5 / 100.0);
				if (text == GlobalData.MainCurrency)
				{
					gv_Select.SetRowCellValue(e.RowHandle, "LineTotal", Math.Round(num6));
					gv_Select.SetRowCellValue(e.RowHandle, "TotalFrgn", 0);
					gv_Select.SetRowCellValue(e.RowHandle, "GTotal", Math.Round(num7));
					gv_Select.SetRowCellValue(e.RowHandle, "GTotalFC", 0);
				}
				else
				{
					gv_Select.SetRowCellValue(e.RowHandle, "TotalFrgn", Math.Round(num6));
					gv_Select.SetRowCellValue(e.RowHandle, "LineTotal", Math.Round(num6 * num3));
					gv_Select.SetRowCellValue(e.RowHandle, "GTotalFC", Math.Round(num7));
					gv_Select.SetRowCellValue(e.RowHandle, "GTotal", Math.Round(num7 * num3));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void wiz_Main_PrevClick(object sender, WizardCommandButtonClickEventArgs e)
	{
		wiz_Main.NextText = "&Next >";
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
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice4 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice3 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.wiz_customerList = new DevExpress.XtraWizard.WizardPage();
		this.grd_ListOfDocument = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListOfDocument = new Apzon.Controls.Grid.GridViewEx();
		this.grd_ListCustomer = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPostingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDocDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRequesterType = new Apzon.Controls.Grid.GridColumnEx();
		this.rRequesterType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcReqName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReqTypeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSalsEmployee = new Apzon.Controls.Grid.GridColumnEx();
		this.rSalesEmployee = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.wiz_PageSelectItem = new DevExpress.XtraWizard.WizardPage();
		this.grd_Select = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Select = new Apzon.Controls.Grid.GridViewEx();
		this.gcDiscount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUnitPrice = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTotalLC = new Apzon.Controls.Grid.GridColumnEx();
		this.gcTotalFC = new Apzon.Controls.Grid.GridColumnEx();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.wiz_PageCustomize = new DevExpress.XtraWizard.WizardPage();
		this.radioGroup_OptionSelect = new Apzon.Controls.Editor.RadioGroupEx();
		this.radioGroup_ExRatePrice = new Apzon.Controls.Editor.RadioGroupEx();
		this.label1 = new System.Windows.Forms.Label();
		this.wiz_Main = new Apzon.Controls.Controls.WizardControlEx();
		this.wiz_PageSelectFreight = new DevExpress.XtraWizard.WizardPage();
		this.grd_Freight = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Freight = new Apzon.Controls.Grid.GridViewEx();
		this.gcBaseRef = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExpnsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BaseMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcOrgExpSum = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		this.wiz_customerList.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ListOfDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListOfDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRequesterType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSalesEmployee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		this.wiz_PageSelectItem.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Select).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Select).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		this.wiz_PageCustomize.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.radioGroup_OptionSelect.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup_ExRatePrice.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.wiz_Main).BeginInit();
		this.wiz_Main.SuspendLayout();
		this.wiz_PageSelectFreight.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Freight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Freight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseMethod).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemTextEditExPrice4.Name = "repositoryItemTextEditExPrice4";
		this.repositoryItemTextEditExPrice3.Name = "repositoryItemTextEditExPrice3";
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.wiz_customerList.Controls.Add(this.grd_ListOfDocument);
		this.wiz_customerList.DescriptionText = "Check Row To Selected";
		this.wiz_customerList.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_customerList.Name = "wiz_customerList";
		this.wiz_customerList.Size = new System.Drawing.Size(917, 578);
		this.wiz_customerList.Text = "Show List Customers to Select";
		this.grd_ListOfDocument.AllowPasteExcel = true;
		this.grd_ListOfDocument.ClipboardData = null;
		this.grd_ListOfDocument.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ListOfDocument.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_ListOfDocument.Location = new System.Drawing.Point(0, 0);
		this.grd_ListOfDocument.MainView = this.gv_ListOfDocument;
		this.grd_ListOfDocument.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_ListOfDocument.Name = "grd_ListOfDocument";
		this.grd_ListOfDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemDateEdit1, this.rRequesterType, this.rSalesEmployee });
		this.grd_ListOfDocument.Size = new System.Drawing.Size(917, 578);
		this.grd_ListOfDocument.TabIndex = 3;
		this.grd_ListOfDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_ListOfDocument, this.gridView1 });
		this.gv_ListOfDocument.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListOfDocument.ColumnKey = null;
		this.gv_ListOfDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.grd_ListCustomer, this.gcPostingDate, this.gcCardCode, this.gcCardName, this.gcRemark, this.gcDocDueDate, this.gcRequesterType, this.gcReqName, this.gcReqTypeName, this.gcSalsEmployee });
		this.gv_ListOfDocument.DetailHeight = 431;
		this.gv_ListOfDocument.DisableFormSetting = false;
		this.gv_ListOfDocument.DisplayException = false;
		this.gv_ListOfDocument.EditColumn = null;
		this.gv_ListOfDocument.FieldCurrency = null;
		this.gv_ListOfDocument.FieldRate = null;
		this.gv_ListOfDocument.FormId = "DocumentCopyWizard";
		this.gv_ListOfDocument.GridControl = this.grd_ListOfDocument;
		this.gv_ListOfDocument.IgnoreFilter = false;
		this.gv_ListOfDocument.IndexEditRow = -1;
		this.gv_ListOfDocument.IndicatorWidth = 47;
		this.gv_ListOfDocument.IsApzTable = false;
		this.gv_ListOfDocument.IsInsertNewRow = false;
		this.gv_ListOfDocument.IsLoadingData = false;
		this.gv_ListOfDocument.IsMappingContextAction = false;
		this.gv_ListOfDocument.ItemId = "header";
		this.gv_ListOfDocument.LstColumnAllowBatch = null;
		this.gv_ListOfDocument.LstColumnNotAllowBatch = null;
		this.gv_ListOfDocument.LstColumnNotEditable = null;
		this.gv_ListOfDocument.LstColumNotSetting = null;
		this.gv_ListOfDocument.Name = "gv_ListOfDocument";
		this.gv_ListOfDocument.OptionsBehavior.ReadOnly = true;
		this.gv_ListOfDocument.OptionsDetail.EnableMasterViewMode = false;
		this.gv_ListOfDocument.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
		this.gv_ListOfDocument.OptionsFind.AlwaysVisible = true;
		this.gv_ListOfDocument.OptionsFind.FindDelay = 100;
		this.gv_ListOfDocument.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_ListOfDocument.OptionsSelection.MultiSelect = true;
		this.gv_ListOfDocument.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_ListOfDocument.OptionsView.ColumnAutoWidth = false;
		this.gv_ListOfDocument.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_ListOfDocument.OptionsView.ShowGroupPanel = false;
		this.gv_ListOfDocument.UseBatchMod = false;
		this.grd_ListCustomer.Caption = "#";
		this.grd_ListCustomer.CurrencyDateField = null;
		this.grd_ListCustomer.CurrencyField = null;
		this.grd_ListCustomer.CurrencyFixed = null;
		this.grd_ListCustomer.CurrencyFixedRate = 0.0;
		this.grd_ListCustomer.CurrencyRateField = null;
		this.grd_ListCustomer.FieldName = "DocNum";
		this.grd_ListCustomer.MinWidth = 23;
		this.grd_ListCustomer.Name = "grd_ListCustomer";
		this.grd_ListCustomer.OptionsColumn.ReadOnly = true;
		this.grd_ListCustomer.PrimaryKey = true;
		this.grd_ListCustomer.UnitField = null;
		this.grd_ListCustomer.Visible = true;
		this.grd_ListCustomer.VisibleIndex = 1;
		this.grd_ListCustomer.Width = 61;
		this.gcPostingDate.Caption = "Posting Date";
		this.gcPostingDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcPostingDate.CurrencyDateField = null;
		this.gcPostingDate.CurrencyField = null;
		this.gcPostingDate.CurrencyFixed = null;
		this.gcPostingDate.CurrencyFixedRate = 0.0;
		this.gcPostingDate.CurrencyRateField = null;
		this.gcPostingDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcPostingDate.FieldName = "DocDate";
		this.gcPostingDate.MinWidth = 23;
		this.gcPostingDate.Name = "gcPostingDate";
		this.gcPostingDate.OptionsColumn.ReadOnly = true;
		this.gcPostingDate.PrimaryKey = false;
		this.gcPostingDate.UnitField = null;
		this.gcPostingDate.Visible = true;
		this.gcPostingDate.VisibleIndex = 2;
		this.gcPostingDate.Width = 111;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyDateField = null;
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.MinWidth = 23;
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.OptionsColumn.ReadOnly = true;
		this.gcCardCode.PrimaryKey = false;
		this.gcCardCode.UnitField = null;
		this.gcCardCode.Visible = true;
		this.gcCardCode.VisibleIndex = 4;
		this.gcCardCode.Width = 104;
		this.gcCardName.Caption = "Customer";
		this.gcCardName.CurrencyDateField = null;
		this.gcCardName.CurrencyField = null;
		this.gcCardName.CurrencyFixed = null;
		this.gcCardName.CurrencyFixedRate = 0.0;
		this.gcCardName.CurrencyRateField = null;
		this.gcCardName.FieldName = "CardName";
		this.gcCardName.MinWidth = 23;
		this.gcCardName.Name = "gcCardName";
		this.gcCardName.OptionsColumn.ReadOnly = true;
		this.gcCardName.PrimaryKey = false;
		this.gcCardName.UnitField = null;
		this.gcCardName.Visible = true;
		this.gcCardName.VisibleIndex = 5;
		this.gcCardName.Width = 136;
		this.gcRemark.Caption = "Remark";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Comments";
		this.gcRemark.MinWidth = 23;
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.OptionsColumn.ReadOnly = true;
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 6;
		this.gcRemark.Width = 216;
		this.gcDocDueDate.Caption = "DueDate";
		this.gcDocDueDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcDocDueDate.CurrencyDateField = null;
		this.gcDocDueDate.CurrencyField = null;
		this.gcDocDueDate.CurrencyFixed = null;
		this.gcDocDueDate.CurrencyFixedRate = 0.0;
		this.gcDocDueDate.CurrencyRateField = null;
		this.gcDocDueDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcDocDueDate.FieldName = "DocDueDate";
		this.gcDocDueDate.MinWidth = 23;
		this.gcDocDueDate.Name = "gcDocDueDate";
		this.gcDocDueDate.OptionsColumn.ReadOnly = true;
		this.gcDocDueDate.PrimaryKey = false;
		this.gcDocDueDate.UnitField = null;
		this.gcDocDueDate.Visible = true;
		this.gcDocDueDate.VisibleIndex = 7;
		this.gcDocDueDate.Width = 205;
		this.gcRequesterType.Caption = "Requester Type";
		this.gcRequesterType.ColumnEdit = this.rRequesterType;
		this.gcRequesterType.CurrencyDateField = null;
		this.gcRequesterType.CurrencyField = null;
		this.gcRequesterType.CurrencyFixed = null;
		this.gcRequesterType.CurrencyFixedRate = 0.0;
		this.gcRequesterType.CurrencyRateField = null;
		this.gcRequesterType.FieldName = "ReqType";
		this.gcRequesterType.MinWidth = 23;
		this.gcRequesterType.Name = "gcRequesterType";
		this.gcRequesterType.OptionsColumn.ReadOnly = true;
		this.gcRequesterType.PrimaryKey = false;
		this.gcRequesterType.UnitField = null;
		this.gcRequesterType.Visible = true;
		this.gcRequesterType.VisibleIndex = 8;
		this.gcRequesterType.Width = 135;
		this.rRequesterType.AutoHeight = false;
		this.rRequesterType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rRequesterType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rRequesterType.DisplayMember = "Name";
		this.rRequesterType.Name = "rRequesterType";
		this.rRequesterType.NullText = "";
		this.rRequesterType.ShowHeader = false;
		this.rRequesterType.ValueMember = "Code";
		this.gcReqName.Caption = "Requester";
		this.gcReqName.CurrencyDateField = null;
		this.gcReqName.CurrencyField = null;
		this.gcReqName.CurrencyFixed = null;
		this.gcReqName.CurrencyFixedRate = 0.0;
		this.gcReqName.CurrencyRateField = null;
		this.gcReqName.FieldName = "ReqName";
		this.gcReqName.MinWidth = 23;
		this.gcReqName.Name = "gcReqName";
		this.gcReqName.OptionsColumn.ReadOnly = true;
		this.gcReqName.PrimaryKey = false;
		this.gcReqName.UnitField = null;
		this.gcReqName.Visible = true;
		this.gcReqName.VisibleIndex = 3;
		this.gcReqName.Width = 139;
		this.gcReqTypeName.Caption = "Requester Type Name";
		this.gcReqTypeName.CurrencyDateField = null;
		this.gcReqTypeName.CurrencyField = null;
		this.gcReqTypeName.CurrencyFixed = null;
		this.gcReqTypeName.CurrencyFixedRate = 0.0;
		this.gcReqTypeName.CurrencyRateField = null;
		this.gcReqTypeName.FieldName = "ReqTypeName";
		this.gcReqTypeName.MinWidth = 23;
		this.gcReqTypeName.Name = "gcReqTypeName";
		this.gcReqTypeName.OptionsColumn.ReadOnly = true;
		this.gcReqTypeName.PrimaryKey = false;
		this.gcReqTypeName.UnitField = null;
		this.gcReqTypeName.Width = 104;
		this.gcSalsEmployee.Caption = "Sales Employee";
		this.gcSalsEmployee.ColumnEdit = this.rSalesEmployee;
		this.gcSalsEmployee.CurrencyDateField = null;
		this.gcSalsEmployee.CurrencyField = null;
		this.gcSalsEmployee.CurrencyFixed = null;
		this.gcSalsEmployee.CurrencyFixedRate = 0.0;
		this.gcSalsEmployee.CurrencyRateField = null;
		this.gcSalsEmployee.FieldName = "SlpCode";
		this.gcSalsEmployee.MinWidth = 23;
		this.gcSalsEmployee.Name = "gcSalsEmployee";
		this.gcSalsEmployee.OptionsColumn.ReadOnly = true;
		this.gcSalsEmployee.PrimaryKey = false;
		this.gcSalsEmployee.UnitField = null;
		this.gcSalsEmployee.Visible = true;
		this.gcSalsEmployee.VisibleIndex = 9;
		this.gcSalsEmployee.Width = 87;
		this.rSalesEmployee.AutoHeight = false;
		this.rSalesEmployee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rSalesEmployee.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpCode", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name")
		});
		this.rSalesEmployee.DisplayMember = "SlpName";
		this.rSalesEmployee.Name = "rSalesEmployee";
		this.rSalesEmployee.NullText = "";
		this.rSalesEmployee.ValueMember = "SlpCode";
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DetailHeight = 431;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "DocumentCopyWizard";
		this.gridView1.GridControl = this.grd_ListOfDocument;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 41;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "OGRIDVIEW1";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.wiz_PageSelectItem.Controls.Add(this.grd_Select);
		this.wiz_PageSelectItem.DescriptionText = "Select line then press next to copy";
		this.wiz_PageSelectItem.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_PageSelectItem.Name = "wiz_PageSelectItem";
		this.wiz_PageSelectItem.Size = new System.Drawing.Size(917, 578);
		this.wiz_PageSelectItem.Text = "Choose to Copy";
		this.grd_Select.AllowPasteExcel = false;
		this.grd_Select.ClipboardData = null;
		this.grd_Select.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Select.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Select.Location = new System.Drawing.Point(0, 0);
		this.grd_Select.MainView = this.gv_Select;
		this.grd_Select.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Select.Name = "grd_Select";
		this.grd_Select.Size = new System.Drawing.Size(917, 578);
		this.grd_Select.TabIndex = 0;
		this.grd_Select.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Select, this.gridView2 });
		this.gv_Select.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Select.ColumnKey = null;
		this.gv_Select.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcDiscount, this.gcUnitPrice, this.gcTotalLC, this.gcTotalFC });
		this.gv_Select.DetailHeight = 431;
		this.gv_Select.DisableFormSetting = false;
		this.gv_Select.DisplayException = false;
		this.gv_Select.EditColumn = null;
		this.gv_Select.FieldCurrency = "Currency";
		this.gv_Select.FieldRate = "Rate";
		this.gv_Select.FormId = "DocumentCopyWizard";
		this.gv_Select.GridControl = this.grd_Select;
		this.gv_Select.IgnoreFilter = false;
		this.gv_Select.IndexEditRow = -1;
		this.gv_Select.IndicatorWidth = 47;
		this.gv_Select.IsApzTable = false;
		this.gv_Select.IsInsertNewRow = false;
		this.gv_Select.IsLoadingData = false;
		this.gv_Select.IsMappingContextAction = false;
		this.gv_Select.ItemId = "LineTable";
		this.gv_Select.LstColumnAllowBatch = null;
		this.gv_Select.LstColumnNotAllowBatch = null;
		this.gv_Select.LstColumnNotEditable = null;
		this.gv_Select.LstColumNotSetting = null;
		this.gv_Select.Name = "gv_Select";
		this.gv_Select.OptionsSelection.CheckBoxSelectorColumnWidth = 35;
		this.gv_Select.OptionsSelection.MultiSelect = true;
		this.gv_Select.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Select.OptionsView.ColumnAutoWidth = false;
		this.gv_Select.UseBatchMod = false;
		this.gv_Select.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Select_CellValueChanged);
		this.gcDiscount.Caption = "Discount %";
		this.gcDiscount.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gcDiscount.CurrencyDateField = null;
		this.gcDiscount.CurrencyField = null;
		this.gcDiscount.CurrencyFixed = null;
		this.gcDiscount.CurrencyFixedRate = 0.0;
		this.gcDiscount.CurrencyRateField = null;
		this.gcDiscount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcDiscount.FieldName = "DiscPrcnt";
		this.gcDiscount.MinWidth = 23;
		this.gcDiscount.Name = "gcDiscount";
		this.gcDiscount.PrimaryKey = false;
		this.gcDiscount.UnitField = null;
		this.gcDiscount.Visible = true;
		this.gcDiscount.VisibleIndex = 1;
		this.gcDiscount.Width = 87;
		this.gcUnitPrice.Caption = "Unit Price";
		this.gcUnitPrice.ColumnEdit = this.repositoryItemTextEditExPrice3;
		this.gcUnitPrice.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcUnitPrice.CurrencyDateField = null;
		this.gcUnitPrice.CurrencyField = "Currency";
		this.gcUnitPrice.CurrencyFixed = null;
		this.gcUnitPrice.CurrencyFixedRate = 0.0;
		this.gcUnitPrice.CurrencyRateField = "Rate";
		this.gcUnitPrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcUnitPrice.FieldName = "PriceBefDi";
		this.gcUnitPrice.MinWidth = 23;
		this.gcUnitPrice.Name = "gcUnitPrice";
		this.gcUnitPrice.PrimaryKey = false;
		this.gcUnitPrice.UnitField = null;
		this.gcUnitPrice.Visible = true;
		this.gcUnitPrice.VisibleIndex = 4;
		this.gcUnitPrice.Width = 87;
		this.gcTotalLC.Caption = "Total (LC)";
		this.gcTotalLC.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gcTotalLC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.MainCurrency;
		this.gcTotalLC.CurrencyDateField = null;
		this.gcTotalLC.CurrencyField = "";
		this.gcTotalLC.CurrencyFixed = null;
		this.gcTotalLC.CurrencyFixedRate = 0.0;
		this.gcTotalLC.CurrencyRateField = "Rate";
		this.gcTotalLC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcTotalLC.FieldName = "LineTotal";
		this.gcTotalLC.MinWidth = 23;
		this.gcTotalLC.Name = "gcTotalLC";
		this.gcTotalLC.PrimaryKey = false;
		this.gcTotalLC.UnitField = null;
		this.gcTotalLC.Visible = true;
		this.gcTotalLC.VisibleIndex = 2;
		this.gcTotalLC.Width = 87;
		this.gcTotalFC.Caption = "Total (FC)";
		this.gcTotalFC.ColumnEdit = this.repositoryItemTextEditExPrice4;
		this.gcTotalFC.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gcTotalFC.CurrencyDateField = null;
		this.gcTotalFC.CurrencyField = "Currency";
		this.gcTotalFC.CurrencyFixed = null;
		this.gcTotalFC.CurrencyFixedRate = 0.0;
		this.gcTotalFC.CurrencyRateField = "Rate";
		this.gcTotalFC.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcTotalFC.FieldName = "TotalFrgn";
		this.gcTotalFC.MinWidth = 23;
		this.gcTotalFC.Name = "gcTotalFC";
		this.gcTotalFC.PrimaryKey = false;
		this.gcTotalFC.UnitField = null;
		this.gcTotalFC.Visible = true;
		this.gcTotalFC.VisibleIndex = 3;
		this.gcTotalFC.Width = 87;
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DetailHeight = 431;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = "DocumentCopyWizard";
		this.gridView2.GridControl = this.grd_Select;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 41;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = "GRIDVIEW2";
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.UseBatchMod = false;
		this.wiz_PageCustomize.Controls.Add(this.radioGroup_OptionSelect);
		this.wiz_PageCustomize.Controls.Add(this.radioGroup_ExRatePrice);
		this.wiz_PageCustomize.Controls.Add(this.label1);
		this.wiz_PageCustomize.DescriptionText = "Setup Ex.Rate For Rice and Draw Data";
		this.wiz_PageCustomize.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_PageCustomize.Name = "wiz_PageCustomize";
		this.wiz_PageCustomize.Size = new System.Drawing.Size(917, 578);
		this.wiz_PageCustomize.Text = "Customize to Setup and Copy Data";
		this.radioGroup_OptionSelect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.radioGroup_OptionSelect.EditControl = null;
		this.radioGroup_OptionSelect.EditValue = true;
		this.radioGroup_OptionSelect.FieldName = null;
		this.radioGroup_OptionSelect.IsSearchControl = false;
		this.radioGroup_OptionSelect.Location = new System.Drawing.Point(89, 438);
		this.radioGroup_OptionSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.radioGroup_OptionSelect.Name = "radioGroup_OptionSelect";
		this.radioGroup_OptionSelect.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroup_OptionSelect.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.radioGroup_OptionSelect.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup_OptionSelect.Properties.Appearance.Options.UseForeColor = true;
		this.radioGroup_OptionSelect.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup_OptionSelect.Properties.Columns = 1;
		this.radioGroup_OptionSelect.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Draw All Data (Freight and Withholding Tax)"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Customize")
		});
		this.radioGroup_OptionSelect.SettingInVisible = false;
		this.radioGroup_OptionSelect.SettingReadonly = false;
		this.radioGroup_OptionSelect.Size = new System.Drawing.Size(575, 69);
		this.radioGroup_OptionSelect.TabIndex = 10;
		this.radioGroup_OptionSelect.SelectedIndexChanged += new System.EventHandler(this.radioGroup_OptionSelect_SelectedIndexChanged);
		this.radioGroup_ExRatePrice.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.radioGroup_ExRatePrice.EditControl = null;
		this.radioGroup_ExRatePrice.EditValue = true;
		this.radioGroup_ExRatePrice.FieldName = null;
		this.radioGroup_ExRatePrice.IsSearchControl = false;
		this.radioGroup_ExRatePrice.Location = new System.Drawing.Point(89, 97);
		this.radioGroup_ExRatePrice.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.radioGroup_ExRatePrice.Name = "radioGroup_ExRatePrice";
		this.radioGroup_ExRatePrice.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroup_ExRatePrice.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.radioGroup_ExRatePrice.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroup_ExRatePrice.Properties.Appearance.Options.UseForeColor = true;
		this.radioGroup_ExRatePrice.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroup_ExRatePrice.Properties.Columns = 1;
		this.radioGroup_ExRatePrice.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[3]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Use Row Exchange Rate from Base Document"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Use Doc and Row Exchange Rate from Base Doc"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Use Current Exchange Rate from the Exchange Rate Table")
		});
		this.radioGroup_ExRatePrice.SettingInVisible = false;
		this.radioGroup_ExRatePrice.SettingReadonly = false;
		this.radioGroup_ExRatePrice.Size = new System.Drawing.Size(701, 89);
		this.radioGroup_ExRatePrice.TabIndex = 9;
		this.label1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.label1.AutoSize = true;
		this.label1.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, 0);
		this.label1.Location = new System.Drawing.Point(85, 78);
		this.label1.Name = "label1";
		this.label1.Size = new System.Drawing.Size(164, 17);
		this.label1.TabIndex = 8;
		this.label1.Text = "Row Ex. Rate For Price";
		this.wiz_Main.AllowPagePadding = false;
		this.wiz_Main.Controls.Add(this.wiz_PageCustomize);
		this.wiz_Main.Controls.Add(this.wiz_PageSelectItem);
		this.wiz_Main.Controls.Add(this.wiz_customerList);
		this.wiz_Main.Controls.Add(this.wiz_PageSelectFreight);
		this.wiz_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wiz_Main.ImageWidth = 216;
		this.wiz_Main.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_Main.MinimumSize = new System.Drawing.Size(117, 123);
		this.wiz_Main.Name = "wiz_Main";
		this.wiz_Main.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[4] { this.wiz_customerList, this.wiz_PageCustomize, this.wiz_PageSelectItem, this.wiz_PageSelectFreight });
		this.wiz_Main.Size = new System.Drawing.Size(917, 635);
		this.wiz_Main.Text = "";
		this.wiz_Main.TitleHeight = 10;
		this.wiz_Main.FinishClick += new System.ComponentModel.CancelEventHandler(this.wiz_Main_FinishClick);
		this.wiz_Main.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_Main_NextClick);
		this.wiz_Main.PrevClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wiz_Main_PrevClick);
		this.wiz_PageSelectFreight.Controls.Add(this.grd_Freight);
		this.wiz_PageSelectFreight.DescriptionText = "Select Freight Charges to Copy";
		this.wiz_PageSelectFreight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.wiz_PageSelectFreight.Name = "wiz_PageSelectFreight";
		this.wiz_PageSelectFreight.Size = new System.Drawing.Size(917, 578);
		this.grd_Freight.AllowPasteExcel = false;
		this.grd_Freight.ClipboardData = null;
		this.grd_Freight.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Freight.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Freight.Location = new System.Drawing.Point(0, 0);
		this.grd_Freight.MainView = this.gv_Freight;
		this.grd_Freight.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.grd_Freight.Name = "grd_Freight";
		this.grd_Freight.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_BaseMethod });
		this.grd_Freight.Size = new System.Drawing.Size(917, 578);
		this.grd_Freight.TabIndex = 0;
		this.grd_Freight.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Freight });
		this.gv_Freight.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Freight.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Freight.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Freight.ColumnKey = null;
		this.gv_Freight.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcBaseRef, this.gcExpnsCode, this.gcLineTotal, this.gcBaseMethod, this.gcOrgExpSum });
		this.gv_Freight.DetailHeight = 431;
		this.gv_Freight.DisableFormSetting = false;
		this.gv_Freight.DisplayException = false;
		this.gv_Freight.EditColumn = null;
		this.gv_Freight.FieldCurrency = null;
		this.gv_Freight.FieldRate = null;
		this.gv_Freight.FormId = null;
		this.gv_Freight.GridControl = this.grd_Freight;
		this.gv_Freight.IgnoreFilter = false;
		this.gv_Freight.IndexEditRow = -1;
		this.gv_Freight.IndicatorWidth = 41;
		this.gv_Freight.IsApzTable = false;
		this.gv_Freight.IsInsertNewRow = false;
		this.gv_Freight.IsLoadingData = false;
		this.gv_Freight.IsMappingContextAction = false;
		this.gv_Freight.ItemId = null;
		this.gv_Freight.LstColumnAllowBatch = null;
		this.gv_Freight.LstColumnNotAllowBatch = null;
		this.gv_Freight.LstColumnNotEditable = null;
		this.gv_Freight.LstColumNotSetting = null;
		this.gv_Freight.Name = "gv_Freight";
		this.gv_Freight.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Freight.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Freight.OptionsPrint.AutoWidth = false;
		this.gv_Freight.OptionsPrint.UsePrintStyles = false;
		this.gv_Freight.OptionsSelection.MultiSelect = true;
		this.gv_Freight.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Freight.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Freight.OptionsView.ColumnAutoWidth = false;
		this.gv_Freight.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Freight.OptionsView.ShowGroupedColumns = true;
		this.gv_Freight.OptionsView.ShowGroupPanel = false;
		this.gv_Freight.UseBatchMod = false;
		this.gcBaseRef.Caption = "Base Document";
		this.gcBaseRef.CurrencyDateField = null;
		this.gcBaseRef.CurrencyField = null;
		this.gcBaseRef.CurrencyFixed = null;
		this.gcBaseRef.CurrencyFixedRate = 0.0;
		this.gcBaseRef.CurrencyRateField = null;
		this.gcBaseRef.FieldName = "BaseRef";
		this.gcBaseRef.MinWidth = 23;
		this.gcBaseRef.Name = "gcBaseRef";
		this.gcBaseRef.OptionsColumn.AllowFocus = false;
		this.gcBaseRef.OptionsColumn.ReadOnly = true;
		this.gcBaseRef.PrimaryKey = false;
		this.gcBaseRef.UnitField = null;
		this.gcBaseRef.Visible = true;
		this.gcBaseRef.VisibleIndex = 1;
		this.gcBaseRef.Width = 98;
		this.gcExpnsCode.Caption = "Freight Name";
		this.gcExpnsCode.CurrencyDateField = null;
		this.gcExpnsCode.CurrencyField = null;
		this.gcExpnsCode.CurrencyFixed = null;
		this.gcExpnsCode.CurrencyFixedRate = 0.0;
		this.gcExpnsCode.CurrencyRateField = null;
		this.gcExpnsCode.FieldName = "ExpnsName";
		this.gcExpnsCode.MinWidth = 23;
		this.gcExpnsCode.Name = "gcExpnsCode";
		this.gcExpnsCode.OptionsColumn.AllowFocus = false;
		this.gcExpnsCode.OptionsColumn.ReadOnly = true;
		this.gcExpnsCode.PrimaryKey = false;
		this.gcExpnsCode.UnitField = null;
		this.gcExpnsCode.Visible = true;
		this.gcExpnsCode.VisibleIndex = 2;
		this.gcExpnsCode.Width = 87;
		this.gcLineTotal.Caption = "Amount";
		this.gcLineTotal.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.gcLineTotal.CurrencyDateField = null;
		this.gcLineTotal.CurrencyField = null;
		this.gcLineTotal.CurrencyFixed = null;
		this.gcLineTotal.CurrencyFixedRate = 0.0;
		this.gcLineTotal.CurrencyRateField = null;
		this.gcLineTotal.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLineTotal.FieldName = "LineTotal";
		this.gcLineTotal.MinWidth = 23;
		this.gcLineTotal.Name = "gcLineTotal";
		this.gcLineTotal.PrimaryKey = false;
		this.gcLineTotal.UnitField = null;
		this.gcLineTotal.Visible = true;
		this.gcLineTotal.VisibleIndex = 3;
		this.gcLineTotal.Width = 87;
		this.gcBaseMethod.Caption = "Drawing Method";
		this.gcBaseMethod.ColumnEdit = this.rep_BaseMethod;
		this.gcBaseMethod.CurrencyDateField = null;
		this.gcBaseMethod.CurrencyField = null;
		this.gcBaseMethod.CurrencyFixed = null;
		this.gcBaseMethod.CurrencyFixedRate = 0.0;
		this.gcBaseMethod.CurrencyRateField = null;
		this.gcBaseMethod.FieldName = "BaseMethod2";
		this.gcBaseMethod.MinWidth = 23;
		this.gcBaseMethod.Name = "gcBaseMethod";
		this.gcBaseMethod.PrimaryKey = false;
		this.gcBaseMethod.UnitField = null;
		this.gcBaseMethod.Visible = true;
		this.gcBaseMethod.VisibleIndex = 4;
		this.gcBaseMethod.Width = 103;
		this.rep_BaseMethod.AutoHeight = false;
		this.rep_BaseMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_BaseMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_BaseMethod.DisplayMember = "Name";
		this.rep_BaseMethod.Name = "rep_BaseMethod";
		this.rep_BaseMethod.NullText = "";
		this.rep_BaseMethod.ShowHeader = false;
		this.rep_BaseMethod.ValueMember = "Code";
		this.rep_BaseMethod.EditValueChanged += new System.EventHandler(this.rep_BaseMethod_EditValueChanged);
		this.gcOrgExpSum.Caption = "OrgExpSum";
		this.gcOrgExpSum.CurrencyDateField = null;
		this.gcOrgExpSum.CurrencyField = null;
		this.gcOrgExpSum.CurrencyFixed = null;
		this.gcOrgExpSum.CurrencyFixedRate = 0.0;
		this.gcOrgExpSum.CurrencyRateField = null;
		this.gcOrgExpSum.MinWidth = 23;
		this.gcOrgExpSum.Name = "gcOrgExpSum";
		this.gcOrgExpSum.PrimaryKey = false;
		this.gcOrgExpSum.UnitField = null;
		this.gcOrgExpSum.Visible = true;
		this.gcOrgExpSum.VisibleIndex = 5;
		this.gcOrgExpSum.Width = 87;
		base.AutoScaleDimensions = new System.Drawing.SizeF(7f, 16f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(917, 635);
		base.Controls.Add(this.wiz_Main);
		base.FormId = "DocumentCopyWizard";
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		base.Name = "frm_CopyWizardPage";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Draw Wizard Setup";
		base.Load += new System.EventHandler(this.frm_wizardpage_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		this.wiz_customerList.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ListOfDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListOfDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRequesterType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSalesEmployee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		this.wiz_PageSelectItem.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Select).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Select).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		this.wiz_PageCustomize.ResumeLayout(false);
		this.wiz_PageCustomize.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.radioGroup_OptionSelect.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroup_ExRatePrice.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.wiz_Main).EndInit();
		this.wiz_Main.ResumeLayout(false);
		this.wiz_PageSelectFreight.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Freight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Freight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseMethod).EndInit();
		base.ResumeLayout(false);
	}
}
