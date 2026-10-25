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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Payment;

public class frm_VoucherMasterData : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtdoc;

	private DataTable _dtLine;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private List<Common.Table_Code> _listCurrencyCode;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Voucher;

	private GridViewEx gv_Voucher;

	private XtraTabControlEx Detail;

	private XtraTabPageEx xtraTabPage1;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Detail;

	private GridViewEx gv_Detail;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEditSimple luk_Type;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEdit luk_Currency;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private RepositoryItemLookUpEditSimple luk_Status;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private RepositoryItemLookUpEditSimple luk_TypeDetail;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private RepositoryItemLookUpEdit luk_Bp;

	private RepositoryItemLookUpEdit luk_BpGroup;

	private ToolStripMenuItem archivedRowToolStripMenuItem;

	private RepositoryItemTextEdit txt_code;

	private RepositoryItemButtonEditCFL btn_Bp;

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
				txt_code.ReadOnly = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				txt_code.ReadOnly = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				txt_code.ReadOnly = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_VoucherMasterData()
	{
		InitializeComponent();
		OnLoadAsync += frm_VoucherMasterData_Load;
	}

	private async Task frm_VoucherMasterData_Load(object sender, EventArgs e)
	{
		try
		{
			SetDataType();
			SetDataCurrency();
			SetDataStatus();
			LoadDataBpGroup();
			SetDataTypeDetail();
			btn_Bp.ValueMember = "CardCode";
			btn_Bp.TypeCFL = Enums.TypeCFL.Customer;
			btn_Bp.RefreshDataSource();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_Bp;
			repositoryItemButtonEditCFL.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethod, (Action<string>)((string input) =>
			{
				this.DrillDown(ApzObjectTypes.oBusinessPartners, input);
			}));
			DataTable dataTable = await GetDataVoucher();
			grd_Voucher.DataSource = dataTable;
			if (!dataTable.IsNotNull())
			{
				gv_Voucher_FocusedRowObjectChanged(null, null);
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
				await Saves(1);
				break;
			case Enums.FormViewModeType.Update:
				await Saves(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void gv_Voucher_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gv_Voucher_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Detail_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Detail_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtLine = grd_Detail.DataSourceTable;
			bool flag = false;
			for (int i = 0; i < _dtLine.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(_dtLine.Rows[_dtLine.Rows.Count - 1]["Date"])))
				{
					flag = true;
				}
			}
			if (flag)
			{
				AddNewDetail();
			}
			if (FormMode != Enums.FormViewModeType.Add)
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

	private async void gv_Voucher_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			await GetDetailByCode(Function.ToString(gv_Voucher.GetFocusedRowCellValue("PinCode")));
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
			REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Voucher.GetSelectedRows() == null || gv_Voucher.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				int[] selectedRows = gv_Voucher.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gv_Voucher.GetRowCellValue(rowHandle, "PinCode").ToString() + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string text2 = await _httpClient.VoucherMasterData.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						GridControlEx gridControlEx = grd_Voucher;
						gridControlEx.DataSource = await GetDataVoucher();
						grd_Detail.DataSource = null;
						gv_Detail.RefreshData();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
						WinCommons.Error(Enums.MessageCode.DeleteError, "");
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		AddNewDetail();
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int focusedRowHandle = gv_Detail.FocusedRowHandle;
			_dtLine.Rows.RemoveAt(focusedRowHandle);
			grd_Detail.DataSource = _dtLine;
			grd_Detail.RefreshDataSource();
			if (FormMode != Enums.FormViewModeType.Add)
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

	private void archivedRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gv_Voucher.SetFocusedRowCellValue("Status", "I");
			if (FormMode != Enums.FormViewModeType.Add)
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

	private void gv_Voucher_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Voucher.GetFocusedRowCellValue("Status"));
			if (text.Equals("I"))
			{
				e.Cancel = true;
			}
			else if (text.Equals("C") && gv_Voucher.FocusedColumn.FieldName != "Remark")
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

	private void gv_Voucher_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			string currencyTrial = Function.ToString(gv_Detail.GetFocusedRowCellValue("Currency"));
			int num = -1;
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currencyTrial.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			if (!e.Column.FieldName.ToUpper().Equals("VALUE"))
			{
				return;
			}
			string text = Function.ToString(gv_Voucher.GetRowCellValue(e.ListSourceRowIndex, "Type"));
			if (Function.ParseDouble(e.Value) > 0.0)
			{
				if (text.Equals("A"))
				{
					e.DisplayText = string.Format("{0:n" + ((num == -1) ? GlobalData.UnitPriceFormat.NumberDecimalDigits : num) + "} {1}", e.Value, currencyTrial.ToUpper());
				}
				else
				{
					e.DisplayText = string.Format("{0:n} {1}", e.Value, "%");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Voucher_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "PinCode")
			{
				e.Value.ToString().Trim();
			}
			if (FormMode != Enums.FormViewModeType.Add)
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

	private async Task<DataTable> GetDataVoucher()
	{
		try
		{
			return await _httpClient.VoucherMasterData.GetAllVoucher();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	public async Task GetDetailByCode(string code)
	{
		try
		{
			WinCommons._OpenProcessing();
			this.ResetData();
			DataTable dataSource = await _httpClient.VoucherMasterData.GetDetailByCode(code);
			grd_Detail.DataSource = dataSource;
			AddNewDetail();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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
			FormMode = Enums.FormViewModeType.Add;
			grd_Voucher.DataSourceTable.Rows.Clear();
			grd_Detail.DataSourceTable.Rows.Clear();
			_dtdoc = grd_Voucher.DataSourceTable.Clone();
			if (_dtdoc == null && _dtdoc.Columns.Count < 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtdoc.NewRow();
			_dtdoc.Rows.Add(row);
			grd_Voucher.DataSource = _dtdoc;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewDetail()
	{
		_dtLine = grd_Detail.DataSourceTable;
		if (_dtLine == null && _dtLine.Columns.Count < 0)
		{
			WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
		}
		DataRow row = _dtLine.NewRow();
		_dtLine.Rows.Add(row);
		grd_Detail.DataSource = _dtLine;
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await GetDataVoucher();
			if (dataTable.IsNotNull())
			{
				grd_Voucher.DataSource = dataTable;
				grd_Voucher.RefreshDataSource();
				string code = Function.ToString(gv_Voucher.GetFocusedRowCellValue("PinCode"));
				await GetDetailByCode(code);
			}
			else
			{
				grd_Voucher.DataSource = null;
				grd_Voucher.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataCurrency()
	{
		try
		{
			_listCurrencyCode = GlobalData.Currencies;
			if (!_listCurrencyCode.Exists((Common.Table_Code x) => x.Code == "##"))
			{
				_listCurrencyCode.Add(new Common.Table_Code
				{
					Code = "##",
					Name = "All Currencies"
				});
			}
			luk_Currency.DataSource = _listCurrencyCode;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void SetDataBP()
	{
		try
		{
			DataTable dataTable = (await LoadBusinessPartner("C", GlobalData.UserName, GlobalData.DefaultBranch.ToString())).Tables[1];
			DataRow dataRow = dataTable.NewRow();
			if (dataTable.Select(string.Format("CardCode = '{0}'", "##")).Count() == 0)
			{
				dataRow["CardCode"] = "##";
				dataRow["Cardname"] = "All";
				dataTable.Rows.Add(dataRow);
			}
			luk_Bp.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataBpGroup()
	{
		try
		{
			DataTable dataTable = _httpClient.BusinessMaster.ShowCardGroup("C");
			DataRow dataRow = dataTable.NewRow();
			if (dataTable.Select("GroupCode=-5").Count() == 0)
			{
				dataRow["GroupCode"] = "-5";
				dataRow["GroupName"] = "All";
				dataTable.Rows.Add(dataRow);
			}
			luk_BpGroup.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataSet> LoadBusinessPartner(string cardType, string userName, string branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "CFL");
			return await helper.GetAsync("LoadBusinessPartner", new { cardType, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void SetDataType()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Amount",
			Code = "A"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Percent",
			Code = "P"
		});
		luk_Type.DataSource = list;
	}

	private void SetDataStatus()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Active",
			Code = "A"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Pending",
			Code = "P"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Close",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "InActive",
			Code = "I"
		});
		luk_Status.DataSource = list;
	}

	private void SetDataTypeDetail()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Issue",
			Code = "I"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Receipt",
			Code = "R"
		});
		luk_TypeDetail.DataSource = list;
	}

	private async Task<int> Saves(int type)
	{
		_ = 2;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				switch (type)
				{
				case 2:
				{
					string arg = Function.ToString(gv_Voucher.GetFocusedRowCellValue("PinCode"));
					_dtdoc = grd_Voucher.DataSourceTable;
					DataRow[] array = _dtdoc.Select($"PinCode='{arg}'");
					if (array.Length != 0)
					{
						_dtdoc = array.CopyToDataTable();
					}
					break;
				}
				case 1:
					_dtdoc = grd_Voucher.DataSourceTable;
					break;
				}
				for (int i = 0; i < _dtdoc.Rows.Count; i++)
				{
					if (Function.ToString(_dtdoc.Rows[i]["PinCode"]) == "")
					{
						_dtdoc.Rows.RemoveAt(i--);
					}
				}
				_dtLine.AcceptChanges();
				if (grd_Detail.DataSource != null)
				{
					_dtLine = grd_Detail.DataSourceTable;
					for (int j = 0; j < _dtLine.Rows.Count; j++)
					{
						if (Function.ToString(_dtLine.Rows[j]["Date"]) == "")
						{
							_dtLine.Rows.RemoveAt(j--);
						}
					}
					_dtLine.AcceptChanges();
				}
				_dtdoc.TableName = "Doc";
				_dtLine.TableName = "Line";
				dataSet.Tables.Add(_dtdoc.Copy());
				dataSet.Tables.Add(_dtLine.Copy());
				HttpResult httpResult = null;
				switch (type)
				{
				case 1:
					httpResult = await _httpClient.VoucherMasterData.Add(dataSet);
					break;
				case 2:
					httpResult = await _httpClient.VoucherMasterData.Update(dataSet);
					break;
				}
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					GridControlEx gridControlEx = grd_Voucher;
					gridControlEx.DataSource = await GetDataVoucher();
					FormMode = Enums.FormViewModeType.View;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
		return 1;
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.VoucherMasterData.FindDataNextLast(Function.ToString(gv_Voucher.GetFocusedRowCellValue("PinCode")), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Voucher.DataSource = dataTable;
				string code = Function.ToString(dataTable.Rows[0]["PinCode"]);
				await GetDetailByCode(code);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Payment.frm_VoucherMasterData));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Voucher = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.archivedRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Voucher = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_code = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Type = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Bp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_BpGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Status = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Bp = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Detail = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Detail = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Detail = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_TypeDetail = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Voucher).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Voucher).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_code).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Bp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BpGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Bp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.Detail).BeginInit();
		this.Detail.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeDetail).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Voucher);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.Detail);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(785, 390);
		this.splitContainerControl1.SplitterPosition = 149;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Voucher.AllowPasteExcel = true;
		this.grd_Voucher.ClipboardData = null;
		this.grd_Voucher.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Voucher.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Voucher.Location = new System.Drawing.Point(0, 0);
		this.grd_Voucher.MainView = this.gv_Voucher;
		this.grd_Voucher.Name = "grd_Voucher";
		this.grd_Voucher.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.luk_Currency, this.luk_Type, this.luk_Status, this.luk_Bp, this.luk_BpGroup, this.txt_code, this.btn_Bp });
		this.grd_Voucher.Size = new System.Drawing.Size(785, 149);
		this.grd_Voucher.TabIndex = 0;
		this.grd_Voucher.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Voucher });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem, this.archivedRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(122, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
		this.addRowToolStripMenuItem.Text = "Add";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
		this.deleteRowToolStripMenuItem.Text = "Remove";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.archivedRowToolStripMenuItem.Name = "archivedRowToolStripMenuItem";
		this.archivedRowToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
		this.archivedRowToolStripMenuItem.Text = "Archived";
		this.archivedRowToolStripMenuItem.Click += new System.EventHandler(this.archivedRowToolStripMenuItem_Click);
		this.gv_Voucher.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Voucher.ColumnKey = null;
		this.gv_Voucher.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10 });
		this.gv_Voucher.DisableFormSetting = false;
		this.gv_Voucher.DisplayException = false;
		this.gv_Voucher.EditColumn = null;
		this.gv_Voucher.FieldCurrency = null;
		this.gv_Voucher.FieldRate = null;
		this.gv_Voucher.FormId = "oVoucherMasterData";
		this.gv_Voucher.GridControl = this.grd_Voucher;
		this.gv_Voucher.IgnoreFilter = false;
		this.gv_Voucher.IndexEditRow = -1;
		this.gv_Voucher.IndicatorWidth = 35;
		this.gv_Voucher.IsApzTable = false;
		this.gv_Voucher.IsInsertNewRow = false;
		this.gv_Voucher.IsLoadingData = false;
		this.gv_Voucher.IsMappingContextAction = false;
		this.gv_Voucher.ItemId = "OVOUCHER";
		this.gv_Voucher.LstColumnAllowBatch = null;
		this.gv_Voucher.LstColumnNotAllowBatch = null;
		this.gv_Voucher.LstColumnNotEditable = null;
		this.gv_Voucher.LstColumNotSetting = null;
		this.gv_Voucher.Name = "gv_Voucher";
		this.gv_Voucher.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Voucher.OptionsView.ShowGroupPanel = false;
		this.gv_Voucher.UseBatchMod = false;
		this.gv_Voucher.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Voucher_CustomDrawRowIndicator);
		this.gv_Voucher.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Voucher_ShowingEditor);
		this.gv_Voucher.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Voucher_FocusedRowObjectChanged);
		this.gv_Voucher.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Voucher_CellValueChanged);
		this.gv_Voucher.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Voucher_CellValueChanging);
		this.gv_Voucher.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Voucher_CustomColumnDisplayText);
		this.gridColumnEx1.Caption = "Pin Code";
		this.gridColumnEx1.ColumnEdit = this.txt_code;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "PinCode";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.txt_code.AutoHeight = false;
		this.txt_code.Name = "txt_code";
		this.txt_code.ReadOnly = true;
		this.gridColumnEx2.Caption = "VoucherName";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "PinName";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx3.Caption = "Type";
		this.gridColumnEx3.ColumnEdit = this.luk_Type;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Type";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.luk_Type.AutoHeight = false;
		this.luk_Type.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Type.DisplayMember = "Name";
		this.luk_Type.DropDownRows = 3;
		this.luk_Type.IsSearchControl = false;
		this.luk_Type.Name = "luk_Type";
		this.luk_Type.NullText = "";
		this.luk_Type.ShowFooter = false;
		this.luk_Type.ShowHeader = false;
		this.luk_Type.ValueMember = "Code";
		this.gridColumnEx4.Caption = "Value";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Value";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx5.Caption = "Currency";
		this.gridColumnEx5.ColumnEdit = this.luk_Currency;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Currency";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.luk_Currency.AutoHeight = false;
		this.luk_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value")
		});
		this.luk_Currency.DisplayMember = "Code";
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.NullText = "";
		this.luk_Currency.ValueMember = "Code";
		this.gridColumnEx6.Caption = "SpecialBp";
		this.gridColumnEx6.ColumnEdit = this.btn_Bp;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "SpecialBp";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.btn_Bp.AutoHeight = false;
		this.btn_Bp.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.btn_Bp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Bp.ContainerControl = this;
		this.btn_Bp.DisplayMember = "CardCode";
		this.btn_Bp.DisplayMember2 = "";
		this.btn_Bp.LoadDataSourceWhenEnter = false;
		this.btn_Bp.Name = "btn_Bp";
		this.btn_Bp.SeparateCharacterForMulti = ';';
		this.btn_Bp.SeparateString = ", ";
		this.btn_Bp.UsingAutoCompleteBox = false;
		this.btn_Bp.UsingDataSource = true;
		this.btn_Bp.ValidateWhenNoDataSource = false;
		this.btn_Bp.ValueMember = "CardCode";
		this.gridColumnEx7.Caption = "SpecialBpGroup";
		this.gridColumnEx7.ColumnEdit = this.luk_BpGroup;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "SpecialBpGroup";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 6;
		this.luk_BpGroup.AutoHeight = false;
		this.luk_BpGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BpGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("GroupCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_BpGroup.DisplayMember = "GroupName";
		this.luk_BpGroup.Name = "luk_BpGroup";
		this.luk_BpGroup.NullText = "";
		this.luk_BpGroup.ValueMember = "GroupCode";
		this.gridColumnEx8.Caption = "Status";
		this.gridColumnEx8.ColumnEdit = this.luk_Status;
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
		this.gridColumnEx8.VisibleIndex = 7;
		this.luk_Status.AutoHeight = false;
		this.luk_Status.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Status.DisplayMember = "Name";
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.NullText = "";
		this.luk_Status.ShowFooter = false;
		this.luk_Status.ShowHeader = false;
		this.luk_Status.ValueMember = "Code";
		this.gridColumnEx9.Caption = "FromDate";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx9.FieldName = "FromDate";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 8;
		this.gridColumnEx10.Caption = "ToDate";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx10.FieldName = "ToDate";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 9;
		this.luk_Bp.AutoHeight = false;
		this.luk_Bp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Bp.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CardName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CardCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Bp.DisplayMember = "CardName";
		this.luk_Bp.Name = "luk_Bp";
		this.luk_Bp.NullText = "";
		this.luk_Bp.ValueMember = "CardCode";
		this.Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.Detail.IsSearchControl = false;
		this.Detail.Location = new System.Drawing.Point(0, 0);
		this.Detail.Name = "Detail";
		this.Detail.SelectedTabPage = this.xtraTabPage1;
		this.Detail.Size = new System.Drawing.Size(785, 231);
		this.Detail.TabIndex = 0;
		this.Detail.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_Detail);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Detail";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(783, 206);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Detail";
		this.grd_Detail.AllowPasteExcel = true;
		this.grd_Detail.ClipboardData = null;
		this.grd_Detail.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Detail.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Detail.Location = new System.Drawing.Point(0, 0);
		this.grd_Detail.MainView = this.gv_Detail;
		this.grd_Detail.Name = "grd_Detail";
		this.grd_Detail.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.luk_TypeDetail });
		this.grd_Detail.Size = new System.Drawing.Size(783, 206);
		this.grd_Detail.TabIndex = 0;
		this.grd_Detail.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Detail });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem1, this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gv_Detail.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Detail.ColumnKey = null;
		this.gv_Detail.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx18 });
		this.gv_Detail.DisableFormSetting = false;
		this.gv_Detail.DisplayException = false;
		this.gv_Detail.EditColumn = null;
		this.gv_Detail.FieldCurrency = null;
		this.gv_Detail.FieldRate = null;
		this.gv_Detail.FormId = "oVoucherMasterData";
		this.gv_Detail.GridControl = this.grd_Detail;
		this.gv_Detail.IgnoreFilter = false;
		this.gv_Detail.IndexEditRow = -1;
		this.gv_Detail.IndicatorWidth = 35;
		this.gv_Detail.IsApzTable = false;
		this.gv_Detail.IsInsertNewRow = false;
		this.gv_Detail.IsLoadingData = false;
		this.gv_Detail.IsMappingContextAction = false;
		this.gv_Detail.ItemId = "ODETAIL";
		this.gv_Detail.LstColumnAllowBatch = null;
		this.gv_Detail.LstColumnNotAllowBatch = null;
		this.gv_Detail.LstColumnNotEditable = null;
		this.gv_Detail.LstColumNotSetting = null;
		this.gv_Detail.Name = "gv_Detail";
		this.gv_Detail.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Detail.OptionsView.ShowGroupPanel = false;
		this.gv_Detail.UseBatchMod = false;
		this.gv_Detail.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Detail_CustomDrawRowIndicator);
		this.gv_Detail.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Detail_CellValueChanging);
		this.gridColumnEx11.Caption = "PinCode";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "PinCode";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx12.Caption = "LineNum";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "LineNum";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx13.Caption = "Date";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx13.FieldName = "Date";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 0;
		this.gridColumnEx14.Caption = "P.I.C";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "Pic";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 1;
		this.gridColumnEx15.Caption = "Type";
		this.gridColumnEx15.ColumnEdit = this.luk_TypeDetail;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "Type";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 2;
		this.luk_TypeDetail.AutoHeight = false;
		this.luk_TypeDetail.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_TypeDetail.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TypeDetail.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_TypeDetail.DisplayMember = "Name";
		this.luk_TypeDetail.IsSearchControl = false;
		this.luk_TypeDetail.Name = "luk_TypeDetail";
		this.luk_TypeDetail.NullText = "";
		this.luk_TypeDetail.ShowFooter = false;
		this.luk_TypeDetail.ShowHeader = false;
		this.luk_TypeDetail.ValueMember = "Code";
		this.gridColumnEx16.Caption = "Linked Trans ID";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 3;
		this.gridColumnEx17.Caption = "Trans Type";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "TransType";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 4;
		this.gridColumnEx18.Caption = "Remark";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "Remark";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 5;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 396);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 396);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(785, 421);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oVoucherMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_VoucherMasterData";
		this.Text = "Voucher Master Data";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Voucher).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Voucher).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_code).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Bp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BpGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Bp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.Detail).EndInit();
		this.Detail.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Detail).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Detail).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TypeDetail).EndInit();
		base.ResumeLayout(false);
	}
}
