using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;

namespace Apzon.Controls.Pivot;

[ToolboxItem(true)]
public class PivotGridControlEx : PivotGridControl
{
	private DataSet _dsContextAction = new DataSet();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public LookUpEdit FilterFieldControl { get; set; }

	public FilterValueControl FilterValueControl { get; set; }

	public string FormId { get; set; }

	public string ItemId { get; set; }

	public ApzObjectTypes ObjectType { get; set; }

	public bool AllowPasteExcel { get; set; }

	public DataTable ClipboardData { get; set; }

	public new object DataSource
	{
		get
		{
			return base.DataSource;
		}
		set
		{
			base.DataSource = value;
		}
	}

	public PivotGridControlEx()
	{
		CustomRowHeight += PivotGridControlEx_CustomRowHeight;
		MouseWheel += PivotGridControlEx_MouseWheel;
		if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
		{
			return;
		}
		Enter += (object sender, EventArgs e) =>
		{
			if (FindForm() is XtraFormEx { Modal: false } && XtraFormEx.EnableDirectionalControl != null)
			{
				Enums.DirectionalControl[] gridToolbarButtons = XtraFormEx.GridToolbarButtons;
				foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
				{
					XtraFormEx.EnableDirectionalControl(diectioControl, isEnable: true);
				}
			}
		};
		Leave += (object sender, EventArgs e) =>
		{
			if (FindForm() is XtraFormEx { Modal: false })
			{
				Enums.DirectionalControl[] gridToolbarButtons = XtraFormEx.GridToolbarButtons;
				foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
				{
					XtraFormEx.EnableDirectionalControl(diectioControl, isEnable: false);
				}
			}
		};
	}

	private void PivotGridControlEx_CustomRowHeight(object sender, PivotCustomRowHeightEventArgs e)
	{
		if (e.Value == null)
		{
			return;
		}
		using Graphics graphics = CreateGraphics();
		if (e.DataField != null)
		{
			string text = e.Value.ToString();
			Font font = Appearance.FieldValue.Font;
			int num = (int)Math.Ceiling(graphics.MeasureString(text, font, e.DataField.Width).Height / font.GetHeight());
			e.RowHeight *= num;
			e.RowHeight += 10;
		}
	}

	private void PivotGridControlEx_MouseWheel(object sender, MouseEventArgs e)
	{
		if (Control.ModifierKeys.HasFlag(Keys.Shift))
		{
			LeftTopCoord = new Point(LeftTopCoord.X - e.Delta / SystemInformation.MouseWheelScrollDelta, LeftTopCoord.Y);
			((DXMouseEventArgs)e).Handled = true;
		}
	}

	protected override void OnLoaded()
	{
		try
		{
			if (FilterFieldControl != null && FilterValueControl != null)
			{
				FilterFieldControl.Properties.Columns.Clear();
				FilterFieldControl.Properties.Columns.Add(new LookUpColumnInfo("FilterCode"));
				FilterFieldControl.Properties.Columns.Add(new LookUpColumnInfo("FilterName"));
				FilterFieldControl.Properties.DisplayMember = "FilterName";
				FilterFieldControl.Properties.ValueMember = "FilterCode";
				FilterFieldControl.EditValueChanged += FilterFieldControl_EditValueChanged;
				FilterValueControl.EditValueChanged += txtFilterValue_EditValueChanged;
				DataTable dataFilterField = _httpClient.FormFilterSettingClient.GetDataFilterField(ObjectType);
				if (dataFilterField.IsNotNull())
				{
					DataRow dataRow = dataFilterField.NewRow();
					dataRow[0] = -1;
					dataRow[1] = "";
					dataRow[2] = "";
					dataFilterField.Rows.Add(dataRow);
					dataFilterField.AcceptChanges();
				}
				FilterFieldControl.Properties.DataSource = dataFilterField;
			}
		}
		catch (Exception)
		{
		}
	}

	private async void FilterFieldControl_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FilterValueControl.EditValue = "";
			foreach (PivotGridField field in Fields)
			{
				field.FilterValues.FilterType = PivotFilterType.Excluded;
				field.FilterValues.Clear();
			}
			DataTable dataTable = FilterFieldControl.Properties.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			string fieldName = Function.ToString(FilterFieldControl.EditValue);
			if (string.IsNullOrEmpty(fieldName))
			{
				return;
			}
			DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["FilterCode"]).Equals(fieldName));
			if (dataRow == null)
			{
				return;
			}
			if (Fields.Cast<PivotGridField>().FirstOrDefault((PivotGridField t) => t.FieldName.Equals(fieldName)) == null)
			{
				PivotGridField pivotGridField2 = new PivotGridField();
				pivotGridField2.Name = "field_filterAdd_" + fieldName;
				pivotGridField2.FieldName = fieldName;
				pivotGridField2.Visible = false;
				Fields.Add(pivotGridField2);
			}
			if (Function.ToString(dataRow["SourceType"]).Equals("U"))
			{
				if (DataSource is DataTable dataTable2 && dataTable2.Columns.Contains(fieldName))
				{
					DataTable dataTable3 = dataTable2.AsDataView().ToTable(true, fieldName);
					dataTable3.Columns[0].ColumnName = "Code";
					DataColumn column = new DataColumn("Name")
					{
						Expression = "Code"
					};
					dataTable3.Columns.Add(column);
					dataTable3.AcceptChanges();
					FilterValueControl.SourceFilter = dataTable3;
				}
			}
			else
			{
				FilterValueControl filterValueControl = FilterValueControl;
				filterValueControl.SourceFilter = await _httpClient.FormFilterSettingClient.GetDataFilterValue(Function.ParseInt(dataRow["DocEntry"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtFilterValue_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!(DataSource is DataTable dataTable))
			{
				return;
			}
			DataTable dataTable2 = dataTable.Copy();
			string filterField = Function.ToString(FilterFieldControl.EditValue);
			string text = Function.ToString(FilterValueControl.EditValue);
			if (!dataTable2.Columns.Contains(filterField))
			{
				return;
			}
			PivotGridField pivotGridField = Fields.Cast<PivotGridField>().FirstOrDefault((PivotGridField t) => t.FieldName.Equals(filterField));
			if (pivotGridField == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(text))
			{
				pivotGridField.FilterValues.FilterType = PivotFilterType.Excluded;
				pivotGridField.FilterValues.Clear();
				return;
			}
			pivotGridField.FilterValues.FilterType = PivotFilterType.Included;
			pivotGridField.FilterValues.Clear();
			Type dataType = dataTable2.Columns[filterField].DataType;
			if (dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long))
			{
				List<int> list = text.Split(';').Select(Function.ParseInt).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					pivotGridField.FilterValues.Add(list[num]);
				}
			}
			else if (dataType == typeof(double) || dataType == typeof(decimal) || dataType == typeof(float))
			{
				List<double> list2 = text.Split(';').Select(Function.ParseDouble).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					pivotGridField.FilterValues.Add(list2[num2]);
				}
			}
			else if (dataType == typeof(DateTime))
			{
				List<DateTime> list3 = text.Split(';').Select(Function.ParseDateTimes).ToList();
				for (int num3 = 0; num3 < list3.Count; num3++)
				{
					pivotGridField.FilterValues.Add(list3[num3]);
				}
			}
			else
			{
				List<string> list4 = text.Split(';').ToList();
				for (int num4 = 0; num4 < list4.Count; num4++)
				{
					pivotGridField.FilterValues.Add(list4[num4]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override void RaisePopupMenuShowing(PopupMenuShowingEventArgs e)
	{
		base.RaisePopupMenuShowing(e);
		try
		{
			LoadContextMenu();
			ReloadContextAction();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void LoadContextMenu()
	{
		ContextMenuStrip contextMenuStrip = ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = (ContextMenuStrip = new ContextMenuStrip());
		}
		bool flag = true;
		foreach (object item in contextMenuStrip.Items)
		{
			if (item is ToolStripMenuItem toolStripMenuItem && (toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYTABLE") || toolStripMenuItem.Text.ToUpper().Equals("COPY TABLE")))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ToolStripSeparator value = new ToolStripSeparator();
			contextMenuStrip.Items.Add(value);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
			toolStripMenuItem2.Text = "Copy Table";
			toolStripMenuItem2.Click += CtxCopytable_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem2);
		}
		flag = true;
		foreach (object item2 in contextMenuStrip.Items)
		{
			if (item2 is ToolStripMenuItem toolStripMenuItem3 && (toolStripMenuItem3.Name.ToUpper().Equals("CTM_COPYROWS") || toolStripMenuItem3.Text.ToUpper().Equals("COPY ROW(S)")))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
			toolStripMenuItem4.Text = "Copy Row(s)";
			toolStripMenuItem4.Click += CtxCopyrow_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem4);
		}
		foreach (object item3 in contextMenuStrip.Items)
		{
			if (item3 is ToolStripMenuItem toolStripMenuItem5 && toolStripMenuItem5.Name.ToUpper().Equals("CTM_CHANGEHORZSCROLLMODE"))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			contextMenuStrip.Items.Add(new ToolStripSeparator());
			ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
			toolStripMenuItem6.Name = "ctm_ChangeHorzScrollMode";
			toolStripMenuItem6.Text = "Change Horizontal Scroll Mode";
			toolStripMenuItem6.Click += CtxChangeHorzMode_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem6);
		}
		if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId))
		{
			return;
		}
		if (FindForm() is XtraFormEx { FormAuth: not Enums.FormAuthorities.Read, DisableContextAction: false })
		{
			flag = true;
			foreach (object item4 in contextMenuStrip.Items)
			{
				if (item4 is ToolStripMenuItem toolStripMenuItem7 && (toolStripMenuItem7.Name.ToUpper().Equals("CTM_ACTION") || toolStripMenuItem7.Text.ToUpper().Equals("CONTEXT ACTION")))
				{
					flag = false;
					break;
				}
			}
			if (flag && GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl r) => r.menucode == "barSub_ContextAction" && r.authoriza != "N") != null)
			{
				ToolStripSeparator value2 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value2);
				ToolStripMenuItem toolStripMenuItem8 = new ToolStripMenuItem();
				toolStripMenuItem8.Name = "CTM_ACTION";
				toolStripMenuItem8.Text = "Context Action";
				toolStripMenuItem8.Click += CtxAction_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem8);
			}
		}
		flag = true;
		foreach (object item5 in contextMenuStrip.Items)
		{
			if (item5 is ToolStripMenuItem toolStripMenuItem9 && (toolStripMenuItem9.Name.ToUpper().Equals("CTM_FORMATCONDITION") || toolStripMenuItem9.Text.ToUpper().Equals("FORMAT CONDITION")))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ToolStripSeparator value3 = new ToolStripSeparator();
			contextMenuStrip.Items.Add(value3);
			ToolStripMenuItem toolStripMenuItem10 = new ToolStripMenuItem();
			toolStripMenuItem10.Name = "CTM_FORMATCONDITION";
			toolStripMenuItem10.Text = "Format Condition";
			toolStripMenuItem10.Click += CtxFormatRule_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem10);
		}
	}

	private void CtxCopyrow_Click(object sender, EventArgs e)
	{
		Export(Enums.ExportExt.Clb);
	}

	private void CtxCopytable_Click(object sender, EventArgs e)
	{
		bool multiSelect = OptionsSelection.MultiSelect;
		OptionsSelection.MultiSelect = true;
		Cells.Selection = new Rectangle(0, 0, Cells.ColumnCount, Cells.RowCount);
		Export(Enums.ExportExt.Clb);
		OptionsSelection.MultiSelect = multiSelect;
	}

	private void CtxChangeHorzMode_Click(object sender, EventArgs e)
	{
		OptionsBehavior.HorizontalScrolling = ((OptionsBehavior.HorizontalScrolling == PivotGridScrolling.CellsArea) ? PivotGridScrolling.Control : PivotGridScrolling.CellsArea);
	}

	private void CtxFormatRule_Click(object sender, EventArgs e)
	{
		try
		{
			frm_FormatRule frm_FormatRule2 = new frm_FormatRule();
			frm_FormatRule2.InitPivot(this);
			if (FindForm() is XtraFormEx frmParent)
			{
				frm_FormatRule2.ShowDialog(frmParent);
			}
			else
			{
				frm_FormatRule2.ShowDialog();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CtxAction_Click(object sender, EventArgs e)
	{
		try
		{
			Type type = Type.GetType("Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms");
			if (type != null)
			{
				((XtraFormEx)Activator.CreateInstance(type, FormId, ItemId, DataSource, this)).ShowDialog();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RemoveDropDownAction(ToolStripMenuItem ctm)
	{
		for (int i = 0; i < ctm.DropDownItems.Count; i++)
		{
			if (ctm.DropDownItems[i] is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().Equals("CTA_"))
			{
				RemoveDropDownAction(toolStripMenuItem);
				ctm.DropDownItems.Remove(toolStripMenuItem);
				i--;
			}
		}
	}

	private void ReloadContextAction()
	{
		ContextMenuStrip contextMenuStrip = ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = (ContextMenuStrip = new ContextMenuStrip());
		}
		for (int i = 0; i < contextMenuStrip.Items.Count; i++)
		{
			if (contextMenuStrip.Items[i] is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().StartsWith("CTA_"))
			{
				RemoveDropDownAction(toolStripMenuItem);
				contextMenuStrip.Items.Remove(toolStripMenuItem);
				i--;
			}
		}
		_dsContextAction = _httpClient.ContextAction.GetData(FormId, ItemId);
		if (!_dsContextAction.IsNotNull())
		{
			return;
		}
		DataTable dataTable = _dsContextAction.Tables[0];
		foreach (DataRow item in from t in dataTable.AsEnumerable()
			where Function.ParseInt(t["FatherId"]) <= 0 && Function.ToString(t["Invisible"]) != "Y"
			select t)
		{
			int absEntry = Function.ParseInt(item["AbsEntry"]);
			string name = "CTA_" + absEntry;
			ToolStripMenuItem toolStripMenuItem2 = CheckExistsContext(contextMenuStrip, name);
			if (toolStripMenuItem2 != null)
			{
				contextMenuStrip.Items.Remove(toolStripMenuItem2);
			}
			toolStripMenuItem2 = new ToolStripMenuItem();
			toolStripMenuItem2.Name = name;
			toolStripMenuItem2.Text = Function.ToString(item["Caption"]);
			string text = Function.ToString(item["Icon"]);
			if (!string.IsNullOrEmpty(text))
			{
				toolStripMenuItem2.Image = ImageResourceCache.Default.GetImageById(text, ImageSize.Size16x16, ImageType.Office2013);
			}
			int num = Function.ParseInt(item["ActionTypeEx"]);
			if (num != 0)
			{
				toolStripMenuItem2.Click += (object s, EventArgs events) =>
				{
					try
					{
						Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms"), absEntry, FormId, ItemId, DataSource, this);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.ErrorLineNumber(ex);
					}
				};
			}
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			if (num != 7000038)
			{
				AddDropDown(absEntry, dataTable, ref toolStripMenuItem2);
			}
		}
	}

	private void AddDropDown(int fatherId, DataTable dt, ref ToolStripMenuItem ctm)
	{
		DataRow[] array = (from t in dt.AsEnumerable()
			where Function.ParseInt(t["FatherId"]) == fatherId && Function.ToString(t["Invisible"]) != "Y"
			select t).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		DataRow[] array2 = array;
		foreach (DataRow dataRow in array2)
		{
			int absEntry = Function.ParseInt(dataRow["AbsEntry"]);
			string name = "CTA_" + absEntry;
			ToolStripMenuItem toolStripMenuItem = CheckExistsContextDropDown(ctm, name);
			if (toolStripMenuItem != null)
			{
				ctm.DropDownItems.Remove(toolStripMenuItem);
			}
			toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Name = name;
			toolStripMenuItem.Text = Function.ToString(dataRow["Caption"]);
			string text = Function.ToString(dataRow["Icon"]);
			if (!string.IsNullOrEmpty(text))
			{
				toolStripMenuItem.Image = ImageResourceCache.Default.GetImageById(text, ImageSize.Size16x16, ImageType.Office2013);
			}
			if (Function.ParseInt(dataRow["ActionTypeEx"]) != 0)
			{
				toolStripMenuItem.Click += (object s, EventArgs events) =>
				{
					try
					{
						Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms"), absEntry, FormId, ItemId, DataSource, this);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.ErrorLineNumber(ex);
					}
				};
			}
			ctm.DropDownItems.Add(toolStripMenuItem);
			AddDropDown(absEntry, dt, ref toolStripMenuItem);
		}
	}

	private ToolStripMenuItem CheckExistsContextDropDown(ToolStripMenuItem ctx, string name)
	{
		foreach (object dropDownItem in ctx.DropDownItems)
		{
			if (dropDownItem is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().Equals(name))
			{
				return toolStripMenuItem;
			}
		}
		return null;
	}

	private ToolStripMenuItem CheckExistsContext(ContextMenuStrip ctx, string name)
	{
		foreach (object item in ctx.Items)
		{
			if (item is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().Equals(name))
			{
				return toolStripMenuItem;
			}
		}
		return null;
	}

	public void Export(Enums.ExportExt extention)
	{
		string text = "";
		switch (extention)
		{
		case Enums.ExportExt.Xls:
		{
			PivotXlsExportOptions pivotXlsExportOptions = new PivotXlsExportOptions(TextExportMode.Value);
			pivotXlsExportOptions.ExportType = ExportType.WYSIWYG;
			text = ExtensionMethods.PathSaveFile("Excel (2003)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXls(text, pivotXlsExportOptions);
			}
			break;
		}
		case Enums.ExportExt.Xlsx:
		{
			PivotXlsxExportOptions pivotXlsxExportOptions = new PivotXlsxExportOptions(TextExportMode.Value);
			pivotXlsxExportOptions.ExportType = ExportType.WYSIWYG;
			text = ExtensionMethods.PathSaveFile("Excel (2007)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXlsx(text, pivotXlsxExportOptions);
			}
			break;
		}
		case Enums.ExportExt.Xlsx2:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", Enums.ExportExt.Xlsx);
			if (!string.IsNullOrEmpty(text))
			{
				using MemoryStream memoryStream3 = new MemoryStream();
				ExportToXlsx(memoryStream3);
				FileStream fileStream3 = new FileStream(text, FileMode.Create, FileAccess.Write);
				memoryStream3.WriteTo(fileStream3);
				fileStream3.Close();
			}
			break;
		case Enums.ExportExt.Xlsx3:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", Enums.ExportExt.Xlsx);
			if (!string.IsNullOrWhiteSpace(text))
			{
				using MemoryStream memoryStream2 = new MemoryStream();
				ExportToXlsx(memoryStream2, new XlsxExportOptionsEx
				{
					ExportType = ExportType.DataAware,
					TextExportMode = TextExportMode.Text
				});
				FileStream fileStream2 = new FileStream(text, FileMode.Create, FileAccess.Write);
				memoryStream2.WriteTo(fileStream2);
				fileStream2.Close();
			}
			break;
		case Enums.ExportExt.Xlsx4:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", Enums.ExportExt.Xlsx);
			if (!string.IsNullOrWhiteSpace(text))
			{
				using MemoryStream memoryStream = new MemoryStream();
				ExportToXlsx(memoryStream, new XlsxExportOptionsEx
				{
					ExportType = ExportType.WYSIWYG,
					TextExportMode = TextExportMode.Text
				});
				FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write);
				memoryStream.WriteTo(fileStream);
				fileStream.Close();
			}
			break;
		case Enums.ExportExt.Txt:
		{
			TextExportOptions textExportOptions = new TextExportOptions();
			textExportOptions.TextExportMode = TextExportMode.Text;
			text = ExtensionMethods.PathSaveFile("Text", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToText(text, textExportOptions);
			}
			break;
		}
		case Enums.ExportExt.Rtf:
			text = ExtensionMethods.PathSaveFile("RTF", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToRtf(text);
			}
			break;
		case Enums.ExportExt.Pdf:
			text = ExtensionMethods.PathSaveFile("PDF", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToPdf(text);
			}
			break;
		case Enums.ExportExt.Html:
			text = ExtensionMethods.PathSaveFile("HTML", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToHtml(text);
			}
			break;
		case Enums.ExportExt.Csv:
			text = ExtensionMethods.PathSaveFile("CSV", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToCsv(text);
			}
			break;
		case Enums.ExportExt.Clb:
			Cells.CopySelectionToClipboard();
			break;
		}
		if (!string.IsNullOrEmpty(text) && FileIO.FileExist(text))
		{
			try
			{
				Process.Start(text);
			}
			catch
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.CanNotOpenFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, text);
			}
		}
	}

	public void Export()
	{
		ExportSettings.DefaultExportType = ExportType.WYSIWYG;
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html |Text File (.txt)|*.txt |Csv File (.csv)|*.csv";
		if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
		{
			return;
		}
		string fileName = saveFileDialog.FileName;
		switch (new FileInfo(fileName).Extension)
		{
		case ".xls":
		{
			PivotXlsExportOptions pivotXlsExportOptions = new PivotXlsExportOptions(TextExportMode.Value);
			pivotXlsExportOptions.ExportType = ExportType.WYSIWYG;
			ExportToXls(fileName, pivotXlsExportOptions);
			break;
		}
		case ".xlsx":
		{
			PivotXlsxExportOptions pivotXlsxExportOptions = new PivotXlsxExportOptions(TextExportMode.Value);
			pivotXlsxExportOptions.ExportType = ExportType.WYSIWYG;
			ExportToXlsx(fileName, pivotXlsxExportOptions);
			break;
		}
		case ".rtf":
			ExportToRtf(fileName);
			break;
		case ".pdf":
			ExportToPdf(fileName);
			break;
		case ".txt":
		{
			TextExportOptions textExportOptions = new TextExportOptions();
			textExportOptions.TextExportMode = TextExportMode.Text;
			ExportToText(fileName, textExportOptions);
			break;
		}
		case ".html":
			ExportToHtml(fileName);
			break;
		case ".csv":
			ExportToCsv(fileName);
			break;
		default:
			ExportToXls(fileName);
			break;
		}
		if (FileIO.FileExist(fileName))
		{
			try
			{
				Process.Start(fileName);
				return;
			}
			catch
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.CanNotOpenFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, fileName);
				return;
			}
		}
		XtraMessageBoxEx.Show(Enums.MessageCode.CanNotSaveFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, fileName);
	}

	protected override void RaiseEditorKeyDown(KeyEventArgs e)
	{
		if (AllowPasteExcel && e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
		{
			this.ExcelToDataSource();
			e.Handled = true;
		}
		else
		{
			base.RaiseEditorKeyDown(e);
		}
	}
}
