CREATE procedure ILD_DIY_Chan_Luu_Outsourcing
(
	in ItemCodeGI nvarchar(50),
	in ItemCodeGR nvarchar(50),
	in U_Type int
 
)
as
BEGIN
	--declare ItemCodeGI nvarchar(50)  ='21100001';
	--declare ItemCodeGR nvarchar(50)  = '21100001';
	--declare U_Type int = 4;
	declare ItemGroupGI int;
	declare ItemGroupGR int;
	declare Result nvarchar(20) ='OK';
	
	
	select "ItmsGrpCod" into ItemGroupGI  default 0 from  OITM where "ItemCode"  = :ItemCodeGI;
	select "ItmsGrpCod" into ItemGroupGR  default 0 from  OITM where "ItemCode"  = :ItemCodeGR;
	
	
	if(U_Type = 1) then  -- Upgrade
		if (:ItemGroupGI =  120 or  :ItemGroupGR = 121) then
			Result  = 'Error';
		end if;
	end if;
	--
	if(U_Type =2) then  -- DownGrade
		if (:ItemGroupGI =  121 or  :ItemGroupGR = 120) then
			Result  = 'Error';
		end if;
	end if;
	--
	if(U_Type =3 or  U_Type =4 ) then  --  Merge/Split	
		if (:ItemCodeGI  <> :ItemCodeGR) then
			Result  = 'Error';
		end if;
	end if;
	---
	select Result from dummy; 
 
END
