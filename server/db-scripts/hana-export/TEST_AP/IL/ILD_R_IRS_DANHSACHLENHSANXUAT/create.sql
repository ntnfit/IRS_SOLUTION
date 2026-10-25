CREATE procedure ILD_R_IRS_DanhSachLenhSanXuat (
	in FromDate	date,
	in ToDate	date
	
)
as
begin
	select
		a."DocEntry",
		a."DocNum",
		CASE a."Status"
			when 'P' then N'Planned'
			when 'R' then N'Released'
			when 'L' then N'Closed'
			when 'C' then 'Canceled'
		end "Status",
		
		Case  a."Type" 
			  when 'S' then 'Standard'
			  when 'P' then 'Special'
			  when 'D' then 'Disassembly'
			  end as "Type",
		
		
		a."PostDate",
		a."StartDate",
		a."ItemCode",
		a."ProdName",
		a."PlannedQty",
		a."CmpltQty",
		a."Uom",
		a."Comments",
		a."PickRmrk",
		a."DueDate",
		(select sum(x."IssuedQty") from WOR1 x where x."DocEntry"=a."DocEntry" and x."IssuedQty">0) as "IssuedQty",
		a."U_POStatus" as "PO_Status"
	from OWOR a
	where
		 a."StartDate" between FromDate and ToDate
		
	order by a."ItemCode",a."DocEntry";
end

