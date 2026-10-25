using System.Collections.Generic;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Administrator;

public class ItemGroupsImpl
{
	public bool Alert { get; set; }

	public string ComponentWarehouse { get; set; }

	public string CostAccount { get; set; }

	public string CostInflationAccount { get; set; }

	public string CostInflationOffsetAccount { get; set; }

	public int CycleCode { get; set; }

	public string DecreaseGLAccount { get; set; }

	public string DecreasingAccount { get; set; }

	public int DefaultInventoryUoM { get; set; }

	public int DefaultUoMGroup { get; set; }

	public string EUExpensesAccount { get; set; }

	public string EUPurchaseCreditAcc { get; set; }

	public string EURevenuesAccount { get; set; }

	public string ExchangeRateDifferencesAccount { get; set; }

	public string ExemptRevenuesAccount { get; set; }

	public string ExemptedCredits { get; set; }

	public string ExpenseClearingAct { get; set; }

	public string ExpenseOffsetAccount { get; set; }

	public string ExpensesAccount { get; set; }

	public string ForeignExpensesAccount { get; set; }

	public string ForeignPurchaseCreditAcc { get; set; }

	public string ForeignRevenuesAccount { get; set; }

	public string GoodsClearingAccount { get; set; }

	public string GroupName { get; set; }

	public string IncreaseGLAccount { get; set; }

	public string IncreasingAccount { get; set; }

	public string InventoryAccount { get; set; }

	public string InventoryOffsetProfitAndLossAccount { get; set; }

	public BoInventorySystem InventorySystem { get; set; }

	public int LeadTime { get; set; }

	public double MinimumOrderQuantity { get; set; }

	public string NegativeInventoryAdjustmentAccount { get; set; }

	public int Number { get; set; }

	public int OrderInterval { get; set; }

	public double OrderMultiple { get; set; }

	public string PAReturnAccount { get; set; }

	public BoPlanningSystem PlanningSystem { get; set; }

	public string PriceDifferencesAccount { get; set; }

	public string ProcurementMethod { get; set; }

	public string PurchaseAccount { get; set; }

	public string PurchaseBalanceAccount { get; set; }

	public string PurchaseCreditAcc { get; set; }

	public string PurchaseOffsetAccount { get; set; }

	public string ReturningAccount { get; set; }

	public string RevenuesAccount { get; set; }

	public string SalesCreditAcc { get; set; }

	public string SalesCreditEUAcc { get; set; }

	public string SalesCreditForeignAcc { get; set; }

	public string ShippedGoodsAccount { get; set; }

	public string StockInTransitAccount { get; set; }

	public string StockInflationAdjustAccount { get; set; }

	public string StockInflationOffsetAccount { get; set; }

	public int ToleranceDays { get; set; }

	public string TransfersAccount { get; set; }

	public List<UserFieldsImpl> UserFields { get; set; }

	public string VATInRevenueAccount { get; set; }

	public string VarianceAccount { get; set; }

	public string WHIncomingCenvatAccount { get; set; }

	public string WHOutgoingCenvatAccount { get; set; }

	public string WIPMaterialAccount { get; set; }

	public string WIPMaterialVarianceAccount { get; set; }

	public List<ItemGroups_WarehouseInfoImpl> WarehouseInfo { get; set; }

	public string WipOffsetProfitAndLossAccount { get; set; }

	public int ITMSGRPCOD { get; set; }

	public string ITMSGRPNAM { get; set; }

	public string Locked { get; set; }

	public int UserSign { get; set; }

	public string InvntSys { get; set; }
}
