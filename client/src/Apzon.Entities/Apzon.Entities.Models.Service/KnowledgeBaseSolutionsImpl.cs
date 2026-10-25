using System;
using System.Collections.Generic;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Service;

public class KnowledgeBaseSolutionsImpl
{
	public int SltCode { get; set; }

	public string ItemCode { get; set; }

	public int? StatusNum { get; set; }

	public int? Owner { get; set; }

	public int? CreatedBy { get; set; }

	public DateTime? DateCreate { get; set; }

	public int? UpdateBy { get; set; }

	public DateTime? DateUpdate { get; set; }

	public string Subject { get; set; }

	public string Symptom { get; set; }

	public string Cause { get; set; }

	public string Descriptio { get; set; }

	public string Attachment { get; set; }

	public int AtcEntry { get; set; }

	public string Transfered { get; set; }

	public short? Instance { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public List<AttachmentFile_Impl> AttachmentFiles { get; set; }
}
