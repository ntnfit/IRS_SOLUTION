using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.UserControls.POS_UI;

public class ucPaymentDialog : XtraUserControl
{
	public Action ForceOk;

	public DataTable DtPayments;

	public double InvTotal;

	public double ChangeDue;

	public string Remarks;

	private readonly List<double> _curDenomin = new List<double> { 1000.0, 2000.0, 5000.0, 10000.0, 20000.0, 50000.0, 100000.0, 200000.0, 500000.0 };

	private readonly List<CheckButton> _listChkButton = new List<CheckButton>();

	private IContainer components;

	private ucNumPad ucNumPad1;

	private CheckButton btn_Cash;

	private GridControlEx grd_Pays;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumn gridColumn3;

	private RetailGrid gv_Pays;

	private CheckButton btn_Credit;

	private CheckButton btn_Check;

	private CheckButton btn_GiftCard;

	private CheckButton btn_Debit;

	private CheckButton btn_OnAccount;

	private SimpleButton btn_Sug6;

	private SimpleButton btn_Sug5;

	private SimpleButton btn_Sug4;

	private SimpleButton btn_Sug3;

	private SimpleButton btn_Sug2;

	private SimpleButton btn_Sug1;

	private LabelControlEx labelControlEx2;

	private SimpleButton btn_10000;

	private SimpleButton btn_20000;

	private SimpleButton btn_50000;

	private SimpleButton btn_100000;

	private SimpleButton btn_200000;

	private LabelControlEx labelControlEx1;

	private SimpleButton btn_500000;

	private CalcEditResizable calc_Amount;

	private LabelControlEx labelControlEx5;

	public CalcEditEx calc_Remain;

	private LabelControlEx labelControlEx6;

	private RepositoryItemLookUpEdit rep_PayType;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemButtonEdit rep_Remove;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private MemoEdit memo_Remarks;

	public string FormId
	{
		get
		{
			return gv_Pays.FormId;
		}
		set
		{
			gv_Pays.FormId = value;
		}
	}

	public string ItemId
	{
		get
		{
			return gv_Pays.ItemId;
		}
		set
		{
			gv_Pays.ItemId = value;
		}
	}

	public ucPaymentDialog()
	{
		InitializeComponent();
		SetEventControl(this);
		Load += OnLoad;
		ucNumPad1.ClientControl = calc_Amount;
	}

	private void LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1",
					Name = "Tiền mặt"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "Thẻ tín dụng"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "Séc"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "Điểm tích lũy"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "Thẻ ghi nợ"
				},
				new Common.Table_Code
				{
					Code = "6",
					Name = "Thẻ quà tặng"
				}
			};
			rep_PayType.DataSource = list;
			rep_PayType.DisplayMember = "Name";
			rep_PayType.ValueMember = "Code";
			rep_PayType.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_PayType.ForceInitialize();
			rep_PayType.PopulateColumns();
			rep_PayType.Columns["Code"].Visible = false;
			rep_PayType.DropDownRows = list.Count;
			DtPayments = new DataTable();
			DtPayments.Columns.Add("PayType", typeof(string));
			DtPayments.Columns.Add("Amount", typeof(double));
			DtPayments.Columns.Add("Note", typeof(string));
			grd_Pays.DataSource = DtPayments;
			grd_Pays.RefreshDataSource();
			_listChkButton.Add(btn_Cash);
			_listChkButton.Add(btn_Credit);
			_listChkButton.Add(btn_Check);
			_listChkButton.Add(btn_GiftCard);
			_listChkButton.Add(btn_OnAccount);
			_listChkButton.Add(btn_Debit);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetEventControl(Control ctr)
	{
		try
		{
			foreach (Control control in ctr.Controls)
			{
				ucNumPad ucNumPad2 = control as ucNumPad;
				SimpleButton simpleButton = control as SimpleButton;
				ButtonEdit buttonEdit = control as ButtonEdit;
				if (control is CheckButton checkButton)
				{
					checkButton.Click += CheckBtnOnClick;
				}
				if (simpleButton != null)
				{
					simpleButton.Click += BtnOnClick;
					simpleButton.TextChanged += BtnOnTextChanged;
				}
				else if (buttonEdit != null && !buttonEdit.ReadOnly && buttonEdit.Properties.TextEditStyle != TextEditStyles.DisableTextEditor)
				{
					buttonEdit.GotFocus += BtnOnGotFocus;
				}
				else if (ucNumPad2 == null)
				{
					SetEventControl(control);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CheckBtnOnClick(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is CheckButton checkButton))
			{
				return;
			}
			foreach (CheckButton item in _listChkButton)
			{
				if (item != checkButton)
				{
					item.Checked = false;
				}
			}
			checkButton.Checked = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SUBMIT()
	{
		try
		{
			DataTable dataTable = grd_Pays.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["PayType"] = "1";
				dataRow["Amount"] = Function.ParseDouble(calc_Amount.EditValue);
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				calc_Remain.EditValue = Function.ParseDouble(calc_Remain.EditValue) - Function.ParseDouble(calc_Amount.EditValue);
				calc_Amount.EditValue = Function.ParseDouble(calc_Remain.EditValue);
			}
			double num = Function.ParseDouble(calc_Remain.EditValue);
			if (num <= 0.0)
			{
				DtPayments = grd_Pays.DataSource as DataTable;
				ChangeDue = Math.Abs(num);
				Remarks = Function.ToString(memo_Remarks.EditValue);
				ForceOk?.Invoke();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnOnGotFocus(object sender, EventArgs eventArgs1)
	{
		try
		{
			ButtonEdit buttonEdit = sender as ButtonEdit;
			ucNumPad1.ClientControl = buttonEdit;
			buttonEdit?.SelectAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnOnTextChanged(object sender, EventArgs eventArgs)
	{
		try
		{
			if (sender is SimpleButton simpleButton)
			{
				if (Math.Abs(Function.ParseDouble(simpleButton.Text)) < double.Epsilon)
				{
					simpleButton.Visible = false;
				}
				else
				{
					simpleButton.Visible = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void BtnOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			if (!(sender is SimpleButton simpleButton))
			{
				return;
			}
			if (!string.IsNullOrEmpty(Function.ToString(simpleButton.Tag)))
			{
				if (grd_Pays.DataSource is DataTable dataTable && Function.ParseDouble(calc_Amount.EditValue) > 0.0 && Function.ParseDouble(calc_Remain.EditValue) > 0.0)
				{
					string type = Function.ToString(simpleButton.Tag);
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["PayType"]) == type);
					if (dataRow != null)
					{
						dataRow["Amount"] = Function.ParseDouble(calc_Amount.EditValue);
					}
					else
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["PayType"] = type;
						dataRow2["Amount"] = Function.ParseDouble(calc_Amount.EditValue);
						dataTable.Rows.Add(dataRow2);
					}
					dataTable.AcceptChanges();
					calc_Remain.EditValue = Function.ParseDouble(calc_Remain.EditValue) - Function.ParseDouble(calc_Amount.EditValue);
					calc_Amount.EditValue = Function.ParseDouble(calc_Remain.EditValue);
				}
			}
			else if (Math.Abs(Function.ParseDouble(simpleButton.Text.Replace(",", ""))) > double.Epsilon)
			{
				calc_Amount.EditValue = Function.ParseDouble(simpleButton.Text.Replace(",", ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void OnLoad(object sender, EventArgs eventArgs)
	{
	}

	private double GetSpecialSug(double val, double par)
	{
		double num = 0.0;
		try
		{
			num = ((!(Math.Ceiling(val / par) < val / par + 1.0)) ? ((val / par + 1.0) * par) : ((Math.Ceiling(val / par) + 1.0) * par));
			if (_curDenomin.Contains(num) || !(val / par > 5.0) || Math.Ceiling(val / par) > val / par)
			{
				num = 0.0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return num;
	}

	private double GetSuggest(double val, double par)
	{
		double num = 0.0;
		try
		{
			if (Math.Ceiling(val / par) < val / par + 1.0)
			{
				num = Math.Ceiling(val / par) * par;
				if (_curDenomin.Contains(num))
				{
					num = 0.0;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return num;
	}

	public void SetValues(double invTotal)
	{
		try
		{
			DataTable dataTable = grd_Pays.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
			}
			calc_Remain.EditValue = invTotal;
			calc_Amount.EditValue = ((invTotal >= 0.0) ? invTotal : 0.0);
			InvTotal = invTotal;
			foreach (CheckButton item in _listChkButton)
			{
				item.Checked = false;
			}
			btn_Cash.Checked = true;
			calc_Amount.Focus();
			memo_Remarks.EditValue = string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetupSuggestions(double amount)
	{
		try
		{
			if (amount <= 0.0)
			{
				amount = 0.0;
			}
			List<double> list = new List<double> { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
			List<double> list2 = _curDenomin.Select((double val) => GetSuggest(amount, val)).ToList().Distinct()
				.ToList();
			list2.Remove(0.0);
			list2.Sort();
			for (int num = 0; num < list2.Count && num <= list.Count - 1; num++)
			{
				list[num] = list2[num];
			}
			btn_Sug1.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[0]);
			btn_Sug2.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[1]);
			btn_Sug3.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[2]);
			btn_Sug4.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[3]);
			btn_Sug5.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[4]);
			btn_Sug6.Text = string.Format(CultureInfo.InvariantCulture, "{0:0,0}", list[5]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ucPaymentDialog_Load(object sender, EventArgs e)
	{
		LoadInitForm();
	}

	private void calc_Remain_EditValueChanged(object sender, EventArgs e)
	{
		SetupSuggestions(Function.ParseDouble(calc_Remain.EditValue));
	}

	private void calc_Amount_EditValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void rep_Remove_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!e.Button.IsLeft)
			{
				return;
			}
			gv_Pays.DeleteDataSourceRowSelected();
			DataTable dataTable = grd_Pays.DataSource as DataTable;
			double num = 0.0;
			if (dataTable.IsNotNull())
			{
				num = dataTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Amount"]));
			}
			calc_Remain.EditValue = InvTotal - num;
			calc_Amount.EditValue = Function.ParseDouble(calc_Remain.EditValue);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.POS_UI.ucPaymentDialog));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.btn_Cash = new DevExpress.XtraEditors.CheckButton();
		this.ucNumPad1 = new Apzon.Forms.UserControls.POS_UI.ucNumPad();
		this.grd_Pays = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Pays = new Apzon.Forms.UserControls.POS_UI.RetailGrid();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.rep_PayType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Remove = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.btn_Credit = new DevExpress.XtraEditors.CheckButton();
		this.btn_Check = new DevExpress.XtraEditors.CheckButton();
		this.btn_GiftCard = new DevExpress.XtraEditors.CheckButton();
		this.btn_Debit = new DevExpress.XtraEditors.CheckButton();
		this.btn_OnAccount = new DevExpress.XtraEditors.CheckButton();
		this.btn_Sug6 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug5 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug4 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug3 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug2 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Sug1 = new DevExpress.XtraEditors.SimpleButton();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_10000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_20000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_50000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_100000 = new DevExpress.XtraEditors.SimpleButton();
		this.btn_200000 = new DevExpress.XtraEditors.SimpleButton();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.btn_500000 = new DevExpress.XtraEditors.SimpleButton();
		this.calc_Amount = new Apzon.Forms.UserControls.POS_UI.CalcEditResizable();
		this.labelControlEx5 = new Apzon.Controls.Controls.LabelControlEx();
		this.calc_Remain = new Apzon.Controls.Editor.CalcEditEx();
		this.labelControlEx6 = new Apzon.Controls.Controls.LabelControlEx();
		this.memo_Remarks = new DevExpress.XtraEditors.MemoEdit();
		((System.ComponentModel.ISupportInitialize)this.grd_Pays).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Pays).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PayType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remove).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Amount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Remain.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Cash.Appearance.BackColor = System.Drawing.Color.FromArgb(195, 214, 155);
		this.btn_Cash.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Cash.Appearance.Options.UseBackColor = true;
		this.btn_Cash.Appearance.Options.UseFont = true;
		this.btn_Cash.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
		this.btn_Cash.AppearancePressed.Options.UseBackColor = true;
		this.btn_Cash.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cash.ImageOptions.Image");
		this.btn_Cash.Location = new System.Drawing.Point(474, 5);
		this.btn_Cash.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_Cash.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Cash.Name = "btn_Cash";
		this.btn_Cash.Size = new System.Drawing.Size(118, 40);
		this.btn_Cash.TabIndex = 1;
		this.btn_Cash.Tag = "1";
		this.btn_Cash.Text = "Tiền mặt";
		this.ucNumPad1.ClientControl = null;
		this.ucNumPad1.Location = new System.Drawing.Point(613, 66);
		this.ucNumPad1.Name = "ucNumPad1";
		this.ucNumPad1.Size = new System.Drawing.Size(267, 281);
		this.ucNumPad1.TabIndex = 20;
		this.grd_Pays.AllowPasteExcel = false;
		this.grd_Pays.ClipboardData = null;
		this.grd_Pays.Location = new System.Drawing.Point(8, 95);
		this.grd_Pays.MainView = this.gv_Pays;
		this.grd_Pays.Name = "grd_Pays";
		this.grd_Pays.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_PayType, this.rep_Remove, this.repositoryItemMemoExEdit1 });
		this.grd_Pays.Size = new System.Drawing.Size(460, 252);
		this.grd_Pays.TabIndex = 21;
		this.grd_Pays.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Pays });
		this.gv_Pays.AutoFillColumn = this.gridColumn2;
		this.gv_Pays.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Pays.ColumnKey = null;
		this.gv_Pays.ColumnPanelRowHeight = 25;
		this.gv_Pays.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumnEx1 });
		this.gv_Pays.DisableFormSetting = false;
		this.gv_Pays.DisplayException = false;
		this.gv_Pays.EditColumn = null;
		this.gv_Pays.FieldCurrency = null;
		this.gv_Pays.FieldRate = null;
		this.gv_Pays.FormId = "";
		this.gv_Pays.GridControl = this.grd_Pays;
		this.gv_Pays.IndexEditRow = -1;
		this.gv_Pays.IndicatorWidth = 30;
		this.gv_Pays.IsApzTable = false;
		this.gv_Pays.IsInsertNewRow = false;
		this.gv_Pays.IsLoadingData = false;
		this.gv_Pays.ItemId = "ODATA";
		this.gv_Pays.LstColumnAllowBatch = null;
		this.gv_Pays.LstColumnNotAllowBatch = null;
		this.gv_Pays.LstColumNotSetting = null;
		this.gv_Pays.Name = "gv_Pays";
		this.gv_Pays.RowHeight = 60;
		this.gv_Pays.UseBatchMod = false;
		this.gridColumn2.Caption = "Số tiền";
		this.gridColumn2.DisplayFormat.FormatString = "n0";
		this.gridColumn2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn2.FieldName = "Amount";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 125;
		this.gridColumn1.Caption = "Loại";
		this.gridColumn1.ColumnEdit = this.rep_PayType;
		this.gridColumn1.FieldName = "PayType";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 97;
		this.rep_PayType.AutoHeight = false;
		this.rep_PayType.Name = "rep_PayType";
		this.rep_PayType.NullText = "";
		this.rep_PayType.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.gridColumn3.Caption = "Notes";
		this.gridColumn3.FieldName = "Note";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 134;
		this.gridColumnEx1.ColumnEdit = this.rep_Remove;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 3;
		this.gridColumnEx1.Width = 67;
		this.rep_Remove.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_Remove.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions)
		});
		this.rep_Remove.Name = "rep_Remove";
		this.rep_Remove.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.rep_Remove.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Remove_ButtonClick);
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.btn_Credit.Appearance.BackColor = System.Drawing.Color.FromArgb(195, 214, 155);
		this.btn_Credit.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Credit.Appearance.Options.UseBackColor = true;
		this.btn_Credit.Appearance.Options.UseFont = true;
		this.btn_Credit.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
		this.btn_Credit.AppearancePressed.Options.UseBackColor = true;
		this.btn_Credit.Location = new System.Drawing.Point(474, 59);
		this.btn_Credit.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_Credit.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Credit.Name = "btn_Credit";
		this.btn_Credit.Size = new System.Drawing.Size(118, 40);
		this.btn_Credit.TabIndex = 22;
		this.btn_Credit.Tag = "2";
		this.btn_Credit.Text = "Thẻ tín dụng";
		this.btn_Check.Appearance.BackColor = System.Drawing.Color.FromArgb(195, 214, 155);
		this.btn_Check.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Check.Appearance.Options.UseBackColor = true;
		this.btn_Check.Appearance.Options.UseFont = true;
		this.btn_Check.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
		this.btn_Check.AppearancePressed.Options.UseBackColor = true;
		this.btn_Check.Location = new System.Drawing.Point(474, 112);
		this.btn_Check.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_Check.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Check.Name = "btn_Check";
		this.btn_Check.Size = new System.Drawing.Size(118, 40);
		this.btn_Check.TabIndex = 23;
		this.btn_Check.Tag = "3";
		this.btn_Check.Text = "Séc";
		this.btn_GiftCard.Appearance.BackColor = System.Drawing.Color.FromArgb(195, 214, 155);
		this.btn_GiftCard.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_GiftCard.Appearance.Options.UseBackColor = true;
		this.btn_GiftCard.Appearance.Options.UseFont = true;
		this.btn_GiftCard.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
		this.btn_GiftCard.AppearancePressed.Options.UseBackColor = true;
		this.btn_GiftCard.Location = new System.Drawing.Point(474, 163);
		this.btn_GiftCard.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_GiftCard.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_GiftCard.Name = "btn_GiftCard";
		this.btn_GiftCard.Size = new System.Drawing.Size(118, 40);
		this.btn_GiftCard.TabIndex = 24;
		this.btn_GiftCard.Tag = "6";
		this.btn_GiftCard.Text = "Thẻ quà tặng";
		this.btn_Debit.Appearance.BackColor = System.Drawing.Color.FromArgb(195, 214, 155);
		this.btn_Debit.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_Debit.Appearance.Options.UseBackColor = true;
		this.btn_Debit.Appearance.Options.UseFont = true;
		this.btn_Debit.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
		this.btn_Debit.AppearancePressed.Options.UseBackColor = true;
		this.btn_Debit.Location = new System.Drawing.Point(474, 310);
		this.btn_Debit.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_Debit.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Debit.Name = "btn_Debit";
		this.btn_Debit.Size = new System.Drawing.Size(118, 36);
		this.btn_Debit.TabIndex = 25;
		this.btn_Debit.Tag = "5";
		this.btn_Debit.Text = "Thẻ ghi nợ";
		this.btn_OnAccount.Appearance.BackColor = System.Drawing.Color.FromArgb(195, 214, 155);
		this.btn_OnAccount.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.btn_OnAccount.Appearance.Options.UseBackColor = true;
		this.btn_OnAccount.Appearance.Options.UseFont = true;
		this.btn_OnAccount.AppearancePressed.BackColor = System.Drawing.Color.FromArgb(192, 192, 0);
		this.btn_OnAccount.AppearancePressed.Options.UseBackColor = true;
		this.btn_OnAccount.Location = new System.Drawing.Point(474, 220);
		this.btn_OnAccount.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Flat;
		this.btn_OnAccount.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_OnAccount.Name = "btn_OnAccount";
		this.btn_OnAccount.Size = new System.Drawing.Size(118, 40);
		this.btn_OnAccount.TabIndex = 26;
		this.btn_OnAccount.Tag = "4";
		this.btn_OnAccount.Text = "Điểm tích lũy";
		this.btn_Sug6.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug6.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug6.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug6.Appearance.Options.UseBackColor = true;
		this.btn_Sug6.Appearance.Options.UseFont = true;
		this.btn_Sug6.Appearance.Options.UseForeColor = true;
		this.btn_Sug6.Location = new System.Drawing.Point(749, 437);
		this.btn_Sug6.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug6.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug6.Name = "btn_Sug6";
		this.btn_Sug6.Size = new System.Drawing.Size(135, 45);
		this.btn_Sug6.TabIndex = 43;
		this.btn_Sug6.Visible = false;
		this.btn_Sug5.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug5.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug5.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug5.Appearance.Options.UseBackColor = true;
		this.btn_Sug5.Appearance.Options.UseFont = true;
		this.btn_Sug5.Appearance.Options.UseForeColor = true;
		this.btn_Sug5.Location = new System.Drawing.Point(607, 437);
		this.btn_Sug5.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug5.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug5.Name = "btn_Sug5";
		this.btn_Sug5.Size = new System.Drawing.Size(135, 45);
		this.btn_Sug5.TabIndex = 42;
		this.btn_Sug5.Visible = false;
		this.btn_Sug4.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug4.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug4.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug4.Appearance.Options.UseBackColor = true;
		this.btn_Sug4.Appearance.Options.UseFont = true;
		this.btn_Sug4.Appearance.Options.UseForeColor = true;
		this.btn_Sug4.Location = new System.Drawing.Point(464, 437);
		this.btn_Sug4.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug4.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug4.Name = "btn_Sug4";
		this.btn_Sug4.Size = new System.Drawing.Size(135, 45);
		this.btn_Sug4.TabIndex = 41;
		this.btn_Sug4.Visible = false;
		this.btn_Sug3.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug3.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug3.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug3.Appearance.Options.UseBackColor = true;
		this.btn_Sug3.Appearance.Options.UseFont = true;
		this.btn_Sug3.Appearance.Options.UseForeColor = true;
		this.btn_Sug3.Location = new System.Drawing.Point(748, 385);
		this.btn_Sug3.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug3.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug3.Name = "btn_Sug3";
		this.btn_Sug3.Size = new System.Drawing.Size(135, 45);
		this.btn_Sug3.TabIndex = 40;
		this.btn_Sug3.Visible = false;
		this.btn_Sug2.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug2.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug2.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug2.Appearance.Options.UseBackColor = true;
		this.btn_Sug2.Appearance.Options.UseFont = true;
		this.btn_Sug2.Appearance.Options.UseForeColor = true;
		this.btn_Sug2.Location = new System.Drawing.Point(607, 385);
		this.btn_Sug2.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug2.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug2.Name = "btn_Sug2";
		this.btn_Sug2.Size = new System.Drawing.Size(135, 45);
		this.btn_Sug2.TabIndex = 39;
		this.btn_Sug2.Visible = false;
		this.btn_Sug1.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 192, 128);
		this.btn_Sug1.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_Sug1.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_Sug1.Appearance.Options.UseBackColor = true;
		this.btn_Sug1.Appearance.Options.UseFont = true;
		this.btn_Sug1.Appearance.Options.UseForeColor = true;
		this.btn_Sug1.Location = new System.Drawing.Point(465, 385);
		this.btn_Sug1.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_Sug1.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_Sug1.Name = "btn_Sug1";
		this.btn_Sug1.Size = new System.Drawing.Size(135, 45);
		this.btn_Sug1.TabIndex = 38;
		this.btn_Sug1.Visible = false;
		this.labelControlEx2.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx2.Appearance.Options.UseFont = true;
		this.labelControlEx2.FieldName = "labelControlEx1";
		this.labelControlEx2.Location = new System.Drawing.Point(465, 363);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.Size = new System.Drawing.Size(89, 16);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 37;
		this.labelControlEx2.Text = "Gợi ý tiền mặt";
		this.btn_10000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_10000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_10000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_10000.Appearance.Options.UseBackColor = true;
		this.btn_10000.Appearance.Options.UseFont = true;
		this.btn_10000.Appearance.Options.UseForeColor = true;
		this.btn_10000.Location = new System.Drawing.Point(368, 437);
		this.btn_10000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_10000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_10000.Name = "btn_10000";
		this.btn_10000.Size = new System.Drawing.Size(90, 45);
		this.btn_10000.TabIndex = 36;
		this.btn_10000.Text = "10,000";
		this.btn_20000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_20000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_20000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_20000.Appearance.Options.UseBackColor = true;
		this.btn_20000.Appearance.Options.UseFont = true;
		this.btn_20000.Appearance.Options.UseForeColor = true;
		this.btn_20000.Location = new System.Drawing.Point(274, 437);
		this.btn_20000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_20000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_20000.Name = "btn_20000";
		this.btn_20000.Size = new System.Drawing.Size(90, 45);
		this.btn_20000.TabIndex = 35;
		this.btn_20000.Text = "20,000";
		this.btn_50000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_50000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_50000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_50000.Appearance.Options.UseBackColor = true;
		this.btn_50000.Appearance.Options.UseFont = true;
		this.btn_50000.Appearance.Options.UseForeColor = true;
		this.btn_50000.Location = new System.Drawing.Point(180, 437);
		this.btn_50000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_50000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_50000.Name = "btn_50000";
		this.btn_50000.Size = new System.Drawing.Size(90, 45);
		this.btn_50000.TabIndex = 34;
		this.btn_50000.Text = "50,000";
		this.btn_100000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_100000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_100000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_100000.Appearance.Options.UseBackColor = true;
		this.btn_100000.Appearance.Options.UseFont = true;
		this.btn_100000.Appearance.Options.UseForeColor = true;
		this.btn_100000.Location = new System.Drawing.Point(368, 385);
		this.btn_100000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_100000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_100000.Name = "btn_100000";
		this.btn_100000.Size = new System.Drawing.Size(90, 45);
		this.btn_100000.TabIndex = 33;
		this.btn_100000.Text = "100,000";
		this.btn_200000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_200000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_200000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_200000.Appearance.Options.UseBackColor = true;
		this.btn_200000.Appearance.Options.UseFont = true;
		this.btn_200000.Appearance.Options.UseForeColor = true;
		this.btn_200000.Location = new System.Drawing.Point(274, 385);
		this.btn_200000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_200000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_200000.Name = "btn_200000";
		this.btn_200000.Size = new System.Drawing.Size(90, 45);
		this.btn_200000.TabIndex = 32;
		this.btn_200000.Text = "200,000";
		this.labelControlEx1.Appearance.Font = new System.Drawing.Font("Tahoma", 9.75f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx1.Appearance.Options.UseFont = true;
		this.labelControlEx1.FieldName = "labelControlEx1";
		this.labelControlEx1.Location = new System.Drawing.Point(183, 363);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(93, 16);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 31;
		this.labelControlEx1.Text = "Theo mệnh giá";
		this.btn_500000.Appearance.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.btn_500000.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Pixel);
		this.btn_500000.Appearance.ForeColor = System.Drawing.SystemColors.WindowText;
		this.btn_500000.Appearance.Options.UseBackColor = true;
		this.btn_500000.Appearance.Options.UseFont = true;
		this.btn_500000.Appearance.Options.UseForeColor = true;
		this.btn_500000.Location = new System.Drawing.Point(180, 385);
		this.btn_500000.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.UltraFlat;
		this.btn_500000.LookAndFeel.UseDefaultLookAndFeel = false;
		this.btn_500000.Name = "btn_500000";
		this.btn_500000.Size = new System.Drawing.Size(90, 45);
		this.btn_500000.TabIndex = 30;
		this.btn_500000.Text = "500,000";
		this.calc_Amount.FieldName = null;
		this.calc_Amount.IsSearchControl = false;
		this.calc_Amount.Location = new System.Drawing.Point(613, 28);
		this.calc_Amount.Name = "calc_Amount";
		this.calc_Amount.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 18.0176f);
		this.calc_Amount.Properties.Appearance.Options.UseFont = true;
		this.calc_Amount.Properties.AutoHeight = false;
		this.calc_Amount.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_Amount.Properties.IsSearchControl = false;
		this.calc_Amount.Properties.NullValuePrompt = "0";
		this.calc_Amount.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_Amount.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_Amount.Size = new System.Drawing.Size(265, 32);
		this.calc_Amount.TabIndex = 0;
		this.calc_Amount.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.calc_Amount_EditValueChanging);
		this.labelControlEx5.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx5.Appearance.Options.UseFont = true;
		this.labelControlEx5.Appearance.Options.UseTextOptions = true;
		this.labelControlEx5.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.labelControlEx5.FieldName = "labelControlEx1";
		this.labelControlEx5.Location = new System.Drawing.Point(613, 4);
		this.labelControlEx5.Name = "labelControlEx5";
		this.labelControlEx5.Size = new System.Drawing.Size(265, 18);
		this.labelControlEx5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx5.TabIndex = 49;
		this.labelControlEx5.Text = "Điền số tiền và chọn loại thanh toán";
		this.calc_Remain.FieldName = null;
		this.calc_Remain.IsSearchControl = false;
		this.calc_Remain.Location = new System.Drawing.Point(8, 27);
		this.calc_Remain.Name = "calc_Remain";
		this.calc_Remain.Properties.Appearance.BackColor = System.Drawing.Color.FromArgb(17, 45, 53);
		this.calc_Remain.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 20.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.calc_Remain.Properties.Appearance.ForeColor = System.Drawing.Color.FromArgb(1, 255, 0);
		this.calc_Remain.Properties.Appearance.Options.UseBackColor = true;
		this.calc_Remain.Properties.Appearance.Options.UseFont = true;
		this.calc_Remain.Properties.Appearance.Options.UseForeColor = true;
		this.calc_Remain.Properties.AutoHeight = false;
		this.calc_Remain.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_Remain.Properties.ButtonsStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.calc_Remain.Properties.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.calc_Remain.Properties.IsSearchControl = false;
		this.calc_Remain.Properties.NullValuePrompt = "0";
		this.calc_Remain.Properties.NullValuePromptShowForEmptyValue = true;
		this.calc_Remain.Properties.ShowDropDown = DevExpress.XtraEditors.Controls.ShowDropDown.Never;
		this.calc_Remain.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.calc_Remain.Size = new System.Drawing.Size(460, 62);
		this.calc_Remain.TabIndex = 50;
		this.calc_Remain.EditValueChanged += new System.EventHandler(this.calc_Remain_EditValueChanged);
		this.labelControlEx6.Appearance.Font = new System.Drawing.Font("Tahoma", 11.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControlEx6.Appearance.Options.UseFont = true;
		this.labelControlEx6.FieldName = "labelControlEx1";
		this.labelControlEx6.Location = new System.Drawing.Point(324, 5);
		this.labelControlEx6.Name = "labelControlEx6";
		this.labelControlEx6.Size = new System.Drawing.Size(144, 18);
		this.labelControlEx6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx6.TabIndex = 51;
		this.labelControlEx6.Text = "Số tiền còn phải trả";
		this.memo_Remarks.EditValue = "";
		this.memo_Remarks.Location = new System.Drawing.Point(8, 353);
		this.memo_Remarks.Name = "memo_Remarks";
		this.memo_Remarks.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 15f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.memo_Remarks.Properties.Appearance.Options.UseFont = true;
		this.memo_Remarks.Properties.NullValuePrompt = "Ghi chú";
		this.memo_Remarks.Properties.NullValuePromptShowForEmptyValue = true;
		this.memo_Remarks.Size = new System.Drawing.Size(168, 134);
		this.memo_Remarks.TabIndex = 53;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.memo_Remarks);
		base.Controls.Add(this.labelControlEx6);
		base.Controls.Add(this.calc_Remain);
		base.Controls.Add(this.calc_Amount);
		base.Controls.Add(this.labelControlEx5);
		base.Controls.Add(this.btn_Sug6);
		base.Controls.Add(this.btn_Sug5);
		base.Controls.Add(this.btn_Sug4);
		base.Controls.Add(this.btn_Sug3);
		base.Controls.Add(this.btn_Sug2);
		base.Controls.Add(this.btn_Sug1);
		base.Controls.Add(this.labelControlEx2);
		base.Controls.Add(this.btn_10000);
		base.Controls.Add(this.btn_20000);
		base.Controls.Add(this.btn_50000);
		base.Controls.Add(this.btn_100000);
		base.Controls.Add(this.btn_200000);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.btn_500000);
		base.Controls.Add(this.btn_OnAccount);
		base.Controls.Add(this.btn_Debit);
		base.Controls.Add(this.btn_GiftCard);
		base.Controls.Add(this.btn_Check);
		base.Controls.Add(this.btn_Credit);
		base.Controls.Add(this.grd_Pays);
		base.Controls.Add(this.ucNumPad1);
		base.Controls.Add(this.btn_Cash);
		base.Name = "ucPaymentDialog";
		base.Size = new System.Drawing.Size(886, 489);
		base.Load += new System.EventHandler(this.ucPaymentDialog_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Pays).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Pays).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_PayType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Remove).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Amount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Remain.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
