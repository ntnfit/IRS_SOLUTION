using System.Data;
using System.Drawing;
using Apzon.Commons;

namespace Apzon.Entities.Models.DMS;

public class MapMarker
{
	public string Code { get; set; }

	public string Name { get; set; }

	public string SvgData { get; set; }

	public string DfltColorHex { get; set; }

	public int DfltSize { get; set; }

	public Image ImageData { get; set; }

	public MapMarker()
	{
	}

	public MapMarker(DataRow t)
	{
		Code = Function.ToString(t["Code"]);
		Name = Function.ToString(t["Name"]);
		SvgData = Function.ToString(t["SvgData"]);
		DfltColorHex = Function.ToString(t["DfltColorHex"]);
		DfltSize = Function.ParseInt(t["DfltSize"]);
		DfltSize = ((DfltSize <= 0) ? 16 : DfltSize);
		ImageData = Function.ConvertSVGtoPNG(SvgData, DfltSize, DfltSize);
	}
}
