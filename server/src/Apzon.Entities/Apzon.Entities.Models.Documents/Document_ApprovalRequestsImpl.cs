namespace Apzon.Entities.Models.Documents;

public class Document_ApprovalRequestsImpl
{
	public int ApprovalTemplatesID { get; set; }

	public int StageId { get; set; }

	public int Count { get; set; }

	public string Remarks { get; set; }

	public string Name { get; set; }

	public string ReceiptName { get; set; }

	public int ReceiptId { get; set; }

	public int ReqId { get; set; }
}
