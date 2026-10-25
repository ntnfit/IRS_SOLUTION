using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.BusinessPartners;

public class ContactPersonImpl
{
	public string Active { get; set; }

	public string Address { get; set; }

	public string CardCode { get; set; }

	public DateTime? BirthDate { get; set; }

	public string Fax { get; set; }

	public string FirstName { get; set; }

	public int InternalCode { get; set; }

	public string LastName { get; set; }

	public string MiddleName { get; set; }

	public string Name { get; set; }

	public string Pager { get; set; }

	public string Password { get; set; }

	public string Position { get; set; }

	public string Profession { get; set; }

	public string Title { get; set; }

	public int CntctCode { get; set; }

	public string Tel1 { get; set; }

	public string Tel2 { get; set; }

	public string Cellolar { get; set; }

	public string E_MailL { get; set; }

	public string Notes1 { get; set; }

	public string Notes2 { get; set; }

	public string BirthPlace { get; set; }

	public string Gender { get; set; }

	public int UserSign { get; set; }

	public string BirthCity { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
