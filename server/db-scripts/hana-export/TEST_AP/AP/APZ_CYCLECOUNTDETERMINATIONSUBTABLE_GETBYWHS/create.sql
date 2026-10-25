
create PROCEDURE apz_CycleCountDeterminationSubtable_GetByWhs(in whs nvarchar(100),in cycleBy int)
AS 
BEGIN
	select T0."ItmsGrpCod" as "Entry",T0."ItmsGrpNam" as "Name",:whs as "WhsCode",T1."CycleCode",t1."Alert","DestUser","NextDate","Time","ExcldZrQty","Alerted","ChangExist" from oitb T0 left join ccs1 T1 on T0."ItmsGrpCod"=T1."Entry" and T1."WhsCode"=:whs where 0=:cycleBy
	union all
	select T0."AbsEntry" as "Entry",T0."SLCode" as "Name",:whs as "WhsCode",T1."CycleCode",t1."Alert","DestUser","NextDate","Time","ExcldZrQty","Alerted","ChangExist" from OBSL T0 left join ccs1 T1 on T0."AbsEntry"=T1."Entry" and T1."WhsCode"=:whs where 1=:cycleBy And T0."FldAbs"=1
	union all
	select T0."AbsEntry" as "Entry",T0."SLCode" as "Name",:whs as "WhsCode",T1."CycleCode",t1."Alert","DestUser","NextDate","Time","ExcldZrQty","Alerted","ChangExist" from OBSL T0 left join ccs1 T1  on T0."AbsEntry"=T1."Entry" and T1."WhsCode"=:whs where 2=:cycleBy And T0."FldAbs"=2
	union all
	select T0."AbsEntry" as "Entry",T0."SLCode" as "Name",:whs as "WhsCode",T1."CycleCode",t1."Alert","DestUser","NextDate","Time","ExcldZrQty","Alerted","ChangExist" from OBSL T0 left join ccs1 T1  on T0."AbsEntry"=T1."Entry" and T1."WhsCode"=:whs where 3=:cycleBy And T0."FldAbs"=3
	union all
	select T0."AbsEntry" as "Entry",T0."SLCode" as "Name",:whs as "WhsCode",T1."CycleCode",t1."Alert","DestUser","NextDate","Time","ExcldZrQty","Alerted","ChangExist" from OBSL T0 left join ccs1 T1  on T0."AbsEntry"=T1."Entry" and T1."WhsCode"=:whs where 4=:cycleBy And T0."FldAbs"=4;

END;




