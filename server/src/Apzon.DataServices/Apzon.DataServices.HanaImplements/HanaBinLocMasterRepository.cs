using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaBinLocMasterRepository : BinLocMasterRepository
{
	public HanaBinLocMasterRepository(Apzon.Services.IUnitOfWork servicesUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(servicesUnitOfWork, unitOfWork, dataService)
	{
	}

	public override string Save(string sessionId, DataTable doc, string action)
	{
		try
		{
			EnumCommandType enumCommandType = ((action == "A") ? EnumCommandType.Add : EnumCommandType.Update);
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				BodyObject = Mapping(sessionId, doc.Rows[0], enumCommandType),
				Action = ((enumCommandType == EnumCommandType.Add) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
				ObjectType = ApzObjectTypes.oBinLocation,
				Key = Function.ParseInt(doc.Rows[0]["AbsEntry"]),
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Delete(string sessionId, int absEntry)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Delete,
				ObjectType = ApzObjectTypes.oBinLocation,
				Key = absEntry,
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Mapping(string sessionId, DataRow row, EnumCommandType type)
	{
		try
		{
			string text = "{";
			if (type == EnumCommandType.Update)
			{
				text += Function.GetJsonString("AbsEntry", Function.ParseInt(row["AbsEntry"]));
			}
			text += Function.GetJsonString("AlternativeSortCode", Function.RawText(row["AltSortCod"]));
			text += Function.GetJsonString("Attribute1", Function.ToString(row["Attr1Val"]));
			text += Function.GetJsonString("Attribute2", Function.ToString(row["Attr2Val"]));
			text += Function.GetJsonString("Attribute3", Function.ToString(row["Attr3Val"]));
			text += Function.GetJsonString("Attribute4", Function.ToString(row["Attr4Val"]));
			text += Function.GetJsonString("Attribute5", Function.ToString(row["Attr5Val"]));
			text += Function.GetJsonString("Attribute6", Function.ToString(row["Attr6Val"]));
			text += Function.GetJsonString("Attribute7", Function.ToString(row["Attr7Val"]));
			text += Function.GetJsonString("Attribute8", Function.ToString(row["Attr8Val"]));
			text += Function.GetJsonString("Attribute9", Function.ToString(row["Attr9Val"]));
			text += Function.GetJsonString("Attribute10", Function.RawText(row["Attr10Val"]));
			text += Function.GetJsonString("BarCode", Function.ToString(row["BarCode"]));
			text += Function.GetJsonString("BinCode", Function.ToString(row["BinCode"]));
			if (!string.IsNullOrEmpty(Function.ToString(row["RtrictDate"])))
			{
				text += Function.GetJsonString("DateRestrictionChanged", Convert.ToDateTime(row["RtrictDate"]).ToString("yyyyMMdd"));
			}
			text += Function.GetJsonString("Description", Function.ToString(row["Descr"]));
			text += Function.GetJsonString("ExcludeAutoAllocOnIssue", Function.ToString(row["NoAutoAllc"]).Equals("Y") ? "tYES" : "tNO");
			text += Function.GetJsonString("Inactive", Function.ToString(row["Disabled"]).Equals("Y") ? "tYES" : "tNO");
			text += Function.GetJsonString("IsSystemBin", Function.ToString(row["SysBin"]).Equals("Y") ? "tYES" : "tNO");
			text += Function.GetJsonString("MaximumQty", Function.ParseDecimal(row["MaxLevel"]));
			text += Function.GetJsonString("MaximumWeight", Function.ParseDecimal(row["MaxWeight1"]));
			text += Function.GetJsonString("MaximumWeight1", Function.ParseDecimal(row["MaxWeight2"]));
			text += Function.GetJsonString("MaximumWeightUnit", Function.ParseInt(row["Wght1Unit"]));
			text += Function.GetJsonString("MaximumWeightUnit1", Function.ParseInt(row["Wght2Unit"]));
			text += Function.GetJsonString("MinimumQty", Function.ParseDecimal(row["MinLevel"]));
			text += Function.GetJsonString("ReceivingBinLocation", Function.ToString(row["ReceiveBin"]).Equals("Y") ? "tYES" : "tNO");
			switch (Function.ParseInt(row["ItmRtrictT"]))
			{
			case 0:
				text += Function.GetJsonString("RestrictedItemType", "briNone");
				break;
			case 1:
				text += Function.GetJsonString("RestrictedItemType", "briSpecificItem");
				break;
			case 2:
				text += Function.GetJsonString("RestrictedItemType", "briSingleItemOnly");
				break;
			case 3:
				text += Function.GetJsonString("RestrictedItemType", "briSpecificItemGroup");
				break;
			case 4:
				text += Function.GetJsonString("RestrictedItemType", "briSpecificItemGroupOnly");
				break;
			}
			switch (Function.ParseInt(row["RtrictType"]))
			{
			case 0:
				text += Function.GetJsonString("RestrictedTransType", "brtNoRestrictions");
				break;
			case 1:
				text += Function.GetJsonString("RestrictedTransType", "brtAllTrans");
				break;
			case 2:
				text += Function.GetJsonString("RestrictedTransType", "brtInboundTrans");
				break;
			case 3:
				text += Function.GetJsonString("RestrictedTransType", "brtOutboundTrans");
				break;
			case 4:
				text += Function.GetJsonString("RestrictedTransType", "brtAllExceptInventoryTrans");
				break;
			}
			switch (Function.ParseInt(row["UoMRtrict"]))
			{
			case 0:
				text += Function.GetJsonString("RestrictedUoMType", "bruNone");
				break;
			case 1:
				text += Function.GetJsonString("RestrictedUoMType", "bruSpecificUoM");
				break;
			case 2:
				text += Function.GetJsonString("RestrictedUoMType", "bruSingleUoMOnly");
				break;
			case 3:
				text += Function.GetJsonString("RestrictedUoMType", "bruSpecificUoMGroup");
				break;
			case 4:
				text += Function.GetJsonString("RestrictedUoMType", "bruSpecificUoMGroupOnly");
				break;
			}
			text += Function.GetJsonString("RestrictionReason", Function.ToString(row["RtrictResn"]));
			text += Function.GetJsonString("SpecificItem", Function.ToString(row["SpcItmCode"]));
			text += Function.GetJsonString("SpecificUoM", Function.ToString(row["SpcUoMCode"]));
			text += Function.GetJsonString("SpecificItemGroup", Function.ToString(row["SpcItmGrpC"]));
			text += Function.GetJsonString("BatchRestrictions", Function.ToString(row["SngBatch"]).Equals("Y") ? "brbSingleBatch" : "brbNoRestrictions");
			text += Function.GetJsonString("SpecificUoMGroup", Function.ToString(row["SpcUgpCode"]));
			text += Function.GetJsonString("Sublevel1", Function.ToString(row["SL1Code"]));
			text += Function.GetJsonString("Sublevel2", Function.ToString(row["SL2Code"]));
			text += Function.GetJsonString("Sublevel3", Function.ToString(row["SL3Code"]));
			text += Function.GetJsonString("Sublevel4", Function.ToString(row["SL4Code"]));
			text += Function.GetJsonString("Warehouse", Function.ToString(row["WhsCode"]));
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}
}
