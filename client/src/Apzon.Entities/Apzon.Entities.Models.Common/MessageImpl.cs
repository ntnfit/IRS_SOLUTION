using System.Collections.Generic;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Common;

public class MessageImpl
{
	public int Attachment { get; set; }

	public List<MessageDataColumnsImpl> MessageDataColumns { get; set; }

	public BoMsgPriorities Priority { get; set; }

	public RecipientCollection RecipientCollection { get; set; }

	public string Subject { get; set; }

	public string Text { get; set; }

	public int User { get; set; }
}
