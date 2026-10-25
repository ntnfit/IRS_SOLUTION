using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class SerialNumbersImpl
{
	public int BaseLineNumber { get; set; }

	public string BatchID { get; set; }

	public int Count { get; set; }

	public DateTime? ExpiryDate { get; set; }

	public string InternalSerialNumber { get; set; }

	public string Location { get; set; }

	public DateTime? ManufactureDate { get; set; }

	public string ManufacturerSerialNumber { get; set; }

	public string Notes { get; set; }

	public double Quantity { get; set; }

	public DateTime? ReceptionDate { get; set; }

	public int SystemSerialNumber { get; set; }

	public int TrackingNote { get; set; }

	public int TrackingNoteLine { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public DateTime? WarrantyEnd { get; set; }

	public DateTime? WarrantyStart { get; set; }
}
