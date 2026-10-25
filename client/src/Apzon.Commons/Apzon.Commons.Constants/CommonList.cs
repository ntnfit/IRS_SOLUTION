using System.Collections.Generic;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;

namespace Apzon.Commons.Constants;

public static class CommonList
{
	public static Dictionary<string, string> EffectiveDiscount = new Dictionary<string, string>
	{
		{ "L", "Lowest Discount" },
		{ "H", "Highest Discount" },
		{ "A", "Average" },
		{ "S", "Total" },
		{ "M", "Discount Multiples" }
	};

	public static Dictionary<string, string> BslineDate = new Dictionary<string, string>
	{
		{ "P", "Posting Date" },
		{ "S", "System Date" },
		{ "T", "Document Date" },
		{ "C", "Closing Date" }
	};

	public static Dictionary<string, string> OpenRcpt = new Dictionary<string, string>
	{
		{ "N", "No" },
		{ "3", "Cash" },
		{ "1", "Checks" },
		{ "4", "Credit" },
		{ "2", "Bank Transfer" },
		{ "5", "Bill of Exchange" }
	};

	public static Dictionary<string, string> DataSource = new Dictionary<string, string>
	{
		{ "N", "Unknown" },
		{ "I", "Interface" },
		{ "U", "Update" },
		{ "M", "Import" },
		{ "O", "DI API" },
		{ "A", "Doc. Generation Wizard" },
		{ "D", "Restore Wizard" },
		{ "P", "Partner Implementation" },
		{ "T", "Year Transfer" }
	};

	public static Dictionary<string, string> BoolValueRep = new Dictionary<string, string>
	{
		{ "Y", "Yes" },
		{ "N", "No" }
	};

	public static Dictionary<string, string> PayDuMonth = new Dictionary<string, string>
	{
		{ "E", "Month End" },
		{ "H", "Half Month" },
		{ "Y", "Month Start" },
		{ "N", " " }
	};

	public static Dictionary<string, string> CrdMthd = new Dictionary<string, string>
	{
		{ "F", "First Installment" },
		{ "L", "Last Installment" },
		{ "E", "Equally" }
	};

	public static Dictionary<string, string> Types = new Dictionary<string, string>
	{
		{ "A", "Alphanumeric" },
		{ "N", "Numeric" },
		{ "D", "Date/Time" },
		{ "B", "Units and Totals" },
		{ "M", "General" }
	};

	public static Dictionary<string, string> EditTypesAlphaNumeric = new Dictionary<string, string>
	{
		{ "", "Regular" },
		{ "?", "Address" },
		{ "#", "Telephone No." },
		{ "B", "Text" }
	};

	public static Dictionary<string, string> EditTypesDateTime = new Dictionary<string, string>
	{
		{ "", "Date" },
		{ "T", "Hour" }
	};

	public static Dictionary<string, string> EditTypesUnitandTotal = new Dictionary<string, string>
	{
		{ "R", "Rate" },
		{ "S", "Amount" },
		{ "P", "Price" },
		{ "Q", "Quantity" },
		{ "%", "Percents" },
		{ "M", "Measure" }
	};

	public static Dictionary<string, string> EditTypesGeneral = new Dictionary<string, string>
	{
		{ "B", "Link" },
		{ "I", "Image" }
	};

	public static Dictionary<int, string> UdtObjectTypes = new Dictionary<int, string>
	{
		{ 0, "No Object" },
		{ 1, "Master Data" },
		{ 2, "Master Data Rows" },
		{ 3, "Document" },
		{ 4, "Document Rows" },
		{ 5, "No Object with Auto. Increment" }
	};

	public static Dictionary<string, string> ReportCategory = new Dictionary<string, string>
	{
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Administration.ToString(),
			MenuCaption.Administration
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Sales.ToString(),
			MenuCaption.SalesAR
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Purchase.ToString(),
			MenuCaption.PurchaseAP
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.BusinessPartner.ToString(),
			MenuCaption.BP
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Inventory.ToString(),
			MenuCaption.Inventory
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Production.ToString(),
			MenuCaption.Production
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Mrp.ToString(),
			MenuCaption.MRP
		},
		{
			Apzon.Commons.Enums.Enums.ReportCategory.Hrm.ToString(),
			MenuCaption.HRM
		}
	};

	public static Dictionary<ApzObjectTypes, List<LogInstance>> ListLogInstance = new Dictionary<ApzObjectTypes, List<LogInstance>>
	{
		{
			ApzObjectTypes.oProjectManager,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOPRG",
					TableName = "APZ_OPRG",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPRG",
					UpdateDateKey = "UpdateDate",
					CreateDateKey = "CreateDate",
					UpdateTimeKey = "UpdateTime",
					CreateTimeKey = "CreateTime",
					UserSignCodeKey = "UserCodeSign",
					UserSignCode2Key = "UserCodeSign2",
					NotShowLogKey = new string[1] { "" }
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APRG1",
					TableName = "APZ_PRG1",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPRG",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APRG2",
					TableName = "APZ_PRG2",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPRG",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APRG3",
					TableName = "APZ_PRG3",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPRG",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APRG4",
					TableName = "APZ_PRG4",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPRG",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APRG6",
					TableName = "APZ_PRG6",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPRG",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oWBSTemplate,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOWBS",
					TableName = "APZ_OWBS",
					Key = "Code",
					LogInstanceKey = "LogInst",
					TableLogInstanceKey = "APZ_AOWBS",
					LineKey = "",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AWBS1",
					TableName = "APZ_WBS1",
					Key = "Code",
					LogInstanceKey = "LogInst",
					TableLogInstanceKey = "APZ_AWBS1",
					LineKey = "LineId"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ATASK",
					TableName = "APZ_TASK",
					Key = "Code",
					LogInstanceKey = "LogInst",
					TableLogInstanceKey = "APZ_ATASK",
					LineKey = "LineId"
				}
			}
		},
		{
			ApzObjectTypes.oProjectConfirm,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOPJC",
					TableName = "APZ_OPJC",
					Key = "DocEntry",
					LineKey = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPJC",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APJC1",
					TableName = "APZ_PJC1",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_APJC1",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oReportCompletion,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AORTC",
					TableName = "APZ_ORTC",
					Key = "DocEntry",
					LineKey = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AORTC",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ARTC1",
					TableName = "APZ_RTC1",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_ARTC1",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oPenalization,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AHPT2",
					TableName = "APZ_HPT2",
					Key = "DocNum",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AHPT2",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHPT1",
					TableName = "APZ_HPT1",
					Key = "DocNum",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AHPT2",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oTaskManagement,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_ACLG",
					TableName = "APZ_OCLG",
					Key = "ClgCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_ACLG",
					UserSignKey = "AssignedBy",
					UserSign2Key = "UserSign",
					UpdateDateKey = "UpdateDate",
					CreateDateKey = "CntctDate",
					UpdateTimeKey = "UpdateTime",
					CreateTimeKey = "CntctTime",
					NotShowLogKey = new string[1] { "HtmlNotes,ResourceIdsInfo,RecurrenceInfo,ReminderInfo" }
				}
			}
		},
		{
			ApzObjectTypes.oEventManagement,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_ACLG",
					TableName = "APZ_OCLG",
					Key = "ClgCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_ACLG",
					UserSignKey = "AssignedBy",
					UserSign2Key = "UserSign",
					UpdateDateKey = "UpdateDate",
					CreateDateKey = "CntctDate",
					UpdateTimeKey = "UpdateTime",
					CreateTimeKey = "CntctTime",
					NotShowLogKey = new string[1] { "HtmlNotes,ResourceIdsInfo,RecurrenceInfo,ReminderInfo" }
				}
			}
		},
		{
			ApzObjectTypes.oNoteManagement,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_ACLG",
					TableName = "APZ_OCLG",
					Key = "ClgCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_ACLG",
					UserSignKey = "AssignedBy",
					UserSign2Key = "UserSign",
					UpdateDateKey = "UpdateDate",
					CreateDateKey = "CntctDate",
					UpdateTimeKey = "UpdateTime",
					CreateTimeKey = "CntctTime",
					NotShowLogKey = new string[1] { "HtmlNotes,ResourceIdsInfo,RecurrenceInfo,ReminderInfo" }
				}
			}
		},
		{
			ApzObjectTypes.oWorkPeriod,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHWP",
					TableName = "APZ_OHWP",
					Key = "PeriodCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHWP",
					UserSignKey = "UserSign",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				}
			}
		},
		{
			ApzObjectTypes.oTimeSheeting,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOTSL",
					TableName = "APZ_OTSL",
					Key = "Period,EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTSL",
					LineKey = "",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ATSL1",
					TableName = "APZ_TSL1",
					Key = "Period,EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTSL",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ATSL3",
					TableName = "APZ_TSL3",
					Key = "Period,EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTSL"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ATSL2",
					TableName = "APZ_TSL2",
					Key = "Period,EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_ATSL2",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				}
			}
		},
		{
			ApzObjectTypes.oPersonalIncomeTax,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOPIT",
					TableName = "APZ_OPIT",
					Key = "Code",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPIT",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APIT1",
					TableName = "APZ_PIT1",
					Key = "Code",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOPIT",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oDeductionType,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHDT",
					TableName = "APZ_OHDT",
					Key = "Code",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHDT",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHDT1",
					TableName = "APZ_HDT1",
					Key = "Code",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHDT",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oHumanresourcesJobVacacy,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHJV",
					TableName = "APZ_OHJV",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV1",
					TableName = "APZ_HJV1",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV2",
					TableName = "APZ_HJV2",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV3",
					TableName = "APZ_HJV3",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV4",
					TableName = "APZ_HJV4",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV5",
					TableName = "APZ_HJV5",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV6",
					TableName = "APZ_HJV6",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV7",
					TableName = "APZ_HJV7",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_AHJV8",
					TableName = "APZ_HJV8",
					Key = "DocEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHJV",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oFormSetting,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AFORMSETTING",
					TableName = "APZ_FORMSETTING",
					Key = "FormId,ItemId,UserSign",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AFORMSETTING",
					UserSign2Key = "UserUpdate",
					UpdateDateKey = "UpdateDate",
					LineKey = "ColumnId"
				}
			}
		},
		{
			ApzObjectTypes.oWorkCode,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHWC",
					TableName = "APZ_OHWC",
					Key = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHWC",
					UserSign2Key = "UserSign",
					UpdateDateKey = "UpdateDate",
					LineKey = "Branch,Code"
				}
			}
		},
		{
			ApzObjectTypes.oRouting,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AORTA",
					TableName = "APZ_ORTA",
					Key = "RoutCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AORTA",
					UserSign2Key = "UserUpdate",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ARTA1",
					TableName = "APZ_RTA1",
					Key = "RoutCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AORTA",
					UserSign2Key = "UserUpdate",
					UpdateDateKey = "UpdateDate",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ARTA2",
					TableName = "APZ_RTA2",
					Key = "RoutCode",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AORTA",
					UserSign2Key = "UserUpdate",
					UpdateDateKey = "UpdateDate",
					LineKey = "FatherLine,ResCode"
				}
			}
		},
		{
			ApzObjectTypes.oOvertimeType,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHOT",
					TableName = "APZ_OHOT",
					Key = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHOT",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					LineKey = "Branch,Code"
				}
			}
		},
		{
			ApzObjectTypes.oSalaryInfo,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHSI",
					TableName = "APZ_OHSI",
					Key = "EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHSI",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oSalAllowance,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AHSI1",
					TableName = "APZ_HSI1",
					Key = "EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AHSI1",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					LineKey = "LineNum,AllowanceType"
				}
			}
		},
		{
			ApzObjectTypes.oSalAdvance,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AHSI2",
					TableName = "APZ_HSI2",
					Key = "EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AHSI2",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oSalCalculation,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOSAL",
					TableName = "APZ_OSAL",
					Key = "Period,empID",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOSAL",
					LineKey = "CalFrom",
					UserSignKey = "UserSign",
					UserSign2Key = "UserSign2",
					CreateDateKey = "CreateDate",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ASAL1",
					TableName = "APZ_SAL1",
					Key = "Period,EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOSAL",
					LineKey = "CalFrom,AllowanceType"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ASAL2",
					TableName = "APZ_SAL2",
					Key = "Period,empID",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOSAL",
					LineKey = "CalFrom,DeducCode"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ASAL3",
					TableName = "APZ_SAL3",
					Key = "Period,empID",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOSAL",
					LineKey = "CalFrom,OTCode"
				}
			}
		},
		{
			ApzObjectTypes.oTimeSheetingHour,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOTSH",
					TableName = "APZ_OTSH",
					Key = "Period,EmpId,OvertimeType",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTSH",
					LineKey = "",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ATSH1",
					TableName = "APZ_TSH1",
					Key = "Period,EmpId,OvertimeType",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTSH",
					LineKey = "LineNum"
				}
			}
		},
		{
			ApzObjectTypes.oAllowanceType,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHAT",
					TableName = "APZ_OHAT",
					Key = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHAT",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					LineKey = "Code"
				}
			}
		},
		{
			ApzObjectTypes.oPenalType,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOHPT",
					TableName = "APZ_OHPT",
					Key = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOHPT",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					LineKey = "Code"
				}
			}
		},
		{
			ApzObjectTypes.oLeaveInformation,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_ATSL2",
					TableName = "APZ_TSL2",
					Key = "Period,EmpId",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_ATSL2",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate"
				}
			}
		},
		{
			ApzObjectTypes.oBiometricData,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_AOTBD",
					TableName = "APZ_OTBD",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTBD",
					CreateDateKey = "ImportDate",
					CreateTimeKey = "ImportTime",
					UserSignKey = "UserImport",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_ATBD1",
					TableName = "APZ_TBD1",
					Key = "AbsEntry",
					LineKey = "AbsEntry,EnrollNo,LineNum",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_AOTBD",
					NotShowLogKey = new string[1] { "FixedNote" }
				}
			}
		},
		{
			ApzObjectTypes.oKPIsSetting,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AKPM",
					TableName = "APZ_D_OKPM",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AKPM",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime",
					CreateDateKey = "CreateDate",
					CreateTimeKey = "CreateTime"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_AKPM1",
					TableName = "APZ_D_KPM1",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AKPM",
					LineKey = "KPI_Code,Condition_Code,FromValue"
				}
			}
		},
		{
			ApzObjectTypes.oKPIsMasterData,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AOKPI",
					TableName = "APZ_D_OKPI",
					Key = "",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOKPI",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime",
					CreateDateKey = "CreateDate",
					CreateTimeKey = "CreateTime",
					LineKey = "Code"
				}
			}
		},
		{
			ApzObjectTypes.oSetIndicatorKPIs,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AKPIS",
					TableName = "APZ_D_KPIS",
					Key = "Code",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AKPIS",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime",
					CreateDateKey = "CreateDate",
					CreateTimeKey = "CreateTime"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_AKPIS1",
					TableName = "APZ_D_KPIS1",
					Key = "Code",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_D_AKPIS",
					LineKey = "KPI_Code,Code"
				}
			}
		},
		{
			ApzObjectTypes.oKPIsListPoint,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AKPR",
					TableName = "APZ_D_OKPR",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AKPR",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime",
					CreateDateKey = "CreateDate",
					CreateTimeKey = "CreateTime"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_AKPR1",
					TableName = "APZ_D_KPR1",
					Key = "AbsEntry",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AKPR",
					LineKey = "AbsEntry,KPI_Code,EmpId",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime",
					CreateDateKey = "CreateDate",
					CreateTimeKey = "CreateTime"
				}
			}
		},
		{
			ApzObjectTypes.oOutletVisitingFrequency,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AOOVF",
					TableName = "APZ_D_OOVF",
					Key = "",
					LogInstanceKey = "LogInstanc",
					TableLogInstanceKey = "APZ_D_AOOVF",
					UserSign2Key = "UserSign",
					UpdateDateKey = "UpdateDate",
					LineKey = "Code"
				}
			}
		},
		{
			ApzObjectTypes.oPromotionBenefit,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AOPBF",
					TableName = "APZ_D_OPBF",
					Key = "Code",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOPBF",
					UserSign2Key = "UserSign2",
					UpdateDateKey = "UpdateDate",
					UpdateTimeKey = "UpdateTime",
					CreateDateKey = "CreateDate",
					CreateTimeKey = "CreateTime"
				}
			}
		},
		{
			ApzObjectTypes.oSalesRoute,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_D_AOSTR",
					TableName = "APZ_D_OSTR",
					Key = "RouteCode",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOSTR",
					UpdateDateKey = "UpdateDate",
					CreateDateKey = "CreateDate",
					UpdateTimeKey = "UpdateTime",
					CreateTimeKey = "CreateTime",
					UserSignKey = "UserSign",
					UserSign2Key = "UserSign2",
					NotShowLogKey = new string[1] { "" }
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_ASTR1",
					TableName = "APZ_D_STR1",
					Key = "RouteCode",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOSTR",
					LineKey = "Id,RouteCode"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_ASTR2",
					TableName = "APZ_D_STR2",
					Key = "RouteCode",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOSTR",
					LineKey = "Id,RouteCode,CardCode,EmpId"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_ASTR3",
					TableName = "APZ_D_STR3",
					Key = "RouteCode",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOSTR",
					LineKey = "Id,RouteCode,CardCode,EmpId"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_D_ASTR4",
					TableName = "APZ_D_STR4",
					Key = "RouteCode",
					LogInstanceKey = "LogInstance",
					TableLogInstanceKey = "APZ_D_AOSTR",
					LineKey = "Id,RouteCode,EmpId"
				}
			}
		},
		{
			ApzObjectTypes.oPromotionProgram,
			new List<LogInstance>
			{
				new LogInstance
				{
					HistoryTableName = "APZ_APPR",
					TableName = "APZ_OPPR",
					Key = "DocEntry",
					LogInstanceKey = "Instance",
					TableLogInstanceKey = "APZ_APPR",
					LineKey = "",
					UserSign2Key = "",
					CreateDateKey = "CreateDate",
					UpdateDateKey = "UpdateDate"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APPR1",
					TableName = "APZ_PPR1",
					Key = "DocEntry",
					LogInstanceKey = "Instance",
					TableLogInstanceKey = "APZ_APPR",
					LineKey = "LineNum"
				},
				new LogInstance
				{
					HistoryTableName = "APZ_APPR2",
					TableName = "APZ_PPR2",
					Key = "DocEntry",
					LogInstanceKey = "Instance",
					TableLogInstanceKey = "APZ_APPR"
				}
			}
		}
	};

	public static Dictionary<Apzon.Commons.Enums.Enums.ChangeLog, ApzObjectTypes> ChangeLogObjMapping = new Dictionary<Apzon.Commons.Enums.Enums.ChangeLog, ApzObjectTypes>
	{
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clProjectManager,
			ApzObjectTypes.oProjectManager
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clPenalization,
			ApzObjectTypes.oPenalization
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clTaskManagement,
			ApzObjectTypes.oTaskManagement
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clEventManagement,
			ApzObjectTypes.oEventManagement
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clNoteManagement,
			ApzObjectTypes.oNoteManagement
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clWorkPeriod,
			ApzObjectTypes.oWorkPeriod
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clTimeSheeting,
			ApzObjectTypes.oTimeSheeting
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clPersionalIncomeTax,
			ApzObjectTypes.oPersonalIncomeTax
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clDeductionType,
			ApzObjectTypes.oDeductionType
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clHumanresourcesJobVacacy,
			ApzObjectTypes.oHumanresourcesJobVacacy
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clFormSetting,
			ApzObjectTypes.oFormSetting
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clWorkCode,
			ApzObjectTypes.oWorkCode
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clRouting,
			ApzObjectTypes.oRouting
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clOvertimeType,
			ApzObjectTypes.oOvertimeType
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clSalaryInfo,
			ApzObjectTypes.oSalaryInfo
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clSalAllowance,
			ApzObjectTypes.oSalAllowance
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clSalAdvance,
			ApzObjectTypes.oSalAdvance
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clSalaryCalculation,
			ApzObjectTypes.oSalCalculation
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clTimeSheetingHour,
			ApzObjectTypes.oTimeSheetingHour
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clAllowanceType,
			ApzObjectTypes.oAllowanceType
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clPenalType,
			ApzObjectTypes.oPenalType
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clLeaveInformation,
			ApzObjectTypes.oLeaveInformation
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clBiometricData,
			ApzObjectTypes.oBiometricData
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clKPIsSetting,
			ApzObjectTypes.oKPIsSetting
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clKPIsMasterData,
			ApzObjectTypes.oKPIsMasterData
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clSetIndicatorKPIs,
			ApzObjectTypes.oSetIndicatorKPIs
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clKPIsListPoint,
			ApzObjectTypes.oKPIsListPoint
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clPromotionBenefit,
			ApzObjectTypes.oPromotionBenefit
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clWBSTemplate,
			ApzObjectTypes.oWBSTemplate
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clProjectConfirm,
			ApzObjectTypes.oProjectConfirm
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clReportCompletion,
			ApzObjectTypes.oReportCompletion
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clOutletVisitingFrequency,
			ApzObjectTypes.oOutletVisitingFrequency
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clSalesRoute,
			ApzObjectTypes.oSalesRoute
		},
		{
			Apzon.Commons.Enums.Enums.ChangeLog.clPromotionProgram,
			ApzObjectTypes.oPromotionProgram
		}
	};

	public static Dictionary<string, bool> HiddenFunction = new Dictionary<string, bool>
	{
		{ "frm_EmpToken", false },
		{ "frm_AuthorToken", false },
		{ "frm_ProcessDIServer", true },
		{ "frm_WorkingTable", false },
		{ "frm_DataOwnerShipAuthorization", false },
		{ "frm_PaymentForInvoice", true },
		{ "frm_CreditCardSetup", true },
		{ "frm_CreditPaymentMethods", true },
		{ "frm_ActivitiesForOpportunity", false },
		{ "frm_Dashboard", true },
		{ "frm_DashBoardAuthorization", true },
		{ "frm_DashboardCategories", true },
		{ "frm_DashboardTab", true },
		{ "frm_PickAndPack", true },
		{ "frm_StockRequest", true },
		{ "frm_MrpPlan", true },
		{ "frm_ProductionConfirm", true },
		{ "frm_ProductionForBatch", true },
		{ "frm_PackingBatch", true },
		{ "frm_ProductionReqForBatch", true },
		{ "frm_ProcurementConfirm", true },
		{ "frm_RetailStore", false },
		{ "frm_Resources", true },
		{ "frm_Routing", true },
		{ "frm_Operation", true },
		{ "frm_HandoverDocument", true },
		{ "frm_Atp", true },
		{ "frm_DeliveryConfirm", true },
		{ "frm_DeliveryControl", true },
		{ "frm_DeliveryOrder", true },
		{ "frm_SalesControl", true },
		{ "frm_ServiceCall", false },
		{ "frm_ServiceCallSetting", false },
		{ "frm_Solution", false },
		{ "frm_Contract", false },
		{ "frm_ReceiptBin", true },
		{ "frm_BatchesControl", true },
		{ "frm_ImportDataWizard", false },
		{ "frm_AllowanceType", false },
		{ "frm_DeductionType", false },
		{ "frm_PersonalIncomeTax", false },
		{ "frm_SalAllowance", false },
		{ "frm_SalAllowanceByEmp", false },
		{ "frm_SalaryCalcResult", true },
		{ "frm_SalaryCalculation", false },
		{ "frm_SalaryInfo", false },
		{ "frm_SalaryParameters ", false },
		{ "frm_SalInfoByEmp", false },
		{ "frm_SalInfoMain", false },
		{ "frm_ImportTimeSheeting", false },
		{ "frm_NewPeriod", false },
		{ "frm_OvertimeType", false },
		{ "frm_TimeSheeting", false },
		{ "frm_TimeSheetingHour", false },
		{ "frm_WorkCode", false },
		{ "frm_WorkPeriod", false },
		{ "frm_WorkType", false },
		{ "frm_AdvertisementSource", false },
		{ "frm_JobType", false },
		{ "frm_JobVacancy", false },
		{ "frm_JVApplicants", false },
		{ "frm_JVBenefits", false },
		{ "frm_JVInterviewStatus", false },
		{ "frm_JVInterviewTypes", false },
		{ "frm_JVQuestion", false },
		{ "frm_JVQuestionGroup", false },
		{ "frm_Qualifications", false },
		{ "frm_QualificationsGroup", false },
		{ "frm_Reviews", false },
		{ "frm_Vehicle", false },
		{ "frm_VehicleType", false },
		{ "frm_ShippingDocument", false },
		{ "frm_ShippingDocumentLoaded", false },
		{ "frm_ShippingDocumentOutboundRoute", false },
		{ "frm_ShippingDocumentPicked", false },
		{ "frm_AutoReportComplete", false },
		{ "frm_GenPOFromMRP", false },
		{ "frm_GenPOFromSchedule", false },
		{ "frm_GenPurchaseFromSchedule", false },
		{ "frm_GenTranferRequestFromMrp", false },
		{ "frm_ItemQuotaArrangement", false },
		{ "frm_ItemQuotaArrangementDate", false },
		{ "frm_Lines", false },
		{ "frm_MRP", false },
		{ "frm_Schedule", false },
		{ "frm_PrintBarCode", false },
		{ "frm_SalesForecast", false },
		{ "SalesForeCastQuickAnalys", false },
		{ "frm_LandedCosts", false },
		{ "frm_LandedCostsSetup", false },
		{ "frm_ORW", false },
		{ "frm_ProductionOrderImportTool", false },
		{ "frm_JetstarPreOrder", true }
	};

	public static Dictionary<string, string> oDocumentNumberingCode = new Dictionary<string, string>
	{
		{
			2 + "S",
			"Business Partners - Vendor"
		},
		{
			2 + "C",
			"Business Partners - Customer"
		},
		{
			4.ToString() ?? "",
			"Items"
		},
		{
			290.ToString() ?? "",
			"Resources"
		},
		{
			13.ToString() ?? "",
			"A/R Invoice"
		},
		{
			14.ToString() ?? "",
			"A/R Credit Notes"
		},
		{
			15.ToString() ?? "",
			"Deliveries"
		},
		{
			16.ToString() ?? "",
			"Returns"
		},
		{
			17.ToString() ?? "",
			"Sales Orders"
		},
		{
			18.ToString() ?? "",
			"A/P Invoices"
		},
		{
			19.ToString() ?? "",
			"A/P Credit Memos"
		},
		{
			20.ToString() ?? "",
			"Goods Receipt PO"
		},
		{
			21.ToString() ?? "",
			"Goods Return"
		},
		{
			22.ToString() ?? "",
			"Purchase Orders"
		},
		{
			23.ToString() ?? "",
			"Sales Quotations"
		},
		{
			24.ToString() ?? "",
			"Incoming Payments"
		},
		{
			30.ToString() ?? "",
			"Journal Entries"
		},
		{
			46.ToString() ?? "",
			"Outgoing Payments"
		},
		{
			59.ToString() ?? "",
			"Goods Receipt"
		},
		{
			60.ToString() ?? "",
			"Goods Issue"
		},
		{
			1250000001.ToString() ?? "",
			"Inventory Transfer Request"
		},
		{
			67.ToString() ?? "",
			"Inventory Transfer"
		},
		{
			69.ToString() ?? "",
			"Landed Cost"
		},
		{
			162.ToString() ?? "",
			"Inventory Revaluation"
		},
		{
			191.ToString() ?? "",
			"Service Call"
		},
		{
			202.ToString() ?? "",
			"Production Order"
		},
		{
			203.ToString() ?? "",
			"A/R Down Payment"
		},
		{
			204.ToString() ?? "",
			"A/P Down Payment"
		},
		{
			10000071.ToString() ?? "",
			"Inventory Posting"
		},
		{
			310000001.ToString() ?? "",
			"Inventory Open Balances"
		},
		{
			540000006.ToString() ?? "",
			"Purchase Quotation"
		},
		{
			1250000025 + "C",
			"Sales Blanket Agreement"
		},
		{
			1250000025 + "S",
			"Purchase Blanket Agreement"
		},
		{
			1470000138.ToString() ?? "",
			"Inventory Counting"
		},
		{
			1470000113.ToString() ?? "",
			"Purchase Request"
		},
		{
			234000031.ToString() ?? "",
			"Return Request"
		}
	};
}
