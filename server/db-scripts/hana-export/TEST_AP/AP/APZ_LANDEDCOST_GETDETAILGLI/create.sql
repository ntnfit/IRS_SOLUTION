
create procedure APZ_LandedCost_GetDetailGLI
(
in objectType int default 0,
in ids nvarchar(30) default ''
)
as
begin
if :objectType=20
then

	SELECT T1."DocNum" as "DocNum",T0.*
	FROM PDN1 T0 INNER JOIN OPDN T1 ON T1."DocEntry"=T0."DocEntry"  where COMPARESPLIT(T1."DocEntry", :ids,',') = 1;



	-- T1."DocEntry" in(SELECT Data FROM Split(:ids,','));

elseif :objectType=18
then

	select T1."DocNum" as "DocNum",T0.* from PCH1  T0 INNER JOIN OPCH T1 ON T1."DocEntry"=T0."DocEntry" where COMPARESPLIT(T1."DocEntry", :ids,',') = 1;
	-- T1."DocEntry" in(SELECT Data FROM Split(:ids,','));
elseif :objectType=69
then

	select T1."DocNum" as "DocNum", case
	 WHEN T0."BaseType"=20 Then a."UomCode"
	 When T0."BaseType"=18 Then b."UomCode"
	end as "UomCode", T0.* from ipf1  T0 INNER JOIN oipf T1 ON T1."DocEntry"=T0."DocEntry"
		left join PDN1 a on T0."ItemCode"=a."ItemCode" and T0."BaseEntry"=a."DocEntry" 
	left join  PCH1 b on T0."ItemCode"=b."ItemCode" and T0."BaseEntry"=b."DocEntry"
	
	 where COMPARESPLIT(T1."DocEntry", :ids,',') = 1;
	 --T1."DocEntry" in(SELECT Data FROM Split(:ids,','));
end if;
end;




