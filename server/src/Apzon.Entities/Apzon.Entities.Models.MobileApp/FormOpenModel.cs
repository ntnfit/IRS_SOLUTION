using System.Collections.Generic;

namespace Apzon.Entities.Models.MobileApp;

public class FormOpenModel
{
	public string FormCaption { get; set; }

	public List<FieldFormModel> Fields { get; set; }
}
