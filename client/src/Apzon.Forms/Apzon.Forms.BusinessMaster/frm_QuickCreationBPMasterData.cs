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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Administrator.Business_Partners;
using Apzon.Forms.Administrator.SetUp.SalesEmployeeBuyers;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Financials;
using Apzon.Forms.Interfaces.BusinessPartner;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.BusinessMaster;

public class frm_QuickCreationBPMasterData : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<Common.Table_Code> lstCurrency;

	private int firstPaymentTermValue = -1;

	private List<BPImpls> lstRowChanged = new List<BPImpls>();

	private bool isLoadData;

	private int pageNumber = 1;

	private int pageSize = 50;

	private int numberOfRecord;

	private int totalPage;

	private IContainer components;

	private GridControlEx grd_QCBPMD;

	private GridViewEx gv_QCBPMD;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridColumnEx gc_BPType;

	private GridColumnEx gc_BPCode;

	private GridColumnEx gc_BPName;

	private GridColumnEx gc_BPForeignName;

	private GridColumnEx gc_BPGroup;

	private GridColumnEx gc_Currency;

	private GridColumnEx gc_SalesEmployee;

	private GridColumnEx gc_AliasName;

	private GridColumnEx gc_PaymentTerm;

	private GridColumnEx gc_CreateLimit;

	private GridColumnEx gc_Tel;

	private GridColumnEx gc_Email;

	private GridColumnEx gc_Mobile;

	private GridColumnEx gc_Remark;

	private RepositoryItemLookUpEdit txt_BPType;

	private RepositoryItemLookUpEdit txt_BPGroup;

	private ContextMenuStrip contextMenuStrip1;

	private RepositoryItemLookUpEdit txt_Currency;

	private RepositoryItemLookUpEdit txt_PaymentTerm;

	private RepositoryItemCalcEdit txt_CreditLimit;

	private RepositoryItemLookUpEdit txt_SaleEmployee;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemButtonEditCFL txt_BPCode;

	private ToolStripMenuItem addToolStripMenuItem;

	private RepositoryItemButtonEditCFL txt_BPGroup1;

	private LabelControlEx labelControlEx2;

	private LookUpEditEx txt_pageSize;

	private LabelControlEx labelControlEx1;

	private SimpleButtonEx btn_pagerPrevious;

	private SimpleButtonEx btn_pagerFirst;

	private SimpleButtonEx btn_pagerLast;

	private SimpleButtonEx btn_pagerNext;

	private LabelLine labelLine1;

	private LabelLine labelLine4;

	private TextEditEx txt_SearchBPName;

	private LookUpEditEx txt_SearchBPType;

	private LookUpEditEx txt_SearchBPGroup;

	private SimpleButtonEx btn_Search;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private TextEditEx txt_SearchBPCode;

	private LabelLine labelLine5;

	private LookUpEditEx txt_SearchCurrency;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem reloadToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private LabelControlEx labelControlEx3;

	private ButtonEditEx txt_goToPage;

	private LabelControlEx txt_totalRecord;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			gv_QCBPMD.OptionsSelection.MultiSelect = true;
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.Add:
			gv_QCBPMD.OptionsSelection.MultiSelect = false;
			btn_OK.Text = Resources.ButtonAdd;
			break;
		}
	}

	public frm_QuickCreationBPMasterData()
	{
		InitializeComponent();
		OnLoadAsync += frm_QuickCreationBPMasterData_Load;
	}

	public void UpdatePager()
	{
		try
		{
			labelControlEx1.Text = "Page " + pageNumber + " of " + totalPage;
			txt_pageSize.EditValue = pageSize;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataTable> GetDataPager(int pageNumber, int pageSize, DataTable searchData)
	{
		try
		{
			new DataSet();
			DataSet dataSet = await _httpClient.BusinessMaster.GetDataQuickBPPager(pageNumber, pageSize, searchData);
			DataTable result = dataSet.Tables[0];
			int num = Function.ParseInt(dataSet.Tables[1].Rows[0]["NumRecord"]);
			txt_totalRecord.Text = "Total record: " + num;
			if (num % pageSize != 0)
			{
				totalPage = num / pageSize + 1;
			}
			else
			{
				totalPage = num / pageSize;
			}
			UpdatePager();
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	public void LoadPageSize()
	{
		try
		{
			List<int> dataSource = new List<int> { 10, 20, 50, 100 };
			txt_pageSize.Properties.DataSource = dataSource;
			txt_pageSize.EditValue = 50;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task<DataTable> GetTableQuickBPMasterData()
	{
		try
		{
			new DataTable();
			return await _httpClient.BusinessMaster.GetDataQuickBPMasterData(string.Empty);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBPGroupByBPType(string groupType)
	{
		try
		{
			new DataTable();
			return _httpClient.BusinessMaster.ShowCardGroup(groupType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private async Task LoadData()
	{
		txt_SearchBPCode.EditValue = null;
		txt_SearchBPGroup.EditValue = null;
		txt_SearchBPName.EditValue = null;
		txt_SearchBPType.EditValue = null;
		txt_SearchCurrency.EditValue = null;
		DataTable searchData = TableSearch();
		DataTable dataSource = await GetDataPager(pageNumber, pageSize, searchData);
		grd_QCBPMD.DataSource = dataSource;
		LoadPageSize();
		LoadBPType();
		LoadBPGroup("A");
		LoadCurrency();
		LoadPaymentTerm();
		await LoadSaleEmployee();
		EnableDisableButtonPager();
		lstRowChanged = new List<BPImpls>();
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadSaleEmployee()
	{
		try
		{
			DataTable dataTable = await _httpClient.Commons.GetSaleEmployee();
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["SlpCode"]) == -10))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["SlpCode"] = -10;
				dataRow["SlpName"] = "Define New";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["SlpCode"]) == -1))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["SlpCode"] = -1;
				dataRow2["SlpName"] = "";
				dataTable.Rows.InsertAt(dataRow2, 0);
				dataTable.AcceptChanges();
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				txt_SaleEmployee.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_QuickCreationBPMasterData_Load(object sender, EventArgs e)
	{
		try
		{
			gv_QCBPMD.ShowLoadingPanel();
			await LoadData();
			gv_QCBPMD.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBPGroupSearch(string cardType)
	{
		try
		{
			DataTable bPGroupByBPType = GetBPGroupByBPType(cardType);
			DataRow dataRow = bPGroupByBPType.NewRow();
			dataRow[0] = DBNull.Value;
			object value = (dataRow[2] = "");
			dataRow[1] = value;
			bPGroupByBPType.Rows.InsertAt(dataRow, 0);
			bPGroupByBPType.AcceptChanges();
			txt_SearchBPGroup.Properties.DataSource = bPGroupByBPType;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBPGroup(string cardType)
	{
		try
		{
			DataTable bPGroupByBPType = GetBPGroupByBPType(cardType);
			txt_BPGroup1.DataSource = bPGroupByBPType;
			LoadBPGroupSearch(cardType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCurrency()
	{
		try
		{
			lstCurrency = GlobalData.Currencies;
			if (!lstCurrency.Exists((Common.Table_Code x) => x.Code == "##"))
			{
				lstCurrency.Add(new Common.Table_Code
				{
					Code = "##",
					Name = "All Currencies"
				});
			}
			if (!lstCurrency.Exists((Common.Table_Code x) => x.Code == "-1"))
			{
				lstCurrency.Add(new Common.Table_Code
				{
					Code = "-1",
					Name = "Define New"
				});
			}
			txt_Currency.DataSource = lstCurrency;
			List<Common.Table_Code> list = lstCurrency.ToList();
			list.Insert(0, new Common.Table_Code
			{
				Code = "",
				Name = ""
			});
			txt_SearchCurrency.Properties.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadBPType()
	{
		try
		{
			List<Tuple<string, string>> dataSource = new List<Tuple<string, string>>
			{
				Tuple.Create("L", "Lead"),
				Tuple.Create("C", "Customer"),
				Tuple.Create("S", "Vendor")
			};
			txt_BPType.DataSource = dataSource;
			List<Tuple<string, string>> dataSource2 = new List<Tuple<string, string>>
			{
				Tuple.Create("", ""),
				Tuple.Create("L", "Lead"),
				Tuple.Create("C", "Customer"),
				Tuple.Create("S", "Vendor")
			};
			txt_SearchBPType.Properties.DataSource = dataSource2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadPaymentTerm()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.ShowPaymentTerm();
			firstPaymentTermValue = Function.ParseInt(Function.ToString(dataTable.Rows[0][1]));
			txt_PaymentTerm.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		DataTable dataSourceTable = grd_QCBPMD.DataSourceTable;
		DataRow dataRow = dataSourceTable.NewRow();
		dataRow["CardType"] = "C";
		DataTable bPGroupByBPType = GetBPGroupByBPType("C");
		dataRow["GroupCode"] = Function.ToString(bPGroupByBPType.Rows[0][0]);
		dataRow["Currency"] = "##";
		dataRow["GroupNum"] = firstPaymentTermValue;
		dataRow["SlpCode"] = -1;
		DataTable dataTable = dataSourceTable.Copy().Clone();
		dataTable.Rows.Add(dataRow.ItemArray);
		dataTable.AcceptChanges();
		grd_QCBPMD.DataSource = dataTable;
		pageNumber = 1;
		totalPage = 1;
		EnableDisableButtonPager();
		UpdatePager();
	}

	private bool CheckExistDoc(string cardCode)
	{
		try
		{
			return _httpClient.BusinessMaster.CheckDoc(cardCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private async Task<BPImpls> TableToEntity(DataRow dr)
	{
		BPImpls ret = new BPImpls();
		try
		{
			ret = Function.DataRowToEntity<BPImpls>(dr);
			List<UserFieldsImpl> list = await WinFuntions.GetUdfFields("OCRD");
			if (list != null && list.Count > 0)
			{
				ret.ListUserFields = new List<UserFieldsImpl>();
				foreach (UserFieldsImpl item2 in list)
				{
					UserFieldsImpl item = new UserFieldsImpl
					{
						Name = item2.Name,
						Value = dr[item2.Name],
						Type = item2.Type,
						DefaultValue = item2.DefaultValue
					};
					ret.ListUserFields.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return ret;
	}

	private async Task<List<BPImpls>> GetListBPImpls()
	{
		List<BPImpls> lst = new List<BPImpls>();
		try
		{
			if (grd_QCBPMD.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				if (FormMode == Enums.FormViewModeType.Update)
				{
					return lstRowChanged;
				}
				foreach (DataRow row in dataTable.Rows)
				{
					if (!string.IsNullOrWhiteSpace(Function.ToString(row["CardCode"])))
					{
						List<BPImpls> list = lst;
						list.Add(await TableToEntity(row));
					}
				}
			}
			return lst;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private async Task AddBMDSap()
	{
		_ = 1;
		try
		{
			List<BPImpls> list = await GetListBPImpls();
			string text = "Error at ";
			bool flag = false;
			foreach (BPImpls item in list)
			{
				string text2 = _httpClient.BusinessMaster.SaveDoc(item);
				if (!string.IsNullOrEmpty(text2))
				{
					flag = true;
					text = text + "CardCode " + item.CardCode + ": " + text2 + Environment.NewLine;
				}
			}
			if (flag)
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			gc_BPCode.OptionsColumn.AllowEdit = false;
			await LoadingGridPager();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task UpdateRowWhenSaveSuccess(string cardCode)
	{
		try
		{
			int rowHandle = gv_QCBPMD.FocusedRowHandle;
			DataTable dt = grd_QCBPMD.DataSourceTable;
			DataRow dataRow = (await _httpClient.BusinessMaster.GetDataQuickBPMasterData(cardCode)).Rows[0];
			dt.Rows[rowHandle].ItemArray = dataRow.ItemArray;
			dt.AcceptChanges();
			grd_QCBPMD.DataSource = dt;
			grd_QCBPMD.RefreshDataSource();
			if (gv_QCBPMD.RowCount == 0)
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

	private bool IsExitsNullBPCode()
	{
		try
		{
			for (int i = 0; i < gv_QCBPMD.RowCount; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(gv_QCBPMD.GetRowCellValue(i, gc_BPCode))))
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void OrderRowWithNullBPCode()
	{
		try
		{
			DataTable dataTable = grd_QCBPMD.DataSourceTable.Copy().Clone();
			int num = 0;
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < gv_QCBPMD.RowCount; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(gv_QCBPMD.GetRowCellValue(i, gc_BPCode))))
				{
					list.Add(gv_QCBPMD.GetDataRow(i));
					num++;
				}
				else
				{
					dataTable.Rows.Add(gv_QCBPMD.GetDataRow(i).ItemArray);
				}
			}
			for (int j = 0; j < num; j++)
			{
				dataTable.Rows.Add(list[j].ItemArray);
			}
			dataTable.AcceptChanges();
			grd_QCBPMD.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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

	private void txt_BPGroupEditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string bpType = Function.ToString(gv_QCBPMD.GetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPType));
			short? groupCode = Function.ParseShort(gv_QCBPMD.GetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPGroup));
			if (Function.ParseShort(e.NewValue) == -1)
			{
				VirFormCustomerVendorGroup virFormCustomerVendorGroup = null;
				if (bpType.ToUpper() == "S")
				{
					virFormCustomerVendorGroup = new frm_VendorGroup();
				}
				else if (bpType.ToUpper() == "C" || bpType.ToUpper() == "L")
				{
					virFormCustomerVendorGroup = new frm_CustomerGroup();
				}
				if (virFormCustomerVendorGroup != null)
				{
					virFormCustomerVendorGroup.IsOpenFromBP = true;
					virFormCustomerVendorGroup.ShowDialog(this);
					DataTable dataSourceTable = grd_QCBPMD.DataSourceTable;
					if (virFormCustomerVendorGroup.NewValue != 0)
					{
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["GroupCode"] = virFormCustomerVendorGroup.NewValue;
					}
					else
					{
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["GroupCode"] = groupCode;
					}
					LoadBPGroup("A");
				}
			}
			if (isLoadData)
			{
				DataRow dataRow = _httpClient.BusinessMaster.GetBPGroup().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["GroupType"]) == bpType && Function.ParseInt(t["GroupCode"]) == groupCode);
				if (dataRow != null)
				{
					DataTable dataSourceTable2 = grd_QCBPMD.DataSourceTable;
					dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["DiscRel"] = (string.IsNullOrEmpty(Function.ToString(dataRow["DiscRel"])) ? ((IConvertible)DBNull.Value) : ((IConvertible)Function.ToString(dataRow["DiscRel"])));
					dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["ListNum"] = (string.IsNullOrEmpty(Function.ToString(dataRow["PriceList"])) ? DBNull.Value : ((object)Function.ParseShort(dataRow["PriceList"])));
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmReplacePriceListAndEffective, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataRow dataRow2 = _httpClient.BusinessMaster.GetBPGroup().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["GroupType"]) == bpType && Function.ParseInt(t["GroupCode"]) == groupCode);
				if (dataRow2 != null)
				{
					DataTable dataSourceTable3 = grd_QCBPMD.DataSourceTable;
					dataSourceTable3.Rows[gv_QCBPMD.FocusedRowHandle]["DiscRel"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["DiscRel"])) ? ((IConvertible)DBNull.Value) : ((IConvertible)Function.ToString(dataRow2["DiscRel"])));
					dataSourceTable3.Rows[gv_QCBPMD.FocusedRowHandle]["ListNum"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["PriceList"])) ? DBNull.Value : ((object)Function.ParseShort(dataRow2["PriceList"])));
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReloadBPGroupWhenChangedBPType(object val)
	{
		try
		{
			DataTable bPGroupByBPType = GetBPGroupByBPType(Function.ToString(val));
			gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPGroup, Function.ToString(bPGroupByBPType.Rows[0][0]));
			gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPType, Function.ToString(val));
			gv_QCBPMD.RefreshRowCell(gv_QCBPMD.FocusedRowHandle, gc_BPGroup);
			gv_QCBPMD.RefreshRowCell(gv_QCBPMD.FocusedRowHandle, gc_BPType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BPType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			ReloadBPGroupWhenChangedBPType(e.NewValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_QCBPMD_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column == gc_BPType)
			{
				ReloadBPGroupWhenChangedBPType(e.Value);
			}
			if (e.Column == gc_Currency && Function.ParseShort(e.Value) == -1)
			{
				Function.ToString(gv_QCBPMD.GetRowCellValue(e.RowHandle, gc_Currency));
				new frm_Currencies().ShowDialog(this);
				LoadCurrency();
				gv_QCBPMD.SetRowCellValue(e.RowHandle, gc_Currency, "##");
			}
			if (e.Column == gc_SalesEmployee && Function.ParseInt(Function.ToString(e.Value)) == -10)
			{
				frm_SalesEmployeeBuyers frm_SalesEmployeeBuyers2 = new frm_SalesEmployeeBuyers();
				frm_SalesEmployeeBuyers2.MaximizeBox = false;
				frm_SalesEmployeeBuyers2.ShowDialog(this);
				await LoadSaleEmployee();
				gv_QCBPMD.SetRowCellValue(e.RowHandle, gc_SalesEmployee, -1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		try
		{
			WinCommons._OpenProcessing();
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				if (gv_QCBPMD.RowCount == 1 && string.IsNullOrEmpty(Function.ToString(gv_QCBPMD.GetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPCode))))
				{
					WinCommons.Error(Enums.MessageCode.BpIsMissing, "");
					return;
				}
				if (CheckDuplicateBPCodeInGrid().Item1)
				{
					WinCommons.Error(Enums.MessageCode.None, "Exists BP Code is duplicate: " + CheckDuplicateBPCodeInGrid().Item2[0]);
					return;
				}
				await AddBMDSap();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			await Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private Tuple<bool, List<string>> CheckDuplicateBPCodeInGrid()
	{
		try
		{
			List<string> list = (from dr in grd_QCBPMD.DataSourceTable.AsEnumerable()
				select dr.Field<string>("CardCode") into x
				where !string.IsNullOrEmpty(Function.ToString(x))
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			return Tuple.Create(list.Count > 0, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return null;
		}
	}

	private async Task PasteFromExcelToDataTable(GridControl grd, GridView gv, List<string> key)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
			if (!dataObject.GetDataPresent(DataFormats.Text))
			{
				return;
			}
			DataTable dataTable = Function.PasteFromExcel(clipboardData);
			if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Columns.Count > 0)
			{
				int num = gv.FocusedRowHandle;
				GridColumn focusedColumn = gv.FocusedColumn;
				if (key.Contains(focusedColumn.FieldName))
				{
					int focusedRowHandle = gv.FocusedRowHandle;
					for (int num2 = dataTable.Rows.Count - 1; num2 >= 0; num2--)
					{
						DataRow row = grd.DataSourceTable().NewRow();
						grd.DataSourceTable().Rows.InsertAt(row, focusedRowHandle);
						grd.DataSourceTable().AcceptChanges();
						int colunmIndex = focusedColumn.VisibleIndex;
						for (int i = 0; i < dataTable.Columns.Count; i++)
						{
							gv.FocusedRowHandle = focusedRowHandle;
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
							if (gridColumn == null)
							{
								break;
							}
							gv.SetRowCellValue(num, gridColumn, dataTable.Rows[num2][i]);
							colunmIndex++;
						}
					}
				}
				else
				{
					int focusedRowHandle2 = gv.FocusedRowHandle;
					for (int num3 = focusedRowHandle2; num3 < gv.RowCount; num3++)
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
						if (!flag || num3 - focusedRowHandle2 >= dataTable.Rows.Count)
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
							gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num3 - focusedRowHandle2][num5]);
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
				}
				Clipboard.Clear();
				grd.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.TheTargetTableCannotAddNewRowAutomaticallyContentBelowRowNotCopied, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async void gv_QCBPMD_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gv_QCBPMD.GetRowCellValue(e.RowHandle, gc_BPCode))))
			{
				BPImpls bPImpls = await TableToEntity(gv_QCBPMD.GetDataRow(e.RowHandle));
				bPImpls.OldCardCode = Function.ToString(gv_QCBPMD.GetDataRow(e.RowHandle)["CardCode"]);
				if (lstRowChanged.Any((BPImpls x) => x.CardCode == Function.ToString(gv_QCBPMD.GetRowCellValue(e.RowHandle, gc_BPCode))))
				{
					lstRowChanged.RemoveAll((BPImpls x) => x.CardCode == Function.ToString(gv_QCBPMD.GetRowCellValue(e.RowHandle, gc_BPCode)));
				}
				lstRowChanged.Add(bPImpls);
			}
			if (e.Column == gc_BPCode)
			{
				Tuple<bool, List<string>> tuple = CheckDuplicateBPCodeInGrid();
				if (tuple.Item1 && gv_QCBPMD.FocusedColumn == gc_BPCode && tuple.Item2.Any((string x) => x == Function.ToString(gv_QCBPMD.GetFocusedRowCellValue(gc_BPCode))))
				{
					WinCommons.Error(Enums.MessageCode.None, "Exists BP Code is duplicate: " + Function.ToString(tuple.Item2[0]));
					gv_QCBPMD.SetRowCellValue(e.RowHandle, e.Column, string.Empty);
					gc_BPCode.OptionsColumn.AllowEdit = true;
					gv_QCBPMD.ShowEditor();
				}
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

	private void txt_PaymentTerm_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (isLoadData)
			{
				int num = Function.ParseInt(Function.ToString(e.NewValue));
				if (num != 0)
				{
					DataTable dataSourceTable = grd_QCBPMD.DataSourceTable;
					DataTable paymentTerm = _httpClient.BusinessMaster.GetPaymentTerm(num);
					if (paymentTerm != null && paymentTerm.Rows.Count > 0)
					{
						DataRow dataRow = paymentTerm.Rows[0];
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["IntrstRate"] = (string.IsNullOrEmpty(Function.ToString(dataRow["LatePyChrg"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow["LatePyChrg"])));
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["ListNum"] = (string.IsNullOrEmpty(Function.ToString(dataRow["ListNum"])) ? DBNull.Value : ((object)Function.ParseShort(dataRow["ListNum"])));
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["Discount"] = (string.IsNullOrEmpty(Function.ToString(dataRow["VolumDscnt"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow["VolumDscnt"])));
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["CreditLine"] = (string.IsNullOrEmpty(Function.ToString(dataRow["CredLimit"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow["CredLimit"])));
						dataSourceTable.Rows[gv_QCBPMD.FocusedRowHandle]["DebtLine"] = (string.IsNullOrEmpty(Function.ToString(dataRow["ObligLimit"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow["ObligLimit"])));
					}
					grd_QCBPMD.DataSource = dataSourceTable;
					grd_QCBPMD.RefreshDataSource();
				}
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmApplyPaymentTerm2Bp, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int num2 = Function.ParseInt(Function.ToString(e.NewValue));
				if (num2 != 0)
				{
					DataTable dataSourceTable2 = grd_QCBPMD.DataSourceTable;
					DataTable paymentTerm2 = _httpClient.BusinessMaster.GetPaymentTerm(num2);
					if (paymentTerm2 != null && paymentTerm2.Rows.Count > 0)
					{
						DataRow dataRow2 = paymentTerm2.Rows[0];
						dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["IntrstRate"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["LatePyChrg"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow2["LatePyChrg"])));
						dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["ListNum"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["ListNum"])) ? DBNull.Value : ((object)Function.ParseShort(dataRow2["ListNum"])));
						dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["Discount"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["VolumDscnt"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow2["VolumDscnt"])));
						dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["CreditLine"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["CredLimit"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow2["CredLimit"])));
						dataSourceTable2.Rows[gv_QCBPMD.FocusedRowHandle]["DebtLine"] = (string.IsNullOrEmpty(Function.ToString(dataRow2["ObligLimit"])) ? DBNull.Value : ((object)Function.ParseDecimal(dataRow2["ObligLimit"])));
					}
					grd_QCBPMD.DataSource = dataSourceTable2;
					grd_QCBPMD.RefreshDataSource();
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_QCBPMD.SelectedRowsCount == 0)
			{
				WinCommons.Warning(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				int[] selectedRows = gv_QCBPMD.GetSelectedRows();
				List<string> lstCardCode = new List<string>();
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					lstCardCode.Add(Function.ToString(gv_QCBPMD.GetRowCellValue(rowHandle, "CardCode")));
				}
				string text = ((selectedRows.Length != 1) ? _httpClient.BusinessMaster.DeleteMulti(lstCardCode) : _httpClient.BusinessMaster.DeleteDoc(Function.ToString(gv_QCBPMD.GetRowCellValue(selectedRows[0], "CardCode"))));
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					lstRowChanged.RemoveAll((BPImpls x) => lstCardCode.Contains(x.CardCode));
					await LoadData();
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
	}

	private async void txt_BPCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!e.Button.IsLeft)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					string cardCode = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
					DataRow dataRow = (await _httpClient.BusinessMaster.GetDataQuickBPMasterData(cardCode)).Rows[0];
					isLoadData = true;
					gv_QCBPMD.BeginUpdate();
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPType, dataRow["CardType"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPCode, dataRow["CardCode"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPGroup, dataRow["GroupCode"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPName, dataRow["CardName"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_BPForeignName, dataRow["CardFName"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_AliasName, dataRow["AliasName"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_CreateLimit, dataRow["CreditLine"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_Currency, dataRow["Currency"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_Email, dataRow["E_Mail"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_PaymentTerm, dataRow["GroupNum"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_Remark, dataRow["Free_Text"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_Tel, dataRow["Phone1"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_Mobile, dataRow["Cellular"]);
					gv_QCBPMD.SetRowCellValue(gv_QCBPMD.FocusedRowHandle, gc_SalesEmployee, dataRow["SlpCode"]);
					gv_QCBPMD.EndUpdate();
					isLoadData = false;
					gv_QCBPMD.RefreshRowCell(gv_QCBPMD.FocusedRowHandle, gc_BPGroup);
				}
				return;
			}
			if (CheckDuplicateBPCodeInGrid().Item1 && CheckDuplicateBPCodeInGrid().Item2.Any((string x) => x == Function.ToString(gv_QCBPMD.GetFocusedRowCellValue(gc_BPCode))))
			{
				WinCommons.Error(Enums.MessageCode.None, "Exists BP Code is duplicate: " + Function.ToString(CheckDuplicateBPCodeInGrid().Item2[0]));
				return;
			}
			WinCommons._OpenProcessing();
			DataRow focusedDataRow = gv_QCBPMD.GetFocusedDataRow();
			DataTable dataTable = focusedDataRow.Table.Clone().Copy();
			dataTable.Rows.Add(focusedDataRow.ItemArray);
			dataTable.AcceptChanges();
			frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData
			{
				dataFromQuickCreationBMD = dataTable,
				isExistsCardCode = CheckExistDoc(Function.ToString(dataTable.Rows[0]["CardCode"]))
			};
			frm_BusinessMasterData2.RefreshQuickCreation = (Action<bool, string>)Delegate.Combine(frm_BusinessMasterData2.RefreshQuickCreation, (Action<bool, string>)((bool ret, string cardCode2) =>
			{
				try
				{
					if (ret)
					{
						UpdateRowWhenSaveSuccess(cardCode2);
					}
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", ex2);
					WinCommons.ErrorLineNumber(ex2);
				}
			}));
			frm_BusinessMasterData2.ShowDialog(this);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_QCBPMD_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			string.IsNullOrEmpty(Function.ToString(gv_QCBPMD.GetRowCellValue(gv_QCBPMD.RowCount - 1, gc_BPCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_QCBPMD_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			if (gv_QCBPMD.FocusedColumn != gc_BPType && gv_QCBPMD.FocusedColumn != gc_BPCode && string.IsNullOrEmpty(Function.ToString(gv_QCBPMD.GetFocusedRowCellValue(gc_BPCode))))
			{
				WinCommons.Warning(Enums.MessageCode.FirstYouShouldEditValueOfBpCodeAddFunctionNotSaveRowNullBpCode, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
			gv_QCBPMD.FocusedRowHandle = gv_QCBPMD.RowCount - 1;
			gc_BPCode.OptionsColumn.AllowEdit = true;
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_QCBPMD_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.Column == gc_BPCode)
		{
			if (CheckExistDoc(Function.ToString(e.CellValue)) && FormMode != Enums.FormViewModeType.Add)
			{
				e.RepositoryItem.ReadOnly = true;
			}
			else
			{
				e.RepositoryItem.ReadOnly = false;
			}
		}
		else if (e.Column == gc_BPGroup)
		{
			DataTable bPGroupByBPType = GetBPGroupByBPType(Function.ToString(gv_QCBPMD.GetRowCellValue(e.RowHandle, gc_BPType)));
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
			repositoryItemLookUpEdit.DisplayMember = "GroupName";
			repositoryItemLookUpEdit.ValueMember = "GroupCode";
			repositoryItemLookUpEdit.DataSource = bPGroupByBPType;
			repositoryItemLookUpEdit.EditValueChanging += txt_BPGroupEditValueChanging;
			e.RepositoryItem = repositoryItemLookUpEdit;
		}
	}

	private void EnableDisableButtonPager()
	{
		if (pageNumber == 1)
		{
			btn_pagerPrevious.Enabled = false;
		}
		else
		{
			btn_pagerPrevious.Enabled = true;
		}
		if (pageNumber == totalPage)
		{
			btn_pagerNext.Enabled = false;
		}
		else
		{
			btn_pagerNext.Enabled = true;
		}
	}

	private async Task LoadingGridPager()
	{
		try
		{
			gv_QCBPMD.ShowLoadingPanel();
			DataTable searchData = TableSearch();
			DataTable dataSource = await GetDataPager(pageNumber, pageSize, searchData);
			grd_QCBPMD.DataSource = dataSource;
			gv_QCBPMD.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task BeforeLeavePage()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && FormMode != 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await Save();
				}
				else
				{
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_pagerPrevious_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await BeforeLeavePage();
			pageNumber--;
			EnableDisableButtonPager();
			await LoadingGridPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_pagerNext_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await BeforeLeavePage();
			pageNumber++;
			EnableDisableButtonPager();
			await LoadingGridPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_pagerFirst_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await BeforeLeavePage();
			pageNumber = 1;
			EnableDisableButtonPager();
			await LoadingGridPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_pagerLast_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await BeforeLeavePage();
			pageNumber = totalPage;
			EnableDisableButtonPager();
			await LoadingGridPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_pageSize_EditValueChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			pageSize = Function.ParseInt(txt_pageSize.EditValue);
			pageNumber = 1;
			await BeforeLeavePage();
			await LoadingGridPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_SearchBPType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			LoadBPGroupSearch(Function.ToString(txt_SearchBPType.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable TableSearch()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("BPCode", typeof(string));
			dataTable.Columns.Add("BPName", typeof(string));
			dataTable.Columns.Add("BPGroup", typeof(string));
			dataTable.Columns.Add("Currency", typeof(string));
			dataTable.Columns.Add("BPType", typeof(string));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["BPCode"] = Function.ToString(txt_SearchBPCode.EditValue);
			dataRow["BPName"] = Function.ToString(txt_SearchBPName.EditValue);
			dataRow["BPGroup"] = Function.ToString(txt_SearchBPGroup.EditValue);
			dataRow["Currency"] = Function.ToString(txt_SearchCurrency.EditValue);
			dataRow["BPType"] = Function.ToString(txt_SearchBPType.EditValue);
			dataTable.Rows.Add(dataRow.ItemArray);
			dataTable.AcceptChanges();
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	public async Task Search(int pageNumber, int pageSize)
	{
		try
		{
			gv_QCBPMD.ShowLoadingPanel();
			DataTable searchData = TableSearch();
			DataTable dataSource = await GetDataPager(pageNumber, pageSize, searchData);
			grd_QCBPMD.DataSource = dataSource;
			grd_QCBPMD.RefreshDataSource();
			gv_QCBPMD.HideLoadingPanel();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await BeforeLeavePage();
			await Search(pageNumber, pageSize);
			EnableDisableButtonPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void reloadToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void buttonEditEx1_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		_ = 1;
		try
		{
			pageNumber = Function.ParseInt(txt_goToPage.EditValue);
			await BeforeLeavePage();
			await LoadingGridPager();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void grd_QCBPMD_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				await PasteFromExcelToDataTable(grd_QCBPMD, gv_QCBPMD, new List<string> { "CardCode" });
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.BusinessMaster.frm_QuickCreationBPMasterData));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_QCBPMD = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_QCBPMD = new Apzon.Controls.Grid.GridViewEx();
		this.gc_BPType = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_BPType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_BPCode = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_BPCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_BPName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BPForeignName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BPGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_BPGroup1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_Currency = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_SalesEmployee = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_SaleEmployee = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_AliasName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_PaymentTerm = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_PaymentTerm = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_CreateLimit = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_CreditLimit = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_Tel = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Email = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Mobile = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_BPGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_pagerNext = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_pagerLast = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_pagerFirst = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_pagerPrevious = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_pageSize = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_SearchBPCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_SearchBPGroup = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_SearchBPType = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_SearchBPName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_SearchCurrency = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.reloadToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.txt_goToPage = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelControlEx3 = new Apzon.Controls.Controls.LabelControlEx();
		this.txt_totalRecord = new Apzon.Controls.Controls.LabelControlEx();
		((System.ComponentModel.ISupportInitialize)this.grd_QCBPMD).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_QCBPMD).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPGroup1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SaleEmployee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PaymentTerm).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CreditLimit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_pageSize.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchCurrency.Properties).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_goToPage.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_QCBPMD.AllowPasteExcel = true;
		this.grd_QCBPMD.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_QCBPMD.ClipboardData = null;
		this.grd_QCBPMD.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_QCBPMD.Location = new System.Drawing.Point(0, 79);
		this.grd_QCBPMD.MainView = this.gv_QCBPMD;
		this.grd_QCBPMD.Name = "grd_QCBPMD";
		this.grd_QCBPMD.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.txt_BPType, this.txt_BPGroup, this.txt_Currency, this.txt_PaymentTerm, this.txt_CreditLimit, this.txt_SaleEmployee, this.txt_BPCode, this.txt_BPGroup1 });
		this.grd_QCBPMD.Size = new System.Drawing.Size(905, 445);
		this.grd_QCBPMD.TabIndex = 0;
		this.grd_QCBPMD.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_QCBPMD });
		this.grd_QCBPMD.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_QCBPMD_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.refreshToolStripMenuItem, this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(145, 70);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.addToolStripMenuItem.Text = "Add new row";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_QCBPMD.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_QCBPMD.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_QCBPMD.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_QCBPMD.ColumnKey = null;
		this.gv_QCBPMD.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gc_BPType, this.gc_BPCode, this.gc_BPName, this.gc_BPForeignName, this.gc_BPGroup, this.gc_Currency, this.gc_SalesEmployee, this.gc_AliasName, this.gc_PaymentTerm, this.gc_CreateLimit,
			this.gc_Tel, this.gc_Email, this.gc_Mobile, this.gc_Remark
		});
		this.gv_QCBPMD.DisableFormSetting = false;
		this.gv_QCBPMD.DisplayException = false;
		this.gv_QCBPMD.EditColumn = null;
		this.gv_QCBPMD.FieldCurrency = null;
		this.gv_QCBPMD.FieldRate = null;
		this.gv_QCBPMD.FormId = "oQuickCreationBusinessPartnerMasterData";
		this.gv_QCBPMD.GridControl = this.grd_QCBPMD;
		this.gv_QCBPMD.IgnoreFilter = false;
		this.gv_QCBPMD.IndexEditRow = -1;
		this.gv_QCBPMD.IndicatorWidth = 35;
		this.gv_QCBPMD.IsApzTable = false;
		this.gv_QCBPMD.IsInsertNewRow = false;
		this.gv_QCBPMD.IsLoadingData = false;
		this.gv_QCBPMD.IsMappingContextAction = false;
		this.gv_QCBPMD.ItemId = "OCRD";
		this.gv_QCBPMD.LstColumnAllowBatch = null;
		this.gv_QCBPMD.LstColumnNotAllowBatch = null;
		this.gv_QCBPMD.LstColumnNotEditable = null;
		this.gv_QCBPMD.LstColumNotSetting = null;
		this.gv_QCBPMD.Name = "gv_QCBPMD";
		this.gv_QCBPMD.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_QCBPMD.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_QCBPMD.OptionsPrint.AutoWidth = false;
		this.gv_QCBPMD.OptionsPrint.UsePrintStyles = false;
		this.gv_QCBPMD.OptionsSelection.MultiSelect = true;
		this.gv_QCBPMD.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_QCBPMD.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = DevExpress.Utils.DefaultBoolean.True;
		this.gv_QCBPMD.OptionsSelection.UseIndicatorForSelection = false;
		this.gv_QCBPMD.OptionsView.BestFitMaxRowCount = 100;
		this.gv_QCBPMD.OptionsView.ColumnAutoWidth = false;
		this.gv_QCBPMD.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_QCBPMD.OptionsView.ShowGroupedColumns = true;
		this.gv_QCBPMD.OptionsView.ShowGroupPanel = false;
		this.gv_QCBPMD.UseBatchMod = false;
		this.gv_QCBPMD.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_QCBPMD_CustomRowCellEditForEditing);
		this.gv_QCBPMD.ShownEditor += new System.EventHandler(this.gv_QCBPMD_ShownEditor);
		this.gv_QCBPMD.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_QCBPMD_CellValueChanged);
		this.gv_QCBPMD.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_QCBPMD_CellValueChanging);
		this.gv_QCBPMD.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_QCBPMD_BeforeLeaveRow);
		this.gc_BPType.Caption = "BP Type";
		this.gc_BPType.ColumnEdit = this.txt_BPType;
		this.gc_BPType.CurrencyDateField = null;
		this.gc_BPType.CurrencyField = null;
		this.gc_BPType.CurrencyFixed = null;
		this.gc_BPType.CurrencyFixedRate = 0.0;
		this.gc_BPType.CurrencyRateField = null;
		this.gc_BPType.FieldName = "CardType";
		this.gc_BPType.Name = "gc_BPType";
		this.gc_BPType.PrimaryKey = false;
		this.gc_BPType.UnitField = null;
		this.gc_BPType.Visible = true;
		this.gc_BPType.VisibleIndex = 1;
		this.txt_BPType.AutoHeight = false;
		this.txt_BPType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_BPType.DisplayMember = "Item2";
		this.txt_BPType.Name = "txt_BPType";
		this.txt_BPType.NullText = "";
		this.txt_BPType.ValueMember = "Item1";
		this.txt_BPType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_BPType_EditValueChanging);
		this.gc_BPCode.Caption = "BP Code";
		this.gc_BPCode.ColumnEdit = this.txt_BPCode;
		this.gc_BPCode.CurrencyDateField = null;
		this.gc_BPCode.CurrencyField = null;
		this.gc_BPCode.CurrencyFixed = null;
		this.gc_BPCode.CurrencyFixedRate = 0.0;
		this.gc_BPCode.CurrencyRateField = null;
		this.gc_BPCode.FieldName = "CardCode";
		this.gc_BPCode.Name = "gc_BPCode";
		this.gc_BPCode.PrimaryKey = true;
		this.gc_BPCode.UnitField = null;
		this.gc_BPCode.Visible = true;
		this.gc_BPCode.VisibleIndex = 2;
		this.txt_BPCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.txt_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_BPCode.ContainerControl = this;
		this.txt_BPCode.DisplayMember = "CardCode";
		this.txt_BPCode.DisplayMember2 = "";
		this.txt_BPCode.LoadDataSourceWhenEnter = false;
		this.txt_BPCode.Name = "txt_BPCode";
		this.txt_BPCode.SeparateCharacterForMulti = ';';
		this.txt_BPCode.SeparateString = ", ";
		this.txt_BPCode.UsingAutoCompleteBox = false;
		this.txt_BPCode.UsingDataSource = true;
		this.txt_BPCode.ValidateWhenNoDataSource = true;
		this.txt_BPCode.ValueMember = "CardCode";
		this.txt_BPCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BPCode_ButtonPressed);
		this.gc_BPName.Caption = "BP Name";
		this.gc_BPName.CurrencyDateField = null;
		this.gc_BPName.CurrencyField = null;
		this.gc_BPName.CurrencyFixed = null;
		this.gc_BPName.CurrencyFixedRate = 0.0;
		this.gc_BPName.CurrencyRateField = null;
		this.gc_BPName.FieldName = "CardName";
		this.gc_BPName.Name = "gc_BPName";
		this.gc_BPName.PrimaryKey = false;
		this.gc_BPName.UnitField = null;
		this.gc_BPName.Visible = true;
		this.gc_BPName.VisibleIndex = 3;
		this.gc_BPForeignName.Caption = "Foreign Name";
		this.gc_BPForeignName.CurrencyDateField = null;
		this.gc_BPForeignName.CurrencyField = null;
		this.gc_BPForeignName.CurrencyFixed = null;
		this.gc_BPForeignName.CurrencyFixedRate = 0.0;
		this.gc_BPForeignName.CurrencyRateField = null;
		this.gc_BPForeignName.FieldName = "CardFName";
		this.gc_BPForeignName.Name = "gc_BPForeignName";
		this.gc_BPForeignName.PrimaryKey = false;
		this.gc_BPForeignName.UnitField = null;
		this.gc_BPForeignName.Visible = true;
		this.gc_BPForeignName.VisibleIndex = 4;
		this.gc_BPGroup.AppearanceCell.Options.UseTextOptions = true;
		this.gc_BPGroup.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.gc_BPGroup.Caption = "BP Group";
		this.gc_BPGroup.ColumnEdit = this.txt_BPGroup1;
		this.gc_BPGroup.CurrencyDateField = null;
		this.gc_BPGroup.CurrencyField = null;
		this.gc_BPGroup.CurrencyFixed = null;
		this.gc_BPGroup.CurrencyFixedRate = 0.0;
		this.gc_BPGroup.CurrencyRateField = null;
		this.gc_BPGroup.FieldName = "GroupCode";
		this.gc_BPGroup.Name = "gc_BPGroup";
		this.gc_BPGroup.PrimaryKey = false;
		this.gc_BPGroup.UnitField = null;
		this.gc_BPGroup.Visible = true;
		this.gc_BPGroup.VisibleIndex = 5;
		this.txt_BPGroup1.AutoHeight = false;
		this.txt_BPGroup1.AutoLoadDataSource = true;
		this.txt_BPGroup1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_BPGroup1.ContainerControl = this;
		this.txt_BPGroup1.DisplayMember = "GroupName";
		this.txt_BPGroup1.DisplayMember2 = "";
		this.txt_BPGroup1.LoadDataSourceWhenEnter = false;
		this.txt_BPGroup1.Name = "txt_BPGroup1";
		this.txt_BPGroup1.SeparateCharacterForMulti = ';';
		this.txt_BPGroup1.SeparateString = ", ";
		this.txt_BPGroup1.UsingAutoCompleteBox = false;
		this.txt_BPGroup1.UsingDataSource = true;
		this.txt_BPGroup1.ValidateWhenNoDataSource = false;
		this.txt_BPGroup1.ValueMember = "GroupCode";
		this.gc_Currency.Caption = "Currency";
		this.gc_Currency.ColumnEdit = this.txt_Currency;
		this.gc_Currency.CurrencyDateField = null;
		this.gc_Currency.CurrencyField = null;
		this.gc_Currency.CurrencyFixed = null;
		this.gc_Currency.CurrencyFixedRate = 0.0;
		this.gc_Currency.CurrencyRateField = null;
		this.gc_Currency.FieldName = "Currency";
		this.gc_Currency.Name = "gc_Currency";
		this.gc_Currency.PrimaryKey = false;
		this.gc_Currency.UnitField = null;
		this.gc_Currency.Visible = true;
		this.gc_Currency.VisibleIndex = 6;
		this.txt_Currency.AutoHeight = false;
		this.txt_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Currency.DisplayMember = "Name";
		this.txt_Currency.Name = "txt_Currency";
		this.txt_Currency.NullText = "";
		this.txt_Currency.ValueMember = "Code";
		this.gc_SalesEmployee.Caption = "Sales Employee";
		this.gc_SalesEmployee.ColumnEdit = this.txt_SaleEmployee;
		this.gc_SalesEmployee.CurrencyDateField = null;
		this.gc_SalesEmployee.CurrencyField = null;
		this.gc_SalesEmployee.CurrencyFixed = null;
		this.gc_SalesEmployee.CurrencyFixedRate = 0.0;
		this.gc_SalesEmployee.CurrencyRateField = null;
		this.gc_SalesEmployee.FieldName = "SlpCode";
		this.gc_SalesEmployee.Name = "gc_SalesEmployee";
		this.gc_SalesEmployee.PrimaryKey = false;
		this.gc_SalesEmployee.UnitField = null;
		this.gc_SalesEmployee.Visible = true;
		this.gc_SalesEmployee.VisibleIndex = 7;
		this.txt_SaleEmployee.AutoHeight = false;
		this.txt_SaleEmployee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_SaleEmployee.DisplayMember = "SlpName";
		this.txt_SaleEmployee.Name = "txt_SaleEmployee";
		this.txt_SaleEmployee.NullText = "";
		this.txt_SaleEmployee.ValueMember = "SlpCode";
		this.gc_AliasName.Caption = "Alias Name Namnx";
		this.gc_AliasName.CurrencyDateField = null;
		this.gc_AliasName.CurrencyField = null;
		this.gc_AliasName.CurrencyFixed = null;
		this.gc_AliasName.CurrencyFixedRate = 0.0;
		this.gc_AliasName.CurrencyRateField = null;
		this.gc_AliasName.FieldName = "AliasName";
		this.gc_AliasName.Name = "gc_AliasName";
		this.gc_AliasName.PrimaryKey = false;
		this.gc_AliasName.UnitField = null;
		this.gc_AliasName.Visible = true;
		this.gc_AliasName.VisibleIndex = 8;
		this.gc_PaymentTerm.Caption = "Payment Term";
		this.gc_PaymentTerm.ColumnEdit = this.txt_PaymentTerm;
		this.gc_PaymentTerm.CurrencyDateField = null;
		this.gc_PaymentTerm.CurrencyField = null;
		this.gc_PaymentTerm.CurrencyFixed = null;
		this.gc_PaymentTerm.CurrencyFixedRate = 0.0;
		this.gc_PaymentTerm.CurrencyRateField = null;
		this.gc_PaymentTerm.FieldName = "GroupNum";
		this.gc_PaymentTerm.Name = "gc_PaymentTerm";
		this.gc_PaymentTerm.PrimaryKey = false;
		this.gc_PaymentTerm.UnitField = null;
		this.gc_PaymentTerm.Visible = true;
		this.gc_PaymentTerm.VisibleIndex = 9;
		this.txt_PaymentTerm.AutoHeight = false;
		this.txt_PaymentTerm.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_PaymentTerm.DisplayMember = "PymntGroup";
		this.txt_PaymentTerm.Name = "txt_PaymentTerm";
		this.txt_PaymentTerm.NullText = "";
		this.txt_PaymentTerm.ValueMember = "GroupNum";
		this.txt_PaymentTerm.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.txt_PaymentTerm_EditValueChanging);
		this.gc_CreateLimit.Caption = "Credit Limit";
		this.gc_CreateLimit.ColumnEdit = this.txt_CreditLimit;
		this.gc_CreateLimit.CurrencyDateField = null;
		this.gc_CreateLimit.CurrencyField = null;
		this.gc_CreateLimit.CurrencyFixed = null;
		this.gc_CreateLimit.CurrencyFixedRate = 0.0;
		this.gc_CreateLimit.CurrencyRateField = null;
		this.gc_CreateLimit.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gc_CreateLimit.FieldName = "CreditLine";
		this.gc_CreateLimit.Name = "gc_CreateLimit";
		this.gc_CreateLimit.PrimaryKey = false;
		this.gc_CreateLimit.UnitField = null;
		this.gc_CreateLimit.Visible = true;
		this.gc_CreateLimit.VisibleIndex = 10;
		this.txt_CreditLimit.AutoHeight = false;
		this.txt_CreditLimit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_CreditLimit.Name = "txt_CreditLimit";
		this.gc_Tel.Caption = "Tel";
		this.gc_Tel.CurrencyDateField = null;
		this.gc_Tel.CurrencyField = null;
		this.gc_Tel.CurrencyFixed = null;
		this.gc_Tel.CurrencyFixedRate = 0.0;
		this.gc_Tel.CurrencyRateField = null;
		this.gc_Tel.FieldName = "Phone1";
		this.gc_Tel.Name = "gc_Tel";
		this.gc_Tel.PrimaryKey = false;
		this.gc_Tel.UnitField = null;
		this.gc_Tel.Visible = true;
		this.gc_Tel.VisibleIndex = 11;
		this.gc_Email.Caption = "Email";
		this.gc_Email.CurrencyDateField = null;
		this.gc_Email.CurrencyField = null;
		this.gc_Email.CurrencyFixed = null;
		this.gc_Email.CurrencyFixedRate = 0.0;
		this.gc_Email.CurrencyRateField = null;
		this.gc_Email.FieldName = "E_Mail";
		this.gc_Email.Name = "gc_Email";
		this.gc_Email.PrimaryKey = false;
		this.gc_Email.UnitField = null;
		this.gc_Email.Visible = true;
		this.gc_Email.VisibleIndex = 12;
		this.gc_Mobile.Caption = "Mobile";
		this.gc_Mobile.CurrencyDateField = null;
		this.gc_Mobile.CurrencyField = null;
		this.gc_Mobile.CurrencyFixed = null;
		this.gc_Mobile.CurrencyFixedRate = 0.0;
		this.gc_Mobile.CurrencyRateField = null;
		this.gc_Mobile.FieldName = "Cellular";
		this.gc_Mobile.Name = "gc_Mobile";
		this.gc_Mobile.PrimaryKey = false;
		this.gc_Mobile.UnitField = null;
		this.gc_Mobile.Visible = true;
		this.gc_Mobile.VisibleIndex = 13;
		this.gc_Remark.Caption = "Remark";
		this.gc_Remark.CurrencyDateField = null;
		this.gc_Remark.CurrencyField = null;
		this.gc_Remark.CurrencyFixed = null;
		this.gc_Remark.CurrencyFixedRate = 0.0;
		this.gc_Remark.CurrencyRateField = null;
		this.gc_Remark.FieldName = "Free_Text";
		this.gc_Remark.Name = "gc_Remark";
		this.gc_Remark.PrimaryKey = false;
		this.gc_Remark.UnitField = null;
		this.gc_Remark.Visible = true;
		this.gc_Remark.VisibleIndex = 14;
		this.txt_BPGroup.AutoHeight = false;
		this.txt_BPGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_BPGroup.DisplayMember = "GroupName";
		this.txt_BPGroup.Name = "txt_BPGroup";
		this.txt_BPGroup.NullText = "";
		this.txt_BPGroup.ValueMember = "GroupCode";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 541);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 13;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 541);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 14;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_pagerNext.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_pagerNext.EditControl = null;
		this.btn_pagerNext.EditValue = "";
		this.btn_pagerNext.FieldName = null;
		this.btn_pagerNext.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_pagerNext.ImageOptions.Image");
		this.btn_pagerNext.IsSearchControl = true;
		this.btn_pagerNext.Location = new System.Drawing.Point(832, 541);
		this.btn_pagerNext.Name = "btn_pagerNext";
		this.btn_pagerNext.ReadOnly = false;
		this.btn_pagerNext.SettingInVisible = false;
		this.btn_pagerNext.SettingReadonly = false;
		this.btn_pagerNext.Size = new System.Drawing.Size(27, 23);
		this.btn_pagerNext.TabIndex = 16;
		this.btn_pagerNext.Click += new System.EventHandler(this.btn_pagerNext_Click);
		this.btn_pagerLast.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_pagerLast.EditControl = null;
		this.btn_pagerLast.EditValue = "";
		this.btn_pagerLast.FieldName = null;
		this.btn_pagerLast.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_pagerLast.ImageOptions.Image");
		this.btn_pagerLast.IsSearchControl = true;
		this.btn_pagerLast.Location = new System.Drawing.Point(865, 541);
		this.btn_pagerLast.Name = "btn_pagerLast";
		this.btn_pagerLast.ReadOnly = false;
		this.btn_pagerLast.SettingInVisible = false;
		this.btn_pagerLast.SettingReadonly = false;
		this.btn_pagerLast.Size = new System.Drawing.Size(27, 23);
		this.btn_pagerLast.TabIndex = 16;
		this.btn_pagerLast.Click += new System.EventHandler(this.btn_pagerLast_Click);
		this.btn_pagerFirst.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_pagerFirst.EditControl = null;
		this.btn_pagerFirst.EditValue = "";
		this.btn_pagerFirst.FieldName = null;
		this.btn_pagerFirst.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_pagerFirst.ImageOptions.Image");
		this.btn_pagerFirst.IsSearchControl = true;
		this.btn_pagerFirst.Location = new System.Drawing.Point(682, 541);
		this.btn_pagerFirst.Name = "btn_pagerFirst";
		this.btn_pagerFirst.ReadOnly = false;
		this.btn_pagerFirst.SettingInVisible = false;
		this.btn_pagerFirst.SettingReadonly = false;
		this.btn_pagerFirst.Size = new System.Drawing.Size(27, 23);
		this.btn_pagerFirst.TabIndex = 16;
		this.btn_pagerFirst.Click += new System.EventHandler(this.btn_pagerFirst_Click);
		this.btn_pagerPrevious.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_pagerPrevious.EditControl = null;
		this.btn_pagerPrevious.EditValue = "";
		this.btn_pagerPrevious.Enabled = false;
		this.btn_pagerPrevious.FieldName = null;
		this.btn_pagerPrevious.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_pagerPrevious.ImageOptions.Image");
		this.btn_pagerPrevious.IsSearchControl = true;
		this.btn_pagerPrevious.Location = new System.Drawing.Point(715, 541);
		this.btn_pagerPrevious.Name = "btn_pagerPrevious";
		this.btn_pagerPrevious.ReadOnly = false;
		this.btn_pagerPrevious.SettingInVisible = false;
		this.btn_pagerPrevious.SettingReadonly = false;
		this.btn_pagerPrevious.Size = new System.Drawing.Size(27, 23);
		this.btn_pagerPrevious.TabIndex = 16;
		this.btn_pagerPrevious.Click += new System.EventHandler(this.btn_pagerPrevious_Click);
		this.labelControlEx1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelControlEx1.EditControl = null;
		this.labelControlEx1.EditValue = "Page 1 of 100";
		this.labelControlEx1.FieldName = null;
		this.labelControlEx1.Location = new System.Drawing.Point(748, 546);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.ReadOnly = false;
		this.labelControlEx1.SettingInVisible = false;
		this.labelControlEx1.SettingReadonly = false;
		this.labelControlEx1.Size = new System.Drawing.Size(67, 13);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 17;
		this.labelControlEx1.Text = "Page 1 of 100";
		this.txt_pageSize.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txt_pageSize.EditControl = null;
		this.txt_pageSize.FieldName = null;
		this.txt_pageSize.IsSearchControl = true;
		this.txt_pageSize.Location = new System.Drawing.Point(632, 543);
		this.txt_pageSize.Name = "txt_pageSize";
		this.txt_pageSize.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_pageSize.Properties.NullText = "";
		this.txt_pageSize.SettingInVisible = false;
		this.txt_pageSize.SettingReadonly = false;
		this.txt_pageSize.Size = new System.Drawing.Size(44, 20);
		this.txt_pageSize.TabIndex = 18;
		this.txt_pageSize.EditValueChanged += new System.EventHandler(this.txt_pageSize_EditValueChanged);
		this.labelControlEx2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelControlEx2.EditControl = null;
		this.labelControlEx2.EditValue = "Page size";
		this.labelControlEx2.FieldName = null;
		this.labelControlEx2.Location = new System.Drawing.Point(581, 546);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.ReadOnly = false;
		this.labelControlEx2.SettingInVisible = false;
		this.labelControlEx2.SettingReadonly = false;
		this.labelControlEx2.Size = new System.Drawing.Size(45, 13);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 19;
		this.labelControlEx2.Text = "Page size";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "BP Type";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(11, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(76, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 21;
		this.labelLine1.Utext = "BP Type";
		this.txt_SearchBPCode.EditControl = null;
		this.txt_SearchBPCode.FieldName = null;
		this.txt_SearchBPCode.IsSearchControl = true;
		this.txt_SearchBPCode.Location = new System.Drawing.Point(281, 15);
		this.txt_SearchBPCode.Name = "txt_SearchBPCode";
		this.txt_SearchBPCode.SettingInVisible = false;
		this.txt_SearchBPCode.SettingReadonly = false;
		this.txt_SearchBPCode.Size = new System.Drawing.Size(100, 20);
		this.txt_SearchBPCode.TabIndex = 20;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(199, 15);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(76, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 21;
		this.labelLine2.Utext = "BP Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "BP Name";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(11, 52);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(76, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 21;
		this.labelLine3.Utext = "BP Name";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(575, 50);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(73, 23);
		this.btn_Search.TabIndex = 22;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.txt_SearchBPGroup.EditControl = null;
		this.txt_SearchBPGroup.FieldName = null;
		this.txt_SearchBPGroup.IsSearchControl = true;
		this.txt_SearchBPGroup.Location = new System.Drawing.Point(281, 52);
		this.txt_SearchBPGroup.Name = "txt_SearchBPGroup";
		this.txt_SearchBPGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_SearchBPGroup.Properties.DisplayMember = "GroupName";
		this.txt_SearchBPGroup.Properties.NullText = "";
		this.txt_SearchBPGroup.Properties.ValueMember = "GroupCode";
		this.txt_SearchBPGroup.SettingInVisible = false;
		this.txt_SearchBPGroup.SettingReadonly = false;
		this.txt_SearchBPGroup.Size = new System.Drawing.Size(100, 20);
		this.txt_SearchBPGroup.TabIndex = 23;
		this.txt_SearchBPType.EditControl = null;
		this.txt_SearchBPType.FieldName = null;
		this.txt_SearchBPType.IsSearchControl = true;
		this.txt_SearchBPType.Location = new System.Drawing.Point(93, 15);
		this.txt_SearchBPType.Name = "txt_SearchBPType";
		this.txt_SearchBPType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_SearchBPType.Properties.DisplayMember = "Item2";
		this.txt_SearchBPType.Properties.NullText = "";
		this.txt_SearchBPType.Properties.ValueMember = "Item1";
		this.txt_SearchBPType.SettingInVisible = false;
		this.txt_SearchBPType.SettingReadonly = false;
		this.txt_SearchBPType.Size = new System.Drawing.Size(100, 20);
		this.txt_SearchBPType.TabIndex = 24;
		this.txt_SearchBPType.EditValueChanged += new System.EventHandler(this.txt_SearchBPType_EditValueChanged);
		this.txt_SearchBPName.EditControl = null;
		this.txt_SearchBPName.FieldName = null;
		this.txt_SearchBPName.IsSearchControl = true;
		this.txt_SearchBPName.Location = new System.Drawing.Point(93, 52);
		this.txt_SearchBPName.Name = "txt_SearchBPName";
		this.txt_SearchBPName.SettingInVisible = false;
		this.txt_SearchBPName.SettingReadonly = false;
		this.txt_SearchBPName.Size = new System.Drawing.Size(100, 20);
		this.txt_SearchBPName.TabIndex = 25;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "BP group";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(199, 52);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(76, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 26;
		this.labelLine4.Utext = "BP group";
		this.txt_SearchCurrency.EditControl = null;
		this.txt_SearchCurrency.FieldName = null;
		this.txt_SearchCurrency.IsSearchControl = true;
		this.txt_SearchCurrency.Location = new System.Drawing.Point(469, 52);
		this.txt_SearchCurrency.Name = "txt_SearchCurrency";
		this.txt_SearchCurrency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_SearchCurrency.Properties.DisplayMember = "Name";
		this.txt_SearchCurrency.Properties.NullText = "";
		this.txt_SearchCurrency.Properties.ValueMember = "Code";
		this.txt_SearchCurrency.SettingInVisible = false;
		this.txt_SearchCurrency.SettingReadonly = false;
		this.txt_SearchCurrency.Size = new System.Drawing.Size(100, 20);
		this.txt_SearchCurrency.TabIndex = 23;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Currency";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(387, 52);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(76, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 26;
		this.labelLine5.Utext = "Currency";
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.reloadToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(114, 26);
		this.reloadToolStripMenuItem.Name = "reloadToolStripMenuItem";
		this.reloadToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.reloadToolStripMenuItem.Text = "Refresh";
		this.reloadToolStripMenuItem.Click += new System.EventHandler(this.reloadToolStripMenuItem_Click);
		this.txt_goToPage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txt_goToPage.EditControl = null;
		this.txt_goToPage.FieldName = null;
		this.txt_goToPage.IsSearchControl = false;
		this.txt_goToPage.Location = new System.Drawing.Point(518, 544);
		this.txt_goToPage.Name = "txt_goToPage";
		this.txt_goToPage.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.OK)
		});
		this.txt_goToPage.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_goToPage.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.txt_goToPage.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_goToPage.SettingInVisible = false;
		this.txt_goToPage.SettingReadonly = false;
		this.txt_goToPage.Size = new System.Drawing.Size(51, 20);
		this.txt_goToPage.TabIndex = 27;
		this.txt_goToPage.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEditEx1_ButtonPressed);
		this.labelControlEx3.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.labelControlEx3.EditControl = null;
		this.labelControlEx3.EditValue = "Go to page";
		this.labelControlEx3.FieldName = null;
		this.labelControlEx3.Location = new System.Drawing.Point(459, 546);
		this.labelControlEx3.Name = "labelControlEx3";
		this.labelControlEx3.ReadOnly = false;
		this.labelControlEx3.SettingInVisible = false;
		this.labelControlEx3.SettingReadonly = false;
		this.labelControlEx3.Size = new System.Drawing.Size(53, 13);
		this.labelControlEx3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx3.TabIndex = 28;
		this.labelControlEx3.Text = "Go to page";
		this.txt_totalRecord.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.txt_totalRecord.EditControl = null;
		this.txt_totalRecord.EditValue = "Total record";
		this.txt_totalRecord.FieldName = null;
		this.txt_totalRecord.Location = new System.Drawing.Point(336, 546);
		this.txt_totalRecord.Name = "txt_totalRecord";
		this.txt_totalRecord.ReadOnly = false;
		this.txt_totalRecord.SettingInVisible = false;
		this.txt_totalRecord.SettingReadonly = false;
		this.txt_totalRecord.Size = new System.Drawing.Size(58, 13);
		this.txt_totalRecord.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.txt_totalRecord.TabIndex = 29;
		this.txt_totalRecord.Text = "Total record";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(904, 576);
		this.ContextMenuStrip = this.contextMenuStrip2;
		base.Controls.Add(this.txt_totalRecord);
		base.Controls.Add(this.labelControlEx3);
		base.Controls.Add(this.txt_goToPage);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_SearchBPName);
		base.Controls.Add(this.txt_SearchBPType);
		base.Controls.Add(this.txt_SearchCurrency);
		base.Controls.Add(this.txt_SearchBPGroup);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_SearchBPCode);
		base.Controls.Add(this.labelControlEx2);
		base.Controls.Add(this.txt_pageSize);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.btn_pagerPrevious);
		base.Controls.Add(this.btn_pagerFirst);
		base.Controls.Add(this.btn_pagerLast);
		base.Controls.Add(this.btn_pagerNext);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.grd_QCBPMD);
		base.FormId = "oQuickCreationBusinessPartnerMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_QuickCreationBPMasterData";
		base.ShowInTaskbar = false;
		this.Text = "Quick Creation Business Partner Master Data";
		((System.ComponentModel.ISupportInitialize)this.grd_QCBPMD).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_QCBPMD).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPGroup1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SaleEmployee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PaymentTerm).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_CreditLimit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BPGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_pageSize.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchBPName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SearchCurrency.Properties).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_goToPage.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
