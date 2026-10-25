using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Svg;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.DMS;

public class frm_Marker : XtraFormEx
{
	private const string OkString = "OK";

	private const string AddString = "Save";

	private DataTable _dtDoc;

	private bool _isLoading;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_ItemCode;

	private GridColumnEx gc_FileName;

	private RepositoryItemButtonEdit rep_Browse;

	private GridColumnEx gc_Name;

	private GridColumnEx gc_Image;

	private RepositoryItemPictureEdit rep_Image;

	private GridColumnEx gc_DfltColorHex;

	private GridColumnEx gc_DfltSize;

	private GridViewEx gridViewEx1;

	private GridColumnEx gridColumnEx1;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private RepositoryItemColorPickEdit repositoryItemColorPickEdit1;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Save";
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_Marker()
	{
		InitializeComponent();
		OnLoadAsync += Frm_Marker_Load;
	}

	private async Task Frm_Marker_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await LoadData();
			dataTable.Columns.Add("FilePath", typeof(string));
			dataTable.Columns.Add("ImageSVG", typeof(Image));
			dataTable.Columns.Add("IsDb", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string svgData = Function.ToString(dataTable.Rows[i]["SvgData"]);
				Function.ParseInt(dataTable.Rows[i]["DfltSize"]);
				string text = Function.ToString(dataTable.Rows[i]["DfltColorHex"]);
				Image value = ((!string.IsNullOrEmpty(text)) ? Function.ConvertSVGtoPNG(svgData, 30, 30, text) : ConvertSVGtoImage(svgData, 30, 30));
				dataTable.Rows[i]["ImageSVG"] = value;
				dataTable.Rows[i]["IsDb"] = "Y";
			}
			grd_Doc.DataSource = dataTable;
			grd_Doc.DatasourceTable().AcceptChanges();
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async Task<DataTable> LoadData()
	{
		try
		{
			return await _httpClient.MarkerClient.LoadData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void Rep_Browse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				OpenFileDialog openFileDialog = new OpenFileDialog();
				openFileDialog.ShowReadOnly = true;
				openFileDialog.Multiselect = false;
				openFileDialog.Filter = "Files Pictures (*.SVG)|*.SVG";
				if (openFileDialog.ShowDialog() == DialogResult.OK && openFileDialog.FileNames.Length != 0 && !string.IsNullOrEmpty(openFileDialog.FileNames[0]))
				{
					Path.GetDirectoryName(openFileDialog.FileNames[0]);
					Image value = SvgBitmap.FromFile(openFileDialog.FileNames[0]).Render(null, 1.0);
					gv_Doc.SetFocusedRowCellValue("ImageSVG", value);
					gv_Doc.SetFocusedRowCellValue("Name", openFileDialog.SafeFileNames[0]);
					gv_Doc.SetFocusedRowCellValue("FilePath", Path.GetDirectoryName(openFileDialog.FileNames[0]));
					string value2 = File.ReadAllText(openFileDialog.FileNames[0]);
					gv_Doc.SetFocusedRowCellValue("SvgData", value2);
					grd_Doc.DataSourceTable.AcceptChanges();
				}
			}
		}
		catch (Exception)
		{
		}
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["DfltSize"] = 16;
			dataSourceTable.Rows.Add(dataRow);
			grd_Doc.DataSource = dataSourceTable;
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Frm_Marker_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void Btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await Save();
				}
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			_dtDoc = grd_Doc.DataSourceTable.Copy();
			List<string> list = (from dr in _dtDoc.Copy().AsEnumerable()
				select dr.Field<string>("Code") into x
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			if (list.Count() > 0)
			{
				WinCommons.Error(Enums.MessageCode.ValueExistsInTableWithId, "", Function.ToString(list[0]));
				return 1;
			}
			if (_dtDoc != null && _dtDoc.Rows.Count > 0)
			{
				for (int num = 0; num < _dtDoc.Rows.Count; num++)
				{
					if (Function.ToString(_dtDoc.Rows[num]["Code"]) == "" && Function.ToString(_dtDoc.Rows[num]["Name"]) == "")
					{
						_dtDoc.Rows.RemoveAt(num--);
					}
					if (Function.ToString(_dtDoc.Rows[num]["Code"]) == "")
					{
						WinCommons.Warning(Enums.MessageCode.CodeNotNull, "");
						return 1;
					}
					if (Function.ParseInt(_dtDoc.Rows[num]["DfltSize"]) <= 0)
					{
						_dtDoc.Rows[num]["DfltSize"] = 16;
					}
				}
			}
			grd_Doc.DatasourceTable().AcceptChanges();
			_dtDoc.TableName = "dtDoc";
			HttpResult httpResult = await _httpClient.MarkerClient.Saves(_dtDoc);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				await Frm_Marker_Load(null, null);
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private void Gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "DfltColorHex")
			{
				string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "SvgData"));
				string text2 = Function.ConvertIntToHex(Function.ParseInt(e.Value));
				if (!string.IsNullOrEmpty(text2) && !string.IsNullOrEmpty(text))
				{
					Image value = Function.ConvertSVGtoPNG(text, 30, 30, text2);
					gv_Doc.SetRowCellValue(e.RowHandle, "ImageSVG", value);
				}
			}
			if (e.Column.FieldName == "SvgData")
			{
				string text3 = Function.ToString(e.Value);
				string text4 = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "DfltColorHex"));
				if (!string.IsNullOrEmpty(text4) && !string.IsNullOrEmpty(text3))
				{
					string colorHex = Function.ConvertIntToHex(Function.ParseInt(text4));
					Image value2 = Function.ConvertSVGtoPNG(text3, 10, 10, colorHex);
					gv_Doc.SetRowCellValue(e.RowHandle, "ImageSVG", value2);
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void DeleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null || gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = await _httpClient.MarkerClient.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						gv_Doc.BeginUpdate();
						gv_Doc.DeleteSelectedRows();
						gv_Doc.EndUpdate();
						grd_Doc.DatasourceTable().AcceptChanges();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "IsDb"));
		if (e.Column.FieldName == "Code" && text.Equals("Y"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void Gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("IsDb"));
			if (gv_Doc.FocusedColumn.FieldName == "Code" && text.Equals("Y"))
			{
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = true;
			}
			else
			{
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public Image ConvertSVGtoImage(string svgData, int width, int height, string colorHex = "")
	{
		byte[] bytes = Encoding.ASCII.GetBytes(svgData);
		using (new MemoryStream(bytes))
		{
			return SvgBitmap.Create(SvgImageBinaryConverter.FromByteArray(bytes)).Render(null, 1.0);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_Marker));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_FileName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Browse = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Image = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Image = new DevExpress.XtraEditors.Repository.RepositoryItemPictureEdit();
		this.gc_DfltColorHex = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemColorPickEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemColorPickEdit();
		this.gc_DfltSize = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Browse).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Image).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(3, 12);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Browse, this.rep_Image, this.repositoryItemMemoEdit1, this.repositoryItemColorPickEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(1058, 286);
		this.grd_Doc.TabIndex = 2;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Doc, this.gridViewEx1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.AddNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.DeleteToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gc_ItemCode, this.gc_FileName, this.gc_Name, this.gc_Image, this.gc_DfltColorHex, this.gc_DfltSize, this.gridColumnEx1 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUploadItemImage";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_UUPIM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsCustomization.AllowRowSizing = true;
		this.gv_Doc.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.RowHeight = 30;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Gv_Doc_RowCellStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.Gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Gv_Doc_CellValueChanged);
		this.gc_ItemCode.Caption = "Mã";
		this.gc_ItemCode.CurrencyDateField = null;
		this.gc_ItemCode.CurrencyField = null;
		this.gc_ItemCode.CurrencyFixed = null;
		this.gc_ItemCode.CurrencyFixedRate = 0.0;
		this.gc_ItemCode.CurrencyRateField = null;
		this.gc_ItemCode.FieldName = "Code";
		this.gc_ItemCode.Name = "gc_ItemCode";
		this.gc_ItemCode.PrimaryKey = true;
		this.gc_ItemCode.UnitField = null;
		this.gc_ItemCode.Visible = true;
		this.gc_ItemCode.VisibleIndex = 0;
		this.gc_ItemCode.Width = 150;
		this.gc_FileName.Caption = "Đường dẫn";
		this.gc_FileName.ColumnEdit = this.rep_Browse;
		this.gc_FileName.CurrencyDateField = null;
		this.gc_FileName.CurrencyField = null;
		this.gc_FileName.CurrencyFixed = null;
		this.gc_FileName.CurrencyFixedRate = 0.0;
		this.gc_FileName.CurrencyRateField = null;
		this.gc_FileName.FieldName = "FilePath";
		this.gc_FileName.Name = "gc_FileName";
		this.gc_FileName.OptionsColumn.ReadOnly = true;
		this.gc_FileName.PrimaryKey = false;
		this.gc_FileName.UnitField = null;
		this.gc_FileName.Visible = true;
		this.gc_FileName.VisibleIndex = 1;
		this.gc_FileName.Width = 150;
		this.rep_Browse.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_Browse.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Browse.Name = "rep_Browse";
		this.rep_Browse.ReadOnly = true;
		this.rep_Browse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.Rep_Browse_ButtonClick);
		this.gc_Name.Caption = "Tên";
		this.gc_Name.CurrencyDateField = null;
		this.gc_Name.CurrencyField = null;
		this.gc_Name.CurrencyFixed = null;
		this.gc_Name.CurrencyFixedRate = 0.0;
		this.gc_Name.CurrencyRateField = null;
		this.gc_Name.FieldName = "Name";
		this.gc_Name.Name = "gc_Name";
		this.gc_Name.PrimaryKey = false;
		this.gc_Name.UnitField = null;
		this.gc_Name.Visible = true;
		this.gc_Name.VisibleIndex = 2;
		this.gc_Name.Width = 150;
		this.gc_Image.Caption = "Ảnh";
		this.gc_Image.ColumnEdit = this.rep_Image;
		this.gc_Image.CurrencyDateField = null;
		this.gc_Image.CurrencyField = null;
		this.gc_Image.CurrencyFixed = null;
		this.gc_Image.CurrencyFixedRate = 0.0;
		this.gc_Image.CurrencyRateField = null;
		this.gc_Image.FieldName = "ImageSVG";
		this.gc_Image.MaxWidth = 30;
		this.gc_Image.MinWidth = 10;
		this.gc_Image.Name = "gc_Image";
		this.gc_Image.OptionsColumn.FixedWidth = true;
		this.gc_Image.OptionsColumn.ReadOnly = true;
		this.gc_Image.PrimaryKey = false;
		this.gc_Image.UnitField = null;
		this.gc_Image.Visible = true;
		this.gc_Image.VisibleIndex = 3;
		this.gc_Image.Width = 30;
		this.rep_Image.AllowZoomOnMouseWheel = DevExpress.Utils.DefaultBoolean.True;
		this.rep_Image.Name = "rep_Image";
		this.rep_Image.ShowZoomSubMenu = DevExpress.Utils.DefaultBoolean.True;
		this.rep_Image.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Zoom;
		this.rep_Image.ZoomingOperationMode = DevExpress.XtraEditors.Repository.ZoomingOperationMode.MouseWheel;
		this.gc_DfltColorHex.Caption = "Mã màu";
		this.gc_DfltColorHex.ColumnEdit = this.repositoryItemColorPickEdit1;
		this.gc_DfltColorHex.CurrencyDateField = null;
		this.gc_DfltColorHex.CurrencyField = null;
		this.gc_DfltColorHex.CurrencyFixed = null;
		this.gc_DfltColorHex.CurrencyFixedRate = 0.0;
		this.gc_DfltColorHex.CurrencyRateField = null;
		this.gc_DfltColorHex.FieldName = "DfltColorHex";
		this.gc_DfltColorHex.Name = "gc_DfltColorHex";
		this.gc_DfltColorHex.PrimaryKey = false;
		this.gc_DfltColorHex.UnitField = null;
		this.gc_DfltColorHex.Visible = true;
		this.gc_DfltColorHex.VisibleIndex = 4;
		this.gc_DfltColorHex.Width = 180;
		this.repositoryItemColorPickEdit1.AutoHeight = false;
		this.repositoryItemColorPickEdit1.AutomaticColor = System.Drawing.Color.Black;
		this.repositoryItemColorPickEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemColorPickEdit1.Name = "repositoryItemColorPickEdit1";
		this.repositoryItemColorPickEdit1.StoreColorAsInteger = true;
		this.gc_DfltSize.Caption = "Kích thước";
		this.gc_DfltSize.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gc_DfltSize.CurrencyDateField = null;
		this.gc_DfltSize.CurrencyField = null;
		this.gc_DfltSize.CurrencyFixed = null;
		this.gc_DfltSize.CurrencyFixedRate = 0.0;
		this.gc_DfltSize.CurrencyRateField = null;
		this.gc_DfltSize.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_DfltSize.FieldName = "DfltSize";
		this.gc_DfltSize.Name = "gc_DfltSize";
		this.gc_DfltSize.PrimaryKey = false;
		this.gc_DfltSize.UnitField = null;
		this.gc_DfltSize.Visible = true;
		this.gc_DfltSize.VisibleIndex = 5;
		this.gc_DfltSize.Width = 85;
		this.gridColumnEx1.Caption = "Xml";
		this.gridColumnEx1.ColumnEdit = this.repositoryItemMemoEdit1;
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "SvgData";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Width = 301;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.GridControl = this.grd_Doc;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "Ok";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(12, 304);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 31);
		this.btn_Ok.TabIndex = 3;
		this.btn_Ok.Text = "Ok";
		this.btn_Ok.Click += new System.EventHandler(this.Btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(104, 304);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 31);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.Btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1063, 339);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Marker";
		this.Text = "Marker";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Browse).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Image).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemColorPickEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		base.ResumeLayout(false);
	}
}
