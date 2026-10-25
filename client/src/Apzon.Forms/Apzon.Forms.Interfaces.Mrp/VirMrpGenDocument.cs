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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Mrps;
using Apzon.Forms.Properties;
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

namespace Apzon.Forms.Interfaces.Mrp;

public class VirMrpGenDocument : XtraFormEx
{
	protected Enums.ItemUomType SetItemUomType = Enums.ItemUomType.InventoryUom;

	protected string Controller = "";

	private IContainer components;

	private LabelLine lbSchedule;

	private SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private LabelLine lbInputType;

	protected GridControlEx grdDoc;

	protected ButtonEditEx txtCode;

	protected DropDownButton btnSave;

	protected LookUpEdit txtInput;

	protected GridViewEx gvDoc;

	protected RepositoryItemButtonEdit rWhs;

	protected RepositoryItemButtonEdit rItemCode;

	protected RepositoryItemLookUpEdit rBran;

	protected RepositoryItemLookUpEdit rSeri;

	protected RepositoryItemButtonEdit rCarCode;

	private RepositoryItemButtonEdit rOriginCode;

	private RepositoryItemLookUpEdit rUomGroup;

	private RepositoryItemLookUpEdit rUseBaseUnits;

	private RepositoryItemButtonEdit rUomCode;

	private RepositoryItemLookUpEdit rTaxCode;

	private RepositoryItemLookUpEdit rEmployee;

	private RepositoryItemLookUpEdit rOriginType;

	private RepositoryItemButtonEdit rProject;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				_formMode = Enums.FormViewModeType.View;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public VirMrpGenDocument()
	{
		InitializeComponent();
		OnLoadAsync += frm_GenPurchaseFromSchedule_Load;
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_GenPurchaseFromSchedule_Load(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			rOriginType.DataSource = WinFuntions.GetListOriginType();
			await GetBranch();
			await GetSeries();
			await LoadUomGroup();
			await LoadTaxCode();
			await LoadDataEmployee();
			LoadDataInputType();
			LoadInventoryUoM();
			LoadColumnGrid();
			LoadButtonSave();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtSchedule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(txtCode.EditValue);
			Code_ChooseFromList(sender, e);
			if (!text.Equals(Function.ToString(txtCode.EditValue)))
			{
				grdDoc.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txtCode_Validating(object sender, CancelEventArgs e)
	{
		if (grdDoc == null)
		{
			return;
		}
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (sender != null && !string.IsNullOrEmpty(Function.ToString(txtCode.EditValue)))
			{
				string text;
				using (new HttpClientHelper<string>(Configuration.ApiServer, Controller))
				{
					text = await GetDataCheckInput();
				}
				if (!string.IsNullOrEmpty(text))
				{
					txtCode.EditValue = null;
					grdDoc.DataSourceTable.Rows.Clear();
					WinCommons.Error(Enums.MessageCode.None, text);
					FormMode = Enums.FormViewModeType.View;
					txtCode.Focus();
					WinCommons._CloseProcessing();
					return;
				}
			}
			await LoadDataFromSchedule();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
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

	private void rWhs_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, Function.ToString(gvDoc.GetFocusedRowCellValue(gvDoc.FocusedColumn.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue(gvDoc.FocusedColumn.FieldName, frm_CFL2.ChooseItem["WhsCode"]);
				}
			}
			else
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gvDoc.FocusedColumn.FieldName));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gvDoc.FocusedColumn.FieldName));
			if (!string.IsNullOrWhiteSpace(text))
			{
				ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
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

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await LoadDataFromSchedule();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void txtInput_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txtCode.EditValue = null;
			grdDoc.DataSource = null;
			grdDoc.RefreshDataSource();
			LoadButtonSave();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rCarCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				ChooseBussinessParter();
				return;
			}
			int focusedRowHandle = gvDoc.FocusedRowHandle;
			string text = Function.ToString(gvDoc.GetRowCellValue(focusedRowHandle, "CardCode"));
			if (!string.IsNullOrEmpty(text))
			{
				frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
				frm_BusinessMasterData2._copyF_CodeCustomer = text;
				frm_BusinessMasterData2.MdiParent = MdiParent;
				frm_BusinessMasterData2.Show();
			}
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
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control && !gvDoc.FocusedColumn.OptionsColumn.ReadOnly)
			{
				WinCommons._OpenProcessing();
				ExtenseionMethods.PasteFromExcelToDataTable(grdDoc, gvDoc, new List<string>());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals("WhsCode"))
			{
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				if (dataSourceTable.IsNotNull() && dataSourceTable.Columns.Contains("InStoc") && dataSourceTable.Columns.Contains("InWhs"))
				{
					DataTable dataItemWhsFromWhsCode = GetDataItemWhsFromWhsCode(Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "ItemCode")), Function.ToString(e.Value));
					if (dataItemWhsFromWhsCode.IsNotNull())
					{
						dataSourceTable.Rows[gvDoc.GetDataSourceRowIndex(e.RowHandle)]["InStoc"] = dataItemWhsFromWhsCode.Rows[0]["OnHandItem"];
						dataSourceTable.Rows[gvDoc.GetDataSourceRowIndex(e.RowHandle)]["InWhs"] = dataItemWhsFromWhsCode.Rows[0]["OnHandWhs"];
						dataSourceTable.AcceptChanges();
						grdDoc.DataSource = dataSourceTable;
					}
				}
			}
			else if (e.Column.FieldName.Equals("UomCode"))
			{
				DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")), Function.ToString(gvDoc.GetFocusedRowCellValue("UomCode")));
				if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
				{
					DataTable dataSourceTable2 = grdDoc.DataSourceTable;
					int dataSourceRowIndex = gvDoc.GetDataSourceRowIndex(e.RowHandle);
					dataSourceTable2.Rows[dataSourceRowIndex]["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
					dataSourceTable2.Rows[dataSourceRowIndex]["unitMsr"] = dataUomByCode.Rows[0]["UomName"];
					dataSourceTable2.Rows[dataSourceRowIndex]["NumPerMsr"] = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
					if (Function.ToString(dataUomByCode.Rows[0]["UomName"]).Equals(Function.ToString(gvDoc.GetFocusedRowCellValue("InvUnitMsr"))))
					{
						dataSourceTable2.Rows[dataSourceRowIndex]["UseBaseUn"] = "Y";
						gvDoc.FocusedColumn = gvDoc.Columns["UseBaseUn"];
					}
					dataSourceTable2.Rows[dataSourceRowIndex]["Quantity"] = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("InvQty")) / Function.ParseDouble(gvDoc.GetFocusedRowCellValue("NumPerMsr"));
					grdDoc.DataSourceTable.AcceptChanges();
				}
			}
			else if (e.Column.FieldName.Equals("CardCode"))
			{
				DataTable dataTable = HttpClientHelperFactory.GetHttpClient().ScheduleClient.GetdataBpItem(Function.ToString(gvDoc.GetFocusedRowCellValue("CardCode")), Function.ToString(gvDoc.GetFocusedRowCellValue("ItemCode")), Function.ParseDateTimes(gvDoc.GetFocusedRowCellValue("ShipDate")).ToString("yyyyMMdd"));
				if (dataTable.IsNotNull())
				{
					gvDoc.SetFocusedRowCellValue("Currency", dataTable.Rows[0]["Currency"]);
					gvDoc.SetFocusedRowCellValue("CardName", dataTable.Rows[0]["CardName"]);
					gvDoc.SetFocusedRowCellValue("Price", dataTable.Rows[0]["Price"]);
					gvDoc.SetFocusedRowCellValue("PriceBefDi", dataTable.Rows[0]["PriceBefDi"]);
					gvDoc.SetFocusedRowCellValue("DiscPrcnt", dataTable.Rows[0]["DiscPrcnt"]);
				}
				else
				{
					gvDoc.SetFocusedRowCellValue("CardName", "");
				}
			}
			else if (e.Column.FieldName.Equals("PriceBefDi") || e.Column.FieldName.Equals("DiscPrcnt"))
			{
				gvDoc.SetFocusedRowCellValue("Price", Function.ParseDouble(e.Value) * (1.0 - Function.ParseDouble(gvDoc.GetFocusedRowCellValue("DiscPrcnt")) / 100.0));
			}
			else if (e.Column.FieldName.Equals("Quantity"))
			{
				gvDoc.SetFocusedRowCellValue("InvQty", Function.ParseDouble(gvDoc.GetFocusedRowCellValue("Quantity")) * Function.ParseDouble(gvDoc.GetFocusedRowCellValue("NumPerMsr")));
			}
			else if (e.Column.FieldName.Equals("VatGroup") && rTaxCode.GetDataSourceRowByKeyValue(e.Value) is VatGroupsImpl vatGroupsImpl)
			{
				grdDoc.DataSourceTable.Rows[gvDoc.GetFocusedDataSourceRowIndex()]["VatPrcnt"] = vatGroupsImpl.Rate;
			}
			grdDoc.DataSourceTable.AcceptChanges();
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
			string fieldName = gvDoc.FocusedColumn.FieldName;
			if (fieldName.Equals("UseBaseUn"))
			{
				double num = 0.0;
				num = Function.ParseDouble(Function.ParseDouble(gvDoc.GetFocusedRowCellValue("NumInBuy")));
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) == -1 && Math.Abs(num - 1.0) < 1E-07)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName.Equals("UomCode"))
			{
				if (Function.ToString(gvDoc.GetFocusedRowCellValue("UseBaseUn")).Equals("Y") || Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName.Equals("unitMsr") && (!Function.ToString(gvDoc.GetFocusedRowCellValue("UseBaseUn")).Equals("N") || Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")) != -1))
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

	private void rButtonEdit_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			this.DrillDownOrigin(Function.ToString(gvDoc.GetFocusedRowCellValue("U_OriginType")), Function.ToString(gvDoc.GetFocusedRowCellValue("U_OriginCode")));
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
				UomGroup = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry"))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue("UomCode", frm_CFL2.ChooseItem["UomCode"]);
				grdDoc.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rUseBaseUnits_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(e.NewValue).Equals("Y"))
			{
				gvDoc.SetFocusedRowCellValue("UomCode", gvDoc.GetFocusedRowCellValue("InvUnitMsr"));
			}
			else
			{
				GridViewEx gridViewEx = gvDoc;
				object value;
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpEntry")) == -1)
				{
					value = Function.ToString(gvDoc.GetFocusedRowCellDisplayText("UgpCode"));
				}
				else if (SetItemUomType == Enums.ItemUomType.PurchaseUom)
				{
					value = gvDoc.GetFocusedRowCellValue("BuyUnitMsr");
				}
				else
				{
					value = ((SetItemUomType == Enums.ItemUomType.SalesUom) ? gvDoc.GetFocusedRowCellValue("SalUnitMsr") : gvDoc.GetFocusedRowCellValue("InvUnitMsr"));
				}
				gridViewEx.SetFocusedRowCellValue("UomCode", value);
			}
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rProject_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Project, multiSelect: false, null);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRowView dataRowView = frm_ChooseFromList2.ChooseItem;
					gvDoc.SetFocusedRowCellValue("Project", Function.ToString(dataRowView["PrjCode"]));
				}
			}
			else
			{
				this.DrillDown(ApzObjectTypes.oProjectCodes, ApzObjectTypes.oProjectCodes, Function.ToString(gvDoc.GetFocusedRowCellValue("Project")));
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
		refreshToolStripMenuItem_Click(null, null);
	}

	public void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtCode.ReadOnly = !isEnable;
			txtCode.Properties.Buttons[0].Visible = isEnable;
			txtInput.ReadOnly = !isEnable;
			btnSave.Enabled = isEnable;
			btnCancel.Enabled = isEnable;
			gvDoc.ReadOnly = !isEnable;
			rSeri.Buttons[0].Visible = isEnable;
			rWhs.Buttons[0].Visible = isEnable;
			rWhs.Buttons[1].Visible = isEnable;
			rItemCode.Buttons[0].Visible = isEnable;
			rCarCode.Buttons[0].Visible = isEnable;
			rCarCode.Buttons[1].Visible = isEnable;
			rBran.Buttons[0].Visible = isEnable;
			contextMenuStrip.Enabled = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task LoadDataFromSchedule()
	{
	}

	protected virtual void LoadButtonSave()
	{
	}

	protected virtual async Task Save(Enums.MrpGenDocumentType type = (Enums.MrpGenDocumentType)0)
	{
	}

	private async Task GetBranch()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await http.GetAsync("GetBranchList", new
			{
				userCode = GlobalData.UserName
			});
			rBran.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void LoadDataInputType()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Schedule",
					Code = "S"
				},
				new Common.Table_Code
				{
					Name = "Mrp",
					Code = "M"
				}
			};
			txtInput.Properties.DataSource = dataSource;
			txtInput.EditValue = "S";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task GetSeries()
	{
	}

	private async Task LoadDataEmployee()
	{
		try
		{
			DataTable dataSource;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
			{
				dataSource = await http.GetAsync("GetSaleEmployee");
			}
			rEmployee.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual void ChooseBussinessParter()
	{
	}

	private DataTable GetDataItemWhsFromWhsCode(string itemCode, string whsCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
		return httpClientHelper.Get("GetDataItemOnhand", new { itemCode, whsCode });
	}

	private async void LoadColumnGrid()
	{
		try
		{
			DataTable dataTable = await GetStructureGrid();
			if (dataTable.IsNotNull())
			{
				gvDoc.CreateColumByStructure(dataTable);
				GridColumn gridColumn = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginType"));
				if (gridColumn != null)
				{
					gridColumn.ColumnEdit = rOriginType;
					gridColumn.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn2 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("U_OriginCode"));
				if (gridColumn2 != null)
				{
					gridColumn2.ColumnEdit = rOriginCode;
					gridColumn2.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn3 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("WhsCode"));
				if (gridColumn3 != null)
				{
					gridColumn3.ColumnEdit = rWhs;
					gridColumn3.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn4 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("FromWhsCod"));
				if (gridColumn4 != null)
				{
					gridColumn4.ColumnEdit = rWhs;
					gridColumn4.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn5 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Series"));
				if (gridColumn5 != null)
				{
					gridColumn5.ColumnEdit = rSeri;
					gridColumn5.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn6 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("BplID"));
				if (gridColumn6 != null)
				{
					gridColumn6.ColumnEdit = rBran;
					gridColumn6.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn7 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("CardCode"));
				if (gridColumn7 != null)
				{
					gridColumn7.ColumnEdit = rCarCode;
					gridColumn7.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn8 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("ItemCode"));
				if (gridColumn8 != null)
				{
					gridColumn8.ColumnEdit = rItemCode;
					gridColumn8.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn9 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("UgpEntry"));
				if (gridColumn9 != null)
				{
					gridColumn9.ColumnEdit = rUomGroup;
					gridColumn9.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn10 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("UseBaseUn"));
				if (gridColumn10 != null)
				{
					gridColumn10.ColumnEdit = rUseBaseUnits;
					gridColumn10.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn11 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("UomCode"));
				if (gridColumn11 != null)
				{
					gridColumn11.ColumnEdit = rUomCode;
					gridColumn11.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn12 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("VatGroup"));
				if (gridColumn12 != null)
				{
					gridColumn12.ColumnEdit = rTaxCode;
					gridColumn12.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn13 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("SlpCode"));
				if (gridColumn12 != null)
				{
					gridColumn13.ColumnEdit = rEmployee;
					gridColumn13.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
				GridColumn gridColumn14 = gvDoc.Columns.LastOrDefault((GridColumn t) => t.FieldName.Equals("Project"));
				if (gridColumn14 != null)
				{
					gridColumn14.ColumnEdit = rProject;
					gridColumn14.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual async Task<DataTable> GetStructureGrid()
	{
		throw new ArgumentException("Could not get data structure");
	}

	private async Task LoadUomGroup()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rUomGroup;
			repositoryItemLookUpEdit.DataSource = await HttpClientHelperFactory.GetHttpClient().Commons.GetDataUomGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInventoryUoM()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Yes",
					Code = "Y"
				},
				new Common.Table_Code
				{
					Name = "No",
					Code = "N"
				}
			};
			rUseBaseUnits.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTaxCode()
	{
		try
		{
			List<VatGroupsImpl> dataSource = await LoadVatGroup();
			rTaxCode.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected virtual async Task<List<VatGroupsImpl>> LoadVatGroup()
	{
		try
		{
			using HttpClientHelper<VatGroupsImpl> hp = new HttpClientHelper<VatGroupsImpl>(Configuration.ApiServer, "Commons");
			return await hp.GetListAsync("GetVatGroup", new
			{
				category = "I"
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new List<VatGroupsImpl>();
	}

	protected virtual void Code_ChooseFromList(object sender, ButtonPressedEventArgs e)
	{
		if (Function.ToString(txtInput.EditValue) == "S")
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Schedule, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					ScheduleStatus = "A",
					ScheduleType = "P"
				};
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					txtCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["PSCode"]);
				}
			}
			else
			{
				int num = Function.ParseInt(txtCode.EditValue);
				if (num != 0)
				{
					this.DrillDown(ApzObjectTypes.oSchedule, ApzObjectTypes.oSchedule, Function.ToString(num));
				}
			}
		}
		else if (e.Button.Index == 0)
		{
			frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.MRP, false, Function.ToString(txtCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "")
			{
				MrpStatus = "A",
				MrpType = "3,4"
			};
			frm_CFL3.ShowDialog(this);
			if (frm_CFL3.ChooseItem != null)
			{
				txtCode.EditValue = Function.ToString(frm_CFL3.ChooseItem["MrpCode"]);
			}
		}
		else if (Function.ParseInt(txtCode.EditValue) != 0)
		{
			frm_MRP frm_MRP2 = new frm_MRP(Function.ParseInt(txtCode.EditValue));
			frm_MRP2.MdiParent = MdiParent;
			frm_MRP2.Show();
		}
	}

	protected virtual async Task<string> GetDataCheckInput()
	{
		using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, Controller);
		return await http.GetAsync("CheckInputData", new
		{
			type = Function.ToString(txtInput.EditValue),
			code = Function.ParseInt(txtCode.EditValue),
			genType = 'P'
		});
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.Mrp.VirMrpGenDocument));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbSchedule = new Apzon.Controls.Controls.LabelLine();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.rWhs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rBran = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rSeri = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rCarCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rOriginCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rUomGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rUseBaseUnits = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rUomCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rTaxCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rEmployee = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rOriginType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rProject = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnSave = new DevExpress.XtraEditors.DropDownButton();
		this.lbInputType = new Apzon.Controls.Controls.LabelLine();
		this.txtInput = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBran).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSeri).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCarCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUomGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUseBaseUnits).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTaxCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEmployee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtInput.Properties).BeginInit();
		base.SuspendLayout();
		this.lbSchedule.EditControl = null;
		this.lbSchedule.EditValue = "Code";
		this.lbSchedule.FieldName = null;
		this.lbSchedule.HideWhenEmpty = false;
		this.lbSchedule.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSchedule.Location = new System.Drawing.Point(265, 9);
		this.lbSchedule.Name = "lbSchedule";
		this.lbSchedule.ReadOnly = false;
		this.lbSchedule.SettingInVisible = false;
		this.lbSchedule.SettingReadonly = false;
		this.lbSchedule.ShowUnderLine = true;
		this.lbSchedule.Size = new System.Drawing.Size(70, 20);
		this.lbSchedule.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSchedule.TabIndex = 0;
		this.lbSchedule.Utext = "Code";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip;
		this.grdDoc.Location = new System.Drawing.Point(1, 33);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[13]
		{
			this.rWhs, this.rItemCode, this.rBran, this.rSeri, this.rCarCode, this.rOriginCode, this.rUomGroup, this.rUseBaseUnits, this.rUomCode, this.rTaxCode,
			this.rEmployee, this.rOriginType, this.rProject
		});
		this.grdDoc.Size = new System.Drawing.Size(504, 341);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.refreshToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(114, 26);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oMrpGenDocument";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ODOCUMENT";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.rWhs.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rWhs.Name = "rWhs";
		this.rWhs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rWhs_ButtonClick);
		this.rItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.rBran.AutoHeight = false;
		this.rBran.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rBran.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rBran.DisplayMember = "Name";
		this.rBran.Name = "rBran";
		this.rBran.NullText = "";
		this.rBran.ShowFooter = false;
		this.rBran.ShowHeader = false;
		this.rBran.ValueMember = "Value";
		this.rSeri.AutoHeight = false;
		this.rSeri.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rSeri.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rSeri.DisplayMember = "Name";
		this.rSeri.Name = "rSeri";
		this.rSeri.NullText = "";
		this.rSeri.ShowFooter = false;
		this.rSeri.ShowHeader = false;
		this.rSeri.ValueMember = "Value";
		this.rCarCode.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rCarCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rCarCode.Name = "rCarCode";
		this.rCarCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rCarCode_ButtonClick);
		this.rOriginCode.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rOriginCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rOriginCode.Name = "rOriginCode";
		this.rOriginCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rButtonEdit_ButtonClick);
		this.rUomGroup.AutoHeight = false;
		this.rUomGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUomGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rUomGroup.DisplayMember = "Name";
		this.rUomGroup.Name = "rUomGroup";
		this.rUomGroup.NullText = "";
		this.rUomGroup.ShowFooter = false;
		this.rUomGroup.ShowHeader = false;
		this.rUomGroup.ValueMember = "Code";
		this.rUseBaseUnits.AutoHeight = false;
		this.rUseBaseUnits.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUseBaseUnits.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rUseBaseUnits.DisplayMember = "Name";
		this.rUseBaseUnits.Name = "rUseBaseUnits";
		this.rUseBaseUnits.NullText = "";
		this.rUseBaseUnits.ShowFooter = false;
		this.rUseBaseUnits.ShowHeader = false;
		this.rUseBaseUnits.ValueMember = "Code";
		this.rUseBaseUnits.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rUseBaseUnits_EditValueChanging);
		this.rUomCode.AutoHeight = false;
		this.rUomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rUomCode.Name = "rUomCode";
		this.rUomCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rUomCode_ButtonClick);
		this.rTaxCode.AutoHeight = false;
		this.rTaxCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rTaxCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2")
		});
		this.rTaxCode.DisplayMember = "Name";
		this.rTaxCode.Name = "rTaxCode";
		this.rTaxCode.NullText = "";
		this.rTaxCode.ValueMember = "Code";
		this.rEmployee.AutoHeight = false;
		this.rEmployee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rEmployee.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpName", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SlpCode", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rEmployee.DisplayMember = "SlpName";
		this.rEmployee.Name = "rEmployee";
		this.rEmployee.NullText = "";
		this.rEmployee.ShowFooter = false;
		this.rEmployee.ShowHeader = false;
		this.rEmployee.ValueMember = "SlpCode";
		this.rOriginType.AutoHeight = false;
		this.rOriginType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOriginType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name3", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name4")
		});
		this.rOriginType.DisplayMember = "Name";
		this.rOriginType.Name = "rOriginType";
		this.rOriginType.ShowHeader = false;
		this.rOriginType.ValueMember = "Code";
		this.rProject.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rProject.Name = "rProject";
		this.rProject.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rProject_ButtonClick);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(163, 380);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.txtCode.EditControl = null;
		this.txtCode.FieldName = null;
		this.txtCode.IsSearchControl = false;
		this.txtCode.Location = new System.Drawing.Point(335, 7);
		this.txtCode.Name = "txtCode";
		this.txtCode.Properties.AppearanceFocused.BackColor = System.Drawing.Color.PapayaWhip;
		this.txtCode.Properties.AppearanceFocused.Options.UseBackColor = true;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.txtCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtCode.SettingInVisible = false;
		this.txtCode.SettingReadonly = false;
		this.txtCode.Size = new System.Drawing.Size(150, 20);
		this.txtCode.TabIndex = 1;
		this.txtCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtSchedule_ButtonClick);
		this.txtCode.Validating += new System.ComponentModel.CancelEventHandler(this.txtCode_Validating);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(9, 380);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(148, 20);
		this.btnSave.TabIndex = 3;
		this.btnSave.Text = "Purchase Order";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.lbInputType.EditControl = null;
		this.lbInputType.EditValue = "Input From";
		this.lbInputType.FieldName = null;
		this.lbInputType.HideWhenEmpty = false;
		this.lbInputType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInputType.Location = new System.Drawing.Point(9, 9);
		this.lbInputType.Name = "lbInputType";
		this.lbInputType.ReadOnly = false;
		this.lbInputType.SettingInVisible = false;
		this.lbInputType.SettingReadonly = false;
		this.lbInputType.ShowUnderLine = true;
		this.lbInputType.Size = new System.Drawing.Size(80, 20);
		this.lbInputType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInputType.TabIndex = 6;
		this.lbInputType.Utext = "Input From";
		this.txtInput.Location = new System.Drawing.Point(89, 6);
		this.txtInput.Name = "txtInput";
		this.txtInput.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtInput.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.txtInput.Properties.DisplayMember = "Name";
		this.txtInput.Properties.NullText = "";
		this.txtInput.Properties.ShowFooter = false;
		this.txtInput.Properties.ShowHeader = false;
		this.txtInput.Properties.ValueMember = "Code";
		this.txtInput.Size = new System.Drawing.Size(160, 20);
		this.txtInput.TabIndex = 7;
		this.txtInput.EditValueChanged += new System.EventHandler(this.txtInput_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(506, 404);
		base.Controls.Add(this.lbInputType);
		base.Controls.Add(this.txtInput);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.lbSchedule);
		base.Controls.Add(this.txtCode);
		base.FormId = "oMrpGenDocument";
		base.IconOptions.ShowIcon = false;
		base.Name = "VirMrpGenDocument";
		this.Text = "Inventory Transfer for Production Schedule";
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBran).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSeri).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCarCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUomGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUseBaseUnits).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTaxCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEmployee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOriginType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtInput.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
