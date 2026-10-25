using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Helper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using ExcelDataReader;

namespace Apzon.Forms.Commons;

public class frm_ReadExcelToGrid : XtraFormEx
{
	private DataSet result;

	private IContainer components;

	private LabelLine labelLine1;

	private SimpleButton btn_Browse;

	private GridControlEx grd_Data;

	private GridViewEx gv_Data;

	private System.Windows.Forms.ComboBox cboSheet;

	private SimpleButton btn_OK;

	private SimpleButton btn_Cancel;

	public frm_ReadExcelToGrid()
	{
		InitializeComponent();
	}

	private void frm_ReadExcelToGrid_Load(object sender, EventArgs e)
	{
	}

	private void btn_Browse_Click(object sender, EventArgs e)
	{
		try
		{
			using (OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Filter = "Excel Files|*.xls;*.xlsx",
				ValidateNames = true
			})
			{
				if (openFileDialog.ShowDialog() == DialogResult.OK)
				{
					IExcelDataReader excelDataReader = ExcelReaderFactory.CreateReader(File.Open(openFileDialog.FileName, FileMode.Open, FileAccess.Read));
					ExcelDataSetConfiguration configuration = new ExcelDataSetConfiguration
					{
						ConfigureDataTable = (IExcelDataReader _) => new ExcelDataTableConfiguration
						{
							UseHeaderRow = true
						}
					};
					result = excelDataReader.AsDataSet(configuration);
					FixDataSet(result);
					cboSheet.Items.Clear();
					foreach (DataTable table in result.Tables)
					{
						cboSheet.Items.Add(table.TableName);
					}
					excelDataReader.Close();
				}
			}
			if (cboSheet.Items.Count > 0)
			{
				cboSheet.SelectedIndex = 0;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FixDataSet(DataSet ds)
	{
		try
		{
			List<DataTable> list = new List<DataTable>(ds.Tables.Count);
			foreach (DataTable table in ds.Tables)
			{
				if (table.Rows.Count == 0)
				{
					list.Add(table);
					continue;
				}
				foreach (DataColumn column in table.Columns)
				{
					column.ColumnName = column.ColumnName.Trim(' ', '\t', '\r', '\n');
				}
				IEnumerable<string> enumerable = from DataColumn dataColumn2 in table.Columns
					where dataColumn2.DataType == typeof(DateTime)
					select dataColumn2.ColumnName;
				List<string> list2 = new List<string>();
				foreach (string c in enumerable)
				{
					if (table.AsEnumerable().All((DataRow r) => ((DateTime)r[c]).Year < 1900))
					{
						list2.Add(c);
					}
				}
				if (list2.Count > 0)
				{
					DataTable dataTable2 = table.Clone();
					foreach (string item in list2)
					{
						dataTable2.Columns[item].DataType = typeof(string);
					}
					dataTable2.BeginLoadData();
					for (int num = 0; num < table.Rows.Count; num++)
					{
						dataTable2.ImportRow(table.Rows[num]);
						foreach (string item2 in list2)
						{
							dataTable2.Rows[num][item2] = ((DateTime)table.Rows[num][item2]).ToString("HHmm");
						}
					}
					dataTable2.EndLoadData();
					list.Add(dataTable2);
				}
				else
				{
					list.Add(table);
				}
			}
			ds.Tables.Clear();
			ds.Tables.AddRange(list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private async void cboSheet_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			gv_Data.Columns.Clear();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(result.Tables[cboSheet.SelectedIndex].Copy());
			await GridViewHelper.GenerateGrid(grd_Data, gv_Data, dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ReadExcelToGrid));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Browse = new DevExpress.XtraEditors.SimpleButton();
		this.grd_Data = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Data = new Apzon.Controls.Grid.GridViewEx();
		this.cboSheet = new System.Windows.Forms.ComboBox();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_Data).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).BeginInit();
		base.SuspendLayout();
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Sheet";
		this.btn_Browse.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Browse.ImageOptions.Image");
		this.btn_Browse.Location = new System.Drawing.Point(300, 9);
		this.btn_Browse.Name = "btn_Browse";
		this.btn_Browse.Size = new System.Drawing.Size(81, 21);
		this.btn_Browse.TabIndex = 2;
		this.btn_Browse.Text = "Browse";
		this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
		this.grd_Data.AllowPasteExcel = false;
		this.grd_Data.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Data.ClipboardData = null;
		this.grd_Data.Location = new System.Drawing.Point(1, 39);
		this.grd_Data.MainView = this.gv_Data;
		this.grd_Data.Name = "grd_Data";
		this.grd_Data.Size = new System.Drawing.Size(913, 468);
		this.grd_Data.TabIndex = 3;
		this.grd_Data.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Data });
		this.gv_Data.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Data.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Data.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Data.ColumnKey = null;
		this.gv_Data.DisableFormSetting = true;
		this.gv_Data.DisplayException = false;
		this.gv_Data.EditColumn = null;
		this.gv_Data.FieldCurrency = null;
		this.gv_Data.FieldRate = null;
		this.gv_Data.FormId = "oReadExcelToGrid";
		this.gv_Data.GridControl = this.grd_Data;
		this.gv_Data.IndexEditRow = -1;
		this.gv_Data.IndicatorWidth = 35;
		this.gv_Data.IsApzTable = false;
		this.gv_Data.IsInsertNewRow = false;
		this.gv_Data.IsLoadingData = false;
		this.gv_Data.ItemId = "ODATA";
		this.gv_Data.LstColumnAllowBatch = null;
		this.gv_Data.LstColumnNotAllowBatch = null;
		this.gv_Data.LstColumNotSetting = null;
		this.gv_Data.Name = "gv_Data";
		this.gv_Data.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Data.OptionsSelection.MultiSelect = true;
		this.gv_Data.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Data.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Data.OptionsView.ColumnAutoWidth = false;
		this.gv_Data.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Data.OptionsView.ShowGroupPanel = false;
		this.gv_Data.UseBatchMod = false;
		this.cboSheet.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
		this.cboSheet.FormattingEnabled = true;
		this.cboSheet.Location = new System.Drawing.Point(76, 9);
		this.cboSheet.Name = "cboSheet";
		this.cboSheet.Size = new System.Drawing.Size(218, 21);
		this.cboSheet.TabIndex = 6;
		this.cboSheet.SelectedIndexChanged += new System.EventHandler(this.cboSheet_SelectedIndexChanged);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(1, 513);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 7;
		this.btn_OK.Text = "OK";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(82, 513);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(917, 540);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.cboSheet);
		base.Controls.Add(this.grd_Data);
		base.Controls.Add(this.btn_Browse);
		base.Controls.Add(this.labelLine1);
		base.FormId = "oReadExcelToGrid";
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_ReadExcelToGrid";
		base.ShowIcon = false;
		this.Text = "Read Excel To Grid";
		base.Load += new System.EventHandler(this.frm_ReadExcelToGrid_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Data).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Data).EndInit();
		base.ResumeLayout(false);
	}
}
