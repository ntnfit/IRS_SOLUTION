CREATE procedure ILD_R_IRS_LenhSanXuatKeHoach
as
begin
	select
		a."DocEntry",
		a."DocNum",
		a."PostDate",
		a."ItemCode",
		a."ProdName",
		a."PlannedQty",
		a."Uom",
		a."Comments",
		a."PickRmrk",
		a."Warehouse",
		case when  a."Type" = 'S' then 'Standard'
			 when  a."Type" = 'P' then 'Special'
			 when  a."Type" = 'D' then 'Disassembly' end as "Type"
	from OWOR a
	where
		  a."Status"  = 'P';
end

