CREATE procedure ILD_R_IRS_Traceability_Items()
as
begin
	select "ItemCode", "ItemName" from OITM where "ItmsGrpCod"=113; -- Group nguyên vật liệu chính
end

