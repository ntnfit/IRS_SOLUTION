using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.SalesOppotunity;

public class frm_ActivityListForSalesOpportunity : XtraFormEx
{
	public DataTable dtActList;

	public int OprLine = -1;

	public int MaxOprLine = -1;

	private string _cardCode = "";

	private IContainer components;

	private GridControlEx grdDoc;

	private GridColumnEx gcCntctDate;

	private GridColumnEx gcCntctTime;

	private GridColumnEx gcHandedBy;

	private GridColumnEx gcActDate;

	private GridColumnEx gcRecurrence;

	private GridColumnEx gcNotes;

	private GridColumnEx gcClgCode;

	private RepositoryItemButtonEdit rep_Date;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private SimpleButtonEx btOK;

	private GridViewEx gvDoc;

	private SimpleButtonEx btActivities;

	private RepositoryItemLookUpEdit rep_HandledBy;

	private RepositoryItemLookUpEdit rep_RecurPat;

	private RepositoryItemDateEdit rep_ActivityDate;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_ActivityListForSalesOpportunity(DataTable dtSource, string CardCode)
	{
		InitializeComponent();
		SettingLanguage();
		dtActList = dtSource;
		_cardCode = CardCode;
	}

	private void SettingLanguage()
	{
		Text = Apzon.Commons.Properties.Resources.Titel_frmActivityListForSalesOpportunity;
		btOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
		btActivities.Text = Apzon.Commons.Properties.Resources.ButtonActivities;
	}

	private void frm_ActivityListForSalesOpportunity_Load(object sender, EventArgs e)
	{
		try
		{
			grdDoc.DataSource = SetDateColumnFormat(dtActList);
			grdDoc.RefreshDataSource();
			LoadComboBox();
			SetFormatControlValue();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable SetDateColumnFormat(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string text = Function.ToString(dt.Rows[i]["ActDate"]);
					if (!(text.ToUpper() != "CLOSED"))
					{
						continue;
					}
					string[] array = text.Split('/');
					if (array.Length == 3)
					{
						if (Function.ParseInt(array[0]) <= 1900)
						{
							dt.Rows[i]["ActDate"] = "";
							continue;
						}
						string dateTimeFormat = GlobalData.DateTimeFormat;
						string value = new DateTime(Function.ParseInt(array[2]), Function.ParseInt(array[1]), Function.ParseInt(array[0])).ToString(dateTimeFormat);
						dt.Rows[i]["ActDate"] = value;
					}
				}
			}
			dt.AcceptChanges();
			return dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void LoadComboBox()
	{
		try
		{
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Opportunity"))
			{
				DataTable dataSource = httpClientHelper.Get("GetUserForListActivity");
				rep_HandledBy.DataSource = dataSource;
			}
			rep_RecurPat.SetDataSource(new string[5, 2]
			{
				{ "N", "None" },
				{ "D", "Daily" },
				{ "W", "Weekly" },
				{ "M", "Monthly" },
				{ "A", "Annually" }
			}, isStringCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btOK_Click(object sender, EventArgs e)
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

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ActivitiesForOpportunity frmAct = new frm_ActivitiesForOpportunity();
			frmAct.MdiParent = MdiParent;
			frmAct.isLoadFromOpportunity = true;
			frmAct._cardCodefrm_SalesOpportunity = _cardCode;
			frmAct.FormClosing += (object s, FormClosingEventArgs events) =>
			{
				if (frmAct.dtCopy.Rows.Count > 0)
				{
					DataRow dataRow = frmAct.dtCopy.Rows[0];
					dtActList.PrimaryKey = new DataColumn[1] { dtActList.Columns["ClgCode"] };
					if (dtActList.Rows.Find(frmAct.dtCopy.Rows[0]["ClgCode"]) == null)
					{
						DataRow dataRow2 = dtActList.NewRow();
						dataRow2["clgcode"] = Function.ToString(dataRow["clgcode"]);
						dataRow2["CntctDate"] = Function.ToString(dataRow["Recontact"]);
						dataRow2["CntctTime"] = FetchStringTime(Function.ToString(dataRow["CntctTime"]));
						dataRow2["HandledBy"] = Function.ToString(dataRow["AttendUser"]);
						if (Function.ToString(dataRow["Closed"]) == "Y")
						{
							dataRow2["ActDate"] = "Closed";
						}
						else
						{
							dataRow2["ActDate"] = Function.ToString(dataRow["Recontact"]);
						}
						dataRow2["RecurPat"] = Function.ToString(dataRow["RecurPat"]);
						dataRow2["Notes"] = Function.ToString(dataRow["Notes"]);
						if (OprLine == -1)
						{
							dataRow2["OprLine"] = MaxOprLine;
						}
						else
						{
							dataRow2["OprLine"] = OprLine;
						}
						dtActList.Rows.Add(dataRow2);
						dtActList.AcceptChanges();
						FormMode = Enums.FormViewModeType.Update;
					}
				}
			};
			frmAct.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void repositoryItemButtonEdit1_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ActivitiesForOpportunity frmAct = new frm_ActivitiesForOpportunity();
			frmAct._clgCode_FromOppotunity = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ClgCode"));
			frmAct.isLoadFromOpportunity = true;
			frmAct.MdiParent = MdiParent;
			frmAct.FormClosing += (object s, FormClosingEventArgs events) =>
			{
				if (frmAct.dtCopy.Rows.Count > 0)
				{
					DataRow dataRow = frmAct.dtCopy.Rows[0];
					dtActList.PrimaryKey = new DataColumn[1] { dtActList.Columns["ClgCode"] };
					DataRow dataRow2 = dtActList.Rows.Find(frmAct.dtCopy.Rows[0]["ClgCode"]);
					if (dataRow2 != null)
					{
						dataRow2["ClgCode"] = Function.ToString(dataRow["ClgCode"]);
						dataRow2["CntctDate"] = Function.ToString(dataRow["Recontact"]);
						dataRow2["CntctTime"] = FetchStringTime(Function.ToString(dataRow["CntctTime"]));
						dataRow2["HandledBy"] = Function.ToString(dataRow["AttendUser"]);
						if (Function.ToString(dataRow["Closed"]) == "Y")
						{
							dataRow2["ActDate"] = "Closed";
						}
						else
						{
							dataRow2["ActDate"] = Function.ToString(dataRow["Recontact"]);
						}
						dataRow2["RecurPat"] = Function.ToString(dataRow["RecurPat"]);
						dataRow2["Notes"] = Function.ToString(dataRow["Notes"]);
						dtActList.AcceptChanges();
					}
				}
			};
			frmAct.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataRow row = dtActList.Rows[gvDoc.GetDataSourceRowIndex(gvDoc.FocusedRowHandle)];
			dtActList.Rows.Remove(row);
			dtActList.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControlValue()
	{
		try
		{
			string dateTimeFormat = GlobalData.DateTimeFormat;
			rep_Date.Mask.MaskType = MaskType.DateTime;
			rep_Date.Mask.EditMask = dateTimeFormat;
			rep_Date.Mask.UseMaskAsDisplayFormat = true;
			rep_ActivityDate.Mask.MaskType = MaskType.DateTime;
			rep_ActivityDate.Mask.EditMask = dateTimeFormat;
			rep_ActivityDate.Mask.UseMaskAsDisplayFormat = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string FetchStringTime(string time)
	{
		string result = string.Empty;
		try
		{
			if (time.Length == 3)
			{
				result = time.Substring(0, 1) + ":" + time.Substring(1, 2);
			}
			if (time.Length == 4)
			{
				result = time.Substring(0, 2) + ":" + time.Substring(2, 2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.frm_ActivityListForSalesOpportunity));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcCntctDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcCntctTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcHandedBy = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_HandledBy = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcActDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ActivityDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcRecurrence = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_RecurPat = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcNotes = new Apzon.Controls.Grid.GridColumnEx();
		this.gcClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.btActivities = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_HandledBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActivityDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActivityDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RecurPat).BeginInit();
		base.SuspendLayout();
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(3, 283);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 0;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(3, 1);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Date, this.rep_HandledBy, this.rep_RecurPat, this.rep_ActivityDate });
		this.grdDoc.Size = new System.Drawing.Size(529, 276);
		this.grdDoc.TabIndex = 2;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gcCntctDate, this.gcCntctTime, this.gcHandedBy, this.gcActDate, this.gcRecurrence, this.gcNotes, this.gcClgCode });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oActivityForSalesOpportunity";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 20;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ODATA";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gcCntctDate.Caption = "Date";
		this.gcCntctDate.ColumnEdit = this.rep_Date;
		this.gcCntctDate.CurrencyDateField = null;
		this.gcCntctDate.CurrencyField = null;
		this.gcCntctDate.CurrencyFixed = null;
		this.gcCntctDate.CurrencyFixedRate = 0.0;
		this.gcCntctDate.CurrencyRateField = null;
		this.gcCntctDate.FieldName = "CntctDate";
		this.gcCntctDate.Name = "gcCntctDate";
		this.gcCntctDate.OptionsColumn.ReadOnly = true;
		this.gcCntctDate.PrimaryKey = true;
		this.gcCntctDate.UnitField = null;
		this.gcCntctDate.Visible = true;
		this.gcCntctDate.VisibleIndex = 0;
		this.gcCntctDate.Width = 95;
		this.rep_Date.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Date.Name = "rep_Date";
		this.rep_Date.ReadOnly = true;
		this.rep_Date.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repositoryItemButtonEdit1_ButtonPressed);
		this.gcCntctTime.Caption = "Time";
		this.gcCntctTime.CurrencyDateField = null;
		this.gcCntctTime.CurrencyField = null;
		this.gcCntctTime.CurrencyFixed = null;
		this.gcCntctTime.CurrencyFixedRate = 0.0;
		this.gcCntctTime.CurrencyRateField = null;
		this.gcCntctTime.FieldName = "CntctTime";
		this.gcCntctTime.Name = "gcCntctTime";
		this.gcCntctTime.OptionsColumn.ReadOnly = true;
		this.gcCntctTime.PrimaryKey = false;
		this.gcCntctTime.UnitField = null;
		this.gcCntctTime.Visible = true;
		this.gcCntctTime.VisibleIndex = 1;
		this.gcCntctTime.Width = 52;
		this.gcHandedBy.Caption = "Handled by";
		this.gcHandedBy.ColumnEdit = this.rep_HandledBy;
		this.gcHandedBy.CurrencyDateField = null;
		this.gcHandedBy.CurrencyField = null;
		this.gcHandedBy.CurrencyFixed = null;
		this.gcHandedBy.CurrencyFixedRate = 0.0;
		this.gcHandedBy.CurrencyRateField = null;
		this.gcHandedBy.FieldName = "HandledBy";
		this.gcHandedBy.Name = "gcHandedBy";
		this.gcHandedBy.OptionsColumn.ReadOnly = true;
		this.gcHandedBy.PrimaryKey = false;
		this.gcHandedBy.UnitField = null;
		this.gcHandedBy.Visible = true;
		this.gcHandedBy.VisibleIndex = 2;
		this.rep_HandledBy.AutoHeight = false;
		this.rep_HandledBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_HandledBy.DisplayMember = "Name";
		this.rep_HandledBy.Name = "rep_HandledBy";
		this.rep_HandledBy.NullText = "";
		this.rep_HandledBy.ValueMember = "Code";
		this.gcActDate.Caption = "Activity Date";
		this.gcActDate.ColumnEdit = this.rep_ActivityDate;
		this.gcActDate.CurrencyDateField = null;
		this.gcActDate.CurrencyField = null;
		this.gcActDate.CurrencyFixed = null;
		this.gcActDate.CurrencyFixedRate = 0.0;
		this.gcActDate.CurrencyRateField = null;
		this.gcActDate.FieldName = "ActDate";
		this.gcActDate.Name = "gcActDate";
		this.gcActDate.OptionsColumn.ReadOnly = true;
		this.gcActDate.PrimaryKey = false;
		this.gcActDate.UnitField = null;
		this.gcActDate.Visible = true;
		this.gcActDate.VisibleIndex = 3;
		this.gcActDate.Width = 92;
		this.rep_ActivityDate.AutoHeight = false;
		this.rep_ActivityDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ActivityDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ActivityDate.Name = "rep_ActivityDate";
		this.gcRecurrence.Caption = "Recurrence";
		this.gcRecurrence.ColumnEdit = this.rep_RecurPat;
		this.gcRecurrence.CurrencyDateField = null;
		this.gcRecurrence.CurrencyField = null;
		this.gcRecurrence.CurrencyFixed = null;
		this.gcRecurrence.CurrencyFixedRate = 0.0;
		this.gcRecurrence.CurrencyRateField = null;
		this.gcRecurrence.FieldName = "RecurPat";
		this.gcRecurrence.Name = "gcRecurrence";
		this.gcRecurrence.OptionsColumn.ReadOnly = true;
		this.gcRecurrence.PrimaryKey = false;
		this.gcRecurrence.UnitField = null;
		this.gcRecurrence.Visible = true;
		this.gcRecurrence.VisibleIndex = 4;
		this.gcRecurrence.Width = 76;
		this.rep_RecurPat.AutoHeight = false;
		this.rep_RecurPat.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_RecurPat.Name = "rep_RecurPat";
		this.rep_RecurPat.NullText = "";
		this.gcNotes.Caption = "Content";
		this.gcNotes.CurrencyDateField = null;
		this.gcNotes.CurrencyField = null;
		this.gcNotes.CurrencyFixed = null;
		this.gcNotes.CurrencyFixedRate = 0.0;
		this.gcNotes.CurrencyRateField = null;
		this.gcNotes.FieldName = "Notes";
		this.gcNotes.Name = "gcNotes";
		this.gcNotes.OptionsColumn.ReadOnly = true;
		this.gcNotes.PrimaryKey = false;
		this.gcNotes.UnitField = null;
		this.gcNotes.Visible = true;
		this.gcNotes.VisibleIndex = 5;
		this.gcNotes.Width = 77;
		this.gcClgCode.Caption = "ClgCode";
		this.gcClgCode.CurrencyDateField = null;
		this.gcClgCode.CurrencyField = null;
		this.gcClgCode.CurrencyFixed = null;
		this.gcClgCode.CurrencyFixedRate = 0.0;
		this.gcClgCode.CurrencyRateField = null;
		this.gcClgCode.FieldName = "ClgCode";
		this.gcClgCode.Name = "gcClgCode";
		this.gcClgCode.OptionsColumn.ReadOnly = true;
		this.gcClgCode.PrimaryKey = false;
		this.gcClgCode.UnitField = null;
		this.btActivities.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btActivities.EditControl = null;
		this.btActivities.EditValue = "Activities";
		this.btActivities.FieldName = null;
		this.btActivities.IsSearchControl = false;
		this.btActivities.Location = new System.Drawing.Point(450, 283);
		this.btActivities.Name = "btActivities";
		this.btActivities.ReadOnly = false;
		this.btActivities.SettingInVisible = false;
		this.btActivities.SettingReadonly = false;
		this.btActivities.Size = new System.Drawing.Size(82, 23);
		this.btActivities.TabIndex = 3;
		this.btActivities.Text = "Activities";
		this.btActivities.Click += new System.EventHandler(this.simpleButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(544, 309);
		base.Controls.Add(this.btActivities);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btOK);
		base.FormId = "oActivityForSalesOpportunity";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ActivityListForSalesOpportunity";
		this.Text = "Activities for Sales Opportunity";
		base.Load += new System.EventHandler(this.frm_ActivityListForSalesOpportunity_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_HandledBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActivityDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ActivityDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_RecurPat).EndInit();
		base.ResumeLayout(false);
	}
}
