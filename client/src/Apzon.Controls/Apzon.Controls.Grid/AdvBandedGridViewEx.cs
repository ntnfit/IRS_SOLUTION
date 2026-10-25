using System;
using System.Diagnostics;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Form;
using DevExpress.Export;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace Apzon.Controls.Grid;

public class AdvBandedGridViewEx : AdvBandedGridView
{
	public string FormId { get; set; }

	public string ItemId { get; set; }

	public Enums.ChangeLog ChangeLogType { get; set; }

	public bool IsApzTable { get; set; }

	public AdvBandedGridViewEx()
	{
		InitComponents();
	}

	public AdvBandedGridViewEx(GridControlEx grid)
		: base(grid)
	{
		InitComponents();
	}

	public void InitComponents()
	{
		ChangeLogType = Enums.ChangeLog.clNone;
		PopupMenuShowing += AdvBandedGridViewEx_PopupMenuShowing;
	}

	private void AdvBandedGridViewEx_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			ContextMenuStrip contextMenuStrip = GridControl.ContextMenuStrip;
			if (contextMenuStrip == null)
			{
				contextMenuStrip = new ContextMenuStrip();
				GridControl.ContextMenuStrip = contextMenuStrip;
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
			if (ChangeLogType == Enums.ChangeLog.clNone)
			{
				return;
			}
			foreach (object item3 in contextMenuStrip.Items)
			{
				if (item3 is ToolStripMenuItem toolStripMenuItem5 && (toolStripMenuItem5.Name.ToUpper().Equals("CTM_CHANGELOG") || toolStripMenuItem5.Text.ToUpper().Equals("CHANGE LOG")))
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				ToolStripSeparator value2 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value2);
				ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
				toolStripMenuItem6.Name = "CTM_CHANGELOG";
				toolStripMenuItem6.Text = "Change Log";
				toolStripMenuItem6.Click += ChangeLogItemOnClick;
				contextMenuStrip.Items.Add(toolStripMenuItem6);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected void CtxCopytable_Click(object sender, EventArgs e)
	{
		bool multiSelect = OptionsSelection.MultiSelect;
		OptionsSelection.MultiSelect = true;
		OptionsClipboard.ClipboardMode = ClipboardMode.Default;
		SelectAll();
		Export(Enums.ExportExt.Clb);
		OptionsSelection.MultiSelect = multiSelect;
	}

	public void Export(Enums.ExportExt extention)
	{
		string text = "";
		XlsExportOptionsEx options = new XlsExportOptionsEx
		{
			ExportType = ExportType.WYSIWYG,
			TextExportMode = TextExportMode.Text
		};
		XlsxExportOptionsEx options2 = new XlsxExportOptionsEx
		{
			ExportType = ExportType.WYSIWYG,
			TextExportMode = TextExportMode.Text
		};
		switch (extention)
		{
		case Enums.ExportExt.Xls:
			text = ExtensionMethods.PathSaveFile("Excel (2003)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXls(text, options);
			}
			break;
		case Enums.ExportExt.Xlsx:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXlsx(text, options2);
			}
			break;
		case Enums.ExportExt.Txt:
			text = ExtensionMethods.PathSaveFile("Text", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToText(text);
			}
			break;
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

	protected void CtxCopyrow_Click(object sender, EventArgs e)
	{
		Export(Enums.ExportExt.Clb);
	}

	private void ChangeLogItemOnClick(object sender, EventArgs e)
	{
		if (GridControl.FindForm() is XtraFormEx xtraFormEx)
		{
			xtraFormEx.CHANGELOG();
		}
	}
}
