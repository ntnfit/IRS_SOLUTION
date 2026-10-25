using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Images;
using DevExpress.Utils.Design;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPrinting;
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Events;
using DevExpress.XtraVerticalGrid.Rows;

namespace Apzon.Controls.Grid;

public class VGridControlEx : VGridControl
{
	private DataSet _dsContextAction = new DataSet();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public string FormId { get; set; }

	public string ItemId { get; set; }

	public ApzObjectTypes ObjectType { get; set; }

	public VGridControlEx()
	{
		InitComponents();
	}

	public void InitComponents()
	{
		OptionsMenu.EnableContextMenu = true;
		PopupMenuShowing += VGridControlEx_PopupMenuShowing;
		Appearance.FocusedRow.BackColor = StyleColor.GridEditting;
	}

	protected override void OnLoaded()
	{
		base.OnLoaded();
	}

	protected override void RaiseShowMenu(DevExpress.XtraVerticalGrid.Events.PopupMenuShowingEventArgs e)
	{
		base.RaiseShowMenu(e);
		try
		{
			CreateContextMenu();
			ReloadContextAction();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected override void RaiseShowMenuCore(DevExpress.XtraVerticalGrid.Events.PopupMenuShowingEventArgs e)
	{
		base.RaiseShowMenuCore(e);
		try
		{
			CreateContextMenu();
			ReloadContextAction();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void VGridControlEx_PopupMenuShowing(object sender, DevExpress.XtraVerticalGrid.Events.PopupMenuShowingEventArgs e)
	{
		try
		{
			CreateContextMenu();
			ReloadContextAction();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void CreateContextMenu()
	{
		ContextMenuStrip contextMenuStrip = ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = (ContextMenuStrip = new ContextMenuStrip());
		}
		foreach (object item in contextMenuStrip.Items)
		{
			if (item is ToolStripMenuItem toolStripMenuItem && (toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYTABLE") || toolStripMenuItem.Text.ToUpper().Equals("COPY TABLE")))
			{
				return;
			}
		}
		ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
		toolStripMenuItem2.Text = "Copy Table";
		toolStripMenuItem2.Click += CtxCopytable_Click;
		contextMenuStrip.Items.Add(toolStripMenuItem2);
		foreach (object item2 in contextMenuStrip.Items)
		{
			if (item2 is ToolStripMenuItem toolStripMenuItem3 && (toolStripMenuItem3.Name.ToUpper().Equals("CTM_COPYROWS") || toolStripMenuItem3.Text.ToUpper().Equals("COPY ROW(S)")))
			{
				return;
			}
		}
		toolStripMenuItem2 = new ToolStripMenuItem();
		toolStripMenuItem2.Text = "Copy Row(s)";
		toolStripMenuItem2.Click += CtxCopyrow_Click;
		contextMenuStrip.Items.Add(toolStripMenuItem2);
		if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId))
		{
			return;
		}
		foreach (object item3 in contextMenuStrip.Items)
		{
			if (item3 is ToolStripMenuItem toolStripMenuItem4 && (toolStripMenuItem4.Name.ToUpper().Equals("CTM_ACTION") || toolStripMenuItem4.Text.ToUpper().Equals("CONTEXT ACTION")))
			{
				return;
			}
		}
		if (GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl r) => r.menucode == "barSub_ContextAction" && r.authoriza != "N") != null)
		{
			ToolStripSeparator value = new ToolStripSeparator();
			contextMenuStrip.Items.Add(value);
			toolStripMenuItem2 = new ToolStripMenuItem();
			toolStripMenuItem2.Name = "CTM_ACTION";
			toolStripMenuItem2.Text = "Context Action";
			toolStripMenuItem2.Click += CtxAction_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem2);
		}
	}

	private void CtxAction_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			Type type = Type.GetType("Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms");
			if (type != null)
			{
				((XtraFormEx)Activator.CreateInstance(type, FormId, ItemId, DataSource, this)).ShowDialog();
			}
			ReloadContextAction();
			WinCommons._CloseProcessing();
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

	private void CtxCopyrow_Click(object sender, EventArgs e)
	{
		Export(Enums.ExportExt.Clb);
	}

	private void CtxCopytable_Click(object sender, EventArgs e)
	{
		bool multiSelect = OptionsSelectionAndFocus.MultiSelect;
		OptionsSelectionAndFocus.MultiSelect = true;
		foreach (EditorRow row in Rows)
		{
			SelectRow(row);
		}
		Export(Enums.ExportExt.Clb);
		OptionsSelectionAndFocus.MultiSelect = multiSelect;
	}

	public void Export(Enums.ExportExt extention)
	{
		string text = "";
		switch (extention)
		{
		case Enums.ExportExt.Xls:
		{
			PivotXlsExportOptions pivotXlsExportOptions = new PivotXlsExportOptions(TextExportMode.Text);
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
			PivotXlsxExportOptions pivotXlsxExportOptions = new PivotXlsxExportOptions(TextExportMode.Text);
			pivotXlsxExportOptions.ExportType = ExportType.WYSIWYG;
			text = ExtensionMethods.PathSaveFile("Excel (2007)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXlsx(text, pivotXlsxExportOptions);
			}
			break;
		}
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
		case Enums.ExportExt.Clb:
			CopyToClipboard();
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
			PivotXlsExportOptions pivotXlsExportOptions = new PivotXlsExportOptions(TextExportMode.Text);
			pivotXlsExportOptions.ExportType = ExportType.WYSIWYG;
			ExportToXls(fileName, pivotXlsExportOptions);
			break;
		}
		case ".xlsx":
		{
			PivotXlsxExportOptions pivotXlsxExportOptions = new PivotXlsxExportOptions(TextExportMode.Text);
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
}
