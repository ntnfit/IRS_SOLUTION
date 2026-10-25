-- B1 DEPENDS: BEFORE:PT:CREATE_OBJECTS AFTER:SP:_TmSp_ValidateSpParam
CREATE  procedure _TmSp_UpdateOriPtrByOriObj (in in_Obj_table_name varchar (500), in in_Obj_ID varchar (500), in in_Orig_Obj_table_name varchar (500), in in_Orig_Obj_ID varchar (500))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
begin
declare prev  int :=0;
declare curr  int :=0;
declare lvl int :=0;

--Parameters validation for security issues
call _TmSp_ValidateSpParam(:in_Obj_table_name);
call _TmSp_ValidateSpParam(:in_Obj_ID);
call _TmSp_ValidateSpParam(:in_Orig_Obj_table_name);
call _TmSp_ValidateSpParam(:in_Orig_Obj_ID);

if (:in_Obj_table_name = 'CPI1') 
then
	chain = select cpi1."DocEntry", cpi1."LineNum", pch1."DocEntry" as "OriBAbsEnt", pch1."LineNum" as "OriBLinNum", pch1."ObjType" as "OriBDocTyp", :lvl as "Level"
	from cpi1 inner join pch1 on cpi1."BaseEntry" = pch1."DocEntry" and cpi1."BaseType" = 18 and cpi1."BaseLine" = pch1."LineNum";
else
	chain = select csi1."DocEntry", csi1."LineNum", inv1."DocEntry" as "OriBAbsEnt", inv1."LineNum" as "OriBLinNum", inv1."ObjType" as "OriBDocTyp", :lvl as "Level"
	from csi1 inner join inv1 on csi1."BaseEntry" = inv1."DocEntry" and csi1."BaseType" = 13 and csi1."BaseLine" = inv1."LineNum";
end if;	

SELECT count(*) into curr FROM :chain;

while curr - prev > 0 do
	lvl := :lvl + 1;
	prev = :curr;
	
if (:in_Obj_table_name = 'CPI1') 
then	
	chain = select * from :chain
	union all
	select cpi1."DocEntry", cpi1."LineNum", :chain."OriBAbsEnt", :chain."OriBLinNum", :chain."OriBDocTyp", :lvl as "Level"
	from cpi1 inner join :chain on cpi1."BaseEntry" = :chain."DocEntry" and cpi1."BaseType" = 163 and cpi1."BaseLine" = :chain."LineNum" and :chain."Level" = :lvl -1;
else
	chain = select * from :chain
	union all
	select csi1."DocEntry", csi1."LineNum", :chain."OriBAbsEnt", :chain."OriBLinNum", :chain."OriBDocTyp", :lvl as "Level"
	from csi1 inner join :chain on csi1."BaseEntry" = :chain."DocEntry" and csi1."BaseType" = 165 and csi1."BaseLine" = :chain."LineNum" and :chain."Level" = :lvl -1;
end if;	
	
	select count(*) into curr FROM :chain;	
end while;

if (:in_Obj_table_name = 'CPI1') 
then
	update cpi1 set cpi1."OriBAbsEnt" = :chain."OriBAbsEnt", cpi1."OriBLinNum" = :chain."OriBLinNum", cpi1."OriBDocTyp" = :chain."OriBDocTyp"
	from cpi1 inner join :chain on cpi1."DocEntry" = :chain."DocEntry" and cpi1."LineNum" = :chain."LineNum";
else
	update csi1 set csi1."OriBAbsEnt" = :chain."OriBAbsEnt", csi1."OriBLinNum" = :chain."OriBLinNum", csi1."OriBDocTyp" = :chain."OriBDocTyp"
	from csi1 inner join :chain on csi1."DocEntry" = :chain."DocEntry" and csi1."LineNum" = :chain."LineNum";
end if;

end;


