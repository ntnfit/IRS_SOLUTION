using Apzon.Commons;
using DevExpress.Utils.Serializing;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Widget;

public sealed class DiagramImageEx : DiagramImage
{
	public bool IsSearchControl { get; set; }

	[XtraSerializableProperty]
	public string ObjectGroup { get; set; }

	[XtraSerializableProperty]
	public ApzObjectTypes ObjType { get; set; }

	public DiagramImageEx()
	{
		DiagramControl.ItemTypeRegistrator.Register(typeof(DiagramImageEx));
	}
}
