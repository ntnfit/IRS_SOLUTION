using System;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Common;

public class MessageHeaderImpl
{
	public int Code { get; set; }

	public BoYesNoEnum Read { get; set; }

	public BoYesNoEnum Received { get; set; }

	public DateTime ReceivedDate { get; set; }

	public DateTime ReceivedTime { get; set; }

	public DateTime SentDate { get; set; }

	public DateTime SentTime { get; set; }
}
