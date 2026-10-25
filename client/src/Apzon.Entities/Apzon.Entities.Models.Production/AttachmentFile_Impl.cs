using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Production;

public class AttachmentFile_Impl
{
	public int AbsEntry { get; set; }

	public int Line { get; set; }

	public string srcPath { get; set; }

	public string trgtPath { get; set; }

	public string FileName { get; set; }

	public string FileExt { get; set; }

	public DateTime Date { get; set; }

	public int UsrID { get; set; }

	public string Copied { get; set; }

	public string Override { get; set; }

	public string subPath { get; set; }

	public string FreeText { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
