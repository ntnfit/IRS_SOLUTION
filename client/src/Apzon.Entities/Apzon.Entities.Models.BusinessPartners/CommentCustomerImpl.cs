using System;

namespace Apzon.Entities.Models.BusinessPartners;

public class CommentCustomerImpl
{
	public Guid Code { get; set; }

	public string Content { get; set; }

	public string RelatedTyp { get; set; }

	public string RelatedEnt { get; set; }

	public int UserSign { get; set; }

	public string UserName { get; set; }

	public DateTime CreatedTime { get; set; }

	public DateTime UpdatedTime { get; set; }

	public int LogInstance { get; set; }
}
