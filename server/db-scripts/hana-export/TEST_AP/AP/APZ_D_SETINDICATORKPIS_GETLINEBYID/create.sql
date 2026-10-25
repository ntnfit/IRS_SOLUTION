
Create PROCEDURE APZ_D_SetIndicatorKPIs_GetLineById(in code nvarchar(50) default '')
AS
BEGIN
	select a.*,b."DisplayCode",b."FormulaType",b."KpiType",b."Currency",
		case
			 WHEN b."FormulaType"='M' Then b."FormulaCode"
			 When b."FormulaType"='F' Then c."Name"
			end "FormulaCode",
		case
			 WHEN a."Type"='K' Then b."Name"
			 When a."Type"='S' Then d."Name"
		end "Name"
	 from APZ_D_KPIS1 a	left join APZ_D_OKPI b on b."Code"=a."KPI_Code"
						left join APZ_D_KPIT c on c."Code"=b."FormulaCode"
						left join APZ_D_KPIS d on d."Code"=a."KPI_Code"
		where a."Code"=:code;
end





