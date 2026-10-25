using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.ItemsMaster;

public class frm_UoMInItem : XtraFormEx
{
	public DataTable _dtSource = new DataTable();

	public DataTable _dtReturn = new DataTable();

	public string _itemCode = "";

	public string _ugpCode = "";

	public string _baseUomCode = "";

	public string _uomType = "";

	public bool _isLoading;

	public bool _btnOKClick;

	public int _ugpEntry;

	public int _dfltEntry;

	public bool _changeData;

	private string _formController = "ItemsMaster";

	private bool _isChangingLength;

	private bool _isChangingVolume;

	private IContainer components;

	private LabelLine lb_BarCode;

	private TextEditEx txt_ItemCode;

	private LabelLine labelLine1;

	private TextEditEx txt_UomGroup;

	private LabelLine labelLine2;

	private TextEditEx txt_InventoryUom;

	private GridControlEx grd_UoM;

	private GridViewEx gv_UoM;

	private GridColumnEx gcWhseName;

	private GridControlEx grd_Pkg;

	private GridViewEx gv_Pkg;

	private GridColumnEx gridColumn1;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOK;

	private SimpleButtonEx btn_SetDefault;

	private TextEditEx txt_UnitName;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private Label lbl_UomCode;

	private Label label2;

	private Label lbl_InvUomCode;

	private LabelLine labelLine5;

	private LookUpEditEx luk_VolUnit;

	private LabelLine labelLine8;

	private CalcEditEx calc_Volume;

	private LabelLine labelLine6;

	private LabelLine labelLine7;

	private LabelLine labelLine9;

	private LabelLine labelLine10;

	private LookUpEditEx lookUpEdit1;

	private LabelLine labelLine11;

	private CalcEditEx calcEdit3;

	private LabelLine labelLine12;

	private LabelLine labelLine13;

	private LabelLine labelLine14;

	private LabelLine labelLine15;

	private LabelLine labelLine16;

	private LabelLine labelLine17;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx btn_NewRow;

	private CalcEditEx calc_ItemPerUnit;

	private CalcEditEx calc_BaseUnit;

	private CalcEditEx calcEdit8;

	private RepositoryItemLookUpEdit rep_UomEntry;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private MeasureUnitEditEx calc_Length1;

	private MeasureUnitEditEx calc_Width1;

	private MeasureUnitEditEx calc_Height1;

	private MeasureUnitEditEx calc_Weight1;

	private MeasureUnitEditEx calcEdit1;

	private MeasureUnitEditEx calcEdit5;

	private MeasureUnitEditEx calcEdit4;

	private MeasureUnitEditEx calcEdit2;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btOK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_UoMInItem()
	{
		InitializeComponent();
	}

	private void frm_UoMInItem_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			txt_ItemCode.Text = _itemCode;
			txt_UomGroup.Text = _ugpCode;
			txt_InventoryUom.Text = _baseUomCode;
			lbl_InvUomCode.Text = _baseUomCode;
			calc_Volume.SetFormatControl(Enums.DisplayFormatType.Amount);
			calcEdit3.SetFormatControl(Enums.DisplayFormatType.Amount);
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
			{
				DataTable dataSource = httpClientHelper.Get("GetVolumeUnit");
				luk_VolUnit.Properties.Columns.Clear();
				luk_VolUnit.Properties.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Name",
					FieldName = "Name"
				});
				luk_VolUnit.Properties.DisplayMember = "Name";
				luk_VolUnit.Properties.ValueMember = "Code";
				luk_VolUnit.Properties.NullText = "";
				luk_VolUnit.Properties.DataSource = dataSource;
			}
			using (HttpClientHelper<DataTable> httpClientHelper2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
			{
				DataTable dataSource2 = httpClientHelper2.Get("GetUoMCombo", new
				{
					ugpEntry = _ugpEntry,
					itemCode = _itemCode,
					type = "I"
				});
				rep_UomEntry.Columns.Clear();
				rep_UomEntry.Columns.Add(new LookUpColumnInfo
				{
					Caption = "Code",
					FieldName = "UomCode"
				});
				rep_UomEntry.DisplayMember = "UomCode";
				rep_UomEntry.ValueMember = "UomEntry";
				rep_UomEntry.NullText = "";
				rep_UomEntry.DataSource = dataSource2;
			}
			_dtReturn = _dtSource.Copy();
			grd_UoM.DataSource = _dtSource;
			grd_UoM.RefreshDataSource();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_UoM_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_isLoading = true;
		try
		{
			DataTable dataTable = grd_UoM.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (Function.ParseInt(gv_UoM.GetFocusedRowCellValue("UomEntry")) > 0)
				{
					int focusedDataSourceRowIndex = gv_UoM.GetFocusedDataSourceRowIndex();
					DataRow dataRow = dataTable.Rows[focusedDataSourceRowIndex];
					DataTable rowsDetails = GetRowsDetails(Function.ParseInt(dataRow["UomEntry"]), _ugpEntry);
					if (rowsDetails != null && rowsDetails.Rows.Count > 0)
					{
						txt_UnitName.Text = Function.ToString(rowsDetails.Rows[0]["UomName"]);
						calc_ItemPerUnit.EditValue = Function.ParseDouble(rowsDetails.Rows[0]["AltQty"]);
						calc_BaseUnit.EditValue = Function.ParseDouble(rowsDetails.Rows[0]["BaseQty"]);
					}
					calc_Length1.EditValue = Function.ParseDouble(dataRow["Length1"]);
					calc_Length1.UnitCode = Function.ParseShort(dataRow["Len1Unit"]);
					calc_Width1.EditValue = Function.ParseDouble(dataRow["Width1"]);
					calc_Width1.UnitCode = Function.ParseShort(dataRow["Wdth1Unit"]);
					calc_Height1.EditValue = Function.ParseDouble(dataRow["Height1"]);
					calc_Height1.UnitCode = Function.ParseShort(dataRow["Hght1Unit"]);
					calc_Volume.EditValue = Function.ParseDouble(dataRow["Volume"]);
					luk_VolUnit.EditValue = Function.ParseShort(dataRow["VolUnit"]);
					calc_Weight1.EditValue = Function.ParseDouble(dataRow["Weight1"]);
					calc_Weight1.UnitCode = Function.ParseShort(dataRow["Wght1Unit"]);
					calc_Length1.Enabled = true;
					calc_Width1.Enabled = true;
					calc_Height1.Enabled = true;
					calc_Volume.Enabled = true;
					calc_Weight1.Enabled = true;
					luk_VolUnit.Enabled = true;
				}
				else
				{
					calc_Length1.Enabled = false;
					calc_Width1.Enabled = false;
					calc_Height1.Enabled = false;
					calc_Volume.Enabled = false;
					calc_Weight1.Enabled = false;
					luk_VolUnit.Enabled = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void gv_UoM_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_UoM.GetRowCellValue(e.RowHandle, "UomEntry")) == _dfltEntry)
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
			else
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Regular);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grd_UoM.DataSource is DataTable dataTable)
			{
				int[] selectedRows = gv_UoM.GetSelectedRows();
				if (selectedRows != null && selectedRows.Length != 0 && gv_UoM.GetFocusedDataSourceRowIndex() > 0)
				{
					dataTable.Rows.RemoveAt(gv_UoM.GetFocusedDataSourceRowIndex());
					dataTable.AcceptChanges();
					grd_UoM.DataSource = dataTable;
					grd_UoM.RefreshDataSource();
					ChangeButtonOKText();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_UoMInItem_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Update;
			if (e.FieldName.Equals(calc_Length1.FieldName) || e.FieldName.Equals(calc_Width1.FieldName) || e.FieldName.Equals(calc_Height1.FieldName))
			{
				double num = Function.ParseDouble(calc_Height1.EditValue);
				double num2 = Function.ParseDouble(calc_Length1.EditValue);
				double num3 = Function.ParseDouble(calc_Width1.EditValue);
				if (Math.Abs(num) > 1E-07 && Math.Abs(num2) > 1E-07 && Math.Abs(num3) > 1E-07)
				{
					short? toUnit = Function.ParseShort(luk_VolUnit.EditValue);
					double value = num * num2 * num3 * toUnit.GetExchangeData(calc_Height1.UnitCode, Enums.MeasureType.LengthAndWidth) * toUnit.GetExchangeData(calc_Length1.UnitCode, Enums.MeasureType.LengthAndWidth) * toUnit.GetExchangeData(calc_Width1.UnitCode, Enums.MeasureType.LengthAndWidth);
					_isChangingLength = true;
					calc_Volume.EditValue = Math.Round(value, GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
					_isChangingLength = false;
				}
			}
			else if (e.FieldName.Equals(calc_Volume.FieldName) && !_isChangingLength)
			{
				calc_Length1.EditValue = 0.0;
				calc_Width1.EditValue = 0.0;
				calc_Height1.EditValue = 0.0;
			}
			UpdateGridViewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		_btnOKClick = false;
		Close();
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				_dtSource = grd_UoM.DataSource as DataTable;
				_dtSource.AcceptChanges();
				_dtReturn = _dtSource.Copy();
				_changeData = true;
				if (_dtSource.Rows.Count == 1)
				{
					_dfltEntry = Function.ParseInt(_dtSource.Rows[0]["UomEntry"]);
				}
				FormMode = Enums.FormViewModeType.View;
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				_btnOKClick = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_SetDefault_Click(object sender, EventArgs e)
	{
		try
		{
			_dfltEntry = Function.ParseInt(gv_UoM.GetFocusedRowCellValue("UomEntry"));
			gv_UoM.LayoutChanged();
			ChangeButtonOKText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_NewRow_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_UoM.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = WinFuntions.GetTableStructure("ITM12");
				if (!dataTable.Columns.Contains("UomCode"))
				{
					dataTable.Columns.Add(new DataColumn
					{
						ColumnName = "UomCode",
						DataType = typeof(string)
					});
					dataTable.Columns.Add(new DataColumn
					{
						ColumnName = "UomName",
						DataType = typeof(string)
					});
				}
			}
			frm_CFL frm = new frm_CFL(Enums.TypeCFL.ListUoMByUoMGroupItemMaster, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm.DataInt1 = _ugpEntry;
			frm.DataString1 = _itemCode;
			frm.DataString2 = "I";
			frm.ShowDialog(this);
			if (frm.ChooseItem != null && !dataTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["UomEntry"]) == Function.ParseInt(frm.ChooseItem["UomEntry"])))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["UomEntry"] = Function.ParseInt(frm.ChooseItem["UomEntry"]);
				dataRow["ItemCode"] = _itemCode;
				dataRow["UomType"] = _uomType;
				dataRow["UomCode"] = Function.ToString(frm.ChooseItem["UomCode"]);
				dataRow["UomName"] = Function.ToString(frm.ChooseItem["UomName"]);
				dataRow["VolUnit"] = GlobalData.GeneralSetting.DefLengthU;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_UoM.DataSource = dataTable;
				grd_UoM.RefreshDataSource();
				ChangeButtonOKText();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeButtonOKText()
	{
		try
		{
			if (!_isLoading)
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

	private void UpdateGridViewRow()
	{
		try
		{
			gv_UoM.SetFocusedRowCellValue("Length1", Function.ParseDouble(calc_Length1.EditValue));
			gv_UoM.SetFocusedRowCellValue("Len1Unit", Function.ParseShort(calc_Length1.UnitCode));
			gv_UoM.SetFocusedRowCellValue("Width1", Function.ParseDouble(calc_Width1.EditValue));
			gv_UoM.SetFocusedRowCellValue("Wdth1Unit", Function.ParseShort(calc_Width1.UnitCode));
			gv_UoM.SetFocusedRowCellValue("Height1", Function.ParseDouble(calc_Height1.EditValue));
			gv_UoM.SetFocusedRowCellValue("Hght1Unit", Function.ParseDouble(calc_Height1.UnitCode));
			gv_UoM.SetFocusedRowCellValue("Volume", Function.ParseDouble(calc_Volume.EditValue));
			gv_UoM.SetFocusedRowCellValue("VolUnit", Function.ParseShort(luk_VolUnit.EditValue));
			gv_UoM.SetFocusedRowCellValue("Weight1", Function.ParseDouble(calc_Weight1.EditValue));
			gv_UoM.SetFocusedRowCellValue("Wght1Unit", Function.ParseDouble(calc_Weight1.UnitCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetRowsDetails(int uomEntry, int ugpEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			return httpClientHelper.Get("UoMInItemGetRowsDetails", new { uomEntry, ugpEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_UoMInItem));
		this.lb_BarCode = new Apzon.Controls.Controls.LabelLine();
		this.txt_ItemCode = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txt_UomGroup = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_InventoryUom = new Apzon.Controls.Editor.TextEditEx();
		this.grd_UoM = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_UoM = new Apzon.Controls.Grid.GridViewEx();
		this.gcWhseName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UomEntry = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Pkg = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Pkg = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SetDefault = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_UnitName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_UomCode = new System.Windows.Forms.Label();
		this.label2 = new System.Windows.Forms.Label();
		this.lbl_InvUomCode = new System.Windows.Forms.Label();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.luk_VolUnit = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.calc_Volume = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.lookUpEdit1 = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.calcEdit3 = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine12 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine13 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine14 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine16 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine17 = new Apzon.Controls.Controls.LabelLine();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_NewRow = new Apzon.Controls.Controls.SimpleButtonEx();
		this.calc_ItemPerUnit = new Apzon.Controls.Editor.CalcEditEx();
		this.calc_BaseUnit = new Apzon.Controls.Editor.CalcEditEx();
		this.calcEdit8 = new Apzon.Controls.Editor.CalcEditEx();
		this.calc_Length1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_Width1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_Height1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calc_Weight1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calcEdit1 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calcEdit5 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calcEdit4 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		this.calcEdit2 = new Apzon.Controls.Editor.MeasureUnitEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UomGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InventoryUom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_UoM).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_UoM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UomEntry).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Pkg).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Pkg).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UnitName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_VolUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Volume.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ItemPerUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BaseUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit8.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Length1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Width1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Height1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Weight1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit5.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit4.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit2.Properties).BeginInit();
		base.SuspendLayout();
		this.lb_BarCode.FieldName = null;
		this.lb_BarCode.HideWhenEmpty = false;
		this.lb_BarCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_BarCode.Location = new System.Drawing.Point(8, 9);
		this.lb_BarCode.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_BarCode.Name = "lb_BarCode";
		this.lb_BarCode.ShowUnderLine = true;
		this.lb_BarCode.Size = new System.Drawing.Size(84, 20);
		this.lb_BarCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_BarCode.TabIndex = 11;
		this.lb_BarCode.TabStop = false;
		this.lb_BarCode.Utext = "Item No.";
		this.txt_ItemCode.Enabled = false;
		this.txt_ItemCode.FieldName = null;
		this.txt_ItemCode.IsSearchControl = false;
		this.txt_ItemCode.Location = new System.Drawing.Point(91, 6);
		this.txt_ItemCode.Name = "txt_ItemCode";
		this.txt_ItemCode.Size = new System.Drawing.Size(151, 20);
		this.txt_ItemCode.TabIndex = 10;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(272, 10);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(84, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 13;
		this.labelLine1.TabStop = false;
		this.labelLine1.Utext = "UoM Group";
		this.txt_UomGroup.Enabled = false;
		this.txt_UomGroup.FieldName = null;
		this.txt_UomGroup.IsSearchControl = false;
		this.txt_UomGroup.Location = new System.Drawing.Point(355, 7);
		this.txt_UomGroup.Name = "txt_UomGroup";
		this.txt_UomGroup.Size = new System.Drawing.Size(103, 20);
		this.txt_UomGroup.TabIndex = 12;
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(491, 7);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(102, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 15;
		this.labelLine2.TabStop = false;
		this.labelLine2.Utext = "Inventory UoM";
		this.txt_InventoryUom.Enabled = false;
		this.txt_InventoryUom.FieldName = null;
		this.txt_InventoryUom.IsSearchControl = false;
		this.txt_InventoryUom.Location = new System.Drawing.Point(592, 5);
		this.txt_InventoryUom.Name = "txt_InventoryUom";
		this.txt_InventoryUom.Size = new System.Drawing.Size(103, 20);
		this.txt_InventoryUom.TabIndex = 14;
		this.grd_UoM.AllowPasteExcel = false;
		this.grd_UoM.ClipboardData = null;
		this.grd_UoM.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_UoM.Location = new System.Drawing.Point(8, 33);
		this.grd_UoM.MainView = this.gv_UoM;
		this.grd_UoM.Name = "grd_UoM";
		this.grd_UoM.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_UomEntry });
		this.grd_UoM.Size = new System.Drawing.Size(290, 146);
		this.grd_UoM.TabIndex = 46;
		this.grd_UoM.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_UoM });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(141, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.removeRowToolStripMenuItem.Text = "Remove row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_UoM.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_UoM.ColumnKey = null;
		this.gv_UoM.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[22]
		{
			this.gcWhseName, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20,
			this.gridColumn21, this.gridColumn22
		});
		this.gv_UoM.DisableFormSetting = false;
		this.gv_UoM.DisplayException = false;
		this.gv_UoM.EditColumn = null;
		this.gv_UoM.FieldCurrency = null;
		this.gv_UoM.FieldRate = null;
		this.gv_UoM.FormId = "oUomInItem";
		this.gv_UoM.GridControl = this.grd_UoM;
		this.gv_UoM.IndexEditRow = -1;
		this.gv_UoM.IndicatorWidth = 35;
		this.gv_UoM.IsApzTable = false;
		this.gv_UoM.IsInsertNewRow = false;
		this.gv_UoM.IsLoadingData = false;
		this.gv_UoM.ItemId = "OUOM";
		this.gv_UoM.LstColumnAllowBatch = null;
		this.gv_UoM.LstColumnNotAllowBatch = null;
		this.gv_UoM.LstColumNotSetting = null;
		this.gv_UoM.Name = "gv_UoM";
		this.gv_UoM.OptionsView.ShowFooter = true;
		this.gv_UoM.OptionsView.ShowGroupPanel = false;
		this.gv_UoM.UseBatchMod = false;
		this.gv_UoM.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_UoM_RowStyle);
		this.gv_UoM.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_UoM_FocusedRowObjectChanged);
		this.gcWhseName.Caption = "Purchasing UoM";
		this.gcWhseName.ColumnEdit = this.rep_UomEntry;
		this.gcWhseName.CurrencyDateField = null;
		this.gcWhseName.CurrencyField = null;
		this.gcWhseName.CurrencyFixed = null;
		this.gcWhseName.CurrencyFixedRate = 0.0;
		this.gcWhseName.CurrencyRateField = null;
		this.gcWhseName.FieldName = "UomEntry";
		this.gcWhseName.Name = "gcWhseName";
		this.gcWhseName.OptionsColumn.AllowEdit = false;
		this.gcWhseName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcWhseName.OptionsColumn.ReadOnly = true;
		this.gcWhseName.PrimaryKey = false;
		this.gcWhseName.UnitField = null;
		this.gcWhseName.Visible = true;
		this.gcWhseName.VisibleIndex = 0;
		this.rep_UomEntry.AutoHeight = false;
		this.rep_UomEntry.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UomEntry.Name = "rep_UomEntry";
		this.gridColumn2.Caption = "UomType";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "UomType";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "ItemCode";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ItemCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn4.Caption = "BcdEntDft";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "BcdEntDft";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn5.Caption = "Height1";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Height1";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn6.Caption = "Hght1Unit";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Hght1Unit";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowEdit = false;
		this.gridColumn6.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn7.Caption = "Height2";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "Height2";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowEdit = false;
		this.gridColumn7.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "Hght2Unit";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "Hght2Unit";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.AllowEdit = false;
		this.gridColumn8.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn9.Caption = "Width1";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Width1";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.AllowEdit = false;
		this.gridColumn9.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn10.Caption = "Wdth1Unit";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Wdth1Unit";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.AllowEdit = false;
		this.gridColumn10.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn11.Caption = "Width2";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Width2";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.AllowEdit = false;
		this.gridColumn11.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn12.Caption = "Wdth2Unit";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "Wdth2Unit";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.AllowEdit = false;
		this.gridColumn12.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn13.Caption = "Length1";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "Length1";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowEdit = false;
		this.gridColumn13.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "Len1Unit";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Len1Unit";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.AllowEdit = false;
		this.gridColumn14.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "Length2";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "Length2";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.OptionsColumn.AllowEdit = false;
		this.gridColumn15.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn15.OptionsColumn.ReadOnly = true;
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn16.Caption = "Len2Unit";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "Len2Unit";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.AllowEdit = false;
		this.gridColumn16.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn17.Caption = "Volume";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "Volume";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.AllowEdit = false;
		this.gridColumn17.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn18.Caption = "VolUnit";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "VolUnit";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.AllowEdit = false;
		this.gridColumn18.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn19.Caption = "Weight1";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "Weight1";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.AllowEdit = false;
		this.gridColumn19.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn20.Caption = "Wght1Unit";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "Wght1Unit";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsColumn.AllowEdit = false;
		this.gridColumn20.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn20.OptionsColumn.ReadOnly = true;
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn21.Caption = "Weight2";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Weight2";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsColumn.AllowEdit = false;
		this.gridColumn21.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn21.OptionsColumn.ReadOnly = true;
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn22.Caption = "Wght2Unit";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "Wght2Unit";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.OptionsColumn.AllowEdit = false;
		this.gridColumn22.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn22.OptionsColumn.ReadOnly = true;
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.grd_Pkg.AllowPasteExcel = false;
		this.grd_Pkg.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Pkg.ClipboardData = null;
		this.grd_Pkg.Enabled = false;
		this.grd_Pkg.Location = new System.Drawing.Point(405, 33);
		this.grd_Pkg.MainView = this.gv_Pkg;
		this.grd_Pkg.Name = "grd_Pkg";
		this.grd_Pkg.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2 });
		this.grd_Pkg.Size = new System.Drawing.Size(290, 146);
		this.grd_Pkg.TabIndex = 47;
		this.grd_Pkg.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Pkg });
		this.gv_Pkg.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Pkg.ColumnKey = null;
		this.gv_Pkg.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[1] { this.gridColumn1 });
		this.gv_Pkg.DisableFormSetting = false;
		this.gv_Pkg.DisplayException = false;
		this.gv_Pkg.EditColumn = null;
		this.gv_Pkg.FieldCurrency = null;
		this.gv_Pkg.FieldRate = null;
		this.gv_Pkg.FormId = "oUomInItem";
		this.gv_Pkg.GridControl = this.grd_Pkg;
		this.gv_Pkg.IndexEditRow = -1;
		this.gv_Pkg.IndicatorWidth = 35;
		this.gv_Pkg.IsApzTable = false;
		this.gv_Pkg.IsInsertNewRow = false;
		this.gv_Pkg.IsLoadingData = false;
		this.gv_Pkg.ItemId = "OWEIGHT";
		this.gv_Pkg.LstColumnAllowBatch = null;
		this.gv_Pkg.LstColumnNotAllowBatch = null;
		this.gv_Pkg.LstColumNotSetting = null;
		this.gv_Pkg.Name = "gv_Pkg";
		this.gv_Pkg.OptionsView.ShowFooter = true;
		this.gv_Pkg.OptionsView.ShowGroupPanel = false;
		this.gv_Pkg.UseBatchMod = false;
		this.gridColumn1.Caption = "Package Types";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "PackageTypes";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueGrayed = "N";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(85, 443);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 49;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(8, 443);
		this.btOK.Name = "btOK";
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 48;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.btn_SetDefault.FieldName = null;
		this.btn_SetDefault.IsSearchControl = false;
		this.btn_SetDefault.Location = new System.Drawing.Point(8, 185);
		this.btn_SetDefault.Name = "btn_SetDefault";
		this.btn_SetDefault.Size = new System.Drawing.Size(84, 23);
		this.btn_SetDefault.TabIndex = 50;
		this.btn_SetDefault.Text = "Set as Default";
		this.btn_SetDefault.Click += new System.EventHandler(this.btn_SetDefault_Click);
		this.txt_UnitName.Enabled = false;
		this.txt_UnitName.FieldName = null;
		this.txt_UnitName.IsSearchControl = false;
		this.txt_UnitName.Location = new System.Drawing.Point(91, 218);
		this.txt_UnitName.Name = "txt_UnitName";
		this.txt_UnitName.Size = new System.Drawing.Size(151, 20);
		this.txt_UnitName.TabIndex = 51;
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(8, 221);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(84, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 52;
		this.labelLine3.TabStop = false;
		this.labelLine3.Utext = "Unit Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(8, 247);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(84, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 54;
		this.labelLine4.TabStop = false;
		this.labelLine4.Utext = "Items per Unit";
		this.lbl_UomCode.AutoSize = true;
		this.lbl_UomCode.Location = new System.Drawing.Point(162, 249);
		this.lbl_UomCode.Name = "lbl_UomCode";
		this.lbl_UomCode.Size = new System.Drawing.Size(13, 13);
		this.lbl_UomCode.TabIndex = 55;
		this.lbl_UomCode.Text = "1";
		this.label2.AutoSize = true;
		this.label2.Location = new System.Drawing.Point(197, 249);
		this.label2.Name = "label2";
		this.label2.Size = new System.Drawing.Size(15, 13);
		this.label2.TabIndex = 56;
		this.label2.Text = "=";
		this.lbl_InvUomCode.AutoSize = true;
		this.lbl_InvUomCode.Location = new System.Drawing.Point(286, 249);
		this.lbl_InvUomCode.Name = "lbl_InvUomCode";
		this.lbl_InvUomCode.Size = new System.Drawing.Size(13, 13);
		this.lbl_InvUomCode.TabIndex = 58;
		this.lbl_InvUomCode.Text = "1";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(11, 283);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(145, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 59;
		this.labelLine5.TabStop = false;
		this.labelLine5.Utext = "Purchasing UoM Dimensions";
		this.luk_VolUnit.Enabled = false;
		this.luk_VolUnit.FieldName = null;
		this.luk_VolUnit.IsSearchControl = false;
		this.luk_VolUnit.Location = new System.Drawing.Point(248, 369);
		this.luk_VolUnit.Name = "luk_VolUnit";
		this.luk_VolUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_VolUnit.Properties.NullText = "";
		this.luk_VolUnit.Properties.PopupFormMinSize = new System.Drawing.Size(35, 150);
		this.luk_VolUnit.Properties.PopupWidth = 50;
		this.luk_VolUnit.Size = new System.Drawing.Size(44, 20);
		this.luk_VolUnit.TabIndex = 86;
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(12, 309);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(117, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 85;
		this.labelLine8.TabStop = false;
		this.labelLine8.Utext = "Length";
		this.calc_Volume.Enabled = false;
		this.calc_Volume.FieldName = "Volume";
		this.calc_Volume.IsSearchControl = false;
		this.calc_Volume.Location = new System.Drawing.Point(128, 369);
		this.calc_Volume.Name = "calc_Volume";
		this.calc_Volume.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_Volume.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_Volume.Properties.IsSearchControl = false;
		this.calc_Volume.Size = new System.Drawing.Size(114, 20);
		this.calc_Volume.TabIndex = 78;
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(12, 393);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(117, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 83;
		this.labelLine6.TabStop = false;
		this.labelLine6.Utext = "Weight";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(12, 372);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(117, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 82;
		this.labelLine7.TabStop = false;
		this.labelLine7.Utext = "Volume";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(12, 351);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(117, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 81;
		this.labelLine9.TabStop = false;
		this.labelLine9.Utext = "Height";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(12, 330);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(117, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 80;
		this.labelLine10.TabStop = false;
		this.labelLine10.Utext = "Width";
		this.lookUpEdit1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lookUpEdit1.Enabled = false;
		this.lookUpEdit1.FieldName = null;
		this.lookUpEdit1.IsSearchControl = false;
		this.lookUpEdit1.Location = new System.Drawing.Point(645, 369);
		this.lookUpEdit1.Name = "lookUpEdit1";
		this.lookUpEdit1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit1.Properties.NullText = "";
		this.lookUpEdit1.Properties.PopupFormMinSize = new System.Drawing.Size(35, 150);
		this.lookUpEdit1.Properties.PopupWidth = 50;
		this.lookUpEdit1.Size = new System.Drawing.Size(44, 20);
		this.lookUpEdit1.TabIndex = 107;
		this.labelLine11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(409, 309);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(117, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 106;
		this.labelLine11.TabStop = false;
		this.labelLine11.Utext = "Length";
		this.calcEdit3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calcEdit3.Enabled = false;
		this.calcEdit3.FieldName = null;
		this.calcEdit3.IsSearchControl = false;
		this.calcEdit3.Location = new System.Drawing.Point(525, 369);
		this.calcEdit3.Name = "calcEdit3";
		this.calcEdit3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcEdit3.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calcEdit3.Properties.IsSearchControl = false;
		this.calcEdit3.Size = new System.Drawing.Size(114, 20);
		this.calcEdit3.TabIndex = 99;
		this.labelLine12.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine12.FieldName = null;
		this.labelLine12.HideWhenEmpty = false;
		this.labelLine12.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine12.Location = new System.Drawing.Point(409, 393);
		this.labelLine12.Name = "labelLine12";
		this.labelLine12.ShowUnderLine = true;
		this.labelLine12.Size = new System.Drawing.Size(117, 20);
		this.labelLine12.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine12.TabIndex = 104;
		this.labelLine12.TabStop = false;
		this.labelLine12.Utext = "Weight";
		this.labelLine13.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine13.FieldName = null;
		this.labelLine13.HideWhenEmpty = false;
		this.labelLine13.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine13.Location = new System.Drawing.Point(409, 372);
		this.labelLine13.Name = "labelLine13";
		this.labelLine13.ShowUnderLine = true;
		this.labelLine13.Size = new System.Drawing.Size(117, 20);
		this.labelLine13.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine13.TabIndex = 103;
		this.labelLine13.TabStop = false;
		this.labelLine13.Utext = "Volume";
		this.labelLine14.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine14.FieldName = null;
		this.labelLine14.HideWhenEmpty = false;
		this.labelLine14.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine14.Location = new System.Drawing.Point(409, 351);
		this.labelLine14.Name = "labelLine14";
		this.labelLine14.ShowUnderLine = true;
		this.labelLine14.Size = new System.Drawing.Size(117, 20);
		this.labelLine14.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine14.TabIndex = 102;
		this.labelLine14.TabStop = false;
		this.labelLine14.Utext = "Height";
		this.labelLine15.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(409, 330);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(117, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 101;
		this.labelLine15.TabStop = false;
		this.labelLine15.Utext = "Width";
		this.labelLine16.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine16.FieldName = null;
		this.labelLine16.HideWhenEmpty = false;
		this.labelLine16.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine16.Location = new System.Drawing.Point(408, 283);
		this.labelLine16.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine16.Name = "labelLine16";
		this.labelLine16.ShowUnderLine = true;
		this.labelLine16.Size = new System.Drawing.Size(145, 20);
		this.labelLine16.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine16.TabIndex = 96;
		this.labelLine16.TabStop = false;
		this.labelLine16.Utext = "Package Type Dimensions";
		this.labelLine17.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine17.FieldName = null;
		this.labelLine17.HideWhenEmpty = false;
		this.labelLine17.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine17.Location = new System.Drawing.Point(408, 217);
		this.labelLine17.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.labelLine17.Name = "labelLine17";
		this.labelLine17.ShowUnderLine = true;
		this.labelLine17.Size = new System.Drawing.Size(98, 20);
		this.labelLine17.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine17.TabIndex = 91;
		this.labelLine17.TabStop = false;
		this.labelLine17.Utext = "Qty per Package";
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.Enabled = false;
		this.simpleButton1.FieldName = null;
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(405, 185);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(101, 23);
		this.simpleButton1.TabIndex = 87;
		this.simpleButton1.Text = "Set as Default";
		this.btn_NewRow.FieldName = null;
		this.btn_NewRow.IsSearchControl = false;
		this.btn_NewRow.Location = new System.Drawing.Point(111, 185);
		this.btn_NewRow.Name = "btn_NewRow";
		this.btn_NewRow.Size = new System.Drawing.Size(50, 23);
		this.btn_NewRow.TabIndex = 108;
		this.btn_NewRow.Text = "New row";
		this.btn_NewRow.Click += new System.EventHandler(this.btn_NewRow_Click);
		this.calc_ItemPerUnit.Enabled = false;
		this.calc_ItemPerUnit.FieldName = null;
		this.calc_ItemPerUnit.IsSearchControl = false;
		this.calc_ItemPerUnit.Location = new System.Drawing.Point(91, 244);
		this.calc_ItemPerUnit.Name = "calc_ItemPerUnit";
		this.calc_ItemPerUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_ItemPerUnit.Properties.IsSearchControl = false;
		this.calc_ItemPerUnit.Size = new System.Drawing.Size(70, 20);
		this.calc_ItemPerUnit.TabIndex = 109;
		this.calc_BaseUnit.Enabled = false;
		this.calc_BaseUnit.FieldName = null;
		this.calc_BaseUnit.IsSearchControl = false;
		this.calc_BaseUnit.Location = new System.Drawing.Point(213, 244);
		this.calc_BaseUnit.Name = "calc_BaseUnit";
		this.calc_BaseUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_BaseUnit.Properties.IsSearchControl = false;
		this.calc_BaseUnit.Size = new System.Drawing.Size(70, 20);
		this.calc_BaseUnit.TabIndex = 110;
		this.calcEdit8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.calcEdit8.Enabled = false;
		this.calcEdit8.FieldName = null;
		this.calcEdit8.IsSearchControl = false;
		this.calcEdit8.Location = new System.Drawing.Point(504, 214);
		this.calcEdit8.Name = "calcEdit8";
		this.calcEdit8.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcEdit8.Properties.IsSearchControl = false;
		this.calcEdit8.Size = new System.Drawing.Size(120, 20);
		this.calcEdit8.TabIndex = 111;
		this.calc_Length1.FieldName = "Length";
		this.calc_Length1.IsSearchControl = false;
		this.calc_Length1.Location = new System.Drawing.Point(128, 306);
		this.calc_Length1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_Length1.Name = "calc_Length1";
		this.calc_Length1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_Length1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_Length1.Size = new System.Drawing.Size(114, 20);
		this.calc_Length1.TabIndex = 112;
		this.calc_Width1.FieldName = "Width";
		this.calc_Width1.IsSearchControl = false;
		this.calc_Width1.Location = new System.Drawing.Point(128, 327);
		this.calc_Width1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_Width1.Name = "calc_Width1";
		this.calc_Width1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_Width1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_Width1.Size = new System.Drawing.Size(114, 20);
		this.calc_Width1.TabIndex = 113;
		this.calc_Height1.FieldName = "Height";
		this.calc_Height1.IsSearchControl = false;
		this.calc_Height1.Location = new System.Drawing.Point(128, 348);
		this.calc_Height1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calc_Height1.Name = "calc_Height1";
		this.calc_Height1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_Height1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_Height1.Size = new System.Drawing.Size(114, 20);
		this.calc_Height1.TabIndex = 114;
		this.calc_Weight1.FieldName = null;
		this.calc_Weight1.IsSearchControl = false;
		this.calc_Weight1.Location = new System.Drawing.Point(128, 390);
		this.calc_Weight1.Name = "calc_Weight1";
		this.calc_Weight1.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_Weight1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_Weight1.Size = new System.Drawing.Size(114, 20);
		this.calc_Weight1.TabIndex = 115;
		this.calcEdit1.FieldName = null;
		this.calcEdit1.IsSearchControl = false;
		this.calcEdit1.Location = new System.Drawing.Point(525, 306);
		this.calcEdit1.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calcEdit1.Name = "calcEdit1";
		this.calcEdit1.Properties.Appearance.Options.UseTextOptions = true;
		this.calcEdit1.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcEdit1.Size = new System.Drawing.Size(114, 20);
		this.calcEdit1.TabIndex = 116;
		this.calcEdit5.FieldName = null;
		this.calcEdit5.IsSearchControl = false;
		this.calcEdit5.Location = new System.Drawing.Point(525, 327);
		this.calcEdit5.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calcEdit5.Name = "calcEdit5";
		this.calcEdit5.Properties.Appearance.Options.UseTextOptions = true;
		this.calcEdit5.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcEdit5.Size = new System.Drawing.Size(114, 20);
		this.calcEdit5.TabIndex = 117;
		this.calcEdit4.FieldName = null;
		this.calcEdit4.IsSearchControl = false;
		this.calcEdit4.Location = new System.Drawing.Point(525, 348);
		this.calcEdit4.MeasureUnitType = Apzon.Commons.Enums.Enums.MeasureType.LengthAndWidth;
		this.calcEdit4.Name = "calcEdit4";
		this.calcEdit4.Properties.Appearance.Options.UseTextOptions = true;
		this.calcEdit4.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcEdit4.Size = new System.Drawing.Size(114, 20);
		this.calcEdit4.TabIndex = 118;
		this.calcEdit2.FieldName = null;
		this.calcEdit2.IsSearchControl = false;
		this.calcEdit2.Location = new System.Drawing.Point(525, 390);
		this.calcEdit2.Name = "calcEdit2";
		this.calcEdit2.Properties.Appearance.Options.UseTextOptions = true;
		this.calcEdit2.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calcEdit2.Size = new System.Drawing.Size(114, 20);
		this.calcEdit2.TabIndex = 119;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(703, 472);
		base.Controls.Add(this.calcEdit2);
		base.Controls.Add(this.calcEdit4);
		base.Controls.Add(this.calcEdit5);
		base.Controls.Add(this.calcEdit1);
		base.Controls.Add(this.calc_Weight1);
		base.Controls.Add(this.calc_Height1);
		base.Controls.Add(this.calc_Width1);
		base.Controls.Add(this.calc_Length1);
		base.Controls.Add(this.calcEdit8);
		base.Controls.Add(this.calc_BaseUnit);
		base.Controls.Add(this.calc_ItemPerUnit);
		base.Controls.Add(this.btn_NewRow);
		base.Controls.Add(this.lookUpEdit1);
		base.Controls.Add(this.labelLine11);
		base.Controls.Add(this.calcEdit3);
		base.Controls.Add(this.labelLine12);
		base.Controls.Add(this.labelLine13);
		base.Controls.Add(this.labelLine14);
		base.Controls.Add(this.labelLine15);
		base.Controls.Add(this.labelLine16);
		base.Controls.Add(this.labelLine17);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.luk_VolUnit);
		base.Controls.Add(this.labelLine8);
		base.Controls.Add(this.calc_Volume);
		base.Controls.Add(this.labelLine6);
		base.Controls.Add(this.labelLine7);
		base.Controls.Add(this.labelLine9);
		base.Controls.Add(this.labelLine10);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.lbl_InvUomCode);
		base.Controls.Add(this.label2);
		base.Controls.Add(this.lbl_UomCode);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.txt_UnitName);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.btn_SetDefault);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grd_Pkg);
		base.Controls.Add(this.grd_UoM);
		base.Controls.Add(this.txt_ItemCode);
		base.Controls.Add(this.txt_UomGroup);
		base.Controls.Add(this.txt_InventoryUom);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lb_BarCode);
		base.FormId = "oUomInItem";
		base.Name = "frm_UoMInItem";
		base.ShowIcon = false;
		this.Text = "Purchasing UoM and Package Types";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_UoMInItem_OnControlValueChanged);
		base.Load += new System.EventHandler(this.frm_UoMInItem_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UomGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_InventoryUom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_UoM).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_UoM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UomEntry).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Pkg).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Pkg).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UnitName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_VolUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Volume.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ItemPerUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_BaseUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit8.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Length1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Width1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Height1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Weight1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit5.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit4.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEdit2.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
