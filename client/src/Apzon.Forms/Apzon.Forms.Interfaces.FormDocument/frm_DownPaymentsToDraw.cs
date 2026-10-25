using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Forms.Properties;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Interfaces.FormDocument;

public class frm_DownPaymentsToDraw : XtraFormEx
{
	private DataTable dtSource = new DataTable();

	private bool _allowEdit;

	public DataTable DownPaymentData;

	private IContainer components;

	private TreeEx treeEx1;

	private TreeListColumnEx col_FatherId;

	private TreeListColumnEx col_Id;

	private TreeListColumnEx treeListColumnEx15;

	private TreeListColumnEx treeListColumnEx14;

	private TreeListColumnEx treeListColumnEx13;

	private TreeListColumnEx treeListColumnEx12;

	private TreeListColumnEx col_NetAmount;

	private TreeListColumnEx treeListColumnEx4;

	private TreeListColumnEx col_GrossAmount;

	private TreeListColumnEx col_OpenNet;

	private TreeListColumnEx treeListColumnEx7;

	private TreeListColumnEx col_OpenGross;

	private TreeListColumnEx treeListColumnEx9;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private TreeListColumnEx col_Selecttion;

	private TreeListColumnEx gcDocCur;

	public frm_DownPaymentsToDraw(DataTable source, bool allowEdit = true)
	{
		InitializeComponent();
		dtSource = source.Copy();
		_allowEdit = allowEdit;
		if (!allowEdit)
		{
			col_NetAmount.OptionsColumn.AllowEdit = false;
			col_GrossAmount.OptionsColumn.AllowEdit = false;
			treeEx1.BeforeCheckNode += treeEx1_BeforeCheckNode;
		}
	}

	private void frm_PaymentToDraw_Load(object sender, EventArgs e)
	{
		treeEx1.DataSource = dtSource.Copy();
		if (!_allowEdit)
		{
			treeEx1.CheckAll();
		}
		else
		{
			foreach (TreeListNode item in treeEx1.Nodes.Where((TreeListNode n) => n.ParentNode == null))
			{
				if (Function.ToString(dtSource.Rows[item.Id]["Selected"]) == "Y")
				{
					item.Checked = true;
				}
			}
		}
		treeEx1.CollapseAll();
	}

	private void treeEx1_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		TreeEx treeEx = sender as TreeEx;
		if ((treeEx.FocusedColumn == col_NetAmount && Function.ParseDecimal(e.Value) > Function.ParseDecimal(treeEx.GetFocusedRowCellValue(col_OpenNet))) || (treeEx.FocusedColumn == col_GrossAmount && Function.ParseDecimal(e.Value) > Function.ParseDecimal(treeEx.GetFocusedRowCellValue(col_OpenGross))))
		{
			e.Valid = false;
		}
	}

	private void treeEx1_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		e.ExceptionMode = ExceptionMode.DisplayError;
		WinCommons.Error(Enums.MessageCode.None, "Enter down payment amount that is no greater than the open amount");
	}

	private void treeEx1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column != col_NetAmount && e.Column != col_GrossAmount)
			{
				return;
			}
			DataTable dt = treeEx1.DataSourceTable;
			string fatherId = Function.ToString(treeEx1.GetRowCellValue(e.Node, col_FatherId));
			string currency = Function.ToString(treeEx1.GetRowCellValue(e.Node, gcDocCur));
			CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
			int num = -1;
			if (currencyCodesImpl != null)
			{
				num = currencyCodesImpl.Decimals;
			}
			num = ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
			if (string.IsNullOrEmpty(fatherId))
			{
				decimal num2 = 0m;
				if (e.Column == col_NetAmount)
				{
					num2 = Function.ParseDecimal(e.Value) / Function.ParseDecimal(dtSource.Rows[e.Node.Id]["NetAmount"]);
					dt.Rows[e.Node.Id]["GrossAmount"] = Math.Round(Function.ParseDecimal(dtSource.Rows[e.Node.Id]["GrossAmount"]) * num2, num, MidpointRounding.AwayFromZero);
				}
				else if (e.Column == col_GrossAmount)
				{
					num2 = Function.ParseDecimal(e.Value) / Function.ParseDecimal(dtSource.Rows[e.Node.Id]["GrossAmount"]);
					dt.Rows[e.Node.Id]["NetAmount"] = Math.Round(Function.ParseDecimal(dtSource.Rows[e.Node.Id]["NetAmount"]) * num2, num, MidpointRounding.AwayFromZero);
				}
				dt.Rows[e.Node.Id]["TaxAmount"] = Math.Round(Function.ParseDecimal(dtSource.Rows[e.Node.Id]["TaxAmount"]) * num2, num, MidpointRounding.AwayFromZero);
				string id = Function.ToString(treeEx1.GetRowCellValue(e.Node, col_Id));
				{
					foreach (DataRow item in from r in dt.AsEnumerable()
						where Function.ToString(r["FatherId"]) == id && Function.ParseDecimal(dt.Rows[e.Node.Id]["OpenNetAmount"]) != 0m
						select r)
					{
						item["NetAmount"] = Math.Round(Function.ParseDecimal(dt.Rows[e.Node.Id]["NetAmount"]) * Function.ParseDecimal(item["OpenNetAmount"]) / Function.ParseDecimal(dt.Rows[e.Node.Id]["OpenNetAmount"]), num, MidpointRounding.AwayFromZero);
						item["TaxAmount"] = Math.Round(Function.ParseDecimal(item["NetAmount"]) * Function.ParseDecimal(item["VatPercent"]), num, MidpointRounding.AwayFromZero);
						item["GrossAmount"] = Math.Round(Function.ParseDecimal(item["NetAmount"]) + Function.ParseDecimal(item["TaxAmount"]), num, MidpointRounding.AwayFromZero);
					}
					return;
				}
			}
			if (e.Column == col_NetAmount)
			{
				dt.Rows[e.Node.Id]["TaxAmount"] = Math.Round(Function.ParseDecimal(e.Value) * Function.ParseDecimal(dt.Rows[e.Node.Id]["VatPercent"]), num, MidpointRounding.AwayFromZero);
				dt.Rows[e.Node.Id]["GrossAmount"] = Math.Round(Function.ParseDecimal(e.Value) + Function.ParseDecimal(dt.Rows[e.Node.Id]["TaxAmount"]), num, MidpointRounding.AwayFromZero);
			}
			else if (e.Column == col_GrossAmount)
			{
				dt.Rows[e.Node.Id]["NetAmount"] = Math.Round(Function.ParseDecimal(e.Value) / (1m + Function.ParseDecimal(dt.Rows[e.Node.Id]["VatPercent"])), num, MidpointRounding.AwayFromZero);
				dt.Rows[e.Node.Id]["TaxAmount"] = Math.Round(Function.ParseDecimal(dt.Rows[e.Node.Id]["NetAmount"]) * Function.ParseDecimal(dt.Rows[e.Node.Id]["VatPercent"]), num, MidpointRounding.AwayFromZero);
			}
			DataRow dataRow = dt.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Id"]) == fatherId);
			EnumerableRowCollection<DataRow> source = from r in dt.AsEnumerable()
				where Function.ToString(r["FatherId"]) == fatherId
				select r;
			if (dataRow != null)
			{
				dataRow["NetAmount"] = Math.Round(source.Sum((DataRow r) => Function.ParseDecimal(r["NetAmount"])), num, MidpointRounding.AwayFromZero);
				dataRow["TaxAmount"] = Math.Round(source.Sum((DataRow r) => Function.ParseDecimal(r["TaxAmount"])), num, MidpointRounding.AwayFromZero);
				dataRow["GrossAmount"] = Math.Round(source.Sum((DataRow r) => Function.ParseDecimal(r["GrossAmount"])), num, MidpointRounding.AwayFromZero);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		DownPaymentData = treeEx1.DataSourceTable;
		DialogResult = DialogResult.OK;
		Close();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void treeEx1_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		if ((e.Column != col_NetAmount && e.Column != col_GrossAmount) || Function.ParseDouble(treeEx1.GetRowCellValue(e.Node, col_OpenNet)) == 0.0)
		{
			e.Appearance.BackColor = StyleColor.GridDisable;
		}
	}

	private void treeEx1_AfterCheckNode(object sender, NodeEventArgs e)
	{
		treeEx1.DataSourceTable.Rows[e.Node.Id]["Selected"] = (e.Node.Checked ? "Y" : "N");
	}

	private void treeEx1_BeforeCheckNode(object sender, CheckNodeEventArgs e)
	{
		e.CanCheck = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.FormDocument.frm_DownPaymentsToDraw));
		this.treeEx1 = new Apzon.Controls.Tree.TreeEx();
		this.col_Selecttion = new Apzon.Controls.Tree.TreeListColumnEx();
		this.col_FatherId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.col_Id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx15 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx14 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx13 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx12 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.col_NetAmount = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.col_GrossAmount = new Apzon.Controls.Tree.TreeListColumnEx();
		this.col_OpenNet = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.col_OpenGross = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumnEx9 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gcDocCur = new Apzon.Controls.Tree.TreeListColumnEx();
		((System.ComponentModel.ISupportInitialize)this.treeEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.treeEx1.AllowPasteExcel = false;
		this.treeEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.treeEx1.CancelNextCellValueChangedScript = false;
		this.treeEx1.CancelNextFocusObjectChangedScript = false;
		this.treeEx1.CancelNextSelectedChangedScript = false;
		this.treeEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.treeEx1.ColumnKey = null;
		this.treeEx1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[15]
		{
			this.col_Selecttion, this.col_FatherId, this.col_Id, this.treeListColumnEx15, this.treeListColumnEx14, this.treeListColumnEx13, this.treeListColumnEx12, this.col_NetAmount, this.treeListColumnEx4, this.col_GrossAmount,
			this.col_OpenNet, this.treeListColumnEx7, this.col_OpenGross, this.treeListColumnEx9, this.gcDocCur
		});
		this.treeEx1.DisableFormSetting = true;
		this.treeEx1.EditColumn = null;
		this.treeEx1.FieldCurrency = null;
		this.treeEx1.FieldRate = null;
		this.treeEx1.FormId = null;
		this.treeEx1.HasFormSetting = false;
		this.treeEx1.IndexEditRow = -1;
		this.treeEx1.IndicatorWidth = 40;
		this.treeEx1.IsApzTable = false;
		this.treeEx1.IsLoadingData = false;
		this.treeEx1.ItemId = null;
		this.treeEx1.KeyFieldName = "Id";
		this.treeEx1.Location = new System.Drawing.Point(0, 0);
		this.treeEx1.LstColumnNotEditable = null;
		this.treeEx1.LstColumNotSetting = null;
		this.treeEx1.LstOldUDF = null;
		this.treeEx1.Name = "treeEx1";
		this.treeEx1.OptionsMenu.ShowExpandCollapseItems = false;
		this.treeEx1.OptionsNavigation.AutoMoveRowFocus = true;
		this.treeEx1.OptionsSelection.MultiSelect = true;
		this.treeEx1.OptionsView.RootCheckBoxStyle = DevExpress.XtraTreeList.NodeCheckBoxStyle.Check;
		this.treeEx1.ParentFieldName = "FatherId";
		this.treeEx1.ReadOnly = false;
		this.treeEx1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemDateEditEx1 });
		this.treeEx1.SettingReadonly = false;
		this.treeEx1.Size = new System.Drawing.Size(1087, 340);
		this.treeEx1.TabIndex = 0;
		this.treeEx1.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeEx1_CellValueChanged);
		this.treeEx1.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.treeEx1_NodeCellStyle);
		this.treeEx1.AfterCheckNode += new DevExpress.XtraTreeList.NodeEventHandler(this.treeEx1_AfterCheckNode);
		this.treeEx1.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.treeEx1_ValidatingEditor);
		this.treeEx1.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.treeEx1_InvalidValueException);
		this.col_Selecttion.Caption = "treeListColumnEx1";
		this.col_Selecttion.CurrencyDateField = null;
		this.col_Selecttion.CurrencyField = null;
		this.col_Selecttion.CurrencyFixed = null;
		this.col_Selecttion.CurrencyFixedRate = 0.0;
		this.col_Selecttion.CurrencyRateField = null;
		this.col_Selecttion.FieldName = "Selected";
		this.col_Selecttion.Name = "col_Selecttion";
		this.col_FatherId.Caption = "treeListColumnEx1";
		this.col_FatherId.CurrencyDateField = null;
		this.col_FatherId.CurrencyField = null;
		this.col_FatherId.CurrencyFixed = null;
		this.col_FatherId.CurrencyFixedRate = 0.0;
		this.col_FatherId.CurrencyRateField = null;
		this.col_FatherId.FieldName = "FatherId";
		this.col_FatherId.Name = "col_FatherId";
		this.col_FatherId.OptionsColumn.AllowEdit = false;
		this.col_Id.Caption = "treeListColumnEx2";
		this.col_Id.CurrencyDateField = null;
		this.col_Id.CurrencyField = null;
		this.col_Id.CurrencyFixed = null;
		this.col_Id.CurrencyFixedRate = 0.0;
		this.col_Id.CurrencyRateField = null;
		this.col_Id.FieldName = "Id";
		this.col_Id.Name = "col_Id";
		this.col_Id.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx15.Caption = "Document Number";
		this.treeListColumnEx15.CurrencyDateField = null;
		this.treeListColumnEx15.CurrencyField = null;
		this.treeListColumnEx15.CurrencyFixed = null;
		this.treeListColumnEx15.CurrencyFixedRate = 0.0;
		this.treeListColumnEx15.CurrencyRateField = null;
		this.treeListColumnEx15.FieldName = "DocNum";
		this.treeListColumnEx15.Name = "treeListColumnEx15";
		this.treeListColumnEx15.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx15.Visible = true;
		this.treeListColumnEx15.VisibleIndex = 0;
		this.treeListColumnEx14.Caption = "Document Type";
		this.treeListColumnEx14.CurrencyDateField = null;
		this.treeListColumnEx14.CurrencyField = null;
		this.treeListColumnEx14.CurrencyFixed = null;
		this.treeListColumnEx14.CurrencyFixedRate = 0.0;
		this.treeListColumnEx14.CurrencyRateField = null;
		this.treeListColumnEx14.FieldName = "ObjType";
		this.treeListColumnEx14.Name = "treeListColumnEx14";
		this.treeListColumnEx14.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx14.Visible = true;
		this.treeListColumnEx14.VisibleIndex = 1;
		this.treeListColumnEx13.Caption = "Remarks";
		this.treeListColumnEx13.CurrencyDateField = null;
		this.treeListColumnEx13.CurrencyField = null;
		this.treeListColumnEx13.CurrencyFixed = null;
		this.treeListColumnEx13.CurrencyFixedRate = 0.0;
		this.treeListColumnEx13.CurrencyRateField = null;
		this.treeListColumnEx13.FieldName = "Remarks";
		this.treeListColumnEx13.Name = "treeListColumnEx13";
		this.treeListColumnEx13.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx13.Visible = true;
		this.treeListColumnEx13.VisibleIndex = 2;
		this.treeListColumnEx12.Caption = "Tax Code";
		this.treeListColumnEx12.CurrencyDateField = null;
		this.treeListColumnEx12.CurrencyField = null;
		this.treeListColumnEx12.CurrencyFixed = null;
		this.treeListColumnEx12.CurrencyFixedRate = 0.0;
		this.treeListColumnEx12.CurrencyRateField = null;
		this.treeListColumnEx12.FieldName = "VatGroup";
		this.treeListColumnEx12.Name = "treeListColumnEx12";
		this.treeListColumnEx12.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx12.Visible = true;
		this.treeListColumnEx12.VisibleIndex = 3;
		this.col_NetAmount.Caption = "Net Amount To Draw";
		this.col_NetAmount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.col_NetAmount.CurrencyDateField = null;
		this.col_NetAmount.CurrencyField = "Currency";
		this.col_NetAmount.CurrencyFixed = null;
		this.col_NetAmount.CurrencyFixedRate = 0.0;
		this.col_NetAmount.CurrencyRateField = null;
		this.col_NetAmount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.col_NetAmount.FieldName = "NetAmount";
		this.col_NetAmount.Format.FormatString = "\"D\"";
		this.col_NetAmount.Name = "col_NetAmount";
		this.col_NetAmount.Visible = true;
		this.col_NetAmount.VisibleIndex = 4;
		this.treeListColumnEx4.Caption = "Tax Amount To Draw";
		this.treeListColumnEx4.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.treeListColumnEx4.CurrencyDateField = null;
		this.treeListColumnEx4.CurrencyField = "Currency";
		this.treeListColumnEx4.CurrencyFixed = null;
		this.treeListColumnEx4.CurrencyFixedRate = 0.0;
		this.treeListColumnEx4.CurrencyRateField = null;
		this.treeListColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.treeListColumnEx4.FieldName = "TaxAmount";
		this.treeListColumnEx4.Name = "treeListColumnEx4";
		this.treeListColumnEx4.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx4.Visible = true;
		this.treeListColumnEx4.VisibleIndex = 5;
		this.col_GrossAmount.Caption = "Gross Amount To Draw";
		this.col_GrossAmount.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.col_GrossAmount.CurrencyDateField = null;
		this.col_GrossAmount.CurrencyField = "Currency";
		this.col_GrossAmount.CurrencyFixed = null;
		this.col_GrossAmount.CurrencyFixedRate = 0.0;
		this.col_GrossAmount.CurrencyRateField = null;
		this.col_GrossAmount.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.col_GrossAmount.FieldName = "GrossAmount";
		this.col_GrossAmount.Format.FormatString = "\"D\"";
		this.col_GrossAmount.Name = "col_GrossAmount";
		this.col_GrossAmount.Visible = true;
		this.col_GrossAmount.VisibleIndex = 6;
		this.col_OpenNet.Caption = "Open Net Amount";
		this.col_OpenNet.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.col_OpenNet.CurrencyDateField = null;
		this.col_OpenNet.CurrencyField = "Currency";
		this.col_OpenNet.CurrencyFixed = null;
		this.col_OpenNet.CurrencyFixedRate = 0.0;
		this.col_OpenNet.CurrencyRateField = null;
		this.col_OpenNet.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.col_OpenNet.FieldName = "OpenNetAmount";
		this.col_OpenNet.Name = "col_OpenNet";
		this.col_OpenNet.OptionsColumn.AllowEdit = false;
		this.col_OpenNet.Visible = true;
		this.col_OpenNet.VisibleIndex = 7;
		this.treeListColumnEx7.Caption = "Open Tax Amount";
		this.treeListColumnEx7.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.treeListColumnEx7.CurrencyDateField = null;
		this.treeListColumnEx7.CurrencyField = "Currency";
		this.treeListColumnEx7.CurrencyFixed = null;
		this.treeListColumnEx7.CurrencyFixedRate = 0.0;
		this.treeListColumnEx7.CurrencyRateField = null;
		this.treeListColumnEx7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.treeListColumnEx7.FieldName = "OpenTaxAmount";
		this.treeListColumnEx7.Name = "treeListColumnEx7";
		this.treeListColumnEx7.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx7.Visible = true;
		this.treeListColumnEx7.VisibleIndex = 8;
		this.col_OpenGross.Caption = "Open Gross Amount";
		this.col_OpenGross.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.col_OpenGross.CurrencyDateField = null;
		this.col_OpenGross.CurrencyField = "Currency";
		this.col_OpenGross.CurrencyFixed = null;
		this.col_OpenGross.CurrencyFixedRate = 0.0;
		this.col_OpenGross.CurrencyRateField = null;
		this.col_OpenGross.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.col_OpenGross.FieldName = "OpenGrossAmount";
		this.col_OpenGross.Name = "col_OpenGross";
		this.col_OpenGross.OptionsColumn.AllowEdit = false;
		this.col_OpenGross.Visible = true;
		this.col_OpenGross.VisibleIndex = 9;
		this.treeListColumnEx9.Caption = "Document Date";
		this.treeListColumnEx9.ColumnEdit = this.repositoryItemDateEditEx1;
		this.treeListColumnEx9.CurrencyDateField = null;
		this.treeListColumnEx9.CurrencyField = null;
		this.treeListColumnEx9.CurrencyFixed = null;
		this.treeListColumnEx9.CurrencyFixedRate = 0.0;
		this.treeListColumnEx9.CurrencyRateField = null;
		this.treeListColumnEx9.FieldName = "DocDate";
		this.treeListColumnEx9.Name = "treeListColumnEx9";
		this.treeListColumnEx9.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx9.Visible = true;
		this.treeListColumnEx9.VisibleIndex = 10;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(13, 353);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(94, 353);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.gcDocCur.Caption = "Currency";
		this.gcDocCur.CurrencyDateField = null;
		this.gcDocCur.CurrencyField = null;
		this.gcDocCur.CurrencyFixed = null;
		this.gcDocCur.CurrencyFixedRate = 0.0;
		this.gcDocCur.CurrencyRateField = null;
		this.gcDocCur.FieldName = "Currency";
		this.gcDocCur.Name = "gcDocCur";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1087, 388);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.treeEx1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_DownPaymentsToDraw.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DownPaymentsToDraw";
		this.Text = "Down Payments To Draw";
		base.Load += new System.EventHandler(this.frm_PaymentToDraw_Load);
		((System.ComponentModel.ISupportInitialize)this.treeEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		base.ResumeLayout(false);
	}
}
