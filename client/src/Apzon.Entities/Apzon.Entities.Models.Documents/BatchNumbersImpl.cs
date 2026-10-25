using System;
using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Documents;

public class BatchNumbersImpl
{
	public DateTime? AddmisionDate { get; set; }

	public int BaseLineNumber { get; set; }

	public string BatchNumber { get; set; }

	public int Count { get; set; }

	public DateTime? ExpiryDate { get; set; }

	public string InternalSerialNumber { get; set; }

	public string Location { get; set; }

	public string ManufacturerSerialNumber { get; set; }

	public DateTime? ManufacturingDate { get; set; }

	public string Notes { get; set; }

	public double Quantity { get; set; }

	public int TrackingNote { get; set; }

	public int TrackingNoteLine { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }
}
