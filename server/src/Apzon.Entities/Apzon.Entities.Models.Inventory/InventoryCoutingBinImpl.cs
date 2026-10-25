using System;

namespace Apzon.Entities.Models.Inventory;

public class InventoryCoutingBinImpl
{
	public int DocEntry { get; set; }

	public int DocNum { get; set; }

	public int Period { get; set; }

	public short Instance { get; set; }

	public int Series { get; set; }

	public string Handwrtten { get; set; }

	public string Canceled { get; set; }

	public string Object { get; set; }

	public int LogInst { get; set; }

	public int UserSign { get; set; }

	public string Transfered { get; set; }

	public string Status { get; set; }

	public DateTime CreateDate { get; set; }

	public int CreateTime { get; set; }

	public DateTime UpdateDate { get; set; }

	public int UpdateTime { get; set; }

	public string DataSource { get; set; }

	public string RequestStatus { get; set; }

	public string Creator { get; set; }

	public string Remark { get; set; }

	public string U_DistNumber { get; set; }

	public string U_WhsCode { get; set; }

	public string U_UserCode { get; set; }

	public string U_BinCode { get; set; }

	public DateTime U_DocDate { get; set; }
}
