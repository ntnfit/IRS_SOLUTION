using Apzon.Entities.Models.Documents;

namespace Apzon.Entities.Models.DMS;

public class DMSSaleImpl
{
	public DocumentsImpl Document { get; set; }

	public int PageIndex { get; set; }

	public string SearchText { get; set; }

	public string SkipAuthorization { get; set; }

	public string DMSSubType { get; set; }
}
