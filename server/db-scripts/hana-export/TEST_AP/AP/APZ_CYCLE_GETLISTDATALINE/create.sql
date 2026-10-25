
CREATE PROCEDURE Apz_Cycle_GetListDataline
(
in whs nvarchar(4000),
in fromCycle int,
in toCycle int,
in fromItem nvarchar(50),
in toItem nvarchar(50)
)
AS 
BEGIN

select c."ItemCode",c."ItemName", a."WhsCode",a."Entry",b."Name",d."NextDate",b."Frequency",d."Time",Cast("Counted" as char(1)) from ccs1 a 
inner  join OCYC b on a."CycleCode"=b."Code" 
inner join oitm c on a."Entry"=c."ItmsGrpCod"
inner join ITW1 d on d."ItemCode"=c."ItemCode"
where	
		(IFNULL(:whs, '') = '' or a."WhsCode" in (select Data from  Split(:whs,','))) and
		(ifnull(:fromCycle, 0) = 0 or a."CycleCode" >= :fromCycle) and
		(ifnull(:toCycle, 0) = 0 or  a."CycleCode" <= :toCycle) and
		IfNULL(:fromItem,'')='' or  c."ItemCode" BETWEEN :fromItem AND :toItem  and d."NextDate" <= CURRENT_DATE;

END;




