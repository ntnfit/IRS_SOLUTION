using System;

namespace Apzon.Entities.Models.Administrator;

public class ContactUsImpl
{
	public int Id { get; set; }

	public string Name { get; set; }

	public string Phone { get; set; }

	public string Email { get; set; }

	public string Address { get; set; }

	public string Remark { get; set; }

	public DateTime CreatedTime { get; set; }
}
