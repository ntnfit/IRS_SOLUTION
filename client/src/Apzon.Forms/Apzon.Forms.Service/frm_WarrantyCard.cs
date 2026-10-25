using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_WarrantyCard : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtLine;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	public int WarranyId;

	private readonly Dictionary<string, string> _lstSatus = new Dictionary<string, string>
	{
		{ "A", "Active" },
		{ "I", "InActive" }
	};

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private SimpleButton btn_Ok;

	private SimpleButton btn_Cancel;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private SimpleButtonEx btn_Search;

	private TextEditEx txt_WNo;

	private ButtonEditCFL btn_BpCode;

	private ButtonEditCFL btn_ItemCode;

	private LookUpEditEx luk_Status;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private RepositoryItemLookUpEditSimple luk_TypeDoc;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemButtonEditCFL btn_ItemDoc;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemLookUpEditSimple luk_StatusDoc;

	private RepositoryItemButtonEditCFL btn_Technical;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx22;

	private RepositoryItemButtonEditCFL btn_Serial;

	private RepositoryItemButtonEditCFL btn_MrfSerial;

	private RepositoryItemButtonEditCFL btn_BpDoc;

	private TextEditEx txt_No;

	private LabelLine labelLine5;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEditCFL btn_ContractTemplate;

	private RepositoryItemDateEditEx repositoryItemDateEditEx3;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private RepositoryItemLookUpEditSimple luk_StatusLine;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

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
				btn_Ok.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_WarrantyCard()
	{
		OnLoadAsync += frm_WarrantyCard_Load;
		InitializeComponent();
	}

	private async Task<DataTable> LoadData(int id, string warrantyCode, string itemCode, string bpCode, string status)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.WarrantyCard.LoadData(id, warrantyCode.Trim(), itemCode.Trim(), bpCode.Trim(), status);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadLineById(int id)
	{
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataTable dataSource = await _httpClient.WarrantyCard.GetDataById(id);
			grd_Line.DataSource = dataSource;
			grd_Line.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Line.DataSourceTable().Rows.Clear();
				_dtDoc = grd_Doc.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow row = _dtDoc.NewRow();
				DataRow dataRow = row;
				dataRow["WrntyID"] = await SetDataNo();
				row["Status"] = "A";
				_dtDoc.Rows.Add(row);
				grd_Doc.DataSource = _dtDoc;
				NewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewLine()
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (_dtLine == null || _dtLine.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtLine.NewRow();
			dataRow["Status"] = "O";
			_dtLine.Rows.Add(dataRow);
			_dtLine.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		if (!_isLoading && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SetDataNo()
	{
		try
		{
			return await _httpClient.WarrantyCard.GetMaxId() + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.WarrantyCard.FindNextLastFrist(Function.ParseInt(txt_No.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("WrntyID"));
				txt_No.EditValue = num;
				await LoadLineById(num);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			DataTable dataTable = grd_Doc.DatasourceTable().Copy();
			(from DataRow t in dataTable.Rows
				where string.IsNullOrEmpty(Function.ToString(t["WrntyNum"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			dataTable.AcceptChanges();
			if (type == 2)
			{
				DataTable dataTable2 = dataTable.Copy();
				dataTable2.AcceptChanges();
				DataRow[] array = dataTable2.Select("WrntyID=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("WrntyID")));
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
			}
			else
			{
				_dtDoc = dataTable.Copy();
			}
			_dtLine = grd_Line.DataSourceTable().Copy();
			(from DataRow t in _dtLine.Rows
				where string.IsNullOrEmpty(Function.ToString(t["ContractNum"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			_dtLine.AcceptChanges();
			DataTable dataTable3 = _dtLine.Copy();
			if ((from t in _dtLine.AsEnumerable()
				select Function.ToString(t["ContractNum"])).Distinct().Count() != dataTable3.Rows.Count)
			{
				List<string> list = (from dr in _dtLine.AsEnumerable()
					select dr.Field<string>("ContractNum") into x
					group x by x into g
					where g.Count() > 1
					select g.Key).ToList();
				WinCommons.Warning(Enums.MessageCode.ContractNumAlreadyExistsInTableWithCode, "", Function.ToString(list[0]));
				return 1;
			}
			_dtDoc.TableName = "dtDoc";
			_dtLine.TableName = "dtLine";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtLine.Copy());
			HttpResult httpResult = ((type != 2) ? (await _httpClient.WarrantyCard.Add(dataSet)) : (await _httpClient.WarrantyCard.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				if (type == 2)
				{
					grd_Doc.DatasourceTable().AcceptChanges();
					grd_Line.DatasourceTable().AcceptChanges();
				}
				else
				{
					btn_Search_Click(null, null);
				}
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private void ChooseContractReceiptMulti(List<DataRow> rows)
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			_dtDoc = grd_Doc.DatasourceTable();
			if (_dtLine == null || _dtLine.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			if (!string.IsNullOrEmpty(Function.ToString(rows[0]["ContractTemplateNum"])))
			{
				gv_Line.SetFocusedRowCellValue("ContractID", Function.ParseInt(rows[0]["ContractID"]));
				gv_Line.SetFocusedRowCellValue("ContractTemplateNum", Function.ToString(rows[0]["ContractTemplateNum"]));
				gv_Line.SetFocusedRowCellValue("ContractCycle", Function.ParseInt(rows[0]["ContractCycle"]));
				gv_Line.SetFocusedRowCellValue("CycleUnit", Function.ToString(rows[0]["CycleUnit"]));
				gv_Line.SetFocusedRowCellValue("TolerantTime", Function.ParseInt(rows[0]["TolerantTime"]));
				gv_Line.SetFocusedRowCellValue("TolerantUnit", Function.ToString(rows[0]["TolerantUnit"]));
				for (int i = 1; i < rows.Count; i++)
				{
					DataRow dataRow = _dtLine.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ContractNum"]) == "");
					if (dataRow != null && !string.IsNullOrEmpty(Function.ToString(rows[i]["ContractTemplateNum"])))
					{
						dataRow["ContractTemplateNum"] = Function.ToString(rows[i]["ContractTemplateNum"]);
						dataRow["ContractID"] = Function.ParseInt(rows[i]["ContractID"]);
						dataRow["ContractCycle"] = Function.ParseInt(rows[i]["ContractCycle"]);
						dataRow["CycleUnit"] = Function.ToString(rows[i]["CycleUnit"]);
						dataRow["TolerantTime"] = Function.ParseInt(rows[i]["TolerantTime"]);
						dataRow["TolerantUnit"] = Function.ToString(rows[i]["TolerantUnit"]);
					}
					NewLine();
				}
			}
			_dtLine.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ItemCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			if (dr != null)
			{
				gv_Doc.SetFocusedRowCellValue("ItemCode", Function.ToString(dr["ItemCode"]));
				gv_Doc.SetFocusedRowCellValue("ItemName", Function.ToString(dr["Dscription"]));
				gv_Doc.SetFocusedRowCellValue("SerialNum", Function.ToString(dr["DistNumber"]));
				gv_Doc.SetFocusedRowCellValue("MfrSerial", Function.ToString(dr["MnfSerial"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_WarrantyCard_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			btn_BpDoc.ValueMember = "CardCode";
			btn_BpDoc.FieldNameInDataRow("CardCode", "CardName");
			btn_BpDoc.FieldNameInGrid("CardCode", "CardName");
			btn_BpDoc.TypeCFL = Enums.TypeCFL.Bp;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_BpDoc;
			repositoryItemButtonEditCFL.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(repositoryItemButtonEditCFL.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_BpCode.Properties.DataSource = dtData.Copy();
			}));
			btn_BpDoc.LoadDataSourceWhenEnter = true;
			btn_BpDoc.RefreshDataSource();
			btn_BpDoc.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_BpCode.Properties.ValueMember = "CardCode";
			btn_BpCode.Properties.TypeCFL = Enums.TypeCFL.Bp;
			btn_BpCode.Properties.LoadDataSourceWhenEnter = true;
			btn_BpCode.Properties.RefreshDataSource();
			btn_BpCode.Properties.TypeDrillDown = ApzObjectTypes.oBusinessPartners;
			btn_Technical.ValueMember = "EmpId";
			btn_Technical.DisplayMember = "lastName";
			btn_Technical.DisplayMember2 = "firstName";
			btn_Technical.SeparateString = " ";
			btn_Technical.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			btn_Technical.RefreshDataSource();
			btn_Technical.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			btn_ItemCode.Properties.ValueMember = "ItemCode";
			btn_ItemCode.Properties.TypeCFL = Enums.TypeCFL.ListItems;
			btn_ItemCode.Properties.TypeDrillDown = ApzObjectTypes.oItems;
			btn_ItemCode.Properties.RefreshDataSource();
			btn_ItemDoc.ValueMember = "ItemCode";
			btn_ItemDoc.FieldNameInDataRow("ItemCode", "Dscription");
			btn_ItemDoc.FieldNameInGrid("ItemCode", "ItemName");
			btn_ItemDoc.TypeCFL = Enums.TypeCFL.ListItemByCardCode;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_ItemDoc;
			repositoryItemButtonEditCFL2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.BpCode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CardCode"));
				frm.Type = Function.ToString(gv_Doc.GetFocusedRowCellValue("Type"));
			}));
			btn_ItemDoc.TypeDrillDown = ApzObjectTypes.oItems;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = btn_ItemDoc;
			repositoryItemButtonEditCFL3.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL3.HandleChooseItem, new Action<DataRow>(ItemCode_HandleChooseItem));
			btn_ItemDoc.RefreshDataSource();
			btn_ContractTemplate.ValueMember = "ContractTemplateNum";
			btn_ContractTemplate.FindCFLBy(Enums.TypeCFL.ListServiceContractTemplate, true, "", Enums.ItemObjectType.NoItemType, null);
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = btn_ContractTemplate;
			repositoryItemButtonEditCFL4.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL4.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.Type = Function.ToString(gv_Doc.GetFocusedRowCellValue("Type"));
			}));
			btn_ContractTemplate.TypeDrillDown = ApzObjectTypes.oServiceContractTemplate;
			btn_ContractTemplate.HandleChooseItemMulti = ChooseContractReceiptMulti;
			btn_ContractTemplate.RefreshDataSource();
			luk_StatusDoc.SetDataSource(_lstSatus);
			luk_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "A", "Active" },
				{ "I", "InActive" }
			});
			luk_TypeDoc.SetDataSource(new Dictionary<string, string>
			{
				{ "S", "Sales" },
				{ "P", "Purchase" }
			});
			luk_StatusLine.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "L", "Close" }
			});
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx> { ["APZ_OINS"] = gv_Doc };
			dictionary["APZ_INS1"] = gv_Line;
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OINS", dictionary), WinFuntions.LoadUdfColumns(gv_Line, "APZ_INS1", dictionary));
			new DataTable();
			string warrantyCode = Function.ToString(txt_WNo.EditValue);
			string status = Function.ToString(luk_Status.EditValue);
			string itemCode = Function.ToString(btn_ItemCode.EditValue);
			string bpCode = Function.ToString(btn_BpCode.EditValue);
			if (WarranyId > 0)
			{
				txt_No.EditValue = WarranyId;
			}
			DataTable dataTable = await LoadData(Function.ParseInt(txt_No.EditValue), warrantyCode, itemCode, bpCode, status);
			WarranyId = 0;
			grd_Doc.DataSource = dataTable;
			grd_Doc.DatasourceTable().AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				gv_Doc_FocusedRowChanged(null, null);
			}
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtLine = grd_Line.DatasourceTable();
			string text = Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, "CycleUnit"));
			int num = Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "ContractCycle"));
			int num2 = Function.ParseInt(gv_Line.GetRowCellValue(e.RowHandle, "TolerantTime"));
			string text2 = Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, "TolerantUnit"));
			if (e.Column.FieldName == "ContractTemplateNum")
			{
				DateTime dateTime = Function.ParseDateTimes(gv_Line.GetRowCellValue(e.RowHandle, "ContractDate"));
				if (!string.IsNullOrEmpty(Function.ToString(dateTime)))
				{
					switch (text)
					{
					case "Y":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime.AddYears(num).AddDays(-1.0);
						break;
					case "M":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime.AddMonths(num).AddDays(-1.0);
						break;
					case "D":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime.AddDays(num);
						break;
					}
					DateTime dateTime2 = Function.ParseDateTimes(gv_Line.GetRowCellValue(e.RowHandle, "EndDate"));
					switch (text2)
					{
					case "Y":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime2.AddYears(num2).AddDays(-1.0);
						break;
					case "M":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime2.AddMonths(num2).AddDays(-1.0);
						break;
					case "D":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime2.AddDays(num2);
						break;
					}
				}
			}
			if (e.Column.FieldName == "ContractDate")
			{
				DateTime dateTime3 = Function.ParseDateTimes(e.Value);
				if (!string.IsNullOrEmpty(Function.ToString(dateTime3)))
				{
					switch (text)
					{
					case "Y":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime3.AddYears(num).AddDays(-1.0);
						break;
					case "M":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime3.AddMonths(num).AddDays(-1.0);
						break;
					case "D":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime3.AddDays(num);
						break;
					}
					DateTime dateTime4 = Function.ParseDateTimes(gv_Line.GetRowCellValue(e.RowHandle, "EndDate"));
					switch (text2)
					{
					case "Y":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime4.AddYears(num2).AddDays(-1.0);
						break;
					case "M":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime4.AddMonths(num2).AddDays(-1.0);
						break;
					case "D":
						_dtLine.Rows[gv_Line.GetDataSourceRowIndex(e.RowHandle)]["EndDate"] = dateTime4.AddDays(num2);
						break;
					}
				}
			}
			if ((from t in _dtLine.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["ContractNum"]))
				select t).ToList().Count <= 0)
			{
				NewLine();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string warrantyCode = Function.ToString(txt_WNo.EditValue);
			string status = Function.ToString(luk_Status.EditValue);
			string itemCode = Function.ToString(btn_ItemCode.EditValue);
			string bpCode = Function.ToString(btn_BpCode.EditValue);
			if (WarranyId > 0)
			{
				txt_No.EditValue = WarranyId;
			}
			DataTable dataSource = await LoadData(Function.ParseInt(txt_No.EditValue), warrantyCode, itemCode, bpCode, status);
			WarranyId = 0;
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Line.DatasourceTable().Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("WrntyID"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("WrntyID"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null && gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("WrntyID"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveWarranty, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, text) == DialogResult.Yes)
			{
				string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("WrntyID"));
				string text2 = await _httpClient.WarrantyCard.DeleteDocs(id);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewLine();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (gv_Line.DataRowCount > 0)
		{
			grd_Line.BeginUpdate();
			gv_Line.DeleteSelectedRows();
			grd_Line.EndUpdate();
			grd_Line.DatasourceTable().AcceptChanges();
			DataChanged();
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_WarrantyCard));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled12 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled13 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled14 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled15 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed16 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled16 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TypeDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemDoc = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Serial = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_MrfSerial = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_BpDoc = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Technical = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ContractTemplate = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx3 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusLine = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_WNo = new Apzon.Controls.Editor.TextEditEx();
		this.btn_BpCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_ItemCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.luk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Serial).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_MrfSerial).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Technical).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ContractTemplate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 77);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(807, 389);
		this.splitContainerControl1.SplitterPosition = 158;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.luk_TypeDoc, this.btn_ItemDoc, this.luk_StatusDoc, this.btn_Technical, this.btn_Serial, this.btn_MrfSerial, this.btn_BpDoc });
		this.grd_Doc.Size = new System.Drawing.Size(807, 158);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10,
			this.gridColumnEx11, this.gridColumnEx12
		});
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oWarrantyCard";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OINS";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "WrntyID";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "WrntyID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Type";
		this.gridColumnEx2.ColumnEdit = this.luk_TypeDoc;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Type";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.luk_TypeDoc.AutoHeight = false;
		this.luk_TypeDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_TypeDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TypeDoc.IsSearchControl = false;
		this.luk_TypeDoc.Name = "luk_TypeDoc";
		this.luk_TypeDoc.NullText = "";
		this.luk_TypeDoc.ShowFooter = false;
		this.luk_TypeDoc.ShowHeader = false;
		this.gridColumnEx3.Caption = "Warranty No";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "WrntyNum";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 0;
		this.gridColumnEx4.Caption = "Item Code";
		this.gridColumnEx4.ColumnEdit = this.btn_ItemDoc;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ItemCode";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 4;
		this.btn_ItemDoc.AutoHeight = false;
		this.btn_ItemDoc.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_ItemDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemDoc.ContainerControl = this;
		this.btn_ItemDoc.DisplayMember = "ItemCode";
		this.btn_ItemDoc.DisplayMember2 = "";
		this.btn_ItemDoc.LoadDataSourceWhenEnter = false;
		this.btn_ItemDoc.Name = "btn_ItemDoc";
		this.btn_ItemDoc.SeparateCharacterForMulti = ';';
		this.btn_ItemDoc.SeparateString = ", ";
		this.btn_ItemDoc.UsingAutoCompleteBox = false;
		this.btn_ItemDoc.UsingDataSource = true;
		this.btn_ItemDoc.ValidateWhenNoDataSource = false;
		this.btn_ItemDoc.ValueMember = "ItemCode";
		this.gridColumnEx5.Caption = "Item Name";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "ItemName";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx6.Caption = "Serial";
		this.gridColumnEx6.ColumnEdit = this.btn_Serial;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SerialNum";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 6;
		this.btn_Serial.AutoHeight = false;
		this.btn_Serial.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_Serial.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Serial.ContainerControl = this;
		this.btn_Serial.DisplayMember = "";
		this.btn_Serial.DisplayMember2 = "";
		this.btn_Serial.LoadDataSourceWhenEnter = false;
		this.btn_Serial.Name = "btn_Serial";
		this.btn_Serial.SeparateCharacterForMulti = ';';
		this.btn_Serial.SeparateString = ", ";
		this.btn_Serial.UsingAutoCompleteBox = false;
		this.btn_Serial.UsingDataSource = true;
		this.btn_Serial.ValidateWhenNoDataSource = false;
		this.btn_Serial.ValueMember = "";
		this.gridColumnEx7.Caption = "Mfr Serial";
		this.gridColumnEx7.ColumnEdit = this.btn_MrfSerial;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "MfrSerial";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 7;
		this.btn_MrfSerial.AutoHeight = false;
		this.btn_MrfSerial.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.btn_MrfSerial.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_MrfSerial.ContainerControl = this;
		this.btn_MrfSerial.DisplayMember = "";
		this.btn_MrfSerial.DisplayMember2 = "";
		this.btn_MrfSerial.LoadDataSourceWhenEnter = false;
		this.btn_MrfSerial.Name = "btn_MrfSerial";
		this.btn_MrfSerial.SeparateCharacterForMulti = ';';
		this.btn_MrfSerial.SeparateString = ", ";
		this.btn_MrfSerial.UsingAutoCompleteBox = false;
		this.btn_MrfSerial.UsingDataSource = true;
		this.btn_MrfSerial.ValidateWhenNoDataSource = false;
		this.btn_MrfSerial.ValueMember = "";
		this.gridColumnEx8.Caption = "Status";
		this.gridColumnEx8.ColumnEdit = this.luk_StatusDoc;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "Status";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 8;
		this.luk_StatusDoc.AutoHeight = false;
		this.luk_StatusDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusDoc.IsSearchControl = false;
		this.luk_StatusDoc.Name = "luk_StatusDoc";
		this.luk_StatusDoc.NullText = "";
		this.luk_StatusDoc.ShowFooter = false;
		this.luk_StatusDoc.ShowHeader = false;
		this.gridColumnEx9.Caption = "BP Code";
		this.gridColumnEx9.ColumnEdit = this.btn_BpDoc;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "CardCode";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.btn_BpDoc.AutoHeight = false;
		this.btn_BpDoc.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.btn_BpDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpDoc.ContainerControl = this;
		this.btn_BpDoc.DisplayMember = "CardCode";
		this.btn_BpDoc.DisplayMember2 = "";
		this.btn_BpDoc.LoadDataSourceWhenEnter = false;
		this.btn_BpDoc.Name = "btn_BpDoc";
		this.btn_BpDoc.SeparateCharacterForMulti = ';';
		this.btn_BpDoc.SeparateString = ", ";
		this.btn_BpDoc.UsingAutoCompleteBox = false;
		this.btn_BpDoc.UsingDataSource = true;
		this.btn_BpDoc.ValidateWhenNoDataSource = false;
		this.btn_BpDoc.ValueMember = "CardCode";
		this.gridColumnEx10.Caption = "BP Name";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "CardName";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 3;
		this.gridColumnEx11.Caption = "Technical / Queue";
		this.gridColumnEx11.ColumnEdit = this.btn_Technical;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "Technician";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 9;
		this.btn_Technical.AutoHeight = false;
		this.btn_Technical.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.btn_Technical.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Technical.ContainerControl = this;
		this.btn_Technical.DisplayMember = "lastName";
		this.btn_Technical.DisplayMember2 = "firstName";
		this.btn_Technical.LoadDataSourceWhenEnter = false;
		this.btn_Technical.Name = "btn_Technical";
		this.btn_Technical.SeparateCharacterForMulti = ';';
		this.btn_Technical.SeparateString = ", ";
		this.btn_Technical.UsingAutoCompleteBox = false;
		this.btn_Technical.UsingDataSource = true;
		this.btn_Technical.ValidateWhenNoDataSource = false;
		this.btn_Technical.ValueMember = "EmpId";
		this.gridColumnEx12.Caption = "Remark";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "Remark";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 10;
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(807, 221);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_Line);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Service Contract";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(805, 196);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Service Contract";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctm_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.btn_ContractTemplate, this.repositoryItemDateEditEx1, this.luk_StatusLine, this.repositoryItemDateEditEx2, this.repositoryItemDateEditEx3 });
		this.grd_Line.Size = new System.Drawing.Size(805, 196);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.ctm_Line.Name = "ctm_Line";
		this.ctm_Line.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx22,
			this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx25, this.gridColumnEx26
		});
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oWarrantyCard";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IgnoreFilter = false;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.IsMappingContextAction = false;
		this.gv_Line.ItemId = "APZ_INS1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Line.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gridColumnEx13.Caption = "WrntyID";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "WrntyID";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.AllowEdit = false;
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = true;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx14.Caption = "LineNum";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "LineNum";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.AllowEdit = false;
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx15.Caption = "ContractID";
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "ContractID";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx16.Caption = "Contract Template No";
		this.gridColumnEx16.ColumnEdit = this.btn_ContractTemplate;
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "ContractTemplateNum";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 1;
		this.gridColumnEx16.Width = 137;
		this.btn_ContractTemplate.AutoHeight = false;
		this.btn_ContractTemplate.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions16.Image");
		this.btn_ContractTemplate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance12, appearanceHovered12, appearancePressed12, appearanceDisabled12, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ContractTemplate.ContainerControl = this;
		this.btn_ContractTemplate.DisplayMember = "ContractTemplateNum";
		this.btn_ContractTemplate.DisplayMember2 = "";
		this.btn_ContractTemplate.LoadDataSourceWhenEnter = false;
		this.btn_ContractTemplate.Name = "btn_ContractTemplate";
		this.btn_ContractTemplate.SeparateCharacterForMulti = ';';
		this.btn_ContractTemplate.SeparateString = ", ";
		this.btn_ContractTemplate.UsingAutoCompleteBox = false;
		this.btn_ContractTemplate.UsingDataSource = true;
		this.btn_ContractTemplate.ValidateWhenNoDataSource = false;
		this.btn_ContractTemplate.ValueMember = "ContractTemplateNum";
		this.gridColumnEx17.Caption = "Contract No";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "ContractNum";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 0;
		this.gridColumnEx18.Caption = "Contract Date";
		this.gridColumnEx18.ColumnEdit = this.repositoryItemDateEditEx3;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx18.FieldName = "ContractDate";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 2;
		this.gridColumnEx18.Width = 93;
		this.repositoryItemDateEditEx3.AutoHeight = false;
		this.repositoryItemDateEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx3.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx3.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx3.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx3.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx3.IsSearchControl = false;
		this.repositoryItemDateEditEx3.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx3.Name = "repositoryItemDateEditEx3";
		this.gridColumnEx19.Caption = "End Date";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx19.FieldName = "EndDate";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 3;
		this.gridColumnEx19.Width = 94;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.gridColumnEx20.Caption = "Terminated Date";
		this.gridColumnEx20.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx20.FieldName = "TerminatedDate";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 4;
		this.gridColumnEx20.Width = 104;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gridColumnEx21.Caption = "Status";
		this.gridColumnEx21.ColumnEdit = this.luk_StatusLine;
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "Status";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx21.Visible = true;
		this.gridColumnEx21.VisibleIndex = 5;
		this.luk_StatusLine.AutoHeight = false;
		this.luk_StatusLine.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusLine.IsSearchControl = false;
		this.luk_StatusLine.Name = "luk_StatusLine";
		this.luk_StatusLine.NullText = "";
		this.luk_StatusLine.ShowFooter = false;
		this.luk_StatusLine.ShowHeader = false;
		this.gridColumnEx22.Caption = "Remark";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.FieldName = "Remark";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 6;
		this.gridColumnEx23.Caption = "ContractCycle";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "ContractCycle";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.OptionsColumn.AllowEdit = false;
		this.gridColumnEx23.OptionsColumn.ReadOnly = true;
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx24.Caption = "CycleUnit";
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = null;
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.FieldName = "CycleUnit";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.OptionsColumn.AllowEdit = false;
		this.gridColumnEx24.OptionsColumn.ReadOnly = true;
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx25.Caption = "TolerantTime";
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = null;
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.FieldName = "TolerantTime";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.AllowEdit = false;
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx26.Caption = "TolerantUnit";
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.FieldName = "TolerantUnit";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.OptionsColumn.AllowEdit = false;
		this.gridColumnEx26.OptionsColumn.ReadOnly = true;
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.Location = new System.Drawing.Point(12, 472);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "Ok";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(93, 472);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Warranty  Code";
		this.labelLine1.FieldName = "";
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 3;
		this.labelLine1.Utext = "Warranty  Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "BP Code";
		this.labelLine2.FieldName = "";
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 51);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "BP Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = "";
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(228, 12);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 5;
		this.labelLine3.Utext = "Status";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Item Code";
		this.labelLine4.FieldName = "";
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(228, 51);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "Item Code";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(638, 48);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 8;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.txt_WNo.EditControl = null;
		this.txt_WNo.FieldName = null;
		this.txt_WNo.IsSearchControl = true;
		this.txt_WNo.Location = new System.Drawing.Point(93, 12);
		this.txt_WNo.Name = "txt_WNo";
		this.txt_WNo.SettingInVisible = false;
		this.txt_WNo.SettingReadonly = false;
		this.txt_WNo.Size = new System.Drawing.Size(100, 20);
		this.txt_WNo.TabIndex = 9;
		this.btn_BpCode.EditControl = null;
		this.btn_BpCode.FieldName = null;
		this.btn_BpCode.IsSearchControl = true;
		this.btn_BpCode.Location = new System.Drawing.Point(93, 51);
		this.btn_BpCode.Name = "btn_BpCode";
		this.btn_BpCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_BpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance13, appearanceHovered13, appearancePressed13, appearanceDisabled13, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance14, appearanceHovered14, appearancePressed14, appearanceDisabled14, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_BpCode.Properties.ContainerControl = null;
		this.btn_BpCode.Properties.DisplayMember = "CardCode";
		this.btn_BpCode.Properties.DisplayMember2 = "";
		this.btn_BpCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_BpCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_BpCode.Properties.SeparateString = ", ";
		this.btn_BpCode.Properties.UsingAutoCompleteBox = false;
		this.btn_BpCode.Properties.UsingDataSource = true;
		this.btn_BpCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_BpCode.Properties.ValueMember = "CardCode";
		this.btn_BpCode.SettingInVisible = false;
		this.btn_BpCode.SettingReadonly = false;
		this.btn_BpCode.Size = new System.Drawing.Size(100, 20);
		this.btn_BpCode.TabIndex = 10;
		this.btn_ItemCode.EditControl = null;
		this.btn_ItemCode.FieldName = null;
		this.btn_ItemCode.IsSearchControl = true;
		this.btn_ItemCode.Location = new System.Drawing.Point(302, 51);
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance15, appearanceHovered15, appearancePressed15, appearanceDisabled15, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance16, appearanceHovered16, appearancePressed16, appearanceDisabled16, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCode.Properties.ContainerControl = null;
		this.btn_ItemCode.Properties.DisplayMember = "ItemCode";
		this.btn_ItemCode.Properties.DisplayMember2 = "";
		this.btn_ItemCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ItemCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_ItemCode.Properties.SeparateString = ", ";
		this.btn_ItemCode.Properties.UsingAutoCompleteBox = false;
		this.btn_ItemCode.Properties.UsingDataSource = true;
		this.btn_ItemCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_ItemCode.Properties.ValueMember = "ItemCode";
		this.btn_ItemCode.SettingInVisible = false;
		this.btn_ItemCode.SettingReadonly = false;
		this.btn_ItemCode.Size = new System.Drawing.Size(100, 20);
		this.btn_ItemCode.TabIndex = 11;
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = true;
		this.luk_Status.Location = new System.Drawing.Point(302, 12);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(100, 20);
		this.luk_Status.TabIndex = 12;
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = true;
		this.txt_No.Location = new System.Drawing.Point(498, 51);
		this.txt_No.Name = "txt_No";
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(100, 20);
		this.txt_No.TabIndex = 14;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Warranty .No";
		this.labelLine5.FieldName = "";
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(417, 51);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 13;
		this.labelLine5.Utext = "Warranty .No";
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(810, 497);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.btn_ItemCode);
		base.Controls.Add(this.btn_BpCode);
		base.Controls.Add(this.txt_WNo);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oWarrantyCard";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WarrantyCard";
		this.Text = "Waranty Card";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Serial).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_MrfSerial).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Technical).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ContractTemplate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_BpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
