CREATE procedure USP_PROD_COSTING_PRODUCT_LINES (
)
as
begin
	select "PrjCode" "Code","PrjCode" || ' - ' || "PrjName" "Name" from "OPRJ" where "Active"='Y';
end

