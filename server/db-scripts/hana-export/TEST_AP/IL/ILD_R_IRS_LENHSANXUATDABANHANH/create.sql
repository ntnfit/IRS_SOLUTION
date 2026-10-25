CREATE procedure ILD_R_IRS_LenhSanXuatDaBanHanh
(
	in FromDate date, -- Lay StartDate
	in ToDate date -- Lay StartDate
)

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
		
		case when  a."Status" = 'R' then 'Released' else 'Closed' end as "Status",
		case when  a."Type" = 'S' then 'Standard'
			 when  a."Type" = 'P' then 'Special'
			 when  a."Type" = 'D' then 'Disassembly' end as "Type"
		
		
			 

	from OWOR a
	inner join OITM b on a."ItemCode"  =  b."ItemCode"
	where
		 a."StartDate" between FromDate and ToDate and
		  a."Status" in ('R','L')--R:Released;  L:Closed
	order by  a."DocNum";
end

