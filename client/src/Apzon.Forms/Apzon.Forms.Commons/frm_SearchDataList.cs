using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class frm_SearchDataList : XtraFormEx
{
	public DataTable SourceDt;

	public dynamic ChooseItem;

	public string XFormId;

	public string ItemId;

	public ApzObjectTypes ObjType;

	private IContainer components;

	private GridControlEx grdDoc;

	private SimpleButtonEx btClose;

	private SimpleButtonEx btChoose;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private GridViewEx gvDoc;

	private SimpleButtonEx btn_Find;

	private SimpleButtonEx btn_Clear;

	private TextEditEx txt_Search;

	public frm_SearchDataList()
	{
		InitializeComponent();
		OnLoadAsync += frm_BusinessMasterDataList_Load;
	}

	private async Task frm_BusinessMasterDataList_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			DataSetResult dataSetResult = await HttpClient.Cfl.GetCflStructure(ObjType);
			if (string.IsNullOrEmpty(dataSetResult.Message))
			{
				DataSet data = dataSetResult.Data;
				if (data != null && data.Tables.Count == 2 && data.Tables[0].IsNotNull())
				{
					await GridViewHelper.GenerateGrid(grdDoc, gvDoc, data.Tables[0], SourceDt, data.Tables[1]);
				}
			}
			foreach (GridColumn column in gvDoc.Columns)
			{
				column.OptionsFilter.AllowFilter = true;
			}
			gvDoc.OptionsSelection.MultiSelect = false;
			grdDoc.DataSource = SourceDt;
			grdDoc.RefreshDataSource();
			FormId = XFormId;
			gvDoc.FormId = XFormId;
			gvDoc.ItemId = ItemId;
			gvDoc.ReloadFormSetting();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btClose_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			DXMouseEventArgs e2 = e as DXMouseEventArgs;
			if (gvDoc.CalcHitInfo(e2.Location).InRow)
			{
				AddChooseItemsGrid();
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Find_Click(object sender, EventArgs e)
	{
		try
		{
			string text = txt_Search.Text;
			switch (ObjType)
			{
			case ApzObjectTypes.oBusinessPartners:
				gvDoc.ActiveFilterString = "Contains([CardCode],'" + text + "') OR Contains([CardName],'" + text + "')";
				break;
			case ApzObjectTypes.oItems:
				gvDoc.ActiveFilterString = "Contains([ItemCode],'" + text + "') OR Contains([ItemName],'" + text + "')";
				break;
			case ApzObjectTypes.oProductTrees:
				gvDoc.ActiveFilterString = "Contains([Code],'" + text + "') OR Contains([ItemName],'" + text + "')";
				break;
			case ApzObjectTypes.oProductionOrders:
				gvDoc.ActiveFilterString = "Contains([ItemCode],'" + text + "') OR Contains([ItemName],'" + text + "')";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Clear_Click(object sender, EventArgs e)
	{
		try
		{
			txt_Search.Text = "";
			gvDoc.ActiveFilterString = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btChoose_Click(object sender, EventArgs e)
	{
		try
		{
			AddChooseItemsGrid();
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_Search_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return)
			{
				btn_Find_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			if (gvDoc.GetSelectedRows() != null && gvDoc.GetSelectedRows().Length != 0)
			{
				int[] selectedRows = gvDoc.GetSelectedRows();
				int num = 0;
				if (num < selectedRows.Length)
				{
					int rowHandle = selectedRows[num];
					ChooseItem = gvDoc.GetRow(rowHandle);
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_SearchDataList));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btClose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Find = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Search = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(12, 41);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCalcEdit1 });
		this.grdDoc.Size = new System.Drawing.Size(628, 522);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oSearchDataList";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 60;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "oDATA";
		this.gvDoc.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsFind.AllowFindPanel = false;
		this.gvDoc.OptionsFind.FindDelay = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowDetailButtons = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.DoubleClick += new System.EventHandler(this.gvDoc_DoubleClick);
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.btClose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btClose.FieldName = null;
		this.btClose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btClose.ImageOptions.Image");
		this.btClose.IsSearchControl = false;
		this.btClose.Location = new System.Drawing.Point(93, 573);
		this.btClose.Name = "btClose";
		this.btClose.Size = new System.Drawing.Size(75, 23);
		this.btClose.TabIndex = 1;
		this.btClose.Text = "Close";
		this.btClose.Click += new System.EventHandler(this.btClose_Click);
		this.btChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btChoose.FieldName = null;
		this.btChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btChoose.ImageOptions.Image");
		this.btChoose.IsSearchControl = false;
		this.btChoose.Location = new System.Drawing.Point(12, 573);
		this.btChoose.Name = "btChoose";
		this.btChoose.Size = new System.Drawing.Size(75, 23);
		this.btChoose.TabIndex = 2;
		this.btChoose.Text = "Choose";
		this.btChoose.Click += new System.EventHandler(this.btChoose_Click);
		this.btn_Find.FieldName = null;
		this.btn_Find.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Find.ImageOptions.Image");
		this.btn_Find.IsSearchControl = false;
		this.btn_Find.Location = new System.Drawing.Point(342, 10);
		this.btn_Find.Name = "btn_Find";
		this.btn_Find.Size = new System.Drawing.Size(64, 23);
		this.btn_Find.TabIndex = 3;
		this.btn_Find.Text = "Find";
		this.btn_Find.Click += new System.EventHandler(this.btn_Find_Click);
		this.btn_Clear.FieldName = null;
		this.btn_Clear.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.ImageOptions.Image");
		this.btn_Clear.IsSearchControl = false;
		this.btn_Clear.Location = new System.Drawing.Point(409, 10);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.Size = new System.Drawing.Size(61, 23);
		this.btn_Clear.TabIndex = 4;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
		this.txt_Search.FieldName = null;
		this.txt_Search.IsSearchControl = false;
		this.txt_Search.Location = new System.Drawing.Point(12, 12);
		this.txt_Search.Name = "txt_Search";
		this.txt_Search.Size = new System.Drawing.Size(324, 20);
		this.txt_Search.TabIndex = 5;
		this.txt_Search.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Search_KeyDown);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btClose;
		base.ClientSize = new System.Drawing.Size(653, 608);
		base.Controls.Add(this.txt_Search);
		base.Controls.Add(this.btn_Clear);
		base.Controls.Add(this.btn_Find);
		base.Controls.Add(this.btChoose);
		base.Controls.Add(this.btClose);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oSearchDataList";
		base.Name = "frm_SearchDataList";
		base.ShowIcon = false;
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
