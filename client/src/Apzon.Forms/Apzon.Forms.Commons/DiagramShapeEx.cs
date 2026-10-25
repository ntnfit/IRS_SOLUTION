using System;
using System.Drawing;
using System.Linq;
using Apzon.Commons;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Global;
using DevExpress.Utils;
using DevExpress.XtraDiagram;

namespace Apzon.Forms.Commons;

public sealed class DiagramShapeEx : DiagramShape
{
	public string Id { get; set; }

	public string ObjectType { get; set; }

	public string ObjectId { get; set; }

	public string ObjectNum { get; set; }

	public string Type { get; set; }

	public string ObjectCode { get; set; }

	public string Status { get; set; }

	public DateTime DocDate { get; set; }

	public string FatherId { get; set; }

	public string RelatedType { get; set; }

	public int RelatedId { get; set; }

	public bool Root { get; set; }

	public double DocTotal { get; set; }

	public string DocCur { get; set; }

	public string IsIns { get; set; }

	public ApzObjectTypes RootType { get; set; }

	public DiagramShapeEx()
	{
	}

	public DiagramShapeEx(string id, string objectType, string objectId, string objectNum, string type, string objectCode, string status, DateTime docDate, string fatherId, string relatedType, int relatedId, bool root, double doctotal, string doccur, string isIns, ApzObjectTypes roottype)
	{
		Id = id;
		if (objectType == 13.ToString())
		{
			ObjectType = ((isIns == "Y") ? 13000.ToString() : 13.ToString());
		}
		else if (objectType == 18.ToString())
		{
			ObjectType = ((isIns == "Y") ? 25.ToString() : 18.ToString());
		}
		else
		{
			ObjectType = objectType;
		}
		ObjectId = objectId;
		ObjectNum = objectNum;
		Type = type;
		ObjectCode = objectCode;
		Status = status;
		DocDate = docDate;
		FatherId = fatherId;
		RelatedType = relatedType;
		RelatedId = relatedId;
		Root = root;
		DocTotal = doctotal;
		DocCur = doccur;
		IsIns = isIns;
		RootType = roottype;
		Appearance.BackColor = ((status == "C") ? Color.Pink : ((status == "L") ? Color.Gray : Color.Wheat));
		Appearance.BorderColor = (root ? Color.DarkOrange : ((status == "C") ? Color.Pink : ((status == "L") ? Color.DarkGray : Color.Cornsilk)));
		if (root)
		{
			Appearance.BorderSize += 2;
		}
		Appearance.ForeColor = ((status == "L" && !root) ? Color.White : Color.Black);
		Appearance.TextOptions.HAlignment = HorzAlignment.Near;
		Appearance.TextOptions.VAlignment = VertAlignment.Center;
		int num = -1;
		if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
		{
			CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(DocCur.ToUpper()));
			if (currencyCodesImpl != null)
			{
				num = currencyCodesImpl.Decimals;
			}
		}
		Content = string.Format("   {0}\r\n   No. {2}\r\n   {1}\r\n   {3}\r\n   {4}\r\n   {5}\r\n   {6} {7}", frm_Diagram.GetObjectString(ObjectType, RootType, isIns), frm_Diagram.GetStatusString(Status), ObjectNum, frm_Diagram.GetTypeString(Type), ObjectCode, (DocDate == new DateTime(1900, 1, 1)) ? "" : DocDate.ToString(GlobalData.DateTimeFormat), DocCur, DocTotal.Equals(0.0) ? "" : DocTotal.ToString("n" + ((num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num)));
		Size = new SizeF(150f, 120f);
		Appearance.Font = AppearanceObject.DefaultFont;
		CanCopy = false;
		CanDelete = false;
		CanResize = false;
		CanRotate = false;
	}

	public DiagramShapeEx(string objecttype, string code, string name, bool root)
	{
		ObjectType = objecttype;
		ObjectCode = name;
		ObjectNum = code;
		Id = code;
		Appearance.BackColor = (root ? Color.White : Color.Wheat);
		Appearance.BorderColor = (root ? Color.DarkOrange : Color.Cornsilk);
		Appearance.ForeColor = Color.Black;
		Appearance.TextOptions.HAlignment = HorzAlignment.Center;
		Appearance.TextOptions.VAlignment = VertAlignment.Center;
		Content = $"{code}\r\n\r\n{name}";
		Size = new SizeF(200f, 85f);
		Appearance.Font = AppearanceObject.DefaultFont;
		CanCopy = false;
		CanDelete = false;
		CanResize = false;
		CanRotate = false;
	}
}
