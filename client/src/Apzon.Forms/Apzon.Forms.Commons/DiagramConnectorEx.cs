using System.Drawing;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Commons;

public sealed class DiagramConnectorEx : DiagramConnector
{
	public string Begin { get; set; }

	public string End { get; set; }

	public DiagramConnectorEx(DiagramShapeEx begin, DiagramShapeEx end, int beginpoint, int endpoit, ConnectorType type)
	{
		Begin = begin.Id;
		End = end.Id;
		Appearance.ContentBackground = Color.White;
		BeginItem = begin;
		BeginItemPointIndex = beginpoint;
		EndItem = end;
		EndItemPointIndex = endpoit;
		Type = type;
		CanCopy = false;
		CanDelete = false;
		CanMove = false;
		CanRotate = false;
		CanChangeRoute = false;
		CanDragBeginPoint = false;
		CanDragEndPoint = false;
		Appearance.BorderSize += 2;
	}
}
