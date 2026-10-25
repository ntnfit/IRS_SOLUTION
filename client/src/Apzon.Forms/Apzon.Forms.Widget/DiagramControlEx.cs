using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using DevExpress.Diagram.Core;
using DevExpress.Utils;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Widget;

public class DiagramControlEx : DiagramControl
{
	private List<DiagramImage> _shapeResize = new List<DiagramImage>();

	public bool IsSearchControl { get; set; }

	public DiagramControlEx()
	{
		try
		{
			OptionsView.AllowShapeShadows = true;
			OptionsView.CanvasSizeMode = CanvasSizeMode.Fill;
			OptionsView.PageMargin = new Padding(0);
			OptionsView.PaperKind = PaperKind.Letter;
			OptionsView.ScrollMargin = new Padding(2000000000);
			OptionsView.ShowGrid = false;
			OptionsView.ShowPageBreaks = false;
			OptionsView.ShowRulers = false;
			CustomGetEditableItemProperties += DiagramControlEx_CustomGetEditableItemProperties;
			ToolTipController toolTipController = new ToolTipController();
			toolTipController.GetActiveObjectInfo += toolTipController1_GetActiveObjectInfo;
			ToolTipController = toolTipController;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetLastShape()
	{
		foreach (DiagramImage item in _shapeResize)
		{
			if (item != null)
			{
				item.Width -= 10f;
				item.Height -= 10f;
			}
		}
		_shapeResize = new List<DiagramImage>();
	}

	private void toolTipController1_GetActiveObjectInfo(object sender, ToolTipControllerGetActiveObjectInfoEventArgs e)
	{
		try
		{
			if (e.SelectedControl != this)
			{
				return;
			}
			DiagramItem diagramItem = CalcHitItem(e.ControlMousePosition);
			if (diagramItem == null)
			{
				ResetLastShape();
				return;
			}
			Cursor = Cursors.Hand;
			if (diagramItem is DiagramImage diagramImage && !_shapeResize.Contains(diagramImage))
			{
				_shapeResize.Add(diagramImage);
				diagramImage.Width += 10f;
				diagramImage.Height += 10f;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DiagramControlEx_CustomGetEditableItemProperties(object sender, DiagramCustomGetEditableItemPropertiesEventArgs e)
	{
		try
		{
			if (e.Item is DiagramImageEx)
			{
				e.Properties.Add(TypeDescriptor.GetProperties(typeof(DiagramImageEx))["ObjectGroup"]);
				e.Properties.Add(TypeDescriptor.GetProperties(typeof(DiagramImageEx))["ObjType"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override DiagramImage CreateImage()
	{
		return base.CreateImage() as DiagramImageEx;
	}
}
