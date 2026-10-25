using System;
using System.Data;

namespace Apzon.Forms.Service;

public class Message
{
	private DataRow row;

	private DateTime date;

	private bool read;

	private bool deleted;

	private bool isReply;

	private bool hasAttachment;

	private int priority = 1;

	private MailType mailType;

	private MailFolder mailFolder;

	private string from = string.Empty;

	private string subject = string.Empty;

	private string text = string.Empty;

	private string plainText = string.Empty;

	public DateTime Date
	{
		get
		{
			return date;
		}
		set
		{
			date = value;
		}
	}

	public string From
	{
		get
		{
			return from;
		}
		set
		{
			from = value;
		}
	}

	public string Subject
	{
		get
		{
			return subject;
		}
		set
		{
			subject = value;
		}
	}

	public string SubjectDisplayText => string.Format("{1}{0}", Subject, IsReply ? "RE: " : "");

	public int Attachment
	{
		get
		{
			if (!hasAttachment)
			{
				return 0;
			}
			return 1;
		}
	}

	public int Read
	{
		get
		{
			if (!read)
			{
				return 0;
			}
			return 1;
		}
	}

	public int Priority
	{
		get
		{
			return priority;
		}
		set
		{
			priority = value;
		}
	}

	internal bool IsReply
	{
		get
		{
			return isReply;
		}
		set
		{
			isReply = value;
		}
	}

	public bool IsUnread => !read;

	internal string Folder => $"{mailFolder}";

	public string Text
	{
		get
		{
			return text;
		}
		set
		{
			text = value;
		}
	}

	public string PlainText => GetPlainText();

	public MailType MailType
	{
		get
		{
			return mailType;
		}
		set
		{
			mailType = value;
		}
	}

	public MailFolder MailFolder
	{
		get
		{
			return mailFolder;
		}
		set
		{
			mailFolder = value;
		}
	}

	public bool Deleted
	{
		get
		{
			return deleted;
		}
		set
		{
			deleted = value;
		}
	}

	internal TimeSpan Delay => DateTime.Now - date;

	public Message()
	{
		date = DateTime.Now;
	}

	public Message(DataRow row)
	{
		this.row = row;
		date = ((DateTime)row["Date"]).Add(DateTime.Now - DataHelper.LastMailDate);
		from = string.Format("{0}", row["From"]);
		subject = string.Format("{0}", row["Subject"]);
		isReply = (bool)row["IsReply"];
		hasAttachment = (bool)row["HasAttachment"];
		read = Delay > TimeSpan.FromHours(6.0);
		if (Delay > TimeSpan.FromHours(50.0) && Delay < TimeSpan.FromHours(100.0))
		{
			read = false;
		}
		text = string.Format("{0}", row["Text"]);
		deleted = false;
		priority = CalcPriority();
		plainText = GetPlainText();
		mailType = MailType.Inbox;
		mailFolder = GetFolder(row);
		DataTweaking();
	}

	private int CalcPriority()
	{
		if (!IsReply || IsUrgent(From))
		{
			return 2;
		}
		if (string.IsNullOrEmpty(Folder))
		{
			return 0;
		}
		return 1;
	}

	private bool IsUrgent(string from)
	{
		if (!string.Equals(from, "Kurt Wehrend"))
		{
			return string.Equals(from, "Juan Betancourt");
		}
		return true;
	}

	private string GetPlainText()
	{
		if (string.IsNullOrEmpty(plainText))
		{
			plainText = ObjectHelper.GetPlainText(text.Substring(0, Math.Min(100, text.Length)));
		}
		return plainText;
	}

	public void ToggleRead()
	{
		read = !read;
	}

	private void DataTweaking()
	{
		if (!IsReply && Subject.IndexOf("IDataStore") > 0)
		{
			read = false;
		}
	}

	private MailFolder GetFolder(DataRow row)
	{
		string text = string.Format("{0}", row["Folder"]);
		if (string.IsNullOrEmpty(text))
		{
			return MailFolder.All;
		}
		return (MailFolder)Enum.Parse(typeof(MailFolder), text.Replace(" ", ""));
	}

	public void SetPlainText(string text)
	{
		plainText = text;
	}
}
