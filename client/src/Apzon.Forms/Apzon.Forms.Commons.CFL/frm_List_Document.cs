using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Apzon.Entities.Models.Documents;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;

namespace Apzon.Forms.Commons.CFL;

public class frm_List_Document : XtraFormEx
{
	public DocumentsImpl ChooseItem;

	public List<DocumentsImpl> ChooseItems;

	public bool Doc_MutiSelectMode;

	public ApzObjectTypes type;

	public string strCardCode = string.Empty;

	private bool _isNoStatus;

	private bool isDraft;

	private string formInput;

	private IContainer components;

	private GridControlEx grd_ListOfDocument;

	private GridViewEx gv_ListOfDocument;

	private GridColumnEx gridColumn1;

	private GridColumnEx gcCardCode;

	private GridColumnEx gcCardName;

	private GridColumnEx gcRemark;

	private GridColumnEx gridColumn5;

	private SimpleButtonEx btnChoose;

	private GridColumnEx gcPostingDate;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private GridColumnEx gcRequesterType;

	private GridColumnEx gcReqName;

	private GridColumnEx gcReqTypeName;

	private SimpleButtonEx btnCancel;

	private GridColumnEx gridColumn2;

	public frm_List_Document(ApzObjectTypes type, bool multiSelect, string txtText, bool isNoStatus = false, bool isDraft = false)
	{
		InitializeComponent();
		SettingLanguage();
		this.type = type;
		Doc_MutiSelectMode = multiSelect;
		strCardCode = txtText;
		ChooseItems = new List<DocumentsImpl>();
		_isNoStatus = isNoStatus;
		this.isDraft = isDraft;
		FormId = "ListOfDocuments";
		gv_ListOfDocument.FormId = FormId;
		gv_ListOfDocument.ItemId = "ODOC";
		if (type == ApzObjectTypes.oPurchaseRequest)
		{
			gcCardCode.Visible = false;
			gcCardName.Visible = false;
			gcReqName.Visible = true;
			gcReqTypeName.Visible = true;
		}
		else
		{
			gcCardCode.Visible = true;
			gcCardName.Visible = true;
			gcReqName.Visible = false;
			gcReqTypeName.Visible = false;
		}
	}

	public frm_List_Document()
	{
		InitializeComponent();
	}

	private void SettingLanguage()
	{
		gcPostingDate.Caption = Resources.Doc_gvDoc_gcPostingDate;
		gcCardName.Caption = Resources.lb_Customer;
		gcRemark.Caption = Resources.Doc_gvDoc_Remark;
		btnChoose.Text = Resources.ButtonChoose;
		btnCancel.Text = Resources.ButtonCancel;
		if (type == ApzObjectTypes.oPurchaseRequest)
		{
			gcCardCode.Caption = Resources.DocPur_gvDoc_gcCardCode;
			gcReqName.Caption = Resources.Doc_gvDoc_gcRequester;
			gcReqTypeName.Caption = Resources.Doc_gvDoc_gcReqTypeName;
			Text = Resources.Titel_ListOfPurchaseRequest;
		}
		else
		{
			gcCardCode.Caption = Resources.DocSales_gvDoc_gcCardCode;
			Text = "List Document";
		}
	}

	private void frm_List_Document_Load(object sender, EventArgs e)
	{
		try
		{
			if (Doc_MutiSelectMode)
			{
				gv_ListOfDocument.OptionsSelection.MultiSelect = true;
				gv_ListOfDocument.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			}
			LoadData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_ListOfDocument.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					DocumentsImpl documentsImpl = (DocumentsImpl)gv_ListOfDocument.GetRow(rowHandle);
					if (Doc_MutiSelectMode)
					{
						ChooseItems.Add(documentsImpl);
						continue;
					}
					ChooseItem = documentsImpl;
					Close();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		Close();
	}

	private void LoadData()
	{
		try
		{
			WinCommons._OpenProcessing();
			_ = string.Empty;
			_ = string.Empty;
			if (isDraft)
			{
				using HttpClientHelper<List<DocumentsImpl>> httpClientHelper = new HttpClientHelper<List<DocumentsImpl>>(Configuration.ApiServer, "Commons");
				List<DocumentsImpl> dataSource = httpClientHelper.Get("ListDocDraft", new { type });
				grd_ListOfDocument.DataSource = dataSource;
				grd_ListOfDocument.RefreshDataSource();
			}
			else if (_isNoStatus)
			{
				using HttpClientHelper<List<DocumentsImpl>> httpClientHelper2 = new HttpClientHelper<List<DocumentsImpl>>(Configuration.ApiServer, "Commons");
				List<DocumentsImpl> dataSource2 = httpClientHelper2.Get("ListDocByCardCode_SalesOpportunity", new
				{
					type = type,
					cardCode = strCardCode
				});
				grd_ListOfDocument.DataSource = dataSource2;
				grd_ListOfDocument.RefreshDataSource();
			}
			else
			{
				using HttpClientHelper<List<DocumentsImpl>> httpClientHelper3 = new HttpClientHelper<List<DocumentsImpl>>(Configuration.ApiServer, "Commons");
				List<DocumentsImpl> dataSource3 = httpClientHelper3.Get("ListDocByCardCode", new
				{
					type = type,
					cardCode = strCardCode
				});
				grd_ListOfDocument.DataSource = dataSource3;
				grd_ListOfDocument.RefreshDataSource();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_ListOfDocument_RowClick(object sender, RowClickEventArgs e)
	{
	}

	private void gv_ListOfDocument_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			GridHitInfo gridHitInfo = gv_ListOfDocument.CalcHitInfo(grd_ListOfDocument.PointToClient(Control.MousePosition));
			if (Doc_MutiSelectMode || !gridHitInfo.InRow)
			{
				return;
			}
			int[] selectedRows = gv_ListOfDocument.GetSelectedRows();
			if (selectedRows.Length != 0)
			{
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					DocumentsImpl documentsImpl = (DocumentsImpl)gv_ListOfDocument.GetRow(rowHandle);
					if (Doc_MutiSelectMode)
					{
						ChooseItems.Add(documentsImpl);
						continue;
					}
					ChooseItem = documentsImpl;
					Close();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		Close();
	}

	private void gv_ListOfDocument_ShowingEditor(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void grd_ListOfDocument_Click(object sender, EventArgs e)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.CFL.frm_List_Document));
		this.grd_ListOfDocument = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ListOfDocument = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPostingDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRequesterType = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReqName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcReqTypeName = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_ListOfDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListOfDocument).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.grd_ListOfDocument.AllowPasteExcel = true;
		this.grd_ListOfDocument.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ListOfDocument.ClipboardData = null;
		this.grd_ListOfDocument.Location = new System.Drawing.Point(2, 1);
		this.grd_ListOfDocument.MainView = this.gv_ListOfDocument;
		this.grd_ListOfDocument.Name = "grd_ListOfDocument";
		this.grd_ListOfDocument.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemDateEdit1 });
		this.grd_ListOfDocument.Size = new System.Drawing.Size(1128, 452);
		this.grd_ListOfDocument.TabIndex = 2;
		this.grd_ListOfDocument.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ListOfDocument });
		this.grd_ListOfDocument.Click += new System.EventHandler(this.grd_ListOfDocument_Click);
		this.gv_ListOfDocument.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ListOfDocument.ColumnKey = null;
		this.gv_ListOfDocument.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumn1, this.gcPostingDate, this.gcCardCode, this.gcCardName, this.gcRemark, this.gridColumn5, this.gcRequesterType, this.gcReqName, this.gcReqTypeName, this.gridColumn2 });
		this.gv_ListOfDocument.DisableFormSetting = false;
		this.gv_ListOfDocument.DisplayException = false;
		this.gv_ListOfDocument.EditColumn = null;
		this.gv_ListOfDocument.FieldCurrency = null;
		this.gv_ListOfDocument.FieldRate = null;
		this.gv_ListOfDocument.FormId = "oListDocument";
		this.gv_ListOfDocument.GridControl = this.grd_ListOfDocument;
		this.gv_ListOfDocument.IgnoreFilter = false;
		this.gv_ListOfDocument.IndexEditRow = -1;
		this.gv_ListOfDocument.IndicatorWidth = 35;
		this.gv_ListOfDocument.IsApzTable = false;
		this.gv_ListOfDocument.IsInsertNewRow = false;
		this.gv_ListOfDocument.IsLoadingData = false;
		this.gv_ListOfDocument.IsMappingContextAction = false;
		this.gv_ListOfDocument.ItemId = "oListOfDocument";
		this.gv_ListOfDocument.LstColumnAllowBatch = null;
		this.gv_ListOfDocument.LstColumnNotAllowBatch = null;
		this.gv_ListOfDocument.LstColumnNotEditable = null;
		this.gv_ListOfDocument.LstColumNotSetting = null;
		this.gv_ListOfDocument.Name = "gv_ListOfDocument";
		this.gv_ListOfDocument.OptionsDetail.EnableMasterViewMode = false;
		this.gv_ListOfDocument.OptionsFilter.ShowAllTableValuesInCheckedFilterPopup = false;
		this.gv_ListOfDocument.OptionsFind.AlwaysVisible = true;
		this.gv_ListOfDocument.OptionsFind.FindDelay = 100;
		this.gv_ListOfDocument.OptionsView.ColumnAutoWidth = false;
		this.gv_ListOfDocument.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_ListOfDocument.OptionsView.ShowGroupPanel = false;
		this.gv_ListOfDocument.UseBatchMod = false;
		this.gv_ListOfDocument.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_ListOfDocument_RowClick);
		this.gv_ListOfDocument.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_ListOfDocument_ShowingEditor);
		this.gv_ListOfDocument.DoubleClick += new System.EventHandler(this.gv_ListOfDocument_DoubleClick);
		this.gridColumn1.Caption = "DocEntry";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "DocEntry";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Width = 52;
		this.gcPostingDate.Caption = "Posting Date";
		this.gcPostingDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.gcPostingDate.CurrencyDateField = null;
		this.gcPostingDate.CurrencyField = null;
		this.gcPostingDate.CurrencyFixed = null;
		this.gcPostingDate.CurrencyFixedRate = 0.0;
		this.gcPostingDate.CurrencyRateField = null;
		this.gcPostingDate.FieldName = "DocDate";
		this.gcPostingDate.Name = "gcPostingDate";
		this.gcPostingDate.OptionsColumn.ReadOnly = true;
		this.gcPostingDate.PrimaryKey = false;
		this.gcPostingDate.UnitField = null;
		this.gcPostingDate.Visible = true;
		this.gcPostingDate.VisibleIndex = 1;
		this.gcPostingDate.Width = 95;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyDateField = null;
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.OptionsColumn.ReadOnly = true;
		this.gcCardCode.PrimaryKey = false;
		this.gcCardCode.UnitField = null;
		this.gcCardCode.Visible = true;
		this.gcCardCode.VisibleIndex = 4;
		this.gcCardCode.Width = 89;
		this.gcCardName.Caption = "Customer";
		this.gcCardName.CurrencyDateField = null;
		this.gcCardName.CurrencyField = null;
		this.gcCardName.CurrencyFixed = null;
		this.gcCardName.CurrencyFixedRate = 0.0;
		this.gcCardName.CurrencyRateField = null;
		this.gcCardName.FieldName = "CardName";
		this.gcCardName.Name = "gcCardName";
		this.gcCardName.OptionsColumn.ReadOnly = true;
		this.gcCardName.PrimaryKey = false;
		this.gcCardName.UnitField = null;
		this.gcCardName.Visible = true;
		this.gcCardName.VisibleIndex = 5;
		this.gcCardName.Width = 117;
		this.gcRemark.Caption = "Remark";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Comments";
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.OptionsColumn.ReadOnly = true;
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 6;
		this.gcRemark.Width = 185;
		this.gridColumn5.Caption = "DueDate";
		this.gridColumn5.ColumnEdit = this.repositoryItemDateEdit1;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "DocDueDate";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 7;
		this.gridColumn5.Width = 176;
		this.gcRequesterType.Caption = "Requester Type";
		this.gcRequesterType.CurrencyDateField = null;
		this.gcRequesterType.CurrencyField = null;
		this.gcRequesterType.CurrencyFixed = null;
		this.gcRequesterType.CurrencyFixedRate = 0.0;
		this.gcRequesterType.CurrencyRateField = null;
		this.gcRequesterType.FieldName = "ReqType";
		this.gcRequesterType.Name = "gcRequesterType";
		this.gcRequesterType.OptionsColumn.ReadOnly = true;
		this.gcRequesterType.PrimaryKey = false;
		this.gcRequesterType.UnitField = null;
		this.gcRequesterType.Width = 116;
		this.gcReqName.Caption = "Requester";
		this.gcReqName.CurrencyDateField = null;
		this.gcReqName.CurrencyField = null;
		this.gcReqName.CurrencyFixed = null;
		this.gcReqName.CurrencyFixedRate = 0.0;
		this.gcReqName.CurrencyRateField = null;
		this.gcReqName.FieldName = "ReqName";
		this.gcReqName.Name = "gcReqName";
		this.gcReqName.OptionsColumn.ReadOnly = true;
		this.gcReqName.PrimaryKey = false;
		this.gcReqName.UnitField = null;
		this.gcReqName.Visible = true;
		this.gcReqName.VisibleIndex = 2;
		this.gcReqName.Width = 119;
		this.gcReqTypeName.Caption = "Requester Type";
		this.gcReqTypeName.CurrencyDateField = null;
		this.gcReqTypeName.CurrencyField = null;
		this.gcReqTypeName.CurrencyFixed = null;
		this.gcReqTypeName.CurrencyFixedRate = 0.0;
		this.gcReqTypeName.CurrencyRateField = null;
		this.gcReqTypeName.FieldName = "ReqTypeName";
		this.gcReqTypeName.Name = "gcReqTypeName";
		this.gcReqTypeName.OptionsColumn.ReadOnly = true;
		this.gcReqTypeName.PrimaryKey = false;
		this.gcReqTypeName.UnitField = null;
		this.gcReqTypeName.Visible = true;
		this.gcReqTypeName.VisibleIndex = 3;
		this.gcReqTypeName.Width = 89;
		this.gridColumn2.Caption = "#";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "DocNum";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Choose";
		this.btnChoose.FieldName = null;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(2, 459);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 3;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(80, 459);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(1131, 488);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.Controls.Add(this.grd_ListOfDocument);
		base.FormId = "oListDocument";
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_List_Document";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "List Document";
		base.Load += new System.EventHandler(this.frm_List_Document_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_ListOfDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ListOfDocument).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
