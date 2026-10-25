using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.PM;

public class frm_Comments : XtraFormEx
{
	private IContainer components;

	private GridControl gridControl1;

	private GridView gridView1;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumn gridColumn3;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private MemoEdit memoEdit1;

	private SimpleButton simpleButton1;

	public string ObjectType { get; set; }

	public string ObjectCode { get; set; }

	public string ObjectDetail { get; set; }

	public frm_Comments()
	{
		InitializeComponent();
	}

	private async void frm_Comments_Load(object sender, EventArgs e)
	{
		try
		{
			GridColumn gridColumn = gridView1.Columns["CREATE_TIME"];
			if (gridColumn != null)
			{
				gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
				gridColumn.DisplayFormat.FormatString = GlobalData.DateTimeFormat + " HH:mm:ss";
			}
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void simpleButton1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string text = Function.ToString(memoEdit1.EditValue);
			if (!string.IsNullOrWhiteSpace(text))
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("OBJTYPE", typeof(string));
				dataTable.Columns.Add("OBJECT_CODE", typeof(string));
				dataTable.Columns.Add("DETAIL_CODE", typeof(string));
				dataTable.Columns.Add("COMMENTS", typeof(string));
				dataTable.Columns.Add("CREATED_BY", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["OBJTYPE"] = ObjectType;
				dataRow["OBJECT_CODE"] = ObjectCode;
				dataRow["DETAIL_CODE"] = ObjectDetail;
				if (text.Length > 500)
				{
					text = text.Substring(0, 500);
				}
				dataRow["COMMENTS"] = text;
				dataTable.Rows.Add(dataRow);
				HttpResult httpResult = await HttpClient.Comments.AddNew(dataTable);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
				}
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = await HttpClient.Comments.GetByObject(ObjectType, ObjectCode, ObjectDetail);
			if (dataTable.IsNotNull())
			{
				gridControl1.DataSource = dataTable;
				gridControl1.RefreshDataSource();
			}
			memoEdit1.EditValue = string.Empty;
			memoEdit1.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_Comments));
		this.gridControl1 = new DevExpress.XtraGrid.GridControl();
		this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn3 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.memoEdit1 = new DevExpress.XtraEditors.MemoEdit();
		this.simpleButton1 = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit1.Properties).BeginInit();
		base.SuspendLayout();
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.Location = new System.Drawing.Point(2, 2);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemMemoExEdit1 });
		this.gridControl1.Size = new System.Drawing.Size(555, 339);
		this.gridControl1.TabIndex = 0;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn2, this.gridColumn3 });
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsBehavior.ReadOnly = true;
		this.gridView1.OptionsView.AutoCalcPreviewLineCount = true;
		this.gridView1.OptionsView.ShowColumnHeaders = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.OptionsView.ShowIndicator = false;
		this.gridView1.OptionsView.ShowPreview = true;
		this.gridView1.PreviewFieldName = "COMMENTS";
		this.gridView1.PreviewIndent = 5;
		this.gridColumn1.Caption = "CREATOR_NAME";
		this.gridColumn1.FieldName = "CREATOR_NAME";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 396;
		this.gridColumn2.Caption = "CREATE_TIME";
		this.gridColumn2.FieldName = "CREATE_TIME";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 170;
		this.gridColumn3.Caption = "COMMENTS";
		this.gridColumn3.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumn3.FieldName = "COMMENTS";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Width = 229;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.memoEdit1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.memoEdit1.Location = new System.Drawing.Point(2, 347);
		this.memoEdit1.Name = "memoEdit1";
		this.memoEdit1.Size = new System.Drawing.Size(504, 37);
		this.memoEdit1.TabIndex = 1;
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleButton1.ImageOptions.SvgImage = (DevExpress.Utils.Svg.SvgImage)resources.GetObject("simpleButton1.ImageOptions.SvgImage");
		this.simpleButton1.Location = new System.Drawing.Point(512, 349);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.Size = new System.Drawing.Size(45, 35);
		this.simpleButton1.TabIndex = 2;
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(558, 396);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.memoEdit1);
		base.Controls.Add(this.gridControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Comments.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Comments";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Comments";
		base.Load += new System.EventHandler(this.frm_Comments_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memoEdit1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
