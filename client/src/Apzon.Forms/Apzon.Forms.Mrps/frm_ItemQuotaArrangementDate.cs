using System;
using System.Collections.Generic;
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

namespace Apzon.Forms.Mrps;

public class frm_ItemQuotaArrangementDate : XtraFormEx
{
	private string _formController = "ItemQuotaArrangementDate";

	private int _colVendorRHandle = -1;

	public int _PeriodID = -1;

	private IContainer components;

	private GridControlEx grd_Date;

	private GridViewEx gv_Date;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private RepositoryItemButtonEdit rep_Vendor;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	protected override void SetFormMode(Enums.FormViewModeType value)
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
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ItemQuotaArrangementDate()
	{
		InitializeComponent();
	}

	private void frm_ItemQuotaArrangementDate_Load(object sender, EventArgs e)
	{
		GetDocs();
		SetFormatControlValue();
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Date.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetTableStructure().Clone();
			}
			if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrWhiteSpace(Function.ToString(t["CardCode"]))))
			{
				DataRow row = dataTable.NewRow();
				dataTable.Rows.Add(row);
				dataTable.AcceptChanges();
			}
			grd_Date.DataSource = dataTable;
			grd_Date.RefreshDataSource();
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
			DataTable dataSourceTable = grd_Date.DataSourceTable;
			if (dataSourceTable.Rows.Count > 0)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			gv_Date.DeleteSelectedRows();
			dataSourceTable.AcceptChanges();
			grd_Date.DataSource = dataSourceTable;
			grd_Date.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (ValidateData())
			{
				SaveData();
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
		Close();
	}

	private void rep_Vendor_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Vendor, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					List<DataRow> chooseItems = frm_CFL2.ChooseItems;
					DataTable dataTable = grd_Date.DataSource as DataTable;
					DataRow dataRow = dataTable.Rows[gv_Date.GetFocusedDataSourceRowIndex()];
					DataRow dataRow2 = chooseItems[0];
					dataRow["CardCode"] = Function.ToString(dataRow2["CardCode"]);
					dataRow["CardName"] = Function.ToString(dataRow2["CardName"]);
					dataRow["PeriodID"] = _PeriodID;
					for (int i = 1; i < chooseItems.Count; i++)
					{
						DataRow dataRow3 = chooseItems[i];
						DataRow dataRow4 = dataTable.NewRow();
						dataRow4["CardCode"] = Function.ToString(dataRow3["CardCode"]);
						dataRow4["CardName"] = Function.ToString(dataRow3["CardName"]);
						dataRow4["PeriodID"] = _PeriodID;
						dataTable.Rows.Add(dataRow4);
					}
					dataTable.AcceptChanges();
					grd_Date.DataSource = dataTable;
					grd_Date.RefreshDataSource();
					gv_Date.RefreshRow(gv_Date.FocusedRowHandle);
				}
			}
			else if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_Date.GetFocusedRowCellValue("CardCode"));
				if (WinFuntions.CheckExistValidValue("CheckExistValidVendor", text))
				{
					this.DrillDown(ApzObjectTypes.oBusinessPartners, ApzObjectTypes.oBusinessPartners, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Date_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void rep_Vendor_Leave(object sender, EventArgs e)
	{
		try
		{
			if (_colVendorRHandle > -1)
			{
				string cardCode = Function.ToString(gv_Date.GetRowCellValue(_colVendorRHandle, "CardCode"));
				string cardName = GetCardName(cardCode);
				gv_Date.SetRowCellValue(_colVendorRHandle, "CardName", cardName);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Vendor_Enter(object sender, EventArgs e)
	{
		try
		{
			_colVendorRHandle = gv_Date.FocusedRowHandle;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Date_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			addToolStripMenuItem_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Date_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Date, gv_Date, new List<string> { "CardCode" });
				DataTable dataSourceTable = grd_Date.DataSourceTable;
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					dataSourceTable.Rows[i]["PeriodID"] = _PeriodID;
				}
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetDocs()
	{
		try
		{
			int periodID = _PeriodID;
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataSource = httpClientHelper.Get("DateGetDocs", new { periodID });
			grd_Date.DataSource = dataSource;
			grd_Date.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveData()
	{
		DisableControls(p: true);
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dataTable = grd_Date.DataSourceTable.Copy();
				for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
				{
					DataRow dataRow = dataTable.Rows[num];
					if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["CardCode"])))
					{
						dataTable.Rows.Remove(dataRow);
					}
				}
				dataTable.AcceptChanges();
				if (dataTable.Rows.Count == 0)
				{
					string text = DeleteRows(_PeriodID);
					if (text == "Y")
					{
						WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
					DisableControls(p: false);
					return;
				}
				DataSet dataSet = new DataSet();
				DataSet dataSet2 = dataTable.DataSet;
				dataTable.TableName = "Date";
				if (dataSet2 != null)
				{
					dataSet2.Tables.Remove("Date");
					dataSet2.AcceptChanges();
				}
				dataSet.Tables.Clear();
				dataSet.Tables.Add(dataTable);
				using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
				string text2 = httpClientHelper.Post("DateSave", dataSet);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					GetDocs();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		DisableControls(p: false);
	}

	private string DeleteRows(int periodID)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			return httpClientHelper.Post("DeleteRows", periodID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	private void DisableControls(bool p)
	{
		try
		{
			btn_Save.Enabled = !p;
			btn_Cancel.Enabled = !p;
			grd_Date.Enabled = !p;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool ValidateData()
	{
		try
		{
			IEnumerable<IGrouping<string, DataRow>> source = from r in grd_Date.DataSourceTable.AsEnumerable()
				group r by Function.ToString(r["CardCode"]).ToLower().Trim() into gr
				where gr.Count() > 1
				select gr;
			if (source.Any())
			{
				WinCommons.Error(Enums.MessageCode.DuplicateVendorCodeWithCode, "", Function.ToString(source.First().First()["CardCode"]));
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private DataTable GetTableStructure()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(int),
				ColumnName = "PeriodID",
				DefaultValue = _PeriodID
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(string),
				ColumnName = "CardCode",
				DefaultValue = ""
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(string),
				ColumnName = "CardName",
				DefaultValue = ""
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(double),
				ColumnName = "LeadTime",
				DefaultValue = 0.0
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(double),
				ColumnName = "Per",
				DefaultValue = 0.0
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(double),
				ColumnName = "MaxQty",
				DefaultValue = 0.0
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(double),
				ColumnName = "BaseQty",
				DefaultValue = 0.0
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(double),
				ColumnName = "MaxLot",
				DefaultValue = 0.0
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(double),
				ColumnName = "MinLot",
				DefaultValue = 0.0
			});
			dataTable.Columns.Add(new DataColumn
			{
				DataType = typeof(string),
				ColumnName = "Remarks",
				DefaultValue = ""
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
	}

	private void SetFormatControlValue()
	{
		try
		{
			gv_Date.Columns["LeadTime"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Date.Columns["LeadTime"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_Date.Columns["Per"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Date.Columns["Per"].DisplayFormat.FormatString = "n" + GlobalData.PercentageRateFormat.NumberDecimalDigits;
			gv_Date.Columns["MaxQty"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Date.Columns["MaxQty"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_Date.Columns["BaseQty"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Date.Columns["BaseQty"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_Date.Columns["MaxLot"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Date.Columns["MaxLot"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			gv_Date.Columns["MinLot"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Date.Columns["MinLot"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string GetCardName(string CardCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = httpClientHelper.Get("GetCardName", new { CardCode });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_ItemQuotaArrangementDate));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Date = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Date = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Vendor = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Date).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Vendor).BeginInit();
		base.SuspendLayout();
		this.grd_Date.AllowPasteExcel = true;
		this.grd_Date.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Date.ClipboardData = null;
		this.grd_Date.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Date.Location = new System.Drawing.Point(6, 1);
		this.grd_Date.MainView = this.gv_Date;
		this.grd_Date.Name = "grd_Date";
		this.grd_Date.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Vendor });
		this.grd_Date.Size = new System.Drawing.Size(746, 304);
		this.grd_Date.TabIndex = 2;
		this.grd_Date.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Date });
		this.grd_Date.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Date_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.addToolStripMenuItem.Text = "Add Row";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Date.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Date.ColumnKey = null;
		this.gv_Date.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn1, this.gridColumn7, this.gridColumn11, this.gridColumn12, this.gridColumn13,
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3
		});
		this.gv_Date.DisableFormSetting = false;
		this.gv_Date.DisplayException = false;
		this.gv_Date.EditColumn = null;
		this.gv_Date.FieldCurrency = null;
		this.gv_Date.FieldRate = null;
		this.gv_Date.FormId = "oItemQuotaArrDate";
		this.gv_Date.GridControl = this.grd_Date;
		this.gv_Date.IgnoreFilter = false;
		this.gv_Date.IndexEditRow = -1;
		this.gv_Date.IndicatorWidth = 70;
		this.gv_Date.IsApzTable = false;
		this.gv_Date.IsInsertNewRow = false;
		this.gv_Date.IsLoadingData = false;
		this.gv_Date.IsMappingContextAction = false;
		this.gv_Date.ItemId = "ODATA";
		this.gv_Date.LstColumnAllowBatch = null;
		this.gv_Date.LstColumnNotAllowBatch = null;
		this.gv_Date.LstColumnNotEditable = null;
		this.gv_Date.LstColumNotSetting = null;
		this.gv_Date.Name = "gv_Date";
		this.gv_Date.OptionsSelection.MultiSelect = true;
		this.gv_Date.OptionsView.ColumnAutoWidth = false;
		this.gv_Date.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Date.OptionsView.ShowGroupPanel = false;
		this.gv_Date.UseBatchMod = false;
		this.gv_Date.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Date_CustomDrawRowIndicator);
		this.gv_Date.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Date_CellValueChanged);
		this.gridColumn2.Caption = "PeriodID";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "PeriodID";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn2.PrimaryKey = true;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "Vendor Code";
		this.gridColumn3.ColumnEdit = this.rep_Vendor;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "CardCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.gridColumn3.Width = 108;
		this.rep_Vendor.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_Vendor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Vendor.Name = "rep_Vendor";
		this.rep_Vendor.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Vendor_ButtonPressed);
		this.rep_Vendor.Enter += new System.EventHandler(this.rep_Vendor_Enter);
		this.rep_Vendor.Leave += new System.EventHandler(this.rep_Vendor_Leave);
		this.gridColumn4.Caption = "Vendor Name";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "CardName";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.gridColumn4.Width = 183;
		this.gridColumn5.Caption = "Lead Time";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "LeadTime";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.gridColumn6.Caption = "%";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Per";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.gridColumn1.Caption = "Maxinum Quantity";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "MaxQty";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 4;
		this.gridColumn1.Width = 106;
		this.gridColumn7.Caption = "Quota Base Qty";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "BaseQty";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 5;
		this.gridColumn7.Width = 93;
		this.gridColumn11.Caption = "Max. Lot Size";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "MaxLot";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 6;
		this.gridColumn12.Caption = "Min. Lot Size ";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "MinLot";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 7;
		this.gridColumn13.Caption = "Remarks";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "Remarks";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 8;
		this.gridColumnEx1.Caption = "Order intervals";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "OrdrIntrvl";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 9;
		this.gridColumnEx2.Caption = "Order Multiple";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "OrdrMulti";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 10;
		this.gridColumnEx3.Caption = "BP Catalog Number";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Substitute";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 11;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 311);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 23;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 311);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 22;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(757, 334);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Date);
		base.FormId = "oItemQuotaArrDate";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemQuotaArrangementDate";
		this.Text = "Item Quota Arrangement - Date";
		base.Load += new System.EventHandler(this.frm_ItemQuotaArrangementDate_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Date).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Vendor).EndInit();
		base.ResumeLayout(false);
	}
}
