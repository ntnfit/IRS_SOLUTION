using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmMrp : XtraFormEx
{
	private HttpClientHelper<Mrp> _mrpClientHelper = new HttpClientHelper<Mrp>(Configuration.ApiServer, "Mrp");

	private HttpClientHelper<Mps> _mpsClientHelper = new HttpClientHelper<Mps>(Configuration.ApiServer, "Mps");

	private readonly string _isMRP = "";

	private int _colNumber_Capacity;

	private int _colNumber_MRP;

	private List<Common.ListItems> lstChangedColor = new List<Common.ListItems>();

	public Mrp obj = new Mrp();

	private DataTable tableCapacity = new DataTable();

	private DataTable tableChangedNguyen = new DataTable();

	private DataTable tableMRP = new DataTable();

	private IContainer components;

	private MemoExEdit memoEx_Notes;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btn_ChangedStatus;

	private DateEditEx date_FixDate;

	private ButtonEditEx txt_MpsCode;

	private SplitContainer splitContainer1;

	private GroupControl groupControl2;

	private GridControlEx grd_MRP;

	private GridViewEx gv_MRP;

	private GroupControl groupControl3;

	private GridControlEx grd_Capacity;

	private GridViewEx gv_Capacity;

	private GridViewEx gridView1;

	private GridViewEx gridView2;

	private TextEditEx txt_POEntry;

	private SimpleButtonEx btnRun;

	private SaveFileDialog saveFile;

	private DateEditEx date_EndDate;

	private ButtonEditEx txt_ProductionOrder;

	private ComboBoxEditEx cbx_Status;

	private TextEditEx txt_MrpCode;

	private TextEditEx txt_MrpName;

	private SimpleButtonEx btn_Export;

	private DateEditEx date_BeginDate;

	private GroupControl groupControl1;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine labelLine8;

	private LabelLine labelLine7;

	private LabelLine labelLine6;

	private LabelLine labelLine5;

	private RepositoryItemLookUpEdit repo_DocType;

	private RepositoryItemDateEdit repo_DateEdit_DueDate;

	private RepositoryItemMemoExEdit repo_MemoExEdit_Notes;

	private RepositoryItemCalcEdit repo_CalcEdit_Qty;

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
			btnSave.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btnSave.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnSave.Text = Resources.ButtonOK;
			break;
		}
	}

	public FrmMrp()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMrp"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmMrp" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void txt_MpsCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			FrmCflMps frmCflMps = new FrmCflMps(this);
			frmCflMps.mpsCode = txt_MpsCode.Text;
			frmCflMps.FormClosing += cfl_FormClosing;
			frmCflMps.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cfl_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(obj.MpsCode))
			{
				txt_MpsCode.Text = "";
				date_BeginDate.EditValue = DateTime.Today;
				date_EndDate.EditValue = DateTime.Today;
				return;
			}
			txt_MpsCode.Text = obj.MpsCode;
			Mps mps = await GetOMPS(0, obj.MpsCode);
			if (mps != null)
			{
				date_BeginDate.EditValue = mps.BeginDate;
				date_EndDate.EditValue = mps.EndDate;
				date_FixDate.EditValue = mps.FixDate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<Mps> GetOMPS(int id, string mpsCode)
	{
		try
		{
			return await _mpsClientHelper.GetAsync("GetOmps", new { id, mpsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new Mps();
	}

	private void txt_ProductionOrder_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		FrmCflProductionOrder cfl = new FrmCflProductionOrder(txt_ProductionOrder.Text);
		cfl.FormClosing += (object s, FormClosingEventArgs ev) =>
		{
			try
			{
				string empty = string.Empty;
				string empty2 = string.Empty;
				if (cfl.ChooseItems != null && cfl.ChooseItems.Count != 0)
				{
					empty += cfl.ChooseItems[0].Text;
					empty2 += cfl.ChooseItems[0].Value;
					for (int i = 1; i < cfl.ChooseItems.Count; i++)
					{
						empty = empty + "," + cfl.ChooseItems[i].Text;
						empty2 = empty2 + "," + cfl.ChooseItems[i].Value;
					}
					txt_ProductionOrder.Text = empty;
					txt_POEntry.Text = empty2;
					cfl.ChooseItems.Clear();
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
		};
		cfl.ShowDialog(this);
	}

	protected override bool ProcessDialogKey(Keys keyData)
	{
		if (keyData == Keys.Tab && IsChoosePO())
		{
			return true;
		}
		return base.ProcessDialogKey(keyData);
	}

	private bool IsChoosePO()
	{
		if (ActiveControl.GetType() == typeof(TextBoxMaskBox) && (ActiveControl as TextBoxMaskBox).OwnerEdit == txt_ProductionOrder)
		{
			FrmCflProductionOrder cfl = new FrmCflProductionOrder(txt_ProductionOrder.Text);
			cfl.FormClosing += (object s, FormClosingEventArgs ev) =>
			{
				try
				{
					string empty = string.Empty;
					string empty2 = string.Empty;
					if (cfl.ChooseItems != null && cfl.ChooseItems.Count != 0)
					{
						empty += cfl.ChooseItems[0].Text;
						empty2 += cfl.ChooseItems[0].Value;
						for (int i = 1; i < cfl.ChooseItems.Count; i++)
						{
							empty = empty + "," + cfl.ChooseItems[i].Text;
							empty2 = empty2 + "," + cfl.ChooseItems[i].Value;
						}
						txt_ProductionOrder.Text = empty;
						txt_POEntry.Text = empty2;
						cfl.ChooseItems.Clear();
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
				}
			};
			cfl.ShowDialog(this);
			return true;
		}
		return false;
	}

	private async void FrmMrp_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			repo_DocType.DataSource = GetDocType();
			repo_DocType.DisplayMember = "Text";
			repo_DocType.ValueMember = "Value";
			WinFuntions._addComboBox(cbx_Status, GetStatus());
			if (obj != null && obj.Id > 0)
			{
				txt_MrpCode.Text = obj.MrpCode;
				txt_MrpName.Text = obj.MrpName;
				txt_MpsCode.Text = obj.MpsCode;
				WinFuntions._selectItemComboBoxByValue(cbx_Status, obj.Status.ToString());
				date_BeginDate.EditValue = obj.BeginDate;
				date_EndDate.EditValue = obj.EndDate;
				date_FixDate.EditValue = obj.FixDate;
				memoEx_Notes.Text = obj.Notes;
				txt_MrpCode.Properties.ReadOnly = true;
				LoadColumn();
				await LoadData();
				if (obj.Status == 2)
				{
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			else
			{
				TextEditEx textEditEx = txt_MrpCode;
				textEditEx.Text = await GetCodeOMRP();
				cbx_Status.SelectedIndex = 0;
				FormMode = Enums.FormViewModeType.Add;
			}
			if (!string.IsNullOrEmpty(_isMRP) && _isMRP.Equals("MRP"))
			{
				DataTable dtHeader = await GetMRPHeader();
				if (dtHeader != null && dtHeader.Rows.Count > 0)
				{
					string absEntry = dtHeader.Rows[0]["AbsEntry"].ToString();
					TextEditEx textEditEx = txt_MrpCode;
					textEditEx.EditValue = await GetCodeOMRP();
					txt_MrpName.EditValue = dtHeader.Rows[0]["Descr"].ToString();
					txt_MpsCode.EditValue = dtHeader.Rows[0]["Code"].ToString();
					date_BeginDate.EditValue = DateTime.Parse(dtHeader.Rows[0]["StartDate"].ToString());
					date_EndDate.EditValue = DateTime.Parse(dtHeader.Rows[0]["EndDate"].ToString());
					date_FixDate.EditValue = DateTime.Parse(dtHeader.Rows[0]["FixDate"].ToString());
					WinFuntions._selectItemComboBoxByValue(cbx_Status, "1");
					LoadColumn();
					LoadData_MRP_SAP(absEntry, DateTime.Parse(date_BeginDate.EditValue.ToString()), DateTime.Parse(date_EndDate.EditValue.ToString()), txt_MpsCode.Text.Trim());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void LoadData_MRP_SAP(string id, DateTime beginDate, DateTime endDate, string mpsCode)
	{
		_ = 7;
		try
		{
			tableMRP = CreateTableMRP();
			obj.MpsCode = txt_MpsCode.Text;
			obj.BeginDate = DateTime.Parse(date_BeginDate.EditValue.ToString());
			obj.EndDate = DateTime.Parse(date_EndDate.EditValue.ToString());
			obj.PoEntry = txt_POEntry.Text;
			new DataTable();
			DataTable dt = await SP_APZ_MRPRun_SAP(id, beginDate, endDate, mpsCode);
			if (dt != null && dt.Rows.Count > 0)
			{
				List<string> lst_ItemCode = new List<string>();
				dt.BeginLoadData();
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string text = dt.Rows[i]["ItemCode"].ToString().Trim();
					if (lst_ItemCode.Count == 0)
					{
						lst_ItemCode.Add(text);
					}
					bool flag = false;
					for (int j = 0; j < lst_ItemCode.Count; j++)
					{
						if (lst_ItemCode[j].Trim().Equals(text))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						lst_ItemCode.Add(text);
					}
				}
				dt.EndLoadData();
				await AddRowByType_SAP(dt, tableMRP, TypeMRP.Demand, lst_ItemCode);
				await AddRowByType_SAP(dt, tableMRP, TypeMRP.OrderPlan, lst_ItemCode);
				await AddRowByType_SAP(dt, tableMRP, TypeMRP.Receivedplan, lst_ItemCode);
				await AddRowByType_SAP(dt, tableMRP, TypeMRP.StockPlan, lst_ItemCode);
				await AddRowByType_SAP(dt, tableMRP, TypeMRP.Requirement, lst_ItemCode);
			}
			grd_MRP.DataSource = tableMRP;
			grd_MRP.RefreshDataSource();
			tableCapacity = CreateTableCapacity();
			new DataTable();
			dt = ((obj == null || obj.Id <= 0) ? (await SP_APZ_MRPRun_Capacity(obj.MpsCode, obj.BeginDate.Value, obj.EndDate.Value)) : (await SP_APZ_GetMRPCapactity(obj.Id)));
			if (dt != null && dt.Rows.Count > 0)
			{
				List<string> list = new List<string>();
				dt.BeginLoadData();
				for (int k = 0; k < dt.Rows.Count; k++)
				{
					string text2 = dt.Rows[k]["Item"].ToString().Trim();
					if (list.Count == 0)
					{
						list.Add(text2);
					}
					bool flag2 = false;
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].Trim().Equals(text2))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						list.Add(text2);
					}
				}
				dt.EndLoadData();
				AddRowByTypeCapactity(dt, tableCapacity, TypePOCapacity.AvailableCapacity, list);
				AddRowByTypeCapactity(dt, tableCapacity, TypePOCapacity.RequirementCapacity, list);
				AddRowByTypeCapactity(dt, tableCapacity, TypePOCapacity.CapacityLoad, list);
			}
			grd_Capacity.DataSource = tableCapacity;
			grd_Capacity.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddRowByType_SAP(DataTable dt, DataTable table, int type, List<string> lstItem)
	{
		try
		{
			for (int i = 0; i < lstItem.Count; i++)
			{
				DataRow[] array = dt.Select("ItemCode = '" + lstItem[i] + "' and Type = " + type);
				if (array == null || array.Length == 0)
				{
					DataRow rows = table.NewRow();
					DataTable dataTable = await GetOITMByItemCode(lstItem[i]);
					if (dataTable == null || dataTable.Rows.Count <= 0)
					{
						continue;
					}
					rows["ItemCode"] = lstItem[i];
					rows["ItemName"] = dataTable.Rows[0]["ItemName"];
					rows["Type"] = type;
					switch (type)
					{
					case 1:
						rows["TypeName"] = "1. Demand";
						break;
					case 2:
						rows["TypeName"] = "2. Order Plan";
						break;
					case 3:
						rows["TypeName"] = "3. Receipt Plan";
						break;
					case 4:
						rows["TypeName"] = "4. Stock";
						break;
					case 5:
						rows["TypeName"] = "5. Requirment";
						break;
					}
					rows["Notes"] = "";
					rows["Line"] = dataTable.Rows[0]["U_Line"].ToString();
					rows["LTime"] = (string.IsNullOrEmpty(dataTable.Rows[0]["LeadTime"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[0]["LeadTime"].ToString()));
					rows["DocumentType"] = dataTable.Rows[0]["PrcrmntMtd"];
					rows["MachineTime"] = (string.IsNullOrEmpty(dataTable.Rows[0]["U_TimeM"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[0]["U_TimeM"].ToString()));
					rows["NumMachine"] = (string.IsNullOrEmpty(dataTable.Rows[0]["U_NumberM"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[0]["U_NumberM"].ToString()));
					for (int j = 0; j < _colNumber_MRP; j++)
					{
						try
						{
							rows["OB"] = 0;
							rows[j.ToString()] = 0;
						}
						catch (Exception)
						{
							rows["OB"] = 0;
							rows[j.ToString()] = 0;
						}
					}
					table.Rows.Add(rows);
				}
				else
				{
					if (array == null || array.Length == 0)
					{
						continue;
					}
					DataTable dataTable2 = array.CopyToDataTable();
					DataRow dataRow = table.NewRow();
					dataTable2.Rows[0]["ItemCode"].ToString();
					dataRow["ItemCode"] = dataTable2.Rows[0]["ItemCode"];
					dataRow["ItemName"] = dataTable2.Rows[0]["ItemName"];
					dataRow["Type"] = dataTable2.Rows[0]["Type"];
					dataRow["TypeName"] = dataTable2.Rows[0]["TypeName"];
					dataRow["Notes"] = dataTable2.Rows[0]["Notes"];
					dataRow["Line"] = dataTable2.Rows[0]["Line"];
					dataRow["LTime"] = (string.IsNullOrEmpty(dataTable2.Rows[0]["LTime"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[0]["LTime"].ToString()));
					dataRow["DocumentType"] = dataTable2.Rows[0]["DocumentType"];
					dataRow["MachineTime"] = (string.IsNullOrEmpty(dataTable2.Rows[0]["MachineTime"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[0]["MachineTime"].ToString()));
					dataRow["NumMachine"] = (string.IsNullOrEmpty(dataTable2.Rows[0]["NumMachine"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[0]["NumMachine"].ToString()));
					dataTable2.BeginLoadData();
					DateTime dateTime = DateTime.Parse(date_BeginDate.EditValue.ToString()).AddDays(-1.0);
					for (int k = 0; k < _colNumber_MRP; k++)
					{
						DataRow[] array2 = dataTable2.Select("DueDate = '" + dateTime.AddDays(k).ToString("MM/dd/yyyy") + "'");
						if (array2 == null || array2.Length == 0)
						{
							dataRow["OB"] = 0;
							dataRow[k.ToString()] = 0;
						}
						else
						{
							dataRow["OB"] = (string.IsNullOrEmpty(array2[0]["OB"].ToString()) ? 0.0 : double.Parse(array2[0]["OB"].ToString()));
							dataRow[k.ToString()] = (string.IsNullOrEmpty(array2[0]["Qty"].ToString()) ? 0.0 : double.Parse(array2[0]["Qty"].ToString()));
						}
					}
					dataTable2.EndLoadData();
					table.Rows.Add(dataRow);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private async Task<DataTable> SP_APZ_MRPRun_SAP(string id, DateTime beginDate, DateTime endDate, string mpsCode)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("SP_APZ_MRPRun_SAP", new { id, beginDate, endDate, mpsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> GetMRPHeader()
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("GetMrpHeader");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<string> GetCodeOMRP()
	{
		try
		{
			return await _mrpClientHelper.GetStringAsync("GetCodeMrp");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return string.Empty;
	}

	private void LoadColumn()
	{
		try
		{
			gv_MRP.Columns.Clear();
			GridColumn gridColumn = gv_MRP.Columns.Add();
			gridColumn.Caption = "Item Code";
			gridColumn.FieldName = "ItemCode";
			gridColumn.Fixed = FixedStyle.Left;
			gridColumn.MinWidth = 50;
			gridColumn.Name = "cl_ItemCode";
			gridColumn.GroupIndex = 0;
			gridColumn.OptionsColumn.AllowEdit = false;
			gridColumn.OptionsColumn.AllowMove = false;
			gridColumn.OptionsColumn.AllowSize = true;
			gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			GridColumn gridColumn2 = gv_MRP.Columns.Add();
			gridColumn2.Caption = "Description";
			gridColumn2.FieldName = "ItemName";
			gridColumn2.Name = "cl_Description";
			gridColumn2.Visible = false;
			GridColumn gridColumn3 = gv_MRP.Columns.Add();
			gridColumn3.Caption = "Document Type";
			gridColumn3.FieldName = "DocumentType";
			gridColumn3.Name = "cl_DocumentType";
			gridColumn3.Visible = false;
			GridColumn gridColumn4 = gv_MRP.Columns.Add();
			gridColumn4.Caption = "MachineTime";
			gridColumn4.FieldName = "MachineTime";
			gridColumn4.Name = "cl_MachineTime";
			gridColumn4.Visible = false;
			GridColumn gridColumn5 = gv_MRP.Columns.Add();
			gridColumn5.Caption = "NumMachine";
			gridColumn5.FieldName = "NumMachine";
			gridColumn5.Name = "cl_NumMachine";
			gridColumn5.Visible = false;
			GridColumn gridColumn6 = gv_MRP.Columns.Add();
			gridColumn6.Caption = "LeadTime";
			gridColumn6.FieldName = "LTime";
			gridColumn6.Name = "cl_LeadTime";
			gridColumn6.Visible = false;
			GridColumn gridColumn7 = gv_MRP.Columns.Add();
			gridColumn7.Caption = "Type";
			gridColumn7.FieldName = "Type";
			gridColumn7.Name = "cl_Type";
			gridColumn7.Visible = false;
			GridColumn gridColumn8 = gv_MRP.Columns.Add();
			gridColumn8.Caption = "Type Name";
			gridColumn8.FieldName = "TypeName";
			gridColumn8.Fixed = FixedStyle.Left;
			gridColumn8.MinWidth = 150;
			gridColumn8.Name = "cl_TypeName";
			gridColumn8.OptionsColumn.AllowEdit = false;
			gridColumn8.OptionsColumn.AllowMove = false;
			gridColumn8.OptionsColumn.AllowSize = true;
			gridColumn8.OptionsColumn.AllowSort = DefaultBoolean.False;
			gridColumn8.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn8.Visible = true;
			gridColumn8.VisibleIndex = 0;
			gridColumn8.Width = 150;
			GridColumn gridColumn9 = gv_MRP.Columns.Add();
			gridColumn9.Caption = "Line";
			gridColumn9.FieldName = "Line";
			gridColumn9.Name = "cl_Line";
			gridColumn9.Visible = true;
			gridColumn9.OptionsColumn.AllowEdit = false;
			gridColumn9.OptionsColumn.AllowMove = false;
			gridColumn9.OptionsColumn.AllowSort = DefaultBoolean.False;
			gridColumn9.Fixed = FixedStyle.Left;
			gridColumn9.VisibleIndex = 1;
			gridColumn9.Width = 80;
			gridColumn9.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			GridColumn gridColumn10 = gv_MRP.Columns.Add();
			gridColumn10.Caption = "Opening Balance";
			gridColumn10.Fixed = FixedStyle.Left;
			gridColumn10.DisplayFormat.FormatType = FormatType.Numeric;
			gridColumn10.DisplayFormat.FormatString = "n2";
			gridColumn10.FieldName = "OB";
			gridColumn10.OptionsColumn.AllowSort = DefaultBoolean.False;
			gridColumn10.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn10.Width = 80;
			gridColumn10.Name = "cl_OB";
			gridColumn10.OptionsColumn.AllowEdit = false;
			gridColumn10.OptionsColumn.AllowMove = false;
			gridColumn10.VisibleIndex = 2;
			DateTime dateTime = DateTime.Parse(date_FixDate.EditValue.ToString());
			DateTime dateTime2 = DateTime.Parse(date_BeginDate.EditValue.ToString()).AddDays(-1.0);
			DateTime dateTime3 = DateTime.Parse(date_EndDate.EditValue.ToString());
			int num = 0;
			while (dateTime2 <= dateTime3)
			{
				if (num == 0)
				{
					GridColumn gridColumn11 = gv_MRP.Columns.Add();
					gridColumn11.Caption = "O. Due";
					gridColumn11.Tag = dateTime2.ToString("dd/MM/yyyy");
					if (dateTime2.DayOfWeek == DayOfWeek.Sunday)
					{
						gridColumn11.AppearanceCell.BackColor = Color.Salmon;
					}
					else if (dateTime2.DayOfWeek == DayOfWeek.Saturday)
					{
						gridColumn11.AppearanceCell.BackColor = Color.GreenYellow;
					}
					gridColumn11.DisplayFormat.FormatType = FormatType.Numeric;
					gridColumn11.DisplayFormat.FormatString = "n2";
					gridColumn11.FieldName = num.ToString();
					gridColumn11.MinWidth = 50;
					gridColumn11.Width = 50;
					gridColumn11.Name = "cl_" + num;
					gridColumn11.OptionsColumn.AllowEdit = false;
					gridColumn11.OptionsColumn.AllowMove = false;
					gridColumn11.OptionsColumn.AllowSort = DefaultBoolean.False;
					gridColumn11.OptionsFilter.AllowFilter = false;
					gridColumn11.VisibleIndex = 3 + num;
					num++;
					dateTime2 = dateTime2.AddDays(1.0);
					continue;
				}
				GridColumn gridColumn12 = gv_MRP.Columns.Add();
				gridColumn12.Caption = dateTime2.ToString("dd/MM/yyyy");
				if (dateTime < dateTime2)
				{
					gridColumn12.AppearanceCell.BackColor = Color.Pink;
				}
				if (dateTime2.DayOfWeek == DayOfWeek.Sunday)
				{
					gridColumn12.AppearanceCell.BackColor = Color.Salmon;
				}
				else if (dateTime2.DayOfWeek == DayOfWeek.Saturday)
				{
					gridColumn12.AppearanceCell.BackColor = Color.GreenYellow;
				}
				RepositoryItemCalcEdit repositoryItemCalcEdit = new RepositoryItemCalcEdit();
				repositoryItemCalcEdit.AutoHeight = false;
				repositoryItemCalcEdit.Buttons.Clear();
				repositoryItemCalcEdit.DisplayFormat.FormatString = "n2";
				repositoryItemCalcEdit.DisplayFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.EditFormat.FormatString = "n2";
				repositoryItemCalcEdit.EditFormat.FormatType = FormatType.Numeric;
				repositoryItemCalcEdit.Mask.EditMask = "n2";
				repositoryItemCalcEdit.AllowMouseWheel = false;
				gridColumn12.ColumnEdit = repositoryItemCalcEdit;
				gridColumn12.DisplayFormat.FormatType = FormatType.Numeric;
				gridColumn12.DisplayFormat.FormatString = "n2";
				gridColumn12.FieldName = num.ToString();
				gridColumn12.MinWidth = 50;
				gridColumn12.Width = 50;
				gridColumn12.Name = "cl_" + num;
				gridColumn12.OptionsColumn.AllowEdit = true;
				gridColumn12.OptionsColumn.AllowMove = false;
				gridColumn12.OptionsColumn.AllowSort = DefaultBoolean.False;
				gridColumn12.OptionsFilter.AllowFilter = false;
				gridColumn12.VisibleIndex = num + 3;
				num++;
				dateTime2 = dateTime2.AddDays(1.0);
			}
			GridColumn gridColumn13 = gv_MRP.Columns.Add();
			gridColumn13.ColumnEdit = new RepositoryItemMemoExEdit
			{
				ShowIcon = false
			};
			gridColumn13.Caption = "Remarks";
			gridColumn13.FieldName = "Notes";
			gridColumn13.MinWidth = 150;
			gridColumn13.Name = "cl_Notes";
			gridColumn13.OptionsColumn.AllowEdit = true;
			gridColumn13.OptionsColumn.AllowMove = false;
			gridColumn13.OptionsColumn.AllowSize = true;
			gridColumn13.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn13.Visible = true;
			gridColumn13.VisibleIndex = num + 4;
			gridColumn13.Width = 150;
			gv_Capacity.Columns.Clear();
			GridColumn gridColumn14 = gv_Capacity.Columns.Add();
			gridColumn14.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn14.Caption = "ID";
			gridColumn14.FieldName = "ID";
			gridColumn14.Name = "gcId";
			gridColumn14.Visible = false;
			gridColumn14.OptionsColumn.AllowMerge = DefaultBoolean.False;
			gridColumn14.OptionsColumn.AllowMove = false;
			GridColumn gridColumn15 = gv_Capacity.Columns.Add();
			gridColumn15.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn15.Caption = "MRPID";
			gridColumn15.FieldName = "MRPID";
			gridColumn15.Name = "gcMrpCode";
			gridColumn15.Visible = false;
			gridColumn15.OptionsColumn.AllowMerge = DefaultBoolean.False;
			gridColumn15.OptionsColumn.AllowMove = false;
			GridColumn gridColumn16 = gv_Capacity.Columns.Add();
			gridColumn16.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn16.Caption = "Resource";
			gridColumn16.FieldName = "Item";
			gridColumn16.Name = "gcItem";
			gridColumn16.GroupIndex = 0;
			gridColumn16.Visible = true;
			gridColumn16.OptionsColumn.AllowMerge = DefaultBoolean.False;
			gridColumn16.OptionsColumn.AllowMove = false;
			GridColumn gridColumn17 = gv_Capacity.Columns.Add();
			gridColumn17.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn17.Caption = "Description";
			gridColumn17.FieldName = "TypeName";
			gridColumn17.Name = "gcTypeName";
			gridColumn17.Visible = true;
			gridColumn17.VisibleIndex = 0;
			gridColumn17.Width = 330;
			gridColumn17.OptionsColumn.AllowSort = DefaultBoolean.False;
			gridColumn17.Fixed = FixedStyle.Left;
			gridColumn17.OptionsColumn.AllowMerge = DefaultBoolean.False;
			gridColumn17.OptionsColumn.AllowMove = false;
			GridColumn gridColumn18 = gv_Capacity.Columns.Add();
			gridColumn18.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
			gridColumn18.Caption = "Type";
			gridColumn18.FieldName = "Type";
			gridColumn18.Name = "gColType";
			gridColumn18.Visible = false;
			RepositoryItemCalcEdit repositoryItemCalcEdit2 = new RepositoryItemCalcEdit();
			repositoryItemCalcEdit2.Buttons.Clear();
			repositoryItemCalcEdit2.DisplayFormat.FormatString = "n2";
			repositoryItemCalcEdit2.DisplayFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.EditFormat.FormatString = "n2";
			repositoryItemCalcEdit2.EditFormat.FormatType = FormatType.Numeric;
			repositoryItemCalcEdit2.Mask.EditMask = "n2";
			repositoryItemCalcEdit2.AllowMouseWheel = false;
			dateTime2 = DateTime.Parse(date_BeginDate.EditValue.ToString()).AddDays(-1.0);
			dateTime3 = DateTime.Parse(date_EndDate.EditValue.ToString());
			int num2 = 0;
			while (dateTime2 <= dateTime3)
			{
				if (num2 == 0)
				{
					GridColumn gridColumn19 = gv_Capacity.Columns.Add();
					gridColumn19.Caption = "O. Due";
					gridColumn19.Tag = dateTime2.ToString("dd/MM/yyyy");
					if (dateTime2.DayOfWeek == DayOfWeek.Sunday)
					{
						gridColumn19.AppearanceCell.BackColor = Color.Salmon;
					}
					else if (dateTime2.DayOfWeek == DayOfWeek.Saturday)
					{
						gridColumn19.AppearanceCell.BackColor = Color.GreenYellow;
					}
					gridColumn19.GroupFormat.FormatType = FormatType.Numeric;
					gridColumn19.DisplayFormat.FormatType = FormatType.Numeric;
					gridColumn19.DisplayFormat.FormatString = "n2";
					gridColumn19.FieldName = num2.ToString();
					gridColumn19.Width = 50;
					gridColumn19.Name = "cl_Capacity" + num;
					gridColumn19.OptionsColumn.AllowEdit = false;
					gridColumn19.OptionsColumn.AllowMove = false;
					gridColumn19.OptionsColumn.AllowMerge = DefaultBoolean.False;
					gridColumn19.OptionsColumn.AllowSort = DefaultBoolean.False;
					gridColumn19.OptionsFilter.AllowFilter = false;
					gridColumn19.VisibleIndex = 1 + num2;
					num2++;
					dateTime2 = dateTime2.AddDays(1.0);
					continue;
				}
				GridColumn gridColumn20 = gv_Capacity.Columns.Add();
				gridColumn20.Caption = dateTime2.ToString("dd/MM/yyyy");
				if (dateTime < dateTime2)
				{
					gridColumn20.AppearanceCell.BackColor = Color.Pink;
				}
				if (dateTime2.DayOfWeek == DayOfWeek.Sunday)
				{
					gridColumn20.AppearanceCell.BackColor = Color.Salmon;
				}
				else if (dateTime2.DayOfWeek == DayOfWeek.Saturday)
				{
					gridColumn20.AppearanceCell.BackColor = Color.GreenYellow;
				}
				gridColumn20.ColumnEdit = repositoryItemCalcEdit2;
				gridColumn20.GroupFormat.FormatType = FormatType.Numeric;
				gridColumn20.DisplayFormat.FormatType = FormatType.Numeric;
				gridColumn20.DisplayFormat.FormatString = "n2";
				gridColumn20.FieldName = num2.ToString();
				gridColumn20.Width = 50;
				gridColumn20.Name = "cl_Capacity" + num;
				gridColumn20.OptionsColumn.AllowEdit = false;
				gridColumn20.OptionsColumn.AllowMove = false;
				gridColumn20.OptionsColumn.AllowMerge = DefaultBoolean.False;
				gridColumn20.OptionsColumn.AllowSort = DefaultBoolean.False;
				gridColumn20.OptionsFilter.AllowFilter = false;
				gridColumn20.VisibleIndex = 1 + num2;
				num2++;
				dateTime2 = dateTime2.AddDays(1.0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable CreateTableMRP()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("ItemCode", typeof(string));
		dataTable.Columns.Add("ItemName", typeof(string));
		dataTable.Columns.Add("Type", typeof(int));
		dataTable.Columns.Add("OB", typeof(decimal));
		dataTable.Columns.Add("Planning", typeof(string));
		dataTable.Columns.Add("DocumentType", typeof(string));
		dataTable.Columns.Add("TypeName", typeof(string));
		dataTable.Columns.Add("Line", typeof(string));
		dataTable.Columns.Add("Notes", typeof(string));
		dataTable.Columns.Add("LTime", typeof(int));
		dataTable.Columns.Add("MachineTime", typeof(double));
		dataTable.Columns.Add("NumMachine", typeof(double));
		DateTime dateTime = DateTime.Parse(date_BeginDate.EditValue.ToString()).AddDays(-1.0);
		DateTime dateTime2 = DateTime.Parse(date_EndDate.EditValue.ToString());
		int num = 0;
		while (dateTime <= dateTime2)
		{
			dataTable.Columns.Add(num.ToString(), typeof(double));
			num++;
			dateTime = dateTime.AddDays(1.0);
		}
		_colNumber_MRP = num;
		return dataTable;
	}

	private DataTable CreateTableCapacity()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Type", typeof(string));
		dataTable.Columns.Add("WorkDay", typeof(DateTime));
		dataTable.Columns.Add("Qty", typeof(double));
		dataTable.Columns.Add("Item", typeof(string));
		dataTable.Columns.Add("TypeName", typeof(string));
		dataTable.Columns.Add("Notes", typeof(string));
		dataTable.Columns.Add("MRPID", typeof(int));
		DateTime dateTime = DateTime.Parse(date_BeginDate.EditValue.ToString()).AddDays(-1.0);
		DateTime dateTime2 = DateTime.Parse(date_EndDate.EditValue.ToString());
		int num = 0;
		while (dateTime <= dateTime2)
		{
			dataTable.Columns.Add(num.ToString(), typeof(double));
			num++;
			dateTime = dateTime.AddDays(1.0);
		}
		_colNumber_Capacity = num;
		return dataTable;
	}

	private async Task LoadData()
	{
		_ = 9;
		try
		{
			tableMRP = CreateTableMRP();
			obj.MpsCode = txt_MpsCode.Text;
			obj.BeginDate = DateTime.Parse(date_BeginDate.EditValue.ToString());
			obj.EndDate = DateTime.Parse(date_EndDate.EditValue.ToString());
			obj.PoEntry = txt_POEntry.Text;
			new DataTable();
			DataTable dt;
			if (obj != null && obj.Id > 0)
			{
				dt = await SP_APZ_GetMRP(obj.Id);
			}
			else
			{
				string item = await GetItemByMpsCode(obj.MpsCode);
				dt = await MrpRun(obj, item);
			}
			if (dt != null && dt.Rows.Count > 0)
			{
				List<string> lst_ItemCode = new List<string>();
				dt.BeginLoadData();
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string text = dt.Rows[i]["ItemCode"].ToString().Trim();
					if (lst_ItemCode.Count == 0)
					{
						lst_ItemCode.Add(text);
					}
					bool flag = false;
					for (int j = 0; j < lst_ItemCode.Count; j++)
					{
						if (lst_ItemCode[j].Trim().Equals(text))
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						lst_ItemCode.Add(text);
					}
				}
				dt.EndLoadData();
				await AddRowByType(dt, tableMRP, TypeMRP.Demand, lst_ItemCode);
				await AddRowByType(dt, tableMRP, TypeMRP.OrderPlan, lst_ItemCode);
				await AddRowByType(dt, tableMRP, TypeMRP.Receivedplan, lst_ItemCode);
				await AddRowByType(dt, tableMRP, TypeMRP.StockPlan, lst_ItemCode);
				await AddRowByType(dt, tableMRP, TypeMRP.Requirement, lst_ItemCode);
			}
			grd_MRP.DataSource = tableMRP;
			grd_MRP.RefreshDataSource();
			tableCapacity = CreateTableCapacity();
			new DataTable();
			dt = ((obj == null || obj.Id <= 0) ? (await SP_APZ_MRPRun_Capacity(obj.MpsCode, obj.BeginDate.Value, obj.EndDate.Value)) : (await SP_APZ_GetMRPCapactity(obj.Id)));
			if (dt != null && dt.Rows.Count > 0)
			{
				List<string> list = new List<string>();
				dt.BeginLoadData();
				for (int k = 0; k < dt.Rows.Count; k++)
				{
					string text2 = dt.Rows[k]["Item"].ToString().Trim();
					if (list.Count == 0)
					{
						list.Add(text2);
					}
					bool flag2 = false;
					for (int l = 0; l < list.Count; l++)
					{
						if (list[l].Trim().Equals(text2))
						{
							flag2 = true;
							break;
						}
					}
					if (!flag2)
					{
						list.Add(text2);
					}
				}
				dt.EndLoadData();
				AddRowByTypeCapactity(dt, tableCapacity, TypePOCapacity.AvailableCapacity, list);
				AddRowByTypeCapactity(dt, tableCapacity, TypePOCapacity.RequirementCapacity, list);
				AddRowByTypeCapactity(dt, tableCapacity, TypePOCapacity.CapacityLoad, list);
			}
			grd_Capacity.DataSource = tableCapacity;
			grd_Capacity.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddRowByTypeCapactity(DataTable dt, DataTable table, int type, List<string> lstItem)
	{
		try
		{
			for (int i = 0; i < lstItem.Count; i++)
			{
				DataRow[] array = dt.Select("Item = '" + lstItem[i] + "' and Type = " + type);
				if (array != null && array.Length != 0)
				{
					DataTable dataTable = array.CopyToDataTable();
					DataRow dataRow = table.NewRow();
					dataRow["Item"] = dataTable.Rows[0]["Item"];
					dataRow["TypeName"] = dataTable.Rows[0]["TypeName"];
					dataRow["Type"] = dataTable.Rows[0]["Type"];
					dataTable.BeginLoadData();
					for (int j = 0; j < dataTable.Rows.Count; j++)
					{
						dataRow[j.ToString()] = (string.IsNullOrEmpty(dataTable.Rows[j]["Qty"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[j]["Qty"].ToString()));
					}
					dataTable.EndLoadData();
					table.Rows.Add(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> SP_APZ_MRPRun_Capacity(string mrpCode, DateTime fromDate, DateTime toDate)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("SP_APZ_MRPRun_Capacity", new { mrpCode, fromDate, toDate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> SP_APZ_GetMRPCapactity(long mrpId)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("SP_APZ_GetMRPCapactity", new { mrpId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task AddRowByType(DataTable dt, DataTable table, int type, List<string> lstItem)
	{
		try
		{
			for (int i = 0; i < lstItem.Count; i++)
			{
				DataRow[] array = dt.Select("ItemCode = '" + lstItem[i] + "' and Type = " + type);
				if (array == null || array.Length == 0)
				{
					DataRow rows = table.NewRow();
					DataTable dataTable = await GetOITMByItemCode(lstItem[i]);
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						rows["ItemCode"] = lstItem[i];
						rows["ItemName"] = dataTable.Rows[0]["ItemName"];
						rows["Type"] = type;
						switch (type)
						{
						case 1:
							rows["TypeName"] = "1. Demand";
							break;
						case 2:
							rows["TypeName"] = "2. Order Plan";
							break;
						case 3:
							rows["TypeName"] = "3. Receipt Plan";
							break;
						case 4:
							rows["TypeName"] = "4. Stock";
							break;
						case 5:
							rows["TypeName"] = "5. Requirment";
							break;
						}
						rows["Notes"] = "";
						rows["Line"] = dataTable.Rows[0]["U_Line"].ToString();
						rows["LTime"] = (string.IsNullOrEmpty(dataTable.Rows[0]["LeadTime"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[0]["LeadTime"].ToString()));
						rows["DocumentType"] = dataTable.Rows[0]["PrcrmntMtd"];
						rows["MachineTime"] = (string.IsNullOrEmpty(dataTable.Rows[0]["U_TimeM"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[0]["U_TimeM"].ToString()));
						rows["NumMachine"] = (string.IsNullOrEmpty(dataTable.Rows[0]["U_NumberM"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[0]["U_NumberM"].ToString()));
						for (int j = 0; j < _colNumber_MRP; j++)
						{
							rows["OB"] = 0;
							rows[j.ToString()] = 0;
						}
						table.Rows.Add(rows);
					}
				}
				else if (array != null && array.Length != 0)
				{
					DataTable dataTable2 = array.CopyToDataTable();
					DataRow dataRow = table.NewRow();
					dataTable2.Rows[0]["ItemCode"].ToString();
					dataRow["ItemCode"] = dataTable2.Rows[0]["ItemCode"];
					dataRow["ItemName"] = dataTable2.Rows[0]["ItemName"];
					dataRow["Type"] = dataTable2.Rows[0]["Type"];
					dataRow["TypeName"] = dataTable2.Rows[0]["TypeName"];
					dataRow["Notes"] = dataTable2.Rows[0]["Notes"];
					dataRow["Line"] = dataTable2.Rows[0]["Line"];
					dataRow["LTime"] = (string.IsNullOrEmpty(dataTable2.Rows[0]["LTime"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[0]["LTime"].ToString()));
					dataRow["DocumentType"] = dataTable2.Rows[0]["DocumentType"];
					dataRow["MachineTime"] = (string.IsNullOrEmpty(dataTable2.Rows[0]["MachineTime"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[0]["MachineTime"].ToString()));
					dataRow["NumMachine"] = (string.IsNullOrEmpty(dataTable2.Rows[0]["NumMachine"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[0]["NumMachine"].ToString()));
					dataTable2.BeginLoadData();
					for (int k = 0; k < dataTable2.Rows.Count; k++)
					{
						dataRow["OB"] = (string.IsNullOrEmpty(dataTable2.Rows[k]["OB"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[k]["OB"].ToString()));
						dataRow[k.ToString()] = (string.IsNullOrEmpty(dataTable2.Rows[k]["Qty"].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[k]["Qty"].ToString()));
					}
					dataTable2.EndLoadData();
					table.Rows.Add(dataRow);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetOITMByItemCode(string itemCode)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("GetOitmByItemCode", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<DataTable> MrpRun(Mrp obj, string item)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsJsonAsync("MrpRun", new Tuple<Mrp, string>(obj, item));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private async Task<string> GetItemByMpsCode(string mpsCode)
	{
		try
		{
			return await _mrpClientHelper.GetStringAsync("GetItemByMpsCode", new { mpsCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return string.Empty;
	}

	private async Task<DataTable> SP_APZ_GetMRP(long mrpId)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("SP_APZ_GetMRP", new { mrpId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	public static List<Common.Table_ID> GetStatus()
	{
		return new List<Common.Table_ID>
		{
			new Common.Table_ID
			{
				ID = 1.ToString(),
				Name = "Draft"
			},
			new Common.Table_ID
			{
				ID = 2.ToString(),
				Name = "Aprroved"
			},
			new Common.Table_ID
			{
				ID = (-1).ToString(),
				Name = "Reject"
			}
		};
	}

	public static List<Common.ListItems> GetDocType()
	{
		return new List<Common.ListItems>
		{
			new Common.ListItems
			{
				Value = "Purchase Order",
				Text = "Purchase Order"
			},
			new Common.ListItems
			{
				Value = "Production Order",
				Text = "Production Order"
			}
		};
	}

	private async void btn_ChangedStatus_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string text = ((Common.ListItems)cbx_Status.SelectedItem).Value.Trim();
			string text2 = null;
			if (text.Equals("1"))
			{
				text2 = await UpdateStatus(obj.Id.ToString(), 2.ToString());
				if (string.IsNullOrEmpty(text2))
				{
					WinFuntions._selectItemComboBoxByValue(cbx_Status, 2.ToString());
				}
			}
			else if (text.Equals("2"))
			{
				text2 = await UpdateStatus(obj.Id.ToString(), 1.ToString());
				if (string.IsNullOrEmpty(text2))
				{
					WinFuntions._selectItemComboBoxByValue(cbx_Status, 1.ToString());
				}
			}
			if (string.IsNullOrEmpty(text2))
			{
				FormMode = Enums.FormViewModeType.Update;
				WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> UpdateStatus(string id, string status)
	{
		try
		{
			return await _mrpClientHelper.GetStringAsync("UpdateStatus", new { id, status });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
	}

	private async void btnRun_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (!ValidRunning() || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRun, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			switch (XtraMessageBox.Show("Bạn có muốn hệ thống tự động lưu kết quả lại sau khi MRP chạy xong không?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
			{
			case DialogResult.Yes:
				if (WinFuntions._text(txt_MrpName))
				{
					LoadColumn();
					await LoadData();
					await SaveData();
				}
				break;
			case DialogResult.No:
				LoadColumn();
				await LoadData();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData()
	{
		gv_MRP.ActiveFilterString = null;
		gv_Capacity.ActiveFilterString = null;
		obj.MrpCode = txt_MrpCode.Text;
		obj.MrpName = txt_MrpName.Text;
		obj.MpsCode = txt_MpsCode.Text;
		obj.BeginDate = DateTime.Parse(date_BeginDate.EditValue.ToString());
		obj.EndDate = DateTime.Parse(date_EndDate.EditValue.ToString());
		obj.FixDate = DateTime.Parse(date_FixDate.EditValue.ToString());
		obj.Status = int.Parse(((Common.ListItems)cbx_Status.SelectedItem).Value);
		obj.PoEntry = txt_POEntry.Text;
		obj.Notes = memoEx_Notes.Text;
		List<Mrp2> lstMRP2;
		Mrp mrp3;
		if (!string.IsNullOrEmpty(_isMRP) && _isMRP.Equals("MRP"))
		{
			DataTable dataTable = (DataTable)grd_MRP.DataSource;
			List<Mrp1> lstMRP1 = new List<Mrp1>();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.BeginLoadData();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					for (int j = 0; j < _colNumber_MRP; j++)
					{
						decimal value = (string.IsNullOrEmpty(dataTable.Rows[i][j.ToString()].ToString()) ? 0m : decimal.Parse(dataTable.Rows[i][j.ToString()].ToString()));
						decimal value2 = (string.IsNullOrEmpty(dataTable.Rows[i]["OB"].ToString()) ? 0m : decimal.Parse(dataTable.Rows[i]["OB"].ToString()));
						string text = ((!gv_MRP.Columns[j.ToString()].Caption.Equals("O. Due")) ? gv_MRP.Columns[j.ToString()].Caption : gv_MRP.Columns[j.ToString()].Tag.ToString());
						DateTime value3 = Function.ToDate(text, "dd/MM/yyyy");
						Mrp1 mrp = new Mrp1();
						mrp.ItemCode = dataTable.Rows[i]["ItemCode"].ToString();
						mrp.ItemName = dataTable.Rows[i]["ItemName"].ToString();
						mrp.Line = dataTable.Rows[i]["Line"].ToString();
						mrp.MachineTime = (string.IsNullOrEmpty(dataTable.Rows[i]["MachineTime"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[i]["MachineTime"].ToString()));
						mrp.NumMachine = (string.IsNullOrEmpty(dataTable.Rows[i]["NumMachine"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[i]["NumMachine"].ToString()));
						mrp.LTime = ((!string.IsNullOrEmpty(dataTable.Rows[i]["LTime"].ToString())) ? int.Parse(dataTable.Rows[i]["LTime"].ToString()) : 0);
						mrp.Qty = value;
						mrp.DocumentType = dataTable.Rows[i]["DocumentType"].ToString();
						mrp.ConfirmQty = mrp.Qty;
						mrp.DueDate = value3;
						mrp.Notes = dataTable.Rows[i]["Notes"].ToString();
						mrp.Type = int.Parse(dataTable.Rows[i]["Type"].ToString());
						mrp.OB = value2;
						lstMRP1.Add(mrp);
					}
				}
				dataTable.EndLoadData();
			}
			DataTable dataTable2 = (DataTable)grd_Capacity.DataSource;
			lstMRP2 = new List<Mrp2>();
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				dataTable2.BeginLoadData();
				for (int k = 0; k < dataTable2.Rows.Count; k++)
				{
					for (int l = 0; l < _colNumber_Capacity; l++)
					{
						double value4 = (string.IsNullOrEmpty(dataTable2.Rows[k][l.ToString()].ToString()) ? 0.0 : double.Parse(dataTable2.Rows[k][l.ToString()].ToString()));
						string text2 = ((!gv_Capacity.Columns[l.ToString()].Caption.Equals("O. Due")) ? gv_Capacity.Columns[l.ToString()].Caption : gv_Capacity.Columns[l.ToString()].Tag.ToString());
						DateTime value5 = Function.ToDate(text2, "dd/MM/yyyy");
						Mrp2 mrp2 = new Mrp2();
						mrp2.Item = dataTable2.Rows[k]["Item"].ToString();
						mrp2.MRPID = ((!string.IsNullOrEmpty(dataTable2.Rows[k]["MRPID"].ToString())) ? int.Parse(dataTable2.Rows[k]["MRPID"].ToString()) : 0);
						mrp2.Qty = value4;
						mrp2.Type = int.Parse(dataTable2.Rows[k]["Type"].ToString());
						mrp2.TypeName = dataTable2.Rows[k]["TypeName"].ToString();
						mrp2.WorkDay = value5;
						mrp2.Notes = "";
						lstMRP2.Add(mrp2);
					}
				}
				dataTable2.EndLoadData();
			}
			_ = string.Empty;
			string text3;
			if (FormMode == Enums.FormViewModeType.Add)
			{
				DataTable dataTable3 = await GetMrpByCode(obj.MrpCode);
				if (dataTable3 != null && dataTable3.Rows.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.None, "MRP code is exist!");
					return;
				}
				text3 = await Create(obj, lstMRP1, lstMRP2);
			}
			else
			{
				text3 = await Update(obj, lstMRP1, lstMRP2);
			}
			if (text3 != null)
			{
				WinCommons.Error(Enums.MessageCode.None, text3);
				return;
			}
			mrp3 = obj;
			mrp3.Id = await GetMaxId("apz_OMRP", "Id");
			FormMode = Enums.FormViewModeType.View;
			txt_MrpCode.Properties.ReadOnly = true;
			WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
			return;
		}
		DataTable dataTable4 = (DataTable)grd_Capacity.DataSource;
		lstMRP2 = new List<Mrp2>();
		if (dataTable4 != null && dataTable4.Rows.Count > 0)
		{
			dataTable4.BeginLoadData();
			for (int m = 0; m < dataTable4.Rows.Count; m++)
			{
				for (int n = 0; n < _colNumber_Capacity; n++)
				{
					double value6 = (string.IsNullOrEmpty(dataTable4.Rows[m][n.ToString()].ToString()) ? 0.0 : double.Parse(dataTable4.Rows[m][n.ToString()].ToString()));
					string text4 = ((!gv_Capacity.Columns[n.ToString()].Caption.Equals("O. Due")) ? gv_Capacity.Columns[n.ToString()].Caption : gv_Capacity.Columns[n.ToString()].Tag.ToString());
					DateTime value7 = Function.ToDate(text4, "dd/MM/yyyy");
					Mrp2 mrp4 = new Mrp2();
					mrp4.Item = dataTable4.Rows[m]["Item"].ToString();
					mrp4.MRPID = ((!string.IsNullOrEmpty(dataTable4.Rows[m]["MRPID"].ToString())) ? int.Parse(dataTable4.Rows[m]["MRPID"].ToString()) : 0);
					mrp4.Qty = value6;
					mrp4.Type = int.Parse(dataTable4.Rows[m]["Type"].ToString());
					mrp4.TypeName = dataTable4.Rows[m]["TypeName"].ToString();
					mrp4.WorkDay = value7;
					mrp4.Notes = "";
					lstMRP2.Add(mrp4);
				}
			}
			dataTable4.EndLoadData();
		}
		_ = string.Empty;
		string text5;
		if (FormMode == Enums.FormViewModeType.Add)
		{
			DataTable dataTable5 = await GetMrpByCode(obj.MrpCode);
			if (dataTable5 != null && dataTable5.Rows.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "MRP code is exist!");
				return;
			}
			text5 = await Create(obj, lstMRP2);
		}
		else
		{
			text5 = await Update(obj, lstMRP2);
		}
		if (text5 != null)
		{
			WinCommons.Error(Enums.MessageCode.None, text5);
			return;
		}
		mrp3 = obj;
		mrp3.Id = await GetMaxId("apz_OMRP", "Id");
		FormMode = Enums.FormViewModeType.View;
		txt_MrpCode.Properties.ReadOnly = true;
		WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
	}

	private async Task<int> GetMaxId(string tableName, string fieldId)
	{
		try
		{
			return await _mrpClientHelper.GetIntAsync("GetMaxId", new { tableName, fieldId });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return 0;
	}

	private async Task<string> Update(Mrp obj, List<Mrp2> lstMRP2)
	{
		try
		{
			return await _mrpClientHelper.PostParamsAsJsonAsync("UpdateData", new Tuple<Mrp, List<Mrp2>>(obj, lstMRP2));
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private async Task<string> Create(Mrp obj, List<Mrp2> lstMRP2)
	{
		try
		{
			return await _mrpClientHelper.PostParamsAsJsonAsync("CreateData", new Tuple<Mrp, List<Mrp2>>(obj, lstMRP2));
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private async Task<string> Update(Mrp obj, List<Mrp1> lstMRP1, List<Mrp2> lstMRP2)
	{
		try
		{
			return await _mrpClientHelper.PostParamsAsJsonAsync("Update", new Tuple<Mrp, List<Mrp1>, List<Mrp2>>(obj, lstMRP1, lstMRP2));
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private async Task<string> Create(Mrp obj, List<Mrp1> lstMRP1, List<Mrp2> lstMRP2)
	{
		try
		{
			return await _mrpClientHelper.PostParamsAsJsonAsync("Create", new Tuple<Mrp, List<Mrp1>, List<Mrp2>>(obj, lstMRP1, lstMRP2));
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private async Task<DataTable> GetMrpByCode(string mrpCode)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("GetMrpByCode", new { mrpCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private bool ValidRunning()
	{
		if (!WinFuntions._date(date_BeginDate))
		{
			return false;
		}
		if (!WinFuntions._date(date_EndDate))
		{
			return false;
		}
		if (!WinFuntions._text(txt_MpsCode))
		{
			return false;
		}
		return true;
	}

	private async void gv_MRP_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			object rowCellValue = gv_MRP.GetRowCellValue(e.RowHandle, "Type");
			if (rowCellValue == null || !rowCellValue.ToString().Equals(TypeMRP.Adjust.ToString()))
			{
				return;
			}
			string caption = e.Column.Caption;
			DateTime workDay = new DateTime(int.Parse(caption.Substring(6, 4)), int.Parse(caption.Substring(3, 2)), int.Parse(caption.Substring(0, 2)));
			double numM = ((gv_MRP.GetRowCellValue(e.RowHandle, "MachineTime") == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle, "MachineTime").ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle, "MachineTime").ToString()));
			double timeM = ((gv_MRP.GetRowCellValue(e.RowHandle, "NumMachine") == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle, "NumMachine").ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle, "NumMachine").ToString()));
			string itemCode = gv_MRP.GetRowCellValue(e.RowHandle, "ItemCode").ToString();
			double num = ((gv_MRP.GetRowCellValue(e.RowHandle, e.Column) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle, e.Column).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle, e.Column).ToString()));
			for (int i = int.Parse(e.Column.FieldName); i <= _colNumber_MRP; i++)
			{
				string text = i.ToString();
				double num2 = ((gv_MRP.GetRowCellValue(e.RowHandle, text) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle, text).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle, text).ToString()));
				double num3 = ((gv_MRP.GetRowCellValue(e.RowHandle - 5, text) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle - 5, text).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle - 5, text).ToString()));
				double num4 = ((gv_MRP.GetRowCellValue(e.RowHandle - 4, text) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle - 4, text).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle - 4, text).ToString()));
				double num5 = ((gv_MRP.GetRowCellValue(e.RowHandle - 3, text) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle - 3, text).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle - 3, text).ToString()));
				double num6 = ((gv_MRP.GetRowCellValue(e.RowHandle - 1, text) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle - 1, text).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle - 1, text).ToString()));
				string fieldName = (int.Parse(text) - 1).ToString();
				double num7 = ((gv_MRP.GetRowCellValue(e.RowHandle + 1, fieldName) == null || string.IsNullOrEmpty(gv_MRP.GetRowCellValue(e.RowHandle + 1, fieldName).ToString())) ? 0.0 : double.Parse(gv_MRP.GetRowCellValue(e.RowHandle + 1, fieldName).ToString())) + num5 + num6 + num2 - num3 - num4;
				gv_MRP.SetRowCellValue(e.RowHandle + 1, text, num7);
			}
			double num8 = ((gv_MRP.ActiveEditor.OldEditValue == null || string.IsNullOrEmpty(gv_MRP.ActiveEditor.OldEditValue.ToString())) ? 0.0 : double.Parse(gv_MRP.ActiveEditor.OldEditValue.ToString()));
			double hieu = num - num8;
			tableChangedNguyen = await usp_BOM_GetChangedItemByCode(itemCode, workDay, num);
			Tinh_ItemChildren(itemCode, hieu, workDay);
			double num9 = 0.0;
			string value = ((gv_MRP.GetRowCellValue(e.RowHandle, "Line") == null) ? "" : gv_MRP.GetRowCellValue(e.RowHandle, "Line").ToString());
			double num10 = 0.0;
			tableMRP.BeginLoadData();
			for (int j = 0; j < tableMRP.Rows.Count; j++)
			{
				string text2 = tableMRP.Rows[j]["Line"].ToString().Trim();
				string text3 = tableMRP.Rows[j]["Type"].ToString().Trim();
				if (text2.Equals(value))
				{
					if (!(text3 == "5"))
					{
						if (text3 == "6")
						{
							double num11 = (string.IsNullOrEmpty(tableMRP.Rows[j][e.Column.FieldName.ToString()].ToString()) ? 0.0 : double.Parse(tableMRP.Rows[j][e.Column.FieldName.ToString()].ToString()));
							num9 += (num10 + num11) * numM * timeM;
						}
					}
					else
					{
						num10 = (string.IsNullOrEmpty(tableMRP.Rows[j][e.Column.FieldName.ToString()].ToString()) ? 0.0 : double.Parse(tableMRP.Rows[j][e.Column.FieldName.ToString()].ToString()));
					}
				}
				double num12 = 0.0;
				double num13 = 0.0;
				for (int k = 0; k < tableCapacity.Rows.Count; k++)
				{
					if (!tableCapacity.Rows[k]["Item"].ToString().Equals(value))
					{
						continue;
					}
					switch (tableCapacity.Rows[k]["Type"].ToString())
					{
					case "1":
						num13 = (string.IsNullOrEmpty(tableCapacity.Rows[k][e.Column.FieldName.ToString()].ToString()) ? 0.0 : double.Parse(tableCapacity.Rows[k][e.Column.FieldName.ToString()].ToString()));
						break;
					case "2":
						tableCapacity.Rows[k][e.Column.FieldName.ToString()] = num9;
						if (num13 == 0.0)
						{
							num12 = 0.0;
							break;
						}
						num12 = num9 / num13;
						num13 = 0.0;
						break;
					case "3":
						tableCapacity.Rows[k][e.Column.FieldName.ToString()] = num12;
						break;
					}
				}
				grd_Capacity.RefreshDataSource();
			}
			tableMRP.EndLoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> usp_BOM_GetChangedItemByCode(string itemCode, DateTime workDay, double Adjust)
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("usp_BOM_GetChangedItemByCode", new { itemCode, workDay, Adjust });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	private int GetColumn(DateTime date)
	{
		DateTime dateTime = DateTime.Parse(date_BeginDate.EditValue.ToString()).AddDays(-1.0);
		DateTime dateTime2 = DateTime.Parse(date_EndDate.EditValue.ToString());
		int num = 0;
		while (dateTime <= dateTime2)
		{
			if (dateTime > date)
			{
				return 0;
			}
			if (date.Equals(dateTime))
			{
				return num;
			}
			num++;
			dateTime = dateTime.AddDays(1.0);
		}
		return 0;
	}

	private void Tinh_CapacityChildren(string line, string column)
	{
		double num = 0.0;
		double num2 = 0.0;
		double num3 = 0.0;
		double num4 = 0.0;
		tableMRP.BeginLoadData();
		for (int i = 0; i < tableMRP.Rows.Count; i++)
		{
			string text = tableMRP.Rows[i]["Line"].ToString().Trim();
			string text2 = tableMRP.Rows[i]["Type"].ToString().Trim();
			num3 = (string.IsNullOrEmpty(tableMRP.Rows[i]["MachineTime"].ToString()) ? 0.0 : double.Parse(tableMRP.Rows[i]["MachineTime"].ToString()));
			num4 = (string.IsNullOrEmpty(tableMRP.Rows[i]["NumMachine"].ToString()) ? 0.0 : double.Parse(tableMRP.Rows[i]["NumMachine"].ToString()));
			if (!text.Equals(line))
			{
				continue;
			}
			if (!(text2 == "5"))
			{
				if (text2 == "6")
				{
					double num5 = (string.IsNullOrEmpty(tableMRP.Rows[i][column].ToString()) ? 0.0 : double.Parse(tableMRP.Rows[i][column].ToString()));
					num += (num2 + num5) * num3 * num4;
				}
			}
			else
			{
				num2 = (string.IsNullOrEmpty(tableMRP.Rows[i][column].ToString()) ? 0.0 : double.Parse(tableMRP.Rows[i][column].ToString()));
			}
		}
		tableMRP.EndLoadData();
		double num6 = 0.0;
		double num7 = 0.0;
		for (int j = 0; j < tableCapacity.Rows.Count; j++)
		{
			if (!tableCapacity.Rows[j]["Item"].ToString().Equals(line))
			{
				continue;
			}
			switch (tableCapacity.Rows[j]["Type"].ToString())
			{
			case "1":
				num7 = (string.IsNullOrEmpty(tableCapacity.Rows[j][column].ToString()) ? 0.0 : double.Parse(tableCapacity.Rows[j][column].ToString()));
				break;
			case "2":
				tableCapacity.Rows[j][column] = num;
				num6 = 0.0;
				if (num7 == 0.0)
				{
					num6 = 0.0;
					break;
				}
				num6 = num / num7;
				num7 = 0.0;
				break;
			case "3":
				tableCapacity.Rows[j][column] = num6;
				break;
			}
		}
	}

	private async Task Tinh_ItemChildren(string itemCode, double Adjust, DateTime dateChanged)
	{
		try
		{
			DataTable dataTable = await usp_BOM_GetChangedItemByCode(itemCode, dateChanged, Adjust);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text = dataTable.Rows[i]["Code"].ToString();
				DateTime date = (string.IsNullOrEmpty(dataTable.Rows[i]["ChangeDate"].ToString()) ? DateTime.Today : Convert.ToDateTime(dataTable.Rows[i]["ChangeDate"].ToString()));
				double num = (string.IsNullOrEmpty(dataTable.Rows[i]["ChangeQuantity"].ToString()) ? 0.0 : double.Parse(dataTable.Rows[i]["ChangeQuantity"].ToString()));
				double num2 = (string.IsNullOrEmpty(tableChangedNguyen.Rows[i]["ChangeQuantity"].ToString()) ? 0.0 : double.Parse(tableChangedNguyen.Rows[i]["ChangeQuantity"].ToString()));
				int column = GetColumn(date);
				DataRow[] array = tableMRP.Select("ItemCode = '" + text + "' and Type = " + TypeMRP.Adjust);
				if (array != null && array.Length != 0)
				{
					DataRow dataRow = array[0];
					double num3 = ((dataRow[column.ToString()] == null || string.IsNullOrEmpty(dataRow[column.ToString()].ToString())) ? 0.0 : double.Parse(dataRow[column.ToString()].ToString()));
					dataRow.BeginEdit();
					dataRow[column.ToString()] = num3 + num;
					dataRow.EndEdit();
					if (num3 + num < num2)
					{
						bool flag = false;
						for (int j = 0; j < lstChangedColor.Count; j++)
						{
							if (lstChangedColor[j].Text.Equals(text))
							{
								flag = true;
								break;
							}
						}
						if (!flag)
						{
							lstChangedColor.Add(new Common.ListItems
							{
								Value = column.ToString(),
								Text = text
							});
						}
					}
					string line = dataRow["Line"].ToString();
					Tinh_CapacityChildren(line, column.ToString());
				}
				DataRow[] array2 = tableMRP.Select("ItemCode = '" + text + "'");
				if (array2 == null || array2.Length == 0)
				{
					continue;
				}
				double num4 = 0.0;
				double num5 = 0.0;
				double num6 = 0.0;
				double num7 = 0.0;
				double num8 = 0.0;
				for (int k = column; k < _colNumber_MRP; k++)
				{
					for (int l = 0; l < array2.Length; l++)
					{
						int num9 = int.Parse(array2[l]["Type"].ToString());
						if (num9 == TypeMRP.Demand)
						{
							num4 = (string.IsNullOrEmpty(array2[l][k.ToString()].ToString()) ? 0.0 : double.Parse(array2[l][k.ToString()].ToString()));
						}
						else if (num9 == TypeMRP.Adjust)
						{
							num8 = (string.IsNullOrEmpty(array2[l][k.ToString()].ToString()) ? 0.0 : double.Parse(array2[l][k.ToString()].ToString()));
						}
						else if (num9 == TypeMRP.OrderPlan)
						{
							num5 = (string.IsNullOrEmpty(array2[l][k.ToString()].ToString()) ? 0.0 : double.Parse(array2[l][k.ToString()].ToString()));
						}
						else if (num9 == TypeMRP.Receivedplan)
						{
							num6 = (string.IsNullOrEmpty(array2[l][k.ToString()].ToString()) ? 0.0 : double.Parse(array2[l][k.ToString()].ToString()));
						}
						else if (num9 == TypeMRP.Requirement)
						{
							num7 = (string.IsNullOrEmpty(array2[l][k.ToString()].ToString()) ? 0.0 : double.Parse(array2[l][k.ToString()].ToString()));
						}
						else if (num9 == TypeMRP.StockAdjust)
						{
							double num10 = ((k >= 1) ? ((string.IsNullOrEmpty(array2[l][(k - 1).ToString()].ToString()) ? 0.0 : double.Parse(array2[l][(k - 1).ToString()].ToString())) + num6 + num7 + num8 - num5 - num4) : ((string.IsNullOrEmpty(array2[l][k.ToString()].ToString()) ? 0.0 : double.Parse(array2[l][k.ToString()].ToString())) + num8));
							DataRow obj = array2[l];
							obj.BeginEdit();
							obj[k.ToString()] = num10;
							obj.EndEdit();
						}
					}
				}
			}
			tableMRP.AcceptChanges();
			grd_MRP.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_MRP_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			string valueDisplayText = gv_MRP.Columns["Line"].FilterInfo.GetValueDisplayText();
			valueDisplayText = valueDisplayText.Replace("[Line]", "[Item]");
			gv_Capacity.ActiveFilterString = valueDisplayText;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_MRP_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_MRP_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			e.Appearance.BackColor = Color.LightGray;
		}
		catch (Exception)
		{
		}
	}

	private void gv_MRP_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			e.Cancel = true;
		}
		catch (Exception)
		{
		}
	}

	private void gv_Capacity_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_Capacity_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (gv_Capacity.GetRowCellValue(e.RowHandle, "Type") != null)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception)
		{
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await Save();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async Task Save()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (ValidForm() && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await SaveData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ValidForm()
	{
		if (!WinFuntions._date(date_BeginDate))
		{
			return false;
		}
		if (!WinFuntions._date(date_EndDate))
		{
			return false;
		}
		if (!WinFuntions._text(txt_MrpCode))
		{
			return false;
		}
		if (!WinFuntions._text(txt_MrpName))
		{
			return false;
		}
		if (!WinFuntions._text(txt_MpsCode))
		{
			return false;
		}
		DataTable dataTable = (DataTable)grd_MRP.DataSource;
		if (dataTable == null || dataTable.Rows.Count == 0)
		{
			WinCommons.Error(Enums.MessageCode.None, "Chưa chạy mrp!");
			return false;
		}
		return true;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Export_Click(object sender, EventArgs e)
	{
		try
		{
			saveFile.FileName = "Export-MRP" + DateTime.Now.ToString("yyyyMMdd");
			saveFile.ValidateNames = true;
			saveFile.Filter = "Excel 97-2003 Workbook (*.xls)|*.xls |Excel Workbook (*.xlsx)|*.xlsx";
			saveFile.Title = "Export to excel";
			if (saveFile.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(saveFile.FileName))
			{
				return;
			}
			string text = Path.GetExtension(saveFile.FileName).ToLower().Trim();
			if (!(text == ".xls"))
			{
				if (text == ".xlsx")
				{
					grd_MRP.ExportToXlsx(saveFile.FileName);
					grd_Capacity.ExportToXls("Export-MRPCapacity" + DateTime.Now.ToString("yyyyMMdd"));
				}
				else
				{
					grd_MRP.ExportToXls(saveFile.FileName);
					grd_Capacity.ExportToXls("Export-MRPCapacity" + DateTime.Now.ToString("yyyyMMdd"));
				}
			}
			else
			{
				grd_MRP.ExportToXls(saveFile.FileName);
				grd_Capacity.ExportToXls("Export-MRPCapacity" + DateTime.Now.ToString("yyyyMMdd"));
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmMrp));
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
		this.memoEx_Notes = new DevExpress.XtraEditors.MemoExEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ChangedStatus = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_FixDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_MpsCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grd_MRP = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MRP = new Apzon.Controls.Grid.GridViewEx();
		this.repo_DocType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repo_DateEdit_DueDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repo_MemoExEdit_Notes = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.repo_CalcEdit_Qty = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.groupControl3 = new DevExpress.XtraEditors.GroupControl();
		this.grd_Capacity = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Capacity = new Apzon.Controls.Grid.GridViewEx();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.txt_POEntry = new Apzon.Controls.Editor.TextEditEx();
		this.btnRun = new Apzon.Controls.Controls.SimpleButtonEx();
		this.saveFile = new System.Windows.Forms.SaveFileDialog();
		this.date_EndDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_ProductionOrder = new Apzon.Controls.Editor.ButtonEditEx();
		this.cbx_Status = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txt_MrpCode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MrpName = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.date_BeginDate = new Apzon.Controls.Editor.DateEditEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.memoEx_Notes.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MpsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MRP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MRP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_DocType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_DateEdit_DueDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_DateEdit_DueDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_MemoExEdit_Notes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repo_CalcEdit_Qty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).BeginInit();
		this.groupControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Capacity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Capacity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_POEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ProductionOrder.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		base.SuspendLayout();
		this.memoEx_Notes.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.memoEx_Notes.Location = new System.Drawing.Point(438, 409);
		this.memoEx_Notes.Name = "memoEx_Notes";
		this.memoEx_Notes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memoEx_Notes.Properties.ShowIcon = false;
		this.memoEx_Notes.Size = new System.Drawing.Size(207, 20);
		this.memoEx_Notes.TabIndex = 55;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(88, 406);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 54;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Add";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(7, 406);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 53;
		this.btnSave.Text = "Add";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btn_ChangedStatus.EditControl = null;
		this.btn_ChangedStatus.EditValue = "Change";
		this.btn_ChangedStatus.FieldName = null;
		this.btn_ChangedStatus.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ChangedStatus.ImageOptions.Image");
		this.btn_ChangedStatus.IsSearchControl = false;
		this.btn_ChangedStatus.Location = new System.Drawing.Point(581, 22);
		this.btn_ChangedStatus.Name = "btn_ChangedStatus";
		this.btn_ChangedStatus.ReadOnly = false;
		this.btn_ChangedStatus.SettingInVisible = false;
		this.btn_ChangedStatus.SettingReadonly = false;
		this.btn_ChangedStatus.Size = new System.Drawing.Size(75, 23);
		this.btn_ChangedStatus.TabIndex = 52;
		this.btn_ChangedStatus.Text = "Change";
		this.btn_ChangedStatus.Click += new System.EventHandler(this.btn_ChangedStatus_Click);
		this.date_FixDate.EditControl = null;
		this.date_FixDate.EditValue = null;
		this.date_FixDate.FieldName = null;
		this.date_FixDate.Location = new System.Drawing.Point(432, 68);
		this.date_FixDate.Name = "date_FixDate";
		this.date_FixDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FixDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.date_FixDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_FixDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FixDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_FixDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_FixDate.Properties.IsSearchControl = false;
		this.date_FixDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_FixDate.Properties.ReadOnly = true;
		this.date_FixDate.SettingInVisible = false;
		this.date_FixDate.SettingReadonly = false;
		this.date_FixDate.Size = new System.Drawing.Size(144, 20);
		this.date_FixDate.TabIndex = 6;
		this.txt_MpsCode.EditControl = null;
		this.txt_MpsCode.FieldName = null;
		this.txt_MpsCode.IsSearchControl = false;
		this.txt_MpsCode.Location = new System.Drawing.Point(123, 68);
		this.txt_MpsCode.Name = "txt_MpsCode";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_MpsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_MpsCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_MpsCode_Properties_ButtonClick);
		this.txt_MpsCode.SettingInVisible = false;
		this.txt_MpsCode.SettingReadonly = false;
		this.txt_MpsCode.Size = new System.Drawing.Size(145, 24);
		this.txt_MpsCode.TabIndex = 2;
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(0, 128);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.groupControl2);
		this.splitContainer1.Panel2.Controls.Add(this.groupControl3);
		this.splitContainer1.Size = new System.Drawing.Size(662, 275);
		this.splitContainer1.SplitterDistance = 137;
		this.splitContainer1.TabIndex = 56;
		this.groupControl2.Controls.Add(this.grd_MRP);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(662, 137);
		this.groupControl2.TabIndex = 0;
		this.groupControl2.Text = "MRP";
		this.grd_MRP.AllowPasteExcel = true;
		this.grd_MRP.ClipboardData = null;
		this.grd_MRP.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_MRP.Location = new System.Drawing.Point(2, 23);
		this.grd_MRP.MainView = this.gv_MRP;
		this.grd_MRP.Name = "grd_MRP";
		this.grd_MRP.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.repo_DocType, this.repo_DateEdit_DueDate, this.repo_MemoExEdit_Notes, this.repo_CalcEdit_Qty });
		this.grd_MRP.Size = new System.Drawing.Size(658, 112);
		this.grd_MRP.TabIndex = 1;
		this.grd_MRP.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_MRP, this.gridView1 });
		this.gv_MRP.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MRP.ColumnKey = null;
		this.gv_MRP.DisableFormSetting = false;
		this.gv_MRP.DisplayException = false;
		this.gv_MRP.EditColumn = null;
		this.gv_MRP.FieldCurrency = null;
		this.gv_MRP.FieldRate = null;
		this.gv_MRP.FormId = null;
		this.gv_MRP.GridControl = this.grd_MRP;
		this.gv_MRP.IgnoreFilter = false;
		this.gv_MRP.IndexEditRow = -1;
		this.gv_MRP.IndicatorWidth = 50;
		this.gv_MRP.IsApzTable = false;
		this.gv_MRP.IsInsertNewRow = false;
		this.gv_MRP.IsLoadingData = false;
		this.gv_MRP.IsMappingContextAction = false;
		this.gv_MRP.ItemId = null;
		this.gv_MRP.LstColumnAllowBatch = null;
		this.gv_MRP.LstColumnNotAllowBatch = null;
		this.gv_MRP.LstColumnNotEditable = null;
		this.gv_MRP.LstColumNotSetting = null;
		this.gv_MRP.Name = "gv_MRP";
		this.gv_MRP.OptionsView.ColumnAutoWidth = false;
		this.gv_MRP.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_MRP.OptionsView.ShowGroupPanel = false;
		this.gv_MRP.UseBatchMod = false;
		this.gv_MRP.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_MRP_CustomDrawRowIndicator);
		this.gv_MRP.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_MRP_RowStyle);
		this.gv_MRP.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_MRP_ShowingEditor);
		this.gv_MRP.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_MRP_CellValueChanged);
		this.gv_MRP.ColumnFilterChanged += new System.EventHandler(this.gv_MRP_ColumnFilterChanged);
		this.repo_DocType.AutoHeight = false;
		this.repo_DocType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_DocType.Name = "repo_DocType";
		this.repo_DateEdit_DueDate.AutoHeight = false;
		this.repo_DateEdit_DueDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_DateEdit_DueDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_DateEdit_DueDate.Name = "repo_DateEdit_DueDate";
		this.repo_MemoExEdit_Notes.AutoHeight = false;
		this.repo_MemoExEdit_Notes.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_MemoExEdit_Notes.Name = "repo_MemoExEdit_Notes";
		this.repo_CalcEdit_Qty.AutoHeight = false;
		this.repo_CalcEdit_Qty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repo_CalcEdit_Qty.Name = "repo_CalcEdit_Qty";
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.GridControl = this.grd_MRP;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.groupControl3.Controls.Add(this.grd_Capacity);
		this.groupControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl3.Location = new System.Drawing.Point(0, 0);
		this.groupControl3.Name = "groupControl3";
		this.groupControl3.Size = new System.Drawing.Size(662, 134);
		this.groupControl3.TabIndex = 0;
		this.groupControl3.Text = "Capacity";
		this.grd_Capacity.AllowPasteExcel = true;
		this.grd_Capacity.ClipboardData = null;
		this.grd_Capacity.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Capacity.Location = new System.Drawing.Point(2, 23);
		this.grd_Capacity.MainView = this.gv_Capacity;
		this.grd_Capacity.Name = "grd_Capacity";
		this.grd_Capacity.Size = new System.Drawing.Size(658, 109);
		this.grd_Capacity.TabIndex = 0;
		this.grd_Capacity.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Capacity, this.gridView2 });
		this.gv_Capacity.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Capacity.ColumnKey = null;
		this.gv_Capacity.DisableFormSetting = false;
		this.gv_Capacity.DisplayException = false;
		this.gv_Capacity.EditColumn = null;
		this.gv_Capacity.FieldCurrency = null;
		this.gv_Capacity.FieldRate = null;
		this.gv_Capacity.FormId = null;
		this.gv_Capacity.GridControl = this.grd_Capacity;
		this.gv_Capacity.IgnoreFilter = false;
		this.gv_Capacity.IndexEditRow = -1;
		this.gv_Capacity.IndicatorWidth = 50;
		this.gv_Capacity.IsApzTable = false;
		this.gv_Capacity.IsInsertNewRow = false;
		this.gv_Capacity.IsLoadingData = false;
		this.gv_Capacity.IsMappingContextAction = false;
		this.gv_Capacity.ItemId = null;
		this.gv_Capacity.LstColumnAllowBatch = null;
		this.gv_Capacity.LstColumnNotAllowBatch = null;
		this.gv_Capacity.LstColumnNotEditable = null;
		this.gv_Capacity.LstColumNotSetting = null;
		this.gv_Capacity.Name = "gv_Capacity";
		this.gv_Capacity.OptionsView.ColumnAutoWidth = false;
		this.gv_Capacity.OptionsView.ShowGroupPanel = false;
		this.gv_Capacity.UseBatchMod = false;
		this.gv_Capacity.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Capacity_CustomDrawRowIndicator);
		this.gv_Capacity.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Capacity_RowStyle);
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = null;
		this.gridView2.GridControl = this.grd_Capacity;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 35;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = null;
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.UseBatchMod = false;
		this.txt_POEntry.EditControl = null;
		this.txt_POEntry.FieldName = null;
		this.txt_POEntry.IsSearchControl = false;
		this.txt_POEntry.Location = new System.Drawing.Point(270, 22);
		this.txt_POEntry.Name = "txt_POEntry";
		this.txt_POEntry.SettingInVisible = false;
		this.txt_POEntry.SettingReadonly = false;
		this.txt_POEntry.Size = new System.Drawing.Size(10, 20);
		this.txt_POEntry.TabIndex = 49;
		this.txt_POEntry.Visible = false;
		this.btnRun.EditControl = null;
		this.btnRun.EditValue = "Run";
		this.btnRun.FieldName = null;
		this.btnRun.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRun.ImageOptions.Image");
		this.btnRun.IsSearchControl = false;
		this.btnRun.Location = new System.Drawing.Point(581, 88);
		this.btnRun.Name = "btnRun";
		this.btnRun.ReadOnly = false;
		this.btnRun.SettingInVisible = false;
		this.btnRun.SettingReadonly = false;
		this.btnRun.Size = new System.Drawing.Size(75, 23);
		this.btnRun.TabIndex = 8;
		this.btnRun.Text = "Run";
		this.btnRun.Click += new System.EventHandler(this.btnRun_Click);
		this.date_EndDate.EditControl = null;
		this.date_EndDate.EditValue = null;
		this.date_EndDate.FieldName = null;
		this.date_EndDate.Location = new System.Drawing.Point(432, 91);
		this.date_EndDate.Name = "date_EndDate";
		this.date_EndDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_EndDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_EndDate.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_EndDate.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.CalendarTimeProperties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_EndDate.Properties.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy";
		this.date_EndDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_EndDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_EndDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_EndDate.Properties.IsSearchControl = false;
		this.date_EndDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_EndDate.Properties.ReadOnly = true;
		this.date_EndDate.SettingInVisible = false;
		this.date_EndDate.SettingReadonly = false;
		this.date_EndDate.Size = new System.Drawing.Size(144, 20);
		this.date_EndDate.TabIndex = 7;
		this.txt_ProductionOrder.EditControl = null;
		this.txt_ProductionOrder.FieldName = null;
		this.txt_ProductionOrder.IsSearchControl = false;
		this.txt_ProductionOrder.Location = new System.Drawing.Point(123, 94);
		this.txt_ProductionOrder.Name = "txt_ProductionOrder";
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.txt_ProductionOrder.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_ProductionOrder.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_ProductionOrder_Properties_ButtonClick);
		this.txt_ProductionOrder.SettingInVisible = false;
		this.txt_ProductionOrder.SettingReadonly = false;
		this.txt_ProductionOrder.Size = new System.Drawing.Size(145, 24);
		this.txt_ProductionOrder.TabIndex = 3;
		this.cbx_Status.EditControl = null;
		this.cbx_Status.FieldName = null;
		this.cbx_Status.IsSearchControl = false;
		this.cbx_Status.Location = new System.Drawing.Point(432, 22);
		this.cbx_Status.Name = "cbx_Status";
		this.cbx_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbx_Status.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cbx_Status.SettingInVisible = false;
		this.cbx_Status.SettingReadonly = false;
		this.cbx_Status.Size = new System.Drawing.Size(144, 20);
		this.cbx_Status.TabIndex = 4;
		this.txt_MrpCode.EditControl = null;
		this.txt_MrpCode.FieldName = null;
		this.txt_MrpCode.IsSearchControl = false;
		this.txt_MrpCode.Location = new System.Drawing.Point(123, 22);
		this.txt_MrpCode.Name = "txt_MrpCode";
		this.txt_MrpCode.Properties.ReadOnly = true;
		this.txt_MrpCode.SettingInVisible = false;
		this.txt_MrpCode.SettingReadonly = false;
		this.txt_MrpCode.Size = new System.Drawing.Size(145, 20);
		this.txt_MrpCode.TabIndex = 0;
		this.txt_MrpName.EditControl = null;
		this.txt_MrpName.FieldName = null;
		this.txt_MrpName.IsSearchControl = false;
		this.txt_MrpName.Location = new System.Drawing.Point(123, 45);
		this.txt_MrpName.Name = "txt_MrpName";
		this.txt_MrpName.SettingInVisible = false;
		this.txt_MrpName.SettingReadonly = false;
		this.txt_MrpName.Size = new System.Drawing.Size(145, 20);
		this.txt_MrpName.TabIndex = 1;
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Export.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Export.EditControl = null;
		this.btn_Export.EditValue = "Export";
		this.btn_Export.FieldName = null;
		this.btn_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Export.ImageOptions.Image");
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(169, 406);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.ReadOnly = false;
		this.btn_Export.SettingInVisible = false;
		this.btn_Export.SettingReadonly = false;
		this.btn_Export.Size = new System.Drawing.Size(75, 23);
		this.btn_Export.TabIndex = 57;
		this.btn_Export.Text = "Export";
		this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
		this.date_BeginDate.EditControl = null;
		this.date_BeginDate.EditValue = null;
		this.date_BeginDate.FieldName = null;
		this.date_BeginDate.Location = new System.Drawing.Point(432, 45);
		this.date_BeginDate.Name = "date_BeginDate";
		this.date_BeginDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_BeginDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_BeginDate.Properties.CalendarTimeProperties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_BeginDate.Properties.CalendarTimeProperties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.CalendarTimeProperties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_BeginDate.Properties.CalendarTimeProperties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.CalendarTimeProperties.Mask.EditMask = "dd/MM/yyyy";
		this.date_BeginDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_BeginDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_BeginDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_BeginDate.Properties.IsSearchControl = false;
		this.date_BeginDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_BeginDate.Properties.ReadOnly = true;
		this.date_BeginDate.SettingInVisible = false;
		this.date_BeginDate.SettingReadonly = false;
		this.date_BeginDate.Size = new System.Drawing.Size(144, 20);
		this.date_BeginDate.TabIndex = 5;
		this.groupControl1.Controls.Add(this.labelLine8);
		this.groupControl1.Controls.Add(this.labelLine7);
		this.groupControl1.Controls.Add(this.labelLine6);
		this.groupControl1.Controls.Add(this.labelLine5);
		this.groupControl1.Controls.Add(this.labelLine4);
		this.groupControl1.Controls.Add(this.labelLine3);
		this.groupControl1.Controls.Add(this.labelLine2);
		this.groupControl1.Controls.Add(this.labelLine1);
		this.groupControl1.Controls.Add(this.btn_ChangedStatus);
		this.groupControl1.Controls.Add(this.date_FixDate);
		this.groupControl1.Controls.Add(this.txt_MpsCode);
		this.groupControl1.Controls.Add(this.txt_POEntry);
		this.groupControl1.Controls.Add(this.btnRun);
		this.groupControl1.Controls.Add(this.date_EndDate);
		this.groupControl1.Controls.Add(this.date_BeginDate);
		this.groupControl1.Controls.Add(this.txt_ProductionOrder);
		this.groupControl1.Controls.Add(this.cbx_Status);
		this.groupControl1.Controls.Add(this.txt_MrpCode);
		this.groupControl1.Controls.Add(this.txt_MrpName);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(662, 120);
		this.groupControl1.TabIndex = 52;
		this.groupControl1.Text = "MRP Information";
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "To Forecast Date";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(313, 92);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(119, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 60;
		this.labelLine8.Utext = "To Forecast Date";
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "To Fix Date";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(313, 71);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(119, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 59;
		this.labelLine7.Utext = "To Fix Date";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "From Date";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(313, 49);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(119, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 58;
		this.labelLine6.Utext = "From Date";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(313, 24);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(119, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 57;
		this.labelLine5.Utext = "Status";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Production Order";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(4, 98);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(119, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 56;
		this.labelLine4.Utext = "Production Order";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "MPS Code";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(4, 73);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(119, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 55;
		this.labelLine3.Utext = "MPS Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(4, 48);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(119, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 54;
		this.labelLine2.Utext = "Description";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "MRP Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(4, 24);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(119, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 53;
		this.labelLine1.Utext = "MRP Code";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(662, 430);
		base.Controls.Add(this.memoEx_Notes);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btn_Export);
		base.Controls.Add(this.groupControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmMrp";
		base.ShowInTaskbar = false;
		this.Text = "MRP Running";
		base.Load += new System.EventHandler(this.FrmMrp_Load);
		((System.ComponentModel.ISupportInitialize)this.memoEx_Notes.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FixDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MpsCode.Properties).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_MRP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MRP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_DocType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_DateEdit_DueDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_DateEdit_DueDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_MemoExEdit_Notes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repo_CalcEdit_Qty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl3).EndInit();
		this.groupControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Capacity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Capacity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_POEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_EndDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ProductionOrder.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbx_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MrpName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_BeginDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
