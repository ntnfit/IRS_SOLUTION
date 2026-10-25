using System;
using System.ComponentModel;
using Apzon.Commons;
using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class NotifiedObj
{
	private ComponentResourceManager _resource = new ComponentResourceManager(Type.GetType("Apzon.Commons.Properties.Notification, Apzon.Commons"));

	public string DateTimeFormat { get; set; }

	public ApzObjectTypes ObjectTypes { get; set; }

	public string NotifiedId { get; set; }

	public DateTime DueDate { get; set; }

	public string DueDateView
	{
		get
		{
			return Function.FormatDateToView(DueDate);
		}
		private set
		{
		}
	}

	public string NotifiedTitle
	{
		get
		{
			switch (ObjectTypes)
			{
			case ApzObjectTypes.oTaskManagement:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Task"));
				}
				return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Task_Comment"));
			case ApzObjectTypes.oEventManagement:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Event"));
				}
				return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Event_Comment"));
			case ApzObjectTypes.oTrackingIssue:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Issue"));
				}
				return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Issue_Comment"));
			case ApzObjectTypes.oNoteManagement:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Note"));
				}
				return string.Format("{0} {1}", CreateUserName, _resource.GetString("Title_Note_Comment"));
			case ApzObjectTypes.oAlertOverview:
				return $"{SumResult} works";
			case ApzObjectTypes.oWorkingProcess:
				if (TranType != Enums.TransactionType.M)
				{
					return "Approval Request Answered";
				}
				return "New Approval Request";
			default:
				return $"UNKNOWN";
			}
		}
		private set
		{
		}
	}

	public string Detail { get; set; }

	public string NotifiedBody
	{
		get
		{
			switch (ObjectTypes)
			{
			case ApzObjectTypes.oTaskManagement:
				return $"{Detail}";
			case ApzObjectTypes.oEventManagement:
				if (TranType != Enums.TransactionType.M)
				{
					return $"{Detail} , in {Location}";
				}
				return Detail;
			default:
				return $"{Detail}";
			}
		}
		private set
		{
		}
	}

	public string CreateUserCode { get; set; }

	public string CreateUserName { get; set; }

	public string AssignUser { get; set; }

	public string NotifiedUser { get; set; }

	public string Location { get; set; }

	public int SumResult { get; set; }

	public int CmtCode { get; set; }

	public Enums.TransactionType TranType { get; set; }

	public string ObjectView
	{
		get
		{
			switch (ObjectTypes)
			{
			case ApzObjectTypes.oTaskManagement:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Task"));
				}
				return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Comments"));
			case ApzObjectTypes.oEventManagement:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Event"));
				}
				return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Comments"));
			case ApzObjectTypes.oTrackingIssue:
				if (TranType != Enums.TransactionType.M)
				{
					return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Issue"));
				}
				return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Comments"));
			case ApzObjectTypes.oAlertOverview:
				return string.Format("{1} ({0})", CountGroup, _resource.GetString("Group_Alert"));
			case ApzObjectTypes.oWorkingProcess:
				return "APPROVAL";
			default:
				return $"UNKNOWN";
			}
		}
		private set
		{
		}
	}

	public int CountGroup { get; set; }

	public int ApptType { get; set; }

	public string RecurrenceInfo { get; set; }

	public TimeSpan TimeBeforeStart { get; set; }

	public bool AssignAll { get; set; }
}
